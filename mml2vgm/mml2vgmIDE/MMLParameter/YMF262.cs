using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mml2vgmIDE.MMLParameter
{
    public class YMF262 : Instrument
    {
        public YMF262(SoundManager.Chip chip) : base(23, chip)
        {
            for (int i = 0; i < 23; i++)
            {
                vol[i] = 63;
                beforeTie[i] = false;
            }
        }

        public override string Name => "YMF262";

        public override void SetParameter(outDatum od, int cc)
        {
        }
    }
}
