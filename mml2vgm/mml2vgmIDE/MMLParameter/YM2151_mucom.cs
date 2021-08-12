using Core;
using musicDriverInterface;
using System;
using System.Linq;

namespace mml2vgmIDE.MMLParameter
{
    public class YM2151_mucom : Instrument
    {
        public override string Name => "YM2151";
        private readonly string[] noteStrTbl = new string[] { "c", "c+", "d", "d+", "e", "f", "f+", "g", "g+", "a", "a+", "b" };

        public YM2151_mucom(SoundManager.Chip chip, Setting setting, MIDIKbd midiKbd) : base(80, chip,setting,midiKbd)
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
            chip.ChMasks[ch] = flg;
            chip.ChMasksPG[ch] = pt * 10 + pg;
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
            chip.silentVoice[ch] = flg;
            chip.silentVoicePG[ch] = pt * 10 + pg;

            midiKbd.SetAssignChipCh(EnmChip.YM2151, ch);
        }

        protected override void SetInstrument(outDatum od, int ch, int cc)
        {
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

            keyOnMeter[ch] = (int)(256.0 / 15 * vol[ch]);
        }

        protected override void SetPan(outDatum od, int ch, int cc)
        {
            if (od.linePos.ch >= pan.Length) return;
            int n = (int)od.args[0];
            pan[od.linePos.ch] = n == 0 ? "-" : (n == 1 ? "Right" : (n == 2 ? "Left" : "Center"));
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
