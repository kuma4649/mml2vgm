using Corex64;
using musicDriverInterface;
using System;
using System.Linq;

namespace mml2vgmIDEx64.MMLParameter
{
    public class YM2610B : Instrument
    {
        public YM2610B(SoundManager.Chip chip, Setting setting, MIDIKbd midiKbd) : base(19, chip,setting,midiKbd)
        {
            for (int i = 0; i < 19; i++)
            {
                vol[i] = i < 9 ? 127 : (i < 12 ? 15 : (i < 18 ? 31 : 255));
                beforeTie[i] = false;
            }
        }
        public override string Name => "YM2610B";

        protected override void SetInstrument(outDatum od, int ch, int cc)
        {
            if (od.linePos.part == "SSG")
            {
                if (od.linePos.ch >= envelope.Length) return;
                envelope[od.linePos.ch] = ((int)od.args[1]).ToString();
            }
            else
                base.SetInstrument(od, ch, cc);
        }

        protected override void SetPan(outDatum od, int ch, int cc)
        {
            if (od.linePos.ch >= pan.Length) return;
            int n = (int)od.args[0];
            pan[od.linePos.ch] = n == 0 ? "-" : (n == 1 ? "Right" : (n == 2 ? "Left" : "Center"));
        }


    }
}
