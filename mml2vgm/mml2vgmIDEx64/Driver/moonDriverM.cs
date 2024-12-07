using Corex64;
using musicDriverInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mml2vgmIDEx64
{
    public class moonDriverM : baseDriver
    {
        public uint YMF278BClockValue { get; internal set; } = 33868800;

        private musicDriverInterface.MmlDatum[] mdrBuf = null;
        private MoonDriverManager MoonDriverManager = null;
        private bool initPhase = true;
        private List<SoundManager.PackData> pcmdata = new List<SoundManager.PackData>();
        private long count = 0;
        private SoundManager.Chip chipYMF278B;
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

                //if (pcmdata != null) chipRegister.YM2608SetRegister(null, count, chipYM2608, pd.ToArray());

                //OPL4 init
                chipRegister.YMF278BSetRegister(null, count, 0, 1, 0x04, 0x00);
                chipRegister.YMF278BSetRegister(null, count, 0, 1, 0x05, 0x03);
                chipRegister.YMF278BSetRegister(null, count, 0, 0, 0xbd, 0x00);
                chipRegister.YMF278BSetRegister(null, count, 0, 2, 0x02, 0x10);

                return;
            }

            MoonDriverManager.Rendering();
            Audio.DriverSeqCounter = count;
            //vgmCurLoop = (uint)GetNowLoopCounter();

            if (MoonDriverManager.Stopped)
            {
                Stopped = true;
                return;
            }
            count++;
        }

        public bool init(MmlDatum[] mdrBuf
            , string mdrWorkPath
            , MoonDriverManager moonDriverManager
            , ChipRegister chipRegister
            , EnmChip[] enmChips
            , uint v1, uint v2
            , string mdrFileName)
        {

            if (moonDriverManager == null) return false;

            this.vgmBuf = null;
            this.mdrBuf = mdrBuf;
            this.chipRegister = chipRegister;
            //this.useChip = useChip;
            //this.latency = latency;
            //this.waitTime = waitTime;
            this.MoonDriverManager = moonDriverManager;
            chipYMF278B = chipRegister.YMF278B[0];
            filename = mdrFileName;

            Counter = 0;
            TotalCounter = 0;
            LoopCounter = 0;
            vgmCurLoop = 0;
            Stopped = false;
            vgmFrameCounter = -latency - waitTime;
            vgmSpeed = 1;
            vgmSpeedCounter = 0;

            initPhase = true;
            pcmdata = new List<SoundManager.PackData>();


            //Driverの初期化
            moonDriverManager.InitDriver(
                System.IO.Path.Combine(mdrWorkPath, "dummy")
                , OPL4InitialWrite
                , OPL4WaitSend
                , mdrBuf
                , chipRegister
                );

            moonDriverManager.StartRendering(Common.DataSequenceSampleRate, (int)YMF278BClockValue);
            moonDriverManager.MSTART(0);

            return true;
        }

        private void OPL4WaitSend(long arg1, int arg2)
        {
            return;
        }

        private void OPL4Write(ChipDatum dat)
        {
            //Log.WriteLine(LogLevel.TRACE, string.Format("FM P{2} Out:Adr[{0:x02}] val[{1:x02}]", (int)dat.address, (int)dat.data, dat.port));
            //Console.WriteLine("FM P{2} Out:Adr[{0:x02}] val[{1:x02}]", (int)dat.address, (int)dat.data, dat.port);
            outDatum od = null;

            if (pcmdata.Count > 0)
            {
                chipRegister.YMF278BSetRegister(od, count, 0, pcmdata.ToArray());
                pcmdata.Clear();
            }

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
            chipRegister.YMF278BSetRegister(od, count, 0, dat.port, dat.address, dat.data);
        }


        private void OPL4InitialWrite(musicDriverInterface.ChipDatum dat)
        {
            if (!initPhase)
            {
                OPL4Write(dat);
                return;
            }

            SoundManager.PackData p = new SoundManager.PackData(null,chipRegister.YMF278B[0], EnmDataType.Block, dat.port * 0x100 + dat.address, dat.data, null);
            pcmdata.Add(p);
        }


    }
}
