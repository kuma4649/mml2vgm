using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class segaPcm : ClsChip
    {
        public byte[] pcmData = null;
        public int Interface = 0;

        public segaPcm(ClsVgm parent, int chipID, string initialPartName, string stPath) : base(parent, chipID, initialPartName, stPath)
        {

            _Name = "SEGAPCM";
            _ShortName = "SPCM";
            _ChMax = 16;
            _canUsePcm = true;

            Frequency = 4026987;
            Interface = 0x00f8000d;

            Ch = new ClsChannel[ChMax];
            SetPartToCh(Ch, initialPartName);
            foreach (ClsChannel ch in Ch)
            {
                ch.Type = enmChannelType.PCM;
                ch.isSecondary = chipID == 1;
                ch.MaxVolume = 127;
            }

        }

        public override void InitPart(ref partWork pw)
        {
            pw.MaxVolume = 255;
            pw.volume = pw.MaxVolume;
            pw.port0 = 0xc0;
        }

    }
}
