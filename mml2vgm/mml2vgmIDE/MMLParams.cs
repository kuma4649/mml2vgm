using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using SoundManager;

namespace mml2vgmIDE
{
    public class MMLParams
    {
        public MMLInst YM2612 = new MMLInst();
        public MMLInst YM2612X = new MMLInst();

        public MMLParams()
        {
            YM2612 = new MMLInst();
            YM2612.inst = new int?[9];
            YM2612.envelope = new int?[9];
            YM2612.notecmd = new string[9];
            YM2612.vol = new int?[9];
            YM2612.octave = new int?[9];
            YM2612.length = new string[9];
            YM2612.pan = new string[9];

            YM2612X = new MMLInst();
            YM2612X.inst = new int?[12];
            YM2612X.envelope = new int?[12];
            YM2612X.notecmd = new string[12];
            YM2612X.vol = new int?[12];
            YM2612X.octave = new int?[12];
            YM2612X.length = new string[12];
            YM2612X.pan = new string[12];
        }

        public bool SetMMLParameter(ref outDatum od, ref long Counter, ref Chip Chip, ref EnmDataType Type, ref int Address, ref int Data, ref object ExData)
        {
            if (od == null || od.type == enmMMLType.unknown || od.linePos == null)
            {
                return true;
            }

            int cc = Audio.sm != null ? Audio.sm.CurrentClockCount : 0;

            if (od.linePos.chip == "YM2612")
            {
                switch (od.type)
                {
                    case enmMMLType.Instrument:
                        YM2612.inst[od.linePos.ch] = (int)od.args[1];
                        break;
                    case enmMMLType.Octave:
                        YM2612.octave[od.linePos.ch] = (int)od.args[0];
                        break;
                    case enmMMLType.OctaveDown:
                        YM2612.octave[od.linePos.ch]--;
                        break;
                    case enmMMLType.OctaveUp:
                        YM2612.octave[od.linePos.ch]++;
                        break;
                    case enmMMLType.Note:
                        if (od.args != null && od.args.Count > 0)
                        {
                            Core.Note nt = (Core.Note)od.args[0];
                            int shift = nt.shift;
                            string f = Math.Sign(shift) >= 0 ? string.Concat(Enumerable.Repeat("+", shift)) : string.Concat(Enumerable.Repeat("-", -shift));
                            YM2612.notecmd[od.linePos.ch] = string.Format("o{0}{1}{2}", YM2612.octave[od.linePos.ch], nt.cmd, f);
                            YM2612.length[od.linePos.ch] = string.Format("{0}(#{1})", cc / nt.length, nt.length);
                        }
                        break;
                    case enmMMLType.Rest:
                        Core.Rest rs = (Core.Rest)od.args[0];
                        YM2612.notecmd[od.linePos.ch] = "r";
                        YM2612.length[od.linePos.ch] = string.Format("{0}(#{1})", cc / rs.length, rs.length);
                        break;
                    case enmMMLType.Volume:
                        if (od.linePos != null)
                            YM2612.vol[od.linePos.ch] = (int)od.args[0];
                        break;
                    case enmMMLType.Pan:
                        int n = (int)od.args[0];
                        YM2612.pan[od.linePos.ch] = n == 0 ? "-" : (n == 1 ? "R" : (n == 2 ? "L" : "C"));
                        break;
                }

            }

            if (od.linePos.chip == "YM2612X")
            {
                switch (od.type)
                {
                    case enmMMLType.Instrument:
                        YM2612X.inst[od.linePos.ch] = (int)od.args[1];
                        break;
                    case enmMMLType.Octave:
                        YM2612X.octave[od.linePos.ch] = (int)od.args[0];
                        break;
                    case enmMMLType.OctaveDown:
                        YM2612X.octave[od.linePos.ch]--;
                        break;
                    case enmMMLType.OctaveUp:
                        YM2612X.octave[od.linePos.ch]++;
                        break;
                    case enmMMLType.Note:
                        if (od.args != null && od.args.Count > 0)
                        {
                            Core.Note nt = (Core.Note)od.args[0];
                            int shift = nt.shift;
                            string f= Math.Sign(shift)>=0 ? string.Concat(Enumerable.Repeat("+", shift)) : string.Concat(Enumerable.Repeat("-", -shift));
                            YM2612X.notecmd[od.linePos.ch] = string.Format("o{0}{1}{2}", YM2612X.octave[od.linePos.ch], nt.cmd, f);
                            YM2612X.length[od.linePos.ch] = string.Format("{0}(#{1})", cc/nt.length, nt.length);
                        }
                        break;
                    case enmMMLType.Rest:
                        Core.Rest rs = (Core.Rest)od.args[0];
                        YM2612X.notecmd[od.linePos.ch] = "r";
                        YM2612X.length[od.linePos.ch] = string.Format("{0}(#{1})", cc / rs.length, rs.length);
                        break;
                    case enmMMLType.Volume:
                        if (od.linePos != null)
                            YM2612X.vol[od.linePos.ch] = (int)od.args[0];
                        break;
                    case enmMMLType.Pan:
                        int n = (int)od.args[0];
                        YM2612X.pan[od.linePos.ch] = n == 0 ? "-" : (n == 1 ? "R" : (n == 2 ? "L" : "C"));
                        break;
                }

            }

            return true;
        }

    }

    public class MMLInst
    {
        public int?[] inst;
        public int?[] envelope;
        public string[] notecmd;
        public int?[] vol;
        public int?[] octave;
        public string[] length;
        public string[] pan;

    }
}