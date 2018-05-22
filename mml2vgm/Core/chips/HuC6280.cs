using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class HuC6280 : ClsChip
    {
        public byte[] pcmData = null;
        public byte CurrentChannel = 0xff;
        public int TotalVolume = 15;
        public int MAXTotalVolume = 15;

        public HuC6280(ClsVgm parent, int chipID, string initialPartName,string stPath) : base( parent, chipID, initialPartName,stPath)
        {

            _Name = "HuC6280";
            _ShortName = "HuC8";
            _ChMax = 6;
            _canUsePcm = true;

            Frequency = 3579545;

            Ch = new ClsChannel[ChMax];
            SetPartToCh(Ch, initialPartName);
            foreach (ClsChannel ch in Ch)
            {
                ch.Type = enmChannelType.WaveForm;
                ch.isSecondary = chipID == 1;
                ch.MaxVolume = 15;
            }

        }


        public override void InitPart(ref partWork pw)
        {
            pw.MaxVolume = 31;
            pw.volume = pw.MaxVolume;
            pw.mixer = 0;
            pw.noise = 0;
        }

    }
}
