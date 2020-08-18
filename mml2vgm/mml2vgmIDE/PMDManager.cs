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
    public class PMDManager
    {
        private iCompiler compiler = null;
        private iDriver driver = null;
        private bool ok = false;
        private Action<string> disp = null;
        private Setting setting = null;
        private InstanceMarker im = null;
        public static string wrkMMLFullPath;
        private static string[] envPmd = null;
        private static string[] envPmdOpt = null;

        public PMDManager(string compilerPath, string driverPath, Action<string> disp, Setting setting)
        {
            try
            {
                this.disp = disp;
                this.setting = setting;

                makeInstance(compilerPath, driverPath);

                ok = true;

                EnvironmentE env = new EnvironmentE();
                env.AddEnv("pmd");
                env.AddEnv("pmdopt");
                envPmd = env.GetEnvVal("pmd");
                envPmdOpt = env.GetEnvVal("pmdopt");

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

            compiler = im.GetCompiler("PMDDotNET.Compiler.Compiler");
            driver = im.GetDriver("PMDDotNET.Driver.Driver");

            if (compiler == null || driver == null)
            {
                throw new Exception("インスタンスの生成に失敗しました。");
            }
        }

        public MmlDatum[] compileFromSrcText(string srcText, string wrkMMLFullPath,string srcFileFullPath, Point point)
        {
            if (!ok) return null;

            PMDManager.wrkMMLFullPath = wrkMMLFullPath;
            compiler.Init();
            compiler.SetCompileSwitch("IDE");

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

            compiler.SetCompileSwitch(string.Format(
                "PmdOption={0} \"{1}\""
                , "/v /C"
                , srcFileFullPath));
            byte[] src = Encoding.GetEncoding(932).GetBytes(srcText);
            using (MemoryStream sourceMML = new MemoryStream(src))
                ret = compiler.Compile(sourceMML, appendFileReaderCallback);// wrkMUCFullPath, disp);
            return ret;
        }

        private static Stream appendFileReaderCallback(string arg)
        {
            string fn;
            fn = arg;
            if (!File.Exists(fn)) fn = Path.Combine(wrkMMLFullPath, fn);

            if (envPmd != null)
            {
                int i = 0;
                while (!File.Exists(fn) && i < envPmd.Length)
                {
                    fn = Path.Combine(
                        envPmd[i++]
                        ,Path.GetFileName(arg)
                        );
                }
            }

            FileStream strm;
            try
            {
                strm = new FileStream(fn, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            catch (IOException)
            {
                Log.WriteLine(LogLevel.ERROR, string.Format("File({0}) not found.",arg));
                strm = null;
            }

            return strm;
        }

        public CompilerInfo GetCompilerInfo()
        {
            if (!ok) return null;
            return compiler.GetCompilerInfo();
        }

        public void Rendering()
        {
            driver.Rendering();
        }

        public void InitDriver(
            string fileName
            , Action<ChipDatum> oPNAWrite
            , Action<long, int> oPNAWaitSend
            , Func<ChipDatum, int> ppz8Write
            , Func<ChipDatum, int> ppsdrvWrite
            , bool notSoundBoard2
            , musicDriverInterface.MmlDatum[] mBuf
            , bool isLoadADPCM
            , bool loadADPCMOnly)
        {

            musicDriverInterface.Log.writeMethod = disp;
            musicDriverInterface.Log.off = 0;

            object[] addtionalPMDDotNETOption = new object[]{
                isLoadADPCM,//bool
                loadADPCMOnly,//bool
                true,//bool isAUTO;
                false,//bool isVA;
                notSoundBoard2,//bool isNRM;
                true,//bool usePPS;
                true,//bool usePPZ;
                !notSoundBoard2,//bool isSPB;
                envPmd,//string[] envPmd;
                envPmdOpt,//string[] envPmdOpt;
                fileName,//string srcFile;
                (Func<string,Stream>)appendFileReaderCallback
            };
            string[] addtionalPMDOption = new string[]{
            };

            driver.Init(
                fileName
                , oPNAWrite
                , oPNAWaitSend
                , mBuf
                , new object[] {
                      addtionalPMDDotNETOption //PMDDotNET option 
                    , addtionalPMDOption // PMD option
                    , ppz8Write
                    , ppsdrvWrite
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
    }
}
