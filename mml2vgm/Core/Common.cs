using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public static class Common
    {
        public static int CheckRange(int n, int min, int max)
        {
            int r = n;

            if (n < min)
            {
                r = min;
            }
            if (n > max)
            {
                r = max;
            }

            return r;
        }

        public static byte[] GetPCMDataFromFile(string path, clsPcm instPCM, out bool is16bit)
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
            if (buf[0] != 'R' || buf[1] != 'I' || buf[2] != 'F' || buf[3] != 'F')
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
                            msgBox.setErrMsg(string.Format("PCMファイル：無効なフォーマットです。({0})", format));
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

        public static void SetUInt32bit31(byte[] buf, int ptr, UInt32 value,bool sw=false)
        {
            buf[ptr + 0] = (byte)(value & 0xff);
            buf[ptr + 1] = (byte)((value & 0xff00) >> 8);
            buf[ptr + 2] = (byte)((value & 0xff0000) >> 16);
            buf[ptr + 3] = (byte)((value & 0x7f000000) >> 24);
            if (sw) buf[ptr + 3] |= 0x80;
        }

        public static byte[] PcmPadding(ref byte[] buf, ref long size, byte paddingData, int paddingSize)
        {
            byte[] newBuf = new byte[size + (paddingSize - (size % paddingSize))];
            for (int i = (int)size; i < newBuf.Length; i++) newBuf[i] = paddingData;
            Array.Copy(buf, newBuf, size);
            buf = newBuf;
            size = buf.Length;
            return newBuf;
        }

        public static List<string> DivParts(string parts,Dictionary<enmChipType,ClsChip[]> chips)
        {
            List<string> ret = new List<string>();
            string a = "";
            int k = 1;
            int m = 0;
            string n0 = "";

            try
            {
                for (int i = 0; i < parts.Length; i++)
                {
                    if (m == 0 && parts[i] >= 'A' && parts[i] <= 'Z')
                    {
                        a = parts[i].ToString();
                        if (i + 1 < parts.Length && parts[i + 1] >= 'a' && parts[i + 1] <= 'z')
                        {
                            a += parts[i + 1].ToString();
                            i++;
                        }
                        else
                        {
                            a += " ";
                        }

                        k = GetChMax(a, chips) > 9 ? 2 : 1;
                        n0 = "";

                    }
                    else if (m == 0 && parts[i] == ',')
                    {
                        n0 = "";
                    }
                    else if (m == 0 && parts[i] == '-')
                    {
                        m = 1;
                    }
                    else if (parts[i] >= '0' && parts[i] <= '9')
                    {
                        string n = parts.Substring(i, k);
                        if (k == 2 && i + 1 < parts.Length)
                        {
                            i++;
                        }

                        if (m == 0)
                        {
                            n0 = n;

                            if (!int.TryParse(n0, out int s)) return null;
                            string p = string.Format("{0}{1:00}", a, s);
                            ret.Add(p);
                        }
                        else
                        {
                            string n1 = n;

                            if (!int.TryParse(n0, out int s)) return null;
                            if (!int.TryParse(n1, out int e)) return null;
                            if (s >= e) return null;

                            do
                            {
                                s++;
                                string p = string.Format("{0}{1:00}", a, s);
                                if (ret.Contains(p)) return null;
                                ret.Add(p);
                            } while (s < e);

                            i++;
                            m = 0;
                            n0 = "";
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch
            {
                //パート解析に失敗 
                msgBox.setErrMsg(string.Format("不正なパート定義です。({0})", parts), "", 0);
            }

            return ret;
        }

        private static int GetChMax(string a, Dictionary<enmChipType,ClsChip[]> chips)
        {
            foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in chips)
            {
                foreach (ClsChip chip in kvp.Value)
                {
                    if (chip.Ch[0].Name.Substring(0, 2) == a)
                    {
                        return chip.ChMax;
                    }
                }
            }

            return 0;
        }

    }
}
