using System.Threading;

namespace SoundManager
{
    public class RealChipSender : ChipSender
    {
        public RealChipSender(Snd ActionOfRealChip, int BufferSize = DATA_SEQUENCE_FREQUENCE)
            : base(ActionOfRealChip, BufferSize)
        {
        }

        protected override void Main()
        {
            try
            {
                Thread.CurrentThread.Name = "RealChipSender";
                while (true)
                {
                    Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
                    while (!GetStart())
                    {
                        Thread.Sleep(100);
                        if (unmount) return;
                    }
                    Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;

                    lock (lockObj) isRunning = true;

                    while (true)
                    {
                        if (unmount) return;
                        Thread.Sleep(1);
                        if (ringBuffer.GetDataSize() == 0)
                        {
                            //送信データが無く、停止指示がある場合のみ停止する
                            if (!GetStart()) break;
                            continue;
                        }

                        //dataが貯まってます！
                        lock (lockObj)
                        {
                            busy = true;
                        }

                        try
                        {
                            while (ringBuffer.Deq(ref od, ref Counter, ref Chip, ref Type, ref Address, ref Data, ref ExData))
                            {
                                ActionOfChip?.Invoke(od, Counter, Chip, Type, Address, Data, ExData);
                                if (unmount) return;
                            }
                        }
                        catch
                        {

                        }

                        lock (lockObj)
                        {
                            busy = false;
                        }
                    }

                    lock (lockObj)
                    {
                        isRunning = false;
                        ringBuffer.Init(ringBufferSize);
                    }

                }
            }
            catch
            {
                lock (lockObj)
                {
                    isRunning = false;
                    Start = false;
                }
            }
            finally
            {
                procExit = true;
            }
        }
    }

}
