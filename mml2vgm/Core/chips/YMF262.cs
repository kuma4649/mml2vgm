using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using musicDriverInterface;

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

    public class YMF262 : ClsChip
    {
        protected int[][] _FNumTbl = new int[1][] {
            //new int[13]
            new int[] {
            // OPL3(FM) : Fnum = ftone*(2**19)/(M/288)/(2**B-1)       ftone:Hz M:MasterClock B:Block
            //   c    c+     d    d+     e     f    f+     g    g+     a    a+     b    >c
             0x158,0x16a,0x180,0x198,0x1b0,0x1ca,0x1e4,0x202,0x220,0x240,0x262,0x286,0x2b0
            }
        };


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
            int i = 0;
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
            pw.beforeVolume = -1;
            pw.volume = 60;
            pw.MaxVolume = 63;
            pw.beforeEnvInstrument = 0;
            pw.envInstrument = 0;
            pw.port = port;
            pw.mixer = 0;
            pw.noise = 0;
            pw.pan.val = 3;
            pw.Type = enmChannelType.FMOPL;
            pw.isOp4Mode = false;
            if (pw.ch > 17) pw.Type = enmChannelType.RHYTHM;
        }

        public override void InitChip()
        {

            if (!use) return;

            parent.OutData((MML)null, port[1], 0x05, 0x01);

            //FM Off
            outYMF262AllKeyOff(null, lstPartWork[0]);

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


        public void outYMF262AllKeyOff(MML mml, partWork pw)
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

        public byte ChnToBaseReg(int chn)
        {
            //Console.Write("Enter ChnToBaseReg: Ch{0}", chn + 1);
            chn %= 9; // A1=LでもA1=Hでもいっしょ。
            byte carrier = (byte)((chn / 3) * 8 + (chn % 3));
            return carrier;
        }

        public byte[] getPortFromCh(int ch)
        {

            if (ch >= 9)
            {
                //Console.WriteLine("getPortFromCh port1");
                return port[1];
            }
            //Console.WriteLine("getPortFromCh port0");
            return port[0];
        }

        public void outYMF262SetInstrument(partWork pw, MML mml, int n, int modeBeforeSend)
        {
            pw.instrument = n;
            // TODO: 7,8,9chはRYMなので変換必須。
            // TODO: 4opモード時の変換。

            if (!parent.instFM.ContainsKey(n))
            {
                msgBox.setWrnMsg(string.Format(msg.get("E17000"), n), mml.line.Lp);
                return;
            }

            if (pw.Type == enmChannelType.RHYTHM)
            {
                SetInstToRhythmChannel(pw, mml, n, modeBeforeSend);
                return;
            }

            if (parent.instFM[n].Length == Const.OPL_OP4_INSTRUMENT_SIZE)
            {
                SetInst4Operator(pw, mml, n, modeBeforeSend, pw.ch);
                return;
            }

            SetInst2Operator(pw, mml, n, modeBeforeSend, pw.ch);
        }

        private void SetInstToRhythmChannel(partWork pw, MML mml, int n, int modeBeforeSend)
        {
            if (rhythmStatus == 0) return;

            if (pw.ch == 18)//BD
            {
                int vch = 6;
                SetInst2Operator(pw, mml, n, modeBeforeSend, vch);
            }
            else if (pw.ch == 19)//SD
            {
                int opeNum = 16;
                SetInst1Operator(pw, mml, n, modeBeforeSend, opeNum);
            }
            else if (pw.ch == 20)//TOM
            {
                int opeNum = 14;
                SetInst1Operator(pw, mml, n, modeBeforeSend, opeNum);
            }
            else if (pw.ch == 21)//CYM
            {
                int opeNum = 17;
                SetInst1Operator(pw, mml, n, modeBeforeSend, opeNum);
            }
            else if (pw.ch == 22)//HH
            {
                int opeNum = 13;
                SetInst1Operator(pw, mml, n, modeBeforeSend, opeNum);
            }
        }

        private void SetInst1Operator(partWork pw, MML mml, int n, int modeBeforeSend, int opeNum)
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
                    parent.OutData(mml, port, (byte)(targetBaseReg + ope * 3 + 0x80)
                        , ((0 & 0xf) << 4) | (15 & 0xf));//SL RR
                    break;
                case 2: // A)ll
                    SetInstAtOneOpeWithoutKslTl(mml, opeNum
                        , 15, 15, 0, 15, 0, 0, 0, 0, 0, 0);
                    parent.OutData(mml, port, (byte)(targetBaseReg + ope * 3 + 0x40)
                        , ((0 & 0x3) << 6) | 0x3f);  //KL(M) TL
                    break;
            }

            SetInstAtOneOpeWithoutKslTl(mml, opeNum,
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
            if (cnt == 0 || pw.Type == enmChannelType.RHYTHM)
            {
                if (ope == 0)
                {
                    //OP1
                    parent.OutData(mml, port, (byte)(0x40 + targetBaseReg + 0)
                        , (byte)(((inst[12 * 0 + 5] & 0x3) << 6) | (inst[12 * 0 + 6] & 0x3f))); //KL(M) TL
                }
            }

            SetInstAtChannelPanFbCnt(mml, (opeNum % 6) % 3 + (opeNum / 6) * 3, (int)pw.pan.val, inst[26], inst[25]);

            pw.beforeVolume = -1;
        }

        private void SetInst2Operator(partWork pw, MML mml, int n, int modeBeforeSend, int vch)
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
                        parent.OutData(mml, port, (byte)(targetBaseReg + ope * 3 + 0x80)
                            , ((0 & 0xf) << 4) | (15 & 0xf));//SL RR
                    break;
                case 2: // A)ll
                    for (byte ope = 0; ope < 2; ope++)
                    {
                        SetInstAtOneOpeWithoutKslTl(mml, (vch / 3 * 6) + (vch % 3) + ope * 3
                            , 15, 15, 0, 15, 0,  0, 0, 0, 0, 0);
                        parent.OutData(mml, port, (byte)(targetBaseReg + ope * 3 + 0x40)
                            , ((0 & 0x3) << 6) | 0x3f);  //KL(M) TL
                    }
                    break;
            }

            int slot1_operatorNumber = (vch / 3 * 6) + (vch % 3) + 0;

            for (int ope = 0; ope < 2; ope++)
            {
                SetInstAtOneOpeWithoutKslTl(mml, slot1_operatorNumber + ope * 3,
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
                parent.OutData(mml, port, (byte)(0x40 + ChnToBaseReg(vch) + 0)
                    , (byte)(((inst[12 * 0 + 5] & 0x3) << 6) | (inst[12 * 0 + 6] & 0x3f))); //KL(M) TL
            }

            SetInstAtChannelPanFbCnt(mml, vch, (int)pw.pan.val, inst[26], inst[25]);

            pw.beforeVolume = -1;
        }

        private void SetInst4Operator(partWork pw, MML mml, int n, int modeBeforeSend, int vch)
        {
            if (!pw.isOp4Mode)
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
                        parent.OutData(mml, port, (byte)(targetBaseReg + ope * 3 + 0x80)
                            , ((0 & 0xf) << 4) | (15 & 0xf));//SL RR
                    break;
                case 2: // A)ll
                    for (byte ope = 0; ope < 2; ope++)
                    {
                        SetInstAtOneOpeWithoutKslTl(mml, (vch / 3 * 6) + (vch % 3) + ope * 3
                            , 15, 15, 0, 15, 0, 0, 0,  0, 0, 0);
                        parent.OutData(mml, port, (byte)(targetBaseReg + ope * 3 + 0x40)
                            , ((0 & 0x3) << 6) | 0x3f);  //KL(M) TL
                    }
                    break;
            }

            int slot1_operatorNumber = (vch / 3 * 6) + (vch % 3) + 0;

            for (int ope = 0; ope < 4; ope++)
            {
                SetInstAtOneOpeWithoutKslTl(mml, slot1_operatorNumber + ope * 3,
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
                parent.OutData(mml, port, (byte)(0x40 + ChnToBaseReg(vch) + 0)
                    , (byte)(((inst[12 * 0 + 5] & 0x3) << 6) | (inst[12 * 0 + 6] & 0x3f))); //KL(M) TL

            if (op2)
                parent.OutData(mml, port, (byte)(0x40 + ChnToBaseReg(vch) + 3)
                    , (byte)(((inst[12 * 1 + 5] & 0x3) << 6) | (inst[12 * 1 + 6] & 0x3f))); //KL(M) TL

            if (op3)
                parent.OutData(mml, port, (byte)(0x40 + ChnToBaseReg(vch) + 8)
                    , (byte)(((inst[12 * 2 + 5] & 0x3) << 6) | (inst[12 * 2 + 6] & 0x3f))); //KL(M) TL


            SetInstAtChannelPanFbCnt(mml, vch, (int)pw.pan.val, inst[51], cnt1);
            SetInstAtChannelPanFbCnt(mml, vch + 3, (int)pw.pan.val, inst[51], cnt2);

            pw.beforeVolume = -1;
        }

        private void SetInstAtChannelPanFbCnt(MML mml, int chNum,int pan,int fb,int cnt)
        {
            //portは9channel毎に切り替わる
            byte[] port = this.port[chNum / 9];

            parent.OutData(
                mml,
                port,
                (byte)(chNum % 9 + 0xC0),
                (byte)(
                    ((fb & 0x07) << 1) | (cnt & 0x01) | (pan * 0x10) // PAN(CHA,CHB (CHC,CHDは未使用))
                )
            );
        }

        public void SetInstAtOneOpeAmVibEgKsMl(MML mml, byte[] port, byte adr, int ml, int am, int vib, int eg, int kr)
        {
            // 0x20
            parent.OutData(
                mml,
                port,
                adr,
                 (byte)((am != 0 ? 0x80 : 0) + (vib != 0 ? 0x40 : 0) + (eg != 0 ? 0x20 : 0) + (kr != 0 ? 0x10 : 0) + (ml & 0xf))
                );
        }

        private void SetInstAtOneOpeWithoutKslTl(MML mml, int opeNum,
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

            parent.OutData(mml, port, (byte)(0x80 + adr), (byte)(((sl & 0xf) << 4) | (rr & 0xf)));
            parent.OutData(mml, port, (byte)(0x60 + adr), (byte)(((ar & 0xf) << 4) | (dr & 0xf)));
            SetInstAtOneOpeAmVibEgKsMl(mml, port, (byte)(0x20 + adr), mt, am, vib, eg, kr);
            parent.OutData(mml, port, (byte)(0xe0 + adr), (byte)(ws & 0x7));
        }

        public void OutFmSetFnum(partWork pw, int octave, int num)
        {
            int freq;
            freq = (int)((num & 0x3ff) | (((octave-1 ) & 0x7) << 10));
            pw.freq = freq;
        }

        public void SetFmFNum(partWork pw, MML mml)
        {
            if (pw.noteCmd == (char)0)
            {
                return;
            }

            //if (pw.Type == enmChannelType.RHYTHM) return;

            int[] ftbl = FNumTbl[0];

            int f = GetFmFNum(ftbl, pw.octaveNow, pw.noteCmd, pw.shift + pw.keyShift + pw.toneDoublerKeyShift);//
            if (pw.bendWaitCounter != -1)
            {
                f = pw.bendFnum;
                Console.Write("ff:{0}   ", f);
            }
            int o = (f & 0x1c00) >> 10;
            f &= 0x3ff;

            f += pw.detune;
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
            OutFmSetFnum(pw, o, f);
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

            int f = ftbl[n];

            return (f & 0x3ff) + ((o & 0x7) << 10);
        }

        public void SetFmVolume(partWork pw, MML mml)
        {
            int vol = pw.volume;

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

            //if (pw.beforeVolume != vol)
            //{
            //if (parent.instFM.ContainsKey(pw.instrument))
            //{
            pw.volume = vol;
            //outYM2413SetInstVol(pw, pw.envInstrument, vol);
            //pw.beforeVolume = vol;
            //}
            //}
        }

        public override void GetFNumAtoB(partWork pw, MML mml
            , out int a, int aOctaveNow, char aCmd, int aShift
            , out int b, int bOctaveNow, char bCmd, int bShift
            , int dir)
        {
            a = GetFNum(pw, mml, aOctaveNow, aCmd, aShift);
            b = GetFNum(pw, mml, bOctaveNow, bCmd, bShift);

            int oa = (a & 0x1c00) >>10;
            int ob = (b & 0x1c00) >>10;
            if (oa != ob)
            {
                if ((a & 0x3ff) == FNumTbl[0][0])
                {
                    oa += Math.Sign(ob - oa);
                    a = (a & 0x3ff) * 2 + (oa <<10);
                }
                else if ((b & 0x3ff) == FNumTbl[0][0])
                {
                    ob += Math.Sign(oa - ob);
                    b = (b & 0x3ff) * ((dir > 0) ? 2 : 1) + (ob <<10);
                }
            }
        }

        public override void SetFNum(partWork pw, MML mml)
        {
            SetFmFNum(pw, mml);
        }

        public override int GetFNum(partWork pw, MML mml, int octave, char cmd, int shift)
        {
            int[] ftbl = FNumTbl[0];
            return GetFmFNum(ftbl, octave, cmd, shift);
        }

        public override void SetVolume(partWork pw, MML mml)
        {
            SetFmVolume(pw, mml);
        }

        public override void SetKeyOn(partWork pw, MML mml)
        {
            pw.keyOn = true;
            SetDummyData(pw, mml);
        }

        public override void SetKeyOff(partWork pw, MML mml)
        {
            pw.keyOn = false;
            pw.keyOff = true;
        }

        public override void SetLfoAtKeyOn(partWork pw, MML mml)
        {
        }

        public override void SetToneDoubler(partWork pw, MML mml)
        {
            //実装不要
        }

        public override int GetToneDoublerShift(partWork pw, int octave, char noteCmd, int shift)
        {
            return 0;
        }


        public override void CmdInstrument(partWork pw, MML mml)
        {
            char type = (char)mml.args[0];
            int n = (int)mml.args[1];

            if (type == 'T')
            {
                msgBox.setErrMsg(msg.get("E17001"), mml.line.Lp);
                return;
            }

            if (type == 'E')
            {
                n = SetEnvelopParamFromInstrument(pw, n, mml);
                return;
            }

            if (type == 'I')
            {
                n = Common.CheckRange(n, 1, 15);
                if (pw.envInstrument != n)
                {
                    pw.envInstrument = n;
                    //outYM2413SetInstVol(pw, n, pw.volume); //INSTをnにセット
                }
                SetDummyData(pw, mml);
                return;
            }

            n = Common.CheckRange(n, 0, 255);
            if (pw.instrument == n) return;

            pw.instrument = n;
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

            outYMF262SetInstrument(pw, mml, n, modeBeforeSend); //音色のセット
            pw.envInstrument = 0;
            //outYM2413SetInstVol(pw, 0, pw.volume); //INSTを0にセット

        }

        public override void CmdMode(partWork pw, MML mml)
        {
            Console.WriteLine("CmdMode()");

            int n = (int)mml.args[0];

            if ((pw.ch > 5 && pw.ch < 9) || pw.Type== enmChannelType.RHYTHM)
            {
                if (n == 0) pw.chip.rhythmStatus &= 0xdf;
                else pw.chip.rhythmStatus |= 0x20;

                return;
            }

            if(pw.Type== enmChannelType.FMOPL)
            {
                if((pw.ch >= 0 && pw.ch < 6)|| (pw.ch >= 9 && pw.ch < 15))
                {
                    int tch = (pw.ch % 9) % 3 + (pw.ch / 9) * 3;

                    if (n == 0)
                    {
                        pw.chip.connectionSel &= (~(1 << tch)) & 0x3f;
                        
                        tch += (pw.ch > 8 ? 6 : 0);
                        pw.chip.lstPartWork[tch].isOp4Mode = false;
                        pw.chip.lstPartWork[tch + 3].isOp4Mode = false;
                    }
                    else
                    {
                        pw.chip.connectionSel |= (1 << tch);

                        tch += (pw.ch > 8 ? 6 : 0);
                        pw.chip.lstPartWork[tch].isOp4Mode = true;
                        pw.chip.lstPartWork[tch + 3].isOp4Mode = true;
                    }

                }
            }
        }

        public override void CmdPan(partWork pw, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 0, 3);
            pw.pan.val = ((n & 1) << 1) | ((n & 2) >> 1);//LR反転
            int vch = pw.ch;

            if (pw.Type == enmChannelType.RHYTHM)
            {
                if (pw.ch == 18) vch = 6;
                else if (pw.ch == 19) vch = 7;
                else if (pw.ch == 20) vch = 8;
                else if (pw.ch == 21) vch = 8;
                else if (pw.ch == 22) vch = 7;
            }

            byte[] port = getPortFromCh(vch);

            byte PanFbCnt = 0;
            if (pw.instrument != -1)
            {
                PanFbCnt = (byte)(
                    (parent.instFM[pw.instrument][26] & 0x07) << 1
                | parent.instFM[pw.instrument][25] & 0x01
                );
            }

            parent.OutData(mml, port, (byte)(vch % 9 + 0xC0), (byte)((
                PanFbCnt
                | (pw.pan.val * 0x10) // PAN
                )));

            SetDummyData(pw, mml);
        }

        public override void CmdLoopExtProc(partWork pw, MML mml)
        {
        }

        public override void CmdSusOnOff(partWork pw, MML mml)
        {
            char c = (char)mml.args[0];
            pw.sus = (c == 'o');
        }

        public override void CmdY(partWork pw, MML mml)
        {
            if (mml.args[0] is string) return;

            if (mml.args.Count == 2)
            {
                byte adr = (byte)(int)mml.args[0];
                byte dat = (byte)(int)mml.args[1];
                int p = 0;
                if (pw.ch < 9)//FM1-9
                    p = 0;
                else if (pw.ch < 18)//FM10-18
                    p = 1;
                else
                    p = 0;

                parent.OutData(mml, port[p], adr, dat);
            }
            else
            {
                byte prt = (byte)mml.args[0];
                byte adr = (byte)mml.args[1];
                byte dat = (byte)mml.args[2];

                parent.OutData(mml, port[prt & 1], adr, dat);

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
                if (pw.Type == enmChannelType.FMOPL)
                {
                    if (!pw.isOp4Mode)
                    {
                        if (pw.beforeVolume != pw.volume && parent.instFM.ContainsKey(pw.instrument))
                        {
                            pw.beforeVolume = pw.volume;


                            int cnt = parent.instFM[pw.instrument][25];
                            if (cnt != 0)
                            {
                                //OP1
                                parent.OutData(
                                    mml,
                                    port[pw.ch / 9],
                                    (byte)(0x40 + ChnToBaseReg(pw.ch) + 0),
                                    (byte)(
                                            ((parent.instFM[pw.instrument][12 * 0 + 5] & 0x3) << 6)  //KL(M)
                                            | Common.CheckRange(((parent.instFM[pw.instrument][12 * 0 + 6] & 0x3f) + (63 - (pw.volume & 0x3f))), 0, 63) //TL
                                        )
                                    );
                            }
                            //OP2
                            parent.OutData(
                                mml,
                                port[pw.ch / 9],
                                (byte)(0x40 + ChnToBaseReg(pw.ch) + 3),
                                (byte)(
                                        ((parent.instFM[pw.instrument][12 * 1 + 5] & 0x3) << 6)  //KL(M)
                                        | Common.CheckRange(((parent.instFM[pw.instrument][12 * 1 + 6] & 0x3f) + (63 - (pw.volume & 0x3f))), 0, 63) //TL
                                    )
                                );
                        }

                    }
                    else
                    {
                        if ((pw.ch >= 3 && pw.ch <= 5) || (pw.ch >= 12 && pw.ch <= 14)) continue;

                        if (pw.beforeVolume != pw.volume && parent.instFM.ContainsKey(pw.instrument))
                        {
                            pw.beforeVolume = pw.volume;

                            int cnt1 = parent.instFM[pw.instrument][49];
                            int cnt2 = parent.instFM[pw.instrument][50];
                            bool[] op = new bool[] { false, false, false, false };

                            if (cnt1 == 0 && cnt2 == 0) { op[3] = true; }
                            else if (cnt1 == 0 && cnt2 == 1) { op[1] = true; op[3] = true; }
                            else if (cnt1 == 1 && cnt2 == 0) { op[0] = true; op[3] = true; }
                            else if (cnt1 == 1 && cnt2 == 1) { op[0] = true; op[2] = true; op[3] = true; }

                            for (int i = 0; i < 4; i++)
                            {
                                if (!op[i]) continue;
                                parent.OutData(mml, port[pw.ch / 9], (byte)(0x40 + ChnToBaseReg(pw.ch) + i * 3 + (i > 1 ? 2 : 0)),
                                    (byte)(
                                        ((parent.instFM[pw.instrument][12 * i + 5] & 0x3) << 6)  //KL(M)
                                        | Common.CheckRange(((parent.instFM[pw.instrument][12 * i + 6] & 0x3f) + (63 - (pw.volume & 0x3f))), 0, 63) //TL
                                    )
                                );
                            }
                        }
                    }

                    if (pw.keyOff)
                    {
                        pw.keyOff = false;
                        parent.OutData(mml, getPortFromCh(pw.ch)
                            , (byte)(0xB0 + pw.ch % 9)
                            , (byte)(
                                ((pw.freq >> 8) & 0x1f)
                              )
                            );
                    }

                    if (pw.beforeFNum != (pw.freq | (pw.keyOn ? 0x4000 : 0x0000)))
                    {
                        pw.beforeFNum = pw.freq | (pw.keyOn ? 0x4000 : 0x0000);
                        //Console.WriteLine("CalcPitch {0} {1}_{2}", pw.freq, pw.freq >> 8 & 0x1F, pw.freq & 0xFF);
                        parent.OutData(mml, getPortFromCh(pw.ch), (byte)(0xa0 + pw.ch % 9), (byte)pw.freq);
                        parent.OutData(mml, getPortFromCh(pw.ch)
                            , (byte)(0xB0 + pw.ch % 9)
                            , (byte)(
                                ((pw.freq >> 8) & 0x1f)
                                | (pw.keyOn ? 0x20 : 0x00)
                              )
                            );
                    }
                }

                else if (pw.Type == enmChannelType.RHYTHM)
                {
                    if (pw.beforeVolume != pw.volume && parent.instFM.ContainsKey(pw.instrument))
                    {
                        pw.beforeVolume = pw.volume;

                        if (pw.ch == 18)
                        {
                            int vch = 6;

                            int cnt = parent.instFM[pw.instrument][25];
                            if (cnt != 0)
                            {
                                //OP1
                                parent.OutData(
                                    mml,
                                    port[vch / 9],
                                    (byte)(0x40 + ChnToBaseReg(vch) + 0),
                                    (byte)(
                                            ((parent.instFM[pw.instrument][12 * 0 + 5] & 0x3) << 6)  //KL(M)
                                            | Common.CheckRange(((parent.instFM[pw.instrument][12 * 0 + 6] & 0x3f) + (63 - (pw.volume & 0x3f))), 0, 63) //TL
                                        )
                                    );
                            }
                            //OP2
                            parent.OutData(
                                mml,
                                port[vch / 9],
                                (byte)(0x40 + ChnToBaseReg(vch) + 3),
                                (byte)(
                                        ((parent.instFM[pw.instrument][12 * 1 + 5] & 0x3) << 6)  //KL(M)
                                        | Common.CheckRange(((parent.instFM[pw.instrument][12 * 1 + 6] & 0x3f) + (63 - (pw.volume & 0x3f))), 0, 63) //TL
                                    )
                                );
                        }
                        else if (pw.ch == 19)
                        {
                            int vch = 7;
                            //OP2
                            parent.OutData(
                                mml,
                                port[vch / 9],
                                (byte)(0x40 + ChnToBaseReg(vch) + 3),
                                (byte)(
                                        ((parent.instFM[pw.instrument][12 * 1 + 5] & 0x3) << 6)  //KL(M)
                                        | Common.CheckRange(((parent.instFM[pw.instrument][12 * 1 + 6] & 0x3f) + (63 - (pw.volume & 0x3f))), 0, 63) //TL
                                    )
                                );
                        }
                        else if (pw.ch == 20)
                        {
                            int vch = 8;
                            //int cnt = parent.instFM[pw.instrument][25];
                            //if (cnt != 0)
                            {
                                //OP1
                                parent.OutData(
                                    mml,
                                    port[vch / 9],
                                    (byte)(0x40 + ChnToBaseReg(vch) + 0),
                                    (byte)(
                                            ((parent.instFM[pw.instrument][12 * 0 + 5] & 0x3) << 6)  //KL(M)
                                            | Common.CheckRange(((parent.instFM[pw.instrument][12 * 0 + 6] & 0x3f) + (63 - (pw.volume & 0x3f))), 0, 63) //TL
                                        )
                                    );
                            }
                        }
                        else if (pw.ch == 21)
                        {
                            int vch = 8;
                            //OP2
                            parent.OutData(
                                mml,
                                port[vch / 9],
                                (byte)(0x40 + ChnToBaseReg(vch) + 3),
                                (byte)(
                                        ((parent.instFM[pw.instrument][12 * 1 + 5] & 0x3) << 6)  //KL(M)
                                        | Common.CheckRange(((parent.instFM[pw.instrument][12 * 1 + 6] & 0x3f) + (63 - (pw.volume & 0x3f))), 0, 63) //TL
                                    )
                                );
                        }
                        else if (pw.ch == 22)
                        {
                            int vch = 7;
                            //int cnt = parent.instFM[pw.instrument][25];
                            //if (cnt != 0)
                            {
                                //OP1
                                parent.OutData(
                                    mml,
                                    port[vch / 9],
                                    (byte)(0x40 + ChnToBaseReg(vch) + 0),
                                    (byte)(
                                            ((parent.instFM[pw.instrument][12 * 0 + 5] & 0x3) << 6)  //KL(M)
                                            | Common.CheckRange(((parent.instFM[pw.instrument][12 * 0 + 6] & 0x3f) + (63 - (pw.volume & 0x3f))), 0, 63) //TL
                                        )
                                    );
                            }
                        }
                    }

                    if (pw.beforeFNum != (pw.freq | (pw.keyOn ? 0x4000 : 0x0000)))
                    {
                        pw.beforeFNum = pw.freq | (pw.keyOn ? 0x4000 : 0x0000);
                        //Console.WriteLine("CalcPitch {0} {1}_{2}", pw.freq, pw.freq >> 8 & 0x1F, pw.freq & 0xFF);

                        int vch = 0;
                        if (pw.ch == 18)//bd
                        {
                            vch = 6;
                        }
                        else if (pw.ch == 19)//sd
                        {
                            vch = 7;
                        }
                        else if (pw.ch == 20)//tom
                        {
                            vch = 8;
                        }
                        else if (pw.ch == 21)//CYM
                        {
                            vch = 8;
                        }
                        else if (pw.ch == 22)//HH
                        {
                            vch = 7;
                        }

                        parent.OutData(mml, getPortFromCh(vch), (byte)(0xa0 + vch % 9), (byte)pw.freq);
                        parent.OutData(mml, getPortFromCh(vch)
                            , (byte)(0xB0 + vch % 9)
                            , (byte)(
                                ((pw.freq >> 8) & 0x1f)
                                //| (pw.keyOn ? 0x20 : 0x00)
                              )
                            );
                    }

                }
            }


            rhythmStatus &= 0xe0;
            rhythmStatus |= (byte)(
                (lstPartWork[18].keyOn ? 0x10 : 0x00)
                | (lstPartWork[19].keyOn ? 0x08 : 0x00)
                | (lstPartWork[20].keyOn ? 0x04 : 0x00)
                | (lstPartWork[21].keyOn ? 0x02 : 0x00)
                | (lstPartWork[22].keyOn ? 0x01 : 0x00)
                );

            if (beforeRhythmStatus != rhythmStatus)
            {
                beforeRhythmStatus = rhythmStatus;
                parent.OutData(mml, port[0], 0xbd, rhythmStatus);
            }

            /*
                        if (!lstPartWork[9].rhythmMode) return;

                        partWork p0, p1;
                        byte dat;
                        p0 = lstPartWork[9];

                        //Key Off
                        if (lstPartWork[9].keyOff
                            || lstPartWork[10].keyOff
                            || lstPartWork[11].keyOff
                            || lstPartWork[12].keyOff
                            || lstPartWork[13].keyOff)
                        {
                            dat = (byte)(0x20
                                | (lstPartWork[9].keyOn ? (lstPartWork[9].keyOff ? 0 : 0x10) : 0)
                                | (lstPartWork[10].keyOn ? (lstPartWork[10].keyOff ? 0 : 0x08) : 0)
                                | (lstPartWork[11].keyOn ? (lstPartWork[11].keyOff ? 0 : 0x04) : 0)
                                | (lstPartWork[12].keyOn ? (lstPartWork[12].keyOff ? 0 : 0x02) : 0)
                                | (lstPartWork[13].keyOn ? (lstPartWork[13].keyOff ? 0 : 0x01) : 0)
                                );
                            lstPartWork[9].rhythmKeyOnData = dat;
                            parent.OutData(mml, port[0], 0x0e, dat);

                            lstPartWork[9].keyOff = false;
                            lstPartWork[10].keyOff = false;
                            lstPartWork[11].keyOff = false;
                            lstPartWork[12].keyOff = false;
                            lstPartWork[13].keyOff = false;
                        }


                        //Key On
                        dat = (byte)(0x20
                            | (lstPartWork[9].keyOn ? 0x10 : 0)
                            | (lstPartWork[10].keyOn ? 0x08 : 0)
                            | (lstPartWork[11].keyOn ? 0x04 : 0)
                            | (lstPartWork[12].keyOn ? 0x02 : 0)
                            | (lstPartWork[13].keyOn ? 0x01 : 0)
                            );
                        if (lstPartWork[9].rhythmKeyOnData != dat)
                        {
                            lstPartWork[9].rhythmKeyOnData = dat;
                            parent.OutData(mml, port[0], 0x0e, dat);
                        }


                        //Freq
                        p0 = lstPartWork[9];
                        if (p0.freq != -1 && p0.beforeFNum != p0.freq)
                        {
                            p0.beforeFNum = p0.freq;

                            parent.OutData(mml, port[0], (byte)0x16, (byte)p0.freq);
                            parent.OutData(mml, port[0]
                                , (byte)0x26
                                , (byte)((p0.freq >> 8) & 0xf)
                                );
                        }

                        p0 = lstPartWork[10];
                        p1 = lstPartWork[13];
                        if ((p0.freq != -1 && p0.beforeFNum != p0.freq)
                            || (p1.freq != -1 && p1.beforeFNum != p1.freq))
                        {
                            if (p1.freq != -1 && p1.beforeFNum != p1.freq)
                            {
                                p0.beforeFNum = p1.freq;
                                p1.beforeFNum = p1.freq;
                            }
                            else if (p0.freq != -1 && p0.beforeFNum != p0.freq)
                            {
                                p0.beforeFNum = p0.freq;
                                p1.beforeFNum = p0.freq;
                            }

                            if (p0.beforeFNum != -1)
                            {
                                parent.OutData(mml, port[0], (byte)0x17, (byte)p0.beforeFNum);
                                parent.OutData(mml, port[0]
                                    , (byte)0x27
                                    , (byte)((p0.beforeFNum >> 8) & 0xf)
                                    );
                            }
                        }

                        p0 = lstPartWork[12];
                        p1 = lstPartWork[11];
                        if ((p0.freq != -1 && p0.beforeFNum != p0.freq)
                            || (p1.freq != -1 && p1.beforeFNum != p1.freq))
                        {
                            if (p1.freq != -1 && p1.beforeFNum != p1.freq)
                            {
                                p0.beforeFNum = p1.freq;
                                p1.beforeFNum = p1.freq;
                            }
                            else if (p0.freq != -1 && p0.beforeFNum != p0.freq)
                            {
                                p0.beforeFNum = p0.freq;
                                p1.beforeFNum = p0.freq;
                            }

                            if (p0.beforeFNum != -1)
                            {
                                parent.OutData(mml, port[0], (byte)0x18, (byte)p0.beforeFNum);
                                parent.OutData(mml, port[0]
                                    , (byte)0x28
                                    , (byte)((p0.beforeFNum >> 8) & 0xf)
                                    );
                            }
                        }


                        //Rhythm Volume
                        p0 = lstPartWork[9];
                        if (p0.beforeVolume != p0.volume)
                        {
                            p0.beforeVolume = p0.volume;
                            parent.OutData(mml, port[0], 0x36, (byte)(15 - (p0.volume & 0xf)));
                        }
                        p0 = lstPartWork[10];
                        p1 = lstPartWork[13];
                        if (p0.beforeVolume != p0.volume || p1.beforeVolume != p1.volume)
                        {
                            p0.beforeVolume = p0.volume;
                            p1.beforeVolume = p1.volume;
                            parent.OutData(mml, port[0], 0x37, (byte)((15 - (p0.volume & 0xf)) | ((15 - (p1.volume & 0xf)) << 4)));
                        }
                        p0 = lstPartWork[12];
                        p1 = lstPartWork[11];
                        if (p0.beforeVolume != p0.volume || p1.beforeVolume != p1.volume)
                        {
                            p0.beforeVolume = p0.volume;
                            p1.beforeVolume = p1.volume;
                            parent.OutData(mml, port[0], 0x38, (byte)((15 - (p0.volume & 0xf)) | ((15 - (p1.volume & 0xf)) << 4)));
                        }

                */
        }


    }
}
