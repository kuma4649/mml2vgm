using System;
using System.Collections.Generic;

namespace Core
{
    public class SN76489 : ClsChip
    {

        public const string PSGF_NUM = "PSGF-NUM";
        protected int[][] _FNumTbl = new int[1][] {
            new int[96]
        };
        private int beforePanData = -1;

        public SN76489(ClsVgm parent, int chipID, string initialPartName, string stPath, bool isSecondary) : base(parent, chipID, initialPartName, stPath, isSecondary)
        {
            _Name = "SN76489";
            _ShortName = "DCSG";
            _ChMax = 4;
            _canUsePcm = false;
            _canUsePI = false;
            FNumTbl = _FNumTbl;

            Frequency = 3579545;

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
                ch.isSecondary = chipID == 1;
            }
            Ch[3].Type = enmChannelType.DCSGNOISE;

            Envelope = new Function();
            Envelope.Max = 15;
            Envelope.Min = 0;

        }

        public override void InitChip()
        {
            if (!use) return;
            if (IsSecondary)
            {
                parent.dat[0x0f] = new outDatum(enmMMLType.unknown, null, null, (byte)(parent.dat[0x0f].val | 0x40));
            }

            OutAllKeyOff();
        }

        public override void InitPart(ref partWork pw)
        {
            pw.MaxVolume = 15;
            pw.volume = pw.MaxVolume;
            pw.port0 = 0x50;
            pw.keyOn = false;
            pw.panL = 3;
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

        public void OutGGPsgStereoPort(MML mml,bool isSecondary, byte data)
        {
            parent.OutData(
                mml,
                (byte)(isSecondary ? 0x3f : 0x4f)
                , data
                );
        }

        public void OutPsgPort(MML mml,bool isSecondary, byte data)
        {
            parent.OutData(
                mml,
                (byte)(isSecondary ? 0x30 : 0x50)
                , data
                );
        }

        public void OutPsgKeyOn(partWork pw, MML mml)
        {

            pw.keyOn = true;
            SetFNum(pw, mml);
            SetVolume(pw, mml);

            MML vmml = new MML();
            vmml.type = enmMMLType.Volume;
            vmml.args = new List<object>();
            vmml.args.Add(pw.volume);
            vmml.line = mml.line;
            SetDummyData(pw, vmml);
        }

        public void OutPsgKeyOff(partWork pw, MML mml)
        {

            if (!pw.envelopeMode) pw.keyOn = false;
            SetVolume(pw,mml);

        }

        public void OutAllKeyOff()
        {

            foreach (partWork pw in lstPartWork)
            {
                pw.beforeFNum = -1;
                pw.beforeVolume = -1;

                pw.keyOn = false;
                OutPsgKeyOff(pw, null);
            }

        }


        public override void SetFNum(partWork pw, MML mml)
        {
            if (pw.Type != enmChannelType.DCSGNOISE)
            {
                int f = -pw.detune;

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
                    f -= pw.lfo[lfo].value + pw.lfo[lfo].param[6];//param[6] : 位相
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
                    f += GetDcsgFNum(pw.octaveNow, pw.noteCmd, pw.shift + pw.keyShift);//
                }

                f = Common.CheckRange(f, 0, 0x3ff);

                if (pw.freq == f) return;
                pw.freq = f;

                byte data = (byte)(0x80 + (pw.ch << 5) + (f & 0xf));
                OutPsgPort(mml, pw.isSecondary, data);

                data = (byte)((f & 0x3f0) >> 4);
                OutPsgPort(mml, pw.isSecondary, data);
            }
            else
            {
                int f = 0xe0 + (pw.noise & 7);
                if (pw.freq == f) return;
                pw.freq = f;
                byte data = (byte)f;
                OutPsgPort(mml, pw.isSecondary, data);
            }

        }

        public override int GetFNum(partWork pw, MML mml, int octave, char cmd, int shift)
        {
            return GetDcsgFNum(octave, cmd, shift);
        }

        public override void SetVolume(partWork pw, MML mml)
        {
            byte data = 0;
            int vol = pw.volume;

            if (pw.keyOn)
            {
                if (pw.envelopeMode)
                {
                    vol = 0;
                    if (pw.envIndex != -1)
                    {
                        vol = pw.envVolume - (15 - pw.volume);
                    }
                    else
                    {
                        pw.keyOn = false;
                    }
                }

                for (int lfo = 0; lfo < 4; lfo++)
                {
                    if (!pw.lfo[lfo].sw) continue;
                    if (pw.lfo[lfo].type != eLfoType.Tremolo) continue;

                    vol += pw.lfo[lfo].value + pw.lfo[lfo].param[6];
                }
            }
            else
            {
                vol = 0;
            }

            vol = Common.CheckRange(vol, 0, 15);

            if (pw.beforeVolume != vol)
            {
                data = (byte)(0x80 + (pw.ch << 5) + 0x10 + (15 - vol));
                OutPsgPort(mml,pw.isSecondary, data);
                pw.beforeVolume = vol;
            }
        }

        public override void SetKeyOn(partWork pw, MML mml)
        {
            OutPsgKeyOn(pw,mml);
        }

        public override void SetKeyOff(partWork pw, MML mml)
        {
            OutPsgKeyOff(pw,mml);
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

            }
        }

        public override void SetToneDoubler(partWork pw)
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

            byte adr = (byte)mml.args[0];
            byte dat = (byte)mml.args[1];

            OutPsgPort(mml,pw.isSecondary, dat);
        }

        public override void CmdNoise(partWork pw, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 0, 7);
            pw.noise = n;
        }

        public override void CmdLoopExtProc(partWork pw, MML mml)
        {
        }

        public override void CmdRest(partWork pw, MML mml)
        {
            base.CmdRest(pw, mml);

            if (pw.envelopeMode)
            {
                if (pw.envIndex != -1)
                {
                    pw.envIndex = 3;
                }
            }
            else
            {
                SetKeyOff(pw,mml);
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

            n = SetEnvelopParamFromInstrument(pw, n,mml);
            SetDummyData(pw, mml);
        }

        public override void CmdPan(partWork pw, MML mml)
        {
            int l = (int)mml.args[0];

            l = Common.CheckRange(l, 0, 3);
            pw.panL = l;
        }

        public override void MultiChannelCommand(MML mml)
        {
            if (!use) return;
            int dat = 0;

            foreach (partWork pw in lstPartWork)
            {
                int p = pw.panL;
                dat |= (((p & 2) == 0 ? 0x00 : 0x10) | ((p & 1) == 0 ? 0x00 : 0x01)) << pw.ch;
            }

            if (beforePanData == dat) return;
            OutGGPsgStereoPort(mml,IsSecondary, (byte)dat);
            beforePanData = dat;

        }

    }
}
