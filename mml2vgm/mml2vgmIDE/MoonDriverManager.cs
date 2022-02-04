using musicDriverInterface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mml2vgmIDE
{
    public class MoonDriverManager
    {
        public static string wrkMMLFullPath;
        public bool Stopped = false;

        private string compilerPath;
        private string driverPath;
        private Action<string> disp;
        private Setting setting;
        private iCompiler compiler = null;
        private iDriver driver = null;
        private bool ok = false;
        private InstanceMarker im = null;
        private GD3Tag currentGD3Tag;
        private ChipRegister chipRegister;

        public MoonDriverManager(string compilerPath, string driverPath, Action<string> disp, Setting setting)
        {
            try
            {
                this.compilerPath = compilerPath;
                this.driverPath = driverPath;
                this.disp = disp;
                this.setting = setting;

                makeInstance(compilerPath, driverPath);

                ok = true;
            }
            catch
            {
                this.compiler = null;
                this.driver = null;
                ok = false;
                throw;
            }
        }

        private void makeInstance(string compilerPath, string driverPath)
        {
            im = new InstanceMarker();
            im.LoadCompilerDll(compilerPath);
            im.LoadDriverDll(driverPath);

            compiler = im.GetCompiler("MoonDriverDotNET.Compiler.Compiler");
            driver = im.GetDriver("MoonDriverDotNET.Driver.Driver");

            if (compiler == null || driver == null)
            {
                throw new Exception("インスタンスの生成に失敗しました。");
            }
        }

        public MmlDatum[] compileFromSrcText(string srcText, string wrkMMLFullPath, string srcFileFullPath, Point point, bool doTrace)
        {
            if (!ok) return null;

            MoonDriverManager.wrkMMLFullPath = wrkMMLFullPath;
            compiler.Init();
            if (doTrace) compiler.SetCompileSwitch("IDE");

            if (point != Point.Empty)
            {
                compiler.SetCompileSwitch(string.Format("SkipPoint=R{0}:C{1}", point.Y, point.X));
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

            musicDriverInterface.Log.level = LogLevel.INFO;

            compiler.SetCompileSwitch(string.Format("ORIGPATH={0}", wrkMMLFullPath));
            compiler.SetCompileSwitch(
                "MoonDriverOption=-i"
                , string.Format( "MoonDriverOption={0}",srcFileFullPath)
                );

            byte[] src = Encoding.GetEncoding(932).GetBytes(srcText);
            using (MemoryStream sourceMML = new MemoryStream(src))
                ret = compiler.Compile(sourceMML, appendFileReaderCallback);// wrkMUCFullPath, disp);

            currentGD3Tag = compiler.GetGD3TagInfo(src);
            //foreach(MmlDatum d in ret)
            //{
            //    if(d.type == enmMMLType.Note)
            //    {
            //        Console.WriteLine("{0} {1}", d.linePos.row, d.linePos.col);
            //        ;
            //    }
            //}
            return ret;
        }

        private static Stream appendFileReaderCallback(string arg)
        {
            string fn;
            fn = arg;
            if (!File.Exists(fn)) fn = Path.Combine(wrkMMLFullPath, fn);

            //if (envPmd != null)
            //{
            //    int i = 0;
            //    while (!File.Exists(fn) && i < envPmd.Length)
            //    {
            //        fn = Path.Combine(
            //            envPmd[i++]
            //            , Path.GetFileName(arg)
            //            );
            //    }
            //}

            FileStream strm = null;
            try
            {
                if (File.Exists(fn))
                {
                    strm = new FileStream(fn, FileMode.Open, FileAccess.Read, FileShare.Read);
                }
            }
            catch (IOException)
            {
                Log.WriteLine(LogLevel.ERROR, string.Format("File({0}) not found.", arg));
                strm = null;
            }

            return strm;
        }

        public CompilerInfo GetCompilerInfo()
        {
            if (!ok) return null;
            return compiler.GetCompilerInfo();
        }

        public void InitDriver(
    string fileName
    , Action<ChipDatum> OPL4Write
    , Action<long, int> OPL4WaitSend
    , musicDriverInterface.MmlDatum[] mdrBuf
    , ChipRegister chipRegister
    )
        {

            musicDriverInterface.Log.writeMethod = disp;
            musicDriverInterface.Log.off = 0;
            setting = Audio.setting;
            this.chipRegister = chipRegister;

            int jumpIndex = -1;
            if (compiler != null)
            {
                CompilerInfo ci = compiler.GetCompilerInfo();
                if (ci.jumpClock != -1) jumpIndex = ci.jumpClock;
            }

            object[] addOp = new object[]{
                fileName,
                //(Func<string, Stream>)appendFileReaderCallback,
                (double)Common.DataSequenceSampleRate,
                jumpIndex
            };

            List<ChipAction> lca = new List<ChipAction>();
            MoonDriverChipAction ca = new MoonDriverChipAction(OPL4Write);lca.Add(ca);
            driver.Init(
                //fileName
                //, OPL4Write
                lca
                , mdrBuf
                , appendFileReaderCallback
                //,OPL4WaitSend
                , addOp
            );

        }

        public class MoonDriverChipAction : ChipAction
        {
            private Action<ChipDatum> oPL4Write;

            public MoonDriverChipAction(Action<ChipDatum> oPL4Write)
            {
                this.oPL4Write = oPL4Write;
            }

            public override string GetChipName()
            {
                throw new NotImplementedException();
            }

            public override void WaitSend(long t1, int t2)
            {
                throw new NotImplementedException();
            }

            public override void WritePCMData(byte[] data, int startAddress, int endAddress)
            {
                throw new NotImplementedException();
            }

            public override void WriteRegister(ChipDatum cd)
            {
                oPL4Write?.Invoke(cd);
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

        public void StartRendering(int DataSequenceSampleRate, int YMF278BClockValue)
        {
            driver.StartRendering(DataSequenceSampleRate
                , new Tuple<string, int>[] { new Tuple<string, int>("YMF278B", YMF278BClockValue) });
        }

        public void Rendering()
        {
            driver.Rendering();
            this.Stopped = (driver.GetStatus() < 1);
        }

    }
}
