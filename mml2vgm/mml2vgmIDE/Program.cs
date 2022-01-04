using System;
using System.Windows.Forms;
using System.Threading;

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
    }
}
