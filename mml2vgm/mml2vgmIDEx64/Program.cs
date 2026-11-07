using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace mml2vgmIDEx64
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // DOBON.NET のコードを流用

            string mutexName = "mml2vgmIDE";
            bool createdNew;

            using (Mutex mutex = new Mutex(true, mutexName, out createdNew))
            {
                // ミューテックスの初期所有権が付与されたか調べる
                if (!createdNew)
                {
                    if (Environment.GetCommandLineArgs().Length > 1)
                    {
                        Process prc = GetPreviousProcess();
                        if (prc != null)
                        {
                            SendString(prc.MainWindowHandle, Environment.GetCommandLineArgs()[1]);
                            return;
                        }
                    }

                    // 見つからない場合は、すでに起動されていると判定して終了
                    MessageBox.Show("多重起動はできません。", "mml2vgmIDEx64", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                try
                {

                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                    Application.ThreadException +=
                        new System.Threading.ThreadExceptionEventHandler(
                            Application_ThreadException);
                    AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                    TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

                    Setting setting = Setting.Load();
                    Audio.Init(setting);

                    FrmMain f = new FrmMain
                    {
                        setting = setting
                    };
                    f.Init();

                    Application.Run(f);

                }
                catch (Exception ex)
                {
                    try
                    {
                        Log.ForcedWrite(ex);
                        MessageBox.Show(ex.Message, "予期しないエラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                    }
                }
                finally
                {
                    // ミューテックスを解放
                    mutex.ReleaseMutex();
                }

            }
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            try
            {
                Log.ForcedWrite(e.Exception);
                MessageBox.Show(e.Exception.Message, "予期しないエラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // アプリケーションを終了する
                Application.Exit();
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Exception ex = e.ExceptionObject as Exception;
                if (ex != null)
                {
                    Log.ForcedWrite(ex);
                }
                else
                {
                    Log.ForcedWrite("Unhandled exception object: {0}", e.ExceptionObject?.ToString() ?? "<null>");
                }
            }
            catch
            {
            }
        }

        private static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            try
            {
                Log.ForcedWrite(e.Exception);
            }
            finally
            {
                e.SetObserved();
            }
        }

        public static Process GetPreviousProcess()
        {
            Process curProcess = Process.GetCurrentProcess();
            Process[] allProcesses = Process.GetProcessesByName(curProcess.ProcessName);

            foreach (Process checkProcess in allProcesses)
            {
                // 現在のプロセスIDは無視する
                if (checkProcess.Id != curProcess.Id)
                {
                    // プロセスのフルパスを比較して同じアプリケーションを探す
                    if (string.Compare(
                        checkProcess.MainModule.FileName,
                        curProcess.MainModule.FileName, true) == 0)
                    {
                        // 同じフルパスのプロセスを取得
                        return checkProcess;
                    }
                }
            }

            // 同じアプリケーションのプロセスが見つからない
            return null;
        }


        // SendMessageで使う構造体（Unicode文字列送信向け）
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpData;
        }

        // SendMessage（データ転送）
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, ref COPYDATASTRUCT lParam);
        public const int WM_COPYDATA = 0x004A;
        public const int WM_PASTE = 0x0302;

        // SendMessageを使ってプロセス間通信で文字列を渡す
        public static void SendString(IntPtr targetWindowHandle, string str)
        {
            COPYDATASTRUCT cds = new COPYDATASTRUCT();
            cds.dwData = IntPtr.Zero;
            cds.lpData = str;
            cds.cbData = str.Length * sizeof(char);
            // 受信側ではlpDataの文字数（cbData/2）を長さにして取り出す

            IntPtr myWindowHandle = Process.GetCurrentProcess().MainWindowHandle;
            SendMessage(targetWindowHandle, WM_COPYDATA, myWindowHandle, ref cds);
        }

    }
}