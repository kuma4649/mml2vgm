using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class Information
    {

        public const string TITLENAME = "TITLENAME";
        public const string TITLENAMEJ = "TITLENAMEJ";
        public const string GAMENAME = "GAMENAME";
        public const string GAMENAMEJ = "GAMENAMEJ";
        public const string SYSTEMNAME = "SYSTEMNAME";
        public const string SYSTEMNAMEJ = "SYSTEMNAMEJ";
        public const string COMPOSER = "COMPOSER";
        public const string COMPOSERJ = "COMPOSERJ";
        public const string RELEASEDATE = "RELEASEDATE";
        public const string CONVERTED = "CONVERTED";
        public const string NOTES = "NOTES";
        public const string PARTNAME = "PART";
        public const string CLOCKCOUNT = "CLOCKCOUNT";
        public const string FORCEDMONOPARTYM2612 = "FORCEDMONOPARTYM2612";
        public const string VERSION = "VERSION";
        public const string FORMAT = "FORMAT";
        public const string XGMBASEFRAME = "XGMBASEFRAME";
        public const string OCTAVEREV = "OCTAVE-REV";
        public const string VOLUMEREV = "VOLUME-REV";
        public const string ISK052539 = "ISK052539";
        public const string PRIMARY = "PRIMARY";
        public const string SECONDARY = "SECONDARY";
        public const string MODEBEFORESEND = "MODEBEFORESEND";

        readonly public static string[] IDName = new string[] { PRIMARY, SECONDARY };
        public const long DEFAULT_TEMPO = 120L;
        public const long DEFAULT_CLOCK_COUNT = 192L;
        public const long DEFAULT_SAMPLES_PER_CLOCK = VGM_SAMPLE_PER_SECOND * 60 * 4 / (DEFAULT_TEMPO * DEFAULT_CLOCK_COUNT);
        public const long VGM_SAMPLE_PER_SECOND = 44100L;
        public const long XGM_DEFAULT_CLOCK_COUNT = 192L;
        public const double XGM_DEFAULT_SAMPLES_PER_CLOCK = 60.0 * 60 * 4 / (DEFAULT_TEMPO * XGM_DEFAULT_CLOCK_COUNT);



        public float Version = 1.71f;
        public string TitleName = "";
        public string TitleNameJ = "";
        public string GameName = "";
        public string GameNameJ = "";
        public string SystemName = "";
        public string SystemNameJ = "";
        public string Composer = "";
        public string ComposerJ = "";
        public string ReleaseDate = "";
        public string Converted = "";
        public string Notes = "";
        public long userClockCount = 0;
        public List<string> monoPart = null;
        public enmFormat format = enmFormat.VGM;
        public long tempo = DEFAULT_TEMPO;
        public long clockCount = DEFAULT_CLOCK_COUNT;
        public double samplesPerClock = DEFAULT_SAMPLES_PER_CLOCK;
        public long xgmSamplesPerSecond = 60L;
        public bool octaveRev = false;
        public bool volumeRev = false;
        public bool isK052539 = false;
        public int modeBeforeSend = 0;


        public void AddInformation(List<Line> lstLine, Dictionary<enmChipType, ClsChip[]> chips)
        {
            foreach (Line ln in lstLine)
            {
                try
                {
                    string s = ln.Txt.Replace("'{", "").Replace("}", "");
                    int p = s.IndexOf("=");
                    if (p < 0) continue;

                    string wrd = s.Substring(0, p).Trim().ToUpper();
                    string val = s.Substring(p + 1).Trim();

                    if (wrd == TITLENAMEJ) TitleNameJ = val;
                    else if (wrd == TITLENAME) TitleName = val;
                    else if (wrd == GAMENAME) GameName = val;
                    else if (wrd == GAMENAMEJ) GameNameJ = val;
                    else if (wrd == SYSTEMNAME) SystemName = val;
                    else if (wrd == SYSTEMNAMEJ) SystemNameJ = val;
                    else if (wrd == COMPOSER) Composer = val;
                    else if (wrd == COMPOSERJ) ComposerJ = val;
                    else if (wrd == RELEASEDATE) ReleaseDate = val;
                    else if (wrd == CONVERTED) Converted = val;
                    else if (wrd == NOTES) Notes = val;
                    else if (wrd == VERSION)
                    {
                        float.TryParse(val, out float v);
                        v = Math.Min(Math.Max(v, 1.00f), 1.71f);
                        if (v != 1.00f
                            && v != 1.01f
                            && v != 1.10f
                            && v != 1.50f
                            && v != 1.51f
                            && v != 1.60f
                            && v != 1.61f
                            && v != 1.70f
                            && v != 1.71f
                            ) v = 1.71f;
                        Version = v;
                    }
                    else if (wrd == CLOCKCOUNT) userClockCount = int.Parse(val);
                    else if (wrd == FORMAT) SetFormat(val);
                    else if (wrd == XGMBASEFRAME) SetXgmBaseFrame(val);
                    else if (wrd == OCTAVEREV) SetOctaveRev(val);
                    else if (wrd == VOLUMEREV) SetVolumeRev(val);
                    else if (wrd == ISK052539) SetIsK052539(val);
                    else if (wrd == FORCEDMONOPARTYM2612 && chips != null) SetMonoPart(val, chips);
                    else if (wrd == MODEBEFORESEND) SetModeBeforeSend(val);
                }
                catch
                {
                    msgBox.setWrnMsg(string.Format(msg.get("E03000"), ln.Txt), ln.Lp);
                }
            }

        }

        private void SetMonoPart(string val, Dictionary<enmChipType,ClsChip[]> chips)
        {
            monoPart = Common.DivParts(val, chips, out int n);
        }

        private void SetFormat(string val)
        {
            switch (val.ToUpper())
            {
                case "VGM":
                default:
                    format = enmFormat.VGM;
                    tempo = DEFAULT_TEMPO;
                    clockCount = DEFAULT_CLOCK_COUNT;
                    samplesPerClock = DEFAULT_SAMPLES_PER_CLOCK;
                    break;
                case "XGM":
                    format = enmFormat.XGM;
                    tempo = DEFAULT_TEMPO;
                    clockCount = XGM_DEFAULT_CLOCK_COUNT;
                    samplesPerClock = XGM_DEFAULT_SAMPLES_PER_CLOCK;
                    //samplesPerClock = xgmSamplesPerSecond * 60.0 * 4.0 / (tempo * clockCount);
                    break;
                case "ZGM":
                    format = enmFormat.ZGM;
                    tempo = DEFAULT_TEMPO;
                    clockCount = DEFAULT_CLOCK_COUNT;
                    samplesPerClock = DEFAULT_SAMPLES_PER_CLOCK;
                    break;
            }
        }

        private void SetXgmBaseFrame(string val)
        {
            switch (val.ToUpper())
            {
                case "NTSC":
                default:
                    xgmSamplesPerSecond = 60;
                    break;
                case "PAL":
                    xgmSamplesPerSecond = 50;
                    break;
            }
        }

        private void SetOctaveRev(string val)
        {
            switch (val.ToUpper())
            {
                case "TRUE":
                case "1":
                case "YES":
                case "Y":
                    octaveRev = true;
                    break;
                case "FALSE":
                case "0":
                case "NO":
                case "N":
                default:
                    octaveRev = false;
                    break;
            }
        }

        private void SetVolumeRev(string val)
        {
            switch (val.ToUpper())
            {
                case "TRUE":
                case "1":
                case "YES":
                case "Y":
                    volumeRev = true;
                    break;
                case "FALSE":
                case "0":
                case "NO":
                case "N":
                default:
                    volumeRev = false;
                    break;
            }
        }

        private void SetIsK052539(string val)
        {
            switch (val.ToUpper())
            {
                case "TRUE":
                case "1":
                case "YES":
                case "Y":
                    isK052539 = true;
                    break;
                case "FALSE":
                case "0":
                case "NO":
                case "N":
                default:
                    isK052539 = false;
                    break;
            }
        }

        private void SetModeBeforeSend(string val)
        {
            switch (val.ToUpper())
            {
                case "N":
                case "NONE":
                    modeBeforeSend = 0;
                    break;
                case "R":
                case "RR":
                case "RELEASERATE":
                    modeBeforeSend = 1;
                    break;
                case "A":
                case "ALL":
                case "TAKUMI":
                    modeBeforeSend = 2;
                    break;
            }
        }

    }
}
