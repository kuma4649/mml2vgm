using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mml2vgm
{
    public class YM2612X : clsChip
    {
        protected int[][] _FNumTbl = new int[1][] {
            new int[13]
        };

        public byte[] pcmData = null;

        public YM2612X(int chipID, string initialPartName) : base(chipID, initialPartName)
        {

            _Name = "YM2612X";
            _ShortName = "OPN2X";
            _ChMax = 12;
            _canUsePcm = true;
            FNumTbl = _FNumTbl;

            Frequency = 7670454;

            Dictionary<string, List<double>> dic = makeFNumTbl();
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

            Ch = new clsChannel[ChMax];
            setPartToCh(Ch, initialPartName);
            foreach (clsChannel ch in Ch)
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
    }
}
