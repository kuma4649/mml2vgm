using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class AY8910 : ClsChip
    {
        //public int[] FNumTbl = new int[] {
        //    // TP = M / (ftone * 16)       16:Divider
        //    //   c    c+     d    d+     e     f    f+     g    g+     a    a+     b
        //     0xEE8,0xE12,0xD48,0xC89,0xBD5,0xB2B,0xA8A,0x9F3,0x964,0x8DD,0x85E,0x7E6
        //    ,0x774,0x709,0x6A4,0x645,0x5EA,0x595,0x545,0x4FA,0x4B2,0x46F,0x42F,0x3F3
        //    ,0x3BA,0x384,0x352,0x322,0x2F5,0x2CB,0x2A3,0x27D,0x259,0x237,0x217,0x1F9
        //    ,0x1DD,0x1C2,0x1A9,0x191,0x17B,0x165,0x151,0x13E,0x12D,0x11C,0x10C,0x0FD
        //    ,0x0EF,0x0E1,0x0D4,0x0C9,0x0BD,0x0B3,0x0A9,0x09F,0x096,0x08E,0x086,0x07E
        //    ,0x077,0x071,0x06A,0x064,0x05F,0x059,0x054,0x050,0x04B,0x047,0x043,0x03F
        //    ,0x03C,0x038,0x035,0x032,0x02F,0x02D,0x02A,0x028,0x026,0x023,0x021,0x020
        //    ,0x01E,0x01C,0x01B,0x019,0x018,0x016,0x015,0x014,0x013,0x012,0x011,0x010
        //};

        public byte SSGKeyOn = 0x3f;

        public AY8910(ClsVgm parent, int chipID, string initialPartName, string stPath, bool isSecondary) : base(parent, chipID, initialPartName, stPath, isSecondary)
        {
            _chipType = enmChipType.AY8910;
            _Name = "AY8910";
            _ShortName = "AY10";
            _ChMax = 3;
            _canUsePcm = false;
            _canUsePI = false;
            IsSecondary = isSecondary;

            Frequency = 1789750;
            port = new byte[][] { new byte[] { 0xa0 } };

            Dictionary<string, List<double>> dic = MakeFNumTbl();
            if (dic != null)
            {
                int c = 0;
                FNumTbl = new int[1][] { new int[96] };
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
                ch.Type = enmChannelType.SSG;
                ch.isSecondary = chipID == 1;
                ch.MaxVolume = 15;
            }

            Envelope = new Function();
            Envelope.Max = 15;
            Envelope.Min = 0;

        }

        public override void InitPart(partWork pw)
        {
            pw.volume = 15;
            pw.MaxVolume = 15;
            pw.port = port;

        }

        public override void InitChip()
        {
            if (!use) return;

            for (int ch = 0; ch < 3; ch++)
            {
                OutSsgKeyOff(null, lstPartWork[ch]);
                lstPartWork[ch].volume = 0;
            }

            if (ChipID != 0)
            {
                parent.dat[0x77] = new outDatum(enmMMLType.unknown, null, null, (byte)(parent.dat[0x77].val | 0x40));//use Secondary
            }
        }

        public void OutSsgKeyOn(MML mml, partWork pw)
        {
            byte pch = (byte)pw.ch;
            int n = (pw.mixer & 0x1) + ((pw.mixer & 0x2) << 2);
            byte data = 0;

            data = (byte)(SSGKeyOn | (9 << pch));
            data &= (byte)(~(n << pch));
            SSGKeyOn = data;

            SetSsgVolume(mml, pw);
            if (pw.HardEnvelopeSw)
            {
                parent.OutData(mml, port[0], 0x0d, (byte)(pw.HardEnvelopeType & 0xf));
            }
            parent.OutData(mml, port[0], 0x07, data);
        }

        public void OutSsgKeyOff(MML mml, partWork pw)
        {
            byte pch = (byte)pw.ch;
            int n = 9;
            byte data = 0;

            data = (byte)(SSGKeyOn | (n << pch));
            SSGKeyOn = data;

            parent.OutData(mml, port[0], (byte)(0x08 + pch), 0);
            pw.beforeVolume = -1;
            parent.OutData(mml, port[0], 0x07, data);
        }

        public void SetSsgVolume(MML mml, partWork pw)
        {
            byte pch = (byte)pw.ch;

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

            if (pw.beforeVolume != vol)
            {
                parent.OutData(mml, port[0], (byte)(0x08 + pch), (byte)vol);
                //pw.beforeVolume = pw.volume;
                pw.beforeVolume = vol;
            }
        }

        public void OutSsgNoise(MML mml, partWork pw, int n)
        {
            parent.OutData(mml, port[0], 0x06, (byte)(n & 0x1f));
        }

        public void SetSsgFNum(partWork pw, MML mml)
        {
            int f = GetSsgFNum(pw, mml, pw.octaveNow, pw.noteCmd, pw.shift + pw.keyShift);//
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

            f = Common.CheckRange(f, 0, 0xfff);
            if (pw.freq == f) return;

            pw.freq = f;

            byte data = 0;

            data = (byte)(f & 0xff);
            parent.OutData(mml, port[0], (byte)(0 + pw.ch * 2), data);

            data = (byte)((f & 0xf00) >> 8);
            parent.OutData(mml, port[0], (byte)(1 + pw.ch * 2), data);
        }

        public int GetSsgFNum(partWork pw, MML mml, int octave, char noteCmd, int shift)
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

        public override int GetFNum(partWork pw, MML mml, int octave, char cmd, int shift)
        {
            return GetSsgFNum(pw, mml, octave, cmd, shift);
        }


        public override void SetFNum(partWork pw, MML mml)
        {
            SetSsgFNum(pw, mml);
        }

        public override void SetKeyOn(partWork pw, MML mml)
        {
            OutSsgKeyOn(mml, pw);
        }

        public override void SetKeyOff(partWork pw, MML mml)
        {
            OutSsgKeyOff(mml, pw);
        }

        public override void SetVolume(partWork pw, MML mml)
        {
            SetSsgVolume(mml, pw);
        }

        public override void SetToneDoubler(partWork pw)
        {
        }

        public override int GetToneDoublerShift(partWork pw, int octave, char noteCmd, int shift)
        {
            return 0;
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

                if (pl.type == eLfoType.Vibrato)
                {
                    SetSsgFNum(pw, mml);
                }

                if (pl.type == eLfoType.Tremolo)
                {
                    SetSsgVolume(mml, pw);
                }

            }
        }

        public override void CmdInstrument(partWork pw, MML mml)
        {
            char type = (char)mml.args[0];
            int n = (int)mml.args[1];

            if (type == 'I')
            {
                msgBox.setErrMsg(msg.get("E08000")
                    , mml.line.Lp);
                return;
            }

            if (type == 'T')
            {
                msgBox.setErrMsg(msg.get("E08001")
                    , mml.line.Lp);
                return;
            }

            if (type == 'E')
            {
                n = SetEnvelopParamFromInstrument(pw, n, mml);
                return;
            }

            SetEnvelopParamFromInstrument(pw, n, mml);
            return;
        }

        public override void CmdNoiseToneMixer(partWork pw, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 0, 3);
            pw.mixer = n;
        }

        public override void CmdNoise(partWork pw, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 0, 31);
            pw.chip.lstPartWork[0].noise = n;//Chipの1Chに保存
            OutSsgNoise(mml, pw, n);
        }

        public override void CmdHardEnvelope(partWork pw, MML mml)
        {
            string cmd = (string)mml.args[0];
            int n = 0;

            switch (cmd)
            {
                case "EH":
                    n = (int)mml.args[1];
                    if (pw.HardEnvelopeSpeed != n)
                    {
                        parent.OutData(mml, port[0], 0x0b, (byte)(n & 0xff));
                        parent.OutData(mml, port[0], 0x0c, (byte)((n >> 8) & 0xff));
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
                        parent.OutData(mml, port[0], 0x0d, (byte)(n & 0xf));
                        pw.HardEnvelopeType = n;
                    }
                    break;
            }
        }

        public override void CmdY(partWork pw, MML mml)
        {
            if (mml.args[0] is string) return;

            int adr = (int)mml.args[0];
            byte dat = (byte)mml.args[1];

            parent.OutData(mml, port[0], (byte)adr, (byte)dat);
        }

        public override void CmdLoopExtProc(partWork pw, MML mml)
        {
        }

    }
}