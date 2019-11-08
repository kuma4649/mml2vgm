using Core;
using System.Collections.Generic;

namespace mml2vgmIDE.MMLParameter
{
    public abstract class Instrument
    {
        public abstract string Name { get; }

        public int ChCount;
        public string[] inst;
        public int?[] envelope;
        public string[] notecmd;
        public int?[] vol;
        public int?[] expression;
        public int?[] velocity;
        public int?[] octave;
        public string[] length;
        public string[] pan;
        public string[] envSw;
        public string[] lfoSw;
        public int?[] detune;
        public int?[] keyShift;
        public int?[] keyOnMeter;
        public bool[] beforeTie;

        public Queue<outDatum>[] TraceInfo;
        public outDatum[] TraceInfoOld;
        public bool isTrace;
        public SoundManager.Chip chip;

        public Instrument(int n, SoundManager.Chip chip)
        {
            ChCount = n;
            inst = new string[n];
            envelope = new int?[n];
            notecmd = new string[n];
            vol = new int?[n];
            expression = new int?[n];
            velocity = new int?[n];
            octave = new int?[n];
            length = new string[n];
            pan = new string[n];
            envSw = new string[n];
            lfoSw = new string[n];
            detune = new int?[n];
            keyShift = new int?[n];
            keyOnMeter = new int?[n];
            beforeTie = new bool[n];

            TraceInfo = new Queue<outDatum>[n];
            for (int i = 0; i < n; i++) TraceInfo[i] = new Queue<outDatum>();
            TraceInfoOld = new outDatum[n];
            this.chip = chip;
        }

        public abstract void SetParameter(outDatum od, int cc);
        
        public void SetMute(int ch, bool flg)
        {
            if (chip == null) return;
            if (chip.ChMasks == null) return;
            if (ch < 0 || ch >= chip.ChMasks.Length) return;
            chip.ChMasks[ch] = flg;
        }

    }

}
