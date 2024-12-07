using Corex64;
using musicDriverInterface;
using System;
using System.Linq;

namespace mml2vgmIDEx64.MMLParameter
{
    public class YM2203 : Instrument
    {
        public YM2203(SoundManager.Chip chip, Setting setting, MIDIKbd midiKbd) : base(9, chip,setting,midiKbd)
        {
            for (int i = 0; i < 9; i++)
            {
                vol[i] = i < 6 ? 127 : 15;
                beforeTie[i] = false;
            }
        }
        public override string Name => "YM2203";

        protected override void SetInstrument(outDatum od, int ch, int cc)
        {
            if (od.linePos.part == "SSG")
            {
                envelope[od.linePos.ch] = ((int)od.args[1]).ToString();
            }
            else
                base.SetInstrument(od, ch, cc);
        }

        protected override void SetPan(outDatum od, int ch, int cc)
        {
            pan[od.linePos.ch] = "-";
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
                        keyOnMeter[od.linePos.ch] = (int)(256.0 / (od.linePos.part == "SSG" ? 16 : 128.0) * vol[od.linePos.ch]);
                }
            }
            beforeTie[od.linePos.ch] = nt.tieSw;
            bendOctaveHosei(od, nt);
        }

    }
}
