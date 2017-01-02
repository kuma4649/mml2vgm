using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mml2vgm
{
    public class YM2203 : clsChip
    {
        public byte SSGKeyOn = 0x3f;

        public YM2203(int chipID, string initialPartName) : base(chipID, initialPartName)
        {

            _Name = "YM2203";
            _ShortName = "OPN";
            _ChMax = 9;
            _canUsePcm = false;

            Frequency = 3993600;// 7987200;
            makeFNumTbl();
            Ch = new clsChannel[ChMax];
            setPartToCh(Ch, initialPartName);
            foreach (clsChannel ch in Ch)
            {
                ch.Type = enmChannelType.FMOPN;
                ch.isSecondary = chipID == 1;
            }

            Ch[2].Type = enmChannelType.FMOPNex;
            Ch[3].Type = enmChannelType.FMOPNex;
            Ch[4].Type = enmChannelType.FMOPNex;
            Ch[5].Type = enmChannelType.FMOPNex;

            Ch[6].Type = enmChannelType.SSG;
            Ch[7].Type = enmChannelType.SSG;
            Ch[8].Type = enmChannelType.SSG;

        }

        new protected void makeFNumTbl()
        {
            for (int i = 0; i < noteTbl.Length; i++)
            {
                OPN_FNumTbl[i] = (int)(Math.Round(((144.0 * noteTbl[i] * Math.Pow(2.0, 19) / Frequency) / Math.Pow(2.0, (4 - 1))), MidpointRounding.AwayFromZero));
            }
            OPN_FNumTbl[12] = OPN_FNumTbl[0] * 2;
        }

    }
}
