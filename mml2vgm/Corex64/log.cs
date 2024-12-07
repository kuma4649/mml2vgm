using System;
using System.IO;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Corex64
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

                    Encoding sjisEnc = Encoding.UTF8;// Encoding.GetEncoding("Shift_JIS");
                    lock (locker)
                    {
                        //using (StreamWriter writer = new StreamWriter(path, true, sjisEnc))
                        {
                            writer.WriteLine(timefmt + msg);
                            writer.Flush();
                        }
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

                Encoding sjisEnc = Encoding.UTF8;// Encoding.GetEncoding("Shift_JIS");
                lock (locker)
                {
                    //using (StreamWriter writer = new StreamWriter(path, true, sjisEnc))
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
                        //System.Console.WriteLine(msg);
                    }
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

                Encoding sjisEnc = Encoding.UTF8;// Encoding.GetEncoding("Shift_JIS");
                lock (locker)
                {
                    writer.WriteLine(timefmt + msg);
                    writer.Flush();
                }
            }
            catch
            {
            }
        }

        public static void Open()
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

                Encoding sjisEnc = Encoding.UTF8;// Encoding.GetEncoding("Shift_JIS");
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
                writer.Close();
                writer = null;
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
