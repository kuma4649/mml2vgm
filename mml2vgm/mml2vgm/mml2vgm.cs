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
                        //XPMCK向けファイルの生成
                        makeXPMCKFile(v.Value);
                        //XPMCK起動
                        execXPMCK();
                        //コンパイルに成功したらPCMデータをゲット
                        long size = 0L;
                        byte[] buf = getVGMPCMData(ref size);
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

        /// <summary>
        /// PCMデータを採取するためにXPMCK向けのファイルを生成する
        /// </summary>
        /// <param name="instPCM">PCM生成情報</param>
        private void makeXPMCKFile(Tuple<string, int, int, long, long> instPCM)
        {

            using (StreamWriter sw = new StreamWriter(Properties.Resources.fnTempMml, false, Encoding.GetEncoding("shift_jis")))
            {
                //PCMの定義
                sw.WriteLine(Properties.Resources.cntXPMCKMML1, 0, instPCM.Item1, instPCM.Item2, instPCM.Item3);
                //PCMを使用
                sw.WriteLine(Properties.Resources.cntXPMCKMML2);

            }

        }

        /// <summary>
        /// XPMCKキッカー
        /// </summary>
        private void execXPMCK()
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = Properties.Resources.fnXPMCK;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.Arguments = string.Format(Properties.Resources.argXPMCK, Properties.Resources.fnTempMml, Properties.Resources.fnTempVgm);
            Process p = Process.Start(psi);
            p.WaitForExit();
        }

        /// <summary>
        /// XPMCKが生成したVGMファイルからPCMデータを採取する
        /// </summary>
        /// <param name="size">データサイズ(byte)</param>
        /// <returns>PCMデータ</returns>
        private byte[] getVGMPCMData(ref long size)
        {
            // ファイルの読み込み
            byte[] buf = File.ReadAllBytes(Properties.Resources.fnTempVgm);

            // 読み込んだらファイルは不要になるので削除
            File.Delete(Properties.Resources.fnTempVgm);
            File.Delete(Properties.Resources.fnTempMml);

            byte[] des;
            size = 0L;

            // ヘッダーのチェック
            if (buf[0x40] != 0x67 || buf[0x41] != 0x66 || buf[0x42] != 0x00)
            {
                return null;
            }

            // サイズ取得
            size = buf[0x43] + buf[0x44] * 0x100 + buf[0x45] * 0x10000 + buf[0x46] * 0x1000000;
            // データ取得
            des = new byte[size];
            Array.Copy(buf, 0x40 + 7, des, 0x00, size);

            return des;

        }

    }
}
