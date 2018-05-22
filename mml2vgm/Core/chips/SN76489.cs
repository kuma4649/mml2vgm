using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class SN76489 : ClsChip
    {

        protected int[][] _FNumTbl = new int[1][] {
            new int[96] 
        };


        public SN76489(ClsVgm parent, int chipID, string initialPartName, string stPath) : base(parent, chipID, initialPartName, stPath)
        {
            _Name = "SN76489";
            _ShortName = "DCSG";
            _ChMax = 4;
            _canUsePcm = false;
            FNumTbl = _FNumTbl;

            Frequency = 3579545;

            Dictionary<string, List<double>> dic = MakeFNumTbl();
            if (dic != null)
            {
                int c = 0;
                foreach (double v in dic["FNUM_00"])
                {
                    FNumTbl[0][c++] = (int)v;
                    if (c == FNumTbl[0].Length) break;
                }
            }

            Ch = new ClsChannel[ChMax];
            SetPartToCh(Ch, initialPartName);
            foreach (ClsChannel ch in Ch)
            {
                ch.Type = enmChannelType.DCSG;
                ch.isSecondary = chipID == 1;
            }
            Ch[3].Type = enmChannelType.DCSGNOISE;
        }

        public override void InitPart(ref partWork pw)
        {
            pw.MaxVolume = 15;
            pw.volume = pw.MaxVolume;
            pw.port0 = 0x50;
        }



    }
}
