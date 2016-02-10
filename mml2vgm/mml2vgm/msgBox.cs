using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mml2vgm
{
    public static class msgBox
    {
        private static List<string> errBox=new List<string>();
        private static List<string> wrnBox=new List<string>();

        public static void clear()
        {
            errBox = new List<string>();
            wrnBox = new List<string>();
        }

        public static void setErrMsg(string msg)
        {
            errBox.Add(msg);
        }

        public static void setErrMsg(string msg, int lineNumber)
        {
            errBox.Add(string.Format("{0} : {1}", lineNumber, msg));
        }

        public static void setWrnMsg(string msg)
        {
            wrnBox.Add(msg);
        }

        public static void setWrnMsg(string msg,int lineNumber)
        {
            wrnBox.Add(string.Format("{0} : {1}", lineNumber, msg));
        }

        public static string[] getErr()
        {
            return errBox.ToArray();
        }

        public static string[] getWrn()
        {
            return wrnBox.ToArray();
        }
    }
}
