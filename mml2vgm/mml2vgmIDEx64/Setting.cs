﻿using mml2vgmIDEx64.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace mml2vgmIDEx64
{
    [Serializable]
    public class Setting
    {
        public Setting()
        {
        }

        private OutputDevice _outputDevice = new OutputDevice();
        public OutputDevice outputDevice
        {
            get
            {
                return _outputDevice;
            }

            set
            {
                _outputDevice = value;
            }
        }

        private ChipType _AY8910Type = new ChipType();
        public ChipType AY8910Type
        {
            get
            {
                return _AY8910Type;
            }

            set
            {
                _AY8910Type = value;
            }
        }

        private ChipType _AY8910SType = new ChipType();
        public ChipType AY8910SType
        {
            get
            {
                return _AY8910SType;
            }

            set
            {
                _AY8910SType = value;
            }
        }

        private ChipType _POKEYType = new ChipType();
        public ChipType POKEYType
        {
            get
            {
                return _POKEYType;
            }

            set
            {
                _POKEYType = value;
            }
        }

        private ChipType _POKEYSType = new ChipType();
        public ChipType POKEYSType
        {
            get
            {
                return _POKEYSType;
            }

            set
            {
                _POKEYSType = value;
            }
        }

        private ChipType _YM2151Type = new ChipType();
        public ChipType YM2151Type
        {
            get
            {
                return _YM2151Type;
            }

            set
            {
                _YM2151Type = value;
            }
        }

        private ChipType _YM2203Type = new ChipType();
        public ChipType YM2203Type
        {
            get
            {
                return _YM2203Type;
            }

            set
            {
                _YM2203Type = value;
            }
        }

        private ChipType _YM2413Type = new ChipType();
        public ChipType YM2413Type
        {
            get
            {
                return _YM2413Type;
            }

            set
            {
                _YM2413Type = value;
            }
        }

        private ChipType _HuC6280Type = new ChipType();
        public ChipType HuC6280Type
        {
            get
            {
                return _HuC6280Type;
            }

            set
            {
                _HuC6280Type = value;
            }
        }

        private ChipType _K051649Type = new ChipType();
        public ChipType K051649Type
        {
            get
            {
                return _K051649Type;
            }

            set
            {
                _K051649Type = value;
            }
        }

        private ChipType _K053260Type = new ChipType();
        public ChipType K053260Type
        {
            get
            {
                return _K053260Type;
            }

            set
            {
                _K053260Type = value;
            }
        }

        private ChipType _K054539Type = new ChipType();
        public ChipType K054539Type
        {
            get
            {
                return _K054539Type;
            }

            set
            {
                _K054539Type = value;
            }
        }

        private ChipType _PPZ8Type = new ChipType();
        public ChipType PPZ8Type
        {
            get
            {
                return _PPZ8Type;
            }

            set
            {
                _PPZ8Type = value;
            }
        }

        private ChipType _PPSDRVType = new ChipType();
        public ChipType PPSDRVType
        {
            get
            {
                return _PPSDRVType;
            }

            set
            {
                _PPSDRVType = value;
            }
        }

        private ChipType _CS4231Type = new ChipType();
        public ChipType CS4231Type
        {
            get
            {
                return _CS4231Type;
            }

            set
            {
                _CS4231Type = value;
            }
        }

        private ChipType _QSoundType = new ChipType();
        public ChipType QSoundType
        {
            get
            {
                return _QSoundType;
            }

            set
            {
                _QSoundType = value;
            }
        }

        private ChipType _RF5C164Type = new ChipType();
        public ChipType RF5C164Type
        {
            get
            {
                return _RF5C164Type;
            }

            set
            {
                _RF5C164Type = value;
            }
        }

        private ChipType _DMGType = new ChipType();
        public ChipType DMGType
        {
            get
            {
                return _DMGType;
            }

            set
            {
                _DMGType = value;
            }
        }

        private ChipType _DMGSType = new ChipType();
        public ChipType DMGSType
        {
            get
            {
                return _DMGSType;
            }

            set
            {
                _DMGSType = value;
            }
        }

        private ChipType _NESType = new ChipType();
        public ChipType NESType
        {
            get
            {
                return _NESType;
            }

            set
            {
                _NESType = value;
            }
        }

        private ChipType _NESSType = new ChipType();
        public ChipType NESSType
        {
            get
            {
                return _NESSType;
            }

            set
            {
                _NESSType = value;
            }
        }

        private ChipType _VRC6Type = new ChipType();
        public ChipType VRC6Type
        {
            get
            {
                return _VRC6Type;
            }

            set
            {
                _VRC6Type = value;
            }
        }

        private ChipType _VRC6SType = new ChipType();
        public ChipType VRC6SType
        {
            get
            {
                return _VRC6SType;
            }

            set
            {
                _VRC6SType = value;
            }
        }

        private ChipType _YM2413SType = new ChipType();
        public ChipType YM2413SType
        {
            get
            {
                return _YM2413SType;
            }

            set
            {
                _YM2413SType = value;
            }
        }

        private ChipType _YM2608Type = new ChipType();
        public ChipType YM2608Type
        {
            get
            {
                return _YM2608Type;
            }

            set
            {
                _YM2608Type = value;
            }
        }

        private ChipType _YM2609Type = new ChipType();
        public ChipType YM2609Type
        {
            get
            {
                return _YM2609Type;
            }

            set
            {
                _YM2609Type = value;
            }
        }
        private ChipType _GigatronType = new ChipType();
        public ChipType GigatronType
        {
            get
            {
                return _GigatronType;
            }

            set
            {
                _GigatronType = value;
            }
        }
        private ChipType _GigatronSType = new ChipType();
        public ChipType GigatronSType
        {
            get
            {
                return _GigatronSType;
            }

            set
            {
                _GigatronSType = value;
            }
        }

        private ChipType _YM2610Type = new ChipType();
        public ChipType YM2610Type
        {
            get
            {
                return _YM2610Type;
            }

            set
            {
                _YM2610Type = value;
            }
        }

        private ChipType _YMF262Type = new ChipType();
        public ChipType YMF262Type
        {
            get
            {
                return _YMF262Type;
            }

            set
            {
                _YMF262Type = value;
            }
        }

        private ChipType _YMF271Type = new ChipType();
        public ChipType YMF271Type
        {
            get
            {
                return _YMF271Type;
            }

            set
            {
                _YMF271Type = value;
            }
        }

        private ChipType _YMF278BType = new ChipType();
        public ChipType YMF278BType
        {
            get
            {
                return _YMF278BType;
            }

            set
            {
                _YMF278BType = value;
            }
        }

        private ChipType _YMZ280BType = new ChipType();
        public ChipType YMZ280BType
        {
            get
            {
                return _YMZ280BType;
            }

            set
            {
                _YMZ280BType = value;
            }
        }

        private ChipType _YM2612Type = new ChipType();
        public ChipType YM2612Type
        {
            get
            {
                return _YM2612Type;
            }

            set
            {
                _YM2612Type = value;
            }
        }

        private ChipType _SN76489Type = new ChipType();
        public ChipType SN76489Type
        {
            get
            {
                return _SN76489Type;
            }

            set
            {
                _SN76489Type = value;
            }
        }

        private ChipType _YM2151SType = new ChipType();
        public ChipType YM2151SType
        {
            get
            {
                return _YM2151SType;
            }

            set
            {
                _YM2151SType = value;
            }
        }

        private ChipType _YM2203SType = new ChipType();
        public ChipType YM2203SType
        {
            get
            {
                return _YM2203SType;
            }

            set
            {
                _YM2203SType = value;
            }
        }

        private ChipType _YM2608SType = new ChipType();
        public ChipType YM2608SType
        {
            get
            {
                return _YM2608SType;
            }

            set
            {
                _YM2608SType = value;
            }
        }

        private ChipType _YM2609SType = new ChipType();
        public ChipType YM2609SType
        {
            get
            {
                return _YM2609SType;
            }

            set
            {
                _YM2609SType = value;
            }
        }

        private ChipType _YM2610SType = new ChipType();
        public ChipType YM2610SType
        {
            get
            {
                return _YM2610SType;
            }

            set
            {
                _YM2610SType = value;
            }
        }

        private ChipType _YM2612SType = new ChipType();
        public ChipType YM2612SType
        {
            get
            {
                return _YM2612SType;
            }

            set
            {
                _YM2612SType = value;
            }
        }

        private ChipType _YMF262SType = new ChipType();
        public ChipType YMF262SType
        {
            get
            {
                return _YMF262SType;
            }

            set
            {
                _YMF262SType = value;
            }
        }

        private ChipType _YMF271SType = new ChipType();
        public ChipType YMF271SType
        {
            get
            {
                return _YMF271SType;
            }

            set
            {
                _YMF271SType = value;
            }
        }

        private ChipType _YMF278BSType = new ChipType();
        public ChipType YMF278BSType
        {
            get
            {
                return _YMF278BSType;
            }

            set
            {
                _YMF278BSType = value;
            }
        }

        private ChipType _YMZ280BSType = new ChipType();
        public ChipType YMZ280BSType
        {
            get
            {
                return _YMZ280BSType;
            }

            set
            {
                _YMZ280BSType = value;
            }
        }

        private ChipType _SN76489SType = new ChipType();
        public ChipType SN76489SType
        {
            get
            {
                return _SN76489SType;
            }

            set
            {
                _SN76489SType = value;
            }
        }

        private ChipType _HuC6280SType = new ChipType();
        public ChipType HuC6280SType
        {
            get
            {
                return _HuC6280SType;
            }

            set
            {
                _HuC6280SType = value;
            }
        }

        private ChipType _K051649SType = new ChipType();
        public ChipType K051649SType
        {
            get
            {
                return _K051649SType;
            }

            set
            {
                _K051649SType = value;
            }
        }

        private ChipType _K053260SType = new ChipType();
        public ChipType K053260SType
        {
            get
            {
                return _K053260SType;
            }

            set
            {
                _K053260SType = value;
            }
        }

        private ChipType _K054539SType = new ChipType();
        public ChipType K054539SType
        {
            get
            {
                return _K054539SType;
            }

            set
            {
                _K054539SType = value;
            }
        }

        private ChipType _YM3526Type = new ChipType();
        public ChipType YM3526Type
        {
            get
            {
                return _YM3526Type;
            }

            set
            {
                _YM3526Type = value;
            }
        }

        private ChipType _YM3526SType = new ChipType();
        public ChipType YM3526SType
        {
            get
            {
                return _YM3526SType;
            }

            set
            {
                _YM3526SType = value;
            }
        }

        private ChipType _YM3812Type = new ChipType();
        public ChipType YM3812Type
        {
            get
            {
                return _YM3812Type;
            }

            set
            {
                _YM3812Type = value;
            }
        }

        private ChipType _YM3812SType = new ChipType();
        public ChipType YM3812SType
        {
            get
            {
                return _YM3812SType;
            }

            set
            {
                _YM3812SType = value;
            }
        }

        private ChipType _Y8950Type = new ChipType();
        public ChipType Y8950Type
        {
            get
            {
                return _Y8950Type;
            }

            set
            {
                _Y8950Type = value;
            }
        }

        private ChipType _Y8950SType = new ChipType();
        public ChipType Y8950SType
        {
            get
            {
                return _Y8950SType;
            }

            set
            {
                _Y8950SType = value;
            }
        }

        private ChipType _C140Type = new ChipType();
        public ChipType C140Type
        {
            get
            {
                return _C140Type;
            }

            set
            {
                _C140Type = value;
            }
        }

        private ChipType _C140SType = new ChipType();
        public ChipType C140SType
        {
            get
            {
                return _C140SType;
            }

            set
            {
                _C140SType = value;
            }
        }

        private ChipType _C352Type = new ChipType();
        public ChipType C352Type
        {
            get
            {
                return _C352Type;
            }

            set
            {
                _C352Type = value;
            }
        }

        private ChipType _C352SType = new ChipType();
        public ChipType C352SType
        {
            get
            {
                return _C352SType;
            }

            set
            {
                _C352SType = value;
            }
        }

        private ChipType _SEGAPCMType = new ChipType();
        public ChipType SEGAPCMType
        {
            get
            {
                return _SEGAPCMType;
            }

            set
            {
                _SEGAPCMType = value;
            }
        }

        private ChipType _SEGAPCMSType = new ChipType();
        public ChipType SEGAPCMSType
        {
            get
            {
                return _SEGAPCMSType;
            }

            set
            {
                _SEGAPCMSType = value;
            }
        }

        private bool _IsManualDetect = false;
        public bool IsManualDetect
        {
            get
            {
                return _IsManualDetect;
            }

            set
            {
                _IsManualDetect = value;
            }
        }

        private int _AutoDetectModuleType = 1;
        public int AutoDetectModuleType
        {
            get
            {
                return _AutoDetectModuleType;
            }

            set
            {
                _AutoDetectModuleType = value;
            }
        }

        private int _LatencyEmulation = 0;
        public int LatencyEmulation
        {
            get
            {
                return _LatencyEmulation;
            }

            set
            {
                _LatencyEmulation = value;
            }
        }

        private int _LatencySCCI = 0;
        public int LatencySCCI
        {
            get
            {
                return _LatencySCCI;
            }

            set
            {
                _LatencySCCI = value;
            }
        }

        private bool _HiyorimiMode = true;
        public bool HiyorimiMode
        {
            get
            {
                return _HiyorimiMode;
            }

            set
            {
                _HiyorimiMode = value;
            }
        }

        private bool _Debug_DispFrameCounter = false;
        public bool Debug_DispFrameCounter
        {
            get
            {
                return _Debug_DispFrameCounter;
            }

            set
            {
                _Debug_DispFrameCounter = value;
            }
        }

        private Other _other = new Other();
        public Other other
        {
            get
            {
                return _other;
            }

            set
            {
                _other = value;
            }
        }

        private Export _export = new Export();
        public Export export
        {
            get
            {
                return _export;
            }

            set
            {
                _export = value;
            }
        }

        private MMLParameters _MMLParameter = new MMLParameters();
        public MMLParameters MMLParameter
        {
            get
            {
                return _MMLParameter;
            }

            set
            {
                _MMLParameter = value;
            }
        }

        private Sien _sien = new Sien();
        public Sien sien
        {
            get
            {
                return _sien;
            }

            set
            {
                _sien = value;
            }
        }

        private Balance _balance = new Balance();
        public Balance balance
        {
            get
            {
                return _balance;
            }

            set
            {
                _balance = value;
            }
        }

        private Location _location = new Location();
        public Location location
        {
            get
            {
                return _location;
            }

            set
            {
                _location = value;
            }
        }

        private string _dockingState = "";
        public string dockingState
        {
            get
            {
                return _dockingState;
            }

            set
            {
                _dockingState = value;
            }
        }

        private MidiExport _midiExport = new MidiExport();
        public MidiExport midiExport
        {
            get
            {
                return _midiExport;
            }

            set
            {
                _midiExport = value;
            }
        }

        private MidiKbd _midiKbd = new MidiKbd();
        public MidiKbd midiKbd
        {
            get
            {
                return _midiKbd;
            }

            set
            {
                _midiKbd = value;
            }
        }

        //private Vst _vst = new Vst();
        //public Vst vst
        //{
        //    get
        //    {
        //        return _vst;
        //    }

        //    set
        //    {
        //        _vst = value;
        //    }
        //}

        private MidiOut _midiOut = new MidiOut();
        public MidiOut midiOut
        {
            get
            {
                return _midiOut;
            }

            set
            {
                _midiOut = value;
            }
        }

        //private NSF _nsf = new NSF();
        //public NSF nsf
        //{
        //    get
        //    {
        //        return _nsf;
        //    }

        //    set
        //    {
        //        _nsf = value;
        //    }
        //}

        //private SID _sid = new SID();
        //public SID sid
        //{
        //    get
        //    {
        //        return _sid;
        //    }

        //    set
        //    {
        //        _sid = value;
        //    }
        //}

        private NukedOPN2 _NukedOPN2 = new NukedOPN2();
        public NukedOPN2 nukedOPN2
        {
            get
            {
                return _NukedOPN2;
            }

            set
            {
                _NukedOPN2 = value;
            }
        }

        private GensOption _GensOption = new GensOption();
        public GensOption gensOption
        {
            get
            {
                return _GensOption;
            }

            set
            {
                _GensOption = value;
            }
        }

        private AutoBalance _autoBalance = new AutoBalance();
        public AutoBalance autoBalance
        {
            get => _autoBalance; set => _autoBalance = value;
        }

        private PMDDotNET _PMDDotNET = new PMDDotNET();
        public PMDDotNET pmdDotNET
        {
            get
            {
                return _PMDDotNET;
            }

            set
            {
                _PMDDotNET = value;
            }
        }

        private Setting.ShortCutKey _shortCutKey = null;
        public Setting.ShortCutKey shortCutKey { get => _shortCutKey; set => _shortCutKey = value; }


        public KeyBoardHook keyBoardHook { get => _keyBoardHook; set => _keyBoardHook = value; }
        public ColorScheme ColorScheme { get => _colorScheme; set => _colorScheme = value; }
        private KeyBoardHook _keyBoardHook = new KeyBoardHook();

        private ColorScheme _colorScheme = new ColorScheme();

        private bool _InfiniteOfflineMode = true;
        public bool InfiniteOfflineMode { get => _InfiniteOfflineMode; set => _InfiniteOfflineMode = value; }

        private bool _UseSien = true;
        public bool UseSien { get => _UseSien; set => _UseSien = value; }

        public bool unuseRealChip { get; set; }
        public MusicInterruptTimer musicInterruptTimer { get; set; } = MusicInterruptTimer.StopWatch;

        public bool OfflineMode = false;

        [Serializable]
        public class OutputDevice
        {

            private int _SampleRate = 44100;
            public int SampleRate
            {
                get
                {
                    return _SampleRate;
                }

                set
                {
                    _SampleRate = value;
                }
            }

            private int _DeviceType = 0;
            public int DeviceType
            {
                get
                {
                    return _DeviceType;
                }

                set
                {
                    _DeviceType = value;
                }
            }

            private int _Latency = 300;
            public int Latency
            {
                get
                {
                    return _Latency;
                }

                set
                {
                    _Latency = value;
                }
            }

            private int _WaitTime = 500;
            public int WaitTime
            {
                get
                {
                    return _WaitTime;
                }

                set
                {
                    _WaitTime = value;
                }
            }

            private string _WaveOutDeviceName = "";
            public string WaveOutDeviceName
            {
                get
                {
                    return _WaveOutDeviceName;
                }

                set
                {
                    _WaveOutDeviceName = value;
                }
            }

            private string _DirectSoundDeviceName = "";
            public string DirectSoundDeviceName
            {
                get
                {
                    return _DirectSoundDeviceName;
                }

                set
                {
                    _DirectSoundDeviceName = value;
                }
            }

            private string _WasapiDeviceName = "";
            public string WasapiDeviceName
            {
                get
                {
                    return _WasapiDeviceName;
                }

                set
                {
                    _WasapiDeviceName = value;
                }
            }

            private bool _WasapiShareMode = true;
            public bool WasapiShareMode
            {
                get
                {
                    return _WasapiShareMode;
                }

                set
                {
                    _WasapiShareMode = value;
                }
            }

            private string _AsioDeviceName = "";
            public string AsioDeviceName
            {
                get
                {
                    return _AsioDeviceName;
                }

                set
                {
                    _AsioDeviceName = value;
                }
            }

            public OutputDevice Copy()
            {
                OutputDevice outputDevice = new OutputDevice();
                outputDevice.SampleRate = this.SampleRate;
                outputDevice.DeviceType = this.DeviceType;
                outputDevice.Latency = this.Latency;
                outputDevice.WaitTime = this.WaitTime;
                outputDevice.WaveOutDeviceName = this.WaveOutDeviceName;
                outputDevice.DirectSoundDeviceName = this.DirectSoundDeviceName;
                outputDevice.WasapiDeviceName = this.WasapiDeviceName;
                outputDevice.WasapiShareMode = this.WasapiShareMode;
                outputDevice.AsioDeviceName = this.AsioDeviceName;

                return outputDevice;
            }
        }

        [Serializable]
        public class ChipType
        {
            private bool _UseEmu = true;
            public bool UseEmu
            {
                get
                {
                    return _UseEmu;
                }

                set
                {
                    _UseEmu = value;
                }
            }

            private bool _UseEmu2 = false;
            public bool UseEmu2
            {
                get
                {
                    return _UseEmu2;
                }

                set
                {
                    _UseEmu2 = value;
                }
            }

            private bool _UseEmu3 = false;
            public bool UseEmu3
            {
                get
                {
                    return _UseEmu3;
                }

                set
                {
                    _UseEmu3 = value;
                }
            }


            private bool _UseScci = false;
            public bool UseScci
            {
                get
                {
                    return _UseScci;
                }

                set
                {
                    _UseScci = value;
                }
            }

            private string _InterfaceName = "";
            public string InterfaceName
            {
                get
                {
                    return _InterfaceName;
                }

                set
                {
                    _InterfaceName = value;
                }
            }

            private int _SoundLocation = -1;
            public int SoundLocation
            {
                get
                {
                    return _SoundLocation;
                }

                set
                {
                    _SoundLocation = value;
                }
            }

            private int _BusID = -1;
            public int BusID
            {
                get
                {
                    return _BusID;
                }

                set
                {
                    _BusID = value;
                }
            }

            private int _SoundChip = -1;
            public int SoundChip
            {
                get
                {
                    return _SoundChip;
                }

                set
                {
                    _SoundChip = value;
                }
            }

            private string _ChipName = "";
            public string ChipName
            {
                get
                {
                    return _ChipName;
                }

                set
                {
                    _ChipName = value;
                }
            }


            private bool _UseScci2 = false;
            public bool UseScci2
            {
                get
                {
                    return _UseScci2;
                }

                set
                {
                    _UseScci2 = value;
                }
            }

            private string _InterfaceName2A = "";
            public string InterfaceName2A
            {
                get
                {
                    return _InterfaceName2A;
                }

                set
                {
                    _InterfaceName2A = value;
                }
            }

            private int _SoundLocation2A = -1;
            public int SoundLocation2A
            {
                get
                {
                    return _SoundLocation2A;
                }

                set
                {
                    _SoundLocation2A = value;
                }
            }

            private int _BusID2A = -1;
            public int BusID2A
            {
                get
                {
                    return _BusID2A;
                }

                set
                {
                    _BusID2A = value;
                }
            }

            private int _SoundChip2A = -1;
            public int SoundChip2A
            {
                get
                {
                    return _SoundChip2A;
                }

                set
                {
                    _SoundChip2A = value;
                }
            }

            private string _ChipName2A = "";
            public string ChipName2A
            {
                get
                {
                    return _ChipName2A;
                }

                set
                {
                    _ChipName2A = value;
                }
            }

            private int _Type = 0;
            public int Type
            {
                get
                {
                    return _Type;
                }

                set
                {
                    _Type = value;
                }
            }

            private int _Type2A = 0;
            public int Type2A
            {
                get
                {
                    return _Type2A;
                }

                set
                {
                    _Type2A = value;
                }
            }

            private int _Type2B = 0;
            public int Type2B
            {
                get
                {
                    return _Type2B;
                }

                set
                {
                    _Type2B = value;
                }
            }

            private string _InterfaceName2B = "";
            public string InterfaceName2B
            {
                get
                {
                    return _InterfaceName2B;
                }

                set
                {
                    _InterfaceName2B = value;
                }
            }

            private int _SoundLocation2B = -1;
            public int SoundLocation2B
            {
                get
                {
                    return _SoundLocation2B;
                }

                set
                {
                    _SoundLocation2B = value;
                }
            }

            private int _BusID2B = -1;
            public int BusID2B
            {
                get
                {
                    return _BusID2B;
                }

                set
                {
                    _BusID2B = value;
                }
            }

            private int _SoundChip2B = -1;
            public int SoundChip2B
            {
                get
                {
                    return _SoundChip2B;
                }

                set
                {
                    _SoundChip2B = value;
                }
            }

            private string _ChipName2B = "";
            public string ChipName2B
            {
                get
                {
                    return _ChipName2B;
                }

                set
                {
                    _ChipName2B = value;
                }
            }


            private bool _UseWait = true;
            public bool UseWait
            {
                get
                {
                    return _UseWait;
                }

                set
                {
                    _UseWait = value;
                }
            }

            private bool _UseWaitBoost = false;
            public bool UseWaitBoost
            {
                get
                {
                    return _UseWaitBoost;
                }

                set
                {
                    _UseWaitBoost = value;
                }
            }

            private bool _OnlyPCMEmulation = false;
            public bool OnlyPCMEmulation
            {
                get
                {
                    return _OnlyPCMEmulation;
                }

                set
                {
                    _OnlyPCMEmulation = value;
                }
            }

            private int _LatencyForEmulation = 0;
            public int LatencyForEmulation
            {
                get
                {
                    return _LatencyForEmulation;
                }

                set
                {
                    _LatencyForEmulation = value;
                }
            }

            private int _LatencyForScci = 0;
            public int LatencyForScci
            {
                get
                {
                    return _LatencyForScci;
                }

                set
                {
                    _LatencyForScci = value;
                }
            }

            private bool _exchgPAN = false;
            public bool exchgPAN { get; set; }


            public ChipType Copy()
            {
                ChipType ct = new ChipType();
                ct.UseEmu = this.UseEmu;
                ct.UseEmu2 = this.UseEmu2;
                ct.UseEmu3 = this.UseEmu3;
                ct.UseScci = this.UseScci;

                ct.SoundLocation = this.SoundLocation;
                ct.Type = this.Type;
                ct.BusID = this.BusID;
                ct.InterfaceName = this.InterfaceName;
                ct.SoundChip = this.SoundChip;
                ct.ChipName = this.ChipName;
                ct.UseScci2 = this.UseScci2;

                ct.SoundLocation2A = this.SoundLocation2A;
                ct.Type2A = this.Type2A;
                ct.InterfaceName2A = this.InterfaceName2A;
                ct.BusID2A = this.BusID2A;
                ct.SoundChip2A = this.SoundChip2A;
                ct.ChipName2A = this.ChipName2A;

                ct.SoundLocation2B = this.SoundLocation2B;
                ct.Type2B = this.Type2B;
                ct.InterfaceName2B = this.InterfaceName2B;
                ct.BusID2B = this.BusID2B;
                ct.SoundChip2B = this.SoundChip2B;
                ct.ChipName2B = this.ChipName2B;

                ct.UseWait = this.UseWait;
                ct.UseWaitBoost = this.UseWaitBoost;
                ct.OnlyPCMEmulation = this.OnlyPCMEmulation;
                ct.LatencyForEmulation = this.LatencyForEmulation;
                ct.LatencyForScci = this.LatencyForScci;
                ct.Type = this.Type;

                ct.exchgPAN = this.exchgPAN;

                return ct;
            }
        }

        [Serializable]
        public class Other
        {
            private bool _UseLoopTimes = true;
            public bool UseLoopTimes
            {
                get
                {
                    return _UseLoopTimes;
                }

                set
                {
                    _UseLoopTimes = value;
                }
            }

            private int _LoopTimes = 2;
            public int LoopTimes
            {
                get
                {
                    return _LoopTimes;
                }

                set
                {
                    _LoopTimes = value;
                }
            }


            private bool _UseGetInst = true;
            public bool UseGetInst
            {
                get
                {
                    return _UseGetInst;
                }

                set
                {
                    _UseGetInst = value;
                }
            }

            private string _DefaultDataPath = "";
            public string DefaultDataPath
            {
                get
                {
                    return _DefaultDataPath;
                }

                set
                {
                    _DefaultDataPath = value;
                }
            }

            //private EnmInstFormat _InstFormat = EnmInstFormat.MML2VGM;
            //public EnmInstFormat InstFormat
            //{
            //    get
            //    {
            //        return _InstFormat;
            //    }

            //    set
            //    {
            //        _InstFormat = value;
            //    }
            //}

            private int _Zoom = 1;
            public int Zoom
            {
                get
                {
                    return _Zoom;
                }

                set
                {
                    _Zoom = value;
                }
            }

            private int _ScreenFrameRate = 60;
            public int ScreenFrameRate
            {
                get
                {
                    return _ScreenFrameRate;
                }

                set
                {
                    _ScreenFrameRate = value;
                }
            }

            private bool _AutoOpen = false;
            public bool AutoOpen
            {
                get
                {
                    return _AutoOpen;
                }

                set
                {
                    _AutoOpen = value;
                }
            }

            private bool _DumpSwitch = false;
            public bool DumpSwitch
            {
                get
                {
                    return _DumpSwitch;
                }

                set
                {
                    _DumpSwitch = value;
                }
            }

            private string _DumpPath = "";
            public string DumpPath
            {
                get
                {
                    return _DumpPath;
                }

                set
                {
                    _DumpPath = value;
                }
            }

            private bool _WavSwitch = false;
            public bool WavSwitch
            {
                get
                {
                    return _WavSwitch;
                }

                set
                {
                    _WavSwitch = value;
                }
            }

            private string _WavPath = "";
            public string WavPath
            {
                get
                {
                    return _WavPath;
                }

                set
                {
                    _WavPath = value;
                }
            }

            private int _FilterIndex = 0;
            public int FilterIndex
            {
                get
                {
                    return _FilterIndex;
                }

                set
                {
                    _FilterIndex = value;
                }
            }

            private string _TextExt = "txt;doc;hed";
            public string TextExt { get => _TextExt; set => _TextExt = value; }

            private string _MMLExt = "mml;gwi";
            public string MMLExt { get => _MMLExt; set => _MMLExt = value; }

            private string _ImageExt = "jpg;gif;png;mag";
            public string ImageExt { get => _ImageExt; set => _ImageExt = value; }

            private bool _InitAlways = false;
            public bool InitAlways { get => _InitAlways; set => _InitAlways = value; }

            private bool _EmptyPlayList = false;
            public bool EmptyPlayList { get => _EmptyPlayList; set => _EmptyPlayList = value; }

            private bool _ChangeEnterCode = true;
            public bool ChangeEnterCode { get => _ChangeEnterCode; set => _ChangeEnterCode = value; }

            private int _Opacity = 100;
            public int Opacity
            {
                get
                {
                    _Opacity = Common.Range(_Opacity, 10, 100);
                    return _Opacity;
                }
                set
                {
                    _Opacity = value;
                    _Opacity = Common.Range(_Opacity, 10, 100);
                }
            }
            private string _TextFontName = "Consolas";
            public string TextFontName { get => _TextFontName; set => _TextFontName = value; }
            private float _TextFontSize = 12.0f;
            public float TextFontSize { get => _TextFontSize; set => _TextFontSize = value; }
            private FontStyle _TextFontStyle = FontStyle.Regular;
            public FontStyle TextFontStyle { get => _TextFontStyle; set => _TextFontStyle = value; }
            public int TabWidth { get => _TabWidth; set => _TabWidth = value; }
            private int _TabWidth = 8;
            private string[] _GwiFileHistory = null;
            private string _Tutorial = "Tutorial.txt";
            private string _CommandManual = "mml2vgm_MMLCommandMemo.txt";
            private string _CommandManualMucom = "";
            private string _CommandManualPMD="";
            private string _CommandManualM98 = "m98コマンド・リファレンス.pdf";
            private string _CommandManualMuap = "";
            private bool _sinWaveGen=false;
            public bool sinWaveGen { get => _sinWaveGen; set => _sinWaveGen = value; }

            public string Tutorial { get => _Tutorial; set => _Tutorial = value; }
            public string CommandManual { get => _CommandManual; set => _CommandManual = value; }
            public string CommandManualMucom { get => _CommandManualMucom; set => _CommandManualMucom = value; }
            public string CommandManualPMD { get => _CommandManualPMD; set => _CommandManualPMD = value; }
            public string CommandManualM98 { get => _CommandManualM98; set => _CommandManualM98 = value; }
            public string CommandManualMuap { get => _CommandManualMuap; set => _CommandManualMuap = value; }

            public string[] GwiFileHistory { get => _GwiFileHistory; set => _GwiFileHistory = value; }

            public bool LogWarning { get; set; } = false;

            public bool PlayDeviceCB { get; set; } = false;

            public int LogLevel { get; set; } = 8;//8:INFO
            public List<string> SearchWordHistory { get; set; }
            public bool SearchWordCaseSenstivity { get; set; }
            public List<string> ReplaceToWordHistory { get; set; }
            private bool _ClearHistory = true;
            public bool ClearHistory { get => _ClearHistory; set => _ClearHistory = value; }
            private bool _UseScript = true;
            public bool UseScript { get => _UseScript; set => _UseScript = value; }
            private bool _UseMucomDotNET = true;
            public bool UseMucomDotNET { get => _UseMucomDotNET; set => _UseMucomDotNET = value; }
            private bool _UsePMDDotNET = true;
            public bool UsePMDDotNET { get => _UsePMDDotNET; set => _UsePMDDotNET = value; }
            private bool _UseMoonDriverDotNET = true;
            public bool HilightOn { get => _HilightOn; set => _HilightOn = value; }
            private bool _HilightOn = true;

            public bool UseMoonDriverDotNET { get => _UseMoonDriverDotNET; set => _UseMoonDriverDotNET = value; }
            public bool UseMuapDotNET { get => _UseMuapDotNET; set => _UseMuapDotNET = value; }
            private bool _UseMuapDotNET = true;
            public bool UseHistoryBackUp { get; set; } = false;
            public int HistoryBackUpKeepFileCount { get; set; } = 5;
            public bool DispWarningMessage { get; set; } = false;

            public Other Copy()
            {
                Other other = new Other();
                other.UseLoopTimes = this.UseLoopTimes;
                other.LoopTimes = this.LoopTimes;
                other.UseGetInst = this.UseGetInst;
                other.DefaultDataPath = this.DefaultDataPath;
                //other.InstFormat = this.InstFormat;
                other.Zoom = this.Zoom;
                other.ScreenFrameRate = this.ScreenFrameRate;
                other.AutoOpen = this.AutoOpen;
                other.DumpSwitch = this.DumpSwitch;
                other.DumpPath = this.DumpPath;
                other.WavSwitch = this.WavSwitch;
                other.WavPath = this.WavPath;
                other.FilterIndex = this.FilterIndex;
                other.TextExt = this.TextExt;
                other.MMLExt = this.MMLExt;
                other.ImageExt = this.ImageExt;
                other.InitAlways = this.InitAlways;
                other.EmptyPlayList = this.EmptyPlayList;
                other.ChangeEnterCode = this.ChangeEnterCode;
                other.Opacity = this.Opacity;
                other.TextFontName = this.TextFontName;
                other.TextFontSize = this.TextFontSize;
                other.TextFontStyle = this.TextFontStyle;
                other.GwiFileHistory = this.GwiFileHistory;
                other.LogWarning = this.LogWarning;
                other.PlayDeviceCB = this.PlayDeviceCB;
                other.LogLevel = this.LogLevel;
                other.Tutorial = this.Tutorial;
                other.CommandManual = this.CommandManual;
                other.CommandManualMucom = this.CommandManualMucom;
                other.CommandManualPMD = this.CommandManualPMD;
                other.CommandManualM98 = this.CommandManualM98;
                other.CommandManualMuap = this.CommandManualMuap;
                other.sinWaveGen = this.sinWaveGen;
                other.ClearHistory = this.ClearHistory;
                other.UseScript = this.UseScript;
                other.UseMucomDotNET = this.UseMucomDotNET;
                other.UsePMDDotNET = this.UsePMDDotNET;
                other.HilightOn = this.HilightOn;
                other.UseMoonDriverDotNET = this.UseMoonDriverDotNET;
                other.UseMuapDotNET = this.UseMuapDotNET;
                other.SearchWordHistory = this.SearchWordHistory;
                other.SearchWordCaseSenstivity = this.SearchWordCaseSenstivity;
                other.ReplaceToWordHistory = this.ReplaceToWordHistory;
                other.UseHistoryBackUp = this.UseHistoryBackUp;
                other.HistoryBackUpKeepFileCount = this.HistoryBackUpKeepFileCount;
                other.DispWarningMessage = this.DispWarningMessage;

                return other;
            }
        }

        [Serializable]
        public class Export
        {
            private bool _FixedExportPlace = false;
            public bool FixedExportPlace
            {
                get
                {
                    return _FixedExportPlace;
                }

                set
                {
                    _FixedExportPlace = value;
                }
            }

            private string _FixedExportPlacePath = "";
            public string FixedExportPlacePath
            {
                get
                {
                    return _FixedExportPlacePath;
                }

                set
                {
                    _FixedExportPlacePath = value;
                }
            }

            private bool _AlwaysAsksForLoopCounts  = false;
            public bool AlwaysAsksForLoopCounts {
                get
                {
                    return _AlwaysAsksForLoopCounts;
                }
                set
                {
                    _AlwaysAsksForLoopCounts = value;
                }
            }

            public Export Copy()
            {
                Export export = new Export();
                export.FixedExportPlace = this.FixedExportPlace;
                export.FixedExportPlacePath = this.FixedExportPlacePath;
                export.AlwaysAsksForLoopCounts = this.AlwaysAsksForLoopCounts;

                return export;
            }
        }

        [Serializable]
        public class MMLParameters
        {
            private bool _dispInstrumentName = true;
            public bool dispInstrumentName
            {
                get
                {
                    return _dispInstrumentName;
                }

                set
                {
                    _dispInstrumentName = value;
                }
            }


            public MMLParameters Copy()
            {
                MMLParameters MMLParameter = new MMLParameters();
                MMLParameter.dispInstrumentName = this.dispInstrumentName;
                return MMLParameter;
            }
        }

        [Serializable]
        public class Balance
        {

            private int _MasterVolume = 0;
            public int MasterVolume
            {
                get
                {
                    if (_MasterVolume > 20 || _MasterVolume < -192) _MasterVolume = 0;
                    return _MasterVolume;
                }

                set
                {
                    _MasterVolume = value;
                    if (_MasterVolume > 20 || _MasterVolume < -192) _MasterVolume = 0;
                }
            }

            private int _YM2609Volume = 0;
            public int YM2609Volume
            {
                get
                {
                    if (_YM2609Volume > 20 || _YM2609Volume < -192) _YM2609Volume = 0;
                    return _YM2609Volume;
                }

                set
                {
                    _YM2609Volume = value;
                    if (_YM2609Volume > 20 || _YM2609Volume < -192) _YM2609Volume = 0;
                }
            }

            private int _YM2612Volume = 0;
            public int YM2612Volume
            {
                get
                {
                    if (_YM2612Volume > 20 || _YM2612Volume < -192) _YM2612Volume = 0;
                    return _YM2612Volume;
                }

                set
                {
                    _YM2612Volume = value;
                    if (_YM2612Volume > 20 || _YM2612Volume < -192) _YM2612Volume = 0;
                }
            }

            private int _SN76489Volume = 0;
            public int SN76489Volume
            {
                get
                {
                    if (_SN76489Volume > 20 || _SN76489Volume < -192) _SN76489Volume = 0;
                    return _SN76489Volume;
                }

                set
                {
                    _SN76489Volume = value;
                    if (_SN76489Volume > 20 || _SN76489Volume < -192) _SN76489Volume = 0;
                }
            }

            private int _RF5C68Volume = 0;
            public int RF5C68Volume
            {
                get
                {
                    if (_RF5C68Volume > 20 || _RF5C68Volume < -192) _RF5C68Volume = 0;
                    return _RF5C68Volume;
                }

                set
                {
                    _RF5C68Volume = value;
                    if (_RF5C68Volume > 20 || _RF5C68Volume < -192) _RF5C68Volume = 0;
                }
            }

            private int _RF5C164Volume = 0;
            public int RF5C164Volume
            {
                get
                {
                    if (_RF5C164Volume > 20 || _RF5C164Volume < -192) _RF5C164Volume = 0;
                    return _RF5C164Volume;
                }

                set
                {
                    _RF5C164Volume = value;
                    if (_RF5C164Volume > 20 || _RF5C164Volume < -192) _RF5C164Volume = 0;
                }
            }

            private int _PWMVolume = 0;
            public int PWMVolume
            {
                get
                {
                    if (_PWMVolume > 20 || _PWMVolume < -192) _PWMVolume = 0;
                    return _PWMVolume;
                }

                set
                {
                    _PWMVolume = value;
                    if (_PWMVolume > 20 || _PWMVolume < -192) _PWMVolume = 0;
                }
            }

            private int _C140Volume = 0;
            public int C140Volume
            {
                get
                {
                    if (_C140Volume > 20 || _C140Volume < -192) _C140Volume = 0;
                    return _C140Volume;
                }

                set
                {
                    _C140Volume = value;
                    if (_C140Volume > 20 || _C140Volume < -192) _C140Volume = 0;
                }
            }

            private int _OKIM6258Volume = 0;
            public int OKIM6258Volume
            {
                get
                {
                    if (_OKIM6258Volume > 20 || _OKIM6258Volume < -192) _OKIM6258Volume = 0;
                    return _OKIM6258Volume;
                }

                set
                {
                    _OKIM6258Volume = value;
                    if (_OKIM6258Volume > 20 || _OKIM6258Volume < -192) _OKIM6258Volume = 0;
                }
            }

            private int _OKIM6295Volume = 0;
            public int OKIM6295Volume
            {
                get
                {
                    if (_OKIM6295Volume > 20 || _OKIM6295Volume < -192) _OKIM6295Volume = 0;
                    return _OKIM6295Volume;
                }

                set
                {
                    _OKIM6295Volume = value;
                    if (_OKIM6295Volume > 20 || _OKIM6295Volume < -192) _OKIM6295Volume = 0;
                }
            }

            private int _SEGAPCMVolume = 0;
            public int SEGAPCMVolume
            {
                get
                {
                    if (_SEGAPCMVolume > 20 || _SEGAPCMVolume < -192) _SEGAPCMVolume = 0;
                    return _SEGAPCMVolume;
                }

                set
                {
                    _SEGAPCMVolume = value;
                    if (_SEGAPCMVolume > 20 || _SEGAPCMVolume < -192) _SEGAPCMVolume = 0;
                }
            }

            private int _AY8910Volume = 0;
            public int AY8910Volume
            {
                get
                {
                    if (_AY8910Volume > 20 || _AY8910Volume < -192) _AY8910Volume = 0;
                    return _AY8910Volume;
                }

                set
                {
                    _AY8910Volume = value;
                    if (_AY8910Volume > 20 || _AY8910Volume < -192) _AY8910Volume = 0;
                }
            }

            private int _PokeyVolume = 0;
            public int PokeyVolume
            {
                get
                {
                    if (_PokeyVolume > 20 || _PokeyVolume < -192) _PokeyVolume = 0;
                    return _PokeyVolume;
                }

                set
                {
                    _PokeyVolume = value;
                    if (_PokeyVolume > 20 || _PokeyVolume < -192) _PokeyVolume = 0;
                }
            }

            private int _YM2413Volume = 0;
            public int YM2413Volume
            {
                get
                {
                    if (_YM2413Volume > 20 || _YM2413Volume < -192) _YM2413Volume = 0;
                    return _YM2413Volume;
                }

                set
                {
                    _YM2413Volume = value;
                    if (_YM2413Volume > 20 || _YM2413Volume < -192) _YM2413Volume = 0;
                }
            }

            private int _YM3526Volume = 0;
            public int YM3526Volume
            {
                get
                {
                    if (_YM3526Volume > 20 || _YM3526Volume < -192) _YM3526Volume = 0;
                    return _YM3526Volume;
                }

                set
                {
                    _YM3526Volume = value;
                    if (_YM3526Volume > 20 || _YM3526Volume < -192) _YM3526Volume = 0;
                }
            }

            private int _Y8950Volume = 0;
            public int Y8950Volume
            {
                get
                {
                    if (_Y8950Volume > 20 || _Y8950Volume < -192) _Y8950Volume = 0;
                    return _Y8950Volume;
                }

                set
                {
                    _Y8950Volume = value;
                    if (_Y8950Volume > 20 || _Y8950Volume < -192) _Y8950Volume = 0;
                }
            }

            private int _HuC6280Volume = 0;
            public int HuC6280Volume
            {
                get
                {
                    if (_HuC6280Volume > 20 || _HuC6280Volume < -192) _HuC6280Volume = 0;
                    return _HuC6280Volume;
                }

                set
                {
                    _HuC6280Volume = value;
                    if (_HuC6280Volume > 20 || _HuC6280Volume < -192) _HuC6280Volume = 0;
                }
            }

            private int _YM2151Volume = 0;
            public int YM2151Volume
            {
                get
                {
                    if (_YM2151Volume > 20 || _YM2151Volume < -192) _YM2151Volume = 0;
                    return _YM2151Volume;
                }

                set
                {
                    _YM2151Volume = value;
                    if (_YM2151Volume > 20 || _YM2151Volume < -192) _YM2151Volume = 0;
                }
            }

            private int _YM2608Volume = 0;
            public int YM2608Volume
            {
                get
                {
                    if (_YM2608Volume > 20 || _YM2608Volume < -192) _YM2608Volume = 0;
                    return _YM2608Volume;
                }

                set
                {
                    _YM2608Volume = value;
                    if (_YM2608Volume > 20 || _YM2608Volume < -192) _YM2608Volume = 0;
                }
            }

            private int _YM2608FMVolume = 0;
            public int YM2608FMVolume
            {
                get
                {
                    if (_YM2608FMVolume > 20 || _YM2608FMVolume < -192) _YM2608FMVolume = 0;
                    return _YM2608FMVolume;
                }

                set
                {
                    _YM2608FMVolume = value;
                    if (_YM2608FMVolume > 20 || _YM2608FMVolume < -192) _YM2608FMVolume = 0;
                }
            }

            private int _YM2608PSGVolume = 0;
            public int YM2608PSGVolume
            {
                get
                {
                    if (_YM2608PSGVolume > 20 || _YM2608PSGVolume < -192) _YM2608PSGVolume = 0;
                    return _YM2608PSGVolume;
                }

                set
                {
                    _YM2608PSGVolume = value;
                    if (_YM2608PSGVolume > 20 || _YM2608PSGVolume < -192) _YM2608PSGVolume = 0;
                }
            }

            private int _YM2608RhythmVolume = 0;
            public int YM2608RhythmVolume
            {
                get
                {
                    if (_YM2608RhythmVolume > 20 || _YM2608RhythmVolume < -192) _YM2608RhythmVolume = 0;
                    return _YM2608RhythmVolume;
                }

                set
                {
                    _YM2608RhythmVolume = value;
                    if (_YM2608RhythmVolume > 20 || _YM2608RhythmVolume < -192) _YM2608RhythmVolume = 0;
                }
            }

            private int _YM2608AdpcmVolume = 0;
            public int YM2608AdpcmVolume
            {
                get
                {
                    if (_YM2608AdpcmVolume > 20 || _YM2608AdpcmVolume < -192) _YM2608AdpcmVolume = 0;
                    return _YM2608AdpcmVolume;
                }

                set
                {
                    _YM2608AdpcmVolume = value;
                    if (_YM2608AdpcmVolume > 20 || _YM2608AdpcmVolume < -192) _YM2608AdpcmVolume = 0;
                }
            }

            private int _YM2203Volume = 0;
            public int YM2203Volume
            {
                get
                {
                    if (_YM2203Volume > 20 || _YM2203Volume < -192) _YM2203Volume = 0;
                    return _YM2203Volume;
                }

                set
                {
                    _YM2203Volume = value;
                    if (_YM2203Volume > 20 || _YM2203Volume < -192) _YM2203Volume = 0;
                }
            }

            private int _YM2203FMVolume = 0;
            public int YM2203FMVolume
            {
                get
                {
                    if (_YM2203FMVolume > 20 || _YM2203FMVolume < -192) _YM2203FMVolume = 0;
                    return _YM2203FMVolume;
                }

                set
                {
                    _YM2203FMVolume = value;
                    if (_YM2203FMVolume > 20 || _YM2203FMVolume < -192) _YM2203FMVolume = 0;
                }
            }

            private int _YM2203PSGVolume = 0;
            public int YM2203PSGVolume
            {
                get
                {
                    if (_YM2203PSGVolume > 20 || _YM2203PSGVolume < -192) _YM2203PSGVolume = 0;
                    return _YM2203PSGVolume;
                }

                set
                {
                    _YM2203PSGVolume = value;
                    if (_YM2203PSGVolume > 20 || _YM2203PSGVolume < -192) _YM2203PSGVolume = 0;
                }
            }

            private int _YM2610Volume = 0;
            public int YM2610Volume
            {
                get
                {
                    if (_YM2610Volume > 20 || _YM2610Volume < -192) _YM2610Volume = 0;
                    return _YM2610Volume;
                }

                set
                {
                    _YM2610Volume = value;
                    if (_YM2610Volume > 20 || _YM2610Volume < -192) _YM2610Volume = 0;
                }
            }

            private int _YM2610FMVolume = 0;
            public int YM2610FMVolume
            {
                get
                {
                    if (_YM2610FMVolume > 20 || _YM2610FMVolume < -192) _YM2610FMVolume = 0;
                    return _YM2610FMVolume;
                }

                set
                {
                    _YM2610FMVolume = value;
                    if (_YM2610FMVolume > 20 || _YM2610FMVolume < -192) _YM2610FMVolume = 0;
                }
            }

            private int _YM2610PSGVolume = 0;
            public int YM2610PSGVolume
            {
                get
                {
                    if (_YM2610PSGVolume > 20 || _YM2610PSGVolume < -192) _YM2610PSGVolume = 0;
                    return _YM2610PSGVolume;
                }

                set
                {
                    _YM2610PSGVolume = value;
                    if (_YM2610PSGVolume > 20 || _YM2610PSGVolume < -192) _YM2610PSGVolume = 0;
                }
            }

            private int _YM2610AdpcmAVolume = 0;
            public int YM2610AdpcmAVolume
            {
                get
                {
                    if (_YM2610AdpcmAVolume > 20 || _YM2610AdpcmAVolume < -192) _YM2610AdpcmAVolume = 0;
                    return _YM2610AdpcmAVolume;
                }

                set
                {
                    _YM2610AdpcmAVolume = value;
                    if (_YM2610AdpcmAVolume > 20 || _YM2610AdpcmAVolume < -192) _YM2610AdpcmAVolume = 0;
                }
            }

            private int _YM2610AdpcmBVolume = 0;
            public int YM2610AdpcmBVolume
            {
                get
                {
                    if (_YM2610AdpcmBVolume > 20 || _YM2610AdpcmBVolume < -192) _YM2610AdpcmBVolume = 0;
                    return _YM2610AdpcmBVolume;
                }

                set
                {
                    _YM2610AdpcmBVolume = value;
                    if (_YM2610AdpcmBVolume > 20 || _YM2610AdpcmBVolume < -192) _YM2610AdpcmBVolume = 0;
                }
            }

            private int _YM3812Volume = 0;
            public int YM3812Volume
            {
                get
                {
                    if (_YM3812Volume > 20 || _YM3812Volume < -192) _YM3812Volume = 0;
                    return _YM3812Volume;
                }

                set
                {
                    _YM3812Volume = value;
                    if (_YM3812Volume > 20 || _YM3812Volume < -192) _YM3812Volume = 0;
                }
            }

            private int _C352Volume = 0;
            public int C352Volume
            {
                get
                {
                    if (_C352Volume > 20 || _C352Volume < -192) _C352Volume = 0;
                    return _C352Volume;
                }

                set
                {
                    _C352Volume = value;
                    if (_C352Volume > 20 || _C352Volume < -192) _C352Volume = 0;
                }
            }

            private int _K054539Volume = 0;
            public int K054539Volume
            {
                get
                {
                    if (_K054539Volume > 20 || _K054539Volume < -192) _K054539Volume = 0;
                    return _K054539Volume;
                }

                set
                {
                    _K054539Volume = value;
                    if (_K054539Volume > 20 || _K054539Volume < -192) _K054539Volume = 0;
                }
            }

            private int _APUVolume = 0;
            public int APUVolume
            {
                get
                {
                    if (_APUVolume > 20 || _APUVolume < -192) _APUVolume = 0;
                    return _APUVolume;
                }

                set
                {
                    _APUVolume = value;
                    if (_APUVolume > 20 || _APUVolume < -192) _APUVolume = 0;
                }
            }

            private int _DMCVolume = 0;
            public int DMCVolume
            {
                get
                {
                    if (_DMCVolume > 20 || _DMCVolume < -192) _DMCVolume = 0;
                    return _DMCVolume;
                }

                set
                {
                    _DMCVolume = value;
                    if (_DMCVolume > 20 || _DMCVolume < -192) _DMCVolume = 0;
                }
            }

            private int _FDSVolume = 0;
            public int FDSVolume
            {
                get
                {
                    if (_FDSVolume > 20 || _FDSVolume < -192) _FDSVolume = 0;
                    return _FDSVolume;
                }

                set
                {
                    _FDSVolume = value;
                    if (_FDSVolume > 20 || _FDSVolume < -192) _FDSVolume = 0;
                }
            }

            private int _MMC5Volume = 0;
            public int MMC5Volume
            {
                get
                {
                    if (_MMC5Volume > 20 || _MMC5Volume < -192) _MMC5Volume = 0;
                    return _MMC5Volume;
                }

                set
                {
                    _MMC5Volume = value;
                    if (_MMC5Volume > 20 || _MMC5Volume < -192) _MMC5Volume = 0;
                }
            }

            private int _N160Volume = 0;
            public int N160Volume
            {
                get
                {
                    if (_N160Volume > 20 || _N160Volume < -192) _N160Volume = 0;
                    return _N160Volume;
                }

                set
                {
                    _N160Volume = value;
                    if (_N160Volume > 20 || _N160Volume < -192) _N160Volume = 0;
                }
            }

            private int _VRC6Volume = 0;
            public int VRC6Volume
            {
                get
                {
                    if (_VRC6Volume > 20 || _VRC6Volume < -192) _VRC6Volume = 0;
                    return _VRC6Volume;
                }

                set
                {
                    _VRC6Volume = value;
                    if (_VRC6Volume > 20 || _VRC6Volume < -192) _VRC6Volume = 0;
                }
            }

            private int _GigatronVolume = 0;
            public int GigatronVolume
            {
                get
                {
                    if (_GigatronVolume > 20 || _GigatronVolume < -192) _GigatronVolume = 0;
                    return _GigatronVolume;
                }

                set
                {
                    _GigatronVolume = value;
                    if (_GigatronVolume > 20 || _GigatronVolume < -192) _GigatronVolume = 0;
                }
            }

            private int _VRC7Volume = 0;
            public int VRC7Volume
            {
                get
                {
                    if (_VRC7Volume > 20 || _VRC7Volume < -192) _VRC7Volume = 0;
                    return _VRC7Volume;
                }

                set
                {
                    _VRC7Volume = value;
                    if (_VRC7Volume > 20 || _VRC7Volume < -192) _VRC7Volume = 0;
                }
            }

            private int _FME7Volume = 0;
            public int FME7Volume
            {
                get
                {
                    if (_FME7Volume > 20 || _FME7Volume < -192) _FME7Volume = 0;
                    return _FME7Volume;
                }

                set
                {
                    _FME7Volume = value;
                    if (_FME7Volume > 20 || _FME7Volume < -192) _FME7Volume = 0;
                }
            }

            private int _DMGVolume = 0;
            public int DMGVolume
            {
                get
                {
                    if (_DMGVolume > 20 || _DMGVolume < -192) _DMGVolume = 0;
                    return _DMGVolume;
                }

                set
                {
                    _DMGVolume = value;
                    if (_DMGVolume > 20 || _DMGVolume < -192) _DMGVolume = 0;
                }
            }

            private int _GA20Volume = 0;
            public int GA20Volume
            {
                get
                {
                    if (_GA20Volume > 20 || _GA20Volume < -192) _GA20Volume = 0;
                    return _GA20Volume;
                }

                set
                {
                    _GA20Volume = value;
                    if (_GA20Volume > 20 || _GA20Volume < -192) _GA20Volume = 0;
                }
            }

            private int _YMZ280BVolume = 0;
            public int YMZ280BVolume
            {
                get
                {
                    if (_YMZ280BVolume > 20 || _YMZ280BVolume < -192) _YMZ280BVolume = 0;
                    return _YMZ280BVolume;
                }

                set
                {
                    _YMZ280BVolume = value;
                    if (_YMZ280BVolume > 20 || _YMZ280BVolume < -192) _YMZ280BVolume = 0;
                }
            }

            private int _YMF271Volume = 0;
            public int YMF271Volume
            {
                get
                {
                    if (_YMF271Volume > 20 || _YMF271Volume < -192) _YMF271Volume = 0;
                    return _YMF271Volume;
                }

                set
                {
                    _YMF271Volume = value;
                    if (_YMF271Volume > 20 || _YMF271Volume < -192) _YMF271Volume = 0;
                }
            }

            private int _YMF262Volume = 0;
            public int YMF262Volume
            {
                get
                {
                    if (_YMF262Volume > 20 || _YMF262Volume < -192) _YMF262Volume = 0;
                    return _YMF262Volume;
                }

                set
                {
                    _YMF262Volume = value;
                    if (_YMF262Volume > 20 || _YMF262Volume < -192) _YMF262Volume = 0;
                }
            }

            private int _YMF278BVolume = 0;
            public int YMF278BVolume
            {
                get
                {
                    if (_YMF278BVolume > 20 || _YMF278BVolume < -192) _YMF278BVolume = 0;
                    return _YMF278BVolume;
                }

                set
                {
                    _YMF278BVolume = value;
                    if (_YMF278BVolume > 20 || _YMF278BVolume < -192) _YMF278BVolume = 0;
                }
            }

            private int _MultiPCMVolume = 0;
            public int MultiPCMVolume
            {
                get
                {
                    if (_MultiPCMVolume > 20 || _MultiPCMVolume < -192) _MultiPCMVolume = 0;
                    return _MultiPCMVolume;
                }

                set
                {
                    _MultiPCMVolume = value;
                    if (_MultiPCMVolume > 20 || _MultiPCMVolume < -192) _MultiPCMVolume = 0;
                }
            }

            private int _QSoundVolume = 0;
            public int QSoundVolume
            {
                get
                {
                    if (_QSoundVolume > 20 || _QSoundVolume < -192) _QSoundVolume = 0;
                    return _QSoundVolume;
                }

                set
                {
                    _QSoundVolume = value;
                    if (_QSoundVolume > 20 || _QSoundVolume < -192) _QSoundVolume = 0;
                }
            }

            private int _K051649Volume = 0;
            public int K051649Volume
            {
                get
                {
                    if (_K051649Volume > 20 || _K051649Volume < -192) _K051649Volume = 0;
                    return _K051649Volume;
                }

                set
                {
                    _K051649Volume = value;
                    if (_K051649Volume > 20 || _K051649Volume < -192) _K051649Volume = 0;
                }
            }

            private int _K053260Volume = 0;
            public int K053260Volume
            {
                get
                {
                    if (_K053260Volume > 20 || _K053260Volume < -192) _K053260Volume = 0;
                    return _K053260Volume;
                }

                set
                {
                    _K053260Volume = value;
                    if (_K053260Volume > 20 || _K053260Volume < -192) _K053260Volume = 0;
                }
            }

            private int _CS4231Volume = 0;
            public int CS4231Volume
            {
                get
                {
                    if (_CS4231Volume > 20 || _CS4231Volume < -192) _CS4231Volume = 0;
                    return _CS4231Volume;
                }

                set
                {
                    _CS4231Volume = value;
                    if (_CS4231Volume > 20 || _CS4231Volume < -192) _CS4231Volume = 0;
                }
            }


            private int _GimicOPNVolume = 100;
            public int GimicOPNVolume
            {
                get
                {
                    if (_GimicOPNVolume > 127 || _GimicOPNVolume < 0) _GimicOPNVolume = 30;
                    return _GimicOPNVolume;
                }

                set
                {
                    _GimicOPNVolume = value;
                    if (_GimicOPNVolume > 127 || _GimicOPNVolume < 0) _GimicOPNVolume = 30;
                }
            }

            private int _GimicOPNAVolume = 100;
            public int GimicOPNAVolume
            {
                get
                {
                    if (_GimicOPNAVolume > 127 || _GimicOPNAVolume < 0) _GimicOPNAVolume = 30;
                    return _GimicOPNAVolume;
                }

                set
                {
                    _GimicOPNAVolume = value;
                    if (_GimicOPNAVolume > 127 || _GimicOPNAVolume < 0) _GimicOPNAVolume = 30;
                }
            }

            public Balance Copy()
            {
                Balance Balance = new Balance();
                Balance.MasterVolume = this.MasterVolume;
                Balance.YM2151Volume = this.YM2151Volume;
                Balance.YM2203Volume = this.YM2203Volume;
                Balance.YM2203FMVolume = this.YM2203FMVolume;
                Balance.YM2203PSGVolume = this.YM2203PSGVolume;
                Balance.YM2413Volume = this.YM2413Volume;
                Balance.YM2608Volume = this.YM2608Volume;
                Balance.YM2608FMVolume = this.YM2608FMVolume;
                Balance.YM2608PSGVolume = this.YM2608PSGVolume;
                Balance.YM2608RhythmVolume = this.YM2608RhythmVolume;
                Balance.YM2608AdpcmVolume = this.YM2608AdpcmVolume;
                Balance.YM2610Volume = this.YM2610Volume;
                Balance.YM2610FMVolume = this.YM2610FMVolume;
                Balance.YM2610PSGVolume = this.YM2610PSGVolume;
                Balance.YM2610AdpcmAVolume = this.YM2610AdpcmAVolume;
                Balance.YM2610AdpcmBVolume = this.YM2610AdpcmBVolume;

                Balance.YM2612Volume = this.YM2612Volume;
                Balance.AY8910Volume = this.AY8910Volume;
                Balance.PokeyVolume = this.PokeyVolume;
                Balance.SN76489Volume = this.SN76489Volume;
                Balance.HuC6280Volume = this.HuC6280Volume;
                Balance.RF5C164Volume = this.RF5C164Volume;
                Balance.RF5C68Volume = this.RF5C68Volume;
                Balance.PWMVolume = this.PWMVolume;
                Balance.OKIM6258Volume = this.OKIM6258Volume;
                Balance.OKIM6295Volume = this.OKIM6295Volume;
                Balance.C140Volume = this.C140Volume;
                Balance.SEGAPCMVolume = this.SEGAPCMVolume;
                Balance.C352Volume = this.C352Volume;
                Balance.K051649Volume = this.K051649Volume;
                Balance.K053260Volume = this.K053260Volume;
                Balance.K054539Volume = this.K054539Volume;
                Balance.QSoundVolume = this.QSoundVolume;
                Balance.MultiPCMVolume = this.MultiPCMVolume;

                Balance.APUVolume = this.APUVolume;
                Balance.DMCVolume = this.DMCVolume;
                Balance.FDSVolume = this.FDSVolume;
                Balance.MMC5Volume = this.MMC5Volume;
                Balance.N160Volume = this.N160Volume;
                Balance.VRC6Volume = this.VRC6Volume;
                Balance.VRC7Volume = this.VRC7Volume;
                Balance.FME7Volume = this.FME7Volume;
                Balance.DMGVolume = this.DMGVolume;
                Balance.GA20Volume = this.GA20Volume;
                Balance.YMZ280BVolume = this.YMZ280BVolume;
                Balance.YMF271Volume = this.YMF271Volume;
                Balance.YMF262Volume = this.YMF262Volume;
                Balance.YMF278BVolume = this.YMF278BVolume;
                Balance.YM3526Volume = this.YM3526Volume;
                Balance.Y8950Volume = this.Y8950Volume;
                Balance.YM3812Volume = this.YM3812Volume;

                Balance.CS4231Volume = this.CS4231Volume;

                Balance.GimicOPNVolume = this.GimicOPNVolume;
                Balance.GimicOPNAVolume = this.GimicOPNAVolume;

                return Balance;
            }

            public void Save(string fullPath)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Balance), typeof(Balance).GetNestedTypes());
                using (StreamWriter sw = new StreamWriter(fullPath, false, new UTF8Encoding(false)))
                {
                    serializer.Serialize(sw, this);
                }
            }

            public static Balance Load(string fullPath)
            {
                try
                {
                    if (!File.Exists(fullPath)) return null;
                    XmlSerializer serializer = new XmlSerializer(typeof(Balance), typeof(Balance).GetNestedTypes());
                    using (StreamReader sr = new StreamReader(fullPath, new UTF8Encoding(false)))
                    {
                        return (Balance)serializer.Deserialize(sr);
                    }
                }
                catch (Exception ex)
                {
                    log.ForcedWrite(ex);
                    return null;
                }
            }

        }

        [Serializable]
        public class Location
        {
            private Rectangle _RMain = Rectangle.Empty;

            public Rectangle RMain { get => _RMain; set => _RMain = value; }

            private Rectangle _RMixer = Rectangle.Empty;
            public Rectangle RMixer { get => _RMixer; set => _RMixer = value; }
            public bool ShowMixer { get; set; } = false;

            private Rectangle _RMIDIKbd = Rectangle.Empty;
            public Rectangle RMIDIKbd { get => _RMIDIKbd; set => _RMIDIKbd = value; }

            private Rectangle _RInputBox = Rectangle.Empty;
            public Rectangle RInputBox { get => _RInputBox; set => _RInputBox = value; }

            private dgvColumnInfo[] _PartCounterClmInfo = null;
            public dgvColumnInfo[] PartCounterClmInfo { get => _PartCounterClmInfo; set => _PartCounterClmInfo = value; }

            public Location Copy()
            {
                Location Location = new Location();

                Location.RMain = this.RMain;
                Location.RMixer = this.RMixer;
                Location.RMIDIKbd = this.RMIDIKbd;
                Location.PartCounterClmInfo = this.PartCounterClmInfo;
                Location.ShowMixer = this.ShowMixer;

                return Location;
            }
        }

        [Serializable]
        public class MidiExport
        {

            private bool _UseMIDIExport = false;
            public bool UseMIDIExport
            {
                get
                {
                    return _UseMIDIExport;
                }

                set
                {
                    _UseMIDIExport = value;
                }
            }

            private bool _UseYM2151Export = false;
            public bool UseYM2151Export
            {
                get
                {
                    return _UseYM2151Export;
                }

                set
                {
                    _UseYM2151Export = value;
                }
            }

            private bool _UseYM2612Export = true;
            public bool UseYM2612Export
            {
                get
                {
                    return _UseYM2612Export;
                }

                set
                {
                    _UseYM2612Export = value;
                }
            }

            private string _ExportPath = "";
            public string ExportPath
            {
                get
                {
                    return _ExportPath;
                }

                set
                {
                    _ExportPath = value;
                }
            }

            private bool _UseVOPMex = false;
            public bool UseVOPMex
            {
                get
                {
                    return _UseVOPMex;
                }

                set
                {
                    _UseVOPMex = value;
                }
            }

            private bool _KeyOnFnum = false;
            public bool KeyOnFnum
            {
                get
                {
                    return _KeyOnFnum;
                }

                set
                {
                    _KeyOnFnum = value;
                }
            }


            public MidiExport Copy()
            {
                MidiExport MidiExport = new MidiExport();

                MidiExport.UseMIDIExport = this.UseMIDIExport;
                MidiExport.UseYM2151Export = this.UseYM2151Export;
                MidiExport.UseYM2612Export = this.UseYM2612Export;
                MidiExport.ExportPath = this.ExportPath;
                MidiExport.UseVOPMex = this.UseVOPMex;
                MidiExport.KeyOnFnum = this.KeyOnFnum;

                return MidiExport;
            }

        }

        [Serializable]
        public class MidiKbd
        {
            private int _Channel = 0;
            private int _Octave = 0;
            private int _Tempo = 0;
            private int _Clockcounter = 0;
            private int _CurrentLength = 0;
            private bool _UseQuantize = false;
            private int _Quantize = 0;
            private LfoParam[] _LfoParams = new LfoParam[4];

            private bool _UseMIDIKeyboard = false;
            public bool UseMIDIKeyboard
            {
                get
                {
                    return _UseMIDIKeyboard;
                }

                set
                {
                    _UseMIDIKeyboard = value;
                }
            }

            private string _MidiInDeviceName = "";
            public string MidiInDeviceName
            {
                get
                {
                    return _MidiInDeviceName;
                }

                set
                {
                    _MidiInDeviceName = value;
                }
            }

            private bool _IsMONO = true;
            public bool IsMONO
            {
                get
                {
                    return _IsMONO;
                }

                set
                {
                    _IsMONO = value;
                }
            }

            private int _useFormat = 0;
            public int UseFormat
            {
                get
                {
                    return _useFormat;
                }

                set
                {
                    _useFormat = value;
                }
            }

            private int _UseMONOChannel = 0;
            public int UseMONOChannel
            {
                get
                {
                    return _UseMONOChannel;
                }

                set
                {
                    _UseMONOChannel = value;
                }
            }

            private bool[] _UseChannel = new bool[9];
            public bool[] UseChannel
            {
                get
                {
                    return _UseChannel;
                }

                set
                {
                    _UseChannel = value;
                }
            }

            //private Tone[] _Tones = new Tone[6];
            //public Tone[] Tones
            //{
            //    get
            //    {
            //        return _Tones;
            //    }

            //    set
            //    {
            //        _Tones = value;
            //    }
            //}

            private int _MidiCtrl_CopyToneFromYM2612Ch1 = 97;
            public int MidiCtrl_CopyToneFromYM2612Ch1
            {
                get
                {
                    return _MidiCtrl_CopyToneFromYM2612Ch1;
                }

                set
                {
                    _MidiCtrl_CopyToneFromYM2612Ch1 = value;
                }
            }

            private int _MidiCtrl_DelOneLog = 96;
            public int MidiCtrl_DelOneLog
            {
                get
                {
                    return _MidiCtrl_DelOneLog;
                }

                set
                {
                    _MidiCtrl_DelOneLog = value;
                }
            }

            private int _MidiCtrl_CopySelecttingLogToClipbrd = 66;
            public int MidiCtrl_CopySelecttingLogToClipbrd
            {
                get
                {
                    return _MidiCtrl_CopySelecttingLogToClipbrd;
                }

                set
                {
                    _MidiCtrl_CopySelecttingLogToClipbrd = value;
                }
            }

            private int _MidiCtrl_Stop = -1;
            public int MidiCtrl_Stop
            {
                get
                {
                    return _MidiCtrl_Stop;
                }

                set
                {
                    _MidiCtrl_Stop = value;
                }
            }

            private int _MidiCtrl_Pause = -1;
            public int MidiCtrl_Pause
            {
                get
                {
                    return _MidiCtrl_Pause;
                }

                set
                {
                    _MidiCtrl_Pause = value;
                }
            }

            private int _MidiCtrl_Fadeout = -1;
            public int MidiCtrl_Fadeout
            {
                get
                {
                    return _MidiCtrl_Fadeout;
                }

                set
                {
                    _MidiCtrl_Fadeout = value;
                }
            }

            private int _MidiCtrl_Previous = -1;
            public int MidiCtrl_Previous
            {
                get
                {
                    return _MidiCtrl_Previous;
                }

                set
                {
                    _MidiCtrl_Previous = value;
                }
            }

            private int _MidiCtrl_Slow = -1;
            public int MidiCtrl_Slow
            {
                get
                {
                    return _MidiCtrl_Slow;
                }

                set
                {
                    _MidiCtrl_Slow = value;
                }
            }

            private int _MidiCtrl_Play = -1;
            public int MidiCtrl_Play
            {
                get
                {
                    return _MidiCtrl_Play;
                }

                set
                {
                    _MidiCtrl_Play = value;
                }
            }

            private int _MidiCtrl_Fast = -1;
            public int MidiCtrl_Fast
            {
                get
                {
                    return _MidiCtrl_Fast;
                }

                set
                {
                    _MidiCtrl_Fast = value;
                }
            }

            private int _MidiCtrl_Next = -1;
            public int MidiCtrl_Next
            {
                get
                {
                    return _MidiCtrl_Next;
                }

                set
                {
                    _MidiCtrl_Next = value;
                }
            }

            private bool _AlwaysTop = false;
            public bool AlwaysTop
            {
                get
                {
                    return _AlwaysTop;
                }

                set
                {
                    _AlwaysTop = value;
                }
            }

            public int Channel { get => _Channel; set => _Channel = value; }
            public int Octave { get => _Octave; set => _Octave = value; }
            public int Tempo { get => _Tempo; set => _Tempo = value; }
            public int Clockcounter { get => _Clockcounter; set => _Clockcounter = value; }
            public int CurrentLength { get => _CurrentLength; set => _CurrentLength = value; }
            public int Quantize { get => _Quantize; set => _Quantize = value; }
            public LfoParam[] LfoParams { get => _LfoParams; set => _LfoParams = value; }
            public bool UseQuantize { get => _UseQuantize; set => _UseQuantize = value; }

            private bool _useOldFunction = false;

            private int _simpleChangePreviewMode_Type = 1;
            private int _simpleChangePreviewMode_Adr = 48;//o3c
            private int _simpleUndo_Type = 1;
            private int _simpleUndo_Adr = 50;//o3d
            private int _simpleWriteSpace_Type = 1;
            private int _simpleWriteSpace_Adr = 52;//o3e
            private int _simpleWriteEnter_Type = 1;
            private int _simpleWriteEnter_Adr = 53;//o3f

            private bool _simpleOctaveChange = false;

            public bool useOldFunction { get => _useOldFunction; set => _useOldFunction = value; }

            public int simpleChangePreviewMode_Type { get => _simpleChangePreviewMode_Type; set => _simpleChangePreviewMode_Type = value; }
            public int simpleChangePreviewMode_Adr { get => _simpleChangePreviewMode_Adr; set => _simpleChangePreviewMode_Adr = value; }
            public int simpleUndo_Type { get => _simpleUndo_Type; set => _simpleUndo_Type = value; }
            public int simpleUndo_Adr { get => _simpleUndo_Adr; set => _simpleUndo_Adr = value; }
            public int simpleWriteSpace_Type { get => _simpleWriteSpace_Type; set => _simpleWriteSpace_Type = value; }
            public int simpleWriteSpace_Adr { get => _simpleWriteSpace_Adr; set => _simpleWriteSpace_Adr = value; }
            public int simpleWriteEnter_Type { get => _simpleWriteEnter_Type; set => _simpleWriteEnter_Type = value; }
            public int simpleWriteEnter_Adr { get => _simpleWriteEnter_Adr; set => _simpleWriteEnter_Adr = value; }
            public bool simpleOctaveChange { get => _simpleOctaveChange; set => _simpleOctaveChange = value; }

            public MidiKbd Copy()
            {
                MidiKbd midiKbd = new MidiKbd();

                midiKbd.MidiInDeviceName = this.MidiInDeviceName;
                midiKbd.UseMIDIKeyboard = this.UseMIDIKeyboard;
                for (int i = 0; i < midiKbd.UseChannel.Length; i++)
                {
                    midiKbd.UseChannel[i] = this.UseChannel[i];
                }
                midiKbd.IsMONO = this.IsMONO;
                midiKbd.UseMONOChannel = this.UseMONOChannel;

                midiKbd.MidiCtrl_CopySelecttingLogToClipbrd = this.MidiCtrl_CopySelecttingLogToClipbrd;
                midiKbd.MidiCtrl_CopyToneFromYM2612Ch1 = this.MidiCtrl_CopyToneFromYM2612Ch1;
                midiKbd.MidiCtrl_DelOneLog = this.MidiCtrl_DelOneLog;
                midiKbd.MidiCtrl_Fadeout = this.MidiCtrl_Fadeout;
                midiKbd.MidiCtrl_Fast = this.MidiCtrl_Fast;
                midiKbd.MidiCtrl_Next = this.MidiCtrl_Next;
                midiKbd.MidiCtrl_Pause = this.MidiCtrl_Pause;
                midiKbd.MidiCtrl_Play = this.MidiCtrl_Play;
                midiKbd.MidiCtrl_Previous = this.MidiCtrl_Previous;
                midiKbd.MidiCtrl_Slow = this.MidiCtrl_Slow;
                midiKbd.MidiCtrl_Stop = this.MidiCtrl_Stop;
                midiKbd.AlwaysTop = this.AlwaysTop;
                midiKbd.Channel = this.Channel;
                midiKbd.Octave = this.Octave;
                midiKbd.Tempo = this.Tempo;
                midiKbd.Clockcounter = this.Clockcounter;
                midiKbd.CurrentLength = this.CurrentLength;
                midiKbd.Quantize = this.Quantize;
                midiKbd.LfoParams = null;
                if (this.LfoParams.Length > 0)
                {
                    midiKbd.LfoParams = new LfoParam[this.LfoParams.Length];
                    for (int i = 0; i < this.LfoParams.Length; i++)
                    {
                        if (this.LfoParams[i] == null) continue;
                        midiKbd.LfoParams[i] = new LfoParam();
                        midiKbd.LfoParams[i] = this.LfoParams[i].Copy();
                    }
                }
                midiKbd.useOldFunction = this.useOldFunction;
                midiKbd.simpleChangePreviewMode_Type = this.simpleChangePreviewMode_Type;
                midiKbd.simpleChangePreviewMode_Adr = this.simpleChangePreviewMode_Adr;
                midiKbd.simpleUndo_Type = this.simpleUndo_Type;
                midiKbd.simpleUndo_Adr = this.simpleUndo_Adr;
                midiKbd.simpleWriteSpace_Type = this.simpleWriteSpace_Type;
                midiKbd.simpleWriteSpace_Adr = this.simpleWriteSpace_Adr;
                midiKbd.simpleWriteEnter_Type = this.simpleWriteEnter_Type;
                midiKbd.simpleWriteEnter_Adr = this.simpleWriteEnter_Adr;
                midiKbd.simpleOctaveChange=this.simpleOctaveChange;
                return midiKbd;
            }
        }

        [Serializable]
        public class LfoParam
        {
            private bool _Use = false;
            private int _Type = 0;
            private int _Delay = 0;
            private int _Speed = 0;
            private int _Delta = 0;
            private int _Depth = 0;
            private int _WaveType = 0;
            private bool _Switch = false;
            private int _Trans = 0;

            public bool Use { get => _Use; set => _Use = value; }
            public int Type { get => _Type; set => _Type = value; }
            public int Delay { get => _Delay; set => _Delay = value; }
            public int Speed { get => _Speed; set => _Speed = value; }
            public int Delta { get => _Delta; set => _Delta = value; }
            public int Depth { get => _Depth; set => _Depth = value; }
            public int WaveType { get => _WaveType; set => _WaveType = value; }
            public bool Switch { get => _Switch; set => _Switch = value; }
            public int Trans { get => _Trans; set => _Trans = value; }

            public LfoParam Copy()
            {
                LfoParam ret = new LfoParam();
                return ret;
            }
        }

        [Serializable]
        public class KeyBoardHook
        {
            [Serializable]
            public class HookKeyInfo
            {
                private bool _Shift = false;
                private bool _Ctrl = false;
                private bool _Win = false;
                private bool _Alt = false;
                private string _Key = "(None)";

                public bool Shift { get => _Shift; set => _Shift = value; }
                public bool Ctrl { get => _Ctrl; set => _Ctrl = value; }
                public bool Win { get => _Win; set => _Win = value; }
                public bool Alt { get => _Alt; set => _Alt = value; }
                public string Key { get => _Key; set => _Key = value; }

                public HookKeyInfo Copy()
                {
                    HookKeyInfo hookKeyInfo = new HookKeyInfo();
                    hookKeyInfo.Shift = this.Shift;
                    hookKeyInfo.Ctrl = this.Ctrl;
                    hookKeyInfo.Win = this.Win;
                    hookKeyInfo.Alt = this.Alt;
                    hookKeyInfo.Key = this.Key;

                    return hookKeyInfo;
                }
            }

            private bool _UseKeyBoardHook = false;
            public bool UseKeyBoardHook
            {
                get
                {
                    return _UseKeyBoardHook;
                }

                set
                {
                    _UseKeyBoardHook = value;
                }
            }

            public HookKeyInfo Stop { get => _Stop; set => _Stop = value; }
            public HookKeyInfo Pause { get => _Pause; set => _Pause = value; }
            public HookKeyInfo Fadeout { get => _Fadeout; set => _Fadeout = value; }
            public HookKeyInfo Prev { get => _Prev; set => _Prev = value; }
            public HookKeyInfo Slow { get => _Slow; set => _Slow = value; }
            public HookKeyInfo Play { get => _Play; set => _Play = value; }
            public HookKeyInfo Next { get => _Next; set => _Next = value; }
            public HookKeyInfo Fast { get => _Fast; set => _Fast = value; }
            private HookKeyInfo _Stop = new HookKeyInfo();
            private HookKeyInfo _Pause = new HookKeyInfo();
            private HookKeyInfo _Fadeout = new HookKeyInfo();
            private HookKeyInfo _Prev = new HookKeyInfo();
            private HookKeyInfo _Slow = new HookKeyInfo();
            private HookKeyInfo _Play = new HookKeyInfo();
            private HookKeyInfo _Next = new HookKeyInfo();
            private HookKeyInfo _Fast = new HookKeyInfo();

            public KeyBoardHook Copy()
            {
                KeyBoardHook keyBoard = new KeyBoardHook();
                keyBoard.UseKeyBoardHook = this.UseKeyBoardHook;
                keyBoard.Stop = this.Stop.Copy();
                keyBoard.Pause = this.Pause.Copy();
                keyBoard.Fadeout = this.Fadeout.Copy();
                keyBoard.Prev = this.Prev.Copy();
                keyBoard.Slow = this.Slow.Copy();
                keyBoard.Play = this.Play.Copy();
                keyBoard.Next = this.Next.Copy();
                keyBoard.Fast = this.Fast.Copy();

                return keyBoard;
            }
        }

        [Serializable]
        public class Sien
        {
            public string[] CacheInstrumentName = null;
            public string[][] CacheInstrumentData = null;
            public bool cacheClear = false;

            public Sien Copy()
            {
                Sien sien = new Sien();
                sien.CacheInstrumentName = this.CacheInstrumentName;
                sien.CacheInstrumentData = this.CacheInstrumentData;
                sien.cacheClear = this.cacheClear;

                return sien;
            }
        }

        //[Serializable]
        //public class Vst
        //{
        //    private string _DefaultPath = "";

        //    private string[] _VSTPluginPath = null;
        //    public string[] VSTPluginPath
        //    {
        //        get
        //        {
        //            return _VSTPluginPath;
        //        }

        //        set
        //        {
        //            _VSTPluginPath = value;
        //        }
        //    }


        //    private vstInfo[] _VSTInfo = null;
        //    public vstInfo[] VSTInfo
        //    {
        //        get
        //        {
        //            return _VSTInfo;
        //        }

        //        set
        //        {
        //            _VSTInfo = value;
        //        }
        //    }

        //    public string DefaultPath
        //    {
        //        get
        //        {
        //            return _DefaultPath;
        //        }

        //        set
        //        {
        //            _DefaultPath = value;
        //        }
        //    }

        //    public Vst Copy()
        //    {
        //        Vst vst = new Vst();

        //        vst.VSTInfo = this.VSTInfo;
        //        vst.DefaultPath = this.DefaultPath;

        //        return vst;
        //    }

        //}

        [Serializable]
        public class MidiOut
        {
            private string _GMReset = "30:F0,7E,7F,09,01,F7";
            public string GMReset { get => _GMReset; set => _GMReset = value; }

            private string _XGReset = "30:F0,43,10,4C,00,00,7E,00,F7";
            public string XGReset { get => _XGReset; set => _XGReset = value; }

            private string _GSReset = "30:F0,41,10,42,12,40,00,7F,00,41,F7";
            public string GSReset { get => _GSReset; set => _GSReset = value; }

            private string _Custom = "";
            public string Custom { get => _Custom; set => _Custom = value; }

            private List<midiOutInfo[]> _lstMidiOutInfo = null;
            public List<midiOutInfo[]> lstMidiOutInfo
            {
                get
                {
                    return _lstMidiOutInfo;
                }
                set
                {
                    _lstMidiOutInfo = value;
                }
            }

            public MidiOut Copy()
            {
                MidiOut MidiOut = new MidiOut();

                MidiOut.GMReset = this.GMReset;
                MidiOut.XGReset = this.XGReset;
                MidiOut.GSReset = this.GSReset;
                MidiOut.Custom = this.Custom;
                MidiOut.lstMidiOutInfo = this.lstMidiOutInfo;

                return MidiOut;
            }

        }

        [Serializable]
        public class NSF
        {
            private bool _NESUnmuteOnReset = true;
            private bool _NESNonLinearMixer = true;
            private bool _NESPhaseRefresh = true;
            private bool _NESDutySwap = false;

            private int _FDSLpf = 2000;
            private bool _FDS4085Reset = false;
            private bool _FDSWriteDisable8000 = true;

            private bool _DMCUnmuteOnReset = true;
            private bool _DMCNonLinearMixer = true;
            private bool _DMCEnable4011 = true;
            private bool _DMCEnablePnoise = true;
            private bool _DMCDPCMAntiClick = false;
            private bool _DMCRandomizeNoise = true;
            private bool _DMCTRImute = true;
            private bool _DMCTRINull = true;

            private bool _MMC5NonLinearMixer = true;
            private bool _MMC5PhaseRefresh = true;

            private bool _N160Serial = false;

            public bool NESUnmuteOnReset
            {
                get
                {
                    return _NESUnmuteOnReset;
                }

                set
                {
                    _NESUnmuteOnReset = value;
                }
            }
            public bool NESNonLinearMixer
            {
                get
                {
                    return _NESNonLinearMixer;
                }

                set
                {
                    _NESNonLinearMixer = value;
                }
            }
            public bool NESPhaseRefresh
            {
                get
                {
                    return _NESPhaseRefresh;
                }

                set
                {
                    _NESPhaseRefresh = value;
                }
            }
            public bool NESDutySwap
            {
                get
                {
                    return _NESDutySwap;
                }

                set
                {
                    _NESDutySwap = value;
                }
            }

            public int FDSLpf
            {
                get
                {
                    return _FDSLpf;
                }

                set
                {
                    _FDSLpf = value;
                }
            }
            public bool FDS4085Reset
            {
                get
                {
                    return _FDS4085Reset;
                }

                set
                {
                    _FDS4085Reset = value;
                }
            }
            public bool FDSWriteDisable8000
            {
                get
                {
                    return _FDSWriteDisable8000;
                }
                set
                {
                    _FDSWriteDisable8000 = value;
                }
            }

            public bool DMCUnmuteOnReset
            {
                get
                {
                    return _DMCUnmuteOnReset;
                }

                set
                {
                    _DMCUnmuteOnReset = value;
                }
            }
            public bool DMCNonLinearMixer
            {
                get
                {
                    return _DMCNonLinearMixer;
                }

                set
                {
                    _DMCNonLinearMixer = value;
                }
            }
            public bool DMCEnable4011
            {
                get
                {
                    return _DMCEnable4011;
                }

                set
                {
                    _DMCEnable4011 = value;
                }
            }
            public bool DMCEnablePnoise
            {
                get
                {
                    return _DMCEnablePnoise;
                }

                set
                {
                    _DMCEnablePnoise = value;
                }
            }
            public bool DMCDPCMAntiClick
            {
                get
                {
                    return _DMCDPCMAntiClick;
                }

                set
                {
                    _DMCDPCMAntiClick = value;
                }
            }
            public bool DMCRandomizeNoise
            {
                get
                {
                    return _DMCRandomizeNoise;
                }

                set
                {
                    _DMCRandomizeNoise = value;
                }
            }
            public bool DMCTRImute
            {
                get
                {
                    return _DMCTRImute;
                }

                set
                {
                    _DMCTRImute = value;
                }
            }
            public bool DMCTRINull
            {
                get
                {
                    return _DMCTRINull;
                }

                set
                {
                    _DMCTRINull = value;
                }
            }

            public bool MMC5NonLinearMixer
            {
                get
                {
                    return _MMC5NonLinearMixer;
                }

                set
                {
                    _MMC5NonLinearMixer = value;
                }
            }
            public bool MMC5PhaseRefresh
            {
                get
                {
                    return _MMC5PhaseRefresh;
                }

                set
                {
                    _MMC5PhaseRefresh = value;
                }
            }

            public bool N160Serial
            {
                get
                {
                    return _N160Serial;
                }

                set
                {
                    _N160Serial = value;
                }
            }


            public NSF Copy()
            {
                NSF NSF = new NSF();

                NSF.NESUnmuteOnReset = this.NESUnmuteOnReset;
                NSF.NESNonLinearMixer = this.NESNonLinearMixer;
                NSF.NESPhaseRefresh = this.NESPhaseRefresh;
                NSF.NESDutySwap = this.NESDutySwap;

                NSF.FDSLpf = this.FDSLpf;
                NSF.FDS4085Reset = this.FDS4085Reset;
                NSF.FDSWriteDisable8000 = this.FDSWriteDisable8000;

                NSF.DMCUnmuteOnReset = this.DMCUnmuteOnReset;
                NSF.DMCNonLinearMixer = this.DMCNonLinearMixer;
                NSF.DMCEnable4011 = this.DMCEnable4011;
                NSF.DMCEnablePnoise = this.DMCEnablePnoise;
                NSF.DMCDPCMAntiClick = this.DMCDPCMAntiClick;
                NSF.DMCRandomizeNoise = this.DMCRandomizeNoise;
                NSF.DMCTRImute = this.DMCTRImute;
                NSF.DMCTRINull = this.DMCTRINull;

                NSF.MMC5NonLinearMixer = this.MMC5NonLinearMixer;
                NSF.MMC5PhaseRefresh = this.MMC5PhaseRefresh;

                NSF.N160Serial = this.N160Serial;

                return NSF;
            }

        }

        [Serializable]
        public class SID
        {
            public string RomKernalPath = "";
            public string RomBasicPath = "";
            public string RomCharacterPath = "";
            public int Quality = 1;
            public int OutputBufferSize = 5000;

            public SID Copy()
            {
                SID SID = new SID();

                SID.RomKernalPath = this.RomKernalPath;
                SID.RomBasicPath = this.RomBasicPath;
                SID.RomCharacterPath = this.RomCharacterPath;
                SID.Quality = this.Quality;
                SID.OutputBufferSize = this.OutputBufferSize;

                return SID;
            }
        }

        [Serializable]
        public class NukedOPN2
        {
            public int EmuType = 0;

            public NukedOPN2 Copy()
            {
                NukedOPN2 no = new NukedOPN2();
                no.EmuType = this.EmuType;

                return no;
            }
        }
        
        [Serializable]
        public class GensOption
        {
            public bool DACHPF = true;
            public bool SSGEG = true;

            public GensOption Copy()
            {
                GensOption go = new GensOption();
                go.DACHPF = this.DACHPF;
                go.SSGEG = this.SSGEG;

                return go;
            }
        }

        [Serializable]
        public class AutoBalance
        {
            private bool _UseThis = false;
            private bool _LoadSongBalance = false;
            private bool _LoadDriverBalance = false;
            private bool _SaveSongBalance = false;
            private bool _SamePositionAsSongData = false;

            public bool UseThis { get => _UseThis; set => _UseThis = value; }
            public bool LoadSongBalance { get => _LoadSongBalance; set => _LoadSongBalance = value; }
            public bool LoadDriverBalance { get => _LoadDriverBalance; set => _LoadDriverBalance = value; }
            public bool SaveSongBalance { get => _SaveSongBalance; set => _SaveSongBalance = value; }
            public bool SamePositionAsSongData { get => _SamePositionAsSongData; set => _SamePositionAsSongData = value; }

            public AutoBalance Copy()
            {
                AutoBalance AutoBalance = new AutoBalance();
                AutoBalance.UseThis = this.UseThis;
                AutoBalance.LoadSongBalance = this.LoadSongBalance;
                AutoBalance.LoadDriverBalance = this.LoadDriverBalance;
                AutoBalance.SaveSongBalance = this.SaveSongBalance;
                AutoBalance.SamePositionAsSongData = this.SamePositionAsSongData;

                return AutoBalance;
            }
        }

        [Serializable]
        public class PMDDotNET
        {
            public string compilerArguments = "/v /C";
            public bool isAuto = true;
            public int soundBoard = 0;
            public bool usePPSDRV = true;
            public bool usePPZ8 = true;
            public string driverArguments = "";
            public bool setManualVolume = false;
            public bool usePPSDRVUseInterfaceDefaultFreq = true;
            public int PPSDRVManualFreq = 2000;
            public int PPSDRVManualWait = 1;
            public int volumeFM = 0;
            public int volumeSSG = 0;
            public int volumeRhythm = 0;
            public int volumeAdpcm = 0;
            public int volumeGIMICSSG = 31;

            public PMDDotNET Copy()
            {
                PMDDotNET p = new PMDDotNET();
                p.compilerArguments = this.compilerArguments;
                p.isAuto = this.isAuto;
                p.soundBoard = this.soundBoard;
                p.usePPSDRV = this.usePPSDRV;
                p.usePPZ8 = this.usePPZ8;
                p.driverArguments = this.driverArguments;
                p.setManualVolume = this.setManualVolume;
                p.usePPSDRVUseInterfaceDefaultFreq = this.usePPSDRVUseInterfaceDefaultFreq;
                p.PPSDRVManualFreq = this.PPSDRVManualFreq;
                p.PPSDRVManualWait = this.PPSDRVManualWait;
                p.volumeFM = this.volumeFM;
                p.volumeSSG = this.volumeSSG;
                p.volumeRhythm = this.volumeRhythm;
                p.volumeAdpcm = this.volumeAdpcm;
                p.volumeGIMICSSG = this.volumeGIMICSSG;

                return p;
            }
        }



        public Setting Copy()
        {
            Setting setting = new Setting();
            setting.outputDevice = this.outputDevice.Copy();
            setting.unuseRealChip = this.unuseRealChip;

            setting.AY8910Type = this.AY8910Type.Copy();
            setting.YM2151Type = this.YM2151Type.Copy();
            setting.YM2203Type = this.YM2203Type.Copy();
            setting.YM2413Type = this.YM2413Type.Copy();
            setting.YM2608Type = this.YM2608Type.Copy();
            setting.YM2610Type = this.YM2610Type.Copy();
            setting.YM2612Type = this.YM2612Type.Copy();
            setting.SN76489Type = this.SN76489Type.Copy();
            setting.C140Type = this.C140Type.Copy();
            setting.SEGAPCMType = this.SEGAPCMType.Copy();

            setting.AY8910SType = this.AY8910SType.Copy();
            setting.YM2151SType = this.YM2151SType.Copy();
            setting.YM2203SType = this.YM2203SType.Copy();
            setting.YM2413SType = this.YM2413SType.Copy();
            setting.YM2608SType = this.YM2608SType.Copy();
            setting.YM2610SType = this.YM2610SType.Copy();
            setting.YM2612SType = this.YM2612SType.Copy();
            setting.SN76489SType = this.SN76489SType.Copy();
            setting.C140SType = this.C140SType.Copy();
            setting.SEGAPCMSType = this.SEGAPCMSType.Copy();

            setting.other = this.other.Copy();
            setting.export = this.export.Copy();
            setting.MMLParameter = this.MMLParameter.Copy();
            setting.balance = this.balance.Copy();
            setting.location = this.location.Copy();
            setting.LatencyEmulation = this.LatencyEmulation;
            setting.LatencySCCI = this.LatencySCCI;
            setting.Debug_DispFrameCounter = this.Debug_DispFrameCounter;
            setting.HiyorimiMode = this.HiyorimiMode;
            setting.musicInterruptTimer = this.musicInterruptTimer;
            //setting.location = this.location.Copy();
            setting.dockingState = this.dockingState;
            setting.midiExport = this.midiExport.Copy();
            setting.midiKbd = this.midiKbd.Copy();
            //setting.vst = this.vst.Copy();
            setting.midiOut = this.midiOut.Copy();
            //setting.nsf = this.nsf.Copy();
            //setting.sid = this.sid.Copy();
            setting.nukedOPN2 = this.nukedOPN2.Copy();
            setting.gensOption = this.gensOption.Copy();
            setting.autoBalance = this.autoBalance.Copy();
            setting.pmdDotNET = this.pmdDotNET.Copy();

            setting.keyBoardHook = this.keyBoardHook.Copy();
            setting.IsManualDetect = this.IsManualDetect;
            setting.AutoDetectModuleType = this.AutoDetectModuleType;
            setting.ColorScheme = this.ColorScheme.Copy();
            setting.InfiniteOfflineMode = this.InfiniteOfflineMode;
            setting.UseSien = this.UseSien;
            setting.shortCutKey = (this.shortCutKey != null) ? this.shortCutKey.Copy() : null;
            setting.sien=this.sien.Copy();

            return setting;
        }

        public void Save()
        {
            string fullPath = Common.settingFilePath;
            fullPath = Path.Combine(fullPath, Resources.cntSettingFileName);

            XmlSerializer serializer = new XmlSerializer(typeof(Setting), typeof(Setting).GetNestedTypes());
            using (StreamWriter sw = new StreamWriter(fullPath, false, new UTF8Encoding(false)))
            {
                serializer.Serialize(sw, this);
            }
        }

        public static Setting Load()
        {
            try
            {
                string fn = Resources.cntSettingFileName;
                string path1 = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
                path1 = string.IsNullOrEmpty(path1) ? Application.ExecutablePath : path1;
                if (System.IO.File.Exists(System.IO.Path.Combine(path1, fn)))
                {
                    Common.settingFilePath = path1;
                }
                else
                {
                    Common.settingFilePath = Common.GetApplicationDataFolder(true);
                }

                string fullPath = Common.settingFilePath;
                fullPath = Path.Combine(fullPath, Resources.cntSettingFileName);

                if (!File.Exists(fullPath))
                {
                    Setting s = new Setting();
                    s.OfflineMode = s.InfiniteOfflineMode;
                    CheckShortCutKey(s);
                    return s;
                }
                XmlSerializer serializer = new XmlSerializer(typeof(Setting), typeof(Setting).GetNestedTypes());
                using (StreamReader sr = new StreamReader(fullPath, new UTF8Encoding(false)))
                {
                    Setting s = (Setting)serializer.Deserialize(sr);
                    s.OfflineMode = s.InfiniteOfflineMode;
                    CheckShortCutKey(s);
                    return s;
                }
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
                Setting s = new Setting();
                s.OfflineMode = s.InfiniteOfflineMode;
                CheckShortCutKey(s);
                return s;
            }
        }

        public static Setting.ShortCutKey.ShortCutKeyInfo[] aryShortcutKeyDefault = new Setting.ShortCutKey.ShortCutKeyInfo[]
        {
            //ShortCutKeyInfo(int number, string func, bool shift, bool ctrl, bool alt, string key)
            new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.FileOpen,"開く",false,false,false,"F1")
            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.FileOpen+1,"開く",false,true,false,"O")
            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.FileSave,"保存",false,false,false,"F2")
            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.FileSave+1,"保存",false,true,false,"S")
            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.Search,"検索",false,true,false,"F")
            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.SearchNext,"次を検索",false,false,false,"F3")
            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.SearchPrev,"前を検索",true,false,false,"F3")
            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.Compile,"コンパイル",false,false,false,"F4")
            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.Play,"再生",false,false,false,"F5")
            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.SkipPlay,"スキップ+再生",true,false,false,"F5")
            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.TracePlay,"トレース+再生",false,true,false,"F5")
            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.SkipTracePlay,"スキップ+トレース+再生",true,true,false,"F5")
            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.JsoloPlay,"Jソロ+再生",false,false,true,"F5")
            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.SkipJsoloPlay,"スキップ+Jソロ+再生",true,false,true,"F5")
            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.TraceJsoloPlay,"トレース+Jソロ+再生",false,true,true,"F5")
            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.SkipTraceJsoloPlay,"スキップ+トレース+Jソロ+再生",true,true,true,"F5")
            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.M98,"M98",false,false,false,"F6")
            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.Pause,"ポーズ",false,false,false,"F8")
            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.Stop,"停止",false,false,false,"F9")
            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.FadeOut,"フェードアウト",true,false,false,"F9")
            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.Slow,"スロー",false,false,false,"F10")
            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.Fastx4,"4倍速",false,false,false,"F11")
            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.MDPlay,"エクスポート&Play MDPlayer",false,false,false,"F12")
            //,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.Sien,"支援",false,false,false,"F12")
            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.CloseTab,"タブを閉じる",false,true,false,"W")
            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.CloseTabForce,"強制的にタブを閉じる",true,true,false,"W")
            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.JumpAnchorNext,"次の//まで飛ぶ",false,true,false,"Next")
            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.JumpAnchorPrev,"前の//まで飛ぶ",false,true,false,"PageUp")
            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.CommentOnOff,"コメント化する/しない",false,true,false,"Divide")
            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.CommentOnOff+1,"コメント化する/しない",false,true,false,"Oem2")
            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.PartEnter,"改行時にパート情報自動挿入",true,false,false,"Enter")
            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.Home,"キャレットをホームポジション/パート情報後に移動",false,false,false,"Home")
            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.ExportDriverFormat,"エクスポートtoDriverFormat",false,false,false,"F7")
            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.ExportWav,"エクスポートtoWav",false,false,false,"")
            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.ExportWavAllChSolo,"エクスポートtoWav All Ch Solo",true,false,false,"F7")

            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.Ch01Solo,"Ch01 Solo ",false,true,false,"D1")
            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.Ch02Solo,"Ch02 Solo ",false,true,false,"D2")
            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.Ch03Solo,"Ch03 Solo ",false,true,false,"D3")
            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.Ch04Solo,"Ch04 Solo ",false,true,false,"D4")
            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.Ch05Solo,"Ch05 Solo ",false,true,false,"D5")
            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.Ch06Solo,"Ch06 Solo ",false,true,false,"D6")
            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.Ch07Solo,"Ch07 Solo ",false,true,false,"D7")
            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.Ch08Solo,"Ch08 Solo ",false,true,false,"D8")
            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.Ch09Solo,"Ch09 Solo ",false,true,false,"D9")
            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.Ch10Solo,"Ch10 Solo ",false,true,false,"D0")
            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.Ch11Solo,"Ch11 Solo ",false,true,false,"OemMinus")
            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.ResetSolo,"Reset Solo",false,true,false,"Oem5")

            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.Ch01Mute,"Ch01 Mute ",false,false,true,"D1")
            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.Ch02Mute,"Ch02 Mute ",false,false,true,"D2")
            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.Ch03Mute,"Ch03 Mute ",false,false,true,"D3")
            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.Ch04Mute,"Ch04 Mute ",false,false,true,"D4")
            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.Ch05Mute,"Ch05 Mute ",false,false,true,"D5")
            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.Ch06Mute,"Ch06 Mute ",false,false,true,"D6")
            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.Ch07Mute,"Ch07 Mute ",false,false,true,"D7")
            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.Ch08Mute,"Ch08 Mute ",false,false,true,"D8")
            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.Ch09Mute,"Ch09 Mute ",false,false,true,"D9")
            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.Ch10Mute,"Ch10 Mute ",false,false,true,"D0")
            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.Ch11Mute,"Ch11 Mute ",false,false,true,"OemMinus")
            ,new Setting.ShortCutKey.ShortCutKeyInfo((int)Setting.ShortCutKey.enmContent.ResetMute,"Reset Mute",false,false,true,"Oem5")

        };


        public static void CheckShortCutKey(Setting setting)
        {
            if (setting.shortCutKey != null)
            {
                if (CheckShortCutKeyStep2(setting)) return;
            }

            //定義が全くない場合は初期設定を設定する
            setting.shortCutKey = new Setting.ShortCutKey();
            setting.shortCutKey.Info = new Setting.ShortCutKey.ShortCutKeyInfo[aryShortcutKeyDefault.Length];
            for (int i = 0; i < aryShortcutKeyDefault.Length; i++)
            {
                setting.shortCutKey.Info[i] = aryShortcutKeyDefault[i].Copy();
            }
        }

        public static bool CheckShortCutKeyStep2(Setting setting)
        {
            //ショートカットキー設定に既存の設定を上書きして、新たな設定を作る
            ShortCutKey.ShortCutKeyInfo[] newAry = new ShortCutKey.ShortCutKeyInfo[aryShortcutKeyDefault.Length];
            for (int i = 0; i < aryShortcutKeyDefault.Length; i++)
            {
                newAry[i] = aryShortcutKeyDefault[i].Copy();
                for (int j = 0; j < setting.shortCutKey.Info.Length; j++)
                {
                    if (newAry[i].number != setting.shortCutKey.Info[j].number) continue;
                    newAry[i] = setting.shortCutKey.Info[j].Copy();
                    if (newAry[i].key.IndexOf("Return") > -1)
                    {
                        newAry[i].key=newAry[i].key.Replace("Return", "Enter");
                    }
                    break;
                }
            }

            //重複が無いか確認する
            bool duplicate = false;
            for (int i = 0; i < newAry.Length; i++)
            {
                for (int j = 0; j < newAry.Length; j++)
                {
                    if (i == j) continue;

                    if (newAry[i].alt != newAry[j].alt) continue;
                    if (newAry[i].shift != newAry[j].shift) continue;
                    if (newAry[i].ctrl != newAry[j].ctrl) continue;
                    if (newAry[i].key.ToString() != newAry[j].key.ToString()) continue;

                    duplicate = true;
                    break;
                }
            }

            //重複がない場合は問題なし
            if (!duplicate)
            {
                setting.shortCutKey.Info = newAry;
                return true;
            }

            DialogResult res = MessageBox.Show(@"ショートカットキーの設定に重複があることを検出しました。
「はい」:設定を初期化して起動する。(推奨)
「いいえ」:このまま起動する", "起動時チェック処理", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (res == DialogResult.Yes)
            {
                setting.shortCutKey.Info = null;
                return false;
            }

            return true;
        }

        [Serializable]
        public class ShortCutKey
        {
            private ShortCutKeyInfo[] _Info = null;
            public ShortCutKeyInfo[] Info { get => _Info; set => _Info = value; }

            [Serializable]
            public class ShortCutKeyInfo
            {
                public int number = -1;
                public string func = "";
                public bool shift = false;
                public bool ctrl = false;
                public bool alt = false;
                public string key = "";

                public bool shift2 = false;
                public bool ctrl2 = false;
                public bool alt2 = false;
                public string key2 = "";

                public int step = 0;

                public ShortCutKeyInfo()
                {
                }

                public ShortCutKeyInfo(int number, string func
                    , bool shift, bool ctrl, bool alt, string key
                    , bool shift2 = false, bool ctrl2 = false, bool alt2 = false, string key2 = "")
                {
                    this.number = number;
                    this.func = func;

                    this.shift = shift;
                    this.ctrl = ctrl;
                    this.alt = alt;
                    this.key = key;

                    this.shift2 = shift2;
                    this.ctrl2 = ctrl2;
                    this.alt2 = alt2;
                    this.key2 = key2;
                }

                public ShortCutKeyInfo Copy()
                {
                    ShortCutKeyInfo ret = new ShortCutKeyInfo(number, func, shift, ctrl, alt, key, shift2, ctrl2, alt2, key2);
                    return ret;
                }

            }

            public enum enmContent : int
            {
                FileOpen = 0,
                FileSave = 100,
                Search = 200,
                SearchNext = 210,
                SearchPrev = 220,
                JumpAnchorNext = 230,
                JumpAnchorPrev = 240,
                Compile = 300,
                Play = 400,
                SkipPlay = 410,
                TracePlay = 420,
                SkipTracePlay = 430,
                JsoloPlay = 440,
                SkipJsoloPlay = 450,
                TraceJsoloPlay = 460,
                SkipTraceJsoloPlay = 470,
                M98 = 500,
                Pause = 600,
                Stop = 800,
                FadeOut = 810,
                Slow = 900,
                Fastx4 = 1000,
                Kbd = -1100,
                Sien = 1100,
                CloseTab = 1200,
                CloseTabForce = 1210,
                CommentOnOff = 1220,
                PartEnter = 1230,
                Home = 1240,
                MDPlay = 1300,
                ExportDriverFormat = 1400,
                ExportWav = 1410,
                ExportWavAllChSolo = 1420,

                Ch01Solo = 2000,
                Ch02Solo = 2100,
                Ch03Solo = 2200,
                Ch04Solo = 2300,
                Ch05Solo = 2400,
                Ch06Solo = 2500,
                Ch07Solo = 2600,
                Ch08Solo = 2700,
                Ch09Solo = 2800,
                Ch10Solo = 2900,
                Ch11Solo = 3000,
                ResetSolo = 3100,
                Ch01Mute = 3200,
                Ch02Mute = 3300,
                Ch03Mute = 3400,
                Ch04Mute = 3500,
                Ch05Mute = 3600,
                Ch06Mute = 3700,
                Ch07Mute = 3800,
                Ch08Mute = 3900,
                Ch09Mute = 4000,
                Ch10Mute = 4100,
                Ch11Mute = 4200,
                ResetMute = 4300,
            };

            public ShortCutKey Copy()
            {
                ShortCutKey ret = new ShortCutKey();
                ret.Info = new ShortCutKeyInfo[this.Info.Length];
                int i = 0;
                foreach (ShortCutKeyInfo inf in this.Info)
                {
                    ret.Info[i++] = inf.Copy();
                }

                return ret;
            }
        }
    }

}
