using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using mucomDotNET.Interface;

namespace mml2vgmIDE
{
    public class mucomManager
    {
        private Assembly asmCompiler = null;
        private Assembly asmDriver = null;
        private iCompiler compiler = null;
        private iDriver driver = null;
        private bool ok = false;
        private Action<string> disp = null;

        public mucomManager(Assembly compiler, Assembly driver, Action<string> disp)
        {
            try
            {
                asmCompiler = compiler;
                asmDriver = driver;
                this.disp = disp;

                var info = asmCompiler.GetType("mucomDotNET.Compiler.Compiler");
                this.compiler = Activator.CreateInstance(info) as iCompiler;

                info = asmDriver.GetType("mucomDotNET.Driver.Driver");
                this.driver = Activator.CreateInstance(info) as iDriver;

                Log.writeLine = WriteLog;

                if (this.compiler == null || this.driver == null)
                {
                    throw new Exception("インスタンスの生成に失敗しました。");
                }

                ok = true;

            }
            catch
            {
                asmCompiler = null;
                asmDriver = null;
                this.compiler = null;
                this.driver = null;
                ok = false;
                throw;
            }

        }

        public void Rendering()
        {
            driver.Rendering();
        }

        private void WriteLog(LogLevel arg1, string arg2)
        {
            disp?.Invoke(arg2);
        }

        public void InitDriver(
            string fileName
            , Action<OPNAData> oPNAWrite
            , Action<long, int> oPNAWaitSend
            , bool notSoundBoard2
            , MubDat[] mubBuf
            , bool isLoadADPCM
            , bool loadADPCMOnly)
        {
            driver.Init(fileName, oPNAWrite, oPNAWaitSend, notSoundBoard2, mubBuf, isLoadADPCM, loadADPCMOnly);
        }

        public void MSTART(int v)
        {
            driver.MSTART(v);
        }

        public void MSTOP()
        {
            driver.MSTOP();
        }

        public void StartRendering(int sampleRate, int yM2608ClockValue)
        {
            driver.StartRendering(sampleRate, yM2608ClockValue);
        }

        public MubDat[] compile(string srcMUCFullPath, string wrkMUCFullPath)
        {
            if (!ok) return null;

            compiler.Init();
            MubDat[] ret = compiler.StartToMubDat(srcMUCFullPath, wrkMUCFullPath);

            return ret;
        }

        public CompilerInfo GetCompilerInfo()
        {
            if (!ok) return null;
            return compiler.GetCompilerInfo();
        }
    }
}
