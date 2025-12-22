using System;
using System.IO;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Collections.Generic;

namespace Core
{
    public static class log
    {
        public static string path = "";
#if DEBUG
        public static bool debug = true;
#else
        public static bool debug = false;
#endif

        public static StreamWriter writer;
        private static object locker = new object();
        private static StringBuilder logBuffer = new StringBuilder();
        private static readonly int BUFFER_SIZE = 4096;
        private static string cachedTimeFormat = "";
        private static long cachedTimeTicks = 0L;
        private static bool pathInitialized = false;

        public static void ForcedWrite(string msg)
        {
            if (writer == null) return;

            int retry = 3;
            do
            {
                try
                {
                    if (path == "")
                    {
                        string fullPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                        fullPath = Path.Combine(fullPath, "KumaApp", AssemblyTitle);
                        if (!Directory.Exists(fullPath)) Directory.CreateDirectory(fullPath);
                        path = Path.Combine(fullPath, "log.txt");
                        if (File.Exists(path)) File.Delete(path);
                    }

                    DateTime dtNow = DateTime.Now;
                    string timefmt = dtNow.ToString("yyyy/MM/dd HH:mm:ss\t");

                    Encoding sjisEnc = Encoding.UTF8;
                    lock (locker)
                    {
                        writer.WriteLine(timefmt + msg);
                        writer.Flush();
                    }
                    retry = 0;
                }
                catch
                {
                    Thread.Sleep(10);
                    retry--;
                }
            }while (retry > 0);
        }

        public static void ForcedWrite(Exception e)
        {
            if (writer == null) return;
            try
            {
                if (path == "")
                {
                    string fullPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    fullPath = Path.Combine(fullPath, "KumaApp", AssemblyTitle);
                    if (!Directory.Exists(fullPath)) Directory.CreateDirectory(fullPath);
                    path = Path.Combine(fullPath, "log.txt");
                    if (File.Exists(path)) File.Delete(path);
                }

                DateTime dtNow = DateTime.Now;
                string timefmt = dtNow.ToString("yyyy/MM/dd HH:mm:ss\t");

                Encoding sjisEnc = Encoding.UTF8;
                lock (locker)
                {
                    string msg = string.Format("例外発生:\r\n- Type ------\r\n{0}\r\n- Message ------\r\n{1}\r\n- Source ------\r\n{2}\r\n- StackTrace ------\r\n{3}\r\n", e.GetType().Name, e.Message, e.Source, e.StackTrace);
                    Exception ie = e;
                    while (ie.InnerException != null)
                    {
                        ie = ie.InnerException;
                        msg += string.Format("内部例外:\r\n- Type ------\r\n{0}\r\n- Message ------\r\n{1}\r\n- Source ------\r\n{2}\r\n- StackTrace ------\r\n{3}\r\n", ie.GetType().Name, ie.Message, ie.Source, ie.StackTrace);
                    }

                    writer.WriteLine(timefmt + msg);
                    writer.Flush();
                }
            }
            catch
            {
            }
        }

        public static void Write(string msg)
        {
            if (!debug) return;
            if (writer == null) return;

            try
            {
                if (!pathInitialized)
                {
                    string fullPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    fullPath = Path.Combine(fullPath, "KumaApp", AssemblyTitle);
                    if (!Directory.Exists(fullPath)) Directory.CreateDirectory(fullPath);
                    path = Path.Combine(fullPath, "log.txt");
                    if (File.Exists(path)) File.Delete(path);
                    pathInitialized = true;
                }

                DateTime dtNow = DateTime.Now;
                string timefmt = GetCachedTimeFormat(dtNow);

                lock (locker)
                {
                    logBuffer.AppendLine($"{timefmt}{msg}");
                    
                    if (logBuffer.Length >= BUFFER_SIZE)
                    {
                        writer.Write(logBuffer.ToString());
                        writer.Flush();
                        logBuffer.Clear();
                    }
                }
            }
            catch
            {
            }
        }

        private static string GetCachedTimeFormat(DateTime dtNow)
        {
            long nowTicks = dtNow.Ticks;
            const long ticksPerSecond = 10000000L;
            
            // キャッシュが存在し、同じ秒内かチェック（Ticksを秒単位で比較）
            if (cachedTimeTicks > 0 && (nowTicks - cachedTimeTicks) / ticksPerSecond == 0)
            {
                return cachedTimeFormat;
            }

            cachedTimeTicks = nowTicks;
            cachedTimeFormat = dtNow.ToString("yyyy/MM/dd HH:mm:ss\t");
            return cachedTimeFormat;
        }

        public static void Open()
        {
            try
            {
                if (!pathInitialized)
                {
                    string fullPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    fullPath = Path.Combine(fullPath, "KumaApp", AssemblyTitle);
                    if (!Directory.Exists(fullPath)) Directory.CreateDirectory(fullPath);
                    path = Path.Combine(fullPath, "log.txt");
                    if (File.Exists(path)) File.Delete(path);
                    pathInitialized = true;
                }

                Encoding sjisEnc = Encoding.UTF8;
                if (writer == null)
                {
                    writer = new StreamWriter(path, true, sjisEnc);
                }
            }
            catch
            {
                writer = null;
            }
        }

        public static void Close()
        {
            if (writer != null)
            {
                lock (locker)
                {
                    if (logBuffer.Length > 0)
                    {
                        writer.Write(logBuffer.ToString());
                        logBuffer.Clear();
                    }
                    writer.Close();
                    writer = null;
                }
            }
        }

        public static void Flush()
        {
            if (writer == null) return;
            
            lock (locker)
            {
                if (logBuffer.Length > 0)
                {
                    writer.Write(logBuffer.ToString());
                    logBuffer.Clear();
                }
                writer.Flush();
            }
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
                return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }
    }
}
