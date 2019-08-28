using System;
using System.IO;
using System.Windows.Forms;

namespace mml2vgm
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new frmMain());
                return;

        }
    }
}
