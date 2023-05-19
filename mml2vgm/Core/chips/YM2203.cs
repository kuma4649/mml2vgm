using musicDriverInterface;
using System;
using System.Collections.Generic;

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

        public YM2203(ClsVgm parent, int chipID, string initialPartName, string stPath, int chipNumber) : base(parent, chipID, initialPartName, stPath, chipNumber)
        {

            _Name = "YM2203";
            _ShortName = "OPN";
            _ChMax = 9;
            _canUsePcm = true;
            _canUsePI = false;
            FNumTbl = _FNumTbl;

            Frequency = 3993600;// 7987200/2;
            port = new byte[][] { new byte[] { (byte)(chipNumber != 0 ? 0xa5 : 0x55) } };
            DataBankID = 0x0c;//TBD(固定値ではなく、恐らくデータごとに連番を振るのが良いと思われる。)

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
            Ch[3].Type = enmChannelType.FMOPNex;
            Ch[4].Type = enmChannelType.FMOPNex;
            Ch[5].Type = enmChannelType.FMOPNex;

            Ch[6].Type = enmChannelType.SSG;
            Ch[7].Type = enmChannelType.SSG;
            Ch[8].Type = enmChannelType.SSG;

            Envelope = new Function();
            Envelope.Max = 255;
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
                page.noise = 0;
                page.noiseFreq = -1;
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
                    page.mixer = 1;
                }
            }

            //SSG vol0
            parent.OutData((MML)null, port[0], 0x08, 0x00);
            parent.OutData((MML)null, port[0], 0x09, 0x00);
            parent.OutData((MML)null, port[0], 0x0a, 0x00);
            //SSG Mixer init
            parent.OutData((MML)null, port[0], 0x07, 0x38);

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


        public override void StorePcm(Dictionary<int, Tuple<string, clsPcm>> newDic, KeyValuePair<int, clsPcm> v, byte[] buf, bool is16bit, int samplerate, params object[] option)
        {
            clsPcmDataInfo pi = pcmDataInfo[0];

            try
            {
                EncAdpcmA ea = new EncAdpcmA();
                buf = SSGPCM_Encode(buf, is16bit);

                long size = buf.Length;
                byte[] newBuf;
                newBuf = new byte[size];
                Array.Copy(buf, newBuf, size);
                buf = newBuf;
                long tSize = size;
                size = buf.Length;

                newDic.Add(
                    v.Key
                    , new Tuple<string, clsPcm>("", new clsPcm(
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
                pcmDataEasy = pi.use ? pi.totalBuf : null;

            }
            catch
            {
                pi.use = false;
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

        public void outYM2203SsgKeyOff(MML mml, partPage page)
        {
            byte pch = (byte)(page.ch - 6);
            int n = 9;
            byte data = 0;

            data = (byte)(((YM2203)page.chip).SSGKeyOn[0] | (n << pch));
            ((YM2203)page.chip).SSGKeyOn[0] = data;

            SOutData(page, mml, port[0], (byte)(0x08 + pch), 0);
            page.beforeVolume = -1;
            SOutData(page, mml, port[0], 0x07, data);
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

            SOutData(page, mml, port[0], adr, dat);
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
            base.MultiChannelCommand(mml);

            foreach (partWork pw in lstPartWork)
            {
                partPage page = pw.cpg;

                if (page.Type == enmChannelType.SSG) continue;

                if (page.keyOn)
                {
                    page.keyOn = false;
                    OutFmKeyOn(page, mml);
                }

            }


        }

    }
}
