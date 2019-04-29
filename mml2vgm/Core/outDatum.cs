using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class outDatum
    {
        public enmMMLType type = enmMMLType.unknown;
        public List<object> args = null;
        public byte val = 0;
        public LinePos linePos = null;

        public outDatum()
        {
        }

        public outDatum(enmMMLType type, List<object> args, LinePos linePos, byte val)
        {
            this.type = type;
            this.args = args;
            this.linePos = linePos;
            this.val = val;
        }
    }
}
