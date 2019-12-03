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

        public static byte[] GetPCMDataFromFile(string path, clsPcm instPCM, out bool isRaw, out bool is16bit, out int samplerate)
        {
            return GetPCMDataFromFile(path, instPCM.fileName, instPCM.vol, out isRaw, out is16bit, out samplerate);
        }

        public static byte[] GetPCMDataFromFile(string path, string fileName, int vol, out bool isRaw, out bool is16bit, out int samplerate)
        {
            string fnPcm = Path.Combine(path, fileName).Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);

            isRaw = false;
            is16bit = false;
            samplerate = 8000;

            if (!File.Exists(fnPcm))
            {
                msgBox.setErrMsg(string.Format(msg.get("E02000"), fileName), new LinePos("-"));
                return null;
            }

            // ファイルの読み込み
            byte[] buf = File.ReadAllBytes(fnPcm);

            if (Path.GetExtension(fileName).ToUpper().Trim() != ".WAV")
            {
                isRaw = true;
                return buf;
            }

            if (buf.Length < 4)
            {
                msgBox.setErrMsg(msg.get("E02001"), new LinePos("-"));
                return null;
            }
            if (buf[0] != 'R' || buf[1] != 'I' || buf[2] != 'F' || buf[3] != 'F')
            {
                msgBox.setErrMsg(msg.get("E02002"), new LinePos("-"));
                return null;
            }

            // サイズ取得
            int fSize = buf[0x4] + buf[0x5] * 0x100 + buf[0x6] * 0x10000 + buf[0x7] * 0x1000000;

            if (buf[0x8] != 'W' || buf[0x9] != 'A' || buf[0xa] != 'V' || buf[0xb] != 'E')
            {
                msgBox.setErrMsg(msg.get("E02003"), new LinePos("-"));
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
                            msgBox.setErrMsg(string.Format(msg.get("E02004"), format), new LinePos("-"));
                            return null;
                        }

                        int channels = buf[p + 2] + buf[p + 3] * 0x100;
                        if (channels != 1)
                        {
                            msgBox.setErrMsg(string.Format(msg.get("E02005"), channels), new LinePos("-"));
                            return null;
                        }

                        samplerate = buf[p + 4] + buf[p + 5] * 0x100 + buf[p + 6] * 0x10000 + buf[p + 7] * 0x1000000;
                        if (samplerate != 8000 && samplerate != 16000 && samplerate != 18500 && samplerate != 14000)
                        {
                            msgBox.setWrnMsg(string.Format(msg.get("E02006"), samplerate), new LinePos("-"));
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
                            msgBox.setErrMsg(string.Format(msg.get("E02007"), bitswidth), new LinePos("-"));
                            return null;
                        }

                        is16bit = bitswidth == 16;

                        int blockalign = buf[p + 12] + buf[p + 13] * 0x100;
                        if (blockalign != (is16bit ? 2 : 1))
                        {
                            msgBox.setErrMsg(string.Format(msg.get("E02008"), blockalign), new LinePos("-"));
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

                        if (p > buf.Length - 4)
                        {
                            p = fSize + 8;
                            break;
                        }

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
                        int b = (int)((short)(des[i] | (des[i + 1] << 8)) * vol * 0.01);
                        b = (b > 0x7fff) ? 0x7fff : b;
                        b = (b < -0x8000) ? -0x8000 : b;
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
                        d = (int)(d * vol * 0.01);
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
                msgBox.setErrMsg(msg.get("E02009"), new LinePos("-"));
                return null;
            }
        }

        public static void SetUInt32bit31(byte[] buf, int ptr, UInt32 value, bool sw = false)
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

        public static List<string> DivParts(string parts, Dictionary<enmChipType, ClsChip[]> chips)
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
                msgBox.setErrMsg(string.Format(msg.get("E02010"), parts), new LinePos("-"));
            }

            return ret;
        }

        internal static void Add32bits(List<byte> desDat, uint v)
        {
            desDat.Add((byte)v);
            desDat.Add((byte)(v >> 8));
            desDat.Add((byte)(v >> 16));
            desDat.Add((byte)(v >> 24));
        }

        internal static void Add16bits(List<byte> desDat, uint v)
        {
            desDat.Add((byte)v);
            desDat.Add((byte)(v >> 8));
        }

        private static int GetChMax(string a, Dictionary<enmChipType, ClsChip[]> chips)
        {
            foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in chips)
            {
                foreach (ClsChip chip in kvp.Value)
                {
                    if (chip == null) continue;

                    if (chip.Ch[0].Name.Substring(0, 2) == a)
                    {
                        return chip.ChMax;
                    }
                }
            }

            return 0;
        }

        public static int ParseNumber(string s)
        {
            if (s.Trim().ToUpper().IndexOf("0x") == 0)
            {
                return Convert.ToInt32(s.Substring(2), 16);
            }
            else if (s.Trim().IndexOf("$") == 0)
            {
                return Convert.ToInt32(s.Substring(1), 16);
            }
            return int.Parse(s);
        }

        public static byte[] MakePCMDataBlock(byte dataType, clsPcmDatSeq pds, byte[] data)
        {
            List<byte> desDat = new List<byte>();

            desDat.Add(0x67); //Data block command
            desDat.Add(0x66); //compatibility command

            desDat.Add(dataType); //data type

            int length = pds.SrcLength == -1 ? data.Length : pds.SrcLength;

            if (data.Length < pds.SrcStartAdr + length)
            {
                length = data.Length - pds.SrcStartAdr;
            }
            byte[] dmy = new byte[length];
            Array.Copy(data, pds.SrcStartAdr, dmy, 0, length);

            Common.Add32bits(desDat, (uint)(length + 8) | (pds.chipNumber!=0 ? 0x8000_0000 : 0x0000_0000));//size of data, in bytes
            Common.Add32bits(desDat, (uint)(pds.DesStartAdr + length));//size of the entire ROM
            Common.Add32bits(desDat, (uint)pds.DesStartAdr);//start address of data

            desDat.AddRange(dmy);

            pds.DatStartAdr = pds.DesStartAdr;
            pds.DatEndAdr = pds.DatStartAdr + length - 1;

            return desDat.ToArray();
        }

        public static byte[] MakePCMDataBlockType2(byte dataType, clsPcmDatSeq pds, byte[] data)
        {
            List<byte> desDat = new List<byte>();

            desDat.Add(0x67); //Data block command
            desDat.Add(0x66); //compatibility command

            desDat.Add(dataType); //data type

            int length = pds.SrcLength == -1 ? data.Length : pds.SrcLength;

            if (data.Length < pds.SrcStartAdr + length)
            {
                length = data.Length - pds.SrcStartAdr;
            }
            byte[] dmy = new byte[length];
            Array.Copy(data, pds.SrcStartAdr, dmy, 0, length);

            Common.Add32bits(desDat, (uint)(length + 2) | (pds.chipNumber!=0 ? 0x8000_0000 : 0x0000_0000));//size of data, in bytes
            Common.Add16bits(desDat, (uint)pds.DesStartAdr);//start address of data

            desDat.AddRange(dmy);

            pds.DatStartAdr = pds.DesStartAdr;
            pds.DatEndAdr = pds.DatStartAdr + length - 1;

            return desDat.ToArray();
        }

        public static void SetLE32(outDatum[] desBuf, uint adr, uint val)
        {
            desBuf[adr].val = (byte)val;
            desBuf[adr + 1].val = (byte)(val >> 8);
            desBuf[adr + 2].val = (byte)(val >> 16);
            desBuf[adr + 3].val = (byte)(val >> 24);
        }

        public static void SetLE32(List<outDatum> desBuf, uint adr, uint val)
        {
            desBuf[(int)adr].val = (byte)val;
            desBuf[(int)adr + 1].val = (byte)(val >> 8);
            desBuf[(int)adr + 2].val = (byte)(val >> 16);
            desBuf[(int)adr + 3].val = (byte)(val >> 24);
        }

        public static void SetLE16(List<outDatum> desBuf, uint adr, ushort val)
        {
            desBuf[(int)adr].val = (byte)val;
            desBuf[(int)adr + 1].val = (byte)(val >> 8);
        }

        public static bool CheckDummyCommand(enmMMLType typ)
        {
            if (typ == enmMMLType.Rest
                || typ == enmMMLType.Tempo
                || typ == enmMMLType.Length
                || typ == enmMMLType.Octave
                || typ == enmMMLType.OctaveDown
                || typ == enmMMLType.OctaveUp
                || typ == enmMMLType.Instrument
                || typ == enmMMLType.Envelope
                || typ == enmMMLType.Volume
                || typ == enmMMLType.Note
                || typ == enmMMLType.Pan
                || typ == enmMMLType.LfoSwitch
                || typ == enmMMLType.Detune
                || typ == enmMMLType.KeyShift
                || typ == enmMMLType.unknown
                || typ == enmMMLType.Lyric
                || typ == enmMMLType.Lfo
                )
            {
                return true;
            }

            return false;
        }

        public static bool CheckSoXVersion(string srcpath,Action<string> Disp)
        {
            try
            {
                string path = Path.Combine(srcpath, "sox\\sox.exe");
                if (!File.Exists(path))
                {
                    return false;
                }

                //SoXの起動
                System.Diagnostics.ProcessStartInfo psi =
                    new System.Diagnostics.ProcessStartInfo();
                psi.FileName = string.Format("\"{0}\"", path);
                psi.Arguments = "--version";
                psi.CreateNoWindow = true;
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = true;
                System.Diagnostics.Process p = System.Diagnostics.Process.Start(psi);
                p.WaitForExit();
                Disp(p.StandardOutput.ReadToEnd().Replace("\r\r\n", "\n"));

            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

    }
}
