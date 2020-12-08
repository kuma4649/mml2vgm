using Core;
using musicDriverInterface;
using System;
using System.Linq;

namespace mml2vgmIDE.MMLParameter
{
    public class YM2609 : Instrument
    {
        public YM2609(SoundManager.Chip chip) : base(12 + 6 + 12 + 6 + 3 + 6, chip)
        {
            for (int i = 0; i < 45; i++)
            {
                vol[i] = i < 18 ? 127 : (i < 30 ? 15 : (i < 36 ? 31 : (i < 39 ? 255 : 31)));
                beforeTie[i] = false;
            }
        }

        public override string Name => "YM2609";

        public override void SetParameter(outDatum od, int cc)
        {
            if (isTrace) TraceInfo[od.linePos.ch].Enqueue(od);

            int n;
            string s;

            switch (od.type)
            {
                case enmMMLType.Instrument:
                    if (od.linePos.part == "SSG")
                    {
                        envelope[od.linePos.ch] = ((int)od.args[1]).ToString();
                    }
                    else
                    {
                        if (od.args.Count == 3 && (od.args[2] != null && od.args[2].ToString() != ""))
                            inst[od.linePos.ch] = od.args[2].ToString();
                        else
                            inst[od.linePos.ch] = od.args[1] != null ? od.args[1].ToString() : "(null)";
                    }
                    break;
                //case enmMMLType.Envelope:
                //envelope[od.linePos.ch] = (int)od.args[1];
                //break;
                case enmMMLType.Volume:
                    if (od.linePos != null)
                        vol[od.linePos.ch] = (int)od.args[0];
                    break;
                case enmMMLType.Pan:
                    if (od.args.Count == 1)
                    {
                        n = (int)od.args[0];
                        pan[od.linePos.ch] = n == 0 ? "-" : (n == 1 ? "Right" : (n == 2 ? "Left" : "Center"));
                    }
                    else
                    {
                        pan[od.linePos.ch] = string.Format("L{0} R{1}", (int)od.args[0], (int)od.args[1]);
                    }
                    break;
                case enmMMLType.Octave:
                    if (od.args != null && od.args.Count > 0)
                    {
                        octave[od.linePos.ch] = (int)od.args[0];
                    }
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
