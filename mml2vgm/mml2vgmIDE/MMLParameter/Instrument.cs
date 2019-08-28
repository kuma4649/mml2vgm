using Core;

namespace mml2vgmIDE.MMLParameter
{
    public abstract class Instrument
    {
        public abstract string Name { get; }

        public string[] inst;
        public int?[] envelope;
        public string[] notecmd;
        public int?[] vol;
        public int?[] octave;
        public string[] length;
        public string[] pan;
        public string[] envSw;
        public string[] lfoSw;
        public int?[] detune;
        public int?[] keyShift;
        public int?[] keyOnMeter;
        public bool[] beforeTie;

        public Instrument(int n)
        {
            inst = new string[n];
            envelope = new int?[n];
            notecmd = new string[n];
            vol = new int?[n];
            octave = new int?[n];
            length = new string[n];
            pan = new string[n];
            envSw = new string[n];
            lfoSw = new string[n];
            detune = new int?[n];
            keyShift = new int?[n];
            keyOnMeter = new int?[n];
            beforeTie = new bool[n];
        }

        public abstract void SetParameter(outDatum od, int cc);

    }

}
