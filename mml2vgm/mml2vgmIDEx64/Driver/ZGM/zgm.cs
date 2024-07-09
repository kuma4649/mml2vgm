using Core;
using SoundManager;
using System;
using System.Collections.Generic;
using static MDSound.dacControl;

namespace mml2vgmIDE.Driver.ZGM
{
    public class zgm : baseDriver
    {

        public const int FCC_ZGM = 0x204D475A;	// "ZGM "
        public const int FCC_GD3 = 0x20336447;  // "Gd3 "
        public const int FCC_DEF = 0x666544;  // "Def"
        public const int FCC_TRK = 0x6b7254;  // "Trk"

        public const uint defaultSN76489ClockValue = 3579545;
        public const uint defaultYM2612ClockValue = 7670454;
        public const uint defaultRF5C164ClockValue = 12500000;
        public const uint defaultPWMClockValue = 23011361;
        public const uint defaultC140ClockValue = 21390;
        public const MDSound.c140.C140_TYPE defaultC140Type = MDSound.c140.C140_TYPE.ASIC219;
        public const uint defaultOKIM6258ClockValue = 4000000;
        public const uint defaultOKIM6295ClockValue = 4000000;
        public const uint defaultSEGAPCMClockValue = 4000000;
        public const uint defaultAY8910ClockValue = 1789750;


        public uint SN76489ClockValue = 3579545;
        public uint YM2612ClockValue = 7670454;
        public uint RF5C68ClockValue = 12500000;
        public uint RF5C164ClockValue = 12500000;
        public uint PWMClockValue = 23011361;
        public uint C140ClockValue = 21390;
        public MDSound.c140.C140_TYPE C140Type = MDSound.c140.C140_TYPE.ASIC219;
        public uint OKIM6258ClockValue = 4000000;
        public byte OKIM6258Type = 0;
        public uint OKIM6295ClockValue = 4000000;
        public uint SEGAPCMClockValue = 4000000;
        public int SEGAPCMInterface = 0;
        public uint YM2151ClockValue;
        public uint YM2608ClockValue;
        public uint YM2609ClockValue;
        public uint YM2203ClockValue;
        public uint YM2610ClockValue;
        public uint YM3812ClockValue;
        public uint YM3526ClockValue;
        public uint Y8950ClockValue;
        public uint YMF262ClockValue;
        public uint YMF271ClockValue;
        public uint YMF278BClockValue;
        public uint YMZ280BClockValue;
        public uint AY8910ClockValue;
        public uint YM2413ClockValue;
        public uint HuC6280ClockValue;
        public uint QSoundClockValue;
        public uint C352ClockValue;
        public byte C352ClockDivider;
        public uint GA20ClockValue;
        public uint K053260ClockValue;
        public uint K054539ClockValue;
        public byte K054539Flags;
        public uint K051649ClockValue;
        public uint DMGClockValue;
        public uint NESClockValue;
        public uint VRC6ClockValue;
        public uint MultiPCMClockValue;
        public uint GigatronClockValue;

        //public dacControl dacControl = new dacControl();
        public bool isPcmRAMWrite = false;
        public bool useChipYM2612Ch6 = false;
        //public Setting setting = null;

        public delegate void RefAction<T1, T2>(T1 arg1, ref T2 arg2);
        private Dictionary<int, RefAction<outDatum, uint>> vgmCmdTbl = new Dictionary<int, RefAction<outDatum, uint>>();

        public List<Chip> chips = null;

        private uint vgmAdr;
        //private int vgmWait;
        private long vgmLoopOffset = 0;
        private uint vgmEof;
        private bool vgmAnalyze;

        private long vgmDataOffset = 0;

        private const int PCM_BANK_COUNT = 0x40;
        public VGM_PCM_BANK[] PCMBank = new VGM_PCM_BANK[PCM_BANK_COUNT];
        private PCMBANK_TBL PCMTbl = new PCMBANK_TBL();
        private byte DacCtrlUsed;
        private byte[] DacCtrlUsg = new byte[0xFF];
        private DACCTRL_DATA[] DacCtrl = new DACCTRL_DATA[0xFF];

        private byte[][] ym2609AdpcmA = new byte[2][] { null, null };
        private byte[][] ym2610AdpcmA = new byte[2][] { null, null };
        private byte[][] ym2610AdpcmB = new byte[2][] { null, null };

        private List<byte> pcmDat = new List<byte>();
        private int chipCommandSize = 1;

        public override bool init(outDatum[] vgmBuf, ChipRegister chipRegister, EnmChip[] useChip, uint latency, uint waitTime, long jumpPointClock)
        {
            this.vgmBuf = vgmBuf;
            this.chipRegister = chipRegister;
            this.useChip = useChip;
            this.latency = latency;
            this.waitTime = waitTime;
            this.jumpPointClock = jumpPointClock;

            //dumpCounter = 0;

            ym2610AdpcmA = new byte[2][] { null, null };
            ym2610AdpcmB = new byte[2][] { null, null };

            if (!getInformationHeader()) return false;

            chipRegister.ZgmSetup(chips);

            vgmAdr = (uint)vgmDataOffset;
            //vgmWait = 0;
            vgmAnalyze = true;
            Counter = 0;
            vgmFrameCounter = -latency - waitTime;
            vgmCurLoop = 0;
            vgmSpeed = 1;
            vgmSpeedCounter = 0;

            for (int i = 0; i < PCM_BANK_COUNT; i++) PCMBank[i] = new VGM_PCM_BANK();
            //dacControl.refresh();
            DacCtrlUsed = 0x00;
            for (byte CurChip = 0x00; CurChip < 0xFF; CurChip++)
            {
                DacCtrl[CurChip] = new DACCTRL_DATA();
                DacCtrl[CurChip].Enable = false;
            }

            try
            {
                setCommands();
            }
            catch (Exception e)
            {
                log.Write(e.StackTrace);
                return false;
            }

            Stopped = false;

            //要るのかな?
            useChipYM2612Ch6 = false;
            foreach (EnmChip uc in useChip)
            {
                if (uc == EnmChip.YM2612Ch6)
                {
                    useChipYM2612Ch6 = true;
                    break;
                }
            }

            return true;
        }

        public override void oneFrameProc()
        {
            try
            {
                vgmSpeedCounter += vgmSpeed;
                while (vgmSpeedCounter >= 1.0)
                {
                    vgmSpeedCounter -= 1.0;
                    if (vgmFrameCounter > -1)
                    {
                        oneFrameVGMMain();
                    }
                    else
                    {
                        vgmFrameCounter++;
                    }
                }
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);

            }
        }

        private void oneFrameVGMMain()
        {
            Counter = Audio.DriverSeqCounter;

            if (!vgmAnalyze)
            {
                Stopped = true;
                return;
            }

            {
                if (vgmAdr >= vgmBuf.Length || vgmAdr >= vgmEof)
                {
                    if (LoopCounter != 0)
                    {
                        vgmAdr = (uint)(vgmLoopOffset);// + 0x1c);
                        chipRegister.LoopCountUp(Audio.DriverSeqCounter);
                        vgmCurLoop++;
                    }
                    else
                    {
                        vgmAnalyze = false;
                        vgmAdr = (uint)vgmBuf.Length;
                        return;
                    }
                }

                outDatum cmd = vgmBuf[vgmAdr].Copy();

                if (vgmCmdTbl.ContainsKey(cmd.val))
                {
                    //Console.WriteLine("{0:X05} : {1:X02} {2:X02} {3:X02}", vgmAdr, vgmBuf[vgmAdr].val, vgmBuf[vgmAdr + 1].val, vgmBuf[vgmAdr + 2].val);
                    vgmCmdTbl[cmd.val](cmd, ref vgmAdr);
                }
                else
                {
                    //わからんコマンド
                    Console.WriteLine("unknown command: Adr:{0:X} Dat:{1:X}", vgmAdr, vgmBuf[vgmAdr].val);
                    vgmAdr++;
                }

            }

            //for (long i = Counter; i < Audio.DriverSeqCounter; i++)
            //{
            //    oneFrameVGMStream(i);
            //}

            vgmFrameCounter++;

        }

        //private void oneFrameVGMStream(long Counter)
        //{
        //    for (int CurChip = 0x00; CurChip < DacCtrlUsed; CurChip++)
        //    {
        //        dacControl.update(Counter, DacCtrlUsg[CurChip], 1);
        //    }
        //}

        private void setCommands()
        {

            vgmCmdTbl.Add(0x01, vcWaitNSamples);
            vgmCmdTbl.Add(0x02, vcWait735Samples);
            vgmCmdTbl.Add(0x03, vcWait882Samples);
            vgmCmdTbl.Add(0x04, vcOverrideLength);

            vgmCmdTbl.Add(0x06, vcEndOfSoundData);
            vgmCmdTbl.Add(0x07, vcDataBlock);
            vgmCmdTbl.Add(0x08, vcPCMRamWrite);
            vgmCmdTbl.Add(0x09, vcDummyChip);

            vgmCmdTbl.Add(0x10, vcWaitN1Samples);
            vgmCmdTbl.Add(0x11, vcWaitN1Samples);
            vgmCmdTbl.Add(0x12, vcWaitN1Samples);
            vgmCmdTbl.Add(0x13, vcWaitN1Samples);
            vgmCmdTbl.Add(0x14, vcWaitN1Samples);
            vgmCmdTbl.Add(0x15, vcWaitN1Samples);
            vgmCmdTbl.Add(0x16, vcWaitN1Samples);
            vgmCmdTbl.Add(0x17, vcWaitN1Samples);

            vgmCmdTbl.Add(0x18, vcWaitN1Samples);
            vgmCmdTbl.Add(0x19, vcWaitN1Samples);
            vgmCmdTbl.Add(0x1a, vcWaitN1Samples);
            vgmCmdTbl.Add(0x1b, vcWaitN1Samples);
            vgmCmdTbl.Add(0x1c, vcWaitN1Samples);
            vgmCmdTbl.Add(0x1d, vcWaitN1Samples);
            vgmCmdTbl.Add(0x1e, vcWaitN1Samples);
            vgmCmdTbl.Add(0x1f, vcWaitN1Samples);

            vgmCmdTbl.Add(0x20, vcWaitNSamplesAndSendYM26120x2a);
            vgmCmdTbl.Add(0x21, vcWaitNSamplesAndSendYM26120x2a);
            vgmCmdTbl.Add(0x22, vcWaitNSamplesAndSendYM26120x2a);
            vgmCmdTbl.Add(0x23, vcWaitNSamplesAndSendYM26120x2a);
            vgmCmdTbl.Add(0x24, vcWaitNSamplesAndSendYM26120x2a);
            vgmCmdTbl.Add(0x25, vcWaitNSamplesAndSendYM26120x2a);
            vgmCmdTbl.Add(0x26, vcWaitNSamplesAndSendYM26120x2a);
            vgmCmdTbl.Add(0x27, vcWaitNSamplesAndSendYM26120x2a);

            vgmCmdTbl.Add(0x28, vcWaitNSamplesAndSendYM26120x2a);
            vgmCmdTbl.Add(0x29, vcWaitNSamplesAndSendYM26120x2a);
            vgmCmdTbl.Add(0x2a, vcWaitNSamplesAndSendYM26120x2a);
            vgmCmdTbl.Add(0x2b, vcWaitNSamplesAndSendYM26120x2a);
            vgmCmdTbl.Add(0x2c, vcWaitNSamplesAndSendYM26120x2a);
            vgmCmdTbl.Add(0x2d, vcWaitNSamplesAndSendYM26120x2a);
            vgmCmdTbl.Add(0x2e, vcWaitNSamplesAndSendYM26120x2a);
            vgmCmdTbl.Add(0x2f, vcWaitNSamplesAndSendYM26120x2a);

            vgmCmdTbl.Add(0x30, vcSetupStreamControl);
            vgmCmdTbl.Add(0x31, vcSetStreamData);
            vgmCmdTbl.Add(0x32, vcSetStreamFrequency);
            vgmCmdTbl.Add(0x33, vcStartStream);
            vgmCmdTbl.Add(0x34, vcStopStream);
            vgmCmdTbl.Add(0x35, vcStartStreamFastCall);

            vgmCmdTbl.Add(0x40, vcSeekToOffsetInPCMDataBank);

        }

        private void vcDummyChip(outDatum od, ref uint vgmAdr)
        {
            chipRegister.writeDummyChipZGM(od, Audio.DriverSeqCounter, vgmBuf[vgmAdr + 1].val, vgmBuf[vgmAdr + 2].val);
            vgmAdr += 3;
        }

        private void vcGGPSGPort06(outDatum od)
        {
            chipRegister.SN76489SetRegisterGGpanning(od, Audio.DriverSeqCounter, vgmBuf[vgmAdr].val == 0x4f ? 0 : 1, vgmBuf[vgmAdr + 1].val);
            vgmAdr += 2;
        }

        private void vcPSG(outDatum od)
        {
            chipRegister.SN76489SetRegister(od, Audio.DriverSeqCounter, vgmBuf[vgmAdr].val == 0x50 ? 0 : 1, vgmBuf[vgmAdr + 1].val);
            vgmAdr += 2;
        }

        private void vcAY8910(outDatum od)
        {
            chipRegister.AY8910SetRegister(od, Audio.DriverSeqCounter, (vgmBuf[vgmAdr + 1].val & 0x80) == 0 ? 0 : 1, vgmBuf[vgmAdr + 1].val & 0x7f, vgmBuf[vgmAdr + 2].val);
            vgmAdr += 3;
        }

        private void vcQSound(outDatum od)
        {
            //chipRegister.setQSoundRegister(0, vgmBuf[vgmAdr + 1].val, vgmBuf[vgmAdr + 2].val, vgmBuf[vgmAdr + 3].val);
            chipRegister.QSoundSetRegister(
                od,
                Audio.DriverSeqCounter,
                0,
                vgmBuf[vgmAdr + 3].val, //adr(register)
                vgmBuf[vgmAdr + 2].val + vgmBuf[vgmAdr + 1].val * 0x100//data
                );
            vgmAdr += 4;
        }

        private void vcYM2413(outDatum od)
        {
            chipRegister.YM2413SetRegister(od, Audio.DriverSeqCounter, (vgmBuf[vgmAdr].val & 0x80) == 0 ? 0 : 1, vgmBuf[vgmAdr + 1].val, vgmBuf[vgmAdr + 2].val);
            vgmAdr += 3;
        }

        private void vcYM3812(outDatum od)
        {
            chipRegister.YM3812SetRegister(od, Audio.DriverSeqCounter, (vgmBuf[vgmAdr].val & 0x80) == 0 ? 0 : 1, vgmBuf[vgmAdr + 1].val, vgmBuf[vgmAdr + 2].val);
            vgmAdr += 3;
        }

        private void vcHuC6280(outDatum od)
        {
            chipRegister.HuC6280SetRegister(od, Audio.DriverSeqCounter, (vgmBuf[vgmAdr + 1].val & 0x80) == 0 ? 0 : 1, vgmBuf[vgmAdr + 1].val & 0x7f, vgmBuf[vgmAdr + 2].val);
            vgmAdr += 3;
        }

        private void vcYM2203(outDatum od)
        {
            chipRegister.YM2203SetRegister(od, Audio.DriverSeqCounter, (vgmBuf[vgmAdr].val & 0x80) == 0 ? 0 : 1, vgmBuf[vgmAdr + 1].val, vgmBuf[vgmAdr + 2].val);
            vgmAdr += 3;
        }

        private void vcYM2608Port0(outDatum od)
        {
            chipRegister.YM2608SetRegister(od, Audio.DriverSeqCounter, (vgmBuf[vgmAdr].val & 0x80) == 0 ? 0 : 1, 0, vgmBuf[vgmAdr + 1].val, vgmBuf[vgmAdr + 2].val);
            vgmAdr += 3;
        }

        private void vcYM2608Port1(outDatum od)
        {
            chipRegister.YM2608SetRegister(od, Audio.DriverSeqCounter, (vgmBuf[vgmAdr].val & 0x80) == 0 ? 0 : 1, 1, vgmBuf[vgmAdr + 1].val, vgmBuf[vgmAdr + 2].val);
            vgmAdr += 3;
        }

        private void vcYM2610Port0(outDatum od)
        {
            chipRegister.YM2610SetRegister(od, Audio.DriverSeqCounter, (vgmBuf[vgmAdr].val & 0x80) == 0 ? 0 : 1, 0, vgmBuf[vgmAdr + 1].val, vgmBuf[vgmAdr + 2].val);
            vgmAdr += 3;
        }

        private void vcYM2610Port1(outDatum od)
        {
            int adr = vgmBuf[vgmAdr + 1].val;
            int dat = vgmBuf[vgmAdr + 2].val;
            chipRegister.YM2610SetRegister(od, Audio.DriverSeqCounter, (vgmBuf[vgmAdr].val & 0x80) == 0 ? 0 : 1, 1, adr, dat);
            vgmAdr += 3;
        }

        private void vcYM3526(outDatum od)
        {
            chipRegister.YM3526SetRegister(od, Audio.DriverSeqCounter, (vgmBuf[vgmAdr].val & 0x80) == 0 ? 0 : 1, vgmBuf[vgmAdr + 1].val, vgmBuf[vgmAdr + 2].val);
            vgmAdr += 3;
        }

        private void vcY8950(outDatum od)
        {
            chipRegister.Y8950SetRegister(od, Audio.DriverSeqCounter, (vgmBuf[vgmAdr].val & 0x80) == 0 ? 0 : 1, vgmBuf[vgmAdr + 1].val, vgmBuf[vgmAdr + 2].val);
            vgmAdr += 3;
        }

        private void vcYM2151(outDatum od)
        {
            chipRegister.YM2151SetRegister(od, Audio.DriverSeqCounter, (vgmBuf[vgmAdr].val & 0x80) == 0 ? 0 : 1, vgmBuf[vgmAdr + 1].val, vgmBuf[vgmAdr + 2].val);
            //, (vgmBuf[vgmAdr].val & 0x80) == 0 ? YM2151Hosei[0].val : YM2151Hosei[1].val, vgmFrameCounter);
            vgmAdr += 3;
        }

        private void vcSEGAPCM(outDatum od)
        {
            //Console.WriteLine("{0:X4} {1:X4}", vgmBuf[vgmAdr + 0x01].val, vgmBuf[vgmAdr + 0x02].val);
            chipRegister.SEGAPCMSetRegister(od, Audio.DriverSeqCounter, 0, (int)((vgmBuf[vgmAdr + 0x01].val & 0xFF) | ((vgmBuf[vgmAdr + 0x02].val & 0xFF) << 8)), vgmBuf[vgmAdr + 0x03].val);
            vgmAdr += 4;
        }

        private void vcWaitNSamples(outDatum od, ref uint vgmAdr)
        {
            if (jumpPointClock < 0)
            {
                Audio.DriverSeqCounter += (int)Common.getLE16(vgmBuf, vgmAdr + 1);
            }
            else
            {
                jumpPointClock -= (int)Common.getLE16(vgmBuf, vgmAdr + 1);
            }
            vgmAdr += 3;
        }

        private void vcWait735Samples(outDatum od, ref uint vgmAdr)
        {
            if (jumpPointClock < 0)
            {
                Audio.DriverSeqCounter += 735;
            }
            else
            {
                jumpPointClock -= 735;
            }
            vgmAdr++;
        }

        private void vcWait882Samples(outDatum od, ref uint vgmAdr)
        {
            if (jumpPointClock < 0)
            {
                Audio.DriverSeqCounter += 882;
            }
            else
            {
                jumpPointClock -= 882;
            }
            vgmAdr++;
        }

        private void vcOverrideLength(outDatum od, ref uint vgmAdr)
        {
            vgmAdr += 4;
        }

        private void vcEndOfSoundData(outDatum od, ref uint vgmAdr)
        {
            vgmAdr = (uint)vgmBuf.Length;
        }

        private void vcDataBlock(outDatum od, ref uint vgmAdr)
        {

            int chipCommandNumber = (chipCommandSize == 1) ? (int)vgmBuf[vgmAdr + 1].val : (int)Common.getLE16(vgmBuf, vgmAdr + 2);
            byte bType = (chipCommandSize == 1) ? vgmBuf[vgmAdr + 2].val : vgmBuf[vgmAdr + 4].val;
            vgmAdr += (uint)((chipCommandSize == 1) ? 0 : 2);
            uint bAdr = vgmAdr + 7;
            uint bLen = Common.getLE32(vgmBuf, vgmAdr + 3);
            //byte chipID = 0;
            //if ((bLen & 0x80000000) != 0)
            //{
            //    bLen &= 0x7fffffff;
            //    chipID = 1;
            //}
            if (!chipRegister.dicChipCmdNo.ContainsKey(chipCommandNumber))
            {
                //未定義のccnの場合(あってはならないが)
                vgmAdr += (uint)bLen + 7;
                return;
            }
            Driver.ZGM.ZgmChip.ZgmChip chip = chipRegister.dicChipCmdNo[chipCommandNumber];

            switch (chip)
            {
                case Driver.ZGM.ZgmChip.HuC6280 _:
                case Driver.ZGM.ZgmChip.YM2612 _:
                    pcmDat.Clear();
                    for (uint i = bAdr; i < bAdr + bLen; i++) pcmDat.Add(vgmBuf[i].val);
                    chipRegister.DACControlAddPCMData(bType, bLen, 0, pcmDat.ToArray());
                    //AddPCMData(bType, bLen, bAdr);
                    vgmAdr += (uint)bLen + 7;
                    break;
                case Driver.ZGM.ZgmChip.YM2609 _:
                    uint romSize = Common.getLE32(vgmBuf, vgmAdr + 7);
                    uint startAddress = Common.getLE32(vgmBuf, vgmAdr + 0x0B);
                    int adpcmAdr = 0x100;
                    if (bType == 1)
                    {
                        adpcmAdr = 0x300;
                    }
                    else if (bType == 2)
                    {
                        adpcmAdr = 0x311;
                    }
                    else if (bType == 3)
                    {
                        if (ym2609AdpcmA[chip.Index] == null || ym2609AdpcmA[chip.Index].Length != romSize) ym2609AdpcmA[chip.Index] = new byte[romSize];
                        if (ym2609AdpcmA[chip.Index].Length > 0)
                        {
                            for (int cnt = 0; cnt < bLen - 8; cnt++)
                            {
                                ym2609AdpcmA[chip.Index][startAddress + cnt] = vgmBuf[vgmAdr + 15 + cnt].val;
                            }
                            chipRegister.YM2609WriteSetAdpcmA(od, Audio.DriverSeqCounter, chip.Index, ym2609AdpcmA[chip.Index]);
                        }
                        vgmAdr += (uint)bLen + 7;
                        break;
                    }
                    else if (bType == 4)
                    {
                        int blockSize = 1024 * 2;
                        for (int j = 0; j < bLen - 8; j += blockSize + 2)
                        {
                            int n = vgmBuf[vgmAdr + 15 + j].val;
                            byte[] wav = new byte[blockSize];
                            for (int i = 0; i < blockSize; i++)
                            {
                                wav[i] = vgmBuf[vgmAdr + 17 + i + j].val;
                            }
                            chipRegister.YM2609WriteSetOperatorWaveDic(od, Audio.DriverSeqCounter, chip.Index, n, wav);
                        }
                        vgmAdr += (uint)bLen + 7;
                        break;
                    }

                    byte[] pcm012Buf = new byte[bLen];
                    for (int cnt = 0; cnt < bLen - 8; cnt++)
                    {
                        pcm012Buf[cnt] = vgmBuf[vgmAdr + 15 + cnt].val;
                    }
                    chipRegister.YM2609WriteSetAdpcm012(od, Audio.DriverSeqCounter, chip.Index, bType, pcm012Buf);

                    //List<PackData> data = new List<PackData>
                    //{
                    //    new PackData(null,chipRegister.YM2609[chip.Index],0,adpcmAdr+ 0x00, 0x20,null),
                    //    new PackData(null,chipRegister.YM2609[chip.Index],0,adpcmAdr+ 0x00, 0x21,null),
                    //    new PackData(null,chipRegister.YM2609[chip.Index],0,adpcmAdr+ 0x00, 0x00,null),

                    //    new PackData(null,chipRegister.YM2609[chip.Index],0,adpcmAdr+ 0x10, 0x00,null),
                    //    new PackData(null,chipRegister.YM2609[chip.Index],0,adpcmAdr+ 0x10, 0x80,null),

                    //    new PackData(null,chipRegister.YM2609[chip.Index],0,adpcmAdr+ 0x00, 0x61,null),
                    //    new PackData(null,chipRegister.YM2609[chip.Index],0,adpcmAdr+ 0x00, 0x68,null),
                    //    new PackData(null,chipRegister.YM2609[chip.Index],0,adpcmAdr+ 0x01, 0x00,null),

                    //    new PackData(null,chipRegister.YM2609[chip.Index],0,adpcmAdr+ 0x02, (byte)(startAddress >> 2),null),
                    //    new PackData(null,chipRegister.YM2609[chip.Index],0,adpcmAdr+ 0x03, (byte)(startAddress >> 10),null),
                    //    new PackData(null,chipRegister.YM2609[chip.Index],0,adpcmAdr+ 0x04, 0xff,null),
                    //    new PackData(null,chipRegister.YM2609[chip.Index],0,adpcmAdr+ 0x05, 0xff,null),
                    //    new PackData(null,chipRegister.YM2609[chip.Index],0,adpcmAdr+ 0x0c, 0xff,null),
                    //    new PackData(null,chipRegister.YM2609[chip.Index],0,adpcmAdr+ 0x0d, 0xff,null)
                    //};
                    //// データ転送
                    //for (int cnt = 0; cnt < bLen - 8; cnt++)
                    //{
                    //    data.Add(new PackData(null, chipRegister.YM2609[chip.Index], 0, adpcmAdr + 0x08, vgmBuf[vgmAdr + 15 + cnt].val, null));
                    //}
                    //data.Add(new PackData(null, chipRegister.YM2609[chip.Index], 0, adpcmAdr + 0x00, 0x00, null));
                    //data.Add(new PackData(null, chipRegister.YM2609[chip.Index], 0, adpcmAdr + 0x10, 0x80, null));

                    //SoundManager.Chip dummyChip = new SoundManager.Chip(1);
                    //dummyChip.Move(chipRegister.YM2609[chip.Index]);

                    //chipRegister.YM2609SetRegister(od, Audio.DriverSeqCounter, dummyChip, data.ToArray());

                    //dumpData(dummyChip, "YM2609_ADPCM", vgmAdr + 15, bLen - 8);
                    vgmAdr += (uint)bLen + 7;
                    break;
                case Driver.ZGM.ZgmChip.SEGAPCM _:// 0x80:
                    uint segapcm_romSize = Common.getLE32(vgmBuf, vgmAdr + 7);
                    uint segapcm_startAddress = Common.getLE32(vgmBuf, vgmAdr + 0x0B);
                    pcmDat.Clear();
                    for (uint i = 0; i < bLen - 8; i++) pcmDat.Add(vgmBuf[vgmAdr + 15 + i].val);
                    chipRegister.SEGAPCMWritePCMData(od, Audio.DriverSeqCounter, (byte)chip.Index, segapcm_romSize, segapcm_startAddress, (uint)pcmDat.Count, pcmDat.ToArray(), 0);
                    vgmAdr += (uint)bLen + 7;
                    break;
                case Driver.ZGM.ZgmChip.YM2608 _:
                    //uint opna_romSize = Common.getLE32(vgmBuf, vgmAdr + 7);
                    uint opna_startAddress = Common.getLE32(vgmBuf, vgmAdr + 0x0B);
                    List<PackData> opna_data = new List<PackData>
                            {
                                new PackData(null,chipRegister.YM2608[chip.Index],0,0x100+ 0x00, 0x20,null),
                                new PackData(null,chipRegister.YM2608[chip.Index],0,0x100+ 0x00, 0x21,null),
                                new PackData(null,chipRegister.YM2608[chip.Index],0,0x100+ 0x00, 0x00,null),

                                new PackData(null,chipRegister.YM2608[chip.Index],0,0x100+ 0x10, 0x00,null),
                                new PackData(null,chipRegister.YM2608[chip.Index],0,0x100+ 0x10, 0x80,null),

                                new PackData(null,chipRegister.YM2608[chip.Index],0,0x100+ 0x00, 0x61,null),
                                new PackData(null,chipRegister.YM2608[chip.Index],0,0x100+ 0x00, 0x68,null),
                                new PackData(null,chipRegister.YM2608[chip.Index],0,0x100+ 0x01, 0x00,null),

                                new PackData(null,chipRegister.YM2608[chip.Index],0,0x100+ 0x02, (byte)(opna_startAddress >> 2),null),
                                new PackData(null,chipRegister.YM2608[chip.Index],0,0x100+ 0x03, (byte)(opna_startAddress >> 10),null),
                                new PackData(null,chipRegister.YM2608[chip.Index],0,0x100+ 0x04, 0xff,null),
                                new PackData(null,chipRegister.YM2608[chip.Index],0,0x100+ 0x05, 0xff,null),
                                new PackData(null,chipRegister.YM2608[chip.Index],0,0x100+ 0x0c, 0xff,null),
                                new PackData(null,chipRegister.YM2608[chip.Index],0,0x100+ 0x0d, 0xff,null)
                            };

                    // データ転送
                    for (int cnt = 0; cnt < bLen - 8; cnt++)
                    {
                        opna_data.Add(new PackData(null, chipRegister.YM2608[chip.Index], 0, 0x100 + 0x08, vgmBuf[vgmAdr + 15 + cnt].val, null));
                    }
                    opna_data.Add(new PackData(null, chipRegister.YM2608[chip.Index], 0, 0x100 + 0x00, 0x00, null));
                    opna_data.Add(new PackData(null, chipRegister.YM2608[chip.Index], 0, 0x100 + 0x10, 0x80, null));

                    //chipRegister.setYM2608Register(0x1, 0x10, 0x13, model);
                    //chipRegister.setYM2608Register(0x1, 0x10, 0x80, model);
                    //chipRegister.setYM2608Register(0x1, 0x00, 0x60, model);
                    //chipRegister.setYM2608Register(0x1, 0x01, 0x00, model);

                    //chipRegister.setYM2608Register(0x1, 0x02, (int)((startAddress >> 2) & 0xff), model);
                    //chipRegister.setYM2608Register(0x1, 0x03, (int)((startAddress >> 10) & 0xff), model);
                    //chipRegister.setYM2608Register(0x1, 0x04, (int)(((startAddress + bLen - 8) >> 2) & 0xff), model);
                    //chipRegister.setYM2608Register(0x1, 0x05, (int)(((startAddress + bLen - 8) >> 10) & 0xff), model);
                    //chipRegister.setYM2608Register(0x1, 0x0c, 0xff, model);
                    //chipRegister.setYM2608Register(0x1, 0x0d, 0xff, model);

                    //for (int cnt = 0; cnt < bLen - 8; cnt++)
                    //{
                    //    chipRegister.setYM2608Register(0x1, 0x08, vgmBuf[vgmAdr + 15 + cnt].val, model);
                    //    chipRegister.setYM2608Register(0x1, 0x10, 0x1b, model);
                    //    chipRegister.setYM2608Register(0x1, 0x10, 0x13, model);
                    //}

                    //chipRegister.setYM2608Register(0x1, 0x00, 0x00, model);
                    //chipRegister.setYM2608Register(0x1, 0x10, 0x80, model);

                    SoundManager.Chip opna_dummyChip = new SoundManager.Chip(1);

                    opna_dummyChip.Model = chipRegister.YM2608[chip.Index].Model;
                    opna_dummyChip.Delay = chipRegister.YM2608[chip.Index].Delay;
                    opna_dummyChip.Device = chipRegister.YM2608[chip.Index].Device;
                    opna_dummyChip.Number = chipRegister.YM2608[chip.Index].Number;
                    opna_dummyChip.Use = chipRegister.YM2608[chip.Index].Use;

                    if (chipRegister.YM2608[chip.Index].Model == EnmVRModel.RealModel)
                    {
                        if (setting.YM2608Type.OnlyPCMEmulation)
                        {
                            opna_dummyChip.Model = EnmVRModel.VirtualModel;
                        }
                        else
                        {
                            Audio.DriverSeqCounter += (long)(bLen * 1.5);
                        }
                    }

                    chipRegister.YM2608SetRegister(od, Audio.DriverSeqCounter, opna_dummyChip, opna_data.ToArray());

                    //dumpData(dummyChip, "YM2608_ADPCM", vgmAdr + 15, bLen - 8);
                    vgmAdr += (uint)bLen + 7;
                    break;

                case Driver.ZGM.ZgmChip.YM2610 _:
                    uint opnb_romSize = Common.getLE32(vgmBuf, vgmAdr + 7);
                    uint opnb_startAddress = Common.getLE32(vgmBuf, vgmAdr + 0x0B);
                    if (bType == 0x82)
                    {
                        if (ym2610AdpcmA[chip.Index] == null || ym2610AdpcmA[chip.Index].Length != opnb_romSize) ym2610AdpcmA[chip.Index] = new byte[opnb_romSize];
                        if (ym2610AdpcmA[chip.Index].Length > 0)
                        {
                            for (int cnt = 0; cnt < bLen - 8; cnt++)
                            {
                                ym2610AdpcmA[chip.Index][opnb_startAddress + cnt] = vgmBuf[vgmAdr + 15 + cnt].val;
                            }
                            //if (model == EnmModel.VirtualModel) 
                            chipRegister.YM2610WriteSetAdpcmA(od, Audio.DriverSeqCounter, chip.Index, ym2610AdpcmA[chip.Index]);
                            //else chipRegister.WriteYM2610_SetAdpcmA(chipID, model, (int)startAddress, (int)(bLen - 8), vgmBuf, (int)(vgmAdr + 15));
                            //dumpData(chipRegister.YM2610[chipID], "YM2610_ADPCMA", vgmAdr + 15, bLen - 8);
                        }
                    }
                    else if (bType == 0x83)
                    {
                        if (ym2610AdpcmB[chip.Index] == null || ym2610AdpcmB[chip.Index].Length != opnb_romSize) ym2610AdpcmB[chip.Index] = new byte[opnb_romSize];
                        if (ym2610AdpcmB[chip.Index].Length > 0)
                        {
                            for (int cnt = 0; cnt < bLen - 8; cnt++)
                            {
                                ym2610AdpcmB[chip.Index][opnb_startAddress + cnt] = vgmBuf[vgmAdr + 15 + cnt].val;
                            }
                            //if (model == EnmModel.VirtualModel)
                            chipRegister.YM2610WriteSetAdpcmB(od, Audio.DriverSeqCounter, chip.Index, ym2610AdpcmB[chip.Index]);
                            //else chipRegister.WriteYM2610_SetAdpcmB(chipID, model, (int)startAddress, (int)(bLen - 8), vgmBuf, (int)(vgmAdr + 15));
                            //dumpData(chipRegister.YM2610[chipID], "YM2610_ADPCMB", vgmAdr + 15, bLen - 8);
                        }
                    }
                    vgmAdr += (uint)bLen + 7;
                    break;
                case Driver.ZGM.ZgmChip.YMF278B _:
                    // YMF278B
                    //chipRegister.writeYMF278BPCMData(chipID, romSize, startAddress, bLen - 8, vgmBuf, vgmAdr + 15);
                    pcmDat.Clear();
                    uint ymf278B_romSize = Common.getLE32(vgmBuf, vgmAdr + 7);
                    uint ymf278B_startAddress = Common.getLE32(vgmBuf, vgmAdr + 0x0B);
                    for (uint i = vgmAdr + 15; i < vgmAdr + 15 + bLen - 8; i++) pcmDat.Add(vgmBuf[i].val);
                    chipRegister.writeYMF278BPCMData((byte)chip.Index, ymf278B_romSize, ymf278B_startAddress, bLen - 8, pcmDat.ToArray(), 0);
                    //dumpData(model, "YMF278B_PCMData", vgmAdr + 15, bLen - 8);
                    break;

                //        case 0x85:
                //            // YMF271
                //            //chipRegister.writeYMF271PCMData(chipID, romSize, startAddress, bLen - 8, vgmBuf, vgmAdr + 15);
                //            pcmDat.Clear();
                //            for (uint i = vgmAdr + 15; i < vgmAdr + 15 + bLen - 8; i++) pcmDat.Add(vgmBuf[i].val);
                //            chipRegister.writeYMF271PCMData(chipID, romSize, startAddress, bLen - 8, pcmDat.ToArray(), 0);
                //            //dumpData(model, "YMF271_PCMData", vgmAdr + 15, bLen - 8);
                //            break;

                //        case 0x86:
                //            // YMZ280B
                //            //chipRegister.writeYMZ280BPCMData(chipID, romSize, startAddress, bLen - 8, vgmBuf, vgmAdr + 15);
                //            pcmDat.Clear();
                //            for (uint i = vgmAdr + 15; i < vgmAdr + 15 + bLen - 8; i++) pcmDat.Add(vgmBuf[i].val);
                //            chipRegister.writeYMZ280BPCMData(chipID, romSize, startAddress, bLen - 8, pcmDat.ToArray(), 0);
                //            //dumpData(model, "YMZ280B_PCMData", vgmAdr + 15, bLen - 8);
                //            break;

                //        case 0x88:
                //            // Y8950
                //            //chipRegister.writeY8950PCMData(chipID, romSize, startAddress, bLen - 8, vgmBuf, vgmAdr + 15);
                //            pcmDat.Clear();
                //            for (uint i = vgmAdr + 15; i < vgmAdr + 15 + bLen - 8; i++) pcmDat.Add(vgmBuf[i].val);
                //            chipRegister.writeY8950PCMData(chipID, romSize, startAddress, bLen - 8, pcmDat.ToArray(), 0);
                //            //dumpData(model, "Y8950_PCMData", vgmAdr + 15, bLen - 8);
                //            break;

                //        case 0x89:
                //            // MultiPCM
                //            //chipRegister.writeMultiPCMPCMData(chipID, romSize, startAddress, bLen - 8, vgmBuf, vgmAdr + 15);
                //            pcmDat.Clear();
                //            for (uint i = vgmAdr + 15; i < vgmAdr + 15 + bLen - 8; i++) pcmDat.Add(vgmBuf[i].val);
                //            chipRegister.writeMultiPCMPCMData(chipID, romSize, startAddress, bLen - 8, pcmDat.ToArray(), 0);
                //            //dumpData(model, "MultiPCM_PCMData", vgmAdr + 15, bLen - 8);
                //            break;

                //        case 0x8b:
                //            // OKIM6295
                //            //chipRegister.writeOKIM6295PCMData(chipID, romSize, startAddress, bLen - 8, vgmBuf, vgmAdr + 15);
                //            pcmDat.Clear();
                //            for (uint i = vgmAdr + 15; i < vgmAdr + 15 + bLen - 8; i++) pcmDat.Add(vgmBuf[i].val);
                //            chipRegister.writeOKIM6295PCMData(chipID, romSize, startAddress, bLen - 8, pcmDat.ToArray(), 0);
                //            //dumpData(model, "OKIM6295_PCMData", vgmAdr + 15, bLen - 8);
                //            break;

                //        case 0x8c:
                //            // K054539
                //            //chipRegister.writeK054539PCMData(chipID, romSize, startAddress, bLen - 8, vgmBuf, vgmAdr + 15);
                //            pcmDat.Clear();
                //            for (uint i = vgmAdr + 15; i < vgmAdr + 15 + bLen - 8; i++) pcmDat.Add(vgmBuf[i].val);
                //            chipRegister.writeK054539PCMData(chipID, romSize, startAddress, bLen - 8, pcmDat.ToArray(), 0);
                //            //dumpData(model, "K054539_PCMData", vgmAdr + 15, bLen - 8);
                //            break;

                case Driver.ZGM.ZgmChip.C140 _:
                    uint c140_romSize = Common.getLE32(vgmBuf, vgmAdr + 7);
                    uint c140_startAddress = Common.getLE32(vgmBuf, vgmAdr + 0x0B);
                    pcmDat.Clear();
                    for (uint i = 0; i < bLen - 8; i++) pcmDat.Add(vgmBuf[vgmAdr + 15 + i].val);
                    chipRegister.C140WritePCMData(od, Audio.DriverSeqCounter, (byte)chip.Index, c140_romSize, c140_startAddress, (uint)pcmDat.Count, pcmDat.ToArray(), 0);
                    vgmAdr += (uint)bLen + 7;
                    break;

                case Driver.ZGM.ZgmChip.C352 _:
                    uint c352_romSize = Common.getLE32(vgmBuf, vgmAdr + 7);
                    uint c352_startAddress = Common.getLE32(vgmBuf, vgmAdr + 0x0B);
                    pcmDat.Clear();
                    for (uint i = 0; i < bLen - 8; i++) pcmDat.Add(vgmBuf[vgmAdr + 15 + i].val);
                    chipRegister.C352WritePCMData(od, Audio.DriverSeqCounter, (byte)chip.Index, c352_romSize, c352_startAddress, (uint)pcmDat.Count, pcmDat.ToArray(), 0);
                    vgmAdr += (uint)bLen + 7;
                    break;

                case Driver.ZGM.ZgmChip.K053260 _:
                    uint K053260_romSize = Common.getLE32(vgmBuf, vgmAdr + 7);
                    uint K053260_startAddress = Common.getLE32(vgmBuf, vgmAdr + 0x0B);
                    pcmDat.Clear();
                    for (uint i = 0; i < bLen - 8; i++) pcmDat.Add(vgmBuf[vgmAdr + 15 + i].val);
                    chipRegister.K053260WritePCMData(od, Audio.DriverSeqCounter, (byte)chip.Index, K053260_romSize, K053260_startAddress, (uint)pcmDat.Count, pcmDat.ToArray(), 0);
                    vgmAdr += (uint)bLen + 7;
                    break;

                case Driver.ZGM.ZgmChip.K054539 _:
                    uint K054539_romSize = Common.getLE32(vgmBuf, vgmAdr + 7);
                    uint K054539_startAddress = Common.getLE32(vgmBuf, vgmAdr + 0x0B);
                    pcmDat.Clear();
                    for (uint i = 0; i < bLen - 8; i++) pcmDat.Add(vgmBuf[vgmAdr + 15 + i].val);
                    chipRegister.K054539WritePCMData(od, Audio.DriverSeqCounter, (byte)chip.Index, K054539_romSize, K054539_startAddress, (uint)pcmDat.Count, pcmDat.ToArray(), 0);
                    vgmAdr += (uint)bLen + 7;
                    break;

                case Driver.ZGM.ZgmChip.QSound _:
                    uint QSound_romSize = Common.getLE32(vgmBuf, vgmAdr + 7);
                    uint QSound_startAddress = Common.getLE32(vgmBuf, vgmAdr + 0x0B);
                    pcmDat.Clear();
                    for (uint i = 0; i < bLen - 8; i++) pcmDat.Add(vgmBuf[vgmAdr + 15 + i].val);
                    chipRegister.QSoundWritePCMData(od, Audio.DriverSeqCounter, (byte)chip.Index, QSound_romSize, QSound_startAddress, (uint)pcmDat.Count, pcmDat.ToArray(), 0);
                    vgmAdr += (uint)bLen + 7;
                    //            // QSound
                    //            //chipRegister.writeQSoundPCMData(chipID, romSize, startAddress, bLen - 8, vgmBuf, vgmAdr + 15);
                    //            pcmDat.Clear();
                    //            for (uint i = vgmAdr + 15; i < vgmAdr + 15 + bLen - 8; i++) pcmDat.Add(vgmBuf[i].val);
                    //            chipRegister.QSoundWritePCMData(od, Audio.DriverSeqCounter, chipID, romSize, startAddress, bLen - 8, pcmDat.ToArray(), 0);
                    //            //dumpData(model, "QSound_PCMData", vgmAdr + 15, bLen - 8);
                    break;

                case Driver.ZGM.ZgmChip.RF5C164 _:
                    //uint RF5C164_romSize = Common.getLE32(vgmBuf, vgmAdr + 7);
                    uint RF5C164_startAddress = Common.getLE32(vgmBuf, vgmAdr + 0x0B);
                    pcmDat.Clear();
                    for (uint i = 0; i < bLen - 8; i++) pcmDat.Add(vgmBuf[vgmAdr + 15 + i].val);
                    chipRegister.RF5C164WritePCMData(od, Audio.DriverSeqCounter, (byte)chip.Index, RF5C164_startAddress, (uint)pcmDat.Count, pcmDat.ToArray(), 0);
                    vgmAdr += (uint)bLen + 7;
                    break;

                case Driver.ZGM.ZgmChip.NES _:
                    //uint RF5C164_romSize = Common.getLE32(vgmBuf, vgmAdr + 7);
                    uint NES_startAddress = Common.getLE32(vgmBuf, vgmAdr + 7);// 0x0B);
                    pcmDat.Clear();
                    for (uint i = 0; i < bLen - 8; i++) pcmDat.Add(vgmBuf[vgmAdr + 15 + i].val);
                    chipRegister.NESWritePCMData(od, Audio.DriverSeqCounter, (byte)chip.Index, NES_startAddress, (uint)pcmDat.Count, pcmDat.ToArray(), 0);
                    vgmAdr += (uint)bLen + 7;
                    break;

                //        case 0x92:
                //            // C352
                //            //chipRegister.writeC352PCMData(chipID, romSize, startAddress, bLen - 8, vgmBuf, vgmAdr + 15);
                //            pcmDat.Clear();
                //            for (uint i = vgmAdr + 15; i < vgmAdr + 15 + bLen - 8; i++) pcmDat.Add(vgmBuf[i].val);
                //            chipRegister.writeC352PCMData(chipID, romSize, startAddress, bLen - 8, pcmDat.ToArray(), 0);
                //            //dumpData(model, "C352_PCMData", vgmAdr + 15, bLen - 8);
                //            break;

                //        case 0x93:
                //            // GA20
                //            //chipRegister.writeGA20PCMData(chipID, romSize, startAddress, bLen - 8, vgmBuf, vgmAdr + 15);
                //            pcmDat.Clear();
                //            for (uint i = vgmAdr + 15; i < vgmAdr + 15 + bLen - 8; i++) pcmDat.Add(vgmBuf[i].val);
                //            chipRegister.writeGA20PCMData(chipID, romSize, startAddress, bLen - 8, pcmDat.ToArray(), 0);
                //            //dumpData(model, "GA20_PCMData", vgmAdr + 15, bLen - 8);
                //            break;
                //    }
                //    vgmAdr += (uint)bLen + 7;
                //    break;
                //case 0xc0:
                //    uint stAdr = Common.getLE16(vgmBuf, vgmAdr + 7);
                //    uint dataSize = bLen - 2;
                //    uint ROMData = vgmAdr + 9;
                //    if ((bType & 0x20) != 0)
                //    {
                //        stAdr = Common.getLE32(vgmBuf, vgmAdr + 7);
                //        dataSize = bLen - 4;
                //        ROMData = vgmAdr + 11;
                //    }

                //    try
                //    {
                //        switch (bType)
                //        {
                //            case 0xc0:
                //                //chipRegister.writeRF5C68PCMData(chipID, stAdr, dataSize, vgmBuf, vgmAdr + 9);
                //                pcmDat.Clear();
                //                for (uint i = vgmAdr + 9; i < vgmAdr + 9 + dataSize; i++) pcmDat.Add(vgmBuf[i].val);
                //                chipRegister.writeRF5C68PCMData(chipID, stAdr, dataSize, pcmDat.ToArray(), 0);
                //                //dumpData(model, "RF5C68_PCMData(8BitMonoSigned)", vgmAdr + 9, dataSize);
                //                break;
                //        }
                //    }
                //    catch (Exception e)
                //    {
                //        log.ForcedWrite(e);
                //    }

                //    vgmAdr += bLen + 7;
                //    break;
                default:
                    vgmAdr += bLen + 7;
                    break;
            }

        }

        private void vcPCMRamWrite(outDatum od, ref uint vgmAdr)
        {

            isPcmRAMWrite = true;

            byte bType = (byte)(vgmBuf[vgmAdr + 2].val & 0x7f);
            //CurrentChip = (vgmBuf[vgmAdr + 2].val & 0x80)>>7;
            uint bReadOffset = Common.getLE24(vgmBuf, vgmAdr + 3);
            uint bWriteOffset = Common.getLE24(vgmBuf, vgmAdr + 6);
            uint bSize = Common.getLE24(vgmBuf, vgmAdr + 9);
            if (bSize == 0) bSize = 0x1000000;
            uint? pcmAdr = GetPCMAddressFromPCMBank(bType, bReadOffset);
            if (pcmAdr != null)
            {
                if (bType == 0x01)
                {
                    chipRegister.writeRF5C68PCMData(0, bWriteOffset, bSize, PCMBank[bType].Data, (uint)pcmAdr);
                }
                if (bType == 0x02)
                {
                    chipRegister.RF5C164WritePCMData(od, Audio.DriverSeqCounter, 0, bWriteOffset, bSize, PCMBank[bType].Data, (uint)pcmAdr);
                }
            }

            vgmAdr += 12;

            isPcmRAMWrite = false;

        }

        private void vcWaitN1Samples(outDatum od, ref uint vgmAdr)
        {
            //vgmWait += (int)(vgmBuf[vgmAdr].val - 0x6f);
            if (jumpPointClock < 0)
            {
                Audio.DriverSeqCounter += (int)(vgmBuf[vgmAdr].val - 0x0f);
            }
            else
            {
                jumpPointClock -= (int)(vgmBuf[vgmAdr].val - 0x0f);
            }
            vgmAdr++;
        }

        private void vcWaitNSamplesAndSendYM26120x2a(outDatum od, ref uint vgmAdr)
        {
            byte dat = GetDACFromPCMBank();
            chipRegister.YM2612SetRegister(od, Audio.DriverSeqCounter, 0, 0, 0x2a, dat);

            if (jumpPointClock < 0)
            {
                Audio.DriverSeqCounter += (int)(vgmBuf[vgmAdr].val - 0x20);
            }
            else
            {
                jumpPointClock -= (int)(vgmBuf[vgmAdr].val - 0x20);
            }

            vgmAdr++;
        }

        private void vcSetupStreamControl(outDatum od, ref uint vgmAdr)
        {
            int pos = 1;
            byte si = vgmBuf[vgmAdr + pos].val;//streamID
            pos++;
            if (si == 0xff)
            {
                vgmAdr += 5;
                if (chipCommandSize == 2) vgmAdr++;
                return;
            }
            //if (!DacCtrl[si].Enable)
            //{
            //    //dacControl.device_start_daccontrol(si);
            //    //dacControl.device_reset_daccontrol(si);
            //    DacCtrl[si].Enable = true;
            //    DacCtrlUsg[DacCtrlUsed] = si;
            //    DacCtrlUsed++;
            //}

            int chipId = vgmBuf[vgmAdr + pos].val;
            pos++;
            if (chipCommandSize == 2)
            {
                chipId += vgmBuf[vgmAdr + pos].val * 0x100;
                pos++;
            }
            byte port = vgmBuf[vgmAdr + pos].val;
            pos++;
            byte cmd = vgmBuf[vgmAdr + pos].val;
            pos++;

            //dacControl.setup_chipZGM(si, chipId, (uint)(port * 0x100 + cmd));
            chipRegister.DACControlSetupStreamControl(
                od
                , Audio.DriverSeqCounter
                , si
                , (byte)(chipId & 0x7f)
                , 0
                , 0
                , (byte)((chipId & 0x80) >> 7)
                , port
                , cmd);

            vgmAdr += (uint)pos;
        }

        private void vcSetStreamData(outDatum od, ref uint vgmAdr)
        {
            //if (model != enmModel.VirtualModel)
            //{
            //    vgmAdr += 5;
            //    return;
            //}

            byte si = vgmBuf[vgmAdr + 1].val;
            if (si == 0xff)
            {
                vgmAdr += 5;
                return;
            }
            //DacCtrl[si].Bank = vgmBuf[vgmAdr + 2].val;
            //if (DacCtrl[si].Bank >= PCM_BANK_COUNT)
            //    DacCtrl[si].Bank = 0x00;

            //VGM_PCM_BANK TempPCM = PCMBank[DacCtrl[si].Bank];
            ////Last95Max = TempPCM->BankCount;
            //dacControl.set_data(si, TempPCM.Data, TempPCM.DataSize,
            //                vgmBuf[vgmAdr + 3].val, vgmBuf[vgmAdr + 4].val);

            byte bank = vgmBuf[vgmAdr + 2].val;
            byte StepSize = vgmBuf[vgmAdr + 3].val;
            byte StepBase = vgmBuf[vgmAdr + 4].val;

            chipRegister.DACControlSetStreamData(od, Audio.DriverSeqCounter, 0, si, bank, StepSize, StepBase);
            vgmAdr += 5;
        }

        private void vcSetStreamFrequency(outDatum od, ref uint vgmAdr)
        {
            //if (model != enmModel.VirtualModel)
            //{
            //    vgmAdr += 6;
            //    return;
            //}

            byte si = vgmBuf[vgmAdr + 1].val;
            if (si == 0xFF)// || !DacCtrl[si].Enable)
            {
                vgmAdr += 0x06;
                return;
            }
            uint TempLng = Common.getLE32(vgmBuf, vgmAdr + 2);
            ////Last95Freq = TempLng;
            //dacControl.set_frequency(si, TempLng);

            chipRegister.DACControlSetFrequency(od, Audio.DriverSeqCounter, 0, si, TempLng);
            vgmAdr += 6;
        }

        private void vcStartStream(outDatum od, ref uint vgmAdr)
        {
            //if (model != enmModel.VirtualModel)
            //{
            //    vgmAdr += 8;
            //    return;
            //}

            byte si = vgmBuf[vgmAdr + 1].val;
            if (si == 0xFF)// || !DacCtrl[si].Enable || PCMBank[DacCtrl[si].Bank].BankCount == 0)
            {
                vgmAdr += 0x0b;
                return;
            }
            uint DataStart = Common.getLE32(vgmBuf, vgmAdr + 2);
            //Last95Drum = 0xFFFF;
            byte TempByt = vgmBuf[vgmAdr + 6].val;
            uint DataLen = Common.getLE32(vgmBuf, vgmAdr + 7);
            //dacControl.start(si, DataStart, TempByt, DataLen, od);
            chipRegister.DACControlStartStream(od, Audio.DriverSeqCounter, 0, si, DataStart, TempByt, DataLen);
            vgmAdr += 0x0B;

        }

        private void vcStopStream(outDatum od, ref uint vgmAdr)
        {
            //if (model != enmModel.VirtualModel)
            //{
            //    vgmAdr += 2;
            //    return;
            //}

            byte si = vgmBuf[vgmAdr + 1].val;
            //if (si < 0xff && !DacCtrl[si].Enable)
            //{
            //    vgmAdr += 0x02;
            //    return;
            //}
            ////Last95Drum = 0xFFFF;
            //if (si < 0xFF)
            //{
            //    dacControl.stop(si);
            //}
            //else
            //{
            //    for (si = 0x00; si < 0xFF; si++)
            //        dacControl.stop(si);
            //}

            chipRegister.DACControlStopStream(od, Audio.DriverSeqCounter, 0, si);
            vgmAdr += 0x02;
        }

        private void vcStartStreamFastCall(outDatum od, ref uint vgmAdr)
        {
            //if (model != enmModel.VirtualModel)
            //{
            //    vgmAdr += 5;
            //    return;
            //}

            byte CurChip = vgmBuf[vgmAdr + 1].val;
            if (CurChip == 0xFF)// || !DacCtrl[CurChip].Enable ||
            //    PCMBank[DacCtrl[CurChip].Bank].BankCount == 0)
            {
                vgmAdr += 0x05;
                return;
            }
            //VGM_PCM_BANK TempPCM = PCMBank[DacCtrl[CurChip].Bank];
            uint TempSht = Common.getLE16(vgmBuf, vgmAdr + 2);
            ////Last95Drum = TempSht;
            ////Last95Max = TempPCM->BankCount;
            //if (TempSht >= TempPCM.BankCount)
            //    TempSht = 0x00;
            //VGM_PCM_DATA TempBnk = TempPCM.Bank[(int)TempSht];

            //byte TempByt = (byte)(dacControl.DCTRL_LMODE_BYTES |
            //            (vgmBuf[vgmAdr + 4].val & 0x10) |         // Reverse Mode
            //            ((vgmBuf[vgmAdr + 4].val & 0x01) << 7));   // Looping
            //dacControl.start(CurChip, TempBnk.DataStart, TempByt, TempBnk.DataSize, od);
            byte mode = vgmBuf[vgmAdr + 4].val;
            chipRegister.DACControlStartStream(od, Audio.DriverSeqCounter, 0, CurChip, TempSht, mode);
            vgmAdr += 0x05;

        }

        private void vcSeekToOffsetInPCMDataBank(outDatum od, ref uint vgmAdr)
        {
            PCMBank[0x00].DataPos = Common.getLE32(vgmBuf, vgmAdr + 1);
            vgmAdr += 5;
        }

        private void vcRf5c68(outDatum od)
        {
            byte id = (byte)((vgmBuf[vgmAdr + 1].val & 0x80) != 0 ? 1 : 0);
            byte cmd = (byte)(vgmBuf[vgmAdr + 1].val & 0x7f);
            chipRegister.writeRF5C68(id, cmd, vgmBuf[vgmAdr + 2].val);
            vgmAdr += 3;
        }

        private void vcRf5c68MemoryWrite(outDatum od)
        {
            uint offset = Common.getLE16(vgmBuf, vgmAdr + 1);
            chipRegister.writeRF5C68MemW(0, offset, vgmBuf[vgmAdr + 3].val);
            vgmAdr += 4;
        }

        private void vcRf5c164(outDatum od)
        {
            byte id = (byte)((vgmBuf[vgmAdr + 1].val & 0x80) != 0 ? 1 : 0);
            byte cmd = (byte)(vgmBuf[vgmAdr + 1].val & 0x7f);
            chipRegister.RF5C164SetRegister(od, Audio.DriverSeqCounter, id, cmd, vgmBuf[vgmAdr + 2].val);
            vgmAdr += 3;
        }

        private void vcRf5c164MemoryWrite(outDatum od)
        {
            uint offset = Common.getLE16(vgmBuf, vgmAdr + 1);
            chipRegister.writeRF5C164MemW(od, Audio.DriverSeqCounter, 0, offset, vgmBuf[vgmAdr + 3].val);
            vgmAdr += 4;
        }

        private void vcK051649(outDatum od)
        {
            int scc1_port = vgmBuf[vgmAdr + 1].val & 0x7f;
            byte scc1_offset = vgmBuf[vgmAdr + 2].val;
            byte rDat = vgmBuf[vgmAdr + 3].val;
            byte scc1_chipid = (byte)((vgmBuf[vgmAdr + 1].val & 0x80) != 0 ? 1 : 0);
            vgmAdr += 4;
            chipRegister.K051649SetRegister(od, Audio.DriverSeqCounter, scc1_chipid, (int)((scc1_port << 1) | 0x00), scc1_offset);
            chipRegister.K051649SetRegister(od, Audio.DriverSeqCounter, scc1_chipid, (int)((scc1_port << 1) | 0x01), rDat);

        }

        private void vcK053260(outDatum od)
        {
            byte id = (byte)((vgmBuf[vgmAdr + 1].val & 0x80) != 0 ? 1 : 0);
            int adr = (int)(vgmBuf[vgmAdr + 1].val & 0x7f);
            byte data = vgmBuf[vgmAdr + 2].val;
            chipRegister.K053260SetRegister(od, Audio.DriverSeqCounter, id, adr, data);
            vgmAdr += 3;
        }

        private void vcK054539(outDatum od)
        {
            byte id = (byte)((vgmBuf[vgmAdr + 1].val & 0x80) != 0 ? 1 : 0);
            int adr = (int)((vgmBuf[vgmAdr + 1].val & 0x7f) * 0x100 + vgmBuf[vgmAdr + 2].val);
            byte data = vgmBuf[vgmAdr + 3].val;
            chipRegister.K054539SetRegister(od, Audio.DriverSeqCounter, id, adr, data);
            vgmAdr += 4;
        }

        private void vcC140(outDatum od)
        {
            byte id = (byte)((vgmBuf[vgmAdr + 1].val & 0x80) != 0 ? 1 : 0);
            int adr = (vgmBuf[vgmAdr + 1].val & 0x7f) * 0x100 + (vgmBuf[vgmAdr + 2].val & 0xff);
            byte data = vgmBuf[vgmAdr + 3].val;
            chipRegister.C140SetRegister(od, Audio.DriverSeqCounter, id, adr, data);
            vgmAdr += 4;
        }

        private void AddPCMData(byte Type, uint DataSize, uint Adr)
        {
            uint CurBnk;
            VGM_PCM_BANK TempPCM;
            VGM_PCM_DATA TempBnk;
            uint BankSize;
            //bool RetVal;
            byte BnkType;
            byte CurDAC;

            BnkType = (byte)(Type & 0x3F);
            if (BnkType >= PCM_BANK_COUNT || vgmCurLoop > 0)
                return;

            if (Type == 0x7F)
            {
                //ReadPCMTable(DataSize, Data);
                ReadPCMTable(DataSize, Adr);
                return;
            }

            TempPCM = PCMBank[BnkType];// &PCMBank[BnkType];
            TempPCM.BnkPos++;
            if (TempPCM.BnkPos <= TempPCM.BankCount)
                return; // Speed hack for restarting playback (skip already loaded blocks)
            CurBnk = TempPCM.BankCount;
            TempPCM.BankCount++;
            //if (Last95Max != 0xFFFF) Last95Max = TempPCM.BankCount;
            TempPCM.Bank.Add(new VGM_PCM_DATA());// = (VGM_PCM_DATA*)realloc(TempPCM->Bank,
                                                 // sizeof(VGM_PCM_DATA) * TempPCM->BankCount);

            if ((Type & 0x40) == 0)
                BankSize = DataSize;
            else
                BankSize = Common.getLE32(vgmBuf, Adr + 1);// ReadLE32(&Data[0x01]);

            byte[] newData = new byte[TempPCM.DataSize + BankSize];
            if (TempPCM.Data != null && TempPCM.Data.Length > 0)
                Array.Copy(TempPCM.Data, newData, TempPCM.Data.Length);
            TempPCM.Data = newData;

            //TempPCM.Data = new byte[TempPCM.DataSize + BankSize];// realloc(TempPCM->Data, TempPCM->DataSize + BankSize);
            TempBnk = TempPCM.Bank[(int)CurBnk];
            TempBnk.DataStart = TempPCM.DataSize;
            TempBnk.Data = new byte[BankSize];
            if ((Type & 0x40) == 0)
            {
                TempBnk.DataSize = DataSize;
                for (int i = 0; i < DataSize; i++)
                {
                    TempPCM.Data[i + TempBnk.DataStart] = vgmBuf[Adr + i].val;
                    TempBnk.Data[i] = vgmBuf[Adr + i].val;
                }
                //TempBnk.Data = TempPCM.Data + TempBnk.DataStart;
                //memcpy(TempBnk->Data, Data, DataSize);
            }
            else
            {
                //TempBnk.Data = TempPCM.Data + TempBnk.DataStart;
                bool RetVal = DecompressDataBlk(TempBnk, DataSize, Adr);
                if (RetVal == false)
                {
                    TempBnk.Data = null;
                    TempBnk.DataSize = 0x00;
                    //return;
                    goto RefreshDACStrm;    // sorry for the goto, but I don't want to copy-paste the code
                }
                for (int i = 0; i < BankSize; i++)// DataSize; i++)
                {
                    TempPCM.Data[i + TempBnk.DataStart] = TempBnk.Data[i];
                }
            }
            //if (BankSize != TempBnk.DataSize) Console.Write("Error reading Data Block! Data Size conflict!\n");
            TempPCM.DataSize += BankSize;

        // realloc may've moved the Bank block, so refresh all DAC Streams
        RefreshDACStrm:
            for (CurDAC = 0x00; CurDAC < DacCtrlUsed; CurDAC++)
            {
                //if (DacCtrl[DacCtrlUsg[CurDAC]].Bank == BnkType)
                    //dacControl.refresh_data(DacCtrlUsg[CurDAC], TempPCM.Data, TempPCM.DataSize);
            }

            return;
        }

        private bool DecompressDataBlk(VGM_PCM_DATA Bank, uint DataSize, uint Adr)
        {
            byte ComprType;
            byte BitDec;
            byte BitCmp;
            byte CmpSubType;
            uint AddVal;
            uint InPos;
            uint InDataEnd;
            uint OutPos;
            uint OutDataEnd;
            uint InVal;
            uint OutVal = 0;// FUINT16 OutVal;
            byte ValSize;
            byte InShift;
            byte OutShift;
            uint Ent1B = 0;// UINT8* Ent1B;
            uint Ent2B = 0;// UINT16* Ent2B;
            //#if defined(_DEBUG) && defined(WIN32)
            //	UINT32 Time;
            //#endif

            // ReadBits Variables
            byte BitsToRead;
            byte BitReadVal;
            byte InValB;
            byte BitMask;
            byte OutBit;

            // Variables for DPCM
            uint OutMask;

            //#if defined(_DEBUG) && defined(WIN32)
            //	Time = GetTickCount();
            //#endif
            ComprType = vgmBuf[Adr + 0].val;
            Bank.DataSize = Common.getLE32(vgmBuf, Adr + 1);

            switch (ComprType)
            {
                case 0x00:  // n-Bit compression
                    BitDec = vgmBuf[Adr + 5].val;
                    BitCmp = vgmBuf[Adr + 6].val;
                    CmpSubType = vgmBuf[Adr + 7].val;
                    AddVal = Common.getLE16(vgmBuf, Adr + 8);

                    if (CmpSubType == 0x02)
                    {
                        //Bank.DataSize = 0x00;
                        //return false;

                        Ent1B = 0;// (UINT8*)PCMTbl.Entries; // Big Endian note: Those are stored in LE and converted when reading.
                        Ent2B = 0;// (UINT16*)PCMTbl.Entries;
                        if (PCMTbl.EntryCount == 0)
                        {
                            Bank.DataSize = 0x00;
                            //printf("Error loading table-compressed data block! No table loaded!\n");
                            return false;
                        }
                        else if (BitDec != PCMTbl.BitDec || BitCmp != PCMTbl.BitCmp)
                        {
                            Bank.DataSize = 0x00;
                            //printf("Warning! Data block and loaded value table incompatible!\n");
                            return false;
                        }
                    }

                    ValSize = (byte)((BitDec + 7) / 8);
                    InPos = Adr + 0x0A;
                    InDataEnd = Adr + DataSize;
                    InShift = 0;
                    OutShift = (byte)(BitDec - BitCmp);
                    //                    OutDataEnd = Bank.Data + Bank.DataSize;
                    OutDataEnd = Bank.DataSize;

                    //for (OutPos = Bank->Data; OutPos < OutDataEnd && InPos < InDataEnd; OutPos += ValSize)
                    for (OutPos = 0; OutPos < OutDataEnd && InPos < InDataEnd; OutPos += ValSize)
                    {
                        //InVal = ReadBits(Data, InPos, &InShift, BitCmp);
                        // inlined - is 30% faster
                        OutBit = 0x00;
                        InVal = 0x0000;
                        BitsToRead = BitCmp;
                        while (BitsToRead != 0)
                        {
                            BitReadVal = (byte)((BitsToRead >= 8) ? 8 : BitsToRead);
                            BitsToRead -= BitReadVal;
                            BitMask = (byte)((1 << BitReadVal) - 1);

                            InShift += BitReadVal;
                            //InValB = (byte)((vgmBuf[InPos].val << InShift >> 8) & BitMask);
                            InValB = (byte)((vgmBuf[InPos].val << InShift >> 8) & BitMask);
                            if (InShift >= 8)
                            {
                                InShift -= 8;
                                InPos++;
                                if (InShift != 0)
                                    InValB |= (byte)((vgmBuf[InPos].val << InShift >> 8) & BitMask);
                            }

                            InVal |= (uint)(InValB << OutBit);
                            OutBit += BitReadVal;
                        }

                        switch (CmpSubType)
                        {
                            case 0x00:  // Copy
                                OutVal = InVal + AddVal;
                                break;
                            case 0x01:  // Shift Left
                                OutVal = (InVal << OutShift) + AddVal;
                                break;
                            case 0x02:  // Table
                                switch (ValSize)
                                {
                                    case 0x01:
                                        OutVal = PCMTbl.Entries[Ent1B + InVal];
                                        break;
                                    case 0x02:
                                        //#ifndef BIG_ENDIAN
                                        //					OutVal = Ent2B[InVal];
                                        //#else
                                        OutVal = (uint)(PCMTbl.Entries[Ent2B + InVal * 2] + PCMTbl.Entries[Ent2B + InVal * 2 + 1] * 0x100);// ReadLE16((UINT8*)&Ent2B[InVal]);
                                                                                                                                           //#endif
                                        break;
                                }
                                break;
                        }

                        //#ifndef BIG_ENDIAN
                        //			//memcpy(OutPos, &OutVal, ValSize);
                        //			if (ValSize == 0x01)
                        //               *((UINT8*)OutPos) = (UINT8)OutVal;
                        //			else //if (ValSize == 0x02)
                        //                *((UINT16*)OutPos) = (UINT16)OutVal;
                        //#else
                        if (ValSize == 0x01)
                        {
                            Bank.Data[OutPos] = (byte)OutVal;
                        }
                        else //if (ValSize == 0x02)
                        {
                            Bank.Data[OutPos + 0x00] = (byte)((OutVal & 0x00FF) >> 0);
                            Bank.Data[OutPos + 0x01] = (byte)((OutVal & 0xFF00) >> 8);
                        }
                        //#endif
                    }
                    break;
                case 0x01:  // Delta-PCM
                    BitDec = vgmBuf[Adr + 5].val;// Data[0x05].val;
                    BitCmp = vgmBuf[Adr + 6].val;// Data[0x06].val;
                    OutVal = Common.getLE16(vgmBuf, Adr + 8);// ReadLE16(&Data[0x08]);

                    Ent1B = 0;// (UINT8*)PCMTbl.Entries;
                    Ent2B = 0;// (UINT16*)PCMTbl.Entries;
                    if (PCMTbl.EntryCount == 0)
                    {
                        Bank.DataSize = 0x00;
                        //printf("Error loading table-compressed data block! No table loaded!\n");
                        return false;
                    }
                    else if (BitDec != PCMTbl.BitDec || BitCmp != PCMTbl.BitCmp)
                    {
                        Bank.DataSize = 0x00;
                        //printf("Warning! Data block and loaded value table incompatible!\n");
                        return false;
                    }

                    ValSize = (byte)((BitDec + 7) / 8);
                    OutMask = (uint)((1 << BitDec) - 1);
                    InPos = vgmBuf[Adr + 0xa].val;// Data + 0x0A;
                    InDataEnd = vgmBuf[Adr + DataSize].val;// Data + DataSize;
                    InShift = 0;
                    //OutShift = (byte)(BitDec - BitCmp);
                    OutDataEnd = Bank.DataSize;// Bank.Data + Bank.DataSize;
                    //AddVal = 0x0000;

                    //                    for (OutPos = Bank.Data; OutPos < OutDataEnd && InPos < InDataEnd; OutPos += ValSize)
                    for (OutPos = 0; OutPos < OutDataEnd && InPos < InDataEnd; OutPos += ValSize)
                    {
                        //InVal = ReadBits(Data, InPos, &InShift, BitCmp);
                        // inlined - is 30% faster
                        OutBit = 0x00;
                        InVal = 0x0000;
                        BitsToRead = BitCmp;
                        while (BitsToRead != 0)
                        {
                            BitReadVal = (byte)((BitsToRead >= 8) ? 8 : BitsToRead);
                            BitsToRead -= BitReadVal;
                            BitMask = (byte)((1 << BitReadVal) - 1);

                            InShift += BitReadVal;
                            InValB = (byte)((vgmBuf[InPos].val << InShift >> 8) & BitMask);
                            if (InShift >= 8)
                            {
                                InShift -= 8;
                                InPos++;
                                if (InShift != 0)
                                    InValB |= (byte)((vgmBuf[InPos].val << InShift >> 8) & BitMask);
                            }

                            InVal |= (byte)(InValB << OutBit);
                            OutBit += BitReadVal;
                        }

                        switch (ValSize)
                        {
                            case 0x01:
                                AddVal = PCMTbl.Entries[Ent1B + InVal];
                                OutVal += AddVal;
                                OutVal &= OutMask;
                                Bank.Data[OutPos] = (byte)OutVal;// *((UINT8*)OutPos) = (UINT8)OutVal;
                                break;
                            case 0x02:
                                //#ifndef BIG_ENDIAN
                                //				AddVal = Ent2B[InVal];
                                //#else
                                AddVal = (uint)(PCMTbl.Entries[Ent2B + InVal] + PCMTbl.Entries[Ent2B + InVal + 1] * 0x100);
                                //AddVal = ReadLE16((UINT8*)&Ent2B[InVal]);
                                //#endif
                                OutVal += AddVal;
                                OutVal &= OutMask;
                                //#ifndef BIG_ENDIAN
                                //				*((UINT16*)OutPos) = (UINT16)OutVal;
                                //#else
                                Bank.Data[OutPos + 0x00] = (byte)((OutVal & 0x00FF) >> 0);
                                Bank.Data[OutPos + 0x01] = (byte)((OutVal & 0xFF00) >> 8);
                                //#endif
                                break;
                        }
                    }
                    break;
                default:
                    //printf("Error: Unknown data block compression!\n");
                    return false;
            }

            //#if defined(_DEBUG) && defined(WIN32)
            //	Time = GetTickCount() - Time;
            //	printf("Decompression Time: %lu\n", Time);
            //#endif

            return true;
        }

        private void ReadPCMTable(uint DataSize, uint Adr)
        {
            byte ValSize;
            uint TblSize;

            PCMTbl.ComprType = vgmBuf[Adr + 0].val;// Data[0x00].val;
            PCMTbl.CmpSubType = vgmBuf[Adr + 1].val;// Data[0x01].val;
            PCMTbl.BitDec = vgmBuf[Adr + 2].val;// Data[0x02].val;
            PCMTbl.BitCmp = vgmBuf[Adr + 3].val;// Data[0x03].val;
            PCMTbl.EntryCount = Common.getLE16(vgmBuf, Adr + 4);// ReadLE16(&Data[0x04].val);

            ValSize = (byte)((PCMTbl.BitDec + 7) / 8);
            TblSize = PCMTbl.EntryCount * ValSize;

            PCMTbl.Entries = new byte[TblSize];// realloc(PCMTbl.Entries, TblSize);
            for (int i = 0; i < TblSize; i++) PCMTbl.Entries[i] = vgmBuf[Adr + 6 + i].val;
            //memcpy(PCMTbl.Entries, &Data[0x06], TblSize);

            if (DataSize < 0x06 + TblSize)
            {
                //Console.Write("Warning! Bad PCM Table Length!\n");
                //printf("Warning! Bad PCM Table Length!\n");
            }

            return;
        }

        private byte GetDACFromPCMBank()
        {
            // for YM2612 DAC data only
            /*VGM_PCM_BANK* TempPCM;
            UINT32 CurBnk;*/
            uint DataPos;

            /*TempPCM = &PCMBank[0x00];
            DataPos = TempPCM->DataPos;
            for (CurBnk = 0x00; CurBnk < TempPCM->BankCount; CurBnk ++)
            {
                if (DataPos < TempPCM->Bank[CurBnk].DataSize)
                {
                    if (TempPCM->DataPos < TempPCM->DataSize)
                        TempPCM->DataPos ++;
                    return TempPCM->Bank[CurBnk].Data[DataPos];
                }
                DataPos -= TempPCM->Bank[CurBnk].DataSize;
            }
            return 0x80;*/

            DataPos = PCMBank[0x00].DataPos;
            if (DataPos >= PCMBank[0x00].DataSize)
                return 0x80;

            PCMBank[0x00].DataPos++;
            return PCMBank[0x00].Bank[0].Data[DataPos];
        }

        private uint? GetPCMAddressFromPCMBank(byte Type, uint DataPos)
        {
            if (Type >= PCM_BANK_COUNT)
                return null;

            if (DataPos >= PCMBank[Type].DataSize)
                return null;

            return DataPos;
        }

        protected bool getInformationHeader()
        {
            chips = new List<Chip>();
            UsedChips = "";

            SN76489ClockValue = 0;
            YM2612ClockValue = 0;
            YM2151ClockValue = 0;
            SEGAPCMClockValue = 0;
            YM2203ClockValue = 0;
            YM2608ClockValue = 0;
            YM2609ClockValue = 0;
            YM2610ClockValue = 0;
            YM3812ClockValue = 0;
            YMF262ClockValue = 0;
            RF5C68ClockValue = 0;
            RF5C164ClockValue = 0;
            PWMClockValue = 0;
            OKIM6258ClockValue = 0;
            C140ClockValue = 0;
            OKIM6295ClockValue = 0;
            AY8910ClockValue = 0;
            YM2413ClockValue = 0;
            HuC6280ClockValue = 0;
            K054539ClockValue = 0;
            NESClockValue = 0;
            VRC6ClockValue = 0;
            MultiPCMClockValue = 0;
            GigatronClockValue = 0;


            //ヘッダーを読み込めるサイズをもっているかチェック
            if (vgmBuf.Length < 0x40) return false;

            //ヘッダーから情報取得

            uint zgm = Common.getLE32(vgmBuf, 0x00);
            if (zgm != FCC_ZGM) return false;

            vgmEof = Common.getLE32(vgmBuf, 0x04);

            uint version = Common.getLE32(vgmBuf, 0x08);
            //バージョンチェック
            if (version < 10) return false;
            Version = string.Format("{0}.{1}{2}", (version & 0xf00) / 0x100, (version & 0xf0) / 0x10, (version & 0xf));

            TotalCounter = Common.getLE32(vgmBuf, 0x0c);
            if (TotalCounter < 0) return false;
            vgmLoopOffset = Common.getLE32(vgmBuf, 0x14);
            LoopCounter = Common.getLE32(vgmBuf, 0x10);

            uint defineAddress = Common.getLE32(vgmBuf, 0x1c);
            uint defineCount = Common.getLE16(vgmBuf, 0x24);
            //音源定義数チェック
            if (defineCount < 1) return false;

            chipCommandSize = (defineCount > 128) ? 2 : 1;

            uint trackAddress = Common.getLE32(vgmBuf, 0x20);
            uint trackCounter = Common.getLE16(vgmBuf, 0x26);
            vgmDataOffset = trackAddress + 11;
            //トラック数チェック
            if (trackCounter != 1) return false;
            uint fcc = Common.getLE24(vgmBuf, trackAddress);
            if (fcc != FCC_TRK) return false;
            uint trackLength = Common.getLE32(vgmBuf, trackAddress + 3);
            vgmLoopOffset = (int)Common.getLE32(vgmBuf, trackAddress + 7);
            if (vgmLoopOffset != 0) LoopCounter = 1;
            vgmEof = (uint)(trackAddress + trackLength);

            uint pos = defineAddress;

            Dictionary<string, int> chipCount = new Dictionary<string, int>();
            for (int i = 0; i < defineCount; i++)
            {
                fcc = Common.getLE24(vgmBuf, pos);
                if (fcc != FCC_DEF) return false;
                ZgmChip.ZgmChip chip = (new ZgmChip.ChipFactory()).Create(Common.getLE32(vgmBuf, pos + 0x4), chipRegister, setting, vgmBuf);
                if (chip == null) return false;//non support

                if (!chipCount.ContainsKey(chip.name)) chipCount.Add(chip.name, -1);
                chipCount[chip.name]++;

                chip.Setup(chipCount[chip.name], ref pos, ref vgmCmdTbl);
                chips.Add(chip);
            }

            UsedChips = GetUsedChipsString(chips);

            try
            {
                GD3 = getGD3Info(vgmBuf);
            }
            catch (ArgumentOutOfRangeException)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 使用チップを列挙した文字列を得る。
        /// (同じチップはカウントされ"x9"のように個数でまとめる。)
        /// </summary>
        private string GetUsedChipsString(List<Chip> chips)
        {
            //同じチップの数をそれぞれ集計する
            Dictionary<string, int> c = new Dictionary<string, int>();
            foreach (ZgmChip.ZgmChip chip in chips)
            {
                if (c.ContainsKey(chip.name)) c[chip.name]++;
                else c.Add(chip.name, 1);
            }
            List<string> cc = new List<string>();
            foreach (string k in c.Keys)
            {
                cc.Add(k + (c[k] < 2 ? "" : string.Format(" x{0}", c[k])));
            }

            return string.Join(" , ", cc);
        }

        public GD3 getGD3Info(outDatum[] buf)
        {
            uint vgmGd3 = Common.getLE32(vgmBuf, 0x18);
            if (vgmGd3 == 0) return null;
            uint vgmGd3Id = Common.getLE32(vgmBuf, vgmGd3);
            if (vgmGd3Id != FCC_GD3) throw new ArgumentOutOfRangeException();

            uint adr = vgmGd3 + 12;// + 0x14;
            GD3 = Common.getGD3Info(buf, adr);
            GD3.UsedChips = UsedChips;

            return GD3;
        }

        public override GD3 getGD3Info(byte[] buf, uint vgmGd3)
        {
            return null;
        }
    }

}
