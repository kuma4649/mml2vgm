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
        FMPCMex,
        MIDI
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
        YM2413 = 10,
        C140 = 11,
        AY8910 = 12,
        CONDUCTOR = 13,
        K051649 = 14,
        QSound = 15,
        K053260 = 16,
        YM2609 = 17,
        MIDI_GM=18
    }

    public enum EnmDevice : int
    {
        None = 0x00000000
        //VGM Chips(VGMで使用されるエミュレーションチップ定義)
        , SN76489 = 0x0000000C
        , YM2413 = 0x00000010
        , YM2612 = 0x0000002C
        , YM2151 = 0x00000030
        , SegaPCM = 0x00000038
        , RF5C68 = 0x00000040
        , YM2203 = 0x00000044
        , YM2608 = 0x00000048
        , YM2610 = 0x0000004C
        , YM2610B = 0x0000004C
        , YM3812 = 0x00000050
        , YM3526 = 0x00000054
        , Y8950 = 0x00000058
        , YMF262 = 0x0000005C
        , YMF278B = 0x00000060
        , YMF271 = 0x00000064
        , YMZ280B = 0x00000068
        , RF5C164 = 0x0000006C
        , PWM = 0x00000070
        , AY8910 = 0x00000074
        , GameBoyDMG = 0x00000080
        , NESAPU = 0x00000084
        , MultiPCM = 0x00000088
        , uPD7759 = 0x0000008C
        , OKIM6258 = 0x00000090
        , OKIM6295 = 0x00000098
        , K051649 = 0x0000009C
        , K054539 = 0x000000A0
        , HuC6280 = 0x000000A4
        , C140 = 0x000000A8
        , K053260 = 0x000000AC
        , Pokey = 0x000000B0
        , QSound = 0x000000B4
        , SCSP = 0x000000B8
        , WonderSwan = 0x000000C0
        , VirtualBoyVSU = 0x000000C4
        , SAA1099 = 0x000000C8
        , ES5503 = 0x000000CC
        , ES5505 = 0x000000D0
        , ES5506 = 0x000000D0
        , X1_010 = 0x000000D8
        , C352 = 0x000000DC
        , GA20 = 0x000000E0
        //OtherChips(仮想チップや他のエミュレーションコア)
        , OtherChips = 0x00010000
        , AY8910B = 0x00010001
        , YM2609 = 0x00010002
        //XG
        , MIDIXG = 0x00020000
        //GS
        , MIDIGS = 0x00030000
        //GM
        , MIDIGM = 0x00040000
        //VSTi
        , MIDIVSTi = 0x00050000
        //Wave
        , Wave = 0x00060000
    }

    public enum enmPcmDefineType
    {
        /// <summary>
        /// 自動定義
        /// </summary>
        Easy,
        /// <summary>
        /// データのみ定義
        /// </summary>
        RawData,
        /// <summary>
        /// 音色情報のみ定義
        /// </summary>
        Set,
        /// <summary>
        /// Mucom88Adpcm定義
        /// </summary>
        Mucom88
    }

    public enum enmPCMSTATUS
    {
        NONE,
        USED,
        ERROR
    }

    //public enum ePartType
    //{
    //    YM2612, YM2612extend, SegaPSG, Rf5c164
    //}

    public enum eLfoType
    {
        unknown,
        Tremolo,
        Vibrato,
        Hardware
    }

}
