using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Core;
using mml2vgmIDE;

namespace SoundManager
{
    public delegate void DataSeqFrqEventHandler(long SeqCounter);

    public class DataSender : BaseSender
    {
        public event DataSeqFrqEventHandler OnDataSeqFrq;

        private static Stopwatch sw = Stopwatch.StartNew();
        private static readonly double swFreq = Stopwatch.Frequency;
        private readonly int Frq = DATA_SEQUENCE_FREQUENCE;
        private const long Def_SeqCounter = -1500;
        private long SeqCounter = Def_SeqCounter;
        private double SeqSpeed = 0.0;
        private double SeqSpeedDelta = 1.0;//0.2;

        private readonly Enq EmuEnq = null;
        private readonly Enq RealEnq = null;
        private PackData[] startData = null;
        private PackData[] stopData = null;
        private Deq ProcessingData;
        private Action WaitSync = null;
        private long EmuDelay = 0;
        private long RealDelay = 0;
        private bool reqSendStopData;

        public DataSender(
            Enq EmuEnq
            , Enq RealEnq
            , Deq ProcessingData
            , PackData[] startData
            , PackData[] stopData
            , DataSeqFrqEventHandler DataSeqFrqCallBack
            , Action WaitSync
            , long EmuDelay
            , long RealDelay
            , int BufferSize = DATA_SEQUENCE_FREQUENCE
            , int Frq = DATA_SEQUENCE_FREQUENCE
            )
        {
            action = Main;
            this.Frq = Frq;
            ringBuffer = new RingBuffer(BufferSize);
            ringBuffer.AutoExtend = false;
            this.ringBufferSize = BufferSize;
            SeqCounter = Def_SeqCounter;
            this.EmuEnq = EmuEnq;
            this.RealEnq = RealEnq;
            this.startData = startData;
            this.stopData = stopData;
            this.ProcessingData = ProcessingData;
            OnDataSeqFrq += DataSeqFrqCallBack;
            this.WaitSync = WaitSync;
            this.EmuDelay = EmuDelay;
            this.RealDelay = RealDelay;
        }

        public void ResetSeqCounter()
        {
            lock (lockObj)
            {
                SeqCounter = Def_SeqCounter;
            }
        }

        public long GetSeqCounter()
        {
            lock (lockObj)
            {
                return SeqCounter;
            }
        }

        public void SetSpeed(double sp)
        {
            lock (lockObj)
            {
                SeqSpeedDelta = sp;
            }
        }

        public double GetSpeed()
        {
            lock (lockObj)
            {
                return SeqSpeedDelta;
            }
        }

        public void Init()
        {
            SeqCounter = Def_SeqCounter;
            ringBuffer.Init(ringBufferSize);

            //開始時のデータの送信
            if (startData != null)
            {
                foreach (PackData dat in startData)
                {
                    //振り分けてEnqueue
                    if (dat.Chip.Model == EnmModel.VirtualModel)
                        while (!EmuEnq(dat.od, 0, dat.Chip, dat.Type, dat.Address, dat.Data, null)) Thread.Sleep(1);
                    else
                        while (!RealEnq(dat.od, 0, dat.Chip, dat.Type, dat.Address, dat.Data, null)) Thread.Sleep(1);
                }
            }

        }

        public new bool Enq(outDatum od, long Counter, Chip Chip, EnmDataType Type, int Address, int Data, object ExData)
        {
            if (Chip == null)
            {
                return ringBuffer.Enq(od, Counter, Chip, Type, Address, Data, ExData);
            }

            switch (Chip.Model)
            {
                case EnmModel.None:
                    return ringBuffer.Enq(od, Counter, Chip, Type, Address, Data, ExData);

                case EnmModel.VirtualModel:
                    return ringBuffer.Enq(od, Counter + EmuDelay, Chip, Type, Address, Data, ExData);

                case EnmModel.RealModel:
                    return ringBuffer.Enq(od, Counter + RealDelay, Chip, Type, Address, Data, ExData);

            }

            return ringBuffer.Enq(od, Counter, Chip, Type, Address, Data, ExData);
        }

        private void Main()
        {
            try
            {

                while (true)
                {
                    Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
                    while (!GetStart())
                    {
                        if (unmount) return;
                        Thread.Sleep(100);
                    }
                    Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;

                    WaitSync?.Invoke();

                    lock (lockObj) isRunning = true;

                    double o = sw.ElapsedTicks / swFreq;
                    double step = 1 / (double)Frq;
                    SeqCounter = Def_SeqCounter;

                    while (true)
                    {
                        if (!GetStart())
                            break;
                        if (unmount) return;
                        Thread.Sleep(0);

                        double el1 = sw.ElapsedTicks / swFreq;
                        if (el1 - o < step) continue;
                        if (el1 - o >= step * Frq / 100.0)//閾値10ms
                        {
                            do
                            {
                                o += step;
                            } while (el1 - o >= step);
                        }
                        else
                        {
                            o += step;
                        }

                        {
                            //待ち合わせ割り込み
                            if (parent.GetInterrupt())
                            {
                                //Thread.Sleep(0);
                                continue;
                            }

                            SeqSpeed += SeqSpeedDelta;
                            while (SeqSpeed >= 1.0)
                            {
                                SeqCounter++;
                                SeqSpeed -= 1.0;
                            }

                            if (SeqCounter < 0) continue;

                            //イベント実施
                            OnDataSeqFrq(SeqCounter);

                            //if (parent.Mode == SendMode.Both)
                            //{
                                //throw new ArgumentOutOfRangeException("演奏時は両モード同時指定できません");
                            //}
                            if ((parent.Mode & SendMode.MML) == SendMode.MML)
                            {
                                if (ringBuffer.GetDataSize() == 0)
                                {
                                    if (!parent.IsRunningAtDataMaker())
                                    {
                                        parent.ResetMode(SendMode.MML);
                                        if ((parent.Mode & SendMode.RealTime) != SendMode.RealTime)
                                            break;
                                    }
                                    continue;
                                }
                            }
                            if ((parent.Mode & SendMode.RealTime) == SendMode.RealTime)
                            {
                                if (reqSendStopData)
                                {
                                    reqSendStopData = false;
                                    if (SendStopData() == -1) return;
                                }
                            }
                            if (parent.Mode == SendMode.none)
                            {
                                //モード指定がnoneの場合はループを抜ける
                                break;
                            }
                        }

                        //dataが貯まってます！
                        while (SeqCounter >= ringBuffer.LookUpCounter())
                        {
                            if (unmount) return;
                            if (!ringBuffer.Deq(ref od, ref Counter, ref Chip, ref Type, ref Address, ref Data, ref ExData))
                            {
                                break;
                            }

                            //データ加工
                            ProcessingData?.Invoke(ref od, ref Counter, ref Chip, ref Type, ref Address, ref Data, ref ExData);

                            if (od != null && od.type == enmMMLType.Tempo)
                            {
                                parent.CurrentTempo = (int)od.args[0];
                                parent.CurrentClockCount = (int)od.args[1];
                                continue;
                            }

                            if (od != null && od.type == enmMMLType.Length)
                            {
                                if (od.linePos.chip == parent.CurrentChip
                                    && od.linePos.ch == parent.CurrentCh - 1)
                                {
                                    parent.CurrentNoteLength = (int)od.args[0];
                                }
                                continue;
                            }

                            //振り分けてEnqueue
                            if (Chip.Model == EnmModel.VirtualModel)
                            {
                                while (!EmuEnq(od, Counter, Chip, Type, Address, Data, ExData))
                                {
                                    if (!Start)
                                    {
                                        break;
                                    }
                                    if (unmount) return;
                                    Thread.Sleep(0);
                                }
                            }
                            else if (Chip.Model == EnmModel.RealModel)
                            {
                                while (!RealEnq(od, Counter, Chip, Type, Address, Data, ExData))
                                {
                                    if (!Start)
                                    {
                                        break;
                                    }
                                    if (unmount) return;
                                    Thread.Sleep(0);
                                }
                            }
                            else
                            {
                                //演奏制御処理
                                switch (Type)
                                {
                                    case EnmDataType.Normal:
                                        break;
                                    case EnmDataType.FadeOut:
                                        parent.SetFadeOut();
                                        break;
                                    case EnmDataType.Loop:
                                        parent.CountUpLoopCounter();
                                        break;
                                }
                            }
                        }
                    }

                    if (SendStopData() == -1) return;

                    lock (lockObj)
                    {
                        isRunning = false;
                        Counter = 0;
                        Start = false;
                    }
                    parent.RequestStopAtEmuChipSender();
                    parent.RequestStopAtRealChipSender();
                }
            }
            catch(Exception ex)
            {
                mml2vgmIDE.log.ForcedWrite(ex);
                lock (lockObj)
                {
                    isRunning = false;
                    Start = false;
                }
            }
            finally
            {
                procExit = true;
                Thread.CurrentThread.Priority = ThreadPriority.Normal;
            }
        }

        private int SendStopData()
        {
            if (stopData == null) return 0;

            foreach (PackData dat in stopData)
            {
                //データ加工
                ProcessingData?.Invoke(ref dat.od, ref SeqCounter, ref dat.Chip, ref dat.Type, ref dat.Address, ref dat.Data, ref dat.ExData);

                //振り分けてEnqueue
                if (dat.Chip.Model == EnmModel.VirtualModel)
                {
                    if (parent.IsRunningAtEmuChipSender())
                    {
                        int timeOut = 1000;
                        while (!EmuEnq(dat.od, SeqCounter, dat.Chip, dat.Type, dat.Address, dat.Data, dat.ExData))
                        {
                            if (unmount) return -1;

                            Thread.Sleep(1);
                            timeOut--;
                            if (timeOut == 0) return 0;
                        }
                    }
                }
                else if (dat.Chip.Model == EnmModel.RealModel)
                {
                    if (parent.IsRunningAtRealChipSender())
                    {
                        int timeOut = 1000;
                        while (!RealEnq(dat.od, SeqCounter, dat.Chip, dat.Type, dat.Address, dat.Data, dat.ExData))
                        {
                            if (unmount) return -1;

                            Thread.Sleep(1);
                            timeOut--;
                            if (timeOut == 0) return 0;
                        }
                    }
                }
            }

            return 0;
        }

        public void SetStopData(PackData[] stopData)
        {
            this.stopData = stopData;
        }

        public void ClearBuffer()
        {
            lock (lockObj)
            {
                reqSendStopData = true;
                ringBuffer.Init(ringBufferSize);
            }
        }
    }

}