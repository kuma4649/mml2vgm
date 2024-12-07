using Corex64;
using musicDriverInterface;
using System;
using System.Linq;

namespace mml2vgmIDEx64.MMLParameter
{
    public class GeneralMIDI : Instrument
    {
        public GeneralMIDI(SoundManager.Chip chip, Setting setting, MIDIKbd midiKbd) : base(99, chip, setting,midiKbd)
        {
            for (int i = 0; i < 99; i++)
            {
                vol[i] = 127;
                expression[i] = 127;
                velocity[i] = 127;
                beforeTie[i] = false;
            }
        }

        public override string Name => "GeneralMIDI";

        protected override void SetInstrument(outDatum od, int ch, int cc)
        {
            if (od.args[0] is char)
            {
                if ((char)od.args[0] == 'n')
                {
                    inst[od.linePos.ch] = od.args[1] != null ? od.args[1].ToString() : "(null)";
                }
                else if ((char)od.args[0] == 'S')
                {
                    //SysEx
                }
                else if ((char)od.args[0] == 'E')
                {
                    envelope[od.linePos.ch] = ((int)od.args[1]).ToString();
                }
            }
            else
            {
                inst[od.linePos.ch] = od.args[0] != null ? od.args[0].ToString() : "(null)";
            }
        }

        protected override void SetPan(outDatum od, int ch, int cc)
        {
            pan[od.linePos.ch] = od.args[0].ToString();
        }

        protected override void SetLfo(outDatum od, int ch, int cc)
        {
            string s = (string)od.args[7];
            lfoSw[od.linePos.ch] = s;
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
                        keyOnMeter[od.linePos.ch] = (int)(256.0
                        / 128 * vol[od.linePos.ch]
                        / 128 * expression[od.linePos.ch]
                        / 128 * velocity[od.linePos.ch]
                        );
                }
            }
            beforeTie[od.linePos.ch] = nt.tieSw;
            bendOctaveHosei(od, nt);
        }

    }
}
