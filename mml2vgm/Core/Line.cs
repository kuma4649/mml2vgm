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

        public Line Copy()
        {
            LinePos lp = new LinePos(this.Lp.fullPath, this.Lp.row, this.Lp.col, this.Lp.length, this.Lp.part, this.Lp.chip, this.Lp.chipIndex, this.Lp.chipNumber, this.Lp.ch);
            Line l = new Line(lp, this.Txt);
            return l;
        }
    }

}
