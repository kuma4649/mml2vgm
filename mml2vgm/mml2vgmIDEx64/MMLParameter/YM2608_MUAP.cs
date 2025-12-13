using Corex64;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mml2vgmIDEx64.MMLParameter
{
    public class YM2608_MUAP : Instrument
    {
        public override string Name => "YM2608";
        private readonly string[] noteStrTbl = new string[] { "c", "c+", "d", "d+", "e", "f", "f+", "g", "g+", "a", "a+", "b" };

        public YM2608_MUAP(SoundManager.Chip chip, Setting setting, MIDIKbd midiKbd) : base(17, chip, setting, midiKbd)
        {
            for(int i = 0; i < 17; i++)
            {
                clockCounter[i] = 192;
                vol[i] = 110;
                vol2[i] = 110;
                volMode[i] = 2;
                //if (i == 3 || i == 4 || i == 5 || i == 9)
                //if (i == 9)
                //{
                //    vol[i] = 11;
                //    vol2[i] = 80 + 11 * 3;
                //}
                partColor[i] = 8;
            }
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
            if (ch < 0 || ch >= chip.ChMasks.Count) return;

            
            
            //リズム ch 10 -> 12
            //ADPCM1 ch=18
            if (ch < 3)//FM ch 0,1,2 -> 0,1,2
            {
                while (chip.ChMasks.Count <= ch) chip.ChMasks.Add(false);
                chip.ChMasks[ch] = flg;
                if (ch == 2)
                {
                    chip.ChMasks[6] = flg;
                    chip.ChMasks[7] = flg;
                    chip.ChMasks[8] = flg;
                }
            }
            else if (ch < 6)//SSG ch 3,4,5 -> 9,10,11
            {
                while (chip.ChMasks.Count <= ch + 6) chip.ChMasks.Add(false);
                chip.ChMasks[ch + 6] = flg;
            }
            else if (ch < 9)//FM ch 6,7,8 -> 3,4,5
            {
                while (chip.ChMasks.Count <= ch - 3) chip.ChMasks.Add(false);
                chip.ChMasks[ch - 3] = flg;
            }
            else if (ch < 10)//Rtm ch 9 -> 12,13,14,15,16,17
            {
                while (chip.ChMasks.Count <= ch + 3) chip.ChMasks.Add(false);
                chip.ChMasks[ch + 3] = flg;
                chip.ChMasks[ch + 4] = flg;
                chip.ChMasks[ch + 5] = flg;
                chip.ChMasks[ch + 6] = flg;
                chip.ChMasks[ch + 7] = flg;
                chip.ChMasks[ch + 8] = flg;
            }
            else if (ch < 11)//ADPCM ch 10 -> 18
            {
                while (chip.ChMasks.Count <= ch + 8) chip.ChMasks.Add(false);
                chip.ChMasks[ch + 8] = flg;
                Audio.chipRegister.CS4231Mute(0, 0, flg);
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

            midiKbd?.SetAssignChipCh(EnmChip.YM2608, ch);
        }

        protected override void SetPan(outDatum od, int ch, int cc)
        {
            int n = ((int)od.args[0]);
            switch (n)
            {
                case 0x80:
                    pan[ch] = "Left";
                    break;
                case 0xc0:
                    pan[ch] = "Center";
                    break;
                case 0x40:
                    pan[ch] = "Right";
                    break;
                case 0x82:
                    pan[ch] = "LM";
                    break;
                case 0x6:
                    pan[ch] = "LK";
                    break;
                case 0xc4:
                    pan[ch] = "MK";
                    break;
                case 0xcc:
                    pan[ch] = "MM";
                    break;
                case 0x41:
                    pan[ch] = "RM";
                    break;
                case 0x9:
                    pan[ch] = "RK";
                    break;
                default:
                    pan[ch] = "";
                    break;
            }
        }

        protected override void SetInstrument(outDatum od, int ch, int cc)
        {
            inst[ch]= ((byte)od.args[1]).ToString();

        }

        private string[] Rstr = new string[]
        {
            "------","-----B","----S-","----SB",
            "---C--","---C-B","---CS-","---CSB",
            "--H---","--H--B","--H-S-","--H-SB",
            "--HC--","--HC-B","--HCS-","--HCSB",
            "-T----","-T---B","-T--S-","-T--SB",
            "-T-C--","-T-C-B","-T-CS-","-T-CSB",
            "-TH---","-TH--B","-TH-S-","-TH-SB",
            "-THC--","-THC-B","-THCS-","-THCSB",
            "R-----","R----B","R---S-","R---SB",
            "R--C--","R--C-B","R--CS-","R--CSB",
            "R-H---","R-H--B","R-H-S-","R-H-SB",
            "R-HC--","R-HC-B","R-HCS-","R-HCSB",
            "RT----","RT---B","RT--S-","RT--SB",
            "RT-C--","RT-C-B","RT-CS-","RT-CSB",
            "RTH---","RTH--B","RTH-S-","RTH-SB",
            "RTHC--","RTHC-B","RTHCS-","RTHCSB",

        };

        protected override void SetNote(outDatum od, int ch, int cc)
        {
            if (ch<0 || ch >= octave.Length) return;

            octave[ch] = ((int)od.args[0] / 12)+1;

            if (ch != 9)
            {
                if (((int)od.args[0] % 12) < noteStrTbl.Length)
                {
                    notecmd[ch] = string.Format("o{0}{1}", octave[ch], noteStrTbl[((int)od.args[0] % 12)]);
                }
            }
            else
            {
                inst[ch] = ((int)od.args[0] % 64).ToString();
                notecmd[ch] = Rstr[(int)od.args[0]%64];

            }

            length[ch] = (int)od.args[1] == 0 ? "-" : (string.Format("{0:0.##}(#{1:d})", 1.0 * clockCounter[ch] / (int)od.args[1], (int)od.args[1]));

            if (vol[ch] == null) return;

            keyOnMeter[ch] = (int)(256.0 / (
                od.linePos.part == "FM" ? ((volMode[ch] == null || volMode[ch] < 2) ? 15 : 127) : (
                od.linePos.part == "SSG" ? 127:(//15 : (
                od.linePos.part == "RHYTHM" ? 127:127//15//15 : 15
                ))) * vol[ch]);

        }

        protected override void SetRest(outDatum od, int ch, int cc)
        {
            if (ch != 9)
            {
                notecmd[ch] = "r";
            }
            else
            {
                notecmd[ch] = Rstr[0];
            }
            length[ch] = string.Format("{0:0.##}(#{1:d})", 1.0 * clockCounter[ch] / (int)od.args[0], (int)od.args[0]);
        }

        protected override void SetLfo(outDatum od, int ch, int cc)
        {
            if (ch >= lfo.Length) return;
            lfo[ch] = string.Format("M{0},{1},{2},{3}", od.args[0], od.args[1], (short)(int)od.args[2], od.args[3]);
        }

        //30 -

        protected override void SetLfoSwitch(outDatum od, int ch, int cc)
        {
            if (ch >= lfoSw.Length) return;
            string s = (bool)od.args[0] ? "ON" : "OFF";
            lfoSw[ch] = s;
        }

        protected override void SetGatetimeDiv(outDatum od, int ch, int cc)
        {
            gatetime[ch] = string.Format("{0}",(int)od.args[0]);
        }

        protected override void SetHardLFO(outDatum od, int ch, int cc)
        {
            if (ch >= hlfo.Length) return;
            hlfo[ch] = string.Format("H{0},{1},{2}", od.args[0], od.args[1], od.args[2]);
        }

        protected override void SetVolume(outDatum od, int ch, int cc)
        {
            if (ch >= vol.Length)
                return;

            if (od.linePos != null)
            {
                vol2[ch] = (int)od.args[0];
                vol[ch] = od.linePos.part == "FM" ? (int)vol2[ch]
                    : od.linePos.part == "SSG" ? (int)vol2[ch] //(((int)vol2[ch] - 80) / 3)
                    : od.linePos.part == "RHYTHM" ? (int)vol2[ch] //(((int)vol2[ch] - 80) / 3)
                    : (int)vol2[ch]//(((int)vol2[ch]) / 8)
                    ;
                if (od.args.Count > 1)
                {
                    volMode[ch] = (byte)od.args[1];
                }
            }
        }

        protected override void SetVolumeUp(outDatum od, int ch, int cc)
        {
            if (ch >= vol.Length)
                return;

            if (od.linePos != null)
            {
                vol2[ch] += (int)od.args[0];
                vol[ch] = od.linePos.part == "FM" ? (int)vol2[ch]
                    : od.linePos.part == "SSG" ? (int)vol2[ch] //(((int)vol2[ch] - 80) / 3)
                    : od.linePos.part == "RHYTHM" ? (int)vol2[ch] //(((int)vol2[ch] - 80) / 3)
                    : (int)vol2[ch]//(((int)vol2[ch]) / 8)
                    ;
                vol[ch] = Math.Min(Math.Max((int)vol[ch], 0), 127);
                volMode[ch] = (byte)2;
            }
        }

        protected override void SetVolumeDown(outDatum od, int ch, int cc)
        {
            if (ch >= vol.Length)
                return;

            if (od.linePos != null)
            {
                vol2[ch] -= (int)od.args[0];
                vol[ch] = od.linePos.part == "FM" ? (int)vol2[ch]
                    : od.linePos.part == "SSG" ? (int)vol2[ch] //(((int)vol2[ch] - 80) / 3)
                    : od.linePos.part == "RHYTHM" ? (int)vol2[ch] //(((int)vol2[ch] - 80) / 3)
                    : (int)vol2[ch]//(((int)vol2[ch]) / 8)
                    ;
                vol[ch] = Math.Min(Math.Max((int)vol[ch], 0), 127);
                volMode[ch] = (byte)2;
            }
        }

    }
}
