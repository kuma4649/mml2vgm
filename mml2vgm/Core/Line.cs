using musicDriverInterface;

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
            LinePos lp = new LinePos(this.Lp.document, this.Lp.srcMMLID, this.Lp.row, this.Lp.col, this.Lp.length, this.Lp.part, this.Lp.chip, this.Lp.chipIndex, this.Lp.chipNumber, this.Lp.ch);
            Line l = new Line(lp, this.Txt);
            return l;
        }
    }

}
