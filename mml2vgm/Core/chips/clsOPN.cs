using musicDriverInterface;
using System;
using System.Collections.Generic;

namespace Core
{
    public class ClsOPN : ClsChip
    {
        public byte[] SSGKeyOn = new byte[] { 0x3f, 0x3f, 0x3f, 0x3f };
        public int noiseFreq = -1;

        public ClsOPN(ClsVgm parent, int chipID, string initialPartName, string stPath, int chipNumber) : base(parent, chipID, initialPartName, stPath, chipNumber)
        {
        }


        public void OutSsgKeyOn(partPage page, MML mml)
        {
            int port;
            int adr;
            int vch;
            GetPortVchSsg(page, out port, out adr, out vch);
            int p = port;
            if (adr == 0x10)
                p++;

            int n = (page.mixer & 0x1) + ((page.mixer & 0x2) << 2);
            byte data = 0;

            data = (byte)(((ClsOPN)page.chip).SSGKeyOn[p] | (9 << vch));
            data &= (byte)(~(n << vch));
            ((ClsOPN)page.chip).SSGKeyOn[p] = data;

            SetSsgVolume(page, mml, page.phaseReset);
            if (page.HardEnvelopeSw)
            {
                SOutData(page, mml, page.port[port], (byte)(adr + 0x0d), (byte)(page.HardEnvelopeType & 0xf));
            }
            SOutData(page, mml, page.port[port], (byte)(adr + 0x07), data);

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
            int port;
            int adr;
            int vch;
            GetPortVchSsg(page, out port, out adr, out vch);
            int p = port;
            if (adr == 0x10)
                p++;

            int n = 9;
            byte data = 0;

            data = (byte)(((ClsOPN)page.chip).SSGKeyOn[p] | (n << vch));
            ((ClsOPN)page.chip).SSGKeyOn[p] = data;

            SOutData(page, mml, page.port[port], (byte)(adr + 0x08 + vch), 0);
            page.beforeVolume = -1;
            SOutData(page, mml, page.port[port], (byte)(adr + 0x07), data);

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

            if (page.varpeggioMode && page.varpIndex != -1)
            {
                vol += page.varpDelta;
            }

            vol = Common.CheckRange(vol, 0, 15) + (page.HardEnvelopeSw ? 0x10 : 0x00);
            if ((((ClsOPN)page.chip).SSGKeyOn[p] & (9 << vch)) == (9 << vch))
            {
                vol = 0;
            }

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

            if (noiseFreq != page.noise)
            {
                noiseFreq = page.noise;
                SOutData(page, mml, page.port[port], (byte)(adr + 0x06), (byte)(page.noise & 0x1f));
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
            int f = -page.detune;
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
                f += GetSsgFNum(page, mml, page.octaveNow, page.noteCmd, page.shift + page.keyShift + page.arpDelta);//
            }

            f = Common.CheckRange(f, 0, 0xfff);
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
        }

        public int GetSsgFNum(partPage page, MML mml, int octave, char noteCmd, int shift)
        {
            int o = octave - 1;
            int n = Const.NOTE.IndexOf(noteCmd) + shift;
            o += n / 12;
            o = Common.CheckRange(o, 0, 7);
            n %= 12;

            int f = o * 12 + n;
            if (f < 0) f = 0;
            if (f >= page.chip.FNumTbl[1].Length) f = page.chip.FNumTbl[1].Length - 1;

            return page.chip.FNumTbl[1][f];
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
        public void OutFmSetVolume(partPage page, MML mml, int vol, int n)
        {
            if (!parent.instFM.ContainsKey(n))
            {
                msgBox.setWrnMsg(string.Format(msg.get("E11000"), n), mml.line.Lp);
                return;
            }

            int alg = parent.instFM[n].Item2[45] & 0x7;
            int[] ope = new int[4] {
                parent.instFM[n].Item2[0*Const.INSTRUMENT_M_OPERATOR_SIZE + 6]
                , parent.instFM[n].Item2[1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 6]
                , parent.instFM[n].Item2[2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 6]
                , parent.instFM[n].Item2[3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 6]
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

            for (int i = 0; i < 4; i++)
            {
                if (algs[alg][i] == 0 || (page.slots & (1 << i)) == 0)
                {
                    ope[i] = -1;
                    continue;
                }
                ope[i] = ope[i] + (127 - vol);
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
            vmml.args.Add(vol);
            vmml.type = enmMMLType.Volume;
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

            if (
                ((page.chip is YM2203) && page.ch >= 3 && page.ch < 6)
                || ((page.chip is YM2608) && page.ch >= 6 && page.ch < 9)
                || ((page.chip is YM2609) && page.ch >= 12 && page.ch < 18)
                || ((page.chip is YM2610B) && page.ch >= 6 && page.ch < 9)
                || ((page.chip is YM2612) && page.ch >= 6 && page.ch < 9)
                || ((page.chip is YM2612X) && page.ch >= 6 && page.ch < 9)
                )
            {
                msgBox.setWrnMsg(msg.get("E11002"), mml.line.Lp);
                return;
            }

            switch (modeBeforeSend)
            {
                case 0: // N)one
                    break;
                case 1: // R)R only
                    for (int ope = 0; ope < 4; ope++) ((ClsOPN)page.chip).OutFmSetSlRr(mml, page, ope, 0, 15);
                    break;
                case 2: // A)ll
                    for (int ope = 0; ope < 4; ope++)
                    {
                        ((ClsOPN)page.chip).OutFmSetDtMl(mml, page, ope, 0, 0);
                        ((ClsOPN)page.chip).OutFmSetKsAr(mml, page, ope, 3, 31);
                        ((ClsOPN)page.chip).OutFmSetAmDr(mml, page, ope, 1, 31);
                        ((ClsOPN)page.chip).OutFmSetSr(mml, page, ope, 31);
                        ((ClsOPN)page.chip).OutFmSetSlRr(mml, page, ope, 0, 15);
                        ((ClsOPN)page.chip).OutFmSetSSGEG(mml, page, ope, 0);
                    }
                    ((ClsOPN)page.chip).OutFmSetFeedbackAlgorithm(mml, page, 7, 7);
                    break;
            }


            for (int ope = 0; ope < 4; ope++)
            {
                //ch3以外の拡張チャンネルでも音色設定できるようにする場合はslotの様子もみてセットすること
                ((ClsOPN)page.chip).OutFmSetDtMl(mml, page, ope, parent.instFM[n].Item2[ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], parent.instFM[n].Item2[ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 8]);
                ((ClsOPN)page.chip).OutFmSetKsAr(mml, page, ope, parent.instFM[n].Item2[ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 7], parent.instFM[n].Item2[ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 1]);
                ((ClsOPN)page.chip).OutFmSetAmDr(mml, page, ope, parent.instFM[n].Item2[ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 10], parent.instFM[n].Item2[ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 2]);
                ((ClsOPN)page.chip).OutFmSetSr(mml, page, ope, parent.instFM[n].Item2[ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 3]);
                ((ClsOPN)page.chip).OutFmSetSlRr(mml, page, ope, parent.instFM[n].Item2[ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 5], parent.instFM[n].Item2[ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 4]);
                ((ClsOPN)page.chip).OutFmSetSSGEG(mml, page, ope, parent.instFM[n].Item2[ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 11]);
            }
            //ch3以外の拡張チャンネルでも音色設定できるようにする場合はslotの様子もみてセットすること
            page.op1ml = parent.instFM[n].Item2[0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
            page.op2ml = parent.instFM[n].Item2[1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
            page.op3ml = parent.instFM[n].Item2[2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
            page.op4ml = parent.instFM[n].Item2[3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
            //ch3以外の拡張チャンネルでも音色設定できるようにする場合はslotの様子もみてセットすること
            page.op1dt2 = 0;
            page.op2dt2 = 0;
            page.op3dt2 = 0;
            page.op4dt2 = 0;

            ((ClsOPN)page.chip).OutFmSetFeedbackAlgorithm(mml, page, parent.instFM[n].Item2[46], parent.instFM[n].Item2[45]);

            int alg = parent.instFM[n].Item2[45] & 0x7;
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
                //ch3以外の拡張チャンネルでも音色設定できるようになったら以下を有効に
                if (algs[alg][i] == 0)// || (pw.ppg[pw.cpgNum].slots & (1 << i)) == 0)
                {
                    op[i] = -1;
                    continue;
                }
                op[i] = Common.CheckRange(op[i], 0, 127);
            }

            partPage vpg = page;
            if (!(page.chip is YM2609))
            {
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

            //ch3以外の拡張チャンネルでも音色設定できるようになったら以下を有効に
            //if ((pw.ppg[pw.cpgNum].slots & 1) != 0 && op[0] != -1) ((ClsOPN)pw.ppg[pw.cpgNum].chip).OutFmSetTl(mml, vpw, 0, op[0]);
            //if ((pw.ppg[pw.cpgNum].slots & 2) != 0 && op[1] != -1) ((ClsOPN)pw.ppg[pw.cpgNum].chip).OutFmSetTl(mml, vpw, 1, op[1]);
            //if ((pw.ppg[pw.cpgNum].slots & 4) != 0 && op[2] != -1) ((ClsOPN)pw.ppg[pw.cpgNum].chip).OutFmSetTl(mml, vpw, 2, op[2]);
            //if ((pw.ppg[pw.cpgNum].slots & 8) != 0 && op[3] != -1) ((ClsOPN)pw.ppg[pw.cpgNum].chip).OutFmSetTl(mml, vpw, 3, op[3]);
            if (op[0] != -1) ((ClsOPN)page.chip).OutFmSetTl(mml, vpg, 0, op[0]);
            if (op[1] != -1) ((ClsOPN)page.chip).OutFmSetTl(mml, vpg, 1, op[1]);
            if (op[2] != -1) ((ClsOPN)page.chip).OutFmSetTl(mml, vpg, 2, op[2]);
            if (op[3] != -1) ((ClsOPN)page.chip).OutFmSetTl(mml, vpg, 3, op[3]);


            //音量を再セットする

            OutFmSetVolume(page, mml, vol, n);

            //拡張チャンネルの場合は他の拡張チャンネルも音量を再セットする
            if (page.Type == enmChannelType.FMOPNex)
            {
                if (!(page.chip is YM2609))
                {
                    if (page.ch != 2) OutFmSetVolume(page.chip.lstPartWork[2].cpg, mml, page.chip.lstPartWork[2].cpg.volume, n);
                    if (page.ch != m + 3) OutFmSetVolume(page.chip.lstPartWork[m + 3].cpg, mml, page.chip.lstPartWork[m + 3].cpg.volume, n);
                    if (page.ch != m + 4) OutFmSetVolume(page.chip.lstPartWork[m + 4].cpg, mml, page.chip.lstPartWork[m + 4].cpg.volume, n);
                    if (page.ch != m + 5) OutFmSetVolume(page.chip.lstPartWork[m + 5].cpg, mml, page.chip.lstPartWork[m + 5].cpg.volume, n);
                }
                else
                {
                    if (page.ch == 2 || page.ch == 12 || page.ch == 13 || page.ch == 14)
                    {
                        //YM2609 ch3 || ch13 || ch14 || ch15
                        if (page.ch != 2) OutFmSetVolume(page.chip.lstPartWork[2].cpg, mml, page.chip.lstPartWork[2].cpg.volume, n);
                        if (page.ch != 12) OutFmSetVolume(page.chip.lstPartWork[12].cpg, mml, page.chip.lstPartWork[12].cpg.volume, n);
                        if (page.ch != 13) OutFmSetVolume(page.chip.lstPartWork[13].cpg, mml, page.chip.lstPartWork[13].cpg.volume, n);
                        if (page.ch != 14) OutFmSetVolume(page.chip.lstPartWork[14].cpg, mml, page.chip.lstPartWork[14].cpg.volume, n);
                    }
                    else
                    {
                        //YM2609 ch9 || ch16 || ch17 || ch18
                        if (page.ch != 8) OutFmSetVolume(page.chip.lstPartWork[8].cpg, mml, page.chip.lstPartWork[8].cpg.volume, n);
                        if (page.ch != 15) OutFmSetVolume(page.chip.lstPartWork[15].cpg, mml, page.chip.lstPartWork[15].cpg.volume, n);
                        if (page.ch != 16) OutFmSetVolume(page.chip.lstPartWork[16].cpg, mml, page.chip.lstPartWork[16].cpg.volume, n);
                        if (page.ch != 17) OutFmSetVolume(page.chip.lstPartWork[17].cpg, mml, page.chip.lstPartWork[17].cpg.volume, n);
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

        public void OutFmSetFnum(partPage page, MML mml, int octave, int num)
        {
            int freq;
            freq = ((num & 0x700) >> 8) + (((octave - 1) & 0x7) << 3);
            freq = (freq << 8) + (num & 0xff);

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

            if (!page.pcm)
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

                            parent.xgmKeyOnData.Add(od);
                        }
                        else
                        {
                            SOutData(page, mml, page.port[0], 0x28, (byte)((slot << 4) + 2));
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
                                od.val = (byte)((page.slots << 4) + (vch & 7));
                                if (mml != null)
                                {
                                    od.type = mml.type;
                                    if (mml.line != null && mml.line.Lp != null)
                                    {
                                        od.linePos = new LinePos(
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

                                parent.xgmKeyOnData.Add(od);
                            }
                            else
                            {
                                //key on
                                SOutData(page, mml, page.port[0], 0x28, (byte)((page.slots << 4) + (vch & 7)));
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
                        od.val = (byte)((slot << 4) + 2);
                        if (mml != null)
                        {
                            od.type = mml.type;
                            if (mml.line != null && mml.line.Lp != null)
                            {
                                od.linePos = new LinePos(
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
                            od.val = (byte)((page.slots << 4) + (vch & 7));
                            if (mml != null)
                            {
                                od.type = mml.type;
                                if (mml.line != null && mml.line.Lp != null)
                                {
                                    od.linePos = new LinePos(
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
                            od.val = (byte)((page.slots << 4) + (vch & 7));
                            if (mml != null)
                            {
                                od.type = mml.type;
                                if (mml.line != null && mml.line.Lp != null)
                                {
                                    od.linePos = new LinePos(
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


            if (page.isPcmMap)
            {
                int nt = Const.NOTE.IndexOf(page.noteCmd);
                int f = page.octaveNow * 12 + nt + page.shift + page.keyShift + page.arpDelta;
                if (parent.instPCMMap.ContainsKey(page.pcmMapNo))
                {
                    if (parent.instPCMMap[page.pcmMapNo].ContainsKey(f))
                    {
                        page.instrument = parent.instPCMMap[page.pcmMapNo][f];
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
                    w = page.waitCounter * page.gatetime / 8L;
                }
                else
                {
                    w = page.waitCounter - page.gatetime;
                }
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
                            , (byte)page.chip.ChipID
                            , (byte)(page.chip.ChipID >> 8)
                        };
                        else cmd = new byte[] {
                            0x30
                            , (byte)page.spg.streamID
                            , (byte)page.chip.ChipID
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


        public void SetFmFNum(partPage page, MML mml)
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

            int[] ftbl = page.chip.FNumTbl[0];
            int f;
            f = GetFmFNum(ftbl, page.octaveNow, page.noteCmd, page.shift + page.keyShift + page.toneDoublerKeyShift + page.arpDelta);//
            if (page.bendWaitCounter != -1)
            {
                f = page.bendFnum;
            }
            int o = (f & 0xf000) / 0x1000;
            f &= 0xfff;

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

        public int GetFmFNum(int[] ftbl, int octave, char noteCmd, int shift)
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
            //    o += n / 12 - ((n % 12 == 0) ? 0 : 1);
            //    o = Common.CheckRange(o, 1, 8);
            //    n %= 12;
            //    if (n < 0) { n += 12; }
            //}

            int f = ftbl[n];

            return (f & 0xfff) + (o & 0xf) * 0x1000;
        }

        public override int GetFNum(partPage page, MML mml, int octave, char cmd, int shift)
        {
            if (page.Type == enmChannelType.FMOPN || page.Type == enmChannelType.FMOPNex)
            {
                return GetFmFNum(FNumTbl[0], octave, cmd, shift);
            }
            if (page.Type == enmChannelType.SSG)
            {
                return GetSsgFNum(page, mml, octave, cmd, shift);
            }
            return 0;
        }

        public override void GetFNumAtoB(partPage page, MML mml
            , out int a, int aOctaveNow, char aCmd, int aShift
            , out int b, int bOctaveNow, char bCmd, int bShift
            , int dir)
        {
            a = GetFNum(page, mml, aOctaveNow, aCmd, aShift);
            b = GetFNum(page, mml, bOctaveNow, bCmd, bShift);

            int oa = (a & 0xf000) / 0x1000;
            int ob = (b & 0xf000) / 0x1000;
            if (oa != ob)
            {
                if ((a & 0xfff) == FNumTbl[0][0])
                {
                    oa += Math.Sign(ob - oa);
                    a = (a & 0xfff) * 2 + oa * 0x1000;
                }
                else if ((b & 0xfff) == FNumTbl[0][0])
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

            if(page.varpeggioMode && page.varpIndex != -1)
            {
                vol += page.varpDelta;
            }

            if (page.spg.beforeVolume != vol)
            {
                if (parent.instFM.ContainsKey(page.instrument))
                {
                    OutFmSetVolume(page, mml, vol, page.instrument);
                }
                page.spg.beforeVolume = vol;
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

                if (pl.param[5] != 1)
                    continue;

                pl.isEnd = false;
                pl.value = (pl.param[0] == 0) ? pl.param[6] : 0;//ディレイ中は振幅補正は適用されない
                pl.waitCounter = pl.param[0];
                pl.direction = pl.param[2] < 0 ? -1 : 1;
                pl.depthWaitCounter = pl.param[7];
                pl.depth = pl.param[3];
                pl.depthV2 = pl.param[2];

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
            byte[] instFM = parent.instFM[i].Item2;
            if (instFM == null || instFM.Length < 1) return;
            Note note = (Note)mml.args[0];

            if (page.TdA == -1)
            {
                //resetToneDoubler
                if (page.op1ml != instFM[0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8])
                {
                    OutFmSetDtMl(mml, page, 0, instFM[0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instFM[0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8]);
                    page.op1ml = instFM[0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
                }
                if (page.op2ml != instFM[1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8])
                {
                    OutFmSetDtMl(mml, page, 1, instFM[1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instFM[1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8]);
                    page.op2ml = instFM[1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
                }
                if (page.op3ml != instFM[2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8])
                {
                    OutFmSetDtMl(mml, page, 2, instFM[2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instFM[2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8]);
                    page.op3ml = instFM[2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
                }
                if (page.op4ml != instFM[3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8])
                {
                    OutFmSetDtMl(mml, page, 3, instFM[3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instFM[3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8]);
                    page.op4ml = instFM[3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
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
                int TdB = oct * 12 + Const.NOTE.IndexOf(note.tDblCmd) + note.tDblShift + page.keyShift + page.arpDelta;
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
                    OutFmSetDtMl(mml, page, 0, instFM[0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instToneDoubler.lstTD[us].OP1ML);
                    page.op1ml = instToneDoubler.lstTD[us].OP1ML;
                }
                if (page.op2ml != instToneDoubler.lstTD[us].OP2ML)
                {
                    OutFmSetDtMl(mml, page, 1, instFM[1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instToneDoubler.lstTD[us].OP2ML);
                    page.op2ml = instToneDoubler.lstTD[us].OP2ML;
                }
                if (page.op3ml != instToneDoubler.lstTD[us].OP3ML)
                {
                    OutFmSetDtMl(mml, page, 2, instFM[2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instToneDoubler.lstTD[us].OP3ML);
                    page.op3ml = instToneDoubler.lstTD[us].OP3ML;
                }
                if (page.op4ml != instToneDoubler.lstTD[us].OP4ML)
                {
                    OutFmSetDtMl(mml, page, 3, instFM[3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instToneDoubler.lstTD[us].OP4ML);
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
                page.mixer = n;
            }
        }

        public override void CmdNoise(partPage page, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 0, 31);

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
            char type = (char)mml.args[0];
            int n = (int)mml.args[1];

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
                SetEnvelopParamFromInstrument(page, n, mml);
                return;
            }

            if (page.Type == enmChannelType.SSG)
            {
                SetEnvelopParamFromInstrument(page, n, mml);
                return;
            }

            n = Common.CheckRange(n, 0, 255);
            if (page.instrument == n) return;
            page.instrument = n;
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
                    byte res = 0;
                    while (n % 10 != 0)
                    {
                        if (n % 10 > 0 && n % 10 < 5)
                        {
                            res += (byte)(1 << (n % 10 - 1));
                        }
                        else
                        {
                            msgBox.setErrMsg(string.Format(msg.get("E11005"), n), mml.line.Lp);
                            break;
                        }
                        n /= 10;
                    }
                    if (res != 0)
                    {
                        page.slotsEX = res;
                        if (page.Ch3SpecialMode)
                        {
                            page.slots = page.slotsEX;
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
                            p.apg.slots = p.apg.slotsEX;
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
                            if (pg.Type == enmChannelType.FMOPNex)
                            {
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
            }
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
                    n = (int)mml.args[1];
                    if (page.HardEnvelopeSpeed != n)
                    {
                        SOutData(page, mml, page.port[port], (byte)(adr + 0x0b), (byte)(n & 0xff));
                        SOutData(page, mml, page.port[port], (byte)(adr + 0x0c), (byte)((n >> 8) & 0xff));
                        page.HardEnvelopeSpeed = n;
                    }
                    break;
                case "EHON":
                    page.HardEnvelopeSw = true;
                    break;
                case "EHOF":
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

        public override void CmdDetune(partPage page, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, -0xfff, 0xfff);
            page.detune = n;
            SetDummyData(page, mml);
        }

    }
}
