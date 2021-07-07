using System.Collections.Generic;

namespace Core
{
    public class Rest
    {

        public char cmd = 'r';
        public int length = 0;

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

        public int addLength { get; internal set; } = 0;
    }
}
