using Core;
using musicDriverInterface;
using System;
using System.Linq;

namespace mml2vgmIDE.MMLParameter
{
    public class SegaPCM : Instrument
    {
        public SegaPCM(SoundManager.Chip chip, Setting setting, MIDIKbd midiKbd) : base(16, chip,setting,midiKbd)
        {
            for (int i = 0; i < 16; i++)
            {
                vol[i] = 127;
                beforeTie[i] = false;
            }
        }

        public override string Name => "SEGAPCM";

        protected override void SetInstrument(outDatum od, int ch, int cc)
        {
            if ((char)od.args[0] == 'E')
                envelope[od.linePos.ch] = ((int)od.args[1]).ToString();
            else
                base.SetInstrument(od, ch, cc);
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
                        keyOnMeter[od.linePos.ch] = (int)(256.0 / 128.0 * vol[od.linePos.ch]);
                }
            }
            beforeTie[od.linePos.ch] = nt.tieSw;
            bendOctaveHosei(od, nt);
        }

    }
}
