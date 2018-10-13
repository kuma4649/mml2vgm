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
                Disp("Start mml2vgm core");
                Disp("");

                Disp(" ファイル存在チェック");
                if (!File.Exists(srcFn))
                {
                    msgBox.setErrMsg("ファイルが見つかりません。");
                    return -1;
                }

                Disp(" ソースファイルの取得");
                string path = Path.GetDirectoryName(Path.GetFullPath(srcFn));
                List<Line> src = GetSrc(File.ReadAllLines(srcFn), path);
                if (src == null)
                {
                    msgBox.setErrMsg("想定外のエラー　ソースファイルの取得に失敗");
                    return -1;
                }

                Disp(" テキスト解析");
                desVGM = new ClsVgm(stPath);
                if (desVGM.Analyze(src) != 0)
                {
                    msgBox.setErrMsg(string.Format(
                        "想定外のエラー　ソース解析に失敗?(analyze)(line:{0})"
                        , desVGM.lineNumber));
                    return -1;
                }

                Disp(" PCM定義&取得");
                if (desVGM.instPCM.Count > 0) GetPCMData(path);

                Disp(" MML文法解析");
                MMLAnalyze mmlAnalyze = new MMLAnalyze(desVGM);
                if (mmlAnalyze.Start() != 0)
                {
                    msgBox.setErrMsg(string.Format(
                        "想定外のエラー　MML解析に失敗?(MMLAnalyze)(line:{0})"
                        , mmlAnalyze.lineNumber));
                    return -1;
                }

                byte[] desBuf = null;
                switch (desVGM.info.format)
                {
                    case enmFormat.VGM:
                        Disp(" Start VGM_GetByteData");
                        desBuf = desVGM.Vgm_getByteData(mmlAnalyze.mmlData);
                        Disp(" End   VGM_GetByteData");
                        break;
                    case enmFormat.XGM:
                        Disp(" Start XGM_GetByteData");
                        desBuf = desVGM.Xgm_getByteData(mmlAnalyze.mmlData);
                        Disp(" End   XGM_GetByteData");
                        break;
                    default:
                        break;
                }

                if (desBuf == null)
                {
                    msgBox.setErrMsg(string.Format(
                        "想定外のエラー　ソース解析に失敗?(getByteData)(line:{0})"
                        , desVGM.lineNumber));
                    return -1;
                }

                Disp(" ファイル出力");
                outFile(desBuf);


                Result();

                return 0;
            }
            catch (Exception ex)
            {
                msgBox.setErrMsg(string.Format(
@"想定外のエラー　line:{0}
メッセージ:
{1}
スタックトレース:
{2}
", desVGM.lineNumber, ex.Message, ex.StackTrace));
                return -1;
            }
            finally
            {
                Disp("End mml2vgm core");
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
                                "インクルードファイル({0})が見つかりません。"
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
            foreach (KeyValuePair<int, clsPcm> v in desVGM.instPCM)
            {

                byte[] buf = Common.GetPCMDataFromFile(path, v.Value, out bool is16bit, out int samplerate);
                if (buf == null)
                {
                    msgBox.setErrMsg(string.Format(
                        "PCMファイルの読み込みに失敗しました。(filename:{0})"
                        , v.Value.fileName));
                    continue;
                }

                desVGM.chips[v.Value.chip][v.Value.isSecondary ? 1 : 0]
                    .StorePcm(newDic, v, buf, is16bit, samplerate);
            }

            desVGM.instPCM = newDic;

        }

        private void Result()
        {
            Disp("");

            string res = "";
            res += DispPCMRegion(desVGM.segapcm[0]);
            res += DispPCMRegion(desVGM.segapcm[1]);
            res += DispPCMRegion(desVGM.c140[0]);
            res += DispPCMRegion(desVGM.c140[1]);

            if (res != "")
            {
                Disp("MODE:");
                //Disp(" YM2608:  None");
                //Disp(" YM2610B: 0=A 1=B");
                //Disp(" RF5C164: None");
                Disp(" SegaPCM: None");
                Disp(" C140:    0=8bit 1=13bit(Compressed 8bit)");
                Disp("");
                Disp("STATUS:");
                Disp(" USED: USED in song");
                Disp(" NONE: Not Used in song");
                Disp(" Error: Invalid Parameter...???");
                Disp("");
                Disp("");
                if (desVGM.c140[0].pcmData != null)
                    Disp(string.Format("C140 PRI : SYSTEM2{0}", ((C140)desVGM.c140[0]).isSystem2 ? "" : "1"));
                if (desVGM.c140[1].pcmData != null)
                    Disp(string.Format("C140 SEC : SYSTEM2{0}", ((C140)desVGM.c140[1]).isSystem2 ? "" : "1"));
                Disp("");

                Disp("-- SAMPLE LIST --");
                Disp("CHIP       PRI/SEC SMPID BANK START(H) END(H)   LOOP(H)  LENGTH   MODE STATUS");
                Disp(res);
            }

        }

        private string DispPCMRegion(ClsChip c)
        {
            if (c.pcmData == null) return "";

            string region = "";

            for (int i = 0; i < 256; i++)
            {
                if (!desVGM.instPCM.ContainsKey(i)) continue;
                if (desVGM.instPCM[i].chip != c.chipType) continue;

                region+=string.Format("{0,-10} {1,-7} {2,-5:D3} {3,-4:D2} {4,-8:X4} {5,-8:X4} {6,-8:X4} {7,8} {8,4} {9}\r\n"
                    , c.Name
                    , c.IsSecondary ? "SEC" : "PRI"
                    , i
                    , desVGM.instPCM[i].stAdr >> 16
                    , desVGM.instPCM[i].stAdr & 0xffff
                    , desVGM.instPCM[i].edAdr & 0xffff
                    , (desVGM.instPCM[i].loopAdr == -1) ? "N/A" : string.Format("{0:X4}", (desVGM.instPCM[i].loopAdr & 0xffff))
                    , desVGM.instPCM[i].size
                    , desVGM.instPCM[i].is16bit ? 1 : 0
                    , desVGM.instPCM[i].status.ToString()
                    );
            }

            region+=(string.Format(" Total Length : {0} byte\r\n", c.pcmData.Length - 15));
            region+="\r\n";

            return region;
        }
    }
}