using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mml2vgmIDE.MMLParameter
{
    public class PPZ8 : Instrument
    {
        public override string Name => "PPZ8";

        public PPZ8(SoundManager.Chip chip) : base(8, chip)
        {
            for (int i = 0; i < 8; i++)
            {
                vol[i] = 0;
                beforeTie[i] = false;
            }
        }



        public override void SetParameter(outDatum od, int cc)
        {
            try
            {
                int ch = od.linePos.ch;
                if (isTrace)
                {
                    if (ch < TraceInfo.Length) TraceInfo[ch].Enqueue(od);
                }
            }
            catch
            {
                ;//握りつぶす
            }
        }


    }
}
