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
        public static string wrkMMLFullPath;

        public int YM2608_FMVolume = 0;
        public int YM2608_SSGVolume = 0;
        public int YM2608_RhythmVolume = 0;
        public int YM2608_AdpcmVolume = 0;
        public int GIMIC_SSGVolume = 0;

        private iCompiler compiler = null;
        private iDriver driver = null;
        private bool ok = false;
        private Action<string> disp = null;
        private Setting setting = null;
        private InstanceMarker im = null;
        private static string[] envPmd = null;
        private static string[] envPmdOpt = null;
        private bool isNRM;
        private bool isSPB;
        private bool isVA;
        private bool usePPS;
        private bool usePPZ;
        private ChipRegister chipRegister;
        private bool isGIMICOPNA;
        private GD3Tag currentGD3Tag;

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

            //musicDriverInterface.Log.level = LogLevel.TRACE;

            compiler.SetCompileSwitch(string.Format(
                "PmdOption={0} \"{1}\""
                , setting.pmdDotNET.compilerArguments
                , srcFileFullPath));
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
            , bool dummy_NotSoundBoard2 //参照しません
            , musicDriverInterface.MmlDatum[] mBuf
            , bool isLoadADPCM
            , bool loadADPCMOnly
            , ChipRegister chipRegister
            , bool isGIMICOPNA)
        {

            musicDriverInterface.Log.writeMethod = disp;
            musicDriverInterface.Log.off = 0;
            setting = Audio.setting;
            this.chipRegister = chipRegister;

            isNRM = setting.pmdDotNET.soundBoard == 0;
            isSPB = setting.pmdDotNET.soundBoard == 1;
            isVA = false;
            usePPS = setting.pmdDotNET.usePPSDRV;
            usePPZ = setting.pmdDotNET.usePPZ8;
            this.isGIMICOPNA = isGIMICOPNA;

            object[] addtionalPMDDotNETOption = new object[]{
                isLoadADPCM,//bool
                loadADPCMOnly,//bool
                setting.pmdDotNET.isAuto,//bool isAUTO;
                isVA,//bool
                isNRM,//bool
                usePPS,//bool
                usePPZ,//bool
                isSPB,//bool
                envPmd,//string[] 環境変数PMD
                envPmdOpt,//string[] 環境変数PMDOpt
                fileName,//string srcFile;
                "",//string PPCFileHeader無視されます(設定不要)
                (Func<string,Stream>)appendFileReaderCallback
            };

            string[] addtionalPMDOption = GetPMDOption();

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

            //AUTO指定の場合に構成が変わるので、構成情報を受け取ってから音量設定を行う
            isNRM = (bool)addtionalPMDDotNETOption[4];//4 = isNrm
            isSPB = (bool)addtionalPMDDotNETOption[7];//7 = isSPB
            isVA = (bool)addtionalPMDDotNETOption[3];//3 = isVA
            usePPS = (bool)addtionalPMDDotNETOption[5];//5 = usePPS
            usePPZ = (bool)addtionalPMDDotNETOption[6];//6 = usePPZ
            string[] pmdOptionVol = SetVolume();

            ////ユーザーがコマンドラインでDオプションを指定していない場合はpmdVolを適用させる
            //if (!pmdvolFound && pmdOptionVol != null && pmdOptionVol.Length > 0)
            //{
            //    ((PMDDotNET.Driver)driver).resetOption(pmdOptionVol);//
            //}

        }


        private string[] GetPMDOption()
        {
            List<string> op = new List<string>();

            //envPMDOpt
            if (envPmdOpt != null && envPmdOpt.Length > 0) foreach (string opt in envPmdOpt) op.Add(opt);

            //引数(IDEではオプション設定)
            string[] drvArgs = setting.pmdDotNET.driverArguments.Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            if (drvArgs != null && drvArgs.Length > 0) foreach (string drvArg in drvArgs) op.Add(drvArg);

            return op.ToArray();
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

        private string[] SetVolume()
        {
            List<string> ret = new List<string>();
            YM2608_FMVolume = 0;
            YM2608_SSGVolume = 0;
            YM2608_RhythmVolume = 0;
            YM2608_AdpcmVolume = 0;
            GIMIC_SSGVolume = 0;

            //fmgen向け設定
            //fm:ssg = 1:0.25で調整
            //
            //  pmd内で1:(0.45～0.50)に補正される
            //  ・OPNの場合のみpmdのコード上でfmの音量を下げるコードを通過する
            //  ・GIMIC ProとLiteのターミナルでも mファイルを再生し確認
            if (isNRM)
            {
                //PC98のOPNを想定
                YM2608_FMVolume = 12;//FM  98は88よりFMが大きい
                YM2608_SSGVolume = -5;//SSG
                YM2608_RhythmVolume = -191;//Rhythm
                YM2608_AdpcmVolume = -191;//Adpcm
            }
            else
            {
                //OPNA(-86/SPB)を想定
                YM2608_FMVolume = 0;//FM
                YM2608_SSGVolume = -5;//SSG
                YM2608_RhythmVolume = 0;//Rhythm //未調査
                YM2608_AdpcmVolume = 0;//Adpcm //未調査
            }

            GIMIC_SSGVolume = 0;
            if (isNRM)
                GIMIC_SSGVolume = 31;//GMC-OPNA に31を送信
            else
                GIMIC_SSGVolume = 66;//GMC-OPNA に66を送信

            //GMC-OPNA以外のOPNA系モジュール
            if (!isGIMICOPNA)
            {
                //pmdのオプションで調整
                ret.Add("/DF12");
                ret.Add("/DS0");
            }

            //SCCIはユーザー任せ...

            return ret.ToArray();
        }

        public GD3Tag GetGD3Tag()
        {
            return currentGD3Tag;
        }
    }
}
