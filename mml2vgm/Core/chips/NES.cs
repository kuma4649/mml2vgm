using musicDriverInterface;
using System;
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
                    if (chipNumber == 0)
                        pcmDataInfo[0].totalBuf = new byte[] {
                            0x07, 0x00, 0x66, 0xc2
                            , 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
                        };
                    else
                        pcmDataInfo[0].totalBuf = new byte[] {
                            0x07, 0x00, 0x66, 0xc2
                            , 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
                    };
                }
                else
                {
                    if (chipNumber == 0)
                        pcmDataInfo[0].totalBuf = new byte[] {
                            0x07, 0x66, 0xc2
                            , 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
                        };
                    else
                        pcmDataInfo[0].totalBuf = new byte[] {
                            0x07, 0x66, 0xc2
                            , 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
                        };
                }
            }
            else
            {
                if (chipNumber == 0)
                    pcmDataInfo[0].totalBuf = new byte[] {
                        0x67, 0x66, 0xc2
                        , 0x00, 0x00, 0x00, 0x00, 0x00, 0xc0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
                    };
                else
                    pcmDataInfo[0].totalBuf = new byte[] {
                        0x67, 0x66, 0xc2
                        , 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
                    };
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
            parent.OutData((MML)null, port[0], new byte[] { 0x11, 0x00 });//dpcm
            parent.OutData((MML)null, port[0], new byte[] { 0x15, 0x0f });//dpcm以外は有効に

            //sweep が無効であってもshift countの影響を受ける(エミュだけ?)ので
            //7(影響が最も小さい)に設定しできるだけ低音が出せるようにする
            parent.OutData((MML)null, port[0], new byte[] { 0x01, 0x07 });//pls1 sweep(shift count MAX)
            parent.OutData((MML)null, port[0], new byte[] { 0x05, 0x07 });//pls2 sweep(shift count MAX)


            //ヘッダの調整
            if (ChipID != 0)
            {
                parent.dat[vgmHeaderPos + 3] = new outDatum(enmMMLType.unknown, null, null, (byte)(parent.dat[vgmHeaderPos + 3].val | 0x40));//use Secondary
            }
        }



        public override void StorePcm(Dictionary<int, Tuple<string, clsPcm>> newDic, KeyValuePair<int, clsPcm> v, byte[] buf, bool is16bit, int samplerate, params object[] option)
        {
            clsPcmDataInfo pi = pcmDataInfo[0];

            try
            {
                List<byte> nbuf = new List<byte>();
                int now = 0;
                int depth = 0;
                int ndt = 0;
                if (!is16bit)
                {
                    for (int i = 0; i < ((buf.Length + 7) / 8) * 8; i++)
                    {
                        sbyte dt = (sbyte)(i < buf.Length ? (buf[i] - 0x80) : 0);
                        if (now <= dt)
                        {
                            ndt |= (1 << depth);
                            now += 8;
                        }
                        else
                        {
                            now -= 8;
                        }

                        depth++;
                        if (depth == 8)
                        {
                            depth = 0;
                            nbuf.Add((byte)ndt);
                            ndt = 0;
                        }
                    }
                    if (nbuf.Count % 16 != 0)
                    {
                        int n = (16 - (nbuf.Count % 16)) * 8;
                        for (int i = 0; i < n; i++)
                        {
                            //if (now <= 0)
                            //{
                            //    ndt |= (1 << depth);
                            //    now += 8;
                            //}
                            //else
                                now -= 8;

                            depth++;
                            if (depth == 8)
                            {
                                depth = 0;
                                nbuf.Add((byte)ndt);
                                ndt = 0;
                            }

                        }
                    }


                }
                else
                {
                    for (int i = 0; i < (((buf.Length / 2) + 7) / 8) * 8; i++)
                    {
                        short dt = (short)(i * 2 + 1 < buf.Length ? (buf[i * 2] + buf[i * 2 + 1] * 0x100) : 0);
                        if (now <= dt)
                        {
                            ndt |= (1 << depth);
                            now += 16;
                        }
                        else
                        {
                            now -= 16;
                        }

                        depth++;
                        if (depth == 8)
                        {
                            depth = 0;
                            nbuf.Add((byte)ndt);
                            ndt = 0;
                        }
                    }
                }

                buf = nbuf.ToArray();
                long size = buf.Length;

                if (newDic.ContainsKey(v.Key))
                {
                    newDic.Remove(v.Key);
                }

                newDic.Add(
                    v.Key
                    , new Tuple<string, clsPcm>("", new clsPcm(
                        v.Value.num
                        , v.Value.seqNum
                        , v.Value.chip
                        , 0
                        , v.Value.fileName
                        , v.Value.freq != -1 ? v.Value.freq : samplerate
                        , v.Value.vol
                        , pi.totalBufPtr 
                        , pi.totalBufPtr + size - 1
                        , size
                        , -1
                        , is16bit
                        , samplerate
                        )
                    ));

                pi.totalBufPtr += size;

                byte[] newBuf = new byte[pi.totalBuf.Length + buf.Length];
                Array.Copy(pi.totalBuf, newBuf, pi.totalBuf.Length);
                Array.Copy(buf, 0, newBuf, pi.totalBuf.Length, buf.Length);

                pi.totalBuf = newBuf;

                pi.use = true;
                Common.SetUInt32bit31(
                    pi.totalBuf
                    , pi.totalHeadrSizeOfDataPtr
                    , (UInt32)(pi.totalBuf.Length - (pi.totalHeadrSizeOfDataPtr + 4))
                    , ChipNumber != 0);
                pcmDataEasy = pi.use ? pi.totalBuf : null;

            }
            catch
            {
                pi.use = false;
                return;
            }

        }

        public override void StorePcmRawData(clsPcmDatSeq pds, byte[] buf, bool isRaw, bool is16bit, int samplerate, params object[] option)
        {
            msgBox.setWrnMsg(msg.get("E32001"), new LinePos(null, "-"));
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
            else if (page.Type == enmChannelType.Noise|| page.Type == enmChannelType.DPCM)
            {
                f = Common.CheckRange(f, 0, 0xf);
                page.FNum = f;
            }
        }

        public int GetSsgFNum(partPage page, MML mml, int octave, char noteCmd, int shift)
        {
            int o = octave - 1;
            int n = Const.NOTE.IndexOf(noteCmd) + shift;
            o += n / 12;

            if (page.Type == enmChannelType.Pulse || page.Type == enmChannelType.Triangle)
            {
                o = Common.CheckRange(o, 0, 7);
                n %= 12;

                int f = o * 12 + n;
                if (f < 0) f = 0;
                if (f >= FNumTbl[0].Length) f = FNumTbl[0].Length - 1;
                return FNumTbl[0][f];
            }
            else if (page.Type == enmChannelType.Noise || page.Type == enmChannelType.DPCM)
            {
                o = Common.CheckRange(o, 0, 1);
                n %= 12;

                int f = Common.CheckRange(o * 12 + n, 0, 0xf);
                if (page.Type == enmChannelType.Noise)
                    f = 15 - f;

                return f;
            }

            return 0;
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
                                vol = 0x10;//仮
                                SOutData(page, mml, port[0], (byte)(0x00 + page.ch * 4), (byte)vol);
                            }

                            //
                            // Sweepの更新
                            //

                            if (page.keyOn)
                            {
                                for(int i = 0; i < page.lfo.Length; i++)
                                {
                                    if (page.lfo[i].type != eLfoType.Hardware) continue;

                                    if (!page.lfo[i].sw)
                                    {
                                        SOutData(page, mml, port[0], (byte)(0x01 + page.ch * 4), (byte)0x70);
                                        continue;
                                    }

                                    vol = 0x80
                                        | (page.lfo[i].depthWaitCounter << 4)
                                        | (page.lfo[i].depth < 0 ? 0x08 : 0x00)
                                        | (Math.Abs(page.lfo[i].depth));
                                    SOutData(page, mml, port[0], (byte)(0x01 + page.ch * 4), (byte)vol);
                                }
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
                                SOutData(page, mml, port[0], (byte)(2 + page.ch * 4), data);

                                //キーオンした直後
                                //または周波数上位が違う　場合は周波数上位再セット
                                //(上位をセットすると位相とエンベロープがリセットされる)
                                if (page.keyOn || (page.FNum != page.beforeFNum && (page.FNum & 0x700) != (page.beforeFNum & 0x700)))
                                {
                                    data = (byte)((page.HardEnvelopeSw ? (page.HardEnvelopeSpeed<<3) : 0) | ((f >> 8) & 0x7));
                                    SOutData(page, mml, port[0], (byte)(3 + page.ch * 4), data);
                                }
                            }


                            //
                            // ボリュームの更新
                            //

                            if (page.keyOn 
                                || page.latestVolume != page.beforeVolume || page.dutyCycle!=page.spg.dutyCycle)
                            {
                                vol = ((page.dutyCycle & 0x3) << 6) | (page.HardEnvelopeSw ? (page.HardEnvelopeType == 0 ? 0x10 : 0x20) : 0x30) | Common.CheckRange(page.latestVolume, 0, 15);//0x30持続音
                                SOutData(page, mml, port[0], (byte)(0x00 + page.ch * 4), (byte)vol);
                            }
                        }
                        break;
                    case enmChannelType.Triangle:
                        {
                            //キーオフした直後
                            if (page.keyOff)
                            {
                                vol = (page.HardEnvelopeSw ? 0 : 0x80) | (page.HardEnvelopeType & 0x7f);//仮
                                SOutData(page, mml, port[0], 0x08, (byte)vol);
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
                                    data = (byte)((page.HardEnvelopeSw ? (page.HardEnvelopeSpeed << 3) : 0) | ((f >> 8) & 0x7));
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
                        }
                        break;
                    case enmChannelType.Noise:
                        {
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
                                    SOutData(page, mml, port[0], 0x0f, (byte)(page.HardEnvelopeSw ? ((page.HardEnvelopeSpeed & 0x1f) << 3) : 0x08));
                            }


                            //
                            // ボリュームの更新
                            //

                            if (page.keyOn || page.latestVolume != page.beforeVolume)
                            {
                                vol = (page.HardEnvelopeSw ? (page.HardEnvelopeType == 0 ? 0x10 : 0x20) : 0x30) | Common.CheckRange(page.latestVolume, 0, 15);//0x30持続音
                                SOutData(page, mml, port[0], 0x0c, (byte)vol);
                            }
                        }
                        break;
                    case enmChannelType.DPCM:
                        {
                            //キーオフした直後
                            if (page.keyOff)
                            {
                                vol = 0x0f;//(DMC bit4 clear)
                                SOutData(page, mml, port[0], (byte)0x15, (byte)vol);
                            }


                            //
                            // 音色のセット
                            //
                            if (page.instrument != page.beforeInstrument)
                            {
                                clsPcm p = parent.instPCM[page.instrument].Item2;
                                SOutData(page, mml, port[0], (byte)0x12, (byte)(p.stAdr / 64));
                                SOutData(page, mml, port[0], (byte)0x13, (byte)(p.size / 16));
                                page.beforeInstrument = page.instrument;
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
                                byte data = (byte)(f & 0x0f);
                                SOutData(page, mml, port[0], 0x10, data);

                            }


                            //
                            // ボリュームの更新
                            //

                            if (page.keyOn || page.latestVolume != page.beforeVolume)
                            {
                                vol = 64;// Common.CheckRange(page.latestVolume, 0, 127);
                                SOutData(page, mml, port[0], 0x11, (byte)vol);
                            }


                            //キーオンした直後
                            if (page.keyOn)
                            {
                                vol = 0x1f;//(Set DMC bit4)
                                SOutData(page, mml, port[0], (byte)0x15, (byte)vol);
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

            //E指定　ソフトエンベロープ切り替え
            if (type == 'E')
            {
                SetEnvelopParamFromInstrument(page, n, mml);
                return;
            }

            //無指定でdpcm以外の場合は ソフトエンベロープ切り替え
            if (page.Type!= enmChannelType.DPCM)
            {
                SetEnvelopParamFromInstrument(page, n, mml);
                return;
            }


            n = Common.CheckRange(n, 0, 255);

            if (!parent.instPCM.ContainsKey(n))
            {
                msgBox.setErrMsg(string.Format(msg.get("E09003"), n)
                    , mml.line.Lp);
                return;
            }

            if (parent.instPCM[n].Item2.chip != enmChipType.NES)
            {
                msgBox.setErrMsg(string.Format(msg.get("E09004"), n)
                    , mml.line.Lp);
                return;
            }

            page.instrument = n;
            page.pcmStartAddress = (int)parent.instPCM[n].Item2.stAdr;
            page.pcmEndAddress = (int)parent.instPCM[n].Item2.edAdr;
            page.pcmLoopAddress = (int)parent.instPCM[n].Item2.loopAdr;
            page.pcmBank = (int)((parent.instPCM[n].Item2.stAdr >> 16));
            SetDummyData(page, mml);

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
            page.keyOff = true;
        }

        public override int GetToneDoublerShift(partPage page, int octave, char noteCmd, int shift)
        {
            return 0;
        }

        public override void SetToneDoubler(partPage page, MML mml)
        {
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

        public override void CmdHardEnvelope(partPage page, MML mml)
        {
            string cmd = (string)mml.args[0];
            //int n = 0;

            switch (cmd)
            {
                case "EH":
                    page.HardEnvelopeSw = true;
                    page.HardEnvelopeSpeed = ((int)mml.args[1] & 0x1f);
                    break;
                case "EHON":
                    page.HardEnvelopeSw = true;
                    break;
                case "EHOF":
                    page.HardEnvelopeSw = false;
                    break;
                case "EHT":
                    page.HardEnvelopeType = (int)mml.args[1];
                    break;
            }
        }

    }
}
