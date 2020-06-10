using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using musicDriverInterface;

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
            port =new byte[][] { new byte[] { (byte)(chipNumber!=0 ? 0xa1 : 0x51) } };

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
            pw.pg[pw.cpg].beforeVolume = (pw.pg[pw.cpg].Type == enmChannelType.FMOPL) ? 15 : -1;
            pw.pg[pw.cpg].volume = 15;
            pw.pg[pw.cpg].MaxVolume = 15;
            pw.pg[pw.cpg].beforeEnvInstrument = 0;
            pw.pg[pw.cpg].envInstrument = 0;
            pw.pg[pw.cpg].port = port;
            pw.pg[pw.cpg].mixer = 0;
            pw.pg[pw.cpg].noise = 0;
        }

        public override void InitChip()
        {

            if (!use) return;

            //FM Off
            outYM2413AllKeyOff(null, lstPartWork[0]);

            if (ChipID != 0 && parent.info.format != enmFormat.ZGM)
            {
                parent.dat[0x13] = new outDatum(enmMMLType.unknown, null, null, (byte)(parent.dat[0x13].val | 0x40));//use Secondary(YM2413 OPLL)
            }

        }


        public void outYM2413SetAdr00_01(MML mml, partWork pw, byte adr, bool AM, bool VIB, bool EG, bool KS, int mul)
        {
            parent.OutData(
                mml,
                port[0]
                , (byte)(adr & 1)
                , (byte)((AM ? 0x80 : 0) + (VIB ? 0x40 : 0) + (EG ? 0x20 : 0) + (KS ? 0x10 : 0) + (mul & 0xf))
                );
        }

        public void outYM2413AllKeyOff(MML mml, partWork pw)
        {
            //Rhythm Off
            parent.OutData(mml, port[0], 0x0e, 0);
            for (byte adr = 0; adr < 9; adr++)
            {
                //Ch Off
                parent.OutData(mml, port[0], (byte)(0x20 + adr), 0);
                parent.OutData(mml, port[0], (byte)(0x30 + adr), 0);
            }
        }

        public void outYM2413SetInstrument(partWork pw, MML mml, int n, int modeBeforeSend)
        {
            pw.pg[pw.cpg].instrument = n;

            if (!parent.instFM.ContainsKey(n))
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
                        parent.OutData(mml, port[0], (byte)(0x6 + ope), (byte)((
                            (0 & 0xf) << 4) //SL
                            | (15 & 0xf) // RR
                            ));
                    }
                    break;
                case 2: // A)ll
                    for (byte ope = 0; ope < 2; ope++)
                    {
                        outYM2413SetAdr00_01(mml, pw, ope
                            , false //AM
                            , false //VIB
                            , false //EG
                            , false //KS
                            , 0 //MT
                            );
                        parent.OutData(mml, port[0], (byte)(0x4 + ope), (byte)((
                            (15 & 0xf) << 4) //AR
                            | (15 & 0xf) // DR
                            ));
                        parent.OutData(mml, port[0], (byte)(0x6 + ope), (byte)((
                            (0 & 0xf) << 4) //SL
                            | (15 & 0xf) // RR
                            ));
                    }
                    parent.OutData(mml, port[0], (byte)(0x2), (byte)(
                        (0 << 6)  //KL(M)
                        | (0 & 0x3f) //TL
                        ));
                    parent.OutData(mml, port[0], (byte)(0x3), (byte)((
                        (3 & 0x3) << 6) //KL(C)
                        | (0) // DT(M)
                        | (0) // DT(C)
                        | (7 & 0x07) //FB
                        ));
                    break;
            }

            for (byte ope = 0; ope < 2; ope++)
            {
                outYM2413SetAdr00_01(mml, pw, ope
                    , parent.instFM[n][ope * 11 + 7] != 0 //AM
                    , parent.instFM[n][ope * 11 + 8] != 0 //VIB
                    , parent.instFM[n][ope * 11 + 9] != 0 //EG
                    , parent.instFM[n][ope * 11 + 10] != 0 //KS
                    , parent.instFM[n][ope * 11 + 6] & 0xf //MT
                    );
                parent.OutData(mml, port[0], (byte)(0x4 + ope), (byte)((
                    (parent.instFM[n][ope * 11 + 1] & 0xf) << 4) //AR
                    | (parent.instFM[n][ope * 11 + 2] & 0xf) // DR
                    ));
                parent.OutData(mml, port[0], (byte)(0x6 + ope), (byte)((
                    (parent.instFM[n][ope * 11 + 3] & 0xf) << 4) //SL
                    | (parent.instFM[n][ope * 11 + 4] & 0xf) // RR
                    ));
            }
            parent.OutData(mml, port[0], (byte)(0x2), (byte)((
                (parent.instFM[n][0 * 11 + 5] & 0x3) << 6)  //KL(M)
                | (parent.instFM[n][23] & 0x3f) //TL
                ));
            parent.OutData(mml, port[0], (byte)(0x3), (byte)((
                (parent.instFM[n][1 * 11 + 5] & 0x3) << 6) //KL(C)
                | (parent.instFM[n][0 * 11 + 11] != 0 ? 0x08 : 0) // DT(M)
                | (parent.instFM[n][1 * 11 + 11] != 0 ? 0x10 : 0) // DT(C)
                | (parent.instFM[n][24] & 0x07) //FB
                ));

            pw.pg[pw.cpg].op1ml = parent.instFM[n][0 * 11 + 5];
            pw.pg[pw.cpg].op2ml = parent.instFM[n][1 * 11 + 5];
            pw.pg[pw.cpg].op1dt2 = 0;
            pw.pg[pw.cpg].op2dt2 = 0;

        }

        //public void outYM2413SetInstVol(partWork pw, int inst, int vol)
        //{
        //    pw.ppg[pw.cpgNum].envInstrument = inst & 0xf;
        //    pw.ppg[pw.cpgNum].volume = vol & 0xf;

        //    parent.OutData(pw.ppg[pw.cpgNum].port0
        //        , (byte)(0x30 + pw.ppg[pw.cpgNum].ch)
        //        , (byte)((pw.ppg[pw.cpgNum].envInstrument << 4) | (15 - pw.ppg[pw.cpgNum].volume))
        //        );
        //}
        public override void GetFNumAtoB(partWork pw, MML mml
            , out int a, int aOctaveNow, char aCmd, int aShift
            , out int b, int bOctaveNow, char bCmd, int bShift
            , int dir)
        {
            a = GetFNum(pw, mml, aOctaveNow, aCmd, aShift);
            b = GetFNum(pw, mml, bOctaveNow, bCmd, bShift);

            int oa = (a & 0x0e00) >> 9;
            int ob = (b & 0x0e00) >> 9;
            if (oa != ob)
            {
                if ((a & 0x1ff) == FNumTbl[0][0])
                {
                    oa += Math.Sign(ob - oa);
                    a = (a & 0x1ff) * 2 + (oa << 9);
                }
                else if ((b & 0x1ff) == FNumTbl[0][0])
                {
                    ob += Math.Sign(oa - ob);
                    b = (b & 0x1ff) * ((dir > 0) ? 2 : 1) + (ob << 9);
                }
            }
        }

        public void OutFmSetFnum(partWork pw, int octave, int num)
        {
            int freq;
            freq = (int)((num & 0x1ff) | (((octave - 1) & 0x7) << 9));
            pw.pg[pw.cpg].freq = freq;
        }

        public void SetFmFNum(partWork pw, MML mml)
        {
            if (pw.pg[pw.cpg].noteCmd == (char)0)
            {
                return;
            }

            int[] ftbl = FNumTbl[0];

            int f = GetFmFNum(ftbl, pw.pg[pw.cpg].octaveNow, pw.pg[pw.cpg].noteCmd, pw.pg[pw.cpg].shift + pw.pg[pw.cpg].keyShift + pw.pg[pw.cpg].toneDoublerKeyShift);//
            if (pw.pg[pw.cpg].bendWaitCounter != -1)
            {
                f = pw.pg[pw.cpg].bendFnum;
            }
            int o = (f & 0x0e00) / 0x0200;
            f &= 0x1ff;

            f = f + pw.pg[pw.cpg].detune;
            for (int lfo = 0; lfo < 4; lfo++)
            {
                if (!pw.pg[pw.cpg].lfo[lfo].sw)
                {
                    continue;
                }
                if (pw.pg[pw.cpg].lfo[lfo].type != eLfoType.Vibrato)
                {
                    continue;
                }
                f += pw.pg[pw.cpg].lfo[lfo].value + pw.pg[pw.cpg].lfo[lfo].param[6];
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

            return (f & 0x1ff) + (o & 0x7) * 0x0200;
        }

        public void SetFmVolume(partWork pw, MML mml)
        {
            int vol = pw.pg[pw.cpg].volume;

            for (int lfo = 0; lfo < 4; lfo++)
            {
                if (!pw.pg[pw.cpg].lfo[lfo].sw)
                {
                    continue;
                }
                if (pw.pg[pw.cpg].lfo[lfo].type != eLfoType.Tremolo)
                {
                    continue;
                }
                vol += pw.pg[pw.cpg].lfo[lfo].value + pw.pg[pw.cpg].lfo[lfo].param[6];
            }

            //if (pw.ppg[pw.cpgNum].beforeVolume != vol)
            //{
            //if (parent.instFM.ContainsKey(pw.ppg[pw.cpgNum].instrument))
            //{
            pw.pg[pw.cpg].volume = vol;
            //outYM2413SetInstVol(pw, pw.ppg[pw.cpgNum].envInstrument, vol);
            //pw.ppg[pw.cpgNum].beforeVolume = vol;
            //}
            //}
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
            pw.pg[pw.cpg].keyOn = true;
            SetDummyData(pw, mml);
        }

        public override void SetKeyOff(partWork pw, MML mml)
        {
            pw.pg[pw.cpg].keyOn = false;
            pw.pg[pw.cpg].keyOff = true;
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
                if (pw.pg[pw.cpg].envInstrument != n)
                {
                    pw.pg[pw.cpg].envInstrument = n;
                    //outYM2413SetInstVol(pw, n, pw.ppg[pw.cpgNum].volume); //INSTをnにセット
                }
                SetDummyData(pw, mml);
                return;
            }

            n = Common.CheckRange(n, 0, 255);
            if (pw.pg[pw.cpg].instrument == n) return;

            pw.pg[pw.cpg].instrument = n;
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

            outYM2413SetInstrument(pw, mml, n, modeBeforeSend); //音色のセット
            pw.pg[pw.cpg].envInstrument = 0;
            //outYM2413SetInstVol(pw, 0, pw.ppg[pw.cpgNum].volume); //INSTを0にセット

        }

        public override void CmdMode(partWork pw, MML mml)
        {
            int n = (int)mml.args[0];
            pw.pg[pw.cpg].chip.lstPartWork[9].pg[lstPartWork[9].cpg].rhythmMode = (n != 0);
            pw.pg[pw.cpg].chip.lstPartWork[10].pg[lstPartWork[10].cpg].rhythmMode = (n != 0);
            pw.pg[pw.cpg].chip.lstPartWork[11].pg[lstPartWork[11].cpg].rhythmMode = (n != 0);
            pw.pg[pw.cpg].chip.lstPartWork[12].pg[lstPartWork[12].cpg].rhythmMode = (n != 0);
            pw.pg[pw.cpg].chip.lstPartWork[13].pg[lstPartWork[13].cpg].rhythmMode = (n != 0);

        }

        public override void CmdY(partWork pw, MML mml)
        {
            byte adr = (byte)(int)mml.args[0];
            byte dat = (byte)(int)mml.args[1];
            parent.OutData(mml, port[0], adr, dat);
        }

        public override void CmdLoopExtProc(partWork pw, MML mml)
        {
        }

        public override void CmdSusOnOff(partWork pw, MML mml)
        {
            char c = (char)mml.args[0];
            pw.pg[pw.cpg].sus = (c == 'o');
        }

        public override void MultiChannelCommand(MML mml)
        {
            foreach (partWork pw in lstPartWork)
            {
                if (pw.pg[pw.cpg].Type == enmChannelType.FMOPL)
                {
                    if (pw.pg[pw.cpg].beforeEnvInstrument != pw.pg[pw.cpg].envInstrument || pw.pg[pw.cpg].beforeVolume != pw.pg[pw.cpg].volume)
                    {
                        pw.pg[pw.cpg].beforeEnvInstrument = pw.pg[pw.cpg].envInstrument;
                        pw.pg[pw.cpg].beforeVolume = pw.pg[pw.cpg].volume;

                        parent.OutData(mml, port[0]
                            , (byte)(0x30 + pw.pg[pw.cpg].ch)
                            , (byte)(((pw.pg[pw.cpg].envInstrument << 4) & 0xf0) | ((15 - pw.pg[pw.cpg].volume) & 0xf))
                            );
                    }

                    if (pw.pg[pw.cpg].keyOff)
                    {
                        pw.pg[pw.cpg].keyOff = false;
                        parent.OutData(mml, port[0]
                            , (byte)(0x20 + pw.pg[pw.cpg].ch)
                            , (byte)(
                                ((pw.pg[pw.cpg].freq >> 8) & 0xf)
                              )
                            );
                    }

                    if (pw.pg[pw.cpg].beforeFNum != (pw.pg[pw.cpg].freq | (pw.pg[pw.cpg].keyOn ? 0x1000 : 0x0000)))
                    {
                        pw.pg[pw.cpg].beforeFNum = pw.pg[pw.cpg].freq | (pw.pg[pw.cpg].keyOn ? 0x1000 : 0x0000);

                        parent.OutData(mml, port[0], (byte)(0x10 + pw.pg[pw.cpg].ch), (byte)pw.pg[pw.cpg].freq);
                        parent.OutData(mml, port[0]
                            , (byte)(0x20 + pw.pg[pw.cpg].ch)
                            , (byte)(
                                ((pw.pg[pw.cpg].freq >> 8) & 0xf)
                                | (pw.pg[pw.cpg].keyOn ? 0x10 : 0x00)
                                | (pw.pg[pw.cpg].sus ? 0x20 : 0x00)
                              )
                            );
                    }
                }

            }

            if (!lstPartWork[9].pg[lstPartWork[9].cpg].rhythmMode) return;

            partWork p0, p1;
            byte dat;
            p0 = lstPartWork[9];

            //Key Off
            if (lstPartWork[9].pg[lstPartWork[9].cpg].keyOff
                || lstPartWork[10].pg[lstPartWork[10].cpg].keyOff
                || lstPartWork[11].pg[lstPartWork[11].cpg].keyOff
                || lstPartWork[12].pg[lstPartWork[12].cpg].keyOff
                || lstPartWork[13].pg[lstPartWork[13].cpg].keyOff)
            {
                dat = (byte)(0x20
                    | (lstPartWork[9].pg[lstPartWork[9].cpg].keyOn ? (lstPartWork[9].pg[lstPartWork[9].cpg].keyOff ? 0 : 0x10) : 0)
                    | (lstPartWork[10].pg[lstPartWork[10].cpg].keyOn ? (lstPartWork[10].pg[lstPartWork[10].cpg].keyOff ? 0 : 0x08) : 0)
                    | (lstPartWork[11].pg[lstPartWork[11].cpg].keyOn ? (lstPartWork[11].pg[lstPartWork[11].cpg].keyOff ? 0 : 0x04) : 0)
                    | (lstPartWork[12].pg[lstPartWork[12].cpg].keyOn ? (lstPartWork[12].pg[lstPartWork[12].cpg].keyOff ? 0 : 0x02) : 0)
                    | (lstPartWork[13].pg[lstPartWork[13].cpg].keyOn ? (lstPartWork[13].pg[lstPartWork[13].cpg].keyOff ? 0 : 0x01) : 0)
                    );
                lstPartWork[9].pg[lstPartWork[9].cpg].rhythmKeyOnData = dat;
                parent.OutData(mml, port[0], 0x0e, dat);

                lstPartWork[9].pg[lstPartWork[9].cpg].keyOff = false;
                lstPartWork[10].pg[lstPartWork[10].cpg].keyOff = false;
                lstPartWork[11].pg[lstPartWork[11].cpg].keyOff = false;
                lstPartWork[12].pg[lstPartWork[12].cpg].keyOff = false;
                lstPartWork[13].pg[lstPartWork[13].cpg].keyOff = false;
            }


            //Key On
            dat = (byte)(0x20
                | (lstPartWork[9].pg[lstPartWork[9].cpg].keyOn ? 0x10 : 0)
                | (lstPartWork[10].pg[lstPartWork[10].cpg].keyOn ? 0x08 : 0)
                | (lstPartWork[11].pg[lstPartWork[11].cpg].keyOn ? 0x04 : 0)
                | (lstPartWork[12].pg[lstPartWork[12].cpg].keyOn ? 0x02 : 0)
                | (lstPartWork[13].pg[lstPartWork[13].cpg].keyOn ? 0x01 : 0)
                );
            if (lstPartWork[9].pg[lstPartWork[9].cpg].rhythmKeyOnData != dat)
            {
                lstPartWork[9].pg[lstPartWork[9].cpg].rhythmKeyOnData = dat;
                parent.OutData(mml, port[0], 0x0e, dat);
            }


            //Freq
            p0 = lstPartWork[9];
            if (p0.pg[p0.cpg].freq != -1 && p0.pg[p0.cpg].beforeFNum != p0.pg[p0.cpg].freq)
            {
                p0.pg[p0.cpg].beforeFNum = p0.pg[p0.cpg].freq;

                parent.OutData(mml, port[0], (byte)0x16, (byte)p0.pg[p0.cpg].freq);
                parent.OutData(mml, port[0]
                    , (byte)0x26
                    , (byte)((p0.pg[p0.cpg].freq >> 8) & 0xf)
                    );
            }

            p0 = lstPartWork[10];
            p1 = lstPartWork[13];
            if ((p0.pg[p0.cpg].freq != -1 && p0.pg[p0.cpg].beforeFNum != p0.pg[p0.cpg].freq)
                || (p1.pg[p1.cpg].freq != -1 && p1.pg[p1.cpg].beforeFNum != p1.pg[p1.cpg].freq))
            {
                if (p1.pg[p1.cpg].freq != -1 && p1.pg[p1.cpg].beforeFNum != p1.pg[p1.cpg].freq)
                {
                    p0.pg[p0.cpg].beforeFNum = p1.pg[p1.cpg].freq;
                    p1.pg[p1.cpg].beforeFNum = p1.pg[p1.cpg].freq;
                }
                else if (p0.pg[p0.cpg].freq != -1 && p0.pg[p0.cpg].beforeFNum != p0.pg[p0.cpg].freq)
                {
                    p0.pg[p0.cpg].beforeFNum = p0.pg[p0.cpg].freq;
                    p1.pg[p1.cpg].beforeFNum = p0.pg[p0.cpg].freq;
                }

                if (p0.pg[p0.cpg].beforeFNum != -1)
                {
                    parent.OutData(mml, port[0], (byte)0x17, (byte)p0.pg[p0.cpg].beforeFNum);
                    parent.OutData(mml, port[0]
                        , (byte)0x27
                        , (byte)((p0.pg[p0.cpg].beforeFNum >> 8) & 0xf)
                        );
                }
            }

            p0 = lstPartWork[12];
            p1 = lstPartWork[11];
            if ((p0.pg[p0.cpg].freq != -1 && p0.pg[p0.cpg].beforeFNum != p0.pg[p0.cpg].freq)
                || (p1.pg[p1.cpg].freq != -1 && p1.pg[p1.cpg].beforeFNum != p1.pg[p1.cpg].freq))
            {
                if (p1.pg[p1.cpg].freq != -1 && p1.pg[p1.cpg].beforeFNum != p1.pg[p1.cpg].freq)
                {
                    p0.pg[p0.cpg].beforeFNum = p1.pg[p1.cpg].freq;
                    p1.pg[p1.cpg].beforeFNum = p1.pg[p1.cpg].freq;
                }
                else if (p0.pg[p0.cpg].freq != -1 && p0.pg[p0.cpg].beforeFNum != p0.pg[p0.cpg].freq)
                {
                    p0.pg[p0.cpg].beforeFNum = p0.pg[p0.cpg].freq;
                    p1.pg[p1.cpg].beforeFNum = p0.pg[p0.cpg].freq;
                }

                if (p0.pg[p0.cpg].beforeFNum != -1)
                {
                    parent.OutData(mml, port[0], (byte)0x18, (byte)p0.pg[p0.cpg].beforeFNum);
                    parent.OutData(mml, port[0]
                        , (byte)0x28
                        , (byte)((p0.pg[p0.cpg].beforeFNum >> 8) & 0xf)
                        );
                }
            }


            //Rhythm Volume
            p0 = lstPartWork[9];
            if (p0.pg[p0.cpg].beforeVolume != p0.pg[p0.cpg].volume)
            {
                p0.pg[p0.cpg].beforeVolume = p0.pg[p0.cpg].volume;
                parent.OutData(mml, port[0], 0x36, (byte)(15 - (p0.pg[p0.cpg].volume & 0xf)));
            }
            p0 = lstPartWork[10];
            p1 = lstPartWork[13];
            if (p0.pg[p0.cpg].beforeVolume != p0.pg[p0.cpg].volume || p1.pg[p1.cpg].beforeVolume != p1.pg[p1.cpg].volume)
            {
                p0.pg[p0.cpg].beforeVolume = p0.pg[p0.cpg].volume;
                p1.pg[p1.cpg].beforeVolume = p1.pg[p1.cpg].volume;
                parent.OutData(mml, port[0], 0x37, (byte)((15 - (p0.pg[p0.cpg].volume & 0xf)) | ((15 - (p1.pg[p1.cpg].volume & 0xf)) << 4)));
            }
            p0 = lstPartWork[12];
            p1 = lstPartWork[11];
            if (p0.pg[p0.cpg].beforeVolume != p0.pg[p0.cpg].volume || p1.pg[p1.cpg].beforeVolume != p1.pg[p1.cpg].volume)
            {
                p0.pg[p0.cpg].beforeVolume = p0.pg[p0.cpg].volume;
                p1.pg[p1.cpg].beforeVolume = p1.pg[p1.cpg].volume;
                parent.OutData(mml, port[0], 0x38, (byte)((15 - (p0.pg[p0.cpg].volume & 0xf)) | ((15 - (p1.pg[p1.cpg].volume & 0xf)) << 4)));
            }


        }

    }
}
