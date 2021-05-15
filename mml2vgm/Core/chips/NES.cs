using musicDriverInterface;
using System.Collections.Generic;

namespace Core
{
    public class NES : ClsChip
    {
        public NES(ClsVgm parent, int chipID, string initialPartName, string stPath, int chipNumber) : base(parent, chipID, initialPartName, stPath, chipNumber)
        {
            _chipType = enmChipType.NES;
            _Name = "NES";
            _ShortName = "NES";
            _ChMax = 5;
            _canUsePcm = true;
            _canUsePI = false;
            ChipNumber = chipNumber;

            Frequency = 1789772;//NTSC (PAL=1662607)
            port = new byte[][] { new byte[] { 0xb4 } };
            vgmHeaderPos = 0x84;
            DataBankID = 0xa;
            int c = 0;
            if (string.IsNullOrEmpty(initialPartName)) return;

            Dictionary<string, List<double>> dic = MakeFNumTbl();
            if (dic != null)
            {
                c = 0;
                FNumTbl = new int[1][] { new int[96] };
                foreach (double v in dic["FNUM_00"])
                {
                    FNumTbl[0][c++] = (int)v;
                    if (c == FNumTbl[0].Length) break;
                }
                if (dic.ContainsKey("MASTERCLOCK"))
                {
                    Frequency = (int)dic["MASTERCLOCK"][0];
                }
            }

            Ch = new ClsChannel[ChMax];
            SetPartToCh(Ch, initialPartName);
            c = 0;
            foreach (ClsChannel ch in Ch)
            {
                ch.Type = c < 2 ? enmChannelType.Pulse : (c == 2 ? enmChannelType.Triangle : (c == 3 ? enmChannelType.Noise : enmChannelType.DPCM));
                ch.chipNumber = chipID == 1;
                ch.MaxVolume = 15;
                c++;
            }

            Envelope = new Function();
            Envelope.Max = 15;
            Envelope.Min = 0;

            pcmDataInfo = new clsPcmDataInfo[] { new clsPcmDataInfo() };
            pcmDataInfo[0].totalBufPtr = 0L;
            pcmDataInfo[0].use = false;

            if (parent.info.format == enmFormat.ZGM)
            {
                if (parent.ChipCommandSize == 2)
                {
                    if (chipNumber == 0) pcmDataInfo[0].totalBuf = new byte[] { 0x07, 0x00, 0x66, DataBankID, 0x00, 0x00, 0x00, 0x00 };
                    else pcmDataInfo[0].totalBuf = new byte[] { 0x07, 0x00, 0x66, DataBankID, 0x00, 0x00, 0x00, 0x00 };
                }
                else
                {
                    if (chipNumber == 0) pcmDataInfo[0].totalBuf = new byte[] { 0x07, 0x66, DataBankID, 0x00, 0x00, 0x00, 0x00 };
                    else pcmDataInfo[0].totalBuf = new byte[] { 0x07, 0x66, DataBankID, 0x00, 0x00, 0x00, 0x00 };
                }
            }
            else
            {
                if (chipNumber == 0) pcmDataInfo[0].totalBuf = new byte[] { 0x67, 0x66, DataBankID, 0x00, 0x00, 0x00, 0x00 };
                else pcmDataInfo[0].totalBuf = new byte[] { 0x67, 0x66, DataBankID, 0x00, 0x00, 0x00, 0x00 };
            }

            pcmDataInfo[0].totalHeaderLength = pcmDataInfo[0].totalBuf.Length;
            pcmDataInfo[0].totalHeadrSizeOfDataPtr = (parent.ChipCommandSize == 2) ? 4 : 3;

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

            for (int ch = 0; ch < ChMax; ch++)
            {
                OutKeyOff(null, lstPartWork[ch].cpg);
                lstPartWork[ch].apg.volume = 0;
            }

            //set volume 0
            parent.OutData((MML)null, port[0], new byte[] { 0x00, 0x00 });//pls1
            parent.OutData((MML)null, port[0], new byte[] { 0x04, 0x00 });//pls2
            parent.OutData((MML)null, port[0], new byte[] { 0x08, 0x00 });//tri
            parent.OutData((MML)null, port[0], new byte[] { 0x0c, 0x00 });//noise
            parent.OutData((MML)null, port[0], new byte[] { 0x09, 0x00 });//dpcm

            //ヘッダの調整
            if (ChipID != 0)
            {
                parent.dat[vgmHeaderPos + 3] = new outDatum(enmMMLType.unknown, null, null, (byte)(parent.dat[vgmHeaderPos + 3].val | 0x40));//use Secondary
            }
        }

        public void OutKeyOn(MML mml, partPage page)
        {
            if (!page.pcm)
            {
                page.keyOn = true;
                return;
            }

            page.keyOn = true;
        }

        public void OutKeyOff(MML mml, partPage page)
        {
            page.keyOff = true;
        }

        public override void SetKeyOn(partPage page, MML mml)
        {
            OutKeyOn(mml, page);
        }

        public override void SetKeyOff(partPage page, MML mml)
        {
            OutKeyOff(mml, page);
        }

        public override void SetVolume(partPage page, MML mml)
        {
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

            page.latestVolume = vol;
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

            if (page.Type == enmChannelType.Pulse || page.Type == enmChannelType.Triangle)
            {
                f = Common.CheckRange(f, 0, 0x7ff);
                page.FNum = f;
            }
            else if (page.Type == enmChannelType.Noise)
            {
                int o = page.octaveNow - 1;
                int n = Const.NOTE.IndexOf(page.noteCmd) + page.shift + page.keyShift + arpNote;
                o += n / 12;
                o = Common.CheckRange(o, 0, 1);
                n %= 12;

                f = 15 - Common.CheckRange(o * 12 + n, 0, 0xf);
                page.FNum = f;
            }
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
                    SetFNum(page, mml);

                if (pl.type == eLfoType.Tremolo)
                    SetVolume(page, mml);

            }
        }


        public override void MultiChannelCommand(MML mml)
        {
            foreach (partWork pw in lstPartWork)
            {
                partPage page = pw.cpg;
                int vol;

                switch (page.Type)
                {
                    case enmChannelType.Pulse:

                        //キーオフした直後
                        if (page.keyOff)
                        {
                            vol = 0x10;//仮
                            SOutData(page, mml, port[0], (byte)(0x00 + page.ch * 4), (byte)vol);
                        }


                        //
                        // 周波数の更新(と位相、エンベロープの初期化)
                        //

                        //キーオンした直後
                        //または周波数が違う　場合は周波数再セット
                        if (page.keyOn 
                            || page.FNum != page.beforeFNum)
                        {
                            int f = page.FNum;
                            byte data = (byte)(f & 0xff);
                            SOutData(page, mml, port[0], (byte)(2 + page.ch * 4), data);

                            //キーオンした直後
                            //または周波数上位が違う　場合は周波数上位再セット
                            //(上位をセットすると位相とエンベロープがリセットされる)
                            if (page.keyOn || (page.FNum != page.beforeFNum && (page.FNum & 0x700) != (page.beforeFNum & 0x700)))
                            {
                                data = (byte)((f & 0x700) >> 8);
                                SOutData(page, mml, port[0], (byte)(3 + page.ch * 4), data);
                            }
                        }


                        //
                        // ボリュームの更新
                        //

                        if (page.keyOn || page.latestVolume != page.beforeVolume)
                        {
                            vol = ((page.dutyCycle & 0x3) << 6) | (page.HardEnvelopeSw ? 0x20 : 0x30) | Common.CheckRange(page.latestVolume, 0, 15);//0x30持続音
                            SOutData(page, mml, port[0], (byte)(0x00 + page.ch * 4), (byte)vol);
                        }

                        break;

                    case enmChannelType.Triangle:

                        //キーオフした直後
                        if (page.keyOff)
                        {
                            vol = 0x00;//仮
                            SOutData(page, mml, port[0], 0x08 , (byte)vol);
                        }


                        //
                        // 周波数の更新(と位相、エンベロープの初期化)
                        //

                        //キーオンした直後
                        //または周波数が違う　場合は周波数再セット
                        if (page.keyOn
                            || page.FNum != page.beforeFNum)
                        {
                            int f = page.FNum;
                            byte data = (byte)(f & 0xff);
                            SOutData(page, mml, port[0], 0x0a, data);

                            //キーオンした直後
                            //または周波数上位が違う　場合は周波数上位再セット
                            //(上位をセットすると位相とエンベロープがリセットされる)
                            if (page.keyOn || (page.FNum != page.beforeFNum && (page.FNum & 0x700) != (page.beforeFNum & 0x700)))
                            {
                                data = (byte)((f & 0x700) >> 8);
                                SOutData(page, mml, port[0], 0x0b, data);
                            }
                        }


                        //
                        // ボリュームの更新
                        //

                        if (page.keyOn || page.latestVolume != page.beforeVolume)
                        {
                            vol = (page.HardEnvelopeSw ? (page.HardEnvelopeSpeed & 0x7f) : 0x81);
                            SOutData(page, mml, port[0], 0x08, (byte)vol);
                        }

                        break;
                    case enmChannelType.Noise:

                        //キーオフした直後
                        if (page.keyOff)
                        {
                            vol = 0x10;//仮
                            SOutData(page, mml, port[0], 0x0c, (byte)vol);
                        }


                        //
                        // 周波数の更新(と位相、エンベロープの初期化)
                        //

                        //キーオンした直後
                        //または周波数が違う　場合は周波数再セット
                        if (page.keyOn
                            || page.FNum != page.beforeFNum)
                        {
                            int f = page.FNum;
                            byte data = (byte)(((page.spg.noise & 1) << 7) | (f & 0xf));
                            SOutData(page, mml, port[0], 0x0e, data);

                            ////キーオンした直後
                            ////(セットすると位相とエンベロープがリセットされる)
                            if (page.keyOn)
                                SOutData(page, mml, port[0], 0x0f, 0x08);
                        }


                        //
                        // ボリュームの更新
                        //

                        if (page.keyOn || page.latestVolume != page.beforeVolume)
                        {
                            vol = (page.HardEnvelopeSw ? 0x20 : 0x30) | Common.CheckRange(page.latestVolume, 0, 15);//0x30持続音
                            SOutData(page, mml, port[0], 0x0c, (byte)vol);
                        }

                        break;
                }

                page.keyOff = false;
                page.keyOn = false;
                page.beforeVolume = page.latestVolume;
                page.beforeFNum = page.FNum;

            }

        }



        public override void CmdInstrument(partPage page, MML mml)
        {
            char type = (char)mml.args[0];
            int n = (int)mml.args[1];

            //Duty比切り替え
            if (type == 'I')
            {
                if (page.Type == enmChannelType.Pulse)
                {
                    n = Common.CheckRange(n, 0, 3);
                    page.dutyCycle = n;
                    SetDummyData(page, mml);
                    return;
                }

                msgBox.setErrMsg(msg.get("E32000"), mml.line.Lp);
                return;
            }
        }

    }
}
