﻿using Corex64;
using musicDriverInterface;
using System;
using System.Linq;

namespace mml2vgmIDEx64.MMLParameter
{
    public class YMF262 : Instrument
    {
        public YMF262(SoundManager.Chip chip, Setting setting, MIDIKbd midiKbd) : base(23, chip,setting,midiKbd)
        {
            for (int i = 0; i < 23; i++)
            {
                vol[i] = 63;
                beforeTie[i] = false;
            }
        }

        public override string Name => "YMF262";

        protected override void SetPan(outDatum od, int ch, int cc)
        {
            int n = (int)od.args[0];
            pan[od.linePos.ch] = n == 0 ? "-" : (n == 1 ? "Right" : (n == 2 ? "Left" : "Center"));
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

            //log.Write(notecmd[od.linePos.ch]);

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
