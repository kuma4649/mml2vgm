using Core;
using MDSound;
using mml2vgmIDE.MMLParameter;
//using Jacobi.Vst.Interop.Host;
//using Jacobi.Vst.Core;
//using mml2vgmIDE.form;
using SoundManager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace mml2vgmIDE
{
    public class Audio
    {

        public static int clockAY8910 = 1789750;
        public static int clockK051649 = 1500000;
        public static int clockC140 = 21390;
        public static int clockC352 = 24192000;
        public static int clockFDS = 0;
        public static int clockHuC6280 = 0;
        public static int clockRF5C164 = 0;
        public static int clockMMC5 = 0;
        public static int clockNESDMC = 0;
        public static int clockOKIM6258 = 0;
        public static int clockOKIM6295 = 0;
        public static int clockSegaPCM = 0;
        public static int clockSN76489 = 0;
        public static int clockYM2151 = 0;
        public static int clockYM2203 = 0;
        public static int clockYM2413 = 0;
        public static int clockYM2608 = 0;
        public static int clockYM2610 = 0;
        public static int clockYM2612 = 0;
        public static int clockYMF278B = 0;
        public static MDSound.MDSound mdsMIDI = null;
        public static Manager mmlParams = new Manager();
        public static mucomManager mucomManager = null;
        public static PMDManager PMDManager = null;
        public static MoonDriverManager MoonDriverManager = null;

        private static object lockObj = new object();
        private static bool _fatalError = false;
        public static bool fatalError
        {
            get
            {
                lock (lockObj)
                {
                    return _fatalError;
                }
            }

            set
            {
                lock (lockObj)
                {
                    _fatalError = value;
                }
            }
        }

        private static uint samplingBuffer = 1024;
        private static MDSound.MDSound mds = null;


        public static bool ReadyOK()
        {
            return mds != null;
        }

        public static RealChip realChip;
        public static ChipRegister chipRegister = null;
        public static HashSet<EnmChip> useChip = new HashSet<EnmChip>();


        public static bool trdClosed = false;
        private static bool _trdStopped = true;
        public static bool trdStopped
        {
            get
            {
                lock (lockObj)
                {
                    return _trdStopped;
                }
            }
            set
            {
                lock (lockObj)
                {
                    _trdStopped = value;
                }
            }
        }
        private static Stopwatch sw = Stopwatch.StartNew();
        private static double swFreq = Stopwatch.Frequency;

        private static outDatum[] vgmBuf = null;
        private static long jumpPointClock;
        private static double vgmSpeed;

        private static musicDriverInterface.MmlDatum[] mubBuf = null;
        private static string mubWorkPath;
        private static string mubFileName;
        private static musicDriverInterface.MmlDatum[] mBuf = null;
        private static string mWorkPath;
        private static string mFileName;
        private static musicDriverInterface.MmlDatum[] mdrBuf = null;
        private static string mdrWorkPath;
        private static string mdrFileName;

        public static double fadeoutCounter;
        public static double fadeoutCounterEmu;
        private static double fadeoutCounterDelta;

        private static bool Paused = false;
        public static bool Stopped = false;
        public static int StepCounter = 0;

        public static Setting setting = null;

        public static baseDriver driver = null;

        private static bool hiyorimiNecessary = false;

        public static ChipLEDs chipLED = new ChipLEDs();
        public static VisVolume visVolume = new VisVolume();

        private static int MasterVolume = 0;
        private static byte[] chips = new byte[256];
        private static EnmFileFormat PlayingFileFormat;
        private static int MidiMode = 0;

        private static System.Diagnostics.Stopwatch stwh = System.Diagnostics.Stopwatch.StartNew();
        public static double ProcTimePer1Frame = 0;

        public static string errMsg = "";
        public static bool flgReinit = false;
#pragma warning disable CS0414 // フィールド 'Audio.bufVirtualFunction_MIDIKeyboard' が割り当てられていますが、値は使用されていません。
        private static short[] bufVirtualFunction_MIDIKeyboard = null;
#pragma warning restore CS0414 // フィールド 'Audio.bufVirtualFunction_MIDIKeyboard' が割り当てられていますが、値は使用されていません。
        private static byte[] mmc5regs = new byte[10];

        private static List<NAudio.Midi.MidiOut> midiOuts = new List<NAudio.Midi.MidiOut>();
        private static List<int> midiOutsType = new List<int>();

        public static SoundManager.SoundManager sm = null;
        private static Enq enq;
        private static RingBuffer emuRecvBuffer = null;
        public static long DriverSeqCounter = 0;
        public static long EmuSeqCounter = 0;
        public static Action<PackData> SetMMLTraceInfo = null;
        public static bool waveMode = false;
        public static bool waveModeAbort = false;


        public static void Init(Setting setting)
        {
            log.ForcedWrite("Audio:Init:Begin");

            log.ForcedWrite("Audio:Init:STEP 01");

            Common.SampleRate = setting.outputDevice.SampleRate;
            NAudioWrap.Init((int)Common.SampleRate, trdVgmVirtualFunction);
            NAudioWrap.PlaybackStopped += NaudioWrap_PlaybackStopped;


            log.ForcedWrite("Audio:Init:STEP 02");
            Audio.setting = setting;
            mmlParams.setting = setting;

            log.ForcedWrite("Audio:Init:STEP 03");
            {
            }



            log.ForcedWrite("Audio:Init:STEP 04");
            {
                if (realChip == null) realChip = new RealChip(!setting.unuseRealChip);

                chipRegister = new ChipRegister(setting, mds, realChip);

                chipRegister.initChipRegister(null);
            }



            log.ForcedWrite("Audio:Init:STEP 05");
            Paused = false;
            Stopped = true;
            fatalError = false;


            log.ForcedWrite("Audio:Init:STEP 09");
            NAudioWrap.Start(Audio.setting);



            log.ForcedWrite("Audio:Init:STEP 10");
            SoundManagerMount();



            log.ForcedWrite("Audio:Init:Complete");



        }

        public void SetMusicInterruptTimer(MusicInterruptTimer m)
        {
            if (sm == null) return;
            sm.SetMusicInterruptTimer(m);
        }

        internal static Chip GetChip(EnmChip chipType)
        {
            switch (chipType)
            {
                case EnmChip.YM2612:
                    return chipRegister.YM2612[0];
                default:
                    return null;
            }
        }

        public static void RealChipManualDetect(Setting setting)
        {
            chipRegister.SetRealChipInfo(EnmZGMDevice.AY8910, setting.AY8910Type, setting.AY8910SType, setting.LatencyEmulation, setting.LatencySCCI);
            chipRegister.SetRealChipInfo(EnmZGMDevice.GameBoyDMG, setting.DMGType, setting.DMGSType, setting.LatencyEmulation, setting.LatencySCCI);
            chipRegister.SetRealChipInfo(EnmZGMDevice.NESAPU, setting.NESType, setting.NESSType, setting.LatencyEmulation, setting.LatencySCCI);
            chipRegister.SetRealChipInfo(EnmZGMDevice.C140, setting.C140Type, setting.C140SType, setting.LatencyEmulation, setting.LatencySCCI);
            chipRegister.SetRealChipInfo(EnmZGMDevice.C352, setting.C352Type, setting.C352SType, setting.LatencyEmulation, setting.LatencySCCI);
            chipRegister.SetRealChipInfo(EnmZGMDevice.RF5C164, new Setting.ChipType(), new Setting.ChipType(), setting.LatencyEmulation, setting.LatencySCCI);
            chipRegister.SetRealChipInfo(EnmZGMDevice.SegaPCM, setting.SEGAPCMType, setting.SEGAPCMSType, setting.LatencyEmulation, setting.LatencySCCI);
            chipRegister.SetRealChipInfo(EnmZGMDevice.SN76489, setting.SN76489Type, setting.SN76489SType, setting.LatencyEmulation, setting.LatencySCCI);
            chipRegister.SetRealChipInfo(EnmZGMDevice.YM2151, setting.YM2151Type, setting.YM2151SType, setting.LatencyEmulation, setting.LatencySCCI);
            chipRegister.SetRealChipInfo(EnmZGMDevice.YM2203, setting.YM2203Type, setting.YM2203SType, setting.LatencyEmulation, setting.LatencySCCI);
            chipRegister.SetRealChipInfo(EnmZGMDevice.YM2413, setting.YM2413Type, setting.YM2413SType, setting.LatencyEmulation, setting.LatencySCCI);
            chipRegister.SetRealChipInfo(EnmZGMDevice.YM2608, setting.YM2608Type, setting.YM2608SType, setting.LatencyEmulation, setting.LatencySCCI);
            chipRegister.SetRealChipInfo(EnmZGMDevice.YM2609, setting.YM2609Type, setting.YM2609SType, setting.LatencyEmulation, setting.LatencySCCI);
            chipRegister.SetRealChipInfo(EnmZGMDevice.YM2610, setting.YM2610Type, setting.YM2610SType, setting.LatencyEmulation, setting.LatencySCCI);
            chipRegister.SetRealChipInfo(EnmZGMDevice.YM2612, setting.YM2612Type, setting.YM2612SType, setting.LatencyEmulation, setting.LatencySCCI);

            chipRegister.SetRealChipInfo(EnmZGMDevice.HuC6280, setting.HuC6280Type, setting.HuC6280SType, setting.LatencyEmulation, setting.LatencySCCI);
            chipRegister.SetRealChipInfo(EnmZGMDevice.K051649, setting.K051649Type, setting.K051649SType, setting.LatencyEmulation, setting.LatencySCCI);
            chipRegister.SetRealChipInfo(EnmZGMDevice.K053260, setting.K053260Type, setting.K053260SType, setting.LatencyEmulation, setting.LatencySCCI);
            chipRegister.SetRealChipInfo(EnmZGMDevice.PPZ8, setting.PPZ8Type, null, setting.LatencyEmulation, setting.LatencySCCI);
            chipRegister.SetRealChipInfo(EnmZGMDevice.PPSDRV, setting.PPSDRVType, null, setting.LatencyEmulation, setting.LatencySCCI);
            chipRegister.SetRealChipInfo(EnmZGMDevice.QSound, setting.QSoundType, null, setting.LatencyEmulation, setting.LatencySCCI);
            chipRegister.SetRealChipInfo(EnmZGMDevice.Y8950, setting.Y8950Type, setting.Y8950SType, setting.LatencyEmulation, setting.LatencySCCI);
            chipRegister.SetRealChipInfo(EnmZGMDevice.YM3526, setting.YM3526Type, setting.YM3526SType, setting.LatencyEmulation, setting.LatencySCCI);
            chipRegister.SetRealChipInfo(EnmZGMDevice.YM3812, setting.YM3812Type, setting.YM3812SType, setting.LatencyEmulation, setting.LatencySCCI);
            chipRegister.SetRealChipInfo(EnmZGMDevice.YMF262, setting.YMF262Type, setting.YMF262SType, setting.LatencyEmulation, setting.LatencySCCI);
            chipRegister.SetRealChipInfo(EnmZGMDevice.YMF271, setting.YMF271Type, setting.YMF271SType, setting.LatencyEmulation, setting.LatencySCCI);
            chipRegister.SetRealChipInfo(EnmZGMDevice.YMF278B, setting.YMF278BType, setting.YMF278BSType, setting.LatencyEmulation, setting.LatencySCCI);
            chipRegister.SetRealChipInfo(EnmZGMDevice.YMZ280B, setting.YMZ280BType, setting.YMZ280BSType, setting.LatencyEmulation, setting.LatencySCCI);
        }

        public static void RealChipAutoDetect(Setting setting)
        {
            Setting.ChipType[] chipType;
            List<Setting.ChipType> ret = realChip.GetRealChipList();

            chipType = new Setting.ChipType[Math.Max(chipRegister.AY8910.Count, 2)];
            for (int i = 0; i < Math.Max(chipRegister.AY8910.Count, 2); i++)
            {
                chipType[i] = new Setting.ChipType();
                if (chipRegister.AY8910.Count <= i) continue;
                if (!chipRegister.AY8910[i].Use) continue;
                chipRegister.AY8910[i].Model = EnmVRModel.VirtualModel;
                chipType[i].UseEmu = true;
                chipType[i].UseScci = false;
                if (ret.Count == 0) continue;
                SearchRealChip(chipType, ret, i, EnmZGMDevice.AY8910, chipRegister.AY8910[i], setting.AutoDetectModuleType == 0 ? 0 : 1);
                if (chipType[i].UseEmu) SearchRealChip(chipType, ret, i, EnmZGMDevice.AY8910, chipRegister.AY8910[i], setting.AutoDetectModuleType == 0 ? 1 : 0);
            }
            chipRegister.SetRealChipInfo(EnmZGMDevice.AY8910, chipType[0], chipType[1], setting.LatencyEmulation, setting.LatencySCCI);

            chipType = new Setting.ChipType[Math.Max(chipRegister.C140.Count, 2)];
            for (int i = 0; i < Math.Max(chipRegister.C140.Count, 2); i++)
            {
                chipType[i] = new Setting.ChipType();
                if (chipRegister.C140.Count <= i) continue;
                if (!chipRegister.C140[i].Use) continue;
                chipRegister.C140[i].Model = EnmVRModel.VirtualModel;
                chipType[i].UseEmu = true;
                chipType[i].UseScci = false;
                if (ret.Count == 0) continue;
                SearchRealChip(chipType, ret, i, EnmZGMDevice.C140, chipRegister.C140[i], setting.AutoDetectModuleType == 0 ? 0 : 1);
                if (chipType[i].UseEmu) SearchRealChip(chipType, ret, i, EnmZGMDevice.C140, chipRegister.C140[i], setting.AutoDetectModuleType == 0 ? 1 : 0);
            }
            chipRegister.SetRealChipInfo(EnmZGMDevice.C140, chipType[0], chipType[1], setting.LatencyEmulation, setting.LatencySCCI);

            chipType = new Setting.ChipType[Math.Max(chipRegister.C352.Count, 2)];
            for (int i = 0; i < Math.Max(chipRegister.C352.Count, 2); i++)
            {
                chipType[i] = new Setting.ChipType();
                if (chipRegister.C352.Count <= i) continue;
                if (!chipRegister.C352[i].Use) continue;
                chipRegister.C352[i].Model = EnmVRModel.VirtualModel;
                chipType[i].UseEmu = true;
                chipType[i].UseScci = false;
                if (ret.Count == 0) continue;
                SearchRealChip(chipType, ret, i, EnmZGMDevice.C352, chipRegister.C352[i], setting.AutoDetectModuleType == 0 ? 0 : 1);
                if (chipType[i].UseEmu) SearchRealChip(chipType, ret, i, EnmZGMDevice.C352, chipRegister.C352[i], setting.AutoDetectModuleType == 0 ? 1 : 0);
            }
            chipRegister.SetRealChipInfo(EnmZGMDevice.C352, chipType[0], chipType[1], setting.LatencyEmulation, setting.LatencySCCI);

            chipType = new Setting.ChipType[Math.Max(chipRegister.HuC6280.Count, 2)];
            for (int i = 0; i < Math.Max(chipRegister.HuC6280.Count, 2); i++)
            {
                chipType[i] = new Setting.ChipType();
                if (chipRegister.HuC6280.Count <= i) continue;
                if (!chipRegister.HuC6280[i].Use) continue;
                chipRegister.HuC6280[i].Model = EnmVRModel.VirtualModel;
                chipType[i].UseEmu = true;
                chipType[i].UseScci = false;
                if (ret.Count == 0) continue;
            }
            chipRegister.SetRealChipInfo(EnmZGMDevice.HuC6280, chipType[0], chipType[1], setting.LatencyEmulation, setting.LatencySCCI);

            chipType = new Setting.ChipType[Math.Max(chipRegister.K051649.Count, 2)];
            for (int i = 0; i < Math.Max(chipRegister.K051649.Count, 2); i++)
            {
                chipType[i] = new Setting.ChipType();
                if (chipRegister.K051649.Count <= i) continue;
                if (!chipRegister.K051649[i].Use) continue;
                chipRegister.K051649[i].Model = EnmVRModel.VirtualModel;
                chipType[i].UseEmu = true;
                chipType[i].UseScci = false;
                if (ret.Count == 0) continue;
            }
            chipRegister.SetRealChipInfo(EnmZGMDevice.K051649, chipType[0], chipType[1], setting.LatencyEmulation, setting.LatencySCCI);

            chipType = new Setting.ChipType[Math.Max(chipRegister.K053260.Count, 2)];
            for (int i = 0; i < Math.Max(chipRegister.K053260.Count, 2); i++)
            {
                chipType[i] = new Setting.ChipType();
                if (chipRegister.K053260.Count <= i) continue;
                if (!chipRegister.K053260[i].Use) continue;
                chipRegister.K053260[i].Model = EnmVRModel.VirtualModel;
                chipType[i].UseEmu = true;
                chipType[i].UseScci = false;
                if (ret.Count == 0) continue;
            }
            chipRegister.SetRealChipInfo(EnmZGMDevice.K053260, chipType[0], chipType[1], setting.LatencyEmulation, setting.LatencySCCI);

            chipType = new Setting.ChipType[Math.Max(chipRegister.PPZ8.Count, 2)];
            for (int i = 0; i < Math.Max(chipRegister.PPZ8.Count, 2); i++)
            {
                chipType[i] = new Setting.ChipType();
                if (chipRegister.PPZ8.Count <= i) continue;
                if (!chipRegister.PPZ8[i].Use) continue;
                chipRegister.PPZ8[i].Model = EnmVRModel.VirtualModel;
                chipType[i].UseEmu = true;
                chipType[i].UseScci = false;
                if (ret.Count == 0) continue;
            }
            chipRegister.SetRealChipInfo(EnmZGMDevice.PPZ8, chipType[0], chipType[1], setting.LatencyEmulation, setting.LatencySCCI);

            chipType = new Setting.ChipType[Math.Max(chipRegister.PPSDRV.Count, 2)];
            for (int i = 0; i < Math.Max(chipRegister.PPSDRV.Count, 2); i++)
            {
                chipType[i] = new Setting.ChipType();
                if (chipRegister.PPSDRV.Count <= i) continue;
                if (!chipRegister.PPSDRV[i].Use) continue;
                chipRegister.PPSDRV[i].Model = EnmVRModel.VirtualModel;
                chipType[i].UseEmu = true;
                chipType[i].UseScci = false;
                if (ret.Count == 0) continue;
            }
            chipRegister.SetRealChipInfo(EnmZGMDevice.PPSDRV, chipType[0], chipType[1], setting.LatencyEmulation, setting.LatencySCCI);

            chipType = new Setting.ChipType[Math.Max(chipRegister.P86.Count, 2)];
            for (int i = 0; i < Math.Max(chipRegister.P86.Count, 2); i++)
            {
                chipType[i] = new Setting.ChipType();
                if (chipRegister.P86.Count <= i) continue;
                if (!chipRegister.P86[i].Use) continue;
                chipRegister.P86[i].Model = EnmVRModel.VirtualModel;
                chipType[i].UseEmu = true;
                chipType[i].UseScci = false;
                if (ret.Count == 0) continue;
            }
            chipRegister.SetRealChipInfo(EnmZGMDevice.P86, chipType[0], chipType[1], setting.LatencyEmulation, setting.LatencySCCI);

            chipType = new Setting.ChipType[Math.Max(chipRegister.QSound.Count, 2)];
            for (int i = 0; i < Math.Max(chipRegister.QSound.Count, 2); i++)
            {
                chipType[i] = new Setting.ChipType();
                if (chipRegister.QSound.Count <= i) continue;
                if (!chipRegister.QSound[i].Use) continue;
                chipRegister.QSound[i].Model = EnmVRModel.VirtualModel;
                chipType[i].UseEmu = true;
                chipType[i].UseScci = false;
                if (ret.Count == 0) continue;
            }
            chipRegister.SetRealChipInfo(EnmZGMDevice.QSound, chipType[0], chipType[1], setting.LatencyEmulation, setting.LatencySCCI);

            chipType = new Setting.ChipType[Math.Max(chipRegister.RF5C164.Count, 2)];
            for (int i = 0; i < Math.Max(chipRegister.RF5C164.Count, 2); i++)
            {
                chipType[i] = new Setting.ChipType();
                if (chipRegister.RF5C164.Count <= i) continue;
                if (!chipRegister.RF5C164[i].Use) continue;
                chipRegister.RF5C164[i].Model = EnmVRModel.VirtualModel;
                chipType[i].UseEmu = true;
                chipType[i].UseScci = false;
                if (ret.Count == 0) continue;
            }
            chipRegister.SetRealChipInfo(EnmZGMDevice.RF5C164, chipType[0], chipType[1], setting.LatencyEmulation, setting.LatencySCCI);

            chipType = new Setting.ChipType[Math.Max(chipRegister.DMG.Count, 2)];
            for (int i = 0; i < Math.Max(chipRegister.DMG.Count, 2); i++)
            {
                chipType[i] = new Setting.ChipType();
                if (chipRegister.DMG.Count <= i) continue;
                if (!chipRegister.DMG[i].Use) continue;
                chipRegister.DMG[i].Model = EnmVRModel.VirtualModel;
                chipType[i].UseEmu = true;
                chipType[i].UseScci = false;
                if (ret.Count == 0) continue;
            }
            chipRegister.SetRealChipInfo(EnmZGMDevice.GameBoyDMG, chipType[0], chipType[1], setting.LatencyEmulation, setting.LatencySCCI);

            chipType = new Setting.ChipType[Math.Max(chipRegister.NES.Count, 2)];
            for (int i = 0; i < Math.Max(chipRegister.NES.Count, 2); i++)
            {
                chipType[i] = new Setting.ChipType();
                if (chipRegister.NES.Count <= i) continue;
                if (!chipRegister.NES[i].Use) continue;
                chipRegister.NES[i].Model = EnmVRModel.VirtualModel;
                chipType[i].UseEmu = true;
                chipType[i].UseScci = false;
                if (ret.Count == 0) continue;
            }
            chipRegister.SetRealChipInfo(EnmZGMDevice.NESAPU, chipType[0], chipType[1], setting.LatencyEmulation, setting.LatencySCCI);

            chipType = new Setting.ChipType[Math.Max(chipRegister.SEGAPCM.Count, 2)];
            for (int i = 0; i < Math.Max(chipRegister.SEGAPCM.Count, 2); i++)
            {
                chipType[i] = new Setting.ChipType();
                if (chipRegister.SEGAPCM.Count <= i) continue;
                if (!chipRegister.SEGAPCM[i].Use) continue;
                chipRegister.SEGAPCM[i].Model = EnmVRModel.VirtualModel;
                chipType[i].UseEmu = true;
                chipType[i].UseScci = false;
                if (ret.Count == 0) continue;
                SearchRealChip(chipType, ret, i, EnmZGMDevice.SegaPCM, chipRegister.SEGAPCM[i], setting.AutoDetectModuleType == 0 ? 0 : 1);
                if (chipType[i].UseEmu) SearchRealChip(chipType, ret, i, EnmZGMDevice.SegaPCM, chipRegister.SEGAPCM[i], setting.AutoDetectModuleType == 0 ? 1 : 0);
            }
            chipRegister.SetRealChipInfo(EnmZGMDevice.SegaPCM, chipType[0], chipType[1], setting.LatencyEmulation, setting.LatencySCCI);

            chipType = new Setting.ChipType[Math.Max(chipRegister.SN76489.Count, 2)];
            for (int i = 0; i < Math.Max(chipRegister.SN76489.Count, 2); i++)
            {
                chipType[i] = new Setting.ChipType();
                if (chipRegister.SN76489.Count <= i) continue;
                if (!chipRegister.SN76489[i].Use) continue;
                chipRegister.SN76489[i].Model = EnmVRModel.VirtualModel;
                chipType[i].UseEmu = true;
                chipType[i].UseScci = false;
                if (ret.Count == 0) continue;
                SearchRealChip(chipType, ret, i, EnmZGMDevice.SN76489, chipRegister.SN76489[i], setting.AutoDetectModuleType == 0 ? 0 : 1);
                if (chipType[i].UseEmu) SearchRealChip(chipType, ret, i, EnmZGMDevice.SN76489, chipRegister.SN76489[i], setting.AutoDetectModuleType == 0 ? 1 : 0);
            }
            chipRegister.SetRealChipInfo(EnmZGMDevice.SN76489, chipType[0], chipType[1], setting.LatencyEmulation, setting.LatencySCCI);

            chipType = new Setting.ChipType[Math.Max(chipRegister.YM2151.Count, 2)];
            for (int i = 0; i < Math.Max(chipRegister.YM2151.Count, 2); i++)
            {
                chipType[i] = new Setting.ChipType();
                if (chipRegister.YM2151.Count <= i) continue;
                if (!chipRegister.YM2151[i].Use) continue;
                chipRegister.YM2151[i].Model = EnmVRModel.VirtualModel;
                chipType[i].UseEmu = true;
                chipType[i].UseEmu2 = true;
                chipType[i].UseEmu3 = true;
                chipType[i].UseScci = false;
                if (ret.Count == 0) continue;
                SearchRealChip(chipType, ret, i, EnmZGMDevice.YM2151, chipRegister.YM2151[i], setting.AutoDetectModuleType == 0 ? 0 : 1);
                if (chipType[i].UseEmu) SearchRealChip(chipType, ret, i, EnmZGMDevice.YM2151, chipRegister.YM2151[i], setting.AutoDetectModuleType == 0 ? 1 : 0);
            }
            chipRegister.SetRealChipInfo(EnmZGMDevice.YM2151, chipType[0], chipType[1], setting.LatencyEmulation, setting.LatencySCCI);

            chipType = new Setting.ChipType[Math.Max(chipRegister.YM2203.Count, 2)];
            for (int i = 0; i < Math.Max(chipRegister.YM2203.Count, 2); i++)
            {
                chipType[i] = new Setting.ChipType();
                if (chipRegister.YM2203.Count <= i) continue;
                if (!chipRegister.YM2203[i].Use) continue;
                chipRegister.YM2203[i].Model = EnmVRModel.VirtualModel;
                chipType[i].UseEmu = true;
                chipType[i].UseScci = false;
                if (ret.Count == 0) continue;
                SearchRealChip(chipType, ret, i, EnmZGMDevice.YM2203, chipRegister.YM2203[i], setting.AutoDetectModuleType == 0 ? 0 : 1);
                if (chipType[i].UseEmu) SearchRealChip(chipType, ret, i, EnmZGMDevice.YM2203, chipRegister.YM2203[i], setting.AutoDetectModuleType == 0 ? 1 : 0);
            }
            chipRegister.SetRealChipInfo(EnmZGMDevice.YM2203, chipType[0], chipType[1], setting.LatencyEmulation, setting.LatencySCCI);

            chipType = new Setting.ChipType[Math.Max(chipRegister.YM2413.Count, 2)];
            for (int i = 0; i < Math.Max(chipRegister.YM2413.Count, 2); i++)
            {
                chipType[i] = new Setting.ChipType();
                if (chipRegister.YM2413.Count <= i) continue;
                if (!chipRegister.YM2413[i].Use) continue;
                chipRegister.YM2413[i].Model = EnmVRModel.VirtualModel;
                chipType[i].UseEmu = true;
                chipType[i].UseScci = false;
                if (ret.Count == 0) continue;
                SearchRealChip(chipType, ret, i, EnmZGMDevice.YM2413, chipRegister.YM2413[i], setting.AutoDetectModuleType == 0 ? 0 : 1);
                if (chipType[i].UseEmu) SearchRealChip(chipType, ret, i, EnmZGMDevice.YM2413, chipRegister.YM2413[i], setting.AutoDetectModuleType == 0 ? 1 : 0);
            }
            chipRegister.SetRealChipInfo(EnmZGMDevice.YM2413, chipType[0], chipType[1], setting.LatencyEmulation, setting.LatencySCCI);

            chipType = new Setting.ChipType[Math.Max(chipRegister.YM3526.Count, 2)];
            for (int i = 0; i < Math.Max(chipRegister.YM3526.Count, 2); i++)
            {
                chipType[i] = new Setting.ChipType();
                if (chipRegister.YM3526.Count <= i) continue;
                if (!chipRegister.YM3526[i].Use) continue;
                chipRegister.YM3526[i].Model = EnmVRModel.VirtualModel;
                chipType[i].UseEmu = true;
                chipType[i].UseScci = false;
                if (ret.Count == 0) continue;
                SearchRealChip(chipType, ret, i, EnmZGMDevice.YM3526, chipRegister.YM3526[i], setting.AutoDetectModuleType == 0 ? 0 : 1);
                if (chipType[i].UseEmu) SearchRealChip(chipType, ret, i, EnmZGMDevice.YM3526, chipRegister.YM3526[i], setting.AutoDetectModuleType == 0 ? 1 : 0);
            }
            chipRegister.SetRealChipInfo(EnmZGMDevice.YM3526, chipType[0], chipType[1], setting.LatencyEmulation, setting.LatencySCCI);

            chipType = new Setting.ChipType[Math.Max(chipRegister.Y8950.Count, 2)];
            for (int i = 0; i < Math.Max(chipRegister.Y8950.Count, 2); i++)
            {
                chipType[i] = new Setting.ChipType();
                if (chipRegister.Y8950.Count <= i) continue;
                if (!chipRegister.Y8950[i].Use) continue;
                chipRegister.Y8950[i].Model = EnmVRModel.VirtualModel;
                chipType[i].UseEmu = true;
                chipType[i].UseScci = false;
                if (ret.Count == 0) continue;
                SearchRealChip(chipType, ret, i, EnmZGMDevice.Y8950, chipRegister.Y8950[i], setting.AutoDetectModuleType == 0 ? 0 : 1);
                if (chipType[i].UseEmu) SearchRealChip(chipType, ret, i, EnmZGMDevice.Y8950, chipRegister.Y8950[i], setting.AutoDetectModuleType == 0 ? 1 : 0);
            }
            chipRegister.SetRealChipInfo(EnmZGMDevice.Y8950, chipType[0], chipType[1], setting.LatencyEmulation, setting.LatencySCCI);

            chipType = new Setting.ChipType[Math.Max(chipRegister.YM3812.Count, 2)];
            for (int i = 0; i < Math.Max(chipRegister.YM3812.Count, 2); i++)
            {
                chipType[i] = new Setting.ChipType();
                if (chipRegister.YM3812.Count <= i) continue;
                if (!chipRegister.YM3812[i].Use) continue;
                chipRegister.YM3812[i].Model = EnmVRModel.VirtualModel;
                chipType[i].UseEmu = true;
                chipType[i].UseScci = false;
                if (ret.Count == 0) continue;
                SearchRealChip(chipType, ret, i, EnmZGMDevice.YM3812, chipRegister.YM3812[i], setting.AutoDetectModuleType == 0 ? 0 : 1);
                if (chipType[i].UseEmu) SearchRealChip(chipType, ret, i, EnmZGMDevice.YM3812, chipRegister.YM3812[i], setting.AutoDetectModuleType == 0 ? 1 : 0);
            }
            chipRegister.SetRealChipInfo(EnmZGMDevice.YM3812, chipType[0], chipType[1], setting.LatencyEmulation, setting.LatencySCCI);

            chipType = new Setting.ChipType[Math.Max(chipRegister.YMF262.Count, 2)];
            for (int i = 0; i < Math.Max(chipRegister.YMF262.Count, 2); i++)
            {
                chipType[i] = new Setting.ChipType();
                if (chipRegister.YMF262.Count <= i) continue;
                if (!chipRegister.YMF262[i].Use) continue;
                chipRegister.YMF262[i].Model = EnmVRModel.VirtualModel;
                chipType[i].UseEmu = true;
                chipType[i].UseScci = false;
                if (ret.Count == 0) continue;
                SearchRealChip(chipType, ret, i, EnmZGMDevice.YMF262, chipRegister.YMF262[i], setting.AutoDetectModuleType == 0 ? 0 : 1);
                if (chipType[i].UseEmu) SearchRealChip(chipType, ret, i, EnmZGMDevice.YMF262, chipRegister.YMF262[i], setting.AutoDetectModuleType == 0 ? 1 : 0);
            }
            chipRegister.SetRealChipInfo(EnmZGMDevice.YMF262, chipType[0], chipType[1], setting.LatencyEmulation, setting.LatencySCCI);

            chipType = new Setting.ChipType[Math.Max(chipRegister.YMF278B.Count, 2)];
            for (int i = 0; i < Math.Max(chipRegister.YMF278B.Count, 2); i++)
            {
                chipType[i] = new Setting.ChipType();
                if (chipRegister.YMF278B.Count <= i) continue;
                if (!chipRegister.YMF278B[i].Use) continue;
                chipRegister.YMF278B[i].Model = EnmVRModel.VirtualModel;
                chipType[i].UseEmu = true;
                chipType[i].UseScci = false;
                if (ret.Count == 0) continue;
                SearchRealChip(chipType, ret, i, EnmZGMDevice.YMF278B, chipRegister.YMF278B[i], setting.AutoDetectModuleType == 0 ? 0 : 1);
                if (chipType[i].UseEmu) SearchRealChip(chipType, ret, i, EnmZGMDevice.YMF278B, chipRegister.YMF278B[i], setting.AutoDetectModuleType == 0 ? 1 : 0);
            }
            chipRegister.SetRealChipInfo(EnmZGMDevice.YMF278B, chipType[0], chipType[1], setting.LatencyEmulation, setting.LatencySCCI);

            chipType = new Setting.ChipType[Math.Max(chipRegister.YMF271.Count, 2)];
            for (int i = 0; i < Math.Max(chipRegister.YMF271.Count, 2); i++)
            {
                chipType[i] = new Setting.ChipType();
                if (chipRegister.YMF271.Count <= i) continue;
                if (!chipRegister.YMF271[i].Use) continue;
                chipRegister.YMF271[i].Model = EnmVRModel.VirtualModel;
                chipType[i].UseEmu = true;
                chipType[i].UseScci = false;
                if (ret.Count == 0) continue;
                SearchRealChip(chipType, ret, i, EnmZGMDevice.YMF271, chipRegister.YMF271[i], setting.AutoDetectModuleType == 0 ? 0 : 1);
                if (chipType[i].UseEmu) SearchRealChip(chipType, ret, i, EnmZGMDevice.YMF271, chipRegister.YMF271[i], setting.AutoDetectModuleType == 0 ? 1 : 0);
            }
            chipRegister.SetRealChipInfo(EnmZGMDevice.YMF271, chipType[0], chipType[1], setting.LatencyEmulation, setting.LatencySCCI);

            chipType = new Setting.ChipType[Math.Max(chipRegister.YM2608.Count, 2)];
            for (int i = 0; i < Math.Max(chipRegister.YM2608.Count, 2); i++)
            {
                chipType[i] = new Setting.ChipType();
                if (chipRegister.YM2608.Count <= i) continue;
                if (!chipRegister.YM2608[i].Use) continue;
                chipRegister.YM2608[i].Model = EnmVRModel.VirtualModel;
                chipType[i].UseEmu = true;
                chipType[i].UseScci = false;
                if (ret.Count == 0) continue;
                SearchRealChip(chipType, ret, i, EnmZGMDevice.YM2608, chipRegister.YM2608[i], setting.AutoDetectModuleType == 0 ? 0 : 1);
                if (chipType[i].UseEmu) SearchRealChip(chipType, ret, i, EnmZGMDevice.YM2608, chipRegister.YM2608[i], setting.AutoDetectModuleType == 0 ? 1 : 0);
            }
            chipRegister.SetRealChipInfo(EnmZGMDevice.YM2608, chipType[0], chipType[1], setting.LatencyEmulation, setting.LatencySCCI);

            chipType = new Setting.ChipType[Math.Max(chipRegister.YM2609.Count, 2)];
            for (int i = 0; i < Math.Max(chipRegister.YM2609.Count, 2); i++)
            {
                chipType[i] = new Setting.ChipType();
                if (chipRegister.YM2609.Count <= i) continue;
                if (!chipRegister.YM2609[i].Use) continue;
                chipRegister.YM2609[i].Model = EnmVRModel.VirtualModel;
                chipType[i].UseEmu = true;
                chipType[i].UseScci = false;
                if (ret.Count == 0) continue;
                SearchRealChip(chipType, ret, i, EnmZGMDevice.YM2609, chipRegister.YM2609[i], setting.AutoDetectModuleType == 0 ? 0 : 1);
                if (chipType[i].UseEmu) SearchRealChip(chipType, ret, i, EnmZGMDevice.YM2609, chipRegister.YM2609[i], setting.AutoDetectModuleType == 0 ? 1 : 0);
            }
            chipRegister.SetRealChipInfo(EnmZGMDevice.YM2609, chipType[0], chipType[1], setting.LatencyEmulation, setting.LatencySCCI);

            chipType = new Setting.ChipType[Math.Max(chipRegister.YM2610.Count, 2)];
            for (int i = 0; i < Math.Max(chipRegister.YM2610.Count, 2); i++)
            {
                chipType[i] = new Setting.ChipType();
                if (chipRegister.YM2610.Count <= i) continue;
                if (!chipRegister.YM2610[i].Use) continue;
                chipRegister.YM2610[i].Model = EnmVRModel.VirtualModel;
                chipType[i].UseEmu = true;
                chipType[i].UseScci = false;
                if (ret.Count == 0) continue;
                SearchRealChip(chipType, ret, i, EnmZGMDevice.YM2610, chipRegister.YM2610[i], setting.AutoDetectModuleType == 0 ? 0 : 1);
                if (chipType[i].UseEmu) SearchRealChip(chipType, ret, i, EnmZGMDevice.YM2610, chipRegister.YM2610[i], setting.AutoDetectModuleType == 0 ? 1 : 0);
            }
            chipRegister.SetRealChipInfo(EnmZGMDevice.YM2610, chipType[0], chipType[1], setting.LatencyEmulation, setting.LatencySCCI);

            chipType = new Setting.ChipType[Math.Max(chipRegister.YM2612.Count,2)];
            for (int i = 0; i < Math.Max(chipRegister.YM2612.Count, 2); i++)
            {
                chipType[i] = new Setting.ChipType();
                if (chipRegister.YM2612.Count <= i) continue;
                if (!chipRegister.YM2612[i].Use) continue;
                chipRegister.YM2612[i].Model = EnmVRModel.VirtualModel;
                Setting.ChipType sct = (i == 0) ? setting.YM2612Type : setting.YM2612SType;
                chipType[i].UseEmu = sct.UseEmu;
                chipType[i].UseEmu2 = sct.UseEmu2;
                chipType[i].UseEmu3 = sct.UseEmu3;
                chipType[i].UseScci = false;

                if (ret.Count == 0) continue;
                SearchRealChip(chipType, ret, i, EnmZGMDevice.YM2612, chipRegister.YM2612[i], setting.AutoDetectModuleType == 0 ? 0 : 1);
                if (chipType[i].UseEmu) SearchRealChip(chipType, ret, i, EnmZGMDevice.YM2612, chipRegister.YM2612[i], setting.AutoDetectModuleType == 0 ? 1 : 0);

                Setting.ChipType ct = (i == 0) ? setting.YM2612Type : setting.YM2612SType;
                chipType[i].OnlyPCMEmulation = ct.OnlyPCMEmulation;
                chipType[i].UseEmu = ct.UseEmu;
                chipType[i].UseEmu2 = ct.UseEmu2;
                chipType[i].UseEmu3 = ct.UseEmu3;
            }
            chipRegister.SetRealChipInfo(EnmZGMDevice.YM2612, chipType[0], chipType[1], setting.LatencyEmulation, setting.LatencySCCI);

        }

        private static void SearchRealChip(Setting.ChipType[] chipType, List<Setting.ChipType> ret, int i, EnmZGMDevice dev, Chip chip, int ModuleType)
        {
            for (int j = 0; j < ret.Count; j++)
            {
                if (ModuleType == 0)//scci
                {
                    if (ret[j].SoundLocation == -1) continue;
                }
                else
                {
                    if (ret[j].SoundLocation != -1) continue;
                }

                EnmRealModel mdl = CheckRealChip(dev, ret[j]);
                if (mdl != EnmRealModel.unknown)
                {
                    chipType[i] = ret[j];
                    chip.Model = EnmVRModel.RealModel;
                    chipType[i].UseEmu = false;
                    chipType[i].UseEmu2 = false;
                    chipType[i].UseEmu3 = false;
                    chipType[i].UseScci = true;

                    ret.RemoveAt(j);

                    break;
                }
            }
        }

        private static EnmRealModel CheckRealChip(EnmZGMDevice dev, Setting.ChipType chipType)
        {
            switch (dev)
            {
                case EnmZGMDevice.AY8910:
                    if (chipType.SoundLocation == -1) //GIMIC ?
                    {
                        if (chipType.Type == (int)Nc86ctl.ChipType.CHIP_YM2608
                            || chipType.Type == (int)Nc86ctl.ChipType.CHIP_YMF288
                            || chipType.Type == (int)Nc86ctl.ChipType.CHIP_YM2203)
                        {
                            return EnmRealModel.GIMIC;
                        }
                    }
                    else
                    {
                        if (chipType.Type == (int)EnmRealChipType.AY8910
                            || chipType.Type == (int)EnmRealChipType.YM2203
                            || chipType.Type == (int)EnmRealChipType.YM2608
                            || chipType.Type == (int)EnmRealChipType.YM2610)
                        {
                            return EnmRealModel.SCCI;
                        }
                    }
                    break;
                case EnmZGMDevice.C140:
                    if (chipType.SoundLocation == -1)
                    {
                    }
                    else
                    {
                        if (chipType.Type == (int)EnmRealChipType.C140)
                        {
                            return EnmRealModel.SCCI;
                        }
                    }
                    break;
                case EnmZGMDevice.SN76489:
                    if (chipType.SoundLocation == -1)
                    {
                    }
                    else
                    {
                        if (chipType.Type == (int)EnmRealChipType.SN76489)
                        {
                            return EnmRealModel.SCCI;
                        }
                    }
                    break;
                case EnmZGMDevice.SegaPCM:
                    if (chipType.SoundLocation == -1)
                    {
                    }
                    else
                    {
                        if (chipType.Type == (int)EnmRealChipType.SEGAPCM)
                        {
                            return EnmRealModel.SCCI;
                        }
                    }
                    break;
                case EnmZGMDevice.YM2151:
                    if (chipType.SoundLocation == -1)
                    {
                        if (chipType.Type == (int)Nc86ctl.ChipType.CHIP_YM2151)
                        {
                            return EnmRealModel.GIMIC;
                        }
                    }
                    else
                    {
                        if (chipType.Type == (int)EnmRealChipType.YM2151)
                        {
                            return EnmRealModel.SCCI;
                        }
                    }
                    break;
                case EnmZGMDevice.YM2203:
                    if (chipType.SoundLocation == -1)
                    {
                        if (chipType.Type == (int)Nc86ctl.ChipType.CHIP_YM2203
                            || chipType.Type == (int)Nc86ctl.ChipType.CHIP_YM2608
                            || chipType.Type == (int)Nc86ctl.ChipType.CHIP_YM2610B
                            || chipType.Type == (int)Nc86ctl.ChipType.CHIP_YMF288)
                        {
                            return EnmRealModel.GIMIC;
                        }
                    }
                    else
                    {
                        if (chipType.Type == (int)EnmRealChipType.YM2203
                            || chipType.Type == (int)EnmRealChipType.YM2608
                            || chipType.Type == (int)EnmRealChipType.YM2610)
                        {
                            return EnmRealModel.SCCI;
                        }
                    }
                    break;
                case EnmZGMDevice.YM2413:
                    if (chipType.SoundLocation == -1)
                    {
                        if (chipType.Type == (int)Nc86ctl.ChipType.CHIP_YM2413)
                        {
                            return EnmRealModel.GIMIC;
                        }
                    }
                    else
                    {
                        if (chipType.Type == (int)EnmRealChipType.YM2413)
                        {
                            return EnmRealModel.SCCI;
                        }
                    }
                    break;
                case EnmZGMDevice.YM2608:
                    if (chipType.SoundLocation == -1)
                    {
                        if (chipType.Type == (int)Nc86ctl.ChipType.CHIP_YM2608
                            || chipType.Type == (int)Nc86ctl.ChipType.CHIP_YMF288)
                        {
                            return EnmRealModel.GIMIC;
                        }
                    }
                    else
                    {
                        if (chipType.Type == (int)EnmRealChipType.YM2608)
                        {
                            return EnmRealModel.SCCI;
                        }
                    }
                    break;
                case EnmZGMDevice.YM2609:
                    break;
                case EnmZGMDevice.YM2610:
                    if (chipType.SoundLocation == -1)
                    {
                        if (chipType.Type == (int)Nc86ctl.ChipType.CHIP_YM2608
                            || chipType.Type == (int)Nc86ctl.ChipType.CHIP_YM2610B
                            || chipType.Type == (int)Nc86ctl.ChipType.CHIP_YMF288)
                        {
                            return EnmRealModel.GIMIC;
                        }
                    }
                    else
                    {
                        if (chipType.Type == (int)EnmRealChipType.YM2608
                            || chipType.Type == (int)EnmRealChipType.YM2610)
                        {
                            return EnmRealModel.SCCI;
                        }
                    }
                    break;
                case EnmZGMDevice.YM2612:
                    if (chipType.SoundLocation == -1)
                    {
                        if (chipType.Type == (int)Nc86ctl.ChipType.CHIP_YM2612
                            || chipType.Type == (int)Nc86ctl.ChipType.CHIP_YM3438)
                        {
                            return EnmRealModel.GIMIC;
                        }
                    }
                    else
                    {
                        if (chipType.Type == (int)EnmRealChipType.YM2612)
                        {
                            return EnmRealModel.SCCI;
                        }
                    }
                    break;
            }
            return EnmRealModel.unknown;
        }

        public static void SetVGMBuffer(EnmFileFormat format, outDatum[] srcBuf, long jumpPointClock)
        {
            PlayingFileFormat = format;
            vgmBuf = srcBuf;
            Audio.jumpPointClock = jumpPointClock;
        }

        public static void SetVGMBuffer(EnmFileFormat format, musicDriverInterface.MmlDatum[] srcBuf, string wrkPath, string mdFileName)
        {
            PlayingFileFormat = format;

            if (format == EnmFileFormat.MUB)
            {
                mubBuf = srcBuf;
                mubWorkPath = wrkPath;
                Audio.mubFileName = mdFileName;
            }
            else if (format == EnmFileFormat.M)
            {
                mBuf = srcBuf;
                mWorkPath = wrkPath;
                Audio.mFileName = mdFileName;
            }
            else if (format == EnmFileFormat.MDR)
            {
                mdrBuf = srcBuf;
                mdrWorkPath = wrkPath;
                Audio.mdrFileName = mdFileName;
            }
        }

        private static void SoundManagerMount()
        {
            sm = new SoundManager.SoundManager();
            DriverAction DriverAction = new DriverAction();
            DriverAction.Init = DriverActionInit;
            DriverAction.Main = DriverActionMain;
            DriverAction.Final = DriverActionFinal;
            sm.setting = setting;
            sm.Setup(
                DriverAction, RealChipAction
                , chipRegister.ProcessingData
                , mmlParams.SetMMLParameter
                , DataSeqFrqCallBack, WaitSync
                , null
                , SoundManager.SoundManager.DATA_SEQUENCE_FREQUENCE * setting.LatencyEmulation / 1000
                , SoundManager.SoundManager.DATA_SEQUENCE_FREQUENCE * setting.LatencySCCI / 1000);
            enq = sm.GetDriverDataEnqueue();
            chipRegister.enq = enq;
            emuRecvBuffer = sm.GetEmuRecvBuffer();
        }

        private static void WaitSync()
        {
            log.Write("Reset Audio Device Sync.");
            Thread.Sleep(50);
            //ResetAudioDeviceSync();
            //while (!GetAudioDeviceSync()) { Thread.Sleep(1); }
            ResetAudioDeviceSync();
            while (!GetAudioDeviceSync()) { Thread.Sleep(0); }
            if (sm.GetSeqCounter() > 0)
            {
                log.Write(string.Format("Warn:{0}", sm.GetSeqCounter()));
            }
        }

        private static void DataSeqFrqCallBack(long Counter)
        {

            if (!sm.GetFadeOut()) return;

            //
            // Fadeout 処理
            //

            fadeoutCounter -= fadeoutCounterDelta;
            if (fadeoutCounter < 0.7)
            {
                fadeoutCounter -= fadeoutCounterDelta * 2.0;
            }

            // fadeout完了したら演奏停止
            if (fadeoutCounter <= 0.0)
            {
                fadeoutCounter = 0.0;
                sm.RequestStopAsync();
            }

            chipRegister.SetFadeoutVolume(Counter, fadeoutCounter);
        }

        private static void DriverActionInit()
        {
            ;
        }

        private static void DriverActionMain()
        {
            driver.oneFrameProc();
            if (driver.Stopped)
            {
                sm.RequestStopAtDataMaker();
            }
        }

        private static void DriverActionFinal()
        {
            softReset(DriverSeqCounter);
        }

        static long rPackCounter = 0;
        static PackData rPack = new PackData();

        private static void RealChipAction(outDatum od, long Counter, Chip Chip, EnmDataType Type, int Address, int Data, object ExData)
        {

            rPackCounter = Counter;
            rPack.od = od;
            rPack.Chip = Chip;
            rPack.Type = Type;
            rPack.Address = Address;
            rPack.Data = Data;
            rPack.ExData = ExData;
            if (rPack.Address != -1 || rPack.Data != -1 || rPack.ExData != null)
            {
                chipRegister.SendChipData(rPackCounter, rPack.Chip, rPack.Type, rPack.Address, rPack.Data, rPack.ExData);
            }
            else
            {
                ;
            }

            //SetMMLTraceInfo?.Invoke(rPack);
        }


        public static void RealChipClose()
        {
            if (realChip != null)
            {
                realChip.Close();
            }
        }

        public static List<Setting.ChipType> GetRealChipList(EnmRealChipType scciType)
        {
            if (realChip == null) return null;
            return realChip.GetRealChipList(scciType);
        }

        public static MDSound.MDSound.Chip GetMDSChipInfo(MDSound.MDSound.enmInstrumentType typ)
        {
            return chipRegister.GetChipInfo(typ);
        }

        public static int getLatency()
        {
            if (setting.outputDevice.DeviceType != Common.DEV_AsioOut)
            {
                return (int)Common.SampleRate * setting.outputDevice.Latency / 1000;
            }
            return NAudioWrap.getAsioLatency();
        }

        public static bool Play(Setting setting, bool doSkipStop = false, Action startedOnceMethod = null)
        {
            useEmu = false;
            useReal = false;

            errMsg = "";
            Stop(SendMode.Both);

            if (doSkipStop)
            {
                sm.SetMode(SendMode.RealTime);
            }

            sm.SetSpeed(1.0);
            vgmSpeed = 1.0;
            sm.SetMusicInterruptTimer(setting.musicInterruptTimer);

            //スレッドなどの準備など(?)で演奏開始時にテンポが乱れることがあるため念のため待つ。
            DriverSeqCounter = sm.GetDriverSeqCounterDelay();

            //開始時にバッファ分のデータが貯まらないうちにコールバックがくるとテンポが乱れるため、レイテンシ(デバイスのバッファ)分だけ演奏開始を待つ。
            DriverSeqCounter += getLatency();

            MDSound.MDSound.np_nes_apu_volume = 0;
            MDSound.MDSound.np_nes_dmc_volume = 0;
            MDSound.MDSound.np_nes_fds_volume = 0;
            MDSound.MDSound.np_nes_fme7_volume = 0;
            MDSound.MDSound.np_nes_mmc5_volume = 0;
            MDSound.MDSound.np_nes_n106_volume = 0;
            MDSound.MDSound.np_nes_vrc6_volume = 0;
            MDSound.MDSound.np_nes_vrc7_volume = 0;


            bool ret = playSet();

            sm.SetMode(SendMode.MML);
            if (sm.Mode == SendMode.MML)
            {
                sm.RequestStart(SendMode.MML, useEmu, useReal);
            }
            else
            {
                sm.RequestStart(SendMode.Both, useEmu, useReal);
            }

            while (!sm.IsRunningAsync())
            {
            }

            Audio.startedOnceMethod = startedOnceMethod;

            EmuSeqCounter = 0;
            Stopped = false;

            //            if (!useEmu) 
            //                sm.RequestStopAtEmuChipSender();
            //            if (!useReal) 
            //                sm.RequestStopAtRealChipSender();


            return ret;
        }

        private static bool playSet()
        {
            bool ret;

            if (PlayingFileFormat == EnmFileFormat.XGM)
            {
                driver = new xgm();
                driver.setting = setting;

                ret = xgmPlay(setting);
            }
            else if (PlayingFileFormat == EnmFileFormat.ZGM)
            {
                //zgmはchipの再定義が必須の為、初期化を行うとmask情報も初期化されてしまう。
                //その為いったん退避しておく
                List<Chip> maskChips = null;
                if (driver != null && (driver is Driver.ZGM.zgm) && ((Driver.ZGM.zgm)driver).chips != null)
                {
                    maskChips = ((Driver.ZGM.zgm)driver).chips;
                }

                driver = new Driver.ZGM.zgm();
                driver.setting = setting;
                //((Driver.ZGM.zgm)driver).dacControl.chipRegister = chipRegister;
                //((Driver.ZGM.zgm)driver).dacControl.model = EnmVRModel.VirtualModel;
                //((Driver.ZGM.zgm)driver).dacControl.driver = ((Driver.ZGM.zgm)driver);

                ret = zgmPlay(setting);

                //mask情報の復帰
                if (ret && maskChips != null)
                {
                    foreach (Chip chip in ((Driver.ZGM.zgm)driver).chips)
                    {
                        foreach (Chip mchip in maskChips)
                        {
                            if (chip.Device != mchip.Device) continue;
                            if (chip.Index != mchip.Index) continue;
                            if (chip.Number != mchip.Number) continue;

                            chip.ChMasks = mchip.ChMasks;
                        }
                    }
                }
            }
            else if (PlayingFileFormat == EnmFileFormat.VGM)
            {
                driver = new vgm();
                driver.setting = setting;
                //((vgm)driver).dacControl.chipRegister = chipRegister;
                //((vgm)driver).dacControl.model = EnmVRModel.VirtualModel;

                ret = vgmPlay(setting);
            }
            else if (PlayingFileFormat == EnmFileFormat.MUB)
            {
                driver = new mucomMub();
                driver.setting = setting;

                ret = mubPlay(setting);
            }
            else if (PlayingFileFormat == EnmFileFormat.M)
            {
                driver = new pmdM();
                driver.setting = setting;

                ret = mPlay(setting);
            }
            else if (PlayingFileFormat == EnmFileFormat.MDR)
            {
                driver = new moonDriverM
                {
                    setting = setting
                };

                ret = mdrPlay(setting);
            }
            else
            {
                ret = false;
            }

            if (!ret) return false;// ?

            return ret;
        }

        public static bool xgmPlay(Setting setting)
        {

            try
            {

                if (vgmBuf == null || setting == null) return false;

                xgm xgmDriver = (xgm)driver;

                ResetFadeOutParam();
                useChip.Clear();
                chipRegister.ClearChipParam();

                List<MDSound.MDSound.Chip> lstChips = new List<MDSound.MDSound.Chip>();

                MDSound.MDSound.Chip chip;

                hiyorimiNecessary = setting.HiyorimiMode;

                chipLED = new ChipLEDs();

                MasterVolume = setting.balance.MasterVolume;

                if (!driver.init(vgmBuf, chipRegister, new EnmChip[] { EnmChip.YM2612, EnmChip.SN76489 }
                    , (uint)(Common.SampleRate * setting.LatencyEmulation / 1000)
                    , (uint)(Common.SampleRate * setting.outputDevice.WaitTime / 1000)
                    , jumpPointClock
                    )) return false;

                chip = new MDSound.MDSound.Chip();
                chip.ID = (byte)0;
                chip.Option = null;
                MDSound.ym2612X ym2612 = null;
                MDSound.ym3438X ym3438 = null;
                MDSound.ym2612mameX ym2612mame = null;

                List<byte> pcmBuf = new List<byte>();
                for(int i = 0; i < ((xgm)driver).sampleDataBlockSize * 256; i++)
                {
                    pcmBuf.Add(vgmBuf[((xgm)driver).sampleDataBlockAddr + i].val);
                }

                if (setting.YM2612Type.UseEmu)
                {
                    if (ym2612 == null) ym2612 = new MDSound.ym2612X();
                    chip.type = MDSound.MDSound.enmInstrumentType.YM2612;
                    chip.Instrument = ym2612;
                    chip.Update = ym2612.Update;
                    chip.Start = ym2612.Start;
                    chip.Stop = ym2612.Stop;
                    chip.Reset = ym2612.Reset;
                    chip.Option = new object[]
                    {
                        (int)(
                            (setting.gensOption.DACHPF ? 0x01: 0x00)
                            |(setting.gensOption.SSGEG ? 0x02: 0x00)
                        )
                    };
                }
                else if (setting.YM2612Type.UseEmu2)
                {
                    if (ym3438 == null) ym3438 = new ym3438X();
                    chip.type = MDSound.MDSound.enmInstrumentType.YM3438;
                    chip.Instrument = ym3438;
                    chip.Update = ym3438.Update;
                    chip.Start = ym3438.Start;
                    chip.Stop = ym3438.Stop;
                    chip.Reset = ym3438.Reset;
                    switch (setting.nukedOPN2.EmuType)
                    {
                        case 0:
                            ym3438.OPN2_SetChipType(ym3438_const.ym3438_type.discrete);
                            break;
                        case 1:
                            ym3438.OPN2_SetChipType(ym3438_const.ym3438_type.asic);
                            break;
                        case 2:
                            ym3438.OPN2_SetChipType(ym3438_const.ym3438_type.ym2612);
                            break;
                        case 3:
                            ym3438.OPN2_SetChipType(ym3438_const.ym3438_type.ym2612_u);
                            break;
                        case 4:
                            ym3438.OPN2_SetChipType(ym3438_const.ym3438_type.asic_lp);
                            break;
                    }
                }
                else if (setting.YM2612Type.UseEmu3)
                {
                    if (ym2612mame == null) ym2612mame = new ym2612mameX();
                    chip.type = MDSound.MDSound.enmInstrumentType.YM2612mame;
                    chip.Instrument = ym2612mame;
                    chip.Update = ym2612mame.Update;
                    chip.Start = ym2612mame.Start;
                    chip.Stop = ym2612mame.Stop;
                    chip.Reset = ym2612mame.Reset;
                }

                chip.SamplingRate = (UInt32)Common.SampleRate;
                chip.Volume = setting.balance.YM2612Volume;
                chip.Clock = (uint)xgmDriver.YM2612ClockValue;
                chip.Option = null;
                chipLED.PriOPN2 = 1;
                chipRegister.YM2612[0].Use = true;
                lstChips.Add(chip);
                useChip.Add(EnmChip.YM2612);

                chip = new MDSound.MDSound.Chip();
                chip.ID = (byte)0;
                sn76489 sn76489 = null;
                SN76496 sn76496 = null;

                if (setting.SN76489Type.UseEmu)
                {
                    if (sn76489 == null) sn76489 = new MDSound.sn76489();
                    chip.type = MDSound.MDSound.enmInstrumentType.SN76489;
                    chip.Instrument = sn76489;
                    chip.Update = sn76489.Update;
                    chip.Start = sn76489.Start;
                    chip.Stop = sn76489.Stop;
                    chip.Reset = sn76489.Reset;
                }
                else if (setting.SN76489Type.UseEmu2)
                {
                    if (sn76496 == null) sn76496 = new MDSound.SN76496();
                    chip.type = MDSound.MDSound.enmInstrumentType.SN76496;
                    chip.Instrument = sn76496;
                    chip.Update = sn76496.Update;
                    chip.Start = sn76496.Start;
                    chip.Stop = sn76496.Stop;
                    chip.Reset = sn76496.Reset;
                }

                chip.SamplingRate = (UInt32)Common.SampleRate;
                chip.Volume = setting.balance.SN76489Volume;
                chip.Clock = (uint)xgmDriver.SN76489ClockValue;
                chip.Option = null;

                chipLED.PriDCSG = 1;
                chipRegister.SN76489[0].Use = true;
                lstChips.Add(chip);
                useChip.Add(EnmChip.SN76489);

                if (hiyorimiNecessary) hiyorimiNecessary = true;
                else hiyorimiNecessary = false;

                log.Write("MDSound 初期化");

                if (mds == null)
                    mds = new MDSound.MDSound((UInt32)Common.SampleRate, samplingBuffer, lstChips.ToArray());
                else
                    mds.Init((UInt32)Common.SampleRate, samplingBuffer, lstChips.ToArray());

                if (setting.YM2612Type.UseEmu)
                {
                    ym2612.XGMfunction.sampleID[0] = ((xgm)driver).sampleID;
                    ym2612.XGMfunction.xgmpcm[0] = ((xgm)driver).xgmpcm;
                    ym2612.XGMfunction.pcmBuf[0] = pcmBuf.ToArray();
                }
                else if (setting.YM2612Type.UseEmu2)
                {
                    ym3438.XGMfunction.sampleID[0] = ((xgm)driver).sampleID;
                    ym3438.XGMfunction.xgmpcm[0] = ((xgm)driver).xgmpcm;
                    ym3438.XGMfunction.pcmBuf[0] = pcmBuf.ToArray();
                }
                else if (setting.YM2612Type.UseEmu3)
                {
                    ym2612mame.XGMfunction.sampleID[0] = ((xgm)driver).sampleID;
                    ym2612mame.XGMfunction.xgmpcm[0] = ((xgm)driver).xgmpcm;
                    ym2612mame.XGMfunction.pcmBuf[0] = pcmBuf.ToArray();
                }

                log.Write("ChipRegister 初期化");
                chipRegister.SetMDSound(mds);
                chipRegister.initChipRegister(lstChips.ToArray());

                if (setting.IsManualDetect)
                {
                    RealChipManualDetect(setting);
                }
                else
                {
                    RealChipAutoDetect(setting);
                }

                if (chipRegister.SN76489[0].Model == EnmVRModel.VirtualModel) useEmu = true;
                if (chipRegister.SN76489[0].Model == EnmVRModel.RealModel) useReal = true;

                if (chipRegister.YM2612[0].Model == EnmVRModel.VirtualModel) useEmu = true;
                if (chipRegister.YM2612[0].Model == EnmVRModel.RealModel)
                {
                    if (setting.YM2612Type.OnlyPCMEmulation) useEmu = true;
                    useReal = true;
                }

                log.Write("Volume 設定");

                SetYM2612Volume(true, setting.balance.YM2612Volume);
                SetSN76489Volume(true, setting.balance.SN76489Volume);

                log.Write("Clock 設定");

                chipRegister.SN76489WriteClock((byte)0, (int)xgmDriver.SN76489ClockValue);
                chipRegister.YM2612WriteClock((byte)0, (int)xgmDriver.YM2612ClockValue);

                //Play

                PackData[] stopData = MakeSoftResetData();
                sm.SetStopData(stopData);

                Paused = false;

                Thread.Sleep(100);

                log.Write("初期化完了");

                return true;
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
                return false;
            }

        }

        public static bool zgmPlay(Setting setting)
        {
            if (vgmBuf == null || setting == null) return false;

            try
            {

                Driver.ZGM.zgm zgmDriver = (Driver.ZGM.zgm)driver;
                ResetFadeOutParam();
                useChip.Clear();
                List<MDSound.MDSound.Chip> lstChips = new List<MDSound.MDSound.Chip>();
                MDSound.MDSound.Chip chip;
                chipLED = new ChipLEDs();


                log.Write("-----------------------");
                log.Write("ドライバ(ZGM)初期化");

                if (!driver.init(vgmBuf
                    , chipRegister
                    , new EnmChip[] { EnmChip.YM2203 }// usechip.ToArray()
                    , (uint)(Common.SampleRate * setting.LatencyEmulation / 1000)
                    , (uint)(Common.SampleRate * setting.outputDevice.WaitTime / 1000)
                    , jumpPointClock
                    ))
                    return false;

                hiyorimiNecessary = setting.HiyorimiMode;
                int hiyorimiDeviceFlag = 0;
                MasterVolume = setting.balance.MasterVolume;


                log.Write("使用チップの調査");
                {

                    int zCnt;

                    zCnt = -1;
                    foreach (Driver.ZGM.ZgmChip.ZgmChip zchip in zgmDriver.chips)
                    {
                        if (!(zchip is Driver.ZGM.ZgmChip.Conductor)) continue;

                        zCnt++;

                        hiyorimiDeviceFlag |= 0x2;

                        log.Write("Use Conductor");

                        chipRegister.CONDUCTOR[zCnt].Use = true;
                        chipRegister.CONDUCTOR[zCnt].Model = EnmVRModel.VirtualModel;
                        chipRegister.CONDUCTOR[zCnt].Device = EnmZGMDevice.Conductor;
                    }

                    zCnt = -1;
                    foreach (Driver.ZGM.ZgmChip.ZgmChip zchip in zgmDriver.chips)
                    {
                        if (!(zchip is Driver.ZGM.ZgmChip.C140)) continue;

                        zCnt++;
                        c140 c140 = new c140();
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.C140;
                        chip.ID = (byte)0;//ZGMでは常に0
                        chip.Instrument = c140;
                        chip.Update = c140.Update;
                        chip.Start = c140.Start;
                        chip.Stop = c140.Stop;
                        chip.Reset = c140.Reset;
                        chip.SamplingRate = (UInt32)Common.SampleRate;
                        chip.Volume = setting.balance.C140Volume;
                        chip.Clock = (uint)zchip.defineInfo.clock;
                        chip.Option = new object[1] {
                             (int)zchip.defineInfo.option[0]
                        };
                        lstChips.Add(chip);

                        hiyorimiDeviceFlag |= (setting.C140Type.UseScci) ? 0x1 : 0x2;

                        log.Write(string.Format("Use C140(#{0}) Clk:{1}"
                            , zCnt
                            , chip.Clock
                            ));

                        chipRegister.C140[zCnt].Use = true;
                        chipRegister.C140[zCnt].Model = EnmVRModel.VirtualModel;
                        chipRegister.C140[zCnt].Device = EnmZGMDevice.C140;
                    }

                    zCnt = -1;
                    foreach (Driver.ZGM.ZgmChip.ZgmChip zchip in zgmDriver.chips)
                    {
                        if (!(zchip is Driver.ZGM.ZgmChip.C352)) continue;

                        zCnt++;
                        c352 c352 = new c352();
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.C352;
                        chip.ID = (byte)0;//ZGMでは常に0
                        chip.Instrument = c352;
                        chip.Update = c352.Update;
                        chip.Start = c352.Start;
                        chip.Stop = c352.Stop;
                        chip.Reset = c352.Reset;
                        chip.SamplingRate = (UInt32)Common.SampleRate;
                        chip.Volume = setting.balance.C352Volume;
                        chip.Clock = (uint)zchip.defineInfo.clock;
                        chip.Option = null;
                        lstChips.Add(chip);

                        hiyorimiDeviceFlag |= (setting.C352Type.UseScci) ? 0x1 : 0x2;

                        log.Write(string.Format("Use C352(#{0}) Clk:{1}"
                            , zCnt
                            , chip.Clock
                            ));

                        chipRegister.C352[zCnt].Use = true;
                        chipRegister.C352[zCnt].Model = EnmVRModel.VirtualModel;
                        chipRegister.C352[zCnt].Device = EnmZGMDevice.C352;
                    }

                    zCnt = -1;
                    foreach (Driver.ZGM.ZgmChip.ZgmChip zchip in zgmDriver.chips)
                    {
                        if (!(zchip is Driver.ZGM.ZgmChip.HuC6280)) continue;

                        zCnt++;
                        Ootake_PSG huC6280 = new Ootake_PSG();
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.HuC6280;
                        chip.ID = (byte)0;//ZGMでは常に0
                        chip.Instrument = huC6280;
                        chip.Update = huC6280.Update;
                        chip.Start = huC6280.Start;
                        chip.Stop = huC6280.Stop;
                        chip.Reset = huC6280.Reset;
                        chip.SamplingRate = (UInt32)Common.SampleRate;
                        chip.Volume = setting.balance.C140Volume;
                        chip.Clock = (uint)zchip.defineInfo.clock;
                        chip.Option = null;
                        lstChips.Add(chip);

                        hiyorimiDeviceFlag |= (setting.HuC6280Type.UseScci) ? 0x1 : 0x2;

                        log.Write(string.Format("Use HuC6280(#{0}) Clk:{1}"
                            , zCnt
                            , chip.Clock
                            ));

                        chipRegister.HuC6280[zCnt].Use = true;
                        chipRegister.HuC6280[zCnt].Model = EnmVRModel.VirtualModel;
                        chipRegister.HuC6280[zCnt].Device = EnmZGMDevice.HuC6280;
                    }

                    zCnt = -1;
                    foreach (Driver.ZGM.ZgmChip.ZgmChip zchip in zgmDriver.chips)
                    {
                        if (!(zchip is Driver.ZGM.ZgmChip.K051649)) continue;

                        zCnt++;
                        MDSound.K051649 k051649 = new MDSound.K051649();
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.K051649;
                        chip.ID = (byte)0;//ZGMでは常に0
                        chip.Instrument = k051649;
                        chip.Update = k051649.Update;
                        chip.Start = k051649.Start;
                        chip.Stop = k051649.Stop;
                        chip.Reset = k051649.Reset;
                        chip.SamplingRate = (UInt32)Common.SampleRate;
                        chip.Volume = setting.balance.K051649Volume;
                        chip.Clock = (uint)zchip.defineInfo.clock;
                        chip.Option = null;
                        lstChips.Add(chip);

                        hiyorimiDeviceFlag |= (setting.K051649Type.UseScci) ? 0x1 : 0x2;

                        log.Write(string.Format("Use K051649(#{0}) Clk:{1}"
                            , zCnt
                            , chip.Clock
                            ));

                        chipRegister.K051649[zCnt].Use = true;
                        chipRegister.K051649[zCnt].Model = EnmVRModel.VirtualModel;
                        chipRegister.K051649[zCnt].Device = EnmZGMDevice.K051649;
                    }

                    zCnt = -1;
                    foreach (Driver.ZGM.ZgmChip.ZgmChip zchip in zgmDriver.chips)
                    {
                        if (!(zchip is Driver.ZGM.ZgmChip.K053260)) continue;

                        zCnt++;
                        MDSound.K053260 k053260 = new MDSound.K053260();
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.K053260;
                        chip.ID = (byte)0;//ZGMでは常に0
                        chip.Instrument = k053260;
                        chip.Update = k053260.Update;
                        chip.Start = k053260.Start;
                        chip.Stop = k053260.Stop;
                        chip.Reset = k053260.Reset;
                        chip.SamplingRate = (UInt32)Common.SampleRate;
                        chip.Volume = setting.balance.K053260Volume;
                        chip.Clock = (uint)zchip.defineInfo.clock;
                        chip.Option = new object[1] {
                             (int)zchip.defineInfo.option[0]
                        };
                        lstChips.Add(chip);

                        hiyorimiDeviceFlag |= (setting.K053260Type.UseScci) ? 0x1 : 0x2;

                        log.Write(string.Format("Use K053260(#{0}) Clk:{1}"
                            , zCnt
                            , chip.Clock
                            ));

                        chipRegister.K053260[zCnt].Use = true;
                        chipRegister.K053260[zCnt].Model = EnmVRModel.VirtualModel;
                        chipRegister.K053260[zCnt].Device = EnmZGMDevice.K053260;
                    }

                    zCnt = -1;
                    foreach (Driver.ZGM.ZgmChip.ZgmChip zchip in zgmDriver.chips)
                    {
                        if (!(zchip is Driver.ZGM.ZgmChip.QSound)) continue;

                        zCnt++;
                        MDSound.Qsound_ctr qsound = new MDSound.Qsound_ctr();
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.QSoundCtr;
                        chip.ID = (byte)0;//ZGMでは常に0
                        chip.Instrument = qsound;
                        chip.Update = qsound.Update;
                        chip.Start = qsound.Start;
                        chip.Stop = qsound.Stop;
                        chip.Reset = qsound.Reset;
                        chip.SamplingRate = (UInt32)Common.SampleRate;
                        chip.Volume = setting.balance.QSoundVolume;
                        chip.Clock = (uint)zchip.defineInfo.clock;
                        chip.Option = null;
                        lstChips.Add(chip);

                        hiyorimiDeviceFlag |= (setting.QSoundType.UseScci) ? 0x1 : 0x2;

                        log.Write(string.Format("Use QSound(#{0}) Clk:{1}"
                            , zCnt
                            , chip.Clock
                            ));

                        chipRegister.QSound[zCnt].Use = true;
                        chipRegister.QSound[zCnt].Model = EnmVRModel.VirtualModel;
                        chipRegister.QSound[zCnt].Device = EnmZGMDevice.QSound;
                    }

                    zCnt = -1;
                    foreach (Driver.ZGM.ZgmChip.ZgmChip zchip in zgmDriver.chips)
                    {
                        if (!(zchip is Driver.ZGM.ZgmChip.RF5C164)) continue;

                        zCnt++;
                        MDSound.scd_pcm rf5c164 = new MDSound.scd_pcm();
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.RF5C164;
                        chip.ID = (byte)0;//ZGMでは常に0
                        chip.Instrument = rf5c164;
                        chip.Update = rf5c164.Update;
                        chip.Start = rf5c164.Start;
                        chip.Stop = rf5c164.Stop;
                        chip.Reset = rf5c164.Reset;
                        chip.SamplingRate = (UInt32)Common.SampleRate;
                        chip.Volume = setting.balance.RF5C164Volume;
                        chip.Clock = (uint)zchip.defineInfo.clock;
                        chip.Option = null;
                        lstChips.Add(chip);

                        hiyorimiDeviceFlag |= (setting.RF5C164Type.UseScci) ? 0x1 : 0x2;

                        log.Write(string.Format("Use RF5C164(#{0}) Clk:{1}"
                            , zCnt
                            , chip.Clock
                            ));

                        chipRegister.RF5C164[zCnt].Use = true;
                        chipRegister.RF5C164[zCnt].Model = EnmVRModel.VirtualModel;
                        chipRegister.RF5C164[zCnt].Device = EnmZGMDevice.RF5C164;
                    }

                    zCnt = -1;
                    foreach (Driver.ZGM.ZgmChip.ZgmChip zchip in zgmDriver.chips)
                    {
                        if (!(zchip is Driver.ZGM.ZgmChip.DMG)) continue;

                        zCnt++;
                        MDSound.scd_pcm dmg = new MDSound.scd_pcm();
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.DMG;
                        chip.ID = (byte)0;//ZGMでは常に0
                        chip.Instrument = dmg;
                        chip.Update = dmg.Update;
                        chip.Start = dmg.Start;
                        chip.Stop = dmg.Stop;
                        chip.Reset = dmg.Reset;
                        chip.SamplingRate = (UInt32)Common.SampleRate;
                        chip.Volume = setting.balance.DMGVolume;
                        chip.Clock = (uint)zchip.defineInfo.clock;
                        chip.Option = null;
                        lstChips.Add(chip);

                        hiyorimiDeviceFlag |= (setting.DMGType.UseScci) ? 0x1 : 0x2;

                        log.Write(string.Format("Use DMG(#{0}) Clk:{1}"
                            , zCnt
                            , chip.Clock
                            ));

                        chipRegister.DMG[zCnt].Use = true;
                        chipRegister.DMG[zCnt].Model = EnmVRModel.VirtualModel;
                        chipRegister.DMG[zCnt].Device = EnmZGMDevice.GameBoyDMG;
                    }

                    zCnt = -1;
                    foreach (Driver.ZGM.ZgmChip.ZgmChip zchip in zgmDriver.chips)
                    {
                        if (!(zchip is Driver.ZGM.ZgmChip.NES)) continue;

                        zCnt++;
                        MDSound.scd_pcm nes = new MDSound.scd_pcm();
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.Nes;
                        chip.ID = (byte)0;//ZGMでは常に0
                        chip.Instrument = nes;
                        chip.Update = nes.Update;
                        chip.Start = nes.Start;
                        chip.Stop = nes.Stop;
                        chip.Reset = nes.Reset;
                        chip.SamplingRate = (UInt32)Common.SampleRate;
                        chip.Volume = setting.balance.APUVolume;
                        chip.Clock = (uint)zchip.defineInfo.clock;
                        chip.Option = null;
                        lstChips.Add(chip);

                        hiyorimiDeviceFlag |= (setting.NESType.UseScci) ? 0x1 : 0x2;

                        log.Write(string.Format("Use NES(#{0}) Clk:{1}"
                            , zCnt
                            , chip.Clock
                            ));

                        chipRegister.NES[zCnt].Use = true;
                        chipRegister.NES[zCnt].Model = EnmVRModel.VirtualModel;
                        chipRegister.NES[zCnt].Device = EnmZGMDevice.NESAPU;
                    }

                    zCnt = -1;
                    foreach (Driver.ZGM.ZgmChip.ZgmChip zchip in zgmDriver.chips)
                    {
                        if (!(zchip is Driver.ZGM.ZgmChip.SEGAPCM)) continue;

                        zCnt++;
                        MDSound.segapcm segapcm = new MDSound.segapcm();
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.SEGAPCM;
                        chip.ID = (byte)0;//ZGMでは常に0
                        chip.Instrument = segapcm;
                        chip.Update = segapcm.Update;
                        chip.Start = segapcm.Start;
                        chip.Stop = segapcm.Stop;
                        chip.Reset = segapcm.Reset;
                        chip.SamplingRate = (UInt32)Common.SampleRate;
                        chip.Volume = setting.balance.SEGAPCMVolume;
                        chip.Clock = (uint)zchip.defineInfo.clock;
                        chip.Option = new object[1] { (int)zchip.defineInfo.option[0] };
                        lstChips.Add(chip);

                        hiyorimiDeviceFlag |= (setting.QSoundType.UseScci) ? 0x1 : 0x2;

                        log.Write(string.Format("Use SEGAPCM(#{0}) Clk:{1}"
                            , zCnt
                            , chip.Clock
                            ));

                        chipRegister.SEGAPCM[zCnt].Use = true;
                        chipRegister.SEGAPCM[zCnt].Model = EnmVRModel.VirtualModel;
                        chipRegister.SEGAPCM[zCnt].Device = EnmZGMDevice.SegaPCM;
                    }

                    zCnt = -1;
                    foreach (Driver.ZGM.ZgmChip.ZgmChip zchip in zgmDriver.chips)
                    {
                        if (!(zchip is Driver.ZGM.ZgmChip.SN76489)) continue;

                        zCnt++;

                        chip = new MDSound.MDSound.Chip();
                        chip.ID = (byte)0;
                        sn76489 sn76489 = null;
                        SN76496 sn76496 = null;

                        if (setting.SN76489Type.UseEmu)
                        {
                            if (sn76489 == null) sn76489 = new MDSound.sn76489();
                            chip.type = MDSound.MDSound.enmInstrumentType.SN76489;
                            chip.Instrument = sn76489;
                            chip.Update = sn76489.Update;
                            chip.Start = sn76489.Start;
                            chip.Stop = sn76489.Stop;
                            chip.Reset = sn76489.Reset;
                        }
                        else if (setting.SN76489Type.UseEmu2)
                        {
                            if (sn76496 == null) sn76496 = new MDSound.SN76496();
                            chip.type = MDSound.MDSound.enmInstrumentType.SN76496;
                            chip.Instrument = sn76496;
                            chip.Update = sn76496.Update;
                            chip.Start = sn76496.Start;
                            chip.Stop = sn76496.Stop;
                            chip.Reset = sn76496.Reset;
                        }

                        chip.SamplingRate = (UInt32)Common.SampleRate;
                        chip.Volume = setting.balance.SN76489Volume;
                        chip.Clock = (uint)zchip.defineInfo.clock;
                        chip.Option = null;
                        lstChips.Add(chip);

                        hiyorimiDeviceFlag |= (setting.SN76489Type.UseScci) ? 0x1 : 0x2;

                        log.Write(string.Format("Use DCSG(#{0}) Clk:{1}"
                            , zCnt
                            , chip.Clock
                            ));

                        chipRegister.SN76489[zCnt].Use = true;
                        chipRegister.SN76489[zCnt].Model = EnmVRModel.VirtualModel;
                        chipRegister.SN76489[zCnt].Device = EnmZGMDevice.SN76489;
                    }

                    zCnt = -1;
                    foreach (Driver.ZGM.ZgmChip.ZgmChip zchip in zgmDriver.chips)
                    {
                        if (!(zchip is Driver.ZGM.ZgmChip.YM2151)) continue;

                        zCnt++;
                        chip = new MDSound.MDSound.Chip();
                        chip.ID = (byte)0;//ZGMでは常に0
                        MDSound.Instrument ym2151 = null;
                        if (setting.YM2151Type.UseEmu) ym2151 = new ym2151();
                        else if (setting.YM2151Type.UseEmu2) ym2151 = new ym2151_mame();
                        else if (setting.YM2151Type.UseEmu3) ym2151 = new ym2151_x68sound();

                        chip.type = MDSound.MDSound.enmInstrumentType.YM2151;
                        chip.Instrument = ym2151;
                        chip.Update = ym2151.Update;
                        chip.Start = ym2151.Start;
                        chip.Stop = ym2151.Stop;
                        chip.Reset = ym2151.Reset;
                        chip.SamplingRate = (UInt32)Common.SampleRate;
                        chip.Volume = setting.balance.YM2151Volume;
                        chip.Clock = (uint)zchip.defineInfo.clock;
                        chip.Option = null;
                        lstChips.Add(chip);

                        hiyorimiDeviceFlag |= 0x2;

                        log.Write(string.Format("Use OPM(#{0}) Clk:{1}"
                            , zCnt
                            , chip.Clock
                            ));

                        chipRegister.YM2151[zCnt].Use = true;
                        chipRegister.YM2151[zCnt].Model = EnmVRModel.VirtualModel;
                        chipRegister.YM2151[zCnt].Device = EnmZGMDevice.YM2151;
                    }

                    zCnt = -1;
                    foreach (Driver.ZGM.ZgmChip.ZgmChip zchip in zgmDriver.chips)
                    {
                        if (!(zchip is Driver.ZGM.ZgmChip.YM2203)) continue;

                        zCnt++;
                        chip = new MDSound.MDSound.Chip();
                        chip.ID = (byte)0;//ZGMでは常に0
                        ym2203 ym2203 = new ym2203();
                        chip.type = MDSound.MDSound.enmInstrumentType.YM2203;
                        chip.Instrument = ym2203;
                        chip.Update = ym2203.Update;
                        chip.Start = ym2203.Start;
                        chip.Stop = ym2203.Stop;
                        chip.Reset = ym2203.Reset;
                        chip.SamplingRate = (UInt32)Common.SampleRate;
                        chip.Volume = setting.balance.YM2203Volume;
                        chip.Clock = (uint)zchip.defineInfo.clock;
                        chip.Option = null;
                        lstChips.Add(chip);

                        hiyorimiDeviceFlag |= 0x2;

                        log.Write(string.Format("Use OPN(#{0}) Clk:{1}"
                            , zCnt
                            , chip.Clock
                            ));

                        chipRegister.YM2203[zCnt].Use = true;
                        chipRegister.YM2203[zCnt].Model = EnmVRModel.VirtualModel;
                        chipRegister.YM2203[zCnt].Device = EnmZGMDevice.YM2203;
                    }

                    zCnt = -1;
                    foreach (Driver.ZGM.ZgmChip.ZgmChip zchip in zgmDriver.chips)
                    {
                        if (!(zchip is Driver.ZGM.ZgmChip.YM2413)) continue;

                        zCnt++;
                        chip = new MDSound.MDSound.Chip();
                        chip.ID = (byte)0;//ZGMでは常に0
                        ym2413 ym2413 = new ym2413();
                        chip.type = MDSound.MDSound.enmInstrumentType.YM2413;
                        chip.Instrument = ym2413;
                        chip.Update = ym2413.Update;
                        chip.Start = ym2413.Start;
                        chip.Stop = ym2413.Stop;
                        chip.Reset = ym2413.Reset;
                        chip.SamplingRate = (UInt32)Common.SampleRate;
                        chip.Volume = setting.balance.YM2413Volume;
                        chip.Clock = (uint)zchip.defineInfo.clock;
                        chip.Option = null;
                        lstChips.Add(chip);

                        hiyorimiDeviceFlag |= 0x2;

                        log.Write(string.Format("Use OPLL(#{0}) Clk:{1}"
                            , zCnt
                            , chip.Clock
                            ));

                        chipRegister.YM2413[zCnt].Use = true;
                        chipRegister.YM2413[zCnt].Model = EnmVRModel.VirtualModel;
                        chipRegister.YM2413[zCnt].Device = EnmZGMDevice.YM2413;
                    }

                    zCnt = -1;
                    foreach (Driver.ZGM.ZgmChip.ZgmChip zchip in zgmDriver.chips)
                    {
                        if (!(zchip is Driver.ZGM.ZgmChip.YM3526)) continue;

                        zCnt++;
                        chip = new MDSound.MDSound.Chip();
                        chip.ID = (byte)0;//ZGMでは常に0
                        ym3526 ym3526 = new ym3526();
                        chip.type = MDSound.MDSound.enmInstrumentType.YM3526;
                        chip.Instrument = ym3526;
                        chip.Update = ym3526.Update;
                        chip.Start = ym3526.Start;
                        chip.Stop = ym3526.Stop;
                        chip.Reset = ym3526.Reset;
                        chip.SamplingRate = (UInt32)Common.SampleRate;
                        chip.Volume = setting.balance.YM3526Volume;
                        chip.Clock = (uint)zchip.defineInfo.clock;
                        chip.Option = null;
                        lstChips.Add(chip);

                        hiyorimiDeviceFlag |= 0x2;

                        log.Write(string.Format("Use OPL(#{0}) Clk:{1}"
                            , zCnt
                            , chip.Clock
                            ));

                        chipRegister.YM3526[zCnt].Use = true;
                        chipRegister.YM3526[zCnt].Model = EnmVRModel.VirtualModel;
                        chipRegister.YM3526[zCnt].Device = EnmZGMDevice.YM3526;
                    }

                    zCnt = -1;
                    foreach (Driver.ZGM.ZgmChip.ZgmChip zchip in zgmDriver.chips)
                    {
                        if (!(zchip is Driver.ZGM.ZgmChip.Y8950)) continue;

                        zCnt++;
                        chip = new MDSound.MDSound.Chip();
                        chip.ID = (byte)0;//ZGMでは常に0
                        y8950 y8950 = new y8950();
                        chip.type = MDSound.MDSound.enmInstrumentType.Y8950;
                        chip.Instrument = y8950;
                        chip.Update = y8950.Update;
                        chip.Start = y8950.Start;
                        chip.Stop = y8950.Stop;
                        chip.Reset = y8950.Reset;
                        chip.SamplingRate = (UInt32)Common.SampleRate;
                        chip.Volume = setting.balance.Y8950Volume;
                        chip.Clock = (uint)zchip.defineInfo.clock;
                        chip.Option = null;
                        lstChips.Add(chip);

                        hiyorimiDeviceFlag |= 0x2;

                        log.Write(string.Format("Use Y8950(#{0}) Clk:{1}"
                            , zCnt
                            , chip.Clock
                            ));

                        chipRegister.Y8950[zCnt].Use = true;
                        chipRegister.Y8950[zCnt].Model = EnmVRModel.VirtualModel;
                        chipRegister.Y8950[zCnt].Device = EnmZGMDevice.Y8950;
                    }

                    zCnt = -1;
                    foreach (Driver.ZGM.ZgmChip.ZgmChip zchip in zgmDriver.chips)
                    {
                        if (!(zchip is Driver.ZGM.ZgmChip.YM3812)) continue;

                        zCnt++;
                        chip = new MDSound.MDSound.Chip();
                        chip.ID = (byte)0;//ZGMでは常に0
                        ym3812 ym3812 = new ym3812();
                        chip.type = MDSound.MDSound.enmInstrumentType.YM3812;
                        chip.Instrument = ym3812;
                        chip.Update = ym3812.Update;
                        chip.Start = ym3812.Start;
                        chip.Stop = ym3812.Stop;
                        chip.Reset = ym3812.Reset;
                        chip.SamplingRate = (UInt32)Common.SampleRate;
                        chip.Volume = setting.balance.YM3812Volume;
                        chip.Clock = (uint)zchip.defineInfo.clock;
                        chip.Option = null;
                        lstChips.Add(chip);

                        hiyorimiDeviceFlag |= 0x2;

                        log.Write(string.Format("Use OPL(#{0}) Clk:{1}"
                            , zCnt
                            , chip.Clock
                            ));

                        chipRegister.YM3812[zCnt].Use = true;
                        chipRegister.YM3812[zCnt].Model = EnmVRModel.VirtualModel;
                        chipRegister.YM3812[zCnt].Device = EnmZGMDevice.YM3812;
                    }

                    zCnt = -1;
                    foreach (Driver.ZGM.ZgmChip.ZgmChip zchip in zgmDriver.chips)
                    {
                        if (!(zchip is Driver.ZGM.ZgmChip.YMF278B)) continue;

                        zCnt++;
                        chip = new MDSound.MDSound.Chip();
                        chip.ID = (byte)0;//ZGMでは常に0
                        ymf278b ymf278b = new ymf278b();
                        chip.type = MDSound.MDSound.enmInstrumentType.YMF278B;
                        chip.Instrument = ymf278b;
                        chip.Update = ymf278b.Update;
                        chip.Start = ymf278b.Start;
                        chip.Stop = ymf278b.Stop;
                        chip.Reset = ymf278b.Reset;
                        chip.SamplingRate = (UInt32)Common.SampleRate;
                        chip.Volume = setting.balance.YMF278BVolume;
                        chip.Clock = (uint)zchip.defineInfo.clock;
                        chip.Option = null;
                        lstChips.Add(chip);

                        hiyorimiDeviceFlag |= 0x2;

                        log.Write(string.Format("Use OPL4(#{0}) Clk:{1}"
                            , zCnt
                            , chip.Clock
                            ));

                        chipRegister.YMF278B[zCnt].Use = true;
                        chipRegister.YMF278B[zCnt].Model = EnmVRModel.VirtualModel;
                        chipRegister.YMF278B[zCnt].Device = EnmZGMDevice.YMF278B;
                    }

                    zCnt = -1;
                    foreach (Driver.ZGM.ZgmChip.ZgmChip zchip in zgmDriver.chips)
                    {
                        if (!(zchip is Driver.ZGM.ZgmChip.YMF271)) continue;

                        zCnt++;
                        chip = new MDSound.MDSound.Chip();
                        chip.ID = (byte)0;//ZGMでは常に0
                        ymf271 ymf271 = new ymf271();
                        chip.type = MDSound.MDSound.enmInstrumentType.YMF271;
                        chip.Instrument = ymf271;
                        chip.Update = ymf271.Update;
                        chip.Start = ymf271.Start;
                        chip.Stop = ymf271.Stop;
                        chip.Reset = ymf271.Reset;
                        chip.SamplingRate = (UInt32)Common.SampleRate;
                        chip.Volume = setting.balance.YMF271Volume;
                        chip.Clock = (uint)zchip.defineInfo.clock;
                        chip.Option = null;
                        lstChips.Add(chip);

                        hiyorimiDeviceFlag |= 0x2;

                        log.Write(string.Format("Use OPX(#{0}) Clk:{1}"
                            , zCnt
                            , chip.Clock
                            ));

                        chipRegister.YMF271[zCnt].Use = true;
                        chipRegister.YMF271[zCnt].Model = EnmVRModel.VirtualModel;
                        chipRegister.YMF271[zCnt].Device = EnmZGMDevice.YMF271;
                    }

                    zCnt = -1;
                    foreach (Driver.ZGM.ZgmChip.ZgmChip zchip in zgmDriver.chips)
                    {
                        if (!(zchip is Driver.ZGM.ZgmChip.YM2608)) continue;

                        zCnt++;
                        chip = new MDSound.MDSound.Chip();
                        chip.ID = (byte)0;//ZGMでは常に0
                        ym2608 ym2608 = new ym2608();
                        chip.type = MDSound.MDSound.enmInstrumentType.YM2608;
                        chip.Instrument = ym2608;
                        chip.Update = ym2608.Update;
                        chip.Start = ym2608.Start;
                        chip.Stop = ym2608.Stop;
                        chip.Reset = ym2608.Reset;
                        chip.SamplingRate = 55467;// (UInt32)Common.SampleRate;
                        chip.Volume = setting.balance.YM2608Volume;
                        chip.Clock = (uint)zchip.defineInfo.clock;
                        chip.Option = null;
                        lstChips.Add(chip);

                        hiyorimiDeviceFlag |= 0x2;

                        log.Write(string.Format("Use OPNA(#{0}) Clk:{1}"
                            , zCnt
                            , chip.Clock
                            ));

                        chipRegister.YM2608[zCnt].Use = true;
                        chipRegister.YM2608[zCnt].Model = EnmVRModel.VirtualModel;
                        chipRegister.YM2608[zCnt].Device = EnmZGMDevice.YM2608;
                    }

                    zCnt = -1;
                    foreach (Driver.ZGM.ZgmChip.ZgmChip zchip in zgmDriver.chips)
                    {
                        if (!(zchip is Driver.ZGM.ZgmChip.YM2609)) continue;

                        zCnt++;
                        chip = new MDSound.MDSound.Chip();
                        chip.ID = (byte)0;//ZGMでは常に0
                        ym2609 ym2609 = new ym2609();
                        chip.type = MDSound.MDSound.enmInstrumentType.YM2609;
                        chip.Instrument = ym2609;
                        chip.Update = ym2609.Update;
                        chip.Start = ym2609.Start;
                        chip.Stop = ym2609.Stop;
                        chip.Reset = ym2609.Reset;
                        chip.SamplingRate = 55467;// (UInt32)Common.SampleRate;
                        chip.Volume = setting.balance.YM2609Volume;
                        chip.Clock = (uint)zchip.defineInfo.clock;
                        chip.Option = null;
                        lstChips.Add(chip);

                        hiyorimiDeviceFlag |= 0x2;

                        log.Write(string.Format("Use OPNA2(#{0}) Clk:{1}"
                            , zCnt
                            , chip.Clock
                            ));

                        chipRegister.YM2609[zCnt].Use = true;
                        chipRegister.YM2609[zCnt].Model = EnmVRModel.VirtualModel;
                        chipRegister.YM2609[zCnt].Device = EnmZGMDevice.YM2609;
                    }

                    zCnt = -1;
                    foreach (Driver.ZGM.ZgmChip.ZgmChip zchip in zgmDriver.chips)
                    {
                        if (!(zchip is Driver.ZGM.ZgmChip.YM2610)) continue;

                        zCnt++;
                        chip = new MDSound.MDSound.Chip();
                        chip.ID = (byte)0;//ZGMでは常に0
                        ym2610 ym2610 = new ym2610();
                        chip.type = MDSound.MDSound.enmInstrumentType.YM2610;
                        chip.Instrument = ym2610;
                        chip.Update = ym2610.Update;
                        chip.Start = ym2610.Start;
                        chip.Stop = ym2610.Stop;
                        chip.Reset = ym2610.Reset;
                        chip.SamplingRate = (UInt32)Common.SampleRate;
                        chip.Volume = setting.balance.YM2610Volume;
                        chip.Clock = (uint)zchip.defineInfo.clock & 0x7fff_ffff;
                        chip.Option = null;
                        lstChips.Add(chip);

                        hiyorimiDeviceFlag |= 0x2;

                        log.Write(string.Format("Use OPNB(#{0}) Clk:{1}"
                            , zCnt
                            , chip.Clock
                            ));

                        chipRegister.YM2610[zCnt].Use = true;
                        chipRegister.YM2610[zCnt].Model = EnmVRModel.VirtualModel;
                        chipRegister.YM2610[zCnt].Device = EnmZGMDevice.YM2610;
                    }

                    zCnt = -1;
                    foreach (Driver.ZGM.ZgmChip.ZgmChip zchip in zgmDriver.chips)
                    {
                        if (!(zchip is Driver.ZGM.ZgmChip.YM2612)) continue;

                        zCnt++;
                        chip = new MDSound.MDSound.Chip();
                        //chip.ID = (byte)0;//ZGMでは常に0
                        //ym2612 ym2612 = new ym2612();
                        //chip.type = MDSound.MDSound.enmInstrumentType.YM2612;
                        //chip.Instrument = ym2612;
                        //chip.Update = ym2612.Update;
                        //chip.Start = ym2612.Start;
                        //chip.Stop = ym2612.Stop;
                        //chip.Reset = ym2612.Reset;
                        //chip.SamplingRate = (UInt32)Common.SampleRate;
                        //chip.Volume = setting.balance.YM2612Volume;
                        //chip.Clock = (uint)zchip.defineInfo.clock;
                        //chip.Option = null;
                        //lstChips.Add(chip);

                        chip.Option = null;
                        MDSound.ym2612 ym2612 = null;
                        MDSound.ym3438 ym3438 = null;
                        MDSound.ym2612mame ym2612mame = null;

                        if (setting.YM2612Type.UseEmu)
                        {
                            if (ym2612 == null) ym2612 = new ym2612();
                            chip.type = MDSound.MDSound.enmInstrumentType.YM2612;
                            chip.Instrument = ym2612;
                            chip.Update = ym2612.Update;
                            chip.Start = ym2612.Start;
                            chip.Stop = ym2612.Stop;
                            chip.Reset = ym2612.Reset;
                            chip.Option = new object[]
                            {
                        (int)(
                            (setting.gensOption.DACHPF ? 0x01: 0x00)
                            |(setting.gensOption.SSGEG ? 0x02: 0x00)
                        )
                            };
                        }
                        else if (setting.YM2612Type.UseEmu2)
                        {
                            if (ym3438 == null) ym3438 = new ym3438();
                            chip.type = MDSound.MDSound.enmInstrumentType.YM3438;
                            chip.Instrument = ym3438;
                            chip.Update = ym3438.Update;
                            chip.Start = ym3438.Start;
                            chip.Stop = ym3438.Stop;
                            chip.Reset = ym3438.Reset;
                            switch (setting.nukedOPN2.EmuType)
                            {
                                case 0:
                                    ym3438.OPN2_SetChipType(ym3438_const.ym3438_type.discrete);
                                    break;
                                case 1:
                                    ym3438.OPN2_SetChipType(ym3438_const.ym3438_type.asic);
                                    break;
                                case 2:
                                    ym3438.OPN2_SetChipType(ym3438_const.ym3438_type.ym2612);
                                    break;
                                case 3:
                                    ym3438.OPN2_SetChipType(ym3438_const.ym3438_type.ym2612_u);
                                    break;
                                case 4:
                                    ym3438.OPN2_SetChipType(ym3438_const.ym3438_type.asic_lp);
                                    break;
                            }
                        }
                        else if (setting.YM2612Type.UseEmu3)
                        {
                            if (ym2612mame == null) ym2612mame = new ym2612mame();
                            chip.type = MDSound.MDSound.enmInstrumentType.YM2612mame;
                            chip.Instrument = ym2612mame;
                            chip.Update = ym2612mame.Update;
                            chip.Start = ym2612mame.Start;
                            chip.Stop = ym2612mame.Stop;
                            chip.Reset = ym2612mame.Reset;
                        }

                        chip.SamplingRate = (UInt32)Common.SampleRate;
                        chip.Volume = setting.balance.YM2612Volume;
                        chip.Clock = (uint)zchip.defineInfo.clock;
                        clockYM2612 = zchip.defineInfo.clock;
                        lstChips.Add(chip);

                        hiyorimiDeviceFlag |= (setting.YM2612Type.UseScci) ? 0x1 : 0x2;
                        hiyorimiDeviceFlag |= (setting.YM2612Type.UseScci && setting.YM2612Type.OnlyPCMEmulation) ? 0x2 : 0x0;

                        log.Write(string.Format("Use OPN2(#{0}) Clk:{1} NukedOPN2Type:{2}"
                            , zCnt
                            , chip.Clock
                            , setting.nukedOPN2.EmuType));

                        chipRegister.YM2612[zCnt].Use = true;
                        chipRegister.YM2612[zCnt].Model = EnmVRModel.VirtualModel;
                        chipRegister.YM2612[zCnt].Device = EnmZGMDevice.YM2612;
                    }

                    zCnt = -1;
                    foreach (Driver.ZGM.ZgmChip.ZgmChip zchip in zgmDriver.chips)
                    {
                        if (!(zchip is Driver.ZGM.ZgmChip.MidiGM)) continue;

                        zCnt++;
                        //chip = new MDSound.MDSound.Chip();
                        //chip.ID = (byte)0;//ZGMでは常に0
                        //MidiGM midiGM = new MidiGM();
                        //chip.type = MDSound.MDSound.enmInstrumentType.GeneralMIDI;
                        //chip.Instrument = midiGM;
                        //chip.Update = midiGM.Update;
                        //chip.Start = midiGM.Start;
                        //chip.Stop = midiGM.Stop;
                        //chip.Reset = midiGM.Reset;
                        //chip.SamplingRate = (UInt32)Common.SampleRate;
                        //chip.Volume = 0;
                        //chip.Clock = (uint)zchip.defineInfo.clock;
                        //chip.Option = null;
                        //lstChips.Add(chip);

                        hiyorimiDeviceFlag |= 0x2;

                        log.Write(string.Format("Use GeneralMIDI(#{0})"
                            , zCnt
                            ));

                        chipRegister.MIDI[zCnt].Use = true;
                        chipRegister.MIDI[zCnt].Model = EnmVRModel.RealModel;
                        chipRegister.MIDI[zCnt].Device = EnmZGMDevice.MIDIGM;
                    }


                    if (zCnt >= 0)
                    {
                        ReleaseAllMIDIout();
                        MakeMIDIout(setting, MidiMode);
                        chipRegister.setMIDIout(setting.midiOut.lstMidiOutInfo[MidiMode], midiOuts, midiOutsType);
                    }
                }


                if (setting.other.sinWaveGen)
                {
                    chip = new MDSound.MDSound.Chip();
                    chip.Option = null;
                    MDSound.SinWave sin = new SinWave();
                    chip.type = MDSound.MDSound.enmInstrumentType.None;
                    chip.Instrument = sin;
                    chip.Update = sin.Update;
                    chip.Start = sin.Start;
                    chip.Stop = sin.Stop;
                    chip.Reset = sin.Reset;
                    chip.SamplingRate = (UInt32)Common.SampleRate;
                    chip.Volume = setting.balance.YM2612Volume;
                    chip.Clock = 0;
                    lstChips.Add(chip);
                }


                if (hiyorimiDeviceFlag == 0x3 && hiyorimiNecessary) hiyorimiNecessary = true;
                else hiyorimiNecessary = false;


                log.Write("MDSound 初期化");

                if (mds == null)
                    mds = new MDSound.MDSound((UInt32)Common.SampleRate, samplingBuffer, lstChips.ToArray());
                else
                    mds.Init((UInt32)Common.SampleRate, samplingBuffer, lstChips.ToArray());


                log.Write("MDSound DAC control 初期化");
                mds.dacControl.init((uint)Common.SampleRate, mds, ((Driver.ZGM.zgm)driver).PCMBank);
                chipRegister.DACControl[0].Use = true;
                chipRegister.DACControl[0].Model = EnmVRModel.VirtualModel;


                log.Write("ChipRegister 初期化");
                chipRegister.SetMDSound(mds);
                chipRegister.initChipRegister(lstChips.ToArray());

                if (setting.IsManualDetect)
                {
                    RealChipManualDetect(setting);
                }
                else
                {
                    RealChipAutoDetect(setting);
                }

                log.Write("使用音源のタイプ調査 ＆ Volume設定");

                foreach (Chip c in chipRegister.CONDUCTOR)
                {
                    if (!c.Use) continue;
                    useEmu = true;
                    break;
                }

                foreach (Chip c in chipRegister.C140)
                {
                    if (!c.Use) continue;
                    if (c.Model == EnmVRModel.VirtualModel) useEmu = true;
                    if (c.Model == EnmVRModel.RealModel) useReal = true;
                    SetC140Volume(true, setting.balance.C140Volume);
                    break;
                }

                foreach (Chip c in chipRegister.HuC6280)
                {
                    if (!c.Use) continue;
                    if (c.Model == EnmVRModel.VirtualModel) useEmu = true;
                    if (c.Model == EnmVRModel.RealModel) useReal = true;
                    SetHuC6280Volume(true, setting.balance.HuC6280Volume);
                    break;
                }

                foreach (Chip c in chipRegister.K051649)
                {
                    if (!c.Use) continue;
                    if (c.Model == EnmVRModel.VirtualModel) useEmu = true;
                    if (c.Model == EnmVRModel.RealModel) useReal = true;
                    SetK051649Volume(true, setting.balance.K051649Volume);
                    break;
                }

                foreach (Chip c in chipRegister.K053260)
                {
                    if (!c.Use) continue;
                    if (c.Model == EnmVRModel.VirtualModel) useEmu = true;
                    if (c.Model == EnmVRModel.RealModel) useReal = true;
                    SetK053260Volume(true, setting.balance.K053260Volume);
                    break;
                }

                foreach (Chip c in chipRegister.QSound)
                {
                    if (!c.Use) continue;
                    if (c.Model == EnmVRModel.VirtualModel) useEmu = true;
                    if (c.Model == EnmVRModel.RealModel) useReal = true;
                    SetQSoundVolume(true, setting.balance.QSoundVolume);
                    break;
                }

                foreach (Chip c in chipRegister.RF5C164)
                {
                    if (!c.Use) continue;
                    if (c.Model == EnmVRModel.VirtualModel) useEmu = true;
                    if (c.Model == EnmVRModel.RealModel) useReal = true;
                    SetRF5C164Volume(true, setting.balance.RF5C164Volume);
                    break;
                }

                foreach (Chip c in chipRegister.DMG)
                {
                    if (!c.Use) continue;
                    if (c.Model == EnmVRModel.VirtualModel) useEmu = true;
                    if (c.Model == EnmVRModel.RealModel) useReal = true;
                    SetDMGVolume(true, setting.balance.DMGVolume);
                    break;
                }

                foreach (Chip c in chipRegister.NES)
                {
                    if (!c.Use) continue;
                    if (c.Model == EnmVRModel.VirtualModel) useEmu = true;
                    if (c.Model == EnmVRModel.RealModel) useReal = true;
                    SetNESVolume(true, setting.balance.APUVolume);
                    break;
                }

                foreach (Chip c in chipRegister.SEGAPCM)
                {
                    if (!c.Use) continue;
                    if (c.Model == EnmVRModel.VirtualModel) useEmu = true;
                    if (c.Model == EnmVRModel.RealModel) useReal = true;
                    SetSegaPCMVolume(true, setting.balance.SEGAPCMVolume);
                    break;
                }

                foreach (Chip c in chipRegister.SN76489)
                {
                    if (!c.Use) continue;
                    if (c.Model == EnmVRModel.VirtualModel) useEmu = true;
                    if (c.Model == EnmVRModel.RealModel) useReal = true;
                    SetSN76489Volume(true, setting.balance.SN76489Volume);
                    break;
                }

                foreach (Chip c in chipRegister.YM2151)
                {
                    if (!c.Use) continue;
                    if (c.Model == EnmVRModel.VirtualModel) useEmu = true;
                    if (c.Model == EnmVRModel.RealModel) useReal = true;
                    SetYM2151Volume(true, setting.balance.YM2151Volume);
                    break;
                }

                foreach (Chip c in chipRegister.YM2203)
                {
                    if (!c.Use) continue;
                    if (c.Model == EnmVRModel.VirtualModel) useEmu = true;
                    if (c.Model == EnmVRModel.RealModel) useReal = true;
                    SetYM2203FMVolume(true, setting.balance.YM2203FMVolume);
                    SetYM2203PSGVolume(true, setting.balance.YM2203PSGVolume);
                    break;
                }

                foreach (Chip c in chipRegister.YM2413)
                {
                    if (!c.Use) continue;
                    if (c.Model == EnmVRModel.VirtualModel) useEmu = true;
                    if (c.Model == EnmVRModel.RealModel) useReal = true;
                    SetYM2413Volume(true, setting.balance.YM2413Volume);
                    break;
                }

                foreach (Chip c in chipRegister.YMF278B)
                {
                    if (!c.Use) continue;
                    if (c.Model == EnmVRModel.VirtualModel) useEmu = true;
                    if (c.Model == EnmVRModel.RealModel) useReal = true;
                    SetYMF278BVolume(true, setting.balance.YMF278BVolume);
                    break;
                }

                foreach (Chip c in chipRegister.YM2608)
                {
                    if (!c.Use) continue;
                    if (c.Model == EnmVRModel.VirtualModel) useEmu = true;
                    if (c.Model == EnmVRModel.RealModel) useReal = true;
                    SetYM2608FMVolume(true, setting.balance.YM2608FMVolume);
                    SetYM2608PSGVolume(true, setting.balance.YM2608PSGVolume);
                    SetYM2608RhythmVolume(true, setting.balance.YM2608RhythmVolume);
                    SetYM2608AdpcmVolume(true, setting.balance.YM2608AdpcmVolume);
                    break;
                }

                foreach (Chip c in chipRegister.YM2609)
                {
                    if (!c.Use) continue;
                    if (c.Model == EnmVRModel.VirtualModel) useEmu = true;
                    if (c.Model == EnmVRModel.RealModel) useReal = true;
                    break;
                }

                foreach (Chip c in chipRegister.YM2610)
                {
                    if (!c.Use) continue;
                    if (c.Model == EnmVRModel.VirtualModel) useEmu = true;
                    if (c.Model == EnmVRModel.RealModel) useReal = true;
                    SetYM2610FMVolume(true, setting.balance.YM2610FMVolume);
                    SetYM2610PSGVolume(true, setting.balance.YM2610PSGVolume);
                    SetYM2610AdpcmAVolume(true, setting.balance.YM2610AdpcmAVolume);
                    SetYM2610AdpcmBVolume(true, setting.balance.YM2610AdpcmBVolume);
                    break;
                }

                foreach (Chip c in chipRegister.YM2612)
                {
                    if (!c.Use) continue;
                    if (c.Model == EnmVRModel.VirtualModel) useEmu = true;
                    if (c.Model == EnmVRModel.RealModel)
                    {
                        if (setting.YM2612Type.OnlyPCMEmulation) useEmu = true;
                        useReal = true;
                    }
                    break;
                }

                foreach (Chip c in chipRegister.MIDI)
                {
                    if (!c.Use) continue;
                    if (c.Model == EnmVRModel.VirtualModel) useEmu = true;
                    if (c.Model == EnmVRModel.RealModel) useReal = true;
                }


                log.Write("Clock 設定");

                for (int i = 0; i < chipRegister.AY8910.Count; i++) if (chipRegister.AY8910[i].Use) chipRegister.AY8910WriteClock((byte)i, (int)zgmDriver.AY8910ClockValue);
                for (int i = 0; i < chipRegister.C140.Count; i++)
                    if (chipRegister.C140[i].Use)
                    {
                        chipRegister.C140WriteClock((byte)i, (int)zgmDriver.C140ClockValue);
                        chipRegister.C140WriteType(chipRegister.C140[i], zgmDriver.C140Type);
                    }
                for (int i = 0; i < chipRegister.DMG.Count; i++) if (chipRegister.DMG[i].Use) chipRegister.DMGWriteClock((byte)i, (int)zgmDriver.DMGClockValue);
                for (int i = 0; i < chipRegister.HuC6280.Count; i++) if (chipRegister.HuC6280[i].Use) chipRegister.HuC6280WriteClock((byte)i, (int)zgmDriver.HuC6280ClockValue);
                for (int i = 0; i < chipRegister.K051649.Count; i++) if (chipRegister.K051649[i].Use) chipRegister.K051649WriteClock((byte)i, (int)zgmDriver.K051649ClockValue);
                for (int i = 0; i < chipRegister.K053260.Count; i++) if (chipRegister.K053260[i].Use) chipRegister.K053260WriteClock((byte)i, (int)zgmDriver.K053260ClockValue);
                for (int i = 0; i < chipRegister.NES.Count; i++) if (chipRegister.NES[i].Use) chipRegister.NESWriteClock((byte)i, (int)zgmDriver.NESClockValue);
                for (int i = 0; i < chipRegister.QSound.Count; i++) if (chipRegister.QSound[i].Use) chipRegister.QSoundWriteClock((byte)i, (int)zgmDriver.QSoundClockValue);
                for (int i = 0; i < chipRegister.RF5C164.Count; i++) if (chipRegister.RF5C164[i].Use) chipRegister.RF5C164WriteClock((byte)i, (int)zgmDriver.RF5C164ClockValue);
                for (int i = 0; i < chipRegister.SEGAPCM.Count; i++) if (chipRegister.SEGAPCM[i].Use) chipRegister.SEGAPCMWriteClock((byte)i, (int)zgmDriver.SEGAPCMClockValue);
                for (int i = 0; i < chipRegister.SN76489.Count; i++) if (chipRegister.SN76489[i].Use) chipRegister.SN76489WriteClock((byte)i, (int)zgmDriver.SN76489ClockValue);
                for (int i = 0; i < chipRegister.YM2151.Count; i++) if (chipRegister.YM2151[i].Use) chipRegister.YM2151WriteClock((byte)i, (int)zgmDriver.YM2151ClockValue);
                for (int i = 0; i < chipRegister.YM2203.Count; i++) if (chipRegister.YM2203[i].Use) chipRegister.YM2203WriteClock((byte)i, (int)zgmDriver.YM2203ClockValue);
                for (int i = 0; i < chipRegister.YM2413.Count; i++) if (chipRegister.YM2413[i].Use) chipRegister.YM2413WriteClock((byte)i, (int)zgmDriver.YM2413ClockValue);
                for (int i = 0; i < chipRegister.YMF278B.Count; i++) if (chipRegister.YMF278B[i].Use) chipRegister.YMF278BWriteClock((byte)i, (int)zgmDriver.YMF278BClockValue);
                for (int i = 0; i < chipRegister.YMF271.Count; i++) if (chipRegister.YMF271[i].Use) chipRegister.YMF271WriteClock((byte)i, (int)zgmDriver.YMF271ClockValue);
                for (int i = 0; i < chipRegister.YM2608.Count; i++) if (chipRegister.YM2608[i].Use) chipRegister.YM2608WriteClock((byte)i, (int)zgmDriver.YM2608ClockValue);
                for (int i = 0; i < chipRegister.YM2609.Count; i++) if (chipRegister.YM2609[i].Use) chipRegister.YM2609WriteClock((byte)i, (int)zgmDriver.YM2609ClockValue);
                for (int i = 0; i < chipRegister.YM2610.Count; i++) if (chipRegister.YM2610[i].Use) chipRegister.YM2610WriteClock((byte)i, (int)zgmDriver.YM2610ClockValue);
                for (int i = 0; i < chipRegister.YM2612.Count; i++) if (chipRegister.YM2612[i].Use) chipRegister.YM2612WriteClock((byte)i, (int)zgmDriver.YM2612ClockValue);


                log.Write("SSGVolumeセット(GIMIC)");

                int SSGVolumeFromTAG = -1;
                SSGVolumeFromTAG = GetGIMICSSGVolumeFromTAG(zgmDriver.GD3.SystemNameJ);
                if (SSGVolumeFromTAG == -1)
                    SSGVolumeFromTAG = GetGIMICSSGVolumeFromTAG(zgmDriver.GD3.SystemName);
                if (SSGVolumeFromTAG == -1)
                    SSGVolumeFromTAG = setting.balance.GimicOPNVolume;

                for (int i = 0; i < chipRegister.YM2203.Count; i++) if (chipRegister.YM2203[i].Use) chipRegister.YM2203SetSSGVolume((byte)i, SSGVolumeFromTAG);
                for (int i = 0; i < chipRegister.YM2608.Count; i++) if (chipRegister.YM2608[i].Use) chipRegister.YM2608SetSSGVolume((byte)i, SSGVolumeFromTAG);


                log.Write("参照用使用音源の登録(ZGM)");

                chipRegister.SetupDicChipCmdNo();


                log.Write("演奏停止用音源送信データ作成");

                PackData[] stopData = MakeSoftResetData();
                sm.SetStopData(stopData);


                Paused = false;
                Thread.Sleep(100);


                log.Write("初期化完了");

                return true;
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
                return false;
            }

        }

        public static bool vgmPlay(Setting setting)
        {

            try
            {

                if (vgmBuf == null || setting == null) return false;

                vgm vgmDriver = (vgm)driver;

                ResetFadeOutParam();
                useChip.Clear();

                List<MDSound.MDSound.Chip> lstChips = new List<MDSound.MDSound.Chip>();

                MDSound.MDSound.Chip chip;

                hiyorimiNecessary = setting.HiyorimiMode;

                chipLED = new ChipLEDs();

                MasterVolume = setting.balance.MasterVolume;

                log.Write("ドライバ(VGM)初期化");

                if (!driver.init(vgmBuf
                    , chipRegister

                    , new EnmChip[] { EnmChip.YM2203 }// usechip.ToArray()
                    , (uint)(Common.SampleRate * setting.LatencyEmulation / 1000)
                    , (uint)(Common.SampleRate * setting.outputDevice.WaitTime / 1000)
                    , jumpPointClock
                    ))
                    return false;

                hiyorimiNecessary = setting.HiyorimiMode;
                int hiyorimiDeviceFlag = 0;

                chipLED = new ChipLEDs();

                MasterVolume = setting.balance.MasterVolume;


                log.Write("使用チップの調査");
                {

                    chipRegister.ClearChipParam();

                    if (vgmDriver.AY8910ClockValue != 0)
                    {
                        MDSound.ay8910 ay8910 = new MDSound.ay8910();
                        for (int i = 0; i < (((vgm)driver).AY8910DualChipFlag ? 2 : 1); i++)
                        {
                            chip = new MDSound.MDSound.Chip();
                            chip.type = MDSound.MDSound.enmInstrumentType.AY8910;
                            chip.ID = (byte)i;
                            chip.Instrument = ay8910;
                            chip.Update = ay8910.Update;
                            chip.Start = ay8910.Start;
                            chip.Stop = ay8910.Stop;
                            chip.Reset = ay8910.Reset;
                            chip.SamplingRate = (UInt32)Common.SampleRate;
                            chip.Volume = setting.balance.AY8910Volume;
                            chip.Clock = (((vgm)driver).AY8910ClockValue & 0x7fffffff) / 2;
                            clockAY8910 = (int)chip.Clock;
                            chip.Option = null;
                            hiyorimiDeviceFlag |= 0x2;

                            if (i == 0)
                            {
                                chipLED.PriAY10 = 1;
                                useChip.Add(EnmChip.AY8910);
                            }
                            else
                            {
                                chipLED.SecAY10 = 1;
                                useChip.Add(EnmChip.S_AY8910);
                            }

                            log.Write(string.Format("Use AY8910({0}) Clk:{1}"
                                , (i == 0) ? "Pri" : "Sec"
                                , chip.Clock
                                ));

                            chipRegister.AY8910[i].Use = true;

                            if (chip.Instrument != null) lstChips.Add(chip);
                        }
                    }

                    if (vgmDriver.C140ClockValue != 0)
                    {
                        MDSound.c140 c140 = new MDSound.c140();
                        for (int i = 0; i < (((vgm)driver).C140DualChipFlag ? 2 : 1); i++)
                        {
                            chip = new MDSound.MDSound.Chip();
                            chip.type = MDSound.MDSound.enmInstrumentType.C140;
                            chip.ID = (byte)i;
                            chip.Instrument = c140;
                            chip.Update = c140.Update;
                            chip.Start = c140.Start;
                            chip.Stop = c140.Stop;
                            chip.Reset = c140.Reset;
                            chip.SamplingRate = (UInt32)Common.SampleRate;
                            chip.Volume = setting.balance.C140Volume;
                            chip.Clock = ((vgm)driver).C140ClockValue;
                            chip.Option = new object[1] { ((vgm)driver).C140Type };

                            hiyorimiDeviceFlag |= 0x2;

                            if (i == 0)
                            {
                                chipLED.PriC140 = 1;
                                useChip.Add(EnmChip.C140);
                            }
                            else
                            {
                                chipLED.SecC140 = 1;
                                useChip.Add(EnmChip.S_C140);
                            }

                            log.Write(string.Format("Use C140({0}) Clk:{1} Type:{2}"
                                , (i == 0) ? "Pri" : "Sec"
                                , chip.Clock
                                , chip.Option[0]
                                ));

                            chipRegister.C140[i].Use = true;

                            if (chip.Instrument != null) lstChips.Add(chip);
                        }
                    }

                    if (vgmDriver.SEGAPCMClockValue != 0)
                    {
                        MDSound.segapcm segapcm = new MDSound.segapcm();
                        for (int i = 0; i < (((vgm)driver).SEGAPCMDualChipFlag ? 2 : 1); i++)
                        {
                            chip = new MDSound.MDSound.Chip();
                            chip.type = MDSound.MDSound.enmInstrumentType.SEGAPCM;
                            chip.ID = (byte)i;
                            chip.Instrument = segapcm;
                            chip.Update = segapcm.Update;
                            chip.Start = segapcm.Start;
                            chip.Stop = segapcm.Stop;
                            chip.Reset = segapcm.Reset;
                            chip.SamplingRate = (UInt32)Common.SampleRate;
                            chip.Volume = setting.balance.SEGAPCMVolume;
                            chip.Clock = ((vgm)driver).SEGAPCMClockValue;
                            chip.Option = new object[1] { ((vgm)driver).SEGAPCMInterface };

                            hiyorimiDeviceFlag |= 0x2;

                            if (i == 0)
                            {
                                chipLED.PriSPCM = 1;
                                useChip.Add(EnmChip.SEGAPCM);
                            }
                            else
                            {
                                chipLED.SecSPCM = 1;
                                useChip.Add(EnmChip.S_SEGAPCM);
                            }

                            log.Write(string.Format("Use SEGAPCM({0}) Clk:{1} Model:{2} Type:{3}"
                                , (i == 0) ? "Pri" : "Sec"
                                , chip.Clock
                                , chipRegister.SEGAPCM[i].Model
                                , chip.Option[0]
                                ));

                            chipRegister.SEGAPCM[i].Use = true;

                            if (chip.Instrument != null) lstChips.Add(chip);
                        }
                    }

                    if (vgmDriver.SN76489ClockValue != 0)
                    {
                        MDSound.sn76489 sn76489 = null;
                        MDSound.SN76496 sn76496 = null;

                        for (int i = 0; i < (((vgm)driver).SN76489DualChipFlag ? 2 : 1); i++)
                        {
                            chip = new MDSound.MDSound.Chip();
                            chip.ID = (byte)i;

                            if ((i == 0 && setting.SN76489Type.UseEmu) || (i == 1 && setting.SN76489SType.UseEmu))
                            {
                                if (sn76489 == null) sn76489 = new MDSound.sn76489();
                                chip.type = MDSound.MDSound.enmInstrumentType.SN76489;
                                chip.Instrument = sn76489;
                                chip.Update = sn76489.Update;
                                chip.Start = sn76489.Start;
                                chip.Stop = sn76489.Stop;
                                chip.Reset = sn76489.Reset;
                            }
                            else if ((i == 0 && setting.SN76489Type.UseEmu2) || (i == 1 && setting.SN76489SType.UseEmu2))
                            {
                                if (sn76496 == null) sn76496 = new MDSound.SN76496();
                                chip.type = MDSound.MDSound.enmInstrumentType.SN76496;
                                chip.Instrument = sn76496;
                                chip.Update = sn76496.Update;
                                chip.Start = sn76496.Start;
                                chip.Stop = sn76496.Stop;
                                chip.Reset = sn76496.Reset;
                            }

                            chip.SamplingRate = (UInt32)Common.SampleRate;
                            chip.Volume = setting.balance.SN76489Volume;
                            chip.Clock = ((vgm)driver).SN76489ClockValue;
                            chip.Option = null;

                            hiyorimiDeviceFlag |= (setting.SN76489Type.UseScci) ? 0x1 : 0x2;

                            if (i == 0)
                            {
                                chipLED.PriDCSG = 1;
                                useChip.Add(EnmChip.SN76489);
                            }
                            else
                            {
                                chipLED.SecDCSG = 1;
                                useChip.Add(EnmChip.S_SN76489);
                            }

                            log.Write(string.Format("Use DCSG({0}) Clk:{1}"
                                , (i == 0) ? "Pri" : "Sec"
                                , chip.Clock
                                ));

                            chipRegister.SN76489[i].Use = true;

                            if (chip.Instrument != null) lstChips.Add(chip);

                        }
                    }

                    if (vgmDriver.YM2151ClockValue != 0)
                    {
                        MDSound.ym2151 ym2151 = null;
                        MDSound.ym2151_mame ym2151_mame = null;
                        MDSound.ym2151_x68sound ym2151_x68sound = null;

                        for (int i = 0; i < (((vgm)driver).YM2151DualChipFlag ? 2 : 1); i++)
                        {
                            chip = new MDSound.MDSound.Chip();
                            chip.ID = (byte)i;

                            if ((i == 0 && setting.YM2151Type.UseEmu) || (i == 1 && setting.YM2151SType.UseEmu))
                            {
                                if (ym2151 == null) ym2151 = new MDSound.ym2151();
                                chip.type = MDSound.MDSound.enmInstrumentType.YM2151;
                                chip.Instrument = ym2151;
                                chip.Update = ym2151.Update;
                                chip.Start = ym2151.Start;
                                chip.Stop = ym2151.Stop;
                                chip.Reset = ym2151.Reset;
                            }
                            else if ((i == 0 && setting.YM2151Type.UseEmu2) || (i == 1 && setting.YM2151SType.UseEmu2))
                            {
                                if (ym2151_mame == null) ym2151_mame = new MDSound.ym2151_mame();
                                chip.type = MDSound.MDSound.enmInstrumentType.YM2151mame;
                                chip.Instrument = ym2151_mame;
                                chip.Update = ym2151_mame.Update;
                                chip.Start = ym2151_mame.Start;
                                chip.Stop = ym2151_mame.Stop;
                                chip.Reset = ym2151_mame.Reset;
                            }
                            else if ((i == 0 && setting.YM2151Type.UseEmu3) || (i == 1 && setting.YM2151SType.UseEmu3))
                            {
                                if (ym2151_x68sound == null) ym2151_x68sound = new MDSound.ym2151_x68sound();
                                chip.type = MDSound.MDSound.enmInstrumentType.YM2151x68sound;
                                chip.Instrument = ym2151_x68sound;
                                chip.Update = ym2151_x68sound.Update;
                                chip.Start = ym2151_x68sound.Start;
                                chip.Stop = ym2151_x68sound.Stop;
                                chip.Reset = ym2151_x68sound.Reset;
                            }

                            chip.SamplingRate = (UInt32)Common.SampleRate;
                            chip.Volume = setting.balance.YM2151Volume;
                            chip.Clock = ((vgm)driver).YM2151ClockValue;
                            chip.Option = null;

                            hiyorimiDeviceFlag |= 0x2;

                            if (i == 0)
                            {
                                chipLED.PriOPM = 1;
                                useChip.Add(EnmChip.YM2151);
                            }
                            else
                            {
                                chipLED.SecOPM = 1;
                                useChip.Add(EnmChip.YM2151);
                            }

                            log.Write(string.Format("Use OPM({0}) Clk:{1} "
                                , (i == 0) ? "Pri" : "Sec"
                                , chip.Clock
                                ));

                            chipRegister.YM2151[i].Use = true;
                            if (chip.Start != null) lstChips.Add(chip);

                        }
                    }

                    if (vgmDriver.YM2203ClockValue != 0)
                    {
                        MDSound.ym2203 ym2203 = new MDSound.ym2203();
                        for (int i = 0; i < (((vgm)driver).YM2203DualChipFlag ? 2 : 1); i++)
                        {
                            chip = new MDSound.MDSound.Chip();
                            chip.type = MDSound.MDSound.enmInstrumentType.YM2203;
                            chip.ID = (byte)i;
                            chip.Instrument = ym2203;
                            chip.Update = ym2203.Update;
                            chip.Start = ym2203.Start;
                            chip.Stop = ym2203.Stop;
                            chip.Reset = ym2203.Reset;
                            chip.SamplingRate = (UInt32)Common.SampleRate;
                            chip.Volume = setting.balance.YM2203Volume;
                            chip.Clock = ((vgm)driver).YM2203ClockValue;
                            chip.Option = null;

                            hiyorimiDeviceFlag |= 0x2;

                            if (i == 0)
                            {
                                chipLED.PriOPN = 1;
                                useChip.Add(EnmChip.YM2203);
                            }
                            else
                            {
                                chipLED.SecOPN = 1;
                                useChip.Add(EnmChip.YM2203);
                            }

                            log.Write(string.Format("Use OPN({0}) Clk:{1} "
                                , (i == 0) ? "Pri" : "Sec"
                                , chip.Clock
                                ));

                            chipRegister.YM2203[i].Use = true;
                            if (chip.Instrument != null) lstChips.Add(chip);
                        }
                    }

                    if (vgmDriver.YM2413ClockValue != 0)
                    {
                        MDSound.ym2413 ym2413 = new MDSound.ym2413();

                        for (int i = 0; i < (((vgm)driver).YM2413DualChipFlag ? 2 : 1); i++)
                        {
                            chip = new MDSound.MDSound.Chip();
                            chip.type = MDSound.MDSound.enmInstrumentType.YM2413;
                            chip.ID = (byte)i;
                            chip.Instrument = ym2413;
                            chip.Update = ym2413.Update;
                            chip.Start = ym2413.Start;
                            chip.Stop = ym2413.Stop;
                            chip.Reset = ym2413.Reset;
                            chip.SamplingRate = (UInt32)Common.SampleRate;
                            chip.Volume = setting.balance.YM2413Volume;
                            chip.Clock = (((vgm)driver).YM2413ClockValue & 0x7fffffff);
                            chip.Option = null;

                            hiyorimiDeviceFlag |= 0x2;

                            if (i == 0)
                            {
                                chipLED.PriOPLL = 1;
                                useChip.Add(EnmChip.YM2413);
                            }
                            else
                            {
                                chipLED.SecOPLL = 1;
                                useChip.Add(EnmChip.S_YM2413);
                            }

                            log.Write(string.Format("Use OPLL({0}) Clk:{1} "
                                , (i == 0) ? "Pri" : "Sec"
                                , chip.Clock
                                ));

                            chipRegister.YM2413[i].Use = true;
                            if (chip.Instrument != null) lstChips.Add(chip);
                        }
                    }

                    if (vgmDriver.YM2608ClockValue != 0)
                    {
                        MDSound.ym2608 ym2608 = new MDSound.ym2608();
                        for (int i = 0; i < (vgmDriver.YM2608DualChipFlag ? 2 : 1); i++)
                        {
                            chip = new MDSound.MDSound.Chip();
                            chip.type = MDSound.MDSound.enmInstrumentType.YM2608;
                            chip.ID = (byte)i;
                            chip.Instrument = ym2608;
                            chip.Update = ym2608.Update;
                            chip.Start = ym2608.Start;
                            chip.Stop = ym2608.Stop;
                            chip.Reset = ym2608.Reset;
                            chip.SamplingRate = 55467;// (UInt32)Common.SampleRate;
                            chip.Volume = setting.balance.YM2608Volume;
                            chip.Clock = vgmDriver.YM2608ClockValue;
                            chip.Option = new object[] { Common.GetApplicationFolder() };
                            hiyorimiDeviceFlag |= 0x2;

                            if (i == 0)
                            {
                                chipLED.PriOPNA = 1;
                                useChip.Add(EnmChip.YM2608);
                            }
                            else
                            {
                                chipLED.SecOPNA = 1;
                                useChip.Add(EnmChip.S_YM2608);
                            }

                            log.Write(string.Format("Use OPNA({0}) Clk:{1}"
                                , (i == 0) ? "Pri" : "Sec"
                                , chip.Clock
                                ));

                            chipRegister.YM2608[i].Use = true;
                            if (chip.Instrument != null) lstChips.Add(chip);
                        }
                    }

                    if (vgmDriver.YM2610ClockValue != 0)
                    {
                        MDSound.ym2610 ym2610 = new MDSound.ym2610();
                        for (int i = 0; i < (((vgm)driver).YM2610DualChipFlag ? 2 : 1); i++)
                        {
                            chip = new MDSound.MDSound.Chip();
                            chip.type = MDSound.MDSound.enmInstrumentType.YM2610;
                            chip.ID = (byte)i;
                            chip.Instrument = ym2610;
                            chip.Update = ym2610.Update;
                            chip.Start = ym2610.Start;
                            chip.Stop = ym2610.Stop;
                            chip.Reset = ym2610.Reset;
                            chip.SamplingRate = (UInt32)Common.SampleRate;
                            chip.Volume = setting.balance.YM2610Volume;
                            chip.Clock = ((vgm)driver).YM2610ClockValue & 0x7fffffff;
                            chip.Option = null;

                            hiyorimiDeviceFlag |= 0x2;

                            if (i == 0)
                            {
                                chipLED.PriOPNB = 1;
                                useChip.Add(EnmChip.YM2610);
                            }
                            else
                            {
                                chipLED.SecOPNB = 1;
                                useChip.Add(EnmChip.S_YM2610);
                            }

                            log.Write(string.Format("Use OPNB({0}) Clk:{1} "
                                , (i == 0) ? "Pri" : "Sec"
                                , chip.Clock
                                ));

                            chipRegister.YM2610[i].Use = true;
                            if (chip.Instrument != null) lstChips.Add(chip);
                        }
                    }

                    if (vgmDriver.YM2612ClockValue != 0)
                    {
                        MDSound.ym2612 ym2612 = null;
                        MDSound.ym3438 ym3438 = null;
                        MDSound.ym2612mame ym2612mame = null;

                        for (int i = 0; i < (((vgm)driver).YM2612DualChipFlag ? 2 : 1); i++)
                        {
                            //MDSound.ym2612 ym2612 = new MDSound.ym2612();
                            chip = new MDSound.MDSound.Chip();
                            chip.ID = (byte)i;
                            chip.Option = null;

                            if ((i == 0 && (setting.YM2612Type.UseEmu || setting.YM2612Type.UseScci))
                                || (i == 1 && setting.YM2612SType.UseEmu || setting.YM2612SType.UseScci))
                            {
                                if (ym2612 == null) ym2612 = new ym2612();
                                chip.type = MDSound.MDSound.enmInstrumentType.YM2612;
                                chip.Instrument = ym2612;
                                chip.Update = ym2612.Update;
                                chip.Start = ym2612.Start;
                                chip.Stop = ym2612.Stop;
                                chip.Reset = ym2612.Reset;
                                chip.Option = new object[]
                                {
                                (int)(
                                    (setting.gensOption.DACHPF ? 0x01: 0x00)
                                    |(setting.gensOption.SSGEG ? 0x02: 0x00)
                                )
                                };
                            }
                            else if ((i == 0 && setting.YM2612Type.UseEmu2)
                                || (i == 1 && setting.YM2612SType.UseEmu2))
                            {
                                if (ym3438 == null) ym3438 = new ym3438();
                                chip.type = MDSound.MDSound.enmInstrumentType.YM3438;
                                chip.Instrument = ym3438;
                                chip.Update = ym3438.Update;
                                chip.Start = ym3438.Start;
                                chip.Stop = ym3438.Stop;
                                chip.Reset = ym3438.Reset;
                                switch (setting.nukedOPN2.EmuType)
                                {
                                    case 0:
                                        ym3438.OPN2_SetChipType(ym3438_const.ym3438_type.discrete);
                                        break;
                                    case 1:
                                        ym3438.OPN2_SetChipType(ym3438_const.ym3438_type.asic);
                                        break;
                                    case 2:
                                        ym3438.OPN2_SetChipType(ym3438_const.ym3438_type.ym2612);
                                        break;
                                    case 3:
                                        ym3438.OPN2_SetChipType(ym3438_const.ym3438_type.ym2612_u);
                                        break;
                                    case 4:
                                        ym3438.OPN2_SetChipType(ym3438_const.ym3438_type.asic_lp);
                                        break;
                                }
                            }
                            else if ((i == 0 && setting.YM2612Type.UseEmu3) || (i == 1 && setting.YM2612SType.UseEmu3))
                            {
                                if (ym2612mame == null) ym2612mame = new ym2612mame();
                                chip.type = MDSound.MDSound.enmInstrumentType.YM2612mame;
                                chip.Instrument = ym2612mame;
                                chip.Update = ym2612mame.Update;
                                chip.Start = ym2612mame.Start;
                                chip.Stop = ym2612mame.Stop;
                                chip.Reset = ym2612mame.Reset;
                            }

                            chip.SamplingRate = (UInt32)Common.SampleRate;
                            chip.Volume = setting.balance.YM2612Volume;
                            chip.Clock = ((vgm)driver).YM2612ClockValue;

                            hiyorimiDeviceFlag |= (setting.YM2612Type.UseScci) ? 0x1 : 0x2;
                            hiyorimiDeviceFlag |= (setting.YM2612Type.UseScci && setting.YM2612Type.OnlyPCMEmulation) ? 0x2 : 0x0;

                            if (i == 0)
                            {
                                chipLED.PriOPN2 = 1;
                                useChip.Add(EnmChip.YM2612);
                            }
                            else
                            {
                                chipLED.SecOPN2 = 1;
                                useChip.Add(EnmChip.S_YM2612);
                            }

                            log.Write(string.Format("Use OPN2({0}) Clk:{1} NukedOPN2Type:{2}"
                                , (i == 0) ? "Pri" : "Sec"
                                , chip.Clock
                                , setting.nukedOPN2.EmuType));

                            chipRegister.YM2612[i].Use = true;

                            if (chip.Instrument != null) lstChips.Add(chip);

                        }
                    }

                    if (vgmDriver.RF5C68ClockValue != 0)
                    {
                        MDSound.rf5c68 rf5c68 = new MDSound.rf5c68();

                        for (int i = 0; i < (((vgm)driver).RF5C68DualChipFlag ? 2 : 1); i++)
                        {
                            chip = new MDSound.MDSound.Chip();
                            chip.type = MDSound.MDSound.enmInstrumentType.RF5C68;
                            chip.ID = (byte)i;
                            chip.Instrument = rf5c68;
                            chip.Update = rf5c68.Update;
                            chip.Start = rf5c68.Start;
                            chip.Stop = rf5c68.Stop;
                            chip.Reset = rf5c68.Reset;
                            chip.SamplingRate = (UInt32)Common.SampleRate;
                            chip.Volume = setting.balance.RF5C68Volume;
                            chip.Clock = ((vgm)driver).RF5C68ClockValue;
                            chip.Option = null;

                            hiyorimiDeviceFlag |= 0x2;

                            if (chip.Instrument != null) lstChips.Add(chip);
                            useChip.Add(i == 0 ? EnmChip.RF5C68 : EnmChip.S_RF5C68);
                        }
                    }

                    if (vgmDriver.RF5C164ClockValue != 0)
                    {
                        MDSound.scd_pcm rf5c164 = new MDSound.scd_pcm();

                        for (int i = 0; i < (((vgm)driver).RF5C164DualChipFlag ? 2 : 1); i++)
                        {
                            chip = new MDSound.MDSound.Chip();
                            chip.type = MDSound.MDSound.enmInstrumentType.RF5C164;
                            chip.ID = (byte)i;
                            chip.Instrument = rf5c164;
                            chip.Update = rf5c164.Update;
                            chip.Start = rf5c164.Start;
                            chip.Stop = rf5c164.Stop;
                            chip.Reset = rf5c164.Reset;
                            chip.SamplingRate = (UInt32)Common.SampleRate;
                            chip.Volume = setting.balance.RF5C164Volume;
                            chip.Clock = ((vgm)driver).RF5C164ClockValue;
                            chip.Option = null;

                            hiyorimiDeviceFlag |= 0x2;

                            if (i == 0)
                            {
                                chipLED.PriRF5C = 1;
                                useChip.Add(EnmChip.RF5C164);
                            }
                            else
                            {
                                chipLED.SecRF5C = 1;
                                useChip.Add(EnmChip.S_RF5C164);
                            }

                            log.Write(string.Format("Use RF5C164({0}) Clk:{1}"
                                , (i == 0) ? "Pri" : "Sec"
                                , chip.Clock
                                ));

                            chipRegister.RF5C164[i].Use = true;
                            if (chip.Instrument != null) lstChips.Add(chip);
                        }
                    }

                    if (vgmDriver.PWMClockValue != 0)
                    {
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.PWM;
                        chip.ID = 0;
                        MDSound.pwm pwm = new MDSound.pwm();
                        chip.Instrument = pwm;
                        chip.Update = pwm.Update;
                        chip.Start = pwm.Start;
                        chip.Stop = pwm.Stop;
                        chip.Reset = pwm.Reset;
                        chip.SamplingRate = (UInt32)Common.SampleRate;
                        chip.Volume = setting.balance.PWMVolume;
                        chip.Clock = ((vgm)driver).PWMClockValue;
                        chip.Option = null;

                        hiyorimiDeviceFlag |= 0x2;

                        chipLED.PriPWM = 1;

                        if (chip.Instrument != null) lstChips.Add(chip);
                        useChip.Add(EnmChip.PWM);
                    }

                    if (vgmDriver.MultiPCMClockValue != 0)
                    {
                        MDSound.multipcm multipcm = new MDSound.multipcm();
                        for (int i = 0; i < (((vgm)driver).MultiPCMDualChipFlag ? 2 : 1); i++)
                        {
                            chip = new MDSound.MDSound.Chip();
                            chip.type = MDSound.MDSound.enmInstrumentType.MultiPCM;
                            chip.ID = (byte)i;
                            chip.Instrument = multipcm;
                            chip.Update = multipcm.Update;
                            chip.Start = multipcm.Start;
                            chip.Stop = multipcm.Stop;
                            chip.Reset = multipcm.Reset;
                            chip.SamplingRate = (UInt32)Common.SampleRate;
                            chip.Volume = setting.balance.MultiPCMVolume;
                            chip.Clock = ((vgm)driver).MultiPCMClockValue;
                            chip.Option = null;

                            hiyorimiDeviceFlag |= 0x2;

                            if (i == 0) chipLED.PriMPCM = 1;
                            else chipLED.SecMPCM = 1;

                            if (chip.Instrument != null) lstChips.Add(chip);
                            useChip.Add(i == 0 ? EnmChip.MultiPCM : EnmChip.S_MultiPCM);
                        }
                    }

                    if (vgmDriver.OKIM6258ClockValue != 0)
                    {
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.OKIM6258;
                        chip.ID = 0;
                        MDSound.okim6258 okim6258 = new MDSound.okim6258();
                        chip.Instrument = okim6258;
                        chip.Update = okim6258.Update;
                        chip.Start = okim6258.Start;
                        chip.Stop = okim6258.Stop;
                        chip.Reset = okim6258.Reset;
                        chip.SamplingRate = (UInt32)Common.SampleRate;
                        chip.Volume = setting.balance.OKIM6258Volume;
                        chip.Clock = ((vgm)driver).OKIM6258ClockValue;
                        chip.Option = new object[1] { (int)((vgm)driver).OKIM6258Type };
                        //chip.Option = new object[1] { 6 };
                        okim6258.okim6258_set_srchg_cb(0, ChangeChipSampleRate, chip);

                        hiyorimiDeviceFlag |= 0x2;

                        chipLED.PriOKI5 = 1;

                        if (chip.Instrument != null) lstChips.Add(chip);
                        useChip.Add(EnmChip.OKIM6258);
                    }

                    if (vgmDriver.OKIM6295ClockValue != 0)
                    {
                        MDSound.okim6295 okim6295 = new MDSound.okim6295();
                        for (byte i = 0; i < (((vgm)driver).OKIM6295DualChipFlag ? 2 : 1); i++)
                        {
                            chip = new MDSound.MDSound.Chip();
                            chip.type = MDSound.MDSound.enmInstrumentType.OKIM6295;
                            chip.ID = (byte)i;
                            chip.Instrument = okim6295;
                            chip.Update = okim6295.Update;
                            chip.Start = okim6295.Start;
                            chip.Stop = okim6295.Stop;
                            chip.Reset = okim6295.Reset;
                            chip.SamplingRate = (UInt32)Common.SampleRate;
                            chip.Volume = setting.balance.OKIM6295Volume;
                            chip.Clock = ((vgm)driver).OKIM6295ClockValue;
                            chip.Option = null;
                            okim6295.okim6295_set_srchg_cb(i, ChangeChipSampleRate, chip);

                            hiyorimiDeviceFlag |= 0x2;

                            if (i == 0) chipLED.PriOKI9 = 1;
                            else chipLED.SecOKI9 = 1;

                            if (chip.Instrument != null) lstChips.Add(chip);
                            useChip.Add(i == 0 ? EnmChip.OKIM6295 : EnmChip.S_OKIM6295);
                        }
                    }

                    if (vgmDriver.YM3526ClockValue != 0)
                    {
                        MDSound.ym3526 ym3526 = new MDSound.ym3526();
                        for (int i = 0; i < (((vgm)driver).YM3526DualChipFlag ? 2 : 1); i++)
                        {
                            chip = new MDSound.MDSound.Chip();
                            chip.type = MDSound.MDSound.enmInstrumentType.YM3526;
                            chip.ID = (byte)i;
                            chip.Instrument = ym3526;
                            chip.Update = ym3526.Update;
                            chip.Start = ym3526.Start;
                            chip.Stop = ym3526.Stop;
                            chip.Reset = ym3526.Reset;
                            chip.SamplingRate = (UInt32)Common.SampleRate;
                            chip.Volume = setting.balance.YM3526Volume;
                            chip.Clock = ((vgm)driver).YM3526ClockValue & 0x7fffffff;
                            chip.Option = null;

                            hiyorimiDeviceFlag |= 0x2;

                            if (i == 0)
                            {
                                chipLED.PriOPL = 1;
                                useChip.Add(EnmChip.YM3526);
                            }
                            else
                            {
                                chipLED.SecOPL = 1;
                                useChip.Add(EnmChip.S_YM3526);
                            }

                            log.Write(string.Format("Use OPL({0}) Clk:{1} "
                                , (i == 0) ? "Pri" : "Sec"
                                , chip.Clock
                                ));

                            chipRegister.YM3526[i].Use = true;
                            if (chip.Instrument != null) lstChips.Add(chip);
                        }
                    }

                    if (vgmDriver.YM3812ClockValue != 0)
                    {
                        MDSound.ym3812 ym3812 = new MDSound.ym3812();
                        for (int i = 0; i < (((vgm)driver).YM3812DualChipFlag ? 2 : 1); i++)
                        {
                            chip = new MDSound.MDSound.Chip();
                            chip.type = MDSound.MDSound.enmInstrumentType.YM3812;
                            chip.ID = (byte)i;
                            chip.Instrument = ym3812;
                            chip.Update = ym3812.Update;
                            chip.Start = ym3812.Start;
                            chip.Stop = ym3812.Stop;
                            chip.Reset = ym3812.Reset;
                            chip.SamplingRate = (UInt32)Common.SampleRate;
                            chip.Volume = setting.balance.YM3812Volume;
                            chip.Clock = ((vgm)driver).YM3812ClockValue & 0x7fffffff;
                            chip.Option = null;

                            hiyorimiDeviceFlag |= 0x2;

                            if (i == 0)
                            {
                                chipLED.PriOPL2 = 1;
                                useChip.Add(EnmChip.YM3812);
                            }
                            else
                            {
                                chipLED.SecOPL2 = 1;
                                useChip.Add(EnmChip.S_YM3812);
                            }

                            log.Write(string.Format("Use OPL2({0}) Clk:{1} "
                                , (i == 0) ? "Pri" : "Sec"
                                , chip.Clock
                                ));

                            chipRegister.YM3812[i].Use = true;
                            if (chip.Instrument != null) lstChips.Add(chip);
                        }
                    }

                    if (vgmDriver.YMF262ClockValue != 0)
                    {
                        MDSound.ymf262 ymf262 = new MDSound.ymf262();
                        for (int i = 0; i < (((vgm)driver).YMF262DualChipFlag ? 2 : 1); i++)
                        {
                            chip = new MDSound.MDSound.Chip();
                            chip.type = MDSound.MDSound.enmInstrumentType.YMF262;
                            chip.ID = (byte)i;
                            chip.Instrument = ymf262;
                            chip.Update = ymf262.Update;
                            chip.Start = ymf262.Start;
                            chip.Stop = ymf262.Stop;
                            chip.Reset = ymf262.Reset;
                            chip.SamplingRate = (UInt32)Common.SampleRate;
                            chip.Volume = setting.balance.YMF262Volume;
                            chip.Clock = ((vgm)driver).YMF262ClockValue & 0x7fffffff;
                            chip.Option = null;

                            hiyorimiDeviceFlag |= 0x2;

                            if (i == 0)
                            {
                                chipLED.PriOPL3 = 1;
                                useChip.Add(EnmChip.YMF262);
                            }
                            else
                            {
                                chipLED.SecOPL3 = 1;
                                useChip.Add(EnmChip.S_YMF262);
                            }

                            log.Write(string.Format("Use OPL3({0}) Clk:{1} "
                                , (i == 0) ? "Pri" : "Sec"
                                , chip.Clock
                                ));

                            chipRegister.YMF262[i].Use = true;
                            if (chip.Instrument != null) lstChips.Add(chip);
                        }
                    }

                    if (vgmDriver.YMF271ClockValue != 0)
                    {
                        MDSound.ymf271 ymf271 = new MDSound.ymf271();
                        for (int i = 0; i < (((vgm)driver).YMF271DualChipFlag ? 2 : 1); i++)
                        {
                            chip = new MDSound.MDSound.Chip();
                            chip.type = MDSound.MDSound.enmInstrumentType.YMF271;
                            chip.ID = (byte)i;
                            chip.Instrument = ymf271;
                            chip.Update = ymf271.Update;
                            chip.Start = ymf271.Start;
                            chip.Stop = ymf271.Stop;
                            chip.Reset = ymf271.Reset;
                            chip.SamplingRate = (UInt32)Common.SampleRate;
                            chip.Volume = setting.balance.YMF271Volume;
                            chip.Clock = ((vgm)driver).YMF271ClockValue & 0x7fffffff;
                            chip.Option = null;

                            hiyorimiDeviceFlag |= 0x2;

                            if (i == 0)
                            {
                                chipLED.PriOPX = 1;
                                useChip.Add(EnmChip.YMF271);
                            }
                            else
                            {
                                chipLED.SecOPX = 1;
                                useChip.Add(EnmChip.S_YMF271);
                            }

                            log.Write(string.Format("Use OPX({0}) Clk:{1} "
                                , (i == 0) ? "Pri" : "Sec"
                                , chip.Clock
                                ));

                            chipRegister.YMF271[i].Use = true;
                            if (chip.Instrument != null) lstChips.Add(chip);
                        }
                    }

                    if (vgmDriver.YMF278BClockValue != 0)
                    {
                        MDSound.ymf278b ymf278b = new MDSound.ymf278b();
                        for (int i = 0; i < (((vgm)driver).YMF278BDualChipFlag ? 2 : 1); i++)
                        {
                            chip = new MDSound.MDSound.Chip();
                            chip.type = MDSound.MDSound.enmInstrumentType.YMF278B;
                            chip.ID = (byte)i;
                            chip.Instrument = ymf278b;
                            chip.Update = ymf278b.Update;
                            chip.Start = ymf278b.Start;
                            chip.Stop = ymf278b.Stop;
                            chip.Reset = ymf278b.Reset;
                            chip.SamplingRate = (UInt32)Common.SampleRate;
                            chip.Volume = setting.balance.YMF278BVolume;
                            chip.Clock = ((vgm)driver).YMF278BClockValue & 0x7fffffff;
                            chip.Option = new object[] { Common.GetApplicationFolder() };

                            hiyorimiDeviceFlag |= 0x2;

                            if (i == 0) chipLED.PriOPL4 = 1;
                            else chipLED.SecOPL4 = 1;

                            if (chip.Instrument != null) lstChips.Add(chip);
                            useChip.Add(i == 0 ? EnmChip.YMF278B : EnmChip.S_YMF278B);
                        }
                    }

                    if (vgmDriver.YMZ280BClockValue != 0)
                    {
                        MDSound.ymz280b ymz280b = new MDSound.ymz280b();
                        for (int i = 0; i < (((vgm)driver).YMZ280BDualChipFlag ? 2 : 1); i++)
                        {
                            chip = new MDSound.MDSound.Chip();
                            chip.type = MDSound.MDSound.enmInstrumentType.YMZ280B;
                            chip.ID = (byte)i;
                            chip.Instrument = ymz280b;
                            chip.Update = ymz280b.Update;
                            chip.Start = ymz280b.Start;
                            chip.Stop = ymz280b.Stop;
                            chip.Reset = ymz280b.Reset;
                            chip.SamplingRate = (UInt32)Common.SampleRate;
                            chip.Volume = setting.balance.YMZ280BVolume;
                            chip.Clock = ((vgm)driver).YMZ280BClockValue & 0x7fffffff;
                            chip.Option = null;

                            hiyorimiDeviceFlag |= 0x2;

                            if (i == 0) chipLED.PriYMZ = 1;
                            else chipLED.SecYMZ = 1;

                            if (chip.Instrument != null) lstChips.Add(chip);
                            useChip.Add(i == 0 ? EnmChip.YMZ280B : EnmChip.S_YMZ280B);
                        }
                    }

                    if (vgmDriver.HuC6280ClockValue != 0)
                    {
                        MDSound.Ootake_PSG huc6280 = new MDSound.Ootake_PSG();
                        for (int i = 0; i < (((vgm)driver).HuC6280DualChipFlag ? 2 : 1); i++)
                        {
                            chip = new MDSound.MDSound.Chip();
                            chip.type = MDSound.MDSound.enmInstrumentType.HuC6280;
                            chip.ID = (byte)i;
                            chip.Instrument = huc6280;
                            chip.Update = huc6280.Update;
                            chip.Start = huc6280.Start;
                            chip.Stop = huc6280.Stop;
                            chip.Reset = huc6280.Reset;
                            chip.SamplingRate = (UInt32)Common.SampleRate;
                            chip.Volume = setting.balance.HuC6280Volume;
                            chip.Clock = (((vgm)driver).HuC6280ClockValue & 0x7fffffff);
                            chip.Option = null;

                            hiyorimiDeviceFlag |= 0x2;

                            if (i == 0)
                            {
                                chipLED.PriHuC = 1;
                                useChip.Add(EnmChip.HuC6280);
                            }
                            else
                            {
                                chipLED.SecHuC = 1;
                                useChip.Add(EnmChip.S_HuC6280);
                            }

                            log.Write(string.Format("Use HuC6280({0}) Clk:{1}"
                                , (i == 0) ? "Pri" : "Sec"
                                , chip.Clock
                                ));

                            chipRegister.HuC6280[i].Use = true;
                            if (chip.Instrument != null) lstChips.Add(chip);
                        }
                    }

                    if (vgmDriver.C352ClockValue != 0)
                    {
                        MDSound.c352 c352 = new c352();
                        for (int i = 0; i < (((vgm)driver).C352DualChipFlag ? 2 : 1); i++)
                        {
                            chip = new MDSound.MDSound.Chip();
                            chip.type = MDSound.MDSound.enmInstrumentType.C352;
                            chip.ID = (byte)i;
                            chip.Instrument = c352;
                            chip.Update = c352.Update;
                            chip.Start = c352.Start;
                            chip.Stop = c352.Stop;
                            chip.Reset = c352.Reset;
                            chip.SamplingRate = (UInt32)Common.SampleRate;
                            chip.Volume = setting.balance.C352Volume;
                            chip.Clock = (((vgm)driver).C352ClockValue & 0x7fffffff);
                            chip.Option = new object[1] { (((vgm)driver).C352ClockDivider) };
                            int divider = (ushort)((((vgm)driver).C352ClockDivider) != 0 ? (((vgm)driver).C352ClockDivider) : 288);
                            clockC352 = (int)(chip.Clock / divider);
                            c352.c352_set_options((byte)(((vgm)driver).C352ClockValue >> 31));
                            hiyorimiDeviceFlag |= 0x2;

                            if (i == 0)
                            {
                                chipLED.PriC352 = 1;
                                useChip.Add(EnmChip.C352);
                            }
                            else
                            {
                                chipLED.SecC352 = 1;
                                useChip.Add(EnmChip.S_C352);
                            }

                            log.Write(string.Format("Use C352({0}) Clk:{1} Type:{2}"
                                , (i == 0) ? "Pri" : "Sec"
                                , chip.Clock
                                , chip.Option[0]
                                ));

                            chipRegister.C352[i].Use = true;

                            if (chip.Instrument != null) lstChips.Add(chip);
                        }
                    }

                    if (vgmDriver.GA20ClockValue != 0)
                    {
                        MDSound.iremga20 ga20 = new iremga20();
                        for (int i = 0; i < (((vgm)driver).GA20DualChipFlag ? 2 : 1); i++)
                        {
                            chip = new MDSound.MDSound.Chip();
                            chip.type = MDSound.MDSound.enmInstrumentType.GA20;
                            chip.ID = (byte)i;
                            chip.Instrument = ga20;
                            chip.Update = ga20.Update;
                            chip.Start = ga20.Start;
                            chip.Stop = ga20.Stop;
                            chip.Reset = ga20.Reset;
                            chip.SamplingRate = (UInt32)Common.SampleRate;
                            chip.Volume = setting.balance.GA20Volume;
                            chip.Clock = (((vgm)driver).GA20ClockValue & 0x7fffffff);
                            chip.Option = null;
                            hiyorimiDeviceFlag |= 0x2;

                            if (i == 0) chipLED.PriGA20 = 1;
                            else chipLED.SecGA20 = 1;

                            if (chip.Instrument != null) lstChips.Add(chip);
                            useChip.Add(i == 0 ? EnmChip.GA20 : EnmChip.S_GA20);
                        }
                    }

                    if (vgmDriver.K051649ClockValue != 0)
                    {
                        MDSound.K051649 k051649 = new MDSound.K051649();

                        for (int i = 0; i < (((vgm)driver).K051649DualChipFlag ? 2 : 1); i++)
                        {
                            chip = new MDSound.MDSound.Chip();
                            chip.type = MDSound.MDSound.enmInstrumentType.K051649;
                            chip.ID = (byte)i;
                            chip.Instrument = k051649;
                            chip.Update = k051649.Update;
                            chip.Start = k051649.Start;
                            chip.Stop = k051649.Stop;
                            chip.Reset = k051649.Reset;
                            chip.SamplingRate = (UInt32)Common.SampleRate;
                            chip.Volume = setting.balance.K051649Volume;
                            chip.Clock = ((vgm)driver).K051649ClockValue;
                            clockK051649 = (int)chip.Clock;
                            chip.Option = null;

                            hiyorimiDeviceFlag |= 0x2;

                            if (i == 0)
                            {
                                chipLED.PriK051649 = 1;
                                useChip.Add(EnmChip.K051649);
                            }
                            else
                            {
                                chipLED.SecK051649 = 1;
                                useChip.Add(EnmChip.S_K051649);
                            }

                            log.Write(string.Format("Use K051649({0}) Clk:{1}"
                                , (i == 0) ? "Pri" : "Sec"
                                , chip.Clock
                                ));

                            chipRegister.K051649[i].Use = true;
                            if (chip.Instrument != null) lstChips.Add(chip);
                        }
                    }

                    if (vgmDriver.K053260ClockValue != 0)
                    {
                        MDSound.K053260 k053260 = new MDSound.K053260();

                        for (int i = 0; i < (((vgm)driver).K053260DualChipFlag ? 2 : 1); i++)
                        {
                            chip = new MDSound.MDSound.Chip();
                            chip.type = MDSound.MDSound.enmInstrumentType.K053260;
                            chip.ID = (byte)i;
                            chip.Instrument = k053260;
                            chip.Update = k053260.Update;
                            chip.Start = k053260.Start;
                            chip.Stop = k053260.Stop;
                            chip.Reset = k053260.Reset;
                            chip.SamplingRate = (UInt32)Common.SampleRate;
                            chip.Volume = setting.balance.K053260Volume;
                            chip.Clock = ((vgm)driver).K053260ClockValue;
                            chip.Option = null;

                            hiyorimiDeviceFlag |= 0x2;

                            if (i == 0)
                            {
                                chipLED.PriK053260 = 1;
                                useChip.Add(EnmChip.K053260);
                            }
                            else
                            {
                                chipLED.SecK053260 = 1;
                                useChip.Add(EnmChip.S_K053260);
                            }

                            log.Write(string.Format("Use K053260({0}) Clk:{1}"
                                , (i == 0) ? "Pri" : "Sec"
                                , chip.Clock
                                ));

                            chipRegister.K053260[i].Use = true;
                            if (chip.Instrument != null) lstChips.Add(chip);
                        }
                    }

                    if (vgmDriver.K054539ClockValue != 0)
                    {
                        MDSound.K054539 k054539 = new MDSound.K054539();

                        for (int i = 0; i < (((vgm)driver).K054539DualChipFlag ? 2 : 1); i++)
                        {
                            chip = new MDSound.MDSound.Chip();
                            chip.type = MDSound.MDSound.enmInstrumentType.K054539;
                            chip.ID = (byte)i;
                            chip.Instrument = k054539;
                            chip.Update = k054539.Update;
                            chip.Start = k054539.Start;
                            chip.Stop = k054539.Stop;
                            chip.Reset = k054539.Reset;
                            chip.SamplingRate = (UInt32)Common.SampleRate;
                            chip.Volume = setting.balance.K054539Volume;
                            chip.Clock = ((vgm)driver).K054539ClockValue;
                            chip.Option = null;
                            if (i == 0) chipLED.PriK054539 = 1;
                            else chipLED.SecK054539 = 1;

                            hiyorimiDeviceFlag |= 0x2;

                            if (chip.Instrument != null) lstChips.Add(chip);
                            useChip.Add(i == 0 ? EnmChip.K054539 : EnmChip.S_K054539);
                        }
                    }

                    if (vgmDriver.QSoundClockValue != 0)
                    {
                        //QSoundはDualChip非対応
                        MDSound.Qsound_ctr qsound = new MDSound.Qsound_ctr();
                        chip = new MDSound.MDSound.Chip();
                        chip.type = MDSound.MDSound.enmInstrumentType.QSoundCtr;
                        chip.ID = (byte)0;
                        chip.Instrument = qsound;
                        chip.Update = qsound.Update;
                        chip.Start = qsound.Start;
                        chip.Stop = qsound.Stop;
                        chip.Reset = qsound.Reset;
                        chip.SamplingRate = (UInt32)Common.SampleRate;
                        chip.Volume = setting.balance.QSoundVolume;
                        chip.Clock = (((vgm)driver).QSoundClockValue);// & 0x7fffffff);
                        chip.Option = null;

                        hiyorimiDeviceFlag |= 0x2;

                        //if (i == 0) chipLED.PriHuC = 1;
                        //else chipLED.SecHuC = 1;
                        chipLED.PriQsnd = 1;
                        useChip.Add(EnmChip.QSound);

                        log.Write(string.Format("Use QSound({0}) Clk:{1}"
                            , "Pri"
                            , chip.Clock
                            ));

                        chipRegister.QSound[0].Use = true;
                        if (chip.Instrument != null) lstChips.Add(chip);
                    }

                    if (vgmDriver.YM3526ClockValue != 0)
                    {
                        MDSound.ym3526 ym3526 = new MDSound.ym3526();

                        for (int i = 0; i < (((vgm)driver).YM3526DualChipFlag ? 2 : 1); i++)
                        {
                            chip = new MDSound.MDSound.Chip();
                            chip.type = MDSound.MDSound.enmInstrumentType.YM3526;
                            chip.ID = (byte)i;
                            chip.Instrument = ym3526;
                            chip.Update = ym3526.Update;
                            chip.Start = ym3526.Start;
                            chip.Stop = ym3526.Stop;
                            chip.Reset = ym3526.Reset;
                            chip.SamplingRate = (UInt32)Common.SampleRate;
                            chip.Volume = setting.balance.YM3526Volume;
                            chip.Clock = ((vgm)driver).YM3526ClockValue;
                            chip.Option = null;
                            if (i == 0) chipLED.PriOPL = 1;
                            else chipLED.SecOPL = 1;

                            hiyorimiDeviceFlag |= 0x2;

                            if (chip.Instrument != null) lstChips.Add(chip);
                            useChip.Add(i == 0 ? EnmChip.YM3526 : EnmChip.S_YM3526);
                        }
                    }

                    if (vgmDriver.Y8950ClockValue != 0)
                    {
                        MDSound.y8950 y8950 = new MDSound.y8950();

                        for (int i = 0; i < (((vgm)driver).Y8950DualChipFlag ? 2 : 1); i++)
                        {
                            chip = new MDSound.MDSound.Chip();
                            chip.type = MDSound.MDSound.enmInstrumentType.Y8950;
                            chip.ID = (byte)i;
                            chip.Instrument = y8950;
                            chip.Update = y8950.Update;
                            chip.Start = y8950.Start;
                            chip.Stop = y8950.Stop;
                            chip.Reset = y8950.Reset;
                            chip.SamplingRate = (UInt32)Common.SampleRate;
                            chip.Volume = setting.balance.Y8950Volume;
                            chip.Clock = ((vgm)driver).Y8950ClockValue;
                            chip.Option = null;
                            if (i == 0)
                            {
                                chipLED.PriY8950 = 1;
                                useChip.Add(EnmChip.Y8950);
                            }
                            else
                            {
                                chipLED.SecY8950 = 1;
                                useChip.Add(EnmChip.S_Y8950);
                            }

                            hiyorimiDeviceFlag |= 0x2;

                            log.Write(string.Format("Use Y8950({0}) Clk:{1}"
                                , (i == 0) ? "Pri" : "Sec"
                                , chip.Clock
                                ));

                            chipRegister.Y8950[i].Use = true;
                            if (chip.Instrument != null) lstChips.Add(chip);
                        }
                    }

                    if (vgmDriver.DMGClockValue != 0)
                    {
                        MDSound.gb dmg = new MDSound.gb();

                        for (int i = 0; i < (((vgm)driver).DMGDualChipFlag ? 2 : 1); i++)
                        {
                            chip = new MDSound.MDSound.Chip();
                            chip.type = MDSound.MDSound.enmInstrumentType.DMG;
                            chip.ID = (byte)i;
                            chip.Instrument = dmg;
                            chip.Update = dmg.Update;
                            chip.Start = dmg.Start;
                            chip.Stop = dmg.Stop;
                            chip.Reset = dmg.Reset;
                            chip.SamplingRate = (UInt32)Common.SampleRate;
                            chip.Volume = setting.balance.DMGVolume;
                            chip.Clock = ((vgm)driver).DMGClockValue;
                            chip.Option = null;
                            if (i == 0) chipLED.PriDMG = 1;
                            else chipLED.SecDMG = 1;

                            hiyorimiDeviceFlag |= 0x2;

                            log.Write(string.Format("Use DMG({0}) Clk:{1}"
                                , (i == 0) ? "Pri" : "Sec"
                                , chip.Clock
                                ));

                            chipRegister.DMG[i].Use = true;
                            if (chip.Instrument != null) lstChips.Add(chip);
                            useChip.Add(i == 0 ? EnmChip.DMG : EnmChip.S_DMG);
                        }
                    }

                    if (vgmDriver.NESClockValue != 0)
                    {

                        for (int i = 0; i < (((vgm)driver).NESDualChipFlag ? 2 : 1); i++)
                        {
                            MDSound.nes_intf nes = new MDSound.nes_intf();
                            chip = new MDSound.MDSound.Chip();
                            chip.type = MDSound.MDSound.enmInstrumentType.Nes;
                            chip.ID = (byte)i;
                            chip.Instrument = nes;
                            chip.Update = nes.Update;
                            chip.Start = nes.Start;
                            chip.Stop = nes.Stop;
                            chip.Reset = nes.Reset;
                            chip.SamplingRate = (UInt32)Common.SampleRate;
                            chip.Volume = setting.balance.APUVolume;
                            chip.Clock = ((vgm)driver).NESClockValue;
                            chip.Option = null;
                            if (i == 0) chipLED.PriNES = 1;
                            else chipLED.SecNES = 1;

                            log.Write(string.Format("Use NES({0}) Clk:{1}"
                                , (i == 0) ? "Pri" : "Sec"
                                , chip.Clock
                                ));

                            chipRegister.NES[i].Use = true;
                            lstChips.Add(chip);
                            useChip.Add(i == 0 ? EnmChip.NES : EnmChip.S_NES);

                            chip = new MDSound.MDSound.Chip();
                            chip.type = MDSound.MDSound.enmInstrumentType.DMC;
                            chip.ID = (byte)i;
                            chip.Instrument = nes;
                            //chip.Update = nes.Update;
                            chip.Start = nes.Start;
                            chip.Stop = nes.Stop;
                            chip.Reset = nes.Reset;
                            chip.SamplingRate = (UInt32)Common.SampleRate;
                            chip.Volume = setting.balance.DMCVolume;
                            chip.Clock = ((vgm)driver).NESClockValue;
                            chip.Option = null;
                            if (i == 0) chipLED.PriDMC = 1;
                            else chipLED.SecDMC = 1;

                            lstChips.Add(chip);
                            useChip.Add(i == 0 ? EnmChip.DMC : EnmChip.S_DMC);


                            chip = new MDSound.MDSound.Chip();
                            chip.type = MDSound.MDSound.enmInstrumentType.FDS;
                            chip.ID = (byte)i;
                            chip.Instrument = nes;
                            //chip.Update = nes.Update;
                            chip.Start = nes.Start;
                            chip.Stop = nes.Stop;
                            chip.Reset = nes.Reset;
                            chip.SamplingRate = (UInt32)Common.SampleRate;
                            chip.Volume = setting.balance.FDSVolume;
                            chip.Clock = ((vgm)driver).NESClockValue;
                            chip.Option = null;
                            if (i == 0) chipLED.PriFDS = 1;
                            else chipLED.SecFDS = 1;

                            if (chip.Instrument != null) lstChips.Add(chip);
                            useChip.Add(i == 0 ? EnmChip.FDS : EnmChip.S_FDS);


                            hiyorimiDeviceFlag |= 0x2;

                        }
                    }
                }


                if (hiyorimiDeviceFlag == 0x3 && hiyorimiNecessary) hiyorimiNecessary = true;
                else hiyorimiNecessary = false;

                log.Write("MDSound 初期化");

                if (mds == null)
                    mds = new MDSound.MDSound((UInt32)Common.SampleRate, samplingBuffer, lstChips.ToArray());
                else
                    mds.Init((UInt32)Common.SampleRate, samplingBuffer, lstChips.ToArray());

                log.Write("MDSound DAC control 初期化");
                mds.dacControl.init((uint)Common.SampleRate, mds, ((vgm)driver).PCMBank);
                chipRegister.DACControl[0].Use = true;
                chipRegister.DACControl[0].Model = EnmVRModel.VirtualModel;

                log.Write("ChipRegister 初期化");
                chipRegister.SetMDSound(mds);
                chipRegister.initChipRegister(lstChips.ToArray());

                if (setting.IsManualDetect)
                {
                    RealChipManualDetect(setting);
                }
                else
                {
                    RealChipAutoDetect(setting);
                }

                for (int i = 0; i < 2; i++)
                {
                    if (chipRegister.AY8910[i].Use)
                    {
                        if (chipRegister.AY8910[i].Model == EnmVRModel.VirtualModel) useEmu = true;
                        if (chipRegister.AY8910[i].Model == EnmVRModel.RealModel) useReal = true;
                    }

                    if (chipRegister.C140[i].Use)
                    {
                        if (chipRegister.C140[i].Model == EnmVRModel.VirtualModel) useEmu = true;
                        if (chipRegister.C140[i].Model == EnmVRModel.RealModel) useReal = true;
                    }

                    if (chipRegister.C352[i].Use)
                    {
                        if (chipRegister.C352[i].Model == EnmVRModel.VirtualModel) useEmu = true;
                        if (chipRegister.C352[i].Model == EnmVRModel.RealModel) useReal = true;
                    }

                    if (chipRegister.DMG[i].Use)
                    {
                        if (chipRegister.DMG[i].Model == EnmVRModel.VirtualModel) useEmu = true;
                        if (chipRegister.DMG[i].Model == EnmVRModel.RealModel) useReal = true;
                    }

                    if (chipRegister.HuC6280[i].Use)
                    {
                        if (chipRegister.HuC6280[i].Model == EnmVRModel.VirtualModel) useEmu = true;
                        if (chipRegister.HuC6280[i].Model == EnmVRModel.RealModel) useReal = true;
                    }

                    if (chipRegister.K051649[i].Use)
                    {
                        if (chipRegister.K051649[i].Model == EnmVRModel.VirtualModel) useEmu = true;
                        if (chipRegister.K051649[i].Model == EnmVRModel.RealModel) useReal = true;
                    }

                    if (chipRegister.K053260[i].Use)
                    {
                        if (chipRegister.K053260[i].Model == EnmVRModel.VirtualModel) useEmu = true;
                        if (chipRegister.K053260[i].Model == EnmVRModel.RealModel) useReal = true;
                    }

                    if (chipRegister.NES[i].Use)
                    {
                        if (chipRegister.NES[i].Model == EnmVRModel.VirtualModel) useEmu = true;
                        if (chipRegister.NES[i].Model == EnmVRModel.RealModel) useReal = true;
                    }

                    if (chipRegister.QSound[i].Use)
                    {
                        if (chipRegister.QSound[i].Model == EnmVRModel.VirtualModel) useEmu = true;
                        if (chipRegister.QSound[i].Model == EnmVRModel.RealModel) useReal = true;
                    }

                    if (chipRegister.RF5C164[i].Use)
                    {
                        if (chipRegister.RF5C164[i].Model == EnmVRModel.VirtualModel) useEmu = true;
                        if (chipRegister.RF5C164[i].Model == EnmVRModel.RealModel) useReal = true;
                    }

                    if (chipRegister.SEGAPCM[i].Use)
                    {
                        if (chipRegister.SEGAPCM[i].Model == EnmVRModel.VirtualModel) useEmu = true;
                        if (chipRegister.SEGAPCM[i].Model == EnmVRModel.RealModel) useReal = true;
                    }

                    if (chipRegister.SN76489[i].Use)
                    {
                        if (chipRegister.SN76489[i].Model == EnmVRModel.VirtualModel) useEmu = true;
                        if (chipRegister.SN76489[i].Model == EnmVRModel.RealModel) useReal = true;
                    }

                    if (chipRegister.YM2151[i].Use)
                    {
                        if (chipRegister.YM2151[i].Model == EnmVRModel.VirtualModel) useEmu = true;
                        if (chipRegister.YM2151[i].Model == EnmVRModel.RealModel)
                        {
                            if (setting.YM2151Type.SoundLocation != -1)//GIMIC以外(SCCIの場合)
                            {
                                driver.SetYM2151Hosei(chipRegister.YM2151[i], vgmDriver.YM2151ClockValue);
                            }
                            useReal = true;
                        }
                    }

                    if (chipRegister.YM2203[i].Use)
                    {
                        if (chipRegister.YM2203[i].Model == EnmVRModel.VirtualModel) useEmu = true;
                        if (chipRegister.YM2203[i].Model == EnmVRModel.RealModel) useReal = true;
                    }

                    if (chipRegister.YM2413[i].Use)
                    {
                        if (chipRegister.YM2413[i].Model == EnmVRModel.VirtualModel) useEmu = true;
                        if (chipRegister.YM2413[i].Model == EnmVRModel.RealModel) useReal = true;
                    }

                    if (chipRegister.YM2608[i].Use)
                    {
                        if (chipRegister.YM2608[i].Model == EnmVRModel.VirtualModel) useEmu = true;
                        if (chipRegister.YM2608[i].Model == EnmVRModel.RealModel)
                        {
                            if (setting.YM2608Type.OnlyPCMEmulation) useEmu = true;
                            useReal = true;
                        }
                    }

                    if (chipRegister.YM2610[i].Use)
                    {
                        if (chipRegister.YM2610[i].Model == EnmVRModel.VirtualModel) useEmu = true;
                        if (chipRegister.YM2610[i].Model == EnmVRModel.RealModel)
                        {
                            if (setting.YM2610Type.OnlyPCMEmulation) useEmu = true;
                            useReal = true;
                        }
                    }

                    if (chipRegister.YM2612[i].Use)
                    {
                        if (chipRegister.YM2612[i].Model == EnmVRModel.VirtualModel) useEmu = true;
                        if (chipRegister.YM2612[i].Model == EnmVRModel.RealModel)
                        {
                            if (setting.YM2612Type.OnlyPCMEmulation) useEmu = true;
                            useReal = true;
                        }
                    }

                    if (chipRegister.YM3526[i].Use)
                    {
                        if (chipRegister.YM3526[i].Model == EnmVRModel.VirtualModel) useEmu = true;
                        if (chipRegister.YM3526[i].Model == EnmVRModel.RealModel) useReal = true;
                    }

                    if (chipRegister.Y8950[i].Use)
                    {
                        if (chipRegister.Y8950[i].Model == EnmVRModel.VirtualModel) useEmu = true;
                        if (chipRegister.Y8950[i].Model == EnmVRModel.RealModel) useReal = true;
                    }

                    if (chipRegister.YM3812[i].Use)
                    {
                        if (chipRegister.YM3812[i].Model == EnmVRModel.VirtualModel) useEmu = true;
                        if (chipRegister.YM3812[i].Model == EnmVRModel.RealModel) useReal = true;
                    }

                    if (chipRegister.YMF262[i].Use)
                    {
                        if (chipRegister.YMF262[i].Model == EnmVRModel.VirtualModel) useEmu = true;
                        if (chipRegister.YMF262[i].Model == EnmVRModel.RealModel) useReal = true;
                    }

                    if (chipRegister.YMF278B[i].Use)
                    {
                        if (chipRegister.YMF278B[i].Model == EnmVRModel.VirtualModel) useEmu = true;
                        if (chipRegister.YMF278B[i].Model == EnmVRModel.RealModel) useReal = true;
                    }

                    if (chipRegister.YMF271[i].Use)
                    {
                        if (chipRegister.YMF271[i].Model == EnmVRModel.VirtualModel) useEmu = true;
                        if (chipRegister.YMF271[i].Model == EnmVRModel.RealModel) useReal = true;
                    }
                }

                log.Write("Volume 設定");

                if (chipRegister.YM2203[0].Use || chipRegister.YM2203[1].Use)
                {
                    SetYM2203FMVolume(true, setting.balance.YM2203FMVolume);
                    SetYM2203PSGVolume(true, setting.balance.YM2203PSGVolume);
                }

                if (chipRegister.YM2608[0].Use || chipRegister.YM2608[1].Use)
                {
                    SetYM2608FMVolume(true, setting.balance.YM2608FMVolume);
                    SetYM2608PSGVolume(true, setting.balance.YM2608PSGVolume);
                    SetYM2608RhythmVolume(true, setting.balance.YM2608RhythmVolume);
                    SetYM2608AdpcmVolume(true, setting.balance.YM2608AdpcmVolume);
                }

                if (chipRegister.YM2610[0].Use || chipRegister.YM2610[1].Use)
                {
                    SetYM2610FMVolume(true, setting.balance.YM2610FMVolume);
                    SetYM2610PSGVolume(true, setting.balance.YM2610PSGVolume);
                    SetYM2610AdpcmAVolume(true, setting.balance.YM2610AdpcmAVolume);
                    SetYM2610AdpcmBVolume(true, setting.balance.YM2610AdpcmBVolume);
                }

                log.Write("Clock 設定");

                for (int i = 0; i < 2; i++)
                {
                    if (chipRegister.AY8910[i].Use) chipRegister.AY8910WriteClock((byte)i, (int)vgmDriver.AY8910ClockValue);
                    if (chipRegister.C140[i].Use)
                    {
                        chipRegister.C140WriteClock((byte)i, (int)vgmDriver.C140ClockValue);
                        chipRegister.C140WriteType(chipRegister.C140[i], vgmDriver.C140Type);
                    }
                    if (chipRegister.DMG[i].Use) chipRegister.DMGWriteClock((byte)i, (int)vgmDriver.DMGClockValue);
                    if (chipRegister.HuC6280[i].Use) chipRegister.HuC6280WriteClock((byte)i, (int)vgmDriver.HuC6280ClockValue);
                    if (chipRegister.K051649[i].Use) chipRegister.K051649WriteClock((byte)i, (int)vgmDriver.K051649ClockValue);
                    if (chipRegister.K053260[i].Use) chipRegister.K053260WriteClock((byte)i, (int)vgmDriver.K053260ClockValue);
                    if (chipRegister.NES[i].Use) chipRegister.NESWriteClock((byte)i, (int)vgmDriver.NESClockValue);
                    if (chipRegister.QSound[i].Use) chipRegister.QSoundWriteClock((byte)i, (int)vgmDriver.QSoundClockValue);
                    if (chipRegister.RF5C164[i].Use) chipRegister.RF5C164WriteClock((byte)i, (int)vgmDriver.RF5C164ClockValue);
                    if (chipRegister.SEGAPCM[i].Use) chipRegister.SEGAPCMWriteClock((byte)i, (int)vgmDriver.SEGAPCMClockValue);
                    if (chipRegister.SN76489[i].Use) chipRegister.SN76489WriteClock((byte)i, (int)vgmDriver.SN76489ClockValue);
                    if (chipRegister.YM2151[i].Use) chipRegister.YM2151WriteClock((byte)i, (int)vgmDriver.YM2151ClockValue);
                    if (chipRegister.YM2203[i].Use) chipRegister.YM2203WriteClock((byte)i, (int)vgmDriver.YM2203ClockValue);
                    if (chipRegister.YM2413[i].Use) chipRegister.YM2413WriteClock((byte)i, (int)vgmDriver.YM2413ClockValue);
                    if (chipRegister.YMF278B[i].Use) chipRegister.YMF278BWriteClock((byte)i, (int)vgmDriver.YMF278BClockValue);
                    if (chipRegister.YMF271[i].Use) chipRegister.YMF271WriteClock((byte)i, (int)vgmDriver.YMF271ClockValue);
                    if (chipRegister.YM2608[i].Use) chipRegister.YM2608WriteClock((byte)i, (int)vgmDriver.YM2608ClockValue);
                    if (chipRegister.YM2612[i].Use) chipRegister.YM2612WriteClock((byte)i, (int)vgmDriver.YM2612ClockValue);

                }


                //
                log.Write("GIMIC向け SSGVolumeセット");
                //

                int SSGVolumeFromTAG = -1;
                SSGVolumeFromTAG = GetGIMICSSGVolumeFromTAG(vgmDriver.GD3.SystemNameJ);
                if (SSGVolumeFromTAG == -1)
                    SSGVolumeFromTAG = GetGIMICSSGVolumeFromTAG(vgmDriver.GD3.SystemName);
                if (SSGVolumeFromTAG == -1)
                    SSGVolumeFromTAG = setting.balance.GimicOPNVolume;

                for (int i = 0; i < 2; i++)
                {
                    if (chipRegister.YM2203[i].Use) chipRegister.YM2203SetSSGVolume((byte)i, SSGVolumeFromTAG);
                    if (chipRegister.YM2608[i].Use) chipRegister.YM2608SetSSGVolume((byte)i, SSGVolumeFromTAG);
                }


                PackData[] stopData = MakeSoftResetData();
                sm.SetStopData(stopData);

                Paused = false;
                //oneTimeReset = false;

                Thread.Sleep(100);

                //Stopped = false;

                log.Write("初期化完了");

                return true;
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
                return false;
            }

        }

        public static bool mubPlay(Setting setting)
        {

            try
            {

                if (mubBuf == null || setting == null) return false;

                mucomMub mubDriver = (mucomMub)driver;
                Common.playingFilePath = Path.GetDirectoryName(mubFileName);

                ResetFadeOutParam();
                useChip.Clear();
                chipRegister.ClearChipParam();

                List<MDSound.MDSound.Chip> lstChips = new List<MDSound.MDSound.Chip>();

                MDSound.MDSound.Chip chip;

                hiyorimiNecessary = setting.HiyorimiMode;

                chipLED = new ChipLEDs();

                MasterVolume = setting.balance.MasterVolume;

                MDSound.ym2608 ym2608 = new MDSound.ym2608();
                for (int i = 0; i < 2; i++)
                {
                    chip = new MDSound.MDSound.Chip();
                    chip.type = MDSound.MDSound.enmInstrumentType.YM2608;
                    chip.ID = (byte)i;
                    chip.Instrument = ym2608;
                    chip.Update = ym2608.Update;
                    chip.Start = ym2608.Start;
                    chip.Stop = ym2608.Stop;
                    chip.Reset = ym2608.Reset;
                    chip.SamplingRate = 55467;// (UInt32)Common.SampleRate;
                    chip.Volume = setting.balance.YM2608Volume;
                    chip.Clock = mubDriver.YM2608ClockValue;
                    Func<string, Stream> fn = Common.GetOPNARyhthmStream;
                    chip.Option = new object[] { fn };
                    //hiyorimiDeviceFlag |= 0x2;

                    if (i == 0)
                    {
                        chipLED.PriOPNA = 1;
                        useChip.Add(EnmChip.YM2608);
                    }
                    else
                    {
                        chipLED.SecOPNA = 1;
                        useChip.Add(EnmChip.S_YM2608);
                    }

                    log.Write(string.Format("Use OPNA({0}) Clk:{1}"
                        , (i == 0) ? "Pri" : "Sec"
                        , chip.Clock
                        ));

                    chipRegister.YM2608[i].Use = true;
                    if (chip.Instrument != null) lstChips.Add(chip);
                }

                MDSound.ym2610 ym2610 = new MDSound.ym2610();
                for (int i = 0; i < 2; i++)
                {
                    chip = new MDSound.MDSound.Chip();
                    chip.type = MDSound.MDSound.enmInstrumentType.YM2610;
                    chip.ID = (byte)i;
                    chip.Instrument = ym2610;
                    chip.Update = ym2610.Update;
                    chip.Start = ym2610.Start;
                    chip.Stop = ym2610.Stop;
                    chip.Reset = ym2610.Reset;
                    chip.SamplingRate = 55467;// (UInt32)Common.SampleRate;
                    chip.Volume = setting.balance.YM2610Volume;
                    chip.Clock = mubDriver.YM2610ClockValue;
                    chip.Option = null;
                    //chip.Option = new object[] { Common.GetApplicationFolder() };
                    //hiyorimiDeviceFlag |= 0x2;

                    if (i == 0)
                    {
                        chipLED.PriOPNB = 1;
                        useChip.Add(EnmChip.YM2610);
                    }
                    else
                    {
                        chipLED.SecOPNB = 1;
                        useChip.Add(EnmChip.S_YM2610);
                    }

                    log.Write(string.Format("Use OPNB({0}) Clk:{1}"
                        , (i == 0) ? "Pri" : "Sec"
                        , chip.Clock
                        ));

                    chipRegister.YM2610[i].Use = true;
                    if (chip.Instrument != null) lstChips.Add(chip);
                }


                if (hiyorimiNecessary) hiyorimiNecessary = true;
                else hiyorimiNecessary = false;

                log.Write("MDSound 初期化");

                if (mds == null)
                    mds = new MDSound.MDSound((UInt32)Common.SampleRate, samplingBuffer, lstChips.ToArray());
                else
                    mds.Init((UInt32)Common.SampleRate, samplingBuffer, lstChips.ToArray());

                log.Write("ChipRegister 初期化");
                chipRegister.SetMDSound(mds);
                chipRegister.initChipRegister(lstChips.ToArray());

                if (setting.IsManualDetect)
                {
                    RealChipManualDetect(setting);
                }
                else
                {
                    RealChipAutoDetect(setting);
                }

                if (
                    chipRegister.YM2608[0].Model == EnmVRModel.VirtualModel
                    || chipRegister.YM2608[1].Model == EnmVRModel.VirtualModel
                    || chipRegister.YM2610[0].Model == EnmVRModel.VirtualModel
                    || chipRegister.YM2610[1].Model == EnmVRModel.VirtualModel
                    )
                    useEmu = true;
                if (
                    chipRegister.YM2608[0].Model == EnmVRModel.RealModel
                    || chipRegister.YM2608[1].Model == EnmVRModel.RealModel
                    || chipRegister.YM2610[0].Model == EnmVRModel.RealModel
                    || chipRegister.YM2610[1].Model == EnmVRModel.RealModel
                    )
                    useReal = true;

                if (!mubDriver.init(
                    mubBuf
                    , mubWorkPath
                    , mucomManager
                    , chipRegister
                    , new EnmChip[] { EnmChip.YM2608, EnmChip.S_YM2608, EnmChip.YM2610, EnmChip.S_YM2610 }
                    , (uint)(Common.SampleRate * setting.LatencyEmulation / 1000)
                    , (uint)(Common.SampleRate * setting.outputDevice.WaitTime / 1000)
                    , mubFileName
                    )

                    ) return false;

                log.Write("Volume 設定");

                SetYM2608Volume(true, setting.balance.YM2608Volume);
                SetYM2610Volume(true, setting.balance.YM2610Volume);

                log.Write("Clock 設定");

                chipRegister.YM2608WriteClock((byte)0, (int)mubDriver.YM2608ClockValue);
                chipRegister.YM2608WriteClock((byte)1, (int)mubDriver.YM2608ClockValue);
                chipRegister.YM2610WriteClock((byte)0, (int)mubDriver.YM2610ClockValue);
                chipRegister.YM2610WriteClock((byte)1, (int)mubDriver.YM2608ClockValue);


                //Play

                PackData[] stopData = MakeSoftResetData();
                sm.SetStopData(stopData);

                Paused = false;

                //Thread.Sleep(100);

                log.Write("初期化完了");



                return true;
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
                return false;
            }

        }

        public static bool mPlay(Setting setting)
        {

            try
            {

                if (mBuf == null || setting == null) return false;

                pmdM mDriver = (pmdM)driver;

                ResetFadeOutParam();
                useChip.Clear();
                chipRegister.ClearChipParam();

                List<MDSound.MDSound.Chip> lstChips = new List<MDSound.MDSound.Chip>();

                MDSound.MDSound.Chip chip;

                hiyorimiNecessary = setting.HiyorimiMode;

                chipLED = new ChipLEDs();

                MasterVolume = setting.balance.MasterVolume;

                chip = new MDSound.MDSound.Chip();
                chip.ID = (byte)0;
                MDSound.ym2608 ym2608 = null;

                if (setting.YM2608Type.UseEmu)
                {
                    ym2608 = new ym2608();
                    chip.type = MDSound.MDSound.enmInstrumentType.YM2608;
                    chip.Instrument = ym2608;
                    chip.Update = ym2608.Update;
                    chip.Start = ym2608.Start;
                    chip.Stop = ym2608.Stop;
                    chip.Reset = ym2608.Reset;
                    chip.SamplingRate = 55467;// (UInt32)Common.SampleRate;
                    chip.Volume = setting.balance.YM2608Volume;
                    chip.Clock = (uint)mDriver.YM2608ClockValue;
                    chip.Option = null;
                    chipLED.PriOPN2 = 1;
                    lstChips.Add(chip);
                    useChip.Add(EnmChip.YM2608);
                }
                chipRegister.YM2608[0].Use = true;

                MDSound.PPZ8 ppz8 = null;
                chip = new MDSound.MDSound.Chip();
                chip.ID = (byte)0;
                ppz8 = new MDSound.PPZ8();
                chip.type = MDSound.MDSound.enmInstrumentType.PPZ8;
                chip.Instrument = ppz8;
                chip.Update = ppz8.Update;
                chip.Start = ppz8.Start;
                chip.Stop = ppz8.Stop;
                chip.Reset = ppz8.Reset;
                chip.SamplingRate = (UInt32)Common.SampleRate;
                chip.Volume = 0;// setting.balance.PPZ8Volume;
                chip.Clock = (uint)mDriver.YM2608ClockValue;
                chip.Option = null;
                chipLED.PriPPZ8 = 1;
                chipRegister.PPZ8[0].Use = true;
                lstChips.Add(chip);
                useChip.Add(EnmChip.PPZ8);


                MDSound.PPSDRV ppsdrv = null;
                chip = new MDSound.MDSound.Chip();
                chip.ID = (byte)0;
                ppsdrv = new PPSDRV();
                chip.type = MDSound.MDSound.enmInstrumentType.PPSDRV;
                chip.Instrument = ppsdrv;
                chip.Update = ppsdrv.Update;
                chip.Start = ppsdrv.Start;
                chip.Stop = ppsdrv.Stop;
                chip.Reset = ppsdrv.Reset;
                chip.SamplingRate = (UInt32)Common.SampleRate;
                chip.Volume = 0;// setting.balance.PPZ8Volume;
                chip.Clock = (uint)mDriver.YM2608ClockValue;
                chip.Option = null;
                chipLED.PriPPSDRV = 1;
                chipRegister.PPSDRV[0].Use = true;
                lstChips.Add(chip);
                useChip.Add(EnmChip.PPSDRV);


                MDSound.P86 p86 = null;
                chip = new MDSound.MDSound.Chip();
                chip.ID = (byte)0;
                p86 = new MDSound.P86();
                chip.type = MDSound.MDSound.enmInstrumentType.P86;
                chip.Instrument = p86;
                chip.Update = p86.Update;
                chip.Start = p86.Start;
                chip.Stop = p86.Stop;
                chip.Reset = p86.Reset;
                chip.SamplingRate = (UInt32)Common.SampleRate;
                chip.Volume = 0;// setting.balance.P86Volume;
                chip.Clock = (uint)mDriver.YM2608ClockValue;
                chip.Option = null;
                chipLED.PriP86 = 1;
                chipRegister.P86[0].Use = true;
                lstChips.Add(chip);
                useChip.Add(EnmChip.P86);



                if (hiyorimiNecessary) hiyorimiNecessary = true;
                else hiyorimiNecessary = false;

                log.Write("MDSound 初期化");

                if (mds == null)
                    mds = new MDSound.MDSound((UInt32)Common.SampleRate, samplingBuffer, lstChips.ToArray());
                else
                    mds.Init((UInt32)Common.SampleRate, samplingBuffer, lstChips.ToArray());

                log.Write("ChipRegister 初期化");
                chipRegister.SetMDSound(mds);
                chipRegister.initChipRegister(lstChips.ToArray());

                if (setting.IsManualDetect)
                {
                    RealChipManualDetect(setting);
                }
                else
                {
                    RealChipAutoDetect(setting);
                }

                bool isGIMICOPNA = false;
                useEmu = true;
                if (chipRegister.YM2608[0].Model == EnmVRModel.VirtualModel) useEmu = true;
                if (chipRegister.YM2608[0].Model == EnmVRModel.RealModel)
                {
                    useReal = true;
                    isGIMICOPNA = chipRegister.YM2608GetGIMICType(0) == Nc86ctl.ChipType.CHIP_OPNA;
                }

                log.Write("Volume(emu) 設定");

                SetYM2608Volume(true, setting.balance.YM2608Volume);

                log.Write("Clock 設定");

                chipRegister.YM2608WriteClock((byte)0, (int)mDriver.YM2608ClockValue);

                log.Write("Clock 設定");

                if (!mDriver.init(mBuf, mWorkPath, PMDManager, chipRegister, new EnmChip[] { EnmChip.YM2608 }
                    , (uint)(Common.SampleRate * setting.LatencyEmulation / 1000)
                    , (uint)(Common.SampleRate * setting.outputDevice.WaitTime / 1000)
                    , mFileName
                    , isGIMICOPNA
                    )

                    ) return false;

                //driverが決定したボリュームを反映する
                SetYM2608FMVolume(true, mDriver.YM2608_FMVolume);
                SetYM2608PSGVolume(true, mDriver.YM2608_SSGVolume);
                SetYM2608RhythmVolume(true, mDriver.YM2608_RhythmVolume);
                SetYM2608AdpcmVolume(true, mDriver.YM2608_AdpcmVolume);
                if (isGIMICOPNA)
                {
                    SetGimicOPNAVolume(true, mDriver.GIMIC_SSGVolume);
                    chipRegister.setYM2608SSGVolume(0, mDriver.GIMIC_SSGVolume, EnmVRModel.RealModel);
                    //System.Threading.Thread.Sleep(500);
                }

                if (chipRegister.YM2608[0].Model == EnmVRModel.RealModel)
                {
                    object opnaPcmData = mDriver.GetOPNAPCMData();
                    if (opnaPcmData != null)
                    {
                        RealChipAction(null, 0, chipRegister.YM2608[0], EnmDataType.Block, -1, -1, opnaPcmData);
                        Thread.Sleep(100);
                    }
                }

                //Play

                PackData[] stopData = MakeSoftResetData();
                sm.SetStopData(stopData);

                Paused = false;

                //Thread.Sleep(100);

                log.Write("初期化完了");



                return true;
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
                return false;
            }

        }

        public static bool mdrPlay(Setting setting)
        {

            try
            {

                if (mdrBuf == null || setting == null) return false;

                moonDriverM mdrDriver = (moonDriverM)driver;

                ResetFadeOutParam();
                useChip.Clear();
                chipRegister.ClearChipParam();

                List<MDSound.MDSound.Chip> lstChips = new List<MDSound.MDSound.Chip>();

                MDSound.MDSound.Chip chip;

                hiyorimiNecessary = setting.HiyorimiMode;

                chipLED = new ChipLEDs();

                MasterVolume = setting.balance.MasterVolume;

                chip = new MDSound.MDSound.Chip();
                chip.ID = (byte)0;
                MDSound.ymf278b ymf278b = null;

                if (setting.YMF278BType.UseEmu)
                {
                    ymf278b = new ymf278b();
                    chip.type = MDSound.MDSound.enmInstrumentType.YMF278B;
                    chip.Instrument = ymf278b;
                    chip.Update = ymf278b.Update;
                    chip.Start = ymf278b.Start;
                    chip.Stop = ymf278b.Stop;
                    chip.Reset = ymf278b.Reset;
                    chip.SamplingRate = (UInt32)Common.SampleRate;
                    chip.Volume = setting.balance.YMF278BVolume;
                    chip.Clock = (uint)mdrDriver.YMF278BClockValue;
                    chip.Option = null;
                    chipLED.PriOPL4 = 1;
                    lstChips.Add(chip);
                    useChip.Add(EnmChip.YMF278B);
                }
                chipRegister.YMF278B[0].Use = true;

                if (hiyorimiNecessary) hiyorimiNecessary = true;
                else hiyorimiNecessary = false;

                log.Write("MDSound 初期化");

                if (mds == null)
                    mds = new MDSound.MDSound((UInt32)Common.SampleRate, samplingBuffer, lstChips.ToArray());
                else
                    mds.Init((UInt32)Common.SampleRate, samplingBuffer, lstChips.ToArray());

                log.Write("ChipRegister 初期化");
                chipRegister.SetMDSound(mds);
                chipRegister.initChipRegister(lstChips.ToArray());

                if (setting.IsManualDetect)
                {
                    RealChipManualDetect(setting);
                }
                else
                {
                    RealChipAutoDetect(setting);
                }

                //bool isGIMICOPNA = false;
                useEmu = true;
                if (chipRegister.YMF278B[0].Model == EnmVRModel.VirtualModel) useEmu = true;
                if (chipRegister.YMF278B[0].Model == EnmVRModel.RealModel)
                {
                    useReal = true;
                    //isGIMICOPNA = chipRegister.YM2608GetGIMICType(0) == Nc86ctl.ChipType.CHIP_OPNA;
                }

                log.Write("Volume(emu) 設定");
                SetYMF278BVolume(true, setting.balance.YMF278BVolume);

                //log.Write("Clock 設定");
                //chipRegister.YMF278BWriteClock((byte)0, (int)mdrDriver.YMF278BClockValue);

                log.Write("Clock 設定");
                if (!mdrDriver.init(mdrBuf, mdrWorkPath, MoonDriverManager, chipRegister, new EnmChip[] { EnmChip.YMF278B }
                    , (uint)(Common.SampleRate * setting.LatencyEmulation / 1000)
                    , (uint)(Common.SampleRate * setting.outputDevice.WaitTime / 1000)
                    , mdrFileName
                    //, isGIMICOPNA
                    )

                    ) return false;

                //実チップの場合にPCMデータの転送が必要な時
                if (chipRegister.YMF278B[0].Model == EnmVRModel.RealModel)
                {
                    //object opnaPcmData = mdrDriver.GetOPNAPCMData();
                    //if (opnaPcmData != null)
                    //{
                    //    RealChipAction(null, 0, chipRegister.YM2608[0], EnmDataType.Block, -1, -1, opnaPcmData);
                    //    Thread.Sleep(100);
                    //}
                }

                //Play

                PackData[] stopData = MakeSoftResetData();
                sm.SetStopData(stopData);

                Paused = false;

                //Thread.Sleep(100);

                log.Write("初期化完了");

                return true;
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
                return false;
            }

        }

        private static void MakeMIDIout(Setting setting, int m)
        {
            if (setting.midiOut.lstMidiOutInfo == null || setting.midiOut.lstMidiOutInfo.Count < 1) return;
            if (setting.midiOut.lstMidiOutInfo[m] == null || setting.midiOut.lstMidiOutInfo[m].Length < 1) return;

            for (int i = 0; i < setting.midiOut.lstMidiOutInfo[m].Length; i++)
            {
                int n = -1;
                int t = 0;
                NAudio.Midi.MidiOut mo = null;

                for (int j = 0; j < NAudio.Midi.MidiOut.NumberOfDevices; j++)
                {
                    if (setting.midiOut.lstMidiOutInfo[m][i].name != NAudio.Midi.MidiOut.DeviceInfo(j).ProductName) continue;

                    n = j;
                    t = setting.midiOut.lstMidiOutInfo[m][i].type;
                    break;
                }

                if (n != -1)
                {
                    try
                    {
                        mo = new NAudio.Midi.MidiOut(n);
                    }
                    catch
                    {
                        mo = null;
                    }
                }

                if (mo != null)
                {
                    midiOuts.Add(mo);
                    midiOutsType.Add(t);
                }
            }
        }

        private static void ReleaseAllMIDIout()
        {
            if (midiOuts.Count > 0)
            {
                for (int i = 0; i < midiOuts.Count; i++)
                {
                    if (midiOuts[i] != null)
                    {
                        try
                        {
                            //resetできない機種もある?
                            midiOuts[i].Reset();
                        }
                        catch { }
                        midiOuts[i].Close();
                        midiOuts[i] = null;
                    }
                }
                midiOuts.Clear();
                midiOutsType.Clear();
            }

        }

        private static int GetGIMICSSGVolumeFromTAG(string tag)
        {
            if (tag.IndexOf("9801") > 0) return 31;
            if (tag.IndexOf("PC-98") > 0) return 31;
            if (tag.IndexOf("PC98") > 0) return 31;

            if (tag.IndexOf("8801") > 0) return 63;
            if (tag.IndexOf("PC-88") > 0) return 63;
            if (tag.IndexOf("PC88") > 0) return 63;

            return -1;
        }


        private static void ResetFadeOutParam()
        {
            fadeoutCounter = 1.0;
            fadeoutCounterEmu = 1.0;
            fadeoutCounterDelta = 0.000004;
            vgmSpeed = 1;

        }

        public static void ChangeChipSampleRate(MDSound.MDSound.Chip chip, int NewSmplRate)
        {
            MDSound.MDSound.Chip CAA = chip;

            if (CAA.SamplingRate == NewSmplRate)
                return;

            // quick and dirty hack to make sample rate changes work
            CAA.SamplingRate = (uint)NewSmplRate;
            if (CAA.SamplingRate < Common.SampleRate)//SampleRate)
                CAA.Resampler = 0x01;
            else if (CAA.SamplingRate == Common.SampleRate)//SampleRate)
                CAA.Resampler = 0x02;
            else if (CAA.SamplingRate > Common.SampleRate)//SampleRate)
                CAA.Resampler = 0x03;
            CAA.SmpP = 1;
            CAA.SmpNext -= CAA.SmpLast;
            CAA.SmpLast = 0x00;

            return;
        }

        public static void FF()
        {
            vgmSpeed = (vgmSpeed == 1) ? 4 : 1;
            sm.SetSpeed(vgmSpeed);
        }

        public static void Slow()
        {
            vgmSpeed = (vgmSpeed == 1) ? 0.25 : 1;
            sm.SetSpeed(vgmSpeed);
        }

        public static void ResetSlow()
        {
            vgmSpeed = 1;
            driver.vgmSpeed = vgmSpeed;
        }

        public static void Pause()
        {

            try
            {
                Paused = !Paused;
                if (Paused)
                {
                    vgmSpeed = 0.0;
                    sm.SetSpeed(0.0);
                }
                else
                {
                    vgmSpeed = 1.0;
                    sm.SetSpeed(1.0);
                }
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }

        }

        public static bool isPaused
        {
            get
            {
                return Paused;
            }
        }

        public static bool isStopped
        {
            get
            {
                return Stopped;
            }
        }

        public static void StepPlay(int Step)
        {
            StepCounter = Step;
        }

        public static void Fadeout()
        {
            sm.SetFadeOut();
            //vgmFadeout = true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode">停止させたいモードを指定</param>
        public static void Stop(SendMode mode)
        {
            switch (mode)
            {
                case SendMode.Both:
                    sm.RequestStop();
                    while (sm.IsRunningAsync())
                    {
                        Thread.Sleep(1);
                        System.Windows.Forms.Application.DoEvents();
                    }
                    sm.ResetMode(SendMode.Both);
                    break;
                case SendMode.MML:
                    //鍵盤が表示されている場合の「停止」はデータ演奏モード(SendMode.MML)のみ停止する
                    sm.ResetMode(SendMode.MML);

                    //データメーカーを止めて、コールバックが来ないようにする
                    sm.RequestStopAtDataMaker();
                    while (sm.IsRunningAtDataMaker())
                    {
                        Thread.Sleep(1);
                        System.Windows.Forms.Application.DoEvents();
                    }

                    PackData[] keyOffData = MakeKeyOffData();
                    sm.SetStopData(keyOffData);

                    //DataSenderに溜まっているであろう演奏データをクリアし、演奏を打ち切る
                    sm.ClearData();
                    break;
            }

        }

        public static void Close()
        {
            try
            {

                Stop(0);
                NAudioWrap.Stop();

                sm.Release();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
        }

        public static long GetCounter()
        {
            if (driver == null) return -1;

            return sm.GetSeqCounter();
        }

        public static long GetTotalCounter()
        {
            if (driver == null) return -1;

            return driver.TotalCounter;
        }

        public static long GetDriverCounter()
        {
            if (driver == null) return -1;


            if (driver is vgm || driver is Driver.ZGM.zgm)
            {
                return sm.GetDataSenderBufferCounter();
            }
            else return 0;
        }

        public static long GetLoopCounter()
        {
            if (driver == null) return -1;

            return driver.LoopCounter;
        }

        public static byte[] GetChipStatus()
        {
            chips[0] = chipRegister.chipLED.PriOPN;
            chipRegister.chipLED.PriOPN = chipLED.PriOPN;
            chips[1] = chipRegister.chipLED.PriOPN2;
            chipRegister.chipLED.PriOPN2 = chipLED.PriOPN2;
            chips[2] = chipRegister.chipLED.PriOPNA;
            chipRegister.chipLED.PriOPNA = chipLED.PriOPNA;
            chips[3] = chipRegister.chipLED.PriOPNB;
            chipRegister.chipLED.PriOPNB = chipLED.PriOPNB;

            chips[4] = chipRegister.chipLED.PriOPM;
            chipRegister.chipLED.PriOPM = chipLED.PriOPM;
            chips[5] = chipRegister.chipLED.PriDCSG;
            chipRegister.chipLED.PriDCSG = chipLED.PriDCSG;
            chips[6] = chipRegister.chipLED.PriRF5C;
            chipRegister.chipLED.PriRF5C = chipLED.PriRF5C;
            chips[7] = chipRegister.chipLED.PriPWM;
            chipRegister.chipLED.PriPWM = chipLED.PriPWM;

            chips[8] = chipRegister.chipLED.PriOKI5;
            chipRegister.chipLED.PriOKI5 = chipLED.PriOKI5;
            chips[9] = chipRegister.chipLED.PriOKI9;
            chipRegister.chipLED.PriOKI9 = chipLED.PriOKI9;
            chips[10] = chipRegister.chipLED.PriC140;
            chipRegister.chipLED.PriC140 = chipLED.PriC140;
            chips[11] = chipRegister.chipLED.PriSPCM;
            chipRegister.chipLED.PriSPCM = chipLED.PriSPCM;

            chips[12] = chipRegister.chipLED.PriAY10;
            chipRegister.chipLED.PriAY10 = chipLED.PriAY10;
            chips[13] = chipRegister.chipLED.PriOPLL;
            chipRegister.chipLED.PriOPLL = chipLED.PriOPLL;
            chips[14] = chipRegister.chipLED.PriHuC;
            chipRegister.chipLED.PriHuC = chipLED.PriHuC;
            chips[15] = chipRegister.chipLED.PriC352;
            chipRegister.chipLED.PriC352 = chipLED.PriC352;
            chips[16] = chipRegister.chipLED.PriK054539;
            chipRegister.chipLED.PriK054539 = chipLED.PriK054539;


            chips[128 + 0] = chipRegister.chipLED.SecOPN;
            chipRegister.chipLED.SecOPN = chipLED.SecOPN;
            chips[128 + 1] = chipRegister.chipLED.SecOPN2;
            chipRegister.chipLED.SecOPN2 = chipLED.SecOPN2;
            chips[128 + 2] = chipRegister.chipLED.SecOPNA;
            chipRegister.chipLED.SecOPNA = chipLED.SecOPNA;
            chips[128 + 3] = chipRegister.chipLED.SecOPNB;
            chipRegister.chipLED.SecOPNB = chipLED.SecOPNB;

            chips[128 + 4] = chipRegister.chipLED.SecOPM;
            chipRegister.chipLED.SecOPM = chipLED.SecOPM;
            chips[128 + 5] = chipRegister.chipLED.SecDCSG;
            chipRegister.chipLED.SecDCSG = chipLED.SecDCSG;
            chips[128 + 6] = chipRegister.chipLED.SecRF5C;
            chipRegister.chipLED.SecRF5C = chipLED.SecRF5C;
            chips[128 + 7] = chipRegister.chipLED.SecPWM;
            chipRegister.chipLED.SecPWM = chipLED.SecPWM;

            chips[128 + 8] = chipRegister.chipLED.SecOKI5;
            chipRegister.chipLED.SecOKI5 = chipLED.SecOKI5;
            chips[128 + 9] = chipRegister.chipLED.SecOKI9;
            chipRegister.chipLED.SecOKI9 = chipLED.SecOKI9;
            chips[128 + 10] = chipRegister.chipLED.SecC140;
            chipRegister.chipLED.SecC140 = chipLED.SecC140;
            chips[128 + 11] = chipRegister.chipLED.SecSPCM;
            chipRegister.chipLED.SecSPCM = chipLED.SecSPCM;

            chips[128 + 12] = chipRegister.chipLED.SecAY10;
            chipRegister.chipLED.SecAY10 = chipLED.SecAY10;
            chips[128 + 13] = chipRegister.chipLED.SecOPLL;
            chipRegister.chipLED.SecOPLL = chipLED.SecOPLL;
            chips[128 + 14] = chipRegister.chipLED.SecHuC;
            chipRegister.chipLED.SecHuC = chipLED.SecHuC;
            chips[128 + 15] = chipRegister.chipLED.SecC352;
            chipRegister.chipLED.SecC352 = chipLED.SecC352;
            chips[128 + 16] = chipRegister.chipLED.SecK054539;
            chipRegister.chipLED.SecK054539 = chipLED.SecK054539;


            return chips;
        }

        public static void updateVol()
        {
            chipRegister.updateVol();
        }

        public static uint GetVgmCurLoopCounter()
        {
            uint cnt = 0;

            if (driver != null)
            {
                cnt = driver.vgmCurLoop;
            }

            return cnt;
        }

        public static bool GetVGMStopped()
        {
            bool v;

            v = driver == null ? true : driver.Stopped;
            return v;
        }

        public static bool GetIsDataBlock()
        {
            if (sm == null) return false;
            return sm.GetInterrupt();
        }



        private static void NaudioWrap_PlaybackStopped(object sender, NAudio.Wave.StoppedEventArgs e)
        {
            if (e.Exception != null)
            {
                System.Windows.Forms.MessageBox.Show(
                    string.Format("デバイスが何らかの原因で停止しました。\r\nメッセージ:\r\n{0}", e.Exception.Message)
                    , "エラー"
                    , System.Windows.Forms.MessageBoxButtons.OK
                    , System.Windows.Forms.MessageBoxIcon.Error);
                flgReinit = true;

                try
                {
                    NAudioWrap.Stop();
                }
                catch (Exception ex)
                {
                    log.ForcedWrite(ex);
                }

            }
            else
            {
                try
                {
                    Stop(0);
                }
                catch { }
            }
        }

        private static void softReset(long counter)
        {
            for (int i = 0; i < chipRegister.CONDUCTOR.Count; i++) if (chipRegister.CONDUCTOR[i].Use) chipRegister.ConductorSoftReset(counter, i);
            for (int i = 0; i < chipRegister.AY8910.Count; i++) if (chipRegister.AY8910[i].Use) chipRegister.AY8910SoftReset(counter, i);
            for (int i = 0; i < chipRegister.C140.Count; i++) if (chipRegister.C140[i].Use) chipRegister.C140SoftReset(counter, i);
            for (int i = 0; i < chipRegister.DMG.Count; i++) if (chipRegister.DMG[i].Use) chipRegister.DMGSoftReset(counter, i);
            for (int i = 0; i < chipRegister.HuC6280.Count; i++) if (chipRegister.HuC6280[i].Use) chipRegister.HuC6280SoftReset(counter, i);
            for (int i = 0; i < chipRegister.K051649.Count; i++) if (chipRegister.K051649[i].Use) chipRegister.K051649SoftReset(counter, i);
            for (int i = 0; i < chipRegister.K053260.Count; i++) if (chipRegister.K053260[i].Use) chipRegister.K053260SoftReset(counter, i);
            for (int i = 0; i < chipRegister.NES.Count; i++) if (chipRegister.NES[i].Use) chipRegister.NESSoftReset(counter, i);
            for (int i = 0; i < chipRegister.PPZ8.Count; i++) if (chipRegister.PPZ8[i].Use) chipRegister.PPZ8SoftReset(counter, i);
            for (int i = 0; i < chipRegister.PPSDRV.Count; i++) if (chipRegister.PPSDRV[i].Use) chipRegister.PPSDRVSoftReset(counter, i);
            for (int i = 0; i < chipRegister.P86.Count; i++) if (chipRegister.P86[i].Use) chipRegister.P86SoftReset(counter, i);
            for (int i = 0; i < chipRegister.QSound.Count; i++) if (chipRegister.QSound[i].Use) chipRegister.QSoundSoftReset(counter, i);
            for (int i = 0; i < chipRegister.RF5C164.Count; i++) if (chipRegister.RF5C164[i].Use) chipRegister.RF5C164SoftReset(counter, i);
            for (int i = 0; i < chipRegister.SEGAPCM.Count; i++) if (chipRegister.SEGAPCM[i].Use) chipRegister.SEGAPCMSoftReset(counter, i);
            for (int i = 0; i < chipRegister.SN76489.Count; i++) if (chipRegister.SN76489[i].Use) chipRegister.SN76489SoftReset(counter, i);
            for (int i = 0; i < chipRegister.YM2151.Count; i++) if (chipRegister.YM2151[i].Use) chipRegister.YM2151SoftReset(counter, i);
            for (int i = 0; i < chipRegister.YM2203.Count; i++) if (chipRegister.YM2203[i].Use) chipRegister.YM2203SoftReset(counter, i);
            for (int i = 0; i < chipRegister.YM2413.Count; i++) if (chipRegister.YM2413[i].Use) chipRegister.YM2413SoftReset(counter, i);
            for (int i = 0; i < chipRegister.YM3526.Count; i++) if (chipRegister.YM3526[i].Use) chipRegister.YM3526SoftReset(counter, i);
            for (int i = 0; i < chipRegister.Y8950.Count; i++) if (chipRegister.Y8950[i].Use) chipRegister.Y8950SoftReset(counter, i);
            for (int i = 0; i < chipRegister.YM3812.Count; i++) if (chipRegister.YM3812[i].Use) chipRegister.YM3812SoftReset(counter, i);
            for (int i = 0; i < chipRegister.YMF262.Count; i++) if (chipRegister.YMF262[i].Use) chipRegister.YMF262SoftReset(counter, i);
            for (int i = 0; i < chipRegister.YMF278B.Count; i++) if (chipRegister.YMF278B[i].Use) chipRegister.YMF278BSoftReset(counter, i);
            for (int i = 0; i < chipRegister.YMF271.Count; i++) if (chipRegister.YMF271[i].Use) chipRegister.YMF271SoftReset(counter, i);
            for (int i = 0; i < chipRegister.YM2608.Count; i++) if (chipRegister.YM2608[i].Use) chipRegister.YM2608SoftReset(counter, i);
            for (int i = 0; i < chipRegister.YM2609.Count; i++) if (chipRegister.YM2609[i].Use) chipRegister.YM2609SoftReset(counter, i);
            for (int i = 0; i < chipRegister.YM2610.Count; i++) if (chipRegister.YM2610[i].Use) chipRegister.YM2610SoftReset(counter, i);
            for (int i = 0; i < chipRegister.YM2612.Count; i++) if (chipRegister.YM2612[i].Use) chipRegister.YM2612SoftReset(counter, i);
            for (int i = 0; i < chipRegister.MIDI.Count; i++) if (chipRegister.MIDI[i].Use) chipRegister.MIDISoftReset(counter, i);
        }

        private static PackData[] MakeSoftResetData()
        {
            List<PackData> data = new List<PackData>();
            for (int i = 0; i < chipRegister.CONDUCTOR.Count; i++) if (chipRegister.CONDUCTOR[i].Use) data.AddRange(chipRegister.ConductorMakeSoftReset(i));
            for (int i = 0; i < chipRegister.AY8910.Count; i++) if (chipRegister.AY8910[i].Use) data.AddRange(chipRegister.AY8910MakeSoftReset(i));
            for (int i = 0; i < chipRegister.C140.Count; i++) if (chipRegister.C140[i].Use) data.AddRange(chipRegister.C140MakeSoftReset(i));
            for (int i = 0; i < chipRegister.DMG.Count; i++) if (chipRegister.DMG[i].Use) data.AddRange(chipRegister.DMGMakeSoftReset(i));
            for (int i = 0; i < chipRegister.HuC6280.Count; i++) if (chipRegister.HuC6280[i].Use) data.AddRange(chipRegister.HuC6280MakeSoftReset(i));
            for (int i = 0; i < chipRegister.K051649.Count; i++) if (chipRegister.K051649[i].Use) data.AddRange(chipRegister.K051649MakeSoftReset(i));
            for (int i = 0; i < chipRegister.K053260.Count; i++) if (chipRegister.K053260[i].Use) data.AddRange(chipRegister.K053260MakeSoftReset(i));
            for (int i = 0; i < chipRegister.NES.Count; i++) if (chipRegister.NES[i].Use) data.AddRange(chipRegister.NESMakeSoftReset(i));
            for (int i = 0; i < chipRegister.PPZ8.Count; i++) if (chipRegister.PPZ8[i].Use) data.AddRange(chipRegister.PPZ8MakeSoftReset(i));
            for (int i = 0; i < chipRegister.PPSDRV.Count; i++) if (chipRegister.PPSDRV[i].Use) data.AddRange(chipRegister.PPSDRVMakeSoftReset(i));
            for (int i = 0; i < chipRegister.P86.Count; i++) if (chipRegister.P86[i].Use) data.AddRange(chipRegister.P86MakeSoftReset(i));
            for (int i = 0; i < chipRegister.QSound.Count; i++) if (chipRegister.QSound[i].Use) data.AddRange(chipRegister.QSoundMakeSoftReset(i));
            for (int i = 0; i < chipRegister.RF5C164.Count; i++) if (chipRegister.RF5C164[i].Use) data.AddRange(chipRegister.RF5C164MakeSoftReset(i));
            for (int i = 0; i < chipRegister.SEGAPCM.Count; i++) if (chipRegister.SEGAPCM[i].Use) data.AddRange(chipRegister.SEGAPCMMakeSoftReset(i));
            for (int i = 0; i < chipRegister.SN76489.Count; i++) if (chipRegister.SN76489[i].Use) data.AddRange(chipRegister.SN76489MakeSoftReset(i));
            for (int i = 0; i < chipRegister.YM2151.Count; i++) if (chipRegister.YM2151[i].Use) data.AddRange(chipRegister.YM2151MakeSoftReset(i));
            for (int i = 0; i < chipRegister.YM2203.Count; i++) if (chipRegister.YM2203[i].Use) data.AddRange(chipRegister.YM2203MakeSoftReset(i));
            for (int i = 0; i < chipRegister.YM2413.Count; i++) if (chipRegister.YM2413[i].Use) data.AddRange(chipRegister.YM2413MakeSoftReset(i));
            for (int i = 0; i < chipRegister.YM3526.Count; i++) if (chipRegister.YM3526[i].Use) data.AddRange(chipRegister.YM3526MakeSoftReset(i));
            for (int i = 0; i < chipRegister.Y8950.Count; i++) if (chipRegister.Y8950[i].Use) data.AddRange(chipRegister.Y8950MakeSoftReset(i));
            for (int i = 0; i < chipRegister.YM3812.Count; i++) if (chipRegister.YM3812[i].Use) data.AddRange(chipRegister.YM3812MakeSoftReset(i));
            for (int i = 0; i < chipRegister.YMF262.Count; i++) if (chipRegister.YMF262[i].Use) data.AddRange(chipRegister.YMF262MakeSoftReset(i));
            for (int i = 0; i < chipRegister.YMF278B.Count; i++) if (chipRegister.YMF278B[i].Use) data.AddRange(chipRegister.YMF278BMakeSoftReset(i));
            for (int i = 0; i < chipRegister.YMF271.Count; i++) if (chipRegister.YMF271[i].Use) data.AddRange(chipRegister.YMF271MakeSoftReset(i));
            for (int i = 0; i < chipRegister.YM2608.Count; i++) if (chipRegister.YM2608[i].Use) data.AddRange(chipRegister.YM2608MakeSoftReset(i));
            for (int i = 0; i < chipRegister.YM2609.Count; i++) if (chipRegister.YM2609[i].Use) data.AddRange(chipRegister.YM2609MakeSoftReset(i));
            for (int i = 0; i < chipRegister.YM2610.Count; i++) if (chipRegister.YM2610[i].Use) data.AddRange(chipRegister.YM2610MakeSoftReset(i));
            for (int i = 0; i < chipRegister.YM2612.Count; i++) if (chipRegister.YM2612[i].Use) data.AddRange(chipRegister.YM2612MakeSoftReset(i));
            for (int i = 0; i < chipRegister.MIDI.Count; i++) if (chipRegister.MIDI[i].Use) data.AddRange(chipRegister.MIDIMakeSoftReset(i));

            return data.ToArray();
        }

        private static PackData[] MakeKeyOffData()
        {
            List<PackData> data = new List<PackData>();

            for (int i = 0; i < chipRegister.CONDUCTOR.Count; i++) if (chipRegister.CONDUCTOR[i].Use) data.AddRange(chipRegister.ConductorMakeSoftReset(i));
            for (int i = 0; i < chipRegister.AY8910.Count; i++) if (chipRegister.AY8910[i].Use) data.AddRange(chipRegister.AY8910MakeSoftReset(i));
            for (int i = 0; i < chipRegister.C140.Count; i++) if (chipRegister.C140[i].Use) data.AddRange(chipRegister.C140MakeSoftReset(i));
            for (int i = 0; i < chipRegister.DMG.Count; i++) if (chipRegister.DMG[i].Use) data.AddRange(chipRegister.DMGMakeSoftReset(i));
            for (int i = 0; i < chipRegister.HuC6280.Count; i++) if (chipRegister.HuC6280[i].Use) data.AddRange(chipRegister.HuC6280MakeSoftReset(i));
            for (int i = 0; i < chipRegister.K051649.Count; i++) if (chipRegister.K051649[i].Use) data.AddRange(chipRegister.K051649MakeSoftReset(i));
            for (int i = 0; i < chipRegister.K053260.Count; i++) if (chipRegister.K053260[i].Use) data.AddRange(chipRegister.K053260MakeSoftReset(i));
            for (int i = 0; i < chipRegister.NES.Count; i++) if (chipRegister.NES[i].Use) data.AddRange(chipRegister.NESMakeSoftReset(i));
            for (int i = 0; i < chipRegister.PPZ8.Count; i++) if (chipRegister.PPZ8[i].Use) data.AddRange(chipRegister.PPZ8MakeSoftReset(i));
            for (int i = 0; i < chipRegister.PPSDRV.Count; i++) if (chipRegister.PPSDRV[i].Use) data.AddRange(chipRegister.PPSDRVMakeSoftReset(i));
            for (int i = 0; i < chipRegister.P86.Count; i++) if (chipRegister.P86[i].Use) data.AddRange(chipRegister.P86MakeSoftReset(i));
            for (int i = 0; i < chipRegister.QSound.Count; i++) if (chipRegister.QSound[i].Use) data.AddRange(chipRegister.QSoundMakeSoftReset(i));
            for (int i = 0; i < chipRegister.RF5C164.Count; i++) if (chipRegister.RF5C164[i].Use) data.AddRange(chipRegister.RF5C164MakeSoftReset(i));
            for (int i = 0; i < chipRegister.SEGAPCM.Count; i++) if (chipRegister.SEGAPCM[i].Use) data.AddRange(chipRegister.SEGAPCMMakeSoftReset(i));
            for (int i = 0; i < chipRegister.SN76489.Count; i++) if (chipRegister.SN76489[i].Use) data.AddRange(chipRegister.SN76489MakeSoftReset(i));
            for (int i = 0; i < chipRegister.YM2151.Count; i++) if (chipRegister.YM2151[i].Use) data.AddRange(chipRegister.YM2151MakeSoftReset(i));
            for (int i = 0; i < chipRegister.YM2203.Count; i++) if (chipRegister.YM2203[i].Use) data.AddRange(chipRegister.YM2203MakeSoftReset(i));
            for (int i = 0; i < chipRegister.YM2413.Count; i++) if (chipRegister.YM2413[i].Use) data.AddRange(chipRegister.YM2413MakeSoftReset(i));
            for (int i = 0; i < chipRegister.YM3526.Count; i++) if (chipRegister.YM3526[i].Use) data.AddRange(chipRegister.YM3526MakeSoftReset(i));
            for (int i = 0; i < chipRegister.Y8950.Count; i++) if (chipRegister.Y8950[i].Use) data.AddRange(chipRegister.Y8950MakeSoftReset(i));
            for (int i = 0; i < chipRegister.YM3812.Count; i++) if (chipRegister.YM3812[i].Use) data.AddRange(chipRegister.YM3812MakeSoftReset(i));
            for (int i = 0; i < chipRegister.YMF262.Count; i++) if (chipRegister.YMF262[i].Use) data.AddRange(chipRegister.YMF262MakeSoftReset(i));
            for (int i = 0; i < chipRegister.YMF278B.Count; i++) if (chipRegister.YMF278B[i].Use) data.AddRange(chipRegister.YMF278BMakeSoftReset(i));
            for (int i = 0; i < chipRegister.YMF271.Count; i++) if (chipRegister.YMF271[i].Use) data.AddRange(chipRegister.YMF271MakeSoftReset(i));
            for (int i = 0; i < chipRegister.YM2608.Count; i++) if (chipRegister.YM2608[i].Use) data.AddRange(chipRegister.YM2608MakeSoftReset(i));
            for (int i = 0; i < chipRegister.YM2609.Count; i++) if (chipRegister.YM2609[i].Use) data.AddRange(chipRegister.YM2609MakeSoftReset(i));
            for (int i = 0; i < chipRegister.YM2610.Count; i++) if (chipRegister.YM2610[i].Use) data.AddRange(chipRegister.YM2610MakeSoftReset(i));
            for (int i = 0; i < chipRegister.YM2612.Count; i++) if (chipRegister.YM2612[i].Use) data.AddRange(chipRegister.YM2612MakeSoftResetKeyOffOnly(i));
            for (int i = 0; i < chipRegister.MIDI.Count; i++) if (chipRegister.MIDI[i].Use) data.AddRange(chipRegister.MIDIMakeSoftReset(i));

            return data.ToArray();
        }

        public static int trdVgmVirtualFunction(short[] buffer, int offset, int sampleCount)
        {
            if (startedOnceMethod != null)
            {
                startedOnceMethod();
                startedOnceMethod = null;
            }

            int cnt = trdVgmVirtualMainFunction(buffer, offset, sampleCount);

            //if (setting.midiKbd.UseMIDIKeyboard)
            //{
            //    if (bufVirtualFunction_MIDIKeyboard == null || bufVirtualFunction_MIDIKeyboard.Length < sampleCount)
            //    {
            //        bufVirtualFunction_MIDIKeyboard = new short[sampleCount];
            //    }
            //}
            return cnt;
        }

        private static long PackCounter = 0;
        private static SoundManager.PackData Pack = new SoundManager.PackData();

        private static bool AudioDeviceSync = false;
        private static object lockObjAudioDeviceSync = new object();
        public static bool GetAudioDeviceSync()
        {
            lock (lockObjAudioDeviceSync)
            {
                return AudioDeviceSync;
            }
        }
        public static void SetAudioDeviceSync()
        {
            lock (lockObjAudioDeviceSync)
            {
                AudioDeviceSync = true;
            }
        }
        public static void ResetAudioDeviceSync()
        {
            lock (lockObjAudioDeviceSync)
            {
                AudioDeviceSync = false;
            }
        }

        private static int trdVgmVirtualMainFunction(short[] buffer, int offset, int sampleCount)
        {
            EmuSampleCount = sampleCount;

            if (buffer == null || buffer.Length < 1 || sampleCount == 0)
            {
                SetAudioDeviceSync();
                return sampleCount;
            }

            if (mds == null || sm == null || !GetAudioDeviceSync() || waveMode)
            {
                SetAudioDeviceSync();
                return sampleCount;
            }

            try
            {

                int i;
                int cnt = 0;

                long bufCnt = sampleCount / 2;
                long seqcnt = sm.GetSeqCounter();

                EmuSeqCounterDelta = sm.GetSpeed();// 1.0;
                if(EmuSeqCounterDelta==0)
                {
                    if(emuRecvBuffer.GetDataSize()>0)
                    EmuSeqCounterDelta = 1;
                }
                //スピードの調整はせずにディレイの調整を行う
                //long sub = (seqcnt - EmuSeqCounter);
                //if (Math.Abs(sub) > bufCnt)
                //{
                //    long delta = Math.Abs(sub) - bufCnt;
                //    if (Math.Sign(delta) > 0)
                //    {

                //    }
                //    else
                //    {

                //    }
                //}

                //if (bufCnt > getLatency()*2)
                //{
                //    ;
                //}
                callcount = 0;

                {

                    //if (StepCounter > 0)
                    //{
                    //    StepCounter -= sampleCount;
                    //    if (StepCounter <= 0)
                    //    {
                    //        Paused = true;
                    //        StepCounter = 0;
                    //        mds.Update(buffer, offset, sampleCount, null);
                    //        SetAudioDeviceSync();
                    //        return sampleCount;
                    //    }
                    //}

                    if (driver != null) driver.vstDelta = 0;
                    stwh.Reset(); stwh.Start();
                    cnt = mds.Update(buffer, offset, sampleCount, oneFrameEmuDataSend);// driverVirtual.oneFrameProc);
                    ProcTimePer1Frame = ((double)stwh.ElapsedMilliseconds / (sampleCount + 1) * 1000000.0);
                    //if(buffer[offset]!=0) Console.WriteLine("{0}", buffer[offset]);
                }

                for (i = 0; i < sampleCount; i++)
                {
                    int mul = (int)(16384.0 * Math.Pow(10.0, MasterVolume / 40.0));
                    buffer[offset + i] = (short)Common.Range((buffer[offset + i] * mul) >> 13, -0x8000, 0x7fff);

                    if (!sm.GetFadeOut()) continue;

                    //フェードアウト処理
                    buffer[offset + i] = (short)(buffer[offset + i] * fadeoutCounterEmu);
                    fadeoutCounterEmu -= fadeoutCounterDelta;
                    if (fadeoutCounterEmu <= 0.0)
                    {
                        fadeoutCounterEmu = 0.0;
                    }

                }

                if (setting.outputDevice.DeviceType != Common.DEV_Null)
                {
                    updateVisualVolume(buffer, offset);
                }

                SetAudioDeviceSync();
                return sampleCount;

            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
                fatalError = true;
                Stopped = true;
            }

            SetAudioDeviceSync();
            return sampleCount;
        }

        public static double EmuSeqCounterDelta = 0.0;
        public static double RealSeqCounterDelta = 0.0;
        static double EmuSeqCounterWDelta = 0.0;
        static int callcount = 0;
        private static bool useEmu;
        private static bool useReal;
        public static int EmuSampleCount;
        private static Action startedOnceMethod = null;
        public static PackData[] stopDataVirtulaOnlySend;

        private static void oneFrameEmuDataSend()
        {
            if (sm.isVirtualOnlySend)
            {
                oneFrameVirtualOnlySendProc();
                return;
            }

            if (emuRecvBuffer == null) return;

            while ((long)emuRecvBuffer.LookUpCounter() <= EmuSeqCounter || !sm.IsRunningAtDataSender())//&& recvBuffer.LookUpCounter() != 0)
            {
                if (sm.IsRunningAtDataSender() && EmuSeqCounter > sm.GetSeqCounter())
                {
                    return;
                }

                bool ret = emuRecvBuffer.Deq(ref Pack.od, ref PackCounter, ref Pack.Chip, ref Pack.Type, ref Pack.Address, ref Pack.Data, ref Pack.ExData);
                if (!ret)
                {
                    if (!sm.IsRunningAtDataSender())
                    {
                        sm.RequestStopAtEmuChipSender();
                    }
                    break;
                }
                if (EmuSeqCounter - PackCounter > 5)
                {
                    ;
                }

                if (Pack.Address != -1 || Pack.Data != -1 || Pack.ExData != null)
                {
#if DEBUG
                    //ブロックデータを送っているのが誰かを調べる場合は有効にする
                    //if(Pack.ExData != null && Pack.ExData is PackData[] && ((PackData[])Pack.ExData).Length>0)
                    //{
                    //    Console.WriteLine("{0}",Pack.Chip.Device);
                    //}
#endif
                    chipRegister.SendChipData(PackCounter, Pack.Chip, Pack.Type, Pack.Address, Pack.Data, Pack.ExData);
                }
                else
                {
                    ;
                }

                //SetMMLTraceInfo?.Invoke(Pack);

            }

            if (!sm.IsRunningAtEmuChipSender())
            {
                if (stopDataVirtulaOnlySend != null)
                {
                    foreach (PackData dat in stopDataVirtulaOnlySend)
                    {
                        emuRecvBuffer.Enq(dat.od, 0, dat.Chip, dat.Type, dat.Address, dat.Data, null);
                    }
                    stopDataVirtulaOnlySend = null;
                }
                EmuSeqCounterDelta = 0;
            }

            while (EmuSeqCounterWDelta >= 1.0)
            {
                if (sm.IsRunningAsync() && sm.IsRunningAtEmuChipSender())
                {
                    EmuSeqCounter++;
                }
                EmuSeqCounterWDelta -= 1.0;
            }
            EmuSeqCounterWDelta += EmuSeqCounterDelta;
            //EmuSeqCounterWDelta += (sm.IsRunningAtEmuChipSender()) ? EmuSeqCounterDelta : 0;
            callcount++;
        }

        private static void oneFrameVirtualOnlySendProc()
        {
            if (emuRecvBuffer == null) return;

            sm.ForcedProc();

            bool ret = emuRecvBuffer.Deq(ref Pack.od, ref PackCounter, ref Pack.Chip, ref Pack.Type, ref Pack.Address, ref Pack.Data, ref Pack.ExData);
            if (!ret)
            {
                if (!sm.IsRunningAtDataSender())
                {
                    sm.RequestStopAtEmuChipSender();
                }
            }
            else
            {
                if (Pack.Address != -1 || Pack.Data != -1 || Pack.ExData != null)
                {
                    chipRegister.SendChipData(PackCounter, Pack.Chip, Pack.Type, Pack.Address, Pack.Data, Pack.ExData);
                }
            }

            if (!sm.IsRunningAtEmuChipSender())
            {
                if (stopDataVirtulaOnlySend != null)
                {
                    foreach (PackData dat in stopDataVirtulaOnlySend)
                    {
                        emuRecvBuffer.Enq(dat.od, 0, dat.Chip, dat.Type, dat.Address, dat.Data, null);
                    }
                    stopDataVirtulaOnlySend = null;
                }
                EmuSeqCounterDelta = 0;
            }

            while (EmuSeqCounterWDelta >= 1.0)
            {
                EmuSeqCounter++;
                EmuSeqCounterWDelta -= 1.0;
            }
            EmuSeqCounterWDelta += EmuSeqCounterDelta;

            callcount++;
        }

        private static void updateVisualVolume(short[] buffer, int offset)
        {
            if (mds == null) return;

            visVolume.master = buffer[offset];

            int[][][] vol = mds.getYM2151VisVolume();
            if (vol != null) visVolume.ym2151 = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

            vol = mds.getYM2203VisVolume();
            if (vol != null) visVolume.ym2203 = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);
            if (vol != null) visVolume.ym2203FM = (short)getMonoVolume(vol[0][1][0], vol[0][1][1], vol[1][1][0], vol[1][1][1]);
            if (vol != null) visVolume.ym2203SSG = (short)getMonoVolume(vol[0][2][0], vol[0][2][1], vol[1][2][0], vol[1][2][1]);

            vol = mds.getYM2612VisVolume();
            if (vol != null) visVolume.ym2612 = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

            vol = mds.getYM2608VisVolume();
            if (vol != null) visVolume.ym2608 = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);
            if (vol != null) visVolume.ym2608FM = (short)getMonoVolume(vol[0][1][0], vol[0][1][1], vol[1][1][0], vol[1][1][1]);
            if (vol != null) visVolume.ym2608SSG = (short)getMonoVolume(vol[0][2][0], vol[0][2][1], vol[1][2][0], vol[1][2][1]);
            if (vol != null) visVolume.ym2608Rtm = (short)getMonoVolume(vol[0][3][0], vol[0][3][1], vol[1][3][0], vol[1][3][1]);
            if (vol != null) visVolume.ym2608APCM = (short)getMonoVolume(vol[0][4][0], vol[0][4][1], vol[1][4][0], vol[1][4][1]);

            vol = mds.getYM2610VisVolume();
            if (vol != null) visVolume.ym2610 = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);
            if (vol != null) visVolume.ym2610FM = (short)getMonoVolume(vol[0][1][0], vol[0][1][1], vol[1][1][0], vol[1][1][1]);
            if (vol != null) visVolume.ym2610SSG = (short)getMonoVolume(vol[0][2][0], vol[0][2][1], vol[1][2][0], vol[1][2][1]);
            if (vol != null) visVolume.ym2610APCMA = (short)getMonoVolume(vol[0][3][0], vol[0][3][1], vol[1][3][0], vol[1][3][1]);
            if (vol != null) visVolume.ym2610APCMB = (short)getMonoVolume(vol[0][4][0], vol[0][4][1], vol[1][4][0], vol[1][4][1]);


            vol = mds.getYM2413VisVolume();
            if (vol != null) visVolume.ym2413 = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

            vol = mds.getYM3526VisVolume();
            if (vol != null) visVolume.ym3526 = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

            vol = mds.getY8950VisVolume();
            if (vol != null) visVolume.y8950 = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

            vol = mds.getYM3812VisVolume();
            if (vol != null) visVolume.ym3812 = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

            vol = mds.getYMF262VisVolume();
            if (vol != null) visVolume.ymf262 = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

            vol = mds.getYMF278BVisVolume();
            if (vol != null) visVolume.ymf278b = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

            vol = mds.getYMF271VisVolume();
            if (vol != null) visVolume.ymf271 = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

            vol = mds.getYMZ280BVisVolume();
            if (vol != null) visVolume.ymz280b = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

            vol = mds.getAY8910VisVolume();
            if (vol != null) visVolume.ay8910 = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

            vol = mds.getSN76489VisVolume();
            if (vol != null) visVolume.sn76489 = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

            vol = mds.getHuC6280VisVolume();
            if (vol != null) visVolume.huc6280 = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);


            vol = mds.getRF5C164VisVolume();
            if (vol != null) visVolume.rf5c164 = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

            vol = mds.getRF5C68VisVolume();
            if (vol != null) visVolume.rf5c68 = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

            vol = mds.getPWMVisVolume();
            if (vol != null) visVolume.pwm = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

            vol = mds.getOKIM6258VisVolume();
            if (vol != null) visVolume.okim6258 = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

            vol = mds.getOKIM6295VisVolume();
            if (vol != null) visVolume.okim6295 = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

            vol = mds.getC140VisVolume();
            if (vol != null) visVolume.c140 = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

            vol = mds.getC352VisVolume();
            if (vol != null) visVolume.c352 = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

            vol = mds.getSegaPCMVisVolume();
            if (vol != null) visVolume.segaPCM = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

            vol = mds.getMultiPCMVisVolume();
            if (vol != null) visVolume.multiPCM = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

            vol = mds.getK051649VisVolume();
            if (vol != null) visVolume.k051649 = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

            vol = mds.getK053260VisVolume();
            if (vol != null) visVolume.k053260 = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

            vol = mds.getK054539VisVolume();
            if (vol != null) visVolume.k054539 = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

            vol = mds.getQSoundCtrVisVolume();
            if (vol != null) visVolume.qSound = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);

            vol = mds.getGA20VisVolume();
            if (vol != null) visVolume.ga20 = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);


            vol = mds.getNESVisVolume();
            if (vol != null) visVolume.APU = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);
            else visVolume.APU = (short)MDSound.MDSound.np_nes_apu_volume;

            vol = mds.getDMCVisVolume();
            if (vol != null) visVolume.DMC = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);
            else visVolume.DMC = (short)MDSound.MDSound.np_nes_dmc_volume;

            vol = mds.getFDSVisVolume();
            if (vol != null) visVolume.FDS = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);
            else visVolume.FDS = (short)MDSound.MDSound.np_nes_fds_volume;

            vol = mds.getMMC5VisVolume();
            if (vol != null) visVolume.MMC5 = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);
            if (visVolume.MMC5 == 0) visVolume.MMC5 = (short)MDSound.MDSound.np_nes_mmc5_volume;

            vol = mds.getN160VisVolume();
            if (vol != null) visVolume.N160 = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);
            if (visVolume.N160 == 0) visVolume.N160 = (short)MDSound.MDSound.np_nes_n106_volume;

            vol = mds.getVRC6VisVolume();
            if (vol != null) visVolume.VRC6 = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);
            if (visVolume.VRC6 == 0) visVolume.VRC6 = (short)MDSound.MDSound.np_nes_vrc6_volume;

            vol = mds.getVRC7VisVolume();
            if (vol != null) visVolume.VRC7 = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);
            if (visVolume.VRC7 == 0) visVolume.VRC7 = (short)MDSound.MDSound.np_nes_vrc7_volume;

            vol = mds.getFME7VisVolume();
            if (vol != null) visVolume.FME7 = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);
            if (visVolume.FME7 == 0) visVolume.FME7 = (short)MDSound.MDSound.np_nes_fme7_volume;

            vol = mds.getDMGVisVolume();
            if (vol != null) visVolume.DMG = (short)getMonoVolume(vol[0][0][0], vol[0][0][1], vol[1][0][0], vol[1][0][1]);
        }

        public static int getMonoVolume(int pl, int pr, int sl, int sr)
        {
            int v = pl + pr + sl + sr;
            v >>= 1;
            if (sl + sr != 0) v >>= 1;

            return v;
        }

        public static long getVirtualFrameCounter()
        {
            if (driver == null) return -1;
            return driver.vgmFrameCounter;
        }

        public static long getRealFrameCounter()
        {
            return -1;
        }

        public static GD3 GetGD3()
        {
            if (driver != null) return driver.GD3;
            return null;
        }


        #region MDSoundインターフェース

        public static int[][] GetFMRegister(int chipID)
        {
            return chipRegister.fmRegisterYM2612[chipID];
        }

        public static int[] GetYM2151Register(int chipID)
        {
            return chipRegister.YM2151FmRegister[chipID];
        }

        public static int[] GetYM2203Register(int chipID)
        {
            return chipRegister.fmRegisterYM2203[chipID];
        }

        public static int[] GetYM2413Register(int chipID)
        {
            return chipRegister.fmRegisterYM2413[chipID];
        }

        public static byte[] GetVRC7Register(int chipID)
        {
            return chipRegister.getVRC7Register(chipID);
        }

        public static int[][] GetYM2608Register(int chipID)
        {
            return chipRegister.fmRegisterYM2608[chipID];
        }

        public static int[][] GetYM2610Register(int chipID)
        {
            return chipRegister.fmRegisterYM2610[chipID];
        }

        public static int[] GetYM3526Register(int chipID)
        {
            return chipRegister.fmRegisterYM3526[chipID];
        }

        public static int[] GetY8950Register(int chipID)
        {
            return chipRegister.fmRegisterY8950[chipID];
        }

        public static int[] GetYM3812Register(int chipID)
        {
            return chipRegister.fmRegisterYM3812[chipID];
        }

        public static int[][] GetYMF262Register(int chipID)
        {
            return chipRegister.fmRegisterYMF262[chipID];
        }

        public static int[][] GetYMF278BRegister(int chipID)
        {
            return chipRegister.fmRegisterYMF278B[chipID];
        }

        public static int[] GetPSGRegister(int chipID)
        {
            return chipRegister.SN76489Register[chipID];
        }

        public static int GetPSGRegisterGGPanning(int chipID)
        {
            return chipRegister.SN76489RegisterGGPan[chipID];
        }

        public static int[] GetAY8910Register(int chipID)
        {
            return chipRegister.AY8910PsgRegister[chipID];
        }

        public static Ootake_PSG.huc6280_state GetHuC6280Register(int chipID)
        {
            return mds.ReadHuC6280Status(chipID);
        }

        public static scd_pcm.pcm_chip_ GetRf5c164Register(int chipID)
        {
            return mds.ReadRf5c164Register(chipID);
        }

        public static byte[] GetC140Register(int chipID)
        {
            return chipRegister.pcmRegisterC140[chipID];
        }

        public static bool[] GetC140KeyOn(int chipID)
        {
            return chipRegister.pcmKeyOnC140[chipID];
        }

        public static ushort[] GetC352Register(int chipID)
        {
            return chipRegister.pcmRegisterC352[chipID];
        }

        public static ushort[] GetC352KeyOn(int chipID)
        {
            return chipRegister.readC352((byte)chipID);
        }

        public static byte[] GetSEGAPCMRegister(int chipID)
        {
            return chipRegister.pcmRegisterSEGAPCM[chipID];
        }

        public static bool[] GetSEGAPCMKeyOn(int chipID)
        {
            return chipRegister.pcmKeyOnSEGAPCM[chipID];
        }

        public static okim6258.okim6258_state GetOKIM6258Register(int chipID)
        {
            return mds.ReadOKIM6258Status(chipID);
        }

        public static segapcm.segapcm_state GetSegaPCMRegister(int chipID)
        {
            return mds.ReadSegaPCMStatus(chipID);
        }

        public static byte[] GetAPURegister(int chipID)
        {
            byte[] reg;

            //nsf向け
            if (chipRegister == null) reg = null;
            else if (chipRegister.nes_apu == null) reg = null;
            else if (chipRegister.nes_apu.chip == null) reg = null;
            else if (chipID == 1) reg = null;
            else reg = chipRegister.nes_apu.chip.reg;

            //vgm向け
            if (reg == null) reg = chipRegister.getNESRegister(chipID);

            return reg;
        }

        public static byte[] GetDMCRegister(int chipID)
        {
            try
            {
                byte[] reg;
                //nsf向け
                if (chipRegister == null) reg = null;
                else if (chipRegister.nes_apu == null) reg = null;
                else if (chipRegister.nes_apu.chip == null) reg = null;
                else if (chipID == 1) reg = null;
                else reg = chipRegister.nes_dmc.chip.reg;

                //vgm向け
                //if (reg == null) reg = chipRegister.getNESRegister(chipID, enmModel.VirtualModel);

                return reg;
            }
            catch
            {
                return null;
            }
        }

        public static MDSound.np.np_nes_fds.NES_FDS GetFDSRegister(int chipID)
        {
            MDSound.np.np_nes_fds.NES_FDS reg;

            //nsf向け
            if (chipRegister == null) reg = null;
            else if (chipRegister.nes_apu == null) reg = null;
            else if (chipRegister.nes_apu.chip == null) reg = null;
            else if (chipID == 1) reg = null;
            else reg = chipRegister.nes_fds.chip;

            //vgm向け
            if (reg == null) reg = chipRegister.getFDSRegister(chipID, EnmVRModel.VirtualModel);

            return reg;
        }

        public static byte[] GetMMC5Register(int chipID)
        {
            //nsf向け
            if (chipRegister == null) return null;
            else if (chipRegister.nes_mmc5 == null) return null;
            else if (chipID == 1) return null;

            uint dat = 0;
            for (uint adr = 0x5000; adr < 0x5008; adr++)
            {
                dat = 0;
                chipRegister.nes_mmc5.Read(adr, ref dat);
                mmc5regs[adr & 0x7] = (byte)dat;
            }

            chipRegister.nes_mmc5.Read(0x5010, ref dat);
            mmc5regs[8] = (byte)(chipRegister.nes_mmc5.pcm_mode ? 1 : 0);
            mmc5regs[9] = chipRegister.nes_mmc5.pcm;


            return mmc5regs;
        }

        public static int[] GetFMKeyOn(int chipID)
        {
            return chipRegister.fmKeyOnYM2612[chipID];
        }

        public static int[] GetYM2151KeyOn(int chipID)
        {
            return chipRegister.YM2151FmKeyOn[chipID];
        }

        public static bool GetOKIM6258KeyOn(int chipID)
        {
            return chipRegister.okim6258Keyon[chipID];
        }

        public static void ResetOKIM6258KeyOn(int chipID)
        {
            chipRegister.okim6258Keyon[chipID] = false;
        }

        public static int GetYM2151PMD(int chipID)
        {
            return chipRegister.YM2151FmPMD[chipID];
        }

        public static int GetYM2151AMD(int chipID)
        {
            return chipRegister.YM2151FmAMD[chipID];
        }

        public static int[] GetYM2608KeyOn(int chipID)
        {
            return chipRegister.fmKeyOnYM2608[chipID];
        }

        public static int[] GetYM2610KeyOn(int chipID)
        {
            return chipRegister.fmKeyOnYM2610[chipID];
        }

        public static int[] GetYM2203KeyOn(int chipID)
        {
            return chipRegister.fmKeyOnYM2203[chipID];
        }

        public static ChipKeyInfo getYM2413KeyInfo(int chipID)
        {
            return chipRegister.getYM2413KeyInfo(chipID);
        }

        public static ChipKeyInfo getYM3526KeyInfo(int chipID)
        {
            return chipRegister.getYM3526KeyInfo(chipID);
        }

        public static ChipKeyInfo getY8950KeyInfo(int chipID)
        {
            return chipRegister.getY8950KeyInfo(chipID);
        }

        public static ChipKeyInfo getYM3812KeyInfo(int chipID)
        {
            return chipRegister.getYM3812KeyInfo(chipID);
        }

        public static ChipKeyInfo getVRC7KeyInfo(int chipID)
        {
            return chipRegister.getVRC7KeyInfo(chipID);
        }

        public static int getYMF262FMKeyON(int chipID)
        {
            return chipRegister.getYMF262FMKeyON(chipID);
        }

        public static int getYMF262RyhthmKeyON(int chipID)
        {
            return chipRegister.getYMF262RyhthmKeyON(chipID);
        }

        public static int getYMF278BFMKeyON(int chipID)
        {
            return chipRegister.getYMF278BFMKeyON(chipID);
        }

        public static void resetYMF278BFMKeyON(int chipID)
        {
            chipRegister.resetYMF278BFMKeyON(chipID);
        }

        public static int getYMF278BRyhthmKeyON(int chipID)
        {
            return chipRegister.getYMF278BRyhthmKeyON(chipID);
        }

        public static void resetYMF278BRyhthmKeyON(int chipID)
        {
            chipRegister.resetYMF278BRyhthmKeyON(chipID);
        }

        public static int[] getYMF278BPCMKeyON(int chipID)
        {
            return chipRegister.getYMF278BPCMKeyON(chipID);
        }

        public static void resetYMF278BPCMKeyON(int chipID)
        {
            chipRegister.resetYMF278BPCMKeyON(chipID);
        }


        public static void SetMasterVolume(bool isAbs, int volume)
        {
            MasterVolume
                = setting.balance.MasterVolume
                = Common.Range((isAbs ? 0 : setting.balance.MasterVolume) + volume, -192, 20);
        }

        public static void SetAY8910Volume(bool isAbs, int volume)
        {
            try
            {
                mds.setVolumeAY8910(setting.balance.AY8910Volume
                    = Common.Range((isAbs ? 0 : setting.balance.AY8910Volume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetYM2151Volume(bool isAbs, int volume)
        {
            try
            {
                int vol
                    = setting.balance.YM2151Volume
                    = Common.Range((isAbs ? 0 : setting.balance.YM2151Volume) + volume, -192, 20);

                mds.SetVolumeYM2151(vol);
                mds.SetVolumeYM2151mame(vol);
                mds.SetVolumeYM2151x68sound(vol);
            }
            catch { }
        }

        public static void SetYM2203Volume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeYM2203(setting.balance.YM2203Volume
                    = Common.Range((isAbs ? 0 : setting.balance.YM2203Volume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetYM2203FMVolume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeYM2203FM(setting.balance.YM2203FMVolume
                    = Common.Range((isAbs ? 0 : setting.balance.YM2203FMVolume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetYM2203PSGVolume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeYM2203PSG(setting.balance.YM2203PSGVolume
                    = Common.Range((isAbs ? 0 : setting.balance.YM2203PSGVolume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetYM2413Volume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeYM2413(setting.balance.YM2413Volume
                    = Common.Range((isAbs ? 0 : setting.balance.YM2413Volume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetK053260Volume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeK053260(setting.balance.K053260Volume
                    = Common.Range((isAbs ? 0 : setting.balance.K053260Volume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetRF5C68Volume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeRF5C68(setting.balance.RF5C68Volume
                    = Common.Range((isAbs ? 0 : setting.balance.RF5C68Volume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetYM3812Volume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeYM3812(setting.balance.YM3812Volume
                    = Common.Range((isAbs ? 0 : setting.balance.YM3812Volume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetY8950Volume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeY8950(setting.balance.Y8950Volume
                    = Common.Range((isAbs ? 0 : setting.balance.Y8950Volume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetYM3526Volume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeYM3526(setting.balance.YM3526Volume
                    = Common.Range((isAbs ? 0 : setting.balance.YM3526Volume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetYM2608Volume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeYM2608(setting.balance.YM2608Volume
                    = Common.Range((isAbs ? 0 : setting.balance.YM2608Volume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetYM2608FMVolume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeYM2608FM(setting.balance.YM2608FMVolume
                    = Common.Range((isAbs ? 0 : setting.balance.YM2608FMVolume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetYM2608PSGVolume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeYM2608PSG(setting.balance.YM2608PSGVolume
                    = Common.Range((isAbs ? 0 : setting.balance.YM2608PSGVolume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetYM2608RhythmVolume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeYM2608Rhythm(setting.balance.YM2608RhythmVolume
                    = Common.Range((isAbs ? 0 : setting.balance.YM2608RhythmVolume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetYM2608AdpcmVolume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeYM2608Adpcm(setting.balance.YM2608AdpcmVolume
                    = Common.Range((isAbs ? 0 : setting.balance.YM2608AdpcmVolume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetYM2610Volume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeYM2610(setting.balance.YM2610Volume
                    = Common.Range((isAbs ? 0 : setting.balance.YM2610Volume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetYM2610FMVolume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeYM2610FM(setting.balance.YM2610FMVolume
                    = Common.Range((isAbs ? 0 : setting.balance.YM2610FMVolume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetYM2610PSGVolume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeYM2610PSG(setting.balance.YM2610PSGVolume
                    = Common.Range((isAbs ? 0 : setting.balance.YM2610PSGVolume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetYM2610AdpcmAVolume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeYM2610AdpcmA(setting.balance.YM2610AdpcmAVolume
                    = Common.Range((isAbs ? 0 : setting.balance.YM2610AdpcmAVolume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetYM2610AdpcmBVolume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeYM2610AdpcmB(setting.balance.YM2610AdpcmBVolume
                    = Common.Range((isAbs ? 0 : setting.balance.YM2610AdpcmBVolume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetYM2612Volume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeYM2612(setting.balance.YM2612Volume
                    = Common.Range((isAbs ? 0 : setting.balance.YM2612Volume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetSN76489Volume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeSN76489(setting.balance.SN76489Volume
                    = Common.Range((isAbs ? 0 : setting.balance.SN76489Volume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetHuC6280Volume(bool isAbs, int volume)
        {
            try
            {
                mds.setVolumeHuC6280(setting.balance.HuC6280Volume
                    = Common.Range((isAbs ? 0 : setting.balance.HuC6280Volume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetRF5C164Volume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeRF5C164(setting.balance.RF5C164Volume
                    = Common.Range((isAbs ? 0 : setting.balance.RF5C164Volume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetPWMVolume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumePWM(setting.balance.PWMVolume
                    = Common.Range((isAbs ? 0 : setting.balance.PWMVolume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetOKIM6258Volume(bool isAbs, int volume)
        {
            try
            {
                int vol = setting.balance.OKIM6258Volume
                    = Common.Range((isAbs ? 0 : setting.balance.OKIM6258Volume) + volume, -192, 20);

                mds.SetVolumeOKIM6258(vol);
                mds.SetVolumeMpcmX68k(vol);
            }
            catch { }
        }

        public static void SetOKIM6295Volume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeOKIM6295(setting.balance.OKIM6295Volume
                    = Common.Range((isAbs ? 0 : setting.balance.OKIM6295Volume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetC140Volume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeC140(setting.balance.C140Volume
                    = Common.Range((isAbs ? 0 : setting.balance.C140Volume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetSegaPCMVolume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeSegaPCM(setting.balance.SEGAPCMVolume
                    = Common.Range((isAbs ? 0 : setting.balance.SEGAPCMVolume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetC352Volume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeC352(setting.balance.C352Volume
                    = Common.Range((isAbs ? 0 : setting.balance.C352Volume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetK051649Volume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeK051649(setting.balance.K051649Volume
                    = Common.Range((isAbs ? 0 : setting.balance.K051649Volume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetK054539Volume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeK054539(setting.balance.K054539Volume
                    = Common.Range((isAbs ? 0 : setting.balance.K054539Volume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetQSoundVolume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeQSoundCtr(setting.balance.QSoundVolume
                    = Common.Range((isAbs ? 0 : setting.balance.QSoundVolume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetDMGVolume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeDMG(setting.balance.DMGVolume
                    = Common.Range((isAbs ? 0 : setting.balance.DMGVolume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetNESVolume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeNES(setting.balance.APUVolume
                    = Common.Range((isAbs ? 0 : setting.balance.APUVolume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetGA20Volume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeGA20(setting.balance.GA20Volume
                    = Common.Range((isAbs ? 0 : setting.balance.GA20Volume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetYMZ280BVolume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeYMZ280B(setting.balance.YMZ280BVolume
                    = Common.Range((isAbs ? 0 : setting.balance.YMZ280BVolume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetYMF271Volume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeYMF271(setting.balance.YMF271Volume
                    = Common.Range((isAbs ? 0 : setting.balance.YMF271Volume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetYMF262Volume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeYMF262(setting.balance.YMF262Volume
                    = Common.Range((isAbs ? 0 : setting.balance.YMF262Volume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetYMF278BVolume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeYMF278B(setting.balance.YMF278BVolume
                    = Common.Range((isAbs ? 0 : setting.balance.YMF278BVolume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetMultiPCMVolume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeMultiPCM(setting.balance.MultiPCMVolume
                    = Common.Range((isAbs ? 0 : setting.balance.MultiPCMVolume) + volume, -192, 20));
            }
            catch { }
        }



        public static void SetAPUVolume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeNES(
                    setting.balance.APUVolume
                    = Common.Range((isAbs ? 0 : setting.balance.APUVolume) + volume, -192, 20)
                    );
            }
            catch { }
        }

        public static void SetDMCVolume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeDMC(setting.balance.DMCVolume
                    = Common.Range((isAbs ? 0 : setting.balance.DMCVolume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetFDSVolume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeFDS(setting.balance.FDSVolume
                    = Common.Range((isAbs ? 0 : setting.balance.FDSVolume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetMMC5Volume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeMMC5(setting.balance.MMC5Volume
                    = Common.Range((isAbs ? 0 : setting.balance.MMC5Volume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetN160Volume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeN160(setting.balance.N160Volume
                    = Common.Range((isAbs ? 0 : setting.balance.N160Volume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetVRC6Volume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeVRC6(setting.balance.VRC6Volume
                    = Common.Range((isAbs ? 0 : setting.balance.VRC6Volume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetVRC7Volume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeVRC7(setting.balance.VRC7Volume
                    = Common.Range((isAbs ? 0 : setting.balance.VRC7Volume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetFME7Volume(bool isAbs, int volume)
        {
            try
            {
                mds.SetVolumeFME7(setting.balance.FME7Volume
                    = Common.Range((isAbs ? 0 : setting.balance.FME7Volume) + volume, -192, 20));
            }
            catch { }
        }

        public static void SetGimicOPNVolume(bool isAbs, int volume)
        {
            setting.balance.GimicOPNVolume = Common.Range((isAbs ? 0 : setting.balance.GimicOPNVolume) + volume, 0, 127);
        }

        public static void SetGimicOPNAVolume(bool isAbs, int volume)
        {
            setting.balance.GimicOPNAVolume = Common.Range((isAbs ? 0 : setting.balance.GimicOPNAVolume) + volume, 0, 127);
        }


        public static int[] GetFMVolume(int chipID)
        {
            return chipRegister.GetYM2612Volume(chipID);
        }

        public static int[] GetYM2151Volume(int chipID)
        {
            return chipRegister.GetYM2151Volume(chipID);
        }

        public static int[] GetYM2608Volume(int chipID)
        {
            return chipRegister.GetYM2608Volume(chipID);
        }

        public static int[][] GetYM2608RhythmVolume(int chipID)
        {
            return chipRegister.GetYM2608RhythmVolume(chipID);
        }

        public static int[] GetYM2608AdpcmVolume(int chipID)
        {
            return chipRegister.GetYM2608AdpcmVolume(chipID);
        }

        public static int[] GetYM2610Volume(int chipID)
        {
            return chipRegister.GetYM2610Volume(chipID);
        }

        public static int[][] GetYM2610RhythmVolume(int chipID)
        {
            return chipRegister.GetYM2610RhythmVolume(chipID);
        }

        public static int[] GetYM2610AdpcmVolume(int chipID)
        {
            return chipRegister.GetYM2610AdpcmVolume(chipID);
        }

        public static int[] GetYM2203Volume(int chipID)
        {
            return chipRegister.GetYM2203Volume(chipID);
        }

        public static int[] GetFMCh3SlotVolume(int chipID)
        {
            return chipRegister.GetYM2612Ch3SlotVolume(chipID);
        }

        public static int[] GetYM2608Ch3SlotVolume(int chipID)
        {
            return chipRegister.GetYM2608Ch3SlotVolume(chipID);
        }

        public static int[] GetYM2610Ch3SlotVolume(int chipID)
        {
            return chipRegister.GetYM2610Ch3SlotVolume(chipID);
        }

        public static int[] GetYM2203Ch3SlotVolume(int chipID)
        {
            return chipRegister.GetYM2203Ch3SlotVolume(chipID);
        }

        public static int[][] GetPSGVolume(int chipID)
        {
            return chipRegister.GetPSGVolume(chipID);
        }



        public static void setSN76489Mask(int chipID, int ch)
        {
            //mds.setSN76489Mask(chipID,1 << ch);
            chipRegister.SN76489SetMask(chipID, ch, true);
        }

        public static void setRF5C164Mask(int chipID, int ch)
        {
            mds.setRf5c164Mask(chipID, ch);
        }

        public static void setYM2151Mask(int chipID, int ch)
        {
            //mds.setYM2151Mask(ch);
            chipRegister.YM2151SetMask(0, chipID, ch, true);
        }

        public static void setYM2203Mask(int chipID, int ch)
        {
            chipRegister.YM2203SetMask(0, chipID, ch, true);
        }

        public static void setYM2413Mask(int chipID, int ch)
        {
            chipRegister.YM2413SetMask(0, chipID, ch, true);
        }

        public static void setYM2608Mask(int chipID, int ch)
        {
            //mds.setYM2608Mask(ch);
            chipRegister.YM2608SetMask(0, chipID, ch, true);
        }

        public static void setYM2610Mask(int chipID, int ch)
        {
            //mds.setYM2610Mask(ch);
            chipRegister.YM2610SetMask(0, chipID, ch, true);
        }

        public static void setYM2612Mask(int chipID, int ch)
        {
            chipRegister.YM2612SetMask(0, chipID, ch, true);
        }

        public static void setYM3526Mask(int chipID, int ch)
        {
            chipRegister.YM3526SetMask(0, chipID, ch, true);
        }

        public static void setY8950Mask(int chipID, int ch)
        {
            chipRegister.Y8950SetMask(0, chipID, ch, true);
        }

        public static void setYM3812Mask(int chipID, int ch)
        {
            chipRegister.YM3812SetMask(0, chipID, ch, true);
        }

        public static void setYMF262Mask(int chipID, int ch)
        {
            chipRegister.YMF262SetMask(0, chipID, ch, true);
        }

        public static void setYMF278BMask(int chipID, int ch)
        {
            chipRegister.YMF278BSetMask(0, chipID, ch, true);
        }

        public static void setC140Mask(int chipID, int ch)
        {
            mds.setC140Mask(chipID, 1 << ch);
        }

        public static void setC352Mask(int chipID, int ch)
        {
            chipRegister.setMaskC352(chipID, ch, true);
        }

        public static void setSegaPCMMask(int chipID, int ch)
        {
            mds.setSegaPcmMask(chipID, 1 << ch);
        }

        public static void setAY8910Mask(int chipID, int ch)
        {
            mds.setAY8910Mask(chipID, 1 << ch);
        }

        public static void setHuC6280Mask(int chipID, int ch)
        {
            chipRegister.HuC6280SetMask(0, chipID, ch, true);
        }

        public static void setOKIM6258Mask(int chipID)
        {
            chipRegister.setMaskOKIM6258(chipID, true);
        }

        public static void setNESMask(int chipID, int ch)
        {
            chipRegister.setNESMask(chipID, ch);
        }

        public static void setDMCMask(int chipID, int ch)
        {
            chipRegister.setNESMask(chipID, ch + 2);
        }

        public static void setFDSMask(int chipID)
        {
            chipRegister.setFDSMask(chipID);
        }

        public static void setMMC5Mask(int chipID, int ch)
        {
            chipRegister.setMMC5Mask(chipID, ch);
        }

        public static void setVRC7Mask(int chipID, int ch)
        {
            chipRegister.setVRC7Mask(chipID, ch);
        }

        public static void setK051649Mask(int chipID, int ch)
        {
            chipRegister.K051649SetMask(0, chipID, ch, true);
        }

        public static void setK053260Mask(int chipID, int ch)
        {
            chipRegister.K053260SetMask(0, chipID, ch, true);
        }

        public static void setQSoundMask(int chipID, int ch)
        {
            chipRegister.QSoundSetMask(0, chipID, ch, true);
        }


        public static void resetSN76489Mask(int chipID, int ch)
        {
            try
            {
                //mds.resetSN76489Mask(chipID, 1 << ch);
                chipRegister.SN76489SetMask(chipID, ch, false);
            }
            catch { }
        }

        public static void resetYM2608Mask(int chipID, int ch)
        {
            try
            {
                //mds.resetYM2608Mask(ch);
                chipRegister.YM2608SetMask(0, chipID, ch, false, Stopped);
            }
            catch { }
        }

        public static void resetYM2612Mask(int chipID, int ch)
        {
            try
            {
                //mds.resetYM2612Mask(chipID, 1 << ch);
                chipRegister.YM2612SetMask(0, chipID, ch, false);
            }
            catch { }
        }

        public static void resetOKIM6258Mask(int chipID)
        {
            chipRegister.setMaskOKIM6258(chipID, false);
        }

        public static void resetYM2203Mask(int chipID, int ch)
        {
            try
            {
                chipRegister.YM2203SetMask(0, chipID, ch, false, Stopped);
            }
            catch { }
        }

        public static void resetYM2413Mask(int chipID, int ch)
        {
            try
            {
                chipRegister.YM2413SetMask(0, chipID, ch, false, Stopped);
            }
            catch { }
        }

        public static void resetRF5C164Mask(int chipID, int ch)
        {
            try
            {
                mds.resetRf5c164Mask(chipID, ch);
            }
            catch { }
        }

        public static void resetYM2151Mask(int chipID, int ch)
        {
            try
            {
                //mds.resetYM2151Mask(ch);
                chipRegister.YM2151SetMask(0, chipID, ch, false);//, Stopped);
            }
            catch { }
        }

        public static void resetYM2610Mask(int chipID, int ch)
        {
            try
            {
                chipRegister.YM2610SetMask(0, chipID, ch, false);
            }
            catch { }
        }

        public static void resetYM3526Mask(int chipID, int ch)
        {
            try
            {
                chipRegister.YM3526SetMask(0, chipID, ch, false);
            }
            catch { }
        }

        public static void resetY8950Mask(int chipID, int ch)
        {
            try
            {
                chipRegister.Y8950SetMask(0, chipID, ch, false);
            }
            catch { }
        }

        public static void resetYM3812Mask(int chipID, int ch)
        {
            try
            {
                chipRegister.YM3812SetMask(0, chipID, ch, false);
            }
            catch { }
        }

        public static void resetYMF262Mask(int chipID, int ch)
        {
            try
            {
                chipRegister.YMF262SetMask(0, chipID, ch, false);
            }
            catch { }
        }

        public static void resetYMF278BMask(int chipID, int ch)
        {
            try
            {
                chipRegister.YMF278BSetMask(0, chipID, ch, false);
            }
            catch { }
        }

        public static void resetC140Mask(int chipID, int ch)
        {
            mds.resetC140Mask(chipID, 1 << ch);
        }

        public static void resetC352Mask(int chipID, int ch)
        {
            try
            {
                chipRegister.setMaskC352(chipID, ch, false);
            }
            catch { }
        }

        public static void resetSegaPCMMask(int chipID, int ch)
        {
            mds.resetSegaPcmMask(chipID, 1 << ch);
        }

        public static void resetAY8910Mask(int chipID, int ch)
        {
            mds.resetAY8910Mask(chipID, 1 << ch);
        }

        public static void resetHuC6280Mask(int chipID, int ch)
        {
            try
            {
                chipRegister.HuC6280SetMask(0, chipID, ch, false);
            }
            catch { }
        }

        public static void resetNESMask(int chipID, int ch)
        {
            chipRegister.resetNESMask(chipID, ch);
        }

        public static void resetDMCMask(int chipID, int ch)
        {
            chipRegister.resetNESMask(chipID, ch + 2);
        }

        public static void resetFDSMask(int chipID)
        {
            chipRegister.resetFDSMask(chipID);
        }

        public static void resetMMC5Mask(int chipID, int ch)
        {
            chipRegister.resetMMC5Mask(chipID, ch);
        }

        public static void resetVRC7Mask(int chipID, int ch)
        {
            chipRegister.resetVRC7Mask(chipID, ch);
        }

        public static void resetK051649Mask(int chipID, int ch)
        {
            chipRegister.K051649SetMask(0, chipID, ch, false);
        }

        public static void resetK053260Mask(int chipID, int ch)
        {
            chipRegister.K053260SetMask(0, chipID, ch, false);
        }

        public static void resetQSoundMask(int chipID, int ch)
        {
            chipRegister.QSoundSetMask(0, chipID, ch, false);
        }

        #endregion

        public static bool PlayToWav(Setting setting,string fnWav, bool doSkipStop = false, Action startedOnceMethod = null)
        {
            useEmu = false;
            useReal = false;

            waveMode = true;

            errMsg = "";
            Stop(SendMode.Both);

            //if (doSkipStop)
            //{
            //sm.SetMode(SendMode.RealTime);
            //}

            //sm.SetSpeed(1.0);
            vgmSpeed = 1.0;
            //sm.SetMusicInterruptTimer(setting.musicInterruptTimer);

            ////スレッドなどの準備など(?)で演奏開始時にテンポが乱れることがあるため念のため待つ。
            //DriverSeqCounter = sm.GetDriverSeqCounterDelay();

            ////開始時にバッファ分のデータが貯まらないうちにコールバックがくるとテンポが乱れるため、レイテンシ(デバイスのバッファ)分だけ演奏開始を待つ。
            //DriverSeqCounter += getLatency();

            //MDSound.MDSound.np_nes_apu_volume = 0;
            //MDSound.MDSound.np_nes_dmc_volume = 0;
            //MDSound.MDSound.np_nes_fds_volume = 0;
            //MDSound.MDSound.np_nes_fme7_volume = 0;
            //MDSound.MDSound.np_nes_mmc5_volume = 0;
            //MDSound.MDSound.np_nes_n106_volume = 0;
            //MDSound.MDSound.np_nes_vrc6_volume = 0;
            //MDSound.MDSound.np_nes_vrc7_volume = 0;


            bool ret = playSet();
            if (!ret) return false;
            //sm.SetMode(SendMode.MML);
            //if (sm.Mode == SendMode.MML)
            //{
            //    sm.RequestStart(SendMode.MML, useEmu, useReal);
            //}
            //else
            //{
            //    sm.RequestStart(SendMode.Both, useEmu, useReal);
            //}

            //while (!sm.IsRunningAsync())
            //{
            //}

            Audio.startedOnceMethod = startedOnceMethod;

            EmuSeqCounter = 0;
            DriverSeqCounter = 0;

            //Stopped = false;

            //            if (!useEmu) 
            sm.RequestStopAtEmuChipSender();
            //            if (!useReal) 
            sm.RequestStopAtRealChipSender();

            Stop(SendMode.Both);

            toWavWaveWriter = new WaveWriter(setting);
            toWavWaveWriter.Open(fnWav);

            Thread mm = new Thread(new ThreadStart(trdToWavRenderingProcess));
            mm.Start();

            return ret;
        }

        public static bool PlayToMid(Setting setting, string fnMid, bool doSkipStop = false, Action startedOnceMethod = null)
        {
            useEmu = false;
            useReal = false;

            waveMode = true;

            errMsg = "";
            Stop(SendMode.Both);

            vgmSpeed = 1.0;
            bool ret = playSet();
            if (!ret) return false;

            Audio.startedOnceMethod = startedOnceMethod;

            EmuSeqCounter = 0;
            DriverSeqCounter = 0;

            sm.RequestStopAtEmuChipSender();
            sm.RequestStopAtRealChipSender();

            Stop(SendMode.Both);

            toMidWriter = new MidWriter(setting);
            toMidWriter.Open(fnMid);

            Thread mm = new Thread(new ThreadStart(trdToMidRenderingProcess));
            mm.Start();

            return ret;
        }

        private static WaveWriter toWavWaveWriter = null;
        private static MidWriter toMidWriter = null;

        private static void trdToWavRenderingProcess()
        {
            short[] buf = new short[2];
            int offset = 0;
            int sampleCount = 1;

            Enq bEnq = chipRegister.enq;
            chipRegister.enq = EnqToWav;
            try
            {

                while (!driver.Stopped && GetVgmCurLoopCounter() < setting.other.LoopTimes && !waveModeAbort)
                {
                    mds.Update(buf, offset, sampleCount, playToWavOneProc);
                    EmuSeqCounter++;
                    toWavWaveWriter.Write(buf, offset, sampleCount * 2);
                }

            }
            finally
            {
                Stop(SendMode.Both);
                toWavWaveWriter.Close();

                chipRegister.enq = bEnq;
                waveMode = false;
            }
        }

        private static void playToWavOneProc()
        {
            //
            if (EmuSeqCounter >= DriverSeqCounter)
                driver.oneFrameProc();
        }

        private static void trdToMidRenderingProcess()
        {
            short[] buf = new short[2];
            int offset = 0;
            int sampleCount = 1;

            Enq bEnq = chipRegister.enq;
            chipRegister.enq = EnqToMid;
            try
            {

                while (!driver.Stopped && GetVgmCurLoopCounter() < 2 && !waveModeAbort)
                {
                    mds.Update(buf, offset, sampleCount, playToMidOneProc);
                    EmuSeqCounter++;
                    toMidWriter.Write(buf, offset, sampleCount * 2);
                }

            }
            finally
            {
                Stop(SendMode.Both);
                toMidWriter.Close();

                chipRegister.enq = bEnq;
                waveMode = false;
            }
        }

        private static void playToMidOneProc()
        {
            //
            if (EmuSeqCounter >= DriverSeqCounter)
                driver.oneFrameProc();
        }

        public static bool EnqToWav(outDatum od, long Counter, Chip Chip, EnmDataType Type, int Address, int Data, object ExData)
        {
            if (Chip == null) return false;
            if (Chip.Device == EnmZGMDevice.Conductor) return false;
            if (Type == EnmDataType.None) return false;

            chipRegister.SendChipData(Counter, Chip, Type, Address, Data, ExData);
            return false;
        }

        public static bool EnqToMid(outDatum od, long Counter, Chip Chip, EnmDataType Type, int Address, int Data, object ExData)
        {
            if (Chip == null) return false;
            if (Chip.Device == EnmZGMDevice.Conductor) return false;
            if (Type == EnmDataType.None) return false;
            if (toMidWriter == null) return false;

            toMidWriter.SendChipData(Counter, Chip, Type, Address, Data, ExData);
            return false;
        }


    }


}
