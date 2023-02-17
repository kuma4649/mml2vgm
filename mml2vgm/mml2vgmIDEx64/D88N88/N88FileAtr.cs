using System;
using System.Text;

namespace mml2vgmIDE.D88N88
{
    public class N88FileAtr
    {
        public byte[] raw;
        public byte[] fnb = new byte[9];
        public string fn = "";
        public byte atr;
        public byte cluster;

        public N88FileAtr(byte[] raw)
        {
            this.raw = raw;
            Array.Copy(raw, 0, fnb, 0, 9);
            atr = raw[9];
            cluster = raw[10];
            try
            {
                //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                fn = System.Text.Encoding.GetEncoding("shift_jis").GetString(fnb);
                fn = string.Format("{0}{1}{2}",
                    fn.Substring(0, 6)
                    , "."
                    //, ((atr & 1) != 0 ? "*" : ((atr & 0x80) == 0 ? " " : "."))
                    , fn.Substring(6));
            }
            catch { }
        }
    }

}
