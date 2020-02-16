using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public List<byte[]> pcmDataDirectA = new List<byte[]>();
        public List<byte[]> pcmDataDirectB = new List<byte[]>();
        public List<byte[]> pcmDataDirectC = new List<byte[]>();

        public int rhythm_TotalVolume = 63;
        public int rhythm_beforeTotalVolume = -1;
        public int rhythm_MAXTotalVolume = 63;
        public byte rhythm_KeyOn = 0;
        public byte rhythm_KeyOff = 0;

        public YM2609(ClsVgm parent, int chipID, string initialPartName, string stPath, int chipNumber) : base(parent, chipID, initialPartName, stPath, chipNumber)
        {
            _chipType = enmChipType.YM2609;
            _Name = "YM2609";
            _ShortName = "OPNA2";
            _ChMax = 12 + 6 + 12 + 6 + 3;//FM:12ch FMex:6ch SSG:12ch Rhythm:6ch ADPCM:3ch
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
            if (dic != null && dic.Count>0)
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
            }

            Ch[36].Type = enmChannelType.ADPCMA;
            Ch[37].Type = enmChannelType.ADPCMB;
            Ch[38].Type = enmChannelType.ADPCMB;

            pcmDataInfo = new clsPcmDataInfo[] { new clsPcmDataInfo(), new clsPcmDataInfo(), new clsPcmDataInfo() };
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
                    //                                     cmdL  cmdH  ccL   ccH   tt    size1 2     3     4
                    pcmDataInfo[0].totalBuf = new byte[] { 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                    pcmDataInfo[1].totalBuf = new byte[] { 0x07, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                    pcmDataInfo[2].totalBuf = new byte[] { 0x07, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                }
                else
                {
                    //                                     cmd   cc    tt    size1 2     3     4
                    pcmDataInfo[0].totalBuf = new byte[] { 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                    pcmDataInfo[1].totalBuf = new byte[] { 0x07, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                    pcmDataInfo[2].totalBuf = new byte[] { 0x07, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                }
            }
            else
            {
                //ZGM以外は使用できない
                pcmDataInfo[0].totalBuf = null;
                pcmDataInfo[1].totalBuf = null;
                pcmDataInfo[2].totalBuf = null;
            }

            pcmDataInfo[0].totalHeaderLength = pcmDataInfo[0].totalBuf.Length;
            pcmDataInfo[0].totalHeadrSizeOfDataPtr = (parent.ChipCommandSize == 2) ? 5 : 3;
            pcmDataInfo[1].totalHeaderLength = pcmDataInfo[1].totalBuf.Length;
            pcmDataInfo[1].totalHeadrSizeOfDataPtr = (parent.ChipCommandSize == 2) ? 5 : 3;
            pcmDataInfo[2].totalHeaderLength = pcmDataInfo[2].totalBuf.Length;
            pcmDataInfo[2].totalHeadrSizeOfDataPtr = (parent.ChipCommandSize == 2) ? 5 : 3;

            Envelope = new Function();
            Envelope.Max = 255;
            Envelope.Min = 0;

        }

        public override void InitChip()
        {
            if (!use) return;

            //initialize shared param
            OutOPNSetHardLfo(null, lstPartWork[0], false, 0);
            OutOPNSetCh3SpecialMode(null, lstPartWork[0], false);

            //FM Off
            OutFmAllKeyOff();

            //SSG Off
            for (int ch = 18; ch < 24; ch++)
            {
                OutSsgKeyOff(null, lstPartWork[ch]);
                lstPartWork[ch].volume = 0;
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
                if (pw.ch == 0 || pw.ch == 5)
                {
                    pw.hardLfoSw = false;
                    pw.hardLfoNum = 0;
                    OutOPNSetHardLfo(null, pw, pw.hardLfoSw, pw.hardLfoNum);
                }

                if (pw.ch < 12)
                {
                    pw.pan.val = 3;
                    pw.ams = 0;
                    pw.fms = 0;
                    if (!pw.dataEnd) OutOPNSetPanAMSPMS(null, pw, 3, 0, 0);
                }
            }

        }

        public override void InitPart(partWork pw)
        {
            pw.slots = (byte)((
                pw.Type == enmChannelType.FMOPN 
                || pw.ch == 2 
                || pw.ch == 8) ? 0xf : 0x0);

            pw.volume = 127;
            pw.MaxVolume = 127;
            if (pw.Type == enmChannelType.SSG)
            {
                pw.MaxVolume = 15;
                pw.volume = 0;
            }
            else if (pw.Type == enmChannelType.RHYTHM)
            {
                pw.MaxVolume = 31;//5bit
                pw.volume = pw.MaxVolume;
            }
            else if (pw.Type == enmChannelType.ADPCMA 
                || pw.Type == enmChannelType.ADPCMB)
            {
                pw.MaxVolume = 255;
                pw.volume = pw.MaxVolume;
            }

            pw.port = port;
        }


        public void SetADPCMAddress(MML mml, partWork pw, int startAdr, int endAdr)
        {
            int p = pw.ch == 36 ? 1 : 3;
            int v = pw.ch != 38 ? 0x00 : 0x11;

            if (pw.pcmStartAddress != startAdr)
            {
                parent.OutData(mml, port[p], (byte)(0x02 + v), (byte)((startAdr >> (2 + (p == 1 ? 0 : 3))) & 0xff));
                parent.OutData(mml, port[p], (byte)(0x03 + v), (byte)((startAdr >> (10 + (p == 1 ? 0 : 3))) & 0xff));
                pw.pcmStartAddress = startAdr;
            }

            if (pw.pcmEndAddress != endAdr)
            {
                parent.OutData(mml, port[p], (byte)(0x04 + v), (byte)(((endAdr - 0x04) >> (2 + (p == 1 ? 0 : 3))) & 0xff));
                parent.OutData(mml, port[p], (byte)(0x05 + v), (byte)(((endAdr - 0x04) >> (10 + (p == 1 ? 0 : 3))) & 0xff));
                pw.pcmEndAddress = endAdr;
            }
        }

        public void SetAdpcmFNum(MML mml, partWork pw)
        {
            int f = GetAdpcmFNum(pw.octaveNow, pw.noteCmd, pw.shift + pw.keyShift);//
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

            byte data = 0;
            int p = pw.ch == 36 ? 1 : 3;
            int v = pw.ch != 38 ? 0x00 : 0x11;

            data = (byte)(f & 0xff);
            parent.OutData(mml, port[p], (byte)(0x09 + v), data);

            data = (byte)((f & 0xff00) >> 8);
            parent.OutData(mml, port[p], (byte)(0x0a + v), data);
        }

        public void SetAdpcmVolume(MML mml, partWork pw)
        {

            int vol = pw.volume;
            if (pw.envelopeMode)
            {
                vol = 0;
                if (pw.envIndex != -1)
                {
                    vol = pw.volume - (0xff - pw.envVolume);
                }
            }
            vol = Common.CheckRange(vol, 0, 0xff);

            if (pw.beforeVolume != vol)
            {
                int p = pw.ch == 36 ? 1 : 3;
                int v = pw.ch != 38 ? 0x00 : 0x11;
                parent.OutData(mml, port[p], (byte)(0x0b + v), (byte)vol);
                pw.beforeVolume = pw.volume;
            }
        }

        public void SetAdpcmPan(MML mml, partWork pw, int panL, int panR)
        {
            panL = Common.CheckRange(panL, 0, 4);
            panR = Common.CheckRange(panR, 0, 4);

            if (pw.panL != panL || pw.panR != panR)
            {
                int port = pw.ch == 36 ? 1 : 3;
                int adr = pw.ch != 38 ? 0x00 : 0x11;

                int v = (panL != 0 ? 0x80 : 00)| (panR != 0 ? 0x40 : 00);
                parent.OutData(mml, base.port[port], (byte)(0x01 + adr), (byte)v);

                v = (((4 - panL) & 0x3) << 6) | (((4 - panR) & 0x3) << 4);
                parent.OutData(mml, base.port[port], (byte)(0x07 + adr), (byte)v);

                pw.panL = panL;
                pw.panR = panR;
            }
        }

        public int GetAdpcmFNum(int octave, char noteCmd, int shift)
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

            return (int)(0x49ba * Const.pcmMTbl[n] * Math.Pow(2, (o - 4)));
        }

        public void OutAdpcmKeyOn(MML mml, partWork pw)
        {

            SetAdpcmVolume(mml, pw);
            int p = pw.ch == 36 ? 1 : 3;
            int v = pw.ch != 38 ? 0x00 : 0x11;
            parent.OutData(mml, port[p], (byte)(0x00+v), 0xa0);

        }

        public void OutAdpcmKeyOff(MML mml, partWork pw)
        {
            int p = pw.ch == 36 ? 1 : 3;
            int v = pw.ch != 38 ? 0x00 : 0x11;

            parent.OutData(mml, port[p],(byte)(0x00+v), 0x01);

        }


        public override void SetFNum(partWork pw, MML mml)
        {
            if (pw.ch < 18)
            {
                SetFmFNum(pw, mml);
            }
            else if (pw.Type == enmChannelType.SSG)
            {
                SetSsgFNum(pw, mml);
            }
            else if (pw.Type == enmChannelType.RHYTHM)
            {
            }
            else if (pw.Type == enmChannelType.ADPCMA || pw.Type == enmChannelType.ADPCMB)
            {
                SetAdpcmFNum(mml, pw);
            }
        }

        public new void SetFmFNum(partWork pw, MML mml)
        {
            if (pw.noteCmd == (char)0)
                return;

            int[] ftbl = pw.chip.FNumTbl[0];

            int f = GetFmFNum(ftbl, pw.octaveNow, pw.noteCmd, pw.shift + pw.keyShift + pw.toneDoublerKeyShift);//
            if (pw.bendWaitCounter != -1)
            {
                f = pw.bendFnum;
            }
            int o = (f & 0xf000) / 0x1000;
            f &= 0xfff;

            f = f + pw.detune;
            for (int lfo = 0; lfo < 4; lfo++)
            {
                if (!pw.lfo[lfo].sw || pw.lfo[lfo].type != eLfoType.Vibrato)
                    continue;

                f += pw.lfo[lfo].value + pw.lfo[lfo].param[6];
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
            OutFmSetFnum(pw, mml, o, f);
            //pw.freq = Common.CheckRange(f, 0, 0x7ff) | (Common.CheckRange(o - 1, 0, 7) << (8 + 3));

        }

        public void OutFmSetPanLFnum(partWork pw, MML mml)
        {
            pw.oldFreq = pw.freq;
            pw.beforePanL = pw.panL;

            int portEx = -1;
            if ((pw.ch == 2 || pw.ch == 12 || pw.ch == 13 || pw.ch == 14) && pw.chip.lstPartWork[2].Ch3SpecialMode)
                portEx = 0;
            else if ((pw.ch == 8 || pw.ch == 15 || pw.ch == 16 || pw.ch == 17) && pw.chip.lstPartWork[8].Ch3SpecialMode)
                portEx = 2;

            if (portEx != -1)
            {
                if ((pw.slots & 8) != 0 && pw.freq != -1)
                {
                    int f = pw.freq + pw.slotDetune[3];
                    parent.OutData(mml, pw.port[portEx], (byte)0xa6, (byte)((f & 0x3f00) >> 8));
                    parent.OutData(mml, pw.port[portEx], (byte)0xa2, (byte)(f & 0xff));
                }
                if ((pw.slots & 4) != 0 && pw.freq != -1)
                {
                    int f = pw.freq + pw.slotDetune[2];
                    parent.OutData(mml, pw.port[portEx], (byte)0xac, (byte)((f & 0x3f00) >> 8));
                    parent.OutData(mml, pw.port[portEx], (byte)0xa8, (byte)(f & 0xff));
                }
                if ((pw.slots & 1) != 0 && pw.freq != -1)
                {
                    int f = pw.freq + pw.slotDetune[0];
                    parent.OutData(mml, pw.port[portEx], (byte)0xad, (byte)((f & 0x3f00) >> 8));
                    parent.OutData(mml, pw.port[portEx], (byte)0xa9, (byte)(f & 0xff));
                }
                if ((pw.slots & 2) != 0 && pw.freq != -1)
                {
                    int f = pw.freq + pw.slotDetune[1];
                    parent.OutData(mml, pw.port[portEx], (byte)0xae, (byte)((f & 0x3f00) >> 8));
                    parent.OutData(mml, pw.port[portEx], (byte)0xaa, (byte)(f & 0xff));
                }
            }
            else
            {
                if (pw.ch >= 12 && pw.ch < 18)
                {
                    return;
                }

                int vch;
                byte[] port;
                GetPortVch(pw, out port, out vch);

                if (pw.freq != -1)
                {
                    parent.OutData(mml, port, (byte)(0xa4 + vch), (byte)(((pw.freq & 0x3f00) >> 8) | (((4 - pw.panL) & 0x3) << 6)));
                    parent.OutData(mml, port, (byte)(0xa0 + vch), (byte)(pw.freq & 0xff));
                }
            }
        }

        public void OutFmSetPanRFeedbackAlgorithm(MML mml, partWork pw)
        {
            int vch;
            byte[] port;
            GetPortVch(pw, out port, out vch);

            pw.feedBack &= 7;
            pw.algo &= 7;
            pw.beforeFeedBack = pw.feedBack;
            pw.beforeAlgo = pw.algo;
            pw.beforePanR = pw.panR;

            parent.OutData(mml, port, (byte)(0xb0 + vch), (byte)((((4 - pw.panR) & 0x3) << 6) | (pw.feedBack << 3) | pw.algo));
        }

        public void OutOPNSetPanAmsAcPms(MML mml, partWork pw)
        {
            //TODO: 効果音パートで指定されている場合の考慮不足
            int vch;
            byte[] port;
            GetPortVch(pw, out port, out vch);

            pw.beforePan.val = pw.pan.val;
            pw.beforeAms = pw.ams;
            pw.beforeAlgConstSw = pw.algConstSw;
            pw.beforePms = pw.pms;

            parent.OutData(mml, port, (byte)(0xb4 + vch), (byte)((pw.pan.val << 6) + (pw.ams << 3) + (pw.algConstSw << 2) + pw.pms));
        }

        public new void SetSsgFNum(partWork pw, MML mml)
        {
            int f = -pw.detune;
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
                f -= pw.lfo[lfo].value + pw.lfo[lfo].param[6];
            }

            if (pw.octaveNow < 1)
            {
                f <<= -pw.octaveNow;
            }
            else
            {
                f >>= pw.octaveNow - 1;
            }

            if (pw.bendWaitCounter != -1)
            {
                f += pw.bendFnum;
            }
            else
            {
                f += GetSsgFNum(pw, mml, pw.octaveNow, pw.noteCmd, pw.shift + pw.keyShift);//
            }

            f = Common.CheckRange(f, 0, 0xfff);
            //if (pw.freq == f && pw.dutyCycle == pw.oldDutyCycle) return;

            pw.freq = f;
            //pw.oldDutyCycle = pw.dutyCycle;

        }

        public override void SetKeyOn(partWork pw, MML mml)
        {
            if (pw.ch < 18)
                OutFmKeyOn(pw, mml);
            else if (pw.Type == enmChannelType.SSG)
            {
                OutSsgKeyOn(pw, mml);
            }
            else if (pw.Type == enmChannelType.RHYTHM)
            {
                pw.keyOn = true;
                pw.keyOff = false;
                SetDummyData(pw, mml);
            }
            else if (pw.Type == enmChannelType.ADPCMA ||pw.Type == enmChannelType.ADPCMB)
            {
                OutAdpcmKeyOn(mml, pw);
            }
        }

        public override void SetKeyOff(partWork pw, MML mml)
        {
            if (pw.ch < 18)
                OutFmKeyOff(pw, mml);
            else if (pw.Type == enmChannelType.SSG)
            {
                OutSsgKeyOff(mml, pw);
            }
            else if (pw.Type == enmChannelType.RHYTHM)
            {
                pw.keyOn = false;
                pw.keyOff = true;
            }
            else if (pw.Type == enmChannelType.ADPCMA|| pw.Type == enmChannelType.ADPCMB)
            {
                OutAdpcmKeyOff(mml, pw);
            }
        }

        public override void SetVolume(partWork pw, MML mml)
        {
            if (pw.Type == enmChannelType.FMOPN
                || pw.Type == enmChannelType.FMOPNex //効果音モード対応チャンネル
                || (pw.Type == enmChannelType.FMPCM && !pw.pcm) //OPN2PCMチャンネル
                || (pw.Type == enmChannelType.FMPCMex && !pw.pcm) //OPN2XPCMチャンネル
                )
            {
                SetFmVolumeM(pw, mml);
            }
            else if (pw.Type == enmChannelType.SSG)
            {
                SetSsgVolume(pw, mml);
            }
            else if (pw.Type == enmChannelType.RHYTHM)
            {
            }
            else if (pw.Type == enmChannelType.ADPCMA || pw.Type == enmChannelType.ADPCMB)
            {
                SetAdpcmVolume(mml, pw);
            }
        }

        public void SetFmVolumeM(partWork pw, MML mml)
        {
            int vol = pw.volume;

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

            if (pw.beforeVolume != vol)
            {
                if (parent.instFM.ContainsKey(pw.instrument))
                {
                    OutFmSetVolumeM(pw, mml, vol, pw.instrument);
                    pw.beforeVolume = vol;
                }
            }
        }


        public override void SetLfoAtKeyOn(partWork pw, MML mml)
        {
            base.SetLfoAtKeyOn(pw, mml);

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
                    if (pw.Type == enmChannelType.ADPCMA || pw.Type == enmChannelType.ADPCMB)
                        SetAdpcmFNum(mml, pw);

                }

                if (pl.type == eLfoType.Tremolo)
                {
                    pw.beforeVolume = -1;

                    //if (pw.Type == enmChannelType.RHYTHM)
                    //SetRhythmVolume(pw);
                    if (pw.Type == enmChannelType.ADPCMA || pw.Type == enmChannelType.ADPCMB)
                        SetAdpcmVolume(mml, pw);

                }

            }
        }

        public override void StorePcm(Dictionary<int, clsPcm> newDic, KeyValuePair<int, clsPcm> v, byte[] buf, bool is16bit, int samplerate, params object[] option)
        {
            int aCh = (int)v.Value.loopAdr;
            aCh = Common.CheckRange(aCh, 0, 2);
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

            SetPCMDataBlock_AB(mml, pcmDataEasyA, pcmDataDirectA);
            SetPCMDataBlock_AB(mml, pcmDataEasyB, pcmDataDirectB);
            SetPCMDataBlock_AB(mml, pcmDataEasyC, pcmDataDirectC);
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
                    pcmDataEasy[sizePtr+0]
                    + (pcmDataEasy[sizePtr+1] << 8)
                    + (pcmDataEasy[sizePtr+2] << 16)
                    + (pcmDataEasy[sizePtr+3] << 24);
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
                            dat[sizePtr+0]
                            + (dat[sizePtr+1] << 8)
                            + (dat[sizePtr+2] << 16)
                            + (dat[sizePtr+3] << 24);
                        if (maxSize < size) maxSize = size;
                    }
                }
            }
            if (pcmDataEasy != null && pcmDataEasy.Length > 0)
            {
                pcmDataEasy[sizePtr+0] = (byte)maxSize;
                pcmDataEasy[sizePtr+1] = (byte)(maxSize >> 8);
                pcmDataEasy[sizePtr+2] = (byte)(maxSize >> 16);
                pcmDataEasy[sizePtr+3] = (byte)(maxSize >> 24);
            }
            if (pcmDataDirect.Count > 0)
            {
                foreach (byte[] dat in pcmDataDirect)
                {
                    if (dat != null && dat.Length > 0)
                    {
                        dat[sizePtr+0] = (byte)maxSize;
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

        public override void CmdY(partWork pw, MML mml)
        {
            base.CmdY(pw, mml);

            if (mml.args[0] is string) return;

            byte adr = (byte)mml.args[0];
            byte dat = (byte)mml.args[1];
            int p = 0;

            if (pw.ch < 3)//FM1-3
                p = 0;
            else if (pw.ch < 6)//FM4-6
                p = 1;
            else if (pw.ch < 9)//FM7-9
                p = 2;
            else if (pw.ch < 12)//FM10-12
                p = 3;
            else if (pw.ch < 15)//FMex13-15
                p = 0;
            else if (pw.ch < 18)//FMex15-18
                p = 2;
            else if (pw.ch < 21)//SSG1-3
                p = 0;
            else if (pw.ch < 24)//SSG4-6
                p = 1;
            else if (pw.ch < 30)//SSG7-12
                p = 2;
            else if (pw.ch < 36)//Rhythm1-6
                p = 0;
            else if (pw.ch == 36)//ADPCM1
                p = 1;
            else if (pw.ch == 37)//ADPCM2
                p = 3;
            else if (pw.ch == 38)//ADPCM3
                p = 3;

            parent.OutData(mml, port[p] , adr, dat);
        }

        public override void CmdMPMS(partWork pw, MML mml)
        {
            if (pw.Type != enmChannelType.FMOPN)
            {
                msgBox.setWrnMsg(msg.get("E18000"), mml.line.Lp);
                return;
            }

            int n = (int)mml.args[1];
            n = Common.CheckRange(n, 0, 7);
            pw.pms = n;
            ((ClsOPN)pw.chip).OutOPNSetPanAMSPMS(
                mml,
                pw
                , (int)pw.pan.val
                , pw.ams
                , pw.pms);
        }

        public override void CmdMAMS(partWork pw, MML mml)
        {
            if (pw.Type != enmChannelType.FMOPN)
            {
                msgBox.setWrnMsg(msg.get("E18001"), mml.line.Lp);
                return;
            }

            int n = (int)mml.args[1];
            n = Common.CheckRange(n, 0, 3);
            pw.ams = n;
            ((ClsOPN)pw.chip).OutOPNSetPanAMSPMS(
                mml,
                pw
                , (int)pw.pan.val
                , pw.ams
                , pw.pms);
        }

        public override void CmdLfo(partWork pw, MML mml)
        {
            base.CmdLfo(pw, mml);

            if (mml.args[0] is string)
            {
                return;
            }

            int c = (char)mml.args[0] - 'P';
            if (pw.lfo[c].type == eLfoType.Hardware)
            {
                if (pw.Type == enmChannelType.FMOPN)
                {
                    if (pw.lfo[c].param.Count < 4)
                    {
                        msgBox.setErrMsg(msg.get("E18002"), mml.line.Lp);
                        return;
                    }
                    if (pw.lfo[c].param.Count > 5)
                    {
                        msgBox.setErrMsg(msg.get("E18003"), mml.line.Lp);
                        return;
                    }

                    pw.lfo[c].param[0] = Common.CheckRange(pw.lfo[c].param[0], 0, (int)parent.info.clockCount);//Delay(無視)
                    pw.lfo[c].param[1] = Common.CheckRange(pw.lfo[c].param[1], 0, 7);//Freq
                    pw.lfo[c].param[2] = Common.CheckRange(pw.lfo[c].param[2], 0, 7);//PMS
                    pw.lfo[c].param[3] = Common.CheckRange(pw.lfo[c].param[3], 0, 3);//AMS
                    if (pw.lfo[c].param.Count == 5)
                    {
                        pw.lfo[c].param[4] = Common.CheckRange(pw.lfo[c].param[4], 0, 1); //Switch
                    }
                    else
                    {
                        pw.lfo[c].param.Add(1);
                    }
                }
            }
        }

        public override void CmdLfoSwitch(partWork pw, MML mml)
        {
            base.CmdLfoSwitch(pw, mml);

            int c = (char)mml.args[0] - 'P';
            int n = (int)mml.args[1];
            if (pw.lfo[c].type == eLfoType.Hardware)
            {
                if (pw.Type == enmChannelType.FMOPN)
                {
                    if (pw.lfo[c].param[4] == 0)
                    {
                        pw.fms = (n == 0) ? 0 : pw.lfo[c].param[2];
                        pw.ams = (n == 0) ? 0 : pw.lfo[c].param[3];
                        ((ClsOPN)pw.chip).OutOPNSetPanAMSPMS(mml, pw, (int)pw.pan.val, pw.ams, pw.fms);
                        pw.chip.lstPartWork[0].hardLfoSw = (n != 0);
                        pw.chip.lstPartWork[0].hardLfoNum = pw.lfo[c].param[1];
                        ((ClsOPN)pw.chip).OutOPNSetHardLfo(null, pw, pw.chip.lstPartWork[0].hardLfoSw, pw.chip.lstPartWork[0].hardLfoNum);
                    }
                    else
                    {
                        ((ClsOPN)pw.chip).OutOPNSetHardLfo(mml, pw, false, pw.lfo[c].param[1]);
                    }
                }
            }
        }

        public override void CmdTotalVolume(partWork pw, MML mml)
        {
            int n = (int)mml.args[0];
            ((YM2609)pw.chip).rhythm_TotalVolume
                = Common.CheckRange(n, 0, ((YM2609)pw.chip).rhythm_MAXTotalVolume);
        }

        public override void CmdPan(partWork pw, MML mml)
        {
            int n = (int)mml.args[0];
            if (pw.Type == enmChannelType.FMOPN || pw.Type == enmChannelType.FMOPNex)
            {
                pw.panL = Common.CheckRange(n, 0, 4);
                n = mml.args.Count < 2 ? 0 : (int)mml.args[1];
                pw.panR = Common.CheckRange(n, 0, 4);
                pw.pan.val = (pw.panL != 0 ? 2 : 0) | (pw.panR != 0 ? 1 : 0);

                //pw.pan.val = n;
                //((ClsOPN)pw.chip).OutOPNSetPanAMSPMS(mml, pw, n, pw.ams, pw.fms);
            }
            else if (pw.Type == enmChannelType.RHYTHM)
            {
                n = Common.CheckRange(n, 0, 3);
                pw.pan.val = n;
            }
            else if (pw.Type == enmChannelType.ADPCMA || pw.Type == enmChannelType.ADPCMB)
            {
                int pl = Common.CheckRange(n, 0, 4);
                n = mml.args.Count < 2 ? 0 : (int)mml.args[1];
                int pr = Common.CheckRange(n, 0, 4);
                ((YM2609)pw.chip).SetAdpcmPan(mml, pw, pl, pr);
            }
            else if (pw.Type == enmChannelType.SSG)
            {
                n = Common.CheckRange(n, 0, 3);
                pw.pan.val = n;
            }
            SetDummyData(pw, mml);
        }

        public override void CmdInstrument(partWork pw, MML mml)
        {
            char type = (char)mml.args[0];
            int n = (type == 'W') ? -1 : (int)mml.args[1];

            if (type == 'n' || type == 'N' || type == 'R' || type == 'A')
            {
                if (pw.Type == enmChannelType.FMOPNex)
                {
                    pw.instrument = n;
                    if (pw.ch == 2 || pw.ch == 12 || pw.ch == 13 || pw.ch == 14)
                    {
                        lstPartWork[2].instrument = n;
                        lstPartWork[12].instrument = n;
                        lstPartWork[13].instrument = n;
                        lstPartWork[14].instrument = n;
                    }
                    else
                    {
                        lstPartWork[8].instrument = n;
                        lstPartWork[15].instrument = n;
                        lstPartWork[16].instrument = n;
                        lstPartWork[17].instrument = n;
                    }
                    OutFmSetInstrument(pw, mml, n, pw.volume, type);
                    return;
                }
            }

            if (type == 'n')
            {
                if (pw.Type == enmChannelType.ADPCMA || pw.Type == enmChannelType.ADPCMB)
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
                        pw.instrument = n;
                        SetADPCMAddress(
                            mml,
                            pw
                            , (int)parent.instPCM[n].stAdr
                            , (int)parent.instPCM[n].edAdr);
                    }
                    return;
                }

                if (pw.Type == enmChannelType.SSG)
                {
                    SetEnvelopParamFromInstrument(pw, n, mml);
                    return;
                }
            }

            if (type == 'I')
            {
                if (pw.Type == enmChannelType.SSG)
                {
                    n = Common.CheckRange(n, 0, 9);
                    pw.dutyCycle = n;
                    return;
                }

                msgBox.setErrMsg(msg.get("E11003"), mml.line.Lp);
                return;
            }

            if (type == 'T')
            {
                n = Common.CheckRange(n, 0, 255);
                pw.toneDoubler = n;
                return;
            }

            if (type == 'E')
            {
                SetEnvelopParamFromInstrument(pw, n, mml);
                return;
            }

            if (type == 'W')
            {
                SetWaveTableFromInstrument(pw, mml);
                return;
            }

            if (pw.Type == enmChannelType.SSG)
            {
                SetEnvelopParamFromInstrument(pw, n, mml);
                return;
            }

            n = Common.CheckRange(n, 0, 255);
            if (pw.instrument == n) return;
            pw.instrument = n;
            OutFmSetInstrument(pw, mml, n, pw.volume, type);
        }

        public void SetWaveTableFromInstrument(partWork pw, MML mml)
        {
            int p = 0;
            foreach(object o in mml.args)
            {
                p++;
                if (p == 1 || o == null)
                {
                    if (p == 5) break;
                    continue;
                }

                OutFmSetWaveTable(mml, pw, p - 2, o);

                if (p == 5) break;
            }

            SetDummyData(pw, mml);
        }

        public void OutFmSetWaveTable(MML mml, partWork pw, int ope, object prm)
        {
            int vch;
            byte[] port;
            GetPortVch(pw.chip.lstPartWork[0], out port, out vch);
            vch = pw.ch;
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

        public void OutFmSetWtLDtMl(MML mml, partWork pw, int ope,int wt, int dt, int ml)
        {
            int vch;
            byte[] port;
            GetPortVch(pw, out port, out vch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            wt &= 1;
            dt &= 7;
            ml &= 15;

            parent.OutData(mml, port, (byte)(0x30 + vch + ope * 4), (byte)((wt << 7) | (dt << 4) | ml));
        }

        public void OutFmSetWtHTl(MML mml, partWork pw, int ope,int wt, int tl)
        {
            int vch;
            byte[] port;
            GetPortVch(pw, out port, out vch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            wt &= 2;
            tl &= 0x7f;

            parent.OutData(mml, port, (byte)(0x40 + vch + ope * 4), (byte)((wt << 6) | tl));
        }

        public void OutFmSetAmDt2Dr(MML mml, partWork pw, int ope, int am,int dt2, int dr)
        {
            int vch;
            byte[] port;
            GetPortVch(pw, out port, out vch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            am &= 1;
            dt2 &= 3;
            dr &= 31;

            parent.OutData(mml, port, (byte)(0x60 + vch + ope * 4), (byte)((am << 7) | (dt2 << 5) | dr));
        }

        public void OutFmSetFbSr(MML mml, partWork pw, int ope,int fb, int sr)
        {
            int vch;
            byte[] port;
            GetPortVch(pw, out port, out vch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            fb &= 7;
            sr &= 31;

            parent.OutData(mml, port, (byte)(0x70 + vch + ope * 4), (byte)((fb << 5) | sr));
        }

        public void OutFmSetALGLinkSSGEG(MML mml, partWork pw, int ope,int all, int ssg)
        {
            int vch;
            byte[] port;
            GetPortVch(pw, out port, out vch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            all &= 15;
            ssg &= 15;

            parent.OutData(mml, port, (byte)(0x90 + vch + ope * 4), (byte)((all << 4) | ssg));
        }

        public new void OutFmSetInstrument(partWork pw, MML mml, int n, int vol, char typeBeforeSend)
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

            if (pw.ch >= 12 && pw.ch < 18)
            {
                msgBox.setWrnMsg(msg.get("E11002"), mml.line.Lp);
                return;
            }

            switch (modeBeforeSend)
            {
                case 0: // N)one
                    break;
                case 1: // R)R only
                    for (int ope = 0; ope < 4; ope++) ((ClsOPN)pw.chip).OutFmSetSlRr(mml, pw, ope, 0, 15);
                    break;
                case 2: // A)ll
                    for (int ope = 0; ope < 4; ope++)
                    {
                        OutFmSetWtLDtMl(mml, pw, ope, 0, 0, 0);
                        ((ClsOPN)pw.chip).OutFmSetKsAr(mml, pw, ope, 3, 31);
                        OutFmSetAmDt2Dr(mml, pw, ope, 1, 0, 31);
                        ((ClsOPN)pw.chip).OutFmSetSr(mml, pw, ope, 31);
                        ((ClsOPN)pw.chip).OutFmSetSlRr(mml, pw, ope, 0, 15);
                        ((ClsOPN)pw.chip).OutFmSetSSGEG(mml, pw, ope, 0);
                    }
                    pw.feedBack = 7;
                    pw.algo = 7;
                    OutFmSetPanRFeedbackAlgorithm(mml, pw);
                    break;
            }

            if (parent.instFM[n].Length == Const.OPNA2_INSTRUMENT_SIZE)
            {
                OutFmSetInstrumentOPNA(pw, mml, n, vol);
                return;
            }

            for (int ope = 0; ope < 4; ope++)
            {
                //ch3以外の拡張チャンネルでも音色設定できるようにする場合はslotの様子もみてセットすること
                OutFmSetWtLDtMl(mml, pw, ope
                    , 0
                    , parent.instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 1 + 8] // 8 : DT1
                    , parent.instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 1 + 7]); // 7 : ML
                ((ClsOPN)pw.chip).OutFmSetKsAr(mml, pw, ope, parent.instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 7], parent.instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 1]);
                OutFmSetAmDt2Dr(mml, pw, ope, parent.instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 10], 0, parent.instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 2]);
                ((ClsOPN)pw.chip).OutFmSetSr(mml, pw, ope, parent.instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 3]);
                ((ClsOPN)pw.chip).OutFmSetSlRr(mml, pw, ope, parent.instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 5], parent.instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 4]);
                ((ClsOPN)pw.chip).OutFmSetSSGEG(mml, pw, ope, parent.instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 11]);
            }

            //ch3以外の拡張チャンネルでも音色設定できるようにする場合はslotの様子もみてセットすること
            pw.op1ml = parent.instFM[n][0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
            pw.op2ml = parent.instFM[n][1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
            pw.op3ml = parent.instFM[n][2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
            pw.op4ml = parent.instFM[n][3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
            //ch3以外の拡張チャンネルでも音色設定できるようにする場合はslotの様子もみてセットすること
            pw.op1dt2 = 0;
            pw.op2dt2 = 0;
            pw.op3dt2 = 0;
            pw.op4dt2 = 0;

            pw.feedBack = parent.instFM[n][46];
            pw.algo = parent.instFM[n][45] & 0x7;
            OutFmSetPanRFeedbackAlgorithm(mml, pw);

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
                if (algs[pw.algo][i] == 0)// || (pw.slots & (1 << i)) == 0)
                {
                    op[i] = -1;
                    continue;
                }
                op[i] = Common.CheckRange(op[i], 0, 127);
            }

            partWork vpw = pw;
            if (pw.chip.lstPartWork[2].Ch3SpecialMode && pw.ch >= 12 && pw.ch < 15)
            {
                vpw = pw.chip.lstPartWork[2];
            }

            if (pw.chip.lstPartWork[8].Ch3SpecialMode && pw.ch >= 15 && pw.ch < 18)
            {
                vpw = pw.chip.lstPartWork[8];
            }

            //ch3以外の拡張チャンネルでも音色設定できるようになったら以下を有効に
            //if ((pw.slots & 1) != 0 && op[0] != -1) ((ClsOPN)pw.chip).OutFmSetTl(vpw, 0, op[0]);
            //if ((pw.slots & 2) != 0 && op[1] != -1) ((ClsOPN)pw.chip).OutFmSetTl(vpw, 1, op[1]);
            //if ((pw.slots & 4) != 0 && op[2] != -1) ((ClsOPN)pw.chip).OutFmSetTl(vpw, 2, op[2]);
            //if ((pw.slots & 8) != 0 && op[3] != -1) ((ClsOPN)pw.chip).OutFmSetTl(vpw, 3, op[3]);
            if (op[0] != -1) OutFmSetWtHTl(mml, vpw, 0, 0, op[0]);
            if (op[1] != -1) OutFmSetWtHTl(mml, vpw, 1, 0, op[1]);
            if (op[2] != -1) OutFmSetWtHTl(mml, vpw, 2, 0, op[2]);
            if (op[3] != -1) OutFmSetWtHTl(mml, vpw, 3, 0, op[3]);

            OutFmSetVolumeM(pw, mml, vol, n);

            //拡張チャンネルの場合は他の拡張チャンネルも音量を再セットする
            if (pw.Type == enmChannelType.FMOPNex)
            {
                if (pw.ch == 2 || pw.ch == 12 || pw.ch == 13 || pw.ch == 14)
                {
                    //YM2609 ch3 || ch13 || ch14 || ch15
                    if (pw.ch != 2) OutFmSetVolumeM(pw.chip.lstPartWork[2], mml, pw.chip.lstPartWork[2].volume, n);
                    if (pw.ch != 12) OutFmSetVolumeM(pw.chip.lstPartWork[12], mml, pw.chip.lstPartWork[12].volume, n);
                    if (pw.ch != 13) OutFmSetVolumeM(pw.chip.lstPartWork[13], mml, pw.chip.lstPartWork[13].volume, n);
                    if (pw.ch != 14) OutFmSetVolumeM(pw.chip.lstPartWork[14], mml, pw.chip.lstPartWork[14].volume, n);
                }
                else
                {
                    //YM2609 ch9 || ch16 || ch17 || ch18
                    if (pw.ch != 8) OutFmSetVolumeM(pw.chip.lstPartWork[8], mml, pw.chip.lstPartWork[8].volume, n);
                    if (pw.ch != 15) OutFmSetVolumeM(pw.chip.lstPartWork[15], mml, pw.chip.lstPartWork[15].volume, n);
                    if (pw.ch != 16) OutFmSetVolumeM(pw.chip.lstPartWork[16], mml, pw.chip.lstPartWork[16].volume, n);
                    if (pw.ch != 17) OutFmSetVolumeM(pw.chip.lstPartWork[17], mml, pw.chip.lstPartWork[17].volume, n);
                }
            }
        }

        private void OutFmSetInstrumentOPNA(partWork pw, MML mml, int n, int vol)
        {
            for (int ope = 0; ope < 4; ope++)
            {
                //ch3以外の拡張チャンネルでも音色設定できるようにする場合はslotの様子もみてセットすること
                OutFmSetWtLDtMl(mml, pw, ope
                    , parent.instFM[n][ope * 15 + 1 + 13] // 13 : WT  1 : No  15 : OPE Size
                    , parent.instFM[n][ope * 15 + 1 + 8] // 8 : DT1
                    , parent.instFM[n][ope * 15 + 1 + 7]); // 7 : ML

                ((ClsOPN)pw.chip).OutFmSetKsAr(mml, pw, ope
                    , parent.instFM[n][ope * 15 + 1 + 6] // 6 : KS
                    , parent.instFM[n][ope * 15 + 1 + 0] // 0 : AR
                    );
                OutFmSetAmDt2Dr(mml, pw, ope
                    , parent.instFM[n][ope * 15 + 1 + 10] // 10 : AM
                    , parent.instFM[n][ope * 15 + 1 + 9] // 9 : DT2
                    , parent.instFM[n][ope * 15 + 1 + 1] // 1 : DR
                    );
                OutFmSetFbSr(mml, pw, ope
                    , parent.instFM[n][ope * 15 + 1 + 12] // 12 : FB
                    , parent.instFM[n][ope * 15 + 1 + 2] //2 : SR
                    );
                ((ClsOPN)pw.chip).OutFmSetSlRr(mml, pw, ope
                    , parent.instFM[n][ope * 15 + 1 + 4] // 4 : SL
                    , parent.instFM[n][ope * 15 + 1 + 3] // 3 : RR
                    );
                OutFmSetALGLinkSSGEG(mml, pw, ope
                    , parent.instFM[n][ope * 15 + 1 + 14] // 14 : ALG Link
                    , parent.instFM[n][ope * 15 + 1 + 11] // 11 : SSG-EG
                    );
            }

            //ch3以外の拡張チャンネルでも音色設定できるようにする場合はslotの様子もみてセットすること
            pw.op1ml = parent.instFM[n][0 * 15 + 1 + 7];// 7 : ML
            pw.op2ml = parent.instFM[n][1 * 15 + 1 + 7];// 7 : ML
            pw.op3ml = parent.instFM[n][2 * 15 + 1 + 7];// 7 : ML
            pw.op4ml = parent.instFM[n][3 * 15 + 1 + 7];// 7 : ML
            //ch3以外の拡張チャンネルでも音色設定できるようにする場合はslotの様子もみてセットすること
            pw.op1dt2 = parent.instFM[n][0 * 15 + 1 + 9];// 9 : DT2
            pw.op2dt2 = parent.instFM[n][1 * 15 + 1 + 9];// 9 : DT2
            pw.op3dt2 = parent.instFM[n][2 * 15 + 1 + 9];// 9 : DT2
            pw.op4dt2 = parent.instFM[n][3 * 15 + 1 + 9];// 9 : DT2

            pw.feedBack = parent.instFM[n][1 + 12] & 7;
            pw.algo = parent.instFM[n][61] == 0xff ? 8 : (parent.instFM[n][61] & 0x7);
            OutFmSetPanRFeedbackAlgorithm(mml, pw);

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

            if (pw.algo == 8)
            {
                algs = GetVolumeOpe(algs, parent.instFM[n], true);
            }

            for (int i = 0; i < 4; i++)
            {
                //ch3以外の拡張チャンネルでも音色設定できるようになったら以下を有効に
                if (algs[pw.algo][i] == 0)// || (pw.slots & (1 << i)) == 0)
                {
                    op[i] = -1;
                    continue;
                }
                op[i] = Common.CheckRange(op[i], 0, 127);
            }

            partWork vpw = pw;
            if (pw.chip.lstPartWork[2].Ch3SpecialMode && pw.ch >= 12 && pw.ch < 15)
            {
                vpw = pw.chip.lstPartWork[2];
            }

            if (pw.chip.lstPartWork[8].Ch3SpecialMode && pw.ch >= 15 && pw.ch < 18)
            {
                vpw = pw.chip.lstPartWork[8];
            }

            if (op[0] != -1) OutFmSetWtHTl(mml, vpw, 0, 0, op[0]);
            if (op[1] != -1) OutFmSetWtHTl(mml, vpw, 1, 0, op[1]);
            if (op[2] != -1) OutFmSetWtHTl(mml, vpw, 2, 0, op[2]);
            if (op[3] != -1) OutFmSetWtHTl(mml, vpw, 3, 0, op[3]);

            OutFmSetVolumeM(pw, mml, vol, n);

        }

        private int[][] GetVolumeOpe(int[][] algs, byte[] inst,bool reverse)
        {
            byte[] a = new byte[]{
                    inst[0 * 15 + 1 + 14]// 14 : ALG Link
                    ,inst[1 * 15 + 1 + 14]// 14 : ALG Link
                    ,inst[2 * 15 + 1 + 14]// 14 : ALG Link
                    ,inst[3 * 15 + 1 + 14]// 14 : ALG Link
                };

            for (int ope = 0; ope < 4; ope++)
            {
                for(int i = 0; i < 4; i++)
                    if ((a[i] & (1 << i)) != 0) a[i] = 0; //自分が設定されている場合はキャリア
                
                algs[8][ope] = (((a[0] | a[1] | a[2] | a[3]) & (1 << ope)) == 0) ? 1 : 0;//自分を親とするopeが一つもないかチェック
                if (reverse) algs[8][ope] = algs[8][ope] == 0 ? 1 : 0;//リバース指定の場合は数値を入れ替える
            }

            return algs;
        }

        public void OutFmSetVolumeM(partWork pw, MML mml, int vol, int n)
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
                if (algs[alg][i] == 0 || (pw.slots & (1 << i)) == 0)
                {
                    ope[i] = -1;
                    continue;
                }
                ope[i] = ope[i] + (127 - vol);
                ope[i] = Common.CheckRange(ope[i], 0, 127);
            }

            partWork vpw = pw;
            if (pw.chip.lstPartWork[2].Ch3SpecialMode && pw.ch >= 12 && pw.ch < 15)
            {
                vpw = pw.chip.lstPartWork[2];
            }
            if (pw.chip.lstPartWork[8].Ch3SpecialMode && pw.ch >= 15 && pw.ch < 18)
            {
                vpw = pw.chip.lstPartWork[8];
            }

            MML vmml = new MML();
            vmml.args = new List<object>();
            vmml.args.Add(vol);
            vmml.type = enmMMLType.Volume;
            if (mml != null)
                vmml.line = mml.line;
            if ((pw.slots & 1) != 0 && ope[0] != -1) OutFmSetWtHTl(vmml, vpw, 0, 0, ope[0]);
            if ((pw.slots & 2) != 0 && ope[1] != -1) OutFmSetWtHTl(vmml, vpw, 1, 0, ope[1]);
            if ((pw.slots & 4) != 0 && ope[2] != -1) OutFmSetWtHTl(vmml, vpw, 2, 0, ope[2]);
            if ((pw.slots & 8) != 0 && ope[3] != -1) OutFmSetWtHTl(vmml, vpw, 3, 0, ope[3]);
            //if ((pw.slots & 1) != 0 ) ((ClsOPN)pw.chip).OutFmSetTl(vpw, 0, ope[0]);
            //if ((pw.slots & 2) != 0 ) ((ClsOPN)pw.chip).OutFmSetTl(vpw, 1, ope[1]);
            //if ((pw.slots & 4) != 0 ) ((ClsOPN)pw.chip).OutFmSetTl(vpw, 2, ope[2]);
            //if ((pw.slots & 8) != 0 ) ((ClsOPN)pw.chip).OutFmSetTl(vpw, 3, ope[3]);
        }

        public override void MultiChannelCommand(MML mml)
        {
            if (!use) return;
            //コマンドを跨ぐデータ向け処理
            foreach (partWork pw in lstPartWork)
            {
                if (pw.Type == enmChannelType.RHYTHM)
                {
                    //Rhythm Volume処理
                    if (pw.beforeVolume != pw.volume || !pw.pan.eq())
                    {
                        parent.OutData(mml, port[0], (byte)(0x18 + (pw.ch - 30)), (byte)((byte)((pw.pan.val & 0x3) << 6) | (byte)(pw.volume & 0x1f)));
                        pw.beforeVolume = pw.volume;
                        pw.pan.rst();
                    }

                    rhythm_KeyOn |= (byte)(pw.keyOn ? (1 << (pw.ch - 30)) : 0);
                    pw.keyOn = false;
                    rhythm_KeyOff |= (byte)(pw.keyOff ? (1 << (pw.ch - 30)) : 0);
                    pw.keyOff = false;
                }
                else if (pw.Type == enmChannelType.SSG)
                {
                    MultiChannelCommand_SSG(mml, pw);
                }
                else if (pw.Type == enmChannelType.FMOPN || pw.Type == enmChannelType.FMOPNex)
                {
                    MultiChannelCommand_FM(mml, pw);
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

            log.Write("KeyOn情報をかき出し");
            int vch;
            byte[] p;
            GetPortVch(lstPartWork[0], out p, out vch);
            foreach (outDatum dat in opna20x028KeyOnData) parent.OutData(dat, p, 0x28, dat.val);
            GetPortVch(lstPartWork[6], out p, out vch);
            foreach (outDatum dat in opna20x228KeyOnData) parent.OutData(dat, p, 0x28, dat.val);
            opna20x028KeyOnData.Clear();
            opna20x228KeyOnData.Clear();
        }

        public List<outDatum> opna20x028KeyOnData = new List<outDatum>();
        public List<outDatum> opna20x228KeyOnData = new List<outDatum>();

        private void MultiChannelCommand_SSG(MML mml, partWork pw)
        {
            int port;
            int adr;
            int vch;
            GetPortVchSsg(pw, out port, out adr, out vch);

            //reg 00 - 05
            if (pw.freq != pw.oldFreq || pw.dutyCycle != pw.oldDutyCycle)
            {
                pw.oldFreq = pw.freq;
                pw.oldDutyCycle = pw.dutyCycle;

                byte data = (byte)(pw.freq & 0xff);
                parent.OutData(mml, pw.port[port], (byte)(adr + 0 + vch * 2), data);

                data = (byte)(((pw.freq & 0xf00) >> 8) | ((pw.dutyCycle & 0xf) << 4));
                parent.OutData(mml, pw.port[port], (byte)(adr + 1 + vch * 2), data);
            }

        }

        private void MultiChannelCommand_FM(MML mml, partWork pw)
        {
            //FNum l
            //panL Block FNum h
            if (pw.panL != pw.beforePanL || pw.freq != pw.oldFreq)
                OutFmSetPanLFnum(pw, mml);

            if (pw.ch < 12)
            {
                if (pw.panR != pw.beforePanR || pw.feedBack != pw.beforeFeedBack || pw.algo != pw.beforeAlgo)
                    OutFmSetPanRFeedbackAlgorithm(mml, pw);
                if (pw.pan.val != pw.beforePan.val || pw.ams != pw.beforeAms || pw.algConstSw != pw.beforeAlgConstSw || pw.pms != pw.beforePms)
                    OutOPNSetPanAmsAcPms(mml, pw);
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

        public override void CmdLoopExtProc(partWork pw, MML mml)
        {
            if (pw.Type == enmChannelType.FMOPN || pw.Type == enmChannelType.FMOPNex)
            {
                pw.pan.val = (pw.panL != 0 ? 2 : 0) | (pw.panR != 0 ? 1 : 0);
            }
        }

    }
}
