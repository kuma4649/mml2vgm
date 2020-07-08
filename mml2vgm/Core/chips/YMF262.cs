using System.Collections.Generic;

namespace Core
{
    /* MEMO From Chroma
     * VGM 0x5E -> Port0
     * VGM 0x5F -> Port1
     * OPL3にするには0x5F 05 01と送ってOPL3のNEWフラグをセットする必要がある。
     * 
     * 
    */

    /* 2op
        '@ L No
        '@ AR DR SL RR KSL TL MT AM VIB EGT KSR WS
        '@ AR DR SL RR KSL TL MT AM VIB EGT KSR WS
        '@ CNT FB

        
    */

    /*
        4op
        1,2,3 -> 4,5,6
        10,11,12 -> 13,14,15

        Rythm 7,8,9
        2op ONLY: 16,17,18
    */

    public class YMF262 : ClsOPL
    {
        //protected int[][] _FNumTbl = new int[1][] {
        //    //new int[13]
        //    new int[] {
        //    // OPL3(FM) : Fnum = ftone*(2**19)/(M/288)/(2**B-1)       ftone:Hz M:MasterClock B:Block
        //    //   c    c+     d    d+     e     f    f+     g    g+     a    a+     b    >c
        //     0x158,0x16a,0x180,0x198,0x1b0,0x1ca,0x1e4,0x202,0x220,0x240,0x262,0x286,0x2b0
        //    }
        //};


        public YMF262(ClsVgm parent, int chipID, string initialPartName, string stPath, int chipNumber) : base(parent, chipID, initialPartName, stPath, chipNumber)
        {

            _Name = "YMF262";
            _ShortName = "OPL3";
            _ChMax = 23;
            // OPL2 mode = 9*2 2op
            // OPL3 mode (all 2op) = 18 2op channel
            // OPL3 Rhythm Mode = 15*4op + 3 rhythm channel
            // OPL3 4op = 1-4, 2-5, 3-6, 10-13, 11-14, 12-15,(6*4op) 6*2op
            // OPL3 All mode = (4op mode) + 7,8,9(RYM) 3 2op channel
            _canUsePcm = false;

            Frequency = 14318180;
            port = new byte[][] { new byte[] { (byte)(chipNumber != 0 ? 0xae : 0x5e) }, new byte[] { (byte)(chipNumber != 0 ? 0xaf : 0x5f) } };

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
            //int i = 0;
            /*
            foreach (ClsChannel ch in Ch)
            {
                ch.Type = i < 18 ? enmChannelType.FMOPL : enmChannelType.RHYTHM;
                ch.chipNumber = chipID == 1;
                i++;
            }*/

        }

        public override void InitPart(partWork pw)
        {
            foreach (partPage page in pw.pg)
            {
                page.beforeVolume = -1;
                page.volume = 60;
                page.MaxVolume = 63;
                page.beforeEnvInstrument = 0;
                page.envInstrument = 0;
                page.port = port;
                page.mixer = 0;
                page.noise = 0;
                page.pan = 3;
                page.Type = enmChannelType.FMOPL;
                page.isOp4Mode = false;
                if (page.ch > 17) page.Type = enmChannelType.RHYTHM;
            }
        }

        public override void InitChip()
        {

            if (!use) return;

            parent.OutData((MML)null, port[1], 0x05, 0x01);

            //FM Off
            outAllKeyOff(null, lstPartWork[0]);

            rhythmStatus = 0x00;
            beforeRhythmStatus = 0xff;
            connectionSel = 0;
            beforeConnectionSel = -1;

            /*
             * if (ChipID != 0 && parent.info.format != enmFormat.ZGM)
            {
                parent.dat[0x13] = new outDatum(enmMMLType.unknown, null, null, (byte)(parent.dat[0x13].val | 0x40));//use Secondary(YM2413 OPLL)
            }
            */

        }

        public override void outAllKeyOff(MML mml, partWork pw)
        {
            //Rhythm Off
            parent.OutData(mml, port[0], 0xBD, 0);
            // Probably wise to reset Rhythm mode.
            for (byte adr = 0; adr <= 8; adr++)
            {
                //Ch Off
                parent.OutData(mml, port[0], (byte)(0xB0 + adr), 0);
                parent.OutData(mml, port[1], (byte)(0xB0 + adr), 0);
            }
        }

        protected override void SetInstToRhythmChannel(partPage page, MML mml, int n, int modeBeforeSend)
        {
            if (rhythmStatus == 0) return;

            if (page.ch == 18)//BD
            {
                int vch = 6;
                SetInst2Operator(page, mml, n, modeBeforeSend, vch);
            }
            else if (page.ch == 19)//SD
            {
                int opeNum = 16;
                SetInst1Operator(page, mml, n, modeBeforeSend, opeNum);
            }
            else if (page.ch == 20)//TOM
            {
                int opeNum = 14;
                SetInst1Operator(page, mml, n, modeBeforeSend, opeNum);
            }
            else if (page.ch == 21)//CYM
            {
                int opeNum = 17;
                SetInst1Operator(page, mml, n, modeBeforeSend, opeNum);
            }
            else if (page.ch == 22)//HH
            {
                int opeNum = 13;
                SetInst1Operator(page, mml, n, modeBeforeSend, opeNum);
            }
        }

        protected override void SetInst1Operator(partPage page, MML mml, int n, int modeBeforeSend, int opeNum)
        {
            byte[] inst = parent.instFM[n];
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

            SetInstAtChannelPanFbCnt(page, mml, (opeNum % 6) % 3 + (opeNum / 6) * 3, page.pan, inst[26], inst[25]);

            page.beforeVolume = -1;
        }

        protected override void SetInst2Operator(partPage page, MML mml, int n, int modeBeforeSend, int vch)
        {
            byte[] inst = parent.instFM[n];
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

            SetInstAtChannelPanFbCnt(page, mml, vch, page.pan, inst[26], inst[25]);

            page.beforeVolume = -1;
        }

        protected override void SetInst4Operator(partPage page, MML mml, int n, int modeBeforeSend, int vch)
        {
            if (!page.isOp4Mode)
            {
                msgBox.setErrMsg(string.Format(msg.get("E26000"), n), mml.line.Lp);
                return;
            }

            byte[] inst = parent.instFM[n];
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


            SetInstAtChannelPanFbCnt(page, mml, vch, page.pan, inst[51], cnt1);
            SetInstAtChannelPanFbCnt(page, mml, vch + 3, page.pan, inst[51], cnt2);

            page.beforeVolume = -1;
        }

        protected override void SetInstAtOneOpeWithoutKslTl(partPage page, MML mml, int opeNum,
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
            SOutData(page, mml, port, (byte)(0xe0 + adr), (byte)(ws & 0x7));
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
                if ((page.ch >= 0 && page.ch < 6) || (page.ch >= 9 && page.ch < 15))
                {
                    int tch = (page.ch % 9) % 3 + (page.ch / 9) * 3;

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

        public override void CmdPan(partPage page, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 0, 3);
            page.pan = ((n & 1) << 1) | ((n & 2) >> 1);//LR反転
            int vch = page.ch;

            if (page.Type == enmChannelType.RHYTHM)
            {
                if (page.ch == 18) vch = 6;
                else if (page.ch == 19) vch = 7;
                else if (page.ch == 20) vch = 8;
                else if (page.ch == 21) vch = 8;
                else if (page.ch == 22) vch = 7;
            }

            byte[] port = getPortFromCh(vch);

            byte PanFbCnt = 0;
            if (page.instrument != -1)
            {
                PanFbCnt = (byte)(
                    (parent.instFM[page.instrument][26] & 0x07) << 1
                | parent.instFM[page.instrument][25] & 0x01
                );
            }

            SOutData(page, mml, port, (byte)(vch % 9 + 0xC0), (byte)((
                PanFbCnt
                | (page.pan * 0x10) // PAN
                )));

            SetDummyData(page, mml);
        }

        public override void CmdY(partPage page, MML mml)
        {
            if (mml.args[0] is string) return;

            if (mml.args.Count == 2)
            {
                byte adr = (byte)(int)mml.args[0];
                byte dat = (byte)(int)mml.args[1];
                int p = 0;
                if (page.ch < 9)//FM1-9
                    p = 0;
                else if (page.ch < 18)//FM10-18
                    p = 1;
                else
                    p = 0;

                SOutData(page, mml, port[p], adr, dat);
            }
            else
            {
                byte prt = (byte)(int)mml.args[0];
                byte adr = (byte)(int)mml.args[1];
                byte dat = (byte)(int)mml.args[2];

                SOutData(page, mml, port[prt & 1], adr, dat);

            }
        }

        public override void MultiChannelCommand(MML mml)
        {

            if (beforeConnectionSel != connectionSel)
            {
                beforeConnectionSel = connectionSel;
                parent.OutData(mml, port[1], 0x04, (byte)connectionSel);
            }

            foreach (partWork pw in lstPartWork)
            {
                foreach (partPage page in pw.pg)
                {
                    if (page.Type == enmChannelType.FMOPL)
                    {
                        if (!page.isOp4Mode)
                        {
                            if (page.beforeVolume != page.volume && parent.instFM.ContainsKey(page.instrument))
                            {
                                page.beforeVolume = page.volume;


                                int cnt = parent.instFM[page.instrument][25];
                                if (cnt != 0)
                                {
                                    //OP1
                                    SOutData(page,
                                        mml,
                                        port[page.ch / 9],
                                        (byte)(0x40 + ChnToBaseReg(page.ch) + 0),
                                        (byte)(
                                                ((parent.instFM[page.instrument][12 * 0 + 5] & 0x3) << 6)  //KL(M)
                                                | Common.CheckRange(((parent.instFM[page.instrument][12 * 0 + 6] & 0x3f) + (63 - (page.volume & 0x3f))), 0, 63) //TL
                                            )
                                        );
                                }
                                //OP2
                                SOutData(page,
                                    mml,
                                    port[page.ch / 9],
                                    (byte)(0x40 + ChnToBaseReg(page.ch) + 3),
                                    (byte)(
                                            ((parent.instFM[page.instrument][12 * 1 + 5] & 0x3) << 6)  //KL(M)
                                            | Common.CheckRange(((parent.instFM[page.instrument][12 * 1 + 6] & 0x3f) + (63 - (page.volume & 0x3f))), 0, 63) //TL
                                        )
                                    );
                            }

                        }
                        else
                        {
                            if ((page.ch >= 3 && page.ch <= 5) || (page.ch >= 12 && page.ch <= 14)) continue;

                            if (page.beforeVolume != page.volume && parent.instFM.ContainsKey(page.instrument))
                            {
                                page.beforeVolume = page.volume;

                                int cnt1 = parent.instFM[page.instrument][49];
                                int cnt2 = parent.instFM[page.instrument][50];
                                bool[] op = new bool[] { false, false, false, false };

                                if (cnt1 == 0 && cnt2 == 0) { op[3] = true; }
                                else if (cnt1 == 0 && cnt2 == 1) { op[1] = true; op[3] = true; }
                                else if (cnt1 == 1 && cnt2 == 0) { op[0] = true; op[3] = true; }
                                else if (cnt1 == 1 && cnt2 == 1) { op[0] = true; op[2] = true; op[3] = true; }

                                for (int i = 0; i < 4; i++)
                                {
                                    if (!op[i]) continue;
                                    SOutData(page, mml, port[page.ch / 9], (byte)(0x40 + ChnToBaseReg(page.ch) + i * 3 + (i > 1 ? 2 : 0)),
                                        (byte)(
                                            ((parent.instFM[page.instrument][12 * i + 5] & 0x3) << 6)  //KL(M)
                                            | Common.CheckRange(((parent.instFM[page.instrument][12 * i + 6] & 0x3f) + (63 - (page.volume & 0x3f))), 0, 63) //TL
                                        )
                                    );
                                }
                            }
                        }

                        if (page.keyOff)
                        {
                            page.keyOff = false;
                            SOutData(page, mml, getPortFromCh(page.ch)
                                , (byte)(0xB0 + page.ch % 9)
                                , (byte)(
                                    ((page.freq >> 8) & 0x1f)
                                  )
                                );
                        }

                        if (page.beforeFNum != (page.freq | (page.keyOn ? 0x4000 : 0x0000)))
                        {
                            page.beforeFNum = page.freq | (page.keyOn ? 0x4000 : 0x0000);
                            //Console.WriteLine("CalcPitch {0} {1}_{2}", pw.ppg[pw.cpgNum].freq, pw.ppg[pw.cpgNum].freq >> 8 & 0x1F, pw.ppg[pw.cpgNum].freq & 0xFF);
                            SOutData(page, mml, getPortFromCh(page.ch), (byte)(0xa0 + page.ch % 9), (byte)page.freq);
                            SOutData(page, mml, getPortFromCh(page.ch)
                                , (byte)(0xB0 + page.ch % 9)
                                , (byte)(
                                    ((page.freq >> 8) & 0x1f)
                                    | (page.keyOn ? 0x20 : 0x00)
                                  )
                                );
                        }
                    }

                    else if (page.Type == enmChannelType.RHYTHM)
                    {
                        if (page.beforeVolume != page.volume && parent.instFM.ContainsKey(page.instrument))
                        {
                            page.beforeVolume = page.volume;

                            if (page.ch == 18)
                            {
                                int vch = 6;

                                int cnt = parent.instFM[page.instrument][25];
                                if (cnt != 0)
                                {
                                    //OP1
                                    SOutData(page,
                                        mml,
                                        port[vch / 9],
                                        (byte)(0x40 + ChnToBaseReg(vch) + 0),
                                        (byte)(
                                                ((parent.instFM[page.instrument][12 * 0 + 5] & 0x3) << 6)  //KL(M)
                                                | Common.CheckRange(((parent.instFM[page.instrument][12 * 0 + 6] & 0x3f) + (63 - (page.volume & 0x3f))), 0, 63) //TL
                                            )
                                        );
                                }
                                //OP2
                                SOutData(page,
                                    mml,
                                    port[vch / 9],
                                    (byte)(0x40 + ChnToBaseReg(vch) + 3),
                                    (byte)(
                                            ((parent.instFM[page.instrument][12 * 1 + 5] & 0x3) << 6)  //KL(M)
                                            | Common.CheckRange(((parent.instFM[page.instrument][12 * 1 + 6] & 0x3f) + (63 - (page.volume & 0x3f))), 0, 63) //TL
                                        )
                                    );
                            }
                            else if (page.ch == 19)
                            {
                                int vch = 7;
                                //OP2
                                SOutData(page,
                                    mml,
                                    port[vch / 9],
                                    (byte)(0x40 + ChnToBaseReg(vch) + 3),
                                    (byte)(
                                            ((parent.instFM[page.instrument][12 * 1 + 5] & 0x3) << 6)  //KL(M)
                                            | Common.CheckRange(((parent.instFM[page.instrument][12 * 1 + 6] & 0x3f) + (63 - (page.volume & 0x3f))), 0, 63) //TL
                                        )
                                    );
                            }
                            else if (page.ch == 20)
                            {
                                int vch = 8;
                                //int cnt = parent.instFM[pw.ppg[pw.cpgNum].instrument][25];
                                //if (cnt != 0)
                                {
                                    //OP1
                                    SOutData(page,
                                        mml,
                                        port[vch / 9],
                                        (byte)(0x40 + ChnToBaseReg(vch) + 0),
                                        (byte)(
                                                ((parent.instFM[page.instrument][12 * 0 + 5] & 0x3) << 6)  //KL(M)
                                                | Common.CheckRange(((parent.instFM[page.instrument][12 * 0 + 6] & 0x3f) + (63 - (page.volume & 0x3f))), 0, 63) //TL
                                            )
                                        );
                                }
                            }
                            else if (page.ch == 21)
                            {
                                int vch = 8;
                                //OP2
                                SOutData(page,
                                    mml,
                                    port[vch / 9],
                                    (byte)(0x40 + ChnToBaseReg(vch) + 3),
                                    (byte)(
                                            ((parent.instFM[page.instrument][12 * 1 + 5] & 0x3) << 6)  //KL(M)
                                            | Common.CheckRange(((parent.instFM[page.instrument][12 * 1 + 6] & 0x3f) + (63 - (page.volume & 0x3f))), 0, 63) //TL
                                        )
                                    );
                            }
                            else if (page.ch == 22)
                            {
                                int vch = 7;
                                //int cnt = parent.instFM[pw.ppg[pw.cpgNum].instrument][25];
                                //if (cnt != 0)
                                {
                                    //OP1
                                    SOutData(page,
                                        mml,
                                        port[vch / 9],
                                        (byte)(0x40 + ChnToBaseReg(vch) + 0),
                                        (byte)(
                                                ((parent.instFM[page.instrument][12 * 0 + 5] & 0x3) << 6)  //KL(M)
                                                | Common.CheckRange(((parent.instFM[page.instrument][12 * 0 + 6] & 0x3f) + (63 - (page.volume & 0x3f))), 0, 63) //TL
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
                            if (page.ch == 18)//bd
                            {
                                vch = 6;
                            }
                            else if (page.ch == 19)//sd
                            {
                                vch = 7;
                            }
                            else if (page.ch == 20)//tom
                            {
                                vch = 8;
                            }
                            else if (page.ch == 21)//CYM
                            {
                                vch = 8;
                            }
                            else if (page.ch == 22)//HH
                            {
                                vch = 7;
                            }

                            SOutData(page, mml, getPortFromCh(vch), (byte)(0xa0 + vch % 9), (byte)page.freq);
                            SOutData(page, mml, getPortFromCh(vch)
                                , (byte)(0xB0 + vch % 9)
                                , (byte)(
                                    ((page.freq >> 8) & 0x1f)
                                  //| (pw.ppg[pw.cpgNum].keyOn ? 0x20 : 0x00)
                                  )
                                );
                        }

                    }
                }
            }


            rhythmStatus &= 0xe0;
            rhythmStatus |= (byte)(
                (lstPartWork[18].cpg.keyOn ? 0x10 : 0x00)
                | (lstPartWork[19].cpg.keyOn ? 0x08 : 0x00)
                | (lstPartWork[20].cpg.keyOn ? 0x04 : 0x00)
                | (lstPartWork[21].cpg.keyOn ? 0x02 : 0x00)
                | (lstPartWork[22].cpg.keyOn ? 0x01 : 0x00)
                );

            if (beforeRhythmStatus != rhythmStatus)
            {
                beforeRhythmStatus = rhythmStatus;
                SOutData(lstPartWork[18].cpg, mml, port[0], 0xbd, rhythmStatus);
            }

        }


    }
}