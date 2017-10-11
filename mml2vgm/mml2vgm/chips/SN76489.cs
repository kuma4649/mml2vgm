using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mml2vgm
{
    public class SN76489 : clsChip
    {

        protected int[][] _FNumTbl = new int[1][] {
            new int[96] 
        };


        public SN76489(int chipID, string initialPartName) : base(chipID, initialPartName)
        {
            _Name = "SN76489";
            _ShortName = "DCSG";
            _ChMax = 4;
            _canUsePcm = false;
            FNumTbl = _FNumTbl;

            Frequency = 3579545;

            Dictionary<string, List<double>> dic = makeFNumTbl();
            if (dic != null)
            {
                int c = 0;
                foreach (double v in dic["FNUM_00"])
                {
                    FNumTbl[0][c++] = (int)v;
                    if (c == FNumTbl[0].Length) break;
                }
            }

            Ch = new clsChannel[ChMax];
            setPartToCh(Ch, initialPartName);
            foreach (clsChannel ch in Ch)
            {
                ch.Type = enmChannelType.DCSG;
                ch.isSecondary = chipID == 1;
            }
            Ch[3].Type = enmChannelType.DCSGNOISE;
        }
    }
}
