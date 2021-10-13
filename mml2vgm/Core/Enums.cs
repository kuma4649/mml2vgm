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
        FMOPX,   // OPX系のFMCh
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
        MIDI,
        Pulse,
        Triangle,
        Noise,
        DPCM,
        Square,
        Saw
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
        MIDI_GM = 18,
        YMF262 = 19,
        C352 = 20,
        YM3526 = 21,
        YM3812 = 22,
        Y8950 = 23,
        YMF271 = 24,
        NES = 25,
        DMG = 26,
        VRC6 = 27
    }

    public enum EnmZGMDevice : int
    {
        None = 0x0000_0000
        //VGM Chips(VGMで使用されるエミュレーションチップ定義)
        , SN76489 = 0x0000_000C
        , YM2413 = 0x0000_0010
        , YM2612 = 0x0000_002C
        , YM2151 = 0x0000_0030
        , SegaPCM = 0x0000_0038
        , RF5C68 = 0x0000_0040
        , YM2203 = 0x0000_0044
        , YM2608 = 0x0000_0048
        , YM2610 = 0x0000_004C
        , YM2610B = 0x0000_004C
        , YM3812 = 0x0000_0050
        , YM3526 = 0x0000_0054
        , Y8950 = 0x0000_0058
        , YMF262 = 0x0000_005C
        , YMF278B = 0x0000_0060
        , YMF271 = 0x0000_0064
        , YMZ280B = 0x0000_0068
        , RF5C164 = 0x0000_006C
        , PWM = 0x0000_0070
        , AY8910 = 0x0000_0074
        , GameBoyDMG = 0x0000_0080
        , NESAPU = 0x0000_0084
        , MultiPCM = 0x0000_0088
        , uPD7759 = 0x0000_008C
        , OKIM6258 = 0x0000_0090
        , OKIM6295 = 0x0000_0098
        , K051649 = 0x0000_009C
        , K054539 = 0x0000_00A0
        , HuC6280 = 0x0000_00A4
        , C140 = 0x0000_00A8
        , K053260 = 0x0000_00AC
        , Pokey = 0x0000_00B0
        , QSound = 0x0000_00B4
        , SCSP = 0x0000_00B8
        , WonderSwan = 0x0000_00C0
        , VirtualBoyVSU = 0x0000_00C4
        , SAA1099 = 0x0000_00C8
        , ES5503 = 0x0000_00CC
        , ES5505 = 0x0000_00D0
        , ES5506 = 0x0000_00D0
        , X1_010 = 0x0000_00D8
        , C352 = 0x0000_00DC
        , GA20 = 0x0000_00E0
        // Chips                
        , Conductor = 0x0001_0000
        , VRC6 = 0x0001_0004
        , VRC7 = 0x0001_0008
        , MMC5 = 0x0001_000C
        , N106 = 0x0001_0010
        , S5B = 0x0001_0014
        // 妄想Chips            
        , OtherChips = 0x0002_0000
        , AY8910B = 0x0002_0000
        , YM2609 = 0x0002_0001
        // XG音源               
        , MIDIXG = 0x0003_0000
        , MU50 = 0x0003_0000
        // LA/GS音源            
        , MIDIGS = 0x0004_0000
        , MT32 = 0x0004_0000
        //GM                    
        , MIDIGM = 0x0005_0000
        //VSTi                  
        , MIDIVSTi = 0x0006_0000
        , VOPMex = 0x0006_0000
        //Wave                  
        , Wave = 0x0007_0000
        , RawWave = 0x0007_0000
        //ドライバ固有音源
        , basicDriverChip = 0x0008_0000
        , PPZ8 = 0x0008_0001
        , PPSDRV = 0x0008_0002
        , P86 = 0x0008_0003
        , DACControl = 0x0008_0004
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
        Hardware,
        Wah
    }

}
