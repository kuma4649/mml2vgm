using Corex64;
using musicDriverInterface;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace mml2vgmIDEx64.MMLParameter
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
        public int?[] volMode;
        public int?[] expression;
        public int?[] velocity;
        public int?[] octave;
        public string[] length;
        public string[] pan;
        public string[] envSw;
        public string[] lfoSw;
        public string[] lfo;
        public string[] hlfo;
        public string[] memo;
        public int?[] detune;
        public int?[] keyShift;
        public int?[] keyOnMeter;
        public int?[] MIDIch;
        public bool[] beforeTie;
        public int[] clockCounter;
        public int[] partColor;

        public ConcurrentQueue<outDatum>[] TraceInfo;
        public outDatum[] TraceInfoOld;
        public bool isTrace;
        public SoundManager.Chip chip;
        public Setting setting = null;
        public MIDIKbd midiKbd = null;

        private Action<outDatum, int, int>[] SetParam;

        public Instrument(int n, SoundManager.Chip chip, Setting setting,MIDIKbd midiKbd)
        {
            ChCount = n;
            inst = new string[n];
            envelope = new string[n];
            gatetime = new string[n];
            notecmd = new string[n];
            vol = new int?[n];
            volMode = new int?[n];
            expression = new int?[n];
            velocity = new int?[n];
            octave = new int?[n];
            length = new string[n];
            pan = new string[n];
            envSw = new string[n];
            lfoSw = new string[n];
            lfo = new string[n];
            hlfo = new string[n];
            memo = new string[n];
            detune = new int?[n];
            keyShift = new int?[n];
            keyOnMeter = new int?[n];
            beforeTie = new bool[n];
            clockCounter = new int[n];
            MIDIch = new int?[n];
            partColor = new int[n];

            TraceInfo = new ConcurrentQueue<outDatum>[n];
            for (int i = 0; i < n; i++)
            {
                TraceInfo[i] = new ConcurrentQueue<outDatum>();
                clockCounter[i] = 128;
                vol[i] = 0;
                volMode[i] = 0;
                beforeTie[i] = false;
                partColor[i] = 1;
            }
            TraceInfoOld = new outDatum[n];
            this.chip = chip;
            this.setting = setting;
            this.midiKbd = midiKbd;

            SetParam = new Action<outDatum, int, int>[]
            {
                //0 -
                null,                null,             SetTempo,     SetInstrument,   SetVolume,
                SetTotalVolume,      SetOctave,        SetOctaveUp,  SetOctaveDown,   null,
                //10 -                                 
                null,                SetLength,        null,         SetPan,          SetDetune,
                null,                null,             null,         SetGatetime,     null,
                //20 -               
                SetEnvelope,         SetExtendChannel, null,         null,            null,
                null,                null,             null,         null,            SetLfo,
                //30 -                                 
                SetLfoSwitch,        null,             null,         null,            SetKeyShift,
                null,                SetMIDICh,        null,         SetNote,         SetRest,
                //40 -                                 
                null,                null,             null,         null,            null,
                null,                SetLyric,         null,         null,            SetVelocity,
                //50 -                                 
                null,                null,             null,         SetClockCounter, null,
                null,                null,             null,         null,            null,
                //60 -                                 
                SetTraceUpdateStack, null,             null,         null,            null,
                null,                null,             SetPartColor, null,            null,
                //70 -                                 
                null,                null,             null,         null,            null,
                null,                SetHardLFO,       null,         null,            null,
                //80 -                                 
                null,                null,             null,         null,            null,
                null,                null,             null,         null,            null,
            };
        }


        public void SetMute(int ch, bool flg)
        {
            if (chip == null) return;
            if (chip.ChMasks == null) return;
            if (ch < 0 || ch >= chip.ChMasks.Count) return;
            chip.ChMasks[ch] = flg;
        }

        public virtual void SetAssign(int ch, bool flg)
        {
        }

        public void SetParameter(outDatum od, int cc, bool traceSkip = false)
        {
            if (od == null || od.args == null || od.args.Count < 1)
                return;

            int ch = ShapingCh(od);
            //if (Audio.isInitialPhase()) return;

            if (isTrace && !traceSkip)
            {
                if (ch < TraceInfo.Length)
                {
                    try
                    {
                        if (!Audio.isInitialPhase())
                        {
                            TraceInfo[ch].Enqueue(od);
                        }
                    }
                    catch
                    {
                        ;
                    }
                }
            }

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
            if (od.linePos.ch >= inst.Length) return;

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
            if (ch >= vol.Length) 
                return;

            if (od.linePos != null)
                vol[ch] = (int)od.args[0];
        }

        protected virtual void SetTotalVolume(outDatum od, int ch, int cc)
        {
            if (ch >= vol.Length) return;

            if (od.linePos != null)
                vol[ch] = (int)od.args[0];
        }

        protected virtual void SetOctave(outDatum od, int ch, int cc)
        {
            if (ch >= octave.Length) return;
            octave[ch] = (int)od.args[0];
        }

        protected virtual void SetOctaveUp(outDatum od, int ch, int cc)
        {
            if (ch >= octave.Length) return;
            octave[ch]++;
        }

        protected virtual void SetOctaveDown(outDatum od, int ch, int cc)
        {
            if (ch >= octave.Length) return;
            octave[ch]--;
        }

        //10 -

        protected virtual void SetLength(outDatum od, int ch, int cc)
        {
            if (od.linePos.ch >= length.Length) return;
            length[od.linePos.ch] = od.args[0].ToString();
        }

        protected virtual void SetPan(outDatum od, int ch, int cc)
        {
            if (od.linePos.ch >= pan.Length) return;
            if(od.args.Count>1)
                pan[od.linePos.ch] = string.Format("L{0} R{1}", (int)od.args[0], (int)od.args[1]);
            else
                pan[od.linePos.ch] = string.Format("{0}", (int)od.args[0]);
        }

        protected virtual void SetDetune(outDatum od, int ch, int cc)
        {
            if (ch >= detune.Length) return;
            if (od.args.Count == 2)
            {
                string cmd = (string)od.args[0];
                int n = (int)od.args[1];

                switch (cmd)
                {
                    case "D":
                        detune[ch] = n;
                        break;
                    case "D>":
                        detune[ch] += n;
                        break;
                    case "D<":
                        detune[ch] -= n;
                        break;
                }
            }
            else
            {
                int n = (int)od.args[0];
                detune[ch] = n;
            }

        }

        protected virtual void SetGatetime(outDatum od, int ch, int cc)
        {
        }

        //20 -

        protected virtual void SetEnvelope(outDatum od, int ch, int cc)
        {
            if (ch >= envSw.Length) return;
            string s = (string)od.args[0];
            envSw[ch] = s == "EON" ? "ON " : "OFF";
        }

        protected virtual void SetExtendChannel(outDatum od, int ch, int cc)
        {
        }

        protected virtual void SetLfo(outDatum od, int ch, int cc)
        {
            ;
        }

        //30 -

        protected virtual void SetLfoSwitch(outDatum od, int ch, int cc)
        {
            if (ch >= lfoSw.Length) return;
            string s = (string)od.args[2];
            lfoSw[ch] = s;
        }

        protected virtual void SetKeyShift(outDatum od, int ch, int cc)
        {
            if (ch >= keyShift.Length) return;
            int n = (int)od.args[0];
            keyShift[ch] = n;
        }

        protected virtual void SetMIDICh(outDatum od, int ch, int cc)
        {
            if (ch >= MIDIch.Length) return;
            int n = (int)od.args[0];
            MIDIch[od.linePos.ch] = n + 1;
        }

        protected virtual void SetNote(outDatum od, int ch, int cc)
        {
            if (ch >= notecmd.Length) return;
            if (!(od.args[0] is Corex64.Note)) return;

            Corex64.Note nt = (Corex64.Note)od.args[0];
            int shift = nt.shift;
            string f = Math.Sign(shift) >= 0 ? string.Concat(Enumerable.Repeat("+", shift)) : string.Concat(Enumerable.Repeat("-", -shift));
            if (nt.trueKeyOn)
                notecmd[od.linePos.ch] = string.Format("o{0}{1}{2}", octave[ch], nt.cmd, f);
            else
                notecmd[od.linePos.ch] = string.Format("skip (o{0}{1}{2})", octave[ch], nt.cmd, f);
            length[ch] = string.Format("{0:0.##}(#{1:d})", 1.0 * cc / nt.length, nt.length);

            if (!beforeTie[ch])
            {
                if (vol[ch] != null)
                {
                    if (nt.trueKeyOn)
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
            if (ch >= notecmd.Length) return;
            if (!(od.args[0] is Corex64.Rest)) return;

            Corex64.Rest rs = (Corex64.Rest)od.args[0];
            notecmd[ch] = "r";
            length[ch] = string.Format("{0:0.##}(#{1:d})", 1.0 * cc / rs.length, rs.length);
        }

        //40 -

        protected virtual void SetLyric(outDatum od, int ch, int cc)
        {
            if (od.args.Count > 1)
            {
                //通常の歌詞
                return;
            }

            //メモ
            memo[ch]=(string)od.args[0];
        }

        protected virtual void SetVelocity(outDatum od, int ch, int cc)
        {
            if (od.linePos.ch >= velocity.Length) return;

            if (od.linePos != null)
                velocity[od.linePos.ch] = (int)od.args[0];
        }

        //50 -

        protected virtual void SetClockCounter(outDatum od, int ch, int cc)
        {
            if (ch >= clockCounter.Length) return;

            clockCounter[ch] = (int)od.args[0];
        }

        //60 -

        protected virtual void SetTraceUpdateStack(outDatum od, int ch, int cc)
        {
        }

        protected virtual void SetPartColor(outDatum od, int ch, int cc)
        {
            if (ch >= partColor.Length) return;

            partColor[ch] = (int)od.args[0];
        }

        protected virtual void SetHardLFO(outDatum od, int ch, int cc)
        {
            ;
        }

        public void bendOctaveHosei(outDatum od,Corex64.Note nt)
        {
            if (!nt.bendSw) return;
            foreach (MML o in nt.bendOctave)
            {
                if (o == null) continue;
                switch (o.type)
                {
                    case enmMMLType.Octave:
                        octave[od.linePos.ch] = (int)o.args[0];
                        break;
                    case enmMMLType.OctaveUp:
                        octave[od.linePos.ch]++;
                        break;
                    case enmMMLType.OctaveDown:
                        octave[od.linePos.ch]--;
                        break;
                }
            }
        }
    }

}
