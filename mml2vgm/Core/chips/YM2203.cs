using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using musicDriverInterface;

namespace Core
{
    public class YM2203 : ClsOPN
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

        public YM2203(ClsVgm parent,int chipID, string initialPartName, string stPath, int chipNumber) : base(parent, chipID, initialPartName, stPath, chipNumber)
        {

            _Name = "YM2203";
            _ShortName = "OPN";
            _ChMax = 9;
            _canUsePcm = false;
            _canUsePI = false;
            FNumTbl = _FNumTbl;

            Frequency = 3993600;// 7987200/2;
            port =new byte[][] { new byte[] { (byte)(chipNumber!=0 ? 0xa5 : 0x55) } };

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
            }

            Ch = new ClsChannel[ChMax];
            SetPartToCh(Ch, initialPartName);
            foreach (ClsChannel ch in Ch)
            {
                ch.Type = enmChannelType.FMOPN;
                ch.chipNumber = chipID == 1;
            }

            Ch[2].Type = enmChannelType.FMOPNex;
            Ch[3].Type = enmChannelType.FMOPNex;
            Ch[4].Type = enmChannelType.FMOPNex;
            Ch[5].Type = enmChannelType.FMOPNex;

            Ch[6].Type = enmChannelType.SSG;
            Ch[7].Type = enmChannelType.SSG;
            Ch[8].Type = enmChannelType.SSG;

            Envelope = new Function();
            Envelope.Max = 255;
            Envelope.Min = 0;

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

                page.port = port;
            }
        }

        public override void InitChip()
        {

            if (!use) return;

            //initialize shared param
            OutOPNSetHardLfo(null, lstPartWork[0].cpg, false, 0);
            OutOPNSetCh3SpecialMode(null, lstPartWork[0].cpg, false);

            //FM Off
            outYM2203AllKeyOff(this);

            //SSG Off
            for (int ch = 6; ch < 9; ch++)
            {
                outYM2203SsgKeyOff(null, lstPartWork[ch].cpg);
                foreach (partPage page in lstPartWork[ch].pg)
                {
                    page.volume = 0;
                }
            }

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
                }
            }

            if (ChipID != 0 && parent.info.format != enmFormat.ZGM)
            {
                parent.dat[0x47] = new outDatum(enmMMLType.unknown, null, null, (byte)(parent.dat[0x47].val | 0x40));//use Secondary
            }

        }


        public void outYM2203AllKeyOff(ClsChip chip)
        {
            if (chip == null) return;

            foreach (partWork pw in chip.lstPartWork)
            {
                foreach (partPage page in pw.pg)
                {
                    if (page.dataEnd) continue;
                    if (page.ch > 2) continue;

                    OutFmKeyOff(page, null);
                    OutFmSetTl(null, page, 0, 127);
                    OutFmSetTl(null, page, 1, 127);
                    OutFmSetTl(null, page, 2, 127);
                    OutFmSetTl(null, page, 3, 127);
                }
            }

        }

        public void outYM2203SsgKeyOff(MML mml,partPage page)
        {
            byte pch = (byte)(page.ch - 6);
            int n = 9;
            byte data = 0;

            data = (byte)(((YM2203)page.chip).SSGKeyOn[0] | (n << pch));
            ((YM2203)page.chip).SSGKeyOn[0] = data;

            SOutData(page,mml,port[0], (byte)(0x08 + pch), 0);
            page.beforeVolume = -1;
            SOutData(page,mml,port[0], 0x07, data);
        }

        public override void SetFNum(partPage page, MML mml)
        {
            if (page.Type != enmChannelType.SSG)
                SetFmFNum(page, mml);
            else if (page.Type == enmChannelType.SSG)
            {
                SetSsgFNum(page, mml);
            }
        }

        public override void SetKeyOn(partPage page, MML mml)
        {
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

        public override void SetKeyOff(partPage page, MML mml)
        {
            if (page.Type != enmChannelType.SSG)
                OutFmKeyOff(page, mml);
            else
                OutSsgKeyOff(mml, page);
        }


        public override void CmdY(partPage page, MML mml)
        {
            base.CmdY(page, mml);

            if (mml.args[0] is string) return;

            byte adr = (byte)(int)mml.args[0];
            byte dat = (byte)(int)mml.args[1];

            SOutData(page,mml,port[0], adr, dat);
        }

        public override void CmdInstrument(partPage page, MML mml)
        {
            char type = (char)mml.args[0];
            int n = (int)mml.args[1];

            if (type == 'n' || type == 'N' || type == 'R' || type == 'A')
            {
                if (page.Type == enmChannelType.FMOPNex)
                {
                    page.instrument = n;
                    lstPartWork[2].cpg.instrument = n;
                    lstPartWork[3].cpg.instrument = n;
                    lstPartWork[4].cpg.instrument = n;
                    lstPartWork[5].cpg.instrument = n;
                    OutFmSetInstrument(page, mml, n, page.volume, type);
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
            }

            //周波数
            page.spg.freq = -1;
            SetFNum(page, null);

            if (page.Type == enmChannelType.SSG)
            {
                //ノイズ周波数
                noiseFreq = -1;
                OutSsgNoise(null, page);

                //ハードエンベロープtype
                page.spg.HardEnvelopeType = -1;
                OutSsgHardEnvType(page, null);

                //ハードエンベロープspeed
                page.spg.HardEnvelopeSpeed = -1;
                OutSsgHardEnvSpeed(page, null);
            }

            //音量
            page.spg.beforeVolume = -1;
            SetVolume(page, null);

        }

        public override void MultiChannelCommand(MML mml)
        {
            if (!use) return;
            //int dat = 0;

            foreach (partWork pw in lstPartWork)
            {
                partPage page = pw.cpg;

                if (page.keyOn)
                {
                    page.keyOn = false;
                    OutFmKeyOn(page, mml);
                }

            }


        }

    }
}
