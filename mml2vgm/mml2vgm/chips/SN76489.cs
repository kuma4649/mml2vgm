using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mml2vgm
{
    public class SN76489 : clsChip
    {

        public SN76489(int chipID, string initialPartName) : base(chipID, initialPartName)
        {
            _Name = "SN76489";
            _ShortName = "DCSG";
            _ChMax = 4;

            Frequency = 3579545;
            Ch = new clsChannel[ChMax];
            setPartToCh(Ch,initialPartName);
                foreach (clsChannel ch in Ch)
                {
                    ch.Type = enmChannelType.DCSG;
                    ch.isSecondary = chipID == 1;
                }
                Ch[3].Type = enmChannelType.DCSGNOISE;
        }
    }
}
