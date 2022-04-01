using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mml2vgmIDE.MMLParameter
{
    public class YM2608_mucom : Instrument
    {
        public override string Name => "YM2608";
        private readonly string[] noteStrTbl = new string[] { "c", "c+", "d", "d+", "e", "f", "f+", "g", "g+", "a", "a+", "b" };

        public YM2608_mucom(SoundManager.Chip chip, Setting setting, MIDIKbd midiKbd) : base(190, chip, setting, midiKbd)
        {
        }

        protected override int ShapingCh(outDatum od)
        {
            int ch = od.linePos.ch;

            //if (ch < 30) return ch;//FM1-3ch
            //if (ch <= 50) return ch + 60;//SSG1-3ch
            //if (ch == 60) return 120;//りずむ
            //if (ch <= 90) return ch - 40;//FM4-6ch
            //if (ch == 100) return 180;//adpcm

            return ch;
        }

        public void SetMute(int ch, int pt, int pg, bool flg)
        {
            if (chip == null) return;
            if (chip.ChMasks == null) return;
            if (ch < 0 || ch >= chip.ChMasks.Length) return;

            //FM ch<9
            //SSG ch<12
            //リズム ch<18
            //ADPCM1 ch=18
            if (ch < 12 || ch == 18)
            {
                chip.ChMasks[ch] = flg;
                chip.ChMasksPG[ch] = pt * 10 + pg;
            }
            else
            {
                chip.ChMasks[12] = flg;
                chip.ChMasks[13] = flg;
                chip.ChMasks[14] = flg;
                chip.ChMasks[15] = flg;
                chip.ChMasks[16] = flg;
                chip.ChMasks[17] = flg;
            }
        }

        public void SetAssign(int ch, int pt, int pg, bool flg)
        {
            if (chip == null) return;
            if (chip.silentVoice == null) return;
            if (ch < 0 || ch >= chip.silentVoice.Length) return;

            //FM ch<9
            //SSG ch<12
            //リズム ch<18
            //ADPCM1 ch=18
            if (ch < 12 || ch == 18)
            {
                chip.silentVoice[ch] = flg;
                chip.silentVoicePG[ch] = pt * 10 + pg;
            }
            else
            {
                chip.silentVoice[12] = flg;
                chip.silentVoice[13] = flg;
                chip.silentVoice[14] = flg;
                chip.silentVoice[15] = flg;
                chip.silentVoice[16] = flg;
                chip.silentVoice[17] = flg;
            }

            midiKbd.SetAssignChipCh(EnmChip.YM2608, ch);
        }

        protected override void SetPan(outDatum od, int ch, int cc)
        {
            int n = (int)od.args[0];
            char p = '\0';
            if (!string.IsNullOrEmpty(od.linePos.part)) p = od.linePos.part[0];
            if (p == 'S' || p == 'F' || p == 'A') //[S]SG [F]M [A]DPCM [R]HYTHM
            {
                pan[ch] = n == 0 ? "-" : (n == 1 ? "Right" : (n == 2 ? "Left" : (n == 3 ? "Center" : n.ToString())));
            }
            else if (p == 'R') //[R]HYTHM
            {
                n = (n & 0xf0) >> 4;
                pan[ch] = n == 0 ? "-" : (n == 1 ? "Right" : (n == 2 ? "Left" : (n == 3 ? "Center" : n.ToString())));
            }
            else
            {
                pan[ch] = "?";
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

        protected override void SetNote(outDatum od, int ch, int cc)
        {
            if (ch >= octave.Length) return;

            octave[ch] = ((int)od.args[0] >> 4);

            if (((int)od.args[0] & 0xf) < noteStrTbl.Length)
            {
                notecmd[ch] = string.Format("o{0}{1}", octave[ch], noteStrTbl[((int)od.args[0] & 0xf)]);
            }

            length[ch] = string.Format("{0:0.##}(#{1:d})", 1.0 * clockCounter[ch] / (int)od.args[1], (int)od.args[1]);

            if (vol[ch] == null) return;

            keyOnMeter[ch] = (int)(256.0 / (
                od.linePos.part == "FM" ? 15 : (
                od.linePos.part == "SSG" ? 15 : (
                od.linePos.part == "RHYTHM" ? 63 : 255
                ))) * vol[ch]);
        }

        protected override void SetRest(outDatum od, int ch, int cc)
        {
            notecmd[ch] = "r";
            length[ch] = string.Format("{0:0.##}(#{1:d})", 1.0 * clockCounter[ch] / (int)od.args[0], (int)od.args[0]);
        }

        protected override void SetLfo(outDatum od, int ch, int cc)
        {
            if (ch >= lfo.Length) return;
            lfo[ch] = string.Format("M{0},{1},{2},{3}", od.args[0], od.args[1], od.args[2], od.args[3]);
        }

        //30 -

        protected override void SetLfoSwitch(outDatum od, int ch, int cc)
        {
            if (ch >= lfoSw.Length) return;
            string s = (bool)od.args[0] ? "ON" : "OFF";
            lfoSw[ch] = s;
        }
    }
}
