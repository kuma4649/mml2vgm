using musicDriverInterface;
using System;
using System.Collections.Generic;

namespace Core
{
    public class YM2608 : ClsOPN
    {
        protected int[][] _FNumTbl = new int[2][] {
            new int[13]
            ,new int[96]
            //new int[] {
            //// OPNA(FM) : TP = (144 * ftone * (2^20) / M) / (2^(B-1))       32:Divider 2:OPNA 
            ////   c    c+     d    d+     e     f    f+     g    g+     a    a+     b    >c
            // 0x289,0x2af,0x2d8,0x303,0x331,0x362,0x395,0x3cc,0x405,0x443,0x484,0x4c8,0x289*2
            //},
            //new int[] {
            //// OPNA(SSG) : TP = M / (ftone * 32 * 2)       32:Divider 2:OPNA 
            ////   c    c+     d    d+     e     f    f+     g    g+     a    a+     b
            // 0xEE8,0xE12,0xD48,0xC89,0xBD5,0xB2B,0xA8A,0x9F3,0x964,0x8DD,0x85E,0x7E6
            //,0x774,0x709,0x6A4,0x645,0x5EA,0x595,0x545,0x4FA,0x4B2,0x46F,0x42F,0x3F3
            //,0x3BA,0x384,0x352,0x322,0x2F5,0x2CB,0x2A3,0x27D,0x259,0x237,0x217,0x1F9
            //,0x1DD,0x1C2,0x1A9,0x191,0x17B,0x165,0x151,0x13E,0x12D,0x11C,0x10C,0x0FD
            //,0x0EF,0x0E1,0x0D4,0x0C9,0x0BD,0x0B3,0x0A9,0x09F,0x096,0x08E,0x086,0x07E
            //,0x077,0x071,0x06A,0x064,0x05F,0x059,0x054,0x050,0x04B,0x047,0x043,0x03F
            //,0x03C,0x038,0x035,0x032,0x02F,0x02D,0x02A,0x028,0x026,0x023,0x021,0x020
            //,0x01E,0x01C,0x01B,0x019,0x018,0x016,0x015,0x014,0x013,0x012,0x011,0x010
            //}
        };

        public int rhythm_TotalVolume = 63;
        public int rhythm_beforeTotalVolume = -1;
        public int rhythm_MAXTotalVolume = 63;
        public byte rhythm_KeyOn = 0;
        public byte rhythm_KeyOff = 0;

        public YM2608(ClsVgm parent, int chipID, string initialPartName, string stPath, int chipNumber) : base(parent, chipID, initialPartName, stPath, chipNumber)
        {
            _chipType = enmChipType.YM2608;
            _Name = "YM2608";
            _ShortName = "OPNA";
            _ChMax = 19;
            _canUsePcm = true;
            _canUsePI = true;
            FNumTbl = _FNumTbl;
            ChipNumber = chipNumber;
            dataType = 0x81;
            Frequency = 7987200;
            port = new byte[][]{
                new byte[] { (byte)(chipNumber!=0 ? 0xa6 : 0x56) }
                , new byte[] { (byte)(chipNumber!=0 ? 0xa7 : 0x57) }
            };
            DataBankID = 0x0d;//TBD(固定値ではなく、恐らくデータごとに連番を振るのが良いと思われる。)

            if (string.IsNullOrEmpty(initialPartName)) return;

            Dictionary<string, List<double>> dic = MakeFNumTbl();
            if (dic != null)
            {
                int c = 0;
                foreach (double v in dic["FNUM_00"])
                {
                    FNumTbl[0][c++] = (int)v;
                    if (c == FNumTbl[0].Length) break;
                }
                FNumTbl[0][FNumTbl[0].Length - 1] = FNumTbl[0][0] * 2;

                c = 0;
                foreach (double v in dic["FNUM_01"])
                {
                    FNumTbl[1][c++] = (int)v;
                    if (c == FNumTbl[1].Length) break;
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
                ch.Type = enmChannelType.FMOPN;
                ch.chipNumber = chipID == 1;
            }

            Ch[2].Type = enmChannelType.FMOPNex;
            Ch[6].Type = enmChannelType.FMOPNex;
            Ch[7].Type = enmChannelType.FMOPNex;
            Ch[8].Type = enmChannelType.FMOPNex;

            Ch[9].Type = enmChannelType.SSG;
            Ch[10].Type = enmChannelType.SSG;
            Ch[11].Type = enmChannelType.SSG;

            Ch[12].Type = enmChannelType.RHYTHM;
            Ch[13].Type = enmChannelType.RHYTHM;
            Ch[14].Type = enmChannelType.RHYTHM;
            Ch[15].Type = enmChannelType.RHYTHM;
            Ch[16].Type = enmChannelType.RHYTHM;
            Ch[17].Type = enmChannelType.RHYTHM;

            Ch[18].Type = enmChannelType.ADPCM;

            pcmDataInfo = new clsPcmDataInfo[] { new clsPcmDataInfo(), new clsPcmDataInfo() };
            pcmDataInfo[0].totalBufPtr = 0L;
            pcmDataInfo[0].use = false;
            pcmDataInfo[1].totalBufPtr = 0L;
            pcmDataInfo[1].use = false;
            if (parent.info.format == enmFormat.ZGM)
            {
                if (parent.ChipCommandSize == 2)
                {
                    if (chipNumber == 0)
                    {
                        pcmDataInfo[0].totalBuf = new byte[] { 0x07, 0x00, 0x66, 0x81, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                        pcmDataInfo[1].totalBuf = new byte[] { 0x07, 0x00, 0x66, DataBankID, 0x00, 0x00, 0x00, 0x00 };
                    }
                    else
                    {
                        pcmDataInfo[0].totalBuf = new byte[] { 0x07, 0x00, 0x66, 0x81, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                        pcmDataInfo[1].totalBuf = new byte[] { 0x07, 0x00, 0x66, DataBankID, 0x00, 0x00, 0x00, 0x00 };
                    }
                }
                else
                {
                    if (chipNumber == 0)
                    {
                        pcmDataInfo[0].totalBuf = new byte[] { 0x07, 0x66, 0x81, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                        pcmDataInfo[1].totalBuf = new byte[] { 0x07, 0x66, DataBankID, 0x00, 0x00, 0x00, 0x00 };
                    }
                    else
                    {
                        pcmDataInfo[0].totalBuf = new byte[] { 0x07, 0x66, 0x81, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                        pcmDataInfo[1].totalBuf = new byte[] { 0x07, 0x66, DataBankID, 0x00, 0x00, 0x00, 0x00 };
                    }
                }
            }
            else
            {
                if (chipNumber == 0)
                {
                    pcmDataInfo[0].totalBuf = new byte[] { 0x67, 0x66, 0x81, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                    pcmDataInfo[1].totalBuf = new byte[] { 0x67, 0x66, DataBankID, 0x00, 0x00, 0x00, 0x00 };
                }
                else
                {
                    pcmDataInfo[0].totalBuf = new byte[] { 0x67, 0x66, 0x81, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                    pcmDataInfo[1].totalBuf = new byte[] { 0x67, 0x66, DataBankID, 0x00, 0x00, 0x00, 0x00 };
                }
            }

            pcmDataInfo[0].totalHeaderLength = pcmDataInfo[0].totalBuf.Length;
            pcmDataInfo[0].totalHeadrSizeOfDataPtr = (parent.ChipCommandSize == 2) ? 4 : 3;
            pcmDataInfo[1].totalHeaderLength = pcmDataInfo[1].totalBuf.Length;
            pcmDataInfo[1].totalHeadrSizeOfDataPtr = (parent.ChipCommandSize == 2) ? 4 : 3;

            Envelope = new Function();
            Envelope.Max = 255;
            Envelope.Min = 0;

        }

        public override void InitChip()
        {
            if (!use) return;

            //initialize shared param
            OutOPNSetHardLfo(null, lstPartWork[0].cpg, false, 0);
            OutOPNSetCh3SpecialMode(null, lstPartWork[0].cpg, false);

            //FM Off
            OutFmAllKeyOff();

            //SSG Off
            for (int ch = 9; ch < 12; ch++)
            {
                OutSsgKeyOff(null, lstPartWork[ch].cpg);
                foreach (partPage page in lstPartWork[ch].pg)
                {
                    page.volume = 0;
                    page.mixer = 1;
                }
            }

            //Use OPNA mode
            parent.OutData((MML)null, port[0], 0x29, 0x82);
            parent.OutData((MML)null, port[1], 0x29, 0x82);
            //SSG vol0
            parent.OutData((MML)null, port[0], 0x08, 0x00);
            parent.OutData((MML)null, port[0], 0x09, 0x00);
            parent.OutData((MML)null, port[0], 0x0a, 0x00);
            //SSG Mixer init
            parent.OutData((MML)null, port[0], 0x07, 0x38);
            //ADPCM Reset
            parent.OutData((MML)null, port[1], 0x10, 0x17);
            parent.OutData((MML)null, port[1], 0x10, 0x80);
            parent.OutData((MML)null, port[1], 0x00, 0x80);
            parent.OutData((MML)null, port[1], 0x01, 0xc0);
            parent.OutData((MML)null, port[1], 0x00, 0x01);

            foreach (partWork pw in lstPartWork)
            {
                foreach (partPage page in pw.pg)
                {
                    if (page.ch == 0)
                    {
                        page.hardLfoSw = false;
                        page.hardLfoNum = 0;
                        OutOPNSetHardLfo(null, page, page.hardLfoSw, page.hardLfoNum);
                    }

                    if (page.ch < 6)
                    {
                        page.pan = 3;
                        page.ams = 0;
                        page.fms = 0;
                        if (!page.dataEnd) OutOPNSetPanAMSPMS(null, page, 3, 0, 0);
                    }
                }
            }

            if (ChipID != 0 && parent.info.format != enmFormat.ZGM)
            {
                parent.dat[0x4b] = new outDatum(enmMMLType.unknown, null, null, (byte)(parent.dat[0x4b].val | 0x40));//use Secondary
            }
        }

        public override void InitPart(partWork pw)
        {
            foreach (partPage page in pw.pg)
            {
                page.slots = (byte)((page.Type == enmChannelType.FMOPN || page.ch == 2) ? 0xf : 0x0);
                page.volume = 127;
                page.MaxVolume = 127;
                if (page.Type == enmChannelType.SSG)
                {
                    //pw.ppg[pw.cpgNum].volume = 32767;
                    page.MaxVolume = 15;
                    page.volume = page.MaxVolume;
                }
                else if (page.Type == enmChannelType.RHYTHM)
                {
                    //pw.ppg[pw.cpgNum].volume = 32767;
                    page.MaxVolume = 31;//5bit
                    page.volume = page.MaxVolume;
                }
                else if (page.Type == enmChannelType.ADPCM)
                {
                    //pw.ppg[pw.cpgNum].volume = 32767;
                    page.MaxVolume = 255;
                    page.volume = page.MaxVolume;
                }

                page.port = port;
                page.noise = 0;
                page.noiseFreq = -1;
            }
        }


        public void SetADPCMAddress(MML mml, partPage page, int startAdr, int endAdr)
        {
            if (page.spg.pcmStartAddress != startAdr)
            {
                SOutData(page, mml, port[1], 0x02, (byte)((startAdr >> 2) & 0xff));
                SOutData(page, mml, port[1], 0x03, (byte)((startAdr >> 10) & 0xff));
                page.spg.pcmStartAddress = startAdr;
            }

            if (page.spg.pcmEndAddress != endAdr)
            {
                SOutData(page, mml, port[1], 0x04, (byte)(((endAdr - 0x04) >> 2) & 0xff));
                SOutData(page, mml, port[1], 0x05, (byte)(((endAdr - 0x04) >> 10) & 0xff));
                page.spg.pcmEndAddress = endAdr;
            }

        }

        public override int GetFNum(partPage page, MML mml, int octave, char cmd, int shift, int pitchShift)
        {
            if (page.Type == enmChannelType.FMOPN
                || page.Type == enmChannelType.FMOPNex
                || (page.Type == enmChannelType.FMPCMex && !page.pcm))
            {
                return GetFmFNum(FNumTbl[0], octave, cmd, shift, pitchShift);
            }
            if (page.Type == enmChannelType.SSG)
            {
                return GetSsgFNum(page, mml, octave, cmd, shift, pitchShift);
            }
            if (page.Type == enmChannelType.ADPCM)
            {
                return GetAdpcmFNum(page, octave, cmd, shift, pitchShift);
            }
            return 0;
        }

        public void SetAdpcmFNum(MML mml, partPage page)
        {
            int arpNote = page.arpFreqMode ? 0 : page.arpDelta;
            int arpFreq = page.arpFreqMode ? page.arpDelta : 0;

            int f = GetAdpcmFNum(page, page.octaveNow, page.noteCmd, page.shift + page.keyShift + arpNote, page.pitchShift);//
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

            f = Common.CheckRange(f, 0, 0xffff);
            if (page.spg.freq == f) return;

            page.freq = f;
            page.spg.freq = f;

            byte data = 0;

            data = (byte)(f & 0xff);
            SOutData(page, mml, port[1], 0x09, data);

            data = (byte)((f & 0xff00) >> 8);
            SOutData(page, mml, port[1], 0x0a, data);
        }

        public void SetAdpcmVolume(MML mml, partPage page)
        {

            int vol = page.volume;
            if (page.envelopeMode)
            {
                vol = 0;
                if (page.envIndex != -1)
                {
                    vol = page.volume - (0xff - page.envVolume);
                }
            }
            vol = Common.CheckRange(vol, 0, 0xff);

            if (page.spg.beforeVolume != vol)
            {
                SOutData(page, mml, port[1], 0x0b, (byte)vol);
                page.spg.beforeVolume = vol;
            }
        }

        public void SetAdpcmPan(MML mml, partPage page)
        {
            if (page.spg.pan != page.pan)
            {
                SOutData(page, mml, port[1], 0x01, (byte)((page.pan & 0x3) << 6));
                page.spg.pan = page.pan;
            }
        }

        public int GetAdpcmFNum(partPage page, int octave, char noteCmd, int shift, int pitchShift)
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

            double freq = 8000.0;
            if (page.instrument != -1)
            {
                freq = (double)parent.instPCM[page.instrument].Item2.samplerate;
                if (parent.instPCM[page.instrument].Item2.freq != -1)
                {
                    freq = (double)parent.instPCM[page.instrument].Item2.freq;
                }
            }

            return (int)(
                //0x4a82 * Const.pcmMTbl[n] * Math.Pow(2, (o - 4)) * (freq / 8000.0)
                0x49d8 * Const.pcmMTbl[n] * Math.Pow(2, (o - 4)) * (freq / 8000.0) + pitchShift
                );
        }

        public void OutAdpcmKeyOn(MML mml, partPage page)
        {

            SetAdpcmVolume(mml, page);
            SOutData(page, mml, port[1], 0x00, 0xa0);

        }

        public void OutAdpcmKeyOff(MML mml, partPage page)
        {

            SOutData(page, mml, port[1], 0x00, 0x01);

        }


        public override void SetFNum(partPage page, MML mml)
        {
            if (page.ch < 9)
                SetFmFNum(page, mml);
            else if (page.Type == enmChannelType.SSG)
            {
                SetSsgFNum(page, mml);
            }
            else if (page.Type == enmChannelType.RHYTHM)
            {
            }
            else if (page.Type == enmChannelType.ADPCM)
            {
                SetAdpcmFNum(mml, page);
            }
        }

        public override void SetKeyOn(partPage page, MML mml)
        {
            SetDummyData(page, mml);
            if (page.Type != enmChannelType.SSG)
            {
                page.keyOn = true;
                //OutFmKeyOn(page, mml);
            }
            else if (page.Type == enmChannelType.SSG)
            {
                OutSsgKeyOn(page, mml);
            }
        }

        public void OutKeyOn(partPage page, MML mml)
        {
            if (page.ch < 9)
                OutFmKeyOn(page, mml);
            else if (page.Type == enmChannelType.SSG)
            {
                OutSsgKeyOn(page, mml);
            }
            else if (page.Type == enmChannelType.RHYTHM)
            {
                page.keyOff = false;
            }
            else if (page.Type == enmChannelType.ADPCM)
            {
                OutAdpcmKeyOn(mml, page);
            }
        }

        public override void SetKeyOff(partPage page, MML mml)
        {
            if (page.ch < 9)
                OutFmKeyOff(page, mml);
            else if (page.Type == enmChannelType.SSG)
            {
                OutSsgKeyOff(mml, page);
            }
            else if (page.Type == enmChannelType.RHYTHM)
            {
                page.keyOn = false;
                page.keyOff = true;
            }
            else if (page.Type == enmChannelType.ADPCM)
            {
                OutAdpcmKeyOff(mml, page);
            }
        }

        public override void SetVolume(partPage page, MML mml)
        {
            base.SetVolume(page, mml);

            if (page.Type == enmChannelType.RHYTHM)
            {
            }
            else if (page.Type == enmChannelType.ADPCM)
            {
                SetAdpcmVolume(mml, page);
            }
        }

        public override void SetLfoAtKeyOn(partPage page, MML mml)
        {
            base.SetLfoAtKeyOn(page, mml);

            for (int lfo = 0; lfo < 4; lfo++)
            {
                clsLfo pl = page.lfo[lfo];
                if (!pl.sw)
                    continue;
                if (pl.type == eLfoType.Hardware)
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
                    if (page.Type == enmChannelType.ADPCM)
                        SetAdpcmFNum(mml, page);

                }

                if (pl.type == eLfoType.Tremolo)
                {
                    page.beforeVolume = -1;

                    //if (pw.ppg[pw.cpgNum].Type == enmChannelType.RHYTHM)
                    //SetRhythmVolume(pw);
                    if (page.Type == enmChannelType.ADPCM)
                        SetAdpcmVolume(mml, page);

                }

            }
        }

        public override void StorePcm(Dictionary<int,Tuple<string, clsPcm>> newDic, KeyValuePair<int, clsPcm> v, byte[] buf, bool is16bit, int samplerate, params object[] option)
        {
            clsPcmDataInfo pi = null;

            try
            {
                EncAdpcmA ea = new EncAdpcmA();
                switch (v.Value.loopAdr)
                {
                    default:
                    case 0:
                        buf = ea.YM_ADPCM_B_Encode(buf, is16bit, false);
                        pi = pcmDataInfo[0];
                        break;
                    case 1://SSGPCM
                        buf = SSGPCM_Encode(buf, is16bit);
                        pi = pcmDataInfo[1];
                        break;
                }

                long size = buf.Length;
                byte[] newBuf;
                newBuf = new byte[size];
                Array.Copy(buf, newBuf, size);
                buf = newBuf;
                long tSize = size;
                size = buf.Length;

                newDic.Add(
                    v.Key
                    ,new Tuple<string, clsPcm>("", new clsPcm(
                        v.Value.num
                        , v.Value.seqNum, v.Value.chip
                        , v.Value.chipNumber
                        , v.Value.fileName
                        , v.Value.freq
                        , v.Value.vol
                        , pi.totalBufPtr
                        , pi.totalBufPtr + size - 1
                        , size
                        , -1
                        , is16bit
                        , samplerate)
                    ));

                pi.totalBufPtr += size;
                newBuf = new byte[pi.totalBuf.Length + buf.Length];
                Array.Copy(pi.totalBuf, newBuf, pi.totalBuf.Length);
                Array.Copy(buf, 0, newBuf, pi.totalBuf.Length, buf.Length);

                pi.totalBuf = newBuf;
                pi.use = true;
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

                pcmDataEasy = pi.use ? pcmDataInfo[0].totalBuf : null;
                pcmDataEasyA = pi.use ? pcmDataInfo[1].totalBuf : null;

            }
            catch
            {
                pi.use = false;
            }

        }

        public override void StorePcmRawData(clsPcmDatSeq pds, byte[] buf, bool isRaw, bool is16bit, int samplerate, params object[] option)
        {
            if (!isRaw)
            {
                //Rawファイルは何もしない
                //Wavファイルはエンコ
                EncAdpcmA ea = new EncAdpcmA();
                buf = ea.YM_ADPCM_B_Encode(buf, is16bit, true);
            }

            pcmDataDirect.Add(Common.MakePCMDataBlock((byte)dataType, pds, buf));

        }

        public override void SetPCMDataBlock(MML mml)
        {
            //if (!CanUsePcm) return;
            if (!use) return;

            SetPCMDataBlock_AB(mml, pcmDataEasy, pcmDataDirect);
            SetPCMDataBlock_AB(mml, pcmDataEasyA, null);
        }

        private void SetPCMDataBlock_AB(MML mml, byte[] pcmDataEasy, List<byte[]> pcmDataDirect)
        {
            int maxSize = 0;
            int ptr = 7 + (parent.ChipCommandSize == 2 ? 2 : 0);

            AdjustPCMData(ptr);

            if (parent.info.format == enmFormat.ZGM)
            {
                if (port.Length < 1) return;

                if (parent.ChipCommandSize != 2)
                {
                    if (port[0].Length < 1) return;

                    if (pcmDataEasy != null && pcmDataEasy.Length > 1) pcmDataEasy[1] = port[0][0];
                    if (pcmDataDirect != null)
                    {
                        for (int i = 0; i < pcmDataDirect.Count; i++)
                        {
                            pcmDataDirect[i][1] = port[0][0];
                        }
                    }
                }
                else
                {
                    if (port[0].Length < 2) return;

                    if (pcmDataEasy != null && pcmDataEasy.Length > 3)
                    {
                        pcmDataEasy[2] = port[0][0];
                        pcmDataEasy[3] = port[0][1];
                    }
                    if (pcmDataDirect != null)
                    {
                        for (int i = 0; i < pcmDataDirect.Count; i++)
                        {
                            pcmDataDirect[i][2] = port[0][0];
                            pcmDataDirect[i][3] = port[0][1];
                        }
                    }
                }
            }

            if (pcmDataEasy != null && pcmDataEasy.Length > ptr+3)
            {
                maxSize =
                    pcmDataEasy[ptr]
                    + (pcmDataEasy[ptr + 1] << 8)
                    + (pcmDataEasy[ptr + 2] << 16)
                    + (pcmDataEasy[ptr + 3] << 24);
            }
            if (pcmDataDirect != null && pcmDataDirect.Count > 0)
            {
                foreach (byte[] dat in pcmDataDirect)
                {
                    if (dat != null && dat.Length > 0)
                    {
                        int size =
                            dat[ptr]
                            + (dat[ptr + 1] << 8)
                            + (dat[ptr + 2] << 16)
                            + (dat[ptr + 3] << 24);
                        if (maxSize < size) maxSize = size;
                    }
                }
            }
            if (pcmDataEasy != null && pcmDataEasy.Length > ptr+3)
            {
                pcmDataEasy[ptr] = (byte)maxSize;
                pcmDataEasy[ptr + 1] = (byte)(maxSize >> 8);
                pcmDataEasy[ptr + 2] = (byte)(maxSize >> 16);
                pcmDataEasy[ptr + 3] = (byte)(maxSize >> 24);
            }
            if (pcmDataDirect != null && pcmDataDirect.Count > 0)
            {
                foreach (byte[] dat in pcmDataDirect)
                {
                    if (dat != null && dat.Length > 0)
                    {
                        dat[ptr] = (byte)maxSize;
                        dat[ptr + 1] = (byte)(maxSize >> 8);
                        dat[ptr + 2] = (byte)(maxSize >> 16);
                        dat[ptr + 3] = (byte)(maxSize >> 24);
                    }
                }
            }

            if (pcmDataEasy != null && pcmDataEasy.Length > 15)
                parent.OutData(mml, null, pcmDataEasy);

            if (pcmDataDirect == null || pcmDataDirect.Count < 1) return;

            foreach (byte[] dat in pcmDataDirect)
            {
                if (dat != null && dat.Length > 0)
                    parent.OutData(mml, null, dat);
            }
        }



        public override void CmdY(partPage page, MML mml)
        {
            base.CmdY(page, mml);

            if (mml.args[0] is string) return;
            if (mml.args.Count < 2) return;

            byte adr = (byte)(int)mml.args[0];
            byte dat = (byte)(int)mml.args[1];

            if (page.Type == enmChannelType.FMOPN || page.Type == enmChannelType.FMOPNex)
                SOutData(page, mml, (page.ch > 2 && page.ch < 6) ? port[1] : port[0], adr, dat);
            else if (page.Type == enmChannelType.SSG)
                SOutData(page, mml, port[0], adr, dat);
            else if (page.Type == enmChannelType.RHYTHM)
                SOutData(page, mml, port[0], adr, dat);
            else if (page.Type == enmChannelType.ADPCM)
                SOutData(page, mml, port[1], adr, dat);
        }

        public override void CmdMPMS(partPage page, MML mml)
        {
            if (page.Type != enmChannelType.FMOPN)
            {
                msgBox.setWrnMsg(msg.get("E18000"), mml.line.Lp);
                return;
            }

            int n = (int)mml.args[1];
            n = Common.CheckRange(n, 0, 7);
            page.pms = n;
            ((ClsOPN)page.chip).OutOPNSetPanAMSPMS(
                mml,
                page
                , page.pan
                , page.ams
                , page.pms);
        }

        public override void CmdMAMS(partPage page, MML mml)
        {
            if (page.Type != enmChannelType.FMOPN)
            {
                msgBox.setWrnMsg(msg.get("E18001"), mml.line.Lp);
                return;
            }

            int n = (int)mml.args[1];
            n = Common.CheckRange(n, 0, 3);
            page.ams = n;
            ((ClsOPN)page.chip).OutOPNSetPanAMSPMS(
                mml,
                page
                , page.pan
                , page.ams
                , page.pms);
        }

        public override void CmdLfo(partPage page, MML mml)
        {
            base.CmdLfo(page, mml);

            if (mml.args[0] is string)
            {
                return;
            }

            int c = (char)mml.args[0] - 'P';
            if (page.lfo[c].type == eLfoType.Hardware)
            {
                if (page.Type == enmChannelType.FMOPN)
                {
                    if (page.lfo[c].param.Count < 4)
                    {
                        msgBox.setErrMsg(msg.get("E18002"), mml.line.Lp);
                        return;
                    }
                    if (page.lfo[c].param.Count > 5)
                    {
                        msgBox.setErrMsg(msg.get("E18003"), mml.line.Lp);
                        return;
                    }

                    page.lfo[c].param[0] = Common.CheckRange(page.lfo[c].param[0], 0, (int)parent.info.clockCount);//Delay(無視)
                    page.lfo[c].param[1] = Common.CheckRange(page.lfo[c].param[1], 0, 7);//Freq
                    page.lfo[c].param[2] = Common.CheckRange(page.lfo[c].param[2], 0, 7);//PMS
                    page.lfo[c].param[3] = Common.CheckRange(page.lfo[c].param[3], 0, 3);//AMS
                    if (page.lfo[c].param.Count == 5)
                    {
                        page.lfo[c].param[4] = Common.CheckRange(page.lfo[c].param[4], 0, 1); //Switch
                    }
                    else
                    {
                        page.lfo[c].param.Add(1);
                    }
                }
            }
        }

        public override void CmdLfoSwitch(partPage page, MML mml)
        {
            base.CmdLfoSwitch(page, mml);

            int c = (char)mml.args[0] - 'P';
            int n = (int)mml.args[1];
            if (page.lfo[c].type == eLfoType.Hardware)
            {
                if (page.Type == enmChannelType.FMOPN)
                {
                    if (page.lfo[c].param[4] == 0)
                    {
                        page.fms = (n == 0) ? 0 : page.lfo[c].param[2];
                        page.ams = (n == 0) ? 0 : page.lfo[c].param[3];
                        ((ClsOPN)page.chip).OutOPNSetPanAMSPMS(mml, page, (int)page.pan, page.ams, page.fms);
                        page.chip.lstPartWork[0].cpg.hardLfoSw = (n != 0);
                        page.chip.lstPartWork[0].cpg.hardLfoNum = page.lfo[c].param[1];
                        ((ClsOPN)page.chip).OutOPNSetHardLfo(null, page, page.chip.lstPartWork[0].cpg.hardLfoSw, page.chip.lstPartWork[0].cpg.hardLfoNum);
                    }
                    else
                    {
                        ((ClsOPN)page.chip).OutOPNSetHardLfo(mml, page, false, page.lfo[c].param[1]);
                    }
                }
            }
        }

        public override void CmdTotalVolume(partPage page, MML mml)
        {
            int n = (int)mml.args[0];
            ((YM2608)page.chip).rhythm_TotalVolume
                = Common.CheckRange(n, 0, ((YM2608)page.chip).rhythm_MAXTotalVolume);
        }

        public override void CmdPan(partPage page, MML mml)
        {
            int n = (int)mml.args[0];
            if (page.Type == enmChannelType.FMOPN || page.Type == enmChannelType.FMOPNex)
            {
                n = Common.CheckRange(n, 0, 3);
                page.pan = n;
                ((ClsOPN)page.chip).OutOPNSetPanAMSPMS(mml, page, n, page.ams, page.fms);
            }
            else if (page.Type == enmChannelType.RHYTHM)
            {
                n = Common.CheckRange(n, 0, 3);
                page.pan = n;
            }
            else if (page.Type == enmChannelType.ADPCM)
            {
                n = Common.CheckRange(n, 0, 3);
                page.pan = n;
                ((YM2608)page.chip).SetAdpcmPan(mml, page);
            }
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

            if (type == 'n' || type == 'N' || type == 'R' || type == 'A')
            {
                if (page.Type == enmChannelType.FMOPNex)
                {
                    if (re) n = page.instrument + n;
                    n = Common.CheckRange(n, 0, 255);

                    page.instrument = n;
                    lstPartWork[2].cpg.instrument = n;
                    lstPartWork[6].cpg.instrument = n;
                    lstPartWork[7].cpg.instrument = n;
                    lstPartWork[8].cpg.instrument = n;
                    OutFmSetInstrument(page, mml, n, page.volume, type);
                    return;
                }
            }

            if (type == 'n')
            {
                if (page.Type == enmChannelType.ADPCM)
                {
                    if (re) n = page.instrument + n;
                    n = Common.CheckRange(n, 0, 255);
                    if (!parent.instPCM.ContainsKey(n))
                    {
                        msgBox.setErrMsg(string.Format(msg.get("E18004"), n), mml.line.Lp);
                    }
                    else
                    {
                        if (parent.instPCM[n].Item2.chip != enmChipType.YM2608)
                        {
                            msgBox.setErrMsg(string.Format(msg.get("E18005"), n), mml.line.Lp);
                        }
                        page.instrument = n;
                        SetADPCMAddress(
                            mml,
                            page
                            , (int)parent.instPCM[n].Item2.stAdr
                            , (int)parent.instPCM[n].Item2.edAdr);
                    }
                    return;
                }

                if (page.Type == enmChannelType.SSG)
                {
                    if (!page.pcm)
                    {
                        SetEnvelopParamFromInstrument(page, n, re, mml);
                        return;
                    }

                    if (page.instrument == n) return;

                    if (!parent.instPCM.ContainsKey(n))
                    {
                        msgBox.setErrMsg(string.Format(msg.get("E11008"), n), mml.line.Lp);
                        return;
                    }

                    if (parent.instPCM[n].Item2.chip != enmChipType.YM2203
                        && parent.instPCM[n].Item2.chip != enmChipType.YM2608
                        && parent.instPCM[n].Item2.chip != enmChipType.YM2610B
                        && parent.instPCM[n].Item2.chip != enmChipType.YM2609)
                    {
                        msgBox.setErrMsg(string.Format(msg.get("E11009"), n, _Name), mml.line.Lp);
                    }

                    if (re) page.instrument += n;
                    else page.instrument = n;

                    return;
                }
            }

            base.CmdInstrument(page, mml);
        }

        public override void SetupPageData(partWork pw, partPage page)
        {

            if (page.Type == enmChannelType.FMOPN || page.Type == enmChannelType.FMOPNex)
            {

                OutFmKeyOff(page, null);
                page.spg.instrument = -1;
                OutFmSetInstrument(page, null, page.instrument, page.volume, 'n');

                //周波数
                page.spg.freq = -1;
                SetFNum(page, null);

                //音量
                page.spg.beforeVolume = -1;
                SetVolume(page, null);

                //パン
                page.spg.pan = page.pan;
                ((ClsOPN)page.chip).OutOPNSetPanAMSPMS(null, page, page.pan, page.ams, page.fms);
            }
            else if (page.Type == enmChannelType.SSG)
            {

                //周波数
                page.spg.freq = -1;
                SetFNum(page, null);

                //ノイズ周波数
                noiseFreq = -1;
                OutSsgNoise(null, page);

                //ハードエンベロープtype
                page.spg.HardEnvelopeType = -1;
                OutSsgHardEnvType(page, null);

                //ハードエンベロープspeed
                page.spg.HardEnvelopeSpeed = -1;
                OutSsgHardEnvSpeed(page, null);

                //音量
                page.spg.beforeVolume = -1;
                SetVolume(page, null);

            }
            else if (page.Type == enmChannelType.RHYTHM)
            {

                //音量
                page.spg.beforeVolume = -1;
                //パン
                page.spg.pan = -1;

            }
            else if (page.Type == enmChannelType.ADPCM)
            {
                //音色
                page.spg.instrument = page.instrument;
                page.spg.pcmEndAddress = -1;
                page.spg.pcmStartAddress = -1;

                SetADPCMAddress(null, page
                    , (int)parent.instPCM[page.instrument].Item2.stAdr
                    , (int)parent.instPCM[page.instrument].Item2.edAdr);

                //周波数
                page.spg.freq = -1;
                SetFNum(page, null);

                //音量
                page.spg.beforeVolume = -1;
                SetVolume(page, null);

                //パン
                page.spg.pan = -1;
                SetAdpcmPan(null, page);
            }

        }

        public override void MultiChannelCommand(MML mml)
        {
            if (!use) return;

            base.MultiChannelCommand(mml);

            //コマンドを跨ぐデータ向け処理
            foreach (partWork pw in lstPartWork)
            {
                partPage page = pw.cpg;

                if (page.Type == enmChannelType.RHYTHM)
                {
                    //Rhythm Volume処理
                    if (page.spg.beforeVolume != page.volume || page.spg.pan != page.pan)
                    {
                        SOutData(page, mml, port[0], (byte)(0x18 + (page.ch - 12)), (byte)((byte)((page.pan & 0x3) << 6) | (byte)(page.volume & 0x1f)));
                        page.spg.beforeVolume = page.volume;
                        page.spg.pan = page.pan;
                    }

                    rhythm_KeyOn |= (byte)(page.keyOn ? (1 << (page.ch - 12)) : 0);
                    page.keyOn = false;
                    rhythm_KeyOff |= (byte)(page.keyOff ? (1 << (page.ch - 12)) : 0);
                    page.keyOff = false;

                }
                else
                {
                    if (page.Type == enmChannelType.SSG)
                        continue;
                    if (page.keyOn)
                    {
                        page.keyOn = false;
                        OutKeyOn(page, mml);
                    }
                }
            }


            partPage pg = lstPartWork[17].cpg;

            //Rhythm KeyOff処理
            if (0 != rhythm_KeyOff)
            {
                byte data = (byte)(0x80 + rhythm_KeyOff);
                SOutData(pg, mml, port[0], 0x10, data);
                rhythm_KeyOff = 0;
            }

            //Rhythm TotalVolume処理
            if (rhythm_beforeTotalVolume != rhythm_TotalVolume)
            {
                SOutData(pg, mml, port[0], 0x11, (byte)(rhythm_TotalVolume & 0x3f));
                rhythm_beforeTotalVolume = rhythm_TotalVolume;
            }

            //Rhythm KeyOn処理
            if (0 != rhythm_KeyOn)
            {
                byte data = (byte)(0x00 + rhythm_KeyOn);
                SOutData(pg, mml, port[0], 0x10, data);
                rhythm_KeyOn = 0;
            }

        }

        public override string DispRegion(Tuple<string, clsPcm> pcm)
        {
            return string.Format("{0,-10} {1,-7} {2,-5:D3} N/A  ${3,-7:X6} ${4,-7:X6} N/A      ${5,-7:X6}  NONE {6}\r\n"
                , Name
                , pcm.Item2.chipNumber != 0 ? "SEC" : "PRI"
                , pcm.Item2.num
                , pcm.Item2.stAdr & 0xffffff
                , pcm.Item2.edAdr & 0xffffff
                , pcm.Item2.size
                , pcm.Item2.status.ToString()
                );
        }

        protected override void GetPortVch(partPage page, out byte[] port, out int vch)
        {
            port = (page.ch > 2 && page.ch < 6) ? page.port[1] : page.port[0];
            vch = (byte)((page.ch > 2 && page.ch < 6) ? page.ch - 3 : (page.ch < 3 ? page.ch : 2));
        }

    }
}