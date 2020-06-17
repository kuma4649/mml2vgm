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
        public string desTiFn = "";
        public bool outVgmFile = true;
        public bool outTraceInfoFile = false;

        public mvc(string[] args)
        {

            //ファイル、オプションの指定無し
            if (args == null || args.Length < 1)
            {
                //disp usage
                Console.WriteLine(msg.get("I07000"));
                Environment.Exit(0);
            }

            int cnt = 0;
            try
            {
                List<string> lstOpt = new List<string>();
                while (args[cnt].Length > 1 && args[cnt][0] == '-')
                {
                    lstOpt.Add(args[cnt++].Substring(1));
                }

                foreach (string opt in lstOpt)
                {
                    //vgm switch
                    switch (opt[0])
                    {
                        case 'v':
                            if (opt[1] == '+') outVgmFile = true;
                            if (opt[1] == '-') outVgmFile = false;
                            break;
                        case 't':
                            if (opt[1] == '+') outTraceInfoFile = true;
                            if (opt[1] == '-') outTraceInfoFile = false;
                            break;
                    }
                }
            }
            catch
            {
                Console.WriteLine(msg.get("E0000"));
                Environment.Exit(0);
            }

            //ファイルの指定無し
            if (args == null || args.Length < cnt)
            {
                //disp usage
                Console.WriteLine(msg.get("I07000"));
                Environment.Exit(0);
            }

            srcFn = args[cnt++];
            if (Path.GetExtension(srcFn) == "")
            {
                srcFn += ".gwi";
            }

            string path1 = System.IO.Path.GetDirectoryName(Path.Combine(System.Environment.CurrentDirectory, srcFn));
            path1 = string.IsNullOrEmpty(path1) ? srcFn : path1;

            if (args.Length > cnt)
            {
                desFn = args[cnt];
            }
            else
            {
                desFn = Path.Combine(path1, Path.GetFileNameWithoutExtension(srcFn) + ".vgm");
            }

            desTiFn = Path.Combine(path1, Path.GetFileNameWithoutExtension(srcFn) + ".ti");

            Core.log.debug = false;
            Core.log.Open();
            Core.log.Write("start compile thread");

            Assembly myAssembly = Assembly.GetEntryAssembly();
            string path = System.IO.Path.GetDirectoryName(myAssembly.Location);
            path = string.IsNullOrEmpty(path1) ? myAssembly.Location : path;

            Mml2vgm mv = new Mml2vgm(srcFn, desFn, path, Disp);
            mv.outVgmFile = outVgmFile;
            mv.outTraceInfoFile = outTraceInfoFile;
            mv.desTiFn = desTiFn;

            int ret = mv.Start();

            if (ret == 0)
            {
                Console.WriteLine(msg.get("I0000"));
                Console.WriteLine(msg.get("I0001"));
                foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in mv.desVGM.chips)
                {
                    foreach (ClsChip chip in kvp.Value)
                    {
                        if (chip == null) continue;
                        List<partWork> pw = chip.lstPartWork;
                        for (int i = 0; i < pw.Count; i++)
                        {
                            if (pw[i].clockCounter == 0) continue;

                            Console.WriteLine(string.Format(msg.get("I0002")
                                , pw[i].pg[0].PartName.Substring(0, 2).Replace(" ", "") + int.Parse(pw[i].pg[0].PartName.Substring(2, 2)).ToString()
                                , pw[i].pg[0].chip.Name.ToUpper()
                                , pw[i].clockCounter
                            ));
                        }
                    }
                }
            }

            Console.WriteLine(msg.get("I0003"));

            foreach (msgInfo mes in msgBox.getWrn())
            {
                Console.Error.WriteLine(string.Format(msg.get("I0004"), mes.filename, mes.line == -1 ? "-" : (mes.line + 1).ToString(), mes.body));
            }

            foreach (msgInfo mes in msgBox.getErr())
            {
                Console.Error.WriteLine(string.Format(msg.get("I0005"), mes.filename, mes.line == -1 ? "-" : (mes.line + 1).ToString(), mes.body));
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

                if (mv.desVGM.ym2608 != null)
                {
                    if (mv.desVGM.ym2608.Length > 0 && mv.desVGM.ym2608[0].pcmDataEasy != null) Console.WriteLine(string.Format(msg.get("I0020"), mv.desVGM.ym2608[0].pcmDataEasy.Length - 15));
                    if (mv.desVGM.ym2608.Length > 1 && mv.desVGM.ym2608[1].pcmDataEasy != null) Console.WriteLine(string.Format(msg.get("I0021"), mv.desVGM.ym2608[1].pcmDataEasy.Length - 15));
                }
                if (mv.desVGM.ym2609 != null)
                {
                    if (mv.desVGM.ym2609.Length > 0 && mv.desVGM.ym2609[0].pcmDataEasyA != null) Console.WriteLine(string.Format(msg.get("I0031"), mv.desVGM.ym2609[0].pcmDataEasyA.Length - 15));
                    if (mv.desVGM.ym2609.Length > 0 && mv.desVGM.ym2609[0].pcmDataEasyB != null) Console.WriteLine(string.Format(msg.get("I0032"), mv.desVGM.ym2609[0].pcmDataEasyB.Length - 15));
                    if (mv.desVGM.ym2609.Length > 0 && mv.desVGM.ym2609[0].pcmDataEasyC != null) Console.WriteLine(string.Format(msg.get("I0033"), mv.desVGM.ym2609[0].pcmDataEasyC.Length - 15));
                    if (mv.desVGM.ym2609.Length > 1 && mv.desVGM.ym2609[1].pcmDataEasyA != null) Console.WriteLine(string.Format(msg.get("I0034"), mv.desVGM.ym2609[1].pcmDataEasyA.Length - 15));
                    if (mv.desVGM.ym2609.Length > 1 && mv.desVGM.ym2609[1].pcmDataEasyB != null) Console.WriteLine(string.Format(msg.get("I0035"), mv.desVGM.ym2609[1].pcmDataEasyB.Length - 15));
                    if (mv.desVGM.ym2609.Length > 1 && mv.desVGM.ym2609[1].pcmDataEasyC != null) Console.WriteLine(string.Format(msg.get("I0036"), mv.desVGM.ym2609[1].pcmDataEasyC.Length - 15));
                }
                if (mv.desVGM.ym2610b != null)
                {
                    if (mv.desVGM.ym2610b.Length > 0 && mv.desVGM.ym2610b[0].pcmDataEasyA != null) Console.WriteLine(string.Format(msg.get("I0022"), mv.desVGM.ym2610b[0].pcmDataEasyA.Length - 15));
                    if (mv.desVGM.ym2610b.Length > 0 && mv.desVGM.ym2610b[0].pcmDataEasyB != null) Console.WriteLine(string.Format(msg.get("I0023"), mv.desVGM.ym2610b[0].pcmDataEasyB.Length - 15));
                    if (mv.desVGM.ym2610b.Length > 1 && mv.desVGM.ym2610b[1].pcmDataEasyA != null) Console.WriteLine(string.Format(msg.get("I0024"), mv.desVGM.ym2610b[1].pcmDataEasyA.Length - 15));
                    if (mv.desVGM.ym2610b.Length > 1 && mv.desVGM.ym2610b[1].pcmDataEasyB != null) Console.WriteLine(string.Format(msg.get("I0025"), mv.desVGM.ym2610b[1].pcmDataEasyB.Length - 15));
                }
                //if (mv.desVGM.segapcm[0].pcmData != null) Console.WriteLine(string.Format(" PCM Data size(SEGAPCM)  : {0} byte", mv.desVGM.segapcm[0].pcmData.Length - 15));
                //if (mv.desVGM.segapcm[1].pcmData != null) Console.WriteLine(string.Format(" PCM Data size(SEGAPCMSecondary)  : {0} byte", mv.desVGM.segapcm[1].pcmData.Length - 15));
                if (mv.desVGM.ym2612 != null)
                {
                    if (mv.desVGM.ym2612.Length > 0 && mv.desVGM.ym2612[0].pcmDataEasy != null) Console.WriteLine(string.Format(msg.get("I0026"), mv.desVGM.ym2612[0].pcmDataEasy.Length));
                }
                if (mv.desVGM.rf5c164 != null)
                {
                    if (mv.desVGM.rf5c164.Length > 0 && mv.desVGM.rf5c164[0].pcmDataEasy != null) Console.WriteLine(string.Format(msg.get("I0027"), mv.desVGM.rf5c164[0].pcmDataEasy.Length - 12));
                    if (mv.desVGM.rf5c164.Length > 1 && mv.desVGM.rf5c164[1].pcmDataEasy != null) Console.WriteLine(string.Format(msg.get("I0028"), mv.desVGM.rf5c164[1].pcmDataEasy.Length - 12));
                }
                if (mv.desVGM.huc6280 != null)
                {
                    if (mv.desVGM.huc6280.Length > 0 && mv.desVGM.huc6280[0].pcmDataEasy != null) Console.WriteLine(string.Format(msg.get("I0029"), mv.desVGM.huc6280[0].pcmDataEasy.Length));
                    if (mv.desVGM.huc6280.Length > 1 && mv.desVGM.huc6280[1].pcmDataEasy != null) Console.WriteLine(string.Format(msg.get("I0030"), mv.desVGM.huc6280[1].pcmDataEasy.Length));
                }
            }

            Console.WriteLine(msg.get("I0050"));

            Core.log.Write("end compile thread");
            Core.log.Close();


            if (msgBox.getErr().Length > 0)
            {
                Environment.Exit(msgBox.getErr().Length);
            }

            Environment.Exit(ret);
        }

        private void Disp(string msg)
        {
            Console.WriteLine(msg);
            Core.log.Write(msg);
        }

    }
}
