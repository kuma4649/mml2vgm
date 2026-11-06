using musicDriverInterface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Corex64
{
    public static class msgBox
    {
        private static List<msgInfo> errBox = new List<msgInfo>();
        private static List<msgInfo> wrnBox = new List<msgInfo>();
        private static HashSet<string> errDedupKeys = new HashSet<string>();

        public static void clear()
        {
            errBox = new List<msgInfo>();
            wrnBox = new List<msgInfo>();
            errDedupKeys = new HashSet<string>();
        }

        public static void setErrMsg(msgInfo msg)
        {
            if (msg == null) return;
            string key = getErrDedupKey(msg);
            if (errDedupKeys.Contains(key)) return;
            errDedupKeys.Add(key);
            errBox.Add(msg);
        }

        public static void setErrMsg(string msg, LinePos lp)
        {
            //errBox.Add(string.Format("(F : {0}  L : {1}) : {2}", System.IO.Path.GetFileName(fn), lineNumber, msg));
            if (lp != null)
            {
                setErrMsg(new msgInfo(lp.document, lp.srcMMLID, lp.row, lp.col, lp.length, msg));
            }
            else
            {
                setErrMsg(new msgInfo(null, "-", -1, -1, -1, msg));
            }
        }

        public static void setWrnMsg(msgInfo msg)
        {
            wrnBox.Add(msg);
        }

        public static void setWrnMsg(string msg, LinePos lp)
        {
            if (lp != null)
            {
                wrnBox.Add(new msgInfo(lp.document, lp.srcMMLID, lp.row, lp.col, lp.length, msg));
            }
            else
            {
                wrnBox.Add(new msgInfo(null, "-", -1, -1, -1, msg));
            }
        }

        public static msgInfo[] getErr()
        {
            return errBox.OrderBy(msg => msg.filename, StringComparer.Ordinal)
                .ThenBy(msg => msg.line).ToArray();
        }

        public static msgInfo[] getWrn()
        {
            return wrnBox.ToArray();
        }

        private static string getErrDedupKey(msgInfo msg)
        {
            string file = msg.filename ?? "-";
            string body = msg.body ?? "";
            return string.Format("{0}|{1}|{2}", file, msg.line, body);
        }
    }

    public class msgInfo
    {
        public object document = null;
        public string filename = "-";
        public int line = -1;
        public int column = -1;
        public int length = -1;
        public string body = "";

        public msgInfo(object document,string filename, int line, int column, int length, string body)
        {
            this.document = document;
            this.filename = filename;
            this.line = line;
            this.column = column;
            this.length = length;
            this.body = body;
        }
    }
}
