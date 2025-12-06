using Corex64;
using musicDriverInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mml2vgmIDEx64
{
    public class muapM : baseDriver
    {
        public uint YM2608ClockValue { get; set; } = 7987200;
        public uint YM2612ClockValue { get; set; } = 7987200;

        private musicDriverInterface.MmlDatum[] mupBuf = null;
        private MuapManager muapManager = null;
        private bool initPhase = true;
        List<SoundManager.PackData>[] pd =
            new List<SoundManager.PackData>[] {
                new List<SoundManager.PackData>()
                , new List<SoundManager.PackData>()
                , new List<SoundManager.PackData>()
            };
        private long count = 0;
        private SoundManager.Chip chipYM2608;
        private SoundManager.Chip chipYM2612;
        private SoundManager.Chip chipCS4231;
        private string filename = "";
        private string oldLyrics;
        private int oldComLength;
        public List<Tuple<long, int, string>> lyrics = null;

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

                chipRegister.YM2608SetRegister(null, count, chipYM2608, pd[0].ToArray());
                chipRegister.YM2612SetRegister(null, count, chipYM2612, pd[1].ToArray());
                chipRegister.CS4231SetRegister(null, count, chipCS4231, pd[2].ToArray());

                return;
            }

            muapManager.Rendering();
            Audio.DriverSeqCounter = count;
            //vgmCurLoop = (uint)GetNowLoopCounter();

            //歌詞取得
            string ly = muapManager.lyrics;
            int llen = muapManager.comlength;
            if (ly != oldLyrics || llen != oldComLength)
            {
                oldLyrics = ly;
                oldComLength = llen;
                lyrics ??= new List<Tuple<long, int, string>>();
                lyrics.Add(new Tuple<long, int, string>(count, llen, ly));//カウンタ込みの歌詞情報をため込む
            }

            if (muapManager.Stopped)
            {
                Stopped = true;
                return;
            }
            count++;
        }

        public bool init(MmlDatum[] mupBuf
            , string mupWorkPath
            , MuapManager muapManager
            , ChipRegister chipRegister
            , EnmChip[] enmChips
            , uint latency
            , uint waitTime
            , string mupFileName)
        {

            if (muapManager == null) return false;

            this.vgmBuf = null;
            this.mupBuf = mupBuf;
            this.chipRegister = chipRegister;
            //this.useChip = useChip;
            //this.latency = latency;
            //this.waitTime = waitTime;
            this.muapManager = muapManager;
            chipYM2608 = chipRegister.YM2608[0];
            chipYM2612 = chipRegister.YM2612[0];
            chipCS4231 = chipRegister.CS4231[0];
            filename = mupFileName;

            Counter = 0;
            TotalCounter = 0;
            LoopCounter = 0;
            vgmCurLoop = 0;
            Stopped = false;
            vgmFrameCounter = -latency - waitTime;
            vgmSpeed = 1;
            vgmSpeedCounter = 0;
            oldLyrics = "";
            oldComLength = 0;

            initPhase = true;
            pd[0] = new List<SoundManager.PackData>();
            pd[1] = new List<SoundManager.PackData>();
            pd[2] = new List<SoundManager.PackData>();


            //Driverの初期化
            muapManager.InitDriver(
                System.IO.Path.Combine(mupWorkPath, "dummy")
                , OPNAInitialWrite
                , OPNBInitialWrite
                , CS4231InitialWrite
                , CS4231InitialRead
                , CS4231EMS_GetCrntMapBuf
                , CS4231EMS_Map
                , CS4231EMS_GetPageMap
                , CS4231EMS_GetHandleName
                , CS4231EMS_SetHandleName
                , CS4231EMS_AllocMemory
                , OPNAWaitSend
                , mupBuf
                , chipRegister
                , mupWorkPath
                );

            muapManager.StartRendering(Common.DataSequenceSampleRate, (int)YM2608ClockValue);
            muapManager.MSTART(0);

            return true;
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

            //if (pcmdata.Count > 0)
            //{
            //    chipRegister.YMF278BSetRegister(od, count, 0, pcmdata.ToArray());
            //    pcmdata.Clear();
            //}

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

        private void OPNBWrite(ChipDatum dat)
        {
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

            chipRegister.YM2612SetRegister(od, count, 0, dat.port, dat.address, dat.data);
        }

        private void CS4231Write(ChipDatum dat)
        {
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

            chipRegister.CS4231SetRegister(od, count, 0, dat.port, dat.address, dat.data);
        }

        private byte CS4231Read(byte dat)
        {
            // muapが読みに来るアドレス
            //  0 ... indexAddress writeで書き込んだ値をそのまま返せばOK
            //  1 ... レジスタ      writeで書き込んだ値をそのまま返せばOK
            //  2 ... status       0を返せばOK
            //  4 ... dmaInt　　　　writeで書き込んだ値をそのまま返せばOK
            return chipRegister.CS4231GetRegister(dat);//即時読み取り専用
        }

        private void OPNAInitialWrite(musicDriverInterface.ChipDatum dat)
        {
            if (!initPhase)
            {
                OPNAWrite(dat);
                return;
            }

            SoundManager.PackData p;
            p = new SoundManager.PackData(
                null,
                chipRegister.YM2608[0], EnmDataType.Block, dat.port * 0x100 + dat.address, dat.data, null);
            pd[0].Add(p);
        }

        private void OPNBInitialWrite(musicDriverInterface.ChipDatum dat)
        {
            if (!initPhase)
            {
                OPNBWrite(dat);
                return;
            }

            SoundManager.PackData p = new SoundManager.PackData(
                null,
                chipRegister.YM2612[0], EnmDataType.Block, dat.port * 0x100 + dat.address, dat.data, null);
            pd[1].Add(p);
        }

        private void CS4231InitialWrite(musicDriverInterface.ChipDatum dat)
        {
            if (!initPhase)
            {
                CS4231Write(dat);
                return;
            }

            CS4231Write(dat);
            SoundManager.PackData p = new SoundManager.PackData(
                null,
                chipRegister.CS4231[0], EnmDataType.Block, dat.port * 0x100 + dat.address, dat.data, null);
            pd[2].Add(p);
        }

        private byte CS4231InitialRead(byte dat)
        {
            return CS4231Read(dat);
        }

        private byte[] CS4231EMS_GetCrntMapBuf()
        {
            return chipRegister.getCS4231EMS_GetCrntMapBuf(0);
        }

        private void CS4231EMS_Map(byte al, ref byte ah, ushort bx, ushort dx)
        {
            chipRegister.setCS4231EMS_Map(0, al, ref ah, bx, dx);
        }

        private ushort CS4231EMS_GetPageMap()
        {
            return chipRegister.getCS4231EMS_GetPageMap(0);
        }

        private void CS4231EMS_GetHandleName(ref byte ah, ushort dx, ref string sbuf)
        {
            chipRegister.getCS4231EMS_GetHandleName(0, ref ah, dx, ref sbuf);
        }

        private void CS4231EMS_SetHandleName(ref byte ah, ushort dx, string emsname2)
        {
            chipRegister.setCS4231EMS_SetHandleName(0, ref ah, dx, emsname2);
        }

        private void CS4231EMS_AllocMemory(ref byte ah, ref ushort dx, ushort bx)
        {
            chipRegister.setCS4231EMS_AllocMemory(0, ref ah, ref dx, bx);
        }

        public List<Tuple<string, string>> GetTags()
        {
            if (chipRegister == null) return null;
            return muapManager.GetTags();
        }
    }
}
