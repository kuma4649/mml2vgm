using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mml2vgm
{
    public class clsVgm
    {

        public Dictionary<int, byte[]> instFM = new Dictionary<int, byte[]>();
        public Dictionary<int, int[]> instENV = new Dictionary<int, int[]>();
        public Dictionary<int, clsPcm> instPCM = new Dictionary<int, clsPcm>();
        public Dictionary<string, List<Tuple<int, string>>> partData = new Dictionary<string, List<Tuple<int, string>>>();
        public Dictionary<string, Tuple<int, string>> aliesData = new Dictionary<string, Tuple<int, string>>();
        public byte[] pcmDataYM2612 = null;
        public byte[] pcmDataRf5c164 = null;
        public List<string> monoPart = null;


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
             0x6ae,0x64e,0x5f4,0x59e,0x54e,0x502,0x4ba,0x476,0x436,0x3f8,0x3c0,0x38a//1
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

        public string[] fCh = new string[]
        {
            //1-6 normal  7-9 effect mode channel
            "F1","F2","F3","F4","F5","F6"  ,"F7","F8","F9"
        };

        public string[] sCh = new string[]
        {
            "S1","S2","S3","S4"
        };

        public string[] rCh = new string[]
        {
            "R1","R2","R3","R4","R5","R6","R7","R8"
        };

        public int lineNumber = 0;

        private const int instrumentSize = 39+8;
        private const int instrumentOperaterSize = 9;
        private const int instrumentMOperaterSize = 11;

        private byte rf5c164KeyOn = 0x0;
        private byte rf5c164CurrentChannel = 0xff;



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

        private const string PARTYM2612 = "PARTYM2612";
        private const string PARTYM2612CH3 = "PARTYM2612CH3";
        private const string PARTYM2612CH6 = "PARTYM2612CH6";
        private const string PARTSN76489 = "PARTSN76489";
        private const string PARTRF5C164 = "PARTRF5C164";
        private const string CLOCKCOUNT = "CLOCKCOUNT";
        private const string FMF_NUM = "FMF-NUM";
        private const string PSGF_NUM = "PSGF-NUM";
        private const string FORCEDMONOPARTYM2612 = "FORCEDMONOPARTYM2612";

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


        private int instrumentCounter = -1;
        private byte[] instrumentBufCache = new byte[instrumentSize];


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
                if(!fCh.Contains(p) && !sCh.Contains(p) && !rCh.Contains(p))
                {
                    msgBox.setWrnMsg(string.Format("未定義のパート({0})のデータは無視されます。", p));
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

                    if (wrd == PARTYM2612) setPartToF_Ch(0, val);
                    if (wrd == PARTYM2612CH3) setPartToF_Ch(2, val);
                    if (wrd == PARTYM2612CH6) setPartToF_Ch(5, val);
                    if (wrd == PARTSN76489) setPartToS_Ch(val);
                    if (wrd == PARTRF5C164) setPartToRf5c164_Ch(val);
                    if (wrd == CLOCKCOUNT) clockCount = int.Parse(val);
                    if (wrd == FMF_NUM) setFmF_NumTbl(val);
                    if (wrd == FORCEDMONOPARTYM2612) setMonoPart(val);

                    for(int i = 0; i < 8; i++)
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

        private void setPartToS_Ch(string val)
        {
            List<string> ss = divParts(val);
            if (ss.Count > 4)
            {
                ss.RemoveRange(4, ss.Count - 4);
            }
            for (int i = 0; i < ss.Count; i++)
            {
                sCh[i] = ss[i];
            }
            checkDuplication(sCh);
        }

        private void setPartToRf5c164_Ch(string val)
        {
            List<string> ss = divParts(val);
            if (ss.Count > 8)
            {
                ss.RemoveRange(8, ss.Count - 8);
            }
            for (int i = 0; i < ss.Count; i++)
            {
                rCh[i] = ss[i];
            }
            checkDuplication(rCh);
        }

        private void setPartToF_Ch(int v, string val)
        {
            List<string> ss = divParts(val);
            if (v == 0)
            {
                for (int i = 0; i < 6; i++)
                {
                    if (i < ss.Count) fCh[i] = ss[i];
                }
            }
            else if (v == 2)
            {
                if (ss.Count > 0) fCh[2] = ss[0];
                if (ss.Count > 1) fCh[6] = ss[1];
                if (ss.Count > 2) fCh[7] = ss[2];
                if (ss.Count > 3) fCh[8] = ss[3];
            }
            else if (v == 5)
            {
                if (ss.Count > 0) fCh[5] = ss[0];
            }

            checkDuplication(fCh);
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
                        int chip = 0;
                        string[] vs = s.Substring(1).Trim().Split(new string[] { "," }, StringSplitOptions.None);
                        int num = int.Parse(vs[0]);
                        string fn = vs[1].Trim().Trim('"');
                        int fq = int.Parse(vs[2]);
                        int vol = int.Parse(vs[3]);
                        int lp = -1;
                        if (vs.Length > 4)
                        {
                            string chipName = vs[4].Trim().ToUpper();
                            chip = getChipNumber(chipName);
                            if (!canUsePCM(chip))
                            {
                                throw new ArgumentOutOfRangeException();
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
                        instPCM.Add(num, new clsPcm(num, chip, fn, fq, vol, 0, 0, lp));
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
                                if (vs.Length == 8) env[i] = getChipNumber("SN76489");
                                else env[i] = getChipNumber(vs[8]);
                                continue;
                            }
                            env[i] = int.Parse(vs[i]);
                        }

                        for(int i=0;i<env.Length-1;i++) {
                            if (env[8] == 1)
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
                            else if (env[8] == 2)
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
                    //default:
                    //    msgBox.setErrMsg("空の音色定義文を受け取りました。", lineNumber);
                    //    return -1;
            }

            return 0;
        }

        private static int getChipNumber(string chipN)
        {
            int chip;
            switch (chipN.ToUpper().Trim())
            {
                case "YM2612":
                    chip = 0;
                    break;
                case "SN76489":
                    chip = 1;
                    break;
                case "RF5C164":
                    chip = 2;
                    break;
                default:
                    chip = -1;
                    break;
            }

            return chip;
        }

        private static bool canUsePCM(int chipNumber)
        {
            bool use = false;

            switch (chipNumber)
            {
                case 0:
                    use = true;
                    break;
                case 1:
                    use = false;
                    break;
                case 2:
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
            char a = ' ';
            char n0 = ' ';
            char n1 = ' ';
            int m = 0;

            for (int i = 0; i < parts.Length; i++)
            {
                if (m == 0 && parts[i] >= 'A' && parts[i] <= 'Z')
                {
                    a = parts[i];
                    m = 1;
                }
                else if (parts[i] == ',')
                {
                    m = 0;
                }
                else if (m == 1 && parts[i] >= '0' && parts[i] <= '9')
                {
                    n0 = parts[i];
                    n1 = (char)(parts[i]+1);
                    if (i < parts.Length - 2 && parts[i + 1] == '-')
                    {
                        n1 = (char)(parts[i + 2] + 1);
                        i += 2;
                        m = 0;
                        if (n0 > n1)
                        {
                            return null;
                        }
                        do
                        {
                            string p = string.Format("{0}{1}", a, n0);
                            if (ret.Contains(p))
                            {
                                return null;
                            }
                            ret.Add(p);
                            n0++;
                        } while (n0 != n1);
                    }
                    else
                    {
                        do
                        {
                            n0 = parts[i];
                            string p = string.Format("{0}{1}", a, n0);
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
        private float pcmBaseFreqPerFreq = 0.0f;
        private float pcmFreqCountBuffer = 0.0f;
        private long waitKeyOnPcmCounter = 0L;
        private long pcmSizeCounter = 0L;
        public partWork[] pw = null;
        private Random rnd = new Random();


        public byte[] getByteData()
        {

            pw = new partWork[21];
            for (int i = 0; i < 9; i++)
            {
                pw[i] = new partWork();
                pw[i].type = (i < 6) ? ePartType.YM2612 : ePartType.YM2612extend;
                pw[i].ch = i + 1;// (i < 6) ? (i + 1) : (i - 8);
                pw[i].partName = fCh[i];
                if (partData.ContainsKey(fCh[i]))
                {
                    pw[i].pData = partData[fCh[i]];
                }
                pw[i].aData = aliesData;
                pw[i].setPos(0);
                pw[i].slots = (byte)((i < 6) ? 0xf : 0x0);
                pw[i].volume = 127;
            }
            for (int i = 9; i < 13; i++)
            {
                pw[i] = new partWork();
                pw[i].type = ePartType.SegaPSG;
                pw[i].ch = (i - 8);// < 6) ? (i + 1) : (i - 8);
                pw[i].partName = sCh[i - 9];
                if (partData.ContainsKey(sCh[i - 9]))
                {
                    pw[i].pData = partData[sCh[i - 9]];
                }
                pw[i].aData = aliesData;
                pw[i].setPos(0);
                pw[i].slots = (byte)(0x0);
                pw[i].volume = 32767;
            }
            for (int i = 13; i < 21; i++)
            {
                pw[i] = new partWork();
                pw[i].type = ePartType.Rf5c164;
                pw[i].ch = (i - 12);// < 6) ? (i + 1) : (i - 8);
                pw[i].partName = rCh[i - 13];
                if (partData.ContainsKey(rCh[i - 13]))
                {
                    pw[i].pData = partData[rCh[i - 13]];
                }
                pw[i].aData = aliesData;
                pw[i].setPos(0);
                pw[i].slots = (byte)(0x0);
                pw[i].volume = 32767;
            }

            dat = new List<byte>();

            makeHeader();

            int endChannel = 0;


            do
            {

                if (waitKeyOnPcmCounter == 0)
                {
                    waitKeyOnPcmCounter = -1;
                }

                for (int ch = 0; ch < pw.Length; ch++)
                {

                    if (pw[ch].waitKeyOnCounter == 0)
                    {
                        if (ch < 9)
                        {
                            if (!pw[ch].tie) outFmKeyOff((byte)ch);
                        }
                        else if (ch < 13)
                        {
                            if (!pw[ch].envelopeMode)
                            {
                                if (!pw[ch].tie) outPsgKeyOff((byte)ch);
                            }
                            else
                            {
                                if (pw[ch].envIndex != -1)
                                {
                                    if (!pw[ch].tie)
                                    {
                                        pw[ch].envIndex = 3;//RR phase
                                        pw[ch].envCounter = 0;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (!pw[ch].envelopeMode)
                            {
                                if (!pw[ch].tie) outRf5c164KeyOff((byte)ch);
                            }
                            else
                            {
                                if (pw[ch].envIndex != -1)
                                {
                                    if (!pw[ch].tie)
                                    {
                                        pw[ch].envIndex = 3;//RR phase
                                        pw[ch].envCounter = 0;
                                    }
                                }
                            }
                        }


                        //次回に引き継ぎリセット
                        pw[ch].beforeTie = pw[ch].tie;
                        pw[ch].tie = false;

                        //ゲートタイムカウンターをリセット
                        pw[ch].waitKeyOnCounter = -1;
                    }

                    //bend処理
                    if (pw[ch].bendWaitCounter == 0)
                    {
                        if (pw[ch].bendList.Count > 0)
                        {
                            Tuple<int, int> bp = pw[ch].bendList.Pop();
                            pw[ch].bendFnum = bp.Item1;
                            pw[ch].bendWaitCounter = bp.Item2;
                        }
                        else
                        {
                            pw[ch].bendWaitCounter = -1;
                        }
                    }

                    //lfo処理
                    for (int lfo = 0; lfo < 4; lfo++)
                    {
                        clsLfo pl = pw[ch].lfo[lfo];

                        if (!pl.sw)
                        {
                            continue;
                        }
                        if (pl.waitCounter != 0)
                        {
                            continue;
                        }

                        if (pl.type == eLfoType.Hardware)
                        {
                            if (ch < 9)
                            {
                                outFmSetPanAMSFMS((byte)ch, pw[ch].pan, pw[ch].lfo[lfo].param[3], pw[ch].lfo[lfo].param[2]);
                                outFmSetHardLfo(true, pw[ch].lfo[lfo].param[1]);
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

                    //Envelope処理(PSGとRf5c164)
                    if ((ch >= 9 && ch <= 12) || (ch >= 13 && ch <= 20))
                    {
                        envelope(ch);
                    }

                    if (ch < 9)
                    {
                        setFmFNum(ch);
                        setFmVolume(ch);
                    }
                    else if (ch < 13)
                    {
                        if (pw[ch].waitKeyOnCounter > 0 || pw[ch].envIndex != -1)
                        {
                            setPsgFNum(ch);
                            setPsgVolume(ch);
                        }
                    }
                    else
                    {
                        if (pw[ch].waitKeyOnCounter > 0 || pw[ch].envIndex != -1)
                        {
                            setRf5c164FNum(ch);
                            setRf5c164Volume(ch);
                        }
                    }

                    //wait消化待ち
                    if (pw[ch].waitCounter > 0)
                    {
                        continue;
                    }

                    //データは最後まで実施されたか
                    if (pw[ch].dataEnd)
                    {
                        continue;
                    }

                    //パートのデータがない場合は何もしないで次へ
                    if (ch < 9)
                    {
                        if (!partData.ContainsKey(fCh[ch]) || partData[fCh[ch]] == null || partData[fCh[ch]].Count < 1)
                        {
                            pw[ch].dataEnd = true;
                            continue;
                        }
                    }
                    else if (ch < 13)
                    {
                        if (!partData.ContainsKey(sCh[ch - 9]) || partData[sCh[ch - 9]] == null || partData[sCh[ch - 9]].Count < 1)
                        {
                            pw[ch].dataEnd = true;
                            continue;
                        }
                    }
                    else if (ch < 21)
                    {
                        if (!partData.ContainsKey(rCh[ch - 13]) || partData[rCh[ch - 13]] == null || partData[rCh[ch - 13]].Count < 1)
                        {
                            pw[ch].dataEnd = true;
                            continue;
                        }
                    }


                    while (pw[ch].waitCounter == 0 && !pw[ch].dataEnd)
                    {
                        char cmd = pw[ch].getChar();
                        lineNumber = pw[ch].getLineNumber();

                        commander(ch, cmd);
                    }
                }


                // 全パートのうち次のコマンドまで一番近い値を求める
                long cnt = long.MaxValue;
                //note
                for (int ch = 0; ch < pw.Length; ch++)
                {
                    if (pw[ch].waitKeyOnCounter > 0)
                    {
                        cnt = Math.Min(cnt, pw[ch].waitKeyOnCounter);
                        continue;
                    }

                    if (pw[ch].waitCounter > 0)
                    {
                        cnt = Math.Min(cnt, pw[ch].waitCounter);
                    }
                }
                //bend
                for (int ch = 0; ch < pw.Length; ch++)
                {
                    if (pw[ch].bendWaitCounter == -1)
                    {
                        continue;
                    }
                    cnt = Math.Min(cnt, pw[ch].bendWaitCounter);
                }
                //lfo
                for (int ch = 0; ch < pw.Length; ch++)
                {
                    for (int lfo = 0; lfo < 4; lfo++)
                    {
                        if (!pw[ch].lfo[lfo].sw)
                        {
                            continue;
                        }
                        if (pw[ch].lfo[lfo].waitCounter == -1)
                        {
                            continue;
                        }
                        cnt = Math.Min(cnt, pw[ch].lfo[lfo].waitCounter);
                    }
                }
                //pcm
                if (waitKeyOnPcmCounter > 0)
                {
                    cnt = Math.Min(cnt, waitKeyOnPcmCounter);
                }
                //envelope
                for (int ch = 9; ch < 13; ch++)
                {
                    if (!pw[ch].envelopeMode)
                    {
                        continue;
                    }
                    if (pw[ch].envIndex == -1)
                    {
                        continue;
                    }
                    cnt = Math.Min(cnt, pw[ch].envCounter);
                }
                //envelope
                for (int ch = 13; ch < 21; ch++)
                {
                    if (!pw[ch].envelopeMode)
                    {
                        continue;
                    }
                    if (pw[ch].envIndex == -1)
                    {
                        continue;
                    }
                    cnt = Math.Min(cnt, pw[ch].envCounter);
                }

                if (cnt != long.MaxValue)
                {

                    // waitcounterを減らす

                    for (int ch = 0; ch < pw.Length; ch++)
                    {

                        if (pw[ch].waitKeyOnCounter > 0)
                        {
                            pw[ch].waitKeyOnCounter -= cnt;
                        }

                        if (pw[ch].waitCounter > 0)
                        {
                            pw[ch].waitCounter -= cnt;
                        }

                        if (pw[ch].bendWaitCounter > 0)
                        {
                            pw[ch].bendWaitCounter -= cnt;
                        }

                        for (int lfo = 0; lfo < 4; lfo++)
                        {
                            if (!pw[ch].lfo[lfo].sw)
                            {
                                continue;
                            }
                            if (pw[ch].lfo[lfo].waitCounter == -1)
                            {
                                continue;
                            }
                            if (pw[ch].lfo[lfo].waitCounter > 0)
                            {
                                pw[ch].lfo[lfo].waitCounter -= cnt;
                            }
                        }
                    }

                    if (waitKeyOnPcmCounter > 0)
                    {
                        waitKeyOnPcmCounter -= cnt;
                    }

                    for (int ch = 9; ch < 13; ch++)
                    {
                        if (!pw[ch].envelopeMode)
                        {
                            continue;
                        }
                        if (pw[ch].envIndex == -1)
                        {
                            continue;
                        }
                        pw[ch].envCounter -= (int)cnt;
                    }

                    for (int ch = 13; ch < 21; ch++)
                    {
                        if (!pw[ch].envelopeMode)
                        {
                            continue;
                        }
                        if (pw[ch].envIndex == -1)
                        {
                            continue;
                        }
                        pw[ch].envCounter -= (int)cnt;
                    }


                    // wait発行

                    lClock += cnt;
                    lSample += (long)samplesPerClock * cnt;

                    if (waitKeyOnPcmCounter == -1)
                    {
                        outWaitNSamples((long)samplesPerClock * cnt);
                    }
                    else
                    {
                        outWaitNSamplesWithPCMSending(cnt);
                    }
                }



                endChannel = 0;
                for (int ch = 0; ch < pw.Length; ch++)
                {
                    if (pw[ch].dataEnd && pw[ch].waitCounter < 1)
                    {
                        endChannel++;
                    }
                }

            } while (endChannel < fCh.Length+sCh.Length+rCh.Length);


            makeFooter();


            return dat.ToArray();
        }

        private void envelope(int ch)
        {
            if (!pw[ch].envelopeMode)
            {
                return;
            }

            if (pw[ch].envIndex == -1)
            {
                return;
            }

            int maxValue = (pw[ch].envelope[8] == 2) ? 255 : 15;

            while (pw[ch].envCounter == 0 && pw[ch].envIndex != -1)
            {
                switch (pw[ch].envIndex)
                {
                    case 0: //AR phase
                        pw[ch].envVolume += pw[ch].envelope[7]; // vol += ST
                        if (pw[ch].envVolume >= maxValue)
                        {
                            pw[ch].envVolume = maxValue;
                            pw[ch].envCounter = pw[ch].envelope[3]; // DR
                            pw[ch].envIndex++;
                            break;
                        }
                        pw[ch].envCounter = pw[ch].envelope[2]; // AR
                        break;
                    case 1: //DR phase
                        pw[ch].envVolume -= pw[ch].envelope[7]; // vol -= ST
                        if (pw[ch].envVolume <= pw[ch].envelope[4]) // vol <= SL
                        {
                            pw[ch].envVolume = pw[ch].envelope[4];
                            pw[ch].envCounter = pw[ch].envelope[5]; // SR
                            pw[ch].envIndex++;
                            break;
                        }
                        pw[ch].envCounter = pw[ch].envelope[3]; // DR
                        break;
                    case 2: //SR phase
                        pw[ch].envVolume -= pw[ch].envelope[7]; // vol -= ST
                        if (pw[ch].envVolume <= 0) // vol <= 0
                        {
                            pw[ch].envVolume = 0;
                            pw[ch].envCounter = 0;
                            pw[ch].envIndex = -1;
                            break;
                        }
                        pw[ch].envCounter = pw[ch].envelope[5]; // SR
                        break;
                    case 3: //RR phase
                        pw[ch].envVolume -= pw[ch].envelope[7]; // vol -= ST
                        if (pw[ch].envVolume <= 0) // vol <= 0
                        {
                            pw[ch].envVolume = 0;
                            pw[ch].envCounter = 0;
                            pw[ch].envIndex = -1;
                            break;
                        }
                        pw[ch].envCounter = pw[ch].envelope[6]; // RR
                        break;
                }
            }

            if (pw[ch].envIndex == -1)
            {
                if (pw[ch].envelope[8] == 1)
                {
                    outPsgKeyOff((byte)ch);
                }
                else
                {
                    outRf5c164KeyOff((byte)ch);
                }
            }
        }


        private void commander(int ch, char cmd)
        {

            switch (cmd)
            {
                case ' ':
                case '\t':
                    pw[ch].incPos();
                    break;
                case '!': // CompileSkip
                    pw[ch].dataEnd = true;
                    pw[ch].waitCounter = -1;
                    break;
                case 'T': // tempo
                    cmdTempo(ch);
                    break;
                case '@': // instrument
                    cmdInstrument(ch);
                    break;
                case 'v': // volume
                    cmdVolume(ch);
                    break;
                case 'o': // octave
                    cmdOctave(ch);
                    break;
                case '>': // octave Up
                    cmdOctaveUp(ch);
                    break;
                case '<': // octave Down
                    cmdOctaveDown(ch);
                    break;
                case ')': // volume Up
                    cmdVolumeUp(ch);
                    break;
                case '(': // volume Down
                    cmdVolumeDown(ch);
                    break;
                case 'l': // length
                    cmdLength(ch);
                    break;
                case '#': // length(clock)
                    cmdClockLength(ch);
                    break;
                case 'p': // pan
                    cmdPan(ch);
                    break;
                case 'D': // Detune
                    cmdDetune(ch);
                    break;
                case 'm': // pcm mode
                    cmdMode(ch);
                    break;
                case 'q': // gatetime
                    cmdGatetime(ch);
                    break;
                case 'Q': // gatetime
                    cmdGatetime2(ch);
                    break;
                case 'E': // envelope
                    cmdEnvelope(ch);
                    break;
                case 'L': // loop point
                    cmdLoop(ch);
                    break;
                case '[': // repeat
                    cmdRepeatStart(ch);
                    break;
                case ']': // repeat
                    cmdRepeatEnd(ch);
                    break;
                case '/': // repeat
                    cmdRepeatExit(ch);
                    break;
                case 'M': // lfo
                    cmdLfo(ch);
                    break;
                case 'S': // lfo switch
                    cmdLfoSwitch(ch);
                    break;
                case 'y': // y
                    cmdY(ch);
                    break;
                case 'w': // noise
                    cmdNoise(ch);
                    break;
                case 'K': // key shift
                    cmdKeyShift(ch);
                    break;
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                case 'g':
                case 'a':
                case 'b':
                case 'r':
                    cmdNote(ch, cmd);
                    break;
                default:
                    msgBox.setErrMsg(string.Format("未知のコマンド{0}を検出しました。", cmd), pw[ch].getLineNumber());
                    pw[ch].incPos();
                    break;
            }
        }

        private void cmdNote(int ch, char cmd)
        {
            pw[ch].incPos();

            //+ -の解析
            int shift = 0;
            while (pw[ch].getChar() == '+' || pw[ch].getChar() == '-')
            {
                shift += pw[ch].getChar() == '+' ? 1 : -1;
                pw[ch].incPos();
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
                if (!pw[ch].getNum(out n))
                {
                    if (!isSecond)
                        n = (int)pw[ch].length;
                    else if (!isMinus)
                    {
                        //タイとして'&'が使用されている
                        pw[ch].tie = true;
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

                if (!pw[ch].tie)
                {
                    m += n;

                    //符点の解析
                    while (pw[ch].getChar() == '.')
                    {
                        if (n % 2 != 0)
                        {
                            msgBox.setWrnMsg("割り切れない.の指定があります。音長は不定です。", lineNumber);
                        }
                        n = n / 2;
                        m += n;
                        pw[ch].incPos();
                    }


                    if (isMinus) ml -= m;
                    else ml += m;
                }

                //ベンドの解析
                int bendDelayCounter = 0;
                int bendShift = 0;
                if (pw[ch].getChar() == '_')
                {
                    pw[ch].incPos();
                    pw[ch].bendOctave = pw[ch].octave;
                    pw[ch].bendNote = 'r';
                    pw[ch].bendWaitCounter = -1;
                    bool loop = true;
                    while (loop)
                    {
                        char bCmd = pw[ch].getChar();
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
                                pw[ch].incPos();
                                //+ -の解析
                                bendShift = 0;
                                while (pw[ch].getChar() == '+' || pw[ch].getChar() == '-')
                                {
                                    bendShift += pw[ch].getChar() == '+' ? 1 : -1;
                                    pw[ch].incPos();
                                }
                                pw[ch].bendShift = bendShift;
                                bendDelayCounter = 0;
                                n = -1;
                                isMinus = false;
                                isSecond = false;
                                do
                                {
                                    m = 0;

                                    //数値の解析
                                    if (!pw[ch].getNum(out n))
                                    {
                                        if (!isSecond)
                                        {
                                            n = 0;
                                            break;
                                        }
                                        else if (!isMinus)
                                        {
                                            //タイとして'&'が使用されている
                                            pw[ch].tie = true;
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

                                    if (!pw[ch].tie)
                                    {
                                        bendDelayCounter += n;

                                        //符点の解析
                                        while (pw[ch].getChar() == '.')
                                        {
                                            if (n % 2 != 0)
                                            {
                                                msgBox.setWrnMsg("割り切れない.の指定があります。音長は不定です。", lineNumber);
                                            }
                                            n = n / 2;
                                            m += n;
                                            pw[ch].incPos();
                                        }


                                        if (isMinus) bendDelayCounter -= m;
                                        else bendDelayCounter += m;
                                    }

                                    if (pw[ch].getChar() == '&')
                                    {
                                        isMinus = false;
                                    }
                                    else if (pw[ch].getChar() == '~')
                                    {
                                        isMinus = true;
                                    }
                                    else
                                    {
                                        break;
                                    }

                                    isSecond = true;
                                    pw[ch].incPos();

                                } while (true);

                                if (cmd != 'r')
                                {
                                    pw[ch].bendNote = bCmd;
                                    bendDelayCounter = checkRange(bendDelayCounter, 0, ml);
                                    pw[ch].bendWaitCounter = bendDelayCounter;
                                }
                                else
                                {
                                    msgBox.setErrMsg("休符にベンドの指定はできません。", lineNumber);
                                }

                                break;
                            case 'o':
                                pw[ch].incPos();
                                if (!pw[ch].getNum(out n))
                                {
                                    msgBox.setErrMsg("不正なオクターブが指定されています。", lineNumber);
                                    n = 110;
                                }
                                n = checkRange(n, 1, 8);
                                pw[ch].bendOctave = n;
                                break;
                            case '>':
                                pw[ch].incPos();
                                pw[ch].bendOctave++;
                                pw[ch].bendOctave = checkRange(pw[ch].bendOctave, 1, 8);
                                break;
                            case '<':
                                pw[ch].incPos();
                                pw[ch].bendOctave--;
                                pw[ch].bendOctave = checkRange(pw[ch].bendOctave, 1, 8);
                                break;
                            default:
                                loop = false;
                                break;
                        }
                    }

                    //音符の変化量
                    int ed = note.IndexOf(pw[ch].bendNote) + 1 + (pw[ch].bendOctave - 1) * 12 + pw[ch].bendShift;
                    ed = checkRange(ed, 0, 8 * 12 - 1);
                    int st = note.IndexOf(cmd) + 1 + (pw[ch].octave - 1) * 12 + shift;
                    st = checkRange(st, 0, 8 * 12 - 1);

                    int delta = ed - st;
                    if (delta == 0 || bendDelayCounter == ml)
                    {
                        pw[ch].bendNote = 'r';
                        pw[ch].bendWaitCounter = -1;
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
                            int a = getPsgFNum(pw[ch].octave, cmd, shift + (i + 0) * Math.Sign(delta));
                            int b = getPsgFNum(pw[ch].octave, cmd, shift + (i + 1) * Math.Sign(delta));
                            if (ch < 9)
                            {
                                a = getFmFNum(pw[ch].octave, cmd, shift + (i + 0) * Math.Sign(delta));
                                b = getFmFNum(pw[ch].octave, cmd, shift + (i + 1) * Math.Sign(delta));
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
                            else if (ch < 13)
                            {
                                a = getPsgFNum(pw[ch].octave, cmd, shift + (i + 0) * Math.Sign(delta));
                                b = getPsgFNum(pw[ch].octave, cmd, shift + (i + 1) * Math.Sign(delta));
                            }
                            else
                            {
                                a = getRf5c164PcmNote(pw[ch].octave, cmd, shift + (i + 0) * Math.Sign(delta));
                                b = getRf5c164PcmNote(pw[ch].octave, cmd, shift + (i + 1) * Math.Sign(delta));
                            }
                            //System.Console.Write("[{0:x} <= n < {1:x}]", a, b);
                            //System.Console.Write("[{0}:{1}]", tl, bf);
                            if (Math.Abs(bf) >= 1.0f)
                            {
                                for (int j = 0; j < (int)Math.Abs(bf); j++)
                                {
                                    int c = b - a;
                                    int d = (int)Math.Abs(bf);
                                    //System.Console.Write(":{0:x}", (int)(a + ((float)c / (float)d) * (float)j));
                                    lstBend.Add((int)(a + ((float)c / (float)d) * (float)j));
                                }
                                bf -= (int)bf;
                            }
                            //System.Console.WriteLine("");
                        }
                        //System.Console.Write("{0}:{1}", ml - bendDelayCounter, lstBend.Count);
                        //System.Console.WriteLine("");
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
                            //System.Console.Write("[{0:x}:{1}]", f, cnt);
                            of = f;
                            cnt = 1;
                        }
                        pw[ch].bendList = new Stack<Tuple<int, int>>();
                        foreach (Tuple<int, int> lbt in lb)
                        {
                            pw[ch].bendList.Push(lbt);
                        }
                        Tuple<int, int> t = pw[ch].bendList.Pop();
                        pw[ch].bendFnum = t.Item1;
                        pw[ch].bendWaitCounter = t.Item2;
                    }
                }

                if (pw[ch].getChar() == '&')
                {
                    isMinus = false;
                }
                else if (pw[ch].getChar() == '~')
                {
                    isMinus = true;
                }
                else
                {
                    break;
                }

                isSecond = true;
                pw[ch].incPos();

            } while (true);

            if (ml < 1)
            {
                msgBox.setErrMsg("負の音長が指定されました。", lineNumber);
                ml = (int)pw[ch].length;
            }


            //装飾の解析完了


            //WaitClockの決定
            pw[ch].waitCounter = ml;

            if (cmd != 'r')
            {

                //発音周波数
                if (pw[ch].bendWaitCounter == -1)
                {
                    pw[ch].noteCmd = cmd;
                    pw[ch].shift = shift;
                }
                else
                {
                    pw[ch].octave = pw[ch].bendOctave;
                    pw[ch].noteCmd = pw[ch].bendNote;
                    pw[ch].shift = pw[ch].bendShift;
                }

                //発音周波数の決定とキーオン
                if (ch < 9)
                {

                    //YM2612

                    if (!pw[ch].pcm)
                    {
                        setFmFNum(ch);
                    }
                    else
                    {
                        getPcmNote(pw[ch]);
                    }
                    //タイ指定では無い場合はキーオンする
                    if (!pw[ch].beforeTie)
                    {
                        setLfoAtKeyOn(ch);
                        setFmVolume(ch);
                        outFmKeyOn((byte)ch);
                    }
                }
                else if (ch < 13)
                {

                    // SN76489

                    setPsgFNum(ch);

                    //タイ指定では無い場合はキーオンする
                    if (!pw[ch].beforeTie)
                    {
                        setEnvelopeAtKeyOn(ch);
                        setLfoAtKeyOn(ch);
                        outPsgKeyOn((byte)ch);
                    }
                }
                else
                {

                    // RF5C164

                    setRf5c164FNum(ch);

                    //setRf5c164Volume(ch);
                    //タイ指定では無い場合はキーオンする
                    if (!pw[ch].beforeTie)
                    {
                        setEnvelopeAtKeyOn(ch);
                        setLfoAtKeyOn(ch);
                        outRf5c164KeyOn((byte)ch);
                    }
                }

                //gateTimeの決定
                if (pw[ch].gatetimePmode)
                {
                    pw[ch].waitKeyOnCounter = pw[ch].waitCounter * pw[ch].gatetime / 8L;
                }
                else
                {
                    pw[ch].waitKeyOnCounter = pw[ch].waitCounter - pw[ch].gatetime;
                }
                if (pw[ch].waitKeyOnCounter < 1) pw[ch].waitKeyOnCounter = 1;

                //PCM専用のWaitClockの決定
                if (pw[ch].pcm)
                {
                    waitKeyOnPcmCounter = pw[ch].waitKeyOnCounter;
                    pcmSizeCounter = instPCM[pw[ch].instrument].size;
                }
            }

            pw[ch].clockCounter += pw[ch].waitCounter;
        }

        private void cmdRepeatExit(int ch)
        {
            int n = -1;
            pw[ch].incPos();
            clsRepeat rx = pw[ch].stackRepeat.Pop();
            if (rx.repeatCount == 1)
            {
                int i = 0;
                while (true)
                {
                    char c = pw[ch].getChar();
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
                    pw[ch].incPos();
                }
                pw[ch].incPos();
                pw[ch].getNum(out n);
            }
            else
            {
                pw[ch].stackRepeat.Push(rx);
            }

        }

        private void cmdRepeatEnd(int ch)
        {
            int n;
            pw[ch].incPos();
            if (!pw[ch].getNum(out n))
            {
                n = 2;
            }
            n = checkRange(n, 1, 255);
            try
            {
                clsRepeat re = pw[ch].stackRepeat.Pop();
                if (re.repeatCount == -1)
                {
                    //初回
                    re.repeatCount = n;
                }
                re.repeatCount--;
                if (re.repeatCount > 0)
                {
                    pw[ch].stackRepeat.Push(re);
                    pw[ch].setPos(re.pos);
                }
            }
            catch
            {
                msgBox.setWrnMsg("[と]の数があいません。", lineNumber);
            }
        }

        private void cmdRepeatStart(int ch)
        {
            pw[ch].incPos();
            clsRepeat rs = new clsRepeat();
            rs.pos = pw[ch].getPos();
            rs.repeatCount = -1;//初期値
            pw[ch].stackRepeat.Push(rs);
        }

        private void cmdLoop(int ch)
        {
            pw[ch].incPos();
            loopOffset = (long)dat.Count;
            loopSamples = lSample;
        }

        private void cmdEnvelope(int ch)
        {
            int n = -1;
            if (ch == 2 || (ch >= 6 && ch <= 8) || (ch >= 9 && ch <= 12) || (ch>=13 && ch<=20))
            {
                pw[ch].incPos();
                switch (pw[ch].getChar())
                {
                    case 'O':
                        pw[ch].incPos();
                        if (ch < 9)
                        {
                            switch (pw[ch].getChar())
                            {
                                case 'N':
                                    pw[ch].incPos();
                                    pw[ch].Ch3SpecialMode = true;
                                    outFmSetCh3SpecialMode(true);
                                    break;
                                case 'F':
                                    pw[ch].incPos();
                                    pw[ch].Ch3SpecialMode = false;
                                    outFmSetCh3SpecialMode(false);
                                    break;
                                default:
                                    msgBox.setErrMsg(string.Format("未知のコマンド(EO{0})が指定されました。", pw[ch].getChar()), lineNumber);
                                    pw[ch].incPos();
                                    break;
                            }
                        }
                        else
                        {
                            switch (pw[ch].getChar())
                            {
                                case 'N':
                                    pw[ch].incPos();
                                    pw[ch].envelopeMode = true;
                                    break;
                                case 'F':
                                    pw[ch].incPos();
                                    pw[ch].envelopeMode = false;
                                    break;
                                default:
                                    msgBox.setErrMsg(string.Format("未知のコマンド(EO{0})が指定されました。", pw[ch].getChar()), lineNumber);
                                    pw[ch].incPos();
                                    break;
                            }
                        }
                        break;
                    case 'X':
                        char c = pw[ch].getChar();
                        if (ch < 9)
                        {
                            pw[ch].incPos();
                            if (!pw[ch].getNum(out n))
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
                                pw[ch].slots = res;
                            }
                        }
                        else
                        {
                            msgBox.setErrMsg("未知のコマンド(EX)が指定されました。", lineNumber);
                        }
                        break;
                    default:
                        if (ch == 2)
                        {
                            int[] s = new int[] { 0, 0, 0, 0 };

                            for (int i = 0; i < 4; i++)
                            {
                                if (pw[ch].getNum(out n))
                                {
                                    s[i] = n;
                                }
                                else
                                {
                                    msgBox.setErrMsg("Eコマンドの解析に失敗しました。", lineNumber);
                                    break;
                                }
                                if (i == 3) break;
                                pw[ch].incPos();
                            }
                            pw[ch].slotDetune = s;
                            break;
                        }
                        else
                        {
                            msgBox.setErrMsg(string.Format("未知のコマンド(E{0})が指定されました。", pw[ch].getChar()), lineNumber);
                            pw[ch].incPos();
                        }
                        break;
                }
            }
            else
            {
                msgBox.setWrnMsg("このパートは効果音モードに対応したチャンネルが指定されていないため、Eコマンドは無視されます。", lineNumber);
                pw[ch].incPos();
            }

        }

        private void cmdGatetime2(int ch)
        {
            int n;
            pw[ch].incPos();
            if (!pw[ch].getNum(out n))
            {
                msgBox.setErrMsg("不正なゲートタイム指定'Q'が指定されています。", lineNumber);
                n = 1;
            }
            n = checkRange(n, 1, 8);
            pw[ch].gatetime = n;
            pw[ch].gatetimePmode = true;
        }

        private void cmdGatetime(int ch)
        {
            int n;
            pw[ch].incPos();
            if (!pw[ch].getNum(out n))
            {
                msgBox.setErrMsg("不正なゲートタイム指定'q'が指定されています。", lineNumber);
                n = 0;
            }
            n = checkRange(n, 0, 255);
            pw[ch].gatetime = n;
            pw[ch].gatetimePmode = false;
        }

        private void cmdMode(int ch)
        {
            int n;
            pw[ch].incPos();
            if (ch == 5)
            {
                if (!pw[ch].getNum(out n))
                {
                    msgBox.setErrMsg("不正なPCMモード指定'm'が指定されています。", lineNumber);
                    n = 0;
                }
                n = checkRange(n, 0, 1);
                pw[ch].pcm = (n == 1);
                outFmSetCh6PCMMode(pw[ch].pcm);
            }
            else
            {
                pw[ch].getNum(out n);
                msgBox.setWrnMsg("このパートは6chではないため、mコマンドは無視されます。", lineNumber);
            }

        }

        private void cmdDetune(int ch)
        {
            int n;
            pw[ch].incPos();
            if (!pw[ch].getNum(out n))
            {
                msgBox.setErrMsg("不正なディチューン'D'が指定されています。", lineNumber);
                n = 0;
            }
            n = checkRange(n, -127, 127);
            pw[ch].detune = n;
        }

        private void cmdPan(int ch)
        {
            int n;
            int vch = ch;

            //効果音モードのチャンネル番号を指定している場合は3chへ変更する
            if (ch == 6 || ch == 7 || ch == 8)
            {
                vch = 2;
            }

            pw[ch].incPos();
            if (ch < 9)
            {
                if (!pw[ch].getNum(out n))
                {
                    msgBox.setErrMsg("不正なパン'p'が指定されています。", lineNumber);
                    n = 10;
                }
                //強制的にモノラルにする
                if (monoPart != null && monoPart.Contains(fCh[ch]))
                {
                    n = 3;
                }
                n = checkRange(n, 1, 3);
                pw[ch].pan = n;
                outFmSetPanAMSFMS((byte)vch, pw[ch].pan, 0, 0);
            }
            else if (ch < 13)
            {
                pw[ch].getNum(out n);
                msgBox.setWrnMsg("PSGパートでは、pコマンドは無視されます。", lineNumber);
            }
            else
            {
                int l;
                int r;
                if (!pw[ch].getNum(out l))
                {
                    msgBox.setErrMsg("不正なパン'p'が指定されています。", lineNumber);
                    l = 15;
                }
                if (pw[ch].getChar() != ',')
                {
                    msgBox.setErrMsg("不正なパン'p'が指定されています。", lineNumber);
                    l = 15;
                    r = 15;
                }
                pw[ch].incPos();
                if (!pw[ch].getNum(out r))
                {
                    msgBox.setErrMsg("不正なパン'p'が指定されています。", lineNumber);
                    r = 15;
                }
                l = checkRange(l, 0, 15);
                r = checkRange(r, 0, 15);
                pw[ch].pan = (r << 4) | l;
                setRf5c164CurrentChannel((byte)ch);
                setRf5c164Pan((byte)ch, pw[ch].pan);
            }

        }

        private void cmdLength(int ch)
        {
            int n;
            pw[ch].incPos();
            if (!pw[ch].getNum(out n))
            {
                msgBox.setErrMsg("不正な音長が指定されています。", lineNumber);
                n = 10;
            }
            n = checkRange(n, 1, 128);
            pw[ch].length = clockCount / n;
        }

        private void cmdClockLength(int ch)
        {
            int n;
            pw[ch].incPos();
            if (!pw[ch].getNum(out n))
            {
                msgBox.setErrMsg("不正な音長が指定されています。", lineNumber);
                n = 10;
            }
            n = checkRange(n, 1, 65535);
            pw[ch].length = n;
        }

        private void cmdVolumeDown(int ch)
        {
            int n;
            pw[ch].incPos();
            if (!pw[ch].getNum(out n))
            {
                msgBox.setErrMsg("不正な音量'('が指定されています。", lineNumber);
                n = 10;
            }
            n = checkRange(n, 1, (ch < 9) ? fmMaxVolume : ((ch < 13) ? psgMaxVolume : rf5c164MaxVolume));
            pw[ch].volume -= n;
            if (ch < 9)
            {
                pw[ch].volume = checkRange(pw[ch].volume, 0, fmMaxVolume);
            }
            else if (ch < 13)
            {
                pw[ch].volume = checkRange(pw[ch].volume, 0, psgMaxVolume);
            }
            else
            {
                pw[ch].volume = checkRange(pw[ch].volume, 0, rf5c164MaxVolume);
            }

        }

        private void cmdVolumeUp(int ch)
        {
            int n;
            pw[ch].incPos();
            if (!pw[ch].getNum(out n))
            {
                msgBox.setErrMsg("不正な音量')'が指定されています。", lineNumber);
                n = 10;
            }
            n = checkRange(n, 1, (ch < 9) ? fmMaxVolume : ((ch < 13) ? psgMaxVolume : rf5c164MaxVolume));
            pw[ch].volume += n;
            if (ch < 9)
            {
                pw[ch].volume = checkRange(pw[ch].volume, 0, fmMaxVolume);
            }
            else if (ch < 13)
            {
                pw[ch].volume = checkRange(pw[ch].volume, 0, psgMaxVolume);
            }
            else
            {
                pw[ch].volume = checkRange(pw[ch].volume, 0, rf5c164MaxVolume);
            }

        }

        private void cmdOctaveDown(int ch)
        {
            pw[ch].incPos();
            pw[ch].octave--;
            pw[ch].octave = checkRange(pw[ch].octave, 1, 8);
        }

        private void cmdOctaveUp(int ch)
        {
            pw[ch].incPos();
            pw[ch].octave++;
            pw[ch].octave = checkRange(pw[ch].octave, 1, 8);
        }

        private void cmdOctave(int ch)
        {
            int n;
            pw[ch].incPos();
            if (!pw[ch].getNum(out n))
            {
                msgBox.setErrMsg("不正なオクターブが指定されています。", lineNumber);
                n = 110;
            }
            n = checkRange(n, 1, 8);
            pw[ch].octave = n;
        }

        private void cmdTempo(int ch)
        {
            int n;
            pw[ch].incPos();
            if (!pw[ch].getNum(out n))
            {
                msgBox.setErrMsg("不正なテンポが指定されています。", lineNumber);
                n = 120;
            }
            n = checkRange(n, 1, 255);
            tempo = n;
            samplesPerClock = vgmSamplesPerSecond * 60 * 4 / (tempo * clockCount);
        }

        private void cmdVolume(int ch)
        {
            int n;
            pw[ch].incPos();
            if (!pw[ch].getNum(out n))
            {
                msgBox.setErrMsg("不正な音量が指定されています。", lineNumber);
                n = 110;
            }
            if (ch < 9)
            {
                n = checkRange(n, 0, fmMaxVolume);
                if (pw[ch].volume != n)
                {
                    pw[ch].volume = n;
                }
            }
            else if (ch < 13)
            {
                n = checkRange(n, 0, psgMaxVolume);
                pw[ch].volume = n;
            }
            else
            {
                n = checkRange(n, 0, rf5c164MaxVolume);
                pw[ch].volume = n;
            }
        }

        private void cmdInstrument(int ch)
        {
            int n;
            pw[ch].incPos();
            if (ch < 9)
            {
                if (!pw[ch].getNum(out n))
                {
                    msgBox.setErrMsg("不正な音色番号が指定されています。", lineNumber);
                    n = 0;
                }
                n = checkRange(n, 0, 255);
                if (pw[ch].instrument != n)
                {
                    pw[ch].instrument = n;
                    if (ch == 2 || ch == 6 || ch == 7 || ch == 8)
                    {
                        pw[2].instrument = n;
                        pw[6].instrument = n;
                        pw[7].instrument = n;
                        pw[8].instrument = n;
                    }
                    if (!pw[ch].pcm)
                    {
                        outFmSetInstrument((byte)ch, n, pw[ch].volume);
                    }
                    else
                    {
                        if (!instPCM.ContainsKey(n))
                        {
                            msgBox.setErrMsg(string.Format("PCM定義に指定された音色番号({0})が存在しません。", n), lineNumber);
                        }
                        else
                        {
                            if (instPCM[n].chip != 0)
                            {
                                msgBox.setErrMsg(string.Format("指定された音色番号({0})はYM2612向けPCMデータではありません。", n), lineNumber);
                            }
                        }
                    }
                }
            }
            else if (ch < 13)
            {
                if (!pw[ch].getNum(out n))
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
                    if (pw[ch].envInstrument != n)
                    {
                        pw[ch].envInstrument = n;
                        pw[ch].envIndex = -1;
                        pw[ch].envCounter = -1;
                        for (int i = 0; i < instENV[n].Length; i++)
                        {
                            pw[ch].envelope[i] = instENV[n][i];
                        }
                    }
                }
            }
            else
            {
                if (pw[ch].getChar() != 'E')
                {
                    if (!pw[ch].getNum(out n))
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
                        if (instPCM[n].chip != 2)
                        {
                            msgBox.setErrMsg(string.Format("指定された音色番号({0})はRF5C164向けPCMデータではありません。", n), lineNumber);
                        }
                        setRf5c164CurrentChannel((byte)ch);
                        setRf5c164SampleStartAddress((byte)ch, (int)instPCM[n].stAdr);
                        setRf5c164LoopAddress((byte)ch, (int)(instPCM[n].loopAdr));
                    }
                }
                else
                {
                    pw[ch].incPos();
                    if (!pw[ch].getNum(out n))
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
                        if (pw[ch].envInstrument != n)
                        {
                            pw[ch].envInstrument = n;
                            pw[ch].envIndex = -1;
                            pw[ch].envCounter = -1;
                            for (int i = 0; i < instENV[n].Length; i++)
                            {
                                pw[ch].envelope[i] = instENV[n][i];
                            }
                        }
                    }
                }
            }
        }

        private void cmdLfo(int ch)
        {

            pw[ch].incPos();
            char c = pw[ch].getChar();
            if (c < 'P' && c > 'S')
            {
                msgBox.setErrMsg("指定できるLFOのチャネルはP,Q,R,Sの4種類です。", lineNumber);
                return;
            }
            c -= 'P';

            pw[ch].incPos();
            char t = pw[ch].getChar();
            if (t != 'T' && t != 'V' && t != 'H')
            {
                msgBox.setErrMsg("指定できるLFOの種類はT,V,Hの3種類です。", lineNumber);
                return;
            }
            pw[ch].lfo[c].type = (t == 'T') ? eLfoType.Tremolo : ((t == 'V') ? eLfoType.Vibrato : eLfoType.Hardware);

            pw[ch].lfo[c].sw = false;
            pw[ch].lfo[c].isEnd = true;

            pw[ch].lfo[c].param = new List<int>();
            int n = -1;
            do
            {
                pw[ch].incPos();
                if (pw[ch].getNum(out n))
                {
                    pw[ch].lfo[c].param.Add(n);
                }
                else
                {
                    msgBox.setErrMsg("LFOの設定値に不正な値が指定されました。", lineNumber);
                    return;
                }

                while(pw[ch].getChar()=='\t' || pw[ch].getChar()==' ') { pw[ch].incPos(); }

            } while (pw[ch].getChar() == ',');
            if (pw[ch].lfo[c].type == eLfoType.Tremolo || pw[ch].lfo[c].type == eLfoType.Vibrato)
            {
                if (pw[ch].lfo[c].param.Count < 4)
                {
                    msgBox.setErrMsg("LFOの設定に必要なパラメータが足りません。", lineNumber);
                    return;
                }
                if (pw[ch].lfo[c].param.Count > 7)
                {
                    msgBox.setErrMsg("LFOの設定に可能なパラメータ数を超えて指定されました。", lineNumber);
                    return;
                }

                pw[ch].lfo[c].param[0] = checkRange(pw[ch].lfo[c].param[0], 0, (int)clockCount);
                pw[ch].lfo[c].param[1] = checkRange(pw[ch].lfo[c].param[1], 1, 255);
                pw[ch].lfo[c].param[2] = checkRange(pw[ch].lfo[c].param[2], -32768, 32787);
                pw[ch].lfo[c].param[3] = Math.Abs(checkRange(pw[ch].lfo[c].param[3], -32768, 32787));
                if (pw[ch].lfo[c].param.Count > 4)
                {
                    pw[ch].lfo[c].param[4] = checkRange(pw[ch].lfo[c].param[4], 0, 4);
                }
                else
                {
                    pw[ch].lfo[c].param.Add(0);
                }
                if (pw[ch].lfo[c].param.Count > 5)
                {
                    pw[ch].lfo[c].param[5] = checkRange(pw[ch].lfo[c].param[5], 0, 1);
                }
                else
                {
                    pw[ch].lfo[c].param.Add(1);
                }
                if (pw[ch].lfo[c].param.Count > 6)
                {
                    pw[ch].lfo[c].param[6] = checkRange(pw[ch].lfo[c].param[6], -32768, 32787);
                }
                else
                {
                    pw[ch].lfo[c].param.Add(0);
                }

            }
            else
            {
                if (pw[ch].lfo[c].param.Count < 4)
                {
                    msgBox.setErrMsg("LFOの設定に必要なパラメータが足りません。", lineNumber);
                    return;
                }
                if (pw[ch].lfo[c].param.Count > 5)
                {
                    msgBox.setErrMsg("LFOの設定に可能なパラメータ数を超えて指定されました。", lineNumber);
                    return;
                }

                pw[ch].lfo[c].param[0] = checkRange(pw[ch].lfo[c].param[0], 0, (int)clockCount);
                pw[ch].lfo[c].param[1] = checkRange(pw[ch].lfo[c].param[1], 0, 7);
                pw[ch].lfo[c].param[2] = checkRange(pw[ch].lfo[c].param[2], 0, 7);
                pw[ch].lfo[c].param[3] = checkRange(pw[ch].lfo[c].param[3], 0, 3);
                if (pw[ch].lfo[c].param.Count == 5)
                {
                    pw[ch].lfo[c].param[4] = checkRange(pw[ch].lfo[c].param[4], 0, 1);
                }
                else
                {
                    pw[ch].lfo[c].param.Add(1);
                }

            }
            //解析　ここまで

            pw[ch].lfo[c].sw = true;
            pw[ch].lfo[c].isEnd = false;
            pw[ch].lfo[c].value = (pw[ch].lfo[c].param[0] == 0) ? pw[ch].lfo[c].param[6] : 0;//ディレイ中は振幅補正は適用されない
            pw[ch].lfo[c].waitCounter = pw[ch].lfo[c].param[0];
            pw[ch].lfo[c].direction = pw[ch].lfo[c].param[2] < 0 ? -1 : 1;
        }

        private void cmdLfoSwitch(int ch)
        {

            pw[ch].incPos();
            char c = pw[ch].getChar();
            if (c < 'P' && c > 'S')
            {
                msgBox.setErrMsg("指定できるLFOのチャネルはP,Q,R,Sの4種類です。", lineNumber);
                return;
            }
            c -= 'P';

            int n = -1;
            pw[ch].incPos();
            if (!pw[ch].getNum(out n))
            {
                msgBox.setErrMsg("LFOの設定値に不正な値が指定されました。", lineNumber);
                return;
            }
            n = checkRange(n, 0, 2);

            //解析　ここまで

            pw[ch].lfo[c].sw = (n == 0) ? false : true;
            if (pw[ch].lfo[c].type == eLfoType.Hardware && pw[ch].lfo[c].param != null)
            {
                if (ch < 9)
                {
                    if (pw[ch].lfo[c].param[4] == 0)
                    {
                        outFmSetHardLfo((n == 0) ? false : true, pw[ch].lfo[c].param[1]);
                    }
                    else
                    {
                        outFmSetHardLfo(false, pw[ch].lfo[c].param[1]);
                    }
                }
            }

        }

        private void cmdY(int ch)
        {
            int n = -1;
            byte adr = 0;
            byte dat = 0;
            pw[ch].incPos();
            if (pw[ch].getNum(out n))
            {
                adr = (byte)(n & 0xff);
            }
            pw[ch].incPos();
            if (pw[ch].getNum(out n))
            {
                dat = (byte)(n & 0xff);
            }

            if (ch < 3)
            {
                outFmAdrPort(false, adr, dat);
            }
            else if (ch < 6)
            {
                outFmAdrPort(true, adr, dat);
            }
            else if (ch < 9)
            {
                outFmAdrPort(false, adr, dat);
            }
            else if (ch < 13)
            {
                outPsgPort(dat);
            }
            else
            {
                outRf5c164Port(adr, dat);
            }
        }

        private void cmdNoise(int ch)
        {
            int n = -1;
            pw[ch].incPos();
            if (ch != 12)
            {
                msgBox.setErrMsg("このチャンネルではwコマンドは使用できません。", lineNumber);
                return;
            }

            if (pw[ch].getNum(out n))
            {
                pw[ch].noise = checkRange(n, 0, 7);
            }
            else
            {
                msgBox.setErrMsg("wコマンドに指定された値が不正です。", lineNumber);
                return;

            }
        }

        private void cmdKeyShift(int ch)
        {
            int n = -1;
            pw[ch].incPos();

            if (pw[ch].getNum(out n))
            {
                pw[ch].keyShift = checkRange(n,-128,128);
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
            if (pcmDataYM2612 != null && pcmDataYM2612.Length > 0)
            {
                foreach (byte b in pcmDataYM2612)
                {
                    dat.Add(b);
                }
            }

            //PCM Data block
            if (pcmDataRf5c164 != null && pcmDataRf5c164.Length > 0)
            {
                foreach (byte b in pcmDataRf5c164)
                {
                    dat.Add(b);
                }
            }

            dat.Add(0x52); dat.Add(0x2b); dat.Add(0x80);

            outFmSetHardLfo(false, 0);
            outFmSetCh3SpecialMode(false);
            outFmSetCh6PCMMode(false);
            outFmAllKeyOff();

            outFmSetPanAMSFMS(0, 3, 0, 0);
            outFmSetPanAMSFMS(1, 3, 0, 0);
            outFmSetPanAMSFMS(2, 3, 0, 0);
            outFmSetPanAMSFMS(3, 3, 0, 0);
            outFmSetPanAMSFMS(4, 3, 0, 0);
            outFmSetPanAMSFMS(5, 3, 0, 0);

            for (int i = 0; i < 8; i++)
            {
                setRf5c164CurrentChannel((byte)(13 + i));
                setRf5c164SampleStartAddress((byte)(13 + i), 0);
                setRf5c164LoopAddress((byte)(13 + i), 0);
                setRf5c164AddressIncrement((byte)(13 + i), 0x400);
                setRf5c164Pan((byte)(13 + i), 0xff);
                setRf5c164Envelope((byte)(13 + i), 0xff);
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
            for (int i = 0; i < pw.Length; i++)
            {
                switch (pw[i].type)
                {
                    case ePartType.YM2612:
                    case ePartType.YM2612extend:
                        useYM2612 += pw[i].clockCounter;
                        break;
                    case ePartType.SegaPSG:
                        useSN76489 += pw[i].clockCounter;
                        break;
                    case ePartType.Rf5c164:
                        useRf5c164 += pw[i].clockCounter;
                        break;
                }
            }

            if (useYM2612 == 0) { dat[0x2c] = 0; dat[0x2d] = 0; dat[0x2e] = 0; dat[0x2f] = 0; }
            if (useSN76489 == 0) { dat[0x0c] = 0; dat[0x0d] = 0; dat[0x0e] = 0; dat[0x0f] = 0; }
            if (useRf5c164 == 0) { dat[0x6c] = 0; dat[0x6d] = 0; dat[0x6e] = 0; dat[0x6f] = 0; }

        }

        private void setEnvelopeAtKeyOn(int ch)
        {
            if (!pw[ch].envelopeMode)
            {
                pw[ch].envVolume = 0;
                pw[ch].envIndex = -1;
                return;
            }

            pw[ch].envIndex = 0;
            pw[ch].envCounter = 0;
            int maxValue = (pw[ch].envelope[8] == 2) ? 255 : 15;

            while (pw[ch].envCounter == 0 && pw[ch].envIndex != -1)
            {
                switch (pw[ch].envIndex)
                {
                    case 0: // AR phase
                        pw[ch].envCounter = pw[ch].envelope[2];
                        if (pw[ch].envelope[2] > 0 && pw[ch].envelope[1] < maxValue)
                        {
                            pw[ch].envVolume = pw[ch].envelope[1];
                        }
                        else
                        {
                            pw[ch].envVolume = maxValue;
                            pw[ch].envIndex++;
                        }
                        break;
                    case 1: // DR phase
                        pw[ch].envCounter = pw[ch].envelope[3];
                        if (pw[ch].envelope[3] > 0 && pw[ch].envelope[4] < maxValue)
                        {
                            pw[ch].envVolume = maxValue;
                        }
                        else
                        {
                            pw[ch].envVolume = pw[ch].envelope[4];
                            pw[ch].envIndex++;
                        }
                        break;
                    case 2: // SR phase
                        pw[ch].envCounter = pw[ch].envelope[5];
                        if (pw[ch].envelope[5] > 0 && pw[ch].envelope[4] != 0)
                        {
                            pw[ch].envVolume = pw[ch].envelope[4];
                        }
                        else
                        {
                            pw[ch].envVolume = 0;
                            pw[ch].envIndex = -1;
                        }
                        break;
                }
            }
        }

        private void setLfoAtKeyOn(int ch)
        {
            for(int lfo = 0; lfo < 4; lfo++)
            {
                clsLfo pl = pw[ch].lfo[lfo];
                if (!pl.sw)
                {
                    continue;
                }
                if (pl.type == eLfoType.Hardware)
                {
                    if (ch < 9)
                    {
                        if (pl.param[4] == 1)
                        {
                            outFmSetHardLfo(false, pl.param[1]);
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
                    if (ch < 9)
                    {
                        setFmFNum(ch);
                    }
                }
                if (pl.type == eLfoType.Tremolo)
                {
                    if (ch < 9)
                    {
                        pw[ch].beforeVolume = -1;
                        setFmVolume(ch);
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


        private void setFmFNum(int ch)
        {
            int f = getFmFNum(pw[ch].octave, pw[ch].noteCmd, pw[ch].shift + pw[ch].keyShift);
            if (pw[ch].bendWaitCounter !=-1)
            {
                f = pw[ch].bendFnum;
            }
            int o = (f & 0xf000) / 0x1000;
            f &= 0xfff;

            f = f + pw[ch].detune;
            for (int lfo = 0; lfo < 4; lfo++)
            {
                if (!pw[ch].lfo[lfo].sw)
                {
                    continue;
                }
                if (pw[ch].lfo[lfo].type != eLfoType.Vibrato)
                {
                    continue;
                }
                f += pw[ch].lfo[lfo].value + pw[ch].lfo[lfo].param[6];
            }
            while (f < fmFNumTbl[0])
            {
                if (o == 0)
                {
                    break;
                }
                o--;
                f = fmFNumTbl[0] * 2 - (fmFNumTbl[0] - f);
            }
            while (f >= fmFNumTbl[0] * 2)
            {
                if (o == 7)
                {
                    break;
                }
                o++;
                f = f - fmFNumTbl[0] * 2 + fmFNumTbl[0];
            }
            f = checkRange(f, 0, 0x7ff);

            outFmSetFnum((byte)ch, o, f);
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

        private int getRf5c164PcmNote(int octave,char noteCmd,int shift)
        {
            //int shift = pw.shift + pw.keyShift;
            //int o = pw.octave;
            //int n = note.IndexOf(pw.noteCmd) + shift;
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

            //pw.pcmOctave = o;
            //pw.pcmNote = n;

            //return (int)(0x0400 * pcmMTbl[pw.pcmNote] * Math.Pow(2, (pw.pcmOctave - 4)));
            return (int)(0x0400 * pcmMTbl[n] * Math.Pow(2, (o - 4)));
        }

        private void getPcmNote(partWork pw)
        {
            int shift = pw.shift + pw.keyShift;
            int o = pw.octave;
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

        private void setFmVolume(int ch)
        {
            int vol = pw[ch].volume;

            for (int lfo = 0; lfo < 4; lfo++)
            {
                if (!pw[ch].lfo[lfo].sw)
                {
                    continue;
                }
                if (pw[ch].lfo[lfo].type != eLfoType.Tremolo)
                {
                    continue;
                }
                vol += pw[ch].lfo[lfo].value + pw[ch].lfo[lfo].param[6];
            }

            if (pw[ch].beforeVolume != vol)
            {
                if (instFM.ContainsKey(pw[ch].instrument))
                {
                    outFmSetVolume((byte)ch, vol, pw[ch].instrument);
                    pw[ch].beforeVolume = vol;
                }
            }
        }

        private void setRf5c164Volume(int ch)
        {
            int vol = pw[ch].volume;

            if (pw[ch].envelopeMode)
            {
                vol = 0;
                if (pw[ch].envIndex != -1)
                {
                    vol = pw[ch].envVolume - (255 - pw[ch].volume);
                }
            }

            for (int lfo = 0; lfo < 4; lfo++)
            {
                if (!pw[ch].lfo[lfo].sw)
                {
                    continue;
                }
                if (pw[ch].lfo[lfo].type != eLfoType.Tremolo)
                {
                    continue;
                }
                vol += pw[ch].lfo[lfo].value + pw[ch].lfo[lfo].param[6];
            }

            vol = checkRange(vol, 0, 255);

            if (pw[ch].beforeVolume != vol)
            {
                    setRf5c164Envelope((byte)ch, vol);
                    pw[ch].beforeVolume = vol;
            }
        }


        private void setPsgFNum(int ch)
        {
            if (ch!=12)
            {
                int f = getPsgFNum(pw[ch].octave, pw[ch].noteCmd, pw[ch].shift + pw[ch].keyShift);
                if (pw[ch].bendWaitCounter != -1)
                {
                    f = pw[ch].bendFnum;
                }
                f = f + pw[ch].detune;
                for (int lfo = 0; lfo < 4; lfo++)
                {
                    if (!pw[ch].lfo[lfo].sw)
                    {
                        continue;
                    }
                    if (pw[ch].lfo[lfo].type != eLfoType.Vibrato)
                    {
                        continue;
                    }
                    f += pw[ch].lfo[lfo].value + pw[ch].lfo[lfo].param[6];
                }

                f = checkRange(f, 0, 0x3ff);
                pw[ch].freq = f;

                byte pch = (byte)((ch - 9) & 3);
                byte data = 0;

                data = (byte)(0x80 + (pch << 5) + (f & 0xf));
                outPsgPort(data);

                data = (byte)((f & 0x3f0) >> 4);
                outPsgPort(data);
            }
            else
            {
                int f = pw[ch].noise;
                byte data = (byte)(0xe0 + (f & 0x7));
                pw[ch].freq = 0x40+(f & 7);
                //outPsgPort(0xe6);
                //outPsgPort(0x0d);
            }

        }

        private int getPsgFNum(int octave,char noteCmd,int shift)
        {
            int o = octave-1;//-2;
            int n = note.IndexOf(noteCmd) + shift;
            o += n / 12;
            //o = checkRange(o, 0, 6);
            o = checkRange(o, 0, 7);
            n %= 12;

            int f = o * 12 + n;
            if (f < 0) f = 0;
            if (f >= psgFNumTbl.Length) f = psgFNumTbl.Length - 1;

            return psgFNumTbl[f];
        }

        private void setPsgVolume(int ch)
        {
            byte pch = (byte)((ch - 9) & 3);
            byte data = 0;

            int vol = pw[ch].volume;

            if (pw[ch].envelopeMode)
            {
                vol = 0;
                if (pw[ch].envIndex != -1)
                {
                    vol = pw[ch].envVolume - (15 - pw[ch].volume);
                }
            }

            for (int lfo = 0; lfo < 4; lfo++)
            {
                if (!pw[ch].lfo[lfo].sw)
                {
                    continue;
                }
                if (pw[ch].lfo[lfo].type != eLfoType.Tremolo)
                {
                    continue;
                }
                vol += pw[ch].lfo[lfo].value + pw[ch].lfo[lfo].param[6];
            }

            vol = checkRange(vol, 0, 15);

            if (pw[ch].beforeVolume != vol)
            {
                data = (byte)(0x80 + (pch << 5) + 0x10 + (15 - vol));
                outPsgPort(data);
                pw[ch].beforeVolume = vol;
            }
        }


        private void setRf5c164FNum(int ch)
        {
            int f = getRf5c164PcmNote(pw[ch].octave, pw[ch].noteCmd, pw[ch].keyShift + pw[ch].shift);
            
            if (pw[ch].bendWaitCounter != -1)
            {
                f = pw[ch].bendFnum;
            }
            f = f + pw[ch].detune;
            for (int lfo = 0; lfo < 4; lfo++)
            {
                if (!pw[ch].lfo[lfo].sw)
                {
                    continue;
                }
                if (pw[ch].lfo[lfo].type != eLfoType.Vibrato)
                {
                    continue;
                }
                f += pw[ch].lfo[lfo].value + pw[ch].lfo[lfo].param[6];
            }

            f = checkRange(f, 0, 0xffff);
            pw[ch].freq = f;

            byte pch = (byte)((ch - 13) & 7);

            setRf5c164CurrentChannel((byte)ch);

            //Address increment 再生スピードをセット
            setRf5c164AddressIncrement((byte)ch, f);

            //Envelope 音量をセット
            //setRf5c164Envelope((byte)ch, pw[ch].volume);

        }

        private void setRf5c164Envelope(byte ch, int volume)
        {
            if (pw[ch].rf5c164Envelope != volume)
            {
                byte data = (byte)(volume & 0xff);
                outRf5c164Port(0x0, data);
                pw[ch].rf5c164Envelope = volume;
            }
        }

        private void setRf5c164Pan(byte ch, int pan)
        {
            if (pw[ch].rf5c164Pan != pan)
            {
                byte data = (byte)(pan & 0xff);
                outRf5c164Port(0x1, data);
                pw[ch].rf5c164Pan = pan;
            }
        }

        private void setRf5c164CurrentChannel(byte ch)
        {
            byte pch = (byte)((ch - 13) & 0x7);
            if (rf5c164CurrentChannel != pch)
            {
                byte data = (byte)(0xc0 + pch);
                outRf5c164Port(0x7, data);
                rf5c164CurrentChannel = pch;
            }
        }

        private void setRf5c164AddressIncrement(byte ch,int f)
        {
            if (pw[ch].rf5c164AddressIncrement != f)
            {
                byte data = (byte)(f & 0xff);
                outRf5c164Port(0x2, data);
                data = (byte)((f >> 8) & 0xff);
                outRf5c164Port(0x3, data);
                pw[ch].rf5c164AddressIncrement = f;
            }
        }

        private void setRf5c164SampleStartAddress(byte ch, int adr)
        {
            if (pw[ch].rf5c164SampleStartAddress != adr)
            {
                byte data = (byte)((adr >> 8) & 0xff);
                outRf5c164Port(0x6, data);
                pw[ch].rf5c164SampleStartAddress = adr;
            }
        }

        private void setRf5c164LoopAddress(byte ch, int adr)
        {
            if (pw[ch].rf5c164LoopAddress != adr)
            {
                byte data = (byte)((adr >> 8) & 0xff);
                outRf5c164Port(0x5, data);
                data = (byte)(adr & 0xff);
                outRf5c164Port(0x4, data);
                pw[ch].rf5c164LoopAddress = adr;
            }
        }

        private void outRf5c164KeyOn(byte ch)
        {
            byte pch = (byte)((ch - 13) & 0x7);
            rf5c164KeyOn |= (byte)(1 << pch);
            byte data = (byte)(~rf5c164KeyOn);
            outRf5c164Port(0x8, data);
        }

        private void outRf5c164KeyOff(byte ch)
        {
            byte pch = (byte)((ch - 13) & 0x7);
            rf5c164KeyOn &= (byte)(~(1 << pch));
            byte data = (byte)(~rf5c164KeyOn);
            outRf5c164Port(0x8, data);
        }



        private void outFmAdrPort(bool isPort2, byte address, byte data)
        {
            outFmAdrPort((byte)(isPort2 ? 0x53 : 0x52),address,data);
        }

        private void outFmAdrPort(byte port, byte address, byte data)
        {
            dat.Add(port);
            dat.Add(address);
            dat.Add(data);
        }

        private void outFmKeyOn(byte ch)
        {
            if (!pw[ch].pcm)
            {
                if (!pw[2].Ch3SpecialMode || (ch != 2 && ch < 6))
                {
                    if (ch < 6)
                    {
                        byte vch = (byte)((ch > 2) ? ch + 1 : ch);
                        //key on
                        outFmAdrPort(0x52, 0x28, (byte)((pw[ch].slots << 4) + (vch & 7)));
                    }
                }
                else
                {
                    pw[ch].Ch3SpecialModeKeyOn = true;

                    int slot = (pw[2].Ch3SpecialModeKeyOn ? pw[2].slots : 0x0)
                        | (pw[6].Ch3SpecialModeKeyOn ? pw[6].slots : 0x0)
                        | (pw[7].Ch3SpecialModeKeyOn ? pw[7].slots : 0x0)
                        | (pw[8].Ch3SpecialModeKeyOn ? pw[8].slots : 0x0);

                    outFmAdrPort(0x52, 0x28, (byte)((slot << 4) + 2));

                }
                return;
            }

            float m = pcmMTbl[pw[ch].pcmNote] * (float)Math.Pow(2, (pw[ch].pcmOctave - 4));
            pcmBaseFreqPerFreq = vgmSamplesPerSecond / ((float)instPCM[pw[ch].instrument].freq * m);
            pcmFreqCountBuffer = 0.0f;
            long p = instPCM[pw[ch].instrument].stAdr;
            dat.Add(0xe0);
            dat.Add((byte)(p & 0xff));
            dat.Add((byte)((p & 0xff00) / 0x100));
            dat.Add((byte)((p & 0xff0000) / 0x10000));
            dat.Add((byte)((p & 0xff000000) / 0x10000));

        }

        private void outFmKeyOff(byte ch)
        {
            if (!pw[ch].pcm)
            {
                if (!pw[2].Ch3SpecialMode || (ch!=2 && ch<6))
                {
                    if (ch < 6)
                    {
                        byte vch = (byte)((ch > 2) ? ch + 1 : ch);
                        //key off
                        outFmAdrPort(0x52, 0x28, (byte)(0x00 + (vch & 7)));
                    }
                }
                else
                {
                    pw[ch].Ch3SpecialModeKeyOn = false;

                    int slot = (pw[2].Ch3SpecialModeKeyOn ? pw[2].slots : 0x0)
                        | (pw[6].Ch3SpecialModeKeyOn ? pw[6].slots : 0x0)
                        | (pw[7].Ch3SpecialModeKeyOn ? pw[7].slots : 0x0)
                        | (pw[8].Ch3SpecialModeKeyOn ? pw[8].slots : 0x0);

                    outFmAdrPort(0x52, 0x28, (byte)((slot << 4) + 2));

                }
                return;
            }

            waitKeyOnPcmCounter = -1;
        }

        private void outFmAllKeyOff()
        {
            for(int i = 0; i < 6; i++)
            {
                outFmKeyOff((byte)i);
                outFmSetTl((byte)i, 0, 127);
                outFmSetTl((byte)i, 1, 127);
                outFmSetTl((byte)i, 2, 127);
                outFmSetTl((byte)i, 3, 127);
            }
        }

        private void outFmSetFnum(byte ch, int octave, int num)
        {

            pw[ch].freq = ((num & 0x700) >> 8) + (((octave - 1) & 0x7) << 3);
            pw[ch].freq = (pw[ch].freq << 8) + (num & 0xff);

            if (pw[2].Ch3SpecialMode && (ch == 2 || ch == 6 || ch == 7 || ch == 8))
            {
                byte port = (byte)0x52;

                if ((pw[ch].slots & 8) != 0)
                {
                    int f = pw[ch].freq + pw[ch].slotDetune[3];
                    outFmAdrPort(port, (byte)0xa6, (byte)((f & 0xff00) >> 8));
                    outFmAdrPort(port, (byte)0xa2, (byte)(f & 0xff));
                }
                if ((pw[ch].slots & 4) != 0)
                {
                    int f=pw[ch].freq + pw[ch].slotDetune[2];
                    outFmAdrPort(port, (byte)0xac, (byte)((f & 0xff00) >> 8));
                    outFmAdrPort(port, (byte)0xa8, (byte)(f & 0xff));
                }
                if ((pw[ch].slots & 1) != 0)
                {
                    int f = pw[ch].freq + pw[ch].slotDetune[0];
                    outFmAdrPort(port, (byte)0xad, (byte)((f & 0xff00) >> 8));
                    outFmAdrPort(port, (byte)0xa9, (byte)(f & 0xff));
                }
                if ((pw[ch].slots & 2) != 0)
                {
                    int f = pw[ch].freq + pw[ch].slotDetune[1];
                    outFmAdrPort(port, (byte)0xae, (byte)((f & 0xff00) >> 8));
                    outFmAdrPort(port, (byte)0xaa, (byte)(f & 0xff));
                }
            }
            else
            {
                if (ch > 5)
                {
                    return;
                }
                byte port = (byte)(0x52 + (ch > 2 ? 1 : 0));
                byte vch = (byte)(ch > 2 ? ch - 3 : ch);

                outFmAdrPort(port, (byte)(0xa4 + vch), (byte)((pw[ch].freq & 0xff00) >> 8));
                outFmAdrPort(port, (byte)(0xa0 + vch), (byte)(pw[ch].freq & 0xff));
            }
        }

        private void outFmCh3SpecialModeSetFnum(byte ope, int octave, int num)
        {
            byte port = (byte)0x52;
            ope &= 3;
            if (ope == 0)
            {
                outFmAdrPort(port, (byte)(0xa6), (byte)(((num & 0x700) >> 8) + (((octave - 1) & 0x7) << 3)));
                outFmAdrPort(port, (byte)(0xa2), (byte)(num & 0xff));
            }
            else
            {
                outFmAdrPort(port, (byte)(0xac+ope), (byte)(((num & 0x700) >> 8) + (((octave - 1) & 0x7) << 3)));
                outFmAdrPort(port, (byte)(0xa8+ope), (byte)(num & 0xff));
            }
        }

        private void outFmSetInstrument(byte ch, int n, int vol)
        {

            if (!instFM.ContainsKey(n))
            {
                msgBox.setWrnMsg(string.Format("未定義の音色(@{0})を指定しています。", n), lineNumber);
                return;
            }

            if (ch > 5)
            {
                msgBox.setWrnMsg("拡張チャンネルでは音色指定はできません。", lineNumber);
                return;
            }

            for (int ope = 0; ope < 4; ope++)
            {

                outFmSetDtMl(ch, ope, instFM[n][ope * instrumentMOperaterSize + 9], instFM[n][ope * instrumentMOperaterSize + 8]);
                outFmSetKsAr(ch, ope, instFM[n][ope * instrumentMOperaterSize + 7], instFM[n][ope * instrumentMOperaterSize + 1]);
                outFmSetAmDr(ch, ope, instFM[n][ope * instrumentMOperaterSize + 10], instFM[n][ope * instrumentMOperaterSize + 2]);
                outFmSetSr(ch, ope, instFM[n][ope * instrumentMOperaterSize + 3]);
                outFmSetSlRr(ch, ope, instFM[n][ope * instrumentMOperaterSize + 5], instFM[n][ope * instrumentMOperaterSize + 4]);
                outFmSetSSGEG(ch, ope, instFM[n][ope * instrumentMOperaterSize + 11]);

            }

            outFmSetFeedbackAlgorithm(ch, instFM[n][46], instFM[n][45]);

            outFmSetVolume(ch, vol, n);
            
        }

        /// <summary>
        /// FMボリュームの設定
        /// </summary>
        /// <param name="ch">チャンネル</param>
        /// <param name="vol">ボリューム値</param>
        /// <param name="n">音色番号</param>
        private void outFmSetVolume(byte ch, int vol, int n)
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

            byte vch = ch;
            if (pw[2].Ch3SpecialMode && ch > 5) { vch = 2; }

            if ((pw[ch].slots & 1) != 0) outFmSetTl(vch, 0, ope[0]);
            if ((pw[ch].slots & 2) != 0) outFmSetTl(vch, 1, ope[1]);
            if ((pw[ch].slots & 4) != 0) outFmSetTl(vch, 2, ope[2]);
            if ((pw[ch].slots & 8) != 0) outFmSetTl(vch, 3, ope[3]);
        }

        private void outFmSetDtMl(byte ch, int ope, int dt, int ml)
        {
            byte port = (byte)(0x52 + (ch > 2 ? 1 : 0));
            ch = (byte)(ch > 2 ? ch - 3 : ch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            dt &= 7;
            ml &= 15;

            outFmAdrPort(port, (byte)(0x30 + ch + ope * 4), (byte)((dt << 4) + ml));
        }

        private void outFmSetTl(byte ch, int ope, int tl)
        {
            byte port = (byte)(0x52 + (ch > 2 ? 1 : 0));
            ch = (byte)(ch > 2 ? ch - 3 : ch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            tl &= 0x7f;

            outFmAdrPort(port,(byte)(0x40 + ch + ope * 4),(byte)tl);
        }

        private void outFmSetKsAr(byte ch, int ope, int ks, int ar)
        {
            byte port = (byte)(0x52 + (ch > 2 ? 1 : 0));
            ch = (byte)(ch > 2 ? ch - 3 : ch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            ks &= 3;
            ar &= 31;

            outFmAdrPort(port, (byte)(0x50 + ch + ope * 4), (byte)((ks << 6) + ar));
        }

        private void outFmSetAmDr(byte ch, int ope, int am,int dr)
        {
            byte port = (byte)(0x52 + (ch > 2 ? 1 : 0));
            ch = (byte)(ch > 2 ? ch - 3 : ch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            am &= 1;
            dr &= 31;

            outFmAdrPort(port, (byte)(0x60 + ch + ope * 4), (byte)((am << 7) + dr));
        }

        private void outFmSetSr(byte ch, int ope, int sr)
        {
            byte port = (byte)(0x52 + (ch > 2 ? 1 : 0));
            ch = (byte)(ch > 2 ? ch - 3 : ch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            sr &= 31;

            dat.Add(port); dat.Add((byte)(0x70 + ch + ope * 4)); dat.Add((byte)sr);
        }

        private void outFmSetSlRr(byte ch, int ope, int sl, int rr)
        {
            byte port = (byte)(0x52 + (ch > 2 ? 1 : 0));
            ch = (byte)(ch > 2 ? ch - 3 : ch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            sl &= 15;
            rr &= 15;

            dat.Add(port); dat.Add((byte)(0x80 + ch + ope * 4)); dat.Add((byte)((sl << 4) + rr));
        }

        private void outFmSetSSGEG(byte ch, int ope, int n)
        {
            byte port = (byte)(0x52 + (ch > 2 ? 1 : 0));
            ch = (byte)(ch > 2 ? ch - 3 : ch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            n &= 15;

            dat.Add(port); dat.Add((byte)(0x90 + ch + ope * 4)); dat.Add((byte)n);
        }

        private void outFmSetFeedbackAlgorithm(byte ch, int fb, int alg)
        {
            byte port = (byte)(0x52 + (ch > 2 ? 1 : 0));
            ch = (byte)(ch > 2 ? ch - 3 : ch);

            fb &= 7;
            alg &= 7;

            dat.Add(port); dat.Add((byte)(0xb0 + ch)); dat.Add((byte)((fb << 3) + alg));
        }

        private void outFmSetPanAMSFMS(byte ch, int pan, int ams, int fms)
        {
            byte port = (byte)(0x52 + (ch > 2 ? 1 : 0));
            ch = (byte)(ch > 2 ? ch - 3 : ch);

            pan = pan & 3;
            ams = ams & 3;
            fms = fms & 3;

            dat.Add(port); dat.Add((byte)(0xb4 + ch)); dat.Add((byte)((pan << 6) + (ams << 4) + fms));
        }

        private void outFmSetHardLfo(bool sw,int lfoNum)
        {
            dat.Add(0x52); dat.Add(0x22); dat.Add((byte)((lfoNum & 7) + (sw ? 8 : 0)));
        }

        private void outFmSetCh3SpecialMode(bool sw)
        {
            // ignore Timer ^^;
            dat.Add(0x52); dat.Add(0x27); dat.Add((byte)((sw ? 0x40 : 0)));
        }

        private void outFmSetCh6PCMMode(bool sw)
        {
            dat.Add(0x52); dat.Add(0x2b); dat.Add((byte)((sw ? 0x80 : 0)));
        }



        private void outPsgPort(byte data)
        {
            dat.Add(0x50);
            dat.Add(data);
        }

        private void _outPsgSetFnum(byte ch, int freq)
        {
            pw[ch].freq=freq;
        }

        //long psgc = 0;
        private void outPsgKeyOn(byte ch)
        {
            byte pch = (byte)((ch - 9) & 3);
            byte data = 0;


            //if (ch == 9) Console.Write("[{0:x}", pw[ch].freq);
            data = (byte)(0x80 + (pch << 5) + 0x00 + (pw[ch].freq & 0xf));
            outPsgPort(data);
            //if (ch == 9) Console.Write(":{0:x}", data);

            if (ch != 12)
            {
                data = (byte)((pw[ch].freq & 0x3f0) >> 4);
                outPsgPort(data);
                //if (ch == 9) Console.Write(":{0:x}", data);
            }

            int vol = pw[ch].volume;
            if (pw[ch].envelopeMode)
            {
                vol = 0;
                if (pw[ch].envIndex != -1)
                {
                    vol = pw[ch].envVolume - (15 - pw[ch].volume);
                }
            }
            if (vol > 15) vol = 15;
            if (vol < 0) vol = 0;
            data = (byte)(0x80 + (pch << 5) + 0x10 + (15 - vol));
            //if (ch == 9) Console.WriteLine(":{0:x}:{1:x}]:{2}", vol, data, psgc++);
            outPsgPort(data);

        }

        private void outPsgKeyOff(byte ch)
        {

            //Console.Write("[KeyOff]");
            byte pch = (byte)((ch - 9) & 3);
            int val = 15;

            byte data = (byte)(0x80 + (pch << 5) + 0x10 + (val & 0xf));
            outPsgPort(data);

        }


        private void outRf5c164Port(byte adr, byte data)
        {
            dat.Add(0xb1);
            dat.Add(adr);
            dat.Add(data);
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

        private void outWaitNSamplesWithPCMSending(long cnt)
        {
            for (int i = 0; i < samplesPerClock * cnt;)
            {
                int f = (int)pcmBaseFreqPerFreq;
                pcmFreqCountBuffer += pcmBaseFreqPerFreq - (int)pcmBaseFreqPerFreq;
                while (pcmFreqCountBuffer > 1.0f)
                {
                    f++;
                    pcmFreqCountBuffer -= 1.0f;
                }
                if (i + f >= samplesPerClock * cnt)
                {
                    pcmFreqCountBuffer += (int)(i + f - samplesPerClock * cnt);
                    f = (int)(samplesPerClock * cnt - i);
                }
                if (pcmSizeCounter > 0)
                {
                    pcmSizeCounter--;
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
