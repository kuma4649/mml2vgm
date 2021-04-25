using Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace mml2vgmIDE
{
    public static class Common
    {
        public const int DEV_WaveOut = 0;
        public const int DEV_DirectSound = 1;
        public const int DEV_WasapiOut = 2;
        public const int DEV_AsioOut = 3;
        public const int DEV_SPPCM = 4;
        public const int DEV_Null = 5;

        public static Int32 SampleRate = 44100;
        public static string settingFilePath = "";
        public static string playingFilePath = "";

        public static void SetDoubleBuffered(Control control)
        {
            control.GetType().InvokeMember(
               "DoubleBuffered",
               BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
               null,
               control,
               new object[] { true });
        }

        public static string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public static string GetApplicationDataFolder(bool make = false)
        {
            string fullPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            fullPath = System.IO.Path.Combine(fullPath, "KumaApp", AssemblyTitle);
            if (!System.IO.Directory.Exists(fullPath)) System.IO.Directory.CreateDirectory(fullPath);

            return fullPath;
        }

        public static int Range(int n, int min, int max)
        {
            return (n > max) ? max : (n < min ? min : n);
        }

        public static string GetApplicationFolder()
        {
            string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            if (!string.IsNullOrEmpty(path))
            {
                path += path[path.Length - 1] == '\\' ? "" : "\\";
            }
            return path;
        }

        public static int GetYM2151Hosei(float YM2151ClockValue, float baseClock)
        {
            int ret = 0;

            float delta = (float)YM2151ClockValue / baseClock;
            float d;
            float oldD = float.MaxValue;
            for (int i = 0; i < Tables.pcmMulTbl.Length; i++)
            {
                d = Math.Abs(delta - Tables.pcmMulTbl[i]);
                ret = i;
                if (d > oldD) break;
                oldD = d;
            }
            ret -= 12;

            return ret;
        }
        public static GD3 getGD3Info(outDatum[] buf, uint adr)
        {
            GD3 GD3 = new GD3();

            GD3.TrackName = "";
            GD3.TrackNameJ = "";
            GD3.GameName = "";
            GD3.GameNameJ = "";
            GD3.SystemName = "";
            GD3.SystemNameJ = "";
            GD3.Composer = "";
            GD3.ComposerJ = "";
            GD3.Converted = "";
            GD3.Notes = "";
            GD3.VGMBy = "";
            GD3.Version = "";
            GD3.UsedChips = "";

            try
            {
                //trackName
                GD3.TrackName = Encoding.Unicode.GetString(Common.getByteArray(buf, ref adr));
                //trackNameJ
                GD3.TrackNameJ = Encoding.Unicode.GetString(Common.getByteArray(buf, ref adr));
                //gameName
                GD3.GameName = Encoding.Unicode.GetString(Common.getByteArray(buf, ref adr));
                //gameNameJ
                GD3.GameNameJ = Encoding.Unicode.GetString(Common.getByteArray(buf, ref adr));
                //systemName
                GD3.SystemName = Encoding.Unicode.GetString(Common.getByteArray(buf, ref adr));
                //systemNameJ
                GD3.SystemNameJ = Encoding.Unicode.GetString(Common.getByteArray(buf, ref adr));
                //Composer
                GD3.Composer = Encoding.Unicode.GetString(Common.getByteArray(buf, ref adr));
                //ComposerJ
                GD3.ComposerJ = Encoding.Unicode.GetString(Common.getByteArray(buf, ref adr));
                //Converted
                GD3.Converted = Encoding.Unicode.GetString(Common.getByteArray(buf, ref adr));
                //VGMBy
                GD3.VGMBy = Encoding.Unicode.GetString(Common.getByteArray(buf, ref adr));
                //Notes
                GD3.Notes = Encoding.Unicode.GetString(Common.getByteArray(buf, ref adr));
                //Lyric(独自拡張)
                byte[] bLyric = Common.getByteArray(buf, ref adr);
                if (bLyric != null)
                {
                    GD3.Lyrics = new List<Tuple<int, int, string>>();
                    int i = 0;
                    int st = 0;
                    while (i < bLyric.Length)
                    {
                        byte h = bLyric[i];
                        byte l = bLyric[i + 1];
                        if ((h == 0x5b && l == 0x00 && i != 0) || i >= bLyric.Length - 2)
                        {
                            if ((i >= bLyric.Length - 2) || (bLyric[i + 2] != 0x5b || bLyric[i + 3] != 0x00))
                            {
                                string m = Encoding.Unicode.GetString(bLyric, st, i - st + ((i >= bLyric.Length - 2) ? 2 : 0));
                                st = i;

                                int cnt = int.Parse(m.Substring(1, m.IndexOf("]") - 1));
                                m = m.Substring(m.IndexOf("]") + 1);
                                GD3.Lyrics.Add(new Tuple<int, int, string>(cnt, cnt, m));
                            }
                        }
                        i += 2;
                    }
                }
                else
                {
                    GD3.Lyrics = null;
                }

            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }

            return GD3;
        }

        public static UInt32 getBE16(byte[] buf, UInt32 adr)
        {
            if (buf == null || buf.Length - 1 < adr + 1)
            {
                throw new IndexOutOfRangeException();
            }

            UInt32 dat;
            dat = (UInt32)buf[adr] * 0x100 + (UInt32)buf[adr + 1];

            return dat;
        }

        public static UInt32 getLE16(byte[] buf, UInt32 adr)
        {
            if (buf == null || buf.Length - 1 < adr + 1)
            {
                throw new IndexOutOfRangeException();
            }

            UInt32 dat;
            dat = (UInt32)buf[adr] + (UInt32)buf[adr + 1] * 0x100;

            return dat;
        }

        public static UInt32 getLE16(outDatum[] buf, UInt32 adr)
        {
            if (buf == null || buf.Length - 1 < adr + 1)
            {
                throw new IndexOutOfRangeException();
            }

            UInt32 dat;
            dat = (UInt32)buf[adr].val + (UInt32)buf[adr + 1].val * 0x100;

            return dat;
        }

        public static UInt32 getLE24(byte[] buf, UInt32 adr)
        {
            if (buf == null || buf.Length - 1 < adr + 2)
            {
                throw new IndexOutOfRangeException();
            }

            UInt32 dat;
            dat = (UInt32)buf[adr] + (UInt32)buf[adr + 1] * 0x100 + (UInt32)buf[adr + 2] * 0x10000;

            return dat;
        }
        public static UInt32 getLE24(outDatum[] buf, UInt32 adr)
        {
            if (buf == null || buf.Length - 1 < adr + 2)
            {
                throw new IndexOutOfRangeException();
            }

            UInt32 dat;
            dat = (UInt32)buf[adr].val + (UInt32)buf[adr + 1].val * 0x100 + (UInt32)buf[adr + 2].val * 0x10000;

            return dat;
        }

        public static UInt32 getLE32(byte[] buf, UInt32 adr)
        {
            if (buf == null || buf.Length - 1 < adr + 3)
            {
                throw new IndexOutOfRangeException();
            }

            UInt32 dat;
            dat = (UInt32)buf[adr] + (UInt32)buf[adr + 1] * 0x100 + (UInt32)buf[adr + 2] * 0x10000 + (UInt32)buf[adr + 3] * 0x1000000;

            return dat;
        }
        public static UInt32 getLE32(outDatum[] buf, UInt32 adr)
        {
            if (buf == null || buf.Length - 1 < adr + 3)
            {
                throw new IndexOutOfRangeException();
            }

            UInt32 dat;
            dat = (UInt32)buf[adr].val + (UInt32)buf[adr + 1].val * 0x100 + (UInt32)buf[adr + 2].val * 0x10000 + (UInt32)buf[adr + 3].val * 0x1000000;

            return dat;
        }

        public static void SetLE24(outDatum[] desBuf, uint adr, uint val)
        {
            desBuf[adr].val = (byte)val;
            desBuf[adr + 1].val = (byte)(val >> 8);
            desBuf[adr + 2].val = (byte)(val >> 16);
        }

        public static void SetLE32(outDatum[] desBuf, uint adr, uint val)
        {
            desBuf[adr].val = (byte)val;
            desBuf[adr + 1].val = (byte)(val >> 8);
            desBuf[adr + 2].val = (byte)(val >> 16);
            desBuf[adr + 3].val = (byte)(val >> 24);
        }

        public static byte[] getByteArray(byte[] buf, ref uint adr)
        {
            if (adr >= buf.Length) return null;

            List<byte> ary = new List<byte>();
            while (buf[adr] != 0 || buf[adr + 1] != 0)
            {
                ary.Add(buf[adr]);
                adr++;
                ary.Add(buf[adr]);
                adr++;
            }
            adr += 2;

            return ary.ToArray();
        }

        public static byte[] getByteArray(outDatum[] buf, ref uint adr)
        {
            if (adr >= buf.Length) return null;

            List<byte> ary = new List<byte>();
            while (buf[adr].val != 0 || buf[adr + 1].val != 0)
            {
                ary.Add(buf[adr].val);
                adr++;
                ary.Add(buf[adr].val);
                adr++;
            }
            adr += 2;

            return ary.ToArray();
        }

        public static void SetBFColor(Control parent, Setting setting)
        {
            if (parent is Form)
            {
                parent.BackColor = Color.FromArgb(setting.ColorScheme.Log_BackColor);
                parent.ForeColor = Color.FromArgb(setting.ColorScheme.Log_ForeColor);
            }

            foreach (Control child in parent.Controls)
            {
                if (child.Controls != null && child.Controls.Count > 0)
                    SetBFColor(child, setting);

                if (child is Label || child is TextBox || child is Panel)
                {
                    child.BackColor = Color.FromArgb(setting.ColorScheme.Log_BackColor);
                    child.ForeColor = Color.FromArgb(setting.ColorScheme.Log_ForeColor);
                }

                if (child is Button)
                {
                    child.BackColor = Color.FromArgb(setting.ColorScheme.Log_BackColor);
                    child.ForeColor = Color.FromArgb(setting.ColorScheme.Log_ForeColor);
                }
            }
        }

        public static EnmMmlFileFormat GetEnmMmlFileFormat(string extension)
        {
            if (extension.ToUpper() == ".GWI") return EnmMmlFileFormat.GWI;
            if (extension.ToUpper() == ".MUC") return EnmMmlFileFormat.MUC;
            if (extension.ToUpper() == ".MML") return EnmMmlFileFormat.MML;
            if (extension.ToUpper() == ".MDL") return EnmMmlFileFormat.MDL;

            return EnmMmlFileFormat.unknown;
        }

        public static Stream GetOPNARyhthmStream(string fn)
        {
            string ffn = fn;

            string chk;

            chk = Path.Combine(playingFilePath, fn);
            if (File.Exists(chk))
                ffn = chk;
            else
            {
                chk = Path.Combine(GetApplicationFolder(), fn);
                if (File.Exists(chk)) ffn = chk;
            }

            try
            {
                if (!File.Exists(ffn)) return null;
                FileStream fs = new FileStream(ffn, FileMode.Open, FileAccess.Read, FileShare.Read);
                return fs;
            }
            catch
            {
                return null;
            }
        }
    }
}
