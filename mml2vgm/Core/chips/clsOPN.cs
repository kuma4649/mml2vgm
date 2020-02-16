using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class ClsOPN : ClsChip
    {
        public byte[] SSGKeyOn = new byte[] { 0x3f, 0x3f, 0x3f, 0x3f };


        public ClsOPN(ClsVgm parent, int chipID, string initialPartName, string stPath, int chipNumber) : base(parent, chipID, initialPartName, stPath, chipNumber)
        {
        }


        public void OutSsgKeyOn(partWork pw,MML mml)
        {
            int port;
            int adr;
            int vch;
            GetPortVchSsg(pw, out port, out adr, out vch);
            int p = port;
            if (adr == 0x10)
                p++;

            int n = (pw.mixer & 0x1) + ((pw.mixer & 0x2) << 2);
            byte data = 0;

            data = (byte)(((ClsOPN)pw.chip).SSGKeyOn[p] | (9 << vch));
            data &= (byte)(~(n << vch));
            ((ClsOPN)pw.chip).SSGKeyOn[p] = data;

            SetSsgVolume(pw, mml);
            if (pw.HardEnvelopeSw)
            {
                parent.OutData(mml, pw.port[port], (byte)(adr + 0x0d), (byte)(pw.HardEnvelopeType & 0xf));
            }
            parent.OutData(mml, pw.port[port], (byte)(adr + 0x07), data);

            MML vmml = new MML();
            vmml.type = enmMMLType.Volume;
            vmml.args = new List<object>();
            vmml.args.Add(pw.volume);
            vmml.line = mml.line;
            SetDummyData(pw, vmml);

        }

        public void OutSsgKeyOff(MML mml, partWork pw)
        {
            int port;
            int adr;
            int vch;
            GetPortVchSsg(pw, out port, out adr, out vch);
            int p = port;
            if (adr == 0x10)
                p++;

            int n = 9;
            byte data = 0;

            data = (byte)(((ClsOPN)pw.chip).SSGKeyOn[p] | (n << vch));
            ((ClsOPN)pw.chip).SSGKeyOn[p] = data;

            parent.OutData(mml, pw.port[port], (byte)(adr + 0x08 + vch), 0);
            pw.beforeVolume = -1;
            parent.OutData(mml, pw.port[port], (byte)(adr + 0x07), data);

        }

        public virtual void SetSsgVolume(partWork pw, MML mml)
        {
            int port;
            int adr;
            int vch;
            GetPortVchSsg(pw, out port, out adr, out vch);
            int p = port;
            if (adr == 0x10)
                p++;

            int vol = pw.volume;
            if (pw.envelopeMode)
            {
                vol = 0;
                if (pw.envIndex != -1)
                {
                    vol = pw.volume - (15 - pw.envVolume);
                }
            }

            for (int lfo = 0; lfo < 4; lfo++)
            {
                if (!pw.lfo[lfo].sw) continue;
                if (pw.lfo[lfo].type != eLfoType.Tremolo) continue;

                vol += pw.lfo[lfo].value + pw.lfo[lfo].param[6];
            }

            vol = Common.CheckRange(vol, 0, 15) + (pw.HardEnvelopeSw ? 0x10 : 0x00);
            if ((((ClsOPN)pw.chip).SSGKeyOn[p] & (9 << vch)) == (9 << vch))
            {
                vol = 0;
            }

            if (pw.chip is YM2609)
            {
                int pan = (int)(pw.pan.val == null ? 0 : pw.pan.val);
                vol |= (byte)(pan << 6);
            }

            if (pw.beforeVolume != vol)
            {
                parent.OutData(mml, pw.port[port], (byte)(adr + 0x08 + vch), (byte)vol);
                pw.beforeVolume = vol;
            }
        }

        public void OutSsgNoise(MML mml,partWork pw, int n)
        {
            int port;
            int adr;
            int vch;//ノイズ設定はch未使用
            GetPortVchSsg(pw, out port, out adr, out vch);

            parent.OutData(mml, pw.port[port], (byte)(adr + 0x06), (byte)(n & 0x1f));
        }

        public void SetSsgFNum(partWork pw,MML mml)
        {
            int f = - pw.detune;
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
                f -= pw.lfo[lfo].value + pw.lfo[lfo].param[6];
            }

            if (pw.octaveNow < 1)
            {
                f <<= -pw.octaveNow;
            }
            else
            {
                f >>= pw.octaveNow - 1;
            }

            if (pw.bendWaitCounter != -1)
            {
                f += pw.bendFnum;
            }
            else
            {
                f += GetSsgFNum(pw, mml, pw.octaveNow, pw.noteCmd, pw.shift + pw.keyShift);//
            }

            f = Common.CheckRange(f, 0, 0xfff);
            if (pw.freq == f) return;

            pw.freq = f;

            int port;
            int adr;
            int vch;
            GetPortVchSsg(pw, out port, out adr, out vch);

            byte data = 0;

            data = (byte)(f & 0xff);
            parent.OutData(mml,pw.port[port], (byte)(adr + 0 + vch * 2), data);

            data = (byte)((f & 0xf00) >> 8);
            parent.OutData(mml,pw.port[port], (byte)(adr + 1 + vch * 2), data);
        }

        public int GetSsgFNum(partWork pw,MML mml, int octave, char noteCmd, int shift)
        {
            int o = octave - 1;
            int n = Const.NOTE.IndexOf(noteCmd) + shift;
            o += n / 12;
            o = Common.CheckRange(o, 0, 7);
            n %= 12;

            int f = o * 12 + n;
            if (f < 0) f = 0;
            if (f >= pw.chip.FNumTbl[1].Length) f = pw.chip.FNumTbl[1].Length - 1;

            return pw.chip.FNumTbl[1][f];
        }


        public void OutOPNSetPanAMSPMS(MML mml, partWork pw, int pan, int ams, int pms)
        {
            //TODO: 効果音パートで指定されている場合の考慮不足
            int vch;
            byte[] port;
            GetPortVch(pw, out port, out vch);

            pan = pan & 3;
            ams = ams & 3;
            pms = pms & 7;

            parent.OutData(mml, port, (byte)(0xb4 + vch), (byte)((pan << 6) + (ams << 4) + pms));
        }

        public void OutOPNSetHardLfo(MML mml,partWork pw, bool sw, int lfoNum)
        {
            parent.OutData(
                mml,
                pw.port[0]
                , 0x22
                , (byte)((lfoNum & 7) + (sw ? 8 : 0))
                );
        }

        public void OutOPNSetCh3SpecialMode(MML mml,partWork pw, bool sw)
        {
            byte[] port = pw.port[0];
            if(pw.chip.chipType== enmChipType.YM2609)
            {
                if (pw.ch == 8 || pw.ch == 15 || pw.ch == 16 || pw.ch == 17)
                {
                    port = pw.port[2];
                }
            }
            // ignore Timer ^^;
            parent.OutData(
                mml,
                port
                , 0x27
                , (byte)((sw ? 0x40 : 0))
                );
        }

        public void OutFmSetFeedbackAlgorithm(MML mml, partWork pw, int fb, int alg)
        {
            int vch;
            byte[] port;
            GetPortVch(pw, out port, out vch);

            fb &= 7;
            alg &= 7;

            parent.OutData(mml, port, (byte)(0xb0 + vch), (byte)((fb << 3) + alg));
        }

        public void OutFmSetDtMl(MML mml, partWork pw, int ope, int dt, int ml)
        {
            int vch;
            byte[] port;
            GetPortVch(pw, out port, out vch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            dt &= 7;
            ml &= 15;

            parent.OutData(mml, port, (byte)(0x30 + vch + ope * 4), (byte)((dt << 4) + ml));
        }

        public void OutFmSetTl(MML mml,partWork pw, int ope, int tl)
        {
            int vch;
            byte[] port;
            GetPortVch(pw, out port, out vch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            tl &= 0x7f;

            parent.OutData(mml,port, (byte)(0x40 + vch + ope * 4), (byte)tl);
        }

        public void OutFmSetKsAr(MML mml,partWork pw, int ope, int ks, int ar)
        {
            int vch;
            byte[] port;
            GetPortVch(pw, out port, out vch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            ks &= 3;
            ar &= 31;

            parent.OutData(mml,port, (byte)(0x50 + vch + ope * 4), (byte)((ks << 6) + ar));
        }

        public void OutFmSetAmDr(MML mml,partWork pw, int ope, int am, int dr)
        {
            int vch;
            byte[] port;
            GetPortVch(pw, out port, out vch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            am &= 1;
            dr &= 31;

            parent.OutData(mml,port, (byte)(0x60 + vch + ope * 4), (byte)((am << 7) + dr));
        }

        public void OutFmSetSr(MML mml,partWork pw, int ope, int sr)
        {
            int vch;
            byte[] port;
            GetPortVch(pw, out port, out vch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            sr &= 31;

            parent.OutData(mml,port, (byte)(0x70 + vch + ope * 4), (byte)(sr));
        }

        public void OutFmSetSlRr(MML mml, partWork pw, int ope, int sl, int rr)
        {
            int vch;
            byte[] port;
            GetPortVch(pw, out port, out vch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            sl &= 15;
            rr &= 15;

            parent.OutData(mml, port, (byte)(0x80 + vch + ope * 4), (byte)((sl << 4) + rr));
        }

        protected void GetPortVch(partWork pw, out byte[] port, out int vch)
        {
            if (!(pw.chip is YM2609))
            {
                port = pw.ch > 2 ? pw.port[1] : pw.port[0];
                vch = (byte)(pw.ch > 2 ? pw.ch - 3 : pw.ch);
            }
            else
            {
                port = 
                    pw.ch < 3 ? 
                    pw.port[0] :
                    (pw.ch < 6 ? 
                        pw.port[1] : 
                        (pw.ch < 9 ? 
                            pw.port[2] :
                            (pw.ch < 12 ?
                                pw.port[3] :
                                (pw.ch < 15 ?
                                    pw.port[0] :
                                    pw.port[2]
                                )
                            )
                        )
                    );
                vch = (byte)(
                    pw.ch < 3 ? 
                    pw.ch : 
                    (pw.ch < 6 ?
                        (pw.ch - 3) : 
                        (pw.ch < 9 ?
                            (pw.ch - 6) :
                            (pw.ch < 12 ?
                                (pw.ch - 9) :
                                (pw.ch < 15 ?
                                    2 :
                                    2
                                )
                            )
                        )
                    )
                );
            }
        }

        protected void GetPortVchSsg(partWork pw, out int port, out int adr, out int vch)
        {
            int m = (pw.chip is YM2203) ? 0 : 3;
            vch = (byte)(pw.ch - (m + 6));
            port = 0;
            adr = 0;

            if (!(pw.chip is YM2609)) return;

            if (pw.ch >= 18 && pw.ch <= 20)
            {
                port = 0;
                vch = (byte)(pw.ch - 18);
                adr = 0;
            }
            else if (pw.ch >= 21 && pw.ch <= 23)
            {
                port = 1;
                vch = (byte)(pw.ch - 21);
                adr = 0x20;
            }
            else if (pw.ch >= 24 && pw.ch <= 26)
            {
                port = 2;
                vch = (byte)(pw.ch - 24);
                adr = 0;
            }
            else if (pw.ch >= 27 && pw.ch <= 29)
            {
                port = 2;
                vch = (byte)(pw.ch - 27);
                adr = 0x10;
            }
        }

        public void OutFmSetSSGEG(MML mml,partWork pw, int ope, int n)
        {
            int vch;
            byte[] port;
            GetPortVch(pw, out port, out vch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            n &= 15;

            parent.OutData(mml,port, (byte)(0x90 + vch + ope * 4), (byte)n);
        }

        /// <summary>
        /// FMボリュームの設定
        /// </summary>
        /// <param name="ch">チャンネル</param>
        /// <param name="vol">ボリューム値</param>
        /// <param name="n">音色番号</param>
        public void OutFmSetVolume(partWork pw, MML mml, int vol, int n)
        {
            if (!parent.instFM.ContainsKey(n))
            {
                msgBox.setWrnMsg(string.Format(msg.get("E11000"), n), mml.line.Lp);
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

            for (int i = 0; i < 4; i++)
            {
                if (algs[alg][i] == 0 || (pw.slots & (1 << i)) == 0)
                {
                    ope[i] = -1;
                    continue;
                }
                ope[i] = ope[i] + (127 - vol);
                ope[i] = Common.CheckRange(ope[i], 0, 127);
            }

            partWork vpw = pw;
            if (!(pw.chip is YM2609))
            {
                int m = (pw.chip is YM2203) ? 0 : 3;
                if (pw.chip.lstPartWork[2].Ch3SpecialMode && pw.ch >= m + 3 && pw.ch < m + 6)
                {
                    vpw = pw.chip.lstPartWork[2];
                }
            }
            else
            {
                if (pw.chip.lstPartWork[2].Ch3SpecialMode && pw.ch >= 12 && pw.ch < 15)
                {
                    vpw = pw.chip.lstPartWork[2];
                }
                if (pw.chip.lstPartWork[8].Ch3SpecialMode && pw.ch >= 15 && pw.ch < 18)
                {
                    vpw = pw.chip.lstPartWork[8];
                }
            }

            MML vmml = new MML();
            vmml.args = new List<object>();
            vmml.args.Add(vol);
            vmml.type = enmMMLType.Volume;
            if (mml != null)
                vmml.line = mml.line;
            if ((pw.slots & 1) != 0 && ope[0] != -1) ((ClsOPN)pw.chip).OutFmSetTl(vmml, vpw, 0, ope[0]);
            if ((pw.slots & 2) != 0 && ope[1] != -1) ((ClsOPN)pw.chip).OutFmSetTl(vmml, vpw, 1, ope[1]);
            if ((pw.slots & 4) != 0 && ope[2] != -1) ((ClsOPN)pw.chip).OutFmSetTl(vmml, vpw, 2, ope[2]);
            if ((pw.slots & 8) != 0 && ope[3] != -1) ((ClsOPN)pw.chip).OutFmSetTl(vmml, vpw, 3, ope[3]);
            //if ((pw.slots & 1) != 0 ) ((ClsOPN)pw.chip).OutFmSetTl(vpw, 0, ope[0]);
            //if ((pw.slots & 2) != 0 ) ((ClsOPN)pw.chip).OutFmSetTl(vpw, 1, ope[1]);
            //if ((pw.slots & 4) != 0 ) ((ClsOPN)pw.chip).OutFmSetTl(vpw, 2, ope[2]);
            //if ((pw.slots & 8) != 0 ) ((ClsOPN)pw.chip).OutFmSetTl(vpw, 3, ope[3]);
        }

        public void OutFmCh3SpecialModeSetFnum(MML mml,partWork pw, byte ope, int octave, int num)
        {
            ope &= 3;
            if (ope == 0)
            {
                parent.OutData(mml,pw.port[0], 0xa6, (byte)(((num & 0x700) >> 8) + (((octave - 1) & 0x7) << 3)));
                parent.OutData(mml,pw.port[0], 0xa2, (byte)(num & 0xff));
            }
            else
            {
                parent.OutData(mml,pw.port[0], (byte)(0xac + ope), (byte)(((num & 0x700) >> 8) + (((octave - 1) & 0x7) << 3)));
                parent.OutData(mml,pw.port[0], (byte)(0xa8 + ope), (byte)(num & 0xff));
            }
        }

        public void OutFmSetInstrument(partWork pw, MML mml, int n, int vol, char typeBeforeSend)
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

            int m = (pw.chip is YM2203) ? 0 : 3;

            if (
                ((pw.chip is YM2203) && pw.ch >= 3 && pw.ch < 6)
                || ((pw.chip is YM2608) && pw.ch >= 6 && pw.ch < 9)
                || ((pw.chip is YM2609) && pw.ch >= 12 && pw.ch < 18)
                || ((pw.chip is YM2610B) && pw.ch >= 6 && pw.ch < 9)
                || ((pw.chip is YM2612) && pw.ch >= 6 && pw.ch < 9)
                || ((pw.chip is YM2612X) && pw.ch >= 6 && pw.ch < 9)
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
                    for (int ope = 0; ope < 4; ope++) ((ClsOPN)pw.chip).OutFmSetSlRr(mml, pw, ope, 0, 15);
                    break;
                case 2: // A)ll
                    for (int ope = 0; ope < 4; ope++)
                    {
                        ((ClsOPN)pw.chip).OutFmSetDtMl(mml, pw, ope, 0, 0);
                        ((ClsOPN)pw.chip).OutFmSetKsAr(mml, pw, ope, 3, 31);
                        ((ClsOPN)pw.chip).OutFmSetAmDr(mml, pw, ope, 1, 31);
                        ((ClsOPN)pw.chip).OutFmSetSr(mml, pw, ope, 31);
                        ((ClsOPN)pw.chip).OutFmSetSlRr(mml, pw, ope, 0, 15);
                        ((ClsOPN)pw.chip).OutFmSetSSGEG(mml, pw, ope, 0);
                    }
                    ((ClsOPN)pw.chip).OutFmSetFeedbackAlgorithm(mml, pw, 7, 7);
                    break;
            }


            for (int ope = 0; ope < 4; ope++)
            {
                //ch3以外の拡張チャンネルでも音色設定できるようにする場合はslotの様子もみてセットすること
                ((ClsOPN)pw.chip).OutFmSetDtMl(mml, pw, ope, parent.instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], parent.instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 8]);
                ((ClsOPN)pw.chip).OutFmSetKsAr(mml, pw, ope, parent.instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 7], parent.instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 1]);
                ((ClsOPN)pw.chip).OutFmSetAmDr(mml, pw, ope, parent.instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 10], parent.instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 2]);
                ((ClsOPN)pw.chip).OutFmSetSr(mml, pw, ope, parent.instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 3]);
                ((ClsOPN)pw.chip).OutFmSetSlRr(mml, pw, ope, parent.instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 5], parent.instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 4]);
                ((ClsOPN)pw.chip).OutFmSetSSGEG(mml, pw, ope, parent.instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 11]);
            }
            //ch3以外の拡張チャンネルでも音色設定できるようにする場合はslotの様子もみてセットすること
            pw.op1ml = parent.instFM[n][0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
            pw.op2ml = parent.instFM[n][1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
            pw.op3ml = parent.instFM[n][2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
            pw.op4ml = parent.instFM[n][3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
            //ch3以外の拡張チャンネルでも音色設定できるようにする場合はslotの様子もみてセットすること
            pw.op1dt2 = 0;
            pw.op2dt2 = 0;
            pw.op3dt2 = 0;
            pw.op4dt2 = 0;

            ((ClsOPN)pw.chip).OutFmSetFeedbackAlgorithm(mml, pw, parent.instFM[n][46], parent.instFM[n][45]);

            int alg = parent.instFM[n][45] & 0x7;
            int[] op = new int[4] {
                parent.instFM[n][0*Const.INSTRUMENT_M_OPERATOR_SIZE + 6]
                , parent.instFM[n][1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 6]
                , parent.instFM[n][2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 6]
                , parent.instFM[n][3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 6]
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
                if (algs[alg][i] == 0)// || (pw.slots & (1 << i)) == 0)
                {
                    op[i] = -1;
                    continue;
                }
                op[i] = Common.CheckRange(op[i], 0, 127);
            }

            partWork vpw = pw;
            if (!(pw.chip is YM2609))
            {
                if (pw.chip.lstPartWork[2].Ch3SpecialMode && pw.ch >= m + 3 && pw.ch < m + 6)
                {
                    vpw = pw.chip.lstPartWork[2];
                }
            }
            else
            {
                if (pw.chip.lstPartWork[2].Ch3SpecialMode && pw.ch >= 12 && pw.ch < 15)
                {
                    vpw = pw.chip.lstPartWork[2];
                }
                if (pw.chip.lstPartWork[8].Ch3SpecialMode && pw.ch >= 15 && pw.ch < 18)
                {
                    vpw = pw.chip.lstPartWork[8];
                }
            }

            //ch3以外の拡張チャンネルでも音色設定できるようになったら以下を有効に
            //if ((pw.slots & 1) != 0 && op[0] != -1) ((ClsOPN)pw.chip).OutFmSetTl(mml, vpw, 0, op[0]);
            //if ((pw.slots & 2) != 0 && op[1] != -1) ((ClsOPN)pw.chip).OutFmSetTl(mml, vpw, 1, op[1]);
            //if ((pw.slots & 4) != 0 && op[2] != -1) ((ClsOPN)pw.chip).OutFmSetTl(mml, vpw, 2, op[2]);
            //if ((pw.slots & 8) != 0 && op[3] != -1) ((ClsOPN)pw.chip).OutFmSetTl(mml, vpw, 3, op[3]);
            if (op[0] != -1) ((ClsOPN)pw.chip).OutFmSetTl(mml, vpw, 0, op[0]);
            if (op[1] != -1) ((ClsOPN)pw.chip).OutFmSetTl(mml, vpw, 1, op[1]);
            if (op[2] != -1) ((ClsOPN)pw.chip).OutFmSetTl(mml, vpw, 2, op[2]);
            if (op[3] != -1) ((ClsOPN)pw.chip).OutFmSetTl(mml, vpw, 3, op[3]);


            //音量を再セットする

            OutFmSetVolume(pw, mml, vol, n);

            //拡張チャンネルの場合は他の拡張チャンネルも音量を再セットする
            if (pw.Type == enmChannelType.FMOPNex)
            {
                if (!(pw.chip is YM2609))
                {
                    if (pw.ch != 2) OutFmSetVolume(pw.chip.lstPartWork[2], mml, pw.chip.lstPartWork[2].volume, n);
                    if (pw.ch != m + 3) OutFmSetVolume(pw.chip.lstPartWork[m + 3], mml, pw.chip.lstPartWork[m + 3].volume, n);
                    if (pw.ch != m + 4) OutFmSetVolume(pw.chip.lstPartWork[m + 4], mml, pw.chip.lstPartWork[m + 4].volume, n);
                    if (pw.ch != m + 5) OutFmSetVolume(pw.chip.lstPartWork[m + 5], mml, pw.chip.lstPartWork[m + 5].volume, n);
                }
                else
                {
                    if (pw.ch == 2 || pw.ch == 12 || pw.ch == 13 || pw.ch == 14)
                    {
                        //YM2609 ch3 || ch13 || ch14 || ch15
                        if (pw.ch != 2) OutFmSetVolume(pw.chip.lstPartWork[2], mml, pw.chip.lstPartWork[2].volume, n);
                        if (pw.ch != 12) OutFmSetVolume(pw.chip.lstPartWork[12], mml, pw.chip.lstPartWork[12].volume, n);
                        if (pw.ch != 13) OutFmSetVolume(pw.chip.lstPartWork[13], mml, pw.chip.lstPartWork[13].volume, n);
                        if (pw.ch != 14) OutFmSetVolume(pw.chip.lstPartWork[14], mml, pw.chip.lstPartWork[14].volume, n);
                    }
                    else
                    {
                        //YM2609 ch9 || ch16 || ch17 || ch18
                        if (pw.ch != 8) OutFmSetVolume(pw.chip.lstPartWork[8], mml, pw.chip.lstPartWork[8].volume, n);
                        if (pw.ch != 15) OutFmSetVolume(pw.chip.lstPartWork[15], mml, pw.chip.lstPartWork[15].volume, n);
                        if (pw.ch != 16) OutFmSetVolume(pw.chip.lstPartWork[16], mml, pw.chip.lstPartWork[16].volume, n);
                        if (pw.ch != 17) OutFmSetVolume(pw.chip.lstPartWork[17], mml, pw.chip.lstPartWork[17].volume, n);
                    }
                }
            }

        }

        public void OutFmKeyOff(partWork pw, MML mml)
        {
            int n = (pw.chip is YM2203) ? 0 : 3;

            if (pw.chip is YM2612X && (pw.ch > 8 || pw.ch == 5) && pw.pcm)
            {
                ((YM2612X)pw.chip).OutYM2612XPcmKeyOFF(mml, pw);
                return;
            }

            if (!pw.pcm)
            {
                if (!(pw.chip is YM2609))
                {
                    if (pw.chip.lstPartWork[2].Ch3SpecialMode && pw.Type == enmChannelType.FMOPNex)
                    {
                        pw.Ch3SpecialModeKeyOn = false;

                        int slot = (pw.chip.lstPartWork[2].Ch3SpecialModeKeyOn ? pw.chip.lstPartWork[2].slots : 0x0)
                            | (pw.chip.lstPartWork[n + 3].Ch3SpecialModeKeyOn ? pw.chip.lstPartWork[n + 3].slots : 0x0)
                            | (pw.chip.lstPartWork[n + 4].Ch3SpecialModeKeyOn ? pw.chip.lstPartWork[n + 4].slots : 0x0)
                            | (pw.chip.lstPartWork[n + 5].Ch3SpecialModeKeyOn ? pw.chip.lstPartWork[n + 5].slots : 0x0);

                        parent.OutData(mml, pw.port[0], 0x28, (byte)((slot << 4) + 2));
                    }
                    else
                    {
                        if (pw.ch >= 0 && pw.ch < n + 3)
                        {
                            byte vch = (byte)((pw.ch > 2) ? pw.ch + 1 : pw.ch);
                            //key off
                            parent.OutData(mml, pw.port[0], 0x28, (byte)(0x00 + (vch & 7)));
                        }
                    }
                }
                else
                {
                    if ((pw.ch == 2 || pw.ch == 12 || pw.ch == 13 || pw.ch == 14) && pw.chip.lstPartWork[2].Ch3SpecialMode)
                    {
                        pw.Ch3SpecialModeKeyOn = false;

                        int slot = (pw.chip.lstPartWork[2].Ch3SpecialModeKeyOn ? pw.chip.lstPartWork[2].slots : 0x0)
                            | (pw.chip.lstPartWork[12].Ch3SpecialModeKeyOn ? pw.chip.lstPartWork[12].slots : 0x0)
                            | (pw.chip.lstPartWork[13].Ch3SpecialModeKeyOn ? pw.chip.lstPartWork[13].slots : 0x0)
                            | (pw.chip.lstPartWork[14].Ch3SpecialModeKeyOn ? pw.chip.lstPartWork[14].slots : 0x0);

                        outDatum od = new outDatum();
                        od.val = (byte)((slot << 4) + 2);
                        if (mml != null)
                        {
                            od.type = mml.type;
                            if (mml.line != null && mml.line.Lp != null)
                            {
                                od.linePos = new LinePos(
                                    mml.line.Lp.fullPath,
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
                        ((YM2609)pw.chip).opna20x028KeyOnData.Add(od);
                    }
                    else if ((pw.ch == 8 || pw.ch == 15 || pw.ch == 16 || pw.ch == 17) && pw.chip.lstPartWork[8].Ch3SpecialMode)
                    {
                        pw.Ch3SpecialModeKeyOn = false;

                        int slot = (pw.chip.lstPartWork[8].Ch3SpecialModeKeyOn ? pw.chip.lstPartWork[8].slots : 0x0)
                            | (pw.chip.lstPartWork[15].Ch3SpecialModeKeyOn ? pw.chip.lstPartWork[15].slots : 0x0)
                            | (pw.chip.lstPartWork[16].Ch3SpecialModeKeyOn ? pw.chip.lstPartWork[16].slots : 0x0)
                            | (pw.chip.lstPartWork[17].Ch3SpecialModeKeyOn ? pw.chip.lstPartWork[17].slots : 0x0);

                        outDatum od = new outDatum();
                        od.val = (byte)((slot << 4) + 2);
                        if (mml != null)
                        {
                            od.type = mml.type;
                            if (mml.line != null && mml.line.Lp != null)
                            {
                                od.linePos = new LinePos(
                                    mml.line.Lp.fullPath,
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
                        ((YM2609)pw.chip).opna20x228KeyOnData.Add(od);
                    }
                    else
                    {
                        if (pw.ch >= 0 && pw.ch < 6)
                        {
                            byte vch = (byte)((pw.ch > 2) ? pw.ch + 1 : pw.ch);
                            //key off
                            outDatum od = new outDatum();
                            od.val = (byte)(0x00 + (vch & 7));
                            if (mml != null)
                            {
                                od.type = mml.type;
                                if (mml.line != null && mml.line.Lp != null)
                                {
                                    od.linePos = new LinePos(
                                        mml.line.Lp.fullPath,
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
                            ((YM2609)pw.chip).opna20x028KeyOnData.Add(od);
                        }
                        else if (pw.ch >= 6 && pw.ch < 12)
                        {
                            byte vch = (byte)(pw.ch - 6);
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
                                        mml.line.Lp.fullPath,
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
                            ((YM2609)pw.chip).opna20x228KeyOnData.Add(od);
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
                    parent.OutData(mml, cmd, (byte)pw.streamID);
                }
            }

            pw.pcmWaitKeyOnCounter = -1;
        }

        public void OutFmAllKeyOff()
        {

            foreach (partWork pw in lstPartWork)
            {
                if (!(pw.chip is YM2609)) { if (pw.ch > 5) continue; }
                else if (pw.ch > 11) continue;

                OutFmKeyOff(pw, null);
                OutFmSetTl(null, pw, 0, 127);
                OutFmSetTl(null, pw, 1, 127);
                OutFmSetTl(null, pw, 2, 127);
                OutFmSetTl(null, pw, 3, 127);
            }

        }

        public void OutFmSetFnum(partWork pw,MML mml, int octave, int num)
        {
            int freq;
            freq = ((num & 0x700) >> 8) + (((octave - 1) & 0x7) << 3);
            freq = (freq << 8) + (num & 0xff);

            if (freq == pw.freq) return;

            pw.freq = freq;

            if (pw.chip.lstPartWork[2].Ch3SpecialMode && pw.Type == enmChannelType.FMOPNex)
            {
                if ((pw.slots & 8) != 0)
                {
                    int f = pw.freq + pw.slotDetune[3];
                    parent.OutData(mml, pw.port[0], (byte)0xa6, (byte)(f >> 8));
                    parent.OutData(mml, pw.port[0], (byte)0xa2, (byte)f);
                }
                if ((pw.slots & 4) != 0)
                {
                    int f = pw.freq + pw.slotDetune[2];
                    parent.OutData(mml, pw.port[0], (byte)0xac, (byte)(f >> 8));
                    parent.OutData(mml, pw.port[0], (byte)0xa8, (byte)f);
                }
                if ((pw.slots & 1) != 0)
                {
                    int f = pw.freq + pw.slotDetune[0];
                    parent.OutData(mml, pw.port[0], (byte)0xad, (byte)(f >> 8));
                    parent.OutData(mml, pw.port[0], (byte)0xa9, (byte)f);
                }
                if ((pw.slots & 2) != 0)
                {
                    int f = pw.freq + pw.slotDetune[1];
                    parent.OutData(mml, pw.port[0], (byte)0xae, (byte)(f >> 8));
                    parent.OutData(mml, pw.port[0], (byte)0xaa, (byte)f);
                }
            }
            else
            {
                int n;
                if (!(pw.chip is YM2609))
                {
                    n = (pw.chip is YM2203) ? 0 : 3;
                    if (pw.ch >= n + 3 && pw.ch < n + 6)
                    {
                        return;
                    }
                }
                else
                {
                    n = 9;
                    if (pw.ch >= 12 && pw.ch < 18)
                    {
                        return;
                    }
                }
                if ((pw.chip is YM2612X) && pw.ch >= 9 && pw.ch <= 11)
                {
                    return;
                }
                if (pw.ch < n + 3)
                {
                    if (pw.pcm) return;

                    int vch;
                    byte[] port;
                    GetPortVch(pw, out port, out vch);

                    parent.OutData(mml, port, (byte)(0xa4 + vch), (byte)((pw.freq & 0xff00) >> 8));
                    parent.OutData(mml, port, (byte)(0xa0 + vch), (byte)(pw.freq & 0xff));
                }
            }
        }

        public void OutFmKeyOn(partWork pw,MML mml)
        {
            SetDummyData(pw, mml);

            int n = (pw.chip is YM2203) ? 0 : 3;

            if (pw.chip is YM2612X && (pw.ch > 8 || pw.ch == 5) && pw.pcm)
            {
                ((YM2612X)pw.chip).OutYM2612XPcmKeyON(mml,pw);
                return;
            }

            if (!pw.pcm)
            {
                if (!(pw.chip is YM2609))
                {
                    if (pw.chip.lstPartWork[2].Ch3SpecialMode && pw.Type == enmChannelType.FMOPNex)
                    {
                        pw.Ch3SpecialModeKeyOn = true;

                        int slot = (pw.chip.lstPartWork[2].Ch3SpecialModeKeyOn ? pw.chip.lstPartWork[2].slots : 0x0)
                            | (pw.chip.lstPartWork[n + 3].Ch3SpecialModeKeyOn ? pw.chip.lstPartWork[n + 3].slots : 0x0)
                            | (pw.chip.lstPartWork[n + 4].Ch3SpecialModeKeyOn ? pw.chip.lstPartWork[n + 4].slots : 0x0)
                            | (pw.chip.lstPartWork[n + 5].Ch3SpecialModeKeyOn ? pw.chip.lstPartWork[n + 5].slots : 0x0);

                        if (pw.chip is YM2612X)
                        {
                            outDatum od = new outDatum();
                            od.val = (byte)((slot << 4) + 2);
                            if (mml != null)
                            {
                                od.type = mml.type;
                                if (mml.line != null && mml.line.Lp != null)
                                {
                                    od.linePos = new LinePos(
                                        mml.line.Lp.fullPath,
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
                            parent.OutData(mml, pw.port[0], 0x28, (byte)((slot << 4) + 2));
                        }
                    }
                    else
                    {
                        if (pw.ch >= 0 && pw.ch < n + 3)
                        {
                            byte vch = (byte)((pw.ch > 2) ? pw.ch + 1 : pw.ch);
                            if (pw.chip is YM2612X)
                            {
                                outDatum od = new outDatum();
                                od.val = (byte)((pw.slots << 4) + (vch & 7));
                                if (mml != null)
                                {
                                    od.type = mml.type;
                                    if (mml.line != null && mml.line.Lp != null)
                                    {
                                        od.linePos = new LinePos(
                                            mml.line.Lp.fullPath,
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
                                parent.OutData(mml, pw.port[0], 0x28, (byte)((pw.slots << 4) + (vch & 7)));
                            }
                        }
                    }
                }
                else
                {
                    if ((pw.ch == 2 || pw.ch == 12 || pw.ch == 13 || pw.ch == 14) && pw.chip.lstPartWork[2].Ch3SpecialMode)
                    {
                        pw.Ch3SpecialModeKeyOn = true;

                        int slot = (pw.chip.lstPartWork[2].Ch3SpecialModeKeyOn ? pw.chip.lstPartWork[2].slots : 0x0)
                            | (pw.chip.lstPartWork[12].Ch3SpecialModeKeyOn ? pw.chip.lstPartWork[12].slots : 0x0)
                            | (pw.chip.lstPartWork[13].Ch3SpecialModeKeyOn ? pw.chip.lstPartWork[13].slots : 0x0)
                            | (pw.chip.lstPartWork[14].Ch3SpecialModeKeyOn ? pw.chip.lstPartWork[14].slots : 0x0);

                        outDatum od = new outDatum();
                        od.val = (byte)((slot << 4) + 2);
                        if (mml != null)
                        {
                            od.type = mml.type;
                            if (mml.line != null && mml.line.Lp != null)
                            {
                                od.linePos = new LinePos(
                                    mml.line.Lp.fullPath,
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
                        ((YM2609)pw.chip).opna20x028KeyOnData.Add(od);

                    }
                    else if ((pw.ch == 8 || pw.ch == 15 || pw.ch == 16 || pw.ch == 17) && pw.chip.lstPartWork[8].Ch3SpecialMode)
                    {
                        pw.Ch3SpecialModeKeyOn = true;

                        int slot = (pw.chip.lstPartWork[8].Ch3SpecialModeKeyOn ? pw.chip.lstPartWork[8].slots : 0x0)
                            | (pw.chip.lstPartWork[15].Ch3SpecialModeKeyOn ? pw.chip.lstPartWork[15].slots : 0x0)
                            | (pw.chip.lstPartWork[16].Ch3SpecialModeKeyOn ? pw.chip.lstPartWork[16].slots : 0x0)
                            | (pw.chip.lstPartWork[17].Ch3SpecialModeKeyOn ? pw.chip.lstPartWork[17].slots : 0x0);

                        outDatum od = new outDatum();
                        od.val = (byte)((slot << 4) + 2);
                        if (mml != null)
                        {
                            od.type = mml.type;
                            if (mml.line != null && mml.line.Lp != null)
                            {
                                od.linePos = new LinePos(
                                    mml.line.Lp.fullPath,
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
                        ((YM2609)pw.chip).opna20x228KeyOnData.Add(od);
                    }
                    else
                    {
                        if (pw.ch >= 0 && pw.ch < 6)
                        {
                            byte vch = (byte)((pw.ch > 2) ? pw.ch + 1 : pw.ch);
                            //key on
                            outDatum od = new outDatum();
                            od.val = (byte)((pw.slots << 4) + (vch & 7));
                            if (mml != null)
                            {
                                od.type = mml.type;
                                if (mml.line != null && mml.line.Lp != null)
                                {
                                    od.linePos = new LinePos(
                                        mml.line.Lp.fullPath,
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
                            ((YM2609)pw.chip).opna20x028KeyOnData.Add(od);
                        }
                        else if (pw.ch >= 6 && pw.ch < 12)
                        {
                            byte vch = (byte)(pw.ch - 6);
                            vch = (byte)(((vch > 2) ? (vch + 1) : vch));
                            //key on
                            outDatum od = new outDatum();
                            od.val = (byte)((pw.slots << 4) + (vch & 7));
                            if (mml != null)
                            {
                                od.type = mml.type;
                                if (mml.line != null && mml.line.Lp != null)
                                {
                                    od.linePos = new LinePos(
                                        mml.line.Lp.fullPath,
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
                            ((YM2609)pw.chip).opna20x228KeyOnData.Add(od);
                        }
                    }
                }

                return;
            }


            if (pw.isPcmMap)
            {
                int nt = Const.NOTE.IndexOf(pw.noteCmd);
                int f = pw.octaveNow * 12 + nt + pw.shift + pw.keyShift;
                if (parent.instPCMMap.ContainsKey(pw.pcmMapNo))
                {
                    if (parent.instPCMMap[pw.pcmMapNo].ContainsKey(f))
                    {
                        pw.instrument = parent.instPCMMap[pw.pcmMapNo][f];
                    }
                    else
                    {
                        msgBox.setErrMsg(string.Format(msg.get("E10025"), pw.octaveNow, pw.noteCmd, pw.shift + pw.keyShift), mml.line.Lp);
                        return;
                    }
                }
                else
                {
                    msgBox.setErrMsg(string.Format(msg.get("E10024"), pw.pcmMapNo), mml.line.Lp);
                    return;
                }
            }

            if (!parent.instPCM.ContainsKey(pw.instrument)) return;

            float m = Const.pcmMTbl[pw.pcmNote] * (float)Math.Pow(2, (pw.pcmOctave - 4));
            pw.pcmBaseFreqPerFreq = Information.VGM_SAMPLE_PER_SECOND / ((float)parent.instPCM[pw.instrument].freq * m);
            pw.pcmFreqCountBuffer = 0.0f;
            long p = parent.instPCM[pw.instrument].stAdr;
            if (parent.info.Version == 1.51f)
            {
                byte[] cmd;
                if (parent.info.format == enmFormat.ZGM)
                {
                    if (parent.ChipCommandSize == 2) cmd = new byte[] { 0x40, 0x00 };
                    else cmd = new byte[] { 0x40 };
                }
                else cmd = new byte[] { 0xe0 };
                parent.OutData(
                    mml, cmd
                    , (byte)(p & 0xff)
                    , (byte)((p & 0xff00) / 0x100)
                    , (byte)((p & 0xff0000) / 0x10000)
                    , (byte)((p & 0xff000000) / 0x10000)
                    );
            }
            else
            {
                long s = parent.instPCM[pw.instrument].size;
                long f = parent.instPCM[pw.instrument].freq;
                long w = 0;
                if (pw.gatetimePmode)
                {
                    w = pw.waitCounter * pw.gatetime / 8L;
                }
                else
                {
                    w = pw.waitCounter - pw.gatetime;
                }
                if (w < 1) w = 1;
                s = Math.Min(s, (long)(w * parent.info.samplesPerClock * f / 44100.0));

                byte[] cmd;
                if (!pw.streamSetup)
                {
                    parent.newStreamID++;
                    pw.streamID = parent.newStreamID;

                    if (parent.info.format == enmFormat.ZGM)
                    {
                        if (parent.ChipCommandSize == 2) cmd = new byte[] {
                            0x30, 0x00
                            , (byte)pw.streamID
                            , (byte)pw.chip.ChipID
                            , (byte)(pw.chip.ChipID >> 8)
                        };
                        else cmd = new byte[] {
                            0x30
                            , (byte)pw.streamID
                            , (byte)pw.chip.ChipID
                        };
                    }
                    else cmd = new byte[] {
                        0x90
                        , (byte)pw.streamID
                        , (byte)(0x02 + (pw.chipNumber!=0 ? 0x80 : 0x00))
                    };

                    parent.OutData(
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
                    parent.OutData(
                        mml
                        // set stream data
                        , cmd
                        , (byte)pw.streamID
                        , 0x00
                        , 0x01
                        , 0x00
                        );

                    pw.streamSetup = true;
                }

                if (pw.streamFreq != f)
                {
                    if (parent.info.format == enmFormat.ZGM)
                    {
                        if (parent.ChipCommandSize == 2) cmd = new byte[] { 0x32, 0x00 };
                        else cmd = new byte[] { 0x32 };
                    }
                    else cmd = new byte[] { 0x92 };
                    //Set Stream Frequency
                    parent.OutData(
                        mml, cmd
                        , (byte)pw.streamID
                        , (byte)(f & 0xff)
                        , (byte)((f & 0xff00) / 0x100)
                        , (byte)((f & 0xff0000) / 0x10000)
                        , (byte)((f & 0xff000000) / 0x10000)
                        );

                    pw.streamFreq = f;
                }

                if (parent.info.format == enmFormat.ZGM)
                {
                    if (parent.ChipCommandSize == 2) cmd = new byte[] { 0x33, 0x00 };
                    else cmd = new byte[] { 0x33 };
                }
                else cmd = new byte[] { 0x93 };
                //Start Stream
                parent.OutData(
                    mml,
                    cmd
                    , (byte)pw.streamID

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

            if (parent.instPCM[pw.instrument].status != enmPCMSTATUS.ERROR)
            {
                parent.instPCM[pw.instrument].status = enmPCMSTATUS.USED;
            }

        }


        public void SetFmFNum(partWork pw,MML mml)
        {
            if (pw.noteCmd == (char)0)
            {
                return;
            }

            int[] ftbl = pw.chip.FNumTbl[0];

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
            OutFmSetFnum(pw, mml, o, f);
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

        public override int GetFNum(partWork pw, MML mml, int octave, char cmd, int shift)
        {
            if (pw.Type == enmChannelType.FMOPN || pw.Type == enmChannelType.FMOPNex)
            {
                return GetFmFNum(FNumTbl[0], octave, cmd, shift);
            }
            if (pw.Type == enmChannelType.SSG)
            {
                return GetSsgFNum(pw, mml, octave, cmd, shift);
            }
            return 0;
        }

        public override void GetFNumAtoB(partWork pw,MML mml
            , out int a, int aOctaveNow, char aCmd, int aShift
            , out int b, int bOctaveNow, char bCmd, int bShift
            , int dir)
        {
            a = GetFNum(pw,mml, aOctaveNow, aCmd, aShift);
            b = GetFNum(pw,mml, bOctaveNow, bCmd, bShift);

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


        public void SetFmVolume(partWork pw,MML mml)
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
                    OutFmSetVolume(pw, mml, vol, pw.instrument);
                    pw.beforeVolume = vol;
                }
            }
        }

        public override void SetKeyOff(partWork pw,MML mml)
        { }

        public override void SetVolume(partWork pw, MML mml)
        {
            if (pw.Type == enmChannelType.FMOPN
                || pw.Type == enmChannelType.FMOPNex //効果音モード対応チャンネル
                || (pw.Type == enmChannelType.FMPCM && !pw.pcm) //OPN2PCMチャンネル
                || (pw.Type == enmChannelType.FMPCMex && !pw.pcm) //OPN2XPCMチャンネル
                )
            {
                SetFmVolume(pw, mml);
            }
            else if (pw.Type == enmChannelType.SSG)
            {
                SetSsgVolume(pw, mml);
            }
        }

        public override void SetLfoAtKeyOn(partWork pw, MML mml)
        {
            for (int lfo = 0; lfo < 4; lfo++)
            {
                clsLfo pl = pw.lfo[lfo];
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
                    if (pw.Type == enmChannelType.FMOPN
                        || pw.Type == enmChannelType.FMOPNex)
                        SetFmFNum(pw, mml);
                    else if (pw.Type == enmChannelType.SSG)
                        SetSsgFNum(pw, mml);

                }

                if (pl.type == eLfoType.Tremolo)
                {
                    pw.beforeVolume = -1;
                    if (pw.Type == enmChannelType.FMOPN
                        || pw.Type == enmChannelType.FMOPNex)
                        SetFmVolume(pw, mml);
                    else if (pw.Type == enmChannelType.SSG)
                        SetSsgVolume(pw, mml);
                }

            }
        }

        public override void SetToneDoubler(partWork pw, MML mml)
        {
            if (pw.Type != enmChannelType.FMOPN && pw.ch != 2)
            {
                return;
            }

            int i = pw.instrument;
            if (i < 0) return;

            pw.toneDoublerKeyShift = 0;
            byte[] instFM = parent.instFM[i];
            if (instFM == null || instFM.Length < 1) return;
            Note note = (Note)mml.args[0];

            if (pw.TdA == -1)
            {
                //resetToneDoubler
                if (pw.op1ml != instFM[0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8])
                {
                    OutFmSetDtMl(mml, pw, 0, instFM[0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instFM[0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8]);
                    pw.op1ml = instFM[0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
                }
                if (pw.op2ml != instFM[1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8])
                {
                    OutFmSetDtMl(mml, pw, 1, instFM[1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instFM[1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8]);
                    pw.op2ml = instFM[1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
                }
                if (pw.op3ml != instFM[2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8])
                {
                    OutFmSetDtMl(mml, pw, 2, instFM[2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instFM[2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8]);
                    pw.op3ml = instFM[2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
                }
                if (pw.op4ml != instFM[3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8])
                {
                    OutFmSetDtMl(mml, pw, 3, instFM[3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instFM[3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8]);
                    pw.op4ml = instFM[3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
                }
            }
            else
            {
                //setToneDoubler
                int oct = pw.octaveNow;
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
                pw.octaveNew = oct;
                int TdB = oct * 12 + Const.NOTE.IndexOf(note.tDblCmd) + note.tDblShift + pw.keyShift;
                int s = TdB - pw.TdA;// - TdB;
                int us = Math.Abs(s);
                int n = pw.toneDoubler;
                clsToneDoubler instToneDoubler = parent.instToneDoubler[n];
                if (us >= instToneDoubler.lstTD.Count)
                {
                    return;
                }

                pw.toneDoublerKeyShift = ((s < 0) ? s : 0) + instToneDoubler.lstTD[us].KeyShift;

                if (pw.op1ml != instToneDoubler.lstTD[us].OP1ML)
                {
                    OutFmSetDtMl(mml, pw, 0, instFM[0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instToneDoubler.lstTD[us].OP1ML);
                    pw.op1ml = instToneDoubler.lstTD[us].OP1ML;
                }
                if (pw.op2ml != instToneDoubler.lstTD[us].OP2ML)
                {
                    OutFmSetDtMl(mml, pw, 1, instFM[1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instToneDoubler.lstTD[us].OP2ML);
                    pw.op2ml = instToneDoubler.lstTD[us].OP2ML;
                }
                if (pw.op3ml != instToneDoubler.lstTD[us].OP3ML)
                {
                    OutFmSetDtMl(mml, pw, 2, instFM[2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instToneDoubler.lstTD[us].OP3ML);
                    pw.op3ml = instToneDoubler.lstTD[us].OP3ML;
                }
                if (pw.op4ml != instToneDoubler.lstTD[us].OP4ML)
                {
                    OutFmSetDtMl(mml, pw, 3, instFM[3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instToneDoubler.lstTD[us].OP4ML);
                    pw.op4ml = instToneDoubler.lstTD[us].OP4ML;
                }

                //pw.TdA = -1;
            }
        }

        public override int GetToneDoublerShift(partWork pw, int octave, char noteCmd, int shift)
        {
            if (pw.Type != enmChannelType.FMOPN && pw.ch != 2)
            {
                return 0;
            }

            int i = pw.instrument;
            if (pw.TdA == -1)
            {
                return 0;
            }

            int TdB = octave * 12 + Const.NOTE.IndexOf(noteCmd) + shift;
            int s = pw.TdA - TdB;
            int us = Math.Abs(s);
            int n = pw.toneDoubler;
            if (us >= parent.instToneDoubler[n].lstTD.Count)
            {
                return 0;
            }

            return ((s < 0) ? s : 0) + parent.instToneDoubler[n].lstTD[us].KeyShift;
        }


        private void CmdY_ToneParamOPN(MML mml,byte adr, partWork pw, byte op, byte dat)
        {
            int ch;
            if (pw.Type == enmChannelType.FMOPNex) ch = 2;
            else if (pw.Type == enmChannelType.FMOPN) ch = pw.ch;
            else return;

            int vch;
            byte[] port;
            GetPortVch(pw, out port, out vch);

            op = (byte)(op == 1 ? 2 : (op == 2 ? 1 : op));

            adr += (byte)(vch + (op << 2));

            parent.OutData(mml,port, adr, dat);
        }

        private void CmdY_ToneParamOPN_FBAL(MML mml,partWork pw, byte dat)
        {
            int ch;
            if (pw.Type == enmChannelType.FMOPNex) ch = 2;
            else if (pw.Type == enmChannelType.FMOPN) ch = pw.ch;
            else return;

            int vch;
            byte[] port;
            GetPortVch(pw, out port, out vch);

            byte adr = (byte)(0xb0 + vch);

            parent.OutData(mml,port, adr, dat);
        }


        public override void CmdNoiseToneMixer(partWork pw, MML mml)
        {
            if (pw.Type == enmChannelType.SSG)
            {
                int n = (int)mml.args[0];
                n = Common.CheckRange(n, 0, 3);
                pw.mixer = n;
            }
        }

        public override void CmdNoise(partWork pw, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 0, 31);

            int ch = 0;
            if (pw.chip is YM2609)
            {
                if (pw.ch >= 18 && pw.ch <= 20) ch = 18;
                if (pw.ch >= 21 && pw.ch <= 23) ch = 21;
                if (pw.ch >= 24 && pw.ch <= 26) ch = 24;
                if (pw.ch >= 27 && pw.ch <= 29) ch = 27;
            }

            pw.chip.lstPartWork[ ch ].noise = n;//各SSGChの1Chに保存
            ((ClsOPN)pw.chip).OutSsgNoise(mml,pw, n);
        }

        public override void CmdInstrument(partWork pw, MML mml)
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
                pw.toneDoubler = n;
                return;
            }

            if (type == 'E')
            {
                SetEnvelopParamFromInstrument(pw, n, mml);
                return;
            }

            if (pw.Type == enmChannelType.SSG)
            {
                SetEnvelopParamFromInstrument(pw, n, mml);
                return;
            }

            n = Common.CheckRange(n, 0, 255);
            if (pw.instrument == n) return;
            pw.instrument = n;
            ((ClsOPN)pw.chip).OutFmSetInstrument(pw, mml, n, pw.volume, type);
        }

        public override void CmdEnvelope(partWork pw, MML mml)
        {

            base.CmdEnvelope(pw, mml);

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
                    if (pw.Type == enmChannelType.SSG)
                    {
                        pw.beforeVolume = -1;
                    }
                    break;
            }
        }

        public override void CmdExtendChannel(partWork pw, MML mml)
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
                        pw.slotsEX = res;
                        if (pw.Ch3SpecialMode)
                        {
                            pw.slots = pw.slotsEX;
                            pw.beforeVolume = -1;
                        }
                    }
                    break;
                case "EXON":
                    pw.Ch3SpecialMode = true;
                    ((ClsOPN)pw.chip).OutOPNSetCh3SpecialMode(mml,pw, true);
                    foreach (partWork p in pw.chip.lstPartWork)
                    {
                        if(pw.chip.chipType== enmChipType.YM2609)
                        {
                            if (pw.ch == 2 || pw.ch == 12 || pw.ch == 13 || pw.ch == 14)
                                if (p.ch == 8 || p.ch == 15 || p.ch == 16 || p.ch == 17)
                                    continue;

                            if (pw.ch == 8 || pw.ch == 15 || pw.ch == 16 || pw.ch == 17)
                                if (p.ch == 2 || p.ch == 12 || p.ch == 13 || p.ch == 14)
                                    continue;
                        }

                        if (p.Type == enmChannelType.FMOPNex)
                        {
                            p.slots = p.slotsEX;
                            p.beforeVolume = -1;
                            p.beforeFNum = -1;
                            //p.freq = -1;
                            p.oldFreq = -1;
                            //SetFmFNum(p,mml);
                        }
                    }
                    break;
                case "EXOF":
                    pw.Ch3SpecialMode = false;
                    ((ClsOPN)pw.chip).OutOPNSetCh3SpecialMode(mml,pw, false);
                    foreach (partWork p in pw.chip.lstPartWork) {
                        if (p.Type == enmChannelType.FMOPNex)
                        {
                            p.beforeVolume = -1;
                            p.beforeFNum = -1;
                            p.freq = -1;

                            if (p.ch != 2 && (p.chip.chipType!=enmChipType.YM2609 || p.ch != 8)) // 2 -> Ch3   8 -> OPNA2のCh9のこと
                                p.slots = 0;
                            else
                            {
                                p.slots = p.slots4OP;
                                SetKeyOff(p, mml);
                            }

                            //SetFmFNum(p);
                        }
                    }
                    break;
                case "EXD":
                    pw.slotDetune[0] = (int)mml.args[1];
                    pw.slotDetune[1] = (int)mml.args[2];
                    pw.slotDetune[2] = (int)mml.args[3];
                    pw.slotDetune[3] = (int)mml.args[4];
                    break;
            }
        }

        public override void CmdVolume(partWork pw, MML mml)
        {
            base.CmdVolume(pw, mml);
        }

        public override void CmdY(partWork pw, MML mml)
        {
            if (mml.args[0] is string toneparamName)
            {
                byte op = (byte)mml.args[1];
                byte dat = (byte)mml.args[2];

                switch (toneparamName)
                {
                    case "DTML":
                        CmdY_ToneParamOPN(mml,0x30, pw, op, dat);
                        break;
                    case "TL":
                        CmdY_ToneParamOPN(mml, 0x40, pw, op, dat);
                        break;
                    case "KSAR":
                        CmdY_ToneParamOPN(mml, 0x50, pw, op, dat);
                        break;
                    case "AMDR":
                        CmdY_ToneParamOPN(mml, 0x60, pw, op, dat);
                        break;
                    case "SR":
                        CmdY_ToneParamOPN(mml, 0x70, pw, op, dat);
                        break;
                    case "SLRR":
                        CmdY_ToneParamOPN(mml, 0x80, pw, op, dat);
                        break;
                    case "SSG":
                        CmdY_ToneParamOPN(mml, 0x90, pw, op, dat);
                        break;
                    case "FBAL":
                        CmdY_ToneParamOPN_FBAL(mml, pw, dat);
                        break;
                }
            }
        }

        public override void CmdLoopExtProc(partWork pw, MML mml)
        {
        }

        public override void CmdHardEnvelope(partWork pw, MML mml)
        {
            if (pw.Type != enmChannelType.SSG) return;

            string cmd = (string)mml.args[0];
            int n = 0;

            int port;
            int adr;
            int vch;
            GetPortVchSsg(pw, out port, out adr, out vch);

            switch (cmd)
            {
                case "EH":
                    n = (int)mml.args[1];
                    if (pw.HardEnvelopeSpeed != n)
                    {
                        parent.OutData(mml, pw.port[port], (byte)(adr + 0x0b), (byte)(n & 0xff));
                        parent.OutData(mml, pw.port[port], (byte)(adr + 0x0c), (byte)((n >> 8) & 0xff));
                        pw.HardEnvelopeSpeed = n;
                    }
                    break;
                case "EHON":
                    pw.HardEnvelopeSw = true;
                    break;
                case "EHOF":
                    pw.HardEnvelopeSw = false;
                    break;
                case "EHT":
                    n = (int)mml.args[1];
                    if (pw.HardEnvelopeType != n)
                    {
                        parent.OutData(mml, pw.port[port], (byte)(adr + 0x0d), (byte)(n & 0xf));
                        pw.HardEnvelopeType = n;
                    }
                    break;
            }
        }

    }
}
