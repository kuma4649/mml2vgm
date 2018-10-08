using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class ClsVgm
    {

        public YM2151[] ym2151 = null;
        public YM2203[] ym2203 = null;
        public YM2608[] ym2608 = null;
        public YM2610B[] ym2610b = null;
        public YM2612[] ym2612 = null;
        public SN76489[] sn76489 = null;
        public RF5C164[] rf5c164 = null;
        public segaPcm[] segapcm = null;
        public HuC6280[] huc6280 = null;
        public YM2612X[] ym2612x = null;
        public YM2413[] ym2413 = null;
        public C140[] c140 = null;
        public AY8910[] ay8910 = null;

        public Dictionary<enmChipType, ClsChip[]> chips;

        public int lineNumber = 0;

        public Dictionary<int, byte[]> instFM = new Dictionary<int, byte[]>();
        public Dictionary<int, int[]> instENV = new Dictionary<int, int[]>();
        public Dictionary<int, clsPcm> instPCM = new Dictionary<int, clsPcm>();
        public Dictionary<int, clsToneDoubler> instToneDoubler = new Dictionary<int, clsToneDoubler>();
        public Dictionary<int, byte[]> instWF = new Dictionary<int, byte[]>();

        public Dictionary<string, List<Line>> partData = new Dictionary<string, List<Line>>();
        public Dictionary<string, Line> aliesData = new Dictionary<string, Line>();

        private int instrumentCounter = -1;
        private byte[] instrumentBufCache = new byte[Const.INSTRUMENT_SIZE];
        private int toneDoublerCounter = -1;
        private List<int> toneDoublerBufCache = new List<int>();
        private int wfInstrumentCounter = -1;
        private byte[] wfInstrumentBufCache = null;

        public int newStreamID = -1;
        private int pcmDataSeqNum=0;

        public Information info = null;

        public ClsVgm(string stPath)
        {
            chips = new Dictionary<enmChipType, ClsChip[]>();
            info = new Information();

            ym2151 = new YM2151[] { new YM2151(this, 0, "X", stPath,false), new YM2151(this, 1, "Xs", stPath,true) };
            ym2203 = new YM2203[] { new YM2203(this, 0, "N", stPath, false), new YM2203(this, 1, "Ns", stPath, true) };
            ym2608 = new YM2608[] { new YM2608(this, 0, "P", stPath, false), new YM2608(this, 1, "Ps", stPath, true) };
            ym2610b = new YM2610B[] { new YM2610B(this, 0, "T", stPath, false), new YM2610B(this, 1, "Ts", stPath, true) };
            ym2612 = new YM2612[] { new YM2612(this, 0, "F", stPath, false), new YM2612(this, 1, "Fs", stPath, true) };
            sn76489 = new SN76489[] { new SN76489(this, 0, "S", stPath, false), new SN76489(this, 1, "Ss", stPath, true) };
            rf5c164 = new RF5C164[] { new RF5C164(this, 0, "R", stPath, false), new RF5C164(this, 1, "Rs", stPath, true) };
            segapcm = new segaPcm[] { new segaPcm(this, 0, "Z", stPath, false), new segaPcm(this, 1, "Zs", stPath, true) };
            huc6280 = new HuC6280[] { new HuC6280(this, 0, "H", stPath, false), new HuC6280(this, 1, "Hs", stPath, true) };
            ym2612x = new YM2612X[] { new YM2612X(this, 0, "E", stPath, false) };
            ym2413 = new YM2413[] { new YM2413(this, 0, "L", stPath, false), new YM2413(this, 1, "Ls", stPath, true) };
            c140 = new C140[] { new C140(this, 0, "Y", stPath, false), new C140(this, 1, "Ys", stPath, true) };
            ay8910 = new AY8910[] { new AY8910(this, 0, "A", stPath, false), new AY8910(this, 1, "As", stPath, true) };

            chips.Add(enmChipType.YM2612, ym2612);
            chips.Add(enmChipType.SN76489, sn76489);
            chips.Add(enmChipType.RF5C164, rf5c164);
            chips.Add(enmChipType.YM2610B, ym2610b);
            chips.Add(enmChipType.YM2608, ym2608);
            chips.Add(enmChipType.YM2203, ym2203);
            chips.Add(enmChipType.YM2151, ym2151);
            chips.Add(enmChipType.SEGAPCM, segapcm);
            chips.Add(enmChipType.HuC6280, huc6280);
            chips.Add(enmChipType.YM2612X, ym2612x);
            chips.Add(enmChipType.YM2413, ym2413);
            chips.Add(enmChipType.C140, c140);
            chips.Add(enmChipType.AY8910, ay8910);

            List<clsTD> lstTD = new List<clsTD>
            {
                new clsTD(4, 4, 4, 4, 0, 0, 0, 0, 0),
                new clsTD(3, 3, 4, 4, 1, 1, 0, 0, 0),
                new clsTD(4, 4, 5, 5, 1, 1, 0, 0, -4),
                new clsTD(3, 3, 4, 4, 2, 2, 0, 0, 0),
                new clsTD(5, 5, 4, 4, 0, 0, 0, 0, 0),
                new clsTD(4, 4, 3, 3, 0, 0, 0, 0, 5),
                new clsTD(4, 4, 4, 4, 1, 1, 0, 0, 0),
                new clsTD(6, 6, 4, 4, 0, 0, 0, 0, 0),
                new clsTD(4, 4, 4, 4, 2, 2, 0, 0, 0),
                new clsTD(6, 6, 5, 5, 1, 1, 0, 0, -4),
                new clsTD(5, 5, 4, 4, 1, 1, 0, 0, 0),
                new clsTD(6, 6, 5, 5, 2, 2, 0, 0, -4),
                new clsTD(8, 8, 4, 4, 0, 0, 0, 0, 0),
                new clsTD(6, 6, 4, 4, 1, 1, 0, 0, 0),
                new clsTD(8, 8, 5, 5, 1, 1, 0, 0, -4),
                new clsTD(6, 6, 4, 4, 2, 2, 0, 0, 0),
                new clsTD(10, 10, 4, 4, 0, 0, 0, 0, 0),
                new clsTD(8, 8, 3, 3, 0, 0, 0, 0, 5),
                new clsTD(8, 8, 4, 4, 1, 1, 0, 0, 0),
                new clsTD(12, 12, 4, 4, 0, 0, 0, 0, 0)
            };
            clsToneDoubler toneDoubler = new clsToneDoubler(0, lstTD);
            instToneDoubler.Add(0, toneDoubler);

        }



        #region analyze

        /// <summary>
        /// ソースを分類する
        /// </summary>
        public int Analyze(List<Line> src)
        {
            log.Write("テキスト解析開始");
            lineNumber = 0;

            bool multiLine = false;
            string s2 = "";

            foreach (Line line in src)
            {

                string s = line.Txt;
                int lineNumber = line.Num;
                string fn = line.Fn;

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
                    info.AddInformation(s2, lineNumber, fn, chips);
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
                        info.AddInformation(s2, lineNumber, fn, chips);
                    }
                    continue;
                }
                else if (s2.IndexOf("@") == 0)
                {
                    // Instrument
                    AddInstrument(s2, fn, lineNumber);
                    continue;
                }
                else if (s2.IndexOf("%") == 0)
                {
                    // Alies
                    AddAlies(s2, fn, lineNumber);
                    continue;
                }
                else
                {
                    // Part
                    AddPart(s2, fn, lineNumber);
                    continue;
                }

            }

            // 定義中のToneDoublerがあればここで定義完了
            if (toneDoublerCounter != -1)
            {
                toneDoublerCounter = -1;
                SetInstToneDoubler();
            }

            // チェック1定義されていない名称を使用したパートが存在するか

            foreach (string p in partData.Keys)
            {
                bool flg = false;
                foreach (KeyValuePair<enmChipType,ClsChip[]> kvp in chips)
                {
                    foreach (ClsChip chip in kvp.Value)
                    {
                        if (chip.ChannelNameContains(p))
                        {
                            flg = true;
                            break;
                        }
                    }
                }
                if (!flg)
                {
                    msgBox.setWrnMsg(string.Format("未定義のパート({0})のデータは無視されます。", p.Substring(0, 2).Trim() + int.Parse(p.Substring(2, 2)).ToString()));
                    flg = false;
                }
            }

            if (info.userClockCount != 0) info.clockCount = info.userClockCount;

            log.Write("テキスト解析完了");
            return 0;

        }

        private int AddInstrument(string buf, string srcFn, int lineNumber)
        {
            if (buf == null || buf.Length < 2)
            {
                msgBox.setWrnMsg("空の音色定義文を受け取りました。", srcFn, lineNumber);
                return -1;
            }

            string s = buf.Substring(1).TrimStart();

            // FMの音色を定義中の場合
            if (instrumentCounter != -1)
            {

                return SetInstrument(s, srcFn, lineNumber);

            }

            // WaveFormの音色を定義中の場合
            if (wfInstrumentCounter != -1)
            {

                return SetWfInstrument(s, srcFn, lineNumber);

            }

            char t = s.ToUpper()[0];
            if (toneDoublerCounter != -1)
            {
                if (t == 'F' || t == 'N' || t == 'M' || t == 'L' || t == 'P' || t == 'E' || t == 'T' || t == 'H')
                {
                    toneDoublerCounter = -1;
                    SetInstToneDoubler();
                }
            }

            switch (t)
            {
                case 'F':
                    instrumentBufCache = new byte[Const.INSTRUMENT_SIZE - 8];
                    instrumentCounter = 0;
                    SetInstrument(s.Substring(1).TrimStart(), srcFn, lineNumber);
                    return 0;

                case 'N':
                    instrumentBufCache = new byte[Const.INSTRUMENT_SIZE];
                    instrumentCounter = 0;
                    SetInstrument(s.Substring(1).TrimStart(), srcFn, lineNumber);
                    return 0;

                case 'M':
                    instrumentBufCache = new byte[Const.INSTRUMENT_SIZE];
                    instrumentCounter = 0;
                    SetInstrument(s.Substring(1).TrimStart(), srcFn, lineNumber);
                    return 0;

                case 'L':
                    instrumentBufCache = new byte[Const.OPL_INSTRUMENT_SIZE];
                    instrumentCounter = 0;
                    SetInstrument(s.Substring(1).TrimStart(), srcFn, lineNumber);
                    return 0;

                case 'P':
                    try
                    {
                        instrumentCounter = -1;
                        enmChipType enmChip = enmChipType.YM2612;
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
                            if (chipName.IndexOf(Information.PRIMARY) >= 0)
                            {
                                isSecondary = false;
                                chipName = chipName.Replace(Information.PRIMARY, "");
                            }
                            else if (chipName.IndexOf(Information.SECONDARY) >= 0)
                            {
                                isSecondary = true;
                                chipName = chipName.Replace(Information.SECONDARY, "");
                            }

                            if (!GetChip(chipName).CanUsePcm)
                            {
                                msgBox.setWrnMsg("未定義のChipName又はPCMを使用できないChipが指定されています。", srcFn, lineNumber);
                                break;
                            }

                            enmChip = GetChipType(chipName);
                        }
                        if (vs.Length > 5)
                        {
                            lp = int.Parse(vs[5]);
                        }
                        if (instPCM.ContainsKey(num))
                        {
                            instPCM.Remove(num);
                        }
                        instPCM.Add(num, new clsPcm(num, pcmDataSeqNum++, enmChip, isSecondary, fn, fq, vol, 0, 0, 0, lp));
                    }
                    catch
                    {
                        msgBox.setWrnMsg("不正なPCM音色定義文です。", srcFn, lineNumber);
                    }
                    return 0;

                case 'E':
                    try
                    {
                        instrumentCounter = -1;
                        string[] vs = s.Substring(1).Trim().Split(new string[] { "," }, StringSplitOptions.None);
                        int[] env = null;
                        env = new int[9];
                        int num = int.Parse(vs[0]);
                        for (int i = 0; i < env.Length; i++)
                        {
                            if (i == 8)
                            {
                                if (vs.Length == 8) env[i] = (int)enmChipType.SN76489;
                                else env[i] = (int)GetChipType(vs[8]);
                                continue;
                            }
                            env[i] = int.Parse(vs[i]);
                        }

                        enmChipType chiptype = GetChipType(env[8]);
                        if (chips.ContainsKey(chiptype) && chips[chiptype][0].Envelope != null)
                        {
                            CheckEnvelopeVolumeRange(srcFn, lineNumber, env, chips[chiptype][0].Envelope.Max, chips[chiptype][0].Envelope.Min);
                            if (env[7] == 0) env[7] = 1;
                        }
                        else
                        {
                            msgBox.setWrnMsg("エンベロープを使用できない音源が選択されています。", srcFn, lineNumber);
                        }

                        if (instENV.ContainsKey(num))
                        {
                            instENV.Remove(num);
                        }
                        instENV.Add(num, env);
                    }
                    catch
                    {
                        msgBox.setWrnMsg("不正なエンベロープ定義文です。", srcFn, lineNumber);
                    }
                    return 0;

                case 'T':
                    try
                    {
                        instrumentCounter = -1;

                        if (s.ToUpper()[1] != 'D') return 0;

                        toneDoublerBufCache.Clear();
                        StoreToneDoublerBuffer(s.ToUpper().Substring(2).TrimStart(), srcFn, lineNumber);
                    }
                    catch
                    {
                        msgBox.setWrnMsg("不正なTone Doubler定義文です。", srcFn, lineNumber);
                    }
                    return 0;

                case 'H':
                    wfInstrumentBufCache = new byte[Const.WF_INSTRUMENT_SIZE];
                    wfInstrumentCounter = 0;
                    SetWfInstrument(s.Substring(1).TrimStart(), srcFn, lineNumber);
                    return 0;

            }

            // ToneDoublerを定義中の場合
            if (toneDoublerCounter != -1)
            {
                return StoreToneDoublerBuffer(s.ToUpper(), srcFn, lineNumber);
            }

            return 0;
        }

        private static void CheckEnvelopeVolumeRange(string srcFn, int lineNumber, int[] env, int max, int min)
        {
            for (int i = 0; i < env.Length - 1; i++)
            {
                if (i != 1 && i != 4 && i != 7) continue;

                if (env[i] > max)
                {
                    env[i] = max;
                    msgBox.setWrnMsg(string.Format("Envelope音量が{0}を超えています。", max), srcFn, lineNumber);
                }
                if (env[i] < min)
                {
                    env[i] = min;
                    msgBox.setWrnMsg(string.Format("Envelope音量が{0}未満です。", min), srcFn, lineNumber);
                }
            }
        }

        private enmChipType GetChipType(string chipN)
        {
            foreach(KeyValuePair<enmChipType,ClsChip[]> kvp in chips)
            {
                foreach(ClsChip chip in kvp.Value)
                {
                    if (chip.Name.ToUpper().Trim() == chipN.ToUpper().Trim()
                        || chip.ShortName.ToUpper().Trim() == chipN.ToUpper().Trim())
                    {
                        return kvp.Key;
                    }
                }
            }

            return enmChipType.None;
        }

        private enmChipType GetChipType(int chipNum)
        {
            foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in chips)
            {
                if ((int)kvp.Key == chipNum)
                {
                    return kvp.Key;
                }
            }

            return enmChipType.None;
        }

        private ClsChip GetChip(string chipN)
        {
            string n = chipN.ToUpper().Trim();

            foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in chips)
            {
                foreach (ClsChip chip in kvp.Value)
                {
                    if (n == chip.Name.ToUpper()) return chip;
                    if (n == chip.ShortName.ToUpper()) return chip;
                }
            }

            return null;
        }

        private int AddAlies(string buf, string srcFn, int lineNumber)
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
                msgBox.setWrnMsg("不正なエイリアス指定です。", srcFn, lineNumber);
                return -1;
            }
            if (data == "")
            {
                //データがない場合は警告する
                msgBox.setWrnMsg("エイリアスにデータがありません。", srcFn, lineNumber);
            }

            if (aliesData.ContainsKey(name))
            {
                aliesData.Remove(name);
            }
            aliesData.Add(name, new Line("", lineNumber, data));

            return 0;
        }

        private int AddPart(string buf, string srcFn, int lineNumber)
        {
            List<string> part = new List<string>();
            string data = "";

            int i = buf.IndexOfAny(new char[] { ' ', '\t' });
            if (i < 0)
            {
                //空白による区切りが見つからない場合は無視する
                return 0;
            }

            part = Common.DivParts(buf.Substring(0, i).Trim(), chips);
            data = buf.Substring(i).Trim();
            if (part == null)
            {
                //パート指定がない場合は警告とする
                msgBox.setWrnMsg("不正なパート指定です。", srcFn, lineNumber);
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
                    partData.Add(p, new List<Line>());
                }
                partData[p].Add(new Line(srcFn, lineNumber, data));
            }

            return 0;
        }

        private int SetInstrument(string vals, string srcFn, int lineNumber)
        {

            try
            {
                instrumentCounter= GetNums(instrumentBufCache, instrumentCounter, vals);

                if (instrumentCounter == instrumentBufCache.Length)
                {
                    //すでに定義済みの場合はいったん削除する(後に定義されたものが優先)
                    if (instFM.ContainsKey(instrumentBufCache[0]))
                    {
                        instFM.Remove(instrumentBufCache[0]);
                    }


                    if(instrumentBufCache.Length == Const.INSTRUMENT_SIZE)
                    {
                        //M
                        instFM.Add(instrumentBufCache[0], instrumentBufCache);
                    }
                    else if(instrumentBufCache.Length == Const.OPL_INSTRUMENT_SIZE)
                    {
                        //OPL
                        instFM.Add(instrumentBufCache[0], instrumentBufCache);
                    }
                    else
                    {
                        //F
                        instFM.Add(instrumentBufCache[0], ConvertFtoM(instrumentBufCache));
                    }

                    instrumentCounter = -1;
                }
            }
            catch
            {
                msgBox.setErrMsg("音色の定義が不正です。", srcFn, lineNumber);
            }

            return 0;
        }

        private int SetWfInstrument(string vals, string srcFn, int lineNumber)
        {

            try
            {
                wfInstrumentCounter= GetNums(wfInstrumentBufCache, wfInstrumentCounter, vals);

                if (wfInstrumentCounter == wfInstrumentBufCache.Length)
                {
                    if (instWF.ContainsKey(wfInstrumentBufCache[0]))
                    {
                        instWF.Remove(wfInstrumentBufCache[0]);
                    }
                    instWF.Add(wfInstrumentBufCache[0], wfInstrumentBufCache);

                    wfInstrumentCounter = -1;
                }
            }
            catch
            {
                msgBox.setErrMsg("WaveForm音色の定義が不正です。", srcFn, lineNumber);
            }

            return 0;
        }

        private int GetNums(byte[] aryBuf,int aryIndex, string vals)
        {
            string n = "";
            string h = "";
            int hc = -1;
            int i = 0;

            foreach (char c in vals)
            {
                if (c == '$')
                {
                    hc = 0;
                    continue;
                }

                if (hc > -1 && ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'F')))
                {
                    h += c;
                    hc++;
                    if (hc == 2)
                    {
                        i = int.Parse(h, System.Globalization.NumberStyles.HexNumber);
                        aryBuf[aryIndex] = (byte)(i & 0xff);
                        aryIndex++;
                        h = "";
                        hc = -1;
                    }
                    continue;
                }

                if ((c >= '0' && c <= '9') || c == '-')
                {
                    n = n + c.ToString();
                    continue;
                }

                if (int.TryParse(n, out i))
                {
                    aryBuf[aryIndex] = (byte)(i & 0xff);
                    aryIndex++;
                    n = "";
                }
            }

            if (!string.IsNullOrEmpty(n))
            {
                if (int.TryParse(n, out i))
                {
                    aryBuf[aryIndex] = (byte)(i & 0xff);
                    aryIndex++;
                    n = "";
                }
            }

            return aryIndex;
        }

        private int StoreToneDoublerBuffer(string vals, string srcFn, int lineNumber)
        {
            string n = "";
            string h = "";
            int hc = -1;
            int i;

            try
            {
                foreach (char c in vals)
                {
                    if (c == '$')
                    {
                        hc = 0;
                        continue;
                    }

                    if (hc > -1 && ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'F')))
                    {
                        h += c;
                        hc++;
                        if (hc == 2)
                        {
                            i = int.Parse(h, System.Globalization.NumberStyles.HexNumber);
                            toneDoublerBufCache.Add(i);
                            toneDoublerCounter++;
                            h = "";
                            hc = -1;
                        }
                        continue;
                    }

                    if ((c >= '0' && c <= '9') || c == '-')
                    {
                        n = n + c.ToString();
                        continue;
                    }

                    if (int.TryParse(n, out i))
                    {
                        toneDoublerBufCache.Add(i);
                        toneDoublerCounter++;
                        n = "";
                    }
                }

                if (!string.IsNullOrEmpty(n))
                {
                    if (int.TryParse(n, out i))
                    {
                        toneDoublerBufCache.Add(i);
                        toneDoublerCounter++;
                        n = "";
                    }
                }

            }
            catch
            {
                msgBox.setErrMsg("Tone Doublerの定義が不正です。", srcFn, lineNumber);
            }

            return 0;
        }

        private void SetInstToneDoubler()
        {
            if (toneDoublerBufCache.Count < 10)
            {
                toneDoublerBufCache.Clear();
                toneDoublerCounter = -1;
                return;
            }

            int num = toneDoublerBufCache[0];
            int counter = 1;
            List<clsTD> lstTD = new List<clsTD>();
            while (counter < toneDoublerBufCache.Count)
            {
                clsTD td = new clsTD(
                    toneDoublerBufCache[counter++]
                    , toneDoublerBufCache[counter++]
                    , toneDoublerBufCache[counter++]
                    , toneDoublerBufCache[counter++]
                    , toneDoublerBufCache[counter++]
                    , toneDoublerBufCache[counter++]
                    , toneDoublerBufCache[counter++]
                    , toneDoublerBufCache[counter++]
                    , toneDoublerBufCache[counter++]
                    );
                lstTD.Add(td);
            }

            clsToneDoubler toneDoubler = new clsToneDoubler(num, lstTD);
            if (instToneDoubler.ContainsKey(num))
            {
                instToneDoubler.Remove(num);
            }
            instToneDoubler.Add(num, toneDoubler);
            toneDoublerBufCache.Clear();
            toneDoublerCounter = -1;
        }

        private byte[] ConvertFtoM(byte[] instrumentBufCache)
        {
            byte[] ret = new byte[Const.INSTRUMENT_SIZE];

            ret[0] = instrumentBufCache[0];

            for (int j = 0; j < 4; j++)
            {
                for (int i = 0; i < Const.INSTRUMENT_OPERATOR_SIZE; i++)
                {
                    ret[j * Const.INSTRUMENT_M_OPERATOR_SIZE + i + 1] = instrumentBufCache[j * Const.INSTRUMENT_OPERATOR_SIZE + i + 1];
                }
            }

            ret[Const.INSTRUMENT_SIZE - 2] = instrumentBufCache[Const.INSTRUMENT_SIZE - 10];
            ret[Const.INSTRUMENT_SIZE - 1] = instrumentBufCache[Const.INSTRUMENT_SIZE - 9];

            return ret;
        }

        #endregion



        public List<byte> dat = null;
        //xgm music data
        public List<byte> xdat = null;
        //xgm keyOnDataList
        public List<byte> xgmKeyOnData = null;

        public double dSample = 0.0;
        public long lClock = 0L;
        private double sampleB = 0.0;

        public long loopOffset = -1L;
        public long loopSamples = -1L;

        private Random rnd = new Random();



        public byte[] Vgm_getByteData(Dictionary<string, List<MML>> mmlData)
        {

            dat = new List<byte>();

            log.Write("ヘッダー情報作成");
            MakeHeader();

            int endChannel = 0;
            newStreamID = -1;
            int totalChannel = 0;
            foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in chips)
            {
                foreach (ClsChip chip in kvp.Value)
                {
                    totalChannel += chip.ChMax;
                }
            }

            log.Write("MML解析開始");
            do
            {
                foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in chips)
                {
                    foreach (ClsChip chip in kvp.Value)
                    {
                        log.Write(string.Format("Chip [{0}]", chip.Name));

                        partWork pw;
                        for (int i = 0; i < chip.lstPartWork.Count; i++)
                        {
                            pw = chip.lstPartWork[
                                chip.ReversePartWork
                                ? (chip.lstPartWork.Count - 1 - i)
                                : i
                                ];
                            partWorkByteData(pw);
                        }
                        if (chip.SupportReversePartWork) chip.ReversePartWork = !chip.ReversePartWork;

                        log.Write("channelを跨ぐコマンド向け処理");
                        chip.MultiChannelCommand();
                    }
                }

                log.Write("全パートのうち次のコマンドまで一番近い値を求める");
                long cnt = long.MaxValue;
                foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in chips)
                {
                    foreach (ClsChip chip in kvp.Value)
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
                                if (cpw.envelopeMode && cpw.envIndex != -1)
                                {
                                    cnt = Math.Min(cnt, cpw.envCounter);
                                }
                            }

                            //pcm
                            if (cpw.pcmWaitKeyOnCounter > 0)
                            {
                                cnt = Math.Min(cnt, cpw.pcmWaitKeyOnCounter);
                            }

                        }

                    }
                }

                log.Write("全パートのwaitcounterを減らす");
                if (cnt != long.MaxValue)
                {

                    // waitcounterを減らす

                    foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in chips)
                    {
                        foreach (ClsChip chip in kvp.Value)
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

                                if (pw.envelopeMode && pw.envIndex != -1)
                                {
                                    pw.envCounter -= (int)cnt;
                                }
                            }
                        }
                    }

                    // wait発行

                    lClock += cnt;
                    dSample += info.samplesPerClock * cnt;

                    if (ym2612[0].lstPartWork[5].pcmWaitKeyOnCounter <= 0)//== -1)
                    {
                        OutWaitNSamples((long)(info.samplesPerClock * cnt));
                    }
                    else
                    {
                        OutWaitNSamplesWithPCMSending(ym2612[0].lstPartWork[5], cnt);
                    }

                }

                log.Write("終了パートのカウント");
                endChannel = 0;
                foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in chips)
                {
                    foreach (ClsChip chip in kvp.Value)
                    {
                        foreach (partWork pw in chip.lstPartWork)
                        {
                            if (!pw.chip.use) endChannel++;
                            else if (pw.dataEnd && pw.waitCounter < 1) endChannel++;
                            else if (loopOffset != -1 && pw.dataEnd && pw.envIndex == 3) endChannel++;
                        }
                    }
                }

            } while (endChannel < totalChannel);

            log.Write("フッター情報の作成");
            MakeFooter();

            return dat.ToArray();
        }

        private void partWorkByteData(partWork pw)
        {

            //未使用のパートの場合は処理を行わない
            if (!pw.chip.use) return;
            if (pw.mmlData == null) return;

            log.Write("MD stream pcm sound off");
            if (pw.pcmWaitKeyOnCounter == 0)
                pw.pcmWaitKeyOnCounter = -1;

            log.Write("KeyOff");
            ProcKeyOff(pw);

            log.Write("Bend");
            ProcBend(pw);

            log.Write("Lfo");
            ProcLfo(pw);

            log.Write("Envelope");
            ProcEnvelope(pw);

            pw.chip.SetFNum(pw);
            pw.chip.SetVolume(pw);

            log.Write("wait消化待ち");
            if (pw.waitCounter > 0)
            {
                return;
            }

            log.Write("データは最後まで実施されたか");
            if (pw.dataEnd)
            {
                return;
            }

            log.Write("パートのデータがない場合は何もしないで次へ");
            if (pw.mmlData == null || pw.mmlData.Count < 1)
            {
                pw.dataEnd = true;
                return;
            }

            log.Write("コマンド毎の処理を実施");
            while (pw.waitCounter == 0 && !pw.dataEnd)
            {
                if (pw.mmlPos >= pw.mmlData.Count)
                {
                    pw.dataEnd = true;
                }
                else
                {
                    MML mml = pw.mmlData[pw.mmlPos];
                    //lineNumber = pw.getLineNumber();
                    Commander(pw, mml);
                }
            }

        }

        private void MakeHeader()
        {

            //Header
            OutData(Const.hDat);

            //PCM Data block
            foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in chips)
            {
                foreach (ClsChip chip in kvp.Value)
                {
                    chip.SetPCMDataBlock();
                }
            }

            //Set Initialize data
            foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in chips)
            {
                foreach (ClsChip chip in kvp.Value)
                {
                    chip.InitChip();
                }
            }

        }

        private void MakeFooter()
        {

            byte[] v;

            //end of data
            OutData(0x66);

            //GD3 offset
            v = DivInt2ByteAry(dat.Count - 0x14);
            dat[0x14] = v[0]; dat[0x15] = v[1]; dat[0x16] = v[2]; dat[0x17] = v[3];

            //Total # samples
            v = DivInt2ByteAry((int)dSample);
            dat[0x18] = v[0]; dat[0x19] = v[1]; dat[0x1a] = v[2]; dat[0x1b] = v[3];

            if (loopOffset != -1)
            {
                //Loop offset
                v = DivInt2ByteAry((int)(loopOffset - 0x1c));
                dat[0x1c] = v[0]; dat[0x1d] = v[1]; dat[0x1e] = v[2]; dat[0x1f] = v[3];

                //Loop # samples
                v = DivInt2ByteAry((int)(dSample - loopSamples));
                dat[0x20] = v[0]; dat[0x21] = v[1]; dat[0x22] = v[2]; dat[0x23] = v[3];
            }

            int p = dat.Count + 12;

            MakeGD3(dat);

            //EoF offset
            v = DivInt2ByteAry(dat.Count - 0x4);
            dat[0x4] = v[0]; dat[0x5] = v[1]; dat[0x6] = v[2]; dat[0x7] = v[3];

            int q = dat.Count - p;

            //GD3 Length
            v = DivInt2ByteAry(q);
            dat[p - 4] = v[0]; dat[p - 3] = v[1]; dat[p - 2] = v[2]; dat[p - 1] = v[3];

            long useYM2151 = 0;
            long useYM2203 = 0;
            long useYM2608 = 0;
            long useYM2610B = 0;
            long useYM2612 = 0;
            long useSN76489 = 0;
            long useRf5c164 = 0;
            long useSegaPcm = 0;
            long useHuC6280 = 0;
            long useC140 = 0;
            long useAY8910 = 0;

            for (int i = 0; i < 2; i++)
            {
                foreach (partWork pw in ym2151[i].lstPartWork)
                { useYM2151 += pw.clockCounter; }
                foreach (partWork pw in ym2203[i].lstPartWork)
                { useYM2203 += pw.clockCounter; }
                foreach (partWork pw in ym2608[i].lstPartWork)
                { useYM2608 += pw.clockCounter; }
                foreach (partWork pw in ym2610b[i].lstPartWork)
                { useYM2610B += pw.clockCounter; }
                foreach (partWork pw in ym2612[i].lstPartWork)
                { useYM2612 += pw.clockCounter; }
                foreach (partWork pw in sn76489[i].lstPartWork)
                { useSN76489 += pw.clockCounter; }
                foreach (partWork pw in rf5c164[i].lstPartWork)
                { useRf5c164 += pw.clockCounter; }
                foreach (partWork pw in segapcm[i].lstPartWork)
                { useSegaPcm += pw.clockCounter; }
                foreach (partWork pw in huc6280[i].lstPartWork)
                { useHuC6280 += pw.clockCounter; }
                foreach (partWork pw in c140[i].lstPartWork)
                { useC140 += pw.clockCounter; }
                foreach (partWork pw in ay8910[i].lstPartWork)
                { useAY8910 += pw.clockCounter; }
            }

            if (useSN76489 == 0)
            { dat[0x0c] = 0; dat[0x0d] = 0; dat[0x0e] = 0; dat[0x0f] = 0; }
            if (useYM2612 == 0)
            { dat[0x2c] = 0; dat[0x2d] = 0; dat[0x2e] = 0; dat[0x2f] = 0; }
            if (useYM2151 == 0)
            { dat[0x30] = 0; dat[0x31] = 0; dat[0x32] = 0; dat[0x33] = 0; }
            if (useSegaPcm == 0)
            { dat[0x38] = 0; dat[0x39] = 0; dat[0x3a] = 0; dat[0x3b] = 0; dat[0x3c] = 0; dat[0x3d] = 0; dat[0x3e] = 0; dat[0x3f] = 0; }
            if (useYM2203 == 0)
            { dat[0x44] = 0; dat[0x45] = 0; dat[0x46] = 0; dat[0x47] = 0; }
            if (useYM2608 == 0)
            { dat[0x48] = 0; dat[0x49] = 0; dat[0x4a] = 0; dat[0x4b] = 0; }
            if (useYM2610B == 0)
            { dat[0x4c] = 0; dat[0x4d] = 0; dat[0x4e] = 0; dat[0x4f] = 0; }
            if (useRf5c164 == 0)
            { dat[0x6c] = 0; dat[0x6d] = 0; dat[0x6e] = 0; dat[0x6f] = 0; }
            if (useHuC6280 == 0)
            { dat[0xa4] = 0; dat[0xa5] = 0; dat[0xa6] = 0; dat[0xa7] = 0; }
            if (useC140 == 0)
            { dat[0xa8] = 0; dat[0xa9] = 0; dat[0xaa] = 0; dat[0xab] = 0; dat[0x96] = 0; }
            if (useAY8910 == 0)
            { dat[0x74] = 0; dat[0x75] = 0; dat[0x76] = 0; dat[0x77] = 0; dat[0x78] = 0; dat[0x79] = 0; dat[0x7a] = 0; dat[0x7b] = 0; }

            if (info.Version == 1.51f)
            { dat[0x08] = 0x51; dat[0x09] = 0x01; }
            else if (info.Version == 1.60f)
            { dat[0x08] = 0x60; dat[0x09] = 0x01; }
            else
            { dat[0x08] = 0x61; dat[0x09] = 0x01; }

        }

        private void MakeGD3(List<byte> dat)
        {
            //'Gd3 '
            dat.Add(0x47); dat.Add(0x64); dat.Add(0x33); dat.Add(0x20);

            //GD3 Version
            dat.Add(0x00); dat.Add(0x01); dat.Add(0x00); dat.Add(0x00);

            //GD3 Length(dummy)
            dat.Add(0x00); dat.Add(0x00); dat.Add(0x00); dat.Add(0x00);

            //TrackName
            dat.AddRange(Encoding.Unicode.GetBytes(info.TitleName));
            dat.Add(0x00); dat.Add(0x00);
            dat.AddRange(Encoding.Unicode.GetBytes(info.TitleNameJ));
            dat.Add(0x00); dat.Add(0x00);

            //GameName
            dat.AddRange(Encoding.Unicode.GetBytes(info.GameName));
            dat.Add(0x00); dat.Add(0x00);
            dat.AddRange(Encoding.Unicode.GetBytes(info.GameNameJ));
            dat.Add(0x00); dat.Add(0x00);

            //SystemName
            dat.AddRange(Encoding.Unicode.GetBytes(info.SystemName));
            dat.Add(0x00); dat.Add(0x00);
            dat.AddRange(Encoding.Unicode.GetBytes(info.SystemNameJ));
            dat.Add(0x00); dat.Add(0x00);

            //Composer
            dat.AddRange(Encoding.Unicode.GetBytes(info.Composer));
            dat.Add(0x00); dat.Add(0x00);
            dat.AddRange(Encoding.Unicode.GetBytes(info.ComposerJ));
            dat.Add(0x00); dat.Add(0x00);

            //ReleaseDate
            dat.AddRange(Encoding.Unicode.GetBytes(info.ReleaseDate));
            dat.Add(0x00); dat.Add(0x00);

            //Converted
            dat.AddRange(Encoding.Unicode.GetBytes(info.Converted));
            dat.Add(0x00); dat.Add(0x00);

            //Notes
            dat.AddRange(Encoding.Unicode.GetBytes(info.Notes));
            dat.Add(0x00); dat.Add(0x00);
        }


        public byte[] Xgm_getByteData(Dictionary<string, List<MML>> mmlData)
        {

            //PartInit();

            dat = new List<byte>();
            xdat = new List<byte>();

            log.Write("ヘッダー情報作成(XGM)");
            Xgm_makeHeader();

            int endChannel = 0;
            int totalChannel = 0;
            foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in chips)
            {
                foreach (ClsChip chip in kvp.Value)
                {
                    if (chip.ShortName != "OPN2X" && chip.ShortName != "DCSG")
                    {
                        foreach (partWork pw in chip.lstPartWork) pw.chip.use = false;
                    }
                    totalChannel += chip.ChMax;
                }
            }

            log.Write("MML解析開始(XGM)");
            do
            {
                //KeyOnリストをクリア
                xgmKeyOnData = new List<byte>();

                foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in chips)
                {
                    foreach (ClsChip chip in kvp.Value)
                    {
                        log.Write(string.Format("Chip [{0}]", chip.Name));

                        //未使用のchipの場合は処理を行わない
                        if (!chip.use) continue;

                        //chip毎の処理
                        Xgm_procChip(chip);
                    }
                }

                log.Write("全パートのうち次のコマンドまで一番近い値を求める");
                long waitCounter = Xgm_procCheckMinimumWaitCounter();

                log.Write("KeyOn情報をかき出し");
                foreach (byte dat in xgmKeyOnData) OutData(0x52, 0x28, dat);

                log.Write("全パートのwaitcounterを減らす");
                if (waitCounter != long.MaxValue)
                {
                    //wait処理
                    Xgm_procWait(waitCounter);
                }

                log.Write("終了パートのカウント");
                endChannel = 0;
                foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in chips)
                {
                    foreach (ClsChip chip in kvp.Value)
                    {
                        foreach (partWork pw in chip.lstPartWork)
                        {
                            if (!pw.chip.use) endChannel++;
                            else if (pw.dataEnd && pw.waitCounter < 1) endChannel++;
                            else if (loopOffset != -1 && pw.dataEnd && pw.envIndex == 3) endChannel++;
                        }
                    }
                }

            } while (endChannel < totalChannel);//全てのチャンネルが終了していない場合はループする

            log.Write("VGMデータをXGMへコンバート");
            dat = ConvertVGMtoXGM(dat);

            log.Write("フッター情報の作成");
            Xgm_makeFooter();

            return dat.ToArray();
        }

        private void Xgm_makeHeader()
        {

            //Header
            foreach (byte b in Const.xhDat)
            {
                xdat.Add(b);
            }

            //FM音源を初期化

            ym2612x[0].OutOPNSetHardLfo(ym2612x[0].lstPartWork[0], false, 0);
            ym2612x[0].OutOPNSetCh3SpecialMode(ym2612x[0].lstPartWork[0], false);
            ym2612x[0].OutSetCh6PCMMode(ym2612x[0].lstPartWork[0], false);
            ym2612x[0].OutFmAllKeyOff();

            foreach (partWork pw in ym2612x[0].lstPartWork)
            {
                if (pw.ch == 0)
                {
                    pw.hardLfoSw = false;
                    pw.hardLfoNum = 0;
                    ym2612x[0].OutOPNSetHardLfo(pw, pw.hardLfoSw, pw.hardLfoNum);
                }

                if (pw.ch < 6)
                {
                    pw.pan.val = 3;
                    pw.ams = 0;
                    pw.fms = 0;
                    if (!pw.dataEnd) ym2612x[0].OutOPNSetPanAMSPMS(pw, 3, 0, 0);
                }
            }

        }

        private void Xgm_makeFooter()
        {

            //$0004               Sample id table
            uint ptr = 0;
            int n = 4;
            foreach (clsPcm p in instPCM.Values)
            {
                if (p.chip != enmChipType.YM2612X) continue;

                uint stAdr = ptr;
                uint size = (uint)p.size;
                if (size > (uint)p.xgmMaxSampleCount + 1)
                {
                    size = (uint)p.xgmMaxSampleCount + 1;
                    size = (uint)((size & 0xffff00) + (size % 0x100 != 0 ? 0x100 : 0x0));
                }
                p.size = size;

                xdat[n + 0] = (byte)((stAdr / 256) & 0xff);
                xdat[n + 1] = (byte)(((stAdr / 256) & 0xff00) >> 8);
                xdat[n + 2] = (byte)((size / 256) & 0xff);
                xdat[n + 3] = (byte)(((size / 256) & 0xff00) >> 8);

                ptr += size;
                n += 4;
            }

            //$0100               Sample data bloc size / 256
            if (ym2612x[0].pcmData != null)
            {
                xdat[0x100] = (byte)((ptr / 256) & 0xff);
                xdat[0x101] = (byte)(((ptr / 256) & 0xff00) >> 8);
            }
            else
            {
                xdat[0x100] = 0;
                xdat[0x101] = 0;
            }

            //$0103 bit #0: NTSC / PAL information
            xdat[0x103] |= (byte)(info.xgmSamplesPerSecond == 50 ? 1 : 0);

            //$0104               Sample data block
            if (ym2612x[0].pcmData != null)
            {
                foreach (clsPcm p in instPCM.Values)
                {
                    if (p.chip != enmChipType.YM2612X) continue;

                    for (uint cnt = 0; cnt < p.size; cnt++)
                    {
                        xdat.Add(ym2612x[0].pcmData[p.stAdr + cnt]);
                    }

                }

            }

            if (dat != null)
            {
                //$0104 + SLEN        Music data bloc size.
                xdat.Add((byte)((dat.Count & 0xff) >> 0));
                xdat.Add((byte)((dat.Count & 0xff00) >> 8));
                xdat.Add((byte)((dat.Count & 0xff0000) >> 16));
                xdat.Add((byte)((dat.Count & 0xff000000) >> 24));

                //$0108 + SLEN        Music data bloc
                foreach (byte b in dat)
                {
                    xdat.Add(b);
                }
            }
            else
            {
                xdat.Add(0);
                xdat.Add(0);
                xdat.Add(0);
                xdat.Add(0);
            }

            //$0108 + SLEN + MLEN GD3 tags
            MakeGD3(xdat);

            dat = xdat;
        }

        private void Xgm_procChip(ClsChip chip)
        {
            foreach (partWork pw in chip.lstPartWork)
            {
                log.Write("KeyOff");
                ProcKeyOff(pw);

                log.Write("Bend");
                ProcBend(pw);

                log.Write("Lfo");
                ProcLfo(pw);

                log.Write("Envelope");
                ProcEnvelope(pw);

                pw.chip.SetFNum(pw);
                pw.chip.SetVolume(pw);

                log.Write("wait消化待ち");
                if (pw.waitCounter > 0) continue;

                log.Write("データは最後まで実施されたか");
                if (pw.dataEnd) continue;

                log.Write("パートのデータがない場合は何もしないで次へ");
                if (pw.mmlData == null || pw.mmlData.Count < 1)
                {
                    pw.dataEnd = true;
                    continue;
                }

                log.Write("コマンド毎の処理を実施");
                while (pw.waitCounter == 0 && !pw.dataEnd)
                {
                    if (pw.mmlPos >= pw.mmlData.Count)
                    {
                        pw.dataEnd = true;
                    }
                    else
                    {
                        MML mml = pw.mmlData[pw.mmlPos];
                        //lineNumber = pw.getLineNumber();
                        Commander(pw, mml);
                    }
                }
            }
        }

        private long Xgm_procCheckMinimumWaitCounter()
        {
            long cnt = long.MaxValue;

            foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in chips)
            {
                foreach (ClsChip chip in kvp.Value)
                {
                    if (!chip.use) continue;

                    foreach (partWork pw in chip.lstPartWork)
                    {
                        //note
                        if (pw.waitKeyOnCounter > 0) cnt = Math.Min(cnt, pw.waitKeyOnCounter);
                        else if (pw.waitCounter > 0) cnt = Math.Min(cnt, pw.waitCounter);

                        //bend
                        if (pw.bendWaitCounter != -1) cnt = Math.Min(cnt, pw.bendWaitCounter);

                        //lfoとenvelopeは音長によるウエイトカウントが存在する場合のみ対象にする。(さもないと、曲のループ直前の効果を出せない)
                        if (cnt < 1) continue;

                        //lfo
                        for (int lfo = 0; lfo < 4; lfo++)
                        {
                            if (!pw.lfo[lfo].sw) continue;
                            if (pw.lfo[lfo].waitCounter == -1) continue;

                            cnt = Math.Min(cnt, pw.lfo[lfo].waitCounter);
                        }

                        //envelope
                        if (!(pw.chip is SN76489)) continue;
                        if (pw.envelopeMode && pw.envIndex != -1) cnt = Math.Min(cnt, pw.envCounter);
                    }
                }
            }

            return cnt;
        }

        private void Xgm_procWait(long cnt)
        {
            foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in chips)
            {
                foreach (ClsChip chip in kvp.Value)
                {
                    if (!chip.use) continue;

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

                        if (!(pw.chip is SN76489)) continue;
                        if (pw.envelopeMode && pw.envIndex != -1) pw.envCounter -= (int)cnt;
                    }
                }
            }

            // wait発行
            lClock += cnt;
            dSample += info.samplesPerClock * cnt;

            sampleB += info.samplesPerClock * cnt;
            OutWaitNSamples((long)(sampleB));
            sampleB -= (long)sampleB;
        }

        private List<byte> ConvertVGMtoXGM(List<byte> src)
        {
            if (src == null || src.Count < 1) return null;

            List<byte> des = new List<byte>();
            loopOffset = -1;

            int[][] opn2reg = new int[2][] { new int[0x100], new int[0x100] };
            for (int i = 0; i < 512; i++) opn2reg[i / 0x100][i % 0x100] = -1;
            int[] psgreg = new int[16];
            int psgch = -1;
            int psgtp = -1;
            for (int i = 0; i < 16; i++) psgreg[i] = -1;
            int framePtr = 0;
            int frameCnt = 0;

            for (int ptr = 0; ptr < src.Count; ptr++)
            {

                byte cmd = src[ptr];
                int p;
                int c;

                switch (cmd)
                {
                    case 0x61: //Wait

                        if (psgtp != -1)
                        {
                            p = des.Count;
                            c = 0;
                            des.Add(0x10);
                            for (int j = 0; j < 16; j++)
                            {
                                if (psgreg[j] == -1) continue;
                                int latch = (j & 1) == 0 ? 0x80 : 0;
                                int ch = (j & 0x0c) << 3;
                                int tp = (j & 2) << 3;
                                des.Add((byte)(latch | (latch != 0 ? (ch | tp) : 0) | psgreg[j]));
                                c++;
                            }
                            c--;
                            des[p] |= (byte)c;

                            psgch = -1;
                            psgtp = -1;
                            for (int i = 0; i < 16; i++) psgreg[i] = -1;
                        }

                        if (des.Count - framePtr > 256)
                        {
                            msgBox.setWrnMsg(string.Format("1Frameに収められる限界バイト数(256byte)を超えています。データを分散させてください。 Frame {0} : {1}byte", frameCnt, des.Count - framePtr));
                        }
                        framePtr = des.Count;

                        int cnt = src[ptr + 1] + src[ptr + 2] * 0x100;
                        for (int j = 0; j < cnt; j++)
                        {
                            //wait
                            des.Add(0x00);
                            frameCnt++;
                        }
                        ptr += 2;
                        break;
                    case 0x50: //DCSG
                        do
                        {
                            bool latch = (src[ptr + 1] & 0x80) != 0;
                            int ch = (src[ptr + 1] & 0x60) >> 5;
                            int tp = (src[ptr + 1] & 0x10) >> 3;
                            int d1 = (src[ptr + 1] & 0xf);
                            int d2 = (src[ptr + 1] & 0x3f);
                            if (latch)
                            {
                                psgch = ch;
                                psgtp = tp;
                                psgreg[ch * 4 + 0 + tp] = d1;
                            }
                            else
                            {
                                if (psgch != -1)
                                {
                                    psgreg[psgch * 4 + 1 + psgtp] = d2;
                                }
                                psgch = -1;
                            }
                            ptr += 2;
                        }while(ptr < src.Count - 1 && src[ptr] == 0x50);
                        ptr--;
                        break;
                    case 0x52: //YM2612 Port0
                        if (opn2reg[0][src[ptr + 1]] != src[ptr + 2] || src[ptr + 1] == 0x28)
                        {

                            bool isKeyOn = src[ptr + 1] == 0x28;
                            if (!isKeyOn)
                            {
                                p = des.Count;
                                c = 0;
                                des.Add(0x20);
                                do
                                {
                                    if (opn2reg[0][src[ptr + 1]] != src[ptr + 2])
                                    {
                                        //F-numの場合は圧縮対象外
                                        if(src[ptr+1]<0xa0 || src[ptr+1]>=0xb0) opn2reg[0][src[ptr + 1]] = src[ptr + 2];

                                        des.Add(src[ptr + 1]);
                                        des.Add(src[ptr + 2]);
                                        c++;
                                    }
                                    ptr += 3;
                                } while (c < 16 && ptr < src.Count - 1 && src[ptr] == 0x52 && src[ptr + 1] != 0x28);
                                c--;
                                ptr--;
                                des[p] |= (byte)c;
                            }
                            else
                            {
                                p = des.Count;
                                c = 0;
                                des.Add(0x40);
                                do
                                {
                                    //des.Add(src[ptr + 1]);
                                    des.Add(src[ptr + 2]);
                                    c++;
                                    ptr += 3;
                                } while (c < 16 && ptr < src.Count - 1 && src[ptr] == 0x52 && src[ptr + 1] == 0x28);
                                c--;
                                ptr--;
                                des[p] |= (byte)c;
                            }
                        }
                        else
                        {
                            ptr += 2;
                        }
                        break;
                    case 0x53: //YM2612 Port1
                        if (opn2reg[1][src[ptr + 1]] != src[ptr + 2])
                        {

                            p = des.Count;
                            c = 0;
                            des.Add(0x30);
                            do
                            {
                                if (opn2reg[1][src[ptr + 1]] != src[ptr + 2])
                                {
                                    //F-numの場合は圧縮対象外
                                    if (src[ptr + 1] < 0xa0 || src[ptr + 1] >= 0xb0) opn2reg[1][src[ptr + 1]] = src[ptr + 2];
                                    des.Add(src[ptr + 1]);
                                    des.Add(src[ptr + 2]);
                                    c++;
                                }
                                ptr += 3;
                            } while (c < 16 && ptr < src.Count - 1 && src[ptr] == 0x53);
                            c--;
                            ptr--;
                            des[p] |= (byte)c;
                        }
                        else
                        {
                            ptr += 2;
                        }
                        break;
                    case 0x54: //PCM KeyON (YM2151)
                        des.Add(src[ptr + 1]);
                        des.Add(src[ptr + 2]);
                        ptr += 2;
                        break;
                    case 0x7e: //LOOP Point
                        loopOffset = des.Count;
                        for (int i = 0; i < 512; i++) opn2reg[i / 0x100][i % 0x100] = -1;
                        break;
                    default:
                        return null;
                }
            }

            if (loopOffset == -1 || loopOffset == des.Count)
            {
                des.Add(0x7f);
            }
            else
            {
                des.Add(0x7e);
                des.Add((byte)(loopOffset & 0xff));
                des.Add((byte)((loopOffset & 0xff00) >> 8));
                des.Add((byte)((loopOffset & 0xff0000) >> 16));
            }

            return des;
        }


        private void ProcKeyOff(partWork pw)
        {
            if (pw.waitKeyOnCounter == 0)
            {
                if (!pw.tie)
                {
                    if (!pw.envelopeMode)
                    {
                        pw.chip.SetKeyOff(pw);
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

                //次回に引き継ぎリセット
                pw.beforeTie = pw.tie;
                pw.tie = false;

                //ゲートタイムカウンターをリセット
                pw.waitKeyOnCounter = -1;
            }
        }

        private void ProcBend(partWork pw)
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

        private void ProcLfo(partWork cpw)
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
                    if ((cpw.chip is YM2612) || (cpw.chip is YM2612X))
                    {
                        cpw.ams = pl.param[3];
                        cpw.fms = pl.param[2];
                        ((ClsOPN)cpw.chip).OutOPNSetPanAMSPMS(cpw, (int)cpw.pan.val, cpw.ams, cpw.fms);
                        cpw.chip.lstPartWork[0].hardLfoSw = true;
                        cpw.chip.lstPartWork[0].hardLfoNum = pl.param[1];
                        ((ClsOPN)cpw.chip).OutOPNSetHardLfo(cpw, cpw.hardLfoSw, cpw.hardLfoNum);
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

        private void ProcEnvelope(partWork pw)
        {
            if (!pw.envelopeMode) return;

            if (pw.envIndex == -1) return;

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
                pw.chip.SetKeyOff(pw);
            }
        }

        private void Commander(partWork pw, MML mml)
        {

            switch (mml.type)
            {
                case enmMMLType.CompileSkip:
                    log.Write("CompileSkip");
                    pw.dataEnd = true;
                    pw.waitCounter = -1;
                    break;
                case enmMMLType.Tempo:
                    log.Write("Tempo");
                    pw.chip.CmdTempo(pw, mml);
                    pw.mmlPos++;
                    break;
                case enmMMLType.Instrument:
                    log.Write("Instrument");
                    pw.chip.CmdInstrument(pw, mml);
                    pw.mmlPos++;
                    break;
                case enmMMLType.Octave:
                    log.Write("Octave");
                    pw.chip.CmdOctave(pw, mml);
                    pw.mmlPos++;
                    break;
                case enmMMLType.OctaveUp:
                    log.Write("OctaveUp");
                    pw.chip.CmdOctaveUp(pw, mml);
                    pw.mmlPos++;
                    break;
                case enmMMLType.OctaveDown:
                    log.Write("OctaveDown");
                    pw.chip.CmdOctaveDown(pw, mml);
                    pw.mmlPos++;
                    break;
                case enmMMLType.Length:
                    log.Write("Length");
                    pw.chip.CmdLength(pw, mml);
                    pw.mmlPos++;
                    break;
                case enmMMLType.TotalVolume:
                    log.Write("TotalVolume");
                    pw.chip.CmdTotalVolume(pw, mml);
                    pw.mmlPos++;
                    break;
                case enmMMLType.Volume:
                    log.Write("Volume");
                    pw.chip.CmdVolume(pw, mml);
                    pw.mmlPos++;
                    break;
                case enmMMLType.VolumeDown:
                    log.Write("VolumeDown");
                    pw.chip.CmdVolumeDown(pw, mml);
                    pw.mmlPos++;
                    break;
                case enmMMLType.VolumeUp:
                    log.Write("VolumeUp");
                    pw.chip.CmdVolumeUp(pw, mml);
                    pw.mmlPos++;
                    break;
                case enmMMLType.Pan:
                    log.Write("Pan");
                    pw.chip.CmdPan(pw, mml);
                    pw.mmlPos++;
                    break;
                case enmMMLType.Gatetime:
                    log.Write("Gatetime");
                    pw.chip.CmdGatetime(pw, mml);
                    pw.mmlPos++;
                    break;
                case enmMMLType.GatetimeDiv:
                    log.Write("GatetimeDiv");
                    pw.chip.CmdGatetime2(pw, mml);
                    pw.mmlPos++;
                    break;
                case enmMMLType.Detune:
                    log.Write("Detune");
                    pw.chip.CmdDetune(pw, mml);
                    pw.mmlPos++;
                    break;
                case enmMMLType.Renpu:
                    log.Write("Renpu");
                    pw.chip.CmdRenpuStart(pw, mml);
                    pw.mmlPos++;
                    break;
                case enmMMLType.RenpuEnd:
                    log.Write("RenpuEnd");
                    pw.chip.CmdRenpuEnd(pw, mml);
                    pw.mmlPos++;
                    break;
                case enmMMLType.Repeat:
                    log.Write("Repeat");
                    pw.chip.CmdRepeatStart(pw, mml);
                    pw.mmlPos++;
                    break;
                case enmMMLType.RepeatEnd:
                    log.Write("RepeatEnd");
                    pw.chip.CmdRepeatEnd(pw, mml);
                    pw.mmlPos++;
                    break;
                case enmMMLType.RepertExit:
                    log.Write("RepertExit");
                    pw.chip.CmdRepeatExit(pw, mml);
                    pw.mmlPos++;
                    break;
                case enmMMLType.Note:
                    log.Write("Note");
                    pw.chip.CmdNote(pw, mml);
                    pw.mmlPos++;
                    break;
                case enmMMLType.Rest:
                    log.Write("Rest");
                    pw.chip.CmdRest(pw, mml);
                    pw.mmlPos++;
                    break;
                case enmMMLType.Envelope:
                    log.Write("Envelope");
                    pw.chip.CmdEnvelope(pw, mml);
                    pw.mmlPos++;
                    break;
                case enmMMLType.HardEnvelope:
                    log.Write("HardEnvelope");
                    pw.chip.CmdHardEnvelope(pw, mml);
                    pw.mmlPos++;
                    break;
                case enmMMLType.ExtendChannel:
                    log.Write("ExtendChannel");
                    pw.chip.CmdExtendChannel(pw, mml);
                    pw.mmlPos++;
                    break;
                case enmMMLType.Lfo:
                    log.Write("Lfo");
                    pw.chip.CmdLfo(pw, mml);
                    pw.mmlPos++;
                    break;
                case enmMMLType.LfoSwitch:
                    log.Write("LfoSwitch");
                    pw.chip.CmdLfoSwitch(pw, mml);
                    pw.mmlPos++;
                    break;
                case enmMMLType.PcmMode:
                    log.Write("PcmMode");
                    pw.chip.CmdMode(pw, mml);
                    pw.mmlPos++;
                    break;
                case enmMMLType.Noise:
                    log.Write("Noise");
                    pw.chip.CmdNoise(pw, mml);
                    pw.mmlPos++;
                    break;
                case enmMMLType.Bend:
                    log.Write("Bend");
                    pw.chip.CmdBend(pw, mml);
                    pw.mmlPos++;
                    break;
                case enmMMLType.Y:
                    log.Write("Y");
                    pw.chip.CmdY(pw, mml);
                    pw.mmlPos++;
                    break;
                case enmMMLType.LoopPoint:
                    log.Write("LoopPoint");
                    pw.chip.CmdLoop(pw, mml);
                    pw.mmlPos++;
                    break;
                case enmMMLType.NoiseToneMixer:
                    log.Write("NoiseToneMixer");
                    pw.chip.CmdNoiseToneMixer(pw, mml);
                    pw.mmlPos++;
                    break;
                case enmMMLType.KeyShift:
                    log.Write("KeyShift");
                    pw.chip.CmdKeyShift(pw, mml);
                    pw.mmlPos++;
                    break;

                default:
                    msgBox.setErrMsg(string.Format("未知のコマンド{0}を検出しました。"
                        , mml.type)
                        , mml.line.Fn
                        , mml.line.Num);
                    pw.mmlPos++;
                    break;
            }
        }


        public void PartInit()
        {
            foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in chips)
            {
                foreach (ClsChip chip in kvp.Value)
                {
                    chip.use = false;
                    chip.lstPartWork = new List<partWork>();

                    for (int i = 0; i < chip.ChMax; i++)
                    {
                        partWork pw = new partWork()
                        {
                            chip = chip,
                            isSecondary = (chip.ChipID == 1),
                            ch = i// + 1;
                        };

                        if (partData.ContainsKey(chip.Ch[i].Name))
                        {
                            pw.pData = partData[chip.Ch[i].Name];
                        }
                        pw.aData = aliesData;
                        pw.setPos(0);

                        pw.Type = chip.Ch[i].Type;
                        pw.slots = 0;
                        pw.volume = 32767;

                        chip.InitPart(ref pw);

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
        }

        public void SetMMLDataToPart(Dictionary<string, List<MML>> mmlData)
        {
            if (mmlData == null) return;

            foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in chips)
            {
                foreach (ClsChip chip in kvp.Value)
                {
                    foreach (partWork pw in chip.lstPartWork)
                    {
                        pw.pData = null;
                        pw.aData = null;
                        //pw.mmlData = null;
                        pw.dataEnd = true;
                        if (mmlData.ContainsKey(pw.PartName))
                        {
                            //pw.mmlData = mmlData[pw.PartName];
                            chip.use = true;
                            pw.dataEnd = false;
                        }
                    }
                }
            }
        }

        //private void CmdNote(partWork pw, char cmd)
        //{
        //    pw.incPos();

        //    //+ -の解析
        //    int shift = 0;
        //    shift = AnaSharp(pw, ref shift);

        //    if (cmd == 'r' && shift != 0)
        //    {
        //        msgBox.setWrnMsg("休符での+、-の指定は無視されます。", pw.getSrcFn(), pw.getLineNumber());
        //    }

        //    int ml = 0;
        //    int n = -1;
        //    bool directFlg = false;
        //    bool isMinus = false;
        //    bool isTieType2 = false;
        //    bool isSecond = false;
        //    do
        //    {
        //        int m = 0;

        //        //数値の解析
        //        //if (!pw.getNum(out n))
        //        if (!pw.getNumNoteLength(out n, out directFlg))
        //        {
        //            if (!isSecond)
        //                n = (int)pw.length;
        //            else if (!isMinus)
        //            {
        //                if (!isTieType2)
        //                {
        //                    //タイとして'&'が使用されている
        //                    pw.tie = true;
        //                }
        //                else
        //                {
        //                    n = (int)pw.length;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            if (n == 0)
        //            {
        //                if (pw.Type != enmChannelType.FMOPM
        //                    && pw.Type != enmChannelType.FMOPN
        //                    && pw.Type != enmChannelType.FMOPNex
        //                    )
        //                {
        //                    msgBox.setErrMsg("Tone Doublerが使用できるのはFM音源のみです。", pw.getSrcFn(), pw.getLineNumber());
        //                    return;
        //                }
        //                pw.TdA = pw.octaveNew * 12 + Const.NOTE.IndexOf(cmd) + shift + pw.keyShift;
        //                pw.octaveNow = pw.octaveNew;

        //                return;
        //            }

        //            if (!directFlg)
        //            {
        //                if ((int)info.clockCount % n != 0)
        //                {
        //                    msgBox.setWrnMsg(string.Format("割り切れない音長({0})の指定があります。音長は不定になります。", n), pw.getSrcFn(), pw.getLineNumber());
        //                }
        //                n = (int)info.clockCount / n;
        //            }
        //            else
        //            {
        //                n = Common.CheckRange(n, 1, 65535);
        //            }
        //        }

        //        //Tone Doubler
        //        if (pw.getChar() == ',')
        //        {
        //            if (pw.Type != enmChannelType.FMOPM
        //                && pw.Type != enmChannelType.FMOPN
        //                && pw.Type != enmChannelType.FMOPNex
        //                )
        //            {
        //                msgBox.setErrMsg("Tone Doublerが使用できるのはFM音源のみです。", pw.getSrcFn(), pw.getLineNumber());
        //                return;
        //            }
        //            pw.TdA = pw.octaveNew * 12 + Const.NOTE.IndexOf(cmd) + shift + pw.keyShift;
        //            pw.octaveNow = pw.octaveNew;
        //            pw.incPos();

        //            return;
        //        }

        //        if (!pw.tie || isTieType2)
        //        {
        //            m += n;

        //            //符点の解析
        //            while (pw.getChar() == '.')
        //            {
        //                if (n % 2 != 0)
        //                {
        //                    msgBox.setWrnMsg("割り切れない.の指定があります。音長は不定です。", pw.getSrcFn(), pw.getLineNumber());
        //                }
        //                n = n / 2;
        //                m += n;
        //                pw.incPos();
        //            }


        //            if (isMinus) ml -= m;
        //            else ml += m;
        //        }

        //        isTieType2 = false;

        //        //ベンドの解析
        //        int bendDelayCounter = 0;
        //        int bendShift = 0;
        //        if (pw.getChar() == '_')
        //        {
        //            pw.incPos();
        //            pw.octaveNow = pw.octaveNew;
        //            pw.bendOctave = pw.octaveNow;
        //            pw.bendNote = 'r';
        //            pw.bendWaitCounter = -1;
        //            bool loop = true;
        //            while (loop)
        //            {
        //                char bCmd = pw.getChar();
        //                switch (bCmd)
        //                {
        //                    case 'c':
        //                    case 'd':
        //                    case 'e':
        //                    case 'f':
        //                    case 'g':
        //                    case 'a':
        //                    case 'b':
        //                        loop = false;
        //                        pw.incPos();
        //                        //+ -の解析
        //                        bendShift = 0;
        //                        bendShift = AnaSharp(pw, ref bendShift);
        //                        pw.bendShift = bendShift;
        //                        bendDelayCounter = 0;
        //                        n = -1;
        //                        isMinus = false;
        //                        isTieType2 = false;
        //                        isSecond = false;
        //                        do
        //                        {
        //                            m = 0;

        //                            //数値の解析
        //                            //if (!pw.getNum(out n))
        //                            if (!pw.getNumNoteLength(out n, out directFlg))
        //                            {
        //                                if (!isSecond)
        //                                {
        //                                    n = 0;
        //                                    break;
        //                                }
        //                                else if (!isMinus)
        //                                {
        //                                    if (!isTieType2)
        //                                    {
        //                                        //タイとして'&'が使用されている
        //                                        pw.tie = true;
        //                                    }
        //                                    else
        //                                    {
        //                                        n = (int)pw.length;
        //                                    }
        //                                    break;
        //                                }
        //                            }
        //                            else
        //                            {
        //                                if (!directFlg)
        //                                {
        //                                    if ((int)info.clockCount % n != 0)
        //                                    {
        //                                        msgBox.setWrnMsg(string.Format("割り切れない音長({0})の指定があります。音長は不定になります。", n), pw.getSrcFn(), pw.getLineNumber());
        //                                    }
        //                                    n = (int)info.clockCount / n;
        //                                }
        //                                else
        //                                {
        //                                    n = Common.CheckRange(n, 1, 65535);
        //                                }
        //                            }

        //                            if (!pw.tie || isTieType2)
        //                            {
        //                                bendDelayCounter += n;

        //                                //符点の解析
        //                                while (pw.getChar() == '.')
        //                                {
        //                                    if (n % 2 != 0)
        //                                    {
        //                                        msgBox.setWrnMsg("割り切れない.の指定があります。音長は不定です。", pw.getSrcFn(), pw.getLineNumber());
        //                                    }
        //                                    n = n / 2;
        //                                    m += n;
        //                                    pw.incPos();
        //                                }


        //                                if (isMinus) bendDelayCounter -= m;
        //                                else bendDelayCounter += m;
        //                            }

        //                            isTieType2 = false;

        //                            if (pw.getChar() == '&')
        //                            {
        //                                isMinus = false;
        //                                isTieType2 = false;
        //                            }
        //                            else if (pw.getChar() == '^')
        //                            {
        //                                isMinus = false;
        //                                isTieType2 = true;
        //                            }
        //                            else if (pw.getChar() == '~')
        //                            {
        //                                isMinus = true;
        //                            }
        //                            else
        //                            {
        //                                break;
        //                            }

        //                            isSecond = true;
        //                            pw.incPos();

        //                        } while (true);

        //                        if (cmd != 'r')
        //                        {
        //                            pw.bendNote = bCmd;
        //                            bendDelayCounter = Common.CheckRange(bendDelayCounter, 0, ml);
        //                            pw.bendWaitCounter = bendDelayCounter;
        //                        }
        //                        else
        //                        {
        //                            msgBox.setErrMsg("休符にベンドの指定はできません。", pw.getSrcFn(), pw.getLineNumber());
        //                        }

        //                        break;
        //                    case 'o':
        //                        pw.incPos();
        //                        if (!pw.getNum(out n))
        //                        {
        //                            msgBox.setErrMsg("不正なオクターブが指定されています。", pw.getSrcFn(), pw.getLineNumber());
        //                            n = 110;
        //                        }
        //                        n = Common.CheckRange(n, 1, 8);
        //                        pw.bendOctave = n;
        //                        break;
        //                    case '>':
        //                        pw.incPos();
        //                        pw.bendOctave += info.octaveRev ? -1 : 1;
        //                        pw.bendOctave = Common.CheckRange(pw.bendOctave, 1, 8);
        //                        break;
        //                    case '<':
        //                        pw.incPos();
        //                        pw.bendOctave += info.octaveRev ? 1 : -1;
        //                        pw.bendOctave = Common.CheckRange(pw.bendOctave, 1, 8);
        //                        break;
        //                    default:
        //                        loop = false;
        //                        break;
        //                }
        //            }

        //            //音符の変化量
        //            int ed = Const.NOTE.IndexOf(pw.bendNote) + 1 + (pw.bendOctave - 1) * 12 + pw.bendShift;
        //            ed = Common.CheckRange(ed, 0, 8 * 12 - 1);
        //            int st = Const.NOTE.IndexOf(cmd) + 1 + (pw.octaveNow - 1) * 12 + shift;//
        //            st = Common.CheckRange(st, 0, 8 * 12 - 1);

        //            int delta = ed - st;
        //            if (delta == 0 || bendDelayCounter == ml)
        //            {
        //                pw.bendNote = 'r';
        //                pw.bendWaitCounter = -1;
        //            }
        //            else
        //            {

        //                //１音符当たりのウエイト
        //                float wait = (ml - bendDelayCounter - 1) / (float)delta;
        //                float tl = 0;
        //                float bf = Math.Sign(wait);
        //                List<int> lstBend = new List<int>();
        //                int toneDoublerShift = GetToneDoublerShift(pw, pw.octaveNow, cmd, shift);
        //                for (int i = 0; i < Math.Abs(delta); i++)
        //                {
        //                    bf += wait;
        //                    tl += wait;
        //                    int a = ((SN76489)pw.chip).GetDcsgFNum(pw.octaveNow, cmd, shift + (i + 0) * Math.Sign(delta));//
        //                    int b = ((SN76489)pw.chip).GetDcsgFNum(pw.octaveNow, cmd, shift + (i + 1) * Math.Sign(delta));//
        //                    if (pw.chip is YM2151)
        //                    {
        //                        a = ((YM2151)pw.chip).GetFNum(pw.octaveNow, cmd, shift + (i + 0) * Math.Sign(delta) + toneDoublerShift);//
        //                        b = ((YM2151)pw.chip).GetFNum(pw.octaveNow, cmd, shift + (i + 1) * Math.Sign(delta) + toneDoublerShift);//
        //                    }
        //                    else if (
        //                        (pw.chip is YM2608 && (pw.Type == enmChannelType.FMOPN || pw.Type == enmChannelType.FMOPNex))
        //                        || (pw.chip is YM2610B && (pw.Type == enmChannelType.FMOPN || pw.Type == enmChannelType.FMOPNex))
        //                        || (pw.chip is YM2612)
        //                        || (pw.chip is YM2612X)
        //                        )
        //                    {
        //                        //int[] ftbl = ((pw.chip is YM2612) || (pw.chip is YM2612X)) ? OPN_FNumTbl_7670454 : pw.chip.FNumTbl[0];
        //                        int[] ftbl = pw.chip.FNumTbl[0];

        //                        a = ((ClsOPN)pw.chip).GetFmFNum(ftbl, pw.octaveNow, cmd, shift + (i + 0) * Math.Sign(delta) + toneDoublerShift);//
        //                        b = ((ClsOPN)pw.chip).GetFmFNum(ftbl, pw.octaveNow, cmd, shift + (i + 1) * Math.Sign(delta) + toneDoublerShift);//
        //                        int oa = (a & 0xf000) / 0x1000;
        //                        int ob = (b & 0xf000) / 0x1000;
        //                        if (oa != ob)
        //                        {
        //                            if ((a & 0xfff) == ftbl[0])
        //                            {
        //                                oa += Math.Sign(ob - oa);
        //                                a = (a & 0xfff) * 2 + oa * 0x1000;
        //                            }
        //                            else if ((b & 0xfff) == ftbl[0])
        //                            {
        //                                ob += Math.Sign(oa - ob);
        //                                b = (b & 0xfff) * ((delta > 0) ? 2 : 1) + ob * 0x1000;
        //                            }
        //                        }
        //                    }
        //                    else if (
        //                        (pw.chip is YM2608 && pw.Type == enmChannelType.SSG)
        //                        || (pw.chip is YM2610B && pw.Type == enmChannelType.SSG)
        //                        )
        //                    {
        //                        a = ((ClsOPN)pw.chip).GetSsgFNum(pw, pw.octaveNow, cmd, shift + (i + 0) * Math.Sign(delta));//
        //                        b = ((ClsOPN)pw.chip).GetSsgFNum(pw, pw.octaveNow, cmd, shift + (i + 1) * Math.Sign(delta));//
        //                    }
        //                    else if (pw.chip is SN76489)
        //                    {
        //                        a = ((SN76489)pw.chip).GetDcsgFNum(pw.octaveNow, cmd, shift + (i + 0) * Math.Sign(delta));//
        //                        b = ((SN76489)pw.chip).GetDcsgFNum(pw.octaveNow, cmd, shift + (i + 1) * Math.Sign(delta));//
        //                    }
        //                    else if (pw.chip is RF5C164)
        //                    {
        //                        a = ((RF5C164)pw.chip).GetRf5c164PcmNote(pw.octaveNow, cmd, shift + (i + 0) * Math.Sign(delta));//
        //                        b = ((RF5C164)pw.chip).GetRf5c164PcmNote(pw.octaveNow, cmd, shift + (i + 1) * Math.Sign(delta));//
        //                    }
        //                    else if (pw.chip is segaPcm)
        //                    {
        //                        a = ((segaPcm)pw.chip).GetSegaPcmFNum(pw.octaveNow, cmd, shift + (i + 0) * Math.Sign(delta));//
        //                        b = ((segaPcm)pw.chip).GetSegaPcmFNum(pw.octaveNow, cmd, shift + (i + 1) * Math.Sign(delta));//
        //                    }
        //                    else if (pw.chip is HuC6280)
        //                    {
        //                        a = ((HuC6280)pw.chip).GetHuC6280Freq(pw.octaveNow, cmd, shift + (i + 0) * Math.Sign(delta));//
        //                        b = ((HuC6280)pw.chip).GetHuC6280Freq(pw.octaveNow, cmd, shift + (i + 1) * Math.Sign(delta));//
        //                    }

        //                    if (Math.Abs(bf) >= 1.0f)
        //                    {
        //                        for (int j = 0; j < (int)Math.Abs(bf); j++)
        //                        {
        //                            int c = b - a;
        //                            int d = (int)Math.Abs(bf);
        //                            lstBend.Add((int)(a + ((float)c / (float)d) * (float)j));
        //                        }
        //                        bf -= (int)bf;
        //                    }

        //                }
        //                Stack<Tuple<int, int>> lb = new Stack<Tuple<int, int>>();
        //                int of = -1;
        //                int cnt = 1;
        //                foreach (int f in lstBend)
        //                {
        //                    if (of == f)
        //                    {
        //                        cnt++;
        //                        continue;
        //                    }
        //                    lb.Push(new Tuple<int, int>(f, cnt));
        //                    of = f;
        //                    cnt = 1;
        //                }
        //                pw.bendList = new Stack<Tuple<int, int>>();
        //                foreach (Tuple<int, int> lbt in lb)
        //                {
        //                    pw.bendList.Push(lbt);
        //                }
        //                Tuple<int, int> t = pw.bendList.Pop();
        //                pw.bendFnum = t.Item1;
        //                pw.bendWaitCounter = t.Item2;
        //            }
        //        }

        //        if (pw.getChar() == '&')
        //        {
        //            isMinus = false;
        //            isTieType2 = false;
        //        }
        //        else if (pw.getChar() == '^')
        //        {
        //            isMinus = false;
        //            isTieType2 = true;
        //        }
        //        else if (pw.getChar() == '~')
        //        {
        //            isMinus = true;
        //        }
        //        else
        //        {
        //            break;
        //        }

        //        isSecond = true;
        //        pw.incPos();

        //    } while (true);

        //    if (ml < 1)
        //    {
        //        msgBox.setErrMsg("負の音長が指定されました。", pw.getSrcFn(), pw.getLineNumber());
        //        ml = (int)pw.length;
        //    }


        //    if (pw.renpuFlg)
        //    {
        //        if (pw.lstRenpuLength!=null && pw.lstRenpuLength.Count > 0)
        //        {
        //            ml = pw.lstRenpuLength[0];
        //            pw.lstRenpuLength.RemoveAt(0);
        //        }
        //    }


        //    //装飾の解析完了



        //    //WaitClockの決定
        //    pw.waitCounter = ml;

        //    if (cmd != 'r')
        //    {
        //        if (pw.reqFreqReset)
        //        {
        //            pw.freq = -1;
        //            pw.reqFreqReset = false;
        //        }

        //        //発音周波数
        //        if (pw.bendWaitCounter == -1)
        //        {
        //            pw.octaveNow = pw.octaveNew;
        //            pw.noteCmd = cmd;
        //            pw.shift = shift;

        //            //Tone Doubler
        //            SetToneDoubler(pw);
        //        }
        //        else
        //        {
        //            pw.octaveNow = pw.octaveNew;
        //            pw.noteCmd = cmd;
        //            pw.shift = shift;

        //            //Tone Doubler
        //            SetToneDoubler(pw);

        //            pw.octaveNew = pw.bendOctave;//
        //            pw.octaveNow = pw.bendOctave;//
        //            pw.noteCmd = pw.bendNote;
        //            pw.shift = pw.bendShift;
        //        }

        //        //強制設定
        //        pw.freq = -1;

        //        //発音周波数の決定とキーオン
        //        if (pw.chip is YM2151)
        //        {

        //            ((YM2151)pw.chip).SetFNum(pw);

        //            //タイ指定では無い場合はキーオンする
        //            if (!pw.beforeTie)
        //            {
        //                SetLfoAtKeyOn(pw);
        //                ((YM2151)pw.chip).SetVolume(pw);
        //                ((YM2151)pw.chip).OutKeyOn(pw);
        //            }
        //        }
        //        else if (pw.chip is YM2203)
        //        {

        //            //YM2203

        //            if (pw.Type == enmChannelType.FMOPN || pw.Type == enmChannelType.FMOPNex)
        //            {
        //                ((ClsOPN)pw.chip).SetFmFNum(pw);

        //                //タイ指定では無い場合はキーオンする
        //                if (!pw.beforeTie)
        //                {
        //                    SetLfoAtKeyOn(pw);
        //                    ((ClsOPN)pw.chip).SetFmVolume(pw);
        //                    ((ClsOPN)pw.chip).OutFmKeyOn(pw);
        //                }
        //            }
        //            else if (pw.Type == enmChannelType.SSG)
        //            {
        //                ((ClsOPN)pw.chip).SetSsgFNum(pw);

        //                //タイ指定では無い場合はキーオンする
        //                if (!pw.beforeTie)
        //                {
        //                    SetEnvelopeAtKeyOn(pw);
        //                    SetLfoAtKeyOn(pw);
        //                    ((ClsOPN)pw.chip).OutSsgKeyOn(pw);
        //                }
        //            }
        //        }
        //        else if (pw.chip is YM2608)
        //        {

        //            //YM2608

        //            if (pw.Type == enmChannelType.FMOPN || pw.Type == enmChannelType.FMOPNex)
        //            {
        //                ((ClsOPN)pw.chip).SetFmFNum(pw);

        //                //タイ指定では無い場合はキーオンする
        //                if (!pw.beforeTie)
        //                {
        //                    SetLfoAtKeyOn(pw);
        //                    ((ClsOPN)pw.chip).SetFmVolume(pw);
        //                    ((ClsOPN)pw.chip).OutFmKeyOn(pw);
        //                }
        //            }
        //            else if (pw.Type == enmChannelType.SSG)
        //            {
        //                ((ClsOPN)pw.chip).SetSsgFNum(pw);

        //                //タイ指定では無い場合はキーオンする
        //                if (!pw.beforeTie)
        //                {
        //                    SetEnvelopeAtKeyOn(pw);
        //                    SetLfoAtKeyOn(pw);
        //                    ((ClsOPN)pw.chip).OutSsgKeyOn(pw);
        //                }
        //            }
        //            else if (pw.Type == enmChannelType.RHYTHM)
        //            {
        //                if (!pw.beforeTie)
        //                {
        //                    pw.keyOn = true;
        //                }
        //            }
        //            else if (pw.Type == enmChannelType.ADPCM)
        //            {
        //                ((YM2608)pw.chip).SetAdpcmFNum(pw);

        //                //タイ指定では無い場合はキーオンする
        //                if (!pw.beforeTie)
        //                {
        //                    SetEnvelopeAtKeyOn(pw);
        //                    SetLfoAtKeyOn(pw);
        //                    ((YM2608)pw.chip).OutAdpcmKeyOn(pw);
        //                }
        //            }
        //        }
        //        else if (pw.chip is YM2610B)
        //        {

        //            //YM2610B

        //            if (pw.Type == enmChannelType.FMOPN || pw.Type == enmChannelType.FMOPNex)
        //            {
        //                ((ClsOPN)pw.chip).SetFmFNum(pw);

        //                //タイ指定では無い場合はキーオンする
        //                if (!pw.beforeTie)
        //                {
        //                    SetLfoAtKeyOn(pw);
        //                    ((ClsOPN)pw.chip).SetFmVolume(pw);
        //                    ((ClsOPN)pw.chip).OutFmKeyOn(pw);
        //                }
        //            }
        //            else if (pw.Type == enmChannelType.SSG)
        //            {
        //                ((ClsOPN)pw.chip).SetSsgFNum(pw);

        //                //タイ指定では無い場合はキーオンする
        //                if (!pw.beforeTie)
        //                {
        //                    SetEnvelopeAtKeyOn(pw);
        //                    SetLfoAtKeyOn(pw);
        //                    ((ClsOPN)pw.chip).OutSsgKeyOn(pw);
        //                }
        //            }
        //            else if (pw.Type == enmChannelType.ADPCMA)
        //            {
        //                if (!pw.beforeTie)
        //                {

        //                    pw.keyOn = true;
        //                    //if ((((YM2610B)pw.chip).adpcmA_KeyOn & (1 << (pw.ch - 12))) != 0)
        //                    //{
        //                    //    ((YM2610B)pw.chip).adpcmA_KeyOff |= (byte)(1 << (pw.ch - 12));
        //                    //    ((YM2610B)pw.chip).adpcmA_beforeKeyOn &= (byte)~(1 << (pw.ch - 12));
        //                    //}
        //                    //((YM2610B)pw.chip).adpcmA_KeyOn |= (byte)(1 << (pw.ch - 12));

        //                }
        //            }
        //            else if (pw.Type == enmChannelType.ADPCMB)
        //            {
        //                ((YM2610B)pw.chip).SetAdpcmBFNum(pw);

        //                //タイ指定では無い場合はキーオンする
        //                if (!pw.beforeTie)
        //                {
        //                    SetEnvelopeAtKeyOn(pw);
        //                    SetLfoAtKeyOn(pw);
        //                    ((YM2610B)pw.chip).OutAdpcmBKeyOn(pw);
        //                }
        //            }
        //        }
        //        else if (pw.chip is YM2612)
        //        {

        //            //YM2612

        //            if (!pw.pcm)
        //            {
        //                ((ClsOPN)pw.chip).SetFmFNum(pw);
        //            }
        //            else
        //            {
        //                ((YM2612)pw.chip).GetPcmNote(pw);
        //            }
        //            //タイ指定では無い場合はキーオンする
        //            if (!pw.beforeTie)
        //            {
        //                SetLfoAtKeyOn(pw);
        //                ((ClsOPN)pw.chip).SetFmVolume(pw);
        //                ((ClsOPN)pw.chip).OutFmKeyOn(pw);
        //            }
        //        }
        //        else if (pw.chip is YM2612X)
        //        {

        //            //YM2612X

        //            if (!pw.pcm)
        //            {
        //                ((ClsOPN)pw.chip).SetFmFNum(pw);
        //            }
        //            else
        //            {
        //                ((YM2612X)pw.chip).GetPcmNote(pw);
        //            }
        //            //タイ指定では無い場合はキーオンする
        //            if (!pw.beforeTie)
        //            {
        //                SetLfoAtKeyOn(pw);
        //                ((ClsOPN)pw.chip).SetFmVolume(pw);
        //                ((ClsOPN)pw.chip).OutFmKeyOn(pw);
        //            }
        //        }
        //        else if (pw.chip is SN76489)
        //        {

        //            // SN76489

        //            ((SN76489)pw.chip).SetDcsgFNum(pw);

        //            //タイ指定では無い場合はキーオンする
        //            if (!pw.beforeTie)
        //            {
        //                SetEnvelopeAtKeyOn(pw);
        //                SetLfoAtKeyOn(pw);
        //                ((SN76489)pw.chip).OutPsgKeyOn(pw);
        //            }
        //        }
        //        else if (pw.chip is RF5C164)
        //        {

        //            // RF5C164

        //            ((RF5C164)pw.chip).SetRf5c164FNum(pw);

        //            //タイ指定では無い場合はキーオンする
        //            if (!pw.beforeTie)
        //            {
        //                SetEnvelopeAtKeyOn(pw);
        //                SetLfoAtKeyOn(pw);
        //                ((RF5C164)pw.chip).SetRf5c164Envelope(pw, pw.volume);
        //                ((RF5C164)pw.chip).OutRf5c164KeyOn(pw);
        //            }
        //        }
        //        else if (pw.chip is segaPcm)
        //        {
        //            ((segaPcm)pw.chip).SetSegaPcmFNum(pw);

        //            if (!pw.beforeTie)
        //            {
        //                SetEnvelopeAtKeyOn(pw);
        //                SetLfoAtKeyOn(pw);
        //                ((segaPcm)pw.chip).OutSegaPcmKeyOn(pw);
        //            }
        //        }
        //        else if (pw.chip is HuC6280)
        //        {

        //            // HuC6280

        //            ((HuC6280)pw.chip).SetHuC6280FNum(pw);

        //            //タイ指定では無い場合はキーオンする
        //            if (!pw.beforeTie)
        //            {
        //                SetEnvelopeAtKeyOn(pw);
        //                SetLfoAtKeyOn(pw);
        //                ((HuC6280)pw.chip).SetHuC6280Envelope(pw, pw.volume);
        //                ((HuC6280)pw.chip).OutHuC6280KeyOn(pw);
        //            }
        //        }

        //        //gateTimeの決定
        //        if (pw.gatetimePmode)
        //        {
        //            pw.waitKeyOnCounter = pw.waitCounter * pw.gatetime / 8L;
        //        }
        //        else
        //        {
        //            pw.waitKeyOnCounter = pw.waitCounter - pw.gatetime;
        //        }
        //        if (pw.waitKeyOnCounter < 1) pw.waitKeyOnCounter = 1;

        //        //PCM専用のWaitClockの決定
        //        if (pw.pcm)
        //        {
        //            pw.pcmWaitKeyOnCounter = -1;
        //            if (info.Version == 1.51f)
        //            {
        //                pw.pcmWaitKeyOnCounter = pw.waitKeyOnCounter;
        //            }

        //            if (instPCM == null || instPCM.Count - 1 < pw.instrument)
        //            {
        //                pw.pcmSizeCounter = 0;
        //            }
        //            else
        //            {
        //                pw.pcmSizeCounter = instPCM[pw.instrument].size;
        //            }

        //        }
        //    }
        //    else
        //    {
        //        if (pw.reqKeyOffReset)
        //        {
        //            if (pw.chip is SN76489)
        //            {
        //                ((SN76489)pw.chip).OutPsgKeyOff(pw);
        //                pw.reqKeyOffReset = false;
        //            }
        //        }
        //    }

        //    pw.clockCounter += pw.waitCounter;
        //}

        //private static int AnaSharp(partWork pw, ref int shift)
        //{
        //    while (pw.getChar() == '+' || pw.getChar() == '-')
        //    {
        //        shift += pw.getChar() == '+' ? 1 : -1;
        //        pw.incPos();
        //    }

        //    return shift;
        //}

        //private void CmdRepeatExit(partWork pw)
        //{
        //    int n = -1;
        //    pw.incPos();
        //    clsRepeat rx = pw.stackRepeat.Pop();
        //    if (rx.repeatCount == 1)
        //    {
        //        int i = 0;
        //        while (true)
        //        {
        //            char c = pw.getChar();
        //            if (c == ']')
        //            {
        //                if (i == 0)
        //                {
        //                    break;
        //                }
        //                else
        //                    i--;
        //            }
        //            else if (c == '[')
        //            {
        //                i++;
        //            }
        //            pw.incPos();
        //        }
        //        pw.incPos();
        //        pw.getNum(out n);
        //    }
        //    else
        //    {
        //        pw.stackRepeat.Push(rx);
        //    }

        //}

        //private void CmdRepeatEnd(partWork pw)
        //{
        //    pw.incPos();
        //    if (!pw.getNum(out int n))
        //    {
        //        n = 2;
        //    }
        //    n = Common.CheckRange(n, 1, 255);
        //    try
        //    {
        //        clsRepeat re = pw.stackRepeat.Pop();
        //        if (re.repeatCount == -1)
        //        {
        //            //初回
        //            re.repeatCount = n;
        //        }
        //        re.repeatCount--;
        //        if (re.repeatCount > 0)
        //        {
        //            pw.stackRepeat.Push(re);
        //            pw.setPos(re.pos);
        //        }
        //    }
        //    catch
        //    {
        //        msgBox.setWrnMsg("[と]の数があいません。", pw.getSrcFn(), pw.getLineNumber());
        //    }
        //}

        //private void CmdRepeatStart(partWork pw)
        //{
        //    pw.incPos();
        //    clsRepeat rs = new clsRepeat()
        //    {
        //        pos = pw.getPos(),
        //        repeatCount = -1//初期値
        //    };
        //    pw.stackRepeat.Push(rs);
        //}

        //private void CmdRenpuStart(partWork pw)
        //{
        //    if (!pw.renpuFlg)
        //    {
        //        //MML解析
        //        List<Tuple<int, string>> lstRenpu = new List<Tuple<int, string>>();
        //        List<int> lstRenpuLength = new List<int>();
        //        int nest = 0;
        //        int pos = 0;
        //        pw.incPos();
        //        pos = pw.getPos();
        //        DecStep1Renpu(pw, lstRenpu, nest);
        //        if (lstRenpu.Count < 1)
        //        {
        //            msgBox.setErrMsg(
        //                "連符コマンドの解析に失敗しました。動作は不定となります。"
        //                , pw.getSrcFn(), pw.getLineNumber());
        //            return;
        //        }
        //        DecStep2Renpu(lstRenpu, lstRenpuLength, 1, 0);

        //        pw.setPos(pos);
        //        pw.renpuFlg = true;
        //        pw.lstRenpuLength = lstRenpuLength;
        //    }
        //    else
        //    {
        //        pw.incPos();
        //    }
        //}

        //private void CmdRenpuEnd(partWork pw)
        //{
        //    if(pw.renpuFlg && pw.lstRenpuLength.Count == 0)
        //    {
        //        pw.renpuFlg = false;
        //        pw.lstRenpuLength = null;
        //    }
        //    pw.incPos();

        //    //数値指定のスキップ
        //    pw.getNumNoteLength(out int n, out bool directFlg);

        //}


        //private bool SkipCommander(partWork pw,char ch)
        //{

        //    //true カウント
        //    //false 無視
        //    switch (ch)
        //    {
        //        case ' ':
        //        case '\t':
        //        case '!': // CompileSkip
        //        case '>': // octave Up
        //        case '<': // octave Down
        //        case 'L': // loop point
        //            pw.incPos();
        //            break;
        //        case 'T': // tempo
        //        case 'v': // volume
        //        case 'o': // octave
        //        case ')': // volume Up
        //        case '(': // volume Down
        //        case 'l': // length
        //        case '#': // length(clock)
        //        case 'D': // Detune
        //        case 'q': // gatetime
        //        case 'Q': // gatetime
        //        case 'K': // key shift
        //            pw.incPos();
        //            pw.getNum(out int n);
        //            break;
        //        case '@': // instrument
        //            SkipCmdInstrument(pw, ch);
        //            break;
        //        case 'V': // totalVolume(Adpcm-A / Rhythm)
        //            SkipCmdTotalVolume(pw);
        //            break;
        //        case 'p': // pan
        //            SkipCmdPan(pw);
        //            break;
        //        case 'm': // pcm mode
        //            SkipCmdMode(pw);
        //            break;
        //        case 'E': // envelope
        //            SkipCmdEnvelope(pw);
        //            break;
        //        //case '[': // repeat
        //        //case ']': // repeat
        //        //case '/': // repeat
        //        case 'M': // lfo
        //            SkipCmdLfo(pw);
        //            break;
        //        case 'S': // lfo switch
        //            SkipCmdLfoSwitch(pw);
        //            break;
        //        case 'y': // y 
        //            SkipCmdY(pw);
        //            break;
        //        case 'w': // noise
        //            SkipCmdNoise(pw);
        //            break;
        //        case 'P': // noise or tone mixer
        //            SkipCmdMixer(pw);
        //            break;
        //        case 'c':
        //        case 'd':
        //        case 'e':
        //        case 'f':
        //        case 'g':
        //        case 'a':
        //        case 'b':
        //        case 'r':
        //            SkipCmdNote(pw, ch);
        //            return true;
        //        default:
        //            msgBox.setErrMsg(
        //                string.Format("連符コマンドの中で未対応のコマンド({0})が使用されています。動作は不定となります。", ch)
        //                , pw.getSrcFn(), pw.getLineNumber());
        //            pw.incPos();
        //            break;
        //    }

        //    return false;
        //}

        //private void SkipCmdNoise(partWork pw)
        //{
        //    int n = -1;
        //    pw.incPos();

        //    if (pw.Type == enmChannelType.DCSGNOISE 
        //        || pw.Type == enmChannelType.SSG 
        //        || (pw.chip is YM2151) 
        //        || (pw.chip is HuC6280 && pw.ch > 3))
        //    {
        //        pw.getNum(out n);
        //    }
        //}

        //private void SkipCmdMixer(partWork pw)
        //{
        //    int n = -1;
        //    pw.incPos();

        //    if ((pw.Type == enmChannelType.SSG && (
        //        (pw.chip is YM2203) || (pw.chip is YM2608) || (pw.chip is YM2610B)
        //        ))
        //        || (pw.chip is YM2151)
        //        || (pw.chip is HuC6280 && pw.ch > 3)
        //        )
        //    {
        //        pw.getNum(out n);
        //    }
        //}

        //private void SkipCmdY(partWork pw)
        //{
        //    int n = -1;
        //    pw.incPos();

        //    char c = pw.getChar();
        //    if (c >= 'A' && c <= 'Z')
        //    {
        //        string toneparamName = "" + c;
        //        pw.incPos();
        //        toneparamName += pw.getChar();
        //        pw.incPos();
        //        if (toneparamName != "TL" && toneparamName != "SR")
        //        {
        //            toneparamName += pw.getChar();
        //            pw.incPos();
        //            if (toneparamName != "SSG")
        //            {
        //                toneparamName += pw.getChar();
        //                pw.incPos();
        //            }
        //        }

        //        if (toneparamName == "DT1M" || toneparamName == "DT2S" || toneparamName == "PMSA")
        //        {
        //            toneparamName += pw.getChar();
        //            pw.incPos();
        //            if (toneparamName == "PMSAM")
        //            {
        //                toneparamName += pw.getChar();
        //                pw.incPos();
        //            }
        //        }

        //        pw.incPos();

        //        if (toneparamName != "FBAL" && toneparamName != "PMSAMS")
        //        {
        //            pw.getNum(out n);
        //            pw.incPos();
        //        }

        //        pw.getNum(out n);

        //        return;
        //    }

        //    pw.getNum(out n);
        //    pw.incPos();
        //    pw.getNum(out n);

        //}

        //private void SkipCmdLfo(partWork pw)
        //{
        //    pw.incPos();
        //    char c = pw.getChar();
        //    if (c == 'A')
        //    {
        //        pw.incPos();
        //        if (pw.getChar() == 'M')
        //        {
        //            pw.incPos();
        //            if (pw.getChar() == 'S') SkipCmdMAMS_PMS(pw);
        //        }
        //        return;
        //    }
        //    if (c < 'P' && c > 'S') return;
        //    pw.incPos();
        //    char t = pw.getChar();
        //    if (c == 'P' && t == 'M')
        //    {
        //        pw.incPos();
        //        if (pw.getChar() == 'S') SkipCmdMAMS_PMS(pw);
        //        return;
        //    }
        //    if (t != 'T' && t != 'V' && t != 'H') return;
        //    int n = -1;
        //    do
        //    {
        //        pw.incPos();
        //        if (!pw.getNum(out n)) return;
        //        while (pw.getChar() == '\t' || pw.getChar() == ' ') pw.incPos(); 
        //    } while (pw.getChar() == ',');
        //}

        //private void SkipCmdLfoSwitch(partWork pw)
        //{
        //    pw.incPos();
        //    if (pw.getChar() < 'P' || pw.getChar() > 'S')
        //    {
        //        pw.incPos();
        //        return;
        //    }
        //    int n = -1;
        //    pw.incPos();
        //    if (!pw.getNum(out n)) return;
        //}

        //private void SkipCmdMAMS_PMS(partWork pw)
        //{
        //    if (!((pw.chip is YM2151) || (pw.chip is YM2608) || (pw.chip is YM2610B) || (pw.chip is YM2612) || (pw.chip is YM2612X)
        //        || (pw.Type == enmChannelType.FMOPM) || (pw.Type == enmChannelType.FMOPN)))
        //        return;

        //    int n = -1;
        //    pw.incPos();
        //    pw.getNum(out n);
        //}

        //private void SkipCmdTotalVolume(partWork pw)
        //{
        //    pw.incPos();
        //    pw.getNum(out int n);
        //    if (pw.chip is HuC6280)
        //    {
        //        pw.incPos();
        //        pw.getNum(out n);
        //    }
        //}

        //private void SkipCmdInstrument(partWork pw, char cmd)
        //{
        //    int n;
        //    pw.incPos();

        //    if (pw.getChar() == 'T')
        //    {
        //        if (pw.Type != enmChannelType.FMOPM && pw.Type != enmChannelType.FMOPN && pw.Type != enmChannelType.FMOPNex)
        //        {
        //            pw.incPos();
        //            return;
        //        }
        //        pw.incPos();
        //        pw.getNum(out n);
        //        return;
        //    }

        //    if ((pw.chip is YM2151) || (pw.chip is YM2203))
        //    {
        //        pw.getNum(out n);
        //    }
        //    else if (pw.chip is YM2608)
        //    {
        //        if ((pw.ch < 9) || (pw.Type == enmChannelType.SSG))
        //        {
        //            pw.getNum(out n);
        //        }
        //        else if ((pw.Type == enmChannelType.RHYTHM) || (pw.Type == enmChannelType.ADPCM))
        //        {
        //            if (pw.getChar() != 'E')
        //            {
        //                pw.getNum(out n);
        //            }
        //            else
        //            {
        //                pw.incPos();
        //                pw.getNum(out n);
        //            }
        //        }
        //    }
        //    else if (pw.chip is YM2610B)
        //    {
        //        if ((pw.ch < 9) || (pw.Type == enmChannelType.SSG))
        //        {
        //            pw.getNum(out n);
        //        }
        //        else if ((pw.Type == enmChannelType.ADPCMA) || (pw.Type == enmChannelType.ADPCMB))
        //        {
        //            if (pw.getChar() != 'E')
        //            {
        //                pw.getNum(out n);
        //            }
        //            else
        //            {
        //                pw.incPos();
        //                pw.getNum(out n);
        //            }
        //        }
        //    }
        //    else if ((pw.chip is YM2612) || (pw.chip is YM2612X) || (pw.chip is SN76489))
        //    {
        //        pw.getNum(out n);
        //    }
        //    else if ((pw.chip is RF5C164) || (pw.chip is segaPcm) || (pw.chip is HuC6280))
        //    {
        //        if (pw.getChar() != 'E')
        //        {
        //            pw.getNum(out n);
        //        }
        //        else
        //        {
        //            pw.incPos();
        //            pw.getNum(out n);
        //        }
        //    }

        //}

        //private void SkipCmdNote(partWork pw, char cmd)
        //{
        //    pw.incPos();

        //    //+ -の解析
        //    int shift = 0;
        //    shift = AnaSharp(pw, ref shift);

        //    int n = -1;
        //    bool directFlg = false;
        //    bool isMinus = false;
        //    bool isTieType2 = false;
        //    bool isSecond = false;
        //    bool pwTie = false;
        //    do
        //    {
        //        //数値の解析
        //        if (!pw.getNumNoteLength(out n, out directFlg))
        //        {
        //            if (!isSecond)
        //                n = (int)pw.length;
        //            else if (!isMinus)
        //            {
        //                if (!isTieType2)
        //                {
        //                    //タイとして'&'が使用されている
        //                    pwTie = true;
        //                }
        //                else
        //                {
        //                    n = (int)pw.length;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            if (n == 0) return;
        //        }

        //        //Tone Doubler
        //        if (pw.getChar() == ',')
        //        {
        //            pw.incPos();
        //            return;
        //        }

        //        if (!pwTie || isTieType2)
        //        {
        //            //符点の解析
        //            while (pw.getChar() == '.') pw.incPos();
        //        }

        //        isTieType2 = false;

        //        //ベンドの解析
        //        if (pw.getChar() == '_')
        //        {
        //            pw.incPos();
        //            bool loop = true;
        //            while (loop)
        //            {
        //                char bCmd = pw.getChar();
        //                switch (bCmd)
        //                {
        //                    case 'c':
        //                    case 'd':
        //                    case 'e':
        //                    case 'f':
        //                    case 'g':
        //                    case 'a':
        //                    case 'b':
        //                        loop = false;
        //                        pw.incPos();
        //                        //+ -の解析
        //                        AnaSharp(pw, ref shift);
        //                        isMinus = false;
        //                        isTieType2 = false;
        //                        isSecond = false;
        //                        do
        //                        {
        //                            //数値の解析
        //                            if (!pw.getNumNoteLength(out n, out directFlg))
        //                            {
        //                                if (!isSecond)
        //                                {
        //                                    break;
        //                                }
        //                                else if (!isMinus)
        //                                {
        //                                    if (!isTieType2)
        //                                    {
        //                                        //タイとして'&'が使用されている
        //                                        pwTie = true;
        //                                    }
        //                                    break;
        //                                }
        //                            }

        //                            if (!pw.tie || isTieType2)
        //                            {
        //                                //符点の解析
        //                                while (pw.getChar() == '.') pw.incPos();
        //                            }

        //                            isTieType2 = false;

        //                            if (pw.getChar() == '&')
        //                            {
        //                                isMinus = false;
        //                                isTieType2 = false;
        //                            }
        //                            else if (pw.getChar() == '^')
        //                            {
        //                                isMinus = false;
        //                                isTieType2 = true;
        //                            }
        //                            else if (pw.getChar() == '~')
        //                            {
        //                                isMinus = true;
        //                            }
        //                            else
        //                            {
        //                                break;
        //                            }

        //                            isSecond = true;
        //                            pw.incPos();

        //                        } while (true);

        //                        break;
        //                    case 'o':
        //                        pw.incPos();
        //                        break;
        //                    case '>':
        //                        pw.incPos();
        //                        break;
        //                    case '<':
        //                        pw.incPos();
        //                        break;
        //                    default:
        //                        loop = false;
        //                        break;
        //                }
        //            }

        //        }

        //        if (pw.getChar() == '&')
        //        {
        //            isMinus = false;
        //            isTieType2 = false;
        //        }
        //        else if (pw.getChar() == '^')
        //        {
        //            isMinus = false;
        //            isTieType2 = true;
        //        }
        //        else if (pw.getChar() == '~')
        //        {
        //            isMinus = true;
        //        }
        //        else
        //        {
        //            break;
        //        }

        //        isSecond = true;
        //        pw.incPos();

        //    } while (true);


        //    //装飾の解析完了


        //}

        //private void SkipCmdPan(partWork pw)
        //{
        //    int n;

        //    pw.incPos();

        //    if ((pw.chip is YM2151) || (pw.chip is YM2608) || (pw.chip is YM2610B)
        //        || (pw.chip is YM2612) || (pw.chip is YM2612X) || (pw.chip is SN76489))
        //    {
        //        pw.getNum(out n);
        //    }
        //    else if ((pw.chip is RF5C164) || (pw.chip is segaPcm) || (pw.chip is HuC6280))
        //    {
        //        pw.getNum(out n);
        //        pw.incPos();
        //        pw.getNum(out n);
        //    }

        //}

        //private void SkipCmdMode(partWork pw)
        //{
        //    pw.incPos();
        //    pw.getNum(out int n);

        //}

        //private void SkipCmdEnvelope(partWork pw)
        //{
        //    int n = -1;
        //    if (
        //        ((pw.chip is YM2203) && (pw.Type == enmChannelType.SSG || pw.Type == enmChannelType.FMOPNex))
        //        || ((pw.chip is YM2608) && (pw.Type == enmChannelType.SSG || pw.Type == enmChannelType.FMOPNex))
        //        || ((pw.chip is YM2610B) && (pw.Type == enmChannelType.SSG || pw.Type == enmChannelType.FMOPNex))
        //        || ((pw.chip is YM2612) && (pw.Type == enmChannelType.FMPCM || pw.Type == enmChannelType.FMOPNex))
        //        || ((pw.chip is YM2612X) && (pw.Type == enmChannelType.FMPCM || pw.Type == enmChannelType.FMOPNex))
        //        || (pw.chip is SN76489)
        //        || (pw.chip is RF5C164)
        //        || (pw.chip is segaPcm)
        //        || (pw.chip is HuC6280)
        //        )
        //    {
        //        pw.incPos();
        //        switch (pw.getChar())
        //        {
        //            case 'O':
        //                pw.incPos();
        //                pw.incPos();
        //                break;
        //            case 'X':
        //                if (
        //                    ((pw.chip is YM2203) && pw.Type == enmChannelType.FMOPNex)
        //                    || ((pw.chip is YM2608) && pw.Type == enmChannelType.FMOPNex)
        //                    || ((pw.chip is YM2610B) && pw.Type == enmChannelType.FMOPNex)
        //                    || pw.chip is YM2612
        //                    || pw.chip is YM2612X
        //                    )
        //                {
        //                    pw.incPos();
        //                    pw.getNum(out n);
        //                }
        //                break;
        //            default:
        //                if (pw.Type == enmChannelType.FMOPNex)
        //                {
        //                    for (int i = 0; i < 4; i++)
        //                    {
        //                        pw.getNum(out n);
        //                        if (i == 3) break;
        //                        pw.incPos();
        //                    }
        //                    break;
        //                }
        //                else
        //                {
        //                    pw.incPos();
        //                }
        //                break;
        //        }
        //    }
        //    else
        //    {
        //        pw.incPos();
        //    }

        //}


        //private void DecStep1Renpu(partWork pw, List<Tuple<int, string>> lstRenpu, int nest)
        //{
        //    nest++;
        //    int count = 0;
        //    string str = "";
        //    char ch;
        //    while ((ch = pw.getChar()) != '}')
        //    {
        //        if (pw.dataEnd) { return; }
        //        if (ch == '{')
        //        {
        //            pw.incPos();
        //            DecStep1Renpu(pw,  lstRenpu, nest);
        //            ch = '*';
        //            str += ch;
        //            count++;
        //        }
        //        else
        //        {
        //            if (SkipCommander(pw,ch))
        //            {
        //                str += ch;
        //                count++;
        //            }
        //        }
        //    }
        //    pw.incPos();

        //    //数値解析(ネスト中は単なるスキップ
        //    if (!pw.getNumNoteLength(out int n, out bool directFlg))
        //    {
        //        n = (int)pw.length;
        //    }
        //    else
        //    {
        //        if (!directFlg)
        //        {
        //            if ((int)info.clockCount % n != 0)
        //            {
        //                msgBox.setWrnMsg(string.Format("割り切れない音長({0})の指定があります。音長は不定になります。", n), pw.getSrcFn(), pw.getLineNumber());
        //            }
        //            n = (int)info.clockCount / n;
        //        }
        //        else
        //        {
        //            n = Common.CheckRange(n, 1, 65535);
        //        }
        //    }
        //    if (nest > 1) n = -1;

        //    lstRenpu.Add(new Tuple<int, string>(n, str));
        //    nest--;
        //}

        //private void DecStep2Renpu(List<Tuple<int, string>> lstRenpu,List<int> lstRenpuLength, int nest,int len)
        //{
        //    Tuple<int, string> t = lstRenpu[lstRenpu.Count - nest];
        //    if (t.Item1 != -1) len = t.Item1;

        //    for (int p = 0; p < t.Item2.Length; p++)
        //    {
        //        int le = len / t.Item2.Length + ((len % t.Item2.Length) == 0 ? 0 : ((len % t.Item2.Length) > p ? 1 : 0));
        //        if (t.Item2[p] != '*') lstRenpuLength.Add(le);
        //        else DecStep2Renpu(lstRenpu,lstRenpuLength, nest + 1, le);
        //    }
        //}

        //private void CmdEnvelope(partWork pw)
        //{
        //    int n = -1;
        //    if (
        //        ((pw.chip is YM2203) && (pw.Type == enmChannelType.SSG || pw.Type == enmChannelType.FMOPNex))
        //        || ((pw.chip is YM2608) && (pw.Type == enmChannelType.SSG || pw.Type == enmChannelType.FMOPNex))
        //        || ((pw.chip is YM2610B) && (pw.Type == enmChannelType.SSG || pw.Type == enmChannelType.FMOPNex))
        //        || ((pw.chip is YM2612) && (pw.Type == enmChannelType.FMPCM || pw.Type == enmChannelType.FMOPNex))
        //        || ((pw.chip is YM2612X) && (pw.Type == enmChannelType.FMPCM || pw.Type == enmChannelType.FMOPNex))
        //        || (pw.chip is SN76489)
        //        || (pw.chip is RF5C164)
        //        || (pw.chip is segaPcm)
        //        || (pw.chip is HuC6280)
        //        )
        //    {
        //        pw.incPos();
        //        switch (pw.getChar())
        //        {
        //            case 'O':
        //                pw.incPos();
        //                if (
        //                    ((pw.chip is YM2203) && pw.Type == enmChannelType.FMOPNex)
        //                    || ((pw.chip is YM2608) && pw.Type == enmChannelType.FMOPNex)
        //                    || ((pw.chip is YM2610B) && pw.Type == enmChannelType.FMOPNex)
        //                    || pw.chip is YM2612
        //                    || pw.chip is YM2612X
        //                    )
        //                {
        //                    switch (pw.getChar())
        //                    {
        //                        case 'N':
        //                            pw.incPos();
        //                            pw.Ch3SpecialMode = true;
        //                            ((ClsOPN)pw.chip).OutOPNSetCh3SpecialMode(pw, true);
        //                            break;
        //                        case 'F':
        //                            pw.incPos();
        //                            pw.Ch3SpecialMode = false;
        //                            ((ClsOPN)pw.chip).OutOPNSetCh3SpecialMode(pw, false);
        //                            break;
        //                        default:
        //                            msgBox.setErrMsg(string.Format("未知のコマンド(EO{0})が指定されました。", pw.getChar()), pw.getSrcFn(), pw.getLineNumber());
        //                            pw.incPos();
        //                            break;
        //                    }
        //                }
        //                else
        //                {
        //                    switch (pw.getChar())
        //                    {
        //                        case 'N':
        //                            pw.incPos();
        //                            pw.envelopeMode = true;
        //                            break;
        //                        case 'F':
        //                            pw.incPos();
        //                            pw.envelopeMode = false;
        //                            if (pw.Type == enmChannelType.SSG)
        //                            {
        //                                pw.beforeVolume = -1;
        //                            }
        //                            break;
        //                        default:
        //                            msgBox.setErrMsg(string.Format("未知のコマンド(EO{0})が指定されました。", pw.getChar()), pw.getSrcFn(), pw.getLineNumber());
        //                            pw.incPos();
        //                            break;
        //                    }
        //                }
        //                break;
        //            case 'X':
        //                char c = pw.getChar();
        //                if (
        //                    ((pw.chip is YM2203) && pw.Type == enmChannelType.FMOPNex)
        //                    || ((pw.chip is YM2608) && pw.Type == enmChannelType.FMOPNex)
        //                    || ((pw.chip is YM2610B) && pw.Type == enmChannelType.FMOPNex)
        //                    || pw.chip is YM2612
        //                    || pw.chip is YM2612X
        //                    )
        //                {
        //                    pw.incPos();
        //                    if (!pw.getNum(out n))
        //                    {
        //                        msgBox.setErrMsg("不正なスロット指定'EX'が指定されています。", pw.getSrcFn(), pw.getLineNumber());
        //                        n = 0;
        //                    }
        //                    byte res = 0;
        //                    while (n % 10 != 0)
        //                    {
        //                        if (n % 10 > 0 && n % 10 < 5)
        //                        {
        //                            res += (byte)(1 << (n % 10 - 1));
        //                        }
        //                        else
        //                        {
        //                            msgBox.setErrMsg(string.Format("未知のコマンド(EX{0})が指定されました。", n), pw.getSrcFn(), pw.getLineNumber());
        //                            break;
        //                        }
        //                        n /= 10;
        //                    }
        //                    if (res != 0)
        //                    {
        //                        pw.slots = res;
        //                    }
        //                }
        //                else
        //                {
        //                    msgBox.setErrMsg("未知のコマンド(EX)が指定されました。", pw.getSrcFn(), pw.getLineNumber());
        //                }
        //                break;
        //            default:
        //                if (pw.Type == enmChannelType.FMOPNex)
        //                {
        //                    int[] s = new int[] { 0, 0, 0, 0 };

        //                    for (int i = 0; i < 4; i++)
        //                    {
        //                        if (pw.getNum(out n))
        //                        {
        //                            s[i] = n;
        //                        }
        //                        else
        //                        {
        //                            msgBox.setErrMsg("Eコマンドの解析に失敗しました。", pw.getSrcFn(), pw.getLineNumber());
        //                            break;
        //                        }
        //                        if (i == 3) break;
        //                        pw.incPos();
        //                    }
        //                    pw.slotDetune = s;
        //                    break;
        //                }
        //                else
        //                {
        //                    msgBox.setErrMsg(string.Format("未知のコマンド(E{0})が指定されました。", pw.getChar()), pw.getSrcFn(), pw.getLineNumber());
        //                    pw.incPos();
        //                }
        //                break;
        //        }
        //    }
        //    else
        //    {
        //        msgBox.setWrnMsg("このパートは効果音モードに対応したチャンネルが指定されていないため、Eコマンドは無視されます。", pw.getSrcFn(), pw.getLineNumber());
        //        pw.incPos();
        //    }

        //}

        //private void CmdInstrument(partWork pw)
        //{
        //    int n;
        //    pw.incPos();

        //    if (pw.getChar() == 'T')
        //    {
        //        if (pw.Type != enmChannelType.FMOPM && pw.Type != enmChannelType.FMOPN && pw.Type != enmChannelType.FMOPNex)
        //        {
        //            msgBox.setErrMsg("Tone DoublerはFM音源以外では使用できません。", pw.getSrcFn(), pw.getLineNumber());
        //            pw.incPos();
        //            return;
        //        }
        //        pw.incPos();
        //        if (!pw.getNum(out n))
        //        {
        //            msgBox.setErrMsg("不正な音色番号が指定されています。", pw.getSrcFn(), pw.getLineNumber());
        //            n = 0;
        //        }
        //        n = Common.CheckRange(n, 0, 255);
        //        pw.toneDoubler = n;
        //        return;
        //    }

        //    if (pw.chip is YM2151)
        //    {
        //        if (!pw.getNum(out n))
        //        {
        //            msgBox.setErrMsg("不正な音色番号が指定されています。", pw.getSrcFn(), pw.getLineNumber());
        //            n = 0;
        //        }
        //        n = Common.CheckRange(n, 0, 255);
        //        if (pw.instrument != n)
        //        {
        //            pw.instrument = n;
        //            ((YM2151)pw.chip).OutSetInstrument(pw, n, pw.volume);
        //        }
        //    }
        //    else if (pw.chip is YM2203)
        //    {
        //        if (pw.ch < 6)
        //        {
        //            if (!pw.getNum(out n))
        //            {
        //                msgBox.setErrMsg("不正な音色番号が指定されています。", pw.getSrcFn(), pw.getLineNumber());
        //                n = 0;
        //            }
        //            n = Common.CheckRange(n, 0, 255);
        //            if (pw.instrument != n)
        //            {
        //                pw.instrument = n;
        //                if (pw.Type == enmChannelType.FMOPNex)
        //                {
        //                    ym2203[pw.chip.ChipID].lstPartWork[2].instrument = n;
        //                    ym2203[pw.chip.ChipID].lstPartWork[3].instrument = n;
        //                    ym2203[pw.chip.ChipID].lstPartWork[4].instrument = n;
        //                    ym2203[pw.chip.ChipID].lstPartWork[5].instrument = n;
        //                }
        //                ym2203[pw.chip.ChipID].OutFmSetInstrument(pw, n, pw.volume);
        //            }
        //        }
        //        else if (pw.Type == enmChannelType.SSG)
        //        {
        //            n = SetEnvelopParamFromInstrument(pw);
        //        }
        //    }
        //    else if (pw.chip is YM2608)
        //    {
        //        if (pw.ch < 9)
        //        {
        //            if (!pw.getNum(out n))
        //            {
        //                msgBox.setErrMsg("不正な音色番号が指定されています。", pw.getSrcFn(), pw.getLineNumber());
        //                n = 0;
        //            }
        //            n = Common.CheckRange(n, 0, 255);
        //            if (pw.instrument != n)
        //            {
        //                pw.instrument = n;
        //                if (pw.Type == enmChannelType.FMOPNex)
        //                {
        //                    ym2608[pw.chip.ChipID].lstPartWork[2].instrument = n;
        //                    ym2608[pw.chip.ChipID].lstPartWork[6].instrument = n;
        //                    ym2608[pw.chip.ChipID].lstPartWork[7].instrument = n;
        //                    ym2608[pw.chip.ChipID].lstPartWork[8].instrument = n;
        //                }
        //                ym2608[pw.chip.ChipID].OutFmSetInstrument(pw, n, pw.volume);
        //            }
        //        }
        //        else if (pw.Type == enmChannelType.SSG)
        //        {
        //            n = SetEnvelopParamFromInstrument(pw);
        //        }
        //        else if (pw.Type == enmChannelType.RHYTHM)
        //        {
        //            if (pw.getChar() != 'E')
        //            {
        //                n = SetEnvelopParamFromInstrument(pw);
        //            }
        //            else
        //            {
        //                pw.incPos();
        //                n = SetEnvelopParamFromInstrument(pw);
        //            }
        //        }
        //        else if (pw.Type == enmChannelType.ADPCM)
        //        {
        //            if (pw.getChar() != 'E')
        //            {
        //                if (!pw.getNum(out n))
        //                {
        //                    msgBox.setErrMsg("不正な音色番号が指定されています。", pw.getSrcFn(), pw.getLineNumber());
        //                    n = 0;
        //                }
        //                n = Common.CheckRange(n, 0, 255);
        //                if (!instPCM.ContainsKey(n))
        //                {
        //                    msgBox.setErrMsg(string.Format("PCM定義に指定された音色番号({0})が存在しません。", n), pw.getSrcFn(), pw.getLineNumber());
        //                }
        //                else
        //                {
        //                    if (instPCM[n].chip != enmChipType.YM2608 || instPCM[n].loopAdr != 1)
        //                    {
        //                        msgBox.setErrMsg(string.Format("指定された音色番号({0})はYM2608向けPCMデータではありません。", n), pw.getSrcFn(), pw.getLineNumber());
        //                    }
        //                    pw.instrument = n;
        //                    ym2608[pw.chip.ChipID].SetADPCMAddress(pw, (int)instPCM[n].stAdr, (int)instPCM[n].edAdr);
        //                }
        //            }
        //            else
        //            {
        //                pw.incPos();
        //                n = SetEnvelopParamFromInstrument(pw);
        //            }
        //        }
        //    }
        //    else if (pw.chip is YM2610B)
        //    {
        //        if (pw.ch < 9)
        //        {
        //            if (!pw.getNum(out n))
        //            {
        //                msgBox.setErrMsg("不正な音色番号が指定されています。", pw.getSrcFn(), pw.getLineNumber());
        //                n = 0;
        //            }
        //            n = Common.CheckRange(n, 0, 255);
        //            if (pw.instrument != n)
        //            {
        //                pw.instrument = n;
        //                if (pw.Type == enmChannelType.FMOPNex)
        //                {
        //                    ym2610b[pw.chip.ChipID].lstPartWork[2].instrument = n;
        //                    ym2610b[pw.chip.ChipID].lstPartWork[6].instrument = n;
        //                    ym2610b[pw.chip.ChipID].lstPartWork[7].instrument = n;
        //                    ym2610b[pw.chip.ChipID].lstPartWork[8].instrument = n;
        //                }
        //                if (!pw.pcm)
        //                {
        //                    ym2610b[pw.chip.ChipID].OutFmSetInstrument(pw, n, pw.volume);
        //                }
        //                else
        //                {
        //                    if (!instPCM.ContainsKey(n))
        //                    {
        //                        msgBox.setErrMsg(string.Format("PCM定義に指定された音色番号({0})が存在しません。", n), pw.getSrcFn(), pw.getLineNumber());
        //                    }
        //                    else
        //                    {
        //                        if (instPCM[n].chip != enmChipType.YM2610B)
        //                        {
        //                            msgBox.setErrMsg(string.Format("指定された音色番号({0})はYM2610B向けPCMデータではありません。", n), pw.getSrcFn(), pw.getLineNumber());
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        else if (pw.Type == enmChannelType.SSG)
        //        {
        //            n = SetEnvelopParamFromInstrument(pw);
        //        }
        //        else if (pw.Type == enmChannelType.ADPCMA)
        //        {
        //            if (pw.getChar() != 'E')
        //            {
        //                if (!pw.getNum(out n))
        //                {
        //                    msgBox.setErrMsg("不正な音色番号が指定されています。", pw.getSrcFn(), pw.getLineNumber());
        //                    n = 0;
        //                }
        //                n = Common.CheckRange(n, 0, 255);
        //                if (!instPCM.ContainsKey(n))
        //                {
        //                    msgBox.setErrMsg(string.Format("PCM定義に指定された音色番号({0})が存在しません。", n), pw.getSrcFn(), pw.getLineNumber());
        //                }
        //                else
        //                {
        //                    if (instPCM[n].chip != enmChipType.YM2610B || instPCM[n].loopAdr != 0)
        //                    {
        //                        msgBox.setErrMsg(string.Format("指定された音色番号({0})はYM2610B向けPCMデータではありません。", n), pw.getSrcFn(), pw.getLineNumber());
        //                    }
        //                    pw.instrument = n;
        //                    ym2610b[pw.chip.ChipID].SetADPCMAAddress(pw, (int)instPCM[n].stAdr, (int)instPCM[n].edAdr);
        //                }
        //            }
        //            else
        //            {
        //                pw.incPos();
        //                n = SetEnvelopParamFromInstrument(pw);
        //            }
        //        }
        //        else if (pw.Type == enmChannelType.ADPCMB)
        //        {
        //            if (pw.getChar() != 'E')
        //            {
        //                if (!pw.getNum(out n))
        //                {
        //                    msgBox.setErrMsg("不正な音色番号が指定されています。", pw.getSrcFn(), pw.getLineNumber());
        //                    n = 0;
        //                }
        //                n = Common.CheckRange(n, 0, 255);
        //                if (!instPCM.ContainsKey(n))
        //                {
        //                    msgBox.setErrMsg(string.Format("PCM定義に指定された音色番号({0})が存在しません。", n), pw.getSrcFn(), pw.getLineNumber());
        //                }
        //                else
        //                {
        //                    if (instPCM[n].chip != enmChipType.YM2610B || instPCM[n].loopAdr != 1)
        //                    {
        //                        msgBox.setErrMsg(string.Format("指定された音色番号({0})はYM2610B向けPCMデータではありません。", n), pw.getSrcFn(), pw.getLineNumber());
        //                    }
        //                    pw.instrument = n;
        //                    ym2610b[pw.chip.ChipID].SetADPCMBAddress(pw, (int)instPCM[n].stAdr, (int)instPCM[n].edAdr);
        //                }
        //            }
        //            else
        //            {
        //                pw.incPos();
        //                n = SetEnvelopParamFromInstrument(pw);
        //            }
        //        }
        //    }
        //    else if (pw.chip is YM2612)
        //    {
        //        if (!pw.getNum(out n))
        //        {
        //            msgBox.setErrMsg("不正な音色番号が指定されています。", pw.getSrcFn(), pw.getLineNumber());
        //            n = 0;
        //        }
        //        n = Common.CheckRange(n, 0, 255);
        //        if (pw.instrument != n)
        //        {
        //            pw.instrument = n;
        //            if (pw.Type == enmChannelType.FMOPNex)
        //            {
        //                ym2612[pw.chip.ChipID].lstPartWork[2].instrument = n;
        //                ym2612[pw.chip.ChipID].lstPartWork[6].instrument = n;
        //                ym2612[pw.chip.ChipID].lstPartWork[7].instrument = n;
        //                ym2612[pw.chip.ChipID].lstPartWork[8].instrument = n;
        //            }
        //            if (!pw.pcm)
        //            {
        //                ym2612[pw.chip.ChipID].OutFmSetInstrument(pw, n, pw.volume);
        //            }
        //            else
        //            {
        //                if (!instPCM.ContainsKey(n))
        //                {
        //                    msgBox.setErrMsg(string.Format("PCM定義に指定された音色番号({0})が存在しません。", n), pw.getSrcFn(), pw.getLineNumber());
        //                }
        //                else
        //                {
        //                    if (instPCM[n].chip != enmChipType.YM2612)
        //                    {
        //                        msgBox.setErrMsg(string.Format("指定された音色番号({0})はYM2612向けPCMデータではありません。", n), pw.getSrcFn(), pw.getLineNumber());
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    else if (pw.chip is YM2612X)
        //    {
        //        if (!pw.getNum(out n))
        //        {
        //            msgBox.setErrMsg("不正な音色番号が指定されています。", pw.getSrcFn(), pw.getLineNumber());
        //            n = 0;
        //        }
        //        n = Common.CheckRange(n, 0, 255);
        //        if (pw.instrument != n)
        //        {
        //            pw.instrument = n;
        //            if (pw.Type == enmChannelType.FMOPNex)
        //            {
        //                ym2612x[pw.chip.ChipID].lstPartWork[2].instrument = n;
        //                ym2612x[pw.chip.ChipID].lstPartWork[6].instrument = n;
        //                ym2612x[pw.chip.ChipID].lstPartWork[7].instrument = n;
        //                ym2612x[pw.chip.ChipID].lstPartWork[8].instrument = n;
        //            }
        //            if (!pw.pcm)
        //            {
        //                ym2612x[pw.chip.ChipID].OutFmSetInstrument(pw, n, pw.volume);
        //            }
        //            else
        //            {
        //                if (!instPCM.ContainsKey(n))
        //                {
        //                    msgBox.setErrMsg(string.Format("PCM定義に指定された音色番号({0})が存在しません。", n), pw.getSrcFn(), pw.getLineNumber());
        //                }
        //                else
        //                {
        //                    if (instPCM[n].chip != enmChipType.YM2612X)
        //                    {
        //                        msgBox.setErrMsg(string.Format("指定された音色番号({0})はYM2612(XGM)向けPCMデータではありません。", n), pw.getSrcFn(), pw.getLineNumber());
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    else if (pw.chip is SN76489)
        //    {
        //        //pw.incPos();
        //        n = SetEnvelopParamFromInstrument(pw);
        //    }
        //    else if (pw.chip is RF5C164)
        //    {
        //        if (pw.getChar() != 'E')
        //        {
        //            if (!pw.getNum(out n))
        //            {
        //                msgBox.setErrMsg("不正な音色番号が指定されています。", pw.getSrcFn(), pw.getLineNumber());
        //                n = 0;
        //            }
        //            n = Common.CheckRange(n, 0, 255);
        //            if (!instPCM.ContainsKey(n))
        //            {
        //                msgBox.setErrMsg(string.Format("PCM定義に指定された音色番号({0})が存在しません。", n), pw.getSrcFn(), pw.getLineNumber());
        //            }
        //            else
        //            {
        //                if (instPCM[n].chip != enmChipType.RF5C164)
        //                {
        //                    msgBox.setErrMsg(string.Format("指定された音色番号({0})はRF5C164向けPCMデータではありません。", n), pw.getSrcFn(), pw.getLineNumber());
        //                }
        //                pw.instrument = n;
        //                ((RF5C164)pw.chip).SetRf5c164CurrentChannel(pw);
        //                ((RF5C164)pw.chip).SetRf5c164SampleStartAddress(pw, (int)instPCM[n].stAdr);
        //                ((RF5C164)pw.chip).SetRf5c164LoopAddress(pw, (int)(instPCM[n].loopAdr));
        //            }
        //        }
        //        else
        //        {
        //            pw.incPos();
        //            n = SetEnvelopParamFromInstrument(pw);
        //        }
        //    }
        //    else if (pw.chip is segaPcm)
        //    {
        //        if (pw.getChar() != 'E')
        //        {
        //            if (!pw.getNum(out n))
        //            {
        //                msgBox.setErrMsg("不正な音色番号が指定されています。", pw.getSrcFn(), pw.getLineNumber());
        //                n = 0;
        //            }
        //            n = Common.CheckRange(n, 0, 255);
        //            if (!instPCM.ContainsKey(n))
        //            {
        //                msgBox.setErrMsg(string.Format("PCM定義に指定された音色番号({0})が存在しません。", n), pw.getSrcFn(), pw.getLineNumber());
        //            }
        //            else
        //            {
        //                if (instPCM[n].chip != enmChipType.SEGAPCM)
        //                {
        //                    msgBox.setErrMsg(string.Format("指定された音色番号({0})はSEGAPCM向けPCMデータではありません。", n), pw.getSrcFn(), pw.getLineNumber());
        //                }
        //                pw.instrument = n;
        //                pw.pcmStartAddress = (int)instPCM[n].stAdr;
        //                pw.pcmEndAddress = (int)instPCM[n].edAdr;
        //                pw.pcmLoopAddress = instPCM[n].loopAdr == 0 ? -1 : (int)instPCM[n].loopAdr;
        //                pw.pcmBank = (int)((instPCM[n].stAdr >> 16) << 1);
        //            }
        //        }
        //        else
        //        {
        //            pw.incPos();
        //            n = SetEnvelopParamFromInstrument(pw);
        //        }
        //    }
        //    else if (pw.chip is HuC6280)
        //    {
        //        if (pw.getChar() != 'E')
        //        {
        //            if (!pw.getNum(out n))
        //            {
        //                msgBox.setErrMsg("不正な音色番号が指定されています。", pw.getSrcFn(), pw.getLineNumber());
        //                n = 0;
        //            }
        //            n = Common.CheckRange(n, 0, 255);
        //            if (pw.instrument != n)
        //            {
        //                pw.instrument = n;
        //                if (!pw.pcm)
        //                {
        //                    ((HuC6280)pw.chip).OutHuC6280SetInstrument(pw, n);
        //                }
        //                else
        //                {
        //                    if (!instPCM.ContainsKey(n))
        //                    {
        //                        msgBox.setErrMsg(string.Format("PCM定義に指定された音色番号({0})が存在しません。", n), pw.getSrcFn(), pw.getLineNumber());
        //                    }
        //                    else
        //                    {
        //                        if (instPCM[n].chip != enmChipType.HuC6280)
        //                        {
        //                            msgBox.setErrMsg(string.Format("指定された音色番号({0})はHuC6280向けPCMデータではありません。", n), pw.getSrcFn(), pw.getLineNumber());
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            pw.incPos();
        //            n = SetEnvelopParamFromInstrument(pw);
        //        }
        //    }
        //    else if (pw.chip is YM2413)
        //    {
        //        if (pw.getChar() == 'E')
        //        {
        //            pw.incPos();
        //            if (!pw.getNum(out n))
        //            {
        //                msgBox.setErrMsg("不正な音色(INST)番号が指定されています。", pw.getSrcFn(), pw.getLineNumber());
        //                n = 0;
        //            }
        //            n = Common.CheckRange(n, 0, 15);
        //            if (pw.envInstrument != n)
        //            {
        //                ((YM2413)pw.chip).outYM2413SetInstVol(pw, n, pw.volume); //INSTを0にセット
        //            }
        //        }
        //        else
        //        {
        //            if (!pw.getNum(out n))
        //            {
        //                msgBox.setErrMsg("不正な音色番号が指定されています。", pw.getSrcFn(), pw.getLineNumber());
        //                n = 0;
        //            }
        //            n = Common.CheckRange(n, 0, 255);
        //            if (pw.instrument != n)
        //            {
        //                ((YM2413)pw.chip).outYM2413SetInstrument(pw, n); //音色のセット
        //                ((YM2413)pw.chip).outYM2413SetInstVol(pw, 0, pw.volume); //INSTを0にセット
        //                OutData(pw.port0, 0x20, 0x19);
        //            }
        //        }
        //    }

        //}

        //private int SetEnvelopParamFromInstrument(partWork pw)
        //{
        //    if (!pw.getNum(out int n))
        //    {
        //        msgBox.setErrMsg("不正なエンベロープ番号が指定されています。", pw.getSrcFn(), pw.getLineNumber());
        //        n = 0;
        //    }
        //    n = Common.CheckRange(n, 0, 255);
        //    if (!instENV.ContainsKey(n))
        //    {
        //        msgBox.setErrMsg(string.Format("エンベロープ定義に指定された音色番号({0})が存在しません。", n), pw.getSrcFn(), pw.getLineNumber());
        //    }
        //    else
        //    {
        //        if (pw.envInstrument != n)
        //        {
        //            pw.envInstrument = n;
        //            pw.envIndex = -1;
        //            pw.envCounter = -1;
        //            for (int i = 0; i < instENV[n].Length; i++)
        //            {
        //                pw.envelope[i] = instENV[n][i];
        //            }
        //        }
        //    }

        //    return n;
        //}

        //private void SetEnvelopeAtKeyOn(partWork pw)
        //{
        //    if (!pw.envelopeMode)
        //    {
        //        pw.envVolume = 0;
        //        pw.envIndex = -1;
        //        return;
        //    }

        //    pw.envIndex = 0;
        //    pw.envCounter = 0;
        //    int maxValue = pw.MaxVolume;// (pw.envelope[8] == (int)enmChipType.RF5C164) ? 255 : 15;

        //    while (pw.envCounter == 0 && pw.envIndex != -1)
        //    {
        //        switch (pw.envIndex)
        //        {
        //            case 0: // AR phase
        //                pw.envCounter = pw.envelope[2];
        //                if (pw.envelope[2] > 0 && pw.envelope[1] < maxValue)
        //                {
        //                    pw.envVolume = pw.envelope[1];
        //                }
        //                else
        //                {
        //                    pw.envVolume = maxValue;
        //                    pw.envIndex++;
        //                }
        //                break;
        //            case 1: // DR phase
        //                pw.envCounter = pw.envelope[3];
        //                if (pw.envelope[3] > 0 && pw.envelope[4] < maxValue)
        //                {
        //                    pw.envVolume = maxValue;
        //                }
        //                else
        //                {
        //                    pw.envVolume = pw.envelope[4];
        //                    pw.envIndex++;
        //                }
        //                break;
        //            case 2: // SR phase
        //                pw.envCounter = pw.envelope[5];
        //                if (pw.envelope[5] > 0 && pw.envelope[4] != 0)
        //                {
        //                    pw.envVolume = pw.envelope[4];
        //                }
        //                else
        //                {
        //                    pw.envVolume = 0;
        //                    pw.envIndex = -1;
        //                }
        //                break;
        //        }
        //    }
        //}

        //private void SetLfoAtKeyOn(partWork pw)
        //{
        //    for (int lfo = 0; lfo < 4; lfo++)
        //    {
        //        clsLfo pl = pw.lfo[lfo];
        //        if (!pl.sw)
        //        {
        //            continue;
        //        }
        //        if (pl.type == eLfoType.Hardware)
        //        {
        //            if (pw.chip is YM2612)
        //            {
        //                if (pl.param[4] == 1)
        //                {
        //                    ((ClsOPN)pw.chip).OutOPNSetHardLfo(pw, false, pl.param[1]);
        //                    pl.waitCounter = pl.param[0];
        //                }
        //            }
        //            continue;
        //        }
        //        if (pl.param[5] != 1)
        //        {
        //            continue;
        //        }

        //        pl.isEnd = false;
        //        pl.value = (pl.param[0] == 0) ? pl.param[6] : 0;//ディレイ中は振幅補正は適用されない
        //        pl.waitCounter = pl.param[0];
        //        pl.direction = pl.param[2] < 0 ? -1 : 1;
        //        if (pl.type == eLfoType.Vibrato)
        //        {
        //            if (pw.chip is YM2151)
        //            {
        //                ((YM2151)pw.chip).SetFNum(pw);
        //            }
        //            else if ((pw.chip is YM2203) || (pw.chip is YM2608) || (pw.chip is YM2610B) || (pw.chip is YM2612))
        //            {
        //                if (pw.Type == enmChannelType.FMOPN || pw.Type == enmChannelType.FMOPNex)
        //                {
        //                    ((ClsOPN)pw.chip).SetFmFNum(pw);
        //                }
        //                else if (pw.Type == enmChannelType.SSG)
        //                {
        //                    ((ClsOPN)pw.chip).SetSsgFNum(pw);
        //                }
        //            }
        //            else if (pw.chip is SN76489)
        //            {
        //                ((SN76489)pw.chip).SetDcsgFNum(pw);
        //            }
        //        }
        //        if (pl.type == eLfoType.Tremolo)
        //        {
        //            if (pw.chip is YM2151)
        //            {
        //                ((YM2151)pw.chip).SetVolume(pw);
        //            }
        //            else if ((pw.chip is YM2203) || (pw.chip is YM2608) || (pw.chip is YM2610B) || (pw.chip is YM2612))
        //            {
        //                pw.beforeVolume = -1;
        //                ((ClsOPN)pw.chip).SetFmVolume(pw);
        //            }
        //        }
        //    }
        //}

        //private int GetToneDoublerShift(partWork pw, int octave, char noteCmd, int shift)
        //{
        //    if (pw.Type != enmChannelType.FMOPM && pw.Type != enmChannelType.FMOPN && pw.Type != enmChannelType.FMOPNex)
        //    {
        //        return 0;
        //    }

        //    int i = pw.instrument;
        //    if (pw.TdA == -1)
        //    {
        //        return 0;
        //    }

        //    int TdB = octave * 12 + Const.NOTE.IndexOf(noteCmd) + shift;
        //    int s = pw.TdA - TdB;
        //    int us = Math.Abs(s);
        //    int n = pw.toneDoubler;
        //    if (us >= instToneDoubler[n].lstTD.Count)
        //    {
        //        return 0;
        //    }

        //    return ((s < 0) ? s : 0) + instToneDoubler[n].lstTD[us].KeyShift;
        //}

        //private void SetToneDoubler(partWork pw)
        //{
        //    if (pw.Type != enmChannelType.FMOPM && pw.Type != enmChannelType.FMOPN && (pw.Type != enmChannelType.FMOPNex || (pw.Type != enmChannelType.FMOPNex || pw.ch != 2)))
        //    {
        //        return;
        //    }
        //    //if (pw.Type != enmChannelType.FMOPM && pw.Type != enmChannelType.FMOPN && pw.Type != enmChannelType.FMOPNex)
        //    //{
        //      //  return;
        //    //}

        //    int i = pw.instrument;
        //    if (i < 0) return;

        //    pw.toneDoublerKeyShift = 0;
        //    if (pw.TdA == -1)
        //    {
        //        if (instFM != null && instFM.Count > i)
        //        {
        //            //resetToneDoubler
        //            if (pw.Type != enmChannelType.FMOPM)
        //            {
        //                if (pw.op1ml != instFM[i][0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8])
        //                {
        //                    ((ClsOPN)pw.chip).OutFmSetDtMl(pw, 0, instFM[i][0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instFM[i][0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8]);
        //                    pw.op1ml = instFM[i][0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
        //                }
        //                if (pw.op2ml != instFM[i][1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8])
        //                {
        //                    ((ClsOPN)pw.chip).OutFmSetDtMl(pw, 1, instFM[i][1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instFM[i][1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8]);
        //                    pw.op2ml = instFM[i][1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
        //                }
        //                if (pw.op3ml != instFM[i][2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8])
        //                {
        //                    ((ClsOPN)pw.chip).OutFmSetDtMl(pw, 2, instFM[i][2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instFM[i][2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8]);
        //                    pw.op3ml = instFM[i][2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
        //                }
        //                if (pw.op4ml != instFM[i][3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8])
        //                {
        //                    ((ClsOPN)pw.chip).OutFmSetDtMl(pw, 3, instFM[i][3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instFM[i][3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8]);
        //                    pw.op4ml = instFM[i][3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
        //                }
        //            }
        //            else
        //            {
        //                //ML
        //                if (pw.op1ml != instFM[i][0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8])
        //                {
        //                    ((YM2151)pw.chip).OutSetDtMl(pw, 0, instFM[i][0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instFM[i][0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8]);
        //                    pw.op1ml = instFM[i][0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
        //                }
        //                if (pw.op2ml != instFM[i][1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8])
        //                {
        //                    ((YM2151)pw.chip).OutSetDtMl(pw, 1, instFM[i][1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instFM[i][1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8]);
        //                    pw.op2ml = instFM[i][1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
        //                }
        //                if (pw.op3ml != instFM[i][2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8])
        //                {
        //                    ((YM2151)pw.chip).OutSetDtMl(pw, 2, instFM[i][2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instFM[i][2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8]);
        //                    pw.op3ml = instFM[i][2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
        //                }
        //                if (pw.op4ml != instFM[i][3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8])
        //                {
        //                    ((YM2151)pw.chip).OutSetDtMl(pw, 3, instFM[i][3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instFM[i][3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8]);
        //                    pw.op4ml = instFM[i][3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
        //                }
        //                //DT2
        //                if (pw.op1dt2 != instFM[i][0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 10])
        //                {
        //                    ((YM2151)pw.chip).OutSetDt2Sr(pw, 0, instFM[i][0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 10], instFM[i][0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 3]);
        //                    pw.op1dt2 = instFM[i][0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 10];
        //                }
        //                if (pw.op2dt2 != instFM[i][1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 10])
        //                {
        //                    ((YM2151)pw.chip).OutSetDt2Sr(pw, 1, instFM[i][1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 10], instFM[i][1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 3]);
        //                    pw.op2dt2 = instFM[i][1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 10];
        //                }
        //                if (pw.op3dt2 != instFM[i][2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 10])
        //                {
        //                    ((YM2151)pw.chip).OutSetDt2Sr(pw, 2, instFM[i][2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 10], instFM[i][2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 3]);
        //                    pw.op3dt2 = instFM[i][2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 10];
        //                }
        //                if (pw.op4dt2 != instFM[i][3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 10])
        //                {
        //                    ((YM2151)pw.chip).OutSetDt2Sr(pw, 3, instFM[i][3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 10], instFM[i][3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 3]);
        //                    pw.op4dt2 = instFM[i][3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 10];
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        //setToneDoubler
        //        int TdB = pw.octaveNow * 12 + Const.NOTE.IndexOf(pw.noteCmd) + pw.shift + pw.keyShift;
        //        int s = pw.TdA - TdB;
        //        int us = Math.Abs(s);
        //        int n = pw.toneDoubler;
        //        if (us >= instToneDoubler[n].lstTD.Count)
        //        {
        //            return;
        //        }

        //        pw.toneDoublerKeyShift = ((s < 0) ? s : 0) + instToneDoubler[n].lstTD[us].KeyShift;

        //        if (instFM != null && instFM.Count > i)
        //        {
        //            if (pw.Type != enmChannelType.FMOPM)
        //            {
        //                if (pw.op1ml != instToneDoubler[n].lstTD[us].OP1ML)
        //                {
        //                    ((ClsOPN)pw.chip).OutFmSetDtMl(pw, 0, instFM[i][0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instToneDoubler[n].lstTD[us].OP1ML);
        //                    pw.op1ml = instToneDoubler[n].lstTD[us].OP1ML;
        //                }
        //                if (pw.op2ml != instToneDoubler[n].lstTD[us].OP2ML)
        //                {
        //                    ((ClsOPN)pw.chip).OutFmSetDtMl(pw, 1, instFM[i][1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instToneDoubler[n].lstTD[us].OP2ML);
        //                    pw.op2ml = instToneDoubler[n].lstTD[us].OP2ML;
        //                }
        //                if (pw.op3ml != instToneDoubler[n].lstTD[us].OP3ML)
        //                {
        //                    ((ClsOPN)pw.chip).OutFmSetDtMl(pw, 2, instFM[i][2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instToneDoubler[n].lstTD[us].OP3ML);
        //                    pw.op3ml = instToneDoubler[n].lstTD[us].OP3ML;
        //                }
        //                if (pw.op4ml != instToneDoubler[n].lstTD[us].OP4ML)
        //                {
        //                    ((ClsOPN)pw.chip).OutFmSetDtMl(pw, 3, instFM[i][3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instToneDoubler[n].lstTD[us].OP4ML);
        //                    pw.op4ml = instToneDoubler[n].lstTD[us].OP4ML;
        //                }
        //            }
        //            else
        //            {
        //                //ML
        //                if (pw.op1ml != instToneDoubler[n].lstTD[us].OP1ML)
        //                {
        //                    ((YM2151)pw.chip).OutSetDtMl(pw, 0, instFM[i][0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instToneDoubler[n].lstTD[us].OP1ML);
        //                    pw.op1ml = instToneDoubler[n].lstTD[us].OP1ML;
        //                }
        //                if (pw.op2ml != instToneDoubler[n].lstTD[us].OP2ML)
        //                {
        //                    ((YM2151)pw.chip).OutSetDtMl(pw, 1, instFM[i][1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instToneDoubler[n].lstTD[us].OP2ML);
        //                    pw.op2ml = instToneDoubler[n].lstTD[us].OP2ML;
        //                }
        //                if (pw.op3ml != instToneDoubler[n].lstTD[us].OP3ML)
        //                {
        //                    ((YM2151)pw.chip).OutSetDtMl(pw, 2, instFM[i][2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instToneDoubler[n].lstTD[us].OP3ML);
        //                    pw.op3ml = instToneDoubler[n].lstTD[us].OP3ML;
        //                }
        //                if (pw.op4ml != instToneDoubler[n].lstTD[us].OP4ML)
        //                {
        //                    ((YM2151)pw.chip).OutSetDtMl(pw, 3, instFM[i][3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instToneDoubler[n].lstTD[us].OP4ML);
        //                    pw.op4ml = instToneDoubler[n].lstTD[us].OP4ML;
        //                }
        //                //DT2
        //                if (pw.op1dt2 != instToneDoubler[n].lstTD[us].OP1DT2)
        //                {
        //                    ((YM2151)pw.chip).OutSetDt2Sr(pw, 0, instToneDoubler[n].lstTD[us].OP1DT2, instFM[i][0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 3]);
        //                    pw.op1dt2 = instToneDoubler[n].lstTD[us].OP1DT2;
        //                }
        //                if (pw.op2dt2 != instToneDoubler[n].lstTD[us].OP2DT2)
        //                {
        //                    ((YM2151)pw.chip).OutSetDt2Sr(pw, 1, instToneDoubler[n].lstTD[us].OP2DT2, instFM[i][1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 3]);
        //                    pw.op2dt2 = instToneDoubler[n].lstTD[us].OP2DT2;
        //                }
        //                if (pw.op3dt2 != instToneDoubler[n].lstTD[us].OP3DT2)
        //                {
        //                    ((YM2151)pw.chip).OutSetDt2Sr(pw, 2, instToneDoubler[n].lstTD[us].OP3DT2, instFM[i][2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 3]);
        //                    pw.op3dt2 = instToneDoubler[n].lstTD[us].OP3DT2;
        //                }
        //                if (pw.op4dt2 != instToneDoubler[n].lstTD[us].OP4DT2)
        //                {
        //                    ((YM2151)pw.chip).OutSetDt2Sr(pw, 3, instToneDoubler[n].lstTD[us].OP4DT2, instFM[i][3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 3]);
        //                    pw.op4dt2 = instToneDoubler[n].lstTD[us].OP4DT2;
        //                }
        //            }
        //        }
        //        pw.TdA = -1;
        //    }
        //}

        private byte[] DivInt2ByteAry(int n)
        {
            return new byte[4] {
                 (byte)( n & 0xff                   )
                ,(byte)((n & 0xff00    ) / 0x100    )
                ,(byte)((n & 0xff0000  ) / 0x10000  )
                ,(byte)((n & 0xff000000) / 0x1000000)
            };
        }

        public void OutData(params byte[] data)
        {
            foreach(byte d in data) dat.Add(d);
        }

        private void OutWaitNSamples(long n)
        {
            long m = n;

            while (m > 0)
            {
                if (m > 0xffff)
                {
                    OutData(
                        0x61
                        , (byte)0xff
                        , (byte)0xff
                        );
                    m -= 0xffff;
                }
                else
                {
                    OutData(
                        0x61
                        , (byte)(m & 0xff)
                        , (byte)((m & 0xff00) >> 8)
                        );
                    m = 0L;
                }
            }
        }

        private void OutWait735Samples(int repeatCount)
        {
            for (int i = 0; i < repeatCount; i++)
            {
                OutData(0x62);
            }
        }

        private void OutWait882Samples(int repeatCount)
        {
            for (int i = 0; i < repeatCount; i++)
            {
                OutData(0x63);
            }
        }

        private void OutWaitNSamplesWithPCMSending(partWork cpw, long cnt)
        {
            for (int i = 0; i < info.samplesPerClock * cnt;)
            {

                int f = (int)cpw.pcmBaseFreqPerFreq;
                cpw.pcmFreqCountBuffer += cpw.pcmBaseFreqPerFreq - (int)cpw.pcmBaseFreqPerFreq;
                while (cpw.pcmFreqCountBuffer > 1.0f)
                {
                    f++;
                    cpw.pcmFreqCountBuffer -= 1.0f;
                }
                if (i + f >= info.samplesPerClock * cnt)
                {
                    cpw.pcmFreqCountBuffer += (int)(i + f - info.samplesPerClock * cnt);
                    f = (int)(info.samplesPerClock * cnt - i);
                }
                if (cpw.pcmSizeCounter > 0)
                {
                    cpw.pcmSizeCounter--;
                    OutData((byte)(0x80 + f));
                }
                else
                {
                    OutWaitNSamples(f);
                }
                i += f;
            }
        }


    }
}
