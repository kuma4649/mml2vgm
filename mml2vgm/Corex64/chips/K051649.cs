﻿using musicDriverInterface;
using System;
using System.Collections.Generic;

namespace Corex64
{
    public class K051649 : ClsChip
    {
        private byte keyOnStatus = 0;
        private byte keyOnStatusOld = 0;

        public K051649(ClsVgm parent, int chipID, string initialPartName, string stPath, int chipNumber) : base(parent, chipID, initialPartName, stPath, chipNumber)
        {
            _chipType = enmChipType.K051649;
            _Name = "K051649";
            _ShortName = "SCC";
            _ChMax = 5;
            _canUsePcm = false;
            _canUsePI = false;
            ChipNumber = chipNumber;

            Frequency = 1789772;
            port = new byte[][] { new byte[] { 0xd2 } };

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
                ch.Type = enmChannelType.WaveForm;
                ch.chipNumber = chipID == 1;
                ch.MaxVolume = 15;
            }

            Envelope = new Function();
            Envelope.Max = 15;
            Envelope.Min = 0;

        }

        public override void InitChip()
        {
            if (!use) return;

            //SCC1 mode
            //isK052539 = false;

            //keyOnOff : 0
            parent.OutData((MML)null, port[0], (byte)((ChipNumber != 0 ? 0x80 : 0x00) + 3), 0, 0);
            keyOnStatus = 0;
            keyOnStatusOld = 0;

            for (int i = 0; i < _ChMax; i++)
            {
                //freq : 0
                parent.OutData((MML)null, port[0], (byte)((ChipNumber != 0 ? 0x80 : 0x00) + 1), (byte)(i * 2 + 0), 0);
                parent.OutData((MML)null, port[0], (byte)((ChipNumber != 0 ? 0x80 : 0x00) + 1), (byte)(i * 2 + 1), 0);

                //volume : 0
                parent.OutData((MML)null, port[0], (byte)((ChipNumber != 0 ? 0x80 : 0x00) + 2), (byte)i, 0);

                //WaveForm : all 0
                if (parent.info.isK052539 || i < 4) //K051の場合は4Ch分の初期化を行う
                {
                    for (int j = 0; j < 32; j++)
                    {
                        parent.OutData((MML)null, port[0], (byte)((ChipNumber != 0 ? 0x80 : 0x00) + (parent.info.isK052539 ? 4 : 0)), (byte)(i * 32 + j), 0);
                    }
                }
            }

            if (ChipID != 0 && parent.info.format != enmFormat.ZGM)
            {
                parent.dat[0x9f] = new outDatum(enmMMLType.unknown, null, null, (byte)(parent.dat[0x9f].val | 0x40));//use Secondary
            }
        }

        public override void InitPart(partWork pw)
        {
            foreach (partPage pg in pw.pg)
            {
                pg.FNum = 0;
                pg.beforeFNum = -1;
                pg.MaxVolume = 15;
                pg.volume = pg.MaxVolume;
                pg.beforeLVolume = -1;
                pg.beforeEnvInstrument = -1;
                pg.keyOn = false;
                pg.port = port;
            }
        }

        public void OutK051649Port(partPage page, MML mml, byte[] cmd, int chipNumber, byte port, byte adr, byte data)
        {
            SOutData(
                page,
                mml,
                cmd
                , (byte)((chipNumber != 0 ? 0x80 : 0x00) + port)
                , adr
                , data);
        }

        public void OutK051649SetInstrument(partPage page, MML mml, int n)
        {

            if (!parent.instWF.ContainsKey(n))
            {
                msgBox.setWrnMsg(string.Format(msg.get("E10021"), n), mml.line.Lp);
                return;
            }

            for (int i = 1; i < parent.instWF[n].Item2.Length; i++) // 添え字0 は音色番号が入っている為1からスタート
            {
                int ch = page.ch;
                if (!parent.info.isK052539 && ch == 4) ch = 3;

                OutK051649Port(
                    page,
                    mml, port[0],
                    page.chipNumber
                    , (byte)(parent.info.isK052539 ? 4 : 0)
                    , (byte)(ch * 32 + i - 1)
                    , parent.instWF[n].Item2[i]);
            }

        }

        public override void StorePcm(Dictionary<int,Tuple<string, clsPcm>> newDic, KeyValuePair<int, clsPcm> v, byte[] buf, bool is16bit, int samplerate, params object[] option)
        {
        }

        public override void StorePcmRawData(clsPcmDatSeq pds, byte[] buf, bool isRaw, bool is16bit, int samplerate, params object[] option)
        {
        }

        public void OutSsgKeyOn(MML mml, partPage page)
        {
            SetSsgVolume(mml, page);
            page.keyOn = true;
        }

        public void OutSsgKeyOff(MML mml, partPage page)
        {
            SetSsgVolume(mml, page);
            page.keyOn = false;
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

                vol += page.lfo[lfo].value + page.lfo[lfo].phase;
            }

            if (page.varpeggioMode)
            {
                vol += page.varpDelta;
            }

            vol = Common.CheckRange(vol, 0, 15);
            page.beforeVolume = vol;

            if (page.spg.beforeVolume != vol)
            {
                OutK051649Port(page, mml, port[0], ChipNumber, 2, (byte)page.ch, (byte)vol);
                page.spg.beforeVolume = vol;
            }
        }

        public void SetSsgFNum(partPage page)
        {
            int arpNote = page.arpFreqMode ? 0 : page.arpDelta;
            int arpFreq = page.arpFreqMode ? page.arpDelta : 0;

            int f = GetSsgFNum(page, page.octaveNow, page.noteCmd, page.shift + page.keyShift + arpNote, page.pitchShift);//
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
                f += page.lfo[lfo].value + page.lfo[lfo].phase;
            }

            f = Common.CheckRange(f, 0, 0xfff);
            page.FNum = f;
        }

        public int GetSsgFNum(partPage page, int octave, char noteCmd, int shift, int pitchShift)
        {
            int o = octave - 1;
            int n = Const.NOTE.IndexOf(noteCmd) + shift;
            o += n / 12;
            o = Common.CheckRange(o, 0, 7);
            n %= 12;

            int f = o * 12 + n;
            f += pitchShift;
            if (f < 0) f = 0;
            if (f >= FNumTbl[0].Length) f = FNumTbl[0].Length - 1;

            return FNumTbl[0][f];
        }

        public override int GetFNum(partPage page, MML mml, int octave, char cmd, int shift, int pitchShift)
        {
            return GetSsgFNum(page, octave, cmd, shift, pitchShift);
        }


        public override void SetFNum(partPage page, MML mml)
        {
            SetSsgFNum(page);
        }

        public override void SetKeyOn(partPage page, MML mml)
        {
            OutSsgKeyOn(mml, page);
            SetDummyData(page, mml);
        }

        public override void SetKeyOff(partPage page, MML mml)
        {
            OutSsgKeyOff(mml, page);
        }

        public override void SetVolume(partPage page, MML mml)
        {
            SetSsgVolume(mml, page);
        }

        public override void SetLfoAtKeyOn(partPage page, MML mml)
        {
            for (int lfo = 0; lfo < 4; lfo++)
            {
                clsLfo pl = page.lfo[lfo];
                if (!pl.sw)
                    continue;
                if (pl.type == eLfoType.Wah) continue;

                if (pl.param[5] != 1)
                    continue;

                pl.isEnd = false;
                pl.value = 0;// (pl.param[0] == 0) ? pl.param[6] : 0;//ディレイ中は振幅補正は適用されない
                pl.phase = (pl.param[0] == 0) ? pl.param[6] : 0;//ディレイ中は振幅補正は適用されない
                pl.waitCounter = pl.param[0];
                pl.direction = pl.param[2] < 0 ? -1 : 1;
                pl.depthWaitCounter = pl.param[7];
                pl.depth = pl.param[3];
                pl.depthV2 = pl.param[2];

            }
        }

        public override int GetToneDoublerShift(partPage page, int octave, char noteCmd, int shift)
        {
            //実装不要
            return 0;
        }

        public override void SetToneDoubler(partPage page, MML mml)
        {
            //実装不要
        }

        public override void CmdY(partPage page, MML mml)
        {
            if (mml.args[0] is string) return;

            byte port = (byte)((int)mml.args[0] >> 8);
            byte adr = (byte)(int)mml.args[0];
            byte dat = (byte)(int)mml.args[1];

            OutK051649Port(page, mml, this.port[0], page.chipNumber, port, adr, dat);
        }

        public override void CmdLoopExtProc(partPage page, MML mml)
        {
            if (page.chip is K051649 && page.chip.use)
            {
                page.beforeFNum = -1;
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

            if (type == 'I')
            {
                msgBox.setErrMsg(msg.get("E10019"), mml.line.Lp);
                return;
            }

            if (type == 'T')
            {
                msgBox.setErrMsg(msg.get("E10020"), mml.line.Lp);
                return;
            }

            if (type == 'E')
            {
                n = SetEnvelopParamFromInstrument(page, n, re, mml);
                return;
            }

            if (re) n = page.instrument + n;
            page.instrument = Common.CheckRange(n, 0, 255);
            if (page.spg.instrument!= page.instrument)
            {
                page.spg.instrument = page.instrument;
                ((K051649)page.chip).OutK051649SetInstrument(page, mml, n);
            }

        }

        public override void CmdDetune(partPage page, MML mml)
        {
            string cmd = (string)mml.args[0];
            int n = (int)mml.args[1];

            switch (cmd)
            {
                case "D":
                    page.detune = n;
                    break;
                case "D>":
                    page.detune += parent.info.octaveRev ? -n : n;
                    break;
                case "D<":
                    page.detune += parent.info.octaveRev ? n : -n;
                    break;
            }
            page.detune = Common.CheckRange(page.detune, -0xfff, 0xfff);

            SetDummyData(page, mml);
        }



        public override void SetupPageData(partWork pw, partPage page)
        {

            page.spg.keyOff = true;
            //SetKeyOff(page, null);

            page.spg.instrument = -1;

            //周波数
            page.spg.freq = -1;
            page.spg.beforeFNum = -1;

            //音量
            page.spg.beforeVolume = -1;
            SetVolume(page, null);

        }

        public override void MultiChannelCommand(MML mml)
        {
            if (!use) return;

            for (int i = 0; i < ChMax; i++)
            {
                partPage page = lstPartWork[i].cpg;
                //fnum
                if (page.beforeFNum != page.FNum)
                {
                    byte data = (byte)page.FNum;
                    OutK051649Port(page, mml, port[0], ChipNumber, 1, (byte)(0 + page.ch * 2), data);

                    data = (byte)((page.FNum & 0xf00) >> 8);
                    OutK051649Port(page, mml, port[0], ChipNumber, 1, (byte)(1 + page.ch * 2), data);
                    page.beforeFNum = page.FNum;
                }

                //instrument
                if (page.spg.instrument != page.instrument)
                {
                    page.spg.instrument = page.instrument;
                    ((K051649)page.chip).OutK051649SetInstrument(page, mml, page.instrument);
                }

                //volume
                SetSsgVolume(mml, page);

                //keyonoff
                if (page.keyOn)
                {
                    keyOnStatus |= (byte)(1 << page.ch);
                }
                else
                {
                    keyOnStatus &= (byte)~(1 << page.ch);
                }
            }

            //keyonoff
            if (keyOnStatus != keyOnStatusOld)
            {
                OutK051649Port(lstPartWork[lstPartWork.Count - 1].cpg, mml, port[0], ChipNumber, 3, 0, keyOnStatus);
                keyOnStatusOld = keyOnStatus;
            }

        }

    }
}
