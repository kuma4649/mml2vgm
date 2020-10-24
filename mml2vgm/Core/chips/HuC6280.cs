using musicDriverInterface;
using System;
using System.Collections.Generic;

namespace Core
{
    public class HuC6280 : ClsChip
    {
        public byte CurrentChannel = 0xff;
        public int TotalVolume = 15;
        public int MAXTotalVolume = 15;

        public HuC6280(ClsVgm parent, int chipID, string initialPartName, string stPath, int chipNumber) : base(parent, chipID, initialPartName, stPath, chipNumber)
        {
            _chipType = enmChipType.HuC6280;
            _Name = "HuC6280";
            _ShortName = "HuC8";
            _ChMax = 6;
            _canUsePcm = true;
            _canUsePI = false;
            ChipNumber = chipNumber;

            Frequency = 3579545;
            port = new byte[][] { new byte[] { 0xb9 } };

            if (string.IsNullOrEmpty(initialPartName)) return;

            Ch = new ClsChannel[ChMax];
            SetPartToCh(Ch, initialPartName);
            foreach (ClsChannel ch in Ch)
            {
                ch.Type = enmChannelType.WaveForm;
                ch.chipNumber = chipID == 1;
                ch.MaxVolume = 15;
            }

            pcmDataInfo = new clsPcmDataInfo[] { new clsPcmDataInfo() };
            pcmDataInfo[0].totalBufPtr = 0L;
            pcmDataInfo[0].use = false;

            if (parent.info.format == enmFormat.ZGM)
            {
                if (parent.ChipCommandSize == 2)
                {
                    if (chipNumber == 0) pcmDataInfo[0].totalBuf = new byte[] { 0x07, 0x00, 0x66, 0x05, 0x00, 0x00, 0x00, 0x00 };
                    else pcmDataInfo[0].totalBuf = new byte[] { 0x07, 0x00, 0x66, 0x05, 0x00, 0x00, 0x00, 0x00 };
                }
                else
                {
                    if (chipNumber == 0) pcmDataInfo[0].totalBuf = new byte[] { 0x07, 0x66, 0x05, 0x00, 0x00, 0x00, 0x00 };
                    else pcmDataInfo[0].totalBuf = new byte[] { 0x07, 0x66, 0x05, 0x00, 0x00, 0x00, 0x00 };
                }
            }
            else
            {
                if (chipNumber == 0) pcmDataInfo[0].totalBuf = new byte[] { 0x67, 0x66, 0x05, 0x00, 0x00, 0x00, 0x00 };
                else pcmDataInfo[0].totalBuf = new byte[] { 0x67, 0x66, 0x05, 0x00, 0x00, 0x00, 0x00 };
            }

            pcmDataInfo[0].totalHeaderLength = pcmDataInfo[0].totalBuf.Length;
            pcmDataInfo[0].totalHeadrSizeOfDataPtr = (parent.ChipCommandSize == 2) ? 4 : 3;


            Envelope = new Function();
            Envelope.Max = 31;
            Envelope.Min = 0;

        }

        public override void InitChip()
        {
            if (!use) return;

            //MasterVolume(Max volume)
            TotalVolume = 0xff;
            parent.OutData((MML)null, port[0], (byte)((ChipNumber != 0 ? 0x80 : 0x00) + 1), 0xff);
            //LFO freq 0
            parent.OutData((MML)null, port[0], (byte)((ChipNumber != 0 ? 0x80 : 0x00) + 8), 0xff);
            //LFO ctrl 0
            parent.OutData((MML)null, port[0], (byte)((ChipNumber != 0 ? 0x80 : 0x00) + 9), 0xff);

            SupportReversePartWork = true;

            foreach (partWork pw in lstPartWork)
            {
                foreach (partPage pg in pw.pg)
                {
                    SetHuC6280CurrentChannel(null, pg);

                    pg.port = port;// 0xb9;

                    //freq( 0 )
                    pg.freq = 0;
                    OutHuC6280Port(pg, null, port[0], 2, 0);
                    OutHuC6280Port(pg, null, port[0], 3, 0);

                    pg.pcm = false;

                    //volume
                    byte data = (byte)(0x00 + (0 & 0x1f));
                    OutHuC6280Port(pg, null, port[0], 4, data);

                    //pan
                    pg.panL = 0;
                    pg.panR = 0;
                    OutHuC6280Port(pg, null, port[0], 5, 0xff);

                    for (int j = 0; j < 32; j++)
                    {
                        OutHuC6280Port(pg, null, port[0], 6, 0);
                    }

                    if (pg.ch > 3)
                    {
                        //noise(Ch5,6 only)
                        pg.noise = 0x1f;
                        OutHuC6280Port(pg, null, port[0], 7, 0x1f);
                    }
                }
            }
        }

        public override void InitPart(partWork pw)
        {
            foreach (partPage pg in pw.pg)
            {
                pg.MaxVolume = 31;
                pg.volume = pg.MaxVolume;
                pg.keyOn = false;
                pg.pan = 0;
                pg.mixer = 0;
                pg.noise = 0;
                pg.port = port;
                pg.freq = -1;
            }
            pw.spg.freq = -1;
        }

        public override void StorePcm(Dictionary<int, clsPcm> newDic, KeyValuePair<int, clsPcm> v, byte[] buf, bool is16bit, int samplerate, params object[] option)
        {
            clsPcmDataInfo pi = pcmDataInfo[0];

            try
            {
                long size = buf.Length;

                for (int i = 0; i < size; i++)
                {
                    buf[i] >>= 3;//5bit化
                }

                if (newDic.ContainsKey(v.Key))
                {
                    newDic.Remove(v.Key);
                }

                newDic.Add(
                    v.Key
                    , new clsPcm(
                        v.Value.num
                        , v.Value.seqNum
                        , v.Value.chip
                        , 0
                        , v.Value.fileName
                        , v.Value.freq != -1 ? v.Value.freq : samplerate
                        , v.Value.vol
                        , pi.totalBufPtr
                        , pi.totalBufPtr + size - 1
                        , size
                        , -1
                        , is16bit
                        , samplerate
                        )
                    );

                pi.totalBufPtr += size;

                byte[] newBuf = new byte[pi.totalBuf.Length + buf.Length];
                Array.Copy(pi.totalBuf, newBuf, pi.totalBuf.Length);
                Array.Copy(buf, 0, newBuf, pi.totalBuf.Length, buf.Length);

                pi.totalBuf = newBuf;

                pi.use = true;
                Common.SetUInt32bit31(
                    pi.totalBuf
                    , pi.totalHeadrSizeOfDataPtr
                    , (UInt32)(pi.totalBuf.Length - (pi.totalHeadrSizeOfDataPtr + 4))
                    , ChipNumber != 0);
                pcmDataEasy = pi.use ? pi.totalBuf : null;

            }
            catch
            {
                pi.use = false;
                return;
            }

        }

        public override void StorePcmRawData(clsPcmDatSeq pds, byte[] buf, bool isRaw, bool is16bit, int samplerate, params object[] option)
        {
            msgBox.setWrnMsg(msg.get("E12007"), new LinePos("-"));
        }

        public override int GetFNum(partPage page, MML mml, int octave, char cmd, int shift)
        {
            return GetHuC6280Freq(octave, cmd, shift);
        }

        private int GetHuC6280Freq(int octave, char noteCmd, int shift)
        {
            int o = octave;
            int n = Const.NOTE.IndexOf(noteCmd) + shift;

            o += n / 12;
            n %= 12;
            if (n < 0)
            {
                n += 12;
                o = Common.CheckRange(--o, 1, 8);
            }
            //if (n >= 0)
            //{
            //    o += n / 12;
            //    o = Common.CheckRange(o, 1, 8);
            //    n %= 12;
            //}
            //else
            //{
            //    o += n / 12 - 1;
            //    o = Common.CheckRange(o, 1, 8);
            //    n %= 12;
            //    if (n < 0) { n += 12; }
            //}
            return (int)(Frequency / 32.0f / 261.62f / (Const.pcmMTbl[n] * (float)Math.Pow(2, (o - 4))));
        }

        public void SetHuC6280Envelope(MML mml, partPage page, int volume)
        {
            //if (page.huc6280Envelope != volume)
            //{
            SetHuC6280CurrentChannel(mml, page);
            //if (!page.keyOn) volume = 0;
            byte data = (byte)((volume != 0 ? 0x80 : 0) + (volume & 0x1f));
            if (page.pcm) data |= 0x40;
            OutHuC6280Port(page, mml, port[0], 4, data);
            //page.huc6280Envelope = volume;
            //}
        }

        partPage CurrentPage = null;
        int bCurrentChannel = 0;

        public void SetHuC6280CurrentChannel(MML mml, partPage page)
        {
            if (CurrentPage != page)
            {
                CurrentPage = page;
                bCurrentChannel = 0;
            }

            byte pch = (byte)page.ch;
            

            if (CurrentChannel != pch || bCurrentChannel==0)
            {
                byte data = (byte)(pch & 0x7);
                OutHuC6280Port(page, mml, port[0], 0x0, data);
                CurrentChannel = pch;
                bCurrentChannel = 1;
            }
        }

        public void SetHuC6280Pan(MML mml, partPage page, int pan)
        {
            if (page.spg.huc6280Pan != pan)
            {
                SetHuC6280CurrentChannel(mml, page);
                byte data = (byte)(pan & 0xff);
                OutHuC6280Port(page, mml, port[0], 0x5, data);
                page.spg.huc6280Pan = pan;
            }
        }

        public void OutHuC6280Port(partPage page, MML mml, byte[] cmd, byte adr, byte data)
        {
            SOutData(
                page,
                mml,
                cmd
                , (byte)((ChipNumber != 0 ? 0x80 : 0x00) + adr)
                , data);
        }

        public void OutHuC6280SetInstrument(partPage page, MML mml, int n)
        {

            if (!parent.instWF.ContainsKey(n))
            {
                msgBox.setWrnMsg(string.Format(msg.get("E12000"), n), mml.line.Lp);
                return;
            }

            SetHuC6280CurrentChannel(mml, page);
            OutHuC6280Port(page, mml, port[0], 4, (byte)(0x40 + page.volume)); //WaveIndexReset(=0x40)

            for (int i = 1; i < parent.instWF[n].Length; i++) // 0 は音色番号が入っている為1からスタート
            {
                OutHuC6280Port(page, mml, port[0], 6, (byte)(parent.instWF[n][i] & 0x1f));
            }

        }

        public void OutHuC6280KeyOn(MML mml, partPage page)
        {
            page.keyOff = false;
            int vol = page.volume;
            if (page.envelopeMode)
            {
                vol = 0;
                if (page.envIndex != -1)
                {
                    vol = page.envVolume - (31 - page.volume);
                }
            }
            if (vol > 31) vol = 31;
            if (vol < 0) vol = 0;
            byte data = (byte)(((vol > 0) ? 0x80 : 0x00) + vol);

            if (!page.pcm)
            {
                SetHuC6280CurrentChannel(mml, page);
                OutHuC6280Port(page, mml, port[0], 0x4, data);
                OutHuC6280Port(page, mml, port[0], 0x5, (byte)page.huc6280Pan);
                return;
            }

            if (parent.info.Version == 1.51f)
            {
                return;
            }

            SetHuC6280CurrentChannel(mml, page);
            data |= 0x40;
            OutHuC6280Port(page, mml, port[0], 0x4, data);
            OutHuC6280Port(page, mml, port[0], 0x5, (byte)page.huc6280Pan);

            if (page.isPcmMap)
            {
                int nt = Const.NOTE.IndexOf(page.noteCmd);
                int ff = page.octaveNow * 12 + nt + page.shift + page.keyShift + page.arpDelta;
                if (parent.instPCMMap.ContainsKey(page.pcmMapNo))
                {
                    if (parent.instPCMMap[page.pcmMapNo].ContainsKey(ff))
                    {
                        page.instrument = parent.instPCMMap[page.pcmMapNo][ff];
                    }
                    else
                    {
                        msgBox.setErrMsg(string.Format(msg.get("E10025"), page.octaveNow, page.noteCmd, page.shift + page.keyShift + page.arpDelta), mml.line.Lp);
                        return;
                    }
                }
                else
                {
                    msgBox.setErrMsg(string.Format(msg.get("E10024"), page.pcmMapNo), mml.line.Lp);
                    return;
                }
            }

            float m = Const.pcmMTbl[page.pcmNote] * (float)Math.Pow(2, (page.pcmOctave - 4));
            page.pcmBaseFreqPerFreq = Information.VGM_SAMPLE_PER_SECOND / ((float)parent.instPCM[page.instrument].freq * m);
            page.pcmFreqCountBuffer = 0.0f;
            long p = parent.instPCM[page.instrument].stAdr;

            long s = parent.instPCM[page.instrument].size;
            long f = parent.instPCM[page.instrument].freq;
            long w = 0;
            if (page.gatetimePmode)
            {
                w = page.waitCounter * page.gatetime / 8L;
            }
            else
            {
                w = page.waitCounter - page.gatetime;
            }
            if (w < 1) w = 1;
            s = Math.Min(s, (long)(w * parent.info.samplesPerClock * f / 44100.0));

            byte[] cmd;
            if (!page.streamSetup)
            {
                parent.newStreamID++;
                page.streamID = parent.newStreamID;
                if (parent.info.format == enmFormat.ZGM)
                {
                    if (parent.ChipCommandSize == 2) cmd = new byte[] { 0x30, 0x00 };
                    else cmd = new byte[] { 0x30 };
                }
                else cmd = new byte[] { 0x90 };
                SOutData(
                    page,
                    mml,
                    // setup stream control
                    cmd
                    , (byte)page.streamID
                    , (byte)(0x1b + (page.chipNumber != 0 ? 0x80 : 0x00)) //0x1b HuC6280
                    , (byte)page.ch
                    , (byte)(0x00 + 0x06)// 0x00 Select Channel 
                                         // set stream data
                    , 0x91
                    , (byte)page.streamID
                    , 0x05 // Data BankID(0x05 HuC6280)
                    , 0x01
                    , 0x00
                    );

                page.streamSetup = true;
            }

            if (page.streamFreq != f)
            {
                if (parent.info.format == enmFormat.ZGM)
                {
                    if (parent.ChipCommandSize == 2) cmd = new byte[] { 0x32, 0x00 };
                    else cmd = new byte[] { 0x32 };
                }
                else cmd = new byte[] { 0x92 };
                //Set Stream Frequency
                SOutData(
                    page,
                    mml,
                    cmd
                    , (byte)page.streamID

                    , (byte)(f & 0xff)
                    , (byte)((f & 0xff00) / 0x100)
                    , (byte)((f & 0xff0000) / 0x10000)
                    , (byte)((f & 0xff000000) / 0x10000)
                    );

                page.streamFreq = f;
            }

            if (parent.info.format == enmFormat.ZGM)
            {
                if (parent.ChipCommandSize == 2) cmd = new byte[] { 0x33, 0x00 };
                else cmd = new byte[] { 0x33 };
            }
            else cmd = new byte[] { 0x93 };
            //Start Stream
            SOutData(
                page,
                mml,
                cmd
                , (byte)page.streamID

                , (byte)(p & 0xff)
                , (byte)((p & 0xff00) / 0x100)
                , (byte)((p & 0xff0000) / 0x10000)
                , (byte)((p & 0xff000000) / 0x10000)

                , 0x01

                , (byte)(s & 0xff)
                , (byte)((s & 0xff00) / 0x100)
                , (byte)((s & 0xff0000) / 0x10000)
                , (byte)((s & 0xff000000) / 0x10000)
                );

            if (parent.instPCM[page.instrument].status != enmPCMSTATUS.ERROR)
            {
                parent.instPCM[page.instrument].status = enmPCMSTATUS.USED;
            }

        }

        public void OutHuC6280KeyOff(MML mml, partPage page)
        {
            if (!page.envelopeMode) page.keyOff = true;
            //SetHuC6280CurrentChannel(mml, page);
            //OutHuC6280Port(page, mml, port[0], 0x4, 0x00);
        }

        public override void SetFNum(partPage page, MML mml)
        {
            if (page.noteCmd == '\0') return;

            int f = GetHuC6280Freq(page.octaveNow, page.noteCmd, page.keyShift + page.shift + page.arpDelta);//

            if (page.bendWaitCounter != -1)
            {
                f = page.bendFnum;
            }
            f = f + page.detune;
            for (int lfo = 0; lfo < 4; lfo++)
            {
                if (!page.lfo[lfo].sw)
                {
                    continue;
                }
                if (page.lfo[lfo].type != eLfoType.Vibrato)
                {
                    continue;
                }
                f += page.lfo[lfo].value + page.lfo[lfo].param[6];
            }

            f = Common.CheckRange(f, 0, 0x0fff);
            page.freq = f;
        }

        public void OutHuC6280FNum(partPage page, MML mml)
        {
            if (page.spg.freq == page.freq) return;

            SetHuC6280CurrentChannel(mml, page);
            if ((page.spg.freq & 0x0ff) != (page.freq & 0x0ff))
                OutHuC6280Port(page, mml, port[0], 2, (byte)(page.freq & 0xff));
            if ((page.spg.freq & 0xf00) != (page.freq & 0xf00))
                OutHuC6280Port(page, mml, port[0], 3, (byte)((page.freq & 0xf00) >> 8));

            page.spg.freq = page.freq;

        }

        public override void SetKeyOn(partPage page, MML mml)
        {
            //OutHuC6280KeyOn(mml, page);
            page.keyOn = true;
            page.keyOff = false;
        }

        public override void SetKeyOff(partPage page, MML mml)
        {
            //OutHuC6280KeyOff(mml, page);
            page.keyOff = true;
        }

        public override void SetVolume(partPage page, MML mml)
        {
            int vol = 0;
            if (page.envelopeMode)
            {
                if (page.envIndex != -1)
                {
                    vol = page.volume;
                }
            }
            else
            {
                //if (pw.ppg[pw.cpgNum].keyOn)//ストリーム処理のbug?
                vol = page.volume;
            }

            if (page.envelopeMode)
            {
                vol = 0;
                if (page.envIndex != -1)
                {
                    vol = page.envVolume - (31 - page.volume);
                }
            }

            for (int lfo = 0; lfo < 4; lfo++)
            {
                if (!page.lfo[lfo].sw)
                {
                    continue;
                }
                if (page.lfo[lfo].type != eLfoType.Tremolo)
                {
                    continue;
                }
                vol += page.lfo[lfo].value + page.lfo[lfo].param[6];
            }

            vol = Common.CheckRange(vol, 0, 31);
            page.beforeVolume = vol;
        }

        public void OutHuC6280Volume(partPage page, MML mml)
        {
            if (page.spg.beforeVolume != page.beforeVolume)
            {
                SetHuC6280Envelope(mml, page, page.beforeVolume);
                page.spg.beforeVolume = page.beforeVolume;
            }
        }

        public override void SetLfoAtKeyOn(partPage page, MML mml)
        {
            for (int lfo = 0; lfo < 4; lfo++)
            {
                clsLfo pl = page.lfo[lfo];
                if (!pl.sw) continue;
                if (pl.type == eLfoType.Hardware) continue;
                if (pl.param[5] != 1) continue;

                pl.isEnd = false;
                pl.value = (pl.param[0] == 0) ? pl.param[6] : 0;//ディレイ中は振幅補正は適用されない
                pl.waitCounter = pl.param[0];
                pl.direction = pl.param[2] < 0 ? -1 : 1;
                pl.depthWaitCounter = pl.param[7];
                pl.depth = pl.param[3];
                pl.depthV2 = pl.param[2];

                if (pl.type == eLfoType.Vibrato)
                {
                    SetFNum(page, mml);
                }
                if (pl.type == eLfoType.Tremolo)
                {
                    page.beforeVolume = -1;
                    SetVolume(page, mml);
                }
            }
        }

        public override int GetToneDoublerShift(partPage page, int octave, char noteCmd, int shift)
        {
            //実装不要
            return 0;
        }

        public override void SetToneDoubler(partPage page, MML mml)
        {
            //実装不要
        }


        public override void CmdNoise(partPage page, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 0, 31);
            if (page.noise != n)
            {
                page.noise = n;
                SetHuC6280CurrentChannel(mml, page);
                OutHuC6280Port(page, mml, port[0], 7, (byte)((page.mixer != 0 ? 0x80 : 0x00) + (page.noise & 0x1f)));
            }
        }

        public override void CmdLfo(partPage page, MML mml)
        {
            base.CmdLfo(page, mml);

            int c = (char)mml.args[0] - 'P';
            if (page.lfo[c].type == eLfoType.Hardware)
            {
                if (page.lfo[c].param.Count < 3)
                {
                    msgBox.setErrMsg(msg.get("E12001"), mml.line.Lp);
                    return;
                }
                if (page.lfo[c].param.Count > 3)
                {
                    msgBox.setErrMsg(msg.get("E12002"), mml.line.Lp);
                    return;
                }

                page.lfo[c].param[0] = Common.CheckRange(page.lfo[c].param[0], 0, 3);//Control(n= 0(Disable),1-3(Ch2波形加算))
                page.lfo[c].param[1] = Common.CheckRange(page.lfo[c].param[1], 0, 255);//Freq(n= 0-255)
                page.lfo[c].param[2] = Common.CheckRange(page.lfo[c].param[2], 0, 4095);//Ch2Freq(n= 0-4095)

            }
        }

        public override void CmdLfoSwitch(partPage page, MML mml)
        {
            base.CmdLfoSwitch(page, mml);

            int c = (char)mml.args[0] - 'P';
            int n = (int)mml.args[1];
            if (page.lfo[c].type == eLfoType.Hardware)
            {
                if (n == 0)
                {
                    OutHuC6280Port(page, mml, port[0], 9, 0); //disable
                }
                else
                {
                    OutHuC6280Port(page, mml, port[0], 9, (byte)page.lfo[c].param[0]);
                    OutHuC6280Port(page, mml, port[0], 8, (byte)page.lfo[c].param[1]);
                    OutHuC6280Port(page, mml, port[0], 0, 1);//CurrentChannel 2
                    CurrentChannel = 1;
                    OutHuC6280Port(page, mml, port[0], 2, (byte)(page.lfo[c].param[2] & 0xff));
                    OutHuC6280Port(page, mml, port[0], 3, (byte)((page.lfo[c].param[2] & 0xf00) >> 8));
                    lstPartWork[1].cpg.freq = page.lfo[c].param[2];
                }
            }
        }

        public override void CmdTotalVolume(partPage page, MML mml)
        {
            int l = (int)mml.args[0];
            int r = (int)mml.args[1];
            l = Common.CheckRange(l, 0, MAXTotalVolume);
            r = Common.CheckRange(r, 0, MAXTotalVolume);
            TotalVolume = (r << 4) | l;

            OutHuC6280Port(
                page,
                mml,
                port[0]
                , 1
                , (byte)TotalVolume
                );
        }

        public override void CmdPan(partPage page, MML mml)
        {
            int l = (int)mml.args[0];
            int r = (int)mml.args[1];

            l = Common.CheckRange(l, 0, 15);
            r = Common.CheckRange(r, 0, 15);
            page.pan = (l << 4) | r;
            //SetHuC6280CurrentChannel(pw);
            SetHuC6280Pan(mml, page, page.pan);
        }

        public override void CmdMode(partPage page, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 0, 1);
            page.pcm = (n == 1);
            page.freq = -1;//freqをリセット
            page.instrument = -1;

            //SetHuC6280CurrentChannel(pw);
            //OutHuC6280Port(pw.ppg[pw.cpgNum].chipNumber, 4, (byte)(0x40 + pw.ppg[pw.cpgNum].volume));
            //for (int i = 0; i < 32; i++) 
            //{
            //    OutHuC6280Port(pw.ppg[pw.cpgNum].chipNumber, 6, 0);
            //}
        }

        public override void CmdY(partPage page, MML mml)
        {
            if (mml.args[0] is string) return;

            byte adr = (byte)(int)mml.args[0];
            byte dat = (byte)(int)mml.args[1];

            OutHuC6280Port(page, mml, port[0], adr, dat);
        }

        public override void CmdLoopExtProc(partPage page, MML mml)
        {
            if (page.chip is HuC6280 && parent.huc6280[page.chipNumber].use)
            {
                parent.huc6280[page.chipNumber].CurrentChannel = 255;
                //setHuC6280CurrentChannel(pw);
                page.beforeFNum = -1;
                page.huc6280Envelope = -1;
                page.huc6280Pan = -1;
            }
        }

        public override void CmdPcmMapSw(partPage page, MML mml)
        {
            bool sw = (bool)mml.args[0];
            page.isPcmMap = sw;
        }

        public override void CmdInstrument(partPage page, MML mml)
        {
            char type = (char)mml.args[0];
            int n = (int)mml.args[1];

            if (type == 'I')
            {
                msgBox.setErrMsg(msg.get("E12003"), mml.line.Lp);
                return;
            }

            if (type == 'T')
            {
                msgBox.setErrMsg(msg.get("E12004"), mml.line.Lp);
                return;
            }

            if (type == 'E')
            {
                n = SetEnvelopParamFromInstrument(page, n, mml);
                return;
            }

            n = Common.CheckRange(n, 0, 255);
            if (!page.pcm)
            {
                if (page.instrument != n)
                {
                    page.instrument = n;
                    ((HuC6280)page.chip).OutHuC6280SetInstrument(page, mml, n);
                }
                return;
            }

            if (page.isPcmMap)
            {
                page.pcmMapNo = n;
                if (!parent.instPCMMap.ContainsKey(n))
                {
                    msgBox.setErrMsg(string.Format(msg.get("E10024"), n), mml.line.Lp);
                }
                return;
            }

            if (page.instrument == n) return;

            if (!parent.instPCM.ContainsKey(n))
            {
                msgBox.setErrMsg(string.Format(msg.get("E12005"), n), mml.line.Lp);
                return;
            }

            if (parent.instPCM[n].chip != enmChipType.HuC6280)
            {
                msgBox.setErrMsg(string.Format(msg.get("E12006"), n), mml.line.Lp);
            }

            page.instrument = n;
            SetDummyData(page, mml);
        }

        public override void CmdNoiseToneMixer(partPage page, MML mml)
        {
            if (page.ch < 4) return;
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 0, 1);
            if (page.mixer != n)
            {
                page.mixer = n;
                SetHuC6280CurrentChannel(mml, page);
                OutHuC6280Port(page, mml, port[0], 7, (byte)((page.mixer != 0 ? 0x80 : 0x00) + (page.noise & 0x1f)));
            }
        }

        public override void SetupPageData(partWork pw, partPage page)
        {

            page.spg.keyOff = true;
            //SetKeyOff(page, null);

            page.spg.instrument = -1;

            //周波数
            page.spg.freq = -1;
            page.spg.beforeFNum = -1;
            
            //音量
            page.spg.beforeVolume = -1;
            SetVolume(page, null);

            //パン
            page.spg.pan = -1;
            SetHuC6280Pan(null, page, page.pan);
        }

        public override void CmdDetune(partPage page, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, -0xfff, 0xfff);
            page.detune = n;
            SetDummyData(page, mml);
        }

        public override void MultiChannelCommand(MML mml)
        {

            foreach (partWork pw in lstPartWork)
            {
                partPage page = pw.cpg;
                CurrentChannel = 255;

                if (page.keyOff)
                {
                    page.keyOff = false;
                    OutHuC6280KeyOff(mml, page);
                }
                if (page.keyOn)
                {
                    page.keyOn = false;
                    OutHuC6280KeyOn(mml, page);
                }

                SetFNum(page, null);
                OutHuC6280FNum(page, mml);

                if (page.keyOff)
                {
                    if (!page.envelopeMode)
                    {
                        page.beforeVolume = 0;
                    }
                }
                OutHuC6280Volume(page, mml);
            }


            //PCMをストリームの機能を使用し再生するため、1Frame毎にカレントチャンネル情報が破壊される。よって次のフレームでリセットできるようにする。
            if (use)
            {
                CurrentChannel = 255;
            }
        }


        public override string DispRegion(clsPcm pcm)
        {
            return string.Format("{0,-10} {1,-7} {2,-5:D3} N/A  ${3,-7:X6} ${4,-7:X6} N/A      ${5,-7:X6}  NONE {6}\r\n"
                , Name
                , pcm.chipNumber != 0 ? "SEC" : "PRI"
                , pcm.num
                , pcm.stAdr & 0xffffff
                , pcm.edAdr & 0xffffff
                , pcm.size
                , pcm.status.ToString()
                );
        }

    }
}
