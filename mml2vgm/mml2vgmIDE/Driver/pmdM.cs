using Core;
using musicDriverInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mml2vgmIDE
{
    public class pmdM : baseDriver
    {
        public uint YM2608ClockValue { get; internal set; } = 7987200;
        public int YM2608_FMVolume=0;
        public int YM2608_SSGVolume = 0;
        public int YM2608_RhythmVolume = 0;
        public int YM2608_AdpcmVolume = 0;
        public int GIMIC_SSGVolume = 0;

        private musicDriverInterface.MmlDatum[] mBuf = null;
        private PMDManager pm = null;
        private bool initPhase = true;
        private List<SoundManager.PackData> pd = new List<SoundManager.PackData>();
        private List<SoundManager.PackData> psd = new List<SoundManager.PackData>();
        private List<SoundManager.PackData> pzd = new List<SoundManager.PackData>();
        private List<SoundManager.PackData> p8d = new List<SoundManager.PackData>();
        private long count = 0;
        private SoundManager.Chip chipYM2608;
        private SoundManager.Chip chipPPZ8;
        private SoundManager.Chip chipPPSDRV;
        private SoundManager.Chip chipP86;
        private string filename = "";


        public override GD3 getGD3Info(byte[] buf, uint vgmGd3)
        {
            return null;
        }

        public override bool init(outDatum[] vgmBuf, ChipRegister chipRegister, EnmChip[] useChip, uint latency, uint waitTime, long jumpPointClock)
        {
            return true;
        }

        public override void oneFrameProc()
        {
            if (initPhase)
            {
                initPhase = false;

                if (pd != null) chipRegister.YM2608SetRegister(null, count, chipYM2608, pd.ToArray());
                if (pzd != null) chipRegister.PPZ8SetRegister(null, count, chipPPZ8, pzd.ToArray());
                if (psd != null) chipRegister.PPSDRVSetRegister(null, count, chipPPSDRV, psd.ToArray());
                if (p8d != null) chipRegister.P86SetRegister(null, count, chipP86, p8d.ToArray());
                return;
            }

            pm.Rendering();
            Audio.DriverSeqCounter = count;
            vgmCurLoop = (uint)GetNowLoopCounter();

            if (pm.Stopped)
            {
                Stopped = true;
                return;
            }
            count++;
        }

        public bool init(MmlDatum[] mBuf
            , string mWorkPath
            , PMDManager pmdManager
            , ChipRegister chipRegister
            , EnmChip[] enmChips
            , uint v1, uint v2
            , string mFileName
            , bool isGIMICOPNA)
        {
            if (pmdManager == null) return false;

            this.vgmBuf = null;
            this.mBuf = mBuf;
            this.chipRegister = chipRegister;
            this.useChip = useChip;
            this.latency = latency;
            this.waitTime = waitTime;
            this.pm = pmdManager;
            chipYM2608 = chipRegister.YM2608[0];
            chipPPZ8 = chipRegister.PPZ8[0];
            chipPPSDRV = chipRegister.PPSDRV[0];
            chipP86 = chipRegister.P86[0];
            filename = mFileName;

            Counter = 0;
            TotalCounter = 0;
            LoopCounter = 0;
            vgmCurLoop = 0;
            Stopped = false;
            vgmFrameCounter = -latency - waitTime;
            vgmSpeed = 1;
            vgmSpeedCounter = 0;
            YM2608_FMVolume = 0;
            YM2608_SSGVolume = 0;
            YM2608_RhythmVolume = 0;
            YM2608_AdpcmVolume = 0;
            GIMIC_SSGVolume = 0;

            initPhase = true;
            pd = new List<SoundManager.PackData>();
            psd = new List<SoundManager.PackData>();
            pzd = new List<SoundManager.PackData>();
            p8d = new List<SoundManager.PackData>();

            //Driverの初期化
            pm.InitDriver(
                System.IO.Path.Combine(mWorkPath, "dummy")
                , OPNAInitialWrite
                , OPNAWaitSend
                , PPZ8Write
                , PPSDRVWrite
                , P86Write
                , false
                , mBuf
                , true
                , false
                , chipRegister
                , isGIMICOPNA
                );

            YM2608_FMVolume     = pm.YM2608_FMVolume    ;
            YM2608_SSGVolume    = pm.YM2608_SSGVolume   ;
            YM2608_RhythmVolume = pm.YM2608_RhythmVolume;
            YM2608_AdpcmVolume  = pm.YM2608_AdpcmVolume ;
            GIMIC_SSGVolume     = pm.GIMIC_SSGVolume;

            if(!Audio.setting.pmdDotNET.isAuto && Audio.setting.pmdDotNET.setManualVolume)
            {
                YM2608_FMVolume = Audio.setting.pmdDotNET.volumeFM;
                YM2608_SSGVolume = Audio.setting.pmdDotNET.volumeSSG;
                YM2608_RhythmVolume = Audio.setting.pmdDotNET.volumeRhythm;
                YM2608_AdpcmVolume = Audio.setting.pmdDotNET.volumeAdpcm;
                GIMIC_SSGVolume = Audio.setting.pmdDotNET.volumeGIMICSSG;
            }


            pm.StartRendering((int)Common.SampleRate, (int)YM2608ClockValue);
            pm.MSTART(0);
            
            return true;
        }

        private int PPSDRVWrite(ChipDatum arg)
        {
            if (arg == null) return 0;

            if (!initPhase)
            {
                outDatum od = null;
                if (arg.addtionalData != null)
                {
                    if (arg.addtionalData is MmlDatum)
                    {
                        MmlDatum md = (MmlDatum)arg.addtionalData;
                        if (md.linePos != null) md.linePos.srcMMLID = filename;
                        od = new outDatum(md.type, md.args, md.linePos, (byte)md.dat);
                    }

                }
                if (arg.port == 0x05)
                {
                    chipRegister.PPSDRVLoad(od, count, 0, (byte[])arg.addtionalData);
                }
                else
                {
                    chipRegister.PPSDRVWrite(od, count, 0, arg.port, arg.address, arg.data);
                }

                return 0;
            }

            SoundManager.PackData p;
            if (arg.port == 0x05)
            {
                p = new SoundManager.PackData(
                    null, null, EnmDataType.Block, arg.address, arg.data, arg.addtionalData);
            }
            else
            {
                p = new SoundManager.PackData(
                    null, null, EnmDataType.Normal, arg.port, arg.address, arg.data);
            }

            psd.Add(p);
            return 0;
        }

        private int PPZ8Write(ChipDatum arg)
        {
            if (arg == null) return 0;

            if (!initPhase)
            {
                outDatum od = null;
                if (arg.addtionalData != null)
                {
                    if (arg.addtionalData is MmlDatum)
                    {
                        MmlDatum md = (MmlDatum)arg.addtionalData;
                        if (md.linePos != null) md.linePos.srcMMLID = filename;
                        od = new outDatum(md.type, md.args, md.linePos, (byte)md.dat);
                    }

                }

                if (arg.port == 0x03)
                {
                    chipRegister.PPZ8LoadPcm(od, count, 0, (byte)arg.address, (byte)arg.data, (byte[])arg.addtionalData);
                }
                else
                {
                    chipRegister.PPZ8Write(od, count, 0, arg.port, arg.address, arg.data);
                }

                return 0;
            }

            SoundManager.PackData p;
            if (arg.port == 0x03)
            {
                p = new SoundManager.PackData(
                    null, null, EnmDataType.Block, arg.address, arg.data, arg.addtionalData);
            }
            else
            {
                p = new SoundManager.PackData(
                    null, null, EnmDataType.Normal, arg.port, arg.address, arg.data);
            }

            pzd.Add(p);
            return 0;
        }

        private int P86Write(ChipDatum arg)
        {
            if (arg == null) return 0;

            if (!initPhase)
            {
                outDatum od = null;
                if (arg.addtionalData != null)
                {
                    if (arg.addtionalData is MmlDatum)
                    {
                        MmlDatum md = (MmlDatum)arg.addtionalData;
                        if (md.linePos != null) md.linePos.srcMMLID = filename;
                        od = new outDatum(md.type, md.args, md.linePos, (byte)md.dat);
                    }

                }

                if (arg.port == 0x00)
                {
                    chipRegister.P86LoadPcm(od, count, 0, (byte)arg.address, (byte)arg.data, (byte[])arg.addtionalData);
                }
                else
                {
                    chipRegister.P86Write(od, count, 0, arg.port, arg.address, arg.data);
                }

                return 0;
            }

            SoundManager.PackData p;
            if (arg.port == 0x00)
            {
                p = new SoundManager.PackData(
                    null, null, EnmDataType.Block, arg.address, arg.data, arg.addtionalData);
            }
            else
            {
                p = new SoundManager.PackData(
                    null, null, EnmDataType.Normal, arg.port, arg.address, arg.data);
            }

            p8d.Add(p);
            return 0;
        }

        private void OPNAWaitSend(long arg1, int arg2)
        {
            return;
        }

        private void OPNAWrite(ChipDatum dat)
        {
            //Log.WriteLine(LogLevel.TRACE, string.Format("FM P{2} Out:Adr[{0:x02}] val[{1:x02}]", (int)dat.address, (int)dat.data, dat.port));
            //Console.WriteLine("FM P{2} Out:Adr[{0:x02}] val[{1:x02}]", (int)dat.address, (int)dat.data, dat.port);
            outDatum od = null;
            if (dat.addtionalData != null)
            {
                if (dat.addtionalData is MmlDatum)
                {
                    MmlDatum md = (MmlDatum)dat.addtionalData;
                    if (md.linePos != null) md.linePos.srcMMLID = filename;
                    od = new outDatum(md.type, md.args, md.linePos, (byte)md.dat);
                }

            }

            //if (od != null && od.linePos != null)
            //{
            //Console.WriteLine("{0}", od.linePos.col);
            //}

            //chipRegister.YM2608SetRegister(od, (long)dat.time, 0, dat.port, dat.address, dat.data);
            chipRegister.YM2608SetRegister(od, count, 0, dat.port, dat.address, dat.data);
        }


        private void OPNAInitialWrite(musicDriverInterface.ChipDatum dat)
        {
            if (!initPhase)
            {
                OPNAWrite(dat);
                return;
            }

            SoundManager.PackData p = new SoundManager.PackData(null, null, EnmDataType.Block, dat.port * 0x100 + dat.address, dat.data, null);
            pd.Add(p);
        }

        public object GetOPNAPCMData()
        {
            object rpd= pd.ToArray();
            pd = null;
            return rpd;
        }

        public int GetNowLoopCounter()
        {
            return pm.GetNowLoopCounter();
        }

    }
}
