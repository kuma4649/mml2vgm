using System;
using System.Collections.Generic;

namespace Core
{
    public class HuC6280 : ClsChip
    {
        public byte CurrentChannel = 0xff;
        public int TotalVolume = 15;
        public int MAXTotalVolume = 15;

        public HuC6280(ClsVgm parent, int chipID, string initialPartName, string stPath, int isSecondary) : base(parent, chipID, initialPartName, stPath, isSecondary)
        {
            _chipType = enmChipType.HuC6280;
            _Name = "HuC6280";
            _ShortName = "HuC8";
            _ChMax = 6;
            _canUsePcm = true;
            _canUsePI = false;
            IsSecondary = isSecondary;

            Frequency = 3579545;
            port = new byte[][] { new byte[] { 0xb9 } };

            if (string.IsNullOrEmpty(initialPartName)) return;

            Ch = new ClsChannel[ChMax];
            SetPartToCh(Ch, initialPartName);
            foreach (ClsChannel ch in Ch)
            {
                ch.Type = enmChannelType.WaveForm;
                ch.isSecondary = chipID == 1;
                ch.MaxVolume = 15;
            }

            pcmDataInfo = new clsPcmDataInfo[] { new clsPcmDataInfo() };
            pcmDataInfo[0].totalBufPtr = 0L;
            pcmDataInfo[0].use = false;

            if (parent.info.format == enmFormat.ZGM)
            {
                if (parent.ChipCommandSize == 2)
                {
                    if (isSecondary==0) pcmDataInfo[0].totalBuf = new byte[] { 0x07, 0x00, 0x66, 0x05, 0x00, 0x00, 0x00, 0x00 };
                    else pcmDataInfo[0].totalBuf = new byte[] { 0x07, 0x00, 0x66, 0x05, 0x00, 0x00, 0x00, 0x00 };
                }
                else
                {
                    if (isSecondary==0) pcmDataInfo[0].totalBuf = new byte[] { 0x07, 0x66, 0x05, 0x00, 0x00, 0x00, 0x00 };
                    else pcmDataInfo[0].totalBuf = new byte[] { 0x07, 0x66, 0x05, 0x00, 0x00, 0x00, 0x00 };
                }
            }
            else
            {
                if (isSecondary==0) pcmDataInfo[0].totalBuf = new byte[] { 0x67, 0x66, 0x05, 0x00, 0x00, 0x00, 0x00 };
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
            OutHuC6280Port(null, port[0], 1, 0xff);
            //LFO freq 0
            OutHuC6280Port(null, port[0], 8, 0);
            //LFO ctrl 0
            OutHuC6280Port(null, port[0], 9, 0);

            SupportReversePartWork = true;

            foreach (partWork pw in lstPartWork)
            {
                SetHuC6280CurrentChannel(null, pw);

                pw.port = port;// 0xb9;

                //freq( 0 )
                pw.freq = 0;
                OutHuC6280Port(null, port[0], 2, 0);
                OutHuC6280Port(null, port[0], 3, 0);

                pw.pcm = false;

                //volume
                byte data = (byte)(0x00 + (0 & 0x1f));
                OutHuC6280Port(null, port[0], 4, data);

                //pan
                pw.panL = 0;
                pw.panR = 0;
                OutHuC6280Port(null, port[0], 5, 0xff);

                for (int j = 0; j < 32; j++)
                {
                    OutHuC6280Port(null, port[0], 6, 0);
                }

                if (pw.ch > 3)
                {
                    //noise(Ch5,6 only)
                    pw.noise = 0x1f;
                    OutHuC6280Port(null, port[0], 7, 0x1f);
                }
            }
        }

        public override void InitPart(partWork pw)
        {
            pw.MaxVolume = 31;
            pw.volume = pw.MaxVolume;
            pw.mixer = 0;
            pw.noise = 0;
            pw.port = port;
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
                    , IsSecondary!=0);
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

        public override void MultiChannelCommand(MML mml)
        {
            //PCMをストリームの機能を使用し再生するため、1Frame毎にカレントチャンネル情報が破壊される。よって次のフレームでリセットできるようにする。
            if (!use) return;
            CurrentChannel = 255;
        }

        public override int GetFNum(partWork pw, MML mml, int octave, char cmd, int shift)
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

        public void SetHuC6280Envelope(MML mml, partWork pw, int volume)
        {
            if (pw.huc6280Envelope != volume)
            {
                SetHuC6280CurrentChannel(mml, pw);
                if (!pw.keyOn) volume = 0;
                byte data = (byte)((volume != 0 ? 0x80 : 0) + (volume & 0x1f));
                OutHuC6280Port(mml, port[0], 4, data);
                pw.huc6280Envelope = volume;
            }
        }

        public void SetHuC6280CurrentChannel(MML mml, partWork pw)
        {
            byte pch = (byte)pw.ch;
            int isSecondary = pw.isSecondary;

            if (CurrentChannel != pch)
            {
                byte data = (byte)(pch & 0x7);
                OutHuC6280Port(mml, port[0], 0x0, data);
                CurrentChannel = pch;
            }
        }

        public void SetHuC6280Pan(MML mml, partWork pw, int pan)
        {
            if (pw.huc6280Pan != pan)
            {
                SetHuC6280CurrentChannel(mml, pw);
                byte data = (byte)(pan & 0xff);
                OutHuC6280Port(mml, port[0], 0x5, data);
                pw.huc6280Pan = pan;
            }
        }

        public void OutHuC6280Port(MML mml, byte[] cmd, byte adr, byte data)
        {
            parent.OutData(
                mml,
                cmd
                , (byte)((IsSecondary!=0 ? 0x80 : 0x00) + adr)
                , data);
        }

        public void OutHuC6280SetInstrument(partWork pw, MML mml, int n)
        {

            if (!parent.instWF.ContainsKey(n))
            {
                msgBox.setWrnMsg(string.Format(msg.get("E12000"), n), mml.line.Lp);
                return;
            }

            SetHuC6280CurrentChannel(mml, pw);
            OutHuC6280Port(mml, port[0], 4, (byte)(0x40 + pw.volume)); //WaveIndexReset(=0x40)

            for (int i = 1; i < parent.instWF[n].Length; i++) // 0 は音色番号が入っている為1からスタート
            {
                OutHuC6280Port(mml, port[0], 6, (byte)(parent.instWF[n][i] & 0x1f));
            }

        }

        public void OutHuC6280KeyOn(MML mml, partWork pw)
        {
            int vol = pw.volume;
            if (pw.envelopeMode)
            {
                vol = 0;
                if (pw.envIndex != -1)
                {
                    vol = pw.envVolume - (31 - pw.volume);
                }
            }
            if (vol > 31) vol = 31;
            if (vol < 0) vol = 0;
            byte data = (byte)(((vol > 0) ? 0x80 : 0x00) + vol);

            if (!pw.pcm)
            {
                SetHuC6280CurrentChannel(mml, pw);
                OutHuC6280Port(mml, port[0], 0x4, data);
                OutHuC6280Port(mml, port[0], 0x5, (byte)pw.huc6280Pan);
                return;
            }

            if (parent.info.Version == 1.51f)
            {
                return;
            }

            SetHuC6280CurrentChannel(mml, pw);
            data |= 0x40;
            OutHuC6280Port(mml, port[0], 0x4, data);
            OutHuC6280Port(mml, port[0], 0x5, (byte)pw.huc6280Pan);

            if (pw.isPcmMap)
            {
                int nt = Const.NOTE.IndexOf(pw.noteCmd);
                int ff = pw.octaveNow * 12 + nt + pw.shift + pw.keyShift;
                if (parent.instPCMMap.ContainsKey(pw.pcmMapNo))
                {
                    if (parent.instPCMMap[pw.pcmMapNo].ContainsKey(ff))
                    {
                        pw.instrument = parent.instPCMMap[pw.pcmMapNo][ff];
                    }
                    else
                    {
                        msgBox.setErrMsg(string.Format(msg.get("E10025"), pw.octaveNow, pw.noteCmd, pw.shift + pw.keyShift), mml.line.Lp);
                        return;
                    }
                }
                else
                {
                    msgBox.setErrMsg(string.Format(msg.get("E10024"), pw.pcmMapNo), mml.line.Lp);
                    return;
                }
            }

            float m = Const.pcmMTbl[pw.pcmNote] * (float)Math.Pow(2, (pw.pcmOctave - 4));
            pw.pcmBaseFreqPerFreq = Information.VGM_SAMPLE_PER_SECOND / ((float)parent.instPCM[pw.instrument].freq * m);
            pw.pcmFreqCountBuffer = 0.0f;
            long p = parent.instPCM[pw.instrument].stAdr;

            long s = parent.instPCM[pw.instrument].size;
            long f = parent.instPCM[pw.instrument].freq;
            long w = 0;
            if (pw.gatetimePmode)
            {
                w = pw.waitCounter * pw.gatetime / 8L;
            }
            else
            {
                w = pw.waitCounter - pw.gatetime;
            }
            if (w < 1) w = 1;
            s = Math.Min(s, (long)(w * parent.info.samplesPerClock * f / 44100.0));

            byte[] cmd;
            if (!pw.streamSetup)
            {
                parent.newStreamID++;
                pw.streamID = parent.newStreamID;
                if (parent.info.format == enmFormat.ZGM)
                {
                    if (parent.ChipCommandSize == 2) cmd = new byte[] { 0x30, 0x00 };
                    else cmd = new byte[] { 0x30 };
                }
                else cmd = new byte[] { 0x90 };
                parent.OutData(
                    mml,
                    // setup stream control
                    cmd
                    , (byte)pw.streamID
                    , (byte)(0x1b + (pw.isSecondary!=0 ? 0x80 : 0x00)) //0x1b HuC6280
                    , (byte)pw.ch
                    , (byte)(0x00 + 0x06)// 0x00 Select Channel 
                                         // set stream data
                    , 0x91
                    , (byte)pw.streamID
                    , 0x05 // Data BankID(0x05 HuC6280)
                    , 0x01
                    , 0x00
                    );

                pw.streamSetup = true;
            }

            if (pw.streamFreq != f)
            {
                if (parent.info.format == enmFormat.ZGM)
                {
                    if (parent.ChipCommandSize == 2) cmd = new byte[] { 0x32, 0x00 };
                    else cmd = new byte[] { 0x32 };
                }
                else cmd = new byte[] { 0x92 };
                //Set Stream Frequency
                parent.OutData(
                    mml,
                    cmd
                    , (byte)pw.streamID

                    , (byte)(f & 0xff)
                    , (byte)((f & 0xff00) / 0x100)
                    , (byte)((f & 0xff0000) / 0x10000)
                    , (byte)((f & 0xff000000) / 0x10000)
                    );

                pw.streamFreq = f;
            }

            if (parent.info.format == enmFormat.ZGM)
            {
                if (parent.ChipCommandSize == 2) cmd = new byte[] { 0x33, 0x00 };
                else cmd = new byte[] { 0x33 };
            }
            else cmd = new byte[] { 0x93 };
            //Start Stream
            parent.OutData(
                mml,
                cmd
                , (byte)pw.streamID

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

            if (parent.instPCM[pw.instrument].status != enmPCMSTATUS.ERROR)
            {
                parent.instPCM[pw.instrument].status = enmPCMSTATUS.USED;
            }

        }

        public void OutHuC6280KeyOff(MML mml, partWork pw)
        {
            SetHuC6280CurrentChannel(mml, pw);

            OutHuC6280Port(mml, port[0], 0x4, 0x00);
            //OutHuC6280Port(pw.isSecondary, 0x5, 0);
        }

        public override void SetFNum(partWork pw, MML mml)
        {
            int f = GetHuC6280Freq(pw.octaveNow, pw.noteCmd, pw.keyShift + pw.shift);//

            if (pw.bendWaitCounter != -1)
            {
                f = pw.bendFnum;
            }
            f = f + pw.detune;
            for (int lfo = 0; lfo < 4; lfo++)
            {
                if (!pw.lfo[lfo].sw)
                {
                    continue;
                }
                if (pw.lfo[lfo].type != eLfoType.Vibrato)
                {
                    continue;
                }
                f += pw.lfo[lfo].value + pw.lfo[lfo].param[6];
            }

            f = Common.CheckRange(f, 0, 0x0fff);

            if (pw.freq == f) return;

            SetHuC6280CurrentChannel(mml, pw);
            if ((pw.freq & 0x0ff) != (f & 0x0ff)) OutHuC6280Port(mml, port[0], 2, (byte)(f & 0xff));
            if ((pw.freq & 0xf00) != (f & 0xf00)) OutHuC6280Port(mml, port[0], 3, (byte)((f & 0xf00) >> 8));
            //OutHuC6280Port(pw.isSecondary, 2, (byte)(f & 0xff));
            //OutHuC6280Port(pw.isSecondary, 3, (byte)((f & 0xf00) >> 8));

            pw.freq = f;

        }

        public override void SetKeyOn(partWork pw, MML mml)
        {
            OutHuC6280KeyOn(mml, pw);
            pw.keyOn = true;
        }

        public override void SetKeyOff(partWork pw, MML mml)
        {
            OutHuC6280KeyOff(mml, pw);
            pw.keyOn = false;
        }

        public override void SetVolume(partWork pw, MML mml)
        {
            int vol = 0;
            if (pw.envelopeMode)
            {
                if (pw.envIndex != -1)
                {
                    vol = pw.volume;
                }
            }
            else
            {
                //if (pw.keyOn)//ストリーム処理のbug?
                vol = pw.volume;
            }

            if (pw.envelopeMode)
            {
                vol = 0;
                if (pw.envIndex != -1)
                {
                    vol = pw.envVolume - (31 - pw.volume);
                }
            }

            for (int lfo = 0; lfo < 4; lfo++)
            {
                if (!pw.lfo[lfo].sw)
                {
                    continue;
                }
                if (pw.lfo[lfo].type != eLfoType.Tremolo)
                {
                    continue;
                }
                vol += pw.lfo[lfo].value + pw.lfo[lfo].param[6];
            }

            vol = Common.CheckRange(vol, 0, 31);
            if (pw.beforeVolume != vol)
            {
                SetHuC6280Envelope(mml, pw, vol);
                pw.beforeVolume = vol;
            }
        }

        public override void SetLfoAtKeyOn(partWork pw, MML mml)
        {
            for (int lfo = 0; lfo < 4; lfo++)
            {
                clsLfo pl = pw.lfo[lfo];
                if (!pl.sw) continue;
                if (pl.type == eLfoType.Hardware) continue;
                if (pl.param[5] != 1) continue;

                pl.isEnd = false;
                pl.value = (pl.param[0] == 0) ? pl.param[6] : 0;//ディレイ中は振幅補正は適用されない
                pl.waitCounter = pl.param[0];
                pl.direction = pl.param[2] < 0 ? -1 : 1;

                if (pl.type == eLfoType.Vibrato)
                {
                    SetFNum(pw, mml);
                }
                if (pl.type == eLfoType.Tremolo)
                {
                    pw.beforeVolume = -1;
                    SetVolume(pw, mml);
                }
            }
        }

        public override int GetToneDoublerShift(partWork pw, int octave, char noteCmd, int shift)
        {
            //実装不要
            return 0;
        }

        public override void SetToneDoubler(partWork pw, MML mml)
        {
            //実装不要
        }


        public override void CmdNoise(partWork pw, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 0, 31);
            if (pw.noise != n)
            {
                pw.noise = n;
                SetHuC6280CurrentChannel(mml, pw);
                OutHuC6280Port(mml, port[0], 7, (byte)((pw.mixer != 0 ? 0x80 : 0x00) + (pw.noise & 0x1f)));
            }
        }

        public override void CmdLfo(partWork pw, MML mml)
        {
            base.CmdLfo(pw, mml);

            int c = (char)mml.args[0] - 'P';
            if (pw.lfo[c].type == eLfoType.Hardware)
            {
                if (pw.lfo[c].param.Count < 3)
                {
                    msgBox.setErrMsg(msg.get("E12001"), mml.line.Lp);
                    return;
                }
                if (pw.lfo[c].param.Count > 3)
                {
                    msgBox.setErrMsg(msg.get("E12002"), mml.line.Lp);
                    return;
                }

                pw.lfo[c].param[0] = Common.CheckRange(pw.lfo[c].param[0], 0, 3);//Control(n= 0(Disable),1-3(Ch2波形加算))
                pw.lfo[c].param[1] = Common.CheckRange(pw.lfo[c].param[1], 0, 255);//Freq(n= 0-255)
                pw.lfo[c].param[2] = Common.CheckRange(pw.lfo[c].param[2], 0, 4095);//Ch2Freq(n= 0-4095)

            }
        }

        public override void CmdLfoSwitch(partWork pw, MML mml)
        {
            base.CmdLfoSwitch(pw, mml);

            int c = (char)mml.args[0] - 'P';
            int n = (int)mml.args[1];
            if (pw.lfo[c].type == eLfoType.Hardware)
            {
                if (n == 0)
                {
                    OutHuC6280Port(mml, port[0], 9, 0); //disable
                }
                else
                {
                    OutHuC6280Port(mml, port[0], 9, (byte)pw.lfo[c].param[0]);
                    OutHuC6280Port(mml, port[0], 8, (byte)pw.lfo[c].param[1]);
                    OutHuC6280Port(mml, port[0], 0, 1);//CurrentChannel 2
                    CurrentChannel = 1;
                    OutHuC6280Port(mml, port[0], 2, (byte)(pw.lfo[c].param[2] & 0xff));
                    OutHuC6280Port(mml, port[0], 3, (byte)((pw.lfo[c].param[2] & 0xf00) >> 8));
                    lstPartWork[1].freq = pw.lfo[c].param[2];
                }
            }
        }

        public override void CmdTotalVolume(partWork pw, MML mml)
        {
            int l = (int)mml.args[0];
            int r = (int)mml.args[1];
            l = Common.CheckRange(l, 0, MAXTotalVolume);
            r = Common.CheckRange(r, 0, MAXTotalVolume);
            TotalVolume = (r << 4) | l;

            OutHuC6280Port(
                mml,
                port[0]
                , 1
                , (byte)TotalVolume
                );
        }

        public override void CmdPan(partWork pw, MML mml)
        {
            int l = (int)mml.args[0];
            int r = (int)mml.args[1];

            l = Common.CheckRange(l, 0, 15);
            r = Common.CheckRange(r, 0, 15);
            pw.pan.val = (l << 4) | r;
            //SetHuC6280CurrentChannel(pw);
            SetHuC6280Pan(mml, pw, (int)pw.pan.val);
        }

        public override void CmdMode(partWork pw, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 0, 1);
            pw.pcm = (n == 1);
            pw.freq = -1;//freqをリセット
            pw.instrument = -1;

            //SetHuC6280CurrentChannel(pw);
            //OutHuC6280Port(pw.isSecondary, 4, (byte)(0x40 + pw.volume));
            //for (int i = 0; i < 32; i++) 
            //{
            //    OutHuC6280Port(pw.isSecondary, 6, 0);
            //}
        }

        public override void CmdY(partWork pw, MML mml)
        {
            if (mml.args[0] is string) return;

            byte adr = (byte)mml.args[0];
            byte dat = (byte)mml.args[1];

            OutHuC6280Port(mml, port[0], adr, dat);
        }

        public override void CmdLoopExtProc(partWork p, MML mml)
        {
            if (p.chip is HuC6280 && parent.huc6280[p.isSecondary].use)
            {
                parent.huc6280[p.isSecondary].CurrentChannel = 255;
                //setHuC6280CurrentChannel(pw);
                p.beforeFNum = -1;
                p.huc6280Envelope = -1;
                p.huc6280Pan = -1;
            }
        }

        public override void CmdPcmMapSw(partWork pw, MML mml)
        {
            bool sw = (bool)mml.args[0];
            pw.isPcmMap = sw;
        }

        public override void CmdInstrument(partWork pw, MML mml)
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
                n = SetEnvelopParamFromInstrument(pw, n, mml);
                return;
            }

            n = Common.CheckRange(n, 0, 255);
            if (!pw.pcm)
            {
                if (pw.instrument != n)
                {
                    pw.instrument = n;
                    ((HuC6280)pw.chip).OutHuC6280SetInstrument(pw, mml, n);
                }
                return;
            }

            if (pw.isPcmMap)
            {
                pw.pcmMapNo = n;
                if (!parent.instPCMMap.ContainsKey(n))
                {
                    msgBox.setErrMsg(string.Format(msg.get("E10024"), n), mml.line.Lp);
                }
                return;
            }

            if (pw.instrument == n) return;

            if (!parent.instPCM.ContainsKey(n))
            {
                msgBox.setErrMsg(string.Format(msg.get("E12005"), n), mml.line.Lp);
                return;
            }

            if (parent.instPCM[n].chip != enmChipType.HuC6280)
            {
                msgBox.setErrMsg(string.Format(msg.get("E12006"), n), mml.line.Lp);
            }

            pw.instrument = n;
            SetDummyData(pw, mml);
        }

        public override void CmdNoiseToneMixer(partWork pw, MML mml)
        {
            if (pw.ch < 4) return;
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 0, 1);
            if (pw.mixer != n)
            {
                pw.mixer = n;
                SetHuC6280CurrentChannel(mml, pw);
                OutHuC6280Port(mml, port[0], 7, (byte)((pw.mixer != 0 ? 0x80 : 0x00) + (pw.noise & 0x1f)));
            }
        }

        public override string DispRegion(clsPcm pcm)
        {
            return string.Format("{0,-10} {1,-7} {2,-5:D3} N/A  ${3,-7:X6} ${4,-7:X6} N/A      ${5,-7:X6}  NONE {6}\r\n"
                , Name
                , pcm.isSecondary!=0 ? "SEC" : "PRI"
                , pcm.num
                , pcm.stAdr & 0xffffff
                , pcm.edAdr & 0xffffff
                , pcm.size
                , pcm.status.ToString()
                );
        }

    }
}
