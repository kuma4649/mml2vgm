using musicDriverInterface;
using System;
using System.Collections.Generic;

namespace Core
{
    public class YM2610B : ClsOPN
    {
        protected int[][] _FNumTbl = new int[2][] {
            new int[13]
            ,new int[96]
            //new int[] {
            //// OPNB(FM) : TP = (144 * ftone * (2^20) / M) / (2^(B-1))       32:Divider 2:OPNB 
            ////   c    c+     d    d+     e     f    f+     g    g+     a    a+     b    >c
            // 0x269,0x28e,0x2b4,0x2de,0x309,0x337,0x368,0x39c,0x3d3,0x40e,0x44b,0x48d,0x4d2
            //},
            //new int[] {
            //// OPNB(SSG) : TP = M / (ftone * 32 * 2)       32:Divider 2:OPNB
            ////   c    c+     d    d+     e     f    f+     g    g+     a    a+     b
            // 0xEEE,0xE18,0xD4D,0xC8E,0xBDA,0xB30,0xA8F,0x9F7,0x968,0x8E1,0x861,0x7E9
            //,0x777,0x70C,0x6A7,0x647,0x5ED,0x598,0x547,0x4FC,0x4B4,0x470,0x431,0x3F4
            //,0x3BC,0x386,0x353,0x324,0x2F6,0x2CC,0x2A4,0x27E,0x25A,0x238,0x218,0x1FA
            //,0x1DE,0x1C3,0x1AA,0x192,0x17B,0x166,0x152,0x13F,0x12D,0x11C,0x10C,0x0FD
            //,0x0EF,0x0E1,0x0D5,0x0C9,0x0BE,0x0B3,0x0A9,0x09F,0x096,0x08E,0x086,0x07F
            //,0x077,0x071,0x06A,0x064,0x05F,0x059,0x054,0x050,0x04B,0x047,0x043,0x03F
            //,0x03C,0x038,0x035,0x032,0x02F,0x02D,0x02A,0x028,0x026,0x024,0x022,0x020
            //,0x01E,0x01C,0x01B,0x019,0x018,0x016,0x015,0x014,0x013,0x012,0x011,0x010
            //}
        };

        public byte[] pcmDataEasyA = null;
        public byte[] pcmDataEasyB = null;
        public byte[] pcmDataEasyC = null;
        public List<byte[]> pcmDataDirectA = new List<byte[]>();
        public List<byte[]> pcmDataDirectB = new List<byte[]>();

        public int adpcmA_TotalVolume = 63;
        public int adpcmA_beforeTotalVolume = -1;
        public int adpcmA_MAXTotalVolume = 63;
        public byte adpcmA_KeyOn = 0;
        public byte adpcmA_KeyOff = 0;

        public YM2610B(ClsVgm parent, int chipID, string initialPartName, string stPath, int chipNumber) : base(parent, chipID, initialPartName, stPath, chipNumber)
        {
            _chipType = enmChipType.YM2610B;
            _Name = "YM2610B";
            _ShortName = "OPNB";
            _ChMax = 19;
            _canUsePcm = true;
            _canUsePI = true;
            FNumTbl = _FNumTbl;
            ChipNumber = chipNumber;
            dataType = 0x82;
            Frequency = 8000000;
            port = new byte[][]{
                new byte[] { (byte)(chipNumber!=0 ? 0xa8 : 0x58) }
                , new byte[] { (byte)(chipNumber!=0 ? 0xa9 : 0x59) }
            };
            DataBankID = 0x0e;//TBD(固定値ではなく、恐らくデータごとに連番を振るのが良いと思われる。)

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

            Ch[12].Type = enmChannelType.ADPCMA;
            Ch[13].Type = enmChannelType.ADPCMA;
            Ch[14].Type = enmChannelType.ADPCMA;
            Ch[15].Type = enmChannelType.ADPCMA;
            Ch[16].Type = enmChannelType.ADPCMA;
            Ch[17].Type = enmChannelType.ADPCMA;

            Ch[18].Type = enmChannelType.ADPCMB;

            pcmDataInfo = new clsPcmDataInfo[3] { new clsPcmDataInfo(), new clsPcmDataInfo(), new clsPcmDataInfo() };
            pcmDataInfo[0].totalBufPtr = 0L;
            pcmDataInfo[0].use = false;
            pcmDataInfo[1].totalBufPtr = 0L;
            pcmDataInfo[1].use = false;
            pcmDataInfo[2].totalBufPtr = 0L;
            pcmDataInfo[2].use = false;

            if (parent.info.format == enmFormat.ZGM)
            {
                if (parent.ChipCommandSize == 2)
                {
                    if (chipNumber == 0)
                    {
                        pcmDataInfo[0].totalBuf = new byte[] { 0x07, 0x00, 0x66, 0x82, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                        pcmDataInfo[1].totalBuf = new byte[] { 0x07, 0x00, 0x66, 0x83, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                        pcmDataInfo[2].totalBuf = new byte[] { 0x07, 0x00, 0x66, DataBankID, 0x00, 0x00, 0x00, 0x00 };
                    }
                    else
                    {
                        pcmDataInfo[0].totalBuf = new byte[] { 0x07, 0x00, 0x66, 0x82, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                        pcmDataInfo[1].totalBuf = new byte[] { 0x07, 0x00, 0x66, 0x83, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                        pcmDataInfo[2].totalBuf = new byte[] { 0x07, 0x00, 0x66, DataBankID, 0x00, 0x00, 0x00, 0x00 };
                    }
                }
                else
                {
                    if (chipNumber == 0)
                    {
                        pcmDataInfo[0].totalBuf = new byte[] { 0x07, 0x66, 0x82, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                        pcmDataInfo[1].totalBuf = new byte[] { 0x07, 0x66, 0x83, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                        pcmDataInfo[2].totalBuf = new byte[] { 0x07, 0x66, DataBankID, 0x00, 0x00, 0x00, 0x00 };
                    }
                    else
                    {
                        pcmDataInfo[0].totalBuf = new byte[] { 0x07, 0x66, 0x82, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                        pcmDataInfo[1].totalBuf = new byte[] { 0x07, 0x66, 0x83, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                        pcmDataInfo[2].totalBuf = new byte[] { 0x07, 0x66, DataBankID, 0x00, 0x00, 0x00, 0x00 };
                    }
                }
            }
            else
            {
                if (chipNumber == 0)
                {
                    pcmDataInfo[0].totalBuf = new byte[] { 0x67, 0x66, 0x82, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                    pcmDataInfo[1].totalBuf = new byte[] { 0x67, 0x66, 0x83, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                    pcmDataInfo[2].totalBuf = new byte[] { 0x67, 0x66, DataBankID, 0x00, 0x00, 0x00, 0x00 };
                }
                else
                {
                    pcmDataInfo[0].totalBuf = new byte[] { 0x67, 0x66, 0x82, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                    pcmDataInfo[1].totalBuf = new byte[] { 0x67, 0x66, 0x83, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                    pcmDataInfo[2].totalBuf = new byte[] { 0x67, 0x66, DataBankID, 0x00, 0x00, 0x00, 0x00 };
                }
            }

            pcmDataInfo[0].totalHeaderLength = pcmDataInfo[0].totalBuf.Length;
            pcmDataInfo[0].totalHeadrSizeOfDataPtr = (parent.ChipCommandSize == 2) ? 4 : 3;
            pcmDataInfo[1].totalHeaderLength = pcmDataInfo[1].totalBuf.Length;
            pcmDataInfo[1].totalHeadrSizeOfDataPtr = (parent.ChipCommandSize == 2) ? 4 : 3;
            pcmDataInfo[2].totalHeaderLength = pcmDataInfo[2].totalBuf.Length;
            pcmDataInfo[2].totalHeadrSizeOfDataPtr = (parent.ChipCommandSize == 2) ? 4 : 3;


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
                for (int p = 0; p < lstPartWork[ch].pg.Count; p++)
                    lstPartWork[ch].pg[p].volume = 0;
                //setSsgVolume(ym2610b[i].lstPartWork[ch]);
                //ym2610b[i].lstPartWork[ch].volume = 15;
            }

            //ADPCM-A/B Reset
            parent.OutData((MML)null, lstPartWork[0].apg.port[0], 0x1c, 0xbf);
            parent.OutData((MML)null, lstPartWork[0].apg.port[0], 0x1c, 0x00);
            parent.OutData((MML)null, lstPartWork[0].apg.port[0], 0x10, 0x00);
            parent.OutData((MML)null, lstPartWork[0].apg.port[0], 0x11, 0xc0);

            foreach (partWork pw in lstPartWork)
            {
                OutOPNSetHardLfo(null, pw.cpg, pw.cpg.hardLfoSw, pw.cpg.hardLfoNum);
                if (!pw.cpg.dataEnd) OutOPNSetPanAMSPMS(null, pw.cpg, 3, 0, 0);

                foreach (partPage page in pw.pg)
                {
                    if (page.ch == 0)
                    {
                        page.hardLfoSw = false;
                        page.hardLfoNum = 0;
                    }

                    if (page.ch < 6)
                    {
                        page.pan = 3;
                        page.ams = 0;
                        page.fms = 0;
                    }
                }
            }

            if (parent.info.format != enmFormat.ZGM)
            {
                parent.dat[0x4f] = new outDatum(enmMMLType.unknown, null, null, (byte)(parent.dat[0x4f].val | 0x80));//YM2610B
                if (ChipNumber != 0)
                {
                    parent.dat[0x4f] = new outDatum(enmMMLType.unknown, null, null, (byte)(parent.dat[0x4f].val | 0x40));//use Secondary
                }
            }
        }

        public override void InitPart(partWork pw)
        {
            for (int p = 0; p < pw.pg.Count; p++)
            {
                pw.pg[p].slots = (byte)((pw.pg[p].Type == enmChannelType.FMOPN || pw.pg[p].ch == 2) ? 0xf : 0x0);
                pw.pg[p].volume = 127;
                pw.pg[p].MaxVolume = 127;
                if (pw.pg[p].Type == enmChannelType.SSG)
                {
                    //pw.ppg[pw.cpgNum].volume = 32767;
                    pw.pg[p].MaxVolume = 15;
                    pw.pg[p].volume = pw.pg[p].MaxVolume;
                }
                else if (pw.pg[p].Type == enmChannelType.ADPCMA)
                {
                    //pw.ppg[pw.cpgNum].volume = 32767;
                    pw.pg[p].MaxVolume = 31;//5bit
                    pw.pg[p].volume = pw.pg[p].MaxVolume;
                }
                else if (pw.pg[p].Type == enmChannelType.ADPCMB)
                {
                    //pw.ppg[pw.cpgNum].volume = 32767;
                    pw.pg[p].MaxVolume = 255;
                    pw.pg[p].volume = pw.pg[p].MaxVolume;
                }
                pw.pg[p].port = port;
                pw.pg[p].noise = 0;
                pw.pg[p].noiseFreq = -1;
            }
        }


        public void SetADPCMAAddress(MML mml, partPage page, int startAdr, int endAdr)
        {
            if (page.spg.pcmStartAddress != startAdr)
            {
                SOutData(page, mml, port[1], (byte)(0x10 + (page.ch - 12)), (byte)((startAdr >> 8) & 0xff));
                SOutData(page, mml, port[1], (byte)(0x18 + (page.ch - 12)), (byte)((startAdr >> 16) & 0xff));
                page.spg.pcmStartAddress = startAdr;
            }

            if (page.spg.pcmEndAddress != endAdr)
            {
                SOutData(page, mml, port[1], (byte)(0x20 + (page.ch - 12)), (byte)(((endAdr - 0x100) >> 8) & 0xff));
                SOutData(page, mml, port[1], (byte)(0x28 + (page.ch - 12)), (byte)(((endAdr - 0x100) >> 16) & 0xff));
                page.spg.pcmEndAddress = endAdr;
            }

        }

        public void SetADPCMBAddress(MML mml, partPage page, int startAdr, int endAdr)
        {
            if (page.spg.pcmStartAddress != startAdr)
            {
                SOutData(page, mml, port[0], 0x12, (byte)((startAdr >> 8) & 0xff));
                SOutData(page, mml, port[0], 0x13, (byte)((startAdr >> 16) & 0xff));
                page.spg.pcmStartAddress = startAdr;
            }

            if (page.spg.pcmEndAddress != endAdr)
            {
                SOutData(page, mml, port[0], 0x14, (byte)(((endAdr - 0x100) >> 8) & 0xff));
                SOutData(page, mml, port[0], 0x15, (byte)(((endAdr - 0x100) >> 16) & 0xff));
                page.spg.pcmEndAddress = endAdr;
            }

        }

        public void SetAdpcmBFNum(MML mml, partPage page)
        {
            int arpNote = page.arpFreqMode ? 0 : page.arpDelta;
            int arpFreq = page.arpFreqMode ? page.arpDelta : 0;

            int f = GetAdpcmBFNum(page.octaveNow, page.noteCmd, page.shift + page.keyShift + arpNote);//
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

            f = Common.CheckRange(f, 0, 0xffff);
            if (page.freq == f) return;

            page.freq = f;

            byte data = 0;

            data = (byte)(f & 0xff);
            SOutData(page, mml, port[0], 0x19, data);

            data = (byte)((f & 0xff00) >> 8);
            SOutData(page, mml, port[0], 0x1a, data);
        }

        public void SetAdpcmBVolume(MML mml, partPage page)
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

            if (page.beforeVolume != vol)
            {
                SOutData(page, mml, port[0], 0x1b, (byte)vol);
                page.beforeVolume = page.volume;
            }
        }

        public void SetAdpcmBPan(MML mml, partPage page)
        {
            if (page.spg.pan != page.pan)
            {
                SOutData(page, mml, port[0], 0x11, (byte)((page.pan & 0x3) << 6));
                page.spg.pan = page.pan;
            }
        }

        public int GetAdpcmBFNum(int octave, char noteCmd, int shift)
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

            return (int)(0x4a53 * Const.pcmMTbl[n] * Math.Pow(2, (o - 4)));
        }

        public void OutAdpcmBKeyOn(MML mml, partPage page)
        {

            SetAdpcmBVolume(mml, page);
            SOutData(page, mml, port[0], 0x10, 0x80);

        }

        public void OutAdpcmBKeyOff(MML mml, partPage page)
        {

            SOutData(page, mml, port[0], 0x10, 0x01);

        }


        public override void SetFNum(partPage page, MML mml)
        {
            if (page.ch < 9)
                SetFmFNum(page, mml);
            else if (page.Type == enmChannelType.SSG)
            {
                SetSsgFNum(page, mml);
            }
            else if (page.Type == enmChannelType.ADPCMA)
            {
            }
            else if (page.Type == enmChannelType.ADPCMB)
            {
                SetAdpcmBFNum(mml, page);
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
            //else if (page.Type == enmChannelType.ADPCMA)
            //{
            //    //ADPCM A はmultiのほうで処理する
            //    page.keyOff = false;
            //}
            else if (page.Type == enmChannelType.ADPCMB)
            {
                OutAdpcmBKeyOn(mml, page);
                if (page.instrument != -1 && parent.instPCM[page.instrument].Item2.status != enmPCMSTATUS.ERROR)
                {
                    parent.instPCM[page.instrument].Item2.status = enmPCMSTATUS.USED;
                }
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
            else if (page.Type == enmChannelType.ADPCMA)
            {
                page.keyOn = false;
                page.keyOff = true;
            }
            else if (page.Type == enmChannelType.ADPCMB)
            {
                OutAdpcmBKeyOff(mml, page);
            }
        }

        public override void SetVolume(partPage page, MML mml)
        {
            base.SetVolume(page, mml);

            if (page.Type == enmChannelType.ADPCMA)
            {
            }
            else if (page.Type == enmChannelType.ADPCMB)
            {
                SetAdpcmBVolume(mml, page);
            }

            if (mml != null)
            {
                MML vmml = new MML();
                vmml.type = enmMMLType.Volume;
                vmml.args = new List<object>();
                vmml.args.Add(page.volume);
                vmml.line = mml.line;
                SetDummyData(page, vmml);
            }
        }

        public override void SetPCMDataBlock(MML mml)
        {
            //if (!CanUsePcm) return;
            if (!use) return;

            SetPCMDataBlock_AB(mml, pcmDataEasyA, pcmDataDirectA);
            SetPCMDataBlock_AB(mml, pcmDataEasyB, pcmDataDirectB);
            SetPCMDataBlock_AB(mml, pcmDataEasyC, null);
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
            if (pcmDataDirect!=null && pcmDataDirect.Count > 0)
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
            if (pcmDataEasy != null && pcmDataEasy.Length > ptr + 3)
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
                pl.value = (pl.param[0] == 0) ? pl.param[6] : 0;//ディレイ中は振幅補正は適用されない
                pl.waitCounter = pl.param[0];
                pl.direction = pl.param[2] < 0 ? -1 : 1;
                pl.depthWaitCounter = pl.param[7];
                pl.depth = pl.param[3];
                pl.depthV2 = pl.param[2];

                if (pl.type == eLfoType.Vibrato)
                {
                    if (page.Type == enmChannelType.ADPCMB)
                        SetAdpcmBFNum(mml, page);

                }

                if (pl.type == eLfoType.Tremolo)
                {
                    page.beforeVolume = -1;

                    //if (pw.ppg[pw.cpgNum].Type == enmChannelType.ADPCMA)
                    //SetAdpcmAVolume(pw);
                    if (page.Type == enmChannelType.ADPCMB)
                        SetAdpcmBVolume(mml, page);

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
                        buf = ea.YM_ADPCM_A_Encode(buf, is16bit);
                        pi = pcmDataInfo[0];
                        break;
                    case 1:
                        buf = ea.YM_ADPCM_B_Encode(buf, is16bit, true);
                        pi = pcmDataInfo[1];
                        break;
                    case 2://SSGPCM
                        buf = SSGPCM_Encode(buf, is16bit);
                        pi = pcmDataInfo[2];
                        break;
                }

                long size = buf.Length;
                byte[] newBuf = new byte[size];
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
                        , v.Value.loopAdr == 0 ? 0 : 1
                        , is16bit
                        , samplerate)
                    ));

                pi.totalBufPtr += size;
                newBuf = new byte[pi.totalBuf.Length + buf.Length];
                Array.Copy(pi.totalBuf, newBuf, pi.totalBuf.Length);
                Array.Copy(buf, 0, newBuf, pi.totalBuf.Length, buf.Length);

                pi.totalBuf = newBuf;
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
                pcmDataEasyA = pi.use ? pcmDataInfo[0].totalBuf : null;
                pcmDataEasyB = pi.use ? pcmDataInfo[1].totalBuf : null;
                pcmDataEasyC = pi.use ? pcmDataInfo[2].totalBuf : null;
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
                buf = pds.DatLoopAdr == 0
                    ? ea.YM_ADPCM_A_Encode(buf, is16bit)
                    : ea.YM_ADPCM_B_Encode(buf, is16bit, true);
            }

            if (pds.DatLoopAdr == 0)
            {
                pcmDataDirectA.Add(Common.MakePCMDataBlock(dataType, pds, buf));
            }
            else
            {
                pcmDataDirectB.Add(Common.MakePCMDataBlock((byte)(dataType + 1), pds, buf));
            }

        }


        public override void CmdY(partPage page, MML mml)
        {
            base.CmdY(page, mml);

            if (mml.args[0] is string) return;

            byte adr = (byte)(int)mml.args[0];
            byte dat = (byte)(int)mml.args[1];

            if (page.Type == enmChannelType.FMOPN || page.Type == enmChannelType.FMOPNex)
                SOutData(page, mml, (page.ch > 2 && page.ch < 6) ? port[1] : port[0], adr, dat);
            else if (page.Type == enmChannelType.SSG)
                SOutData(page, mml, port[0], adr, dat);
            else if (page.Type == enmChannelType.ADPCMA)
                SOutData(page, mml, port[1], adr, dat);
            else if (page.Type == enmChannelType.ADPCMB)
                SOutData(page, mml, port[0], adr, dat);
        }

        public override void CmdMPMS(partPage page, MML mml)
        {
            if (page.Type != enmChannelType.FMOPN)
            {
                msgBox.setWrnMsg(msg.get("E19000"), mml.line.Lp);
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
                msgBox.setWrnMsg(msg.get("E19001"), mml.line.Lp);
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
                        msgBox.setErrMsg(msg.get("E19002"), mml.line.Lp);
                        return;
                    }
                    if (page.lfo[c].param.Count > 5)
                    {
                        msgBox.setErrMsg(msg.get("E19003"), mml.line.Lp);
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
                        ((ClsOPN)page.chip).OutOPNSetPanAMSPMS(mml, page, page.pan, page.ams, page.fms);
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
            ((YM2610B)page.chip).adpcmA_TotalVolume
                = Common.CheckRange(n, 0, ((YM2610B)page.chip).adpcmA_MAXTotalVolume);
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
            else if (page.Type == enmChannelType.ADPCMA)
            {
                n = Common.CheckRange(n, 0, 3);
                page.pan = n;
            }
            else if (page.Type == enmChannelType.ADPCMB)
            {
                n = Common.CheckRange(n, 0, 3);
                page.pan = n;
                ((YM2610B)page.chip).SetAdpcmBPan(mml, page);
            }
            SetDummyData(page, mml);
        }

        public override void CmdPcmMapSw(partPage page, MML mml)
        {
            bool sw = (bool)mml.args[0];
            if (page.Type == enmChannelType.ADPCMA)
            {
                page.isPcmMap = sw;
            }
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
                if (page.Type == enmChannelType.ADPCMA)
                {
                    if (page.isPcmMap)
                    {
                        if (re) n = page.pcmMapNo + n;
                        n = Common.CheckRange(n, 0, 255);
                        page.pcmMapNo = n;
                        if (!parent.instPCMMap.ContainsKey(n))
                        {
                            msgBox.setErrMsg(string.Format(msg.get("E10024"), n), mml.line.Lp);
                        }
                        return;
                    }

                    if (re) n = page.instrument + n;
                    n = Common.CheckRange(n, 0, 255);
                    if (!parent.instPCM.ContainsKey(n))
                    {
                        msgBox.setErrMsg(string.Format(msg.get("E19004"), n), mml.line.Lp);
                    }
                    else
                    {
                        if (parent.instPCM[n].Item2.chip != enmChipType.YM2610B
                            || parent.instPCM[n].Item2.loopAdr != 0)
                        {
                            msgBox.setErrMsg(string.Format(msg.get("E19005"), n), mml.line.Lp);
                        }
                        page.instrument = n;
                        SetADPCMAAddress(
                            mml,
                            page
                            , (int)parent.instPCM[n].Item2.stAdr
                            , (int)parent.instPCM[n].Item2.edAdr);

                    }
                    return;
                }

                if (page.Type == enmChannelType.ADPCMB)
                {
                    if (re) n = page.instrument + n;
                    n = Common.CheckRange(n, 0, 255);
                    if (!parent.instPCM.ContainsKey(n))
                    {
                        msgBox.setErrMsg(string.Format(msg.get("E19004"), n), mml.line.Lp);
                    }
                    else
                    {
                        if (parent.instPCM[n].Item2.chip != enmChipType.YM2610B
                            || parent.instPCM[n].Item2.loopAdr != 1)
                        {
                            msgBox.setErrMsg(string.Format(msg.get("E19005"), n), mml.line.Lp);
                        }
                        page.instrument = n;
                        SetADPCMBAddress(
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
            else if (page.Type == enmChannelType.ADPCMA)
            {
                //音色
                page.spg.instrument = page.instrument;
                page.spg.pcmEndAddress = -1;
                page.spg.pcmStartAddress = -1;

                SetADPCMAAddress(null, page
                    , (int)parent.instPCM[page.instrument].Item2.stAdr
                    , (int)parent.instPCM[page.instrument].Item2.edAdr);

                //音量
                page.spg.beforeVolume = -1;
                //パン
                page.spg.pan = -1;

            }
            else if (page.Type == enmChannelType.ADPCMB)
            {
                //音色
                page.spg.instrument = page.instrument;
                page.spg.pcmEndAddress = -1;
                page.spg.pcmStartAddress = -1;

                SetADPCMBAddress(null, page
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
                SetAdpcmBPan(null, page);
            }

        }

        public override void MultiChannelCommand(MML mml)
        {
            if (!use) return;

            base.MultiChannelCommand(mml);

            //コマンドを跨ぐデータ向け処理
            foreach (partWork pw in lstPartWork)
            {
                foreach (partPage page in pw.pg)
                {
                    if (page.Type == enmChannelType.ADPCMA)
                    {
                        //Adpcm-A TotalVolume処理
                        if (page.spg.beforeVolume != page.volume || page.spg.pan != page.pan)
                        {
                            SOutData(page, mml, port[1], (byte)(0x08 + (page.ch - 12)), (byte)((byte)((page.pan & 0x3) << 6) | (byte)(page.volume & 0x1f)));
                            page.spg.beforeVolume = page.volume;
                            page.spg.pan = page.pan;
                        }

                        if (page.isPcmMap && page.keyOn)
                        {
                            int n = Const.NOTE.IndexOf(page.noteCmd);
                            int arpNote = page.arpFreqMode ? 0 : page.arpDelta;
                            int f = page.octaveNow * 12 + n + page.shift + page.keyShift + arpNote;
                            if (parent.instPCMMap.ContainsKey(page.pcmMapNo))
                            {
                                if (parent.instPCMMap[page.pcmMapNo].ContainsKey(f))
                                {
                                    page.instrument = parent.instPCMMap[page.pcmMapNo][f];
                                    SetADPCMAAddress(
                                        mml,
                                        page
                                        , (int)parent.instPCM[page.instrument].Item2.stAdr
                                        , (int)parent.instPCM[page.instrument].Item2.edAdr);
                                }
                                else
                                {
                                    msgBox.setErrMsg(string.Format(msg.get("E10025"), page.octaveNow, page.noteCmd, page.shift + page.keyShift + arpNote), mml.line.Lp);
                                    return;
                                }
                            }
                            else
                            {
                                msgBox.setErrMsg(string.Format(msg.get("E10024"), page.pcmMapNo), mml.line.Lp);
                                return;
                            }

                            if (parent.instPCM[page.instrument].Item2.status != enmPCMSTATUS.ERROR)
                            {
                                parent.instPCM[page.instrument].Item2.status = enmPCMSTATUS.USED;
                            }
                        }

                        adpcmA_KeyOn |= (byte)(page.keyOn ? (1 << (page.ch - 12)) : 0);
                        page.keyOn = false;
                        adpcmA_KeyOff |= (byte)(page.keyOff ? (1 << (page.ch - 12)) : 0);
                        page.keyOff = false;


                    }
                    else
                    {
                        if (page.Type == enmChannelType.SSG) continue;
                        if (page.keyOn)
                        {
                            page.keyOn = false;
                            OutKeyOn(page, mml);
                        }
                    }
                }
            }

            partPage pg = lstPartWork[17].cpg;

            //Adpcm-A KeyOff処理
            if (0 != adpcmA_KeyOff)
            {
                byte data = (byte)(0x80 + adpcmA_KeyOff);
                SOutData(pg, mml, port[1], 0x00, data);
                adpcmA_KeyOff = 0;
            }

            //Adpcm-A TotalVolume処理
            if (adpcmA_beforeTotalVolume != adpcmA_TotalVolume)
            {
                SOutData(pg, mml, port[1], 0x01, (byte)(adpcmA_TotalVolume & 0x3f));
                adpcmA_beforeTotalVolume = adpcmA_TotalVolume;
            }

            //Adpcm-A KeyOn処理
            if (0 != adpcmA_KeyOn)
            {
                byte data = (byte)(0x00 + adpcmA_KeyOn);
                SOutData(pg, mml, port[1], 0x00, data);
                adpcmA_KeyOn = 0;
            }
        }


        public override string DispRegion(Tuple<string, clsPcm> pcm)
        {
            return string.Format("{0,-10} {1,-7} {2,-5:D3} N/A  ${3,-7:X6} ${4,-7:X6} N/A      ${5,-7:X6}  NONE {6}\r\n"
                , Name + (pcm.Item2.loopAdr == 0 ? "_A" : "_B")
                , pcm.Item2.chipNumber != 0 ? "SEC" : "PRI"
                , pcm.Item2.num
                , pcm.Item2.stAdr & 0xffffff
                , pcm.Item2.edAdr & 0xffffff
                , pcm.Item2.size
                , pcm.Item2.status.ToString()
                );
        }

    }
}