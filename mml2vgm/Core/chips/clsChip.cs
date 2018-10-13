using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

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
            get
            {
                return _ChMax;
            }
        }
        protected int _ChMax = 0;

        public int ChipID
        {
            get
            {
                return _ChipID;
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

        public bool IsSecondary
        {
            get
            {
                return _IsSecondary;
            }

            set
            {
                _IsSecondary = value;
            }
        }

        public bool SupportReversePartWork = false;
        public bool ReversePartWork = false;

        protected bool _IsSecondary = false;

        public Function Envelope = null;

        protected int _ChipID = -1;
        protected ClsVgm parent;
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
        public byte[] pcmData = null;




        public ClsChip(ClsVgm parent, int chipID, string initialPartName, string stPath, bool isSecondary)
        {
            this.parent = parent;
            this._ChipID = chipID;
            this.stPath = stPath;
            this.IsSecondary = IsSecondary;
            MakeFNumTbl();
        }

        protected Dictionary<string, List<double>> MakeFNumTbl()
        {
            //for (int i = 0; i < noteTbl.Length; i++)
            //{
            //    FNumTbl[0][i] = (int)(Math.Round(((144.0 * noteTbl[i] * Math.Pow(2.0, 20) / Frequency) / Math.Pow(2.0, (4 - 1))), MidpointRounding.AwayFromZero));
            //}
            //FNumTbl[0][12] = FNumTbl[0][0] * 2;

            string fn = string.Format("FNUM_{0}.txt", Name);
            Stream stream = null;
            Dictionary<string, List<double>> dic = new Dictionary<string, List<double>>();

            if (File.Exists(Path.Combine(stPath, fn)))
            {
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
                msgBox.setErrMsg(string.Format("エンベロープ定義に指定された音色番号({0})が存在しません。", n)
                    , mml.line.Fn
                    , mml.line.Num);
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

            return n;
        }

        private int AnalyzeBend(partWork pw, Note note, int ml)
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
                for (int i = 0; i < Math.Abs(delta); i++)
                {
                    bf += wait;
                    tl += wait;
                    GetFNumAtoB(
                        pw
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
                msgBox.setErrMsg("指定されたLFOのパラメータが未指定です。"
                    , mml.line.Fn
                    , mml.line.Num);
                return false;
            }

            return true;
        }




        public virtual void InitChip()
        {
            throw new NotImplementedException("継承先で要実装");
        }

        public virtual void InitPart(ref partWork pw)
        {
            throw new NotImplementedException("継承先で要実装");
        }

        public virtual void StorePcm(Dictionary<int, clsPcm> newDic, KeyValuePair<int, clsPcm> v, byte[] buf, bool is16bit, int samplerate, params object[] option)
        {
            pcmDataInfo = null;
        }

        public virtual int GetToneDoublerShift(partWork pw, int octave, char noteCmd, int shift)
        {
            throw new NotImplementedException("継承先で要実装");
        }

        public virtual void SetToneDoubler(partWork pw)
        {
            throw new NotImplementedException("継承先で要実装");
        }

        public virtual int GetFNum(partWork pw, int octave, char cmd, int shift)
        {
            throw new NotImplementedException("継承先で要実装");
        }

        public virtual void GetFNumAtoB(partWork pw
            , out int a, int aOctaveNow, char aCmd, int aShift
            , out int b, int bOctaveNow, char bCmd, int bShift
            , int dir)
        {
            a = GetFNum(pw, aOctaveNow, aCmd, aShift);
            b = GetFNum(pw, bOctaveNow, bCmd, bShift);
        }

        public virtual void SetFNum(partWork pw)
        {
            throw new NotImplementedException("継承先で要実装");
        }

        public virtual void SetKeyOn(partWork pw)
        {
            throw new NotImplementedException("継承先で要実装");
        }

        public virtual void SetKeyOff(partWork pw)
        {
            throw new NotImplementedException("継承先で要実装");
        }

        public virtual void SetVolume(partWork pw)
        {
            throw new NotImplementedException("継承先で要実装");
        }

        public virtual void SetPCMDataBlock()
        {
            throw new NotImplementedException("継承先で要実装");
        }

        public virtual void SetLfoAtKeyOn(partWork pw)
        {
            throw new NotImplementedException("継承先で要実装");
        }

        public virtual void SetEnvelopeAtKeyOn(partWork pw)
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



        public virtual void CmdY(partWork pw, MML mml)
        {
            throw new NotImplementedException("継承先で要実装");
        }

        public virtual void CmdTempo(partWork pw, MML mml)
        {
            parent.info.tempo = (int)mml.args[0];
            if (parent.info.format == enmFormat.VGM)
            {
                parent.info.samplesPerClock = Information.VGM_SAMPLE_PER_SECOND * 60.0 * 4.0 / (parent.info.tempo * parent.info.clockCount);
            }
            else
            {
                parent.info.samplesPerClock = parent.info.xgmSamplesPerSecond * 60.0 * 4.0 / (parent.info.tempo * parent.info.clockCount);
            }
        }

        public virtual void CmdKeyShift(partWork pw, MML mml)
        {
            int n = (int)mml.args[0];
            pw.keyShift = Common.CheckRange(n, -128, 128);
        }

        public virtual void CmdNoise(partWork pw, MML mml)
        {
            msgBox.setErrMsg("このチャンネルではwコマンドは使用できません。"
                    , mml.line.Fn
                    , mml.line.Num
                    );
        }

        public virtual void CmdMPMS(partWork pw, MML mml)
        {
            msgBox.setErrMsg("このチャンネルではMPMSコマンドは使用できません。"
                    , mml.line.Fn
                    , mml.line.Num
                    );
        }

        public virtual void CmdMAMS(partWork pw, MML mml)
        {
            msgBox.setErrMsg("このチャンネルではMAMSコマンドは使用できません。"
                    , mml.line.Fn
                    , mml.line.Num
                    );
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
            for (int i = 2; i < mml.args.Count; i++) pw.lfo[c].param.Add((int)mml.args[i]);

            if (pw.lfo[c].type == eLfoType.Tremolo || pw.lfo[c].type == eLfoType.Vibrato)
            {
                if (pw.lfo[c].param.Count < 4)
                {
                    msgBox.setErrMsg("LFOの設定に必要なパラメータが足りません。"
                    , mml.line.Fn
                    , mml.line.Num
                    );
                    return;
                }
                if (pw.lfo[c].param.Count > 7)
                {
                    msgBox.setErrMsg("LFOの設定に可能なパラメータ数を超えて指定されました。"
                    , mml.line.Fn
                    , mml.line.Num
                    );
                    return;
                }

                pw.lfo[c].param[0] = Common.CheckRange(pw.lfo[c].param[0], 0, (int)parent.info.clockCount);
                pw.lfo[c].param[1] = Common.CheckRange(pw.lfo[c].param[1], 1, 255);
                pw.lfo[c].param[2] = Common.CheckRange(pw.lfo[c].param[2], -32768, 32787);
                pw.lfo[c].param[3] = Math.Abs(Common.CheckRange(pw.lfo[c].param[3], 0, 32787));
                if (pw.lfo[c].param.Count > 4)
                {
                    pw.lfo[c].param[4] = Common.CheckRange(pw.lfo[c].param[4], 0, 4);
                }
                else
                {
                    pw.lfo[c].param.Add(0);
                }
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
                    pw.lfo[c].param[6] = Common.CheckRange(pw.lfo[c].param[6], -32768, 32787);
                    if (pw.lfo[c].param[6] == 0) pw.lfo[c].param[6] = 1;
                }
                else
                {
                    pw.lfo[c].param.Add(1);
                }

                pw.lfo[c].sw = true;
                pw.lfo[c].isEnd = false;
                pw.lfo[c].value = (pw.lfo[c].param[0] == 0) ? pw.lfo[c].param[6] : 0;//ディレイ中は振幅補正は適用されない
                pw.lfo[c].waitCounter = pw.lfo[c].param[0];
                pw.lfo[c].direction = pw.lfo[c].param[2] < 0 ? -1 : 1;
            }
            else
            {
                pw.lfo[c].sw = true;
                pw.lfo[c].isEnd = false;
                pw.lfo[c].value = 0;
                pw.lfo[c].waitCounter = -1;
                pw.lfo[c].direction = 0;
            }
        }

        public virtual void CmdLfoSwitch(partWork pw, MML mml)
        {
            int c = (char)mml.args[0] - 'P';
            int n = (int)mml.args[1];

            //LFOの設定値をチェック
            if (n != 0 && !CheckLFOParam(pw, (int)c,mml))
            {
                return;
            }

            pw.lfo[c].sw = !(n == 0);

        }

        public virtual void CmdOctaveDown(partWork pw, MML mml)
        {
            pw.octaveNew += parent.info.octaveRev ? 1 : -1;
            pw.octaveNew = Common.CheckRange(pw.octaveNew, 1, 8);
        }

        public virtual void CmdOctaveUp(partWork pw, MML mml)
        {
            pw.octaveNew += parent.info.octaveRev ? -1 : 1;
            pw.octaveNew = Common.CheckRange(pw.octaveNew, 1, 8);
        }

        public virtual void CmdOctave(partWork pw, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 1, 8);
            pw.octaveNew = n;
        }

        public virtual void CmdVolumeDown(partWork pw, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 1, pw.MaxVolume);
            pw.volume -= n;
            pw.volume = Common.CheckRange(pw.volume, 0, pw.MaxVolume);

        }

        public virtual void CmdVolumeUp(partWork pw, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 1, pw.MaxVolume);
            pw.volume += n;
            n = Common.CheckRange(n, 0, pw.MaxVolume);

        }

        public virtual void CmdVolume(partWork pw, MML mml)
        {
            int n = (int)mml.args[0];
            pw.volume = Common.CheckRange(n, 0, pw.MaxVolume);
        }

        public virtual void CmdTotalVolume(partWork pw, MML mml)
        {
            msgBox.setErrMsg("このチャンネルではMPMSコマンドは使用できません。"
                    , mml.line.Fn
                    , mml.line.Num
                    );
        }

        public virtual void CmdLength(partWork pw, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 1, 65535);
            pw.length = n;
        }

        public virtual void CmdClockLength(partWork pw, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 1, 65535);
            pw.length = n;
        }

        public virtual void CmdPan(partWork pw, MML mml)
        {
            msgBox.setErrMsg("このチャンネルではpコマンドは使用できません。"
                    , mml.line.Fn
                    , mml.line.Num
                    );
        }

        public virtual void CmdDetune(partWork pw, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, -127, 127);
            pw.detune = n;
        }

        public virtual void CmdGatetime2(partWork pw, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 1, 8);
            pw.gatetime = n;
            pw.gatetimePmode = true;
        }

        public virtual void CmdGatetime(partWork pw, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 0, 255);
            pw.gatetime = n;
            pw.gatetimePmode = false;
        }

        public virtual void CmdMode(partWork pw, MML mml)
        {
            msgBox.setErrMsg("このチャンネルではmコマンドは使用できません。"
                    , mml.line.Fn
                    , mml.line.Num
                    );
        }

        public void CmdLoop(partWork pw, MML mml)
        {
            pw.incPos();
            parent.loopOffset = (long)parent.dat.Count;
            parent.loopClock = (long)parent.lClock;
            parent.loopSamples = (long)parent.dSample;

            if (parent.info.format == enmFormat.XGM)
            {
                parent.OutData(0x7e);
            }

            foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in parent.chips)
            {
                foreach (ClsChip chip in kvp.Value)
                {
                    foreach (partWork p in chip.lstPartWork)
                    {
                        p.reqFreqReset = true;
                        p.beforeLVolume = -1;
                        p.beforeRVolume = -1;
                        p.beforeVolume = -1;
                        p.pan = new dint(3);
                        p.beforeTie = false;

                        CmdLoopExtProc(p,mml);
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

        public virtual void CmdEnvelope(partWork pw, MML mml)
        {
            if (!(mml.args[0] is string))
            {
                msgBox.setErrMsg("Envelopeの解析に失敗しました。"
                    , mml.line.Fn
                    , mml.line.Num);

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
            return;
        }

        public virtual void CmdHardEnvelope(partWork pw, MML mml)
        {
            msgBox.setWrnMsg("このパートではEHコマンドは使用できません。"
                    , mml.line.Fn
                    , mml.line.Num
                    );
        }

        public virtual void CmdExtendChannel(partWork pw, MML mml)
        {
            msgBox.setWrnMsg("このパートは効果音モードに対応したチャンネルが指定されていないため、Eコマンドは無視されます。"
                    , mml.line.Fn
                    , mml.line.Num
                    );
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
                bendDelayCounter = AnalyzeBend(pw, note, ml);
            }


            if (note.length < 1)
            {
                msgBox.setErrMsg("負の音長が指定されました。"
                    , mml.line.Fn
                    , mml.line.Num
                    );
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
            SetToneDoubler(pw);

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
                SetEnvelopeAtKeyOn(pw);
                SetLfoAtKeyOn(pw);
                SetVolume(pw);
                //強制設定
                //pw.freq = -1;
                //発音周波数の決定
                SetFNum(pw);
                SetKeyOn(pw);
            }
            else
            {
                //強制設定
                //pw.freq = -1;
                //発音周波数の決定
                SetFNum(pw);
                SetVolume(pw);
            }

            //gateTimeの決定
            if (pw.gatetimePmode)
                pw.waitKeyOnCounter = pw.waitCounter * pw.gatetime / 8L;
            else
                pw.waitKeyOnCounter = pw.waitCounter - pw.gatetime;
            if (pw.waitKeyOnCounter < 1) pw.waitKeyOnCounter = 1;

            pw.clockCounter += pw.waitCounter;
        }

        public virtual void CmdRest(partWork pw, MML mml)
        {
            Rest rest = (Rest)mml.args[0];
            int ml = 0;

            ml = rest.length;

            if (rest.length < 1)
            {
                msgBox.setErrMsg("負の音長が指定されました。"
                    , mml.line.Fn
                    , mml.line.Num
                    );
                ml = (int)pw.length;
            }

            //WaitClockの決定
            pw.waitCounter = ml;

            //pw.octaveNow = pw.octaveNew;
            //pw.noteCmd = rest.cmd;
            //pw.shift = 0;
            pw.tie = false;

            pw.clockCounter += pw.waitCounter;
        }

        public virtual void CmdBend(partWork pw,MML mml)
        {
            //何もする必要なし
        }

        public virtual void CmdNoiseToneMixer(partWork pw, MML mml)
        {
            msgBox.setErrMsg("このパートではPコマンドは使用できません。"
                , mml.line.Fn
                , mml.line.Num);
        }

        public virtual void MultiChannelCommand()
        { }
    }

    public class clsPcmDataInfo
    {
        public byte[] totalBuf;
        public long totalBufPtr;
        public bool use;
    }

    public class ClsChannel
    {
        public string Name;
        public enmChannelType Type;
        public bool isSecondary;
        public int MaxVolume;
    }

    public class Function
    {
        public int Max;
        public int Min;
    }

}
