using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mml2vgm
{
    public class clsVgm
    {
        public float Version = 1.60f;

        public int[] OPN_FNumTbl_7670454 = new int[] {
            //   c    c+     d    d+     e     f    f+     g    g+     a    a+     b    >c
             0x289,0x2af,0x2d8,0x303,0x331,0x362,0x395,0x3cc,0x405,0x443,0x484,0x4c8,0x289*2
        };

        public int[] OPN_FNumTbl_8000000 = new int[] {
            //   c    c+     d    d+     e     f    f+     g    g+     a    a+     b    >c
             0x269,0x28e,0x2b4,0x2de,0x309,0x337,0x368,0x39c,0x3d3,0x40e,0x44c,0x48d,0x4d2
        };

        public int[] psgFNumTbl = new int[] {
            //   c    c+     d    d+     e     f    f+     g    g+     a    a+     b
             0x6ae,0x64e,0x5f4,0x59e,0x54e,0x502,0x4ba,0x476,0x436,0x3f8,0x3c0,0x38a//1 0x3ff over note is silent 
            ,0x357,0x327,0x2fa,0x2cf,0x2a7,0x281,0x25d,0x23b,0x21b,0x1fc,0x1e0,0x1c5//2
            ,0x1ac,0x194,0x17d,0x168,0x153,0x140,0x12e,0x11d,0x10d,0x0fe,0x0f0,0x0e3//3
            ,0x0d6,0x0ca,0x0be,0x0b4,0x0aa,0x0a0,0x097,0x08f,0x087,0x07f,0x078,0x071//4
            ,0x06b,0x065,0x05f,0x05a,0x055,0x050,0x04c,0x047,0x043,0x040,0x03c,0x039//5
            ,0x035,0x032,0x030,0x02d,0x02a,0x028,0x026,0x024,0x022,0x020,0x01e,0x01c//6
            ,0x01b,0x019,0x018,0x016,0x015,0x014,0x013,0x012,0x011,0x010,0x00f,0x00e//7
            ,0x00d,0x00d,0x00c,0x00b,0x00b,0x00a,0x009,0x008,0x007,0x006,0x005,0x004//8
        };

        // TP = M / (ftone * 64)
        public int[] ssgFNumTbl_8000000 = new int[] {
            //   c    c+     d    d+     e     f    f+     g    g+     a    a+     b
             0xEEE,0xE18,0xD4D,0xC8E,0xBDA,0xB30,0xA8F,0x9F7,0x968,0x8E1,0x861,0x7E9
            ,0x777,0x70C,0x6A7,0x647,0x5ED,0x598,0x547,0x4FC,0x4B4,0x470,0x431,0x3F4
            ,0x3BC,0x386,0x353,0x324,0x2F6,0x2CC,0x2A4,0x27E,0x25A,0x238,0x218,0x1FA
            ,0x1DE,0x1C3,0x1AA,0x192,0x17B,0x166,0x152,0x13F,0x12D,0x11C,0x10C,0x0FD
            ,0x0EF,0x0E1,0x0D5,0x0C9,0x0BE,0x0B3,0x0A9,0x09F,0x096,0x08E,0x086,0x07F
            ,0x077,0x071,0x06A,0x064,0x05F,0x059,0x054,0x050,0x04B,0x047,0x043,0x03F
            ,0x03C,0x038,0x035,0x032,0x02F,0x02D,0x02A,0x028,0x026,0x024,0x022,0x020
            ,0x01E,0x01C,0x01B,0x019,0x018,0x016,0x015,0x014,0x013,0x012,0x011,0x010
        };

        /*
        C   ド	    261.62
        C#	ド#	    277.18 1.05947557526183
        D   レ	    293.66 1.122467701246082
        D   レ#	    311.12 1.189205718217262
        E   ミ	    329.62 1.259918966439875
        F   ファ	349.22 1.334836786178427
        F#	ファ#	369.99 1.414226741074841
        G   ソ	    391.99 1.498318171393624
        G#	ソ#	    415.30 1.587416864154117
        A   ラ	    440.00 1.681828606375659
        A#	ラ#	    466.16 1.781820961700176
        B   シ	    493.88 1.887776163901842
        */
        public float[] pcmMTbl = new float[]
        {
            1.0f
            ,1.05947557526183f
            ,1.122467701246082f
            ,1.189205718217262f
            ,1.259918966439875f
            ,1.334836786178427f
            ,1.414226741074841f
            ,1.498318171393624f
            ,1.587416864154117f
            ,1.681828606375659f
            ,1.781820961700176f
            ,1.887776163901842f
        };

        public string note = "c_d_ef_g_a_b";

        private const int instrumentSize = 39 + 8;
        private const int instrumentOperaterSize = 9;
        private const int instrumentMOperaterSize = 11;

        private const string TITLENAME = "TITLENAME";
        private const string TITLENAMEJ = "TITLENAMEJ";
        private const string GAMENAME = "GAMENAME";
        private const string GAMENAMEJ = "GAMENAMEJ";
        private const string SYSTEMNAME = "SYSTEMNAME";
        private const string SYSTEMNAMEJ = "SYSTEMNAMEJ";
        private const string COMPOSER = "COMPOSER";
        private const string COMPOSERJ = "COMPOSERJ";
        private const string RELEASEDATE = "RELEASEDATE";
        private const string CONVERTED = "CONVERTED";
        private const string NOTES = "NOTES";
        private const string PARTNAME = "PART";
        private const string CLOCKCOUNT = "CLOCKCOUNT";
        private const string FMF_NUM = "FMF-NUM";
        private const string PSGF_NUM = "PSGF-NUM";
        private const string FORCEDMONOPARTYM2612 = "FORCEDMONOPARTYM2612";
        private const string VERSION = "VERSION";
        private const string PRIMARY = "PRIMARY";
        private const string SECONDARY = "SECONDARY";
        readonly private string[] IDName = new string[] { PRIMARY, SECONDARY };

        //header
        private byte[] hDat = new byte[] {
                //'Vgm '
                0x56,0x67,0x6d,0x20,
                //Eof offset(see below)
                0x00,0x00,0x00,0x00,
                //Version number(v1.51(0x0000151))
                0x51,0x01,0x00,0x00,
                //SN76489(0x369e99)
                0x99,0x9e,0x36,0x00,
                //YM2413 clock(no use)
                0x00,0x00,0x00,0x00,
                //GD3 offset(no use)
                0x00,0x00,0x00,0x00,
                //Total # samples(see below)
                0x00,0x00,0x00,0x00,
                //Loop offset(no use)
                0x00,0x00,0x00,0x00,
                //Loop # samples(no use)
                0x00,0x00,0x00,0x00,
                //Rate(NTSC 60Hz)
                0x3c,0x00,0x00,0x00,
                //SN76489 feedback(0x09 Mega Drive)
                0x09,0x00,
                //SN76489 shift register width(0x10 Mega Drive)
                0x10,
                //SN76489 Flags(0x00 version 1.51 and later)
                0x00,
                //0x2c YM2612 clock(0x750ab5)
                0xb5,0x0a,0x75,0x00,
                //0x30 YM2151 clock(no use)
                0x00,0x00,0x00,0x00,
                //0x34 VGM data offset(1.50 only)
                0x0c+16*4,0x00,0x00,0x00,
                //0x38 Sega PCM clock(no use)
                0x00,0x00,0x00,0x00,
                //0x3c Sega PCM interface register(no use)
                0x00,0x00,0x00,0x00
                //0x40 RF5C68 clock(no use)
                ,0x00,0x00,0x00,0x00,
                //0x44
                0x00,0x00,0x00,0x00,
                //0x48
                0x00,0x00,0x00,0x00,
                //0x4c YM2610/B clock(0x7a1200)
                0x00,0x12,0x7a,0x00,
                //0x50
                0x00,0x00,0x00,0x00,
                //0x54
                0x00,0x00,0x00,0x00,
                //0x58
                0x00,0x00,0x00,0x00,
                //0x5c
                0x00,0x00,0x00,0x00,
                //0x60
                0x00,0x00,0x00,0x00,
                //0x64
                0x00,0x00,0x00,0x00,
                //0x68
                0x00,0x00,0x00,0x00,
                //0x6c RF5C164 clock(0xbebc20)
                0x20,0xbc,0xbe,0x00,
                //0x70
                0x00,0x00,0x00,0x00,
                //0x74
                0x00,0x00,0x00,0x00,
                //0x78
                0x00,0x00,0x00,0x00,
                //0x7c
                0x00,0x00,0x00,0x00
            };


        public YM2612[] ym2612 = null;
        public SN76489[] sn76489 = null;
        public RF5C164[] rf5c164 = null;
        public YM2610B[] ym2610b = null;
        public List<clsChip> chips = new List<clsChip>();

        public int lineNumber = 0;

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

        public Dictionary<int, byte[]> instFM = new Dictionary<int, byte[]>();
        public Dictionary<int, int[]> instENV = new Dictionary<int, int[]>();
        public Dictionary<int, clsPcm> instPCM = new Dictionary<int, clsPcm>();
        public Dictionary<string, List<Tuple<int, string>>> partData = new Dictionary<string, List<Tuple<int, string>>>();
        public Dictionary<string, Tuple<int, string>> aliesData = new Dictionary<string, Tuple<int, string>>();
        public List<string> monoPart = null;

        private int instrumentCounter = -1;
        private byte[] instrumentBufCache = new byte[instrumentSize];
        private int newStreamID = -1;


        public clsVgm()
        {
            chips = new List<clsChip>();

            ym2612 = new YM2612[] { new YM2612(0, "F"), new YM2612(1, "Fs") };
            sn76489 = new SN76489[] { new SN76489(0, "S"), new SN76489(1, "Ss") };
            rf5c164 = new RF5C164[] { new RF5C164(0, "R"), new RF5C164(1, "Rs") };
            ym2610b = new YM2610B[] { new YM2610B(0, "T"), new YM2610B(1, "Ts") };

            chips.Add(ym2612[0]);
            chips.Add(ym2612[1]);
            chips.Add(sn76489[0]);
            chips.Add(sn76489[1]);
            chips.Add(rf5c164[0]);
            chips.Add(rf5c164[1]);
            chips.Add(ym2610b[0]);
            chips.Add(ym2610b[1]);

        }

        public int analyze(string[] buf)
        {
            lineNumber = 0;

            bool multiLine = false;
            string s2 = "";

            foreach (string s in buf)
            {

                lineNumber++;

                if (multiLine)
                {
                    if (s.Trim() == "")
                    {
                        continue;
                    }
                    s2 += s.Trim() + "\r\n";
                    if (s.IndexOf("}") < 0)
                    {
                        continue;
                    }
                    multiLine = false;
                    s2 = s2.Substring(0, s2.IndexOf("}"));
                    // Information
                    addInformation(s2, lineNumber);
                    continue;
                }

                // 行頭が'以外は読み飛ばす
                if (s.TrimStart().IndexOf("'") != 0)
                {
                    continue;
                }

                s2 = s.TrimStart().Substring(1).TrimStart();
                // 'のみの行も読み飛ばす
                if (s2.Trim() == "")
                {
                    continue;
                }

                if (s2.IndexOf("{") == 0)
                {
                    multiLine = true;
                    s2 = s2.Substring(1);

                    if (s2.IndexOf("}") > -1)
                    {
                        multiLine = false;
                        s2 = s2.Substring(0, s2.IndexOf("}")).Trim();
                        // Information
                        addInformation(s2, lineNumber);
                    }
                    continue;
                }
                else if (s2.IndexOf("@") == 0)
                {
                    // Instrument
                    addInstrument(s2, lineNumber);
                    continue;
                }
                else if (s2.IndexOf("%") == 0)
                {
                    // Alies
                    addAlies(s2, lineNumber);
                    continue;
                }
                else
                {
                    // Part
                    addPart(s2, lineNumber);
                    continue;
                }

            }


            // チェック1定義されていない名称を使用したパートが存在するか

            foreach (string p in partData.Keys)
            {
                bool flg = false;
                foreach (clsChip chip in chips)
                {
                    if (chip.ChannelNameContains(p))
                    {
                        flg = true;
                        break;
                    }
                }
                if (!flg)
                {
                    msgBox.setWrnMsg(string.Format("未定義のパート({0})のデータは無視されます。", p.Substring(0, 2).Trim() + int.Parse(p.Substring(2, 2)).ToString()));
                    flg = false;
                }
            }

            return 0;

        }

        private int addInformation(string buf, int lineNumber)
        {
            string[] settings = buf.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string s in settings)
            {
                try
                {
                    int p = s.IndexOf("=");
                    if (p < 0) continue;

                    string wrd = s.Substring(0, p).Trim().ToUpper();
                    string val = s.Substring(p + 1).Trim();

                    if (wrd == TITLENAME) TitleName = val;
                    if (wrd == TITLENAMEJ) TitleNameJ = val;
                    if (wrd == GAMENAME) GameName = val;
                    if (wrd == GAMENAMEJ) GameNameJ = val;
                    if (wrd == SYSTEMNAME) SystemName = val;
                    if (wrd == SYSTEMNAMEJ) SystemNameJ = val;
                    if (wrd == COMPOSER) Composer = val;
                    if (wrd == COMPOSERJ) ComposerJ = val;
                    if (wrd == RELEASEDATE) ReleaseDate = val;
                    if (wrd == CONVERTED) Converted = val;
                    if (wrd == NOTES) Notes = val;
                    if (wrd == VERSION)
                    {
                        float v = 1.60f;
                        float.TryParse(val, out v);
                        if (v != 1.51f && v != 1.60f) v = 1.60f;
                        Version = v;
                    }

                    foreach (clsChip chip in chips)
                    {
                        if (wrd == PARTNAME + chip.Name + IDName[chip.ChipID]) chip.setPartToCh(chip.Ch, val);
                        if (wrd == PARTNAME + chip.ShortName + IDName[chip.ChipID]) chip.setPartToCh(chip.Ch, val);
                        if (chip.ChipID == 0)
                        {
                            if (wrd == PARTNAME + chip.Name) chip.setPartToCh(chip.Ch, val);
                            if (wrd == PARTNAME + chip.ShortName) chip.setPartToCh(chip.Ch, val);
                        }
                    }

                    if (wrd == CLOCKCOUNT) clockCount = int.Parse(val);
                    if (wrd == FMF_NUM) setFmF_NumTbl(val);
                    if (wrd == FORCEDMONOPARTYM2612) setMonoPart(val);

                    for (int i = 0; i < 8; i++)
                    {
                        if (wrd == string.Format("{0}{1}", PSGF_NUM, i + 1)) setPsgF_NumTbl(val, i);
                    }
                }
                catch
                {
                    msgBox.setWrnMsg(string.Format("不正な定義です。({0})", s), lineNumber);
                }
            }
            return 0;
        }

        private void setFmF_NumTbl(string val)
        {
            //厳密なチェックを行っていないので設定値によってはバグる危険有り

            string[] s = val.Split(new string[] { ",", " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < s.Length; i++)
            {
                OPN_FNumTbl_7670454[i] = int.Parse(s[i], System.Globalization.NumberStyles.HexNumber);
            }
            OPN_FNumTbl_7670454[12] = OPN_FNumTbl_7670454[0] * 2;
        }

        private void setPsgF_NumTbl(string val, int oct)
        {
            //厳密なチェックを行っていないので設定値によってはバグる危険有り

            string[] s = val.Split(new string[] { ",", " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < s.Length; i++)
            {
                if (i + oct * 12 >= psgFNumTbl.Length)
                {
                    break;
                }
                psgFNumTbl[i + oct * 12] = int.Parse(s[i], System.Globalization.NumberStyles.HexNumber);
            }
        }

        private void setMonoPart(string val)
        {
            monoPart = divParts(val);
        }

        private int addInstrument(string buf, int lineNumber)
        {
            if (buf == null || buf.Length < 2)
            {
                msgBox.setWrnMsg("空の音色定義文を受け取りました。", lineNumber);
                return -1;
            }

            string s = buf.Substring(1).TrimStart();

            // FMの音色を定義中の場合
            if (instrumentCounter != -1)
            {

                return setInstrument(s, lineNumber);

            }

            switch (s.ToUpper()[0])
            {
                case 'F':
                    instrumentBufCache = new byte[instrumentSize - 8];
                    instrumentCounter = 0;
                    setInstrument(s.Substring(1).TrimStart(), lineNumber);
                    break;

                case 'M':
                    instrumentBufCache = new byte[instrumentSize];
                    instrumentCounter = 0;
                    setInstrument(s.Substring(1).TrimStart(), lineNumber);
                    break;

                case 'P':
                    try
                    {
                        enmChipType chip = enmChipType.YM2612;
                        string[] vs = s.Substring(1).Trim().Split(new string[] { "," }, StringSplitOptions.None);
                        int num = int.Parse(vs[0]);
                        string fn = vs[1].Trim().Trim('"');
                        int fq = int.Parse(vs[2]);
                        int vol = int.Parse(vs[3]);
                        int lp = -1;
                        bool isSecondary = false;
                        if (vs.Length > 4)
                        {
                            string chipName = vs[4].Trim().ToUpper();
                            isSecondary = false;
                            if (chipName.IndexOf(PRIMARY) >= 0)
                            {
                                isSecondary = false;
                                chipName = chipName.Replace(PRIMARY, "");
                            }
                            else if (chipName.IndexOf(SECONDARY) >= 0)
                            {
                                isSecondary = true;
                                chipName = chipName.Replace(SECONDARY, "");
                            }
                            chip = getChipNumber(chipName);
                            if (!canUsePCM(chip))
                            {
                                msgBox.setWrnMsg("未定義のChipName又はPCMを使用できないChipが指定されています。", lineNumber);
                                break;
                            }
                        }
                        if (vs.Length > 5)
                        {
                            lp = int.Parse(vs[5]);
                        }
                        if (instPCM.ContainsKey(num))
                        {
                            instPCM.Remove(num);
                        }
                        instPCM.Add(num, new clsPcm(num, chip, isSecondary, fn, fq, vol, 0, 0, 0, lp));
                    }
                    catch
                    {
                        msgBox.setWrnMsg("不正なPCM音色定義文です。", lineNumber);
                    }
                    break;
                case 'E':
                    try
                    {
                        string[] vs = s.Substring(1).Trim().Split(new string[] { "," }, StringSplitOptions.None);
                        int[] env = null;
                        env = new int[9];
                        int num = int.Parse(vs[0]);
                        for (int i = 0; i < env.Length; i++)
                        {
                            if (i == 8)
                            {
                                if (vs.Length == 8) env[i] = (int)getChipNumber("SN76489");
                                else env[i] = (int)getChipNumber(vs[8]);
                                continue;
                            }
                            env[i] = int.Parse(vs[i]);
                        }

                        for (int i = 0; i < env.Length - 1; i++)
                        {
                            if (env[8] == (int)enmChipType.SN76489)
                            {
                                checkEnvelopeVolumeRange(lineNumber, env, i, 15, 0);
                                if (env[7] == 0) env[7] = 1;
                            }
                            else if (env[8] == (int)enmChipType.RF5C164)
                            {
                                checkEnvelopeVolumeRange(lineNumber, env, i, 255, 0);
                                if (env[7] == 0) env[7] = 1;
                            }
                            else if (env[8] == (int)enmChipType.YM2610B)
                            {
                                checkEnvelopeVolumeRange(lineNumber, env, i, 255, 0);
                                if (env[7] == 0) env[7] = 1;
                            }
                            else
                            {
                                msgBox.setWrnMsg("エンベロープを使用できない音源が選択されています。", lineNumber);
                            }
                        }

                        if (instENV.ContainsKey(num))
                        {
                            instENV.Remove(num);
                        }
                        instENV.Add(num, env);
                    }
                    catch
                    {
                        msgBox.setWrnMsg("不正なエンベロープ定義文です。", lineNumber);
                    }
                    break;
            }

            return 0;
        }

        private static void checkEnvelopeVolumeRange(int lineNumber, int[] env, int i, int max, int min)
        {
            if (i == 1 || i == 4 || i == 7)
            {
                if (env[i] > max)
                {
                    env[i] = max;
                    msgBox.setWrnMsg(string.Format("Envelope音量が{0}を超えています。", max), lineNumber);
                }
                if (env[i] < min)
                {
                    env[i] = min;
                    msgBox.setWrnMsg(string.Format("Envelope音量が{0}未満です。", min), lineNumber);
                }
            }
        }

        private static enmChipType getChipNumber(string chipN)
        {
            enmChipType chip;
            switch (chipN.ToUpper().Trim())
            {
                case "YM2612":
                    chip = enmChipType.YM2612;
                    break;
                case "SN76489":
                    chip = enmChipType.SN76489;
                    break;
                case "RF5C164":
                    chip = enmChipType.RF5C164;
                    break;
                case "YM2610":
                case "YM2610B":
                    chip = enmChipType.YM2610B;
                    break;
                default:
                    chip = enmChipType.None;
                    break;
            }

            return chip;
        }

        private static bool canUsePCM(enmChipType chipNumber)
        {
            bool use = false;

            switch (chipNumber)
            {
                case enmChipType.YM2610B:
                case enmChipType.YM2612:
                case enmChipType.RF5C164:
                    use = true;
                    break;
                case enmChipType.SN76489:
                default:
                    use = false;
                    break;
            }

            return use;

        }

        private int addAlies(string buf, int lineNumber)
        {
            string name = "";
            string data = "";

            int i = buf.Substring(1).Trim().IndexOfAny(new char[] { ' ', '\t' });
            if (i < 0)
            {
                //空白による区切りが見つからない場合は無視する
                return 0;
            }

            name = buf.Substring(1).Trim().Substring(0, i).Trim();
            data = buf.Substring(1).Trim().Substring(i).Trim();
            if (name == "")
            {
                //エイリアス指定がない場合は警告とする
                msgBox.setWrnMsg("不正なエイリアス指定です。", lineNumber);
                return -1;
            }
            if (data == "")
            {
                //データがない場合は警告する
                msgBox.setWrnMsg("エイリアスにデータがありません。", lineNumber);
            }

            if (aliesData.ContainsKey(name))
            {
                aliesData.Remove(name);
            }
            aliesData.Add(name, new Tuple<int, string>(lineNumber, data));

            return 0;
        }

        private int addPart(string buf, int lineNumber)
        {
            List<string> part = new List<string>();
            string data = "";

            int i = buf.IndexOfAny(new char[] { ' ', '\t' });
            if (i < 0)
            {
                //空白による区切りが見つからない場合は無視する
                return 0;
            }

            part = divParts(buf.Substring(0, i).Trim());
            data = buf.Substring(i).Trim();
            if (part == null)
            {
                //パート指定がない場合は警告とする
                msgBox.setWrnMsg("不正なパート指定です。", lineNumber);
                return -1;
            }
            if (data == "")
            {
                //データがない場合は無視する
                return 0;
            }

            foreach (string p in part)
            {
                if (!partData.ContainsKey(p))
                {
                    partData.Add(p, new List<Tuple<int, string>>());
                }
                partData[p].Add(new Tuple<int, string>(lineNumber, data));
            }

            return 0;
        }

        private List<string> divParts(string parts)
        {
            List<string> ret = new List<string>();
            string a = "";
            int k = 1;
            int m = 0;
            string n0 = "";

            for (int i = 0; i < parts.Length; i++)
            {
                if (m == 0 && parts[i] >= 'A' && parts[i] <= 'Z')
                {
                    a = parts[i].ToString();
                    if (i + 1 < parts.Length && parts[i + 1] >= 'a' && parts[i + 1] <= 'z')
                    {
                        a += parts[i + 1].ToString();
                        i++;
                    }
                    else
                    {
                        a += " ";
                    }

                    k = getChMax(a) > 9 ? 2 : 1;
                    n0 = "";

                }
                else if (m == 0 && parts[i] == ',')
                {
                    n0 = "";
                }
                else if (m == 0 && parts[i] == '-')
                {
                    m = 1;
                }
                else if (parts[i] >= '0' && parts[i] <= '9')
                {
                    string n = parts.Substring(i, k);
                    if (k == 2 && i + 1 < parts.Length)
                    {
                        i++;
                    }

                    if (m == 0)
                    {
                        n0 = n;

                        int s;
                        if (!int.TryParse(n0, out s)) return null;
                        string p = string.Format("{0}{1:00}", a, s);
                        ret.Add(p);
                    }
                    else
                    {
                        string n1 = n;

                        int s, e;
                        if (!int.TryParse(n0, out s)) return null;
                        if (!int.TryParse(n1, out e)) return null;
                        if (s >= e) return null;

                        do
                        {
                            s++;
                            string p = string.Format("{0}{1:00}", a, s);
                            if (ret.Contains(p)) return null;
                            ret.Add(p);
                        } while (s < e);

                        i++;
                        m = 0;
                        n0 = "";
                    }
                }
                else
                {
                    return null;
                }
            }

            return ret;
        }

        private int getChMax(string a)
        {
            foreach (clsChip chip in chips)
            {
                if (chip.Ch[0].Name.Substring(0, 2) == a)
                {
                    return chip.ChMax;
                }
            }

            return 0;
        }

        private int setInstrument(string vals, int lineNumber)
        {
            string n = "";

            try
            {
                foreach (char c in vals)
                {
                    if ((c >= '0' && c <= '9') || c == '-')
                    {
                        n = n + c.ToString();
                        continue;
                    }

                    int i;
                    if (int.TryParse(n, out i))
                    {
                        instrumentBufCache[instrumentCounter] = (byte)(i & 0xff);
                        instrumentCounter++;
                        n = "";
                    }
                }

                if (!string.IsNullOrEmpty(n))
                {
                    int i;
                    if (int.TryParse(n, out i))
                    {
                        instrumentBufCache[instrumentCounter] = (byte)(i & 0xff);
                        instrumentCounter++;
                        n = "";
                    }
                }

                if (instrumentCounter == instrumentBufCache.Length)
                {
                    if (instFM.ContainsKey(instrumentBufCache[0]))
                    {
                        instFM.Remove(instrumentBufCache[0]);
                    }
                    if (instrumentBufCache.Length == instrumentSize)
                    {
                        //M
                        instFM.Add(instrumentBufCache[0], instrumentBufCache);
                    }
                    else
                    {
                        //F
                        instFM.Add(instrumentBufCache[0], convertFtoM(instrumentBufCache));
                    }

                    instrumentCounter = -1;
                }
            }
            catch
            {
                msgBox.setErrMsg("音色の定義が不正です。", lineNumber);
            }

            return 0;
        }

        private byte[] convertFtoM(byte[] instrumentBufCache)
        {
            byte[] ret = new byte[instrumentSize];

            ret[0] = instrumentBufCache[0];

            for (int j = 0; j < 4; j++)
            {
                for (int i = 0; i < instrumentOperaterSize; i++)
                {
                    ret[j * instrumentMOperaterSize + i + 1] = instrumentBufCache[j * instrumentOperaterSize + i + 1];
                }
            }

            ret[instrumentSize - 2] = instrumentBufCache[instrumentSize - 10];
            ret[instrumentSize - 1] = instrumentBufCache[instrumentSize - 9];

            return ret;
        }



        private List<byte> dat = null;

        public const long vgmSamplesPerSecond = 44100L;
        private const long defaultTempo = 120L;
        private const long defaultClockCount = 192L;
        private const long defaultSamplesPerClock = vgmSamplesPerSecond * 60 * 4 / (defaultTempo * defaultClockCount);

        private long tempo = defaultTempo;
        private long clockCount = defaultClockCount;
        private double samplesPerClock = defaultSamplesPerClock;
        public long lSample = 0L;
        public long lClock = 0L;
        private long loopOffset = -1L;
        private long loopSamples = -1L;

        private Random rnd = new Random();


        public byte[] getByteData()
        {

            partInit();

            dat = new List<byte>();

            makeHeader();

            int endChannel = 0;
            newStreamID = -1;
            int totalChannel = 0;
            foreach (clsChip chip in chips) totalChannel += chip.ChMax;

            do
            {

                foreach (clsChip chip in chips)
                {

                    foreach (partWork pw in chip.lstPartWork)
                    {
                        //未使用のパートの場合は処理を行わない
                        if (!pw.chip.use) continue;

                        //pcm sound off
                        if (pw.pcmWaitKeyOnCounter == 0)
                        {
                            pw.pcmWaitKeyOnCounter = -1;
                        }

                        //KeyOff
                        procKeyOff(pw);

                        //Bend
                        procBend(pw);

                        //Lfo
                        procLfo(pw);

                        //Envelope
                        procEnvelope(pw);

                        //wait消化待ち
                        if (pw.waitCounter > 0)
                        {
                            continue;
                        }

                        //データは最後まで実施されたか
                        if (pw.dataEnd)
                        {
                            continue;
                        }

                        //パートのデータがない場合は何もしないで次へ
                        if (!partData.ContainsKey(pw.PartName) || partData[pw.PartName] == null || partData[pw.PartName].Count < 1)
                        {
                            pw.dataEnd = true;
                            continue;
                        }

                        //コマンド毎の処理を実施
                        while (pw.waitCounter == 0 && !pw.dataEnd)
                        {
                            char cmd = pw.getChar();
                            lineNumber = pw.getLineNumber();

                            commander(pw, cmd);
                        }

                    }

                    //channelを跨ぐコマンド向け処理
                    if (chip is YM2610B)
                    {
                        YM2610B y = (YM2610B)chip;

                        //コマンドを跨ぐデータ向け処理
                        foreach (partWork pw in y.lstPartWork)
                        {
                            if (pw.Type == enmChannelType.ADPCMA)
                            {
                                //Adpcm-A TotalVolume処理
                                if (pw.beforeVolume != pw.volume || pw.beforePan != pw.pan)
                                {
                                    outFmAdrPort(pw.port1, (byte)(0x08 + (pw.ch - 12)), (byte)((byte)((pw.pan & 0x3) << 6) | (byte)(pw.volume & 0x1f)));
                                    pw.beforeVolume = pw.volume;
                                    pw.beforePan = pw.pan;
                                }

                                y.adpcmA_KeyOn |= (byte)(pw.keyOn ? (1 << (pw.ch - 12)) : 0);
                                pw.keyOn = false;
                                y.adpcmA_KeyOff |= (byte)(pw.keyOff ? (1 << (pw.ch - 12)) : 0);
                                pw.keyOff = false;
                            }
                        }

                        //Adpcm-A KeyOff処理
                        if (0 != y.adpcmA_KeyOff)
                        {
                            byte data = (byte)(0x80 + y.adpcmA_KeyOff);
                            outFmAdrPort(y.lstPartWork[0].port1, 0x00, data);
                            y.adpcmA_KeyOff = 0;
                        }

                        //Adpcm-A TotalVolume処理
                        if (y.adpcmA_beforeTotalVolume != y.adpcmA_TotalVolume)
                        {
                            outFmAdrPort(y.lstPartWork[0].port1, 0x01, (byte)(y.adpcmA_TotalVolume & 0x3f));
                            y.adpcmA_beforeTotalVolume = y.adpcmA_TotalVolume;
                        }

                        //Adpcm-A KeyOn処理
                        if (0 != y.adpcmA_KeyOn)
                        {
                            byte data = (byte)(0x00 + y.adpcmA_KeyOn);
                            outFmAdrPort(y.lstPartWork[0].port1, 0x00, data);
                            y.adpcmA_KeyOn = 0;
                        }

                    }

                }


                // 全パートのうち次のコマンドまで一番近い値を求める
                long cnt = long.MaxValue;
                foreach (clsChip chip in chips)
                {
                    for (int ch = 0; ch < chip.lstPartWork.Count; ch++)
                    {

                        partWork cpw = chip.lstPartWork[ch];

                        if (!cpw.chip.use) continue;

                        //note
                        if (cpw.waitKeyOnCounter > 0)
                        {
                            cnt = Math.Min(cnt, cpw.waitKeyOnCounter);
                        }
                        else if (cpw.waitCounter > 0)
                        {
                            cnt = Math.Min(cnt, cpw.waitCounter);
                        }

                        //bend
                        if (cpw.bendWaitCounter != -1)
                        {
                            cnt = Math.Min(cnt, cpw.bendWaitCounter);
                        }

                        //lfoとenvelopeは音長によるウエイトカウントが存在する場合のみ対象にする。(さもないと、曲のループ直前の効果を出せない)
                        if (cnt > 0)
                        {
                            //lfo
                            for (int lfo = 0; lfo < 4; lfo++)
                            {
                                if (!cpw.lfo[lfo].sw) continue;
                                if (cpw.lfo[lfo].waitCounter == -1) continue;

                                cnt = Math.Min(cnt, cpw.lfo[lfo].waitCounter);
                            }

                            //envelope
                            if ((cpw.chip is YM2610B && (cpw.Type == enmChannelType.SSG || cpw.Type == enmChannelType.ADPCMA || cpw.Type == enmChannelType.ADPCMB))
                                || cpw.chip is SN76489 
                                || cpw.chip is RF5C164)
                            {
                                if (cpw.envelopeMode && cpw.envIndex != -1)
                                {
                                    cnt = Math.Min(cnt, cpw.envCounter);
                                }
                            }
                        }

                        //pcm
                        if (cpw.pcmWaitKeyOnCounter > 0)
                        {
                            cnt = Math.Min(cnt, cpw.pcmWaitKeyOnCounter);
                        }

                    }

                }

                if (cnt != long.MaxValue)
                {

                    // waitcounterを減らす

                    foreach (clsChip chip in chips)
                    {
                        foreach (partWork pw in chip.lstPartWork)
                        {

                            if (pw.waitKeyOnCounter > 0) pw.waitKeyOnCounter -= cnt;

                            if (pw.waitCounter > 0) pw.waitCounter -= cnt;

                            if (pw.bendWaitCounter > 0) pw.bendWaitCounter -= cnt;

                            for (int lfo = 0; lfo < 4; lfo++)
                            {
                                if (!pw.lfo[lfo].sw) continue;
                                if (pw.lfo[lfo].waitCounter == -1) continue;

                                if (pw.lfo[lfo].waitCounter > 0)
                                {
                                    pw.lfo[lfo].waitCounter -= cnt;
                                    if (pw.lfo[lfo].waitCounter < 0) pw.lfo[lfo].waitCounter = 0;
                                }
                            }

                            if (pw.pcmWaitKeyOnCounter > 0)
                            {
                                pw.pcmWaitKeyOnCounter -= cnt;
                            }

                            if ((pw.chip is YM2610B && (pw.Type == enmChannelType.SSG || pw.Type== enmChannelType.ADPCMA || pw.Type== enmChannelType.ADPCMB))
                                || pw.chip is SN76489
                                || pw.chip is RF5C164)
                            {
                                if (pw.envelopeMode && pw.envIndex != -1)
                                {
                                    pw.envCounter -= (int)cnt;
                                }
                            }
                        }
                    }

                    // wait発行

                    lClock += cnt;
                    lSample += (long)samplesPerClock * cnt;

                    if (ym2612[0].lstPartWork[5].pcmWaitKeyOnCounter <= 0)//== -1)
                    {
                        outWaitNSamples((long)samplesPerClock * cnt);
                    }
                    else
                    {
                        outWaitNSamplesWithPCMSending(ym2612[0].lstPartWork[5], cnt);
                    }
                }

                endChannel = 0;
                foreach (clsChip chip in chips)
                {
                    foreach (partWork pw in chip.lstPartWork)
                    {
                        if (!pw.chip.use) endChannel++;
                        else if (pw.dataEnd && pw.waitCounter < 1) endChannel++;
                        else if (loopOffset != -1 && pw.dataEnd && pw.envIndex == 3) endChannel++;
                    }
                }

            } while (endChannel < totalChannel);


            makeFooter();


            return dat.ToArray();
        }

        private void procEnvelope(partWork pw)
        {
            //Envelope処理
            if (
                (pw.chip is YM2610B && (pw.Type == enmChannelType.SSG || pw.Type == enmChannelType.ADPCMA || pw.Type == enmChannelType.ADPCMB))
                || pw.chip is SN76489
                || pw.chip is RF5C164
                )
            {
                envelope(pw);
            }

            if ((pw.chip is YM2610B && pw.ch < 6) || (pw.chip is YM2612))
            {
                setFmFNum(pw);
                setFmVolume(pw);
            }
            else if (pw.chip is YM2610B && pw.Type == enmChannelType.SSG)
            {
                setSsgFNum(pw);
                setSsgVolume(pw);
            }
            else if (pw.chip is YM2610B && pw.Type == enmChannelType.ADPCMB)
            {
                setAdpcmBFNum(pw);
                setAdpcmBVolume(pw);
            }
            else if (pw.chip is SN76489)
            {
                if (pw.waitKeyOnCounter > 0 || pw.envIndex != -1)
                {
                    setPsgFNum(pw);
                    setPsgVolume(pw);
                }
            }
            else if (pw.chip is RF5C164)
            {
                if (pw.waitKeyOnCounter > 0 || pw.envIndex != -1)
                {
                    setRf5c164FNum(pw);
                    setRf5c164Volume(pw);
                }
            }
        }

        private void procLfo(partWork cpw)
        {
            //lfo処理
            for (int lfo = 0; lfo < 4; lfo++)
            {
                clsLfo pl = cpw.lfo[lfo];

                if (!pl.sw)
                {
                    continue;
                }
                if (pl.waitCounter > 0)//== -1)
                {
                    continue;
                }

                if (pl.type == eLfoType.Hardware)
                {
                    if (cpw.chip is YM2612)
                    {
                        cpw.ams = pl.param[3];
                        cpw.fms = pl.param[2];
                        outOPNSetPanAMSFMS(cpw, cpw.pan, cpw.ams, cpw.fms);
                        cpw.chip.lstPartWork[0].hardLfoSw = true;
                        cpw.chip.lstPartWork[0].hardLfoNum = pl.param[1];
                        outOPNSetHardLfo(cpw, cpw.hardLfoSw, cpw.hardLfoNum);
                        pl.waitCounter = -1;
                    }
                    continue;
                }

                switch (pl.param[4])
                {
                    case 0: //三角
                        pl.value += Math.Abs(pl.param[2]) * pl.direction;
                        pl.waitCounter = pl.param[1];
                        if ((pl.direction > 0 && pl.value >= pl.param[3]) || (pl.direction < 0 && pl.value <= -pl.param[3]))
                        {
                            pl.value = pl.param[3] * pl.direction;
                            pl.direction = -pl.direction;
                        }
                        break;
                    case 1: //のこぎり
                        pl.value += Math.Abs(pl.param[2]) * pl.direction;
                        pl.waitCounter = pl.param[1];
                        if ((pl.direction > 0 && pl.value >= pl.param[3]) || (pl.direction < 0 && pl.value <= -pl.param[3]))
                        {
                            pl.value = -pl.param[3] * pl.direction;
                        }
                        break;
                    case 2: //矩形
                        pl.value = pl.param[3] * pl.direction;
                        pl.waitCounter = pl.param[1];
                        pl.direction = -pl.direction;
                        break;
                    case 3: //ワンショット
                        pl.value += Math.Abs(pl.param[2]) * pl.direction;
                        pl.waitCounter = pl.param[1];
                        if ((pl.direction > 0 && pl.value >= pl.param[3]) || (pl.direction < 0 && pl.value <= -pl.param[3]))
                        {
                            pl.waitCounter = -1;
                        }
                        break;
                    case 4: //ランダム
                        pl.value = rnd.Next(-pl.param[3], pl.param[3]);
                        pl.waitCounter = pl.param[1];
                        break;
                }

            }
        }

        private static void procBend(partWork pw)
        {
            //bend処理
            if (pw.bendWaitCounter == 0)
            {
                if (pw.bendList.Count > 0)
                {
                    Tuple<int, int> bp = pw.bendList.Pop();
                    pw.bendFnum = bp.Item1;
                    pw.bendWaitCounter = bp.Item2;
                }
                else
                {
                    pw.bendWaitCounter = -1;
                }
            }
        }

        private void procKeyOff(partWork pw)
        {
            if (pw.waitKeyOnCounter == 0)
            {
                if (pw.chip is YM2610B)
                {
                    if (!pw.tie)
                    {
                        if (pw.ch < 9) outFmKeyOff(pw);
                        else if (pw.Type == enmChannelType.SSG)
                        {
                            if (!pw.envelopeMode)
                            {
                                outSsgKeyOff(pw);
                            }
                            else
                            {
                                if (pw.envIndex != -1)
                                {
                                    pw.envIndex = 3;//RR phase
                                    pw.envCounter = 0;
                                }
                            }
                        }
                        else if (pw.Type == enmChannelType.ADPCMA)
                        {
                            pw.keyOn = false;
                            pw.keyOff = true;
                            //((YM2610B)pw.chip).adpcmA_KeyOff |= (byte)(1 << (pw.ch - 12));
                        }
                        else if (pw.Type == enmChannelType.ADPCMB)
                        {
                            if (!pw.envelopeMode)
                            {
                                outAdpcmBKeyOff(pw);
                            }
                            else
                            {
                                if (pw.envIndex != -1)
                                {
                                    pw.envIndex = 3;//RR phase
                                    pw.envCounter = 0;
                                }
                            }
                        }
                    }
                }
                else if (pw.chip is YM2612)
                {
                    if (!pw.tie)
                    {
                        outFmKeyOff(pw);
                    }
                }
                else if (pw.chip is SN76489)
                {
                    if (!pw.envelopeMode)
                    {
                        if (!pw.tie) outPsgKeyOff(pw);
                    }
                    else
                    {
                        if (pw.envIndex != -1)
                        {
                            if (!pw.tie)
                            {
                                pw.envIndex = 3;//RR phase
                                pw.envCounter = 0;
                            }
                        }
                    }
                }
                else
                {
                    if (!pw.envelopeMode)
                    {
                        if (!pw.tie) outRf5c164KeyOff(pw);
                    }
                    else
                    {
                        if (pw.envIndex != -1)
                        {
                            if (!pw.tie)
                            {
                                pw.envIndex = 3;//RR phase
                                pw.envCounter = 0;
                            }
                        }
                    }
                }


                //次回に引き継ぎリセット
                pw.beforeTie = pw.tie;
                pw.tie = false;

                //ゲートタイムカウンターをリセット
                pw.waitKeyOnCounter = -1;
            }
        }

        private void partInit()
        {
            foreach (clsChip chip in chips)
            {

                chip.use = false;
                chip.lstPartWork = new List<partWork>();

                for (int i = 0; i < chip.ChMax; i++)
                {
                    partWork pw = new partWork();
                    pw.chip = chip;
                    pw.isSecondary = (chip.ChipID == 1);
                    pw.ch = i;// + 1;
                    if (partData.ContainsKey(chip.Ch[i].Name))
                    {
                        pw.pData = partData[chip.Ch[i].Name];
                    }
                    pw.aData = aliesData;
                    pw.setPos(0);
                    pw.Type = chip.Ch[i].Type;
                    pw.slots = 0;
                    pw.volume = 32767;

                    if (chip is YM2612)
                    {
                        pw.slots = (byte)(((pw.Type == enmChannelType.FMOPN || pw.Type == enmChannelType.FMPCM) || i == 2) ? 0xf : 0x0);
                        pw.volume = 127;
                        pw.MaxVolume = 127;
                        pw.port0 = (byte)(0x2 | (pw.isSecondary ? 0xa0 : 0x50));
                        pw.port1 = (byte)(0x3 | (pw.isSecondary ? 0xa0 : 0x50));
                    }
                    else if (chip is SN76489)
                    {
                        pw.MaxVolume = 15;
                        pw.volume = pw.MaxVolume;
                        pw.port0 = 0x50;
                    }
                    else if (chip is RF5C164)
                    {
                        pw.MaxVolume = 255;
                        pw.volume = pw.MaxVolume;
                        pw.port0 = 0xb1;
                    }
                    else if (chip is YM2610B)
                    {
                        pw.slots = (byte)((pw.Type == enmChannelType.FMOPN || i == 2) ? 0xf : 0x0);
                        pw.volume = 127;
                        pw.MaxVolume = 127;
                        if (pw.Type == enmChannelType.SSG)
                        {
                            //pw.volume = 32767;
                            pw.MaxVolume = 15;
                            pw.volume = pw.MaxVolume;
                        }
                        else if (pw.Type == enmChannelType.ADPCMA)
                        {
                            //pw.volume = 32767;
                            pw.MaxVolume = 31;//5bit
                            pw.volume = pw.MaxVolume;
                        }
                        else if (pw.Type == enmChannelType.ADPCMB)
                        {
                            //pw.volume = 32767;
                            pw.MaxVolume = 255;
                            pw.volume = pw.MaxVolume;
                        }
                        pw.port0 = (byte)(0x8 | (pw.isSecondary ? 0xa0 : 0x50));
                        pw.port1 = (byte)(0x9 | (pw.isSecondary ? 0xa0 : 0x50));
                    }

                    pw.PartName = chip.Ch[i].Name;
                    pw.waitKeyOnCounter = -1;
                    pw.waitCounter = 0;
                    pw.freq = -1;

                    pw.dataEnd = false;
                    if (pw.pData == null || pw.pData.Count < 1)
                    {
                        pw.dataEnd = true;
                    }
                    else
                    {
                        chip.use = true;
                    }

                    chip.lstPartWork.Add(pw);

                }
            }

        }

        private void envelope(partWork pw)
        {
            if (!pw.envelopeMode)
            {
                return;
            }

            if (pw.envIndex == -1)
            {
                return;
            }

            int maxValue = pw.MaxVolume;

            while (pw.envCounter == 0 && pw.envIndex != -1)
            {
                switch (pw.envIndex)
                {
                    case 0: //AR phase
                        pw.envVolume += pw.envelope[7]; // vol += ST
                        if (pw.envVolume >= maxValue)
                        {
                            pw.envVolume = maxValue;
                            pw.envCounter = pw.envelope[3]; // DR
                            pw.envIndex++;
                            break;
                        }
                        pw.envCounter = pw.envelope[2]; // AR
                        break;
                    case 1: //DR phase
                        pw.envVolume -= pw.envelope[7]; // vol -= ST
                        if (pw.envVolume <= pw.envelope[4]) // vol <= SL
                        {
                            pw.envVolume = pw.envelope[4];
                            pw.envCounter = pw.envelope[5]; // SR
                            pw.envIndex++;
                            break;
                        }
                        pw.envCounter = pw.envelope[3]; // DR
                        break;
                    case 2: //SR phase
                        pw.envVolume -= pw.envelope[7]; // vol -= ST
                        if (pw.envVolume <= 0) // vol <= 0
                        {
                            pw.envVolume = 0;
                            pw.envCounter = 0;
                            pw.envIndex = -1;
                            break;
                        }
                        pw.envCounter = pw.envelope[5]; // SR
                        break;
                    case 3: //RR phase
                        pw.envVolume -= pw.envelope[7]; // vol -= ST
                        if (pw.envVolume <= 0) // vol <= 0
                        {
                            pw.envVolume = 0;
                            pw.envCounter = 0;
                            pw.envIndex = -1;
                            break;
                        }
                        pw.envCounter = pw.envelope[6]; // RR
                        break;
                }
            }

            if (pw.envIndex == -1)
            {
                if (pw.chip is SN76489)
                {
                    outPsgKeyOff(pw);
                }
                else if (pw.chip is YM2610B && pw.Type == enmChannelType.SSG)
                {
                    outSsgKeyOff(pw);
                }
                else if (pw.chip is YM2610B && pw.Type == enmChannelType.ADPCMB)
                {
                    outAdpcmBKeyOff(pw);
                }
                else if (pw.chip is RF5C164)
                {
                    outRf5c164KeyOff(pw);
                }
            }
        }


        private void commander(partWork pw, char cmd)
        {

            switch (cmd)
            {
                case ' ':
                case '\t':
                    pw.incPos();
                    break;
                case '!': // CompileSkip
                    pw.dataEnd = true;
                    pw.waitCounter = -1;
                    break;
                case 'T': // tempo
                    cmdTempo(pw);
                    break;
                case '@': // instrument
                    cmdInstrument(pw);
                    break;
                case 'v': // volume
                    cmdVolume(pw);
                    break;
                case 'V': // totalVolume(Adpcm-A / Rhythm)
                    cmdTotalVolume(pw);
                    break;
                case 'o': // octave
                    cmdOctave(pw);
                    break;
                case '>': // octave Up
                    cmdOctaveUp(pw);
                    break;
                case '<': // octave Down
                    cmdOctaveDown(pw);
                    break;
                case ')': // volume Up
                    cmdVolumeUp(pw);
                    break;
                case '(': // volume Down
                    cmdVolumeDown(pw);
                    break;
                case 'l': // length
                    cmdLength(pw);
                    break;
                case '#': // length(clock)
                    cmdClockLength(pw);
                    break;
                case 'p': // pan
                    cmdPan(pw);
                    break;
                case 'D': // Detune
                    cmdDetune(pw);
                    break;
                case 'm': // pcm mode
                    cmdMode(pw);
                    break;
                case 'q': // gatetime
                    cmdGatetime(pw);
                    break;
                case 'Q': // gatetime
                    cmdGatetime2(pw);
                    break;
                case 'E': // envelope
                    cmdEnvelope(pw);
                    break;
                case 'L': // loop point
                    cmdLoop(pw);
                    break;
                case '[': // repeat
                    cmdRepeatStart(pw);
                    break;
                case ']': // repeat
                    cmdRepeatEnd(pw);
                    break;
                case '/': // repeat
                    cmdRepeatExit(pw);
                    break;
                case 'M': // lfo
                    cmdLfo(pw);
                    break;
                case 'S': // lfo switch
                    cmdLfoSwitch(pw);
                    break;
                case 'y': // y
                    cmdY(pw);
                    break;
                case 'w': // noise
                    cmdNoise(pw);
                    break;
                case 'P': // noise or tone mixer
                    cmdMixer(pw);
                    break;
                case 'K': // key shift
                    cmdKeyShift(pw);
                    break;
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                case 'g':
                case 'a':
                case 'b':
                case 'r':
                    cmdNote(pw, cmd);
                    break;
                default:
                    msgBox.setErrMsg(string.Format("未知のコマンド{0}を検出しました。", cmd), pw.getLineNumber());
                    pw.incPos();
                    break;
            }
        }

        private void cmdNote(partWork pw, char cmd)
        {
            pw.incPos();

            //+ -の解析
            int shift = 0;
            while (pw.getChar() == '+' || pw.getChar() == '-')
            {
                shift += pw.getChar() == '+' ? 1 : -1;
                pw.incPos();
            }
            if (cmd == 'r' && shift != 0)
            {
                msgBox.setWrnMsg("休符での+、-の指定は無視されます。", lineNumber);
            }

            int ml = 0;
            int n = -1;
            bool isMinus = false;
            bool isSecond = false;
            do
            {
                int m = 0;

                //数値の解析
                if (!pw.getNum(out n))
                {
                    if (!isSecond)
                        n = (int)pw.length;
                    else if (!isMinus)
                    {
                        //タイとして'&'が使用されている
                        pw.tie = true;
                    }
                }
                else
                {
                    if ((int)clockCount % n != 0)
                    {
                        msgBox.setWrnMsg(string.Format("割り切れない音長({0})の指定があります。音長は不定になります。", n), lineNumber);
                    }
                    n = (int)clockCount / n;
                }

                if (!pw.tie)
                {
                    m += n;

                    //符点の解析
                    while (pw.getChar() == '.')
                    {
                        if (n % 2 != 0)
                        {
                            msgBox.setWrnMsg("割り切れない.の指定があります。音長は不定です。", lineNumber);
                        }
                        n = n / 2;
                        m += n;
                        pw.incPos();
                    }


                    if (isMinus) ml -= m;
                    else ml += m;
                }

                //ベンドの解析
                int bendDelayCounter = 0;
                int bendShift = 0;
                if (pw.getChar() == '_')
                {
                    pw.incPos();
                    pw.octaveNow = pw.octaveNew;
                    pw.bendOctave = pw.octaveNow;
                    pw.bendNote = 'r';
                    pw.bendWaitCounter = -1;
                    bool loop = true;
                    while (loop)
                    {
                        char bCmd = pw.getChar();
                        switch (bCmd)
                        {
                            case 'c':
                            case 'd':
                            case 'e':
                            case 'f':
                            case 'g':
                            case 'a':
                            case 'b':
                                loop = false;
                                pw.incPos();
                                //+ -の解析
                                bendShift = 0;
                                while (pw.getChar() == '+' || pw.getChar() == '-')
                                {
                                    bendShift += pw.getChar() == '+' ? 1 : -1;
                                    pw.incPos();
                                }
                                pw.bendShift = bendShift;
                                bendDelayCounter = 0;
                                n = -1;
                                isMinus = false;
                                isSecond = false;
                                do
                                {
                                    m = 0;

                                    //数値の解析
                                    if (!pw.getNum(out n))
                                    {
                                        if (!isSecond)
                                        {
                                            n = 0;
                                            break;
                                        }
                                        else if (!isMinus)
                                        {
                                            //タイとして'&'が使用されている
                                            pw.tie = true;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if ((int)clockCount % n != 0)
                                        {
                                            msgBox.setWrnMsg(string.Format("割り切れない音長({0})の指定があります。音長は不定になります。", n), lineNumber);
                                        }
                                        n = (int)clockCount / n;
                                    }

                                    if (!pw.tie)
                                    {
                                        bendDelayCounter += n;

                                        //符点の解析
                                        while (pw.getChar() == '.')
                                        {
                                            if (n % 2 != 0)
                                            {
                                                msgBox.setWrnMsg("割り切れない.の指定があります。音長は不定です。", lineNumber);
                                            }
                                            n = n / 2;
                                            m += n;
                                            pw.incPos();
                                        }


                                        if (isMinus) bendDelayCounter -= m;
                                        else bendDelayCounter += m;
                                    }

                                    if (pw.getChar() == '&')
                                    {
                                        isMinus = false;
                                    }
                                    else if (pw.getChar() == '~')
                                    {
                                        isMinus = true;
                                    }
                                    else
                                    {
                                        break;
                                    }

                                    isSecond = true;
                                    pw.incPos();

                                } while (true);

                                if (cmd != 'r')
                                {
                                    pw.bendNote = bCmd;
                                    bendDelayCounter = checkRange(bendDelayCounter, 0, ml);
                                    pw.bendWaitCounter = bendDelayCounter;
                                }
                                else
                                {
                                    msgBox.setErrMsg("休符にベンドの指定はできません。", lineNumber);
                                }

                                break;
                            case 'o':
                                pw.incPos();
                                if (!pw.getNum(out n))
                                {
                                    msgBox.setErrMsg("不正なオクターブが指定されています。", lineNumber);
                                    n = 110;
                                }
                                n = checkRange(n, 1, 8);
                                pw.bendOctave = n;
                                break;
                            case '>':
                                pw.incPos();
                                pw.bendOctave++;
                                pw.bendOctave = checkRange(pw.bendOctave, 1, 8);
                                break;
                            case '<':
                                pw.incPos();
                                pw.bendOctave--;
                                pw.bendOctave = checkRange(pw.bendOctave, 1, 8);
                                break;
                            default:
                                loop = false;
                                break;
                        }
                    }

                    //音符の変化量
                    int ed = note.IndexOf(pw.bendNote) + 1 + (pw.bendOctave - 1) * 12 + pw.bendShift;
                    ed = checkRange(ed, 0, 8 * 12 - 1);
                    int st = note.IndexOf(cmd) + 1 + (pw.octaveNow - 1) * 12 + shift;//
                    st = checkRange(st, 0, 8 * 12 - 1);

                    int delta = ed - st;
                    if (delta == 0 || bendDelayCounter == ml)
                    {
                        pw.bendNote = 'r';
                        pw.bendWaitCounter = -1;
                    }
                    else
                    {
                        //１音符当たりのウエイト
                        float wait = (ml - bendDelayCounter - 1) / (float)delta;
                        float tl = 0;
                        float bf = Math.Sign(wait);
                        List<int> lstBend = new List<int>();
                        for (int i = 0; i < Math.Abs(delta); i++)
                        {
                            bf += wait;
                            tl += wait;
                            int a = getPsgFNum(pw.octaveNow, cmd, shift + (i + 0) * Math.Sign(delta));//
                            int b = getPsgFNum(pw.octaveNow, cmd, shift + (i + 1) * Math.Sign(delta));//
                            if (
                                (pw.chip is YM2610B && pw.ch < 9)
                                || (pw.chip is YM2612))
                            {
                                int[] ftbl = (pw.chip is YM2612) ? OPN_FNumTbl_7670454 : OPN_FNumTbl_8000000;

                                a = getFmFNum(ftbl, pw.octaveNow, cmd, shift + (i + 0) * Math.Sign(delta));//
                                b = getFmFNum(ftbl, pw.octaveNow, cmd, shift + (i + 1) * Math.Sign(delta));//
                                int oa = (a & 0xf000) / 0x1000;
                                int ob = (b & 0xf000) / 0x1000;
                                if (oa != ob)
                                {
                                    if ((a & 0xfff) == ftbl[0])
                                    {
                                        oa += Math.Sign(ob - oa);
                                        a = (a & 0xfff) * 2 + oa * 0x1000;
                                    }
                                    else if ((b & 0xfff) == ftbl[0])
                                    {
                                        ob += Math.Sign(oa - ob);
                                        b = (b & 0xfff) * ((delta > 0) ? 2 : 1) + ob * 0x1000;
                                    }
                                }
                            }
                            else if (pw.chip is YM2610B && pw.Type == enmChannelType.SSG)
                            {
                                a = getSsgFNum(pw.octaveNow, cmd, shift + (i + 0) * Math.Sign(delta));//
                                b = getSsgFNum(pw.octaveNow, cmd, shift + (i + 1) * Math.Sign(delta));//
                            }
                            else if (pw.chip is SN76489)
                            {
                                a = getPsgFNum(pw.octaveNow, cmd, shift + (i + 0) * Math.Sign(delta));//
                                b = getPsgFNum(pw.octaveNow, cmd, shift + (i + 1) * Math.Sign(delta));//
                            }
                            else if (pw.chip is RF5C164)
                            {
                                a = getRf5c164PcmNote(pw.octaveNow, cmd, shift + (i + 0) * Math.Sign(delta));//
                                b = getRf5c164PcmNote(pw.octaveNow, cmd, shift + (i + 1) * Math.Sign(delta));//
                            }

                            if (Math.Abs(bf) >= 1.0f)
                            {
                                for (int j = 0; j < (int)Math.Abs(bf); j++)
                                {
                                    int c = b - a;
                                    int d = (int)Math.Abs(bf);
                                    lstBend.Add((int)(a + ((float)c / (float)d) * (float)j));
                                }
                                bf -= (int)bf;
                            }

                        }
                        Stack<Tuple<int, int>> lb = new Stack<Tuple<int, int>>();
                        int of = -1;
                        int cnt = 1;
                        foreach (int f in lstBend)
                        {
                            if (of == f)
                            {
                                cnt++;
                                continue;
                            }
                            lb.Push(new Tuple<int, int>(f, cnt));
                            of = f;
                            cnt = 1;
                        }
                        pw.bendList = new Stack<Tuple<int, int>>();
                        foreach (Tuple<int, int> lbt in lb)
                        {
                            pw.bendList.Push(lbt);
                        }
                        Tuple<int, int> t = pw.bendList.Pop();
                        pw.bendFnum = t.Item1;
                        pw.bendWaitCounter = t.Item2;
                    }
                }

                if (pw.getChar() == '&')
                {
                    isMinus = false;
                }
                else if (pw.getChar() == '~')
                {
                    isMinus = true;
                }
                else
                {
                    break;
                }

                isSecond = true;
                pw.incPos();

            } while (true);

            if (ml < 1)
            {
                msgBox.setErrMsg("負の音長が指定されました。", lineNumber);
                ml = (int)pw.length;
            }


            //装飾の解析完了


            //WaitClockの決定
            pw.waitCounter = ml;

            if (cmd != 'r')
            {

                //発音周波数
                if (pw.bendWaitCounter == -1)
                {
                    pw.octaveNow = pw.octaveNew;
                    pw.noteCmd = cmd;
                    pw.shift = shift;
                }
                else
                {
                    pw.octaveNew = pw.bendOctave;//
                    pw.octaveNow = pw.bendOctave;//
                    pw.noteCmd = pw.bendNote;
                    pw.shift = pw.bendShift;
                }

                //発音周波数の決定とキーオン
                if (pw.chip is YM2610B)
                {

                    //YM2610B

                    if (pw.Type == enmChannelType.FMOPN || pw.Type == enmChannelType.FMOPNex)
                    {
                        setFmFNum(pw);

                        //タイ指定では無い場合はキーオンする
                        if (!pw.beforeTie)
                        {
                            setLfoAtKeyOn(pw);
                            setFmVolume(pw);
                            outFmKeyOn(pw);
                        }
                    }
                    else if (pw.Type == enmChannelType.SSG)
                    {
                        setSsgFNum(pw);

                        //タイ指定では無い場合はキーオンする
                        if (!pw.beforeTie)
                        {
                            setEnvelopeAtKeyOn(pw);
                            setLfoAtKeyOn(pw);
                            outSsgKeyOn(pw);
                        }
                    }
                    else if (pw.Type == enmChannelType.ADPCMA)
                    {
                        if (!pw.beforeTie)
                        {

                            pw.keyOn = true;
                            //if ((((YM2610B)pw.chip).adpcmA_KeyOn & (1 << (pw.ch - 12))) != 0)
                            //{
                            //    ((YM2610B)pw.chip).adpcmA_KeyOff |= (byte)(1 << (pw.ch - 12));
                            //    ((YM2610B)pw.chip).adpcmA_beforeKeyOn &= (byte)~(1 << (pw.ch - 12));
                            //}
                            //((YM2610B)pw.chip).adpcmA_KeyOn |= (byte)(1 << (pw.ch - 12));

                        }
                    }
                    else if (pw.Type == enmChannelType.ADPCMB)
                    {
                        setAdpcmBFNum(pw);

                        //タイ指定では無い場合はキーオンする
                        if (!pw.beforeTie)
                        {
                            setEnvelopeAtKeyOn(pw);
                            setLfoAtKeyOn(pw);
                            outAdpcmBKeyOn(pw);
                        }
                    }
                }
                else if (pw.chip is YM2612)
                {

                    //YM2612

                    if (!pw.pcm)
                    {
                        setFmFNum(pw);
                    }
                    else
                    {
                        getPcmNote(pw);
                    }
                    //タイ指定では無い場合はキーオンする
                    if (!pw.beforeTie)
                    {
                        setLfoAtKeyOn(pw);
                        setFmVolume(pw);
                        outFmKeyOn(pw);
                    }
                }
                else if (pw.chip is SN76489)
                {

                    // SN76489

                    setPsgFNum(pw);

                    //タイ指定では無い場合はキーオンする
                    if (!pw.beforeTie)
                    {
                        setEnvelopeAtKeyOn(pw);
                        setLfoAtKeyOn(pw);
                        outPsgKeyOn(pw);
                    }
                }
                else if (pw.chip is RF5C164)
                {

                    // RF5C164

                    setRf5c164FNum(pw);

                    //タイ指定では無い場合はキーオンする
                    if (!pw.beforeTie)
                    {
                        setEnvelopeAtKeyOn(pw);
                        setLfoAtKeyOn(pw);
                        setRf5c164Envelope(pw, pw.volume);
                        outRf5c164KeyOn(pw);
                    }
                }

                //gateTimeの決定
                if (pw.gatetimePmode)
                {
                    pw.waitKeyOnCounter = pw.waitCounter * pw.gatetime / 8L;
                }
                else
                {
                    pw.waitKeyOnCounter = pw.waitCounter - pw.gatetime;
                }
                if (pw.waitKeyOnCounter < 1) pw.waitKeyOnCounter = 1;

                //PCM専用のWaitClockの決定
                if (pw.pcm)
                {
                    pw.pcmWaitKeyOnCounter = -1;
                    if (Version != 1.60f)
                    {
                        pw.pcmWaitKeyOnCounter = pw.waitKeyOnCounter;
                    }
                    pw.pcmSizeCounter = instPCM[pw.instrument].size;
                }
            }

            pw.clockCounter += pw.waitCounter;
        }

        private void cmdRepeatExit(partWork pw)
        {
            int n = -1;
            pw.incPos();
            clsRepeat rx = pw.stackRepeat.Pop();
            if (rx.repeatCount == 1)
            {
                int i = 0;
                while (true)
                {
                    char c = pw.getChar();
                    if (c == ']')
                    {
                        if (i == 0)
                        {
                            break;
                        }
                        else
                            i--;
                    }
                    else if (c == '[')
                    {
                        i++;
                    }
                    pw.incPos();
                }
                pw.incPos();
                pw.getNum(out n);
            }
            else
            {
                pw.stackRepeat.Push(rx);
            }

        }

        private void cmdRepeatEnd(partWork pw)
        {
            int n;
            pw.incPos();
            if (!pw.getNum(out n))
            {
                n = 2;
            }
            n = checkRange(n, 1, 255);
            try
            {
                clsRepeat re = pw.stackRepeat.Pop();
                if (re.repeatCount == -1)
                {
                    //初回
                    re.repeatCount = n;
                }
                re.repeatCount--;
                if (re.repeatCount > 0)
                {
                    pw.stackRepeat.Push(re);
                    pw.setPos(re.pos);
                }
            }
            catch
            {
                msgBox.setWrnMsg("[と]の数があいません。", lineNumber);
            }
        }

        private void cmdRepeatStart(partWork pw)
        {
            pw.incPos();
            clsRepeat rs = new clsRepeat();
            rs.pos = pw.getPos();
            rs.repeatCount = -1;//初期値
            pw.stackRepeat.Push(rs);
        }

        private void cmdLoop(partWork pw)
        {
            pw.incPos();
            loopOffset = (long)dat.Count;
            loopSamples = lSample;

            foreach (clsChip chip in chips)
            {
                foreach (partWork p in chip.lstPartWork)
                {
                    p.freq = -1;

                    if (p.chip is RF5C164 && rf5c164[p.isSecondary ? 1 : 0].use)
                    {
                        //rf5c164の設定済み周波数値を初期化(ループ時に直前の周波数を引き継いでしまうケースがあるため)
                        p.rf5c164AddressIncrement = -1;
                        int n = p.instrument;
                        p.pcmStartAddress = -1;
                        p.pcmLoopAddress = -1;
                        if (n != -1)
                        {
                            setRf5c164CurrentChannel(p);
                            setRf5c164SampleStartAddress(p, (int)instPCM[n].stAdr);
                            setRf5c164LoopAddress(p, (int)(instPCM[n].loopAdr));
                        }
                    }
                }
            }
        }

        private void cmdEnvelope(partWork pw)
        {
            int n = -1;
            if (
                ((pw.chip is YM2610B) && (pw.Type == enmChannelType.SSG || pw.Type == enmChannelType.FMOPNex))
                || ((pw.chip is YM2612) && (pw.Type == enmChannelType.FMPCM || (pw.Type == enmChannelType.FMOPNex)))
                || (pw.chip is SN76489)
                || (pw.chip is RF5C164)
                )
            {
                pw.incPos();
                switch (pw.getChar())
                {
                    case 'O':
                        pw.incPos();
                        if (
                            ((pw.chip is YM2610B) && pw.Type == enmChannelType.FMOPNex)
                            || pw.chip is YM2612
                            )
                        {
                            switch (pw.getChar())
                            {
                                case 'N':
                                    pw.incPos();
                                    pw.Ch3SpecialMode = true;
                                    outOPNSetCh3SpecialMode(pw, true);
                                    break;
                                case 'F':
                                    pw.incPos();
                                    pw.Ch3SpecialMode = false;
                                    outOPNSetCh3SpecialMode(pw, false);
                                    break;
                                default:
                                    msgBox.setErrMsg(string.Format("未知のコマンド(EO{0})が指定されました。", pw.getChar()), lineNumber);
                                    pw.incPos();
                                    break;
                            }
                        }
                        else
                        {
                            switch (pw.getChar())
                            {
                                case 'N':
                                    pw.incPos();
                                    pw.envelopeMode = true;
                                    break;
                                case 'F':
                                    pw.incPos();
                                    pw.envelopeMode = false;
                                    if (pw.Type == enmChannelType.SSG)
                                    {
                                        pw.beforeVolume = -1;
                                    }
                                    break;
                                default:
                                    msgBox.setErrMsg(string.Format("未知のコマンド(EO{0})が指定されました。", pw.getChar()), lineNumber);
                                    pw.incPos();
                                    break;
                            }
                        }
                        break;
                    case 'X':
                        char c = pw.getChar();
                        if (
                            ((pw.chip is YM2610B) && pw.Type == enmChannelType.FMOPNex)
                            || pw.chip is YM2612
                            )
                        {
                            pw.incPos();
                            if (!pw.getNum(out n))
                            {
                                msgBox.setErrMsg("不正なスロット指定'EX'が指定されています。", lineNumber);
                                n = 0;
                            }
                            byte res = 0;
                            while (n % 10 != 0)
                            {
                                if (n % 10 > 0 && n % 10 < 5)
                                {
                                    res += (byte)(1 << (n % 10 - 1));
                                }
                                else
                                {
                                    msgBox.setErrMsg(string.Format("未知のコマンド(EX{0})が指定されました。", n), lineNumber);
                                    break;
                                }
                                n /= 10;
                            }
                            if (res != 0)
                            {
                                pw.slots = res;
                            }
                        }
                        else
                        {
                            msgBox.setErrMsg("未知のコマンド(EX)が指定されました。", lineNumber);
                        }
                        break;
                    default:
                        if (pw.Type == enmChannelType.FMOPNex)
                        {
                            int[] s = new int[] { 0, 0, 0, 0 };

                            for (int i = 0; i < 4; i++)
                            {
                                if (pw.getNum(out n))
                                {
                                    s[i] = n;
                                }
                                else
                                {
                                    msgBox.setErrMsg("Eコマンドの解析に失敗しました。", lineNumber);
                                    break;
                                }
                                if (i == 3) break;
                                pw.incPos();
                            }
                            pw.slotDetune = s;
                            break;
                        }
                        else
                        {
                            msgBox.setErrMsg(string.Format("未知のコマンド(E{0})が指定されました。", pw.getChar()), lineNumber);
                            pw.incPos();
                        }
                        break;
                }
            }
            else
            {
                msgBox.setWrnMsg("このパートは効果音モードに対応したチャンネルが指定されていないため、Eコマンドは無視されます。", lineNumber);
                pw.incPos();
            }

        }

        private void cmdGatetime2(partWork pw)
        {
            int n;
            pw.incPos();
            if (!pw.getNum(out n))
            {
                msgBox.setErrMsg("不正なゲートタイム指定'Q'が指定されています。", lineNumber);
                n = 1;
            }
            n = checkRange(n, 1, 8);
            pw.gatetime = n;
            pw.gatetimePmode = true;
        }

        private void cmdGatetime(partWork pw)
        {
            int n;
            pw.incPos();
            if (!pw.getNum(out n))
            {
                msgBox.setErrMsg("不正なゲートタイム指定'q'が指定されています。", lineNumber);
                n = 0;
            }
            n = checkRange(n, 0, 255);
            pw.gatetime = n;
            pw.gatetimePmode = false;
        }

        private void cmdMode(partWork pw)
        {
            int n;
            pw.incPos();
            if (pw.chip is YM2612 && pw.Type == enmChannelType.FMPCM)
            {
                if (!pw.getNum(out n))
                {
                    msgBox.setErrMsg("不正なPCMモード指定'm'が指定されています。", lineNumber);
                    n = 0;
                }
                n = checkRange(n, 0, 1);
                pw.pcm = (n == 1);
                pw.freq = -1;//freqをリセット
                outYM2612SetCh6PCMMode(pw, pw.pcm);
            }
            else
            {
                pw.getNum(out n);
                msgBox.setWrnMsg("このパートは6chではないため、mコマンドは無視されます。", lineNumber);
            }

        }

        private void cmdDetune(partWork pw)
        {
            int n;
            pw.incPos();
            if (!pw.getNum(out n))
            {
                msgBox.setErrMsg("不正なディチューン'D'が指定されています。", lineNumber);
                n = 0;
            }

            if (pw.Type == enmChannelType.FMOPM
                || pw.Type == enmChannelType.FMOPN
                || pw.Type == enmChannelType.FMOPNex
                || pw.Type == enmChannelType.Multi
                || pw.Type == enmChannelType.DCSG
                || pw.Type == enmChannelType.DCSGNOISE
                || pw.Type == enmChannelType.SSG
                )
            {
                n = checkRange(n, -127, 127);
            }

            pw.detune = n;
        }

        private void cmdPan(partWork pw)
        {
            int n;
            int vch = pw.ch;

            pw.incPos();
            if (pw.chip is YM2610B)
            {
                if (pw.Type == enmChannelType.FMOPN || pw.Type == enmChannelType.FMOPNex)
                {
                    //効果音モードのチャンネル番号を指定している場合は3chへ変更する
                    if (pw.ch >= 6 && pw.ch <= 8)
                    {
                        vch = 2;
                    }

                    if (!pw.getNum(out n))
                    {
                        msgBox.setErrMsg("不正なパン'p'が指定されています。", lineNumber);
                        n = 3;
                    }
                    //強制的にモノラルにする
                    if (monoPart != null && monoPart.Contains(ym2612[0].Ch[5].Name))
                    {
                        n = 3;
                    }
                    n = checkRange(n, 1, 3);
                    pw.pan = n;
                    outOPNSetPanAMSFMS(pw, n, pw.ams, pw.fms);
                }
                else if (pw.Type == enmChannelType.ADPCMA)
                {
                    if (!pw.getNum(out n))
                    {
                        msgBox.setErrMsg("不正なパン'p'が指定されています。", lineNumber);
                        n = 3;
                    }
                    n = checkRange(n, 0, 3);
                    pw.pan = n;
                }
                else if (pw.Type == enmChannelType.ADPCMB)
                {
                    if (!pw.getNum(out n))
                    {
                        msgBox.setErrMsg("不正なパン'p'が指定されています。", lineNumber);
                        n = 3;
                    }
                    n = checkRange(n, 0, 3);
                    setAdpcmBPan(pw, n);
                }
            }
            else if (pw.chip is YM2612)
            {
                //効果音モードのチャンネル番号を指定している場合は3chへ変更する
                if (pw.ch >= 6 && pw.ch <= 8)
                {
                    vch = 2;
                }

                if (!pw.getNum(out n))
                {
                    msgBox.setErrMsg("不正なパン'p'が指定されています。", lineNumber);
                    n = 10;
                }
                //強制的にモノラルにする
                if (monoPart != null && monoPart.Contains(ym2612[0].Ch[5].Name))
                {
                    n = 3;
                }
                n = checkRange(n, 1, 3);
                pw.pan = n;
                outOPNSetPanAMSFMS(pw, n, pw.ams, pw.fms);
            }
            else if (pw.chip is SN76489)
            {
                pw.getNum(out n);
                msgBox.setWrnMsg("PSGパートでは、pコマンドは無視されます。", lineNumber);
            }
            else if (pw.chip is RF5C164)
            {
                int l;
                int r;
                if (!pw.getNum(out l))
                {
                    msgBox.setErrMsg("不正なパン'p'が指定されています。", lineNumber);
                    l = 15;
                }
                if (pw.getChar() != ',')
                {
                    msgBox.setErrMsg("不正なパン'p'が指定されています。", lineNumber);
                    l = 15;
                    r = 15;
                }
                pw.incPos();
                if (!pw.getNum(out r))
                {
                    msgBox.setErrMsg("不正なパン'p'が指定されています。", lineNumber);
                    r = 15;
                }
                l = checkRange(l, 0, 15);
                r = checkRange(r, 0, 15);
                pw.pan = (r << 4) | l;
                setRf5c164CurrentChannel(pw);
                setRf5c164Pan(pw, pw.pan);
            }

        }

        private void cmdLength(partWork pw)
        {
            int n;
            pw.incPos();
            if (!pw.getNum(out n))
            {
                msgBox.setErrMsg("不正な音長が指定されています。", lineNumber);
                n = 10;
            }
            n = checkRange(n, 1, 128);
            pw.length = clockCount / n;
        }

        private void cmdClockLength(partWork pw)
        {
            int n;
            pw.incPos();
            if (!pw.getNum(out n))
            {
                msgBox.setErrMsg("不正な音長が指定されています。", lineNumber);
                n = 10;
            }
            n = checkRange(n, 1, 65535);
            pw.length = n;
        }

        private void cmdVolumeDown(partWork pw)
        {
            int n;
            pw.incPos();
            if (!pw.getNum(out n))
            {
                msgBox.setErrMsg("不正な音量'('が指定されています。", lineNumber);
                n = 10;
            }
            n = checkRange(n, 1, pw.MaxVolume);
            pw.volume -= n;
            pw.volume = checkRange(pw.volume, 0, pw.MaxVolume);

        }

        private void cmdVolumeUp(partWork pw)
        {
            int n;
            pw.incPos();
            if (!pw.getNum(out n))
            {
                msgBox.setErrMsg("不正な音量')'が指定されています。", lineNumber);
                n = 10;
            }
            n = checkRange(n, 1, pw.MaxVolume);
            pw.volume += n;
            n = checkRange(n, 0, pw.MaxVolume);

        }

        private void cmdOctaveDown(partWork pw)
        {
            pw.incPos();
            pw.octaveNew--;
            pw.octaveNew = checkRange(pw.octaveNew, 1, 8);
        }

        private void cmdOctaveUp(partWork pw)
        {
            pw.incPos();
            pw.octaveNew++;
            pw.octaveNew = checkRange(pw.octaveNew, 1, 8);
        }

        private void cmdOctave(partWork pw)
        {
            int n;
            pw.incPos();
            if (!pw.getNum(out n))
            {
                msgBox.setErrMsg("不正なオクターブが指定されています。", lineNumber);
                n = 110;
            }
            n = checkRange(n, 1, 8);
            pw.octaveNew = n;
        }

        private void cmdTempo(partWork pw)
        {
            int n;
            pw.incPos();
            if (!pw.getNum(out n))
            {
                msgBox.setErrMsg("不正なテンポが指定されています。", lineNumber);
                n = 120;
            }
            n = checkRange(n, 1, 255);
            tempo = n;
            samplesPerClock = vgmSamplesPerSecond * 60 * 4 / (tempo * clockCount);
        }

        private void cmdVolume(partWork pw)
        {
            int n;
            pw.incPos();
            if (!pw.getNum(out n))
            {
                msgBox.setErrMsg("不正な音量が指定されています。", lineNumber);
                n = 110;
            }

            pw.volume = checkRange(n, 0, pw.MaxVolume);
        }

        private void cmdTotalVolume(partWork pw)
        {
            int n;
            pw.incPos();
            if (!pw.getNum(out n))
            {
                msgBox.setErrMsg("不正な音量が指定されています。", lineNumber);
                n = 63;
            }
            if ((pw.chip is YM2610B) && pw.Type== enmChannelType.ADPCMA)
            {
                ((YM2610B)pw.chip).adpcmA_TotalVolume = checkRange(n, 0, ((YM2610B)pw.chip).adpcmA_MAXTotalVolume);
            }

        }

        private void cmdInstrument(partWork pw)
        {
            int n;
            pw.incPos();

            if (pw.chip is YM2610B)
            {
                if (pw.ch < 9)
                {
                    if (!pw.getNum(out n))
                    {
                        msgBox.setErrMsg("不正な音色番号が指定されています。", lineNumber);
                        n = 0;
                    }
                    n = checkRange(n, 0, 255);
                    if (pw.instrument != n)
                    {
                        pw.instrument = n;
                        if (pw.Type == enmChannelType.FMOPNex)
                        {
                            ym2610b[pw.chip.ChipID].lstPartWork[2].instrument = n;
                            ym2610b[pw.chip.ChipID].lstPartWork[6].instrument = n;
                            ym2610b[pw.chip.ChipID].lstPartWork[7].instrument = n;
                            ym2610b[pw.chip.ChipID].lstPartWork[8].instrument = n;
                        }
                        if (!pw.pcm)
                        {
                            outFmSetInstrument(pw, n, pw.volume);
                        }
                        else
                        {
                            if (!instPCM.ContainsKey(n))
                            {
                                msgBox.setErrMsg(string.Format("PCM定義に指定された音色番号({0})が存在しません。", n), lineNumber);
                            }
                            else
                            {
                                if (instPCM[n].chip != enmChipType.YM2610B)
                                {
                                    msgBox.setErrMsg(string.Format("指定された音色番号({0})はYM2610B向けPCMデータではありません。", n), lineNumber);
                                }
                            }
                        }
                    }
                }
                else if (pw.Type == enmChannelType.SSG)
                {
                    n = setEnvelopParamFromInstrument(pw);
                }
                else if (pw.Type == enmChannelType.ADPCMA)
                {
                    if (pw.getChar() != 'E')
                    {
                        if (!pw.getNum(out n))
                        {
                            msgBox.setErrMsg("不正な音色番号が指定されています。", lineNumber);
                            n = 0;
                        }
                        n = checkRange(n, 0, 255);
                        if (!instPCM.ContainsKey(n))
                        {
                            msgBox.setErrMsg(string.Format("PCM定義に指定された音色番号({0})が存在しません。", n), lineNumber);
                        }
                        else
                        {
                            if (instPCM[n].chip != enmChipType.YM2610B || instPCM[n].loopAdr != 0)
                            {
                                msgBox.setErrMsg(string.Format("指定された音色番号({0})はYM2610B向けPCMデータではありません。", n), lineNumber);
                            }
                            pw.instrument = n;
                            setYM2610BADPCMAAddress(pw, (int)instPCM[n].stAdr, (int)instPCM[n].edAdr);
                        }
                    }
                    else
                    {
                        pw.incPos();
                        n = setEnvelopParamFromInstrument(pw);
                    }
                }
                else if (pw.Type == enmChannelType.ADPCMB)
                {
                    if (pw.getChar() != 'E')
                    {
                        if (!pw.getNum(out n))
                        {
                            msgBox.setErrMsg("不正な音色番号が指定されています。", lineNumber);
                            n = 0;
                        }
                        n = checkRange(n, 0, 255);
                        if (!instPCM.ContainsKey(n))
                        {
                            msgBox.setErrMsg(string.Format("PCM定義に指定された音色番号({0})が存在しません。", n), lineNumber);
                        }
                        else
                        {
                            if (instPCM[n].chip != enmChipType.YM2610B || instPCM[n].loopAdr != 1)
                            {
                                msgBox.setErrMsg(string.Format("指定された音色番号({0})はYM2610B向けPCMデータではありません。", n), lineNumber);
                            }
                            pw.instrument = n;
                            setYM2610BADPCMBAddress(pw, (int)instPCM[n].stAdr, (int)instPCM[n].edAdr);
                        }
                    }
                    else
                    {
                        pw.incPos();
                        n = setEnvelopParamFromInstrument(pw);
                    }
                }
            }
            else if (pw.chip is YM2612)
            {
                if (!pw.getNum(out n))
                {
                    msgBox.setErrMsg("不正な音色番号が指定されています。", lineNumber);
                    n = 0;
                }
                n = checkRange(n, 0, 255);
                if (pw.instrument != n)
                {
                    pw.instrument = n;
                    if (pw.Type == enmChannelType.FMOPNex)
                    {
                        ym2612[pw.chip.ChipID].lstPartWork[2].instrument = n;
                        ym2612[pw.chip.ChipID].lstPartWork[6].instrument = n;
                        ym2612[pw.chip.ChipID].lstPartWork[7].instrument = n;
                        ym2612[pw.chip.ChipID].lstPartWork[8].instrument = n;
                    }
                    if (!pw.pcm)
                    {
                        outFmSetInstrument(pw, n, pw.volume);
                    }
                    else
                    {
                        if (!instPCM.ContainsKey(n))
                        {
                            msgBox.setErrMsg(string.Format("PCM定義に指定された音色番号({0})が存在しません。", n), lineNumber);
                        }
                        else
                        {
                            if (instPCM[n].chip != enmChipType.YM2612)
                            {
                                msgBox.setErrMsg(string.Format("指定された音色番号({0})はYM2612向けPCMデータではありません。", n), lineNumber);
                            }
                        }
                    }
                }
            }
            else if (pw.chip is SN76489)
            {
                //pw.incPos();
                n = setEnvelopParamFromInstrument(pw);
            }
            else if (pw.chip is RF5C164)
            {
                if (pw.getChar() != 'E')
                {
                    if (!pw.getNum(out n))
                    {
                        msgBox.setErrMsg("不正な音色番号が指定されています。", lineNumber);
                        n = 0;
                    }
                    n = checkRange(n, 0, 255);
                    if (!instPCM.ContainsKey(n))
                    {
                        msgBox.setErrMsg(string.Format("PCM定義に指定された音色番号({0})が存在しません。", n), lineNumber);
                    }
                    else
                    {
                        if (instPCM[n].chip != enmChipType.RF5C164)
                        {
                            msgBox.setErrMsg(string.Format("指定された音色番号({0})はRF5C164向けPCMデータではありません。", n), lineNumber);
                        }
                        pw.instrument = n;
                        setRf5c164CurrentChannel(pw);
                        setRf5c164SampleStartAddress(pw, (int)instPCM[n].stAdr);
                        setRf5c164LoopAddress(pw, (int)(instPCM[n].loopAdr));
                    }
                }
                else
                {
                    pw.incPos();
                    n = setEnvelopParamFromInstrument(pw);
                }
            }
        }

        private int setEnvelopParamFromInstrument(partWork pw)
        {
            int n;
            if (!pw.getNum(out n))
            {
                msgBox.setErrMsg("不正なエンベロープ番号が指定されています。", lineNumber);
                n = 0;
            }
            n = checkRange(n, 0, 255);
            if (!instENV.ContainsKey(n))
            {
                msgBox.setErrMsg(string.Format("エンベロープ定義に指定された音色番号({0})が存在しません。", n), lineNumber);
            }
            else
            {
                if (pw.envInstrument != n)
                {
                    pw.envInstrument = n;
                    pw.envIndex = -1;
                    pw.envCounter = -1;
                    for (int i = 0; i < instENV[n].Length; i++)
                    {
                        pw.envelope[i] = instENV[n][i];
                    }
                }
            }

            return n;
        }

        private void cmdLfo(partWork pw)
        {

            pw.incPos();
            char c = pw.getChar();
            if (c < 'P' && c > 'S')
            {
                msgBox.setErrMsg("指定できるLFOのチャネルはP,Q,R,Sの4種類です。", lineNumber);
                return;
            }
            c -= 'P';

            pw.incPos();
            char t = pw.getChar();
            if (t != 'T' && t != 'V' && t != 'H')
            {
                msgBox.setErrMsg("指定できるLFOの種類はT,V,Hの3種類です。", lineNumber);
                return;
            }
            pw.lfo[c].type = (t == 'T') ? eLfoType.Tremolo : ((t == 'V') ? eLfoType.Vibrato : eLfoType.Hardware);

            pw.lfo[c].sw = false;
            pw.lfo[c].isEnd = true;

            pw.lfo[c].param = new List<int>();
            int n = -1;
            do
            {
                pw.incPos();
                if (pw.getNum(out n))
                {
                    pw.lfo[c].param.Add(n);
                }
                else
                {
                    msgBox.setErrMsg("LFOの設定値に不正な値が指定されました。", lineNumber);
                    return;
                }

                while (pw.getChar() == '\t' || pw.getChar() == ' ') { pw.incPos(); }

            } while (pw.getChar() == ',');
            if (pw.lfo[c].type == eLfoType.Tremolo || pw.lfo[c].type == eLfoType.Vibrato)
            {
                if (pw.lfo[c].param.Count < 4)
                {
                    msgBox.setErrMsg("LFOの設定に必要なパラメータが足りません。", lineNumber);
                    return;
                }
                if (pw.lfo[c].param.Count > 7)
                {
                    msgBox.setErrMsg("LFOの設定に可能なパラメータ数を超えて指定されました。", lineNumber);
                    return;
                }

                pw.lfo[c].param[0] = checkRange(pw.lfo[c].param[0], 0, (int)clockCount);
                pw.lfo[c].param[1] = checkRange(pw.lfo[c].param[1], 1, 255);
                pw.lfo[c].param[2] = checkRange(pw.lfo[c].param[2], -32768, 32787);
                pw.lfo[c].param[3] = Math.Abs(checkRange(pw.lfo[c].param[3], -32768, 32787));
                if (pw.lfo[c].param.Count > 4)
                {
                    pw.lfo[c].param[4] = checkRange(pw.lfo[c].param[4], 0, 4);
                }
                else
                {
                    pw.lfo[c].param.Add(0);
                }
                if (pw.lfo[c].param.Count > 5)
                {
                    pw.lfo[c].param[5] = checkRange(pw.lfo[c].param[5], 0, 1);
                }
                else
                {
                    pw.lfo[c].param.Add(1);
                }
                if (pw.lfo[c].param.Count > 6)
                {
                    pw.lfo[c].param[6] = checkRange(pw.lfo[c].param[6], -32768, 32787);
                }
                else
                {
                    pw.lfo[c].param.Add(0);
                }

            }
            else
            {
                if (pw.lfo[c].param.Count < 4)
                {
                    msgBox.setErrMsg("LFOの設定に必要なパラメータが足りません。", lineNumber);
                    return;
                }
                if (pw.lfo[c].param.Count > 5)
                {
                    msgBox.setErrMsg("LFOの設定に可能なパラメータ数を超えて指定されました。", lineNumber);
                    return;
                }

                pw.lfo[c].param[0] = checkRange(pw.lfo[c].param[0], 0, (int)clockCount);
                pw.lfo[c].param[1] = checkRange(pw.lfo[c].param[1], 0, 7);
                pw.lfo[c].param[2] = checkRange(pw.lfo[c].param[2], 0, 7);
                pw.lfo[c].param[3] = checkRange(pw.lfo[c].param[3], 0, 3);
                if (pw.lfo[c].param.Count == 5)
                {
                    pw.lfo[c].param[4] = checkRange(pw.lfo[c].param[4], 0, 1);
                }
                else
                {
                    pw.lfo[c].param.Add(1);
                }

            }
            //解析　ここまで

            pw.lfo[c].sw = true;
            pw.lfo[c].isEnd = false;
            pw.lfo[c].value = (pw.lfo[c].param[0] == 0) ? pw.lfo[c].param[6] : 0;//ディレイ中は振幅補正は適用されない
            pw.lfo[c].waitCounter = pw.lfo[c].param[0];
            pw.lfo[c].direction = pw.lfo[c].param[2] < 0 ? -1 : 1;
        }

        private void cmdLfoSwitch(partWork pw)
        {

            pw.incPos();
            char c = pw.getChar();
            if (c < 'P' && c > 'S')
            {
                msgBox.setErrMsg("指定できるLFOのチャネルはP,Q,R,Sの4種類です。", lineNumber);
                return;
            }
            c -= 'P';

            int n = -1;
            pw.incPos();
            if (!pw.getNum(out n))
            {
                msgBox.setErrMsg("LFOの設定値に不正な値が指定されました。", lineNumber);
                return;
            }
            n = checkRange(n, 0, 2);

            //解析　ここまで

            pw.lfo[c].sw = (n == 0) ? false : true;
            if (pw.lfo[c].type == eLfoType.Hardware && pw.lfo[c].param != null)
            {
                if (pw.chip is YM2612)
                {
                    if (pw.lfo[c].param[4] == 0)
                    {
                        outOPNSetHardLfo(pw, (n == 0) ? false : true, pw.lfo[c].param[1]);
                    }
                    else
                    {
                        outOPNSetHardLfo(pw, false, pw.lfo[c].param[1]);
                    }
                }
            }

        }

        private void cmdY(partWork pw)
        {
            int n = -1;
            byte adr = 0;
            byte dat = 0;
            pw.incPos();
            if (pw.getNum(out n))
            {
                adr = (byte)(n & 0xff);
            }
            pw.incPos();
            if (pw.getNum(out n))
            {
                dat = (byte)(n & 0xff);
            }

            if ((pw.chip is YM2610B) || (pw.chip is YM2612))
            {
                outFmAdrPort((pw.ch > 3 && pw.ch < 6) ? pw.port1 : pw.port0, adr, dat);
            }
            else if (pw.chip is SN76489)
            {
                outPsgPort(pw.isSecondary, dat);
            }
            else if (pw.chip is RF5C164)
            {
                outRf5c164Port(pw.isSecondary, adr, dat);
            }
        }

        private void cmdNoise(partWork pw)
        {
            int n = -1;
            pw.incPos();

            if (pw.Type == enmChannelType.DCSGNOISE || pw.Type == enmChannelType.SSG)
            {
                if (pw.getNum(out n))
                {
                    n = checkRange(n, 0, pw.Type == enmChannelType.DCSGNOISE ? 7 : 31);
                    if (pw.Type == enmChannelType.DCSGNOISE)
                    {
                        pw.noise = n; // DCSGの場合は4Chに保存
                    }
                    else
                    {
                        pw.chip.lstPartWork[0].noise = n;//その他SSGの場合は、そのChipの1Chに保存
                        outSsgNoise(pw, n);
                    }
                }
                else
                {
                    msgBox.setErrMsg("wコマンドに指定された値が不正です。", lineNumber);
                    return;

                }
            }
            else
            {
                msgBox.setErrMsg("このチャンネルではwコマンドは使用できません。", lineNumber);
                return;
            }
        }

        private void cmdMixer(partWork pw)
        {
            int n = -1;
            pw.incPos();

            if (pw.Type == enmChannelType.SSG)
            {
                if (pw.getNum(out n))
                {
                    n = checkRange(n, 0, 3);
                    pw.mixer = n;
                }
                else
                {
                    msgBox.setErrMsg("Pコマンドに指定された値が不正です。", lineNumber);
                    return;

                }
            }
            else
            {
                msgBox.setErrMsg("このチャンネルではPコマンドは使用できません。", lineNumber);
                return;
            }
        }

        private void cmdKeyShift(partWork pw)
        {
            int n = -1;
            pw.incPos();

            if (pw.getNum(out n))
            {
                pw.keyShift = checkRange(n, -128, 128);
            }
            else
            {
                msgBox.setErrMsg("Kコマンドに指定された値が不正です。", lineNumber);
                return;

            }
        }

        private void makeHeader()
        {

            //Header
            foreach (byte b in hDat)
            {
                dat.Add(b);
            }

            //PCM Data block
            if ((ym2612[0].use || ym2612[1].use) && ym2612[0].pcmData != null && ym2612[0].pcmData.Length > 0)
            {
                foreach (byte b in ym2612[0].pcmData)
                {
                    dat.Add(b);
                }
            }

            for (int i = 0; i < 2; i++)
            {
                //PCM Data block
                if (rf5c164[i].use && rf5c164[i].pcmData != null && rf5c164[i].pcmData.Length > 0)
                {
                    foreach (byte b in rf5c164[i].pcmData)
                    {
                        dat.Add(b);
                    }
                }

                //ADPCM-A Data block
                if (ym2610b[i].use && ym2610b[i].pcmDataA != null && ym2610b[i].pcmDataA.Length > 0)
                {
                    foreach (byte b in ym2610b[i].pcmDataA)
                    {
                        dat.Add(b);
                    }
                }

                //ADPCM-B Data block
                if (ym2610b[i].use && ym2610b[i].pcmDataB != null && ym2610b[i].pcmDataB.Length > 0)
                {
                    foreach (byte b in ym2610b[i].pcmDataB)
                    {
                        dat.Add(b);
                    }
                }
            }

            for (int i = 0; i < 2; i++)
            {
                if (ym2612[i].use)
                {
                    outOPNSetHardLfo(ym2612[i].lstPartWork[0], false, 0);
                    outOPNSetCh3SpecialMode(ym2612[i].lstPartWork[0], false);
                    outYM2612SetCh6PCMMode(ym2612[i].lstPartWork[0], false);

                    outFmAllKeyOff(ym2612[i]);

                    foreach (partWork pw in ym2612[i].lstPartWork)
                    {
                        if (pw.ch == 0)
                        {
                            pw.hardLfoSw = false;
                            pw.hardLfoNum = 0;
                            outOPNSetHardLfo(pw, pw.hardLfoSw, pw.hardLfoNum);
                        }

                        if (pw.ch < 6)
                        {
                            pw.pan = 3;
                            pw.ams = 0;
                            pw.fms = 0;
                            if (!pw.dataEnd) outOPNSetPanAMSFMS(pw, 3, 0, 0);
                        }
                    }

                    if (i != 0) dat[0x2f] |= 0x40;
                }

                if (sn76489[i].use)
                {
                    if (i != 0) dat[0x0f] |= 0x40;
                }

                if (rf5c164[i].use)
                {
                    for (int ch = 0; ch < rf5c164[i].ChMax; ch++)
                    {
                        partWork pw = rf5c164[i].lstPartWork[ch];

                        setRf5c164CurrentChannel(pw);
                        setRf5c164SampleStartAddress(pw, 0);
                        setRf5c164LoopAddress(pw, 0);
                        setRf5c164AddressIncrement(pw, 0x400);
                        setRf5c164Pan(pw, 0xff);
                        setRf5c164Envelope(pw, 0xff);
                    }

                    if (i != 0) dat[0x6f] |= 0x40;
                }

                if (ym2610b[i].use)
                {
                    //initialize shared param
                    outOPNSetHardLfo(ym2610b[i].lstPartWork[0], false, 0);
                    outOPNSetCh3SpecialMode(ym2610b[i].lstPartWork[0], false);

                    //FM Off
                    outFmAllKeyOff(ym2610b[i]);

                    //SSG Off
                    for (int ch = 9; ch < 12; ch++)
                    {
                        outSsgKeyOff(ym2610b[i].lstPartWork[ch]);
                        ym2610b[i].lstPartWork[ch].volume = 0;
                        //setSsgVolume(ym2610b[i].lstPartWork[ch]);
                        //ym2610b[i].lstPartWork[ch].volume = 15;
                    }

                    //ADPCM-A/B Reset
                    outFmAdrPort(ym2610b[i].lstPartWork[0].port0, 0x1c, 0xbf);
                    outFmAdrPort(ym2610b[i].lstPartWork[0].port0, 0x1c, 0x00);
                    outFmAdrPort(ym2610b[i].lstPartWork[0].port0, 0x10, 0x00);
                    outFmAdrPort(ym2610b[i].lstPartWork[0].port0, 0x11, 0xc0);

                    foreach (partWork pw in ym2610b[i].lstPartWork)
                    {
                        if (pw.ch == 0)
                        {
                            pw.hardLfoSw = false;
                            pw.hardLfoNum = 0;
                            outOPNSetHardLfo(pw, pw.hardLfoSw, pw.hardLfoNum);
                        }

                        if (pw.ch < 6)
                        {
                            pw.pan = 3;
                            pw.ams = 0;
                            pw.fms = 0;
                            if (!pw.dataEnd) outOPNSetPanAMSFMS(pw, 3, 0, 0);
                        }
                    }

                    dat[0x4f] |= 0x80;//YM2610B
                    if (i != 0) dat[0x4f] |= 0x40;//use Secondary
                }

            }

        }

        private void makeFooter()
        {

            byte[] v;

            //end of data
            dat.Add(0x66);

            //GD3 offset
            v = divInt2ByteAry(dat.Count - 0x14);
            dat[0x14] = v[0]; dat[0x15] = v[1]; dat[0x16] = v[2]; dat[0x17] = v[3];

            //Total # samples
            v = divInt2ByteAry((int)lSample);
            dat[0x18] = v[0]; dat[0x19] = v[1]; dat[0x1a] = v[2]; dat[0x1b] = v[3];

            if (loopOffset != -1)
            {
                //Loop offset
                v = divInt2ByteAry((int)(loopOffset - 0x1c));
                dat[0x1c] = v[0]; dat[0x1d] = v[1]; dat[0x1e] = v[2]; dat[0x1f] = v[3];

                //Loop # samples
                v = divInt2ByteAry((int)(lSample - loopSamples));
                dat[0x20] = v[0]; dat[0x21] = v[1]; dat[0x22] = v[2]; dat[0x23] = v[3];
            }

            //'Gd3 '
            dat.Add(0x47); dat.Add(0x64); dat.Add(0x33); dat.Add(0x20);

            //GD3 Version
            dat.Add(0x00); dat.Add(0x01); dat.Add(0x00); dat.Add(0x00);

            //GD3 Length(dummy)
            dat.Add(0x00); dat.Add(0x00); dat.Add(0x00); dat.Add(0x00);

            int p = dat.Count;

            //TrackName
            dat.AddRange(Encoding.Unicode.GetBytes(TitleName));
            dat.Add(0x00); dat.Add(0x00);
            dat.AddRange(Encoding.Unicode.GetBytes(TitleNameJ));
            dat.Add(0x00); dat.Add(0x00);

            //GameName
            dat.AddRange(Encoding.Unicode.GetBytes(GameName));
            dat.Add(0x00); dat.Add(0x00);
            dat.AddRange(Encoding.Unicode.GetBytes(GameNameJ));
            dat.Add(0x00); dat.Add(0x00);

            //SystemName
            dat.AddRange(Encoding.Unicode.GetBytes(SystemName));
            dat.Add(0x00); dat.Add(0x00);
            dat.AddRange(Encoding.Unicode.GetBytes(SystemNameJ));
            dat.Add(0x00); dat.Add(0x00);

            //Composer
            dat.AddRange(Encoding.Unicode.GetBytes(Composer));
            dat.Add(0x00); dat.Add(0x00);
            dat.AddRange(Encoding.Unicode.GetBytes(ComposerJ));
            dat.Add(0x00); dat.Add(0x00);

            //ReleaseDate
            dat.AddRange(Encoding.Unicode.GetBytes(ReleaseDate));
            dat.Add(0x00); dat.Add(0x00);

            //Converted
            dat.AddRange(Encoding.Unicode.GetBytes(Converted));
            dat.Add(0x00); dat.Add(0x00);

            //Notes
            dat.AddRange(Encoding.Unicode.GetBytes(Notes));
            dat.Add(0x00); dat.Add(0x00);

            //EoF offset
            v = divInt2ByteAry(dat.Count - 0x4);
            dat[0x4] = v[0]; dat[0x5] = v[1]; dat[0x6] = v[2]; dat[0x7] = v[3];

            int q = dat.Count - p;

            //GD3 Length
            v = divInt2ByteAry(q);
            dat[p - 4] = v[0]; dat[p - 3] = v[1]; dat[p - 2] = v[2]; dat[p - 1] = v[3];

            long useYM2610B = 0;
            long useYM2612 = 0;
            long useSN76489 = 0;
            long useRf5c164 = 0;

            for (int i = 0; i < 2; i++)
            {
                foreach (partWork pw in ym2610b[i].lstPartWork)
                {
                    useYM2610B += pw.clockCounter;
                }
                foreach (partWork pw in ym2612[i].lstPartWork)
                {
                    useYM2612 += pw.clockCounter;
                }
                foreach (partWork pw in sn76489[i].lstPartWork)
                {
                    useSN76489 += pw.clockCounter;
                }
                foreach (partWork pw in rf5c164[i].lstPartWork)
                {
                    useRf5c164 += pw.clockCounter;
                }
            }

            if (useYM2610B == 0) { dat[0x4c] = 0; dat[0x4d] = 0; dat[0x4e] = 0; dat[0x4f] = 0; }
            if (useYM2612 == 0) { dat[0x2c] = 0; dat[0x2d] = 0; dat[0x2e] = 0; dat[0x2f] = 0; }
            if (useSN76489 == 0) { dat[0x0c] = 0; dat[0x0d] = 0; dat[0x0e] = 0; dat[0x0f] = 0; }
            if (useRf5c164 == 0) { dat[0x6c] = 0; dat[0x6d] = 0; dat[0x6e] = 0; dat[0x6f] = 0; }

            if (Version == 1.51f)
            {
                dat[0x08] = 0x51;
                dat[0x09] = 0x01;
            }
            else if (Version == 1.60f)
            {
                dat[0x08] = 0x60;
                dat[0x09] = 0x01;
            }

        }

        private void setEnvelopeAtKeyOn(partWork pw)
        {
            if (!pw.envelopeMode)
            {
                pw.envVolume = 0;
                pw.envIndex = -1;
                return;
            }

            pw.envIndex = 0;
            pw.envCounter = 0;
            int maxValue = pw.MaxVolume;// (pw.envelope[8] == (int)enmChipType.RF5C164) ? 255 : 15;

            while (pw.envCounter == 0 && pw.envIndex != -1)
            {
                switch (pw.envIndex)
                {
                    case 0: // AR phase
                        pw.envCounter = pw.envelope[2];
                        if (pw.envelope[2] > 0 && pw.envelope[1] < maxValue)
                        {
                            pw.envVolume = pw.envelope[1];
                        }
                        else
                        {
                            pw.envVolume = maxValue;
                            pw.envIndex++;
                        }
                        break;
                    case 1: // DR phase
                        pw.envCounter = pw.envelope[3];
                        if (pw.envelope[3] > 0 && pw.envelope[4] < maxValue)
                        {
                            pw.envVolume = maxValue;
                        }
                        else
                        {
                            pw.envVolume = pw.envelope[4];
                            pw.envIndex++;
                        }
                        break;
                    case 2: // SR phase
                        pw.envCounter = pw.envelope[5];
                        if (pw.envelope[5] > 0 && pw.envelope[4] != 0)
                        {
                            pw.envVolume = pw.envelope[4];
                        }
                        else
                        {
                            pw.envVolume = 0;
                            pw.envIndex = -1;
                        }
                        break;
                }
            }
        }

        private void setLfoAtKeyOn(partWork pw)
        {
            for (int lfo = 0; lfo < 4; lfo++)
            {
                clsLfo pl = pw.lfo[lfo];
                if (!pl.sw)
                {
                    continue;
                }
                if (pl.type == eLfoType.Hardware)
                {
                    if (pw.chip is YM2612)
                    {
                        if (pl.param[4] == 1)
                        {
                            outOPNSetHardLfo(pw, false, pl.param[1]);
                            pl.waitCounter = pl.param[0];
                        }
                    }
                    continue;
                }
                if (pl.param[5] != 1)
                {
                    continue;
                }

                pl.isEnd = false;
                pl.value = (pl.param[0] == 0) ? pl.param[6] : 0;//ディレイ中は振幅補正は適用されない
                pl.waitCounter = pl.param[0];
                pl.direction = pl.param[2] < 0 ? -1 : 1;
                if (pl.type == eLfoType.Vibrato)
                {
                    if ((pw.chip is YM2610B) || (pw.chip is YM2612))
                    {
                        setFmFNum(pw);
                    }
                }
                if (pl.type == eLfoType.Tremolo)
                {
                    if ((pw.chip is YM2610B) || (pw.chip is YM2612))
                    {
                        pw.beforeVolume = -1;
                        setFmVolume(pw);
                    }
                }
            }
        }

        private byte[] divInt2ByteAry(int n)
        {
            return new byte[4] {
                 (byte)( n & 0xff                   )
                ,(byte)((n & 0xff00    ) / 0x100    )
                ,(byte)((n & 0xff0000  ) / 0x10000  )
                ,(byte)((n & 0xff000000) / 0x1000000)
            };
        }

        private int checkRange(int n, int min, int max)
        {
            int r = n;

            if (n < min)
            {
                r = min;
            }
            if (n > max)
            {
                r = max;
            }

            return r;
        }


        private void setFmFNum(partWork pw)
        {
            int[] ftbl = (pw.chip is YM2612) ? OPN_FNumTbl_7670454 : OPN_FNumTbl_8000000;

            int f = getFmFNum(ftbl, pw.octaveNow, pw.noteCmd, pw.shift + pw.keyShift);//
            if (pw.bendWaitCounter != -1)
            {
                f = pw.bendFnum;
            }
            int o = (f & 0xf000) / 0x1000;
            f &= 0xfff;

            f = f + pw.detune;
            for (int lfo = 0; lfo < 4; lfo++)
            {
                if (!pw.lfo[lfo].sw)
                {
                    continue;
                }
                if (pw.lfo[lfo].type != eLfoType.Vibrato)
                {
                    continue;
                }
                f += pw.lfo[lfo].value + pw.lfo[lfo].param[6];
            }
            while (f < ftbl[0])
            {
                if (o == 1)
                {
                    break;
                }
                o--;
                f = ftbl[0] * 2 - (ftbl[0] - f);
            }
            while (f >= ftbl[0] * 2)
            {
                if (o == 8)
                {
                    break;
                }
                o++;
                f = f - ftbl[0] * 2 + ftbl[0];
            }
            f = checkRange(f, 0, 0x7ff);

            outFmSetFnum(pw, o, f);
        }

        private int getFmFNum(int[] ftbl, int octave, char noteCmd, int shift)
        {
            int o = octave;
            int n = note.IndexOf(noteCmd) + shift;
            if (n >= 0)
            {
                o += n / 12;
                o = checkRange(o, 1, 8);
                n %= 12;
            }
            else
            {
                o += n / 12 - ((n % 12 == 0) ? 0 : 1);
                o = checkRange(o, 1, 8);
                n %= 12;
                if (n < 0) { n += 12; }
            }

            int f = ftbl[n];

            return (f & 0xfff) + (o & 0xf) * 0x1000;
        }

        private void getPcmNote(partWork pw)
        {
            int shift = pw.shift + pw.keyShift;
            int o = pw.octaveNow;//
            int n = note.IndexOf(pw.noteCmd) + shift;
            if (n >= 0)
            {
                o += n / 12;
                o = checkRange(o, 1, 8);
                n %= 12;
            }
            else
            {
                o += n / 12 - 1;
                o = checkRange(o, 1, 8);
                n %= 12;
                if (n < 0) { n += 12; }
            }

            pw.pcmOctave = o;
            pw.pcmNote = n;
        }

        private void setFmVolume(partWork pw)
        {
            int vol = pw.volume;

            for (int lfo = 0; lfo < 4; lfo++)
            {
                if (!pw.lfo[lfo].sw)
                {
                    continue;
                }
                if (pw.lfo[lfo].type != eLfoType.Tremolo)
                {
                    continue;
                }
                vol += pw.lfo[lfo].value + pw.lfo[lfo].param[6];
            }

            if (pw.beforeVolume != vol)
            {
                if (instFM.ContainsKey(pw.instrument))
                {
                    outFmSetVolume(pw, vol, pw.instrument);
                    pw.beforeVolume = vol;
                }
            }
        }


        private void setSsgFNum(partWork pw)
        {
            int f = getSsgFNum(pw.octaveNow, pw.noteCmd, pw.shift + pw.keyShift);//
            if (pw.bendWaitCounter != -1)
            {
                f = pw.bendFnum;
            }
            f = f + pw.detune;
            for (int lfo = 0; lfo < 4; lfo++)
            {
                if (!pw.lfo[lfo].sw)
                {
                    continue;
                }
                if (pw.lfo[lfo].type != eLfoType.Vibrato)
                {
                    continue;
                }
                f += pw.lfo[lfo].value + pw.lfo[lfo].param[6];
            }

            f = checkRange(f, 0, 0xfff);
            if (pw.freq == f) return;

            pw.freq = f;

            byte data = 0;

            data = (byte)(f & 0xff);
            outFmAdrPort(pw.port0, (byte)(0 + (pw.ch - 9) * 2), data);

            data = (byte)((f & 0xf00) >> 8);
            outFmAdrPort(pw.port0, (byte)(1 + (pw.ch - 9) * 2), data);
        }

        private int getSsgFNum(int octave, char noteCmd, int shift)
        {
            int o = octave - 1;
            int n = note.IndexOf(noteCmd) + shift;
            o += n / 12;
            o = checkRange(o, 0, 7);
            n %= 12;

            int f = o * 12 + n;
            if (f < 0) f = 0;
            if (f >= ssgFNumTbl_8000000.Length) f = ssgFNumTbl_8000000.Length - 1;

            return ssgFNumTbl_8000000[f];
        }


        private void setAdpcmBFNum(partWork pw)
        {
            int f = getAdpcmBFNum(pw.octaveNow, pw.noteCmd, pw.shift + pw.keyShift);//
            if (pw.bendWaitCounter != -1)
            {
                f = pw.bendFnum;
            }
            f = f + pw.detune;
            for (int lfo = 0; lfo < 4; lfo++)
            {
                if (!pw.lfo[lfo].sw)
                {
                    continue;
                }
                if (pw.lfo[lfo].type != eLfoType.Vibrato)
                {
                    continue;
                }
                f += pw.lfo[lfo].value + pw.lfo[lfo].param[6];
            }

            f = checkRange(f, 0, 0xffff);
            if (pw.freq == f) return;

            pw.freq = f;

            byte data = 0;

            data = (byte)(f & 0xff);
            outFmAdrPort(pw.port0, 0x19, data);

            data = (byte)((f & 0xff00) >> 8);
            outFmAdrPort(pw.port0, 0x1a, data);
        }

        private int getAdpcmBFNum(int octave, char noteCmd, int shift)
        {
            int o = octave - 1;
            int n = note.IndexOf(noteCmd) + shift;
            if (n >= 0)
            {
                o += n / 12;
                o = checkRange(o, 1, 8);
                n %= 12;
            }
            else
            {
                o += n / 12 - 1;
                o = checkRange(o, 1, 8);
                n %= 12;
                if (n < 0) { n += 12; }
            }

            return (int)(0x49ba * pcmMTbl[n] * Math.Pow(2, (o - 4)));
        }


        private void setPsgFNum(partWork pw)
        {
            if (pw.Type != enmChannelType.DCSGNOISE)
            {
                int f = getPsgFNum(pw.octaveNow, pw.noteCmd, pw.shift + pw.keyShift);//
                if (pw.bendWaitCounter != -1)
                {
                    f = pw.bendFnum;
                }
                f = f + pw.detune;
                for (int lfo = 0; lfo < 4; lfo++)
                {
                    if (!pw.lfo[lfo].sw)
                    {
                        continue;
                    }
                    if (pw.lfo[lfo].type != eLfoType.Vibrato)
                    {
                        continue;
                    }
                    f += pw.lfo[lfo].value + pw.lfo[lfo].param[6];
                }

                f = checkRange(f, 0, 0x3ff);
                if (pw.freq == f) return;

                pw.freq = f;

                byte data = 0;

                data = (byte)(0x80 + (pw.ch << 5) + (f & 0xf));
                outPsgPort(pw.isSecondary, data);

                data = (byte)((f & 0x3f0) >> 4);
                outPsgPort(pw.isSecondary, data);
            }
            else
            {
                int f = pw.noise;
                byte data = (byte)(0xe0 + (f & 0x7));
                pw.freq = 0x40 + (f & 7);
            }

        }

        private int getPsgFNum(int octave, char noteCmd, int shift)
        {
            int o = octave - 1;
            int n = note.IndexOf(noteCmd) + shift;
            o += n / 12;
            o = checkRange(o, 0, 7);
            n %= 12;

            int f = o * 12 + n;
            if (f < 0) f = 0;
            if (f >= psgFNumTbl.Length) f = psgFNumTbl.Length - 1;

            return psgFNumTbl[f];
        }

        private void setPsgVolume(partWork pw)
        {
            byte data = 0;

            int vol = pw.volume;

            if (pw.envelopeMode)
            {
                vol = 0;
                if (pw.envIndex != -1)
                {
                    vol = pw.envVolume - (15 - pw.volume);
                }
            }

            for (int lfo = 0; lfo < 4; lfo++)
            {
                if (!pw.lfo[lfo].sw) continue;
                if (pw.lfo[lfo].type != eLfoType.Tremolo) continue;

                vol += pw.lfo[lfo].value + pw.lfo[lfo].param[6];
            }

            vol = checkRange(vol, 0, 15);

            if (pw.beforeVolume != vol)
            {
                data = (byte)(0x80 + (pw.ch << 5) + 0x10 + (15 - vol));
                outPsgPort(pw.isSecondary, data);
                pw.beforeVolume = vol;
            }
        }


        private int getRf5c164PcmNote(int octave, char noteCmd, int shift)
        {
            int o = octave;
            int n = note.IndexOf(noteCmd) + shift;
            if (n >= 0)
            {
                o += n / 12;
                o = checkRange(o, 1, 8);
                n %= 12;
            }
            else
            {
                o += n / 12 - 1;
                o = checkRange(o, 1, 8);
                n %= 12;
                if (n < 0) { n += 12; }
            }

            return (int)(0x0400 * pcmMTbl[n] * Math.Pow(2, (o - 4)));
        }

        private void setRf5c164Volume(partWork pw)
        {
            int vol = pw.volume;

            if (pw.envelopeMode)
            {
                vol = 0;
                if (pw.envIndex != -1)
                {
                    vol = pw.envVolume - (255 - pw.volume);
                }
            }

            for (int lfo = 0; lfo < 4; lfo++)
            {
                if (!pw.lfo[lfo].sw)
                {
                    continue;
                }
                if (pw.lfo[lfo].type != eLfoType.Tremolo)
                {
                    continue;
                }
                vol += pw.lfo[lfo].value + pw.lfo[lfo].param[6];
            }

            vol = checkRange(vol, 0, 255);

            if (pw.beforeVolume != vol)
            {
                setRf5c164Envelope(pw, vol);
                pw.beforeVolume = vol;
            }
        }

        private void setRf5c164FNum(partWork pw)
        {
            int f = getRf5c164PcmNote(pw.octaveNow, pw.noteCmd, pw.keyShift + pw.shift);//

            if (pw.bendWaitCounter != -1)
            {
                f = pw.bendFnum;
            }
            f = f + pw.detune;
            for (int lfo = 0; lfo < 4; lfo++)
            {
                if (!pw.lfo[lfo].sw)
                {
                    continue;
                }
                if (pw.lfo[lfo].type != eLfoType.Vibrato)
                {
                    continue;
                }
                f += pw.lfo[lfo].value + pw.lfo[lfo].param[6];
            }

            f = checkRange(f, 0, 0xffff);
            pw.freq = f;

            setRf5c164CurrentChannel(pw);

            //Address increment 再生スピードをセット
            setRf5c164AddressIncrement(pw, f);

        }

        private void setRf5c164Envelope(partWork pw, int volume)
        {
            if (pw.rf5c164Envelope != volume)
            {
                setRf5c164CurrentChannel(pw);
                byte data = (byte)(volume & 0xff);
                outRf5c164Port(pw.isSecondary, 0x0, data);
                pw.rf5c164Envelope = volume;
            }
        }

        private void setRf5c164Pan(partWork pw, int pan)
        {
            if (pw.rf5c164Pan != pan)
            {
                setRf5c164CurrentChannel(pw);
                byte data = (byte)(pan & 0xff);
                outRf5c164Port(pw.isSecondary, 0x1, data);
                pw.rf5c164Pan = pan;
            }
        }

        private void setRf5c164CurrentChannel(partWork pw)
        {
            byte pch = (byte)pw.ch;
            bool isSecondary = pw.isSecondary;
            int chipID = pw.chip.ChipID;

            if (rf5c164[chipID].CurrentChannel != pch)
            {
                byte data = (byte)(0xc0 + pch);
                outRf5c164Port(isSecondary, 0x7, data);
                rf5c164[chipID].CurrentChannel = pch;
            }
        }

        private void setRf5c164AddressIncrement(partWork pw, int f)
        {
            if (pw.rf5c164AddressIncrement != f)
            {
                byte data = (byte)(f & 0xff);
                outRf5c164Port(pw.isSecondary, 0x2, data);
                data = (byte)((f >> 8) & 0xff);
                outRf5c164Port(pw.isSecondary, 0x3, data);
                pw.rf5c164AddressIncrement = f;
            }
        }

        private void setRf5c164SampleStartAddress(partWork pw, int adr)
        {
            if (pw.pcmStartAddress != adr)
            {
                byte data = (byte)((adr >> 8) & 0xff);
                outRf5c164Port(pw.isSecondary, 0x6, data);
                pw.pcmStartAddress = adr;
            }
        }

        private void setRf5c164LoopAddress(partWork pw, int adr)
        {
            if (pw.pcmLoopAddress != adr)
            {
                byte data = (byte)((adr >> 8) & 0xff);
                outRf5c164Port(pw.isSecondary, 0x5, data);
                data = (byte)(adr & 0xff);
                outRf5c164Port(pw.isSecondary, 0x4, data);
                pw.pcmLoopAddress = adr;
            }
        }

        private void outRf5c164KeyOn(partWork pw)
        {
            rf5c164[pw.chip.ChipID].KeyOn |= (byte)(1 << pw.ch);
            byte data = (byte)(~rf5c164[pw.chip.ChipID].KeyOn);
            outRf5c164Port(pw.isSecondary, 0x8, data);
        }

        private void outRf5c164KeyOff(partWork pw)
        {
            rf5c164[pw.chip.ChipID].KeyOn &= (byte)(~(1 << pw.ch));
            byte data = (byte)(~rf5c164[pw.chip.ChipID].KeyOn);
            outRf5c164Port(pw.isSecondary, 0x8, data);
        }

        private void outRf5c164Port(bool isSecondary, byte adr, byte data)
        {
            dat.Add(0xb1);
            dat.Add((byte)((adr & 0x7f) | (isSecondary ? 0x80 : 0x00)));
            dat.Add(data);
        }


        private void setYM2610BADPCMAAddress(partWork pw, int startAdr, int endAdr)
        {
            if (pw.pcmStartAddress != startAdr)
            {
                outFmAdrPort(pw.port1, (byte)(0x10 + (pw.ch - 12)), (byte)((startAdr >> 8) & 0xff));
                outFmAdrPort(pw.port1, (byte)(0x18 + (pw.ch - 12)), (byte)((startAdr >> 16) & 0xff));
                pw.pcmStartAddress = startAdr;
            }

            if (pw.pcmEndAddress != endAdr)
            {
                outFmAdrPort(pw.port1, (byte)(0x20 + (pw.ch - 12)), (byte)(((endAdr - 0x100) >> 8) & 0xff));
                outFmAdrPort(pw.port1, (byte)(0x28 + (pw.ch - 12)), (byte)(((endAdr - 0x100) >> 16) & 0xff));
                pw.pcmEndAddress = endAdr;
            }

        }

        private void setYM2610BADPCMBAddress(partWork pw, int startAdr, int endAdr)
        {
            if (pw.pcmStartAddress != startAdr)
            {
                outFmAdrPort(pw.port0, 0x12, (byte)((startAdr >> 8) & 0xff));
                outFmAdrPort(pw.port0, 0x13, (byte)((startAdr >> 16) & 0xff));
                pw.pcmStartAddress = startAdr;
            }

            if (pw.pcmEndAddress != endAdr)
            {
                outFmAdrPort(pw.port0, 0x14, (byte)(((endAdr - 0x100) >> 8) & 0xff));
                outFmAdrPort(pw.port0, 0x15, (byte)(((endAdr - 0x100) >> 16) & 0xff));
                pw.pcmEndAddress = endAdr;
            }

            //outFmAdrPort(pw.port1, 0x01, 0x3f);
            //outFmAdrPort(pw.port1, 0x08, 0xdf);
            //outFmAdrPort(pw.port1, 0x00, 0x01);
        }



        private void outFmAdrPort(byte port, byte address, byte data)
        {
            dat.Add(port);
            dat.Add(address);
            dat.Add(data);
        }

        private void outFmKeyOn(partWork pw)
        {
            if (!pw.pcm)
            {
                if (pw.chip.lstPartWork[2].Ch3SpecialMode && pw.Type == enmChannelType.FMOPNex)
                {
                    pw.Ch3SpecialModeKeyOn = true;

                    int slot = (pw.chip.lstPartWork[2].Ch3SpecialModeKeyOn ? pw.chip.lstPartWork[2].slots : 0x0)
                        | (pw.chip.lstPartWork[6].Ch3SpecialModeKeyOn ? pw.chip.lstPartWork[6].slots : 0x0)
                        | (pw.chip.lstPartWork[7].Ch3SpecialModeKeyOn ? pw.chip.lstPartWork[7].slots : 0x0)
                        | (pw.chip.lstPartWork[8].Ch3SpecialModeKeyOn ? pw.chip.lstPartWork[8].slots : 0x0);

                    outFmAdrPort(pw.port0, 0x28, (byte)((slot << 4) + 2));
                }
                else
                {
                    if (pw.ch >= 0 && pw.ch < 6)
                    {
                        byte vch = (byte)((pw.ch > 2) ? pw.ch + 1 : pw.ch);
                        //key on
                        outFmAdrPort(pw.port0, 0x28, (byte)((pw.slots << 4) + (vch & 7)));
                    }
                }

                return;
            }

            float m = pcmMTbl[pw.pcmNote] * (float)Math.Pow(2, (pw.pcmOctave - 4));
            pw.pcmBaseFreqPerFreq = vgmSamplesPerSecond / ((float)instPCM[pw.instrument].freq * m);
            pw.pcmFreqCountBuffer = 0.0f;
            long p = instPCM[pw.instrument].stAdr;
            if (Version == 1.51f)
            {
                dat.Add(0xe0);
                dat.Add((byte)(p & 0xff));
                dat.Add((byte)((p & 0xff00) / 0x100));
                dat.Add((byte)((p & 0xff0000) / 0x10000));
                dat.Add((byte)((p & 0xff000000) / 0x10000));
            }
            else
            {
                long s = instPCM[pw.instrument].size;
                long f = instPCM[pw.instrument].freq;
                long w = 0;
                if (pw.gatetimePmode)
                {
                    w = pw.waitCounter * pw.gatetime / 8L;
                }
                else
                {
                    w = pw.waitCounter - pw.gatetime;
                }
                if (w < 1) w = 1;
                s = Math.Min(s, w * (long)samplesPerClock * f / 44100);

                if (!pw.streamSetup)
                {
                    newStreamID++;
                    pw.streamID = newStreamID;
                    // setup stream control
                    dat.Add(0x90);
                    dat.Add((byte)pw.streamID);
                    dat.Add((byte)(0x02 + (pw.isSecondary ? 0x80 : 0x00)));
                    dat.Add(0x00);
                    dat.Add(0x2a);

                    // set stream data
                    dat.Add(0x91);
                    dat.Add((byte)pw.streamID);
                    dat.Add(0x00);
                    dat.Add(0x01);
                    dat.Add(0x00);

                    pw.streamSetup = true;
                }

                if (pw.streamFreq != f)
                {
                    //Set Stream Frequency
                    dat.Add(0x92);
                    dat.Add((byte)pw.streamID);

                    dat.Add((byte)(f & 0xff));
                    dat.Add((byte)((f & 0xff00) / 0x100));
                    dat.Add((byte)((f & 0xff0000) / 0x10000));
                    dat.Add((byte)((f & 0xff000000) / 0x10000));

                    pw.streamFreq = f;
                }

                //Start Stream
                dat.Add(0x93);
                dat.Add((byte)pw.streamID);

                dat.Add((byte)(p & 0xff));
                dat.Add((byte)((p & 0xff00) / 0x100));
                dat.Add((byte)((p & 0xff0000) / 0x10000));
                dat.Add((byte)((p & 0xff000000) / 0x10000));

                dat.Add(0x01);

                dat.Add((byte)(s & 0xff));
                dat.Add((byte)((s & 0xff00) / 0x100));
                dat.Add((byte)((s & 0xff0000) / 0x10000));
                dat.Add((byte)((s & 0xff000000) / 0x10000));
            }
        }

        private void outFmKeyOff(partWork pw)
        {
            if (!pw.pcm)
            {
                if (pw.chip.lstPartWork[2].Ch3SpecialMode && pw.Type == enmChannelType.FMOPNex)
                {
                    pw.Ch3SpecialModeKeyOn = false;

                    int slot = (pw.chip.lstPartWork[2].Ch3SpecialModeKeyOn ? pw.chip.lstPartWork[2].slots : 0x0)
                        | (pw.chip.lstPartWork[6].Ch3SpecialModeKeyOn ? pw.chip.lstPartWork[6].slots : 0x0)
                        | (pw.chip.lstPartWork[7].Ch3SpecialModeKeyOn ? pw.chip.lstPartWork[7].slots : 0x0)
                        | (pw.chip.lstPartWork[8].Ch3SpecialModeKeyOn ? pw.chip.lstPartWork[8].slots : 0x0);

                    outFmAdrPort(pw.port0, 0x28, (byte)((slot << 4) + 2));
                }
                else
                {
                    if (pw.ch >= 0 && pw.ch < 6)
                    {
                        byte vch = (byte)((pw.ch > 2) ? pw.ch + 1 : pw.ch);
                        //key off
                        outFmAdrPort(pw.port0, 0x28, (byte)(0x00 + (vch & 7)));
                    }
                }

                return;
            }

            pw.pcmWaitKeyOnCounter = -1;
        }

        private void outFmAllKeyOff(clsChip chip)
        {

            foreach (partWork pw in chip.lstPartWork)
            {
                if (pw.dataEnd) continue;
                if (pw.ch > 5) continue;

                outFmKeyOff(pw);
                outFmSetTl(pw, 0, 127);
                outFmSetTl(pw, 1, 127);
                outFmSetTl(pw, 2, 127);
                outFmSetTl(pw, 3, 127);
            }

        }

        private void outFmSetFnum(partWork pw, int octave, int num)
        {
            int freq;
            freq = ((num & 0x700) >> 8) + (((octave - 1) & 0x7) << 3);
            freq = (freq << 8) + (num & 0xff);

            if (freq == pw.freq) return;

            pw.freq = freq;

            if (pw.chip.lstPartWork[2].Ch3SpecialMode && pw.Type == enmChannelType.FMOPNex)
            {
                if ((pw.slots & 8) != 0)
                {
                    int f = pw.freq + pw.slotDetune[3];
                    outFmAdrPort(pw.port0, (byte)0xa6, (byte)((f & 0xff00) >> 8));
                    outFmAdrPort(pw.port0, (byte)0xa2, (byte)(f & 0xff));
                }
                if ((pw.slots & 4) != 0)
                {
                    int f = pw.freq + pw.slotDetune[2];
                    outFmAdrPort(pw.port0, (byte)0xac, (byte)((f & 0xff00) >> 8));
                    outFmAdrPort(pw.port0, (byte)0xa8, (byte)(f & 0xff));
                }
                if ((pw.slots & 1) != 0)
                {
                    int f = pw.freq + pw.slotDetune[0];
                    outFmAdrPort(pw.port0, (byte)0xad, (byte)((f & 0xff00) >> 8));
                    outFmAdrPort(pw.port0, (byte)0xa9, (byte)(f & 0xff));
                }
                if ((pw.slots & 2) != 0)
                {
                    int f = pw.freq + pw.slotDetune[1];
                    outFmAdrPort(pw.port0, (byte)0xae, (byte)((f & 0xff00) >> 8));
                    outFmAdrPort(pw.port0, (byte)0xaa, (byte)(f & 0xff));
                }
            }
            else
            {
                if (pw.ch >= 6 && pw.ch < 9)
                {
                    return;
                }
                if (pw.ch < 6)
                {
                    byte port = pw.ch > 2 ? pw.port1 : pw.port0;
                    byte vch = (byte)(pw.ch > 2 ? pw.ch - 3 : pw.ch);

                    outFmAdrPort(port, (byte)(0xa4 + vch), (byte)((pw.freq & 0xff00) >> 8));
                    outFmAdrPort(port, (byte)(0xa0 + vch), (byte)(pw.freq & 0xff));
                }
            }
        }

        private void outFmCh3SpecialModeSetFnum(partWork pw, byte ope, int octave, int num)
        {
            ope &= 3;
            if (ope == 0)
            {
                outFmAdrPort(pw.port0, 0xa6, (byte)(((num & 0x700) >> 8) + (((octave - 1) & 0x7) << 3)));
                outFmAdrPort(pw.port0, 0xa2, (byte)(num & 0xff));
            }
            else
            {
                outFmAdrPort(pw.port0, (byte)(0xac + ope), (byte)(((num & 0x700) >> 8) + (((octave - 1) & 0x7) << 3)));
                outFmAdrPort(pw.port0, (byte)(0xa8 + ope), (byte)(num & 0xff));
            }
        }

        private void outFmSetInstrument(partWork pw, int n, int vol)
        {

            if (!instFM.ContainsKey(n))
            {
                msgBox.setWrnMsg(string.Format("未定義の音色(@{0})を指定しています。", n), lineNumber);
                return;
            }

            if (pw.ch >= 6 && pw.ch < 9)
            {
                msgBox.setWrnMsg("拡張チャンネルでは音色指定はできません。", lineNumber);
                return;
            }

            for (int ope = 0; ope < 4; ope++)
            {

                outFmSetDtMl(pw, ope, instFM[n][ope * instrumentMOperaterSize + 9], instFM[n][ope * instrumentMOperaterSize + 8]);
                outFmSetKsAr(pw, ope, instFM[n][ope * instrumentMOperaterSize + 7], instFM[n][ope * instrumentMOperaterSize + 1]);
                outFmSetAmDr(pw, ope, instFM[n][ope * instrumentMOperaterSize + 10], instFM[n][ope * instrumentMOperaterSize + 2]);
                outFmSetSr(pw, ope, instFM[n][ope * instrumentMOperaterSize + 3]);
                outFmSetSlRr(pw, ope, instFM[n][ope * instrumentMOperaterSize + 5], instFM[n][ope * instrumentMOperaterSize + 4]);
                outFmSetSSGEG(pw, ope, instFM[n][ope * instrumentMOperaterSize + 11]);

            }

            outFmSetFeedbackAlgorithm(pw, instFM[n][46], instFM[n][45]);

            outFmSetVolume(pw, vol, n);

        }

        /// <summary>
        /// FMボリュームの設定
        /// </summary>
        /// <param name="ch">チャンネル</param>
        /// <param name="vol">ボリューム値</param>
        /// <param name="n">音色番号</param>
        private void outFmSetVolume(partWork pw, int vol, int n)
        {
            if (!instFM.ContainsKey(n))
            {
                msgBox.setWrnMsg(string.Format("未定義の音色(@{0})を指定している場合ボリュームの変更はできません。", n), lineNumber);
                return;
            }

            int alg = instFM[n][45] & 0x7;
            int[] ope = new int[4] {
                instFM[n][0*instrumentMOperaterSize + 6]
                , instFM[n][1 * instrumentMOperaterSize + 6]
                , instFM[n][2 * instrumentMOperaterSize + 6]
                , instFM[n][3 * instrumentMOperaterSize + 6]
            };
            int[][] algs = new int[8][]
            {
                new int[4] { 0,0,0,1}
                ,new int[4] { 0,0,0,1}
                ,new int[4] { 0,0,0,1}
                ,new int[4] { 0,0,0,1}
                ,new int[4] { 0,1,0,1}
                ,new int[4] { 0,1,1,1}
                ,new int[4] { 0,1,1,1}
                ,new int[4] { 1,1,1,1}
            };

            int minV = 127;
            for (int i = 0; i < 4; i++)
            {
                if (algs[alg][i] == 1)
                {
                    minV = Math.Min(minV, ope[i]);
                }
            }

            for (int i = 0; i < 4; i++)
            {
                if (algs[alg][i] == 0)
                {
                    continue;
                }
                ope[i] = ope[i] - minV + (127 - vol);
                if (ope[i] < 0)
                {
                    ope[i] = 0;
                }
                if (ope[i] > 127)
                {
                    ope[i] = 127;
                }
            }

            partWork vpw = pw;
            if (ym2612[pw.chip.ChipID].lstPartWork[2].Ch3SpecialMode && pw.ch >= 6 && pw.ch < 9)
            {
                vpw = ym2612[pw.chip.ChipID].lstPartWork[2];
            }

            if ((pw.slots & 1) != 0) outFmSetTl(vpw, 0, ope[0]);
            if ((pw.slots & 2) != 0) outFmSetTl(vpw, 1, ope[1]);
            if ((pw.slots & 4) != 0) outFmSetTl(vpw, 2, ope[2]);
            if ((pw.slots & 8) != 0) outFmSetTl(vpw, 3, ope[3]);
        }

        private void outFmSetDtMl(partWork pw, int ope, int dt, int ml)
        {
            int vch = pw.ch;
            byte port = vch > 2 ? pw.port1 : pw.port0;
            vch = (byte)(vch > 2 ? vch - 3 : vch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            dt &= 7;
            ml &= 15;

            outFmAdrPort(port, (byte)(0x30 + vch + ope * 4), (byte)((dt << 4) + ml));
        }

        private void outFmSetTl(partWork pw, int ope, int tl)
        {
            byte port = (pw.ch > 2 ? pw.port1 : pw.port0);
            int vch = (byte)(pw.ch > 2 ? pw.ch - 3 : pw.ch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            tl &= 0x7f;

            outFmAdrPort(port, (byte)(0x40 + vch + ope * 4), (byte)tl);
        }

        private void outFmSetKsAr(partWork pw, int ope, int ks, int ar)
        {
            int vch = pw.ch;
            byte port = (pw.ch > 2 ? pw.port1 : pw.port0);
            vch = (byte)(vch > 2 ? vch - 3 : vch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            ks &= 3;
            ar &= 31;

            outFmAdrPort(port, (byte)(0x50 + vch + ope * 4), (byte)((ks << 6) + ar));
        }

        private void outFmSetAmDr(partWork pw, int ope, int am, int dr)
        {
            int vch = pw.ch;
            byte port = (pw.ch > 2 ? pw.port1 : pw.port0);
            vch = (byte)(vch > 2 ? vch - 3 : vch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            am &= 1;
            dr &= 31;

            outFmAdrPort(port, (byte)(0x60 + vch + ope * 4), (byte)((am << 7) + dr));
        }

        private void outFmSetSr(partWork pw, int ope, int sr)
        {
            int vch = pw.ch;
            byte port = pw.ch > 2 ? pw.port1 : pw.port0;
            vch = (byte)(vch > 2 ? vch - 3 : vch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            sr &= 31;

            outFmAdrPort(port, (byte)(0x70 + vch + ope * 4), (byte)(sr));
        }

        private void outFmSetSlRr(partWork pw, int ope, int sl, int rr)
        {
            int vch = pw.ch;
            byte port = pw.ch > 2 ? pw.port1 : pw.port0;
            vch = (byte)(vch > 2 ? vch - 3 : vch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            sl &= 15;
            rr &= 15;

            outFmAdrPort(port, (byte)(0x80 + vch + ope * 4), (byte)((sl << 4) + rr));
        }

        private void outFmSetSSGEG(partWork pw, int ope, int n)
        {
            int vch = pw.ch;
            byte port = pw.ch > 2 ? pw.port1 : pw.port0;
            vch = (byte)(vch > 2 ? vch - 3 : vch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            n &= 15;

            outFmAdrPort(port, (byte)(0x90 + vch + ope * 4), (byte)n);
        }

        private void outFmSetFeedbackAlgorithm(partWork pw, int fb, int alg)
        {
            int vch = pw.ch;
            byte port = pw.ch > 2 ? pw.port1 : pw.port0;
            vch = (byte)(vch > 2 ? vch - 3 : vch);

            fb &= 7;
            alg &= 7;

            outFmAdrPort(port, (byte)(0xb0 + vch), (byte)((fb << 3) + alg));
        }

        private void outOPNSetPanAMSFMS(partWork pw, int pan, int ams, int fms)
        {
            int vch = pw.ch;
            byte port = pw.ch > 2 ? pw.port1 : pw.port0;
            vch = (byte)(vch > 2 ? vch - 3 : vch);

            pan = pan & 3;
            ams = ams & 3;
            fms = fms & 3;

            outFmAdrPort(port, (byte)(0xb4 + vch), (byte)((pan << 6) + (ams << 4) + fms));
        }

        private void outOPNSetHardLfo(partWork pw, bool sw, int lfoNum)
        {
            dat.Add(pw.port0);
            dat.Add(0x22);
            dat.Add((byte)((lfoNum & 7) + (sw ? 8 : 0)));
        }

        private void outOPNSetCh3SpecialMode(partWork pw, bool sw)
        {
            // ignore Timer ^^;
            dat.Add(pw.port0);
            dat.Add(0x27);
            dat.Add((byte)((sw ? 0x40 : 0)));
        }

        private void outYM2612SetCh6PCMMode(partWork pw, bool sw)
        {
            dat.Add(pw.port0);
            dat.Add(0x2b);
            dat.Add((byte)((sw ? 0x80 : 0)));
        }


        private void outSsgKeyOn(partWork pw)
        {
            byte pch = (byte)(pw.ch - 9);
            int n = (pw.mixer & 0x1) + ((pw.mixer & 0x2) << 2);
            byte data = (byte)(((YM2610B)pw.chip).SSGKeyOn | (9 << pch));
            data &= (byte)(~(n << pch));
            ((YM2610B)pw.chip).SSGKeyOn = data;

            setSsgVolume(pw);
            outFmAdrPort(pw.port0, 0x07, data);

        }

        private void outSsgKeyOff(partWork pw)
        {
            byte pch = (byte)(pw.ch - 9);
            int n = 9;
            byte data = (byte)(((YM2610B)pw.chip).SSGKeyOn | (n << pch));
            ((YM2610B)pw.chip).SSGKeyOn = data;

            outFmAdrPort(pw.port0, (byte)(0x08 + pch), 0);
            pw.beforeVolume = -1;
            outFmAdrPort(pw.port0, 0x07, data);

        }

        private void setSsgVolume(partWork pw)
        {
            byte pch = (byte)(pw.ch - 9);

            int vol = pw.volume;
            if (pw.envelopeMode)
            {
                vol = 0;
                if (pw.envIndex != -1)
                {
                    vol = pw.volume - (15 - pw.envVolume);
                }
            }
            vol = checkRange(vol, 0, 15);

            if (pw.beforeVolume != vol)
            {
                outFmAdrPort(pw.port0, (byte)(0x08 + pch), (byte)vol);
                pw.beforeVolume = pw.volume;
            }
        }

        private void outSsgNoise(partWork pw, int n)
        {
            outFmAdrPort(pw.port0, 0x06, (byte)(n & 0x1f));
        }


        private void outAdpcmBKeyOn(partWork pw)
        {

            setAdpcmBVolume(pw);
            outFmAdrPort(pw.port0, 0x10, 0x80);

        }

        private void outAdpcmBKeyOff(partWork pw)
        {

            outFmAdrPort(pw.port0, 0x10, 0x01);

        }

        private void setAdpcmBVolume(partWork pw)
        {

            int vol = pw.volume;
            if (pw.envelopeMode)
            {
                vol = 0;
                if (pw.envIndex != -1)
                {
                    vol = pw.volume - (0xff - pw.envVolume);
                }
            }
            vol = checkRange(vol, 0, 0xff);

            if (pw.beforeVolume != vol)
            {
                outFmAdrPort(pw.port0, 0x1b, (byte)vol);
                pw.beforeVolume = pw.volume;
            }
        }

        private void setAdpcmBPan(partWork pw, int pan)
        {
            if (pw.pan != pan)
            {
                outFmAdrPort(pw.port0, 0x11, (byte)((pan & 0x3) << 6));
                pw.pan = pan;
            }
        }


        private void outPsgPort(bool isSecondary, byte data)
        {
            dat.Add((byte)(isSecondary ? 0x30 : 0x50));
            dat.Add(data);
        }

        //private void _outPsgSetFnum(partWork pw, int freq)
        //{
        //pw.freq=freq;
        //}

        private void outPsgKeyOn(partWork pw)
        {
            byte pch = (byte)pw.ch;
            byte data = 0;


            data = (byte)(0x80 + (pch << 5) + 0x00 + (pw.freq & 0xf));
            outPsgPort(pw.isSecondary, data);

            if (pch != 3)
            {
                data = (byte)((pw.freq & 0x3f0) >> 4);
                outPsgPort(pw.isSecondary, data);
            }

            int vol = pw.volume;
            if (pw.envelopeMode)
            {
                vol = 0;
                if (pw.envIndex != -1)
                {
                    vol = pw.envVolume - (15 - pw.volume);
                }
            }
            if (vol > 15) vol = 15;
            if (vol < 0) vol = 0;
            data = (byte)(0x80 + (pch << 5) + 0x10 + (15 - vol));
            outPsgPort(pw.isSecondary, data);

        }

        private void outPsgKeyOff(partWork pw)
        {

            byte pch = (byte)pw.ch;
            int val = 15;

            byte data = (byte)(0x80 + (pch << 5) + 0x10 + (val & 0xf));
            outPsgPort((pw.ch - 9 * 2) > 3, data);

        }



        private void outWaitNSamples(long n)
        {
            long m = n;

            while (m > 0)
            {
                if (m > 0xffff)
                {
                    dat.Add(0x61);
                    dat.Add((byte)0xff);
                    dat.Add((byte)0xff);
                    m -= 0xffff;
                }
                else
                {
                    dat.Add(0x61);
                    dat.Add((byte)(m & 0xff));
                    dat.Add((byte)((m & 0xff00) >> 8));
                    m = 0L;
                }
            }
        }

        private void outWait735Samples(int repeatCount)
        {
            for (int i = 0; i < repeatCount; i++)
            {
                dat.Add(0x62);
            }
        }

        private void outWait882Samples(int repeatCount)
        {
            for (int i = 0; i < repeatCount; i++)
            {
                dat.Add(0x63);
            }
        }

        private void outWaitNSamplesWithPCMSending(partWork cpw, long cnt)
        {
            for (int i = 0; i < samplesPerClock * cnt;)
            {

                int f = (int)cpw.pcmBaseFreqPerFreq;
                cpw.pcmFreqCountBuffer += cpw.pcmBaseFreqPerFreq - (int)cpw.pcmBaseFreqPerFreq;
                while (cpw.pcmFreqCountBuffer > 1.0f)
                {
                    f++;
                    cpw.pcmFreqCountBuffer -= 1.0f;
                }
                if (i + f >= samplesPerClock * cnt)
                {
                    cpw.pcmFreqCountBuffer += (int)(i + f - samplesPerClock * cnt);
                    f = (int)(samplesPerClock * cnt - i);
                }
                if (cpw.pcmSizeCounter > 0)
                {
                    cpw.pcmSizeCounter--;
                    dat.Add((byte)(0x80 + f));
                }
                else
                {
                    outWaitNSamples(f);
                }
                i += f;
            }
        }



    }
}
