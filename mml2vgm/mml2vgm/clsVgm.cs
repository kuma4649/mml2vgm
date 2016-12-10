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

        public clsChip ym2612 = new clsChip();
        public clsChip sn76489 = new clsChip();
        public clsChip rf5c164 = new clsChip();
        public List<clsChip> chips = new List<clsChip>();

        public int lineNumber = 0;

        private const int instrumentSize = 39 + 8;
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

        private const string PARTNAME = "PART";
        private const string PARTRF5C164 = "PARTRF5C164";

        private const string CLOCKCOUNT = "CLOCKCOUNT";
        private const string FMF_NUM = "FMF-NUM";
        private const string PSGF_NUM = "PSGF-NUM";
        private const string FORCEDMONOPARTYM2612 = "FORCEDMONOPARTYM2612";
        private const string VERSION = "VERSION";

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

        private bool ym2612SetupStream = false;
        private long ym2612StreamFreq = 0;

        public clsVgm()
        {
            chips = new List<clsChip>();

            ym2612 = new clsChip();
            ym2612.Name = "YM2612";
            ym2612.ShortName = "OPN2";
            ym2612.Type = enmChipType.YM2612;
            ym2612.Frequency = 7670454;
            ym2612.ChMax = 9;
            ym2612.Ch = new clsChannel[2][] { new clsChannel[ym2612.ChMax], new clsChannel[ym2612.ChMax] };
            setPartToCh(ym2612.Ch[0], "F");
            setPartToCh(ym2612.Ch[1], "Fs");
            for (int chipNum = 0; chipNum < 2; chipNum++)
            {
                foreach (clsChannel ch in ym2612.Ch[chipNum])
                {
                    ch.Type = enmChannelType.FMOPN;
                    ch.isSecondary = chipNum == 1;
                }
                ym2612.Ch[chipNum][5].Type = enmChannelType.FMPCM;
                ym2612.Ch[chipNum][6].Type = enmChannelType.FMOPNex;
                ym2612.Ch[chipNum][7].Type = enmChannelType.FMOPNex;
                ym2612.Ch[chipNum][8].Type = enmChannelType.FMOPNex;
            }
            chips.Add(ym2612);

            sn76489 = new clsChip();
            sn76489.Name = "SN76489";
            sn76489.ShortName = "DCSG";
            sn76489.Type = enmChipType.SN76489;
            sn76489.Frequency = 3579545;
            sn76489.ChMax = 4;
            sn76489.Ch = new clsChannel[2][] { new clsChannel[sn76489.ChMax], new clsChannel[sn76489.ChMax] };
            setPartToCh(sn76489.Ch[0], "S");
            setPartToCh(sn76489.Ch[1], "Ss");
            for (int chipNum = 0; chipNum < 2; chipNum++)
            {
                foreach (clsChannel ch in sn76489.Ch[chipNum])
                {
                    ch.Type = enmChannelType.DCSG;
                    ch.isSecondary = chipNum == 1;
                }
                sn76489.Ch[chipNum][3].Type = enmChannelType.DCSGNOISE;
            }
            chips.Add(sn76489);

            rf5c164 = new clsChip();
            rf5c164.Name = "RF5C164";
            rf5c164.ShortName = "RF5C";
            rf5c164.Type = enmChipType.RF5C164;
            rf5c164.Frequency = 12500000;
            rf5c164.ChMax = 8;
            rf5c164.Ch = new clsChannel[2][] { new clsChannel[rf5c164.ChMax], new clsChannel[rf5c164.ChMax] };
            setPartToCh(rf5c164.Ch[0], "R");
            setPartToCh(rf5c164.Ch[1], "Rs");
            for (int chipNum = 0; chipNum < 2; chipNum++)
            {
                foreach (clsChannel ch in rf5c164.Ch[chipNum])
                {
                    ch.Type = enmChannelType.PCM;
                    ch.isSecondary = chipNum == 1;
                }
            }
            chips.Add(rf5c164);

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
                    if (chip.ChannelNameContains(0, p) && chip.ChannelNameContains(1, p))
                    {
                        flg = true;
                        break;
                    }
                }
                if (flg)
                {
                    msgBox.setWrnMsg(string.Format("未定義のパート({0})のデータは無視されます。", p));
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

                    if (wrd == PARTNAME + ym2612.Name + "SECONDARY") setPartToCh(ym2612.Ch[1], val);
                    if (wrd == PARTNAME + ym2612.ShortName + "SECONDARY") setPartToCh(ym2612.Ch[1], val);
                    if (wrd == PARTNAME + ym2612.Name + "PRIMARY") setPartToCh(ym2612.Ch[0], val);
                    if (wrd == PARTNAME + ym2612.ShortName + "PRIMARY") setPartToCh(ym2612.Ch[0], val);
                    if (wrd == PARTNAME + ym2612.Name) setPartToCh(ym2612.Ch[0], val);
                    if (wrd == PARTNAME + ym2612.ShortName) setPartToCh(ym2612.Ch[0], val);

                    if (wrd == PARTNAME + sn76489.Name + "SECONDARY") setPartToCh(sn76489.Ch[1], val);
                    if (wrd == PARTNAME + sn76489.ShortName + "SECONDARY") setPartToCh(sn76489.Ch[1], val);
                    if (wrd == PARTNAME + sn76489.Name + "PRIMARY") setPartToCh(sn76489.Ch[0], val);
                    if (wrd == PARTNAME + sn76489.ShortName + "PRIMARY") setPartToCh(sn76489.Ch[0], val);
                    if (wrd == PARTNAME + sn76489.Name) setPartToCh(sn76489.Ch[0], val);
                    if (wrd == PARTNAME + sn76489.ShortName) setPartToCh(sn76489.Ch[0], val);

                    if (wrd == PARTNAME + rf5c164.Name + "SECONDARY") setPartToCh(rf5c164.Ch[1], val);
                    if (wrd == PARTNAME + rf5c164.ShortName + "SECONDARY") setPartToCh(rf5c164.Ch[1], val);
                    if (wrd == PARTNAME + rf5c164.Name + "PRIMARY") setPartToCh(rf5c164.Ch[0], val);
                    if (wrd == PARTNAME + rf5c164.ShortName + "PRIMARY") setPartToCh(rf5c164.Ch[0], val);
                    if (wrd == PARTNAME + rf5c164.Name) setPartToCh(rf5c164.Ch[0], val);
                    if (wrd == PARTNAME + rf5c164.ShortName) setPartToCh(rf5c164.Ch[0], val);

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

            //checkDuplication(fCh);
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
        private float pcmBaseFreqPerFreq = 0.0f;
        private float pcmFreqCountBuffer = 0.0f;
        private long waitKeyOnPcmCounter = 0L;
        private long pcmSizeCounter = 0L;
        public List<partWork> lstPartWork = null;
        private Random rnd = new Random();


        public byte[] getByteData()
        {

            lstPartWork = new List<partWork>();
            partWork pw = null;

            foreach (clsChip chip in chips)
            {

                for (int chipNum = 0; chipNum < 2; chipNum++)
                {
                    chip.partStartCh[chipNum] = lstPartWork.Count;
                    chip.use[chipNum] = false;

                    for (int i = 0; i < chip.ChMax; i++)
                    {
                        pw = new partWork();
                        pw.chip = chip;
                        pw.isSecondary = (chipNum == 1);
                        pw.ch = i + 1;
                        if (partData.ContainsKey(chip.Ch[chipNum][i].Name))
                        {
                            pw.pData = partData[chip.Ch[chipNum][i].Name];
                        }
                        pw.aData = aliesData;
                        pw.setPos(0);
                        switch (chip.Type)
                        {
                            case enmChipType.YM2612:
                                pw.slots = (byte)((chip.Ch[chipNum][i].Type == enmChannelType.FMOPN || chip.Ch[chipNum][i].Type == enmChannelType.FMPCM) ? 0xf : 0x0);
                                pw.volume = 127;
                                break;
                            default:
                                pw.slots = 0;
                                pw.volume = 32767;
                                break;
                        }

                        pw.PartName = chip.Ch[chipNum][i].Name;
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
                            chip.use[chipNum] = true;
                        }

                        lstPartWork.Add(pw);

                    }
                }
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

                for (int ch = 0; ch < lstPartWork.Count; ch++)
                {
                    partWork cpw = lstPartWork[ch];

                    if (!cpw.chip.use[cpw.isSecondary ? 1 : 0])
                    {
                        continue;
                    }

                    if (cpw.waitKeyOnCounter == 0)
                    {
                        if (cpw.chip.Type == enmChipType.YM2612)
                        {
                            if (!cpw.tie) outFmKeyOff((byte)ch);
                        }
                        else if (cpw.chip.Type == enmChipType.SN76489)
                        {
                            if (!cpw.envelopeMode)
                            {
                                if (!cpw.tie) outPsgKeyOff((byte)ch);
                            }
                            else
                            {
                                if (cpw.envIndex != -1)
                                {
                                    if (!cpw.tie)
                                    {
                                        cpw.envIndex = 3;//RR phase
                                        cpw.envCounter = 0;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (!cpw.envelopeMode)
                            {
                                if (!cpw.tie) outRf5c164KeyOff((byte)ch);
                            }
                            else
                            {
                                if (cpw.envIndex != -1)
                                {
                                    if (!cpw.tie)
                                    {
                                        cpw.envIndex = 3;//RR phase
                                        cpw.envCounter = 0;
                                    }
                                }
                            }
                        }


                        //次回に引き継ぎリセット
                        cpw.beforeTie = cpw.tie;
                        cpw.tie = false;

                        //ゲートタイムカウンターをリセット
                        cpw.waitKeyOnCounter = -1;
                    }

                    //bend処理
                    if (cpw.bendWaitCounter == 0)
                    {
                        if (cpw.bendList.Count > 0)
                        {
                            Tuple<int, int> bp = cpw.bendList.Pop();
                            cpw.bendFnum = bp.Item1;
                            cpw.bendWaitCounter = bp.Item2;
                        }
                        else
                        {
                            cpw.bendWaitCounter = -1;
                        }
                    }

                    //lfo処理
                    for (int lfo = 0; lfo < 4; lfo++)
                    {
                        clsLfo pl = cpw.lfo[lfo];

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
                            if (cpw.chip.Type == enmChipType.YM2612)
                            {
                                outFmSetPanAMSFMS((byte)ch, cpw.pan, cpw.lfo[lfo].param[3], cpw.lfo[lfo].param[2]);
                                outFmSetHardLfo(ch > 8, true, cpw.lfo[lfo].param[1]);
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
                    if (cpw.chip.Type == enmChipType.SN76489 || cpw.chip.Type == enmChipType.RF5C164)
                    {
                        envelope(ch);
                    }

                    if (cpw.chip.Type == enmChipType.YM2612)
                    {
                        setFmFNum(ch);
                        setFmVolume(ch);
                    }
                    else if (cpw.chip.Type == enmChipType.SN76489)
                    {
                        if (cpw.waitKeyOnCounter > 0 || cpw.envIndex != -1)
                        {
                            setPsgFNum(ch);
                            setPsgVolume(ch);
                        }
                    }
                    else
                    {
                        if (cpw.waitKeyOnCounter > 0 || cpw.envIndex != -1)
                        {
                            setRf5c164FNum(ch);
                            setRf5c164Volume(ch);
                        }
                    }

                    //wait消化待ち
                    if (cpw.waitCounter > 0)
                    {
                        continue;
                    }

                    //データは最後まで実施されたか
                    if (cpw.dataEnd)
                    {
                        continue;
                    }

                    //パートのデータがない場合は何もしないで次へ
                    if (!partData.ContainsKey(cpw.PartName) || partData[cpw.PartName] == null || partData[cpw.PartName].Count < 1)
                    {
                        cpw.dataEnd = true;
                        continue;
                    }

                    while (cpw.waitCounter == 0 && !cpw.dataEnd)
                    {
                        char cmd = cpw.getChar();
                        lineNumber = cpw.getLineNumber();

                        commander(ch, cmd);
                    }
                }


                // 全パートのうち次のコマンドまで一番近い値を求める
                long cnt = long.MaxValue;
                //note
                for (int ch = 0; ch < lstPartWork.Count; ch++)
                {
                    if (lstPartWork[ch].waitKeyOnCounter > 0)
                    {
                        cnt = Math.Min(cnt, lstPartWork[ch].waitKeyOnCounter);
                        continue;
                    }

                    if (lstPartWork[ch].waitCounter > 0)
                    {
                        cnt = Math.Min(cnt, lstPartWork[ch].waitCounter);
                    }
                }
                //bend
                for (int ch = 0; ch < lstPartWork.Count; ch++)
                {
                    if (lstPartWork[ch].bendWaitCounter == -1)
                    {
                        continue;
                    }
                    cnt = Math.Min(cnt, lstPartWork[ch].bendWaitCounter);
                }
                //lfo
                for (int ch = 0; ch < lstPartWork.Count; ch++)
                {
                    for (int lfo = 0; lfo < 4; lfo++)
                    {
                        if (!lstPartWork[ch].lfo[lfo].sw)
                        {
                            continue;
                        }
                        if (lstPartWork[ch].lfo[lfo].waitCounter == -1)
                        {
                            continue;
                        }
                        if (loopOffset != -1 && lstPartWork[ch].dataEnd) continue;
                        cnt = Math.Min(cnt, lstPartWork[ch].lfo[lfo].waitCounter);
                    }
                }
                //pcm
                if (waitKeyOnPcmCounter > 0)
                {
                    cnt = Math.Min(cnt, waitKeyOnPcmCounter);
                }
                //envelope
                for (int ch = 0; ch < sn76489.ChMax * 2; ch++)
                {
                    partWork cpw = lstPartWork[sn76489.partStartCh[0] + ch];
                    if (!cpw.chip.use[cpw.isSecondary ? 1 : 0]) continue;

                    if (!cpw.envelopeMode) continue;
                    if (cpw.envIndex == -1) continue;
                    if (loopOffset != -1 && cpw.dataEnd && cpw.envIndex == 3) continue;

                    cnt = Math.Min(cnt, cpw.envCounter);
                }

                //envelope
                for (int ch = 0; ch < rf5c164.ChMax * 2; ch++)
                {
                    partWork cpw = lstPartWork[rf5c164.partStartCh[0] + ch];
                    if (!cpw.chip.use[cpw.isSecondary ? 1 : 0]) continue;

                    if (!cpw.envelopeMode) continue;
                    if (cpw.envIndex == -1) continue;
                    if (loopOffset != -1 && cpw.dataEnd && cpw.envIndex == 3) continue;

                    cnt = Math.Min(cnt, cpw.envCounter);
                }


                if (cnt != long.MaxValue)
                {

                    // waitcounterを減らす

                    for (int ch = 0; ch < lstPartWork.Count; ch++)
                    {

                        partWork cpw = lstPartWork[ch];

                        if (cpw.waitKeyOnCounter > 0) cpw.waitKeyOnCounter -= cnt;

                        if (cpw.waitCounter > 0) cpw.waitCounter -= cnt;

                        if (cpw.bendWaitCounter > 0) cpw.bendWaitCounter -= cnt;

                        for (int lfo = 0; lfo < 4; lfo++)
                        {
                            if (!cpw.lfo[lfo].sw) continue;
                            if (cpw.lfo[lfo].waitCounter == -1) continue;

                            if (cpw.lfo[lfo].waitCounter > 0) cpw.lfo[lfo].waitCounter -= cnt;
                        }
                    }

                    if (waitKeyOnPcmCounter > 0)
                    {
                        waitKeyOnPcmCounter -= cnt;
                    }

                    for (int ch = 0; ch < sn76489.ChMax * 2; ch++)
                    {
                        partWork cpw = lstPartWork[sn76489.partStartCh[0] + ch];

                        if (!cpw.envelopeMode) continue;
                        if (cpw.envIndex == -1) continue;

                        cpw.envCounter -= (int)cnt;
                    }

                    for (int ch = 0; ch < rf5c164.ChMax * 2; ch++)
                    {
                        partWork cpw = lstPartWork[rf5c164.partStartCh[0] + ch];

                        if (!cpw.envelopeMode) continue;
                        if (cpw.envIndex == -1) continue;

                        cpw.envCounter -= (int)cnt;
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
                for (int ch = 0; ch < lstPartWork.Count; ch++)
                {
                    partWork cpw = lstPartWork[ch];

                    if (!cpw.chip.use[cpw.isSecondary ? 1 : 0]) endChannel++;
                    else if (cpw.dataEnd && cpw.waitCounter < 1) endChannel++;
                    else if (loopOffset != -1 && cpw.dataEnd && cpw.envIndex == 3) endChannel++;
                }

            } while (endChannel < (ym2612.ChMax + sn76489.ChMax + rf5c164.ChMax) * 2);


            makeFooter();


            return dat.ToArray();
        }

        private void envelope(int ch)
        {
            if (!lstPartWork[ch].envelopeMode)
            {
                return;
            }

            if (lstPartWork[ch].envIndex == -1)
            {
                return;
            }

            int maxValue = (lstPartWork[ch].envelope[8] == 2) ? 255 : 15;

            while (lstPartWork[ch].envCounter == 0 && lstPartWork[ch].envIndex != -1)
            {
                switch (lstPartWork[ch].envIndex)
                {
                    case 0: //AR phase
                        lstPartWork[ch].envVolume += lstPartWork[ch].envelope[7]; // vol += ST
                        if (lstPartWork[ch].envVolume >= maxValue)
                        {
                            lstPartWork[ch].envVolume = maxValue;
                            lstPartWork[ch].envCounter = lstPartWork[ch].envelope[3]; // DR
                            lstPartWork[ch].envIndex++;
                            break;
                        }
                        lstPartWork[ch].envCounter = lstPartWork[ch].envelope[2]; // AR
                        break;
                    case 1: //DR phase
                        lstPartWork[ch].envVolume -= lstPartWork[ch].envelope[7]; // vol -= ST
                        if (lstPartWork[ch].envVolume <= lstPartWork[ch].envelope[4]) // vol <= SL
                        {
                            lstPartWork[ch].envVolume = lstPartWork[ch].envelope[4];
                            lstPartWork[ch].envCounter = lstPartWork[ch].envelope[5]; // SR
                            lstPartWork[ch].envIndex++;
                            break;
                        }
                        lstPartWork[ch].envCounter = lstPartWork[ch].envelope[3]; // DR
                        break;
                    case 2: //SR phase
                        lstPartWork[ch].envVolume -= lstPartWork[ch].envelope[7]; // vol -= ST
                        if (lstPartWork[ch].envVolume <= 0) // vol <= 0
                        {
                            lstPartWork[ch].envVolume = 0;
                            lstPartWork[ch].envCounter = 0;
                            lstPartWork[ch].envIndex = -1;
                            break;
                        }
                        lstPartWork[ch].envCounter = lstPartWork[ch].envelope[5]; // SR
                        break;
                    case 3: //RR phase
                        lstPartWork[ch].envVolume -= lstPartWork[ch].envelope[7]; // vol -= ST
                        if (lstPartWork[ch].envVolume <= 0) // vol <= 0
                        {
                            lstPartWork[ch].envVolume = 0;
                            lstPartWork[ch].envCounter = 0;
                            lstPartWork[ch].envIndex = -1;
                            break;
                        }
                        lstPartWork[ch].envCounter = lstPartWork[ch].envelope[6]; // RR
                        break;
                }
            }

            if (lstPartWork[ch].envIndex == -1)
            {
                if (lstPartWork[ch].envelope[8] == 1)
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
                    lstPartWork[ch].incPos();
                    break;
                case '!': // CompileSkip
                    lstPartWork[ch].dataEnd = true;
                    lstPartWork[ch].waitCounter = -1;
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
                    msgBox.setErrMsg(string.Format("未知のコマンド{0}を検出しました。", cmd), lstPartWork[ch].getLineNumber());
                    lstPartWork[ch].incPos();
                    break;
            }
        }

        private void cmdNote(int ch, char cmd)
        {
            lstPartWork[ch].incPos();

            //+ -の解析
            int shift = 0;
            while (lstPartWork[ch].getChar() == '+' || lstPartWork[ch].getChar() == '-')
            {
                shift += lstPartWork[ch].getChar() == '+' ? 1 : -1;
                lstPartWork[ch].incPos();
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
                if (!lstPartWork[ch].getNum(out n))
                {
                    if (!isSecond)
                        n = (int)lstPartWork[ch].length;
                    else if (!isMinus)
                    {
                        //タイとして'&'が使用されている
                        lstPartWork[ch].tie = true;
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

                if (!lstPartWork[ch].tie)
                {
                    m += n;

                    //符点の解析
                    while (lstPartWork[ch].getChar() == '.')
                    {
                        if (n % 2 != 0)
                        {
                            msgBox.setWrnMsg("割り切れない.の指定があります。音長は不定です。", lineNumber);
                        }
                        n = n / 2;
                        m += n;
                        lstPartWork[ch].incPos();
                    }


                    if (isMinus) ml -= m;
                    else ml += m;
                }

                //ベンドの解析
                int bendDelayCounter = 0;
                int bendShift = 0;
                if (lstPartWork[ch].getChar() == '_')
                {
                    lstPartWork[ch].incPos();
                    lstPartWork[ch].octaveNow = lstPartWork[ch].octaveNew;
                    lstPartWork[ch].bendOctave = lstPartWork[ch].octaveNow;
                    lstPartWork[ch].bendNote = 'r';
                    lstPartWork[ch].bendWaitCounter = -1;
                    bool loop = true;
                    while (loop)
                    {
                        char bCmd = lstPartWork[ch].getChar();
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
                                lstPartWork[ch].incPos();
                                //+ -の解析
                                bendShift = 0;
                                while (lstPartWork[ch].getChar() == '+' || lstPartWork[ch].getChar() == '-')
                                {
                                    bendShift += lstPartWork[ch].getChar() == '+' ? 1 : -1;
                                    lstPartWork[ch].incPos();
                                }
                                lstPartWork[ch].bendShift = bendShift;
                                bendDelayCounter = 0;
                                n = -1;
                                isMinus = false;
                                isSecond = false;
                                do
                                {
                                    m = 0;

                                    //数値の解析
                                    if (!lstPartWork[ch].getNum(out n))
                                    {
                                        if (!isSecond)
                                        {
                                            n = 0;
                                            break;
                                        }
                                        else if (!isMinus)
                                        {
                                            //タイとして'&'が使用されている
                                            lstPartWork[ch].tie = true;
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

                                    if (!lstPartWork[ch].tie)
                                    {
                                        bendDelayCounter += n;

                                        //符点の解析
                                        while (lstPartWork[ch].getChar() == '.')
                                        {
                                            if (n % 2 != 0)
                                            {
                                                msgBox.setWrnMsg("割り切れない.の指定があります。音長は不定です。", lineNumber);
                                            }
                                            n = n / 2;
                                            m += n;
                                            lstPartWork[ch].incPos();
                                        }


                                        if (isMinus) bendDelayCounter -= m;
                                        else bendDelayCounter += m;
                                    }

                                    if (lstPartWork[ch].getChar() == '&')
                                    {
                                        isMinus = false;
                                    }
                                    else if (lstPartWork[ch].getChar() == '~')
                                    {
                                        isMinus = true;
                                    }
                                    else
                                    {
                                        break;
                                    }

                                    isSecond = true;
                                    lstPartWork[ch].incPos();

                                } while (true);

                                if (cmd != 'r')
                                {
                                    lstPartWork[ch].bendNote = bCmd;
                                    bendDelayCounter = checkRange(bendDelayCounter, 0, ml);
                                    lstPartWork[ch].bendWaitCounter = bendDelayCounter;
                                }
                                else
                                {
                                    msgBox.setErrMsg("休符にベンドの指定はできません。", lineNumber);
                                }

                                break;
                            case 'o':
                                lstPartWork[ch].incPos();
                                if (!lstPartWork[ch].getNum(out n))
                                {
                                    msgBox.setErrMsg("不正なオクターブが指定されています。", lineNumber);
                                    n = 110;
                                }
                                n = checkRange(n, 1, 8);
                                lstPartWork[ch].bendOctave = n;
                                break;
                            case '>':
                                lstPartWork[ch].incPos();
                                lstPartWork[ch].bendOctave++;
                                lstPartWork[ch].bendOctave = checkRange(lstPartWork[ch].bendOctave, 1, 8);
                                break;
                            case '<':
                                lstPartWork[ch].incPos();
                                lstPartWork[ch].bendOctave--;
                                lstPartWork[ch].bendOctave = checkRange(lstPartWork[ch].bendOctave, 1, 8);
                                break;
                            default:
                                loop = false;
                                break;
                        }
                    }

                    //音符の変化量
                    int ed = note.IndexOf(lstPartWork[ch].bendNote) + 1 + (lstPartWork[ch].bendOctave - 1) * 12 + lstPartWork[ch].bendShift;
                    ed = checkRange(ed, 0, 8 * 12 - 1);
                    int st = note.IndexOf(cmd) + 1 + (lstPartWork[ch].octaveNow - 1) * 12 + shift;//
                    st = checkRange(st, 0, 8 * 12 - 1);

                    int delta = ed - st;
                    if (delta == 0 || bendDelayCounter == ml)
                    {
                        lstPartWork[ch].bendNote = 'r';
                        lstPartWork[ch].bendWaitCounter = -1;
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
                            int a = getPsgFNum(lstPartWork[ch].octaveNow, cmd, shift + (i + 0) * Math.Sign(delta));//
                            int b = getPsgFNum(lstPartWork[ch].octaveNow, cmd, shift + (i + 1) * Math.Sign(delta));//
                            if (ch < 9*2)
                            {
                                a = getFmFNum(lstPartWork[ch].octaveNow, cmd, shift + (i + 0) * Math.Sign(delta));//
                                b = getFmFNum(lstPartWork[ch].octaveNow, cmd, shift + (i + 1) * Math.Sign(delta));//
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
                            else if (ch < 13*2)
                            {
                                a = getPsgFNum(lstPartWork[ch].octaveNow, cmd, shift + (i + 0) * Math.Sign(delta));//
                                b = getPsgFNum(lstPartWork[ch].octaveNow, cmd, shift + (i + 1) * Math.Sign(delta));//
                            }
                            else
                            {
                                a = getRf5c164PcmNote(lstPartWork[ch].octaveNow, cmd, shift + (i + 0) * Math.Sign(delta));//
                                b = getRf5c164PcmNote(lstPartWork[ch].octaveNow, cmd, shift + (i + 1) * Math.Sign(delta));//
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
                        lstPartWork[ch].bendList = new Stack<Tuple<int, int>>();
                        foreach (Tuple<int, int> lbt in lb)
                        {
                            lstPartWork[ch].bendList.Push(lbt);
                        }
                        Tuple<int, int> t = lstPartWork[ch].bendList.Pop();
                        lstPartWork[ch].bendFnum = t.Item1;
                        lstPartWork[ch].bendWaitCounter = t.Item2;
                    }
                }

                if (lstPartWork[ch].getChar() == '&')
                {
                    isMinus = false;
                }
                else if (lstPartWork[ch].getChar() == '~')
                {
                    isMinus = true;
                }
                else
                {
                    break;
                }

                isSecond = true;
                lstPartWork[ch].incPos();

            } while (true);

            if (ml < 1)
            {
                msgBox.setErrMsg("負の音長が指定されました。", lineNumber);
                ml = (int)lstPartWork[ch].length;
            }


            //装飾の解析完了


            //WaitClockの決定
            lstPartWork[ch].waitCounter = ml;

            if (cmd != 'r')
            {

                //発音周波数
                if (lstPartWork[ch].bendWaitCounter == -1)
                {
                    lstPartWork[ch].octaveNow = lstPartWork[ch].octaveNew;
                    lstPartWork[ch].noteCmd = cmd;
                    lstPartWork[ch].shift = shift;
                }
                else
                {
                    lstPartWork[ch].octaveNew = lstPartWork[ch].bendOctave;//
                    lstPartWork[ch].octaveNow = lstPartWork[ch].bendOctave;//
                    lstPartWork[ch].noteCmd = lstPartWork[ch].bendNote;
                    lstPartWork[ch].shift = lstPartWork[ch].bendShift;
                }

                //発音周波数の決定とキーオン
                if (ch < 9*2)
                {

                    //YM2612

                    if (!lstPartWork[ch].pcm)
                    {
                        setFmFNum(ch);
                    }
                    else
                    {
                        getPcmNote(lstPartWork[ch]);
                    }
                    //タイ指定では無い場合はキーオンする
                    if (!lstPartWork[ch].beforeTie)
                    {
                        setLfoAtKeyOn(ch);
                        setFmVolume(ch);
                        outFmKeyOn((byte)ch);
                    }
                }
                else if (ch < 13*2)
                {

                    // SN76489

                    setPsgFNum(ch);

                    //タイ指定では無い場合はキーオンする
                    if (!lstPartWork[ch].beforeTie)
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
                    if (!lstPartWork[ch].beforeTie)
                    {
                        setEnvelopeAtKeyOn(ch);
                        setLfoAtKeyOn(ch);
                        setRf5c164Envelope((byte)ch, lstPartWork[ch].volume);
                        outRf5c164KeyOn((byte)ch);
                    }
                }

                //gateTimeの決定
                if (lstPartWork[ch].gatetimePmode)
                {
                    lstPartWork[ch].waitKeyOnCounter = lstPartWork[ch].waitCounter * lstPartWork[ch].gatetime / 8L;
                }
                else
                {
                    lstPartWork[ch].waitKeyOnCounter = lstPartWork[ch].waitCounter - lstPartWork[ch].gatetime;
                }
                if (lstPartWork[ch].waitKeyOnCounter < 1) lstPartWork[ch].waitKeyOnCounter = 1;

                //PCM専用のWaitClockの決定
                if (lstPartWork[ch].pcm)
                {
                    waitKeyOnPcmCounter = -1;
                    if (Version != 1.60f)
                    {
                        waitKeyOnPcmCounter = lstPartWork[ch].waitKeyOnCounter;
                    }
                    pcmSizeCounter = instPCM[lstPartWork[ch].instrument].size;
                }
            }

            lstPartWork[ch].clockCounter += lstPartWork[ch].waitCounter;
        }

        private void cmdRepeatExit(int ch)
        {
            int n = -1;
            lstPartWork[ch].incPos();
            clsRepeat rx = lstPartWork[ch].stackRepeat.Pop();
            if (rx.repeatCount == 1)
            {
                int i = 0;
                while (true)
                {
                    char c = lstPartWork[ch].getChar();
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
                    lstPartWork[ch].incPos();
                }
                lstPartWork[ch].incPos();
                lstPartWork[ch].getNum(out n);
            }
            else
            {
                lstPartWork[ch].stackRepeat.Push(rx);
            }

        }

        private void cmdRepeatEnd(int ch)
        {
            int n;
            lstPartWork[ch].incPos();
            if (!lstPartWork[ch].getNum(out n))
            {
                n = 2;
            }
            n = checkRange(n, 1, 255);
            try
            {
                clsRepeat re = lstPartWork[ch].stackRepeat.Pop();
                if (re.repeatCount == -1)
                {
                    //初回
                    re.repeatCount = n;
                }
                re.repeatCount--;
                if (re.repeatCount > 0)
                {
                    lstPartWork[ch].stackRepeat.Push(re);
                    lstPartWork[ch].setPos(re.pos);
                }
            }
            catch
            {
                msgBox.setWrnMsg("[と]の数があいません。", lineNumber);
            }
        }

        private void cmdRepeatStart(int ch)
        {
            lstPartWork[ch].incPos();
            clsRepeat rs = new clsRepeat();
            rs.pos = lstPartWork[ch].getPos();
            rs.repeatCount = -1;//初期値
            lstPartWork[ch].stackRepeat.Push(rs);
        }

        private void cmdLoop(int ch)
        {
            lstPartWork[ch].incPos();
            loopOffset = (long)dat.Count;
            loopSamples = lSample;

            foreach (partWork p in lstPartWork)
            {
                p.freq = -1;

                if (p.chip.Type == enmChipType.RF5C164 && rf5c164.use[p.isSecondary ? 1 : 0])
                {
                    //rf5c164の設定済み周波数値を初期化(ループ時に直前の周波数を引き継いでしまうケースがあるため)
                    p.rf5c164AddressIncrement = -1;
                    int n = p.instrument;
                    p.rf5c164SampleStartAddress = -1;
                    p.rf5c164LoopAddress = -1;
                    if (n != -1)
                    {
                        setRf5c164CurrentChannel((byte)ch);
                        setRf5c164SampleStartAddress((byte)ch, (int)instPCM[n].stAdr);
                        setRf5c164LoopAddress((byte)ch, (int)(instPCM[n].loopAdr));
                    }
                }

            }
        }

        private void cmdEnvelope(int ch)
        {
            int n = -1;
            if (ch == 2 || (ch >= 6 && ch < 9) || ch == 11 || (ch >= 15 && ch < 18) || (ch >= 9*2 && ch < 13*2) || (ch>=13*2 && ch<21*2))
            {
                lstPartWork[ch].incPos();
                switch (lstPartWork[ch].getChar())
                {
                    case 'O':
                        lstPartWork[ch].incPos();
                        if (ch < 9)
                        {
                            switch (lstPartWork[ch].getChar())
                            {
                                case 'N':
                                    lstPartWork[ch].incPos();
                                    lstPartWork[ch].Ch3SpecialMode = true;
                                    outFmSetCh3SpecialMode(ch>8,true);
                                    break;
                                case 'F':
                                    lstPartWork[ch].incPos();
                                    lstPartWork[ch].Ch3SpecialMode = false;
                                    outFmSetCh3SpecialMode(ch > 8, false);
                                    break;
                                default:
                                    msgBox.setErrMsg(string.Format("未知のコマンド(EO{0})が指定されました。", lstPartWork[ch].getChar()), lineNumber);
                                    lstPartWork[ch].incPos();
                                    break;
                            }
                        }
                        else
                        {
                            switch (lstPartWork[ch].getChar())
                            {
                                case 'N':
                                    lstPartWork[ch].incPos();
                                    lstPartWork[ch].envelopeMode = true;
                                    break;
                                case 'F':
                                    lstPartWork[ch].incPos();
                                    lstPartWork[ch].envelopeMode = false;
                                    break;
                                default:
                                    msgBox.setErrMsg(string.Format("未知のコマンド(EO{0})が指定されました。", lstPartWork[ch].getChar()), lineNumber);
                                    lstPartWork[ch].incPos();
                                    break;
                            }
                        }
                        break;
                    case 'X':
                        char c = lstPartWork[ch].getChar();
                        if (ch < 9*2)
                        {
                            lstPartWork[ch].incPos();
                            if (!lstPartWork[ch].getNum(out n))
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
                                lstPartWork[ch].slots = res;
                            }
                        }
                        else
                        {
                            msgBox.setErrMsg("未知のコマンド(EX)が指定されました。", lineNumber);
                        }
                        break;
                    default:
                        if (ch == 2 || ch==11)
                        {
                            int[] s = new int[] { 0, 0, 0, 0 };

                            for (int i = 0; i < 4; i++)
                            {
                                if (lstPartWork[ch].getNum(out n))
                                {
                                    s[i] = n;
                                }
                                else
                                {
                                    msgBox.setErrMsg("Eコマンドの解析に失敗しました。", lineNumber);
                                    break;
                                }
                                if (i == 3) break;
                                lstPartWork[ch].incPos();
                            }
                            lstPartWork[ch].slotDetune = s;
                            break;
                        }
                        else
                        {
                            msgBox.setErrMsg(string.Format("未知のコマンド(E{0})が指定されました。", lstPartWork[ch].getChar()), lineNumber);
                            lstPartWork[ch].incPos();
                        }
                        break;
                }
            }
            else
            {
                msgBox.setWrnMsg("このパートは効果音モードに対応したチャンネルが指定されていないため、Eコマンドは無視されます。", lineNumber);
                lstPartWork[ch].incPos();
            }

        }

        private void cmdGatetime2(int ch)
        {
            int n;
            lstPartWork[ch].incPos();
            if (!lstPartWork[ch].getNum(out n))
            {
                msgBox.setErrMsg("不正なゲートタイム指定'Q'が指定されています。", lineNumber);
                n = 1;
            }
            n = checkRange(n, 1, 8);
            lstPartWork[ch].gatetime = n;
            lstPartWork[ch].gatetimePmode = true;
        }

        private void cmdGatetime(int ch)
        {
            int n;
            lstPartWork[ch].incPos();
            if (!lstPartWork[ch].getNum(out n))
            {
                msgBox.setErrMsg("不正なゲートタイム指定'q'が指定されています。", lineNumber);
                n = 0;
            }
            n = checkRange(n, 0, 255);
            lstPartWork[ch].gatetime = n;
            lstPartWork[ch].gatetimePmode = false;
        }

        private void cmdMode(int ch)
        {
            int n;
            lstPartWork[ch].incPos();
            if (ch == 5 || ch==14)
            {
                if (!lstPartWork[ch].getNum(out n))
                {
                    msgBox.setErrMsg("不正なPCMモード指定'm'が指定されています。", lineNumber);
                    n = 0;
                }
                n = checkRange(n, 0, 1);
                lstPartWork[ch].pcm = (n == 1);
                lstPartWork[ch].freq = -1;//freqをリセット
                outFmSetCh6PCMMode(ch > 8, lstPartWork[ch].pcm);
            }
            else
            {
                lstPartWork[ch].getNum(out n);
                msgBox.setWrnMsg("このパートは6chではないため、mコマンドは無視されます。", lineNumber);
            }

        }

        private void cmdDetune(int ch)
        {
            int n;
            lstPartWork[ch].incPos();
            if (!lstPartWork[ch].getNum(out n))
            {
                msgBox.setErrMsg("不正なディチューン'D'が指定されています。", lineNumber);
                n = 0;
            }
            n = checkRange(n, -127, 127);
            lstPartWork[ch].detune = n;
        }

        private void cmdPan(int ch)
        {
            int n;
            int vch = ch;

            //効果音モードのチャンネル番号を指定している場合は3chへ変更する
            if (ch == 6 || ch == 7 || ch == 8 || ch == 15 || ch == 16 || ch == 17)
            {
                vch = ch > 9 ? 11 : 2;
            }

            lstPartWork[ch].incPos();
            if (ch < 9*2)
            {
                if (!lstPartWork[ch].getNum(out n))
                {
                    msgBox.setErrMsg("不正なパン'p'が指定されています。", lineNumber);
                    n = 10;
                }
                //強制的にモノラルにする
                if (monoPart != null && monoPart.Contains(ym2612.Ch[0][ch].Name))
                {
                    n = 3;
                }
                n = checkRange(n, 1, 3);
                lstPartWork[ch].pan = n;
                outFmSetPanAMSFMS((byte)vch, n, 0, 0);
            }
            else if (ch < 13*2)
            {
                lstPartWork[ch].getNum(out n);
                msgBox.setWrnMsg("PSGパートでは、pコマンドは無視されます。", lineNumber);
            }
            else
            {
                int l;
                int r;
                if (!lstPartWork[ch].getNum(out l))
                {
                    msgBox.setErrMsg("不正なパン'p'が指定されています。", lineNumber);
                    l = 15;
                }
                if (lstPartWork[ch].getChar() != ',')
                {
                    msgBox.setErrMsg("不正なパン'p'が指定されています。", lineNumber);
                    l = 15;
                    r = 15;
                }
                lstPartWork[ch].incPos();
                if (!lstPartWork[ch].getNum(out r))
                {
                    msgBox.setErrMsg("不正なパン'p'が指定されています。", lineNumber);
                    r = 15;
                }
                l = checkRange(l, 0, 15);
                r = checkRange(r, 0, 15);
                lstPartWork[ch].pan = (r << 4) | l;
                setRf5c164CurrentChannel((byte)ch);
                setRf5c164Pan((byte)ch, lstPartWork[ch].pan);
            }

        }

        private void cmdLength(int ch)
        {
            int n;
            lstPartWork[ch].incPos();
            if (!lstPartWork[ch].getNum(out n))
            {
                msgBox.setErrMsg("不正な音長が指定されています。", lineNumber);
                n = 10;
            }
            n = checkRange(n, 1, 128);
            lstPartWork[ch].length = clockCount / n;
        }

        private void cmdClockLength(int ch)
        {
            int n;
            lstPartWork[ch].incPos();
            if (!lstPartWork[ch].getNum(out n))
            {
                msgBox.setErrMsg("不正な音長が指定されています。", lineNumber);
                n = 10;
            }
            n = checkRange(n, 1, 65535);
            lstPartWork[ch].length = n;
        }

        private void cmdVolumeDown(int ch)
        {
            int n;
            lstPartWork[ch].incPos();
            if (!lstPartWork[ch].getNum(out n))
            {
                msgBox.setErrMsg("不正な音量'('が指定されています。", lineNumber);
                n = 10;
            }
            n = checkRange(n, 1, (ch < 9) ? fmMaxVolume : ((ch < 13) ? psgMaxVolume : rf5c164MaxVolume));
            lstPartWork[ch].volume -= n;
            if (ch < 9*2)
            {
                lstPartWork[ch].volume = checkRange(lstPartWork[ch].volume, 0, fmMaxVolume);
            }
            else if (ch < 13*2)
            {
                lstPartWork[ch].volume = checkRange(lstPartWork[ch].volume, 0, psgMaxVolume);
            }
            else
            {
                lstPartWork[ch].volume = checkRange(lstPartWork[ch].volume, 0, rf5c164MaxVolume);
            }

        }

        private void cmdVolumeUp(int ch)
        {
            int n;
            lstPartWork[ch].incPos();
            if (!lstPartWork[ch].getNum(out n))
            {
                msgBox.setErrMsg("不正な音量')'が指定されています。", lineNumber);
                n = 10;
            }
            n = checkRange(n, 1, (ch < 9*2) ? fmMaxVolume : ((ch < 13*2) ? psgMaxVolume : rf5c164MaxVolume));
            lstPartWork[ch].volume += n;
            if (ch < 9*2)
            {
                lstPartWork[ch].volume = checkRange(lstPartWork[ch].volume, 0, fmMaxVolume);
            }
            else if (ch < 13*2)
            {
                lstPartWork[ch].volume = checkRange(lstPartWork[ch].volume, 0, psgMaxVolume);
            }
            else
            {
                lstPartWork[ch].volume = checkRange(lstPartWork[ch].volume, 0, rf5c164MaxVolume);
            }

        }

        private void cmdOctaveDown(int ch)
        {
            lstPartWork[ch].incPos();
            lstPartWork[ch].octaveNew--;
            lstPartWork[ch].octaveNew = checkRange(lstPartWork[ch].octaveNew, 1, 8);
        }

        private void cmdOctaveUp(int ch)
        {
            lstPartWork[ch].incPos();
            lstPartWork[ch].octaveNew++;
            lstPartWork[ch].octaveNew = checkRange(lstPartWork[ch].octaveNew, 1, 8);
        }

        private void cmdOctave(int ch)
        {
            int n;
            lstPartWork[ch].incPos();
            if (!lstPartWork[ch].getNum(out n))
            {
                msgBox.setErrMsg("不正なオクターブが指定されています。", lineNumber);
                n = 110;
            }
            n = checkRange(n, 1, 8);
            lstPartWork[ch].octaveNew = n;
        }

        private void cmdTempo(int ch)
        {
            int n;
            lstPartWork[ch].incPos();
            if (!lstPartWork[ch].getNum(out n))
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
            lstPartWork[ch].incPos();
            if (!lstPartWork[ch].getNum(out n))
            {
                msgBox.setErrMsg("不正な音量が指定されています。", lineNumber);
                n = 110;
            }
            if (ch < 9*2)
            {
                n = checkRange(n, 0, fmMaxVolume);
                if (lstPartWork[ch].volume != n)
                {
                    lstPartWork[ch].volume = n;
                }
            }
            else if (ch < 13*2)
            {
                n = checkRange(n, 0, psgMaxVolume);
                lstPartWork[ch].volume = n;
            }
            else
            {
                n = checkRange(n, 0, rf5c164MaxVolume);
                lstPartWork[ch].volume = n;
            }
        }

        private void cmdInstrument(int ch)
        {
            int n;
            lstPartWork[ch].incPos();
            if (ch < 9*2)
            {
                if (!lstPartWork[ch].getNum(out n))
                {
                    msgBox.setErrMsg("不正な音色番号が指定されています。", lineNumber);
                    n = 0;
                }
                n = checkRange(n, 0, 255);
                if (lstPartWork[ch].instrument != n)
                {
                    lstPartWork[ch].instrument = n;
                    if (ch == 2 || ch == 6 || ch == 7 || ch == 8)
                    {
                        lstPartWork[2].instrument = n;
                        lstPartWork[6].instrument = n;
                        lstPartWork[7].instrument = n;
                        lstPartWork[8].instrument = n;
                    }
                    if (ch == 11 || ch == 15 || ch == 16 || ch == 17)
                    {
                        lstPartWork[11].instrument = n;
                        lstPartWork[15].instrument = n;
                        lstPartWork[16].instrument = n;
                        lstPartWork[17].instrument = n;
                    }
                    if (!lstPartWork[ch].pcm)
                    {
                        outFmSetInstrument((byte)ch, n, lstPartWork[ch].volume);
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
            else if (ch < 13*2)
            {
                if (!lstPartWork[ch].getNum(out n))
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
                    if (lstPartWork[ch].envInstrument != n)
                    {
                        lstPartWork[ch].envInstrument = n;
                        lstPartWork[ch].envIndex = -1;
                        lstPartWork[ch].envCounter = -1;
                        for (int i = 0; i < instENV[n].Length; i++)
                        {
                            lstPartWork[ch].envelope[i] = instENV[n][i];
                        }
                    }
                }
            }
            else
            {
                if (lstPartWork[ch].getChar() != 'E')
                {
                    if (!lstPartWork[ch].getNum(out n))
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
                        lstPartWork[ch].instrument = n;
                        setRf5c164CurrentChannel((byte)ch);
                        setRf5c164SampleStartAddress((byte)ch, (int)instPCM[n].stAdr);
                        setRf5c164LoopAddress((byte)ch, (int)(instPCM[n].loopAdr));
                    }
                }
                else
                {
                    lstPartWork[ch].incPos();
                    if (!lstPartWork[ch].getNum(out n))
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
                        if (lstPartWork[ch].envInstrument != n)
                        {
                            lstPartWork[ch].envInstrument = n;
                            lstPartWork[ch].envIndex = -1;
                            lstPartWork[ch].envCounter = -1;
                            for (int i = 0; i < instENV[n].Length; i++)
                            {
                                lstPartWork[ch].envelope[i] = instENV[n][i];
                            }
                        }
                    }
                }
            }
        }

        private void cmdLfo(int ch)
        {

            lstPartWork[ch].incPos();
            char c = lstPartWork[ch].getChar();
            if (c < 'P' && c > 'S')
            {
                msgBox.setErrMsg("指定できるLFOのチャネルはP,Q,R,Sの4種類です。", lineNumber);
                return;
            }
            c -= 'P';

            lstPartWork[ch].incPos();
            char t = lstPartWork[ch].getChar();
            if (t != 'T' && t != 'V' && t != 'H')
            {
                msgBox.setErrMsg("指定できるLFOの種類はT,V,Hの3種類です。", lineNumber);
                return;
            }
            lstPartWork[ch].lfo[c].type = (t == 'T') ? eLfoType.Tremolo : ((t == 'V') ? eLfoType.Vibrato : eLfoType.Hardware);

            lstPartWork[ch].lfo[c].sw = false;
            lstPartWork[ch].lfo[c].isEnd = true;

            lstPartWork[ch].lfo[c].param = new List<int>();
            int n = -1;
            do
            {
                lstPartWork[ch].incPos();
                if (lstPartWork[ch].getNum(out n))
                {
                    lstPartWork[ch].lfo[c].param.Add(n);
                }
                else
                {
                    msgBox.setErrMsg("LFOの設定値に不正な値が指定されました。", lineNumber);
                    return;
                }

                while(lstPartWork[ch].getChar()=='\t' || lstPartWork[ch].getChar()==' ') { lstPartWork[ch].incPos(); }

            } while (lstPartWork[ch].getChar() == ',');
            if (lstPartWork[ch].lfo[c].type == eLfoType.Tremolo || lstPartWork[ch].lfo[c].type == eLfoType.Vibrato)
            {
                if (lstPartWork[ch].lfo[c].param.Count < 4)
                {
                    msgBox.setErrMsg("LFOの設定に必要なパラメータが足りません。", lineNumber);
                    return;
                }
                if (lstPartWork[ch].lfo[c].param.Count > 7)
                {
                    msgBox.setErrMsg("LFOの設定に可能なパラメータ数を超えて指定されました。", lineNumber);
                    return;
                }

                lstPartWork[ch].lfo[c].param[0] = checkRange(lstPartWork[ch].lfo[c].param[0], 0, (int)clockCount);
                lstPartWork[ch].lfo[c].param[1] = checkRange(lstPartWork[ch].lfo[c].param[1], 1, 255);
                lstPartWork[ch].lfo[c].param[2] = checkRange(lstPartWork[ch].lfo[c].param[2], -32768, 32787);
                lstPartWork[ch].lfo[c].param[3] = Math.Abs(checkRange(lstPartWork[ch].lfo[c].param[3], -32768, 32787));
                if (lstPartWork[ch].lfo[c].param.Count > 4)
                {
                    lstPartWork[ch].lfo[c].param[4] = checkRange(lstPartWork[ch].lfo[c].param[4], 0, 4);
                }
                else
                {
                    lstPartWork[ch].lfo[c].param.Add(0);
                }
                if (lstPartWork[ch].lfo[c].param.Count > 5)
                {
                    lstPartWork[ch].lfo[c].param[5] = checkRange(lstPartWork[ch].lfo[c].param[5], 0, 1);
                }
                else
                {
                    lstPartWork[ch].lfo[c].param.Add(1);
                }
                if (lstPartWork[ch].lfo[c].param.Count > 6)
                {
                    lstPartWork[ch].lfo[c].param[6] = checkRange(lstPartWork[ch].lfo[c].param[6], -32768, 32787);
                }
                else
                {
                    lstPartWork[ch].lfo[c].param.Add(0);
                }

            }
            else
            {
                if (lstPartWork[ch].lfo[c].param.Count < 4)
                {
                    msgBox.setErrMsg("LFOの設定に必要なパラメータが足りません。", lineNumber);
                    return;
                }
                if (lstPartWork[ch].lfo[c].param.Count > 5)
                {
                    msgBox.setErrMsg("LFOの設定に可能なパラメータ数を超えて指定されました。", lineNumber);
                    return;
                }

                lstPartWork[ch].lfo[c].param[0] = checkRange(lstPartWork[ch].lfo[c].param[0], 0, (int)clockCount);
                lstPartWork[ch].lfo[c].param[1] = checkRange(lstPartWork[ch].lfo[c].param[1], 0, 7);
                lstPartWork[ch].lfo[c].param[2] = checkRange(lstPartWork[ch].lfo[c].param[2], 0, 7);
                lstPartWork[ch].lfo[c].param[3] = checkRange(lstPartWork[ch].lfo[c].param[3], 0, 3);
                if (lstPartWork[ch].lfo[c].param.Count == 5)
                {
                    lstPartWork[ch].lfo[c].param[4] = checkRange(lstPartWork[ch].lfo[c].param[4], 0, 1);
                }
                else
                {
                    lstPartWork[ch].lfo[c].param.Add(1);
                }

            }
            //解析　ここまで

            lstPartWork[ch].lfo[c].sw = true;
            lstPartWork[ch].lfo[c].isEnd = false;
            lstPartWork[ch].lfo[c].value = (lstPartWork[ch].lfo[c].param[0] == 0) ? lstPartWork[ch].lfo[c].param[6] : 0;//ディレイ中は振幅補正は適用されない
            lstPartWork[ch].lfo[c].waitCounter = lstPartWork[ch].lfo[c].param[0];
            lstPartWork[ch].lfo[c].direction = lstPartWork[ch].lfo[c].param[2] < 0 ? -1 : 1;
        }

        private void cmdLfoSwitch(int ch)
        {

            lstPartWork[ch].incPos();
            char c = lstPartWork[ch].getChar();
            if (c < 'P' && c > 'S')
            {
                msgBox.setErrMsg("指定できるLFOのチャネルはP,Q,R,Sの4種類です。", lineNumber);
                return;
            }
            c -= 'P';

            int n = -1;
            lstPartWork[ch].incPos();
            if (!lstPartWork[ch].getNum(out n))
            {
                msgBox.setErrMsg("LFOの設定値に不正な値が指定されました。", lineNumber);
                return;
            }
            n = checkRange(n, 0, 2);

            //解析　ここまで

            lstPartWork[ch].lfo[c].sw = (n == 0) ? false : true;
            if (lstPartWork[ch].lfo[c].type == eLfoType.Hardware && lstPartWork[ch].lfo[c].param != null)
            {
                if (ch < 9*2)
                {
                    if (lstPartWork[ch].lfo[c].param[4] == 0)
                    {
                        outFmSetHardLfo(ch > 8, (n == 0) ? false : true, lstPartWork[ch].lfo[c].param[1]);
                    }
                    else
                    {
                        outFmSetHardLfo(ch > 8, false, lstPartWork[ch].lfo[c].param[1]);
                    }
                }
            }

        }

        private void cmdY(int ch)
        {
            int n = -1;
            byte adr = 0;
            byte dat = 0;
            lstPartWork[ch].incPos();
            if (lstPartWork[ch].getNum(out n))
            {
                adr = (byte)(n & 0xff);
            }
            lstPartWork[ch].incPos();
            if (lstPartWork[ch].getNum(out n))
            {
                dat = (byte)(n & 0xff);
            }

            if (ch < 9*2)
            {
                outFmAdrPort(ch>8,(ch > 3 && ch < 6) || (ch > 12 && ch < 15), adr, dat);
            }
            else if (ch < 13*2)
            {
                outPsgPort((ch - 9*2) > 3, dat);
            }
            else
            {
                outRf5c164Port(adr, dat);
            }
        }

        private void cmdNoise(int ch)
        {
            int n = -1;
            lstPartWork[ch].incPos();
            if (ch != 9 * 2 + 4 - 1 && ch != 9 * 2 + 4 + 4 - 1)
            {
                msgBox.setErrMsg("このチャンネルではwコマンドは使用できません。", lineNumber);
                return;
            }

            if (lstPartWork[ch].getNum(out n))
            {
                lstPartWork[ch].noise = checkRange(n, 0, 7);
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
            lstPartWork[ch].incPos();

            if (lstPartWork[ch].getNum(out n))
            {
                lstPartWork[ch].keyShift = checkRange(n,-128,128);
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
            if ((ym2612.use[0] || ym2612.use[1]) && pcmDataYM2612 != null && pcmDataYM2612.Length > 0)
            {
                foreach (byte b in pcmDataYM2612)
                {
                    dat.Add(b);
                }
            }

            //PCM Data block
            if ((rf5c164.use[0] || rf5c164.use[1]) && pcmDataRf5c164 != null && pcmDataRf5c164.Length > 0)
            {
                foreach (byte b in pcmDataRf5c164)
                {
                    dat.Add(b);
                }
            }


            if (ym2612.use[0])
            {
                outFmSetHardLfo(false, false, 0);
                outFmSetCh3SpecialMode(false, false);
                outFmSetCh6PCMMode(false, false);

                outFmAllKeyOff(false);

                for (int ch = 0; ch < ym2612.ChMax; ch++)
                {
                    if (ym2612.Ch[0][ch].Type == enmChannelType.FMOPN || ym2612.Ch[0][ch].Type == enmChannelType.FMPCM)
                    {
                        if (!lstPartWork[ym2612.partStartCh[0] + ch].dataEnd) outFmSetPanAMSFMS((byte)(ym2612.partStartCh[0] + ch), 3, 0, 0);
                    }
                }
            }

            if (ym2612.use[1])
            {
                outFmSetHardLfo(true, false, 0);
                outFmSetCh3SpecialMode(true, false);
                outFmSetCh6PCMMode(true, false);

                outFmAllKeyOff(true);

                for (int ch = 0; ch < ym2612.ChMax; ch++)
                {
                    if (ym2612.Ch[1][ch].Type == enmChannelType.FMOPN || ym2612.Ch[1][ch].Type == enmChannelType.FMPCM)
                    {
                        if (!lstPartWork[ym2612.partStartCh[1] + ch].dataEnd) outFmSetPanAMSFMS((byte)(ym2612.partStartCh[1] + ch), 3, 0, 0);
                    }
                }

                dat[0x2f] |= 0x40;
            }

            if (sn76489.use[0])
            {
            }

            if (sn76489.use[1])
            {
                dat[0x0f] |= 0x40;
            }

            if (rf5c164.use[0])
            {
                for (int ch = 0; ch < rf5c164.ChMax; ch++)
                {
                    byte i = (byte)(rf5c164.partStartCh[0] + ch);

                    setRf5c164CurrentChannel(i);
                    setRf5c164SampleStartAddress(i, 0);
                    setRf5c164LoopAddress(i, 0);
                    setRf5c164AddressIncrement(i, 0x400);
                    setRf5c164Pan(i, 0xff);
                    setRf5c164Envelope(i, 0xff);
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
            for (int i = 0; i < lstPartWork.Count; i++)
            {
                switch (lstPartWork[i].chip.Type)
                {
                    case enmChipType.YM2612:
                        useYM2612 += lstPartWork[i].clockCounter;
                        break;
                    case enmChipType.SN76489:
                        useSN76489 += lstPartWork[i].clockCounter;
                        break;
                    case enmChipType.RF5C164:
                        useRf5c164 += lstPartWork[i].clockCounter;
                        break;
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

        private void setEnvelopeAtKeyOn(int ch)
        {
            if (!lstPartWork[ch].envelopeMode)
            {
                lstPartWork[ch].envVolume = 0;
                lstPartWork[ch].envIndex = -1;
                return;
            }

            lstPartWork[ch].envIndex = 0;
            lstPartWork[ch].envCounter = 0;
            int maxValue = (lstPartWork[ch].envelope[8] == 2) ? 255 : 15;

            while (lstPartWork[ch].envCounter == 0 && lstPartWork[ch].envIndex != -1)
            {
                switch (lstPartWork[ch].envIndex)
                {
                    case 0: // AR phase
                        lstPartWork[ch].envCounter = lstPartWork[ch].envelope[2];
                        if (lstPartWork[ch].envelope[2] > 0 && lstPartWork[ch].envelope[1] < maxValue)
                        {
                            lstPartWork[ch].envVolume = lstPartWork[ch].envelope[1];
                        }
                        else
                        {
                            lstPartWork[ch].envVolume = maxValue;
                            lstPartWork[ch].envIndex++;
                        }
                        break;
                    case 1: // DR phase
                        lstPartWork[ch].envCounter = lstPartWork[ch].envelope[3];
                        if (lstPartWork[ch].envelope[3] > 0 && lstPartWork[ch].envelope[4] < maxValue)
                        {
                            lstPartWork[ch].envVolume = maxValue;
                        }
                        else
                        {
                            lstPartWork[ch].envVolume = lstPartWork[ch].envelope[4];
                            lstPartWork[ch].envIndex++;
                        }
                        break;
                    case 2: // SR phase
                        lstPartWork[ch].envCounter = lstPartWork[ch].envelope[5];
                        if (lstPartWork[ch].envelope[5] > 0 && lstPartWork[ch].envelope[4] != 0)
                        {
                            lstPartWork[ch].envVolume = lstPartWork[ch].envelope[4];
                        }
                        else
                        {
                            lstPartWork[ch].envVolume = 0;
                            lstPartWork[ch].envIndex = -1;
                        }
                        break;
                }
            }
        }

        private void setLfoAtKeyOn(int ch)
        {
            for(int lfo = 0; lfo < 4; lfo++)
            {
                clsLfo pl = lstPartWork[ch].lfo[lfo];
                if (!pl.sw)
                {
                    continue;
                }
                if (pl.type == eLfoType.Hardware)
                {
                    if (ch < 9*2)
                    {
                        if (pl.param[4] == 1)
                        {
                            outFmSetHardLfo((ch > 8), false, pl.param[1]);
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
                    if (ch < 9*2)
                    {
                        setFmFNum(ch);
                    }
                }
                if (pl.type == eLfoType.Tremolo)
                {
                    if (ch < 9*2)
                    {
                        lstPartWork[ch].beforeVolume = -1;
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
            int f = getFmFNum(lstPartWork[ch].octaveNow, lstPartWork[ch].noteCmd, lstPartWork[ch].shift + lstPartWork[ch].keyShift);//
            if (lstPartWork[ch].bendWaitCounter !=-1)
            {
                f = lstPartWork[ch].bendFnum;
            }
            int o = (f & 0xf000) / 0x1000;
            f &= 0xfff;

            f = f + lstPartWork[ch].detune;
            for (int lfo = 0; lfo < 4; lfo++)
            {
                if (!lstPartWork[ch].lfo[lfo].sw)
                {
                    continue;
                }
                if (lstPartWork[ch].lfo[lfo].type != eLfoType.Vibrato)
                {
                    continue;
                }
                f += lstPartWork[ch].lfo[lfo].value + lstPartWork[ch].lfo[lfo].param[6];
            }
//            f = checkRange(f, 0, 0x7ff);
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

        private void setFmVolume(int ch)
        {
            int vol = lstPartWork[ch].volume;

            for (int lfo = 0; lfo < 4; lfo++)
            {
                if (!lstPartWork[ch].lfo[lfo].sw)
                {
                    continue;
                }
                if (lstPartWork[ch].lfo[lfo].type != eLfoType.Tremolo)
                {
                    continue;
                }
                vol += lstPartWork[ch].lfo[lfo].value + lstPartWork[ch].lfo[lfo].param[6];
            }

            if (lstPartWork[ch].beforeVolume != vol)
            {
                if (instFM.ContainsKey(lstPartWork[ch].instrument))
                {
                    outFmSetVolume((byte)ch, vol, lstPartWork[ch].instrument);
                    lstPartWork[ch].beforeVolume = vol;
                }
            }
        }

        private void setRf5c164Volume(int ch)
        {
            int vol = lstPartWork[ch].volume;

            if (lstPartWork[ch].envelopeMode)
            {
                vol = 0;
                if (lstPartWork[ch].envIndex != -1)
                {
                    vol = lstPartWork[ch].envVolume - (255 - lstPartWork[ch].volume);
                }
            }

            for (int lfo = 0; lfo < 4; lfo++)
            {
                if (!lstPartWork[ch].lfo[lfo].sw)
                {
                    continue;
                }
                if (lstPartWork[ch].lfo[lfo].type != eLfoType.Tremolo)
                {
                    continue;
                }
                vol += lstPartWork[ch].lfo[lfo].value + lstPartWork[ch].lfo[lfo].param[6];
            }

            vol = checkRange(vol, 0, 255);

            if (lstPartWork[ch].beforeVolume != vol)
            {
                    setRf5c164Envelope((byte)ch, vol);
                    lstPartWork[ch].beforeVolume = vol;
            }
        }


        private void setPsgFNum(int ch)
        {
            partWork cpw = lstPartWork[ch];

            if (cpw.chip.Ch[cpw.isSecondary ? 1 : 0][ch - sn76489.partStartCh[cpw.isSecondary ? 1 : 0]].Type != enmChannelType.DCSGNOISE)// ch!=9*2+4*1 -1 && ch!=9*2+4*2-1)
            {
                int f = getPsgFNum(cpw.octaveNow, cpw.noteCmd, cpw.shift + cpw.keyShift);//
                if (cpw.bendWaitCounter != -1)
                {
                    f = cpw.bendFnum;
                }
                f = f + cpw.detune;
                for (int lfo = 0; lfo < 4; lfo++)
                {
                    if (!cpw.lfo[lfo].sw)
                    {
                        continue;
                    }
                    if (cpw.lfo[lfo].type != eLfoType.Vibrato)
                    {
                        continue;
                    }
                    f += cpw.lfo[lfo].value + cpw.lfo[lfo].param[6];
                }

                f = checkRange(f, 0, 0x3ff);
                if (cpw.freq == f) return;

                cpw.freq = f;

                byte pch = (byte)(ch - sn76489.partStartCh[cpw.isSecondary ? 1 : 0]);
                byte data = 0;

                data = (byte)(0x80 + (pch << 5) + (f & 0xf));
                outPsgPort(cpw.isSecondary, data);

                data = (byte)((f & 0x3f0) >> 4);
                outPsgPort(cpw.isSecondary, data);
            }
            else
            {
                int f = cpw.noise;
                byte data = (byte)(0xe0 + (f & 0x7));
                cpw.freq = 0x40 + (f & 7);
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
            partWork cpw = lstPartWork[ch];
            byte pch = (byte)(ch - sn76489.partStartCh[cpw.isSecondary ? 1 : 0]);
            byte data = 0;

            int vol = cpw.volume;

            if (cpw.envelopeMode)
            {
                vol = 0;
                if (cpw.envIndex != -1)
                {
                    vol = cpw.envVolume - (15 - cpw.volume);
                }
            }

            for (int lfo = 0; lfo < 4; lfo++)
            {
                if (!cpw.lfo[lfo].sw) continue;
                if (cpw.lfo[lfo].type != eLfoType.Tremolo) continue;

                vol += cpw.lfo[lfo].value + cpw.lfo[lfo].param[6];
            }

            vol = checkRange(vol, 0, 15);

            if (cpw.beforeVolume != vol)
            {
                data = (byte)(0x80 + (pch << 5) + 0x10 + (15 - vol));
                outPsgPort(cpw.isSecondary, data);
                cpw.beforeVolume = vol;
            }
        }


        private void setRf5c164FNum(int ch)
        {
            int f = getRf5c164PcmNote(lstPartWork[ch].octaveNow, lstPartWork[ch].noteCmd, lstPartWork[ch].keyShift + lstPartWork[ch].shift);//
            
            if (lstPartWork[ch].bendWaitCounter != -1)
            {
                f = lstPartWork[ch].bendFnum;
            }
            f = f + lstPartWork[ch].detune;
            for (int lfo = 0; lfo < 4; lfo++)
            {
                if (!lstPartWork[ch].lfo[lfo].sw)
                {
                    continue;
                }
                if (lstPartWork[ch].lfo[lfo].type != eLfoType.Vibrato)
                {
                    continue;
                }
                f += lstPartWork[ch].lfo[lfo].value + lstPartWork[ch].lfo[lfo].param[6];
            }

            f = checkRange(f, 0, 0xffff);
            lstPartWork[ch].freq = f;

            byte pch = (byte)((ch - 13*2) & 7);

            setRf5c164CurrentChannel((byte)ch);

            //Address increment 再生スピードをセット
            setRf5c164AddressIncrement((byte)ch, f);

            //Envelope 音量をセット
            //setRf5c164Envelope((byte)ch, pw[ch].volume);

        }

        private void setRf5c164Envelope(byte ch, int volume)
        {
            if (lstPartWork[ch].rf5c164Envelope != volume)
            {
                setRf5c164CurrentChannel(ch);
                byte data = (byte)(volume & 0xff);
                outRf5c164Port(0x0, data);
                lstPartWork[ch].rf5c164Envelope = volume;
            }
        }

        private void setRf5c164Pan(byte ch, int pan)
        {
            if (lstPartWork[ch].rf5c164Pan != pan)
            {
                setRf5c164CurrentChannel(ch);
                byte data = (byte)(pan & 0xff);
                outRf5c164Port(0x1, data);
                lstPartWork[ch].rf5c164Pan = pan;
            }
        }

        private void setRf5c164CurrentChannel(byte ch)
        {
            byte pch = (byte)((ch - 13*2) & 0x7);
            if (rf5c164CurrentChannel != pch)
            {
                byte data = (byte)(0xc0 + pch);
                outRf5c164Port(0x7, data);
                rf5c164CurrentChannel = pch;
            }
        }

        private void setRf5c164AddressIncrement(byte ch,int f)
        {
            if (lstPartWork[ch].rf5c164AddressIncrement != f)
            {
                byte data = (byte)(f & 0xff);
                outRf5c164Port(0x2, data);
                data = (byte)((f >> 8) & 0xff);
                outRf5c164Port(0x3, data);
                lstPartWork[ch].rf5c164AddressIncrement = f;
            }
        }

        private void setRf5c164SampleStartAddress(byte ch, int adr)
        {
            if (lstPartWork[ch].rf5c164SampleStartAddress != adr)
            {
                byte data = (byte)((adr >> 8) & 0xff);
                outRf5c164Port(0x6, data);
                lstPartWork[ch].rf5c164SampleStartAddress = adr;
            }
        }

        private void setRf5c164LoopAddress(byte ch, int adr)
        {
            if (lstPartWork[ch].rf5c164LoopAddress != adr)
            {
                byte data = (byte)((adr >> 8) & 0xff);
                outRf5c164Port(0x5, data);
                data = (byte)(adr & 0xff);
                outRf5c164Port(0x4, data);
                lstPartWork[ch].rf5c164LoopAddress = adr;
            }
        }

        private void outRf5c164KeyOn(byte ch)
        {
            byte pch = (byte)((ch - 13*2) & 0x7);
            rf5c164KeyOn |= (byte)(1 << pch);
            byte data = (byte)(~rf5c164KeyOn);
            outRf5c164Port(0x8, data);
        }

        private void outRf5c164KeyOff(byte ch)
        {
            byte pch = (byte)((ch - 13*2) & 0x7);
            rf5c164KeyOn &= (byte)(~(1 << pch));
            byte data = (byte)(~rf5c164KeyOn);
            outRf5c164Port(0x8, data);
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

        private void outFmKeyOn(byte ch)
        {
            if (!lstPartWork[ch].pcm)
            {
                if (lstPartWork[2].Ch3SpecialMode && (ch == 2 || (ch >= 6 && ch < 9)))
                {
                    lstPartWork[ch].Ch3SpecialModeKeyOn = true;

                    int slot = (lstPartWork[2].Ch3SpecialModeKeyOn ? lstPartWork[2].slots : 0x0)
                        | (lstPartWork[6].Ch3SpecialModeKeyOn ? lstPartWork[6].slots : 0x0)
                        | (lstPartWork[7].Ch3SpecialModeKeyOn ? lstPartWork[7].slots : 0x0)
                        | (lstPartWork[8].Ch3SpecialModeKeyOn ? lstPartWork[8].slots : 0x0);

                    outFmAdrPort(false, 0x52, 0x28, (byte)((slot << 4) + 2));
                }
                else if (lstPartWork[11].Ch3SpecialMode && (ch == 11 || (ch >= 15 && ch < 18)))
                {
                    lstPartWork[ch].Ch3SpecialModeKeyOn = true;

                    int slot = (lstPartWork[11].Ch3SpecialModeKeyOn ? lstPartWork[11].slots : 0x0)
                        | (lstPartWork[15].Ch3SpecialModeKeyOn ? lstPartWork[15].slots : 0x0)
                        | (lstPartWork[16].Ch3SpecialModeKeyOn ? lstPartWork[16].slots : 0x0)
                        | (lstPartWork[17].Ch3SpecialModeKeyOn ? lstPartWork[17].slots : 0x0);

                    outFmAdrPort(true, 0x52, 0x28, (byte)((slot << 4) + 2));
                }
                else
                {
                    if (ch >= 0 && ch < 6)
                    {
                        byte vch = (byte)((ch > 2) ? ch + 1 : ch);
                        //key on
                        outFmAdrPort(false, 0x52, 0x28, (byte)((lstPartWork[ch].slots << 4) + (vch & 7)));
                    }
                    else if (ch >= 9 && ch < 15)
                    {
                        byte vch = (byte)((ch > 11) ? ch + 1 - 9 : ch - 9);
                        //key on
                        outFmAdrPort(true, 0x52, 0x28, (byte)((lstPartWork[ch].slots << 4) + (vch & 7)));
                    }
                }

                return;
            }

            float m = pcmMTbl[lstPartWork[ch].pcmNote] * (float)Math.Pow(2, (lstPartWork[ch].pcmOctave - 4));
            pcmBaseFreqPerFreq = vgmSamplesPerSecond / ((float)instPCM[lstPartWork[ch].instrument].freq * m);
            pcmFreqCountBuffer = 0.0f;
            long p = instPCM[lstPartWork[ch].instrument].stAdr;
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
                long s = instPCM[lstPartWork[ch].instrument].size;
                long f = instPCM[lstPartWork[ch].instrument].freq;
                long w = 0;
                if (lstPartWork[ch].gatetimePmode)
                {
                    w = lstPartWork[ch].waitCounter * lstPartWork[ch].gatetime / 8L;
                }
                else
                {
                    w = lstPartWork[ch].waitCounter - lstPartWork[ch].gatetime;
                }
                if (w < 1) w = 1;
                s = Math.Min(s, w * (long)samplesPerClock * f / 44100);

                if (!ym2612SetupStream)
                {
                    // setup stream control
                    dat.Add(0x90);
                    dat.Add(0x00);
                    dat.Add(0x02);
                    dat.Add(0x00);
                    dat.Add(0x2a);

                    // set stream data
                    dat.Add(0x91);
                    dat.Add(0x00);
                    dat.Add(0x00);
                    dat.Add(0x01);
                    dat.Add(0x00);

                    ym2612SetupStream = true;
                }

                if (ym2612StreamFreq != f)
                {
                    //Set Stream Frequency
                    dat.Add(0x92);
                    dat.Add(0x00);

                    dat.Add((byte)(f & 0xff));
                    dat.Add((byte)((f & 0xff00) / 0x100));
                    dat.Add((byte)((f & 0xff0000) / 0x10000));
                    dat.Add((byte)((f & 0xff000000) / 0x10000));

                    ym2612StreamFreq = f;
                }

                //Start Stream
                dat.Add(0x93);
                dat.Add(0x00);

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

        private void outFmKeyOff(byte ch)
        {
            if (!lstPartWork[ch].pcm)
            {
                if (lstPartWork[2].Ch3SpecialMode && (ch == 2 || (ch >= 6 && ch < 9)))
                {
                    lstPartWork[ch].Ch3SpecialModeKeyOn = false;

                    int slot = (lstPartWork[2].Ch3SpecialModeKeyOn ? lstPartWork[2].slots : 0x0)
                        | (lstPartWork[6].Ch3SpecialModeKeyOn ? lstPartWork[6].slots : 0x0)
                        | (lstPartWork[7].Ch3SpecialModeKeyOn ? lstPartWork[7].slots : 0x0)
                        | (lstPartWork[8].Ch3SpecialModeKeyOn ? lstPartWork[8].slots : 0x0);

                    outFmAdrPort(false,0x52, 0x28, (byte)((slot << 4) + 2));
                }
                else if (lstPartWork[11].Ch3SpecialMode && (ch == 11 || (ch >= 15 && ch < 18)))
                {
                    lstPartWork[ch].Ch3SpecialModeKeyOn = false;

                    int slot = (lstPartWork[11].Ch3SpecialModeKeyOn ? lstPartWork[11].slots : 0x0)
                        | (lstPartWork[15].Ch3SpecialModeKeyOn ? lstPartWork[15].slots : 0x0)
                        | (lstPartWork[16].Ch3SpecialModeKeyOn ? lstPartWork[16].slots : 0x0)
                        | (lstPartWork[17].Ch3SpecialModeKeyOn ? lstPartWork[17].slots : 0x0);

                    outFmAdrPort(true, 0x52, 0x28, (byte)((slot << 4) + 2));
                }
                else
                {
                    if (ch >= 0 && ch < 6)
                    {
                        byte vch = (byte)((ch > 2) ? ch + 1 : ch);
                        //key off
                        outFmAdrPort(false,0x52, 0x28, (byte)(0x00 + (vch & 7)));
                    }
                    else if (ch >= 9 && ch < 15)
                    {
                        byte vch = (byte)((ch > 11) ? ch + 1-9 : ch-9);
                        //key off
                        outFmAdrPort(true, 0x52, 0x28, (byte)(0x00 + (vch & 7)));
                    }
                }

                return;
            }

            waitKeyOnPcmCounter = -1;
        }

        private void outFmAllKeyOff(bool isSecondary)
        {
            for (int ch = 0; ch < ym2612.ChMax; ch++)
            {
                if (ym2612.Ch[isSecondary ? 1 : 0][ch].Type == enmChannelType.FMOPN || ym2612.Ch[isSecondary ? 1 : 0][ch].Type == enmChannelType.FMPCM)
                {
                    if (!lstPartWork[ym2612.partStartCh[isSecondary ? 1 : 0] + ch].dataEnd)
                    {
                        byte i = (byte)(ym2612.partStartCh[isSecondary ? 1 : 0] + ch);
                        outFmKeyOff(i);
                        outFmSetTl(i, 0, 127);
                        outFmSetTl(i, 1, 127);
                        outFmSetTl(i, 2, 127);
                        outFmSetTl(i, 3, 127);
                    }
                }
            }
        }

        private void outFmSetFnum(byte ch, int octave, int num)
        {
            int freq;
            freq = ((num & 0x700) >> 8) + (((octave - 1) & 0x7) << 3);
            freq = (freq << 8) + (num & 0xff);

            if (freq == lstPartWork[ch].freq) return;

            lstPartWork[ch].freq = freq;
            
            if (lstPartWork[2].Ch3SpecialMode && (ch == 2 || ch == 6 || ch == 7 || ch == 8))
            {
                byte port = (byte)0x52;

                if ((lstPartWork[ch].slots & 8) != 0)
                {
                    int f = lstPartWork[ch].freq + lstPartWork[ch].slotDetune[3];
                    outFmAdrPort(false, port, (byte)0xa6, (byte)((f & 0xff00) >> 8));
                    outFmAdrPort(false, port, (byte)0xa2, (byte)(f & 0xff));
                }
                if ((lstPartWork[ch].slots & 4) != 0)
                {
                    int f = lstPartWork[ch].freq + lstPartWork[ch].slotDetune[2];
                    outFmAdrPort(false, port, (byte)0xac, (byte)((f & 0xff00) >> 8));
                    outFmAdrPort(false, port, (byte)0xa8, (byte)(f & 0xff));
                }
                if ((lstPartWork[ch].slots & 1) != 0)
                {
                    int f = lstPartWork[ch].freq + lstPartWork[ch].slotDetune[0];
                    outFmAdrPort(false, port, (byte)0xad, (byte)((f & 0xff00) >> 8));
                    outFmAdrPort(false, port, (byte)0xa9, (byte)(f & 0xff));
                }
                if ((lstPartWork[ch].slots & 2) != 0)
                {
                    int f = lstPartWork[ch].freq + lstPartWork[ch].slotDetune[1];
                    outFmAdrPort(false, port, (byte)0xae, (byte)((f & 0xff00) >> 8));
                    outFmAdrPort(false, port, (byte)0xaa, (byte)(f & 0xff));
                }
            }
            else if (lstPartWork[11].Ch3SpecialMode && (ch == 11 || ch == 15 || ch == 16 || ch == 17))
            {
                byte port = (byte)0x52;

                if ((lstPartWork[ch].slots & 8) != 0)
                {
                    int f = lstPartWork[ch].freq + lstPartWork[ch].slotDetune[3];
                    outFmAdrPort(true, port, (byte)0xa6, (byte)((f & 0xff00) >> 8));
                    outFmAdrPort(true, port, (byte)0xa2, (byte)(f & 0xff));
                }
                if ((lstPartWork[ch].slots & 4) != 0)
                {
                    int f = lstPartWork[ch].freq + lstPartWork[ch].slotDetune[2];
                    outFmAdrPort(true, port, (byte)0xac, (byte)((f & 0xff00) >> 8));
                    outFmAdrPort(true, port, (byte)0xa8, (byte)(f & 0xff));
                }
                if ((lstPartWork[ch].slots & 1) != 0)
                {
                    int f = lstPartWork[ch].freq + lstPartWork[ch].slotDetune[0];
                    outFmAdrPort(true, port, (byte)0xad, (byte)((f & 0xff00) >> 8));
                    outFmAdrPort(true, port, (byte)0xa9, (byte)(f & 0xff));
                }
                if ((lstPartWork[ch].slots & 2) != 0)
                {
                    int f = lstPartWork[ch].freq + lstPartWork[ch].slotDetune[1];
                    outFmAdrPort(true, port, (byte)0xae, (byte)((f & 0xff00) >> 8));
                    outFmAdrPort(true, port, (byte)0xaa, (byte)(f & 0xff));
                }
            }
            else
            {
                if ((ch >= 6 && ch < 9) || (ch >= 15 && ch < 18))
                {
                    return;
                }
                if (ch < 6)
                {
                    byte port = (byte)(0x52 + (ch > 2 ? 1 : 0));
                    byte vch = (byte)(ch > 2 ? ch - 3 : ch);

                    outFmAdrPort(false, port, (byte)(0xa4 + vch), (byte)((lstPartWork[ch].freq & 0xff00) >> 8));
                    outFmAdrPort(false, port, (byte)(0xa0 + vch), (byte)(lstPartWork[ch].freq & 0xff));
                }
                else
                {
                    byte port = (byte)(0x52 + (ch > 11 ? 1 : 0));
                    byte vch = (byte)(ch > 11 ? ch - 3 - 9 : ch - 9);

                    outFmAdrPort(true, port, (byte)(0xa4 + vch), (byte)((lstPartWork[ch].freq & 0xff00) >> 8));
                    outFmAdrPort(true, port, (byte)(0xa0 + vch), (byte)(lstPartWork[ch].freq & 0xff));
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

        private void outFmSetInstrument(byte ch, int n, int vol)
        {

            if (!instFM.ContainsKey(n))
            {
                msgBox.setWrnMsg(string.Format("未定義の音色(@{0})を指定しています。", n), lineNumber);
                return;
            }

            if ((ch >= 6 && ch < 9) || (ch >= 15 && ch < 18))
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
            if (lstPartWork[2].Ch3SpecialMode && ch >= 6 && ch < 9) { vch = 2; }
            if (lstPartWork[11].Ch3SpecialMode && ch >= 6 + 9 && ch < 9 + 9) { vch = 11; }

            if ((lstPartWork[ch].slots & 1) != 0) outFmSetTl(vch, 0, ope[0]);
            if ((lstPartWork[ch].slots & 2) != 0) outFmSetTl(vch, 1, ope[1]);
            if ((lstPartWork[ch].slots & 4) != 0) outFmSetTl(vch, 2, ope[2]);
            if ((lstPartWork[ch].slots & 8) != 0) outFmSetTl(vch, 3, ope[3]);
        }

        private void outFmSetDtMl(byte ch, int ope, int dt, int ml)
        {
            int vch = (ch >= 9) ? (ch - 9) : ch;
            byte port = (byte)(0x52 + (vch > 2 ? 1 : 0));
            vch = (byte)(vch > 2 ? vch - 3 : vch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            dt &= 7;
            ml &= 15;

            outFmAdrPort((ch >= 9), port, (byte)(0x30 + vch + ope * 4), (byte)((dt << 4) + ml));
        }

        private void outFmSetTl(byte ch, int ope, int tl)
        {
            int vch = (ch >= 9) ? (ch - 9) : ch;
            byte port = (byte)(0x52 + (vch > 2 ? 1 : 0));
            vch = (byte)(vch > 2 ? vch - 3 : vch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            tl &= 0x7f;

            outFmAdrPort((ch >= 9), port,(byte)(0x40 + vch + ope * 4),(byte)tl);
        }

        private void outFmSetKsAr(byte ch, int ope, int ks, int ar)
        {
            int vch = (ch >= 9) ? (ch - 9) : ch;
            byte port = (byte)(0x52 + (vch > 2 ? 1 : 0));
            vch = (byte)(vch > 2 ? vch - 3 : vch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            ks &= 3;
            ar &= 31;

            outFmAdrPort((ch >= 9), port, (byte)(0x50 + vch + ope * 4), (byte)((ks << 6) + ar));
        }

        private void outFmSetAmDr(byte ch, int ope, int am,int dr)
        {
            int vch = (ch >= 9) ? (ch - 9) : ch;
            byte port = (byte)(0x52 + (vch > 2 ? 1 : 0));
            vch = (byte)(vch > 2 ? vch - 3 : vch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            am &= 1;
            dr &= 31;

            outFmAdrPort((ch >= 9),port, (byte)(0x60 + vch + ope * 4), (byte)((am << 7) + dr));
        }

        private void outFmSetSr(byte ch, int ope, int sr)
        {
            int vch = (ch >= 9) ? (ch - 9) : ch;
            byte port = (byte)(0x52 + (vch > 2 ? 1 : 0));
            vch = (byte)(vch > 2 ? vch - 3 : vch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            sr &= 31;

            dat.Add((byte)((port & 0xf) + (ch >= 9 ? 0xA0 : 0x50)));
            dat.Add((byte)(0x70 + vch + ope * 4));
            dat.Add((byte)sr);
        }

        private void outFmSetSlRr(byte ch, int ope, int sl, int rr)
        {
            int vch = (ch >= 9) ? (ch - 9) : ch;
            byte port = (byte)(0x52 + (vch > 2 ? 1 : 0));
            vch = (byte)(vch > 2 ? vch - 3 : vch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            sl &= 15;
            rr &= 15;

            dat.Add((byte)((port & 0xf) + (ch >= 9 ? 0xA0 : 0x50)));
            dat.Add((byte)(0x80 + vch + ope * 4));
            dat.Add((byte)((sl << 4) + rr));
        }

        private void outFmSetSSGEG(byte ch, int ope, int n)
        {
            int vch = (ch >= 9) ? (ch - 9) : ch;
            byte port = (byte)(0x52 + (vch > 2 ? 1 : 0));
            vch = (byte)(vch > 2 ? vch - 3 : vch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            n &= 15;

            dat.Add((byte)((port & 0xf) + (ch >= 9 ? 0xA0 : 0x50)));
            dat.Add((byte)(0x90 + vch + ope * 4));
            dat.Add((byte)n);
        }

        private void outFmSetFeedbackAlgorithm(byte ch, int fb, int alg)
        {
            int vch = (ch >= 9) ? (ch - 9) : ch;
            byte port = (byte)(0x52 + (vch > 2 ? 1 : 0));
            vch = (byte)(vch > 2 ? vch - 3 : vch);

            fb &= 7;
            alg &= 7;

            dat.Add((byte)((port & 0xf) + (ch >= 9 ? 0xA0 : 0x50)));
            dat.Add((byte)(0xb0 + vch));
            dat.Add((byte)((fb << 3) + alg));
        }

        private void outFmSetPanAMSFMS(byte ch, int pan, int ams, int fms)
        {
            int vch = (ch >= 9) ? (ch - 9) : ch;
            byte port = (byte)(0x52 + (vch > 2 ? 1 : 0));
            vch = (byte)(vch > 2 ? vch - 3 : vch);

            pan = pan & 3;
            ams = ams & 3;
            fms = fms & 3;

            dat.Add((byte)((port & 0xf) + (ch >= 9 ? 0xA0 : 0x50)));
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

        private void _outPsgSetFnum(byte ch, int freq)
        {
            lstPartWork[ch].freq=freq;
        }

        //long psgc = 0;
        private void outPsgKeyOn(byte ch)
        {
            byte pch = (byte)((ch - 9 * 2) & 3);
            byte data = 0;


            //if (ch == 9) Console.Write("[{0:x}", pw[ch].freq);
            data = (byte)(0x80 + (pch << 5) + 0x00 + (lstPartWork[ch].freq & 0xf));
            outPsgPort((ch - 9 * 2) > 3, data);
            //if (ch == 9) Console.Write(":{0:x}", data);

            if (ch != 3 + 4 * 0 + 9 * 2 && ch != 3 + 4 * 1 + 9 * 2)
            {
                data = (byte)((lstPartWork[ch].freq & 0x3f0) >> 4);
                outPsgPort((ch - 9 * 2) > 3, data);
                //if (ch == 9) Console.Write(":{0:x}", data);
            }

            int vol = lstPartWork[ch].volume;
            if (lstPartWork[ch].envelopeMode)
            {
                vol = 0;
                if (lstPartWork[ch].envIndex != -1)
                {
                    vol = lstPartWork[ch].envVolume - (15 - lstPartWork[ch].volume);
                }
            }
            if (vol > 15) vol = 15;
            if (vol < 0) vol = 0;
            data = (byte)(0x80 + (pch << 5) + 0x10 + (15 - vol));
            //if (ch == 9) Console.WriteLine(":{0:x}:{1:x}]:{2}", vol, data, psgc++);
            outPsgPort((ch - 9 * 2) > 3, data);

        }

        private void outPsgKeyOff(byte ch)
        {

            //Console.Write("[KeyOff]");
            byte pch = (byte)((ch - 9*2) & 3);
            int val = 15;

            byte data = (byte)(0x80 + (pch << 5) + 0x10 + (val & 0xf));
            outPsgPort((ch - 9 * 2) > 3, data);

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
