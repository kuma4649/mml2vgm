using musicDriverInterface;
using System;
using System.Collections.Generic;

namespace Core
{
    public class ClsOPL : ClsChip
    {
        protected int[][] _FNumTbl = new int[1][] {
            //new int[13]
            new int[] {
            // OPL/2(FM) : Fnum = ftone*(2**19)/(M/ 72)/(2**B-1)       ftone:Hz M:MasterClock B:Block
            // OPL3(FM)  : Fnum = ftone*(2**19)/(M/288)/(2**B-1)       ftone:Hz M:MasterClock B:Block
            //   c    c+     d    d+     e     f    f+     g    g+     a    a+     b    >c
             0x158,0x16a,0x180,0x198,0x1b0,0x1ca,0x1e4,0x202,0x220,0x240,0x262,0x286,0x2b0
            }
        };

        public ClsOPL(ClsVgm parent, int chipID, string initialPartName, string stPath, int chipNumber) : base(parent, chipID, initialPartName, stPath, chipNumber)
        {
        }

        public override void InitChip()
        {

            if (!use) return;

            //FM Off
            outAllKeyOff(null, lstPartWork[0]);
            rhythmStatus = 0x00;
            beforeRhythmStatus = 0xff;
            connectionSel = 0;
            beforeConnectionSel = -1;

        }

        public override void InitPart(partWork pw)
        {
            foreach (partPage pg in pw.pg)
            {
                pg.beforeVolume = -1;
                pg.volume = 60;
                pg.MaxVolume = 63;
                pg.beforeEnvInstrument = 0;
                pg.envInstrument = 0;
                pg.port = port;
                pg.mixer = 0;
                pg.noise = 0;
                pg.pan = 3;
                pg.Type = enmChannelType.FMOPL;
                pg.isOp4Mode = false;
                if (pg.ch > 8)
                    pg.Type = enmChannelType.RHYTHM;
            }
        }

        public virtual byte ChnToBaseReg(int chn)
        {
            //Console.Write("Enter ChnToBaseReg: Ch{0}", chn + 1);
            chn %= 9; // A1=LでもA1=Hでもいっしょ。
            byte carrier = (byte)((chn / 3) * 8 + (chn % 3));
            return carrier;
        }

        public virtual byte[] getPortFromCh(int ch)
        {

            if (ch >= 9)
            {
                //Console.WriteLine("getPortFromCh port1");
                return port[1];
            }
            //Console.WriteLine("getPortFromCh port0");
            return port[0];
        }

        public virtual void outAllKeyOff(MML mml, partWork pw)
        {
            parent.OutData(mml, port[0], 0xBD, 0);
            // Probably wise to reset Rhythm mode.
            for (byte adr = 0; adr <= 8; adr++)
            {
                //Ch Off
                parent.OutData(mml, port[0], (byte)(0xB0 + adr), 0);
            }
        }

        public virtual void outOPLSetInstrument(partPage page, MML mml, int n, int modeBeforeSend)
        {
            page.instrument = n;

            if (!parent.instOPL.ContainsKey(n))
            {
                msgBox.setWrnMsg(string.Format(msg.get("E17000"), n), mml.line.Lp);
                return;
            }

            if (page.Type == enmChannelType.RHYTHM)
            {
                SetInstToRhythmChannel(page, mml, n, modeBeforeSend);
                return;
            }

            if (parent.instOPL[n].Item2.Length == Const.OPL_OP4_INSTRUMENT_SIZE)
            {
                SetInst4Operator(page, mml, n, modeBeforeSend, page.ch);
                return;
            }

            if (parent.instOPL[n].Item2.Length == Const.OPL3_INSTRUMENT_SIZE)
            {
                SetInst2Operator(page, mml, n, modeBeforeSend, page.ch);
                return;
            }

            msgBox.setErrMsg(string.Format(msg.get("E17002"), n), mml.line.Lp);
            page.instrument = -1;
        }

        protected virtual void SetInstToRhythmChannel(partPage page, MML mml, int n, int modeBeforeSend)
        {
            if (rhythmStatus == 0) return;

            if (page.ch == 9)//BD
            {
                int vch = 6;
                SetInst2Operator(page, mml, n, modeBeforeSend, vch);
            }
            else if (page.ch == 10)//SD
            {
                int opeNum = 16;
                SetInst1Operator(page, mml, n, modeBeforeSend, opeNum);
            }
            else if (page.ch == 11)//TOM
            {
                int opeNum = 14;
                SetInst1Operator(page, mml, n, modeBeforeSend, opeNum);
            }
            else if (page.ch == 12)//CYM
            {
                int opeNum = 17;
                SetInst1Operator(page, mml, n, modeBeforeSend, opeNum);
            }
            else if (page.ch == 13)//HH
            {
                int opeNum = 13;
                SetInst1Operator(page, mml, n, modeBeforeSend, opeNum);
            }
        }

        protected virtual void SetInst1Operator(partPage page, MML mml, int n, int modeBeforeSend, int opeNum)
        {
            byte[] inst = parent.instOPL[n].Item2;
            int targetBaseReg = (opeNum / 6) * 8 + (opeNum % 6);
            byte[] port = this.port[opeNum / 18];
            int ope = (opeNum % 6) / 3;

            switch (modeBeforeSend)
            {
                case 0: // N)one
                    break;
                case 1: // R)R only
                    SOutData(page, mml, port, (byte)(targetBaseReg + ope * 3 + 0x80)
                        , ((0 & 0xf) << 4) | (15 & 0xf));//SL RR
                    break;
                case 2: // A)ll
                    SetInstAtOneOpeWithoutKslTl(page, mml, opeNum
                        , 15, 15, 0, 15, 0, 0, 0, 0, 0, 0);
                    SOutData(page, mml, port, (byte)(targetBaseReg + ope * 3 + 0x40)
                        , ((0 & 0x3) << 6) | 0x3f);  //KL(M) TL
                    break;
            }

            SetInstAtOneOpeWithoutKslTl(page, mml, opeNum,
                inst[ope * 12 + 1 + 0],//AR
                inst[ope * 12 + 1 + 1],//DR
                inst[ope * 12 + 1 + 2],//SL
                inst[ope * 12 + 1 + 3],//RR
                inst[ope * 12 + 1 + 6],//MT
                inst[ope * 12 + 1 + 7],//AM
                inst[ope * 12 + 1 + 8],//VIB
                inst[ope * 12 + 1 + 9],//EGT
                inst[ope * 12 + 1 + 10],//KSR
                inst[ope * 12 + 1 + 11]//WS
            );

            int cnt = inst[25];
            if (cnt == 0 || page.Type == enmChannelType.RHYTHM)
            {
                if (ope == 0)
                {
                    //OP1
                    SOutData(page, mml, port, (byte)(0x40 + targetBaseReg + 0)
                        , (byte)(((inst[12 * 0 + 5] & 0x3) << 6) | (inst[12 * 0 + 6] & 0x3f))); //KL(M) TL
                }
            }

            SetInstAtChannelPanFbCnt(page, mml, (opeNum % 6) % 3 + (opeNum / 6) * 3, (int)page.pan, inst[26], inst[25]);

            page.beforeVolume = -1;
        }

        protected virtual void SetInst2Operator(partPage page, MML mml, int n, int modeBeforeSend, int vch)
        {
            byte[] inst = parent.instOPL[n].Item2;
            byte targetBaseReg = ChnToBaseReg(vch);
            byte[] port = getPortFromCh(vch);

            switch (modeBeforeSend)
            {
                case 0: // N)one
                    break;
                case 1: // R)R only
                    for (int ope = 0; ope < 2; ope++)
                        SOutData(page, mml, port, (byte)(targetBaseReg + ope * 3 + 0x80)
                            , ((0 & 0xf) << 4) | (15 & 0xf));//SL RR
                    break;
                case 2: // A)ll
                    for (byte ope = 0; ope < 2; ope++)
                    {
                        SetInstAtOneOpeWithoutKslTl(page, mml, (vch / 3 * 6) + (vch % 3) + ope * 3
                            , 15, 15, 0, 15, 0, 0, 0, 0, 0, 0);
                        SOutData(page, mml, port, (byte)(targetBaseReg + ope * 3 + 0x40)
                            , ((0 & 0x3) << 6) | 0x3f);  //KL(M) TL
                    }
                    break;
            }

            int slot1_operatorNumber = (vch / 3 * 6) + (vch % 3) + 0;

            for (int ope = 0; ope < 2; ope++)
            {
                SetInstAtOneOpeWithoutKslTl(page, mml, slot1_operatorNumber + ope * 3,
                    inst[ope * 12 + 1 + 0],
                    inst[ope * 12 + 1 + 1],
                    inst[ope * 12 + 1 + 2],
                    inst[ope * 12 + 1 + 3],
                    inst[ope * 12 + 1 + 6],
                    inst[ope * 12 + 1 + 7],
                    inst[ope * 12 + 1 + 8],
                    inst[ope * 12 + 1 + 9],
                    inst[ope * 12 + 1 + 10],
                    inst[ope * 12 + 1 + 11]
                    );
            }

            //TLはvolumeの設定と一緒に行うがキャリアのみである。
            //そのため、CNT0の場合はモジュレータのパラメータをセットする必要がある
            int cnt = inst[25];
            if (cnt == 0)
            {
                //OP1
                SOutData(page, mml, port, (byte)(0x40 + ChnToBaseReg(vch) + 0)
                    , (byte)(((inst[12 * 0 + 5] & 0x3) << 6) | (inst[12 * 0 + 6] & 0x3f))); //KL(M) TL
            }

            SetInstAtChannelPanFbCnt(page, mml, vch, (int)page.pan, inst[26], inst[25]);

            page.beforeVolume = -1;
        }

        protected virtual void SetInst4Operator(partPage page, MML mml, int n, int modeBeforeSend, int vch)
        {
            if (!page.isOp4Mode)
            {
                msgBox.setErrMsg(string.Format(msg.get("E26000"), n), mml.line.Lp);
                return;
            }

            byte[] inst = parent.instOPL[n].Item2;
            byte targetBaseReg = ChnToBaseReg(vch);
            byte[] port = getPortFromCh(vch);

            switch (modeBeforeSend)
            {
                case 0: // N)one
                    break;
                case 1: // R)R only
                    for (int ope = 0; ope < 2; ope++)
                        SOutData(page, mml, port, (byte)(targetBaseReg + ope * 3 + 0x80)
                            , ((0 & 0xf) << 4) | (15 & 0xf));//SL RR
                    break;
                case 2: // A)ll
                    for (byte ope = 0; ope < 2; ope++)
                    {
                        SetInstAtOneOpeWithoutKslTl(page, mml, (vch / 3 * 6) + (vch % 3) + ope * 3
                            , 15, 15, 0, 15, 0, 0, 0, 0, 0, 0);
                        SOutData(page, mml, port, (byte)(targetBaseReg + ope * 3 + 0x40)
                            , ((0 & 0x3) << 6) | 0x3f);  //KL(M) TL
                    }
                    break;
            }

            int slot1_operatorNumber = (vch / 3 * 6) + (vch % 3) + 0;

            for (int ope = 0; ope < 4; ope++)
            {
                SetInstAtOneOpeWithoutKslTl(page, mml, slot1_operatorNumber + ope * 3,
                    inst[ope * 12 + 1 + 0],
                    inst[ope * 12 + 1 + 1],
                    inst[ope * 12 + 1 + 2],
                    inst[ope * 12 + 1 + 3],
                    inst[ope * 12 + 1 + 6],
                    inst[ope * 12 + 1 + 7],
                    inst[ope * 12 + 1 + 8],
                    inst[ope * 12 + 1 + 9],
                    inst[ope * 12 + 1 + 10],
                    inst[ope * 12 + 1 + 11]
                    );
            }

            //TLはvolumeの設定と一緒に行うがキャリアのみである。
            //そのため、CNT0の場合はモジュレータのパラメータをセットする必要がある
            int cnt1 = inst[49];
            int cnt2 = inst[50];
            bool op1 = false;
            bool op2 = false;
            bool op3 = false;

            if (cnt1 == 0 && cnt2 == 0) { op1 = true; op2 = true; op3 = true; }
            else if (cnt1 == 0 && cnt2 == 1) { op1 = true; op3 = true; }
            else if (cnt1 == 1 && cnt2 == 0) { op2 = true; op3 = true; }
            else if (cnt1 == 1 && cnt2 == 1) { op2 = true; }

            if (op1)
                SOutData(page, mml, port, (byte)(0x40 + ChnToBaseReg(vch) + 0)
                    , (byte)(((inst[12 * 0 + 5] & 0x3) << 6) | (inst[12 * 0 + 6] & 0x3f))); //KL(M) TL

            if (op2)
                SOutData(page, mml, port, (byte)(0x40 + ChnToBaseReg(vch) + 3)
                    , (byte)(((inst[12 * 1 + 5] & 0x3) << 6) | (inst[12 * 1 + 6] & 0x3f))); //KL(M) TL

            if (op3)
                SOutData(page, mml, port, (byte)(0x40 + ChnToBaseReg(vch) + 8)
                    , (byte)(((inst[12 * 2 + 5] & 0x3) << 6) | (inst[12 * 2 + 6] & 0x3f))); //KL(M) TL


            SetInstAtChannelPanFbCnt(page, mml, vch, (int)page.pan, inst[51], cnt1);
            SetInstAtChannelPanFbCnt(page, mml, vch + 3, (int)page.pan, inst[51], cnt2);

            page.beforeVolume = -1;
        }

        protected virtual void SetInstAtOneOpeWithoutKslTl(partPage page, MML mml, int opeNum,
            int ar, int dr, int sl, int rr,
            int mt, int am, int vib, int eg,
            int kr,
            int ws
            )
        {
            //portは18operator毎に切り替わる
            byte[] port = this.port[opeNum / 18];

            // % 18       ... port毎のoperator番号を得る --- (1)
            // / 6 ) * 8  ... (1) に対応するアドレスは6opeごとに8アドレス毎に分けられ、
            // % 6        ...                         0～5アドレスに割り当てられている
            int adr = ((opeNum % 18) / 6) * 8 + (opeNum % 6);

            ////slot1かslot2を求める
            //// % 6        ... slotは6opeの範囲で0か1を繰り返す
            //// / 3        ... slotは3ope毎に0か1を繰り返す
            //int slot = (opeNum % 6) / 3;

            SOutData(page, mml, port, (byte)(0x80 + adr), (byte)(((sl & 0xf) << 4) | (rr & 0xf)));
            SOutData(page, mml, port, (byte)(0x60 + adr), (byte)(((ar & 0xf) << 4) | (dr & 0xf)));
            SetInstAtOneOpeAmVibEgKsMl(page, mml, port, (byte)(0x20 + adr), mt, am, vib, eg, kr);
            //SOutData(page,mml, port, (byte)(0xe0 + adr), (byte)(ws & 0x7));
        }

        protected virtual void SetInstAtChannelPanFbCnt(partPage page, MML mml, int chNum, int pan, int fb, int cnt)
        {
            //portは9channel毎に切り替わる
            byte[] port = this.port[chNum / 9];

            SOutData(page,
                mml,
                port,
                (byte)(chNum % 9 + 0xC0),
                (byte)(
                    ((fb & 0x07) << 1) | (cnt & 0x01) | (pan * 0x10) // PAN(CHA,CHB (CHC,CHDは未使用))
                )
            );
        }

        public virtual void SetInstAtOneOpeAmVibEgKsMl(partPage page, MML mml, byte[] port, byte adr, int ml, int am, int vib, int eg, int kr)
        {
            // 0x20
            SOutData(page,
                mml,
                port,
                adr,
                 (byte)((am != 0 ? 0x80 : 0) + (vib != 0 ? 0x40 : 0) + (eg != 0 ? 0x20 : 0) + (kr != 0 ? 0x10 : 0) + (ml & 0xf))
                );
        }


        public virtual void OutFmSetFnum(partPage page, int octave, int num)
        {
            int freq;
            freq = (int)((num & 0x3ff) | (((octave - 1) & 0x7) << 10));
            page.freq = freq;
        }

        public virtual void SetFmFNum(partPage page, MML mml)
        {
            if (page.noteCmd == (char)0)
            {
                return;
            }

            //if (pw.ppg[pw.cpgNum].Type == enmChannelType.RHYTHM) return;

            int[] ftbl = FNumTbl[0];

            int arpNote = page.arpFreqMode ? 0 : page.arpDelta;
            int arpFreq = page.arpFreqMode ? page.arpDelta : 0;

            int f = GetFmFNum(ftbl, page.octaveNow, page.noteCmd, page.shift + page.keyShift + page.toneDoublerKeyShift + arpNote, page.pitchShift);//
            if (page.bendWaitCounter != -1)
            {
                f = page.bendFnum;
                //Console.Write("ff:{0}   ", f);
            }
            int o = (f & 0x1c00) >> 10;
            f &= 0x3ff;

            f += page.detune;
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
                f = f - ftbl[0] * 2;// + ftbl[0];
            }
            f = Common.CheckRange(f, 0, 0x3ff);
            //Console.WriteLine("o:{0} f:{1}",o,f);
            OutFmSetFnum(page, o, f);
        }

        public virtual int GetFmFNum(int[] ftbl, int octave, char noteCmd, int shift, int pitchShift)
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

            int f = ftbl[n];
            f += pitchShift;

            return (f & 0x3ff) + ((o & 0x7) << 10);
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

            if (page.varpeggioMode)
            {
                vol += page.varpDelta;
            }

            //if (pw.ppg[pw.cpgNum].beforeVolume != vol)
            //{
            //if (parent.instOPL.ContainsKey(pw.ppg[pw.cpgNum].instrument))
            //{
            page.volume = vol;
            //outYM2413SetInstVol(pw, pw.ppg[pw.cpgNum].envInstrument, vol);
            //pw.ppg[pw.cpgNum].beforeVolume = vol;
            //}
            //}
        }

        public virtual void SetFmTL(partPage page, MML mml)
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

            if (page.spg.beforeTlDelta1 != tl1 )
            {
                if (parent.instOPL.ContainsKey(page.instrument))
                {
                    OutFmSetTL(page, mml, tl1, page.instrument);
                }
                page.spg.beforeTlDelta1 = tl1;
            }
        }

        public void OutFmSetTL(partPage page, MML mml, int tl1, int n)
        {
            if (!parent.instOPL.ContainsKey(n))
            {
                msgBox.setWrnMsg(string.Format(msg.get("E11000"), n), mml.line.Lp);
                return;
            }

            int alg = parent.instOPL[n].Item2[25];
            int opeTL = -1;
            if (alg == 0)
            {
                opeTL = parent.instOPL[n].Item2[12 * 0 + 6];
                opeTL = opeTL - tl1;
                opeTL = Common.CheckRange(opeTL, 0, 63);
            }

            partPage vpg = page;
            MML vmml = new MML();
            vmml.args = new List<object>();
            vmml.args.Add(tl1);
            vmml.type = enmMMLType.unknown;//.TotalLevel;
            if (mml != null)
                vmml.line = mml.line;
            if (opeTL != -1) OutFmSetTl(vmml, vpg, 0, opeTL);
        }

        public void OutFmSetTl(MML mml, partPage page, int ope, int tl)
        {

            byte[] port = getPortFromCh(page.ch);
            SOutData(page,
                mml,
                port,
                (byte)(0x40 + ChnToBaseReg(page.ch) + 0),
                (byte)(
                        ((parent.instOPL[page.instrument].Item2[12 * ope + 5] & 0x3) << 6)  //KL(M)
                        | Common.CheckRange( tl, 0, 63) //TL
                    )
                );
        }

        public override void GetFNumAtoB(partPage page, MML mml
            , out int a, int aOctaveNow, char aCmd, int aShift, int aPitchShift
            , out int b, int bOctaveNow, char bCmd, int bShift, int bPitchShift
            , int dir)
        {
            a = GetFNum(page, mml, aOctaveNow, aCmd, aShift, aPitchShift);
            b = GetFNum(page, mml, bOctaveNow, bCmd, bShift, bPitchShift);

            int oa = (a & 0x1c00) >> 10;
            int ob = (b & 0x1c00) >> 10;
            if (oa != ob)
            {
                if (((a - aPitchShift) & 0x3ff) == FNumTbl[0][0])
                {
                    oa += Math.Sign(ob - oa);
                    a = (a & 0x3ff) * 2 + (oa << 10);
                }
                else if (((b - bPitchShift) & 0x3ff) == FNumTbl[0][0])
                {
                    ob += Math.Sign(oa - ob);
                    b = (b & 0x3ff) * ((dir > 0) ? 2 : 1) + (ob << 10);
                }
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
        }

        public override void SetKeyOn(partPage page, MML mml)
        {
            page.keyOn = true;
            //SetDummyData(page, mml);
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
                if (re) n = page.instrument + n;
                n = Common.CheckRange(n, 1, 15);
                if (page.envInstrument != n)
                {
                    page.envInstrument = n;
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

            outOPLSetInstrument(page, mml, n, modeBeforeSend); //音色のセット
            page.envInstrument = 0;

        }

        public override void CmdMode(partPage page, MML mml)
        {
            //Console.WriteLine("CmdMode()");

            int n = (int)mml.args[0];

            if ((page.ch > 5 && page.ch < 9) || page.Type == enmChannelType.RHYTHM)
            {
                if (n == 0) page.chip.rhythmStatus &= 0xdf;
                else page.chip.rhythmStatus |= 0x20;

                return;
            }

            if (page.Type == enmChannelType.FMOPL)
            {
                if (page.ch >= 0 && page.ch < 6)
                {
                    int tch = page.ch % 3;

                    if (n == 0)
                    {
                        page.chip.connectionSel &= (~(1 << tch)) & 0x3f;

                        tch += (page.ch > 8 ? 6 : 0);
                        page.chip.lstPartWork[tch].cpg.isOp4Mode = false;
                        page.chip.lstPartWork[tch + 3].cpg.isOp4Mode = false;
                    }
                    else
                    {
                        page.chip.connectionSel |= (1 << tch);

                        tch += (page.ch > 8 ? 6 : 0);
                        page.chip.lstPartWork[tch].cpg.isOp4Mode = true;
                        page.chip.lstPartWork[tch + 3].cpg.isOp4Mode = true;
                    }

                }
            }
        }

        //public override void CmdPan(partPage page, MML mml)
        //{
        //    throw new NotImplementedException();
        //}

        public override void CmdLoopExtProc(partPage page, MML mml)
        {
        }

        public override void CmdY(partPage page, MML mml)
        {
            if (mml.args[0] is string) return;

            byte adr = (byte)(int)mml.args[0];
            byte dat = (byte)(int)mml.args[1];
            int p = 0;

            SOutData(page, mml, port[p], adr, dat);
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
            page.detune = Common.CheckRange(page.detune, -0x3ff, 0x3ff);

            SetDummyData(page, mml);
        }

        public override void MultiChannelCommand(MML mml)
        {

            foreach (partWork pw in lstPartWork)
            {
                foreach (partPage page in pw.pg)
                {
                    if (page.Type == enmChannelType.FMOPL)
                    {
                        if (page.beforeVolume != page.volume && parent.instOPL.ContainsKey(page.instrument))
                        {
                            page.beforeVolume = page.volume;


                            int cnt = parent.instOPL[page.instrument].Item2[25];
                            if (cnt != 0)
                            {
                                //OP1
                                SOutData(page,
                                    mml,
                                    port[0],
                                    (byte)(0x40 + ChnToBaseReg(page.ch) + 0),
                                    (byte)(
                                            ((parent.instOPL[page.instrument].Item2[12 * 0 + 5] & 0x3) << 6)  //KL(M)
                                            | Common.CheckRange(((parent.instOPL[page.instrument].Item2[12 * 0 + 6] & 0x3f) + (63 - (page.volume & 0x3f))), 0, 63) //TL
                                        )
                                    );
                            }
                            //OP2
                            SOutData(page,
                                mml,
                                port[0],
                                (byte)(0x40 + ChnToBaseReg(page.ch) + 3),
                                (byte)(
                                        ((parent.instOPL[page.instrument].Item2[12 * 1 + 5] & 0x3) << 6)  //KL(M)
                                        | Common.CheckRange(((parent.instOPL[page.instrument].Item2[12 * 1 + 6] & 0x3f) + (63 - (page.volume & 0x3f))), 0, 63) //TL
                                    )
                                );
                        }

                        SetFmTL(page, mml);

                        if (page.keyOff)
                        {
                            page.keyOff = false;
                            SOutData(page, mml, getPortFromCh(page.ch)
                                , (byte)(0xB0 + page.ch)
                                , (byte)(
                                    ((page.freq >> 8) & 0x1f)
                                  )
                                );
                            page.beforeFNum = page.freq;
                        }

                        if (page.beforeFNum != (page.freq | (page.keyOn ? 0x4000 : 0x0000)))
                        {
                            page.beforeFNum = page.freq | (page.keyOn ? 0x4000 : 0x0000);
                            //Console.WriteLine("CalcPitch {0} {1}_{2}", pw.ppg[pw.cpgNum].freq, pw.ppg[pw.cpgNum].freq >> 8 & 0x1F, pw.ppg[pw.cpgNum].freq & 0xFF);
                            SOutData(page, mml, getPortFromCh(page.ch), (byte)(0xa0 + page.ch), (byte)page.freq);
                            SOutData(page, mml, getPortFromCh(page.ch)
                                , (byte)(0xB0 + page.ch)
                                , (byte)(
                                    ((page.freq >> 8) & 0x1f)
                                    | (page.keyOn ? 0x20 : 0x00)
                                  )
                                );
                        }
                    }

                    else if (page.Type == enmChannelType.RHYTHM)
                    {
                        if (page.beforeVolume != page.volume && parent.instOPL.ContainsKey(page.instrument))
                        {
                            page.beforeVolume = page.volume;

                            if (page.ch == 9)
                            {
                                int vch = 6;

                                int cnt = parent.instOPL[page.instrument].Item2[25];
                                if (cnt != 0)
                                {
                                    //OP1
                                    SOutData(page,
                                        mml,
                                        port[0],
                                        (byte)(0x40 + ChnToBaseReg(vch) + 0),
                                        (byte)(
                                                ((parent.instOPL[page.instrument].Item2[12 * 0 + 5] & 0x3) << 6)  //KL(M)
                                                | Common.CheckRange(((parent.instOPL[page.instrument].Item2[12 * 0 + 6] & 0x3f) + (63 - (page.volume & 0x3f))), 0, 63) //TL
                                            )
                                        );
                                }
                                //OP2
                                SOutData(page,
                                    mml,
                                    port[0],
                                    (byte)(0x40 + ChnToBaseReg(vch) + 3),
                                    (byte)(
                                            ((parent.instOPL[page.instrument].Item2[12 * 1 + 5] & 0x3) << 6)  //KL(M)
                                            | Common.CheckRange(((parent.instOPL[page.instrument].Item2[12 * 1 + 6] & 0x3f) + (63 - (page.volume & 0x3f))), 0, 63) //TL
                                        )
                                    );
                            }
                            else if (page.ch == 10)
                            {
                                int vch = 7;
                                //OP2
                                SOutData(page,
                                    mml,
                                    port[0],
                                    (byte)(0x40 + ChnToBaseReg(vch) + 3),
                                    (byte)(
                                            ((parent.instOPL[page.instrument].Item2[12 * 1 + 5] & 0x3) << 6)  //KL(M)
                                            | Common.CheckRange(((parent.instOPL[page.instrument].Item2[12 * 1 + 6] & 0x3f) + (63 - (page.volume & 0x3f))), 0, 63) //TL
                                        )
                                    );
                            }
                            else if (page.ch == 11)
                            {
                                int vch = 8;
                                //int cnt = parent.instOPL[pw.ppg[pw.cpgNum].instrument][25];
                                //if (cnt != 0)
                                {
                                    //OP1
                                    SOutData(page,
                                        mml,
                                        port[0],
                                        (byte)(0x40 + ChnToBaseReg(vch) + 0),
                                        (byte)(
                                                ((parent.instOPL[page.instrument].Item2[12 * 0 + 5] & 0x3) << 6)  //KL(M)
                                                | Common.CheckRange(((parent.instOPL[page.instrument].Item2[12 * 0 + 6] & 0x3f) + (63 - (page.volume & 0x3f))), 0, 63) //TL
                                            )
                                        );
                                }
                            }
                            else if (page.ch == 12)
                            {
                                int vch = 8;
                                //OP2
                                SOutData(page,
                                    mml,
                                    port[0],
                                    (byte)(0x40 + ChnToBaseReg(vch) + 3),
                                    (byte)(
                                            ((parent.instOPL[page.instrument].Item2[12 * 1 + 5] & 0x3) << 6)  //KL(M)
                                            | Common.CheckRange(((parent.instOPL[page.instrument].Item2[12 * 1 + 6] & 0x3f) + (63 - (page.volume & 0x3f))), 0, 63) //TL
                                        )
                                    );
                            }
                            else if (page.ch == 13)
                            {
                                int vch = 7;
                                //int cnt = parent.instOPL[pw.ppg[pw.cpgNum].instrument][25];
                                //if (cnt != 0)
                                {
                                    //OP1
                                    SOutData(page,
                                        mml,
                                        port[0],
                                        (byte)(0x40 + ChnToBaseReg(vch) + 0),
                                        (byte)(
                                                ((parent.instOPL[page.instrument].Item2[12 * 0 + 5] & 0x3) << 6)  //KL(M)
                                                | Common.CheckRange(((parent.instOPL[page.instrument].Item2[12 * 0 + 6] & 0x3f) + (63 - (page.volume & 0x3f))), 0, 63) //TL
                                            )
                                        );
                                }
                            }
                        }

                        if (page.beforeFNum != (page.freq | (page.keyOn ? 0x4000 : 0x0000)))
                        {
                            page.beforeFNum = page.freq | (page.keyOn ? 0x4000 : 0x0000);
                            //Console.WriteLine("CalcPitch {0} {1}_{2}", pw.ppg[pw.cpgNum].freq, pw.ppg[pw.cpgNum].freq >> 8 & 0x1F, pw.ppg[pw.cpgNum].freq & 0xFF);

                            int vch = 0;
                            if (page.ch == 9)//bd
                            {
                                vch = 6;
                            }
                            else if (page.ch == 10)//sd
                            {
                                vch = 7;
                            }
                            else if (page.ch == 11)//tom
                            {
                                vch = 8;
                            }
                            else if (page.ch == 12)//CYM
                            {
                                vch = 8;
                            }
                            else if (page.ch == 13)//HH
                            {
                                vch = 7;
                            }

                            SOutData(page, mml, getPortFromCh(vch), (byte)(0xa0 + vch), (byte)page.freq);
                            SOutData(page, mml, getPortFromCh(vch)
                                , (byte)(0xB0 + vch)
                                , (byte)(
                                    ((page.freq >> 8) & 0x1f)
                                  //| (pw.ppg[pw.cpgNum].keyOn ? 0x20 : 0x00)
                                  )
                                );
                        }

                    }

                    else if(page.Type==enmChannelType.ADPCM)//Y8950専用
                    {
                        //ボリュームチェック
                        int vol = page.volume;
                        if (page.envelopeMode)
                        {
                            vol = 0;
                            if (page.envIndex != -1)
                            {
                                vol = page.volume - (0xff - page.envVolume);
                            }
                        }
                        vol = Common.CheckRange(vol, 0, 0xff);//256段階
                        if (page.spg.beforeVolume != vol)
                        {
                            SOutData(page, mml, port[0], 0x12, (byte)vol);
                            page.spg.beforeVolume = vol;
                        }

                        //キーオフチェック
                        if (page.keyOff)
                        {
                            page.keyOff = false;
                            SOutData(page, mml, port[0]
                                , 0x07
                                , 0x20
                                );
                            page.beforeFNum = page.freq;
                        }

                        //キーオンチェック
                        if (page.keyOn)
                        {
                            page.keyOn = false;
                            SOutData(page, mml, port[0]
                                , 0x07
                                , (byte)(0xa0 | ((page.pcmLoopAddress & 2) != 0 ? 0x10 : 0x00))
                                );
                        }

                        //freqチェック
                        if (page.spg.freq != page.freq)
                        {
                            page.spg.freq = page.freq;
                            byte data = 0;
                            data = (byte)(page.freq & 0xff);
                            SOutData(page, mml, port[0], 0x10, data);//DELTA-N(L)
                            data = (byte)((page.freq & 0xff00) >> 8);
                            SOutData(page, mml, port[0], 0x11, data);//DELTA-N(H)
                        }

                    }
                }
            }


            rhythmStatus &= 0xe0;
            rhythmStatus |= (byte)(
                (lstPartWork[9].cpg.keyOn ? 0x10 : 0x00)
                | (lstPartWork[10].cpg.keyOn ? 0x08 : 0x00)
                | (lstPartWork[11].cpg.keyOn ? 0x04 : 0x00)
                | (lstPartWork[12].cpg.keyOn ? 0x02 : 0x00)
                | (lstPartWork[13].cpg.keyOn ? 0x01 : 0x00)
                );

            if (beforeRhythmStatus != rhythmStatus)
            {
                beforeRhythmStatus = rhythmStatus;
                SOutData(lstPartWork[9].cpg, mml, port[0], 0xbd, rhythmStatus);
            }

        }



    }
}
