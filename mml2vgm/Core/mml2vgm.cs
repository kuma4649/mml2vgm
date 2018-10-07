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

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="srcFn">ソースファイル</param>
        /// <param name="desFn">出力ファイル</param>
        public Mml2vgm(string srcFn, string desFn, string stPath)
        {
            this.srcFn = srcFn;
            this.desFn = desFn;
            this.stPath = stPath;
        }

        /// <summary>
        /// コンパイル開始
        /// </summary>
        /// <returns></returns>
        public int Start()
        {
            try
            {
                log.Write("start mml2vgm core");

                log.Write("ファイル存在チェック");
                if (!File.Exists(srcFn))
                {
                    msgBox.setErrMsg("ファイルが見つかりません。");
                    return -1;
                }

                log.Write("ソースファイルの取得");
                string path = Path.GetDirectoryName(Path.GetFullPath(srcFn));
                List<Line> src = GetSrc(File.ReadAllLines(srcFn), path);
                if (src == null)
                {
                    msgBox.setErrMsg("想定外のエラー　ソースファイルの取得に失敗");
                    return -1;
                }

                log.Write("テキスト解析");
                desVGM = new ClsVgm(stPath);
                if (desVGM.Analyze(src) != 0)
                {
                    msgBox.setErrMsg(string.Format(
                        "想定外のエラー　ソース解析に失敗?(analyze)(line:{0})"
                        , desVGM.lineNumber));
                    return -1;
                }

                log.Write("PCM定義&取得");
                if (desVGM.instPCM.Count > 0) GetPCMData(path);

                log.Write("MML文法解析");
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
                        log.Write("MML解析開始(start VGM_GetByteData)");
                        desBuf = desVGM.Vgm_getByteData(mmlAnalyze.mmlData);
                        break;
                    case enmFormat.XGM:
                        log.Write("MML解析開始(start XGM_GetByteData)");
                        desBuf = desVGM.Xgm_getByteData(mmlAnalyze.mmlData);
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

                log.Write("ファイル出力");
                outFile(desBuf);

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
                log.Write("end mml2vgm core");
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

                byte[] buf = Common.GetPCMDataFromFile(path, v.Value, out bool is16bit);
                if (buf == null)
                {
                    msgBox.setErrMsg(string.Format(
                        "PCMファイルの読み込みに失敗しました。(filename:{0})"
                        , v.Value.fileName));
                    continue;
                }

                desVGM.chips[v.Value.chip][v.Value.isSecondary ? 1 : 0]
                    .StorePcm(newDic, v, buf, is16bit);
            }

            desVGM.instPCM = newDic;

        }

    }
}