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


    public class YMF262 : ClsChip
    {
        protected int[][] _FNumTbl = new int[1][] {
            //new int[13]
            new int[] {
            // OPL3(FM) : Fnum = ftone*(2**19)/(M/288)/(2**B-1)       ftone:Hz M:MasterClock B:Block
            //   c    c+     d    d+     e     f    f+     g    g+     a    a+     b    >c
             0x0ac,0x0b5,0x0c0,0x0cc,0x0d8,0x0e5,0x0f2,0x101,0x110,0x120,0x131,0x143,0x0ac*2
            }
        };

        public YMF262(ClsVgm parent, int chipID, string initialPartName, string stPath, int chipNumber) : base(parent, chipID, initialPartName, stPath, chipNumber)
        {

            _Name = "YMF262";
            _ShortName = "OPL3";
            _ChMax = 18; 
            // OPL2 mode = 9*2 2op
            // OPL3 mode (all 2op) = 18 2op channel
            // OPL3 Rhythm Mode = 15*4op + 3 rhythm channel
            // OPL3 4op = 1-4, 2-5, 3-6, 10-13, 11-14, 12-15,(6*4op) 6*2op
            // OPL3 All mode = (4op mode) + 7,8,9(RYM) 3 2op channel
            _canUsePcm = false;

            Frequency = 14318180;
            port =new byte[][] { new byte[] { (byte)(chipNumber!=0 ? 0xae : 0x5e) }, new byte[] { (byte)( chipNumber!=0? 0xaf : 0x5f ) } };

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
            pw.beforeVolume = (pw.Type == enmChannelType.FMOPL) ? 15 : -1;
            pw.volume = 15;
            pw.MaxVolume = 15;
            pw.beforeEnvInstrument = 0;
            pw.envInstrument = 0;
            pw.port = port;
            pw.mixer = 0;
            pw.noise = 0;
        }

        public override void InitChip()
        {

            if (!use) return;

            //FM Off
            outYMF262AllKeyOff(null, lstPartWork[0]);

            /*
             * if (ChipID != 0 && parent.info.format != enmFormat.ZGM)
            {
                parent.dat[0x13] = new outDatum(enmMMLType.unknown, null, null, (byte)(parent.dat[0x13].val | 0x40));//use Secondary(YM2413 OPLL)
            }
            */

        }


        public void outYMF262SetAdr00_01(MML mml, partWork pw, byte adr, bool AM, bool VIB, bool EG, bool KS, int mul)
        {
            // 0x20
            parent.OutData(
                mml,
                port[0]
                , adr
                , (byte)((AM ? 0x80 : 0) + (VIB ? 0x40 : 0) + (EG ? 0x20 : 0) + (KS ? 0x10 : 0) + (mul & 0xf))
                );
        }

        public void outYMF262AllKeyOff(MML mml, partWork pw)
        {
            
            //Rhythm Off
            //parent.OutData(mml, port[0], 0x0e, 0);
            // Probably wise to reset Rhythm mode.
            for (byte adr = 0; adr <= 8; adr++)
            {
                //Ch Off
                parent.OutData(mml, port[0], (byte)(0xB0 + adr), 0);
                parent.OutData(mml, port[1], (byte)(0xB0 + adr), 0);
            }
        }

        public (byte,byte) ChnToBaseReg(int chn)
        {
            byte carrier = 0x20, modulator = 0x23;
            if (chn > 9) chn -= 9; // A1=LでもA1=Hでもいっしょ。

            if(chn <= 3)
            {
                carrier += (byte)chn;
                modulator += (byte)chn;
            } else if(chn <= 6)
            {
                carrier += 8;
                modulator += 8;

                carrier += (byte)(chn-3);
                modulator += (byte)(chn-3);
            } else if(chn <= 9)
            {
                carrier += 16;
                modulator += 16;

                carrier += (byte)(chn - 6);
                modulator += (byte)(chn - 6);
            }
            
            return (carrier, modulator);
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
            /*
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
                        outYMF262SetAdr00_01(mml, pw, ope
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
            }*/

            for (byte ope = 0; ope < 2; ope++)
            {
                byte targetBaseReg = ope == 0 ? ChnToBaseReg(pw.ch).Item1 : ChnToBaseReg(pw.ch).Item2;

                outYMF262SetAdr00_01(mml, pw, targetBaseReg
                    , parent.instFM[n][ope * 11 + 7] != 0 //AM
                    , parent.instFM[n][ope * 11 + 8] != 0 //VIB
                    , parent.instFM[n][ope * 11 + 9] != 0 //EG
                    , parent.instFM[n][ope * 11 + 10] != 0 //KS
                    , parent.instFM[n][ope * 11 + 6] & 0xf //MT
                    );
                parent.OutData(mml, port[0], (byte)(targetBaseReg + 0x40), (byte)((
                    (parent.instFM[n][ope * 11 + 1] & 0xf) << 4) //AR
                    | (parent.instFM[n][ope * 11 + 2] & 0xf) // DR
                    ));
                parent.OutData(mml, port[0], (byte)(targetBaseReg + 0x60), (byte)((
                    (parent.instFM[n][ope * 11 + 3] & 0xf) << 4) //SL
                    | (parent.instFM[n][ope * 11 + 4] & 0xf) // RR
                    ));

                parent.OutData(mml, port[0], (byte)(targetBaseReg + 0x20), (byte)((
                    (parent.instFM[n][0 * 11 + 5] & 0x3) << 6)  //KL(M)
                    | (parent.instFM[n][23] & 0x3f) //TL
                    ));
            }
                parent.OutData(mml, port[0], (byte)(pw.ch%8+0xC0), (byte)((
                    (parent.instFM[n][24] & 0x07)<<1 //FB
                    )));
            
            pw.op1ml = parent.instFM[n][0 * 11 + 5];
            pw.op2ml = parent.instFM[n][1 * 11 + 5];
            pw.op1dt2 = 0;
            pw.op2dt2 = 0;

        }

        //public void outYM2413SetInstVol(partWork pw, int inst, int vol)
        //{
        //    pw.envInstrument = inst & 0xf;
        //    pw.volume = vol & 0xf;

        //    parent.OutData(pw.port0
        //        , (byte)(0x30 + pw.ch)
        //        , (byte)((pw.envInstrument << 4) | (15 - pw.volume))
        //        );
        //}

        public void OutFmSetFnum(partWork pw, int octave, int num)
        {
            int freq;
            freq = (int)((num & 0x1ff) | (((octave - 1) & 0x7) << 9));
            pw.freq = freq;
        }

        public void SetFmFNum(partWork pw, MML mml)
        {
            if (pw.noteCmd == (char)0)
            {
                return;
            }

            int[] ftbl = FNumTbl[0];

            int f = GetFmFNum(ftbl, pw.octaveNow, pw.noteCmd, pw.shift + pw.keyShift + pw.toneDoublerKeyShift);//
            if (pw.bendWaitCounter != -1)
            {
                f = pw.bendFnum;
            }
            int o = (f & 0xf000) / 0x1000;
            f &= 0xfff;

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

            return (f & 0xfff) + (o & 0xf) * 0x1000;
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
            int n = (int)mml.args[0];
            pw.chip.lstPartWork[9].rhythmMode = (n != 0);
            pw.chip.lstPartWork[10].rhythmMode = (n != 0);
            pw.chip.lstPartWork[11].rhythmMode = (n != 0);
            pw.chip.lstPartWork[12].rhythmMode = (n != 0);
            pw.chip.lstPartWork[13].rhythmMode = (n != 0);

        }

        public override void CmdY(partWork pw, MML mml)
        {
            byte adr = (byte)mml.args[0];
            byte dat = (byte)mml.args[1];
            parent.OutData(mml, port[0], adr, dat);
        }

        public override void CmdLoopExtProc(partWork pw, MML mml)
        {
        }

        public override void CmdSusOnOff(partWork pw, MML mml)
        {
            char c = (char)mml.args[0];
            pw.sus = (c == 'o');
        }

        public override void MultiChannelCommand(MML mml)
        {
            foreach (partWork pw in lstPartWork)
            {
                if (pw.Type == enmChannelType.FMOPL)
                {
                    if (pw.beforeEnvInstrument != pw.envInstrument || pw.beforeVolume != pw.volume)
                    {
                        pw.beforeEnvInstrument = pw.envInstrument;
                        pw.beforeVolume = pw.volume;

                        parent.OutData(mml, port[0]
                            , (byte)(0x30 + pw.ch)
                            , (byte)(((pw.envInstrument << 4) & 0xf0) | ((15 - pw.volume) & 0xf))
                            );
                    }

                    if (pw.keyOff)
                    {
                        pw.keyOff = false;
                        parent.OutData(mml, port[0]
                            , (byte)(0x20 + pw.ch)
                            , (byte)(
                                ((pw.freq >> 8) & 0xf)
                              )
                            );
                    }

                    if (pw.beforeFNum != (pw.freq | (pw.keyOn ? 0x1000 : 0x0000)))
                    {
                        pw.beforeFNum = pw.freq | (pw.keyOn ? 0x1000 : 0x0000);

                        parent.OutData(mml, port[0], (byte)(0x10 + pw.ch), (byte)pw.freq);
                        parent.OutData(mml, port[0]
                            , (byte)(0x20 + pw.ch)
                            , (byte)(
                                ((pw.freq >> 8) & 0xf)
                                | (pw.keyOn ? 0x10 : 0x00)
                                | (pw.sus ? 0x20 : 0x00)
                              )
                            );
                    }
                }

            }

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


        }

    }
}
