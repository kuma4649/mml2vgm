using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mml2vgm
{
    public class segaPcm : clsChip
    {
        public byte[] pcmData = null;
        public int Interface = 0;

        public segaPcm(int chipID, string initialPartName) : base(chipID, initialPartName)
        {

            _Name = "SEGAPCM";
            _ShortName = "SPCM";
            _ChMax = 16;
            _canUsePcm = true;

            Frequency = 4026987;
            Interface = 0x00f8000d;
            Ch = new clsChannel[ChMax];
            setPartToCh(Ch, initialPartName);
            foreach (clsChannel ch in Ch)
            {
                ch.Type = enmChannelType.PCM;
                ch.isSecondary = chipID == 1;
            }

        }
    }
}
