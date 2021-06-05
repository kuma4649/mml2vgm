using Core;
using musicDriverInterface;
using System;
using System.Linq;

namespace mml2vgmIDE.MMLParameter
{
    public class QSound : Instrument
    {
        public QSound(SoundManager.Chip chip, Setting setting, MIDIKbd midiKbd) : base(16, chip,setting,midiKbd)
        {
            for (int i = 0; i < 16; i++)
            {
                vol[i] = 3000;
                beforeTie[i] = false;
            }
        }

        public override string Name => "QSound";

        protected override void SetInstrument(outDatum od, int ch, int cc)
        {
            if ((char)od.args[0] == 'E')
                envelope[od.linePos.ch] = ((int)od.args[1]).ToString();
            else
                base.SetInstrument(od, ch, cc);
        }

        protected override void SetPan(outDatum od, int ch, int cc)
        {
            pan[od.linePos.ch] = string.Format("{0}", (int)od.args[0]);
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
                    double v = Math.Min((double)vol[od.linePos.ch], 5000.0);//65535までだが、恐らく4000ぐらいが最大として使用する音量かと思われる
                    keyOnMeter[od.linePos.ch] = (int)(256.0 / 5000.0 * vol[od.linePos.ch]);
                }
            }
            beforeTie[od.linePos.ch] = nt.tieSw;
        }

    }
}
