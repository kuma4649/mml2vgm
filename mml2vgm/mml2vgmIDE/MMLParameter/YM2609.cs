using Core;
using musicDriverInterface;
using System;
using System.Linq;

namespace mml2vgmIDE.MMLParameter
{
    public class YM2609 : Instrument
    {
        public YM2609(SoundManager.Chip chip, Setting setting, MIDIKbd midiKbd) : base(12 + 6 + 12 + 6 + 3 + 6, chip,setting,midiKbd)
        {
            for (int i = 0; i < 45; i++)
            {
                vol[i] = i < 18 ? 127 : (i < 30 ? 15 : (i < 36 ? 31 : (i < 39 ? 255 : 31)));
                beforeTie[i] = false;
            }
        }

        public override string Name => "YM2609";

        protected override void SetInstrument(outDatum od, int ch, int cc)
        {
            if (od.linePos.part == "SSG")
            {
                if (od.args[0] is char && (char)od.args[0] == 'W')
                {
                    if (setting.MMLParameter.dispInstrumentName
                        && od.args.Count == 3
                        && (od.args[2] != null
                        && od.args[2].ToString() != ""))
                        inst[od.linePos.ch] = string.Format("{0}", od.args[2]);
                    else
                        inst[od.linePos.ch] = string.Format("W:{0}", (int)od.args[1]);
                }
                else if (od.args[0] is char && (char)od.args[0] == 'I')
                    inst[od.linePos.ch] = string.Format("I:{0}", (int)od.args[1]);
                else
                    envelope[od.linePos.ch] = ((int)od.args[1]).ToString();
            }
            else
                base.SetInstrument(od, ch, cc);
        }

        protected override void SetPan(outDatum od, int ch, int cc)
        {
            if (od.args.Count == 1)
            {
                int n = (int)od.args[0];
                pan[od.linePos.ch] = n == 0 ? "-" : (n == 1 ? "Right" : (n == 2 ? "Left" : "Center"));
            }
            else
                base.SetPan(od, ch, cc);
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
                        keyOnMeter[od.linePos.ch] = (int)(256.0 / (
                        od.linePos.part == "FMOPN" ? 128 : (
                        od.linePos.part == "FMOPNex" ? 128 : (
                        od.linePos.part == "SSG" ? 16 : (
                        od.linePos.part == "RHYTHM" ? 32 : (
                        od.linePos.part == "ADPCM" ? 32 : ( //ADPCM-A
                        od.linePos.part == "ADPCMA" ? 256 : ( //ADPCM 0
                        od.linePos.part == "ADPCMB" ? 256 : 32 // ADPCM 1/2
                        ))))))) * vol[od.linePos.ch]);
                }
            }
            beforeTie[od.linePos.ch] = nt.tieSw;
            bendOctaveHosei(od, nt);
        }

    }
}
