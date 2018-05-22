using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public enum enmFormat
    {
        VGM,
        XGM,
        ZGM
    }

    public enum enmChannelType
    {
        Multi,   // その他
        FMOPL,   // OPL系のFMCh
        FMOPN,   // OPN系のFMCh
        FMOPNex, // OPN系の拡張FMCh
        FMOPM,   // OPM系のFMCh
        DCSG,
        PCM,
        ADPCM,
        RHYTHM,
        FMPCM,
        DCSGNOISE,
        SSG,
        ADPCMA,
        ADPCMB,
        WaveForm,
        FMPCMex
    }

    public enum enmChipType : int
    {
        None = -1,
        YM2151 = 0,
        YM2203 = 1,
        YM2608 = 2,
        YM2610B = 3,
        YM2612 = 4,
        SN76489 = 5,
        RF5C164 = 6,
        SEGAPCM = 7,
        HuC6280 = 8,
        YM2612X = 9,
        YM2413 = 10
    }

    public enum ePartType
    {
        YM2612, YM2612extend, SegaPSG, Rf5c164
    }

    public enum eLfoType
    {
        Tremolo, Vibrato, Hardware
    }

}
