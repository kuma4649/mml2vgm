using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class YM2612 : ClsChip
    {
        protected int[][] _FNumTbl = new int[1][] {
            new int[13]
            //{
            ////   c    c+     d    d+     e     f    f+     g    g+     a    a+     b    >c
            // 0x289,0x2af,0x2d8,0x303,0x331,0x362,0x395,0x3cc,0x405,0x443,0x484,0x4c8,0x289*2
            //}
        };


        public byte[] pcmData = null;

        public YM2612(ClsVgm parent, int chipID, string initialPartName, string stPath) : base(parent, chipID, initialPartName, stPath)
        {

            _Name = "YM2612";
            _ShortName = "OPN2";
            _ChMax = 9;
            _canUsePcm = true;
            FNumTbl = _FNumTbl;

            Frequency = 7670454;

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
            }

            Ch = new ClsChannel[ChMax];
            SetPartToCh(Ch, initialPartName);
            foreach (ClsChannel ch in Ch)
            {
                ch.Type = enmChannelType.FMOPN;
                ch.isSecondary = chipID == 1;
            }

            Ch[2].Type = enmChannelType.FMOPNex;
            Ch[5].Type = enmChannelType.FMPCM;
            Ch[6].Type = enmChannelType.FMOPNex;
            Ch[7].Type = enmChannelType.FMOPNex;
            Ch[8].Type = enmChannelType.FMOPNex;

        }

        public override void InitPart(ref partWork pw)
        {
            pw.slots = (byte)(((pw.Type == enmChannelType.FMOPN || pw.Type == enmChannelType.FMPCM) || pw.ch == 2) ? 0xf : 0x0);
            pw.volume = 127;
            pw.MaxVolume = 127;
            pw.port0 = (byte)(0x2 | (pw.isSecondary ? 0xa0 : 0x50));
            pw.port1 = (byte)(0x3 | (pw.isSecondary ? 0xa0 : 0x50));
        }

        public override void InitChip()
        {
            if (use)
            {
                parent.OutOPNSetHardLfo(lstPartWork[0], false, 0);
                parent.OutOPNSetCh3SpecialMode(lstPartWork[0], false);
                OutSetCh6PCMMode(lstPartWork[0], false);

                parent.OutFmAllKeyOff(this);

                foreach (partWork pw in lstPartWork)
                {
                    if (pw.ch == 0)
                    {
                        pw.hardLfoSw = false;
                        pw.hardLfoNum = 0;
                        parent.OutOPNSetHardLfo(pw, pw.hardLfoSw, pw.hardLfoNum);
                    }

                    if (pw.ch < 6)
                    {
                        pw.pan.val = 3;
                        pw.ams = 0;
                        pw.fms = 0;
                        if (!pw.dataEnd) parent.OutOPNSetPanAMSPMS(pw, 3, 0, 0);
                    }
                }

                if (ChipID != 0) parent.dat[0x2f] |= 0x40;
            }

        }

        public void OutSetCh6PCMMode(partWork pw, bool sw)
        {
            parent.dat.Add(pw.port0);
            parent.dat.Add(0x2b);
            parent.dat.Add((byte)((sw ? 0x80 : 0)));
        }

        public void SetF_NumTbl(string val)
        {
            //厳密なチェックを行っていないので設定値によってはバグる危険有り

            string[] s = val.Split(new string[] { ",", " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < s.Length; i++)
            {
                FNumTbl[0][i] = int.Parse(s[i], System.Globalization.NumberStyles.HexNumber);
            }
            FNumTbl[0][12] = FNumTbl[0][0] * 2;
        }

    }
}
