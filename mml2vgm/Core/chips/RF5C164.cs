using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class RF5C164 : ClsChip
    {
         
        public byte KeyOn = 0x0;
        public byte CurrentChannel = 0xff;
        public byte[] pcmData = null;

        public RF5C164(ClsVgm parent, int chipID, string initialPartName, string stPath) : base(parent, chipID, initialPartName, stPath)
        {
            _Name = "RF5C164";
            _ShortName = "RF5C";
            _ChMax = 8;
            _canUsePcm = true;

            Frequency = 12500000;
            Ch = new ClsChannel[ChMax];
            SetPartToCh(Ch, initialPartName);
            foreach (ClsChannel ch in Ch)
            {
                ch.Type = enmChannelType.PCM;
                ch.isSecondary = chipID == 1;
            }
        }

        public override void InitPart(ref partWork pw)
        {
            pw.MaxVolume = 255;
            pw.volume = pw.MaxVolume;
            pw.port0 = 0xb1;
        }

    }
}
