using System;

namespace mml2vgmIDE.MMLParameter
{
    public class InstrumentFactory
    {
        public Instrument Create(string name)
        {
            switch (name)
            {
                case "YM2612":
                //return new YM2612();
                default:
                    throw new ArgumentException();
            }
        }
    }
}
