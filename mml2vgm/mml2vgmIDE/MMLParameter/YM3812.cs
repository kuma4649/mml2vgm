using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mml2vgmIDE.MMLParameter
{
    public class YM3812 : Instrument
    {
        public YM3812(SoundManager.Chip chip) : base(9+5, chip)
        {
            for (int i = 0; i < 9+5; i++)
            {
                vol[i] = 63;
                beforeTie[i] = false;
            }
        }

        public override string Name => "YM3812";

        public override void SetParameter(outDatum od, int cc)
        {
        }
    }
}
