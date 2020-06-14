using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class Conductor : ClsChip
    {
        public Conductor(ClsVgm parent, int chipID, string initialPartName, string stPath, int chipNumber) : base(parent, chipID, initialPartName, stPath, chipNumber)
        {
            _chipType = enmChipType.CONDUCTOR;
            _Name = "CONDUCTOR";
            _ShortName = "CON";
            _ChMax = 2;
            _canUsePcm = false;
            _canUsePI = false;
            ChipNumber = chipNumber;

            Frequency = 44100;
            port = new byte[][]{
                new byte[] { 0x00 }
            };

            if (string.IsNullOrEmpty(initialPartName)) return;

            Ch = new ClsChannel[ChMax];
            SetPartToCh(Ch, initialPartName);
            foreach (ClsChannel ch in Ch)
            {
                ch.Type = enmChannelType.SSG;
                ch.chipNumber = chipID == 1;
                ch.MaxVolume = 15;
            }

        }

        public override void InitPart(partWork pw)
        {
            foreach (partPage pg in pw.pg)
            {
                pg.volume = 15;
                pg.MaxVolume = 15;
                pg.port = port;
            }
        }

        public override void InitChip()
        {
            if (!use) return;

            for (int ch = 0; ch < ChMax; ch++)
            {
                foreach (partPage pg in lstPartWork[ch].pg)
                {
                    pg.volume = 0;
                }
            }

        }

        public override void StorePcmRawData(clsPcmDatSeq pds, byte[] buf, bool isRaw, bool is16bit, int samplerate, params object[] option)
        {
        }

        public override bool StorePcmCheck()
        {
            return false;
        }

        public override int GetToneDoublerShift(partPage page, int octave, char noteCmd, int shift)
        {
            return 0;
        }

        public override void SetToneDoubler(partPage page, MML mml)
        {
        }

        public override int GetFNum(partPage page, MML mml, int octave, char cmd, int shift)
        {
            return 0;
        }


        public override void SetFNum(partPage page, MML mml)
        {
        }

        public override void SetKeyOn(partPage page, MML mml)
        {
        }

        public override void SetKeyOff(partPage page, MML mml)
        {
        }

        public override void SetVolume(partPage page, MML mml)
        {
        }

        public override void SetLfoAtKeyOn(partPage page, MML mml)
        {
        }

        public override void CmdY(partPage page, MML mml)
        {
        }

        public override void CmdLoopExtProc(partPage page, MML mml)
        {
        }

        public override void CmdInstrument(partPage page, MML mml)
        {
        }

    }
}
