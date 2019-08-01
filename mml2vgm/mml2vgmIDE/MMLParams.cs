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
        public MMLInst YM2612 ;
        public MMLInst YM2612X;
        public MMLInst SN76489;
        public MMLInst RF5C164;

        public MMLParams()
        {
            YM2612 = new MMLInst(9);
            YM2612X = new MMLInst(12);
            SN76489 = new MMLInst(4);
            RF5C164 = new MMLInst(8);
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
                SetYM2612Param(od, cc);
            }
            else if (od.linePos.chip == "YM2612X")
            {
                SetYM2612XParam(od, cc);
            }
            else if (od.linePos.chip == "SN76489")
            {
                SetSN76489Param(od, cc);
            }
            else if (od.linePos.chip == "RF5C164")
            {
                SetRF5C164Param(od, cc);
            }

            return true;
        }

        private void SetRF5C164Param(outDatum od, int cc)
        {
            switch (od.type)
            {
                case enmMMLType.Instrument:
                    if ((char)od.args[0] == 'E')
                    {
                        RF5C164.envelope[od.linePos.ch] = (int)od.args[1];
                    }
                    else
                    {
                        RF5C164.inst[od.linePos.ch] = (int)od.args[1];
                    }
                    break;
                case enmMMLType.Envelope:
                    break;
                case enmMMLType.Octave:
                    RF5C164.octave[od.linePos.ch] = (int)od.args[0];
                    break;
                case enmMMLType.OctaveDown:
                    RF5C164.octave[od.linePos.ch]--;
                    break;
                case enmMMLType.OctaveUp:
                    RF5C164.octave[od.linePos.ch]++;
                    break;
                case enmMMLType.Note:
                    if (od.args != null && od.args.Count > 0)
                    {
                        Core.Note nt = (Core.Note)od.args[0];
                        int shift = nt.shift;
                        string f = Math.Sign(shift) >= 0 ? string.Concat(Enumerable.Repeat("+", shift)) : string.Concat(Enumerable.Repeat("-", -shift));
                        RF5C164.notecmd[od.linePos.ch] = string.Format("o{0}{1}{2}", RF5C164.octave[od.linePos.ch], nt.cmd, f);
                        RF5C164.length[od.linePos.ch] = string.Format("{0:0.##}(#{1:d})", 1.0 * cc / nt.length, nt.length);
                    }
                    break;
                case enmMMLType.Rest:
                    Core.Rest rs = (Core.Rest)od.args[0];
                    RF5C164.notecmd[od.linePos.ch] = "r";
                    RF5C164.length[od.linePos.ch] = string.Format("{0:0.##}(#{1:d})", 1.0 * cc / rs.length, rs.length);
                    break;
                case enmMMLType.Volume:
                    if (od.linePos != null)
                        RF5C164.vol[od.linePos.ch] = (int)od.args[0];
                    break;
                case enmMMLType.Pan:
                    RF5C164.pan[od.linePos.ch] = string.Format("L{0} R{1}",(int)od.args[0],(int)od.args[1]);
                    break;

            }
        }

        private void SetSN76489Param(outDatum od, int cc)
        {
            switch (od.type)
            {
                case enmMMLType.Instrument:
                    SN76489.envelope[od.linePos.ch] = (int)od.args[1];
                    break;
                case enmMMLType.Octave:
                    SN76489.octave[od.linePos.ch] = (int)od.args[0];
                    break;
                case enmMMLType.OctaveDown:
                    SN76489.octave[od.linePos.ch]--;
                    break;
                case enmMMLType.OctaveUp:
                    SN76489.octave[od.linePos.ch]++;
                    break;
                case enmMMLType.Note:
                    if (od.args != null && od.args.Count > 0)
                    {
                        Core.Note nt = (Core.Note)od.args[0];
                        int shift = nt.shift;
                        string f = Math.Sign(shift) >= 0 ? string.Concat(Enumerable.Repeat("+", shift)) : string.Concat(Enumerable.Repeat("-", -shift));
                        SN76489.notecmd[od.linePos.ch] = string.Format("o{0}{1}{2}", SN76489.octave[od.linePos.ch], nt.cmd, f);
                        SN76489.length[od.linePos.ch] = string.Format("{0:0.##}(#{1:d})", 1.0 * cc / nt.length, nt.length);
                    }
                    break;
                case enmMMLType.Rest:
                    Core.Rest rs = (Core.Rest)od.args[0];
                    SN76489.notecmd[od.linePos.ch] = "r";
                    SN76489.length[od.linePos.ch] = string.Format("{0:0.##}(#{1:d})", 1.0 * cc / rs.length, rs.length);
                    break;
                case enmMMLType.Volume:
                    if (od.linePos != null)
                        SN76489.vol[od.linePos.ch] = (int)od.args[0];
                    break;
                case enmMMLType.Pan:
                    int n = (int)od.args[0];
                    SN76489.pan[od.linePos.ch] = n == 0 ? "-" : (n == 1 ? "R" : (n == 2 ? "L" : "C"));
                    break;

            }
        }

        private void SetYM2612XParam(outDatum od, int cc)
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
                        string f = Math.Sign(shift) >= 0 ? string.Concat(Enumerable.Repeat("+", shift)) : string.Concat(Enumerable.Repeat("-", -shift));
                        YM2612X.notecmd[od.linePos.ch] = string.Format("o{0}{1}{2}", YM2612X.octave[od.linePos.ch], nt.cmd, f);
                        YM2612X.length[od.linePos.ch] = string.Format("{0:0.##}(#{1:d})", 1.0 * cc / nt.length, nt.length);
                    }
                    break;
                case enmMMLType.Rest:
                    Core.Rest rs = (Core.Rest)od.args[0];
                    YM2612X.notecmd[od.linePos.ch] = "r";
                    YM2612X.length[od.linePos.ch] = string.Format("{0:0.##}(#{1:d})", 1.0 * cc / rs.length, rs.length);
                    break;
                case enmMMLType.Volume:
                    if (od.linePos != null)
                        YM2612X.vol[od.linePos.ch] = (int)od.args[0];
                    break;
                case enmMMLType.Pan:
                    int n = (int)od.args[0];
                    YM2612X.pan[od.linePos.ch] = n == 0 ? "-" : (n == 1 ? "Right" : (n == 2 ? "Left" : "Center"));
                    break;
            }
        }

        private void SetYM2612Param(outDatum od, int cc)
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
                        YM2612.length[od.linePos.ch] = string.Format("{0:0.##}(#{1:d})", 1.0 * cc / nt.length, nt.length);
                    }
                    break;
                case enmMMLType.Rest:
                    Core.Rest rs = (Core.Rest)od.args[0];
                    YM2612.notecmd[od.linePos.ch] = "r";
                    YM2612.length[od.linePos.ch] = string.Format("{0:0.##}(#{1:d})", 1.0 * cc / rs.length, rs.length);
                    break;
                case enmMMLType.Volume:
                    if (od.linePos != null)
                        YM2612.vol[od.linePos.ch] = (int)od.args[0];
                    break;
                case enmMMLType.Pan:
                    int n = (int)od.args[0];
                    YM2612.pan[od.linePos.ch] = n == 0 ? "-" : (n == 1 ? "Right" : (n == 2 ? "Left" : "Center"));
                    break;
            }
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

        public MMLInst(int n)
        {
            inst = new int?[9];
            envelope = new int?[9];
            notecmd = new string[9];
            vol = new int?[9];
            octave = new int?[9];
            length = new string[9];
            pan = new string[9];
        }
    }
}