using Core;
using MDSound.np.chip;
using MDSound.np.cpu;
using MDSound.np.memory;
using SoundManager;
using System;
using System.Collections.Generic;

namespace mml2vgmIDE
{
    public class ChipRegister
    {

        private MDSound.MDSound mds = null;
        private midiOutInfo[] midiOutInfos = null;
        private List<NAudio.Midi.MidiOut> midiOuts = null;
        private List<int> midiOutsType = null;
        //private List<vstInfo2> vstMidiOuts = null;
        //private List<int> vstMidiOutsType = null;


        private Dictionary<MDSound.MDSound.enmInstrumentType, MDSound.MDSound.Chip> dicChipsInfo = new Dictionary<MDSound.MDSound.enmInstrumentType, MDSound.MDSound.Chip>();

        private Setting.ChipType[] ctAY8910 = new Setting.ChipType[2] { null, null };
        private Setting.ChipType[] ctC140 = new Setting.ChipType[2] { null, null };
        private Setting.ChipType[] ctC352 = new Setting.ChipType[2] { null, null };
        private Setting.ChipType[] ctDMG = new Setting.ChipType[2] { null, null };
        private Setting.ChipType[] ctHuC6280 = new Setting.ChipType[2] { null, null };
        private Setting.ChipType[] ctK051649 = new Setting.ChipType[2] { null, null };
        private Setting.ChipType[] ctK053260 = new Setting.ChipType[2] { null, null };
        private Setting.ChipType[] ctNES = new Setting.ChipType[2] { null, null };
        private Setting.ChipType[] ctVRC6 = new Setting.ChipType[2] { null, null };
        private Setting.ChipType[] ctPPZ8 = new Setting.ChipType[2] { null, null };
        private Setting.ChipType[] ctPPSDRV = new Setting.ChipType[2] { null, null };
        private Setting.ChipType[] ctP86 = new Setting.ChipType[2] { null, null };
        private Setting.ChipType[] ctQSound = new Setting.ChipType[2] { null, null };
        private Setting.ChipType[] ctRF5C164 = new Setting.ChipType[2] { null, null };
        private Setting.ChipType[] ctSEGAPCM = new Setting.ChipType[2] { null, null };
        private Setting.ChipType[] ctSN76489 = new Setting.ChipType[2] { null, null };
        private Setting.ChipType[] ctY8950 = new Setting.ChipType[2] { null, null };
        private Setting.ChipType[] ctYM2151 = new Setting.ChipType[2] { null, null };
        private Setting.ChipType[] ctYM2203 = new Setting.ChipType[2] { null, null };
        private Setting.ChipType[] ctYM2413 = new Setting.ChipType[2] { null, null };
        private Setting.ChipType[] ctYM2608 = new Setting.ChipType[2] { null, null };
        private Setting.ChipType[] ctYM2609 = new Setting.ChipType[2] { null, null };
        private Setting.ChipType[] ctYM2610 = new Setting.ChipType[2] { null, null };
        private Setting.ChipType[] ctYM2612 = new Setting.ChipType[2] { null, null };
        private Setting.ChipType[] ctYM3526 = new Setting.ChipType[2] { null, null };
        private Setting.ChipType[] ctYM3812 = new Setting.ChipType[2] { null, null };
        private Setting.ChipType[] ctYMF262 = new Setting.ChipType[2] { null, null };
        private Setting.ChipType[] ctYMF271 = new Setting.ChipType[2] { null, null };
        private Setting.ChipType[] ctYMF278B = new Setting.ChipType[2] { null, null };
        private Setting.ChipType[] ctYMZ280B = new Setting.ChipType[2] { null, null };

        private RealChip realChip = null;
        private RSoundChip[] scAY8910 = new RSoundChip[2] { null, null };
        private RSoundChip[] scC140 = new RSoundChip[2] { null, null };
        private RSoundChip[] scC352 = new RSoundChip[2] { null, null };
        private RSoundChip[] scDMG = new RSoundChip[2] { null, null };
        private RSoundChip[] scHuC6280 = new RSoundChip[2] { null, null };
        private RSoundChip[] scK051649 = new RSoundChip[2] { null, null };
        private RSoundChip[] scK053260 = new RSoundChip[2] { null, null };
        private RSoundChip[] scNES = new RSoundChip[2] { null, null };
        private RSoundChip[] scVRC6 = new RSoundChip[2] { null, null };
        private RSoundChip[] scPPZ8 = new RSoundChip[2] { null, null };
        private RSoundChip[] scPPSDRV = new RSoundChip[2] { null, null };
        private RSoundChip[] scP86 = new RSoundChip[2] { null, null };
        private RSoundChip[] scQSound = new RSoundChip[2] { null, null };
        private RSoundChip[] scRF5C164 = new RSoundChip[2] { null, null };
        private RSoundChip[] scSEGAPCM = new RSoundChip[2] { null, null };
        private RSoundChip[] scSN76489 = new RSoundChip[2] { null, null };
        private RSoundChip[] scYM2151 = new RSoundChip[2] { null, null };
        private RSoundChip[] scYM2203 = new RSoundChip[2] { null, null };
        private RSoundChip[] scYM2413 = new RSoundChip[2] { null, null };
        private RSoundChip[] scYM2612 = new RSoundChip[2] { null, null };
        private RSoundChip[] scYM2608 = new RSoundChip[2] { null, null };
        private RSoundChip[] scYM2609 = new RSoundChip[2] { null, null };
        private RSoundChip[] scYM2610 = new RSoundChip[2] { null, null };
        private RSoundChip[] scYM2610EA = new RSoundChip[2] { null, null };
        private RSoundChip[] scYM2610EB = new RSoundChip[2] { null, null };
        private RSoundChip[] scYM3526 = new RSoundChip[2] { null, null };
        private RSoundChip[] scY8950 = new RSoundChip[2] { null, null };
        private RSoundChip[] scYM3812 = new RSoundChip[2] { null, null };
        private RSoundChip[] scYMF262 = new RSoundChip[2] { null, null };
        private RSoundChip[] scYMF271 = new RSoundChip[2] { null, null };
        private RSoundChip[] scYMF278B = new RSoundChip[2] { null, null };
        private RSoundChip[] scYMZ280B = new RSoundChip[2] { null, null };

        /// <summary>
        /// AlgMask (Alg:4はop2 op3が逆に並んでいます)
        /// </summary>
        private byte[] algM = new byte[] { 0x08, 0x08, 0x08, 0x08, 0x0c, 0x0e, 0x0e, 0x0f };
        private int[] opN = new int[] { 0, 2, 1, 3 };
        private int[] noteTbl = new int[] { 2, 4, 5, -1, 6, 8, 9, -1, 10, 12, 13, -1, 14, 0, 1, -1 };
        private int[] noteTbl2 = new int[] { 13, 14, 0, -1, 1, 2, 4, -1, 5, 6, 8, -1, 9, 10, 12, -1 };

        private int nsfAPUmask = 0;
        private int nsfDMCmask = 0;
        private int nsfFDSmask = 0;
        private int nsfMMC5mask = 0;
        private int nsfVRC7mask = 0;

        public ChipLEDs chipLED = new ChipLEDs();
        public Enq enq;

        public int[][] YM2151FmRegister = new int[][] { null, null };
        public int[][] YM2151FmKeyOn = new int[][] { null, null };
        public int[][] YM2151FmVol = new int[][] {
            new int[8] { 0,0,0,0,0,0,0,0 }
            , new int[8] { 0,0,0,0,0,0,0,0 }
        };
        private int[] YM2151NowFadeoutVol = new int[] { 0, 0 };
        private bool[][] YM2151MaskFMCh = new bool[][] {
            new bool[8] { false, false, false, false, false, false, false, false }
            ,new bool[8] { false, false, false, false, false, false, false, false }
        };
        public int[] YM2151FmAMD = new int[] { -1, -1 };
        public int[] YM2151FmPMD = new int[] { -1, -1 };

        public int[][] fmRegisterYM2203 = new int[][] { null, null };
        public int[][] fmKeyOnYM2203 = new int[][] { null, null };
        public int[][] fmCh3SlotVolYM2203 = new int[][] { new int[4], new int[4] };
        private int[] nowYM2203FadeoutVol = new int[] { 0, 0 };
        public int[][] fmVolYM2203 = new int[][] { new int[9], new int[9] };
        private bool[][] maskFMChYM2203 = new bool[][] {
            new bool[9] { false, false, false, false, false, false, false, false , false }
            ,new bool[9] { false, false, false, false, false, false, false, false , false }
        };

        public int[][] fmRegisterYM2413 = new int[][] { null, null };
        //private int[] fmRegisterYM2413RyhthmB = new int[2] { 0, 0 };
        //private int[] fmRegisterYM2413Ryhthm = new int[2] { 0, 0 };
        private ChipKeyInfo[] kiYM2413 = new ChipKeyInfo[2] { new ChipKeyInfo(14), new ChipKeyInfo(14) };
        private ChipKeyInfo[] kiYM2413ret = new ChipKeyInfo[2] { new ChipKeyInfo(14), new ChipKeyInfo(14) };
        private bool[][] maskFMChYM2413 = new bool[][] {
            new bool[14] { false, false, false, false, false, false, false, false, false, false, false, false, false, false}
            , new bool[14] { false, false, false, false, false, false, false, false, false, false, false, false, false, false}
        };
        private int[] nowYM2413FadeoutVol = new int[] { 0, 0 };

        public int[][][] fmRegisterYM2612 = new int[][][] { new int[][] { null, null }, new int[][] { null, null } };
        public int[][] fmKeyOnYM2612 = new int[][] { null, null };
        public int[][] fmVolYM2612 = new int[][] {
            new int[9] { 0,0,0,0,0,0,0,0,0 }
            ,new int[9] { 0,0,0,0,0,0,0,0,0 }
        };
        public int[][] fmCh3SlotVolYM2612 = new int[][] { new int[4], new int[4] };
        private int[] nowYM2612FadeoutVol = new int[] { 0, 0 };
        private bool[][] maskFMChYM2612 = new bool[][] { new bool[6] { false, false, false, false, false, false }, new bool[6] { false, false, false, false, false, false } };

        public int[][][] fmRegisterYM2608 = new int[][][] { new int[][] { null, null }, new int[][] { null, null } };
        public int[][] fmKeyOnYM2608 = new int[][] { null, null };
        public int[][] fmVolYM2608 = new int[][] {
            new int[9] { 0,0,0,0,0,0,0,0,0 }
            ,new int[9] { 0,0,0,0,0,0,0,0,0 }
        };
        public int[][] fmCh3SlotVolYM2608 = new int[][] { new int[4], new int[4] };
        public int[][][] fmVolYM2608Rhythm = new int[][][] {
            new int[6][] { new int[2], new int[2], new int[2], new int[2], new int[2], new int[2] }
            , new int[6][] { new int[2], new int[2], new int[2], new int[2], new int[2], new int[2] }
        };
        public int[][] fmVolYM2608Adpcm = new int[][] { new int[2], new int[2] };
        public int[] fmVolYM2608AdpcmPan = new int[] { 0, 0 };

        private int[] nowYM2608FadeoutVol = new int[] { 0, 0 };
        private bool[][] maskFMChYM2608 = new bool[][] {
            new bool[14] { false, false, false, false, false, false, false, false, false, false, false, false, false, false}
            , new bool[14] { false, false, false, false, false, false, false, false, false, false, false, false, false, false}
        };
        /// <summary>
        /// ////////////////////////////////////////////////////////////
        /// </summary>
        //private bool[][] silentVoiceFMChYM2608 = new bool[][] {
        //    new bool[14] { true, false, false, false, false, false, false, false, false, false, false, false, false, false}
        //    , new bool[14] { false, false, false, false, false, false, false, false, false, false, false, false, false, false}
        //};

        public int[][][] fmRegisterYM2610 = new int[][][] { new int[][] { null, null }, new int[][] { null, null } };
        public int[][] fmKeyOnYM2610 = new int[][] { null, null };
        public int[][] fmVolYM2610 = new int[][] {
            new int[9] { 0,0,0,0,0,0,0,0,0 }
            ,new int[9] { 0,0,0,0,0,0,0,0,0 }
        };
        public int[][] fmCh3SlotVolYM2610 = new int[][] { new int[4], new int[4] };
        public int[][][] fmVolYM2610Rhythm = new int[][][] {
            new int[6][] { new int[2], new int[2], new int[2], new int[2], new int[2], new int[2] }
            , new int[6][] { new int[2], new int[2], new int[2], new int[2], new int[2], new int[2] }
        };
        public int[][] fmVolYM2610Adpcm = new int[][] { new int[2], new int[2] };
        public int[] fmVolYM2610AdpcmPan = new int[] { 0, 0 };
        private int[] nowYM2610FadeoutVol = new int[] { 0, 0 };
        private bool[][] maskFMChYM2610 = new bool[][] {
            new bool[14] { false, false, false, false, false, false, false, false, false, false, false, false, false, false }
            , new bool[14] { false, false, false, false, false, false, false, false, false, false, false, false, false, false }
        };

        public int[][] fmRegisterYM3526 = new int[][] { null, null };
        private int[] fmRegisterYM3526FM = new int[] { 0, 0 };
        private int[] fmRegisterYM3526RyhthmB = new int[] { 0, 0 };
        private int[] fmRegisterYM3526Ryhthm = new int[] { 0, 0 };
        private ChipKeyInfo[] kiYM3526 = new ChipKeyInfo[2] { new ChipKeyInfo(14), new ChipKeyInfo(14) };
        private ChipKeyInfo[] kiYM3526ret = new ChipKeyInfo[2] { new ChipKeyInfo(14), new ChipKeyInfo(14) };
        private int[] YM3526NowFadeoutVol = new int[] { 0, 0 };
        private bool[][] maskFMChYM3526 = new bool[2][] {
            new bool[9 + 5]
            {
                false, false, false, false, false, false, false, false, false, false,
                false, false, false, false
            },
            new bool[9 + 5]
            {
                false, false, false, false, false, false, false, false, false, false,
                false, false, false, false
            }
        };

        public int[][] fmRegisterYM3812 = new int[][] { null, null };
        private int[] fmRegisterYM3812FM = new int[] { 0, 0 };
        private int[] fmRegisterYM3812RyhthmB = new int[] { 0, 0 };
        private int[] fmRegisterYM3812Ryhthm = new int[] { 0, 0 };
        private ChipKeyInfo[] kiYM3812 = new ChipKeyInfo[2] { new ChipKeyInfo(14), new ChipKeyInfo(14) };
        private ChipKeyInfo[] kiYM3812ret = new ChipKeyInfo[2] { new ChipKeyInfo(14), new ChipKeyInfo(14) };
        private int[] YM3812NowFadeoutVol = new int[] { 0, 0 };
        private bool[][] maskFMChYM3812 = new bool[2][] {
            new bool[9 + 5]
            {
                false, false, false, false, false, false, false, false, false, false,
                false, false, false, false
            },
            new bool[9 + 5]
            {
                false, false, false, false, false, false, false, false, false, false,
                false, false, false, false
            }
        };

        private ChipKeyInfo[] kiVRC7 = new ChipKeyInfo[2] { new ChipKeyInfo(14), new ChipKeyInfo(14) };
        private ChipKeyInfo[] kiVRC7ret = new ChipKeyInfo[2] { new ChipKeyInfo(14), new ChipKeyInfo(14) };

        public int[][][] fmRegisterYMF262 = new int[][][] { new int[][] { null, null }, new int[][] { null, null } };
        private int[] fmRegisterYMF262FM = new int[2] { 0, 0 };
        private int[] fmRegisterYMF262RyhthmB = new int[2] { 0, 0 };
        private int[] fmRegisterYMF262Ryhthm = new int[2] { 0, 0 };
        private int[] YMF262NowFadeoutVol = new int[] { 0, 0 };
        private bool[][] maskFMChYMF262 = new bool[][] {
            new bool[23] {
                false, false, false, false, false, false, false, false, false, false,
                false, false, false, false, false, false, false, false, false, false,
                false, false, false
                }
            , new bool[23] {
                false, false, false, false, false, false, false, false, false, false,
                false, false, false, false, false, false, false, false, false, false,
                false, false, false
                }
        };

        public int[][][] fmRegisterYMF271 = new int[][][] { new int[][] { null, null }, new int[][] { null, null } };

        public int[][][] fmRegisterYMF278B = new int[][][] { new int[][] { null, null }, new int[][] { null, null } };
        private int[] fmRegisterYMF278BFM = new int[2] { 0, 0 };
        private int[][] fmRegisterYMF278BPCM = new int[2][] { new int[24], new int[24] };
        private int[] fmRegisterYMF278BRyhthmB = new int[2] { 0, 0 };
        private int[] fmRegisterYMF278BRyhthm = new int[2] { 0, 0 };
        private bool[][] maskFMChYMF278B = new bool[][] {
            new bool[47] {
                false, false, false, false, false, false, false, false, false, false,  false, false, false, false, false, false, false, false, false, false,
                false, false, false, false, false, false, false, false, false, false,  false, false, false, false, false, false, false, false, false, false,
                false, false, false, false, false, false, false  }
            , new bool[47] {
                false, false, false, false, false, false, false, false, false, false,  false, false, false, false, false, false, false, false, false, false,
                false, false, false, false, false, false, false, false, false, false,  false, false, false, false, false, false, false, false, false, false,
                false, false, false, false, false, false, false  }
        };
        private byte[] YMF278BCh = new byte[]
        {
                0,3,1,4,2,5,6,7,8,9,12,10,13,11,14,15,16,17,
                18,19,20,21,22,
                23,24,25,26,27,28, 29,30,31,32,33,34,
                35,36,37,38,39,40, 41,42,43,44,45,46
        };
        private int[] YMF278BNowFadeoutVol = new int[] { 0, 0 };

        public int[][] YMZ280BRegister = new int[][] { null, null };

        public int[][] fmRegisterY8950 = new int[][] { null, null };
        private int[] fmRegisterY8950FM = new int[] { 0, 0 };
        private int[] fmRegisterY8950RyhthmB = new int[] { 0, 0 };
        private int[] fmRegisterY8950Ryhthm = new int[] { 0, 0 };
        private int[] Y8950NowFadeoutVol = new int[] { 0, 0 };
        private ChipKeyInfo[] kiY8950 = new ChipKeyInfo[2] { new ChipKeyInfo(15), new ChipKeyInfo(15) };
        private ChipKeyInfo[] kiY8950ret = new ChipKeyInfo[2] { new ChipKeyInfo(15), new ChipKeyInfo(15) };
        private bool[][] maskFMChY8950 = new bool[2][] {
            new bool[9 + 5 + 1]
            {
                false, false, false, false, false, false, false, false, false, false,
                false, false, false, false
                , false
            },
            new bool[9 + 5 + 1]
            {
                false, false, false, false, false, false, false, false, false, false,
                false, false, false, false
                , false
            }
        };

        public int[][] SN76489Register = new int[][] { null, null };
        public int[] SN76489RegisterGGPan = new int[] { 0xff, 0xff };
        public int[][][] SN76489Vol = new int[][][] {
            new int[4][] { new int[2], new int[2], new int[2], new int[2] }
            ,new int[4][] { new int[2], new int[2], new int[2], new int[2] }
        };
        public int[] SN76489NowFadeoutVol = new int[] { 0, 0 };
        public bool[][] SN76489MaskCh = new bool[][] {
            new bool[4] {false,false,false,false }
            ,new bool[4] {false,false,false,false }
        };

        private int[] AY8910ChFreq = new int[] { 0, 0, 0 };
        public int[][] AY8910PsgRegister = new int[][] { null, null };
        public int[][] AY8910PsgKeyOn = new int[][] { null, null };
        private int[] AY8910NowFadeoutVol = new int[] { 0, 0 };
        public int[][] AY8910PsgVol = new int[][] { new int[3], new int[3] };
        private bool[][] AY8910MaskPSGCh = new bool[][] {
            new bool[3] { false, false, false }
            ,new bool[3] { false, false, false }
        };

        private int[] C140NowFadeoutVol = new int[] { 0, 0 };
        private int[] RF5C164NowFadeoutVol = new int[] { 0, 0 };
        private int[] SEGAPCMNowFadeoutVol = new int[] { 0, 0 };
        private int[] HuC6280NowFadeoutVol = new int[] { 0, 0 };
        private int[] K051649NowFadeoutVol = new int[] { 0, 0 };
        private int[] K053260NowFadeoutVol = new int[] { 0, 0 };
        private int[] QSoundNowFadeoutVol = new int[] { 0, 0 };

        private bool[] maskOKIM6258 = new bool[2] { false, false };
        public bool[] okim6258Keyon = new bool[2] { false, false };

        public byte[][] pcmRegisterC140 = new byte[2][] { null, null };
        public bool[][] pcmKeyOnC140 = new bool[2][] { null, null };

        public ushort[][] pcmRegisterC352 = new ushort[2][] { null, null };
        public ushort[][] pcmKeyOnC352 = new ushort[2][] { null, null };
        private bool[][] maskChC352 = new bool[][] {
            new bool[32] {
                false, false, false, false, false, false, false, false,  false, false, false, false, false, false, false, false,
                false, false, false, false, false, false, false, false,  false, false, false, false, false, false, false, false
            }
            ,new bool[32] {
                false, false, false, false, false, false, false, false,  false, false, false, false, false, false, false, false,
                false, false, false, false, false, false, false, false,  false, false, false, false, false, false, false, false
            }
        };

        public byte[] K051649tKeyOnOff = new byte[] { 0, 0 };
        public bool[][] maskChK051649 = new bool[][]
        {
            new bool[]{ false, false, false, false, false},
            new bool[]{ false, false, false, false, false}
        };

        public byte[][] pcmRegisterSEGAPCM = new byte[2][] { null, null };
        public bool[][] pcmKeyOnSEGAPCM = new bool[2][] { null, null };

        public MIDIParam[] midiParams = new MIDIParam[] { null, null };

        public nes_bank nes_bank = null;
        public nes_mem nes_mem = null;
        public km6502 nes_cpu = null;
        public nes_apu nes_apu = null;
        public nes_dmc nes_dmc = null;
        public nes_fds nes_fds = null;
        public nes_n106 nes_n106 = null;
        public nes_vrc6 nes_vrc6 = null;
        public nes_mmc5 nes_mmc5 = null;
        public nes_fme7 nes_fme7 = null;
        public nes_vrc7 nes_vrc7 = null;


        private int[] LatchedRegister = new int[] { 0, 0 };
        private int[] NoiseFreq = new int[] { 0, 0 };

        /// <summary>
        /// ワーク用Chip(Chipはインスタンスの中身だけやりとりされる)
        /// </summary>
        private Chip dummyChip = new Chip(1);

        private int volF = 1;
        //private MIDIExport midiExport = null;

        int[] algVolTbl = new int[8] { 8, 8, 8, 8, 0xa, 0xe, 0xe, 0xf };



        public List<Chip> CONDUCTOR = new List<Chip>();
        public List<Chip> DACControl = new List<Chip>();
        public List<Chip> AY8910 = new List<Chip>();
        public List<Chip> C140 = new List<Chip>();
        public List<Chip> C352 = new List<Chip>();
        public List<Chip> MIDI = new List<Chip>();
        public List<Chip> HuC6280 = new List<Chip>();
        public List<Chip> K051649 = new List<Chip>();
        public List<Chip> RF5C164 = new List<Chip>();
        public List<Chip> DMG = new List<Chip>();
        public List<Chip> NES = new List<Chip>();
        public List<Chip> VRC6 = new List<Chip>();
        public List<Chip> SEGAPCM = new List<Chip>();
        public List<Chip> SN76489 = new List<Chip>();
        public List<Chip> YM2151 = new List<Chip>();
        public List<Chip> YM2203 = new List<Chip>();
        public List<Chip> YM2413 = new List<Chip>();
        public List<Chip> YM3526 = new List<Chip>();
        public List<Chip> Y8950 = new List<Chip>();
        public List<Chip> YM3812 = new List<Chip>();
        public List<Chip> YMF262 = new List<Chip>();
        public List<Chip> YM2608 = new List<Chip>();
        public List<Chip> YM2609 = new List<Chip>();
        public List<Chip> YM2610 = new List<Chip>();
        public List<Chip> YM2612 = new List<Chip>();
        public List<Chip> QSound = new List<Chip>();
        public List<Chip> K053260 = new List<Chip>();
        public List<Chip> PPZ8 = new List<Chip>();
        public List<Chip> PPSDRV = new List<Chip>();
        public List<Chip> P86 = new List<Chip>();
        public List<Chip> YMF278B = new List<Chip>();
        public List<Chip> YMF271 = new List<Chip>();



        public ChipRegister(Setting setting, MDSound.MDSound mds, RealChip nScci)
        {
            ClearChipParam();

            this.mds = mds;
            this.realChip = nScci;
            initChipRegister(null);

            //midiExport = new MIDIExport(setting);
            //midiExport.fmRegisterYM2612 = fmRegisterYM2612;
            //midiExport.fmRegisterYM2151 = YM2151FmRegister;
        }

        public void SetMDSound(MDSound.MDSound mds)
        {
            this.mds = mds;
        }

        public void SetRealChipInfo(EnmZGMDevice dev, Setting.ChipType chipTypeP, Setting.ChipType chipTypeS, int LatencyEmulation, int LatencyReal)
        {
            int LEmu = SoundManager.SoundManager.DATA_SEQUENCE_FREQUENCE * LatencyEmulation / 1000;
            int LReal = SoundManager.SoundManager.DATA_SEQUENCE_FREQUENCE * LatencyReal / 1000;

            switch (dev)
            {
                case EnmZGMDevice.AY8910:
                    ctAY8910 = new Setting.ChipType[] { chipTypeP, chipTypeS };
                    for (int i = 0; i < AY8910.Count; i++)
                    {
                        AY8910[i].Model = EnmVRModel.VirtualModel;
                        AY8910[i].Delay = LEmu;
                        if (i > 1) continue;

                        scAY8910[i] = realChip.GetRealChip(ctAY8910[i]);
                        if (scAY8910[i] != null) scAY8910[i].init();
                        if (AY8910.Count < i + 1) AY8910.Add(new Chip(3));
                        AY8910[i].Model = ctAY8910[i].UseEmu ? EnmVRModel.VirtualModel : EnmVRModel.RealModel;
                        AY8910[i].Delay = (AY8910[i].Model == EnmVRModel.VirtualModel ? LEmu : LReal);
                    }
                    break;
                case EnmZGMDevice.C140:
                    ctC140 = new Setting.ChipType[] { chipTypeP, chipTypeS };
                    for (int i = 0; i < C140.Count; i++)
                    {
                        C140[i].Model = EnmVRModel.VirtualModel;
                        C140[i].Delay = LEmu;
                        if (i > 1) continue;

                        scC140[i] = realChip.GetRealChip(ctC140[i]);
                        if (scC140[i] != null) scC140[i].init();
                        if (C140.Count < i + 1) C140.Add(new Chip(24));
                        C140[i].Model = ctC140[i].UseEmu ? EnmVRModel.VirtualModel : EnmVRModel.RealModel;
                        C140[i].Delay = (C140[i].Model == EnmVRModel.VirtualModel ? LEmu : LReal);
                    }
                    break;
                case EnmZGMDevice.C352:
                    ctC352 = new Setting.ChipType[] { chipTypeP, chipTypeS };
                    for (int i = 0; i < C352.Count; i++)
                    {
                        C352[i].Model = EnmVRModel.VirtualModel;
                        C352[i].Delay = LEmu;
                        if (i > 1) continue;

                        scC352[i] = realChip.GetRealChip(ctC352[i]);
                        if (scC352[i] != null) scC352[i].init();
                        if (C352.Count < i + 1) C352.Add(new Chip(32));
                        C352[i].Model = ctC352[i].UseEmu ? EnmVRModel.VirtualModel : EnmVRModel.RealModel;
                        C352[i].Delay = (C352[i].Model == EnmVRModel.VirtualModel ? LEmu : LReal);
                    }
                    break;
                case EnmZGMDevice.GameBoyDMG:
                    ctDMG = new Setting.ChipType[] { chipTypeP, chipTypeS };
                    for (int i = 0; i < DMG.Count; i++)
                    {
                        DMG[i].Model = EnmVRModel.VirtualModel;
                        DMG[i].Delay = LEmu;
                        if (i > 1) continue;

                        scDMG[i] = realChip.GetRealChip(ctDMG[i]);
                        if (scDMG[i] != null) scDMG[i].init();
                        if (DMG.Count < i + 1) DMG.Add(new Chip(3));
                        DMG[i].Model = ctDMG[i].UseEmu ? EnmVRModel.VirtualModel : EnmVRModel.RealModel;
                        DMG[i].Delay = (DMG[i].Model == EnmVRModel.VirtualModel ? LEmu : LReal);
                    }
                    break;
                case EnmZGMDevice.HuC6280:
                    ctHuC6280 = new Setting.ChipType[] { chipTypeP, chipTypeS };
                    for (int i = 0; i < HuC6280.Count; i++)
                    {
                        HuC6280[i].Model = EnmVRModel.VirtualModel;
                        HuC6280[i].Delay = LEmu;
                        if (i > 1) continue;

                        scHuC6280[i] = null;
                        if (scHuC6280[i] != null) scHuC6280[i].init();
                        if (HuC6280.Count < i + 1) HuC6280.Add(new Chip(6));
                        HuC6280[i].Model = ctHuC6280[i].UseEmu ? EnmVRModel.VirtualModel : EnmVRModel.RealModel;
                        HuC6280[i].Delay = (HuC6280[i].Model == EnmVRModel.VirtualModel ? LEmu : LReal);
                    }
                    break;
                case EnmZGMDevice.K051649:
                    ctK051649 = new Setting.ChipType[] { chipTypeP, chipTypeS };
                    for (int i = 0; i < K051649.Count; i++)
                    {
                        K051649[i].Model = EnmVRModel.VirtualModel;
                        K051649[i].Delay = LEmu;
                        if (i > 1) continue;

                        scK051649[i] = null;
                        if (scK051649[i] != null) scK051649[i].init();
                        if (K051649.Count < i + 1) K051649.Add(new Chip(5));
                        K051649[i].Model = ctK051649[i].UseEmu ? EnmVRModel.VirtualModel : EnmVRModel.RealModel;
                        K051649[i].Delay = (K051649[i].Model == EnmVRModel.VirtualModel ? LEmu : LReal);
                    }
                    break;
                case EnmZGMDevice.K053260:
                    ctK053260 = new Setting.ChipType[] { chipTypeP, chipTypeS };
                    for (int i = 0; i < K053260.Count; i++)
                    {
                        K053260[i].Model = EnmVRModel.VirtualModel;
                        K053260[i].Delay = LEmu;
                        if (i > 1) continue;

                        scK053260[i] = null;
                        if (scK053260[i] != null) scK053260[i].init();
                        if (K053260.Count < i + 1) K053260.Add(new Chip(4));
                        K053260[i].Model = ctK053260[i].UseEmu ? EnmVRModel.VirtualModel : EnmVRModel.RealModel;
                        K053260[i].Delay = (K053260[i].Model == EnmVRModel.VirtualModel ? LEmu : LReal);
                    }
                    break;
                case EnmZGMDevice.NESAPU:
                    ctNES = new Setting.ChipType[] { chipTypeP, chipTypeS };
                    for (int i = 0; i < NES.Count; i++)
                    {
                        NES[i].Model = EnmVRModel.VirtualModel;
                        NES[i].Delay = LEmu;
                        if (i > 1) continue;

                        scNES[i] = realChip.GetRealChip(ctNES[i]);
                        if (scNES[i] != null) scNES[i].init();
                        if (NES.Count < i + 1) NES.Add(new Chip(3));
                        NES[i].Model = ctNES[i].UseEmu ? EnmVRModel.VirtualModel : EnmVRModel.RealModel;
                        NES[i].Delay = (NES[i].Model == EnmVRModel.VirtualModel ? LEmu : LReal);
                    }
                    break;
                case EnmZGMDevice.VRC6:
                    ctVRC6 = new Setting.ChipType[] { chipTypeP, chipTypeS };
                    for (int i = 0; i < VRC6.Count; i++)
                    {
                        VRC6[i].Model = EnmVRModel.VirtualModel;
                        VRC6[i].Delay = LEmu;
                        if (i > 1) continue;

                        scVRC6[i] = realChip.GetRealChip(ctVRC6[i]);
                        if (scVRC6[i] != null) scVRC6[i].init();
                        if (VRC6.Count < i + 1) VRC6.Add(new Chip(3));
                        VRC6[i].Model = ctVRC6[i].UseEmu ? EnmVRModel.VirtualModel : EnmVRModel.RealModel;
                        VRC6[i].Delay = (VRC6[i].Model == EnmVRModel.VirtualModel ? LEmu : LReal);
                    }
                    break;
                case EnmZGMDevice.PPZ8:
                    ctPPZ8 = new Setting.ChipType[] { chipTypeP, chipTypeS };
                    for (int i = 0; i < PPZ8.Count; i++)
                    {
                        PPZ8[i].Model = EnmVRModel.VirtualModel;
                        PPZ8[i].Delay = LEmu;
                        if (i > 1) continue;

                        scPPZ8[i] = null;
                        if (scPPZ8[i] != null) scPPZ8[i].init();
                        if (ctPPZ8[i] != null)
                        {
                            if (PPZ8.Count < i + 1) PPZ8.Add(new Chip(8));
                            PPZ8[i].Model = ctPPZ8[i].UseEmu ? EnmVRModel.VirtualModel : EnmVRModel.RealModel;
                            PPZ8[i].Delay = (PPZ8[i].Model == EnmVRModel.VirtualModel ? LEmu : LReal);
                        }
                    }
                    break;
                case EnmZGMDevice.PPSDRV:
                    ctPPSDRV = new Setting.ChipType[] { chipTypeP, chipTypeS };
                    for (int i = 0; i < PPSDRV.Count; i++)
                    {
                        PPSDRV[i].Model = EnmVRModel.VirtualModel;
                        PPSDRV[i].Delay = LEmu;
                        if (i > 1) continue;

                        scPPSDRV[i] = null;
                        if (scPPSDRV[i] != null) scPPSDRV[i].init();
                        if (ctPPSDRV[i] != null)
                        {
                            if (PPSDRV.Count < i + 1) PPSDRV.Add(new Chip(8));
                            PPSDRV[i].Model = ctPPSDRV[i].UseEmu ? EnmVRModel.VirtualModel : EnmVRModel.RealModel;
                            PPSDRV[i].Delay = (PPSDRV[i].Model == EnmVRModel.VirtualModel ? LEmu : LReal);
                        }
                    }
                    break;
                case EnmZGMDevice.P86:
                    ctP86 = new Setting.ChipType[] { chipTypeP, chipTypeS };
                    for (int i = 0; i < P86.Count; i++)
                    {
                        P86[i].Model = EnmVRModel.VirtualModel;
                        P86[i].Delay = LEmu;
                        if (i > 1) continue;

                        scP86[i] = null;
                        if (scP86[i] != null) scP86[i].init();
                        if (ctP86[i] != null)
                        {
                            if (P86.Count < i + 1) P86.Add(new Chip(8));
                            P86[i].Model = ctP86[i].UseEmu ? EnmVRModel.VirtualModel : EnmVRModel.RealModel;
                            P86[i].Delay = (P86[i].Model == EnmVRModel.VirtualModel ? LEmu : LReal);
                        }
                    }
                    break;
                case EnmZGMDevice.QSound:
                    ctQSound = new Setting.ChipType[] { chipTypeP, chipTypeS };
                    for (int i = 0; i < QSound.Count; i++)
                    {
                        QSound[i].Model = EnmVRModel.VirtualModel;
                        QSound[i].Delay = LEmu;
                        if (i > 1) continue;

                        scQSound[i] = null;
                        if (scQSound[i] != null) scQSound[i].init();
                        if (ctQSound[i] != null)
                        {
                            if (QSound.Count < i + 1) QSound.Add(new Chip(16));
                            QSound[i].Model = ctQSound[i].UseEmu ? EnmVRModel.VirtualModel : EnmVRModel.RealModel;
                            QSound[i].Delay = (QSound[i].Model == EnmVRModel.VirtualModel ? LEmu : LReal);
                        }
                    }
                    break;
                case EnmZGMDevice.RF5C164:
                    ctRF5C164 = new Setting.ChipType[] { chipTypeP, chipTypeS };
                    for (int i = 0; i < RF5C164.Count; i++)
                    {
                        RF5C164[i].Model = EnmVRModel.VirtualModel;
                        RF5C164[i].Delay = LEmu;
                        if (i > 1) continue;

                        scRF5C164[i] = null;
                        if (scRF5C164[i] != null) scRF5C164[i].init();
                        if (RF5C164.Count < i + 1) RF5C164.Add(new Chip(8));
                        RF5C164[i].Model = ctRF5C164[i].UseEmu ? EnmVRModel.VirtualModel : EnmVRModel.RealModel;
                        RF5C164[i].Delay = (RF5C164[i].Model == EnmVRModel.VirtualModel ? LEmu : LReal);
                    }
                    break;
                case EnmZGMDevice.SegaPCM:
                    ctSEGAPCM = new Setting.ChipType[] { chipTypeP, chipTypeS };
                    for (int i = 0; i < SEGAPCM.Count; i++)
                    {
                        SEGAPCM[i].Model = EnmVRModel.VirtualModel;
                        SEGAPCM[i].Delay = LEmu;
                        if (i > 1) continue;

                        scSEGAPCM[i] = realChip.GetRealChip(ctSEGAPCM[i]);
                        if (scSEGAPCM[i] != null) scSEGAPCM[i].init();
                        if (SEGAPCM.Count < i + 1) SEGAPCM.Add(new Chip(16));
                        SEGAPCM[i].Model = ctSEGAPCM[i].UseEmu ? EnmVRModel.VirtualModel : EnmVRModel.RealModel;
                        SEGAPCM[i].Delay = (SEGAPCM[i].Model == EnmVRModel.VirtualModel ? LEmu : LReal);
                    }
                    break;
                case EnmZGMDevice.SN76489:
                    ctSN76489 = new Setting.ChipType[] { chipTypeP, chipTypeS };
                    for (int i = 0; i < SN76489.Count; i++)
                    {
                        SN76489[i].Model = EnmVRModel.VirtualModel;
                        SN76489[i].Delay = LEmu;
                        if (i > 1) continue;

                        scSN76489[i] = realChip.GetRealChip(ctSN76489[i]);
                        if (scSN76489[i] != null) scSN76489[i].init();
                        if (SN76489.Count < i + 1) SN76489.Add(new Chip(4));
                        SN76489[i].Model = (ctSN76489[i].UseEmu || ctSN76489[i].UseEmu2) ? EnmVRModel.VirtualModel : EnmVRModel.RealModel;
                        SN76489[i].Delay = (SN76489[i].Model == EnmVRModel.VirtualModel ? LEmu : LReal);
                    }
                    break;
                case EnmZGMDevice.YM2151:
                    ctYM2151 = new Setting.ChipType[] { chipTypeP, chipTypeS };
                    for (int i = 0; i < YM2151.Count; i++)
                    {
                        YM2151[i].Model = EnmVRModel.VirtualModel;
                        YM2151[i].Delay = LEmu;
                        if (i > 1) continue;

                        scYM2151[i] = realChip.GetRealChip(ctYM2151[i]);
                        if (scYM2151[i] != null) scYM2151[i].init();
                        if (YM2151.Count < i + 1) YM2151.Add(new Chip(8));
                        YM2151[i].Model = (ctYM2151[i].UseEmu || ctYM2151[i].UseEmu2 || ctYM2151[i].UseEmu3) ? EnmVRModel.VirtualModel : EnmVRModel.RealModel;
                        YM2151[i].Delay = (YM2151[i].Model == EnmVRModel.VirtualModel ? LEmu : LReal);
                    }
                    break;
                case EnmZGMDevice.YM2203:
                    ctYM2203 = new Setting.ChipType[] { chipTypeP, chipTypeS };
                    for (int i = 0; i < YM2203.Count; i++)
                    {
                        YM2203[i].Model = EnmVRModel.VirtualModel;
                        YM2203[i].Delay = LEmu;
                        if (i > 1) continue;

                        scYM2203[i] = realChip.GetRealChip(ctYM2203[i]);
                        if (scYM2203[i] != null) scYM2203[i].init();
                        if (YM2203.Count < i + 1) YM2203.Add(new Chip(9));
                        YM2203[i].Model = ctYM2203[i].UseEmu ? EnmVRModel.VirtualModel : EnmVRModel.RealModel;
                        YM2203[i].Delay = (YM2203[i].Model == EnmVRModel.VirtualModel ? LEmu : LReal);

                    }
                    break;
                case EnmZGMDevice.YM2413:
                    ctYM2413 = new Setting.ChipType[] { chipTypeP, chipTypeS };
                    for (int i = 0; i < YM2413.Count; i++)
                    {
                        YM2413[i].Model = EnmVRModel.VirtualModel;
                        YM2413[i].Delay = LEmu;
                        if (i > 1) continue;

                        scYM2413[i] = realChip.GetRealChip(ctYM2413[i]);
                        if (scYM2413[i] != null) scYM2413[i].init();
                        if (YM2413.Count < i + 1) YM2413.Add(new Chip(14));
                        YM2413[i].Model = ctYM2413[i].UseEmu ? EnmVRModel.VirtualModel : EnmVRModel.RealModel;
                        YM2413[i].Delay = (YM2413[i].Model == EnmVRModel.VirtualModel ? LEmu : LReal);
                    }
                    break;
                case EnmZGMDevice.YM3526:
                    ctYM3526 = new Setting.ChipType[] { chipTypeP, chipTypeS };
                    for (int i = 0; i < YM3526.Count; i++)
                    {
                        YM3526[i].Model = EnmVRModel.VirtualModel;
                        YM3526[i].Delay = LEmu;
                        if (i > 1) continue;

                        scYM3526[i] = realChip.GetRealChip(ctYM3526[i]);
                        if (scYM3526[i] != null) scYM3526[i].init();
                        if (YM3526.Count < i + 1) YM3526.Add(new Chip(18 + 5));
                        YM3526[i].Model = ctYM3526[i].UseEmu ? EnmVRModel.VirtualModel : EnmVRModel.RealModel;
                        YM3526[i].Delay = (YM3526[i].Model == EnmVRModel.VirtualModel ? LEmu : LReal);
                    }
                    break;
                case EnmZGMDevice.YM3812:
                    ctYM3812 = new Setting.ChipType[] { chipTypeP, chipTypeS };
                    for (int i = 0; i < YM3812.Count; i++)
                    {
                        YM3812[i].Model = EnmVRModel.VirtualModel;
                        YM3812[i].Delay = LEmu;
                        if (i > 1) continue;

                        scYM3812[i] = realChip.GetRealChip(ctYM3812[i]);
                        if (scYM3812[i] != null) scYM3812[i].init();
                        if (YM3812.Count < i + 1) YM3812.Add(new Chip(18 + 5));
                        YM3812[i].Model = ctYM3812[i].UseEmu ? EnmVRModel.VirtualModel : EnmVRModel.RealModel;
                        YM3812[i].Delay = (YM3812[i].Model == EnmVRModel.VirtualModel ? LEmu : LReal);
                    }
                    break;
                case EnmZGMDevice.YMF262:
                    ctYMF262 = new Setting.ChipType[] { chipTypeP, chipTypeS };
                    for (int i = 0; i < YMF262.Count; i++)
                    {
                        YMF262[i].Model = EnmVRModel.VirtualModel;
                        YMF262[i].Delay = LEmu;
                        if (i > 1) continue;

                        scYMF262[i] = realChip.GetRealChip(ctYMF262[i]);
                        if (scYMF262[i] != null) scYMF262[i].init();
                        if (YMF262.Count < i + 1) YMF262.Add(new Chip(18 + 5));
                        YMF262[i].Model = ctYMF262[i].UseEmu ? EnmVRModel.VirtualModel : EnmVRModel.RealModel;
                        YMF262[i].Delay = (YMF262[i].Model == EnmVRModel.VirtualModel ? LEmu : LReal);
                    }
                    break;
                case EnmZGMDevice.YMF271:
                    ctYMF271 = new Setting.ChipType[] { chipTypeP, chipTypeS };
                    for (int i = 0; i < YMF271.Count; i++)
                    {
                        YMF271[i].Model = EnmVRModel.VirtualModel;
                        YMF271[i].Delay = LEmu;
                        if (i > 1) continue;

                        scYMF271[i] = realChip.GetRealChip(ctYMF271[i]);
                        if (scYMF271[i] != null) scYMF271[i].init();
                        if (YMF271.Count < i + 1) YMF271.Add(new Chip(48));
                        YMF271[i].Model = ctYMF271[i].UseEmu ? EnmVRModel.VirtualModel : EnmVRModel.RealModel;
                        YMF271[i].Delay = (YMF271[i].Model == EnmVRModel.VirtualModel ? LEmu : LReal);
                    }
                    break;
                case EnmZGMDevice.YM2608:
                    ctYM2608 = new Setting.ChipType[] { chipTypeP, chipTypeS };
                    for (int i = 0; i < YM2608.Count; i++)
                    {
                        YM2608[i].Model = EnmVRModel.VirtualModel;
                        YM2608[i].Delay = LEmu;
                        if (i > 1) continue;

                        scYM2608[i] = realChip.GetRealChip(ctYM2608[i]);
                        if (scYM2608[i] != null) scYM2608[i].init();
                        if (YM2608.Count < i + 1) YM2608.Add(new Chip(6 + 3 + 3 + 6 + 1));
                        YM2608[i].Model = ctYM2608[i].UseEmu ? EnmVRModel.VirtualModel : EnmVRModel.RealModel;
                        YM2608[i].Delay = (YM2608[i].Model == EnmVRModel.VirtualModel ? LEmu : LReal);
                    }
                    break;
                case EnmZGMDevice.YM2609:
                    ctYM2609 = new Setting.ChipType[] { chipTypeP, chipTypeS };
                    for (int i = 0; i < YM2609.Count; i++)
                    {
                        YM2609[i].Model = EnmVRModel.VirtualModel;
                        YM2609[i].Delay = LEmu;
                        if (i > 1) continue;

                        scYM2609[i] = realChip.GetRealChip(ctYM2609[i]);
                        if (scYM2609[i] != null) scYM2609[i].init();
                        if (YM2609.Count < i + 1) YM2609.Add(new Chip(12 + 6 + 12 + 6 + 3 + 6));
                        YM2609[i].Model = ctYM2609[i].UseEmu ? EnmVRModel.VirtualModel : EnmVRModel.RealModel;
                        YM2609[i].Delay = (YM2609[i].Model == EnmVRModel.VirtualModel ? LEmu : LReal);
                    }
                    break;
                case EnmZGMDevice.YM2610:
                    ctYM2610 = new Setting.ChipType[] { chipTypeP, chipTypeS };
                    for (int i = 0; i < YM2610.Count; i++)
                    {
                        YM2610[i].Model = EnmVRModel.VirtualModel;
                        YM2610[i].Delay = LEmu;
                        if (i > 1) continue;

                        scYM2610[i] = realChip.GetRealChip(ctYM2610[i]);
                        if (scYM2610[i] != null) scYM2610[i].init();
                        scYM2610EA[i] = realChip.GetRealChip(ctYM2610[i], 1);
                        if (scYM2610EA[i] != null) scYM2610EA[i].init();
                        scYM2610EB[i] = realChip.GetRealChip(ctYM2610[i], 2);
                        if (scYM2610EB[i] != null) scYM2610EB[i].init();
                        if (YM2610.Count < i + 1) YM2610.Add(new Chip(6 + 3 + 3 + 6 + 1));
                        YM2610[i].Model = ctYM2610[i].UseEmu ? EnmVRModel.VirtualModel : EnmVRModel.RealModel;
                        YM2610[i].Delay = (YM2610[i].Model == EnmVRModel.VirtualModel ? LEmu : LReal);
                    }
                    break;
                case EnmZGMDevice.YM2612:
                    ctYM2612 = new Setting.ChipType[] { chipTypeP, chipTypeS };
                    for (int i = 0; i < YM2612.Count; i++)
                    {
                        YM2612[i].Model = EnmVRModel.VirtualModel;
                        YM2612[i].Delay = LEmu;
                        if (i > 1) continue;

                        scYM2612[i] = realChip.GetRealChip(ctYM2612[i]);
                        if (scYM2612[i] != null) scYM2612[i].init();
                        if (YM2612.Count < i + 1) YM2612.Add(new Chip(6 + 3));
                        YM2612[i].Model = ctYM2612[i].UseScci ? EnmVRModel.RealModel : EnmVRModel.VirtualModel;
                        YM2612[i].Delay = (YM2612[i].Model == EnmVRModel.VirtualModel ? LEmu : LReal);
                    }
                    break;

                case EnmZGMDevice.Y8950:
                    ctY8950 = new Setting.ChipType[] { chipTypeP, chipTypeS };
                    for (int i = 0; i < Y8950.Count; i++)
                    {
                        Y8950[i].Model = EnmVRModel.VirtualModel;
                        Y8950[i].Delay = LEmu;
                        if (i > 1) continue;

                        scY8950[i] = realChip.GetRealChip(ctY8950[i]);
                        if (scY8950[i] != null) scY8950[i].init();
                        if (Y8950.Count < i + 1) Y8950.Add(new Chip(15));
                        Y8950[i].Model = ctY8950[i].UseEmu ? EnmVRModel.VirtualModel : EnmVRModel.RealModel;
                        Y8950[i].Delay = (Y8950[i].Model == EnmVRModel.VirtualModel ? LEmu : LReal);
                    }
                    break;
                case EnmZGMDevice.YMF278B:
                    ctYMF278B = new Setting.ChipType[] { chipTypeP, chipTypeS };
                    break;
                case EnmZGMDevice.YMZ280B:
                    ctYMZ280B = new Setting.ChipType[] { chipTypeP, chipTypeS };
                    break;
                default:
                    throw new NotImplementedException();
            }

        }

        public void ClearChipParam()
        {

            CONDUCTOR.Clear();
            DACControl.Clear();

            for (int i = 0; i < 2; i++)
            {
                if (CONDUCTOR.Count < i + 1) CONDUCTOR.Add(new Chip(2));
                CONDUCTOR[i].Use = false;
                CONDUCTOR[i].Model = EnmVRModel.None;
                CONDUCTOR[i].Device = EnmZGMDevice.Conductor;
                CONDUCTOR[i].Number = i;
                CONDUCTOR[i].Hosei = 0;

                if (DACControl.Count < i + 1) DACControl.Add(new Chip(2));
                DACControl[i].Use = false;
                DACControl[i].Model = EnmVRModel.None;
                DACControl[i].Device = EnmZGMDevice.DACControl;
                DACControl[i].Number = i;
                DACControl[i].Hosei = 0;

                if (AY8910.Count < i + 1) AY8910.Add(new Chip(3));
                AY8910[i].Use = false;
                AY8910[i].Model = EnmVRModel.None;
                AY8910[i].Device = EnmZGMDevice.AY8910;
                AY8910[i].Number = i;
                AY8910[i].Hosei = 0;

                if (C140.Count < i + 1) C140.Add(new Chip(24));
                C140[i].Use = false;
                C140[i].Model = EnmVRModel.None;
                C140[i].Device = EnmZGMDevice.C140;
                C140[i].Number = i;
                C140[i].Hosei = 0;

                if (C352.Count < i + 1) C352.Add(new Chip(32));
                C352[i].Use = false;
                C352[i].Model = EnmVRModel.None;
                C352[i].Device = EnmZGMDevice.C352;
                C352[i].Number = i;
                C352[i].Hosei = 0;

                if (DMG.Count < i + 1) DMG.Add(new Chip(4));
                DMG[i].Use = false;
                DMG[i].Model = EnmVRModel.None;
                DMG[i].Device = EnmZGMDevice.GameBoyDMG;
                DMG[i].Number = i;
                DMG[i].Hosei = 0;

                if (HuC6280.Count < i + 1) HuC6280.Add(new Chip(6));
                HuC6280[i].Use = false;
                HuC6280[i].Model = EnmVRModel.None;
                HuC6280[i].Device = EnmZGMDevice.HuC6280;
                HuC6280[i].Number = i;
                HuC6280[i].Hosei = 0;

                if (K051649.Count < i + 1) K051649.Add(new Chip(5));
                K051649[i].Use = false;
                K051649[i].Model = EnmVRModel.None;
                K051649[i].Device = EnmZGMDevice.K051649;
                K051649[i].Number = i;
                K051649[i].Hosei = 0;

                if (K053260.Count < i + 1) K053260.Add(new Chip(4));
                K053260[i].Use = false;
                K053260[i].Model = EnmVRModel.None;
                K053260[i].Device = EnmZGMDevice.K053260;
                K053260[i].Number = i;
                K053260[i].Hosei = 0;

                if (NES.Count < i + 1) NES.Add(new Chip(5));
                NES[i].Use = false;
                NES[i].Model = EnmVRModel.None;
                NES[i].Device = EnmZGMDevice.NESAPU;
                NES[i].Number = i;
                NES[i].Hosei = 0;

                if (VRC6.Count < i + 1) VRC6.Add(new Chip(3));
                VRC6[i].Use = false;
                VRC6[i].Model = EnmVRModel.None;
                VRC6[i].Device = EnmZGMDevice.VRC6;
                VRC6[i].Number = i;
                VRC6[i].Hosei = 0;

                if (PPSDRV.Count < i + 1) PPSDRV.Add(new Chip(1));
                PPSDRV[i].Use = false;
                PPSDRV[i].Model = EnmVRModel.None;
                PPSDRV[i].Device = EnmZGMDevice.PPSDRV;
                PPSDRV[i].Number = i;
                PPSDRV[i].Hosei = 0;

                if (PPZ8.Count < i + 1) PPZ8.Add(new Chip(8));
                PPZ8[i].Use = false;
                PPZ8[i].Model = EnmVRModel.None;
                PPZ8[i].Device = EnmZGMDevice.PPZ8;
                PPZ8[i].Number = i;
                PPZ8[i].Hosei = 0;

                if (P86.Count < i + 1) P86.Add(new Chip(1));
                P86[i].Use = false;
                P86[i].Model = EnmVRModel.None;
                P86[i].Device = EnmZGMDevice.P86;
                P86[i].Number = i;
                P86[i].Hosei = 0;

                if (QSound.Count < i + 1) QSound.Add(new Chip(16));
                QSound[i].Use = false;
                QSound[i].Model = EnmVRModel.None;
                QSound[i].Device = EnmZGMDevice.QSound;
                QSound[i].Number = i;
                QSound[i].Hosei = 0;

                if (RF5C164.Count < i + 1) RF5C164.Add(new Chip(8));
                RF5C164[i].Use = false;
                RF5C164[i].Model = EnmVRModel.None;
                RF5C164[i].Device = EnmZGMDevice.RF5C164;
                RF5C164[i].Number = i;
                RF5C164[i].Hosei = 0;

                if (SEGAPCM.Count < i + 1) SEGAPCM.Add(new Chip(16));
                SEGAPCM[i].Use = false;
                SEGAPCM[i].Model = EnmVRModel.None;
                SEGAPCM[i].Device = EnmZGMDevice.SegaPCM;
                SEGAPCM[i].Number = i;
                SEGAPCM[i].Hosei = 0;

                if (YM2151.Count < i + 1) YM2151.Add(new Chip(8));
                YM2151[i].Use = false;
                YM2151[i].Model = EnmVRModel.None;
                YM2151[i].Device = EnmZGMDevice.YM2151;
                YM2151[i].Number = i;
                YM2151[i].Hosei = 0;

                if (YM2203.Count < i + 1) YM2203.Add(new Chip(9));
                YM2203[i].Use = false;
                YM2203[i].Model = EnmVRModel.None;
                YM2203[i].Device = EnmZGMDevice.YM2203;
                YM2203[i].Number = i;
                YM2203[i].Hosei = 0;

                if (YM2413.Count < i + 1) YM2413.Add(new Chip(14));
                YM2413[i].Use = false;
                YM2413[i].Model = EnmVRModel.None;
                YM2413[i].Device = EnmZGMDevice.YM2413;
                YM2413[i].Number = i;
                YM2413[i].Hosei = 0;

                if (YM3526.Count < i + 1) YM3526.Add(new Chip(9 + 5));
                YM3526[i].Use = false;
                YM3526[i].Model = EnmVRModel.None;
                YM3526[i].Device = EnmZGMDevice.YM3526;
                YM3526[i].Number = i;
                YM3526[i].Hosei = 0;

                if (Y8950.Count < i + 1) Y8950.Add(new Chip(9 + 5 + 1));
                Y8950[i].Use = false;
                Y8950[i].Model = EnmVRModel.None;
                Y8950[i].Device = EnmZGMDevice.Y8950;
                Y8950[i].Number = i;
                Y8950[i].Hosei = 0;

                if (YM3812.Count < i + 1) YM3812.Add(new Chip(9 + 5));
                YM3812[i].Use = false;
                YM3812[i].Model = EnmVRModel.None;
                YM3812[i].Device = EnmZGMDevice.YM3812;
                YM3812[i].Number = i;
                YM3812[i].Hosei = 0;

                if (YMF262.Count < i + 1) YMF262.Add(new Chip(18 + 5));
                YMF262[i].Use = false;
                YMF262[i].Model = EnmVRModel.None;
                YMF262[i].Device = EnmZGMDevice.YMF262;
                YMF262[i].Number = i;
                YMF262[i].Hosei = 0;

                if (YMF278B.Count < i + 1) YMF278B.Add(new Chip(18 + 5 + 24));
                YMF278B[i].Use = false;
                YMF278B[i].Model = EnmVRModel.None;
                YMF278B[i].Device = EnmZGMDevice.YMF278B;
                YMF278B[i].Number = i;
                YMF278B[i].Hosei = 0;

                if (YMF271.Count < i + 1) YMF271.Add(new Chip(18 + 5 + 24));
                YMF271[i].Use = false;
                YMF271[i].Model = EnmVRModel.None;
                YMF271[i].Device = EnmZGMDevice.YMF271;
                YMF271[i].Number = i;
                YMF271[i].Hosei = 0;

                if (YM2608.Count < i + 1) YM2608.Add(new Chip(6 + 3 + 3 + 6 + 1));
                YM2608[i].Use = false;
                YM2608[i].Model = EnmVRModel.None;
                YM2608[i].Device = EnmZGMDevice.YM2608;
                YM2608[i].Number = i;
                YM2608[i].Hosei = 0;

                if (YM2609.Count < i + 1) YM2609.Add(new Chip(12 + 6 + 12 + 6 + 3 + 6));
                YM2609[i].Use = false;
                YM2609[i].Model = EnmVRModel.None;
                YM2609[i].Device = EnmZGMDevice.YM2609;
                YM2609[i].Number = i;
                YM2609[i].Hosei = 0;

                if (YM2610.Count < i + 1) YM2610.Add(new Chip(6 + 3 + 3 + 6 + 1));
                YM2610[i].Use = false;
                YM2610[i].Model = EnmVRModel.None;
                YM2610[i].Device = EnmZGMDevice.YM2610;
                YM2610[i].Number = i;
                YM2610[i].Hosei = 0;

                if (YM2612.Count < i + 1) YM2612.Add(new Chip(6 + 3));
                YM2612[i].Use = false;
                YM2612[i].Model = EnmVRModel.None;
                YM2612[i].Device = EnmZGMDevice.YM2612;
                YM2612[i].Number = i;
                YM2612[i].Hosei = 0;

                if (SN76489.Count < i + 1) SN76489.Add(new Chip(4));
                SN76489[i].Use = false;
                SN76489[i].Model = EnmVRModel.None;
                SN76489[i].Device = EnmZGMDevice.SN76489;
                SN76489[i].Number = i;
                SN76489[i].Hosei = 0;
            }

            if (MIDI.Count < 1) MIDI.Add(new Chip(100));
            MIDI[0].Use = false;
            MIDI[0].Model = EnmVRModel.None;
            MIDI[0].Device = EnmZGMDevice.MIDIGM;
            MIDI[0].Number = 0;
            MIDI[0].Hosei = 0;
        }

        public void ZgmSetup(List<Chip> chips)
        {
            CONDUCTOR.Clear();
            C140.Clear();
            C352.Clear();
            HuC6280.Clear();
            K051649.Clear();
            K053260.Clear();
            QSound.Clear();
            RF5C164.Clear();
            AY8910.Clear();
            DMG.Clear();
            NES.Clear();
            VRC6.Clear();
            SEGAPCM.Clear();
            SN76489.Clear();
            YM2151.Clear();
            YM2203.Clear();
            YM2413.Clear();
            YM3526.Clear();
            Y8950.Clear();
            YM3812.Clear();
            YMF262.Clear();
            YMF278B.Clear();
            YMF271.Clear();
            YM2608.Clear();
            YM2609.Clear();
            YM2610.Clear();
            YM2612.Clear();
            MIDI.Clear();

            foreach (Chip c in chips)
            {
                if (c is Driver.ZGM.ZgmChip.Conductor) CONDUCTOR.Add(c);
                if (c is Driver.ZGM.ZgmChip.SN76489) SN76489.Add(c);
                if (c is Driver.ZGM.ZgmChip.YM2413) YM2413.Add(c);
                if (c is Driver.ZGM.ZgmChip.YM2612) YM2612.Add(c);
                if (c is Driver.ZGM.ZgmChip.YM2151) YM2151.Add(c);
                if (c is Driver.ZGM.ZgmChip.SEGAPCM) SEGAPCM.Add(c);
                if (c is Driver.ZGM.ZgmChip.YM2203) YM2203.Add(c);
                if (c is Driver.ZGM.ZgmChip.YM2608) YM2608.Add(c);
                if (c is Driver.ZGM.ZgmChip.YM2610) YM2610.Add(c);
                if (c is Driver.ZGM.ZgmChip.YM3812) YM3812.Add(c);
                if (c is Driver.ZGM.ZgmChip.YM3526) YM3526.Add(c);
                if (c is Driver.ZGM.ZgmChip.Y8950) Y8950.Add(c);
                if (c is Driver.ZGM.ZgmChip.YMF262) YMF262.Add(c);
                if (c is Driver.ZGM.ZgmChip.YMF278B) YMF278B.Add(c);
                if (c is Driver.ZGM.ZgmChip.YMF271) YMF271.Add(c);
                if (c is Driver.ZGM.ZgmChip.RF5C164) RF5C164.Add(c);
                if (c is Driver.ZGM.ZgmChip.AY8910) AY8910.Add(c);
                if (c is Driver.ZGM.ZgmChip.DMG) DMG.Add(c);
                if (c is Driver.ZGM.ZgmChip.NES) NES.Add(c);
                if (c is Driver.ZGM.ZgmChip.VRC6) VRC6.Add(c);
                if (c is Driver.ZGM.ZgmChip.K051649) K051649.Add(c);
                if (c is Driver.ZGM.ZgmChip.HuC6280) HuC6280.Add(c);
                if (c is Driver.ZGM.ZgmChip.C140) C140.Add(c);
                if (c is Driver.ZGM.ZgmChip.K053260) K053260.Add(c);
                if (c is Driver.ZGM.ZgmChip.QSound) QSound.Add(c);
                if (c is Driver.ZGM.ZgmChip.C352) C352.Add(c);

                if (c is Driver.ZGM.ZgmChip.YM2609) YM2609.Add(c);
                if (c is Driver.ZGM.ZgmChip.MidiGM) MIDI.Add(c);
            }
        }

        public bool ProcessingData(ref outDatum od, ref long Counter, ref Chip Chip, ref EnmDataType Type, ref int Address, ref int Data, ref object ExData)
        {
            if (Type != EnmDataType.Normal) return true;

            switch (Chip.Device)
            {
                case EnmZGMDevice.DACControl:
                    DACControlSetRegisterProcessing(ref Counter, ref Chip, ref Type, ref Address, ref Data, ref ExData);
                    break;
                case EnmZGMDevice.AY8910:
                    AY8910SetRegisterProcessing(ref Counter, ref Chip, ref Type, ref Address, ref Data, ref ExData);
                    break;
                case EnmZGMDevice.C140:
                    C140SetRegisterProcessing(ref Counter, ref Chip, ref Type, ref Address, ref Data, ref ExData);
                    break;
                case EnmZGMDevice.C352:
                    C352SetRegisterProcessing(ref Counter, ref Chip, ref Type, ref Address, ref Data, ref ExData);
                    break;
                case EnmZGMDevice.GameBoyDMG:
                    DMGSetRegisterProcessing(ref Counter, ref Chip, ref Type, ref Address, ref Data, ref ExData);
                    break;
                case EnmZGMDevice.HuC6280:
                    HuC6280SetRegisterProcessing(ref Counter, ref Chip, ref Type, ref Address, ref Data, ref ExData);
                    break;
                case EnmZGMDevice.K051649:
                    K051649SetRegisterProcessing(ref Counter, ref Chip, ref Type, ref Address, ref Data, ref ExData);
                    break;
                case EnmZGMDevice.K053260:
                    K053260SetRegisterProcessing(ref Counter, ref Chip, ref Type, ref Address, ref Data, ref ExData);
                    break;
                case EnmZGMDevice.NESAPU:
                    NESSetRegisterProcessing(ref Counter, ref Chip, ref Type, ref Address, ref Data, ref ExData);
                    break;
                case EnmZGMDevice.VRC6:
                    VRC6SetRegisterProcessing(ref Counter, ref Chip, ref Type, ref Address, ref Data, ref ExData);
                    break;
                case EnmZGMDevice.QSound:
                    QSoundSetRegisterProcessing(ref Counter, ref Chip, ref Type, ref Address, ref Data, ref ExData);
                    break;
                case EnmZGMDevice.RF5C164:
                    RF5C164SetRegisterProcessing(ref Counter, ref Chip, ref Type, ref Address, ref Data, ref ExData);
                    break;
                case EnmZGMDevice.SegaPCM:
                    SEGAPCMSetRegisterProcessing(ref Counter, ref Chip, ref Type, ref Address, ref Data, ref ExData);
                    break;
                case EnmZGMDevice.SN76489:
                    SN76489SetRegisterProcessing(ref Counter, ref Chip, ref Type, ref Address, ref Data, ref ExData);
                    break;
                case EnmZGMDevice.YM2151:
                    YM2151SetRegisterProcessing(ref Counter, ref Chip, ref Type, ref Address, ref Data, ref ExData);
                    break;
                case EnmZGMDevice.YM2203:
                    YM2203SetRegisterProcessing(ref Counter, ref Chip, ref Type, ref Address, ref Data, ref ExData);
                    break;
                case EnmZGMDevice.YM2413:
                    YM2413SetRegisterProcessing(ref Counter, ref Chip, ref Type, ref Address, ref Data, ref ExData);
                    break;
                case EnmZGMDevice.YM3526:
                    YM3526SetRegisterProcessing(ref Counter, ref Chip, ref Type, ref Address, ref Data, ref ExData);
                    break;
                case EnmZGMDevice.Y8950:
                    Y8950SetRegisterProcessing(ref Counter, ref Chip, ref Type, ref Address, ref Data, ref ExData);
                    break;
                case EnmZGMDevice.YM3812:
                    YM3812SetRegisterProcessing(ref Counter, ref Chip, ref Type, ref Address, ref Data, ref ExData);
                    break;
                case EnmZGMDevice.YMF262:
                    YMF262SetRegisterProcessing(ref Counter, ref Chip, ref Type, ref Address, ref Data, ref ExData);
                    break;
                case EnmZGMDevice.YMF278B:
                    YMF278BSetRegisterProcessing(ref Counter, ref Chip, ref Type, ref Address, ref Data, ref ExData);
                    break;
                case EnmZGMDevice.YMF271:
                    YMF271SetRegisterProcessing(ref Counter, ref Chip, ref Type, ref Address, ref Data, ref ExData);
                    break;
                case EnmZGMDevice.YM2608:
                    YM2608SetRegisterProcessing(ref Counter, ref Chip, ref Type, ref Address, ref Data, ref ExData);
                    break;
                case EnmZGMDevice.YM2609:
                    YM2609SetRegisterProcessing(ref od, ref Counter, ref Chip, ref Type, ref Address, ref Data, ref ExData);
                    break;
                case EnmZGMDevice.YM2610:
                    YM2610SetRegisterProcessing(ref Counter, ref Chip, ref Type, ref Address, ref Data, ref ExData);
                    break;
                case EnmZGMDevice.YM2612:
                    YM2612SetRegisterProcessing(ref Counter, ref Chip, ref Type, ref Address, ref Data, ref ExData);
                    break;
                case EnmZGMDevice.MIDIGM:
                    MidiGMSetRegisterProcessing(ref Counter, ref Chip, ref Type, ref Address, ref Data, ref ExData);
                    break;
                case EnmZGMDevice.None:
                    //Dummy Command
                    break;
                case EnmZGMDevice.PPZ8:
                    PPZ8SetRegisterProcessing(ref Counter, ref Chip, ref Type, ref Address, ref Data, ref ExData);
                    break;
                case EnmZGMDevice.PPSDRV:
                    PPSDRVSetRegisterProcessing(ref Counter, ref Chip, ref Type, ref Address, ref Data, ref ExData);
                    break;
                case EnmZGMDevice.P86:
                    P86SetRegisterProcessing(ref Counter, ref Chip, ref Type, ref Address, ref Data, ref ExData);
                    break;
                default:
                    throw new ArgumentException();
            }

            return true;
        }

        public void SendChipData(long packCounter, Chip Chip, EnmDataType type, int address, int data, object exData)
        {

#if DEBUG
            //特定のデータを送るタイミングに疑いがあるとき、以下のコメントアウトを有効にしデバッグする
            //Console.WriteLine("packCounter:{0}",packCounter);
#endif 

            switch (Chip.Device)
            {
                case EnmZGMDevice.DACControl:
                    DACControlWriteRegisterControl(Chip, type, address, data, exData);
                    break;
                case EnmZGMDevice.AY8910:
                    AY8910WriteRegisterControl(Chip, type, address, data, exData);
                    break;
                case EnmZGMDevice.C140:
                    C140WriteRegisterControl(Chip, type, address, data, exData);
                    break;
                case EnmZGMDevice.C352:
                    C352WriteRegisterControl(Chip, type, address, data, exData);
                    break;
                case EnmZGMDevice.GameBoyDMG:
                    DMGWriteRegisterControl(Chip, type, address, data, exData);
                    break;
                case EnmZGMDevice.MIDIGM:
                    MidiGMWriteRegisterControl(Chip, type, address, data, exData);
                    break;
                case EnmZGMDevice.HuC6280:
                    HuC6280WriteRegisterControl(Chip, type, address, data, exData);
                    break;
                case EnmZGMDevice.K051649:
                    K051649WriteRegisterControl(Chip, type, address, data, exData);
                    break;
                case EnmZGMDevice.K053260:
                    K053260WriteRegisterControl(Chip, type, address, data, exData);
                    break;
                case EnmZGMDevice.NESAPU:
                    NESWriteRegisterControl(Chip, type, address, data, exData);
                    break;
                case EnmZGMDevice.VRC6:
                    VRC6WriteRegisterControl(Chip, type, address, data, exData);
                    break;
                case EnmZGMDevice.PPZ8:
                    PPZ8WriteRegisterControl(Chip, type, address, data, exData);
                    break;
                case EnmZGMDevice.PPSDRV:
                    PPSDRVWriteRegisterControl(Chip, type, address, data, exData);
                    break;
                case EnmZGMDevice.P86:
                    P86WriteRegisterControl(Chip, type, address, data, exData);
                    break;
                case EnmZGMDevice.QSound:
                    QSoundWriteRegisterControl(Chip, type, address, data, exData);
                    break;
                case EnmZGMDevice.RF5C164:
                    RF5C164WriteRegisterControl(Chip, type, address, data, exData);
                    break;
                case EnmZGMDevice.SegaPCM:
                    SEGAPCMWriteRegisterControl(Chip, type, address, data, exData);
                    break;
                case EnmZGMDevice.YM2151:
                    YM2151WriteRegisterControl(Chip, type, address, data, exData);
                    break;
                case EnmZGMDevice.YM2203:
                    YM2203WriteRegisterControl(Chip, type, address, data, exData);
                    break;
                case EnmZGMDevice.YM2413:
                    YM2413WriteRegisterControl(Chip, type, address, data, exData);
                    break;
                case EnmZGMDevice.YM3526:
                    YM3526WriteRegisterControl(Chip, type, address, data, exData);
                    break;
                case EnmZGMDevice.Y8950:
                    Y8950WriteRegisterControl(Chip, type, address, data, exData);
                    break;
                case EnmZGMDevice.YM3812:
                    YM3812WriteRegisterControl(Chip, type, address, data, exData);
                    break;
                case EnmZGMDevice.YMF262:
                    YMF262WriteRegisterControl(Chip, type, address, data, exData);
                    break;
                case EnmZGMDevice.YMF271:
                    YMF271WriteRegisterControl(Chip, type, address, data, exData);
                    break;
                case EnmZGMDevice.YMF278B:
                    YMF278BWriteRegisterControl(Chip, type, address, data, exData);
                    break;
                case EnmZGMDevice.YM2608:
                    YM2608WriteRegisterControl(Chip, type, address, data, exData);
                    break;
                case EnmZGMDevice.YM2609:
                    YM2609WriteRegisterControl(Chip, type, address, data, exData);
                    break;
                case EnmZGMDevice.YM2610:
                    YM2610WriteRegisterControl(Chip, type, address, data, exData);
                    break;
                case EnmZGMDevice.YM2612:
                    YM2612WriteRegisterControl(Chip, type, address, data, exData);
                    break;
                case EnmZGMDevice.SN76489:
                    SN76489WriteRegisterControl(Chip, type, address, data, exData);
                    break;
                case EnmZGMDevice.None:
                    //Dummy Command
                    break;
                default:
                    throw new ArgumentException();
            }
        }

        public void SetFadeoutVolume(long counter, double fadeoutCounter)
        {
            AY8910SetFadeoutVolume(counter, (int)((1.0 - fadeoutCounter) * 15.0));
            C140SetFadeoutVolume(counter, (int)((1.0 - fadeoutCounter) * 255.0));
            DMGSetFadeoutVolume(counter, (int)((1.0 - fadeoutCounter) * 15.0));
            HuC6280SetFadeoutVolume(counter, (int)((1.0 - fadeoutCounter) * 127.0));
            K051649SetFadeoutVolume(counter, (int)((1.0 - fadeoutCounter) * 15.0));
            K053260SetFadeoutVolume(counter, (int)((1.0 - fadeoutCounter) * 127.0));
            NESSetFadeoutVolume(counter, (int)((1.0 - fadeoutCounter) * 15.0));
            QSoundSetFadeoutVolume(counter, (int)((1.0 - fadeoutCounter) * 65535.0));
            RF5C164SetFadeoutVolume(counter, (int)((1.0 - fadeoutCounter) * 255.0));
            SEGAPCMSetFadeoutVolume(counter, (int)((1.0 - fadeoutCounter) * 255.0));
            SN76489SetFadeoutVolume(counter, (int)((1.0 - fadeoutCounter) * 15.0));
            YM2151SetFadeoutVolume(counter, (int)((1.0 - fadeoutCounter) * 127.0));
            YM2203SetFadeoutVolume(counter, (int)((1.0 - fadeoutCounter) * 127.0));
            YM2413SetFadeoutVolume(counter, (int)((1.0 - fadeoutCounter) * 15.0));
            YM3526SetFadeoutVolume(counter, (int)((1.0 - fadeoutCounter) * 63.0));
            Y8950SetFadeoutVolume(counter, (int)((1.0 - fadeoutCounter) * 63.0));
            YM3812SetFadeoutVolume(counter, (int)((1.0 - fadeoutCounter) * 63.0));
            YMF262SetFadeoutVolume(counter, (int)((1.0 - fadeoutCounter) * 63.0));
            YMF278BSetFadeoutVolume(counter, (int)((1.0 - fadeoutCounter) * 63.0));
            YM2608SetFadeoutVolume(counter, (int)((1.0 - fadeoutCounter) * 127.0));
            YM2610SetFadeoutVolume(counter, (int)((1.0 - fadeoutCounter) * 127.0));
            YM2612SetFadeoutVolume(counter, (int)((1.0 - fadeoutCounter) * 127.0));
        }

        public void LoopCountUp(long Counter)
        {
            dummyChip.Model = EnmVRModel.None;
            enq(null, Counter, dummyChip, EnmDataType.Loop, -1, -1, null);
        }

        public void initChipRegister(MDSound.MDSound.Chip[] chipInfos)
        {

            dicChipsInfo.Clear();
            if (chipInfos != null)
            {
                foreach (MDSound.MDSound.Chip c in chipInfos)
                {
                    if (!dicChipsInfo.ContainsKey(c.type))
                    {
                        dicChipsInfo.Add(c.type, c);
                    }
                }
            }

            for (int chipID = 0; chipID < 2; chipID++)
            {

                fmRegisterYM2612[chipID] = new int[2][] { new int[0x100], new int[0x100] };
                for (int i = 0; i < 0x100; i++)
                {
                    fmRegisterYM2612[chipID][0][i] = 0;
                    fmRegisterYM2612[chipID][1][i] = 0;
                }
                fmRegisterYM2612[chipID][0][0xb4] = 0xc0;
                fmRegisterYM2612[chipID][0][0xb5] = 0xc0;
                fmRegisterYM2612[chipID][0][0xb6] = 0xc0;
                fmRegisterYM2612[chipID][1][0xb4] = 0xc0;
                fmRegisterYM2612[chipID][1][0xb5] = 0xc0;
                fmRegisterYM2612[chipID][1][0xb6] = 0xc0;
                fmKeyOnYM2612[chipID] = new int[6] { 0, 0, 0, 0, 0, 0 };

                fmRegisterYM2608[chipID] = new int[2][] { new int[0x100], new int[0x100] };
                for (int i = 0; i < 0x100; i++)
                {
                    fmRegisterYM2608[chipID][0][i] = 0;
                    fmRegisterYM2608[chipID][1][i] = 0;
                }
                fmRegisterYM2608[chipID][0][0xb4] = 0xc0;
                fmRegisterYM2608[chipID][0][0xb5] = 0xc0;
                fmRegisterYM2608[chipID][0][0xb6] = 0xc0;
                fmRegisterYM2608[chipID][1][0xb4] = 0xc0;
                fmRegisterYM2608[chipID][1][0xb5] = 0xc0;
                fmRegisterYM2608[chipID][1][0xb6] = 0xc0;
                fmKeyOnYM2608[chipID] = new int[6] { 0, 0, 0, 0, 0, 0 };

                fmRegisterYM2610[chipID] = new int[2][] { new int[0x100], new int[0x100] };
                for (int i = 0; i < 0x100; i++)
                {
                    fmRegisterYM2610[chipID][0][i] = 0;
                    fmRegisterYM2610[chipID][1][i] = 0;
                }
                fmRegisterYM2610[chipID][0][0xb4] = 0xc0;
                fmRegisterYM2610[chipID][0][0xb5] = 0xc0;
                fmRegisterYM2610[chipID][0][0xb6] = 0xc0;
                fmRegisterYM2610[chipID][1][0xb4] = 0xc0;
                fmRegisterYM2610[chipID][1][0xb5] = 0xc0;
                fmRegisterYM2610[chipID][1][0xb6] = 0xc0;
                fmKeyOnYM2610[chipID] = new int[6] { 0, 0, 0, 0, 0, 0 };

                fmRegisterYM3526[chipID] = new int[0x100];
                for (int i = 0; i < 0x100; i++)
                {
                    fmRegisterYM3526[chipID][i] = 0;
                }

                fmRegisterYM3812[chipID] = new int[0x100];
                for (int i = 0; i < 0x100; i++)
                {
                    fmRegisterYM3812[chipID][i] = 0;
                }

                fmRegisterYMF262[chipID] = new int[2][] { new int[0x100], new int[0x100] };
                for (int i = 0; i < 0x100; i++)
                {
                    fmRegisterYMF262[chipID][0][i] = 0;
                    fmRegisterYMF262[chipID][1][i] = 0;
                }

                fmRegisterYMF271[chipID] = new int[7][] { new int[0x100], new int[0x100], new int[0x100], new int[0x100], new int[0x100], new int[0x100], new int[0x100] };
                for (int i = 0; i < 0x100; i++)
                {
                    fmRegisterYMF271[chipID][0][i] = 0;
                    fmRegisterYMF271[chipID][1][i] = 0;
                    fmRegisterYMF271[chipID][2][i] = 0;
                    fmRegisterYMF271[chipID][3][i] = 0;
                    fmRegisterYMF271[chipID][4][i] = 0;
                    fmRegisterYMF271[chipID][5][i] = 0;
                    fmRegisterYMF271[chipID][6][i] = 0;
                }

                fmRegisterYMF278B[chipID] = new int[3][] { new int[0x100], new int[0x100], new int[0x100] };
                for (int i = 0; i < 0x100; i++)
                {
                    fmRegisterYMF278B[chipID][0][i] = 0;
                    fmRegisterYMF278B[chipID][1][i] = 0;
                    fmRegisterYMF278B[chipID][2][i] = 0;
                }
                fmRegisterYMF278BRyhthm[0] = 0;
                fmRegisterYMF278BRyhthm[1] = 0;
                fmRegisterYMF278BRyhthmB[0] = 0;
                fmRegisterYMF278BRyhthmB[1] = 0;

                fmRegisterY8950[chipID] = new int[0x100];
                for (int i = 0; i < 0x100; i++)
                {
                    fmRegisterY8950[chipID][i] = 0;
                }

                YMZ280BRegister[chipID] = new int[0x100];
                for (int i = 0; i < 0x100; i++)
                {
                    YMZ280BRegister[chipID][i] = 0;
                }

                YM2151FmRegister[chipID] = new int[0x100];
                for (int i = 0; i < 0x100; i++)
                {
                    YM2151FmRegister[chipID][i] = 0;
                }
                YM2151FmKeyOn[chipID] = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 };

                fmRegisterYM2203[chipID] = new int[0x100];
                for (int i = 0; i < 0x100; i++)
                {
                    fmRegisterYM2203[chipID][i] = 0;
                }
                fmKeyOnYM2203[chipID] = new int[6] { 0, 0, 0, 0, 0, 0 };

                SN76489Register[chipID] = new int[8] { 0, 15, 0, 15, 0, 15, 0, 15 };

                fmRegisterYM2413[chipID] = new int[0x39];
                for (int i = 0; i < 0x39; i++)
                {
                    fmRegisterYM2413[chipID][i] = 0;
                }
                //fmRegisterYM2413Ryhthm[0] = 0;
                //fmRegisterYM2413Ryhthm[1] = 0;
                //fmRegisterYM2413RyhthmB[0] = 0;
                //fmRegisterYM2413RyhthmB[1] = 0;

                AY8910PsgRegister[chipID] = new int[0x100];
                for (int i = 0; i < 0x100; i++)
                {
                    AY8910PsgRegister[chipID][i] = 0;
                }
                AY8910PsgKeyOn[chipID] = new int[3] { 0, 0, 0 };

                pcmRegisterC140[chipID] = new byte[0x200];
                pcmKeyOnC140[chipID] = new bool[24];

                pcmRegisterC352[chipID] = new ushort[0x203];
                pcmKeyOnC352[chipID] = new ushort[32];

                pcmRegisterSEGAPCM[chipID] = new byte[0x200];
                pcmKeyOnSEGAPCM[chipID] = new bool[16];

                midiParams[chipID] = new MIDIParam();

                AY8910NowFadeoutVol[chipID] = 0;
                C140NowFadeoutVol[chipID] = 0;
                HuC6280NowFadeoutVol[chipID] = 0;
                K051649NowFadeoutVol[chipID] = 0;
                RF5C164NowFadeoutVol[chipID] = 0;
                SEGAPCMNowFadeoutVol[chipID] = 0;
                SN76489NowFadeoutVol[chipID] = 0;
                YM2151NowFadeoutVol[chipID] = 0;
                nowYM2203FadeoutVol[chipID] = 0;
                nowYM2413FadeoutVol[chipID] = 0;
                nowYM2608FadeoutVol[chipID] = 0;
                nowYM2610FadeoutVol[chipID] = 0;
                nowYM2612FadeoutVol[chipID] = 0;

            }

            nes_bank = null;
            nes_mem = null;
            nes_cpu = null;
            nes_apu = null;
            nes_dmc = null;
            nes_fds = null;
            nes_n106 = null;
            nes_vrc6 = null;
            nes_mmc5 = null;
            nes_fme7 = null;
            nes_vrc7 = null;

        }

        public MDSound.MDSound.Chip GetChipInfo(MDSound.MDSound.enmInstrumentType typ)
        {
            if (dicChipsInfo.ContainsKey(typ)) return dicChipsInfo[typ];
            return null;
        }

        public void Close()
        {
            //midiExport.Close();
        }




        //public midiOutInfo[] GetMIDIoutInfo()
        //{
        //    return midiOutInfos;
        //}

        //public void setMIDIout(midiOutInfo[] midiOutInfos, List<NAudio.Midi.MidiOut> midiOuts, List<int> midiOutsType, List<vstInfo2> vstMidiOuts, List<int> vstMidiOutsType)
        //{
        //    this.midiOutInfos = null;
        //    if (midiOutInfos != null && midiOutInfos.Length > 0)
        //    {
        //        this.midiOutInfos = new midiOutInfo[midiOutInfos.Length];
        //        for (int i = 0; i < midiOutInfos.Length; i++)
        //        {
        //            this.midiOutInfos[i] = new midiOutInfo();
        //            this.midiOutInfos[i].beforeSendType = midiOutInfos[i].beforeSendType;
        //            this.midiOutInfos[i].fileName = midiOutInfos[i].fileName;
        //            this.midiOutInfos[i].id = midiOutInfos[i].id;
        //            this.midiOutInfos[i].isVST = midiOutInfos[i].isVST;
        //            this.midiOutInfos[i].manufacturer = midiOutInfos[i].manufacturer;
        //            this.midiOutInfos[i].name = midiOutInfos[i].name;
        //            this.midiOutInfos[i].type = midiOutInfos[i].type;
        //            this.midiOutInfos[i].vendor = midiOutInfos[i].vendor;
        //        }
        //    }
        //    this.midiOuts = midiOuts;
        //    this.midiOutsType = midiOutsType;
        //    this.vstMidiOuts = vstMidiOuts;
        //    this.vstMidiOutsType = vstMidiOutsType;

        //    if (midiParams == null && midiParams.Length < 1) return;
        //    if (midiOutsType == null && vstMidiOutsType == null) return;
        //    if (midiOuts == null && vstMidiOuts == null) return;

        //    if (midiOutsType.Count > 0) midiParams[0].MIDIModule = Math.Min(midiOutsType[0], 2);
        //    if (midiOutsType.Count > 1) midiParams[1].MIDIModule = Math.Min(midiOutsType[1], 2);

        //    if (vstMidiOutsType.Count > 0)
        //    {
        //        if (midiOutsType.Count < 1 || (midiOutsType.Count > 0 && midiOuts[0] == null)) midiParams[0].MIDIModule = Math.Min(vstMidiOutsType[0], 2);
        //    }
        //    if (vstMidiOutsType.Count > 1)
        //    {
        //        if (midiOutsType.Count < 2 || (midiOutsType.Count > 1 && midiOuts[1] == null)) midiParams[1].MIDIModule = Math.Min(vstMidiOutsType[1], 2);
        //    }
        //}

        //internal void SetFileName(string fn)
        //{
        //    midiExport.PlayingFileName = fn;
        //}

        //private void MIDIWriteRegisterControl(Chip chip, EnmDataType type, int address, int data, object exData)
        //{
        //    if (MIDI.Model == EnmModel.RealModel)
        //    {
        //        switch (data)
        //        {
        //            case -1:
        //                if (exData != null && exData is byte[])
        //                {
        //                    midiOuts[address].SendBuffer((byte[])exData);
        //                    if (address < midiParams.Length) midiParams[address].SendBuffer((byte[])exData);
        //                }
        //                break;
        //            case -2:
        //                try
        //                {
        //                    //resetできない機種もある?
        //                    midiOuts[address].Reset();
        //                }
        //                catch { }
        //                break;
        //            case -3:
        //                midiParams[address].Lyric = (string)exData;
        //                break;
        //        }
        //    }
        //}

        public int getMIDIoutCount()
        {
            if (midiOuts == null) return 0;
            return midiOuts.Count;
        }

        public void sendMIDIout(long Counter, int chipID, byte cmd, byte prm1, byte prm2)//, int deltaFrames = 0)
        {
            if (MIDI[0].Model == EnmVRModel.RealModel)
            {
                if (midiOuts == null) return;
                if (chipID >= midiOuts.Count) return;
                if (midiOuts[chipID] == null) return;

                enq(null, Counter, MIDI[0], EnmDataType.Block, chipID, -1, new byte[] { cmd, prm1, prm2 });
                //midiOuts[chipID].SendBuffer(data);
                //if (chipID < midiParams.Length) midiParams[chipID].SendBuffer(data);
                return;
            }

            //EnmModel model = EnmModel.RealModel;
            //if (model == EnmModel.RealModel)
            //{
            //    if (midiOuts == null) return;
            //    if (num >= midiOuts.Count) return;
            //    if (midiOuts[num] == null) return;

            //    midiOuts[num].SendBuffer(new byte[] { cmd, prm1, prm2 });
            //    if (num < midiParams.Length) midiParams[num].SendBuffer(new byte[] { cmd, prm1, prm2 });
            //    return;
            //}

            //if (vstMidiOuts == null) return;
            //if (num >= vstMidiOuts.Count) return;
            //if (vstMidiOuts[num] == null) return;

            //Jacobi.Vst.Core.VstMidiEvent evt = new Jacobi.Vst.Core.VstMidiEvent(
            //    deltaFrames
            //    , 0//noteLength
            //    , 0//noteOffset
            //    , new byte[] { cmd, prm1, prm2 }
            //    , 0//detune
            //    , 0//noteOffVelocity
            //    );
            //vstMidiOuts[num].AddMidiEvent(evt);
            //if (num < midiParams.Length) midiParams[num].SendBuffer(new byte[] { cmd, prm1, prm2 });
        }

        public void sendMIDIout(long Counter, int chipID, byte cmd, byte prm1)//, int deltaFrames = 0)
        {
            if (MIDI[0].Model == EnmVRModel.RealModel)
            {
                if (midiOuts == null) return;
                if (chipID >= midiOuts.Count) return;
                if (midiOuts[chipID] == null) return;

                enq(null, Counter, MIDI[0], EnmDataType.Block, chipID, -1, new byte[] { cmd, prm1 });
                //midiOuts[chipID].SendBuffer(data);
                //if (chipID < midiParams.Length) midiParams[chipID].SendBuffer(data);
                return;
            }

            //EnmModel model = EnmModel.RealModel;
            //if (model == EnmModel.RealModel)
            //{
            //    if (midiOuts == null) return;
            //    if (num >= midiOuts.Count) return;
            //    if (midiOuts[num] == null) return;

            //    midiOuts[num].SendBuffer(new byte[] { cmd, prm1 });
            //    if (num < midiParams.Length) midiParams[num].SendBuffer(new byte[] { cmd, prm1 });
            //    return;
            //}

            //if (vstMidiOuts == null) return;
            //if (num >= vstMidiOuts.Count) return;
            //if (vstMidiOuts[num] == null) return;

            //Jacobi.Vst.Core.VstMidiEvent evt = new Jacobi.Vst.Core.VstMidiEvent(
            //    deltaFrames
            //    , 0//noteLength
            //    , 0//noteOffset
            //    , new byte[] { cmd, prm1 }
            //    , 0//detune
            //    , 0//noteOffVelocity
            //    );
            //vstMidiOuts[num].AddMidiEvent(evt);
            //if (num < midiParams.Length) midiParams[num].SendBuffer(new byte[] { cmd, prm1 });
        }

        public void sendMIDIout(long Counter, int chipID, byte[] data)
        {
            if (MIDI[0].Model == EnmVRModel.RealModel)
            {
                if (midiOuts == null) return;
                if (chipID >= midiOuts.Count) return;
                if (midiOuts[chipID] == null) return;

                enq(null, Counter, MIDI[0], EnmDataType.Block, chipID, -1, data);
                //midiOuts[chipID].SendBuffer(data);
                //if (chipID < midiParams.Length) midiParams[chipID].SendBuffer(data);
                return;
            }

            //if (vstMidiOuts == null) return;
            //if (num >= vstMidiOuts.Count) return;
            //if (vstMidiOuts[num] == null) return;

            //Jacobi.Vst.Core.VstMidiEvent evt = new Jacobi.Vst.Core.VstMidiEvent(
            //    deltaFrames
            //    , 0//noteLength
            //    , 0//noteOffset
            //    , data
            //    , 0//detune
            //    , 0//noteOffVelocity
            //    );
            //vstMidiOuts[num].AddMidiEvent(evt);
            //if (num < midiParams.Length) midiParams[num].SendBuffer(data);
        }

        //public void sendMIDIoutLyric(long Counter, int chipID, string comment)
        //{
        //    if (midiOuts == null) return;
        //    if (chipID >= midiOuts.Count) return;
        //    if (midiOuts[chipID] == null) return;

        //    enq(Counter, MIDI, EnmDataType.Block, chipID, -3, comment);

        //}

        //public void resetAllMIDIout()
        //{
        //    if (midiOuts != null)
        //    {
        //        for (int i = 0; i < midiOuts.Count; i++)
        //        {
        //            if (midiOuts[i] == null) continue;
        //            try
        //            {
        //                //resetできない機種もある?
        //                midiOuts[i].Reset();
        //            }
        //            catch { }
        //        }
        //    }
        //    if (vstMidiOuts != null)
        //    {
        //        for (int i = 0; i < vstMidiOuts.Count; i++)
        //        {
        //            if (vstMidiOuts[i] == null) continue;
        //            if (vstMidiOuts[i].vstPlugins == null) continue;
        //            if (vstMidiOuts[i].vstPlugins.PluginCommandStub == null) continue;
        //            try
        //            {
        //                List<byte> dat = new List<byte>();
        //                for (int ch = 0; ch < 16; ch++)
        //                {
        //                    sendMIDIout(0, i, new byte[] { (byte)(0xb0 + ch), 120, 0x00 });
        //                    sendMIDIout(0, i, new byte[] { (byte)(0xb0 + ch), 64, 0x00 });
        //                }

        //            }
        //            catch { }
        //        }
        //    }
        //}

        //public void MIDISoftReset(long Counter, int ChipID)
        //{
        //    List<PackData> data = MIDIMakeSoftReset(ChipID);
        //    enq(Counter, MIDI, EnmDataType.Block, ChipID, -2, data.ToArray());
        //}

        //public List<PackData> MIDIMakeSoftReset(int chipID)
        //{
        //    List<PackData> data = new List<PackData>();
        //    int i;

        //    for (i = 0; i < midiOuts.Count; i++)
        //    {
        //        data.Add(new PackData(MIDI, EnmDataType.Block, i, -2, null));//naudio midi reset
        //        for (int ch = 0; ch < 16; ch++)
        //        {
        //            data.Add(new PackData(MIDI, EnmDataType.Block, i, -1, new byte[] { (byte)(0xb0 + ch), 11, 0x00 }));//Volume 0
        //            data.Add(new PackData(MIDI, EnmDataType.Block, i, -1, new byte[] { (byte)(0xb0 + ch),  7, 0x00 }));//Exp 0
        //            data.Add(new PackData(MIDI, EnmDataType.Block, i, -1, new byte[] { (byte)(0xb0 + ch), 64, 0x00 }));//Hold off
        //            data.Add(new PackData(MIDI, EnmDataType.Block, i, -1, new byte[] { (byte)(0xb0 + ch), 91, 0x00 }));//Rev 0
        //            data.Add(new PackData(MIDI, EnmDataType.Block, i, -1, new byte[] { (byte)(0xb0 + ch), 93, 0x00 }));//Cho 0
        //            data.Add(new PackData(MIDI, EnmDataType.Block, i, -1, new byte[] { (byte)(0xb0 + ch), 94, 0x00 }));//Var 0
        //            data.Add(new PackData(MIDI, EnmDataType.Block, i, -1, new byte[] { (byte)(0xb0 + ch), 0x7b, 0x00 }));//all note off
        //            data.Add(new PackData(MIDI, EnmDataType.Block, i, -1, new byte[] { (byte)(0xb0 + ch), 0x78, 0x00 }));//all sound off
        //        }
        //    }

        //    return data;
        //}



        public void writeDummyChip(outDatum od, long Counter, byte chipID, byte chipNumber)
        {
            switch (chipID)
            {
                case 0:
                    enq(od, Counter, null, EnmDataType.Normal, -1, -1, null);
                    break;
                case 0xa0:
                    enq(od, Counter, AY8910[chipNumber], EnmDataType.Normal, -1, -1, null);
                    break;
                case 0x30:
                case 0x3f:
                case 0x4f:
                case 0x50:
                    enq(od, Counter, SN76489[chipNumber], EnmDataType.Normal, -1, -1, null);
                    break;
                case 0x51:
                case 0xa1:
                    enq(od, Counter, YM2413[chipNumber], EnmDataType.Normal, -1, -1, null);
                    break;
                case 0x52:
                case 0xa2:
                    enq(od, Counter, YM2612[chipNumber], EnmDataType.Normal, -1, -1, null);
                    break;
                case 0x54:
                case 0xa4:
                    enq(od, Counter, YM2151[chipNumber], EnmDataType.Normal, -1, -1, null);
                    break;
                case 0x55:
                case 0xa5:
                    enq(od, Counter, YM2203[chipNumber], EnmDataType.Normal, -1, -1, null);
                    break;
                case 0x56:
                case 0xa6:
                    enq(od, Counter, YM2608[chipNumber], EnmDataType.Normal, -1, -1, null);
                    break;
                case 0x58:
                case 0xa8:
                    enq(od, Counter, YM2610[chipNumber], EnmDataType.Normal, -1, -1, null);
                    break;
                case 0x5a:
                case 0xaa:
                    enq(od, Counter, YM3812[chipNumber], EnmDataType.Normal, -1, -1, null);
                    break;
                case 0x5b:
                case 0xab:
                    enq(od, Counter, YM3526[chipNumber], EnmDataType.Normal, -1, -1, null);
                    break;
                case 0x5c:
                case 0xac:
                    enq(od, Counter, Y8950[chipNumber], EnmDataType.Normal, -1, -1, null);
                    break;
                case 0x5e:
                case 0x5f:
                case 0xae:
                case 0xaf:
                    enq(od, Counter, YMF262[chipNumber], EnmDataType.Normal, -1, -1, null);
                    break;
                case 0xb1:
                    enq(od, Counter, RF5C164[chipNumber], EnmDataType.Normal, -1, -1, null);
                    break;
                case 0xb3:
                    enq(od, Counter, DMG[chipNumber], EnmDataType.Normal, -1, -1, null);
                    break;
                case 0xb4:
                    enq(od, Counter, NES[chipNumber], EnmDataType.Normal, -1, -1, null);
                    break;
                case 0xb9:
                    enq(od, Counter, HuC6280[chipNumber], EnmDataType.Normal, -1, -1, null);
                    break;
                case 0xc0:
                    enq(od, Counter, SEGAPCM[chipNumber], EnmDataType.Normal, -1, -1, null);
                    break;
                case 0xd0:
                    enq(od, Counter, YMF278B[chipNumber], EnmDataType.Normal, -1, -1, null);
                    break;
                case 0xd1:
                    enq(od, Counter, YMF271[chipNumber], EnmDataType.Normal, -1, -1, null);
                    break;
                case 0xd2:
                    enq(od, Counter, K051649[chipNumber], EnmDataType.Normal, -1, -1, null);
                    break;
                case 0xba:
                    enq(od, Counter, K053260[chipNumber], EnmDataType.Normal, -1, -1, null);
                    break;
                case 0xc4:
                    enq(od, Counter, QSound[chipNumber], EnmDataType.Normal, -1, -1, null);
                    break;
                case 0xd4:
                    enq(od, Counter, C140[chipNumber], EnmDataType.Normal, -1, -1, null);
                    break;
                case 0xe1:
                    enq(od, Counter, C352[chipNumber], EnmDataType.Normal, -1, -1, null);
                    break;
                default:
                    log.Write(string.Format("Chipregister.cs:writeDummyChip  unknown chipID:[{0:x02}]",chipID));
                    break;
            }
        }

        public void writeDummyChipZGM(outDatum od, long Counter, byte v1, byte v2)
        {
            int v = v1 + v2 * 256;
            if (!dicChipCmdNo.ContainsKey(v)) return;

            enq(od, Counter, dicChipCmdNo[v], EnmDataType.None, -1, -1, null);
        }

        public Dictionary<int, Driver.ZGM.ZgmChip.ZgmChip> dicChipCmdNo = new Dictionary<int, Driver.ZGM.ZgmChip.ZgmChip>();

        public void SetupDicChipCmdNo()
        {
            dicChipCmdNo.Clear();

            foreach (Driver.ZGM.ZgmChip.ZgmChip c in CONDUCTOR) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
            foreach (Driver.ZGM.ZgmChip.ZgmChip c in SN76489) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
            foreach (Driver.ZGM.ZgmChip.ZgmChip c in YM2413) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
            foreach (Driver.ZGM.ZgmChip.ZgmChip c in YM2612) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
            foreach (Driver.ZGM.ZgmChip.ZgmChip c in YM2151) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
            foreach (Driver.ZGM.ZgmChip.ZgmChip c in SEGAPCM) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
            foreach (Driver.ZGM.ZgmChip.ZgmChip c in YM2203) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
            foreach (Driver.ZGM.ZgmChip.ZgmChip c in YM2608) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
            foreach (Driver.ZGM.ZgmChip.ZgmChip c in YM2610) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
            foreach (Driver.ZGM.ZgmChip.ZgmChip c in YM3812) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
            foreach (Driver.ZGM.ZgmChip.ZgmChip c in YM3526) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
            foreach (Driver.ZGM.ZgmChip.ZgmChip c in Y8950) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
            foreach (Driver.ZGM.ZgmChip.ZgmChip c in YMF262) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
            foreach (Driver.ZGM.ZgmChip.ZgmChip c in YMF278B) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
            foreach (Driver.ZGM.ZgmChip.ZgmChip c in YMF271) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
            foreach (Driver.ZGM.ZgmChip.ZgmChip c in RF5C164) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
            foreach (Driver.ZGM.ZgmChip.ZgmChip c in AY8910) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
            foreach (Driver.ZGM.ZgmChip.ZgmChip c in DMG) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
            foreach (Driver.ZGM.ZgmChip.ZgmChip c in NES) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
            foreach (Driver.ZGM.ZgmChip.ZgmChip c in VRC6) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
            foreach (Driver.ZGM.ZgmChip.ZgmChip c in K051649) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
            foreach (Driver.ZGM.ZgmChip.ZgmChip c in HuC6280) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
            foreach (Driver.ZGM.ZgmChip.ZgmChip c in C140) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
            foreach (Driver.ZGM.ZgmChip.ZgmChip c in K053260) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
            foreach (Driver.ZGM.ZgmChip.ZgmChip c in QSound) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
            foreach (Driver.ZGM.ZgmChip.ZgmChip c in C352) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
            foreach (Driver.ZGM.ZgmChip.ZgmChip c in YM2609) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
            foreach (Driver.ZGM.ZgmChip.ZgmChip c in MIDI) dicChipCmdNo.Add(c.defineInfo.commandNo, c);
        }




        #region DACControl

        private void DACControlWriteRegisterControl(Chip Chip, EnmDataType type, int address, int data, object exData)
        {
            object[] args = (object[])exData;

            if (type == EnmDataType.Normal)
            {
                if (Chip.Model == EnmVRModel.VirtualModel)
                {
                    switch (address)
                    {
                        case 0:
                            mds.dacControl.SetupStreamControl(
                                (byte)args[0]
                                , (byte)args[1]
                                , (byte)args[2]
                                , (byte)args[3]
                                , (byte)args[4]
                                , (byte)args[5]
                                , (byte)args[6]
                                );
                            break;
                        case 1:
                            mds.dacControl.SetStreamData(
                                (byte)args[0]
                                , (byte)args[1]
                                , (byte)args[2]
                                , (byte)args[3]
                                );
                            break;
                        case 2:
                            mds.dacControl.SetStreamFrequency(
                                (byte)args[0]
                                , (uint)args[1]
                                );
                            break;
                        case 3:
                            mds.dacControl.StartStream(
                                (byte)args[0]
                                , (uint)args[1]
                                , (byte)args[2]
                                , (uint)args[3]
                                );
                            break;
                        case 4:
                            mds.dacControl.StopStream(
                                (byte)args[0]
                                );
                            break;
                        case 5:
                            mds.dacControl.StartStreamFastCall(
                                (byte)args[0]
                                , (uint)args[1]
                                , (byte)args[2]
                                );
                            break;
                    }
                }
                if (Chip.Model == EnmVRModel.RealModel)
                {
                    ;
                }
            }
            else if (type == EnmDataType.Block)
            {
                Audio.sm.SetInterrupt();

                try
                {
                    if (exData == null) return;

                    PackData[] pdata = (PackData[])exData;
                    if (Chip.Model == EnmVRModel.VirtualModel)
                    {
                        foreach (PackData dat in pdata)
                            ;
                    }
                    if (Chip.Model == EnmVRModel.RealModel)
                    {
                        if (scAY8910[Chip.Number] != null)
                        {
                            foreach (PackData dat in pdata)
                            {
                                ;
                            }
                        }
                    }
                }
                finally
                {
                    Audio.sm.ResetInterrupt();
                }
            }
        }

        public void DACControlSetRegisterProcessing(ref long Counter, ref Chip Chip, ref EnmDataType Type, ref int Address, ref int dData, ref object ExData)
        {
        }

        public void DACControlAddPCMData(byte type,uint DataSize,uint Adr,byte[] vgmBuf)
        {
            mds.dacControl.AddPCMData(type, DataSize, Adr, vgmBuf);
        }

        public void DACControlSetupStreamControl(outDatum od, long Counter, byte si, byte ChipType, byte EmuType, byte ChipIndex, byte ChipID, byte port, byte cmd)
        {
            byte et = EmuType;
            switch (ChipType)
            {
                case 0:
                    if (ctSN76489 == null || ctSN76489.Length < ChipID + 1 || ctSN76489[ChipID] == null) et = 0;
                    else if (ctSN76489[ChipID].UseEmu) et = 0;
                    else if (ctSN76489[ChipID].UseEmu2) et = 1;
                    else if (ctSN76489[ChipID].UseEmu3) et = 2;
                    break;
                case 2:
                    if (ctYM2612 == null || ctYM2612.Length < ChipID + 1 || ctYM2612[ChipID] == null) et = 0;
                    else if (ctYM2612[ChipID].UseEmu) et = 0;
                    else if (ctYM2612[ChipID].UseEmu2) et = 1;
                    else if (ctYM2612[ChipID].UseEmu3) et = 2;
                    break;
            }

            enq(od, Counter, DACControl[ChipID], EnmDataType.Normal, 0, 0,
                new object[] { si, ChipType, et, ChipIndex, ChipID, port, cmd });
            //mds.dacControl.SetupStreamControl(si, ChipType, EmuType, ChipIndex, ChipID, port, cmd);
        }

        public void DACControlSetStreamData(outDatum od, long Counter, int ChipID, byte si, byte bank, byte StepSize, byte StepBase)
        {
            enq(od, Counter, DACControl[ChipID], EnmDataType.Normal, 1, 0,
                new object[] { si, bank, StepSize, StepBase });
        }

        public void DACControlSetFrequency(outDatum od, long Counter, int ChipID, byte si, uint TempLng)
        {
            enq(od, Counter, DACControl[ChipID], EnmDataType.Normal, 2, 0,
                new object[] { si, TempLng });
            //mds.dacControl.SetStreamFrequency(si, TempLng);
        }

        public void DACControlStartStream(outDatum od, long Counter, int ChipID, byte si, uint DataStart, byte TempByt, uint DataLen)
        {
            enq(od, Counter, DACControl[ChipID], EnmDataType.Normal, 3, 0,
                new object[] { si, DataStart, TempByt, DataLen });
            //mds.dacControl.StartStream(si, DataStart, TempByt, DataLen);
        }

        public void DACControlStopStream(outDatum od, long Counter, int ChipID, byte si)
        {
            enq(od, Counter, DACControl[ChipID], EnmDataType.Normal, 4, 0,
                new object[] { si });
            //mds.dacControl.StopStream(si);
        }

        public void DACControlStartStream(outDatum od, long Counter, int ChipID, byte CurChip, uint TempSht, byte mode)
        {
            enq(od, Counter, DACControl[ChipID], EnmDataType.Normal, 5, 0,
                new object[] { CurChip, TempSht, mode });
            //mds.dacControl.StartStreamFastCall(CurChip, TempSht, mode);
        }

        #endregion



        #region AY8910

        private void AY8910WriteRegisterControl(Chip Chip, EnmDataType type, int address, int data, object exData)
        {
            if (type == EnmDataType.Normal)
            {
                if (Chip.Model == EnmVRModel.VirtualModel)
                {
                    if (!ctAY8910[Chip.Number].UseScci && ctAY8910[Chip.Number].UseEmu)
                    {
                        mds.WriteAY8910(Chip.Index, (byte)Chip.Number, (byte)address, (byte)data);
                        //Console.WriteLine("{0}:{1}:{2}:{3}", Chip.Index, (byte)Chip.Number, (byte)address, (byte)data);
                    }
                }
                if (Chip.Model == EnmVRModel.RealModel)
                {
                    if (scAY8910[Chip.Number] != null)
                    {
                        int skip = 0x0;
                        //if(scAY8910[Chip.Number] is RC86ctlSoundChip)
                        //{
                        //    if(((RC86ctlSoundChip)scAY8910[Chip.Number]).chiptype== Nc86ctl.ChipType.CHIP_UNKNOWN)
                        //    {
                        //        skip = 0x100;
                        //    }
                        //}
                        scAY8910[Chip.Number].setRegister(address + skip, data);
                    }
                }
            }
            else if (type == EnmDataType.Block)
            {
                Audio.sm.SetInterrupt();

                try
                {
                    if (exData == null) return;

                    PackData[] pdata = (PackData[])exData;
                    if (Chip.Model == EnmVRModel.VirtualModel)
                    {
                        foreach (PackData dat in pdata)
                            mds.WriteAY8910(Chip.Index, (byte)Chip.Number, (byte)dat.Address, (byte)dat.Data);
                    }
                    if (Chip.Model == EnmVRModel.RealModel)
                    {
                        if (scAY8910[Chip.Number] != null)
                        {
                            foreach (PackData dat in pdata)
                            {
                                int skip = 0x0;
                                //if (scAY8910[Chip.Number] is RC86ctlSoundChip)
                                //{
                                //    if (((RC86ctlSoundChip)scAY8910[Chip.Number]).chiptype == Nc86ctl.ChipType.CHIP_UNKNOWN)
                                //    {
                                //        skip = 0x100;
                                //    }
                                //}
                                scAY8910[Chip.Number].setRegister(dat.Address + skip, dat.Data);
                            }
                        }
                    }
                }
                finally
                {
                    Audio.sm.ResetInterrupt();
                }
            }
        }

        public void AY8910SetRegisterProcessing(ref long Counter, ref Chip Chip, ref EnmDataType Type, ref int Address, ref int dData, ref object ExData)
        {
            if (ctAY8910 == null) return;
            if (Address == -1 && dData == -1) return;
            if (!AY8910[Chip.Number].Use) return;

            if (Chip.Number == 0) chipLED.PriAY10 = 2;
            else chipLED.SecAY10 = 2;

            int dAddr = (Address & 0xff);

            AY8910PsgRegister[Chip.Number][dAddr] = dData;

            //ssg level
            if ((dAddr == 0x08 || dAddr == 0x09 || dAddr == 0x0a))
            {
                int d = AY8910NowFadeoutVol[Chip.Number];// >> 3;
                dData = Math.Max(dData - d, 0);
                dData = Chip.ChMasks[dAddr - 0x08] ? 0 : dData;
            }

            if (AY8910[Chip.Number].Model == EnmVRModel.RealModel && scAY8910[Chip.Number].mul != 1.0)
            {
                if (dAddr >= 0x00 && dAddr <= 0x05)
                {
                    int ch = dAddr >> 1;
                    int b = dAddr & 1;
                    int nowFreq = AY8910ChFreq[ch];
                    dData &= 0xff;
                    if (b == 0)
                    {
                        AY8910ChFreq[ch] = (AY8910ChFreq[ch] & 0xf00) | dData;
                    }
                    else
                    {
                        AY8910ChFreq[ch] = (AY8910ChFreq[ch] & 0x0ff) | (dData << 8);
                    }

                    if (nowFreq != AY8910ChFreq[ch])
                    {
                        nowFreq = (int)(AY8910ChFreq[ch] * scAY8910[Chip.Number].mul);
                        PackData[] pdata = new PackData[2] { new PackData(), new PackData() };

                        pdata[0].Address = ch * 2;
                        pdata[0].Data = (byte)nowFreq;
                        pdata[1].Address = ch * 2 + 1;
                        pdata[1].Data = (byte)(nowFreq >> 8);

                        Type = EnmDataType.Block;
                        ExData = pdata;
                    }
                }
                if (dAddr == 0x06)
                {

                }
            }

        }

        public void AY8910SetRegister(outDatum od, long Counter, int ChipID, int dAddr, int dData)
        {
            enq(od, Counter, AY8910[ChipID], EnmDataType.Normal, dAddr, dData, null);
        }

        public void AY8910SetRegister(outDatum od, long Counter, int ChipID, PackData[] data)
        {
            enq(od, Counter, AY8910[ChipID], EnmDataType.Block, -1, -1, data);
        }

        public void AY8910SoftReset(long Counter, int ChipID)
        {
            List<PackData> data = AY8910MakeSoftReset(ChipID);
            AY8910SetRegister(null, Counter, ChipID, data.ToArray());
        }

        public List<PackData> AY8910MakeSoftReset(int chipID)
        {
            List<PackData> data = new List<PackData>();
            int i;

            // SSG 音程(2byte*3ch)
            for (i = 0x00; i < 0x05 + 1; i++)
            {
                data.Add(new PackData(null, AY8910[chipID], EnmDataType.Normal, i, 0x00, null));
            }
            data.Add(new PackData(null, AY8910[chipID], EnmDataType.Normal, 0x06, 0x00, null)); // SSG ノイズ周波数
            data.Add(new PackData(null, AY8910[chipID], EnmDataType.Normal, 0x07, 0x38, null)); // SSG ミキサ
                                                                                                // SSG ボリューム(3ch)
            for (i = 0x08; i < 0x0A + 1; i++)
            {
                data.Add(new PackData(null, AY8910[chipID], EnmDataType.Normal, i, 0x00, null));
            }
            // SSG Envelope
            for (i = 0x0B; i < 0x0D + 1; i++)
            {
                data.Add(new PackData(null, AY8910[chipID], EnmDataType.Normal, i, 0x00, null));
            }

            return data;
        }

        public void AY8910SetMask(long Counter, int chipID, int ch, bool mask, bool noSend = false)
        {
            AY8910MaskPSGCh[chipID][ch] = mask;

            if (noSend) return;

            int c = ch;
            if (ch < 3)
            {
                AY8910SetRegister(null, Counter, (byte)chipID, 0x08 + c, AY8910PsgRegister[chipID][0x08 + c]);
            }
        }

        public void AY8910WriteClock(byte chipID, int clock)
        {
            if (scAY8910 != null && scAY8910[chipID] != null)
            {
                if (scAY8910[chipID] is RC86ctlSoundChip)
                {
                    Nc86ctl.ChipType ct = ((RC86ctlSoundChip)scAY8910[chipID]).chiptype;
                    //OPNA/OPN3Lが選ばれている場合は周波数を2倍にする
                    if (ct == Nc86ctl.ChipType.CHIP_OPN3L || ct == Nc86ctl.ChipType.CHIP_OPNA)
                    {
                        clock *= 4;
                    }
                }

                scAY8910[chipID].dClock = scAY8910[chipID].SetMasterClock((uint)clock);
                scAY8910[chipID].mul = (double)scAY8910[chipID].dClock / (double)clock;

                if(scAY8910[chipID] is RC86ctlSoundChip)
                {
                    if(((RC86ctlSoundChip)scAY8910[chipID]).chiptype== Nc86ctl.ChipType.CHIP_UNKNOWN)
                    {
                        scAY8910[chipID].mul = 1.0;
                    }
                }
            }
        }

        public void AY8910SetFadeoutVolume(long Counter, int v)
        {
            for (int i = 0; i < AY8910.Count; i++)
            {
                if (!AY8910[i].Use) continue;
                if (AY8910[i].Model == EnmVRModel.VirtualModel) continue;
                if (AY8910NowFadeoutVol[i] == v) continue;

                AY8910NowFadeoutVol[i] = v;
                for (int c = 0; c < 3; c++)
                {
                    AY8910SetRegister(null, Counter, i, 0x08 + c, AY8910PsgRegister[i][0x08 + c]);
                }
            }
        }

        public void AY8910SetSSGVolume(byte chipID, int vol)
        {
            if (scAY8910 != null && scAY8910[chipID] != null)
            {
                scAY8910[chipID].setSSGVolume((byte)vol);
            }
        }

        #endregion



        #region DMG

        private void DMGWriteRegisterControl(Chip Chip, EnmDataType type, int address, int data, object exData)
        {
            if (type == EnmDataType.Normal)
            {
                if (Chip.Model == EnmVRModel.VirtualModel)
                {
                    if (!ctDMG[Chip.Number].UseScci && ctDMG[Chip.Number].UseEmu)
                        mds.WriteDMG(Chip.Index, (byte)Chip.Number, (byte)address, (byte)data);
                }
                if (Chip.Model == EnmVRModel.RealModel)
                {
                    if (scDMG[Chip.Number] != null)
                    {
                        int skip = 0x0;
                        if (scDMG[Chip.Number] is RC86ctlSoundChip)
                        {
                            if (((RC86ctlSoundChip)scDMG[Chip.Number]).chiptype == Nc86ctl.ChipType.CHIP_UNKNOWN)
                            {
                                skip = 0x100;
                            }
                        }
                        scDMG[Chip.Number].setRegister(address + skip, data);
                    }
                }
            }
            else if (type == EnmDataType.Block)
            {
                Audio.sm.SetInterrupt();

                try
                {
                    if (exData == null) return;

                    PackData[] pdata = (PackData[])exData;
                    if (Chip.Model == EnmVRModel.VirtualModel)
                    {
                        foreach (PackData dat in pdata)
                            mds.WriteDMG(Chip.Index, (byte)Chip.Number, (byte)dat.Address, (byte)dat.Data);
                    }
                    if (Chip.Model == EnmVRModel.RealModel)
                    {
                        if (scDMG[Chip.Number] != null)
                        {
                            foreach (PackData dat in pdata)
                            {
                                int skip = 0x0;
                                if (scDMG[Chip.Number] is RC86ctlSoundChip)
                                {
                                    if (((RC86ctlSoundChip)scDMG[Chip.Number]).chiptype == Nc86ctl.ChipType.CHIP_UNKNOWN)
                                    {
                                        skip = 0x100;
                                    }
                                }
                                scDMG[Chip.Number].setRegister(dat.Address + skip, dat.Data);
                            }
                        }
                    }
                }
                finally
                {
                    Audio.sm.ResetInterrupt();
                }
            }
        }

        public void DMGSetRegisterProcessing(ref long Counter, ref Chip Chip, ref EnmDataType Type, ref int Address, ref int dData, ref object ExData)
        {
        }

        public void DMGSetRegister(outDatum od, long Counter, int ChipID, int dAddr, int dData)
        {
            enq(od, Counter, DMG[ChipID], EnmDataType.Normal, dAddr, dData, null);
        }

        public void DMGSetRegister(outDatum od, long Counter, int ChipID, PackData[] data)
        {
            enq(od, Counter, DMG[ChipID], EnmDataType.Block, -1, -1, data);
        }

        public void DMGSoftReset(long Counter, int ChipID)
        {
            List<PackData> data = DMGMakeSoftReset(ChipID);
            DMGSetRegister(null, Counter, ChipID, data.ToArray());
        }

        public List<PackData> DMGMakeSoftReset(int chipID)
        {
            List<PackData> data = new List<PackData>();
            int i;

            // SSG 音程(2byte*3ch)
            for (i = 0x00; i < 0x05 + 1; i++)
            {
                data.Add(new PackData(null, DMG[chipID], EnmDataType.Normal, i, 0x00, null));
            }
            data.Add(new PackData(null, DMG[chipID], EnmDataType.Normal, 0x06, 0x00, null)); // SSG ノイズ周波数
            data.Add(new PackData(null, DMG[chipID], EnmDataType.Normal, 0x07, 0x38, null)); // SSG ミキサ
                                                                                                // SSG ボリューム(3ch)
            for (i = 0x08; i < 0x0A + 1; i++)
            {
                data.Add(new PackData(null, DMG[chipID], EnmDataType.Normal, i, 0x00, null));
            }
            // SSG Envelope
            for (i = 0x0B; i < 0x0D + 1; i++)
            {
                data.Add(new PackData(null, DMG[chipID], EnmDataType.Normal, i, 0x00, null));
            }

            return data;
        }

        public void DMGSetMask(long Counter, int chipID, int ch, bool mask, bool noSend = false)
        {
        }

        public void DMGWriteClock(byte chipID, int clock)
        {
            if (scDMG != null && scDMG[chipID] != null)
            {
                if (scDMG[chipID] is RC86ctlSoundChip)
                {
                    Nc86ctl.ChipType ct = ((RC86ctlSoundChip)scDMG[chipID]).chiptype;
                }

                scDMG[chipID].dClock = scDMG[chipID].SetMasterClock((uint)clock);
                scDMG[chipID].mul = (double)scDMG[chipID].dClock / (double)clock;

                if (scDMG[chipID] is RC86ctlSoundChip)
                {
                    if (((RC86ctlSoundChip)scDMG[chipID]).chiptype == Nc86ctl.ChipType.CHIP_UNKNOWN)
                    {
                        scDMG[chipID].mul = 1.0;
                    }
                }
            }
        }

        public void DMGSetFadeoutVolume(long Counter, int v)
        {
        }

        public void DMGSetSSGVolume(byte chipID, int vol)
        {
        }

        #endregion



        #region NES

        private void NESWriteRegisterControl(Chip Chip, EnmDataType type, int address, int data, object exData)
        {
            if (type == EnmDataType.Normal)
            {
                if (Chip.Model == EnmVRModel.VirtualModel)
                {
                    if (!ctNES[Chip.Number].UseScci && ctNES[Chip.Number].UseEmu)
                        mds.WriteNES(Chip.Index, (byte)Chip.Number, (byte)address, (byte)data);
                }
                if (Chip.Model == EnmVRModel.RealModel)
                {
                    if (scNES[Chip.Number] != null)
                    {
                        int skip = 0x0;
                        if (scNES[Chip.Number] is RC86ctlSoundChip)
                        {
                            if (((RC86ctlSoundChip)scNES[Chip.Number]).chiptype == Nc86ctl.ChipType.CHIP_UNKNOWN)
                            {
                                skip = 0x100;
                            }
                        }
                        scNES[Chip.Number].setRegister(address + skip, data);
                    }
                }
            }
            else if (type == EnmDataType.Block)
            {
                Audio.sm.SetInterrupt();

                try
                {
                    if (exData == null) return;

                    if (exData is PackData[])
                    {
                        PackData[] pdata = (PackData[])exData;
                        if (Chip.Model == EnmVRModel.VirtualModel)
                        {
                            foreach (PackData dat in pdata)
                                mds.WriteNES(Chip.Index, (byte)Chip.Number, (byte)dat.Address, (byte)dat.Data);
                        }
                        if (Chip.Model == EnmVRModel.RealModel)
                        {
                            if (scNES[Chip.Number] != null)
                            {
                                foreach (PackData dat in pdata)
                                {
                                    int skip = 0x0;
                                    if (scNES[Chip.Number] is RC86ctlSoundChip)
                                    {
                                        if (((RC86ctlSoundChip)scNES[Chip.Number]).chiptype == Nc86ctl.ChipType.CHIP_UNKNOWN)
                                        {
                                            skip = 0x100;
                                        }
                                    }
                                    scNES[Chip.Number].setRegister(dat.Address + skip, dat.Data);
                                }
                            }
                        }
                        return;
                    }

                    uint stAdr = (uint)((object[])exData)[0];
                    uint dataSize = (uint)((object[])exData)[1];
                    byte[] pcmData = (byte[])((object[])exData)[2];
                    uint vgmAdr = (uint)((object[])exData)[3];

                    if (Chip.Model == EnmVRModel.VirtualModel)
                    {
                        log.Write("Sending NES(Emu) PCM");
                        mds.WriteNESRam(Chip.Index, (byte)Chip.Number, (int)stAdr, (int)dataSize, pcmData, (int)vgmAdr);
                    }
                    if (Chip.Model == EnmVRModel.RealModel)
                    {
                        ;
                    }
                }
                finally
                {
                    Audio.sm.ResetInterrupt();
                }
            }
        }

        public void NESSetRegisterProcessing(ref long Counter, ref Chip Chip, ref EnmDataType Type, ref int Address, ref int dData, ref object ExData)
        {
        }

        public void NESSetRegister(outDatum od, long Counter, int ChipID, int dAddr, int dData)
        {
            enq(od, Counter, NES[ChipID], EnmDataType.Normal, dAddr, dData, null);
        }

        public void NESSetRegister(outDatum od, long Counter, int ChipID, PackData[] data)
        {
            enq(od, Counter, NES[ChipID], EnmDataType.Block, -1, -1, data);
        }

        public void NESSoftReset(long Counter, int ChipID)
        {
            List<PackData> data = NESMakeSoftReset(ChipID);
            NESSetRegister(null, Counter, ChipID, data.ToArray());
        }

        public List<PackData> NESMakeSoftReset(int chipID)
        {
            List<PackData> data = new List<PackData>();
            for (int i = 0; i < 0x80; i++)
            {
                if (i == 0x3f) continue;//0x4023はスキップ
                data.Add(new PackData(null, NES[chipID], EnmDataType.Normal, i, 0x00, null));
            }

            data.Add(new PackData(null, NES[chipID], EnmDataType.Normal, 0x29, 0x80, null));
            data.Add(new PackData(null, NES[chipID], EnmDataType.Normal, 0x3f, 0x00, null));

            return data;
        }

        public void NESWritePCMData(outDatum od, long Counter, byte ChipID, uint stAdr, uint dataSize, byte[] vgmBuf, uint vgmAdr)
        {
            enq(od, Counter, NES[ChipID], EnmDataType.Block, -1, -2, new object[] { stAdr, dataSize, vgmBuf, vgmAdr });

        }

        public void NESSetMask(long Counter, int chipID, int ch, bool mask, bool noSend = false)
        {
        }

        public void NESWriteClock(byte chipID, int clock)
        {
            if (scNES != null && scNES[chipID] != null)
            {
                //if (scNES[chipID] is RC86ctlSoundChip)
                //{
                //    Nc86ctl.ChipType ct = ((RC86ctlSoundChip)scNES[chipID]).chiptype;
                //    //OPNA/OPN3Lが選ばれている場合は周波数を2倍にする
                //    if (ct == Nc86ctl.ChipType.CHIP_OPN3L || ct == Nc86ctl.ChipType.CHIP_OPNA)
                //    {
                //        clock *= 4;
                //    }
                //}

                scNES[chipID].dClock = scNES[chipID].SetMasterClock((uint)clock);
                scNES[chipID].mul = (double)scNES[chipID].dClock / (double)clock;

                if (scNES[chipID] is RC86ctlSoundChip)
                {
                    if (((RC86ctlSoundChip)scNES[chipID]).chiptype == Nc86ctl.ChipType.CHIP_UNKNOWN)
                    {
                        scNES[chipID].mul = 1.0;
                    }
                }
            }
        }

        public void NESSetFadeoutVolume(long Counter, int v)
        {
        }

        public void NESSetSSGVolume(byte chipID, int vol)
        {
        }

        #endregion



        #region VRC6

        private void VRC6WriteRegisterControl(Chip Chip, EnmDataType type, int address, int data, object exData)
        {
            if (type == EnmDataType.Normal)
            {
                if (Chip.Model == EnmVRModel.VirtualModel)
                {
                    if (!ctVRC6[Chip.Number].UseScci && ctVRC6[Chip.Number].UseEmu)
                        mds.WriteVRC6(Chip.Index, (byte)Chip.Number, (byte)address, (byte)data);
                }
                if (Chip.Model == EnmVRModel.RealModel)
                {
                    if (scVRC6[Chip.Number] != null)
                    {
                        int skip = 0x0;
                        if (scVRC6[Chip.Number] is RC86ctlSoundChip)
                        {
                            if (((RC86ctlSoundChip)scVRC6[Chip.Number]).chiptype == Nc86ctl.ChipType.CHIP_UNKNOWN)
                            {
                                skip = 0x100;
                            }
                        }
                        scVRC6[Chip.Number].setRegister(address + skip, data);
                    }
                }
            }
            else if (type == EnmDataType.Block)
            {
                Audio.sm.SetInterrupt();

                try
                {
                    if (exData == null) return;

                    if (exData is PackData[])
                    {
                        PackData[] pdata = (PackData[])exData;
                        if (Chip.Model == EnmVRModel.VirtualModel)
                        {
                            foreach (PackData dat in pdata)
                                mds.WriteVRC6(Chip.Index, (byte)Chip.Number, (byte)dat.Address, (byte)dat.Data);
                        }
                        if (Chip.Model == EnmVRModel.RealModel)
                        {
                            if (scNES[Chip.Number] != null)
                            {
                                foreach (PackData dat in pdata)
                                {
                                    int skip = 0x0;
                                    if (scVRC6[Chip.Number] is RC86ctlSoundChip)
                                    {
                                        if (((RC86ctlSoundChip)scVRC6[Chip.Number]).chiptype == Nc86ctl.ChipType.CHIP_UNKNOWN)
                                        {
                                            skip = 0x100;
                                        }
                                    }
                                    scVRC6[Chip.Number].setRegister(dat.Address + skip, dat.Data);
                                }
                            }
                        }
                        return;
                    }

                    uint stAdr = (uint)((object[])exData)[0];
                    uint dataSize = (uint)((object[])exData)[1];
                    byte[] pcmData = (byte[])((object[])exData)[2];
                    uint vgmAdr = (uint)((object[])exData)[3];

                    if (Chip.Model == EnmVRModel.VirtualModel)
                    {
                    }
                    if (Chip.Model == EnmVRModel.RealModel)
                    {
                        ;
                    }
                }
                finally
                {
                    Audio.sm.ResetInterrupt();
                }
            }
        }

        public void VRC6SetRegisterProcessing(ref long Counter, ref Chip Chip, ref EnmDataType Type, ref int Address, ref int dData, ref object ExData)
        {
        }

        public void VRC6SetRegister(outDatum od, long Counter, int ChipID, int dAddr, int dData)
        {
            enq(od, Counter, VRC6[ChipID], EnmDataType.Normal, dAddr, dData, null);
        }

        public void VRC6SetRegister(outDatum od, long Counter, int ChipID, PackData[] data)
        {
            enq(od, Counter, VRC6[ChipID], EnmDataType.Block, -1, -1, data);
        }

        public void VRC6SoftReset(long Counter, int ChipID)
        {
            List<PackData> data = VRC6MakeSoftReset(ChipID);
            VRC6SetRegister(null, Counter, ChipID, data.ToArray());
        }

        public List<PackData> VRC6MakeSoftReset(int chipID)
        {
            List<PackData> data = new List<PackData>();
            data.Add(new PackData(null, VRC6[chipID], EnmDataType.Normal, 0x00, 0x00, null));//Pulse vol:0
            data.Add(new PackData(null, VRC6[chipID], EnmDataType.Normal, 0x04, 0x00, null));//Pulse vol:0
            data.Add(new PackData(null, VRC6[chipID], EnmDataType.Normal, 0x08, 0x00, null));//Saw vol:0

            //// duty比50%:7 volume:15 = 0x7f
            //data.Add(new PackData(null, VRC6[chipID], EnmDataType.Normal, 0x00, 0x7f, null));
            //// 1789773Hz / 16 / 440Hz - 1 = 253(0xfd)
            //data.Add(new PackData(null, VRC6[chipID], EnmDataType.Normal, 0x01, 0xfd, null));//(CPU / (16 * f)) - 1 
            //// phase reset
            //data.Add(new PackData(null, VRC6[chipID], EnmDataType.Normal, 0x02, 0x80, null));//Pulse vol:0
            ////SawはCPU/14/f-1

            return data;
        }

        public void VRC6WriteClock(byte chipID, int clock)
        {
            if (scVRC6 != null && scVRC6[chipID] != null)
            {
                scVRC6[chipID].dClock = scVRC6[chipID].SetMasterClock((uint)clock);
                scVRC6[chipID].mul = (double)scVRC6[chipID].dClock / (double)clock;

                if (scVRC6[chipID] is RC86ctlSoundChip)
                {
                    if (((RC86ctlSoundChip)scVRC6[chipID]).chiptype == Nc86ctl.ChipType.CHIP_UNKNOWN)
                    {
                        scVRC6[chipID].mul = 1.0;
                    }
                }
            }
        }

        #endregion



        #region C140

        private void C140WriteRegisterControl(Chip Chip, EnmDataType type, int address, int data, object exData)
        {
            if (type == EnmDataType.Normal)
            {
                if (Chip.Model == EnmVRModel.VirtualModel)
                {
                    if (!ctC140[Chip.Number].UseScci && ctC140[Chip.Number].UseEmu)
                        mds.WriteC140(Chip.Index, (byte)Chip.Number, (uint)address, (byte)data);
                }
                if (Chip.Model == EnmVRModel.RealModel)
                {
                    if (scC140[Chip.Number] != null)
                        scC140[Chip.Number].setRegister(address, data);
                }
            }
            else if (type == EnmDataType.Block)
            {
                Audio.sm.SetInterrupt();

                try
                {
                    if (exData == null) return;

                    if (data == -1)
                    {
                        PackData[] pdata = (PackData[])exData;
                        if (Chip.Model == EnmVRModel.VirtualModel)
                        {
                            foreach (PackData dat in pdata)
                                mds.WriteC140(Chip.Index, (byte)Chip.Number, (uint)dat.Address, (byte)dat.Data);
                        }
                        if (Chip.Model == EnmVRModel.RealModel)
                        {
                            if (scC140[Chip.Number] != null)
                            {
                                foreach (PackData dat in pdata)
                                    scC140[Chip.Number].setRegister(dat.Address, dat.Data);
                            }
                        }
                    }
                    else
                    {
                        if (Chip.Model == EnmVRModel.VirtualModel)
                            mds.WriteC140PCMData(Chip.Index, (byte)Chip.Number, (uint)((object[])exData)[0], (uint)((object[])exData)[1], (uint)((object[])exData)[2], (byte[])((object[])exData)[3], (uint)((object[])exData)[4]);
                        // ROMSize, DataStart, DataLength, romdata, SrcStartAdr);
                        else
                        {
                            if (scC140 != null && scC140[Chip.Number] != null)
                            {
                                // スタートアドレス設定
                                scC140[Chip.Number].setRegister(0x10000, (byte)((uint)((object[])exData)[1]));
                                scC140[Chip.Number].setRegister(0x10001, (byte)((uint)((object[])exData)[1] >> 8));
                                scC140[Chip.Number].setRegister(0x10002, (byte)((uint)((object[])exData)[1] >> 16));
                                // データ転送
                                for (int cnt = 0; cnt < (uint)((object[])exData)[2]; cnt++)
                                {
                                    scC140[Chip.Number].setRegister(0x10004, ((byte[])((object[])exData)[3])[(uint)((object[])exData)[4] + cnt]);
                                }
                                //scC140[chipID].setRegister(0x10006, (int)ROMSize);

                                realChip.SendData();
                            }
                        }
                    }
                }
                finally
                {
                    Audio.sm.ResetInterrupt();
                }
            }
        }

        public void C140SetRegisterProcessing(ref long Counter, ref Chip Chip, ref EnmDataType Type, ref int Address, ref int dData, ref object ExData)
        {
            if (ctC140 == null) return;
            if (Address == -1 && dData == -1) return;

            if (Chip.Number == 0) chipLED.PriC140 = 2;
            else chipLED.SecC140 = 2;

            int dAddr = (Address & 0x1ff);
            if (dAddr < 0x180)
            {
                pcmRegisterC140[Chip.Number][dAddr] = (byte)dData;

                if (
                    (Chip.Model == EnmVRModel.VirtualModel && (ctC140[Chip.Number] == null || !ctC140[Chip.Number].UseScci))
                    || (Chip.Model == EnmVRModel.RealModel && (scC140 != null && scC140[Chip.Number] != null))
                    )
                {
                    byte ch = (byte)(dAddr >> 4);
                    switch (dAddr & 0xf)
                    {
                        case 0x05:
                            if ((dData & 0x80) != 0)
                            {
                                pcmKeyOnC140[Chip.Number][ch] = true;
                            }
                            if (Chip.ChMasks[ch]) dData &= 0x7f;
                            break;
                    }
                }

                if ((dAddr & 0xf) == 0 || (dAddr & 0xf) == 1)
                {
                    int d = C140NowFadeoutVol[Chip.Number];// >> 3;
                    dData = Math.Max(dData - d, 0);
                    //dData = dData;
                }
            }

            //if (Chip.Model == EnmModel.VirtualModel)
            //{
            //    if (ctC140[Chip.Number] == null || !ctC140[Chip.Number].UseScci)
            //        mds.WriteC140((byte)Chip.Number, (uint)dAddr, (byte)dData);
            //}
            //else
            //{
            //    if (scC140 != null && scC140[Chip.Number] != null) scC140[Chip.Number].setRegister((int)dAddr, dData);
            //}
        }

        public void C140SetRegister(outDatum od, long Counter, int ChipID, int dAddr, int dData)
        {
            enq(od, Counter, C140[ChipID], EnmDataType.Normal, dAddr, dData, null);
        }

        public void C140SetRegister(outDatum od, long Counter, int ChipID, PackData[] data)
        {
            enq(od, Counter, C140[ChipID], EnmDataType.Block, -1, -1, data);
        }

        public void C140WritePCMData(outDatum od, long Counter, byte chipID, uint ROMSize, uint DataStart, uint DataLength, byte[] romdata, uint SrcStartAdr)
        {
            enq(od, Counter, C140[chipID], EnmDataType.Block, -1, -2, new object[] { ROMSize, DataStart, DataLength, romdata, SrcStartAdr });

            //if (model == EnmModel.VirtualModel)
            //    mds.WriteC140PCMData(chipID, ROMSize, DataStart, DataLength, romdata, SrcStartAdr);
            //else
            //{
            //    if (scC140 != null && scC140[chipID] != null)
            //    {
            //        // スタートアドレス設定
            //        scC140[chipID].setRegister(0x10000, (byte)(DataStart));
            //        scC140[chipID].setRegister(0x10001, (byte)(DataStart >> 8));
            //        scC140[chipID].setRegister(0x10002, (byte)(DataStart >> 16));
            //        // データ転送
            //        for (int cnt = 0; cnt < DataLength; cnt++)
            //        {
            //            scC140[chipID].setRegister(0x10004, romdata[SrcStartAdr + cnt]);
            //        }
            //        //scC140[chipID].setRegister(0x10006, (int)ROMSize);

            //        realChip.SendData();
            //    }
            //}
        }

        public void C140WriteType(Chip Chip, MDSound.c140.C140_TYPE type)
        {
            if (Chip.Model == EnmVRModel.RealModel)
            {
                if (scC140 != null && scC140[Chip.Number] != null)
                {
                    switch (type)
                    {
                        case MDSound.c140.C140_TYPE.SYSTEM2:
                            scC140[Chip.Number].setRegister(0x10008, 0);
                            break;
                        case MDSound.c140.C140_TYPE.SYSTEM21:
                            scC140[Chip.Number].setRegister(0x10008, 1);
                            break;
                        case MDSound.c140.C140_TYPE.ASIC219:
                            scC140[Chip.Number].setRegister(0x10008, 2);
                            break;
                    }
                }
            }
        }

        public void C140SoftReset(long Counter, int ChipID)
        {
            List<PackData> data = C140MakeSoftReset(ChipID);
            C140SetRegister(null, Counter, ChipID, data.ToArray());
        }

        public List<PackData> C140MakeSoftReset(int chipID)
        {
            List<PackData> data = new List<PackData>();
            int i;

            for (i = 0; i < 24; i++)
            {
                data.Add(new PackData(null, C140[chipID], EnmDataType.Normal, (i << 4) + 0x05, 0x00, null));// KeyOff
                data.Add(new PackData(null, C140[chipID], EnmDataType.Normal, (i << 4) + 0x00, 0x00, null));// L vol
                data.Add(new PackData(null, C140[chipID], EnmDataType.Normal, (i << 4) + 0x01, 0x00, null));// R vol
            }

            return data;
        }

        public void C140WriteClock(byte chipID, int clock)
        {
            if (scC140 != null && scC140[chipID] != null)
            {
                scC140[chipID].dClock = scC140[chipID].SetMasterClock((uint)clock);
                scC140[chipID].mul = (double)scC140[chipID].dClock / (double)clock;
            }
        }

        public void C140SetFadeoutVolume(long Counter, int v)
        {
            for (int i = 0; i < C140.Count; i++)
            {
                if (!C140[i].Use) continue;
                if (C140[i].Model == EnmVRModel.VirtualModel) continue;
                if (C140NowFadeoutVol[i] >> 4 == v >> 4) continue;

                C140NowFadeoutVol[i] = v;
                for (int c = 0; c < 24; c++)
                {
                    C140SetRegister(null, Counter, i, (c << 4) + 0, pcmRegisterC140[i][(c << 4) + 0]);
                    C140SetRegister(null, Counter, i, (c << 4) + 1, pcmRegisterC140[i][(c << 4) + 1]);
                }
            }
        }

        #endregion



        #region C352

        private void C352WriteRegisterControl(Chip Chip, EnmDataType type, int address, int data, object exData)
        {
            if (type == EnmDataType.Normal)
            {
                if (Chip.Model == EnmVRModel.VirtualModel)
                {
                    if (!ctC352[Chip.Number].UseScci && ctC352[Chip.Number].UseEmu)
                        mds.WriteC352(Chip.Index, (byte)Chip.Number, (uint)address, (uint)data);
                }
                if (Chip.Model == EnmVRModel.RealModel)
                {
                    if (scC352[Chip.Number] != null)
                        scC352[Chip.Number].setRegister(address, data);
                }
            }
            else if (type == EnmDataType.Block)
            {
                Audio.sm.SetInterrupt();

                try
                {
                    if (exData == null) return;

                    if (data == -1)
                    {
                        PackData[] pdata = (PackData[])exData;
                        if (Chip.Model == EnmVRModel.VirtualModel)
                        {
                            foreach (PackData dat in pdata)
                                mds.WriteC352(Chip.Index, (byte)Chip.Number, (uint)dat.Address, (byte)dat.Data);
                        }
                        if (Chip.Model == EnmVRModel.RealModel)
                        {
                            if (scC352[Chip.Number] != null)
                            {
                                foreach (PackData dat in pdata)
                                    scC352[Chip.Number].setRegister(dat.Address, dat.Data);
                            }
                        }
                    }
                    else
                    {
                        if (Chip.Model == EnmVRModel.VirtualModel)
                            mds.WriteC352PCMData(Chip.Index, (byte)Chip.Number, (uint)((object[])exData)[0], (uint)((object[])exData)[1], (uint)((object[])exData)[2], (byte[])((object[])exData)[3], (uint)((object[])exData)[4]);
                        // ROMSize, DataStart, DataLength, romdata, SrcStartAdr);
                        else
                        {
                            if (scC352 != null && scC352[Chip.Number] != null)
                            {
                                // スタートアドレス設定
                                scC352[Chip.Number].setRegister(0x10000, (byte)((uint)((object[])exData)[1]));
                                scC352[Chip.Number].setRegister(0x10001, (byte)((uint)((object[])exData)[1] >> 8));
                                scC352[Chip.Number].setRegister(0x10002, (byte)((uint)((object[])exData)[1] >> 16));
                                // データ転送
                                for (int cnt = 0; cnt < (uint)((object[])exData)[2]; cnt++)
                                {
                                    scC352[Chip.Number].setRegister(0x10004, ((byte[])((object[])exData)[3])[(uint)((object[])exData)[4] + cnt]);
                                }
                                //scC352[chipID].setRegister(0x10006, (int)ROMSize);

                                realChip.SendData();
                            }
                        }
                    }
                }
                finally
                {
                    Audio.sm.ResetInterrupt();
                }
            }
        }

        public void C352SetRegisterProcessing(ref long Counter, ref Chip Chip, ref EnmDataType Type, ref int Address, ref int dData, ref object ExData)
        {
            if (ctC352 == null) return;
            if (Address == -1 && dData == -1) return;

            if (Chip.Number == 0) chipLED.PriC352 = 2;
            else chipLED.SecC352 = 2;

        }

        public void C352SetRegister(outDatum od, long Counter, int ChipID, int dAddr, int dData)
        {
            enq(od, Counter, C352[ChipID], EnmDataType.Normal, dAddr, dData, null);
        }

        public void C352SetRegister(outDatum od, long Counter, int ChipID, PackData[] data)
        {
            enq(od, Counter, C352[ChipID], EnmDataType.Block, -1, -1, data);
        }

        public void C352WritePCMData(outDatum od, long Counter, byte chipID, uint ROMSize, uint DataStart, uint DataLength, byte[] romdata, uint SrcStartAdr)
        {
            enq(od, Counter, C352[chipID], EnmDataType.Block, -1, -2, new object[] { ROMSize, DataStart, DataLength, romdata, SrcStartAdr });
        }

        #endregion



        #region HuC6280

        private void HuC6280WriteRegisterControl(Chip Chip, EnmDataType type, int address, int data, object exData)
        {
            if (type == EnmDataType.Normal)
            {
                if (Chip.Model == EnmVRModel.VirtualModel)
                {
                    if (!ctHuC6280[Chip.Number].UseScci)
                    {
                        mds.WriteHuC6280(Chip.Index, (byte)Chip.Number, (byte)address, (byte)data);
                    }
                }
                if (Chip.Model == EnmVRModel.RealModel)
                {
                }
            }
            else if (type == EnmDataType.Block)
            {
                Audio.sm.SetInterrupt();

                try
                {
                    if (exData == null) return;

                    PackData[] pdata = (PackData[])exData;
                    if (Chip.Model == EnmVRModel.VirtualModel)
                    {
                        foreach (PackData dat in pdata)
                            mds.WriteHuC6280(dat.Chip.Index, (byte)dat.Chip.Number, (byte)dat.Address, (byte)dat.Data);
                    }
                    if (Chip.Model == EnmVRModel.RealModel)
                    {
                    }
                }
                finally
                {
                    Audio.sm.ResetInterrupt();
                }
            }
        }

        public void HuC6280SetRegisterProcessing(ref long Counter, ref Chip Chip, ref EnmDataType Type, ref int Address, ref int dData, ref object ExData)
        {
            if (ctHuC6280 == null) return;
            if (Address == -1 && dData == -1) return;

            if (Chip.Number == 0) chipLED.PriHuC = 2;
            else chipLED.SecHuC = 2;

            int reg = Address & 0xf;
            if (reg == 0)
            {
                Chip.currentCh = dData & 0x7;
            }
            else if (reg == 4)
            {
                if (Chip.ChMasks[Chip.currentCh]) dData &= 0x7f;
            }
        }

        public void HuC6280SetRegister(outDatum od, long Counter, int chipID, int dAddr, int dData)
        {
            enq(od, Counter, HuC6280[chipID], EnmDataType.Normal, dAddr, dData, null);

        }

        public void HuC6280SetRegister(outDatum od, long Counter, int chipID, PackData[] data)
        {
            enq(od, Counter, HuC6280[chipID], EnmDataType.Block, -1, -1, data);

        }

        public void HuC6280SoftReset(long Counter, int chipID)
        {
            List<PackData> data = HuC6280MakeSoftReset(chipID);
            HuC6280SetRegister(null, Counter, chipID, data.ToArray());
        }

        public void HuC6280SetMask(long Counter, int chipID, int ch, bool mask)
        {
            //mds.setHuC6280Mask(chipID, 1 << ch);
            //mds.resetHuC6280Mask(chipID, 1 << ch);
        }

        public void HuC6280WriteClock(byte chipID, int clock)
        {
            if (scHuC6280 != null && scHuC6280.Length > chipID && scHuC6280[chipID] != null)
            {
                scHuC6280[chipID].dClock = scHuC6280[chipID].SetMasterClock((uint)clock);
            }
        }

        public void HuC6280SetFadeoutVolume(long Counter, int v)
        {
            for (int i = 0; i < HuC6280.Count; i++)
            {
                if (!HuC6280[i].Use) continue;
                if (HuC6280[i].Model == EnmVRModel.VirtualModel) continue;
                if (HuC6280NowFadeoutVol[i] == v) continue;

                HuC6280NowFadeoutVol[i] = v;

                for (int c = 0; c < 3; c++)
                {
                }
            }
        }

        public List<PackData> HuC6280MakeSoftReset(int chipID)
        {
            List<PackData> data = new List<PackData>();
            int i;

            for (i = 0; i < 6; i++)
            {
                data.Add(new PackData(null, HuC6280[chipID], EnmDataType.Normal, 0x00, i, null));
                data.Add(new PackData(null, HuC6280[chipID], EnmDataType.Normal, 0x04, 0x00, null));
            }
            data.Add(new PackData(null, HuC6280[chipID], EnmDataType.Normal, 0x01, 0x00, null));//TotalVolume0

            return data;
        }

        #endregion



        #region K051649(SCC)

        private void K051649WriteRegisterControl(Chip Chip, EnmDataType type, int address, int data, object exData)
        {
            if (type == EnmDataType.Normal)
            {
                if (Chip.Model == EnmVRModel.VirtualModel)
                {
                    if ((address & 1) != 0)
                    {
                        if ((address >> 1) == 3)//keyonoff
                        {
                            K051649tKeyOnOff[Chip.Number] = (byte)data;
                            data &= (byte)(maskChK051649[Chip.Number][0] ? 0xfe : 0xff);
                            data &= (byte)(maskChK051649[Chip.Number][1] ? 0xfd : 0xff);
                            data &= (byte)(maskChK051649[Chip.Number][2] ? 0xfb : 0xff);
                            data &= (byte)(maskChK051649[Chip.Number][3] ? 0xf7 : 0xff);
                            data &= (byte)(maskChK051649[Chip.Number][4] ? 0xef : 0xff);
                        }
                    }
                    if (!ctK051649[Chip.Number].UseScci)
                    {
                        mds.WriteK051649(Chip.Index, (byte)Chip.Number, (byte)address, (byte)data);
                    }
                }
                if (Chip.Model == EnmVRModel.RealModel)
                {
                }
            }
            else if (type == EnmDataType.Block)
            {
                Audio.sm.SetInterrupt();

                try
                {
                    if (exData == null) return;

                    PackData[] pdata = (PackData[])exData;
                    if (Chip.Model == EnmVRModel.VirtualModel)
                    {
                        foreach (PackData dat in pdata)
                            mds.WriteK051649(dat.Chip.Index, (byte)dat.Chip.Number, (byte)dat.Address, (byte)dat.Data);
                    }
                    if (Chip.Model == EnmVRModel.RealModel)
                    {
                    }
                }
                finally
                {
                    Audio.sm.ResetInterrupt();
                }
            }
        }

        public void K051649SetRegisterProcessing(ref long Counter, ref Chip Chip, ref EnmDataType Type, ref int Address, ref int dData, ref object ExData)
        {
            if (ctK051649 == null) return;
            if (Address == -1 && dData == -1) return;

            if (Chip.Number == 0) chipLED.PriK051649 = 2;
            else chipLED.SecK051649 = 2;

            if ((Address & 1) == 1)
            {
                if ((Address >> 1) == 0x03)
                {
                    dData =
                        (Chip.ChMasks[0] ? 0 : (dData & 1))
                        | (Chip.ChMasks[1] ? 0 : (dData & 2))
                        | (Chip.ChMasks[2] ? 0 : (dData & 4))
                        | (Chip.ChMasks[3] ? 0 : (dData & 8))
                        | (Chip.ChMasks[4] ? 0 : (dData & 16))
                        ;
                }
            }
        }

        public void K051649SetRegister(outDatum od, long Counter, int chipID, int dAddr, int dData)
        {
            enq(od, Counter, K051649[chipID], EnmDataType.Normal, dAddr, dData, null);

        }

        public void K051649SetRegister(outDatum od, long Counter, int chipID, PackData[] data)
        {
            enq(od, Counter, K051649[chipID], EnmDataType.Block, -1, -1, data);

        }

        public void K051649SoftReset(long Counter, int chipID)
        {
            List<PackData> data = K051649MakeSoftReset(chipID);
            K051649SetRegister(null, Counter, chipID, data.ToArray());
        }

        public void K051649SetMask(long Counter, int chipID, int ch, bool mask)
        {
            if (mask)
            {
                //set
                //maskChK051649[chipID][ch] = true;
                //writeK051649((byte)chipID, (3 << 1) | 1, K051649tKeyOnOff[chipID]);
            }
            else
            {
                //reset
                //maskChK051649[chipID][ch] = false;
                //writeK051649((byte)chipID, (3 << 1) | 1, K051649tKeyOnOff[chipID]);
            }
        }

        public void K051649WriteClock(byte chipID, int clock)
        {
            if (scK051649 != null && scK051649.Length > chipID && scK051649[chipID] != null)
            {
                scK051649[chipID].dClock = scK051649[chipID].SetMasterClock((uint)clock);
            }
        }

        public void K051649SetFadeoutVolume(long Counter, int v)
        {
            for (int i = 0; i < K051649.Count; i++)
            {
                if (!K051649[i].Use) continue;
                if (K051649[i].Model == EnmVRModel.VirtualModel) continue;
                if (K051649NowFadeoutVol[i] == v) continue;

                K051649NowFadeoutVol[i] = v;

            }
        }

        public List<PackData> K051649MakeSoftReset(int chipID)
        {
            List<PackData> data = new List<PackData>();
            data.Add(new PackData(null, K051649[chipID], EnmDataType.Normal, 0x07, 0x00, null));

            return data;
        }

        #endregion



        #region K053260

        private void K053260WriteRegisterControl(Chip Chip, EnmDataType type, int address, int data, object exData)
        {
            if (type == EnmDataType.Normal)
            {
                if (Chip.Model == EnmVRModel.VirtualModel)
                {
                    if (!ctK053260[Chip.Number].UseScci)
                    {
                        mds.WriteK053260(Chip.Index, (byte)Chip.Number, (byte)address, (byte)data);
                    }
                }
                if (Chip.Model == EnmVRModel.RealModel)
                {
                }
            }
            else if (type == EnmDataType.Block)
            {
                Audio.sm.SetInterrupt();

                try
                {
                    if (exData == null) return;

                    if (data == -1)
                    {
                        PackData[] pdata = (PackData[])exData;
                        if (Chip.Model == EnmVRModel.VirtualModel)
                        {
                            foreach (PackData dat in pdata)
                                mds.WriteK053260(dat.Chip.Index, (byte)dat.Chip.Number, (byte)dat.Address, (byte)dat.Data);
                        }
                        if (Chip.Model == EnmVRModel.RealModel)
                        {
                        }
                    }
                    else
                    {
                        if (Chip.Model == EnmVRModel.VirtualModel)
                            mds.WriteK053260PCMData(Chip.Index, (byte)Chip.Number, (uint)((object[])exData)[0], (uint)((object[])exData)[1], (uint)((object[])exData)[2], (byte[])((object[])exData)[3], (uint)((object[])exData)[4]);
                        else
                        {
                        }
                    }
                }
                finally
                {
                    Audio.sm.ResetInterrupt();
                }
            }
        }

        public void K053260SetRegisterProcessing(ref long Counter, ref Chip Chip, ref EnmDataType Type, ref int Address, ref int dData, ref object ExData)
        {
            if (ctK053260 == null) return;
            if (Address == -1 && dData == -1) return;

            if (Chip.Number == 0) chipLED.PriK053260 = 2;
            else chipLED.SecK053260 = 2;

            if (Address == 0x28)
            {
                dData =
                    (Chip.ChMasks[0] ? 0 : (dData & 1))
                    | (Chip.ChMasks[1] ? 0 : (dData & 2))
                    | (Chip.ChMasks[2] ? 0 : (dData & 4))
                    | (Chip.ChMasks[3] ? 0 : (dData & 8))
                    ;
            }
        }

        public void K053260SetRegister(outDatum od, long Counter, int ChipID, int dAddr, int dData)
        {
            enq(od, Counter, K053260[ChipID], EnmDataType.Normal, dAddr, dData, null);
        }

        public void K053260SetRegister(outDatum od, long Counter, int ChipID, PackData[] data)
        {
            enq(od, Counter, K053260[ChipID], EnmDataType.Block, -1, -1, data);
        }

        public void K053260WritePCMData(outDatum od, long Counter, byte chipID, uint ROMSize, uint DataStart, uint DataLength, byte[] romdata, uint SrcStartAdr)
        {
            enq(od, Counter, K053260[chipID], EnmDataType.Block, -1, -2, new object[] { ROMSize, DataStart, DataLength, romdata, SrcStartAdr });
        }

        public void K053260SoftReset(long Counter, int chipID)
        {
            List<PackData> data = K053260MakeSoftReset(chipID);
            K053260SetRegister(null, Counter, chipID, data.ToArray());
        }

        public void K053260SetMask(long Counter, int chipID, int ch, bool mask)
        {
            if (mask)
            {
            }
            else
            {
            }
        }

        public void K053260WriteClock(byte chipID, int clock)
        {
            if (scK053260 != null && scK053260.Length > chipID && scK053260[chipID] != null)
            {
                scK053260[chipID].dClock = scK053260[chipID].SetMasterClock((uint)clock);
            }
        }

        public void K053260SetFadeoutVolume(long Counter, int v)
        {
            for (int i = 0; i < K053260.Count; i++)
            {
                if (!K053260[i].Use) continue;
                if (K053260[i].Model == EnmVRModel.VirtualModel) continue;
                if (K053260NowFadeoutVol[i] == v) continue;

                K053260NowFadeoutVol[i] = v;

            }
        }

        public List<PackData> K053260MakeSoftReset(int chipID)
        {
            List<PackData> data = new List<PackData>();

            data.Add(new PackData(null, K053260[chipID], EnmDataType.Normal, 0x28, 0x00, null));// KeyOff
            for (int i = 0; i < 4; i++)
            {
                data.Add(new PackData(null, K053260[chipID], EnmDataType.Normal, (i + 1) * 8 + 7, 0x00, null));// volume:0
            }

            return data;
        }

        #endregion



        #region PPZ8

        private void PPZ8WriteRegisterControl(Chip Chip, EnmDataType type, int address, int data, object exData)
        {
            if (type == EnmDataType.Normal)
            {
                if (Chip.Model == EnmVRModel.VirtualModel)
                {
                    if (!ctPPZ8[Chip.Number].UseScci)
                    {
                        if (exData is int)
                            mds.WritePPZ8(Chip.Index, (byte)Chip.Number, address, data, (int)exData, null);
                    }
                }
                if (Chip.Model == EnmVRModel.RealModel)
                {
                }
            }
            else if (type == EnmDataType.Block)
            {
                Audio.sm.SetInterrupt();

                try
                {
                    if (exData == null) return;

                    if (data == -1)
                    {
                        PackData[] pdata = (PackData[])exData;
                        if (Chip.Model == EnmVRModel.VirtualModel)
                        {
                            foreach (PackData dat in pdata)
                            {
                                ;
                                if (dat.Type== EnmDataType.Normal)
                                {
                                    mds.WritePPZ8(dat.Chip.Index, (byte)dat.Chip.Number
                                        , dat.Address, dat.Data, (int)dat.ExData, null);
                                }
                                else
                                {
                                    mds.WritePPZ8PCMData(dat.Chip.Index, (byte)dat.Chip.Number
                                        , (byte)dat.Address
                                        , (byte)dat.Data
                                        , (byte[][])dat.ExData
                                        );

                                }
                            }
                        }
                        if (Chip.Model == EnmVRModel.RealModel)
                        {
                        }
                    }
                    else
                    {
                        if (Chip.Model == EnmVRModel.VirtualModel)
                            mds.WritePPZ8PCMData(Chip.Index, (byte)Chip.Number
                                , (byte)((object[])exData)[0]
                                , (byte)((object[])exData)[1]
                                , (byte[][])((object[])exData)[2]
                                );
                        else
                        {
                        }
                    }
                }
                finally
                {
                    Audio.sm.ResetInterrupt();
                }
            }
        }

        public void PPZ8SetRegisterProcessing(ref long Counter, ref Chip Chip, ref EnmDataType Type, ref int Address, ref int dData, ref object ExData)
        {
            if (ctPPZ8 == null) return;
            if (Address == -1 && dData == -1) return;

            if (Chip.Number == 0) chipLED.PriPPZ8 = 2;

        }

        public void PPZ8SetRegister(outDatum od, long Counter, Chip chip, PackData[] data)
        {
            enq(od, Counter, chip, EnmDataType.Block, -1, -1, data);
        }

        public void PPZ8SoftReset(long Counter, int chipID)
        {
            List<PackData> data = PPZ8MakeSoftReset(chipID);
            PPZ8SetRegister(null, Counter, PPZ8[chipID], data.ToArray());
        }

        public List<PackData> PPZ8MakeSoftReset(int chipID)
        {
            List<PackData> data = new List<PackData>();
            for (int i = 0; i < 8; i++)
            {
                data.Add(new PackData(null, PPZ8[chipID], EnmDataType.Normal, 0x02, i, 0));
            }

            return data;
        }

        #endregion



        #region PPSDRV

        private void PPSDRVWriteRegisterControl(Chip Chip, EnmDataType type, int address, int data, object exData)
        {
            if (type == EnmDataType.Normal)
            {
                if (Chip.Model == EnmVRModel.VirtualModel)
                {
                    if (!ctPPSDRV[Chip.Number].UseScci)
                    {
                        if(exData is int)
                            mds.WritePPSDRV(Chip.Index, (byte)Chip.Number, address, data, (int)exData, null);
                    }
                }
                if (Chip.Model == EnmVRModel.RealModel)
                {
                }
            }
            else if (type == EnmDataType.Block)
            {
                Audio.sm.SetInterrupt();

                try
                {
                    if (exData == null) return;

                    if (data == -1)
                    {
                        PackData[] pdata = (PackData[])exData;
                        if (Chip.Model == EnmVRModel.VirtualModel)
                        {
                            foreach (PackData dat in pdata)
                            {
                                ;
                                if (dat.Type == EnmDataType.Normal)
                                {
                                    mds.WritePPSDRV(dat.Chip.Index, (byte)dat.Chip.Number
                                        , dat.Address, dat.Data, (int)dat.ExData, null);
                                }
                                else
                                {
                                    mds.WritePPSDRVPCMData(dat.Chip.Index, (byte)dat.Chip.Number
                                        , (byte[])dat.ExData
                                        );

                                }
                            }
                        }
                        if (Chip.Model == EnmVRModel.RealModel)
                        {
                        }
                    }
                    else
                    {
                        if (Chip.Model == EnmVRModel.VirtualModel)
                            mds.WritePPSDRVPCMData(Chip.Index, (byte)Chip.Number
                                , (byte[])((object[])exData)[0]
                                );
                        else
                        {
                        }
                    }
                }
                finally
                {
                    Audio.sm.ResetInterrupt();
                }
            }
        }

        public void PPSDRVSetRegisterProcessing(ref long Counter, ref Chip Chip, ref EnmDataType Type, ref int Address, ref int dData, ref object ExData)
        {
            if (ctPPSDRV == null) return;
            if (Address == -1 && dData == -1) return;

            if (Chip.Number == 0) chipLED.PriPPSDRV = 2;

        }

        public void PPSDRVSetRegister(outDatum od, long Counter, Chip chip, PackData[] data)
        {
            enq(od, Counter, chip, EnmDataType.Block, -1, -1, data);
        }

        public void PPSDRVSoftReset(long Counter, int chipID)
        {
            List<PackData> data = PPSDRVMakeSoftReset(chipID);
            PPSDRVSetRegister(null, Counter, PPSDRV[chipID], data.ToArray());
        }

        public List<PackData> PPSDRVMakeSoftReset(int chipID)
        {
            List<PackData> data = new List<PackData>();
            data.Add(new PackData(null, PPSDRV[chipID], EnmDataType.Normal, 0x02, 0, 0));

            return data;
        }


        #endregion



        #region P86

        private void P86WriteRegisterControl(Chip Chip, EnmDataType type, int address, int data, object exData)
        {
            if (type == EnmDataType.Normal)
            {
                if (Chip.Model == EnmVRModel.VirtualModel)
                {
                    if (!ctP86[Chip.Number].UseScci)
                    {
                        if (exData is int)
                            mds.WriteP86(Chip.Index, (byte)Chip.Number, address, data, (int)exData, null);
                    }
                }
                if (Chip.Model == EnmVRModel.RealModel)
                {
                }
            }
            else if (type == EnmDataType.Block)
            {
                Audio.sm.SetInterrupt();

                try
                {
                    if (exData == null) return;

                    if (data == -1)
                    {
                        PackData[] pdata = (PackData[])exData;
                        if (Chip.Model == EnmVRModel.VirtualModel)
                        {
                            foreach (PackData dat in pdata)
                            {
                                ;
                                if (dat.Type == EnmDataType.Normal)
                                {
                                    mds.WriteP86(dat.Chip.Index, (byte)dat.Chip.Number
                                        , dat.Address, dat.Data, (int)dat.ExData, null);
                                }
                                else
                                {
                                    mds.WriteP86PCMData(dat.Chip.Index, (byte)dat.Chip.Number
                                        , (byte)dat.Address
                                        , (byte)dat.Data
                                        , (byte[])dat.ExData
                                        );

                                }
                            }
                        }
                        if (Chip.Model == EnmVRModel.RealModel)
                        {
                        }
                    }
                    else
                    {
                        if (Chip.Model == EnmVRModel.VirtualModel)
                            mds.WriteP86PCMData(Chip.Index, (byte)Chip.Number
                                , (byte)((object[])exData)[0]
                                , (byte)((object[])exData)[1]
                                , (byte[])((object[])exData)[2]
                                );
                        else
                        {
                        }
                    }
                }
                finally
                {
                    Audio.sm.ResetInterrupt();
                }
            }
        }

        public void P86SetRegisterProcessing(ref long Counter, ref Chip Chip, ref EnmDataType Type, ref int Address, ref int dData, ref object ExData)
        {
            if (ctP86 == null) return;
            if (Address == -1 && dData == -1) return;

            if (Chip.Number == 0) chipLED.PriP86 = 2;

        }

        public void P86SetRegister(outDatum od, long Counter, Chip chip, PackData[] data)
        {
            enq(od, Counter, chip, EnmDataType.Block, -1, -1, data);
        }

        public void P86SoftReset(long Counter, int chipID)
        {
            List<PackData> data = P86MakeSoftReset(chipID);
            P86SetRegister(null, Counter, P86[chipID], data.ToArray());
        }

        public List<PackData> P86MakeSoftReset(int chipID)
        {
            List<PackData> data = new List<PackData>();
            data.Add(new PackData(null, P86[chipID], EnmDataType.Normal, 0x09, 0, 0));

            return data;
        }

        #endregion



        #region QSound

        private void QSoundWriteRegisterControl(Chip Chip, EnmDataType type, int address, int data, object exData)
        {
            if (type == EnmDataType.Normal)
            {
                if (Chip.Model == EnmVRModel.VirtualModel)
                {
                    if (!ctQSound[Chip.Number].UseScci)
                    {
                        mds.WriteQSoundCtr(Chip.Index, (byte)Chip.Number, 0, (byte)(data >> 8));
                        mds.WriteQSoundCtr(Chip.Index, (byte)Chip.Number, 1, (byte)data);
                        mds.WriteQSoundCtr(Chip.Index, (byte)Chip.Number, 2, (byte)address);
                    }
                }
                if (Chip.Model == EnmVRModel.RealModel)
                {
                }
            }
            else if (type == EnmDataType.Block)
            {
                Audio.sm.SetInterrupt();

                try
                {
                    if (exData == null) return;

                    if (data == -1)
                    {
                        PackData[] pdata = (PackData[])exData;
                        if (Chip.Model == EnmVRModel.VirtualModel)
                        {
                            foreach (PackData dat in pdata)
                            {
                                mds.WriteQSoundCtr(Chip.Index, (byte)dat.Chip.Number, 0, (byte)(dat.Data >> 8));
                                mds.WriteQSoundCtr(Chip.Index, (byte)dat.Chip.Number, 1, (byte)dat.Data);
                                mds.WriteQSoundCtr(Chip.Index, (byte)dat.Chip.Number, 2, (byte)dat.Address);
                            }
                        }
                        if (Chip.Model == EnmVRModel.RealModel)
                        {
                        }
                    }
                    else
                    {
                        if (Chip.Model == EnmVRModel.VirtualModel)
                            mds.WriteQSoundCtrPCMData(Chip.Index, (byte)Chip.Number, (uint)((object[])exData)[0], (uint)((object[])exData)[1], (uint)((object[])exData)[2], (byte[])((object[])exData)[3], (uint)((object[])exData)[4]);
                        else
                        {
                        }
                    }
                }
                finally
                {
                    Audio.sm.ResetInterrupt();
                }
            }
        }

        public void QSoundSetRegisterProcessing(ref long Counter, ref Chip Chip, ref EnmDataType Type, ref int Address, ref int dData, ref object ExData)
        {
            if (ctQSound == null) return;
            if (Address == -1 && dData == -1) return;

            if (Chip.Number == 0) chipLED.PriQsnd = 2;

            if (Address < 0x80)
            {
                if ((Address & 0x7) == 3)
                {
                    int ch = Address >> 3;
                    dData = Chip.ChMasks[ch] ? 0 : dData;
                }
            }
        }

        public void QSoundSetRegister(outDatum od, long Counter, int ChipID, int dAddr, int dData)
        {
            enq(od, Counter, QSound[ChipID], EnmDataType.Normal, dAddr, dData, null);
        }

        public void QSoundSetRegister(outDatum od, long Counter, int ChipID, PackData[] data)
        {
            enq(od, Counter, QSound[ChipID], EnmDataType.Block, -1, -1, data);
        }

        public void QSoundWritePCMData(outDatum od, long Counter, byte chipID, uint ROMSize, uint DataStart, uint DataLength, byte[] romdata, uint SrcStartAdr)
        {
            enq(od, Counter, QSound[chipID], EnmDataType.Block, -1, -2, new object[] { ROMSize, DataStart, DataLength, romdata, SrcStartAdr });
        }

        public void QSoundSoftReset(long Counter, int chipID)
        {
            List<PackData> data = QSoundMakeSoftReset(chipID);
            QSoundSetRegister(null, Counter, chipID, data.ToArray());
        }

        public void QSoundSetMask(long Counter, int chipID, int ch, bool mask)
        {
            if (mask)
            {
            }
            else
            {
            }
        }

        public void QSoundWriteClock(byte chipID, int clock)
        {
            if (scQSound != null && scQSound.Length > chipID && scQSound[chipID] != null)
            {
                scQSound[chipID].dClock = scQSound[chipID].SetMasterClock((uint)clock);
            }
        }

        public void QSoundSetFadeoutVolume(long Counter, int v)
        {
            for (int i = 0; i < QSound.Count; i++)
            {
                if (!QSound[i].Use) continue;
                if (QSound[i].Model == EnmVRModel.VirtualModel) continue;
                if (QSoundNowFadeoutVol[i] == v) continue;

                QSoundNowFadeoutVol[i] = v;

            }
        }

        public List<PackData> QSoundMakeSoftReset(int chipID)
        {
            List<PackData> data = new List<PackData>();

            for (int i = 0; i < 16; i++)
            {
                data.Add(new PackData(null, QSound[chipID], EnmDataType.Normal, (i << 3) + 0x03, 0x00, null));// KeyOff
                data.Add(new PackData(null, QSound[chipID], EnmDataType.Normal, (i << 3) + 0x06, 0x00, null));// volume:0
            }

            return data;
        }

        #endregion



        #region RF5C164

        public void writeRF5C68PCMData(byte chipid, uint stAdr, uint dataSize, byte[] vgmBuf, uint vgmAdr)
        {
            EnmVRModel model = EnmVRModel.VirtualModel;
            //if (chipid == 0) chipLED.PriRF5C = 2;
            //else chipLED.SecRF5C = 2;

            if (model == EnmVRModel.VirtualModel)
                mds.WriteRF5C68PCMData(chipid, stAdr, dataSize, vgmBuf, vgmAdr);
        }

        public void writeRF5C68(byte chipid, byte adr, byte data)
        {
            EnmVRModel model = EnmVRModel.VirtualModel;

            if (model == EnmVRModel.VirtualModel)
            {
                mds.WriteRF5C68(chipid, adr, data);
            }
        }

        public void writeRF5C68MemW(byte chipid, uint offset, byte data)
        {
            EnmVRModel model = EnmVRModel.VirtualModel;
            if (model == EnmVRModel.VirtualModel)
                mds.WriteRF5C68MemW(chipid, offset, data);
        }

        private void RF5C164WriteRegisterControl(Chip Chip, EnmDataType type, int address, int data, object exData)
        {
            if (type == EnmDataType.Normal)
            {
                if (Chip.Model == EnmVRModel.VirtualModel)
                {
                    //if (!ctRF5C164[Chip.Number].UseScci && ctRF5C164[Chip.Number].UseEmu)
                    //{
                    mds.WriteRF5C164(Chip.Index, (byte)Chip.Number, (byte)address, (byte)data);
                    //}
                }
                if (Chip.Model == EnmVRModel.RealModel)
                {
                    ;
                }
            }
            else if (type == EnmDataType.Block)
            {
                Audio.sm.SetInterrupt();

                try
                {
                    if (exData == null) return;

                    if (exData is PackData[])
                    {
                        PackData[] pdata = (PackData[])exData;
                        if (Chip.Model == EnmVRModel.VirtualModel)
                        {
                            if (data == -1)
                            {
                                foreach (PackData dat in pdata)
                                    mds.WriteRF5C164(dat.Chip.Index, (byte)dat.Chip.Number, (byte)dat.Address, (byte)dat.Data);
                            }
                            else
                            {
                                foreach (PackData dat in pdata)
                                    mds.WriteRF5C164MemW(dat.Chip.Index, (byte)dat.Chip.Number, (byte)dat.Address, (byte)dat.Data);
                            }
                        }
                        if (Chip.Model == EnmVRModel.RealModel)
                        {
                            ;
                        }
                        return;
                    }

                    uint stAdr = (uint)((object[])exData)[0];
                    uint dataSize = (uint)((object[])exData)[1];
                    byte[] pcmData = (byte[])((object[])exData)[2];
                    uint vgmAdr = (uint)((object[])exData)[3];

                    if (Chip.Model == EnmVRModel.VirtualModel)
                    {
                        log.Write("Sending RF5C164(Emu) PCM");
                        mds.WriteRF5C164PCMData(Chip.Index, (byte)Chip.Number, stAdr, dataSize, pcmData, vgmAdr);
                    }
                    if (Chip.Model == EnmVRModel.RealModel)
                    {
                        ;
                    }
                }
                finally
                {
                    Audio.sm.ResetInterrupt();
                }
            }
        }

        public void RF5C164SetRegisterProcessing(ref long Counter, ref Chip Chip, ref EnmDataType Type, ref int Address, ref int dData, ref object ExData)
        {
            //if (ctRF5C164 == null) return;
            if (Address == -1 && dData == -1) return;

            if (Chip.Number == 0) chipLED.PriRF5C = 2;
            else chipLED.SecRF5C = 2;

            if ((Address & 0xf) == 8)
            {
                dData =
                    ((Chip.ChMasks[0] ? 1 : (dData & 1))
                    | (Chip.ChMasks[1] ? 2 : (dData & 2))
                    | (Chip.ChMasks[2] ? 4 : (dData & 4))
                    | (Chip.ChMasks[3] ? 8 : (dData & 8))

                    | (Chip.ChMasks[4] ? 16 : (dData & 16))
                    | (Chip.ChMasks[5] ? 32 : (dData & 32))
                    | (Chip.ChMasks[6] ? 64 : (dData & 64))
                    | (Chip.ChMasks[7] ? 128 : (dData & 128)))
                    ;
            }

        }

        public void RF5C164SetRegister(outDatum od, long Counter, byte ChipID, byte dAddr, byte dData)
        {
            enq(od, Counter, RF5C164[ChipID], EnmDataType.Normal, dAddr, dData, null);
        }

        public void RF5C164SetRegister(outDatum od, long Counter, int ChipID, PackData[] data)
        {
            enq(od, Counter, RF5C164[ChipID], EnmDataType.Block, -1, -1, data);
        }

        public void RF5C164WritePCMData(outDatum od, long Counter, byte ChipID, uint stAdr, uint dataSize, byte[] vgmBuf, uint vgmAdr)
        {
            enq(od, Counter, RF5C164[ChipID], EnmDataType.Block, -1, -2, new object[] { stAdr, dataSize, vgmBuf, vgmAdr });

        }

        public void writeRF5C164MemW(outDatum od, long Counter, byte ChipID, uint offset, byte data)
        {
            enq(od, Counter, RF5C164[ChipID], EnmDataType.Block, -1, -2, new PackData[] { new PackData(od, RF5C164[ChipID], EnmDataType.Normal, (int)offset, data, null) });
        }

        public void RF5C164SoftReset(long Counter, int ChipID)
        {
            List<PackData> data = RF5C164MakeSoftReset(ChipID);
            RF5C164SetRegister(null, Counter, ChipID, data.ToArray());
        }

        public List<PackData> RF5C164MakeSoftReset(int chipID)
        {
            List<PackData> data = new List<PackData>();
            data.Add(new PackData(null, RF5C164[chipID], EnmDataType.Normal, 0x8, 0xff, null));

            return data;
        }

        public void RF5C164WriteClock(byte chipID, int clock)
        {
            if (scRF5C164 != null && scRF5C164.Length > chipID && scRF5C164[chipID] != null)
            {
                scRF5C164[chipID].dClock = scRF5C164[chipID].SetMasterClock((uint)clock);
                scRF5C164[chipID].mul = (double)scRF5C164[chipID].dClock / (double)clock;
            }
        }

        public void RF5C164SetFadeoutVolume(long Counter, int v)
        {
            for (int i = 0; i < RF5C164.Count; i++)
            {
                if (!RF5C164[i].Use) continue;
                if (RF5C164[i].Model == EnmVRModel.VirtualModel) continue;
                if (RF5C164NowFadeoutVol[i] >> 4 == v >> 4) continue;

                RF5C164NowFadeoutVol[i] = v;
                for (int c = 0; c < 24; c++)
                {
                    //RF5C164SetRegister(null, Counter, i, (c << 4) + 0, pcmRegisterC140[i][(c << 4) + 0]);
                    //RF5C164SetRegister(null, Counter, i, (c << 4) + 1, pcmRegisterC140[i][(c << 4) + 1]);
                }
            }
        }

        #endregion



        #region YM2151(OPM)

        private void YM2151WriteRegisterControl(Chip Chip, EnmDataType type, int address, int data, object exData)
        {
            if (type == EnmDataType.Normal)
            {
                if (Chip.Model == EnmVRModel.VirtualModel)
                {
                    if (!ctYM2151[Chip.Number].UseScci)
                    {
                        if (ctYM2151[Chip.Number].UseEmu) mds.WriteYM2151(Chip.Index, (byte)Chip.Number, (byte)address, (byte)data);
                        if (ctYM2151[Chip.Number].UseEmu2) mds.WriteYM2151mame(Chip.Index, (byte)Chip.Number, (byte)address, (byte)data);
                        if (ctYM2151[Chip.Number].UseEmu3) mds.WriteYM2151x68sound(Chip.Index, (byte)Chip.Number, (byte)address, (byte)data);
                    }
                }
                if (Chip.Model == EnmVRModel.RealModel)
                {
                    if (scYM2151[Chip.Number] != null)
                    {
                        if (Chip.Hosei == 0 || (address < 0x28 || address > 0x2f))
                        {
                            scYM2151[Chip.Number].setRegister(address, data);
                        }
                        else
                        {
                            int oct = (data & 0x70) >> 4;
                            int note = data & 0xf;
                            note = (note < 3) ? note : ((note < 7) ? (note - 1) : ((note < 11) ? (note - 2) : (note - 3)));
                            note += Chip.Hosei - 1;
                            if (note < 0)
                            {
                                oct += (note / 12) - 1;
                                note = (note % 12) + 12;
                            }
                            else
                            {
                                oct += (note / 12);
                                note %= 12;
                            }

                            note = (note < 3) ? note : ((note < 6) ? (note + 1) : ((note < 9) ? (note + 2) : (note + 3)));
                            scYM2151[Chip.Number].setRegister(address, (oct << 4) | note);
                        }
                    }
                }
            }
            else if (type == EnmDataType.Block)
            {
                Audio.sm.SetInterrupt();

                try
                {
                    if (exData == null) return;

                    PackData[] pdata = (PackData[])exData;
                    if (Chip.Model == EnmVRModel.VirtualModel)
                    {
                        if (ctYM2151[Chip.Number].UseEmu) foreach (PackData dat in pdata) mds.WriteYM2151(Chip.Index, (byte)dat.Chip.Number, (byte)dat.Address, (byte)dat.Data);
                        if (ctYM2151[Chip.Number].UseEmu2) foreach (PackData dat in pdata) mds.WriteYM2151mame(Chip.Index, (byte)dat.Chip.Number, (byte)dat.Address, (byte)dat.Data);
                        if (ctYM2151[Chip.Number].UseEmu3) foreach (PackData dat in pdata) mds.WriteYM2151x68sound(Chip.Index, (byte)dat.Chip.Number, (byte)dat.Address, (byte)dat.Data);
                    }
                    if (Chip.Model == EnmVRModel.RealModel)
                    {
                        if (scYM2151[Chip.Number] != null)
                        {
                            foreach (PackData dat in pdata)
                                scYM2151[Chip.Number].setRegister(dat.Address, dat.Data);
                        }
                    }
                }
                finally
                {
                    Audio.sm.ResetInterrupt();
                }
            }
        }

        public void YM2151SetRegisterProcessing(ref long Counter, ref Chip Chip, ref EnmDataType Type, ref int Address, ref int dData, ref object ExData)
        {
            if (ctYM2151 == null) return;
            if (Address == -1 && dData == -1) return;

            if (Chip.Number == 0) chipLED.PriOPM = 2;
            else chipLED.SecOPM = 2;

            //int dPort = (byte)(Address >> 8);
            int dAddr = (byte)Address;

            YM2151FmRegister[Chip.Number][dAddr] = dData;

            if (Chip.Model == EnmVRModel.VirtualModel)
            {
                //midiExport.outMIDIData(EnmChip.YM2151, Chip.Number, dPort, dAddr, dData, Chip.Hosei, Counter);
            }

            if ((Chip.Model == EnmVRModel.RealModel && ctYM2151[Chip.Number].UseScci) || (Chip.Model == EnmVRModel.VirtualModel && !ctYM2151[Chip.Number].UseScci))
            {
                if (dAddr == 0x08) //Key-On/Off
                {
                    int ch = dData & 0x7;
                    if (ch >= 0 && ch < 8)
                    {
                        if ((dData & 0x78) != 0)
                        {
                            byte con = (byte)(dData & 0x78);
                            YM2151FmKeyOn[Chip.Number][ch] = con | 1;
                            YM2151FmVol[Chip.Number][ch] = 256 * 6;
                        }
                        else
                        {
                            YM2151FmKeyOn[Chip.Number][ch] &= 0xfe;
                        }
                    }
                }
            }

            //AMD/PMD
            if (dAddr == 0x19)
            {
                if ((dData & 0x80) != 0)
                {
                    YM2151FmPMD[Chip.Number] = dData & 0x7f;
                }
                else
                {
                    YM2151FmAMD[Chip.Number] = dData & 0x7f;
                }
            }

            if ((dAddr & 0xf8) == 0x20)
            {
                int al = dData & 0x07;//AL
                int ch = (dAddr & 0x7);

                for (int i = 0; i < 4; i++)
                {
                    int slot = (i == 0) ? 0 : ((i == 1) ? 2 : ((i == 2) ? 1 : 3));
                    if ((algM[al] & (1 << slot)) > 0)
                    {
                        if (YM2151MaskFMCh[Chip.Number][ch])
                        {
                            if (Chip.Model == EnmVRModel.VirtualModel)
                            {
                                if (!ctYM2151[Chip.Number].UseScci)
                                {
                                    if (ctYM2151[Chip.Number].UseEmu) mds.WriteYM2151(Chip.Index, (byte)Chip.Number, (byte)(0x60 + i * 8 + ch), (byte)127);
                                    if (ctYM2151[Chip.Number].UseEmu2) mds.WriteYM2151mame(Chip.Index, (byte)Chip.Number, (byte)(0x60 + i * 8 + ch), (byte)127);
                                    if (ctYM2151[Chip.Number].UseEmu3) mds.WriteYM2151x68sound(Chip.Index, (byte)Chip.Number, (byte)(0x60 + i * 8 + ch), (byte)127);
                                }
                            }
                            else
                            {
                                if (scYM2151 != null && scYM2151[Chip.Number] != null) scYM2151[Chip.Number].setRegister(0x60 + i * 8 + ch, 127);
                            }
                        }
                    }
                }
            }

            if ((dAddr & 0xf0) == 0x60 || (dAddr & 0xf0) == 0x70)//TL
            {
                int ch = (dAddr & 0x7);

                int al = YM2151FmRegister[Chip.Number][0x20 + ch] & 0x07;
                int slot = (dAddr & 0x18) >> 3;
                //slot = (slot == 0) ? 0 : ((slot == 1) ? 2 : ((slot == 2) ? 1 : 3));
                if ((algM[al] & (1 << slot)) > 0)
                {
                    dData = Math.Min(dData + YM2151NowFadeoutVol[Chip.Number], 127);
                    dData = YM2151MaskFMCh[Chip.Number][ch] ? 127 : dData;
                }
            }

            //if (Chip.Model == EnmModel.VirtualModel)
            //{
            //    if (!ctYM2151[Chip.Number].UseScci)
            //    {
            //        if (ctYM2151[Chip.Number].UseEmu) mds.WriteYM2151((byte)Chip.Number, (byte)dAddr, (byte)dData);
            //        if (ctYM2151[Chip.Number].UseEmu2) mds.WriteYM2151mame((byte)Chip.Number, (byte)dAddr, (byte)dData);
            //        if (ctYM2151[Chip.Number].UseEmu3) mds.WriteYM2151x68sound((byte)Chip.Number, (byte)dAddr, (byte)dData);
            //    }
            //}
            //else
            //{
            //    if (scYM2151[Chip.Number] == null) return;

            //    if (dAddr >= 0x28 && dAddr <= 0x2f)
            //    {
            //        if (hosei == 0)
            //        {
            //            scYM2151[Chip.Number].setRegister(dAddr, dData);
            //        }
            //        else
            //        {
            //            int oct = (dData & 0x70) >> 4;
            //            int note = dData & 0xf;
            //            note = (note < 3) ? note : ((note < 7) ? (note - 1) : ((note < 11) ? (note - 2) : (note - 3)));
            //            note += hosei - 1;
            //            if (note < 0)
            //            {
            //                oct += (note / 12) - 1;
            //                note = (note % 12) + 12;
            //            }
            //            else
            //            {
            //                oct += (note / 12);
            //                note %= 12;
            //            }

            //            note = (note < 3) ? note : ((note < 6) ? (note + 1) : ((note < 9) ? (note + 2) : (note + 3)));
            //            if (scYM2151[Chip.Number] != null)
            //                scYM2151[Chip.Number].setRegister(dAddr, (oct << 4) | note);
            //        }
            //    }
            //    else
            //    {
            //        scYM2151[Chip.Number].setRegister(dAddr, dData);
            //    }
            //}

            if (dAddr == 0x08) //Key-On/Off
            {
                int ch = dData & 0x7;

                if (Chip.ChMasks[ch])
                {
                    dData = ch;
                }
            }
        }

        public void YM2151SetRegister(outDatum od, long Counter, int ChipID, int dAddr, int dData)
        {
            enq(od, Counter, YM2151[ChipID], EnmDataType.Normal, dAddr, dData, null);
        }

        public void YM2151SetRegister(outDatum od, long Counter, int ChipID, PackData[] data)
        {
            enq(od, Counter, YM2151[ChipID], EnmDataType.Block, -1, -1, data);
        }

        public void YM2151SetRegister(outDatum od, long Counter, Chip chip, PackData[] data)
        {
            enq(od, Counter, chip, EnmDataType.Block, -1, -1, data);
        }

        public void YM2151SoftReset(long Counter, int chipID)
        {
            List<PackData> data = YM2151MakeSoftReset(chipID);
            YM2151SetRegister(null, Counter, chipID, data.ToArray());
        }

        public List<PackData> YM2151MakeSoftReset(int ChipID)
        {
            List<PackData> data = new List<PackData>();
            int i;

            //FM全チャネルキーオフ
            for (i = 0; i < 8; i++)
            {
                // note off
                data.Add(new PackData(null, YM2151[ChipID], EnmDataType.Normal, 0x08, 0x00 + i, null));
            }

            data.Add(new PackData(null, YM2151[ChipID], EnmDataType.Normal, 0x0f, 0x00, null)); //  FM NOISE ENABLE/NOISE FREQ
            data.Add(new PackData(null, YM2151[ChipID], EnmDataType.Normal, 0x18, 0x00, null)); //  FM HW LFO FREQ
            data.Add(new PackData(null, YM2151[ChipID], EnmDataType.Normal, 0x19, 0x80, null)); //  FM PMD/VALUE
            data.Add(new PackData(null, YM2151[ChipID], EnmDataType.Normal, 0x19, 0x00, null)); //  FM AMD/VALUE
            data.Add(new PackData(null, YM2151[ChipID], EnmDataType.Normal, 0x1b, 0x00, null)); //  FM HW LFO WAVEFORM

            //FM HW LFO RESET
            data.Add(new PackData(null, YM2151[ChipID], EnmDataType.Normal, 0x01, 0x02, null));
            data.Add(new PackData(null, YM2151[ChipID], EnmDataType.Normal, 0x01, 0x00, null));

            data.Add(new PackData(null, YM2151[ChipID], EnmDataType.Normal, 0x10, 0x00, null)); // FM Timer-A(H)
            data.Add(new PackData(null, YM2151[ChipID], EnmDataType.Normal, 0x11, 0x00, null)); // FM Timer-A(L)
            data.Add(new PackData(null, YM2151[ChipID], EnmDataType.Normal, 0x12, 0x00, null)); // FM Timer-B
            data.Add(new PackData(null, YM2151[ChipID], EnmDataType.Normal, 0x14, 0x00, null)); // FM Timer Control

            for (i = 0; i < 8; i++)
            {
                //  FB/ALG/PAN
                data.Add(new PackData(null, YM2151[ChipID], EnmDataType.Normal, 0x20 + i, 0x00, null));
                // KC                 
                data.Add(new PackData(null, YM2151[ChipID], EnmDataType.Normal, 0x28 + i, 0x00, null));
                // KF                 
                data.Add(new PackData(null, YM2151[ChipID], EnmDataType.Normal, 0x30 + i, 0x00, null));
                // PMS/AMS            
                data.Add(new PackData(null, YM2151[ChipID], EnmDataType.Normal, 0x38 + i, 0x00, null));
            }
            for (i = 0; i < 0x20; i++)
            {
                // DT1/ML
                data.Add(new PackData(null, YM2151[ChipID], EnmDataType.Normal, 0x40 + i, 0x00, null));
                // TL=127             
                data.Add(new PackData(null, YM2151[ChipID], EnmDataType.Normal, 0x60 + i, 0x7f, null));
                // KS/AR              
                data.Add(new PackData(null, YM2151[ChipID], EnmDataType.Normal, 0x80 + i, 0x1F, null));
                // AMD/D1R            
                data.Add(new PackData(null, YM2151[ChipID], EnmDataType.Normal, 0xa0 + i, 0x00, null));
                // DT2/D2R            
                data.Add(new PackData(null, YM2151[ChipID], EnmDataType.Normal, 0xc0 + i, 0x00, null));
                // D1L/RR             
                data.Add(new PackData(null, YM2151[ChipID], EnmDataType.Normal, 0xe0 + i, 0x0F, null));
            }

            return data;
        }

        public void YM2151SetMask(long Counter, int chipID, int ch, bool mask)
        {
            YM2151MaskFMCh[chipID][ch] = mask;

            YM2151SetRegister(null, Counter, (byte)chipID, 0x60 + ch, YM2151FmRegister[chipID][0x60 + ch]);
            YM2151SetRegister(null, Counter, (byte)chipID, 0x68 + ch, YM2151FmRegister[chipID][0x68 + ch]);
            YM2151SetRegister(null, Counter, (byte)chipID, 0x70 + ch, YM2151FmRegister[chipID][0x70 + ch]);
            YM2151SetRegister(null, Counter, (byte)chipID, 0x78 + ch, YM2151FmRegister[chipID][0x78 + ch]);

        }

        public void YM2151WriteClock(byte chipID, int clock)
        {
            if (scYM2151 != null && scYM2151.Length > chipID && scYM2151[chipID] != null)
            {
                scYM2151[chipID].dClock = scYM2151[chipID].SetMasterClock((uint)clock);
            }
        }

        public void YM2151SetFadeoutVolume(long Counter, int v)
        {
            for (int i = 0; i < YM2151.Count; i++)
            {
                if (!YM2151[i].Use) continue;
                if (YM2151[i].Model == EnmVRModel.VirtualModel) continue;
                if (YM2151NowFadeoutVol[i] >> 3 == v >> 3) continue;

                YM2151NowFadeoutVol[i] = v;

                for (int c = 0; c < 8; c++)
                {
                    YM2151SetRegister(null, Counter, i, 0x60 + c, YM2151FmRegister[i][0x60 + c]);
                    YM2151SetRegister(null, Counter, i, 0x68 + c, YM2151FmRegister[i][0x68 + c]);
                    YM2151SetRegister(null, Counter, i, 0x70 + c, YM2151FmRegister[i][0x70 + c]);
                    YM2151SetRegister(null, Counter, i, 0x78 + c, YM2151FmRegister[i][0x78 + c]);
                }
            }
        }

        public void YM2151SetSyncWait(byte chipID, int wait)
        {
            if (scYM2151[chipID] != null && ctYM2151[chipID].UseWait)
            {
                scYM2151[chipID].setRegister(-1, (int)(wait * (ctYM2151[chipID].UseWaitBoost ? 2.0 : 1.0)));
            }
        }

        public int YM2151GetClock(byte chipID)
        {
            if (scYM2151[chipID] == null) return -1;

            return (int)scYM2151[chipID].dClock;
        }

        #endregion



        #region YM2203(OPN)

        private void YM2203WriteRegisterControl(Chip Chip, EnmDataType type, int address, int data, object exData)
        {
            if (type == EnmDataType.Normal)
            {
                if (Chip.Model == EnmVRModel.VirtualModel)
                {
                    if (!ctYM2203[Chip.Number].UseScci && ctYM2203[Chip.Number].UseEmu)
                        mds.WriteYM2203(Chip.Index, (byte)Chip.Number, (byte)address, (byte)data);
                    //if (address == 0x28 && (data & 7) == 2) log.Write(string.Format("{0:X02}",data));
                }
                if (Chip.Model == EnmVRModel.RealModel)
                {
                    if (scYM2203[Chip.Number] != null)
                        scYM2203[Chip.Number].setRegister(address, data);
                }
            }
            else if (type == EnmDataType.Block)
            {
                Audio.sm.SetInterrupt();

                try
                {
                    if (exData == null) return;

                    PackData[] pdata = (PackData[])exData;
                    if (Chip.Model == EnmVRModel.VirtualModel)
                    {
                        foreach (PackData dat in pdata)
                            mds.WriteYM2203(dat.Chip.Index, (byte)dat.Chip.Number, (byte)dat.Address, (byte)dat.Data);
                    }
                    if (Chip.Model == EnmVRModel.RealModel)
                    {
                        if (scYM2203[Chip.Number] != null)
                        {
                            foreach (PackData dat in pdata)
                                scYM2203[Chip.Number].setRegister(dat.Address, dat.Data);
                        }
                    }
                }
                finally
                {
                    Audio.sm.ResetInterrupt();
                }
            }
        }

        public void YM2203SetRegisterProcessing(ref long Counter, ref Chip Chip, ref EnmDataType Type, ref int Address, ref int dData, ref object ExData)
        {
            if (ctYM2203 == null) return;
            if (Address == -1 && dData == -1) return;

            if (Chip.Number == 0) chipLED.PriOPN = 2;
            else chipLED.SecOPN = 2;

            int dAddr = (Address & 0xff);
            int ch;

            fmRegisterYM2203[Chip.Number][dAddr] = dData;

            if ((Chip.Model == EnmVRModel.RealModel && ctYM2203[Chip.Number].UseScci) || (Chip.Model == EnmVRModel.VirtualModel && !ctYM2203[Chip.Number].UseScci))
            {
                if (dAddr == 0x28)
                {
                    ch = dData & 0x3;
                    if (ch >= 0 && ch < 3)
                    {
                        if (ch != 2 || (fmRegisterYM2203[Chip.Number][0x27] & 0xc0) != 0x40)
                        {
                            if ((dData & 0xf0) != 0)
                            {
                                fmKeyOnYM2203[Chip.Number][ch] = (dData & 0xf0) | 1;
                                fmVolYM2203[Chip.Number][ch] = 256 * 6;
                            }
                            else
                            {
                                fmKeyOnYM2203[Chip.Number][ch] &= 0xfe;
                            }
                        }
                        else
                        {
                            fmKeyOnYM2203[Chip.Number][2] = (dData & 0xf0);
                            if ((dData & 0x10) > 0) fmCh3SlotVolYM2203[Chip.Number][0] = 256 * 6;
                            if ((dData & 0x20) > 0) fmCh3SlotVolYM2203[Chip.Number][1] = 256 * 6;
                            if ((dData & 0x40) > 0) fmCh3SlotVolYM2203[Chip.Number][2] = 256 * 6;
                            if ((dData & 0x80) > 0) fmCh3SlotVolYM2203[Chip.Number][3] = 256 * 6;
                        }
                    }
                }

            }


            if ((dAddr & 0xf0) == 0x40)//TL
            {
                ch = (dAddr & 0x3);
                if (ch != 3)
                {
                    int al = fmRegisterYM2203[Chip.Number][0xb0 + ch] & 0x7;
                    int slot = (dAddr & 0xc) >> 2;
                    dData &= 0x7f;

                    if ((algM[al] & (1 << slot)) != 0)
                    {
                        dData = Math.Min(dData + nowYM2203FadeoutVol[Chip.Number], 127);
                        dData = maskFMChYM2203[Chip.Number][ch] ? 127 : dData;
                    }
                }
            }

            //if ((dAddr & 0xf0) == 0xb0)//AL
            //{
            //    int ch = (dAddr & 0x3);
            //    int al = dData & 0x07;//AL

            //    if (ch != 3 && maskFMChYM2203[Chip.Number][ch])
            //    {
            //        for (int i = 0; i < 4; i++)
            //        {
            //            int slot = (i == 0) ? 0 : ((i == 1) ? 2 : ((i == 2) ? 1 : 3));
            //            if ((algM[al] & (1 << slot)) > 0)
            //            {
            //                setYM2203Register(Chip.Number, 0x40 + ch + slot * 4, fmRegisterYM2203[Chip.Number][0x40 + ch]);
            //            }
            //        }
            //    }
            //}

            //ssg level
            if ((dAddr == 0x08 || dAddr == 0x09 || dAddr == 0x0a))
            {
                int d = nowYM2203FadeoutVol[Chip.Number] >> 3;
                dData = Math.Max(dData - d, 0);
                dData = maskFMChYM2203[Chip.Number][dAddr - 0x08 + 3] ? 0 : dData;
            }

            //if (Chip.Model == EnmModel.VirtualModel)
            //{
            //    if (!ctYM2203[Chip.Number].UseScci)
            //    {
            //        mds.WriteYM2203((byte)Chip.Number, (byte)dAddr, (byte)dData);
            //    }
            //}
            //else
            //{
            //    if (scYM2203[Chip.Number] == null) return;

            //    scYM2203[Chip.Number].setRegister(dAddr, dData);
            //}


            //FM ch<6
            if (Address == 0x28)
            {
                ch = dData & 0x3;

                if (Chip.ChMasks[ch]) dData &= 0xf;
            }

            //SSG ch<9
            int adl = Address & 0x00f;
            if (adl == 8 || adl == 9 || adl == 10)
            {
                int adr = Address & 0x3f0;

                //SSG1
                if (adr == 0x000)
                {
                    ch = adl - 8 + 6 + 0;
                    if (Chip.ChMasks[ch])
                        dData = 0;
                }
            }

        }

        public void YM2203SetRegister(outDatum od, long Counter, int ChipID, int dAddr, int dData)
        {
            enq(od, Counter, YM2203[ChipID], EnmDataType.Normal, dAddr, dData, null);
        }

        public void YM2203SetRegister(outDatum od, long Counter, int ChipID, PackData[] data)
        {
            enq(od, Counter, YM2203[ChipID], EnmDataType.Block, -1, -1, data);
        }

        public void YM2203SoftReset(long Counter, int ChipID)
        {
            List<PackData> data = YM2203MakeSoftReset(ChipID);
            YM2203SetRegister(null, Counter, ChipID, data.ToArray());
        }

        public List<PackData> YM2203MakeSoftReset(int chipID)
        {
            List<PackData> data = new List<PackData>();
            int i;

            // FM全チャネルキーオフ
            data.Add(new PackData(null, YM2203[chipID], EnmDataType.Normal, 0x28, 0x00, null));
            data.Add(new PackData(null, YM2203[chipID], EnmDataType.Normal, 0x28, 0x01, null));
            data.Add(new PackData(null, YM2203[chipID], EnmDataType.Normal, 0x28, 0x02, null));

            // FM TL=127
            for (i = 0x40; i < 0x4F + 1; i++)
            {
                data.Add(new PackData(null, YM2203[chipID], EnmDataType.Normal, i, 0x7f, null));
            }
            // FM ML/DT
            for (i = 0x30; i < 0x3F + 1; i++)
            {
                data.Add(new PackData(null, YM2203[chipID], EnmDataType.Normal, i, 0x0, null));
            }
            // FM AR,DR,SR,KS,AMON
            for (i = 0x50; i < 0x7F + 1; i++)
            {
                data.Add(new PackData(null, YM2203[chipID], EnmDataType.Normal, i, 0x0, null));
            }
            // FM SL,RR
            for (i = 0x80; i < 0x8F + 1; i++)
            {
                data.Add(new PackData(null, YM2203[chipID], EnmDataType.Normal, i, 0xff, null));
            }
            // FM F-Num, FB/CONNECT
            for (i = 0x90; i < 0xBF + 1; i++)
            {
                data.Add(new PackData(null, YM2203[chipID], EnmDataType.Normal, i, 0x0, null));
            }
            // FM PAN/AMS/PMS
            for (i = 0xB4; i < 0xB6 + 1; i++)
            {
                data.Add(new PackData(null, YM2203[chipID], EnmDataType.Normal, i, 0xc0, null));
            }
            data.Add(new PackData(null, YM2203[chipID], EnmDataType.Normal, 0x22, 0x00, null)); // HW LFO
            data.Add(new PackData(null, YM2203[chipID], EnmDataType.Normal, 0x24, 0x00, null)); // Timer-A(1)
            data.Add(new PackData(null, YM2203[chipID], EnmDataType.Normal, 0x25, 0x00, null)); // Timer-A(2)
            data.Add(new PackData(null, YM2203[chipID], EnmDataType.Normal, 0x26, 0x00, null)); // Timer-B
            data.Add(new PackData(null, YM2203[chipID], EnmDataType.Normal, 0x27, 0x30, null)); // Timer Control

            // SSG 音程(2byte*3ch)
            for (i = 0x00; i < 0x05 + 1; i++)
            {
                data.Add(new PackData(null, YM2203[chipID], EnmDataType.Normal, i, 0x00, null));
            }
            data.Add(new PackData(null, YM2203[chipID], EnmDataType.Normal, 0x06, 0x00, null)); // SSG ノイズ周波数
            data.Add(new PackData(null, YM2203[chipID], EnmDataType.Normal, 0x07, 0x38, null)); // SSG ミキサ
                                                                                                // SSG ボリューム(3ch)
            for (i = 0x08; i < 0x0A + 1; i++)
            {
                data.Add(new PackData(null, YM2203[chipID], EnmDataType.Normal, i, 0x00, null));
            }
            // SSG Envelope
            for (i = 0x0B; i < 0x0D + 1; i++)
            {
                data.Add(new PackData(null, YM2203[chipID], EnmDataType.Normal, i, 0x00, null));
            }

            return data;
        }

        public void YM2203SetMask(long Counter, int chipID, int ch, bool mask, bool noSend = false)
        {
            maskFMChYM2203[chipID][ch] = mask;
            if (ch >= 6 && ch < 9)
            {
                maskFMChYM2203[chipID][2] = mask;
                maskFMChYM2203[chipID][6] = mask;
                maskFMChYM2203[chipID][7] = mask;
                maskFMChYM2203[chipID][8] = mask;
            }

            if (noSend) return;

            int c = ch;
            if (ch < 3)
            {
                YM2203SetRegister(null, Counter, (byte)chipID, 0x40 + c, fmRegisterYM2203[chipID][0x40 + c]);
                YM2203SetRegister(null, Counter, (byte)chipID, 0x44 + c, fmRegisterYM2203[chipID][0x44 + c]);
                YM2203SetRegister(null, Counter, (byte)chipID, 0x48 + c, fmRegisterYM2203[chipID][0x48 + c]);
                YM2203SetRegister(null, Counter, (byte)chipID, 0x4c + c, fmRegisterYM2203[chipID][0x4c + c]);

            }
            else
            {
                YM2203SetRegister(null, Counter, (byte)chipID, 0x08 + c - 3, fmRegisterYM2203[chipID][0x08 + c - 3]);
            }
        }

        public void YM2203WriteClock(byte chipID, int clock)
        {
            if (scYM2203 != null && scYM2203.Length > chipID && scYM2203[chipID] != null)
            {
                if (scYM2203[chipID] is RC86ctlSoundChip)
                {
                    Nc86ctl.ChipType ct = ((RC86ctlSoundChip)scYM2203[chipID]).chiptype;
                    //OPNA/OPN3Lが選ばれている場合は周波数を2倍にする
                    if (ct == Nc86ctl.ChipType.CHIP_OPN3L || ct == Nc86ctl.ChipType.CHIP_OPNA)
                    {
                        clock *= 2;
                    }
                }
                scYM2203[chipID].dClock = scYM2203[chipID].SetMasterClock((uint)clock);
            }
        }

        public void YM2203SetFadeoutVolume(long Counter, int v)
        {
            for (int i = 0; i < YM2203.Count; i++)
            {
                if (!YM2203[i].Use) continue;
                if (YM2203[i].Model == EnmVRModel.VirtualModel) continue;
                if ((nowYM2203FadeoutVol[i] >> 3) == (v >> 3)) continue;

                nowYM2203FadeoutVol[i] = v;
                for (int c = 0; c < 3; c++)
                {
                    YM2203SetRegister(null, Counter, i, 0x40 + c, fmRegisterYM2203[i][0x40 + c]);
                    YM2203SetRegister(null, Counter, i, 0x44 + c, fmRegisterYM2203[i][0x44 + c]);
                    YM2203SetRegister(null, Counter, i, 0x48 + c, fmRegisterYM2203[i][0x48 + c]);
                    YM2203SetRegister(null, Counter, i, 0x4c + c, fmRegisterYM2203[i][0x4c + c]);
                }
            }
        }

        public void YM2203SetSSGVolume(byte chipID, int vol)
        {
            if (scYM2203 != null && scYM2203[chipID] != null)
            {
                scYM2203[chipID].setSSGVolume((byte)vol);
            }
        }

        #endregion



        #region YM2413(OPL)

        private void YM2413WriteRegisterControl(Chip Chip, EnmDataType type, int address, int data, object exData)
        {
            if (type == EnmDataType.Normal)
            {
                if (Chip.Model == EnmVRModel.VirtualModel)
                {
                    if (!ctYM2413[Chip.Number].UseScci && ctYM2413[Chip.Number].UseEmu)
                        mds.WriteYM2413(Chip.Index, (byte)Chip.Number, (byte)address, (byte)data);
                }
                if (Chip.Model == EnmVRModel.RealModel)
                {
                    if (scYM2413[Chip.Number] != null)
                        scYM2413[Chip.Number].setRegister(address, data);
                }
            }
            else if (type == EnmDataType.Block)
            {
                Audio.sm.SetInterrupt();

                try
                {
                    if (exData == null) return;

                    PackData[] pdata = (PackData[])exData;
                    if (Chip.Model == EnmVRModel.VirtualModel)
                    {
                        foreach (PackData dat in pdata)
                            mds.WriteYM2413(dat.Chip.Index, (byte)dat.Chip.Number, (byte)dat.Address, (byte)dat.Data);
                    }
                    if (Chip.Model == EnmVRModel.RealModel)
                    {
                        if (scYM2413[Chip.Number] != null)
                        {
                            foreach (PackData dat in pdata)
                                scYM2413[Chip.Number].setRegister(dat.Address, dat.Data);
                        }
                    }
                }
                finally
                {
                    Audio.sm.ResetInterrupt();
                }
            }
        }

        public void YM2413SetRegisterProcessing(ref long Counter, ref Chip Chip, ref EnmDataType Type, ref int Address, ref int dData, ref object ExData)
        {
            if (ctYM2413 == null) return;
            if (Address == -1 && dData == -1) return;

            if (Chip.Number == 0) chipLED.PriOPLL = 2;
            else chipLED.SecOPLL = 2;

            int dAddr = (Address & 0xff);
            int ch;

            fmRegisterYM2413[Chip.Number][dAddr] = dData;

            //if ((Chip.Model == EnmModel.RealModel && ctYM2413[Chip.Number].UseScci) || (Chip.Model == EnmModel.VirtualModel && !ctYM2413[Chip.Number].UseScci))
            //{
            if (dAddr >= 0x20 && dAddr <= 0x28)
            {
                ch = dAddr - 0x20;
                int k = dData & 0x10;
                if (k == 0)
                {
                    kiYM2413[Chip.Number].Off[ch] = true;
                }
                else
                {
                    if (kiYM2413[Chip.Number].Off[ch]) kiYM2413[Chip.Number].On[ch] = true;
                    kiYM2413[Chip.Number].Off[ch] = false;
                }

                //mask適用
                if (maskFMChYM2413[Chip.Number][ch]) dData &= 0xef;
            }

            if (dAddr == 0x0e)
            {
                for (int c = 0; c < 5; c++)
                {
                    if ((dData & (0x10 >> c)) == 0)
                    {
                        kiYM2413[Chip.Number].Off[c + 9] = true;
                    }
                    else
                    {
                        if (kiYM2413[Chip.Number].Off[c + 9]) kiYM2413[Chip.Number].On[c + 9] = true;
                        kiYM2413[Chip.Number].Off[c + 9] = false;
                    }
                }

                dData = (dData & 0x20)
                    | (Chip.ChMasks[9] ? 0 : (dData & 0x10))
                    | (Chip.ChMasks[10] ? 0 : (dData & 0x08))
                    | (Chip.ChMasks[11] ? 0 : (dData & 0x04))
                    | (Chip.ChMasks[12] ? 0 : (dData & 0x02))
                    | (Chip.ChMasks[13] ? 0 : (dData & 0x01))
                    ;
            }

            if (dAddr >= 0x30 && dAddr <= 0x38)
            {
                int v = (dData & 0xf) + nowYM2413FadeoutVol[Chip.Number];
                v = Math.Max(Math.Min(v, 15), 0);
                dData = (dData & 0xf0) | (v & 0xf);
            }

            //if (Chip.Model == EnmModel.VirtualModel)
            //{
            //    if (!ctYM2413[Chip.Number].UseScci)
            //    {
            //        mds.WriteYM2413((byte)Chip.Number, (byte)dAddr, (byte)dData);
            //    }
            //}
            //else
            //{
            //    if (scYM2413[Chip.Number] == null) return;
            //    scYM2413[Chip.Number].setRegister(dAddr, dData);
            //}


            //FM ch<18
            if (dAddr >= 0x20 && dAddr <= 0x28)
            {
                ch = dAddr & 0xf;
                if (Chip.ChMasks[ch]) dData &= 0xef;
            }

        }

        public void YM2413SetRegister(outDatum od, long Counter, int ChipID, int dAddr, int dData)
        {
            enq(od, Counter, YM2413[ChipID], EnmDataType.Normal, dAddr, dData, null);
        }

        public void YM2413SetRegister(outDatum od, long Counter, int ChipID, PackData[] data)
        {
            enq(od, Counter, YM2413[ChipID], EnmDataType.Block, -1, -1, data);
        }

        public void YM2413SoftReset(long Counter, int ChipID)
        {
            List<PackData> data = YM2413MakeSoftReset(ChipID);
            YM2413SetRegister(null, Counter, ChipID, data.ToArray());
        }

        public List<PackData> YM2413MakeSoftReset(int chipID)
        {
            List<PackData> data = new List<PackData>();
            int i;

            // FM全チャネルキーオフ
            for (i = 0; i < 9; i++)
            {
                data.Add(new PackData(null, YM2413[chipID], EnmDataType.Normal, 0x20 + i, 0x00, null));
            }
            // FM Vol=15(min)
            for (i = 0; i < 9; i++)
            {
                data.Add(new PackData(null, YM2413[chipID], EnmDataType.Normal, 0x30 + i, 0x0f, null));
            }

            return data;
        }

        public void YM2413SetMask(long Counter, int chipID, int ch, bool mask, bool noSend = false)
        {
            maskFMChYM2413[chipID][ch] = mask;

            if (ch < 9)
            {
                YM2413SetRegister(null, Counter, (byte)chipID, 0x20 + ch, fmRegisterYM2413[chipID][0x20 + ch]);
            }
            else if (ch < 14)
            {
                YM2413SetRegister(null, Counter, (byte)chipID, 0x0e, fmRegisterYM2413[chipID][0x0e]);
            }
        }

        public void YM2413WriteClock(byte chipID, int clock)
        {
            if (scYM2413 != null && scYM2413.Length > chipID && scYM2413[chipID] != null)
            {
                scYM2413[chipID].dClock = scYM2413[chipID].SetMasterClock((uint)clock);
            }
        }

        public void YM2413SetFadeoutVolume(long Counter, int v)
        {
            for (int i = 0; i < YM2413.Count; i++)
            {
                if (!YM2413[i].Use) continue;
                if (YM2413[i].Model == EnmVRModel.VirtualModel) continue;
                if (nowYM2413FadeoutVol[i] == v) continue;

                nowYM2413FadeoutVol[i] = v;
                for (int c = 0; c < 9; c++)
                {
                    YM2413SetRegister(null, Counter, i, 0x30 + c, fmRegisterYM2413[i][0x30 + c]);
                }
            }
        }

        #endregion



        #region YM3526

        private void YM3526WriteRegisterControl(Chip Chip, EnmDataType type, int address, int data, object exData)
        {
            if (type == EnmDataType.Normal)
            {
                if (Chip.Model == EnmVRModel.VirtualModel)
                {
                    if (!ctYM3526[Chip.Number].UseScci && ctYM3526[Chip.Number].UseEmu)
                        mds.WriteYM3526(Chip.Index, (byte)Chip.Number, (byte)address, (byte)data);
                }
                if (Chip.Model == EnmVRModel.RealModel)
                {
                    if (scYM3526[Chip.Number] != null)
                        scYM3526[Chip.Number].setRegister(address, data);
                }
            }
            else if (type == EnmDataType.Block)
            {
                Audio.sm.SetInterrupt();

                try
                {
                    if (exData == null) return;

                    PackData[] pdata = (PackData[])exData;
                    if (Chip.Model == EnmVRModel.VirtualModel)
                    {
                        foreach (PackData dat in pdata)
                            mds.WriteYM3526(Chip.Index, (byte)Chip.Number, (byte)address, (byte)data);
                    }
                    if (Chip.Model == EnmVRModel.RealModel)
                    {
                        if (scYM3526[Chip.Number] != null)
                        {
                            foreach (PackData dat in pdata)
                                scYM3526[Chip.Number].setRegister(dat.Address, dat.Data);
                        }
                    }
                }
                finally
                {
                    Audio.sm.ResetInterrupt();
                }
            }
        }

        public void YM3526SetRegisterProcessing(ref long Counter, ref Chip Chip, ref EnmDataType type, ref int Address, ref int dData, ref object ExData)
        {
            if (ctYM3526 == null) return;
            if (Address == -1 && dData == -1) return;

            if (Chip.Number == 0) chipLED.PriOPL = 2;
            else chipLED.SecOPL = 2;

            int dAddr = (Address & 0xff);
            int ch;

            fmRegisterYM3526[Chip.Number][dAddr] = dData;

            if (dAddr >= 0xb0 && dAddr <= 0xb8)
            {
                ch = dAddr - 0xb0;
                int k = (dData >> 5) & 1;
                if (k == 0)
                {
                    fmRegisterYM3526FM[Chip.Number] &= ~(1 << ch);
                }
                else
                {
                    fmRegisterYM3526FM[Chip.Number] |= (1 << ch);
                }
                fmRegisterYM3526FM[Chip.Number] &= 0x3ffff;
                if (maskFMChYM3526[Chip.Number][ch]) dData &= 0x1f;
            }

            if (dAddr >= 0x40 && dAddr <= 0x55) //TL制御(TBD)
            {
                ch = dAddr - 0x40;
            }

            if (dAddr == 0xbd)
            {
                if ((fmRegisterYM3526RyhthmB[Chip.Number] & 0x10) == 0 && (dData & 0x10) != 0) fmRegisterYM3526Ryhthm[Chip.Number] |= 0x10;
                if ((fmRegisterYM3526RyhthmB[Chip.Number] & 0x08) == 0 && (dData & 0x08) != 0) fmRegisterYM3526Ryhthm[Chip.Number] |= 0x08;
                if ((fmRegisterYM3526RyhthmB[Chip.Number] & 0x04) == 0 && (dData & 0x04) != 0) fmRegisterYM3526Ryhthm[Chip.Number] |= 0x04;
                if ((fmRegisterYM3526RyhthmB[Chip.Number] & 0x02) == 0 && (dData & 0x02) != 0) fmRegisterYM3526Ryhthm[Chip.Number] |= 0x02;
                if ((fmRegisterYM3526RyhthmB[Chip.Number] & 0x01) == 0 && (dData & 0x01) != 0) fmRegisterYM3526Ryhthm[Chip.Number] |= 0x01;
                fmRegisterYM3526RyhthmB[Chip.Number] = dData;

                if (maskFMChYM3526[Chip.Number][9]) dData &= 0xef;
                if (maskFMChYM3526[Chip.Number][10]) dData &= 0xf7;
                if (maskFMChYM3526[Chip.Number][11]) dData &= 0xfb;
                if (maskFMChYM3526[Chip.Number][12]) dData &= 0xfd;
                if (maskFMChYM3526[Chip.Number][13]) dData &= 0xfe;

            }

        }

        public void YM3526SetRegister(outDatum od, long Counter, int ChipID, int dAddr, int dData)
        {
            enq(od, Counter, YM3526[ChipID], EnmDataType.Normal, dAddr, dData, null);
        }

        public void YM3526SetRegister(outDatum od, long Counter, int ChipID, PackData[] data)
        {
            enq(od, Counter, YM3526[ChipID], EnmDataType.Block, -1, -1, data);
        }

        public void YM3526SoftReset(long Counter, int chipID)
        {
            List<PackData> data = YM3526MakeSoftReset(chipID);
            YM3526SetRegister(null, Counter, chipID, data.ToArray());
        }

        public List<PackData> YM3526MakeSoftReset(int chipID)
        {
            List<PackData> data = new List<PackData>();
            int i;

            // FM全チャネルキーオフ
            for (i = 0; i < 9; i++)
            {
                data.Add(new PackData(null, YM3526[chipID], EnmDataType.Normal, 0x0a0 + i, 0x00, null));
                data.Add(new PackData(null, YM3526[chipID], EnmDataType.Normal, 0x0b0 + i, 0x00, null));
            }
            // FM Vol=15(min)
            for (i = 0x40; i < 0x56; i++)
            {
                data.Add(new PackData(null, YM3526[chipID], EnmDataType.Normal, 0x000 + i, 0x3f, null));
            }

            return data;
        }

        public void YM3526SetMask(long Counter, int chipID, int ch, bool mask, bool noSend = false)
        {
            maskFMChYM3526[chipID][YMF278BCh[ch]] = mask;

            if (ch < 9)
            {
                YM3526SetRegister(null, Counter, (byte)chipID, 0xb0 + ch, fmRegisterYM3526[chipID][0xb0 + ch]);
            }
            else
            {
                YM3526SetRegister(null, Counter, (byte)chipID, 0xbd, fmRegisterYM3526[chipID][0xbd]);
            }
        }

        public void YM3526SetFadeoutVolume(long Counter, int v)
        {
            for (int i = 0; i < YM3526.Count; i++)
            {
                if (!YM3526[i].Use) continue;
                if (YM3526[i].Model == EnmVRModel.VirtualModel) continue;
                if (YM3526NowFadeoutVol[i] == v) continue;

                YM3526NowFadeoutVol[i] = v;// (v & 0x78) >> 3;
                for (int c = 0; c < 9; c++)
                {
                    int adr = 0x40 + (c / 3) * 6 + (c % 3);
                    YM3526SetRegister(null, Counter, i, adr, fmRegisterYM3526[i][adr]);
                    YM3526SetRegister(null, Counter, i, adr + 3, fmRegisterYM3526[i][adr + 3]);
                }
            }
        }

        #endregion



        #region Y8950(MSX-AUDIO)


        private void Y8950WriteRegisterControl(Chip Chip, EnmDataType type, int address, int data, object exData)
        {
            if (type == EnmDataType.Normal)
            {
                if (Chip.Model == EnmVRModel.VirtualModel)
                {
                    if (!ctY8950[Chip.Number].UseScci && ctY8950[Chip.Number].UseEmu)
                        mds.WriteY8950(Chip.Index, (byte)Chip.Number, (byte)address, (byte)data);
                }
                if (Chip.Model == EnmVRModel.RealModel)
                {
                    if (scY8950[Chip.Number] != null)
                        scY8950[Chip.Number].setRegister(address, data);
                }
            }
            else if (type == EnmDataType.Block)
            {
                Audio.sm.SetInterrupt();

                try
                {
                    if (exData == null) return;

                    PackData[] pdata = (PackData[])exData;
                    if (Chip.Model == EnmVRModel.VirtualModel)
                    {
                        foreach (PackData dat in pdata)
                            mds.WriteY8950(Chip.Index, (byte)Chip.Number, (byte)address, (byte)data);
                    }
                    if (Chip.Model == EnmVRModel.RealModel)
                    {
                        if (scY8950[Chip.Number] != null)
                        {
                            foreach (PackData dat in pdata)
                                scY8950[Chip.Number].setRegister(dat.Address, dat.Data);
                        }
                    }
                }
                finally
                {
                    Audio.sm.ResetInterrupt();
                }
            }
        }

        public void Y8950SetRegisterProcessing(ref long Counter, ref Chip Chip, ref EnmDataType type, ref int Address, ref int dData, ref object ExData)
        {
            if (ctY8950 == null) return;
            if (Address == -1 && dData == -1) return;

            if (Chip.Number == 0) chipLED.PriY8950 = 2;
            else chipLED.SecY8950 = 2;

            int dAddr = (Address & 0xff);
            int ch;

            fmRegisterY8950[Chip.Number][dAddr] = dData;

            if (dAddr >= 0xb0 && dAddr <= 0xb8)
            {
                ch = dAddr - 0xb0;
                int k = (dData >> 5) & 1;
                if (k == 0)
                {
                    fmRegisterY8950FM[Chip.Number] &= ~(1 << ch);
                }
                else
                {
                    fmRegisterY8950FM[Chip.Number] |= (1 << ch);
                }
                fmRegisterY8950FM[Chip.Number] &= 0x3ffff;
                if (maskFMChY8950[Chip.Number][ch]) dData &= 0x1f;
            }

            if (dAddr == 0xbd)
            {
                if ((fmRegisterY8950RyhthmB[Chip.Number] & 0x10) == 0 && (dData & 0x10) != 0) fmRegisterY8950Ryhthm[Chip.Number] |= 0x10;
                if ((fmRegisterY8950RyhthmB[Chip.Number] & 0x08) == 0 && (dData & 0x08) != 0) fmRegisterY8950Ryhthm[Chip.Number] |= 0x08;
                if ((fmRegisterY8950RyhthmB[Chip.Number] & 0x04) == 0 && (dData & 0x04) != 0) fmRegisterY8950Ryhthm[Chip.Number] |= 0x04;
                if ((fmRegisterY8950RyhthmB[Chip.Number] & 0x02) == 0 && (dData & 0x02) != 0) fmRegisterY8950Ryhthm[Chip.Number] |= 0x02;
                if ((fmRegisterY8950RyhthmB[Chip.Number] & 0x01) == 0 && (dData & 0x01) != 0) fmRegisterY8950Ryhthm[Chip.Number] |= 0x01;
                fmRegisterY8950RyhthmB[Chip.Number] = dData;

                if (maskFMChY8950[Chip.Number][9]) dData &= 0xef;
                if (maskFMChY8950[Chip.Number][10]) dData &= 0xf7;
                if (maskFMChY8950[Chip.Number][11]) dData &= 0xfb;
                if (maskFMChY8950[Chip.Number][12]) dData &= 0xfd;
                if (maskFMChY8950[Chip.Number][13]) dData &= 0xfe;

            }

            //ADPCM
            if (dAddr == 0x07)
            {
                int k = (dData & 0x80);
                if (k == 0)
                {
                    kiY8950[Chip.Number].On[14] = false;
                    kiY8950[Chip.Number].Off[14] = true;
                }
                else
                {
                    kiY8950[Chip.Number].On[14] = true;
                }
                if (maskFMChY8950[Chip.Number][14]) dData &= 0x7f;
            }
        }

        public void Y8950SetRegister(outDatum od, long Counter, int ChipID, int dAddr, int dData)
        {
            enq(od, Counter, Y8950[ChipID], EnmDataType.Normal, dAddr, dData, null);
        }

        public void Y8950SetRegister(outDatum od, long Counter, int ChipID, PackData[] data)
        {
            enq(od, Counter, Y8950[ChipID], EnmDataType.Block, -1, -1, data);
        }

        public void Y8950SoftReset(long Counter, int chipID)
        {
            List<PackData> data = Y8950MakeSoftReset(chipID);
            Y8950SetRegister(null, Counter, chipID, data.ToArray());
        }

        public List<PackData> Y8950MakeSoftReset(int chipID)
        {
            List<PackData> data = new List<PackData>();
            int i;

            // FM全チャネルキーオフ
            for (i = 0; i < 9; i++)
            {
                data.Add(new PackData(null, Y8950[chipID], EnmDataType.Normal, 0x0a0 + i, 0x00, null));
                data.Add(new PackData(null, Y8950[chipID], EnmDataType.Normal, 0x0b0 + i, 0x00, null));
            }
            // FM Vol=64(min)
            for (i = 0x40; i < 0x56; i++)
            {
                data.Add(new PackData(null, Y8950[chipID], EnmDataType.Normal, 0x000 + i, 0x3f, null));
            }

            //ADPCMキーオフ
            data.Add(new PackData(null, Y8950[chipID], EnmDataType.Normal, 0x000 + 0x07, 0x20, null));

            return data;
        }

        public void Y8950SetMask(long Counter, int chipID, int ch, bool mask, bool noSend = false)
        {
            maskFMChY8950[chipID][YMF278BCh[ch]] = mask;

            if (ch < 9)
            {
                Y8950SetRegister(null, Counter, (byte)chipID, 0xb0 + ch, fmRegisterY8950[chipID][0xb0 + ch]);
            }
            else
            {
                Y8950SetRegister(null, Counter, (byte)chipID, 0xbd, fmRegisterY8950[chipID][0xbd]);
            }
        }

        public void Y8950SetFadeoutVolume(long Counter, int v)
        {
            for (int i = 0; i < Y8950.Count; i++)
            {
                if (!Y8950[i].Use) continue;
                if (Y8950[i].Model == EnmVRModel.VirtualModel) continue;
                if (Y8950NowFadeoutVol[i] == v) continue;

                Y8950NowFadeoutVol[i] = v;// (v & 0x78) >> 3;
                for (int c = 0; c < 9; c++)
                {
                    int adr = 0x40 + (c / 3) * 6 + (c % 3);
                    Y8950SetRegister(null, Counter, i, adr, fmRegisterY8950[i][adr]);
                    Y8950SetRegister(null, Counter, i, adr + 3, fmRegisterY8950[i][adr + 3]);
                }
            }
        }

        public void writeY8950PCMData(byte chipid, uint ROMSize, uint DataStart, uint DataLength, byte[] romdata, uint SrcStartAdr)
        {
            EnmVRModel model = EnmVRModel.VirtualModel;
            if (chipid == 0) chipLED.PriY8950 = 2;
            else chipLED.SecY8950 = 2;

            if (model == EnmVRModel.VirtualModel)
                mds.WriteY8950PCMData(chipid, ROMSize, DataStart, DataLength, romdata, SrcStartAdr);
        }

        public ChipKeyInfo getY8950KeyInfo(int chipID)
        {
            for (int ch = 0; ch < kiY8950[chipID].Off.Length; ch++)
            {
                kiY8950ret[chipID].Off[ch] = kiY8950[chipID].Off[ch];
                kiY8950ret[chipID].On[ch] = kiY8950[chipID].On[ch];
                kiY8950[chipID].On[ch] = false;
            }
            return kiY8950ret[chipID];
        }


        #endregion



        #region YM3812

        private void YM3812WriteRegisterControl(Chip Chip, EnmDataType type, int address, int data, object exData)
        {
            if (type == EnmDataType.Normal)
            {
                if (Chip.Model == EnmVRModel.VirtualModel)
                {
                    if (!ctYM3812[Chip.Number].UseScci && ctYM3812[Chip.Number].UseEmu)
                        mds.WriteYM3812(Chip.Index, (byte)Chip.Number, (byte)address, (byte)data);
                }
                if (Chip.Model == EnmVRModel.RealModel)
                {
                    if (scYM3812[Chip.Number] != null)
                        scYM3812[Chip.Number].setRegister(address, data);
                }
            }
            else if (type == EnmDataType.Block)
            {
                Audio.sm.SetInterrupt();

                try
                {
                    if (exData == null) return;

                    PackData[] pdata = (PackData[])exData;
                    if (Chip.Model == EnmVRModel.VirtualModel)
                    {
                        foreach (PackData dat in pdata)
                            mds.WriteYM3812(Chip.Index, (byte)Chip.Number, (byte)address, (byte)data);
                    }
                    if (Chip.Model == EnmVRModel.RealModel)
                    {
                        if (scYM3812[Chip.Number] != null)
                        {
                            foreach (PackData dat in pdata)
                                scYM3812[Chip.Number].setRegister(dat.Address, dat.Data);
                        }
                    }
                }
                finally
                {
                    Audio.sm.ResetInterrupt();
                }
            }
        }

        public void YM3812SetRegisterProcessing(ref long Counter, ref Chip Chip, ref EnmDataType type, ref int Address, ref int dData, ref object ExData)
        {
            if (ctYM3812 == null) return;
            if (Address == -1 && dData == -1) return;

            if (Chip.Number == 0) chipLED.PriOPL2 = 2;
            else chipLED.SecOPL2 = 2;

            int dAddr = (Address & 0xff);
            int ch;

            fmRegisterYM3812[Chip.Number][dAddr] = dData;

            if (dAddr >= 0xb0 && dAddr <= 0xb8)
            {
                ch = dAddr - 0xb0;
                int k = (dData >> 5) & 1;
                if (k == 0)
                {
                    fmRegisterYM3812FM[Chip.Number] &= ~(1 << ch);
                }
                else
                {
                    fmRegisterYM3812FM[Chip.Number] |= (1 << ch);
                }
                fmRegisterYM3812FM[Chip.Number] &= 0x3ffff;
                if (maskFMChYM3812[Chip.Number][ch]) dData &= 0x1f;
            }

            if (dAddr == 0xbd)
            {
                if ((fmRegisterYM3812RyhthmB[Chip.Number] & 0x10) == 0 && (dData & 0x10) != 0) fmRegisterYM3812Ryhthm[Chip.Number] |= 0x10;
                if ((fmRegisterYM3812RyhthmB[Chip.Number] & 0x08) == 0 && (dData & 0x08) != 0) fmRegisterYM3812Ryhthm[Chip.Number] |= 0x08;
                if ((fmRegisterYM3812RyhthmB[Chip.Number] & 0x04) == 0 && (dData & 0x04) != 0) fmRegisterYM3812Ryhthm[Chip.Number] |= 0x04;
                if ((fmRegisterYM3812RyhthmB[Chip.Number] & 0x02) == 0 && (dData & 0x02) != 0) fmRegisterYM3812Ryhthm[Chip.Number] |= 0x02;
                if ((fmRegisterYM3812RyhthmB[Chip.Number] & 0x01) == 0 && (dData & 0x01) != 0) fmRegisterYM3812Ryhthm[Chip.Number] |= 0x01;
                fmRegisterYM3812RyhthmB[Chip.Number] = dData;

                if (maskFMChYM3812[Chip.Number][9]) dData &= 0xef;
                if (maskFMChYM3812[Chip.Number][10]) dData &= 0xf7;
                if (maskFMChYM3812[Chip.Number][11]) dData &= 0xfb;
                if (maskFMChYM3812[Chip.Number][12]) dData &= 0xfd;
                if (maskFMChYM3812[Chip.Number][13]) dData &= 0xfe;

            }

        }

        public void YM3812SetRegister(outDatum od, long Counter, int ChipID, int dAddr, int dData)
        {
            enq(od, Counter, YM3812[ChipID], EnmDataType.Normal, dAddr, dData, null);
        }

        public void YM3812SetRegister(outDatum od, long Counter, int ChipID, PackData[] data)
        {
            enq(od, Counter, YM3812[ChipID], EnmDataType.Block, -1, -1, data);
        }

        public void YM3812SoftReset(long Counter, int chipID)
        {
            List<PackData> data = YM3812MakeSoftReset(chipID);
            YM3812SetRegister(null, Counter, chipID, data.ToArray());
        }

        public List<PackData> YM3812MakeSoftReset(int chipID)
        {
            List<PackData> data = new List<PackData>();
            int i;

            // FM全チャネルキーオフ
            for (i = 0; i < 9; i++)
            {
                data.Add(new PackData(null, YM3812[chipID], EnmDataType.Normal, 0x0a0 + i, 0x00, null));
                data.Add(new PackData(null, YM3812[chipID], EnmDataType.Normal, 0x0b0 + i, 0x00, null));
            }
            // FM Vol=15(min)
            for (i = 0x40; i < 0x56; i++)
            {
                data.Add(new PackData(null, YM3812[chipID], EnmDataType.Normal, 0x000 + i, 0x3f, null));
            }

            return data;
        }

        public void YM3812SetMask(long Counter, int chipID, int ch, bool mask, bool noSend = false)
        {
            maskFMChYM3812[chipID][YMF278BCh[ch]] = mask;

            if (ch < 9)
            {
                YM3812SetRegister(null, Counter, (byte)chipID, 0xb0 + ch, fmRegisterYM3812[chipID][0xb0 + ch]);
            }
            else
            {
                YM3812SetRegister(null, Counter, (byte)chipID, 0xbd, fmRegisterYM3812[chipID][0xbd]);
            }
        }

        public void YM3812SetFadeoutVolume(long Counter, int v)
        {
            for (int i = 0; i < YM3812.Count; i++)
            {
                if (!YM3812[i].Use) continue;
                if (YM3812[i].Model == EnmVRModel.VirtualModel) continue;
                if (YM3812NowFadeoutVol[i] == v) continue;

                YM3812NowFadeoutVol[i] = v;// (v & 0x78) >> 3;
                for (int c = 0; c < 9; c++)
                {
                    int adr = 0x40 + (c / 3) * 6 + (c % 3);
                    YM3812SetRegister(null, Counter, i, adr, fmRegisterYM3812[i][adr]);
                    YM3812SetRegister(null, Counter, i, adr + 3, fmRegisterYM3812[i][adr + 3]);
                }
            }
        }

        #endregion



        #region YMF262(OPL3)

        private void YMF262WriteRegisterControl(Chip Chip, EnmDataType type, int address, int data, object exData)
        {
            if (type == EnmDataType.Normal)
            {
                if (Chip.Model == EnmVRModel.VirtualModel)
                {
                    if (!ctYMF262[Chip.Number].UseScci && ctYMF262[Chip.Number].UseEmu)
                        mds.WriteYMF262(Chip.Index, (byte)Chip.Number, (byte)(address >> 8), (byte)address, (byte)data);
                }
                if (Chip.Model == EnmVRModel.RealModel)
                {
                    if (scYMF262[Chip.Number] != null)
                        scYMF262[Chip.Number].setRegister(address, data);
                }
            }
            else if (type == EnmDataType.Block)
            {
                Audio.sm.SetInterrupt();

                try
                {
                    if (exData == null) return;

                    PackData[] pdata = (PackData[])exData;
                    if (Chip.Model == EnmVRModel.VirtualModel)
                    {
                        foreach (PackData dat in pdata)
                            mds.WriteYMF262(Chip.Index, (byte)Chip.Number, (byte)(address >> 8), (byte)address, (byte)data);
                    }
                    if (Chip.Model == EnmVRModel.RealModel)
                    {
                        if (scYMF262[Chip.Number] != null)
                        {
                            foreach (PackData dat in pdata)
                                scYMF262[Chip.Number].setRegister(dat.Address, dat.Data);
                        }
                    }
                }
                finally
                {
                    Audio.sm.ResetInterrupt();
                }
            }
        }

        public void YMF262SetRegisterProcessing(ref long Counter, ref Chip Chip, ref EnmDataType type, ref int Address, ref int dData, ref object ExData)
        {
            if (ctYMF262 == null) return;
            if (Address == -1 && dData == -1) return;

            if (Chip.Number == 0) chipLED.PriOPL3 = 2;
            else chipLED.SecOPL3 = 2;

            int dAddr = (Address & 0xff);
            int dPort = Address >> 8;
            int ch;

            fmRegisterYMF262[Chip.Number][dPort][dAddr] = dData;

            if (dAddr >= 0xb0 && dAddr <= 0xb8)
            {
                ch = dAddr - 0xb0 + dPort * 9;
                int k = (dData >> 5) & 1;
                if (k == 0)
                {
                    fmRegisterYMF262FM[Chip.Number] &= ~(1 << ch);
                }
                else
                {
                    fmRegisterYMF262FM[Chip.Number] |= (1 << ch);
                }
                fmRegisterYMF262FM[Chip.Number] &= 0x3ffff;
                if (maskFMChYMF262[Chip.Number][ch]) dData &= 0x1f;
            }

            if (dAddr == 0xbd && dPort == 0)
            {
                if ((fmRegisterYMF262RyhthmB[Chip.Number] & 0x10) == 0 && (dData & 0x10) != 0) fmRegisterYMF262Ryhthm[Chip.Number] |= 0x10;
                if ((fmRegisterYMF262RyhthmB[Chip.Number] & 0x08) == 0 && (dData & 0x08) != 0) fmRegisterYMF262Ryhthm[Chip.Number] |= 0x08;
                if ((fmRegisterYMF262RyhthmB[Chip.Number] & 0x04) == 0 && (dData & 0x04) != 0) fmRegisterYMF262Ryhthm[Chip.Number] |= 0x04;
                if ((fmRegisterYMF262RyhthmB[Chip.Number] & 0x02) == 0 && (dData & 0x02) != 0) fmRegisterYMF262Ryhthm[Chip.Number] |= 0x02;
                if ((fmRegisterYMF262RyhthmB[Chip.Number] & 0x01) == 0 && (dData & 0x01) != 0) fmRegisterYMF262Ryhthm[Chip.Number] |= 0x01;
                fmRegisterYMF262RyhthmB[Chip.Number] = dData;

                if (maskFMChYMF262[Chip.Number][18]) dData &= 0xef;
                if (maskFMChYMF262[Chip.Number][19]) dData &= 0xf7;
                if (maskFMChYMF262[Chip.Number][20]) dData &= 0xfb;
                if (maskFMChYMF262[Chip.Number][21]) dData &= 0xfd;
                if (maskFMChYMF262[Chip.Number][22]) dData &= 0xfe;

            }

        }

        public void YMF262SetRegister(outDatum od, long Counter, int ChipID, int dPort, int dAddr, int dData)
        {
#if DEBUG
            //log.Write(string.Format("MML Param  Chip:[{0}] Type:[{1}]", od.linePos == null ? "(null)" : od.linePos.chip, od.type));
#endif
            enq(od, Counter, YMF262[ChipID], EnmDataType.Normal, dPort * 0x100 + dAddr, dData, null);
        }

        public void YMF262SetRegister(outDatum od, long Counter, int ChipID, PackData[] data)
        {
            enq(od, Counter, YMF262[ChipID], EnmDataType.Block, -1, -1, data);
        }

        public void YMF262SoftReset(long Counter, int chipID)
        {
            List<PackData> data = YMF262MakeSoftReset(chipID);
            YMF262SetRegister(null, Counter, chipID, data.ToArray());
        }

        public List<PackData> YMF262MakeSoftReset(int chipID)
        {
            List<PackData> data = new List<PackData>();
            int i;

            // FM全チャネルキーオフ
            for (i = 0; i < 9; i++)
            {
                data.Add(new PackData(null, YMF262[chipID], EnmDataType.Normal, 0x0a0 + i, 0x00, null));
                data.Add(new PackData(null, YMF262[chipID], EnmDataType.Normal, 0x0b0 + i, 0x00, null));
                data.Add(new PackData(null, YMF262[chipID], EnmDataType.Normal, 0x1a0 + i, 0x00, null));
                data.Add(new PackData(null, YMF262[chipID], EnmDataType.Normal, 0x1b0 + i, 0x00, null));
            }
            // FM Vol=15(min)
            for (i = 0x40; i < 0x56; i++)
            {
                data.Add(new PackData(null, YMF262[chipID], EnmDataType.Normal, 0x000 + i, 0x3f, null));
                data.Add(new PackData(null, YMF262[chipID], EnmDataType.Normal, 0x100 + i, 0x3f, null));
            }

            return data;
        }

        public void YMF262SetMask(long Counter, int chipID, int ch, bool mask, bool noSend = false)
        {
            maskFMChYMF262[chipID][YMF278BCh[ch]] = mask;

            if (ch < 9)
            {
                YMF262SetRegister(null, Counter, (byte)chipID, 0, 0xb0 + ch, fmRegisterYMF262[chipID][0][0xb0 + ch]);
            }
            else if (ch < 18)
            {
                YMF262SetRegister(null, Counter, (byte)chipID, 1, 0xb0 + ch - 9, fmRegisterYMF262[chipID][1][0xb0 + ch - 9]);
            }
            else
            {
                YMF262SetRegister(null, Counter, (byte)chipID, 0, 0xbd, fmRegisterYMF262[chipID][1][0xbd]);
            }
        }

        public void YMF262SetFadeoutVolume(long Counter, int v)
        {
            for (int i = 0; i < YMF262.Count; i++)
            {
                if (!YMF262[i].Use) continue;
                if (YMF262[i].Model == EnmVRModel.VirtualModel) continue;
                if (YMF262NowFadeoutVol[i] == v) continue;

                YMF262NowFadeoutVol[i] = v;// (v & 0x78) >> 3;
                for (int c = 0; c < 18; c++)
                {
                    int port = c / 9;
                    int adr = 0x40 + (c / 3) * 6 + (c % 3);
                    YMF262SetRegister(null, Counter, i, port, adr, fmRegisterYMF262[i][port][adr]);
                    YMF262SetRegister(null, Counter, i, port, adr + 3, fmRegisterYMF262[i][port][adr + 3]);
                }
            }
        }


        #endregion



        #region YMF278B(OPL4)

        private void YMF278BWriteRegisterControl(Chip Chip, EnmDataType type, int address, int data, object exData)
        {
            if (type == EnmDataType.Normal)
            {
                if (Chip.Model == EnmVRModel.VirtualModel)
                {
                    if (!ctYMF278B[Chip.Number].UseScci && ctYMF278B[Chip.Number].UseEmu)
                    {
                        //Console.WriteLine(string.Format("[INFO   ] FM P{0:x1} Out:Adr[{1:x02}] val[{2:x02}]", address>>8, (byte)address, (byte)data));
                        mds.WriteYMF278B(Chip.Index, (byte)Chip.Number, (byte)(address >> 8), (byte)address, (byte)data);
                    }
                }
                if (Chip.Model == EnmVRModel.RealModel)
                {
                    if (scYMF278B[Chip.Number] != null)
                        scYMF278B[Chip.Number].setRegister(address, data);
                }
            }
            else if (type == EnmDataType.Block)
            {
                Audio.sm.SetInterrupt();

                try
                {
                    if (exData == null) return;

                    PackData[] pdata = (PackData[])exData;
                    if (Chip.Model == EnmVRModel.VirtualModel)
                    {
                        foreach (PackData dat in pdata)
                            mds.WriteYMF278B(Chip.Index, (byte)Chip.Number, (byte)(dat.Address >> 8), (byte)dat.Address, (byte)dat.Data);
                    }
                    if (Chip.Model == EnmVRModel.RealModel)
                    {
                        if (scYMF278B[Chip.Number] != null)
                        {
                            foreach (PackData dat in pdata)
                                scYMF278B[Chip.Number].setRegister(dat.Address, dat.Data);
                        }
                    }
                }
                finally
                {
                    Audio.sm.ResetInterrupt();
                }
            }
        }

        public void YMF278BSetRegisterProcessing(ref long Counter, ref Chip Chip, ref EnmDataType type, ref int Address, ref int dData, ref object ExData)
        {
            if (ctYMF278B == null) return;
            if (Address == -1 && dData == -1) return;
            return;

            if (Chip.Number == 0) chipLED.PriOPL4 = 2;
            else chipLED.SecOPL4 = 2;

            int dAddr = (Address & 0xff);
            int dPort = Address >> 8;
            int ch;

            fmRegisterYMF278B[Chip.Number][dPort][dAddr] = dData;

            if (dAddr >= 0xb0 && dAddr <= 0xb8)
            {
                ch = dAddr - 0xb0 + dPort * 9;
                int k = (dData >> 5) & 1;
                if (k == 0)
                {
                    fmRegisterYMF278BFM[Chip.Number] &= ~(1 << ch);
                }
                else
                {
                    fmRegisterYMF278BFM[Chip.Number] |= (1 << ch);
                }
                fmRegisterYMF278BFM[Chip.Number] &= 0x3ffff;
                if (maskFMChYMF278B[Chip.Number][ch]) dData &= 0x1f;
            }

            if (dAddr == 0xbd && dPort == 0)
            {
                if ((fmRegisterYMF278BRyhthmB[Chip.Number] & 0x10) == 0 && (dData & 0x10) != 0) fmRegisterYMF278BRyhthm[Chip.Number] |= 0x10;
                if ((fmRegisterYMF278BRyhthmB[Chip.Number] & 0x08) == 0 && (dData & 0x08) != 0) fmRegisterYMF278BRyhthm[Chip.Number] |= 0x08;
                if ((fmRegisterYMF278BRyhthmB[Chip.Number] & 0x04) == 0 && (dData & 0x04) != 0) fmRegisterYMF278BRyhthm[Chip.Number] |= 0x04;
                if ((fmRegisterYMF278BRyhthmB[Chip.Number] & 0x02) == 0 && (dData & 0x02) != 0) fmRegisterYMF278BRyhthm[Chip.Number] |= 0x02;
                if ((fmRegisterYMF278BRyhthmB[Chip.Number] & 0x01) == 0 && (dData & 0x01) != 0) fmRegisterYMF278BRyhthm[Chip.Number] |= 0x01;
                fmRegisterYMF278BRyhthmB[Chip.Number] = dData;

                if (maskFMChYMF278B[Chip.Number][18]) dData &= 0xef;
                if (maskFMChYMF278B[Chip.Number][19]) dData &= 0xf7;
                if (maskFMChYMF278B[Chip.Number][20]) dData &= 0xfb;
                if (maskFMChYMF278B[Chip.Number][21]) dData &= 0xfd;
                if (maskFMChYMF278B[Chip.Number][22]) dData &= 0xfe;

            }

        }

        public void YMF278BSetRegister(outDatum od, long Counter, int ChipID, int dPort, int dAddr, int dData)
        {
#if DEBUG
            //log.Write(string.Format("MML Param  Chip:[{0}] Type:[{1}]", od.linePos == null ? "(null)" : od.linePos.chip, od.type));
            //log.Write(string.Format("[INFO  ] FM P{0:x1} Out:Adr[{1:x02}] val[{2:x02}]", dPort, dAddr, dData));
            //System.Threading.Thread.Sleep(10);
#endif
            enq(od, Counter, YMF278B[ChipID], EnmDataType.Normal, dPort * 0x100 + dAddr, dData, null);
        }

        public void YMF278BSetRegister(outDatum od, long Counter, int ChipID, PackData[] data)
        {
            enq(od, Counter, YMF278B[ChipID], EnmDataType.Block, -1, -1, data);
        }

        public void YMF278BSoftReset(long Counter, int chipID)
        {
            List<PackData> data = YMF278BMakeSoftReset(chipID);
            YMF278BSetRegister(null, Counter, chipID, data.ToArray());
        }

        public List<PackData> YMF278BMakeSoftReset(int chipID)
        {
            List<PackData> data = new List<PackData>();
            int i;

            // FM全チャネルキーオフ
            for (i = 0; i < 9; i++)
            {
                data.Add(new PackData(null, YMF278B[chipID], EnmDataType.Normal, 0x0a0 + i, 0x00, null));
                data.Add(new PackData(null, YMF278B[chipID], EnmDataType.Normal, 0x0b0 + i, 0x00, null));
                data.Add(new PackData(null, YMF278B[chipID], EnmDataType.Normal, 0x1a0 + i, 0x00, null));
                data.Add(new PackData(null, YMF278B[chipID], EnmDataType.Normal, 0x1b0 + i, 0x00, null));
            }
            // FM Vol=15(min)
            for (i = 0x40; i < 0x56; i++)
            {
                data.Add(new PackData(null, YMF278B[chipID], EnmDataType.Normal, 0x000 + i, 0x3f, null));
                data.Add(new PackData(null, YMF278B[chipID], EnmDataType.Normal, 0x100 + i, 0x3f, null));
            }

            for (i = 0x68; i < 0x80; i++)
            {
                data.Add(new PackData(null, YMF278B[chipID], EnmDataType.Normal, 0x200 + i, 0x00, null));
            }
            for (i = 0x50; i < 0x68; i++)
            {
                data.Add(new PackData(null, YMF278B[chipID], EnmDataType.Normal, 0x200 + i, 0x00, null));
            }

            return data;
        }

        public void YMF278BSetMask(long Counter, int chipID, int ch, bool mask, bool noSend = false)
        {
            maskFMChYMF278B[chipID][YMF278BCh[ch]] = mask;

            if (ch < 9)
            {
                YMF278BSetRegister(null, Counter, (byte)chipID, 0, 0xb0 + ch, fmRegisterYMF278B[chipID][0][0xb0 + ch]);
            }
            else if (ch < 18)
            {
                YMF278BSetRegister(null, Counter, (byte)chipID, 1, 0xb0 + ch - 9, fmRegisterYMF278B[chipID][1][0xb0 + ch - 9]);
            }
            else
            {
                YMF278BSetRegister(null, Counter, (byte)chipID, 0, 0xbd, fmRegisterYMF278B[chipID][1][0xbd]);
            }
        }

        public void YMF278BSetFadeoutVolume(long Counter, int v)
        {
            for (int i = 0; i < YMF278B.Count; i++)
            {
                if (!YMF278B[i].Use) continue;
                if (YMF278B[i].Model == EnmVRModel.VirtualModel) continue;
                if (YMF278BNowFadeoutVol[i] == v) continue;

                YMF278BNowFadeoutVol[i] = v;// (v & 0x78) >> 3;
                for (int c = 0; c < 18; c++)
                {
                    int port = c / 9;
                    int adr = 0x40 + (c / 3) * 6 + (c % 3);
                    YMF278BSetRegister(null, Counter, i, port, adr, fmRegisterYMF278B[i][port][adr]);
                    YMF278BSetRegister(null, Counter, i, port, adr + 3, fmRegisterYMF278B[i][port][adr + 3]);
                }
            }
        }

        public void YMF278BWriteClock(byte chipID, int clock)
        {
            if (scYMF278B != null && scYMF278B.Length > chipID && scYMF278B[chipID] != null)
            {
                scYMF278B[chipID].dClock = scYMF278B[chipID].SetMasterClock((uint)clock);
            }
        }


        #endregion



        #region YMF271(OPX)

        private void YMF271WriteRegisterControl(Chip Chip, EnmDataType type, int address, int data, object exData)
        {
            if (type == EnmDataType.Normal)
            {
                if (Chip.Model == EnmVRModel.VirtualModel)
                {
                    if (!ctYMF271[Chip.Number].UseScci && ctYMF271[Chip.Number].UseEmu)
                    {
                        //Console.WriteLine(string.Format("[INFO   ] FM P{0:x1} Out:Adr[{1:x02}] val[{2:x02}]", address>>8, (byte)address, (byte)data));
                        mds.WriteYMF271(Chip.Index, (byte)Chip.Number, (byte)(address >> 8), (byte)address, (byte)data);
                    }
                }
                if (Chip.Model == EnmVRModel.RealModel)
                {
                    if (scYMF271[Chip.Number] != null)
                        scYMF271[Chip.Number].setRegister(address, data);
                }
            }
            else if (type == EnmDataType.Block)
            {
                Audio.sm.SetInterrupt();

                try
                {
                    if (exData == null) return;

                    PackData[] pdata = (PackData[])exData;
                    if (Chip.Model == EnmVRModel.VirtualModel)
                    {
                        foreach (PackData dat in pdata)
                            mds.WriteYMF271(Chip.Index, (byte)Chip.Number, (byte)(dat.Address >> 8), (byte)dat.Address, (byte)dat.Data);
                    }
                    if (Chip.Model == EnmVRModel.RealModel)
                    {
                        if (scYMF271[Chip.Number] != null)
                        {
                            foreach (PackData dat in pdata)
                                scYMF271[Chip.Number].setRegister(dat.Address, dat.Data);
                        }
                    }
                }
                finally
                {
                    Audio.sm.ResetInterrupt();
                }
            }
        }

        public void YMF271SetRegisterProcessing(ref long Counter, ref Chip Chip, ref EnmDataType type, ref int Address, ref int dData, ref object ExData)
        {
            if (ctYMF271 == null) return;
            if (Address == -1 && dData == -1) return;
            return;

            //if (Chip.Number == 0) chipLED.PriOPX = 2;
            //else chipLED.SecOPX = 2;

            //int dAddr = (Address & 0xff);
            //int dPort = Address >> 8;
            //int ch;

            //fmRegisterYMF271[Chip.Number][dPort][dAddr] = dData;

            //if (dAddr >= 0xb0 && dAddr <= 0xb8)
            //{
            //    ch = dAddr - 0xb0 + dPort * 9;
            //    int k = (dData >> 5) & 1;
            //    if (k == 0)
            //    {
            //        fmRegisterYMF271FM[Chip.Number] &= ~(1 << ch);
            //    }
            //    else
            //    {
            //        fmRegisterYMF271FM[Chip.Number] |= (1 << ch);
            //    }
            //    fmRegisterYMF271FM[Chip.Number] &= 0x3ffff;
            //    if (maskFMChYMF271[Chip.Number][ch]) dData &= 0x1f;
            //}

        }

        public void YMF271SetRegister(outDatum od, long Counter, int ChipID, int dPort, int dAddr, int dData)
        {
#if DEBUG
            //log.Write(string.Format("MML Param  Chip:[{0}] Type:[{1}]", od.linePos == null ? "(null)" : od.linePos.chip, od.type));
            //log.Write(string.Format("[INFO  ] FM P{0:x1} Out:Adr[{1:x02}] val[{2:x02}]", dPort, dAddr, dData));
            //System.Threading.Thread.Sleep(10);
#endif
            enq(od, Counter, YMF271[ChipID], EnmDataType.Normal, dPort * 0x100 + dAddr, dData, null);
        }

        public void YMF271SetRegister(outDatum od, long Counter, int ChipID, PackData[] data)
        {
            enq(od, Counter, YMF271[ChipID], EnmDataType.Block, -1, -1, data);
        }

        public void YMF271SoftReset(long Counter, int chipID)
        {
            List<PackData> data = YMF271MakeSoftReset(chipID);
            YMF271SetRegister(null, Counter, chipID, data.ToArray());
        }

        public List<PackData> YMF271MakeSoftReset(int chipID)
        {
            List<PackData> data = new List<PackData>();
            //int i;

            //// FM全チャネルキーオフ
            //for (i = 0; i < 9; i++)
            //{
            //    data.Add(new PackData(null, YMF271[chipID], EnmDataType.Normal, 0x0a0 + i, 0x00, null));
            //    data.Add(new PackData(null, YMF271[chipID], EnmDataType.Normal, 0x0b0 + i, 0x00, null));
            //    data.Add(new PackData(null, YMF271[chipID], EnmDataType.Normal, 0x1a0 + i, 0x00, null));
            //    data.Add(new PackData(null, YMF271[chipID], EnmDataType.Normal, 0x1b0 + i, 0x00, null));
            //}
            //// FM Vol=15(min)
            //for (i = 0x40; i < 0x56; i++)
            //{
            //    data.Add(new PackData(null, YMF271[chipID], EnmDataType.Normal, 0x000 + i, 0x3f, null));
            //    data.Add(new PackData(null, YMF271[chipID], EnmDataType.Normal, 0x100 + i, 0x3f, null));
            //}

            //for (i = 0x68; i < 0x80; i++)
            //{
            //    data.Add(new PackData(null, YMF271[chipID], EnmDataType.Normal, 0x200 + i, 0x00, null));
            //}
            //for (i = 0x50; i < 0x68; i++)
            //{
            //    data.Add(new PackData(null, YMF271[chipID], EnmDataType.Normal, 0x200 + i, 0x00, null));
            //}

            return data;
        }

        public void YMF271SetMask(long Counter, int chipID, int ch, bool mask, bool noSend = false)
        {
            //maskFMChYMF271[chipID][YMF271Ch[ch]] = mask;

            //if (ch < 9)
            //{
            //    YMF271SetRegister(null, Counter, (byte)chipID, 0, 0xb0 + ch, fmRegisterYMF271[chipID][0][0xb0 + ch]);
            //}
            //else if (ch < 18)
            //{
            //    YMF271SetRegister(null, Counter, (byte)chipID, 1, 0xb0 + ch - 9, fmRegisterYMF271[chipID][1][0xb0 + ch - 9]);
            //}
            //else
            //{
            //    YMF271SetRegister(null, Counter, (byte)chipID, 0, 0xbd, fmRegisterYMF271[chipID][1][0xbd]);
            //}
        }

        public void YMF271SetFadeoutVolume(long Counter, int v)
        {
            //for (int i = 0; i < YMF271.Count; i++)
            //{
            //    if (!YMF271[i].Use) continue;
            //    if (YMF271[i].Model == EnmVRModel.VirtualModel) continue;
            //    if (YMF271NowFadeoutVol[i] == v) continue;

            //    YMF271NowFadeoutVol[i] = v;// (v & 0x78) >> 3;
            //    for (int c = 0; c < 18; c++)
            //    {
            //        int port = c / 9;
            //        int adr = 0x40 + (c / 3) * 6 + (c % 3);
            //        YMF271SetRegister(null, Counter, i, port, adr, fmRegisterYMF271[i][port][adr]);
            //        YMF271SetRegister(null, Counter, i, port, adr + 3, fmRegisterYMF271[i][port][adr + 3]);
            //    }
            //}
        }

        public void YMF271WriteClock(byte chipID, int clock)
        {
            if (scYMF271 != null && scYMF271.Length > chipID && scYMF271[chipID] != null)
            {
                scYMF271[chipID].dClock = scYMF271[chipID].SetMasterClock((uint)clock);
            }
        }


        #endregion



        #region YM2608(OPNA)

        public Nc86ctl.ChipType YM2608GetGIMICType(int ChipID)
        {
            if (scYM2608[ChipID] == null) return Nc86ctl.ChipType.CHIP_UNKNOWN;
            if (scYM2608[ChipID] is RC86ctlSoundChip)
            {
                return (Nc86ctl.ChipType)((RC86ctlSoundChip)scYM2608[ChipID]).Type;
            }

            return Nc86ctl.ChipType.CHIP_UNKNOWN;
        }

        private void YM2608WriteRegisterControl(Chip Chip, EnmDataType type, int address, int data, object exData)
        {
            if (type == EnmDataType.Normal || type== EnmDataType.Force)
            {
                if (Chip.Model == EnmVRModel.VirtualModel)
                {
                    if (ctYM2608[Chip.Number]==null ||(!ctYM2608[Chip.Number].UseScci && ctYM2608[Chip.Number].UseEmu))
                    {
                        //log.Write(string.Format("FM P{2} Out:Adr[{0:x02}] val[{1:x02}]", (byte)address, (byte)data, (byte)(address >> 8)));
                        mds.WriteYM2608(Chip.Index, (byte)Chip.Number, (byte)(address >> 8), (byte)address, (byte)data);
                    }
                    else if (ctYM2610[Chip.Number].OnlyPCMEmulation)
                    {
                        {
                            bool bSend = false;
                            // レジスタをマスクして送信する
                            if (address >= 0x100 && address <= 0x110)
                            {
                                // ADPCM
                                bSend = true;
                            }

                            if (bSend)
                            {
                                mds.WriteYM2608(Chip.Index, (byte)Chip.Number, (byte)(address >> 8), (byte)address, (byte)data);
                            }
                        }
                    }
                }
                if (Chip.Model == EnmVRModel.RealModel)
                {
                    if (scYM2608[Chip.Number] != null)
                    {
                        if (scYM2608[Chip.Number] is RC86ctlSoundChip && ((RC86ctlSoundChip)scYM2608[Chip.Number]).Type == (int)Nc86ctl.ChipType.CHIP_OPL3)
                        {
                            bool bSend = true;
                            // レジスタをマスクして送信する
                            if (address >= 0x100 && address <= 0x110)
                            {
                                // ADPCM
                                bSend = false;
                            }
                            if (bSend)
                            {
                                scYM2608[Chip.Number].setRegister(address, data);
                            }
                        }
                        else
                        {
                            scYM2608[Chip.Number].setRegister(address, data);
                        }
                    }
                }
            }
            else if (type == EnmDataType.Block || type == EnmDataType.ForceBlock)
            {
                Audio.sm.SetInterrupt();

                try
                {
                    if (exData == null) return;

                    PackData[] pdata = (PackData[])exData;
                    if (Chip.Model == EnmVRModel.VirtualModel)
                    {
                        log.Write("Sending YM2608(Emu) Block data");
                        foreach (PackData dat in pdata)
                        {
                            //Console.WriteLine("FM P{2} Out:Adr[{0:x02}] val[{1:x02}]", (byte)dat.Address, (byte)dat.Data, (byte)(dat.Address >> 8));
                            mds.WriteYM2608((byte)dat.Chip.Number, (byte)(dat.Address >> 8), (byte)dat.Address, (byte)dat.Data);
                        }
                    }
                    if (Chip.Model == EnmVRModel.RealModel)
                    {
                        if (scYM2608[Chip.Number] != null)
                        {
                            log.Write("Sending YM2608 Block data");
                            foreach (PackData dat in pdata)
                                scYM2608[Chip.Number].setRegister(dat.Address, dat.Data);
                            if (Chip.Number == 0)
                                Audio.realChip.WaitOPNADPCMData(Audio.setting.YM2608Type.SoundLocation == -1);
                            else
                                Audio.realChip.WaitOPNADPCMData(Audio.setting.YM2608SType.SoundLocation == -1);
                        }
                    }
                }
                finally
                {
                    Audio.sm.ResetInterrupt();
                }
            }
        }

        public void YM2608SetRegisterProcessing(ref long Counter, ref Chip Chip, ref EnmDataType Type, ref int Address, ref int dData, ref object ExData)
        {
            if (ctYM2608 == null) return;
            if (Address == -1 && dData == -1) return;

            if (Chip.Number == 0) chipLED.PriOPNA = 2;
            else chipLED.SecOPNA = 2;

            int dPort = (Address >> 8) & 1;
            int dAddr = (Address & 0xff);
            int ch;

            fmRegisterYM2608[Chip.Number][dPort][dAddr] = dData;

            //
            // 情報取得のみ
            //

            if ((Chip.Model == EnmVRModel.RealModel && ctYM2608[Chip.Number].UseScci) || (Chip.Model == EnmVRModel.VirtualModel && !ctYM2608[Chip.Number].UseScci))
            {
                //FM 0x00:0x28:KeyON:bit7-4 SLOT:bit2-0 Ch
                if (dPort == 0 && dAddr == 0x28)
                {
                    // data:0,1,2,4,5,6 -> Ch:1,2,3,4,5,6
                    ch = (dData & 0x3) + ((dData & 0x4) > 0 ? 3 : 0);
                    if (ch >= 0 && ch < 6)// && (dData & 0xf0) > 0)
                    {
                        //Ch3ではなく、Ch3であっても効果音モードでは無い場合
                        if (ch != 2 || (fmRegisterYM2608[Chip.Number][0][0x27] & 0xc0) != 0x40)
                        {
                            //スロットを一つ以上指定している場合はキーオン
                            if ((dData & 0xf0) != 0)
                            {
                                fmKeyOnYM2608[Chip.Number][ch] = (dData & 0xf0) | 1;
                                fmVolYM2608[Chip.Number][ch] = 256 * 6;
                            }
                            else
                            {
                                fmKeyOnYM2608[Chip.Number][ch] &= 0xfe;
                            }
                        }
                        else
                        {
                            fmKeyOnYM2608[Chip.Number][2] = dData & 0xf0;
                            if ((dData & 0x10) != 0) fmCh3SlotVolYM2608[Chip.Number][0] = 256 * 6;
                            if ((dData & 0x20) != 0) fmCh3SlotVolYM2608[Chip.Number][1] = 256 * 6;
                            if ((dData & 0x40) != 0) fmCh3SlotVolYM2608[Chip.Number][2] = 256 * 6;
                            if ((dData & 0x80) != 0) fmCh3SlotVolYM2608[Chip.Number][3] = 256 * 6;
                        }
                    }
                }

                //ADPCM 0x01:0x01:CONTROL2:bit 7:L 6:R 3:合成スタート 2:1=再生0=録音 1:DRAMアクセスモード1=8bit0=1bit 0:外部メモリー指定
                if (dPort == 1 && dAddr == 0x01)
                {
                    fmVolYM2608AdpcmPan[Chip.Number] = (dData & 0xc0) >> 6;
                    if (fmVolYM2608AdpcmPan[Chip.Number] > 0)
                    {
                        fmVolYM2608Adpcm[Chip.Number][0] = (int)((256 * 6.0 * fmRegisterYM2608[Chip.Number][1][0x0b] / 64.0) * ((fmVolYM2608AdpcmPan[Chip.Number] & 0x02) > 0 ? 1 : 0));
                        fmVolYM2608Adpcm[Chip.Number][1] = (int)((256 * 6.0 * fmRegisterYM2608[Chip.Number][1][0x0b] / 64.0) * ((fmVolYM2608AdpcmPan[Chip.Number] & 0x01) > 0 ? 1 : 0));
                    }
                }

                //Rhythm 0x00:0x10:Dump/KeyOn
                if (dPort == 0 && dAddr == 0x10)
                {
                    int tl = fmRegisterYM2608[Chip.Number][0][0x11] & 0x3f;
                    for (int i = 0; i < 6; i++)
                    {
                        if ((dData & (0x1 << i)) > 0)
                        {
                            int il = fmRegisterYM2608[Chip.Number][0][0x18 + i] & 0x1f;
                            int pan = (fmRegisterYM2608[Chip.Number][0][0x18 + i] & 0xc0) >> 6;
                            fmVolYM2608Rhythm[Chip.Number][i][0] = (int)(256 * 6 * ((tl * il) >> 4) / 127.0) * ((pan & 2) > 0 ? 1 : 0);
                            fmVolYM2608Rhythm[Chip.Number][i][1] = (int)(256 * 6 * ((tl * il) >> 4) / 127.0) * ((pan & 1) > 0 ? 1 : 0);
                        }
                    }
                }

            }


            //強制無加工データは加工しない
            if (Type == EnmDataType.Force || Type == EnmDataType.ForceBlock) 
                return;


            //
            // データ加工
            //

            //FM:$xx:$4x:total level
            if ((dAddr & 0xf0) == 0x40)//TL
            {
                //bit1-0:ch number
                ch = (dAddr & 0x3);
                if (ch != 3)//chが3は本来意味が無いが、指定してくるドライバーも存在するので弾く
                {
                    int al = fmRegisterYM2608[Chip.Number][dPort][0xb0 + ch] & 0x07;//AL
                    int slot = (dAddr & 0xc) >> 2;
                    dData &= 0x7f;

                    //キャリアかどうか判定し、そうであれば、音量値を加工する
                    if ((algM[al] & (1 << slot)) != 0)
                    {
                        dData = Math.Min(dData + nowYM2608FadeoutVol[Chip.Number], 127);
                        dData = maskFMChYM2608[Chip.Number][dPort * 3 + ch] ? 127 : dData;

                        //if (silentVoiceFMChYM2608[Chip.Number][dPort * 3 + ch]) Type = EnmDataType.None;
                    }
                }
            }
            if ((dAddr >= 0xa0 && dAddr <= 0xa2) || (dAddr >= 0xa4 && dAddr <= 0xa6))//0xa0-0xa6:CH1-6 FNUM
            {
                ch = (dAddr & 0x3);
                if (Chip.silentVoice[dPort * 3 + ch]) Type = EnmDataType.None;
            }
            if ((dAddr >= 0xa8 && dAddr <= 0xaa) || (dAddr >= 0xac && dAddr <= 0xae))//0xa8-0xae:CH3 SLOT FNUM
            {
                if (Chip.silentVoice[dPort * 3 + 2]) Type = EnmDataType.None;
            }

            //ssg level
            if (dPort == 0)
            {
                if (dAddr == 0x08 || dAddr == 0x09 || dAddr == 0x0a)
                {
                    int d = nowYM2608FadeoutVol[Chip.Number] >> 3;
                    dData = Math.Max(dData - d, 0);
                    dData = maskFMChYM2608[Chip.Number][dAddr - 0x08 + 6] ? 0 : dData;

                    if (Chip.silentVoice[dAddr - 0x08 + 6]) Type = EnmDataType.None;
                }
                else if (dAddr >= 0x00 && dAddr <= 0x05)//0x00-0x05:SSG Ch1-3のfnum
                {
                    if (Chip.silentVoice[dAddr / 2 + 6]) Type = EnmDataType.None;
                }
            }

            //rhythm level
            if (dPort == 0 && dAddr == 0x11)
            {
                int d = nowYM2608FadeoutVol[Chip.Number] >> 1;
                dData = Math.Max(dData - d, 0);
            }

            //adpcm level
            if (dPort == 1 && dAddr == 0x0b)
            {
                int d = nowYM2608FadeoutVol[Chip.Number] * 2;
                dData = Math.Max(dData - d, 0);
                dData = maskFMChYM2608[Chip.Number][12] ? 0 : dData;

                if (Chip.silentVoice[12]) Type = EnmDataType.None;
            }

            //adpcm start
            if (dPort == 1 && dAddr == 0x00)
            {
                if ((dData & 0x80) != 0 && maskFMChYM2608[Chip.Number][12])
                {
                    dData &= 0x7f;
                }
                if (Chip.silentVoice[12]) Type = EnmDataType.None;
            }

            //adpcm delta
            if (dPort == 1 && (dAddr == 0x0a || dAddr == 0x09))
            {
                if (Chip.silentVoice[12]) Type = EnmDataType.None;
            }

            //Ryhthm
            if (dPort == 0 && dAddr == 0x10)
            {
                if (maskFMChYM2608[Chip.Number][13])
                {
                    dData = 0;
                }

                if (Chip.silentVoice[13]) Type = EnmDataType.None;
            }


            Address = dPort * 0x100 + dAddr;


            //FM ch<9
            if (Address == 0x28)
            {
                ch = dData & 0x3;
                ch += ((dData & 0x4) == 0) ? 0 : 3;
                ch += (Address == 0x228) ? 9 : 0;

                if (Chip.ChMasks[ch]) dData &= 0xf;

                if (Chip.silentVoice[ch]) Type = EnmDataType.None;
            }

            //SSG ch<12
            int adl = Address & 0x00f;
            if (adl == 8 || adl == 9 || adl == 10)
            {
                int adr = Address & 0x3f0;

                //SSG1
                if (adr == 0x000)
                {
                    ch = adl - 8 + 9 + 0;
                    if (Chip.ChMasks[ch])
                        dData = 0;

                }

                if (Chip.silentVoice[adl - 0x08 + 6]) Type = EnmDataType.None;
            }

            //リズム ch<18
            if (Address == 0x10)
            {
                byte mask = 0x80;
                for (int i = 0; i < 6; i++)
                {
                    mask |= (byte)((Chip.ChMasks[i + 12] ? 0 : 1) << i);
                }

                dData &= mask;

                if (Chip.silentVoice[13]) Type = EnmDataType.None;
            }

            //ADPCM1 ch=18
            if (Address == 0x100)
            {
                if (Chip.ChMasks[18]) dData &= 0x7f;

                if (Chip.silentVoice[12]) Type = EnmDataType.None;
            }

        }

        public void YM2608SetRegister(outDatum od, long Counter, int ChipID, int dPort, int dAddr, int dData)
        {
            dummyChip.Move(YM2608[ChipID]);

            int address = dPort * 0x100 + dAddr;
            if ((address >= 0x100 && address <= 0x110) && ctYM2608[0].OnlyPCMEmulation)
            {
                dummyChip.Model = EnmVRModel.VirtualModel;
            }

            if (dPort == -1 && dAddr == -1 && dData == -1)
            {
                //ダミーデータ(演奏情報だけ流したい時向け)
                enq(od, Counter, dummyChip, EnmDataType.Normal, -1, -1, null);
            }
            else
                enq(od, Counter, dummyChip, EnmDataType.Normal, dPort * 0x100 + dAddr, dData, null);
        }

        public void YM2608SetRegister(outDatum od, long Counter, Chip chip, PackData[] data)
        {
            enq(od, Counter, chip, EnmDataType.Block, -1, -1, data);
        }

        public void YM2608SoftReset(long Counter, int chipID)
        {
            List<PackData> data = YM2608MakeSoftReset(chipID);
            YM2608SetRegister(null, Counter, YM2608[chipID], data.ToArray());
        }

        public List<PackData> YM2608MakeSoftReset(int chipID)
        {
            List<PackData> data = new List<PackData>();
            int i;

            // FM全チャネルキーオフ
            data.Add(new PackData(null, YM2608[chipID], EnmDataType.Normal, 0x28, 0x00, null));
            data.Add(new PackData(null, YM2608[chipID], EnmDataType.Normal, 0x28, 0x01, null));
            data.Add(new PackData(null, YM2608[chipID], EnmDataType.Normal, 0x28, 0x02, null));
            data.Add(new PackData(null, YM2608[chipID], EnmDataType.Normal, 0x28, 0x04, null));
            data.Add(new PackData(null, YM2608[chipID], EnmDataType.Normal, 0x28, 0x05, null));
            data.Add(new PackData(null, YM2608[chipID], EnmDataType.Normal, 0x28, 0x06, null));

            // FM TL=127
            for (i = 0x40; i < 0x4F + 1; i++)
            {
                data.Add(new PackData(null, YM2608[chipID], EnmDataType.Normal, 0x000 + i, 0x7f, null));
                data.Add(new PackData(null, YM2608[chipID], EnmDataType.Normal, 0x100 + i, 0x7f, null));
            }
            // FM ML/DT
            for (i = 0x30; i < 0x3F + 1; i++)
            {
                data.Add(new PackData(null, YM2608[chipID], EnmDataType.Normal, i, 0x0, null));
                data.Add(new PackData(null, YM2608[chipID], EnmDataType.Normal, 0x100 + i, 0x0, null));
            }
            // FM AR,DR,SR,KS,AMON
            for (i = 0x50; i < 0x7F + 1; i++)
            {
                data.Add(new PackData(null, YM2608[chipID], EnmDataType.Normal, i, 0x0, null));
                data.Add(new PackData(null, YM2608[chipID], EnmDataType.Normal, 0x100 + i, 0x0, null));
            }
            // FM SL,RR
            for (i = 0x80; i < 0x8F + 1; i++)
            {
                data.Add(new PackData(null, YM2608[chipID], EnmDataType.Normal, i, 0xff, null));
                data.Add(new PackData(null, YM2608[chipID], EnmDataType.Normal, 0x100 + i, 0xff, null));
            }
            // FM F-Num, FB/CONNECT
            for (i = 0x90; i < 0xBF + 1; i++)
            {
                data.Add(new PackData(null, YM2608[chipID], EnmDataType.Normal, i, 0x0, null));
                data.Add(new PackData(null, YM2608[chipID], EnmDataType.Normal, 0x100 + i, 0x0, null));
            }
            // FM PAN/AMS/PMS
            for (i = 0xB4; i < 0xB6 + 1; i++)
            {
                data.Add(new PackData(null, YM2608[chipID], EnmDataType.Normal, i, 0xc0, null));
                data.Add(new PackData(null, YM2608[chipID], EnmDataType.Normal, 0x100 + i, 0xc0, null));
            }
            data.Add(new PackData(null, YM2608[chipID], EnmDataType.Normal, 0x22, 0x00, null)); // HW LFO
            data.Add(new PackData(null, YM2608[chipID], EnmDataType.Normal, 0x24, 0x00, null)); // Timer-A(1)
            data.Add(new PackData(null, YM2608[chipID], EnmDataType.Normal, 0x25, 0x00, null)); // Timer-A(2)
            data.Add(new PackData(null, YM2608[chipID], EnmDataType.Normal, 0x26, 0x00, null)); // Timer-B
            data.Add(new PackData(null, YM2608[chipID], EnmDataType.Normal, 0x27, 0x30, null)); // Timer Control
            data.Add(new PackData(null, YM2608[chipID], EnmDataType.Normal, 0x29, 0x80, null)); // FM4-6 Enable

            // SSG 音程(2byte*3ch)
            for (i = 0x00; i < 0x05 + 1; i++)
            {
                data.Add(new PackData(null, YM2608[chipID], EnmDataType.Normal, i, 0x00, null));
            }
            data.Add(new PackData(null, YM2608[chipID], EnmDataType.Normal, 0x06, 0x00, null)); // SSG ノイズ周波数
            data.Add(new PackData(null, YM2608[chipID], EnmDataType.Normal, 0x07, 0x38, null)); // SSG ミキサ
                                                                                                // SSG ボリューム(3ch)
            for (i = 0x08; i < 0x0A + 1; i++)
            {
                data.Add(new PackData(null, YM2608[chipID], EnmDataType.Normal, i, 0x00, null));
            }
            // SSG Envelope
            for (i = 0x0B; i < 0x0D + 1; i++)
            {
                data.Add(new PackData(null, YM2608[chipID], EnmDataType.Normal, i, 0x00, null));
            }

            // RHYTHM
            data.Add(new PackData(null, YM2608[chipID], EnmDataType.Normal, 0x10, 0xBF, null)); // 強制発音停止
            data.Add(new PackData(null, YM2608[chipID], EnmDataType.Normal, 0x11, 0x00, null)); // Total Level
            data.Add(new PackData(null, YM2608[chipID], EnmDataType.Normal, 0x18, 0x00, null)); // BD音量
            data.Add(new PackData(null, YM2608[chipID], EnmDataType.Normal, 0x19, 0x00, null)); // SD音量
            data.Add(new PackData(null, YM2608[chipID], EnmDataType.Normal, 0x1A, 0x00, null)); // CYM音量
            data.Add(new PackData(null, YM2608[chipID], EnmDataType.Normal, 0x1B, 0x00, null)); // HH音量
            data.Add(new PackData(null, YM2608[chipID], EnmDataType.Normal, 0x1C, 0x00, null)); // TOM音量
            data.Add(new PackData(null, YM2608[chipID], EnmDataType.Normal, 0x1D, 0x00, null)); // RIM音量

            // ADPCM              
            data.Add(new PackData(null, YM2608[chipID], EnmDataType.Normal, 0x100 + 0x00, 0x21, null)); // ADPCMリセット
            data.Add(new PackData(null, YM2608[chipID], EnmDataType.Normal, 0x100 + 0x01, 0x06, null)); // ADPCM消音
            data.Add(new PackData(null, YM2608[chipID], EnmDataType.Normal, 0x100 + 0x10, 0x9C, null)); // FLAGリセット        

            return data;
        }

        public void YM2608SetMask(long Counter, int ChipID, int ch, bool mask, bool noSend = false)
        {
            maskFMChYM2608[ChipID][ch] = mask;
            if (ch >= 9 && ch < 12)
            {
                maskFMChYM2608[ChipID][2] = mask;
                maskFMChYM2608[ChipID][9] = mask;
                maskFMChYM2608[ChipID][10] = mask;
                maskFMChYM2608[ChipID][11] = mask;
            }

            int c = (ch < 3) ? ch : (ch - 3);
            int p = (ch < 3) ? 0 : 1;

            if (noSend) return;

            if (ch < 6)
            {
                YM2608SetRegister(null, Counter, ChipID, p, 0x40 + c, fmRegisterYM2608[ChipID][p][0x40 + c]);
                YM2608SetRegister(null, Counter, ChipID, p, 0x44 + c, fmRegisterYM2608[ChipID][p][0x44 + c]);
                YM2608SetRegister(null, Counter, ChipID, p, 0x48 + c, fmRegisterYM2608[ChipID][p][0x48 + c]);
                YM2608SetRegister(null, Counter, ChipID, p, 0x4c + c, fmRegisterYM2608[ChipID][p][0x4c + c]);

            }
            else if (ch < 9)
            {
                YM2608SetRegister(null, Counter, ChipID, 0, 0x08 + ch - 6, fmRegisterYM2608[ChipID][0][0x08 + ch - 6]);
            }
            else if (ch < 12)
            {
                YM2608SetRegister(null, Counter, ChipID, 0, 0x40 + 2, fmRegisterYM2608[ChipID][0][0x40 + 2]);
                YM2608SetRegister(null, Counter, ChipID, 0, 0x44 + 2, fmRegisterYM2608[ChipID][0][0x44 + 2]);
                YM2608SetRegister(null, Counter, ChipID, 0, 0x48 + 2, fmRegisterYM2608[ChipID][0][0x48 + 2]);
                YM2608SetRegister(null, Counter, ChipID, 0, 0x4c + 2, fmRegisterYM2608[ChipID][0][0x4c + 2]);

            }
        }

        public void YM2608WriteClock(byte chipID, int clock)
        {
            if (scYM2608 != null && scYM2608.Length > chipID && scYM2608[chipID] != null)
            {
                scYM2608[chipID].dClock = scYM2608[chipID].SetMasterClock((uint)clock);
            }
        }

        public void YM2608SetFadeoutVolume(long Counter, int v)
        {
            for (int i = 0; i < YM2608.Count; i++)
            {
                if (!YM2608[i].Use) continue;
                if (YM2608[i].Model == EnmVRModel.VirtualModel) continue;
                if ((nowYM2608FadeoutVol[i] >> 3) == (v >> 3)) continue;

                nowYM2608FadeoutVol[i] = v;

                for (int p = 0; p < 2; p++)
                {
                    for (int c = 0; c < 3; c++)
                    {
                        YM2608SetRegister(null, Counter, i, p, 0x40 + c, fmRegisterYM2608[i][p][0x40 + c]);
                        YM2608SetRegister(null, Counter, i, p, 0x44 + c, fmRegisterYM2608[i][p][0x44 + c]);
                        YM2608SetRegister(null, Counter, i, p, 0x48 + c, fmRegisterYM2608[i][p][0x48 + c]);
                        YM2608SetRegister(null, Counter, i, p, 0x4c + c, fmRegisterYM2608[i][p][0x4c + c]);
                    }
                }

                //ssg
                YM2608SetRegister(null, Counter, i, 0, 0x08, fmRegisterYM2608[i][0][0x08]);
                YM2608SetRegister(null, Counter, i, 0, 0x09, fmRegisterYM2608[i][0][0x09]);
                YM2608SetRegister(null, Counter, i, 0, 0x0a, fmRegisterYM2608[i][0][0x0a]);

                //rhythm                   
                YM2608SetRegister(null, Counter, i, 0, 0x11, fmRegisterYM2608[i][0][0x11]);

                //adpcm                    
                YM2608SetRegister(null, Counter, i, 1, 0x0b, fmRegisterYM2608[i][1][0x0b]);
            }
        }

        public void YM2608SetSyncWait(byte chipID, int wait)
        {
            if (scYM2608[chipID] != null && ctYM2608[chipID].UseWait)
            {
                scYM2608[chipID].setRegister(-1, (int)(wait * (ctYM2608[chipID].UseWaitBoost ? 2.0 : 1.0)));
            }
        }

        public void YM2608SetSSGVolume(byte chipID, int vol)
        {
            if (scYM2608 != null && scYM2608[chipID] != null)
            {
                scYM2608[chipID].setSSGVolume((byte)vol);
            }
        }

        /// <summary>
        /// 不要かも
        /// </summary>
        /// <param name="chipID"></param>
        public void YM2608SendData(byte chipID)
        {
            EnmVRModel model = EnmVRModel.VirtualModel;
            if (model == EnmVRModel.VirtualModel) return;

            if (scYM2608[chipID] != null && ctYM2608[chipID].UseWait)
            {
                realChip.SendData();
                while (!scYM2608[chipID].isBufferEmpty()) { }
            }
        }

        public void setYM2608SSGVolume(byte chipID, int vol, EnmVRModel model)
        {
            if (model == EnmVRModel.VirtualModel)
            {
            }
            else
            {
                if (scYM2608 != null && scYM2608[chipID] != null)
                {
                    scYM2608[chipID].setSSGVolume((byte)vol);
                }
            }
        }

        #endregion



        #region YM2610(OPNB)

        private void YM2610WriteRegisterControl(Chip Chip, EnmDataType type, int address, int data, object exData)
        {
            if (type == EnmDataType.Normal)
            {
                if (Chip.Model == EnmVRModel.VirtualModel)
                {
                    if (!ctYM2610[Chip.Number].UseScci && ctYM2610[Chip.Number].UseEmu)
                    {
                        //Console.WriteLine("I:{0:x2} N:{1:x2} A:{2:x4} D:{3:X2}", Chip.Index, (byte)Chip.Number, address, (byte)data);
                        mds.WriteYM2610(Chip.Index, (byte)Chip.Number, (byte)(address >> 8), (byte)address, (byte)data);
                    }
                    else if (ctYM2610[Chip.Number].OnlyPCMEmulation)
                    {
                        //if (ctYM2610[Chip.Number].UseEmu)
                        {
                            bool bSend = false;
                            // レジスタをマスクして送信する
                            if (address >= 0x100 && address <= 0x12d)
                            {
                                // ADPCM-A
                                bSend = true;
                            }
                            else if (address >= 0x010 && address <= 0x01c)
                            {
                                // ADPCM-B
                                bSend = true;
                            }

                            if (bSend)
                            {
                                mds.WriteYM2610(Chip.Index, (byte)Chip.Number, (byte)(address >> 8), (byte)address, (byte)data);
                            }
                        }
                    }

                }
                if (Chip.Model == EnmVRModel.RealModel)
                {
                    if (scYM2610[Chip.Number] != null)
                    {
                        if (scYM2610[Chip.Number] is RScciSoundChip && ((RScciSoundChip)scYM2610[Chip.Number]).Type == (int)EnmRealChipType.YM2610)
                        {
                            scYM2610[Chip.Number].setRegister(address, data);
                        }
                        else
                        {
                            bool bSend = true;
                            // レジスタをマスクして送信する
                            if (address >= 0x100 && address <= 0x12d)
                            {
                                // ADPCM-A
                                bSend = false;
                            }
                            else if (address >= 0x010 && address <= 0x01c)
                            {
                                // ADPCM-B
                                bSend = false;
                            }
                            if (bSend)
                            {
                                scYM2610[Chip.Number].setRegister(address, data);
                            }

                            if (scYM2610EB[Chip.Number] != null
                                && !ctYM2610[0].OnlyPCMEmulation)
                            {
                                scYM2610EB[Chip.Number].setRegister(address | 0x10000, data);
                            }
                        }
                    }
                    else
                    {
                        if (scYM2610EA[Chip.Number] != null)
                        {
                            bool bSend = true;
                            // レジスタをマスクして送信する
                            if (address >= 0x100 && address <= 0x12d)
                            {
                                // ADPCM-A
                                bSend = false;
                            }
                            else if (address >= 0x010 && address <= 0x01c)
                            {
                                // ADPCM-B
                                bSend = false;
                            }
                            if (bSend)
                            {
                                scYM2610EA[Chip.Number].setRegister(address, data);
                            }
                        }
                        if (scYM2610EB[Chip.Number] != null && !ctYM2610[0].OnlyPCMEmulation)
                        {
                            scYM2610EB[Chip.Number].setRegister(address | 0x10000, data);
                        }
                    }
                }
            }
            else if (type == EnmDataType.Block)
            {
                Audio.sm.SetInterrupt();

                try
                {
                    if (exData == null) return;

                    if (exData is PackData[])
                    {
                        PackData[] pdata = (PackData[])exData;
                        if (Chip.Model == EnmVRModel.VirtualModel)
                        {
                            foreach (PackData dat in pdata)
                            {
                                //Console.WriteLine("I:{0:x2} N:{1:x2} A:{2:x4} D:{3:X2}", dat.Chip.Index, (byte)dat.Chip.Number, dat.Address, (byte)dat.Data);
                                mds.WriteYM2610(dat.Chip.Index, (byte)dat.Chip.Number, (byte)(dat.Address >> 8), (byte)dat.Address, (byte)dat.Data);
                            }
                        }
                        if (Chip.Model == EnmVRModel.RealModel)
                        {
                            if (scYM2610[Chip.Number] != null)
                            {
                                foreach (PackData dat in pdata)
                                    scYM2610[Chip.Number].setRegister(dat.Address, dat.Data);
                            }
                        }
                        return;
                    }

                    byte[] adpcmData = (byte[])exData;
                    if (Chip.Model == EnmVRModel.VirtualModel)
                    {
                        log.Write(string.Format("Sending YM2610(Emu) ADPCM-{0}", (data == -1) ? "A" : "B"));
                        if (data == -1)
                        {
                            mds.WriteYM2610_SetAdpcmA(Chip.Index, (byte)Chip.Number, adpcmData);
                        }
                        else
                        {
                            mds.WriteYM2610_SetAdpcmB(Chip.Index, (byte)Chip.Number, adpcmData);
                        }
                    }
                    if (Chip.Model == EnmVRModel.RealModel)
                    {
                        if (scYM2610[Chip.Number] != null)
                        {
                            //実OPNBのみ
                            if ((scYM2610[Chip.Number] is RScciSoundChip) && scYM2610[Chip.Number].Type == (int)EnmRealChipType.YM2610)
                            {
                                byte dPort = 2;
                                int startAddr = 0;
                                scYM2610[Chip.Number].setRegister((dPort << 8) | 0x00, 0x00);
                                scYM2610[Chip.Number].setRegister((dPort << 8) | 0x01, (startAddr >> 8) & 0xff);
                                scYM2610[Chip.Number].setRegister((dPort << 8) | 0x02, (startAddr >> 16) & 0xff);

                                // pushReg(CMD_YM2610|0x02,0x03,0x01);
                                scYM2610[Chip.Number].setRegister((dPort << 8) | 0x03, (data == -1) ? 0x01 : 0x00);
                                // データ転送
                                log.Write(string.Format("Sending YM2610 ADPCM-{0}", (data == -1) ? "A" : "B"));
                                for (int cnt = 0; cnt < adpcmData.Length; cnt++)
                                {
                                    // pushReg(CMD_YM2610|0x02,0x04,*m_pDump);
                                    scYM2610[Chip.Number].setRegister((dPort << 8) | 0x04, adpcmData[cnt]);
                                }

                                realChip.SendData();
                            }
                        }
                        if (scYM2610EB[Chip.Number] != null && !ctYM2610[0].OnlyPCMEmulation)
                        {
                            byte dPort = 2;
                            int startAddr = 0;
                            scYM2610EB[Chip.Number].setRegister((dPort << 8) | 0x10000, 0x00);
                            scYM2610EB[Chip.Number].setRegister((dPort << 8) | 0x10001, (startAddr >> 8) & 0xff);
                            scYM2610EB[Chip.Number].setRegister((dPort << 8) | 0x10002, (startAddr >> 16) & 0xff);

                            // pushReg(CMD_YM2610|0x02,0x03,0x01);
                            scYM2610EB[Chip.Number].setRegister((dPort << 8) | 0x10003, (data == -1) ? 0x01 : 0x00);
                            // データ転送
                            log.Write(string.Format("Sending YM2610 ADPCM-{0}", (data == -1) ? "A" : "B"));
                            for (int cnt = 0; cnt < adpcmData.Length; cnt++)
                            {
                                // pushReg(CMD_YM2610|0x02,0x04,*m_pDump);
                                scYM2610EB[Chip.Number].setRegister((dPort << 8) | 0x10004, adpcmData[cnt]);
                            }

                            realChip.SendData();
                        }
                    }
                }
                finally
                {
                    Audio.sm.ResetInterrupt();
                }
            }
        }

        public void YM2610SetRegister(outDatum od, long Counter, Chip chip, byte[] data,bool isA)
        {
            enq(od, Counter, chip, EnmDataType.Block, -1, isA ? -1 : -2, data);
        }


        public void YM2610SetRegisterProcessing(ref long Counter, ref Chip Chip, ref EnmDataType Type, ref int Address, ref int dData, ref object ExData)
        {
            if (ctYM2610 == null) return;
            if (Address == -1 && dData == -1) return;

            if (Chip.Number == 0) chipLED.PriOPNB = 2;
            else chipLED.SecOPNB = 2;

            int dPort = (Address >> 8) & 1;
            int dAddr = (Address & 0xff);
            int ch;

            fmRegisterYM2610[Chip.Number][dPort][dAddr] = dData;

            if ((Chip.Model == EnmVRModel.RealModel && (ctYM2610[Chip.Number].UseScci || ctYM2610[Chip.Number].UseScci2)) || (Chip.Model == EnmVRModel.VirtualModel && !ctYM2610[Chip.Number].UseScci))
            {
                //fmRegisterYM2610[dPort][dAddr] = dData;
                if (dPort == 0 && dAddr == 0x28)
                {
                    ch = (dData & 0x3) + ((dData & 0x4) > 0 ? 3 : 0);
                    if (ch >= 0 && ch < 6)// && (dData & 0xf0) > 0)
                    {
                        if (ch != 2 || (fmRegisterYM2610[Chip.Number][0][0x27] & 0xc0) != 0x40)
                        {
                            if ((dData & 0xf0) != 0)
                            {
                                fmKeyOnYM2610[Chip.Number][ch] = (dData & 0xf0) | 1;
                                fmVolYM2610[Chip.Number][ch] = 256 * 6;
                            }
                            else
                            {
                                fmKeyOnYM2610[Chip.Number][ch] &= 0xfe;
                            }
                        }
                        else
                        {
                            fmKeyOnYM2610[Chip.Number][2] = dData & 0xf0;
                            if ((dData & 0x10) > 0) fmCh3SlotVolYM2610[Chip.Number][0] = 256 * 6;
                            if ((dData & 0x20) > 0) fmCh3SlotVolYM2610[Chip.Number][1] = 256 * 6;
                            if ((dData & 0x40) > 0) fmCh3SlotVolYM2610[Chip.Number][2] = 256 * 6;
                            if ((dData & 0x80) > 0) fmCh3SlotVolYM2610[Chip.Number][3] = 256 * 6;
                        }
                    }
                }

                // ADPCM B KEYON
                if (dPort == 0 && dAddr == 0x10)
                {
                    if ((dData & 0x80) != 0)
                    {
                        int p = (fmRegisterYM2610[Chip.Number][0][0x11] & 0xc0) >> 6;
                        p = p == 0 ? 3 : p;
                        if (fmVolYM2610AdpcmPan[Chip.Number] != p)
                            fmVolYM2610AdpcmPan[Chip.Number] = p;

                        //if (fmVolYM2610AdpcmPan > 0)
                        //{
                        fmVolYM2610Adpcm[Chip.Number][0] = (int)((256 * 6.0 * fmRegisterYM2610[Chip.Number][0][0x1b] / 64.0) * ((fmVolYM2610AdpcmPan[Chip.Number] & 0x02) > 0 ? 1 : 0));
                        fmVolYM2610Adpcm[Chip.Number][1] = (int)((256 * 6.0 * fmRegisterYM2610[Chip.Number][0][0x1b] / 64.0) * ((fmVolYM2610AdpcmPan[Chip.Number] & 0x01) > 0 ? 1 : 0));
                        //                        System.Console.WriteLine("{0:X2}:{1:X2}", 0x09, fmRegisterYM2610[1][0x09]);
                        //                        System.Console.WriteLine("{0:X2}:{1:X2}", 0x0A, fmRegisterYM2610[1][0x0A]);
                        //}
                    }
                }

                // ADPCM A KEYON
                if (dPort == 1 && dAddr == 0x00)
                {
                    if ((dData & 0x80) == 0)
                    {
                        int tl = fmRegisterYM2610[Chip.Number][1][0x01] & 0x3f;
                        for (int i = 0; i < 6; i++)
                        {
                            if ((dData & (0x1 << i)) > 0)
                            {
                                int il = fmRegisterYM2610[Chip.Number][1][0x08 + i] & 0x1f;
                                int pan = (fmRegisterYM2610[Chip.Number][1][0x08 + i] & 0xc0) >> 6;
                                fmVolYM2610Rhythm[Chip.Number][i][0] = (int)(256 * 6 * ((tl * il) >> 4) / 127.0) * ((pan & 2) > 0 ? 1 : 0);
                                fmVolYM2610Rhythm[Chip.Number][i][1] = (int)(256 * 6 * ((tl * il) >> 4) / 127.0) * ((pan & 1) > 0 ? 1 : 0);
                            }
                        }
                    }
                }

            }



            if ((dAddr & 0xf0) == 0x40)//TL
            {
                ch = (dAddr & 0x3);
                if (ch != 3)
                {
                    int al = fmRegisterYM2610[Chip.Number][dPort][0xb0 + ch] & 0x07;//AL
                    int slot = (dAddr & 0xc) >> 2;
                    dData &= 0x7f;

                    if ((algM[al] & (1 << slot)) != 0)
                    {
                        dData = Math.Min(dData + nowYM2610FadeoutVol[Chip.Number], 127);
                        dData = maskFMChYM2610[Chip.Number][dPort * 3 + ch] ? 127 : dData;
                    }
                }
                //}
            }

            //if ((dAddr & 0xf0) == 0xb0)//AL
            //{
            //    int ch = (dAddr & 0x3);
            //    int al = dData & 0x07;//AL

            //    if (ch != 3 && maskFMChYM2610[Chip.Number ][ch])
            //    {
            //        for (int i = 0; i < 4; i++)
            //        {
            //            int slot = (i == 0) ? 0 : ((i == 1) ? 2 : ((i == 2) ? 1 : 3));
            //            if ((algM[al] & (1 << slot)) > 0)
            //            {
            //                setYM2610Register(Chip.Number , dPort, 0x40 + ch + slot * 4, fmRegisterYM2610[Chip.Number ][dPort][0x40 + ch]);
            //            }
            //        }
            //    }
            //}

            //ssg level
            if (dPort == 0 && (dAddr == 0x08 || dAddr == 0x09 || dAddr == 0x0a))
            {
                int d = nowYM2610FadeoutVol[Chip.Number] >> 3;
                dData = Math.Max(dData - d, 0);
                dData = maskFMChYM2610[Chip.Number][dAddr - 0x08 + 6] ? 0 : dData;
            }

            //rhythm level
            if (dPort == 1 && dAddr == 0x01)
            {
                int d = nowYM2610FadeoutVol[Chip.Number] >> 1;
                dData = Math.Max(dData - d, 0);
                dData = maskFMChYM2610[Chip.Number][12] ? 0 : dData;
            }

            //Rhythm
            if (dPort == 1 && dAddr == 0x00)
            {
                if (maskFMChYM2610[Chip.Number][12])
                {
                    dData = 0;
                }
            }

            //adpcm level
            if (dPort == 0 && dAddr == 0x1b)
            {
                int d = nowYM2610FadeoutVol[Chip.Number] * 2;
                dData = Math.Max(dData - d, 0);
                dData = maskFMChYM2610[Chip.Number][13] ? 0 : dData;
            }

            //adpcm start
            if (dPort == 0 && dAddr == 0x10)
            {
                if ((dData & 0x80) != 0 && maskFMChYM2610[Chip.Number][13])
                {
                    dData &= 0x7f;
                }
            }



            //if (Chip.Model == EnmModel.VirtualModel)
            //{
            //    if (!ctYM2610[Chip.Number ].UseScci && !ctYM2610[Chip.Number ].UseScci2)
            //    {
            //        mds.WriteYM2610((byte)Chip.Number , (byte)dPort, (byte)dAddr, (byte)dData);
            //    }
            //}
            //else
            //{
            //    if (scYM2610[Chip.Number ] != null) scYM2610[Chip.Number ].setRegister(dPort * 0x100 + dAddr, dData);
            //    if (scYM2610EA[Chip.Number ] != null)
            //    {
            //        int dReg = (dPort << 8) | dAddr;
            //        bool bSend = true;
            //        // レジスタをマスクして送信する
            //        if (dReg >= 0x100 && dReg <= 0x12d)
            //        {
            //            // ADPCM-A
            //            bSend = false;
            //        }
            //        else if (dReg >= 0x010 && dReg <= 0x01c)
            //        {
            //            // ADPCM-B
            //            bSend = false;
            //        }
            //        if (bSend)
            //        {
            //            scYM2610EA[Chip.Number ].setRegister((dPort << 8) | dAddr, dData);
            //        }
            //    }
            //    if (scYM2610EB[Chip.Number ] != null)
            //    {
            //        scYM2610EB[Chip.Number ].setRegister((dPort << 8) | dAddr | 0x10000, dData);
            //    }
            //}


            //FM ch<9
            if (Address == 0x28)
            {
                ch = dData & 0x3;
                ch += ((dData & 0x4) == 0) ? 0 : 3;
                ch += (Address == 0x228) ? 9 : 0;

                if (Chip.ChMasks[ch]) dData &= 0xf;
            }

            //SSG ch<12
            int adl = Address & 0x00f;
            if (adl == 8 || adl == 9 || adl == 10)
            {
                int adr = Address & 0x3f0;

                //SSG1
                if (adr == 0x000)
                {
                    ch = adl - 8 + 9 + 0;
                    if (Chip.ChMasks[ch])
                        dData = 0;
                }
            }

            //ADPCM-A ch<18
            if (Address == 0x100)
            {
                byte mask = 0x80;
                for (int i = 0; i < 6; i++)
                {
                    mask |= (byte)((Chip.ChMasks[i + 12] ? 0 : 1) << i);
                }

                dData &= mask;
            }

            //ADPCM-B ch=18
            if (Address == 0x010)
            {
                if (Chip.ChMasks[18]) dData &= 0x7f;
            }

        }

        public void YM2610SetRegister(outDatum od, long Counter, int ChipID, int dPort, int dAddr, int dData)
        {
            //Console.WriteLine("P:{0:x2} A:{1:x2} D:{2:x2}",dPort,dAddr,dData);
            dummyChip.Move(YM2610[ChipID]);

            int address = dPort * 0x100 + dAddr;
            if ((
                (address >= 0x100 && address <= 0x12d)
                || (address >= 0x010 && address <= 0x01c)
                //|| ((address >= 0x200 && address <= 0x204) || address == 0x210)
                )
                && ctYM2610[0].OnlyPCMEmulation)
            {
                dummyChip.Model = EnmVRModel.VirtualModel;
            }

            enq(od, Counter, dummyChip, EnmDataType.Normal, dPort * 0x100 + dAddr, dData, null);
            //enq(Counter, YM2610[ChipID], EnmDataType.Normal, dPort * 0x100 + dAddr, dData, null);
        }

        public void YM2610SetRegister(outDatum od, long Counter, int ChipID, PackData[] data)
        {
            enq(null, Counter, YM2610[ChipID], EnmDataType.Block, -1, -1, data);
        }

        public void YM2610WriteSetAdpcmA(outDatum od, long Counter, int ChipID, byte[] ym2610AdpcmA)
        {
            dummyChip.Move(YM2610[ChipID]);

            if (ctYM2610[0].OnlyPCMEmulation)
            {
                dummyChip.Model = EnmVRModel.VirtualModel;
            }

            enq(od, Counter, dummyChip, EnmDataType.Block, -1, -1, ym2610AdpcmA);

        }

        public void YM2610WriteSetAdpcmB(outDatum od, long Counter, int ChipID, byte[] ym2610AdpcmB)
        {
            dummyChip.Move(YM2610[ChipID]);

            if (ctYM2610[0].OnlyPCMEmulation)
            {
                dummyChip.Model = EnmVRModel.VirtualModel;
            }

            enq(od, Counter, dummyChip, EnmDataType.Block, -1, -2, ym2610AdpcmB);
        }

        public void YM2610SoftReset(long Counter, int chipID)
        {
            List<PackData> data = YM2610MakeSoftReset(chipID);
            YM2610SetRegister(null, Counter, chipID, data.ToArray());
        }

        public List<PackData> YM2610MakeSoftReset(int chipID)
        {
            List<PackData> data = new List<PackData>();
            int i;

            // FM全チャネルキーオフ
            data.Add(new PackData(null, YM2610[chipID], EnmDataType.Normal, 0x28, 0x00, null));
            data.Add(new PackData(null, YM2610[chipID], EnmDataType.Normal, 0x28, 0x01, null));
            data.Add(new PackData(null, YM2610[chipID], EnmDataType.Normal, 0x28, 0x02, null));
            data.Add(new PackData(null, YM2610[chipID], EnmDataType.Normal, 0x28, 0x04, null));
            data.Add(new PackData(null, YM2610[chipID], EnmDataType.Normal, 0x28, 0x05, null));
            data.Add(new PackData(null, YM2610[chipID], EnmDataType.Normal, 0x28, 0x06, null));

            // FM TL=127
            for (i = 0x40; i < 0x4F + 1; i++)
            {
                data.Add(new PackData(null, YM2610[chipID], EnmDataType.Normal, 0x000 + i, 0x7f, null));
                data.Add(new PackData(null, YM2610[chipID], EnmDataType.Normal, 0x100 + i, 0x7f, null));
            }
            // FM ML/DT
            for (i = 0x30; i < 0x3F + 1; i++)
            {
                data.Add(new PackData(null, YM2610[chipID], EnmDataType.Normal, i, 0x0, null));
                data.Add(new PackData(null, YM2610[chipID], EnmDataType.Normal, 0x100 + i, 0x0, null));
            }
            // FM AR,DR,SR,KS,AMON
            for (i = 0x50; i < 0x7F + 1; i++)
            {
                data.Add(new PackData(null, YM2610[chipID], EnmDataType.Normal, i, 0x0, null));
                data.Add(new PackData(null, YM2610[chipID], EnmDataType.Normal, 0x100 + i, 0x0, null));
            }
            // FM SL,RR
            for (i = 0x80; i < 0x8F + 1; i++)
            {
                data.Add(new PackData(null, YM2610[chipID], EnmDataType.Normal, i, 0xff, null));
                data.Add(new PackData(null, YM2610[chipID], EnmDataType.Normal, 0x100 + i, 0xff, null));
            }
            // FM F-Num, FB/CONNECT
            for (i = 0x90; i < 0xBF + 1; i++)
            {
                data.Add(new PackData(null, YM2610[chipID], EnmDataType.Normal, i, 0x0, null));
                data.Add(new PackData(null, YM2610[chipID], EnmDataType.Normal, 0x100 + i, 0x0, null));
            }
            // FM PAN/AMS/PMS
            for (i = 0xB4; i < 0xB6 + 1; i++)
            {
                data.Add(new PackData(null, YM2610[chipID], EnmDataType.Normal, i, 0xc0, null));
                data.Add(new PackData(null, YM2610[chipID], EnmDataType.Normal, 0x100 + i, 0xc0, null));
            }
            data.Add(new PackData(null, YM2610[chipID], EnmDataType.Normal, 0x22, 0x00, null)); // HW LFO
            data.Add(new PackData(null, YM2610[chipID], EnmDataType.Normal, 0x24, 0x00, null)); // Timer-A(1)
            data.Add(new PackData(null, YM2610[chipID], EnmDataType.Normal, 0x25, 0x00, null)); // Timer-A(2)
            data.Add(new PackData(null, YM2610[chipID], EnmDataType.Normal, 0x26, 0x00, null)); // Timer-B
            data.Add(new PackData(null, YM2610[chipID], EnmDataType.Normal, 0x27, 0x30, null)); // Timer Control
            data.Add(new PackData(null, YM2610[chipID], EnmDataType.Normal, 0x29, 0x80, null)); // FM4-6 Enable

            // SSG 音程(2byte*3ch)
            for (i = 0x00; i < 0x05 + 1; i++)
            {
                data.Add(new PackData(null, YM2610[chipID], EnmDataType.Normal, i, 0x00, null));
            }
            data.Add(new PackData(null, YM2610[chipID], EnmDataType.Normal, 0x06, 0x00, null)); // SSG ノイズ周波数
            data.Add(new PackData(null, YM2610[chipID], EnmDataType.Normal, 0x07, 0x38, null)); // SSG ミキサ
                                                                                                // SSG ボリューム(3ch)
            for (i = 0x08; i < 0x0A + 1; i++)
            {
                data.Add(new PackData(null, YM2610[chipID], EnmDataType.Normal, i, 0x00, null));
            }
            // SSG Envelope
            for (i = 0x0B; i < 0x0D + 1; i++)
            {
                data.Add(new PackData(null, YM2610[chipID], EnmDataType.Normal, i, 0x00, null));
            }

            // ADPCM-A
            data.Add(new PackData(null, YM2610[chipID], EnmDataType.Normal, 0x100 + 0x00, 0xBF, null)); // 強制発音停止
            data.Add(new PackData(null, YM2610[chipID], EnmDataType.Normal, 0x100 + 0x01, 0x00, null)); // Total Level
            data.Add(new PackData(null, YM2610[chipID], EnmDataType.Normal, 0x100 + 0x08, 0x00, null)); // BD音量
            data.Add(new PackData(null, YM2610[chipID], EnmDataType.Normal, 0x100 + 0x09, 0x00, null)); // SD音量
            data.Add(new PackData(null, YM2610[chipID], EnmDataType.Normal, 0x100 + 0x0A, 0x00, null)); // CYM音量
            data.Add(new PackData(null, YM2610[chipID], EnmDataType.Normal, 0x100 + 0x0B, 0x00, null)); // HH音量
            data.Add(new PackData(null, YM2610[chipID], EnmDataType.Normal, 0x100 + 0x0C, 0x00, null)); // TOM音量
            data.Add(new PackData(null, YM2610[chipID], EnmDataType.Normal, 0x100 + 0x0D, 0x00, null)); // RIM音量

            // ADPCM-B
            data.Add(new PackData(null, YM2610[chipID], EnmDataType.Normal, 0x000 + 0x10, 0x01, null)); // ADPCMリセット
            data.Add(new PackData(null, YM2610[chipID], EnmDataType.Normal, 0x000 + 0x1B, 0x00, null)); // ADPCM消音
            data.Add(new PackData(null, YM2610[chipID], EnmDataType.Normal, 0x000 + 0x1C, 0x00, null)); // FLAGリセット        

            return data;
        }

        public void YM2610SetMask(long Counter, int ChipID, int ch, bool mask, bool noSend = false)
        {
            maskFMChYM2610[ChipID][ch] = mask;
            if (ch >= 9 && ch < 12)
            {
                maskFMChYM2610[ChipID][2] = mask;
                maskFMChYM2610[ChipID][9] = mask;
                maskFMChYM2610[ChipID][10] = mask;
                maskFMChYM2610[ChipID][11] = mask;
            }

            int c = (ch < 3) ? ch : (ch - 3);
            int p = (ch < 3) ? 0 : 1;

            if (ch < 6)
            {
                YM2610SetRegister(null, Counter, ChipID, p, 0x40 + c, fmRegisterYM2610[ChipID][p][0x40 + c]);
                YM2610SetRegister(null, Counter, ChipID, p, 0x44 + c, fmRegisterYM2610[ChipID][p][0x44 + c]);
                YM2610SetRegister(null, Counter, ChipID, p, 0x48 + c, fmRegisterYM2610[ChipID][p][0x48 + c]);
                YM2610SetRegister(null, Counter, ChipID, p, 0x4c + c, fmRegisterYM2610[ChipID][p][0x4c + c]);

            }
            else if (ch < 9)
            {
                YM2610SetRegister(null, Counter, ChipID, 0, 0x08 + ch - 6, fmRegisterYM2610[ChipID][0][0x08 + ch - 6]);
            }
            else if (ch < 12)
            {
                YM2610SetRegister(null, Counter, ChipID, 0, 0x40 + 2, fmRegisterYM2610[ChipID][0][0x40 + 2]);
                YM2610SetRegister(null, Counter, ChipID, 0, 0x44 + 2, fmRegisterYM2610[ChipID][0][0x44 + 2]);
                YM2610SetRegister(null, Counter, ChipID, 0, 0x48 + 2, fmRegisterYM2610[ChipID][0][0x48 + 2]);
                YM2610SetRegister(null, Counter, ChipID, 0, 0x4c + 2, fmRegisterYM2610[ChipID][0][0x4c + 2]);

            }
        }

        public void YM2610WriteClock(byte chipID, int clock)
        {
            if (scYM2610 != null && scYM2610.Length > chipID && scYM2610[chipID] != null)
            {
                scYM2610[chipID].dClock = scYM2610[chipID].SetMasterClock((uint)clock);
            }
        }

        public void YM2610SetFadeoutVolume(long Counter, int v)
        {
            for (int i = 0; i < YM2610.Count; i++)
            {
                if (!YM2610[i].Use) continue;
                if (YM2610[i].Model == EnmVRModel.VirtualModel) continue;
                if ((nowYM2610FadeoutVol[i] >> 3) == (v >> 3)) continue;

                nowYM2610FadeoutVol[i] = v;

                for (int p = 0; p < 2; p++)
                {
                    for (int c = 0; c < 3; c++)
                    {
                        YM2610SetRegister(null, Counter, i, p, 0x40 + c, fmRegisterYM2610[i][p][0x40 + c]);
                        YM2610SetRegister(null, Counter, i, p, 0x44 + c, fmRegisterYM2610[i][p][0x44 + c]);
                        YM2610SetRegister(null, Counter, i, p, 0x48 + c, fmRegisterYM2610[i][p][0x48 + c]);
                        YM2610SetRegister(null, Counter, i, p, 0x4c + c, fmRegisterYM2610[i][p][0x4c + c]);
                    }
                }

                //ssg
                YM2610SetRegister(null, Counter, i, 0, 0x08, fmRegisterYM2610[i][0][0x08]);
                YM2610SetRegister(null, Counter, i, 0, 0x09, fmRegisterYM2610[i][0][0x09]);
                YM2610SetRegister(null, Counter, i, 0, 0x0a, fmRegisterYM2610[i][0][0x0a]);

                //rhythm
                YM2610SetRegister(null, Counter, i, 1, 0x01, fmRegisterYM2610[i][1][0x01]);

                //adpcm
                YM2610SetRegister(null, Counter, i, 0, 0x1b, fmRegisterYM2610[i][0][0x1b]);
            }
        }

        public void YM2610SetSyncWait(byte chipID, int wait)
        {
            if (scYM2610[chipID] != null && ctYM2610[chipID].UseWait)
            {
                scYM2610[chipID].setRegister(-1, (int)(wait * (ctYM2610[chipID].UseWaitBoost ? 2.0 : 1.0)));
            }
        }

        public void YM2610SetSSGVolume(byte chipID, int vol)
        {
            if (scYM2610 != null && scYM2610[chipID] != null)
            {
                scYM2610[chipID].setSSGVolume((byte)vol);
            }
        }

        #endregion



        #region YM2609(OPNA2)

        private void YM2609WriteRegisterControl(Chip Chip, EnmDataType type, int address, int data, object exData)
        {
            if (type == EnmDataType.Normal)
            {
                if (Chip.Model == EnmVRModel.VirtualModel)
                {
                    if (!ctYM2609[Chip.Number].UseScci && ctYM2609[Chip.Number].UseEmu)
                    {
                        mds.WriteYM2609(Chip.Index, (byte)Chip.Number, (byte)(address >> 8), (byte)address, (byte)data);
                        //if(address==0xb4) Console.WriteLine("{0:x02} : {1:x02} : {2:x02} : {3:x02}", (byte)Chip.Number, (byte)(address >> 8), (byte)address, (byte)data);
                    }
                }
                if (Chip.Model == EnmVRModel.RealModel)
                {
                }
            }
            else if (type == EnmDataType.Block)
            {
                Audio.sm.SetInterrupt();

                try
                {
                    if (exData == null) return;

                    if (exData is PackData[])
                    {
                        PackData[] pdata = (PackData[])exData;
                        if (Chip.Model == EnmVRModel.VirtualModel)
                        {
                            //log.Write("Sending YM2609(Emu) ADPCM");
                            foreach (PackData dat in pdata)
                                mds.WriteYM2609(dat.Chip.Index, (byte)dat.Chip.Number, (byte)(dat.Address >> 8), (byte)dat.Address, (byte)dat.Data);
                        }
                        if (Chip.Model == EnmVRModel.RealModel)
                        {
                        }
                        return;
                    }
                    byte[] adpcmData = (byte[])exData;
                    log.Write(string.Format("Sending YM2609(Emu) ADPCM-{0}", (data == -1) ? "A" : "B"));
                    if (data == -1)
                    {
                        mds.WriteYM2609_SetAdpcmA(Chip.Index, (byte)Chip.Number, adpcmData);
                    }
                }
                finally
                {
                    Audio.sm.ResetInterrupt();
                }
            }
        }

        public void YM2609SetRegisterProcessing(ref outDatum od, ref long Counter, ref Chip Chip, ref EnmDataType Type, ref int Address, ref int dData, ref object ExData)
        {
            if (ctYM2609 == null) return;
            if (Address == -1 && dData == -1) return;

            if (Chip.Number == 0) chipLED.PriOPNA = 2;
            else chipLED.SecOPNA = 2;

            if (Type != EnmDataType.Normal) return;

            int ch;

            // FMch:0-12
            // FMexch:13-18

            //FM ch<18
            if (Address == 0x28 || Address == 0x228)
            {
                ch = dData & 0x3;
                ch += ((dData & 0x4) == 0) ? 0 : 3;
                ch += (Address == 0x228) ? 6 : 0;

                if (Chip.ChMasks[ch]) dData &= 0xf;
            }

            //SSG ch<30
            int adl = Address & 0x00f;
            if (adl == 8 || adl == 9 || adl == 10)
            {
                int adr = Address & 0x3f0;

                //SSG1
                if (adr == 0x000)
                {
                    ch = adl - 8 + 18 + 0;
                    if (Chip.ChMasks[ch])
                        dData = 0;
                }
                //SSG2
                else if (adr == 0x120)
                {
                    ch = adl - 8 + 18 + 3;
                    if (Chip.ChMasks[ch]) dData = 0;
                }
                //SSG3
                else if (adr == 0x200)
                {
                    ch = adl - 8 + 18 + 6;
                    if (Chip.ChMasks[ch]) dData = 0;
                }
                //SSG4
                else if (adr == 0x210)
                {
                    ch = adl - 8 + 18 + 9;
                    if (Chip.ChMasks[ch]) dData = 0;
                }
            }

            //リズム ch<36
            if (Address == 0x10)
            {
                byte mask = 0x80;
                for (int i = 0; i < 6; i++)
                {
                    mask |= (byte)((Chip.ChMasks[i + 30] ? 0 : 1) << i);
                }

                dData &= mask;
            }

            //ADPCM1 ch=36
            if (Address == 0x100)
            {
                if (Chip.ChMasks[36]) dData &= 0x7f;
            }

            //ADPCM2 ch=37
            if (Address == 0x300)
            {
                if (Chip.ChMasks[37]) dData &= 0x7f;
            }
            //ADPCM3 ch=38
            if (Address == 0x311)
            {
                if (Chip.ChMasks[38]) dData &= 0x7f;
            }
            //18+12+6+3

            //ADPCM-A ch=39, 40, 41, 42, 43, 44
            if (Address == 0x111)
            {
                byte mask = 0x80;
                for (int i = 0; i < 6; i++)
                {
                    mask |= (byte)((Chip.ChMasks[i + 39] ? 0 : 1) << i);
                }

                dData &= mask;
            }

        }

        public void YM2609SetRegister(outDatum od, long Counter, int ChipID, int dPort, int dAddr, int dData)
        {
            dummyChip.Move(YM2609[ChipID]);

            int address = dPort * 0x100 + dAddr;
            if ((address >= 0x100 && address <= 0x110) && ctYM2609[0].OnlyPCMEmulation)
            {
                dummyChip.Model = EnmVRModel.VirtualModel;
            }

            enq(od, Counter, dummyChip, EnmDataType.Normal, dPort * 0x100 + dAddr, dData, null);
        }

        public void YM2609SetRegister(outDatum od, long Counter, Chip chip, PackData[] data)
        {
            enq(od, Counter, chip, EnmDataType.Block, -1, -1, data);
        }

        public void YM2609WriteClock(byte chipID, int clock)
        {
            if (scYM2609 != null && scYM2609.Length > chipID && scYM2609[chipID] != null)
            {
                scYM2609[chipID].dClock = scYM2609[chipID].SetMasterClock((uint)clock);
            }
        }

        public void YM2609SoftReset(long Counter, int chipID)
        {
            List<PackData> data = YM2609MakeSoftReset(chipID);
            YM2609SetRegister(null, Counter, YM2609[chipID], data.ToArray());
        }

        public List<PackData> YM2609MakeSoftReset(int chipID)
        {
            List<PackData> data = new List<PackData>();
            int i;

            // FM全チャネルキーオフ
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x028, 0x00, null));
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x028, 0x01, null));
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x028, 0x02, null));
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x028, 0x04, null));
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x028, 0x05, null));
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x028, 0x06, null));
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x028, 0x00, null));
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x228, 0x01, null));
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x228, 0x02, null));
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x228, 0x04, null));
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x228, 0x05, null));
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x228, 0x06, null));

            // FM TL=127
            for (i = 0x40; i < 0x4F + 1; i++)
            {
                data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x000 + i, 0x7f, null));
                data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x100 + i, 0x7f, null));
                data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x200 + i, 0x7f, null));
                data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x300 + i, 0x7f, null));
            }
            // FM ML/DT
            for (i = 0x30; i < 0x3F + 1; i++)
            {
                data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, i, 0x0, null));
                data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x100 + i, 0x0, null));
                data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x200 + i, 0x0, null));
                data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x300 + i, 0x0, null));
            }
            // FM AR,DR,SR,KS,AMON
            for (i = 0x50; i < 0x7F + 1; i++)
            {
                data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, i, 0x0, null));
                data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x100 + i, 0x0, null));
                data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x200 + i, 0x0, null));
                data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x300 + i, 0x0, null));
            }
            // FM SL,RR
            for (i = 0x80; i < 0x8F + 1; i++)
            {
                data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, i, 0xff, null));
                data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x100 + i, 0xff, null));
                data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x200 + i, 0xff, null));
                data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x300 + i, 0xff, null));
            }
            // FM F-Num, FB/CONNECT
            for (i = 0x90; i < 0xBF + 1; i++)
            {
                data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, i, 0x0, null));
                data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x100 + i, 0x0, null));
                data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x200 + i, 0x0, null));
                data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x300 + i, 0x0, null));
            }
            // FM PAN/AMS/PMS
            for (i = 0xB4; i < 0xB6 + 1; i++)
            {
                data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, i, 0xc0, null));
                data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x100 + i, 0xc0, null));
                data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x200 + i, 0xc0, null));
                data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x300 + i, 0xc0, null));
            }
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x022, 0x00, null)); // HW LFO
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x024, 0x00, null)); // Timer-A(1)
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x025, 0x00, null)); // Timer-A(2)
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x026, 0x00, null)); // Timer-B
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x027, 0x30, null)); // Timer Control
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x029, 0x80, null)); // FM4-6 Enable
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x222, 0x00, null)); // HW LFO
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x224, 0x00, null)); // Timer-A(1)
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x225, 0x00, null)); // Timer-A(2)
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x226, 0x00, null)); // Timer-B
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x227, 0x30, null)); // Timer Control
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x229, 0x80, null)); // FM4-6 Enable

            // SSG 音程(2byte*3ch)
            for (i = 0x00; i < 0x05 + 1; i++)
            {
                data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, i, 0x00, null));
                data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x120 + i, 0x00, null));
                data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x200 + i, 0x00, null));
                data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x210 + i, 0x00, null));
            }
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x06, 0x00, null)); // SSG ノイズ周波数
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x07, 0x38, null)); // SSG ミキサ
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x120 + 0x06, 0x00, null)); // SSG ノイズ周波数
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x120 + 0x07, 0x38, null)); // SSG ミキサ
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x200 + 0x06, 0x00, null)); // SSG ノイズ周波数
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x200 + 0x07, 0x38, null)); // SSG ミキサ
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x210 + 0x06, 0x00, null)); // SSG ノイズ周波数
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x210 + 0x07, 0x38, null)); // SSG ミキサ
                                                                                                        // SSG ボリューム(3ch)
            for (i = 0x08; i < 0x0A + 1; i++)
            {
                data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, i, 0x00, null));
                data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x120 + i, 0x00, null));
                data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x200 + i, 0x00, null));
                data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x210 + i, 0x00, null));
            }
            // SSG Envelope
            for (i = 0x0B; i < 0x0D + 1; i++)
            {
                data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, i, 0x00, null));
                data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x120 + i, 0x00, null));
                data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x200 + i, 0x00, null));
                data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x210 + i, 0x00, null));
            }

            // RHYTHM
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x10, 0xBF, null)); // 強制発音停止
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x11, 0x00, null)); // Total Level
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x18, 0x00, null)); // BD音量
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x19, 0x00, null)); // SD音量
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x1A, 0x00, null)); // CYM音量
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x1B, 0x00, null)); // HH音量
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x1C, 0x00, null)); // TOM音量
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x1D, 0x00, null)); // RIM音量

            // ADPCM              
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x100 + 0x00, 0x21, null)); // ADPCMリセット
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x100 + 0x01, 0x06, null)); // ADPCM消音
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x100 + 0x10, 0x9C, null)); // FLAGリセット        
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x300 + 0x00, 0x21, null)); // ADPCMリセット
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x300 + 0x01, 0x06, null)); // ADPCM消音
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x300 + 0x10, 0x9C, null)); // FLAGリセット        
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x311 + 0x00, 0x21, null)); // ADPCMリセット
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x311 + 0x01, 0x06, null)); // ADPCM消音
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x311 + 0x10, 0x9C, null)); // FLAGリセット        

            // ADPCM-A
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x111, 0xBF, null)); // 強制発音停止
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x112, 0x00, null)); // Total Level
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x113, 0x00, null)); // BD音量
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x114, 0x00, null));
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x113, 0x01, null)); // SD音量
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x114, 0x00, null));
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x113, 0x02, null)); // CYM音量
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x114, 0x00, null));
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x113, 0x03, null)); // HH音量
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x114, 0x00, null));
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x113, 0x04, null)); // TOM音量
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x114, 0x00, null));
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x113, 0x05, null)); // RIM音量
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x114, 0x00, null));

            //Effect
            data.Add(new PackData(null, YM2609[chipID], EnmDataType.Normal, 0x323, 0x80, null));

            return data;
        }

        public void YM2609WriteSetAdpcmA(outDatum od, long Counter, int ChipID, byte[] ym2609AdpcmA)
        {
            dummyChip.Move(YM2609[ChipID]);

            if (ctYM2609[0].OnlyPCMEmulation)
            {
                dummyChip.Model = EnmVRModel.VirtualModel;
            }

            enq(od, Counter, dummyChip, EnmDataType.Block, -1, -1, ym2609AdpcmA);

        }

        #endregion



        #region YM2612(OPN2)

        private void YM2612WriteRegisterControl(Chip Chip, EnmDataType type, int address, int data, object exData)
        {
            if (type == EnmDataType.Normal)
            {
                byte port = (byte)(address >> 8);
                
                //Console.WriteLine("Adr:{0:x02} Dat:{1:x02}",address,data);

                if (Chip.Model == EnmVRModel.VirtualModel)
                {
                    if (ctYM2612[Chip.Number] == null || !ctYM2612[Chip.Number].UseScci)
                    {
                        if (mds == null) return;
                        if (ctYM2612[Chip.Number] == null || ctYM2612[Chip.Number].UseEmu)
                        {
                            if (port == 10)
                                mds.PlayPCM_YM2612X(Chip.Index, (byte)Chip.Number, port, (byte)address, (byte)data);
                            else
                                //Gens
                                mds.WriteYM2612(Chip.Index, (byte)Chip.Number, (byte)(address >> 8), (byte)address, (byte)data);
                        }
                        else if (ctYM2612[Chip.Number].UseEmu2)
                        {
                            if (port == 10)
                                mds.PlayPCM_YM3438X(Chip.Index, (byte)Chip.Number, port, (byte)address, (byte)data);
                            else
                                //Nuked
                                mds.WriteYM3438(Chip.Index, (byte)Chip.Number, (byte)(address >> 8), (byte)address, (byte)data);
                        }
                        else if (ctYM2612[Chip.Number].UseEmu3)
                        {
                            if (port == 10)
                                mds.PlayPCM_YM2612mameX(Chip.Index, (byte)Chip.Number, port, (byte)address, (byte)data);
                            else
                                //mame
                                mds.WriteYM2612mame(Chip.Index, (byte)Chip.Number, (byte)(address >> 8), (byte)address, (byte)data);
                        }
                    }
                    else if (ctYM2612[Chip.Number].OnlyPCMEmulation)
                    {
                        if (address == 0x2a || address == 0x2b || address == 0x1b6)
                        {
                            if (mds == null) return;
                            if (ctYM2612[Chip.Number].UseEmu)
                            {
                                mds.WriteYM2612(Chip.Index, (byte)Chip.Number, (byte)(address >> 8), (byte)address, (byte)data);
                            }
                            else if (ctYM2612[Chip.Number].UseEmu2)
                            {
                                mds.WriteYM3438(Chip.Index, (byte)Chip.Number, (byte)(address >> 8), (byte)address, (byte)data);
                            }
                            else if (ctYM2612[Chip.Number].UseEmu3)
                            {
                                mds.WriteYM2612mame(Chip.Index, (byte)Chip.Number, (byte)(address >> 8), (byte)address, (byte)data);
                            }
                        }
                    }
                }
                else if (Chip.Model == EnmVRModel.RealModel)
                {
                    if (address != -1 && data != -1)
                    {
                        if (scYM2612[Chip.Number] != null)
                            scYM2612[Chip.Number].setRegister(address, data);
                        //Console.WriteLine("{0:x02} {1:x02}", address, data);
                    }
                }
            }
            else if (type == EnmDataType.Block)
            {
                Audio.sm.SetInterrupt();

                try
                {
                    if (exData == null) return;

                    PackData[] pdata = (PackData[])exData;
                    if (Chip.Model == EnmVRModel.VirtualModel)
                    {
                        if (mds == null) return;
                        //foreach (PackData dat in pdata)
                        //{
                        //    mds.WriteYM2612(dat.Chip.Index, (byte)dat.Chip.Number, (byte)(dat.Address >> 8), (byte)dat.Address, (byte)dat.Data);
                        //}

                        if (ctYM2612[Chip.Number].UseEmu)
                        {
                            foreach (PackData dat in pdata)
                                if ((dat.Address & 0xf00) == 0xa00)
                                    mds.PlayPCM_YM2612X(dat.Chip.Index, (byte)dat.Chip.Number, 10, (byte)address, (byte)data);
                                else
                                    mds.WriteYM2612(dat.Chip.Index, (byte)dat.Chip.Number, (byte)(dat.Address >> 8), (byte)dat.Address, (byte)dat.Data);
                        }
                        else if (ctYM2612[Chip.Number].UseEmu2)
                        {
                            foreach (PackData dat in pdata)
                                if ((dat.Address & 0xf00) == 0xa00)
                                    mds.PlayPCM_YM3438X(dat.Chip.Index, (byte)dat.Chip.Number, 10, (byte)address, (byte)data);
                                else
                                    mds.WriteYM3438(dat.Chip.Index, (byte)dat.Chip.Number, (byte)(dat.Address >> 8), (byte)dat.Address, (byte)dat.Data);
                        }
                        else if (ctYM2612[Chip.Number].UseEmu3)
                        {
                            foreach (PackData dat in pdata)
                                if ((dat.Address & 0xf00) == 0xa00)
                                    mds.PlayPCM_YM2612mameX(dat.Chip.Index, (byte)dat.Chip.Number, 10, (byte)address, (byte)data);
                                else
                                    mds.WriteYM2612mame(dat.Chip.Index, (byte)dat.Chip.Number, (byte)(dat.Address >> 8), (byte)dat.Address, (byte)dat.Data);
                        }
                    }
                    if (Chip.Model == EnmVRModel.RealModel)
                    {
                        //foreach (PackData dat in pdata)
                        //scYM2612[Chip.Number].setRegister(dat.Address, dat.Data);
                        //Audio.realChip.WaitOPN2DPCMData();
                    }
                }
                finally
                {
                    Audio.sm.ResetInterrupt();
                }
            }
        }

        public void YM2612SetRegisterProcessing(ref long Counter, ref Chip Chip, ref EnmDataType Type, ref int Address, ref int dData, ref object ExData)
        {
            //XGM
            if ((Address & 0xf00) == 0xa00)
                return;

            if (ctYM2612 == null) return;
            if (Address == -1 && dData == -1)
            {
                return;
            }

            if (Chip.Number == 0) chipLED.PriOPN2 = 2;
            else chipLED.SecOPN2 = 2;

            int dPort = (Address >> 8);
            int dAddr = (Address & 0xff);
            //int pddata = dData;

            fmRegisterYM2612[Chip.Number][dPort][dAddr] = dData;

            //if (Chip.Model == EnmVRModel.VirtualModel)
            //{
                //midiExport.outMIDIData(EnmChip.YM2612, Chip.Number, dPort, dAddr, dData, 0, Counter);
            //}

            if ((Chip.Model == EnmVRModel.RealModel && ctYM2612[Chip.Number].UseScci) || (Chip.Model == EnmVRModel.VirtualModel && !ctYM2612[Chip.Number].UseScci))
            {
                //fmRegister[dPort][dAddr] = dData;
                if (dPort == 0 && dAddr == 0x28)
                {
                    int ch = (dData & 0x3) + ((dData & 0x4) > 0 ? 3 : 0);
                    if (ch >= 0 && ch < 6)// && (dData & 0xf0)>0)
                    {
                        if (ch != 2 || (fmRegisterYM2612[Chip.Number][0][0x27] & 0xc0) != 0x40)
                        {
                            if (ch != 5 || (fmRegisterYM2612[Chip.Number][0][0x2b] & 0x80) == 0)
                            {
                                if ((dData & 0xf0) != 0)
                                {
                                    fmKeyOnYM2612[Chip.Number][ch] = (dData & 0xf0) | 1;
                                    fmVolYM2612[Chip.Number][ch] = 256 * 6;
                                }
                                else
                                {
                                    fmKeyOnYM2612[Chip.Number][ch] = (dData & 0xf0) | 0;
                                }
                            }
                        }
                        else
                        {
                            fmKeyOnYM2612[Chip.Number][2] = (dData & 0xf0);
                            if ((dData & 0x10) > 0) fmCh3SlotVolYM2612[Chip.Number][0] = 256 * 6;
                            if ((dData & 0x20) > 0) fmCh3SlotVolYM2612[Chip.Number][1] = 256 * 6;
                            if ((dData & 0x40) > 0) fmCh3SlotVolYM2612[Chip.Number][2] = 256 * 6;
                            if ((dData & 0x80) > 0) fmCh3SlotVolYM2612[Chip.Number][3] = 256 * 6;
                        }
                    }
                }

                //PCM
                if ((fmRegisterYM2612[Chip.Number][0][0x2b] & 0x80) > 0)
                {
                    if (fmRegisterYM2612[Chip.Number][0][0x2a] > 0)
                    {
                        fmVolYM2612[Chip.Number][5] = Math.Abs(fmRegisterYM2612[Chip.Number][0][0x2a] - 0x7f) * 20;
                    }
                }
            }

            if ((dAddr & 0xf0) == 0x40)//TL
            {
                int ch = (dAddr & 0x3);
                dData &= 0x7f;

                if (ch != 3)
                {
                    int al = fmRegisterYM2612[Chip.Number][dPort][0xb0 + ch] & 0x07;
                    int i = (dAddr & 0xc) >> 2;
                    int slot = (i == 0) ? 0 : ((i == 1) ? 1 : ((i == 2) ? 2 : 3));
                    if ((algM[al] & (1 << slot)) > 0)
                    {
                        dData = Math.Min(dData + nowYM2612FadeoutVol[Chip.Number], 127);
                        dData = maskFMChYM2612[Chip.Number][dPort * 3 + ch] ? 127 : dData;
                        //if (nowYM2612FadeoutVol[Chip.Number] != 0)
                        //{
                        //log.Write(string.Format("fv{0}", nowYM2612FadeoutVol[Chip.Number]));
                        //log.Write(string.Format("ddata{0}", dData));
                        //}
                    }
                }
            }

            if ((dAddr & 0xf0) == 0xb0)//AL
            {
                int ch = (dAddr & 0x3);
                int al = dData & 0x07;//AL

                if (ch != 3 && maskFMChYM2612[Chip.Number][ch])
                {
                    for (int i = 0; i < 4; i++)
                    {
                        int slot = (i == 0) ? 0 : ((i == 1) ? 2 : ((i == 2) ? 1 : 3));
                        if ((algM[al] & (1 << slot)) > 0)
                        {
                            //setYM2612Register(Counter,Chip.Number, dPort, 0x40 + ch + slot * 4, fmRegisterYM2612[Chip.Number][dPort][0x40 + ch]);
                        }
                    }
                }
            }

            if (dPort == 0 && dAddr == 0x2a)
            {
                if (maskFMChYM2612[Chip.Number][5]) dData = 0x80;
            }

            //if(dPort==0 && dAddr == 0x4c)
            //{
            //    log.Write(string.Format("Counter {0}  Ch1 op4 ddata{1}", Counter, dData));
            //    if (dData < oldddata)
            //    {
            //        ;
            //    }
            //    oldddata = dData;
            //    oldpddata = pddata;
            //}



            //if (Chip.Model == EnmModel.VirtualModel)
            //{
            //    //仮想音源の処理
            //    if (ctYM2612[Chip.Number].UseScci)
            //    {
            //        //Scciを使用する場合でも
            //        //PCM(6Ch)だけエミュで発音するとき
            //        if (ctYM2612[Chip.Number].OnlyPCMEmulation)
            //        {
            //            //if (dPort == 0 && dAddr == 0x2b)
            //            //{
            //            //    if (ctYM2612[Chip.Number].UseEmu) mds.WriteYM2612((byte)Chip.Number, (byte)dPort, (byte)dAddr, (byte)dData);
            //            //    if (ctYM2612[Chip.Number].UseEmu2) mds.WriteYM3438((byte)Chip.Number, (byte)dPort, (byte)dAddr, (byte)dData);
            //            //}
            //            //else if (dPort == 0 && dAddr == 0x2a)
            //            //{
            //            //    if (ctYM2612[Chip.Number].UseEmu) mds.WriteYM2612((byte)Chip.Number, (byte)dPort, (byte)dAddr, (byte)dData);
            //            //    if (ctYM2612[Chip.Number].UseEmu2) mds.WriteYM3438((byte)Chip.Number, (byte)dPort, (byte)dAddr, (byte)dData);
            //            //}
            //            //else if (dPort == 1 && dAddr == 0xb6)
            //            //{
            //            //    if (ctYM2612[Chip.Number].UseEmu) mds.WriteYM2612((byte)Chip.Number, (byte)dPort, (byte)dAddr, (byte)dData);
            //            //    if (ctYM2612[Chip.Number].UseEmu2) mds.WriteYM3438((byte)Chip.Number, (byte)dPort, (byte)dAddr, (byte)dData);
            //            //}
            //        }
            //    }
            //    //else
            //    //{
            //    //    //エミュを使用する場合のみMDSoundへデータを送る
            //    //    if (ctYM2612[Chip.Number].UseEmu) mds.WriteYM2612((byte)Chip.Number, (byte)dPort, (byte)dAddr, (byte)dData);
            //    //    if (ctYM2612[Chip.Number].UseEmu2) mds.WriteYM3438((byte)Chip.Number, (byte)dPort, (byte)dAddr, (byte)dData);
            //    //}
            //}
            //else
            //{

            //    //実音源(Scci)

            //    if (scYM2612[Chip.Number] == null) return;

            //    //PCM(6Ch)だけエミュで発音するとき
            //    if (ctYM2612[Chip.Number].OnlyPCMEmulation)
            //    {
            //        //アドレスを調べてPCMにはデータを送らない
            //        if (dPort == 0 && dAddr == 0x2b)
            //        {
            //            //scYM2612[Chip.Number].setRegister(dPort * 0x100 + dAddr, dData);
            //            Chip.Model = EnmModel.VirtualModel;
            //        }
            //        else if (dPort == 0 && dAddr == 0x2a)
            //        {
            //            Chip.Model = EnmModel.VirtualModel;
            //        }
            //        else
            //        {
            //            //scYM2612[Chip.Number].setRegister(dPort * 0x100 + dAddr, dData);
            //        }
            //    }
            //    //else
            //    //{
            //    //    //Scciへデータを送る
            //    //    scYM2612[Chip.Number].setRegister(dPort * 0x100 + dAddr, dData);
            //    //}
            //}

            //FM ch<9
            if (Address == 0x28)
            {
                int ch = dData & 0x3;
                ch += ((dData & 0x4) == 0) ? 0 : 3;

                if (Chip.ChMasks[ch]) dData &= 0xf;
            }

            if (dPort == 0 && dAddr == 0x2a)
            {
                if (Chip.ChMasks[5]) dData = 0x80;
            }
        }

        public void YM2612SetRegister(outDatum od, long Counter, int ChipID, int dPort, int dAddr, int dData)
        {
            dummyChip.Move(YM2612[ChipID]);

            if (ctYM2612[0].OnlyPCMEmulation)
            {
                if (dPort == 0x00)
                {
                    if (dAddr == 0x2a)
                    {
                        dummyChip.Model = EnmVRModel.VirtualModel;
                    }
                    else if (dAddr == 0x2b)
                    {
                        if ((dData & 0x80) != 0)
                        {
                            dummyChip.Model = EnmVRModel.VirtualModel;
                        }
                    }
                }

                if (dPort == 0x01)
                {
                    if (dAddr == 0xb6 && YM2612[ChipID].Model != EnmVRModel.VirtualModel)
                    {
                        enq(od, Counter, YM2612[ChipID], EnmDataType.Normal, dPort * 0x100 + dAddr, dData, null);
                        dummyChip.Model = EnmVRModel.VirtualModel;
                    }
                }
            }

            enq(od, Counter, dummyChip, EnmDataType.Normal, dPort * 0x100 + dAddr, dData, null);
        }

        public void YM2612SetRegister(outDatum od, long Counter, int ChipID, PackData[] data)
        {
            enq(od, Counter, YM2612[ChipID], EnmDataType.Block, -1, -1, data);
        }

        public void YM2612SetRegisterXGM(long Counter, int dData)
        {
            dummyChip.Move(YM2612[0]);
            dummyChip.Model = EnmVRModel.VirtualModel;

            enq(null, Counter, dummyChip, EnmDataType.Normal, 0x2a, dData, null);
        }

        public void YM2612SoftReset(long Counter, int chipID)
        {
            List<PackData> data = YM2612MakeSoftReset(chipID);
            YM2612SetRegister(null, Counter, chipID, data.ToArray());
        }

        public void YM2612SetMask(long Counter, int chipID, int ch, bool mask)
        {
            maskFMChYM2612[chipID][ch] = mask;

            int c = (ch < 3) ? ch : (ch - 3);
            int p = (ch < 3) ? 0 : 1;

            YM2612SetRegister(null, Counter, (byte)chipID, p, 0x40 + c, fmRegisterYM2612[chipID][p][0x40 + c]);
            YM2612SetRegister(null, Counter, (byte)chipID, p, 0x44 + c, fmRegisterYM2612[chipID][p][0x44 + c]);
            YM2612SetRegister(null, Counter, (byte)chipID, p, 0x48 + c, fmRegisterYM2612[chipID][p][0x48 + c]);
            YM2612SetRegister(null, Counter, (byte)chipID, p, 0x4c + c, fmRegisterYM2612[chipID][p][0x4c + c]);

        }

        public void YM2612WriteClock(byte chipID, int clock)
        {
            if (scYM2612 != null && scYM2612.Length > chipID && scYM2612[chipID] != null)
            {
                scYM2612[chipID].dClock = scYM2612[chipID].SetMasterClock((uint)clock);
            }
        }

        public void YM2612SetFadeoutVolume(long Counter, int v)
        {
            for (int i = 0; i < YM2612.Count; i++)
            {
                if (!YM2612[i].Use) continue;
                if (YM2612[i].Model == EnmVRModel.VirtualModel) continue;
                if (nowYM2612FadeoutVol[i] == v) continue;

                nowYM2612FadeoutVol[i] = v;

                for (int p = 0; p < 2; p++)
                {
                    for (int c = 0; c < 3; c++)
                    {
                        YM2612SetRegister(null, Counter, i, p, 0x40 + c, fmRegisterYM2612[i][p][0x40 + c]);
                        YM2612SetRegister(null, Counter, i, p, 0x44 + c, fmRegisterYM2612[i][p][0x44 + c]);
                        YM2612SetRegister(null, Counter, i, p, 0x48 + c, fmRegisterYM2612[i][p][0x48 + c]);
                        YM2612SetRegister(null, Counter, i, p, 0x4c + c, fmRegisterYM2612[i][p][0x4c + c]);
                        //if (c == 0 && p == 0)
                        //{
                        //    log.Write(string.Format("send Counter{0} Ch1 op4 ddata{1} v:{2}", Counter, fmRegisterYM2612[Chip.Number][p][0x4c], v));
                        //}
                    }
                }
            }
        }

        public List<PackData> YM2612MakeSoftReset(int chipID)
        {
            List<PackData> data = new List<PackData>();
            int i;

            // FM全チャネルキーオフ
            data.Add(new PackData(null, YM2612[chipID], EnmDataType.Normal, 0x28, 0x00, null));
            data.Add(new PackData(null, YM2612[chipID], EnmDataType.Normal, 0x28, 0x01, null));
            data.Add(new PackData(null, YM2612[chipID], EnmDataType.Normal, 0x28, 0x02, null));
            data.Add(new PackData(null, YM2612[chipID], EnmDataType.Normal, 0x28, 0x04, null));
            data.Add(new PackData(null, YM2612[chipID], EnmDataType.Normal, 0x28, 0x05, null));
            data.Add(new PackData(null, YM2612[chipID], EnmDataType.Normal, 0x28, 0x06, null));

            // FM TL=127
            for (i = 0x40; i < 0x4F + 1; i++)
            {
                data.Add(new PackData(null, YM2612[chipID], EnmDataType.Normal, 0x000 + i, 0x7f, null));
                data.Add(new PackData(null, YM2612[chipID], EnmDataType.Normal, 0x100 + i, 0x7f, null));
            }
            // FM ML/DT
            for (i = 0x30; i < 0x3F + 1; i++)
            {
                data.Add(new PackData(null, YM2612[chipID], EnmDataType.Normal, i, 0x0, null));
                data.Add(new PackData(null, YM2612[chipID], EnmDataType.Normal, 0x100 + i, 0x0, null));
            }
            // FM AR,DR,SR,KS,AMON
            for (i = 0x50; i < 0x7F + 1; i++)
            {
                data.Add(new PackData(null, YM2612[chipID], EnmDataType.Normal, i, 0x0, null));
                data.Add(new PackData(null, YM2612[chipID], EnmDataType.Normal, 0x100 + i, 0x0, null));
            }
            // FM SL,RR
            for (i = 0x80; i < 0x8F + 1; i++)
            {
                data.Add(new PackData(null, YM2612[chipID], EnmDataType.Normal, i, 0xff, null));
                data.Add(new PackData(null, YM2612[chipID], EnmDataType.Normal, 0x100 + i, 0xff, null));
            }
            // FM F-Num, FB/CONNECT
            for (i = 0x90; i < 0xBF + 1; i++)
            {
                data.Add(new PackData(null, YM2612[chipID], EnmDataType.Normal, i, 0x0, null));
                data.Add(new PackData(null, YM2612[chipID], EnmDataType.Normal, 0x100 + i, 0x0, null));
            }
            // FM PAN/AMS/PMS
            for (i = 0xB4; i < 0xB6 + 1; i++)
            {
                data.Add(new PackData(null, YM2612[chipID], EnmDataType.Normal, i, 0xc0, null));
                data.Add(new PackData(null, YM2612[chipID], EnmDataType.Normal, 0x100 + i, 0xc0, null));
            }
            data.Add(new PackData(null, YM2612[chipID], EnmDataType.Normal, 0x22, 0x00, null)); // HW LFO
            data.Add(new PackData(null, YM2612[chipID], EnmDataType.Normal, 0x24, 0x00, null)); // Timer-A(1)
            data.Add(new PackData(null, YM2612[chipID], EnmDataType.Normal, 0x25, 0x00, null)); // Timer-A(2)
            data.Add(new PackData(null, YM2612[chipID], EnmDataType.Normal, 0x26, 0x00, null)); // Timer-B
            data.Add(new PackData(null, YM2612[chipID], EnmDataType.Normal, 0x27, 0x30, null)); // Timer Control
            data.Add(new PackData(null, YM2612[chipID], EnmDataType.Normal, 0x29, 0x80, null)); // FM4-6 Enable
            data.Add(new PackData(null, YM2612[chipID], EnmDataType.Normal, 0x2a, 0x80, null)); // PCM 0

            data.Add(new PackData(null, YM2612[chipID], EnmDataType.Normal, 0x0a00, 0x00, null)); // XGMPCM Ch1 OFF
            data.Add(new PackData(null, YM2612[chipID], EnmDataType.Normal, 0x0a01, 0x00, null)); // XGMPCM Ch2 OFF
            data.Add(new PackData(null, YM2612[chipID], EnmDataType.Normal, 0x0a02, 0x00, null)); // XGMPCM Ch3 OFF
            data.Add(new PackData(null, YM2612[chipID], EnmDataType.Normal, 0x0a03, 0x00, null)); // XGMPCM Ch4 OFF

            return data;
        }

        public List<PackData> YM2612MakeSoftResetKeyOffOnly(int chipID)
        {
            List<PackData> data = new List<PackData>();
            //int i;

            // FM全チャネルキーオフ
            data.Add(new PackData(null, YM2612[chipID], EnmDataType.Normal, 0x28, 0x00, null));
            data.Add(new PackData(null, YM2612[chipID], EnmDataType.Normal, 0x28, 0x01, null));
            data.Add(new PackData(null, YM2612[chipID], EnmDataType.Normal, 0x28, 0x02, null));
            data.Add(new PackData(null, YM2612[chipID], EnmDataType.Normal, 0x28, 0x04, null));
            data.Add(new PackData(null, YM2612[chipID], EnmDataType.Normal, 0x28, 0x05, null));
            data.Add(new PackData(null, YM2612[chipID], EnmDataType.Normal, 0x28, 0x06, null));

            data.Add(new PackData(null, YM2612[chipID], EnmDataType.Normal, 0x29, 0x80, null)); // FM4-6 Enable
            data.Add(new PackData(null, YM2612[chipID], EnmDataType.Normal, 0x2a, 0x80, null)); // PCM 0

            return data;
        }

        public void YM2612SetSyncWait(byte chipID, int wait)
        {
            if (scYM2612[chipID] != null && ctYM2612[chipID].UseWait)
            {
                scYM2612[chipID].setRegister(-1, (int)(wait * (ctYM2612[chipID].UseWaitBoost ? 2.0 : 1.0)));
            }
        }

        #endregion



        #region segapcm

        private void SEGAPCMWriteRegisterControl(Chip Chip, EnmDataType type, int address, int data, object exData)
        {
            if (type == EnmDataType.Normal)
            {
                try
                {
                    if (Chip.Model == EnmVRModel.VirtualModel)
                    {
                        if (!ctSEGAPCM[Chip.Number].UseScci && ctSEGAPCM[Chip.Number].UseEmu)
                            mds.WriteSEGAPCM(Chip.Index, (byte)Chip.Number, (int)address, (byte)data);
                    }
                    if (Chip.Model == EnmVRModel.RealModel)
                    {
                        if (scSEGAPCM[Chip.Number] != null)
                            scSEGAPCM[Chip.Number].setRegister(address, data);
                    }
                }
                catch
                {

                }
            }
            else if (type == EnmDataType.Block)
            {
                Audio.sm.SetInterrupt();

                try
                {
                    if (exData == null) return;

                    if (data == -1)
                    {
                        PackData[] pdata = (PackData[])exData;
                        if (Chip.Model == EnmVRModel.VirtualModel)
                        {
                            foreach (PackData dat in pdata)
                                mds.WriteSEGAPCM(dat.Chip.Index, (byte)dat.Chip.Number, (int)dat.Address, (byte)dat.Data);
                        }
                        if (Chip.Model == EnmVRModel.RealModel)
                        {
                            if (scSEGAPCM[Chip.Number] != null)
                            {
                                foreach (PackData dat in pdata)
                                    scSEGAPCM[Chip.Number].setRegister(dat.Address, dat.Data);
                            }
                        }
                    }
                    else
                    {
                        if (Chip.Model == EnmVRModel.VirtualModel)
                            mds.WriteSEGAPCMPCMData(Chip.Index, (byte)Chip.Number, (uint)((object[])exData)[0], (uint)((object[])exData)[1], (uint)((object[])exData)[2], (byte[])((object[])exData)[3], (uint)((object[])exData)[4]);
                        // ROMSize, DataStart, DataLength, romdata, SrcStartAdr);
                        else
                        {
                            if (scSEGAPCM != null && scSEGAPCM[Chip.Number] != null)
                            {
                                // スタートアドレス設定
                                scSEGAPCM[Chip.Number].setRegister(0x10000, (byte)((uint)((object[])exData)[1]));
                                scSEGAPCM[Chip.Number].setRegister(0x10001, (byte)((uint)((object[])exData)[1] >> 8));
                                scSEGAPCM[Chip.Number].setRegister(0x10002, (byte)((uint)((object[])exData)[1] >> 16));
                                // データ転送
                                for (int cnt = 0; cnt < (uint)((object[])exData)[2]; cnt++)
                                {
                                    scSEGAPCM[Chip.Number].setRegister(0x10004, ((byte[])((object[])exData)[3])[(uint)((object[])exData)[4] + cnt]);
                                }
                                scSEGAPCM[Chip.Number].setRegister(0x10006, (int)(uint)((object[])exData)[0]);

                                realChip.SendData();
                            }
                        }
                    }
                }
                finally
                {
                    Audio.sm.ResetInterrupt();
                }
            }
        }

        public void SEGAPCMSetRegisterProcessing(ref long Counter, ref Chip Chip, ref EnmDataType Type, ref int Address, ref int dData, ref object ExData)
        {
            if (ctSEGAPCM == null) return;
            if (Address == -1 && dData == -1) return;

            if (Chip.Number == 0) chipLED.PriSPCM = 2;
            else chipLED.SecSPCM = 2;

            pcmRegisterSEGAPCM[Chip.Number][Address & 0x1ff] = (byte)dData;

            if (
                (Chip.Model == EnmVRModel.VirtualModel && (ctSEGAPCM[Chip.Number] == null || !ctSEGAPCM[Chip.Number].UseScci))
                || (Chip.Model == EnmVRModel.RealModel && (scSEGAPCM != null && scSEGAPCM[Chip.Number] != null))
                )
            {

                if ((Address & 0x87) == 0x86)
                {
                    byte ch = (byte)((Address >> 3) & 0xf);
                    if ((dData & 0x01) == 0)
                    {
                        pcmKeyOnSEGAPCM[Chip.Number][ch] = true;
                    }
                    if (Chip.ChMasks[ch]) dData &= 0xfe;
                }

                if (Address < 0x86 && ((Address & 0x03) == 2 || (Address & 0x03) == 3))
                {
                    int d = SEGAPCMNowFadeoutVol[Chip.Number];// >> 3;
                    dData = Math.Max(dData - d, 0);
                }
            }

            if (((Address - 0x86) & 7) == 0)
            {
                int ch = (Address - 0x86) >> 3;
                if (ch >= 0 && ch < 16)
                {
                    if (Chip.ChMasks[ch])
                    {
                        dData |= 1;//keyoff flg
                    }
                }
            }

            //if (Chip.Model == EnmModel.VirtualModel)
            //{
            //    if (!ctSEGAPCM[Chip.Number].UseScci)
            //        mds.WriteSEGAPCM((byte)Chip.Number, Address, (byte)dData);
            //    //System.Console.WriteLine("ChipID={0} Offset={1:X} Data={2:X} ", ChipID, Offset, Data);
            //}
            //else
            //{
            //    if (scSEGAPCM != null && scSEGAPCM[Chip.Number] != null) scSEGAPCM[Chip.Number].setRegister(Address, (byte)dData);
            //}
        }

        public void SEGAPCMSetRegister(outDatum od, long Counter, int ChipID, int dAddr, int dData)
        {
            enq(od, Counter, SEGAPCM[ChipID], EnmDataType.Normal, dAddr, dData, null);
        }

        public void SEGAPCMSetRegister(long Counter, int ChipID, PackData[] data)
        {
            enq(null, Counter, SEGAPCM[ChipID], EnmDataType.Block, -1, -1, data);
        }

        public void SEGAPCMWritePCMData(outDatum od, long Counter, byte chipID, uint ROMSize, uint DataStart, uint DataLength, byte[] romdata, uint SrcStartAdr)
        {
            enq(od, Counter, SEGAPCM[chipID], EnmDataType.Block, -1, -2, new object[] { ROMSize, DataStart, DataLength, romdata, SrcStartAdr });

            //EnmModel model = EnmModel.VirtualModel;
            //if (chipID == 0) chipLED.PriSPCM = 2;
            //else chipLED.SecSPCM = 2;

            //if (model == EnmModel.VirtualModel)
            //{
            //    mds.WriteSEGAPCMPCMData(chipID, ROMSize, DataStart, DataLength, romdata, SrcStartAdr);
            //}
            //else
            //{
            //    if (scSEGAPCM != null && scSEGAPCM[chipID] != null)
            //    {
            //        // スタートアドレス設定
            //        scSEGAPCM[chipID].setRegister(0x10000, (byte)(DataStart));
            //        scSEGAPCM[chipID].setRegister(0x10001, (byte)(DataStart >> 8));
            //        scSEGAPCM[chipID].setRegister(0x10002, (byte)(DataStart >> 16));
            //        // データ転送
            //        for (int cnt = 0; cnt < DataLength; cnt++)
            //        {
            //            scSEGAPCM[chipID].setRegister(0x10004, romdata[SrcStartAdr + cnt]);
            //        }
            //        scSEGAPCM[chipID].setRegister(0x10006, (int)ROMSize);

            //        realChip.SendData();
            //    }
            //}
        }

        public void SEGAPCMSoftReset(long Counter, int ChipID)
        {
            List<PackData> data = SEGAPCMMakeSoftReset(ChipID);
            SEGAPCMSetRegister(Counter, ChipID, data.ToArray());
        }

        public List<PackData> SEGAPCMMakeSoftReset(int chipID)
        {
            List<PackData> data = new List<PackData>();
            int i;

            for (i = 0; i < 16; i++)
            {
                data.Add(new PackData(null, SEGAPCM[chipID], EnmDataType.Normal, (i * 8) + 0x86, 0x01, null));// KeyOff
                data.Add(new PackData(null, SEGAPCM[chipID], EnmDataType.Normal, (i * 8) + 0x02, 0x00, null));// L vol
                data.Add(new PackData(null, SEGAPCM[chipID], EnmDataType.Normal, (i * 8) + 0x03, 0x00, null));// R vol
            }

            return data;
        }

        public void SEGAPCMWriteClock(byte chipID, int clock)
        {
            if (scSEGAPCM != null && scSEGAPCM.Length > chipID && scSEGAPCM[chipID] != null)
            {
                scSEGAPCM[chipID].setRegister(0x10005, (int)clock);
                scSEGAPCM[chipID].dClock = scSEGAPCM[chipID].SetMasterClock((uint)clock);
                scSEGAPCM[chipID].mul = (double)scSEGAPCM[chipID].dClock / (double)clock;
            }
        }

        public void SEGAPCMSetFadeoutVolume(long Counter, int v)
        {
            for (int i = 0; i < SEGAPCM.Count; i++)
            {
                if (!SEGAPCM[i].Use) continue;
                if (SEGAPCM[i].Model == EnmVRModel.VirtualModel) continue;
                if (SEGAPCMNowFadeoutVol[i] >> 4 == v >> 4) continue;

                SEGAPCMNowFadeoutVol[i] = v;
                for (int c = 0; c < 16; c++)
                {
                    SEGAPCMSetRegister(null, Counter, i, (c * 8) + 2, pcmRegisterSEGAPCM[i][(c * 8) + 2]);
                    SEGAPCMSetRegister(null, Counter, i, (c * 8) + 3, pcmRegisterSEGAPCM[i][(c * 8) + 3]);
                }
            }
        }

        #endregion



        #region SN76489(DCSG)

        private void SN76489WriteRegisterControl(Chip Chip, EnmDataType type, int address, int data, object exData)
        {
            if (type == EnmDataType.Normal)
            {
                if (Chip.Model == EnmVRModel.VirtualModel)
                {
                    if (ctSN76489[Chip.Number] == null || !ctSN76489[Chip.Number].UseScci)
                    {
                        if (mds == null) return;

                        if (ctSN76489[Chip.Number].UseEmu)
                        {
                            if (address != 0x100)
                            {
                                mds.WriteSN76489(Chip.Index, (byte)Chip.Number, (byte)data);
                            }
                            else
                            {
                                mds.WriteSN76489GGPanning(Chip.Index, (byte)Chip.Number, (byte)data);
                            }
                        }
                        else if (ctSN76489[Chip.Number].UseEmu2)
                        {
                            if (address != 0x100)
                            {
                                mds.WriteSN76496(Chip.Index, (byte)Chip.Number, (byte)data);
                            }
                            else
                            {
                                mds.WriteSN76496GGPanning(Chip.Index, (byte)Chip.Number, (byte)data);
                            }
                        }
                    }
                }
                if (Chip.Model == EnmVRModel.RealModel)
                {
                    if (scSN76489[Chip.Number] != null)
                    {
                        scSN76489[Chip.Number].setRegister(address, data);
                    }
                }
            }
            else if (type == EnmDataType.Block)
            {
                Audio.sm.SetInterrupt();

                try
                {
                    if (exData == null) return;

                    PackData[] pdata = (PackData[])exData;
                    if (Chip.Model == EnmVRModel.VirtualModel)
                    {
                        if (mds == null) return;
                        foreach (PackData dat in pdata)
                        {
                            if (ctSN76489[dat.Chip.Number].UseEmu)
                            {
                                if (dat.Address != 0x100)
                                {
                                    mds.WriteSN76489(dat.Chip.Index, (byte)dat.Chip.Number, (byte)dat.Data);
                                }
                                else
                                {
                                    mds.WriteSN76489GGPanning(dat.Chip.Index, (byte)dat.Chip.Number, (byte)dat.Data);
                                }
                            }
                            else if (ctSN76489[dat.Chip.Number].UseEmu2)
                            {
                                if (dat.Address != 0x100)
                                {
                                    mds.WriteSN76496(dat.Chip.Index, (byte)dat.Chip.Number, (byte)dat.Data);
                                }
                                else
                                {
                                    mds.WriteSN76496GGPanning(dat.Chip.Index, (byte)dat.Chip.Number, (byte)dat.Data);
                                }
                            }
                        }
                    }
                    if (Chip.Model == EnmVRModel.RealModel)
                    {
                        foreach (PackData dat in pdata)
                        {
                            if (scSN76489[Chip.Number] != null)
                                scSN76489[dat.Chip.Number].setRegister(dat.Address, dat.Data);
                        }
                    }
                }
                finally
                {
                    Audio.sm.ResetInterrupt();
                }
            }
        }

        public void SN76489SetRegisterGGpanning(outDatum od, long Counter, int ChipID, int dData)
        {
            enq(od, Counter, SN76489[ChipID], EnmDataType.Normal, 0x100, dData, null);

            //EnmModel model = EnmModel.VirtualModel;
            //if (ctSN76489 == null) return;

            //if (chipID == 0) chipLED.PriDCSG = 2;
            //else chipLED.SecDCSG = 2;

            //if (model == EnmModel.RealModel)
            //{
            //    if (ctSN76489[chipID].UseScci)
            //    {
            //        if (scSN76489[chipID] == null) return;
            //    }
            //}
            //else
            //{
            //    if (!ctSN76489[chipID].UseScci && ctSN76489[chipID].UseEmu)
            //    {
            //        mds.WriteSN76489GGPanning((byte)chipID, (byte)dData);
            //        sn76489RegisterGGPan[chipID] = dData;
            //    }
            //}
        }

        public void SN76489SetRegisterProcessing(ref long Counter, ref Chip Chip, ref EnmDataType Type, ref int Address, ref int dData, ref object ExData)
        {
            if (ctSN76489 == null) return;
            if (Address == -1 && dData == -1) return;

            if (Chip.Number == 0) chipLED.PriDCSG = 2;
            else chipLED.SecDCSG = 2;

            if (Address == 0x100)
            {
                SN76489RegisterGGPan[Chip.Number] = dData;
                return;
            }

            SN76489Write(Chip.Number, dData);

            if ((dData & 0x90) == 0x90)
            {
                SN76489Vol[Chip.Number][(dData & 0x60) >> 5][0] = (15 - (dData & 0xf)) * ((SN76489RegisterGGPan[Chip.Number] >> (((dData & 0x60) >> 5) + 4)) & 0x1);
                SN76489Vol[Chip.Number][(dData & 0x60) >> 5][1] = (15 - (dData & 0xf)) * ((SN76489RegisterGGPan[Chip.Number] >> ((dData & 0x60) >> 5)) & 0x1);

                int v = dData & 0xf;
                v = v + SN76489NowFadeoutVol[Chip.Number];
                //v += SN76489MaskCh[Chip.Number][(dData & 0x60) >> 5] ? 15 : 0;
                v = Chip.ChMasks[(dData & 0x60) >> 5] ? 15 : v;
                v = Math.Min(v, 15);
                dData = (dData & 0xf0) | v;
            }

            //if (Chip.Model == EnmModel.RealModel)
            //{
            //if (ctSN76489[Chip.Number].UseScci)
            //{
            //    if (scSN76489[Chip.Number] == null) return;
            //    scSN76489[Chip.Number].setRegister(0, dData);
            //}
            //}
            //else
            //{
            //if (!ctSN76489[Chip.Number].UseScci && ctSN76489[Chip.Number].UseEmu)
            //{
            //    mds.WriteSN76489((byte)Chip.Number, (byte)dData);
            //}
            //}
        }

        public void SN76489SetRegister(outDatum od, long Counter, int ChipID, int dData)
        {
            enq(od, Counter, SN76489[ChipID], EnmDataType.Normal, 0, dData, null);
        }

        public void SN76489SetRegister(long Counter, int ChipID, PackData[] data)
        {
            enq(null, Counter, SN76489[ChipID], EnmDataType.Block, -1, -1, data);
        }

        public void SN76489SoftReset(long Counter, int chipID)
        {
            List<PackData> data = SN76489MakeSoftReset(chipID);
            SN76489SetRegister(Counter, chipID, data.ToArray());
        }

        public void SN76489SetMask(int chipID, int ch, bool mask)
        {
            SN76489MaskCh[chipID][ch] = mask;
        }

        public void SN76489WriteClock(byte chipID, int clock)
        {
            if (scSN76489 != null && scSN76489.Length > chipID && scSN76489[chipID] != null)
            {
                scSN76489[chipID].dClock = scSN76489[chipID].SetMasterClock((uint)clock);
            }
        }

        public void SN76489SetFadeoutVolume(long Counter, int v)
        {
            for (int i = 0; i < SN76489.Count; i++)
            {
                if (!SN76489[i].Use) continue;
                if (SN76489[i].Model == EnmVRModel.VirtualModel) continue;
                if (SN76489NowFadeoutVol[i] == v) continue;

                SN76489NowFadeoutVol[i] = v;// (v & 0x78) >> 3;
                for (int c = 0; c < 4; c++)
                {
                    SN76489SetRegister(null, Counter, i, 0x90 + (c << 5) + SN76489Register[i][1 + (c << 1)]);
                }
            }
        }

        public List<PackData> SN76489MakeSoftReset(int chipID)
        {
            List<PackData> data = new List<PackData>();

            //vol off
            data.Add(new PackData(null, SN76489[chipID], EnmDataType.Normal, 0, 0x9f, null));
            data.Add(new PackData(null, SN76489[chipID], EnmDataType.Normal, 0, 0xbf, null));
            data.Add(new PackData(null, SN76489[chipID], EnmDataType.Normal, 0, 0xdf, null));
            data.Add(new PackData(null, SN76489[chipID], EnmDataType.Normal, 0, 0xff, null));

            return data;
        }

        private void SN76489Write(int chipID, int data)
        {
            if ((data & 0x80) > 0)
            {
                /* Latch/data byte  %1 cc t dddd */
                LatchedRegister[chipID] = (data >> 4) & 0x07;
                SN76489Register[chipID][LatchedRegister[chipID]] =
                    (SN76489Register[chipID][LatchedRegister[chipID]] & 0x3f0) /* zero low 4 bits */
                    | (data & 0xf);                            /* and replace with data */
            }
            else
            {
                /* Data byte        %0 - dddddd */
                if ((LatchedRegister[chipID] % 2) == 0 && (LatchedRegister[chipID] < 5))
                    /* Tone register */
                    SN76489Register[chipID][LatchedRegister[chipID]] =
                        (SN76489Register[chipID][LatchedRegister[chipID]] & 0x00f) /* zero high 6 bits */
                        | ((data & 0x3f) << 4);                 /* and replace with data */
                else
                    /* Other register */
                    SN76489Register[chipID][LatchedRegister[chipID]] = data & 0x0f; /* Replace with data */
            }
            switch (LatchedRegister[chipID])
            {
                case 0:
                case 2:
                case 4: /* Tone channels */
                    if (SN76489Register[chipID][LatchedRegister[chipID]] == 0)
                        SN76489Register[chipID][LatchedRegister[chipID]] = 1; /* Zero frequency changed to 1 to avoid div/0 */
                    break;
                case 6: /* Noise */
                    NoiseFreq[chipID] = 0x10 << (SN76489Register[chipID][6] & 0x3); /* set noise signal generator frequency */
                    break;
            }
        }

        public void SN76489SetSyncWait(byte chipID, int wait)
        {
            if (scSN76489 != null && ctSN76489[chipID].UseWait)
            {
                scSN76489[chipID].setRegister(-1, (int)(wait * (ctSN76489[chipID].UseWaitBoost ? 2.0 : 1.0)));
            }
        }

        #endregion



        #region MIDI

        private void MidiGMWriteRegisterControl(Chip Chip, EnmDataType type, int address, int data, object exData)
        {
            if (type == EnmDataType.Normal)
            {
                if (Chip.Model == EnmVRModel.VirtualModel)
                {
                }
                if (Chip.Model == EnmVRModel.RealModel)
                {
                    int num = address;
                    if (midiOuts == null) return;
                    if (num >= midiOuts.Count) return;
                    if (midiOuts[num] == null) return;

                    midiOuts[num].SendBuffer((byte[])exData);
                    if (num < midiParams.Length) midiParams[num].SendBuffer((byte[])exData);
                    return;
                }
            }
            else if (type == EnmDataType.Block)
            {
                if (midiOuts.Count < 1) return;

                Audio.sm.SetInterrupt();

                try
                {
                    if (exData == null) return;

                    if (data == -1)
                    {
                        if (exData is byte[])
                        {
                            byte[] pdata = (byte[])exData;
                            if (Chip.Model == EnmVRModel.VirtualModel)
                            {
                            }
                            if (Chip.Model == EnmVRModel.RealModel)
                            {
                                if (midiOuts.Count > address && midiOuts[address] != null) midiOuts[address].SendBuffer((byte[])exData);
                            }
                        }
                        else if (exData is PackData[])
                        {
                            PackData[] pdata = (PackData[])exData;
                            if (Chip.Model == EnmVRModel.VirtualModel)
                            {
                            }
                            if (Chip.Model == EnmVRModel.RealModel)
                            {
                                foreach (PackData dat in pdata)
                                {
                                    if (midiOuts.Count <= dat.Chip.Number || midiOuts[dat.Chip.Number] == null) continue;
                                    midiOuts[dat.Chip.Number].SendBuffer((byte[])dat.ExData);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (Chip.Model == EnmVRModel.VirtualModel)
                        {

                        }
                        else
                        {
                            //if (scC140 != null && scC140[Chip.Number] != null)
                            //{
                            // スタートアドレス設定
                            //scC140[Chip.Number].setRegister(0x10000, (byte)((uint)((object[])exData)[1]));
                            //scC140[Chip.Number].setRegister(0x10001, (byte)((uint)((object[])exData)[1] >> 8));
                            //scC140[Chip.Number].setRegister(0x10002, (byte)((uint)((object[])exData)[1] >> 16));
                            // データ転送
                            //for (int cnt = 0; cnt < (uint)((object[])exData)[2]; cnt++)
                            //{
                            //scC140[Chip.Number].setRegister(0x10004, ((byte[])((object[])exData)[3])[(uint)((object[])exData)[4] + cnt]);
                            //}
                            //scC140[chipID].setRegister(0x10006, (int)ROMSize);

                            //realChip.SendData();
                            //}
                        }
                    }
                }
                finally
                {
                    Audio.sm.ResetInterrupt();
                }
            }
        }

        public void MidiGMSetRegisterProcessing(ref long Counter, ref Chip Chip, ref EnmDataType Type, ref int Address, ref int dData, ref object ExData)
        {
            if (Chip.Number == 0) chipLED.PriMID = 2;
            else chipLED.SecMID = 2;

            if (Chip.Model == EnmVRModel.VirtualModel)
            {
            }

            if (Chip.Model == EnmVRModel.RealModel)
            {
            }

            if (ExData != null && ExData is byte[] && ((byte[])ExData).Length == 3)
            {
                if ((((byte[])ExData)[0] & 0xf0) == 0x90)
                {
                    int ch = ((byte[])ExData)[0] & 0xf;
                    if (Chip.ChMasks[ch])
                    {
                        ((byte[])ExData)[2] = 0;
                    }
                }
            }
        }

        public void MIDISetRegister(outDatum od, long Counter, int ChipID, int dPort, byte[] dData)
        {
            dummyChip.Move(MIDI[ChipID]);

            enq(od, Counter, dummyChip, EnmDataType.Normal, dPort, 0, dData);
        }

        public void MIDISetRegister(outDatum od, long Counter, Chip chip, PackData[] data)
        {
            enq(od, Counter, chip, EnmDataType.Block, -1, -1, data);
        }

        public void MIDISoftReset(long Counter, int chipID)
        {
            List<PackData> data = MIDIMakeSoftReset(chipID);
            MIDISetRegister(null, Counter, MIDI[chipID], data.ToArray());
        }

        public List<PackData> MIDIMakeSoftReset(int chipID)
        {
            List<PackData> data = new List<PackData>();

            //vol off
            data.Add(new PackData(null, MIDI[chipID], EnmDataType.Block, chipID, -1, new byte[] {
                0xb0, 0x78, 0x00,  0xb1, 0x78, 0x0,  0xb2, 0x78, 0x0,  0xb3, 0x78, 0x0,
                0xb4, 0x78, 0x00,  0xb5, 0x78, 0x0,  0xb6, 0x78, 0x0,  0xb7, 0x78, 0x0,
                0xb8, 0x78, 0x00,  0xb9, 0x78, 0x0,  0xba, 0x78, 0x0,  0xbb, 0x78, 0x0,
                0xbc, 0x78, 0x00,  0xbd, 0x78, 0x0,  0xbe, 0x78, 0x0,  0xbf, 0x78, 0x0
            }));

            return data;
        }

        #endregion



        #region Conductor

        public void ConductorSoftReset(long Counter, int chipID)
        {
            //List<PackData> data = ConductorMakeSoftReset(chipID);
        }

        public List<PackData> ConductorMakeSoftReset(int chipID)
        {
            return new List<PackData>();
        }

        #endregion


        public void setMaskYM3526(int chipID, int ch, bool mask)
        {
            maskFMChYM3526[chipID][ch] = mask;
        }

        public void setMaskYM3812(int chipID, int ch, bool mask)
        {
            maskFMChYM3812[chipID][ch] = mask;
        }

        public void setMaskYMF278B(int chipID, int ch, bool mask)
        {
            maskFMChYMF278B[chipID][YMF278BCh[ch]] = mask;
        }

        public void setMaskC352(int chipID, int ch, bool mask)
        {
            maskChC352[chipID][ch] = mask;
        }

        public void setMaskOKIM6258(int chipID, bool mask)
        {
            maskOKIM6258[chipID] = mask;

            writeOKIM6258((byte)chipID, 0, 1);
        }

        public void setNESMask(int chipID, int ch)
        {
            switch (ch)
            {
                case 0:
                case 1:
                    nsfAPUmask |= 1 << ch;
                    if (nes_apu != null) nes_apu.SetMask(nsfAPUmask);
                    break;
                case 2:
                case 3:
                case 4:
                    nsfDMCmask |= 1 << (ch - 2);
                    if (nes_dmc != null) nes_dmc.SetMask(nsfDMCmask);
                    break;
            }
            mds.setNESMask(chipID, ch);
        }

        public void resetNESMask(int chipID, int ch)
        {
            switch (ch)
            {
                case 0:
                case 1:
                    nsfAPUmask &= ~(1 << ch);
                    if (nes_apu != null) nes_apu.SetMask(nsfAPUmask);
                    break;
                case 2:
                case 3:
                case 4:
                    nsfDMCmask &= ~(1 << (ch - 2));
                    if (nes_dmc != null) nes_dmc.SetMask(nsfDMCmask);
                    break;
            }
            mds.resetNESMask(chipID, ch);
        }

        public void setFDSMask(int chipID)
        {
            nsfFDSmask |= 1;
            if (nes_fds != null) nes_fds.SetMask(nsfFDSmask);
            mds.setFDSMask(chipID);
        }

        public void resetFDSMask(int chipID)
        {
            nsfFDSmask &= ~1;
            if (nes_fds != null) nes_fds.SetMask(nsfFDSmask);
            mds.resetFDSMask(chipID);
        }

        public void setMMC5Mask(int chipID, int ch)
        {
            nsfMMC5mask |= 1 << ch;
            if (nes_mmc5 != null) nes_mmc5.SetMask(nsfMMC5mask);
        }

        public void resetMMC5Mask(int chipID, int ch)
        {
            nsfMMC5mask &= ~(1 << ch);
            if (nes_mmc5 != null) nes_mmc5.SetMask(nsfMMC5mask);
        }

        public void setVRC7Mask(int chipID, int ch)
        {
            nsfVRC7mask |= 1 << ch;
            if (nes_vrc7 != null) nes_vrc7.SetMask(nsfVRC7mask);
        }

        public void resetVRC7Mask(int chipID, int ch)
        {
            nsfVRC7mask &= ~(1 << ch);
            if (nes_vrc7 != null) nes_vrc7.SetMask(nsfVRC7mask);
        }



        public void setDMGRegister(int chipID, int dAddr, int dData)
        {
            EnmVRModel model = EnmVRModel.VirtualModel;
            if (chipID == 0) chipLED.PriDMG = 2;
            else chipLED.SecDMG = 2;

            if (model == EnmVRModel.VirtualModel)
            {
                //if (!ctNES[chipID].UseScci)
                //{
                mds.WriteDMG((byte)chipID, (byte)dAddr, (byte)dData);
                //}
            }
            else
            {
                //if (scNES[chipID] == null) return;

                //scNES[chipID].setRegister(dAddr, dData);
            }
        }

        public void setNESRegister(int chipID, int dAddr, int dData)
        {
            EnmVRModel model = EnmVRModel.VirtualModel;
            if (chipID == 0) chipLED.PriNES = 2;
            else chipLED.SecNES = 2;

            if (model == EnmVRModel.VirtualModel)
            {
                //if (!ctNES[chipID].UseScci)
                //{
                mds.WriteNES((byte)chipID, (byte)dAddr, (byte)dData);
                //}
            }
            else
            {
                //if (scNES[chipID] == null) return;

                //scNES[chipID].setRegister(dAddr, dData);
            }
        }

        public byte[] getNESRegister(int chipID)
        {
            EnmVRModel model = EnmVRModel.VirtualModel;
            if (chipID == 0) chipLED.PriNES = 2;
            else chipLED.SecNES = 2;

            if (model == EnmVRModel.VirtualModel)
            {
                //if (!ctNES[chipID].UseScci)
                //{
                return mds.ReadNESapu((byte)chipID);
                //}
            }
            else
            {
                return null;
                //if (scNES[chipID] == null) return;

                //scNES[chipID].setRegister(dAddr, dData);
            }
        }

        public MDSound.np.np_nes_fds.NES_FDS getFDSRegister(int chipID, EnmVRModel model)
        {
            if (chipID == 0) chipLED.PriFDS = 2;
            else chipLED.SecFDS = 2;

            if (model == EnmVRModel.VirtualModel)
            {
                //if (!ctNES[chipID].UseScci)
                //{
                return mds.ReadFDS((byte)chipID);
                //}
            }
            else
            {
                return null;
                //if (scFDS[chipID] == null) return;

                //scFDS[chipID].setRegister(dAddr, dData);
            }

        }

        public MDSound.np.chip.nes_mmc5 getMMC5Register(int chipID, EnmVRModel model)
        {
            if (chipID == 0) chipLED.PriMMC5 = 2;
            else chipLED.SecMMC5 = 2;

            if (model == EnmVRModel.VirtualModel)
            {
                return null;// mds.ReadMMC5((byte)chipID);
            }
            else
            {
                return null;
            }

        }

        public byte[] getVRC7Register(int chipID)
        {
            if (nes_vrc7 == null) return null;
            if (chipID != 0) return null;

            return nes_vrc7.GetVRC7regs();
        }

        public void setMultiPCMRegister(int chipID, int dAddr, int dData)
        {
            EnmVRModel model = EnmVRModel.VirtualModel;
            if (chipID == 0) chipLED.PriMPCM = 2;
            else chipLED.SecMPCM = 2;

            if (model == EnmVRModel.VirtualModel)
            {
                mds.WriteMultiPCM((byte)chipID, (byte)dAddr, (byte)dData);
            }
            else
            {
            }
        }

        public void setMultiPCMSetBank(int chipID, int dCh, int dAddr)
        {
            EnmVRModel model = EnmVRModel.VirtualModel;
            if (chipID == 0) chipLED.PriMPCM = 2;
            else chipLED.SecMPCM = 2;

            if (model == EnmVRModel.VirtualModel)
            {
                mds.WriteMultiPCMSetBank((byte)chipID, (byte)dCh, (int)dAddr);
            }
            else
            {
            }
        }

        //public void setQSoundRegister(int chipID, byte mm, byte ll, byte rr)
        //{
        //    EnmVRModel model = EnmVRModel.VirtualModel;
        //    if (chipID == 0) chipLED.PriQsnd = 2;

        //    if (model == EnmVRModel.VirtualModel)
        //    {
        //        mds.WriteQSoundCtr((byte)chipID, 0, mm);
        //        mds.WriteQSoundCtr((byte)chipID, 1, ll);
        //        mds.WriteQSoundCtr((byte)chipID, 2, rr);
        //    }
        //    else
        //    {
        //    }
        //}

        public void setGA20Register(int chipID, Int32 Adr, byte Dat)
        {
            EnmVRModel model = EnmVRModel.VirtualModel;
            if (chipID == 0) chipLED.PriGA20 = 2;
            else chipLED.SecGA20 = 2;

            if (model == EnmVRModel.VirtualModel)
            {
                mds.WriteGA20((byte)chipID, (byte)Adr, Dat);
            }
            else
            {
            }
        }

        //public void setYM3812Register(int chipID, int dAddr, int dData)
        //{
        //    EnmVRModel model = EnmVRModel.VirtualModel;
        //    //if (ctYM3812 == null) return;

        //    if (chipID == 0) chipLED.PriOPL2 = 2;
        //    else chipLED.SecOPL2 = 2;

        //    if (model == EnmVRModel.VirtualModel)
        //    {
        //        fmRegisterYM3812[chipID][dAddr] = dData;

        //        if (dAddr >= 0xb0 && dAddr <= 0xb8)
        //        {
        //            int ch = dAddr - 0xb0;
        //            int k = (dData >> 5) & 1;
        //            if (k == 0)
        //            {
        //                kiYM3812[chipID].Off[ch] = true;
        //            }
        //            else
        //            {
        //                if (kiYM3812[chipID].Off[ch]) kiYM3812[chipID].On[ch] = true;
        //                kiYM3812[chipID].Off[ch] = false;
        //            }
        //            if (maskFMChYM3812[chipID][ch]) dData &= 0x1f;
        //        }

        //        if (dAddr == 0xbd)
        //        {

        //            for (int c = 0; c < 5; c++)
        //            {
        //                if ((dData & (0x10 >> c)) == 0)
        //                {
        //                    kiYM3812[chipID].Off[c + 9] = true;
        //                }
        //                else
        //                {
        //                    if (kiYM3812[chipID].Off[c + 9]) kiYM3812[chipID].On[c + 9] = true;
        //                    kiYM3812[chipID].Off[c + 9] = false;
        //                }
        //            }

        //            if (maskFMChYM3812[chipID][9]) dData &= 0xef;
        //            if (maskFMChYM3812[chipID][10]) dData &= 0xf7;
        //            if (maskFMChYM3812[chipID][11]) dData &= 0xfb;
        //            if (maskFMChYM3812[chipID][12]) dData &= 0xfd;
        //            if (maskFMChYM3812[chipID][13]) dData &= 0xfe;

        //        }

        //    }

        //    if (model == EnmVRModel.VirtualModel)
        //    {
        //        //if (!ctYM3812[chipID].UseScci)
        //        {
        //            mds.WriteYM3812((byte)chipID, (byte)dAddr, (byte)dData);
        //        }
        //    }
        //    else
        //    {
        //    }

        //}





        //public void setYMF271Register(int chipID, int dPort, int dAddr, int dData)
        //{
        //    EnmVRModel model = EnmVRModel.VirtualModel;
        //    if (ctYMF271 == null) return;

        //    if (chipID == 0) chipLED.PriOPX = 2;
        //    else chipLED.SecOPX = 2;

        //    if (model == EnmVRModel.VirtualModel) fmRegisterYMF271[chipID][dPort][dAddr] = dData;

        //    if (model == EnmVRModel.VirtualModel)
        //    {
        //        if (!ctYMF271[chipID].UseScci)
        //        {
        //            mds.WriteYMF271((byte)chipID, (byte)dPort, (byte)dAddr, (byte)dData);
        //        }
        //    }
        //    else
        //    {
        //        if (scYMF271[chipID] == null) return;
        //        scYMF271[chipID].setRegister(dPort * 0x100 + dAddr, dData);
        //    }

        //}

        public void setYMF278BRegister(int chipID, int dPort, int dAddr, int dData)
        {
            EnmVRModel model = EnmVRModel.VirtualModel;

            if (ctYMF278B == null) return;

            if (chipID == 0) chipLED.PriOPL4 = 2;
            else chipLED.SecOPL4 = 2;

            if (model == EnmVRModel.VirtualModel)
            {
                fmRegisterYMF278B[chipID][dPort][dAddr] = dData;

                //if (dPort == 2)
                //{
                //Console.WriteLine("p=2:adr{0:x02} dat{1:x02}", dAddr, dData);
                //}

                if (dAddr >= 0xb0 && dAddr <= 0xb8)
                {
                    int ch = dAddr - 0xb0 + dPort * 9;
                    int k = (dData >> 5) & 1;
                    if (k == 0)
                    {
                        fmRegisterYMF278BFM[chipID] &= ~(1 << ch);
                    }
                    else
                    {
                        fmRegisterYMF278BFM[chipID] |= (1 << ch);
                    }
                    fmRegisterYMF278BFM[chipID] &= 0x3ffff;
                    if (maskFMChYMF278B[chipID][ch]) dData &= 0x1f;
                }

                if (dAddr == 0xbd && dPort == 0)
                {
                    if ((fmRegisterYMF278BRyhthmB[chipID] & 0x10) == 0 && (dData & 0x10) != 0) fmRegisterYMF278BRyhthm[chipID] |= 0x10;
                    if ((fmRegisterYMF278BRyhthmB[chipID] & 0x08) == 0 && (dData & 0x08) != 0) fmRegisterYMF278BRyhthm[chipID] |= 0x08;
                    if ((fmRegisterYMF278BRyhthmB[chipID] & 0x04) == 0 && (dData & 0x04) != 0) fmRegisterYMF278BRyhthm[chipID] |= 0x04;
                    if ((fmRegisterYMF278BRyhthmB[chipID] & 0x02) == 0 && (dData & 0x02) != 0) fmRegisterYMF278BRyhthm[chipID] |= 0x02;
                    if ((fmRegisterYMF278BRyhthmB[chipID] & 0x01) == 0 && (dData & 0x01) != 0) fmRegisterYMF278BRyhthm[chipID] |= 0x01;
                    fmRegisterYMF278BRyhthmB[chipID] = dData;

                    if (maskFMChYMF278B[chipID][18]) dData &= 0xef;
                    if (maskFMChYMF278B[chipID][19]) dData &= 0xf7;
                    if (maskFMChYMF278B[chipID][20]) dData &= 0xfb;
                    if (maskFMChYMF278B[chipID][21]) dData &= 0xfd;
                    if (maskFMChYMF278B[chipID][22]) dData &= 0xfe;
                }

                if (dPort == 2 && (dAddr >= 0x68 && dAddr <= 0x7f))
                {
                    int k = dData >> 7;
                    if (k == 0)
                    {
                        fmRegisterYMF278BPCM[chipID][dAddr - 0x68] = 2;
                    }
                    else
                    {
                        fmRegisterYMF278BPCM[chipID][dAddr - 0x68] = 1;
                    }
                    if (maskFMChYMF278B[chipID][dAddr - 0x68 + 23]) dData &= 0x7f;
                }

            }

            if (model == EnmVRModel.VirtualModel)
            {
                if (!ctYMF278B[chipID].UseScci)
                {
                    mds.WriteYMF278B((byte)chipID, (byte)dPort, (byte)dAddr, (byte)dData);
                }
            }
            else
            {
                if (scYMF278B[chipID] == null) return;
                scYMF278B[chipID].setRegister(dPort * 0x100 + dAddr, dData);
            }

        }

        //public void setYM3526Register(int chipID, int dAddr, int dData)
        //{
        //    EnmVRModel model = EnmVRModel.VirtualModel;
        //    if (ctYM3526 == null) return;

        //    if (chipID == 0) chipLED.PriOPL = 2;
        //    else chipLED.SecOPL = 2;

        //    if (model == EnmVRModel.VirtualModel)
        //    {
        //        fmRegisterYM3526[chipID][dAddr] = dData;
        //        if (dAddr >= 0xb0 && dAddr <= 0xb8)
        //        {
        //            int ch = dAddr - 0xb0;
        //            int k = (dData >> 5) & 1;
        //            if (k == 0)
        //            {
        //                kiYM3526[chipID].On[ch] = false;
        //                kiYM3526[chipID].Off[ch] = true;
        //            }
        //            else
        //            {
        //                kiYM3526[chipID].On[ch] = true;
        //            }
        //            if (maskFMChYM3526[chipID][ch]) dData &= 0x1f;
        //        }

        //        if (dAddr == 0xbd)
        //        {

        //            for (int c = 0; c < 5; c++)
        //            {
        //                if ((dData & (0x10 >> c)) == 0)
        //                {
        //                    kiYM3526[chipID].Off[c + 9] = true;
        //                }
        //                else
        //                {
        //                    if (kiYM3526[chipID].Off[c + 9]) kiYM3526[chipID].On[c + 9] = true;
        //                    kiYM3526[chipID].Off[c + 9] = false;
        //                }
        //            }

        //            if (maskFMChYM3526[chipID][9]) dData &= 0xef;
        //            if (maskFMChYM3526[chipID][10]) dData &= 0xf7;
        //            if (maskFMChYM3526[chipID][11]) dData &= 0xfb;
        //            if (maskFMChYM3526[chipID][12]) dData &= 0xfd;
        //            if (maskFMChYM3526[chipID][13]) dData &= 0xfe;

        //        }

        //    }

        //    if (model == EnmVRModel.VirtualModel)
        //    {
        //        //if (!ctYM3526[chipID].UseScci)
        //        {
        //            mds.WriteYM3526((byte)chipID, (byte)dAddr, (byte)dData);
        //        }
        //    }
        //    else
        //    {
        //    }

        //}

        public void setYMZ280BRegister(int chipID, int dAddr, int dData)
        {
            EnmVRModel model = EnmVRModel.VirtualModel;
            if (ctYMZ280B == null) return;

            if (chipID == 0) chipLED.PriYMZ = 2;
            else chipLED.SecYMZ = 2;

            if (model == EnmVRModel.VirtualModel) YMZ280BRegister[chipID][dAddr] = dData;

            if (model == EnmVRModel.VirtualModel)
            {
                if (!ctYMZ280B[chipID].UseScci)
                {
                    mds.WriteYMZ280B((byte)chipID, (byte)dAddr, (byte)dData);
                }
            }
            else
            {
                if (scYMZ280B[chipID] == null) return;
                scYMZ280B[chipID].setRegister(dAddr, dData);
            }

        }

        public void writeNESPCMData(byte chipid, uint stAdr, uint dataSize, byte[] vgmBuf, uint vgmAdr)
        {
            EnmVRModel model = EnmVRModel.VirtualModel;
            if (chipid == 0) chipLED.PriNES = 2;
            else chipLED.SecNES = 2;

            if (model == EnmVRModel.VirtualModel)
                mds.WriteNESRam(chipid, (int)stAdr, (int)dataSize, vgmBuf, (int)vgmAdr);
        }

        public void writePWM(byte chipid, byte adr, uint data)
        {
            EnmVRModel model = EnmVRModel.VirtualModel;
            if (chipid == 0) chipLED.PriPWM = 2;
            else chipLED.SecPWM = 2;

            if (model == EnmVRModel.VirtualModel)
                mds.WritePWM(chipid, adr, data);
        }

        public void writeK053260(byte chipid, uint adr, byte data)
        {
            EnmVRModel model = EnmVRModel.VirtualModel;
            if (chipid == 0) chipLED.PriK053260 = 2;
            else chipLED.SecK053260 = 2;

            if (model == EnmVRModel.VirtualModel)
                mds.WriteK053260(chipid, (byte)adr, data);
        }

        public void writeK054539(byte chipid, uint adr, byte data)
        {
            EnmVRModel model = EnmVRModel.VirtualModel;
            if (chipid == 0) chipLED.PriK054539 = 2;
            else chipLED.SecK054539 = 2;

            if (model == EnmVRModel.VirtualModel)
                mds.WriteK054539(chipid, (int)adr, data);
        }

        public void writeK053260PCMData(byte chipid, uint ROMSize, uint DataStart, uint DataLength, byte[] romdata, uint SrcStartAdr)
        {
            EnmVRModel model = EnmVRModel.VirtualModel;
            if (chipid == 0) chipLED.PriK053260 = 2;
            else chipLED.SecK053260 = 2;

            if (model == EnmVRModel.VirtualModel)
                mds.WriteK053260PCMData(chipid, ROMSize, DataStart, DataLength, romdata, SrcStartAdr);
        }

        public void writeK054539PCMData(byte chipid, uint ROMSize, uint DataStart, uint DataLength, byte[] romdata, uint SrcStartAdr)
        {
            EnmVRModel model = EnmVRModel.VirtualModel;
            if (chipid == 0) chipLED.PriK054539 = 2;
            else chipLED.SecK054539 = 2;

            if (model == EnmVRModel.VirtualModel)
                mds.WriteK054539PCMData(chipid, ROMSize, DataStart, DataLength, romdata, SrcStartAdr);
        }

        public void writeQSoundPCMData(byte chipid, uint ROMSize, uint DataStart, uint DataLength, byte[] romdata, uint SrcStartAdr)
        {
            EnmVRModel model = EnmVRModel.VirtualModel;
            if (chipid == 0) chipLED.PriQsnd = 2;

            if (model == EnmVRModel.VirtualModel)
                mds.WriteQSoundCtrPCMData(chipid, ROMSize, DataStart, DataLength, romdata, SrcStartAdr);
        }

        public void writeC352(byte chipid, uint adr, uint data)
        {
            EnmVRModel model = EnmVRModel.VirtualModel;
            if (chipid == 0) chipLED.PriC352 = 2;
            else chipLED.SecC352 = 2;

            pcmRegisterC352[chipid][adr] = (ushort)data;
            //int c = (int)adr / 8;
            if (adr < 0x100 && (adr % 8) == 3 && maskChC352[chipid][adr / 8])
            {
                data &= 0xbfff;
            }
            if (model == EnmVRModel.VirtualModel)
            {
                mds.WriteC352(chipid, adr, data);
                //Console.WriteLine("{0:x04} {1:x04}",adr,data);
            }
        }

        public ushort[] readC352(byte chipid)
        {
            return mds.ReadC352Flag(chipid);
        }

        public void writeC352PCMData(byte chipid, uint ROMSize, uint DataStart, uint DataLength, byte[] romdata, uint SrcStartAdr)
        {
            EnmVRModel model = EnmVRModel.VirtualModel;
            if (chipid == 0) chipLED.PriC352 = 2;
            else chipLED.SecC352 = 2;

            if (model == EnmVRModel.VirtualModel)
                mds.WriteC352PCMData(chipid, ROMSize, DataStart, DataLength, romdata, SrcStartAdr);
        }

        public void writeGA20PCMData(byte chipid, uint ROMSize, uint DataStart, uint DataLength, byte[] romdata, uint SrcStartAdr)
        {
            EnmVRModel model = EnmVRModel.VirtualModel;
            if (chipid == 0) chipLED.PriGA20 = 2;
            else chipLED.SecGA20 = 2;

            if (model == EnmVRModel.VirtualModel)
                mds.WriteGA20PCMData(chipid, ROMSize, DataStart, DataLength, romdata, SrcStartAdr);
        }

        public void writeOKIM6258(byte ChipID, byte Port, byte Data)
        {
            EnmVRModel model = EnmVRModel.VirtualModel;
            if (ChipID == 0) chipLED.PriOKI5 = 2;
            else chipLED.SecOKI5 = 2;

            if (Port == 0x00)
            {
                if ((Data & 0x2) != 0) okim6258Keyon[ChipID] = true;

                if (maskOKIM6258[ChipID])
                {
                    if ((Data & 0x2) != 0) return;
                }
            }
            if (Port == 0x1)
            {
                if (maskOKIM6258[ChipID]) return;
            }

            if (model == EnmVRModel.VirtualModel)
                mds.WriteOKIM6258(ChipID, Port, Data);
        }

        public void writeOKIM6295(byte ChipID, byte Port, byte Data)
        {
            EnmVRModel model = EnmVRModel.VirtualModel;
            if (ChipID == 0) chipLED.PriOKI9 = 2;
            else chipLED.SecOKI9 = 2;

            if (model == EnmVRModel.VirtualModel)
            {
                mds.WriteOKIM6295(ChipID, Port, Data);
                //System.Console.WriteLine("ChipID={0} Port={1:X} Data={2:X} ",ChipID,Port,Data);
            }
        }

        public void writeOKIM6295PCMData(byte chipid, uint ROMSize, uint DataStart, uint DataLength, byte[] romdata, uint SrcStartAdr)
        {
            EnmVRModel model = EnmVRModel.VirtualModel;
            if (chipid == 0) chipLED.PriOKI9 = 2;
            else chipLED.SecOKI9 = 2;

            if (model == EnmVRModel.VirtualModel)
                mds.WriteOKIM6295PCMData(chipid, ROMSize, DataStart, DataLength, romdata, SrcStartAdr);
        }

        public void writeMultiPCMPCMData(byte chipid, uint ROMSize, uint DataStart, uint DataLength, byte[] romdata, uint SrcStartAdr)
        {
            EnmVRModel model = EnmVRModel.VirtualModel;
            if (chipid == 0) chipLED.PriMPCM = 2;
            else chipLED.SecMPCM = 2;

            if (model == EnmVRModel.VirtualModel)
                mds.WriteMultiPCMPCMData(chipid, ROMSize, DataStart, DataLength, romdata, SrcStartAdr);
        }

        public void writeYMF271PCMData(byte chipid, uint ROMSize, uint DataStart, uint DataLength, byte[] romdata, uint SrcStartAdr)
        {
            EnmVRModel model = EnmVRModel.VirtualModel;
            if (chipid == 0) chipLED.PriOPX = 2;
            else chipLED.SecOPX = 2;

            if (model == EnmVRModel.VirtualModel)
                mds.WriteYMF271PCMData(chipid, ROMSize, DataStart, DataLength, romdata, SrcStartAdr);
        }

        public void writeYMF278BPCMData(byte chipid, uint ROMSize, uint DataStart, uint DataLength, byte[] romdata, uint SrcStartAdr)
        {
            EnmVRModel model = EnmVRModel.VirtualModel;
            if (chipid == 0) chipLED.PriOPL4 = 2;
            else chipLED.SecOPL4 = 2;

            if (model == EnmVRModel.VirtualModel)
                mds.WriteYMF278BPCMData(chipid, ROMSize, DataStart, DataLength, romdata, SrcStartAdr);
        }

        public void writeYMZ280BPCMData(byte chipid, uint ROMSize, uint DataStart, uint DataLength, byte[] romdata, uint SrcStartAdr)
        {
            EnmVRModel model = EnmVRModel.VirtualModel;
            if (chipid == 0) chipLED.PriYMZ = 2;
            else chipLED.SecYMZ = 2;

            if (model == EnmVRModel.VirtualModel)
                mds.WriteYMZ280BPCMData(chipid, ROMSize, DataStart, DataLength, romdata, SrcStartAdr);
        }


        //
        // 鍵盤のボリューム表示のため音量を取得する
        //

        /// <summary>
        /// ボリューム情報の更新
        /// </summary>
        public void updateVol()
        {
            volF--;
            if (volF > 0) return;

            volF = 1;

            for (int chipID = 0; chipID < 2; chipID++)
            {
                for (int i = 0; i < 9; i++)
                {
                    if (fmVolYM2612[chipID][i] > 0) { fmVolYM2612[chipID][i] -= 50; if (fmVolYM2612[chipID][i] < 0) fmVolYM2612[chipID][i] = 0; }
                }
                for (int i = 0; i < 4; i++)
                {
                    if (fmCh3SlotVolYM2612[chipID][i] > 0) { fmCh3SlotVolYM2612[chipID][i] -= 50; if (fmCh3SlotVolYM2612[chipID][i] < 0) fmCh3SlotVolYM2612[chipID][i] = 0; }
                }
                for (int i = 0; i < 8; i++)
                {
                    if (YM2151FmVol[chipID][i] > 0) { YM2151FmVol[chipID][i] -= 50; if (YM2151FmVol[chipID][i] < 0) YM2151FmVol[chipID][i] = 0; }
                }
                for (int i = 0; i < 9; i++)
                {
                    if (fmVolYM2608[chipID][i] > 0) { fmVolYM2608[chipID][i] -= 50; if (fmVolYM2608[chipID][i] < 0) fmVolYM2608[chipID][i] = 0; }
                }
                for (int i = 0; i < 4; i++)
                {
                    if (fmCh3SlotVolYM2608[chipID][i] > 0) { fmCh3SlotVolYM2608[chipID][i] -= 50; if (fmCh3SlotVolYM2608[chipID][i] < 0) fmCh3SlotVolYM2608[chipID][i] = 0; }
                }
                for (int i = 0; i < 6; i++)
                {
                    if (fmVolYM2608Rhythm[chipID][i][0] > 0) { fmVolYM2608Rhythm[chipID][i][0] -= 50; if (fmVolYM2608Rhythm[chipID][i][0] < 0) fmVolYM2608Rhythm[chipID][i][0] = 0; }
                    if (fmVolYM2608Rhythm[chipID][i][1] > 0) { fmVolYM2608Rhythm[chipID][i][1] -= 50; if (fmVolYM2608Rhythm[chipID][i][1] < 0) fmVolYM2608Rhythm[chipID][i][1] = 0; }
                }

                if (fmVolYM2608Adpcm[chipID][0] > 0) { fmVolYM2608Adpcm[chipID][0] -= 50; if (fmVolYM2608Adpcm[chipID][0] < 0) fmVolYM2608Adpcm[chipID][0] = 0; }
                if (fmVolYM2608Adpcm[chipID][1] > 0) { fmVolYM2608Adpcm[chipID][1] -= 50; if (fmVolYM2608Adpcm[chipID][1] < 0) fmVolYM2608Adpcm[chipID][1] = 0; }

                for (int i = 0; i < 9; i++)
                {
                    if (fmVolYM2610[chipID][i] > 0) { fmVolYM2610[chipID][i] -= 50; if (fmVolYM2610[chipID][i] < 0) fmVolYM2610[chipID][i] = 0; }
                }
                for (int i = 0; i < 4; i++)
                {
                    if (fmCh3SlotVolYM2610[chipID][i] > 0) { fmCh3SlotVolYM2610[chipID][i] -= 50; if (fmCh3SlotVolYM2610[chipID][i] < 0) fmCh3SlotVolYM2610[chipID][i] = 0; }
                }
                for (int i = 0; i < 6; i++)
                {
                    if (fmVolYM2610Rhythm[chipID][i][0] > 0) { fmVolYM2610Rhythm[chipID][i][0] -= 50; if (fmVolYM2610Rhythm[chipID][i][0] < 0) fmVolYM2610Rhythm[chipID][i][0] = 0; }
                    if (fmVolYM2610Rhythm[chipID][i][1] > 0) { fmVolYM2610Rhythm[chipID][i][1] -= 50; if (fmVolYM2610Rhythm[chipID][i][1] < 0) fmVolYM2610Rhythm[chipID][i][1] = 0; }
                }

                if (fmVolYM2610Adpcm[chipID][0] > 0) { fmVolYM2610Adpcm[chipID][0] -= 50; if (fmVolYM2610Adpcm[chipID][0] < 0) fmVolYM2610Adpcm[chipID][0] = 0; }
                if (fmVolYM2610Adpcm[chipID][1] > 0) { fmVolYM2610Adpcm[chipID][1] -= 50; if (fmVolYM2610Adpcm[chipID][1] < 0) fmVolYM2610Adpcm[chipID][1] = 0; }

                for (int i = 0; i < 6; i++)
                {
                    if (fmVolYM2203[chipID][i] > 0) { fmVolYM2203[chipID][i] -= 50; if (fmVolYM2203[chipID][i] < 0) fmVolYM2203[chipID][i] = 0; }
                }
                for (int i = 0; i < 4; i++)
                {
                    if (fmCh3SlotVolYM2203[chipID][i] > 0) { fmCh3SlotVolYM2203[chipID][i] -= 50; if (fmCh3SlotVolYM2203[chipID][i] < 0) fmCh3SlotVolYM2203[chipID][i] = 0; }
                }
            }

        }

        public int[] GetYM2151Volume(int chipID)
        {
            return YM2151FmVol[chipID];
        }

        public int[] GetYM2203Volume(int chipID)
        {
            return fmVolYM2203[chipID];
        }

        public int[] GetYM2608Volume(int chipID)
        {
            return fmVolYM2608[chipID];
        }

        public int[] GetYM2610Volume(int chipID)
        {
            return fmVolYM2610[chipID];
        }

        public int[] GetYM2612Volume(int chipID)
        {
            return fmVolYM2612[chipID];
        }

        public int[] GetYM2203Ch3SlotVolume(int chipID)
        {
            //if (ctYM2612.UseScci)
            //{
            return fmCh3SlotVolYM2203[chipID];
            //}
            //return mds.ReadFMCh3SlotVolume();
        }

        public int[] GetYM2608Ch3SlotVolume(int chipID)
        {
            //if (ctYM2612.UseScci)
            //{
            return fmCh3SlotVolYM2608[chipID];
            //}
            //return mds.ReadFMCh3SlotVolume();
        }

        public int[] GetYM2610Ch3SlotVolume(int chipID)
        {
            //if (ctYM2612.UseScci)
            //{
            return fmCh3SlotVolYM2610[chipID];
            //}
            //return mds.ReadFMCh3SlotVolume();
        }

        public int[] GetYM2612Ch3SlotVolume(int chipID)
        {
            return fmCh3SlotVolYM2612[chipID];
        }

        public int[][] GetYM2608RhythmVolume(int chipID)
        {
            return fmVolYM2608Rhythm[chipID];
        }

        public int[][] GetYM2610RhythmVolume(int chipID)
        {
            return fmVolYM2610Rhythm[chipID];
        }

        public int[] GetYM2608AdpcmVolume(int chipID)
        {
            return fmVolYM2608Adpcm[chipID];
        }

        public int[] GetYM2610AdpcmVolume(int chipID)
        {
            return fmVolYM2610Adpcm[chipID];
        }

        public int[][] GetPSGVolume(int chipID)
        {

            return SN76489Vol[chipID];

        }


        public ChipKeyInfo getYM2413KeyInfo(int chipID)
        {
            for (int ch = 0; ch < kiYM2413[chipID].Off.Length; ch++)
            {
                kiYM2413ret[chipID].Off[ch] = kiYM2413[chipID].Off[ch];
                kiYM2413ret[chipID].On[ch] = kiYM2413[chipID].On[ch];
                kiYM2413[chipID].On[ch] = false;
            }
            return kiYM2413ret[chipID];
        }

        public ChipKeyInfo getYM3526KeyInfo(int chipID)
        {
            for (int ch = 0; ch < kiYM3526[chipID].Off.Length; ch++)
            {
                kiYM3526ret[chipID].Off[ch] = kiYM3526[chipID].Off[ch];
                kiYM3526ret[chipID].On[ch] = kiYM3526[chipID].On[ch];
                kiYM3526[chipID].On[ch] = false;
            }
            return kiYM3526ret[chipID];
        }

        public ChipKeyInfo getYM3812KeyInfo(int chipID)
        {
            for (int ch = 0; ch < kiYM3812[chipID].Off.Length; ch++)
            {
                kiYM3812ret[chipID].Off[ch] = kiYM3812[chipID].Off[ch];
                kiYM3812ret[chipID].On[ch] = kiYM3812[chipID].On[ch];
                kiYM3812[chipID].On[ch] = false;
            }
            return kiYM3812ret[chipID];
        }

        public ChipKeyInfo getVRC7KeyInfo(int chipID)
        {
            if (nes_vrc7 == null) return null;
            if (chipID != 0) return null;

            MDSound.np.chip.nes_vrc7.ChipKeyInfo ki = nes_vrc7.getVRC7KeyInfo(chipID);

            for (int ch = 0; ch < 6; ch++)
            {
                kiVRC7ret[chipID].On[ch] = ki.On[ch];
                kiVRC7ret[chipID].Off[ch] = ki.Off[ch];
            }
            return kiVRC7ret[chipID];
        }

        //public int getYM2413RyhthmKeyON(int chipID)
        //{
        //    int r = fmRegisterYM2413Ryhthm[chipID];
        //    fmRegisterYM2413Ryhthm[chipID] = 0;
        //    return r;
        //}

        public int getYMF262RyhthmKeyON(int chipID)
        {
            int r = fmRegisterYMF262Ryhthm[chipID];
            fmRegisterYMF262Ryhthm[chipID] = 0;
            return r;
        }

        public int getYMF278BRyhthmKeyON(int chipID)
        {
            return fmRegisterYMF278BRyhthm[chipID];
        }

        public void resetYMF278BRyhthmKeyON(int chipID)
        {
            fmRegisterYMF278BRyhthm[chipID] = 0;
        }

        public int[] getYMF278BPCMKeyON(int chipID)
        {
            return fmRegisterYMF278BPCM[chipID];
        }

        public void resetYMF278BPCMKeyON(int chipID)
        {
            for (int i = 0; i < 24; i++)
                fmRegisterYMF278BPCM[chipID][i] = 0;
        }

        public int getYMF262FMKeyON(int chipID)
        {
            return fmRegisterYMF262FM[chipID];
        }

        public int getYMF278BFMKeyON(int chipID)
        {
            return fmRegisterYMF278BFM[chipID];
        }

        public void resetYMF278BFMKeyON(int chipID)
        {
            fmRegisterYMF278BFM[chipID] = 0;
        }


        public void setMIDIout(midiOutInfo[] midiOutInfos, List<NAudio.Midi.MidiOut> midiOuts, List<int> midiOutsType)
        {
            this.midiOutInfos = null;
            if (midiOutInfos != null && midiOutInfos.Length > 0)
            {
                this.midiOutInfos = new midiOutInfo[midiOutInfos.Length];
                for (int i = 0; i < midiOutInfos.Length; i++)
                {
                    this.midiOutInfos[i] = new midiOutInfo();
                    this.midiOutInfos[i].beforeSendType = midiOutInfos[i].beforeSendType;
                    this.midiOutInfos[i].fileName = midiOutInfos[i].fileName;
                    this.midiOutInfos[i].id = midiOutInfos[i].id;
                    this.midiOutInfos[i].isVST = midiOutInfos[i].isVST;
                    this.midiOutInfos[i].manufacturer = midiOutInfos[i].manufacturer;
                    this.midiOutInfos[i].name = midiOutInfos[i].name;
                    this.midiOutInfos[i].type = midiOutInfos[i].type;
                    this.midiOutInfos[i].vendor = midiOutInfos[i].vendor;
                }
            }
            this.midiOuts = midiOuts;
            this.midiOutsType = midiOutsType;

            if (midiParams == null && midiParams.Length < 1) return;
            if (midiOutsType == null) return;
            if (midiOuts == null) return;

            if (midiOutsType.Count > 0) midiParams[0].MIDIModule = Math.Min(midiOutsType[0], 2);
            if (midiOutsType.Count > 1) midiParams[1].MIDIModule = Math.Min(midiOutsType[1], 2);

        }

        public void PPSDRVLoad(outDatum od, long Counter, int ChipID, byte[] addtionalData)
        {
            EnmVRModel model = EnmVRModel.VirtualModel;
            if (ChipID == 0) chipLED.PriPPSDRV = 2;
            else chipLED.SecPPSDRV = 2;

            if (model == EnmVRModel.VirtualModel)
                enq(od, Counter, PPSDRV[ChipID], EnmDataType.Block, -1, -2, new object[] { addtionalData });
        }

        public void PPSDRVWrite(outDatum od, long Counter, int ChipID, int dPort, int dAddr, int dData)
        {
            dummyChip.Move(PPSDRV[ChipID]);

            if (dPort == -1 && dAddr == -1 && dData == -1)
            {
                //ダミーデータ(演奏情報だけ流したい時向け)
                enq(od, Counter, dummyChip, EnmDataType.Normal, -1, -1, null);
            }
            else
                enq(od, Counter, dummyChip, EnmDataType.Normal, dPort, dAddr, dData);

        }

        public void PPZ8LoadPcm(outDatum od, long Counter, int ChipID, byte address, byte data, byte[] addtionalData)
        {
            EnmVRModel model = EnmVRModel.VirtualModel;
            if (ChipID == 0) chipLED.PriPPZ8 = 2;
            else chipLED.SecPPZ8 = 2;

            if (model == EnmVRModel.VirtualModel)
                enq(od, Counter, PPZ8[ChipID], EnmDataType.Block, -1, -2, new object[] { address, data, addtionalData });
        }

        public void PPZ8Write(outDatum od, long Counter, int ChipID, int dPort, int dAddr, int dData)
        {
            dummyChip.Move(PPZ8[ChipID]);

            if (dPort == -1 && dAddr == -1 && dData == -1)
            {
                //ダミーデータ(演奏情報だけ流したい時向け)
                enq(od, Counter, dummyChip, EnmDataType.Normal, -1, -1, null);
            }
            else
                enq(od, Counter, dummyChip, EnmDataType.Normal, dPort, dAddr, dData);

        }

        public void P86LoadPcm(outDatum od, long Counter, int ChipID, byte bank, byte mode, byte[] pcmData)
        {
            EnmVRModel model = EnmVRModel.VirtualModel;
            if (ChipID == 0) chipLED.PriP86 = 2;
            else chipLED.SecP86 = 2;

            if (model == EnmVRModel.VirtualModel)
                enq(od, Counter, P86[ChipID], EnmDataType.Block, -1, -2, new object[] { bank, mode, pcmData });
        }

        public void P86Write(outDatum od, long Counter, int ChipID, int dPort, int dAddr, int dData)
        {
            dummyChip.Move(P86[ChipID]);

            if (dPort == -1 && dAddr == -1 && dData == -1)
            {
                //ダミーデータ(演奏情報だけ流したい時向け)
                enq(od, Counter, dummyChip, EnmDataType.Normal, -1, -1, null);
            }
            else
                enq(od, Counter, dummyChip, EnmDataType.Normal, dPort, dAddr, dData);

        }

    }

    public class ChipKeyInfo
    {
        public bool[] On = null;
        public bool[] Off = null;

        public ChipKeyInfo(int n)
        {
            On = new bool[n];
            Off = new bool[n];
            for (int i = 0; i < n; i++) Off[i] = true;
        }
    }

}
