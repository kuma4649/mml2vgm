using Core;
using musicDriverInterface;
using System;
using System.Linq;

namespace mml2vgmIDE.MMLParameter
{
    public class AY8910 : Instrument
    {
        public AY8910(SoundManager.Chip chip,Setting setting) : base(3, chip, setting)
        {
            for (int i = 0; i < 3; i++)
            {
                vol[i] = 15;
                beforeTie[i] = false;
            }
        }

        public override string Name => "AY8910";

        protected override void SetInstrument(outDatum od, int ch, int cc)
        {
            envelope[od.linePos.ch] = ((int)od.args[1]).ToString();
        }

        protected override void SetNote(outDatum od, int ch, int cc)
        {
            Core.Note nt = (Core.Note)od.args[0];
            int shift = nt.shift;
            string f = Math.Sign(shift) >= 0 ? string.Concat(Enumerable.Repeat("+", shift)) : string.Concat(Enumerable.Repeat("-", -shift));
            notecmd[od.linePos.ch] = string.Format("o{0}{1}{2}", octave[od.linePos.ch], nt.cmd, f);
            length[od.linePos.ch] = string.Format("{0:0.##}(#{1:d})", 1.0 * cc / nt.length, nt.length);

            if (!beforeTie[od.linePos.ch])
            {
                if (vol[od.linePos.ch] != null)
                {
                    keyOnMeter[od.linePos.ch] = (int)(256.0 / 16.0 * vol[od.linePos.ch]);
                }
            }
            beforeTie[od.linePos.ch] = nt.tieSw;
        }

    }
}
