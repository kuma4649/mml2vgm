using musicDriverInterface;
using System;
using System.Collections.Generic;

namespace Core
{
    public class AY8910 : ClsChip
    {
        //public int[] FNumTbl = new int[] {
        //    // TP = M / (ftone * 16)       16:Divider
        //    //   c    c+     d    d+     e     f    f+     g    g+     a    a+     b
        //     0xEE8,0xE12,0xD48,0xC89,0xBD5,0xB2B,0xA8A,0x9F3,0x964,0x8DD,0x85E,0x7E6
        //    ,0x774,0x709,0x6A4,0x645,0x5EA,0x595,0x545,0x4FA,0x4B2,0x46F,0x42F,0x3F3
        //    ,0x3BA,0x384,0x352,0x322,0x2F5,0x2CB,0x2A3,0x27D,0x259,0x237,0x217,0x1F9
        //    ,0x1DD,0x1C2,0x1A9,0x191,0x17B,0x165,0x151,0x13E,0x12D,0x11C,0x10C,0x0FD
        //    ,0x0EF,0x0E1,0x0D4,0x0C9,0x0BD,0x0B3,0x0A9,0x09F,0x096,0x08E,0x086,0x07E
        //    ,0x077,0x071,0x06A,0x064,0x05F,0x059,0x054,0x050,0x04B,0x047,0x043,0x03F
        //    ,0x03C,0x038,0x035,0x032,0x02F,0x02D,0x02A,0x028,0x026,0x023,0x021,0x020
        //    ,0x01E,0x01C,0x01B,0x019,0x018,0x016,0x015,0x014,0x013,0x012,0x011,0x010
        //};

        public byte SSGKeyOn = 0x3f;
        public int noiseFreq = -1;
        public byte ChipType = 0;
        public byte Flags = 0;
        public int[] hsFnumTbl = null;
        private byte DataBankID = 0x09;

        public AY8910(ClsVgm parent, int chipID, string initialPartName, string stPath, int chipNumber) : base(parent, chipID, initialPartName, stPath, chipNumber)
        {
            _chipType = enmChipType.AY8910;
            _Name = "AY8910";
            _ShortName = "AY10";
            _ChMax = 3;
            _canUsePcm = true;
            _canUsePI = false;
            ChipNumber = chipNumber;

            Frequency = 1789750;
            port = new byte[][] { new byte[] { 0xa0 } };

            if (string.IsNullOrEmpty(initialPartName)) return;

            Dictionary<string, List<double>> dic = MakeFNumTbl();
            if (dic != null)
            {
                int c = 0;
                FNumTbl = new int[1][] { new int[96] };
                foreach (double v in dic["FNUM_00"])
                {
                    FNumTbl[0][c++] = (int)v;
                    if (c == FNumTbl[0].Length) break;
                }
                if (dic.ContainsKey("MASTERCLOCK"))
                {
                    Frequency = (int)dic["MASTERCLOCK"][0];
                }
                if (dic.ContainsKey("CHIPTYPE"))
                {
                    ChipType = (byte)(int)dic["CHIPTYPE"][0];
                }
                if (dic.ContainsKey("FLAGS"))
                {
                    Flags = (byte)(int)dic["FLAGS"][0];
                }
                c = 0;
                hsFnumTbl = new int[72];
                foreach (double v in dic["HSFNUM_00"])
                {
                    hsFnumTbl[c++] = (int)v;
                    if (c == hsFnumTbl.Length) break;
                }
            }

            Ch = new ClsChannel[ChMax];
            SetPartToCh(Ch, initialPartName);
            foreach (ClsChannel ch in Ch)
            {
                ch.Type = enmChannelType.SSG;
                ch.chipNumber = chipID == 1;
                ch.MaxVolume = 15;
            }

            Envelope = new Function();
            Envelope.Max = 15;
            Envelope.Min = 0;

            pcmDataInfo = new clsPcmDataInfo[] { new clsPcmDataInfo() };
            pcmDataInfo[0].totalBufPtr = 0L;
            pcmDataInfo[0].use = false;

            if (parent.info.format == enmFormat.ZGM)
            {
                if (parent.ChipCommandSize == 2)
                {
                    if (chipNumber == 0) pcmDataInfo[0].totalBuf = new byte[] { 0x07, 0x00, 0x66, DataBankID, 0x00, 0x00, 0x00, 0x00 };
                    else pcmDataInfo[0].totalBuf = new byte[] { 0x07, 0x00, 0x66, DataBankID, 0x00, 0x00, 0x00, 0x00 };
                }
                else
                {
                    if (chipNumber == 0) pcmDataInfo[0].totalBuf = new byte[] { 0x07, 0x66, DataBankID, 0x00, 0x00, 0x00, 0x00 };
                    else pcmDataInfo[0].totalBuf = new byte[] { 0x07, 0x66, DataBankID, 0x00, 0x00, 0x00, 0x00 };
                }
            }
            else
            {
                if (chipNumber == 0) pcmDataInfo[0].totalBuf = new byte[] { 0x67, 0x66, DataBankID, 0x00, 0x00, 0x00, 0x00 };
                else pcmDataInfo[0].totalBuf = new byte[] { 0x67, 0x66, DataBankID, 0x00, 0x00, 0x00, 0x00 };
            }

            pcmDataInfo[0].totalHeaderLength = pcmDataInfo[0].totalBuf.Length;
            pcmDataInfo[0].totalHeadrSizeOfDataPtr = (parent.ChipCommandSize == 2) ? 4 : 3;


            //from fmgen code
            EmitTable = new int[16];
            double Base = 0x4000 / 3.0 * Math.Pow(10.0, 0.0 / 40.0);
            for (int i = 15; i >= 1; i--)
            {
                EmitTable[i] = (int)(Base);
                Base /= 1.189207115;
                Base /= 1.189207115;
            }
            EmitTable[0] = 0;

            for(int i = 0; i < 16; i++)
            {
                EmitTable[i] = (int)(EmitTable[i] / (EmitTable[15] / 255.0));
            }
        }

        private int[] EmitTable;

        public override void InitPart(partWork pw)
        {
            foreach (partPage pg in pw.pg)
            {
                pg.volume = 15;
                pg.MaxVolume = 15;
                pg.port = port;
            }

        }

        public override void InitChip()
        {
            if (!use) return;

            for (int ch = 0; ch < 3; ch++)
            {
                OutSsgKeyOff(null, lstPartWork[ch].cpg);
                lstPartWork[ch].apg.volume = 0;
            }

            parent.OutData((MML)null, port[0],new byte[]{ 0x07, 0x3f});

            if (ChipID != 0)
            {
                parent.dat[0x77] = new outDatum(enmMMLType.unknown, null, null, (byte)(parent.dat[0x77].val | 0x40));//use Secondary
            }
        }


        public override void StorePcm(Dictionary<int, Tuple<string, clsPcm>> newDic, KeyValuePair<int, clsPcm> v, byte[] buf, bool is16bit, int samplerate, params object[] option)
        {
            clsPcmDataInfo pi = pcmDataInfo[0];

            try
            {
                long size = buf.Length;

                for (int i = 0; i < size; i++)
                {
                    int dis = int.MaxValue;
                    int n = 0;
                    for(int j = 0; j < 16; j++)
                    {
                        int ndis = Math.Abs(buf[i] - EmitTable[j]);
                        if (ndis < dis)
                        {
                            dis = ndis;
                            n = j;
                        }
                    }
                    buf[i] = (byte)n;
                }

                if (newDic.ContainsKey(v.Key))
                {
                    newDic.Remove(v.Key);
                }

                newDic.Add(
                    v.Key
                    , new Tuple<string, clsPcm>("", new clsPcm(
                        v.Value.num
                        , v.Value.seqNum
                        , v.Value.chip
                        , 0
                        , v.Value.fileName
                        , v.Value.freq != -1 ? v.Value.freq : samplerate
                        , v.Value.vol
                        , pi.totalBufPtr
                        , pi.totalBufPtr + size - 1
                        , size
                        , -1
                        , is16bit
                        , samplerate
                        )
                    ));

                pi.totalBufPtr += size;

                byte[] newBuf = new byte[pi.totalBuf.Length + buf.Length];
                Array.Copy(pi.totalBuf, newBuf, pi.totalBuf.Length);
                Array.Copy(buf, 0, newBuf, pi.totalBuf.Length, buf.Length);

                pi.totalBuf = newBuf;

                pi.use = true;
                Common.SetUInt32bit31(
                    pi.totalBuf
                    , pi.totalHeadrSizeOfDataPtr
                    , (UInt32)(pi.totalBuf.Length - (pi.totalHeadrSizeOfDataPtr + 4))
                    , ChipNumber != 0);
                pcmDataEasy = pi.use ? pi.totalBuf : null;

            }
            catch
            {
                pi.use = false;
                return;
            }

        }

        public override void StorePcmRawData(clsPcmDatSeq pds, byte[] buf, bool isRaw, bool is16bit, int samplerate, params object[] option)
        {
            msgBox.setWrnMsg(msg.get("E12007"), new LinePos(null, "-"));
        }



        public void OutSsgKeyOn(MML mml, partPage page)
        {
            if (!page.pcm)
            {
                page.keyOn = true;

                //byte pch = (byte)page.ch;
                //int n = (page.mixer & 0x1) + ((page.mixer & 0x2) << 2);
                //byte data = (byte)(SSGKeyOn | 9 << pch);
                //data &= (byte)(~(n << pch));
                //SSGKeyOn = data;

                SetSsgVolume(mml, page);
                if (page.HardEnvelopeSw || page.hardEnvelopeSync.sw)
                {
                    if (page.hardEnvelopeSync.sw)
                    {
                        byte pch = (byte)page.ch;
                        SOutData(page, mml, port[0], (byte)(0x08 + pch), 0x10);
                    }

                    //parent.OutData(mml, port[0], 0x0d, (byte)(page.HardEnvelopeType & 0xf));
                    SOutData(page, mml, port[0], 0x0d, (byte)(page.HardEnvelopeType & 0xf));
                }
                //parent.OutData(mml, port[0], 0x07, data);

                //SOutData(page, mml, port[0], 0x07, data);
                return;
            }

            if (parent.info.Version == 1.51f)
            {
                return;
            }

            float m = Const.pcmMTbl[page.pcmNote] * (float)Math.Pow(2, (page.pcmOctave - 4));
            page.pcmBaseFreqPerFreq = Information.VGM_SAMPLE_PER_SECOND / ((float)parent.instPCM[page.instrument].Item2.freq * m);
            page.pcmFreqCountBuffer = 0.0f;

            long p = parent.instPCM[page.instrument].Item2.stAdr;
            long s = parent.instPCM[page.instrument].Item2.size;
            long f = parent.instPCM[page.instrument].Item2.freq;
            long w = 0;
            if (page.gatetimePmode)
            {
                if (!page.gatetimeReverse)
                    w = page.waitCounter * page.gatetime / 8L;
                else
                    w = page.waitCounter - page.waitCounter * page.gatetime / 8L;
            }
            else
            {
                if (!page.gatetimeReverse)
                    w = page.waitCounter - page.gatetime;
                else
                    w = page.gatetime;
            }
            if (w > page.waitCounter) w = page.waitCounter;
            if (w < 1) w = 1;
            s = Math.Min(s, (long)(w * parent.info.samplesPerClock * f / 44100.0));

            byte[] cmd;
            if (!page.streamSetup)
            {
                parent.newStreamID++;
                page.streamID = parent.newStreamID;
                if (parent.info.format == enmFormat.ZGM)
                {
                    if (parent.ChipCommandSize == 2) cmd = new byte[] { 0x30, 0x00 };
                    else cmd = new byte[] { 0x30 };
                }
                else cmd = new byte[] { 0x90 };

                int ch = page.ch;
                byte sendCmd = (byte)(0x08 + ch);//0x08:volume

                SOutData(
                    page,
                    mml,
                    // setup stream control
                    cmd
                    , (byte)page.streamID
                    , (byte)(0x12 + (page.chipNumber != 0 ? 0x80 : 0x00)) //0x00 SN76489/SN76496
                    , 0x00//pp 
                    , sendCmd //cc
                              // set stream data
                    , 0x91
                    , (byte)page.streamID
                    , DataBankID // Data BankID(0x08 SN76489/SN76496)
                    , 0x01 // Step Size
                    , 0x00 // StepBase
                    );

                page.streamSetup = true;
            }

            if (page.streamFreq != f)
            {
                if (parent.info.format == enmFormat.ZGM)
                {
                    if (parent.ChipCommandSize == 2) cmd = new byte[] { 0x32, 0x00 };
                    else cmd = new byte[] { 0x32 };
                }
                else cmd = new byte[] { 0x92 };
                //Set Stream Frequency
                SOutData(
                    page,
                    mml,
                    cmd
                    , (byte)page.streamID

                    , (byte)(f & 0xff)
                    , (byte)((f & 0xff00) / 0x100)
                    , (byte)((f & 0xff0000) / 0x10000)
                    , (byte)((f & 0xff000000) / 0x10000)
                    );

                page.streamFreq = f;
            }

            if (parent.info.format == enmFormat.ZGM)
            {
                if (parent.ChipCommandSize == 2) cmd = new byte[] { 0x33, 0x00 };
                else cmd = new byte[] { 0x33 };
            }
            else cmd = new byte[] { 0x93 };
            //Start Stream
            SOutData(
                page,
                mml,
                cmd
                , (byte)page.streamID

                , (byte)(p & 0xff)
                , (byte)((p & 0xff00) / 0x100)
                , (byte)((p & 0xff0000) / 0x10000)
                , (byte)((p & 0xff000000) / 0x10000)

                , 0x01

                , (byte)(s & 0xff)
                , (byte)((s & 0xff00) / 0x100)
                , (byte)((s & 0xff0000) / 0x10000)
                , (byte)((s & 0xff000000) / 0x10000)
                );

            if (parent.instPCM[page.instrument].Item2.status != enmPCMSTATUS.ERROR)
            {
                parent.instPCM[page.instrument].Item2.status = enmPCMSTATUS.USED;
            }

            page.freq = 0;//Flip Flop OFF mode

        }

        public void OutSsgKeyOff(MML mml, partPage page)
        {
            page.keyOn = false;

            if (page.hardEnvelopeSync.sw)
            {
                byte pch = (byte)page.ch;
                SOutData(page, mml, port[0], (byte)(0x08 + pch), 0);
            }
            //byte pch = (byte)page.ch;
            //int n = 9;
            //byte data = (byte)(SSGKeyOn | n << pch);
            //SSGKeyOn = data;

            //parent.OutData(mml, port[0], (byte)(0x08 + pch), 0);
            //SOutData(page, mml, port[0], (byte)(0x08 + pch), 0);
            //page.spg.beforeVolume = -1;
            //parent.OutData(mml, port[0], 0x07, data);

            //SOutData(page, mml, port[0], 0x07, data);
        }

        public void SetSsgVolume(MML mml, partPage page)
        {
            byte pch = (byte)page.ch;

            int vol = page.volume;
            if (page.envelopeMode)
            {
                vol = 0;
                if (page.envIndex != -1)
                {
                    vol = page.volume - (15 - page.envVolume);
                }
            }

            for (int lfo = 0; lfo < 4; lfo++)
            {
                if (!page.lfo[lfo].sw) continue;
                if (page.lfo[lfo].type != eLfoType.Tremolo) continue;

                vol += page.lfo[lfo].value + page.lfo[lfo].param[6];
            }

            if (page.varpeggioMode)
            {
                vol += page.varpDelta;
            }

            vol = Common.CheckRange(vol, 0, 15) + ((page.HardEnvelopeSw || page.hardEnvelopeSync.sw) ? 0x10 : 0x00);

            if (page.spg.beforeVolume != vol)
            {
                //parent.OutData(mml, port[0], (byte)(0x08 + pch), (byte)vol);
                SOutData(page, mml, port[0], (byte)(0x08 + pch), (byte)vol);
                //pw.ppg[pw.cpgNum].beforeVolume = pw.ppg[pw.cpgNum].volume;
                page.spg.beforeVolume = vol;
            }
        }

        public void OutSsgNoise(MML mml, partPage page)
        {
            if (noiseFreq != page.noise)
            {
                noiseFreq = page.noise;
                SOutData(page, mml, port[0], 0x06, (byte)(page.noise & 0x1f));
            }
        }

        public void SetSsgFNum(partPage page, MML mml)
        {
            int arpNote = page.arpFreqMode ? 0 : page.arpDelta;
            int arpFreq = page.arpFreqMode ? page.arpDelta : 0;

            int f = GetSsgFNum(page, mml, page.octaveNow, page.noteCmd, page.shift + page.keyShift + arpNote);//
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
            byte data = (byte)(f & 0xff);
            //parent.OutData(mml, port[0], (byte)(0 + page.ch * 2), data);
            SOutData(page, mml, port[0], (byte)(0 + page.ch * 2), data);

            data = (byte)((f & 0xf00) >> 8);
            //parent.OutData(mml, port[0], (byte)(1 + page.ch * 2), data);
            SOutData(page, mml, port[0], (byte)(1 + page.ch * 2), data);

            if (page.hardEnvelopeSync.sw)
            {
                f = GetSsgHsFNum(page, mml, page.hardEnvelopeSync.octave, page.noteCmd, page.shift + page.keyShift + arpNote);//
                f = f + page.hardEnvelopeSync.detune;
                page.hsFnum = f;

                SOutData(page, mml, port[0], 0x0b, (byte)(page.hsFnum & 0xff));
                SOutData(page, mml, port[0], 0x0c, (byte)((page.hsFnum >> 8) & 0xff));
            }
        }

        public int GetSsgFNum(partPage page, MML mml, int octave, char noteCmd, int shift)
        {
            int o = octave - 1;
            int n = Const.NOTE.IndexOf(noteCmd) + shift;
            o += n / 12;
            o = Common.CheckRange(o, 0, 7);
            n %= 12;

            int f = o * 12 + n;
            if (f < 0) f = 0;
            if (f >= FNumTbl[0].Length) f = FNumTbl[0].Length - 1;

            return FNumTbl[0][f];
        }

        public int GetSsgHsFNum(partPage page, MML mml, int octave, char noteCmd, int shift)
        {
            int o = octave - 1;
            int n = Const.NOTE.IndexOf(noteCmd) + shift;
            o += n / 12;
            o = Common.CheckRange(o, 1, 6);
            n %= 12;

            int f = o * 12 + n;
            if (f < 0) f = 0;
            if (f >= hsFnumTbl.Length) f = hsFnumTbl.Length - 1;

            return hsFnumTbl[f];
        }

        public override int GetFNum(partPage page, MML mml, int octave, char cmd, int shift)
        {
            return GetSsgFNum(page, mml, octave, cmd, shift);
        }


        public override void SetFNum(partPage page, MML mml)
        {
            SetSsgFNum(page, mml);
        }

        public override void SetKeyOn(partPage page, MML mml)
        {
            OutSsgKeyOn(mml, page);
        }

        public override void SetKeyOff(partPage page, MML mml)
        {
            OutSsgKeyOff(mml, page);
        }

        public override void SetVolume(partPage page, MML mml)
        {
            SetSsgVolume(mml, page);
        }

        public override void SetToneDoubler(partPage page, MML mml)
        {
        }

        public override int GetToneDoublerShift(partPage page, int octave, char noteCmd, int shift)
        {
            return 0;
        }

        public override void SetLfoAtKeyOn(partPage page, MML mml)
        {
            for (int lfo = 0; lfo < 4; lfo++)
            {
                clsLfo pl = page.lfo[lfo];
                if (!pl.sw)
                    continue;
                if (pl.type == eLfoType.Wah)
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
                    SetSsgFNum(page, mml);
                }

                if (pl.type == eLfoType.Tremolo)
                {
                    SetSsgVolume(mml, page);
                }

            }
        }

        public override void CmdInstrument(partPage page, MML mml)
        {
            char type = (char)mml.args[0];
            int n = (int)mml.args[1];

            if (type == 'I')
            {
                msgBox.setErrMsg(msg.get("E08000")
                    , mml.line.Lp);
                return;
            }

            if (type == 'T')
            {
                msgBox.setErrMsg(msg.get("E08001")
                    , mml.line.Lp);
                return;
            }

            if (type == 'E')
            {
                SetEnvelopParamFromInstrument(page, n, mml);
                return;
            }

            if (!page.pcm)
            {
                SetEnvelopParamFromInstrument(page, n, mml);
                return;
            }

            if (page.instrument == n) return;

            if (!parent.instPCM.ContainsKey(n))
            {
                msgBox.setErrMsg(string.Format(msg.get("E08002"), n), mml.line.Lp);
                return;
            }

            if (parent.instPCM[n].Item2.chip != enmChipType.AY8910)
            {
                msgBox.setErrMsg(string.Format(msg.get("E08003"), n), mml.line.Lp);
            }

            page.instrument = n;
            SetDummyData(page, mml);
        }

        public override void CmdNoiseToneMixer(partPage page, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 0, 3);
            page.mixer = n;
        }

        public override void CmdNoise(partPage page, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 0, 31);
            page.noise = n;
            OutSsgNoise(mml, page);
        }

        public override void CmdHardEnvelope(partPage page, MML mml)
        {
            string cmd = (string)mml.args[0];
            //int n = 0;

            switch (cmd)
            {
                case "EH":
                    page.hardEnvelopeSync.sw = false;
                    page.HardEnvelopeSpeed = (int)mml.args[1];
                    OutSsgHardEnvSpeed(page, mml);
                    break;
                case "EHON":
                    page.hardEnvelopeSync.sw = false;
                    page.HardEnvelopeSw = true;
                    break;
                case "EHOF":
                    page.hardEnvelopeSync.sw = false;
                    page.HardEnvelopeSw = false;
                    break;
                case "EHT":
                    page.HardEnvelopeType = (int)mml.args[1];
                    break;
            }
        }

        private void OutSsgHardEnvType(partPage page, MML mml)
        {
            if (page.spg.HardEnvelopeType != page.HardEnvelopeType)
            {
                //parent.OutData(mml, port[0], 0x0d, (byte)(n & 0xf));
                SOutData(page, mml, port[0], 0x0d, (byte)(page.HardEnvelopeType & 0xf));
                page.spg.HardEnvelopeType = page.HardEnvelopeType;
            }
        }

        private void OutSsgHardEnvSpeed(partPage page, MML mml)
        {
            page.hardEnvelopeSync.sw = false;
            if (page.spg.HardEnvelopeSpeed != page.HardEnvelopeSpeed)
            {
                //parent.OutData(mml, port[0], 0x0b, (byte)(n & 0xff));
                //parent.OutData(mml, port[0], 0x0c, (byte)((n >> 8) & 0xff));
                SOutData(page, mml, port[0], 0x0b, (byte)(page.HardEnvelopeSpeed & 0xff));
                SOutData(page, mml, port[0], 0x0c, (byte)((page.HardEnvelopeSpeed >> 8) & 0xff));
                page.spg.HardEnvelopeSpeed = page.HardEnvelopeSpeed;
            }
        }

        public override void CmdY(partPage page, MML mml)
        {
            if (mml.args[0] is string) return;

            int adr = (byte)(int)mml.args[0];
            byte dat = (byte)(int)mml.args[1];

            //parent.OutData(mml, port[0], (byte)adr, (byte)dat);
            SOutData(page, mml, port[0], (byte)adr, (byte)dat);
        }

        public override void CmdLoopExtProc(partPage page, MML mml)
        {
            page.spg.freq = -1;
        }

        public override void CmdDetune(partPage page, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, -0xfff, 0xfff);
            page.detune = n;
            SetDummyData(page, mml);
        }

        public override void CmdHardEnvelopeSync(partPage page, MML mml)
        {
            string cmd = (string)mml.args[0];
            switch (cmd)
            {
                case "HSON":
                    page.hardEnvelopeSync.sw = true;
                    page.HardEnvelopeSw = false;
                    break;
                case "HSOF":
                    page.hardEnvelopeSync.sw = false;
                    break;
                case "HSO":
                    page.hardEnvelopeSync.octave = (int)mml.args[1];
                    page.hardEnvelopeSync.octave = Common.CheckRange(page.hardEnvelopeSync.octave, 1, 6);
                    break;
                case "H>":
                    page.hardEnvelopeSync.octave += parent.info.octaveRev ? -1 : 1;
                    page.hardEnvelopeSync.octave = Common.CheckRange(page.hardEnvelopeSync.octave, 1, 6);
                    break;
                case "H<":
                    page.hardEnvelopeSync.octave += parent.info.octaveRev ? 1 : -1;
                    page.hardEnvelopeSync.octave = Common.CheckRange(page.hardEnvelopeSync.octave, 1, 6);
                    break;
                case "HSD":
                    page.hardEnvelopeSync.detune = (int)mml.args[1];
                    break;
                case "HSTN":
                    page.hardEnvelopeSync.tone = true;
                    break;
                case "HSTF":
                    page.hardEnvelopeSync.tone = false;
                    break;
            }
        }

        public override void CmdMode(partPage page, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 0, 1);
            if (n == 1)
            {
                page.freq = 0;//freqをリセット
                page.spg.freq = -1;
                SetFNum(page, mml);
            }

            page.pcm = (n == 1);
            page.instrument = -1;
            page.spg.beforeVolume = -1;
        }

        public override void SetupPageData(partWork pw, partPage page)
        {
            //周波数
            page.spg.freq = -1;
            SetFNum(page, null);

            //ノイズ周波数
            noiseFreq = -1;
            OutSsgNoise(null, page);

            if (!page.hardEnvelopeSync.sw)
            {
                //ハードエンベロープtype
                page.spg.HardEnvelopeType = -1;
                OutSsgHardEnvType(page, null);

                //ハードエンベロープspeed
                page.spg.HardEnvelopeSpeed = -1;
                OutSsgHardEnvSpeed(page, null);
            }
            else
            {
                //ハードエンベロープtype
                page.spg.HardEnvelopeType = -1;
                OutSsgHardEnvType(page, null);
            }


            //音量
            page.spg.beforeVolume = -1;
            SetVolume(page, null);

        }

        public override void MultiChannelCommand(MML mml)
        {
            byte nSSGKeyOn = 0;

            foreach (partWork pw in lstPartWork)
            {
                partPage page = pw.cpg;
                byte data = (byte)(9 << page.ch);
                if (page.keyOn)
                {
                    if (!page.hardEnvelopeSync.sw)
                    {
                        //noise D5/4/3  tone D2/1/0
                        int n = (page.mixer & 0x1) + ((page.mixer & 0x2) << 2);
                        data &= (byte)(~(n << page.ch));
                        nSSGKeyOn |= data;
                    }
                    else
                    {
                        int mx = (page.hardEnvelopeSync.tone ? page.mixer : 0x0);
                        int n = (mx & 0x1) + ((mx & 0x2) << 2);
                        data &= (byte)(~(n << page.ch));
                        nSSGKeyOn |= data;
                    }
                }
                else
                {
                    nSSGKeyOn |= data;
                }
            }

            if (SSGKeyOn != nSSGKeyOn)
            {
                parent.OutData( mml, port[0], 0x07, nSSGKeyOn);
                SSGKeyOn = nSSGKeyOn;
            }
        }

    }
}