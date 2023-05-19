using MDSound;
using musicDriverInterface;
using System;
using System.Collections.Generic;
using System.Drawing;
using static MDSound.mpcmX68k;

namespace Core
{
    public class ClsOPN : ClsChip
    {
        private byte[] nSSGKeyOn = new byte[] { 0, 0, 0, 0 };
        public byte[] SSGKeyOn = new byte[] { 0x3f, 0x3f, 0x3f, 0x3f };
        public int noiseFreq = -1;
        public int[] hsFnumTbl = null;
        private int[] EmitTable;

        public ClsOPN(ClsVgm parent, int chipID, string initialPartName, string stPath, int chipNumber) : base(parent, chipID, initialPartName, stPath, chipNumber)
        {
            //from fmgen code
            EmitTable = new int[16];
            double Base = 0x4000 / 3.0 * Math.Pow(10.0, 0.0 / 40.0);
            for (int i = 15; i >= 1; i--)
            {
                EmitTable[i] = (int)(Base);
                Base /= 1.189207115;
                Base /= 1.189207115;
            }
            EmitTable[0] = 0;

            for (int i = 0; i < 16; i++)
            {
                EmitTable[i] = (int)(EmitTable[i] / (EmitTable[15] / 255.0));
            }

            octaveMin = -1;
            octaveMax = 9;
        }

        protected byte[] SSGPCM_Encode(byte[] buf, bool is16bit)
        {

            long size = buf.Length;

            for (int i = 0; i < size; i++)
            {
                int dis = int.MaxValue;
                int n = 0;
                for (int j = 0; j < 16; j++)
                {
                    int ndis = Math.Abs(buf[i] - EmitTable[j]);
                    if (ndis < dis)
                    {
                        dis = ndis;
                        n = j;
                    }
                }
                buf[i] = (byte)n;
            }

            return buf;
        }


        public void OutSsgKeyOn(partPage page, MML mml)
        {
            if (page.pcm)
            {
                OutSsgPCMKeyOn(page, mml);
                return;
            }

            page.keyOn = true;

            int port;
            int adr;
            int vch;
            GetPortVchSsg(page, out port, out adr, out vch);
            int p = port;
            if (adr == 0x10)
                p++;

            //int n = (page.mixer & 0x1) + ((page.mixer & 0x2) << 2);
            //byte data = 0;

            //data = (byte)(((ClsOPN)page.chip).SSGKeyOn[p] | (9 << vch));
            //data &= (byte)(~(n << vch));
            //((ClsOPN)page.chip).SSGKeyOn[p] = data;

            SetSsgVolume(page, mml, page.phaseReset);
            if (page.HardEnvelopeSw || page.hardEnvelopeSync.sw)
            {
                if (page.hardEnvelopeSync.sw)
                {
                    SOutData(page, mml, page.port[port], (byte)(adr + 0x08 + vch), 0x10);
                }

                SOutData(page, mml, page.port[port], (byte)(adr + 0x0d), (byte)(page.HardEnvelopeType & 0xf));
            }
            //SOutData(page, mml, page.port[port], (byte)(adr + 0x07), data);

            if (mml != null)
            {
                MML vmml = new MML();
                vmml.type = enmMMLType.Volume;
                vmml.args = new List<object>();
                vmml.args.Add(page.volume);
                vmml.line = mml.line;
                SetDummyData(page, vmml);
            }
        }

        public void OutSsgKeyOff(MML mml, partPage page)
        {
            if (page.pcm)
            {
                OutSsgPCMKeyOff(page, mml);
                return;
            }

            page.keyOn = false;

            int port;
            int adr;
            int vch;
            GetPortVchSsg(page, out port, out adr, out vch);
            int p = port;
            if (adr == 0x10)
                p++;

            if (page.hardEnvelopeSync.sw)
            {
                SOutData(page, mml, page.port[port], (byte)(adr + 0x08 + vch), 0);
            }

            //int n = 9;
            //byte data = 0;

            //data = (byte)(((ClsOPN)page.chip).SSGKeyOn[p] | (n << vch));
            //((ClsOPN)page.chip).SSGKeyOn[p] = data;

            //SOutData(page, mml, page.port[port], (byte)(adr + 0x08 + vch), 0);
            //page.beforeVolume = -1;
            //SOutData(page, mml, page.port[port], (byte)(adr + 0x07), data);

        }


        private void OutSsgPCMKeyOn(partPage page, MML mml)
        {
            if (parent.info.Version == 1.51f)
            {
                return;
            }

            float m = Const.pcmMTbl[page.pcmNote] * (float)Math.Pow(2, (page.pcmOctave - 4));
            page.pcmBaseFreqPerFreq = Information.VGM_SAMPLE_PER_SECOND / ((float)parent.instPCM[page.instrument].Item2.freq * m);
            page.pcmFreqCountBuffer = 0.0f;

            ssgpcmStreamSetup(page, mml);
            SetSsgPCMFNum(page, mml);
            ssgpcmStreamStart(page, mml);

            if (parent.instPCM[page.instrument].Item2.status != enmPCMSTATUS.ERROR)
            {
                parent.instPCM[page.instrument].Item2.status = enmPCMSTATUS.USED;
            }

            page.freq = 0;//Flip Flop OFF mode

        }

        private void ssgpcmStreamStart(partPage page, MML mml)
        {
            byte[] cmd;
            if (parent.info.format == enmFormat.ZGM)
            {
                if (parent.ChipCommandSize == 2) cmd = new byte[] { 0x33, 0x00 };
                else cmd = new byte[] { 0x33 };
            }
            else cmd = new byte[] { 0x93 };

            long p = parent.instPCM[page.instrument].Item2.stAdr;
            long s = parent.instPCM[page.instrument].Item2.size;
            long w;
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
            s = Math.Min(s, (long)(w * parent.info.samplesPerClock * page.streamFreq / 44100.0));

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
        }

        private void ssgpcmStreamSetup(partPage page, MML mml)
        {
            byte[] cmd;
            if (page.streamSetup) return;

            parent.newStreamID++;
            page.streamID = parent.newStreamID;
            if (parent.info.format == enmFormat.ZGM)
            {
                if (parent.ChipCommandSize == 2) cmd = new byte[] { 0x30, 0x00 };
                else cmd = new byte[] { 0x30 };
            }
            else cmd = new byte[] { 0x90 };

            int port;
            int adr;
            int vch;
            GetPortVchSsg(page, out port, out adr, out vch);
            if (adr == 0x10)
                port++;

            int ch = vch;
            byte sendCmd = (byte)(0x08 + ch);//0x08:volume

            byte cc = 0;
            if (this is YM2203) cc = 0x06;
            else if (this is YM2608) cc = 0x07;
            else if (this is YM2610B) cc = 0x08;

            SOutData(
                page,
                mml,
                // setup stream control
                cmd
                , (byte)page.streamID
                , (byte)(cc + (page.chipNumber != 0 ? 0x80 : 0x00))
                , 0x00//pp 
                , sendCmd //cc
                          // set stream data
                , 0x91
                , (byte)page.streamID
                , DataBankID // Data BankID
                , 0x01 // Step Size
                , 0x00 // StepBase
                );

            // vgmplay等他プレーヤーでの再生時ノイズレジスタ初期値対策。
            // ノイズとSSGPCMの使い方組み合わせ次第では不十分な設定、要適切な変更
            // by きゃどん(2023/05/19 Thanks)
            //SOutData(page, mml, page.port[port], (byte)(adr + 0x07), 0x38); //All Noise Off

            page.streamSetup = true;
        }

        private void OutSsgPCMKeyOff(partPage page, MML mml)
        {
            byte[] cmd;

            //Stop Stream
            if (parent.info.format == enmFormat.ZGM)
            {
                if (parent.ChipCommandSize == 2) cmd = new byte[] { 0x34, 0x00 };
                else cmd = new byte[] { 0x34 };
            }
            else cmd = new byte[] { 0x94 };
            SOutData(
                page,
                mml,
                cmd
                , (byte)page.streamID
                );
        }

        public virtual void SetSsgVolume(partPage page, MML mml,bool phaseReset=false)
        {
            int port;
            int adr;
            int vch;
            GetPortVchSsg(page, out port, out adr, out vch);
            int p = port;
            if (adr == 0x10)
                p++;

            int vol = page.volume;
            if (page.envelopeMode)
            {
                vol = 0;
                if (page.envIndex != -1)
                {
                    vol = page.volume - (15 - page.envVolume);
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

            vol = Common.CheckRange(vol, 0, 15) 
                + (
                    (page.HardEnvelopeSw || page.hardEnvelopeSync.sw) ? 0x10 : 0x00
                );

            //if (!page.keyOn)// (((ClsOPN)page.chip).SSGKeyOn[p] & (9 << vch)) == (9 << vch))
            //{
            //    vol = 0;
            //}

            if (page.chip is YM2609)
            {
                int pan = page.pan & 3;
                vol |= (byte)(pan << 6);
                if (phaseReset) vol |= 0x20;
            }

            if (page.spg.beforeVolume != vol)
            {
                SOutData(page, mml, page.port[port], (byte)(adr + 0x08 + vch), (byte)vol);
                page.spg.beforeVolume = vol;
            }
        }

        public void OutSsgNoise(MML mml, partPage page)
        {
            int port;
            int adr;
            int vch;//ノイズ設定はch未使用
            GetPortVchSsg(page, out port, out adr, out vch);

            if (page.noiseFreq != page.noise)
            {
                page.noiseFreq = page.noise;
                if (!(page.chip is YM2609))
                    SOutData(page, mml, page.port[port], (byte)(adr + 0x06), (byte)(page.noise & 0x1f));
                else
                    SOutData(page, mml, page.port[port], (byte)(adr + 0x06), (byte)(page.noise & 0x3f));
            }
        }

        public void OutSsgHardEnvType(partPage page, MML mml)
        {
            if (page.spg.HardEnvelopeType != page.HardEnvelopeType)
            {
                SOutData(page, mml, port[0], 0x0d, (byte)(page.HardEnvelopeType & 0xf));
                page.spg.HardEnvelopeType = page.HardEnvelopeType;
            }
        }

        public void OutSsgHardEnvSpeed(partPage page, MML mml)
        {
            if (page.spg.HardEnvelopeSpeed != page.HardEnvelopeSpeed)
            {
                SOutData(page, mml, port[0], 0x0b, (byte)(page.HardEnvelopeSpeed & 0xff));
                SOutData(page, mml, port[0], 0x0c, (byte)((page.HardEnvelopeSpeed >> 8) & 0xff));
                page.spg.HardEnvelopeSpeed = page.HardEnvelopeSpeed;
            }
        }


        public void SetSsgFNum(partPage page, MML mml)
        {
            int arpNote = page.arpFreqMode ? 0 : page.arpDelta;
            int arpFreq = page.arpFreqMode ? page.arpDelta : 0;

            int f = 0;
            if (!page.pcm)
            {
                f = -page.detune;
                f = f + arpFreq;
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
                    f -= page.lfo[lfo].value + page.lfo[lfo].param[6];
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
                    f += GetSsgFNum(page, mml, page.octaveNow, page.noteCmd, page.shift + page.keyShift + arpNote, page.pitchShift);//
                }

                f = Common.CheckRange(f, 0, 0xfff);
            }
            else
            {
                ssgpcmStreamSetup(page, mml);
                SetSsgPCMFNum(page, mml);
            }

            if (page.spg.freq == f) return;

            page.freq = f;
            page.spg.freq = f;

            int port;
            int adr;
            int vch;
            GetPortVchSsg(page, out port, out adr, out vch);

            byte data = 0;

            data = (byte)(f & 0xff);
            SOutData(page, mml, page.port[port], (byte)(adr + 0 + vch * 2), data);

            data = (byte)((f & 0xf00) >> 8);
            SOutData(page, mml, page.port[port], (byte)(adr + 1 + vch * 2), data);

            if (page.hardEnvelopeSync.sw)
            {
                f = GetSsgHsFNum(page, mml
                    , page.hardEnvelopeSync.octave
                    , page.noteCmd
                    , page.shift + page.keyShift + arpNote);//
                f = f + page.hardEnvelopeSync.detune;
                page.hsFnum = f;

                SOutData(page, mml, page.port[port], (byte)(adr + 0x0b), (byte)(page.hsFnum & 0xff));
                SOutData(page, mml, page.port[port], (byte)(adr + 0x0c), (byte)((page.hsFnum >> 8) & 0xff));
            }
        }

        private void SetSsgPCMFNum(partPage page, MML mml)
        {
            int arpNote = page.arpFreqMode ? 0 : page.arpDelta;
            int arpFreq = page.arpFreqMode ? page.arpDelta : 0;

            long f = 0;
            f = -page.detune;
            f = f + arpFreq;
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
                f -= page.lfo[lfo].value + page.lfo[lfo].param[6];
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
                f += GetSsgPCMFNum(page, mml, page.octaveNow, page.noteCmd, page.shift + page.keyShift + arpNote, page.pitchShift);//
            }

            f = Common.CheckRange((int)f, 0, 0x7fff_ffff);

            if (page.streamFreq != f)
            {
                byte[] cmd;
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

        }

        public int GetSsgFNum(partPage page, MML mml, int octave, char noteCmd, int shift,int pitchShift)
        {
            if (page.pcm)
            {
                return GetSsgPCMFNum(page, mml, octave, noteCmd, shift, pitchShift);
            }

            int o = octave - 1;
            int n = Const.NOTE.IndexOf(noteCmd) + shift;
            o += n / 12;
            o = Common.CheckRange(o, 0, 7);
            n %= 12;

            int f = o * 12 + n;
            f += pitchShift;
            if (f < 0) f = 0;
            if (f >= page.chip.FNumTbl[1].Length) f = page.chip.FNumTbl[1].Length - 1;

            return page.chip.FNumTbl[1][f];
        }

        public int GetSsgPCMFNum(partPage page, MML mml, int octave, char noteCmd, int shift, int pitchShift)
        {
            int o = octave - 1;
            int n = Const.NOTE.IndexOf(noteCmd) + shift;

            o += n / 12;
            n %= 12;
            if (n < 0)
            {
                n += 12;
                o = Common.CheckRange(--o, 1, 8);
            }

            long f = parent.instPCM[page.instrument].Item2.freq;

            return ((int)(f * Const.pcmMTbl[n] * Math.Pow(2, (o - 3))) + 1) + pitchShift;
        }

        public int GetSsgHsFNum(partPage page, MML mml, int octave, char noteCmd, int shift)
        {
            int o = octave - 1;
            int n = Const.NOTE.IndexOf(noteCmd) + shift;
            o += n / 12;
            o = Common.CheckRange(o, 1, 6);
            n %= 12;

            int f = o * 12 + n;
            if (f < 0) f = 0;
            if (f >= hsFnumTbl.Length) f = hsFnumTbl.Length - 1;

            return hsFnumTbl[f];
        }

        public void OutOPNSetPanAMSPMS(MML mml, partPage page, int pan, int ams, int pms)
        {
            //TODO: 効果音パートで指定されている場合の考慮不足
            int vch;
            byte[] port;
            GetPortVch(page, out port, out vch);
            if (page.chip is YM2612X && page.ch > 8 && page.ch < 12)
            {
                vch = 2;
            }

            pan = pan & 3;
            ams = ams & 3;
            pms = pms & 7;

            SOutData(page, mml, port, (byte)(0xb4 + vch), (byte)((pan << 6) + (ams << 4) + pms));
        }

        public void OutOPNSetHardLfo(MML mml, partPage page, bool sw, int lfoNum)
        {
            SOutData(page,
                mml,
                page.port[0]
                , 0x22
                , (byte)((lfoNum & 7) + (sw ? 8 : 0))
                );
        }

        public void OutOPNSetCh3SpecialMode(MML mml, partPage page, bool sw)
        {
            byte[] port = page.port[0];
            if (page.chip.chipType == enmChipType.YM2609)
            {
                if (page.ch == 8 || page.ch == 15 || page.ch == 16 || page.ch == 17)
                {
                    port = page.port[2];
                }
            }
            // ignore Timer ^^;
            SOutData(page,
                mml,
                port
                , 0x27
                , (byte)((sw ? 0x40 : 0))
                );
        }

        public void OutFmSetFeedbackAlgorithm(MML mml, partPage page, int fb, int alg)
        {
            int vch;
            byte[] port;
            GetPortVch(page, out port, out vch);

            fb &= 7;
            alg &= 7;

            SOutData(page, mml, port, (byte)(0xb0 + vch), (byte)((fb << 3) + alg));
        }

        public void OutFmSetDtMl(MML mml, partPage page, int ope, int dt, int ml)
        {
            int vch;
            byte[] port;
            GetPortVch(page, out port, out vch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            dt &= 7;
            ml &= 15;

            SOutData(page, mml, port, (byte)(0x30 + vch + ope * 4), (byte)((dt << 4) + ml));
        }

        public void OutFmSetTl(MML mml, partPage page, int ope, int tl)
        {
            int vch;
            byte[] port;
            GetPortVch(page, out port, out vch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            tl &= 0x7f;

            if (page.beforeTL[ope] == tl) return;
            page.beforeTL[ope] = tl;

            SOutData(page, mml, port, (byte)(0x40 + vch + ope * 4), (byte)tl);
        }

        public void OutFmSetKsAr(MML mml, partPage page, int ope, int ks, int ar, int pr = -1)
        {
            int vch;
            byte[] port;
            GetPortVch(page, out port, out vch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            ks = (ks & 3) << 6;
            ar = ar & 31;
            pr = (pr != -1) ? ((pr & 1) << 5) : 0;

            SOutData(page, mml, port, (byte)(0x50 + vch + ope * 4), (byte)(ks | ar | pr));
        }

        public void OutFmSetAmDr(MML mml, partPage page, int ope, int am, int dr)
        {
            int vch;
            byte[] port;
            GetPortVch(page, out port, out vch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            am &= 1;
            dr &= 31;

            SOutData(page, mml, port, (byte)(0x60 + vch + ope * 4), (byte)((am << 7) + dr));
        }

        public void OutFmSetSr(MML mml, partPage page, int ope, int sr)
        {
            int vch;
            byte[] port;
            GetPortVch(page, out port, out vch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            sr &= 31;

            SOutData(page, mml, port, (byte)(0x70 + vch + ope * 4), (byte)(sr));
        }

        public void OutFmSetSlRr(MML mml, partPage page, int ope, int sl, int rr)
        {
            int vch;
            byte[] port;
            GetPortVch(page, out port, out vch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            sl &= 15;
            rr &= 15;

            SOutData(page, mml, port, (byte)(0x80 + vch + ope * 4), (byte)((sl << 4) + rr));
        }

        protected void GetPortVch(partPage page, out byte[] port, out int vch)
        {
            if (!(page.chip is YM2609))
            {
                port = page.ch > 2 ? page.port[1] : page.port[0];
                vch = (byte)(page.ch > 2 ? page.ch - 3 : page.ch);
            }
            else
            {
                port =
                    page.ch < 3 ?
                    page.port[0] :
                    (page.ch < 6 ?
                        page.port[1] :
                        (page.ch < 9 ?
                            page.port[2] :
                            (page.ch < 12 ?
                                page.port[3] :
                                (page.ch < 15 ?
                                    page.port[0] :
                                    page.port[2]
                                )
                            )
                        )
                    );
                vch = (byte)(
                    page.ch < 3 ?
                    page.ch :
                    (page.ch < 6 ?
                        (page.ch - 3) :
                        (page.ch < 9 ?
                            (page.ch - 6) :
                            (page.ch < 12 ?
                                (page.ch - 9) :
                                (page.ch < 15 ?
                                    2 :
                                    2
                                )
                            )
                        )
                    )
                );
            }
        }

        protected void GetPortVchSsg(partPage page, out int port, out int adr, out int vch)
        {
            int m = (page.chip is YM2203) ? 0 : 3;
            vch = (byte)(page.ch - (m + 6));
            port = 0;
            adr = 0;

            if (!(page.chip is YM2609)) return;

            if (page.ch >= 18 && page.ch <= 20)
            {
                port = 0;
                vch = (byte)(page.ch - 18);
                adr = 0;
            }
            else if (page.ch >= 21 && page.ch <= 23)
            {
                port = 1;
                vch = (byte)(page.ch - 21);
                adr = 0x20;
            }
            else if (page.ch >= 24 && page.ch <= 26)
            {
                port = 2;
                vch = (byte)(page.ch - 24);
                adr = 0;
            }
            else if (page.ch >= 27 && page.ch <= 29)
            {
                port = 2;
                vch = (byte)(page.ch - 27);
                adr = 0x10;
            }
        }

        public void OutFmSetSSGEG(MML mml, partPage page, int ope, int n)
        {
            int vch;
            byte[] port;
            GetPortVch(page, out port, out vch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            n &= 15;

            SOutData(page, mml, port, (byte)(0x90 + vch + ope * 4), (byte)n);
        }

        /// <summary>
        /// FMボリュームの設定
        /// </summary>
        /// <param name="ch">チャンネル</param>
        /// <param name="vol">ボリューム値</param>
        /// <param name="n">音色番号</param>
        public void OutFmSetVolume(partPage page, MML mml, int vol)//, int n)
        {
            //if (!parent.instFM.ContainsKey(n))
            //{
            //    msgBox.setWrnMsg(string.Format(msg.get("E11000"), n), mml.line.Lp);
            //    return;
            //}
            //Console.WriteLine("{0}", vol);

            int m = (page.chip is YM2203) ? 0 : 3;
            partPage vpg = page;
            if (!(page.chip is YM2609))
            {
                if (page.chip.lstPartWork[2].cpg.Ch3SpecialMode && page.ch >= m + 3 && page.ch < m + 6)
                    vpg = page.chip.lstPartWork[2].cpg;
            }
            else
            {
                if (page.chip.lstPartWork[2].cpg.Ch3SpecialMode && page.ch >= 12 && page.ch < 15)
                    vpg = page.chip.lstPartWork[2].cpg;
                if (page.chip.lstPartWork[8].cpg.Ch3SpecialMode && page.ch >= 15 && page.ch < 18)
                    vpg = page.chip.lstPartWork[8].cpg;
            }

            int alg = vpg.voice[0] & 0x7;
            int[] ope = new int[4] {
                vpg.voice[partPage.voiceWidth+0*partPage.voiceWidth+5]
                , vpg.voice[partPage.voiceWidth+1*partPage.voiceWidth+5]
                , vpg.voice[partPage.voiceWidth + 2 * partPage.voiceWidth + 5]
                , vpg.voice[partPage.voiceWidth + 3 * partPage.voiceWidth + 5]
            };
            int[][] algs = new int[8][]
            {
                new int[4] { 0,0,0,1}
                ,new int[4] { 0,0,0,1}
                ,new int[4] { 0,0,0,1}
                ,new int[4] { 0,0,0,1}
                ,new int[4] { 0,1,0,1}
                ,new int[4] { 0,1,1,1}
                ,new int[4] { 0,1,1,1}
                ,new int[4] { 1,1,1,1}
            };

            byte res = 0;
            if (page.voperator != 0)
            {
                int opV = page.voperator;
                while (opV % 10 != 0)
                {
                    if (opV % 10 > 0 && opV % 10 < 5)
                    {
                        res += (byte)(1 << (opV % 10 - 1));
                    }
                    else
                    {
                        msgBox.setErrMsg(string.Format(msg.get("E11005"), opV), mml.line.Lp);
                        break;
                    }
                    opV /= 10;
                }
            }

            for (int i = 0; i < 4; i++)
            {
                if (res == 0)
                {
                    if (algs[alg][i] == 0)
                    {
                        int n = GetTLOFS(page, mml, i);
                        if (n == 0)
                        {
                            ope[i] = -1;
                        }
                        else
                        {
                            ope[i] += n;
                            ope[i] = Common.CheckRange(ope[i], 0, 127);
                        }
                        continue;
                    }
                    else if ((page.slots & (1 << i)) == 0)
                    {
                        //ope[i] = -1;
                        ope[i] += GetTLOFS(page, mml, i);
                        ope[i] = Common.CheckRange(ope[i], 0, 127);
                        continue;
                    }
                }
                else
                {
                    if((res & (1 << i)) == 0)
                    {
                        ope[i] = -1;
                        continue;
                    }
                }
                ope[i] += (127 - vol);
                ope[i] += GetTLOFS(page,mml,i);
                ope[i] = Common.CheckRange(ope[i], 0, 127);
            }

            MML vmml = new MML();
            vmml.args = new List<object>();
            vmml.args.Add(vol);
            vmml.type = enmMMLType.Volume;
            if (mml != null)
                vmml.line = mml.line;
            if ((page.slots & 1) != 0 && ope[0] != -1) ((ClsOPN)page.chip).OutFmSetTl(vmml, vpg, 0, ope[0]);
            if ((page.slots & 2) != 0 && ope[1] != -1) ((ClsOPN)page.chip).OutFmSetTl(vmml, vpg, 1, ope[1]);
            if ((page.slots & 4) != 0 && ope[2] != -1) ((ClsOPN)page.chip).OutFmSetTl(vmml, vpg, 2, ope[2]);
            if ((page.slots & 8) != 0 && ope[3] != -1) ((ClsOPN)page.chip).OutFmSetTl(vmml, vpg, 3, ope[3]);
        }

        public int GetTLOFS(partPage page,MML mml, int slot)
        {
            int tlOfs = 0;

            foreach (byte k in page.TLOFS.Keys)
            {
                byte trgSlot = GetSlotBit(mml, page.TLOFS[k]);
                if ((trgSlot & (1 << slot)) == 0) continue;
                if (!parent.instTLOFS.ContainsKey(k)) continue;

                byte[] dat = parent.instTLOFS[k];
                if (dat == null || dat.Length < 3) continue;
                byte trig = dat[1];
                byte reso = Math.Max(dat[2], (byte)1);

                int n;
                if (trig == 0)
                {
                    //octave 
                    int addShift = page.keyShift + page.toneDoublerKeyShift + (page.arpFreqMode ? 0 : page.arpDelta);
                    n = page.octaveNow * 12 + "c d ef g a b".IndexOf(page.noteCmd) + page.shift + 12 + addShift;
                    n = Common.CheckRange(n, 0, 132);
                }
                else
                {
                    //volume
                    n = 127 - page.volume;
                    n = Common.CheckRange(n, 0, 127);
                }
                n = n / reso;
                n += 3;
                if (n >= dat.Length) n = dat.Length - 1;
                sbyte v = (sbyte)dat[n];
                tlOfs += v;
            }

            return tlOfs;
        }

        public void OutFmSetTL(partPage page, MML mml, int tl1, int tl2, int tl3, int tl4,int slot)//, int n)
        {
            //if (!parent.instFM.ContainsKey(n))
            //{
            //    msgBox.setWrnMsg(string.Format(msg.get("E11000"), n), mml.line.Lp);
            //    return;
            //}

            int alg =page.voice[0] & 0x7;
            int[] ope = new int[4] {
                page.voice[partPage.voiceWidth + 0 * partPage.voiceWidth + 5]
                , page.voice[partPage.voiceWidth + 1 * partPage.voiceWidth + 5]
                , page.voice[partPage.voiceWidth + 2 * partPage.voiceWidth + 5]
                , page.voice[partPage.voiceWidth + 3 * partPage.voiceWidth + 5]
            };
            int[][] algs = new int[8][]
            {
                new int[4] { 1,1,1,0}
                ,new int[4] { 1,1,1,0}
                ,new int[4] { 1,1,1,0}
                ,new int[4] { 1,1,1,0}
                ,new int[4] { 1,0,1,0}
                ,new int[4] { 1,0,0,0}
                ,new int[4] { 1,0,0,0}
                ,new int[4] { 0,0,0,0}
            };

            for (int i = 0; i < 4; i++)
            {
                if (algs[alg][i] == 0 || (slot & (1 << i)) == 0)
                {
                    ope[i] = -1;
                    continue;
                }
                if (i == 0) ope[i] = ope[i] - tl1;
                if (i == 1) ope[i] = ope[i] - tl2;
                if (i == 2) ope[i] = ope[i] - tl3;
                if (i == 3) ope[i] = ope[i] - tl4;
                ope[i] = Common.CheckRange(ope[i], 0, 127);
            }

            partPage vpg = page;
            if (!(page.chip is YM2609))
            {
                int m = (page.chip is YM2203) ? 0 : 3;
                if (page.chip.lstPartWork[2].cpg.Ch3SpecialMode && page.ch >= m + 3 && page.ch < m + 6)
                {
                    vpg = page.chip.lstPartWork[2].cpg;
                }
            }
            else
            {
                if (page.chip.lstPartWork[2].cpg.Ch3SpecialMode && page.ch >= 12 && page.ch < 15)
                {
                    vpg = page.chip.lstPartWork[2].cpg;
                }
                if (page.chip.lstPartWork[8].cpg.Ch3SpecialMode && page.ch >= 15 && page.ch < 18)
                {
                    vpg = page.chip.lstPartWork[8].cpg;
                }
            }

            MML vmml = new MML();
            vmml.args = new List<object>();
            vmml.args.Add(tl1);
            vmml.type = enmMMLType.unknown;//.TotalLevel;
            if (mml != null)
                vmml.line = mml.line;
            if ((page.slots & 1) != 0 && ope[0] != -1) ((ClsOPN)page.chip).OutFmSetTl(vmml, vpg, 0, ope[0]);
            if ((page.slots & 2) != 0 && ope[1] != -1) ((ClsOPN)page.chip).OutFmSetTl(vmml, vpg, 1, ope[1]);
            if ((page.slots & 4) != 0 && ope[2] != -1) ((ClsOPN)page.chip).OutFmSetTl(vmml, vpg, 2, ope[2]);
            if ((page.slots & 8) != 0 && ope[3] != -1) ((ClsOPN)page.chip).OutFmSetTl(vmml, vpg, 3, ope[3]);
        }

        public void OutFmCh3SpecialModeSetFnum(MML mml, partPage page, byte ope, int octave, int num)
        {
            ope &= 3;
            if (ope == 0)
            {
                SOutData(page, mml, page.port[0], 0xa6, (byte)(((num & 0x700) >> 8) + (((octave - 1) & 0x7) << 3)));
                SOutData(page, mml, page.port[0], 0xa2, (byte)(num & 0xff));
            }
            else
            {
                SOutData(page, mml, page.port[0], (byte)(0xac + ope), (byte)(((num & 0x700) >> 8) + (((octave - 1) & 0x7) << 3)));
                SOutData(page, mml, page.port[0], (byte)(0xa8 + ope), (byte)(num & 0xff));
            }
        }

        public void OutFmSetInstrument(partPage page, MML mml, int n, int vol, char typeBeforeSend)
        {
            int modeBeforeSend = parent.info.modeBeforeSend;
            if (typeBeforeSend == 'n' || typeBeforeSend == 'N' || typeBeforeSend == 'R' || typeBeforeSend == 'A')
            {
                if (typeBeforeSend == 'N')
                {
                    modeBeforeSend = 0;
                }
                else if (typeBeforeSend == 'R')
                {
                    modeBeforeSend = 1;
                }
                else if (typeBeforeSend == 'A')
                {
                    modeBeforeSend = 2;
                }
            }

            if (!parent.instFM.ContainsKey(n))
            {
                msgBox.setWrnMsg(string.Format(msg.get("E11001"), n), mml.line.Lp);
                return;
            }

            int m = (page.chip is YM2203) ? 0 : 3;

            //ここにOP指定を読み込む処理追加
            byte UMop = 0xf;
            bool isDef = true;
            int FBALG = 3;
            if (page.Type == enmChannelType.FMOPNex)
            {
                //
                if (page.Ch3SpecialMode)
                {
                    if (mml.args.Count > 4 && mml.args[3].ToString() == "OP")
                    {
                        UMop = GetSlotBit(mml, (byte)(int)mml.args[4]);
                        UMop = (byte)Common.CheckRange(UMop, 1, 15);
                        isDef = false;
                    }
                    if (mml.args.Count > 6 && mml.args[5].ToString() == "FA")
                    {
                        FBALG = (int)mml.args[6];
                    }
                }
            }

            partPage vpg = page;
            if (!(page.chip is YM2609))
            {
                if (page.chip.lstPartWork[2].cpg.Ch3SpecialMode && page.ch >= m + 3 && page.ch < m + 6)
                {
                    vpg = page.chip.lstPartWork[2].cpg;
                }
                if (isDef && (page.ch == 2 || page.ch == m + 3 || page.ch == m + 4 || page.ch == m + 5))
                {
                    page.chip.lstPartWork[2].cpg.instrument = n;
                    page.chip.lstPartWork[m + 3].cpg.instrument = n;
                    page.chip.lstPartWork[m + 4].cpg.instrument = n;
                    page.chip.lstPartWork[m + 5].cpg.instrument = n;
                }
            }
            else
            {
                if (page.chip.lstPartWork[2].cpg.Ch3SpecialMode && page.ch >= 12 && page.ch < 15)
                {
                    vpg = page.chip.lstPartWork[2].cpg;
                    if (isDef)
                    {
                        page.chip.lstPartWork[2].cpg.instrument = n;
                        page.chip.lstPartWork[12].cpg.instrument = n;
                        page.chip.lstPartWork[13].cpg.instrument = n;
                        page.chip.lstPartWork[14].cpg.instrument = n;
                    }
                }
                if (page.chip.lstPartWork[8].cpg.Ch3SpecialMode && page.ch >= 15 && page.ch < 18)
                {
                    vpg = page.chip.lstPartWork[8].cpg;
                    if (isDef)
                    {
                        page.chip.lstPartWork[8].cpg.instrument = n;
                        page.chip.lstPartWork[15].cpg.instrument = n;
                        page.chip.lstPartWork[16].cpg.instrument = n;
                        page.chip.lstPartWork[17].cpg.instrument = n;
                    }
                }
            }

            switch (modeBeforeSend)
            {
                case 0: // N)one
                    break;
                case 1: // R)R only
                    for (int ope = 0; ope < 4; ope++)
                    {
                        if (!isDef && (UMop & (1 << ope)) == 0) continue;
                        ((ClsOPN)page.chip).OutFmSetSlRr(mml, vpg, ope, 0, 15);
                    }
                    break;
                case 2: // A)ll
                    for (int ope = 0; ope < 4; ope++)
                    {
                        if (!isDef && (UMop & (1 << ope)) == 0) continue;
                        ((ClsOPN)page.chip).OutFmSetDtMl(mml, vpg, ope, 0, 0);
                        ((ClsOPN)page.chip).OutFmSetKsAr(mml, vpg, ope, 3, 31);
                        ((ClsOPN)page.chip).OutFmSetAmDr(mml, vpg, ope, 1, 31);
                        ((ClsOPN)page.chip).OutFmSetSr(mml, vpg, ope, 31);
                        ((ClsOPN)page.chip).OutFmSetSlRr(mml, vpg, ope, 0, 15);
                        ((ClsOPN)page.chip).OutFmSetSSGEG(mml, vpg, ope, 0);
                    }
                    ((ClsOPN)page.chip).OutFmSetFeedbackAlgorithm(mml, vpg, 7, 7);
                    break;
            }


            for (int ope = 0; ope < 4; ope++)
            {
                //ch3以外の拡張チャンネルであってもUMopが未指定の場合はそのまま設定する
                if (!isDef && (UMop & (1 << ope)) == 0) continue;

                ((ClsOPN)page.chip).OutFmSetDtMl(mml, vpg, ope, parent.instFM[n].Item2[ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], parent.instFM[n].Item2[ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 8]);
                ((ClsOPN)page.chip).OutFmSetKsAr(mml, vpg, ope, parent.instFM[n].Item2[ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 7], parent.instFM[n].Item2[ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 1]);
                ((ClsOPN)page.chip).OutFmSetAmDr(mml, vpg, ope, parent.instFM[n].Item2[ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 10], parent.instFM[n].Item2[ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 2]);
                ((ClsOPN)page.chip).OutFmSetSr(mml, vpg, ope, parent.instFM[n].Item2[ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 3]);
                ((ClsOPN)page.chip).OutFmSetSlRr(mml, vpg, ope, parent.instFM[n].Item2[ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 5], parent.instFM[n].Item2[ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 4]);
                ((ClsOPN)page.chip).OutFmSetSSGEG(mml, vpg, ope, parent.instFM[n].Item2[ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 11]);

                for (int i = 0; i < Const.INSTRUMENT_M_OPERATOR_SIZE; i++)
                    vpg.voice[partPage.voiceWidth + ope * partPage.voiceWidth + i]
                        = parent.instFM[n].Item2[ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 1 + i];
            }

            if ((page.slots & 1) != 0) page.op1ml = parent.instFM[n].Item2[0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
            if ((page.slots & 2) != 0) page.op2ml = parent.instFM[n].Item2[1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
            if ((page.slots & 4) != 0) page.op3ml = parent.instFM[n].Item2[2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
            if ((page.slots & 8) != 0) page.op4ml = parent.instFM[n].Item2[3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
            if ((page.slots & 1) != 0) page.op1dt2 = 0;
            if ((page.slots & 2) != 0) page.op2dt2 = 0;
            if ((page.slots & 4) != 0) page.op3dt2 = 0;
            if ((page.slots & 8) != 0) page.op4dt2 = 0;

            if ((FBALG & 1) != 0) page.voice[0] = vpg.voice[0] = parent.instFM[n].Item2[45];//ALG
            if ((FBALG & 2) != 0) page.voice[1] = vpg.voice[1] = parent.instFM[n].Item2[46];//FB
            ((ClsOPN)page.chip).OutFmSetFeedbackAlgorithm(mml, vpg, vpg.voice[1], vpg.voice[0]);

            int alg = page.voice[0] & 0x7;
            int[] op = new int[4] {
                parent.instFM[n].Item2[0*Const.INSTRUMENT_M_OPERATOR_SIZE + 6]
                , parent.instFM[n].Item2[1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 6]
                , parent.instFM[n].Item2[2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 6]
                , parent.instFM[n].Item2[3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 6]
            };
            int[][] algs = new int[8][]
            {
                new int[4] { 1,1,1,0}
                ,new int[4] { 1,1,1,0}
                ,new int[4] { 1,1,1,0}
                ,new int[4] { 1,1,1,0}
                ,new int[4] { 1,0,1,0}
                ,new int[4] { 1,0,0,0}
                ,new int[4] { 1,0,0,0}
                ,new int[4] { 0,0,0,0}
            };

            for (int i = 0; i < 4; i++)
            {
                if ( 
                    (isDef && algs[alg][i] == 0) 
                    || 
                    (!isDef && (algs[alg][i] == 0 || (page.slots & (1 << i)) == 0))
                    )
                {
                    op[i] = -1;
                    continue;
                }
                op[i] = Common.CheckRange(op[i], 0, 127);
            }


            if ((isDef || (UMop & 1) != 0) && op[0] != -1) ((ClsOPN)page.chip).OutFmSetTl(mml, vpg, 0, op[0]);
            if ((isDef || (UMop & 2) != 0) && op[1] != -1) ((ClsOPN)page.chip).OutFmSetTl(mml, vpg, 1, op[1]);
            if ((isDef || (UMop & 4) != 0) && op[2] != -1) ((ClsOPN)page.chip).OutFmSetTl(mml, vpg, 2, op[2]);
            if ((isDef || (UMop & 8) != 0) && op[3] != -1) ((ClsOPN)page.chip).OutFmSetTl(mml, vpg, 3, op[3]);


            //音量を再セットする

            OutFmSetVolume(page, mml, vol);

            //拡張チャンネルの場合は他の拡張チャンネルも音量を再セットする
            if (page.Type == enmChannelType.FMOPNex && isDef)
            {
                if (!(page.chip is YM2609))
                {
                    if (page.ch != 2) OutFmSetVolume(page.chip.lstPartWork[2].cpg, mml, page.chip.lstPartWork[2].cpg.volume);
                    if (page.ch != m + 3) OutFmSetVolume(page.chip.lstPartWork[m + 3].cpg, mml, page.chip.lstPartWork[m + 3].cpg.volume);
                    if (page.ch != m + 4) OutFmSetVolume(page.chip.lstPartWork[m + 4].cpg, mml, page.chip.lstPartWork[m + 4].cpg.volume);
                    if (page.ch != m + 5) OutFmSetVolume(page.chip.lstPartWork[m + 5].cpg, mml, page.chip.lstPartWork[m + 5].cpg.volume);
                }
                else
                {
                    if (page.ch == 2 || page.ch == 12 || page.ch == 13 || page.ch == 14)
                    {
                        //YM2609 ch3 || ch13 || ch14 || ch15
                        if (page.ch != 2) OutFmSetVolume(page.chip.lstPartWork[2].cpg, mml, page.chip.lstPartWork[2].cpg.volume);
                        if (page.ch != 12) OutFmSetVolume(page.chip.lstPartWork[12].cpg, mml, page.chip.lstPartWork[12].cpg.volume);
                        if (page.ch != 13) OutFmSetVolume(page.chip.lstPartWork[13].cpg, mml, page.chip.lstPartWork[13].cpg.volume);
                        if (page.ch != 14) OutFmSetVolume(page.chip.lstPartWork[14].cpg, mml, page.chip.lstPartWork[14].cpg.volume);
                    }
                    else
                    {
                        //YM2609 ch9 || ch16 || ch17 || ch18
                        if (page.ch != 8) OutFmSetVolume(page.chip.lstPartWork[8].cpg, mml, page.chip.lstPartWork[8].cpg.volume);
                        if (page.ch != 15) OutFmSetVolume(page.chip.lstPartWork[15].cpg, mml, page.chip.lstPartWork[15].cpg.volume);
                        if (page.ch != 16) OutFmSetVolume(page.chip.lstPartWork[16].cpg, mml, page.chip.lstPartWork[16].cpg.volume);
                        if (page.ch != 17) OutFmSetVolume(page.chip.lstPartWork[17].cpg, mml, page.chip.lstPartWork[17].cpg.volume);
                    }
                }
            }

        }

        public void OutFmKeyOff(partPage page, MML mml)
        {
            int n = (page.chip is YM2203) ? 0 : 3;

            if (page.chip is YM2612X && (page.ch > 8 || page.ch == 5) && page.pcm)
            {
                ((YM2612X)page.chip).OutYM2612XPcmKeyOFF(mml, page);
                return;
            }

            if (!page.pcm)
            {
                if (!(page.chip is YM2609))
                {
                    if (page.chip.lstPartWork[2].cpg.Ch3SpecialMode && page.Type == enmChannelType.FMOPNex)
                    {
                        page.Ch3SpecialModeKeyOn = false;

                        int slot = (page.chip.lstPartWork[2].apg.Ch3SpecialModeKeyOn ? page.chip.lstPartWork[2].apg.slots : 0x0)
                            | (page.chip.lstPartWork[n + 3].apg.Ch3SpecialModeKeyOn ? page.chip.lstPartWork[n + 3].apg.slots : 0x0)
                            | (page.chip.lstPartWork[n + 4].apg.Ch3SpecialModeKeyOn ? page.chip.lstPartWork[n + 4].apg.slots : 0x0)
                            | (page.chip.lstPartWork[n + 5].apg.Ch3SpecialModeKeyOn ? page.chip.lstPartWork[n + 5].apg.slots : 0x0);

                        SOutData(page, mml, page.port[0], 0x28, (byte)((slot << 4) + 2));
                    }
                    else
                    {
                        if (page.ch >= 0 && page.ch < n + 3)
                        {
                            byte vch = (byte)((page.ch > 2) ? page.ch + 1 : page.ch);
                            //key off
                            SOutData(page, mml, page.port[0], 0x28, (byte)(0x00 + (vch & 7)));
                            page.keyOnDelay.KeyOff();
                        }
                    }
                }
                else
                {
                    if ((page.ch == 2 || page.ch == 12
                        || page.ch == 13 || page.ch == 14)
                        && page.chip.lstPartWork[2].apg.Ch3SpecialMode)
                    {
                        page.Ch3SpecialModeKeyOn = false;

                        int slot = (page.chip.lstPartWork[2].apg.Ch3SpecialModeKeyOn ? page.chip.lstPartWork[2].apg.slots : 0x0)
                            | (page.chip.lstPartWork[12].apg.Ch3SpecialModeKeyOn ? page.chip.lstPartWork[12].apg.slots : 0x0)
                            | (page.chip.lstPartWork[13].apg.Ch3SpecialModeKeyOn ? page.chip.lstPartWork[13].apg.slots : 0x0)
                            | (page.chip.lstPartWork[14].apg.Ch3SpecialModeKeyOn ? page.chip.lstPartWork[14].apg.slots : 0x0);

                        outDatum od = new outDatum();
                        od.val = (byte)((slot << 4) + 2);
                        if (mml != null)
                        {
                            od.type = mml.type;
                            if (mml.line != null && mml.line.Lp != null)
                            {
                                od.linePos = new LinePos(
                                    mml.line.Lp.document,
                                    mml.line.Lp.srcMMLID,
                                    mml.line.Lp.row,
                                    mml.line.Lp.col,
                                    mml.line.Lp.length,
                                    mml.line.Lp.part,
                                    mml.line.Lp.chip,
                                    mml.line.Lp.chipIndex,
                                    mml.line.Lp.chipNumber,
                                    mml.line.Lp.ch);
                            }
                        }
                        ((YM2609)page.chip).opna20x028KeyOnData.Add(od);
                    }
                    else if ((page.ch == 8 || page.ch == 15 || page.ch == 16 || page.ch == 17) && page.chip.lstPartWork[8].apg.Ch3SpecialMode)
                    {
                        page.Ch3SpecialModeKeyOn = false;

                        int slot = (page.chip.lstPartWork[8].apg.Ch3SpecialModeKeyOn ? page.chip.lstPartWork[8].apg.slots : 0x0)
                            | (page.chip.lstPartWork[15].apg.Ch3SpecialModeKeyOn ? page.chip.lstPartWork[15].apg.slots : 0x0)
                            | (page.chip.lstPartWork[16].apg.Ch3SpecialModeKeyOn ? page.chip.lstPartWork[16].apg.slots : 0x0)
                            | (page.chip.lstPartWork[17].apg.Ch3SpecialModeKeyOn ? page.chip.lstPartWork[17].apg.slots : 0x0);

                        outDatum od = new outDatum();
                        od.val = (byte)((slot << 4) + 2);
                        if (mml != null)
                        {
                            od.type = mml.type;
                            if (mml.line != null && mml.line.Lp != null)
                            {
                                od.linePos = new LinePos(
                                    mml.line.Lp.document,
                                    mml.line.Lp.srcMMLID,
                                    mml.line.Lp.row,
                                    mml.line.Lp.col,
                                    mml.line.Lp.length,
                                    mml.line.Lp.part,
                                    mml.line.Lp.chip,
                                    mml.line.Lp.chipIndex,
                                    mml.line.Lp.chipNumber,
                                    mml.line.Lp.ch);
                            }
                        }
                        ((YM2609)page.chip).opna20x228KeyOnData.Add(od);
                    }
                    else
                    {
                        if (page.ch >= 0 && page.ch < 6)
                        {
                            byte vch = (byte)((page.ch > 2) ? page.ch + 1 : page.ch);
                            //key off
                            outDatum od = new outDatum();
                            od.val = (byte)(0x00 + (vch & 7));
                            if (mml != null)
                            {
                                od.type = mml.type;
                                if (mml.line != null && mml.line.Lp != null)
                                {
                                    od.linePos = new LinePos(
                                        mml.line.Lp.document,
                                        mml.line.Lp.srcMMLID,
                                        mml.line.Lp.row,
                                        mml.line.Lp.col,
                                        mml.line.Lp.length,
                                        mml.line.Lp.part,
                                        mml.line.Lp.chip,
                                        mml.line.Lp.chipIndex,
                                        mml.line.Lp.chipNumber,
                                        mml.line.Lp.ch);
                                }
                            }
                            ((YM2609)page.chip).opna20x028KeyOnData.Add(od);
                        }
                        else if (page.ch >= 6 && page.ch < 12)
                        {
                            byte vch = (byte)(page.ch - 6);
                            vch = (byte)(((vch > 2) ? (vch + 1) : vch));
                            //key off
                            outDatum od = new outDatum();
                            od.val = (byte)(0 + (vch & 7));
                            if (mml != null)
                            {
                                od.type = mml.type;
                                if (mml.line != null && mml.line.Lp != null)
                                {
                                    od.linePos = new LinePos(
                                        mml.line.Lp.document,
                                        mml.line.Lp.srcMMLID,
                                        mml.line.Lp.row,
                                        mml.line.Lp.col,
                                        mml.line.Lp.length,
                                        mml.line.Lp.part,
                                        mml.line.Lp.chip,
                                        mml.line.Lp.chipIndex,
                                        mml.line.Lp.chipNumber,
                                        mml.line.Lp.ch);
                                }
                            }
                            ((YM2609)page.chip).opna20x228KeyOnData.Add(od);
                        }
                    }
                }
                return;
            }


            if (parent.info.Version == 1.51f)
            {

            }
            else
            {
                byte[] cmd;
                if (parent.info.format == enmFormat.ZGM)
                {
                    if (parent.ChipCommandSize == 2) cmd = new byte[] { 0x34, 0x00 };
                    else cmd = new byte[] { 0x34 };
                }
                else cmd = new byte[] { 0x94 };

                if (parent.info.format == enmFormat.VGM)
                {
                    //Stop Stream
                    SOutData(page, mml, cmd, (byte)page.spg.streamID);
                }
            }

            page.pcmWaitKeyOnCounter = -1;
        }

        public void OutFmAllKeyOff()
        {

            foreach (partWork pw in lstPartWork)
            {
                foreach (partPage page in pw.pg)
                {
                    if (!(page.chip is YM2609)) { if (page.ch > 5) continue; }
                    else if (page.ch > 11) continue;

                    OutFmKeyOff(page, null);
                    OutFmSetTl(null, page, 0, 127);
                    OutFmSetTl(null, page, 1, 127);
                    OutFmSetTl(null, page, 2, 127);
                    OutFmSetTl(null, page, 3, 127);
                }
            }
        }

        public virtual void OutFmSetFnum(partPage page, MML mml, int octave, int num)
        {
            int freq;
            freq = ((num & 0x700) >> 8) + (((octave - 1) & 0x7) << 3);
            freq = (freq << 8) + (num & 0xff);

            if (freq == page.spg.freq) return;

            page.spg.freq = freq;

            partWork pw = page.chip.lstPartWork[2];
            if (page.chip is YM2609)
            {
                if (page.ch == 8 || page.ch == 15 || page.ch == 16 || page.ch == 17)
                {
                    pw = page.chip.lstPartWork[8];
                }
            }

            if (pw.apg.Ch3SpecialMode && page.Type == enmChannelType.FMOPNex)
            {
                if ((page.slots & 8) != 0)
                {
                    int f;
                    if (pw.slotFixedFnum[3] == -1) f = freq + page.slotDetune[3];
                    else f = pw.slotFixedFnum[3] + page.slotDetune[3];
                    SOutData(page, mml, page.port[0], (byte)0xa6, (byte)(f >> 8));
                    SOutData(page, mml, page.port[0], (byte)0xa2, (byte)f);
                }
                if ((page.slots & 4) != 0)
                {
                    int f;
                    if (pw.slotFixedFnum[2] == -1) f = freq + page.slotDetune[2];
                    else f = pw.slotFixedFnum[2] + page.slotDetune[2];
                    SOutData(page, mml, page.port[0], (byte)0xac, (byte)(f >> 8));
                    SOutData(page, mml, page.port[0], (byte)0xa8, (byte)f);
                }
                if ((page.slots & 1) != 0)
                {
                    int f;
                    if (pw.slotFixedFnum[0] == -1) f = freq + page.slotDetune[0];
                    else f = pw.slotFixedFnum[0] + page.slotDetune[0];
                    SOutData(page, mml, page.port[0], (byte)0xad, (byte)(f >> 8));
                    SOutData(page, mml, page.port[0], (byte)0xa9, (byte)f);
                }
                if ((page.slots & 2) != 0)
                {
                    int f;
                    if (pw.slotFixedFnum[1] == -1) f = freq + page.slotDetune[1];
                    else f = pw.slotFixedFnum[1] + page.slotDetune[1];
                    SOutData(page, mml, page.port[0], (byte)0xae, (byte)(f >> 8));
                    SOutData(page, mml, page.port[0], (byte)0xaa, (byte)f);
                }
            }
            else
            {
                int n;
                if (!(page.chip is YM2609))
                {
                    n = (page.chip is YM2203) ? 0 : 3;
                    if (page.ch >= n + 3 && page.ch < n + 6)
                    {
                        return;
                    }
                }
                else
                {
                    n = 9;
                    if (page.ch >= 12 && page.ch < 18)
                    {
                        return;
                    }
                }
                if ((page.chip is YM2612X) && page.ch >= 9 && page.ch <= 11)
                {
                    return;
                }
                if (page.ch < n + 3)
                {
                    if (page.pcm) return;

                    int vch;
                    byte[] port;
                    GetPortVch(page, out port, out vch);

                    SOutData(page, mml, port, (byte)(0xa4 + vch), (byte)((freq & 0xff00) >> 8));
                    SOutData(page, mml, port, (byte)(0xa0 + vch), (byte)(freq & 0xff));
                }
            }
        }

        public void OutFmSetForcedFnum(partPage page, MML mml, int num)
        {
            int freq;
            freq = num & 0xffff;

            if (freq == page.spg.freq) return;

            page.spg.freq = freq;

            if (page.chip.lstPartWork[2].apg.Ch3SpecialMode && page.Type == enmChannelType.FMOPNex)
            {
                if ((page.slots & 8) != 0)
                {
                    int f = freq + page.slotDetune[3];
                    SOutData(page, mml, page.port[0], (byte)0xa6, (byte)(f >> 8));
                    SOutData(page, mml, page.port[0], (byte)0xa2, (byte)f);
                }
                if ((page.slots & 4) != 0)
                {
                    int f = freq + page.slotDetune[2];
                    SOutData(page, mml, page.port[0], (byte)0xac, (byte)(f >> 8));
                    SOutData(page, mml, page.port[0], (byte)0xa8, (byte)f);
                }
                if ((page.slots & 1) != 0)
                {
                    int f = freq + page.slotDetune[0];
                    SOutData(page, mml, page.port[0], (byte)0xad, (byte)(f >> 8));
                    SOutData(page, mml, page.port[0], (byte)0xa9, (byte)f);
                }
                if ((page.slots & 2) != 0)
                {
                    int f = freq + page.slotDetune[1];
                    SOutData(page, mml, page.port[0], (byte)0xae, (byte)(f >> 8));
                    SOutData(page, mml, page.port[0], (byte)0xaa, (byte)f);
                }
            }
            else
            {
                int n;
                if (!(page.chip is YM2609))
                {
                    n = (page.chip is YM2203) ? 0 : 3;
                    if (page.ch >= n + 3 && page.ch < n + 6)
                    {
                        return;
                    }
                }
                else
                {
                    n = 9;
                    if (page.ch >= 12 && page.ch < 18)
                    {
                        return;
                    }
                }
                if ((page.chip is YM2612X) && page.ch >= 9 && page.ch <= 11)
                {
                    return;
                }
                if (page.ch < n + 3)
                {
                    if (page.pcm) return;

                    int vch;
                    byte[] port;
                    GetPortVch(page, out port, out vch);

                    SOutData(page, mml, port, (byte)(0xa4 + vch), (byte)((freq & 0xff00) >> 8));
                    SOutData(page, mml, port, (byte)(0xa0 + vch), (byte)(freq & 0xff));
                }
            }
        }

        public void OutFmKeyOn(partPage page, MML mml)
        {
            SetDummyData(page, mml);

            int n = (page.chip is YM2203) ? 0 : 3;

            if (page.chip is YM2612X && (page.ch > 8 || page.ch == 5) && page.chip.lstPartWork[5].pg[0].pcm)
            {
                ((YM2612X)page.chip).OutYM2612XPcmKeyON(mml, page);
                return;
            }

            if (!page.pcm)//FM KeyON
            {
                OutFmKeyOnMain(page, mml, n);
                return;
            }

            OutFmKeyOnPCMSide(page, mml);
        }

        private void OutFmKeyOnMain(partPage page, MML mml, int n)
        {
            if (!(page.chip is YM2609))
            {
                if (page.chip.lstPartWork[2].apg.Ch3SpecialMode && page.Type == enmChannelType.FMOPNex)
                {
                    page.Ch3SpecialModeKeyOn = true;

                    int slot = (page.chip.lstPartWork[2].apg.Ch3SpecialModeKeyOn ? page.chip.lstPartWork[2].apg.slots : 0x0)
                        | (page.chip.lstPartWork[n + 3].apg.Ch3SpecialModeKeyOn ? page.chip.lstPartWork[n + 3].apg.slots : 0x0)
                        | (page.chip.lstPartWork[n + 4].apg.Ch3SpecialModeKeyOn ? page.chip.lstPartWork[n + 4].apg.slots : 0x0)
                        | (page.chip.lstPartWork[n + 5].apg.Ch3SpecialModeKeyOn ? page.chip.lstPartWork[n + 5].apg.slots : 0x0);

                    if (page.chip is YM2612X)
                    {
                        outDatum od = new outDatum();
                        od.val = (byte)((slot << 4) + 2);
                        if (mml != null)
                        {
                            od.type = mml.type;
                            if (mml.line != null && mml.line.Lp != null)
                            {
                                od.linePos = new LinePos(
                                    mml.line.Lp.document,
                                    mml.line.Lp.srcMMLID,
                                    mml.line.Lp.row,
                                    mml.line.Lp.col,
                                    mml.line.Lp.length,
                                    mml.line.Lp.part,
                                    mml.line.Lp.chip,
                                    mml.line.Lp.chipIndex,
                                    mml.line.Lp.chipNumber,
                                    mml.line.Lp.ch);
                            }
                        }

                        if (page.RR15sw)
                        {
                            if (page.RR15 == 0) SetRR15(page, mml, slot);
                            ResetRR15(page, mml, slot);
                        }
                        parent.xgmKeyOnData.Add(od);
                    }
                    else
                    {
                        if (!page.keyOnDelay.sw)
                        {
                            if (page.RR15sw)
                            {
                                if (page.RR15 == 0) SetRR15(page, mml, slot);
                                ResetRR15(page, mml, slot);
                            }
                            SOutData(page, mml, page.port[0], 0x28, (byte)((slot << 4) + (2 & 7)));
                        }
                        else
                        {
                            SOutData(page, mml, page.port[0], 0x28, (byte)((page.keyOnDelay.keyOn << 4) + (2 & 7)));
                            page.keyOnDelay.beforekeyOn = page.keyOnDelay.keyOn;
                        }
                    }
                }
                else
                {
                    if (page.ch >= 0 && page.ch < n + 3)
                    {
                        byte vch = (byte)((page.ch > 2) ? page.ch + 1 : page.ch);
                        if (page.chip is YM2612X)
                        {
                            outDatum od = new outDatum();
                            if (!page.keyOnDelay.sw)
                            {
                                od.val = (byte)((page.slots << 4) + (vch & 7));
                            }
                            else
                            {
                                od.val = (byte)((page.keyOnDelay.keyOn << 4) + (vch & 7));
                                page.keyOnDelay.beforekeyOn = 0xff;
                            }
                            if (mml != null)
                            {
                                od.type = mml.type;
                                if (mml.line != null && mml.line.Lp != null)
                                {
                                    od.linePos = new LinePos(
                                        mml.line.Lp.document,
                                        mml.line.Lp.srcMMLID,
                                        mml.line.Lp.row,
                                        mml.line.Lp.col,
                                        mml.line.Lp.length,
                                        mml.line.Lp.part,
                                        mml.line.Lp.chip,
                                        mml.line.Lp.chipIndex,
                                        mml.line.Lp.chipNumber,
                                        mml.line.Lp.ch);
                                }
                            }

                            if (page.RR15sw)
                            {
                                if (page.RR15 == 0) SetRR15(page, mml, page.slots);
                                ResetRR15(page, mml, page.slots);
                            }
                            parent.xgmKeyOnData.Add(od);
                        }
                        else
                        {
                            //key on
                            if (!page.keyOnDelay.sw)
                            {
                                if (page.RR15sw)
                                {
                                    if (page.RR15 == 0) SetRR15(page, mml, page.slots);
                                    ResetRR15(page, mml, page.slots);
                                }
                                SOutData(page, mml, page.port[0], 0x28, (byte)((page.slots << 4) + (vch & 7)));
                            }
                            else
                            {
                                SOutData(page, mml, page.port[0], 0x28, (byte)((page.keyOnDelay.keyOn << 4) + (vch & 7)));
                                page.keyOnDelay.beforekeyOn = page.keyOnDelay.keyOn;
                            }
                        }
                    }
                }
            }
            else
            {
                if ((page.ch == 2 || page.ch == 12 || page.ch == 13 || page.ch == 14) && page.chip.lstPartWork[2].apg.Ch3SpecialMode)
                {
                    page.Ch3SpecialModeKeyOn = true;

                    int slot = (page.chip.lstPartWork[2].apg.Ch3SpecialModeKeyOn ? page.chip.lstPartWork[2].apg.slots : 0x0)
                        | (page.chip.lstPartWork[12].apg.Ch3SpecialModeKeyOn ? page.chip.lstPartWork[12].apg.slots : 0x0)
                        | (page.chip.lstPartWork[13].apg.Ch3SpecialModeKeyOn ? page.chip.lstPartWork[13].apg.slots : 0x0)
                        | (page.chip.lstPartWork[14].apg.Ch3SpecialModeKeyOn ? page.chip.lstPartWork[14].apg.slots : 0x0);

                    outDatum od = new outDatum();
                    od.val = (byte)((slot << 4) + 2);
                    if (mml != null)
                    {
                        od.type = mml.type;
                        if (mml.line != null && mml.line.Lp != null)
                        {
                            od.linePos = new LinePos(
                                mml.line.Lp.document,
                                mml.line.Lp.srcMMLID,
                                mml.line.Lp.row,
                                mml.line.Lp.col,
                                mml.line.Lp.length,
                                mml.line.Lp.part,
                                mml.line.Lp.chip,
                                mml.line.Lp.chipIndex,
                                mml.line.Lp.chipNumber,
                                mml.line.Lp.ch);
                        }
                    }
                    ((YM2609)page.chip).opna20x028KeyOnData.Add(od);

                }
                else if ((page.ch == 8 || page.ch == 15 || page.ch == 16 || page.ch == 17) && page.chip.lstPartWork[8].apg.Ch3SpecialMode)
                {
                    page.Ch3SpecialModeKeyOn = true;

                    int slot = (page.chip.lstPartWork[8].apg.Ch3SpecialModeKeyOn ? page.chip.lstPartWork[8].apg.slots : 0x0)
                        | (page.chip.lstPartWork[15].apg.Ch3SpecialModeKeyOn ? page.chip.lstPartWork[15].apg.slots : 0x0)
                        | (page.chip.lstPartWork[16].apg.Ch3SpecialModeKeyOn ? page.chip.lstPartWork[16].apg.slots : 0x0)
                        | (page.chip.lstPartWork[17].apg.Ch3SpecialModeKeyOn ? page.chip.lstPartWork[17].apg.slots : 0x0);

                    outDatum od = new outDatum();
                    if (!page.keyOnDelay.sw)
                    {
                        od.val = (byte)((slot << 4) + 2);
                    }
                    else
                    {
                        od.val = (byte)((page.keyOnDelay.keyOn << 4) + 2);
                        page.keyOnDelay.beforekeyOn = 0xff;
                    }
                    if (mml != null)
                    {
                        od.type = mml.type;
                        if (mml.line != null && mml.line.Lp != null)
                        {
                            od.linePos = new LinePos(
                                mml.line.Lp.document,
                                mml.line.Lp.srcMMLID,
                                mml.line.Lp.row,
                                mml.line.Lp.col,
                                mml.line.Lp.length,
                                mml.line.Lp.part,
                                mml.line.Lp.chip,
                                mml.line.Lp.chipIndex,
                                mml.line.Lp.chipNumber,
                                mml.line.Lp.ch);
                        }
                    }
                    ((YM2609)page.chip).opna20x228KeyOnData.Add(od);
                }
                else
                {
                    if (page.ch >= 0 && page.ch < 6)
                    {
                        byte vch = (byte)((page.ch > 2) ? page.ch + 1 : page.ch);
                        //key on
                        outDatum od = new outDatum();
                        if (!page.keyOnDelay.sw)
                        {
                            od.val = (byte)((page.slots << 4) + (vch & 7));
                        }
                        else
                        {
                            od.val = (byte)((page.keyOnDelay.keyOn << 4) + (vch & 7));
                            page.keyOnDelay.beforekeyOn = 0xff;
                        }
                        if (mml != null)
                        {
                            od.type = mml.type;
                            if (mml.line != null && mml.line.Lp != null)
                            {
                                od.linePos = new LinePos(
                                    mml.line.Lp.document,
                                    mml.line.Lp.srcMMLID,
                                    mml.line.Lp.row,
                                    mml.line.Lp.col,
                                    mml.line.Lp.length,
                                    mml.line.Lp.part,
                                    mml.line.Lp.chip,
                                    mml.line.Lp.chipIndex,
                                    mml.line.Lp.chipNumber,
                                    mml.line.Lp.ch);
                            }
                        }
                        ((YM2609)page.chip).opna20x028KeyOnData.Add(od);
                    }
                    else if (page.ch >= 6 && page.ch < 12)
                    {
                        byte vch = (byte)(page.ch - 6);
                        vch = (byte)(((vch > 2) ? (vch + 1) : vch));
                        //key on
                        outDatum od = new outDatum();
                        if (!page.keyOnDelay.sw)
                        {
                            od.val = (byte)((page.slots << 4) + (vch & 7));
                        }
                        else
                        {
                            od.val = (byte)((page.keyOnDelay.keyOn << 4) + (vch & 7));
                            page.keyOnDelay.beforekeyOn = 0xff;
                        }
                        if (mml != null)
                        {
                            od.type = mml.type;
                            if (mml.line != null && mml.line.Lp != null)
                            {
                                od.linePos = new LinePos(
                                    mml.line.Lp.document,
                                    mml.line.Lp.srcMMLID,
                                    mml.line.Lp.row,
                                    mml.line.Lp.col,
                                    mml.line.Lp.length,
                                    mml.line.Lp.part,
                                    mml.line.Lp.chip,
                                    mml.line.Lp.chipIndex,
                                    mml.line.Lp.chipNumber,
                                    mml.line.Lp.ch);
                            }
                        }
                        ((YM2609)page.chip).opna20x228KeyOnData.Add(od);
                    }
                }
            }
        }

        protected virtual void SetRR15(partPage page, MML mml, int slot)
        {
            for (byte ope = 0; ope < 4; ope++)
            {
                if ((slot & (1 << ope)) == 0) continue;
                //if (!parent.instFM.ContainsKey(page.instrument)) continue;
                ((ClsOPN)page.chip).OutFmSetSlRr(mml, page, ope,
                    page.voice[partPage.voiceWidth + ope * partPage.voiceWidth + 4],// parent.instFM[page.instrument].Item2[ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 5],
                    15);
                //((ClsOPN)page.chip).OutFmSetTl(mml, page, ope,
                //    127);
            }
        }

        protected virtual void ResetRR15(partPage page, MML mml, int slot)
        {
            //if (!parent.instFM.ContainsKey(page.instrument)) return;
            for (byte ope = 0; ope < 4; ope++)
            {
                if ((slot & (1 << ope)) == 0) continue;
                ((ClsOPN)page.chip).OutFmSetSlRr(mml, page, ope,
                    page.voice[partPage.voiceWidth + ope * partPage.voiceWidth + 4],//parent.instFM[page.instrument].Item2[ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 5],
                    page.voice[partPage.voiceWidth + ope * partPage.voiceWidth + 3]);// parent.instFM[page.instrument].Item2[ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 4]);
            }
            //page.spg.beforeVolume = -1;
            //SetFmVolume(page, mml);
            //OutFmSetTL(page, mml, 0, 0, 0, 0, slot, page.instrument);
        }

        private void OutFmKeyOnPCMSide(partPage page, MML mml)
        {
            if (page.isPcmMap)
            {
                int nt = Const.NOTE.IndexOf(page.noteCmd);
                int arpNote = page.arpFreqMode ? 0 : page.arpDelta;
                int f = page.octaveNow * 12 + nt + page.shift + page.keyShift + arpNote;
                if (parent.instPCMMap.ContainsKey(page.pcmMapNo))
                {
                    if (parent.instPCMMap[page.pcmMapNo].ContainsKey(f))
                    {
                        page.instrument = parent.instPCMMap[page.pcmMapNo][f];
                    }
                    else
                    {
                        msgBox.setErrMsg(string.Format(msg.get("E10025"), page.octaveNow, page.noteCmd, page.shift + page.keyShift + arpNote), mml.line.Lp);
                        return;
                    }
                }
                else
                {
                    msgBox.setErrMsg(string.Format(msg.get("E10024"), page.pcmMapNo), mml.line.Lp);
                    return;
                }
            }

            if (!parent.instPCM.ContainsKey(page.instrument)) return;

            float m = Const.pcmMTbl[page.pcmNote] * (float)Math.Pow(2, (page.pcmOctave - 4));
            page.pcmBaseFreqPerFreq = Information.VGM_SAMPLE_PER_SECOND / ((float)parent.instPCM[page.instrument].Item2.freq * m);
            page.pcmFreqCountBuffer = 0.0f;
            long p = parent.instPCM[page.instrument].Item2.stAdr;
            if (parent.info.Version == 1.51f)
            {
                byte[] cmd;
                if (parent.info.format == enmFormat.ZGM)
                {
                    if (parent.ChipCommandSize == 2) cmd = new byte[] { 0x40, 0x00 };
                    else cmd = new byte[] { 0x40 };
                }
                else cmd = new byte[] { 0xe0 };
                SOutData(page,
                    mml, cmd
                    , (byte)(p & 0xff)
                    , (byte)((p & 0xff00) / 0x100)
                    , (byte)((p & 0xff0000) / 0x10000)
                    , (byte)((p & 0xff000000) / 0x10000)
                    );
            }
            else
            {
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
                if (!page.spg.streamSetup)
                {
                    parent.newStreamID++;
                    page.spg.streamID = parent.newStreamID;

                    if (parent.info.format == enmFormat.ZGM)
                    {
                        if (parent.ChipCommandSize == 2) cmd = new byte[] {
                            0x30, 0x00
                            , (byte)page.spg.streamID
                            , (byte)(0x02 + (page.chipNumber!=0 ? 0x80 : 0x00))
                            , 0
                        };
                        else cmd = new byte[] {
                            0x30
                            , (byte)page.spg.streamID
                            , (byte)(0x02 + (page.chipNumber!=0 ? 0x80 : 0x00))
                        };
                    }
                    else cmd = new byte[] {
                        0x90
                        , (byte)page.spg.streamID
                        , (byte)(0x02 + (page.chipNumber!=0 ? 0x80 : 0x00))
                    };

                    SOutData(page,
                        mml,
                        // setup stream control
                        cmd
                        , 0x00
                        , 0x2a
                        );
                    if (parent.info.format == enmFormat.ZGM)
                    {
                        if (parent.ChipCommandSize == 2) cmd = new byte[] { 0x31, 0x00 };
                        else cmd = new byte[] { 0x31 };
                    }
                    else cmd = new byte[] { 0x91 };
                    SOutData(page,
                        mml
                        // set stream data
                        , cmd
                        , (byte)page.spg.streamID
                        , 0x00
                        , 0x01
                        , 0x00
                        );

                    page.spg.streamSetup = true;
                }

                if (page.spg.streamFreq != f)
                {
                    if (parent.info.format == enmFormat.ZGM)
                    {
                        if (parent.ChipCommandSize == 2) cmd = new byte[] { 0x32, 0x00 };
                        else cmd = new byte[] { 0x32 };
                    }
                    else cmd = new byte[] { 0x92 };
                    //Set Stream Frequency
                    SOutData(page,
                        mml, cmd
                        , (byte)page.spg.streamID
                        , (byte)(f & 0xff)
                        , (byte)((f & 0xff00) / 0x100)
                        , (byte)((f & 0xff0000) / 0x10000)
                        , (byte)((f & 0xff000000) / 0x10000)
                        );

                    page.spg.streamFreq = f;
                }

                if (parent.info.format == enmFormat.ZGM)
                {
                    if (parent.ChipCommandSize == 2) cmd = new byte[] { 0x33, 0x00 };
                    else cmd = new byte[] { 0x33 };
                }
                else cmd = new byte[] { 0x93 };
                //Start Stream
                SOutData(page,
                    mml,
                    cmd
                    , (byte)page.spg.streamID

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
            }

            if (parent.instPCM[page.instrument].Item2.status != enmPCMSTATUS.ERROR)
            {
                parent.instPCM[page.instrument].Item2.status = enmPCMSTATUS.USED;
            }
        }

        public virtual void SetFmFNum(partPage page, MML mml)
        {
            if (page.noteCmd == (char)0)
            {
                return;
            }

            if (page.forcedFnum != -1)
            {
                SetFmForcedFNum(page, mml);
                return;
            }

            SetVolume(page, mml);

            int[] ftbl = page.chip.FNumTbl[0];
            int arpNote = page.arpFreqMode ? 0 : page.arpDelta;
            int arpFreq = page.arpFreqMode ? page.arpDelta : 0;
            int f;
            f = GetFmFNum(ftbl, page.octaveNow, page.noteCmd, page.shift + page.keyShift + page.toneDoublerKeyShift + arpNote, page.pitchShift);//
            if (page.bendWaitCounter != -1)
            {
                f = page.bendFnum;
            }
            int o = (f & 0xf000) / 0x1000;
            f &= 0xfff;

            f += page.detune;
            f += arpFreq;
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

            while (f < ftbl[0])
            {
                if (o == 1)
                {
                    break;
                }
                o--;
                f = ftbl[0] * 2 - (ftbl[0] - f);
            }
            while (f >= ftbl[0] * 2)
            {
                if (o == 8)
                {
                    break;
                }
                o++;
                f = f - ftbl[0] * 2 + ftbl[0];
            }
            f = Common.CheckRange(f, 0, 0x7ff);
            OutFmSetFnum(page, mml, o, f);
        }

        public void SetFmForcedFNum(partPage page, MML mml)
        {
            int[] ftbl = page.chip.FNumTbl[0];

            int f = page.forcedFnum & 0xffff;

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
            f = Common.CheckRange(f, 0, 0xffff);
            OutFmSetForcedFnum(page, mml, f);
        }

        public int GetFmFNum(int[] ftbl, int octave, char noteCmd, int shift,int pitchShift)
        {
            int o = octave;
            int n = Const.NOTE.IndexOf(noteCmd) + shift;

            o += n / 12;
            n %= 12;
            if (n < 0)
            {
                n += 12;
                o--;
            }

            int oo = o;
            int mul = 0;
            o = Common.CheckRange(o, 1, 8);
            if (oo < o || oo > o) mul = oo - o;

            //if (n >= 0)
            //{
            //    o += n / 12;
            //    o = Common.CheckRange(o, 1, 8);
            //    n %= 12;
            //}
            //else
            //{
            //    o += n / 12 - ((n % 12 == 0) ? 0 : 1);
            //    o = Common.CheckRange(o, 1, 8);
            //    n %= 12;
            //    if (n < 0) { n += 12; }
            //}

            int f = ftbl[n];
            if (mul < 0) f >>= -mul;
            else f <<= mul;
            f += pitchShift;

            return (f & 0xfff) + (o & 0xf) * 0x1000;
        }

        public override int GetFNum(partPage page, MML mml, int octave, char cmd, int shift, int pitchShift)
        {
            if (page.Type == enmChannelType.FMOPN || page.Type == enmChannelType.FMOPNex)
            {
                return GetFmFNum(FNumTbl[0], octave, cmd, shift, pitchShift);
            }
            if (page.Type == enmChannelType.SSG)
            {
                return GetSsgFNum(page, mml, octave, cmd, shift, pitchShift);
            }
            return 0;
        }

        public override void GetFNumAtoB(partPage page, MML mml
            , out int a, int aOctaveNow, char aCmd, int aShift, int aPitchShift
            , out int b, int bOctaveNow, char bCmd, int bShift, int bPitchShift
            , int dir)
        {
            a = GetFNum(page, mml, aOctaveNow, aCmd, aShift, aPitchShift);
            b = GetFNum(page, mml, bOctaveNow, bCmd, bShift, bPitchShift);

            int oa = (a & 0xf000) / 0x1000;
            int ob = (b & 0xf000) / 0x1000;
            if (oa != ob)
            {
                if (((a-aPitchShift) & 0xfff) == FNumTbl[0][0])
                {
                    oa += Math.Sign(ob - oa);
                    a = (a & 0xfff) * 2 + oa * 0x1000;
                }
                else if (((b-bPitchShift) & 0xfff) == FNumTbl[0][0])
                {
                    ob += Math.Sign(oa - ob);
                    b = (b & 0xfff) * ((dir > 0) ? 2 : 1) + ob * 0x1000;
                }
            }
        }


        public void SetFmVolume(partPage page, MML mml)
        {
            int vol = page.volume;

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

            if(page.varpeggioMode)
            {
                vol += page.varpDelta;
            }

            //if (page.spg.beforeVolume != vol)
            {
                //if (parent.instFM.ContainsKey(page.instrument))
                {
                    OutFmSetVolume(page, mml, vol);
                }
                page.spg.beforeVolume = vol;
            }
        }

        public void SetFmTL(partPage page, MML mml)
        {
            int tl1 = page.tlDelta1;
            int tl2 = page.tlDelta2;
            int tl3 = page.tlDelta3;
            int tl4 = page.tlDelta4;
            int slot = 0;

            for (int lfo = 0; lfo < 4; lfo++)
            {
                if (!page.lfo[lfo].sw)
                {
                    continue;
                }
                if (page.lfo[lfo].type != eLfoType.Wah)
                {
                    continue;
                }

                if ((page.lfo[lfo].slot & 1) != 0) { tl1 += page.lfo[lfo].value + page.lfo[lfo].param[1 + 6]; slot |= 1; }
                if ((page.lfo[lfo].slot & 2) != 0) { tl2 += page.lfo[lfo].value + page.lfo[lfo].param[1 + 6]; slot |= 2; }
                if ((page.lfo[lfo].slot & 4) != 0) { tl3 += page.lfo[lfo].value + page.lfo[lfo].param[1 + 6]; slot |= 4; }
                if ((page.lfo[lfo].slot & 8) != 0) { tl4 += page.lfo[lfo].value + page.lfo[lfo].param[1 + 6]; slot |= 8; }
            }

            if (page.spg.beforeTlDelta1 != tl1 || page.spg.beforeTlDelta2 != tl2 || page.spg.beforeTlDelta3 != tl3 || page.spg.beforeTlDelta4 != tl4)
            {
                //if (parent.instFM.ContainsKey(page.instrument))
                {
                    OutFmSetTL(page, mml, tl1, tl2, tl3, tl4, slot);//, page.instrument);
                }
                page.spg.beforeTlDelta1 = tl1;
                page.spg.beforeTlDelta2 = tl2;
                page.spg.beforeTlDelta3 = tl3;
                page.spg.beforeTlDelta4 = tl4;
            }
        }

        public override void SetKeyOff(partPage page, MML mml)
        {
        }

        public override void SetVolume(partPage page, MML mml)
        {
            if (page.Type == enmChannelType.FMOPN
                || page.Type == enmChannelType.FMOPNex //効果音モード対応チャンネル
                || (page.Type == enmChannelType.FMPCM && !page.pcm) //OPN2PCMチャンネル
                || (page.Type == enmChannelType.FMPCMex && !page.pcm) //OPN2XPCMチャンネル
                )
            {
                SetFmVolume(page, mml);
                SetFmTL(page, mml);
            }
            else if (page.Type == enmChannelType.SSG)
            {
                SetSsgVolume(page, mml);
            }
        }

        public override void SetLfoAtKeyOn(partPage page, MML mml)
        {
            for (int lfo = 0; lfo < 4; lfo++)
            {
                clsLfo pl = page.lfo[lfo];
                if (!pl.sw)
                    continue;
                if (pl.type == eLfoType.Hardware)
                    continue;
                int w = 0;
                if (pl.type == eLfoType.Wah) w = 1;

                if (pl.param[w+5] != 1)
                    continue;

                pl.isEnd = false;
                pl.value = (pl.param[w + 0] == 0) ? pl.param[w + 6] : 0;//ディレイ中は振幅補正は適用されない
                pl.waitCounter = pl.param[w + 0];
                pl.direction = pl.param[w + 2] < 0 ? -1 : 1;
                pl.depthWaitCounter = pl.param[w + 7];
                pl.depth = pl.param[w + 3];
                pl.depthV2 = pl.param[w + 2];

                if (pl.type == eLfoType.Vibrato)
                {
                    if (page.Type == enmChannelType.FMOPN
                        || page.Type == enmChannelType.FMOPNex)
                        SetFmFNum(page, mml);
                    else if (page.Type == enmChannelType.SSG)
                        SetSsgFNum(page, mml);

                }

                if (pl.type == eLfoType.Tremolo)
                {
                    page.beforeVolume = -1;
                    if (page.Type == enmChannelType.FMOPN
                        || page.Type == enmChannelType.FMOPNex)
                        SetFmVolume(page, mml);
                    else if (page.Type == enmChannelType.SSG)
                        SetSsgVolume(page, mml);
                }

                if (pl.type == eLfoType.Wah)
                {
                    page.beforeVolume = -1;
                    SetFmTL(page, mml);
                }

            }
        }

        public override void SetToneDoubler(partPage page, MML mml)
        {
            if (page.Type != enmChannelType.FMOPN && page.ch != 2)
            {
                return;
            }

            int i = page.instrument;
            if (i < 0) return;

            page.toneDoublerKeyShift = 0;
            //byte[] instFM = parent.instFM[i].Item2;
            //if (instFM == null || instFM.Length < 1) return;
            Note note = (Note)mml.args[0];

            if (page.TdA == -1)
            {
                //resetToneDoubler
                if (page.op1ml != page.voice[partPage.voiceWidth + 0 * partPage.voiceWidth + 7])// + instFM[0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8])
                {
                    //OutFmSetDtMl(mml, page, 0, instFM[0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instFM[0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8]);
                    //page.op1ml = instFM[0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
                    OutFmSetDtMl(mml, page, 0, page.voice[partPage.voiceWidth + 0 * partPage.voiceWidth + 8], page.voice[partPage.voiceWidth + 0 * partPage.voiceWidth + 7]);
                    page.op1ml = page.voice[partPage.voiceWidth + 0 * partPage.voiceWidth + 7];
                }
                if (page.op2ml != page.voice[partPage.voiceWidth + 1 * partPage.voiceWidth + 7])
                {
                    OutFmSetDtMl(mml, page, 1, page.voice[partPage.voiceWidth + 1 * partPage.voiceWidth + 8], page.voice[partPage.voiceWidth + 1 * partPage.voiceWidth + 7]);
                    page.op2ml = page.voice[partPage.voiceWidth + 1 * partPage.voiceWidth + 7];
                }
                if (page.op3ml != page.voice[partPage.voiceWidth + 2 * partPage.voiceWidth + 7])
                {
                    OutFmSetDtMl(mml, page, 2, page.voice[partPage.voiceWidth + 2 * partPage.voiceWidth + 8], page.voice[partPage.voiceWidth + 2 * partPage.voiceWidth + 7]);
                    page.op3ml = page.voice[partPage.voiceWidth + 2 * partPage.voiceWidth + 7];
                }
                if (page.op4ml != page.voice[partPage.voiceWidth + 3 * partPage.voiceWidth + 7])
                {
                    OutFmSetDtMl(mml, page, 3, page.voice[partPage.voiceWidth + 3 * partPage.voiceWidth + 8], page.voice[partPage.voiceWidth + 3 * partPage.voiceWidth + 7]);
                    page.op4ml = page.voice[partPage.voiceWidth + 3 * partPage.voiceWidth + 7];
                }
            }
            else
            {
                //setToneDoubler
                int oct = page.octaveNow;
                foreach (MML octMml in note.tDblOctave)
                {
                    switch (octMml.type)
                    {
                        case enmMMLType.Octave:
                            oct = (int)octMml.args[0];
                            break;
                        case enmMMLType.OctaveUp:
                            oct++;
                            break;
                        case enmMMLType.OctaveDown:
                            oct--;
                            break;
                    }
                }
                oct = Common.CheckRange(oct, 1, 8);
                page.octaveNew = oct;
                int arpNote = page.arpFreqMode ? 0 : page.arpDelta;
                int TdB = oct * 12 + Const.NOTE.IndexOf(note.tDblCmd) + note.tDblShift + page.keyShift + arpNote;
                int s = TdB - page.TdA;// - TdB;
                int us = Math.Abs(s);
                int n = page.toneDoubler;
                clsToneDoubler instToneDoubler = parent.instToneDoubler[n];
                if (us >= instToneDoubler.lstTD.Count)
                {
                    return;
                }

                page.toneDoublerKeyShift = ((s < 0) ? s : 0) + instToneDoubler.lstTD[us].KeyShift;

                if (page.op1ml != instToneDoubler.lstTD[us].OP1ML)
                {
                    //OutFmSetDtMl(mml, page, 0, instFM[0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instToneDoubler.lstTD[us].OP1ML);
                    OutFmSetDtMl(mml, page, 0, page.voice[partPage.voiceWidth + 0 * partPage.voiceWidth + 8], instToneDoubler.lstTD[us].OP1ML);
                    page.op1ml = instToneDoubler.lstTD[us].OP1ML;
                }
                if (page.op2ml != instToneDoubler.lstTD[us].OP2ML)
                {
                    OutFmSetDtMl(mml, page, 1, page.voice[partPage.voiceWidth + 1 * partPage.voiceWidth + 8], instToneDoubler.lstTD[us].OP2ML);
                    page.op2ml = instToneDoubler.lstTD[us].OP2ML;
                }
                if (page.op3ml != instToneDoubler.lstTD[us].OP3ML)
                {
                    OutFmSetDtMl(mml, page, 2, page.voice[partPage.voiceWidth + 2 * partPage.voiceWidth + 8], instToneDoubler.lstTD[us].OP3ML);
                    page.op3ml = instToneDoubler.lstTD[us].OP3ML;
                }
                if (page.op4ml != instToneDoubler.lstTD[us].OP4ML)
                {
                    OutFmSetDtMl(mml, page, 3, page.voice[partPage.voiceWidth + 3 * partPage.voiceWidth + 8], instToneDoubler.lstTD[us].OP4ML);
                    page.op4ml = instToneDoubler.lstTD[us].OP4ML;
                }

                //pw.ppg[pw.cpgNum].TdA = -1;
            }
        }

        public override int GetToneDoublerShift(partPage page, int octave, char noteCmd, int shift)
        {
            if (page.Type != enmChannelType.FMOPN && page.ch != 2)
            {
                return 0;
            }

            int i = page.instrument;
            if (page.TdA == -1)
            {
                return 0;
            }

            int TdB = octave * 12 + Const.NOTE.IndexOf(noteCmd) + shift;
            int s = page.TdA - TdB;
            int us = Math.Abs(s);
            int n = page.toneDoubler;
            if (us >= parent.instToneDoubler[n].lstTD.Count)
            {
                return 0;
            }

            return ((s < 0) ? s : 0) + parent.instToneDoubler[n].lstTD[us].KeyShift;
        }


        private void CmdY_ToneParamOPN(MML mml, byte adr, partPage page, byte op, byte dat)
        {
            int ch;
            if (page.Type == enmChannelType.FMOPNex) ch = 2;
            else if (page.Type == enmChannelType.FMOPN) ch = page.ch;
            else if (page.Type == enmChannelType.FMPCM) ch = page.ch;
            else if (page.Type == enmChannelType.FMPCMex) ch = page.ch;
            else return;

            int vch;
            byte[] port;
            GetPortVch(page, out port, out vch);
            if (page.Type == enmChannelType.FMOPNex)
            {
                if (!(page.chip is YM2609))
                {
                    vch = 2;
                    port = page.port[0];
                }
                else
                {
                    vch = 2;
                    if (page.ch == 2 || (page.ch >= 12 && page.ch <= 14)) port = page.port[0];
                    else if (page.ch == 8 || (page.ch >= 15 && page.ch <= 17)) port = page.port[2];
                }
            }

            op = (byte)(op == 1 ? 2 : (op == 2 ? 1 : op));

            adr += (byte)(vch + (op << 2));

            SOutData(page, mml, port, adr, dat);
        }

        private void CmdY_ToneParamOPN_FBAL(MML mml, partPage page, byte dat)
        {
            int ch;
            if (page.Type == enmChannelType.FMOPNex) ch = 2;
            else if (page.Type == enmChannelType.FMOPN) ch = page.ch;
            else return;

            int vch;
            byte[] port;
            GetPortVch(page, out port, out vch);

            byte adr = (byte)(0xb0 + vch);

            SOutData(page, mml, port, adr, dat);
        }


        public override void CmdNoiseToneMixer(partPage page, MML mml)
        {
            if (page.Type == enmChannelType.SSG)
            {
                int n = (int)mml.args[0];
                n = Common.CheckRange(n, 0, 3);
                if (page.pcm) page.beforeMixer = n;
                else page.mixer = n;
            }
        }

        public override void CmdNoise(partPage page, MML mml)
        {
            int n = (int)mml.args[0];
            if (mml.args.Count > 1)
            {
                if ((char)mml.args[1] == '>')
                {
                    n = page.noise - n;
                }
                if ((char)mml.args[1] == '<')
                {
                    n = page.noise + n;
                }
            }
            n = Common.CheckRange(n, 0, 63);

            //int ch = 0;
            //if (page.chip is YM2609)
            //{
            //    if (page.ch >= 18 && page.ch <= 20) ch = 18;
            //    if (page.ch >= 21 && page.ch <= 23) ch = 21;
            //    if (page.ch >= 24 && page.ch <= 26) ch = 24;
            //    if (page.ch >= 27 && page.ch <= 29) ch = 27;
            //}

            page.noise = n;
            ((ClsOPN)page.chip).OutSsgNoise(mml, page);
        }

        public override void CmdForcedFnum(partPage page, MML mml)
        {
            int n1 = (int)mml.args[0];
            int n2 = 0;
            if (n1 != 0) n2 = (int)mml.args[1];
            n1 = Common.CheckRange(n1, 0, 1);
            n2 = Common.CheckRange(n2, 0, 0xffff);

            page.forcedFnum = n1 == 0 ? -1 : n2;
        }

        public override void CmdInstrument(partPage page, MML mml)
        {
            SetDummyData(page, mml);
            char type;
            bool re = false;
            int n;
            if (mml.args[0] is bool)
            {
                type = (char)mml.args[1];
                re = true;
                n = (int)mml.args[2];
            }
            else
            {
                type = (char)mml.args[0];
                n = (int)mml.args[1];
            }

            if (type == 'I')
            {
                msgBox.setErrMsg(msg.get("E11003"), mml.line.Lp);
                return;
            }

            if (type == 'T')
            {
                n = Common.CheckRange(n, 0, 255);
                page.toneDoubler = n;
                return;
            }

            if (type == 'E')
            {
                SetEnvelopParamFromInstrument(page, n, re, mml);
                return;
            }

            if (page.Type == enmChannelType.SSG)
            {
                if (!page.pcm)
                {
                    SetEnvelopParamFromInstrument(page, n, re, mml);
                    return;
                }

                if (page.instrument == n) return;

                if (!parent.instPCM.ContainsKey(n))
                {
                    msgBox.setErrMsg(string.Format(msg.get("E11008"), n), mml.line.Lp);
                    return;
                }

                if (parent.instPCM[n].Item2.chip != enmChipType.YM2203
                    && parent.instPCM[n].Item2.chip != enmChipType.YM2608
                    && parent.instPCM[n].Item2.chip != enmChipType.YM2610B
                    && parent.instPCM[n].Item2.chip != enmChipType.YM2609)
                {
                    msgBox.setErrMsg(string.Format(msg.get("E11009"), n, _Name), mml.line.Lp);
                }

                if (re) page.instrument += n;
                else page.instrument = n;

                return;
            }

            if (re) n = page.instrument + n;
            n = Common.CheckRange(n, 0, 255);
            if (page.beforeInstrument == n) return;
            page.instrument = n;
            page.beforeInstrument = n;
            ((ClsOPN)page.chip).OutFmSetInstrument(page, mml, n, page.volume, type);
        }

        public override void CmdEnvelope(partPage page, MML mml)
        {

            base.CmdEnvelope(page, mml);

            if (!(mml.args[0] is string))
            {
                msgBox.setErrMsg(msg.get("E11004")
                    , mml.line.Lp);

                return;
            }

            string cmd = (string)mml.args[0];

            switch (cmd)
            {
                case "EOF":
                    if (page.Type == enmChannelType.SSG)
                    {
                        page.beforeVolume = -1;
                    }
                    break;
            }
        }

        public override void CmdExtendChannel(partPage page, MML mml)
        {
            string cmd = (string)mml.args[0];

            switch (cmd)
            {
                case "EX":
                    int n = (int)mml.args[1];
                    byte res = GetSlotBit(mml, n);
                    if (res != 0)
                    {
                        page.slotsEX = res;
                        if (page.Ch3SpecialMode)
                        {
                            page.slots = page.slotsEX;
                            page.spg.freq = -1;
                            page.beforeVolume = -1;
                        }
                    }
                    break;
                case "EXON":
                    page.Ch3SpecialMode = true;
                    ((ClsOPN)page.chip).OutOPNSetCh3SpecialMode(mml, page, true);
                    foreach (partWork p in page.chip.lstPartWork)
                    {
                        if (page.chip.chipType == enmChipType.YM2609)
                        {
                            if (page.ch == 2 || page.ch == 12 || page.ch == 13 || page.ch == 14)
                                if (p.apg.ch == 8 || p.apg.ch == 15 || p.apg.ch == 16 || p.apg.ch == 17)
                                    continue;

                            if (page.ch == 8 || page.ch == 15 || page.ch == 16 || page.ch == 17)
                                if (p.apg.ch == 2 || p.apg.ch == 12 || p.apg.ch == 13 || p.apg.ch == 14)
                                    continue;
                        }

                        if (p.apg.Type == enmChannelType.FMOPNex)
                        {
                            p.apg.Ch3SpecialMode = true;
                            p.apg.slots = 0;// p.apg.slotsEX;
                            p.apg.beforeVolume = -1;
                            p.apg.beforeFNum = -1;
                            //p.freq = -1;
                            p.apg.oldFreq = -1;
                            //SetFmFNum(p,mml);
                        }
                    }
                    break;
                case "EXOF":
                    page.Ch3SpecialMode = false;
                    ((ClsOPN)page.chip).OutOPNSetCh3SpecialMode(mml, page, false);
                    foreach (partWork p in page.chip.lstPartWork)
                    {
                        foreach (partPage pg in p.pg)
                        {
                            if (page.chip.chipType == enmChipType.YM2609)
                            {
                                if (page.ch == 2 || page.ch == 12 || page.ch == 13 || page.ch == 14)
                                    if (p.apg.ch == 8 || p.apg.ch == 15 || p.apg.ch == 16 || p.apg.ch == 17)
                                        continue;

                                if (page.ch == 8 || page.ch == 15 || page.ch == 16 || page.ch == 17)
                                    if (p.apg.ch == 2 || p.apg.ch == 12 || p.apg.ch == 13 || p.apg.ch == 14)
                                        continue;
                            }

                            if (pg.Type == enmChannelType.FMOPNex)
                            {
                                p.apg.Ch3SpecialMode = false;
                                pg.beforeVolume = -1;
                                pg.beforeFNum = -1;
                                pg.freq = -1;

                                if (pg.ch != 2 && (pg.chip.chipType != enmChipType.YM2609 || pg.ch != 8)) // 2 -> Ch3   8 -> OPNA2のCh9のこと
                                    pg.slots = 0;
                                else
                                {
                                    pg.slots = pg.slots4OP;
                                    SetKeyOff(pg, mml);
                                }

                                //SetFmFNum(p);
                            }
                        }
                    }
                    break;
                case "EXD":
                    page.slotDetune[0] = (int)mml.args[1];
                    page.slotDetune[1] = (int)mml.args[2];
                    page.slotDetune[2] = (int)mml.args[3];
                    page.slotDetune[3] = (int)mml.args[4];
                    break;
                case "EXF":
                    
                    partWork pw = page.chip.lstPartWork[2];
                    if (page.chip is YM2609)
                    {
                        if(page.ch == 8 || page.ch == 15 || page.ch == 16 || page.ch == 17)
                        {
                            pw = page.chip.lstPartWork[8];
                        }
                    }

                    pw.slotFixedFnum[0] = -1;
                    pw.slotFixedFnum[1] = -1;
                    pw.slotFixedFnum[2] = -1;
                    pw.slotFixedFnum[3] = -1;
                    if (mml.args.Count > 1) pw.slotFixedFnum[0] = (int)mml.args[1];
                    if (mml.args.Count > 2) pw.slotFixedFnum[1] = (int)mml.args[2];
                    if (mml.args.Count > 3) pw.slotFixedFnum[2] = (int)mml.args[3];
                    if (mml.args.Count > 4) pw.slotFixedFnum[3] = (int)mml.args[4];

                    break;
            }
        }

        public byte GetSlotBit(MML mml,int n)
        {
            byte res = 0;
            while (n % 10 != 0)
            {
                if (n % 10 > 0 && n % 10 < 5)
                {
                    res += (byte)(1 << (n % 10 - 1));
                }
                else
                {
                    msgBox.setErrMsg(string.Format(msg.get("E11013"), n), mml == null ? null : mml.line.Lp);
                    break;
                }
                n /= 10;
            }
            return res;
        }

        public override void CmdVOperator(partPage page, MML mml)
        {
            int val = (int)mml.args[0];
            page.voperator = val;
        }

        public override void CmdVolume(partPage page, MML mml)
        {
            base.CmdVolume(page, mml);
        }

        public override void CmdRelativeVolumeSetting(partPage page, MML mml)
        {
            if (relVol.ContainsKey(page.ch))
                relVol.Remove(page.ch);

            relVol.Add(page.ch, (int)mml.args[0]);
        }

        public override int GetDefaultRelativeVolume(partPage page, MML mml)
        {
            if (relVol.ContainsKey(page.ch)) return relVol[page.ch];

            return 1;
        }


        public override void CmdY(partPage page, MML mml)
        {
            if (mml.args[0] is string toneparamName)
            {
                byte op = (byte)(int)mml.args[1];
                byte dat = (byte)(int)mml.args[2];

                switch (toneparamName)
                {
                    case "DTML":
                        CmdY_ToneParamOPN(mml, 0x30, page, op, dat);
                        break;
                    case "TL":
                        CmdY_ToneParamOPN(mml, 0x40, page, op, dat);
                        break;
                    case "KSAR":
                        CmdY_ToneParamOPN(mml, 0x50, page, op, dat);
                        break;
                    case "AMDR":
                        CmdY_ToneParamOPN(mml, 0x60, page, op, dat);
                        break;
                    case "SR":
                        CmdY_ToneParamOPN(mml, 0x70, page, op, dat);
                        break;
                    case "SLRR":
                        CmdY_ToneParamOPN(mml, 0x80, page, op, dat);
                        break;
                    case "SSG":
                        CmdY_ToneParamOPN(mml, 0x90, page, op, dat);
                        break;
                    case "FBAL":
                        CmdY_ToneParamOPN_FBAL(mml, page, dat);
                        break;
                }
            }
        }

        public override void CmdLoopExtProc(partPage page, MML mml)
        {
            page.spg.freq = -1;
        }

        public override void CmdHardEnvelope(partPage page, MML mml)
        {
            if (page.Type != enmChannelType.SSG) return;

            string cmd = (string)mml.args[0];
            int n = 0;

            int port;
            int adr;
            int vch;
            GetPortVchSsg(page, out port, out adr, out vch);

            switch (cmd)
            {
                case "EH":
                    page.hardEnvelopeSync.sw = false;
                    n = (int)mml.args[1];
                    if (page.HardEnvelopeSpeed != n)
                    {
                        SOutData(page, mml, page.port[port], (byte)(adr + 0x0b), (byte)(n & 0xff));
                        SOutData(page, mml, page.port[port], (byte)(adr + 0x0c), (byte)((n >> 8) & 0xff));
                        page.HardEnvelopeSpeed = n;
                    }
                    break;
                case "EHON":
                    page.hardEnvelopeSync.sw = false;
                    page.HardEnvelopeSw = true;
                    break;
                case "EHOF":
                    page.hardEnvelopeSync.sw = false;
                    page.HardEnvelopeSw = false;
                    break;
                case "EHT":
                    n = (int)mml.args[1];
                    if (page.HardEnvelopeType != n)
                    {
                        SOutData(page, mml, page.port[port], (byte)(adr + 0x0d), (byte)(n & 0xf));
                        page.HardEnvelopeType = n;
                    }
                    break;
            }
        }

        public override void CmdHardEnvelopeSync(partPage page, MML mml)
        {
            string cmd = (string)mml.args[0];
            switch (cmd)
            {
                case "HSON":
                    page.hardEnvelopeSync.sw = true;
                    page.HardEnvelopeSw = false;
                    break;
                case "HSOF":
                    page.hardEnvelopeSync.sw = false;
                    break;
                case "HSO":
                    page.hardEnvelopeSync.octave = (int)mml.args[1];
                    page.hardEnvelopeSync.octave = Common.CheckRange(page.hardEnvelopeSync.octave, 1, 6);
                    break;
                case "H>":
                    page.hardEnvelopeSync.octave += parent.info.octaveRev ? -1 : 1;
                    page.hardEnvelopeSync.octave = Common.CheckRange(page.hardEnvelopeSync.octave, 1, 6);
                    break;
                case "H<":
                    page.hardEnvelopeSync.octave += parent.info.octaveRev ? 1 : -1;
                    page.hardEnvelopeSync.octave = Common.CheckRange(page.hardEnvelopeSync.octave, 1, 6);
                    break;
                case "HSD":
                    page.hardEnvelopeSync.detune = (int)mml.args[1];
                    break;
                case "HSTN":
                    page.hardEnvelopeSync.tone = true;
                    break;
                case "HSTF":
                    page.hardEnvelopeSync.tone = false;
                    break;
            }
        }

        public override void CmdDetune(partPage page, MML mml)
        {
            string cmd = (string)mml.args[0];
            int n = (int)mml.args[1];

            switch (cmd)
            {
                case "D":
                    page.detune = n;
                    break;
                case "D>":
                    page.detune += parent.info.octaveRev ? -n : n;
                    break;
                case "D<":
                    page.detune += parent.info.octaveRev ? n : -n;
                    break;
            }
            page.detune = Common.CheckRange(page.detune, -0xfff, 0xfff);

            SetDummyData(page, mml);
        }

        public override void CmdKeyOnDelay(partPage page, MML mml)
        {
            if (mml.args == null || (mml.args.Count != 4 && mml.args.Count != 1))
            {
                msgBox.setErrMsg(msg.get("E11006")
                    , mml.line.Lp);

                return;
            }

            //パラメータが一つの場合はスイッチのみ変化
            if (mml.args.Count == 1)
            {
                if (!(mml.args[0] is int))
                {
                    msgBox.setErrMsg(msg.get("E11007")
                        , mml.line.Lp);
                    return;
                }

                page.keyOnDelay.sw = ((int)mml.args[0] != 0);
                return;
            }

            //パラメータが４つの場合はスロットのディレイ値をセット＆スイッチ変化
            bool sw = false;
            for (int i = 0; i < 4; i++)
            {
                if (!(mml.args[i] is int))
                {
                    msgBox.setErrMsg(msg.get("E11007")
                        , mml.line.Lp);
                    return;
                }

                page.keyOnDelay.delay[i] = (int)mml.args[i];
                if (page.keyOnDelay.delay[i] != 0) sw = true;
            }
            page.keyOnDelay.sw = sw;

        }

        public override void CmdMode(partPage page, MML mml)
        {
            if (page.Type != enmChannelType.SSG) return;

            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 0, 1);
            if (n == 1)
            {
                page.freq = 0;//freqをリセット
                page.spg.freq = -1;
                SetFNum(page, mml);
                page.beforeMixer = page.mixer;
                page.mixer = 0;
            }
            else
            {
                page.mixer = page.beforeMixer;
            }

            page.pcm = (n == 1);
            page.instrument = -1;
            page.spg.beforeVolume = -1;
        }

        public override void CmdRR15(partPage page, MML mml)
        {
            if (mml.args == null || (mml.args.Count != 1&& mml.args.Count != 2))
            {
                msgBox.setErrMsg(msg.get("E11010")
                    , mml.line.Lp);
                return;
            }

            if(!(mml.args[0] is bool))
            {
                msgBox.setErrMsg(msg.get("E11010")
                    , mml.line.Lp);
                return;
            }

            page.RR15sw = (bool)mml.args[0];
            if (mml.args.Count == 2)
            {
                page.RR15 = (int)mml.args[1];
            }

            if (!page.RR15sw)
            {
                ResetRR15(page, mml, page.slots);
            }
        }

        public override void SetRR15(partPage page)
        {
            SetRR15(page, null, page.slots);
        }

        public override void CmdTLOFS(partPage page, MML mml)
        {
            if (page.Type != enmChannelType.FMOPN && page.Type != enmChannelType.FMOPNex)
            {
                msgBox.setErrMsg(msg.get("E11012")
                        , mml.line.Lp);
                return;
            }

            if (mml.args == null || mml.args.Count < 2)
            {
                msgBox.setErrMsg(msg.get("E11012")
                    , mml.line.Lp);
                return;
            }

            bool sw = (bool)mml.args[0];
            if (sw)
            {
                SetTLOFS(page, mml);
                return;
            }

            ResetTLOFS(page, mml);
        }


        private void SetTLOFS(partPage page, MML mml)
        {
            if (mml == null || mml.args == null || mml.args.Count < 3) return;

            byte n = (byte)(int)mml.args[1];
            if (page.TLOFS.ContainsKey(n))
            {
                page.TLOFS.Remove(n);
            }
            page.TLOFS.Add(n, (byte)(int)mml.args[2]);
        }

        private void ResetTLOFS(partPage page, MML mml)
        {
            byte n = (byte)(int)mml.args[1];
            if (page.TLOFS.ContainsKey(n))
            {
                page.TLOFS.Remove(n);
            }
        }

        public override void SetupPageData(partWork pw, partPage page)
        {
            //周波数
            page.spg.freq = -1;
            SetFNum(page, null);

            //ノイズ周波数
            noiseFreq = -1;
            OutSsgNoise(null, page);

            if (!page.hardEnvelopeSync.sw)
            {
                //ハードエンベロープtype
                page.spg.HardEnvelopeType = -1;
                OutSsgHardEnvType(page, null);

                //ハードエンベロープspeed
                page.spg.HardEnvelopeSpeed = -1;
                OutSsgHardEnvSpeed(page, null);
            }
            else
            {
                //ハードエンベロープtype
                page.spg.HardEnvelopeType = -1;
                OutSsgHardEnvType(page, null);
            }


            //音量
            page.spg.beforeVolume = -1;
            SetVolume(page, null);

        }

        public override void MultiChannelCommand(MML mml)
        {
            nSSGKeyOn[0] = 0;
            nSSGKeyOn[1] = 0;
            nSSGKeyOn[2] = 0;
            nSSGKeyOn[3] = 0;

            int ssg = 0;
            foreach (partWork pw in lstPartWork)
            {
                partPage page = pw.cpg;
                int port;
                int adr;
                //byte[] fmPort;
                int vch;

                if (pw.cpg.Type == enmChannelType.FMOPN|| (!pw.cpg.Ch3SpecialMode && pw.cpg.Type == enmChannelType.FMOPNex))
                {
                    if (pw.cpg.keyOnDelay.sw)
                    {
                        if (pw.cpg.keyOnDelay.beforekeyOn != pw.cpg.keyOnDelay.keyOn)
                        {
                            vch = (byte)((pw.cpg.ch > 2) ? pw.cpg.ch + 1 : pw.cpg.ch);
                            SOutData(pw.cpg, mml, pw.cpg.port[0], 0x28, (byte)((pw.cpg.keyOnDelay.keyOn << 4) + (vch & 7)));
                            pw.cpg.keyOnDelay.beforekeyOn = pw.cpg.keyOnDelay.keyOn;
                        }
                    }
                    continue;
                }

                if (pw.cpg.Type != enmChannelType.SSG) continue;


                //foreach (partPage page in pw.pg)
                GetPortVchSsg(page, out port, out adr, out vch);
                int p = port;
                //if (adr == 0x10)
                //    p++;

                byte data = (byte)(9 << vch);//9 -> noise&toneが消音
                if (page.keyOn)
                {
                    if (!page.hardEnvelopeSync.sw)
                    {
                        //noise D5/4/3  tone D2/1/0
                        //(bitに0をセットすると出力 1で消音)
                        int n = (page.mixer & 0x1) + ((page.mixer & 0x2) << 2);
                        data &= (byte)(~(n << vch));
                        nSSGKeyOn[ssg / 3] |= data;
                    }
                    else
                    {
                        int mx = (page.hardEnvelopeSync.tone ? page.mixer : 0x0);
                        int n = (mx & 0x1) + ((mx & 0x2) << 2);
                        data &= (byte)(~(n << vch));
                        nSSGKeyOn[ssg / 3] |= data;
                    }
                }
                else
                {
                    nSSGKeyOn[ssg / 3] |= data;
                }

                ssg++;
            }

            if (SSGKeyOn[0] != nSSGKeyOn[0])
            {
                parent.OutData(mml, port[0], 0x07, nSSGKeyOn[0]);
                SSGKeyOn[0] = nSSGKeyOn[0];
            }

            if (this is YM2609)
            {
                if (SSGKeyOn[1] != nSSGKeyOn[1])
                {
                    parent.OutData(mml, port[1], 0x27, nSSGKeyOn[1]);
                    SSGKeyOn[1] = nSSGKeyOn[1];
                }
                if (SSGKeyOn[2] != nSSGKeyOn[2])
                {
                    parent.OutData(mml, port[2], 0x07, nSSGKeyOn[2]);
                    SSGKeyOn[2] = nSSGKeyOn[2];
                }
                if (SSGKeyOn[3] != nSSGKeyOn[3])
                {
                    parent.OutData(mml, port[2], 0x17, nSSGKeyOn[3]);
                    SSGKeyOn[3] = nSSGKeyOn[3];
                }
            }

        }

        private void outTL(partPage page,MML mml)
        {

            SetVolume(page, null);
            int n = page.instrument;

            for(int i = 0; i < 4; i++)
            {
                int tl = 0;

                tl = Common.CheckRange(tl, 0, 127);
                if (page.beforeTL[0] != tl)
                {
                    page.beforeTL[0] = tl;
                    OutFmSetTl(mml, page, 0, tl);
                }
            }
        }

    }
}
