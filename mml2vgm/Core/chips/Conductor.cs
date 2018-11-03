using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class Conductor : ClsChip
    {
        public Conductor(ClsVgm parent, int chipID, string initialPartName, string stPath, bool isSecondary) : base(parent, chipID, initialPartName, stPath, isSecondary)
        {
            _chipType = enmChipType.CONDUCTOR;
            _Name = "CONDUCTOR";
            _ShortName = "CON";
            _ChMax = 2;
            _canUsePcm = false;
            _canUsePI = false;
            IsSecondary = isSecondary;

            Frequency = 44100;

            Ch = new ClsChannel[ChMax];
            SetPartToCh(Ch, initialPartName);
            foreach (ClsChannel ch in Ch)
            {
                ch.Type = enmChannelType.SSG;
                ch.isSecondary = chipID == 1;
                ch.MaxVolume = 15;
            }

        }

        public override void InitPart(ref partWork pw)
        {
            pw.volume = 15;
            pw.MaxVolume = 15;
            pw.port0 = 0;
        }

        public override void InitChip()
        {
            if (!use) return;

            for (int ch = 0; ch < ChMax; ch++)
            {
                lstPartWork[ch].volume = 0;
            }

        }

        public override void SetFNum(partWork pw)
        {
        }

        public override void SetVolume(partWork pw)
        {
        }

    }
}
