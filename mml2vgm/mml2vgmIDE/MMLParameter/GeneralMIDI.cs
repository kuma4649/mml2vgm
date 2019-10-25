using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;

namespace mml2vgmIDE.MMLParameter
{
    public class GeneralMIDI : Instrument
    {
        public GeneralMIDI() : base(99)
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

        public override void SetParameter(outDatum od, int cc)
        {
            if (isTrace) TraceInfo[od.linePos.ch].Enqueue(od);

            int n;
            string s;
            switch (od.type)
            {
                case enmMMLType.Instrument:
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
                            envelope[od.linePos.ch] = (int)od.args[1];
                        }
                    }
                    else
                    {
                        inst[od.linePos.ch] = od.args[0] != null ? od.args[0].ToString() : "(null)";
                    }
                    break;
                //case enmMMLType.Envelope:
                //envelope[od.linePos.ch] = (int)od.args[1];
                //break;
                case enmMMLType.TotalVolume:
                    if (od.linePos != null)
                        vol[od.linePos.ch] = (int)od.args[0];
                    break;
                case enmMMLType.Volume:
                    if (od.linePos != null)
                        expression[od.linePos.ch] = (int)od.args[0];
                    break;
                case enmMMLType.Velocity:
                    if (od.linePos != null)
                        velocity[od.linePos.ch] = (int)od.args[0];
                    break;
                case enmMMLType.Pan:
                        pan[od.linePos.ch] = od.args[0].ToString();
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
                                keyOnMeter[od.linePos.ch] = (int)(256.0 
                                    / 128 * vol[od.linePos.ch] 
                                    / 128 * expression[od.linePos.ch]
                                    / 128 * velocity[od.linePos.ch]
                                    );
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
                case enmMMLType.Envelope:
                    s = (string)od.args[0];
                    envSw[od.linePos.ch] = s == "EON" ? "ON " : "OFF";
                    break;
                case enmMMLType.LfoSwitch:
                    s = (string)od.args[2];
                    lfoSw[od.linePos.ch] = s;
                    break;
                case enmMMLType.Lfo:
                    s = (string)od.args[7];
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
