using musicDriverInterface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static MDSound.np.chip.InfoBuffer;

namespace Corex64
{
    public class K054539 : ClsChip
    {
        public List<long> memoryMap = null;
        private int beforeKeyON = -1;
        private int beforeKeyOFF = -1;
        private int beforeloopDpcm = -1;
        private int beforePan12 = -1;
        private int beforePan34 = -1;

        public K054539(ClsVgm parent, int chipID, string initialPartName, string stPath, int chipNumber) : base(parent, chipID, initialPartName, stPath, chipNumber)
        {
            _chipType = enmChipType.K054539;
            _Name = "K054539";
            _ShortName = "K54";
            _ChMax = 8;
            _canUsePcm = true;
            _canUsePI = false;
            ChipNumber = chipNumber;

            Frequency = 18_432_000;
            port = new byte[][] { new byte[] { 0xd3 } };

            if (string.IsNullOrEmpty(initialPartName)) return;

            Ch = new ClsChannel[ChMax];
            SetPartToCh(Ch, initialPartName);
            foreach (ClsChannel ch in Ch)
            {
                ch.Type = enmChannelType.PCM;
                ch.chipNumber = chipID == 1;
                ch.MaxVolume = 64;
            }

            pcmDataInfo = new clsPcmDataInfo[] { new clsPcmDataInfo() };
            pcmDataInfo[0].totalBufPtr = 0L;
            pcmDataInfo[0].use = false;
            if (parent.info.format == enmFormat.ZGM)
            {
                if (parent.ChipCommandSize == 2)
                {
                    pcmDataInfo[0].totalBuf = new byte[] {
                        0x07,0x00, 0x66, 0x8c
                        , 0x00, 0x00, 0x00, 0x00 //size of data
                        , 0x00, 0x00, 0x00, 0x00 //size of the entire ROM
                        , 0x00, 0x00, 0x00, 0x00 //start address of data
                    };
                }
                else
                {
                    pcmDataInfo[0].totalBuf = new byte[] {
                        0x07, 0x66, 0x8c
                        , 0x00, 0x00, 0x00, 0x00 //size of data
                        , 0x00, 0x00, 0x00, 0x00 //size of the entire ROM
                        , 0x00, 0x00, 0x00, 0x00 //start address of data
                    };
                }
            }
            else
            {
                pcmDataInfo[0].totalBuf = new byte[] {
                    0x67, 0x66, 0x8c
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
                pg.MaxVolume = 64;
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
                    pg.panL = 0x08;//Center  (1-7 right, 8 middle, 9-f left)
                    pg.volume = pg.MaxVolume;
                    pg.port = port;
                }
            }

            if (ChipID != 0 && parent.info.format != enmFormat.ZGM)
            {
                parent.dat[0xa3] = new outDatum(enmMMLType.unknown, null, null, (byte)(parent.dat[0xaf].val | 0x40));
            }

            beforeKeyON = -1;
            beforeloopDpcm = -1;

            //Loop Switch all off    Rev volume 255(min) panning 16(center)
            for (int ch = 0; ch < ChMax; ch++)
            {
                parent.OutData((MML)null, port[0], (byte)(ChipID == 0 ? 0x02 : 0x82), (byte)(ch * 2 + 1), 0);
                parent.OutData((MML)null, port[0], (byte)(ChipID == 0 ? 0x00 : 0x80), (byte)(ch * 0x20 + 0x04), (byte)255);
                parent.OutData((MML)null, port[0], (byte)(ChipID == 0 ? 0x00 : 0x80), (byte)(ch * 0x20 + 0x05), (byte)0x88);
            }
            //Channel all Active(0x22c)
            parent.OutData((MML)null, port[0], (byte)(ChipID == 0 ? 0x02 : 0x82), (byte)0x2c, 0xff);
            //Global control(0x22f)
            parent.OutData((MML)null, port[0], (byte)(ChipID == 0 ? 0x02 : 0x82), (byte)0x2f, 0x01);

            currentBaseFreq = 8000.0;
            MakeFreqTable(currentBaseFreq);
        }

        private void MakeFreqTable(double baseFreq)
        {
            n2f = new int[8 * 12];
            for (int o = 0; o < 8; o++)
            {
                //Console.WriteLine("o:{0}", o);
                for (int n = 0; n < 12; n++)
                {
                    double mul = Const.pcmMTbl[n] * Math.Pow(2, (o - 4));
                    n2f[o * 12 + n] = (int)(0x10000 * (double)(baseFreq / (Frequency / 384)) * mul);
                }
            }
        }

        public override void StorePcm(Dictionary<int,Tuple<string, clsPcm>> newDic, KeyValuePair<int, clsPcm> v, byte[] buf, bool is16bit, int samplerate, params object[] option)
        {
            clsPcmDataInfo pi = pcmDataInfo[0];

            try
            {

                List<byte> dBuf = new List<byte>();
                int op = 0;

                //if (is16bit)
                //{
                //    throw new NotImplementedException();
                //}
                //else
                //{
                
                op = 0;//0:8bit(default) 1:16bit 2:4bit dpcm
                if (v.Value.option != null && v.Value.option.Length > 0 && v.Value.option[0] != null && v.Value.option[0] is string)
                {
                    op = int.Parse((string)v.Value.option[0]);
                }

                switch (op)
                {
                    case 0://8bit PCM (default)
                           //8bit unsigned -> 8bit signed
                        dBuf.Add(0x80);//end mark追加(reverse向け)
                        for (int i = 0; i < buf.Length; i++)
                        {
                            byte d = (byte)(buf[i] - 0x80);
                            if (d == 0x80) d = 0x81;//end mark回避
                            dBuf.Add(d);//符号化
                        }
                        dBuf.Add(0x80);//end mark追加
                        break;
                    case 1://16bit PCM
                        short s;
                        dBuf.Add(0x00);//end mark追加(reverse向け)
                        dBuf.Add(0x80);//end mark追加(reverse向け)
                        for (int i = 0; i < buf.Length; i+=2)
                        {
                            if (i + 1 == buf.Length) break;
                            s = (short)(buf[i] + (buf[i + 1] << 8));
                            if (s == (short)(0x8000 - 0x10000)) s = (short)(0x8001 - 0x10000);//end mark回避
                            dBuf.Add((byte)s);
                            dBuf.Add((byte)(s>>8));
                        }
                        dBuf.Add(0x00);//end mark追加
                        dBuf.Add(0x80);//end mark追加
                        break;
                    case 2://4bit dPCM
                        dBuf.Add(0x88);//end mark追加(reverse向け)
                        EncK054539Dpcm(dBuf, buf);
                        dBuf.Add(0x88);//end mark追加
                        buf = dBuf.ToArray();
                        break;
                }
                //}
                buf = dBuf.ToArray();

                long size = buf.Length;
                byte[] newBuf = new byte[size];
                Array.Copy(buf, newBuf, size);
                buf = newBuf;

                int freeAdr = (int)pi.totalBufPtr;

                newDic.Add(
                    v.Key
                    , new Tuple<string, clsPcm>("", new clsPcm(
                        v.Value.num
                        , v.Value.seqNum, v.Value.chip
                        , v.Value.chipNumber
                        , v.Value.fileName
                        , v.Value.freq
                        , v.Value.vol
                        , freeAdr + (op == 1 ? 2 : 1)
                        , freeAdr + size - (op == 1 ? 2 : 1)
                        , size
                        , (v.Value.loopAdr == -1 ? v.Value.loopAdr : (v.Value.loopAdr + freeAdr))
                        , is16bit
                        , samplerate
                        , op)
                    ));

                if (newDic[v.Key].Item2.loopAdr != -1
                    && (v.Value.loopAdr < 0 || v.Value.loopAdr >= size))
                {
                    msgBox.setErrMsg(string.Format(msg.get("E34000")
                        , newDic[v.Key].Item2.loopAdr
                        , size - 1), new LinePos(null, "-"));
                    newDic[v.Key].Item2.loopAdr = -1;
                    newDic[v.Key].Item2.status = enmPCMSTATUS.ERROR;
                }

                //if (freeAdr == pi.totalBufPtr)
                //{
                pi.totalBufPtr += size;
                    newBuf = new byte[pi.totalBuf.Length + buf.Length];
                    Array.Copy(pi.totalBuf, newBuf, pi.totalBuf.Length);
                    Array.Copy(buf, 0, newBuf, pi.totalBuf.Length, buf.Length);
                    pi.totalBuf = newBuf;
                //}
                //else
                //{
                //    Array.Copy(buf, 0, pi.totalBuf, freeAdr + pi.totalHeaderLength, buf.Length);
                //}

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

        readonly sbyte[] dpcmQ = new sbyte[16]{
                0, 1, 2, 4, 8, 16, 32, 64,
                0,-64, -32, -16, -8, -4, -2, -1
            };

        private void EncK054539Dpcm(List<byte> dBuf, byte[] buf)
        {
            byte cmp = 0;
            int oldValue = 0;
            for (int i = 0; i < buf.Length; i++)
            {
                int oriValue = buf[i] - 0x80;
                int abs = sbyte.MaxValue;
                int absJ = 0;
                for (int j = 0; j < dpcmQ.Length; j++)
                {
                    int a = Math.Abs(
                        Math.Min(Math.Max(oldValue + dpcmQ[j], sbyte.MinValue), sbyte.MaxValue) 
                        - oriValue
                        );
                    if (a >= abs) continue;
                    abs = a;
                    absJ = j;
                }

                if (i % 2 == 0) cmp = (byte)absJ; 
                else dBuf.Add((byte)(cmp | (absJ << 4)));

                oldValue += dpcmQ[absJ];
                oldValue = Math.Min(Math.Max(oldValue, sbyte.MinValue), sbyte.MaxValue);
            }
        }

        public override string DispRegion(Tuple<string, clsPcm> pcm)
        {
            int op = 0;
            if (pcm.Item2.option != null
                && pcm.Item2.option.Length > 0
                && pcm.Item2.option[0] != null
                && pcm.Item2.option[0] is int
                )
            {
                op = (int)pcm.Item2.option[0];
            }

            return string.Format("{0,-10} {1,-7} {2,-5:D3} {3,-4:D2} ${4,-7:X4} ${5,-7:X4} {6} ${7,-7:X4}  {8,5}{9}\r\n"
                , Name //0
                , pcm.Item2.chipNumber != 0 ? "SEC" : "PRI" //1
                , pcm.Item2.num //2
                , pcm.Item2.stAdr >> 16 //3
                , pcm.Item2.stAdr & 0xffff //4
                , pcm.Item2.edAdr & 0xffff //5
                , pcm.Item2.loopAdr == -1 ? "N/A     " : string.Format("${0,-7:X4}", (pcm.Item2.loopAdr & 0xffff)) //6
                , pcm.Item2.size //7
                , op == 0 ? "8bit " : (op == 1 ? "16bit" : (op == 2 ? "DPCM " : "???")) //mode //8
                , pcm.Item2.status.ToString() //9
                );
        }

        private void OutK054539Port(MML mml, byte[] cmd, partPage page, int adr, byte data)
        {
            SOutData(
                page,
                mml
                , cmd
                , (byte)((ChipNumber != 0 ? 0x80 : 0x00) + (byte)(adr>>8))
                , (byte)adr
                , data
                );

            Debug.WriteLine("{0:x04} {1:x02}", (ushort)adr, data);
        }

        private void OutK054539KeyOn(partPage page, MML mml)
        {
            int adr = 0;

            if (page.instrument == -1)
            {
                LinePos lp = mml?.line?.Lp;
                if (lp == null) lp = new LinePos(null,"-");
                msgBox.setErrMsg(msg.get("E34005"), lp);
                return;
            }

            //Volume
            SetVolume(page, mml);

            //Address shift
            int stAdr = page.pcmStartAddress + page.addressShift;
            if (!page.reversePlay)
            {
                if (stAdr >= page.pcmEndAddress) stAdr = page.pcmEndAddress - 1;
            }
            else
            {
                if (stAdr <= page.pcmEndAddress) stAdr = page.pcmEndAddress - 1;
            }

            if (page.spg.beforepcmStartAddress != stAdr)
            {
                //StartAdr Lsb
                adr = page.ch * 0x20 + 0x0c;
                OutK054539Port(mml, port[0], page
                    , adr
                    , (byte)stAdr
                    );
                //StartAdr mid
                OutK054539Port(mml, port[0], page
                    , adr + 1
                    , (byte)(stAdr >> 8)
                    );
                //StartAdr msb
                OutK054539Port(mml, port[0], page
                    , adr + 2
                    , (byte)(stAdr >> 16)
                    );

                page.spg.beforepcmStartAddress = stAdr;
            }

            //K054539は終了アドレスの指定不要

            //K054539はループアドレスの指定可能
            int lpAdr = page.pcmLoopAddress;
            adr = page.ch * 0x20 + 0x08;

            if (page.spg.beforepcmLoopAddress != lpAdr)
            {
                //LoopAdr Lsb
                OutK054539Port(mml, port[0], page
                    , adr
                    , (byte)lpAdr
                    );
                //LoopAdr mid
                OutK054539Port(mml, port[0], page
                    , adr + 1
                    , (byte)(lpAdr >> 8)
                    );
                //LoopAdr msb
                OutK054539Port(mml, port[0], page
                    , adr + 2
                    , (byte)(lpAdr >> 16)
                    );

                page.spg.beforepcmEndAddress = lpAdr;
            }
            //Loop Switch
            OutK054539Port(mml, port[0], page
                , 0x200 + page.ch * 2 + 1
                , (byte)(lpAdr != -1 ? 1 : 0)
                );


            if (parent.instPCM[page.instrument].Item2.status != enmPCMSTATUS.ERROR)
            {
                parent.instPCM[page.instrument].Item2.status = enmPCMSTATUS.USED;
            }

        }


        public int GetK054539FNum(MML mml, partPage page, int octave, char noteCmd, int shift, int pitchShift)
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

                return n2f[(o + 1) * 12 + n] + pitchShift;//0xe41;

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

            int f = GetK054539FNum(mml, page, page.octaveNow, page.noteCmd, page.shift + page.keyShift + arpNote, page.pitchShift);//
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
                f += page.lfo[lfo].value + page.lfo[lfo].phase;
            }

            f = Common.CheckRange(f, 0, 0xffffff);
            if (page.spg.freq == f) return;
            page.spg.freq = f;

            int data = f;
            if (page.spg.beforeFNum != data)
            {
                int adr = page.ch * 0x20 + 0x00;
                OutK054539Port(mml, port[0], page
                    , adr + 0
                    , (byte)data);
                OutK054539Port(mml, port[0], page
                    , adr + 1
                    , (byte)(data >> 8));
                OutK054539Port(mml, port[0], page
                    , adr + 2
                    , (byte)(data >> 16));
                page.spg.beforeFNum = data;
            }

        }

        public override int GetFNum(partPage page, MML mml, int octave, char cmd, int shift, int pitchShift)
        {
            return GetK054539FNum(mml, page, octave, cmd, shift, pitchShift);
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
            //page.keyOn = false;
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
                vol += page.lfo[lfo].value + page.lfo[lfo].phase;
            }

            if (page.varpeggioMode)
            {
                vol += page.varpDelta;
            }

            byte data = (byte)Common.CheckRange(vol, 0, 64);
            if (page.spg.beforeVolume != data)
            {
                byte adr = (byte)(page.ch * 0x20 + 0x03);
                OutK054539Port(mml, port[0], page
                    , adr
                    , (byte)(64 - data)
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
                pl.value = 0;// (pl.param[0] == 0) ? pl.param[6] : 0;//ディレイ中は振幅補正は適用されない
                pl.phase = (pl.param[0] == 0) ? pl.param[6] : 0;//ディレイ中は振幅補正は適用されない
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

            page.panL = Common.CheckRange(p, 1, 31);

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
                msgBox.setErrMsg(msg.get("E34001")
                    , mml.line.Lp);
                return;
            }

            if (type == 'T')
            {
                msgBox.setErrMsg(msg.get("E34002")
                    , mml.line.Lp);
                return;
            }

            if (type == 'E')
            {
                n = SetEnvelopParamFromInstrument(page, n, re, mml);
                return;
            }

            page.reversePlay = false;
            if (type == 'R')
            {
                page.reversePlay = true;
            }

            if (re) n = page.instrument + n;
            n = Common.CheckRange(n, 0, 255);

            if (!parent.instPCM.ContainsKey(n))
            {
                msgBox.setErrMsg(string.Format(msg.get("E34003"), n)
                    , mml.line.Lp);
                return;
            }

            if (parent.instPCM[n].Item2.chip != enmChipType.K054539)
            {
                msgBox.setErrMsg(string.Format(msg.get("E34004"), n)
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

            OutK054539Port(mml, port[0], page, adr, dat);
        }

        public override void CmdLoopExtProc(partPage page, MML mml)
        {
        }

        public override void CmdDetune(partPage page, MML mml)
        {
            string cmd = (string)mml.args[0];
            int n = (int)mml.args[1];

            switch (cmd)
            {
                case "D":
                    page.detune = n;
                    break;
                case "D>":
                    page.detune += parent.info.octaveRev ? -n : n;
                    break;
                case "D<":
                    page.detune += parent.info.octaveRev ? n : -n;
                    break;
            }
            page.detune = Common.CheckRange(page.detune, -0xfff, 0xfff);

            SetDummyData(page, mml);
        }

        public override void CmdEffect(partPage page, MML mml)
        {
            if ((string)mml.args[0] != "Rv") return;

            byte ch = (byte)page.ch;
            //Reverb
            switch ((char)mml.args[1])
            {
                case 'D'://delay
                    int v = (int)mml.args[2];
                    v = Common.CheckRange(v, 0, 65535);
                    OutK054539Port(mml, port[0], page
                        , ch * 0x20 + 0x06
                        , (byte)v
                        );
                    OutK054539Port(mml, port[0], page
                        , ch * 0x20 + 0x07
                        , (byte)(v>>8)
                        );
                    break;
                case 'S'://rev volume
                    int sl = (int)mml.args[2];
                    sl = Common.CheckRange(sl, 0, 255);
                    OutK054539Port(mml, port[0], page
                        , ch * 0x20 + 0x04
                        , (byte)(255-sl)
                        );
                    break;
            }
        }


        public override void SetupPageData(partWork pw, partPage page)
        {

            page.spg.keyOn = false;
            page.spg.keyOff = true;
            //SetKeyOff(page, null);

            page.spg.instrument = -1;
            page.spg.reversePlay = false;

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
            
            UpdateKeyOff(mml);

            foreach (partWork pw in lstPartWork)
            {
                partPage page = pw.cpg;
                //instrument
                if (page.spg.instrument != page.instrument||page.spg.reversePlay!=page.reversePlay)
                {
                    page.spg.instrument = page.instrument;
                    page.spg.reversePlay = page.reversePlay;
                    if (!page.reversePlay)
                    {
                        page.pcmStartAddress = (int)parent.instPCM[page.instrument].Item2.stAdr;
                        page.pcmEndAddress = (int)parent.instPCM[page.instrument].Item2.edAdr;
                    }
                    else
                    {
                        page.pcmEndAddress = (int)parent.instPCM[page.instrument].Item2.stAdr;
                        page.pcmStartAddress = (int)parent.instPCM[page.instrument].Item2.edAdr;
                    }
                    page.pcmLoopAddress = (int)parent.instPCM[page.instrument].Item2.loopAdr;
                    page.pcmBank = (int)(parent.instPCM[page.instrument].Item2.option[0]);
                    //page.keyOff = true;
                    //page.spg.keyOn = false;
                }

                if (page.spg.keyOn != page.keyOn)
                {
                    page.spg.keyOn = page.keyOn;
                    OutK054539KeyOn(page, mml);
                }

            }

            //pan
            foreach (partWork pw in lstPartWork)
            {
                if (pw.cpg.beforePanL != pw.cpg.panL)
                {
                    OutK054539Port(mml, port[0], pw.cpg, pw.cpg.ch*0x20+0x05, (byte)(pw.cpg.panL+0x80));
                    pw.cpg.beforePanL = pw.cpg.panL;
                }
            }

            UpdateKeyOn(mml);

        }

        private void UpdateKeyOn(MML mml)
        {
            //KeyOn/Off
            byte v = 0;
            foreach (partWork pw in lstPartWork)
            {
                bool keyOn = pw.cpg.keyOn;
                pw.cpg.keyOn = false;
                if (pw.cpg.beforekeyOn && keyOn)
                {
                    keyOn = false;
                }
                else
                {
                    pw.cpg.beforekeyOn = false;
                }

                v |= (byte)(keyOn ? (1 << pw.cpg.ch) : 0);
                beforeKeyOFF &= (byte)(0xfe << pw.cpg.ch);

                if (keyOn)
                {
                    pw.cpg.beforekeyOn = pw.cpg.keyOn;

                    //Address shift
                    int stAdr = pw.cpg.pcmStartAddress + pw.cpg.addressShift;
                    if (!pw.cpg.reversePlay)
                    {
                        if (stAdr >= pw.cpg.pcmEndAddress) stAdr = pw.cpg.pcmEndAddress - 1;
                    }
                    else
                    {
                        if (stAdr <= pw.cpg.pcmEndAddress) stAdr = pw.cpg.pcmEndAddress - 1;
                    }

                    //StartAdr Lsb
                    int adr = pw.cpg.ch * 0x20 + 0x0c;
                    OutK054539Port(mml, port[0], pw.cpg
                        , adr
                        , (byte)stAdr
                        );
                    //StartAdr mid
                    OutK054539Port(mml, port[0], pw.cpg
                        , adr + 1
                        , (byte)(stAdr >> 8)
                        );
                    //StartAdr msb
                    OutK054539Port(mml, port[0], pw.cpg
                        , adr + 2
                        , (byte)(stAdr >> 16)
                        );

                    adr = pw.cpg.ch * 0x2 + 0x200;
                    OutK054539Port(mml, port[0], pw.cpg
                        , adr + 0
                        , (byte)((pw.cpg.pcmBank << 2) | (pw.cpg.reversePlay ? 0x20 : 0x00))//type (b2-3), reverse (b5) 
                        );

                    pw.cpg.spg.beforepcmStartAddress = stAdr;
                }
            }

            if (beforeKeyON != v)
            {
                OutK054539Port(mml, port[0], lstPartWork[lstPartWork.Count - 1].cpg, 0x214, v);
                beforeKeyON = v;
            }
        }

        private void UpdateKeyOff(MML mml)
        {
            //KeyOn/Off
            byte v = 0;// (byte)(beforeKeyON != -1 ? beforeKeyON : 0);
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
                    v |= (byte)(0x01 << pw.cpg.ch);
                    beforeKeyON &= (byte)(0xfe << pw.cpg.ch);
                    pw.cpg.keyOff = false;
                    //pw.cpg.keyOn = false;
                }
            }

            if (beforeKeyOFF != v)
            {
                OutK054539Port(mml, port[0], lstPartWork[lstPartWork.Count - 1].cpg, 0x215, v);
                beforeKeyOFF = v;
            }

        }
    }
}
