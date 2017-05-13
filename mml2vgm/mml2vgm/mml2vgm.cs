using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO.Compression;

namespace mml2vgm
{
    /// <summary>
    /// コンパイラ
    /// </summary>
    public class mml2vgm
    {

        private string srcFn;
        private string desFn;
        public clsVgm desVGM = null;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="srcFn">ソースファイル</param>
        /// <param name="desFn">出力ファイル</param>
        public mml2vgm(string srcFn, string desFn)
        {

            this.srcFn = srcFn;
            this.desFn = desFn;

        }

        /// <summary>
        /// コンパイル開始
        /// </summary>
        /// <returns></returns>
        public int Start()
        {

            desVGM = null;

            try
            {

                // ファイル存在チェック
                if (!File.Exists(srcFn))
                {
                    msgBox.setErrMsg("ファイルが見つかりません。");
                    return -1;
                }

                // ファイルの読み込み
                string[] srcBuf = File.ReadAllLines(srcFn);
                string path = Path.GetDirectoryName(Path.GetFullPath(srcFn));

                desVGM = new clsVgm();

                List<Line> src = new List<Line>();
                int ln = 1;
                foreach (string s in srcBuf)
                {
                    if (!string.IsNullOrEmpty(s) && s.TrimStart().Length > 2 && s.TrimStart().Substring(0, 2) == "'+")
                    {
                        string includeFn = s.Substring(2).Trim().Trim('"');
                        if (!File.Exists(includeFn))
                        {
                            includeFn = Path.Combine(path, includeFn);
                            if (!File.Exists(includeFn))
                            {
                                msgBox.setErrMsg(string.Format("インクルードファイル({0})が見つかりません。",includeFn));
                                return -1;
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

                // 解析
                int ret;
                ret= desVGM.analyze(src);

                if (ret != 0)
                {
                    msgBox.setErrMsg(string.Format("想定外のエラー　ソース解析に失敗?(analyze)(line:{0})", desVGM.lineNumber));
                    return -1;
                }

                // PCM定義あり?
                if (desVGM.instPCM.Count > 0)
                {
                    getPCMData(path);
                }


                // 解析した情報をもとにVGMファイル作成
                byte[] desBuf;
                if (desVGM.format == clsVgm.enmFormat.VGM)
                {
                    desBuf = desVGM.getByteData();
                }
                else
                {
                    desBuf = desVGM.xgm_getByteData();
                }
                

                if (desBuf == null)
                {
                    msgBox.setErrMsg(string.Format("想定外のエラー　ソース解析に失敗?(getByteData)(line:{0})", desVGM.lineNumber));
                    return -1;
                }


                if (Path.GetExtension(desFn).ToLower() != ".vgz")
                {
                    if (desVGM.format == clsVgm.enmFormat.VGM)
                    {
                        // ファイル出力
                        File.WriteAllBytes(desFn, desBuf);
                    }
                    else
                    {
                        //XGM
                        // ファイル出力
                        File.WriteAllBytes(Path.Combine(Path.GetDirectoryName(desFn), Path.GetFileNameWithoutExtension(desFn) + ".xgm"), desBuf);
                    }
                }
                else
                {
                    int num;
                    byte[] buf = new byte[1024];

                    MemoryStream inStream = new MemoryStream(desBuf);
                    FileStream outStream = new FileStream(desFn, FileMode.Create);
                    GZipStream compStream = new GZipStream(outStream, CompressionMode.Compress);

                    using (inStream)
                    using (outStream)
                    using (compStream)
                    {
                        while ((num = inStream.Read(buf, 0, buf.Length)) > 0)
                        {
                            compStream.Write(buf, 0, num);
                        }
                    }
                }

                // 終了
                return 0;
            }
            catch (Exception ex)
            {
                msgBox.setErrMsg(string.Format("想定外のエラー　line:{0} \r\nメッセージ:\r\n{1}\r\nスタックトレース:\r\n{2}\r\n", desVGM.lineNumber, ex.Message, ex.StackTrace));
                return -1;
            }
        }

        private void getPCMData(string path)
        {
            byte[] tbufYM2612 = new byte[7] { 0x67, 0x66, 0x00, 0x00, 0x00, 0x00, 0x00 };
            long pYM2612 = 0L;
            bool uYM2612 = false;

            byte[] tbufYM2612X = new byte[0];
            long pYM2612X = 0L;
            bool uYM2612X = false;

            byte[] tbufRf5c164P = new byte[12] { 0xb1, 0x07, 0x00, 0x67, 0x66, 0xc1, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            byte[] tbufRf5c164S = new byte[12] { 0xb1, 0x87, 0x00, 0x67, 0x66, 0xc1, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            long pRf5c164P = 0L;
            long pRf5c164S = 0L;
            bool uRf5c164P = false;
            bool uRf5c164S = false;

            //
            byte[] tbufYm2608AdpcmP = new byte[15] { 0x67, 0x66, 0x81, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            byte[] tbufYm2608AdpcmS = new byte[15] { 0x67, 0x66, 0x81, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            long pYm2608AdpcmP = 0L;
            long pYm2608AdpcmS = 0L;
            bool uYm2608AdpcmP = false;
            bool uYm2608AdpcmS = false;

            //
            byte[] tbufYm2610AdpcmAP = new byte[15] { 0x67, 0x66, 0x82, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            byte[] tbufYm2610AdpcmAS = new byte[15] { 0x67, 0x66, 0x82, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            long pYm2610AdpcmAP = 0L;
            long pYm2610AdpcmAS = 0L;
            bool uYm2610AdpcmAP = false;
            bool uYm2610AdpcmAS = false;

            //
            byte[] tbufYm2610AdpcmBP = new byte[15] { 0x67, 0x66, 0x83, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            byte[] tbufYm2610AdpcmBS = new byte[15] { 0x67, 0x66, 0x83, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            long pYm2610AdpcmBP = 0L;
            long pYm2610AdpcmBS = 0L;
            bool uYm2610AdpcmBP = false;
            bool uYm2610AdpcmBS = false;

            //
            byte[] tbufSegaPCMP = new byte[15] { 0x67, 0x66, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            byte[] tbufSegaPCMS = new byte[15] { 0x67, 0x66, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            long pSegaPCMP = 0L;
            long pSegaPCMS = 0L;
            bool uSegaPCMP = false;
            bool uSegaPCMS = false;

            byte[] tbufHuC6280P = new byte[7] { 0x67, 0x66, 0x05, 0x00, 0x00, 0x00, 0x00 };
            byte[] tbufHuC6280S = new byte[7] { 0x67, 0x66, 0x05, 0x00, 0x00, 0x00, 0x00 };
            long pHuC6280P = 0L;
            long pHuC6280S = 0L;
            bool uHuC6280P = false;
            bool uHuC6280S = false;

            Dictionary<int, clsPcm> newDic = new Dictionary<int, clsPcm>();
            foreach (KeyValuePair<int, clsPcm> v in desVGM.instPCM)
            {

                bool is16bit;
                byte[] buf = getPCMDataFromFile(path, v.Value,out is16bit);
                if (buf == null)
                {
                    msgBox.setErrMsg(string.Format("PCMファイルの読み込みに失敗しました。(filename:{0})", v.Value.fileName));
                    continue;
                }

                long size = buf.Length;

                if (v.Value.chip == enmChipType.YM2608)
                {
                    size = storeAdpcmYM2608(ref tbufYm2608AdpcmP, ref tbufYm2608AdpcmS, ref pYm2608AdpcmP, ref pYm2608AdpcmS, ref uYm2608AdpcmP, ref uYm2608AdpcmS, newDic, v, is16bit, ref buf);
                }
                else if (v.Value.chip == enmChipType.YM2610B)
                {
                    size = storeAdpcmYM2610B(ref tbufYm2610AdpcmAP, ref tbufYm2610AdpcmAS, ref pYm2610AdpcmAP, ref pYm2610AdpcmAS, ref uYm2610AdpcmAP, ref uYm2610AdpcmAS, ref tbufYm2610AdpcmBP, ref tbufYm2610AdpcmBS, ref pYm2610AdpcmBP, ref pYm2610AdpcmBS, ref uYm2610AdpcmBP, ref uYm2610AdpcmBS, newDic, v, is16bit, ref buf);
                }
                else if (v.Value.chip == enmChipType.SEGAPCM)
                {
                    storePcmSegaPCM(ref tbufSegaPCMP, ref tbufSegaPCMS, ref pSegaPCMP, ref pSegaPCMS, ref uSegaPCMP, ref uSegaPCMS, newDic, v, ref buf, ref size);
                }
                else if (v.Value.chip == enmChipType.YM2612)
                {
                    uYM2612 = storePcmYM2612(ref tbufYM2612, ref pYM2612, newDic, v, buf, size);
                }
                else if (v.Value.chip == enmChipType.RF5C164)
                {
                    storePcmRF5C164(ref tbufRf5c164P, ref tbufRf5c164S, ref pRf5c164P, ref pRf5c164S, ref uRf5c164P, ref uRf5c164S, newDic, v, ref buf, ref size);
                }
                else if (v.Value.chip == enmChipType.HuC6280)
                {
                    if (!v.Value.isSecondary)
                    {
                        uHuC6280P = storePcmHuC6280(ref tbufHuC6280P, ref pHuC6280P, newDic, v, buf, size);
                    }
                    else
                    {
                        uHuC6280S = storePcmHuC6280(ref tbufHuC6280S, ref pHuC6280S, newDic, v, buf, size);
                    }
                }
                else if (v.Value.chip == enmChipType.YM2612X)
                {
                    uYM2612X = storePcmYM2612X(ref tbufYM2612X, ref pYM2612X, newDic, v, buf, size);
                }
            }


            tbufYm2608AdpcmP[3] = (byte)((tbufYm2608AdpcmP.Length - 15 + 8) & 0xff);
            tbufYm2608AdpcmP[4] = (byte)(((tbufYm2608AdpcmP.Length - 15 + 8) & 0xff00) / 0x100);
            tbufYm2608AdpcmP[5] = (byte)(((tbufYm2608AdpcmP.Length - 15 + 8) & 0xff0000) / 0x10000);
            tbufYm2608AdpcmP[6] = (byte)(((tbufYm2608AdpcmP.Length - 15 + 8) & 0x7f000000) / 0x1000000);

            tbufYm2608AdpcmP[7] = (byte)((tbufYm2608AdpcmP.Length - 15 + 8) & 0xff);
            tbufYm2608AdpcmP[8] = (byte)(((tbufYm2608AdpcmP.Length - 15 + 8) & 0xff00) / 0x100);
            tbufYm2608AdpcmP[9] = (byte)(((tbufYm2608AdpcmP.Length - 15 + 8) & 0xff0000) / 0x10000);
            tbufYm2608AdpcmP[10] = (byte)(((tbufYm2608AdpcmP.Length - 15 + 8) & 0x7f000000) / 0x1000000);
            desVGM.ym2608[0].pcmData = uYm2608AdpcmP ? tbufYm2608AdpcmP : null;

            tbufYm2608AdpcmS[3] = (byte)((tbufYm2608AdpcmS.Length - 15 + 8) & 0xff);
            tbufYm2608AdpcmS[4] = (byte)(((tbufYm2608AdpcmS.Length - 15 + 8) & 0xff00) / 0x100);
            tbufYm2608AdpcmS[5] = (byte)(((tbufYm2608AdpcmS.Length - 15 + 8) & 0xff0000) / 0x10000);
            tbufYm2608AdpcmS[6] = (byte)(((tbufYm2608AdpcmS.Length - 15 + 8) & 0x7f000000) / 0x1000000);
            tbufYm2608AdpcmS[6] |= 0x80;

            tbufYm2608AdpcmS[7] = (byte)((tbufYm2608AdpcmS.Length - 15 + 8) & 0xff);
            tbufYm2608AdpcmS[8] = (byte)(((tbufYm2608AdpcmS.Length - 15 + 8) & 0xff00) / 0x100);
            tbufYm2608AdpcmS[9] = (byte)(((tbufYm2608AdpcmS.Length - 15 + 8) & 0xff0000) / 0x10000);
            tbufYm2608AdpcmS[10] = (byte)(((tbufYm2608AdpcmS.Length - 15 + 8) & 0x7f000000) / 0x1000000);
            desVGM.ym2608[1].pcmData = uYm2608AdpcmS ? tbufYm2608AdpcmS : null;


            tbufYm2610AdpcmAP[3] = (byte)((tbufYm2610AdpcmAP.Length - 15+8) & 0xff);
            tbufYm2610AdpcmAP[4] = (byte)(((tbufYm2610AdpcmAP.Length - 15 + 8) & 0xff00) / 0x100);
            tbufYm2610AdpcmAP[5] = (byte)(((tbufYm2610AdpcmAP.Length - 15 + 8) & 0xff0000) / 0x10000);
            tbufYm2610AdpcmAP[6] = (byte)(((tbufYm2610AdpcmAP.Length - 15 + 8) & 0x7f000000) / 0x1000000);

            tbufYm2610AdpcmAP[7] = (byte)((tbufYm2610AdpcmAP.Length - 15 + 8) & 0xff);
            tbufYm2610AdpcmAP[8] = (byte)(((tbufYm2610AdpcmAP.Length - 15 + 8) & 0xff00) / 0x100);
            tbufYm2610AdpcmAP[9] = (byte)(((tbufYm2610AdpcmAP.Length - 15 + 8) & 0xff0000) / 0x10000);
            tbufYm2610AdpcmAP[10] = (byte)(((tbufYm2610AdpcmAP.Length - 15 + 8) & 0x7f000000) / 0x1000000);
            desVGM.ym2610b[0].pcmDataA = uYm2610AdpcmAP ? tbufYm2610AdpcmAP : null;

            tbufYm2610AdpcmAS[3] = (byte)((tbufYm2610AdpcmAS.Length - 15 + 8) & 0xff);
            tbufYm2610AdpcmAS[4] = (byte)(((tbufYm2610AdpcmAS.Length - 15 + 8) & 0xff00) / 0x100);
            tbufYm2610AdpcmAS[5] = (byte)(((tbufYm2610AdpcmAS.Length - 15 + 8) & 0xff0000) / 0x10000);
            tbufYm2610AdpcmAS[6] = (byte)(((tbufYm2610AdpcmAS.Length - 15 + 8) & 0x7f000000) / 0x1000000);
            tbufYm2610AdpcmAS[6] |= 0x80;

            tbufYm2610AdpcmAS[7] = (byte)((tbufYm2610AdpcmAS.Length - 15 + 8) & 0xff);
            tbufYm2610AdpcmAS[8] = (byte)(((tbufYm2610AdpcmAS.Length - 15 + 8) & 0xff00) / 0x100);
            tbufYm2610AdpcmAS[9] = (byte)(((tbufYm2610AdpcmAS.Length - 15 + 8) & 0xff0000) / 0x10000);
            tbufYm2610AdpcmAS[10] = (byte)(((tbufYm2610AdpcmAS.Length - 15 + 8) & 0x7f000000) / 0x1000000);
            desVGM.ym2610b[1].pcmDataA = uYm2610AdpcmAS ? tbufYm2610AdpcmAS : null;

            tbufYm2610AdpcmBP[3] = (byte)((tbufYm2610AdpcmBP.Length - 15 + 8) & 0xff);
            tbufYm2610AdpcmBP[4] = (byte)(((tbufYm2610AdpcmBP.Length - 15 + 8) & 0xff00) / 0x100);
            tbufYm2610AdpcmBP[5] = (byte)(((tbufYm2610AdpcmBP.Length - 15 + 8) & 0xff0000) / 0x10000);
            tbufYm2610AdpcmBP[6] = (byte)(((tbufYm2610AdpcmBP.Length - 15 + 8) & 0x7f000000) / 0x1000000);

            tbufYm2610AdpcmBP[7] = (byte)((tbufYm2610AdpcmBP.Length - 15 + 8) & 0xff);
            tbufYm2610AdpcmBP[8] = (byte)(((tbufYm2610AdpcmBP.Length - 15 + 8) & 0xff00) / 0x100);
            tbufYm2610AdpcmBP[9] = (byte)(((tbufYm2610AdpcmBP.Length - 15 + 8) & 0xff0000) / 0x10000);
            tbufYm2610AdpcmBP[10] = (byte)(((tbufYm2610AdpcmBP.Length - 15 + 8) & 0x7f000000) / 0x1000000);
            desVGM.ym2610b[0].pcmDataB = uYm2610AdpcmBP ? tbufYm2610AdpcmBP : null;

            tbufYm2610AdpcmBS[3] = (byte)((tbufYm2610AdpcmBS.Length - 15 + 8) & 0xff);
            tbufYm2610AdpcmBS[4] = (byte)(((tbufYm2610AdpcmBS.Length - 15 + 8) & 0xff00) / 0x100);
            tbufYm2610AdpcmBS[5] = (byte)(((tbufYm2610AdpcmBS.Length - 15 + 8) & 0xff0000) / 0x10000);
            tbufYm2610AdpcmBS[6] = (byte)(((tbufYm2610AdpcmBS.Length - 15 + 8) & 0x7f000000) / 0x1000000);
            tbufYm2610AdpcmBS[6] |= 0x80;

            tbufYm2610AdpcmBS[7] = (byte)((tbufYm2610AdpcmBS.Length - 15 + 8) & 0xff);
            tbufYm2610AdpcmBS[8] = (byte)(((tbufYm2610AdpcmBS.Length - 15 + 8) & 0xff00) / 0x100);
            tbufYm2610AdpcmBS[9] = (byte)(((tbufYm2610AdpcmBS.Length - 15 + 8) & 0xff0000) / 0x10000);
            tbufYm2610AdpcmBS[10] = (byte)(((tbufYm2610AdpcmBS.Length - 15 + 8) & 0x7f000000) / 0x1000000);
            desVGM.ym2610b[1].pcmDataB = uYm2610AdpcmBS ? tbufYm2610AdpcmBS : null;


            tbufSegaPCMP[3] = (byte)((tbufSegaPCMP.Length - 15 + 8) & 0xff);
            tbufSegaPCMP[4] = (byte)(((tbufSegaPCMP.Length - 15 + 8) & 0xff00) / 0x100);
            tbufSegaPCMP[5] = (byte)(((tbufSegaPCMP.Length - 15 + 8) & 0xff0000) / 0x10000);
            tbufSegaPCMP[6] = (byte)(((tbufSegaPCMP.Length - 15 + 8) & 0x7f000000) / 0x1000000);

            tbufSegaPCMP[7] = (byte)((tbufSegaPCMP.Length - 15 + 8) & 0xff);
            tbufSegaPCMP[8] = (byte)(((tbufSegaPCMP.Length - 15 + 8) & 0xff00) / 0x100);
            tbufSegaPCMP[9] = (byte)(((tbufSegaPCMP.Length - 15 + 8) & 0xff0000) / 0x10000);
            tbufSegaPCMP[10] = (byte)(((tbufSegaPCMP.Length - 15 + 8) & 0x7f000000) / 0x1000000);
            desVGM.segapcm[0].pcmData = uSegaPCMP ? tbufSegaPCMP : null;

            tbufSegaPCMS[3] = (byte)((tbufSegaPCMS.Length - 15 + 8) & 0xff);
            tbufSegaPCMS[4] = (byte)(((tbufSegaPCMS.Length - 15 + 8) & 0xff00) / 0x100);
            tbufSegaPCMS[5] = (byte)(((tbufSegaPCMS.Length - 15 + 8) & 0xff0000) / 0x10000);
            tbufSegaPCMS[6] = (byte)(((tbufSegaPCMS.Length - 15 + 8) & 0x7f000000) / 0x1000000);
            tbufSegaPCMS[6] |= 0x80;

            tbufSegaPCMS[7] = (byte)((tbufSegaPCMS.Length - 15 + 8) & 0xff);
            tbufSegaPCMS[8] = (byte)(((tbufSegaPCMS.Length - 15 + 8) & 0xff00) / 0x100);
            tbufSegaPCMS[9] = (byte)(((tbufSegaPCMS.Length - 15 + 8) & 0xff0000) / 0x10000);
            tbufSegaPCMS[10] = (byte)(((tbufSegaPCMS.Length - 15 + 8) & 0x7f000000) / 0x1000000);
            desVGM.segapcm[1].pcmData = uSegaPCMS ? tbufSegaPCMS : null;


            tbufYM2612[3] = (byte)((tbufYM2612.Length - 7) & 0xff);
            tbufYM2612[4] = (byte)(((tbufYM2612.Length - 7) & 0xff00) / 0x100);
            tbufYM2612[5] = (byte)(((tbufYM2612.Length - 7) & 0xff0000) / 0x10000);
            tbufYM2612[6] = (byte)(((tbufYM2612.Length - 7) & 0x7f000000) / 0x1000000);
            desVGM.ym2612[0].pcmData = uYM2612 ? tbufYM2612 : null;

            tbufRf5c164P[6] = (byte)((tbufRf5c164P.Length - 10) & 0xff);
            tbufRf5c164P[7] = (byte)(((tbufRf5c164P.Length - 10) & 0xff00) / 0x100);
            tbufRf5c164P[8] = (byte)(((tbufRf5c164P.Length - 10) & 0xff0000) / 0x10000);
            tbufRf5c164P[9] = (byte)(((tbufRf5c164P.Length - 10) & 0x7f000000) / 0x1000000);
            desVGM.rf5c164[0].pcmData = uRf5c164P ? tbufRf5c164P : null;

            tbufRf5c164S[6] = (byte)((tbufRf5c164S.Length - 10) & 0xff);
            tbufRf5c164S[7] = (byte)(((tbufRf5c164S.Length - 10) & 0xff00) / 0x100);
            tbufRf5c164S[8] = (byte)(((tbufRf5c164S.Length - 10) & 0xff0000) / 0x10000);
            tbufRf5c164S[9] = (byte)(((tbufRf5c164S.Length - 10) & 0x7f000000) / 0x1000000);
            tbufRf5c164S[9] |= 0x80;
            desVGM.rf5c164[1].pcmData = uRf5c164S ? tbufRf5c164S : null;

            tbufHuC6280P[3] = (byte)((tbufHuC6280P.Length - 7) & 0xff);
            tbufHuC6280P[4] = (byte)(((tbufHuC6280P.Length - 7) & 0xff00) / 0x100);
            tbufHuC6280P[5] = (byte)(((tbufHuC6280P.Length - 7) & 0xff0000) / 0x10000);
            tbufHuC6280P[6] = (byte)(((tbufHuC6280P.Length - 7) & 0x7f000000) / 0x1000000);
            desVGM.huc6280[0].pcmData = uHuC6280P ? tbufHuC6280P : null;

            tbufHuC6280S[3] = (byte)((tbufHuC6280S.Length - 7) & 0xff);
            tbufHuC6280S[4] = (byte)(((tbufHuC6280S.Length - 7) & 0xff00) / 0x100);
            tbufHuC6280S[5] = (byte)(((tbufHuC6280S.Length - 7) & 0xff0000) / 0x10000);
            tbufHuC6280S[6] = (byte)(((tbufHuC6280S.Length - 7) & 0x7f000000) / 0x1000000);
            desVGM.huc6280[1].pcmData = uHuC6280S ? tbufHuC6280S : null;

            desVGM.ym2612x[0].pcmData = uYM2612X ? tbufYM2612X : null;

            desVGM.instPCM = newDic;
        }

        private static void storePcmRF5C164(ref byte[] tbufRf5c164P, ref byte[] tbufRf5c164S, ref long pRf5c164P, ref long pRf5c164S, ref bool uRf5c164P, ref bool uRf5c164S, Dictionary<int, clsPcm> newDic, KeyValuePair<int, clsPcm> v, ref byte[] buf, ref long size)
        {
            byte[] newBuf = new byte[size + 1];
            Array.Copy(buf, newBuf, size);
            newBuf[size] = 0xff;
            buf = newBuf;
            long tSize = size;
            size = buf.Length;

            //Padding
            if (size % 0x100 != 0)
            {
                newBuf = pcmPadding(ref buf, ref size, 0x80, 0x100);
            }

            newDic.Add(
                v.Key
                , new clsPcm(
                    v.Value.num,v.Value.seqNum, v.Value.chip
                    , v.Value.isSecondary
                    , v.Value.fileName
                    , v.Value.freq
                    , v.Value.vol
                    , v.Value.isSecondary ? pRf5c164S : pRf5c164P
                    , (v.Value.isSecondary ? pRf5c164S : pRf5c164P) + size
                    , size
                    , (v.Value.isSecondary ? pRf5c164S : pRf5c164P) + (v.Value.loopAdr == -1 ? tSize : v.Value.loopAdr))
                );

            if (v.Value.isSecondary) pRf5c164S += size;
            else pRf5c164P += size;

            for (int i = 0; i < tSize; i++)
            {
                if (buf[i] != 0xff)
                {
                    if (buf[i] >= 0x80)
                    {
                        buf[i] = buf[i];
                    }
                    else
                    {
                        buf[i] = (byte)(0x80 - buf[i]);
                    }
                }

                if (buf[i] == 0xff)
                {
                    buf[i] = 0xfe;
                }

            }
            if (!v.Value.isSecondary)
            {
                newBuf = new byte[tbufRf5c164P.Length + buf.Length];
                Array.Copy(tbufRf5c164P, newBuf, tbufRf5c164P.Length);
                Array.Copy(buf, 0, newBuf, tbufRf5c164P.Length, buf.Length);

                tbufRf5c164P = newBuf;
                uRf5c164P = true;
            }
            else
            {
                newBuf = new byte[tbufRf5c164S.Length + buf.Length];
                Array.Copy(tbufRf5c164S, newBuf, tbufRf5c164S.Length);
                Array.Copy(buf, 0, newBuf, tbufRf5c164S.Length, buf.Length);

                tbufRf5c164S = newBuf;
                uRf5c164S = true;
            }
        }

        private static bool storePcmYM2612(ref byte[] tbufYM2612, ref long pYM2612, Dictionary<int, clsPcm> newDic, KeyValuePair<int, clsPcm> v, byte[] buf, long size)
        {
            bool uYM2612;

            if (newDic.ContainsKey(v.Key))
            {
                newDic.Remove(v.Key);
            }
            newDic.Add(v.Key, new clsPcm(v.Value.num, v.Value.seqNum, v.Value.chip, false, v.Value.fileName, v.Value.freq, v.Value.vol, pYM2612, pYM2612 + size, size, -1));
            pYM2612 += size;

            byte[] newBuf = new byte[tbufYM2612.Length + buf.Length];
            Array.Copy(tbufYM2612, newBuf, tbufYM2612.Length);
            Array.Copy(buf, 0, newBuf, tbufYM2612.Length, buf.Length);

            tbufYM2612 = newBuf;
            uYM2612 = true;
            return uYM2612;
        }

        private static bool storePcmYM2612X(ref byte[] tbufYM2612X, ref long pYM2612X, Dictionary<int, clsPcm> newDic, KeyValuePair<int, clsPcm> v, byte[] buf, long size)
        {
            bool uYM2612X;
            byte[] newBuf;

            for (int i = 0; i < size; i++)
            {
                buf[i] -= 0x80;//符号化
            }

            //Padding
            if (size % 0x100 != 0)
            {
                newBuf = pcmPadding(ref buf, ref size, 0x00, 0x100);
                buf = newBuf;
            }

            if (newDic.ContainsKey(v.Key))
            {
                newDic.Remove(v.Key);
            }
            newDic.Add(v.Key, new clsPcm(v.Value.num, v.Value.seqNum, v.Value.chip, false, v.Value.fileName, v.Value.freq, v.Value.vol, pYM2612X, pYM2612X + size, size, -1));
            pYM2612X += size;

            newBuf = new byte[tbufYM2612X.Length + buf.Length];
            Array.Copy(tbufYM2612X, newBuf, tbufYM2612X.Length);
            Array.Copy(buf, 0, newBuf, tbufYM2612X.Length, buf.Length);

            tbufYM2612X = newBuf;
            uYM2612X = true;
            return uYM2612X;
        }

        private static bool storePcmHuC6280(ref byte[] tbufHuC6280, ref long pHuC6280, Dictionary<int, clsPcm> newDic, KeyValuePair<int, clsPcm> v, byte[] buf, long size)
        {
            bool uHuC6280;

            for (int i = 0; i < size; i++)
            {
                buf[i] >>= 3;//5bit化
            }

            if (newDic.ContainsKey(v.Key))
            {
                newDic.Remove(v.Key);
            }
            newDic.Add(v.Key, new clsPcm(v.Value.num, v.Value.seqNum, v.Value.chip, false, v.Value.fileName, v.Value.freq, v.Value.vol, pHuC6280, pHuC6280 + size, size, -1));
            pHuC6280 += size;

            byte[] newBuf = new byte[tbufHuC6280.Length + buf.Length];
            Array.Copy(tbufHuC6280, newBuf, tbufHuC6280.Length);
            Array.Copy(buf, 0, newBuf, tbufHuC6280.Length, buf.Length);

            tbufHuC6280 = newBuf;
            uHuC6280 = true;
            return uHuC6280;
        }

        private static void storePcmSegaPCM(ref byte[] tbufSegaPCMP, ref byte[] tbufSegaPCMS, ref long pSegaPCMP, ref long pSegaPCMS, ref bool uSegaPCMP, ref bool uSegaPCMS, Dictionary<int, clsPcm> newDic, KeyValuePair<int, clsPcm> v, ref byte[] buf, ref long size)
        {
            byte[] newBuf = new byte[size];
            Array.Copy(buf, newBuf, size);
            buf = newBuf;
            size = buf.Length;

            //Padding
            if (size % 0x100 != 0)
            {
                newBuf = pcmPadding(ref buf, ref size, 0x80, 0x100);
            }

            //65536 バイトを超える場合はそれ以降をカット
            if (size > 0x10000)
            {
                List<byte> n = newBuf.ToList();
                n.RemoveRange(0x10000, (int)(size - 0x10000));
                newBuf = n.ToArray();
                size = 0x10000;
            }

            //パディング(空きが足りない場合はバンクをひとつ進める(0x10000)為、空きを全て埋める)
            if (!v.Value.isSecondary)
            {
                int fs = (tbufSegaPCMP.Length - 15) % 0x10000;
                if (size > 0x10000 - fs)
                {
                    List<byte> n = tbufSegaPCMP.ToList();
                    for (int i = 0; i < 0x10000 - fs; i++) n.Add(0x80);
                    tbufSegaPCMP = n.ToArray();
                    pSegaPCMP += 0x10000 - fs;
                }
            }
            else
            {
                int fs = (tbufSegaPCMS.Length - 15) % 0x10000;
                if (size > 0x10000 - fs)
                {
                    List<byte> n = tbufSegaPCMS.ToList();
                    for (int i = 0; i < 0x10000 - fs; i++) n.Add(0x80);
                    tbufSegaPCMS = n.ToArray();
                    pSegaPCMS += 0x10000 - fs;
                }
            }

            newDic.Add(
                v.Key
                , new clsPcm(
                    v.Value.num
                    , v.Value.seqNum, v.Value.chip
                    , v.Value.isSecondary
                    , v.Value.fileName
                    , v.Value.freq
                    , v.Value.vol
                    , v.Value.isSecondary ? pSegaPCMS : pSegaPCMP
                    , (v.Value.isSecondary ? pSegaPCMS : pSegaPCMP) + size
                    , size
                    , 0)
                );

            if (!v.Value.isSecondary)
            {
                pSegaPCMP += size;
                newBuf = new byte[tbufSegaPCMP.Length + buf.Length];
                Array.Copy(tbufSegaPCMP, newBuf, tbufSegaPCMP.Length);
                Array.Copy(buf, 0, newBuf, tbufSegaPCMP.Length, buf.Length);

                tbufSegaPCMP = newBuf;
                uSegaPCMP = true;
            }
            else
            {
                pSegaPCMS += size;
                newBuf = new byte[tbufSegaPCMS.Length + buf.Length];
                Array.Copy(tbufSegaPCMS, newBuf, tbufSegaPCMS.Length);
                Array.Copy(buf, 0, newBuf, tbufSegaPCMS.Length, buf.Length);

                tbufSegaPCMS = newBuf;
                uSegaPCMS = true;
            }
        }

        private static long storeAdpcmYM2610B(ref byte[] tbufYm2610AdpcmAP, ref byte[] tbufYm2610AdpcmAS, ref long pYm2610AdpcmAP, ref long pYm2610AdpcmAS, ref bool uYm2610AdpcmAP, ref bool uYm2610AdpcmAS, ref byte[] tbufYm2610AdpcmBP, ref byte[] tbufYm2610AdpcmBS, ref long pYm2610AdpcmBP, ref long pYm2610AdpcmBS, ref bool uYm2610AdpcmBP, ref bool uYm2610AdpcmBS, Dictionary<int, clsPcm> newDic, KeyValuePair<int, clsPcm> v, bool is16bit, ref byte[] buf)
        {
            long size;
            if (v.Value.loopAdr == 0)
            {
                EncAdpcmA ea = new EncAdpcmA();
                buf = ea.YM_ADPCM_A_Encode(buf, is16bit);
                size = buf.Length;

                byte[] newBuf;

                newBuf = new byte[size];
                Array.Copy(buf, newBuf, size);
                buf = newBuf;
                long tSize = size;
                size = buf.Length;

                newDic.Add(
                    v.Key
                    , new clsPcm(
                        v.Value.num
                        , v.Value.seqNum, v.Value.chip
                        , v.Value.isSecondary
                        , v.Value.fileName
                        , v.Value.freq
                        , v.Value.vol
                        , v.Value.isSecondary ? pYm2610AdpcmAS : pYm2610AdpcmAP
                        , (v.Value.isSecondary ? pYm2610AdpcmAS : pYm2610AdpcmAP) + size
                        , size
                        , 0)
                    );

                if (!v.Value.isSecondary)
                {
                    pYm2610AdpcmAP += size;
                    newBuf = new byte[tbufYm2610AdpcmAP.Length + buf.Length];
                    Array.Copy(tbufYm2610AdpcmAP, newBuf, tbufYm2610AdpcmAP.Length);
                    Array.Copy(buf, 0, newBuf, tbufYm2610AdpcmAP.Length, buf.Length);

                    tbufYm2610AdpcmAP = newBuf;
                    uYm2610AdpcmAP = true;
                }
                else
                {
                    pYm2610AdpcmAS += size;
                    newBuf = new byte[tbufYm2610AdpcmAS.Length + buf.Length];
                    Array.Copy(tbufYm2610AdpcmAS, newBuf, tbufYm2610AdpcmAS.Length);
                    Array.Copy(buf, 0, newBuf, tbufYm2610AdpcmAS.Length, buf.Length);

                    tbufYm2610AdpcmAS = newBuf;
                    uYm2610AdpcmAS = true;
                }
            }
            else
            {
                EncAdpcmA ea = new EncAdpcmA();
                buf = ea.YM_ADPCM_B_Encode(buf, is16bit, true);
                size = buf.Length;

                byte[] newBuf;

                newBuf = new byte[size];
                Array.Copy(buf, newBuf, size);
                buf = newBuf;
                long tSize = size;
                size = buf.Length;

                newDic.Add(
                    v.Key
                    , new clsPcm(
                        v.Value.num
                        , v.Value.seqNum, v.Value.chip
                        , v.Value.isSecondary
                        , v.Value.fileName
                        , v.Value.freq
                        , v.Value.vol
                        , v.Value.isSecondary ? pYm2610AdpcmBS : pYm2610AdpcmBP
                        , (v.Value.isSecondary ? pYm2610AdpcmBS : pYm2610AdpcmBP) + size
                        , size
                        , 1)
                    );

                if (!v.Value.isSecondary)
                {
                    pYm2610AdpcmBP += size;
                    newBuf = new byte[tbufYm2610AdpcmBP.Length + buf.Length];
                    Array.Copy(tbufYm2610AdpcmBP, newBuf, tbufYm2610AdpcmBP.Length);
                    Array.Copy(buf, 0, newBuf, tbufYm2610AdpcmBP.Length, buf.Length);

                    tbufYm2610AdpcmBP = newBuf;
                    uYm2610AdpcmBP = true;
                }
                else
                {
                    pYm2610AdpcmBS += size;
                    newBuf = new byte[tbufYm2610AdpcmBS.Length + buf.Length];
                    Array.Copy(tbufYm2610AdpcmBS, newBuf, tbufYm2610AdpcmBS.Length);
                    Array.Copy(buf, 0, newBuf, tbufYm2610AdpcmBS.Length, buf.Length);

                    tbufYm2610AdpcmBS = newBuf;
                    uYm2610AdpcmBS = true;
                }
            }

            return size;
        }

        private static long storeAdpcmYM2608(ref byte[] tbufYm2608AdpcmP, ref byte[] tbufYm2608AdpcmS, ref long pYm2608AdpcmP, ref long pYm2608AdpcmS, ref bool uYm2608AdpcmP, ref bool uYm2608AdpcmS, Dictionary<int, clsPcm> newDic, KeyValuePair<int, clsPcm> v, bool is16bit, ref byte[] buf)
        {
            long size;
            EncAdpcmA ea = new EncAdpcmA();
            buf = ea.YM_ADPCM_B_Encode(buf, is16bit, false);
            size = buf.Length;

            byte[] newBuf;

            newBuf = new byte[size];
            Array.Copy(buf, newBuf, size);
            buf = newBuf;
            long tSize = size;
            size = buf.Length;

            newDic.Add(
                v.Key
                , new clsPcm(
                    v.Value.num
                    , v.Value.seqNum, v.Value.chip
                    , v.Value.isSecondary
                    , v.Value.fileName
                    , v.Value.freq
                    , v.Value.vol
                    , v.Value.isSecondary ? pYm2608AdpcmS : pYm2608AdpcmP
                    , (v.Value.isSecondary ? pYm2608AdpcmS : pYm2608AdpcmP) + size
                    , size
                    , 1)
                );

            if (!v.Value.isSecondary)
            {
                pYm2608AdpcmP += size;
                newBuf = new byte[tbufYm2608AdpcmP.Length + buf.Length];
                Array.Copy(tbufYm2608AdpcmP, newBuf, tbufYm2608AdpcmP.Length);
                Array.Copy(buf, 0, newBuf, tbufYm2608AdpcmP.Length, buf.Length);

                tbufYm2608AdpcmP = newBuf;
                uYm2608AdpcmP = true;
            }
            else
            {
                pYm2608AdpcmS += size;
                newBuf = new byte[tbufYm2608AdpcmS.Length + buf.Length];
                Array.Copy(tbufYm2608AdpcmS, newBuf, tbufYm2608AdpcmS.Length);
                Array.Copy(buf, 0, newBuf, tbufYm2608AdpcmS.Length, buf.Length);

                tbufYm2608AdpcmS = newBuf;
                uYm2608AdpcmS = true;
            }

            return size;
        }

        private static byte[] pcmPadding(ref byte[] buf, ref long size,byte paddingData,int paddingSize)
        {
            byte[] newBuf = new byte[size + (paddingSize - (size % paddingSize))];
            for (int i = (int)size; i < newBuf.Length; i++) newBuf[i] = paddingData;
            Array.Copy(buf, newBuf, size);
            buf = newBuf;
            size = buf.Length;
            return newBuf;
        }

        private byte[] getPCMDataFromFile(string path , clsPcm instPCM,out bool is16bit)
        {
            string fnPcm = Path.Combine(path, instPCM.fileName);
            is16bit = false;

            if (!File.Exists(fnPcm))
            {
                msgBox.setErrMsg(string.Format("PCMファイルの読み込みに失敗しました。(filename:{0})", instPCM.fileName));
                return null;
            }

            // ファイルの読み込み
            byte[] buf = File.ReadAllBytes(fnPcm);

            if (buf.Length < 4)
            {
                msgBox.setErrMsg("PCMファイル：不正なRIFFヘッダです。");
                return null;
            }
            if(buf[0] != 'R' || buf[1] != 'I' || buf[2] != 'F' || buf[3] != 'F')
            {
                msgBox.setErrMsg("PCMファイル：不正なRIFFヘッダです。");
                return null;
            }

            // サイズ取得
            int fSize = buf[0x4] + buf[0x5] * 0x100 + buf[0x6] * 0x10000 + buf[0x7] * 0x1000000;

            if (buf[0x8] != 'W' || buf[0x9] != 'A' || buf[0xa] != 'V' || buf[0xb] != 'E')
            {
                msgBox.setErrMsg("PCMファイル：不正なRIFFヘッダです。");
                return null;
            }

            try
            {
                int p = 12;
                byte[] des = null;

                while (p < fSize + 8)
                {
                    if (buf[p + 0] == 'f' && buf[p + 1] == 'm' && buf[p + 2] == 't' && buf[p + 3] == ' ')
                    {
                        p += 4;
                        int size = buf[p + 0] + buf[p + 1] * 0x100 + buf[p + 2] * 0x10000 + buf[p + 3] * 0x1000000;
                        p += 4;
                        int format = buf[p + 0] + buf[p + 1] * 0x100;
                        if (format != 1)
                        {
                            msgBox.setErrMsg(string.Format("PCMファイル：無効なフォーマットです。({0})",format));
                            return null;
                        }

                        int channels = buf[p + 2] + buf[p + 3] * 0x100;
                        if (channels != 1)
                        {
                            msgBox.setErrMsg(string.Format("PCMファイル：仕様(mono)とは異なるチャネル数です。({0})", channels));
                            return null;
                        }

                        int samplerate = buf[p + 4] + buf[p + 5] * 0x100 + buf[p + 6] * 0x10000 + buf[p + 7] * 0x1000000;
                        if (samplerate != 8000 && samplerate != 16000 && samplerate != 18500 && samplerate != 14000)
                        {
                            msgBox.setWrnMsg(string.Format("PCMファイル：仕様(8KHz/14KHz/16KHz/18.5KHz)とは異なるサンプリングレートです。({0})", samplerate));
                            //return null;
                        }

                        int bytepersec = buf[p + 8] + buf[p + 9] * 0x100 + buf[p + 10] * 0x10000 + buf[p + 11] * 0x1000000;
                        if (bytepersec != 8000)
                        {
                        //    msgBox.setWrnMsg(string.Format("PCMファイル：仕様とは異なる平均データ割合です。({0})", bytepersec));
                        //    return null;
                        }

                        int bitswidth = buf[p + 14] + buf[p + 15] * 0x100;
                        if (bitswidth != 8 && bitswidth != 16)
                        {
                            msgBox.setErrMsg(string.Format("PCMファイル：仕様(8bit/16bit)とは異なる1サンプルあたりのビット数です。({0})", bitswidth));
                            return null;
                        }

                        is16bit = bitswidth == 16;

                        int blockalign = buf[p + 12] + buf[p + 13] * 0x100;
                        if (blockalign != (is16bit ? 2 : 1))
                        {
                            msgBox.setErrMsg(string.Format("PCMファイル：無効なデータのブロックサイズです。({0})", blockalign));
                            return null;
                        }


                        p += size;
                    }
                    else if (buf[p + 0] == 'd' && buf[p + 1] == 'a' && buf[p + 2] == 't' && buf[p + 3] == 'a')
                    {
                        p += 4;
                        int size = buf[p + 0] + buf[p + 1] * 0x100 + buf[p + 2] * 0x10000 + buf[p + 3] * 0x1000000;
                        p += 4;

                        des = new byte[size];
                        Array.Copy(buf, p, des, 0x00, size);
                        p += size;
                    }
                    else
                    {
                        p += 4;
                        int size = buf[p + 0] + buf[p + 1] * 0x100 + buf[p + 2] * 0x10000 + buf[p + 3] * 0x1000000;
                        p += 4;

                        p += size;
                    }
                }

                // volumeの加工
                if (is16bit)
                {
                    for (int i = 0; i < des.Length; i += 2)
                    {
                        //16bitのwavファイルはsignedのデータのためそのままボリューム変更可能
                        int b = (int)((short)(des[i] | (des[i + 1] << 8)) * instPCM.vol * 0.01);
                        b = (b > 0xffff) ? 0xffff : b;
                        des[i] = (byte)(b & 0xff);
                        des[i + 1] = (byte)((b & 0xff00) >> 8);
                    }
                }
                else
                {
                    for (int i = 0; i < des.Length; i++)
                    {
                        //8bitのwavファイルはunsignedのデータのためsignedのデータに変更してからボリューム変更する
                        int d = des[i];
                        //signed化
                        d -= 0x80;
                        d = (int)(d * instPCM.vol * 0.01);
                        //clip
                        d = (d > 127) ? 127 : d;
                        d = (d < -128) ? -128 : d;
                        //unsigned化
                        d += 0x80;

                        des[i] = (byte)d;
                    }
                }

                return des;
            }
            catch
            {
                msgBox.setErrMsg("PCMファイル：不正或いは未知のチャンクを持つファイルです。");
                return null;
            }
        }

    }
}
