using musicDriverInterface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mml2vgmIDEx64
{
    public class MuapManager
    {
        public static string wrkMMLFullPath;
        public bool Stopped = false;
        public string lyrics = "";
        public int comlength = 0;

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

        public MuapManager(string compilerPath, string driverPath, Action<string> disp, Setting setting)
        {
            try
            {
                musicDriverInterface.Log.writeLine = writeLine;
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

            compiler = im.GetCompiler("muapDotNET.Compiler.Compiler");
            driver = im.GetDriver("muapDotNET.Driver.Driver");

            if (compiler == null || driver == null)
            {
                throw new Exception("インスタンスの生成に失敗しました。");
            }
        }

        public MmlDatum[] compileFromSrcText(string srcText, string wrkMMLFullPath, string srcFileFullPath, Point point, bool doTrace)
        {
            if (!ok) return null;

            MuapManager.wrkMMLFullPath = wrkMMLFullPath;
            compiler.Init();
            //if (doTrace) compiler.SetCompileSwitch("IDE");

            //if (point != Point.Empty)
            //{
                //compiler.SetCompileSwitch(string.Format("SkipPoint=R{0}:C{1}", point.Y, point.X));
            //}

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

            //compiler.SetCompileSwitch(string.Format("ORIGPATH={0}", wrkMMLFullPath));
            //compiler.SetCompileSwitch(
            //    "MoonDriverOption=-i"
            //    , string.Format( "MoonDriverOption={0}",srcFileFullPath)
            //    );

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
    , Action<ChipDatum> OPNAWrite
    , Action<ChipDatum> OPNBWrite
    , Action<ChipDatum> CS4231Write
    , Func<byte,byte> CS4231Read
    , Func<byte[]> CS4231EMS_GetCrntMapBuf
    , iDriver.dlgEMS_Map CS4231EMS_Map
    , Func<ushort> CS4231EMS_GetPageMap
    , iDriver.dlgEMS_GetHandleName CS4231EMS_GetHandleName
    , iDriver.dlgEMS_SetHandleName CS4231EMS_SetHandleName
    , iDriver.dlgEMS_AllocMemory CS4231EMS_AllocMemory
    , Action<long, int> OPNAWaitSend
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
                (Func<byte,byte>)CS4231Read,//CS4231 のレジスタ読み込み
                (Func<byte[]>)CS4231EMS_GetCrntMapBuf,
                (iDriver.dlgEMS_Map)CS4231EMS_Map,
                (Func<ushort>)CS4231EMS_GetPageMap,
                (iDriver.dlgEMS_GetHandleName)CS4231EMS_GetHandleName,
                (iDriver.dlgEMS_SetHandleName)CS4231EMS_SetHandleName,
                (iDriver.dlgEMS_AllocMemory)CS4231EMS_AllocMemory,
                null,//TONES.DTA
                0,//Sound Device Mode
                null//@LABEL ptr
            };

            List<ChipAction> lca = new List<ChipAction>();
            MuapDriverChipAction ca;
            ca = new MuapDriverChipAction(OPNAWrite); lca.Add(ca);
            ca = new MuapDriverChipAction(OPNBWrite); lca.Add(ca);
            ca = new MuapDriverChipAction(CS4231Write); lca.Add(ca);
            driver.Init(
                lca
                , mdrBuf
                , appendFileReaderCallback
                , addOp
            );

        }

        private void writeLine(LogLevel lvl,string msg)
        {
            if (lvl > LogLevel.INFO) return;
            disp(string.Format("{0}:{1}",lvl,msg));
        }

        public class MuapDriverChipAction : ChipAction
        {
            private Action<ChipDatum> oPNAWrite;

            public MuapDriverChipAction(Action<ChipDatum> oPNAWrite)
            {
                this.oPNAWrite = oPNAWrite;
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
                oPNAWrite?.Invoke(cd);
            }
        }

        public void MSTART(int v)
        {
            driver.MusicSTART(v);
            object[] work = (object[])driver.GetWork();
            chipRegister.setCS4231FIFOBuf(0, (byte[])work[0]);
            //chipRegister.setCS4231Int0bEnt(0, (Action)work[1]);
            if (work.Length > 2 && work[2] is List<Tuple<int, string>>)
            {
                List<Tuple<int, string>> Lyrics = (List<Tuple<int, string>>)work[2];
            }
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

            //歌詞取得
            List<Tuple<string, string>> work = driver.GetTags();
            if (work != null && work.Count == 2)
            {
                string ly = work[0].Item2;
                string len = work[1].Item2;
                int ilen = -1;
                if (int.TryParse(len, out ilen))
                {
                    lyrics = ly;
                    comlength = ilen;
                }
            }
        }

        public List<Tuple<string, string>> GetTags()
        {
            return driver.GetTags();
        }

    }
}
