using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mml2vgmIDE
{
    public class mucomMub : baseDriver
    {
        public uint YM2608ClockValue { get; internal set; } = 7987200;
        private musicDriverInterface.MmlDatum[] mubBuf = null;
        private mucomManager mm = null;
        bool initPhase = true;
        List<SoundManager.PackData> pd = new List<SoundManager.PackData>();
        private SoundManager.Chip chipYM2608;

        public override GD3 getGD3Info(byte[] buf, uint vgmGd3)
        {
            throw new NotImplementedException();
        }

        public override bool init(outDatum[] vgmBuf, ChipRegister chipRegister, EnmChip[] useChip, uint latency, uint waitTime)
        {
            throw new NotImplementedException();
        }

        long count = 0;

        public override void oneFrameProc()
        {
            if (initPhase)
            {
                initPhase = false;
                chipRegister.YM2608SetRegister(null, count, chipYM2608, pd.ToArray());
                return;
            }
            
            mm.Rendering();
            count++;
        }

        public bool init(musicDriverInterface.MmlDatum[] mubBuf,string workPath, mucomManager mucomManager, ChipRegister chipRegister, EnmChip[] useChip, uint latency, uint waitTime)
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

            Counter = 0;
            TotalCounter = 0;
            LoopCounter = 0;
            vgmCurLoop = 0;
            Stopped = false;
            vgmFrameCounter = -latency - waitTime;
            vgmSpeed = 1;
            vgmSpeedCounter = 0;

            initPhase = true;
            pd = new List<SoundManager.PackData>();

            //Driverの初期化
            mm.InitDriver(System.IO.Path.Combine(workPath, "dummy"), OPNAInitialWrite, OPNAWaitSend, false, mubBuf, true, false);
            mm.StartRendering((int)Common.SampleRate, (int)YM2608ClockValue);
            mm.MSTART(0);

            return true;
        }

        private void OPNAWaitSend(long arg1, int arg2)
        {
            return;
        }

        private void OPNAWrite(musicDriverInterface.ChipDatum dat)
        {
            //Log.WriteLine(LogLevel.TRACE, string.Format("FM P{2} Out:Adr[{0:x02}] val[{1:x02}]", (int)dat.address, (int)dat.data, dat.port));
            //Console.WriteLine("FM P{2} Out:Adr[{0:x02}] val[{1:x02}]", (int)dat.address, (int)dat.data, dat.port);
            outDatum od = null;
            if (dat.addtionalData != null)
            {
                if(dat.addtionalData is musicDriverInterface.MmlDatum)
                {
                    musicDriverInterface.MmlDatum md = (musicDriverInterface.MmlDatum)dat.addtionalData;
                    od = new outDatum(md.type, md.args, md.linePos, (byte)md.dat);
                }
                
            }

            if (od != null && od.linePos != null)
            {
                Console.WriteLine("{0}", od.linePos.col);
            }

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

    }
}
