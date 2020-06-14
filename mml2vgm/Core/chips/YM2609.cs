using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using musicDriverInterface;

namespace Core
{
    public class YM2609 : ClsOPN
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

        public byte[] pcmDataEasyA = null;
        public byte[] pcmDataEasyB = null;
        public byte[] pcmDataEasyC = null;
        public byte[] pcmDataEasyD = null;
        public List<byte[]> pcmDataDirectA = new List<byte[]>();
        public List<byte[]> pcmDataDirectB = new List<byte[]>();
        public List<byte[]> pcmDataDirectC = new List<byte[]>();

        public int rhythm_TotalVolume = 63;
        public int rhythm_beforeTotalVolume = -1;
        public int rhythm_MAXTotalVolume = 63;
        public byte rhythm_KeyOn = 0;
        public byte rhythm_KeyOff = 0;

        public int adpcma_TotalVolume = 63;
        public int adpcma_beforeTotalVolume = -1;
        public int adpcma_MAXTotalVolume = 63;
        public byte adpcma_KeyOn = 0;
        public byte adpcma_KeyOff = 0;

        public YM2609(ClsVgm parent, int chipID, string initialPartName, string stPath, int chipNumber) : base(parent, chipID, initialPartName, stPath, chipNumber)
        {
            _chipType = enmChipType.YM2609;
            _Name = "YM2609";
            _ShortName = "OPNA2";
            _ChMax = 12 + 6 + 12 + 6 + 3 + 6;//FM:12ch FMex:6ch SSG:12ch Rhythm:6ch ADPCM:3ch ADPCM-A:6ch
            _canUsePcm = true;
            _canUsePI = true;
            FNumTbl = _FNumTbl;
            ChipNumber = chipNumber;
            dataType = 0x81;
            Frequency = 7987200;
            port = new byte[][]{
                new byte[] { 0x00 }
                , new byte[] { 0x01 }
                , new byte[] { 0x02 }
                , new byte[] { 0x03 }
            };

            if (string.IsNullOrEmpty(initialPartName)) return;

            Dictionary<string, List<double>> dic = MakeFNumTbl();
            if (dic != null && dic.Count > 0)
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
            }

            Ch = new ClsChannel[ChMax];
            SetPartToCh(Ch, initialPartName);
            foreach (ClsChannel ch in Ch)
            {
                ch.Type = enmChannelType.FMOPN;
                ch.chipNumber = chipID == 1;
            }

            Ch[2].Type = enmChannelType.FMOPNex;//ch:3
            Ch[8].Type = enmChannelType.FMOPNex;//ch:9
            for (int i = 0; i < 6; i++)
            {
                Ch[12 + i].Type = enmChannelType.FMOPNex;
                Ch[18 + i].Type = enmChannelType.SSG;
                Ch[24 + i].Type = enmChannelType.SSG;
                Ch[30 + i].Type = enmChannelType.RHYTHM;
                Ch[39 + i].Type = enmChannelType.ADPCM;
            }

            Ch[36].Type = enmChannelType.ADPCMA;
            Ch[37].Type = enmChannelType.ADPCMB;
            Ch[38].Type = enmChannelType.ADPCMB;

            pcmDataInfo = new clsPcmDataInfo[] {
                new clsPcmDataInfo(), new clsPcmDataInfo(),
                new clsPcmDataInfo(), new clsPcmDataInfo() };
            for (int i = 0; i < 4; i++)
            {
                pcmDataInfo[i].totalBufPtr = 0L;
                pcmDataInfo[i].use = false;
            }

            if (parent.info.format == enmFormat.ZGM)
            {
                if (parent.ChipCommandSize == 2)
                {
                    //                                     cmdL  cmdH  ccL   ccH   tt    size1 2     3     4
                    pcmDataInfo[0].totalBuf = new byte[] { 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                    pcmDataInfo[1].totalBuf = new byte[] { 0x07, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                    pcmDataInfo[2].totalBuf = new byte[] { 0x07, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                    pcmDataInfo[3].totalBuf = new byte[] { 0x07, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                }
                else
                {
                    //                                     cmd   cc    tt    size1 2     3     4
                    pcmDataInfo[0].totalBuf = new byte[] { 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                    pcmDataInfo[1].totalBuf = new byte[] { 0x07, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                    pcmDataInfo[2].totalBuf = new byte[] { 0x07, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                    pcmDataInfo[3].totalBuf = new byte[] { 0x07, 0x00, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                }
            }
            else
            {
                //ZGM以外は使用できない
                pcmDataInfo[0].totalBuf = null;
                pcmDataInfo[1].totalBuf = null;
                pcmDataInfo[2].totalBuf = null;
                pcmDataInfo[3].totalBuf = null;
            }

            pcmDataInfo[0].totalHeaderLength = pcmDataInfo[0].totalBuf.Length;
            pcmDataInfo[0].totalHeadrSizeOfDataPtr = (parent.ChipCommandSize == 2) ? 5 : 3;
            pcmDataInfo[1].totalHeaderLength = pcmDataInfo[1].totalBuf.Length;
            pcmDataInfo[1].totalHeadrSizeOfDataPtr = (parent.ChipCommandSize == 2) ? 5 : 3;
            pcmDataInfo[2].totalHeaderLength = pcmDataInfo[2].totalBuf.Length;
            pcmDataInfo[2].totalHeadrSizeOfDataPtr = (parent.ChipCommandSize == 2) ? 5 : 3;
            pcmDataInfo[3].totalHeaderLength = pcmDataInfo[3].totalBuf.Length;
            pcmDataInfo[3].totalHeadrSizeOfDataPtr = (parent.ChipCommandSize == 2) ? 5 : 3;

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
            for (int ch = 18; ch < 24; ch++)
            {
                OutSsgKeyOff(null, lstPartWork[ch].cpg);
                foreach (partPage page in lstPartWork[ch].pg)
                {
                    page.volume = 0;
                }
            }

            //Use OPNA mode
            parent.OutData((MML)null, port[0], 0x29, 0x82);
            //Use OPNA2 mode
            parent.OutData((MML)null, port[2], 0x29, 0x82);

            //SSG vol0
            parent.OutData((MML)null, port[0], 0x08, 0x00);
            parent.OutData((MML)null, port[0], 0x09, 0x00);
            parent.OutData((MML)null, port[0], 0x0a, 0x00);
            parent.OutData((MML)null, port[1], 0x28, 0x00);
            parent.OutData((MML)null, port[1], 0x29, 0x00);
            parent.OutData((MML)null, port[1], 0x2a, 0x00);
            parent.OutData((MML)null, port[2], 0x08, 0x00);
            parent.OutData((MML)null, port[2], 0x09, 0x00);
            parent.OutData((MML)null, port[2], 0x0a, 0x00);
            parent.OutData((MML)null, port[2], 0x18, 0x00);
            parent.OutData((MML)null, port[2], 0x19, 0x00);
            parent.OutData((MML)null, port[2], 0x1a, 0x00);

            //ADPCMA Reset
            parent.OutData((MML)null, port[1], 0x10, 0x17);
            parent.OutData((MML)null, port[1], 0x10, 0x80);
            parent.OutData((MML)null, port[1], 0x00, 0x80);
            parent.OutData((MML)null, port[1], 0x01, 0xc0);
            parent.OutData((MML)null, port[1], 0x00, 0x01);
            //ADPCMB Reset
            parent.OutData((MML)null, port[3], 0x10, 0x17);
            parent.OutData((MML)null, port[3], 0x10, 0x80);
            parent.OutData((MML)null, port[3], 0x00, 0x80);
            parent.OutData((MML)null, port[3], 0x01, 0xc0);
            parent.OutData((MML)null, port[3], 0x00, 0x01);
            //ADPCMC Reset
            parent.OutData((MML)null, port[3], 0x21, 0x17);
            parent.OutData((MML)null, port[3], 0x21, 0x80);
            parent.OutData((MML)null, port[3], 0x11, 0x80);
            parent.OutData((MML)null, port[3], 0x12, 0xc0);
            parent.OutData((MML)null, port[3], 0x11, 0x01);

            foreach (partWork pw in lstPartWork)
            {
                foreach (partPage page in pw.pg)
                {
                    if (page.ch == 0 || page.ch == 5)
                    {
                        page.hardLfoSw = false;
                        page.hardLfoNum = 0;
                        OutOPNSetHardLfo(null, page, page.hardLfoSw, page.hardLfoNum);
                    }

                    if (page.ch < 12)
                    {
                        page.pan.val = 3;
                        page.ams = 0;
                        page.fms = 0;
                        if (!page.dataEnd) OutOPNSetPanAMSPMS(null, page, 3, 0, 0);
                    }
                }
            }

        }

        public override void InitPart(partWork pw)
        {
            foreach (partPage page in pw.pg)
            {
                page.slots = (byte)((
                    page.Type == enmChannelType.FMOPN
                    || page.ch == 2
                    || page.ch == 8) ? 0xf : 0x0);

                page.volume = 127;
                page.MaxVolume = 127;
                if (page.Type == enmChannelType.SSG)
                {
                    page.MaxVolume = 15;
                    page.volume = 0;
                }
                else if (page.Type == enmChannelType.RHYTHM)
                {
                    page.MaxVolume = 31;//5bit
                    page.volume = page.MaxVolume;
                }
                else if (page.Type == enmChannelType.ADPCMA
                    || page.Type == enmChannelType.ADPCMB)
                {
                    page.MaxVolume = 255;
                    page.volume = page.MaxVolume;
                }
                else if (page.Type == enmChannelType.ADPCM)
                {
                    page.MaxVolume = 31;//5bit
                    page.volume = page.MaxVolume;
                }

                page.port = port;
            }
        }


        public void SetADPCMAddress(MML mml, partPage page, int startAdr, int endAdr)
        {
            int p = page.ch == 36 ? 1 : 3;
            int v = page.ch != 38 ? 0x00 : 0x11;

            if (page.pcmStartAddress != startAdr)
            {
                parent.OutData(mml, port[p], (byte)(0x02 + v), (byte)((startAdr >> (2 + (p == 1 ? 0 : 3))) & 0xff));
                parent.OutData(mml, port[p], (byte)(0x03 + v), (byte)((startAdr >> (10 + (p == 1 ? 0 : 3))) & 0xff));
                page.pcmStartAddress = startAdr;
            }

            if (page.pcmEndAddress != endAdr)
            {
                parent.OutData(mml, port[p], (byte)(0x04 + v), (byte)(((endAdr - 0x04) >> (2 + (p == 1 ? 0 : 3))) & 0xff));
                parent.OutData(mml, port[p], (byte)(0x05 + v), (byte)(((endAdr - 0x04) >> (10 + (p == 1 ? 0 : 3))) & 0xff));
                page.pcmEndAddress = endAdr;
            }
        }

        public void SetADPCMAAddress(MML mml, partPage page, int startAdr, int endAdr)
        {

            parent.OutData(mml, port[1], 0x13, (byte)(page.ch - 39));

            if (page.pcmStartAddress != startAdr)
            {
                parent.OutData(mml, port[1], 0x16, (byte)((startAdr >> 8) & 0xff));
                parent.OutData(mml, port[1], 0x16, (byte)((startAdr >> 16) & 0xff));
                page.pcmStartAddress = startAdr;
            }

            if (page.pcmEndAddress != endAdr)
            {
                parent.OutData(mml, port[1], 0x17, (byte)(((endAdr - 0x100) >> 8) & 0xff));
                parent.OutData(mml, port[1], 0x17, (byte)(((endAdr - 0x100) >> 16) & 0xff));
                page.pcmEndAddress = endAdr;
            }

        }

        public void SetAdpcmFNum(MML mml, partPage page)
        {
            int f = GetAdpcmFNum(page, page.octaveNow, page.noteCmd, page.shift + page.keyShift);//
            if (page.bendWaitCounter != -1)
            {
                f = page.bendFnum;
            }
            f = f + page.detune;
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
            int p = page.ch == 36 ? 1 : 3;
            int v = page.ch != 38 ? 0x00 : 0x11;

            data = (byte)(f & 0xff);
            parent.OutData(mml, port[p], (byte)(0x09 + v), data);

            data = (byte)((f & 0xff00) >> 8);
            parent.OutData(mml, port[p], (byte)(0x0a + v), data);
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

            if (page.beforeVolume != vol)
            {
                int p = page.ch == 36 ? 1 : 3;
                int v = page.ch != 38 ? 0x00 : 0x11;
                parent.OutData(mml, port[p], (byte)(0x0b + v), (byte)vol);
                page.beforeVolume = page.volume;
            }
        }

        public void SetAdpcmPan(MML mml, partPage page, int panL, int panR)
        {
            panL = Common.CheckRange(panL, 0, 4);
            panR = Common.CheckRange(panR, 0, 4);

            if (page.panL != panL || page.panR != panR)
            {
                int port = page.ch == 36 ? 1 : 3;
                int adr = page.ch != 38 ? 0x00 : 0x11;

                int v = (panL != 0 ? 0x80 : 00) | (panR != 0 ? 0x40 : 00);
                parent.OutData(mml, base.port[port], (byte)(0x01 + adr), (byte)v);

                v = (((4 - panL) & 0x3) << 6) | (((4 - panR) & 0x3) << 4);
                parent.OutData(mml, base.port[port], (byte)(0x07 + adr), (byte)v);

                page.panL = panL;
                page.panR = panR;
            }
        }

        public int GetAdpcmFNum(partPage page, int octave, char noteCmd, int shift)
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
                freq = (double)parent.instPCM[page.instrument].samplerate;
                if (parent.instPCM[page.instrument].freq != -1)
                {
                    freq = (double)parent.instPCM[page.instrument].freq;
                }
            }

            return (int)(
                0x49ba * Const.pcmMTbl[n] * Math.Pow(2, (o - 4)) * (freq / 8000.0)
                );
        }

        public void OutAdpcmKeyOn(MML mml, partPage page)
        {

            SetAdpcmVolume(mml, page);
            int p = page.ch == 36 ? 1 : 3;
            int v = page.ch != 38 ? 0x00 : 0x11;
            parent.OutData(mml, port[p], (byte)(0x00 + v), 0xa0);

        }

        public void OutAdpcmKeyOff(MML mml, partPage page)
        {
            int p = page.ch == 36 ? 1 : 3;
            int v = page.ch != 38 ? 0x00 : 0x11;

            parent.OutData(mml, port[p], (byte)(0x00 + v), 0x01);

        }


        public override void SetFNum(partPage page, MML mml)
        {
            if (page.ch < 18)
            {
                SetFmFNum(page, mml);
            }
            else if (page.Type == enmChannelType.SSG)
            {
                SetSsgFNum(page, mml);
            }
            else if (page.Type == enmChannelType.RHYTHM)
            {
            }
            else if (page.Type == enmChannelType.ADPCMA || page.Type == enmChannelType.ADPCMB)
            {
                SetAdpcmFNum(mml, page);
            }
        }

        public new void SetFmFNum(partPage page, MML mml)
        {
            if (page.noteCmd == (char)0)
                return;

            int[] ftbl = page.chip.FNumTbl[0];

            int f = GetFmFNum(ftbl, page.octaveNow, page.noteCmd, page.shift + page.keyShift + page.toneDoublerKeyShift);//
            if (page.bendWaitCounter != -1)
            {
                f = page.bendFnum;
            }
            int o = (f & 0xf000) / 0x1000;
            f &= 0xfff;

            f = f + page.detune;
            for (int lfo = 0; lfo < 4; lfo++)
            {
                if (!page.lfo[lfo].sw || page.lfo[lfo].type != eLfoType.Vibrato)
                    continue;

                f += page.lfo[lfo].value + page.lfo[lfo].param[6];
            }

            while (f < ftbl[0])
            {
                if (o == 1)
                    break;
                o--;
                f = ftbl[0] * 2 - (ftbl[0] - f);
            }

            while (f >= ftbl[0] * 2)
            {
                if (o == 8)
                    break;
                o++;
                f = f - ftbl[0] * 2 + ftbl[0];
            }

            f = Common.CheckRange(f, 0, 0x7ff);
            //OutFmSetFnum(pw, mml, o, f);
            page.freq = Common.CheckRange(f, 0, 0x7ff) | (Common.CheckRange(o - 1, 0, 7) << (8 + 3));

        }

        public void OutFmSetPanLFnum(partPage page, MML mml)
        {
            page.oldFreq = page.freq;
            page.beforePanL = page.panL;

            int portEx = -1;
            if ((page.ch == 2 || page.ch == 12 || page.ch == 13 || page.ch == 14) && page.chip.lstPartWork[2].cpg.Ch3SpecialMode)
                portEx = 0;
            else if ((page.ch == 8 || page.ch == 15 || page.ch == 16 || page.ch == 17) && page.chip.lstPartWork[8].cpg.Ch3SpecialMode)
                portEx = 2;

            if (portEx != -1)
            {
                if ((page.slots & 8) != 0 && page.freq != -1)
                {
                    int f = page.freq + page.slotDetune[3];
                    parent.OutData(mml, page.port[portEx], (byte)0xa6, (byte)((f & 0x3f00) >> 8));
                    parent.OutData(mml, page.port[portEx], (byte)0xa2, (byte)(f & 0xff));
                }
                if ((page.slots & 4) != 0 && page.freq != -1)
                {
                    int f = page.freq + page.slotDetune[2];
                    parent.OutData(mml, page.port[portEx], (byte)0xac, (byte)((f & 0x3f00) >> 8));
                    parent.OutData(mml, page.port[portEx], (byte)0xa8, (byte)(f & 0xff));
                }
                if ((page.slots & 1) != 0 && page.freq != -1)
                {
                    int f = page.freq + page.slotDetune[0];
                    parent.OutData(mml, page.port[portEx], (byte)0xad, (byte)((f & 0x3f00) >> 8));
                    parent.OutData(mml, page.port[portEx], (byte)0xa9, (byte)(f & 0xff));
                }
                if ((page.slots & 2) != 0 && page.freq != -1)
                {
                    int f = page.freq + page.slotDetune[1];
                    parent.OutData(mml, page.port[portEx], (byte)0xae, (byte)((f & 0x3f00) >> 8));
                    parent.OutData(mml, page.port[portEx], (byte)0xaa, (byte)(f & 0xff));
                }
            }
            else
            {
                if (page.ch >= 12 && page.ch < 18)
                {
                    return;
                }

                int vch;
                byte[] port;
                GetPortVch(page, out port, out vch);

                if (page.freq != -1)
                {
                    parent.OutData(mml, port, (byte)(0xa4 + vch), (byte)(((page.freq & 0x3f00) >> 8) | (((4 - page.panL) & 0x3) << 6)));
                    parent.OutData(mml, port, (byte)(0xa0 + vch), (byte)(page.freq & 0xff));
                }
            }
        }

        public void OutFmSetPanRFeedbackAlgorithm(MML mml, partPage page)
        {
            int vch;
            byte[] port;
            GetPortVch(page, out port, out vch);

            page.feedBack &= 7;
            page.algo &= 7;
            page.beforeFeedBack = page.feedBack;
            page.beforeAlgo = page.algo;
            page.beforePanR = page.panR;

            parent.OutData(mml, port, (byte)(0xb0 + vch), (byte)((((4 - page.panR) & 0x3) << 6) | (page.feedBack << 3) | page.algo));
        }

        public void OutOPNSetPanAmsAcPms(MML mml, partPage page)
        {
            //TODO: 効果音パートで指定されている場合の考慮不足
            int vch;
            byte[] port;
            GetPortVch(page, out port, out vch);

            page.beforePan.val = page.pan.val;
            page.beforeAms = page.ams;
            page.beforeAlgConstSw = page.algConstSw;
            page.beforePms = page.pms;

            parent.OutData(mml, port, (byte)(0xb4 + vch), (byte)((page.pan.val << 6) + (page.ams << 3) + (page.algConstSw << 2) + page.pms));
        }

        public new void SetSsgFNum(partPage page, MML mml)
        {
            int f = -page.detune;
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
                f -= page.lfo[lfo].value + page.lfo[lfo].param[6];
            }

            if (page.octaveNow < 1)
            {
                f <<= -page.octaveNow;
            }
            else
            {
                f >>= page.octaveNow - 1;
            }

            if (page.bendWaitCounter != -1)
            {
                f += page.bendFnum;
            }
            else
            {
                f += GetSsgFNum(page, mml, page.octaveNow, page.noteCmd, page.shift + page.keyShift);//
            }

            f = Common.CheckRange(f, 0, 0xfff);
            //if (pw.ppg[pw.cpgNum].freq == f && pw.ppg[pw.cpgNum].dutyCycle == pw.ppg[pw.cpgNum].oldDutyCycle) return;

            page.freq = f;
            //pw.ppg[pw.cpgNum].oldDutyCycle = pw.ppg[pw.cpgNum].dutyCycle;

        }

        public override void SetKeyOn(partPage page, MML mml)
        {
            if (page.ch < 18)
                OutFmKeyOn(page, mml);
            else if (page.Type == enmChannelType.SSG)
            {
                OutSsgKeyOn(page, mml);
            }
            else if (page.Type == enmChannelType.RHYTHM)
            {
                page.keyOn = true;
                page.keyOff = false;
                SetDummyData(page, mml);
            }
            else if (page.Type == enmChannelType.ADPCM)
            {
                page.keyOn = true;
                page.keyOff = false;
                SetDummyData(page, mml);
            }
            else if (page.Type == enmChannelType.ADPCMA || page.Type == enmChannelType.ADPCMB)
            {
                OutAdpcmKeyOn(mml, page);
            }
        }

        public override void SetKeyOff(partPage page, MML mml)
        {
            if (page.ch < 18)
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
                page.keyOn = false;
                page.keyOff = true;
            }
            else if (page.Type == enmChannelType.ADPCMA || page.Type == enmChannelType.ADPCMB)
            {
                OutAdpcmKeyOff(mml, page);
            }
        }

        public override void SetVolume(partPage page, MML mml)
        {
            if (page.Type == enmChannelType.FMOPN
                || page.Type == enmChannelType.FMOPNex //効果音モード対応チャンネル
                || (page.Type == enmChannelType.FMPCM && !page.pcm) //OPN2PCMチャンネル
                || (page.Type == enmChannelType.FMPCMex && !page.pcm) //OPN2XPCMチャンネル
                )
            {
                SetFmVolumeM(page, mml);
            }
            else if (page.Type == enmChannelType.SSG)
            {
                SetSsgVolume(page, mml);
            }
            else if (page.Type == enmChannelType.RHYTHM)
            {
            }
            else if (page.Type == enmChannelType.ADPCMA || page.Type == enmChannelType.ADPCMB)
            {
                SetAdpcmVolume(mml, page);
            }
        }

        public void SetFmVolumeM(partPage page, MML mml)
        {
            int vol = page.volume;

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

            if (page.beforeVolume != vol)
            {
                if (parent.instFM.ContainsKey(page.instrument))
                {
                    OutFmSetVolumeM(page, mml, vol, page.instrument);
                    page.beforeVolume = vol;
                }
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
                    if (page.Type == enmChannelType.ADPCMA || page.Type == enmChannelType.ADPCMB)
                        SetAdpcmFNum(mml, page);

                }

                if (pl.type == eLfoType.Tremolo)
                {
                    page.beforeVolume = -1;

                    //if (pw.ppg[pw.cpgNum].Type == enmChannelType.RHYTHM)
                    //SetRhythmVolume(pw);
                    if (page.Type == enmChannelType.ADPCMA || page.Type == enmChannelType.ADPCMB)
                        SetAdpcmVolume(mml, page);

                }

            }
        }

        public override void StorePcm(Dictionary<int, clsPcm> newDic, KeyValuePair<int, clsPcm> v, byte[] buf, bool is16bit, int samplerate, params object[] option)
        {
            int aCh = (int)v.Value.loopAdr;
            aCh = Common.CheckRange(aCh, 0, 3);
            clsPcmDataInfo pi = pcmDataInfo[aCh];

            try
            {
                EncAdpcmA ea = new EncAdpcmA();
                switch (v.Value.loopAdr)
                {
                    case 0:
                        buf = ea.YM_ADPCM_B_Encode(buf, is16bit, false);
                        break;
                    case 1:
                    case 2:
                        buf = ea.YM_ADPCM_B_Encode(buf, is16bit, true);
                        break;
                    case 3:
                        buf = ea.YM_ADPCM_A_Encode(buf, is16bit);
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
                    , new clsPcm(
                        v.Value.num
                        , v.Value.seqNum, v.Value.chip
                        , v.Value.chipNumber
                        , v.Value.fileName
                        , v.Value.freq
                        , v.Value.vol
                        , pi.totalBufPtr
                        , pi.totalBufPtr + size - 1
                        , size
                        , aCh
                        , is16bit
                        , samplerate)
                    );

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
                    , false
                    );
                Common.SetUInt32bit31(
                    pi.totalBuf
                    , pi.totalHeadrSizeOfDataPtr + 4
                    , (UInt32)(pi.totalBuf.Length - (pi.totalHeadrSizeOfDataPtr + 4 + 4 + 4))
                    );
                pcmDataEasyA = pcmDataInfo[0].totalBuf;
                pcmDataEasyB = pcmDataInfo[1].totalBuf;
                pcmDataEasyC = pcmDataInfo[2].totalBuf;
                pcmDataEasyD = pcmDataInfo[3].totalBuf;

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
                switch (pds.DatLoopAdr)
                {
                    case 0:
                        buf = ea.YM_ADPCM_B_Encode(buf, is16bit, false);
                        base.pcmDataDirect.Add(Common.MakePCMDataBlock((byte)dataType, pds, buf));
                        break;
                    case 1:
                    case 2:
                        buf = ea.YM_ADPCM_B_Encode(buf, is16bit, true);
                        base.pcmDataDirect.Add(Common.MakePCMDataBlock((byte)dataType, pds, buf));
                        break;
                    case 3:
                        buf = ea.YM_ADPCM_A_Encode(buf, is16bit);
                        pcmDataDirectA.Add(Common.MakePCMDataBlock((byte)dataType, pds, buf));
                        break;
                }
            }


        }

        public override void SetPCMDataBlock(MML mml)
        {
            //if (!CanUsePcm) return;
            if (!use) return;

            SetPCMDataBlock_AB(mml, pcmDataEasyA, pcmDataDirectA);
            SetPCMDataBlock_AB(mml, pcmDataEasyB, pcmDataDirectB);
            SetPCMDataBlock_AB(mml, pcmDataEasyC, pcmDataDirectC);
            SetPCMDataBlock_AB(mml, pcmDataEasyD, pcmDataDirect);
        }

        private void SetPCMDataBlock_AB(MML mml, byte[] pcmDataEasy, List<byte[]> pcmDataDirect)
        {
            int sizePtr = 7 + ((parent.ChipCommandSize != 2) ? 0 : 2);
            int maxSize = 0;
            if (pcmDataEasy != null && pcmDataEasy.Length > 0)
            {
                pcmDataEasy[1] = (byte)ChipID;
                if (parent.ChipCommandSize == 2) pcmDataEasy[2] = (byte)(ChipID >> 8);

                maxSize =
                    pcmDataEasy[sizePtr + 0]
                    + (pcmDataEasy[sizePtr + 1] << 8)
                    + (pcmDataEasy[sizePtr + 2] << 16)
                    + (pcmDataEasy[sizePtr + 3] << 24);
            }
            if (pcmDataDirect.Count > 0)
            {
                foreach (byte[] dat in pcmDataDirect)
                {
                    if (dat != null && dat.Length > 0)
                    {
                        dat[1] = (byte)ChipID;
                        if (parent.ChipCommandSize == 2) dat[2] = (byte)(ChipID >> 8);
                        int size =
                            dat[sizePtr + 0]
                            + (dat[sizePtr + 1] << 8)
                            + (dat[sizePtr + 2] << 16)
                            + (dat[sizePtr + 3] << 24);
                        if (maxSize < size) maxSize = size;
                    }
                }
            }
            if (pcmDataEasy != null && pcmDataEasy.Length > 0)
            {
                pcmDataEasy[sizePtr + 0] = (byte)maxSize;
                pcmDataEasy[sizePtr + 1] = (byte)(maxSize >> 8);
                pcmDataEasy[sizePtr + 2] = (byte)(maxSize >> 16);
                pcmDataEasy[sizePtr + 3] = (byte)(maxSize >> 24);
            }
            if (pcmDataDirect.Count > 0)
            {
                foreach (byte[] dat in pcmDataDirect)
                {
                    if (dat != null && dat.Length > 0)
                    {
                        dat[sizePtr + 0] = (byte)maxSize;
                        dat[sizePtr + 1] = (byte)(maxSize >> 8);
                        dat[sizePtr + 2] = (byte)(maxSize >> 16);
                        dat[sizePtr + 3] = (byte)(maxSize >> 24);
                    }
                }
            }

            if (pcmDataEasy != null && pcmDataEasy.Length > 15)
                parent.OutData(mml, null, pcmDataEasy);

            if (pcmDataDirect.Count < 1) return;

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

            if (mml.args.Count == 2)
            {
                byte adr = (byte)(int)mml.args[0];
                byte dat = (byte)(int)mml.args[1];
                int p = 0;

                if (page.ch < 3)//FM1-3
                    p = 0;
                else if (page.ch < 6)//FM4-6
                    p = 1;
                else if (page.ch < 9)//FM7-9
                    p = 2;
                else if (page.ch < 12)//FM10-12
                    p = 3;
                else if (page.ch < 15)//FMex13-15
                    p = 0;
                else if (page.ch < 18)//FMex15-18
                    p = 2;
                else if (page.ch < 21)//SSG1-3
                    p = 0;
                else if (page.ch < 24)//SSG4-6
                    p = 1;
                else if (page.ch < 30)//SSG7-12
                    p = 2;
                else if (page.ch < 36)//Rhythm1-6
                    p = 0;
                else if (page.ch == 36)//ADPCM1
                    p = 1;
                else if (page.ch == 37)//ADPCM2
                    p = 3;
                else if (page.ch == 38)//ADPCM3
                    p = 3;

                parent.OutData(mml, port[p], adr, dat);
            }
            else
            {
                byte prt = (byte)(int)mml.args[0];
                byte adr = (byte)(int)mml.args[1];
                byte dat = (byte)(int)mml.args[2];

                parent.OutData(mml, port[prt & 3], adr, dat);

            }
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
                , (int)page.pan.val
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
                , (int)page.pan.val
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
                        ((ClsOPN)page.chip).OutOPNSetPanAMSPMS(mml, page, (int)page.pan.val, page.ams, page.fms);
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
            ((YM2609)page.chip).rhythm_TotalVolume
                = Common.CheckRange(n, 0, ((YM2609)page.chip).rhythm_MAXTotalVolume);
        }

        public override void CmdPan(partPage page, MML mml)
        {
            int n = (int)mml.args[0];
            if (page.Type == enmChannelType.FMOPN || page.Type == enmChannelType.FMOPNex)
            {
                page.panL = Common.CheckRange(n, 0, 4);
                n = mml.args.Count < 2 ? 0 : (int)mml.args[1];
                page.panR = Common.CheckRange(n, 0, 4);
                page.pan.val = (page.panL != 0 ? 2 : 0) | (page.panR != 0 ? 1 : 0);

                //pw.ppg[pw.cpgNum].pan.val = n;
                //((ClsOPN)pw.ppg[pw.cpgNum].chip).OutOPNSetPanAMSPMS(mml, pw, n, pw.ppg[pw.cpgNum].ams, pw.ppg[pw.cpgNum].fms);
            }
            else if (page.Type == enmChannelType.RHYTHM)
            {
                n = Common.CheckRange(n, 0, 3);
                page.pan.val = n;
            }
            else if (page.Type == enmChannelType.ADPCMA || page.Type == enmChannelType.ADPCMB)
            {
                int pl = Common.CheckRange(n, 0, 4);
                n = mml.args.Count < 2 ? 0 : (int)mml.args[1];
                int pr = Common.CheckRange(n, 0, 4);
                ((YM2609)page.chip).SetAdpcmPan(mml, page, pl, pr);
            }
            else if (page.Type == enmChannelType.ADPCM)
            {
                page.panL = Common.CheckRange(n, 0, 4);
                n = mml.args.Count < 2 ? 0 : (int)mml.args[1];
                page.panR = Common.CheckRange(n, 0, 4);

                page.pan.val = (page.panL != 0 ? 2 : 0) | (page.panR != 0 ? 1 : 0);

                page.panL = page.panL < 1 ? 0 : (4 - page.panL);
                page.panR = page.panR < 1 ? 0 : (4 - page.panR);
            }
            else if (page.Type == enmChannelType.SSG)
            {
                n = Common.CheckRange(n, 0, 3);
                page.pan.val = n;
            }
            SetDummyData(page, mml);
        }

        public override void CmdInstrument(partPage page, MML mml)
        {
            char type = (char)mml.args[0];
            int n = (type == 'W') ? -1 : (int)mml.args[1];

            if (type == 'n' || type == 'N' || type == 'R' || type == 'A')
            {
                if (page.Type == enmChannelType.FMOPNex)
                {
                    page.instrument = n;
                    if (page.ch == 2 || page.ch == 12 || page.ch == 13 || page.ch == 14)
                    {
                        lstPartWork[2].cpg.instrument = n;
                        lstPartWork[12].cpg.instrument = n;
                        lstPartWork[13].cpg.instrument = n;
                        lstPartWork[14].cpg.instrument = n;
                    }
                    else
                    {
                        lstPartWork[8].cpg.instrument = n;
                        lstPartWork[15].cpg.instrument = n;
                        lstPartWork[16].cpg.instrument = n;
                        lstPartWork[17].cpg.instrument = n;
                    }
                    OutFmSetInstrument(page, mml, n, page.volume, type);
                    return;
                }
            }

            if (type == 'n')
            {
                if (page.Type == enmChannelType.ADPCMA || page.Type == enmChannelType.ADPCMB || page.Type == enmChannelType.ADPCM)
                {
                    n = Common.CheckRange(n, 0, 255);
                    if (!parent.instPCM.ContainsKey(n))
                    {
                        msgBox.setErrMsg(string.Format(msg.get("E18004"), n), mml.line.Lp);
                    }
                    else
                    {
                        if (parent.instPCM[n].chip != enmChipType.YM2609)
                        {
                            msgBox.setErrMsg(string.Format(msg.get("E18005"), n), mml.line.Lp);
                        }
                        page.instrument = n;
                        if (page.Type != enmChannelType.ADPCM)
                        {
                            SetADPCMAddress(
                                mml,
                                page
                                , (int)parent.instPCM[n].stAdr
                                , (int)parent.instPCM[n].edAdr);
                        }
                        else
                        {
                            SetADPCMAAddress(
                                mml,
                                page
                                , (int)parent.instPCM[n].stAdr
                                , (int)parent.instPCM[n].edAdr);
                        }
                    }
                    return;
                }

                if (page.Type == enmChannelType.SSG)
                {
                    SetEnvelopParamFromInstrument(page, n, mml);
                    return;
                }
            }

            if (type == 'I')
            {
                if (page.Type == enmChannelType.SSG)
                {
                    n = Common.CheckRange(n, 0, 9);
                    page.dutyCycle = n;
                    return;
                }

                msgBox.setErrMsg(msg.get("E11003"), mml.line.Lp);
                return;
            }

            if (type == 'T')
            {
                n = Common.CheckRange(n, 0, 255);
                page.toneDoubler = n;
                return;
            }

            if (type == 'E')
            {
                SetEnvelopParamFromInstrument(page, n, mml);
                return;
            }

            if (type == 'W')
            {
                SetWaveTableFromInstrument(page, mml);
                return;
            }

            if (page.Type == enmChannelType.SSG)
            {
                SetEnvelopParamFromInstrument(page, n, mml);
                return;
            }

            n = Common.CheckRange(n, 0, 255);
            if (page.instrument == n) return;
            page.instrument = n;
            OutFmSetInstrument(page, mml, n, page.volume, type);
        }

        // mml2vgm                      : opna2 effect ch
        // U01-12 FM     ch1-12 ( 0-11) : 0-11
        // U13-15 FMch3ex  1-3  (12-14) : 2
        // U16-18 FMch9ex  1-3  (15-17) : 8
        // U19-30 PSG    ch1-12 (18-29) : 12-23
        // U31-36 Rhythm ch1-6  (30-35) : 27-32
        // U37    ADPCM0 ch1    (36)    : 24
        // U38    ADPCM1 ch1    (37)    : 25
        // U39    ADPCM2 ch1    (38)    : 26
        // U40-45 ADPCMA ch1-6  (39-44) : 33-38

        public override void CmdEffect(partPage page, MML mml)
        {
            if (mml.args.Count == 3)
            {
                byte ch = (byte)page.ch;
                if (ch < 12)
                {
                    ;
                }
                else if (ch < 15)
                {
                    ch = 2;
                }
                else if (ch < 18)
                {
                    ch = 8;
                }
                else if (ch < 30)
                {
                    ch = (byte)(ch - 6);
                }
                else if (ch < 36)
                {
                    ch = (byte)(ch - 3);
                }
                else if (ch < 39)
                {
                    ch = (byte)(ch - 12);
                }
                else
                {
                    ch = (byte)(ch - 6);
                }

                if ((string)mml.args[0] == "Rv")
                {
                    //Reverb
                    switch ((char)mml.args[1])
                    {
                        case 'D':
                            int v = (int)mml.args[2];
                            parent.OutData(mml, port[3], 0x22, (byte)v);
                            break;
                        case 'S':
                            int sl = (int)mml.args[2];
                            parent.OutData(mml, port[3], 0x23, ch);
                            parent.OutData(mml, port[3], 0x24, (byte)sl);
                            break;
                    }
                }
                else if ((string)mml.args[0] == "Ds")
                {
                    int v = (int)mml.args[2];
                    //Distortion
                    switch ((char)mml.args[1])
                    {
                        case 'S'://switch
                            page.effectDistortionSwitch = v * 0x80;
                            parent.OutData(mml, port[3], 0x23, ch);
                            parent.OutData(mml, port[3], 0x25, (byte)(page.effectDistortionSwitch + page.effectDistortionVolume));
                            break;
                        case 'V'://volume
                            page.effectDistortionVolume = v & 0x7f;
                            parent.OutData(mml, port[3], 0x23, ch);
                            parent.OutData(mml, port[3], 0x25, (byte)(page.effectDistortionSwitch + page.effectDistortionVolume));
                            break;
                        case 'G'://gain
                            parent.OutData(mml, port[3], 0x23, ch);
                            parent.OutData(mml, port[3], 0x26, (byte)(v & 0x7f));
                            break;
                        case 'C'://CutOff
                            parent.OutData(mml, port[3], 0x23, ch);
                            parent.OutData(mml, port[3], 0x27, (byte)(v & 0x7f));
                            break;
                    }
                }
            }
        }

        public void SetWaveTableFromInstrument(partPage page, MML mml)
        {
            int p = 0;
            foreach (object o in mml.args)
            {
                p++;
                if (p == 1 || o == null)
                {
                    if (p == 5) break;
                    continue;
                }

                OutFmSetWaveTable(mml, page, p - 2, o);

                if (p == 5) break;
            }

            SetDummyData(page, mml);
        }

        public void OutFmSetWaveTable(MML mml, partPage page, int ope, object prm)
        {
            int vch;
            byte[] port;
            GetPortVch(page, out port, out vch);
            vch = page.ch;
            int n = 0;
            bool reset = false;
            if (prm is string)
            {
                reset = true;
            }
            else
            {
                n = (int)prm;
            }

            //データを流し込みますよ？宣言を送信
            parent.OutData(mml, port, 0x2b, (byte)((vch << 4) | ((reset ? 1 : 0) << 2) | (ope & 0x3)));

            //実はリセット宣言だった　又は流し込むデータなんてなかった場合は処理終了
            if (reset || !parent.instOPNA2WF.ContainsKey(n)) return;

            //データの流し込みいくよー
            ushort[] wd = parent.instOPNA2WF[n];
            for (n = 0; n < wd.Length; n++)
            {
                if (n == 0) continue;

                short s = (short)wd[n];
                ushort d = (ushort)((4095 - Math.Abs(s)) * 2 + (s < 0 ? 1 : 0));
                parent.OutData(mml, port, 0x2c, (byte)d);
                parent.OutData(mml, port, 0x2c, (byte)(d >> 8));

            }
        }

        public void OutFmSetWtLDtMl(MML mml, partPage page, int ope, int wt, int dt, int ml)
        {
            int vch;
            byte[] port;
            GetPortVch(page, out port, out vch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            wt &= 1;
            dt &= 7;
            ml &= 15;

            parent.OutData(mml, port, (byte)(0x30 + vch + ope * 4), (byte)((wt << 7) | (dt << 4) | ml));
        }

        public void OutFmSetWtHTl(MML mml, partPage page, int ope, int wt, int tl)
        {
            int vch;
            byte[] port;
            GetPortVch(page, out port, out vch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            wt &= 2;
            tl &= 0x7f;

            parent.OutData(mml, port, (byte)(0x40 + vch + ope * 4), (byte)((wt << 6) | tl));
        }

        public void OutFmSetAmDt2Dr(MML mml, partPage page, int ope, int am, int dt2, int dr)
        {
            int vch;
            byte[] port;
            GetPortVch(page, out port, out vch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            am &= 1;
            dt2 &= 3;
            dr &= 31;

            parent.OutData(mml, port, (byte)(0x60 + vch + ope * 4), (byte)((am << 7) | (dt2 << 5) | dr));
        }

        public void OutFmSetFbSr(MML mml, partPage page, int ope, int fb, int sr)
        {
            int vch;
            byte[] port;
            GetPortVch(page, out port, out vch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            fb &= 7;
            sr &= 31;

            parent.OutData(mml, port, (byte)(0x70 + vch + ope * 4), (byte)((fb << 5) | sr));
        }

        public void OutFmSetALGLinkSSGEG(MML mml, partPage page, int ope, int all, int ssg)
        {
            int vch;
            byte[] port;
            GetPortVch(page, out port, out vch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            all &= 15;
            ssg &= 15;

            parent.OutData(mml, port, (byte)(0x90 + vch + ope * 4), (byte)((all << 4) | ssg));
        }

        public new void OutFmSetInstrument(partPage page, MML mml, int n, int vol, char typeBeforeSend)
        {
            int modeBeforeSend = parent.info.modeBeforeSend;
            if (typeBeforeSend == 'n' || typeBeforeSend == 'N' || typeBeforeSend == 'R' || typeBeforeSend == 'A')
            {
                if (typeBeforeSend == 'N')
                {
                    modeBeforeSend = 0;
                }
                else if (typeBeforeSend == 'R')
                {
                    modeBeforeSend = 1;
                }
                else if (typeBeforeSend == 'A')
                {
                    modeBeforeSend = 2;
                }
            }

            if (!parent.instFM.ContainsKey(n))
            {
                msgBox.setWrnMsg(string.Format(msg.get("E11001"), n), mml.line.Lp);
                return;
            }

            //int m = 3;

            if (page.ch >= 12 && page.ch < 18)
            {
                msgBox.setWrnMsg(msg.get("E11002"), mml.line.Lp);
                return;
            }

            switch (modeBeforeSend)
            {
                case 0: // N)one
                    break;
                case 1: // R)R only
                    for (int ope = 0; ope < 4; ope++) ((ClsOPN)page.chip).OutFmSetSlRr(mml, page, ope, 0, 15);
                    break;
                case 2: // A)ll
                    for (int ope = 0; ope < 4; ope++)
                    {
                        OutFmSetWtLDtMl(mml, page, ope, 0, 0, 0);
                        ((ClsOPN)page.chip).OutFmSetKsAr(mml, page, ope, 3, 31);
                        OutFmSetAmDt2Dr(mml, page, ope, 1, 0, 31);
                        ((ClsOPN)page.chip).OutFmSetSr(mml, page, ope, 31);
                        ((ClsOPN)page.chip).OutFmSetSlRr(mml, page, ope, 0, 15);
                        ((ClsOPN)page.chip).OutFmSetSSGEG(mml, page, ope, 0);
                    }
                    page.feedBack = 7;
                    page.algo = 7;
                    OutFmSetPanRFeedbackAlgorithm(mml, page);
                    break;
            }

            if (parent.instFM[n].Length == Const.OPNA2_INSTRUMENT_SIZE)
            {
                OutFmSetInstrumentOPNA(page, mml, n, vol);
                return;
            }

            for (int ope = 0; ope < 4; ope++)
            {
                //ch3以外の拡張チャンネルでも音色設定できるようにする場合はslotの様子もみてセットすること
                OutFmSetWtLDtMl(mml, page, ope
                    , 0
                    , parent.instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 1 + 8] // 8 : DT1
                    , parent.instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 1 + 7]); // 7 : ML
                ((ClsOPN)page.chip).OutFmSetKsAr(mml, page, ope, parent.instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 7], parent.instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 1]);
                OutFmSetAmDt2Dr(mml, page, ope, parent.instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 10], 0, parent.instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 2]);
                ((ClsOPN)page.chip).OutFmSetSr(mml, page, ope, parent.instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 3]);
                ((ClsOPN)page.chip).OutFmSetSlRr(mml, page, ope, parent.instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 5], parent.instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 4]);
                ((ClsOPN)page.chip).OutFmSetSSGEG(mml, page, ope, parent.instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 11]);
            }

            //ch3以外の拡張チャンネルでも音色設定できるようにする場合はslotの様子もみてセットすること
            page.op1ml = parent.instFM[n][0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
            page.op2ml = parent.instFM[n][1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
            page.op3ml = parent.instFM[n][2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
            page.op4ml = parent.instFM[n][3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
            //ch3以外の拡張チャンネルでも音色設定できるようにする場合はslotの様子もみてセットすること
            page.op1dt2 = 0;
            page.op2dt2 = 0;
            page.op3dt2 = 0;
            page.op4dt2 = 0;

            page.feedBack = parent.instFM[n][46];
            page.algo = parent.instFM[n][45] & 0x7;
            OutFmSetPanRFeedbackAlgorithm(mml, page);

            int[] op = new int[4] {
                parent.instFM[n][0*Const.INSTRUMENT_M_OPERATOR_SIZE + 6]
                , parent.instFM[n][1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 6]
                , parent.instFM[n][2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 6]
                , parent.instFM[n][3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 6]
            };
            int[][] algs = new int[8][]
            {
                new int[4] { 1,1,1,0}
                ,new int[4] { 1,1,1,0}
                ,new int[4] { 1,1,1,0}
                ,new int[4] { 1,1,1,0}
                ,new int[4] { 1,0,1,0}
                ,new int[4] { 1,0,0,0}
                ,new int[4] { 1,0,0,0}
                ,new int[4] { 0,0,0,0}
            };

            for (int i = 0; i < 4; i++)
            {
                //ch3以外の拡張チャンネルでも音色設定できるようになったら以下を有効に
                if (algs[page.algo][i] == 0)// || (pw.ppg[pw.cpgNum].slots & (1 << i)) == 0)
                {
                    op[i] = -1;
                    continue;
                }
                op[i] = Common.CheckRange(op[i], 0, 127);
            }

            partPage vpg = page;
            if (page.chip.lstPartWork[2].cpg.Ch3SpecialMode && page.ch >= 12 && page.ch < 15)
            {
                vpg = page.chip.lstPartWork[2].cpg;
            }

            if (page.chip.lstPartWork[8].cpg.Ch3SpecialMode && page.ch >= 15 && page.ch < 18)
            {
                vpg = page.chip.lstPartWork[8].cpg;
            }

            //ch3以外の拡張チャンネルでも音色設定できるようになったら以下を有効に
            //if ((pw.ppg[pw.cpgNum].slots & 1) != 0 && op[0] != -1) ((ClsOPN)pw.ppg[pw.cpgNum].chip).OutFmSetTl(vpw, 0, op[0]);
            //if ((pw.ppg[pw.cpgNum].slots & 2) != 0 && op[1] != -1) ((ClsOPN)pw.ppg[pw.cpgNum].chip).OutFmSetTl(vpw, 1, op[1]);
            //if ((pw.ppg[pw.cpgNum].slots & 4) != 0 && op[2] != -1) ((ClsOPN)pw.ppg[pw.cpgNum].chip).OutFmSetTl(vpw, 2, op[2]);
            //if ((pw.ppg[pw.cpgNum].slots & 8) != 0 && op[3] != -1) ((ClsOPN)pw.ppg[pw.cpgNum].chip).OutFmSetTl(vpw, 3, op[3]);
            if (op[0] != -1) OutFmSetWtHTl(mml, vpg, 0, 0, op[0]);
            if (op[1] != -1) OutFmSetWtHTl(mml, vpg, 1, 0, op[1]);
            if (op[2] != -1) OutFmSetWtHTl(mml, vpg, 2, 0, op[2]);
            if (op[3] != -1) OutFmSetWtHTl(mml, vpg, 3, 0, op[3]);

            OutFmSetVolumeM(page, mml, vol, n);

            //拡張チャンネルの場合は他の拡張チャンネルも音量を再セットする
            if (page.Type == enmChannelType.FMOPNex)
            {
                if (page.ch == 2 || page.ch == 12 || page.ch == 13 || page.ch == 14)
                {
                    //YM2609 ch3 || ch13 || ch14 || ch15
                    if (page.ch != 2) OutFmSetVolumeM(page.chip.lstPartWork[2].cpg, mml, page.chip.lstPartWork[2].cpg.volume, n);
                    if (page.ch != 12) OutFmSetVolumeM(page.chip.lstPartWork[12].cpg, mml, page.chip.lstPartWork[12].cpg.volume, n);
                    if (page.ch != 13) OutFmSetVolumeM(page.chip.lstPartWork[13].cpg, mml, page.chip.lstPartWork[13].cpg.volume, n);
                    if (page.ch != 14) OutFmSetVolumeM(page.chip.lstPartWork[14].cpg, mml, page.chip.lstPartWork[14].cpg.volume, n);
                }
                else
                {
                    //YM2609 ch9 || ch16 || ch17 || ch18
                    if (page.ch != 8) OutFmSetVolumeM(page.chip.lstPartWork[8].cpg, mml, page.chip.lstPartWork[8].cpg.volume, n);
                    if (page.ch != 15) OutFmSetVolumeM(page.chip.lstPartWork[15].cpg, mml, page.chip.lstPartWork[15].cpg.volume, n);
                    if (page.ch != 16) OutFmSetVolumeM(page.chip.lstPartWork[16].cpg, mml, page.chip.lstPartWork[16].cpg.volume, n);
                    if (page.ch != 17) OutFmSetVolumeM(page.chip.lstPartWork[17].cpg, mml, page.chip.lstPartWork[17].cpg.volume, n);
                }
            }
        }

        private void OutFmSetInstrumentOPNA(partPage page, MML mml, int n, int vol)
        {
            for (int ope = 0; ope < 4; ope++)
            {
                //ch3以外の拡張チャンネルでも音色設定できるようにする場合はslotの様子もみてセットすること
                OutFmSetWtLDtMl(mml, page, ope
                    , parent.instFM[n][ope * 15 + 1 + 13] // 13 : WT  1 : No  15 : OPE Size
                    , parent.instFM[n][ope * 15 + 1 + 8] // 8 : DT1
                    , parent.instFM[n][ope * 15 + 1 + 7]); // 7 : ML

                ((ClsOPN)page.chip).OutFmSetKsAr(mml, page, ope
                    , parent.instFM[n][ope * 15 + 1 + 6] // 6 : KS
                    , parent.instFM[n][ope * 15 + 1 + 0] // 0 : AR
                    );
                OutFmSetAmDt2Dr(mml, page, ope
                    , parent.instFM[n][ope * 15 + 1 + 10] // 10 : AM
                    , parent.instFM[n][ope * 15 + 1 + 9] // 9 : DT2
                    , parent.instFM[n][ope * 15 + 1 + 1] // 1 : DR
                    );
                OutFmSetFbSr(mml, page, ope
                    , parent.instFM[n][ope * 15 + 1 + 12] // 12 : FB
                    , parent.instFM[n][ope * 15 + 1 + 2] //2 : SR
                    );
                ((ClsOPN)page.chip).OutFmSetSlRr(mml, page, ope
                    , parent.instFM[n][ope * 15 + 1 + 4] // 4 : SL
                    , parent.instFM[n][ope * 15 + 1 + 3] // 3 : RR
                    );
                OutFmSetALGLinkSSGEG(mml, page, ope
                    , parent.instFM[n][ope * 15 + 1 + 14] // 14 : ALG Link
                    , parent.instFM[n][ope * 15 + 1 + 11] // 11 : SSG-EG
                    );
            }

            //ch3以外の拡張チャンネルでも音色設定できるようにする場合はslotの様子もみてセットすること
            page.op1ml = parent.instFM[n][0 * 15 + 1 + 7];// 7 : ML
            page.op2ml = parent.instFM[n][1 * 15 + 1 + 7];// 7 : ML
            page.op3ml = parent.instFM[n][2 * 15 + 1 + 7];// 7 : ML
            page.op4ml = parent.instFM[n][3 * 15 + 1 + 7];// 7 : ML
            //ch3以外の拡張チャンネルでも音色設定できるようにする場合はslotの様子もみてセットすること
            page.op1dt2 = parent.instFM[n][0 * 15 + 1 + 9];// 9 : DT2
            page.op2dt2 = parent.instFM[n][1 * 15 + 1 + 9];// 9 : DT2
            page.op3dt2 = parent.instFM[n][2 * 15 + 1 + 9];// 9 : DT2
            page.op4dt2 = parent.instFM[n][3 * 15 + 1 + 9];// 9 : DT2

            page.feedBack = parent.instFM[n][1 + 12] & 7;
            page.algo = parent.instFM[n][61] == 0xff ? 8 : (parent.instFM[n][61] & 0x7);
            OutFmSetPanRFeedbackAlgorithm(mml, page);

            int[] op = new int[4] {
                parent.instFM[n][0 * 15 + 1 + 5]// 5 : TL
                , parent.instFM[n][1 * 15 + 1 + 5]// 5 : TL
                , parent.instFM[n][2 * 15 + 1 + 5]// 5 : TL
                , parent.instFM[n][3 * 15 + 1 + 5]// 5 : TL
            };

            int[][] algs = new int[9][]
            {
                new int[4] { 1 , 1 , 1 , 0 }
                ,new int[4] { 1 , 1 , 1 , 0 }
                ,new int[4] { 1 , 1 , 1 , 0 }
                ,new int[4] { 1 , 1 , 1 , 0 }
                ,new int[4] { 1 , 0 , 1 , 0 }
                ,new int[4] { 1 , 0 , 0 , 0 }
                ,new int[4] { 1 , 0 , 0 , 0 }
                ,new int[4] { 0 , 0 , 0 , 0 }
                ,new int[4] { 0 , 0 , 0 , 0 } // ALG Link向け
            };

            if (page.algo == 8)
            {
                algs = GetVolumeOpe(algs, parent.instFM[n], true);
            }

            for (int i = 0; i < 4; i++)
            {
                //ch3以外の拡張チャンネルでも音色設定できるようになったら以下を有効に
                if (algs[page.algo][i] == 0)// || (pw.ppg[pw.cpgNum].slots & (1 << i)) == 0)
                {
                    op[i] = -1;
                    continue;
                }
                op[i] = Common.CheckRange(op[i], 0, 127);
            }

            partPage vpg = page;
            if (page.chip.lstPartWork[2].cpg.Ch3SpecialMode && page.ch >= 12 && page.ch < 15)
            {
                vpg = page.chip.lstPartWork[2].cpg;
            }

            if (page.chip.lstPartWork[8].cpg.Ch3SpecialMode && page.ch >= 15 && page.ch < 18)
            {
                vpg = page.chip.lstPartWork[8].cpg;
            }

            if (op[0] != -1) OutFmSetWtHTl(mml, vpg, 0, 0, op[0]);
            if (op[1] != -1) OutFmSetWtHTl(mml, vpg, 1, 0, op[1]);
            if (op[2] != -1) OutFmSetWtHTl(mml, vpg, 2, 0, op[2]);
            if (op[3] != -1) OutFmSetWtHTl(mml, vpg, 3, 0, op[3]);

            OutFmSetVolumeM(page, mml, vol, n);

        }

        private int[][] GetVolumeOpe(int[][] algs, byte[] inst, bool reverse)
        {
            byte[] a = new byte[]{
                    inst[0 * 15 + 1 + 14]// 14 : ALG Link
                    ,inst[1 * 15 + 1 + 14]// 14 : ALG Link
                    ,inst[2 * 15 + 1 + 14]// 14 : ALG Link
                    ,inst[3 * 15 + 1 + 14]// 14 : ALG Link
                };

            for (int ope = 0; ope < 4; ope++)
            {
                for (int i = 0; i < 4; i++)
                    if ((a[i] & (1 << i)) != 0) a[i] = 0; //自分が設定されている場合はキャリア

                algs[8][ope] = (((a[0] | a[1] | a[2] | a[3]) & (1 << ope)) == 0) ? 1 : 0;//自分を親とするopeが一つもないかチェック
                if (reverse) algs[8][ope] = algs[8][ope] == 0 ? 1 : 0;//リバース指定の場合は数値を入れ替える
            }

            return algs;
        }

        public void OutFmSetVolumeM(partPage page, MML mml, int vol, int n)
        {
            if (!parent.instFM.ContainsKey(n))
            {
                msgBox.setWrnMsg(string.Format(msg.get("E11000"), n), mml.line.Lp);
                return;
            }

            int alg;
            int[] ope;
            if (parent.instFM[n].Length == Const.OPNA2_INSTRUMENT_SIZE)
            {
                alg = parent.instFM[n][61] == 0xff ? 8 : (parent.instFM[n][61] & 0x7);
                ope = new int[4] {
                    parent.instFM[n][0 * 15 + 1 + 5]// 5 : TL
                    , parent.instFM[n][1 * 15 + 1 + 5]// 5 : TL
                    , parent.instFM[n][2 * 15 + 1 + 5]// 5 : TL
                    , parent.instFM[n][3 * 15 + 1 + 5]// 5 : TL
                };
            }
            else
            {
                alg = parent.instFM[n][45] & 0x7;
                ope = new int[4] {
                    parent.instFM[n][0*Const.INSTRUMENT_M_OPERATOR_SIZE + 6]
                    , parent.instFM[n][1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 6]
                    , parent.instFM[n][2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 6]
                    , parent.instFM[n][3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 6]
                };
            }

            int[][] algs = new int[9][]
            {
                new int[4] { 0,0,0,1}
                ,new int[4] { 0,0,0,1}
                ,new int[4] { 0,0,0,1}
                ,new int[4] { 0,0,0,1}
                ,new int[4] { 0,1,0,1}
                ,new int[4] { 0,1,1,1}
                ,new int[4] { 0,1,1,1}
                ,new int[4] { 1,1,1,1}
                ,new int[4] { 0 , 0 , 0 , 0 } // ALG Link向け
            };

            if (alg == 8)
            {
                algs = GetVolumeOpe(algs, parent.instFM[n], false);
            }

            for (int i = 0; i < 4; i++)
            {
                if (algs[alg][i] == 0 || (page.slots & (1 << i)) == 0)
                {
                    ope[i] = -1;
                    continue;
                }
                ope[i] = ope[i] + (127 - vol);
                ope[i] = Common.CheckRange(ope[i], 0, 127);
            }

            partPage vpg = page;
            if (page.chip.lstPartWork[2].cpg.Ch3SpecialMode && page.ch >= 12 && page.ch < 15)
            {
                vpg = page.chip.lstPartWork[2].cpg;
            }
            if (page.chip.lstPartWork[8].cpg.Ch3SpecialMode && page.ch >= 15 && page.ch < 18)
            {
                vpg = page.chip.lstPartWork[8].cpg;
            }

            MML vmml = new MML();
            vmml.args = new List<object>();
            vmml.args.Add(vol);
            vmml.type = enmMMLType.Volume;
            if (mml != null)
                vmml.line = mml.line;
            if ((page.slots & 1) != 0 && ope[0] != -1) OutFmSetWtHTl(vmml, vpg, 0, 0, ope[0]);
            if ((page.slots & 2) != 0 && ope[1] != -1) OutFmSetWtHTl(vmml, vpg, 1, 0, ope[1]);
            if ((page.slots & 4) != 0 && ope[2] != -1) OutFmSetWtHTl(vmml, vpg, 2, 0, ope[2]);
            if ((page.slots & 8) != 0 && ope[3] != -1) OutFmSetWtHTl(vmml, vpg, 3, 0, ope[3]);
            //if ((pw.ppg[pw.cpgNum].slots & 1) != 0 ) ((ClsOPN)pw.ppg[pw.cpgNum].chip).OutFmSetTl(vpw, 0, ope[0]);
            //if ((pw.ppg[pw.cpgNum].slots & 2) != 0 ) ((ClsOPN)pw.ppg[pw.cpgNum].chip).OutFmSetTl(vpw, 1, ope[1]);
            //if ((pw.ppg[pw.cpgNum].slots & 4) != 0 ) ((ClsOPN)pw.ppg[pw.cpgNum].chip).OutFmSetTl(vpw, 2, ope[2]);
            //if ((pw.ppg[pw.cpgNum].slots & 8) != 0 ) ((ClsOPN)pw.ppg[pw.cpgNum].chip).OutFmSetTl(vpw, 3, ope[3]);
        }

        public override void MultiChannelCommand(MML mml)
        {
            if (!use) return;
            //コマンドを跨ぐデータ向け処理
            foreach (partWork pw in lstPartWork)
            {
                foreach (partPage page in pw.pg)
                {
                    if (page.Type == enmChannelType.RHYTHM)//固定周波数ADPCM
                    {
                        //Rhythm Volume処理
                        if (page.beforeVolume != page.volume || !page.pan.eq())
                        {
                            parent.OutData(mml, port[0], (byte)(0x18 + (page.ch - 30)), (byte)((byte)((page.pan.val & 0x3) << 6) | (byte)(page.volume & 0x1f)));
                            page.beforeVolume = page.volume;
                            page.pan.rst();
                        }

                        rhythm_KeyOn |= (byte)(page.keyOn ? (1 << (page.ch - 30)) : 0);
                        page.keyOn = false;
                        rhythm_KeyOff |= (byte)(page.keyOff ? (1 << (page.ch - 30)) : 0);
                        page.keyOff = false;
                    }
                    else if (page.Type == enmChannelType.ADPCM)//固定周波数ADPCM
                    {
                        //ADPCM-A Volume処理
                        if (page.beforeVolume != page.volume)
                        {
                            parent.OutData(mml, port[1], (byte)0x13, (byte)(page.ch - 39));
                            parent.OutData(mml, port[1], (byte)0x14, (byte)(page.volume & 0x1f));
                            page.beforeVolume = page.volume;
                        }

                        if (page.pan.val != page.beforePan.val || page.panR != page.beforePanR || page.panL != page.beforePanL)
                        {
                            parent.OutData(mml, port[1], (byte)0x13, (byte)(page.ch - 39));
                            parent.OutData(mml, port[1], (byte)0x15, (byte)(
                                ((page.pan.val & 2) << 6)
                                | ((page.panL & 3) << 5)
                                | ((page.pan.val & 1) << 4)
                                | ((page.panR & 3) << 2)
                                ));
                            page.beforePan.val = page.pan.val;
                            page.beforePanR = page.panR;
                            page.beforePanL = page.panL;
                        }

                        adpcma_KeyOn |= (byte)(page.keyOn ? (1 << (page.ch - 39)) : 0);
                        page.keyOn = false;
                        adpcma_KeyOff |= (byte)(page.keyOff ? (1 << (page.ch - 39)) : 0);
                        page.keyOff = false;
                    }
                    else if (page.Type == enmChannelType.SSG)
                    {
                        MultiChannelCommand_SSG(mml, page);
                    }
                    else if (page.Type == enmChannelType.FMOPN || page.Type == enmChannelType.FMOPNex)
                    {
                        MultiChannelCommand_FM(mml, page);
                    }
                }
            }

            //チャンネルを跨ぐデータ向け処理

            //Rhythm KeyOff処理
            if (0 != rhythm_KeyOff)
            {
                byte data = (byte)(0x80 + rhythm_KeyOff);
                parent.OutData(mml, port[0], 0x10, data);
                rhythm_KeyOff = 0;
            }

            //Rhythm TotalVolume処理
            if (rhythm_beforeTotalVolume != rhythm_TotalVolume)
            {
                parent.OutData(mml, port[0], 0x11, (byte)(rhythm_TotalVolume & 0x3f));
                rhythm_beforeTotalVolume = rhythm_TotalVolume;
            }

            //Rhythm KeyOn処理
            if (0 != rhythm_KeyOn)
            {
                byte data = (byte)(0x00 + rhythm_KeyOn);
                parent.OutData(mml, port[0], 0x10, data);
                rhythm_KeyOn = 0;
            }

            //adpcma KeyOff処理
            if (0 != adpcma_KeyOff)
            {
                byte data = (byte)(0x80 + adpcma_KeyOff);
                parent.OutData(mml, port[1], 0x11, data);
                adpcma_KeyOff = 0;
            }

            //adpcma TotalVolume処理
            if (adpcma_beforeTotalVolume != adpcma_TotalVolume)
            {
                parent.OutData(mml, port[1], 0x12, (byte)(adpcma_TotalVolume & 0x3f));
                adpcma_beforeTotalVolume = adpcma_TotalVolume;
            }

            //adpcma KeyOn処理
            if (0 != adpcma_KeyOn)
            {
                byte data = (byte)(0x00 + adpcma_KeyOn);
                parent.OutData(mml, port[1], 0x11, data);
                adpcma_KeyOn = 0;
            }

            log.Write("KeyOn情報をかき出し");
            int vch;
            byte[] p;
            GetPortVch(lstPartWork[0].cpg, out p, out vch);
            foreach (outDatum dat in opna20x028KeyOnData) parent.OutData(dat, p, 0x28, dat.val);
            GetPortVch(lstPartWork[6].cpg, out p, out vch);
            foreach (outDatum dat in opna20x228KeyOnData) parent.OutData(dat, p, 0x28, dat.val);
            opna20x028KeyOnData.Clear();
            opna20x228KeyOnData.Clear();
        }

        public List<outDatum> opna20x028KeyOnData = new List<outDatum>();
        public List<outDatum> opna20x228KeyOnData = new List<outDatum>();

        private void MultiChannelCommand_SSG(MML mml, partPage page)
        {
            int port;
            int adr;
            int vch;
            GetPortVchSsg(page, out port, out adr, out vch);

            //reg 00 - 05
            if (page.freq != page.oldFreq || page.dutyCycle != page.oldDutyCycle)
            {
                page.oldFreq = page.freq;
                page.oldDutyCycle = page.dutyCycle;

                byte data = (byte)(page.freq & 0xff);
                parent.OutData(mml, page.port[port], (byte)(adr + 0 + vch * 2), data);

                data = (byte)(((page.freq & 0xf00) >> 8) | ((page.dutyCycle & 0xf) << 4));
                parent.OutData(mml, page.port[port], (byte)(adr + 1 + vch * 2), data);
            }

        }

        private void MultiChannelCommand_FM(MML mml, partPage page)
        {
            //FNum l
            //panL Block FNum h
            if (page.panL != page.beforePanL || page.freq != page.oldFreq)
                OutFmSetPanLFnum(page, mml);

            if (page.ch < 12)
            {
                if (page.panR != page.beforePanR || page.feedBack != page.beforeFeedBack || page.algo != page.beforeAlgo)
                    OutFmSetPanRFeedbackAlgorithm(mml, page);
                if (page.pan.val != page.beforePan.val || page.ams != page.beforeAms || page.algConstSw != page.beforeAlgConstSw || page.pms != page.beforePms)
                    OutOPNSetPanAmsAcPms(mml, page);
            }
        }

        public override string DispRegion(clsPcm pcm)
        {
            return string.Format("{0,-10} {1,-7} {2,-5:D3} N/A  ${3,-7:X6} ${4,-7:X6} N/A      ${5,-7:X6}  NONE {6}\r\n"
                , Name
                , pcm.chipNumber != 0 ? "SEC" : "PRI"
                , pcm.num
                , pcm.stAdr & 0xffffff
                , pcm.edAdr & 0xffffff
                , pcm.size
                , pcm.status.ToString()
                );
        }

        public override void CmdLoopExtProc(partPage page, MML mml)
        {
            if (page.Type == enmChannelType.FMOPN || page.Type == enmChannelType.FMOPNex)
            {
                page.pan.val = (page.panL != 0 ? 2 : 0) | (page.panR != 0 ? 1 : 0);
                page.beforePanL = -1;
                page.beforePanR = -1;
            }
        }

    }
}
