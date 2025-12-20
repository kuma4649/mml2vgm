using Corex64;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mml2vgmIDEx64.MMLParameter
{
    public class YM2608_PMD : Instrument
    {
        public override string Name => "YM2608";
        private string[] noteStrTbl = new string[] { "c", "c+", "d", "d+", "e", "f", "f+", "g", "g+", "a", "a+", "b" };

        public YM2608_PMD(SoundManager.Chip chip, Setting setting, MIDIKbd midiKbd) : base(20, chip, setting,midiKbd)
        {
        }

        protected override int ShapingCh(outDatum od)
        {
            int ch = od.linePos.ch;

            if (od.linePos.part == "Rhythm")
            {
                ch = 12;
            }

            return ch;
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

        protected override void SetInstrument(outDatum od, int ch, int cc)
        {
            if ((od.args[0] is char && (char)od.args[0] == 'E') || od.linePos.part == "SSG")
            {
                envelope[ch] = ((int)od.args[1]).ToString();
                return;
            }

            base.SetInstrument(od, ch, cc);
        }

        protected override void SetGatetime(outDatum od, int ch, int cc)
        {
            gatetime[ch] = string.Format("Q%{0:d03} q{1}-{2},{3}"
                , 255 - (int)od.args[0]
                , (int)od.args[1]
                , (byte)((int)od.args[3] + (int)od.args[1])
                , (int)od.args[2]
                );
        }

        protected override void SetNote(outDatum od, int ch, int cc)
        {
            if (ch >= octave.Length) return;

            if (od.linePos.part != "Rhythm")
            {
                octave[ch] = ((int)od.args[0] >> 4);
                if (((int)od.args[0] & 0xf) < noteStrTbl.Length)
                {
                    notecmd[ch] = string.Format("o{0}{1}", octave[ch], noteStrTbl[((int)od.args[0] & 0xf)]);
                    keyOnMeter[ch] = 255;//TBD
                    length[ch] = string.Format("{0:0.##}(#{1:d})", cc / (int)od.args[1], (int)od.args[1]);
                    if (vol[ch] != null)
                    {
                        keyOnMeter[ch] = (int)(256.0 / (
                            od.linePos.part == "FMOPN" ? 127 : (
                            od.linePos.part == "SSG" ? 15 : (
                            od.linePos.part == "RHYTHM" ? 63 : (
                            od.linePos.part == "FMOPNex" ? 127 : 255
                            )))) * vol[ch]);
                    }
                    return;
                }

                //TBD
                notecmd[ch] = "r";
                length[ch] = string.Format("{0:0.##}(#{1:d})", cc / (int)od.args[1], (int)od.args[1]);

                return;
            }

            int nn = (int)od.args[0];
            notecmd[ch] =
                  (((nn & 1024) != 0) ? "Rc" : "--")
                + (((nn & 512) != 0) ? "Cc" : "--")
                + (((nn & 256) != 0) ? "Ho" : "--")
                + (((nn & 128) != 0) ? "Hc" : "--")
                + (((nn & 64) != 0) ? "S2" : "--")
                + (((nn & 32) != 0) ? "Rs" : "--")
                + (((nn & 16) != 0) ? "Ht" : "--")
                + (((nn & 8) != 0) ? "Mt" : "--")
                + (((nn & 4) != 0) ? "Lt" : "--")
                + (((nn & 2) != 0) ? "S1" : "--")
                + (((nn & 1) != 0) ? "Bd" : "--")
                ;
            length[ch] = string.Format("{0:0.##}(#{1:d})", cc / (int)od.args[1], (int)od.args[1]);

        }

        protected override void SetRest(outDatum od, int ch, int cc)
        {
            if (od.linePos.part != "Rhythm")
            {
                //TBD
                notecmd[ch] = "r";
                length[ch] = string.Format("{0:0.##}(#{1:d})", cc / (int)od.args[1], (int)od.args[1]);
            }
            else
            {
                ;
            }
        }

        protected override void SetLfo(outDatum od, int ch, int cc)
        {
            if (od != null && od.args != null)
            {
                switch ((int)od.args[0])
                {
                    case 0: // M / MA / MB
                        lfoPrm[ch] = string.Format("MA{0},{1},{2},{3} MB{4},{5},{6},{7} ",
                            (byte)od.args[1], (byte)od.args[2], (sbyte)od.args[3], (byte)od.args[4],
                            (byte)od.args[5], (byte)od.args[6], (sbyte)od.args[7], (byte)od.args[8]
                            );
                        break;
                    case 1: // MW / MWA / MWB
                        lfoType[ch] = string.Format("MWA{0} MWB{1} ",
                            (byte)od.args[1], (byte)od.args[2]
                            );
                        break;
                }
                lfo[ch] = lfoType[ch] + lfoPrm[ch];
            }
        }

        //30 -

        protected override void SetLfoSwitch(outDatum od, int ch, int cc)
        {
            lfoSw[ch] = string.Format("*A{0} *B{1}", ((int)od.args[0])&7, ((int)od.args[0] & 0x70)>>4);
        }


    }
}
