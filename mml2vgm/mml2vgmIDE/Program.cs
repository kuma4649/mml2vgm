using System;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace mml2vgmIDE
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {

            //DOBON.NET さんのコードを流用

            string mutexName = "mml2vgmIDE";
            bool createdNew;

            using (Mutex mutex = new Mutex(true, mutexName, out createdNew))
            {
                //ミューテックスの初期所有権が付与されたか調べる
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

                    //されなかった場合は、すでに起動していると判断して終了
                    MessageBox.Show("多重起動はできません。");
                    return;
                }

                try
                {

                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.ThreadException +=
                        new System.Threading.ThreadExceptionEventHandler(
                            Application_ThreadException);

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
                        log.ForcedWrite(ex);
                        MessageBox.Show(ex.Message, "致命的なエラー");
                    }
                    finally
                    {
                    }
                }
                finally
                {
                    //ミューテックスを解放する
                    mutex.ReleaseMutex();
                }

            }
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            try
            {
                log.ForcedWrite(e.Exception);
                MessageBox.Show(e.Exception.Message, "致命的なエラー");
            }
            finally
            {
                //アプリケーションを終了する
                Application.Exit();
            }
        }

        public static Process GetPreviousProcess()
        {
            Process curProcess = Process.GetCurrentProcess();
            Process[] allProcesses = Process.GetProcessesByName(curProcess.ProcessName);

            foreach (Process checkProcess in allProcesses)
            {
                // 自分自身のプロセスIDは無視する
                if (checkProcess.Id != curProcess.Id)
                {
                    // プロセスのフルパス名を比較して同じアプリケーションか検証
                    if (string.Compare(
                        checkProcess.MainModule.FileName,
                        curProcess.MainModule.FileName, true) == 0)
                    {
                        // 同じフルパス名のプロセスを取得
                        return checkProcess;
                    }
                }
            }

            // 同じアプリケーションのプロセスが見つからない！
            return null;
        }


        //SendMessageで送る構造体（Unicode文字列送信に最適化したパターン）
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpData;
        }

        //SendMessage（データ転送）
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, ref COPYDATASTRUCT lParam);
        public const int WM_COPYDATA = 0x004A;
        public const int WM_PASTE = 0x0302;

        //SendMessageを使ってプロセス間通信で文字列を渡す
        public static void SendString(IntPtr targetWindowHandle, string str)
        {
            COPYDATASTRUCT cds = new COPYDATASTRUCT();
            cds.dwData = IntPtr.Zero;
            cds.lpData = str;
            cds.cbData = str.Length * sizeof(char);
            //受信側ではlpDataの文字列を(cbData/2)の長さでstring.Substring()する

            IntPtr myWindowHandle = Process.GetCurrentProcess().MainWindowHandle;
            SendMessage(targetWindowHandle, WM_COPYDATA, myWindowHandle, ref cds);
        }

    }
}
