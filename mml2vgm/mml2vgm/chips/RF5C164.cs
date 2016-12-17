using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mml2vgm
{
    public class RF5C164 : clsChip
    {
         
        public byte KeyOn = 0x0;
        public byte CurrentChannel = 0xff;
        public byte[] pcmData = null;

        public RF5C164(int chipID, string initialPartName) : base(chipID, initialPartName)
        {
            _Name = "RF5C164";
            _ShortName = "RF5C";
            _ChMax = 8;

            Frequency = 12500000;
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
