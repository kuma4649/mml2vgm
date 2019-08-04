using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;

namespace mml2vgmIDE.MMLParameter
{
    public class YM2203 : Instrument
    {
        public YM2203() : base(9)
        {
        }
        public override string Name => "YM2203";

        public override void SetParameter(outDatum od, int cc)
        {
            switch (od.type)
            {
                case enmMMLType.Instrument:
                    if (od.linePos.part == "SSG")
                    {
                        envelope[od.linePos.ch] = (int)od.args[1];
                    }
                    else
                    {
                        inst[od.linePos.ch] = (int)od.args[1];
                    }
                    break;
                case enmMMLType.Volume:
                    if (od.linePos != null)
                        vol[od.linePos.ch] = (int)od.args[0];
                    break;
                case enmMMLType.Pan:
                    pan[od.linePos.ch] = "-";
                    //int n = (int)od.args[0];
                    //pan[od.linePos.ch] = n == 0 ? "-" : (n == 1 ? "Right" : (n == 2 ? "Left" : "Center"));
                    break;
                case enmMMLType.Octave:
                    octave[od.linePos.ch] = (int)od.args[0];
                    break;
                case enmMMLType.OctaveDown:
                    octave[od.linePos.ch]--;
                    break;
                case enmMMLType.OctaveUp:
                    octave[od.linePos.ch]++;
                    break;
                case enmMMLType.Note:
                    if (od.args != null && od.args.Count > 0)
                    {
                        Core.Note nt = (Core.Note)od.args[0];
                        int shift = nt.shift;
                        string f = Math.Sign(shift) >= 0 ? string.Concat(Enumerable.Repeat("+", shift)) : string.Concat(Enumerable.Repeat("-", -shift));
                        notecmd[od.linePos.ch] = string.Format("o{0}{1}{2}", octave[od.linePos.ch], nt.cmd, f);
                        length[od.linePos.ch] = string.Format("{0:0.##}(#{1:d})", 1.0 * cc / nt.length, nt.length);
                    }
                    break;
                case enmMMLType.Rest:
                    Core.Rest rs = (Core.Rest)od.args[0];
                    notecmd[od.linePos.ch] = "r";
                    length[od.linePos.ch] = string.Format("{0:0.##}(#{1:d})", 1.0 * cc / rs.length, rs.length);
                    break;
            }
        }
    }
}
