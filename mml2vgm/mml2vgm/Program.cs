using System;
using System.IO;

namespace mml2vgm
{
    class Program
    {
        static void Main(string[] args)
        {

            if (args.Length < 1)
            {
                return;
            }

            foreach (string arg in args)
            {
                if (!File.Exists(arg))
                {
                    continue;
                }

                msgBox.clear();

                mml2vgm mv = new mml2vgm(arg, Path.ChangeExtension(args[0], ".vgm"));
                mv.Start();

                foreach (string mes in msgBox.getWrn())
                {
                    Console.WriteLine(string.Format("Warning : {0}", mes));
                }

                foreach (string mes in msgBox.getErr())
                {
                    Console.WriteLine(string.Format("Error : {0}", mes));
                }

#if DEBUG
                Console.WriteLine("DEBUG実行が終了しました。何かキーを押してください。");
                Console.ReadKey();
#endif

            }

        }
    }
}
