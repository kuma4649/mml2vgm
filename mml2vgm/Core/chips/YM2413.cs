using musicDriverInterface;
using System;
using System.Collections.Generic;

namespace Core
{
    public class YM2413 : ClsChip
    {
        protected int[][] _FNumTbl = new int[1][] {
            //new int[13]
            new int[] {
            // OPLL(FM) : Fnum = 9 * 2^(22-B) * ftone / M       ftone:Hz M:MasterClock B:Block
            //   c    c+     d    d+     e     f    f+     g    g+     a    a+     b    >c
             0x0ac,0x0b5,0x0c0,0x0cc,0x0d8,0x0e5,0x0f2,0x101,0x110,0x120,0x131,0x143,0x0ac*2
            }
        };

        public YM2413(ClsVgm parent, int chipID, string initialPartName, string stPath, int chipNumber) : base(parent, chipID, initialPartName, stPath, chipNumber)
        {

            _Name = "YM2413";
            _ShortName = "OPLL";
            _ChMax = 14; // FM 9ch + Rhythm 5ch
            _canUsePcm = false;

            Frequency = 3579545;
            port = new byte[][] { new byte[] { (byte)(chipNumber != 0 ? 0xa1 : 0x51) } };

            if (string.IsNullOrEmpty(initialPartName)) return;

            Dictionary<string, List<double>> dic = MakeFNumTbl();
            if (dic != null)
            {
                int c = 0;
                FNumTbl = new int[1][];
                FNumTbl[0] = new int[13];
                foreach (double v in dic["FNUM_00"])
                {
                    FNumTbl[0][c++] = (int)v;
                    if (c == FNumTbl[0].Length) break;
                }
                FNumTbl[0][FNumTbl[0].Length - 1] = FNumTbl[0][0] * 2;

            }

            Ch = new ClsChannel[ChMax];
            SetPartToCh(Ch, initialPartName);
            int i = 0;
            foreach (ClsChannel ch in Ch)
            {
                ch.Type = i < 9 ? enmChannelType.FMOPL : enmChannelType.RHYTHM;
                ch.chipNumber = chipID == 1;
                i++;
            }

        }

        public override void InitPart(partWork pw)
        {
            foreach (partPage page in pw.pg)
            {
                page.beforeVolume = -1;// (page.Type == enmChannelType.FMOPL) ? 15 : -1;
                page.volume = 0;
                page.MaxVolume = 15;
                page.beforeEnvInstrument = 0;
                page.envInstrument = 0;
                page.port = port;
                page.mixer = 0;
                page.noise = 0;
            }
        }

        public override void InitChip()
        {

            if (!use) return;

            //FM Off
            outYM2413AllKeyOff(null, lstPartWork[0].cpg);

            if (ChipID != 0 && parent.info.format != enmFormat.ZGM)
            {
                parent.dat[0x13] = new outDatum(enmMMLType.unknown, null, null, (byte)(parent.dat[0x13].val | 0x40));//use Secondary(YM2413 OPLL)
            }

        }


        public void outYM2413SetAdr00_01(MML mml, partPage page, byte adr, bool AM, bool VIB, bool EG, bool KS, int mul)
        {
            SOutData(
                page,
                mml,
                port[0]
                , (byte)(adr & 1)
                , (byte)((AM ? 0x80 : 0) + (VIB ? 0x40 : 0) + (EG ? 0x20 : 0) + (KS ? 0x10 : 0) + (mul & 0xf))
                );
        }

        public void outYM2413AllKeyOff(MML mml, partPage page)
        {
            //Rhythm Off
            SOutData(page, mml, port[0], 0x0e, 0);
            for (byte adr = 0; adr < 9; adr++)
            {
                //Ch Off
                SOutData(page, mml, port[0], (byte)(0x20 + adr), 0);
                SOutData(page, mml, port[0], (byte)(0x30 + adr), 0);
            }
        }

        public void outYM2413SetInstrument(partPage page, MML mml, int n, int modeBeforeSend)
        {
            page.instrument = n;

            if (!parent.instOPL.ContainsKey(n))
            {
                msgBox.setWrnMsg(string.Format(msg.get("E17000"), n), mml.line.Lp);
                return;
            }

            switch (modeBeforeSend)
            {
                case 0: // N)one
                    break;
                case 1: // R)R only
                    for (int ope = 0; ope < 2; ope++)
                    {
                        SOutData(page, mml, port[0], (byte)(0x6 + ope), (byte)((
                            (0 & 0xf) << 4) //SL
                            | (15 & 0xf) // RR
                            ));
                    }
                    break;
                case 2: // A)ll
                    for (byte ope = 0; ope < 2; ope++)
                    {
                        outYM2413SetAdr00_01(mml, page, ope
                            , false //AM
                            , false //VIB
                            , false //EG
                            , false //KS
                            , 0 //MT
                            );
                        SOutData(page, mml, port[0], (byte)(0x4 + ope), (byte)((
                            (15 & 0xf) << 4) //AR
                            | (15 & 0xf) // DR
                            ));
                        SOutData(page, mml, port[0], (byte)(0x6 + ope), (byte)((
                            (0 & 0xf) << 4) //SL
                            | (15 & 0xf) // RR
                            ));
                    }
                    SOutData(page, mml, port[0], (byte)(0x2), (byte)(
                        (0 << 6)  //KL(M)
                        | (0 & 0x3f) //TL
                        ));
                    SOutData(page, mml, port[0], (byte)(0x3), (byte)((
                        (3 & 0x3) << 6) //KL(C)
                        | (0) // DT(M)
                        | (0) // DT(C)
                        | (7 & 0x07) //FB
                        ));
                    break;
            }

            for (byte ope = 0; ope < 2; ope++)
            {
                outYM2413SetAdr00_01(mml, page, ope
                    , parent.instOPL[n].Item2[ope * 11 + 7] != 0 //AM
                    , parent.instOPL[n].Item2[ope * 11 + 8] != 0 //VIB
                    , parent.instOPL[n].Item2[ope * 11 + 9] != 0 //EG
                    , parent.instOPL[n].Item2[ope * 11 + 10] != 0 //KS
                    , parent.instOPL[n].Item2[ope * 11 + 6] & 0xf //MT
                    );
                SOutData(page, mml, port[0], (byte)(0x4 + ope), (byte)((
                    (parent.instOPL[n].Item2[ope * 11 + 1] & 0xf) << 4) //AR
                    | (parent.instOPL[n].Item2[ope * 11 + 2] & 0xf) // DR
                    ));
                SOutData(page, mml, port[0], (byte)(0x6 + ope), (byte)((
                    (parent.instOPL[n].Item2[ope * 11 + 3] & 0xf) << 4) //SL
                    | (parent.instOPL[n].Item2[ope * 11 + 4] & 0xf) // RR
                    ));
            }
            SOutData(page, mml, port[0], (byte)(0x2), (byte)((
                (parent.instOPL[n].Item2[0 * 11 + 5] & 0x3) << 6)  //KL(M)
                | (parent.instOPL[n].Item2[23] & 0x3f) //TL
                ));
            SOutData(page, mml, port[0], (byte)(0x3), (byte)((
                (parent.instOPL[n].Item2[1 * 11 + 5] & 0x3) << 6) //KL(C)
                | (parent.instOPL[n].Item2[0 * 11 + 11] != 0 ? 0x08 : 0) // DT(M)
                | (parent.instOPL[n].Item2[1 * 11 + 11] != 0 ? 0x10 : 0) // DT(C)
                | (parent.instOPL[n].Item2[24] & 0x07) //FB
                ));

            page.op1ml = parent.instOPL[n].Item2[0 * 11 + 5];
            page.op2ml = parent.instOPL[n].Item2[1 * 11 + 5];
            page.op1dt2 = 0;
            page.op2dt2 = 0;

            page.spg.beforeVolume = -1;
        }

        //public void outYM2413SetInstVol(partWork pw, int inst, int vol)
        //{
        //    pw.ppg[pw.cpgNum].envInstrument = inst & 0xf;
        //    pw.ppg[pw.cpgNum].volume = vol & 0xf;

        //    SOutData(page,pw.ppg[pw.cpgNum].port0
        //        , (byte)(0x30 + pw.ppg[pw.cpgNum].ch)
        //        , (byte)((pw.ppg[pw.cpgNum].envInstrument << 4) | (15 - pw.ppg[pw.cpgNum].volume))
        //        );
        //}
        public override void GetFNumAtoB(partPage page, MML mml
            , out int a, int aOctaveNow, char aCmd, int aShift, int aPitchShift
            , out int b, int bOctaveNow, char bCmd, int bShift, int bPitchShift
            , int dir)
        {
            a = GetFNum(page, mml, aOctaveNow, aCmd, aShift, aPitchShift);
            b = GetFNum(page, mml, bOctaveNow, bCmd, bShift, bPitchShift);

            int oa = (a & 0x0e00) >> 9;
            int ob = (b & 0x0e00) >> 9;
            if (oa != ob)
            {
                if (((a - aPitchShift) & 0x1ff) == FNumTbl[0][0])
                {
                    oa += Math.Sign(ob - oa);
                    a = (a & 0x1ff) * 2 + (oa << 9);
                }
                else if (((b - bPitchShift) & 0x1ff) == FNumTbl[0][0])
                {
                    ob += Math.Sign(oa - ob);
                    b = (b & 0x1ff) * ((dir > 0) ? 2 : 1) + (ob << 9);
                }
            }
        }

        public void OutFmSetFnum(partPage page, int octave, int num)
        {
            int freq;
            freq = (int)((num & 0x1ff) | (((octave - 1) & 0x7) << 9));
            page.freq = freq;
        }

        public void SetFmFNum(partPage page, MML mml)
        {
            if (page.noteCmd == (char)0)
            {
                return;
            }

            int[] ftbl = FNumTbl[0];
            int arpNote = page.arpFreqMode ? 0 : page.arpDelta;
            int arpFreq = page.arpFreqMode ? page.arpDelta : 0;

            int f = GetFmFNum(ftbl, page.octaveNow, page.noteCmd, page.shift + page.keyShift + page.toneDoublerKeyShift + arpNote, page.pitchShift);//
            if (page.bendWaitCounter != -1)
            {
                f = page.bendFnum;
            }
            int o = (f & 0x0e00) / 0x0200;
            f &= 0x1ff;

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
            f = Common.CheckRange(f, 0, 0x1ff);
            OutFmSetFnum(page, o, f);
        }

        public int GetFmFNum(int[] ftbl, int octave, char noteCmd, int shift, int pitchShift)
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

            return (f & 0x1ff) + (o & 0x7) * 0x0200 + pitchShift;
        }

        public void OutFmSetTL(partPage page, MML mml, int tl1, int n)
        {
            if (!parent.instOPL.ContainsKey(n))
            {
                msgBox.setWrnMsg(string.Format(msg.get("E11000"), n), mml.line.Lp);
                return;
            }

            int ope = parent.instOPL[n].Item2[23] & 0x3f;
            ope = ope - tl1;
            ope = Common.CheckRange(ope, 0, 127);
            int kl = parent.instOPL[n].Item2[0 * 11 + 5] & 0x3;

            partPage vpg = page;
            MML vmml = new MML();
            vmml.args = new List<object>();
            vmml.args.Add(tl1);
            vmml.type = enmMMLType.unknown;//.TotalLevel;
            if (mml != null)
                vmml.line = mml.line;
            OutFmSetKLTl(vmml, vpg,kl, ope);
        }

        public void OutFmSetKLTl(MML mml, partPage page,int kl, int tl)
        {
            kl &= 0x3;
            tl &= 0x3f;
            SOutData(page, mml, port[0], (byte)(0x2), (byte)((kl << 6) | tl));
        }

        public void SetFmVolume(partPage page, MML mml)
        {
            //int vol = page.volume;

            //for (int lfo = 0; lfo < 4; lfo++)
            //{
            //    if (!page.lfo[lfo].sw)
            //    {
            //        continue;
            //    }
            //    if (page.lfo[lfo].type != eLfoType.Tremolo)
            //    {
            //        continue;
            //    }
            //    vol += page.lfo[lfo].value + page.lfo[lfo].param[6];
            //}

            //if (page.varpeggioMode)
            //{
            //    vol += page.varpDelta;
            //}

            ////if (pw.ppg[pw.cpgNum].beforeVolume != vol)
            ////{
            ////if (parent.instOPL.ContainsKey(pw.ppg[pw.cpgNum].instrument))
            ////{
            //page.volume = Common.CheckRange(vol, 0, 15);
            ////outYM2413SetInstVol(pw, pw.ppg[pw.cpgNum].envInstrument, vol);
            ////pw.ppg[pw.cpgNum].beforeVolume = vol;
            ////}
            ////}
        }

        public void SetFmTL(partPage page, MML mml)
        {
            int tl1 = page.tlDelta1;

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

                if ((page.lfo[lfo].slot & 1) != 0) tl1 += page.lfo[lfo].value + page.lfo[lfo].param[1 + 6];
            }

            if (page.spg.beforeTlDelta1 != tl1)
            {
                if (parent.instOPL.ContainsKey(page.instrument))
                {
                    OutFmSetTL(page, mml, tl1, page.instrument);
                }
                page.spg.beforeTlDelta1 = tl1;
            }
        }

        public override void SetFNum(partPage page, MML mml)
        {
            SetFmFNum(page, mml);
        }

        public override int GetFNum(partPage page, MML mml, int octave, char cmd, int shift, int pitchShift)
        {
            int[] ftbl = FNumTbl[0];
            return GetFmFNum(ftbl, octave, cmd, shift, pitchShift);
        }

        public override void SetVolume(partPage page, MML mml)
        {
            SetFmVolume(page, mml);
            if (page.Type == enmChannelType.FMOPL)
            {
                SetFmTL(page, mml);
            }
        }

        public override void SetKeyOn(partPage page, MML mml)
        {
            page.keyOn = true;
            SetDummyData(page, mml);
        }

        public override void SetKeyOff(partPage page, MML mml)
        {
            page.keyOn = false;
            page.keyOff = true;
        }

        public override void SetLfoAtKeyOn(partPage page, MML mml)
        {
            for (int lfo = 0; lfo < 4; lfo++)
            {
                clsLfo pl = page.lfo[lfo];
                if (!pl.sw)
                    continue;

                int w = 0;
                if (pl.type == eLfoType.Wah) w = 1;

                if (pl.param[w + 5] != 1)
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
                    if (page.Type == enmChannelType.FMOPL)
                        SetFmFNum(page, mml);

                }

                if (pl.type == eLfoType.Tremolo)
                {
                    page.beforeVolume = -1;
                    if (page.Type == enmChannelType.FMOPL)
                        SetFmVolume(page, mml);
                }

                if (pl.type == eLfoType.Wah)
                {
                    page.beforeVolume = -1;
                    if (page.Type == enmChannelType.FMOPL)
                        SetFmTL(page, mml);
                }

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

            if (type == 'T')
            {
                msgBox.setErrMsg(msg.get("E17001"), mml.line.Lp);
                return;
            }

            if (type == 'E')
            {
                n = SetEnvelopParamFromInstrument(page, n, re, mml);
                return;
            }

            if (type == 'I')
            {
                if (re) n = page.envInstrument + n;
                n = Common.CheckRange(n, 1, 15);
                if (page.envInstrument != n)
                {
                    page.envInstrument = n;
                    //outYM2413SetInstVol(pw, n, pw.ppg[pw.cpgNum].volume); //INSTをnにセット
                }
                SetDummyData(page, mml);
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

            outYM2413SetInstrument(page, mml, n, modeBeforeSend); //音色のセット
            page.envInstrument = 0;
            //outYM2413SetInstVol(pw, 0, pw.ppg[pw.cpgNum].volume); //INSTを0にセット

        }

        public override void CmdMode(partPage page, MML mml)
        {
            int n = (int)mml.args[0];
            page.chip.lstPartWork[9].spg.rhythmMode = (n != 0);
            page.chip.lstPartWork[10].spg.rhythmMode = (n != 0);
            page.chip.lstPartWork[11].spg.rhythmMode = (n != 0);
            page.chip.lstPartWork[12].spg.rhythmMode = (n != 0);
            page.chip.lstPartWork[13].spg.rhythmMode = (n != 0);

        }

        public override void CmdY(partPage page, MML mml)
        {
            byte adr = (byte)(int)mml.args[0];
            byte dat = (byte)(int)mml.args[1];
            SOutData(page, mml, port[0], adr, dat);
        }

        public override void CmdLoopExtProc(partPage page, MML mml)
        {
        }

        public override void CmdSusOnOff(partPage page, MML mml)
        {
            char c = (char)mml.args[0];
            page.sus = (c == 'o');
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
            page.detune = Common.CheckRange(page.detune, -0x1ff, 0x1ff);

            SetDummyData(page, mml);

        }

        public override void SetupPageData(partWork pw, partPage page)
        {
            page.keyOff = true;
            page.spg.instrument = -1;

            page.spg.beforeEnvInstrument = -1;

            //周波数
            page.spg.beforeFNum = -1;

            //音量
            page.spg.beforeVolume = -1;
            SetVolume(page, null);

        }

        public override void MultiChannelCommand(MML mml)
        {
            foreach (partWork pw in lstPartWork)
            {

                partPage page = pw.cpg;
                if (page.Type != enmChannelType.FMOPL) continue;

                if (page.spg.beforeEnvInstrument != page.envInstrument)// || page.spg.beforeVolume != page.volume)
                {
                    page.spg.beforeEnvInstrument = page.envInstrument;
                }

                int vol = page.volume;

                if (page.envIndex != -1) vol += page.envVolume;

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

                vol = Common.CheckRange(vol, 0, 15);

                if (page.spg.beforeInstrument != page.envInstrument || page.spg.beforeVolume != vol)
                {
                    page.spg.beforeVolume = vol;
                    page.spg.beforeInstrument = page.envInstrument;
                    SOutData(page, mml, port[0]
                        , (byte)(0x30 + page.ch)
                        , (byte)(((page.envInstrument << 4) & 0xf0) | ((15 - vol) & 0xf))
                        );
                }

                if (page.keyOff)
                {
                    page.keyOff = false;
                    SOutData(page, mml, port[0]
                        , (byte)(0x20 + page.ch)
                        , (byte)(
                            ((page.freq >> 8) & 0xf)
                            | (page.sus ? 0x20 : 0x00)
                          )
                        );
                    page.spg.beforeFNum = page.freq;
                }

                if (page.freq != -1 && page.spg.beforeFNum != (page.freq | (page.keyOn ? 0x1000 : 0x0000)))
                {
                    page.spg.beforeFNum = page.freq | (page.keyOn ? 0x1000 : 0x0000);

                    SOutData(page, mml, port[0], (byte)(0x10 + page.ch), (byte)page.freq);
                    SOutData(page, mml, port[0]
                        , (byte)(0x20 + page.ch)
                        , (byte)(
                            ((page.freq >> 8) & 0xf)
                            | (page.keyOn ? 0x10 : 0x00)
                            | (page.sus ? 0x20 : 0x00)
                          )
                        );
                }

            }

            if (!lstPartWork[9].spg.rhythmMode) return;

            partWork p0, p1;
            byte dat;
            p0 = lstPartWork[9];

            //Key Off
            if (lstPartWork[9].cpg.keyOff
                || lstPartWork[10].cpg.keyOff
                || lstPartWork[11].cpg.keyOff
                || lstPartWork[12].cpg.keyOff
                || lstPartWork[13].cpg.keyOff)
            {
                dat = (byte)(0x20
                    | (lstPartWork[9].cpg.keyOn ? (lstPartWork[9].cpg.keyOff ? 0 : 0x10) : 0)
                    | (lstPartWork[10].cpg.keyOn ? (lstPartWork[10].cpg.keyOff ? 0 : 0x08) : 0)
                    | (lstPartWork[11].cpg.keyOn ? (lstPartWork[11].cpg.keyOff ? 0 : 0x04) : 0)
                    | (lstPartWork[12].cpg.keyOn ? (lstPartWork[12].cpg.keyOff ? 0 : 0x02) : 0)
                    | (lstPartWork[13].cpg.keyOn ? (lstPartWork[13].cpg.keyOff ? 0 : 0x01) : 0)
                    );
                lstPartWork[9].cpg.rhythmKeyOnData = dat;
                SOutData(lstPartWork[9].cpg, mml, port[0], 0x0e, dat);

                lstPartWork[9].cpg.keyOff = false;
                lstPartWork[10].cpg.keyOff = false;
                lstPartWork[11].cpg.keyOff = false;
                lstPartWork[12].cpg.keyOff = false;
                lstPartWork[13].cpg.keyOff = false;
            }


            //Key On
            dat = (byte)(0x20
                | (lstPartWork[9].cpg.keyOn ? 0x10 : 0)
                | (lstPartWork[10].cpg.keyOn ? 0x08 : 0)
                | (lstPartWork[11].cpg.keyOn ? 0x04 : 0)
                | (lstPartWork[12].cpg.keyOn ? 0x02 : 0)
                | (lstPartWork[13].cpg.keyOn ? 0x01 : 0)
                );
            if (lstPartWork[9].cpg.rhythmKeyOnData != dat)
            {
                lstPartWork[9].cpg.rhythmKeyOnData = dat;
                SOutData(lstPartWork[9].cpg, mml, port[0], 0x0e, dat);
            }


            //Freq
            p0 = lstPartWork[9];
            if (p0.cpg.freq != -1 && p0.spg.beforeFNum != p0.cpg.freq)
            {
                p0.spg.beforeFNum = p0.cpg.freq;

                SOutData(p0.cpg, mml, port[0], (byte)0x16, (byte)p0.cpg.freq);
                SOutData(p0.cpg, mml, port[0]
                    , (byte)0x26
                    , (byte)(
                              ((p0.cpg.freq >> 8) & 0xf)
                            | (p0.cpg.sus ? 0x20 : 0x00)
                            )
                    );
            }

            p0 = lstPartWork[10];
            p1 = lstPartWork[13];
            if ((p0.cpg.freq != -1 && p0.spg.beforeFNum != p0.cpg.freq)
                || (p1.cpg.freq != -1 && p1.spg.beforeFNum != p1.cpg.freq))
            {
                if (p1.cpg.freq != -1 && p1.spg.beforeFNum != p1.cpg.freq)
                {
                    p0.spg.beforeFNum = p1.cpg.freq;
                    p1.spg.beforeFNum = p1.cpg.freq;
                }
                else if (p0.cpg.freq != -1 && p0.spg.beforeFNum != p0.cpg.freq)
                {
                    p0.spg.beforeFNum = p0.cpg.freq;
                    p1.spg.beforeFNum = p0.cpg.freq;
                }

                if (p0.spg.beforeFNum != -1)
                {
                    SOutData(p0.cpg, mml, port[0], (byte)0x17, (byte)p0.spg.beforeFNum);
                    SOutData(p0.cpg, mml, port[0]
                        , (byte)0x27
                        , (byte)(
                            ((p0.spg.beforeFNum >> 8) & 0xf)
                            | (p0.cpg.sus ? 0x20 : 0x00)
                            )
                        );
                }
            }

            p0 = lstPartWork[12];
            p1 = lstPartWork[11];
            if ((p0.cpg.freq != -1 && p0.spg.beforeFNum != p0.cpg.freq)
                || (p1.cpg.freq != -1 && p1.spg.beforeFNum != p1.cpg.freq))
            {
                if (p1.cpg.freq != -1 && p1.spg.beforeFNum != p1.cpg.freq)
                {
                    p0.spg.beforeFNum = p1.cpg.freq;
                    p1.spg.beforeFNum = p1.cpg.freq;
                }
                else if (p0.cpg.freq != -1 && p0.spg.beforeFNum != p0.cpg.freq)
                {
                    p0.spg.beforeFNum = p0.cpg.freq;
                    p1.spg.beforeFNum = p0.cpg.freq;
                }

                if (p0.spg.beforeFNum != -1)
                {
                    SOutData(p0.cpg, mml, port[0], (byte)0x18, (byte)p0.spg.beforeFNum);
                    SOutData(p0.cpg, mml, port[0]
                        , (byte)0x28
                        , (byte)(
                            ((p0.spg.beforeFNum >> 8) & 0xf)
                            | (p0.cpg.sus ? 0x20 : 0x00)
                            )
                        );
                }
            }


            //Rhythm Volume
            p0 = lstPartWork[9];
            if (p0.spg.beforeVolume != p0.cpg.volume)
            {
                p0.spg.beforeVolume = p0.cpg.volume;
                SOutData(p0.cpg, mml, port[0], 0x36, (byte)(15 - (p0.cpg.volume & 0xf)));
            }
            p0 = lstPartWork[10];
            p1 = lstPartWork[13];
            if (p0.spg.beforeVolume != p0.cpg.volume || p1.spg.beforeVolume != p1.cpg.volume)
            {
                p0.spg.beforeVolume = p0.cpg.volume;
                p1.spg.beforeVolume = p1.cpg.volume;
                SOutData(p0.cpg, mml, port[0], 0x37, (byte)((15 - (p0.cpg.volume & 0xf)) | ((15 - (p1.cpg.volume & 0xf)) << 4)));
            }
            p0 = lstPartWork[12];
            p1 = lstPartWork[11];
            if (p0.spg.beforeVolume != p0.cpg.volume || p1.spg.beforeVolume != p1.cpg.volume)
            {
                p0.spg.beforeVolume = p0.cpg.volume;
                p1.spg.beforeVolume = p1.cpg.volume;
                SOutData(p0.cpg, mml, port[0], 0x38, (byte)((15 - (p0.cpg.volume & 0xf)) | ((15 - (p1.cpg.volume & 0xf)) << 4)));
            }


        }

    }
}
