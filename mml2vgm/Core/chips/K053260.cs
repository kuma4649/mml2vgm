using musicDriverInterface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public class K053260 : ClsChip
    {
        public List<long> memoryMap = null;
        private int beforeKeyON = -1;
        private int beforeloopDpcm = -1;
        private int beforePan12 = -1;
        private int beforePan34 = -1;

        public K053260(ClsVgm parent, int chipID, string initialPartName, string stPath, int chipNumber) : base(parent, chipID, initialPartName, stPath, chipNumber)
        {
            _chipType = enmChipType.K053260;
            _Name = "K053260";
            _ShortName = "K53";
            _ChMax = 4;
            _canUsePcm = true;
            _canUsePI = false;
            ChipNumber = chipNumber;

            Frequency = 3_579_545;
            port = new byte[][] { new byte[] { 0xba } };

            if (string.IsNullOrEmpty(initialPartName)) return;

            Ch = new ClsChannel[ChMax];
            SetPartToCh(Ch, initialPartName);
            foreach (ClsChannel ch in Ch)
            {
                ch.Type = enmChannelType.PCM;
                ch.chipNumber = chipID == 1;
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
            foreach (partPage pg in pw.pg)
            {
                pg.MaxVolume = 255;
                pg.volume = pg.MaxVolume;
                pg.port = port;
            }
        }

        private int[] n2f;
        private double currentBaseFreq;

        public override void InitChip()
        {
            if (!use) return;

            for (int ch = 0; ch < ChMax; ch++)
            {
                partWork pw = lstPartWork[ch];
                foreach (partPage pg in pw.pg)
                {
                    pg.MaxVolume = Ch[ch].MaxVolume;
                    pg.panL = 0x4;//Center
                    pg.volume = pg.MaxVolume;
                    pg.port = port;
                }
            }

            if (ChipID != 0 && parent.info.format != enmFormat.ZGM)
            {
                parent.dat[0xaf] = new outDatum(enmMMLType.unknown, null, null, (byte)(parent.dat[0xaf].val | 0x40));
            }

            beforeKeyON = -1;
            beforeloopDpcm = -1;
            beforePan12 = -1;
            beforePan34 = -1;

            parent.OutData((MML)null, port[0], (byte)((ChipNumber != 0 ? 0x80 : 0x00) + 0x2f), 3);

            currentBaseFreq = 8000.0;
            MakeFreqTable(currentBaseFreq);
        }

        private void MakeFreqTable(double baseFreq)
        {
            n2f = new int[8 * 12];
            int ind = 0;
            double dis = double.MaxValue;
            for (int o = 0; o < 8; o++)
            {
                //Console.WriteLine("o:{0}", o);
                for (int n = 0; n < 12; n++)
                {
                    double frq = baseFreq * Const.pcmMTbl[n] * Math.Pow(2, (o - 4));
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

        public override void StorePcm(Dictionary<int,Tuple<string, clsPcm>> newDic, KeyValuePair<int, clsPcm> v, byte[] buf, bool is16bit, int samplerate, params object[] option)
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
                    , new Tuple<string, clsPcm>("", new clsPcm(
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
                        , samplerate
                        , isDpcm)
                    ));

                if (newDic[v.Key].Item2.loopAdr != -1 && (v.Value.loopAdr < 0 || v.Value.loopAdr >= size))
                {
                    msgBox.setErrMsg(string.Format(msg.get("E23000")
                        , newDic[v.Key].Item2.loopAdr
                        , size - 1), new LinePos(null,"-"));
                    newDic[v.Key].Item2.loopAdr = -1;
                    newDic[v.Key].Item2.status = enmPCMSTATUS.ERROR;
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
                newDic[v.Key].Item2.status = enmPCMSTATUS.ERROR;
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

        public override string DispRegion(Tuple<string, clsPcm> pcm)
        {
            bool mode = false;
            if (pcm.Item2.option != null
                && pcm.Item2.option.Length > 0
                && pcm.Item2.option[0] != null
                && pcm.Item2.option[0] is int
                && (int)pcm.Item2.option[0] == 1)
            {
                mode = true;
            }

            return string.Format("{0,-10} {1,-7} {2,-5:D3} {3,-4:D2} ${4,-7:X4} ${5,-7:X4} {6} ${7,-7:X4}  {8,4} {9}\r\n"
                , Name //0
                , pcm.Item2.chipNumber != 0 ? "SEC" : "PRI" //1
                , pcm.Item2.num //2
                , pcm.Item2.stAdr >> 16 //3
                , pcm.Item2.stAdr & 0xffff //4
                , pcm.Item2.edAdr & 0xffff //5
                , pcm.Item2.loopAdr == -1 ? "N/A     " : string.Format("${0,-7:X4}", (pcm.Item2.loopAdr & 0xffff)) //6
                , pcm.Item2.size //7
                , mode ? "DPCM" : "-" //mode //8
                , pcm.Item2.status.ToString() //9
                );
        }

        private void OutK053260Port(MML mml, byte[] cmd, partPage page, byte adr, byte data)
        {
            SOutData(
                page,
                mml
                , cmd
                , (byte)((ChipNumber != 0 ? 0x80 : 0x00) + adr)
                , data
                );

            //Console.WriteLine("{0:x02} {1:x02}", adr, data);
        }

        private void OutK053260KeyOn(partPage page, MML mml)
        {
            byte adr = 0;
            byte data = 0;

            if (page.instrument == -1)
            {
                LinePos lp = mml?.line?.Lp;
                if (lp == null) lp = new LinePos(null,"-");
                msgBox.setErrMsg(msg.get("E23005"), lp);
                return;
            }

            //Volume
            SetVolume(page, mml);

            //Address shift
            int stAdr = page.pcmStartAddress + page.addressShift;
            if (stAdr >= page.pcmEndAddress) stAdr = page.pcmEndAddress - 1;

            if (page.spg.beforepcmStartAddress != stAdr)
            {
                //StartAdr L
                adr = (byte)((page.ch + 1) * 8 + 0x04);
                data = (byte)stAdr;
                OutK053260Port(mml, port[0], page
                    , adr
                    , data
                    );
                //StartAdr H
                adr = (byte)((page.ch + 1) * 8 + 0x05);
                data = (byte)(stAdr >> 8);
                OutK053260Port(mml, port[0], page
                    , adr
                    , data
                    );

                page.spg.beforepcmStartAddress = stAdr;
            }

            //K053260は終了アドレスではなくサイズを指定する
            //また、K053260はバンクをまたぐ演奏が可能(但しサイズは65534まで)
            //TODO:よって現在、パディングを行っているが不要かもしれない。
            int size = page.pcmEndAddress - page.pcmStartAddress + 1;
            if (page.spg.beforepcmEndAddress != size)
            {
                //EndAdr L
                adr = (byte)((page.ch + 1) * 8 + 0x02);
                data = (byte)size;
                OutK053260Port(mml, port[0], page
                    , adr
                    , data
                    );
                //EndAdr H
                adr = (byte)((page.ch + 1) * 8 + 0x03);
                data = (byte)(size >> 8);
                OutK053260Port(mml, port[0], page
                    , adr
                    , data
                    );

                page.spg.beforepcmEndAddress = size;
            }


            //K053260のループはFlagです。trueのとき、StartAddressに戻ります
            //また、他のチャンネルと合わせて設定する必要があります。


            if (page.spg.beforepcmBank != page.pcmBank)
            {
                adr = (byte)((page.ch + 1) * 8 + 0x06);
                data = (byte)(page.pcmBank);
                OutK053260Port(mml, port[0], page
                    , adr
                    , data
                    );

                page.spg.beforepcmBank = page.pcmBank;
            }

            if (parent.instPCM[page.instrument].Item2.status != enmPCMSTATUS.ERROR)
            {
                parent.instPCM[page.instrument].Item2.status = enmPCMSTATUS.USED;
            }

        }


        public int GetK053260FNum(MML mml, partPage page, int octave, char noteCmd, int shift)
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

                if (page.instrument < 0 || !parent.instPCM.ContainsKey(page.instrument))
                {
                    return 0;
                }

                double freq = (double)parent.instPCM[page.instrument].Item2.samplerate;
                if (parent.instPCM[page.instrument].Item2.freq != -1)
                {
                    freq = (double)parent.instPCM[page.instrument].Item2.freq;
                }

                if (currentBaseFreq != freq)
                {
                    MakeFreqTable(freq);
                    currentBaseFreq = freq;
                }

                return n2f[(o + 1) * 12 + n];//0xe41;

            }
            catch
            {
                return 0;
            }
        }

        public override void SetFNum(partPage page, MML mml)
        {
            int arpNote = page.arpFreqMode ? 0 : page.arpDelta;
            int arpFreq = page.arpFreqMode ? page.arpDelta : 0;

            int f = GetK053260FNum(mml, page, page.octaveNow, page.noteCmd, page.shift + page.keyShift + arpNote);//
            if (page.bendWaitCounter != -1)
            {
                f = page.bendFnum;
            }
            f = f + page.detune;
            f = f + arpFreq;
            for (int lfo = 0; lfo < 4; lfo++)
            {
                if (!page.lfo[lfo].sw)
                {
                    continue;
                }
                if (page.lfo[lfo].type != eLfoType.Vibrato)
                {
                    continue;
                }
                f += page.lfo[lfo].value + page.lfo[lfo].param[6];
            }

            f = Common.CheckRange(f, 0, 0xfff);
            if (page.spg.freq == f) return;

            page.spg.freq = f;


            //Delta
            int data = f & 0xfff;
            if (page.spg.beforeFNum != data)
            {
                int adr = (page.ch + 1) * 8 + 0x00;
                OutK053260Port(mml, port[0], page
                    , (byte)adr
                    , (byte)data);
                adr++;
                OutK053260Port(mml, port[0], page
                    , (byte)adr
                    , (byte)((data & 0xf00) >> 8));
                page.spg.beforeFNum = data;
            }

        }

        public override int GetFNum(partPage page, MML mml, int octave, char cmd, int shift)
        {
            return GetK053260FNum(mml, page, octave, cmd, shift);
        }

        public override void SetKeyOn(partPage page, MML mml)
        {
            //OutK053260KeyOn(page, mml);
            page.keyOn = true;
            //page.keyOff = false;
            SetDummyData(page, mml);
            //UpdateKeyOn(mml);
        }

        public override void SetKeyOff(partPage page, MML mml)
        {
            page.keyOn = false;
            page.keyOff = true;
            //UpdateKeyOn(mml);
        }

        public override void SetVolume(partPage page, MML mml)
        {
            int vol = page.volume;

            if (page.envelopeMode)
            {
                vol = 0;
                if (page.envIndex != -1)
                {
                    vol = page.envVolume - (page.MaxVolume - page.volume);
                }
            }
            //Console.WriteLine("{0} ", pw.ppg[pw.cpgNum].envVolume);

            for (int lfo = 0; lfo < 4; lfo++)
            {
                if (!page.lfo[lfo].sw)
                {
                    continue;
                }
                if (page.lfo[lfo].type != eLfoType.Tremolo)
                {
                    continue;
                }
                vol += page.lfo[lfo].value + page.lfo[lfo].param[6];
            }

            if (page.varpeggioMode)
            {
                vol += page.varpDelta;
            }

            byte data = (byte)Common.CheckRange(vol, 0, 127);
            if (page.spg.beforeVolume != data)
            {
                byte adr = (byte)((page.ch + 1) * 8 + 0x07);
                OutK053260Port(mml, port[0], page
                    , adr
                    , data
                    );
                page.spg.beforeVolume = data;
            }
            SetDummyData(page, mml);

        }

        public override void SetLfoAtKeyOn(partPage page, MML mml)
        {
            for (int lfo = 0; lfo < 4; lfo++)
            {
                clsLfo pl = page.lfo[lfo];
                if (!pl.sw)
                    continue;
                if (pl.type == eLfoType.Wah) continue;

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
                    SetFNum(page, mml);
                }
                if (pl.type == eLfoType.Tremolo)
                {
                    page.beforeVolume = -1;
                    SetVolume(page, mml);
                }
            }
        }

        public override void SetToneDoubler(partPage page, MML mml)
        {
            //実装不要
        }

        public override int GetToneDoublerShift(partPage page, int octave, char noteCmd, int shift)
        {
            return 0;
        }

        public override void CmdPan(partPage page, MML mml)
        {
            int p = (int)mml.args[0];

            p = Common.CheckRange(p, 0, 7);
            page.panL = (7 - p);

            SetDummyData(page, mml);
        }

        public override void CmdInstrument(partPage page, MML mml)
        {
            char type;
            bool re = false;
            int n;
            if (mml.args[0] is bool)
            {
                type = (char)mml.args[1];
                re = true;
                n = (int)mml.args[2];
            }
            else
            {
                type = (char)mml.args[0];
                n = (int)mml.args[1];
            }

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
                n = SetEnvelopParamFromInstrument(page, n, re, mml);
                return;
            }

            if (re) n = page.instrument + n;
            n = Common.CheckRange(n, 0, 255);

            if (!parent.instPCM.ContainsKey(n))
            {
                msgBox.setErrMsg(string.Format(msg.get("E23003"), n)
                    , mml.line.Lp);
                return;
            }

            if (parent.instPCM[n].Item2.chip != enmChipType.K053260)
            {
                msgBox.setErrMsg(string.Format(msg.get("E23004"), n)
                    , mml.line.Lp);
                return;
            }

            page.instrument = n;
            SetDummyData(page, mml);

        }

        public override void CmdY(partPage page, MML mml)
        {
            if (mml.args[0] is string) return;

            byte adr = (byte)(int)mml.args[0];
            byte dat = (byte)(int)mml.args[1];

            OutK053260Port(mml, port[0], page, adr, dat);
        }

        public override void CmdLoopExtProc(partPage page, MML mml)
        {
        }

        public override void CmdDetune(partPage page, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, -0xfff, 0xfff);
            page.detune = n;
            SetDummyData(page, mml);
        }



        public override void SetupPageData(partWork pw, partPage page)
        {

            page.spg.keyOn = false;
            page.spg.keyOff = true;
            //SetKeyOff(page, null);

            page.spg.instrument = -1;

            //周波数
            page.spg.freq = -1;
            page.spg.beforeFNum = -1;
            SetFNum(page, null);

            //音量
            page.spg.beforeVolume = -1;
            SetVolume(page, null);

            //パン
            //page.spg.pan = -1;//不要
        }

        public override void MultiChannelCommand(MML mml)
        {
            byte v, w;
            
            UpdateKeyOff(mml);

            foreach (partWork pw in lstPartWork)
            {
                partPage page = pw.cpg;
                //instrument
                if (page.spg.instrument != page.instrument)
                {
                    page.spg.instrument = page.instrument;
                    page.pcmStartAddress = (int)parent.instPCM[page.instrument].Item2.stAdr;
                    page.pcmEndAddress = (int)parent.instPCM[page.instrument].Item2.edAdr;
                    page.pcmLoopAddress = (int)parent.instPCM[page.instrument].Item2.loopAdr;
                    page.pcmBank = (int)((parent.instPCM[page.instrument].Item2.stAdr >> 16));
                    page.keyOff = true;
                    page.spg.keyOn = false;
                }

                if (page.spg.keyOn != page.keyOn)
                {
                    page.spg.keyOn = page.keyOn;
                    OutK053260KeyOn(page, mml);
                }

            }

            //loop flg/dpcm flag
            v = 0;
            foreach (partWork pw in lstPartWork)
            {
                partPage page = pw.cpg;
                v |= (byte)(page.pcmLoopAddress != -1 ? (1 << page.ch) : 0);
                if (page.instrument != -1
                    && parent.instPCM.ContainsKey(page.instrument))
                {
                    v |= (byte)((bool)parent.instPCM[page.instrument].Item2.option[0] ? (16 << page.ch) : 0);
                }
            }

            if (beforeloopDpcm != v)
            {
                OutK053260Port(mml, port[0], lstPartWork[lstPartWork.Count - 1].cpg, 0x2a, v);
                beforeloopDpcm = v;
            }

            //pan
            v = 0;
            w = 0;
            foreach (partWork pw in lstPartWork)
            {
                if (pw.cpg.ch < 2)
                    v |= (byte)((pw.cpg.panL & 0x7) << (pw.cpg.ch == 0 ? 0 : 3));
                else
                    w |= (byte)((pw.cpg.panL & 0x7) << (pw.cpg.ch == 2 ? 0 : 3));
            }

            if (beforePan12 != v)
            {
                OutK053260Port(mml, port[0], lstPartWork[lstPartWork.Count - 1].cpg, 0x2c, v);
                beforePan12 = v;
            }
            if (beforePan34 != w)
            {
                OutK053260Port(mml, port[0], lstPartWork[lstPartWork.Count - 1].cpg, 0x2d, w);
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
                v |= (byte)(pw.cpg.keyOn ? (1 << pw.cpg.ch) : 0);
            }
            if (beforeKeyON != v)
            {
                OutK053260Port(mml, port[0], lstPartWork[lstPartWork.Count - 1].cpg, 0x28, v);
                beforeKeyON = v;
            }

        }

        private void UpdateKeyOff(MML mml)
        {
            //KeyOn/Off
            byte v = (byte)(beforeKeyON != -1 ? beforeKeyON : 0);
            foreach (partWork pw in lstPartWork)
            {
                // keyOffの指示がでていて
                //    + ( 
                //        次のKeyONの指示もでている
                //    or  エンベロープがOFFっている
                //    or  VアルペジオがOFFっている
                //      )
                if (pw.cpg.keyOff && (pw.cpg.keyOn || !pw.cpg.envelopeMode && !pw.cpg.varpeggioMode))
                {
                    v &= (byte)(0xfe << pw.cpg.ch);
                    pw.cpg.keyOff = false;
                }
            }

            if (beforeKeyON != v)
            {
                OutK053260Port(mml, port[0], lstPartWork[lstPartWork.Count - 1].cpg, 0x28, v);
                beforeKeyON = v;
            }

        }
    }
}
