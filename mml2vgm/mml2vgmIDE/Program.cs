using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

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

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
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
