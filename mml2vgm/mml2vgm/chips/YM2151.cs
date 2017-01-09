using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mml2vgm
{
    public class YM2151 : clsChip
    {
        public YM2151(int chipID, string initialPartName) : base(chipID, initialPartName)
        {

            _Name = "YM2151";
            _ShortName = "OPM";
            _ChMax = 8;
            _canUsePcm = false;

            Frequency = 3579545;

            makeFNumTbl();
            Ch = new clsChannel[ChMax];
            setPartToCh(Ch, initialPartName);
            foreach (clsChannel ch in Ch)
            {
                ch.Type = enmChannelType.FMOPM;
                ch.isSecondary = chipID == 1;
            }

        }
    }
}
