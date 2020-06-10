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
            port =new byte[][]{
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
            if (ChipID!= 0 && parent.info.format != enmFormat.ZGM)
            {
                parent.dat[0x0f] = new outDatum(enmMMLType.unknown, null, null, (byte)(parent.dat[0x0f].val | 0x40));
            }

            OutAllKeyOff();
        }

        public override void InitPart(partWork pw)
        {
            pw.pg[pw.cpg].MaxVolume = 15;
            pw.pg[pw.cpg].volume = pw.pg[pw.cpg].MaxVolume;
            pw.pg[pw.cpg].keyOn = false;
            pw.pg[pw.cpg].panL = 3;
            pw.pg[pw.cpg].port = port;
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

        public void OutGGPsgStereoPort(MML mml, byte[] cmd, byte data)
        {
            parent.OutData(
                mml, cmd
                , data
                );
        }

        public void OutPsgPort(MML mml, byte[] cmd, byte data)
        {
            parent.OutData(
                mml, cmd
                , data
                );
        }

        public void OutPsgKeyOn(partWork pw, MML mml)
        {

            pw.pg[pw.cpg].keyOn = true;
            SetFNum(pw, mml);
            SetVolume(pw, mml);
            SetDummyData(pw, mml);

            MML vmml = new MML();
            vmml.type = enmMMLType.Volume;
            vmml.args = new List<object>();
            vmml.args.Add(pw.pg[pw.cpg].volume);
            vmml.line = mml.line;
            SetDummyData(pw, vmml);
            
        }

        public void OutPsgKeyOff(partWork pw, MML mml)
        {

            if (!pw.pg[pw.cpg].envelopeMode) pw.pg[pw.cpg].keyOn = false;
            SetVolume(pw, mml);

        }

        public void OutAllKeyOff()
        {

            foreach (partWork pw in lstPartWork)
            {
                pw.pg[pw.cpg].beforeFNum = -1;
                pw.pg[pw.cpg].beforeVolume = -1;

                pw.pg[pw.cpg].keyOn = false;
                OutPsgKeyOff(pw, null);
            }

        }


        public override void SetFNum(partWork pw, MML mml)
        {
            if (pw.pg[pw.cpg].Type != enmChannelType.DCSGNOISE)
            {
                int f = -pw.pg[pw.cpg].detune;

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
                    f -= pw.pg[pw.cpg].lfo[lfo].value + pw.pg[pw.cpg].lfo[lfo].param[6];//param[6] : 位相
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
                    f += GetDcsgFNum(pw.pg[pw.cpg].octaveNow, pw.pg[pw.cpg].noteCmd, pw.pg[pw.cpg].shift + pw.pg[pw.cpg].keyShift);//
                }

                f = Common.CheckRange(f, 0, 0x3ff);

                if (pw.pg[pw.cpg].freq == f) return;
                pw.pg[pw.cpg].freq = f;

                byte data = (byte)(0x80 + (pw.pg[pw.cpg].ch << 5) + (f & 0xf));
                OutPsgPort(mml, port[0],  data);

                data = (byte)((f & 0x3f0) >> 4);
                OutPsgPort(mml, port[0], data);
            }
            else
            {
                int f = 0xe0 + (pw.pg[pw.cpg].noise & 7);
                if (pw.pg[pw.cpg].freq == f) return;
                pw.pg[pw.cpg].freq = f;
                byte data = (byte)f;
                OutPsgPort(mml, port[0],  data);
            }

        }

        public override int GetFNum(partWork pw, MML mml, int octave, char cmd, int shift)
        {
            return GetDcsgFNum(octave, cmd, shift);
        }

        public override void SetVolume(partWork pw, MML mml)
        {
            byte data = 0;
            int vol = pw.pg[pw.cpg].volume;

            if (pw.pg[pw.cpg].keyOn)
            {
                if (pw.pg[pw.cpg].envelopeMode)
                {
                    vol = 0;
                    if (pw.pg[pw.cpg].envIndex != -1)
                    {
                        vol = pw.pg[pw.cpg].envVolume - (15 - pw.pg[pw.cpg].volume);
                    }
                    else
                    {
                        pw.pg[pw.cpg].keyOn = false;
                    }
                }

                for (int lfo = 0; lfo < 4; lfo++)
                {
                    if (!pw.pg[pw.cpg].lfo[lfo].sw) continue;
                    if (pw.pg[pw.cpg].lfo[lfo].type != eLfoType.Tremolo) continue;

                    vol += pw.pg[pw.cpg].lfo[lfo].value + pw.pg[pw.cpg].lfo[lfo].param[6];
                }
            }
            else
            {
                vol = 0;
            }

            vol = Common.CheckRange(vol, 0, 15);

            if (pw.pg[pw.cpg].beforeVolume != vol)
            {
                data = (byte)(0x80 + (pw.pg[pw.cpg].ch << 5) + 0x10 + (15 - vol));
                OutPsgPort(mml, port[0],  data);
                pw.pg[pw.cpg].beforeVolume = vol;
            }
        }

        public override void SetKeyOn(partWork pw, MML mml)
        {
            OutPsgKeyOn(pw, mml);
        }

        public override void SetKeyOff(partWork pw, MML mml)
        {
            OutPsgKeyOff(pw, mml);
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

            }
        }

        public override void SetToneDoubler(partWork pw, MML mml)
        {
            //実装不要
        }

        public override int GetToneDoublerShift(partWork pw, int octave, char noteCmd, int shift)
        {
            return 0;
        }


        public override void CmdY(partWork pw, MML mml)
        {
            if (mml.args[0] is string) return;

            byte adr = (byte)(int)mml.args[0];
            byte dat = (byte)(int)mml.args[1];

            OutPsgPort(mml, port[0],  dat);
        }

        public override void CmdNoise(partWork pw, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 0, 7);
            pw.pg[pw.cpg].noise = n;
        }

        public override void CmdDCSGCh3Freq(partWork pw, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 0, 0x3ff);

            byte data = (byte)(0xc0 + (n & 0xf));
            OutPsgPort(mml, port[0], data);

            data = (byte)(n >> 4);
            OutPsgPort(mml, port[0], data);
        }

        public override void CmdLoopExtProc(partWork pw, MML mml)
        {
        }

        public override void CmdRest(partWork pw, MML mml)
        {
            base.CmdRest(pw, mml);

            if (pw.pg[pw.cpg].envelopeMode)
            {
                if (pw.pg[pw.cpg].envIndex != -1)
                {
                    pw.pg[pw.cpg].envIndex = 3;
                }
            }
            else
            {
                SetKeyOff(pw, mml);
            }

        }

        public override void CmdInstrument(partWork pw, MML mml)
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

            n = SetEnvelopParamFromInstrument(pw, n, mml);
            SetDummyData(pw, mml);
        }

        public override void CmdPan(partWork pw, MML mml)
        {
            int l = (int)mml.args[0];

            l = Common.CheckRange(l, 0, 3);
            pw.pg[pw.cpg].panL = l;
        }

        public override void MultiChannelCommand(MML mml)
        {
            if (!use) return;
            int dat = 0;

            foreach (partWork pw in lstPartWork)
            {
                int p = pw.pg[pw.cpg].panL;
                dat |= (((p & 2) == 0 ? 0x00 : 0x10) | ((p & 1) == 0 ? 0x00 : 0x01)) << pw.pg[pw.cpg].ch;
            }

            if (beforePanData == dat) return;
            OutGGPsgStereoPort(mml, port[1], (byte)dat);
            beforePanData = dat;

        }

    }
}
