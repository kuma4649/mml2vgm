using musicDriverInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class C352 : ClsChip
    {
        private short[] mulaw_tab;
        public List<long> memoryMap = null;

        public bool executeKeyonoff { get; private set; }

        public C352(ClsVgm parent, int chipID, string initialPartName, string stPath, int chipNumber) : base(parent, chipID, initialPartName, stPath, chipNumber)
        {
            _chipType = enmChipType.C352;
            _Name = "C352";
            _ShortName = "C352";
            _ChMax = 32;
            _canUsePcm = true;
            _canUsePI = true;
            ChipNumber = chipNumber;

            Frequency = 24192000;
            port = new byte[][] { new byte[] { 0xe1 } };

            dataType = 0x92;

            if (string.IsNullOrEmpty(initialPartName)) return;

            Ch = new ClsChannel[ChMax];
            SetPartToCh(Ch, initialPartName);
            foreach (ClsChannel ch in Ch)
            {
                ch.Type = enmChannelType.PCM;
                ch.chipNumber = chipID == 1;
                ch.MaxVolume = 255;
            }

            pcmDataInfo = new clsPcmDataInfo[] { new clsPcmDataInfo() };
            pcmDataInfo[0].totalBufPtr = 0L;
            pcmDataInfo[0].use = false;
            if (parent.info.format == enmFormat.ZGM)
            {
                if (parent.ChipCommandSize == 2)
                    pcmDataInfo[0].totalBuf = new byte[] { 0x07, 0x00, 0x66, 0x92, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                else
                    pcmDataInfo[0].totalBuf = new byte[] { 0x07, 0x66, 0x92, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            }
            else
                pcmDataInfo[0].totalBuf = new byte[] { 0x67, 0x66, 0x92, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            pcmDataInfo[0].totalHeaderLength = pcmDataInfo[0].totalBuf.Length;
            pcmDataInfo[0].totalHeadrSizeOfDataPtr = (parent.ChipCommandSize == 2) ? 4 : 3;

            Envelope = new Function();
            Envelope.Max = 255;
            Envelope.Min = 0;

            makeC352Table();
            memoryMap = new List<long>();
        }

        public override void InitPart(partWork pw)
        {
            pw.MaxVolume = 256;
            pw.volume = pw.MaxVolume;
            pw.port = port;
        }

        public override void InitChip()
        {
            if (!use) return;

            for (int ch = 0; ch < ChMax; ch++)
            {
                partWork pw = lstPartWork[ch];
                pw.MaxVolume = Ch[ch].MaxVolume;
                pw.panL = 255;
                pw.panR = 255;
                pw.panRL = 0;
                pw.panRR = 0;
                pw.volume = pw.MaxVolume;
            }

            if (ChipNumber != 0)
            {
                //C352はDualChip非サポート
                //parent.dat[0xef] = new outDatum(enmMMLType.unknown, null, null, (byte)(parent.dat[0xef].val | 0x40));
            }
        }

        /// <summary>
        ///  8bit unsigned Wave ->  8bit signed LinnerPCM
        /// 16bit signed   Wave ->  8bit signed mu-law
        /// </summary>
        /// <param name="newDic"></param>
        /// <param name="v"></param>
        /// <param name="buf"></param>
        /// <param name="option"></param>
        public override void StorePcm(Dictionary<int, clsPcm> newDic, KeyValuePair<int, clsPcm> v, byte[] buf, bool is16bit, int samplerate, params object[] option)
        {
            clsPcmDataInfo pi = pcmDataInfo[0];

            try
            {

                if (is16bit)
                {
                    //16bit signed Only
                    buf = standbyPCM(buf, true, false);
                }
                else
                {
                    //8bit unsigned Only
                    buf = standbyPCM(buf, false, false);
                }

                long size = buf.Length;
                byte[] newBuf = new byte[size];
                Array.Copy(buf, newBuf, size);
                buf = newBuf;

                //65536 バイトを超える場合はそれ以降をカット
                if (size > 0x10000)
                {
                    List<byte> n = newBuf.ToList();
                    n.RemoveRange(0x10000, (int)(size - 0x10000));
                    newBuf = n.ToArray();
                    size = 0x10000;
                }

                //空いているBankを探す
                int freeBank = 0;
                int freeAdr = 0;
                do
                {
                    if (memoryMap.Count < freeBank + 1)
                    {
                        memoryMap.Add(0);
                        freeAdr = 0 + freeBank * 0x10000;
                        memoryMap[freeBank] += size;
                        break;
                    }

                    if (size < 0x10000 - memoryMap[freeBank])
                    {
                        freeAdr = (int)(memoryMap[freeBank] + freeBank * 0x10000);
                        memoryMap[freeBank] += size;
                        break;
                    }

                    freeBank++;

                } while (true);

                //パディング(空きが足りない場合はバンクをひとつ進める(0x10000)為、空きを全て埋める)
                while (freeAdr > pi.totalBuf.Length - pi.totalHeaderLength)
                {
                    int fs = (pi.totalBuf.Length - pi.totalHeaderLength) % 0x10000;

                    List<byte> n = pi.totalBuf.ToList();
                    for (int i = 0; i < 0x10000 - fs; i++) n.Add(0x00);
                    pi.totalBuf = n.ToArray();
                    pi.totalBufPtr += 0x10000 - fs;
                }

                newDic.Add(
                    v.Key
                    , new clsPcm(
                        v.Value.num
                        , v.Value.seqNum, v.Value.chip
                        , v.Value.chipNumber
                        , v.Value.fileName
                        , v.Value.freq
                        , v.Value.vol
                        , freeAdr
                        , freeAdr + size - 1
                        , size
                        , (v.Value.loopAdr == -1 ? v.Value.loopAdr : (v.Value.loopAdr + freeAdr))
                        , is16bit
                        , samplerate)
                    );

                if (newDic[v.Key].loopAdr != -1 && (v.Value.loopAdr < 0 || v.Value.loopAdr >= size))
                {
                    msgBox.setErrMsg(string.Format(msg.get("E09000")
                        , newDic[v.Key].loopAdr
                        , size - 1), new LinePos("-"));
                    newDic[v.Key].loopAdr = -1;
                    newDic[v.Key].status = enmPCMSTATUS.ERROR;
                }

                if (freeAdr == pi.totalBufPtr)
                {
                    pi.totalBufPtr += size;

                    newBuf = new byte[pi.totalBuf.Length + buf.Length];
                    Array.Copy(pi.totalBuf, newBuf, pi.totalBuf.Length);
                    Array.Copy(buf, 0, newBuf, pi.totalBuf.Length, buf.Length);
                    pi.totalBuf = newBuf;
                }
                else
                {
                    Array.Copy(buf, 0, pi.totalBuf, freeAdr + pi.totalHeaderLength, buf.Length);
                }

                Common.SetUInt32bit31(
                    pi.totalBuf
                    , pi.totalHeadrSizeOfDataPtr
                    , (UInt32)(pi.totalBuf.Length - (pi.totalHeadrSizeOfDataPtr + 4))
                    , ChipNumber != 0
                    );//size of data ( totalHeadrSizeOfDataPtr:サイズ値を設定する位置 4:サイズ値の大きさ(4byte32bit) )
                Common.SetUInt32bit31(
                    pi.totalBuf
                    , pi.totalHeadrSizeOfDataPtr + 4
                    , (UInt32)(pi.totalBuf.Length - (pi.totalHeadrSizeOfDataPtr + 4 + 4 + 4))
                    );//size of the entire ROM
                pi.use = true;
                pcmDataEasy = pi.use ? pi.totalBuf : null;
            }
            catch
            {
                pi.use = false;
                newDic[v.Key].status = enmPCMSTATUS.ERROR;
            }

            //for(int i = 0; i < pi.totalBuf.Length; i++)
            //{
            //    if (i % 16 == 0)
            //    {
            //        Console.Write("\r\n{0:x06}::", i);
            //    }
            //    Console.Write("{0:x02} ", pi.totalBuf[i]);
            //}
        }

        public override void StorePcmRawData(clsPcmDatSeq pds, byte[] buf, bool isRaw, bool is16bit, int samplerate, params object[] option)
        {
            bool isCPCM = false;

            //Optionをチェック が0以外の場合、bufに対し13bitCompPCM向け圧縮処理を行う
            if (pds.Option != null && pds.Option.Length > 0)
            {
                isCPCM = pds.Option[0].ToString() != "0";
            }

            List<byte> dBuf = new List<byte>();
            if (isRaw)
            {
                //Rawファイル

                //圧縮指定がある時　圧縮処理を実施
                //それ以外は そのまま
                if (isCPCM) buf = standbyPCM(buf, false, true);
                else dBuf.AddRange(buf);
            }
            else
            {
                //Wavファイル

                //16bitWavの時　圧縮処理を実施
                //それ以外は8bitLiner処理を実施
                if (is16bit) buf = standbyPCM(buf, true, false);
                else buf = standbyPCM(buf, false, false);
            }

            pcmDataDirect.Add(Common.MakePCMDataBlock(dataType, pds, buf));
        }

        private byte[] standbyPCM(byte[] pcmbuf, bool is16bitSrc, bool mulaw)
        {
            byte[] dest = new byte[pcmbuf.Length / (is16bitSrc ? 2 : 1)];

            // 8/16bitPCM  ->  8bitPCM/mu-law(C352)
            for (int i = 0; i < dest.Length; i++)
            {
                int m;
                if (is16bitSrc)
                {
                    short dat = (short)(pcmbuf[i * 2] + pcmbuf[i * 2 + 1] * 0x100);
                    if (mulaw) m = mulawConversion(dat);
                    else m = dat >> 8;

                    dest[i] = (byte)m;
                    continue;
                }

                if (mulaw)
                {
                    short dat = (short)(pcmbuf[i] << 8);
                    m = mulawConversion(dat);
                }
                else m = pcmbuf[i] ^ 0x80;
                dest[i] = (byte)m;
            }

            return dest;
        }


        /// <summary>
        /// C352向けmulawテーブルのインデックスに変換します
        /// Thank you! Ian Karlsson(@SuperCTR)!
        /// </summary>
        private int mulawConversion(short dat)
        {
            int m;
            dat += 0x10;
            int k;
            k = Math.Abs(dat);
            if (k > 0x7a00)
                m = 0x7f;
            else if (k > 0x4400)
                m = 0x64 + ((k - 0x4400) / 0x200);
            else if (k > 0x1000)
                m = 0x30 + ((k - 0x1000) / 0x100);
            else if (k > 0x400)
                m = 0x18 + ((k - 0x400) / 0x80);
            else if (k > 0x200)
                m = 0x10 + ((k - 0x200) / 0x40);
            else
                m = k / 0x20;

            m |= ((dat < 0) ? 0x80 : 0);
            //Console.WriteLine("org:{0} enc:{1} dis:{2}", dat, mulaw_tab[m], Math.Abs(dat - mulaw_tab[m]));
            return m;
        }

        /// <summary>
        /// C352向けmulawテーブルを生成します
        /// Thank you! musicalman!
        /// </summary>
        private void makeC352Table()
        {
            mulaw_tab = new short[256];

            int j = 0;

            for (int i = 0; i < 128; i++)
            {
                mulaw_tab[i] = (short)(j << 5);
                if (i < 16)
                    j += 1;
                else if (i < 24)
                    j += 2;
                else if (i < 48)
                    j += 4;
                else if (i < 100)
                    j += 8;
                else
                    j += 16;
            }
            for (int i = 128; i < 256; i++)
                mulaw_tab[i] = (short)((~mulaw_tab[i - 128]) & 0xffe0);
        }



        public int GetC352FNum(MML mml, partWork pw, int octave, char noteCmd, int shift)
        {
            try
            {
                int o = octave - 1;
                int n = Const.NOTE.IndexOf(noteCmd) + shift;

                o += n / 12;
                n %= 12;
                if (n < 0)
                {
                    n += 12;
                    o = Common.CheckRange(--o, 0, 7);
                }

                if (pw.instrument < 0 || !parent.instPCM.ContainsKey(pw.instrument))
                {
                    return 0;
                }

                if (parent.instPCM[pw.instrument].freq == -1)
                {
                    return ((int)(
                        1.531931
                        * 8000.0
                        * Const.pcmMTbl[n]
                        * Math.Pow(2, (o - 3))
                        * ((double)parent.instPCM[pw.instrument].samplerate / 8000.0)
                        ));
                }
                else
                {
                    return ((int)(
                        1.531931
                        * 8000.0
                        * Const.pcmMTbl[n]
                        * Math.Pow(2, (o - 3))
                        * ((double)parent.instPCM[pw.instrument].freq / 8000.0)
                        ));
                }

            }
            catch
            {
                return 0;
            }
        }

        public void OutC352Port(MML mml, partWork pw, int adr, int data)
        {
            parent.OutData(
                mml,
                pw.port[0]
                , (byte)(adr >> 8)
                , (byte)(adr)
                , (byte)(data >> 8)
                , (byte)(data)
                );
        }



        public override void SetFNum(partWork pw, MML mml)
        {
            int f = GetC352FNum(mml, pw, pw.octaveNow, pw.noteCmd, pw.shift + pw.keyShift);//
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

            f = Common.CheckRange(f, 0, 0xffff);
            if (pw.freq == f) return;

            pw.freq = f;


            //Delta
            int data = f & 0xffff;
            if (pw.beforeFNum != data)
            {
                int adr = pw.ch * 8 + 0x02;//0x02:freq
                OutC352Port(mml, pw
                    , adr
                    , data);
                pw.beforeFNum = data;
            }

        }

        public override void SetKeyOn(partWork pw, MML mml)
        {
            pw.keyOn = true;
            pw.keyOff = false;
            pw.changeFlag = true;
            executeKeyonoff = true;

            //Volume
            SetVolume(pw, mml);

            //Address shift
            int stAdr = pw.pcmStartAddress + pw.addressShift;
            if (stAdr >= pw.pcmEndAddress) stAdr = pw.pcmEndAddress - 1;

            int adr, data;
            if (pw.beforepcmStartAddress != stAdr)
            {
                //StartAdr
                adr = pw.ch * 8 + 0x05;
                data = (ushort)(stAdr & 0xffff);
                OutC352Port(mml, pw, adr, data);

                pw.beforepcmStartAddress = stAdr;
            }

            if (pw.beforepcmEndAddress != pw.pcmEndAddress)
            {
                int eAdr = pw.pcmEndAddress;
                //EndAdr
                adr = pw.ch * 8 + 0x06;
                data = (ushort)(eAdr & 0xffff);
                OutC352Port(mml, pw, adr, data);

                pw.beforepcmEndAddress = pw.pcmEndAddress;
            }

            if (pw.beforepcmLoopAddress != pw.pcmLoopAddress)
            {
                if (pw.pcmLoopAddress != -1)
                {
                    //LoopAdr H
                    adr = pw.ch * 8 + 0x07;
                    data = (ushort)(pw.pcmLoopAddress & 0xffff);
                    OutC352Port(mml, pw, adr, data);

                    pw.beforepcmLoopAddress = pw.pcmLoopAddress;
                }
            }

            if (pw.beforepcmBank != pw.pcmBank)
            {
                adr = pw.ch * 8 + 0x04;
                data = (ushort)(pw.pcmBank & 0xffff);
                OutC352Port(mml, pw, adr, data);

                pw.beforepcmBank = pw.pcmBank;
            }

        }

        public override void SetKeyOff(partWork pw, MML mml)
        {
            pw.keyOn = false;
            pw.keyOff = true;
            pw.changeFlag = true;
            executeKeyonoff = true;
        }

        public override void SetVolume(partWork pw, MML mml)
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
            //Console.WriteLine("{0} ", pw.envVolume);

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
            int rvl = vol * pw.panRL / pw.MaxVolume;
            int rvr = vol * pw.panRR / pw.MaxVolume;
            rvl = Common.CheckRange(rvl, 0, pw.MaxVolume);
            rvr = Common.CheckRange(rvr, 0, pw.MaxVolume);

            if (pw.beforeLVolume != vl || pw.beforeRVolume != vr)
            {
                //front Volume
                int adr = pw.ch * 8 + 0x00;
                OutC352Port(mml, pw
                    , adr
                    , ((byte)vl << 8) | (byte)vr
                    );
                pw.beforeLVolume = vl;
                pw.beforeRVolume = vr;
            }

            if (pw.beforeRLVolume != rvl || pw.beforeRRVolume != rvr)
            {
                //rear Volume
                int adr = pw.ch * 8 + 0x01;
                OutC352Port(mml, pw
                    , adr
                    , ((byte)rvl << 8) | (byte)rvr
                    );
                pw.beforeRLVolume = rvl;
                pw.beforeRRVolume = rvr;
            }
        }

        public override void SetLfoAtKeyOn(partWork pw, MML mml)
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
                pl.depthWaitCounter = pl.param[7];
                pl.depth = pl.param[3];
                pl.depthV2 = pl.param[2];

                if (pl.type == eLfoType.Vibrato)
                {
                    SetFNum(pw, mml);
                }
                if (pl.type == eLfoType.Tremolo)
                {
                    pw.beforeVolume = -1;
                    SetVolume(pw, mml);
                }
            }
        }

        public override void CmdInstrument(partWork pw, MML mml)
        {
            char type = (char)mml.args[0];
            int n = (int)mml.args[1];

            if (type == 'I')
            {
                msgBox.setErrMsg(msg.get("E27001")
                    , mml.line.Lp);
                return;
            }

            if (type == 'T')
            {
                msgBox.setErrMsg(msg.get("E27002")
                    , mml.line.Lp);
                return;
            }

            if (type == 'E')
            {
                n = SetEnvelopParamFromInstrument(pw, n, mml);
                return;
            }

            n = Common.CheckRange(n, 0, 255);

            if (!parent.instPCM.ContainsKey(n))
            {
                msgBox.setErrMsg(string.Format(msg.get("E27003"), n)
                    , mml.line.Lp);
                return;
            }

            if (parent.instPCM[n].chip != enmChipType.C352)
            {
                msgBox.setErrMsg(string.Format(msg.get("E27004"), n)
                    , mml.line.Lp);
                return;
            }

            pw.instrument = n;
            pw.pcmStartAddress = (int)(parent.instPCM[n].stAdr & 0xffff);
            pw.pcmEndAddress = (int)(parent.instPCM[n].edAdr & 0xffff);
            pw.pcmLoopAddress = (int)(parent.instPCM[n].loopAdr & 0xffff);
            pw.pcmBank = (int)((parent.instPCM[n].stAdr >> 16) & 0xffff);
            SetDummyData(pw, mml);

        }

        public override void MultiChannelCommand(MML mml)
        {
            foreach (partWork pw in lstPartWork)
            {
                if (!pw.changeFlag) continue;
                pw.changeFlag = false;

                int flag =
                    (pw.keyOn ? 0x4000 : 0x0000) |
                    (pw.keyOff ? 0x2000 : 0x0000)
                    ;

                OutC352Port(mml, pw, pw.ch * 8 + 3, flag);
            }

            if (executeKeyonoff)
            {
                parent.OutData(mml, port[0], 0x02, 0x02, 0x0, 0x0);//execute keyon/keyoff
                executeKeyonoff = false;
            }
        }

        public override string DispRegion(clsPcm pcm)
        {
            return string.Format("{0,-10} {1,-7} {2,-4:D3} ${3,-4:X4} ${4,-7:X4} ${5,-7:X4} {6} ${7,-7:X4}  {8,4} {9}\r\n"
                , Name //0
                , pcm.chipNumber != 0 ? "SEC" : "PRI" //1
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