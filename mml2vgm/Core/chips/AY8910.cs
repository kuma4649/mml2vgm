using musicDriverInterface;
using System.Collections.Generic;

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
        public int noiseFreq = -1;

        public AY8910(ClsVgm parent, int chipID, string initialPartName, string stPath, int chipNumber) : base(parent, chipID, initialPartName, stPath, chipNumber)
        {
            _chipType = enmChipType.AY8910;
            _Name = "AY8910";
            _ShortName = "AY10";
            _ChMax = 3;
            _canUsePcm = false;
            _canUsePI = false;
            ChipNumber = chipNumber;

            Frequency = 1789750;
            port = new byte[][] { new byte[] { 0xa0 } };

            if (string.IsNullOrEmpty(initialPartName)) return;

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
                ch.chipNumber = chipID == 1;
                ch.MaxVolume = 15;
            }

            Envelope = new Function();
            Envelope.Max = 15;
            Envelope.Min = 0;

        }

        public override void InitPart(partWork pw)
        {
            foreach (partPage pg in pw.pg)
            {
                pg.volume = 15;
                pg.MaxVolume = 15;
                pg.port = port;
            }

        }

        public override void InitChip()
        {
            if (!use) return;

            //for (int ch = 0; ch < 3; ch++)
            //{
            //OutSsgKeyOff(null, lstPartWork[ch].cpg);
            //lstPartWork[ch].apg.volume = 0;
            //}

            if (ChipID != 0)
            {
                parent.dat[0x77] = new outDatum(enmMMLType.unknown, null, null, (byte)(parent.dat[0x77].val | 0x40));//use Secondary
            }
        }

        public void OutSsgKeyOn(MML mml, partPage page)
        {
            byte pch = (byte)page.ch;
            int n = (page.mixer & 0x1) + ((page.mixer & 0x2) << 2);
            byte data = (byte)(SSGKeyOn | 9 << pch);
            data &= (byte)(~(n << pch));
            SSGKeyOn = data;

            SetSsgVolume(mml, page);
            if (page.HardEnvelopeSw)
            {
                //parent.OutData(mml, port[0], 0x0d, (byte)(page.HardEnvelopeType & 0xf));
                SOutData(page, mml, port[0], 0x0d, (byte)(page.HardEnvelopeType & 0xf));
            }
            //parent.OutData(mml, port[0], 0x07, data);
            SOutData(page, mml, port[0], 0x07, data);
        }

        public void OutSsgKeyOff(MML mml, partPage page)
        {
            byte pch = (byte)page.ch;
            int n = 9;
            byte data = (byte)(SSGKeyOn | n << pch);
            SSGKeyOn = data;

            //parent.OutData(mml, port[0], (byte)(0x08 + pch), 0);
            //SOutData(page, mml, port[0], (byte)(0x08 + pch), 0);
            //page.spg.beforeVolume = -1;
            //parent.OutData(mml, port[0], 0x07, data);
            SOutData(page, mml, port[0], 0x07, data);
        }

        public void SetSsgVolume(MML mml, partPage page)
        {
            byte pch = (byte)page.ch;

            int vol = page.volume;
            if (page.envelopeMode)
            {
                vol = 0;
                if (page.envIndex != -1)
                {
                    vol = page.volume - (15 - page.envVolume);
                }
            }

            for (int lfo = 0; lfo < 4; lfo++)
            {
                if (!page.lfo[lfo].sw) continue;
                if (page.lfo[lfo].type != eLfoType.Tremolo) continue;

                vol += page.lfo[lfo].value + page.lfo[lfo].param[6];
            }

            if (page.varpeggioMode)
            {
                vol += page.varpDelta;
            }

            vol = Common.CheckRange(vol, 0, 15) + (page.HardEnvelopeSw ? 0x10 : 0x00);

            if (page.spg.beforeVolume != vol)
            {
                //parent.OutData(mml, port[0], (byte)(0x08 + pch), (byte)vol);
                SOutData(page, mml, port[0], (byte)(0x08 + pch), (byte)vol);
                //pw.ppg[pw.cpgNum].beforeVolume = pw.ppg[pw.cpgNum].volume;
                page.spg.beforeVolume = vol;
            }
        }

        public void OutSsgNoise(MML mml, partPage page)
        {
            if (noiseFreq != page.noise)
            {
                noiseFreq = page.noise;
                SOutData(page, mml, port[0], 0x06, (byte)(page.noise & 0x1f));
            }
        }

        public void SetSsgFNum(partPage page, MML mml)
        {
            int arpNote = page.arpFreqMode ? 0 : page.arpDelta;
            int arpFreq = page.arpFreqMode ? page.arpDelta : 0;

            int f = GetSsgFNum(page, mml, page.octaveNow, page.noteCmd, page.shift + page.keyShift + arpNote);//
            if (page.bendWaitCounter != -1)
            {
                f = page.bendFnum;
            }
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

            f = Common.CheckRange(f, 0, 0xfff);
            if (page.spg.freq == f) return;

            page.spg.freq = f;
            byte data = (byte)(f & 0xff);
            //parent.OutData(mml, port[0], (byte)(0 + page.ch * 2), data);
            SOutData(page, mml, port[0], (byte)(0 + page.ch * 2), data);

            data = (byte)((f & 0xf00) >> 8);
            //parent.OutData(mml, port[0], (byte)(1 + page.ch * 2), data);
            SOutData(page, mml, port[0], (byte)(1 + page.ch * 2), data);
        }

        public int GetSsgFNum(partPage page, MML mml, int octave, char noteCmd, int shift)
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

        public override int GetFNum(partPage page, MML mml, int octave, char cmd, int shift)
        {
            return GetSsgFNum(page, mml, octave, cmd, shift);
        }


        public override void SetFNum(partPage page, MML mml)
        {
            SetSsgFNum(page, mml);
        }

        public override void SetKeyOn(partPage page, MML mml)
        {
            OutSsgKeyOn(mml, page);
        }

        public override void SetKeyOff(partPage page, MML mml)
        {
            OutSsgKeyOff(mml, page);
        }

        public override void SetVolume(partPage page, MML mml)
        {
            SetSsgVolume(mml, page);
        }

        public override void SetToneDoubler(partPage page, MML mml)
        {
        }

        public override int GetToneDoublerShift(partPage page, int octave, char noteCmd, int shift)
        {
            return 0;
        }

        public override void SetLfoAtKeyOn(partPage page, MML mml)
        {
            for (int lfo = 0; lfo < 4; lfo++)
            {
                clsLfo pl = page.lfo[lfo];
                if (!pl.sw)
                    continue;
                if (pl.type == eLfoType.Wah)
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
                    SetSsgFNum(page, mml);
                }

                if (pl.type == eLfoType.Tremolo)
                {
                    SetSsgVolume(mml, page);
                }

            }
        }

        public override void CmdInstrument(partPage page, MML mml)
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
                SetEnvelopParamFromInstrument(page, n, mml);
                return;
            }

            SetEnvelopParamFromInstrument(page, n, mml);
            return;
        }

        public override void CmdNoiseToneMixer(partPage page, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 0, 3);
            page.mixer = n;
        }

        public override void CmdNoise(partPage page, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 0, 31);
            page.noise = n;
            OutSsgNoise(mml, page);
        }

        public override void CmdHardEnvelope(partPage page, MML mml)
        {
            string cmd = (string)mml.args[0];
            //int n = 0;

            switch (cmd)
            {
                case "EH":
                    page.HardEnvelopeSpeed = (int)mml.args[1];
                    OutSsgHardEnvSpeed(page, mml);
                    break;
                case "EHON":
                    page.HardEnvelopeSw = true;
                    break;
                case "EHOF":
                    page.HardEnvelopeSw = false;
                    break;
                case "EHT":
                    page.HardEnvelopeType = (int)mml.args[1];
                    OutSsgHardEnvType(page, mml);
                    break;
            }
        }

        private void OutSsgHardEnvType(partPage page, MML mml)
        {
            if (page.spg.HardEnvelopeType != page.HardEnvelopeType)
            {
                //parent.OutData(mml, port[0], 0x0d, (byte)(n & 0xf));
                SOutData(page, mml, port[0], 0x0d, (byte)(page.HardEnvelopeType & 0xf));
                page.spg.HardEnvelopeType = page.HardEnvelopeType;
            }
        }

        private void OutSsgHardEnvSpeed(partPage page, MML mml)
        {
            if (page.spg.HardEnvelopeSpeed != page.HardEnvelopeSpeed)
            {
                //parent.OutData(mml, port[0], 0x0b, (byte)(n & 0xff));
                //parent.OutData(mml, port[0], 0x0c, (byte)((n >> 8) & 0xff));
                SOutData(page, mml, port[0], 0x0b, (byte)(page.HardEnvelopeSpeed & 0xff));
                SOutData(page, mml, port[0], 0x0c, (byte)((page.HardEnvelopeSpeed >> 8) & 0xff));
                page.spg.HardEnvelopeSpeed = page.HardEnvelopeSpeed;
            }
        }

        public override void CmdY(partPage page, MML mml)
        {
            if (mml.args[0] is string) return;

            int adr = (byte)(int)mml.args[0];
            byte dat = (byte)(int)mml.args[1];

            //parent.OutData(mml, port[0], (byte)adr, (byte)dat);
            SOutData(page, mml, port[0], (byte)adr, (byte)dat);
        }

        public override void CmdLoopExtProc(partPage page, MML mml)
        {
        }

        public override void CmdDetune(partPage page, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, -0xfff, 0xfff);
            page.detune = n;
            SetDummyData(page, mml);
        }

        public override void SetupPageData(partWork pw, partPage page)
        {
            //周波数
            page.spg.freq = -1;
            SetFNum(page, null);

            //ノイズ周波数
            noiseFreq = -1;
            OutSsgNoise(null, page);

            //ハードエンベロープtype
            page.spg.HardEnvelopeType = -1;
            OutSsgHardEnvType(page, null);

            //ハードエンベロープspeed
            page.spg.HardEnvelopeSpeed = -1;
            OutSsgHardEnvSpeed(page, null);

            //音量
            page.spg.beforeVolume = -1;
            SetVolume(page, null);

        }

        public override void MultiChannelCommand(MML mml)
        {
            foreach (partWork pw in lstPartWork)
            {
                foreach (partPage page in pw.pg)
                {
                }
            }
        }

    }
}