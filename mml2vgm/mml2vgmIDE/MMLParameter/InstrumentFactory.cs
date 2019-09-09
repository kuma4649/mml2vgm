using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mml2vgmIDE.MMLParameter
{
    public class InstrumentFactory
    {
        public Instrument Create(string name)
        {
            switch (name)
            {
                case "YM2612":
                    return new YM2612();
                default:
                    throw new ArgumentException();
            }
        }
    }
}
