using Core;
using musicDriverInterface;
using System;
using System.Linq;

namespace mml2vgmIDE.MMLParameter
{
    public class YM2608 : Instrument
    {
        public override string Name => "YM2608";

        public YM2608(SoundManager.Chip chip, Setting setting) : base(20, chip, setting)
        {
        }

        protected override void SetInstrument(outDatum od, int ch, int cc)
        {
            if ((od.args[0] is char && (char)od.args[0] == 'E') || od.linePos.part == "SSG")
            {
                envelope[ch] = ((int)od.args[1]).ToString();
                return;
            }

            base.SetInstrument(od, ch, cc);
        }

        protected override void SetPan(outDatum od, int ch, int cc)
        {
            int n = (int)od.args[0];
            if (od.linePos.part == "SSG")
            {
                pan[ch] = "-";
            }
            else
            {
                pan[ch] = n == 0 ? "-" : (n == 1 ? "Right" : (n == 2 ? "Left" : (n == 3 ? "Center" : n.ToString())));
            }
        }

        //  0- 2 FM1-3ch
        //  3- 5 FM4-6ch
        //  6- 8 FM3ex1-3ch 
        //  9-11 SSG1-3ch
        // 12-17 Rhythm1-6ch
        // 18    adpcm1ch

    }
}
