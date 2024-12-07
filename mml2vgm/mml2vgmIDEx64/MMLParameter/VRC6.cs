using Corex64;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mml2vgmIDEx64.MMLParameter
{
    public class VRC6:Instrument
    {
        public VRC6(SoundManager.Chip chip, Setting setting, MIDIKbd midiKbd) : base(3, chip, setting, midiKbd)
        {
            for (int i = 0; i < 3; i++)
            {
                vol[i] = 15;
                beforeTie[i] = false;
            }
        }

        public override string Name => "VRC6";

        protected override void SetInstrument(outDatum od, int ch, int cc)
        {
            envelope[od.linePos.ch] = ((int)od.args[1]).ToString();
        }

        protected override void SetNote(outDatum od, int ch, int cc)
        {
            Corex64.Note nt = (Corex64.Note)od.args[0];
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
            if (Audio.chipRegister == null || Audio.chipRegister.VRC6 == null) return null;

            if (od.linePos.chipIndex < Audio.chipRegister.VRC6.Count)
            {
                chip = Audio.chipRegister.VRC6[od.linePos.chipIndex];
            }

            if (chip == null && od.linePos.chipIndex >= 0x80)
            {
                Driver.ZGM.ZgmChip.ZgmChip zChip = Audio.chipRegister.dicChipCmdNo[od.linePos.chipIndex];
                chip = Audio.chipRegister.VRC6[zChip.Index];
            }

            VRC6 vrc6 = new VRC6(chip, setting, midiKbd);
            vrc6.isTrace = isTrace;

            return vrc6;
        }

    }
}
