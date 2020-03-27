using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using musicDriverInterface;

namespace Core
{
    public class ClsVgm
    {

        public Conductor[] conductor = null;
        public YM2151[] ym2151 = null;
        public YM2203[] ym2203 = null;
        public YM2608[] ym2608 = null;
        public YM2609[] ym2609 = null;
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
        public K051649[] k051649 = null;
        public QSound[] qsound = null;
        public K053260[] k053260 = null;
        public MidiGM[] midiGM = null;

        public Dictionary<enmChipType, ClsChip[]> chips;


        public Dictionary<int, byte[]> instFM = new Dictionary<int, byte[]>();
        public Dictionary<int, int[]> instENV = new Dictionary<int, int[]>();
        public Dictionary<int, clsPcm> instPCM = new Dictionary<int, clsPcm>();
        public List<clsPcmDatSeq> instPCMDatSeq = new List<clsPcmDatSeq>();
        public Dictionary<int, Dictionary<int, int>> instPCMMap = new Dictionary<int, Dictionary<int, int>>();
        public Dictionary<int, clsToneDoubler> instToneDoubler = new Dictionary<int, clsToneDoubler>();
        public Dictionary<int, byte[]> instWF = new Dictionary<int, byte[]>();
        public Dictionary<int, ushort[]> instOPNA2WF = new Dictionary<int, ushort[]>();
        public Dictionary<int, byte[]> midiSysEx = new Dictionary<int, byte[]>();

        public Dictionary<string, List<Line>> partData = new Dictionary<string, List<Line>>();
        public Dictionary<string, Line> aliesData = new Dictionary<string, Line>();

        private int instrumentCounter = -1;
        private byte[] instrumentBufCache = new byte[Const.INSTRUMENT_SIZE];
        private int toneDoublerCounter = -1;
        private List<int> toneDoublerBufCache = new List<int>();
        private int wfInstrumentCounter = -1;
        private byte[] wfInstrumentBufCache = null;
        private int opna2wfInstrumentCounter = -1;
        private ushort[] opna2WfInstrumentBufCache = null;
        public bool doSkip = false;
        public bool doSkipStop = false;
        public Point caretPoint = Point.Empty;
        private int midiSysExCounter = -1;

        public int newStreamID = -1;

        public SourceParser sp = null;
        public Information info = null;

        public ClsVgm(string stPath, SourceParser sp)
        {
            this.sp = sp;
            this.info = sp.info;// new Information();

            chips = new Dictionary<enmChipType, ClsChip[]>();

            Tuple<string, string, List<string>, int[]> n;

            List<Conductor> lstCONDUCTOR = new List<Conductor>();
            n = sp.dicChipPartName[enmChipType.CONDUCTOR];
            for (int i = 0; i < n.Item3.Count; i++)
            {
                if (string.IsNullOrEmpty(n.Item3[i])) continue;
                if (sp.lnChipPartName.Contains(n.Item3[i]))
                    lstCONDUCTOR.Add(new Conductor(this, i, n.Item3[i], stPath, (info.format == enmFormat.ZGM ? 0 : i)));
            }
            if (lstCONDUCTOR.Count > 0)
            {
                conductor = lstCONDUCTOR.ToArray();
                chips.Add(enmChipType.CONDUCTOR, conductor);
            }

            List<YM2151> lstYM2151 = new List<YM2151>();
            n = sp.dicChipPartName[enmChipType.YM2151];
            for (int i = 0; i < n.Item3.Count; i++)
            {
                if (string.IsNullOrEmpty(n.Item3[i])) continue;
                if (sp.lnChipPartName.Contains(n.Item3[i]))
                    lstYM2151.Add(new YM2151(this, i, n.Item3[i], stPath, (info.format == enmFormat.ZGM ? 0 : i)));
            }
            if (lstYM2151.Count > 0)
            {
                ym2151 = lstYM2151.ToArray();
                chips.Add(enmChipType.YM2151, ym2151);
            }

            List<YM2203> lstYM2203 = new List<YM2203>();
            n = sp.dicChipPartName[enmChipType.YM2203];
            for (int i = 0; i < n.Item3.Count; i++)
            {
                if (string.IsNullOrEmpty(n.Item3[i])) continue;
                if (sp.lnChipPartName.Contains(n.Item3[i]))
                    lstYM2203.Add(new YM2203(this, i, n.Item3[i], stPath, (info.format == enmFormat.ZGM ? 0 : i)));
            }
            if (lstYM2203.Count > 0)
            {
                ym2203 = lstYM2203.ToArray();
                chips.Add(enmChipType.YM2203, ym2203);
            }

            List<YM2608> lstYM2608 = new List<YM2608>();
            n = sp.dicChipPartName[enmChipType.YM2608];
            for (int i = 0; i < n.Item3.Count; i++)
            {
                if (string.IsNullOrEmpty(n.Item3[i])) continue;
                if (sp.lnChipPartName.Contains(n.Item3[i]))
                    lstYM2608.Add(new YM2608(this, i, n.Item3[i], stPath, (info.format == enmFormat.ZGM ? 0 : i)));
            }
            if (lstYM2608.Count > 0)
            {
                ym2608 = lstYM2608.ToArray();
                chips.Add(enmChipType.YM2608, ym2608);
            }

            List<YM2609> lstYM2609 = new List<YM2609>();
            n = sp.dicChipPartName[enmChipType.YM2609];
            for (int i = 0; i < n.Item3.Count; i++)
            {
                if (string.IsNullOrEmpty(n.Item3[i])) continue;
                if (sp.lnChipPartName.Contains(n.Item3[i]))
                    lstYM2609.Add(new YM2609(this, i, n.Item3[i], stPath, (info.format == enmFormat.ZGM ? 0 : i)));
            }
            if (lstYM2609.Count > 0)
            {
                ym2609 = lstYM2609.ToArray();
                chips.Add(enmChipType.YM2609, ym2609);
            }

            List<YM2610B> lstYM2610B = new List<YM2610B>();
            n = sp.dicChipPartName[enmChipType.YM2610B];
            for (int i = 0; i < n.Item3.Count; i++)
            {
                if (string.IsNullOrEmpty(n.Item3[i])) continue;
                if (sp.lnChipPartName.Contains(n.Item3[i]))
                    lstYM2610B.Add(new YM2610B(this, i, n.Item3[i], stPath, (info.format == enmFormat.ZGM ? 0 : i)));
            }
            if (lstYM2610B.Count > 0)
            {
                ym2610b = lstYM2610B.ToArray();
                chips.Add(enmChipType.YM2610B, ym2610b);
            }

            List<YM2612> lstYM2612 = new List<YM2612>();
            n = sp.dicChipPartName[enmChipType.YM2612];
            for (int i = 0; i < n.Item3.Count; i++)
            {
                if (string.IsNullOrEmpty(n.Item3[i])) continue;
                if (sp.lnChipPartName.Contains(n.Item3[i]))
                    lstYM2612.Add(new YM2612(this, i, n.Item3[i], stPath, (info.format == enmFormat.ZGM ? 0 : i)));
            }
            if (lstYM2612.Count > 0)
            {
                ym2612 = lstYM2612.ToArray();
                chips.Add(enmChipType.YM2612, ym2612);
            }

            List<SN76489> lstSN76489 = new List<SN76489>();
            n = sp.dicChipPartName[enmChipType.SN76489];
            for (int i = 0; i < n.Item3.Count; i++)
            {
                if (string.IsNullOrEmpty(n.Item3[i])) continue;
                if (sp.lnChipPartName.Contains(n.Item3[i]))
                    lstSN76489.Add(new SN76489(this, i, n.Item3[i], stPath, (info.format == enmFormat.ZGM ? 0 : i)));
            }
            if (lstSN76489.Count > 0)
            {
                sn76489 = lstSN76489.ToArray();
                chips.Add(enmChipType.SN76489, sn76489);
            }

            List<RF5C164> lstRF5C164 = new List<RF5C164>();
            n = sp.dicChipPartName[enmChipType.RF5C164];
            for (int i = 0; i < n.Item3.Count; i++)
            {
                if (string.IsNullOrEmpty(n.Item3[i])) continue;
                if (sp.lnChipPartName.Contains(n.Item3[i]))
                    lstRF5C164.Add(new RF5C164(this, i, n.Item3[i], stPath, (info.format == enmFormat.ZGM ? 0 : i)));
            }
            if (lstRF5C164.Count > 0)
            {
                rf5c164 = lstRF5C164.ToArray();
                chips.Add(enmChipType.RF5C164, rf5c164);
            }

            List<segaPcm> lstSEGAPCM = new List<segaPcm>();
            n = sp.dicChipPartName[enmChipType.SEGAPCM];
            for (int i = 0; i < n.Item3.Count; i++)
            {
                if (string.IsNullOrEmpty(n.Item3[i])) continue;
                if (sp.lnChipPartName.Contains(n.Item3[i]))
                    lstSEGAPCM.Add(new segaPcm(this, i, n.Item3[i], stPath, (info.format == enmFormat.ZGM ? 0 : i)));
            }
            if (lstSEGAPCM.Count > 0)
            {
                segapcm = lstSEGAPCM.ToArray();
                chips.Add(enmChipType.SEGAPCM, segapcm);
            }

            List<HuC6280> lstHuC6280 = new List<HuC6280>();
            n = sp.dicChipPartName[enmChipType.HuC6280];
            for (int i = 0; i < n.Item3.Count; i++)
            {
                if (string.IsNullOrEmpty(n.Item3[i])) continue;
                if (sp.lnChipPartName.Contains(n.Item3[i]))
                    lstHuC6280.Add(new HuC6280(this, i, n.Item3[i], stPath, (info.format == enmFormat.ZGM ? 0 : i)));
            }
            if (lstHuC6280.Count > 0)
            {
                huc6280 = lstHuC6280.ToArray();
                chips.Add(enmChipType.HuC6280, huc6280);
            }

            List<YM2612X> lstYM2612X = new List<YM2612X>();
            n = sp.dicChipPartName[enmChipType.YM2612X];
            for (int i = 0; i < n.Item3.Count; i++)
            {
                if (string.IsNullOrEmpty(n.Item3[i])) continue;
                if (sp.lnChipPartName.Contains(n.Item3[i]))
                    lstYM2612X.Add(new YM2612X(this, i, n.Item3[i], stPath, (info.format == enmFormat.ZGM ? 0 : i)));
            }
            if (lstYM2612X.Count > 0)
            {
                ym2612x = lstYM2612X.ToArray();
                chips.Add(enmChipType.YM2612X, ym2612x);
            }

            List<YM2413> lstYM2413 = new List<YM2413>();
            n = sp.dicChipPartName[enmChipType.YM2413];
            for (int i = 0; i < n.Item3.Count; i++)
            {
                if (string.IsNullOrEmpty(n.Item3[i])) continue;
                if (sp.lnChipPartName.Contains(n.Item3[i]))
                    lstYM2413.Add(new YM2413(this, i, n.Item3[i], stPath, (info.format == enmFormat.ZGM ? 0 : i)));
            }
            if (lstYM2413.Count > 0)
            {
                ym2413 = lstYM2413.ToArray();
                chips.Add(enmChipType.YM2413, ym2413);
            }

            List<C140> lstC140 = new List<C140>();
            n = sp.dicChipPartName[enmChipType.C140];
            for (int i = 0; i < n.Item3.Count; i++)
            {
                if (string.IsNullOrEmpty(n.Item3[i])) continue;
                if (sp.lnChipPartName.Contains(n.Item3[i]))
                    lstC140.Add(new C140(this, i, n.Item3[i], stPath, (info.format == enmFormat.ZGM ? 0 : i)));
            }
            if (lstC140.Count > 0)
            {
                c140 = lstC140.ToArray();
                chips.Add(enmChipType.C140, c140);
            }

            List<AY8910> lstAY8910 = new List<AY8910>();
            n = sp.dicChipPartName[enmChipType.AY8910];
            for (int i = 0; i < n.Item3.Count; i++)
            {
                if (string.IsNullOrEmpty(n.Item3[i])) continue;
                if (sp.lnChipPartName.Contains(n.Item3[i]))
                    lstAY8910.Add(new AY8910(this, i, n.Item3[i], stPath, (info.format == enmFormat.ZGM ? 0 : i)));
            }
            if (lstAY8910.Count > 0)
            {
                ay8910 = lstAY8910.ToArray();
                chips.Add(enmChipType.AY8910, ay8910);
            }

            List<K051649> lstK051649 = new List<K051649>();
            n = sp.dicChipPartName[enmChipType.K051649];
            for (int i = 0; i < n.Item3.Count; i++)
            {
                if (string.IsNullOrEmpty(n.Item3[i])) continue;
                if (sp.lnChipPartName.Contains(n.Item3[i]))
                    lstK051649.Add(new K051649(this, i, n.Item3[i], stPath, (info.format == enmFormat.ZGM ? 0 : i)));
            }
            if (lstK051649.Count > 0)
            {
                k051649 = lstK051649.ToArray();
                chips.Add(enmChipType.K051649, k051649);
            }

            List<QSound> lstQSound = new List<QSound>();
            n = sp.dicChipPartName[enmChipType.QSound];
            for (int i = 0; i < n.Item3.Count; i++)
            {
                if (string.IsNullOrEmpty(n.Item3[i])) continue;
                if (sp.lnChipPartName.Contains(n.Item3[i]))
                    lstQSound.Add(new QSound(this, i, n.Item3[i], stPath, (info.format == enmFormat.ZGM ? 0 : i)));
            }
            if (lstQSound.Count > 0)
            {
                qsound = lstQSound.ToArray();
                chips.Add(enmChipType.QSound, qsound);
            }

            List<K053260> lstK053260 = new List<K053260>();
            n = sp.dicChipPartName[enmChipType.K053260];
            for (int i = 0; i < n.Item3.Count; i++)
            {
                if (string.IsNullOrEmpty(n.Item3[i])) continue;
                if (sp.lnChipPartName.Contains(n.Item3[i]))
                    lstK053260.Add(new K053260(this, i, n.Item3[i], stPath, (info.format == enmFormat.ZGM ? 0 : i)));
            }
            if (lstK053260.Count > 0)
            {
                k053260 = lstK053260.ToArray();
                chips.Add(enmChipType.K053260, k053260);
            }

            List<MidiGM> lstMidiGM = new List<MidiGM>();
            n = sp.dicChipPartName[enmChipType.MIDI_GM];
            for (int i = 0; i < n.Item3.Count; i++)
            {
                if (string.IsNullOrEmpty(n.Item3[i])) continue;
                if (sp.lnChipPartName.Contains(n.Item3[i]))
                    lstMidiGM.Add(new MidiGM(this, i, n.Item3[i], stPath, (info.format == enmFormat.ZGM ? 0 : i)));
            }
            if (lstMidiGM.Count > 0)
            {
                midiGM = lstMidiGM.ToArray();
                chips.Add(enmChipType.MIDI_GM, midiGM);
            }

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

            // Information
            info.AddInformation(sp.lnInformation, chips);
            foreach (Line line in sp.lnInstrument) AddInstrument(line);
            foreach (Line line in sp.lnAlies) AddAlies(line);
            foreach (Line line in sp.lnPart) AddPart(line);

            // 定義中のToneDoublerがあればここで定義完了
            if (toneDoublerCounter != -1)
            {
                toneDoublerCounter = -1;
                SetInstToneDoubler();
            }

            // 定義中のMidiSysExがあればここで定義完了
            if (midiSysExCounter != -1)
            {
                midiSysExCounter = -1;
                SetMidiSysEx();
            }

            // チェック1定義されていない名称を使用したパートが存在するか

            foreach (string p in partData.Keys)
            {
                bool flg = false;
                foreach (KeyValuePair<enmChipType,ClsChip[]> kvp in chips)
                {
                    foreach (ClsChip chip in kvp.Value)
                    {
                        if (chip == null) continue;

                        if (chip.ChannelNameContains(p))
                        {
                            flg = true;
                            break;
                        }
                    }
                }
                if (!flg)
                {
                    msgBox.setWrnMsg(string.Format(
                        msg.get("E01000")
                        , p.Substring(0, 2).Trim() + int.Parse(p.Substring(2, 2)).ToString()
                        ),
                        new LinePos("-")
                        );
                    flg = false;
                }
            }

            if (info.userClockCount != 0) info.clockCount = info.userClockCount;

            log.Write("テキスト解析完了");
            return 0;

        }

        private int AddInstrument(Line line)
        {
            string buf = line.Txt.Trim().Replace("'@", "").Trim();

            if (buf.Length < 1)
            {
                msgBox.setWrnMsg(msg.get("E01001"), line.Lp);
                return -1;
            }

            if (buf.ToUpper().IndexOf("MUCOM88ADPCM") == 0)
            {
                defineMUCOM88ADPCMInstrument(line);
                return 0;
            }

            if (buf.ToUpper().IndexOf("MUCOM88") == 0)
            {
                defineMUCOM88Instrument(line);
                return 0;
            }

            if (buf.ToUpper().IndexOf("TFI") == 0)
            {
                defineTFIInstrument(line);
                return 0;
            }

            // FMの音色を定義中の場合
            if (instrumentCounter != -1)
            {

                return SetInstrument(line);

            }

            // WaveFormの音色を定義中の場合
            if (wfInstrumentCounter != -1)
            {

                return SetWfInstrument(line);

            }

            // opna2 WaveFormの音色を定義中の場合
            if (opna2wfInstrumentCounter != -1)
            {

                return SetOPNA2WfInstrument(line);

            }

            char t = buf.ToUpper()[0];
            if (toneDoublerCounter != -1)
            {
                //他の定義が現れたらtoneDoublerの定義は終了
                if (t == 'F' || t == 'N' || t == 'M' || t == 'L' || t == 'P' || t == 'E' || t == 'T' || t == 'H' || t == 'W' || t == 'S')
                {
                    toneDoublerCounter = -1;
                    SetInstToneDoubler();
                }
            }

            if (midiSysExCounter != -1)
            {
                //他の定義が現れたらSysExの定義は終了
                if (t == 'F' || t == 'N' || t == 'M' || t == 'L' || t == 'P' || t == 'E' || t == 'T' || t == 'H' || t == 'W' || t == 'S')
                {
                    midiSysExCounter = -1;
                    SetMidiSysEx();
                }
            }

            switch (t)
            {
                case 'F':
                    instrumentBufCache = new byte[Const.INSTRUMENT_SIZE - 8];
                    instrumentCounter = 0;
                    SetInstrument(line);
                    return 0;

                case 'N':
                    instrumentBufCache = new byte[Const.INSTRUMENT_SIZE];
                    instrumentCounter = 0;
                    SetInstrument(line);
                    return 0;

                case 'M':
                    instrumentBufCache = new byte[Const.INSTRUMENT_SIZE];
                    instrumentCounter = 0;
                    SetInstrument(line);
                    return 0;

                case 'L':
                    instrumentBufCache = new byte[Const.OPL_INSTRUMENT_SIZE];
                    instrumentCounter = 0;
                    SetInstrument(line);
                    return 0;

                case 'A':
                    instrumentBufCache = new byte[Const.OPNA2_INSTRUMENT_SIZE];
                    instrumentCounter = 0;
                    SetInstrument(line);
                    return 0;

                case 'P':
                    definePCMInstrument(line);
                    return 0;

                case 'E':
                    try
                    {
                        instrumentCounter = -1;
                        string[] vs = buf.Substring(1).Trim().Split(new string[] { "," }, StringSplitOptions.None);
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
                        if (chips.ContainsKey(chiptype))
                        {
                            ClsChip c = chips[chiptype][0] != null ? chips[chiptype][0] : chips[chiptype][1];
                            if (c.Envelope != null)
                            {
                                CheckEnvelopeVolumeRange(line, env, c.Envelope.Max, c.Envelope.Min);
                                if (env[7] == 0) env[7] = 1;
                            }
                        }
                        else
                        {
                            msgBox.setWrnMsg(msg.get("E01004"), line.Lp);
                        }

                        if (instENV.ContainsKey(num))
                        {
                            instENV.Remove(num);
                        }
                        instENV.Add(num, env);
                    }
                    catch
                    {
                        msgBox.setWrnMsg(msg.get("E01005"), line.Lp);
                    }
                    return 0;

                case 'T':
                    try
                    {
                        instrumentCounter = -1;

                        if (buf.ToUpper()[1] != 'D') return 0;

                        toneDoublerBufCache.Clear();
                        StoreToneDoublerBuffer(buf.ToUpper().Substring(2).TrimStart(), line);
                    }
                    catch
                    {
                        msgBox.setWrnMsg(msg.get("E01006"), line.Lp);
                    }
                    return 0;

                case 'H':
                    wfInstrumentBufCache = new byte[Const.WF_INSTRUMENT_SIZE];
                    wfInstrumentCounter = 0;
                    SetWfInstrument(line);
                    return 0;

                case 'W':
                    opna2WfInstrumentBufCache = new ushort[Const.OPNA2_WF_INSTRUMENT_SIZE];
                    opna2wfInstrumentCounter = 0;
                    SetOPNA2WfInstrument(line);
                    return 0;

                case 'S':
                    midiSysExCounter = 0;
                    StoreMidiSysExBuffer(line);
                    return 0;
            }

            // ToneDoublerを定義中の場合
            if (toneDoublerCounter != -1)
            {
                return StoreToneDoublerBuffer(buf.ToUpper(), line);
            }

            if (midiSysExCounter != -1)
            {
                return StoreMidiSysExBuffer(line);
            }

            return 0;
        }

        private void defineMUCOM88ADPCMInstrument(Line line)
        {
            try
            {
                string[] vs = line.Txt.Trim().Substring(2).Trim().Substring(12).Trim().Split(new string[] { "," }, StringSplitOptions.None);
                if (vs.Length < 1) throw new ArgumentOutOfRangeException();
                for (int i = 0; i < vs.Length; i++) vs[i] = vs[i].Trim();

                int num = Common.ParseNumber(vs[0]);
                string fn = vs[1].Trim().Trim('"');
                fn = fn.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
                if (!File.Exists(fn))
                {
                    //fn = Path.Combine(line.Lp.path, fn);
                    fn = Path.Combine(wrkPath, fn);
                    fn = fn.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
                }

                byte[] pcmdat = File.ReadAllBytes(fn);
                mucomADPCM2PCM.initial(pcmdat);
                List<mucomADPCM2PCM.mucomPCMInfo> lstInfo = mucomADPCM2PCM.lstMucomPCMInfo;

                foreach (mucomADPCM2PCM.mucomPCMInfo info in lstInfo)
                {
                    clsPcmDatSeq pds = new clsPcmDatSeq(
                        enmPcmDefineType.Mucom88
                        , info.no+num
                        , info.name
                        , 8000
                        , 150
                        , enmChipType.YM2612
                        , 0
                        , -1
                        );

                    instPCMDatSeq.Add(pds);
                }

                return;
            }
            catch
            {
                msgBox.setWrnMsg(msg.get("E01019"), line.Lp);
            }
        }

        private void defineMUCOM88Instrument(Line line)
        {
            try
            {
                string[] vs = line.Txt.Trim().Substring(2).Trim().Substring(7).Trim().Split(new string[] { "," }, StringSplitOptions.None);
                if (vs.Length < 1) throw new ArgumentOutOfRangeException();
                for (int i = 0; i < vs.Length; i++) vs[i] = vs[i].Trim();

                int num = Common.ParseNumber(vs[0]);
                string fn = vs[1].Trim().Trim('"');
                fn = fn.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
                if (!File.Exists(fn))
                {
                    //fn = Path.Combine(line.Lp.path, fn);
                    fn = Path.Combine(wrkPath, fn);
                    fn = fn.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
                }

                byte[] buf = File.ReadAllBytes(fn);
                for (int p = 0; p < buf.Length; p += 32)
                {
                    byte[] voi = new byte[26];
                    Array.Copy(buf, p + 0, voi, 0, 26);
                    if (instFM.ContainsKey(num + p / 32))
                    {
                        instFM.Remove(num + p / 32);
                    }
                    instFM.Add(num + p / 32, ConvertMUCOM88toM(num,voi));
                }

                return;
            }
            catch
            {
                msgBox.setWrnMsg(msg.get("E01018"), line.Lp);
            }
        }

        private byte[] ConvertMUCOM88toM(int num, byte[] voi)
        {
            List<byte> ret = new List<byte>();
            ret.Add((byte)voi[0]);

            int[] op = new int[] { 0, 2, 1, 3 };
            int sft = 1;
            for (int i = 0; i < 4; i++)
            {
                ret.Add((byte)(voi[8 + op[i] + sft] & 0x1f));//AR OP1   +0
                ret.Add((byte)(voi[12 + op[i] + sft] & 0x1f));//DR OP1   +1
                ret.Add((byte)(voi[16 + op[i] + sft] & 0x1f));//SR OP1   +2
                ret.Add((byte)(voi[20 + op[i] + sft] & 0x0f));//RR OP1   +3
                ret.Add((byte)((voi[20 + op[i] + sft] >> 4) & 0x0f));//SL OP1   +4
                ret.Add((byte)(voi[4 + op[i] + sft] & 0x7f));//TL OP1   +5
                ret.Add((byte)((voi[8 + op[i] + sft] >> 6) & 0x03));//KS OP1   +6
                ret.Add((byte)((voi[0 + op[i] + sft] >> 0) & 0x0f));//ML OP1   +7
                ret.Add((byte)((voi[0 + op[i] + sft] >> 4) & 0x07));//DT1 OP1   +8
                ret.Add(1);//AM OP1   +9
                ret.Add(0);//SSGEG OP1   +10
            }
            ret.Add((byte)(voi[4 * 6 + sft] & 0x7));//ALG
            ret.Add((byte)((voi[4 * 6 + sft] >> 3) & 0x7));//FB

            return ret.ToArray();
        }

        private void defineTFIInstrument(Line line)
        {
            try
            {
                string[] vs = line.Txt.Trim().Substring(2).Trim().Substring(3).Trim().Split(new string[] { "," }, StringSplitOptions.None);
                if (vs.Length < 1) throw new ArgumentOutOfRangeException();
                for (int i = 0; i < vs.Length; i++) vs[i] = vs[i].Trim();

                int num = Common.ParseNumber(vs[0]);
                string fn = vs[1].Trim().Trim('"');
                byte[] buf;
                fn = fn.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
                if (!File.Exists(fn))
                {
                    //fn = Path.Combine(line.Lp.path, fn);
                    fn = Path.Combine(wrkPath, fn);
                    fn = fn.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
                }
                buf = File.ReadAllBytes(fn);

                if (instFM.ContainsKey(num))
                {
                    instFM.Remove(num);
                }
                instFM.Add(num, ConvertTFItoM(num, buf));

                return;
            }
            catch
            {
                msgBox.setWrnMsg(msg.get("E01021"), line.Lp);
            }
        }

        private byte[] ConvertTFItoM(int num, byte[] voi)
        {
            List<byte> ret = new List<byte>();
            ret.Add((byte)num);

            int[] op = new int[] { 0, 2, 1, 3 };
            int sft = 2;
            for (int i = 0; i < 4; i++)
            {
                ret.Add((byte)(voi[0x04 + op[i] * 10 + sft] & 0x1f));//AR OP1   +0
                ret.Add((byte)(voi[0x05 + op[i] * 10 + sft] & 0x1f));//DR OP1   +1
                ret.Add((byte)(voi[0x06 + op[i] * 10 + sft] & 0x1f));//SR OP1   +2
                ret.Add((byte)(voi[0x07 + op[i] * 10 + sft] & 0x0f));//RR OP1   +3
                ret.Add((byte)(voi[0x08 + op[i] * 10 + sft] & 0x0f));//SL OP1   +4
                ret.Add((byte)(voi[0x02 + op[i] * 10 + sft] & 0x7f));//TL OP1   +5
                ret.Add((byte)(voi[0x03 + op[i] * 10 + sft] & 0x03));//KS OP1   +6
                ret.Add((byte)(voi[0x00 + op[i] * 10 + sft] & 0x0f));//ML OP1   +7
                ret.Add((byte)((voi[0x01 + op[i] * 10 + sft] & 0x07) - 3));//DT1 OP1   +8
                ret.Add(1);//AM OP1   +9
                ret.Add((byte)(voi[0x09 + op[i] * 10 + sft] & 0x0f));//SSGEG OP1   +10
            }
            ret.Add((byte)(voi[0] & 0x7));//ALG
            ret.Add((byte)(voi[1] & 0x7));//FB

            return ret.ToArray();
        }

        private void definePCMInstrument(Line line)
        {
            try
            {
                string[] vs = line.Txt.Trim().Substring(2).Trim().Substring(1).Trim().Split(new string[] { "," }, StringSplitOptions.None);
                if (vs.Length < 1) throw new ArgumentOutOfRangeException();
                for (int i = 0; i < vs.Length; i++) vs[i] = vs[i].Trim();

                switch (vs[0][0])
                {
                    case 'D':
                        definePCMInstrumentRawData(line, vs);
                        break;
                    case 'I':
                        definePCMInstrumentSet(line, vs);
                        break;
                    case 'M':
                        definePCMMapModeSet(line, vs);
                        break;
                    default:
                        definePCMInstrumentEasy(line, vs);
                        break;
                }

                return;
            }
            catch
            {
                msgBox.setWrnMsg(msg.get("E01003"), line.Lp);
            }
        }

        /// <summary>
        /// '@ P No , "FileName" , [BaseFreq] , Volume ( , [ChipName] , [Option] )
        /// </summary>
        private void definePCMInstrumentEasy(Line line, string[] vs)
        {
            instrumentCounter = -1;
            enmChipType enmChip = enmChipType.YM2612;

            int num = Common.ParseNumber(vs[0]);

            string fn = vs[1].Trim().Trim('"');

            int fq;
            try
            {
                if (string.IsNullOrEmpty(vs[2])) fq = -1;
                else fq = Common.ParseNumber(vs[2]);
            }
            catch
            {
                fq = -1;
            }

            int vol;
            try
            {
                if (string.IsNullOrEmpty(vs[3])) vol = 100;
                else vol = Common.ParseNumber(vs[3]);
            }
            catch
            {
                vol = 100;
            }

            int chipNumber = 0;

            if (vs.Length > 4)
            {
                enmChip = GetChipTypeForPCM(line, vs[4], out chipNumber);
                if (enmChip == enmChipType.None) return;
            }

            if (info.format == enmFormat.XGM)
            {
                if(enmChip!= enmChipType.YM2612X)
                {
                    msgBox.setErrMsg(msg.get("E01017"), line.Lp);
                    return;
                }
            }

            int opt = -1;

            if (vs.Length > 5)
            {
                try
                {
                    opt = Common.ParseNumber(vs[5]);
                }
                catch
                {
                    opt = -1;
                }
            }

            //optionが未指定の場合の初期値を設定
            if (opt == -1)
            {
                if (enmChip == enmChipType.YM2610B)
                {
                    opt = 0;
                }

                if (enmChip == enmChipType.YM2612X)
                {
                    opt = 36;
                }
            }

            instPCMDatSeq.Add(new clsPcmDatSeq(
                enmPcmDefineType.Easy
                , num
                , fn
                , fq
                , vol
                , enmChip
                , chipNumber
                , opt
                ));

            //if (instPCM.ContainsKey(num))
            //{
            //    instPCM.Remove(num);
            //}
            //instPCM.Add(num, new clsPcm(num, pcmDataSeqNum++, enmChip, chipNumber, fn, fq, vol, 0, 0, 0, lp, false, 8000));
        }

        /// <summary>
        /// '@ PD "FileName" , ChipName , [SrcStartAdr] , [DesStartAdr] , [Length] , [Option]
        /// </summary>
        private void definePCMInstrumentRawData(Line line, string[] vs)
        {

            string FileName = vs[0].Substring(1).Trim().Trim('"');
            enmChipType ChipName = GetChipTypeForPCM(line, vs[1], out int chipNumber);

            if (info.format == enmFormat.XGM)
            {
                if (ChipName != enmChipType.YM2612X)
                {
                    msgBox.setErrMsg(msg.get("E01017"), line.Lp);
                    return;
                }
            }

            int SrcStartAdr = 0;
            if (vs.Length > 2 && !string.IsNullOrEmpty(vs[2].Trim()))
            {
                SrcStartAdr = Common.ParseNumber(vs[2]);
            }
            int DesStartAdr = 0;
            if (vs.Length > 3 && !string.IsNullOrEmpty(vs[3].Trim()))
            {
                DesStartAdr = Common.ParseNumber(vs[3]);
            }
            int Length = -1;
            if (vs.Length > 4 && !string.IsNullOrEmpty(vs[4].Trim()))
            {
                Length = Common.ParseNumber(vs[4]);
            }
            string[] Option = null;
            if (vs.Length > 5)
            {
                Option = new string[vs.Length - 5];
                Array.Copy(vs, 5, Option, 0, vs.Length - 5);
            }

            instPCMDatSeq.Add(new clsPcmDatSeq(
                enmPcmDefineType.RawData
                , FileName
                , ChipName
                , chipNumber
                , SrcStartAdr
                , DesStartAdr
                , Length
                , Option
                ));

        }

        /// <summary>
        /// '@ PI No , ChipName , [BaseFreq] , StartAdr , EndAdr , [LoopAdr] , [Option]
        /// </summary>
        private void definePCMInstrumentSet(Line line, string[] vs)
        {
            int num = Common.ParseNumber(vs[0].Substring(1));
            enmChipType ChipName = GetChipTypeForPCM(line, vs[1], out int chipNumber);
            if (ChipName == enmChipType.None) return;

            if (info.format == enmFormat.XGM)
            {
                if (ChipName != enmChipType.YM2612X)
                {
                    msgBox.setErrMsg(msg.get("E01017"), line.Lp);
                    return;
                }
            }

            if (chips[ChipName] != null)
            {
                if (!chips[ChipName][0].CanUsePICommand())
                {
                    msgBox.setWrnMsg(string.Format(msg.get("E10018"), chips[ChipName][0].Name), line.Lp);
                    return;
                }
            }

            int BaseFreq;
            //if (vs.Length > 2 && !string.IsNullOrEmpty(vs[2].Trim()))
            //{
            try
            {
                BaseFreq = Common.ParseNumber(vs[2]);
            }
            catch
            {
                BaseFreq = 8000;
            }

            //StartAdr省略不可
            int StartAdr = 0;
            StartAdr = Common.ParseNumber(vs[3]);

            //EndAdr省略不可(RF5C164は設定不可)
            int EndAdr = 0;
            if (ChipName != enmChipType.RF5C164)
            {
                EndAdr = Common.ParseNumber(vs[4]);
            }
            else
            {
                if (!string.IsNullOrEmpty(vs[4].ToString()))
                    throw new ArgumentOutOfRangeException();
            }

            //LoopAdr(RF5C164は省略不可)
            int LoopAdr;
            if (ChipName != enmChipType.RF5C164)
            {
                LoopAdr = (ChipName != enmChipType.YM2610B) ? -1 : 0;
                if (vs.Length > 5 && !string.IsNullOrEmpty(vs[5].Trim()))
                {
                    LoopAdr = Common.ParseNumber(vs[5]);
                }
            }
            else
            {
                LoopAdr = Common.ParseNumber(vs[5]);
            }

            string[] Option = null;
            if (vs.Length > 6)
            {
                Option = new string[vs.Length - 6];
                Array.Copy(vs, 6, Option, 0, vs.Length - 6);
            }
            if(ChipName== enmChipType.YM2610B)
            {
                if (Option == null || Option.Length < 1)
                {
                    LoopAdr = 0;
                }
                else
                {
                    LoopAdr = 1;
                    if (Option[0].Trim() != "1")
                    {
                        LoopAdr = 0;
                    }
                }
            }

            instPCMDatSeq.Add(new clsPcmDatSeq(
                enmPcmDefineType.Set
                , num
                , ChipName
                , chipNumber
                , BaseFreq
                , StartAdr
                , EndAdr
                , LoopAdr
                , Option
                ));

        }

        /// <summary>
        /// '@ PM Octave , Note , No
        /// </summary>
        private void definePCMMapModeSet(Line line, string[] vs)
        {
            int map = Common.ParseNumber(vs[0].Substring(1));
            map = Math.Min(Math.Max(map, 0), 255);
            int oct = Common.ParseNumber(vs[1]);
            oct = Math.Min(Math.Max(oct, 1), 8);
            int note = Common.ParseNumber(vs[2]);
            note = Math.Min(Math.Max(note, 0), 11);

            if (!instPCMMap.ContainsKey(map))
            {
                instPCMMap.Add(map, new Dictionary<int, int>());
            }

            int i = 0;
            while (i < vs.Length - 3)
            {
                int no;
                if (string.IsNullOrEmpty(vs[3 + i]))
                {
                    i++;
                    continue;
                }
                no = Common.ParseNumber(vs[3 + i]);
                no = Math.Min(Math.Max(no, 0), 255);

                if (instPCMMap[map].ContainsKey(oct * 12 + note + i))
                {
                    instPCMMap[map].Remove(oct * 12 + note + i);
                }
                instPCMMap[map].Add(oct * 12 + note + i, no);
                i++;
            }
        }

        private static void CheckEnvelopeVolumeRange(Line line, int[] env, int max, int min)
        {
            for (int i = 0; i < env.Length - 1; i++)
            {
                if (i != 1 && i != 4 && i != 7) continue;

                if (env[i] > max)
                {
                    env[i] = max;
                    msgBox.setWrnMsg(string.Format(msg.get("E01007"), max), line.Lp);
                }
                if (env[i] < min)
                {
                    env[i] = min;
                    msgBox.setWrnMsg(string.Format(msg.get("E01008"), min), line.Lp);
                }
            }
        }


        private enmChipType GetChipTypeForPCM(Line line, string strChip, out int chipNumber)
        {
            enmChipType enmChip = enmChipType.YM2612;
            string chipName = strChip.Trim().ToUpper();
            chipNumber = 0;
            if (chipName == "") return enmChipType.YM2612;

            if (chipName.IndexOf(Information.PRIMARY) >= 0)
            {
                chipNumber = 0;
                chipName = chipName.Replace(Information.PRIMARY, "");
            }
            else if (chipName.IndexOf(Information.SECONDARY) >= 0)
            {
                chipNumber = 1;
                chipName = chipName.Replace(Information.SECONDARY, "");
            }

            ClsChip cp=null;
            cp = GetChip(chipName);
            if (cp == null)
            {
                //パート名が指定されている場合はそれからチップが取得できるかためす
                cp = GetChipFromPartName(chipName);
            }

            if (cp==null || !cp.CanUsePcm)
            {
                msgBox.setWrnMsg(string.Format(msg.get("E01002"), chipName), line.Lp);
                return enmChipType.None;
            }
            enmChip = GetChipType(chipName);
            if(enmChip== enmChipType.None)
            {
                //パート名が指定されている場合はそれからチップTypeが取得できるかためす
                enmChip = GetChipTypeFromPartName(chipName);
            }

            return enmChip;
        }

        private enmChipType GetChipType(string chipN)
        {
            foreach(KeyValuePair<enmChipType,ClsChip[]> kvp in chips)
            {
                foreach(ClsChip chip in kvp.Value)
                {
                    if (chip == null) continue;
                    if (chip.Name.ToUpper().Trim() == chipN.ToUpper().Trim()
                        || chip.ShortName.ToUpper().Trim() == chipN.ToUpper().Trim())
                    {
                        return kvp.Key;
                    }
                }
            }

            return enmChipType.None;
        }

        private enmChipType GetChipTypeFromPartName(string partN)
        {
            foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in chips)
            {
                foreach (ClsChip chip in kvp.Value)
                {
                    if (chip == null) continue;
                    if (chip.PartName == partN)
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
                    if (chip == null) continue;
                    if (n == chip.Name.ToUpper()) return chip;
                    if (n == chip.ShortName.ToUpper()) return chip;
                }
            }

            return null;
        }

        private ClsChip GetChipFromPartName(string partN)
        {
            foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in chips)
            {
                foreach (ClsChip chip in kvp.Value)
                {
                    if (chip == null) continue;
                    if (partN == chip.PartName) return chip;
                }
            }
            return null;
        }

        private int AddAlies(Line line)
        {
            string name = "";
            string data = "";
            string buf = line.Txt.Trim().Substring(2).Trim();

            int i = buf.IndexOfAny(new char[] { ' ', '\t' });
            if (i < 0)
            {
                //空白による区切りが見つからない場合は無視する
                return 0;
            }

            name = buf.Substring(0, i).Trim();
            data = buf.Substring(i).Trim();
            if (name == "")
            {
                //エイリアス指定がない場合は警告とする
                msgBox.setWrnMsg(msg.get("E01009"), line.Lp);
                return -1;
            }
            if (data == "")
            {
                //データがない場合は警告する
                msgBox.setWrnMsg(msg.get("E01010"), line.Lp);
            }

            if (aliesData.ContainsKey(name))
            {
                aliesData.Remove(name);
            }
            Line l = new Line(new LinePos(
                line.Lp.srcMMLID,
                line.Lp.row,
                line.Lp.col,
                line.Lp.length,
                line.Lp.part,
                line.Lp.chip,
                line.Lp.chipIndex,
                line.Lp.chipNumber,
                line.Lp.ch), line.Txt);
            l.Lp.col = buf.IndexOfAny(new char[] { ' ', '\t' }) + 3;
            aliesData.Add(name, l);

            return 0;
        }

        private int AddPart(Line line)
        {
            List<string> part = new List<string>();
            string data = "";
            string buf = line.Txt;

            int i = buf.IndexOfAny(new char[] { ' ', '\t' });
            if (i < 0)
            {
                //空白による区切りが見つからない場合は無視する
                return 0;
            }

            part = Common.DivParts(buf.Substring(1, i).Trim(), chips);
            data = buf.Substring(i).Trim();
            if (part == null)
            {
                //パート指定がない場合は警告とする
                msgBox.setWrnMsg(msg.get("E01011"), line.Lp);
                return -1;
            }
            if (data == "")
            {
                ////データがない場合は無視する
                //return 0;
                line.Txt += " ";//スキップ再生に対応するためダミーの空白を強制的に入れる
            }

            foreach (string p in part)
            {
                if (!partData.ContainsKey(p))
                {
                    partData.Add(p, new List<Line>());
                }
                Line l = new Line(new LinePos(
                    line.Lp.srcMMLID,
                    line.Lp.row,
                    line.Lp.col,
                    line.Lp.length,
                    line.Lp.part,
                    line.Lp.chip,
                    line.Lp.chipIndex,
                    line.Lp.chipNumber,
                    line.Lp.ch), line.Txt);
                l.Lp.col = i + 1;
                partData[p].Add(l);
            }

            return 0;
        }

        private int SetInstrument(Line line)
        {

            try
            {
                instrumentCounter= GetNums(instrumentBufCache, instrumentCounter, line.Txt.Substring(1).TrimStart());

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
                    else if (instrumentBufCache.Length == Const.OPL_INSTRUMENT_SIZE)
                    {
                        //OPL
                        instFM.Add(instrumentBufCache[0], instrumentBufCache);
                    }
                    else if (instrumentBufCache.Length == Const.OPNA2_INSTRUMENT_SIZE)
                    {
                        //OPNA2
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
                msgBox.setErrMsg(msg.get("E01012"), line.Lp);
            }

            return 0;
        }

        private int SetWfInstrument(Line line)
        {

            try
            {
                wfInstrumentCounter = GetNums(wfInstrumentBufCache, wfInstrumentCounter, line.Txt.Substring(1).TrimStart());

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
                msgBox.setErrMsg(msg.get("E01013"), line.Lp);
            }

            return 0;
        }

        private int SetOPNA2WfInstrument(Line line)
        {

            try
            {
                opna2wfInstrumentCounter = GetNums2(opna2WfInstrumentBufCache, opna2wfInstrumentCounter, line.Txt.Substring(1).TrimStart());

                if (opna2wfInstrumentCounter == opna2WfInstrumentBufCache.Length)
                {
                    if (instOPNA2WF.ContainsKey(opna2WfInstrumentBufCache[0]))
                    {
                        instOPNA2WF.Remove(opna2WfInstrumentBufCache[0]);
                    }
                    instOPNA2WF.Add(opna2WfInstrumentBufCache[0], opna2WfInstrumentBufCache);

                    opna2wfInstrumentCounter = -1;
                }
            }
            catch
            {
                msgBox.setErrMsg(msg.get("E01013"), line.Lp);
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

        private int GetNums2(ushort[] aryBuf, int aryIndex, string vals)
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
                    if (hc == 4)
                    {
                        i = int.Parse(h, System.Globalization.NumberStyles.HexNumber);
                        aryBuf[aryIndex] = (ushort)(i & 0xffff);
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
                    aryBuf[aryIndex] = (ushort)(i & 0xffff);
                    aryIndex++;
                    n = "";
                }
            }

            if (!string.IsNullOrEmpty(n))
            {
                if (int.TryParse(n, out i))
                {
                    aryBuf[aryIndex] = (ushort)(i & 0xffff);
                    aryIndex++;
                    n = "";
                }
            }

            return aryIndex;
        }

        private List<byte> GetNums(int ptr, string vals)
        {
            List<byte> lstBuf = new List<byte>();
            string n = "";
            string h = "";
            int hc = -1;
            int i = 0;
            int p = 0;

            foreach (char c in vals)
            {
                p++;
                if (p <= ptr) continue;

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
                        lstBuf.Add((byte)(i & 0xff));
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
                    lstBuf.Add((byte)(i & 0xff));
                    n = "";
                }
            }

            if (!string.IsNullOrEmpty(n))
            {
                if (int.TryParse(n, out i))
                {
                    lstBuf.Add((byte)(i & 0xff));
                    n = "";
                }
            }

            return lstBuf;
        }

        private int StoreToneDoublerBuffer(string vals, Line line)
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
                msgBox.setErrMsg(msg.get("E01014"), line.Lp);
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

        private int StoreMidiSysExBuffer(Line line)
        {
            try
            {
                List<byte> buf = null;
                if (midiSysExCounter == 0)
                {
                    buf = GetNums(line.Txt.IndexOf('S') + 1, line.Txt);
                    midiSysExCounter = buf[0] + 1;
                    if (midiSysEx.ContainsKey(buf[0]))
                    {
                        midiSysEx.Remove(buf[0]);
                    }
                    midiSysEx.Add(buf[0], buf.ToArray());
                }
                else
                {
                    buf = GetNums(line.Txt.IndexOf('@') + 1, line.Txt);
                    List<byte> ebuf = midiSysEx[midiSysExCounter - 1].ToList();
                    ebuf.AddRange(buf);
                    midiSysEx.Remove(midiSysExCounter - 1);
                    midiSysEx.Add(midiSysExCounter - 1, ebuf.ToArray());
                }
            }
            catch { }

            return 0;
        }


        private void SetMidiSysEx()
        {
            midiSysExCounter = -1;
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



        private List<outDatum> _dat = null;

        public List<outDatum> dat
        {
            get
            {
                return _dat;
            }
            set
            {
                _dat = value;
            }
        }

        //xgm music data
        private List<outDatum> _xdat = null;
        public List<outDatum> xdat
        {
            get
            {
                return _xdat;
            }
            set
            {
                _xdat = value;
            }
        }

        //xgm keyOnDataList
        public List<outDatum> xgmKeyOnData = null;

        public double dSample = 0.0;
        public long lClock = 0L;
        private double sampleB = 0.0;
        public string lyric = "";

        public long loopOffset = -1L;
        public long loopClock = -1L;
        public long loopSamples = -1L;

        private Random rnd = new Random();

        public int jumpCommandCounter = 0;
        public bool useJumpCommand = false;
        public bool useSkipPlayCommand = false;

        /// <summary>
        /// ダミーコマンドの総バイト数
        /// </summary>
        public long dummyCmdCounter = 0;
        /// <summary>
        /// ダミーコマンドの総クロック数
        /// </summary>
        public long dummyCmdClock = 0;
        /// <summary>
        /// ダミーコマンドの総サンプル数
        /// </summary>
        public long dummyCmdSample = 0;
        /// <summary>
        /// ダミーコマンドを含むLoopOffset
        /// </summary>
        public long dummyCmdLoopOffset = 0;
        /// <summary>
        /// ダミーコマンドを含むLoopOffset
        /// </summary>
        public long dummyCmdLoopClock = 0;
        /// <summary>
        /// ダミーコマンドを含むLoopOffset
        /// </summary>
        public long dummyCmdLoopSamples = 0;
        public long dummyCmdLoopOffsetAddress=0;

        public LinePos linePos { get; internal set; }
        public bool isRealTimeMode { get; set; }
        public int ChipCommandSize { get; set; }
        public string wrkPath { get; internal set; }

        public outDatum[] Vgm_getByteData(Dictionary<string, List<MML>> mmlData)
        {
            //スキップ再生の指定がある場合は、キャレット位置まで(SkipPlayコマンドが来るまで)ウェイトの発行をしない。
            if (doSkip)
            {
                useSkipPlayCommand = true;
            }

            dat = new List<outDatum>();

            log.Write("ヘッダー情報作成");
            MakeHeader();

            int endChannel = 0;
            newStreamID = -1;
            int totalChannel = 0;
            foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in chips)
            {
                foreach (ClsChip chip in kvp.Value)
                {
                    if (chip == null) continue;

                    totalChannel += chip.ChMax;
                }
            }

            log.Write("MML解析開始");
            long waitCounter = 0;
            do
            {
                log.Write("全パートコマンド解析");
                AnalyzeAllPartCommand();

                log.Write("全パートのうち次のコマンドまで一番近い値を求める");
                waitCounter = ComputeAllPartDistance();

                //log.Write("全パートのwaitcounterを減らす");
                //DecAllPartWaitCounter(waitCounter);

                log.Write("終了パートのカウント");
                endChannel = CountUpEndPart();

                if (endChannel < totalChannel)
                {
                    log.Write("全パートのwaitcounterを減らす");
                    DecAllPartWaitCounter(waitCounter);
                }

            } while (endChannel < totalChannel);

            //残カット
            //if (loopClock != -1 && waitCounter > 0 && waitCounter != long.MaxValue)
            //{
            //    lClock -= waitCounter;
            //    dSample -= (long)(info.samplesPerClock * waitCounter);
            //}
            if (loopClock == -1)
            {
                AllKeyOffEnv();
            }

            log.Write("フッター情報の作成");
            MakeFooter();

            return dat.ToArray();
        }

        private void AllKeyOffEnv()
        {
            foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in chips)
            {
                foreach (ClsChip chip in kvp.Value)
                {
                    if (chip == null) continue;
                    for (int i = 0; i < chip.lstPartWork.Count; i++)
                    {
                        partWork pw = chip.lstPartWork[
                            chip.ReversePartWork
                            ? (chip.lstPartWork.Count - 1 - i)
                            : i
                            ];
                        if (pw.envIndex != -1)
                        {
                            chip.SetKeyOff(pw, null);
                        }
                    }
                }
            }
        }

        public int CountUpEndPart()
        {
            int endChannel = 0;
            foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in chips)
            {
                foreach (ClsChip chip in kvp.Value)
                {
                    if (chip == null) continue;

                    foreach (partWork pw in chip.lstPartWork)
                    {
                        if (!pw.chip.use) endChannel++;
                        else if (pw.dataEnd && pw.waitCounter < 1) endChannel++;
                        else if (loopOffset != -1 && pw.dataEnd && pw.envIndex == 3) endChannel++;
                    }
                }
            }

            return endChannel;
        }

        public void DecAllPartWaitCounter(long waitCounter)
        {
            if (waitCounter != long.MaxValue)
            {

                // waitcounterを減らす

                foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in chips)
                {
                    foreach (ClsChip chip in kvp.Value)
                    {
                        if (chip == null) continue;

                        foreach (partWork pw in chip.lstPartWork)
                        {

                            if (pw.waitKeyOnCounter > 0) pw.waitKeyOnCounter -= waitCounter;

                            if (pw.waitCounter > 0) pw.waitCounter -= waitCounter;

                            if (pw.bendWaitCounter > 0) pw.bendWaitCounter -= waitCounter;

                            for (int lfo = 0; lfo < 4; lfo++)
                            {
                                if (!pw.lfo[lfo].sw) continue;
                                if (pw.lfo[lfo].waitCounter == -1) continue;

                                if (pw.lfo[lfo].waitCounter > 0)
                                {
                                    pw.lfo[lfo].waitCounter -= waitCounter;
                                    if (pw.lfo[lfo].waitCounter < 0) pw.lfo[lfo].waitCounter = 0;
                                }
                            }

                            if (pw.pcmWaitKeyOnCounter > 0)
                            {
                                pw.pcmWaitKeyOnCounter -= waitCounter;
                            }

                            if (pw.envelopeMode && pw.envIndex != -1)
                            {
                                pw.envCounter -= (int)waitCounter;
                            }
                        }
                    }
                }

                // wait発行

                lClock += waitCounter;
                dSample += (long)(info.samplesPerClock * waitCounter);

                if (jumpCommandCounter == 0 && !useSkipPlayCommand)
                {
                    bool flg = false;
                    if (ym2612 != null && ym2612[0] != null)
                    {
                        if (ym2612[0].lstPartWork[5].pcmWaitKeyOnCounter > 0) flg = true;
                    }

                    if (flg) OutWaitNSamplesWithPCMSending(ym2612[0].lstPartWork[5], waitCounter); 
                    else OutWaitNSamples((long)(info.samplesPerClock * waitCounter));
                }
            }
        }

        public long ComputeAllPartDistance()
        {
            long waitCounter = long.MaxValue;
            foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in chips)
            {
                foreach (ClsChip chip in kvp.Value)
                {
                    if (chip == null) continue;

                    for (int ch = 0; ch < chip.lstPartWork.Count; ch++)
                    {

                        partWork cpw = chip.lstPartWork[ch];

                        if (!cpw.chip.use) continue;

                        //note
                        if (cpw.waitKeyOnCounter > 0)
                        {
                            waitCounter = Math.Min(waitCounter, cpw.waitKeyOnCounter);
                        }
                        else if (cpw.waitCounter > 0)
                        {
                            waitCounter = Math.Min(waitCounter, cpw.waitCounter);
                        }

                        //bend
                        if (cpw.bendWaitCounter != -1)
                        {
                            waitCounter = Math.Min(waitCounter, cpw.bendWaitCounter);
                        }

                        //lfoとenvelopeは音長によるウエイトカウントが存在する場合のみ対象にする。(さもないと、曲のループ直前の効果を出せない)
                        if (waitCounter > 0)
                        {
                            //if (!cpw.dataEnd)
                            {
                                //lfo
                                for (int lfo = 0; lfo < 4; lfo++)
                                {
                                    if (!cpw.lfo[lfo].sw) continue;
                                    if (cpw.lfo[lfo].waitCounter == -1) continue;

                                    waitCounter = Math.Min(waitCounter, cpw.lfo[lfo].waitCounter);
                                }

                                //envelope
                                if (cpw.envelopeMode && cpw.envIndex != -1)
                                {
                                    waitCounter = Math.Min(waitCounter, cpw.envCounter);
                                }
                            }
                        }

                        //pcm
                        if (cpw.pcmWaitKeyOnCounter > 0)
                        {
                            waitCounter = Math.Min(waitCounter, cpw.pcmWaitKeyOnCounter);
                        }

                    }

                }
            }

            return waitCounter;
        }

        public void AnalyzeAllPartCommand()
        {
            foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in chips)
            {
                foreach (ClsChip chip in kvp.Value)
                {
                    if (chip == null) continue;

                    //未使用のパートの場合は処理を行わない
                    if (!chip.use) continue;

                    log.Write(string.Format("Chip [{0}]", chip.Name));

                    for (int i = 0; i < chip.lstPartWork.Count; i++)
                    {
                        partWork pw = chip.lstPartWork[
                            chip.ReversePartWork
                            ? (chip.lstPartWork.Count - 1 - i)
                            : i
                            ];
                        partWorkByteData(pw);
                    }
                    if (chip.SupportReversePartWork) chip.ReversePartWork = !chip.ReversePartWork;

                    log.Write("channelを跨ぐコマンド向け処理");
                    chip.MultiChannelCommand(null);
                }
            }
        }



        public void partWorkByteData(partWork pw)
        {

            //未使用のパートの場合は処理を行わない
            if (!pw.chip.use) return;
            if (pw.mmlData == null && !isRealTimeMode) return;

            log.Write("MD stream pcm sound off");
            if (pw.pcmWaitKeyOnCounter == 0)
                pw.pcmWaitKeyOnCounter = -1;

            log.Write("KeyOff");
            if (!isRealTimeMode) ProcKeyOff(pw);

            log.Write("Bend");
            ProcBend(pw);

            log.Write("Lfo");
            ProcLfo(pw);

            log.Write("Envelope");
            ProcEnvelope(pw);

            pw.chip.SetFNum(pw,null);
            pw.chip.SetVolume(pw,null);

            if (isRealTimeMode) return;

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

            if(doSkipStop && !useSkipPlayCommand)
            {
                pw.dataEnd = true;
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
                    mml.line.Lp.ch = pw.ch;
                    mml.line.Lp.chipIndex = pw.chip.ChipID;
                    mml.line.Lp.chipNumber = pw.chipNumber;
                    mml.line.Lp.chip = pw.chip.Name;
                    int c = mml.line.Txt.IndexOfAny(new char[] { ' ', '\t' });
                    //c += mml.line.Txt.Substring(c).Length - mml.line.Txt.Substring(c).TrimStart().Length;
                    mml.line.Lp.col = mml.column + c;//-1;
                    mml.line.Lp.part = pw.Type.ToString();
                    
                    //lineNumber = pw.getLineNumber();
                    Commander(pw, mml);
                }
            }

        }

        private void MakeHeader()
        {

            //Header
            if (info.Version <= 1.50f)
            {
                //length 0x40
                for (int i = 0; i < 0x40; i++) OutData((MML)null, null, Const.hDat[i]);
            }
            else
            {
                //length 0x100
                OutData((MML)null, Const.hDat);
            }

            //PCM Data block
            foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in chips)
            {
                foreach (ClsChip chip in kvp.Value)
                {
                    if (chip == null) continue;

                    chip.SetPCMDataBlock(null);
                }
            }

            //Set Initialize data
            foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in chips)
            {
                foreach (ClsChip chip in kvp.Value)
                {
                    if (chip == null) continue;
                    chip.InitChip();
                }
            }

        }

        private void MakeFooter()
        {

            byte[] v;

            //end of data
            byte[] cmd;
            if (info.format == enmFormat.ZGM)
            {
                if (ChipCommandSize == 2) cmd = new byte[] { 0x06, 0x00 };
                else cmd = new byte[] { 0x06 };
            }
            else cmd = new byte[] { 0x66 };
            OutData((MML)null, cmd);

            //Version
            int vs = (int)(info.Version * 100);
            int hv = vs / 100;
            vs = vs - hv * 100;
            dat[0x08] = new outDatum(enmMMLType.unknown, null, null, (byte)((vs % 10) + (vs / 10) * 0x10));
            dat[0x09] = new outDatum(enmMMLType.unknown, null, null, (byte)hv);
            dat[0x0a] = new outDatum(enmMMLType.unknown, null, null, 0);
            dat[0x0b] = new outDatum(enmMMLType.unknown, null, null, 0);

            //GD3 offset
            v = DivInt2ByteAry(dat.Count - 0x14 - (int)dummyCmdCounter);
            dat[0x14] = new outDatum(enmMMLType.unknown, null, null, v[0]);
            dat[0x15] = new outDatum(enmMMLType.unknown, null, null, v[1]);
            dat[0x16] = new outDatum(enmMMLType.unknown, null, null, v[2]);
            dat[0x17] = new outDatum(enmMMLType.unknown, null, null, v[3]);

            //Total # samples
            v = DivInt2ByteAry((int)dSample);
            dat[0x18] = new outDatum(enmMMLType.unknown, null, null, v[0]);
            dat[0x19] = new outDatum(enmMMLType.unknown, null, null, v[1]);
            dat[0x1a] = new outDatum(enmMMLType.unknown, null, null, v[2]);
            dat[0x1b] = new outDatum(enmMMLType.unknown, null, null, v[3]);

            if (loopOffset != -1 && (!doSkip && !useJumpCommand)) //!(doSkip && doSkipStop) スキップ時又はJコマンド使用時は　Lコマンドループしない
            {
                //Loop offset
                v = DivInt2ByteAry((int)(loopOffset - 0x1c));
                dat[0x1c] = new outDatum(enmMMLType.unknown, null, null, v[0]);
                dat[0x1d] = new outDatum(enmMMLType.unknown, null, null, v[1]);
                dat[0x1e] = new outDatum(enmMMLType.unknown, null, null, v[2]);
                dat[0x1f] = new outDatum(enmMMLType.unknown, null, null, v[3]);

                //Loop # samples
                v = DivInt2ByteAry((int)(dSample - loopSamples));
                dat[0x20] = new outDatum(enmMMLType.unknown, null, null, v[0]);
                dat[0x21] = new outDatum(enmMMLType.unknown, null, null, v[1]);
                dat[0x22] = new outDatum(enmMMLType.unknown, null, null, v[2]);
                dat[0x23] = new outDatum(enmMMLType.unknown, null, null, v[3]);
            }

            int p = dat.Count + 12;

            GD3maker gd3 = new GD3maker();
            gd3.make(dat, info, lyric);

            //EoF offset
            v = DivInt2ByteAry(dat.Count - 0x4 - (int)dummyCmdCounter);
            dat[0x4] = new outDatum(enmMMLType.unknown, null, null, v[0]);
            dat[0x5] = new outDatum(enmMMLType.unknown, null, null, v[1]);
            dat[0x6] = new outDatum(enmMMLType.unknown, null, null, v[2]);
            dat[0x7] = new outDatum(enmMMLType.unknown, null, null, v[3]);

            int q = dat.Count - p;

            //GD3 Length
            v = DivInt2ByteAry(q);
            dat[p - 4] = new outDatum(enmMMLType.unknown, null, null, v[0]);
            dat[p - 3] = new outDatum(enmMMLType.unknown, null, null, v[1]);
            dat[p - 2] = new outDatum(enmMMLType.unknown, null, null, v[2]);
            dat[p - 1] = new outDatum(enmMMLType.unknown, null, null, v[3]);

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
            long useYM2413 = 0;
            long useK051649 = 0;
            long useQSound = 0;
            long useK053260 = 0;
            long useYM2151_S = 0;
            long useYM2203_S = 0;
            long useYM2608_S = 0;
            long useYM2610B_S = 0;
            long useYM2612_S = 0;
            long useSN76489_S = 0;
            long useRf5c164_S = 0;
            long useSegaPcm_S = 0;
            long useHuC6280_S = 0;
            long useC140_S = 0;
            long useAY8910_S = 0;
            long useYM2413_S = 0;
            long useK051649_S = 0;
            long useK053260_S = 0;

            for (int i = 0; i < 2; i++)
            {
                if (ym2151!= null && ym2151.Length > i && ym2151[i] != null)
                    foreach (partWork pw in ym2151[i].lstPartWork)
                    { useYM2151 += pw.clockCounter; if (ym2151[i].ChipID == 1) useYM2151_S += pw.clockCounter; }

                if (ym2203 != null && ym2203.Length > i && ym2203[i] != null)
                    foreach (partWork pw in ym2203[i].lstPartWork)
                    { useYM2203 += pw.clockCounter; if (ym2203[i].ChipID == 1) useYM2203_S += pw.clockCounter; }
                    

                if (ym2608 != null && ym2608.Length > i && ym2608[i] != null)
                    foreach (partWork pw in ym2608[i].lstPartWork)
                    { useYM2608 += pw.clockCounter; if (ym2608[i].ChipID == 1) useYM2608_S += pw.clockCounter; }

                if (ym2610b != null && ym2610b.Length > i && ym2610b[i] != null)
                    foreach (partWork pw in ym2610b[i].lstPartWork)
                    { useYM2610B += pw.clockCounter; if (ym2610b[i].ChipID == 1) useYM2610B_S += pw.clockCounter; }

                if (ym2612 != null && ym2612.Length>i && ym2612[i] != null)
                    foreach (partWork pw in ym2612[i].lstPartWork)
                    { useYM2612 += pw.clockCounter; if (ym2612[i].ChipID == 1) useYM2612_S += pw.clockCounter; }

                if (sn76489 != null && sn76489.Length > i && sn76489[i] != null)
                    foreach (partWork pw in sn76489[i].lstPartWork)
                    { useSN76489 += pw.clockCounter; if (sn76489[i].ChipID == 1) useSN76489_S += pw.clockCounter; }

                if (rf5c164 != null && rf5c164.Length > i && rf5c164[i] != null)
                    foreach (partWork pw in rf5c164[i].lstPartWork)
                    { useRf5c164 += pw.clockCounter; if (rf5c164[i].ChipID == 1) useRf5c164_S += pw.clockCounter; }

                if (segapcm != null && segapcm.Length > i && segapcm[i] != null)
                    foreach (partWork pw in segapcm[i].lstPartWork)
                    { useSegaPcm += pw.clockCounter; if (segapcm[i].ChipID == 1) useSegaPcm_S += pw.clockCounter; }

                if (huc6280 != null && huc6280.Length > i && huc6280[i] != null)
                    foreach (partWork pw in huc6280[i].lstPartWork)
                    { useHuC6280 += pw.clockCounter; if (huc6280[i].ChipID == 1) useHuC6280_S += pw.clockCounter; }

                if (c140 != null && c140.Length > i && c140[i] != null)
                    foreach (partWork pw in c140[i].lstPartWork)
                    { useC140 += pw.clockCounter; if (c140[i].ChipID == 1) useC140_S += pw.clockCounter; }

                if (ay8910 != null && ay8910.Length > i && ay8910[i] != null)
                    foreach (partWork pw in ay8910[i].lstPartWork)
                    { useAY8910 += pw.clockCounter; if (ay8910[i].ChipID == 1) useAY8910_S += pw.clockCounter; }

                if (ym2413 != null && ym2413.Length > i && ym2413[i] != null)
                    foreach (partWork pw in ym2413[i].lstPartWork)
                    { useYM2413 += pw.clockCounter; if (ym2413[i].ChipID == 1) useYM2413_S += pw.clockCounter; }

                if (k051649 != null && k051649.Length > i && k051649[i] != null)
                    foreach (partWork pw in k051649[i].lstPartWork)
                    { useK051649 += pw.clockCounter; if (k051649[i].ChipID == 1) useK051649_S += pw.clockCounter; }

                if (qsound != null && qsound.Length > i && qsound[i] != null)
                    foreach (partWork pw in qsound[i].lstPartWork)
                    { useQSound += pw.clockCounter; }

                if (k053260 != null && k053260.Length > i && k053260[i] != null)
                    foreach (partWork pw in k053260[i].lstPartWork)
                    { useK053260 += pw.clockCounter; if (k053260[i].ChipID == 1) useK053260_S += pw.clockCounter; }
            }

            if (info.Version >= 1.00f && useSN76489 != 0)
            {
                SN76489 s = sn76489[0] != null ? sn76489[0] : sn76489[1];
                Common.SetLE32(dat, 0x0c, (uint)s.Frequency | (uint)(useSN76489_S == 0 ? 0 : 0x40000000));
            }
            if (info.Version >= 1.10f && useYM2612 != 0)
            {
                YM2612 y = ym2612[0] != null ? ym2612[0] : ym2612[1];
                Common.SetLE32(dat, 0x2c, (uint)y.Frequency | (uint)(useYM2612_S == 0 ? 0 : 0x40000000));
            }
            if (info.Version >= 1.10f && useYM2151 != 0)
            {
                YM2151 y = ym2151[0] != null ? ym2151[0] : ym2151[1];
                Common.SetLE32(dat, 0x30, (uint)y.Frequency | (uint)(useYM2151_S == 0 ? 0 : 0x40000000));
            }
            if (info.Version >= 1.51f && useSegaPcm != 0)
            {
                segaPcm s = segapcm[0] != null ? segapcm[0] : segapcm[1];
                Common.SetLE32(dat, 0x38, (uint)s.Frequency | (uint)(useSegaPcm_S == 0 ? 0 : 0x40000000));

                dat[0x3c] = new outDatum(enmMMLType.unknown, null, null, 0x0d);
                dat[0x3d] = new outDatum(enmMMLType.unknown, null, null, 0x00);
                dat[0x3e] = new outDatum(enmMMLType.unknown, null, null, 0xf8);
                dat[0x3f] = new outDatum(enmMMLType.unknown, null, null, 0x00);

            }
            if (info.Version >= 1.51f && useYM2203 != 0)
            {
                YM2203 y = ym2203[0] != null ? ym2203[0] : ym2203[1];
                Common.SetLE32(dat, 0x44, (uint)y.Frequency | (uint)(useYM2203_S == 0 ? 0 : 0x40000000));
            }
            if (info.Version >= 1.51f && useYM2608 != 0)
            {
                YM2608 y = ym2608[0] != null ? ym2608[0] : ym2608[1];
                Common.SetLE32(dat, 0x48, (uint)y.Frequency | (uint)(useYM2608_S == 0 ? 0 : 0x40000000));
            }

            //YM2609 はVGMに未対応

            if (info.Version >= 1.51f && useYM2610B != 0)
            {
                YM2610B y = ym2610b[0] != null ? ym2610b[0] : ym2610b[1];
                Common.SetLE32(dat, 0x4c, (uint)y.Frequency | (uint)(useYM2610B_S == 0 ? 0x8000_0000 : 0xc000_0000));// bit31: YM2610Bモード
            }
            if (info.Version >= 1.51f && useRf5c164 != 0)
            {
                RF5C164 r = rf5c164[0] != null ? rf5c164[0] : rf5c164[1];
                Common.SetLE32(dat, 0x6c, (uint)r.Frequency | (uint)(useRf5c164_S == 0 ? 0 : 0x40000000));
            }
            if (info.Version >= 1.61f && useHuC6280 != 0)
            {
                HuC6280 h = huc6280[0] != null ? huc6280[0] : huc6280[1];
                Common.SetLE32(dat, 0xa4, (uint)h.Frequency | (uint)(useHuC6280_S == 0 ? 0 : 0x40000000));
            }
            if (info.Version >= 1.61f && useC140 != 0)
            {
                C140 c = c140[0] != null ? c140[0] : c140[1];
                Common.SetLE32(dat, 0xa8, (uint)c.Frequency | (uint)(useC140_S == 0 ? 0 : 0x40000000));
                if (c != null && !c.isSystem2)
                    dat[0x96] = new outDatum(enmMMLType.unknown, null, null, (byte)1);
                else
                    dat[0x96] = new outDatum(enmMMLType.unknown, null, null, (byte)0);
            }
            if (info.Version >= 1.51f && useAY8910 != 0)
            {
                AY8910 a = ay8910[0] != null ? ay8910[0] : ay8910[1];
                Common.SetLE32(dat, 0x74, (uint)a.Frequency | (uint)(useAY8910_S == 0 ? 0 : 0x40000000));
                dat[0x78] = new outDatum(enmMMLType.unknown, null, null, 0);
                dat[0x79] = new outDatum(enmMMLType.unknown, null, null, 0);
                dat[0x7a] = new outDatum(enmMMLType.unknown, null, null, 0);
                dat[0x7b] = new outDatum(enmMMLType.unknown, null, null, 0);
            }
            if (info.Version >= 1.00f && useYM2413 != 0)
            {
                YM2413 y = ym2413[0] != null ? ym2413[0] : ym2413[1];
                Common.SetLE32(dat, 0x10, (uint)y.Frequency | (uint)(useYM2413_S == 0 ? 0 : 0x40000000));
            }
            if (info.Version >= 1.61f && useK051649 != 0)
            {
                K051649 k = k051649[0] != null ? k051649[0] : k051649[1];
                Common.SetLE32(dat, 0x9c, (uint)k.Frequency | (uint)(useK051649_S == 0 ? 0 : 0x40000000));
            }
            if (info.Version >= 1.61f && useQSound != 0)
            {
                Common.SetLE32(dat, 0xb4, (uint)qsound[0].Frequency);
            }
            if (info.Version >= 1.61f && useK053260 != 0)
            {
                Common.SetLE32(dat, 0xac, (uint)k053260[0].Frequency);
            }

            //if (info.Version == 1.51f)
            //{
            //    dat[0x08] = new outDatum(enmMMLType.unknown, null, null, 0x51);
            //    dat[0x09] = new outDatum(enmMMLType.unknown, null, null, 0x01);
            //}
            //else if (info.Version == 1.60f)
            //{
            //    dat[0x08] = new outDatum(enmMMLType.unknown, null, null, 0x60);
            //    dat[0x09] = new outDatum(enmMMLType.unknown, null, null, 0x01);
            //}
            //else
            //{
            //    dat[0x08] = new outDatum(enmMMLType.unknown, null, null, 0x61);
            //    dat[0x09] = new outDatum(enmMMLType.unknown, null, null, 0x01);
            //}

        }

        public outDatum[] Xgm_getByteData(Dictionary<string, List<MML>> mmlData)
        {
            if (ym2612x == null || ym2612x[0] == null) return null;

            if (doSkip)
            {
                useSkipPlayCommand = true;
            }

            dat = new List<outDatum>();
            xdat = new List<outDatum>();

            log.Write("ヘッダー情報作成(XGM)");
            Xgm_makeHeader();

            int endChannel = 0;
            int totalChannel = 0;
            foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in chips)
            {
                foreach (ClsChip chip in kvp.Value)
                {
                    if (chip == null) continue;
                    if (chip.ShortName != "CON" && chip.ShortName != "OPN2X" && chip.ShortName != "DCSG")
                    {
                        foreach (partWork pw in chip.lstPartWork) pw.chip.use = false;
                    }
                    totalChannel += chip.ChMax;
                }
            }

            log.Write("MML解析開始(XGM)");
            long waitCounter;
            do
            {
                //KeyOnリストをクリア
                xgmKeyOnData = new List<outDatum>();

                foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in chips)
                {
                    foreach (ClsChip chip in kvp.Value)
                    {
                        if (chip == null) continue;
                        log.Write(string.Format("Chip [{0}]", chip.Name));

                        //未使用のchipの場合は処理を行わない
                        if (!chip.use) continue;

                        //chip毎の処理
                        Xgm_procChip(chip);
                    }
                }

                log.Write("全パートのうち次のコマンドまで一番近い値を求める");
                waitCounter = Xgm_procCheckMinimumWaitCounter();

                log.Write("KeyOn情報をかき出し");
                foreach (outDatum dat in xgmKeyOnData) OutData(dat, null, 0x52, 0x28, dat.val);

                //log.Write("全パートのwaitcounterを減らす");
                //if (waitCounter != long.MaxValue)
                //{
                //    //wait処理
                //    Xgm_procWait(waitCounter);
                //}

                log.Write("終了パートのカウント");
                endChannel = 0;
                foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in chips)
                {
                    foreach (ClsChip chip in kvp.Value)
                    {
                        if (chip == null) continue;

                        foreach (partWork pw in chip.lstPartWork)
                        {
                            if (!pw.chip.use) 
                                endChannel++;
                            else if (pw.dataEnd) {
                                if (pw.waitCounter < 1) 
                                    endChannel++;
                                else if (loopOffset != -1)
                                {
                                    if ((pw.envIndex == 3 || pw.envIndex == -1))
                                        endChannel++;
                                    //else if (loopOffset != -1 && pw.dataEnd) endChannel++;
                                }
                            }
                        }
                    }
                }

                log.Write("全パートのwaitcounterを減らす");
                if (waitCounter != long.MaxValue && endChannel < totalChannel)
                {
                    //wait処理
                    Xgm_procWait(waitCounter);
                }

            } while (endChannel < totalChannel);//全てのチャンネルが終了していない場合はループする
            //if (loopClock != -1)
            //{
            //    if (waitCounter > 0)
            //    {
            //        lClock -= waitCounter;
            //        dSample -= (long)(info.samplesPerClock * waitCounter);
            //    }
            //}
            if (loopClock == -1)
            {
                AllKeyOffEnv();
            }

            log.Write("VGMデータをXGMへコンバート");
            dat = ConvertVGMtoXGM(dat);

            log.Write("フッター情報の作成");
            Xgm_makeFooter();

            return dat.ToArray();
        }

        private void Xgm_makeHeader()
        {
            if (ym2612x == null || ym2612x[0] == null) return;

            //Header
            foreach (byte b in Const.xhDat)
            {
                xdat.Add(new outDatum(enmMMLType.unknown, null, null, b));
            }

            //FM音源を初期化

            ym2612x[0].OutOPNSetHardLfo(null,ym2612x[0].lstPartWork[0], false, 0);
            ym2612x[0].OutOPNSetCh3SpecialMode(null,ym2612x[0].lstPartWork[0], false);
            ym2612x[0].OutSetCh6PCMMode(null,ym2612x[0].lstPartWork[0], false);
            ym2612x[0].OutFmAllKeyOff();

            foreach (partWork pw in ym2612x[0].lstPartWork)
            {
                if (pw.ch == 0)
                {
                    pw.hardLfoSw = false;
                    pw.hardLfoNum = 0;
                    ym2612x[0].OutOPNSetHardLfo(null,pw, pw.hardLfoSw, pw.hardLfoNum);
                }

                if (pw.ch < 6)
                {
                    pw.pan.val = 3;
                    pw.ams = 0;
                    pw.fms = 0;
                    if (!pw.dataEnd) ym2612x[0].OutOPNSetPanAMSPMS(null,pw, 3, 0, 0);
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
                //if (size > (uint)p.xgmMaxSampleCount + 1)
                //{
                    //size = (uint)p.xgmMaxSampleCount + 1;
                    //size = (uint)((size & 0xffff00) + (size % 0x100 != 0 ? 0x100 : 0x0));
                //}
                p.size = size;

                xdat[n + 0] = new outDatum(enmMMLType.unknown, null, null, (byte)((stAdr / 256) & 0xff));
                xdat[n + 1] = new outDatum(enmMMLType.unknown, null, null, (byte)(((stAdr / 256) & 0xff00) >> 8));
                xdat[n + 2] = new outDatum(enmMMLType.unknown, null, null, (byte)((size / 256) & 0xff));
                xdat[n + 3] = new outDatum(enmMMLType.unknown, null, null, (byte)(((size / 256) & 0xff00) >> 8));

                ptr += size;
                n += 4;
            }

            //$0100               Sample data bloc size / 256
            if (ym2612x[0].pcmDataEasy != null)
            {
                xdat[0x100] = new outDatum(enmMMLType.unknown, null, null, (byte)((ptr / 256) & 0xff));
                xdat[0x101] = new outDatum(enmMMLType.unknown, null, null, (byte)(((ptr / 256) & 0xff00) >> 8));
            }
            else
            {
                xdat[0x100] = new outDatum(enmMMLType.unknown, null, null, 0);
                xdat[0x101] = new outDatum(enmMMLType.unknown, null, null, 0);
            }

            //$0103 bit #0: NTSC / PAL information
            xdat[0x103] = new outDatum(enmMMLType.unknown, null, null, (byte)(xdat[0x103].val | (byte)(info.xgmSamplesPerSecond == 50 ? 1 : 0)));

            //$0104               Sample data block
            if (ym2612x[0].pcmDataEasy != null)
            {
                foreach (clsPcm p in instPCM.Values)
                {
                    if (p.chip != enmChipType.YM2612X) continue;

                    for (uint cnt = 0; cnt < p.size; cnt++)
                    {
                        xdat.Add(new outDatum(enmMMLType.unknown, null, null, ym2612x[0].pcmDataEasy[p.stAdr + cnt]));
                    }

                }

            }

            dummyCmdLoopOffsetAddress += xdat.Count+4;

            if (dat != null)
            {
                //$0104 + SLEN        Music data bloc size.
                xdat.Add(new outDatum(enmMMLType.unknown, null, null, (byte)((dat.Count & 0xff) >> 0)));
                xdat.Add(new outDatum(enmMMLType.unknown, null, null, (byte)((dat.Count & 0xff00) >> 8)));
                xdat.Add(new outDatum(enmMMLType.unknown, null, null, (byte)((dat.Count & 0xff0000) >> 16)));
                xdat.Add(new outDatum(enmMMLType.unknown, null, null, (byte)((dat.Count & 0xff000000) >> 24)));

                //$0108 + SLEN        Music data bloc
                foreach (outDatum b in dat)
                {
                    //Console.WriteLine("{0:x2}", b.val);
                    xdat.Add(b);
                }
            }
            else
            {
                xdat.Add(new outDatum(enmMMLType.unknown, null, null, 0));
                xdat.Add(new outDatum(enmMMLType.unknown, null, null, 0));
                xdat.Add(new outDatum(enmMMLType.unknown, null, null, 0));
                xdat.Add(new outDatum(enmMMLType.unknown, null, null, 0));
            }

            //$0108 + SLEN + MLEN GD3 tags
            GD3maker gd3 = new GD3maker();
            gd3.make(xdat, info, lyric);

            dat = xdat;
        }

        private void Xgm_procChip(ClsChip chip)
        {
            if (chip == null) throw new ArgumentNullException();

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

                pw.chip.SetFNum(pw,null);
                pw.chip.SetVolume(pw,null);

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
                        mml.line.Lp.ch = pw.ch;
                        mml.line.Lp.chipIndex = pw.chip.ChipID;
                        mml.line.Lp.chipNumber = pw.chipNumber;
                        mml.line.Lp.chip = pw.chip.Name;
                        int c = mml.line.Txt.IndexOfAny(new char[] { ' ', '\t' });
                        //c += mml.line.Txt.Substring(c).Length - mml.line.Txt.Substring(c).TrimStart().Length;
                        mml.line.Lp.col = mml.column + c;//-1;
                        mml.line.Lp.part = pw.Type.ToString();

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
                    if (chip == null) continue;
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

                        //if (!pw.dataEnd) //ここを有効にするとデータ読み取り終了後即エンベロープ処理をしなくなってしまう
                        {
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
            }

            return cnt;
        }

        private void Xgm_procWait(long cnt)
        {
            foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in chips)
            {
                foreach (ClsChip chip in kvp.Value)
                {
                    if (chip == null) continue;
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

            if (jumpCommandCounter == 0 && !useSkipPlayCommand)
            {
                // wait発行
                lClock += cnt;
                dSample += (long)(info.samplesPerClock * cnt);
                //Console.WriteLine("pw.ch{0} lclock{1}", ym2612x[0].lstPartWork[0].clockCounter, lClock);

                sampleB += info.samplesPerClock * cnt;
                //Console.WriteLine("samplesPerClock{0}", info.samplesPerClock);
                OutWaitNSamples((long)(sampleB));
                sampleB -= (long)sampleB;
            }
        }

        private List<outDatum> ConvertVGMtoXGM(List<outDatum> src)
        {
            if (src == null || src.Count < 1) return null;

            List<outDatum> des = new List<outDatum>();
            loopOffset = -1;

            int[][] opn2reg = new int[2][] { new int[0x100], new int[0x100] };
            for (int i = 0; i < 512; i++) opn2reg[i / 0x100][i % 0x100] = -1;
            outDatum[] psgreg = new outDatum[16];
            int psgch = -1;
            int psgtp = -1;
            //for (int i = 0; i < 16; i++) psgreg[i] = -1;
            int framePtr = 0;
            int frameCnt = 0;
            int frameDummyCounter = 0;
            outDatum od;
            dummyCmdCounter = 0;

            for (int ptr = 0; ptr < src.Count; ptr++)
            {

                outDatum cmd = src[ptr];
                int p;
                int c;

                switch (cmd.val)
                {
                    case 0x61: //Wait

                        if (psgtp != -1)
                        {
                            p = des.Count;
                            c = 0;
                            od = new outDatum(cmd.type, cmd.args, cmd.linePos, 0x10);
                            des.Add(od);
                            for (int j = 0; j < 16; j++)
                            {
                                if (psgreg[j] == null) continue;
                                int latch = (j & 1) == 0 ? 0x80 : 0;
                                int ch = (j & 0x0c) << 3;
                                int tp = (j & 2) << 3;
                                od = new outDatum(psgreg[j].type, psgreg[j].args, psgreg[j].linePos, (byte)(latch | (latch != 0 ? (ch | tp) : 0) | psgreg[j].val));
                                des.Add(od);
                                c++;
                            }
                            c--;
                            des[p].val |= (byte)c;

                            psgch = -1;
                            psgtp = -1;
                            for (int i = 0; i < 16; i++) psgreg[i] = null;
                        }

                        if (des.Count - frameDummyCounter - framePtr > 256)
                        {
                            msgBox.setWrnMsg(string.Format(msg.get("E01015"), frameCnt, des.Count - frameDummyCounter - framePtr), new LinePos("-"));
                        }
                        framePtr = des.Count;
                        frameDummyCounter = 0;

                        int cnt = src[ptr + 1].val + src[ptr + 2].val * 0x100;
                        for (int j = 0; j < cnt; j++)
                        {
                            //wait
                            od = new outDatum(cmd.type, cmd.args, cmd.linePos, 0x00);
                            des.Add(od);
                            frameCnt++;
                        }
                        ptr += 2;
                        break;
                    case 0x50: //DCSG
                        do
                        {
                            bool latch = (src[ptr + 1].val & 0x80) != 0;
                            int ch = (src[ptr + 1].val & 0x60) >> 5;
                            int tp = (src[ptr + 1].val & 0x10) >> 3;
                            int d1 = (src[ptr + 1].val & 0xf);
                            int d2 = (src[ptr + 1].val & 0x3f);
                            if (latch)
                            {
                                psgch = ch;
                                psgtp = tp;
                                psgreg[ch * 4 + 0 + tp] = new outDatum(src[ptr + 1].type, src[ptr + 1].args, src[ptr + 1].linePos, (byte)d1);
                            }
                            else
                            {
                                if (psgch != -1)
                                {
                                    psgreg[psgch * 4 + 1 + psgtp] = new outDatum(src[ptr + 1].type, src[ptr + 1].args, src[ptr + 1].linePos, (byte)d2);
                                }
                                psgch = -1;
                            }
                            ptr += 2;
                        } while (ptr < src.Count - 1 && src[ptr].val == 0x50);
                        ptr--;
                        break;
                    case 0x52: //YM2612 Port0
                        //送信しようとしているデータがすでにチップに送ったものと同じかどうかチェックする。
                        //同じ場合は送信しない(データサイズ的には圧縮されることになる)
                        //また、キーオン(0x28)の場合は別のコマンドになる
                        if (opn2reg[0][src[ptr + 1].val] != src[ptr + 2].val || src[ptr + 1].val == 0x28)
                        {
                            bool isKeyOn = src[ptr + 1].val == 0x28;
                            if (!isKeyOn)
                            {
                                //キーオンではない場合は、キーオン以外のデータが続くものとして処理する
                                p = des.Count;
                                c = 0;

                                //0x20(OPN2のデータが続くことを示すデータ)を書く
                                od = new outDatum(cmd.type, cmd.args, cmd.linePos, 0x20);
                                des.Add(od);

                                do
                                {
                                    //送信しようとしているデータがすでにチップに送ったものと同じかどうかチェックする。
                                    //同じ場合は送信しない(データサイズ的には圧縮されることになる)
                                    //(ループに入った初回は、違うことが保証されているがそれ以降は調べる必要がある)
                                    if (opn2reg[0][src[ptr + 1].val] != src[ptr + 2].val)
                                    {
                                        //F-numの場合は圧縮対象外
                                        if (src[ptr + 1].val < 0xa0 || src[ptr + 1].val >= 0xb0) opn2reg[0][src[ptr + 1].val] = src[ptr + 2].val;

                                        //OPN2アドレスの書き込み
                                        od = new outDatum(src[ptr + 1].type, src[ptr + 1].args, src[ptr + 1].linePos, src[ptr + 1].val);
                                        des.Add(od);
                                        //Console.WriteLine("{0:x2}", od.val);
                                        //OPN2値の書き込み
                                        od = new outDatum(src[ptr + 2].type, src[ptr + 2].args, src[ptr + 2].linePos, src[ptr + 2].val);
                                        des.Add(od);
                                        //Console.WriteLine("    {0:x2}", od.val);
                                        c++;
                                    }

                                    //次の命令へ移るため3足す(vgmではOPN2の命令長が3byte固定のため)
                                    ptr += 3;

                                } while (c < 16 //圧縮は最大16個
                                    && ptr < src.Count - 1 //ポインターがデータ内であることをチェック
                                    && src[ptr].val == 0x52 //次の命令がOPN2の命令である間はループ
                                    && src[ptr + 1].val != 0x28 //しかしキーオン(0x28)の場合はループから抜ける
                                    );
                                c--;//cは0x0～0xfで1～16個を表すため-1する
                                ptr--;//ptrはfor文で必ず+1されるのでその分引いておく
                                des[p].val |= (byte)c;//命令に圧縮できた個数を論理和
                            }
                            else
                            {
                                //キーオンの場合は、そのデータが続くものとして処理する
                                p = des.Count;
                                c = 0;

                                //0x40(OPN2のキーオンのデータが続くことを示すデータ)を書く
                                od = new outDatum(cmd.type, cmd.args, cmd.linePos, 0x40);
                                des.Add(od);
                                do
                                {
                                    //des.Add(src[ptr + 1]);
                                    od = new outDatum(src[ptr + 2].type, src[ptr + 2].args, src[ptr + 2].linePos, src[ptr + 2].val);
                                    des.Add(od);
                                    c++;
                                    ptr += 3;
                                } while (c < 16 
                                    && ptr < src.Count - 1 
                                    && src[ptr].val == 0x52 
                                    && src[ptr + 1].val == 0x28
                                    );
                                c--;
                                ptr--;
                                des[p].val |= (byte)c;
                            }
                        }
                        else
                        {
                            //次の命令へ
                            ptr += 2;
                        }
                        break;
                    case 0x53: //YM2612 Port1
                        if (opn2reg[1][src[ptr + 1].val] != src[ptr + 2].val)
                        {

                            p = des.Count;
                            c = 0;
                            od = new outDatum(cmd.type, cmd.args, cmd.linePos, 0x30);
                            des.Add(od);
                            do
                            {
                                if (opn2reg[1][src[ptr + 1].val] != src[ptr + 2].val)
                                {
                                    //F-numの場合は圧縮対象外
                                    if (src[ptr + 1].val < 0xa0 || src[ptr + 1].val >= 0xb0) opn2reg[1][src[ptr + 1].val] = src[ptr + 2].val;
                                    od = new outDatum(src[ptr + 1].type, src[ptr + 1].args, src[ptr + 1].linePos, src[ptr + 1].val);
                                    des.Add(od);
                                    od = new outDatum(src[ptr + 2].type, src[ptr + 2].args, src[ptr + 2].linePos, src[ptr + 2].val);
                                    des.Add(od);
                                    c++;
                                }
                                ptr += 3;
                            } while (c < 16 && ptr < src.Count - 1 && src[ptr].val == 0x53);
                            c--;
                            ptr--;
                            des[p].val |= (byte)c;
                        }
                        else
                        {
                            ptr += 2;
                        }
                        break;
                    case 0x54: //PCM KeyON (YM2151)
                        //mml2vgmではxgmを生成するとき0x54を4重PCMのコマンドに割り当てている。(本体はvgmではOPMのコマンド)
                        od = new outDatum(src[ptr + 1].type, src[ptr + 1].args, src[ptr + 1].linePos, src[ptr + 1].val);
                        des.Add(od);
                        od = new outDatum(src[ptr + 2].type, src[ptr + 2].args, src[ptr + 2].linePos, src[ptr + 2].val);
                        des.Add(od);
                        ptr += 2;
                        break;
                    case 0x7e: //LOOP Point
                        loopOffset = des.Count - dummyCmdCounter;//ダミーコマンドを抜いた場合のオフセット値
                        dummyCmdLoopOffset = des.Count;//ダミーコマンド込みのオフセット値
                        dummyCmdLoopOffsetAddress = ptr;//ソースのループコマンドが存在するアドレス

                        //ループ後のレジスタに反応するために現在の値を全て初期化する
                        //さもなくばループ後に音色が変わらないなどの現象が発生する
                        for (int i = 0; i < 512; i++) opn2reg[i / 0x100][i % 0x100] = -1;

                        break;
                    case 0x2f: 
                        //TODO: Dummy Command
                        //dummyコマンドの除去はmml2vgm.cs:OutXgmFileで行う。
                        if (cmd.val == 0x2f //dummyChipコマンド　(第2引数：chipID 第３引数:chipNumber)
                            && Common.CheckDummyCommand(cmd.type))//ここで指定できるmmlコマンドは元々はChipに送信することのないコマンドのみ(さもないと、通常のコマンドのデータと見分けがつかなくなる可能性がある)
                        {
                            src[ptr].val = 0x60;//XGM向けダミーコマンド
                            des.Add(src[ptr]);
                            des.Add(src[ptr + 1]);
                            des.Add(src[ptr + 2]);
                            ptr += 2;
                            dummyCmdCounter += 3;
                            frameDummyCounter += 3;
                        }
                        else
                        {
                            ;
                        }

                        break;
                    default:
                        msgBox.setErrMsg(string.Format("Unknown command[{0:X}]", cmd.val), new LinePos("-"));
                        return null;
                }
            }

            if (loopOffset == -1 || loopOffset == des.Count)
            {
                od = new outDatum(enmMMLType.unknown, null, null, 0x7f);
                des.Add(od);
            }
            else
            {
                dummyCmdLoopOffsetAddress = des.Count;
                od = new outDatum(enmMMLType.unknown, null, null, 0x7e);
                des.Add(od);
                
                //od = new outDatum(enmMMLType.unknown, null, null, (byte)loopOffset);
                //des.Add(od);
                //od = new outDatum(enmMMLType.unknown, null, null, (byte)(loopOffset >> 8));
                //des.Add(od);
                //od = new outDatum(enmMMLType.unknown, null, null, (byte)(loopOffset >> 16));
                //des.Add(od);
                od = new outDatum(enmMMLType.unknown, null, null, (byte)dummyCmdLoopOffset);
                des.Add(od);
                od = new outDatum(enmMMLType.unknown, null, null, (byte)(dummyCmdLoopOffset >> 8));
                des.Add(od);
                od = new outDatum(enmMMLType.unknown, null, null, (byte)(dummyCmdLoopOffset >> 16));
                des.Add(od);

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
                        pw.chip.SetKeyOff(pw,null);
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
                    //midi向け
                    pw.tieBend = pw.bendFnum;
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
                    //if ((cpw.chip is YM2612) || (cpw.chip is YM2612X))
                    //{
                        //cpw.fms = pl.param[2];
                        //cpw.ams = pl.param[3];
                        //((ClsOPN)cpw.chip).OutOPNSetPanAMSPMS(null, cpw, (int)cpw.pan.val, cpw.ams, cpw.fms);
                        //cpw.chip.lstPartWork[0].hardLfoSw = true;
                        //cpw.chip.lstPartWork[0].hardLfoNum = pl.param[1];
                        //((ClsOPN)cpw.chip).OutOPNSetHardLfo(null, cpw, cpw.chip.lstPartWork[0].hardLfoSw, cpw.chip.lstPartWork[0].hardLfoNum);
                        //pl.waitCounter = -1;
                    //}
                    pl.waitCounter = -1;
                    continue;
                }

                switch (pl.param[4])
                {
                    case 0: //三角
                        pl.value += Math.Abs(pl.param[2]) * pl.direction;
                        pl.waitCounter = pl.param[1];
                        if ((pl.direction > 0 && pl.value >= pl.depth) || (pl.direction < 0 && pl.value <= -pl.depth))
                        {
                            pl.value = pl.depth * pl.direction;
                            pl.direction = -pl.direction;
                            procLfo_updateDepth(pl);
                        }
                        break;
                    case 1: //のこぎり
                        pl.value += Math.Abs(pl.param[2]) * pl.direction;
                        pl.waitCounter = pl.param[1];
                        if ((pl.direction > 0 && pl.value >= pl.depth) || (pl.direction < 0 && pl.value <= -pl.depth))
                        {
                            pl.value = -pl.depth * pl.direction;
                            procLfo_updateDepth(pl);
                        }
                        break;
                    case 2: //矩形
                        if (pl.direction < 0) pl.value = pl.depth;
                        else pl.value = pl.depthV2;
                        pl.waitCounter = pl.param[1];
                        pl.direction = -pl.direction;
                        if (pl.param[7] != 0)
                        {
                            pl.depthWaitCounter--;
                            if (pl.depthWaitCounter == 0)
                            {
                                pl.depthWaitCounter = pl.param[7];
                                pl.depth += (pl.param[2] >= pl.param[3]) ? pl.param[8] : (-pl.param[8]);
                                pl.depth = Common.CheckRange(pl.depth, 0, 32767);
                                pl.depthV2 += (pl.param[3] >= pl.param[2]) ? pl.param[8] : (-pl.param[8]);
                                pl.depthV2 = Common.CheckRange(pl.depthV2, 0, 32767);
                            }
                        }
                        break;
                    case 3: //ワンショット
                        pl.value += Math.Abs(pl.param[2]) * pl.direction;
                        pl.waitCounter = pl.param[1];
                        if ((pl.direction > 0 && pl.value >= pl.depth) || (pl.direction < 0 && pl.value <= -pl.depth))
                        {
                            pl.waitCounter = -1;
                        }
                        break;
                    case 4: //ランダム
                        pl.value = rnd.Next(-pl.depth, pl.depth);
                        pl.waitCounter = pl.param[1];
                        procLfo_updateDepth(pl);
                        break;
                }

            }
        }

        private static void procLfo_updateDepth(clsLfo pl)
        {
            if (pl.param[7] != 0)
            {
                pl.depthWaitCounter--;
                if (pl.depthWaitCounter == 0)
                {
                    pl.depthWaitCounter = pl.param[7];
                    pl.depth += pl.param[8];
                    pl.depth = Common.CheckRange(pl.depth, 0, 32767);
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
                        //System.Diagnostics.Debug.Write("eAR");
                        pw.envVolume += pw.envelope[7]; // vol += ST
                        //System.Diagnostics.Debug.Write(string.Format(":{0}", pw.envVolume));
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
                        //System.Diagnostics.Debug.Write("eDR");
                        pw.envVolume -= pw.envelope[7]; // vol -= ST
                        //System.Diagnostics.Debug.Write(string.Format(":{0}", pw.envVolume));
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
                        //System.Diagnostics.Debug.Write("eSR");
                        pw.envVolume -= pw.envelope[7]; // vol -= ST
                        //System.Diagnostics.Debug.Write(string.Format(":{0}", pw.envVolume));
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
                        //System.Diagnostics.Debug.Write("eRR");
                        pw.envVolume -= pw.envelope[7]; // vol -= ST
                        //System.Diagnostics.Debug.Write(string.Format(":{0}", pw.envVolume));
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
                pw.chip.SetKeyOff(pw, null);
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
                case enmMMLType.DirectMode:
                    log.Write("DirectMode");
                    pw.chip.CmdDirectMode(pw, mml);
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
                case enmMMLType.Lyric:
                    log.Write("Lyric");
                    pw.chip.CmdLyric(pw, mml);
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
                case enmMMLType.PcmMap:
                    log.Write("PcmMapMode");
                    pw.chip.CmdPcmMapSw(pw, mml);
                    pw.mmlPos++;
                    break;
                case enmMMLType.Noise:
                    log.Write("Noise");
                    pw.chip.CmdNoise(pw, mml);
                    pw.mmlPos++;
                    break;
                case enmMMLType.DCSGCh3Freq:
                    log.Write("DCSGCh3Freq");
                    pw.chip.CmdDCSGCh3Freq(pw, mml);
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
                case enmMMLType.JumpPoint:
                    log.Write("JumpPoint");
                    jumpCommandCounter--;
                    if (jumpCommandCounter < 0) jumpCommandCounter = 0;
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
                case enmMMLType.AddressShift:
                    log.Write("AddressShift");
                    pw.chip.CmdAddressShift(pw, mml);
                    pw.mmlPos++;
                    break;
                case enmMMLType.MIDICh:
                    log.Write("MIDICh");
                    pw.chip.CmdMIDICh(pw, mml);
                    pw.mmlPos++;
                    break;
                case enmMMLType.MIDIControlChange:
                    log.Write("MIDIControlChange");
                    pw.chip.CmdMIDIControlChange(pw, mml);
                    pw.mmlPos++;
                    break;
                case enmMMLType.Velocity:
                    log.Write("Velocity");
                    pw.chip.CmdVelocity(pw, mml);
                    pw.mmlPos++;
                    break;
                case enmMMLType.SusOnOff:
                    log.Write("SusOnOff");
                    pw.chip.CmdSusOnOff(pw, mml);
                    pw.mmlPos++;
                    break;
                case enmMMLType.SkipPlay:
                    log.Write("SkipPlay");
                    useSkipPlayCommand = false;
                    if (doSkipStop)
                    {
                        pw.dataEnd = true;
                    }
                    pw.mmlPos++;
                    break;
                case enmMMLType.ToneDoubler:
                    log.Write("ToneDoubler");
                    //pw.chip.CmdToneDoubler(pw, mml);
                    pw.mmlPos++;
                    break;
                default:
                    msgBox.setErrMsg(string.Format(msg.get("E01016")
                        , mml.type)
                        , mml.line.Lp);
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
                    if (chip == null) continue;

                    chip.use = false;
                    chip.lstPartWork = new List<partWork>();

                    for (int i = 0; i < chip.ChMax; i++)
                    {
                        partWork pw = new partWork()
                        {
                            chip = chip,
                            chipNumber = ((info.format != enmFormat.ZGM && chip.ChipID == 1) ? 1 : 0),
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

                        chip.InitPart(pw);

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

        private byte[] DivInt2ByteAry(int n)
        {
            return new byte[4] {
                 (byte)( n & 0xff                   )
                ,(byte)((n & 0xff00    ) / 0x100    )
                ,(byte)((n & 0xff0000  ) / 0x10000  )
                ,(byte)((n & 0xff000000) / 0x1000000)
            };
        }

        //public void OutData(MML mml, params byte[] data)
        //{
        //    foreach (byte d in data)
        //    {
        //        outDatum od = new outDatum();
        //        od.val = d;
        //        if (mml != null)
        //        {
        //            od.type = mml.type;
        //            od.args = mml.args;
        //            if (mml.line != null && mml.line.Lp != null)
        //            {
        //                od.linePos = new LinePos(
        //                    mml.line.Lp.fullPath,
        //                    mml.line.Lp.row,
        //                    mml.line.Lp.col,
        //                    mml.line.Lp.length,
        //                    mml.line.Lp.part,
        //                    mml.line.Lp.chip,
        //                    mml.line.Lp.chipNumber,
        //                    mml.line.Lp.ch);
        //            }
        //        }
        //        dat.Add(od);
        //    }
        //}

        public void OutData(MML mml, byte[] cmd, params byte[] data)
        {
            if (cmd != null && cmd.Length > 0)
            {
                foreach (byte d in cmd)
                {
                    outDatum od = new outDatum();
                    od.val = d;
                    if (mml != null)
                    {
                        od.type = mml.type;
                        od.args = mml.args;
                        if (mml.line != null && mml.line.Lp != null)
                        {
                            od.linePos = new LinePos(
                                mml.line.Lp.srcMMLID,
                                mml.line.Lp.row,
                                mml.line.Lp.col,
                                mml.line.Lp.length,
                                mml.line.Lp.part,
                                mml.line.Lp.chip,
                                mml.line.Lp.chipIndex,
                                mml.line.Lp.chipNumber,
                                mml.line.Lp.ch);
                        }
                    }
                    dat.Add(od);

                    //Console.Write("{0:x02} :",d);
                }
            }

            foreach (byte d in data)
            {
                outDatum od = new outDatum();
                od.val = d;
                if (mml != null)
                {
                    od.type = mml.type;
                    od.args = mml.args;
                    if (mml.line != null && mml.line.Lp != null)
                    {
                        od.linePos = new LinePos(
                            mml.line.Lp.srcMMLID,
                            mml.line.Lp.row,
                            mml.line.Lp.col,
                            mml.line.Lp.length,
                            mml.line.Lp.part,
                            mml.line.Lp.chip,
                            mml.line.Lp.chipIndex,
                            mml.line.Lp.chipNumber,
                            mml.line.Lp.ch);
                    }
                }
                dat.Add(od);
                //Console.Write("{0:x02} :", d);
            }

            //Console.WriteLine("");
        }

        //public void OutData(outDatum od, params byte[] data)
        //{
        //    foreach (byte d in data)
        //    {
        //        outDatum o = new outDatum();
        //        o.val = d;
        //        if (od != null)
        //        {
        //            o.type = od.type;
        //            if (od.linePos != null)
        //            {
        //                o.linePos = new LinePos(
        //                    od.linePos.fullPath,
        //                    od.linePos.row,
        //                    od.linePos.col,
        //                    od.linePos.length,
        //                    od.linePos.part,
        //                    od.linePos.chip,
        //                    od.linePos.chipNumber,
        //                    od.linePos.ch);
        //            }
        //        }
        //        dat.Add(o);
        //    }
        //}

        public void OutData(outDatum od, byte[] cmd, params byte[] data)
        {
            if (cmd != null && cmd.Length > 0)
            {
                foreach (byte d in cmd)
                {
                    outDatum o = new outDatum();
                    o.val = d;
                    if (od != null)
                    {
                        o.type = od.type;
                        if (od.linePos != null)
                        {
                            o.linePos = new LinePos(
                                od.linePos.srcMMLID,
                                od.linePos.row,
                                od.linePos.col,
                                od.linePos.length,
                                od.linePos.part,
                                od.linePos.chip,
                                od.linePos.chipIndex,
                                od.linePos.chipNumber,
                                od.linePos.ch);
                        }
                    }
                    dat.Add(o);
                    //Console.Write("{0:x02} :", d);
                }
            }
            foreach (byte d in data)
            {
                outDatum o = new outDatum();
                o.val = d;
                if (od != null)
                {
                    o.type = od.type;
                    if (od.linePos != null)
                    {
                        o.linePos = new LinePos(
                            od.linePos.srcMMLID,
                            od.linePos.row,
                            od.linePos.col,
                            od.linePos.length,
                            od.linePos.part,
                            od.linePos.chip,
                            od.linePos.chipIndex,
                            od.linePos.chipNumber,
                            od.linePos.ch);
                    }
                }
                dat.Add(o);
                //Console.Write("{0:x02} :", d);
            }
            //Console.WriteLine("");
        }

        private void OutWaitNSamples(long n)
        {
            long m = n;
            byte[] cmd;
            if (info.format == enmFormat.ZGM)
            {
                if (ChipCommandSize == 2) cmd = new byte[] { 0x01, 0x00 };
                else cmd = new byte[] { 0x01 };
            }
            else cmd = new byte[] { 0x61 };

            while (m > 0)
            {
                if (m > 0xffff)
                {
                    OutData((MML)null, cmd, (byte)0xff, (byte)0xff);
                    m -= 0xffff;
                }
                else
                {
                    OutData((MML)null, cmd, (byte)(m & 0xff), (byte)((m & 0xff00) >> 8));
                    m = 0L;
                }
            }
        }

        private void OutWait735Samples(int repeatCount)
        {
            byte[] cmd;
            if (info.format == enmFormat.ZGM)
            {
                if (ChipCommandSize == 2) cmd = new byte[] { 0x02, 0x00 };
                else cmd = new byte[] { 0x02 };
            }
            else cmd = new byte[] { 0x62 };

            for (int i = 0; i < repeatCount; i++)
            {
                OutData((MML)null, cmd);
            }
        }

        private void OutWait882Samples(int repeatCount)
        {
            byte[] cmd;
            if (info.format == enmFormat.ZGM)
            {
                if (ChipCommandSize == 2) cmd = new byte[] { 0x03, 0x00 };
                else cmd = new byte[] { 0x03 };
            }
            else cmd = new byte[] { 0x63 };

            for (int i = 0; i < repeatCount; i++)
            {
                OutData((MML)null, cmd);
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
                    byte[] cmd;
                    if (info.format == enmFormat.ZGM)
                    {
                        if (ChipCommandSize == 2) cmd = new byte[] { (byte)(0x80 + f), 0x00 };
                        else cmd = new byte[] { (byte)(0x20 + f) };
                    }
                    else cmd = new byte[] { (byte)(0x80 + f) };
                    OutData((MML)null, cmd);
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
