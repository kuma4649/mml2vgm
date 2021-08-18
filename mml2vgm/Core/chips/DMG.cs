using musicDriverInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class DMG : ClsChip
    {
        private const byte NR10 = 0x00;
        private const byte NR11 = 0x01;
        private const byte NR12 = 0x02;
        private const byte NR13 = 0x03;
        private const byte NR14 = 0x04;
        // 0x05       
        private const byte NR21 = 0x06;
        private const byte NR22 = 0x07;
        private const byte NR23 = 0x08;
        private const byte NR24 = 0x09;
        private const byte NR30 = 0x0A;
        private const byte NR31 = 0x0B;
        private const byte NR32 = 0x0C;
        private const byte NR33 = 0x0D;
        private const byte NR34 = 0x0E;
        // 0x0F       
        private const byte NR41 = 0x10;
        private const byte NR42 = 0x11;
        private const byte NR43 = 0x12;
        private const byte NR44 = 0x13;
        private const byte NR50 = 0x14;
        private const byte NR51 = 0x15;
        private const byte NR52 = 0x16;
        // 0x17 - 0x1F
        private const byte AUD3W0 = 0x20;
        private const byte AUD3W1 = 0x21;
        private const byte AUD3W2 = 0x22;
        private const byte AUD3W3 = 0x23;
        private const byte AUD3W4 = 0x24;
        private const byte AUD3W5 = 0x25;
        private const byte AUD3W6 = 0x26;
        private const byte AUD3W7 = 0x27;
        private const byte AUD3W8 = 0x28;
        private const byte AUD3W9 = 0x29;
        private const byte AUD3WA = 0x2A;
        private const byte AUD3WB = 0x2B;
        private const byte AUD3WC = 0x2C;
        private const byte AUD3WD = 0x2D;
        private const byte AUD3WE = 0x2E;
        private const byte AUD3WF = 0x2F;

        private byte pan = 0xff;

        public DMG(ClsVgm parent, int chipID, string initialPartName, string stPath, int chipNumber) : base(parent, chipID, initialPartName, stPath, chipNumber)
        {
            _chipType = enmChipType.DMG;
            _Name = "DMG";
            _ShortName = "DMG";
            _ChMax = 4;
            _canUsePcm = true;
            _canUsePI = false;
            ChipNumber = chipNumber;

            Frequency = 4194304;
            port = new byte[][] { new byte[] { 0xb3 } };
            vgmHeaderPos = 0x80;
            DataBankID = 0xb;
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
                ch.Type = c < 2 ? enmChannelType.Pulse : (c == 2 ? enmChannelType.WaveForm : enmChannelType.Noise);
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
                pg.dutyCycle = 0;
                pg.spg.dutyCycle = -1;
                pg.panL = 3;
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

            parent.OutData((MML)null, port[0], new byte[] { NR52, 0x80 });//Sound ON
            parent.OutData((MML)null, port[0], new byte[] { NR30, 0x80 });//Ch3 Enable

            //set volume 0
            parent.OutData((MML)null, port[0], new byte[] { NR12, 0x00 });//pls1
            parent.OutData((MML)null, port[0], new byte[] { NR22, 0x00 });//pls2
            parent.OutData((MML)null, port[0], new byte[] { NR32, 0x00 });//wf
            parent.OutData((MML)null, port[0], new byte[] { NR14, 0x00 });//pls1
            parent.OutData((MML)null, port[0], new byte[] { NR24, 0x00 });//pls2
            parent.OutData((MML)null, port[0], new byte[] { NR34, 0x00 });//wf
            parent.OutData((MML)null, port[0], new byte[] { NR42, 0x00 });//noise
            parent.OutData((MML)null, port[0], new byte[] { NR50, 0x77 });//total
            parent.OutData((MML)null, port[0], new byte[] { NR51, 0xff });//pan
            pan = 0xff;

            //ヘッダの調整
            if (parent.info.format == enmFormat.VGM && ChipID != 0)
            {
                parent.dat[vgmHeaderPos + 3] = new outDatum(enmMMLType.unknown, null, null, (byte)(parent.dat[vgmHeaderPos + 3].val | 0x40));//use Secondary
            }
        }

        public override void StorePcm(Dictionary<int, Tuple<string, clsPcm>> newDic, KeyValuePair<int, clsPcm> v, byte[] buf, bool is16bit, int samplerate, params object[] option)
        {
        }

        public override void StorePcmRawData(clsPcmDatSeq pds, byte[] buf, bool isRaw, bool is16bit, int samplerate, params object[] option)
        {
        }




        public void OutKeyOn(MML mml, partPage page)
        {
            if (!page.pcm)
            {
                page.keyOn = true;

                SetVolume(page, mml);

                return;
            }

        }

        public void OutKeyOff(MML mml, partPage page)
        {
            page.keyOff = true;
        }

        public override int GetToneDoublerShift(partPage page, int octave, char noteCmd, int shift)
        {
            return 0;
        }

        public override void SetToneDoubler(partPage page, MML mml)
        {
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

            int f = GetSsgFNum(page, mml, page.octaveNow, page.noteCmd, page.shift + page.keyShift + arpNote, page.pitchShift);//
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
            else if (page.Type == enmChannelType.Noise || page.Type == enmChannelType.DPCM)
            {
                f = Common.CheckRange(f, 0, 0xf);
                page.FNum = f;
            }
            else if (page.Type == enmChannelType.WaveForm)
            {
                f = Common.CheckRange(f, 0, 0x7ff);
                page.FNum = f;
            }
        }

        public int GetSsgFNum(partPage page, MML mml, int octave, char noteCmd, int shift, int pitchShift)
        {
            int o = octave - 1;
            int n = Const.NOTE.IndexOf(noteCmd) + shift;
            o += n / 12;

            if (page.Type == enmChannelType.Pulse|| page.Type == enmChannelType.WaveForm)
            {
                o = Common.CheckRange(o, 0, 7);
                n %= 12;

                int f = o * 12 + n;
                if (f < 0) f = 0;
                if (f >= FNumTbl[0].Length) f = FNumTbl[0].Length - 1;
                return FNumTbl[0][f] + pitchShift;
            }
            else if (page.Type == enmChannelType.Noise)
            {
                o = Common.CheckRange(o, 0, 1);
                n %= 12;
                n += o * 12;
                n = 15 - n;
                n += pitchShift;

                int f = Common.CheckRange(n, 0, 0xf);
                return f;
            }

            return 0;
        }

        public override int GetFNum(partPage page, MML mml, int octave, char cmd, int shift, int pitchShift)
        {
            return GetSsgFNum(page, mml, octave, cmd, shift, pitchShift);
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
                if (pl.type == eLfoType.Hardware) continue;
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
                        {
                            //キーオフした直後
                            if (page.keyOff)
                            {
                                SOutData(page, mml, port[0], page.ch == 0 ? NR12 : NR22, 0);
                                //SOutData(page, mml, port[0], page.ch == 0 ? NR14 : NR24, 0);
                            }

                            //
                            // Sweepの更新
                            //

                            if (page.keyOn && page.ch == 0)
                            {
                                for (int i = 0; i < page.lfo.Length; i++)
                                {
                                    if (page.lfo[i].type != eLfoType.Hardware) continue;

                                    if (!page.lfo[i].sw)
                                    {
                                        SOutData(page, mml, port[0], NR10, (byte)0x00);
                                        continue;
                                    }

                                    vol = ((page.lfo[i].depthWaitCounter & 0x7) << 4)
                                        | (page.lfo[i].depth < 0 ? 0x08 : 0x00)
                                        | (Math.Abs(page.lfo[i].depth) & 0x7);
                                    SOutData(page, mml, port[0], NR10, (byte)vol);
                                }
                            }

                            //
                            // Duty比とLengthの更新
                            //
                            if (page.dutyCycle != page.spg.dutyCycle)
                            {
                                //bit7-6:duty bit5-0:sound length
                                vol = ((page.dutyCycle & 0x3) << 6) | (page.HardEnvelopeSw ? page.HardEnvelopeType : 0);
                                SOutData(page, mml, port[0], page.ch == 0 ? NR11 : NR21, (byte)vol);
                            }

                            //
                            // ボリュームの更新(キーオンのまえに行う必要あり)
                            //

                            if (page.keyOn 
                                || page.latestVolume != page.beforeVolume)
                            {
                                //bit7-4:envelope(volume) bit3:envelope direction bit2-0:envelope time
                                vol = ((page.latestVolume & 0xf) << 4) | (
                                    (page.HardEnvelopeSpeed > 0 ? 0x0 : 0x8) | Math.Abs(page.HardEnvelopeSpeed)
                                    );
                                SOutData(page, mml, port[0], page.ch == 0 ? NR12 : NR22, (byte)vol);
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
                                byte data = (byte)f;
                                //frqLow(8bit)
                                SOutData(page, mml, port[0], page.ch == 0 ? NR13 : NR23, data);

                                //キーオンした直後
                                //または周波数上位が違う　場合は周波数上位再セット
                                //(上位をセットすると位相とエンベロープがリセットされる)
                                if (page.keyOn 
                                    || (page.FNum != page.beforeFNum && (page.FNum & 0x700) != (page.beforeFNum & 0x700)))
                                {
                                    //bit7:phaseReset bit6:length_enable bit2-0:frqHi(3bit)
                                    data = (byte)((page.keyOn ? 0x80 : 0x00) | (page.HardEnvelopeSw ? 0x40 : 0x00) | ((f >> 8) & 0x7));
                                    SOutData(page, mml, port[0], page.ch == 0 ? NR14 : NR24, data);
                                }
                            }


                        }
                        break;
                    case enmChannelType.Noise:
                        {
                            //キーオフした直後
                            if (page.keyOff)
                            {
                                vol = 0x00;
                                SOutData(page, mml, port[0], NR42, (byte)vol);//0x4089 この方法あってるのかなぁ
                            }


                            //
                            // Duty比とLengthの更新
                            //
                            if (page.dutyCycle != page.spg.dutyCycle)
                            {
                                //bit5-0:sound length
                                vol = page.HardEnvelopeSw ? page.HardEnvelopeType : 0;
                                SOutData(page, mml, port[0], NR41, (byte)vol);
                            }

                            //
                            // ボリュームの更新
                            //

                            if (page.keyOn 
                                || page.latestVolume != page.beforeVolume)
                            {
                                vol = ((page.latestVolume & 0xf) << 4) | (
                                    (page.HardEnvelopeSpeed > 0 ? 0x0 : 0x8) | Math.Abs(page.HardEnvelopeSpeed)
                                    );
                                SOutData(page, mml, port[0], NR42, (byte)vol);
                            }

                            //
                            // 周波数の更新(と位相、エンベロープの初期化)
                            //

                            //キーオンした直後
                            //または周波数が違う　場合は周波数再セット
                            if (page.keyOn
                                || page.FNum != page.beforeFNum)
                            {
                                //0-15(4bit) 0,1(1bit) 0-7(3bit)
                                int f = ((page.FNum & 0xf) << 4) | (page.mixer & 0x8) | (page.noise & 0x7);
                                byte data = (byte)f;
                                SOutData(page, mml, port[0], NR43, data);

                                //キーオンした直後
                                //または周波数上位が違う　場合は周波数上位再セット
                                //(上位をセットすると位相とエンベロープがリセットされる)
                                if (page.keyOn)
                                    //|| (page.FNum != page.beforeFNum && (page.FNum & 0x700) != (page.beforeFNum & 0x700)))
                                {
                                    //bit7:keyon
                                    data = (byte)((page.keyOn ? 0x80 : 0x00) | (page.HardEnvelopeSw ? 0x40 : 0x00));
                                    SOutData(page, mml, port[0], NR44, data);
                                }
                            }


                        }
                        break;
                    case enmChannelType.WaveForm:
                        {
                            //キーオフした直後
                            if (page.keyOff)
                            {
                                vol = 0x00;
                                SOutData(page, mml, port[0], NR32 , (byte)vol);// この方法あってるのかなぁ
                            }


                            //
                            // Duty比とLengthの更新
                            //
                            if (page.dutyCycle != page.spg.dutyCycle)
                            {
                                //bit5-0:sound length
                                vol = page.HardEnvelopeSw ? page.HardEnvelopeType : 0;
                                SOutData(page, mml, port[0], NR31, (byte)vol);
                            }


                            //
                            // ボリュームの更新(キーオンのまえに行う必要あり)
                            //

                            if (page.keyOn
                                || page.latestVolume != page.beforeVolume)
                            {
                                vol = (((4 - page.latestVolume) & 0x3) << 5);
                                SOutData(page, mml, port[0], NR32 , (byte)vol);
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
                                byte data = (byte)f;
                                //frqLow(8bit)
                                SOutData(page, mml, port[0], NR33, data);

                                //キーオンした直後
                                //または周波数上位が違う　場合は周波数上位再セット
                                //(上位をセットすると位相とエンベロープがリセットされる)
                                if (page.keyOn 
                                    || (page.FNum != page.beforeFNum && (page.FNum & 0x700) != (page.beforeFNum & 0x700)))
                                {
                                    //bit7:keyon bit3:length_enable bit2-0:frqHi(3bit)
                                    data = (byte)((page.keyOn ? 0x80 : 0x00) | (page.HardEnvelopeSw ? 0x40 : 0x00) | ((f >> 8) & 0x7));
                                    SOutData(page, mml, port[0], NR34, data);
                                }
                            }

                        }
                        break;
                }

                page.keyOff = false;
                page.keyOn = false;
                page.beforeVolume = page.latestVolume;
                page.beforeFNum = page.FNum;
                page.spg.dutyCycle = page.dutyCycle;

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

            //Duty比切り替え
            if (type == 'I')
            {
                if (page.Type == enmChannelType.Pulse)
                {
                    if (re) n = page.instrument + n;
                    n = Common.CheckRange(n, 0, 3);
                    page.dutyCycle = n;
                    SetDummyData(page, mml);
                    return;
                }

                msgBox.setErrMsg(msg.get("E32000"), mml.line.Lp);
                return;
            }

            //E指定　ソフトエンベロープ切り替え
            if (type == 'E')
            {
                SetEnvelopParamFromInstrument(page, n, re, mml);
                return;
            }

            if (page.Type == enmChannelType.Pulse)
            {
                if (re) n = page.instrument + n;
                n = Common.CheckRange(n, 0, 3);
                page.dutyCycle = n;
                SetDummyData(page, mml);
                return;
            }
            else if (page.Type == enmChannelType.WaveForm)
            {
                if (re) n = page.instrument + n;
                n = Common.CheckRange(n, 0, 255);
                if (!parent.instWF.ContainsKey(n))
                {
                    msgBox.setErrMsg(string.Format(msg.get("E32004"), n)
                        , mml.line.Lp);
                    return;
                }

                SetWaveFormFromInstrument(page, n, mml);
                return;
            }

            //無指定の場合は ソフトエンベロープ切り替え
            SetEnvelopParamFromInstrument(page, n, re, mml);

        }

        /// <summary>
        /// Ch3:波形書き換え
        /// </summary>
        private void SetWaveFormFromInstrument(partPage page, int n, MML mml)
        {
            SOutData(page, mml, port[0], NR30, 0x00);
            page.beforeFNum = -1;
            for (int i = 0; i < 16; i++)
            {
                // 0 は音色番号が入っている為1からスタート
                SOutData(page, mml, port[0], (byte)(0x20 + i)
                    , (byte)(
                    ((parent.instWF[n].Item2[i * 2 + 0 + 1] & 0xf)<<4)
                    | (parent.instWF[n].Item2[i * 2 + 1 + 1] & 0xf)
                    ));
            }
            SOutData(page, mml, port[0], NR30, 0x80);


        }

        public override void CmdNoiseToneMixer(partPage page, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 0, 1);
            page.mixer = n == 1 ? 0x8 : 0x0;
        }

        public override void CmdNoise(partPage page, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 0, 7);
            page.noise = n;
        }

        public override void CmdHardEnvelope(partPage page, MML mml)
        {
            string cmd = (string)mml.args[0];

            switch (cmd)
            {
                case "EH":
                    page.HardEnvelopeSpeed = Common.CheckRange((int)mml.args[1], -7, 7);
                    break;
                case "EHON":
                    page.HardEnvelopeSw = true;
                    break;
                case "EHOF":
                    page.HardEnvelopeSw = false;
                    break;
                case "EHT":
                    page.HardEnvelopeSw = true;
                    if(page.Type!= enmChannelType.WaveForm)
                        page.HardEnvelopeType = Common.CheckRange((int)mml.args[1], 0, 63);
                    else
                        page.HardEnvelopeType = Common.CheckRange((int)mml.args[1], 0, 255);
                    page.spg.dutyCycle = -1;//noise chも含む(フラグとして使っている...)
                    break;
            }
        }

        public override void CmdPan(partPage page, MML mml)
        {
            int l = (int)mml.args[0];

            l = Common.CheckRange(l, 0, 3);
            page.panL = l;

            pan &= (byte)(~(0x11 << page.ch));
            pan |= (byte)( ((page.panL & 1) * 0x1 | (page.panL & 2) * 0x08) << page.ch);
            SOutData(page, mml, port[0], NR51, pan);
        }

        public override void CmdY(partPage page, MML mml)
        {
            if (mml.args[0] is string) return;

            byte adr = (byte)(int)mml.args[0];
            byte dat = (byte)(int)mml.args[1];

            SOutData(page, mml, port[0],adr, dat);
        }

        public override void CmdLfo(partPage page, MML mml)
        {
            base.CmdLfo(page, mml);

            int c = (char)mml.args[0] - 'P';
            if (page.lfo[c].type != eLfoType.Hardware) return;


            if (page.lfo[c].param.Count < 2)
            {
                msgBox.setErrMsg(msg.get("E32002"), mml.line.Lp);
                return;
            }
            if (page.lfo[c].param.Count > 2)
            {
                msgBox.setErrMsg(msg.get("E32003"), mml.line.Lp);
                return;
            }

            page.lfo[c].param[0] = Common.CheckRange(page.lfo[c].param[0], 0, 7);//Speed
            page.lfo[c].param[1] = Common.CheckRange(page.lfo[c].param[1], -7, 7);//Shift count
            page.lfo[c].depthWaitCounter = page.lfo[c].param[0];
            page.lfo[c].depth = page.lfo[c].param[1];

        }

        public override void CmdLfoSwitch(partPage page, MML mml)
        {
            base.CmdLfoSwitch(page, mml);

            int c = (char)mml.args[0] - 'P';
            int n = (int)mml.args[1];
            if (page.lfo[c].type == eLfoType.Hardware)
            {
                if (n == 0)
                {
                    page.lfo[c].sw = false;
                }
                else
                {
                    page.lfo[c].sw = true;
                    page.lfo[c].depthWaitCounter = page.lfo[c].param[0];
                    page.lfo[c].depth = page.lfo[c].param[1];
                }
            }
        }

    }
}
