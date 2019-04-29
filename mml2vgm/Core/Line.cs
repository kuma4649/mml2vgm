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
        public LinePos Lp = null;

        public Line(LinePos lp, string txt)
        {
            Lp = lp;
            Txt = txt;
        }
    }

}
