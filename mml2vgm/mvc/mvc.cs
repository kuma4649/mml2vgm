using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Core;

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
                Console.WriteLine(Properties.Resources.msgUsage);
                Environment.Exit(0);
            }

            srcFn = args[0];
            if (System.IO.Path.GetExtension(srcFn) == "")
            {
                srcFn += ".gwi";
            }

            if (args.Length > 1)
            {
                desFn = args[1];
            }
            else
            {
                desFn = System.IO.Path.GetFileNameWithoutExtension(srcFn) + ".vgm";
            }

            Core.log.debug = false;
            Core.log.Open();
            Core.log.Write("start compile thread");

            Assembly myAssembly = Assembly.GetEntryAssembly();
            string path = System.IO.Path.GetDirectoryName(myAssembly.Location);
            Mml2vgm mv = new Mml2vgm(srcFn, desFn, path,Disp);
            int ret = mv.Start();

            if (ret == 0)
            {
                Console.WriteLine("\r\nPart  Chip    Count");
                Console.WriteLine("--------------------");
                foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in mv.desVGM.chips)
                {
                    foreach (ClsChip chip in kvp.Value)
                    {
                        List<partWork> pw = chip.lstPartWork;
                        for (int i = 0; i < pw.Count; i++)
                        {
                            if (pw[i].clockCounter == 0) continue;

                            Console.WriteLine(string.Format(" {0}   {1}   {2}"
                                , pw[i].PartName.Substring(0, 2).Replace(" ", "") + int.Parse(pw[i].PartName.Substring(2, 2)).ToString()
                                , pw[i].chip.Name.ToUpper()
                                , pw[i].clockCounter
                            ));
                        }
                    }
                }
            }

            Console.WriteLine("\r\nResult");

            foreach (string mes in msgBox.getWrn())
            {
                Console.WriteLine(string.Format(" Warning : {0}", mes));
            }

            foreach (string mes in msgBox.getErr())
            {
                Console.WriteLine(string.Format(" Error : {0}", mes));
            }

            Console.WriteLine("");
            Console.WriteLine(string.Format(" Errors : {0}\r\n Warnings : {1}", msgBox.getErr().Length, msgBox.getWrn().Length));

            if (mv.desVGM != null)
            {
                if (mv.desVGM.loopSamples != -1)
                {
                    Console.WriteLine(string.Format(" Loop Clocks  : {0}", mv.desVGM.loopClock));
                    if (mv.desVGM.info.format == enmFormat.VGM)
                        Console.WriteLine(string.Format(" Loop Samples : {0:0.00}({1:0.00}s)"
                            , mv.desVGM.loopSamples
                            , mv.desVGM.loopSamples / 44100L));
                    else
                        Console.WriteLine(string.Format(" Loop Samples : {0:0.00}({1:0.00}s)"
                            , mv.desVGM.loopSamples
                            , mv.desVGM.loopSamples / (mv.desVGM.info.xgmSamplesPerSecond)));
                }

                Console.WriteLine(string.Format(" Total Clocks  : {0}", mv.desVGM.lClock));
                if (mv.desVGM.info.format == enmFormat.VGM)
                    Console.WriteLine(string.Format(" Total Samples : {0:0.00}({1:0.00}s)"
                        , mv.desVGM.dSample
                        , mv.desVGM.dSample / 44100L));
                else
                    Console.WriteLine(string.Format(" Total Samples : {0:0.00}({1:0.00}s)"
                        , mv.desVGM.dSample
                        , mv.desVGM.dSample / (mv.desVGM.info.xgmSamplesPerSecond)));

                if (mv.desVGM.ym2608[0].pcmData != null) Console.WriteLine(string.Format(" ADPCM Data size(YM2608)  : ({0}/262143) byte", mv.desVGM.ym2608[0].pcmData.Length - 15));
                if (mv.desVGM.ym2608[1].pcmData != null) Console.WriteLine(string.Format(" ADPCM Data size(YM2608Secondary)  : ({0}/262143) byte", mv.desVGM.ym2608[1].pcmData.Length - 15));
                if (mv.desVGM.ym2610b[0].pcmDataA != null) Console.WriteLine(string.Format(" ADPCM-A Data size(YM2610B)  : ({0}/16777215) byte", mv.desVGM.ym2610b[0].pcmDataA.Length - 15));
                if (mv.desVGM.ym2610b[0].pcmDataB != null) Console.WriteLine(string.Format(" ADPCM-B Data size(YM2610B)  : ({0}/16777215) byte", mv.desVGM.ym2610b[0].pcmDataB.Length - 15));
                if (mv.desVGM.ym2610b[1].pcmDataA != null) Console.WriteLine(string.Format(" ADPCM-A Data size(YM2610BSecondary)  : ({0}/16777215) byte", mv.desVGM.ym2610b[1].pcmDataA.Length - 15));
                if (mv.desVGM.ym2610b[1].pcmDataB != null) Console.WriteLine(string.Format(" ADPCM-B Data size(YM2610BSecondary)  : ({0}/16777215) byte", mv.desVGM.ym2610b[1].pcmDataB.Length - 15));
                //if (mv.desVGM.segapcm[0].pcmData != null) Console.WriteLine(string.Format(" PCM Data size(SEGAPCM)  : {0} byte", mv.desVGM.segapcm[0].pcmData.Length - 15));
                //if (mv.desVGM.segapcm[1].pcmData != null) Console.WriteLine(string.Format(" PCM Data size(SEGAPCMSecondary)  : {0} byte", mv.desVGM.segapcm[1].pcmData.Length - 15));
                if (mv.desVGM.ym2612[0].pcmData != null) Console.WriteLine(string.Format(" PCM Data size(YM2612)  : {0} byte", mv.desVGM.ym2612[0].pcmData.Length));
                if (mv.desVGM.rf5c164[0].pcmData != null) Console.WriteLine(string.Format(" PCM Data size(RF5C164) : ({0}/65535) byte", mv.desVGM.rf5c164[0].pcmData.Length - 12));
                if (mv.desVGM.rf5c164[1].pcmData != null) Console.WriteLine(string.Format(" PCM Data size(RF5C164Secondary) : ({0}/65535) byte", mv.desVGM.rf5c164[1].pcmData.Length - 12));
                if (mv.desVGM.huc6280[0].pcmData != null) Console.WriteLine(string.Format(" PCM Data size(HuC6280)  : {0} byte", mv.desVGM.huc6280[0].pcmData.Length));
                if (mv.desVGM.huc6280[1].pcmData != null) Console.WriteLine(string.Format(" PCM Data size(HuC6280Secondary)  : {0} byte", mv.desVGM.huc6280[1].pcmData.Length));
            }

            Console.WriteLine("\r\nFinished.\r\n");

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
