using musicDriverInterface;
using System.Collections.Generic;

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

        public static void setErrMsg(string msg, LinePos lp)
        {
            //errBox.Add(string.Format("(F : {0}  L : {1}) : {2}", System.IO.Path.GetFileName(fn), lineNumber, msg));
            if (lp != null)
            {
                errBox.Add(new msgInfo(lp.srcMMLID, lp.row, lp.col, lp.length, msg));
            }
            else
            {
                errBox.Add(new msgInfo("-", -1, -1, -1, msg));
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
                wrnBox.Add(new msgInfo(lp.srcMMLID, lp.row, lp.col, lp.length, msg));
            }
            else
            {
                wrnBox.Add(new msgInfo("-", -1, -1, -1, msg));
            }
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
        public int column = -1;
        public int length = -1;
        public string body = "";

        public msgInfo(string filename, int line, int column, int length, string body)
        {
            this.filename = filename;
            this.line = line;
            this.column = column;
            this.length = length;
            this.body = body;
        }
    }
}
