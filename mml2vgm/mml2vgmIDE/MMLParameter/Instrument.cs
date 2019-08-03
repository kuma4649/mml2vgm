using Core;

namespace mml2vgmIDE.MMLParameter
{
    public abstract class Instrument
    {
        public abstract string Name { get; }

        public int?[] inst;
        public int?[] envelope;
        public string[] notecmd;
        public int?[] vol;
        public int?[] octave;
        public string[] length;
        public string[] pan;

        public Instrument(int n)
        {
            inst = new int?[n];
            envelope = new int?[n];
            notecmd = new string[n];
            vol = new int?[n];
            octave = new int?[n];
            length = new string[n];
            pan = new string[n];
        }

        public abstract void SetParameter(outDatum od, int cc);

    }

}
