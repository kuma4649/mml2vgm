using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using musicDriverInterface;

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

            int n = (pw.pg[pw.cpg].mixer & 0x1) + ((pw.pg[pw.cpg].mixer & 0x2) << 2);
            byte data = 0;

            data = (byte)(((ClsOPN)pw.pg[pw.cpg].chip).SSGKeyOn[p] | (9 << vch));
            data &= (byte)(~(n << vch));
            ((ClsOPN)pw.pg[pw.cpg].chip).SSGKeyOn[p] = data;

            SetSsgVolume(pw, mml);
            if (pw.pg[pw.cpg].HardEnvelopeSw)
            {
                parent.OutData(mml, pw.pg[pw.cpg].port[port], (byte)(adr + 0x0d), (byte)(pw.pg[pw.cpg].HardEnvelopeType & 0xf));
            }
            parent.OutData(mml, pw.pg[pw.cpg].port[port], (byte)(adr + 0x07), data);

            MML vmml = new MML();
            vmml.type = enmMMLType.Volume;
            vmml.args = new List<object>();
            vmml.args.Add(pw.pg[pw.cpg].volume);
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

            data = (byte)(((ClsOPN)pw.pg[pw.cpg].chip).SSGKeyOn[p] | (n << vch));
            ((ClsOPN)pw.pg[pw.cpg].chip).SSGKeyOn[p] = data;

            parent.OutData(mml, pw.pg[pw.cpg].port[port], (byte)(adr + 0x08 + vch), 0);
            pw.pg[pw.cpg].beforeVolume = -1;
            parent.OutData(mml, pw.pg[pw.cpg].port[port], (byte)(adr + 0x07), data);

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

            int vol = pw.pg[pw.cpg].volume;
            if (pw.pg[pw.cpg].envelopeMode)
            {
                vol = 0;
                if (pw.pg[pw.cpg].envIndex != -1)
                {
                    vol = pw.pg[pw.cpg].volume - (15 - pw.pg[pw.cpg].envVolume);
                }
            }

            for (int lfo = 0; lfo < 4; lfo++)
            {
                if (!pw.pg[pw.cpg].lfo[lfo].sw) continue;
                if (pw.pg[pw.cpg].lfo[lfo].type != eLfoType.Tremolo) continue;

                vol += pw.pg[pw.cpg].lfo[lfo].value + pw.pg[pw.cpg].lfo[lfo].param[6];
            }

            vol = Common.CheckRange(vol, 0, 15) + (pw.pg[pw.cpg].HardEnvelopeSw ? 0x10 : 0x00);
            if ((((ClsOPN)pw.pg[pw.cpg].chip).SSGKeyOn[p] & (9 << vch)) == (9 << vch))
            {
                vol = 0;
            }

            if (pw.pg[pw.cpg].chip is YM2609)
            {
                int pan = (int)(pw.pg[pw.cpg].pan.val == null ? 0 : pw.pg[pw.cpg].pan.val);
                vol |= (byte)(pan << 6);
            }

            if (pw.pg[pw.cpg].beforeVolume != vol)
            {
                parent.OutData(mml, pw.pg[pw.cpg].port[port], (byte)(adr + 0x08 + vch), (byte)vol);
                pw.pg[pw.cpg].beforeVolume = vol;
            }
        }

        public void OutSsgNoise(MML mml,partWork pw, int n)
        {
            int port;
            int adr;
            int vch;//ノイズ設定はch未使用
            GetPortVchSsg(pw, out port, out adr, out vch);

            parent.OutData(mml, pw.pg[pw.cpg].port[port], (byte)(adr + 0x06), (byte)(n & 0x1f));
        }

        public void SetSsgFNum(partWork pw,MML mml)
        {
            int f = - pw.pg[pw.cpg].detune;
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
                f -= pw.pg[pw.cpg].lfo[lfo].value + pw.pg[pw.cpg].lfo[lfo].param[6];
            }

            if (pw.pg[pw.cpg].octaveNow < 1)
            {
                f <<= -pw.pg[pw.cpg].octaveNow;
            }
            else
            {
                f >>= pw.pg[pw.cpg].octaveNow - 1;
            }

            if (pw.pg[pw.cpg].bendWaitCounter != -1)
            {
                f += pw.pg[pw.cpg].bendFnum;
            }
            else
            {
                f += GetSsgFNum(pw, mml, pw.pg[pw.cpg].octaveNow, pw.pg[pw.cpg].noteCmd, pw.pg[pw.cpg].shift + pw.pg[pw.cpg].keyShift);//
            }

            f = Common.CheckRange(f, 0, 0xfff);
            if (pw.pg[pw.cpg].freq == f) return;

            pw.pg[pw.cpg].freq = f;

            int port;
            int adr;
            int vch;
            GetPortVchSsg(pw, out port, out adr, out vch);

            byte data = 0;

            data = (byte)(f & 0xff);
            parent.OutData(mml,pw.pg[pw.cpg].port[port], (byte)(adr + 0 + vch * 2), data);

            data = (byte)((f & 0xf00) >> 8);
            parent.OutData(mml,pw.pg[pw.cpg].port[port], (byte)(adr + 1 + vch * 2), data);
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
            if (f >= pw.pg[pw.cpg].chip.FNumTbl[1].Length) f = pw.pg[pw.cpg].chip.FNumTbl[1].Length - 1;

            return pw.pg[pw.cpg].chip.FNumTbl[1][f];
        }


        public void OutOPNSetPanAMSPMS(MML mml, partWork pw, int pan, int ams, int pms)
        {
            //TODO: 効果音パートで指定されている場合の考慮不足
            int vch;
            byte[] port;
            GetPortVch(pw, out port, out vch);
            if (pw.pg[pw.cpg].chip is YM2612X && pw.pg[pw.cpg].ch > 8 && pw.pg[pw.cpg].ch<12)
            {
                vch = 2;
            }

            pan = pan & 3;
            ams = ams & 3;
            pms = pms & 7;

            parent.OutData(mml, port, (byte)(0xb4 + vch), (byte)((pan << 6) + (ams << 4) + pms));
        }

        public void OutOPNSetHardLfo(MML mml,partWork pw, bool sw, int lfoNum)
        {
            parent.OutData(
                mml,
                pw.pg[pw.cpg].port[0]
                , 0x22
                , (byte)((lfoNum & 7) + (sw ? 8 : 0))
                );
        }

        public void OutOPNSetCh3SpecialMode(MML mml,partWork pw, bool sw)
        {
            byte[] port = pw.pg[pw.cpg].port[0];
            if(pw.pg[pw.cpg].chip.chipType== enmChipType.YM2609)
            {
                if (pw.pg[pw.cpg].ch == 8 || pw.pg[pw.cpg].ch == 15 || pw.pg[pw.cpg].ch == 16 || pw.pg[pw.cpg].ch == 17)
                {
                    port = pw.pg[pw.cpg].port[2];
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
            if (!(pw.pg[pw.cpg].chip is YM2609))
            {
                port = pw.pg[pw.cpg].ch > 2 ? pw.pg[pw.cpg].port[1] : pw.pg[pw.cpg].port[0];
                vch = (byte)(pw.pg[pw.cpg].ch > 2 ? pw.pg[pw.cpg].ch - 3 : pw.pg[pw.cpg].ch);
            }
            else
            {
                port = 
                    pw.pg[pw.cpg].ch < 3 ? 
                    pw.pg[pw.cpg].port[0] :
                    (pw.pg[pw.cpg].ch < 6 ? 
                        pw.pg[pw.cpg].port[1] : 
                        (pw.pg[pw.cpg].ch < 9 ? 
                            pw.pg[pw.cpg].port[2] :
                            (pw.pg[pw.cpg].ch < 12 ?
                                pw.pg[pw.cpg].port[3] :
                                (pw.pg[pw.cpg].ch < 15 ?
                                    pw.pg[pw.cpg].port[0] :
                                    pw.pg[pw.cpg].port[2]
                                )
                            )
                        )
                    );
                vch = (byte)(
                    pw.pg[pw.cpg].ch < 3 ? 
                    pw.pg[pw.cpg].ch : 
                    (pw.pg[pw.cpg].ch < 6 ?
                        (pw.pg[pw.cpg].ch - 3) : 
                        (pw.pg[pw.cpg].ch < 9 ?
                            (pw.pg[pw.cpg].ch - 6) :
                            (pw.pg[pw.cpg].ch < 12 ?
                                (pw.pg[pw.cpg].ch - 9) :
                                (pw.pg[pw.cpg].ch < 15 ?
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
            int m = (pw.pg[pw.cpg].chip is YM2203) ? 0 : 3;
            vch = (byte)(pw.pg[pw.cpg].ch - (m + 6));
            port = 0;
            adr = 0;

            if (!(pw.pg[pw.cpg].chip is YM2609)) return;

            if (pw.pg[pw.cpg].ch >= 18 && pw.pg[pw.cpg].ch <= 20)
            {
                port = 0;
                vch = (byte)(pw.pg[pw.cpg].ch - 18);
                adr = 0;
            }
            else if (pw.pg[pw.cpg].ch >= 21 && pw.pg[pw.cpg].ch <= 23)
            {
                port = 1;
                vch = (byte)(pw.pg[pw.cpg].ch - 21);
                adr = 0x20;
            }
            else if (pw.pg[pw.cpg].ch >= 24 && pw.pg[pw.cpg].ch <= 26)
            {
                port = 2;
                vch = (byte)(pw.pg[pw.cpg].ch - 24);
                adr = 0;
            }
            else if (pw.pg[pw.cpg].ch >= 27 && pw.pg[pw.cpg].ch <= 29)
            {
                port = 2;
                vch = (byte)(pw.pg[pw.cpg].ch - 27);
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
                if (algs[alg][i] == 0 || (pw.pg[pw.cpg].slots & (1 << i)) == 0)
                {
                    ope[i] = -1;
                    continue;
                }
                ope[i] = ope[i] + (127 - vol);
                ope[i] = Common.CheckRange(ope[i], 0, 127);
            }

            partWork vpw = pw;
            if (!(pw.pg[pw.cpg].chip is YM2609))
            {
                int m = (pw.pg[pw.cpg].chip is YM2203) ? 0 : 3;
                if (pw.pg[pw.cpg].chip.lstPartWork[2].pg[pw.cpg].Ch3SpecialMode && pw.pg[pw.cpg].ch >= m + 3 && pw.pg[pw.cpg].ch < m + 6)
                {
                    vpw = pw.pg[pw.cpg].chip.lstPartWork[2];
                }
            }
            else
            {
                if (pw.pg[pw.cpg].chip.lstPartWork[2].pg[pw.cpg].Ch3SpecialMode && pw.pg[pw.cpg].ch >= 12 && pw.pg[pw.cpg].ch < 15)
                {
                    vpw = pw.pg[pw.cpg].chip.lstPartWork[2];
                }
                if (pw.pg[pw.cpg].chip.lstPartWork[8].pg[pw.cpg].Ch3SpecialMode && pw.pg[pw.cpg].ch >= 15 && pw.pg[pw.cpg].ch < 18)
                {
                    vpw = pw.pg[pw.cpg].chip.lstPartWork[8];
                }
            }

            MML vmml = new MML();
            vmml.args = new List<object>();
            vmml.args.Add(vol);
            vmml.type = enmMMLType.Volume;
            if (mml != null)
                vmml.line = mml.line;
            if ((pw.pg[pw.cpg].slots & 1) != 0 && ope[0] != -1) ((ClsOPN)pw.pg[pw.cpg].chip).OutFmSetTl(vmml, vpw, 0, ope[0]);
            if ((pw.pg[pw.cpg].slots & 2) != 0 && ope[1] != -1) ((ClsOPN)pw.pg[pw.cpg].chip).OutFmSetTl(vmml, vpw, 1, ope[1]);
            if ((pw.pg[pw.cpg].slots & 4) != 0 && ope[2] != -1) ((ClsOPN)pw.pg[pw.cpg].chip).OutFmSetTl(vmml, vpw, 2, ope[2]);
            if ((pw.pg[pw.cpg].slots & 8) != 0 && ope[3] != -1) ((ClsOPN)pw.pg[pw.cpg].chip).OutFmSetTl(vmml, vpw, 3, ope[3]);
            //if ((pw.ppg[pw.cpgNum].slots & 1) != 0 ) ((ClsOPN)pw.ppg[pw.cpgNum].chip).OutFmSetTl(vpw, 0, ope[0]);
            //if ((pw.ppg[pw.cpgNum].slots & 2) != 0 ) ((ClsOPN)pw.ppg[pw.cpgNum].chip).OutFmSetTl(vpw, 1, ope[1]);
            //if ((pw.ppg[pw.cpgNum].slots & 4) != 0 ) ((ClsOPN)pw.ppg[pw.cpgNum].chip).OutFmSetTl(vpw, 2, ope[2]);
            //if ((pw.ppg[pw.cpgNum].slots & 8) != 0 ) ((ClsOPN)pw.ppg[pw.cpgNum].chip).OutFmSetTl(vpw, 3, ope[3]);
        }

        public void OutFmCh3SpecialModeSetFnum(MML mml,partWork pw, byte ope, int octave, int num)
        {
            ope &= 3;
            if (ope == 0)
            {
                parent.OutData(mml,pw.pg[pw.cpg].port[0], 0xa6, (byte)(((num & 0x700) >> 8) + (((octave - 1) & 0x7) << 3)));
                parent.OutData(mml,pw.pg[pw.cpg].port[0], 0xa2, (byte)(num & 0xff));
            }
            else
            {
                parent.OutData(mml,pw.pg[pw.cpg].port[0], (byte)(0xac + ope), (byte)(((num & 0x700) >> 8) + (((octave - 1) & 0x7) << 3)));
                parent.OutData(mml,pw.pg[pw.cpg].port[0], (byte)(0xa8 + ope), (byte)(num & 0xff));
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

            int m = (pw.pg[pw.cpg].chip is YM2203) ? 0 : 3;

            if (
                ((pw.pg[pw.cpg].chip is YM2203) && pw.pg[pw.cpg].ch >= 3 && pw.pg[pw.cpg].ch < 6)
                || ((pw.pg[pw.cpg].chip is YM2608) && pw.pg[pw.cpg].ch >= 6 && pw.pg[pw.cpg].ch < 9)
                || ((pw.pg[pw.cpg].chip is YM2609) && pw.pg[pw.cpg].ch >= 12 && pw.pg[pw.cpg].ch < 18)
                || ((pw.pg[pw.cpg].chip is YM2610B) && pw.pg[pw.cpg].ch >= 6 && pw.pg[pw.cpg].ch < 9)
                || ((pw.pg[pw.cpg].chip is YM2612) && pw.pg[pw.cpg].ch >= 6 && pw.pg[pw.cpg].ch < 9)
                || ((pw.pg[pw.cpg].chip is YM2612X) && pw.pg[pw.cpg].ch >= 6 && pw.pg[pw.cpg].ch < 9)
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
                    for (int ope = 0; ope < 4; ope++) ((ClsOPN)pw.pg[pw.cpg].chip).OutFmSetSlRr(mml, pw, ope, 0, 15);
                    break;
                case 2: // A)ll
                    for (int ope = 0; ope < 4; ope++)
                    {
                        ((ClsOPN)pw.pg[pw.cpg].chip).OutFmSetDtMl(mml, pw, ope, 0, 0);
                        ((ClsOPN)pw.pg[pw.cpg].chip).OutFmSetKsAr(mml, pw, ope, 3, 31);
                        ((ClsOPN)pw.pg[pw.cpg].chip).OutFmSetAmDr(mml, pw, ope, 1, 31);
                        ((ClsOPN)pw.pg[pw.cpg].chip).OutFmSetSr(mml, pw, ope, 31);
                        ((ClsOPN)pw.pg[pw.cpg].chip).OutFmSetSlRr(mml, pw, ope, 0, 15);
                        ((ClsOPN)pw.pg[pw.cpg].chip).OutFmSetSSGEG(mml, pw, ope, 0);
                    }
                    ((ClsOPN)pw.pg[pw.cpg].chip).OutFmSetFeedbackAlgorithm(mml, pw, 7, 7);
                    break;
            }


            for (int ope = 0; ope < 4; ope++)
            {
                //ch3以外の拡張チャンネルでも音色設定できるようにする場合はslotの様子もみてセットすること
                ((ClsOPN)pw.pg[pw.cpg].chip).OutFmSetDtMl(mml, pw, ope, parent.instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], parent.instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 8]);
                ((ClsOPN)pw.pg[pw.cpg].chip).OutFmSetKsAr(mml, pw, ope, parent.instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 7], parent.instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 1]);
                ((ClsOPN)pw.pg[pw.cpg].chip).OutFmSetAmDr(mml, pw, ope, parent.instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 10], parent.instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 2]);
                ((ClsOPN)pw.pg[pw.cpg].chip).OutFmSetSr(mml, pw, ope, parent.instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 3]);
                ((ClsOPN)pw.pg[pw.cpg].chip).OutFmSetSlRr(mml, pw, ope, parent.instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 5], parent.instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 4]);
                ((ClsOPN)pw.pg[pw.cpg].chip).OutFmSetSSGEG(mml, pw, ope, parent.instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 11]);
            }
            //ch3以外の拡張チャンネルでも音色設定できるようにする場合はslotの様子もみてセットすること
            pw.pg[pw.cpg].op1ml = parent.instFM[n][0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
            pw.pg[pw.cpg].op2ml = parent.instFM[n][1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
            pw.pg[pw.cpg].op3ml = parent.instFM[n][2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
            pw.pg[pw.cpg].op4ml = parent.instFM[n][3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
            //ch3以外の拡張チャンネルでも音色設定できるようにする場合はslotの様子もみてセットすること
            pw.pg[pw.cpg].op1dt2 = 0;
            pw.pg[pw.cpg].op2dt2 = 0;
            pw.pg[pw.cpg].op3dt2 = 0;
            pw.pg[pw.cpg].op4dt2 = 0;

            ((ClsOPN)pw.pg[pw.cpg].chip).OutFmSetFeedbackAlgorithm(mml, pw, parent.instFM[n][46], parent.instFM[n][45]);

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
                if (algs[alg][i] == 0)// || (pw.ppg[pw.cpgNum].slots & (1 << i)) == 0)
                {
                    op[i] = -1;
                    continue;
                }
                op[i] = Common.CheckRange(op[i], 0, 127);
            }

            partWork vpw = pw;
            if (!(pw.pg[pw.cpg].chip is YM2609))
            {
                if (pw.pg[pw.cpg].chip.lstPartWork[2].pg[pw.cpg].Ch3SpecialMode && pw.pg[pw.cpg].ch >= m + 3 && pw.pg[pw.cpg].ch < m + 6)
                {
                    vpw = pw.pg[pw.cpg].chip.lstPartWork[2];
                }
            }
            else
            {
                if (pw.pg[pw.cpg].chip.lstPartWork[2].pg[pw.cpg].Ch3SpecialMode && pw.pg[pw.cpg].ch >= 12 && pw.pg[pw.cpg].ch < 15)
                {
                    vpw = pw.pg[pw.cpg].chip.lstPartWork[2];
                }
                if (pw.pg[pw.cpg].chip.lstPartWork[8].pg[pw.cpg].Ch3SpecialMode && pw.pg[pw.cpg].ch >= 15 && pw.pg[pw.cpg].ch < 18)
                {
                    vpw = pw.pg[pw.cpg].chip.lstPartWork[8];
                }
            }

            //ch3以外の拡張チャンネルでも音色設定できるようになったら以下を有効に
            //if ((pw.ppg[pw.cpgNum].slots & 1) != 0 && op[0] != -1) ((ClsOPN)pw.ppg[pw.cpgNum].chip).OutFmSetTl(mml, vpw, 0, op[0]);
            //if ((pw.ppg[pw.cpgNum].slots & 2) != 0 && op[1] != -1) ((ClsOPN)pw.ppg[pw.cpgNum].chip).OutFmSetTl(mml, vpw, 1, op[1]);
            //if ((pw.ppg[pw.cpgNum].slots & 4) != 0 && op[2] != -1) ((ClsOPN)pw.ppg[pw.cpgNum].chip).OutFmSetTl(mml, vpw, 2, op[2]);
            //if ((pw.ppg[pw.cpgNum].slots & 8) != 0 && op[3] != -1) ((ClsOPN)pw.ppg[pw.cpgNum].chip).OutFmSetTl(mml, vpw, 3, op[3]);
            if (op[0] != -1) ((ClsOPN)pw.pg[pw.cpg].chip).OutFmSetTl(mml, vpw, 0, op[0]);
            if (op[1] != -1) ((ClsOPN)pw.pg[pw.cpg].chip).OutFmSetTl(mml, vpw, 1, op[1]);
            if (op[2] != -1) ((ClsOPN)pw.pg[pw.cpg].chip).OutFmSetTl(mml, vpw, 2, op[2]);
            if (op[3] != -1) ((ClsOPN)pw.pg[pw.cpg].chip).OutFmSetTl(mml, vpw, 3, op[3]);


            //音量を再セットする

            OutFmSetVolume(pw, mml, vol, n);

            //拡張チャンネルの場合は他の拡張チャンネルも音量を再セットする
            if (pw.pg[pw.cpg].Type == enmChannelType.FMOPNex)
            {
                if (!(pw.pg[pw.cpg].chip is YM2609))
                {
                    if (pw.pg[pw.cpg].ch != 2) OutFmSetVolume(pw.pg[pw.cpg].chip.lstPartWork[2], mml, pw.pg[pw.cpg].chip.lstPartWork[2].pg[pw.cpg].volume, n);
                    if (pw.pg[pw.cpg].ch != m + 3) OutFmSetVolume(pw.pg[pw.cpg].chip.lstPartWork[m + 3], mml, pw.pg[pw.cpg].chip.lstPartWork[m + 3].pg[pw.cpg].volume, n);
                    if (pw.pg[pw.cpg].ch != m + 4) OutFmSetVolume(pw.pg[pw.cpg].chip.lstPartWork[m + 4], mml, pw.pg[pw.cpg].chip.lstPartWork[m + 4].pg[pw.cpg].volume, n);
                    if (pw.pg[pw.cpg].ch != m + 5) OutFmSetVolume(pw.pg[pw.cpg].chip.lstPartWork[m + 5], mml, pw.pg[pw.cpg].chip.lstPartWork[m + 5].pg[pw.cpg].volume, n);
                }
                else
                {
                    if (pw.pg[pw.cpg].ch == 2 || pw.pg[pw.cpg].ch == 12 || pw.pg[pw.cpg].ch == 13 || pw.pg[pw.cpg].ch == 14)
                    {
                        //YM2609 ch3 || ch13 || ch14 || ch15
                        if (pw.pg[pw.cpg].ch != 2) OutFmSetVolume(pw.pg[pw.cpg].chip.lstPartWork[2], mml, pw.pg[pw.cpg].chip.lstPartWork[2].pg[pw.cpg].volume, n);
                        if (pw.pg[pw.cpg].ch != 12) OutFmSetVolume(pw.pg[pw.cpg].chip.lstPartWork[12], mml, pw.pg[pw.cpg].chip.lstPartWork[12].pg[pw.cpg].volume, n);
                        if (pw.pg[pw.cpg].ch != 13) OutFmSetVolume(pw.pg[pw.cpg].chip.lstPartWork[13], mml, pw.pg[pw.cpg].chip.lstPartWork[13].pg[pw.cpg].volume, n);
                        if (pw.pg[pw.cpg].ch != 14) OutFmSetVolume(pw.pg[pw.cpg].chip.lstPartWork[14], mml, pw.pg[pw.cpg].chip.lstPartWork[14].pg[pw.cpg].volume, n);
                    }
                    else
                    {
                        //YM2609 ch9 || ch16 || ch17 || ch18
                        if (pw.pg[pw.cpg].ch != 8) OutFmSetVolume(pw.pg[pw.cpg].chip.lstPartWork[8], mml, pw.pg[pw.cpg].chip.lstPartWork[8].pg[pw.cpg].volume, n);
                        if (pw.pg[pw.cpg].ch != 15) OutFmSetVolume(pw.pg[pw.cpg].chip.lstPartWork[15], mml, pw.pg[pw.cpg].chip.lstPartWork[15].pg[pw.cpg].volume, n);
                        if (pw.pg[pw.cpg].ch != 16) OutFmSetVolume(pw.pg[pw.cpg].chip.lstPartWork[16], mml, pw.pg[pw.cpg].chip.lstPartWork[16].pg[pw.cpg].volume, n);
                        if (pw.pg[pw.cpg].ch != 17) OutFmSetVolume(pw.pg[pw.cpg].chip.lstPartWork[17], mml, pw.pg[pw.cpg].chip.lstPartWork[17].pg[pw.cpg].volume, n);
                    }
                }
            }

        }

        public void OutFmKeyOff(partWork pw, MML mml)
        {
            int n = (pw.pg[pw.cpg].chip is YM2203) ? 0 : 3;

            if (pw.pg[pw.cpg].chip is YM2612X && (pw.pg[pw.cpg].ch > 8 || pw.pg[pw.cpg].ch == 5) && pw.pg[pw.cpg].pcm)
            {
                ((YM2612X)pw.pg[pw.cpg].chip).OutYM2612XPcmKeyOFF(mml, pw);
                return;
            }

            if (!pw.pg[pw.cpg].pcm)
            {
                if (!(pw.pg[pw.cpg].chip is YM2609))
                {
                    if (pw.pg[pw.cpg].chip.lstPartWork[2].pg[pw.cpg].Ch3SpecialMode && pw.pg[pw.cpg].Type == enmChannelType.FMOPNex)
                    {
                        pw.pg[pw.cpg].Ch3SpecialModeKeyOn = false;

                        int slot = (pw.pg[pw.cpg].chip.lstPartWork[2].pg[pw.cpg].Ch3SpecialModeKeyOn ? pw.pg[pw.cpg].chip.lstPartWork[2].pg[pw.cpg].slots : 0x0)
                            | (pw.pg[pw.cpg].chip.lstPartWork[n + 3].pg[pw.cpg].Ch3SpecialModeKeyOn ? pw.pg[pw.cpg].chip.lstPartWork[n + 3].pg[pw.cpg].slots : 0x0)
                            | (pw.pg[pw.cpg].chip.lstPartWork[n + 4].pg[pw.cpg].Ch3SpecialModeKeyOn ? pw.pg[pw.cpg].chip.lstPartWork[n + 4].pg[pw.cpg].slots : 0x0)
                            | (pw.pg[pw.cpg].chip.lstPartWork[n + 5].pg[pw.cpg].Ch3SpecialModeKeyOn ? pw.pg[pw.cpg].chip.lstPartWork[n + 5].pg[pw.cpg].slots : 0x0);

                        parent.OutData(mml, pw.pg[pw.cpg].port[0], 0x28, (byte)((slot << 4) + 2));
                    }
                    else
                    {
                        if (pw.pg[pw.cpg].ch >= 0 && pw.pg[pw.cpg].ch < n + 3)
                        {
                            byte vch = (byte)((pw.pg[pw.cpg].ch > 2) ? pw.pg[pw.cpg].ch + 1 : pw.pg[pw.cpg].ch);
                            //key off
                            parent.OutData(mml, pw.pg[pw.cpg].port[0], 0x28, (byte)(0x00 + (vch & 7)));
                        }
                    }
                }
                else
                {
                    if ((pw.pg[pw.cpg].ch == 2 || pw.pg[pw.cpg].ch == 12 
                        || pw.pg[pw.cpg].ch == 13 || pw.pg[pw.cpg].ch == 14) 
                        && pw.pg[pw.cpg].chip.lstPartWork[2].pg[pw.cpg].Ch3SpecialMode)
                    {
                        pw.pg[pw.cpg].Ch3SpecialModeKeyOn = false;

                        int slot = (pw.pg[pw.cpg].chip.lstPartWork[2].pg[pw.cpg].Ch3SpecialModeKeyOn ? pw.pg[pw.cpg].chip.lstPartWork[2].pg[pw.cpg].slots : 0x0)
                            | (pw.pg[pw.cpg].chip.lstPartWork[12].pg[pw.cpg].Ch3SpecialModeKeyOn ? pw.pg[pw.cpg].chip.lstPartWork[12].pg[pw.cpg].slots : 0x0)
                            | (pw.pg[pw.cpg].chip.lstPartWork[13].pg[pw.cpg].Ch3SpecialModeKeyOn ? pw.pg[pw.cpg].chip.lstPartWork[13].pg[pw.cpg].slots : 0x0)
                            | (pw.pg[pw.cpg].chip.lstPartWork[14].pg[pw.cpg].Ch3SpecialModeKeyOn ? pw.pg[pw.cpg].chip.lstPartWork[14].pg[pw.cpg].slots : 0x0);

                        outDatum od = new outDatum();
                        od.val = (byte)((slot << 4) + 2);
                        if (mml != null)
                        {
                            od.type = mml.type;
                            if (mml.line != null && mml.line.Lp != null)
                            {
                                od.linePos = new LinePos(
                                    mml.line.Lp.srcMMLID,
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
                        ((YM2609)pw.pg[pw.cpg].chip).opna20x028KeyOnData.Add(od);
                    }
                    else if ((pw.pg[pw.cpg].ch == 8 || pw.pg[pw.cpg].ch == 15 || pw.pg[pw.cpg].ch == 16 || pw.pg[pw.cpg].ch == 17) && pw.pg[pw.cpg].chip.lstPartWork[8].pg[pw.cpg].Ch3SpecialMode)
                    {
                        pw.pg[pw.cpg].Ch3SpecialModeKeyOn = false;

                        int slot = (pw.pg[pw.cpg].chip.lstPartWork[8].pg[pw.cpg].Ch3SpecialModeKeyOn ? pw.pg[pw.cpg].chip.lstPartWork[8].pg[pw.cpg].slots : 0x0)
                            | (pw.pg[pw.cpg].chip.lstPartWork[15].pg[pw.cpg].Ch3SpecialModeKeyOn ? pw.pg[pw.cpg].chip.lstPartWork[15].pg[pw.cpg].slots : 0x0)
                            | (pw.pg[pw.cpg].chip.lstPartWork[16].pg[pw.cpg].Ch3SpecialModeKeyOn ? pw.pg[pw.cpg].chip.lstPartWork[16].pg[pw.cpg].slots : 0x0)
                            | (pw.pg[pw.cpg].chip.lstPartWork[17].pg[pw.cpg].Ch3SpecialModeKeyOn ? pw.pg[pw.cpg].chip.lstPartWork[17].pg[pw.cpg].slots : 0x0);

                        outDatum od = new outDatum();
                        od.val = (byte)((slot << 4) + 2);
                        if (mml != null)
                        {
                            od.type = mml.type;
                            if (mml.line != null && mml.line.Lp != null)
                            {
                                od.linePos = new LinePos(
                                    mml.line.Lp.srcMMLID,
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
                        ((YM2609)pw.pg[pw.cpg].chip).opna20x228KeyOnData.Add(od);
                    }
                    else
                    {
                        if (pw.pg[pw.cpg].ch >= 0 && pw.pg[pw.cpg].ch < 6)
                        {
                            byte vch = (byte)((pw.pg[pw.cpg].ch > 2) ? pw.pg[pw.cpg].ch + 1 : pw.pg[pw.cpg].ch);
                            //key off
                            outDatum od = new outDatum();
                            od.val = (byte)(0x00 + (vch & 7));
                            if (mml != null)
                            {
                                od.type = mml.type;
                                if (mml.line != null && mml.line.Lp != null)
                                {
                                    od.linePos = new LinePos(
                                        mml.line.Lp.srcMMLID,
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
                            ((YM2609)pw.pg[pw.cpg].chip).opna20x028KeyOnData.Add(od);
                        }
                        else if (pw.pg[pw.cpg].ch >= 6 && pw.pg[pw.cpg].ch < 12)
                        {
                            byte vch = (byte)(pw.pg[pw.cpg].ch - 6);
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
                                        mml.line.Lp.srcMMLID,
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
                            ((YM2609)pw.pg[pw.cpg].chip).opna20x228KeyOnData.Add(od);
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
                    parent.OutData(mml, cmd, (byte)pw.pg[pw.cpg].streamID);
                }
            }

            pw.pg[pw.cpg].pcmWaitKeyOnCounter = -1;
        }

        public void OutFmAllKeyOff()
        {

            foreach (partWork pw in lstPartWork)
            {
                if (!(pw.pg[pw.cpg].chip is YM2609)) { if (pw.pg[pw.cpg].ch > 5) continue; }
                else if (pw.pg[pw.cpg].ch > 11) continue;

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

            if (freq == pw.pg[pw.cpg].freq) return;

            pw.pg[pw.cpg].freq = freq;

            if (pw.pg[pw.cpg].chip.lstPartWork[2].pg[pw.cpg].Ch3SpecialMode && pw.pg[pw.cpg].Type == enmChannelType.FMOPNex)
            {
                if ((pw.pg[pw.cpg].slots & 8) != 0)
                {
                    int f = pw.pg[pw.cpg].freq + pw.pg[pw.cpg].slotDetune[3];
                    parent.OutData(mml, pw.pg[pw.cpg].port[0], (byte)0xa6, (byte)(f >> 8));
                    parent.OutData(mml, pw.pg[pw.cpg].port[0], (byte)0xa2, (byte)f);
                }
                if ((pw.pg[pw.cpg].slots & 4) != 0)
                {
                    int f = pw.pg[pw.cpg].freq + pw.pg[pw.cpg].slotDetune[2];
                    parent.OutData(mml, pw.pg[pw.cpg].port[0], (byte)0xac, (byte)(f >> 8));
                    parent.OutData(mml, pw.pg[pw.cpg].port[0], (byte)0xa8, (byte)f);
                }
                if ((pw.pg[pw.cpg].slots & 1) != 0)
                {
                    int f = pw.pg[pw.cpg].freq + pw.pg[pw.cpg].slotDetune[0];
                    parent.OutData(mml, pw.pg[pw.cpg].port[0], (byte)0xad, (byte)(f >> 8));
                    parent.OutData(mml, pw.pg[pw.cpg].port[0], (byte)0xa9, (byte)f);
                }
                if ((pw.pg[pw.cpg].slots & 2) != 0)
                {
                    int f = pw.pg[pw.cpg].freq + pw.pg[pw.cpg].slotDetune[1];
                    parent.OutData(mml, pw.pg[pw.cpg].port[0], (byte)0xae, (byte)(f >> 8));
                    parent.OutData(mml, pw.pg[pw.cpg].port[0], (byte)0xaa, (byte)f);
                }
            }
            else
            {
                int n;
                if (!(pw.pg[pw.cpg].chip is YM2609))
                {
                    n = (pw.pg[pw.cpg].chip is YM2203) ? 0 : 3;
                    if (pw.pg[pw.cpg].ch >= n + 3 && pw.pg[pw.cpg].ch < n + 6)
                    {
                        return;
                    }
                }
                else
                {
                    n = 9;
                    if (pw.pg[pw.cpg].ch >= 12 && pw.pg[pw.cpg].ch < 18)
                    {
                        return;
                    }
                }
                if ((pw.pg[pw.cpg].chip is YM2612X) && pw.pg[pw.cpg].ch >= 9 && pw.pg[pw.cpg].ch <= 11)
                {
                    return;
                }
                if (pw.pg[pw.cpg].ch < n + 3)
                {
                    if (pw.pg[pw.cpg].pcm) return;

                    int vch;
                    byte[] port;
                    GetPortVch(pw, out port, out vch);

                    parent.OutData(mml, port, (byte)(0xa4 + vch), (byte)((pw.pg[pw.cpg].freq & 0xff00) >> 8));
                    parent.OutData(mml, port, (byte)(0xa0 + vch), (byte)(pw.pg[pw.cpg].freq & 0xff));
                }
            }
        }

        public void OutFmKeyOn(partWork pw,MML mml)
        {
            SetDummyData(pw, mml);

            int n = (pw.pg[pw.cpg].chip is YM2203) ? 0 : 3;

            if (pw.pg[pw.cpg].chip is YM2612X && (pw.pg[pw.cpg].ch > 8 || pw.pg[pw.cpg].ch == 5) && pw.pg[pw.cpg].pcm)
            {
                ((YM2612X)pw.pg[pw.cpg].chip).OutYM2612XPcmKeyON(mml,pw);
                return;
            }

            if (!pw.pg[pw.cpg].pcm)
            {
                if (!(pw.pg[pw.cpg].chip is YM2609))
                {
                    if (pw.pg[pw.cpg].chip.lstPartWork[2].pg[pw.cpg].Ch3SpecialMode && pw.pg[pw.cpg].Type == enmChannelType.FMOPNex)
                    {
                        pw.pg[pw.cpg].Ch3SpecialModeKeyOn = true;

                        int slot = (pw.pg[pw.cpg].chip.lstPartWork[2].pg[pw.cpg].Ch3SpecialModeKeyOn ? pw.pg[pw.cpg].chip.lstPartWork[2].pg[pw.cpg].slots : 0x0)
                            | (pw.pg[pw.cpg].chip.lstPartWork[n + 3].pg[pw.cpg].Ch3SpecialModeKeyOn ? pw.pg[pw.cpg].chip.lstPartWork[n + 3].pg[pw.cpg].slots : 0x0)
                            | (pw.pg[pw.cpg].chip.lstPartWork[n + 4].pg[pw.cpg].Ch3SpecialModeKeyOn ? pw.pg[pw.cpg].chip.lstPartWork[n + 4].pg[pw.cpg].slots : 0x0)
                            | (pw.pg[pw.cpg].chip.lstPartWork[n + 5].pg[pw.cpg].Ch3SpecialModeKeyOn ? pw.pg[pw.cpg].chip.lstPartWork[n + 5].pg[pw.cpg].slots : 0x0);

                        if (pw.pg[pw.cpg].chip is YM2612X)
                        {
                            outDatum od = new outDatum();
                            od.val = (byte)((slot << 4) + 2);
                            if (mml != null)
                            {
                                od.type = mml.type;
                                if (mml.line != null && mml.line.Lp != null)
                                {
                                    od.linePos = new LinePos(
                                        mml.line.Lp.srcMMLID,
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
                            parent.OutData(mml, pw.pg[pw.cpg].port[0], 0x28, (byte)((slot << 4) + 2));
                        }
                    }
                    else
                    {
                        if (pw.pg[pw.cpg].ch >= 0 && pw.pg[pw.cpg].ch < n + 3)
                        {
                            byte vch = (byte)((pw.pg[pw.cpg].ch > 2) ? pw.pg[pw.cpg].ch + 1 : pw.pg[pw.cpg].ch);
                            if (pw.pg[pw.cpg].chip is YM2612X)
                            {
                                outDatum od = new outDatum();
                                od.val = (byte)((pw.pg[pw.cpg].slots << 4) + (vch & 7));
                                if (mml != null)
                                {
                                    od.type = mml.type;
                                    if (mml.line != null && mml.line.Lp != null)
                                    {
                                        od.linePos = new LinePos(
                                            mml.line.Lp.srcMMLID,
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
                                parent.OutData(mml, pw.pg[pw.cpg].port[0], 0x28, (byte)((pw.pg[pw.cpg].slots << 4) + (vch & 7)));
                            }
                        }
                    }
                }
                else
                {
                    if ((pw.pg[pw.cpg].ch == 2 || pw.pg[pw.cpg].ch == 12 || pw.pg[pw.cpg].ch == 13 || pw.pg[pw.cpg].ch == 14) && pw.pg[pw.cpg].chip.lstPartWork[2].pg[pw.cpg].Ch3SpecialMode)
                    {
                        pw.pg[pw.cpg].Ch3SpecialModeKeyOn = true;

                        int slot = (pw.pg[pw.cpg].chip.lstPartWork[2].pg[pw.cpg].Ch3SpecialModeKeyOn ? pw.pg[pw.cpg].chip.lstPartWork[2].pg[pw.cpg].slots : 0x0)
                            | (pw.pg[pw.cpg].chip.lstPartWork[12].pg[pw.cpg].Ch3SpecialModeKeyOn ? pw.pg[pw.cpg].chip.lstPartWork[12].pg[pw.cpg].slots : 0x0)
                            | (pw.pg[pw.cpg].chip.lstPartWork[13].pg[pw.cpg].Ch3SpecialModeKeyOn ? pw.pg[pw.cpg].chip.lstPartWork[13].pg[pw.cpg].slots : 0x0)
                            | (pw.pg[pw.cpg].chip.lstPartWork[14].pg[pw.cpg].Ch3SpecialModeKeyOn ? pw.pg[pw.cpg].chip.lstPartWork[14].pg[pw.cpg].slots : 0x0);

                        outDatum od = new outDatum();
                        od.val = (byte)((slot << 4) + 2);
                        if (mml != null)
                        {
                            od.type = mml.type;
                            if (mml.line != null && mml.line.Lp != null)
                            {
                                od.linePos = new LinePos(
                                    mml.line.Lp.srcMMLID,
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
                        ((YM2609)pw.pg[pw.cpg].chip).opna20x028KeyOnData.Add(od);

                    }
                    else if ((pw.pg[pw.cpg].ch == 8 || pw.pg[pw.cpg].ch == 15 || pw.pg[pw.cpg].ch == 16 || pw.pg[pw.cpg].ch == 17) && pw.pg[pw.cpg].chip.lstPartWork[8].pg[pw.cpg].Ch3SpecialMode)
                    {
                        pw.pg[pw.cpg].Ch3SpecialModeKeyOn = true;

                        int slot = (pw.pg[pw.cpg].chip.lstPartWork[8].pg[pw.cpg].Ch3SpecialModeKeyOn ? pw.pg[pw.cpg].chip.lstPartWork[8].pg[pw.cpg].slots : 0x0)
                            | (pw.pg[pw.cpg].chip.lstPartWork[15].pg[pw.cpg].Ch3SpecialModeKeyOn ? pw.pg[pw.cpg].chip.lstPartWork[15].pg[pw.cpg].slots : 0x0)
                            | (pw.pg[pw.cpg].chip.lstPartWork[16].pg[pw.cpg].Ch3SpecialModeKeyOn ? pw.pg[pw.cpg].chip.lstPartWork[16].pg[pw.cpg].slots : 0x0)
                            | (pw.pg[pw.cpg].chip.lstPartWork[17].pg[pw.cpg].Ch3SpecialModeKeyOn ? pw.pg[pw.cpg].chip.lstPartWork[17].pg[pw.cpg].slots : 0x0);

                        outDatum od = new outDatum();
                        od.val = (byte)((slot << 4) + 2);
                        if (mml != null)
                        {
                            od.type = mml.type;
                            if (mml.line != null && mml.line.Lp != null)
                            {
                                od.linePos = new LinePos(
                                    mml.line.Lp.srcMMLID,
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
                        ((YM2609)pw.pg[pw.cpg].chip).opna20x228KeyOnData.Add(od);
                    }
                    else
                    {
                        if (pw.pg[pw.cpg].ch >= 0 && pw.pg[pw.cpg].ch < 6)
                        {
                            byte vch = (byte)((pw.pg[pw.cpg].ch > 2) ? pw.pg[pw.cpg].ch + 1 : pw.pg[pw.cpg].ch);
                            //key on
                            outDatum od = new outDatum();
                            od.val = (byte)((pw.pg[pw.cpg].slots << 4) + (vch & 7));
                            if (mml != null)
                            {
                                od.type = mml.type;
                                if (mml.line != null && mml.line.Lp != null)
                                {
                                    od.linePos = new LinePos(
                                        mml.line.Lp.srcMMLID,
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
                            ((YM2609)pw.pg[pw.cpg].chip).opna20x028KeyOnData.Add(od);
                        }
                        else if (pw.pg[pw.cpg].ch >= 6 && pw.pg[pw.cpg].ch < 12)
                        {
                            byte vch = (byte)(pw.pg[pw.cpg].ch - 6);
                            vch = (byte)(((vch > 2) ? (vch + 1) : vch));
                            //key on
                            outDatum od = new outDatum();
                            od.val = (byte)((pw.pg[pw.cpg].slots << 4) + (vch & 7));
                            if (mml != null)
                            {
                                od.type = mml.type;
                                if (mml.line != null && mml.line.Lp != null)
                                {
                                    od.linePos = new LinePos(
                                        mml.line.Lp.srcMMLID,
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
                            ((YM2609)pw.pg[pw.cpg].chip).opna20x228KeyOnData.Add(od);
                        }
                    }
                }

                return;
            }


            if (pw.pg[pw.cpg].isPcmMap)
            {
                int nt = Const.NOTE.IndexOf(pw.pg[pw.cpg].noteCmd);
                int f = pw.pg[pw.cpg].octaveNow * 12 + nt + pw.pg[pw.cpg].shift + pw.pg[pw.cpg].keyShift;
                if (parent.instPCMMap.ContainsKey(pw.pg[pw.cpg].pcmMapNo))
                {
                    if (parent.instPCMMap[pw.pg[pw.cpg].pcmMapNo].ContainsKey(f))
                    {
                        pw.pg[pw.cpg].instrument = parent.instPCMMap[pw.pg[pw.cpg].pcmMapNo][f];
                    }
                    else
                    {
                        msgBox.setErrMsg(string.Format(msg.get("E10025"), pw.pg[pw.cpg].octaveNow, pw.pg[pw.cpg].noteCmd, pw.pg[pw.cpg].shift + pw.pg[pw.cpg].keyShift), mml.line.Lp);
                        return;
                    }
                }
                else
                {
                    msgBox.setErrMsg(string.Format(msg.get("E10024"), pw.pg[pw.cpg].pcmMapNo), mml.line.Lp);
                    return;
                }
            }

            if (!parent.instPCM.ContainsKey(pw.pg[pw.cpg].instrument)) return;

            float m = Const.pcmMTbl[pw.pg[pw.cpg].pcmNote] * (float)Math.Pow(2, (pw.pg[pw.cpg].pcmOctave - 4));
            pw.pg[pw.cpg].pcmBaseFreqPerFreq = Information.VGM_SAMPLE_PER_SECOND / ((float)parent.instPCM[pw.pg[pw.cpg].instrument].freq * m);
            pw.pg[pw.cpg].pcmFreqCountBuffer = 0.0f;
            long p = parent.instPCM[pw.pg[pw.cpg].instrument].stAdr;
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
                long s = parent.instPCM[pw.pg[pw.cpg].instrument].size;
                long f = parent.instPCM[pw.pg[pw.cpg].instrument].freq;
                long w = 0;
                if (pw.pg[pw.cpg].gatetimePmode)
                {
                    w = pw.pg[pw.cpg].waitCounter * pw.pg[pw.cpg].gatetime / 8L;
                }
                else
                {
                    w = pw.pg[pw.cpg].waitCounter - pw.pg[pw.cpg].gatetime;
                }
                if (w < 1) w = 1;
                s = Math.Min(s, (long)(w * parent.info.samplesPerClock * f / 44100.0));

                byte[] cmd;
                if (!pw.pg[pw.cpg].streamSetup)
                {
                    parent.newStreamID++;
                    pw.pg[pw.cpg].streamID = parent.newStreamID;

                    if (parent.info.format == enmFormat.ZGM)
                    {
                        if (parent.ChipCommandSize == 2) cmd = new byte[] {
                            0x30, 0x00
                            , (byte)pw.pg[pw.cpg].streamID
                            , (byte)pw.pg[pw.cpg].chip.ChipID
                            , (byte)(pw.pg[pw.cpg].chip.ChipID >> 8)
                        };
                        else cmd = new byte[] {
                            0x30
                            , (byte)pw.pg[pw.cpg].streamID
                            , (byte)pw.pg[pw.cpg].chip.ChipID
                        };
                    }
                    else cmd = new byte[] {
                        0x90
                        , (byte)pw.pg[pw.cpg].streamID
                        , (byte)(0x02 + (pw.pg[pw.cpg].chipNumber!=0 ? 0x80 : 0x00))
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
                        , (byte)pw.pg[pw.cpg].streamID
                        , 0x00
                        , 0x01
                        , 0x00
                        );

                    pw.pg[pw.cpg].streamSetup = true;
                }

                if (pw.pg[pw.cpg].streamFreq != f)
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
                        , (byte)pw.pg[pw.cpg].streamID
                        , (byte)(f & 0xff)
                        , (byte)((f & 0xff00) / 0x100)
                        , (byte)((f & 0xff0000) / 0x10000)
                        , (byte)((f & 0xff000000) / 0x10000)
                        );

                    pw.pg[pw.cpg].streamFreq = f;
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
                    , (byte)pw.pg[pw.cpg].streamID

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

            if (parent.instPCM[pw.pg[pw.cpg].instrument].status != enmPCMSTATUS.ERROR)
            {
                parent.instPCM[pw.pg[pw.cpg].instrument].status = enmPCMSTATUS.USED;
            }

        }


        public void SetFmFNum(partWork pw,MML mml)
        {
            if (pw.pg[pw.cpg].noteCmd == (char)0)
            {
                return;
            }

            int[] ftbl = pw.pg[pw.cpg].chip.FNumTbl[0];

            int f = GetFmFNum(ftbl, pw.pg[pw.cpg].octaveNow, pw.pg[pw.cpg].noteCmd, pw.pg[pw.cpg].shift + pw.pg[pw.cpg].keyShift + pw.pg[pw.cpg].toneDoublerKeyShift);//
            if (pw.pg[pw.cpg].bendWaitCounter != -1)
            {
                f = pw.pg[pw.cpg].bendFnum;
            }
            int o = (f & 0xf000) / 0x1000;
            f &= 0xfff;

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
            if (pw.pg[pw.cpg].Type == enmChannelType.FMOPN || pw.pg[pw.cpg].Type == enmChannelType.FMOPNex)
            {
                return GetFmFNum(FNumTbl[0], octave, cmd, shift);
            }
            if (pw.pg[pw.cpg].Type == enmChannelType.SSG)
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

            if (pw.pg[pw.cpg].beforeVolume != vol)
            {
                if (parent.instFM.ContainsKey(pw.pg[pw.cpg].instrument))
                {
                    OutFmSetVolume(pw, mml, vol, pw.pg[pw.cpg].instrument);
                    pw.pg[pw.cpg].beforeVolume = vol;
                }
            }
        }

        public override void SetKeyOff(partWork pw,MML mml)
        { }

        public override void SetVolume(partWork pw, MML mml)
        {
            if (pw.pg[pw.cpg].Type == enmChannelType.FMOPN
                || pw.pg[pw.cpg].Type == enmChannelType.FMOPNex //効果音モード対応チャンネル
                || (pw.pg[pw.cpg].Type == enmChannelType.FMPCM && !pw.pg[pw.cpg].pcm) //OPN2PCMチャンネル
                || (pw.pg[pw.cpg].Type == enmChannelType.FMPCMex && !pw.pg[pw.cpg].pcm) //OPN2XPCMチャンネル
                )
            {
                SetFmVolume(pw, mml);
            }
            else if (pw.pg[pw.cpg].Type == enmChannelType.SSG)
            {
                SetSsgVolume(pw, mml);
            }
        }

        public override void SetLfoAtKeyOn(partWork pw, MML mml)
        {
            for (int lfo = 0; lfo < 4; lfo++)
            {
                clsLfo pl = pw.pg[pw.cpg].lfo[lfo];
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
                    if (pw.pg[pw.cpg].Type == enmChannelType.FMOPN
                        || pw.pg[pw.cpg].Type == enmChannelType.FMOPNex)
                        SetFmFNum(pw, mml);
                    else if (pw.pg[pw.cpg].Type == enmChannelType.SSG)
                        SetSsgFNum(pw, mml);

                }

                if (pl.type == eLfoType.Tremolo)
                {
                    pw.pg[pw.cpg].beforeVolume = -1;
                    if (pw.pg[pw.cpg].Type == enmChannelType.FMOPN
                        || pw.pg[pw.cpg].Type == enmChannelType.FMOPNex)
                        SetFmVolume(pw, mml);
                    else if (pw.pg[pw.cpg].Type == enmChannelType.SSG)
                        SetSsgVolume(pw, mml);
                }

            }
        }

        public override void SetToneDoubler(partWork pw, MML mml)
        {
            if (pw.pg[pw.cpg].Type != enmChannelType.FMOPN && pw.pg[pw.cpg].ch != 2)
            {
                return;
            }

            int i = pw.pg[pw.cpg].instrument;
            if (i < 0) return;

            pw.pg[pw.cpg].toneDoublerKeyShift = 0;
            byte[] instFM = parent.instFM[i];
            if (instFM == null || instFM.Length < 1) return;
            Note note = (Note)mml.args[0];

            if (pw.pg[pw.cpg].TdA == -1)
            {
                //resetToneDoubler
                if (pw.pg[pw.cpg].op1ml != instFM[0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8])
                {
                    OutFmSetDtMl(mml, pw, 0, instFM[0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instFM[0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8]);
                    pw.pg[pw.cpg].op1ml = instFM[0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
                }
                if (pw.pg[pw.cpg].op2ml != instFM[1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8])
                {
                    OutFmSetDtMl(mml, pw, 1, instFM[1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instFM[1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8]);
                    pw.pg[pw.cpg].op2ml = instFM[1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
                }
                if (pw.pg[pw.cpg].op3ml != instFM[2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8])
                {
                    OutFmSetDtMl(mml, pw, 2, instFM[2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instFM[2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8]);
                    pw.pg[pw.cpg].op3ml = instFM[2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
                }
                if (pw.pg[pw.cpg].op4ml != instFM[3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8])
                {
                    OutFmSetDtMl(mml, pw, 3, instFM[3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instFM[3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8]);
                    pw.pg[pw.cpg].op4ml = instFM[3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
                }
            }
            else
            {
                //setToneDoubler
                int oct = pw.pg[pw.cpg].octaveNow;
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
                pw.pg[pw.cpg].octaveNew = oct;
                int TdB = oct * 12 + Const.NOTE.IndexOf(note.tDblCmd) + note.tDblShift + pw.pg[pw.cpg].keyShift;
                int s = TdB - pw.pg[pw.cpg].TdA;// - TdB;
                int us = Math.Abs(s);
                int n = pw.pg[pw.cpg].toneDoubler;
                clsToneDoubler instToneDoubler = parent.instToneDoubler[n];
                if (us >= instToneDoubler.lstTD.Count)
                {
                    return;
                }

                pw.pg[pw.cpg].toneDoublerKeyShift = ((s < 0) ? s : 0) + instToneDoubler.lstTD[us].KeyShift;

                if (pw.pg[pw.cpg].op1ml != instToneDoubler.lstTD[us].OP1ML)
                {
                    OutFmSetDtMl(mml, pw, 0, instFM[0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instToneDoubler.lstTD[us].OP1ML);
                    pw.pg[pw.cpg].op1ml = instToneDoubler.lstTD[us].OP1ML;
                }
                if (pw.pg[pw.cpg].op2ml != instToneDoubler.lstTD[us].OP2ML)
                {
                    OutFmSetDtMl(mml, pw, 1, instFM[1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instToneDoubler.lstTD[us].OP2ML);
                    pw.pg[pw.cpg].op2ml = instToneDoubler.lstTD[us].OP2ML;
                }
                if (pw.pg[pw.cpg].op3ml != instToneDoubler.lstTD[us].OP3ML)
                {
                    OutFmSetDtMl(mml, pw, 2, instFM[2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instToneDoubler.lstTD[us].OP3ML);
                    pw.pg[pw.cpg].op3ml = instToneDoubler.lstTD[us].OP3ML;
                }
                if (pw.pg[pw.cpg].op4ml != instToneDoubler.lstTD[us].OP4ML)
                {
                    OutFmSetDtMl(mml, pw, 3, instFM[3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instToneDoubler.lstTD[us].OP4ML);
                    pw.pg[pw.cpg].op4ml = instToneDoubler.lstTD[us].OP4ML;
                }

                //pw.ppg[pw.cpgNum].TdA = -1;
            }
        }

        public override int GetToneDoublerShift(partWork pw, int octave, char noteCmd, int shift)
        {
            if (pw.pg[pw.cpg].Type != enmChannelType.FMOPN && pw.pg[pw.cpg].ch != 2)
            {
                return 0;
            }

            int i = pw.pg[pw.cpg].instrument;
            if (pw.pg[pw.cpg].TdA == -1)
            {
                return 0;
            }

            int TdB = octave * 12 + Const.NOTE.IndexOf(noteCmd) + shift;
            int s = pw.pg[pw.cpg].TdA - TdB;
            int us = Math.Abs(s);
            int n = pw.pg[pw.cpg].toneDoubler;
            if (us >= parent.instToneDoubler[n].lstTD.Count)
            {
                return 0;
            }

            return ((s < 0) ? s : 0) + parent.instToneDoubler[n].lstTD[us].KeyShift;
        }


        private void CmdY_ToneParamOPN(MML mml,byte adr, partWork pw, byte op, byte dat)
        {
            int ch;
            if (pw.pg[pw.cpg].Type == enmChannelType.FMOPNex) ch = 2;
            else if (pw.pg[pw.cpg].Type == enmChannelType.FMOPN) ch = pw.pg[pw.cpg].ch;
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
            if (pw.pg[pw.cpg].Type == enmChannelType.FMOPNex) ch = 2;
            else if (pw.pg[pw.cpg].Type == enmChannelType.FMOPN) ch = pw.pg[pw.cpg].ch;
            else return;

            int vch;
            byte[] port;
            GetPortVch(pw, out port, out vch);

            byte adr = (byte)(0xb0 + vch);

            parent.OutData(mml,port, adr, dat);
        }


        public override void CmdNoiseToneMixer(partWork pw, MML mml)
        {
            if (pw.pg[pw.cpg].Type == enmChannelType.SSG)
            {
                int n = (int)mml.args[0];
                n = Common.CheckRange(n, 0, 3);
                pw.pg[pw.cpg].mixer = n;
            }
        }

        public override void CmdNoise(partWork pw, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 0, 31);

            int ch = 0;
            if (pw.pg[pw.cpg].chip is YM2609)
            {
                if (pw.pg[pw.cpg].ch >= 18 && pw.pg[pw.cpg].ch <= 20) ch = 18;
                if (pw.pg[pw.cpg].ch >= 21 && pw.pg[pw.cpg].ch <= 23) ch = 21;
                if (pw.pg[pw.cpg].ch >= 24 && pw.pg[pw.cpg].ch <= 26) ch = 24;
                if (pw.pg[pw.cpg].ch >= 27 && pw.pg[pw.cpg].ch <= 29) ch = 27;
            }

            pw.pg[pw.cpg].chip.lstPartWork[ ch ].pg[pw.cpg].noise = n;//各SSGChの1Chに保存
            ((ClsOPN)pw.pg[pw.cpg].chip).OutSsgNoise(mml,pw, n);
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
                pw.pg[pw.cpg].toneDoubler = n;
                return;
            }

            if (type == 'E')
            {
                SetEnvelopParamFromInstrument(pw, n, mml);
                return;
            }

            if (pw.pg[pw.cpg].Type == enmChannelType.SSG)
            {
                SetEnvelopParamFromInstrument(pw, n, mml);
                return;
            }

            n = Common.CheckRange(n, 0, 255);
            if (pw.pg[pw.cpg].instrument == n) return;
            pw.pg[pw.cpg].instrument = n;
            ((ClsOPN)pw.pg[pw.cpg].chip).OutFmSetInstrument(pw, mml, n, pw.pg[pw.cpg].volume, type);
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
                    if (pw.pg[pw.cpg].Type == enmChannelType.SSG)
                    {
                        pw.pg[pw.cpg].beforeVolume = -1;
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
                        pw.pg[pw.cpg].slotsEX = res;
                        if (pw.pg[pw.cpg].Ch3SpecialMode)
                        {
                            pw.pg[pw.cpg].slots = pw.pg[pw.cpg].slotsEX;
                            pw.pg[pw.cpg].beforeVolume = -1;
                        }
                    }
                    break;
                case "EXON":
                    pw.pg[pw.cpg].Ch3SpecialMode = true;
                    ((ClsOPN)pw.pg[pw.cpg].chip).OutOPNSetCh3SpecialMode(mml,pw, true);
                    foreach (partWork p in pw.pg[pw.cpg].chip.lstPartWork)
                    {
                        if(pw.pg[pw.cpg].chip.chipType== enmChipType.YM2609)
                        {
                            if (pw.pg[pw.cpg].ch == 2 || pw.pg[pw.cpg].ch == 12 || pw.pg[pw.cpg].ch == 13 || pw.pg[pw.cpg].ch == 14)
                                if (p.pg[pw.cpg].ch == 8 || p.pg[pw.cpg].ch == 15 || p.pg[pw.cpg].ch == 16 || p.pg[pw.cpg].ch == 17)
                                    continue;

                            if (pw.pg[pw.cpg].ch == 8 || pw.pg[pw.cpg].ch == 15 || pw.pg[pw.cpg].ch == 16 || pw.pg[pw.cpg].ch == 17)
                                if (p.pg[pw.cpg].ch == 2 || p.pg[pw.cpg].ch == 12 || p.pg[pw.cpg].ch == 13 || p.pg[pw.cpg].ch == 14)
                                    continue;
                        }

                        if (p.pg[pw.cpg].Type == enmChannelType.FMOPNex)
                        {
                            p.pg[pw.cpg].slots = p.pg[pw.cpg].slotsEX;
                            p.pg[pw.cpg].beforeVolume = -1;
                            p.pg[pw.cpg].beforeFNum = -1;
                            //p.freq = -1;
                            p.pg[pw.cpg].oldFreq = -1;
                            //SetFmFNum(p,mml);
                        }
                    }
                    break;
                case "EXOF":
                    pw.pg[pw.cpg].Ch3SpecialMode = false;
                    ((ClsOPN)pw.pg[pw.cpg].chip).OutOPNSetCh3SpecialMode(mml,pw, false);
                    foreach (partWork p in pw.pg[pw.cpg].chip.lstPartWork) {
                        if (p.pg[pw.cpg].Type == enmChannelType.FMOPNex)
                        {
                            p.pg[pw.cpg].beforeVolume = -1;
                            p.pg[pw.cpg].beforeFNum = -1;
                            p.pg[pw.cpg].freq = -1;

                            if (p.pg[pw.cpg].ch != 2 && (p.pg[pw.cpg].chip.chipType!=enmChipType.YM2609 || p.pg[pw.cpg].ch != 8)) // 2 -> Ch3   8 -> OPNA2のCh9のこと
                                p.pg[pw.cpg].slots = 0;
                            else
                            {
                                p.pg[pw.cpg].slots = p.pg[pw.cpg].slots4OP;
                                SetKeyOff(p, mml);
                            }

                            //SetFmFNum(p);
                        }
                    }
                    break;
                case "EXD":
                    pw.pg[pw.cpg].slotDetune[0] = (int)mml.args[1];
                    pw.pg[pw.cpg].slotDetune[1] = (int)mml.args[2];
                    pw.pg[pw.cpg].slotDetune[2] = (int)mml.args[3];
                    pw.pg[pw.cpg].slotDetune[3] = (int)mml.args[4];
                    break;
            }
        }

        public override void CmdVolume(partWork pw, MML mml)
        {
            base.CmdVolume(pw, mml);
        }

        public override void CmdRelativeVolumeSetting(partWork pw, MML mml)
        {
            if (relVol.ContainsKey(pw.pg[pw.cpg].ch))
                relVol.Remove(pw.pg[pw.cpg].ch);

            relVol.Add(pw.pg[pw.cpg].ch, (int)mml.args[0]);
        }

        public override int GetDefaultRelativeVolume(partWork pw, MML mml)
        {
            if (relVol.ContainsKey(pw.pg[pw.cpg].ch)) return relVol[pw.pg[pw.cpg].ch];

            return 1;
        }


        public override void CmdY(partWork pw, MML mml)
        {
            if (mml.args[0] is string toneparamName)
            {
                byte op = (byte)(int)mml.args[1];
                byte dat = (byte)(int)mml.args[2];

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
            if (pw.pg[pw.cpg].Type != enmChannelType.SSG) return;

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
                    if (pw.pg[pw.cpg].HardEnvelopeSpeed != n)
                    {
                        parent.OutData(mml, pw.pg[pw.cpg].port[port], (byte)(adr + 0x0b), (byte)(n & 0xff));
                        parent.OutData(mml, pw.pg[pw.cpg].port[port], (byte)(adr + 0x0c), (byte)((n >> 8) & 0xff));
                        pw.pg[pw.cpg].HardEnvelopeSpeed = n;
                    }
                    break;
                case "EHON":
                    pw.pg[pw.cpg].HardEnvelopeSw = true;
                    break;
                case "EHOF":
                    pw.pg[pw.cpg].HardEnvelopeSw = false;
                    break;
                case "EHT":
                    n = (int)mml.args[1];
                    if (pw.pg[pw.cpg].HardEnvelopeType != n)
                    {
                        parent.OutData(mml, pw.pg[pw.cpg].port[port], (byte)(adr + 0x0d), (byte)(n & 0xf));
                        pw.pg[pw.cpg].HardEnvelopeType = n;
                    }
                    break;
            }
        }

    }
}
