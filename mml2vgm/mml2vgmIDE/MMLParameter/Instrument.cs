using Core;
using musicDriverInterface;
using System;
using System.Collections.Generic;
using System.Linq;

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

        private Action<outDatum, int, int>[] SetParam;

        public Instrument(int n, SoundManager.Chip chip, Setting setting)
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
                vol[i] = 0;
                beforeTie[i] = false;
            }
            TraceInfoOld = new outDatum[n];
            this.chip = chip;
            this.setting = setting;

            SetParam = new Action<outDatum, int, int>[]
            {
                //0 -
                null,                null,             SetTempo,    SetInstrument,   SetVolume,
                SetTotalVolume,      SetOctave,        SetOctaveUp, SetOctaveDown,   null,
                //10 -                                 
                null,                SetLength,        null,        SetPan,          SetDetune,
                null,                null,             null,        SetGatetime,     null,
                //20 -               
                SetEnvelope,         SetExtendChannel, null,        null,            null,
                null,                null,             null,        null,            SetLfo,
                //30 -                                 
                SetLfoSwitch,        null,             null,        null,            SetKeyShift,
                null,                SetMIDICh,        null,        SetNote,         SetRest,
                //40 -                                 
                null,                null,             null,        null,            null,
                null,                SetLyric,         null,        null,            SetVelocity,
                //50 -                                 
                null,                null,             null,        SetClockCounter, null,
                null,                null,             null,        null,            null,
                //60 -                                 
                SetTraceUpdateStack, null,             null,
            };
        }


        public void SetMute(int ch, bool flg)
        {
            if (chip == null) return;
            if (chip.ChMasks == null) return;
            if (ch < 0 || ch >= chip.ChMasks.Length) return;
            chip.ChMasks[ch] = flg;
        }

        public void SetParameter(outDatum od, int cc)
        {
            if (od == null || od.args == null || od.args.Count < 1) 
                return;

            int ch = ShapingCh(od);

            if (isTrace)
            {
                if (ch < TraceInfo.Length) TraceInfo[ch].Enqueue(od);
            }

            Console.WriteLine("{0}",od.type);
            SetParam[(int)od.type]?.Invoke(od, ch, cc);
        }

        protected virtual int ShapingCh(outDatum od)
        {
            int ch = od.linePos.ch;
            return ch;
        }

        //0 -

        protected virtual void SetTempo(outDatum od, int ch, int cc)
        {
        }

        protected virtual void SetInstrument(outDatum od, int ch, int cc)
        {
            if (setting.MMLParameter.dispInstrumentName
                && od.args.Count == 3
                && (od.args[2] != null
                && od.args[2].ToString() != ""))
                inst[od.linePos.ch] = od.args[2].ToString();
            else
                inst[od.linePos.ch] = od.args[1] != null ? od.args[1].ToString() : "(null)";

            //Console.WriteLine("{0} {1}",ch, od.args[1]);
        }

        protected virtual void SetVolume(outDatum od, int ch, int cc)
        {
            if (od.linePos != null)
                vol[ch] = (int)od.args[0];
        }

        protected virtual void SetTotalVolume(outDatum od, int ch, int cc)
        {
            if (od.linePos != null)
                vol[ch] = (int)od.args[0];
        }

        protected virtual void SetOctave(outDatum od, int ch, int cc)
        {
            octave[ch] = (int)od.args[0];
        }

        protected virtual void SetOctaveUp(outDatum od, int ch, int cc)
        {
            octave[ch]++;
        }

        protected virtual void SetOctaveDown(outDatum od, int ch, int cc)
        {
            octave[ch]--;
        }

        //10 -

        protected virtual void SetLength(outDatum od, int ch, int cc)
        {
            length[od.linePos.ch] = od.args[0].ToString();
        }

        protected virtual void SetPan(outDatum od, int ch, int cc)
        {
            pan[od.linePos.ch] = string.Format("L{0} R{1}", (int)od.args[0], (int)od.args[1]);
        }

        protected virtual void SetDetune(outDatum od, int ch, int cc)
        {
            int n = (int)od.args[0];
            detune[ch] = n;
        }

        protected virtual void SetGatetime(outDatum od, int ch, int cc)
        {
        }

        protected virtual void SetLfo(outDatum od, int ch, int cc)
        {
        }

        //20 -

        protected virtual void SetEnvelope(outDatum od, int ch, int cc)
        {
            string s = (string)od.args[0];
            envSw[ch] = s == "EON" ? "ON " : "OFF";
        }

        protected virtual void SetExtendChannel(outDatum od, int ch, int cc)
        {
        }

        //30 -

        protected virtual void SetLfoSwitch(outDatum od, int ch, int cc)
        {
            string s = (string)od.args[2];
            lfoSw[ch] = s;
        }

        protected virtual void SetKeyShift(outDatum od, int ch, int cc)
        {
            int n = (int)od.args[0];
            keyShift[ch] = n;
        }

        protected virtual void SetMIDICh(outDatum od, int ch, int cc)
        {
            int n = (int)od.args[0];
            MIDIch[od.linePos.ch] = n + 1;
        }

        protected virtual void SetNote(outDatum od, int ch, int cc)
        {
            Core.Note nt = (Core.Note)od.args[0];
            int shift = nt.shift;
            string f = Math.Sign(shift) >= 0 ? string.Concat(Enumerable.Repeat("+", shift)) : string.Concat(Enumerable.Repeat("-", -shift));
            notecmd[ch] = string.Format("o{0}{1}{2}", octave[ch], nt.cmd, f);
            length[ch] = string.Format("{0:0.##}(#{1:d})", 1.0 * cc / nt.length, nt.length);

            if (!beforeTie[ch])
            {
                if (vol[ch] != null)
                {
                    keyOnMeter[ch] = (int)(256.0 / (
                        od.linePos.part == "FMOPN" ? 128 : (
                        od.linePos.part == "FMOPNex" ? 128 : (
                        od.linePos.part == "SSG" ? 16 : (
                        od.linePos.part == "RHYTHM" ? 32 : 256
                        )))) * vol[ch]);
                }
            }
            beforeTie[ch] = nt.tieSw;
        }

        protected virtual void SetRest(outDatum od, int ch, int cc)
        {
            Core.Rest rs = (Core.Rest)od.args[0];
            notecmd[ch] = "r";
            length[ch] = string.Format("{0:0.##}(#{1:d})", 1.0 * cc / rs.length, rs.length);
        }

        //40 -

        protected virtual void SetLyric(outDatum od, int ch, int cc)
        {
        }

        protected virtual void SetVelocity(outDatum od, int ch, int cc)
        {
            if (od.linePos != null)
                velocity[od.linePos.ch] = (int)od.args[0];
        }

        //50 -

        protected virtual void SetClockCounter(outDatum od, int ch, int cc)
        {
            clockCounter[ch] = (int)od.args[0];
        }

        //60 -

        protected virtual void SetTraceUpdateStack(outDatum od, int ch, int cc)
        {
        }

    }

}
