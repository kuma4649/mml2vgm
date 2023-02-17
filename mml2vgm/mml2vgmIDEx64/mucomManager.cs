using musicDriverInterface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace mml2vgmIDE
{
    public class mucomManager
    {
        private iCompiler compiler = null;
        private iDriver driver = null;
        private iPreprocessor preprocessor = null;
        private bool ok = false;
        private Action<string> disp = null;
        private Setting setting = null;
        private InstanceMarker im = null;

        public string wrkMUCFullPath { get; private set; }
        public uint OPMClock = 3579545;
        public bool SSGExtend = false;

        public bool Stopped = false;

        public mucomManager(string compilerPath, string driverPath, string preprocessorPath, Action<string> disp, Setting setting)
        {
            try
            {
                this.disp = disp;
                this.setting = setting;

                makeInstance(compilerPath, driverPath, preprocessorPath);

                ok = true;

            }
            catch
            {
                this.compiler = null;
                this.driver = null;
                this.preprocessor = null;
                ok = false;
                throw;
            }

        }

        private void makeInstance(string compilerPath, string driverPath, string preprocessorPath)
        {
            im = new InstanceMarker();
            im.LoadCompilerDll(compilerPath);
            im.LoadDriverDll(driverPath);
            im.LoadPreprocessorDll(preprocessorPath);

            compiler = im.GetCompiler("mucomDotNET.Compiler.Compiler");
            driver = im.GetDriver("mucomDotNET.Driver.Driver");
            preprocessor = im.GetPreprocessor("M98DotNETcore.M98");

            if (compiler == null || driver == null || preprocessor == null)
            {
                throw new Exception("インスタンスの生成に失敗しました。");
            }
        }

        public void Rendering()
        {
            driver.Rendering();
            this.Stopped = (driver.GetStatus() < 1);
        }

        //private void WriteLog(LogLevel arg1, string arg2)
        //{
        //disp?.Invoke(arg2);
        //}

        public void InitDriver(
            string fileName
            , List<Action<ChipDatum>> oPNAWrite
            , List<Action<byte[],int,int>> WritePCMData
            , Action<long, int> oPNAWaitSend
            , bool notSoundBoard2
            , musicDriverInterface.MmlDatum[] mubBuf
            , bool isLoadADPCM
            , bool loadADPCMOnly)
        {
            List<ChipAction> lca = new List<ChipAction>();
            mucomChipAction ca;
            ca = new mucomChipAction(oPNAWrite[0], null, oPNAWaitSend); lca.Add(ca);
            ca = new mucomChipAction(oPNAWrite[1], null, null); lca.Add(ca);
            ca = new mucomChipAction(oPNAWrite[2], WritePCMData[2], null); lca.Add(ca);
            ca = new mucomChipAction(oPNAWrite[3], WritePCMData[3], null); lca.Add(ca);
            ca = new mucomChipAction(oPNAWrite[4], null, null); lca.Add(ca);

            driver.Init(
                lca
                //fileName
                //, oPNAWrite
                //, WritePCMData
                //, oPNAWaitSend
                , mubBuf
                , null
                , new object[] {
                      notSoundBoard2
                    , isLoadADPCM
                    , loadADPCMOnly
                    , fileName
                });
        }

        public class mucomChipAction : ChipAction
        {
            private Action<ChipDatum> _Write;
            private Action<byte[], int, int> _WritePCMData;
            private Action<long, int> _WaitSend;

            public mucomChipAction(Action<ChipDatum> Write, Action<byte[], int, int> WritePCMData, Action<long, int> WaitSend)
            {
                _Write = Write;
                _WritePCMData = WritePCMData;
                _WaitSend = WaitSend;
            }

            public override string GetChipName()
            {
                throw new NotImplementedException();
            }

            public override void WaitSend(long t1, int t2)
            {
                _WaitSend?.Invoke(t1, t2);
            }

            public override void WritePCMData(byte[] data, int startAddress, int endAddress)
            {
                _WritePCMData?.Invoke(data, startAddress, endAddress);
            }

            public override void WriteRegister(ChipDatum cd)
            {
                _Write?.Invoke(cd);
            }
        }

        public void MSTART(int v)
        {
            driver.MusicSTART(v);
        }

        public void MSTOP()
        {
            driver.MusicSTOP();
        }

        public void StartRendering(int sampleRate, int yM2608ClockValue, int yM2610ClockValue, int yM2151ClockValue)
        {
            driver.StartRendering(sampleRate, new Tuple<string, int>[] {
                new Tuple<string, int>("YM2608", yM2608ClockValue)
                ,new Tuple<string, int>("YM2608", yM2608ClockValue)
                ,new Tuple<string, int>("YM2610", yM2610ClockValue)
                ,new Tuple<string, int>("YM2610", yM2610ClockValue)
                ,new Tuple<string, int>("YM2151", yM2151ClockValue)
            });
        }

        public MmlDatum[] compile(string srcMUCFullPath, string wrkMUCFullPath)
        {
            if (!ok) return null;

            this.wrkMUCFullPath = wrkMUCFullPath;
            compiler.Init();
            compiler.SetCompileSwitch("IDE");

            MmlDatum[] ret;
            musicDriverInterface.Log.writeMethod = disp;
            musicDriverInterface.Log.off = 0;
            if (!setting.other.LogWarning)
            {
                musicDriverInterface.Log.off = (int)musicDriverInterface.LogLevel.WARNING;
            }
            if (setting.other.LogLevel == (int)LogLevel.INFO) musicDriverInterface.Log.level = LogLevel.INFO;
            else if (setting.other.LogLevel == (int)LogLevel.DEBUG) musicDriverInterface.Log.level = LogLevel.DEBUG;
            else if (setting.other.LogLevel == (int)LogLevel.TRACE) musicDriverInterface.Log.level = LogLevel.TRACE;
            using (FileStream sourceMML = new FileStream(srcMUCFullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                ret = compiler.Compile(sourceMML, appendFileReaderCallback);// wrkMUCFullPath, disp);
            return ret;
        }

        public MmlDatum[] compileFromSrcText(string srcText, string wrkMUCFullPath, Point poi)
        {
            if (!ok) return null;

            this.wrkMUCFullPath = wrkMUCFullPath;
            compiler.Init();
            compiler.SetCompileSwitch("IDE");

            if (poi != Point.Empty)
            {
                compiler.SetCompileSwitch(string.Format("SkipPoint=R{0}:C{1}", poi.Y, poi.X));
            }

            MmlDatum[] ret;
            musicDriverInterface.Log.writeMethod = disp;
            musicDriverInterface.Log.off = 0;
            if (!setting.other.LogWarning)
            {
                musicDriverInterface.Log.off = (int)musicDriverInterface.LogLevel.WARNING;
            }
            if (setting.other.LogLevel == (int)LogLevel.INFO) musicDriverInterface.Log.level = LogLevel.INFO;
            else if (setting.other.LogLevel == (int)LogLevel.DEBUG) musicDriverInterface.Log.level = LogLevel.DEBUG;
            else if (setting.other.LogLevel == (int)LogLevel.TRACE) musicDriverInterface.Log.level = LogLevel.TRACE;

            byte[] src = Encoding.GetEncoding(932).GetBytes(srcText);
            using (MemoryStream sourceMML = new MemoryStream(src))
                ret = compiler.Compile(sourceMML, appendFileReaderCallback);// wrkMUCFullPath, disp);
            return ret;
        }


        private Stream appendFileReaderCallback(string arg)
        {

            string fn = Path.Combine(
                wrkMUCFullPath// Path.GetDirectoryName(wrkMUCFullPath)
                , arg
                );
            
            if (!File.Exists(fn)) return null;

            FileStream strm;
            try
            {
                strm = new FileStream(fn, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            catch (IOException)
            {
                strm = null;
            }

            return strm;
        }

        public CompilerInfo GetCompilerInfo()
        {
            if (!ok) return null;
            return compiler.GetCompilerInfo();
        }

        public string preprocess(string src)
        {
            musicDriverInterface.Log.writeMethod = disp;
            disp("M98 を開始");

            byte[] sbAry = Encoding.GetEncoding(932).GetBytes(src);
            string ret;

            using (MemoryStream sourceMML = new MemoryStream(sbAry))
            using (MemoryStream destMML = new MemoryStream())
            {
                preprocessor.Preprocess(sourceMML, destMML, appendFileReaderCallback);
                ret = Encoding.GetEncoding(932).GetString(destMML.GetBuffer());
            }

            disp("M98 を完了");
            return ret;
        }

        public int GetNowLoopCounter()
        {
            int lp = driver.GetNowLoopCounter();
            return (lp < 0) ? 0 : lp;
        }


        public Tuple<string,string>[] GetTagFromBuf(byte[] buf, iEncoding enc)
        {
            List<Tuple<string, string>> ret = new List<Tuple<string, string>>();
            uint magic;
            uint tagdata=0;
            uint tagsize=0;

            magic = Common.getLE32(buf, 0x0000);

            if (magic == 0x3843554d) //'MUC8'
            {
                tagdata = Common.getLE32(buf, 0x000c);
                tagsize = Common.getLE32(buf, 0x0010);
            }
            else if (magic == 0x3842554d) //'MUB8'
            {
                tagdata = Common.getLE32(buf, 0x000c);
                tagsize = Common.getLE32(buf, 0x0010);
            }
            else if (magic == 0x6250756d)
            {
                tagdata = Common.getLE32(buf, 0x0012);
                tagsize = Common.getLE32(buf, 0x0016);
            }
            else
            {
            }

            if (tagdata == 0) return null;
            List<byte> lb = new List<byte>();
            for (int i = 0; i < tagsize; i++)
            {
                lb.Add(buf[tagdata + i]);
            }

            return GetTagsByteArray(lb.ToArray(), enc);
        }

        private Tuple<string, string>[] GetTagsByteArray(byte[] buf, iEncoding enc)
        {
            var text = enc.GetStringFromSjisArray(buf) //Encoding.GetEncoding("shift_jis").GetString(buf)
                .Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Where(x => x.IndexOf("#") == 0);

            List<Tuple<string, string>> tags = new List<Tuple<string, string>>();
            foreach (string v in text)
            {
                try
                {
                    int p = v.IndexOf(' ');
                    string tag = "";
                    string ele = "";
                    if (p >= 0)
                    {
                        tag = v.Substring(1, p).Trim().ToLower();
                        ele = v.Substring(p + 1).Trim();
                        Tuple<string, string> item = new Tuple<string, string>(tag, ele);
                        tags.Add(item);
                    }
                }
                catch { }
            }

            SetDriverOptionFromTags(tags);

            return tags.ToArray();
        }

        private void SetDriverOptionFromTags(List<Tuple<string, string>> tags)
        {
            if (tags == null) return;
            if (tags.Count < 1) return;

            foreach (var tag in tags)
            {
                if (tag == null) continue;
                if (tag.Item1.ToLower().Trim() == "opmclockmode")
                {
                    if (!string.IsNullOrEmpty(tag.Item2))
                    {
                        string val = tag.Item2.ToLower().Trim();

                        OPMClock = 3579545;
                        if (val == "x68000" || val == "x68k" || val == "x68" || val == "x" || val == "4000000" || val == "x680x0")
                        {
                            OPMClock = 4000000;
                        }
                    }
                }
                else if (tag.Item1.ToLower().Trim() == "ssgextend")
                {
                    if (!string.IsNullOrEmpty(tag.Item2))
                    {
                        string val = tag.Item2.ToLower().Trim();

                        SSGExtend = false;
                        if (val == "on" || val == "yes" || val == "y" || val == "1" || val == "true" || val == "t")
                        {
                            SSGExtend = true;
                        }
                    }
                }

            }
        }

        public Tuple<string, string>[] GetTagFromBuf(MmlDatum[] buf, iEncoding enc)
        {
            OPMClock = 3579545;

            List<byte> lbuf = new List<byte>();
            foreach(MmlDatum v in buf)
            {
                lbuf.Add((byte)v.dat);
            }
            return GetTagFromBuf(lbuf.ToArray(), enc);
        }

        public void SetSolo(Tuple<int, int, int> ch)
        {
            if (driver == null || ch == null) return;
            driver.SetDriverSwitch("AllMute", true);
            if(ch.Item1==-1)
                driver.SetDriverSwitch("SetMute", 0, ch.Item3, 0, false);
            else
            driver.SetDriverSwitch("SetMute", ch.Item1, ch.Item2, ch.Item3, false);
        }
    }
}
