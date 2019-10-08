using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;

namespace mml2vgmIDE.MMLParameter
{
    public class GeneralMIDI : Instrument
    {
        public GeneralMIDI() : base(99)
        {
            for (int i = 0; i < 99; i++)
            {
                vol[i] = 127;
                beforeTie[i] = false;
            }
        }

        public override string Name => "GeneralMIDI";

        public override void SetParameter(outDatum od, int cc)
        {
        }
    }
}
