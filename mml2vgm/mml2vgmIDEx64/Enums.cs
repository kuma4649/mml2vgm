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
        , YM3812 = 8
        , YMF262 = 9
        , YM2413 = 10
        , YM3526 = 11
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
        , Force
        , ForceBlock
    }

    public enum EnmVRModel
    {
        None
        , VirtualModel
        , RealModel
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
    , PPZ8
    , PPSDRV
    , P86

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
    , S_PPZ8
    , S_PPSDRV
    , S_P86
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
        MUC = 17,
        ZGM = 18,
        M = 19,
        XGM2 = 20,
    }

    public enum EnmMmlFileFormat : int
    {
        unknown = 0,
        GWI = 1,
        MUC = 2,
        MML = 3,
        MDL = 4
    }

    public enum SendMode : int
    {
        none = 0,
        MML = 1,
        RealTime = 2,
        Both = 3,
        Force = 4
    }

    public enum MusicInterruptTimer
    {
        StopWatch,
        DateTime,
        QueryPerformanceCounter
    }
}
