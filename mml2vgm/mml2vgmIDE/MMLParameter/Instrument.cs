using Core;
using System.Collections.Generic;

namespace mml2vgmIDE.MMLParameter
{
    public abstract class Instrument
    {
        public abstract string Name { get; }

        public int ChCount;
        public string[] inst;
        public string[] envelope;
        public string[] notecmd;
        public string[] gatetime;
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
        public int?[] MIDIch;
        public bool[] beforeTie;
        public int[] clockCounter;

        public Queue<outDatum>[] TraceInfo;
        public outDatum[] TraceInfoOld;
        public bool isTrace;
        public SoundManager.Chip chip;
        public Setting setting = null;

        public Instrument(int n, SoundManager.Chip chip,Setting setting)
        {
            ChCount = n;
            inst = new string[n];
            envelope = new string[n];
            gatetime = new string[n];
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
            clockCounter = new int[n];
            MIDIch = new int?[n];

            TraceInfo = new Queue<outDatum>[n];
            for (int i = 0; i < n; i++)
            {
                TraceInfo[i] = new Queue<outDatum>();
                clockCounter[i] = 128;
            }
            TraceInfoOld = new outDatum[n];
            this.chip = chip;
            this.setting = setting;
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
