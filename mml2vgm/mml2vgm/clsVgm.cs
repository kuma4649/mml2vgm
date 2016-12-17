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

        public int[] fmFNumTbl = new int[] {
             0x289 // c
            ,0x2af // c+
            ,0x2d8 // d
            ,0x303 // d+
            ,0x331 // e
            ,0x362 // f
            ,0x395 // f+
            ,0x3cc // g
            ,0x405 // g+
            ,0x443 // a
            ,0x484 // a+
            ,0x4c8 // b
            ,0x289*2 // >c
        };

        public int[] psgFNumTbl = new int[] {
             0x6ae,0x64e,0x5f4,0x59e,0x54e,0x502,0x4ba,0x476,0x436,0x3f8,0x3c0,0x38a//1 0x3ff over note is silent 
            ,0x357,0x327,0x2fa,0x2cf,0x2a7,0x281,0x25d,0x23b,0x21b,0x1fc,0x1e0,0x1c5//2
            ,0x1ac,0x194,0x17d,0x168,0x153,0x140,0x12e,0x11d,0x10d,0x0fe,0x0f0,0x0e3//3
            ,0x0d6,0x0ca,0x0be,0x0b4,0x0aa,0x0a0,0x097,0x08f,0x087,0x07f,0x078,0x071//4
            ,0x06b,0x065,0x05f,0x05a,0x055,0x050,0x04c,0x047,0x043,0x040,0x03c,0x039//5
            ,0x035,0x032,0x030,0x02d,0x02a,0x028,0x026,0x024,0x022,0x020,0x01e,0x01c//6
            ,0x01b,0x019,0x018,0x016,0x015,0x014,0x013,0x012,0x011,0x010,0x00f,0x00e//7
            ,0x00d,0x00d,0x00c,0x00b,0x00b,0x00a,0x009,0x008,0x007,0x006,0x005,0x004//8
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
                //YM2612 clock(0x750ab5)
                0xb5,0x0a,0x75,0x00,
                //YM2151 clock(no use)
                0x00,0x00,0x00,0x00,
                //VGM data offset(1.50 only)
                0x0c+16*4,0x00,0x00,0x00,
                //Sega PCM clock(no use)
                0x00,0x00,0x00,0x00,
                //Sega PCM interface register(no use)
                0x00,0x00,0x00,0x00
                //0x40 RF5C68 clock(no use)
                ,0x00,0x00,0x00,0x00,
                //0x44
                0x00,0x00,0x00,0x00,
                //0x48
                0x00,0x00,0x00,0x00,
                //0x4c
                0x00,0x00,0x00,0x00,
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

            ym2612 = new YM2612[] { new YM2612(0,"F"), new YM2612(1, "Fs") };
            sn76489 = new SN76489[] { new SN76489(0, "S"), new SN76489(1, "Ss") };
            rf5c164 = new RF5C164[] { new RF5C164(0, "R"), new RF5C164(1, "Rs") };

            chips.Add(ym2612[0]);
            chips.Add(ym2612[1]);
            chips.Add(sn76489[0]);
            chips.Add(sn76489[1]);
            chips.Add(rf5c164[0]);
            chips.Add(rf5c164[1]);

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
            
            foreach(string p in partData.Keys)
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

        private int addInformation(string buf,int lineNumber)
        {
            string[] settings = buf.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach(string s in settings)
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

                    for (int i = 0; i < 2; i++)
                    {
                        if (wrd == PARTNAME + ym2612[i].Name + IDName[i]) setPartToCh(ym2612[i].Ch, val);
                        if (wrd == PARTNAME + ym2612[i].ShortName + IDName[i]) setPartToCh(ym2612[i].Ch, val);
                        if (wrd == PARTNAME + sn76489[i].Name + IDName[i]) setPartToCh(sn76489[i].Ch, val);
                        if (wrd == PARTNAME + sn76489[i].ShortName + IDName[i]) setPartToCh(sn76489[i].Ch, val);
                        if (wrd == PARTNAME + rf5c164[i].Name + IDName[i]) setPartToCh(rf5c164[i].Ch, val);
                        if (wrd == PARTNAME + rf5c164[i].ShortName + IDName[i]) setPartToCh(rf5c164[i].Ch, val);
                    }
                    if (wrd == PARTNAME + ym2612[0].Name) setPartToCh(ym2612[0].Ch, val);
                    if (wrd == PARTNAME + ym2612[0].ShortName) setPartToCh(ym2612[0].Ch, val);
                    if (wrd == PARTNAME + sn76489[0].Name) setPartToCh(sn76489[0].Ch, val);
                    if (wrd == PARTNAME + sn76489[0].ShortName) setPartToCh(sn76489[0].Ch, val);
                    if (wrd == PARTNAME + rf5c164[0].Name) setPartToCh(rf5c164[0].Ch, val);
                    if (wrd == PARTNAME + rf5c164[0].ShortName) setPartToCh(rf5c164[0].Ch, val);

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
                fmFNumTbl[i]= int.Parse(s[i], System.Globalization.NumberStyles.HexNumber);
            }
            fmFNumTbl[12] = fmFNumTbl[0] * 2;
        }

        private void setPsgF_NumTbl(string val,int oct)
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

        private void setPartToCh(clsChannel[] Ch, string val)
        {
            if (val == null || (val.Length != 1 && val.Length != 2)) return;

            string f = val[0].ToString();
            string r = (val.Length == 2) ? val[1].ToString() : " ";

            for (int i = 0; i < Ch.Length; i++)
            {
                if (Ch[i] == null) Ch[i] = new clsChannel();
                Ch[i].Name = string.Format("{0}{1}{2:00}", f, r, i + 1);
            }

        }

        private void setMonoPart(string val)
        {
            monoPart = divParts(val);
        }

        private void checkDuplication(string[] s)
        {
            foreach(string a in s)
            {
                int c = 0;
                foreach(string b in s)
                {
                    if (a == b) c++;
                }
                if (c > 1)
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        private int addInstrument(string buf, int lineNumber)
        {
            if(buf == null || buf.Length < 2)
            {
                msgBox.setWrnMsg("空の音色定義文を受け取りました。",lineNumber);
                return -1;
            }

            string s = buf.Substring(1).TrimStart();

            // FMの音色を定義中の場合
            if (instrumentCounter != -1)
            {

                return setInstrument(s,lineNumber);

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
                        bool isSecondary=false;
                        if (vs.Length > 4)
                        {
                            string chipName = vs[4].Trim().ToUpper();
                            isSecondary = false;
                            if (chipName.IndexOf(PRIMARY) >= 0)
                            {
                                isSecondary = false;
                                chipName = chipName.Replace(PRIMARY, "");
                            }else if (chipName.IndexOf(SECONDARY) >= 0)
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
                        instPCM.Add(num, new clsPcm(num, chip, isSecondary, fn, fq, vol, 0, 0, lp));
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

                        for(int i=0;i<env.Length-1;i++) {
                            if (env[8] == (int)enmChipType.SN76489)
                            {
                                if (i == 1 || i == 4 || i == 7)
                                {
                                    if (env[i] > 15)
                                    {
                                        env[i] = 15;
                                        msgBox.setWrnMsg("Envelope音量が15を超えています。", lineNumber);
                                    }
                                    if (env[i] < 0)
                                    {
                                        env[i] = 0;
                                        msgBox.setWrnMsg("Envelope音量が0未満です。", lineNumber);
                                    }
                                    if (env[i] == 0 && i == 7)
                                    {
                                        env[7] = 1;
                                    }
                                }
                            }
                            else if (env[8] == (int)enmChipType.RF5C164)
                            {
                                if (i == 1 || i == 4 || i == 7)
                                {
                                    if (env[i] > 255)
                                    {
                                        env[i] = 255;
                                        msgBox.setWrnMsg("Envelope音量が255を超えています。", lineNumber);
                                    }
                                    if (env[i] < 0)
                                    {
                                        env[i] = 0;
                                        msgBox.setWrnMsg("Envelope音量が0未満です。", lineNumber);
                                    }
                                    if (env[i] == 0 && i == 7)
                                    {
                                        env[7] = 1;
                                    }
                                }
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

        private static enmChipType getChipNumber(string chipN)
        {
            enmChipType chip;
            switch (chipN.ToUpper().Trim())
            {
                case "YM2612":
                    chip =  enmChipType.YM2612;
                    break;
                case "SN76489":
                    chip = enmChipType.SN76489;
                    break;
                case "RF5C164":
                    chip = enmChipType.RF5C164;
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
                case enmChipType.YM2612:
                    use = true;
                    break;
                case enmChipType.SN76489:
                    use = false;
                    break;
                case enmChipType.RF5C164:
                    use = true;
                    break;
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

            foreach(string p in part)
            {
                if (!partData.ContainsKey(p))
                {
                    partData.Add(p,new List<Tuple<int, string>>());
                }
                partData[p].Add(new Tuple<int, string>(lineNumber, data));
            }

            return 0;
        }

        private List<string> divParts(string parts)
        {
            List<string> ret = new List<string>();
            string a = "";
            int m = 0;

            for (int i = 0; i < parts.Length; i++)
            {
                if (m == 0 && parts[i] >= 'A' && parts[i] <= 'Z')
                {
                    a = parts[i].ToString();
                    m = 1;
                    if (i + 1 < parts.Length && parts[i + 1] >= 'a' && parts[i + 1] <= 'z')
                    {
                        a += parts[i + 1].ToString();
                        i++;
                    }
                    else
                    {
                        a += " ";
                    }
                }
                else if (parts[i] == ',')
                {
                    m = 0;
                }
                else if (m == 1 && parts[i] >= '0' && parts[i] <= '9')
                {
                    string n0 = parts[i].ToString();
                    int k = 1;// (getChMax(a) > 9) ? 2 : 1;
                    if (k == 2 && i + 1 < parts.Length)
                    {
                        n0 += parts[i + 1].ToString();
                        i++;
                    }

                    if (i < parts.Length - 2 && parts[i + 1] == '-')
                    {
                        string n1 = parts[i + 2].ToString();
                        if (k == 2 && i + 3 < parts.Length)
                        {
                            n1 += parts[i + 3].ToString();
                            i++;
                        }

                        int s, e;
                        if (!int.TryParse(n0, out s)) return null;
                        if (!int.TryParse(n1, out e)) return null;

                        if (s > e)
                        {
                            return null;
                        }
                        do
                        {
                            string p = string.Format("{0}{1:00}", a, s);
                            if (ret.Contains(p))
                            {
                                return null;
                            }
                            ret.Add(p);
                            s++;
                        } while (s <= e);

                        i += 2;
                        m = 0;
                    }
                    else
                    {
                        do
                        {
                            n0 = parts[i].ToString();
                            if (k == 2 && i + 1 < parts.Length)
                            {
                                n0 += parts[i + 1].ToString();
                                i++;
                            }

                            int s;
                            if (!int.TryParse(n0, out s)) return null;
                            string p = string.Format("{0}{1:00}", a, s);
                            ret.Add(p);
                            i++;
                        } while (i < parts.Length && parts[i] >= '0' && parts[i] <= '9');
                        m = 0;
                    }
                }
                else
                {
                    return null;
                }
            }

            return ret;
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
        private const int fmMaxVolume = 127;
        private const int psgMaxVolume = 15;
        private const int rf5c164MaxVolume = 255;

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
                            if (cpw.chip is SN76489 || cpw.chip is RF5C164)
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

                            if (chip is SN76489 || chip is RF5C164)
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

            } while (endChannel < (ym2612[0].ChMax + sn76489[0].ChMax + rf5c164[0].ChMax+ ym2612[1].ChMax + sn76489[1].ChMax + rf5c164[1].ChMax) );


            makeFooter();


            return dat.ToArray();
        }

        private void procEnvelope(partWork cpw)
        {
            //Envelope処理(PSGとRf5c164)
            if (cpw.chip is SN76489 || cpw.chip is RF5C164)
            {
                envelope(cpw);
            }

            if (cpw.chip is YM2612)
            {
                setFmFNum(cpw);
                setFmVolume(cpw);
            }
            else if (cpw.chip is SN76489)
            {
                if (cpw.waitKeyOnCounter > 0 || cpw.envIndex != -1)
                {
                    setPsgFNum(cpw);
                    setPsgVolume(cpw);
                }
            }
            else
            {
                if (cpw.waitKeyOnCounter > 0 || cpw.envIndex != -1)
                {
                    setRf5c164FNum(cpw);
                    setRf5c164Volume(cpw);
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
                if (pl.waitCounter >0)//== -1)
                {
                    continue;
                }

                if (pl.type == eLfoType.Hardware)
                {
                    if (cpw.chip is YM2612)
                    {
                        outFmSetPanAMSFMS(cpw, cpw.pan, pl.param[3], pl.param[2]);
                        outFmSetHardLfo(cpw.isSecondary, true, pl.param[1]);
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
                if (pw.chip is YM2612)
                {
                    if (!pw.tie) outFmKeyOff(pw);
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

                //chip.partStartCh = 0;
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

                    if (chip is YM2612)
                    {
                        pw.slots = (byte)(i < 6 ? 0xf : 0x0);
                        pw.volume = 127;
                    }
                    else
                    {
                        pw.slots = 0;
                        pw.volume = 32767;
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

            int maxValue = (pw.envelope[8] == (int)enmChipType.RF5C164) ? 255 : 15;

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
                if (pw.envelope[8] == (int)enmChipType.SN76489)
                {
                    outPsgKeyOff(pw);
                }
                else
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
                            if (pw.chip is YM2612)
                            {
                                a = getFmFNum(pw.octaveNow, cmd, shift + (i + 0) * Math.Sign(delta));//
                                b = getFmFNum(pw.octaveNow, cmd, shift + (i + 1) * Math.Sign(delta));//
                                int oa = (a & 0xf000) / 0x1000;
                                int ob = (b & 0xf000) / 0x1000;
                                if (oa != ob)
                                {
                                    if ((a & 0xfff) == fmFNumTbl[0])
                                    {
                                        oa += Math.Sign(ob - oa);
                                        a = (a & 0xfff) * 2 + oa * 0x1000;
                                    }
                                    else if ((b & 0xfff) == fmFNumTbl[0])
                                    {
                                        ob += Math.Sign(oa - ob);
                                        b = (b & 0xfff) * ((delta > 0) ? 2 : 1) + ob * 0x1000;
                                    }
                                }
                            }
                            else if (pw.chip is SN76489)
                            {
                                a = getPsgFNum(pw.octaveNow, cmd, shift + (i + 0) * Math.Sign(delta));//
                                b = getPsgFNum(pw.octaveNow, cmd, shift + (i + 1) * Math.Sign(delta));//
                            }
                            else
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
                if (pw.chip is YM2612)
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
                else
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

            foreach(clsChip chip in chips) {
                foreach (partWork p in chip.lstPartWork)
                {
                    p.freq = -1;

                    if (p.chip is RF5C164 && rf5c164[p.isSecondary ? 1 : 0].use)
                    {
                        //rf5c164の設定済み周波数値を初期化(ループ時に直前の周波数を引き継いでしまうケースがあるため)
                        p.rf5c164AddressIncrement = -1;
                        int n = p.instrument;
                        p.rf5c164SampleStartAddress = -1;
                        p.rf5c164LoopAddress = -1;
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
            if (((pw.chip is YM2612) && (pw.Type== enmChannelType.FMPCM || (pw.Type== enmChannelType.FMOPNex))) || (pw.chip is SN76489) || (pw.chip is RF5C164))
            {
                pw.incPos();
                switch (pw.getChar())
                {
                    case 'O':
                        pw.incPos();
                        if (pw.chip is YM2612)
                        {
                            switch (pw.getChar())
                            {
                                case 'N':
                                    pw.incPos();
                                    pw.Ch3SpecialMode = true;
                                    outFmSetCh3SpecialMode(pw.isSecondary,true);
                                    break;
                                case 'F':
                                    pw.incPos();
                                    pw.Ch3SpecialMode = false;
                                    outFmSetCh3SpecialMode(pw.isSecondary, false);
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
                        if (pw.chip is YM2612)
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
                outFmSetCh6PCMMode(pw.isSecondary, pw.pcm);
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
            n = checkRange(n, -127, 127);
            pw.detune = n;
        }

        private void cmdPan(partWork pw)
        {
            int n;
            int vch = pw.ch;

            pw.incPos();
            if (pw.chip is YM2612)
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
                outFmSetPanAMSFMS(pw, n, 0, 0);
            }
            else if (pw.chip is SN76489)
            {
                pw.getNum(out n);
                msgBox.setWrnMsg("PSGパートでは、pコマンドは無視されます。", lineNumber);
            }
            else
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
            n = checkRange(n, 1, (pw.chip is YM2612) ? fmMaxVolume : ((pw.chip is SN76489) ? psgMaxVolume : rf5c164MaxVolume));
            pw.volume -= n;
            if (pw.chip is YM2612)
            {
                pw.volume = checkRange(pw.volume, 0, fmMaxVolume);
            }
            else if (pw.chip is SN76489)
            {
                pw.volume = checkRange(pw.volume, 0, psgMaxVolume);
            }
            else
            {
                pw.volume = checkRange(pw.volume, 0, rf5c164MaxVolume);
            }

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
            n = checkRange(n, 1, (pw.chip is YM2612) ? fmMaxVolume : ((pw.chip is SN76489) ? psgMaxVolume : rf5c164MaxVolume));
            pw.volume += n;
            if (pw.chip is YM2612)
            {
                pw.volume = checkRange(pw.volume, 0, fmMaxVolume);
            }
            else if (pw.chip is SN76489)
            {
                pw.volume = checkRange(pw.volume, 0, psgMaxVolume);
            }
            else
            {
                pw.volume = checkRange(pw.volume, 0, rf5c164MaxVolume);
            }

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
            if (pw.chip is YM2612)
            {
                n = checkRange(n, 0, fmMaxVolume);
                if (pw.volume != n)
                {
                    pw.volume = n;
                }
            }
            else if (pw.chip is SN76489)
            {
                n = checkRange(n, 0, psgMaxVolume);
                pw.volume = n;
            }
            else
            {
                n = checkRange(n, 0, rf5c164MaxVolume);
                pw.volume = n;
            }
        }

        private void cmdInstrument(partWork pw)
        {
            int n;
            pw.incPos();
            if (pw.chip is YM2612)
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
                    if (pw.Type== enmChannelType.FMOPNex)
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
            }
            else
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
                }
            }
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

                while(pw.getChar()=='\t' || pw.getChar()==' ') { pw.incPos(); }

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
                        outFmSetHardLfo(pw.isSecondary, (n == 0) ? false : true, pw.lfo[c].param[1]);
                    }
                    else
                    {
                        outFmSetHardLfo(pw.isSecondary, false, pw.lfo[c].param[1]);
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

            if (pw.chip is YM2612)
            {
                outFmAdrPort(pw.isSecondary, (pw.ch > 3 && pw.ch < 6), adr, dat);
            }
            else if (pw.chip is SN76489)
            {
                outPsgPort(pw.isSecondary, dat);
            }
            else
            {
                outRf5c164Port(pw.isSecondary, adr, dat);
            }
        }

        private void cmdNoise(partWork pw)
        {
            int n = -1;
            pw.incPos();
            if (pw.ch != 3)
            {
                msgBox.setErrMsg("このチャンネルではwコマンドは使用できません。", lineNumber);
                return;
            }

            if (pw.getNum(out n))
            {
                pw.noise = checkRange(n, 0, 7);
            }
            else
            {
                msgBox.setErrMsg("wコマンドに指定された値が不正です。", lineNumber);
                return;

            }
        }

        private void cmdKeyShift(partWork pw)
        {
            int n = -1;
            pw.incPos();

            if (pw.getNum(out n))
            {
                pw.keyShift = checkRange(n,-128,128);
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

            //PCM Data block
            for (int i = 0; i < 2; i++)
            {
                if (rf5c164[i].use && rf5c164[i].pcmData != null && rf5c164[i].pcmData.Length > 0)
                {
                    foreach (byte b in rf5c164[i].pcmData)
                    {
                        dat.Add(b);
                    }
                }
            }

            for (int i = 0; i < 2; i++)
            {
                if (ym2612[i].use)
                {
                    outFmSetHardLfo(i > 0, false, 0);
                    outFmSetCh3SpecialMode(i > 0, false);
                    outFmSetCh6PCMMode(i > 0, false);

                    outFmAllKeyOff(i > 0);

                    for (int ch = 0; ch < ym2612[i].ChMax; ch++)
                    {
                        if (ym2612[i].Ch[ch].Type == enmChannelType.FMOPN || ym2612[i].Ch[ch].Type == enmChannelType.FMPCM)
                        {
                            if (!ym2612[i].lstPartWork[ch].dataEnd) outFmSetPanAMSFMS(ym2612[i].lstPartWork[ch], 3, 0, 0);
                        }
                    }

                    if (i != 0) dat[0x2f] |= 0x40;
                }
            }

            if (sn76489[0].use)
            {
            }

            if (sn76489[1].use)
            {
                dat[0x0f] |= 0x40;
            }

            for (int i = 0; i < 2; i++)
            {
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

            long useYM2612 = 0;
            long useSN76489 = 0;
            long useRf5c164 = 0;

            for (int i = 0; i < 2; i++)
            {
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
            int maxValue = (pw.envelope[8] == (int)enmChipType.RF5C164) ? 255 : 15;

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
            for(int lfo = 0; lfo < 4; lfo++)
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
                            outFmSetHardLfo(pw.isSecondary, false, pl.param[1]);
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
                    if (pw.chip is YM2612)
                    {
                        setFmFNum(pw);
                    }
                }
                if (pl.type == eLfoType.Tremolo)
                {
                    if (pw.chip is YM2612)
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
            int f = getFmFNum(pw.octaveNow, pw.noteCmd, pw.shift + pw.keyShift);//
            if (pw.bendWaitCounter !=-1)
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
            while (f < fmFNumTbl[0])
            {
                if (o == 1)
                {
                    break;
                }
                o--;
                f = fmFNumTbl[0] * 2 - (fmFNumTbl[0] - f);
            }
            while (f >= fmFNumTbl[0] * 2)
            {
                if (o == 8)
                {
                    break;
                }
                o++;
                f = f - fmFNumTbl[0] * 2 + fmFNumTbl[0];
            }
            f = checkRange(f, 0, 0x7ff);

            outFmSetFnum(pw, o, f);
        }

        private int getFmFNum(int octave, char noteCmd, int shift)
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

            int f = fmFNumTbl[n];

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

        private int getPsgFNum(int octave,char noteCmd,int shift)
        {
            int o = octave-1;
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

        private void setRf5c164AddressIncrement(partWork pw,int f)
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
            if (pw.rf5c164SampleStartAddress != adr)
            {
                byte data = (byte)((adr >> 8) & 0xff);
                outRf5c164Port(pw.isSecondary, 0x6, data);
                pw.rf5c164SampleStartAddress = adr;
            }
        }

        private void setRf5c164LoopAddress(partWork pw, int adr)
        {
            if (pw.rf5c164LoopAddress != adr)
            {
                byte data = (byte)((adr >> 8) & 0xff);
                outRf5c164Port(pw.isSecondary, 0x5, data);
                data = (byte)(adr & 0xff);
                outRf5c164Port(pw.isSecondary, 0x4, data);
                pw.rf5c164LoopAddress = adr;
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

        private void outRf5c164Port(bool isSecondary,byte adr, byte data)
        {
            dat.Add(0xb1);
            dat.Add((byte)((adr & 0x7f) | (isSecondary ? 0x80 : 0x00)));
            dat.Add(data);
        }




        private void outFmAdrPort(bool isSecondary, bool isPort2, byte address, byte data)
        {
            outFmAdrPort(isSecondary,(byte)(isPort2 ? 0x53 : 0x52), address, data);
        }

        private void outFmAdrPort(bool isSecondary,byte port, byte address, byte data)
        {
            dat.Add((byte)((port & 0xf) + (isSecondary ? 0xa0 : 0x50)));
            dat.Add(address);
            dat.Add(data);
        }

        private void outFmKeyOn(partWork pw)
        {
            if (!pw.pcm)
            {
                if (ym2612[pw.chip.ChipID].lstPartWork[2].Ch3SpecialMode && pw.Type== enmChannelType.FMOPNex)
                {
                    pw.Ch3SpecialModeKeyOn = true;

                    int slot = (ym2612[pw.chip.ChipID].lstPartWork[2].Ch3SpecialModeKeyOn ? ym2612[pw.chip.ChipID].lstPartWork[2].slots : 0x0)
                        | (ym2612[pw.chip.ChipID].lstPartWork[6].Ch3SpecialModeKeyOn ? ym2612[pw.chip.ChipID].lstPartWork[6].slots : 0x0)
                        | (ym2612[pw.chip.ChipID].lstPartWork[7].Ch3SpecialModeKeyOn ? ym2612[pw.chip.ChipID].lstPartWork[7].slots : 0x0)
                        | (ym2612[pw.chip.ChipID].lstPartWork[8].Ch3SpecialModeKeyOn ? ym2612[pw.chip.ChipID].lstPartWork[8].slots : 0x0);

                    outFmAdrPort(pw.isSecondary, 0x52, 0x28, (byte)((slot << 4) + 2));
                }
                else
                {
                    if (pw.ch >= 0 && pw.ch < 6)
                    {
                        byte vch = (byte)((pw.ch > 2) ? pw.ch + 1 : pw.ch);
                        //key on
                        outFmAdrPort(pw.isSecondary, 0x52, 0x28, (byte)((pw.slots << 4) + (vch & 7)));
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
                if (ym2612[pw.chip.ChipID].lstPartWork[2].Ch3SpecialMode && pw.Type== enmChannelType.FMOPNex)
                {
                    pw.Ch3SpecialModeKeyOn = false;

                    int slot = (ym2612[pw.chip.ChipID].lstPartWork[2].Ch3SpecialModeKeyOn ? ym2612[pw.chip.ChipID].lstPartWork[2].slots : 0x0)
                        | (ym2612[pw.chip.ChipID].lstPartWork[6].Ch3SpecialModeKeyOn ? ym2612[pw.chip.ChipID].lstPartWork[6].slots : 0x0)
                        | (ym2612[pw.chip.ChipID].lstPartWork[7].Ch3SpecialModeKeyOn ? ym2612[pw.chip.ChipID].lstPartWork[7].slots : 0x0)
                        | (ym2612[pw.chip.ChipID].lstPartWork[8].Ch3SpecialModeKeyOn ? ym2612[pw.chip.ChipID].lstPartWork[8].slots : 0x0);

                    outFmAdrPort(pw.isSecondary,0x52, 0x28, (byte)((slot << 4) + 2));
                }
                else
                {
                    if (pw.ch >= 0 && pw.ch < 6)
                    {
                        byte vch = (byte)((pw.ch > 2) ? pw.ch + 1 : pw.ch);
                        //key off
                        outFmAdrPort(pw.isSecondary,0x52, 0x28, (byte)(0x00 + (vch & 7)));
                    }
                }

                return;
            }

            pw.pcmWaitKeyOnCounter = -1;
        }

        private void outFmAllKeyOff(bool isSecondary)
        {
            int chipID = isSecondary ? 1 : 0;

            for (int ch = 0; ch < ym2612[chipID].ChMax; ch++)
            {
                partWork pw = ym2612[chipID].lstPartWork[ch];
                if (ch < 6)
                {
                    if (!ym2612[chipID].lstPartWork[ch].dataEnd)
                    {
                        outFmKeyOff(pw);
                        outFmSetTl(pw, 0, 127);
                        outFmSetTl(pw, 1, 127);
                        outFmSetTl(pw, 2, 127);
                        outFmSetTl(pw, 3, 127);
                    }
                }
            }
        }

        private void outFmSetFnum(partWork pw, int octave, int num)
        {
            int freq;
            freq = ((num & 0x700) >> 8) + (((octave - 1) & 0x7) << 3);
            freq = (freq << 8) + (num & 0xff);

            if (freq == pw.freq) return;

            pw.freq = freq;

            if (ym2612[pw.chip.ChipID].lstPartWork[2].Ch3SpecialMode && pw.Type == enmChannelType.FMOPNex)
            {
                byte port = (byte)0x52;

                if ((pw.slots & 8) != 0)
                {
                    int f = pw.freq + pw.slotDetune[3];
                    outFmAdrPort(pw.isSecondary, port, (byte)0xa6, (byte)((f & 0xff00) >> 8));
                    outFmAdrPort(pw.isSecondary, port, (byte)0xa2, (byte)(f & 0xff));
                }
                if ((pw.slots & 4) != 0)
                {
                    int f = pw.freq + pw.slotDetune[2];
                    outFmAdrPort(pw.isSecondary, port, (byte)0xac, (byte)((f & 0xff00) >> 8));
                    outFmAdrPort(pw.isSecondary, port, (byte)0xa8, (byte)(f & 0xff));
                }
                if ((pw.slots & 1) != 0)
                {
                    int f = pw.freq + pw.slotDetune[0];
                    outFmAdrPort(pw.isSecondary, port, (byte)0xad, (byte)((f & 0xff00) >> 8));
                    outFmAdrPort(pw.isSecondary, port, (byte)0xa9, (byte)(f & 0xff));
                }
                if ((pw.slots & 2) != 0)
                {
                    int f = pw.freq + pw.slotDetune[1];
                    outFmAdrPort(pw.isSecondary, port, (byte)0xae, (byte)((f & 0xff00) >> 8));
                    outFmAdrPort(pw.isSecondary, port, (byte)0xaa, (byte)(f & 0xff));
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
                    byte port = (byte)(0x52 + (pw.ch > 2 ? 1 : 0));
                    byte vch = (byte)(pw.ch > 2 ? pw.ch - 3 : pw.ch);

                    outFmAdrPort(pw.isSecondary, port, (byte)(0xa4 + vch), (byte)((pw.freq & 0xff00) >> 8));
                    outFmAdrPort(pw.isSecondary, port, (byte)(0xa0 + vch), (byte)(pw.freq & 0xff));
                }
            }
        }

        private void outFmCh3SpecialModeSetFnum(bool isSecondary, byte ope, int octave, int num)
        {
            byte port = (byte)0x52;
            ope &= 3;
            if (ope == 0)
            {
                outFmAdrPort(isSecondary, port, (byte)(0xa6), (byte)(((num & 0x700) >> 8) + (((octave - 1) & 0x7) << 3)));
                outFmAdrPort(isSecondary, port, (byte)(0xa2), (byte)(num & 0xff));
            }
            else
            {
                outFmAdrPort(isSecondary, port, (byte)(0xac + ope), (byte)(((num & 0x700) >> 8) + (((octave - 1) & 0x7) << 3)));
                outFmAdrPort(isSecondary, port, (byte)(0xa8 + ope), (byte)(num & 0xff));
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
            byte port = (byte)(0x52 + (vch > 2 ? 1 : 0));
            vch = (byte)(vch > 2 ? vch - 3 : vch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            dt &= 7;
            ml &= 15;

            outFmAdrPort(pw.isSecondary, port, (byte)(0x30 + vch + ope * 4), (byte)((dt << 4) + ml));
        }

        private void outFmSetTl(partWork pw, int ope, int tl)
        {
            byte port = (byte)(0x52 + (pw.ch > 2 ? 1 : 0));
            int vch = (byte)(pw.ch > 2 ? pw.ch - 3 : pw.ch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            tl &= 0x7f;

            outFmAdrPort(pw.isSecondary, port,(byte)(0x40 + vch + ope * 4),(byte)tl);
        }

        private void outFmSetKsAr(partWork pw, int ope, int ks, int ar)
        {
            int vch = pw.ch;
            byte port = (byte)(0x52 + (vch > 2 ? 1 : 0));
            vch = (byte)(vch > 2 ? vch - 3 : vch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            ks &= 3;
            ar &= 31;

            outFmAdrPort(pw.isSecondary, port, (byte)(0x50 + vch + ope * 4), (byte)((ks << 6) + ar));
        }

        private void outFmSetAmDr(partWork pw, int ope, int am,int dr)
        {
            int vch = pw.ch;
            byte port = (byte)(0x52 + (vch > 2 ? 1 : 0));
            vch = (byte)(vch > 2 ? vch - 3 : vch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            am &= 1;
            dr &= 31;

            outFmAdrPort(pw.isSecondary,port, (byte)(0x60 + vch + ope * 4), (byte)((am << 7) + dr));
        }

        private void outFmSetSr(partWork pw, int ope, int sr)
        {
            int vch = pw.ch;
            byte port = (byte)(0x52 + (vch > 2 ? 1 : 0));
            vch = (byte)(vch > 2 ? vch - 3 : vch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            sr &= 31;

            dat.Add((byte)((port & 0xf) + (pw.isSecondary ? 0xA0 : 0x50)));
            dat.Add((byte)(0x70 + vch + ope * 4));
            dat.Add((byte)sr);
        }

        private void outFmSetSlRr(partWork pw, int ope, int sl, int rr)
        {
            int vch = pw.ch;
            byte port = (byte)(0x52 + (vch > 2 ? 1 : 0));
            vch = (byte)(vch > 2 ? vch - 3 : vch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            sl &= 15;
            rr &= 15;

            dat.Add((byte)((port & 0xf) + (pw.isSecondary ? 0xA0 : 0x50)));
            dat.Add((byte)(0x80 + vch + ope * 4));
            dat.Add((byte)((sl << 4) + rr));
        }

        private void outFmSetSSGEG(partWork pw, int ope, int n)
        {
            int vch = pw.ch;
            byte port = (byte)(0x52 + (vch > 2 ? 1 : 0));
            vch = (byte)(vch > 2 ? vch - 3 : vch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            n &= 15;

            dat.Add((byte)((port & 0xf) + (pw.isSecondary ? 0xA0 : 0x50)));
            dat.Add((byte)(0x90 + vch + ope * 4));
            dat.Add((byte)n);
        }

        private void outFmSetFeedbackAlgorithm(partWork pw, int fb, int alg)
        {
            int vch = pw.ch;
            byte port = (byte)(0x52 + (vch > 2 ? 1 : 0));
            vch = (byte)(vch > 2 ? vch - 3 : vch);

            fb &= 7;
            alg &= 7;

            dat.Add((byte)((port & 0xf) + (pw.isSecondary ? 0xA0 : 0x50)));
            dat.Add((byte)(0xb0 + vch));
            dat.Add((byte)((fb << 3) + alg));
        }

        private void outFmSetPanAMSFMS(partWork pw, int pan, int ams, int fms)
        {
            int vch = pw.ch;
            byte port = (byte)(0x52 + (vch > 2 ? 1 : 0));
            vch = (byte)(vch > 2 ? vch - 3 : vch);

            pan = pan & 3;
            ams = ams & 3;
            fms = fms & 3;

            dat.Add((byte)((port & 0xf) + (pw.isSecondary ? 0xA0 : 0x50)));
            dat.Add((byte)(0xb4 + vch));
            dat.Add((byte)((pan << 6) + (ams << 4) + fms));
        }

        private void outFmSetHardLfo(bool isSecondary,bool sw,int lfoNum)
        {
            dat.Add((byte)(0x2 + (isSecondary ? 0xA0 : 0x50)));
            dat.Add(0x22);
            dat.Add((byte)((lfoNum & 7) + (sw ? 8 : 0)));
        }

        private void outFmSetCh3SpecialMode(bool isSecondary, bool sw)
        {
            // ignore Timer ^^;
            dat.Add((byte)(0x2 + (isSecondary ? 0xA0 : 0x50)));
            dat.Add(0x27);
            dat.Add((byte)((sw ? 0x40 : 0)));
        }

        private void outFmSetCh6PCMMode(bool isSecondary, bool sw)
        {
            dat.Add((byte)(0x2 + (isSecondary ? 0xA0 : 0x50)));
            dat.Add(0x2b);
            dat.Add((byte)((sw ? 0x80 : 0)));
        }



        private void outPsgPort(bool isSecondary, byte data)
        {
            dat.Add((byte)(isSecondary ? 0x30 : 0x50));
            dat.Add(data);
        }

        private void _outPsgSetFnum(partWork pw, int freq)
        {
            pw.freq=freq;
        }

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
            long m=n;

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

        private void outWaitNSamplesWithPCMSending(partWork cpw,long cnt)
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
