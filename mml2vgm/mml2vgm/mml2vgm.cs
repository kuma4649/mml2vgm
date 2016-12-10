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
                    byte[] tbufYM2612 = new byte[7] { 0x67, 0x66, 0x00, 0x00, 0x00, 0x00, 0x00 };
                    long pYM2612 = 0L;
                    bool uYM2612 = false;
                    byte[] tbufRf5c164 = new byte[12] { 0xb1, 0x07, 0x00, 0x67, 0x66, 0xc1, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                    long pRf5c164 = 0L;
                    bool uRf5c164 = false;
                    Dictionary<int, clsPcm> newDic = new Dictionary<int, clsPcm>();
                    foreach (KeyValuePair<int, clsPcm> v in desVGM.instPCM)
                    {

                        byte[] buf = getPCMData(path , v.Value);
                        if (buf == null)
                        {
                            msgBox.setErrMsg(string.Format("PCMファイルの読み込みに失敗しました。(filename:{0})", v.Value.fileName));
                            continue;
                        }

                        long size = buf.Length;

                        if (v.Value.chip == 0)
                        {
                            newDic.Add(v.Key, new clsPcm(v.Value.num, v.Value.chip, v.Value.fileName, v.Value.freq, v.Value.vol, pYM2612, size, -1));
                            pYM2612 += size;

                            byte[] newBuf = new byte[tbufYM2612.Length + buf.Length];
                            Array.Copy(tbufYM2612, newBuf, tbufYM2612.Length);
                            Array.Copy(buf, 0, newBuf, tbufYM2612.Length, buf.Length);

                            tbufYM2612 = newBuf;
                            uYM2612 = true;
                        }
                        else if (v.Value.chip == 2)
                        {
                            byte[] newBuf;

                            newBuf = new byte[size + 1];
                            Array.Copy(buf, newBuf, size);
                            newBuf[size] = 0xff;
                            buf = newBuf;
                            long tSize = size;
                            size = buf.Length;

                            //Padding
                            if (size % 0x100 != 0)
                            {
                                newBuf = new byte[size + (0x100 - (size % 0x100))];
                                for (int i = (int)size; i < newBuf.Length; i++) newBuf[i] = 0x80;
                                Array.Copy(buf, newBuf, size);
                                buf = newBuf;
                                size = buf.Length;
                            }

                            if (v.Value.loopAdr == -1)
                            {
                                newDic.Add(v.Key, new clsPcm(v.Value.num, v.Value.chip, v.Value.fileName, v.Value.freq, v.Value.vol, pRf5c164, size, pRf5c164 + tSize));
                            }
                            else
                            {
                                newDic.Add(v.Key, new clsPcm(v.Value.num, v.Value.chip, v.Value.fileName, v.Value.freq, v.Value.vol, pRf5c164, size, pRf5c164 + v.Value.loopAdr));
                            }
                            pRf5c164 += size;

                            for (int i = 0; i < tSize; i++)
                            {
                                if (buf[i] != 0xff)
                                {
                                    if (buf[i] >= 0x80)
                                    {
                                        //buf[i] = (byte)(0xff - buf[i]);
                                        buf[i] = buf[i];
                                    }
                                    else
                                    {
                                        //buf[i] = (byte)(0x0 + buf[i]);
                                        buf[i] = (byte)(0x80 - buf[i]);
                                    }
                                }

                                if (buf[i] == 0xff)
                                {
                                    buf[i] = 0xfe;
                                }

                            }
                            newBuf = new byte[tbufRf5c164.Length + buf.Length];
                            Array.Copy(tbufRf5c164, newBuf, tbufRf5c164.Length);
                            Array.Copy(buf, 0, newBuf, tbufRf5c164.Length, buf.Length);

                            tbufRf5c164 = newBuf;
                            uRf5c164 = true;
                        }
                    }

                    tbufYM2612[3] = (byte)((tbufYM2612.Length - 7) & 0xff);
                    tbufYM2612[4] = (byte)(((tbufYM2612.Length - 7) & 0xff00) / 0x100);
                    tbufYM2612[5] = (byte)(((tbufYM2612.Length - 7) & 0xff0000) / 0x10000);
                    tbufYM2612[6] = (byte)(((tbufYM2612.Length - 7) & 0xff000000) / 0x1000000);
                    desVGM.pcmDataYM2612 = uYM2612 ? tbufYM2612 : null;

                    tbufRf5c164[6] = (byte)((tbufRf5c164.Length - 10) & 0xff);
                    tbufRf5c164[7] = (byte)(((tbufRf5c164.Length - 10) & 0xff00) / 0x100);
                    tbufRf5c164[8] = (byte)(((tbufRf5c164.Length - 10) & 0xff0000) / 0x10000);
                    tbufRf5c164[9] = (byte)(((tbufRf5c164.Length - 10) & 0xff000000) / 0x1000000);
                    desVGM.pcmDataRf5c164 = uRf5c164 ? tbufRf5c164 : null;

                    desVGM.instPCM = newDic;
                }

                // 解析した情報をもとにVGMファイル作成
                byte[] desBuf = desVGM.getByteData();

                if (desBuf == null)
                {
                    msgBox.setErrMsg(string.Format("想定外のエラー　ソース解析に失敗?(getByteData)(line:{0})", desVGM.lineNumber));
                    return -1;
                }

                if (Path.GetExtension(desFn).ToLower() != ".vgz")
                {
                    // ファイル出力
                    File.WriteAllBytes(desFn, desBuf);
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

        private byte[] getPCMData(string path , clsPcm instPCM)
        {
            string fnPcm = Path.Combine(path, instPCM.fileName);

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
                            msgBox.setErrMsg(string.Format("PCMファイル：無効なチャネル数です。({0})", channels));
                            return null;
                        }

                        int samplerate = buf[p + 4] + buf[p + 5] * 0x100 + buf[p + 6] * 0x10000 + buf[p + 7] * 0x1000000;
                        if (samplerate != 8000)
                        {
                            msgBox.setWrnMsg(string.Format("PCMファイル：仕様とは異なるサンプリングレートです。({0})", samplerate));
                            //return null;
                        }

                        int bytepersec = buf[p + 8] + buf[p + 9] * 0x100 + buf[p + 10] * 0x10000 + buf[p + 11] * 0x1000000;
                        if (bytepersec != 8000)
                        {
                            msgBox.setWrnMsg(string.Format("PCMファイル：仕様とは異なる平均データ割合です。({0})", bytepersec));
                            //return null;
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
                    double b = (double)des[i] * (double)instPCM.vol * 0.01;
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
