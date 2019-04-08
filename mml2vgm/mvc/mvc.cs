using Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace mvc
{
    public class mvc
    {
        /// <summary>
        /// コンパイル対象
        /// </summary>
        public string srcFn = "";
        public string desFn = "";

        public mvc(string[] args)
        {

            //ファイル、オプションの指定無し
            if (args == null || args.Length < 1)
            {
                //disp usage
                Console.WriteLine(msg.get("I07000"));
                Environment.Exit(0);
            }

            srcFn = args[0];
            if (Path.GetExtension(srcFn) == "")
            {
                srcFn += ".gwi";
            }

            if (args.Length > 1)
            {
                desFn = args[1];
            }
            else
            {
                desFn = Path.Combine(Path.GetDirectoryName(srcFn), Path.GetFileNameWithoutExtension(srcFn) + ".vgm");
            }

            Core.log.debug = false;
            Core.log.Open();
            Core.log.Write("start compile thread");

            Assembly myAssembly = Assembly.GetEntryAssembly();
            string path = System.IO.Path.GetDirectoryName(myAssembly.Location);
            Mml2vgm mv = new Mml2vgm(srcFn, desFn, path, Disp);
            int ret = mv.Start();

            if (ret == 0)
            {
                Console.WriteLine(msg.get("I0000"));
                Console.WriteLine(msg.get("I0001"));
                foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in mv.desVGM.chips)
                {
                    foreach (ClsChip chip in kvp.Value)
                    {
                        List<partWork> pw = chip.lstPartWork;
                        for (int i = 0; i < pw.Count; i++)
                        {
                            if (pw[i].clockCounter == 0) continue;

                            Console.WriteLine(string.Format(msg.get("I0002")
                                , pw[i].PartName.Substring(0, 2).Replace(" ", "") + int.Parse(pw[i].PartName.Substring(2, 2)).ToString()
                                , pw[i].chip.Name.ToUpper()
                                , pw[i].clockCounter
                            ));
                        }
                    }
                }
            }

            Console.WriteLine(msg.get("I0003"));

            foreach (string mes in msgBox.getWrn())
            {
                Console.WriteLine(string.Format(msg.get("I0004"), mes));
            }

            foreach (string mes in msgBox.getErr())
            {
                Console.WriteLine(string.Format(msg.get("I0005"), mes));
            }

            Console.WriteLine("");
            Console.WriteLine(string.Format(msg.get("I0006"), msgBox.getErr().Length, msgBox.getWrn().Length));

            if (mv.desVGM != null)
            {
                if (mv.desVGM.loopSamples != -1)
                {
                    Console.WriteLine(string.Format(msg.get("I0007"), mv.desVGM.loopClock));
                    if (mv.desVGM.info.format == enmFormat.VGM)
                        Console.WriteLine(string.Format(msg.get("I0008")
                            , mv.desVGM.loopSamples
                            , mv.desVGM.loopSamples / 44100L));
                    else
                        Console.WriteLine(string.Format(msg.get("I0008")
                            , mv.desVGM.loopSamples
                            , mv.desVGM.loopSamples / (mv.desVGM.info.xgmSamplesPerSecond)));
                }

                Console.WriteLine(string.Format(msg.get("I0009"), mv.desVGM.lClock));
                if (mv.desVGM.info.format == enmFormat.VGM)
                    Console.WriteLine(string.Format(msg.get("I0010")
                        , mv.desVGM.dSample
                        , mv.desVGM.dSample / 44100L));
                else
                    Console.WriteLine(string.Format(msg.get("I0010")
                        , mv.desVGM.dSample
                        , mv.desVGM.dSample / (mv.desVGM.info.xgmSamplesPerSecond)));

                if (mv.desVGM.ym2608[0].pcmDataEasy != null) Console.WriteLine(string.Format(msg.get("I0020"), mv.desVGM.ym2608[0].pcmDataEasy.Length - 15));
                if (mv.desVGM.ym2608[1].pcmDataEasy != null) Console.WriteLine(string.Format(msg.get("I0021"), mv.desVGM.ym2608[1].pcmDataEasy.Length - 15));
                if (mv.desVGM.ym2610b[0].pcmDataEasyA != null) Console.WriteLine(string.Format(msg.get("I0022"), mv.desVGM.ym2610b[0].pcmDataEasyA.Length - 15));
                if (mv.desVGM.ym2610b[0].pcmDataEasyB != null) Console.WriteLine(string.Format(msg.get("I0023"), mv.desVGM.ym2610b[0].pcmDataEasyB.Length - 15));
                if (mv.desVGM.ym2610b[1].pcmDataEasyA != null) Console.WriteLine(string.Format(msg.get("I0024"), mv.desVGM.ym2610b[1].pcmDataEasyA.Length - 15));
                if (mv.desVGM.ym2610b[1].pcmDataEasyB != null) Console.WriteLine(string.Format(msg.get("I0025"), mv.desVGM.ym2610b[1].pcmDataEasyB.Length - 15));
                //if (mv.desVGM.segapcm[0].pcmData != null) Console.WriteLine(string.Format(" PCM Data size(SEGAPCM)  : {0} byte", mv.desVGM.segapcm[0].pcmData.Length - 15));
                //if (mv.desVGM.segapcm[1].pcmData != null) Console.WriteLine(string.Format(" PCM Data size(SEGAPCMSecondary)  : {0} byte", mv.desVGM.segapcm[1].pcmData.Length - 15));
                if (mv.desVGM.ym2612[0].pcmDataEasy != null) Console.WriteLine(string.Format(msg.get("I0026"), mv.desVGM.ym2612[0].pcmDataEasy.Length));
                if (mv.desVGM.rf5c164[0].pcmDataEasy != null) Console.WriteLine(string.Format(msg.get("I0027"), mv.desVGM.rf5c164[0].pcmDataEasy.Length - 12));
                if (mv.desVGM.rf5c164[1].pcmDataEasy != null) Console.WriteLine(string.Format(msg.get("I0028"), mv.desVGM.rf5c164[1].pcmDataEasy.Length - 12));
                if (mv.desVGM.huc6280[0].pcmDataEasy != null) Console.WriteLine(string.Format(msg.get("I0029"), mv.desVGM.huc6280[0].pcmDataEasy.Length));
                if (mv.desVGM.huc6280[1].pcmDataEasy != null) Console.WriteLine(string.Format(msg.get("I0030"), mv.desVGM.huc6280[1].pcmDataEasy.Length));
            }

            Console.WriteLine(msg.get("I0050"));

            Core.log.Write("end compile thread");
            Core.log.Close();


            Environment.Exit(ret);
        }

        private void Disp(string msg)
        {
            Console.WriteLine(msg);
            Core.log.Write(msg);
        }

    }
}
