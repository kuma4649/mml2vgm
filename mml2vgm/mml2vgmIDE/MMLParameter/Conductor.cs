using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using musicDriverInterface;

namespace mml2vgmIDE.MMLParameter
{
    public class Conductor : Instrument
    {

        public Conductor(SoundManager.Chip chip) : base(2,chip)
        {
            for (int i = 0; i < 2; i++)
            {
                vol[i] = 0;
                beforeTie[i] = false;
            }
        }

        public override string Name => "CONDUCTOR";

        public override void SetParameter(outDatum od, int cc)
        {
            if (isTrace) TraceInfo[od.linePos.ch].Enqueue(od);


            switch (od.type)
            {
                case enmMMLType.Instrument:
                    break;
                case enmMMLType.Octave:
                    break;
                case enmMMLType.OctaveDown:
                    break;
                case enmMMLType.OctaveUp:
                    break;
                case enmMMLType.Note:
                    if (od.args != null && od.args.Count > 0)
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
                    break;
                case enmMMLType.Rest:
                    Core.Rest rs = (Core.Rest)od.args[0];
                    notecmd[od.linePos.ch] = "r";
                    length[od.linePos.ch] = string.Format("{0:0.##}(#{1:d})", 1.0 * cc / rs.length, rs.length);
                    break;
                case enmMMLType.Lyric:
                    int ls = (int)od.args[1];
                    notecmd[od.linePos.ch] = "lyric";
                    length[od.linePos.ch] = string.Format("{0:0.##}(#{1:d})", 1.0 * cc / ls, ls);
                    keyOnMeter[od.linePos.ch] = (int)(255.0);
                    break;
                case enmMMLType.Volume:
                    break;
                case enmMMLType.Pan:
                    break;
                case enmMMLType.Envelope:
                    break;
                case enmMMLType.LfoSwitch:
                    break;
                case enmMMLType.Detune:
                    break;
                case enmMMLType.KeyShift:
                    break;

            }
        }

    }
}
