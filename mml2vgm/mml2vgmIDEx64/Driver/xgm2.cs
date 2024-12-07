using Corex64;
using System;
using System.Net.Http.Headers;
using static MDSound.XGMFunction;

namespace mml2vgmIDEx64
{
    public class xgm2 : baseDriver
    {
        public const int FCC_XGM = 0x324d4758;	// "XGM2"
        public const int FCC_GD3 = 0x20336447;  // "Gd3 "
        public readonly int SN76489ClockValue = 3579545;
        public readonly int YM2612ClockValue = 7670454;

        private double musicStep = 1;// setting.outputDevice.SampleRate / 60.0;
        private double pcmStep = 1;// setting.outputDevice.SampleRate / 14000.0;
        private double musicDownCounter = 0.0;
        private double pcmDownCounter = 0.0;
        private uint fmmusicPtr = 0;
        private uint psgmusicPtr = 0;
        private byte DACEnable = 0;
        private bool ch3spEnable = false;
        private bool isNTSC = false;
        private bool existGD3 = false;
        private bool multiTrack = false;
        private bool packedData = false;

        public uint sampleDataBlockSize = 0;
        private uint fmDataBlockSize = 0;
        private uint psgDataBlockSize = 0;
        public uint sampleDataBlockAddr = 0;
        private uint fmDataBlockAddr = 0;
        private uint psgDataBlockAddr = 0;
        private uint gd3DataBlockAddr = 0;

        public XGMSampleID[] sampleID = null;
        private uint[] fmID = null;
        private uint[] psgID = null;
        private uint[] gd3ID = null;

        public XGM2PCM[] xgm2pcm = null;
        private double pcmSpeedCounter;

        private int fmWaitCnt = 0;
        private bool endFm = false;
        private uint fmLoopCnt = 0;
        private int psgWaitCnt = 0;
        private bool endPsg = false;
        private uint psgLoopCnt = 0;

        private byte[] vd = new byte[30];
        private byte[][][] fmTL = new byte[2][][] {
            new byte[3][] { new byte[4] , new byte[4] , new byte[4] } ,
            new byte[3][] { new byte[4] , new byte[4] , new byte[4] } };
        private byte[][][] fmSLRR = new byte[2][][] {
            new byte[3][] { new byte[4] , new byte[4] , new byte[4] } ,
            new byte[3][] { new byte[4] , new byte[4] , new byte[4] } };
        private byte[][] fmALG = new byte[2][] {
            new byte[3],
            new byte[3]};
        private byte[][] fmPanAmsPms = new byte[2][] {
            new byte[3],
            new byte[3]};
        private uint[][][] fmFreq = new uint[2][][] {
            new uint[3][] { new uint[4] , new uint[4] , new uint[4] } ,
            new uint[3][] { new uint[4] , new uint[4] , new uint[4] } };

        private uint[] psgFreq = new uint[4];
        private uint[] psgVol = new uint[4];

        private int pendingFrame;
        private static byte[] ch3FnumAdr = [0xad, 0xae, 0xac, 0xa6];//op1:0xad op2:0xae op3:0xac op4:0xa6
        private byte ch3KeyOn = 0;
        private bool vi = true;




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

            if (!getXGM2Info(vgmBuf)) return false;

            //Driverの初期化
            fmmusicPtr = fmDataBlockAddr;
            psgmusicPtr = psgDataBlockAddr;
            xgm2pcm = [new XGM2PCM(), new XGM2PCM(), new XGM2PCM(), new XGM2PCM()];

            DACEnable = 0;
            ch3spEnable = false;

            fmWaitCnt = 0;
            fmLoopCnt = 0;
            psgWaitCnt = 0;
            psgLoopCnt = 0;
            pendingFrame = 0;
            endFm = false;
            if (fmDataBlockSize == 0) endFm = true;
            endPsg = false;
            if (psgDataBlockSize == 0) endPsg = true;
            vi = true;

            return true;
        }

        public override void oneFrameProc()
        {
            try
            {
                if (vi)
                {
                    WriteYM2612P0(new outDatum(0x2b), new outDatum(0x80));
                    WriteYM2612P0(new outDatum(0x2a), new outDatum(0x80));
                    WriteYM2612P0(new outDatum(0x2b), new outDatum(0x00));
                    WriteYM2612P0(new outDatum(0x27), new outDatum(0x05));
                    vi = false;
                }

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
            getXGM2Info(buf);
            return GD3;
        }

        private GD3 getGD3Info(outDatum[] vgmBuf)
        {
            if (!existGD3) return new GD3();

            GD3 GD3 = Common.getGD3Info(vgmBuf, gd3DataBlockAddr + 12);
            GD3.UsedChips = UsedChips;

            return GD3;
        }

        private bool getXGM2Info(outDatum[] vgmBuf)
        {
            if (vgmBuf == null) return false;

            try
            {
                if (Common.getLE32(vgmBuf, 0) != FCC_XGM) return false;

                Version = string.Format("{0:d}", vgmBuf[0x0004].val);
                byte formatDesc = vgmBuf[0x0005].val;
                isNTSC = (formatDesc & 0x1) == 0;
                multiTrack = (formatDesc & 0x2) != 0;
                existGD3 = (formatDesc & 0x4) != 0;
                packedData = (formatDesc & 0x8) != 0;

                sampleDataBlockSize = Common.getLE16(vgmBuf, 0x0006) * 256;
                fmDataBlockSize = Common.getLE16(vgmBuf, 0x0008) * 256;
                psgDataBlockSize = Common.getLE16(vgmBuf, 0x000a) * 256;

                uint ptr = 0x000c;
                sampleID = new XGMSampleID[((multiTrack ? 504 : 248) - 12) / 2];//最後の12バイトはPCM SFXに使用する
                for (uint i = 0; i < sampleID.Length; i++)
                {
                    sampleID[i] = new XGMSampleID();
                    sampleID[i].addr = (Common.getLE16(vgmBuf, ptr+i*2) * 256);//0xffff00 is empty
                    sampleID[i].size = (Common.getLE16(vgmBuf, ptr+i*2 + 2) * 256);

                    if (sampleID[i].size == 0xffff00)
                    {
                        sampleID[i].size = sampleDataBlockSize - sampleID[i].addr;
                        break;
                    }
                    sampleID[i].size -= sampleID[i].addr;

                }

                ptr += (uint)(sampleID.Length * 2);
                ptr += 12;

                if (multiTrack)
                {
                    fmID = new uint[128];
                    for (uint i = 0; i < 128; i++)
                    {
                        fmID[i] = Common.getLE16(vgmBuf, ptr) * 256;//0xffff00 is empty
                        ptr += 2;
                    }
                    psgID = new uint[128];
                    for (uint i = 0; i < 128; i++)
                    {
                        psgID[i] = Common.getLE16(vgmBuf, ptr) * 256;//0xffff00 is empty
                        ptr += 2;
                    }
                }

                sampleDataBlockAddr = ptr;
                ptr += sampleDataBlockSize;
                fmDataBlockAddr = ptr;
                ptr += fmDataBlockSize;
                psgDataBlockAddr = ptr;
                ptr += psgDataBlockSize;

                if (multiTrack)
                {
                    gd3ID = new uint[128];
                    for (uint i = 0; i < 128; i++)
                    {
                        gd3ID[i] = Common.getLE16(vgmBuf, ptr) * 256;//0xffff00 is empty
                        ptr += 2;
                    }
                }
                gd3DataBlockAddr = ptr;

                if (!existGD3) GD3 = new GD3();
                else
                {
                    GD3 = Common.getGD3Info(vgmBuf, gd3DataBlockAddr + 12);
                    GD3.UsedChips = UsedChips;
                }

            }
            catch (Exception e)
            {
                log.Write(string.Format("XGM2の情報取得中に例外発生 Message=[{0}] StackTrace=[{1}]", e.Message, e.StackTrace));
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

                //jump処理
                while (jumpPointClock > 0)
                {
                    oneFrameXGM();
                    jumpPointClock--;
                }

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
                Stopped = true;
            }
        }

        private void oneFrameXGM()
        {
            if (!endFm) oneFrameFM();
            if (!endPsg) oneFramePsg();
            if (endFm && endPsg) Stopped = true;
            vgmCurLoop = Math.Min(fmLoopCnt, psgLoopCnt);

            ////wait
            //    if (jumpPointClock < 0) break;
            //    jumpPointClock--;//= (long)musicStep;

            ////end command
            //    Stopped = true;

            //// Dummy Command
            //    chipRegister.YM2612SetRegister(cmd, Audio.DriverSeqCounter, 0, 0, -1, -1);
            //    musicPtr += 2;

        }

        private void oneFrameFM()
        {
            if (fmWaitCnt-- > 0) return;

            while (true)
            {
                if (fmmusicPtr >= vgmBuf.Length) { endFm = true; return; }
                outDatum dat = vgmBuf[fmmusicPtr++];

                if (dat.type == musicDriverInterface.enmMMLType.IDE && dat.args.Count > 0 && dat.args[0] is outDatum)
                {
                    if (Corex64.Common.CheckDummyCommand(((outDatum)dat.args[0]).type))
                        chipRegister.YM2612SetRegister(((outDatum)dat.args[0]), Audio.DriverSeqCounter, 0, 0, -1, -1);
                    continue;
                }

                byte cmd = (byte)(dat.val & 0xf0);
                byte val = (byte)(dat.val & 0x0f);
                byte id, cs, port, keyOffOn, pan, slot, tl;
                outDatum odcmd = dat.Copy();
                outDatum odval = dat.Copy();
                odval.val = val;

                switch (cmd)
                {
                    case 0x00://wait
                        fmWaitCnt = val;
                        if (fmWaitCnt == 15) fmWaitCnt = vgmBuf[fmmusicPtr++].val + 15;
                        return;
                    case 0x10://pcm play
                        id = vgmBuf[fmmusicPtr++].val;
                        PlayPCM(val, id);
                        break;
                    case 0x20://ym2612 load instrument
                        cs = (byte)(val & 0x3);
                        port = (byte)((val & 0x4) >> 2);
                        for (int i = 0; i < 30; i++) vd[i] = vgmBuf[fmmusicPtr++].val;
                        SendInst(cs, port, vd);
                        break;
                    case 0x30://YM2612 frequency set + key OFF/ON
                        fmFreqSetAndKeyOffOn(odval);
                        break;
                    case 0x40://YM2612 key OFF/ON ($28)
                        cs = (byte)(val & 0x7);//0x28はport込み
                        keyOffOn = (byte)((val & 0x8) >> 3);
                        odcmd.val = 0x28;
                        WriteYM2612P0(odcmd, new outDatum((byte)((keyOffOn != 0 ? 0xf0 : 0x00) | cs)));
                        if (cs == 2)
                            ch3KeyOn = (byte)(keyOffOn != 0 ? 0xf0 : 0x00);
                        break;
                    case 0x50://YM2612 key sequence ($28)
                        cs = (byte)(val & 0x7);
                        keyOffOn = (byte)((val & 0x8) >> 3);
                        odcmd.val = 0x28;
                        if (keyOffOn == 0)
                        {
                            WriteYM2612P0(odcmd, new outDatum((byte)(0x00 | cs)));//OFF
                            WriteYM2612P0(odcmd, new outDatum((byte)(0xf0 | cs)));//ON
                        }
                        else
                        {
                            WriteYM2612P0(odcmd, new outDatum((byte)(0xf0 | cs)));//ON
                            WriteYM2612P0(odcmd, new outDatum((byte)(0x00 | cs)));//OFF
                        }
                        if (cs == 2)
                            ch3KeyOn = (byte)(keyOffOn != 0 ? 0xf0 : 0x00);
                        break;
                    case 0x60://YM2612 port 0 panning
                        cs = (byte)(val & 0x3);
                        pan = (byte)((val & 0xc) << 4);
                        fmPanAmsPms[0][cs] = (byte)((fmPanAmsPms[0][cs] & 0x3f) | pan);
                        odcmd.val = (byte)(0xb4 + cs);
                        WriteYM2612P0(odcmd, new outDatum(fmPanAmsPms[0][cs]));
                        break;
                    case 0x70://YM2612 port 1 panning
                        cs = (byte)(val & 0x3);
                        pan = (byte)((val & 0xc) << 4);
                        fmPanAmsPms[1][cs] = (byte)((fmPanAmsPms[1][cs] & 0x3f) | pan);
                        odcmd.val = (byte)(0xb4 + cs);
                        WriteYM2612P1(odcmd, new outDatum(fmPanAmsPms[1][cs]));
                        break;
                    case 0x80://YM2612 frequency set + key OFF/ON + end of frame
                        fmFreqSetAndKeyOffOn(odval);
                        return;
                    case 0x90://YM2612 TL set
                        cs = (byte)(val & 0x3);
                        slot = (byte)((val & 0xc) >> 2);
                        port = (byte)(vgmBuf[fmmusicPtr].val & 0x1);
                        tl = (byte)(vgmBuf[fmmusicPtr].val >> 1);
                        fmmusicPtr += 1;
                        fmTL[port][cs][slot] = tl;
                        odcmd.val = (byte)(0x40 + slot * 4 + cs);
                        WriteYM2612(port == 0, odcmd, new outDatum(fmTL[port][cs][slot]));
                        break;
                    case 0xa0://YM2612 frequency delta
                        fmFreqDeltaSet(odval);
                        break;
                    case 0xb0://YM2612 frequency delta + end of frame
                        fmFreqDeltaSet(odval);
                        return;
                    case 0xc0://YM2612 TL delta
                        fmTlDeltaSet(odval);
                        break;
                    case 0xd0://YM2612 TL delta + end of frame
                        fmTlDeltaSet(odval);
                        return;
                    case 0xe0://YM2612 general register write
                        cs = (byte)((val & 0x7) + 1);
                        port = (byte)((val & 0x8) >> 3);
                        for (int i = 0; i < cs; i++)
                        {
                            WriteYM2612(port == 0, vgmBuf[fmmusicPtr++], vgmBuf[fmmusicPtr++]);
                        }
                        break;
                    case 0xf0:
                        switch (val)
                        {
                            case 0x00://Frame splitter (for too long frame) - increment 'frame to process' counter
                                pendingFrame++;
                                return;
                            case 0x08://YM2612 advanced $28 (key) register write (not ALL OFF/ON)
                                dat = vgmBuf[fmmusicPtr];
                                fmmusicPtr += 1;
                                odcmd.val = 0x28;
                                WriteYM2612P0(odcmd, dat);
                                break;
                            case 0x09://YM2612 register $22 (LFO) write
                                dat = vgmBuf[fmmusicPtr];
                                fmmusicPtr += 1;
                                odcmd.val = 0x22;
                                WriteYM2612P0(odcmd, dat);
                                break;
                            case 0x0a://YM2612 register $27.6 = 1 (CH3 special mode enable)
                                odcmd.val = 0x27;
                                WriteYM2612P0(odcmd, new outDatum(0x45));
                                ch3spEnable = true;
                                break;
                            case 0x0b://YM2612 register $27.6 = 0 (CH3 special mode disable)
                                odcmd.val = 0x27;
                                WriteYM2612P0(odcmd, new outDatum(0x05));
                                ch3spEnable = false;
                                break;
                            case 0x0c://YM2612 register $2B = 80 (DAC enable)
                                odcmd.val = 0x2b;
                                WriteYM2612P0(odcmd, new outDatum(0x80));
                                DACEnable = 1;
                                break;
                            case 0x0d://YM2612 register $2B = 00 (DAC disable)
                                odcmd.val = 0x2b;
                                WriteYM2612P0(odcmd, new outDatum(0x00));
                                DACEnable = 0;
                                break;
                            case 0x0f:
                                uint loopAdr = Common.getLE24(vgmBuf, fmmusicPtr);
                                if (loopAdr == 0xffffff) endFm = true;
                                fmmusicPtr = fmDataBlockAddr + loopAdr;
                                fmLoopCnt++;
                                break;
                            default:
                                throw new NotImplementedException();
                        }
                        break;
                }
            }
        }

        private void fmTlDeltaSet(outDatum val)
        {
            byte cs, port, slot, tl, addOrsub;
            cs = (byte)(val.val & 0x3);
            slot = (byte)((val.val & 0xc) >> 2);
            port = (byte)(vgmBuf[fmmusicPtr].val & 0x1);
            addOrsub = (byte)((vgmBuf[fmmusicPtr].val & 0x2) >> 1);
            tl = (byte)((vgmBuf[fmmusicPtr].val >> 2) + 1);
            fmmusicPtr += 1;
            fmTL[port][cs][slot] = (byte)(fmTL[port][cs][slot] + (addOrsub == 0 ? 1 : -1) * tl);
            WriteYM2612(port == 0, new outDatum((byte)(0x40 + slot * 4 + cs)), new outDatum(fmTL[port][cs][slot]));
        }

        private void fmFreqDeltaSet(outDatum val)
        {
            byte cs, port, ch3m, addOrsub;
            uint freq;

            cs = (byte)(val.val & 0x3);
            port = (byte)((val.val & 0x4) >> 2);
            ch3m = (byte)((val.val & 0x8) >> 3);
            addOrsub = (byte)(vgmBuf[fmmusicPtr].val & 0x1);
            freq = (byte)((vgmBuf[fmmusicPtr].val >> 1) + 1);
            fmmusicPtr += 1;

            if (ch3m == 0)
            {
                fmFreq[port][cs][0] = (uint)(fmFreq[port][cs][0] + (addOrsub == 0 ? 1 : -1) * freq);
                WriteYM2612(port == 0, new outDatum((byte)(0xa4 + cs)), new outDatum((byte)(fmFreq[port][cs][0] >> 8)));
                WriteYM2612(port == 0, new outDatum((byte)(0xa0 + cs)), new outDatum((byte)fmFreq[port][cs][0]));
                return;
            }

            fmFreq[port][2][cs] = (uint)(fmFreq[port][2][cs] + (addOrsub == 0 ? 1 : -1) * freq);
            WriteYM2612(true, new outDatum(ch3FnumAdr[cs]), new outDatum((byte)(fmFreq[port][2][cs] >> 8)));
            WriteYM2612(true, new outDatum((byte)(ch3FnumAdr[cs] - 4)), new outDatum((byte)fmFreq[port][2][cs]));
        }

        private void fmFreqSetAndKeyOffOn(outDatum val)
        {
            byte cs, port, ch3m, keyOff, keyOn;
            uint freq;

            cs = (byte)(val.val & 0x3);
            port = (byte)((val.val & 0x4) >> 2);
            ch3m = (byte)((val.val & 0x8) >> 3);
            keyOff = (byte)((vgmBuf[fmmusicPtr].val & 0x40) >> 6);
            keyOn = (byte)((vgmBuf[fmmusicPtr].val & 0x80) >> 7);
            freq = Common.getBE16(vgmBuf, fmmusicPtr) & 0x3fff;
            fmmusicPtr += 2;

            if (ch3m == 0)
            {
                fmFreq[port][cs][0] = freq;
                if (keyOff != 0)
                {
                    if (cs == 2 && port == 0)
                        ch3KeyOn = 0x00;
                    WriteYM2612P0(new outDatum(0x28), new outDatum((byte)(0x00 | cs | (port << 2))));
                }
                WriteYM2612(port == 0,new outDatum( (byte)(0xa4 + cs)), new outDatum((byte)(freq >> 8)));
                WriteYM2612(port == 0, new outDatum((byte)(0xa0 + cs)), new outDatum((byte)freq));
                if (keyOn != 0)
                {
                    if (cs == 2 && port == 0)
                        ch3KeyOn = 0xf0;
                    WriteYM2612P0(new outDatum(0x28), new outDatum((byte)(0xf0 | cs | (port << 2))));
                }
                return;
            }


            fmFreq[port][2][cs] = freq;

            //byte m = (byte)(0x20 << cs);
            //if (keyOff != 0)
            //{
            //    ch3KeyOn = (byte)(ch3KeyOn & (~m));
            //    WriteYM2612P0(0x28, (byte)(ch3KeyOn | 2));
            //}
            WriteYM2612(true, new outDatum(ch3FnumAdr[cs]),new outDatum( (byte)(fmFreq[port][2][cs] >> 8)));
            WriteYM2612(true, new outDatum((byte)(ch3FnumAdr[cs] - 4)), new outDatum((byte)fmFreq[port][2][cs]));
            //if (keyOn != 0)
            //{
            //    ch3KeyOn = (byte)(ch3KeyOn | m);
            //    WriteYM2612P0(0x28, (byte)(ch3KeyOn | 2));
            //}
        }

        private void SendInst(byte cs, byte port, byte[] vd)
        {
            outDatum dmy = new outDatum();
            //ml/dt
            chipRegister.YM2612SetRegister(dmy, Audio.DriverSeqCounter, 0, port, 0x30 + cs, vd[0]);
            chipRegister.YM2612SetRegister(dmy, Audio.DriverSeqCounter, 0, port, 0x34 + cs, vd[1]);
            chipRegister.YM2612SetRegister(dmy, Audio.DriverSeqCounter, 0, port, 0x38 + cs, vd[2]);
            chipRegister.YM2612SetRegister(dmy, Audio.DriverSeqCounter, 0, port, 0x3c + cs, vd[3]);
            //tl
            fmTL[port][cs][0] = vd[4];
            fmTL[port][cs][1] = vd[5];
            fmTL[port][cs][2] = vd[6];
            fmTL[port][cs][3] = vd[7];
            //AR/SR
            chipRegister.YM2612SetRegister(dmy, Audio.DriverSeqCounter, 0, port, 0x50 + cs, vd[8]);
            chipRegister.YM2612SetRegister(dmy, Audio.DriverSeqCounter, 0, port, 0x54 + cs, vd[9]);
            chipRegister.YM2612SetRegister(dmy, Audio.DriverSeqCounter, 0, port, 0x58 + cs, vd[10]);
            chipRegister.YM2612SetRegister(dmy, Audio.DriverSeqCounter, 0, port, 0x5c + cs, vd[11]);
            //DR/AM
            chipRegister.YM2612SetRegister(dmy, Audio.DriverSeqCounter, 0, port, 0x60 + cs, vd[12]);
            chipRegister.YM2612SetRegister(dmy, Audio.DriverSeqCounter, 0, port, 0x64 + cs, vd[13]);
            chipRegister.YM2612SetRegister(dmy, Audio.DriverSeqCounter, 0, port, 0x68 + cs, vd[14]);
            chipRegister.YM2612SetRegister(dmy, Audio.DriverSeqCounter, 0, port, 0x6c + cs, vd[15]);
            //SR
            chipRegister.YM2612SetRegister(dmy, Audio.DriverSeqCounter, 0, port, 0x70 + cs, vd[16]);
            chipRegister.YM2612SetRegister(dmy, Audio.DriverSeqCounter, 0, port, 0x74 + cs, vd[17]);
            chipRegister.YM2612SetRegister(dmy, Audio.DriverSeqCounter, 0, port, 0x78 + cs, vd[18]);
            chipRegister.YM2612SetRegister(dmy, Audio.DriverSeqCounter, 0, port, 0x7c + cs, vd[19]);
            //SL/RR
            fmSLRR[port][cs][0] = vd[20];
            fmSLRR[port][cs][1] = vd[21];
            fmSLRR[port][cs][2] = vd[22];
            fmSLRR[port][cs][3] = vd[23];
            //SSGEG
            chipRegister.YM2612SetRegister(dmy, Audio.DriverSeqCounter, 0, port, 0x90 + cs, vd[24]);
            chipRegister.YM2612SetRegister(dmy, Audio.DriverSeqCounter, 0, port, 0x94 + cs, vd[25]);
            chipRegister.YM2612SetRegister(dmy, Audio.DriverSeqCounter, 0, port, 0x98 + cs, vd[26]);
            chipRegister.YM2612SetRegister(dmy, Audio.DriverSeqCounter, 0, port, 0x9c + cs, vd[27]);
            //FB/ALG
            chipRegister.YM2612SetRegister(dmy, Audio.DriverSeqCounter, 0, port, 0xb0 + cs, vd[28]);
            fmALG[port][cs] = (byte)(vd[28] & 0xf);

            chipRegister.YM2612SetRegister(dmy, Audio.DriverSeqCounter, 0, port, 0x40 + cs, vd[4]);
            chipRegister.YM2612SetRegister(dmy, Audio.DriverSeqCounter, 0, port, 0x44 + cs, vd[5]);
            chipRegister.YM2612SetRegister(dmy, Audio.DriverSeqCounter, 0, port, 0x48 + cs, vd[6]);
            chipRegister.YM2612SetRegister(dmy, Audio.DriverSeqCounter, 0, port, 0x4c + cs, vd[7]);
            chipRegister.YM2612SetRegister(dmy, Audio.DriverSeqCounter, 0, port, 0x80 + cs, vd[20]);
            chipRegister.YM2612SetRegister(dmy, Audio.DriverSeqCounter, 0, port, 0x84 + cs, vd[21]);
            chipRegister.YM2612SetRegister(dmy, Audio.DriverSeqCounter, 0, port, 0x88 + cs, vd[22]);
            chipRegister.YM2612SetRegister(dmy, Audio.DriverSeqCounter, 0, port, 0x8c + cs, vd[23]);

            //pan/ams/pms
            chipRegister.YM2612SetRegister(dmy, Audio.DriverSeqCounter, 0, port, 0xb4 + cs, vd[29]);
            fmPanAmsPms[port][cs] = vd[29];
        }

        private void PlayPCM(byte X, byte id)
        {
            //PCM play command:
            //ポート11をXGMPCM2の操作用として使用する!
            chipRegister.YM2612SetRegister(null, Audio.DriverSeqCounter, 0, 11, X, id);
        }



        private void oneFramePsg()
        {
            if (psgWaitCnt-- > 0) return;

            while (true)
            {
                if (psgmusicPtr >= vgmBuf.Length) { endPsg = true; return; }

                outDatum odDat = vgmBuf[psgmusicPtr++];

                if (odDat.type == musicDriverInterface.enmMMLType.IDE && odDat.args.Count > 0 && odDat.args[0] is outDatum)
                {
                    if (Corex64.Common.CheckDummyCommand(((outDatum)odDat.args[0]).type))
                        chipRegister.YM2612SetRegister(((outDatum)odDat.args[0]), Audio.DriverSeqCounter, 0, 0, -1, -1);
                    continue;
                }

                byte dat = odDat.val;
                byte cmd = (byte)(dat & 0xf0);
                byte val = (byte)(dat & 0x0f);

                bool eof, addOrsub;
                byte td, ch, delta, env;

                switch (cmd)
                {
                    case 0x00://wait
                        psgWaitCnt = val;
                        if (psgWaitCnt == 14) psgWaitCnt = vgmBuf[psgmusicPtr++].val + 14;
                        else if (psgWaitCnt == 15)
                        {
                            uint loopAdr = Common.getLE24(vgmBuf, psgmusicPtr);
                            if (loopAdr == 0xffffff) endPsg = true;
                            psgmusicPtr = psgDataBlockAddr + loopAdr;
                            psgLoopCnt++;
                            break;
                        }
                        return;
                    case 0x10://PSG freq/tone low update + end of frame
                        eof = (val & 1) != 0;
                        odDat= vgmBuf[psgmusicPtr++];
                        dat = odDat.val;
                        WritePSG(odDat);
                        ch = (byte)((dat & 0x60) >> 5);
                        psgFreq[ch] = (uint)((psgFreq[ch] & 0x3f0) | (uint)(dat & 0xf));
                        if (eof) return;
                        break;
                    case 0x20://PSG freq/tone update
                        td = (byte)(val & 3);
                        ch = (byte)((val & 0xc) >> 2);
                        odDat = vgmBuf[psgmusicPtr++];
                        dat = odDat.val;
                        psgFreq[ch] = (uint)(dat | (td << 8));
                        WritePSG(new outDatum((byte)(0x80 | (byte)(ch << 5) | (byte)(psgFreq[ch] & 0xf))));
                        WritePSG(new outDatum((byte)((psgFreq[ch] & 0x3f0) >> 4)));
                        break;
                    case 0x30://PSG freq/tone update + end of frame
                        td = (byte)(val & 3);
                        ch = (byte)((val & 0xc) >> 2);
                        odDat = vgmBuf[psgmusicPtr++];
                        dat = odDat.val;
                        psgFreq[ch] = (uint)(dat | (td << 8));
                        WritePSG(new outDatum((byte)(0x80 | (byte)(ch << 5) | (byte)(psgFreq[ch] & 0xf))));
                        WritePSG(new outDatum((byte)((psgFreq[ch] & 0x3f0) >> 4)));
                        return;
                    case 0x40:
                        delta = (byte)((val & 3) + 1);
                        addOrsub = ((val & 4) != 0);//false:add  true:sub
                        eof = (val & 8) != 0;
                        ch = 0;
                        psgFreq[ch] = (uint)(psgFreq[ch] + (addOrsub ? -1 : 1) * delta);
                        WritePSG(new outDatum((byte)(0x80 | (byte)(ch << 5) | (byte)(psgFreq[ch] & 0xf))));
                        WritePSG(new outDatum((byte)((psgFreq[ch] & 0x3f0) >> 4)));
                        if (eof) return;
                        break;
                    case 0x50:
                        delta = (byte)((val & 3) + 1);
                        addOrsub = ((val & 4) != 0);//false:add  true:sub
                        eof = (val & 8) != 0;
                        ch = 1;
                        psgFreq[ch] = (uint)(psgFreq[ch] + (addOrsub ? -1 : 1) * delta);
                        WritePSG(new outDatum((byte)(0x80 | (byte)(ch << 5) | (byte)(psgFreq[ch] & 0xf))));
                        WritePSG(new outDatum((byte)((psgFreq[ch] & 0x3f0) >> 4)));
                        if (eof) return;
                        break;
                    case 0x60:
                        delta = (byte)((val & 3) + 1);
                        addOrsub = ((val & 4) != 0);//false:add  true:sub
                        eof = (val & 8) != 0;
                        ch = 2;
                        psgFreq[ch] = (uint)(psgFreq[ch] + (addOrsub ? -1 : 1) * delta);
                        WritePSG(new outDatum((byte)(0x80 | (byte)(ch << 5) | (byte)(psgFreq[ch] & 0xf))));
                        WritePSG(new outDatum((byte)((psgFreq[ch] & 0x3f0) >> 4)));
                        if (eof) return;
                        break;
                    case 0x70:
                        delta = (byte)((val & 3) + 1);
                        addOrsub = ((val & 4) != 0);//false:add  true:sub
                        eof = (val & 8) != 0;
                        ch = 3;
                        psgFreq[ch] = (uint)(psgFreq[ch] + (addOrsub ? -1 : 1) * delta);
                        WritePSG(new outDatum((byte)(0x80 | (byte)(ch << 5) | (byte)(psgFreq[ch] & 0xf))));
                        WritePSG(new outDatum((byte)((psgFreq[ch] & 0x3f0) >> 4)));
                        if (eof) return;
                        break;
                    case 0x80://PSG ch0 vol/env update
                        env = (byte)val;
                        ch = 0;
                        psgVol[ch] = env;
                        WritePSG(new outDatum((byte)(0x90 | (byte)(ch << 5) | (byte)(psgVol[ch] & 0xf))));
                        break;
                    case 0x90:
                        env = (byte)val;
                        ch = 1;
                        psgVol[ch] = env;
                        WritePSG(new outDatum((byte)(0x90 | (byte)(ch << 5) | (byte)(psgVol[ch] & 0xf))));
                        break;
                    case 0xa0:
                        env = (byte)val;
                        ch = 2;
                        psgVol[ch] = env;
                        WritePSG(new outDatum((byte)(0x90 | (byte)(ch << 5) | (byte)(psgVol[ch] & 0xf))));
                        break;
                    case 0xb0:
                        env = (byte)val;
                        ch = 3;
                        psgVol[ch] = env;
                        WritePSG(new outDatum((byte)(0x90 | (byte)(ch << 5) | (byte)(psgVol[ch] & 0xf))));
                        break;
                    case 0xc0:
                        delta = (byte)((val & 3) + 1);
                        addOrsub = ((val & 4) != 0);//false:add  true:sub
                        eof = (val & 8) != 0;
                        ch = 0;
                        psgVol[ch] = (uint)(psgVol[ch] + (addOrsub ? -1 : 1) * delta);
                        WritePSG(new outDatum((byte)(0x90 | (byte)(ch << 5) | (byte)(psgVol[ch] & 0xf))));
                        if (eof) return;
                        break;
                    case 0xd0:
                        delta = (byte)((val & 3) + 1);
                        addOrsub = ((val & 4) != 0);//false:add  true:sub
                        eof = (val & 8) != 0;
                        ch = 1;
                        psgVol[ch] = (uint)(psgVol[ch] + (addOrsub ? -1 : 1) * delta);
                        WritePSG(new outDatum((byte)(0x90 | (byte)(ch << 5) | (byte)(psgVol[ch] & 0xf))));
                        if (eof) return;
                        break;
                    case 0xe0:
                        delta = (byte)((val & 3) + 1);
                        addOrsub = ((val & 4) != 0);//false:add  true:sub
                        eof = (val & 8) != 0;
                        ch = 2;
                        psgVol[ch] = (uint)(psgVol[ch] + (addOrsub ? -1 : 1) * delta);
                        WritePSG(new outDatum((byte)(0x90 | (byte)(ch << 5) | (byte)(psgVol[ch] & 0xf))));
                        if (eof) return;
                        break;
                    case 0xf0:
                        delta = (byte)((val & 3) + 1);
                        addOrsub = ((val & 4) != 0);//false:add  true:sub
                        eof = (val & 8) != 0;
                        ch = 3;
                        psgVol[ch] = (uint)(psgVol[ch] + (addOrsub ? -1 : 1) * delta);
                        WritePSG(new outDatum((byte)(0x90 | (byte)(ch << 5) | (byte)(psgVol[ch] & 0xf))));
                        if (eof) return;
                        break;
                }
            }
        }




        private void WritePSG(outDatum cmd)
        {
            chipRegister.SN76489SetRegister(cmd, Audio.DriverSeqCounter, 0, cmd.val);
        }

        private void WriteYM2612(bool isP0, outDatum adr, outDatum val)
        {
            if (isP0) WriteYM2612P0(adr, val);
            else WriteYM2612P1(adr, val);

            if (adr.val >= 0x40 && adr.val < 0x50)
            {
                byte slot = (byte)((adr.val - 0x40) / 4);
                byte cs = (byte)((adr.val - 0x40) % 4);
                fmTL[isP0 ? 0 : 1][cs][slot] = val.val;
            }
        }

        private void WriteYM2612P0(outDatum adr, outDatum val)
        {
            if (adr.val == 0x2b) DACEnable = (byte)(val.val & 0x80);
            else if (adr.val == 0x27) ch3spEnable = ((val.val & 0x40) != 0);

            chipRegister.YM2612SetRegister(adr, Audio.DriverSeqCounter, 0, 0, adr.val, val.val);
        }

        private void WriteYM2612P1(outDatum adr, outDatum val)
        {
            chipRegister.YM2612SetRegister(adr, Audio.DriverSeqCounter, 0, 1, adr.val, val.val);
        }

        public override GD3 getGD3Info(byte[] buf, uint vgmGd3)
        {
            throw new NotImplementedException();
        }

    }

}
