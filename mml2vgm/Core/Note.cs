using System;
using System.Collections.Generic;

namespace Core
{
    public class Rest
    {

        public char cmd = 'r';
        public int length = 0;

        /// <summary>
        /// 次が音符かどうか(RR15処理用)
        /// </summary>
        public bool nextIsNote { get; internal set; }

        public Rest Copy()
        {
            Rest n = new Rest();
            n.cmd = this.cmd;
            n.length = this.length;
            n.nextIsNote = this.nextIsNote;

            return n;
        }
    }

    public class Note : Rest
    {

        public int shift = 0;
        public int pitchShift = 0;
        public bool tieSw = false;

        public bool bendSw = false;
        public char bendCmd = ' ';
        public int bendShift = 0;
        public int bendPitchShift = 0;
        public List<MML> bendOctave;

        public bool tDblSw = false;
        public char tDblCmd = ' ';
        public int tDblShift = 0;
        public List<MML> tDblOctave;

        public int velocity = -1;
        public bool chordSw = false;

        public int addLength = 0;
        public bool trueKeyOn = false;

        public Note Copy()
        {
            Note n = new Note();

            n.shift = this.shift;
            n.pitchShift = this.pitchShift;
            n.tieSw = this.tieSw;
            n.cmd = this.cmd;
            n.length = this.length;

            n.bendSw = this.bendSw;
            n.bendCmd = this.bendCmd;
            n.bendShift = this.bendShift;
            n.bendPitchShift = this.bendPitchShift;
            n.bendOctave = this.bendOctave;

            n.tDblSw = this.tDblSw;
            n.tDblCmd = this.tDblCmd;
            n.tDblShift = this.tDblShift;
            n.tDblOctave=this.tDblOctave;

            n.velocity = this.velocity;
            n.chordSw = this.chordSw;

            n.addLength = this.addLength;
            n.trueKeyOn = this.trueKeyOn;

            n.nextIsNote = this.nextIsNote;
            
            return n;
        }
    }
}
