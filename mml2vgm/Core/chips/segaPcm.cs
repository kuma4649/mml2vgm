using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class segaPcm : ClsChip
    {
        public int Interface = 0;

        public segaPcm(ClsVgm parent, int chipID, string initialPartName, string stPath, bool isSecondary) : base(parent, chipID, initialPartName, stPath, isSecondary)
        {
            _chipType = enmChipType.SEGAPCM;
            _Name = "SEGAPCM";
            _ShortName = "SPCM";
            _ChMax = 16;
            _canUsePcm = true;
            _canUsePI = true;
            IsSecondary = isSecondary;
            dataType = 0x80;
            Frequency = 4026987;
            Interface = 0x00f8000d;

            Ch = new ClsChannel[ChMax];
            SetPartToCh(Ch, initialPartName);
            foreach (ClsChannel ch in Ch)
            {
                ch.Type = enmChannelType.PCM;
                ch.isSecondary = chipID == 1;
                ch.MaxVolume = 127;
            }

            pcmDataInfo = new clsPcmDataInfo[] { new clsPcmDataInfo() };
            pcmDataInfo[0].totalBufPtr = 0L;
            pcmDataInfo[0].use = false;
            if (!isSecondary)
            {
                pcmDataInfo[0].totalBuf = new byte[15] { 0x67, 0x66, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            }
            else
            {
                pcmDataInfo[0].totalBuf = new byte[15] { 0x67, 0x66, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            }

            Envelope = new Function();
            Envelope.Max = 127;
            Envelope.Min = 0;

        }

        public override void InitPart(ref partWork pw)
        {
            pw.MaxVolume = 255;
            pw.volume = pw.MaxVolume;
            pw.port0 = 0xc0;
        }

        public override void InitChip()
        {
            if (!use) return;

            for (int ch = 0; ch < ChMax; ch++)
            {
                partWork pw = lstPartWork[ch];
                pw.MaxVolume = Ch[ch].MaxVolume;
                pw.panL = 127;
                pw.panR = 127;
                pw.volume = pw.MaxVolume;
            }

            if (IsSecondary) parent.dat[0x3b] |= 0x40;
        }


        public int GetSegaPcmFNum(int octave, char noteCmd, int shift)
        {
            int o = octave - 1;
            int n = Const.NOTE.IndexOf(noteCmd) + shift;
            if (n >= 0)
            {
                o += n / 12;
                o = Common.CheckRange(o, 1, 8);
                n %= 12;
            }
            else
            {
                o += n / 12 - 1;
                o = Common.CheckRange(o, 1, 8);
                n %= 12;
                if (n < 0) { n += 12; }
            }

            return ((int)(64 * Const.pcmMTbl[n] * Math.Pow(2, (o - 3))) + 1);
        }

        public void OutSegaPcmKeyOff(partWork pw)
        {
            int adr = pw.ch * 8 + 0x86;
            byte d = (byte)(((pw.pcmBank & 0x3f) << 2) | (pw.pcmLoopAddress != -1 ? 0 : 2) | 1);

            OutSegaPcmPort(pw, adr, d);
        }

        public void OutSegaPcmKeyOn(partWork pw)
        {
            int adr = 0;
            byte d = 0;

            //KeyOff
            OutSegaPcmKeyOff(pw);

            //Volume
            SetVolume(pw);

            //StartAdr
            adr = pw.ch * 8 + 0x85;
            d = (byte)((pw.pcmStartAddress & 0xff00) >> 8);
            OutSegaPcmPort(pw, adr, d);

            //StartAdr
            adr = pw.ch * 8 + 0x84;
            d = (byte)((pw.pcmStartAddress & 0x00ff) >> 0);
            OutSegaPcmPort(pw, adr, d);

            if (pw.pcmLoopAddress != -1)
            {
                if (pw.beforepcmLoopAddress != pw.pcmLoopAddress)
                {
                    //LoopAdr
                    adr = pw.ch * 8 + 0x05;
                    d = (byte)((pw.pcmLoopAddress & 0xff00) >> 8);
                    OutSegaPcmPort(pw, adr, d);

                    //LoopAdr
                    adr = pw.ch * 8 + 0x04;
                    d = (byte)((pw.pcmLoopAddress & 0x00ff) >> 0);
                    OutSegaPcmPort(pw, adr, d);

                    pw.beforepcmLoopAddress = pw.pcmLoopAddress;
                }
            }

            if (pw.beforepcmEndAddress != pw.pcmEndAddress)
            {
                //EndAdr
                adr = pw.ch * 8 + 0x06;
                d = (byte)((pw.pcmEndAddress & 0xff00) >> 8);
                d = (byte)((d != 0) ? (d - 1) : 0);
                OutSegaPcmPort(pw, adr, d);
                pw.beforepcmEndAddress = pw.pcmEndAddress;
            }

            adr = pw.ch * 8 + 0x86;
            d = (byte)(((pw.pcmBank & 0x3f) << 2) | (pw.pcmLoopAddress != -1 ? 0 : 2) | 0);
            OutSegaPcmPort(pw, adr, d);

            if (parent.instPCM[pw.instrument].status != enmPCMSTATUS.ERROR)
            {
                parent.instPCM[pw.instrument].status = enmPCMSTATUS.USED;
            }
        }

        public void OutSegaPcmPort(partWork pw, int adr, byte data)
        {
            parent.OutData(
                pw.port0
                , (byte)adr //ll
                , (byte)(((adr & 0x7f00) >> 8) | (pw.isSecondary ? 0x80 : 0)) //hh
                , data //dd
                );
        }


        public override void StorePcm(Dictionary<int, clsPcm> newDic, KeyValuePair<int, clsPcm> v, byte[] buf,bool is16bit,int samplerate, params object[] option)
        {
            clsPcmDataInfo pi = pcmDataInfo[0];

            try
            {
                long size = buf.Length;
                byte[] newBuf = new byte[size];
                Array.Copy(buf, newBuf, size);
                buf = newBuf;

                //Padding
                if (size % 0x100 != 0)
                {
                    newBuf = Common.PcmPadding(ref buf, ref size, 0x80, 0x100);
                }

                //65536 バイトを超える場合はそれ以降をカット
                if (size > 0x10000)
                {
                    List<byte> n = newBuf.ToList();
                    n.RemoveRange(0x10000, (int)(size - 0x10000));
                    newBuf = n.ToArray();
                    size = 0x10000;
                }

                //パディング(空きが足りない場合はバンクをひとつ進める(0x10000)為、空きを全て埋める)
                int fs = (pi.totalBuf.Length - 15) % 0x10000;
                if (size > 0x10000 - fs)
                {
                    List<byte> n = pi.totalBuf.ToList();
                    for (int i = 0; i < 0x10000 - fs; i++) n.Add(0x80);
                    pi.totalBuf = n.ToArray();
                    pi.totalBufPtr += 0x10000 - fs;
                }

                newDic.Add(
                    v.Key
                    , new clsPcm(
                        v.Value.num
                        , v.Value.seqNum, v.Value.chip
                        , v.Value.isSecondary
                        , v.Value.fileName
                        , v.Value.freq
                        , v.Value.vol
                        , pi.totalBufPtr
                        , pi.totalBufPtr + size
                        , size
                        , pi.totalBufPtr + v.Value.loopAdr
                        , is16bit
                        , samplerate)
                    );

                pi.totalBufPtr += size;
                newBuf = new byte[pi.totalBuf.Length + buf.Length];
                Array.Copy(pi.totalBuf, newBuf, pi.totalBuf.Length);
                Array.Copy(buf, 0, newBuf, pi.totalBuf.Length, buf.Length);

                pi.totalBuf = newBuf;
                Common.SetUInt32bit31(pi.totalBuf, 3, (UInt32)(pi.totalBuf.Length - 7), IsSecondary);
                Common.SetUInt32bit31(pi.totalBuf, 7, (UInt32)(pi.totalBuf.Length - 7));
                pi.use = true;
                pcmDataEasy = pi.use ? pi.totalBuf : null;
            }
            catch
            {
                pi.use= false;
                newDic[v.Key].status = enmPCMSTATUS.ERROR;
            }

        }

        public override void StorePcmRawData(clsPcmDatSeq pds, byte[] buf, bool isRaw, bool is16bit, int samplerate, params object[] option)
        {
            if (!isRaw)
            {
                //Rawファイルは何もしない
                //Wavファイルはエンコ
                buf = Encode(buf, false);
            }

            pcmDataDirect.Add(Common.MakePCMDataBlock((byte)dataType, pds, buf));

        }

        private byte[] Encode(byte[] buf, bool is16bit)
        {
            long size = buf.Length;
            long tSize = buf.Length;
            byte[] newBuf;
            clsPcmDataInfo pi = pcmDataInfo[0];

            //Padding
            if (size % 0x100 != 0)
            {
                size++;
                tSize = size;
                newBuf = new byte[size];
                Array.Copy(buf, newBuf, size - 1);
                buf = newBuf;
                newBuf = Common.PcmPadding(ref buf, ref size, 0x80, 0x100);
            }
            else
            {
                newBuf = new byte[size];
                Array.Copy(buf, newBuf, size);
            }

            //65536 バイトを超える場合はそれ以降をカット
            if (size > 0x10000)
            {
                List<byte> n = newBuf.ToList();
                n.RemoveRange(0x10000, (int)(size - 0x10000));
                newBuf = n.ToArray();
                size = 0x10000;
            }

            //パディング(空きが足りない場合はバンクをひとつ進める(0x10000)為、空きを全て埋める)
            int fs = (pi.totalBuf.Length - 15) % 0x10000;
            if (size > 0x10000 - fs)
            {
                List<byte> n = pi.totalBuf.ToList();
                for (int i = 0; i < 0x10000 - fs; i++) n.Add(0x80);
                pi.totalBuf = n.ToArray();
                pi.totalBufPtr += 0x10000 - fs;
            }

            buf = newBuf;

            return buf;
        }

        public override void SetFNum(partWork pw)
        {
            int f = GetSegaPcmFNum(pw.octaveNow, pw.noteCmd, pw.shift + pw.keyShift);//
            if (pw.bendWaitCounter != -1)
            {
                f = pw.bendFnum;
            }
            f = f + pw.detune;
            for (int lfo = 0; lfo < 4; lfo++)
            {
                if (!pw.lfo[lfo].sw)
                {
                    continue;
                }
                if (pw.lfo[lfo].type != eLfoType.Vibrato)
                {
                    continue;
                }
                f += pw.lfo[lfo].value + pw.lfo[lfo].param[6];
            }

            f = Common.CheckRange(f, 0, 0xff);
            if (pw.freq == f) return;

            pw.freq = f;


            //Delta
            byte data = (byte)(f & 0xff);
            int adr = pw.ch * 8 + 0x07;
            if (pw.beforeFNum != data)
            {
                OutSegaPcmPort(pw, adr, data);
                pw.beforeFNum = data;
            }

        }

        public override void SetVolume(partWork pw)
        {
            int vol = pw.volume;

            if (pw.envelopeMode)
            {
                vol = 0;
                if (pw.envIndex != -1)
                {
                    vol = pw.envVolume - (pw.MaxVolume - pw.volume);
                }
            }

            for (int lfo = 0; lfo < 4; lfo++)
            {
                if (!pw.lfo[lfo].sw)
                {
                    continue;
                }
                if (pw.lfo[lfo].type != eLfoType.Tremolo)
                {
                    continue;
                }
                vol += pw.lfo[lfo].value + pw.lfo[lfo].param[6];
            }

            int vl = vol * pw.panL / pw.MaxVolume;
            int vr = vol * pw.panR / pw.MaxVolume;
            vl = Common.CheckRange(vl, 0, pw.MaxVolume);
            vr = Common.CheckRange(vr, 0, pw.MaxVolume);

            if (pw.beforeLVolume != vl)
            {
                //Volume(Left)
                int adr = pw.ch * 8 + 0x02;
                OutSegaPcmPort(pw, adr, (byte)vl);
                pw.beforeLVolume = vl;
            }

            if (pw.beforeRVolume != vr)
            {
                //Volume(Right)
                int adr = pw.ch * 8 + 0x03;
                OutSegaPcmPort(pw, adr, (byte)vr);
                pw.beforeRVolume = vr;
            }
        }

        public override int GetFNum(partWork pw, int octave, char cmd, int shift)
        {
            return GetSegaPcmFNum(octave, cmd, shift);
        }

        public override void SetKeyOn(partWork pw)
        {
            OutSegaPcmKeyOn(pw);
        }

        public override void SetKeyOff(partWork pw)
        {
            OutSegaPcmKeyOff(pw);
        }

        public override void SetLfoAtKeyOn(partWork pw)
        {
            for (int lfo = 0; lfo < 4; lfo++)
            {
                clsLfo pl = pw.lfo[lfo];
                if (!pl.sw)
                    continue;

                if (pl.param[5] != 1)
                    continue;

                pl.isEnd = false;
                pl.value = (pl.param[0] == 0) ? pl.param[6] : 0;//ディレイ中は振幅補正は適用されない
                pl.waitCounter = pl.param[0];
                pl.direction = pl.param[2] < 0 ? -1 : 1;

                if (pl.type == eLfoType.Vibrato)
                {
                    SetFNum(pw);
                }
                if (pl.type == eLfoType.Tremolo)
                {
                    pw.beforeVolume = -1;
                    SetVolume(pw);
                }
            }
        }

        public override void SetToneDoubler(partWork pw)
        {
            //実装不要
        }

        public override int GetToneDoublerShift(partWork pw, int octave, char noteCmd, int shift)
        {
            return 0;
        }


        public override void CmdY(partWork pw, MML mml)
        {
            if (mml.args[0] is string) return;

            byte adr = (byte)mml.args[0];
            byte dat = (byte)mml.args[1];

            OutSegaPcmPort(pw, adr, dat);
        }

        public override void CmdPan(partWork pw, MML mml)
        {
            int l = (int)mml.args[0];
            int r = (int)mml.args[1];

            l = Common.CheckRange(l, 0, 127);
            r = Common.CheckRange(r, 0, 127);
            pw.panL = l;
            pw.panR = r;
        }

        public override void CmdInstrument(partWork pw, MML mml)
        {
            char type = (char)mml.args[0];
            int n = (int)mml.args[1];

            if (type == 'I')
            {
                msgBox.setErrMsg(msg.get("E14001")
                    , mml.line.Fn
                    , mml.line.Num);
                return;
            }

            if (type == 'T')
            {
                msgBox.setErrMsg(msg.get("E14002")
                    , mml.line.Fn
                    , mml.line.Num);
                return;
            }

            if (type == 'E')
            {
                n = SetEnvelopParamFromInstrument(pw, n,mml);
                return;
            }

            n = Common.CheckRange(n, 0, 255);

            if (!parent.instPCM.ContainsKey(n))
            {
                msgBox.setErrMsg(string.Format(msg.get("E14003"), n)
                    , mml.line.Fn
                    , mml.line.Num);
                return;
            }

            if (parent.instPCM[n].chip != enmChipType.SEGAPCM)
            {
                msgBox.setErrMsg(string.Format(msg.get("E14004"), n)
                    , mml.line.Fn
                    , mml.line.Num);
                return;
            }

            pw.instrument = n;
            pw.pcmStartAddress = (int)parent.instPCM[n].stAdr;
            pw.pcmEndAddress = (int)parent.instPCM[n].edAdr;
            pw.pcmLoopAddress = parent.instPCM[n].loopAdr == 0 ? -1 : (int)parent.instPCM[n].loopAdr;
            pw.pcmBank = (int)((parent.instPCM[n].stAdr >> 16) << 1);

        }

        public override void CmdLoopExtProc(partWork pw, MML mml)
        {
        }

        public override string DispRegion(clsPcm pcm)
        {
            return string.Format("{0,-10} {1,-7} {2,-5:D3} {3,-4:D2} ${4,-7:X4} ${5,-7:X4} {6} ${7,-7:X4}  {8,4} {9}\r\n"
                , Name //0
                , pcm.isSecondary ? "SEC" : "PRI" //1
                , pcm.num //2
                , pcm.stAdr >> 16 //3
                , pcm.stAdr & 0xffff //4
                , pcm.edAdr & 0xffff //5
                , pcm.loopAdr == -1 ? "N/A     " : string.Format("${0,-7:X4}", (pcm.loopAdr & 0xffff)) //6
                , pcm.size //7
                , pcm.is16bit ? 1 : 0 //8
                , pcm.status.ToString() //9
                );
        }


    }
}
