using musicDriverInterface;
using System;
using System.Collections.Generic;

namespace Core
{
    public class SN76489 : ClsChip
    {

        public const string PSGF_NUM = "PSGF-NUM";
        protected int[][] _FNumTbl = new int[1][] {
            new int[96]
        };
        private int beforePanData = -1;
        private int dcsgCh3Freq = -1;
        private int beforeDcsgCh3Freq = -1;

        public SN76489(ClsVgm parent, int chipID, string initialPartName, string stPath, int chipNumber) : base(parent, chipID, initialPartName, stPath, chipNumber)
        {
            _Name = "SN76489";
            _ShortName = "DCSG";
            _ChMax = 4;
            _canUsePcm = true;
            _canUsePI = false;
            FNumTbl = _FNumTbl;
            port = new byte[][]{
                new byte[] { (byte)(chipNumber!=0 ? 0x30 : 0x50) }
                , new byte[] { (byte)(chipNumber!=0 ? 0x3f : 0x4f) }
            };

            Frequency = 3579545;
            this.ChipNumber = chipNumber;

            Envelope = new Function();
            Envelope.Max = 15;
            Envelope.Min = 0;

            if (string.IsNullOrEmpty(initialPartName)) return;

            Dictionary<string, List<double>> dic = MakeFNumTbl();
            if (dic != null)
            {
                int c = 0;
                foreach (double v in dic["FNUM_00"])
                {
                    FNumTbl[0][c++] = (int)v;
                    if (c == FNumTbl[0].Length) break;
                }
            }

            Ch = new ClsChannel[ChMax];
            SetPartToCh(Ch, initialPartName);
            foreach (ClsChannel ch in Ch)
            {
                ch.Type = enmChannelType.DCSG;
                ch.chipNumber = chipID == 1;
                ch.MaxVolume = 15;
            }
            Ch[3].Type = enmChannelType.DCSGNOISE;

            pcmDataInfo = new clsPcmDataInfo[] { new clsPcmDataInfo() };
            pcmDataInfo[0].totalBufPtr = 0L;
            pcmDataInfo[0].use = false;

            if (parent.info.format == enmFormat.ZGM)
            {
                if (parent.ChipCommandSize == 2)
                {
                    if (chipNumber == 0) pcmDataInfo[0].totalBuf = new byte[] { 0x07, 0x00, 0x66, 0x08, 0x00, 0x00, 0x00, 0x00 };
                    else pcmDataInfo[0].totalBuf = new byte[] { 0x07, 0x00, 0x66, 0x05, 0x00, 0x00, 0x00, 0x00 };
                }
                else
                {
                    if (chipNumber == 0) pcmDataInfo[0].totalBuf = new byte[] { 0x07, 0x66, 0x08, 0x00, 0x00, 0x00, 0x00 };
                    else pcmDataInfo[0].totalBuf = new byte[] { 0x07, 0x66, 0x08, 0x00, 0x00, 0x00, 0x00 };
                }
            }
            else
            {
                if (chipNumber == 0) pcmDataInfo[0].totalBuf = new byte[] { 0x67, 0x66, 0x08, 0x00, 0x00, 0x00, 0x00 };
                else pcmDataInfo[0].totalBuf = new byte[] { 0x67, 0x66, 0x08, 0x00, 0x00, 0x00, 0x00 };
            }

            pcmDataInfo[0].totalHeaderLength = pcmDataInfo[0].totalBuf.Length;
            pcmDataInfo[0].totalHeadrSizeOfDataPtr = (parent.ChipCommandSize == 2) ? 4 : 3;

        }

        public override void InitChip()
        {
            if (!use) return;
            if (ChipID != 0 && parent.info.format != enmFormat.ZGM)
            {
                parent.dat[0x0f] = new outDatum(enmMMLType.unknown, null, null, (byte)(parent.dat[0x0f].val | 0x40));
            }

            OutAllKeyOff();
        }

        public override void InitPart(partWork pw)
        {
            foreach (partPage pg in pw.pg)
            {
                pg.MaxVolume = 15;
                pg.volume = pg.MaxVolume;
                pg.keyOn = false;
                pg.panL = 3;
                pg.port = port;
            }
        }


        public override void StorePcm(Dictionary<int, Tuple<string, clsPcm>> newDic, KeyValuePair<int, clsPcm> v, byte[] buf, bool is16bit, int samplerate, params object[] option)
        {
            clsPcmDataInfo pi = pcmDataInfo[0];

            try
            {
                long size = buf.Length;

                for (int i = 0; i < size; i++)
                {
                    sbyte sb = (sbyte)((int)buf[i] - 0x80);
                    sb >>= 4;//4bit化
                    buf[i] = (byte)(sb + 8);//符号なし
                    buf[i] = (byte)((15 - (buf[i] & 0xf)));//+-反転
                }

                if (newDic.ContainsKey(v.Key))
                {
                    newDic.Remove(v.Key);
                }

                newDic.Add(
                    v.Key
                    , new Tuple<string, clsPcm>("", new clsPcm(
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
                    ));

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
            msgBox.setWrnMsg(msg.get("E12007"), new LinePos(null,"-"));
        }


        public int GetDcsgFNum(int octave, char noteCmd, int shift)
        {
            int o = octave - 1;
            int n = Const.NOTE.IndexOf(noteCmd) + shift;
            o += n / 12;
            o = Common.CheckRange(o, 0, 7);
            n %= 12;

            int f = o * 12 + n;
            if (f < 0) f = 0;
            if (f >= FNumTbl[0].Length) f = FNumTbl[0].Length - 1;

            return FNumTbl[0][f];
        }

        public void OutGGPsgStereoPort(partPage page, MML mml, byte[] cmd, byte data)
        {
            SOutData(
                page,
                mml, cmd
                , data
                );
        }

        public void OutPsgPort(partPage page, MML mml, byte[] cmd, byte data)
        {
            //Console.Write("Ch:{0} ", page.ch);
            //foreach(byte b in cmd) Console.Write("{0:x2} ", b);
            //Console.WriteLine("{0:x2}", data);

            SOutData(
                page,
                mml, cmd
                , data
                );

        }

        public void OutPsgKeyOn(partPage page, MML mml)
        {

            page.keyOff = false;
            if (!page.pcm)
            {
                SetFNum(page, mml);
                //SetVolume(page, mml);
                SetDummyData(page, mml);

                if (mml != null)
                {
                    MML vmml = new MML();
                    vmml.type = enmMMLType.Volume;
                    vmml.args = new List<object>();
                    vmml.args.Add(page.volume);
                    vmml.line = mml.line;
                    SetDummyData(page, vmml);
                }

                return;
            }

            if (parent.info.Version == 1.51f)
            {
                return;
            }

            float m = Const.pcmMTbl[page.pcmNote] * (float)Math.Pow(2, (page.pcmOctave - 4));
            page.pcmBaseFreqPerFreq = Information.VGM_SAMPLE_PER_SECOND / ((float)parent.instPCM[page.instrument].Item2.freq * m);
            page.pcmFreqCountBuffer = 0.0f;

            long p = parent.instPCM[page.instrument].Item2.stAdr;
            long s = parent.instPCM[page.instrument].Item2.size;
            long f = parent.instPCM[page.instrument].Item2.freq;
            long w = 0;
            if (page.gatetimePmode)
            {
                if (!page.gatetimeReverse)
                    w = page.waitCounter * page.gatetime / 8L;
                else
                    w = page.waitCounter - page.waitCounter * page.gatetime / 8L;
            }
            else
            {
                if (!page.gatetimeReverse)
                    w = page.waitCounter - page.gatetime;
                else
                    w = page.gatetime;
            }
            if (w > page.waitCounter) w = page.waitCounter;
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

                int ch = page.ch;
                byte sendCmd = (byte)(0x80 + (ch << 5) + 0x10);//0x00:Frequency 0x10:Volume Write

                SOutData(
                    page,
                    mml,
                    // setup stream control
                    cmd
                    , (byte)page.streamID
                    , (byte)(0x00 + (page.chipNumber != 0 ? 0x80 : 0x00)) //0x00 SN76489/SN76496
                    , 0x00//pp 
                    , sendCmd //cc
                    // set stream data
                    , 0x91
                    , (byte)page.streamID
                    , 0x08 // Data BankID(0x08 SN76489/SN76496)
                    , 0x01 // Step Size
                    , 0x00 // StepBase
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

            if (parent.instPCM[page.instrument].Item2.status != enmPCMSTATUS.ERROR)
            {
                parent.instPCM[page.instrument].Item2.status = enmPCMSTATUS.USED;
            }

            page.freq = 0;//Flip Flop OFF mode
        }

        public void OutPsgKeyOff(partPage page, MML mml)
        {

            if (!page.envelopeMode && !page.varpeggioMode) page.keyOff = true;
            //SetVolume(page, mml);

        }

        public void OutAllKeyOff()
        {

            foreach (partWork pw in lstPartWork)
            {
                foreach (partPage page in pw.pg)
                {
                    page.beforeFNum = -1;
                    page.beforeVolume = -1;

                    page.keyOn = false;
                    OutPsgKeyOff(page, null);
                }
            }
        }


        public override void SetFNum(partPage page, MML mml)
        {
            if (page.Type != enmChannelType.DCSGNOISE)
            {
                int arpNote = page.arpFreqMode ? 0 : page.arpDelta;
                int arpFreq = page.arpFreqMode ? page.arpDelta : 0;

                int f = -page.detune;
                f = f - arpFreq;

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
                    f -= page.lfo[lfo].value + page.lfo[lfo].param[6];//param[6] : 位相
                }

                if (page.octaveNow < 1)
                {
                    f <<= -page.octaveNow;
                }
                else
                {
                    f >>= page.octaveNow - 1;
                }

                if (page.bendWaitCounter != -1)
                {
                    f += page.bendFnum;
                }
                else
                {
                    f += GetDcsgFNum(page.octaveNow, page.noteCmd, page.shift + page.keyShift + arpNote);//
                }

                f = Common.CheckRange(f, 0, 0x3ff);
                page.freq = f;
            }
            else
            {
                int f = 0xe0 + (page.noise & 7);
                page.freq = f;
            }

        }

        private void OutPsgFnum(partPage page, MML mml)
        {
            if (page.spg.freq == page.freq) return;
            if (page.pcm) return;

            page.spg.freq = page.freq;
            if (page.Type != enmChannelType.DCSGNOISE)
            {
                byte data = (byte)(0x80 + (page.ch << 5) + (page.freq & 0xf));
                OutPsgPort(page, mml, port[0], data);

                data = (byte)((page.freq & 0x3f0) >> 4);
                OutPsgPort(page, mml, port[0], data);
            }
            else
            {
                page.spg.noise = page.noise;
                byte data = (byte)page.freq;
                OutPsgPort(page, mml, port[0], data);
            }
        }

        public override int GetFNum(partPage page, MML mml, int octave, char cmd, int shift)
        {
            return GetDcsgFNum(octave, cmd, shift);
        }

        public override void SetVolume(partPage page, MML mml)
        {
            int vol = page.volume;

            //if (!page.keyOff)
            //{
            if (page.envelopeMode)
            {
                vol = 0;
                if (page.envIndex != -1)
                {
                    vol = page.envVolume - (15 - page.volume);
                }
                else
                {
                    page.keyOn = false;
                }
            }

            for (int lfo = 0; lfo < 4; lfo++)
            {
                if (!page.lfo[lfo].sw) continue;
                if (page.lfo[lfo].type != eLfoType.Tremolo) continue;

                vol += page.lfo[lfo].value + page.lfo[lfo].param[6];
            }

            if (page.varpeggioMode)
            {
                vol += page.varpDelta;
            }

            //}
            //else
            //{
            //vol = 0;
            //}

            page.beforeVolume = Common.CheckRange(vol, 0, 15);

        }

        private void OutPsgVolume(partPage page, MML mml)
        {
            if (page.pcm) return;

            if (page.spg.beforeVolume != page.beforeVolume)
            {
                byte data = (byte)(0x80 + (page.ch << 5) + 0x10 + (15 - page.beforeVolume));
                OutPsgPort(page, mml, port[0], data);
                page.spg.beforeVolume = page.beforeVolume;
            }
        }

        public override void SetKeyOn(partPage page, MML mml)
        {
            //OutPsgKeyOn(page, mml);
            page.keyOn = true;
            page.keyOff = false;
        }

        public override void SetKeyOff(partPage page, MML mml)
        {
            //OutPsgKeyOff(page, mml);
            page.keyOff = true;
        }

        public override void SetLfoAtKeyOn(partPage page, MML mml)
        {
            for (int lfo = 0; lfo < 4; lfo++)
            {
                clsLfo pl = page.lfo[lfo];
                if (!pl.sw)
                    continue;
                if (pl.type == eLfoType.Wah) continue;

                if (pl.param[5] != 1)
                    continue;

                pl.isEnd = false;
                pl.value = (pl.param[0] == 0) ? pl.param[6] : 0;//ディレイ中は振幅補正は適用されない
                pl.waitCounter = pl.param[0];
                pl.direction = pl.param[2] < 0 ? -1 : 1;
                pl.depthWaitCounter = pl.param[7];
                pl.depth = pl.param[3];
                pl.depthV2 = pl.param[2];

            }
        }

        public override void SetToneDoubler(partPage page, MML mml)
        {
            //実装不要
        }

        public override int GetToneDoublerShift(partPage page, int octave, char noteCmd, int shift)
        {
            return 0;
        }


        public override void CmdY(partPage page, MML mml)
        {
            if (mml.args[0] is string) return;

            //byte adr = (byte)(int)mml.args[0];
            byte dat = (byte)(int)mml.args[1];

            OutPsgPort(page, mml, port[0], dat);
        }

        public override void CmdNoise(partPage page, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 0, 7);
            page.noise = n;
        }

        public override void CmdDCSGCh3Freq(partPage page, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 0, 0x3ff);
            dcsgCh3Freq = n;
            //OutDCSGCh3Freq(page, mml);
        }

        private void OutDCSGCh3Freq(partPage page, MML mml)
        {
            dcsgCh3Freq = Common.CheckRange(dcsgCh3Freq, -1, 0x3ff);

            if (beforeDcsgCh3Freq != dcsgCh3Freq && dcsgCh3Freq != -1)
            {
                beforeDcsgCh3Freq = dcsgCh3Freq;
                byte data = (byte)(0xc0 + (dcsgCh3Freq & 0xf));
                OutPsgPort(page, mml, port[0], data);

                data = (byte)(dcsgCh3Freq >> 4);
                OutPsgPort(page, mml, port[0], data);
            }
        }

        public override void CmdLoopExtProc(partPage page, MML mml)
        {
        }

        public override void CmdRest(partPage page, MML mml)
        {
            base.CmdRest(page, mml);

            if (page.envelopeMode)
            {
                if (page.envIndex != -1)
                {
                    page.envIndex = 3;
                }
            }
            else
            {
                SetKeyOff(page, mml);
            }

        }

        public override void CmdInstrument(partPage page, MML mml)
        {
            char type = (char)mml.args[0];
            int n = (int)mml.args[1];

            if (type == 'I')
            {
                msgBox.setErrMsg(msg.get("E15001"), mml.line.Lp);
                return;
            }

            if (type == 'T')
            {
                msgBox.setErrMsg(msg.get("E15002"), mml.line.Lp);
                return;
            }

            if (!page.pcm)
            {
                SetEnvelopParamFromInstrument(page, n, mml);
                SetDummyData(page, mml);
                return;
            }

            if (page.instrument == n) return;

            if (!parent.instPCM.ContainsKey(n))
            {
                msgBox.setErrMsg(string.Format(msg.get("E12005"), n), mml.line.Lp);
                return;
            }

            if (parent.instPCM[n].Item2.chip != enmChipType.SN76489)
            {
                msgBox.setErrMsg(string.Format(msg.get("E12006"), n), mml.line.Lp);
            }

            page.instrument = n;
            SetDummyData(page, mml);
        }

        public override void CmdPan(partPage page, MML mml)
        {
            int l = (int)mml.args[0];

            l = Common.CheckRange(l, 0, 3);
            page.panL = l;
        }

        public override void CmdDetune(partPage page, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, -0x3ff, 0x3ff);
            page.detune = n;
            SetDummyData(page, mml);
        }

        public override void CmdMode(partPage page, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 0, 1);
            if (n == 1)
            {
                page.freq = 0;//freqをリセット
                page.spg.freq = -1;
                OutPsgFnum(page, mml);
            }

            page.pcm = (n == 1);
            page.instrument = -1;
            page.spg.beforeVolume = -1;
        }



        public override void SetupPageData(partWork pw, partPage page)
        {
            page.spg.keyOff = true;
            if (page.Type != enmChannelType.DCSGNOISE)
            {
                //周波数
                page.spg.freq = -1;
                //SetFNum(page, null);

            }
            else
            {
                //ノイズモード
                page.spg.freq = -1;
                page.spg.noise = -1;
                //SetFNum(page, null);

                //ノイズ周波数(Ch.3連動モード時のみ復帰させる)
                if ((page.noise & 3) == 3)
                {
                    beforeDcsgCh3Freq = -1;
                    //OutDCSGCh3Freq(page, null);
                }
            }

            //音量
            page.spg.beforeVolume = -1;
            //SetVolume(page, null);

        }

        public override void MultiChannelCommand(MML mml)
        {
            if (!use) return;
            int dat = 0;

            foreach (partWork pw in lstPartWork)
            {
                partPage page = pw.cpg;
                int p = page.panL;
                dat |= (((p & 2) == 0 ? 0x00 : 0x10) | ((p & 1) == 0 ? 0x00 : 0x01)) << page.ch;
            }

            if (beforePanData != dat)
            {
                if (parent.info.format != enmFormat.XGM)//XGMではGGのステレオ機能は対応していない
                {
                    if (parent.info.enableGGStereoDCSG)//enable指定の場合のみ
                    {
                        OutGGPsgStereoPort(lstPartWork[lstPartWork.Count - 1].cpg, mml, port[1], (byte)dat);
                    }
                }
                beforePanData = dat;
            }

            foreach (partWork pw in lstPartWork)
            {
                partPage page = pw.cpg;


                if (page.keyOff)
                {
                    page.keyOff = false;
                    OutPsgKeyOff(page, mml);
                }
                if (page.keyOn)
                {
                    page.keyOn = false;
                    OutPsgKeyOn(page, mml);
                }


                OutPsgFnum(page, mml);


                if (page.Type == enmChannelType.DCSGNOISE)
                {
                    OutDCSGCh3Freq(page, mml);
                }


                if (page.keyOff)
                {
                    if (!page.envelopeMode)
                    {
                        page.beforeVolume = 0;
                    }
                }
                OutPsgVolume(page, mml);
            }
        }

    }
}