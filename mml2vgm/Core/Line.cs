using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class Line
    {
        public string Txt = "";
        public int Num = 0;
        public string Fn = "";

        public Line(string fn, int num, string txt)
        {
            Fn = fn;
            Num = num;
            Txt = txt;
        }
    }

}
