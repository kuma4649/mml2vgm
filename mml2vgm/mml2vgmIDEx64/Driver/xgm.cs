using Corex64;
using System;

namespace mml2vgmIDEx64
{
    public class xgm : baseDriver
    {
        public const int FCC_XGM = 0x204d4758;	// "XGM "
        public const int FCC_GD3 = 0x20336447;  // "Gd3 "
        public readonly int SN76489ClockValue = 3579545;
        public readonly int YM2612ClockValue = 7670454;

        public MDSound.XGMFunction.XGMSampleID[] sampleID = new MDSound.XGMFunction.XGMSampleID[63];
        public uint sampleDataBlockSize = 0;
        public uint sampleDataBlockAddr = 0;
        private uint musicDataBlockSize = 0;
        private uint musicDataBlockAddr = 0;
        private byte dataInformation = 0;
        private bool isNTSC = false;
        private bool existGD3 = false;
        private uint gd3InfoStartAddr = 0;
        public MDSound.XGMFunction.XGMPCM[] xgmpcm = null;
        private double musicStep = Common.DataSequenceSampleRate / 60.0;
        private double musicDownCounter = 0.0;
        private uint musicPtr = 0;

        public override bool init(outDatum[] xgmBuf, ChipRegister chipRegister, EnmChip[] useChip, uint latency, uint waitTime, long jumpPointClock)
        {

            this.vgmBuf = xgmBuf;
            this.chipRegister = chipRegister;
            this.useChip = useChip;
            this.latency = latency;
            this.waitTime = waitTime;
            this.jumpPointClock = jumpPointClock;

            Counter = 0;
            TotalCounter = 0;
            LoopCounter = 0;
            vgmCurLoop = 0;
            Stopped = false;
            vgmFrameCounter = -latency - waitTime;
            vgmSpeed = 1;
            vgmSpeedCounter = 0;

            if (!getXGMInfo(vgmBuf)) return false;

            //Driverの初期化
            musicPtr = musicDataBlockAddr;
            xgmpcm = new MDSound.XGMFunction.XGMPCM[] {
                new MDSound.XGMFunction.XGMPCM()
                , new MDSound.XGMFunction.XGMPCM()
                , new MDSound.XGMFunction.XGMPCM()
                , new MDSound.XGMFunction.XGMPCM() 
            };

            return true;
        }

        public override void oneFrameProc()
        {
            try
            {
                vgmSpeedCounter += vgmSpeed;
                while (vgmSpeedCounter >= 1.0 && !Stopped)
                {
                    vgmSpeedCounter -= 1.0;
                    if (vgmFrameCounter > -1)
                    {
                        oneFrameMain();
                    }
                    else
                    {
                        vgmFrameCounter++;
                    }
                }
                //Stopped = !IsPlaying();
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
        }

        public GD3 getGD3Info(outDatum[] buf, uint vgmGd3)
        {
            getXGMInfo(buf);
            return GD3;
        }

        private GD3 getGD3Info(outDatum[] vgmBuf)
        {
            if (!existGD3) return new GD3();

            GD3 GD3 = Common.getGD3Info(vgmBuf, gd3InfoStartAddr + 12);
            GD3.UsedChips = UsedChips;

            return GD3;
        }

        private bool getXGMInfo(outDatum[] vgmBuf)
        {
            if (vgmBuf == null) return false;

            try
            {
                if (Common.getLE32(vgmBuf, 0) != FCC_XGM) return false;

                for (uint i = 0; i < 63; i++)
                {
                    sampleID[i] = new MDSound.XGMFunction.XGMSampleID();
                    sampleID[i].addr = (Common.getLE16(vgmBuf, i * 4 + 4) * 256);
                    sampleID[i].size = (Common.getLE16(vgmBuf, i * 4 + 6) * 256);
                }

                sampleDataBlockSize = Common.getLE16(vgmBuf, 0x100);
                //versionInformation = vgmBuf[0x102].val;
                dataInformation = vgmBuf[0x103].val;
                isNTSC = (dataInformation & 0x1) == 0;
                existGD3 = (dataInformation & 0x2) != 0;
                //multiTrackFile = (dataInformation & 0x4) != 0;
                sampleDataBlockAddr = 0x104;
                musicDataBlockSize = Common.getLE32(vgmBuf, sampleDataBlockAddr + sampleDataBlockSize * 256);
                musicDataBlockAddr = sampleDataBlockAddr + sampleDataBlockSize * 256 + 4;
                gd3InfoStartAddr = musicDataBlockAddr + musicDataBlockSize;
                GD3 = getGD3Info(vgmBuf);

                if (musicDataBlockSize == 0)
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                log.Write(string.Format("XGMの情報取得中に例外発生 Message=[{0}] StackTrace=[{1}]", e.Message, e.StackTrace));
                return false;
            }

            return true;
        }

        public bool IsPlaying()
        {
            return true;
        }

        private void oneFrameMain()
        {
            try
            {

                Counter++;
                vgmFrameCounter++;
                Audio.DriverSeqCounter++;

                musicStep = Common.DataSequenceSampleRate / (isNTSC ? 60.0 : 50.0);

                if (musicDownCounter <= 0.0)
                {
                    //xgm処理
                    oneFrameXGM();
                    musicDownCounter += musicStep;
                }
                musicDownCounter -= 1.0;

            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
        }

        private void oneFrameXGM()
        {
            while (true)
            {
                outDatum cmd = vgmBuf[musicPtr++].Copy();

                //wait
                if (cmd.val == 0)
                {
                    if (jumpPointClock < 0) break;
                    jumpPointClock--;//= (long)musicStep;
                }

                //loop command
                if (cmd.val == 0x7e)
                {
                    musicPtr = musicDataBlockAddr + Common.getLE24(vgmBuf, musicPtr);
                    vgmCurLoop++;
                    continue;
                }

                //end command
                if (cmd.val == 0x7f)
                {
                    Stopped = true;
                    break;
                }

                //TODO: Dummy Command
                if (cmd.val == 0x60 && Corex64.Common.CheckDummyCommand(cmd.type))
                {
                    chipRegister.YM2612SetRegister(cmd, Audio.DriverSeqCounter, 0, 0, -1, -1);
                    musicPtr += 2;
                    continue;
                }

                byte L = (byte)(cmd.val & 0xf);
                byte H = (byte)(cmd.val & 0xf0);

                if (H == 0x10)
                {
                    //PSG register write:
                    WritePSG(cmd, L);
                }
                else if (H == 0x20)
                {
                    //Console.WriteLine("cmd{0:x2}", cmd.val);
                    //YM2612 port 0 register write:
                    WriteYM2612P0(cmd, L);
                }
                else if (H == 0x30)
                {
                    //YM2612 port 1 register write:
                    WriteYM2612P1(cmd, L);
                }
                else if (H == 0x40)
                {
                    //YM2612 key off/on ($28) command write:
                    WriteYM2612Key(cmd, L);
                }
                else if (H == 0x50)
                {
                    //PCM play command:
                    //ポート10をXGMPCMの操作用として使用する!
                    chipRegister.YM2612SetRegister(null, Audio.DriverSeqCounter, 0, 10, L, vgmBuf[musicPtr++].val);
                }
            }
        }

        private void WritePSG(outDatum cmd, byte X)
        {
            for (int i = 0; i < X + 1; i++)
            {
                outDatum od = vgmBuf[musicPtr];
                chipRegister.SN76489SetRegister(vgmBuf[musicPtr], Audio.DriverSeqCounter, 0, vgmBuf[musicPtr].val);
                musicPtr++;
            }
        }

        private void WriteYM2612P0(outDatum cmd, byte X)
        {
            for (int i = 0; i < X + 1; i++)
            {
                outDatum od = vgmBuf[musicPtr];
                byte adr = vgmBuf[musicPtr++].val;
                byte val = vgmBuf[musicPtr++].val;
                chipRegister.YM2612SetRegister(od, Audio.DriverSeqCounter, 0, 0, adr, val);
            }
        }

        private void WriteYM2612P1(outDatum cmd, byte X)
        {
            for (int i = 0; i < X + 1; i++)
            {
                outDatum od = vgmBuf[musicPtr];
                byte adr = vgmBuf[musicPtr++].val;
                byte val = vgmBuf[musicPtr++].val;
                chipRegister.YM2612SetRegister(od, Audio.DriverSeqCounter, 0, 1, adr, val);
            }
        }

        private void WriteYM2612Key(outDatum cmd, byte X)
        {
            for (int i = 0; i < X + 1; i++)
            {
                chipRegister.YM2612SetRegister(vgmBuf[musicPtr], Audio.DriverSeqCounter, 0, 0, 0x28, vgmBuf[musicPtr].val);
                //Console.WriteLine("{0:x2}", vgmBuf[musicPtr].val);
                musicPtr++;
            }
        }

        public override GD3 getGD3Info(byte[] buf, uint vgmGd3)
        {
            throw new NotImplementedException();
        }

    }

}
