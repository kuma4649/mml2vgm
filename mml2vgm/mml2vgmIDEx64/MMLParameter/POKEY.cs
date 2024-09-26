using Core;
using musicDriverInterface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace mml2vgmIDE.MMLParameter
{
    public class POKEY : Instrument
    {

        public POKEY(SoundManager.Chip chip, Setting setting, MIDIKbd midiKbd) : base(4, chip, setting, midiKbd)
        {

            for (int i = 0; i < 4; i++)
            {
                vol[i] = 15;
                beforeTie[i] = false;
            }
        }

        public override string Name => "POKEY";

        protected override void SetInstrument(outDatum od, int ch, int cc)
        {
            envelope[od.linePos.ch] = ((int)od.args[1]).ToString();
        }

        protected override void SetNote(outDatum od, int ch, int cc)
        {
            Core.Note nt = (Core.Note)od.args[0];
            int shift = nt.shift;
            string f = Math.Sign(shift) >= 0 ? string.Concat(Enumerable.Repeat("+", shift)) : string.Concat(Enumerable.Repeat("-", -shift));
            if (nt.trueKeyOn)
                notecmd[od.linePos.ch] = string.Format("o{0}{1}{2}", octave[od.linePos.ch], nt.cmd, f);
            else
                notecmd[od.linePos.ch] = string.Format("skip (o{0}{1}{2})", octave[od.linePos.ch], nt.cmd, f);
            length[od.linePos.ch] = string.Format("{0:0.##}(#{1:d})", 1.0 * cc / nt.length, nt.length);

            if (!beforeTie[od.linePos.ch])
            {
                if (vol[od.linePos.ch] != null)
                {
                    if (nt.trueKeyOn)
                        keyOnMeter[od.linePos.ch] = (int)(256.0 / 16.0 * vol[od.linePos.ch]);
                }
            }
            beforeTie[od.linePos.ch] = nt.tieSw;
            bendOctaveHosei(od, nt);
        }

        public static Instrument SetupInstInfo(outDatum od, ref SoundManager.Chip chip, Setting setting, bool isTrace, MIDIKbd midiKbd)
        {
            if (Audio.chipRegister == null || Audio.chipRegister.POKEY == null) return null;

            if (od.linePos.chipIndex < Audio.chipRegister.POKEY.Count)
            {
                chip = Audio.chipRegister.POKEY[od.linePos.chipIndex];
            }

            if (chip == null && od.linePos.chipIndex >= 0x80)
            {
                Driver.ZGM.ZgmChip.ZgmChip zChip = Audio.chipRegister.dicChipCmdNo[od.linePos.chipIndex];
                chip = Audio.chipRegister.POKEY[zChip.Index];
            }

            POKEY pokey = new POKEY(chip, setting, midiKbd);
            pokey.isTrace = isTrace;

            return pokey;
        }

    }
}
