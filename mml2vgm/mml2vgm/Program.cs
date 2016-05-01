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

            //foreach (string arg in args)
            //{
            //    if (!File.Exists(arg))
            //    {
            //        continue;
            //    }

            //    msgBox.clear();

            //    mml2vgm mv = new mml2vgm(arg, Path.ChangeExtension(args[0], Properties.Resources.ExtensionVGM));
            //    mv.Start();

            //    foreach (string mes in msgBox.getWrn())
            //    {
            //        Console.WriteLine(string.Format("Warning : {0}", mes));
            //    }

            //    foreach (string mes in msgBox.getErr())
            //    {
            //        Console.WriteLine(string.Format("Error : {0}", mes));
            //    }

            //}
        }
    }
}
