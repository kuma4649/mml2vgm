using musicDriverInterface;
using System;
using System.Collections.Generic;

namespace Core
{
    public class YM2151 : ClsChip
    {
        public YM2151(ClsVgm parent, int chipID, string initialPartName, string stPath, int chipNumber) : base(parent, chipID, initialPartName, stPath, chipNumber)
        {

            _Name = "YM2151";
            _ShortName = "OPM";
            _ChMax = 8;
            _canUsePcm = false;

            Frequency = 3579545;
            port = new byte[][] { new byte[] { (byte)(chipNumber != 0 ? 0xa4 : 0x54) } };

            if (string.IsNullOrEmpty(initialPartName)) return;

            MakeFNumTbl();
            Ch = new ClsChannel[ChMax];
            SetPartToCh(Ch, initialPartName);
            foreach (ClsChannel ch in Ch)
            {
                ch.Type = enmChannelType.FMOPM;
                ch.chipNumber = chipID == 1;
            }

        }

        public override void InitChip()
        {
            if (!use) return;

            //initialize shared param

            //FM Off
            OutAllKeyOff();

            foreach (partWork pw in lstPartWork)
            {
                foreach (partPage page in pw.pg)
                {
                    if (page.ch == 0)
                    {
                        page.hardLfoFreq = 0;
                        page.hardLfoPMD = 0;
                        page.hardLfoAMD = 0;

                        //Reset Hard LFO
                        OutSetHardLfoFreq(null, page, page.hardLfoFreq);
                        OutSetHardLfoDepth(null, page, false, page.hardLfoAMD);
                        OutSetHardLfoDepth(null, page, true, page.hardLfoPMD);
                    }

                    page.ams = 0;
                    page.pms = 0;
                    if (!page.dataEnd) OutSetPMSAMS(null, page, 0, 0);
                }
            }

            if (ChipID != 0 && parent.info.format != enmFormat.ZGM)
            {
                parent.dat[0x33] = new outDatum(enmMMLType.unknown, null, null, (byte)(parent.dat[0x33].val | 0x40));//use Secondary
            }
        }

        public override void InitPart(partWork pw)
        {
            foreach (partPage page in pw.pg)
            {
                page.slots = 0xf;
                page.volume = 127;
                page.MaxVolume = 127;
                page.port = port;
                page.mixer = 0;
                page.noise = 0;
                page.beforeMixer = -1;
                page.beforeNoise = -1;
            }
        }


        public void OutSetFnum(MML mml, partPage page, int octave, int note, int kf)
        {
            octave &= 0x7;
            note &= 0xf;
            note = note < 3 ? note : (note < 6 ? (note + 1) : (note < 9 ? (note + 2) : (note + 3)));
            SOutData(page, mml, port[0], (byte)(0x28 + page.ch), (byte)((octave << 4) | note));
            SOutData(page, mml, port[0], (byte)(0x30 + page.ch), (byte)(kf << 2));
        }

        public void OutSetVolume(partPage page, MML mml, int vol, int n)
        {
            if (!parent.instOPM.ContainsKey(n))
            {
                msgBox.setWrnMsg(string.Format(msg.get("E16000"), n), mml.line.Lp);
                return;
            }

            int alg = parent.instOPM[n].Item2[45] & 0x7;
            int[] ope = new int[4] {
                parent.instOPM[n].Item2[0*Const.INSTRUMENT_M_OPERATOR_SIZE + 6]
                , parent.instOPM[n].Item2[1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 6]
                , parent.instOPM[n].Item2[2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 6]
                , parent.instOPM[n].Item2[3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 6]
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

            //int minV = 127;
            //for (int i = 0; i < 4; i++)
            //{
            //    if (algs[alg][i] == 1 && (pw.ppg[pw.cpgNum].slots & (1 << i)) != 0)
            //    {
            //        minV = Math.Min(minV, ope[i]);
            //    }
            //}

            for (int i = 0; i < 4; i++)
            {
                if (algs[alg][i] == 0 || (page.slots & (1 << i)) == 0)
                {
                    ope[i] = -1;
                    continue;
                }
                //ope[i] = ope[i] - minV + (127 - vol);
                ope[i] = ope[i] + (127 - vol);
                if (ope[i] < 0)
                {
                    ope[i] = 0;
                }
                if (ope[i] > 127)
                {
                    ope[i] = 127;
                }
            }

            if ((page.slots & 1) != 0 && ope[0] != -1) OutSetTl(mml, page, 0, ope[0]);
            if ((page.slots & 2) != 0 && ope[1] != -1) OutSetTl(mml, page, 1, ope[1]);
            if ((page.slots & 4) != 0 && ope[2] != -1) OutSetTl(mml, page, 2, ope[2]);
            if ((page.slots & 8) != 0 && ope[3] != -1) OutSetTl(mml, page, 3, ope[3]);
        }

        public void OutFmSetTL(partPage page, MML mml, int tl1, int tl2, int tl3, int tl4,int slot, int n)
        {
            if (!parent.instOPM.ContainsKey(n))
            {
                msgBox.setWrnMsg(string.Format(msg.get("E11000"), n), mml.line.Lp);
                return;
            }

            int alg = parent.instOPM[n].Item2[45] & 0x7;
            int[] ope = new int[4] {
                parent.instOPM[n].Item2[0*Const.INSTRUMENT_M_OPERATOR_SIZE + 6]
                , parent.instOPM[n].Item2[1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 6]
                , parent.instOPM[n].Item2[2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 6]
                , parent.instOPM[n].Item2[3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 6]
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
            int m = (page.chip is YM2203) ? 0 : 3;
            if (page.chip.lstPartWork[2].cpg.Ch3SpecialMode && page.ch >= m + 3 && page.ch < m + 6)
            {
                vpg = page.chip.lstPartWork[2].cpg;
            }

            MML vmml = new MML();
            vmml.args = new List<object>();
            vmml.args.Add(tl1);
            vmml.type = enmMMLType.unknown;//.TotalLevel;
            if (mml != null)
                vmml.line = mml.line;
            if ((page.slots & 1) != 0 && ope[0] != -1) OutSetTl(vmml, vpg, 0, ope[0]);
            if ((page.slots & 2) != 0 && ope[1] != -1) OutSetTl(vmml, vpg, 1, ope[1]);
            if ((page.slots & 4) != 0 && ope[2] != -1) OutSetTl(vmml, vpg, 2, ope[2]);
            if ((page.slots & 8) != 0 && ope[3] != -1) OutSetTl(vmml, vpg, 3, ope[3]);
        }

        public void OutSetTl(MML mml, partPage page, int ope, int tl)
        {
            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            tl &= 0x7f;

            SOutData(page,
                mml,
                port[0]
                , (byte)(0x60 + page.ch + ope * 8)
                , (byte)tl
                );
        }

        public void OutSetHardLfoFreq(MML mml, partPage page, int freq)
        {
            SOutData(page,
                mml,
                port[0]
                , 0x18
                , (byte)(freq & 0xff)
                );
        }

        public void OutSetHardLfoDepth(MML mml, partPage page, bool isPMD, int depth)
        {
            SOutData(page,
                mml,
                port[0]
                , 0x19
                , (byte)((isPMD ? 0x80 : 0x00) | (depth & 0x7f))
                );
        }

        public void OutSetPMSAMS(MML mml, partPage page, int PMS, int AMS)
        {
            SOutData(page,
                mml,
                port[0]
                , (byte)(0x38 + page.ch)
                , (byte)(((PMS & 0x7) << 4) | (AMS & 0x3))
                );
        }

        public void OutSetPanFeedbackAlgorithm(MML mml, partPage page, int pan, int fb, int alg)
        {
            pan &= 3;
            fb &= 7;
            alg &= 7;

            SOutData(page, mml, port[0], (byte)(0x20 + page.ch), (byte)((pan << 6) | (fb << 3) | alg));
        }

        public void OutSetDtMl(MML mml, partPage page, int ope, int dt, int ml)
        {
            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            dt &= 7;
            ml &= 15;

            SOutData(page, mml, port[0], (byte)(0x40 + page.ch + ope * 8), (byte)((dt << 4) | ml));
        }

        public void OutSetKsAr(MML mml, partPage page, int ope, int ks, int ar)
        {
            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            ks &= 3;
            ar &= 31;

            SOutData(page, mml, port[0], (byte)(0x80 + page.ch + ope * 8), (byte)((ks << 6) | ar));
        }

        public void OutSetAmDr(MML mml, partPage page, int ope, int am, int dr)
        {
            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            am &= 1;
            dr &= 31;

            SOutData(page, mml, port[0], (byte)(0xa0 + page.ch + ope * 8), (byte)((am << 7) | dr));
        }

        public void OutSetDt2Sr(MML mml, partPage page, int ope, int dt2, int sr)
        {
            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            dt2 &= 3;
            sr &= 31;

            SOutData(page, mml, port[0], (byte)(0xc0 + page.ch + ope * 8), (byte)((dt2 << 6) | sr));
        }

        public void OutSetSlRr(MML mml, partPage page, int ope, int sl, int rr)
        {
            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            sl &= 15;
            rr &= 15;

            SOutData(page, mml, port[0], (byte)(0xe0 + page.ch + ope * 8), (byte)((sl << 4) | rr));
        }

        public void OutSetHardLfo(MML mml, partPage page, bool sw, List<int> param)
        {
            if (sw)
            {
                SOutData(page, mml, port[0], 0x1b, (byte)(param[0] & 0x3));//type
                SOutData(page, mml, port[0], 0x18, (byte)(param[1] & 0xff));//LFRQ
                SOutData(page, mml, port[0], 0x19, (byte)((param[2] & 0x7f) | 0x80));//PMD
                SOutData(page, mml, port[0], 0x19, (byte)((param[3] & 0x7f) | 0x00));//AMD
            }
            else
            {
                SOutData(page, mml, port[0], 0x1b, 0);//type
                SOutData(page, mml, port[0], 0x18, 0);//LFRQ
                SOutData(page, mml, port[0], 0x19, 0x80);//PMD
                SOutData(page, mml, port[0], 0x19, 0x00);//AMD
            }
        }

        public void OutSetInstrument(partPage page, MML mml, int n, int vol, int modeBeforeSend)
        {

            if (!parent.instOPM.ContainsKey(n))
            {
                msgBox.setWrnMsg(string.Format(msg.get("E16001"), n), mml.line.Lp);
                return;
            }

            switch (modeBeforeSend)
            {
                case 0: // N)one
                    break;
                case 1: // R)R only
                    for (int ope = 0; ope < 4; ope++) OutSetSlRr(mml, page, ope, 0, 15);
                    break;
                case 2: // A)ll
                    for (int ope = 0; ope < 4; ope++)
                    {
                        OutSetDtMl(mml, page, ope, 0, 0);
                        OutSetKsAr(mml, page, ope, 3, 31);
                        OutSetAmDr(mml, page, ope, 1, 31);
                        OutSetDt2Sr(mml, page, ope, 0, 31);
                        OutSetSlRr(mml, page, ope, 0, 15);
                    }
                    OutSetPanFeedbackAlgorithm(mml, page, page.pan, 7, 7);
                    break;
            }

            for (int ope = 0; ope < 4; ope++)
            {

                OutSetDtMl(mml, page, ope, parent.instOPM[n].Item2[ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], parent.instOPM[n].Item2[ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 8]);
                OutSetKsAr(mml, page, ope, parent.instOPM[n].Item2[ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 7], parent.instOPM[n].Item2[ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 1]);
                OutSetAmDr(mml, page, ope, parent.instOPM[n].Item2[ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 11], parent.instOPM[n].Item2[ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 2]);
                OutSetDt2Sr(mml, page, ope, parent.instOPM[n].Item2[ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 10], parent.instOPM[n].Item2[ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 3]);
                OutSetSlRr(mml, page, ope, parent.instOPM[n].Item2[ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 5], parent.instOPM[n].Item2[ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 4]);

            }
            page.op1ml = parent.instOPM[n].Item2[0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
            page.op2ml = parent.instOPM[n].Item2[1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
            page.op3ml = parent.instOPM[n].Item2[2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
            page.op4ml = parent.instOPM[n].Item2[3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
            page.op1dt2 = parent.instOPM[n].Item2[0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 10];
            page.op2dt2 = parent.instOPM[n].Item2[1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 10];
            page.op3dt2 = parent.instOPM[n].Item2[2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 10];
            page.op4dt2 = parent.instOPM[n].Item2[3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 10];

            OutSetPanFeedbackAlgorithm(mml, page, page.pan, parent.instOPM[n].Item2[46], parent.instOPM[n].Item2[45]);

            int alg = parent.instOPM[n].Item2[45] & 0x7;
            int[] op = new int[4] {
                parent.instOPM[n].Item2[0*Const.INSTRUMENT_M_OPERATOR_SIZE + 6]
                , parent.instOPM[n].Item2[1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 6]
                , parent.instOPM[n].Item2[2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 6]
                , parent.instOPM[n].Item2[3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 6]
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
                if (algs[alg][i] == 0 || (page.slots & (1 << i)) == 0)
                {
                    op[i] = -1;
                    continue;
                }
                if (op[i] < 0)
                {
                    op[i] = 0;
                }
                if (op[i] > 127)
                {
                    op[i] = 127;
                }
            }

            if ((page.slots & 1) != 0 && op[0] != -1) OutSetTl(mml, page, 0, op[0]);
            if ((page.slots & 2) != 0 && op[1] != -1) OutSetTl(mml, page, 1, op[1]);
            if ((page.slots & 4) != 0 && op[2] != -1) OutSetTl(mml, page, 2, op[2]);
            if ((page.slots & 8) != 0 && op[3] != -1) OutSetTl(mml, page, 3, op[3]);

            ((YM2151)page.chip).OutSetVolume(page, mml, vol, n);

        }

        public void OutKeyOn(MML mml, partPage page)
        {

            if (page.ch == 7 && page.mixer == 1)
            {
                SOutData(page, mml, port[0], 0x0f, (byte)((page.mixer << 7) | (page.noise & 0x1f)));
            }
            //key on
            SOutData(page, mml, port[0], 0x08, (byte)((page.slots << 3) + page.ch));
        }

        public void OutKeyOff(MML mml, partPage page)
        {

            //key off
            SOutData(page, mml, port[0], 0x08, (byte)(0x00 + (page.ch & 7)));
            if (page.ch == 7 && page.mixer == 1)
            {
                SOutData(page, mml, port[0], 0x0f, 0x00);
            }

        }

        public void OutAllKeyOff()
        {

            foreach (partWork pw in lstPartWork)
            {
                foreach (partPage page in pw.pg)
                {
                    if (page.dataEnd) continue;

                    OutKeyOff(null, page);
                    OutSetTl(null, page, 0, 127);
                    OutSetTl(null, page, 1, 127);
                    OutSetTl(null, page, 2, 127);
                    OutSetTl(null, page, 3, 127);
                }
            }

        }


        public override void SetFNum(partPage page, MML mml)
        {
            int arpNote = page.arpFreqMode ? 0 : page.arpDelta;
            int arpFreq = page.arpFreqMode ? page.arpDelta : 0;

            int f = GetFNum(page, mml, page.octaveNow, page.noteCmd, page.shift + page.keyShift + page.toneDoublerKeyShift + arpNote, page.pitchShift);//

            if (page.bendWaitCounter != -1)
            {
                f = page.bendFnum;
            }

            f = f + page.detune;
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
                f += page.lfo[lfo].value + page.lfo[lfo].param[6];
            }

            f = Common.CheckRange(f, 0, 9 * 12 * 64 - 1);
            int oct = f / (12 * 64);
            int note = (f - oct * 12 * 64) / 64;
            int kf = f - oct * 12 * 64 - note * 64;

            OutSetFnum(mml, page, oct, note, kf);
        }

        public override int GetFNum(partPage page, MML mml, int octave, char noteCmd, int shift, int pitchShift)
        {
            int o = octave;
            int n = Const.NOTE.IndexOf(noteCmd) + shift - 1;

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
            //    if (o == 0 && n < 0)
            //    {
            //        o = 1;
            //        n = 0;
            //    }
            //    else
            //    {
            //        o = Common.CheckRange(o, 1, 8);
            //        n %= 12;
            //        if (n < 0) { n += 12; }
            //    }
            //}
            o--;

            return n * 64 + o * 12 * 64 + pitchShift;
        }

        public override void SetVolume(partPage page, MML mml)
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

            if (page.varpeggioMode)
            {
                vol += page.varpDelta;
            }

            if (page.beforeVolume != vol)
            {
                if (parent.instOPM.ContainsKey(page.instrument))
                {
                    OutSetVolume(page, mml, vol, page.instrument);
                    page.beforeVolume = vol;
                }
            }

            SetFmTL(page, mml);
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
                if (parent.instOPM.ContainsKey(page.instrument))
                {
                    OutFmSetTL(page, mml, tl1, tl2, tl3, tl4, slot, page.instrument);
                }
                page.spg.beforeTlDelta1 = tl1;
                page.spg.beforeTlDelta2 = tl2;
                page.spg.beforeTlDelta3 = tl3;
                page.spg.beforeTlDelta4 = tl4;
            }
        }
        public override void SetKeyOn(partPage page, MML mml)
        {
            page.keyOn = true;
            //OutKeyOn(mml, page);
        }

        public override void SetKeyOff(partPage page, MML mml)
        {
            OutKeyOff(mml, page);
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
                    SetFNum(page, mml);

                if (pl.type == eLfoType.Tremolo)
                    SetVolume(page, mml);

                if (pl.type == eLfoType.Wah)
                {
                    page.beforeVolume = -1;
                    SetFmTL(page, mml);
                }

            }
        }

        public override int GetToneDoublerShift(partPage page, int octave, char noteCmd, int shift)
        {
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

        public override void SetToneDoubler(partPage page, MML mml)
        {
            int i = page.instrument;
            if (i < 0) return;

            page.toneDoublerKeyShift = 0;
            byte[] instOPM = parent.instOPM[i].Item2;
            if (instOPM == null || instOPM.Length < 1) return;
            Note note = (Note)mml.args[0];

            if (page.TdA == -1)
            {
                //resetToneDoubler
                //ML
                if (page.op1ml != instOPM[0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8])
                {
                    ((YM2151)page.chip).OutSetDtMl(mml, page, 0, instOPM[0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instOPM[0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8]);
                    page.op1ml = instOPM[0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
                }
                if (page.op2ml != instOPM[1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8])
                {
                    ((YM2151)page.chip).OutSetDtMl(mml, page, 1, instOPM[1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instOPM[1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8]);
                    page.op2ml = instOPM[1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
                }
                if (page.op3ml != instOPM[2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8])
                {
                    ((YM2151)page.chip).OutSetDtMl(mml, page, 2, instOPM[2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instOPM[2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8]);
                    page.op3ml = instOPM[2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
                }
                if (page.op4ml != instOPM[3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8])
                {
                    ((YM2151)page.chip).OutSetDtMl(mml, page, 3, instOPM[3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instOPM[3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8]);
                    page.op4ml = instOPM[3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
                }
                //DT2
                if (page.op1dt2 != instOPM[0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 10])
                {
                    ((YM2151)page.chip).OutSetDt2Sr(mml, page, 0, instOPM[0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 10], instOPM[0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 3]);
                    page.op1dt2 = instOPM[0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 10];
                }
                if (page.op2dt2 != instOPM[1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 10])
                {
                    ((YM2151)page.chip).OutSetDt2Sr(mml, page, 1, instOPM[1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 10], instOPM[1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 3]);
                    page.op2dt2 = instOPM[1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 10];
                }
                if (page.op3dt2 != instOPM[2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 10])
                {
                    ((YM2151)page.chip).OutSetDt2Sr(mml, page, 2, instOPM[2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 10], instOPM[2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 3]);
                    page.op3dt2 = instOPM[2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 10];
                }
                if (page.op4dt2 != instOPM[3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 10])
                {
                    ((YM2151)page.chip).OutSetDt2Sr(mml, page, 3, instOPM[3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 10], instOPM[3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 3]);
                    page.op4dt2 = instOPM[3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 10];
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

                //ML
                if (page.op1ml != instToneDoubler.lstTD[us].OP1ML)
                {
                    ((YM2151)page.chip).OutSetDtMl(mml, page, 0, instOPM[0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instToneDoubler.lstTD[us].OP1ML);
                    page.op1ml = instToneDoubler.lstTD[us].OP1ML;
                }
                if (page.op2ml != instToneDoubler.lstTD[us].OP2ML)
                {
                    ((YM2151)page.chip).OutSetDtMl(mml, page, 1, instOPM[1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instToneDoubler.lstTD[us].OP2ML);
                    page.op2ml = instToneDoubler.lstTD[us].OP2ML;
                }
                if (page.op3ml != instToneDoubler.lstTD[us].OP3ML)
                {
                    ((YM2151)page.chip).OutSetDtMl(mml, page, 2, instOPM[2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instToneDoubler.lstTD[us].OP3ML);
                    page.op3ml = instToneDoubler.lstTD[us].OP3ML;
                }
                if (page.op4ml != instToneDoubler.lstTD[us].OP4ML)
                {
                    ((YM2151)page.chip).OutSetDtMl(mml, page, 3, instOPM[3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instToneDoubler.lstTD[us].OP4ML);
                    page.op4ml = instToneDoubler.lstTD[us].OP4ML;
                }
                //DT2
                if (page.op1dt2 != instToneDoubler.lstTD[us].OP1DT2)
                {
                    ((YM2151)page.chip).OutSetDt2Sr(mml, page, 0, instToneDoubler.lstTD[us].OP1DT2, instOPM[0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 3]);
                    page.op1dt2 = instToneDoubler.lstTD[us].OP1DT2;
                }
                if (page.op2dt2 != instToneDoubler.lstTD[us].OP2DT2)
                {
                    ((YM2151)page.chip).OutSetDt2Sr(mml, page, 1, instToneDoubler.lstTD[us].OP2DT2, instOPM[1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 3]);
                    page.op2dt2 = instToneDoubler.lstTD[us].OP2DT2;
                }
                if (page.op3dt2 != instToneDoubler.lstTD[us].OP3DT2)
                {
                    ((YM2151)page.chip).OutSetDt2Sr(mml, page, 2, instToneDoubler.lstTD[us].OP3DT2, instOPM[2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 3]);
                    page.op3dt2 = instToneDoubler.lstTD[us].OP3DT2;
                }
                if (page.op4dt2 != instToneDoubler.lstTD[us].OP4DT2)
                {
                    ((YM2151)page.chip).OutSetDt2Sr(mml, page, 3, instToneDoubler.lstTD[us].OP4DT2, instOPM[3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 3]);
                    page.op4dt2 = instToneDoubler.lstTD[us].OP4DT2;
                }

                //pw.ppg[pw.cpgNum].TdA = -1;
            }
        }


        public override void CmdNoiseToneMixer(partPage page, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 0, 1);
            page.mixer = n;
        }

        public override void CmdNoise(partPage page, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 0, 31);
            if (page.noise != n)
            {
                page.noise = n;
            }
        }

        public override void CmdMPMS(partPage page, MML mml)
        {
            int n = (int)mml.args[1];
            n = Common.CheckRange(n, 0, 7);
            page.pms = n;
            ((YM2151)page.chip).OutSetPMSAMS(mml, page, page.pms, page.ams);
        }

        public override void CmdMAMS(partPage page, MML mml)
        {
            int n = (int)mml.args[1];
            n = Common.CheckRange(n, 0, 3);
            page.ams = n;
            ((YM2151)page.chip).OutSetPMSAMS(mml, page, page.pms, page.ams);
        }

        public override void CmdLfo(partPage page, MML mml)
        {
            base.CmdLfo(page, mml);

            if (mml.args[0] is string) return;

            int c = (char)mml.args[0] - 'P';
            if (page.lfo[c].type == eLfoType.Hardware)
            {
                if (page.lfo[c].param.Count < 4)
                {
                    msgBox.setErrMsg(msg.get("E16002"), mml.line.Lp);
                    return;
                }
                if (page.lfo[c].param.Count > 5)
                {
                    msgBox.setErrMsg(msg.get("E16003"), mml.line.Lp);
                    return;
                }

                page.lfo[c].param[0] = Common.CheckRange(page.lfo[c].param[0], 0, 3); //Type
                page.lfo[c].param[1] = Common.CheckRange(page.lfo[c].param[1], 0, 255); //LFRQ
                page.lfo[c].param[2] = Common.CheckRange(page.lfo[c].param[2], 0, 127); //PMD
                page.lfo[c].param[3] = Common.CheckRange(page.lfo[c].param[3], 0, 127); //AMD
                if (page.lfo[c].param.Count == 5)
                {
                    page.lfo[c].param[4] = Common.CheckRange(page.lfo[c].param[4], 0, 1);
                }
                else
                {
                    page.lfo[c].param.Add(0);
                }
            }
        }

        public override void CmdLfoSwitch(partPage page, MML mml)
        {
            base.CmdLfoSwitch(page, mml);

            int c = (char)mml.args[0] - 'P';
            int n = (int)mml.args[1];
            if (page.lfo[c].type == eLfoType.Hardware)
            {
                ((YM2151)page.chip).OutSetHardLfo(mml, page, (n == 0) ? false : true, page.lfo[c].param);
            }
        }

        public override void CmdPan(partPage page, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 0, 3);
            page.pan = (n == 1) ? 2 : (n == 2 ? 1 : n);
            if (page.instrument < 0)
            {
                msgBox.setErrMsg(msg.get("E16004")
                    , mml.line.Lp);
            }
            else
            {
                if (parent.instOPM.ContainsKey(page.instrument))
                {
                    ((YM2151)page.chip).OutSetPanFeedbackAlgorithm(
                        mml,
                        page
                        , (int)page.pan
                        , parent.instOPM[page.instrument].Item2[46]
                        , parent.instOPM[page.instrument].Item2[45]
                        );
                }
                else
                {
                    msgBox.setErrMsg(string.Format(msg.get("E16001"),page.instrument)
                        , mml.line.Lp);
                }
            }
        }

        public override void CmdInstrument(partPage page, MML mml)
        {
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
                msgBox.setErrMsg(msg.get("E16005"), mml.line.Lp);
                return;
            }

            if (type == 'T')
            {
                if (re) n = page.toneDoubler + n;
                n = Common.CheckRange(n, 0, 255);
                page.toneDoubler = n;
                return;
            }

            if (type == 'E')
            {
                SetEnvelopParamFromInstrument(page, n,re, mml);
                return;
            }

            if (re) n = page.instrument + n;
            n = Common.CheckRange(n, 0, 255);
            if (page.instrument == n) return;

            page.instrument = n;
            int modeBeforeSend = parent.info.modeBeforeSend;
            if (type == 'N')
            {
                modeBeforeSend = 0;
            }
            else if (type == 'R')
            {
                modeBeforeSend = 1;
            }
            else if (type == 'A')
            {
                modeBeforeSend = 2;
            }

            OutSetInstrument(page, mml, n, page.volume, modeBeforeSend);
        }

        public override void CmdY(partPage page, MML mml)
        {
            if (mml.args[0] is string toneparamName)
            {
                byte op = (byte)(int)mml.args[1];
                op = (byte)(op == 1 ? 2 : (op == 2 ? 1 : op));
                byte dat = (byte)(int)mml.args[2];

                switch (toneparamName)
                {
                    case "PANFBAL":
                    case "PANFLCON":
                        SOutData(page, mml, port[0], (byte)(0x20 + page.ch), dat);
                        break;
                    case "PMSAMS":
                        SOutData(page, mml, port[0], (byte)(0x38 + page.ch), dat);
                        break;
                    case "DTML":
                    case "DTMUL":
                    case "DT1ML":
                    case "DT1MUL":
                        SOutData(page, mml, port[0], (byte)(0x40 + page.ch + op * 8), dat);
                        break;
                    case "TL":
                        SOutData(page, mml, port[0], (byte)(0x60 + page.ch + op * 8), dat);
                        break;
                    case "KSAR":
                        SOutData(page, mml, port[0], (byte)(0x80 + page.ch + op * 8), dat);
                        break;
                    case "AMDR":
                    case "AMED1R":
                        SOutData(page, mml, port[0], (byte)(0xa0 + page.ch + op * 8), dat);
                        break;
                    case "DT2SR":
                    case "DT2D2R":
                        SOutData(page, mml, port[0], (byte)(0xc0 + page.ch + op * 8), dat);
                        break;
                    case "SLRR":
                    case "D1LRR":
                        SOutData(page, mml, port[0], (byte)(0xe0 + page.ch + op * 8), dat);
                        break;
                }
            }
            else
            {
                byte adr = (byte)(int)mml.args[0];
                byte dat = (byte)(int)mml.args[1];
                SOutData(page, mml, port[0], adr, dat);
            }
        }

        public override void CmdLoopExtProc(partPage page, MML mml)
        {
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

            page.detune = Common.CheckRange(page.detune, -(9 * 12 * 64 - 1), (9 * 12 * 64 - 1));
            SetDummyData(page, mml);
        }

        public override void SetupPageData(partWork pw, partPage page)
        {

            OutKeyOff(null, page);

            //音色
            page.spg.instrument = -1;
            OutSetInstrument(page, null, page.instrument, page.volume, 'n');

            //周波数
            page.spg.freq = -1;
            SetFNum(page, null);

            //音量
            page.spg.beforeVolume = -1;
            SetVolume(page, null);

            //panは音色設定時に再設定されるので不要
        }

        public override void MultiChannelCommand(MML mml)
        {
            if (!use) return;
            //int dat = 0;

            foreach (partWork pw in lstPartWork)
            {
                partPage page = pw.cpg;

                if (page.keyOn)
                {
                    page.keyOn = false;
                    OutKeyOn(mml, page);
                }

                if (page.ch == 7 && (page.beforeNoise != page.noise|| page.beforeMixer != page.mixer))
                {
                    page.beforeNoise = page.noise;
                    page.beforeMixer = page.mixer;
                    SOutData(page, mml, port[0], 0x0f, (byte)((page.mixer << 7) | (page.noise & 0x1f)));
                }

            }


        }

    }
}