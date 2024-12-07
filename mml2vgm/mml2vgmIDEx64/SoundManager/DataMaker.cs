using mml2vgmIDEx64;
using System;
using System.Threading;

namespace SoundManager
{
    /// <summary>
    /// データ生成器
    /// ミュージックドライバーを駆動させ、生成するデータをDataSenderに送る
    /// </summary>
    public class DataMaker : BaseMakerSender
    {
        private readonly DriverAction ActionOfDriver;
        private bool pause = false;

        public SendMode Mode { get; internal set; }

        public DataMaker(DriverAction ActionOfDriver)
        {
            action = Main;
            this.ActionOfDriver = ActionOfDriver;
        }

        private void Main()
        {
            try
            {
                Thread.CurrentThread.Name = "DataMaker";
                while (true)
                {

                    pause = false;

                    while (true)
                    {
                        Thread.Sleep(100);
                        if (GetStart())
                        {
                            break;
                        }
                        if (unmount) return;
                    }

                    lock (lockObj) isRunning = true;

                    ActionOfDriver?.Init?.Invoke();

                    while (true)
                    {
                        if (!GetStart())
                            break;
                        if (unmount) return;

                        if (pause)
                        {
                            if (parent.GetDataSenderBufferSize() >= DATA_SEQUENCE_FREQUENCE / 2)
                            {
                                Thread.Sleep(10);
                                continue;
                            }

                            pause = false;
                        }

                        Thread.Sleep(0);
                        ActionOfDriver?.Main?.Invoke();

                        if (parent.GetDataSenderBufferSize() >= DATA_SEQUENCE_FREQUENCE)
                        {
                            pause = true;
                        }
                    }

                    ActionOfDriver?.Final?.Invoke();

                    lock (lockObj) isRunning = false;

                }
            }
            catch(Exception e)
            {
                lock (lockObj)
                {
                    isRunning = false;
                    Start = false;
                }

                log.ForcedWrite(e);
            }
            finally
            {
                procExit = true;
                unmount = true;
            }
        }

        internal void RequestChangeMode(SendMode mode)
        {
            throw new NotImplementedException();
        }
    }

}
