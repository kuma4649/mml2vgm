using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class MML
    {
        public Line line;
        public int column;

        public enmMMLType type;
        public List<object> args;
    }

    public enum enmMMLType
    {
        unknown,
        CompileSkip,    // !
        Tempo,          // T
        Instrument,     // @
        Volume,         // v
        TotalVolume,    // V
        Octave,         // o
        OctaveUp,       // >
        OctaveDown,     // <
        VolumeUp,       // )
        VolumeDown,     // (
        Length,         // l
        LengthClock,    // #
        Pan,            // p
        Detune,         // D
        DirectMode,     // DON DOF
        PcmMode,        // m
        PcmMap,         // mon mof
        Gatetime,       // q
        GatetimeDiv,    // Q
        Envelope,       // E
        ExtendChannel,  // EX
        HardEnvelope,   // EH
        LoopPoint,      // L
        Repeat,         // [
        RepeatEnd,      // ]
        Renpu,          // {
        RenpuEnd,       // }
        RepertExit,     // /
        Lfo,            // M
        LfoSwitch,      // S
        Y,              // y
        Noise,          // w
        NoiseToneMixer, // P
        KeyShift,       // K
        AddressShift,   // A
        Note,           // c d e f g a b
        Rest,           // r
        RestNoWork,     // R
        Bend,           // _
        Tie,            // &
        TiePC,          // ^
        TieMC,          // ~
        ToneDoubler,    // , 0
        Lyric,          // "
        SusOnOff,       // so sf
        JumpPoint,      // J
        SkipPlay        // caret位置からの演奏のための特殊コマンド
    }
}
