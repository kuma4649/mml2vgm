using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            Setting setting = Setting.Load();
            Audio.Init(setting);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            FrmMain f = new FrmMain();
            f.setting = setting;
            f.Init();
            Application.Run(f);
        }
    }
}
