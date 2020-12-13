using Core;

namespace mml2vgmIDE.MMLParameter
{
    public class Y8950 : Instrument
    {
        public Y8950(SoundManager.Chip chip) : base(9 + 5 + 1, chip)
        {
            for (int i = 0; i < 9 + 5 + 1; i++)
            {
                vol[i] = 63;
                beforeTie[i] = false;
            }
        }

        public override string Name => "Y8950";

        public override void SetParameter(outDatum od, int cc)
        {
        }
    }
}