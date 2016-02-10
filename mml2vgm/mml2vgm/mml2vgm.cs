using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace mml2vgm
{
    public class mml2vgm
    {

        private string srcFn;
        private string desFn;

        public mml2vgm(string srcFn,string desFn)
        {

            this.srcFn = srcFn;
            this.desFn = desFn;

        }

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
                    byte[] tbuf = new byte[7] { 0x67, 0x66, 0x00, 0x00, 0x00, 0x00, 0x00 };
                    Dictionary<int, Tuple<string, int, int, long, long>> newDic=new Dictionary<int, Tuple<string, int, int, long, long>>();
                    long p = 0L;
                    foreach (KeyValuePair<int,Tuple<string,int,int,long,long>> v in desVGM.instPCM)
                    {
                        //XPMCK向けファイルの生成
                        makeXPMCKFile(v.Value);
                        //XPMCK起動
                        execXPMCK();
                        //コンパイルに成功したらvgmをゲット
                        long size = 0L;
                        byte[] buf=getVGMPCMData(ref size);
                        newDic.Add(v.Key, new Tuple<string, int, int, long, long>(v.Value.Item1, v.Value.Item2, v.Value.Item3, p, size));
                        p += size;

                        byte[] newBuf = new byte[tbuf.Length + buf.Length];
                        Array.Copy(tbuf, newBuf, tbuf.Length);
                        Array.Copy(buf, 0, newBuf, tbuf.Length, buf.Length);

                        tbuf = newBuf;
                    }
                    tbuf[3] = (byte)((tbuf.Length-7) & 0xff);
                    tbuf[4] = (byte)(((tbuf.Length - 7) & 0xff00) / 0x100);
                    tbuf[5] = (byte)(((tbuf.Length - 7) & 0xff0000) / 0x10000);
                    tbuf[6] = (byte)(((tbuf.Length - 7) & 0xff000000) / 0x1000000);
                    desVGM.pcmData = tbuf;
                    desVGM.instPCM = newDic;
                }

                // 解析結果をもとにVGMファイル作成
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

        private void makeXPMCKFile(Tuple<string, int, int, long,long> instPCM)
        {

            using (StreamWriter sw = new StreamWriter(@".\temp.mml", false, Encoding.GetEncoding("shift_jis")))
            {

                sw.WriteLine("@XPCM{0} = {{\"{1}\" {2} {3}}}", 0, instPCM.Item1, instPCM.Item2, instPCM.Item3);
                sw.WriteLine("J o0 M2 l16 {0}", "c");

            }

        }

        private void execXPMCK()
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = @".\xpmck\xpmc.exe";
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.Arguments = @"-gen -v .\temp.mml .\temp.vgm";
            Process p = Process.Start(psi);
            p.WaitForExit();
        }

        private byte[] getVGMPCMData(ref long size)
        {
            // ファイルの読み込み
            byte[] buf = File.ReadAllBytes(@".\temp.vgm");
            File.Delete(@".\temp.vgm");
            File.Delete(@".\temp.mml");
            byte[] des;
            size = 0L;

            if (buf[0x40] != 0x67 || buf[0x41] != 0x66 || buf[0x42] != 0x00)
            {
                return null;
            }

            size = buf[0x43] + buf[0x44] * 0x100 + buf[0x45] * 0x10000 + buf[0x46] * 0x1000000;
            des = new byte[size];

            Array.Copy(buf, 0x40 + 7, des, 0x00, size);

            return des;

        }

    }
}
