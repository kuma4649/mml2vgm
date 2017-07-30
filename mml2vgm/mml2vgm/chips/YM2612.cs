using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mml2vgm
{
    public class YM2612 : clsChip
    {
        protected int[][] _FNumTbl = new int[1][] {
            new int[] {
            //   c    c+     d    d+     e     f    f+     g    g+     a    a+     b    >c
             0x289,0x2af,0x2d8,0x303,0x331,0x362,0x395,0x3cc,0x405,0x443,0x484,0x4c8,0x289*2
            }
        };


        public byte[] pcmData = null;

        public YM2612(int chipID, string initialPartName) : base(chipID, initialPartName)
        {

            _Name = "YM2612";
            _ShortName = "OPN2";
            _ChMax = 9;
            _canUsePcm = true;
            FNumTbl = _FNumTbl;

            Frequency = 7670454;
            Ch = new clsChannel[ChMax];
            setPartToCh(Ch, initialPartName);
            foreach (clsChannel ch in Ch)
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
    }
}
