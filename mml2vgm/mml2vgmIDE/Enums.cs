using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mml2vgmIDE
{
    public enum EnmRealChipType : int
    {
        YM2608 = 1
        , YM2151 = 2
        , YM2610 = 3
        , YM2203 = 4
        , YM2612 = 5
        , AY8910 = 6
        , SN76489 = 7
        , YM2413 = 10
        , SPPCM = 42
        , C140 = 43
        , SEGAPCM = 44
    }

    public enum EnmDataType
    {
        None
    , Normal
    , Block
    , Loop
    , FadeOut
    }

    public enum EnmModel
    {
        None
        , VirtualModel
        , RealModel
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

    public enum EnmChip : int
    {
        Unuse = 0

    , SN76489
    , YM2612
    , YM2612Ch6
    , RF5C164
    , PWM
    , C140
    , OKIM6258
    , OKIM6295
    , SEGAPCM
    , YM2151
    , YM2608
    , YM2203
    , YM2610
    , AY8910
    , HuC6280
    , YM2413
    , NES
    , DMC
    , FDS
    , MMC5
    , YMF262
    , YMF278B
    , VRC7
    , C352
    , YM3526
    , Y8950
    , YM3812
    , K051649
    , N160
    , VRC6
    , FME7
    , RF5C68
    , MultiPCM
    , YMF271
    , YMZ280B
    , QSound
    , GA20
    , K053260
    , K054539
    , DMG

    , S_SN76489
    , S_YM2612
    , S_YM2612Ch6
    , S_RF5C164
    , S_PWM
    , S_C140
    , S_OKIM6258
    , S_OKIM6295
    , S_SEGAPCM
    , S_YM2151
    , S_YM2608
    , S_YM2203
    , S_YM2610
    , S_AY8910
    , S_HuC6280
    , S_YM2413
    , S_NES
    , S_DMC
    , S_FDS
    , S_MMC5
    , S_YMF262
    , S_YMF278B
    , S_VRC7
    , S_C352
    , S_YM3526
    , S_Y8950
    , S_YM3812
    , S_K051649
    , S_N160
    , S_VRC6
    , S_FME7
    , S_RF5C68
    , S_MultiPCM
    , S_YMF271
    , S_YMZ280B
    , S_QSound
    , S_GA20
    , S_K053260
    , S_K054539
    , S_DMG
    }

    public enum EnmRealModel
    {
        unknown,
        SCCI,
        GIMIC
    }

    public enum EnmFileFormat : int
    {
        unknown = 0,
        VGM = 1,
        NRT = 2,
        XGM = 3,
        S98 = 4,
        MID = 5,
        RCP = 6,
        NSF = 7,
        HES = 8,
        ZIP = 9,
        M3U = 10,
        SID = 11,
        MDR = 12,
        LZH = 13,
        MDX = 14,
        MND = 15,
        MUB = 16,
        MUC = 17
    }

    public enum SendMode : int
    {
        none = 0,
        MML = 1,
        RealTime = 2,
        Both = 3
    }
}
