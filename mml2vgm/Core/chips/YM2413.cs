using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class YM2413 : ClsChip
    {
        public int[][] FNumTbl = new int[1][] {
            //new int[13]
            new int[] {
            // OPLL(FM) : Fnum = 9 * 2^(22-B) * ftone / M       ftone:Hz M:MasterClock B:Block
            //   c    c+     d    d+     e     f    f+     g    g+     a    a+     b    >c
             0x0ac,0x0b5,0x0c0,0x0cc,0x0d8,0x0e5,0x0f2,0x101,0x110,0x120,0x131,0x143,0x0ac*2
            }
        };

        public YM2413(ClsVgm parent, int chipID, string initialPartName, string stPath, bool isSecondary) : base(parent, chipID, initialPartName, stPath, isSecondary)
        {

            _Name = "YM2413";
            _ShortName = "OPLL";
            _ChMax = 14; // FM 9ch + Rhythm 5ch
            _canUsePcm = false;

            Frequency = 3579545;

            MakeFNumTbl();
            Ch = new ClsChannel[ChMax];
            SetPartToCh(Ch, initialPartName);
            int i = 0;
            foreach (ClsChannel ch in Ch)
            {
                ch.Type = i < 9 ? enmChannelType.FMOPL : enmChannelType.RHYTHM;
                ch.isSecondary = chipID == 1;
                i++;
            }

        }

        public override void InitPart(ref partWork pw)
        {
            pw.beforeVolume = 15;
            pw.volume = 15;
            pw.MaxVolume = 15;
            pw.beforeEnvInstrument = 0;
            pw.envInstrument = 0;
            pw.port0 = (byte)(0x1 | (pw.isSecondary ? 0xa0 : 0x50));
            pw.port1 = 0xff;
            pw.mixer = 0;
            pw.noise = 0;
        }

        public override void InitChip()
        {

            if (!use) return;

            //FM Off
            outYM2413AllKeyOff(lstPartWork[0]);

            if (ChipID != 0) parent.dat[0x13] |= 0x40;//use Secondary(YM2413 OPLL)

        }


        public void outYM2413SetAdr00_01(partWork pw, byte adr, bool AM, bool VIB, bool EG, bool KS, int mul)
        {
            parent.OutData(
                pw.port0
                , (byte)(adr & 1)
                , (byte)((AM ? 0x80 : 0) + (VIB ? 0x40 : 0) + (EG ? 0x20 : 0) + (KS ? 0x10 : 0) + (mul & 0xf))
                );
        }

        public void outYM2413AllKeyOff(partWork pw)
        {
            //Rhythm Off
            parent.OutData(pw.port0, 0x0e, 0);
            for (byte adr = 0; adr < 9; adr++)
            {
                //Ch Off
                parent.OutData(pw.port0, (byte)(0x20 + adr), 0);
                parent.OutData(pw.port0, (byte)(0x30 + adr), 0);
            }
        }

        public void outYM2413SetInstrument(partWork pw, int n)
        {
            pw.instrument = n;

            if (!parent.instFM.ContainsKey(n))
            {
                msgBox.setWrnMsg(string.Format("未定義の音色(@{0})を指定しています。", n), pw.getSrcFn(), pw.getLineNumber());
                return;
            }

            for (byte ope = 0; ope < 2; ope++)
            {
                outYM2413SetAdr00_01(pw, ope
                    , parent.instFM[n][ope * 11 + 7] != 0 //AM
                    , parent.instFM[n][ope * 11 + 8] != 0 //VIB
                    , parent.instFM[n][ope * 11 + 9] != 0 //EG
                    , parent.instFM[n][ope * 11 + 10] != 0 //KS
                    , parent.instFM[n][ope * 11 + 6] & 0xf //MT
                    );
                parent.OutData(pw.port0, (byte)(0x4 + ope), (byte)((
                    (parent.instFM[n][ope * 11 + 1] & 0xf) << 4) //AR
                    | (parent.instFM[n][ope * 11 + 2] & 0xf) // DR
                    ));
                parent.OutData(pw.port0, (byte)(0x6 + ope), (byte)((
                    (parent.instFM[n][ope * 11 + 3] & 0xf) << 4) //SL
                    | (parent.instFM[n][ope * 11 + 4] & 0xf) // RR
                    ));
            }
            parent.OutData(pw.port0, (byte)(0x2), (byte)((
                (parent.instFM[n][0 * 11 + 5] & 0x3) << 6)  //KL(M)
                | (parent.instFM[n][23] & 0x3f) //TL
                ));
            parent.OutData(pw.port0, (byte)(0x3), (byte)((
                (parent.instFM[n][1 * 11 + 5] & 0x3) << 6) //KL(C)
                | (parent.instFM[n][0 * 11 + 11] != 0 ? 0x08 : 0) // DT(M)
                | (parent.instFM[n][1 * 11 + 11] != 0 ? 0x10 : 0) // DT(C)
                | (parent.instFM[n][24] & 0x07) //FB
                ));

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

        public void SetFmFNum(partWork pw)
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
            if (n >= 0)
            {
                o += n / 12;
                o = Common.CheckRange(o, 1, 8);
                n %= 12;
            }
            else
            {
                o += n / 12 - ((n % 12 == 0) ? 0 : 1);
                o = Common.CheckRange(o, 1, 8);
                n %= 12;
                if (n < 0) { n += 12; }
            }

            int f = ftbl[n];

            return (f & 0xfff) + (o & 0xf) * 0x1000;
        }

        public void SetFmVolume(partWork pw)
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

        public override void SetFNum(partWork pw)
        {
            SetFmFNum(pw);
        }

        public override void SetVolume(partWork pw)
        {
            SetFmVolume(pw);
        }

        public override void SetKeyOn(partWork pw)
        {
            pw.keyOn = true;
        }

        public override void SetKeyOff(partWork pw)
        {
            pw.keyOn = false;
            pw.keyOff = true;
        }

        public override void SetLfoAtKeyOn(partWork pw)
        {
        }

        public override void CmdInstrument(partWork pw, MML mml)
        {
            char type = (char)mml.args[0];
            int n = (int)mml.args[1];

            if (type == 'T')
            {
                msgBox.setErrMsg("Tone DoublerはOPN,OPM音源以外では使用できません。", pw.getSrcFn(), pw.getLineNumber());
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
                return;
            }

            n = Common.CheckRange(n, 0, 255);
            if (pw.instrument == n) return;

            outYM2413SetInstrument(pw, n); //音色のセット
            pw.envInstrument = 0;
            //outYM2413SetInstVol(pw, 0, pw.volume); //INSTを0にセット

        }

        public override void SetPCMDataBlock()
        {
            //実装不要
        }

        public override void SetToneDoubler(partWork pw)
        {
            //実装不要
        }

        public override void MultiChannelCommand()
        {
            foreach (partWork pw in lstPartWork)
            {
                if (pw.Type != enmChannelType.FMOPL) continue;

                if (pw.beforeEnvInstrument != pw.envInstrument || pw.beforeVolume != pw.volume)
                {
                    pw.beforeEnvInstrument = pw.envInstrument;
                    pw.beforeVolume = pw.volume;

                    parent.OutData(pw.port0
                        , (byte)(0x30 + pw.ch)
                        , (byte)(((pw.envInstrument << 4) & 0xf0) | ((15 - pw.volume) & 0xf))
                        );
                }


                if (pw.keyOff)
                {
                    pw.keyOff = false;
                    parent.OutData(pw.port0
                        , (byte)(0x20 + pw.ch)
                        , (byte)(
                            ((pw.freq >> 8) & 0xf)
                          )
                        );
                }

                if (pw.beforeFNum != (pw.freq | (pw.keyOn ? 0x1000 : 0x0000)))
                {
                    pw.beforeFNum = pw.freq | (pw.keyOn ? 0x1000 : 0x0000);

                    parent.OutData(pw.port0, (byte)(0x10 + pw.ch), (byte)pw.freq);
                    parent.OutData(pw.port0
                        , (byte)(0x20 + pw.ch)
                        , (byte)(
                            ((pw.freq >> 8) & 0xf)
                            | (pw.keyOn ? 0x10 : 0x00)
                          )
                        );
                }
            }
        }

    }
}
