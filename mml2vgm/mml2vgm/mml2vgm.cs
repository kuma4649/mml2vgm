using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace mml2vgm
{
    /// <summary>
    /// コンパイラ
    /// </summary>
    public class mml2vgm
    {

        private string srcFn;
        private string desFn;

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

            clsVgm desVGM = null;

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

                // 解析
                int ret = desVGM.analyze(srcBuf);

                if (ret != 0)
                {
                    msgBox.setErrMsg(string.Format("想定外のエラー　ソース解析に失敗?(analyze)(line:{0})", desVGM.lineNumber));
                    return -1;
                }

                // PCM定義あり?
                if (desVGM.instPCM.Count > 0)
                {
                    //PCMデータをXPMCKに生成してもらい、そのデータを取得する
                    byte[] tbuf = new byte[7] { 0x67, 0x66, 0x00, 0x00, 0x00, 0x00, 0x00 };
                    Dictionary<int, Tuple<string, int, int, long, long>> newDic = new Dictionary<int, Tuple<string, int, int, long, long>>();
                    long p = 0L;
                    foreach (KeyValuePair<int, Tuple<string, int, int, long, long>> v in desVGM.instPCM)
                    {
                        ////XPMCK向けファイルの生成
                        //makeXPMCKFile(v.Value);
                        ////XPMCK起動
                        //execXPMCK();
                        ////コンパイルに成功したらPCMデータをゲット
                        //long size = 0L;
                        //byte[] buf = getVGMPCMData(ref size);

                        byte[] buf = getPCMData(path , v.Value);
                        if (buf == null)
                        {
                            msgBox.setErrMsg(string.Format("PCMファイルの読み込みに失敗しました。(filename:{0})", v.Value.Item1));
                            continue;
                        }

                        long size = buf.Length;

                        newDic.Add(v.Key, new Tuple<string, int, int, long, long>(v.Value.Item1, v.Value.Item2, v.Value.Item3, p, size));
                        p += size;


                        byte[] newBuf = new byte[tbuf.Length + buf.Length];
                        Array.Copy(tbuf, newBuf, tbuf.Length);
                        Array.Copy(buf, 0, newBuf, tbuf.Length, buf.Length);

                        tbuf = newBuf;
                    }
                    tbuf[3] = (byte)((tbuf.Length - 7) & 0xff);
                    tbuf[4] = (byte)(((tbuf.Length - 7) & 0xff00) / 0x100);
                    tbuf[5] = (byte)(((tbuf.Length - 7) & 0xff0000) / 0x10000);
                    tbuf[6] = (byte)(((tbuf.Length - 7) & 0xff000000) / 0x1000000);
                    desVGM.pcmData = tbuf;
                    desVGM.instPCM = newDic;
                }

                // 解析した情報をもとにVGMファイル作成
                byte[] desBuf = desVGM.getByteData();

                if (desBuf == null)
                {
                    msgBox.setErrMsg(string.Format("想定外のエラー　ソース解析に失敗?(getByteData)(line:{0})", desVGM.lineNumber));
                    return -1;
                }

                // ファイル出力
                File.WriteAllBytes(desFn, desBuf);

                // 終了
                return 0;
            }
            catch (Exception ex)
            {
                msgBox.setErrMsg(string.Format("想定外のエラー　line:{0} \r\nメッセージ:\r\n{1}\r\nスタックトレース:\r\n{2}\r\n", desVGM.lineNumber, ex.Message, ex.StackTrace));
                return -1;
            }
        }

        private byte[] getPCMData(string path , Tuple<string, int, int, long, long> instPCM)
        {
            string fnPcm = Path.Combine(path, instPCM.Item1);

            if (!File.Exists(fnPcm))
            {
                msgBox.setErrMsg(string.Format("PCMファイルの読み込みに失敗しました。(filename:{0})", instPCM.Item1));
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
                            msgBox.setErrMsg(string.Format("PCMファイル：無効なチャネル数です。({0})", channels));
                            return null;
                        }

                        int samplerate = buf[p + 4] + buf[p + 5] * 0x100 + buf[p + 6] * 0x10000 + buf[p + 7] * 0x1000000;
                        if (samplerate != 8000)
                        {
                            msgBox.setErrMsg(string.Format("PCMファイル：無効なサンプリングレートです。({0})", samplerate));
                            return null;
                        }

                        int bytepersec = buf[p + 8] + buf[p + 9] * 0x100 + buf[p + 10] * 0x10000 + buf[p + 11] * 0x1000000;
                        if (bytepersec != 8000)
                        {
                            msgBox.setErrMsg(string.Format("PCMファイル：無効な平均データ割合です。({0})", bytepersec));
                            return null;
                        }

                        int blockalign = buf[p + 12] + buf[p + 13] * 0x100;
                        if (blockalign != 1)
                        {
                            msgBox.setErrMsg(string.Format("PCMファイル：無効なデータのブロックサイズです。({0})", blockalign));
                            return null;
                        }

                        int bitswidth = buf[p + 14] + buf[p + 15] * 0x100;
                        if (bitswidth != 8)
                        {
                            msgBox.setErrMsg(string.Format("PCMファイル：無効な1サンプルあたりのビット数です。({0})", bitswidth));
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

                for (int i = 0; i < des.Length; i++)
                {
                    double b = (double)des[i] * (double)instPCM.Item3 * 0.01;
                    b = (b > 255) ? 255.0 : b;
                    des[i] = (byte)b;
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
