using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mucomDotNET.Interface
{
    public class OPNAData
    {
        public byte port = 0;
        public byte address = 0;
        public byte data = 0;
        public ulong time = 0L;
        public object addtionalData = null;

        public OPNAData(byte port, byte address, byte data, ulong time = 0, object addtionalData = null)
        {
            this.port = port;
            this.address = address;
            this.data = data;
            this.time = time;
            this.addtionalData = addtionalData;
        }
    }
}
