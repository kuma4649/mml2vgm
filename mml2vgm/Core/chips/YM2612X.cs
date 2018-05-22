using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class YM2612X : ClsChip
    {
        protected int[][] _FNumTbl = new int[1][] {
            new int[13]
        };

        public byte[] pcmData = null;

        public YM2612X(ClsVgm parent, int chipID, string initialPartName, string stPath) : base(parent, chipID, initialPartName, stPath)
        {

            _Name = "YM2612X";
            _ShortName = "OPN2X";
            _ChMax = 12;
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
            Ch[5].Type = enmChannelType.FMPCMex;
            Ch[6].Type = enmChannelType.FMOPNex;
            Ch[7].Type = enmChannelType.FMOPNex;
            Ch[8].Type = enmChannelType.FMOPNex;
            Ch[9].Type = enmChannelType.FMPCMex;
            Ch[10].Type = enmChannelType.FMPCMex;
            Ch[11].Type = enmChannelType.FMPCMex;

        }

        public override void InitPart(ref partWork pw)
        {
            pw.slots = (byte)((pw.Type == enmChannelType.FMOPN || pw.ch == 2 || pw.ch == 5) ? 0xf : 0x0);
            pw.volume = 127;
            pw.MaxVolume = 127;
            pw.port0 = (byte)(pw.isSecondary ? 0xa2 : 0x52);
            pw.port1 = (byte)(pw.isSecondary ? 0xa3 : 0x53);
            pw.pcm = pw.ch > 9;
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
