using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public static class msgBox
    {
        private static List<msgInfo> errBox = new List<msgInfo>();
        private static List<msgInfo> wrnBox = new List<msgInfo>();

        public static void clear()
        {
            errBox = new List<msgInfo>();
            wrnBox = new List<msgInfo>();
        }

        public static void setErrMsg(msgInfo msg)
        {
            errBox.Add(msg);
        }

        public static void setErrMsg(string msg, string fn, int lineNumber)
        {
            //errBox.Add(string.Format("(F : {0}  L : {1}) : {2}", System.IO.Path.GetFileName(fn), lineNumber, msg));
            errBox.Add(new msgInfo(System.IO.Path.GetFileName(fn), lineNumber, msg));
        }

        public static void setWrnMsg(msgInfo msg)
        {
            wrnBox.Add(msg);
        }

        public static void setWrnMsg(string msg, string fn, int lineNumber)
        {
            wrnBox.Add(new msgInfo(System.IO.Path.GetFileName(fn), lineNumber, msg));
        }

        public static msgInfo[] getErr()
        {
            return errBox.ToArray();
        }

        public static msgInfo[] getWrn()
        {
            return wrnBox.ToArray();
        }
    }

    public class msgInfo
    {
        public string filename = "-";
        public int line = -1;
        public string body = "";

        public msgInfo(string filename, int line, string body)
        {
            this.filename = filename;
            this.line = line;
            this.body = body;
        }
    }
}
