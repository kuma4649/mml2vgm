using musicDriverInterface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace Corex64
{
    abstract public class ClsChip
    {
        public enmChipType chipType
        {
            get
            {
                return _chipType;
            }
        }
        protected enmChipType _chipType = enmChipType.None;

        public string Name
        {
            get
            {
                return _Name;
            }
        }
        protected string _Name = "";

        public string ShortName
        {
            get
            {
                return _ShortName;
            }
        }
        protected string _ShortName = "";

        public int ChMax
        {
            get => _ChMax;
        }
        protected int _ChMax = 0;

        public int ChipID
        {
            get
            {
                return _ChipID;
            }

            set
            {
                _ChipID = value;
            }
        }

        public bool CanUsePcm
        {
            get
            {
                return _canUsePcm;
            }

            set
            {
                _canUsePcm = value;
            }
        }
        protected bool _canUsePcm = false;

        public bool CanUsePI
        {
            get
            {
                return _canUsePI;
            }

            set
            {
                _canUsePI = value;
            }
        }
        protected bool _canUsePI = false;

        public int ChipNumber
        {
            get
            {
                return _ChipNumber;
            }

            set
            {
                _ChipNumber = value;
            }
        }
        protected int _ChipNumber = 0;

        public string PartName { get; set; }
        public byte rhythmStatus { get; internal set; } = 0;
        public byte beforeRhythmStatus { get; internal set; } = 0xff;
        public int connectionSel { get; internal set; } = 0;
        public int beforeConnectionSel { get; internal set; } = -1;

        public bool SupportReversePartWork = false;
        public bool ReversePartWork = false;


        public Function Envelope = null;

        protected int _ChipID = -1;
        protected ClsVgm parent;
        protected byte dataType;
        public ClsChannel[] Ch;
        public int Frequency = 7670454;
        public bool use;
        public List<partWork> lstPartWork;
        public double[] noteTbl = new double[] {
            //   c       c+        d       d+        e        f       f+        g       g+        a       a+        b
            261.62 , 277.18 , 293.66 , 311.12 , 329.62 , 349.22 , 369.99 , 391.99 , 415.30 , 440.00 , 466.16 , 493.88
        };
        public int[][] FNumTbl;
        private string stPath = "";
        public clsPcmDataInfo[] pcmDataInfo;
        public byte[] pcmDataEasy = null;
        public byte[] pcmDataEasyA = null;
        public List<byte[]> pcmDataDirect = new List<byte[]>();
        public byte[][] port;
        public Dictionary<int, int> relVol = new Dictionary<int, int>();
        public byte vgmHeaderPos;
        public byte DataBankID;

        public ClsChip(ClsVgm parent, int chipID, string initialPartName, string stPath, int chipNumber)
        {
            this.parent = parent;
            this._ChipID = chipID;
            this.stPath = stPath;
            this.ChipNumber = chipNumber;
            this.PartName = initialPartName;

            MakeFNumTbl();
        }

        public void SetCommand(int cmdNo)
        {
            //if (parent.ChipCommandSize == 2)
            //{
            //    port0 = new byte[] {
            //        (byte)cmdNo,
            //        (byte)(cmdNo>>8)
            //    };
            //}
            //else port0 = new byte[] {
            //        (byte)cmdNo,
            //};
        }

        protected Dictionary<string, List<double>> MakeFNumTbl()
        {
            if (string.IsNullOrEmpty(Name)) return null;
            //for (int i = 0; i < noteTbl.Length; i++)
            //{
            //    FNumTbl[0][i] = (int)(Math.Round(((144.0 * noteTbl[i] * Math.Pow(2.0, 20) / Frequency) / Math.Pow(2.0, (4 - 1))), MidpointRounding.AwayFromZero));
            //}
            //FNumTbl[0][12] = FNumTbl[0][0] * 2;

            string fn = string.Format("FNUM_{0}.txt", Name);
            Dictionary<string, List<double>> dic = new Dictionary<string, List<double>>();

            Stream stream;
            if (File.Exists(Path.Combine(stPath, "fnum", fn)))
            {
                fn = Path.Combine(stPath, "fnum", fn);
                stream = new FileStream(fn, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            else
            {
                System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
                string[] resources = asm.GetManifestResourceNames();
                foreach (string resource in resources)
                {
                    if (resource.IndexOf(fn) >= 0)
                    {
                        fn = resource;
                    }
                }
                stream = asm.GetManifestResourceStream(fn);
            }

            try
            {
                if (stream != null)
                {
                    using (System.IO.StreamReader sr = new System.IO.StreamReader(stream, Encoding.Unicode))
                    {
                        stream = null;
                        while (!sr.EndOfStream)
                        {
                            //内容を読み込む
                            string[] s = sr.ReadLine().Split(new string[] { "=" }, StringSplitOptions.None);
                            if (s == null || s.Length != 2) continue;
                            if (s[0].Trim() == "" || s[0].Trim().Length < 1 || s[0].Trim()[0] == '\'') continue;
                            string[] val = s[1].Split(new string[] { "," }, StringSplitOptions.None);
                            s[0] = s[0].ToUpper().Trim();

                            if (!dic.ContainsKey(s[0]))
                            {
                                List<double> value = new List<double>();
                                dic.Add(s[0], value);
                            }

                            foreach (string v in val)
                            {
                                string vv = v.Trim();

                                if (vv[0] == '$' && vv.Length > 1)
                                {
                                    int num16 = Convert.ToInt32(vv.Substring(1), 16);
                                    dic[s[0]].Add(num16);
                                }
                                else
                                {
                                    if (double.TryParse(vv, out double o))
                                    {
                                        dic[s[0]].Add(o);
                                    }
                                }
                            }

                        }
                    }
                }
            }
            catch
            {
                dic = null;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Dispose();
                }

            }

            return dic;
        }

        public bool ChannelNameContains(string name)
        {
            foreach (ClsChannel c in Ch)
            {
                if (c.Name == name) return true;
            }
            return false;
        }

        public void SetPartToCh(ClsChannel[] Ch, string val)
        {
            if (val == null || (val.Length != 1 && val.Length != 2)) return;

            string f = val[0].ToString();
            string r = (val.Length == 2) ? val[1].ToString() : " ";

            for (int i = 0; i < Ch.Length; i++)
            {
                if (Ch[i] == null) Ch[i] = new ClsChannel();
                Ch[i].Name = string.Format("{0}{1}{2:00}", f, r, i + 1);
            }

            //checkDuplication(fCh);
        }

        public int SetEnvelopParamFromInstrument(partPage page, int n, bool re, MML mml)
        {
            if (!parent.instENV.ContainsKey(n))
            {
                msgBox.setErrMsg(string.Format(msg.get("E10000"), n)
                    , mml.line.Lp);
            }
            else
            {
                page.envelopeMode = true;
                if (re) n = page.envInstrument + n;
                if (page.envInstrument != n)
                {
                    page.envInstrument = n;
                    page.envIndex = -1;
                    page.envCounter = -1;

                    if (parent.instENV[n].Item2.Length == 9)
                    {
                        //既存フォーマットを拡張フォーマットに編集する
                        page.envelope[(int)eENV.num] = parent.instENV[n].Item2[0]; //  num <- num
                        page.envelope[(int)eENV.SV] = parent.instENV[n].Item2[1]; //   SV  <- SV
                        page.envelope[(int)eENV.AR] = parent.instENV[n].Item2[2]; //   AR  <- AR
                        page.envelope[(int)eENV.AST] = parent.instENV[n].Item2[7]; //  AST <- ST
                        page.envelope[(int)eENV.DR] = parent.instENV[n].Item2[3]; //   DR  <- DR
                        page.envelope[(int)eENV.DST] = parent.instENV[n].Item2[7]; //  DST <- ST
                        page.envelope[(int)eENV.SL] = parent.instENV[n].Item2[4]; //   SL  <- SL
                        page.envelope[(int)eENV.SR] = parent.instENV[n].Item2[5]; //   SR  <- SR
                        page.envelope[(int)eENV.SST] = parent.instENV[n].Item2[7]; //  SST <- ST
                        page.envelope[(int)eENV.RL] = 0; //                            RL  <- 0
                        page.envelope[(int)eENV.RR] = parent.instENV[n].Item2[6]; //   RR  <- RR
                        page.envelope[(int)eENV.RST] = parent.instENV[n].Item2[7]; //  RST <- ST
                        page.envelope[(int)eENV.chiptype] = parent.instENV[n].Item2[8]; //  chiptype <- chiptype
                    }
                    else
                    {
                        //拡張フォーマット時は特に編集不要
                        for (int i = 0; i < parent.instENV[n].Item2.Length; i++)
                        {
                            page.envelope[i] = parent.instENV[n].Item2[i];
                        }

                    }
                }
            }
            SetDummyData(page, mml);
            return n;
        }

        private int AnalyzeBend(partPage page, MML mml, Note note, int ml)
        {
            int bendDelayCounter;
            page.octaveNow = page.octaveNew;
            page.bendOctave = page.octaveNow;
            page.bendNote = note.bendCmd;
            page.bendShift = note.bendShift;
            page.bendWaitCounter = -1;
            bendDelayCounter = 0;//TODO: bendDelay

            if (note.bendOctave != null)
            {
                for (int i = 0; i < note.bendOctave.Count; i++)
                {
                    switch (note.bendOctave[i].type)
                    {
                        case enmMMLType.Octave:
                            int n = (int)note.bendOctave[i].args[0];
                            n = Common.CheckRange(n, 0, 8);
                            page.bendOctave = n;
                            break;
                        case enmMMLType.OctaveUp:
                            //pw.incPos(page);
                            page.bendOctave += parent.info.octaveRev ? -1 : 1;
                            page.bendOctave = Common.CheckRange(page.bendOctave, 0, 8);
                            break;
                        case enmMMLType.OctaveDown:
                            //pw.incPos(page);
                            page.bendOctave += parent.info.octaveRev ? 1 : -1;
                            page.bendOctave = Common.CheckRange(page.bendOctave, 0, 8);
                            break;
                    }
                }
            }

            //音符の変化量
            int ed = Const.NOTE.IndexOf(page.bendNote) + 1 + page.bendOctave * 12 + page.bendShift;
            ed = Common.CheckRange(ed, 0, 9 * 12 - 1);
            int st = Const.NOTE.IndexOf(note.cmd) + 1 + page.octaveNow * 12 + note.shift;//
            st = Common.CheckRange(st, 0, 9 * 12 - 1);
            //今回のノートを保存
            page.MPortamentLastNote = page.bendOctave * 12 + "czdzefzgzazb".IndexOf(page.bendNote) + page.bendShift;

            int delta = ed - st;
            if (delta != 0 && (ml - bendDelayCounter - 1) / (float)delta == 0)
            {
                page.bendNote = 'r';
                page.bendWaitCounter = -1;
                msgBox.setErrMsg(msg.get("E10047"), mml.line.Lp);
            }
            else if ((delta == 0 && note.pitchShift == note.bendPitchShift) || bendDelayCounter == ml)
            {
                page.bendNote = 'r';
                page.bendWaitCounter = -1;
            }
            else
            {
                //１音符当たりのウエイト
                float wait = (ml - bendDelayCounter - 1) / (float)(delta == 0 ? 1 : delta);
                float tl = 0;
                float bf = Math.Sign(wait);
                List<int> lstBend = new List<int>();
                //int toneDoublerShift = 
                GetToneDoublerShift(
                page
                , page.octaveNow
                , note.cmd
                , note.shift);

                //midi向け
                if (!page.beforeTie)
                {
                    page.bendStartNote = note.cmd;
                    page.bendStartOctave = page.octaveNow;
                    page.bendStartShift = page.shift;
                }

                for (int i = 0; i < Math.Abs(delta == 0 ? 1 : delta); i++)
                {
                    bf += wait;
                    tl += wait;
                    int arpNote = page.arpFreqMode ? 0 : page.arpDelta;
                    int pitchShift = note.bendPitchShift - note.pitchShift;
                    int aPitchShift = (int)(pitchShift / (float)Math.Abs(delta == 0 ? 1 : delta) * (i + 0)) + note.pitchShift;
                    int bPitchShift = (int)(pitchShift / (float)Math.Abs(delta == 0 ? 1 : delta) * (i + 1)) + note.pitchShift;

                    GetFNumAtoB(
                        page
                        , mml
                        , out int a
                        , page.octaveNow
                        , note.cmd
                        , note.shift + (i + 0) * Math.Sign(delta) + page.keyShift + page.toneDoublerKeyShift + arpNote
                        , aPitchShift
                        , out int b
                        , page.octaveNow
                        , note.cmd
                        , note.shift + (i + 1) * Math.Sign(delta) + page.keyShift + page.toneDoublerKeyShift + arpNote
                        , bPitchShift
                        , delta
                        );

                    if (Math.Abs(bf) >= 1.0f || wait == 0)
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
                page.bendList = new Stack<Tuple<int, int>>();
                foreach (Tuple<int, int> lbt in lb)
                {
                    page.bendList.Push(lbt);
                }
                Tuple<int, int> t = page.bendList.Pop();
                page.bendFnum = t.Item1;
                page.bendWaitCounter = t.Item2;
                page.bendPitchShift = note.bendPitchShift;
            }

            return bendDelayCounter;
        }

        private bool CheckLFOParam(partPage page, int c, MML mml)
        {
            if (page.lfo[c].param == null)
            {
                msgBox.setErrMsg(msg.get("E10001")
                    , mml.line.Lp);
                return false;
            }

            return true;
        }



        public virtual void InitChip()
        {
            throw new NotImplementedException("継承先で要実装");
        }

        public virtual void InitPart(partWork pw)
        {
            throw new NotImplementedException("継承先で要実装");
        }


        public bool CanUsePICommand()
        {
            return CanUsePI;
        }

        public virtual void StorePcm(Dictionary<int, Tuple<string, clsPcm>> newDic, KeyValuePair<int, clsPcm> v, byte[] buf, bool is16bit, int samplerate, params object[] option)
        {
            pcmDataInfo = null;
        }

        public virtual void StorePcmRawData(clsPcmDatSeq pds, byte[] buf, bool isRaw, bool is16bit, int samplerate, params object[] option)
        {
            throw new NotImplementedException("継承先で要実装");
        }

        public virtual bool StorePcmCheck()
        {
            throw new NotImplementedException("継承先で要実装");
        }

        public virtual void SetPCMDataBlock(MML mml)
        {
            if (!CanUsePcm) return;
            if (!use) return;

            int maxSize = 0;
            int ptr = 7 + (parent.ChipCommandSize == 2 ? 2 : 0);

            AdjustPCMData(ptr);

            if (parent.info.format == enmFormat.ZGM)
            {
                if (port.Length < 1) return;

                if (parent.ChipCommandSize != 2)
                {
                    if (port[0].Length < 1) return;

                    if (pcmDataEasy != null && pcmDataEasy.Length > 1) pcmDataEasy[1] = port[0][0];
                    for (int i = 0; i < pcmDataDirect.Count; i++)
                    {
                        pcmDataDirect[i][1] = port[0][0];
                    }
                }
                else
                {
                    if (port[0].Length < 2) return;

                    if (pcmDataEasy != null && pcmDataEasy.Length > 3)
                    {
                        pcmDataEasy[2] = port[0][0];
                        pcmDataEasy[3] = port[0][1];
                    }
                    for (int i = 0; i < pcmDataDirect.Count; i++)
                    {
                        pcmDataDirect[i][2] = port[0][0];
                        pcmDataDirect[i][3] = port[0][1];
                    }
                }
            }

            if (pcmDataEasy != null && pcmDataEasy.Length > 0)
            {
                maxSize =
                    pcmDataEasy[ptr]
                    + (pcmDataEasy[ptr + 1] << 8)
                    + (pcmDataEasy[ptr + 2] << 16)
                    + (pcmDataEasy[ptr + 3] << 24);
            }
            if (pcmDataDirect != null && pcmDataDirect.Count > 0)
            {
                foreach (byte[] dat in pcmDataDirect)
                {
                    if (dat != null && dat.Length > 0)
                    {
                        int size =
                            dat[ptr]
                            + (dat[ptr + 1] << 8)
                            + (dat[ptr + 2] << 16)
                            + (dat[ptr + 3] << 24);
                        if (maxSize < size) maxSize = size;
                    }
                }
            }
            if (pcmDataEasy != null && pcmDataEasy.Length > 0)
            {
                pcmDataEasy[ptr] = (byte)maxSize;
                pcmDataEasy[ptr + 1] = (byte)(maxSize >> 8);
                pcmDataEasy[ptr + 2] = (byte)(maxSize >> 16);
                pcmDataEasy[ptr + 3] = (byte)(maxSize >> 24);
            }
            if (pcmDataDirect != null && pcmDataDirect.Count > 0)
            {
                foreach (byte[] dat in pcmDataDirect)
                {
                    if (dat != null && dat.Length > 0)
                    {
                        dat[ptr] = (byte)maxSize;
                        dat[ptr + 1] = (byte)(maxSize >> 8);
                        dat[ptr + 2] = (byte)(maxSize >> 16);
                        dat[ptr + 3] = (byte)(maxSize >> 24);
                    }
                }
            }

            if (pcmDataEasy != null && pcmDataEasy.Length > 0)
                parent.OutData(mml, null, pcmDataEasy);

            if (pcmDataDirect == null || pcmDataDirect.Count < 1) return;

            foreach (byte[] dat in pcmDataDirect)
            {
                if (dat != null && dat.Length > 0)
                    parent.OutData(mml, null, dat);
            }
        }

        public virtual void AdjustPCMData(int ptr)
        {
        }

        public virtual int GetToneDoublerShift(partPage page, int octave, char noteCmd, int shift)
        {
            throw new NotImplementedException("継承先で要実装");
        }

        public virtual void SetToneDoubler(partPage page, MML mml)
        {
            throw new NotImplementedException("継承先で要実装");
        }


        public virtual int GetFNum(partPage page, MML mml, int octave, char cmd, int shift, int pitchShift)
        {
            throw new NotImplementedException("継承先で要実装");
        }

        public virtual void GetFNumAtoB(partPage page, MML mml
            , out int a, int aOctaveNow, char aCmd, int aShift, int aPitchShift
            , out int b, int bOctaveNow, char bCmd, int bShift, int bPitchShift
            , int dir)
        {
            a = GetFNum(page, mml, aOctaveNow, aCmd, aShift, aPitchShift);
            b = GetFNum(page, mml, bOctaveNow, bCmd, bShift, bPitchShift);
        }

        public virtual void SetFNum(partPage page, MML mml)
        {
            throw new NotImplementedException("継承先で要実装");
        }


        public virtual void SetDummyData(partPage page, MML mml)
        {
            byte[] cmd;
            if (page.chip.parent.info.format == enmFormat.ZGM)
            {
                if (page.chip.parent.ChipCommandSize == 2)
                {
                    cmd = new byte[] { 0x09, 0x00, page.port[0][0], page.port[0][1] };
                    parent.dummyCmdCounter += 4;
                }
                else
                {
                    cmd = new byte[] { 0x09, page.port[0][0] };
                    parent.dummyCmdCounter += 3;
                }

                //Console.WriteLine("SkipAddress:{0:x06} skip:{1:x06}", parent.dat.Count, parent.dummyCmdCounter);
                parent.OutData(mml, cmd, (byte)(page.chip.ChipNumber));//0x09(zgm):DummyChip (!!CAUTION!!)
            }
            else
            {
                cmd = new byte[] { 0x2f, page.port[0][0] };
                parent.OutData(mml, cmd, (byte)(page.chip.ChipNumber));//0x2f(vgm/xgm):DummyChip (!!CAUTION!!)
                parent.dummyCmdCounter += 3;
            }

        }

        public virtual void SetKeyOn(partPage page, MML mml)
        {
            throw new NotImplementedException("継承先で要実装");
        }

        public virtual void SetKeyOff(partPage page, MML mml)
        {
            throw new NotImplementedException("継承先で要実装");
        }

        public virtual void SetVolume(partPage page, MML mml)
        {
            throw new NotImplementedException("継承先で要実装");
        }

        public virtual void SetLfoAtKeyOn(partPage page, MML mml)
        {
            throw new NotImplementedException("継承先で要実装");
        }

        public virtual void SetEnvelopeAtKeyOn(partPage page, MML mml)
        {
            if (!page.envelopeMode)
            {
                page.envVolume = 0;
                page.envIndex = -1;
                return;
            }

            //System.Diagnostics.Debug.WriteLine("");
            //System.Diagnostics.Debug.WriteLine("EnvKeyOn");

            page.envIndex = 0;
            page.envCounter = 0;
            int maxValue = page.MaxVolume;// (pw.ppg[pw.cpgNum].envelope[8] == (int)enmChipType.RF5C164) ? 255 : 15;

            while (page.envCounter == 0 && page.envIndex != -1)
            {
                switch (page.envIndex)
                {
                    case 0: // AR phase
                        //System.Diagnostics.Debug.Write("EnvAR");
                        page.envCounter = page.envelope[(int)eENV.AR];
                        if (page.envelope[(int)eENV.AR] > 0 && page.envelope[(int)eENV.SV] < maxValue)
                        {
                            page.envVolume = page.envelope[(int)eENV.SV];
                            //System.Diagnostics.Debug.Write(string.Format(":{0}",pw.ppg[pw.cpgNum].envVolume));
                        }
                        else
                        {
                            page.envVolume = maxValue;
                            page.envIndex++;
                            //System.Diagnostics.Debug.Write(string.Format(":next", pw.ppg[pw.cpgNum].envVolume));
                        }
                        //System.Diagnostics.Debug.WriteLine("");
                        break;
                    case 1: // DR phase
                        //System.Diagnostics.Debug.Write("EnvDR");
                        page.envCounter = page.envelope[(int)eENV.DR];
                        if (page.envelope[(int)eENV.DR] > 0 && page.envelope[(int)eENV.SL] < maxValue)
                        {
                            page.envVolume = maxValue;
                            //System.Diagnostics.Debug.Write(string.Format(":{0}", pw.ppg[pw.cpgNum].envVolume));
                        }
                        else
                        {
                            page.envVolume = page.envelope[(int)eENV.SL];
                            page.envIndex++;
                            //System.Diagnostics.Debug.Write(string.Format(":next", pw.ppg[pw.cpgNum].envVolume));
                        }
                        //System.Diagnostics.Debug.WriteLine("");
                        break;
                    case 2: // SR phase
                        //System.Diagnostics.Debug.Write("EnvSR");
                        page.envCounter = page.envelope[(int)eENV.SR];
                        if (page.envelope[(int)eENV.SR] > 0 && page.envelope[(int)eENV.SL] != 0)
                        {
                            page.envVolume = page.envelope[(int)eENV.SL];
                            //System.Diagnostics.Debug.Write(string.Format(":{0}", pw.ppg[pw.cpgNum].envVolume));
                        }
                        else
                        {
                            page.envVolume = 0;
                            page.envIndex = -1;
                            //System.Diagnostics.Debug.Write(string.Format(":end", pw.ppg[pw.cpgNum].envVolume));
                        }
                        //System.Diagnostics.Debug.WriteLine("");
                        break;
                }
            }
        }

        public virtual void SetArpeggioAtKeyOn(partPage page, MML mml)
        {
            if (!page.arpeggioMode
                || page.arpInstrument == -1
                || !parent.instArp.ContainsKey(page.arpInstrument)
                || parent.instArp[page.arpInstrument].Length < 2
                )
            {
                page.arpIndex = -1;
                return;
            }

            page.arpIndex = 0;
            page.arpDelta = 0;
            page.arpInstrumentPtr = 2;//0番目はinstの番号
            page.arpGatetime = 0;
            if ((parent.instArp[page.arpInstrument][1].dat & 1) == 0)
            {
                page.arpTieMode = false;
            }
            else
            {
                page.arpTieMode = true;
            }
            if ((parent.instArp[page.arpInstrument][1].dat & 2) == 0)
            {
                page.arpFreqMode = false;
            }
            else
            {
                page.arpFreqMode = true;
            }
            page.arpLoopPtr = -1;
            page.arpCounter = 0;

            page.arpGatetimePmode = false;
        }

        public virtual void SetVArpeggioAtKeyOn(partPage page, MML mml)
        {
            if (!page.varpeggioMode
                || page.varpInstrument == -1
                || !parent.instVArp.ContainsKey(page.varpInstrument)
                || parent.instVArp[page.varpInstrument].Length < 2
                )
            {
                page.varpIndex = -1;
                return;
            }

            page.varpIndex = 0;
            page.varpDelta = 0;
            page.varpInstrumentPtr = 2;//0番目はinstの番号
            page.varpLoopPtr = -1;
            page.varpCounter = 0;

        }

        public virtual void SetCommandArpeggioAtKeyOn(partPage page, MML mml)
        {
            foreach (CommandArpeggio ca in page.commandArpeggio.Values)
            {

                if (!ca.Sw
                    || ca.Num == -1
                    || !parent.instCommandArp.ContainsKey(ca.Num)
                    || parent.instCommandArp[ca.Num].Length < 2
                    )
                {
                    //ca.Num = -1;
                    continue;
                }

                if (ca.Sync != 0) continue;

                ca.Ptr = 0;
                ca.LoopPtr = -1;
                ca.WaitCounter = 0;

            }
        }



        public virtual void CmdY(partPage page, MML mml)
        {
            throw new NotImplementedException("継承先で要実装");
        }

        public virtual void CmdTempo(partPage page, MML mml)
        {
            parent.info.tempo = (int)mml.args[0];
            if (parent.info.vgmVsync == -1)
            {
                if (parent.info.format == enmFormat.VGM || parent.info.format == enmFormat.ZGM)
                {
                    parent.info.samplesPerClock = Information.VGM_SAMPLE_PER_SECOND * 60.0 * 4.0 / (parent.info.tempo * parent.info.clockCount);
                }
                else
                {
                    parent.info.samplesPerClock = parent.info.xgmSamplesPerSecond * 60.0 * 4.0 / (parent.info.tempo * parent.info.clockCount);
                }
            }
            else
            {
                if (parent.info.format == enmFormat.VGM || parent.info.format == enmFormat.ZGM)
                {
                    parent.info.samplesPerClock = 44100 / parent.info.vgmVsync;
                }
                else
                {
                    parent.info.samplesPerClock = parent.info.xgmSamplesPerSecond / parent.info.vgmVsync;
                }

            }

            SetDummyData(page, mml);
        }

        public virtual void CmdKeyShift(partPage page, MML mml)
        {
            int n = (int)mml.args[0];
            bool r = mml.args.Count > 1;

            if (!r) page.keyShift = Common.CheckRange(n, -128, 128);
            else page.keyShift = Common.CheckRange(page.keyShift + n, -128, 128);

            SetDummyData(page, mml);
        }

        public virtual void CmdAddressShift(partPage page, MML mml)
        {
            int sign = (int)mml.args[0];
            int n = (int)mml.args[1];

            page.addressShift = (sign == 0) ? n : (page.addressShift + (n * sign));
            if (page.addressShift < 0) page.addressShift = 0;
        }

        public virtual void CmdMIDICh(partPage page, MML mml)
        {
            msgBox.setErrMsg(msg.get("E10028")
                    , mml.line.Lp);
        }

        public virtual void CmdMIDIControlChange(partPage page, MML mml)
        {
            msgBox.setErrMsg(msg.get("E10031")
                    , mml.line.Lp);
        }

        public virtual void CmdVelocity(partPage page, MML mml)
        {
            msgBox.setErrMsg(msg.get("E10029")
                    , mml.line.Lp);
        }

        public virtual void CmdNoise(partPage page, MML mml)
        {
            msgBox.setErrMsg(msg.get("E10002")
                    , mml.line.Lp);
        }

        public virtual void CmdDCSGCh3Freq(partPage page, MML mml)
        {
            msgBox.setErrMsg(msg.get("E10032")
                    , mml.line.Lp);
        }

        public virtual void CmdSusOnOff(partPage page, MML mml)
        {
            msgBox.setErrMsg(msg.get("E10022")
                    , mml.line.Lp);
        }

        public virtual void CmdEffect(partPage page, MML mml)
        {
            msgBox.setErrMsg(msg.get("E10033")
                    , mml.line.Lp);
        }

        public virtual void CmdForcedFnum(partPage page, MML mml)
        {
            msgBox.setErrMsg(msg.get("E10035")
                    , mml.line.Lp);
        }

        public virtual void CmdMPMS(partPage page, MML mml)
        {
            msgBox.setErrMsg(msg.get("E10003")
                    , mml.line.Lp);
        }

        public virtual void CmdMAMS(partPage page, MML mml)
        {
            msgBox.setErrMsg(msg.get("E10004")
                    , mml.line.Lp);
        }

        public virtual void CmdLfo(partPage page, MML mml)
        {
            if (mml.args[0] is string)
            {
                if ((string)mml.args[0] == "MAMS")
                {
                    CmdMAMS(page, mml);
                    return;
                }
                if ((string)mml.args[0] == "MPMS")
                {
                    CmdMPMS(page, mml);
                    return;
                }
            }

            int c = (char)mml.args[0] - 'P';
            eLfoType t = eLfoType.unknown;
            switch ((char)mml.args[1])
            {
                case 'T':
                    t = eLfoType.Tremolo;
                    break;
                case 'V':
                    t = eLfoType.Vibrato;
                    break;
                case 'H':
                    t = eLfoType.Hardware;
                    break;
                case 'W':
                    t = eLfoType.Wah;
                    break;
            }

            page.lfo[c].type = t;
            page.lfo[c].sw = false;
            page.lfo[c].isEnd = true;
            page.lfo[c].param = new List<int>();
            for (int i = 2; i < mml.args.Count; i++)
            {
                if (mml.args[i] is int)
                    page.lfo[c].param.Add((int)mml.args[i]);
            }

            if (page.lfo[c].type == eLfoType.Tremolo
                || page.lfo[c].type == eLfoType.Vibrato
                || page.lfo[c].type == eLfoType.Wah)
            {
                if (page.lfo[c].param.Count < 4 + (page.lfo[c].type == eLfoType.Wah ? 1 : 0))
                {
                    msgBox.setErrMsg(msg.get("E10005")
                    , mml.line.Lp);
                    return;
                }
                if (page.lfo[c].param.Count > 9 + (page.lfo[c].type == eLfoType.Wah ? 1 : 0))
                {
                    msgBox.setErrMsg(msg.get("E10006")
                    , mml.line.Lp);
                    return;
                }

                int w = 0;
                if (page.lfo[c].type == eLfoType.Wah) w = 1;

                page.lfo[c].param[w + 0] = Common.CheckRange(page.lfo[c].param[w + 0], 0, (int)parent.info.clockCount);
                page.lfo[c].param[w + 1] = Common.CheckRange(page.lfo[c].param[w + 1], 1, 255);
                page.lfo[c].param[w + 2] = Common.CheckRange(page.lfo[c].param[w + 2], -32768, 32767);
                if (page.lfo[c].param.Count > w + 4)
                {
                    page.lfo[c].param[w + 4] = Common.CheckRange(page.lfo[c].param[w + 4], 0, 4);
                }
                else
                {
                    page.lfo[c].param.Add(0);
                }

                if (page.lfo[c].param[w + 4] != 2) page.lfo[c].param[w + 3] = Math.Abs(Common.CheckRange(page.lfo[c].param[w + 3], 0, 32767));
                else page.lfo[c].param[w + 3] = Common.CheckRange(page.lfo[c].param[w + 3], -32768, 32767);

                if (page.lfo[c].param.Count > w + 5)
                {
                    page.lfo[c].param[w + 5] = Common.CheckRange(page.lfo[c].param[w + 5], 0, 1);
                }
                else
                {
                    page.lfo[c].param.Add(1);
                }
                if (page.lfo[c].param.Count > w + 6)
                {
                    page.lfo[c].param[w + 6] = Common.CheckRange(page.lfo[c].param[w + 6], -32768, 32767);
                    //if (pw.ppg[pw.cpgNum].lfo[c].param[6] == 0) pw.ppg[pw.cpgNum].lfo[c].param[6] = 1;
                }
                else
                {
                    page.lfo[c].param.Add(0);
                }

                //DepthSpeed
                if (page.lfo[c].param.Count > w + 7) page.lfo[c].param[w + 7] = Common.CheckRange(page.lfo[c].param[w + 7], 0, 255);
                else page.lfo[c].param.Add(0);

                //DepthDelta
                if (page.lfo[c].param.Count > w + 8) page.lfo[c].param[w + 8] = Common.CheckRange(page.lfo[c].param[w + 8], -32768, 32767);
                else page.lfo[c].param.Add(0);

                page.lfo[c].sw = true;
                page.lfo[c].isEnd = false;
                page.lfo[c].value = 0;// (page.lfo[c].param[w + 0] == 0) ? page.lfo[c].param[w + 6] : 0;//ディレイ中は振幅補正は適用されない
                page.lfo[c].waitCounter = page.lfo[c].param[w + 0];
                page.lfo[c].direction = page.lfo[c].param[w + 2] < 0 ? -1 : 1;
                if (page.lfo[c].param[w + 4] == 2) page.lfo[c].direction = -1; //矩形の場合は必ず-1(Val1から開始する)をセット
                page.lfo[c].depthWaitCounter = page.lfo[c].param[w + 7];
                page.lfo[c].depth = page.lfo[c].param[w + 3];
                page.lfo[c].depthV2 = page.lfo[c].param[w + 2];
                page.lfo[c].phase = (page.lfo[c].param[w + 0] == 0) ? page.lfo[c].param[w + 6] : 0;

                page.lfo[c].slot = 0;
                if (page.lfo[c].type == eLfoType.Wah)
                {
                    string n = page.lfo[c].param[0].ToString();
                    foreach (char ch in n)
                    {
                        if (ch == '1') page.lfo[c].slot += 1;
                        if (ch == '2') page.lfo[c].slot += 2;
                        if (ch == '3') page.lfo[c].slot += 4;
                        if (ch == '4') page.lfo[c].slot += 8;
                    }
                }
            }
            else
            {
                page.lfo[c].sw = true;
                page.lfo[c].isEnd = false;
                page.lfo[c].value = 0;
                page.lfo[c].waitCounter = -1;
                page.lfo[c].direction = 0;
                page.lfo[c].depthWaitCounter = 0;
                page.lfo[c].depth = 0;
                page.lfo[c].depthV2 = 0;
            }

            mml.args.Add(
                (page.lfo[0].sw ? "P" : "-")
                + (page.lfo[1].sw ? "Q" : "-")
                + (page.lfo[2].sw ? "R" : "-")
                + (page.lfo[3].sw ? "S" : "-"));
            SetDummyData(page, mml);
        }

        public virtual void CmdLfoSwitch(partPage page, MML mml)
        {
            int c = (char)mml.args[0] - 'P';
            int n = (int)mml.args[1];

            //LFOの設定値をチェック
            if (n != 0 && !CheckLFOParam(page, (int)c, mml))
            {
                return;
            }

            page.lfo[c].sw = !(n == 0);

            mml.args.Add(
                (page.lfo[0].sw ? "P" : "-")
                + (page.lfo[1].sw ? "Q" : "-")
                + (page.lfo[2].sw ? "R" : "-")
                + (page.lfo[3].sw ? "S" : "-"));
            SetDummyData(page, mml);
        }


        public virtual void CmdEnvelope(partPage page, MML mml)
        {
            if (!(mml.args[0] is string))
            {
                msgBox.setErrMsg(msg.get("E10010")
                    , mml.line.Lp);

                return;
            }

            string cmd = (string)mml.args[0];

            switch (cmd)
            {
                case "EON":
                    page.envelopeMode = true;
                    break;
                case "EOF":
                    page.envelopeMode = false;
                    if (page.Type == enmChannelType.SSG)
                    {
                        page.beforeVolume = -1;
                    }
                    break;
            }

            SetDummyData(page, mml);
            return;
        }

        public virtual void CmdArpeggio(partPage page, MML mml)
        {
            if (!(mml.args[0] is string))
            {
                msgBox.setErrMsg(msg.get("E10034"), mml.line.Lp);
                return;
            }

            string cmd = (string)mml.args[0];

            switch (cmd)
            {
                case "AP":
                    int inst = (int)mml.args[1];
                    if (parent.instArp.ContainsKey(inst))
                    {
                        page.arpInstrument = inst;
                        page.arpeggioMode = true;
                    }
                    else
                    {
                        //ARPの定義がない場合はエラー
                        msgBox.setErrMsg(msg.get("E10039"), mml.line.Lp);
                        page.arpeggioMode = false;
                    }
                    return;
                case "APON":
                    page.arpeggioMode = true;
                    if (!parent.instArp.ContainsKey(page.arpInstrument))
                    {
                        //ARPの定義がない場合はエラー
                        msgBox.setErrMsg(msg.get("E10040"), mml.line.Lp);
                        page.arpeggioMode = false;
                    }
                    return;
                case "APOF":
                    page.arpeggioMode = false;
                    page.beforeFNum = -1;
                    page.arpDelta = 0;
                    SetKeyOff(page, mml);
                    return;


                case "VP":
                    int vinst = (int)mml.args[1];
                    if (parent.instVArp.ContainsKey(vinst))
                    {
                        page.varpInstrument = vinst;
                        page.varpeggioMode = true;
                    }
                    else
                    {
                        //VARの定義がない場合はエラー
                        msgBox.setErrMsg(msg.get("E10037"), mml.line.Lp);
                        page.varpeggioMode = false;
                    }
                    return;
                case "VPON":
                    page.varpeggioMode = true;
                    if (!parent.instVArp.ContainsKey(page.varpInstrument))
                    {
                        //VARの定義がない場合はエラー
                        msgBox.setErrMsg(msg.get("E10038"), mml.line.Lp);
                        page.varpeggioMode = false;
                    }
                    return;
                case "VPOF":
                    page.varpeggioMode = false;
                    page.varpDelta = 0;
                    SetKeyOff(page, mml);
                    return;
            }

            //ここからCA コマンドの解析

            if (!(mml.args[1] is int))
            {
                //数値指定ではない場合はエラー
                msgBox.setErrMsg(msg.get("E10036"), mml.line.Lp);
                return;
            }

            int n1 = (int)mml.args[1];
            if (!page.commandArpeggio.ContainsKey(n1))
                page.commandArpeggio.Add(n1, new CommandArpeggio());

            switch (cmd)
            {
                case "CAON":
                    page.commandArpeggio[n1].Sw = true;
                    if (!parent.instCommandArp.ContainsKey(page.commandArpeggio[n1].Num))
                    {
                        //ARPの定義がない場合はエラー
                        msgBox.setErrMsg(msg.get("E10042"), mml.line.Lp);
                        page.commandArpeggio[n1].Sw = false;
                    }
                    if (parent.instCommandArp[page.commandArpeggio[n1].Num].Length < 3)
                    {
                        //ARPの定義がない場合はエラー
                        msgBox.setErrMsg(msg.get("E10042"), mml.line.Lp);
                        page.commandArpeggio[n1].Sw = false;
                        return;
                    }

                    page.commandArpeggio[n1].Sync = parent.instCommandArp[page.commandArpeggio[n1].Num][2].dat;
                    if (page.commandArpeggio[n1].Sync != 0)
                    {
                        page.commandArpeggio[n1].Ptr = 0;
                        page.commandArpeggio[n1].LoopPtr = -1;
                        page.commandArpeggio[n1].WaitClock = 1;
                        page.commandArpeggio[n1].WaitCounter = 0;
                    }
                    break;
                case "CAOF":
                    page.commandArpeggio[n1].Sw = false;
                    if (page.commandArpeggio[n1].Sync != 0) page.commandArpeggio[n1].Ptr = -1;
                    break;
                case "CA":
                    if (!(mml.args[2] is int))
                    {
                        //数値指定ではない場合はエラー
                        msgBox.setErrMsg(msg.get("E10041"), mml.line.Lp);
                        page.commandArpeggio[n1].Sw = false;
                        return;
                    }
                    page.commandArpeggio[n1].Sw = true;
                    int n2 = (int)mml.args[2];

                    if (!parent.instCommandArp.ContainsKey(n2))
                    {
                        //ARPの定義がない場合はエラー
                        msgBox.setErrMsg(msg.get("E10041"), mml.line.Lp);
                        page.commandArpeggio[n1].Sw = false;
                        return;
                    }
                    if (parent.instCommandArp[n2].Length < 3)
                    {
                        //ARPの定義がない場合はエラー
                        msgBox.setErrMsg(msg.get("E10041"), mml.line.Lp);
                        page.commandArpeggio[n1].Sw = false;
                        return;
                    }

                    page.commandArpeggio[n1].Num = n2;
                    page.commandArpeggio[n1].Sync = parent.instCommandArp[page.commandArpeggio[n1].Num][2].dat;
                    if (page.commandArpeggio[n1].Sync != 0)
                    {
                        page.commandArpeggio[n1].Ptr = 0;
                        page.commandArpeggio[n1].LoopPtr = -1;
                        page.commandArpeggio[n1].WaitClock = 1;
                        page.commandArpeggio[n1].WaitCounter = 0;
                    }
                    break;
            }

            //SetDummyData(page, mml);
            return;
        }

        public virtual void CmdPhaseReset(partPage page, MML mml)
        {
            string cmd = (string)mml.args[0];

            switch (cmd)
            {
                case "PRON":
                    page.phaseReset = true;
                    break;
                case "PROF":
                    page.phaseReset = false;
                    break;
            }

            //SetDummyData(page, mml);
            return;
        }

        public virtual void CmdPageDirectSend(partPage page, MML mml)
        {
            string cmd = (string)mml.args[0];

            switch (cmd)
            {
                case "PDON":
                    page.spg.DirectSend = true;
                    break;
                case "PDOF":
                    page.spg.DirectSend = false;
                    break;
            }

            //SetDummyData(page, mml);
            return;
        }

        public virtual void CmdHardEnvelope(partPage page, MML mml)
        {
            msgBox.setWrnMsg(msg.get("E10011")
                    , mml.line.Lp);
        }


        public virtual void CmdTotalVolume(partPage page, MML mml)
        {
            msgBox.setErrMsg(msg.get("E10026")
                    , mml.line.Lp);
        }

        public virtual void CmdVolume(partPage page, MML mml)
        {
            int n;
            n = (mml.args != null && mml.args.Count > 0) ? (int)mml.args[0] : page.latestVolume;
            page.volume = Common.CheckRange(n, 0, page.MaxVolume);
            page.latestVolume = n;

            MML vmml = new MML();
            vmml.type = enmMMLType.Volume;
            vmml.args = new List<object>();
            vmml.args.Add(page.volume);
            vmml.line = mml.line;
            SetDummyData(page, vmml);

        }

        public virtual void CmdVolumeUp(partPage page, MML mml)
        {
            int n;
            if (mml.args[0] == null)
            {
                n = GetDefaultRelativeVolume(page, mml);
            }
            else
                n = (int)mml.args[0];

            n = Common.CheckRange(n, 1, page.MaxVolume);
            page.volume += parent.info.volumeRev ? -n : n;
            page.volume = Common.CheckRange(page.volume, 0, page.MaxVolume);

            MML vmml = new MML();
            vmml.type = enmMMLType.Volume;
            vmml.args = new List<object>();
            vmml.args.Add(page.volume);
            vmml.line = mml.line;
            SetDummyData(page, vmml);
        }

        public virtual void CmdVolumeDown(partPage page, MML mml)
        {
            int n;
            if (mml.args[0] == null)
            {
                n = GetDefaultRelativeVolume(page, mml);
            }
            else
                n = (int)mml.args[0];

            n = Common.CheckRange(n, 1, page.MaxVolume);
            page.volume -= parent.info.volumeRev ? -n : n;
            page.volume = Common.CheckRange(page.volume, 0, page.MaxVolume);

            MML vmml = new MML();
            vmml.type = enmMMLType.Volume;
            vmml.args = new List<object>();
            vmml.args.Add(page.volume);
            vmml.line = mml.line;
            SetDummyData(page, vmml);
        }

        public virtual int GetDefaultRelativeVolume(partPage page, MML mml)
        {
            if (relVol.ContainsKey(page.ch)) return relVol[page.ch];
            return 1;
        }

        public virtual void CmdRelativeVolumeSetting(partPage page, MML mml)
        {
            if (relVol.ContainsKey(page.ch))
                relVol.Remove(page.ch);

            relVol.Add(page.ch, (int)mml.args[0]);
        }

        public int octaveMin { get; set; } = 1;
        public int octaveMax { get; set; } = 8;

        public virtual void CmdOctave(partPage page, MML mml)
        {
            int n;
            n = (mml.args != null && mml.args.Count > 0) ? (int)mml.args[0] : page.latestOctave;
            page.octaveNew = Common.CheckRange(n, octaveMin, octaveMax);
            page.latestOctave = n;

            MML vmml = new MML();
            vmml.type = enmMMLType.Octave;
            vmml.args = new List<object>();
            vmml.args.Add(page.octaveNew);
            vmml.line = mml.line;
            SetDummyData(page, vmml);
        }

        public virtual void CmdOctaveUp(partPage page, MML mml)
        {
            page.octaveNew += parent.info.octaveRev ? -1 : 1;
            page.octaveNew = Common.CheckRange(page.octaveNew, octaveMin, octaveMax);

            MML vmml = new MML();
            vmml.type = enmMMLType.Octave;
            vmml.args = new List<object>();
            vmml.args.Add(page.octaveNew);
            vmml.line = mml.line;
            SetDummyData(page, vmml);
        }

        public virtual void CmdOctaveDown(partPage page, MML mml)
        {
            page.octaveNew += parent.info.octaveRev ? 1 : -1;
            page.octaveNew = Common.CheckRange(page.octaveNew, octaveMin, octaveMax);

            MML vmml = new MML();
            vmml.type = enmMMLType.Octave;
            vmml.args = new List<object>();
            vmml.args.Add(page.octaveNew);
            vmml.line = mml.line;
            SetDummyData(page, vmml);
        }


        public virtual void CmdLength(partPage page, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 1, 65535);
            page.length = n;

            SetDummyData(page, mml);
        }

        public virtual void CmdClockLength(partPage page, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 1, 65535);
            page.length = n;
        }


        public virtual void CmdPan(partPage page, MML mml)
        {
            msgBox.setErrMsg(msg.get("E10008")
                    , mml.line.Lp);
        }


        public virtual void CmdDetune(partPage page, MML mml)
        {
            string cmd = (string)mml.args[0];
            int n = (int)mml.args[1];
            switch (cmd)
            {
                case "D":
                    page.detune = n;
                    break;
                case "D>":
                    page.detune += parent.info.octaveRev ? -n : n;
                    break;
                case "D<":
                    page.detune += parent.info.octaveRev ? n : -n;
                    break;
            }
            page.detune = Common.CheckRange(page.detune, -128, 128);

            SetDummyData(page, mml);
        }

        public virtual void CmdDirectMode(partPage page, MML mml)
        {
            msgBox.setErrMsg(msg.get("E10027")
                    , mml.line.Lp);
        }

        public virtual void CmdGatetime(partPage page, MML mml)
        {
            int n = (int)mml.args[0];
            //n = Common.CheckRange(n, -256, 255);
            page.gatetime = n;
            page.gatetimePmode = false;
            page.gatetimeReverse = false;
            if (mml.args.Count > 1)
                page.gatetimeReverse = (bool)mml.args[1];
        }

        public virtual void CmdGatetime2(partPage page, MML mml)
        {
            int n = (int)mml.args[0];
            n = Math.Max(n, 1);
            page.gatetime = n;
            page.gatetimePmode = true;
            page.gatetimeReverse = false;
            if (mml.args.Count > 1)
                page.gatetimeReverse = (bool)mml.args[1];
        }


        public virtual void CmdMode(partPage page, MML mml)
        {
            msgBox.setErrMsg(msg.get("E10009")
                    , mml.line.Lp);
        }

        public virtual void CmdPcmMapSw(partPage page, MML mml)
        {
            msgBox.setErrMsg(msg.get("E10023")
                    , mml.line.Lp);
        }

        public virtual void CmdNoiseToneMixer(partPage page, MML mml)
        {
            msgBox.setErrMsg(msg.get("E10014")
                    , mml.line.Lp);
        }


        public void CmdLoop(partPage page, MML mml)
        {
            //pw.incPos(page);
            parent.loopOffset = (long)parent.dat.Count - parent.dummyCmdCounter;
            parent.loopClock = (long)parent.lClock - parent.dummyCmdClock;
            parent.loopSamples = (long)parent.dSample - parent.dummyCmdSample;
            parent.dummyCmdLoopOffset = (long)parent.dat.Count;
            parent.dummyCmdLoopClock = (long)parent.lClock;
            parent.dummyCmdLoopSamples = (long)parent.dSample;

            if (parent.info.format == enmFormat.XGM)
            {
                parent.OutData(mml, null, 0x7e);
            }

            foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in parent.chips)
            {
                foreach (ClsChip chip in kvp.Value)
                {
                    if (chip == null) continue;

                    foreach (partWork p in chip.lstPartWork)
                    {
                        foreach (partPage pg in p.pg)
                        {
                            pg.reqFreqReset = true;
                            pg.beforeLVolume = -1;
                            pg.beforeRVolume = -1;
                            pg.beforeVolume = -1;
                            pg.pan = 3;
                            pg.beforeTie = false;
                            pg.beforepcmBank = -1;
                            pg.beforepcmEndAddress = -1;
                            pg.beforepcmLoopAddress = -1;
                            pg.beforepcmStartAddress = -1;
                            pg.beforeInstrument = -1;

                            chip.CmdLoopExtProc(pg, mml);
                        }
                    }
                }
            }
        }

        public virtual void CmdLoopExtProc(partPage page, MML mml)
        {
            throw new NotImplementedException("継承先で要実装");
        }


        public virtual void CmdInstrument(partPage page, MML mml)
        {
            throw new NotImplementedException("継承先で要実装");
        }



        public virtual void CmdExtendChannel(partPage page, MML mml)
        {
            msgBox.setWrnMsg(msg.get("E10012")
                    , mml.line.Lp);
        }

        public virtual void CmdVOperator(partPage page, MML mml)
        {
            msgBox.setWrnMsg(msg.get("E10049")
                    , mml.line.Lp);
        }

        public virtual void CmdVGuard(partPage page, MML mml)
        {
            msgBox.setWrnMsg(msg.get("E10050")
                    , mml.line.Lp);
        }

        public virtual void CmdRenpuStart(partPage page, MML mml)
        {
            if (mml.args == null) return;
            int noteCount = (int)mml.args[0];
            int len = (int)page.length;
            int n;
            if (mml.args.Count > 1)
            {
                n = (int)mml.args[1];
                n = Common.CheckRange(n, 1, 65535);
                len = n;
            }
            if (page.stackRenpu.Count > 0)
            {
                len = page.stackRenpu.First().lstRenpuLength[0];
                page.stackRenpu.First().lstRenpuLength.RemoveAt(0);
            }

            ////連符内の音符の長さを作成
            //for (int p = 0; p < noteCount; p++)
            //{
            //    int le = len / noteCount +
            //        (
            //          (len % noteCount) == 0
            //          ? 0
            //          : (
            //              (len % noteCount) > p
            //              ? 1
            //              : 0
            //            )
            //        );

            //    lstRenpuLength.Add(le);
            //}


            ////連符内の音符の長さを作成(残尿方式)
            //// 協力： itoken(@SNDR_SNDL)さん、 ewifan(@ewifan)さん 39 2021/01/16

            //int dc = len / noteCount;
            //float df = 0;
            //int sum = 0;
            //List<int> lstRenpuLength = new List<int>();

            //for (int i = 0; i < noteCount; i++)
            //{
            //    lstRenpuLength.Add(dc);
            //    df += ((float)len / (float)noteCount) - dc;
            //    if (df >= 1.0f)
            //    {
            //        lstRenpuLength[i]++;
            //        df -= 1.0f;
            //    }
            //    sum += lstRenpuLength[i];
            //}
            //if (sum != len)
            //{
            //    lstRenpuLength[lstRenpuLength.Count - 1] += len - sum;
            //}


            ////位置調整のためにローテートを行う
            //int r = noteCount / 2 - (len % noteCount) + ((noteCount / 2) >= (len % noteCount) ? 1 : 0);
            //for (int i = 0; i < Math.Abs(r); i++)
            //{
            //    if (r > 0)
            //    {
            //        int v = lstRenpuLength[0];
            //        for (int j = 0; j < noteCount - 1; j++)
            //        {
            //            lstRenpuLength[j] = lstRenpuLength[j + 1];
            //        }
            //        lstRenpuLength[lstRenpuLength.Count - 1] = v;
            //    }
            //    else
            //    {
            //        int v = lstRenpuLength[lstRenpuLength.Count - 1];
            //        for (int j = noteCount - 1;j>0 ; j--)
            //        {
            //            lstRenpuLength[j] = lstRenpuLength[j - 1];
            //        }
            //        lstRenpuLength[0] = v;
            //    }
            //}

            List<int> lstRenpuLength = f3(len, noteCount);

            page.renpuFlg = true;

            clsRenpu rp = new clsRenpu();
            rp.lstRenpuLength = lstRenpuLength;
            page.stackRenpu.Push(rp);
        }

        /// <summary>
        /// DDA 
        ///   zipperpull(@zipperpull)さんより
        /// </summary>
        /// <returns></returns>
        public List<int> f3(int len, int noteCount)
        {
            // len > noteCount は前提
            int dc = len / noteCount;
            int mod = len % noteCount;
            int di = noteCount / 2;
            //Console.WriteLine("dc:{0} mod:{1} di:{2}", dc, mod, di);

            List<int> lstRenpuLength = new List<int>();

            for (int i = 0; i < noteCount; i++)
            {
                lstRenpuLength.Add(dc);
                di += mod;
                if (di >= noteCount)
                {
                    lstRenpuLength[i]++;
                    di -= noteCount;
                }
            }

            return lstRenpuLength;
        }

        public virtual void CmdRenpuEnd(partPage page, MML mml)
        {
            //popしない内からスタックが空の場合は何もしない。
            if (page.stackRenpu.Count == 0) return;

            page.stackRenpu.Pop();

            if (page.stackRenpu.Count == 0)
            {
                page.renpuFlg = false;
            }

        }


        public virtual void CmdRepeatStart(partPage page, MML mml)
        {
            //何もする必要なし
        }

        public virtual void CmdRepeatEnd(partPage page, MML mml)
        {
            int count = (int)mml.args[0];
            int wkCount;
            int pos = (int)mml.args[1];
            if (mml.args.Count < 3)
            {
                wkCount = count;
                mml.args.Add(wkCount);
            }
            else
            {
                wkCount = (int)mml.args[2];
            }

            wkCount--;
            if (wkCount > 0)
            {
                page.mmlPos = pos - 1;
                mml.args[2] = wkCount;
            }
            else
            {
                mml.args.RemoveAt(2);
            }
        }

        public virtual void CmdRepeatExit(partPage page, MML mml)
        {
            int pos = (int)mml.args[0];
            MML repeatEnd = page.mmlData[pos];
            int wkCount = (int)repeatEnd.args[0];
            if (repeatEnd.args.Count > 2)
            {
                wkCount = (int)repeatEnd.args[2];
            }

            //最終リピート中のみ]に飛ばす
            if (wkCount < 2)
            {
                page.mmlPos = pos - 1;
            }
        }


        public virtual void CmdNote(partWork pw, partPage page, MML mml)
        {
            Note note = (Note)mml.args[0];
            note.trueKeyOn = true;

            if (page.PASwitch)
            {

                if (page.PAPos != page.PAIndex)
                {
                    //休符処理
                    RestProc(page, mml, note.length);
                    note.trueKeyOn = false;

                    if (!note.tieSw)
                    {
                        page.PAIndex++;
                        page.PAIndex %= page.PACount;
                    }
                    return;
                }

                if (!note.tieSw)
                {
                    page.PAIndex++;
                    page.PAIndex %= page.PACount;
                }
            }


            if (note.cmd == 'x')
            {
                note.cmd = page.beforeNote;
                note.shift += page.beforeNoteShift;
            }

            if (note.tDblSw)
            {
                int arpNote = page.arpFreqMode ? 0 : page.arpDelta;
                page.TdA = page.octaveNew * 12
                    + Const.NOTE.IndexOf(note.cmd)
                    + note.shift
                    + page.keyShift
                    + arpNote;
                page.octaveNow = page.octaveNew;
            }

            int ml = note.length;
            if (note.bendSw)
            {
                //ベンドの解析
                //int bendDelayCounter =
                AnalyzeBend(page, mml, note, ml);
            }
            else if (
                (page.MPortamentSwitch != 0 && page.MPortamentOneshotSwitch == 0)
                || (page.MPortamentSwitch == 0 && page.MPortamentOneshotSwitch != 0)
                )
            {
                page.MPortamentOneshotSwitch = 0;

                //直前のノートがない場合は差分値(MPortamentDelta)を使用して作成する
                //(Oneshot(reset)の場合もリセットする)
                if (page.MPortamentLastNote == -1)
                {
                    page.MPortamentLastNote = page.octaveNew * 12 + "czdzefzgzazb".IndexOf(note.cmd) + note.shift + page.MPortamentDelta;
                    page.MPortamentLastNote = Math.Min(Math.Max(page.MPortamentLastNote, 0), 8 * 12 + 11);
                }

                //ベンドコマンドの作成
                note.bendCmd = note.cmd;
                note.bendShift = note.shift;
                page.octaveNow = page.octaveNew;
                page.octaveNew = page.MPortamentLastNote / 12;
                note.cmd = "czdzefzgzazb"[page.MPortamentLastNote % 12];
                note.shift = 0;
                if (note.cmd == 'z')
                {
                    note.cmd = "czdzefzgzazb"[(page.MPortamentLastNote % 12) - 1];
                    note.shift = 1;
                }
                note.bendOctave = new List<MML>();
                if (page.octaveNew != page.octaveNow)
                {
                    MML item = new MML();
                    item.type = enmMMLType.Octave;
                    item.args = new List<object>();
                    item.args.Add(page.octaveNow);
                    note.bendOctave.Add(item);
                }

                //今回のノートを保存
                page.MPortamentLastNote = page.octaveNow * 12 + "czdzefzgzazb".IndexOf(note.bendCmd) + note.bendShift;

                //ベンド実施
                AnalyzeBend(page, mml, note, Math.Min(ml, page.MPortamentLength));

                note.cmd = note.bendCmd;
                note.shift = note.bendShift;
            }
            else
            {
                page.MPortamentOneshotSwitch = 0;
                //今回のノートを保存
                page.MPortamentLastNote = page.octaveNew * 12 + "czdzefzgzazb".IndexOf(note.cmd) + note.shift;
            }


            if (note.length < 1)
            {
                msgBox.setErrMsg(msg.get("E10013")
                    , mml.line.Lp);
                ml = (int)page.length;
            }

            if (page.renpuFlg)
            {
                if (page.stackRenpu.Count > 0)
                {
                    ml = page.stackRenpu.First().lstRenpuLength[0];
                    page.stackRenpu.First().lstRenpuLength.RemoveAt(0);
                }
            }

            //WaitClockの決定
            page.waitCounter = ml;

            if (page.reqFreqReset)
            {
                page.freq = -1;
                page.reqFreqReset = false;
            }

            bool forcedKeyoff = false;
            if (page.waitKeyOnCounter > 0 && !page.tie)
            {
                forcedKeyoff = true;
            }

            page.octaveNow = page.octaveNew;
            page.noteCmd = note.cmd;
            page.shift = note.shift;
            page.pitchShift = note.pitchShift;
            page.tie = note.tieSw;

            //Tone Doubler
            if (note.tDblSw)
            {
                SetToneDoubler(page, mml);
            }
            else
            {
                if (page.TdA != -1)
                {
                    page.TdA = -1;
                    SetToneDoubler(page, mml);
                }
            }

            //発音周波数
            if (page.bendWaitCounter != -1)
            {
                page.octaveNew = page.bendOctave;//
                page.octaveNow = page.bendOctave;//
                page.noteCmd = page.bendNote;
                page.shift = page.bendShift;
                page.pitchShift = 0;//ベンド時は開始時のピッチシフトと到達時のピッチシフトが異なるためfnum計算時にそれを含めている。よってここでは0に設定する
            }

            //gateTimeの決定
            if (!page.PASwitch)
            {
                if (page.gatetimePmode)
                {
                    if (!page.gatetimeReverse)
                        page.waitKeyOnCounter = page.waitCounter * page.gatetime / 8L;
                    else
                        page.waitKeyOnCounter = page.waitCounter - page.waitCounter * (page.gatetime % 8L) / 8L;//リバース時は剰余数で計算
                }
                else
                {
                    if (!page.gatetimeReverse)
                        page.waitKeyOnCounter = page.waitCounter - page.gatetime;
                    else
                        page.waitKeyOnCounter = Math.Abs(page.gatetime);//リバース時は負の数でも同じ動作
                }
            }
            else
            {
                if (page.gatetimePmode)
                {
                    if (!page.gatetimeReverse)
                        page.waitKeyOnCounter = note.arpPartLength * page.gatetime / 8L;
                    else
                        page.waitKeyOnCounter = note.arpPartLength - page.waitCounter * (page.gatetime % 8L) / 8L;//リバース時は剰余数で計算
                }
                else
                {
                    if (!page.gatetimeReverse)
                        page.waitKeyOnCounter = note.arpPartLength - page.gatetime;
                    else
                        page.waitKeyOnCounter = Math.Abs(page.gatetime);//リバース時は負の数でも同じ動作
                }
            }
            //if (page.waitKeyOnCounter > page.waitCounter) page.waitKeyOnCounter = page.waitCounter;
            if (page.waitKeyOnCounter < 1) page.waitKeyOnCounter = 1;

            //RR15timeの決定
            if (page.RR15sw)
            {
                page.waitRR15Counter = -1;
                if (note.nextIsNote)
                {
                    page.waitRR15Counter = page.waitCounter - page.RR15;
                    if (page.waitRR15Counter < 1) page.waitRR15Counter = 1;
                }
            }

            //タイ指定では無い場合はキーオンする
            if (!page.beforeTie)
            {
                MML bk = mml;
                mml = null;
                if (page.envIndex != -1 || page.varpIndex != -1 || forcedKeyoff)
                {
                    SetKeyOff(page, mml);
                }
                SetArpeggioAtKeyOn(page, mml);
                SetVArpeggioAtKeyOn(page, mml);
                SetCommandArpeggioAtKeyOn(page, mml);
                SetEnvelopeAtKeyOn(page, mml);
                SetLfoAtKeyOn(page, mml);
                SetVolume(page,  bk);
                //強制設定
                //pw.ppg[pw.cpgNum].freq = -1;
                //発音周波数の決定
                SetDummyData(page,  bk);
                SetFNum(page, mml);
                //midiむけ
                if (page.bendWaitCounter == -1)
                    ResetTieBend(page, mml);
                //if (!pw.ppg[pw.cpgNum].chip.parent.useSkipPlayCommand)
                //{

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
                mml = bk;
                SetKeyOn(page, mml);
                //}
            }
            else
            {
                MML bk = mml;
                mml = null;
                //強制設定
                //pw.ppg[pw.cpgNum].freq = -1;
                //発音周波数の決定
                SetDummyData(page, bk);
                SetFNum(page, mml);
                SetTieBend(page, mml);
                SetVolume(page, mml);
                mml = bk;

                if (page.Type == enmChannelType.MIDI)
                {
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

                    SetKeyOn(page, mml);
                }
            }

            if (note.chordSw) page.waitCounter = 0;

            page.clockCounter += page.waitCounter;

            page.enableInterrupt = false;
            if (page != pw.cpg)
                page.requestInterrupt = true;

            page.beforeNote = note.cmd;
            page.beforeNoteShift = note.shift;

        }

        public virtual void CmdRest(partPage page, MML mml)
        {
            Rest rest = (Rest)mml.args[0];
            int ml = rest.length;
            RestProc(page, mml, ml);
        }

        private void RestProc(partPage page, MML mml,int ml)
        { 
            if (ml < 1)
            {
                msgBox.setErrMsg(msg.get("E10013")
                    , mml.line.Lp);
                ml = (int)page.length;
            }

            if (page.renpuFlg)
            {
                if (page.stackRenpu.Count > 0)
                {
                    ml = page.stackRenpu.First().lstRenpuLength[0];
                    page.stackRenpu.First().lstRenpuLength.RemoveAt(0);
                }
            }

            //WaitClockの決定
            page.waitCounter = ml;

            //pw.ppg[pw.cpgNum].octaveNow = pw.ppg[pw.cpgNum].octaveNew;
            //pw.ppg[pw.cpgNum].noteCmd = rest.cmd;
            //pw.ppg[pw.cpgNum].shift = 0;
            page.tie = false;

            page.clockCounter += page.waitCounter;
            SetDummyData(page, mml);
            if (page.Type == enmChannelType.FMOPL
                || page.Type == enmChannelType.FMOPN
                || page.Type == enmChannelType.FMOPNex
                || page.Type == enmChannelType.FMOPM
                || page.Type == enmChannelType.FMOPX
                )
            {
                SetFNum(page, mml);
                SetVolume(page, mml);
            }

            //RR15timeの決定
            if (page.RR15sw)
            {
                page.waitRR15Counter = -1;
                if (((Rest)mml.args[0]).nextIsNote)
                {
                    page.waitRR15Counter = page.waitCounter - page.RR15;
                    if (page.waitRR15Counter < 1) page.waitRR15Counter = 1;
                }
            }

            page.enableInterrupt = true;//割り込み許可
        }

        protected virtual void ResetTieBend(partPage page, MML mml)
        {
        }

        protected virtual void SetTieBend(partPage page, MML mml)
        {
        }

        public virtual void CmdLyric(partPage page, MML mml)
        {
            if (mml.args.Count > 1)
            {
                string str = (string)mml.args[0];
                int ml = (int)mml.args[1];

                if (ml < 1)
                {
                    msgBox.setErrMsg(msg.get("E10013")
                        , mml.line.Lp);
                    ml = (int)page.length;
                }

                str = string.Format("[{0}]{1}", parent.dSample.ToString(), str);
                parent.lyric += str;
                //WaitClockの決定
                page.waitCounter = ml;
                page.tie = false;

                page.clockCounter += page.waitCounter;
            }
            SetDummyData(page, mml);
        }

        public virtual void CmdBend(partPage page, MML mml)
        {
            //何もする必要なし
        }

        public virtual void CmdTraceUpdateStack(partPage page, MML mml)
        {
            //
            SetDummyData(page, mml);
        }

        public virtual void CmdKeyOnDelay(partPage page, MML mml)
        {
            msgBox.setErrMsg(msg.get("E10043")
                    , mml.line.Lp);
        }

        public virtual void CmdHardEnvelopeSync(partPage page, MML mml)
        {
            msgBox.setErrMsg(msg.get("E10044")
                    , mml.line.Lp);
        }

        public virtual void CmdModulation(partPage page, MML mml)
        {
            msgBox.setErrMsg(msg.get("E10046")
                    , mml.line.Lp);
        }

        public virtual void CmdPartColor(partPage page, MML mml)
        {
            //
            SetDummyData(page, mml);
        }

        public void CheckInterrupt(partWork pw, partPage page)
        {
            if (!page.requestInterrupt) return;

            //割り込み要求有り

            bool success = false;
            if (page == pw.cpg)
            {
                //自分がカレントページの場合は割り込み成功
                success = true;
            }
            else
            {
                bool enablePage = true;
                foreach (partPage pg in pw.pg)
                {
                    if (pg == page && enablePage)
                    {
                        //自分が見つかり、更に割り込み可能な場合は割り込み成功
                        success = true;
                        break;
                    }

                    //割り込みを禁止するページだった場合は割り込み失敗
                    if (!pg.enableInterrupt)
                    {
                        enablePage = false;
                        break;
                    }

                }
            }

            if (!success)
            {
                page.requestInterrupt = false;//割り込み要求を下げる
                return;
            }

            //割り込み成功
            pw.cpg = page;
        }

        public void LoopPage()
        {
            foreach (partWork pw in lstPartWork)
            {
                foreach (partPage page in pw.pg)
                {
                    FlashPage(pw, page);
                }
            }
        }

        private void FlashPage(partWork pw, partPage page)
        {
            //ページ形式
            if (!page.isLayer)
            {
                //カレントページではない時
                if (page != pw.cpg)
                {
                    //処理しない(但しダイレクトセンドモードが有効な場合はコマンドを送信する)
                    if (pw.spg.DirectSend)
                        parent.OutData(page.sendData);
                    page.sendData.Clear();//送信データクリア
                    return;
                }

                if (page.requestInterrupt)
                {
                    //このページの音色をセットアップする
                    //(但しダイレクトセンドモードが有効な場合はコマンドを送信しない)
                    if (!pw.spg.DirectSend)
                        SetupPageData(pw, page);
                }
            }

            page.requestInterrupt = false;//割り込み要求を下げる

            //このページのデータをセンドする
            parent.OutData(page.sendData);
            page.sendData.Clear();

        }

        public void SOutData(partPage page, MML mml, byte[] cmd, params byte[] data)
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
                                mml.line.Lp.document,
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
                    page.sendData.Add(od);

                    //log.Write(string.Format("cmd {0:x02} :", d));
                }
                //return;
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
                            mml.line.Lp.document,
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
                page.sendData.Add(od);
                //Console.Write("dat {0:x02} :", d);
            }

            //Console.WriteLine("S{0}", mml == null ? "NULL" : mml.type.ToString());
        }

        public void SOutData(partPage page, outDatum od, byte[] cmd, params byte[] data)
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
                            od.linePos.document,
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
                    page.sendData.Add(o);
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
                            od.linePos.document,
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
                page.sendData.Add(o);
                //Console.Write("{0:x02} :", d);
            }
            //Console.WriteLine("");
        }


        public virtual void SetupPageData(partWork pw, partPage page)
        {
        }

        public virtual void MultiChannelCommand(MML mml)
        {
        }


        public virtual string DispRegion(Tuple<string, clsPcm> pcm)
        {
            return "みじっそう";
        }

        public virtual void CmdReversePhase(partPage page, MML mml)
        {
            msgBox.setErrMsg(msg.get("E10045")
                    , mml.line.Lp);
        }

        public void CmdPortament(partPage page, MML mml)
        {
            String cmd = (String)mml.args[0];
            int n1, n2, n3;
            switch (cmd)
            {
                case "PO":
                    n1 = (int)mml.args[1];
                    n2 = (int)mml.args[2];
                    n3 = (int)mml.args[3];
                    page.MPortamentSwitch = n1;
                    page.MPortamentDelta = n2;
                    page.MPortamentLength = Math.Max(n3, 2);
                    page.MPortamentLastNote = -1;
                    break;
                case "POO":
                    page.MPortamentOneshotSwitch = 1;
                    break;
                case "POOR":
                    page.MPortamentOneshotSwitch = 2;
                    page.MPortamentLastNote = -1;
                    break;
                case "POS":
                    n1 = (int)mml.args[1];
                    page.MPortamentSwitch = n1;
                    break;
                case "POR":
                    n1 = (int)mml.args[1];
                    page.MPortamentDelta = n1;
                    page.MPortamentLastNote = -1;
                    break;
                case "POL":
                    n2 = (int)mml.args[1];
                    page.MPortamentLength = Math.Max(n2, 2);
                    break;
            }
        }

        public void CmdPartArpeggio_End(partPage page, MML mml)
        {
            page.PASwitch = false;
        }

        public void CmdPartArpeggio_Start(partPage page, MML mml)
        {
            //設定値系のセット
            page.PASwitch = true;
            page.PAMode = (int)mml.args[0];
            page.PAPos = (int)mml.args[1];
            page.PACount = (int)mml.args[2];

            //ワーク系の初期化
            page.PAIndex = 0;
            page.PAnoteOld = null;
            page.PATie = false;
            page.PATiePos = 0;

        }

        public virtual void CmdRR15(partPage page, MML mml)
        {
            msgBox.setErrMsg(msg.get("E10048")
                    , mml.line.Lp);
        }

        public virtual void SetRR15(partPage page)
        {
            msgBox.setErrMsg(msg.get("E10048")
                    , null);
        }

        public virtual void CmdTLOFS(partPage page, MML mml)
        {
            msgBox.setErrMsg(msg.get("E10048")
                    , mml.line.Lp);
        }
    }

    public class clsPcmDataInfo
    {
        public byte[] totalBuf;
        public int totalHeaderLength;
        public int totalHeadrSizeOfDataPtr;
        public long totalBufPtr;
        public bool use;
    }

    public class ClsChannel
    {
        public string Name;
        public enmChannelType Type;
        public bool chipNumber;
        public int MaxVolume;
    }

    public class Function
    {
        public int Max;
        public int Min;
    }

}
