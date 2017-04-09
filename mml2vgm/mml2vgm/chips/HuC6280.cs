using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mml2vgm
{
    public class HuC6280 : clsChip
    {
        public byte[] pcmData = null;
        public byte CurrentChannel = 0xff;
        public int TotalVolume = 15;
        public int MAXTotalVolume = 15;

        public HuC6280(int chipID, string initialPartName) : base(chipID, initialPartName)
        {

            _Name = "HuC6280";
            _ShortName = "HuC8";
            _ChMax = 6;
            _canUsePcm = true;

            Frequency = 3579545;

            Ch = new clsChannel[ChMax];
            setPartToCh(Ch, initialPartName);
            foreach (clsChannel ch in Ch)
            {
                ch.Type = enmChannelType.WaveForm;
                ch.isSecondary = chipID == 1;
                ch.MaxVolume = 15;
            }

        }

    }
}
