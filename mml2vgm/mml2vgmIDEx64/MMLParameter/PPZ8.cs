using Core;
using musicDriverInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mml2vgmIDE.MMLParameter
{
    public class PPZ8 : Instrument
    {
        public override string Name => "PPZ8";
        public EnmMmlFileFormat mmlType = EnmMmlFileFormat.GWI;
        private string[] noteStrTbl = new string[] { "c", "c+", "d", "d+", "e", "f", "f+", "g", "g+", "a", "a+", "b" };

        public PPZ8(SoundManager.Chip chip, Setting setting, MIDIKbd midiKbd) : base(8, chip,setting,midiKbd)
        {
            for (int i = 0; i < 8; i++)
            {
                vol[i] = 0;
                beforeTie[i] = false;
            }
        }


        protected override void SetInstrument(outDatum od, int ch, int cc)
        {
            if (od.args[0] is char && (char)od.args[0] == 'E')
                envelope[od.linePos.ch] = ((int)od.args[1]).ToString();
            else
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
                pan[ch] = "-";
                if (n != 0)
                {
                    n -= 5;
                    pan[ch] = string.Format("{0}({1})", (n == 0 ? "Center" : (n < 0 ? "Left" : "Right")), n);
                }
            }
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
            if (od.args == null || od.args.Count <= 0) return;

            if (ch < octave.Length)
            {
                octave[ch] = ((int)od.args[0] >> 4);
                if (((int)od.args[0] & 0xf) < noteStrTbl.Length)
                {
                    notecmd[ch] = string.Format("o{0}{1}", octave[ch], noteStrTbl[((int)od.args[0] & 0xf)]);
                    length[ch] = string.Format("{0:0.##}(#{1:d})", 1.0 * cc / (int)od.args[1], (int)od.args[1]);
                    if (vol[ch] != null)
                        keyOnMeter[ch] = (int)(256.0 / 255 * vol[ch]);
                }
                else
                {
                    notecmd[ch] = "r";
                    length[ch] = string.Format("{0:0.##}(#{1:d})", 1.0 * cc / (int)od.args[1], (int)od.args[1]);
                }
            }
        }



    }
}
