using Core;
using musicDriverInterface;
using System;
using System.Collections.Generic;

namespace mml2vgmIDE
{
    public class mucomMub : baseDriver
    {
        public uint YM2608ClockValue { get; internal set; } = 7987200;
        public uint YM2610ClockValue { get; internal set; } = 8000000;
        private musicDriverInterface.MmlDatum[] mubBuf = null;
        private mucomManager mm = null;
        bool initPhase = true;
        List<SoundManager.PackData>[] pd = new List<SoundManager.PackData>[] { new List<SoundManager.PackData>(), new List<SoundManager.PackData>(), new List<SoundManager.PackData>(), new List<SoundManager.PackData>() };
        byte[][] pdOPNB = new byte[4][];
        private SoundManager.Chip chipYM2608;
        private SoundManager.Chip chipYM2608S;
        private SoundManager.Chip chipYM2610;
        private SoundManager.Chip chipYM2610S;
        private string filename = "";

        public override GD3 getGD3Info(byte[] buf, uint vgmGd3)
        {
            throw new NotImplementedException();
        }

        public override bool init(outDatum[] vgmBuf, ChipRegister chipRegister, EnmChip[] useChip, uint latency, uint waitTime, long jumpPointClock)
        {
            throw new NotImplementedException();
        }

        long count = 0;

        public override void oneFrameProc()
        {
            if (initPhase)
            {
                initPhase = false;
                chipRegister.YM2608SetRegister(null, count, chipYM2608, pd[0].ToArray());
                chipRegister.YM2608SetRegister(null, count, chipYM2608S, pd[1].ToArray());
                chipRegister.YM2608SetRegister(null, count, chipYM2610, pd[2].ToArray());
                chipRegister.YM2608SetRegister(null, count, chipYM2610S, pd[3].ToArray());

                chipRegister.YM2610SetRegister(null, count, chipYM2610, pdOPNB[0],true);
                chipRegister.YM2610SetRegister(null, count, chipYM2610, pdOPNB[1],false);
                chipRegister.YM2610SetRegister(null, count, chipYM2610S, pdOPNB[2], true);
                chipRegister.YM2610SetRegister(null, count, chipYM2610S, pdOPNB[3],false);
                return;
            }

            mm.Rendering();
            Audio.DriverSeqCounter = count;
            vgmCurLoop = (uint)GetNowLoopCounter();

            if (mm.Stopped)
            {
                Stopped = true;
            }
            count++;
        }

        public bool init(musicDriverInterface.MmlDatum[] mubBuf, string workPath, mucomManager mucomManager, ChipRegister chipRegister
            , EnmChip[] useChip
            , uint latency
            , uint waitTime
            , string fn
            )
        {
            if (mucomManager == null) return false;

            this.vgmBuf = null;
            this.mubBuf = mubBuf;
            this.chipRegister = chipRegister;
            this.useChip = useChip;
            this.latency = latency;
            this.waitTime = waitTime;
            this.mm = mucomManager;
            chipYM2608 = chipRegister.YM2608[0];
            chipYM2608S = chipRegister.YM2608[1];
            chipYM2610 = chipRegister.YM2610[0];
            chipYM2610S = chipRegister.YM2610[1];
            filename = fn;

            Counter = 0;
            TotalCounter = 0;
            LoopCounter = 0;
            vgmCurLoop = 0;
            Stopped = false;
            vgmFrameCounter = -latency - waitTime;
            vgmSpeed = 1;
            vgmSpeedCounter = 0;

            initPhase = true;
            pd[0] = new List<SoundManager.PackData>();
            pd[1] = new List<SoundManager.PackData>();
            pd[2] = new List<SoundManager.PackData>();
            pd[3] = new List<SoundManager.PackData>();
            pdOPNB[0] = null;
            pdOPNB[1] = null;
            pdOPNB[2] = null;
            pdOPNB[3] = null;

            //Driverの初期化
            mm.InitDriver(
                System.IO.Path.Combine(workPath, "dummy")
                , new List<Action<musicDriverInterface.ChipDatum>>() {
                    OPNAWriteP
                    ,OPNAWriteS
                    ,OPNBWriteP
                    ,OPNBWriteS
                }
                , new List<Action<byte[], int, int>>() { 
                    null
                    , null
                    , OPNBWriteAdpcmP
                    , OPNBWriteAdpcmS
                }
                , OPNAWaitSend
                , false, mubBuf, true, false);
            mm.StartRendering((int)Common.SampleRate, (int)YM2608ClockValue);
            mm.MSTART(0);

            return true;
        }

        private void OPNAWaitSend(long arg1, int arg2)
        {
            return;
        }

        private void OPNAWrite(int ChipID,ChipDatum dat)
        {
            //Log.WriteLine(LogLevel.TRACE, string.Format("FM P{2} Out:Adr[{0:x02}] val[{1:x02}]", (int)dat.address, (int)dat.data, dat.port));
            //Console.WriteLine("FM P{2} Out:Adr[{0:x02}] val[{1:x02}]", (int)dat.address, (int)dat.data, dat.port);
            outDatum od = null;
            if (dat.addtionalData != null)
            {
                if (dat.addtionalData is musicDriverInterface.MmlDatum)
                {
                    musicDriverInterface.MmlDatum md = (musicDriverInterface.MmlDatum)dat.addtionalData;
                    if (md.linePos != null) md.linePos.srcMMLID = filename;
                    od = new outDatum(md.type, md.args, md.linePos, (byte)md.dat);
                }

            }

            //if (od != null && od.linePos != null)
            //{
            //Console.WriteLine("{0}", od.linePos.col);
            //}

            //chipRegister.YM2608SetRegister(od, (long)dat.time, 0, dat.port, dat.address, dat.data);
            chipRegister.YM2608SetRegister(od, count, ChipID, dat.port, dat.address, dat.data);
        }

        private void OPNBWrite(int ChipID, ChipDatum dat)
        {
            //Log.WriteLine(LogLevel.TRACE, string.Format("FM P{2} Out:Adr[{0:x02}] val[{1:x02}]", (int)dat.address, (int)dat.data, dat.port));
            //Console.WriteLine("FM P{2} Out:Adr[{0:x02}] val[{1:x02}]", (int)dat.address, (int)dat.data, dat.port);
            outDatum od = null;
            if (dat.addtionalData != null)
            {
                if (dat.addtionalData is musicDriverInterface.MmlDatum)
                {
                    musicDriverInterface.MmlDatum md = (musicDriverInterface.MmlDatum)dat.addtionalData;
                    if (md.linePos != null) md.linePos.srcMMLID = filename;
                    od = new outDatum(md.type, md.args, md.linePos, (byte)md.dat);
                }

            }

            //if (od != null && od.linePos != null)
            //{
            //Console.WriteLine("{0}", od.linePos.col);
            //}

            //chipRegister.YM2608SetRegister(od, (long)dat.time, 0, dat.port, dat.address, dat.data);
            chipRegister.YM2610SetRegister(od, count, ChipID, dat.port, dat.address, dat.data);
        }

        private void OPNAInitialWrite(int ChipID, ChipDatum dat)
        {
            if (!initPhase)
            {
                OPNAWrite(ChipID, dat);
                return;
            }

            SoundManager.PackData p = new SoundManager.PackData(null, null, EnmDataType.Block, dat.port * 0x100 + dat.address, dat.data, null);
            p.Chip.Number = ChipID;
            pd[ChipID].Add(p);
        }
        private void OPNBInitialWrite(int ChipID, ChipDatum dat)
        {
            if (!initPhase)
            {
                OPNBWrite(ChipID, dat);
                return;
            }

            SoundManager.PackData p = new SoundManager.PackData(null, null, EnmDataType.Block, dat.port * 0x100 + dat.address, dat.data, null);
            pd[ChipID+2].Add(p);
        }

        public int GetNowLoopCounter()
        {
            return mm.GetNowLoopCounter();
        }

        private void OPNAWriteP(ChipDatum dat)
        {
            OPNAInitialWrite(0, dat);
        }
        private void OPNAWriteS(ChipDatum dat)
        {
            OPNAInitialWrite(1, dat);
        }
        private void OPNBWriteP(ChipDatum dat)
        {
            OPNBInitialWrite(0, dat);
        }
        private void OPNBWriteS(ChipDatum dat)
        {
            OPNBInitialWrite(1, dat);
        }
        private void OPNBWriteAdpcmP(byte[] pcmData, int s, int e)
        {
            if (s == 0)
            {

                pdOPNB[0] = pcmData;

                //outDatum od = null;
                //chipRegister.YM2610WriteSetAdpcmA(od, count, 0, pcmData);
            }
            else
            {
                pdOPNB[1] = pcmData;

                //outDatum od = null;
                //chipRegister.YM2610WriteSetAdpcmB(od, count, 0, pcmData);
                //OPNBWrite_AdpcmB(0, pcmData);
            }
        }
        private void OPNBWriteAdpcmS(byte[] pcmData, int s, int e)
        {
            if (s == 0)
            {
                pdOPNB[2] = pcmData;

                //outDatum od = null;
                //chipRegister.YM2610WriteSetAdpcmA(od, count, 1, pcmData);
                //OPNBWrite_AdpcmA(0, pcmData);
            }
            else
            {
                pdOPNB[3] = pcmData;

                //outDatum od = null;
                //chipRegister.YM2610WriteSetAdpcmB(od, count, 1, pcmData);
                //OPNBWrite_AdpcmB(0, pcmData);
            }
            //if (s == 0) OPNBWrite_AdpcmA(1, pcmData);
            //else OPNBWrite_AdpcmB(1, pcmData);
        }

    }
}
