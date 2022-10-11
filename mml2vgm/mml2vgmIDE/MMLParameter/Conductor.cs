using Core;
using musicDriverInterface;
using System;
using System.Linq;

namespace mml2vgmIDE.MMLParameter
{
    public class Conductor : Instrument
    {

        public Conductor(SoundManager.Chip chip, Setting setting, MIDIKbd midiKbd) : base(2, chip,setting,midiKbd)
        {
            for (int i = 0; i < 2; i++)
            {
                vol[i] = 0;
                beforeTie[i] = false;
            }
        }

        public override string Name => "CONDUCTOR";

        protected override void SetInstrument(outDatum od, int ch, int cc)
        {
            inst[od.linePos.ch] = "-";
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

        protected override void SetLyric(outDatum od, int ch, int cc)
        {
            int ls = (int)od.args[1];
            notecmd[od.linePos.ch] = "lyric";
            length[od.linePos.ch] = string.Format("{0:0.##}(#{1:d})", 1.0 * cc / ls, ls);
            keyOnMeter[od.linePos.ch] = (int)(255.0);
        }

    }
}
