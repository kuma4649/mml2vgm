using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class YM2151 : ClsChip
    {
        public YM2151(ClsVgm parent, int chipID, string initialPartName, string stPath, bool isSecondary) : base(parent, chipID, initialPartName, stPath, isSecondary)
        {

            _Name = "YM2151";
            _ShortName = "OPM";
            _ChMax = 8;
            _canUsePcm = false;

            Frequency = 3579545;

            MakeFNumTbl();
            Ch = new ClsChannel[ChMax];
            SetPartToCh(Ch, initialPartName);
            foreach (ClsChannel ch in Ch)
            {
                ch.Type = enmChannelType.FMOPM;
                ch.isSecondary = chipID == 1;
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
                if (pw.ch == 0)
                {
                    pw.hardLfoFreq = 0;
                    pw.hardLfoPMD = 0;
                    pw.hardLfoAMD = 0;

                    //Reset Hard LFO
                    OutSetHardLfoFreq(pw, pw.hardLfoFreq);
                    OutSetHardLfoDepth(pw, false, pw.hardLfoAMD);
                    OutSetHardLfoDepth(pw, true, pw.hardLfoPMD);
                }

                pw.ams = 0;
                pw.pms = 0;
                if (!pw.dataEnd) OutSetPMSAMS(pw, 0, 0);

            }

            if (IsSecondary) parent.dat[0x33] |= 0x40;//use Secondary
        }

        public override void InitPart(ref partWork pw)
        {
            pw.slots = 0xf;
            pw.volume = 127;
            pw.MaxVolume = 127;
            pw.port0 = (byte)(0x4 | (pw.isSecondary ? 0xa0 : 0x50));
            pw.port1 = 0xff;
            pw.mixer = 0;
            pw.noise = 0;
        }


        public void OutSetFnum(partWork pw, int octave, int note, int kf)
        {
            octave &= 0x7;
            note &= 0xf;
            note = note < 3 ? note : (note < 6 ? (note + 1) : (note < 9 ? (note + 2) : (note + 3)));
            parent.OutData(pw.port0, (byte)(0x28 + pw.ch), (byte)((octave << 4) | note));
            parent.OutData(pw.port0, (byte)(0x30 + pw.ch), (byte)(kf << 2));
        }

        public void OutSetVolume(partWork pw, int vol, int n)
        {
            if (!parent.instFM.ContainsKey(n))
            {
                msgBox.setWrnMsg(string.Format("未定義の音色(@{0})を指定している場合ボリュームの変更はできません。", n), pw.getSrcFn(), pw.getLineNumber());
                return;
            }

            int alg = parent.instFM[n][45] & 0x7;
            int[] ope = new int[4] {
                parent.instFM[n][0*Const.INSTRUMENT_M_OPERATOR_SIZE + 6]
                , parent.instFM[n][1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 6]
                , parent.instFM[n][2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 6]
                , parent.instFM[n][3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 6]
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
            //    if (algs[alg][i] == 1 && (pw.slots & (1 << i)) != 0)
            //    {
            //        minV = Math.Min(minV, ope[i]);
            //    }
            //}

            for (int i = 0; i < 4; i++)
            {
                if (algs[alg][i] == 0 || (pw.slots & (1 << i)) == 0)
                {
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

            if ((pw.slots & 1) != 0) OutSetTl(pw, 0, ope[0]);
            if ((pw.slots & 2) != 0) OutSetTl(pw, 1, ope[1]);
            if ((pw.slots & 4) != 0) OutSetTl(pw, 2, ope[2]);
            if ((pw.slots & 8) != 0) OutSetTl(pw, 3, ope[3]);
        }

        public void OutSetTl(partWork pw, int ope, int tl)
        {
            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            tl &= 0x7f;

            parent.OutData(
                pw.port0
                , (byte)(0x60 + pw.ch + ope * 8)
                , (byte)tl
                );
        }

        public void OutSetHardLfoFreq(partWork pw, int freq)
        {
            parent.OutData(
                pw.port0
                , 0x18
                , (byte)(freq & 0xff)
                );
        }

        public void OutSetHardLfoDepth(partWork pw, bool isPMD, int depth)
        {
            parent.OutData(
                pw.port0
                , 0x19
                , (byte)((isPMD ? 0x80 : 0x00) | (depth & 0x7f))
                );
        }

        public void OutSetPMSAMS(partWork pw, int PMS, int AMS)
        {
            parent.OutData(
                pw.port0
                , (byte)(0x38 + pw.ch)
                , (byte)(((PMS & 0x7) << 4) | (AMS & 0x3))
                );
        }

        public void OutSetPanFeedbackAlgorithm(partWork pw, int pan, int fb, int alg)
        {
            pan &= 3;
            fb &= 7;
            alg &= 7;

            parent.OutData(pw.port0, (byte)(0x20 + pw.ch), (byte)((pan << 6) | (fb << 3) | alg));
        }

        public void OutSetDtMl(partWork pw, int ope, int dt, int ml)
        {
            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            dt &= 7;
            ml &= 15;

            parent.OutData(pw.port0, (byte)(0x40 + pw.ch + ope * 8), (byte)((dt << 4) | ml));
        }

        public void OutSetKsAr(partWork pw, int ope, int ks, int ar)
        {
            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            ks &= 3;
            ar &= 31;

            parent.OutData(pw.port0, (byte)(0x80 + pw.ch + ope * 8), (byte)((ks << 6) | ar));
        }

        public void OutSetAmDr(partWork pw, int ope, int am, int dr)
        {
            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            am &= 1;
            dr &= 31;

            parent.OutData(pw.port0, (byte)(0xa0 + pw.ch + ope * 8), (byte)((am << 7) | dr));
        }

        public void OutSetDt2Sr(partWork pw, int ope, int dt2, int sr)
        {
            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            dt2 &= 3;
            sr &= 31;

            parent.OutData(pw.port0, (byte)(0xc0 + pw.ch + ope * 8), (byte)((dt2 << 6) | sr));
        }

        public void OutSetSlRr(partWork pw, int ope, int sl, int rr)
        {
            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            sl &= 15;
            rr &= 15;

            parent.OutData(pw.port0, (byte)(0xe0 + pw.ch + ope * 8), (byte)((sl << 4) | rr));
        }

        public void OutSetHardLfo(partWork pw, bool sw, List<int> param)
        {
            if (sw)
            {
                parent.OutData(pw.port0, 0x1b, (byte)(param[0] & 0x3));//type
                parent.OutData(pw.port0, 0x18, (byte)(param[1] & 0xff));//LFRQ
                parent.OutData(pw.port0, 0x19, (byte)((param[2] & 0x7f) | 0x80));//PMD
                parent.OutData(pw.port0, 0x19, (byte)((param[3] & 0x7f) | 0x00));//AMD
            }
            else
            {
                parent.OutData(pw.port0, 0x1b, 0);//type
                parent.OutData(pw.port0, 0x18, 0);//LFRQ
                parent.OutData(pw.port0, 0x19, 0x80);//PMD
                parent.OutData(pw.port0, 0x19, 0x00);//AMD
            }
        }

        public void OutSetInstrument(partWork pw, int n, int vol)
        {

            if (!parent.instFM.ContainsKey(n))
            {
                msgBox.setWrnMsg(string.Format("未定義の音色(@{0})を指定しています。", n), pw.getSrcFn(), pw.getLineNumber());
                return;
            }

            for (int ope = 0; ope < 4; ope++)
            {

                OutSetDtMl(pw, ope, parent.instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], parent.instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 8]);
                OutSetKsAr(pw, ope, parent.instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 7], parent.instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 1]);
                OutSetAmDr(pw, ope, parent.instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 11], parent.instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 2]);
                OutSetDt2Sr(pw, ope, parent.instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 10], parent.instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 3]);
                OutSetSlRr(pw, ope, parent.instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 5], parent.instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 4]);

            }
            pw.op1ml = parent.instFM[n][0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
            pw.op2ml = parent.instFM[n][1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
            pw.op3ml = parent.instFM[n][2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
            pw.op4ml = parent.instFM[n][3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
            pw.op1dt2 = parent.instFM[n][0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 10];
            pw.op2dt2 = parent.instFM[n][1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 10];
            pw.op3dt2 = parent.instFM[n][2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 10];
            pw.op4dt2 = parent.instFM[n][3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 10];

            OutSetPanFeedbackAlgorithm(pw, (int)pw.pan.val, parent.instFM[n][46], parent.instFM[n][45]);
            ((YM2151)pw.chip).OutSetVolume(pw, vol, n);

        }

        public void OutKeyOn(partWork pw)
        {

            if (pw.ch == 7 && pw.mixer == 1)
            {
                parent.OutData(pw.port0, 0x0f, (byte)((pw.mixer << 7) | (pw.noise & 0x1f)));
            }
            //key on
            parent.OutData(pw.port0, 0x08, (byte)((pw.slots << 3) + pw.ch));
        }

        public void OutKeyOff(partWork pw)
        {

            //key off
            parent.OutData(pw.port0, 0x08, (byte)(0x00 + (pw.ch & 7)));
            if (pw.ch == 7 && pw.mixer == 1)
            {
                parent.OutData(pw.port0, 0x0f, 0x00);
            }

        }

        public void OutAllKeyOff()
        {

            foreach (partWork pw in lstPartWork)
            {
                if (pw.dataEnd) continue;

                OutKeyOff(pw);
                OutSetTl(pw, 0, 127);
                OutSetTl(pw, 1, 127);
                OutSetTl(pw, 2, 127);
                OutSetTl(pw, 3, 127);
            }

        }


        public override void SetFNum(partWork pw)
        {

            int f = GetFNum(pw,pw.octaveNow, pw.noteCmd, pw.shift + pw.keyShift + pw.toneDoublerKeyShift);//

            if (pw.bendWaitCounter != -1)
            {
                f = pw.bendFnum;
            }

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

            f = Common.CheckRange(f, 0, 9 * 12 * 64 - 1);
            int oct = f / (12 * 64);
            int note = (f - oct * 12 * 64) / 64;
            int kf = f - oct * 12 * 64 - note * 64;

            OutSetFnum(pw, oct, note, kf);
        }

        public override int GetFNum(partWork pw,int octave, char noteCmd, int shift)
        {
            int o = octave;
            int n = Const.NOTE.IndexOf(noteCmd) + shift - 1;

            if (n >= 0)
            {
                o += n / 12;
                o = Common.CheckRange(o, 1, 8);
                n %= 12;
            }
            else
            {
                o += n / 12 - ((n % 12 == 0) ? 0 : 1);
                if (o == 0 && n < 0)
                {
                    o = 1;
                    n = 0;
                }
                else
                {
                    o = Common.CheckRange(o, 1, 8);
                    n %= 12;
                    if (n < 0) { n += 12; }
                }
            }
            o--;

            return n * 64 + o * 12 * 64;
        }

        public override void SetVolume(partWork pw)
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

            if (pw.beforeVolume != vol)
            {
                if (parent.instFM.ContainsKey(pw.instrument))
                {
                    OutSetVolume(pw, vol, pw.instrument);
                    pw.beforeVolume = vol;
                }
            }
        }

        public override void SetKeyOff(partWork pw)
        {
            OutKeyOff(pw);
        }

        public override void SetLfoAtKeyOn(partWork pw)
        {
            for (int lfo = 0; lfo < 4; lfo++)
            {
                clsLfo pl = pw.lfo[lfo];

                if (!pl.sw)
                    continue;
                if (pl.type == eLfoType.Hardware)
                    continue;
                if (pl.param[5] != 1)
                    continue;

                pl.isEnd = false;
                pl.value = (pl.param[0] == 0) ? pl.param[6] : 0;//ディレイ中は振幅補正は適用されない
                pl.waitCounter = pl.param[0];
                pl.direction = pl.param[2] < 0 ? -1 : 1;

                if (pl.type == eLfoType.Vibrato)
                    SetFNum(pw);

                if (pl.type == eLfoType.Tremolo)
                    SetVolume(pw);

            }
        }

        public override void SetPCMDataBlock()
        {
            //実装不要
        }


        public override void CmdNoiseToneMixer(partWork pw, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 0, 1);
            pw.mixer = n;
        }

        public override void CmdNoise(partWork pw, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 0, 31);
            if (pw.noise != n)
            {
                pw.noise = n;
            }
        }

        public override void CmdMPMS(partWork pw, MML mml)
        {
            int n = (int)mml.args[1];
            n = Common.CheckRange(n, 0, 7);
            pw.pms = n;
            ((YM2151)pw.chip).OutSetPMSAMS(pw, pw.pms, pw.ams);
        }

        public override void CmdMAMS(partWork pw, MML mml)
        {
            int n = (int)mml.args[1];
            n = Common.CheckRange(n, 0, 3);
            pw.ams = n;
            ((YM2151)pw.chip).OutSetPMSAMS(pw, pw.pms, pw.ams);
        }

        public override void CmdLfo(partWork pw, MML mml)
        {
            base.CmdLfo(pw, mml);

            int c = (char)mml.args[0] - 'P';
            if (pw.lfo[c].type == eLfoType.Hardware)
            {
                if (pw.lfo[c].param.Count < 4)
                {
                    msgBox.setErrMsg("LFOの設定に必要なパラメータが足りません。", pw.getSrcFn(), pw.getLineNumber());
                    return;
                }
                if (pw.lfo[c].param.Count > 5)
                {
                    msgBox.setErrMsg("LFOの設定に可能なパラメータ数を超えて指定されました。", pw.getSrcFn(), pw.getLineNumber());
                    return;
                }

                pw.lfo[c].param[0] = Common.CheckRange(pw.lfo[c].param[0], 0, 3); //Type
                pw.lfo[c].param[1] = Common.CheckRange(pw.lfo[c].param[1], 0, 255); //LFRQ
                pw.lfo[c].param[2] = Common.CheckRange(pw.lfo[c].param[2], 0, 127); //PMD
                pw.lfo[c].param[3] = Common.CheckRange(pw.lfo[c].param[3], 0, 127); //AMD
                if (pw.lfo[c].param.Count == 5)
                {
                    pw.lfo[c].param[4] = Common.CheckRange(pw.lfo[c].param[4], 0, 1);
                }
                else
                {
                    pw.lfo[c].param.Add(0);
                }
            }
        }

        public override void CmdLfoSwitch(partWork pw, MML mml)
        {
            base.CmdLfo(pw, mml);

            int c = (char)mml.args[0] - 'P';
            int n = (int)mml.args[1];
            if (pw.lfo[c].type == eLfoType.Hardware)
            {
                ((YM2151)pw.chip).OutSetHardLfo(pw, (n == 0) ? false : true, pw.lfo[c].param);
            }
        }

        public override void CmdPan(partWork pw, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 1, 3);
            pw.pan.val = (n == 1) ? 2 : (n == 2 ? 1 : n);
            if (pw.instrument < 0)
            {
                msgBox.setErrMsg("音色指定前にパンは指定できません。"
                    , pw.getSrcFn(), pw.getLineNumber());
            }
            else
            {
                ((YM2151)pw.chip).OutSetPanFeedbackAlgorithm(
                    pw
                    , (int)pw.pan.val
                    , parent.instFM[pw.instrument][46]
                    , parent.instFM[pw.instrument][45]
                    );
            }
        }

        public override void CmdInstrument(partWork pw, MML mml)
        {
            char type = (char)mml.args[0];
            int n = (int)mml.args[1];

            if (type == 'I')
            {
                msgBox.setErrMsg("この音源はInstrumentを持っていません。", pw.getSrcFn(), pw.getLineNumber());
                return;
            }

            if (type == 'T')
            {
                n = Common.CheckRange(n, 0, 255);
                pw.toneDoubler = n;
                return;
            }

            if (type == 'E')
            {
                SetEnvelopParamFromInstrument(pw, n,mml);
                return;
            }

            n = Common.CheckRange(n, 0, 255);
            if (pw.instrument == n) return;

            pw.instrument = n;
            OutSetInstrument(pw, n, pw.volume);
        }

        public override void CmdY(partWork pw, MML mml)
        {
            if (mml.args[0] is string toneparamName)
            {
                byte op = (byte)mml.args[1];
                op = (byte)(op == 1 ? 2 : (op == 2 ? 1 : op));
                byte dat = (byte)mml.args[2];

                switch (toneparamName)
                {
                    case "PANFBAL":
                    case "PANFLCON":
                        parent.OutData(pw.port0, (byte)(0x20 + pw.ch), dat);
                        break;
                    case "PMSAMS":
                        parent.OutData(pw.port0, (byte)(0x38 + pw.ch), dat);
                        break;
                    case "DTML":
                    case "DTMUL":
                    case "DT1ML":
                    case "DT1MUL":
                        parent.OutData(pw.port0, (byte)(0x40 + pw.ch + op * 8), dat);
                        break;
                    case "TL":
                        parent.OutData(pw.port0, (byte)(0x60 + pw.ch + op * 8), dat);
                        break;
                    case "KSAR":
                        parent.OutData(pw.port0, (byte)(0x80 + pw.ch + op * 8), dat);
                        break;
                    case "AMDR":
                    case "AMED1R":
                        parent.OutData(pw.port0, (byte)(0xa0 + pw.ch + op * 8), dat);
                        break;
                    case "DT2SR":
                    case "DT2D2R":
                        parent.OutData(pw.port0, (byte)(0xc0 + pw.ch + op * 8), dat);
                        break;
                    case "SLRR":
                    case "D1LRR":
                        parent.OutData(pw.port0, (byte)(0xe0 + pw.ch + op * 8), dat);
                        break;
                }
            }
            else
            {
                byte adr = (byte)mml.args[0];
                byte dat = (byte)mml.args[1];
                parent.OutData(pw.port0, adr, dat);
            }
        }

        public override void CmdLoopExtProc(partWork pw, MML mml)
        {
        }

    }
}
