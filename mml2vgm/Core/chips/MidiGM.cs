using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class MidiGM : ClsChip
    {
        public MidiGM(ClsVgm parent, int chipID, string initialPartName, string stPath, int isSecondary) : base(parent, chipID, initialPartName, stPath, isSecondary)
        {
            _chipType = enmChipType.MIDI_GM;
            _Name = "GeneralMIDI";
            _ShortName = "GM";
            _ChMax = 99;
            _canUsePcm = false;
            _canUsePI = false;
            IsSecondary = isSecondary;

            Frequency = 0;
            port = new byte[][] { new byte[] { 0x00 } };
            dataType = 0;

            if (string.IsNullOrEmpty(initialPartName)) return;

            Ch = new ClsChannel[ChMax];
            SetPartToCh(Ch, initialPartName);
            foreach (ClsChannel ch in Ch)
            {
                ch.Type = enmChannelType.MIDI;
                ch.isSecondary = chipID == 1;
                ch.MaxVolume = 127;
            }

            pcmDataInfo = null;

        }

        public override void InitChip()
        {
            if (!use) return;

            //initialize shared param

            //All Key Off

            foreach (partWork pw in lstPartWork)
            {
                pw.port = port;
            }

        }

        public override void InitPart(partWork pw)
        {
            pw.volume = 127;
            pw.MaxVolume = 127;
        }

        public override void SetFNum(partWork pw, MML mml)
        {
        }

        public override void SetVolume(partWork pw, MML mml)
        {
        }

        public override void SetLfoAtKeyOn(partWork pw, MML mml)
        {
        }

        public override void SetKeyOn(partWork pw, MML mml)
        {
        }

        public override void SetKeyOff(partWork pw, MML mml)
        {
        }

    }
}
