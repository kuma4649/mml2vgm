using Core;

namespace mml2vgmIDE.MMLParameter
{
    public class C352 : Instrument
    {
        public C352(SoundManager.Chip chip) : base(32, chip)
        {
            for (int i = 0; i < 32; i++)
            {
                vol[i] = 255;
                beforeTie[i] = false;
            }
        }

        public override string Name => "C352";

        public override void SetParameter(outDatum od, int cc)
        {

        }
    }
}
