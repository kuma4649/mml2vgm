using Core;

namespace mml2vgmIDE.MMLParameter
{
    public class YM3526 : Instrument
    {
        public YM3526(SoundManager.Chip chip) : base(9 + 5, chip)
        {
            for (int i = 0; i < 9 + 5; i++)
            {
                vol[i] = 63;
                beforeTie[i] = false;
            }
        }

        public override string Name => "YM3526";

        public override void SetParameter(outDatum od, int cc)
        {
        }
    }
}
