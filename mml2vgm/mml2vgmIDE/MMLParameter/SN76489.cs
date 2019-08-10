using Core;
using System;
using System.Linq;

namespace mml2vgmIDE.MMLParameter
{
    public class SN76489 : Instrument
    {
        public SN76489() : base(4)
        {
            for (int i = 0; i < 4; i++)
            {
                vol[i] = 15;
                beforeTie[i] = false;
            }
        }

        public override string Name => "SN76489";

        public override void SetParameter(outDatum od, int cc)
        {
            int n;
            string s;
            switch (od.type)
            {
                case enmMMLType.Instrument:
                    envelope[od.linePos.ch] = (int)od.args[1];
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
                case enmMMLType.Volume:
                    if (od.linePos != null)
                        vol[od.linePos.ch] = (int)od.args[0];
                    break;
                case enmMMLType.Pan:
                    n = (int)od.args[0];
                    pan[od.linePos.ch] = n == 0 ? "-" : (n == 1 ? "R" : (n == 2 ? "L" : "C"));
                    break;
                case enmMMLType.Envelope:
                    s = (string)od.args[0];
                    envSw[od.linePos.ch] = s == "EON" ? "ON " : "OFF";
                    break;
                case enmMMLType.LfoSwitch:
                    s = (string)od.args[2];
                    lfoSw[od.linePos.ch] = s;
                    break;
                case enmMMLType.Detune:
                    n = (int)od.args[0];
                    detune[od.linePos.ch] = n;
                    break;
                case enmMMLType.KeyShift:
                    n = (int)od.args[0];
                    keyShift[od.linePos.ch] = n;
                    break;

            }
        }

    }
}
