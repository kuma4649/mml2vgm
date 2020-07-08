using musicDriverInterface;
using System;
using System.Drawing;
using System.IO;
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
        }

        //private void WriteLog(LogLevel arg1, string arg2)
        //{
        //disp?.Invoke(arg2);
        //}

        public void InitDriver(
            string fileName
            , Action<ChipDatum> oPNAWrite
            , Action<long, int> oPNAWaitSend
            , bool notSoundBoard2
            , musicDriverInterface.MmlDatum[] mubBuf
            , bool isLoadADPCM
            , bool loadADPCMOnly)
        {
            driver.Init(
                fileName
                , oPNAWrite
                , oPNAWaitSend
                , mubBuf
                , new object[] {
                      notSoundBoard2
                    , isLoadADPCM
                    , loadADPCMOnly
                });
        }

        public void MSTART(int v)
        {
            driver.MusicSTART(v);
        }

        public void MSTOP()
        {
            driver.MusicSTOP();
        }

        public void StartRendering(int sampleRate, int yM2608ClockValue)
        {
            driver.StartRendering(sampleRate, new Tuple<string, int>[] { new Tuple<string, int>("YM2608", yM2608ClockValue) });
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
    }
}
