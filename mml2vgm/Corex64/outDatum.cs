﻿using musicDriverInterface;
using System.Collections.Generic;

namespace Corex64
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

        public outDatum(byte val)
        {
            this.val = val;
        }

        public outDatum(enmMMLType type, List<object> args, LinePos linePos, byte val)
        {
            this.type = type;
            this.args = args;
            this.linePos = linePos;
            this.val = val;
        }

        public outDatum Copy()
        {
            outDatum ret = new outDatum();
            ret.type = this.type;
            ret.type = this.type;
            ret.args = this.args;
            ret.linePos = this.linePos;
            ret.val = this.val;

            return ret;
        }
    }
}
