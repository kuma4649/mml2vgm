using System;
using System.Collections.Generic;
using musicDriverInterface;

namespace Core
{
    public class SN76489 : ClsChip
    {

        public const string PSGF_NUM = "PSGF-NUM";
        protected int[][] _FNumTbl = new int[1][] {
            new int[96]
        };
        private int beforePanData = -1;

        public SN76489(ClsVgm parent, int chipID, string initialPartName, string stPath, int chipNumber) : base(parent, chipID, initialPartName, stPath, chipNumber)
        {
            _Name = "SN76489";
            _ShortName = "DCSG";
            _ChMax = 4;
            _canUsePcm = false;
            _canUsePI = false;
            FNumTbl = _FNumTbl;
            port = new byte[][]{
                new byte[] { (byte)(chipNumber!=0 ? 0x30 : 0x50) }
                , new byte[] { (byte)(chipNumber!=0 ? 0x3f : 0x4f) }
            };

            Frequency = 3579545;
            this.ChipNumber = chipNumber;

            Envelope = new Function();
            Envelope.Max = 15;
            Envelope.Min = 0;

            if (string.IsNullOrEmpty(initialPartName)) return;

            Dictionary<string, List<double>> dic = MakeFNumTbl();
            if (dic != null)
            {
                int c = 0;
                foreach (double v in dic["FNUM_00"])
                {
                    FNumTbl[0][c++] = (int)v;
                    if (c == FNumTbl[0].Length) break;
                }
            }

            Ch = new ClsChannel[ChMax];
            SetPartToCh(Ch, initialPartName);
            foreach (ClsChannel ch in Ch)
            {
                ch.Type = enmChannelType.DCSG;
                ch.chipNumber = chipID == 1;
            }
            Ch[3].Type = enmChannelType.DCSGNOISE;


        }

        public override void InitChip()
        {
            if (!use) return;
            if (ChipID != 0 && parent.info.format != enmFormat.ZGM)
            {
                parent.dat[0x0f] = new outDatum(enmMMLType.unknown, null, null, (byte)(parent.dat[0x0f].val | 0x40));
            }

            OutAllKeyOff();
        }

        public override void InitPart(partWork pw)
        {
            foreach (partPage pg in pw.pg)
            {
                pg.MaxVolume = 15;
                pg.volume = pg.MaxVolume;
                pg.keyOn = false;
                pg.panL = 3;
                pg.port = port;
            }
        }


        public int GetDcsgFNum(int octave, char noteCmd, int shift)
        {
            int o = octave - 1;
            int n = Const.NOTE.IndexOf(noteCmd) + shift;
            o += n / 12;
            o = Common.CheckRange(o, 0, 7);
            n %= 12;

            int f = o * 12 + n;
            if (f < 0) f = 0;
            if (f >= FNumTbl[0].Length) f = FNumTbl[0].Length - 1;

            return FNumTbl[0][f];
        }

        public void OutGGPsgStereoPort(partPage page, MML mml, byte[] cmd, byte data)
        {
            SOutData(
                page,
                mml, cmd
                , data
                );
        }

        public void OutPsgPort(partPage page, MML mml, byte[] cmd, byte data)
        {
            SOutData(
                page,
                mml, cmd
                , data
                );
        }

        public void OutPsgKeyOn(partPage page, MML mml)
        {

            page.keyOff = false;
            SetFNum(page, mml);
            SetVolume(page, mml);
            SetDummyData(page, mml);

            if (mml != null)
            {
                MML vmml = new MML();
                vmml.type = enmMMLType.Volume;
                vmml.args = new List<object>();
                vmml.args.Add(page.volume);
                vmml.line = mml.line;
                SetDummyData(page, vmml);
            }
        }

        public void OutPsgKeyOff(partPage page, MML mml)
        {

            if (!page.envelopeMode) page.keyOff = true;
            SetVolume(page, mml);

        }

        public void OutAllKeyOff()
        {

            foreach (partWork pw in lstPartWork)
            {
                foreach (partPage page in pw.pg)
                {
                    page.beforeFNum = -1;
                    page.beforeVolume = -1;

                    page.keyOn = false;
                    OutPsgKeyOff(page, null);
                }
            }
        }


        public override void SetFNum(partPage page, MML mml)
        {
            if (page.Type != enmChannelType.DCSGNOISE)
            {
                int f = -page.detune;

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
                    f -= page.lfo[lfo].value + page.lfo[lfo].param[6];//param[6] : 位相
                }

                if (page.octaveNow < 1)
                {
                    f <<= -page.octaveNow;
                }
                else
                {
                    f >>= page.octaveNow - 1;
                }

                if (page.bendWaitCounter != -1)
                {
                    f += page.bendFnum;
                }
                else
                {
                    f += GetDcsgFNum(page.octaveNow, page.noteCmd, page.shift + page.keyShift);//
                }

                f = Common.CheckRange(f, 0, 0x3ff);

                if (page.freq == f) return;
                page.freq = f;

                byte data = (byte)(0x80 + (page.ch << 5) + (f & 0xf));
                OutPsgPort(page,mml, port[0], data);

                data = (byte)((f & 0x3f0) >> 4);
                OutPsgPort(page, mml, port[0], data);
            }
            else
            {
                int f = 0xe0 + (page.noise & 7);
                if (page.freq == f) return;
                page.freq = f;
                byte data = (byte)f;
                OutPsgPort(page, mml, port[0], data);
            }

        }

        public override int GetFNum(partPage page, MML mml, int octave, char cmd, int shift)
        {
            return GetDcsgFNum(octave, cmd, shift);
        }

        public override void SetVolume(partPage page, MML mml)
        {
            byte data = 0;
            int vol = page.volume;

            if (!page.keyOff)
            {
                if (page.envelopeMode)
                {
                    vol = 0;
                    if (page.envIndex != -1)
                    {
                        vol = page.envVolume - (15 - page.volume);
                    }
                    else
                    {
                        page.keyOn = false;
                    }
                }

                for (int lfo = 0; lfo < 4; lfo++)
                {
                    if (!page.lfo[lfo].sw) continue;
                    if (page.lfo[lfo].type != eLfoType.Tremolo) continue;

                    vol += page.lfo[lfo].value + page.lfo[lfo].param[6];
                }
            }
            else
            {
                vol = 0;
            }

            vol = Common.CheckRange(vol, 0, 15);

            if (page.beforeVolume != vol)
            {
                data = (byte)(0x80 + (page.ch << 5) + 0x10 + (15 - vol));
                OutPsgPort(page, mml, port[0], data);
                page.beforeVolume = vol;
            }
        }

        public override void SetKeyOn(partPage page, MML mml)
        {
            //OutPsgKeyOn(page, mml);
            page.keyOn = true;
            page.keyOff = false;
        }

        public override void SetKeyOff(partPage page, MML mml)
        {
            //OutPsgKeyOff(page, mml);
            page.keyOff = true;
        }

        public override void SetLfoAtKeyOn(partPage page, MML mml)
        {
            for (int lfo = 0; lfo < 4; lfo++)
            {
                clsLfo pl = page.lfo[lfo];
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


        public override void CmdY(partPage page, MML mml)
        {
            if (mml.args[0] is string) return;

            byte adr = (byte)(int)mml.args[0];
            byte dat = (byte)(int)mml.args[1];

            OutPsgPort(page, mml, port[0], dat);
        }

        public override void CmdNoise(partPage page, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 0, 7);
            page.noise = n;
        }

        public override void CmdDCSGCh3Freq(partPage page, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 0, 0x3ff);

            byte data = (byte)(0xc0 + (n & 0xf));
            OutPsgPort(page, mml, port[0], data);

            data = (byte)(n >> 4);
            OutPsgPort(page, mml, port[0], data);
        }

        public override void CmdLoopExtProc(partPage page, MML mml)
        {
        }

        public override void CmdRest(partPage page, MML mml)
        {
            base.CmdRest(page, mml);

            if (page.envelopeMode)
            {
                if (page.envIndex != -1)
                {
                    page.envIndex = 3;
                }
            }
            else
            {
                SetKeyOff(page, mml);
            }

        }

        public override void CmdInstrument(partPage page, MML mml)
        {
            char type = (char)mml.args[0];
            int n = (int)mml.args[1];

            if (type == 'I')
            {
                msgBox.setErrMsg(msg.get("E15001"), mml.line.Lp);
                return;
            }

            if (type == 'T')
            {
                msgBox.setErrMsg(msg.get("E15002"), mml.line.Lp);
                return;
            }

            n = SetEnvelopParamFromInstrument(page, n, mml);
            SetDummyData(page, mml);
        }

        public override void CmdPan(partPage page, MML mml)
        {
            int l = (int)mml.args[0];

            l = Common.CheckRange(l, 0, 3);
            page.panL = l;
        }

        public override void SetupPageData(partWork pw, partPage page)
        {

            //周波数
            page.spg.freq = -1;
            SetFNum(page, null);

            //ノイズ周波数
            //noiseFreq = -1;
            //OutSsgNoise(null, page);

            //音量
            page.spg.beforeVolume = -1;
            SetVolume(page, null);

        }
        public override void MultiChannelCommand(MML mml)
        {
            if (!use) return;
            int dat = 0;

            foreach (partWork pw in lstPartWork)
            {
                partPage page = pw.cpg;
                int p = page.panL;
                dat |= (((p & 2) == 0 ? 0x00 : 0x10) | ((p & 1) == 0 ? 0x00 : 0x01)) << page.ch;
            }

            if (beforePanData != dat)
            {
                if (parent.info.format != enmFormat.XGM)//XGMではGGのステレオ機能は対応していない
                {
                    if (parent.info.enableGGStereoDCSG)//enable指定の場合のみ
                    {
                        OutGGPsgStereoPort(lstPartWork[lstPartWork.Count - 1].cpg, mml, port[1], (byte)dat);
                    }
                }
                beforePanData = dat;
            }

            foreach (partWork pw in lstPartWork)
            {
                partPage page = pw.cpg;

                if (page.keyOff)
                {
                    page.keyOff = false;
                    OutPsgKeyOff(page, mml);
                }
                if (page.keyOn)
                {
                    page.keyOn = false;
                    OutPsgKeyOn(page, mml);
                }
            }
        }

    }
}