using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class K053260 : ClsChip
    {
        public List<long> memoryMap = null;
        private int beforeKeyON = -1;
        private int beforeloopDpcm = -1;
        private int beforePan12 = -1;
        private int beforePan34 = -1;

        public K053260(ClsVgm parent, int chipID, string initialPartName, string stPath, bool isSecondary) : base(parent, chipID, initialPartName, stPath, isSecondary)
        {
            _chipType = enmChipType.K053260;
            _Name = "K053260";
            _ShortName = "K53";
            _ChMax = 4;
            _canUsePcm = true;
            _canUsePI = false;
            IsSecondary = isSecondary;

            Frequency = 3_579_545;
            port =new byte[][] { new byte[] { 0xba } };

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
            if (parent.info.format == enmFormat.ZGM)
            {
                if (parent.ChipCommandSize == 2)
                {
                    pcmDataInfo[0].totalBuf = new byte[] {
                        0x07,0x00, 0x66, 0x8e
                        , 0x00, 0x00, 0x00, 0x00 //size of data
                        , 0x00, 0x00, 0x00, 0x00 //size of the entire ROM
                        , 0x00, 0x00, 0x00, 0x00 //start address of data
                    };
                }
                else
                {
                    pcmDataInfo[0].totalBuf = new byte[] {
                        0x07, 0x66, 0x8e
                        , 0x00, 0x00, 0x00, 0x00 //size of data
                        , 0x00, 0x00, 0x00, 0x00 //size of the entire ROM
                        , 0x00, 0x00, 0x00, 0x00 //start address of data
                    };
                }
            }
            else
            {
                pcmDataInfo[0].totalBuf = new byte[] {
                    0x67, 0x66, 0x8e
                    , 0x00, 0x00, 0x00, 0x00 //size of data
                    , 0x00, 0x00, 0x00, 0x00 //size of the entire ROM
                    , 0x00, 0x00, 0x00, 0x00 //start address of data
                };
            }

            pcmDataInfo[0].totalHeaderLength = pcmDataInfo[0].totalBuf.Length;
            pcmDataInfo[0].totalHeadrSizeOfDataPtr = (parent.ChipCommandSize == 2) ? 4 : 3;

            memoryMap = new List<long>();
        }

        public override void InitPart(partWork pw)
        {
            pw.MaxVolume = 255;
            pw.volume = pw.MaxVolume;
            pw.port = port;
        }

        private int[] n2f;

        public override void InitChip()
        {
            if (!use) return;

            for (int ch = 0; ch < ChMax; ch++)
            {
                partWork pw = lstPartWork[ch];
                pw.MaxVolume = Ch[ch].MaxVolume;
                pw.panL = 0x4;//Center
                pw.volume = pw.MaxVolume;
                pw.port = port;
            }

            if (IsSecondary)
            {
                parent.dat[0xaf] = new outDatum(enmMMLType.unknown, null, null, (byte)(parent.dat[0xaf].val | 0x40));
            }

            beforeKeyON = -1;
            beforeloopDpcm = -1;
            beforePan12 = -1;
            beforePan34 = -1;

            OutK053260Port(null, port[0], null, 0x2f, 3);

            n2f = new int[8 * 12];
            int ind = 0;
            double dis = double.MaxValue;
            for (int o = 0; o < 8; o++)
            {
                //Console.WriteLine("o:{0}", o);
                for (int n = 0; n < 12; n++)
                {
                    double frq = 8000.0 * Const.pcmMTbl[n] * Math.Pow(2, (o - 4));
                    double kfrq;
                    while (true)
                    {
                        kfrq = (double)(Frequency) / (double)(0x1000 - ind);
                        if (Math.Abs(frq - kfrq) < dis)
                        {
                            dis = Math.Abs(frq - kfrq);
                        }
                        else
                        {
                            n2f[o * 12 + n] = ind - 1;
                            dis = double.MaxValue;
                            break;
                        }
                        ind++;
                    }
                    //Console.Write("{0:x04} ", n2f[o*12+n]);
                }
                //Console.WriteLine("");
            }
        }

        public override void StorePcm(Dictionary<int, clsPcm> newDic, KeyValuePair<int, clsPcm> v, byte[] buf, bool is16bit, int samplerate, params object[] option)
        {
            clsPcmDataInfo pi = pcmDataInfo[0];

            try
            {

                List<byte> dBuf = new List<byte>();
                bool isDpcm = false;

                if (is16bit)
                {
                    throw new NotImplementedException();
                }
                else
                {
                    if (option != null && option.Length > 0 && option[0] != null && option[0] is int)
                    {
                        int op = (int)option[0];
                        if (op == 1)
                        {
                            EncK053260Dpcm(dBuf, buf, false, false);
                            buf = dBuf.ToArray();
                            isDpcm = true;
                        }
                    }
                    else
                    {
                        //8bit unsigned -> 8bit signed
                        for (int i = 0; i < buf.Length; i++)
                        {
                            dBuf.Add((byte)(buf[i] - 0x80));//符号化
                        }
                    }
                }
                buf = dBuf.ToArray();

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
                    //if (size > 0x10000 - fs)
                    //{
                    List<byte> n = pi.totalBuf.ToList();
                    for (int i = 0; i < 0x10000 - fs; i++) n.Add(0x80);
                    pi.totalBuf = n.ToArray();
                    pi.totalBufPtr += 0x10000 - fs;
                    //}
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
                        , freeAdr
                        , freeAdr + size - 1
                        , size
                        , (v.Value.loopAdr == -1 ? v.Value.loopAdr : (v.Value.loopAdr + freeAdr))
                        , is16bit
                        , samplerate
                        , isDpcm)
                    );

                if (newDic[v.Key].loopAdr != -1 && (v.Value.loopAdr < 0 || v.Value.loopAdr >= size))
                {
                    msgBox.setErrMsg(string.Format(msg.get("E23000")
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
                    Array.Copy(buf, 0, pi.totalBuf, freeAdr, buf.Length);
                }

                Common.SetUInt32bit31(
                    pi.totalBuf
                    , pi.totalHeadrSizeOfDataPtr
                    , (UInt32)(pi.totalBuf.Length - (pi.totalHeadrSizeOfDataPtr + 4))
                    , IsSecondary
                    );
                Common.SetUInt32bit31(
                    pi.totalBuf
                    , pi.totalHeadrSizeOfDataPtr + 4
                    , (UInt32)(pi.totalBuf.Length - (pi.totalHeadrSizeOfDataPtr + 4 + 4 + 4))
                    );
                pi.use = true;
                pcmDataEasy = pi.use ? pi.totalBuf : null;
            }
            catch
            {
                pi.use = false;
                newDic[v.Key].status = enmPCMSTATUS.ERROR;
            }
        }

        public override void StorePcmRawData(clsPcmDatSeq pds, byte[] buf, bool isRaw, bool is16bit, int samplerate, params object[] option)
        {
            throw new NotImplementedException();
        }

        private void EncK053260Dpcm(List<byte> dBuf, byte[] buf, bool v1, bool v2)
        {
            throw new NotImplementedException();
        }

        public override string DispRegion(clsPcm pcm)
        {
            bool mode = false;
            if (pcm.option != null
                && pcm.option.Length > 0
                && pcm.option[0] != null
                && pcm.option[0] is int
                && (int)pcm.option[0] == 1)
            {
                mode = true;
            }

            return string.Format("{0,-10} {1,-7} {2,-5:D3} {3,-4:D2} ${4,-7:X4} ${5,-7:X4} {6} ${7,-7:X4}  {8,4} {9}\r\n"
                , Name //0
                , pcm.isSecondary ? "SEC" : "PRI" //1
                , pcm.num //2
                , pcm.stAdr >> 16 //3
                , pcm.stAdr & 0xffff //4
                , pcm.edAdr & 0xffff //5
                , pcm.loopAdr == -1 ? "N/A     " : string.Format("${0,-7:X4}", (pcm.loopAdr & 0xffff)) //6
                , pcm.size //7
                , mode ? "DPCM" : "-" //mode //8
                , pcm.status.ToString() //9
                );
        }

        private void OutK053260Port(MML mml, byte[] cmd, partWork pw, byte adr, byte data)
        {
            parent.OutData(
                mml
                , cmd
                , (byte)((IsSecondary ? 0x80 : 0x00) + adr)
                , data
                );

            //Console.WriteLine("{0:x02} {1:x02}", adr, data);
        }

        private void OutK053260KeyOn(partWork pw, MML mml)
        {
            byte adr = 0;
            byte data = 0;

            if (pw.instrument == -1)
            {
                LinePos lp = mml?.line?.Lp;
                if (lp == null) lp = new LinePos("-");
                msgBox.setErrMsg(msg.get("E23005"), lp);
                return;
            }

            //Volume
            SetVolume(pw, mml);

            //Address shift
            int stAdr = pw.pcmStartAddress + pw.addressShift;
            if (stAdr >= pw.pcmEndAddress) stAdr = pw.pcmEndAddress - 1;

            if (pw.beforepcmStartAddress != stAdr)
            {
                //StartAdr L
                adr = (byte)((pw.ch + 1) * 8 + 0x04);
                data = (byte)stAdr;
                OutK053260Port(mml, port[0], pw
                    , adr
                    , data
                    );
                //StartAdr H
                adr = (byte)((pw.ch + 1) * 8 + 0x05);
                data = (byte)(stAdr >> 8);
                OutK053260Port(mml, port[0], pw
                    , adr
                    , data
                    );

                pw.beforepcmStartAddress = stAdr;
            }

            if (pw.beforepcmEndAddress != pw.pcmEndAddress)
            {
                //EndAdr L
                adr = (byte)((pw.ch + 1) * 8 + 0x02);
                data = (byte)pw.pcmEndAddress;
                OutK053260Port(mml, port[0], pw
                    , adr
                    , data
                    );
                //EndAdr H
                adr = (byte)((pw.ch + 1) * 8 + 0x03);
                data = (byte)(pw.pcmEndAddress >> 8);
                OutK053260Port(mml, port[0], pw
                    , adr
                    , data
                    );

                pw.beforepcmEndAddress = pw.pcmEndAddress;
            }


            //K053260のループはFlagです。trueのとき、StartAddressに戻ります
            //また、他のチャンネルと合わせて設定する必要があります。


            if (pw.beforepcmBank != pw.pcmBank)
            {
                adr = (byte)((pw.ch + 1) * 8 + 0x06);
                data = (byte)(pw.pcmBank);
                OutK053260Port(mml, port[0], pw
                    , adr
                    , data
                    );

                pw.beforepcmBank = pw.pcmBank;
            }

            if (parent.instPCM[pw.instrument].status != enmPCMSTATUS.ERROR)
            {
                parent.instPCM[pw.instrument].status = enmPCMSTATUS.USED;
            }

        }


        public int GetK053260FNum(MML mml, partWork pw, int octave, char noteCmd, int shift)
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

                //if (n >= 0)
                //{
                //    o += n / 12;
                //    o = Common.CheckRange(o, 0, 7);
                //    n %= 12;
                //}
                //else
                //{
                //    o += n / 12 - 1;
                //    o = Common.CheckRange(o, 0, 7);
                //    n %= 12;
                //    if (n < 0) { n += 12; }
                //}

                if (pw.instrument < 0 || !parent.instPCM.ContainsKey(pw.instrument))
                {
                    return 0;
                }

                double freq = (double)parent.instPCM[pw.instrument].samplerate;
                if (parent.instPCM[pw.instrument].freq != -1)
                {
                    freq = (double)parent.instPCM[pw.instrument].freq;
                }

                return n2f[(o + 1) * 12 + n];//0xe41;

            }
            catch
            {
                return 0;
            }
        }

        public override void SetFNum(partWork pw, MML mml)
        {
            int f = GetK053260FNum(mml, pw, pw.octaveNow, pw.noteCmd, pw.shift + pw.keyShift);//
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

            f = Common.CheckRange(f, 0, 0xfff);
            if (pw.freq == f) return;

            pw.freq = f;


            //Delta
            int data = f & 0xfff;
            if (pw.beforeFNum != data)
            {
                int adr = (pw.ch + 1) * 8 + 0x00;
                OutK053260Port(mml, port[0], pw
                    , (byte)adr
                    , (byte)data);
                adr++;
                OutK053260Port(mml, port[0], pw
                    , (byte)adr
                    , (byte)((data & 0xf00) >> 8));
                pw.beforeFNum = data;
            }

        }

        public override int GetFNum(partWork pw, MML mml, int octave, char cmd, int shift)
        {
            return GetK053260FNum(mml, pw, octave, cmd, shift);
        }

        public override void SetKeyOn(partWork pw, MML mml)
        {
            OutK053260KeyOn(pw, mml);
            pw.keyOn = true;
            SetDummyData(pw, mml);
            UpdateKeyOn(mml);
        }

        public override void SetKeyOff(partWork pw, MML mml)
        {
            pw.keyOn = false;
            pw.keyOff = true;
            UpdateKeyOn(mml);
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

            byte data = (byte)(vol & 0x7f);
            if (pw.beforeVolume != data)
            {
                byte adr = (byte)((pw.ch + 1) * 8 + 0x07);
                OutK053260Port(mml, port[0], pw
                    , adr
                    , data
                    );
                pw.beforeVolume = data;
            }
            SetDummyData(pw, mml);

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

        public override void SetToneDoubler(partWork pw)
        {
            //実装不要
        }

        public override int GetToneDoublerShift(partWork pw, int octave, char noteCmd, int shift)
        {
            return 0;
        }

        public override void CmdPan(partWork pw, MML mml)
        {
            int p = (int)mml.args[0];

            p = Common.CheckRange(p, 0, 7);
            pw.panL = (7 - p);

            SetDummyData(pw, mml);
        }

        public override void CmdInstrument(partWork pw, MML mml)
        {
            char type = (char)mml.args[0];
            int n = (int)mml.args[1];

            if (type == 'I')
            {
                msgBox.setErrMsg(msg.get("E23001")
                    , mml.line.Lp);
                return;
            }

            if (type == 'T')
            {
                msgBox.setErrMsg(msg.get("E23002")
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
                msgBox.setErrMsg(string.Format(msg.get("E23003"), n)
                    , mml.line.Lp);
                return;
            }

            if (parent.instPCM[n].chip != enmChipType.K053260)
            {
                msgBox.setErrMsg(string.Format(msg.get("E23004"), n)
                    , mml.line.Lp);
                return;
            }

            pw.instrument = n;
            pw.pcmStartAddress = (int)parent.instPCM[n].stAdr;
            pw.pcmEndAddress = (int)parent.instPCM[n].edAdr;
            pw.pcmLoopAddress = (int)parent.instPCM[n].loopAdr;
            pw.pcmBank = (int)((parent.instPCM[n].stAdr >> 16));
            SetDummyData(pw, mml);

        }

        public override void CmdY(partWork pw, MML mml)
        {
            if (mml.args[0] is string) return;

            byte adr = (byte)mml.args[0];
            byte dat = (byte)mml.args[1];

            OutK053260Port(mml, port[0], pw, adr, dat);
        }

        public override void CmdLoopExtProc(partWork pw, MML mml)
        {
        }

        public override void MultiChannelCommand(MML mml)
        {
            byte v, w;

            //loop flg/dpcm flag
            v = 0;
            foreach (partWork pw in lstPartWork)
            {
                v |= (byte)(pw.pcmLoopAddress != -1 ? (1 << pw.ch) : 0);
                if (pw.instrument != -1
                    && parent.instPCM.ContainsKey(pw.instrument))
                {
                    v |= (byte)((bool)parent.instPCM[pw.instrument].option[0] ? (16 << pw.ch) : 0);
                }
            }
            if (beforeloopDpcm != v)
            {
                OutK053260Port(mml, port[0], null, 0x2a, v);
                beforeloopDpcm = v;
            }

            //pan
            v = 0;
            w = 0;
            foreach (partWork pw in lstPartWork)
            {
                if (pw.ch < 2)
                    v |= (byte)((pw.panL & 0x7) << (pw.ch == 0 ? 0 : 3));
                else
                    w |= (byte)((pw.panL & 0x7) << (pw.ch == 2 ? 0 : 3));
            }
            if (beforePan12 != v)
            {
                OutK053260Port(mml, port[0], null, 0x2c, v);
                beforePan12 = v;
            }
            if (beforePan34 != w)
            {
                OutK053260Port(mml, port[0], null, 0x2d, w);
                beforePan34 = w;
            }

            UpdateKeyOn(mml);

        }

        private void UpdateKeyOn(MML mml)
        {
            //KeyOn/Off
            byte v = 0;
            foreach (partWork pw in lstPartWork)
            {
                v |= (byte)(pw.keyOn ? (1 << pw.ch) : 0);
            }
            if (beforeKeyON != v)
            {
                OutK053260Port(mml, port[0], null, 0x28, v);
                beforeKeyON = v;
            }

        }
    }
}
