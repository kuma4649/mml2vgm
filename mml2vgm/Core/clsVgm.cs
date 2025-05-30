﻿using Core.chips;
using musicDriverInterface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

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
        public SN76489X2[] sn76489x2 = null;
        public RF5C164[] rf5c164 = null;
        public segaPcm[] segapcm = null;
        public HuC6280[] huc6280 = null;
        public YM2612X[] ym2612x = null;
        public YM2612X2[] ym2612x2 = null;
        public YM2413[] ym2413 = null;
        public YM3526[] ym3526 = null;
        public Y8950[] y8950 = null;
        public YM3812[] ym3812 = null;
        public YMF262[] ymf262 = null;
        public YMF271[] ymf271 = null;
        public C140[] c140 = null;
        public C352[] c352 = null;
        public AY8910[] ay8910 = null;
        public Pokey[] pokey = null;
        public K051649[] k051649 = null;
        public QSound[] qsound = null;
        public K053260[] k053260 = null;
        public K054539[] k054539 = null;
        public MidiGM[] midiGM = null;
        public NES[] nes = null;
        public DMG[] dmg = null;
        public VRC6[] vrc6 = null;
        public Gigatron[] gigatron = null;

        public Dictionary<enmChipType, ClsChip[]> chips;


        public Dictionary<int, Tuple<string, byte[]>> instFM = new Dictionary<int, Tuple<string, byte[]>>();
        public Dictionary<int, Tuple<string, byte[]>> instOPM = new Dictionary<int, Tuple<string, byte[]>>();
        public Dictionary<int, Tuple<string, byte[]>> instOPL = new Dictionary<int, Tuple<string, byte[]>>();
        public Dictionary<int, Tuple<string, byte[]>> instOPX = new Dictionary<int, Tuple<string, byte[]>>();
        public Dictionary<int, Tuple<string, int[]>> instENV = new Dictionary<int, Tuple<string, int[]>>();
        public Dictionary<int, Tuple<string, clsPcm>> instPCM = new Dictionary<int, Tuple<string, clsPcm>>();
        public List<clsPcmDatSeq> instPCMDatSeq = new List<clsPcmDatSeq>();
        public Dictionary<int, Dictionary<int, int>> instPCMMap = new Dictionary<int, Dictionary<int, int>>();
        public Dictionary<int, clsToneDoubler> instToneDoubler = new Dictionary<int, clsToneDoubler>();
        public Dictionary<int, Tuple<string, byte[]>> instWF = new Dictionary<int, Tuple<string, byte[]>>();
        public Dictionary<int, Tuple<string, ushort[]>> instOPNA2WF = new Dictionary<int, Tuple<string, ushort[]>>();
        public Dictionary<int, Tuple<string, byte[]>> instOPNA2WFS = new Dictionary<int, Tuple<string, byte[]>>();
        public Dictionary<int, Tuple<string, byte[]>> instFDSMod = new Dictionary<int, Tuple<string, byte[]>>();
        public Dictionary<int, Tuple<string, byte[]>> midiSysEx = new Dictionary<int, Tuple<string, byte[]>>();
        public Dictionary<int, MmlDatum[]> instArp = new Dictionary<int, MmlDatum[]>();
        public Dictionary<int, MmlDatum[]> instVArp = new Dictionary<int, MmlDatum[]>();
        public Dictionary<int, MmlDatum[]> instCommandArp = new Dictionary<int, MmlDatum[]>();
        public Dictionary<int, byte[]> instTLOFS = new Dictionary<int, byte[]>();

        public Dictionary<string, List<List<Line>>>[] partData = new Dictionary<string, List<List<Line>>>[]{
            new Dictionary<string, List<List<Line>>>(),new Dictionary<string, List<List<Line>>>()
        };
        public Dictionary<string, Line> aliesData = new Dictionary<string, Line>();

        private int toneDoublerCounter = -1;
        private List<int> toneDoublerBufCache = new List<int>();

        private string instrumentName;

        private int instrumentCounter = -1;
        private byte[] instrumentBufCache = null;// new byte[Const.INSTRUMENT_SIZE];

        private int opmInstrumentCounter = -1;
        private byte[] opmInstrumentBufCache = null;

        private int oplInstrumentCounter = -1;
        private byte[] oplInstrumentBufCache = null;

        private int opxInstrumentCounter = -1;
        private byte[] opxInstrumentBufCache = null;

        private int wfInstrumentCounter = -1;
        private byte[] wfInstrumentBufCache = null;

        private int opna2wfInstrumentCounter = -1;
        private ushort[] opna2WfInstrumentBufCache = null;

        private int opna2wfsInstrumentCounter = -1;
        private byte[] opna2WfsInstrumentBufCache = null;


        public bool doSkip = false;
        public bool doSkipStop = false;
        public Point caretPoint = Point.Empty;
        private int midiSysExCounter = -1;
        private int instArpCounter = -1;
        private int instCommandArpCounter = -1;
        private int instVArpCounter = -1;
        private int instTLOFSCounter = -1;

        private int instModCounter = -1;
        private byte[] fdsModulationInstrumentBufCache = null;


        /// <summary>
        /// GUI/CUI向け(IDEは別)
        /// </summary>
        public bool doJumping { get; internal set; }


        public int newStreamID = -1;

        public SourceParser sp = null;
        public Information info = null;

        public byte[] pcmCache = null;
        public string fnPcmCache = "";


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

            List<SN76489X2> lstSN76489X2 = new List<SN76489X2>();
            n = sp.dicChipPartName[enmChipType.SN76489X2];
            for (int i = 0; i < n.Item3.Count; i++)
            {
                if (string.IsNullOrEmpty(n.Item3[i])) continue;
                if (sp.lnChipPartName.Contains(n.Item3[i]))
                    lstSN76489X2.Add(new SN76489X2(this, i, n.Item3[i], stPath, (info.format == enmFormat.ZGM ? 0 : i)));
            }
            if (lstSN76489X2.Count > 0)
            {
                sn76489x2 = lstSN76489X2.ToArray();
                chips.Add(enmChipType.SN76489X2, sn76489x2);
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

            List<YM2612X2> lstYM2612X2 = new List<YM2612X2>();
            n = sp.dicChipPartName[enmChipType.YM2612X2];
            for (int i = 0; i < n.Item3.Count; i++)
            {
                if (string.IsNullOrEmpty(n.Item3[i])) continue;
                if (sp.lnChipPartName.Contains(n.Item3[i]))
                    lstYM2612X2.Add(new YM2612X2(this, i, n.Item3[i], stPath, (info.format == enmFormat.ZGM ? 0 : i)));
            }
            if (lstYM2612X2.Count > 0)
            {
                ym2612x2 = lstYM2612X2.ToArray();
                chips.Add(enmChipType.YM2612X2, ym2612x2);
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

            List<YM3526> lstYM3526 = new List<YM3526>();
            n = sp.dicChipPartName[enmChipType.YM3526];
            for (int i = 0; i < n.Item3.Count; i++)
            {
                if (string.IsNullOrEmpty(n.Item3[i])) continue;
                if (sp.lnChipPartName.Contains(n.Item3[i]))
                    lstYM3526.Add(new YM3526(this, i, n.Item3[i], stPath, (info.format == enmFormat.ZGM ? 0 : i)));
            }
            if (lstYM3526.Count > 0)
            {
                ym3526 = lstYM3526.ToArray();
                chips.Add(enmChipType.YM3526, ym3526);
            }

            List<Y8950> lstY8950 = new List<Y8950>();
            n = sp.dicChipPartName[enmChipType.Y8950];
            for (int i = 0; i < n.Item3.Count; i++)
            {
                if (string.IsNullOrEmpty(n.Item3[i])) continue;
                if (sp.lnChipPartName.Contains(n.Item3[i]))
                    lstY8950.Add(new Y8950(this, i, n.Item3[i], stPath, (info.format == enmFormat.ZGM ? 0 : i)));
            }
            if (lstY8950.Count > 0)
            {
                y8950 = lstY8950.ToArray();
                chips.Add(enmChipType.Y8950, y8950);
            }

            List<YM3812> lstYM3812 = new List<YM3812>();
            n = sp.dicChipPartName[enmChipType.YM3812];
            for (int i = 0; i < n.Item3.Count; i++)
            {
                if (string.IsNullOrEmpty(n.Item3[i])) continue;
                if (sp.lnChipPartName.Contains(n.Item3[i]))
                    lstYM3812.Add(new YM3812(this, i, n.Item3[i], stPath, (info.format == enmFormat.ZGM ? 0 : i)));
            }
            if (lstYM3812.Count > 0)
            {
                ym3812 = lstYM3812.ToArray();
                chips.Add(enmChipType.YM3812, ym3812);
            }

            List<YMF262> lstYMF262 = new List<YMF262>();
            n = sp.dicChipPartName[enmChipType.YMF262];
            for (int i = 0; i < n.Item3.Count; i++)
            {
                if (string.IsNullOrEmpty(n.Item3[i])) continue;
                if (sp.lnChipPartName.Contains(n.Item3[i]))
                    lstYMF262.Add(new YMF262(this, i, n.Item3[i], stPath, (info.format == enmFormat.ZGM ? 0 : i)));
            }
            if (lstYMF262.Count > 0)
            {
                ymf262 = lstYMF262.ToArray();
                chips.Add(enmChipType.YMF262, ymf262);
            }

            List<YMF271> lstYMF271 = new List<YMF271>();
            n = sp.dicChipPartName[enmChipType.YMF271];
            for (int i = 0; i < n.Item3.Count; i++)
            {
                if (string.IsNullOrEmpty(n.Item3[i])) continue;
                if (sp.lnChipPartName.Contains(n.Item3[i]))
                    lstYMF271.Add(new YMF271(this, i, n.Item3[i], stPath, (info.format == enmFormat.ZGM ? 0 : i)));
            }
            if (lstYMF271.Count > 0)
            {
                ymf271 = lstYMF271.ToArray();
                chips.Add(enmChipType.YMF271, ymf271);
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

            List<C352> lstC352 = new List<C352>();
            n = sp.dicChipPartName[enmChipType.C352];
            for (int i = 0; i < n.Item3.Count; i++)
            {
                if (string.IsNullOrEmpty(n.Item3[i])) continue;
                if (sp.lnChipPartName.Contains(n.Item3[i]))
                    lstC352.Add(new C352(this, i, n.Item3[i], stPath, (info.format == enmFormat.ZGM ? 0 : i)));
            }
            if (lstC352.Count > 0)
            {
                c352 = lstC352.ToArray();
                chips.Add(enmChipType.C352, c352);
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

            List<Pokey> lstPokey = new List<Pokey>();
            n = sp.dicChipPartName[enmChipType.POKEY];
            for (int i = 0; i < n.Item3.Count; i++)
            {
                if (string.IsNullOrEmpty(n.Item3[i])) continue;
                if (sp.lnChipPartName.Contains(n.Item3[i]))
                    lstPokey.Add(new Pokey(this, i, n.Item3[i], stPath, (info.format == enmFormat.ZGM ? 0 : i)));
            }
            if (lstPokey.Count > 0)
            {
                pokey = lstPokey.ToArray();
                chips.Add(enmChipType.POKEY, pokey);
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

            List<K054539> lstK054539 = new List<K054539>();
            n = sp.dicChipPartName[enmChipType.K054539];
            for (int i = 0; i < n.Item3.Count; i++)
            {
                if (string.IsNullOrEmpty(n.Item3[i])) continue;
                if (sp.lnChipPartName.Contains(n.Item3[i]))
                    lstK054539.Add(new K054539(this, i, n.Item3[i], stPath, (info.format == enmFormat.ZGM ? 0 : i)));
            }
            if (lstK054539.Count > 0)
            {
                k054539 = lstK054539.ToArray();
                chips.Add(enmChipType.K054539, k054539);
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

            List<NES> lstNES = new List<NES>();
            n = sp.dicChipPartName[enmChipType.NES];
            for (int i = 0; i < n.Item3.Count; i++)
            {
                if (string.IsNullOrEmpty(n.Item3[i])) continue;
                if (sp.lnChipPartName.Contains(n.Item3[i]))
                    lstNES.Add(new NES(this, i, n.Item3[i], stPath, (info.format == enmFormat.ZGM ? 0 : i)));
            }
            if (lstNES.Count > 0)
            {
                nes = lstNES.ToArray();
                chips.Add(enmChipType.NES, nes);
            }

            List<DMG> lstDMG = new List<DMG>();
            n = sp.dicChipPartName[enmChipType.DMG];
            for (int i = 0; i < n.Item3.Count; i++)
            {
                if (string.IsNullOrEmpty(n.Item3[i])) continue;
                if (sp.lnChipPartName.Contains(n.Item3[i]))
                    lstDMG.Add(new DMG(this, i, n.Item3[i], stPath, (info.format == enmFormat.ZGM ? 0 : i)));
            }
            if (lstDMG.Count > 0)
            {
                dmg = lstDMG.ToArray();
                chips.Add(enmChipType.DMG, dmg);
            }

            List<VRC6> lstVRC6 = new List<VRC6>();
            n = sp.dicChipPartName[enmChipType.VRC6];
            for (int i = 0; i < n.Item3.Count; i++)
            {
                if (string.IsNullOrEmpty(n.Item3[i])) continue;
                if (sp.lnChipPartName.Contains(n.Item3[i]))
                    lstVRC6.Add(new VRC6(this, i, n.Item3[i], stPath, (info.format == enmFormat.ZGM ? 0 : i)));
            }
            if (lstVRC6.Count > 0)
            {
                vrc6 = lstVRC6.ToArray();
                chips.Add(enmChipType.VRC6, vrc6);
            }

            List<Gigatron> lstGigatron = new List<Gigatron>();
            n = sp.dicChipPartName[enmChipType.Gigatron];
            for (int i = 0; i < n.Item3.Count; i++)
            {
                if (string.IsNullOrEmpty(n.Item3[i])) continue;
                if (sp.lnChipPartName.Contains(n.Item3[i]))
                    lstGigatron.Add(new Gigatron(this, i, n.Item3[i], stPath, (info.format == enmFormat.ZGM ? 0 : i)));
            }
            if (lstGigatron.Count > 0)
            {
                gigatron = lstGigatron.ToArray();
                chips.Add(enmChipType.Gigatron, gigatron);
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

            // 定義中のWaveFormがあればここで定義完了
            if (wfInstrumentCounter != -1)
            {
                SetWfInstrument();
            }

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

            // 定義中のinstArpがあればここで定義完了
            if (instArpCounter != -1)
            {
                instArpCounter = -1;
                SetInstArp();
            }

            // 定義中のinstCommandArpがあればここで定義完了
            if (instCommandArpCounter != -1)
            {
                instCommandArpCounter = -1;
                SetInstCommandArp();
            }

            // 定義中のinstCommandArpがあればここで定義完了
            if (instModCounter != -1)
            {
                instModCounter = -1;
                SetInstModulation();
            }

            if (instTLOFSCounter != -1)
            {
                instTLOFSCounter = -1;
                SetInstTLOFS();
            }

            //定義中で終わってしまった場合はエラーとする
            CheckDefineInstrument(null);

            // チェック1定義されていない名称を使用したパートが存在するか

            for (int n = 0; n < 2; n++)
            {
                foreach (string p in partData[n].Keys)
                {
                    bool flg = false;
                    foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in chips)
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
                            new LinePos(null,"-")
                            );
                        flg = false;
                    }
                }
            }

            if (info.userClockCount != 0) info.clockCount = info.userClockCount;

            log.Write("テキスト解析完了");
            return 0;

        }

        private void CheckDefineInstrument(Line line)
        {
            if (instrumentCounter != -1)
            {
                if (line == null)
                    msgBox.setErrMsg(string.Format(msg.get("E01022"), "FM"), null);
                else
                    msgBox.setErrMsg(string.Format(msg.get("E01023"), "FM"), line.Lp);
            }
            if (opmInstrumentCounter != -1)
            {
                if (line == null)
                    msgBox.setErrMsg(string.Format(msg.get("E01022"), "OPM"), null);
                else
                    msgBox.setErrMsg(string.Format(msg.get("E01023"), "OPM"), line.Lp);
            }
            if (oplInstrumentCounter != -1)
            {
                if (line == null)
                    msgBox.setErrMsg(string.Format(msg.get("E01022"), "OPL"), null);
                else
                    msgBox.setErrMsg(string.Format(msg.get("E01023"), "OPL"), line.Lp);
            }
            if (opxInstrumentCounter != -1)
            {
                if (line == null)
                    msgBox.setErrMsg(string.Format(msg.get("E01022"), "OPX"), null);
                else
                    msgBox.setErrMsg(string.Format(msg.get("E01023"), "OPX"), line.Lp);
            }

            // opna2 WaveFormの音色を定義中の場合
            if (opna2wfInstrumentCounter != -1)
            {
                if (line == null)
                    msgBox.setErrMsg(string.Format(msg.get("E01022"), "WaveForm(OPNA2)"), null);
                else
                    msgBox.setErrMsg(string.Format(msg.get("E01023"), "WaveForm(OPNA2)"), line.Lp);
            }

            // opna2 WaveForm(SSG)の音色を定義中の場合
            if (opna2wfsInstrumentCounter != -1)
            {
                if (line == null)
                    msgBox.setErrMsg(string.Format(msg.get("E01022"), "WaveForm(OPNA2 SSG)"), null);
                else
                    msgBox.setErrMsg(string.Format(msg.get("E01023"), "WaveForm(OPNA2 SSG)"), line.Lp);
            }

            if (instModCounter != -1)
            {
                if (line == null)
                    msgBox.setErrMsg(string.Format(msg.get("E01022"), "FDS Modulation"), null);
                else
                    msgBox.setErrMsg(string.Format(msg.get("E01023"), "FDS Modulation"), line.Lp);
            }
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

            if (buf.ToUpper().IndexOf("TLOFS") == 0)
            {
                instrumentName = "";
                try
                {
                    instTLOFSCounter = 0;
                    StoreTLOFSBuffer(line);
                }
                catch
                {
                    msgBox.setWrnMsg(msg.get("E01xxx"), line.Lp);
                }
                return 0;
            }

            // FMの音色を定義中の場合
            if (instrumentCounter != -1)
            {
                return SetInstrument(line);
            }

            // OPMの音色を定義中の場合
            if (opmInstrumentCounter != -1)
            {
                return SetOpmInstrument(line);
            }

            // OPLの音色を定義中の場合
            if (oplInstrumentCounter != -1)
            {
                return SetOplInstrument(line);
            }

            // OPXの音色を定義中の場合
            if (opxInstrumentCounter != -1)
            {
                return SetOpxInstrument(line);
            }

            // opna2 WaveFormの音色を定義中の場合
            if (opna2wfInstrumentCounter != -1)
            {
                return SetOPNA2WfInstrument(line);
            }

            // opna2 WaveForm(SSG)の音色を定義中の場合
            if (opna2wfsInstrumentCounter != -1)
            {
                return SetOPNA2WfsInstrument(line);
            }

            if (instModCounter != -1)
            {
                return StoreInstModulationBuffer(line);
            }

            char t = (buf != null && buf.Length > 0) ? buf.ToUpper()[0] : '\0';
            char t1 = (buf != null && buf.Length > 1) ? buf.ToUpper()[1] : '\0';
            char t2 = (buf != null && buf.Length > 2) ? buf.ToUpper()[2] : '\0';

            if (wfInstrumentCounter != -1)
            {
                //他の定義が現れたらwaveformの定義は終了
                if (t == 'F' || t == 'N' || t == 'M' || t == 'L'
                    || t == 'A' || t == 'V' || t == 'C'
                    || t == 'P' || t == 'E'
                    || t == 'T' || t == 'H' || t == 'W' || t == 'S')
                {
                    SetWfInstrument();
                }
            }

            if (toneDoublerCounter != -1)
            {
                //他の定義が現れたらtoneDoublerの定義は終了
                if (t == 'F' || t == 'N' || t == 'M' || t == 'L'
                    || t == 'A' || t == 'V' || t == 'C'
                    || t == 'P' || t == 'E' 
                    || t == 'T' || t == 'H' || t == 'W' || t == 'S')
                {
                    toneDoublerCounter = -1;
                    SetInstToneDoubler();
                }
            }

            if (midiSysExCounter != -1)
            {
                //他の定義が現れたらSysExの定義は終了
                if (t == 'F' || t == 'N' || t == 'M' || t == 'L'
                    || t == 'A' || t == 'V' || t == 'C'
                    || t == 'P' || t == 'E'
                    || t == 'T' || t == 'H' || t == 'W' || t == 'S')
                {
                    midiSysExCounter = -1;
                    SetMidiSysEx();
                }
            }

            if (instArpCounter != -1)
            {
                //他の定義が現れたらArpの定義は終了
                if (t == 'F' || t == 'N' || t == 'M' || t == 'L'
                    || t == 'A' || t == 'V' || t == 'C'
                    || t == 'P' || t == 'E'
                    || t == 'T' || t == 'H' || t == 'W' || t == 'S')
                    SetInstArp();
            }

            if (instVArpCounter != -1)
            {
                //他の定義が現れたらVArpの定義は終了
                if (t == 'F' || t == 'N' || t == 'M' || t == 'L'
                    || t == 'A' || t == 'V' || t == 'C'
                    || t == 'P' || t == 'E'
                    || t == 'T' || t == 'H' || t == 'W' || t == 'S')
                    SetInstVArp();
            }

            if (instModCounter != -1)
            {
                //他の定義が現れたらArpの定義は終了
                if (t == 'F' || t == 'N' || t == 'M' || t == 'L'
                    || t == 'A' || t == 'V' || t == 'C'
                    || t == 'P' || t == 'E'
                    || t == 'T' || t == 'H' || t == 'W' || t == 'S')
                    SetInstModulation();
            }

            //定義中に次の定義を始めた場合はエラーとする
            CheckDefineInstrument(line);

            //instrumentName = "";
            string val;
            switch (t)
            {
                case 'F':
                    instrumentName = "";
                    instrumentBufCache = new byte[Const.INSTRUMENT_SIZE - 8];
                    instrumentCounter = 0;
                    SetInstrument(line);
                    return 0;

                case 'N':
                    instrumentName = "";
                    instrumentBufCache = new byte[Const.INSTRUMENT_SIZE];
                    instrumentCounter = 0;
                    SetInstrument(line);
                    return 0;

                case 'M':
                    if(t1=='O' && t2 == 'D')
                    {
                        instrumentName = "";
                        fdsModulationInstrumentBufCache = new byte[Const.FDS_MOD_INSTRUMENT_SIZE];
                        instModCounter = 0;
                        StoreInstModulationBuffer(line);
                        return 0;
                    }
                    instrumentName = "";
                    opmInstrumentBufCache = new byte[Const.OPM_INSTRUMENT_SIZE];
                    opmInstrumentCounter = 0;
                    SetOpmInstrument(line);
                    return 0;

                case 'L':
                    instrumentName = "";
                    val = buf.ToUpper();
                    if (val.Length > 1 && val[1] == 'L')
                        oplInstrumentBufCache = new byte[Const.OPLL_INSTRUMENT_SIZE];
                    else if (val.Length > 1 && val[1] == '4')
                    {
                        oplInstrumentBufCache = new byte[Const.OPL_OP4_INSTRUMENT_SIZE];
                        line.Txt = val.Substring(2);
                    }
                    else
                        oplInstrumentBufCache = new byte[Const.OPL3_INSTRUMENT_SIZE];
                    oplInstrumentCounter = 0;
                    SetOplInstrument(line);
                    return 0;

                case 'X':
                    instrumentName = "";
                    val = buf.ToUpper();
                    if (val.Length > 1 && val[1] == '1')
                        opxInstrumentBufCache = new byte[Const.OPX_1OP_INSTRUMENT_SIZE];
                    else if (val.Length > 1 && val[1] == '2')
                        opxInstrumentBufCache = new byte[Const.OPX_2OP_INSTRUMENT_SIZE];
                    else if (val.Length > 1 && val[1] == '3')
                        opxInstrumentBufCache = new byte[Const.OPX_3OP_INSTRUMENT_SIZE];
                    else
                        opxInstrumentBufCache = new byte[Const.OPX_4OP_INSTRUMENT_SIZE];
                    opxInstrumentCounter = 0;
                    SetOpxInstrument(line);
                    return 0;



                case 'A'://@ ARP
                    instrumentName = "";
                    if (t1 == 'R' && t2 == 'P')
                    {
                        instArpCounter = 0;
                        StoreInstArpBuffer(line);
                    }
                    else
                    {
                        instrumentBufCache = new byte[Const.OPNA2_INSTRUMENT_SIZE];
                        instrumentCounter = 0;
                        SetInstrument(line);
                    }
                    return 0;

                case 'V'://@ VAR
                    instrumentName = "";
                    if (t1 == 'A' && t2 == 'R')
                    {
                        instVArpCounter = 0;
                        StoreInstVArpBuffer(line);
                    }
                    return 0;

                case 'C'://@ CAR
                    instrumentName = "";
                    if (t1 == 'A' && t2 == 'R')
                    {
                        instCommandArpCounter = 0;
                        StoreInstCommandArpBuffer(line);
                    }
                    return 0;

                case 'P':// @ P
                    instrumentName = "";
                    definePCMInstrument(line);
                    return 0;

                case 'E':// @ E
                    instrumentName = "";
                    try
                    {
                        instrumentCounter = -1;
                        //string[] vs = buf.Substring(1).Trim().Split(new string[] { "," }, StringSplitOptions.None);
                        string[] vs = Common.CutComment(buf).Substring(1).Trim().Split(new string[] { "," }, StringSplitOptions.None);
                        int[] env = null;
                        if (vs.Length < 10) env = new int[9];
                        else env = new int[13];
                        int num = int.Parse(vs[0]);
                        enmChipType chiptype = enmChipType.SN76489;
                        int envType = 8;
                        if (vs.Length > 9) envType = 12;

                        for (int i = 0; i < env.Length; i++)
                        {
                            if (i == envType)
                            {
                                if ((envType == 8 && vs.Length < 10) 
                                    || (envType == 12 && vs.Length > 9))
                                {
                                    if (vs.Length == envType) env[i] = (int)enmChipType.SN76489;
                                    else
                                    {
                                        chiptype = GetChipType(vs[envType]);
                                        env[i] = (int)chiptype;
                                    }
                                    continue;
                                }
                            }
                            env[i] = int.Parse(vs[i]);
                        }

                        if (chips.ContainsKey(chiptype))
                        {
                            ClsChip c = chips[chiptype][0] != null ? chips[chiptype][0] : chips[chiptype][1];
                            if (c.Envelope != null)
                            {
                                CheckEnvelopeVolumeRange(line, env, c.Envelope.Max, c.Envelope.Min,envType);
                                //if (env[7] == 0) env[7] = 1;
                            }
                        }
                        else
                        {
                            msgBox.setWrnMsg(msg.get("E01004"), line.Lp);
                        }

                        if (instENV.ContainsKey(num))
                            instENV.Remove(num);
                        instENV.Add(num, new Tuple<string, int[]>("", env));
                    }
                    catch
                    {
                        msgBox.setWrnMsg(msg.get("E01005"), line.Lp);
                    }
                    return 0;

                case 'T':// @ T
                    instrumentName = "";
                    try
                    {
                        instrumentCounter = -1;

                        if (buf.ToUpper()[1] != 'D') return 0;

                        toneDoublerBufCache.Clear();
                        StoreToneDoublerBuffer(Common.CutComment(buf).ToUpper().Substring(2).TrimStart(), line);
                    }
                    catch
                    {
                        msgBox.setWrnMsg(msg.get("E01006"), line.Lp);
                    }
                    return 0;

                case 'H':// @ H
                    instrumentName = "";
                    wfInstrumentBufCache = new byte[
                        Math.Max(Const.WF_HuC_K051_INSTRUMENT_SIZE, Const.WF_FDS_INSTRUMENT_SIZE)
                        ];
                    wfInstrumentCounter = 0;
                    StoreWfInstrument(line);
                    return 0;

                case 'W':// @ W
                    instrumentName = "";
                    if (buf.ToUpper()[1] == 'S')
                    {
                        opna2WfsInstrumentBufCache = new byte[Const.OPNA2_WFS_INSTRUMENT_SIZE];
                        opna2wfsInstrumentCounter = 0;
                        SetOPNA2WfsInstrument(line);
                    }
                    else
                    {
                        opna2WfInstrumentBufCache = new ushort[Const.OPNA2_WF_INSTRUMENT_SIZE];
                        opna2wfInstrumentCounter = 0;
                        SetOPNA2WfInstrument(line);
                    }
                    return 0;

                case 'S':// @ S
                    instrumentName = "";
                    midiSysExCounter = 0;
                    StoreMidiSysExBuffer(line);
                    return 0;
            }

            // Waveformを定義中の場合
            if (wfInstrumentCounter != -1)
            {
                return StoreWfInstrument(line);
            }

            // ToneDoublerを定義中の場合
            if (toneDoublerCounter != -1)
            {
                return StoreToneDoublerBuffer(Common.CutComment(buf).ToUpper(), line);
            }

            if (midiSysExCounter != -1)
            {
                return StoreMidiSysExBuffer(line);
            }

            if (instArpCounter != -1)
            {
                return StoreInstArpBuffer(line);
            }

            if (instVArpCounter != -1)
            {
                return StoreInstVArpBuffer(line);
            }

            if (instCommandArpCounter != -1)
            {
                return StoreInstCommandArpBuffer(line);
            }

            if (instTLOFSCounter != -1)
            {
                return StoreTLOFSBuffer(line);
            }

            return 0;
        }

        private void defineMUCOM88ADPCMInstrument(Line line)
        {
            try
            {
                string[] vs = Common.CutComment(line.Txt).Trim().Substring(2).Trim().Substring(12).Trim().Split(new string[] { "," }, StringSplitOptions.None);
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
                        , info.no + num
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
                string[] vs = Common.CutComment(line.Txt).Trim().Substring(2).Trim().Substring(7).Trim().Split(new string[] { "," }, StringSplitOptions.None);
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
                        instFM.Remove(num + p / 32);
                    instFM.Add(num + p / 32, new Tuple<string, byte[]>("", ConvertMUCOM88toM(num, voi)));
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
                string[] vs = Common.CutComment(line.Txt).Trim().Substring(2).Trim().Substring(3).Trim().Split(new string[] { "," }, StringSplitOptions.None);
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
                    instFM.Remove(num);
                instFM.Add(num, new Tuple<string, byte[]>("", ConvertTFItoM(num, buf)));

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

        private void defineTLOFSInstrument(Line line)
        {
            try
            {
                string[] vs = Common.CutComment(line.Txt).Trim().Substring(2).Trim().Substring(5).Trim().Split(new string[] { "," }, StringSplitOptions.None);
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
                    instFM.Remove(num);
                instFM.Add(num, new Tuple<string, byte[]>("", ConvertTFItoM(num, buf)));

                return;
            }
            catch
            {
                msgBox.setWrnMsg(msg.get("E01021"), line.Lp);
            }
        }

        private void definePCMInstrument(Line line)
        {
            try
            {
                string[] vs = Common.CutComment(line.Txt).Trim().Substring(2).Trim().Substring(1).Trim().Split(new string[] { "," }, StringSplitOptions.None);
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
        /// '@ P No , "FileName" , [BaseFreq] , Volume ( , [ChipName] , [Option...] )
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
                if (enmChip != enmChipType.YM2612X)
                {
                    msgBox.setErrMsg(msg.get("E01017"), line.Lp);
                    return;
                }
            }
            else if (info.format == enmFormat.XGM2)
            {
                if (enmChip != enmChipType.YM2612X2)
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
                if (enmChip == enmChipType.Y8950)
                    opt = 0;

                if (enmChip == enmChipType.YM2610B)
                    opt = 0;

                if (enmChip == enmChipType.YM2612X)
                    opt = 36;

                if (enmChip == enmChipType.YM2612X2)
                    opt = 36;
            }

            object[] option = null;
            if (vs.Length > 6)
            {
                option = new object[vs.Length - 6];
                for (int i = 0; i < option.Length; i++)
                {
                    option[i] = vs[i + 6];
                }
            }

            for (int i = 0; i < instPCMDatSeq.Count; i++)
            {
                clsPcmDatSeq ele = instPCMDatSeq[i];
                if (ele.No == num)
                {
                    instPCMDatSeq.Remove(ele);
                    i--;
                }
            }
            //if (instPCM.ContainsKey(num))
            //{
            //    instPCM.Remove(num);
            //}


            instPCMDatSeq.Add(new clsPcmDatSeq(
                enmPcmDefineType.Easy
                , num
                , fn
                , fq
                , vol
                , enmChip
                , chipNumber
                , opt
                , option
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
                SrcStartAdr = Common.ParseNumber(vs[2]);
            int DesStartAdr = 0;
            if (vs.Length > 3 && !string.IsNullOrEmpty(vs[3].Trim()))
                 DesStartAdr = Common.ParseNumber(vs[3]);
            int Length = -1;
            if (vs.Length > 4 && !string.IsNullOrEmpty(vs[4].Trim()))
                Length = Common.ParseNumber(vs[4]);
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
                EndAdr = Common.ParseNumber(vs[4]);
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
                    LoopAdr = Common.ParseNumber(vs[5]);
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
            if (ChipName == enmChipType.YM2610B)
            {
                if (Option == null || Option.Length < 1)
                    LoopAdr = 0;
                else
                {
                    LoopAdr = 1;
                    if (Option[0].Trim() != "1")
                        LoopAdr = 0;
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
                instPCMMap.Add(map, new Dictionary<int, int>());

            int i = 0;
            while (i < vs.Length - 3)
            {
                int no;
                if (string.IsNullOrEmpty(vs[3 + i]))
                {
                    i++;
                    continue;
                }
                string nums = vs[3 + i].Trim().ToUpper();
                bool isHalf = false;
                if (nums.IndexOf('H') != -1)
                {
                    isHalf = true;
                    nums = nums.Replace("H", "");
                }
                else if (nums.IndexOf('F') != -1)
                {
                    isHalf = false;
                    nums = nums.Replace("F", "");
                }
                no = Common.ParseNumber(nums);
                no = Math.Min(Math.Max(no, 0), 255);

                if (instPCMMap[map].ContainsKey(oct * 12 + note + i))
                    instPCMMap[map].Remove(oct * 12 + note + i);
                instPCMMap[map].Add(oct * 12 + note + i, no + (isHalf ? 0x1_0000 : 0x0));
                i++;
            }
        }

        private static void CheckEnvelopeVolumeRange(Line line, int[] env, int max, int min,int envType)
        {
            if (envType == 8)
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
            if (envType == 12)
            {
                for (int i = 0; i < env.Length - 1; i++)
                {
                    if (i != 1 && i != 6 && i != 9) continue;

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

            ClsChip cp = null;
            cp = GetChip(chipName);
            if (cp == null)
                cp = GetChipFromPartName(chipName);

            if (cp == null || !cp.CanUsePcm)
            {
                msgBox.setWrnMsg(string.Format(msg.get("E01002"), chipName), line.Lp);
                return enmChipType.None;
            }
            enmChip = GetChipType(chipName);
            if (enmChip == enmChipType.None)
                enmChip = GetChipTypeFromPartName(chipName);

            return enmChip;
        }

        private enmChipType GetChipType(string chipN)
        {
            foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in chips)
            {
                foreach (ClsChip chip in kvp.Value)
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
                msgBox.setWrnMsg(msg.get("E01010"), line.Lp);

            if (aliesData.ContainsKey(name))
                aliesData.Remove(name);
            Line l = new Line(new LinePos(
                line.Lp.document,
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

            part = Common.DivParts(buf.Substring(1, i).Trim(), chips, out int ura, out bool isLayer);
            data = buf.Substring(i).Trim();
            if (part == null)
            {
                //パート指定がない場合は警告とする
                msgBox.setWrnMsg(msg.get("E01011"), line.Lp);
                return -1;
            }
            if (data == "") line.Txt += " ";

            int pIndex = 0;
            foreach (string p in part)
            {
                int n = isLayer ? 1 : 0;

                if (!partData[n].ContainsKey(p))
                    partData[n].Add(p, new List<List<Line>>());
                Line l = new Line(new LinePos(
                    line.Lp.document,
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
                l.Index = pIndex++;
                l.Count = part.Count;
                while (partData[n][p].Count < ura + 1)
                {
                    partData[n][p].Add(new List<Line>());
                }

                partData[n][p][ura].Add(l);
            }

            return 0;
        }

        private int SetInstrument(Line line)
        {

            try
            {
                string name = "";
                instrumentCounter = GetNums(instrumentBufCache, instrumentCounter, Common.CutComment(line.Txt).Substring(1).TrimStart(), ref name, line);
                if (string.IsNullOrEmpty(instrumentName)) instrumentName = name;//音色名

                if (instrumentCounter == instrumentBufCache.Length)
                {
                    //すでに定義済みの場合はいったん削除する(後に定義されたものが優先)
                    if (instFM.ContainsKey(instrumentBufCache[0]))
                        instFM.Remove(instrumentBufCache[0]);


                    if (instrumentBufCache.Length == Const.INSTRUMENT_SIZE)
                        instFM.Add(instrumentBufCache[0], new Tuple<string, byte[]>(instrumentName, instrumentBufCache));
                    else if (instrumentBufCache.Length == Const.OPLL_INSTRUMENT_SIZE)
                        instFM.Add(instrumentBufCache[0], new Tuple<string, byte[]>(instrumentName, instrumentBufCache));
                    else if (instrumentBufCache.Length == Const.OPL3_INSTRUMENT_SIZE)
                        instFM.Add(instrumentBufCache[0], new Tuple<string, byte[]>(instrumentName, instrumentBufCache));
                    else if (instrumentBufCache.Length == Const.OPL_OP4_INSTRUMENT_SIZE)
                        instFM.Add(instrumentBufCache[0], new Tuple<string, byte[]>(instrumentName, instrumentBufCache));
                    else if (instrumentBufCache.Length == Const.OPNA2_INSTRUMENT_SIZE)
                        instFM.Add(instrumentBufCache[0], new Tuple<string, byte[]>(instrumentName, instrumentBufCache));
                    else
                    {
                        //F
                        instFM.Add(instrumentBufCache[0], new Tuple<string, byte[]>(instrumentName, ConvertFtoM(instrumentBufCache)));
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

        private int SetOpmInstrument(Line line)
        {

            try
            {
                string name = "";
                opmInstrumentCounter = GetNums(opmInstrumentBufCache, opmInstrumentCounter, Common.CutComment(line.Txt).Substring(1).TrimStart(), ref name, line);
                if (string.IsNullOrEmpty(instrumentName)) instrumentName = name;//音色名

                if (opmInstrumentCounter == opmInstrumentBufCache.Length)
                {
                    //すでに定義済みの場合はいったん削除する(後に定義されたものが優先)
                    if (instOPM.ContainsKey(opmInstrumentBufCache[0]))
                        instOPM.Remove(opmInstrumentBufCache[0]);


                    if (opmInstrumentBufCache.Length == Const.OPM_INSTRUMENT_SIZE)
                        instOPM.Add(opmInstrumentBufCache[0], new Tuple<string, byte[]>(instrumentName, opmInstrumentBufCache));

                    opmInstrumentCounter = -1;
                }
            }
            catch
            {
                msgBox.setErrMsg(msg.get("E01012"), line.Lp);
            }

            return 0;
        }

        private int SetOplInstrument(Line line)
        {

            try
            {
                string name = "";
                oplInstrumentCounter = GetNums(oplInstrumentBufCache, oplInstrumentCounter, Common.CutComment(line.Txt).Substring(1).TrimStart(), ref name, line);
                if (string.IsNullOrEmpty(instrumentName)) instrumentName = name;//音色名

                if (oplInstrumentCounter == oplInstrumentBufCache.Length)
                {
                    //すでに定義済みの場合はいったん削除する(後に定義されたものが優先)
                    if (instOPL.ContainsKey(oplInstrumentBufCache[0]))
                        instOPL.Remove(oplInstrumentBufCache[0]);


                    if (oplInstrumentBufCache.Length == Const.OPLL_INSTRUMENT_SIZE)
                        instOPL.Add(oplInstrumentBufCache[0], new Tuple<string, byte[]>(instrumentName, oplInstrumentBufCache));
                    else if (oplInstrumentBufCache.Length == Const.OPL3_INSTRUMENT_SIZE)
                        instOPL.Add(oplInstrumentBufCache[0], new Tuple<string, byte[]>(instrumentName, oplInstrumentBufCache));
                    else if (oplInstrumentBufCache.Length == Const.OPL_OP4_INSTRUMENT_SIZE)
                        instOPL.Add(oplInstrumentBufCache[0], new Tuple<string, byte[]>(instrumentName, oplInstrumentBufCache));

                    oplInstrumentCounter = -1;
                }
            }
            catch
            {
                msgBox.setErrMsg(msg.get("E01012"), line.Lp);
            }

            return 0;
        }

        private int SetOpxInstrument(Line line)
        {

            try
            {
                string name = "";
                opxInstrumentCounter = GetNums(opxInstrumentBufCache, opxInstrumentCounter, Common.CutComment(line.Txt).Substring(1).TrimStart(), ref name, line);
                if (string.IsNullOrEmpty(instrumentName)) instrumentName = name;//音色名

                if (opxInstrumentCounter == opxInstrumentBufCache.Length)
                {
                    //すでに定義済みの場合はいったん削除する(後に定義されたものが優先)
                    if (instOPX.ContainsKey(opxInstrumentBufCache[1]))
                        instOPX.Remove(opxInstrumentBufCache[1]);

                    if (opxInstrumentBufCache.Length == Const.OPX_4OP_INSTRUMENT_SIZE
                     || opxInstrumentBufCache.Length == Const.OPX_3OP_INSTRUMENT_SIZE
                     || opxInstrumentBufCache.Length == Const.OPX_2OP_INSTRUMENT_SIZE
                     || opxInstrumentBufCache.Length == Const.OPX_1OP_INSTRUMENT_SIZE)
                        instOPX.Add(opxInstrumentBufCache[1], new Tuple<string, byte[]>(instrumentName, opxInstrumentBufCache));

                    opxInstrumentCounter = -1;
                }
            }
            catch
            {
                msgBox.setErrMsg(msg.get("E01012"), line.Lp);
            }

            return 0;
        }

        private int StoreWfInstrument(Line line)
        {

            try
            {
                string name = "";
                wfInstrumentCounter = GetNums(wfInstrumentBufCache, wfInstrumentCounter, Common.CutComment(line.Txt).Substring(1).TrimStart(), ref name, line);
                if (string.IsNullOrEmpty(instrumentName)) instrumentName = name;//音色名

                //音色定義形式「H」の最大の大きさ(65:FDS waveform)の場合はここでWFの一覧に加える。
                //(33(K051やHuc)の場合はこの時点では検知できないので、他の定義が始まるときに検出する処理に任せる)
                if (wfInstrumentCounter == 65)
                    SetWfInstrument();
            }
            catch
            {
                msgBox.setErrMsg(msg.get("E01013"), line.Lp);
            }

            return 0;
        }

        private int SetWfInstrument()
        {

            try
            {
                if (wfInstrumentCounter == Const.WF_FDS_INSTRUMENT_SIZE
                    || wfInstrumentCounter == Const.WF_HuC_K051_INSTRUMENT_SIZE)
                {
                    if (instWF.ContainsKey(wfInstrumentBufCache[0]))
                        instWF.Remove(wfInstrumentBufCache[0]);

                    if (wfInstrumentCounter == Const.WF_HuC_K051_INSTRUMENT_SIZE)
                    {
                        byte[] b = new byte[Const.WF_HuC_K051_INSTRUMENT_SIZE];
                        Array.Copy(wfInstrumentBufCache, b, Const.WF_HuC_K051_INSTRUMENT_SIZE);
                        instWF.Add(b[0], new Tuple<string, byte[]>(instrumentName, b));
                    }
                    else
                    {
                        byte[] b = new byte[Const.WF_FDS_INSTRUMENT_SIZE];
                        Array.Copy(wfInstrumentBufCache, b, Const.WF_FDS_INSTRUMENT_SIZE);
                        instWF.Add(b[0], new Tuple<string, byte[]>(instrumentName, b));
                    }

                    wfInstrumentCounter = -1;
                }
            }
            catch
            {
            }

            return 0;
        }

        private int SetOPNA2WfInstrument(Line line)
        {

            try
            {
                string name="";
                opna2wfInstrumentCounter = GetNums2(opna2WfInstrumentBufCache, opna2wfInstrumentCounter, Common.CutComment(line.Txt).Substring(1).TrimStart(),ref name);
                if (string.IsNullOrEmpty(instrumentName)) instrumentName = name;//音色名

                if (opna2wfInstrumentCounter == opna2WfInstrumentBufCache.Length)
                {
                    if (instOPNA2WF.ContainsKey(opna2WfInstrumentBufCache[0]))
                        instOPNA2WF.Remove(opna2WfInstrumentBufCache[0]);
                    instOPNA2WF.Add(opna2WfInstrumentBufCache[0], new Tuple<string, ushort[]>(instrumentName, opna2WfInstrumentBufCache));

                    opna2wfInstrumentCounter = -1;
                }
            }
            catch
            {
                msgBox.setErrMsg(msg.get("E01013"), line.Lp);
            }

            return 0;
        }

        private int SetOPNA2WfsInstrument(Line line)
        {

            try
            {
                string name = "";
                opna2wfsInstrumentCounter = GetNums(opna2WfsInstrumentBufCache, opna2wfsInstrumentCounter, Common.CutComment(line.Txt).Substring(1).TrimStart(), ref name, line);
                if (string.IsNullOrEmpty(instrumentName)) instrumentName = name;//音色名

                if (opna2wfsInstrumentCounter == opna2WfsInstrumentBufCache.Length)
                {
                    if (instOPNA2WFS.ContainsKey(opna2WfsInstrumentBufCache[0]))
                        instOPNA2WFS.Remove(opna2WfsInstrumentBufCache[0]);
                    instOPNA2WFS.Add(opna2WfsInstrumentBufCache[0], new Tuple<string, byte[]>(instrumentName, opna2WfsInstrumentBufCache));

                    opna2wfsInstrumentCounter = -1;
                }
            }
            catch
            {
                msgBox.setErrMsg(msg.get("E01013"), line.Lp);
            }

            return 0;
        }

        private int GetNums(byte[] aryBuf, int aryIndex, string vals,ref string name,Line line)
        {
            string n = "";
            string h = "";
            int hc = -1;
            int i = 0;

            for (int ind = 0; ind < vals.Length; ind++)
            {
                char c = vals[ind];
                if (c == '$')
                {
                    hc = 0;
                    continue;
                }

                if (hc > -1 && ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f')))
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

                if (c == '"')
                {
                    name = "";
                    int j = ind + 1;
                    for (; j < vals.Length && vals[j] != '"'; j++)
                    {
                        name += vals[j];
                    }
                    ind = j;
                    n = "";
                    continue;
                }

                if (int.TryParse(n, out i))
                {
                    if (aryBuf.Length <= aryIndex)
                    {
                        msgBox.setErrMsg(msg.get("E01024"), line == null ? null : line.Lp);
                        return aryIndex;
                    }
                    else
                    {
                        aryBuf[aryIndex] = (byte)(i & 0xff);
                    }
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

        private int GetNums2(ushort[] aryBuf, int aryIndex, string vals, ref string name)
        {
            string n = "";
            string h = "";
            int hc = -1;
            int i = 0;

            for (int ind = 0; ind < vals.Length; ind++)
            {
                char c = vals[ind];
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

                if (c == '"')
                {
                    name = "";
                    int j = ind + 1;
                    for (; j < vals.Length && vals[j] != '"'; j++)
                    {
                        name += vals[j];
                    }
                    ind = j;
                    n = "";
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

        private List<byte> GetNums(int ptr, string vals, ref string name)
        {
            List<byte> lstBuf = new List<byte>();
            string n = "";
            string h = "";
            int hc = -1;
            int i = 0;
            int p = 0;

            for (int ind = 0; ind < vals.Length; ind++)
            {
                char c = vals[ind];
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

                if (c == '"')
                {
                    name = "";
                    int j = ind + 1;
                    for (; j < vals.Length && vals[j] != '"'; j++)
                    {
                        name += vals[j];
                    }
                    ind = j;
                    n = "";
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

        /// <summary>
        /// ARP専用
        /// </summary>
        private List<MmlDatum> GetNumsIntForArp(int ptr, string vals)
        {
            List<MmlDatum> lstBuf = new List<MmlDatum>();
            string n = "";
            string h = "";
            int hc = -1;
            int i = 0;
            int p = 0;
            MmlDatum dat = null;
            enmMMLType tp;
            tp = enmMMLType.Note;

            foreach (char c in vals)
            {
                p++;
                if (p <= ptr) continue;

                if (c == '|' || c == '/')
                {
                    hc = -1;
                    n = "";
                    dat = new MmlDatum();
                    dat.type = enmMMLType.LoopPoint;
                    dat.dat = c == '|' ? 0 : 1;
                    lstBuf.Add(dat);
                    continue;
                }
                else if (c == 'E')
                {
                    hc = -1;
                    n = "";
                    dat = new MmlDatum();
                    dat.type = enmMMLType.LoopPoint;
                    dat.dat = 2;
                    lstBuf.Add(dat);
                    continue;
                }
                else if (c == '#')
                {
                    tp = enmMMLType.LengthClock;
                    continue;

                }
                else if (c == 'Q')
                {
                    tp = enmMMLType.Gatetime;
                    continue;
                }

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
                        dat = new MmlDatum();
                        dat.type = enmMMLType.Note;
                        dat.dat = (byte)(i & 0xff);
                        lstBuf.Add(dat);
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
                    dat = new MmlDatum();
                    dat.type = tp;
                    dat.dat = i;
                    lstBuf.Add(dat);

                    tp = enmMMLType.Note;
                    n = "";
                }
            }

            if (!string.IsNullOrEmpty(n))
            {
                if (int.TryParse(n, out i))
                {
                    dat = new MmlDatum();
                    dat.type = enmMMLType.Note;
                    dat.dat = i;
                    lstBuf.Add(dat);
                    n = "";
                }
            }

            return lstBuf;
        }

        /// <summary>
        /// VAR専用
        /// </summary>
        private List<MmlDatum> GetNumsIntForVArp(int ptr, string vals)
        {
            List<MmlDatum> lstBuf = new List<MmlDatum>();
            string n = "";
            string h = "";
            int hc = -1;
            int i = 0;
            int p = 0;
            MmlDatum dat = null;
            enmMMLType tp;
            tp = enmMMLType.Note;

            foreach (char c in vals)
            {
                p++;
                if (p <= ptr) continue;

                if (c == '|' || c == '/')
                {
                    hc = -1;
                    n = "";
                    dat = new MmlDatum();
                    dat.type = enmMMLType.LoopPoint;
                    dat.dat = c == '|' ? 0 : 1;
                    lstBuf.Add(dat);
                    continue;
                }
                else if (c == 'E')
                {
                    hc = -1;
                    n = "";
                    dat = new MmlDatum();
                    dat.type = enmMMLType.LoopPoint;
                    dat.dat = 2;
                    lstBuf.Add(dat);
                    continue;
                }
                else if (c == '#')
                {
                    tp = enmMMLType.LengthClock;
                    continue;

                }

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
                        dat = new MmlDatum();
                        dat.type = enmMMLType.Note;
                        dat.dat = (byte)(i & 0xff);
                        lstBuf.Add(dat);
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
                    dat = new MmlDatum();
                    dat.type = tp;
                    dat.dat = i;
                    lstBuf.Add(dat);

                    tp = enmMMLType.Note;
                    n = "";
                }
            }

            if (!string.IsNullOrEmpty(n))
            {
                if (int.TryParse(n, out i))
                {
                    dat = new MmlDatum();
                    dat.type = enmMMLType.Note;
                    dat.dat = i;
                    lstBuf.Add(dat);
                    n = "";
                }
            }

            return lstBuf;
        }

        /// <summary>
        /// Command ARP専用
        /// </summary>
        private List<MmlDatum> GetNumsIntForCommandArp(int ptr, string anavals, int sin = 0, enmMMLType defaultMMLType = enmMMLType.Instrument, List<object> defaultMMLArgs = null)
        {
            //wordのリストを作成する
            List<string> lstWord = new List<string>();
            string wrd = "";
            int strSw = 0;
            for (int i = ptr; i < anavals.Length; i++)
            {
                char c = anavals[i];

                if (c == '"')
                {
                    strSw++;
                    strSw &= 1;
                    continue;
                }

                if ((c == ',' || c == ' ') && strSw == 0)
                {
                    if (wrd.Trim() != "")
                    {
                        lstWord.Add(wrd.Trim());
                    }
                    wrd = "";
                    continue;
                }

                wrd += c;
            }
            if (!string.IsNullOrEmpty(wrd)) lstWord.Add(wrd);


            List<MmlDatum> lstBuf = new List<MmlDatum>();
            enmMMLType tp = defaultMMLType;
            List<object> tpArgs = defaultMMLArgs;
            enmMMLType defTp = tp;
            List<object> defArgs = tpArgs;

            foreach (string vals in lstWord)
            {

                if (tp == enmMMLType.Y && (vals.IndexOf(',') > -1 || vals.IndexOf(' ') > -1))
                {
                    MmlDatum d = new MmlDatum();
                    d.type = enmMMLType.Y;
                    d.args = new List<object>();
                    d.dat = 0;
                    string[] items = vals.Split(new string[] { ",", " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string item in items)
                    {
                        object ret = GetYCommandItemsAtCmdArp(item);
                        if (ret == null) continue;
                        d.args.Add(ret);
                    }
                    if (d.args.Count == 3 && d.args[0] is string)
                    {
                        //opを0始まりに直す
                        int op = (int)d.args[1] - 1;
                        op = Common.CheckRange(op, 0, 3);
                        d.args[1] = op;
                    }

                    if (d.args.Count == 2 || d.args.Count == 3)
                        lstBuf.Add(d);
                }
                else
                {
                    GetItemsAtCmdArp(sin, lstBuf, ref tp, ref tpArgs, ref defTp, ref defArgs, vals);
                }
            }

            return lstBuf;
        }

        /// <summary>
        /// TLOFS専用
        /// </summary>
        private List<byte> GetNumsIntForTLOFS(int ptr, string vals)
        {
            List<byte> lstBuf = new List<byte>();
            string n = "";
            string h = "";
            int hc = -1;
            int i;
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

                if (c == 'O')
                {
                    if (vals.Length > p + 1)
                    {
                        string v = vals.Substring(p, 2);
                        if (v == "CT")
                        {
                            lstBuf.Add(0);
                            continue;
                        }
                    }
                }

                if (c == 'V')
                {
                    if (vals.Length > p + 1)
                    {
                        string v = vals.Substring(p, 2);
                        if (v == "OL")
                        {
                            lstBuf.Add(1);
                            continue;
                        }
                    }
                }

                if (hc > -1 && ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'F')))
                {
                    h += c;
                    hc++;
                    if (hc == 2)
                    {
                        i = int.Parse(h, System.Globalization.NumberStyles.HexNumber);
                        byte dat = (byte)i;
                        lstBuf.Add(dat);
                        h = "";
                        hc = -1;
                    }
                    continue;
                }

                if ((c >= '0' && c <= '9') || c == '-' || c == '+')
                {
                    n +=  c.ToString();
                    continue;
                }

                if (int.TryParse(n, out i))
                {
                    lstBuf.Add((byte)i);
                    n = "";
                }
            }

            if (!string.IsNullOrEmpty(n))
            {
                if (int.TryParse(n, out i))
                {
                    lstBuf.Add((byte)i);
                    n = "";
                }
            }

            return lstBuf;
        }

        private static void GetItemsAtCmdArp(int sin, List<MmlDatum> lstBuf, ref enmMMLType tp, ref List<object> tpArgs, ref enmMMLType defTp, ref List<object> defArgs, string vals)
        {
            string n = "";
            //string h = "";
            //int hc = -1;
            int i = 0;
            MmlDatum dat = null;
            char instType = 'n';
            List<int> ivalue = new List<int>();
            string yCMD = "";

            int p;
            for (p = 0; p < vals.Length; p++)
            {
                char c = vals[p];

                //if (hc > -1)
                //{
                //    if ((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F'))
                //    {
                //        h += c;
                //        hc++;
                //        continue;
                //    }
                //    else
                //    {
                //        i = int.Parse(h, System.Globalization.NumberStyles.HexNumber);
                //        h = "";
                //        hc = -1;
                //        ivalue.Add(i);
                //    }
                //}

                if (c == '|' || c == '/')
                {
                    n = "";
                    dat = new MmlDatum();
                    dat.type = enmMMLType.LoopPoint;
                    dat.dat = c == '|' ? 0 : 1;
                    lstBuf.Add(dat);
                    continue;
                }

                if (c == 'E')
                {
                    n = "";
                    dat = new MmlDatum();
                    dat.type = enmMMLType.LoopPoint;
                    dat.dat = 2;
                    lstBuf.Add(dat);
                    continue;
                }

                if (c == '#')
                {
                    n = "";
                    tp = enmMMLType.LengthClock;
                    continue;
                }

                if (c == '@')
                {
                    tp = enmMMLType.Instrument;
                    char nc = p + 1 < vals.Length ? vals[p + 1] : '\0';

                    if (nc == 'I' || nc == 'E' || nc == 'N' || nc == 'R' || nc == 'A' || nc == 'W')
                    {
                        instType = nc;
                        //ptr = p + 1;
                        p++;
                    }
                    n = "";
                    if (sin == 0 && lstBuf.Count == 1)//デフォルトコマンドの位置か
                    {
                        defTp = tp;

                        dat = new MmlDatum();
                        dat.type = enmMMLType.Instrument;//.DefaultCommand;
                        dat.args = new List<object>(new object[] { defTp });
                        if (instType == 'n')
                        {
                        }
                        else
                        {
                            dat.args = new List<object>(new object[] { instType });
                            defArgs = new List<object>(new object[] { instType });
                        }
                        lstBuf.Add(dat);
                    }
                    tpArgs = dat.args;
                    continue;
                }

                if (c == 'p')
                {
                    tp = enmMMLType.Pan;
                    n = "";
                    if (sin == 0 && lstBuf.Count == 1)//デフォルトコマンドの位置か
                    {
                        defTp = tp;

                        dat = new MmlDatum();
                        dat.type = enmMMLType.Pan;//.DefaultCommand;
                        dat.args = new List<object>(new object[] { defTp });
                        lstBuf.Add(dat);
                    }
                    continue;
                }

                if (c == 'y')
                {
                    tp = enmMMLType.Y;
                    n = "";
                    if (sin == 0 && lstBuf.Count == 1)//デフォルトコマンドの位置か
                    {
                        defTp = tp;

                        dat = new MmlDatum();
                        dat.type = enmMMLType.Y;//.DefaultCommand;
                        dat.args = new List<object>(new object[] { defTp });
                        lstBuf.Add(dat);
                    }
                    continue;
                }

                if (c == 'P')
                {
                    tp = enmMMLType.NoiseToneMixer;
                    n = "";
                    if (sin == 0 && lstBuf.Count == 1)//デフォルトコマンドの位置か
                    {
                        defTp = tp;

                        dat = new MmlDatum();
                        dat.type = enmMMLType.NoiseToneMixer;//.DefaultCommand;
                        dat.args = new List<object>(new object[] { defTp });
                        lstBuf.Add(dat);
                    }
                    continue;
                }

                if (c == 'w')
                {
                    char nc = (p + 1) < vals.Length ? vals[p + 1] : '\0';
                    if (nc == 'f')
                    {
                        //ptr = p + 1;
                        p++;
                        tp = enmMMLType.DCSGCh3Freq;
                    }
                    else
                        tp = enmMMLType.Noise;

                    n = "";
                    if (sin == 0 && lstBuf.Count == 1)//デフォルトコマンドの位置か
                    {
                        defTp = tp;

                        dat = new MmlDatum();
                        dat.type = tp;//.DefaultCommand;
                        dat.args = new List<object>(new object[] { defTp });
                        lstBuf.Add(dat);
                    }
                    continue;
                }

                if (c == '$')
                {
                    n = GetHexNumericStrAtCmdArp(vals, ref p);
                    if (int.TryParse(n, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out i))
                    {
                        p--;
                        ivalue.Add(i);
                        n = "";
                        continue;
                    }
                }
                else if ((c >= '0' && c <= '9') || c == '-' || c == '+')
                {
                    n = GetNumericStrAtCmdArp(vals, ref p);
                    if (int.TryParse(n, out i))
                    {
                        p--;
                        ivalue.Add(i);
                        n = "";
                        continue;
                    }
                }

                if (!string.IsNullOrEmpty(n))
                {
                    if (int.TryParse(n, out i))
                        ivalue.Add(i);
                    n = "";
                }
                else
                {
                    if (tp == enmMMLType.Y)
                    {
                        char nc = c;
                        while (nc != ',' && nc != '\0' && nc != ' ' && nc != '\t')
                        {
                            yCMD += nc;
                            p++;
                            nc = p < vals.Length ? vals[p] : '\0';
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(n))
            {
                if (int.TryParse(n, out i))
                    ivalue.Add(i);
                n = "";
            }

            if (ivalue.Count > 0)
            {
                dat = new MmlDatum();
                dat.type = tp;
                dat.dat = ivalue[0];

                if (tp == enmMMLType.Instrument)
                {
                    dat.args = new List<object>();
                    if (tpArgs == null) dat.args.Add(instType);
                    else dat.args.AddRange(tpArgs);
                    instType = 'n';
                    foreach (int j in ivalue)
                    {
                        dat.args.Add(j);
                    }
                }

                if (tp == enmMMLType.Pan || tp == enmMMLType.NoiseToneMixer || tp == enmMMLType.Noise || tp == enmMMLType.DCSGCh3Freq)
                {
                    dat.args = new List<object>();
                    foreach (int j in ivalue)
                    {
                        dat.args.Add(j);
                    }
                }
                if (tp == enmMMLType.Y)
                {
                    dat.args = new List<object>();
                    if (yCMD != "") dat.args.Add(yCMD);
                    foreach (int j in ivalue)
                    {
                        dat.args.Add(j);
                    }
                    yCMD = "";
                }

                lstBuf.Add(dat);
                tp = defTp;
                tpArgs = defArgs;

                ivalue.Clear();
            }

        }

        private static object GetYCommandItemsAtCmdArp(string vals)
        {
            string n;
            int i;
            object ret = null;
            int p = 0;
            vals = vals.Trim().ToUpper();
            char c = vals[0];

            if (c == '$')
            {
                n = GetHexNumericStrAtCmdArp(vals, ref p);
                if (int.TryParse(n, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out i)) ret = i;
            }
            else if ((c >= '0' && c <= '9') || c == '-' || c == '+')
            {
                n = GetNumericStrAtCmdArp(vals, ref p);
                if (int.TryParse(n, out i)) ret = i;
            }
            else
            {
                string[] cmds = new string[] {
                    "DTML",
                    "TL",
                    "KSAR",
                    "AMDR",
                    "SR",
                    "SLRR",
                    "SSG",
                    "FBAL",
                    "PANFBAL",
                    "PANFLCON",
                    "PMSAMS",
                    "DTML",
                    "DTMUL",
                    "DT1ML",
                    "DT1MUL",
                    "TL",
                    "KSAR",
                    "AMDR",
                    "AMED1R",
                    "DT2SR",
                    "DT2D2R",
                    "SLRR",
                    "D1LRR"
                };
                if (cmds.Contains(vals)) ret = vals;
            }

            return ret;
        }

        private static string GetNumericStrAtCmdArp(string src, ref int ptr)
        {
            string ret = "";
            int stPtr = ptr;
            do
            {
                char c = src[ptr];
                if (!(
                    (c >= '0' && c <= '9')
                    || (ptr == stPtr && (c == '-' || c == '+'))
                    )) break;
                ret += c;
                ptr++;
            } while (ptr<src.Length);

            return ret;
        }

        private static string GetHexNumericStrAtCmdArp(string src, ref int ptr)
        {
            string ret = "";
            int stPtr = ++ptr;
            do
            {
                char c = src[ptr];
                if (!(
                    (c >= '0' && c <= '9')
                    || (c >= 'A' && c <= 'F')
                    || (c >= 'a' && c <= 'f')
                    || (ptr == stPtr && (c == '-' || c == '+'))
                    )) break;
                ret += c;
                ptr++;
            } while (ptr < src.Length);

            return ret;
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
                instToneDoubler.Remove(num);
            instToneDoubler.Add(num, toneDoubler);
            toneDoublerBufCache.Clear();
            toneDoublerCounter = -1;
        }

        private int StoreMidiSysExBuffer(Line line)
        {
            try
            {
                string name = "";
                List<byte> buf = null;
                if (midiSysExCounter == 0)
                {
                    buf = GetNums(Common.CutComment(line.Txt).IndexOf('S') + 1, Common.CutComment(line.Txt), ref name);
                    if (string.IsNullOrEmpty(instrumentName)) instrumentName = name;//音色名
                        midiSysExCounter = buf[0] + 1;
                    if (midiSysEx.ContainsKey(buf[0]))
                        midiSysEx.Remove(buf[0]);
                    midiSysEx.Add(buf[0], new Tuple<string, byte[]>(instrumentName, buf.ToArray()));
                }
                else
                {
                    buf = GetNums(Common.CutComment(line.Txt).IndexOf('@') + 1, Common.CutComment(line.Txt), ref name);
                    if (string.IsNullOrEmpty(instrumentName)) instrumentName = name;//音色名
                    List<byte> ebuf = midiSysEx[midiSysExCounter - 1].Item2.ToList();
                    ebuf.AddRange(buf);
                    midiSysEx.Remove(midiSysExCounter - 1);
                    midiSysEx.Add(midiSysExCounter - 1, new Tuple<string, byte[]>(instrumentName, ebuf.ToArray()));
                }
            }
            catch { }

            return 0;
        }

        private int StoreInstArpBuffer(Line line)
        {
            try
            {
                List<MmlDatum> buf = null;
                if (instArpCounter == 0)
                {
                    string txt = line.Txt.ToUpper();
                    buf = GetNumsIntForArp(Common.CutComment(txt).IndexOf("ARP") + 3, Common.CutComment(txt));
                    instArpCounter = buf[0].dat + 1;
                    if (instArp.ContainsKey(buf[0].dat))
                        instArp.Remove(buf[0].dat);
                    instArp.Add(buf[0].dat, buf.ToArray());
                }
                else
                {
                    string txt = line.Txt.ToUpper();
                    buf = GetNumsIntForArp(Common.CutComment(txt).IndexOf('@') + 1, Common.CutComment(txt));
                    List<MmlDatum> ebuf = instArp[instArpCounter - 1].ToList();
                    ebuf.AddRange(buf);
                    instArp.Remove(instArpCounter - 1);
                    instArp.Add(instArpCounter - 1, ebuf.ToArray());
                }
            }
            catch { }

            return 0;
        }

        private int StoreInstVArpBuffer(Line line)
        {
            try
            {
                List<MmlDatum> buf = null;
                if (instVArpCounter == 0)
                {
                    string txt = line.Txt.ToUpper();
                    buf = GetNumsIntForVArp(Common.CutComment(txt).IndexOf("VAR") + 3, Common.CutComment(txt));
                    instVArpCounter = buf[0].dat + 1;
                    if (instVArp.ContainsKey(buf[0].dat))
                        instVArp.Remove(buf[0].dat);
                    instVArp.Add(buf[0].dat, buf.ToArray());
                }
                else
                {
                    string txt = line.Txt.ToUpper();
                    buf = GetNumsIntForVArp(Common.CutComment(txt).IndexOf('@') + 1, Common.CutComment(txt));
                    List<MmlDatum> ebuf = instVArp[instVArpCounter - 1].ToList();
                    ebuf.AddRange(buf);
                    instVArp.Remove(instVArpCounter - 1);
                    instVArp.Add(instVArpCounter - 1, ebuf.ToArray());
                }
            }
            catch { }

            return 0;
        }

        private int StoreTLOFSBuffer(Line line)
        {
            try
            {
                List<byte> buf = null;
                if (instTLOFSCounter == 0)
                {
                    string txt = line.Txt.ToUpper();
                    buf = GetNumsIntForTLOFS(Common.CutComment(txt).IndexOf("TLOFS") + 5, Common.CutComment(txt));
                    instTLOFSCounter = buf[0] + 1;
                    if (instTLOFS.ContainsKey(buf[0]))
                        instTLOFS.Remove(buf[0]);
                    instTLOFS.Add(buf[0], buf.ToArray());
                }
                else
                {
                    string txt = line.Txt.ToUpper();
                    buf = GetNumsIntForTLOFS(Common.CutComment(txt).IndexOf('@') + 1, Common.CutComment(txt));
                    List<byte> ebuf = new List<byte>(instTLOFS[instTLOFSCounter - 1]);
                    ebuf.AddRange(buf);
                    instTLOFS.Remove(instTLOFSCounter - 1);
                    instTLOFS.Add(instTLOFSCounter - 1, ebuf.ToArray());
                }
            }
            catch { }

            return 0;
        }

        private int StoreInstCommandArpBuffer(Line line)
        {
            try
            {
                List<MmlDatum> buf = null;
                if (instCommandArpCounter == 0)
                {
                    string txt = line.Txt;
                    buf = GetNumsIntForCommandArp(Common.CutComment(txt).IndexOf("CAR") + 3, Common.CutComment(txt));
                    instCommandArpCounter = buf[0].dat + 1;
                    if (instCommandArp.ContainsKey(buf[0].dat))
                        instCommandArp.Remove(buf[0].dat);
                    instCommandArp.Add(buf[0].dat, buf.ToArray());
                }
                else
                {
                    string txt = line.Txt;
                    buf = GetNumsIntForCommandArp(
                        Common.CutComment(txt).IndexOf('@') + 1
                        , Common.CutComment(txt)
                        , 1
                        , instCommandArp[instCommandArpCounter - 1].Length > 1 ? instCommandArp[instCommandArpCounter - 1][1].type : enmMMLType.Instrument
                        , instCommandArp[instCommandArpCounter - 1].Length > 1 ? instCommandArp[instCommandArpCounter - 1][1].args : null);
                    List <MmlDatum> ebuf = instCommandArp[instCommandArpCounter - 1].ToList();
                    ebuf.AddRange(buf);
                    instCommandArp.Remove(instCommandArpCounter - 1);
                    instCommandArp.Add(instCommandArpCounter - 1, ebuf.ToArray());
                }
            }
            catch { }

            return 0;
        }

        private int StoreInstModulationBuffer(Line line)
        {
            try
            {
                string name = "";
                instModCounter = GetNums(fdsModulationInstrumentBufCache, instModCounter, Common.CutComment(line.Txt).Substring(1).TrimStart(), ref name, line);
                if (string.IsNullOrEmpty(instrumentName)) instrumentName = name;//音色名

                if (instModCounter == Const.FDS_MOD_INSTRUMENT_SIZE)
                    SetInstModulation();// SetWfInstrument();
            }
            catch
            {
                msgBox.setErrMsg(msg.get("E01013"), line.Lp);
            }


            return 0;
        }

        private void SetMidiSysEx()
        {
            midiSysExCounter = -1;
        }

        private void SetInstArp()
        {
            instArpCounter = -1;
        }

        private void SetInstVArp()
        {
            instVArpCounter = -1;
        }

        private void SetInstTLOFS()
        {
            instTLOFSCounter = -1;
        }

        private void SetInstCommandArp()
        {
            instCommandArpCounter = -1;
        }

        private void SetInstModulation()
        {
            try
            {
                if (instModCounter == Const.FDS_MOD_INSTRUMENT_SIZE)
                {
                    if (instFDSMod.ContainsKey(fdsModulationInstrumentBufCache[0]))
                        instFDSMod.Remove(fdsModulationInstrumentBufCache[0]);

                    instFDSMod.Add(fdsModulationInstrumentBufCache[0], new Tuple<string, byte[]>(instrumentName, fdsModulationInstrumentBufCache));

                }
            }
            catch
            {
            }

            instModCounter = -1;

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

        //public int jumpCommandCounter = 0;
        //public bool useJumpCommand = false;
        //public bool useSkipPlayCommand = false;

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
        public long dummyCmdLoopOffsetAddress = 0;
        private double bSample;

        public LinePos linePos { get; internal set; }
        public bool isRealTimeMode { get; set; }
        public int ChipCommandSize { get; set; }
        public string wrkPath { get; internal set; }
        public long jumpPointClock { get; set; } = -1;
        public List<Tuple<enmChipType, int>> jumpChannels = new List<Tuple<enmChipType, int>>();
        public bool compileEnd=false;

        public outDatum[] Vgm_getByteData(Dictionary<string, List<MML>> mmlData)
        {
            //スキップ再生の指定がある場合は、キャレット位置まで(SkipPlayコマンドが来るまで)ウェイトの発行をしない。
            //if (doSkip)
            //{
            //useSkipPlayCommand = true;
            //}
            jumpPointClock = -1;
            jumpChannels = new List<Tuple<enmChipType, int>>();
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

            } while (endChannel < totalChannel && !compileEnd);

            //残カット
            //if (loopClock != -1 && waitCounter > 0 && waitCounter != long.MaxValue)
            //{
            //    lClock -= waitCounter;
            //    dSample -= (long)(info.samplesPerClock * waitCounter);
            //}
            if (loopClock == -1)
                AllKeyOffEnv();

            //ページのClockCounterを比較し最大のものをパートのclockCounterとする
            CompClockCounter();

            log.Write("フッター情報の作成");
            MakeFooter();

            return dat.ToArray();
        }

        public void CompClockCounter()
        {
            foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in chips)
            {
                foreach (ClsChip chip in kvp.Value)
                {
                    if (chip == null) continue;

                    foreach (partWork pw in chip.lstPartWork)
                    {
                        pw.clockCounter = 0;
                        foreach (partPage pg in pw.pg)
                        {
                            if (pg.clockCounter > pw.clockCounter) pw.clockCounter = pg.clockCounter;
                        }
                    }
                }
            }

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
                        //partWork pw = chip.lstPartWork[
                        //    chip.ReversePartWork
                        //    ? (chip.lstPartWork.Count - 1 - i)
                        //    : i
                        //    ];
                        partWork pw = chip.lstPartWork[i];
                        foreach (partPage page in pw.pg)
                        {
                            if (page.envIndex != -1 || page.varpIndex != -1)
                                chip.SetKeyOff(page, null);
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
                        int pc = 0;
                        foreach (partPage pg in pw.pg)
                        {
                            if (!pg.chip.use) pc++;// endChannel++;
                            else if (pg.dataEnd && pg.waitCounter < 1) pc++;
                            else if (loopOffset != -1 && pg.dataEnd && pg.envIndex == 3) pc++;
                        }
                        if (pc == pw.pg.Count) endChannel++;
                    }
                }
            }

            return endChannel;
        }

        public void DecAllPartWaitCounter(long waitCounter)
        {
            if (waitCounter == long.MaxValue) return;

            // waitcounterを減らす

            foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in chips)
            {
                foreach (ClsChip chip in kvp.Value)
                {
                    if (chip == null) continue;

                    foreach (partWork pw in chip.lstPartWork)
                    {
                        foreach (partPage pg in pw.pg)
                        {
                            if (pg.waitKeyOnCounter > 0) pg.waitKeyOnCounter -= waitCounter;

                            if (pg.waitRR15Counter > 0)
                                pg.waitRR15Counter -= waitCounter;

                            if (pg.waitCounter > 0) pg.waitCounter -= waitCounter;

                            if (pg.bendWaitCounter > 0) pg.bendWaitCounter -= waitCounter;

                            for (int lfo = 0; lfo < 4; lfo++)
                            {
                                if (!pg.lfo[lfo].sw) continue;
                                if (pg.lfo[lfo].waitCounter == -1) continue;

                                if (pg.lfo[lfo].waitCounter > 0)
                                {
                                    pg.lfo[lfo].waitCounter -= waitCounter;
                                    if (pg.lfo[lfo].waitCounter < 0) pg.lfo[lfo].waitCounter = 0;
                                }
                            }

                            if (pg.pcmWaitKeyOnCounter > 0)
                                pg.pcmWaitKeyOnCounter -= waitCounter;

                            if (pg.envelopeMode && pg.envIndex != -1)
                                pg.envCounter -= (int)waitCounter;

                            if (pg.arpeggioMode && pg.arpIndex != -1)
                                pg.arpCounter -= (int)waitCounter;

                            if (pg.varpeggioMode && pg.varpIndex != -1)
                                pg.varpCounter -= (int)waitCounter;

                            foreach (CommandArpeggio ca in pg.commandArpeggio.Values)
                            {
                                if (!ca.Sw) continue;
                                if (ca.Ptr == -1) continue;
                                ca.WaitCounter -= (int)waitCounter;
                            }

                            if (pg.keyOnDelay.sw)
                            {
                                for (int i = 0; i < 4; i++)
                                {
                                    if (pg.keyOnDelay.delayWrk[i] <= 0) continue;
                                    pg.keyOnDelay.delayWrk[i] -= (int)waitCounter;
                                }
                            }

                            if (pg.noteOns == null) continue;
                            for (int i = 0; i < pg.noteOns.Length; i++)
                            {
                                if (pg.noteOns[i] == null) continue;
                                if (pg.noteOns[i].length < 1) continue;
                                pg.noteOns[i].length -= waitCounter;
                            }
                        }
                    }
                }
            }

            // wait発行

            if (!doJumping)
            {
                if (info.format == enmFormat.XGM2)
                {
                    lClock += waitCounter;
                    bSample += info.samplesPerClock * waitCounter;
                    OutWaitNSamples((long)bSample);
                    dSample += (long)bSample;
                    bSample -= (long)bSample;
                }
                else
                {
                    lClock += waitCounter;
                    dSample += (long)(info.samplesPerClock * waitCounter);
                    bSample += info.samplesPerClock * waitCounter - (double)(long)(info.samplesPerClock * waitCounter);
                    dSample += (long)bSample;
                    bSample -= (long)bSample;

                    bool flg = false;
                    if (ym2612 != null && ym2612[0] != null)
                    {
                        if (ym2612[0].lstPartWork[5].cpg.pcmWaitKeyOnCounter > 0) flg = true;
                    }
                    if (flg) OutWaitNSamplesWithPCMSending(ym2612[0].lstPartWork[5], waitCounter);
                    else OutWaitNSamples((long)(info.samplesPerClock * waitCounter));
                }
            }
            else
            {
                if (info.format == enmFormat.XGM2)
                {
                    bSample += info.samplesPerClock * waitCounter;
                    bSample -= (long)bSample;
                }
                else
                {
                    bSample += info.samplesPerClock * waitCounter - (double)(long)(info.samplesPerClock * waitCounter);
                    bSample -= (long)bSample;
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

                        foreach (partPage page in cpw.pg)
                        {
                            if (!page.chip.use) continue;

                            //note
                            if (page.waitKeyOnCounter > 0)
                                waitCounter = Math.Min(waitCounter, page.waitKeyOnCounter);
                            if (page.waitRR15Counter > 0)
                                waitCounter = Math.Min(waitCounter, page.waitRR15Counter);
                            if (page.waitCounter > 0)
                                waitCounter = Math.Min(waitCounter, page.waitCounter);

                            //bend
                            if (page.bendWaitCounter != -1)
                                waitCounter = Math.Min(waitCounter, page.bendWaitCounter);

                            //lfoとenvelopeは音長によるウエイトカウントが存在する場合のみ対象にする。(さもないと、曲のループ直前の効果を出せない)
                            if (waitCounter > 0)
                            {
                                //if (!cpw.ppg[pw.cpgNum].dataEnd)
                                {
                                    //lfo
                                    for (int lfo = 0; lfo < 4; lfo++)
                                    {
                                        if (!page.lfo[lfo].sw) continue;
                                        if (page.lfo[lfo].waitCounter == -1) continue;

                                        waitCounter = Math.Min(waitCounter, page.lfo[lfo].waitCounter);
                                    }

                                    if (page.keyOnDelay.sw)
                                    {
                                        waitCounter = Math.Min(waitCounter, 1);
                                    }

                                    //envelope
                                    if (page.envelopeMode && page.envIndex != -1)
                                        waitCounter = Math.Min(waitCounter, page.envCounter);

                                    if (page.arpeggioMode && page.arpIndex != -1)
                                        waitCounter = Math.Min(waitCounter, page.arpCounter);

                                    if (page.varpeggioMode && page.varpIndex != -1)
                                        waitCounter = Math.Min(waitCounter, page.varpCounter);

                                    foreach (CommandArpeggio ca in page.commandArpeggio.Values)
                                    {
                                        if (!ca.Sw) continue;
                                        if (ca.Ptr == -1) continue;

                                        waitCounter = Math.Min(waitCounter, ca.WaitCounter);
                                    }
                                }
                            }

                            //pcm
                            if (page.pcmWaitKeyOnCounter > 0)
                                waitCounter = Math.Min(waitCounter, page.pcmWaitKeyOnCounter);

                            //MIDINoteOns
                            if (page.noteOns == null) continue;
                            for (int i = 0; i < page.noteOns.Length; i++)
                            {
                                if (page.noteOns[i] == null) continue;
                                if (page.noteOns[i].length < 1) continue;

                                waitCounter = Math.Min(waitCounter, page.noteOns[i].length);
                            }
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
                        //partWork pw = chip.lstPartWork[
                        //    chip.ReversePartWork
                        //    ? (chip.lstPartWork.Count - 1 - i)
                        //    : i
                        //    ];
                        partWork pw = chip.lstPartWork[i];

                        for (int p = 0; p < pw.pg.Count; p++)
                        {
                            partWorkByteData(pw, p);
                            if (compileEnd) return;
                        }
                    }

                    //
                    // 各パートのページ割り込みチェック
                    //
                    for (int i = 0; i < chip.lstPartWork.Count; i++)
                    {
                        //partWork pw = chip.lstPartWork[
                        //    chip.ReversePartWork
                        //    ? (chip.lstPartWork.Count - 1 - i)
                        //    : i
                        //    ];
                        partWork pw = chip.lstPartWork[i];

                        for (int p = 0; p < pw.pg.Count; p++)
                        {
                            chip.CheckInterrupt(pw, pw.pg[p]);
                        }
                    }

                    //if (chip.SupportReversePartWork) chip.ReversePartWork = !chip.ReversePartWork;

                    chip.LoopPage();

                    log.Write("channelを跨ぐコマンド向け処理");
                    chip.MultiChannelCommand(null);

                    chip.LoopPage();

                }
            }
        }



        public void partWorkByteData(partWork pw, int p)
        {
            //アクティブページの更新
            pw.apg = pw.pg[p];
            partPage pg = pw.pg[p];

            //未使用のパートの場合は処理を行わない
            if (!pg.chip.use) return;
            if (pg.mmlData == null && !isRealTimeMode) return;

            log.Write("MD stream pcm sound off");
            if (pg.pcmWaitKeyOnCounter == 0)
                pg.pcmWaitKeyOnCounter = -1;

            log.Write("RR15");
            ProcRR15(pg);

            log.Write("KeyOff");
            if (!isRealTimeMode) ProcKeyOff(pg);

            log.Write("Bend");
            ProcBend(pg);

            log.Write("Lfo");
            ProcLfo(pg);

            log.Write("Envelope");
            ProcEnvelope(pg);

            log.Write("Arpeggio");
            ProcArpeggio(pg);

            log.Write("Volume Arpeggio");
            ProcVArpeggio(pg);

            log.Write("Command Arpeggio");
            ProcCommandArpeggio(pg);

            log.Write("KeyOnDelay");
            ProcKeyOnDelay(pg);

            ProcMidiNoteOff(pg);

            if (!pg.isLayer
                || (pg.isLayer && checkNextCommandNote(pg))
                )
            {
                if (pg.noteCmd != 0) //0のとき(最初期)は正しいfnumが得られない為Fnumとvolを発行しない
                    pg.chip.SetFNum(pg, null);
            }

            if (pg.noteCmd != 0)
                pg.chip.SetVolume(pg, null);

            if (isRealTimeMode) return;

            log.Write("wait消化待ち");
            if (pg.waitCounter > 0)
            {
                return;
            }

            log.Write("データは最後まで実施されたか");
            if (pg.dataEnd)
            {
                return;
            }

            //if(doSkipStop && !useSkipPlayCommand)
            //{
            //pw.ppg[pw.cpgNum].dataEnd = true;
            //}

            log.Write("パートのデータがない場合は何もしないで次へ");
            if (pg.mmlData == null || pg.mmlData.Count < 1)
            {
                pg.dataEnd = true;
                pg.enableInterrupt = true;
                return;
            }

            log.Write("コマンド毎の処理を実施");
            while (pg.waitCounter == 0 && !pg.dataEnd)
            {
                if (pg.mmlPos >= pg.mmlData.Count)
                {
                    pg.dataEnd = true;
                    pg.enableInterrupt = true;
                }
                else
                {
                    MML mml = pg.mmlData[pg.mmlPos];
                    mml.line.Lp.ch = pg.ch;
                    if (info.format == enmFormat.ZGM)
                    {
                        mml.line.Lp.chipIndex = pg.chip.ChipID;
                        mml.line.Lp.chipNumber = pg.chipNumber;
                    }
                    else
                    {
                        mml.line.Lp.chipIndex = 0;// pg.chip.ChipID;
                        mml.line.Lp.chipNumber = pg.chip.ChipID;// pg.chipNumber;
                    }
                    mml.line.Lp.chip = pg.chip.Name;
                    int c = mml.line.Txt.IndexOfAny(new char[] { ' ', '\t' });
                    mml.line.Lp.col = mml.column + c;
                    mml.line.Lp.part = pg.Type.ToString();

                    Commander(pw, pg, mml);
                }
            }

        }

        private bool checkNextCommandNote(partPage pg)
        {
            if (pg == null) return false;
            if (pg.mmlData == null) return false;

            int i = pg.mmlPos;
            while (i < pg.mmlData.Count)
            {
                if (pg.mmlData[i].type == enmMMLType.Note) return true;
                else if (pg.mmlData[i].type == enmMMLType.Rest) return false;
                else if (pg.mmlData[i].type == enmMMLType.RestNoWork) return false;
                i++;
            }

            return false;
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
                    foreach (partWork pw in chip.lstPartWork)
                    {
                        foreach (partPage pg in pw.pg)
                        {
                            if (pg == pw.cpg) OutData(pg.sendData);
                            pg.sendData.Clear();
                        }
                    }
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

            if (loopOffset != -1)
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

            long useAY8910 = 0;
            long useAY8910_S = 0;
            long usePOKEY = 0;
            long usePOKEY_S = 0;
            long useC140 = 0;
            long useC140_S = 0;
            long useC352 = 0;
            long useC352_S = 0;
            long useDMG = 0;
            long useDMG_S = 0;
            long useHuC6280 = 0;
            long useHuC6280_S = 0;
            long useK051649 = 0;
            long useK051649_S = 0;
            long useK053260 = 0;
            long useK053260_S = 0;
            long useK054539 = 0;
            long useK054539_S = 0;
            long useNES = 0;
            long useNES_S = 0;
            long useQSound = 0;
            long useRf5c164 = 0;
            long useRf5c164_S = 0;
            long useSegaPcm = 0;
            long useSegaPcm_S = 0;
            long useSN76489 = 0;
            long useSN76489_S = 0;
            long useY8950 = 0;
            long useY8950_S = 0;
            long useYM2151 = 0;
            long useYM2151_S = 0;
            long useYM2203 = 0;
            long useYM2203_S = 0;
            long useYM2413 = 0;
            long useYM2413_S = 0;
            long useYM2608 = 0;
            long useYM2608_S = 0;
            long useYM2610B = 0;
            long useYM2610B_S = 0;
            long useYM2612 = 0;
            long useYM2612_S = 0;
            long useYM3526 = 0;
            long useYM3526_S = 0;
            long useYM3812 = 0;
            long useYM3812_S = 0;
            long useYMF262 = 0;
            long useYMF262_S = 0;
            long useYMF271 = 0;
            long useYMF271_S = 0;

            for (int i = 0; i < 2; i++)
            {
                if (ym2151 != null && ym2151.Length > i && ym2151[i] != null)
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

                if (ym2612 != null && ym2612.Length > i && ym2612[i] != null)
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

                if (ay8910 != null && ay8910.Length > i && ay8910[i] != null)
                    foreach (partWork pw in ay8910[i].lstPartWork)
                    { useAY8910 += pw.clockCounter; if (ay8910[i].ChipID == 1) useAY8910_S += pw.clockCounter; }

                if (pokey != null && pokey.Length > i && pokey[i] != null)
                    foreach (partWork pw in pokey[i].lstPartWork)
                    { usePOKEY += pw.clockCounter; if (pokey[i].ChipID == 1) usePOKEY_S += pw.clockCounter; }

                if (c140 != null && c140.Length > i && c140[i] != null)
                    foreach (partWork pw in c140[i].lstPartWork)
                    { useC140 += pw.clockCounter; if (c140[i].ChipID == 1) useC140_S += pw.clockCounter; }

                if (c352 != null && c352.Length > i && c352[i] != null)
                    foreach (partWork pw in c352[i].lstPartWork)
                    { useC352 += pw.clockCounter; if (c352[i].ChipID == 1) useC352_S += pw.clockCounter; }

                if (dmg != null && dmg.Length > i && dmg[i] != null)
                    foreach (partWork pw in dmg[i].lstPartWork)
                    { useDMG += pw.clockCounter; if (dmg[i].ChipID == 1) useDMG_S += pw.clockCounter; }

                if (nes != null && nes.Length > i && nes[i] != null)
                    foreach (partWork pw in nes[i].lstPartWork)
                    { useNES += pw.clockCounter; if (nes[i].ChipID == 1) useNES_S += pw.clockCounter; }

                if (ym2413 != null && ym2413.Length > i && ym2413[i] != null)
                    foreach (partWork pw in ym2413[i].lstPartWork)
                    { useYM2413 += pw.clockCounter; if (ym2413[i].ChipID == 1) useYM2413_S += pw.clockCounter; }

                if (ym3526 != null && ym3526.Length > i && ym3526[i] != null)
                    foreach (partWork pw in ym3526[i].lstPartWork)
                    { useYM3526 += pw.clockCounter; if (ym3526[i].ChipID == 1) useYM3526_S += pw.clockCounter; }

                if (y8950 != null && y8950.Length > i && y8950[i] != null)
                    foreach (partWork pw in y8950[i].lstPartWork)
                    { useY8950 += pw.clockCounter; if (y8950[i].ChipID == 1) useY8950_S += pw.clockCounter; }

                if (ym3812 != null && ym3812.Length > i && ym3812[i] != null)
                    foreach (partWork pw in ym3812[i].lstPartWork)
                    { useYM3812 += pw.clockCounter; if (ym3812[i].ChipID == 1) useYM3812_S += pw.clockCounter; }

                if (ymf262 != null && ymf262.Length > i && ymf262[i] != null)
                    foreach (partWork pw in ymf262[i].lstPartWork)
                    { useYMF262 += pw.clockCounter; if (ymf262[i].ChipID == 1) useYMF262_S += pw.clockCounter; }

                if (ymf271 != null && ymf271.Length > i && ymf271[i] != null)
                    foreach (partWork pw in ymf271[i].lstPartWork)
                    { useYMF271 += pw.clockCounter; if (ymf271[i].ChipID == 1) useYMF271_S += pw.clockCounter; }

                if (k051649 != null && k051649.Length > i && k051649[i] != null)
                    foreach (partWork pw in k051649[i].lstPartWork)
                    { useK051649 += pw.clockCounter; if (k051649[i].ChipID == 1) useK051649_S += pw.clockCounter; }

                if (qsound != null && qsound.Length > i && qsound[i] != null)
                    foreach (partWork pw in qsound[i].lstPartWork)
                    { useQSound += pw.clockCounter; }

                if (k053260 != null && k053260.Length > i && k053260[i] != null)
                    foreach (partWork pw in k053260[i].lstPartWork)
                    { useK053260 += pw.clockCounter; if (k053260[i].ChipID == 1) useK053260_S += pw.clockCounter; }

                if (k054539 != null && k054539.Length > i && k054539[i] != null)
                    foreach (partWork pw in k054539[i].lstPartWork)
                    { useK054539 += pw.clockCounter; if (k054539[i].ChipID == 1) useK054539_S += pw.clockCounter; }
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
            if (info.Version >= 1.71f && useC352 != 0)
            {
                C352 c = c352[0] != null ? c352[0] : c352[1];
                Common.SetLE32(dat, 0xdc, (uint)c.Frequency | (uint)(useC352_S == 0 ? 0 : 0x40000000));
                if (c != null)
                    dat[0xd6] = new outDatum(enmMMLType.unknown, null, null, (byte)(c.C352Divider/4));
            }
            if (info.Version >= 1.51f && useAY8910 != 0)
            {
                AY8910 a = ay8910[0] != null ? ay8910[0] : ay8910[1];
                Common.SetLE32(dat, 0x74, (uint)a.Frequency | (uint)(useAY8910_S == 0 ? 0 : 0x40000000));
                dat[0x78] = new outDatum(enmMMLType.unknown, null, null, a.ChipType);
                dat[0x79] = new outDatum(enmMMLType.unknown, null, null, a.Flags);
                dat[0x7a] = new outDatum(enmMMLType.unknown, null, null, 0);
                dat[0x7b] = new outDatum(enmMMLType.unknown, null, null, 0);
            }
            if (info.Version >= 1.61f && usePOKEY != 0)
            {
                Pokey a = pokey[0] != null ? pokey[0] : pokey[1];
                Common.SetLE32(dat, 0xb0, (uint)a.Frequency | (uint)(usePOKEY_S == 0 ? 0 : 0x40000000));
            }
            if (info.Version >= 1.61f && useDMG != 0)
            {
                DMG c = dmg[0] != null ? dmg[0] : dmg[1];
                Common.SetLE32(dat, 0x80, (uint)c.Frequency | (uint)(useDMG_S == 0 ? 0 : 0x40000000));
            }
            if (info.Version >= 1.61f && useNES != 0)
            {
                NES c = nes[0] != null ? nes[0] : nes[1];
                Common.SetLE32(dat, 0x84, (uint)c.Frequency | (uint)(useNES_S == 0 ? 0 : 0x40000000));
                dat[0x87].val |= (byte)(c.EnableFDS ? 0x80 : 0x00);
            }
            if (info.Version >= 1.00f && useYM2413 != 0)
            {
                YM2413 y = ym2413[0] != null ? ym2413[0] : ym2413[1];
                Common.SetLE32(dat, 0x10, (uint)y.Frequency | (uint)(useYM2413_S == 0 ? 0 : 0x40000000));
            }
            if (info.Version >= 1.51f && useYM3526 != 0)
            {
                YM3526 u = ym3526[0] != null ? ym3526[0] : ym3526[1];
                Common.SetLE32(dat, 0x54, (uint)u.Frequency | (uint)(useYM3526_S == 0 ? 0 : 0x40000000));
            }
            if (info.Version >= 1.51f && useY8950 != 0)
            {
                Y8950 u = y8950[0] != null ? y8950[0] : y8950[1];
                Common.SetLE32(dat, 0x58, (uint)u.Frequency | (uint)(useY8950_S == 0 ? 0 : 0x40000000));
            }
            if (info.Version >= 1.51f && useYM3812 != 0)
            {
                YM3812 u = ym3812[0] != null ? ym3812[0] : ym3812[1];
                Common.SetLE32(dat, 0x50, (uint)u.Frequency | (uint)(useYM3812_S == 0 ? 0 : 0x40000000));
            }
            if (info.Version >= 1.51f && useYMF262 != 0)
            {
                YMF262 u = ymf262[0] != null ? ymf262[0] : ymf262[1];
                Common.SetLE32(dat, 0x5c, (uint)u.Frequency | (uint)(useYMF262_S == 0 ? 0 : 0x40000000));
            }
            if (info.Version >= 1.51f && useYMF271 != 0)
            {
                YMF271 u = ymf271[0] != null ? ymf271[0] : ymf271[1];
                Common.SetLE32(dat, 0x64, (uint)u.Frequency | (uint)(useYMF271_S == 0 ? 0 : 0x40000000));
            }
            if (info.Version >= 1.61f && useK051649 != 0)
            {
                K051649 k = k051649[0] != null ? k051649[0] : k051649[1];
                Common.SetLE32(dat, 0x9c, (uint)k.Frequency | (uint)(useK051649_S == 0 ? 0 : 0x40000000));
            }
            if (info.Version >= 1.61f && useQSound != 0)
                Common.SetLE32(dat, 0xb4, (uint)qsound[0].Frequency);

            if (info.Version >= 1.61f && useK053260 != 0)
                Common.SetLE32(dat, 0xac, (uint)k053260[0].Frequency);

            if (info.Version >= 1.61f && useK054539 != 0)
                Common.SetLE32(dat, 0xa0, (uint)k054539[0].Frequency | (uint)(useK054539_S == 0 ? 0 : 0x40000000));

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
            if ((ym2612x == null || ym2612x[0] == null)
                && (sn76489 == null || sn76489[0] == null)
                ) return null;

            //if (doSkip)
            //{
            //useSkipPlayCommand = true;
            //}
            jumpPointClock = -1;
            jumpChannels = new List<Tuple<enmChipType, int>>();

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
                        foreach (partWork pw in chip.lstPartWork) pw.apg.chip.use = false;
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
                        if (compileEnd) goto lblexit;
                        //
                        // 各パートのページ割り込みチェック
                        //
                        for (int i = 0; i < chip.lstPartWork.Count; i++)
                        {
                            //partWork pw = chip.lstPartWork[
                            //    chip.ReversePartWork
                            //    ? (chip.lstPartWork.Count - 1 - i)
                            //    : i
                            //    ];
                            partWork pw = chip.lstPartWork[i];

                            for (int p = 0; p < pw.pg.Count; p++)
                            {
                                chip.CheckInterrupt(pw, pw.pg[p]);
                            }
                        }

                        chip.LoopPage();

                        log.Write("channelを跨ぐコマンド向け処理");
                        chip.MultiChannelCommand(null);

                        chip.LoopPage();

                    }
                }
            lblexit:;
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
                            if (!pw.apg.chip.use)
                                endChannel++;
                            else if (pw.apg.dataEnd)
                            {
                                if (pw.apg.waitCounter < 1)
                                    endChannel++;
                                else if (loopOffset != -1)
                                {
                                    if ((pw.apg.envIndex == 3 || pw.apg.envIndex == -1))
                                        endChannel++;
                                    //else if (loopOffset != -1 && pw.ppg[pw.cpgNum].dataEnd) endChannel++;
                                }
                            }
                        }
                    }
                }

                log.Write("全パートのwaitcounterを減らす");
                if (waitCounter != long.MaxValue && endChannel < totalChannel)
                    Xgm_procWait(waitCounter);

            } while (endChannel < totalChannel && !compileEnd);//全てのチャンネルが終了していない場合はループする
            //if (loopClock != -1)
            //{
            //    if (waitCounter > 0)
            //    {
            //        lClock -= waitCounter;
            //        dSample -= (long)(info.samplesPerClock * waitCounter);
            //    }
            //}
            if (loopClock == -1)
                AllKeyOffEnv();

            //ページのClockCounterを比較し最大のものをパートのclockCounterとする
            CompClockCounter();

            log.Write("VGMデータをXGMへコンバート");
            dat = ConvertVGMtoXGM(dat);

            log.Write("フッター情報の作成");
            Xgm_makeFooter();

            return dat.ToArray();
        }

        private void Xgm_makeHeader()
        {
            if ((ym2612x == null || ym2612x[0] == null) && (sn76489 == null || sn76489[0] == null)) return;

            //Header
            foreach (byte b in Const.xhDat)
            {
                xdat.Add(new outDatum(enmMMLType.unknown, null, null, b));
            }

            //FM音源を初期化

            if (ym2612x != null && ym2612x[0] != null)
            {
                ym2612x[0].OutOPNSetHardLfo(null, ym2612x[0].lstPartWork[0].cpg, false, 0);
                ym2612x[0].OutOPNSetCh3SpecialMode(null, ym2612x[0].lstPartWork[0].cpg, false);
                ym2612x[0].OutSetCh6PCMMode(null, ym2612x[0].lstPartWork[0].cpg, false);
                ym2612x[0].OutFmAllKeyOff();

                foreach (partWork pw in ym2612x[0].lstPartWork)
                {
                    foreach (partPage page in pw.pg)
                    {
                        if (page.ch == 0)
                        {
                            page.hardLfoSw = false;
                            page.hardLfoNum = 0;
                            ym2612x[0].OutOPNSetHardLfo(null, page, pw.apg.hardLfoSw, pw.apg.hardLfoNum);
                        }

                        if (page.ch < 6)
                        {
                            page.pan = 3;
                            page.ams = 0;
                            page.fms = 0;
                            if (!page.dataEnd) ym2612x[0].OutOPNSetPanAMSPMS(null, page, 3, 0, 0);
                        }
                    }
                }
            }
        }

        private void Xgm_makeFooter()
        {

            if (pcmCache == null)
            {
                //$0004               Sample id table
                uint ptr = 0;
                int n = 4;
                foreach (Tuple<string, clsPcm> p in instPCM.Values)
                {
                    if (p.Item2.chip != enmChipType.YM2612X) continue;

                    uint stAdr = ptr;
                    uint size = (uint)p.Item2.size;
                    //if (size > (uint)p.xgmMaxSampleCount + 1)
                    //{
                    //size = (uint)p.xgmMaxSampleCount + 1;
                    //size = (uint)((size & 0xffff00) + (size % 0x100 != 0 ? 0x100 : 0x0));
                    //}
                    p.Item2.size = size;

                    xdat[n + 0] = new outDatum(enmMMLType.unknown, null, null, (byte)((stAdr / 256) & 0xff));
                    xdat[n + 1] = new outDatum(enmMMLType.unknown, null, null, (byte)(((stAdr / 256) & 0xff00) >> 8));
                    xdat[n + 2] = new outDatum(enmMMLType.unknown, null, null, (byte)((size / 256) & 0xff));
                    xdat[n + 3] = new outDatum(enmMMLType.unknown, null, null, (byte)(((size / 256) & 0xff00) >> 8));

                    ptr += size;
                    n += 4;
                }

                //$0100               Sample data block size / 256
                if (ym2612x != null && ym2612x[0] != null && ym2612x[0].pcmDataEasy != null)
                {
                    xdat[0x100] = new outDatum(enmMMLType.unknown, null, null, (byte)(ptr / 256));
                    xdat[0x101] = new outDatum(enmMMLType.unknown, null, null, (byte)((ptr / 256) >> 8));
                }
                else
                {
                    xdat[0x100] = new outDatum(enmMMLType.unknown, null, null, 0);
                    xdat[0x101] = new outDatum(enmMMLType.unknown, null, null, 0);
                }

                //$0103 bit #0: NTSC / PAL information
                xdat[0x103] = new outDatum(enmMMLType.unknown, null, null, (byte)(xdat[0x103].val | (byte)(info.xgmSamplesPerSecond == 50 ? 1 : 0)));

                //$0104               Sample data block
                if (ym2612x != null && ym2612x[0] != null && ym2612x[0].pcmDataEasy != null)
                {
                    foreach (Tuple<string, clsPcm> p in instPCM.Values)
                    {
                        if (p.Item2.chip != enmChipType.YM2612X) continue;

                        for (uint cnt = 0; cnt < p.Item2.size; cnt++)
                        {
                            xdat.Add(new outDatum(enmMMLType.unknown, null, null, ym2612x[0].pcmDataEasy[p.Item2.stAdr + cnt]));
                        }

                    }

                }
            }
            else
            {
                xdat.Clear();
                for (uint cnt = 0; cnt < pcmCache.Length; cnt++)
                {
                    xdat.Add(new outDatum(enmMMLType.unknown, null, null, pcmCache[cnt]));
                }
                //$0103 bit #0: NTSC / PAL information
                xdat[0x103] = new outDatum(enmMMLType.unknown, null, null, (byte)(xdat[0x103].val | (byte)(info.xgmSamplesPerSecond == 50 ? 1 : 0)));
            }

            if (info.usePcmCache && pcmCache == null)
            {
                //pcmのキャッシュがないままここまで来た場合は次回向けにキャッシュを作成する
                try
                {
                    List<byte> lstCache = new List<byte>();
                    for (int cnt = 0; cnt < xdat.Count; cnt++)
                    {
                        lstCache.Add(xdat[cnt].val);
                    }
                    File.WriteAllBytes(fnPcmCache, lstCache.ToArray());
                }
                catch
                {
                    ;//キャッシュ作成失敗
                }
            }

            dummyCmdLoopOffsetAddress += xdat.Count + 4;

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
                foreach (partPage page in pw.pg)
                {
                    log.Write("RR15");
                    ProcRR15(page);

                    log.Write("KeyOff");
                    ProcKeyOff(page);

                    log.Write("Bend");
                    ProcBend(page);

                    log.Write("Lfo");
                    ProcLfo(page);

                    log.Write("Envelope");
                    ProcEnvelope(page);

                    log.Write("Arpeggio");
                    ProcArpeggio(page);

                    log.Write("Volume Arpeggio");
                    ProcVArpeggio(page);

                    log.Write("Command Arpeggio");
                    ProcCommandArpeggio(page);

                    log.Write("Key On Delay");
                    ProcKeyOnDelay(page);

                    if (!page.isLayer
                        || (page.isLayer && checkNextCommandNote(page))
                        )
                    {
                        if (page.noteCmd != 0) //0のとき(最初期)は正しいfnumが得られない為Fnumとvolを発行しない
                            page.chip.SetFNum(page, null);
                    }

                    if (page.noteCmd != 0)
                        page.chip.SetVolume(page, null);

                    log.Write("wait消化待ち");
                    if (page.waitCounter > 0) continue;

                    log.Write("データは最後まで実施されたか");
                    if (page.dataEnd) continue;

                    log.Write("パートのデータがない場合は何もしないで次へ");
                    if (page.mmlData == null || page.mmlData.Count < 1)
                    {
                        page.dataEnd = true;
                        page.enableInterrupt = true;
                        continue;
                    }

                    log.Write("コマンド毎の処理を実施");
                    while (page.waitCounter == 0 && !page.dataEnd)
                    {
                        if (page.mmlPos >= page.mmlData.Count)
                        {
                            page.dataEnd = true;
                            page.enableInterrupt = true;
                        }
                        else
                        {
                            MML mml = page.mmlData[page.mmlPos];
                            mml.line.Lp.ch = page.ch;
                            mml.line.Lp.chipIndex = page.chip.ChipID;
                            mml.line.Lp.chipNumber = page.chipNumber;
                            mml.line.Lp.chip = page.chip.Name;
                            int c = mml.line.Txt.IndexOfAny(new char[] { ' ', '\t' });
                            //c += mml.line.Txt.Substring(c).Length - mml.line.Txt.Substring(c).TrimStart().Length;
                            mml.line.Lp.col = mml.column + c;//-1;
                            mml.line.Lp.part = page.Type.ToString();

                            //lineNumber = pw.getLineNumber();
                            Commander(pw, page, mml);
                            if (compileEnd) 
                                return;
                        }
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
                        foreach (partPage page in pw.pg)
                        {
                            //note
                            if (page.waitKeyOnCounter > 0) cnt = Math.Min(cnt, page.waitKeyOnCounter);
                            else if (page.waitCounter > 0) cnt = Math.Min(cnt, page.waitCounter);
                            if (page.waitRR15Counter > 0) cnt = Math.Min(cnt, page.waitRR15Counter);

                            //bend
                            if (page.bendWaitCounter != -1) cnt = Math.Min(cnt, page.bendWaitCounter);

                            //lfoとenvelopeは音長によるウエイトカウントが存在する場合のみ対象にする。(さもないと、曲のループ直前の効果を出せない)
                            if (cnt < 1) continue;

                            //if (!pw.ppg[pw.cpgNum].dataEnd) //ここを有効にするとデータ読み取り終了後即エンベロープ処理をしなくなってしまう
                            {
                                //lfo
                                for (int lfo = 0; lfo < 4; lfo++)
                                {
                                    if (!page.lfo[lfo].sw) continue;
                                    if (page.lfo[lfo].waitCounter == -1) continue;

                                    cnt = Math.Min(cnt, page.lfo[lfo].waitCounter);
                                }

                                if (page.arpeggioMode && page.arpIndex != -1) 
                                    cnt = Math.Min(cnt, page.arpCounter);

                                if (page.keyOnDelay.sw)
                                    cnt = Math.Min(cnt, 1);

                                //envelope
                                //if (!(page.chip is SN76489)) continue;
                                if (page.envelopeMode && page.envIndex != -1) cnt = Math.Min(cnt, page.envCounter);

                                if (page.arpeggioMode && page.arpIndex != -1)
                                    cnt = Math.Min(cnt, page.arpCounter);

                                if (page.varpeggioMode && page.varpIndex != -1)
                                    cnt = Math.Min(cnt, page.varpCounter);

                                foreach (CommandArpeggio ca in page.commandArpeggio.Values)
                                {
                                    if (!ca.Sw) continue;
                                    if (ca.Ptr == -1) continue;

                                    cnt = Math.Min(cnt, ca.WaitCounter);
                                }
                            }
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
                        foreach (partPage pg in pw.pg)
                        {

                            if (pg.waitKeyOnCounter > 0) pg.waitKeyOnCounter -= cnt;
                            if (pg.waitRR15Counter > 0) pg.waitRR15Counter -= cnt;
                            if (pg.waitCounter > 0) pg.waitCounter -= cnt;
                            if (pg.bendWaitCounter > 0) pg.bendWaitCounter -= cnt;

                            for (int lfo = 0; lfo < 4; lfo++)
                            {
                                if (!pg.lfo[lfo].sw) continue;
                                if (pg.lfo[lfo].waitCounter == -1) continue;

                                if (pg.lfo[lfo].waitCounter > 0)
                                {
                                    pg.lfo[lfo].waitCounter -= cnt;
                                    if (pg.lfo[lfo].waitCounter < 0) pg.lfo[lfo].waitCounter = 0;
                                }
                            }

                            if (pg.arpeggioMode && pg.arpIndex != -1) pg.arpCounter -= (int)cnt;

                            if (pg.varpeggioMode && pg.varpIndex != -1)
                                pg.varpCounter -= (int)cnt;

                            foreach (CommandArpeggio ca in pg.commandArpeggio.Values)
                            {
                                if (!ca.Sw) continue;
                                if (ca.Ptr == -1) continue;
                                ca.WaitCounter -= (int)cnt;
                            }

                            if (pg.keyOnDelay.sw)
                            {
                                for (int i = 0; i < 4; i++)
                                {
                                    if (pg.keyOnDelay.delayWrk[i] <= 0) continue;
                                    pg.keyOnDelay.delayWrk[i] -= (int)cnt;
                                }
                            }

                            if (!(pg.chip is SN76489)) continue;
                            if (pg.envelopeMode && pg.envIndex != -1) pg.envCounter -= (int)cnt;
                        }
                    }
                }
            }


            // wait発行
            if (!doJumping)
            {
                lClock += cnt;
                sampleB += info.samplesPerClock * cnt;
                OutWaitNSamples((long)(sampleB));
                dSample += (long)sampleB;
                sampleB -= (long)sampleB;
            }
            else
            {
                sampleB += info.samplesPerClock * cnt;
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
                            msgBox.setWrnMsg(string.Format(msg.get("E01015"), frameCnt, des.Count - frameDummyCounter - framePtr), new LinePos(null, "-"));
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
                                    psgreg[psgch * 4 + 1 + psgtp] = new outDatum(src[ptr + 1].type, src[ptr + 1].args, src[ptr + 1].linePos, (byte)d2);
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
                        msgBox.setErrMsg(string.Format("Unknown command[{0:X}]", cmd.val), new LinePos(null,"-"));
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


        private void ProcRR15(partPage page)
        {
            if (page.waitRR15Counter != 0)
            {
                return;
            }

            page.chip.SetRR15(page);
            page.waitRR15Counter = -1;
        }

        private void ProcKeyOff(partPage page)
        {
            if (page.waitKeyOnCounter != 0)
            {
                if (page.waitKeyOnCounter > 0)
                {
                    if (page.waitCounter <= 0)
                    {
                        page.beforeTie = page.tie;
                        page.tie = false;
                    }
                }
                return;
            }

            if (!page.tie)
            {
                if (!page.envelopeMode && !page.varpeggioMode)
                {
                    page.chip.SetKeyOff(page, null);
                    page.arpIndex = -1;
                    if (page.varpIndex != -1)
                    {
                        page.varpIndex = 1;
                        page.varpCounter = 0;
                    }
                    foreach (CommandArpeggio ca in page.commandArpeggio.Values)
                    {
                        if (ca.Sync != 0) continue;
                        ca.Ptr = -1;
                    }
                }
                else
                {
                    if (page.envIndex != -1)
                    {
                        page.envIndex = 3;//RR phase
                        page.envCounter = 0;
                        if (page.envelope[(int)eENV.RL] != 0) page.envVolume = page.envelope[(int)eENV.RL];
                    }

                    if (page.varpIndex != -1)
                    {
                        page.varpIndex = 1;//RR phase
                        page.varpCounter = 0;
                    }
                }
            }

            //次回に引き継ぎリセット
            page.beforeTie = page.tie;
            page.tie = false;

            //ゲートタイムカウンターをリセット
            page.waitKeyOnCounter = -1;
        }

        private void ProcBend(partPage page)
        {
            //bend処理
            if (page.bendWaitCounter == 0)
            {
                if (page.bendList.Count > 0)
                {
                    Tuple<int, int> bp = page.bendList.Pop();
                    page.bendFnum = bp.Item1;
                    page.bendWaitCounter = bp.Item2;
                }
                else
                {
                    page.pitchShift = page.bendPitchShift;
                    page.bendWaitCounter = -1;
                    //midi向け
                    page.tieBend = page.bendFnum;
                }
            }
        }

        private void ProcLfo(partPage page)
        {
            //lfo処理
            for (int lfo = 0; lfo < 4; lfo++)
            {
                clsLfo pl = page.lfo[lfo];

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
                    //if ((cpw.ppg[pw.cpgNum].chip is YM2612) || (cpw.ppg[pw.cpgNum].chip is YM2612X))
                    //{
                    //cpw.ppg[pw.cpgNum].fms = pl.param[2];
                    //cpw.ppg[pw.cpgNum].ams = pl.param[3];
                    //((ClsOPN)cpw.ppg[pw.cpgNum].chip).OutOPNSetPanAMSPMS(null, cpw, (int)cpw.ppg[pw.cpgNum].pan.val, cpw.ppg[pw.cpgNum].ams, cpw.ppg[pw.cpgNum].fms);
                    //cpw.ppg[pw.cpgNum].chip.lstPartWork[0].hardLfoSw = true;
                    //cpw.ppg[pw.cpgNum].chip.lstPartWork[0].hardLfoNum = pl.param[1];
                    //((ClsOPN)cpw.ppg[pw.cpgNum].chip).OutOPNSetHardLfo(null, cpw, cpw.ppg[pw.cpgNum].chip.lstPartWork[0].hardLfoSw, cpw.ppg[pw.cpgNum].chip.lstPartWork[0].hardLfoNum);
                    //pl.waitCounter = -1;
                    //}
                    pl.waitCounter = -1;
                    continue;
                }

                int w = 0;
                if (pl.type == eLfoType.Wah) w = 1;
                pl.phase = pl.param[w + 6];

                switch (pl.param[w+4])
                {
                    case 0: //三角
                        pl.value += Math.Abs(pl.param[w + 2]) * pl.direction;
                        pl.waitCounter = pl.param[w + 1];
                        if ((pl.direction > 0 && pl.value >= pl.depth) || (pl.direction < 0 && pl.value <= -pl.depth))
                        {
                            pl.value = pl.depth * pl.direction;
                            pl.direction = -pl.direction;
                            procLfo_updateDepth(pl);
                        }
                        //Console.WriteLine("{0}", pl.value);
                        break;
                    case 1: //のこぎり
                        pl.value += Math.Abs(pl.param[w + 2]) * pl.direction;
                        pl.waitCounter = pl.param[w + 1];
                        if ((pl.direction > 0 && pl.value >= pl.depth) || (pl.direction < 0 && pl.value <= -pl.depth))
                        {
                            pl.value = -pl.depth * pl.direction;
                            procLfo_updateDepth(pl);
                        }
                        break;
                    case 2: //矩形
                        if (pl.direction < 0) pl.value = pl.depth;
                        else pl.value = pl.depthV2;
                        pl.waitCounter = pl.param[w + 1];
                        pl.direction = -pl.direction;
                        if (pl.param[w + 7] != 0)
                        {
                            pl.depthWaitCounter--;
                            if (pl.depthWaitCounter == 0)
                            {
                                pl.depthWaitCounter = pl.param[w + 7];
                                pl.depth += (pl.param[w + 2] >= pl.param[w + 3]) ? pl.param[w + 8] : (-pl.param[w + 8]);
                                pl.depth = Common.CheckRange(pl.depth, 0, 32767);
                                pl.depthV2 += (pl.param[w + 3] >= pl.param[w + 2]) ? pl.param[w + 8] : (-pl.param[w + 8]);
                                pl.depthV2 = Common.CheckRange(pl.depthV2, 0, 32767);
                            }
                        }
                        break;
                    case 3: //ワンショット
                        pl.value += Math.Abs(pl.param[w + 2]) * pl.direction;
                        pl.waitCounter = pl.param[w + 1];

                        if (Math.Abs(pl.value) >= Math.Abs(pl.depth))
                        {
                            pl.value = Math.Abs(pl.depth) * pl.direction;
                            pl.waitCounter = -1;
                        }
                        //Console.WriteLine("{0}", pl.param[2]);
                        break;
                    case 4: //ランダム
                        pl.value = rnd.Next(-pl.depth, pl.depth);
                        pl.waitCounter = pl.param[w + 1];
                        procLfo_updateDepth(pl);
                        break;
                }

            }
        }

        private static void procLfo_updateDepth(clsLfo pl)
        {
            int w = 0;
            if (pl.type == eLfoType.Wah) w = 1;
            if (pl.param[w+7] != 0)
            {
                pl.depthWaitCounter--;
                if (pl.depthWaitCounter == 0)
                {
                    pl.depthWaitCounter = pl.param[w + 7];
                    pl.depth += pl.param[w + 8];
                    pl.depth = Common.CheckRange(pl.depth, 0, 32767);
                }
            }
        }

        private void ProcMidiNoteOff(partPage page)
        {
            if (page.noteOns == null) return;
            for (int i = 0; i < page.noteOns.Length; i++)
            {
                if (page.noteOns[i] == null) continue;
                if (page.noteOns[i].length > 0) continue;
                if (!page.noteOns[i].Keyon) continue;

                if (page.noteOns[i].length != -1) 
                    page.noteOns[i].Keyon = false;
            }
        }

        private void ProcEnvelope(partPage page)
        {
            if (!page.envelopeMode)
            {
                return;
            }

            if (page.envIndex == -1) return;

            int maxValue = page.MaxVolume;

            while (page.envCounter == 0 && page.envIndex != -1)
            {
                switch (page.envIndex)
                {
                    case 0: //AR phase
                        //System.Diagnostics.Debug.Write("eAR");
                        page.envVolume += page.envelope[(int)eENV.AST];
                        //System.Diagnostics.Debug.Write(string.Format(":{0}", pw.ppg[pw.cpgNum].envVolume));
                        if (page.envVolume >= maxValue)
                        {
                            page.envVolume = maxValue;
                            page.envCounter = page.envelope[(int)eENV.DR];
                            page.envIndex++;
                            break;
                        }
                        page.envCounter = page.envelope[(int)eENV.AR];
                        break;
                    case 1: //DR phase
                        //System.Diagnostics.Debug.Write("eDR");
                        page.envVolume -= page.envelope[(int)eENV.DST]; 
                        //System.Diagnostics.Debug.Write(string.Format(":{0}", pw.ppg[pw.cpgNum].envVolume));
                        if (page.envVolume <= page.envelope[(int)eENV.SL])
                        {
                            page.envVolume = page.envelope[(int)eENV.SL];
                            page.envCounter = page.envelope[(int)eENV.SR]; // SR
                            page.envIndex++;
                            break;
                        }
                        page.envCounter = page.envelope[(int)eENV.DR]; // DR
                        break;
                    case 2: //SR phase
                        //System.Diagnostics.Debug.Write("eSR");
                        page.envVolume -= page.envelope[(int)eENV.SST]; // vol -= ST
                        //System.Diagnostics.Debug.Write(string.Format(":{0}", pw.ppg[pw.cpgNum].envVolume));
                        if (page.envVolume <= 0) // vol <= 0
                        {
                            page.envVolume = 0;
                            page.envCounter = 0;
                            page.envIndex = -1;
                            break;
                        }
                        page.envCounter = page.envelope[(int)eENV.SR]; // SR
                        break;
                    case 3: //RR phase
                        //System.Diagnostics.Debug.Write("eRR");
                        page.envVolume -= page.envelope[(int)eENV.RST]; // vol -= ST
                        //System.Diagnostics.Debug.Write(string.Format(":{0}", pw.ppg[pw.cpgNum].envVolume));
                        if (page.envVolume <= 0) // vol <= 0
                        {
                            page.envVolume = 0;
                            page.envCounter = 0;
                            page.envIndex = -1;
                            //page.arpIndex = -1;//アルペジオも止める
                            //foreach (CommandArpeggio ca in page.commandArpeggio.Values) ca.Ptr = -1;
                            break;
                        }
                        page.envCounter = page.envelope[(int)eENV.RR]; // RR
                        break;
                }
            }

            if (page.envIndex == -1)
                page.chip.SetKeyOff(page, null);
        }



        private void ProcArpeggio(partPage page)
        {
            if (!page.arpeggioMode) return;//アルペジオのモードが有効になっていない場合
            if (page.arpIndex == -1) return;//動作不要な場合
            if (page.arpInstrument == -1) return;//アルペジオ番号未設定の場合
            if (!instArp.ContainsKey(page.arpInstrument)) return;//存在しないアルペジオ番号の場合

            while (page.arpCounter == 0 && page.arpIndex != -1)
            {
                switch (page.arpIndex)
                {
                    case 0://KeyON phase
                        //最後まで解析したかな
                        if (page.arpInstrumentPtr >= instArp[page.arpInstrument].Length)
                        {
                            //ループ設定あるかな もしくは無限ループ対策フラグが立ったまま(waitしていない状態)かな
                            if (page.arpLoopPtr < 0 || page.arpInfinite)
                            {
                                //ループが設定されていない場合は動作はここで完了
                                page.arpIndex = -1;
                                page.arpDelta = 0;
                                continue;
                            }
                            //ループポイントセット
                            page.arpInstrumentPtr = page.arpLoopPtr;
                        }

                        //データの読み込み
                        MmlDatum delta = instArp[page.arpInstrument][page.arpInstrumentPtr++];

                        //ループポイント指定かな
                        if (delta.type == enmMMLType.LoopPoint)
                        {
                            if (delta.dat == 2)// e
                            {
                                //動作はここで完了
                                page.arpIndex = -1;
                                //page.arpDelta = 0;
                               continue;
                            }

                            page.arpLoopPtr = page.arpInstrumentPtr;//ループポイントの設定
                            page.arpInfinite = true;
                            if (page.arpLoopPtr >= instArp[page.arpInstrument].Length)
                            {
                                //ループポイント指定あとのデータがない場合は動作はここで完了
                                page.arpIndex = -1;
                                continue;
                            }
                            continue;
                        }
                        else if (delta.type == enmMMLType.LengthClock)
                        {
                            int clock = delta.dat;
                            clock = clock < 1 ? 1 : clock;//0の場合は補正
                            page.arpClock = clock;
                            continue;
                        }
                        else if (delta.type == enmMMLType.Gatetime)
                        {
                            int q = delta.dat;
                            page.arpGatetime = q;
                            continue;
                        }

                        //keyOn のlengthを求める
                        int w = page.arpClock;
                        if (page.arpGatetimePmode)
                            w = page.arpClock * page.arpGatetime / 8;
                        else
                        {
                            w = page.arpClock - page.arpGatetime;
                        }
                        if (w < 1) w = 1;

                        page.arpKeyOnLength = w;//tbd
                        //keyOff のlengthを求める
                        page.arpKeyOffLength = Math.Max(page.arpClock - page.arpKeyOnLength, 0);//念のため0未満にならないように
                        //deltaは前回の値を基準に変化する
                        page.arpDelta += delta.dat;

                        if (!page.arpTieMode)
                        {
                            page.chip.SetKeyOff(page, null);
                            page.chip.SetEnvelopeAtKeyOn(page, null);
                            page.chip.SetVArpeggioAtKeyOn(page, null);
                            if (page.keyOnDelay.sw)
                            {
                                page.keyOnDelay.keyOn = 0;
                                page.keyOnDelay.beforekeyOn = 0xff;
                                for (int i = 0; i < 4; i++)
                                {
                                    page.keyOnDelay.delayWrk[i] = page.keyOnDelay.delay[i];
                                    if (page.keyOnDelay.delayWrk[i] == 0 && page.keyOnDelay.delay[i] != -1)
                                        page.keyOnDelay.keyOn |= (byte)(1 << i);
                                }
                            }
                            page.chip.SetKeyOn(page, null);

                        }

                        page.arpCounter = page.arpKeyOnLength;
                        if (page.arpCounter > 0) page.arpInfinite = false;
                        page.arpIndex++;
                        break;

                    case 1://KeyOff phase

                        if (!page.arpTieMode)
                        {
                            if (!page.envelopeMode && !page.varpeggioMode)
                                page.chip.SetKeyOff(page, null);
                            else
                            {
                                if (!page.envelopeMode) //エンベロープ無しの場合
                                {
                                    if (page.varpeggioMode)//ボリュームアルペジオが有効な場合はリリースに移行する
                                    {
                                        page.varpIndex = 1;//RR phase
                                        page.varpCounter = 0;//RR phase
                                    }
                                    else//エンベロープもボリュームアルペジオも無効な場合はキーオフする
                                        page.chip.SetKeyOff(page, null);
                                }
                                else //エンベロープありの場合
                                {
                                    if (page.varpeggioMode)//ボリュームアルペジオが有効な場合はリリースに移行する
                                    {
                                        page.varpIndex = 1;//RR phase
                                        page.varpCounter = 0;//RR phase
                                    }

                                    if (page.envIndex != -1)//エンベロープ動作中の場合はリリースに移行する
                                    {
                                        page.envIndex = 3;//RR phase
                                        page.envCounter = 0;
                                        if (page.envelope[(int)eENV.RL] != 0) page.envVolume = page.envelope[(int)eENV.RL];
                                    }

                                }

                                //if (page.varpeggioMode)
                                //{
                                //    page.varpIndex = 1;//RR
                                //}

                                //if(page.envelopeMode)
                                //{
                                //    if (page.envIndex != -1)
                                //    {
                                //        page.envIndex = 3;//RR phase
                                //        page.envCounter = 0;
                                //    }
                                //}
                            }
                        }

                        page.arpCounter = page.arpKeyOffLength;
                        if (page.arpCounter > 0) page.arpInfinite = false;
                        page.arpIndex++;
                        break;

                    case 2://次行ってみよう
                        page.arpIndex = 0;
                        break;
                }
            }

            if (page.arpIndex == -1 && page.varpIndex == -1)
                page.chip.SetKeyOff(page, null);
        }

        private void ProcVArpeggio(partPage page)
        {
            if (!page.varpeggioMode) return;//アルペジオのモードが有効になっていない場合
            if (page.varpIndex == -1) return;//動作不要な場合
            if (page.varpInstrument == -1) return;//アルペジオ番号未設定の場合
            if (!instVArp.ContainsKey(page.varpInstrument)) return;//存在しないアルペジオ番号の場合

            while (page.varpCounter == 0 && page.varpIndex != -1)
            {
                switch (page.varpIndex)
                {
                    case 0://KeyON phase
                        //最後まで解析したかな
                        if (page.varpInstrumentPtr >= instVArp[page.varpInstrument].Length)
                        {
                            //ループ設定あるかな
                            if (page.varpLoopPtr < 0 || page.varpInfinite)
                            {
                                //ループが設定されていない場合は動作はここで完了
                                page.varpIndex = -1;
                                page.varpDelta = 0;
                                continue;
                            }
                            //ループポイントセット
                            page.varpInstrumentPtr = page.varpLoopPtr;
                        }

                        //データの読み込み
                        MmlDatum delta = instVArp[page.varpInstrument][page.varpInstrumentPtr++];

                        //ループポイント指定かな
                        if (delta.type == enmMMLType.LoopPoint)
                        {
                            if (delta.dat == 2)// e だったとき
                            {
                                //動作はここで完了
                                page.varpIndex = -1;
                                continue;
                            }

                            if (page.varpLoopPtr == -1)
                            {
                                page.varpLoopPtr = page.varpInstrumentPtr;//ループポイントの設定
                                page.varpInfinite = true;
                                if (page.varpLoopPtr >= instVArp[page.varpInstrument].Length)
                                {
                                    //ループポイント指定あとのデータがない場合は動作はここで完了
                                    page.varpIndex = -1;
                                    continue;
                                }

                                if (delta.dat != 0)
                                    page.varpDelta = 0;
                            }
                            else
                            {
                                //2個目のループポイントの場合は一つ目のループポイントへ戻る
                                //ループポイントセット
                                page.varpInstrumentPtr = page.varpLoopPtr;
                            }
                            continue;
                        }
                        else if (delta.type == enmMMLType.LengthClock)
                        {
                            int clock = delta.dat;
                            clock = clock < 1 ? 1 : clock;//0の場合は補正
                            page.varpClock = clock;
                            continue;
                        }

                        //keyOn のlengthを求める
                        int w = page.varpClock;
                        if (w < 1) w = 1;

                        page.varpKeyOnLength = w;//tbd
                        //deltaは前回の値を基準に変化する
                        page.varpDelta += delta.dat;
                        page.varpCounter = page.varpKeyOnLength;
                        if (page.varpCounter > 0) page.varpInfinite = false;
                        //page.varpIndex++;
                        break;

                    case 1://KeyOff phase
                        //最後のループポイントを探す
                        int lp = -1;
                        for (int p = 0; p < instVArp[page.varpInstrument].Length; p++) 
                        {
                            if (instVArp[page.varpInstrument][p].type != enmMMLType.LoopPoint || instVArp[page.varpInstrument][p].dat == 2) continue;
                            lp = p + 1;
                        }
                        
                        if (lp != page.varpLoopPtr)//既存のループポイントと異なるループポイントが見つかった(2個目のループポイントが見つかった)
                        {
                            if (instVArp[page.varpInstrument][lp - 1].dat != 0)//ループポイントが「/」の場合はリセット
                                page.varpDelta = 0;

                            page.varpInstrumentPtr = lp;
                            page.varpLoopPtr = lp;
                            page.varpInfinite = true;

                            page.varpCounter = 0;
                            page.varpIndex = 0;
                        }
                        else
                        {
                            //一つ目のループポイントしか見つからなかった
                            //ループポイントが無かった

                            page.varpIndex = 0;
                        }

                        break;

                    case 2://次行ってみよう
                        page.varpIndex = 0;
                        break;
                }
            }

        }

        private void ProcCommandArpeggio(partPage page)
        {
            foreach (CommandArpeggio ca in page.commandArpeggio.Values)
            {
                if (!ca.Sw) continue;//アルペジオのモードが有効になっていない場合
                if (ca.Ptr == -1) continue;//動作不要な場合
                if (ca.Num == -1) continue;//アルペジオ番号未設定の場合
                if (!instCommandArp.ContainsKey(ca.Num)) continue;//存在しないアルペジオ番号の場合
                if (ca.WaitCounter > 0) continue;//持続時間中の場合

                while (ca.WaitCounter == 0 && ca.Ptr != -1)
                {
                    //No 定義番号　はスキップ
                    if (ca.Ptr == 0)
                    {
                        ca.Ptr++;
                        continue;
                    }

                    //Cmd デフォルトコマンド 
                    if (ca.Ptr == 1)
                    {
                        ca.DefCmd = instCommandArp[ca.Num][ca.Ptr].type;
                        ca.DefCmdArg= instCommandArp[ca.Num][ca.Ptr].args;
                        ca.Ptr++;
                        continue;
                    }

                    //Sync 
                    if (ca.Ptr == 2)
                    {
                        ca.Sync = instCommandArp[ca.Num][ca.Ptr].dat;
                        ca.Ptr++;
                        continue;
                    }

                    //定義の終端まできたかな？
                    if (ca.Ptr == instCommandArp[ca.Num].Length)
                    {
                        //ループポイントの設定があるなら繰り返し処理続行
                        if (ca.LoopPtr != -1 && !ca.Infinite)
                        {
                            ca.Ptr = ca.LoopPtr;
                            continue;
                        }
                        //今回はここまで。
                        ca.Ptr = -1;
                        continue;
                    }

                    //コマンド一つ取り出す
                    MmlDatum md = instCommandArp[ca.Num][ca.Ptr++];
                    MML mml;
                    switch (md.type)
                    {
                        case enmMMLType.LengthClock:
                            ca.WaitClock = Math.Max(md.dat, 1);
                            continue;

                        case enmMMLType.LoopPoint:
                            if (md.dat == 2)//e
                            {
                                //今回はここまで。
                                ca.Ptr = -1;
                                continue;
                            }
                            ca.LoopPtr = ca.Ptr;
                            if (ca.LoopPtr >= instCommandArp[ca.Num].Length)
                                ca.LoopPtr = -1;
                            if (ca.LoopPtr != -1) ca.Infinite = true;
                            continue;

                        case enmMMLType.Instrument:
                            ca.WaitCounter = ca.WaitClock;
                            if (ca.WaitCounter > 0) ca.Infinite = false;
                            mml = new MML();
                            mml.args = md.args;
                            mml.type = md.type;
                            mml.line = new Line(md.linePos, "");
                            page.chip.CmdInstrument(page, mml);
                            if (page.chip.chipType == enmChipType.HuC6280)
                            {
                                //HuC6280はキーオンディレイ未対応だから不要かな...
                                //if (page.keyOnDelay.sw)
                                //{
                                //    for (int i = 0; i < 4; i++)
                                //    {
                                //        page.keyOnDelay.delayWrk[i] = page.keyOnDelay.delay[i];
                                //        if (page.keyOnDelay.delayWrk[i] == 0 && page.keyOnDelay.delay[i] != -1)
                                //        page.keyOnDelay.keyOn |= (byte)(1 << i);
                                //    }
                                //}
                                page.chip.SetKeyOn(page, mml);
                            }
                            break;

                        case enmMMLType.Pan:
                            ca.WaitCounter = ca.WaitClock;
                            if (ca.WaitCounter > 0) ca.Infinite = false;
                            mml = new MML();
                            mml.args = md.args;
                            mml.type = md.type;
                            mml.line = new Line(md.linePos, "");
                            page.chip.CmdPan(page, mml);
                            break;

                        case enmMMLType.NoiseToneMixer:
                            ca.WaitCounter = ca.WaitClock;
                            if (ca.WaitCounter > 0) ca.Infinite = false;
                            mml = new MML();
                            mml.args = md.args;
                            mml.type = md.type;
                            mml.line = new Line(md.linePos, "");
                            page.chip.CmdNoiseToneMixer(page, mml);
                            break;

                        case enmMMLType.Noise:
                            ca.WaitCounter = ca.WaitClock;
                            if (ca.WaitCounter > 0) ca.Infinite = false;
                            mml = new MML();
                            mml.args = md.args;
                            mml.type = md.type;
                            mml.line = new Line(md.linePos, "");
                            page.chip.CmdNoise(page, mml);
                            break;

                        case enmMMLType.DCSGCh3Freq:
                            ca.WaitCounter = ca.WaitClock;
                            if (ca.WaitCounter > 0) ca.Infinite = false;
                            mml = new MML();
                            mml.args = md.args;
                            mml.type = md.type;
                            mml.line = new Line(md.linePos, "");
                            page.chip.CmdDCSGCh3Freq(page, mml);
                            break;
                        case enmMMLType.Y:
                            ca.WaitCounter = ca.WaitClock;
                            if (ca.WaitCounter > 0) ca.Infinite = false;
                            mml = new MML();
                            mml.args = md.args;
                            mml.type = md.type;
                            mml.line = new Line(md.linePos, "");
                            page.chip.CmdY(page, mml);
                            break;

                        default:
                            ;
                            break;
                    }

                }

                //終了処理
                if (ca.Ptr == -1)
                {
                }
            }

        }

        private void ProcKeyOnDelay(partPage page)
        {
            if (!page.keyOnDelay.sw) return;

            for (int i = 0; i < 4; i++)
            {
                if (page.keyOnDelay.delayWrk[i] < 0) continue;

                if (page.keyOnDelay.delayWrk[i] == 0 && page.keyOnDelay.delay[i] != -1)
                {
                    page.keyOnDelay.keyOn |= (byte)(1 << i);
                }
            }
        }



        private void Commander(partWork pw, partPage page, MML mml)
        {

            switch (mml.type)
            {
                case enmMMLType.CompileSkip:
                    log.Write("CompileSkip");
                    page.dataEnd = true;
                    page.enableInterrupt = true;
                    page.waitCounter = -1;
                    if ((string)mml.args[0] != "partEnd")
                        compileEnd = true;
                    break;
                case enmMMLType.Tempo:
                    log.Write("Tempo");
                    page.chip.CmdTempo(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.Instrument:
                    log.Write("Instrument");
                    page.chip.CmdInstrument(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.Octave:
                    log.Write("Octave");
                    page.chip.CmdOctave(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.OctaveUp:
                    log.Write("OctaveUp");
                    page.chip.CmdOctaveUp(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.OctaveDown:
                    log.Write("OctaveDown");
                    page.chip.CmdOctaveDown(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.Length:
                    log.Write("Length");
                    page.chip.CmdLength(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.TotalVolume:
                    log.Write("TotalVolume");
                    page.chip.CmdTotalVolume(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.Volume:
                    log.Write("Volume");
                    page.chip.CmdVolume(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.VolumeDown:
                    log.Write("VolumeDown");
                    page.chip.CmdVolumeDown(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.VolumeUp:
                    log.Write("VolumeUp");
                    page.chip.CmdVolumeUp(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.Pan:
                    log.Write("Pan");
                    page.chip.CmdPan(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.Gatetime:
                    log.Write("Gatetime");
                    page.chip.CmdGatetime(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.GatetimeDiv:
                    log.Write("GatetimeDiv");
                    page.chip.CmdGatetime2(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.Detune:
                    log.Write("Detune");
                    page.chip.CmdDetune(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.DirectMode:
                    log.Write("DirectMode");
                    page.chip.CmdDirectMode(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.Renpu:
                    log.Write("Renpu");
                    page.chip.CmdRenpuStart(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.RenpuEnd:
                    log.Write("RenpuEnd");
                    page.chip.CmdRenpuEnd(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.Repeat: //展開済みなので無駄
                    log.Write("Repeat");
                    page.chip.CmdRepeatStart(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.RepeatEnd: //展開済みなので無駄
                    log.Write("RepeatEnd");
                    page.chip.CmdRepeatEnd(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.RepertExit: //展開済みなので無駄
                    log.Write("RepertExit");
                    page.chip.CmdRepeatExit(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.Note:
                    log.Write("Note");
                    MML n = mml.Copy();
                    page.chip.CmdNote(pw, page, n);
                    page.mmlPos++;
                    break;
                case enmMMLType.Rest:
                    log.Write("Rest");
                    page.chip.CmdRest(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.Lyric:
                    log.Write("Lyric");
                    page.chip.CmdLyric(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.Envelope:
                    log.Write("Envelope");
                    page.chip.CmdEnvelope(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.Arpeggio:
                    log.Write("Arpeggio");
                    page.chip.CmdArpeggio(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.PhaseReset:
                    log.Write("PhaseReset");
                    page.chip.CmdPhaseReset(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.ReversePhase:
                    log.Write("ReversePhase");
                    page.chip.CmdReversePhase(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.PageDirectSend:
                    log.Write("PageDirectSend");
                    page.chip.CmdPageDirectSend(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.HardEnvelope:
                    log.Write("HardEnvelope");
                    page.chip.CmdHardEnvelope(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.ExtendChannel:
                    log.Write("ExtendChannel");
                    page.chip.CmdExtendChannel(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.VOperator:
                    log.Write("VOperator");
                    page.chip.CmdVOperator(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.VGuard:
                    log.Write("VGuard");
                    page.chip.CmdVGuard(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.Lfo:
                    log.Write("Lfo");
                    page.chip.CmdLfo(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.LfoSwitch:
                    log.Write("LfoSwitch");
                    page.chip.CmdLfoSwitch(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.PcmMode:
                    log.Write("PcmMode");
                    page.chip.CmdMode(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.PcmMap:
                    log.Write("PcmMapMode");
                    page.chip.CmdPcmMapSw(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.Noise:
                    log.Write("Noise");
                    page.chip.CmdNoise(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.DCSGCh3Freq:
                    log.Write("DCSGCh3Freq");
                    page.chip.CmdDCSGCh3Freq(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.Bend:
                    log.Write("Bend");
                    page.chip.CmdBend(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.Y:
                    log.Write("Y");
                    page.chip.CmdY(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.LoopPoint:
                    log.Write("LoopPoint");
                    page.chip.CmdLoop(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.JumpPoint:
                    log.Write("JumpPoint");
                    jumpPointClock = (long)dSample;// lClock;
                    jumpChannels.Add(new Tuple<enmChipType, int>(page.chip.chipType, page.ch));
                    if (doJumping) doJumping = false;
                    page.mmlPos++;
                    break;
                case enmMMLType.NoiseToneMixer:
                    log.Write("NoiseToneMixer");
                    page.chip.CmdNoiseToneMixer(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.Portament:
                    log.Write("Portament");
                    page.chip.CmdPortament(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.KeyShift:
                    log.Write("KeyShift");
                    page.chip.CmdKeyShift(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.AddressShift:
                    log.Write("AddressShift");
                    page.chip.CmdAddressShift(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.MIDICh:
                    log.Write("MIDICh");
                    page.chip.CmdMIDICh(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.MIDIControlChange:
                    log.Write("MIDIControlChange");
                    page.chip.CmdMIDIControlChange(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.Velocity:
                    log.Write("Velocity");
                    page.chip.CmdVelocity(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.SusOnOff:
                    log.Write("SusOnOff");
                    page.chip.CmdSusOnOff(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.Effect:
                    log.Write("Effect");
                    page.chip.CmdEffect(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.ForcedFnum:
                    log.Write("ForcedFnum");
                    page.chip.CmdForcedFnum(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.SkipPlay:
                    log.Write("SkipPlay");
                    jumpPointClock = (long)dSample;
                    jumpChannels.Add(new Tuple<enmChipType, int>(page.chip.chipType, page.ch));

                    page.mmlPos++;
                    break;
                case enmMMLType.ToneDoubler:
                    log.Write("ToneDoubler");
                    //page.ppage[page.cpageNum].chip.CmdToneDoubler(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.RelativeVolumeSetting:
                    log.Write("RelativeVolumeSetting");
                    page.chip.CmdRelativeVolumeSetting(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.Synchronous:
                    log.Write("Synchronous");//同期コマンドの場合は特に処理は必要ない
                    if ((int)mml.args[0] == 3)
                    {
                        MML smml = new MML();
                        smml.args = new List<object>();
                        smml.args.Add(mml.args[2]);
                        smml.line = mml.line;
                        page.chip.CmdRest(page, smml);
                    }
                    page.mmlPos++;
                    break;
                case enmMMLType.TraceUpdateStack:
                    log.Write("TraceUpdateStack");
                    page.chip.CmdTraceUpdateStack(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.KeyOnDelay:
                    log.Write("KeyOnDelay");
                    page.chip.CmdKeyOnDelay(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.HardEnvelopeSync:
                    log.Write("HardEnvelopeSync");
                    page.chip.CmdHardEnvelopeSync(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.Modulation:
                    log.Write("Modulation");
                    page.chip.CmdModulation(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.PartColor:
                    log.Write("PartColor");
                    page.chip.CmdPartColor(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.PartPriority:
                    log.Write("PartPriority");
                    page.chip.CmdPartPriority(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.PartArpeggio_Start:
                    log.Write("PartArpeggio_Start");
                    page.chip.CmdPartArpeggio_Start(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.PartArpeggio_End:
                    log.Write("PartArpeggio_End");
                    page.chip.CmdPartArpeggio_End(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.RR15:
                    log.Write("RR15");
                    page.chip.CmdRR15(page, mml);
                    page.mmlPos++;
                    break;
                case enmMMLType.TLOFS:
                    log.Write("TLOFS");
                    page.chip.CmdTLOFS(page, mml);
                    page.mmlPos++;
                    break;
                default:
                    msgBox.setErrMsg(string.Format(msg.get("E01016")
                        , mml.type)
                        , mml.line.Lp);
                    page.mmlPos++;
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
                            pg = new List<partPage>(),
                        };

                        for (int n = 0; n < 2; n++)
                        {
                            if (!partData[n].ContainsKey(chip.Ch[i].Name)) continue;

                            for (int c = 0; c < partData[n][chip.Ch[i].Name].Count; c++)
                            {
                                if (partData[n][chip.Ch[i].Name][c] == null || partData[n][chip.Ch[i].Name][c].Count < 1)
                                    continue;

                                pw.apg = new partPage(pw, pw.spg);
                                pw.apg.chip = chip;
                                pw.apg.chipNumber = ((info.format != enmFormat.ZGM && chip.ChipID == 1) ? 1 : 0);
                                pw.apg.ch = i;// + 1;
                                pw.apg.pData = partData[n][chip.Ch[i].Name][c];
                                pw.apg.isLayer = (n == 1);
                                pw.pg.Add(pw.apg);
                            }

                            pw.apg = pw.pg[0];
                            pw.cpg = pw.pg[0];

                            pw.aData = aliesData;

                            pw.setPos(pw.apg, 0);

                            foreach (partPage page in pw.pg)
                            {
                                page.Type = chip.Ch[i].Type;
                                page.slots = 0;
                                page.volume = 32767;
                            }

                            chip.InitPart(pw);

                            foreach (partPage page in pw.pg)
                                OutData(page.sendData);

                            foreach (partPage pg in pw.pg)
                            {
                                pg.PartName = chip.Ch[i].Name;
                                pg.waitKeyOnCounter = -1;
                                pg.waitRR15Counter = -1;
                                pg.waitCounter = 0;
                                pg.freq = -1;

                                pg.dataEnd = false;
                                if (pg.pData == null || pg.pData.Count < 1)
                                {
                                    pg.dataEnd = true;
                                    pg.enableInterrupt = true;
                                }
                                else
                                {
                                    bool flg = false;
                                    if (pg.pData != null && pg.pData.Count > 0)
                                    {
                                        //foreach (List<Line> plin in pw.apg.pData)
                                        {
                                            foreach (Line pl in pg.pData)
                                            {
                                                if (pl != null)
                                                {
                                                    flg = true;
                                                    break;
                                                }
                                            }
                                        }
                                    }

                                    if (flg) chip.use = true;
                                }
                            }

                        }

                        if (pw.pg.Count < 1)
                        {
                            //パートデータが無くてもページ0は用意する
                            pw.apg = new partPage(pw, pw.spg);
                            pw.cpg = pw.apg;
                            pw.apg.chip = chip;
                            pw.apg.chipNumber = ((info.format != enmFormat.ZGM && chip.ChipID == 1) ? 1 : 0);
                            pw.apg.ch = i;// + 1;
                            pw.pg.Add(pw.apg);

                            pw.aData = aliesData;

                            pw.setPos(pw.apg, 0);

                            foreach (partPage page in pw.pg)
                            {
                                page.Type = chip.Ch[i].Type;
                                page.slots = 0;
                                page.volume = 32767;
                            }

                            chip.InitPart(pw);

                            foreach (partPage page in pw.pg)
                                OutData(page.sendData);

                            foreach (partPage pg in pw.pg)
                            {
                                pg.PartName = chip.Ch[i].Name;
                                pg.waitKeyOnCounter = -1;
                                pg.waitRR15Counter = -1;
                                pg.waitCounter = 0;
                                pg.freq = -1;

                                pg.dataEnd = false;
                                if (pg.pData == null || pg.pData.Count < 1)
                                {
                                    pg.dataEnd = true;
                                    pg.enableInterrupt = true;
                                }
                                else
                                {
                                    bool flg = false;
                                    if (pg.pData != null && pg.pData.Count > 0)
                                    {
                                        //foreach (List<Line> plin in pw.apg.pData)
                                        {
                                            foreach (Line pl in pg.pData)
                                            {
                                                if (pl != null)
                                                {
                                                    flg = true;
                                                    break;
                                                }
                                            }
                                        }
                                    }

                                    if (flg) chip.use = true;
                                }
                            }
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
                        //pw.pData = null;
                        pw.aData = null;
                        //pw.ppg[pw.cpgNum].mmlData = null;
                        pw.apg.dataEnd = true;
                        if (mmlData.ContainsKey(pw.apg.PartName))
                        {
                            //pw.ppg[pw.cpgNum].mmlData = mmlData[pw.ppg[pw.cpgNum].PartName];
                            chip.use = true;
                            pw.apg.dataEnd = false;
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
#if DEBUG
            //if (mml != null && mml.line != null && mml.line.Lp != null)
            //    Console.WriteLine("Chip:[{0}] type:[{1}]", mml.line.Lp.chip, mml.type);
            //else Console.WriteLine("mml null");
#endif
            //string msg = "";
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
                            od.linePos = new LinePos(mml.line.Lp.document, mml.line.Lp.srcMMLID, mml.line.Lp.row, mml.line.Lp.col, mml.line.Lp.length, mml.line.Lp.part, mml.line.Lp.chip, mml.line.Lp.chipIndex, mml.line.Lp.chipNumber, mml.line.Lp.ch);
                    }
                    dat.Add(od);

                    //msg+=string.Format("{0:x02} :", d);
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
                        od.linePos = new LinePos(mml.line.Lp.document, mml.line.Lp.srcMMLID, mml.line.Lp.row, mml.line.Lp.col, mml.line.Lp.length, mml.line.Lp.part, mml.line.Lp.chip, mml.line.Lp.chipIndex, mml.line.Lp.chipNumber, mml.line.Lp.ch);
                }
                dat.Add(od);
                //msg += string.Format("{0:x02} :", d);
            }

            //msg += string.Format("{0}", mml == null ? "NULL" : mml.type.ToString());
            //log.ForcedWrite(msg);
        }

        public void OutData(outDatum od, byte[] cmd, params byte[] data)
        {
            //string msg = "";
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
                            o.linePos = new LinePos(od.linePos.document, od.linePos.srcMMLID, od.linePos.row, od.linePos.col, od.linePos.length, od.linePos.part, od.linePos.chip, od.linePos.chipIndex, od.linePos.chipNumber, od.linePos.ch);
                    }
                    dat.Add(o);
                    //msg += string.Format("{0:x02} :", d);
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
                        o.linePos = new LinePos(od.linePos.document, od.linePos.srcMMLID, od.linePos.row, od.linePos.col, od.linePos.length, od.linePos.part, od.linePos.chip, od.linePos.chipIndex, od.linePos.chipNumber, od.linePos.ch);
                }
                dat.Add(o);
                //msg += string.Format("{0:x02} :", d);
            }
            //log.ForcedWrite(msg);
        }

        public void OutData(List<outDatum> sendData)
        {
            if (sendData == null || sendData.Count < 1) return;

            //string msg="SDataFlashing";
            foreach (outDatum od in sendData)
            {
                dat.Add(od);
                //msg+=string.Format("{0:x02} :", od.val);
            }
            //log.ForcedWrite(msg);
        }

        private byte[] zgmWaitCmd1 = new byte[] { 0x01 };
        private byte[] zgmWaitCmd2 = new byte[] { 0x01, 0x00 };
        private byte[] vgmWaitCmd = new byte[] { 0x61 };

        private void OutWaitNSamples(long n)
        {
            long m = n;
            byte[] cmd;
            if (info.format == enmFormat.ZGM)
            {
                if (ChipCommandSize == 2) cmd = zgmWaitCmd2;
                else cmd = zgmWaitCmd1;
            }
            else cmd = vgmWaitCmd;

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

                int f = (int)cpw.apg.pcmBaseFreqPerFreq;
                cpw.apg.pcmFreqCountBuffer += cpw.apg.pcmBaseFreqPerFreq - (int)cpw.apg.pcmBaseFreqPerFreq;
                while (cpw.apg.pcmFreqCountBuffer > 1.0f)
                {
                    f++;
                    cpw.apg.pcmFreqCountBuffer -= 1.0f;
                }
                if (i + f >= info.samplesPerClock * cnt)
                {
                    cpw.apg.pcmFreqCountBuffer += (int)(i + f - info.samplesPerClock * cnt);
                    f = (int)(info.samplesPerClock * cnt - i);
                }
                if (cpw.apg.pcmSizeCounter > 0)
                {
                    cpw.apg.pcmSizeCounter--;
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
