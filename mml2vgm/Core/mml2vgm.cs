using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Drawing;

namespace Core
{
    /// <summary>
    /// コンパイラ
    /// </summary>
    public class Mml2vgm
    {

        public ClsVgm desVGM = null;
        public outDatum[] desBuf = null;
        public bool outVgmFile = true;
        public bool outTraceInfoFile = false;
        public string desTiFn = "";

        private string srcFn;
        private string desFn;
        private string stPath;
        private Action<string> Disp = null;
        private string wrkPath;
        private string[] srcTxt;

        private int pcmDataSeqNum = 0;
        public bool doSkip = false;
        public Point caretPoint = Point.Empty;
        private bool bufferMode = false;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="srcFn">ソースファイル</param>
        /// <param name="desFn">出力ファイル</param>
        /// <param name="stPath">アプリケーションパス</param>
        /// <param name="disp">メッセージ表示メソッド</param>
        public Mml2vgm(string srcFn, string desFn, string stPath, Action<string> disp)
        {
            this.srcFn = srcFn;
            this.desFn = desFn;
            this.stPath = stPath;
            this.Disp = disp;
            this.wrkPath = Path.GetDirectoryName(Path.GetFullPath(srcFn));
            this.srcTxt = null;
            bufferMode = false;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="srcFn">ソースファイル</param>
        /// <param name="desFn">出力ファイル</param>
        /// <param name="stPath">アプリケーションパス</param>
        /// <param name="disp">メッセージ表示メソッド</param>
        /// <param name="wrkPath">取り込みファイルが存在するパス(通常はソースファイルと同じパス)</param>
        public Mml2vgm(string srcFn, string desFn, string stPath, Action<string> disp ,string wrkPath)
        {
            this.srcFn = srcFn;
            this.desFn = desFn;
            this.stPath = stPath;
            this.Disp = disp;
            this.wrkPath = wrkPath;
            this.srcTxt = null;
            bufferMode = false;
        }

        public Mml2vgm(string[] srcTxt, string srcFn, string stPath, Action<string> disp, string wrkPath)
        {
            this.srcFn = srcFn;
            this.desFn = null;
            this.stPath = stPath;
            this.Disp = disp;
            this.wrkPath = wrkPath;
            this.srcTxt = srcTxt;
            bufferMode = true;
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

                List<Line> src = null;

                if (!bufferMode)
                {
                    Disp(msg.get("I04001"));
                    srcFn = srcFn.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
                    if (!File.Exists(srcFn))
                    {
                        msgBox.setErrMsg(msg.get("E04000"), new LinePos(srcFn));
                        return -1;
                    }

                    Disp(msg.get("I04002"));
                    //string path = Path.GetDirectoryName(Path.GetFullPath(srcFn));
                    //wrkPath インクルードファイル取り込み対象パス
                    srcTxt = File.ReadAllLines(srcFn);
                }

                src = GetSrc(srcTxt, wrkPath);
                if (src == null)
                {
                    msgBox.setErrMsg(msg.get("E04001"), new LinePos(srcFn));
                    return -1;
                }

                Disp(msg.get("I04003"));
                desVGM = new ClsVgm(stPath);
                desVGM.doSkip = doSkip;
                desVGM.caretPoint = caretPoint;
                if (desVGM.Analyze(src) != 0)
                {
                    msgBox.setErrMsg(string.Format(
                        msg.get("E04002")
                        , desVGM.linePos.row), desVGM.linePos);
                    return -1;
                }

                Disp(msg.get("I04004"));
                //wrkPath PCMファイル取り込み対象パス
                if (desVGM.instPCMDatSeq.Count > 0) GetPCMData(wrkPath);

                Disp(msg.get("I04005"));

                MMLAnalyze mmlAnalyze = new MMLAnalyze(desVGM);
                mmlAnalyze.doSkip = doSkip;
                mmlAnalyze.caretPoint = caretPoint;

                if (mmlAnalyze.Start() != 0)
                {
                    msgBox.setErrMsg(string.Format(
                        msg.get("E04003")
                        , mmlAnalyze.linePos.row), mmlAnalyze.linePos);
                    return -1;
                }

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
                        , desVGM.linePos.row), desVGM.linePos);
                    return -1;
                }

                if (outVgmFile && !bufferMode)
                {
                    Disp(msg.get("I04021"));
                    OutVgmFile(desBuf);
                }

                if (outTraceInfoFile && !bufferMode)
                {
                    Disp(msg.get("I04022"));
                    OutTraceInfoFile(desBuf);
                }

                Result();

                FileInformation.loopCounter = desVGM.loopClock;
                FileInformation.totalCounter = desVGM.lClock;
                FileInformation.format =desVGM.info.format;

                return 0;
            }
            catch (Exception ex)
            {
                Disp(ex.Message);
                msgBox.setErrMsg(string.Format(msg.get("E04005")
                    , (desVGM.linePos == null ? "-" : desVGM.linePos.row.ToString())
                    , ex.Message
                    , ex.StackTrace), desVGM.linePos);
                return -1;
            }
            finally
            {
                Disp(msg.get("I04011"));
                Disp("");
            }
        }

        private void OutVgmFile(outDatum[] desBuf)
        {
            List<byte> lstBuf = new List<byte>();
            int skipCount = 0;
            foreach (outDatum od in desBuf)
            {

                //ダミーコマンドをスキップする
                if (skipCount > 0)
                {
                    skipCount--;
                    continue;
                }
                if (od.val == 0x2f //dummyChipコマンド　(第2引数：chipID 第３引数:isSecondary)
                    && od.type == enmMMLType.Rest//ここで指定できるmmlコマンドは元々はChipに送信することのないコマンドのみ(さもないと、通常のコマンドのデータと見分けがつかなくなる可能性がある)
                    )
                {
                    skipCount = 2;
                    continue;
                }

                lstBuf.Add(od.val);
            }
            byte[] bufs = lstBuf.ToArray();

            if (Path.GetExtension(desFn).ToLower() != ".vgz")
            {
                if (desVGM.info.format == enmFormat.VGM)
                {
                    log.Write("VGMファイル出力");
                    File.WriteAllBytes(
                        desFn
                        , bufs);
                }
                else
                {
                    log.Write("XGMファイル出力");
                    File.WriteAllBytes(
                        Path.Combine(
                            Path.GetDirectoryName(desFn)
                            , Path.GetFileNameWithoutExtension(desFn) + ".xgm"
                            )
                        , bufs);
                }
                return;
            }

            log.Write(msg.get("I04021"));

            int num;
            byte[] buf = new byte[1024];

            MemoryStream inStream = new MemoryStream(bufs);
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

        private void OutTraceInfoFile(outDatum[] desBuf)
        {
            List<string> buf = new List<string>();
            foreach (outDatum od in desBuf)
            {
                if (od.linePos != null)
                {
                    buf.Add(string.Format(
                        "File:[{0}] Row:[{1}] Col:[{2}] Len:[{3}] Chip:[{4}] Secondary:[{5}] Part:[{6}] Ch:[{7}] MMLType:[{8}] Val:[{9:X2}]"
                        , od.linePos.filename
                        , od.linePos.row
                        , od.linePos.col
                        , od.linePos.length
                        , od.linePos.chip
                        , od.linePos.isSecondary
                        , od.linePos.part
                        , od.linePos.ch
                        , od.type
                        , od.val));
                }
                else
                {
                    buf.Add(string.Format(
                        "File:[{0}] Row:[{1}] Col:[{2}] Len:[{3}] Chip:[{4}] Secondary:[{5}] Part:[{6}] Ch:[{7}] MMLType:[{8}] Val:[{9:X2}]"
                        , ""
                        , ""
                        , ""
                        , ""
                        , ""
                        , ""
                        , ""
                        , ""
                        , od.type
                        , od.val));
                }
            }

            log.Write(msg.get("I04022"));
            File.WriteAllLines(desTiFn, buf, System.Text.Encoding.UTF8);
        }

        private List<Line> GetSrc(string[] srcBuf, string path)
        {
            try
            {
                List<Line> des = new List<Line>();
                return GetSrcRec(srcBuf, des, path, srcFn, 0);
            }
            catch
            {
                msgBox.setErrMsg(string.Format(
                                msg.get("E04008")
                                , srcFn), new LinePos(srcFn));
                return null;
            }
        }

        private List<Line> GetSrcRec(string[] srcBuf, List<Line> des, string path, string sourceFileName, int sourceLineNumber)
        {
            foreach (string s in srcBuf)
            {
                Line line = new Line(new LinePos(sourceFileName, sourceLineNumber), s);
                des.Add(line);
                sourceLineNumber++;

                if (!string.IsNullOrEmpty(s)
                    && s.TrimStart().Length > 2
                    && s.TrimStart().Substring(0, 2) == "'+")
                {
                    string includeFn = s.Substring(2).Trim().Trim('"');
                    includeFn = includeFn.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
                    if (!File.Exists(includeFn))
                    {
                        includeFn = Path.Combine(path, includeFn);
                        includeFn = includeFn.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
                        if (!File.Exists(includeFn))
                        {
                            msgBox.setErrMsg(string.Format(
                                msg.get("E04006")
                                , includeFn), new LinePos(sourceFileName, sourceLineNumber, 0, s.Length));
                            return null;
                        }
                    }
                    string[] incBuf = File.ReadAllLines(includeFn);
                    des = GetSrcRec(incBuf, des, path, includeFn, 0);
                }
            }

            return des;
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
                                , v.fileName), new LinePos(pds.FileName));
                            continue;
                        }

                        if (desVGM.info.format == enmFormat.XGM && v.isSecondary)
                        {
                            msgBox.setErrMsg(string.Format(
                                msg.get("E01017")
                                , v.fileName), new LinePos(pds.FileName));
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
                                , v.fileName), new LinePos(pds.FileName));
                            continue;
                        }

                        if (desVGM.info.format == enmFormat.XGM && v.isSecondary)
                        {
                            msgBox.setErrMsg(string.Format(
                                msg.get("E01017")
                                , v.fileName), new LinePos(pds.FileName));
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
                                , pds.FileName), new LinePos(pds.FileName));
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
                            , pds.chip != enmChipType.RF5C164
                                ? (pds.DatEndAdr - pds.DatStartAdr + 1)
                                : (pds.DatLoopAdr - pds.DatStartAdr + 1)
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