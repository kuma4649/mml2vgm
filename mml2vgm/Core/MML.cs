using musicDriverInterface;
using System.Collections.Generic;

namespace Core
{
    public class MML
    {
        public Line line;
        public int column;

        public enmMMLType type;
        public List<object> args;

        public MML Copy()
        {
            MML n = new MML();
            n.line = line.Copy();
            n.column = column;
            n.type = type;
            if (args != null)
            {
                n.args = new List<object>();
                for (int i = 0; i < args.Count; i++)
                {
                    n.args.Add(args[i]);
                    if (n.args[i].GetType() == typeof(Note))
                    {
                        n.args[i] = ((Note)n.args[i]).Copy();
                    }
                }
            }
            return n;
        }
    }

}
