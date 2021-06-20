using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class YMF271 : ClsChip
    {
        //
        // Note:
        //
        // Groupは0～11まである
        // Group0/4/8 でのみPCMを使用可能
        // Sync2/3 で1slot又は4slot分のPCMを割り当てることができる
        // 上記より最大3*4=12個のPCMが発音可能
        //
        // また、Group0/4/8ではFMの基本波形にPCM波形を割り当てることが可能。
        //
        //
        // Sync0 4slot(FM) (1Groupで4Op形式のFM1個)
        // Sync1 2slotx2(FM) (1Groupで2Op形式のFM2個)
        // sync2 3slot(FM),1slot(PCM) (1Groupで3Op形式のFM1個と、PCM1個)
        // Sync3 1slot(PCM) (1Groupで4このPCM)

        // Ch01:Group01 01
        // Ch02:Group01 25
        // Ch03:Group01 13
        // Ch04:Group01 37
        // ...
        // Ch45:Group12 12
        // Ch46:Group12 36
        // Ch47:Group12 24
        // Ch48:Group12 48



        public class OPXchInfo
        {
            public readonly int group;
            public readonly int groupAdr;
            public readonly int slot;
            public readonly int bank;
            public readonly int bankB;

            public OPXchInfo(partPage page)
            {
                group = page.ch / 4;
                groupAdr = tblGrpNum[group];
                slot = tblSltNum[page.ch];
                bank = slot / 12;
                bankB = (bank & 1) != 0 ? 1 : 0;
            }
            public OPXchInfo(int ch)
            {
                group = ch / 4;
                groupAdr = tblGrpNum[group];
                slot = tblSltNum[ch];
                bank = slot / 12;
                bankB = (bank & 1) != 0 ? 1 : 0;
            }
        }


        private static readonly int[] tblGrpNum = new int[]
        {
            0,1,2,4,5,6,8,9,10,12,13,14
        };
        private static readonly int[] tblSltNum = new int[]
        {
            //bank 1 3 2 4 の順
            0,24,12,36,            1,25,13,37,            2,26,14,38,            3,27,15,39,
            4,28,16,40,            5,29,17,41,            6,30,18,42,            7,31,19,43,
            8,32,20,44,            9,33,21,45,            10,34,22,46,           11,35,23,47
        };
        private static readonly int[][] tblCM4 = new int[][]
        {
            //4OP 1 3 2 4
             new int[]{0,0,0,1 }
            ,new int[]{0,0,0,1 }
            ,new int[]{0,0,0,1 }
            ,new int[]{0,0,0,1 }

            ,new int[]{0,0,0,1 }
            ,new int[]{0,0,0,1 }
            ,new int[]{0,1,0,1 }
            ,new int[]{0,1,0,1 }

            ,new int[]{1,0,0,1 }
            ,new int[]{1,0,0,1 }
            ,new int[]{0,1,1,1 }
            ,new int[]{0,1,1,1 }
            ,new int[]{0,1,1,1 }
        };
        private static readonly int[][] tblCM3 = new int[][]
        {
            //3OP 1 3 2 
             new int[]{0,0,1 }
            ,new int[]{0,0,1 }
            ,new int[]{0,0,1 }
            ,new int[]{1,0,1 }

            ,new int[]{0,1,1 }
            ,new int[]{0,1,1 }
            ,new int[]{1,1,1 }
            ,new int[]{1,1,1 }
        };
        private static readonly int[][] tblCM2 = new int[][]
        {
            //2OP 1 3  / 2 4
             new int[]{0,1 }
            ,new int[]{0,1 }
            ,new int[]{1,1 }
            ,new int[]{1,1 }
        };



        public YMF271(ClsVgm parent, int chipID, string initialPartName, string stPath, int chipNumber) : base(parent, chipID, initialPartName, stPath, chipNumber)
        {

            _Name = "YMF271";
            _ShortName = "OPX";
            _ChMax = 48;
            _canUsePcm = true;

            Frequency = 16934400;
            port = new byte[][] { new byte[] { 0xd1 } };

            if (string.IsNullOrEmpty(initialPartName)) return;

            Dictionary<string, List<double>> dic = MakeFNumTbl();
            if (dic != null)
            {
                int c = 0;
                FNumTbl = new int[1][];
                FNumTbl[0] = new int[13];
                if (dic.Count != 0)
                {
                    foreach (double v in dic["FNUM_00"])
                    {
                        FNumTbl[0][c++] = (int)v;
                        if (c == FNumTbl[0].Length) break;
                    }
                    FNumTbl[0][FNumTbl[0].Length - 1] = FNumTbl[0][0] * 2;
                }
            }

            Ch = new ClsChannel[ChMax];
            SetPartToCh(Ch, initialPartName);

        }

        public override void InitPart(partWork pw)
        {
            foreach (partPage page in pw.pg)
            {
                page.beforeVolume = -1;
                page.volume = 110;
                page.MaxVolume = 127; // slot tl 7bit
                page.beforeEnvInstrument = 0;
                page.envInstrument = 0;
                page.port = port;
                page.mixer = 0;//?
                page.noise = 0;//?
                page.panL = 15;
                page.panR = 15;
                page.panRL = 0;
                page.panRR = 0;
                page.Type = enmChannelType.FMOPX;
            }
        }

        public override void InitChip()
        {

            if (!use) return;

            //* ここにChipに送る初期化コマンドを記述 *//

            //parent.OutData((MML)null, port[0], 0x05, 0x01);
            //FM All Off
            outAllKeyOff(null, lstPartWork[0]);


            //* ここに制御向け初期化を記述 *//

            //rhythmStatus = 0x00;
            //beforeRhythmStatus = 0xff;
            //connectionSel = 0;
            //beforeConnectionSel = -1;
            foreach (partWork pt in lstPartWork)
            {
                foreach (partPage pg in pt.pg)
                {
                    pg.sync = 0;
                }
            }

        }

        public void outAllKeyOff(MML mml, partWork pw)
        {
            // KeyOff, TotalLevel,ChLevel(panpot)をリセット
            for (byte adr = 0x0; adr < 0x10; adr++)
            {
                if ((adr & 3) == 0x3) continue;//0 1 2 4 5 6 8 9 A C D E のGroup12個

                byte dat = 0;
                //                                        bnk,adr,dat
                parent.OutData(mml, port[0], 0x00, (byte)(0x00 + adr), dat);//KEY OFF 拡張出力無視(EN:d7 EXTout:d6-d3)
                parent.OutData(mml, port[0], 0x01, (byte)(0x00 + adr), dat);//KEY OFF 拡張出力無視(EN:d7 EXTout:d6-d3)
                parent.OutData(mml, port[0], 0x02, (byte)(0x00 + adr), dat);//KEY OFF 拡張出力無視(EN:d7 EXTout:d6-d3)
                parent.OutData(mml, port[0], 0x03, (byte)(0x00 + adr), dat);//KEY OFF 拡張出力無視(EN:d7 EXTout:d6-d3)
                //                                        bnk,adr,dat
                parent.OutData(mml, port[0], 0x00, (byte)(0x40 + adr), dat);//TL 0
                parent.OutData(mml, port[0], 0x01, (byte)(0x40 + adr), dat);//TL 0
                parent.OutData(mml, port[0], 0x02, (byte)(0x40 + adr), dat);//TL 0
                parent.OutData(mml, port[0], 0x03, (byte)(0x40 + adr), dat);//TL 0
                //                                        bnk,adr,dat
                parent.OutData(mml, port[0], 0x00, (byte)(0xd0 + adr), dat);//CH0/CH1 0
                parent.OutData(mml, port[0], 0x01, (byte)(0xd0 + adr), dat);//CH0/CH1 0 シンクロモード無視
                parent.OutData(mml, port[0], 0x02, (byte)(0xd0 + adr), dat);//CH0/CH1 0 シンクロモード無視
                parent.OutData(mml, port[0], 0x03, (byte)(0xd0 + adr), dat);//CH0/CH1 0 シンクロモード無視
                //                                        bnk,adr,dat
                parent.OutData(mml, port[0], 0x00, (byte)(0xe0 + adr), dat);//CH2/CH3 0
                parent.OutData(mml, port[0], 0x01, (byte)(0xe0 + adr), dat);//CH2/CH3 0 シンクロモード無視
                parent.OutData(mml, port[0], 0x02, (byte)(0xe0 + adr), dat);//CH2/CH3 0 シンクロモード無視
                parent.OutData(mml, port[0], 0x03, (byte)(0xe0 + adr), dat);//CH2/CH3 0 シンクロモード無視

                // Bank6(Utility register)

                parent.OutData(mml, port[0], 0x06, (byte)(0x00 + adr), dat);//PFM 0 Sync 0
            }

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

            outOPXSetInstrument(page, mml, n, modeBeforeSend); //音色のセット
            page.envInstrument = 0;

        }

        /// <summary>
        /// sync切り替えコマンド 's' (本来はSusコマンド)
        /// </summary>
        public override void CmdSusOnOff(partPage page, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 0, 3);
            OPXchInfo info = new OPXchInfo(page);

            //groupが0/4/8以外はsync3(pcm only)を使えないようにする
            if (n == 3 && (info.group & 3) != 0)
            {
                msgBox.setErrMsg(string.Format(msg.get("E31004"), page.ch + 1), mml.line.Lp);
                return;
            }

            page.sync = n;
            parent.OutData(mml, port[0], 0x06, (byte)(0x00 + info.groupAdr), (byte)((page.pcm ? 0x80 : 00) | page.sync));//PFM | Sync
        }

        /// <summary>
        /// pcmモード切り替え
        /// </summary>
        public override void CmdMode(partPage page, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 0, 1);
            OPXchInfo info = new OPXchInfo(page);

            //groupが0/4/8以外はsync3(pcm only)を使えないようにする
            if ((info.group & 3) != 0)
            {
                return;//無視して終わり(エラーを出さない)
            }

            page.pcm = n != 0;
            parent.OutData(mml, port[0], 0x06, (byte)(0x00 + info.groupAdr), (byte)((page.pcm ? 0x80 : 00) | page.sync));//PFM | Sync
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
            //page.keyOn = false;
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
                    SetFNum(page, mml);
                }

                if (pl.type == eLfoType.Tremolo)
                {
                    page.beforeVolume = -1;
                    SetFmVolume(page, mml);
                }

                if (pl.type == eLfoType.Wah)
                {
                    page.beforeVolume = -1;
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


        public override void SetFNum(partPage page, MML mml)
        {
            if (page.noteCmd == (char)0)
            {
                return;
            }

            //if (page.forcedFnum != -1)
            //{
            //    SetFmForcedFNum(page, mml);
            //    return;
            //}

            int[] ftbl = page.chip.FNumTbl[0];
            int arpNote = page.arpFreqMode ? 0 : page.arpDelta;
            int arpFreq = page.arpFreqMode ? page.arpDelta : 0;
            int f;
            f = GetFmFNum(ftbl, page.octaveNow, page.noteCmd, page.shift + page.keyShift + page.toneDoublerKeyShift + arpNote);//
            if (page.bendWaitCounter != -1)
            {
                f = page.bendFnum;
            }
            int o = (f & 0xf000) / 0x1000;
            f &= 0xfff;

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
            f = Common.CheckRange(f, 0, 0xfff);
            o--;

            page.freq = (o << 12) | f;
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

        public void outOPXSetInstrument(partPage page, MML mml, int n, int modeBeforeSend)
        {
            page.instrument = n;

            if (!parent.instOPX.ContainsKey(n))
            {
                msgBox.setWrnMsg(string.Format(msg.get("E31000"), n), mml.line.Lp);
                return;
            }

            switch (parent.instOPX[n].Item2[0])
            {
                case 4:
                    SetInst4Operator(page, mml, n, modeBeforeSend, page.ch);
                    return;
                case 3:
                    SetInst3Operator(page, mml, n, modeBeforeSend, page.ch);
                    return;
                case 2:
                    SetInst2Operator(page, mml, n, modeBeforeSend, page.ch);
                    return;
            }

            msgBox.setErrMsg(string.Format(msg.get("E31002"), n), mml.line.Lp);
            page.instrument = -1;
        }

        private void SetInst4Operator(partPage page, MML mml, int n, int modeBeforeSend, int vch)
        {
            if (page.sync != 0)
            {
                msgBox.setErrMsg(string.Format(msg.get("E31003"), n), mml.line.Lp);
                return;
            }

            byte[] inst = parent.instOPX[n].Item2;
            OPXchInfo info = new OPXchInfo(vch);

            //byte targetBaseReg = ChnToBaseReg(vch);
            //byte[] port = getPortFromCh(vch);

            switch (modeBeforeSend)
            {
                case 0: // N)one
                    break;
                case 1: // R)R only
                        //if (page.opMode == 4)
                        //{
                    SOutData(page, mml, port[0], 0x00, (byte)(0x80 + info.groupAdr), 0x0f);//SL RR bank1
                    SOutData(page, mml, port[0], 0x02, (byte)(0x80 + info.groupAdr), 0x0f);//SL RR bank3
                    SOutData(page, mml, port[0], 0x01, (byte)(0x80 + info.groupAdr), 0x0f);//SL RR bank2
                    SOutData(page, mml, port[0], 0x03, (byte)(0x80 + info.groupAdr), 0x0f);//SL RR bank4
                    //}
                    //else if (page.opMode == 3)
                    //{
                    //    SOutData(page, mml, port[0], 0x00, (byte)(0x80 + grpAdr), 0x0f);//SL RR bank1
                    //    SOutData(page, mml, port[0], 0x02, (byte)(0x80 + grpAdr), 0x0f);//SL RR bank3
                    //    SOutData(page, mml, port[0], 0x01, (byte)(0x80 + grpAdr), 0x0f);//SL RR bank2
                    //}
                    //else if (page.opMode == 2)
                    //{
                    //    SOutData(page, mml, port[0], (byte)(0x00 + bnkB), (byte)(0x80 + grpAdr), 0x0f);//SL RR bank1/2
                    //    SOutData(page, mml, port[0], (byte)(0x02 + bnkB), (byte)(0x80 + grpAdr), 0x0f);//SL RR bank3/4
                    //}
                    break;
                case 2: // A)ll
                    //TBD
                    break;
            }

            //   AR  DR  SR  RR  SL  TL  KS  ML  DT  WF  ACC FB  LFO AMS PMS
            //'@ 031,001,001,001,005,026,000,001,002,000,000,000,000,000,000 ;S1
            page.algo = inst[62] & 0xf;
            SOutData(page, mml, port[0], 0, (byte)(0xc0 + info.groupAdr)//ALG
                , (byte)page.algo);

            for (int ope = 0; ope < 4; ope++)
            {
                byte opeN = (byte)(ope == 1 ? 2 : (ope == 2 ? 1 : ope));
                int opeP = 2 + ope * 15;

                SOutData(page, mml, port[0], opeN, (byte)(0x10 + info.groupAdr)//LFO(8)
                    , (byte)(inst[opeP + 12] & 0xff));
                SOutData(page, mml, port[0], opeN, (byte)(0x20 + info.groupAdr)//AMS(2) PMS(3) LFOWF(2)
                    , (byte)(((inst[opeP + 13] & 0x3) << 6) | (inst[opeP + 14] & 0x3)));
                SOutData(page, mml, port[0], opeN, (byte)(0x30 + info.groupAdr)//DT(3) ML(4)
                    , (byte)(((inst[opeP + 8] & 0x7) << 4) | (inst[opeP + 7] & 0xf)));
                SOutData(page, mml, port[0], opeN, (byte)(0x50 + info.groupAdr)//KS(3) AR(5)
                    , (byte)(((inst[opeP + 6] & 0x7) << 5) | (inst[opeP + 0] & 0x1f)));
                SOutData(page, mml, port[0], opeN, (byte)(0x60 + info.groupAdr)//DR(5)
                    , (byte)(inst[opeP + 1] & 0x1f));
                SOutData(page, mml, port[0], opeN, (byte)(0x70 + info.groupAdr)//SR(5)
                    , (byte)(inst[opeP + 2] & 0x1f));
                SOutData(page, mml, port[0], opeN, (byte)(0x80 + info.groupAdr)//SL(4) RR(4)
                    , (byte)(((inst[opeP + 4] & 0xf) << 4) | (inst[opeP + 3] & 0xf)));
                SOutData(page, mml, port[0], opeN, (byte)(0xb0 + info.groupAdr)//ACC(1) FB(3) WF(3)
                    , (byte)(((inst[opeP + 10] & 0x1) << 7)
                    | ((inst[opeP + 11] & 0x7) << 4)
                    | (inst[opeP + 9] & 0x7)));


                //TLはvolumeの設定と一緒に行うがキャリアのみである。
                //そのため、algからモジュレータのパラメータをセットする必要がある
                if (tblCM4[page.algo][ope] != 0) continue;

                SOutData(page, mml, port[0], opeN, (byte)(0x40 + info.groupAdr)//TL(7)
                    , (byte)(inst[opeP + 5] & 0x7f));
            }

            page.beforeVolume = -1;
        }

        private void SetInst3Operator(partPage page, MML mml, int n, int modeBeforeSend, int vch)
        {
            //sync チェック
            if (page.sync != 2)
            {
                msgBox.setErrMsg(string.Format(msg.get("E31005"), n), mml.line.Lp);
                return;
            }

            byte[] inst = parent.instOPX[n].Item2;
            OPXchInfo info = new OPXchInfo(vch);

            //bank チェック
            if (info.bank == 3)
            {
                msgBox.setErrMsg(string.Format(msg.get("E31008"), page.ch + 1, n), mml.line.Lp);
                return;
            }

            switch (modeBeforeSend)
            {
                case 0: // N)one
                    break;
                case 1: // R)R only
                    SOutData(page, mml, port[0], 0x00, (byte)(0x80 + info.groupAdr), 0x0f);//SL RR bank1
                    SOutData(page, mml, port[0], 0x02, (byte)(0x80 + info.groupAdr), 0x0f);//SL RR bank3
                    SOutData(page, mml, port[0], 0x01, (byte)(0x80 + info.groupAdr), 0x0f);//SL RR bank2
                    break;
                case 2: // A)ll
                    //TBD
                    break;
            }

            //   AR  DR  SR  RR  SL  TL  KS  ML  DT  WF  ACC FB  LFO AMS PMS
            //'@ 031,001,001,001,005,026,000,001,002,000,000,000,000,000,000 ;S1

            page.algo = inst[47] & 0xf;
            page.algo = Common.CheckRange(page.algo, 0, 7);
            SOutData(page, mml, port[0], 0, (byte)(0xc0 + info.groupAdr)//ALG
                , (byte)page.algo);

            for (int ope = 0; ope < 3; ope++)
            {
                byte opeN = (byte)(ope == 1 ? 2 : (ope == 2 ? 1 : ope));
                int opeP = 2 + ope * 15;

                SOutData(page, mml, port[0], opeN, (byte)(0x10 + info.groupAdr)//LFO(8)
                    , (byte)(inst[opeP + 12] & 0xff));
                SOutData(page, mml, port[0], opeN, (byte)(0x20 + info.groupAdr)//AMS(2) PMS(3) LFOWF(2)
                    , (byte)(((inst[opeP + 13] & 0x3) << 6) | (inst[opeP + 14] & 0x3)));
                SOutData(page, mml, port[0], opeN, (byte)(0x30 + info.groupAdr)//DT(3) ML(4)
                    , (byte)(((inst[opeP + 8] & 0x7) << 4) | (inst[opeP + 7] & 0xf)));
                SOutData(page, mml, port[0], opeN, (byte)(0x50 + info.groupAdr)//KS(3) AR(5)
                    , (byte)(((inst[opeP + 6] & 0x7) << 5) | (inst[opeP + 0] & 0x1f)));
                SOutData(page, mml, port[0], opeN, (byte)(0x60 + info.groupAdr)//DR(5)
                    , (byte)(inst[opeP + 1] & 0x1f));
                SOutData(page, mml, port[0], opeN, (byte)(0x70 + info.groupAdr)//SR(5)
                    , (byte)(inst[opeP + 2] & 0x1f));
                SOutData(page, mml, port[0], opeN, (byte)(0x80 + info.groupAdr)//SL(4) RR(4)
                    , (byte)(((inst[opeP + 4] & 0xf) << 4) | (inst[opeP + 3] & 0xf)));
                SOutData(page, mml, port[0], opeN, (byte)(0xb0 + info.groupAdr)//ACC(1) FB(3) WF(3)
                    , (byte)(((inst[opeP + 10] & 0x1) << 7)
                    | ((inst[opeP + 11] & 0x7) << 4)
                    | (inst[opeP + 9] & 0x7)));


                //TLはvolumeの設定と一緒に行うがキャリアのみである。
                //そのため、algからモジュレータのパラメータをセットする必要がある
                if (tblCM3[page.algo][ope] != 0) continue;

                SOutData(page, mml, port[0], opeN, (byte)(0x40 + info.groupAdr)//TL(7)
                    , (byte)(inst[opeP + 5] & 0x7f));
            }

            page.beforeVolume = -1;
        }

        private void SetInst2Operator(partPage page, MML mml, int n, int modeBeforeSend, int vch)
        {
            //sync チェック
            if (page.sync != 1)
            {
                msgBox.setErrMsg(string.Format(msg.get("E31006"), n), mml.line.Lp);
                return;
            }

            byte[] inst = parent.instOPX[n].Item2;
            OPXchInfo info = new OPXchInfo(vch);

            switch (modeBeforeSend)
            {
                case 0: // N)one
                    break;
                case 1: // R)R only
                    SOutData(page, mml, port[0], (byte)(0x00 + info.bankB), (byte)(0x80 + info.groupAdr), 0x0f);//SL RR bank1
                    SOutData(page, mml, port[0], (byte)(0x02 + info.bankB), (byte)(0x80 + info.groupAdr), 0x0f);//SL RR bank3
                    break;
                case 2: // A)ll
                    //TBD
                    break;
            }

            //   AR  DR  SR  RR  SL  TL  KS  ML  DT  WF  ACC FB  LFO AMS PMS
            //'@ 031,001,001,001,005,026,000,001,002,000,000,000,000,000,000 ;S1

            page.algo = inst[32] & 0xf;
            page.algo = Common.CheckRange(page.algo, 0, 3);
            SOutData(page, mml, port[0], (byte)info.bankB, (byte)(0xc0 + info.groupAdr)//ALG
                , (byte)page.algo);

            for (int ope = 0; ope < 2; ope++)
            {
                byte opeN = (byte)((ope == 1 ? 2 : (ope == 2 ? 1 : ope)) + info.bankB);
                int opeP = 2 + ope * 15;

                SOutData(page, mml, port[0], opeN, (byte)(0x10 + info.groupAdr)//LFO(8)
                    , (byte)(inst[opeP + 12] & 0xff));
                SOutData(page, mml, port[0], opeN, (byte)(0x20 + info.groupAdr)//AMS(2) PMS(3) LFOWF(2)
                    , (byte)(((inst[opeP + 13] & 0x3) << 6) | (inst[opeP + 14] & 0x3)));
                SOutData(page, mml, port[0], opeN, (byte)(0x30 + info.groupAdr)//DT(3) ML(4)
                    , (byte)(((inst[opeP + 8] & 0x7) << 4) | (inst[opeP + 7] & 0xf)));
                SOutData(page, mml, port[0], opeN, (byte)(0x50 + info.groupAdr)//KS(3) AR(5)
                    , (byte)(((inst[opeP + 6] & 0x7) << 5) | (inst[opeP + 0] & 0x1f)));
                SOutData(page, mml, port[0], opeN, (byte)(0x60 + info.groupAdr)//DR(5)
                    , (byte)(inst[opeP + 1] & 0x1f));
                SOutData(page, mml, port[0], opeN, (byte)(0x70 + info.groupAdr)//SR(5)
                    , (byte)(inst[opeP + 2] & 0x1f));
                SOutData(page, mml, port[0], opeN, (byte)(0x80 + info.groupAdr)//SL(4) RR(4)
                    , (byte)(((inst[opeP + 4] & 0xf) << 4) | (inst[opeP + 3] & 0xf)));
                SOutData(page, mml, port[0], opeN, (byte)(0xb0 + info.groupAdr)//ACC(1) FB(3) WF(3)
                    , (byte)(((inst[opeP + 10] & 0x1) << 7)
                    | ((inst[opeP + 11] & 0x7) << 4)
                    | (inst[opeP + 9] & 0x7)));


                //TLはvolumeの設定と一緒に行うがキャリアのみである。
                //そのため、algからモジュレータのパラメータをセットする必要がある
                if (tblCM2[page.algo][ope] != 0) continue;

                SOutData(page, mml, port[0], opeN, (byte)(0x40 + info.groupAdr)//TL(7)
                    , (byte)(inst[opeP + 5] & 0x7f));
            }

            page.beforeVolume = -1;
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

            page.volume = vol;
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

            if (page.spg.beforeTlDelta1 != tl1)
            {
                if (parent.instOPL.ContainsKey(page.instrument))
                {
                    OutFmSetTL(page, mml, tl1);
                }
                page.spg.beforeTlDelta1 = tl1;
            }
        }

        public void OutFmSetTL(partPage page, MML mml, int tl1)
        {
            int n = page.instrument;
            if (!parent.instOPL.ContainsKey(n))
            {
                msgBox.setWrnMsg(string.Format(msg.get("E11000"), n), mml.line.Lp);
                return;
            }

            OPXchInfo info = new OPXchInfo(page);
            int opeTL = -1;
            byte[] inst = parent.instOPX[n].Item2;

            switch (page.sync)
            {
                case 0://4ope
                    for (int ope = 0; ope < 4; ope++)
                    {
                        byte opeN = (byte)(ope == 1 ? 2 : (ope == 2 ? 1 : ope));
                        int opeP = 2 + ope * 15;
                        if (tblCM4[page.algo][ope] != 0) continue;
                        opeTL = (inst[opeP + 5] & 0x7f) - tl1;
                        opeTL = Common.CheckRange(opeTL, 0, 127);
                        SOutData(page, mml, port[0], opeN, (byte)(0x40 + info.groupAdr)//TL(7)
                            , (byte)opeTL);
                    }
                    break;
                case 1://2ope x 2ope
                    for (int ope = 0; ope < 2; ope++)
                    {
                        byte opeN = (byte)((ope == 1 ? 2 : (ope == 2 ? 1 : ope)) + info.bankB);
                        int opeP = 2 + ope * 15;
                        if (tblCM2[page.algo][ope] != 0) continue;
                        opeTL = (inst[opeP + 5] & 0x7f) - tl1;
                        opeTL = Common.CheckRange(opeTL, 0, 127);
                        SOutData(page, mml, port[0], opeN, (byte)(0x40 + info.groupAdr)//TL(7)
                            , (byte)opeTL);
                    }
                    break;
                case 2://3ope 
                    for (int ope = 0; ope < 3; ope++)
                    {
                        byte opeN = (byte)(ope == 1 ? 2 : (ope == 2 ? 1 : ope));
                        int opeP = 2 + ope * 15;
                        if (tblCM3[page.algo][ope] != 0) continue;
                        opeTL = (inst[opeP + 5] & 0x7f) - tl1;
                        opeTL = Common.CheckRange(opeTL, 0, 127);
                        SOutData(page, mml, port[0], opeN, (byte)(0x40 + info.groupAdr)//TL(7)
                            , (byte)opeTL);
                    }
                    break;
            }
        }

        public void OutFmSetVolume(partPage page, MML mml, int vol)
        {
            int n = page.instrument;
            if (!parent.instOPX.ContainsKey(n))
            {
                msgBox.setWrnMsg(string.Format(msg.get("E11000"), n), mml==null?null:mml.line.Lp);
                return;
            }

            OPXchInfo info = new OPXchInfo(page);
            int opeTL;
            byte[] inst = parent.instOPX[n].Item2;

            switch (page.sync)
            {
                case 0://4ope
                    if (inst[0] != 4)//4ope音色のみ
                    {
                        msgBox.setErrMsg(
                            string.Format(msg.get("E31009")
                            , page.sync
                            , n)
                            , mml == null ? null : mml.line.Lp);
                        return;
                    }
                    for (int ope = 0; ope < 4; ope++)
                    {
                        byte opeN = (byte)(ope == 1 ? 2 : (ope == 2 ? 1 : ope));
                        int opeP = 2 + ope * 15;
                        if (tblCM4[page.algo][ope] == 0) continue;
                        opeTL = (127 - vol) + (inst[opeP + 5] & 0x7f);
                        opeTL = Common.CheckRange(opeTL, 0, 127);
                        SOutData(page, mml, port[0], opeN, (byte)(0x40 + info.groupAdr)//TL(7)
                            , (byte)opeTL);
                    }
                    break;
                case 1://2ope x 2ope
                    if (inst[0] != 2)//2ope音色のみ
                    {
                        msgBox.setErrMsg(
                            string.Format(msg.get("E31009")
                            , page.sync
                            , n)
                            , mml == null ? null : mml.line.Lp);
                        return;
                    }
                    for (int ope = 0; ope < 2; ope++)
                    {
                        byte opeN = (byte)((ope == 1 ? 2 : (ope == 2 ? 1 : ope)) + info.bankB);
                        int opeP = 2 + ope * 15;
                        if (tblCM2[page.algo][ope] == 0) continue;
                        opeTL = (127 - vol) + (inst[opeP + 5] & 0x7f);
                        opeTL = Common.CheckRange(opeTL, 0, 127);
                        SOutData(page, mml, port[0], opeN, (byte)(0x40 + info.groupAdr)//TL(7)
                            , (byte)opeTL);
                    }
                    break;
                case 2://3ope 
                    if (inst[0] != 3)//3ope音色のみ
                    {
                        msgBox.setErrMsg(
                            string.Format(msg.get("E31009")
                            , page.sync
                            , n)
                            , mml == null ? null : mml.line.Lp);
                        return;
                    }
                    for (int ope = 0; ope < 3; ope++)
                    {
                        byte opeN = (byte)(ope == 1 ? 2 : (ope == 2 ? 1 : ope));
                        int opeP = 2 + ope * 15;
                        if (tblCM3[page.algo][ope] == 0) continue;
                        opeTL = (127 - vol) + (inst[opeP + 5] & 0x7f);
                        opeTL = Common.CheckRange(opeTL, 0, 127);
                        SOutData(page, mml, port[0], opeN, (byte)(0x40 + info.groupAdr)//TL(7)
                            , (byte)opeTL);
                    }
                    break;
            }
        }

        public override void MultiChannelCommand(MML mml)
        {

            foreach (partWork pw in lstPartWork)
            {
                foreach (partPage page in pw.pg)
                {
                    OPXchInfo info = new OPXchInfo(page);

                    if (page.beforeVolume != page.volume && parent.instOPX.ContainsKey(page.instrument))
                    {
                        page.beforeVolume = page.volume;
                        OutFmSetVolume(page, mml, page.volume);
                    }

                    SetFmTL(page, mml);

                    if (page.keyOff)
                    {
                        page.keyOff = false;
                        SOutData(page, mml, port[0], (byte)info.bank, (byte)(0x00 + info.groupAdr), 0x00);
                    }

                    if (page.keyOn)
                    {
                        page.keyOn = false;
                        SOutData(page, mml, port[0], (byte)info.bank, (byte)(0x00 + info.groupAdr), 0x01);
                    }

                    if (page.beforeFNum != page.freq)
                    {
                        page.beforeFNum = page.freq;

                        int f = page.freq & 0xfff;
                        int o = (page.freq >> 12) & 0xf;
                        SOutData(page, mml, port[0], (byte)info.bank, (byte)(0xa0 + info.groupAdr), (byte)((o << 4) | ((f >> 8) & 0xf)));
                        SOutData(page, mml, port[0], (byte)info.bank, (byte)(0x90 + info.groupAdr), (byte)f);
                    }
                }
            }

        }

    }

}