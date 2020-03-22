using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using musicDriverInterface;

namespace Core
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
        public List<byte[]> pcmDataDirect = new List<byte[]>();
        public byte[][] port;

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
            Stream stream = null;
            Dictionary<string, List<double>> dic = new Dictionary<string, List<double>>();

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

        public int SetEnvelopParamFromInstrument(partWork pw, int n, MML mml)
        {
            if (!parent.instENV.ContainsKey(n))
            {
                msgBox.setErrMsg(string.Format(msg.get("E10000"), n)
                    , mml.line.Lp);
            }
            else
            {
                if (pw.envInstrument != n)
                {
                    pw.envInstrument = n;
                    pw.envIndex = -1;
                    pw.envCounter = -1;
                    for (int i = 0; i < parent.instENV[n].Length; i++)
                    {
                        pw.envelope[i] = parent.instENV[n][i];
                    }
                }
            }
            SetDummyData(pw, mml);
            return n;
        }

        private int AnalyzeBend(partWork pw,MML mml, Note note, int ml)
        {
            int n = -1;
            int bendDelayCounter;
            pw.octaveNow = pw.octaveNew;
            pw.bendOctave = pw.octaveNow;
            pw.bendNote = note.bendCmd;
            pw.bendShift = note.bendShift;
            pw.bendWaitCounter = -1;
            bendDelayCounter = 0;//TODO: bendDelay

            for (int i = 0; i < note.bendOctave.Count; i++)
            {
                switch (note.bendOctave[i].type)
                {
                    case enmMMLType.Octave:
                        n = (int)note.bendOctave[i].args[0];
                        n = Common.CheckRange(n, 1, 8);
                        pw.bendOctave = n;
                        break;
                    case enmMMLType.OctaveUp:
                        pw.incPos();
                        pw.bendOctave += parent.info.octaveRev ? -1 : 1;
                        pw.bendOctave = Common.CheckRange(pw.bendOctave, 1, 8);
                        break;
                    case enmMMLType.OctaveDown:
                        pw.incPos();
                        pw.bendOctave += parent.info.octaveRev ? 1 : -1;
                        pw.bendOctave = Common.CheckRange(pw.bendOctave, 1, 8);
                        break;
                }
            }

            //音符の変化量
            int ed = Const.NOTE.IndexOf(pw.bendNote) + 1 + (pw.bendOctave - 1) * 12 + pw.bendShift;
            ed = Common.CheckRange(ed, 0, 8 * 12 - 1);
            int st = Const.NOTE.IndexOf(note.cmd) + 1 + (pw.octaveNow - 1) * 12 + note.shift;//
            st = Common.CheckRange(st, 0, 8 * 12 - 1);

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
                int toneDoublerShift = GetToneDoublerShift(
                    pw
                    , pw.octaveNow
                    , note.cmd
                    , note.shift);

                //midi向け
                if (!pw.beforeTie)
                {
                    pw.bendStartNote = note.cmd;
                    pw.bendStartOctave = pw.octaveNow;
                    pw.bendStartShift = pw.shift;
                }

                for (int i = 0; i < Math.Abs(delta); i++)
                {
                    bf += wait;
                    tl += wait;
                    GetFNumAtoB(
                        pw
                        ,mml
                        , out int a
                        , pw.octaveNow
                        , note.cmd
                        , note.shift + (i + 0) * Math.Sign(delta)
                        , out int b
                        , pw.octaveNow
                        , note.cmd
                        , note.shift + (i + 1) * Math.Sign(delta)
                        , delta
                        );

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

            return bendDelayCounter;
        }

        private bool CheckLFOParam(partWork pw, int c, MML mml)
        {
            if (pw.lfo[c].param == null)
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

        public virtual void StorePcm(Dictionary<int, clsPcm> newDic, KeyValuePair<int, clsPcm> v, byte[] buf, bool is16bit, int samplerate, params object[] option)
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

            if(parent.info.format== enmFormat.ZGM)
            {
                if (port.Length < 1) return;

                if (parent.ChipCommandSize != 2)
                {
                    if (port[0].Length < 1) return;

                    if (pcmDataEasy != null && pcmDataEasy.Length > 1) pcmDataEasy[1] = port[0][0];
                    for(int i = 0; i < pcmDataDirect.Count; i++)
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
            if (pcmDataDirect.Count > 0)
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
            if (pcmDataDirect.Count > 0)
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
                parent.OutData(mml, pcmDataEasy);

            if (pcmDataDirect.Count < 1) return;

            foreach (byte[] dat in pcmDataDirect)
            {
                if (dat != null && dat.Length > 0)
                    parent.OutData(mml, dat);
            }
        }


        public virtual int GetToneDoublerShift(partWork pw, int octave, char noteCmd, int shift)
        {
            throw new NotImplementedException("継承先で要実装");
        }

        public virtual void SetToneDoubler(partWork pw, MML mml)
        {
            throw new NotImplementedException("継承先で要実装");
        }


        public virtual int GetFNum(partWork pw,MML mml, int octave, char cmd, int shift)
        {
            throw new NotImplementedException("継承先で要実装");
        }

        public virtual void GetFNumAtoB(partWork pw, MML mml
            , out int a, int aOctaveNow, char aCmd, int aShift
            , out int b, int bOctaveNow, char bCmd, int bShift
            , int dir)
        {
            a = GetFNum(pw,mml, aOctaveNow, aCmd, aShift);
            b = GetFNum(pw,mml, bOctaveNow, bCmd, bShift);
        }

        public virtual void SetFNum(partWork pw, MML mml)
        {
            throw new NotImplementedException("継承先で要実装");
        }


        public virtual void SetDummyData(partWork pw, MML mml)
        {
            byte[] cmd;
            if (pw.chip.parent.info.format == enmFormat.ZGM)
            {
                if (pw.chip.parent.ChipCommandSize == 2)
                {
                    cmd = new byte[] { 0x09, 0x00, pw.port[0][0], pw.port[0][1] };
                    parent.dummyCmdCounter += 4;
                }
                else
                {
                    cmd = new byte[] { 0x09, pw.port[0][0] };
                    parent.dummyCmdCounter += 3;
                }

                //Console.WriteLine("SkipAddress:{0:x06} skip:{1:x06}", parent.dat.Count, parent.dummyCmdCounter);
                parent.OutData(mml, cmd, (byte)(pw.chip.ChipNumber));//0x09(zgm):DummyChip (!!CAUTION!!)
            }
            else
            {
                cmd = new byte[] { 0x2f, pw.port[0][0] };
                parent.OutData(mml, cmd, (byte)(pw.chip.ChipNumber));//0x2f(vgm/xgm):DummyChip (!!CAUTION!!)
                parent.dummyCmdCounter += 3;
            }

        }

        public virtual void SetKeyOn(partWork pw, MML mml)
        {
            throw new NotImplementedException("継承先で要実装");
        }

        public virtual void SetKeyOff(partWork pw, MML mml)
        {
            throw new NotImplementedException("継承先で要実装");
        }

        public virtual void SetVolume(partWork pw,MML mml)
        {
            throw new NotImplementedException("継承先で要実装");
        }

        public virtual void SetLfoAtKeyOn(partWork pw,MML mml)
        {
            throw new NotImplementedException("継承先で要実装");
        }

        public virtual void SetEnvelopeAtKeyOn(partWork pw, MML mml)
        {
            if (!pw.envelopeMode)
            {
                pw.envVolume = 0;
                pw.envIndex = -1;
                return;
            }

            //System.Diagnostics.Debug.WriteLine("");
            //System.Diagnostics.Debug.WriteLine("EnvKeyOn");

            pw.envIndex = 0;
            pw.envCounter = 0;
            int maxValue = pw.MaxVolume;// (pw.envelope[8] == (int)enmChipType.RF5C164) ? 255 : 15;

            while (pw.envCounter == 0 && pw.envIndex != -1)
            {
                switch (pw.envIndex)
                {
                    case 0: // AR phase
                        //System.Diagnostics.Debug.Write("EnvAR");
                        pw.envCounter = pw.envelope[2];
                        if (pw.envelope[2] > 0 && pw.envelope[1] < maxValue)
                        {
                            pw.envVolume = pw.envelope[1];
                            //System.Diagnostics.Debug.Write(string.Format(":{0}",pw.envVolume));
                        }
                        else
                        {
                            pw.envVolume = maxValue;
                            pw.envIndex++;
                            //System.Diagnostics.Debug.Write(string.Format(":next", pw.envVolume));
                        }
                        //System.Diagnostics.Debug.WriteLine("");
                        break;
                    case 1: // DR phase
                        //System.Diagnostics.Debug.Write("EnvDR");
                        pw.envCounter = pw.envelope[3];
                        if (pw.envelope[3] > 0 && pw.envelope[4] < maxValue)
                        {
                            pw.envVolume = maxValue;
                            //System.Diagnostics.Debug.Write(string.Format(":{0}", pw.envVolume));
                        }
                        else
                        {
                            pw.envVolume = pw.envelope[4];
                            pw.envIndex++;
                            //System.Diagnostics.Debug.Write(string.Format(":next", pw.envVolume));
                        }
                        //System.Diagnostics.Debug.WriteLine("");
                        break;
                    case 2: // SR phase
                        //System.Diagnostics.Debug.Write("EnvSR");
                        pw.envCounter = pw.envelope[5];
                        if (pw.envelope[5] > 0 && pw.envelope[4] != 0)
                        {
                            pw.envVolume = pw.envelope[4];
                            //System.Diagnostics.Debug.Write(string.Format(":{0}", pw.envVolume));
                        }
                        else
                        {
                            pw.envVolume = 0;
                            pw.envIndex = -1;
                            //System.Diagnostics.Debug.Write(string.Format(":end", pw.envVolume));
                        }
                        //System.Diagnostics.Debug.WriteLine("");
                        break;
                }
            }
        }



        public virtual void CmdY(partWork pw, MML mml)
        {
            throw new NotImplementedException("継承先で要実装");
        }

        public virtual void CmdTempo(partWork pw, MML mml)
        {
            parent.info.tempo = (int)mml.args[0];
            if (parent.info.format == enmFormat.VGM|| parent.info.format == enmFormat.ZGM)
            {
                parent.info.samplesPerClock = Information.VGM_SAMPLE_PER_SECOND * 60.0 * 4.0 / (parent.info.tempo * parent.info.clockCount);
            }
            else
            {
                parent.info.samplesPerClock = parent.info.xgmSamplesPerSecond * 60.0 * 4.0 / (parent.info.tempo * parent.info.clockCount);
            }

            SetDummyData(pw, mml);
        }

        public virtual void CmdKeyShift(partWork pw, MML mml)
        {
            int n = (int)mml.args[0];
            pw.keyShift = Common.CheckRange(n, -128, 128);
            SetDummyData(pw, mml);
        }

        public virtual void CmdAddressShift(partWork pw, MML mml)
        {
            int sign = (int)mml.args[0];
            int n = (int)mml.args[1];

            pw.addressShift = (sign == 0) ? n : (pw.addressShift + (n * sign));
            if (pw.addressShift < 0) pw.addressShift = 0;
        }

        public virtual void CmdMIDICh(partWork pw, MML mml)
        {
            msgBox.setErrMsg(msg.get("E10028")
                    , mml.line.Lp);
        }

        public virtual void CmdMIDIControlChange(partWork pw, MML mml)
        {
            msgBox.setErrMsg(msg.get("E10031")
                    , mml.line.Lp);
        }

        public virtual void CmdVelocity(partWork pw, MML mml)
        {
            msgBox.setErrMsg(msg.get("E10029")
                    , mml.line.Lp);
        }

        public virtual void CmdNoise(partWork pw, MML mml)
        {
            msgBox.setErrMsg(msg.get("E10002")
                    , mml.line.Lp);
        }

        public virtual void CmdDCSGCh3Freq(partWork pw, MML mml)
        {
            msgBox.setErrMsg(msg.get("E10032")
                    , mml.line.Lp);
        }

        public virtual void CmdSusOnOff(partWork pw, MML mml)
        {
            msgBox.setErrMsg(msg.get("E10022")
                    , mml.line.Lp);
        }


        public virtual void CmdMPMS(partWork pw, MML mml)
        {
            msgBox.setErrMsg(msg.get("E10003")
                    , mml.line.Lp);
        }

        public virtual void CmdMAMS(partWork pw, MML mml)
        {
            msgBox.setErrMsg(msg.get("E10004")
                    , mml.line.Lp);
        }

        public virtual void CmdLfo(partWork pw, MML mml)
        {
            if (mml.args[0] is string)
            {
                if ((string)mml.args[0] == "MAMS")
                {
                    CmdMAMS(pw, mml);
                    return;
                }
                if ((string)mml.args[0] == "MPMS")
                {
                    CmdMPMS(pw, mml);
                    return;
                }
            }

            int c = (char)mml.args[0] - 'P';
            eLfoType t = (char)mml.args[1] == 'T' ? eLfoType.Tremolo
                : ((char)mml.args[1] == 'V' ? eLfoType.Vibrato : eLfoType.Hardware);

            pw.lfo[c].type = t;
            pw.lfo[c].sw = false;
            pw.lfo[c].isEnd = true;
            pw.lfo[c].param = new List<int>();
            for (int i = 2; i < mml.args.Count; i++)
            {
                if(mml.args[i] is int)
                pw.lfo[c].param.Add((int)mml.args[i]);
            }

            if (pw.lfo[c].type == eLfoType.Tremolo || pw.lfo[c].type == eLfoType.Vibrato)
            {
                if (pw.lfo[c].param.Count < 4)
                {
                    msgBox.setErrMsg(msg.get("E10005")
                    , mml.line.Lp);
                    return;
                }
                if (pw.lfo[c].param.Count > 9)
                {
                    msgBox.setErrMsg(msg.get("E10006")
                    , mml.line.Lp);
                    return;
                }

                pw.lfo[c].param[0] = Common.CheckRange(pw.lfo[c].param[0], 0, (int)parent.info.clockCount);
                pw.lfo[c].param[1] = Common.CheckRange(pw.lfo[c].param[1], 1, 255);
                pw.lfo[c].param[2] = Common.CheckRange(pw.lfo[c].param[2], -32768, 32767);
                if (pw.lfo[c].param.Count > 4)
                {
                    pw.lfo[c].param[4] = Common.CheckRange(pw.lfo[c].param[4], 0, 4);
                }
                else
                {
                    pw.lfo[c].param.Add(0);
                }

                if (pw.lfo[c].param[4] != 2) pw.lfo[c].param[3] = Math.Abs(Common.CheckRange(pw.lfo[c].param[3], 0, 32767));
                else pw.lfo[c].param[3] = Common.CheckRange(pw.lfo[c].param[3], -32768, 32767);

                if (pw.lfo[c].param.Count > 5)
                {
                    pw.lfo[c].param[5] = Common.CheckRange(pw.lfo[c].param[5], 0, 1);
                }
                else
                {
                    pw.lfo[c].param.Add(1);
                }
                if (pw.lfo[c].param.Count > 6)
                {
                    pw.lfo[c].param[6] = Common.CheckRange(pw.lfo[c].param[6], -32768, 32767);
                    //if (pw.lfo[c].param[6] == 0) pw.lfo[c].param[6] = 1;
                }
                else
                {
                    pw.lfo[c].param.Add(0);
                }

                //DepthSpeed
                if (pw.lfo[c].param.Count > 7) pw.lfo[c].param[7] = Common.CheckRange(pw.lfo[c].param[7], 0, 255);
                else pw.lfo[c].param.Add(0);

                //DepthDelta
                if (pw.lfo[c].param.Count > 8) pw.lfo[c].param[8] = Common.CheckRange(pw.lfo[c].param[8], -32768, 32767);
                else pw.lfo[c].param.Add(0);

                pw.lfo[c].sw = true;
                pw.lfo[c].isEnd = false;
                pw.lfo[c].value = (pw.lfo[c].param[0] == 0) ? pw.lfo[c].param[6] : 0;//ディレイ中は振幅補正は適用されない
                pw.lfo[c].waitCounter = pw.lfo[c].param[0];
                pw.lfo[c].direction = pw.lfo[c].param[2] < 0 ? -1 : 1;
                if (pw.lfo[c].param[4] == 2) pw.lfo[c].direction = -1; //矩形の場合は必ず-1(Val1から開始する)をセット
                pw.lfo[c].depthWaitCounter = pw.lfo[c].param[7];
                pw.lfo[c].depth = pw.lfo[c].param[3];
                pw.lfo[c].depthV2 = pw.lfo[c].param[2];
            }
            else
            {
                pw.lfo[c].sw = true;
                pw.lfo[c].isEnd = false;
                pw.lfo[c].value = 0;
                pw.lfo[c].waitCounter = -1;
                pw.lfo[c].direction = 0;
                pw.lfo[c].depthWaitCounter = 0;
                pw.lfo[c].depth = 0;
                pw.lfo[c].depthV2 = 0;
            }

            mml.args.Add(
                (pw.lfo[0].sw ? "P" : "-")
                + (pw.lfo[1].sw ? "Q" : "-")
                + (pw.lfo[2].sw ? "R" : "-")
                + (pw.lfo[3].sw ? "S" : "-"));
            SetDummyData(pw, mml);
        }

        public virtual void CmdLfoSwitch(partWork pw, MML mml)
        {
            int c = (char)mml.args[0] - 'P';
            int n = (int)mml.args[1];

            //LFOの設定値をチェック
            if (n != 0 && !CheckLFOParam(pw, (int)c, mml))
            {
                return;
            }

            pw.lfo[c].sw = !(n == 0);

            mml.args.Add(
                (pw.lfo[0].sw ? "P" : "-")
                + (pw.lfo[1].sw ? "Q" : "-")
                + (pw.lfo[2].sw ? "R" : "-")
                + (pw.lfo[3].sw ? "S" : "-"));
            SetDummyData(pw, mml);
        }


        public virtual void CmdEnvelope(partWork pw, MML mml)
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
                    pw.envelopeMode = true;
                    break;
                case "EOF":
                    pw.envelopeMode = false;
                    if (pw.Type == enmChannelType.SSG)
                    {
                        pw.beforeVolume = -1;
                    }
                    break;
            }

            SetDummyData(pw, mml);
            return;
        }

        public virtual void CmdHardEnvelope(partWork pw, MML mml)
        {
            msgBox.setWrnMsg(msg.get("E10011")
                    , mml.line.Lp);
        }


        public virtual void CmdTotalVolume(partWork pw, MML mml)
        {
            msgBox.setErrMsg(msg.get("E10026")
                    , mml.line.Lp);
        }

        public virtual void CmdVolume(partWork pw, MML mml)
        {
            int n;
            n = (mml.args != null && mml.args.Count > 0) ? (int)mml.args[0] : pw.latestVolume;
            pw.volume = Common.CheckRange(n, 0, pw.MaxVolume);
            pw.latestVolume = n;

            MML vmml = new MML();
            vmml.type = enmMMLType.Volume;
            vmml.args = new List<object>();
            vmml.args.Add(pw.volume);
            vmml.line = mml.line;
            SetDummyData(pw, vmml);

        }

        public virtual void CmdVolumeUp(partWork pw, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 1, pw.MaxVolume);
            pw.volume += parent.info.volumeRev ? -n : n;
            pw.volume = Common.CheckRange(pw.volume, 0, pw.MaxVolume);

            MML vmml = new MML();
            vmml.type = enmMMLType.Volume;
            vmml.args = new List<object>();
            vmml.args.Add(pw.volume);
            vmml.line = mml.line;
            SetDummyData(pw, vmml);
        }

        public virtual void CmdVolumeDown(partWork pw, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 1, pw.MaxVolume);
            pw.volume -= parent.info.volumeRev ? -n : n;
            pw.volume = Common.CheckRange(pw.volume, 0, pw.MaxVolume);

            MML vmml = new MML();
            vmml.type = enmMMLType.Volume;
            vmml.args = new List<object>();
            vmml.args.Add(pw.volume);
            vmml.line = mml.line;
            SetDummyData(pw, vmml);
        }


        public virtual void CmdOctave(partWork pw, MML mml)
        {
            int n;
            n = (mml.args != null && mml.args.Count > 0) ? (int)mml.args[0] : pw.latestOctave;
            pw.octaveNew = Common.CheckRange(n, 1, 8);
            pw.latestOctave = n;

            MML vmml = new MML();
            vmml.type = enmMMLType.Octave;
            vmml.args = new List<object>();
            vmml.args.Add(pw.octaveNew);
            vmml.line = mml.line;
            SetDummyData(pw, vmml);
        }

        public virtual void CmdOctaveUp(partWork pw, MML mml)
        {
            pw.octaveNew += parent.info.octaveRev ? -1 : 1;
            pw.octaveNew = Common.CheckRange(pw.octaveNew, 1, 8);

            MML vmml = new MML();
            vmml.type = enmMMLType.Octave;
            vmml.args = new List<object>();
            vmml.args.Add(pw.octaveNow);
            vmml.line = mml.line;
            SetDummyData(pw, vmml);
        }

        public virtual void CmdOctaveDown(partWork pw, MML mml)
        {
            pw.octaveNew += parent.info.octaveRev ? 1 : -1;
            pw.octaveNew = Common.CheckRange(pw.octaveNew, 1, 8);

            MML vmml = new MML();
            vmml.type = enmMMLType.Octave;
            vmml.args = new List<object>();
            vmml.args.Add(pw.octaveNow);
            vmml.line = mml.line;
            SetDummyData(pw, vmml);
        }


        public virtual void CmdLength(partWork pw, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 1, 65535);
            pw.length = n;

            SetDummyData(pw, mml);
        }

        public virtual void CmdClockLength(partWork pw, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 1, 65535);
            pw.length = n;
        }


        public virtual void CmdPan(partWork pw, MML mml)
        {
            msgBox.setErrMsg(msg.get("E10008")
                    , mml.line.Lp);
        }


        public virtual void CmdDetune(partWork pw, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, -127, 127);
            pw.detune = n;
            SetDummyData(pw, mml);
        }

        public virtual void CmdDirectMode(partWork pw, MML mml)
        {
            msgBox.setErrMsg(msg.get("E10027")
                    , mml.line.Lp);
        }

        public virtual void CmdGatetime(partWork pw, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 0, 255);
            pw.gatetime = n;
            pw.gatetimePmode = false;
        }

        public virtual void CmdGatetime2(partWork pw, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 1, 8);
            pw.gatetime = n;
            pw.gatetimePmode = true;
        }


        public virtual void CmdMode(partWork pw, MML mml)
        {
            msgBox.setErrMsg(msg.get("E10009")
                    , mml.line.Lp);
        }

        public virtual void CmdPcmMapSw(partWork pw, MML mml)
        {
            msgBox.setErrMsg(msg.get("E10023")
                    , mml.line.Lp);
        }

        public virtual void CmdNoiseToneMixer(partWork pw, MML mml)
        {
            msgBox.setErrMsg(msg.get("E10014")
                    , mml.line.Lp);
        }


        public void CmdLoop(partWork pw, MML mml)
        {
            pw.incPos();
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
                        p.reqFreqReset = true;
                        p.beforeLVolume = -1;
                        p.beforeRVolume = -1;
                        p.beforeVolume = -1;
                        p.pan = new dint(3);
                        p.beforeTie = false;
                        
                        chip.CmdLoopExtProc(p,mml);
                    }
                }
            }
        }

        public virtual void CmdLoopExtProc(partWork pw, MML mml)
        {
            throw new NotImplementedException("継承先で要実装");
        }


        public virtual void CmdInstrument(partWork pw, MML mml)
        {
            throw new NotImplementedException("継承先で要実装");
        }



        public virtual void CmdExtendChannel(partWork pw, MML mml)
        {
            msgBox.setWrnMsg(msg.get("E10012")
                    , mml.line.Lp);
        }


        public virtual void CmdRenpuStart(partWork pw, MML mml)
        {
            List<int> lstRenpuLength = new List<int>();
            int noteCount = (int)mml.args[0];
            int len = (int)pw.length;
            if (mml.args.Count > 1)
            {
                int n = (int)mml.args[1];
                n = Common.CheckRange(n, 1, 65535);
                len = n;
            }
            if (pw.stackRenpu.Count > 0)
            {
                len = pw.stackRenpu.First().lstRenpuLength[0];
                pw.stackRenpu.First().lstRenpuLength.RemoveAt(0);
            }
            //TODO: ネストしている場合と、数値していないの場合

            //連符内の音符の長さを作成
            for (int p = 0; p < noteCount; p++)
            {
                int le = len / noteCount +
                    (
                      (len % noteCount) == 0
                      ? 0
                      : (
                          (len % noteCount) > p
                          ? 1
                          : 0
                        )
                    );

                lstRenpuLength.Add(le);
            }

            pw.renpuFlg = true;

            clsRenpu rp = new clsRenpu();
            rp.lstRenpuLength = lstRenpuLength;
            pw.stackRenpu.Push(rp);
        }

        public virtual void CmdRenpuEnd(partWork pw, MML mml)
        {
            //popしない内からスタックが空の場合は何もしない。
            if (pw.stackRenpu.Count == 0) return;

            pw.stackRenpu.Pop();

            if (pw.stackRenpu.Count == 0)
            {
                pw.renpuFlg = false;
            }

        }


        public virtual void CmdRepeatStart(partWork pw, MML mml)
        {
            //何もする必要なし
        }

        public virtual void CmdRepeatEnd(partWork pw, MML mml)
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
                pw.mmlPos=pos-1;
                mml.args[2] = wkCount;
            }
            else
            {
                mml.args.RemoveAt(2);
            }
        }

        public virtual void CmdRepeatExit(partWork pw, MML mml)
        {
            int pos = (int)mml.args[0];
            MML repeatEnd = pw.mmlData[pos];
            int wkCount = (int)repeatEnd.args[0];
            if (repeatEnd.args.Count > 2)
            {
                wkCount = (int)repeatEnd.args[2];
            }

            //最終リピート中のみ]に飛ばす
            if (wkCount < 2)
            {
                pw.mmlPos = pos-1;
            }
        }


        public virtual void CmdNote(partWork pw, MML mml)
        {
            Note note = (Note)mml.args[0];
            int ml = 0;

            if (note.tDblSw)
            {
                pw.TdA = pw.octaveNew * 12
                    + Const.NOTE.IndexOf(note.cmd)
                    + note.shift
                    + pw.keyShift;
                pw.octaveNow = pw.octaveNew;
            }

            ml = note.length;

            //ベンドの解析
            int bendDelayCounter = 0;
            if (note.bendSw)
            {
                bendDelayCounter = AnalyzeBend(pw, mml, note, ml);
            }


            if (note.length < 1)
            {
                msgBox.setErrMsg(msg.get("E10013")
                    , mml.line.Lp);
                ml = (int)pw.length;
            }

            if (pw.renpuFlg)
            {
                if (pw.stackRenpu.Count > 0)
                {
                    ml = pw.stackRenpu.First().lstRenpuLength[0];
                    pw.stackRenpu.First().lstRenpuLength.RemoveAt(0);
                }
            }

            //WaitClockの決定
            pw.waitCounter = ml;

            if (pw.reqFreqReset)
            {
                pw.freq = -1;
                pw.reqFreqReset = false;
            }

            pw.octaveNow = pw.octaveNew;
            pw.noteCmd = note.cmd;
            pw.shift = note.shift;
            pw.tie = note.tieSw;

            //Tone Doubler
            if (note.tDblSw)
            {
                SetToneDoubler(pw, mml);
            }
            else
            {
                if (pw.TdA != -1)
                {
                    pw.TdA = -1;
                    SetToneDoubler(pw, mml);
                }
            }

            //発音周波数
            if (pw.bendWaitCounter != -1)
            {
                pw.octaveNew = pw.bendOctave;//
                pw.octaveNow = pw.bendOctave;//
                pw.noteCmd = pw.bendNote;
                pw.shift = pw.bendShift;
            }

            //タイ指定では無い場合はキーオンする
            if (!pw.beforeTie)
            {
                if (pw.envIndex != -1)
                {
                    SetKeyOff(pw, mml);
                }
                SetEnvelopeAtKeyOn(pw,mml);
                SetLfoAtKeyOn(pw, mml);
                SetVolume(pw, mml);
                //強制設定
                //pw.freq = -1;
                //発音周波数の決定
                SetFNum(pw,mml);
                //midiむけ
                if(pw.bendWaitCounter==-1)
                    ResetTieBend(pw, mml);
                if (!pw.chip.parent.useSkipPlayCommand)
                {
                    SetKeyOn(pw, mml);
                }
            }
            else
            {
                //強制設定
                //pw.freq = -1;
                //発音周波数の決定
                SetDummyData(pw, mml);
                SetFNum(pw,mml);
                SetTieBend(pw,mml);
                SetVolume(pw, mml);
            }

            //gateTimeの決定
            if (pw.gatetimePmode)
                pw.waitKeyOnCounter = pw.waitCounter * pw.gatetime / 8L;
            else
                pw.waitKeyOnCounter = pw.waitCounter - pw.gatetime;
            if (pw.waitKeyOnCounter < 1) pw.waitKeyOnCounter = 1;

            pw.clockCounter += pw.waitCounter;
        }

        protected virtual void ResetTieBend(partWork pw, MML mml)
        {
        }

        protected virtual void SetTieBend(partWork pw, MML mml)
        {
        }

        public virtual void CmdRest(partWork pw, MML mml)
        {
            Rest rest = (Rest)mml.args[0];
            int ml = 0;

            ml = rest.length;

            if (rest.length < 1)
            {
                msgBox.setErrMsg(msg.get("E10013")
                    , mml.line.Lp);
                ml = (int)pw.length;
            }

            //WaitClockの決定
            pw.waitCounter = ml;

            //pw.octaveNow = pw.octaveNew;
            //pw.noteCmd = rest.cmd;
            //pw.shift = 0;
            pw.tie = false;

            pw.clockCounter += pw.waitCounter;
            SetDummyData(pw, mml);
        }

        public virtual void CmdLyric(partWork pw, MML mml)
        {
            string str = (string)mml.args[0];
            int ml = (int)mml.args[1];

            if (ml < 1)
            {
                msgBox.setErrMsg(msg.get("E10013")
                    , mml.line.Lp);
                ml = (int)pw.length;
            }

            str = string.Format("[{0}]{1}", parent.dSample.ToString(), str);
            parent.lyric += str;
            //WaitClockの決定
            pw.waitCounter = ml;
            pw.tie = false;

            pw.clockCounter += pw.waitCounter;
            SetDummyData(pw, mml);
        }

        public virtual void CmdBend(partWork pw,MML mml)
        {
            //何もする必要なし
        }


        public virtual void MultiChannelCommand(MML mml)
        { }


        public virtual string DispRegion(clsPcm pcm)
        {
            return "みじっそう";
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
