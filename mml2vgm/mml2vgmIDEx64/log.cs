using mml2vgmIDEx64.Properties;
using System;
using System.IO;
using System.Text;

namespace mml2vgmIDEx64
{
    public static class log
    {
#if DEBUG
        public static bool debug = true;
#else
        public static bool debug = false;
#endif
        public static string path = "";
        public static bool consoleEchoBack = false;
        private static Encoding sjisEnc = null;// Encoding.GetEncoding("Shift_JIS");
        public static Action<string> dispMsg;


        public static void ForcedWrite(string msg, params object[] prm)
        {
            if (sjisEnc == null)
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                sjisEnc = Encoding.GetEncoding("Shift_JIS");
            }
            try
            {
                if (path == "")
                {
                    string fullPath = Common.settingFilePath;
                    path = Path.Combine(fullPath, Resources.cntLogFilename);
                    if (File.Exists(path)) File.Delete(path);
                }
                string timefmt = DateTime.Now.ToString(Resources.cntTimeFormat);

#if DEBUG
                dispMsg?.Invoke(string.Format(msg, prm));
#else
                using (StreamWriter writer = new StreamWriter(path, true, sjisEnc))
                {
                    writer.WriteLine(timefmt + msg);
                    if (consoleEchoBack) Console.WriteLine(timefmt + msg);
                }
#endif
            }
            catch
            {
            }
        }

        public static void ForcedWrite(Exception e)
        {
            if (sjisEnc == null)
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                sjisEnc = Encoding.GetEncoding("Shift_JIS");
            }
            try
            {
                if (path == "")
                {
                    string fullPath = Common.settingFilePath;
                    path = Path.Combine(fullPath, Resources.cntLogFilename);
                    if (File.Exists(path)) File.Delete(path);
                }
                string timefmt = DateTime.Now.ToString(Resources.cntTimeFormat);

#if DEBUG
                string msg = string.Format(Resources.cntExceptionFormat, e.GetType().Name, e.Message, e.Source, e.StackTrace);
                Exception ie = e;
                while (ie.InnerException != null)
                {
                    ie = ie.InnerException;
                    msg += string.Format(Resources.cntInnerExceptionFormat, ie.GetType().Name, ie.Message, ie.Source, ie.StackTrace);
                }
                dispMsg?.Invoke(timefmt + msg);
#else
                using (StreamWriter writer = new StreamWriter(path, true, sjisEnc))
                {
                    string msg = string.Format(Resources.cntExceptionFormat, e.GetType().Name, e.Message, e.Source, e.StackTrace);
                    Exception ie = e;
                    while (ie.InnerException != null)
                    {
                        ie = ie.InnerException;
                        msg += string.Format(Resources.cntInnerExceptionFormat, ie.GetType().Name, ie.Message, ie.Source, ie.StackTrace);
                    }

                    writer.WriteLine(timefmt + msg);
                    if (consoleEchoBack) Console.WriteLine(timefmt + msg);
                }
#endif
            }
            catch
            {
            }
        }

        public static void Write(string msg)
        {
            if (!debug) return;

            if (sjisEnc == null)
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                sjisEnc = Encoding.GetEncoding("Shift_JIS");
            }
            try
            {
                if (path == "")
                {
                    string fullPath = Common.settingFilePath;
                    path = Path.Combine(fullPath, Resources.cntLogFilename);
                    if (File.Exists(path)) File.Delete(path);
                }
                string timefmt = DateTime.Now.ToString(Resources.cntTimeFormat);

#if DEBUG
                dispMsg?.Invoke(timefmt + msg);
#else
                using (StreamWriter writer = new StreamWriter(path, true, sjisEnc))
                {
                    writer.WriteLine(timefmt + msg);
                    if (consoleEchoBack) Console.WriteLine(timefmt + msg);
                }
#endif
            }
            catch
            {
            }
        }

        public static int Range(int n, int min, int max)
        {
            return (n > max) ? max : (n < min ? min : n);
        }

    }
}
