using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace Core
{
    /// <summary>
    /// コンパイラ
    /// </summary>
    public class Mml2vgm
    {

        private string srcFn;
        private string desFn;
        private string stPath;
        public ClsVgm desVGM = null;
        private Action<string> Disp = null;
        private int pcmDataSeqNum = 0;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="srcFn">ソースファイル</param>
        /// <param name="desFn">出力ファイル</param>
        public Mml2vgm(string srcFn, string desFn, string stPath,Action<string> disp)
        {
            this.srcFn = srcFn;
            this.desFn = desFn;
            this.stPath = stPath;
            this.Disp = disp;
        }

        /// <summary>
        /// コンパイル開始
        /// </summary>
        /// <returns></returns>
        public int Start()
        {
            try
            {
                Disp(string.Format(msg.get("I04000"), "mml2vgm"));
                Disp("");

                Disp(msg.get("I04001"));
                if (!File.Exists(srcFn))
                {
                    msgBox.setErrMsg(msg.get("E04000"));
                    return -1;
                }

                Disp(msg.get("I04002"));
                string path = Path.GetDirectoryName(Path.GetFullPath(srcFn));
                List<Line> src = GetSrc(File.ReadAllLines(srcFn), path);
                if (src == null)
                {
                    msgBox.setErrMsg(msg.get("E04001"));
                    return -1;
                }

                Disp(msg.get("I04003"));
                desVGM = new ClsVgm(stPath);
                if (desVGM.Analyze(src) != 0)
                {
                    msgBox.setErrMsg(string.Format(
                        msg.get("E04002")
                        , desVGM.lineNumber));
                    return -1;
                }

                Disp(msg.get("I04004"));
                if (desVGM.instPCMDatSeq.Count > 0) GetPCMData(path);

                Disp(msg.get("I04005"));
                MMLAnalyze mmlAnalyze = new MMLAnalyze(desVGM);
                if (mmlAnalyze.Start() != 0)
                {
                    msgBox.setErrMsg(string.Format(
                        msg.get("E04003")
                        , mmlAnalyze.lineNumber));
                    return -1;
                }

                byte[] desBuf = null;
                switch (desVGM.info.format)
                {
                    case enmFormat.VGM:
                        Disp(msg.get("I04006"));
                        desBuf = desVGM.Vgm_getByteData(mmlAnalyze.mmlData);
                        Disp(msg.get("I04007"));
                        break;
                    case enmFormat.XGM:
                        Disp(msg.get("I04008"));
                        desBuf = desVGM.Xgm_getByteData(mmlAnalyze.mmlData);
                        Disp(msg.get("I04009"));
                        break;
                    default:
                        break;
                }

                if (desBuf == null)
                {
                    msgBox.setErrMsg(string.Format(
                        msg.get("E04004")
                        , desVGM.lineNumber));
                    return -1;
                }

                Disp(msg.get("I04010"));
                outFile(desBuf);


                Result();

                return 0;
            }
            catch (Exception ex)
            {
                msgBox.setErrMsg(string.Format(msg.get("E04005")
                    , desVGM.lineNumber
                    , ex.Message
                    , ex.StackTrace));
                return -1;
            }
            finally
            {
                Disp(msg.get("I04011"));
                Disp("");
            }
        }

        private void outFile(byte[] desBuf)
        {
            if (Path.GetExtension(desFn).ToLower() != ".vgz")
            {
                if (desVGM.info.format == enmFormat.VGM)
                {
                    log.Write("VGMファイル出力");
                    File.WriteAllBytes(
                        desFn
                        , desBuf);
                }
                else
                {
                    log.Write("XGMファイル出力");
                    File.WriteAllBytes(
                        Path.Combine(
                            Path.GetDirectoryName(desFn)
                            , Path.GetFileNameWithoutExtension(desFn) + ".xgm"
                            )
                        , desBuf);
                }
                return;
            }

            log.Write("VGZファイル出力");

            int num;
            byte[] buf = new byte[1024];

            MemoryStream inStream = new MemoryStream(desBuf);
            FileStream outStream = new FileStream(desFn, FileMode.Create);
            GZipStream compStream = new GZipStream(outStream, CompressionMode.Compress);

            try
            {
                while ((num = inStream.Read(buf, 0, buf.Length)) > 0)
                {
                    compStream.Write(buf, 0, num);
                }
            }
            catch { }
            finally
            {
                if (compStream != null) compStream.Dispose();
                if (outStream != null) outStream.Dispose();
                if (inStream != null) inStream.Dispose();
            }
        }

        private List<Line> GetSrc(string[] srcBuf, string path)
        {
            List<Line> src = new List<Line>();
            int ln = 1;
            foreach (string s in srcBuf)
            {
                if (!string.IsNullOrEmpty(s) 
                    && s.TrimStart().Length > 2 
                    && s.TrimStart().Substring(0, 2) == "'+")
                {
                    string includeFn = s.Substring(2).Trim().Trim('"');
                    if (!File.Exists(includeFn))
                    {
                        includeFn = Path.Combine(path, includeFn);
                        if (!File.Exists(includeFn))
                        {
                            msgBox.setErrMsg(string.Format(
                                msg.get("E04006")
                                , includeFn));
                            return null;
                        }
                    }
                    string[] incBuf = File.ReadAllLines(includeFn);
                    int iln = 1;
                    foreach (string i in incBuf)
                    {
                        Line iline = new Line(includeFn, iln, i);
                        src.Add(iline);
                        iln++;
                    }

                    ln++;
                    continue;
                }

                Line line = new Line(srcFn, ln, s);
                src.Add(line);
                ln++;
            }

            return src;
        }

        private void GetPCMData(string path)
        {
            Dictionary<int, clsPcm> newDic = new Dictionary<int, clsPcm>();
            foreach (clsPcmDatSeq pds in desVGM.instPCMDatSeq)
            {
                byte[] buf;
                clsPcm v;
                bool isRaw;
                bool is16bit;
                int samplerate;

                if (pds.chip == enmChipType.None) continue;

                switch (pds.type)
                {
                    case enmPcmDefineType.Easy:
                        if (desVGM.instPCM.ContainsKey(pds.No))
                        {
                            desVGM.instPCM.Remove(pds.No);
                        }
                        v = new clsPcm(
                            pds.No
                            , pcmDataSeqNum++
                            , pds.chip
                            , pds.isSecondary
                            , pds.FileName
                            , pds.BaseFreq
                            , pds.Volume
                            , 0
                            , 0
                            , 0
                            , pds.DatLoopAdr
                            , false
                            , 8000);
                        desVGM.instPCM.Add(pds.No, v);

                        //ファイルの読み込み
                        buf = Common.GetPCMDataFromFile(path, v, out isRaw, out is16bit, out samplerate);
                        if (buf == null)
                        {
                            msgBox.setErrMsg(string.Format(
                                msg.get("E04007")
                                , v.fileName));
                            continue;
                        }

                        if (desVGM.info.format == enmFormat.XGM && v.isSecondary)
                        {
                            msgBox.setErrMsg(string.Format(
                                msg.get("E01017")
                                , v.fileName));
                            continue;
                        }

                        desVGM.chips[v.chip][v.isSecondary ? 1 : 0]
                            .StorePcm(
                            newDic
                            , new KeyValuePair<int, clsPcm>(pds.No, v)
                            , buf
                            , is16bit
                            , samplerate);

                        if (v.chip != enmChipType.YM2610B)
                        {
                            pds.DatEndAdr = (int)desVGM.chips[v.chip][v.isSecondary ? 1 : 0].pcmDataEasy.Length - 16;
                        }
                        else
                        {
                            if (pds.DatLoopAdr == 0)
                            {
                                //ADPCM-A
                                pds.DatEndAdr = (int)((YM2610B)desVGM.chips[v.chip][v.isSecondary ? 1 : 0]).pcmDataEasyA.Length - 16;
                            }
                            else
                            {
                                //ADPCM-B
                                pds.DatEndAdr = (int)((YM2610B)desVGM.chips[v.chip][v.isSecondary ? 1 : 0]).pcmDataEasyB.Length - 16;
                            }

                        }
                        break;
                    case enmPcmDefineType.Mucom88:
                        if (desVGM.instPCM.ContainsKey(pds.No))
                        {
                            desVGM.instPCM.Remove(pds.No);
                        }
                        v = new clsPcm(
                            pds.No
                            , pcmDataSeqNum++
                            , pds.chip
                            , pds.isSecondary
                            , pds.FileName
                            , pds.BaseFreq
                            , pds.Volume
                            , 0
                            , 0
                            , 0
                            , pds.DatLoopAdr
                            , false
                            , 8000);
                        desVGM.instPCM.Add(pds.No, v);

                        mucomADPCM2PCM.mucomPCMInfo info = null;
                        for (int i = 0; i < mucomADPCM2PCM.lstMucomPCMInfo.Count; i++)
                        {
                            mucomADPCM2PCM.mucomPCMInfo inf = mucomADPCM2PCM.lstMucomPCMInfo[i];
                            if (pds.No == inf.no)
                            {
                                info = inf;
                                break;
                            }
                        }
                        if (info == null) return;

                        //ファイルの読み込み
                        buf = mucomADPCM2PCM.GetPcmData(info);
                        if (buf == null)
                        {
                            msgBox.setErrMsg(string.Format(
                                msg.get("E04007")
                                , v.fileName));
                            continue;
                        }

                        if (desVGM.info.format == enmFormat.XGM && v.isSecondary)
                        {
                            msgBox.setErrMsg(string.Format(
                                msg.get("E01017")
                                , v.fileName));
                            continue;
                        }

                        desVGM.chips[v.chip][v.isSecondary ? 1 : 0]
                            .StorePcm(
                            newDic
                            , new KeyValuePair<int, clsPcm>(pds.No, v)
                            , buf
                            , false
                            , 8000);

                        break;
                    case enmPcmDefineType.RawData:
                        //ファイルの読み込み
                        buf = Common.GetPCMDataFromFile(path, pds.FileName, 100, out isRaw, out is16bit, out samplerate);
                        if (buf == null)
                        {
                            msgBox.setErrMsg(string.Format(
                                msg.get("E04007")
                                , pds.FileName));
                            continue;
                        }
                        desVGM.chips[pds.chip][pds.isSecondary ? 1 : 0]
                            .StorePcmRawData(
                            pds
                            , buf
                            , isRaw
                            , is16bit
                            , samplerate);
                        break;
                    case enmPcmDefineType.Set:
                        //if(!desVGM.chips[pds.chip][pds.isSecondary ? 1 : 0].StorePcmCheck())
                        //{
                        //    return;
                        //}
                        if (desVGM.instPCM.ContainsKey(pds.No))
                        {
                            desVGM.instPCM.Remove(pds.No);
                        }
                        v = new clsPcm(
                            pds.No
                            , pcmDataSeqNum++
                            , pds.chip
                            , pds.isSecondary
                            , ""
                            , pds.BaseFreq
                            , 100
                            , pds.DatStartAdr
                            , pds.DatEndAdr
                            , pds.chip!= enmChipType.RF5C164 
                                ? (pds.DatEndAdr - pds.DatStartAdr + 1)
                                : (pds.DatLoopAdr - pds.DatStartAdr + 1 )
                            , pds.DatLoopAdr 
                            , false
                            , 8000
                            , pds.Option);
                        newDic.Add(pds.No, v);

                        break;
                }
            }

            desVGM.instPCM = newDic;

        }

        private void Result()
        {
            Disp("");

            string res = "";
            foreach (ClsChip[] chips in desVGM.chips.Values)
            {
                foreach (ClsChip chip in chips)
                {
                    res += DispPCMRegion(chip);
                }
            }

            if (res != "")
            {
                Disp(msg.get("I04012"));
                //Disp(" YM2608:  None");
                //Disp(" YM2610B: 0=A 1=B");
                //Disp(" RF5C164: None");
                Disp("");
                Disp(msg.get("I04013"));
                Disp("");
                Disp("");
                if (desVGM.c140[0].pcmDataEasy != null)
                    Disp(string.Format(msg.get("I04014")
                        , ((C140)desVGM.c140[0]).isSystem2 ? "" : "1"));
                if (desVGM.c140[1].pcmDataEasy != null)
                    Disp(string.Format(msg.get("I04015")
                        , ((C140)desVGM.c140[1]).isSystem2 ? "" : "1"));
                Disp("");

                Disp(msg.get("I04016"));
                Disp(msg.get("I04017"));
                Disp(res);
            }



            res = "";
            foreach (ClsChip[] chips in desVGM.chips.Values)
            {
                foreach (ClsChip chip in chips)
                {
                    res += DispPCMRegionDataBlock(chip);
                }
            }

            if (res != "")
            {
                Disp(msg.get("I04019"));
                Disp(msg.get("I04020"));
                Disp(res);
            }
        }

        private string DispPCMRegion(ClsChip c)
        {
            if (c.chipType != enmChipType.YM2610B)
            {
                if (c.pcmDataEasy == null && c.pcmDataDirect.Count == 0) return "";
            }
            else
            {
                YM2610B opnb = (YM2610B)c;
                if (opnb.pcmDataEasyA == null 
                    && opnb.pcmDataEasyB == null
                    && opnb.pcmDataDirectA.Count == 0
                    && opnb.pcmDataDirectB.Count == 0
                    ) return "";
            }

            string region = "";

            for (int i = 0; i < 256; i++)
            {
                if (!desVGM.instPCM.ContainsKey(i)) continue;
                if (desVGM.instPCM[i].chip != c.chipType) continue;
                if (desVGM.instPCM[i].isSecondary != c.IsSecondary) continue;

                region += c.DispRegion(desVGM.instPCM[i]);

            }

            long tl = 0;
            foreach (int i in desVGM.instPCM.Keys)
            {
                tl += desVGM.instPCM[i].size;
            }
            region +=(string.Format(msg.get("I04018"), tl));
            region+="\r\n";

            return region;
        }

        private string DispPCMRegionDataBlock(ClsChip c)
        {
            YM2610B opnb;
            string region = "";
            long tl = 0;

            if (c.chipType != enmChipType.YM2610B)
            {
                if (c.pcmDataEasy == null && c.pcmDataDirect.Count == 0) return "";

                if (c.pcmDataEasy != null)
                {
                    region += string.Format("{0,-10} {1,-7} ${2,-7:X6} ${3,-7:X6} ${4,-7:X6}  {5}\r\n"
                        , c.Name
                        , c.IsSecondary ? "SEC" : "PRI"
                        , 0
                        , c.pcmDataEasy.Length - 1
                        , c.pcmDataEasy.Length
                        , "AUTO"
                        );
                    tl += c.pcmDataEasy.Length;
                }
            }
            else
            {
                opnb = (YM2610B)c;
                if (opnb.pcmDataEasyA == null
                    && opnb.pcmDataEasyB == null
                    && opnb.pcmDataDirectA.Count == 0
                    && opnb.pcmDataDirectB.Count == 0
                    ) return "";

                if (opnb.pcmDataEasyA != null)
                {
                    region += string.Format("{0,-10} {1,-7} ${2,-7:X6} ${3,-7:X6} ${4,-7:X6}  {5}\r\n"
                        , opnb.Name+"_A"
                        , opnb.IsSecondary ? "SEC" : "PRI"
                        , 0
                        , opnb.pcmDataEasyA.Length - 1
                        , opnb.pcmDataEasyA.Length
                        , "AUTO"
                        );
                    tl += opnb.pcmDataEasyA.Length;
                }
                if (opnb.pcmDataEasyB != null)
                {
                    region += string.Format("{0,-10} {1,-7} ${2,-7:X6} ${3,-7:X6} ${4,-7:X6}  {5}\r\n"
                        , opnb.Name+"_B"
                        , opnb.IsSecondary ? "SEC" : "PRI"
                        , 0
                        , opnb.pcmDataEasyB.Length - 1
                        , opnb.pcmDataEasyB.Length
                        , "AUTO"
                        );
                    tl += opnb.pcmDataEasyB.Length;
                }

            }


            if (c.pcmDataDirect.Count > 0)
            {
                foreach (clsPcmDatSeq pds in desVGM.instPCMDatSeq)
                {
                    if (pds.type == enmPcmDefineType.Set) continue;
                    if (pds.type == enmPcmDefineType.Easy) continue;
                    if (pds.chip != c.chipType) continue;
                    if (pds.isSecondary != c.IsSecondary) continue;
                    if (!desVGM.chips[pds.chip][0].CanUsePICommand()) continue;

                    region += string.Format("{0,-10} {1,-7} ${2,-7:X6} ${3,-7:X6} ${4,-7:X6}  {5}\r\n"
                        , c.Name
                        , pds.isSecondary ? "SEC" : "PRI"
                        , pds.DatStartAdr
                        , pds.DatEndAdr
                        , pds.DatEndAdr - pds.DatStartAdr + 1
                        , pds.type == enmPcmDefineType.Easy ? "AUTO" : "MANUAL"
                        );
                    tl += pds.DatEndAdr - pds.DatStartAdr + 1;
                }
            }

            if (region != "")
            {
                region += (string.Format(msg.get("I04018"), tl));
                region += "\r\n";
            }

            return region;
        }



    }
}