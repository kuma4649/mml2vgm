using musicDriverInterface;
using System;
using System.Collections.Generic;

namespace Core
{
    public class RF5C164 : ClsChip
    {

        public byte KeyOn = 0x0;
        public byte CurrentChannel = 0xff;

        public RF5C164(ClsVgm parent, int chipID, string initialPartName, string stPath, int chipNumber) : base(parent, chipID, initialPartName, stPath, chipNumber)
        {
            _chipType = enmChipType.RF5C164;
            _Name = "RF5C164";
            _ShortName = "RF5C";
            _ChMax = 8;
            _canUsePcm = true;
            _canUsePI = true;
            ChipNumber = chipNumber;
            dataType = 0xc1;

            Frequency = 12500000;
            port = new byte[][] { new byte[] { 0xb1 } };

            if (string.IsNullOrEmpty(initialPartName)) return;

            Ch = new ClsChannel[ChMax];
            SetPartToCh(Ch, initialPartName);
            foreach (ClsChannel ch in Ch)
            {
                ch.Type = enmChannelType.PCM;
                ch.chipNumber = chipID == 1;
            }

            pcmDataInfo = new clsPcmDataInfo[] { new clsPcmDataInfo() };
            pcmDataInfo[0].totalBufPtr = 0L;
            pcmDataInfo[0].use = false;
            if (parent.info.format == enmFormat.ZGM)
            {
                if (parent.ChipCommandSize == 2)
                {
                    if (chipNumber == 0)
                        pcmDataInfo[0].totalBuf = new byte[] {
                            0xb1, 0x00, 0x07, 0x00,//通常コマンド
                            0x07, 0x00, 0x66, 0xc1
                            , 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
                        };
                    else
                        pcmDataInfo[0].totalBuf = new byte[] {
                            0xb1,0x00, 0x87, 0x00,//通常コマンド
                            0x07, 0x00, 0x66, 0xc1
                            , 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
                    };
                }
                else
                {
                    if (chipNumber == 0)
                        pcmDataInfo[0].totalBuf = new byte[] {
                            0xb1, 0x07, 0x00//通常コマンド
                            , 0x07, 0x66, 0xc1
                            , 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
                        };
                    else
                        pcmDataInfo[0].totalBuf = new byte[] {
                            0xb1, 0x87, 0x00//通常コマンド
                            , 0x07, 0x66, 0xc1
                            , 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
                        };
                }
            }
            else
            {
                if (chipNumber == 0)
                    pcmDataInfo[0].totalBuf = new byte[] {
                        0xb1, 0x07, 0x00//通常コマンド
                        , 0x67, 0x66, 0xc1
                        , 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
                    };
                else
                    pcmDataInfo[0].totalBuf = new byte[] {
                        0xb1, 0x87, 0x00//通常コマンド
                        , 0x67, 0x66, 0xc1
                        , 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
                    };
            }

            pcmDataInfo[0].totalHeaderLength = pcmDataInfo[0].totalBuf.Length;
            pcmDataInfo[0].totalHeadrSizeOfDataPtr = (parent.ChipCommandSize == 2) ? 4 : 3;

            Envelope = new Function();
            Envelope.Max = 255;
            Envelope.Min = 0;
        }

        public override void StorePcm(Dictionary<int, clsPcm> newDic, KeyValuePair<int, clsPcm> v, byte[] buf, bool is16bit, int samplerate, params object[] option)
        {
            clsPcmDataInfo pi = pcmDataInfo[0];

            try
            {
                long size = buf.Length;
                byte[] newBuf = new byte[size + 1];
                //Array.Copy(buf, newBuf, size);
                //newBuf[size] = 0xff;
                //buf = newBuf;
                long tSize = size;
                //size = buf.Length;

                buf = Encode(buf, false);
                size = buf.Length;
                //long tSize = size - 1;

                newDic.Add(
                    v.Key
                    , new clsPcm(
                        v.Value.num, v.Value.seqNum, v.Value.chip
                        , v.Value.chipNumber
                        , v.Value.fileName
                        , v.Value.freq
                        , v.Value.vol
                        , pi.totalBufPtr
                        , pi.totalBufPtr + size - 1
                        , size
                        , pi.totalBufPtr + (v.Value.loopAdr == -1 ? tSize : v.Value.loopAdr)
                        , is16bit
                        , samplerate
                        )
                    );

                pi.totalBufPtr += size;

                newBuf = new byte[pi.totalBuf.Length + buf.Length];
                Array.Copy(pi.totalBuf, newBuf, pi.totalBuf.Length);
                Array.Copy(buf, 0, newBuf, pi.totalBuf.Length, buf.Length);

                pi.totalBuf = newBuf;
                Common.SetUInt32bit31(
                    pi.totalBuf
                    , pi.totalHeadrSizeOfDataPtr + (parent.ChipCommandSize == 2 ? 4 : 3)//通常コマンド分を他のチップと比べて余計に加算する
                    , (UInt32)(pi.totalBuf.Length - (pi.totalHeadrSizeOfDataPtr + 4 + (parent.ChipCommandSize == 2 ? 4 : 3)))//通常コマンド分を他のチップと比べて余計に加算する
                    , ChipNumber != 0
                    );

                //RF5C164のPCMブロックの前に通常コマンドが存在するため書き換える
                if (parent.info.format == enmFormat.ZGM)
                {
                    pi.totalBuf[0] = port[0][0];
                    if (parent.ChipCommandSize == 2)
                    {
                        pi.totalBuf[1] = port[0][1];
                    }
                }

                pi.use = true;
                pcmDataEasy = pi.use ? pi.totalBuf : null;
            }
            catch
            {
                pi.use = false;
            }

        }

        public override void InitChip()
        {
            if (!use) return;

            for (int ch = 0; ch < ChMax; ch++)
            {
                partWork pw = lstPartWork[ch];
                foreach (partPage pg in pw.pg)
                {
                    SetRf5c164CurrentChannel(null, pg);
                    pg.beforepcmStartAddress = -1;
                    pg.pcmStartAddress = 0;
                    SetRf5c164SampleStartAddress(null, pg);
                    SetRf5c164LoopAddress(null, pg, 0);
                    SetRf5c164AddressIncrement(null, pg, 0x400);
                    SetRf5c164Pan(null, pg, 0xff);
                    SetRf5c164Envelope(null, pg, 0xff);
                }
            }

            if (ChipID != 0 && parent.info.format != enmFormat.ZGM)
            {
                parent.dat[0x6f] = new outDatum(enmMMLType.unknown, null, null, (byte)(parent.dat[0x6f].val | 0x40));
            }

            SupportReversePartWork = true;
        }

        public override void InitPart(partWork pw)
        {
            foreach (partPage pg in pw.pg)
            {
                pg.MaxVolume = 255;
                pg.volume = pg.MaxVolume;
                pg.port = port;
            }
        }


        private int GetRf5c164PcmNote(partPage page, int octave, char noteCmd, int shift)
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

            if (page.instrument < 0 || !parent.instPCM.ContainsKey(page.instrument))
            {
                return 0;
            }

            if (parent.instPCM[page.instrument].freq == -1)
            {
                return ((int)(
                    0x0400
                    * Const.pcmMTbl[n]
                    * Math.Pow(2, (o - 4))
                    * ((double)parent.instPCM[page.instrument].samplerate / 8000.0)
                    ));
            }
            else
            {
                return ((int)(
                    0x0400
                    * Const.pcmMTbl[n]
                    * Math.Pow(2, (o - 4))
                    * ((double)parent.instPCM[page.instrument].freq / 8000.0)
                    ));
            }

        }

        public void SetRf5c164Envelope(MML mml, partPage page, int volume)
        {
            if (page.rf5c164Envelope != volume)
            {
                SetRf5c164CurrentChannel(mml, page);
                byte data = (byte)(volume & 0xff);
                OutRf5c164Port(page, mml, port[0], page.chipNumber, 0x0, data);
                page.rf5c164Envelope = volume;
            }
        }

        public void SetRf5c164Pan(MML mml, partPage page, int pan)
        {
            if (page.rf5c164Pan != pan)
            {
                SetRf5c164CurrentChannel(mml, page);
                byte data = (byte)(pan & 0xff);
                OutRf5c164Port(page, mml, port[0], page.chipNumber, 0x1, data);
                page.rf5c164Pan = pan;
            }
        }

        public void SetRf5c164CurrentChannel(MML mml, partPage page)
        {
            byte pch = (byte)page.ch;
            int chipNumber = page.chipNumber;
            int chipID = page.chip.ChipID;

            if (CurrentChannel != pch)
            {
                byte data = (byte)(0xc0 + pch);
                OutRf5c164Port(page, mml, port[0], chipNumber, 0x7, data);
                CurrentChannel = pch;
            }
        }

        public void SetRf5c164AddressIncrement(MML mml, partPage page, int f)
        {
            if (page.rf5c164AddressIncrement != f)
            {
                SetRf5c164CurrentChannel(mml, page);

                byte data = (byte)(f & 0xff);
                OutRf5c164Port(page, mml, port[0], page.chipNumber, 0x2, data);
                data = (byte)((f >> 8) & 0xff);
                OutRf5c164Port(page, mml, port[0], page.chipNumber, 0x3, data);
                page.rf5c164AddressIncrement = f;
            }
        }

        public void SetRf5c164SampleStartAddress(MML mml, partPage page)
        {

            //Address shift
            int stAdr = page.pcmStartAddress + page.addressShift;
            //if (stAdr >= pw.ppg[pw.cpgNum].pcmEndAddress) stAdr = pw.ppg[pw.cpgNum].pcmEndAddress - 1;

            if (page.beforepcmStartAddress != stAdr && stAdr >= 0)
            {
                SetRf5c164CurrentChannel(mml, page);
                byte data = (byte)(stAdr >> 8);
                OutRf5c164Port(page, mml, port[0], page.chipNumber, 0x6, data);
                //pw.ppg[pw.cpgNum].pcmStartAddress = stAdr;
            }
        }

        public void SetRf5c164LoopAddress(MML mml, partPage page, int adr)
        {
            if (page.pcmLoopAddress != adr)
            {
                SetRf5c164CurrentChannel(mml, page);
                byte data = (byte)(adr >> 8);
                OutRf5c164Port(page, mml, port[0], page.chipNumber, 0x5, data);
                data = (byte)adr;
                OutRf5c164Port(page, mml, port[0], page.chipNumber, 0x4, data);
                page.pcmLoopAddress = adr;
            }
        }

        public void OutRf5c164KeyOn(MML mml, partPage page)
        {
            if (parent.instPCM.Count < 1) return;
            //SetRf5c164CurrentChannel(mml, page);
            SetRf5c164SampleStartAddress(mml, page);
            KeyOn |= (byte)(1 << page.ch);
            byte data = (byte)(~KeyOn);
            OutRf5c164Port(page, mml, port[0], page.chipNumber, 0x8, data);
            if (!parent.instPCM.ContainsKey(page.instrument))
            {
                if (page.instrument == -1)
                    msgBox.setErrMsg(msg.get("E10030"), mml.line.Lp);
                else
                    msgBox.setErrMsg(string.Format(msg.get("E10021"), page.instrument), mml.line.Lp);
                return;
            }
            if (parent.instPCM[page.instrument].status != enmPCMSTATUS.ERROR)
            {
                parent.instPCM[page.instrument].status = enmPCMSTATUS.USED;
            }
        }

        public void OutRf5c164KeyOff(MML mml, partPage page)
        {
            KeyOn &= (byte)(~(1 << page.ch));
            byte data = (byte)(~KeyOn);
            OutRf5c164Port(page, mml, port[0], page.chipNumber, 0x8, data);
        }

        public void OutRf5c164Port(partPage page, MML mml, byte[] cmd, int chipNumber, byte adr, byte data)
        {
            SOutData(
                page,
                mml,
                cmd
                , (byte)((adr & 0x7f) | (chipNumber != 0 ? 0x80 : 0x00))
                , data
                );
        }


        private byte[] Encode(byte[] buf, bool is16bit)
        {
            long size = buf.Length;
            long tSize = buf.Length;
            byte[] newBuf;
            //Padding
            if (size % 0x100 != 0)
            {
                size++;
                tSize = size;
                newBuf = new byte[size];
                Array.Copy(buf, newBuf, size - 1);
                buf = newBuf;
                newBuf = Common.PcmPadding(ref buf, ref size, 0x00, 0x100);
            }
            else
            {
                newBuf = new byte[size];
                Array.Copy(buf, newBuf, size);
            }
            buf = newBuf;

            for (int i = 0; i < tSize; i++)
            {
                //調整
                if (buf[i] >= 0x80)
                {
                    buf[i] = buf[i];
                }
                else
                {
                    buf[i] = (byte)(0x80 - buf[i]);
                }

                if (buf[i] == 0xff)//調整の結果0xffの場合は0xfeにしてループポイント回避
                {
                    buf[i] = 0xfe;
                }
            }
            buf[tSize - 1] = 0xff;
            //buf[buf.Length-1] = 0xff;

            return buf;
        }

        public override void StorePcmRawData(clsPcmDatSeq pds, byte[] buf, bool isRaw, bool is16bit, int samplerate, params object[] option)
        {
            if (!isRaw)
            {
                //Rawファイルは何もしない
                //Wavファイルはエンコ
                buf = Encode(buf, false);
            }

            pcmDataDirect.Add(Common.MakePCMDataBlockType2(dataType, pds, buf));

        }

        public override void SetPCMDataBlock(MML mml)
        {
            if (!CanUsePcm) return;
            if (!use) return;

            if (parent.info.format == enmFormat.ZGM)
            {
                if (port.Length < 1) return;

                if (parent.ChipCommandSize != 2)
                {
                    if (port[0].Length < 1) return;

                    if (pcmDataEasy != null && pcmDataEasy.Length > 1)
                    {
                        pcmDataEasy[0] = port[0][0];
                        pcmDataEasy[4] = port[0][0];
                    }
                    for (int i = 0; i < pcmDataDirect.Count; i++)
                    {
                        pcmDataDirect[i][0] = port[0][0];
                        pcmDataDirect[i][4] = port[0][0];
                    }
                }
                else
                {
                    if (port[0].Length < 2) return;

                    if (pcmDataEasy != null && pcmDataEasy.Length > 3)
                    {
                        pcmDataEasy[0] = port[0][0];
                        pcmDataEasy[1] = port[0][1];
                        pcmDataEasy[5] = port[0][0];
                        pcmDataEasy[6] = port[0][1];
                    }
                    for (int i = 0; i < pcmDataDirect.Count; i++)
                    {
                        pcmDataDirect[i][0] = port[0][0];
                        pcmDataDirect[i][1] = port[0][1];
                        pcmDataDirect[i][5] = port[0][0];
                        pcmDataDirect[i][6] = port[0][1];
                    }
                }
            }

            if (pcmDataEasy != null && pcmDataEasy.Length > 0)
                parent.OutData(mml, pcmDataEasy);

            if (pcmDataDirect.Count < 1) return;

            foreach (byte[] dat in pcmDataDirect)
            {
                if (dat != null && dat.Length > 0)
                    parent.OutData(mml, dat);
            }
        }




        public override void SetVolume(partPage page, MML mml)
        {
            int vol = page.volume;

            if (page.envelopeMode)
            {
                vol = 0;
                if (page.envIndex != -1)
                {
                    vol = page.envVolume - (page.MaxVolume - page.volume);
                }
            }

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

            vol = Common.CheckRange(vol, 0, 255);

            if (page.beforeVolume != vol)
            {
                SetRf5c164Envelope(mml, page, vol);
                page.beforeVolume = vol;
            }
        }

        public override int GetFNum(partPage page, MML mml, int octave, char cmd, int shift)
        {
            return GetRf5c164PcmNote(page, octave, cmd, shift);
        }

        public override void SetFNum(partPage page, MML mml)
        {
            int f = GetFNum(page, mml, page.octaveNow, page.noteCmd, page.keyShift + page.shift + page.arpDelta);//

            if (page.bendWaitCounter != -1)
            {
                f = page.bendFnum;
            }
            f = f + page.detune;
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

            f = Common.CheckRange(f, 0, 0xffff);
            page.freq = f;

            //Address increment 再生スピードをセット
            SetRf5c164AddressIncrement(mml, page, f);

        }

        public override void SetKeyOff(partPage page, MML mml)
        {
            OutRf5c164KeyOff(mml, page);
        }

        public override void SetKeyOn(partPage page, MML mml)
        {
            OutRf5c164KeyOn(mml, page);
            if (mml == null) return;

            MML vmml = new MML();
            vmml.type = enmMMLType.Volume;
            vmml.args = new List<object>();
            vmml.args.Add(page.volume);
            vmml.line = mml.line;
            SetDummyData(page, vmml);

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

                if (pl.type == eLfoType.Vibrato)
                {
                    SetFNum(page, mml);
                }
                if (pl.type == eLfoType.Tremolo)
                {
                    page.beforeVolume = -1;
                    SetVolume(page, mml);
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


        public override void CmdY(partPage page, MML mml)
        {
            if (mml.args[0] is string) return;

            byte adr = (byte)(int)mml.args[0];
            byte dat = (byte)(int)mml.args[1];

            OutRf5c164Port(page, mml, port[0], page.chipNumber, adr, dat);
        }

        public override void CmdPan(partPage page, MML mml)
        {
            int l = (int)mml.args[0];
            int r = (int)mml.args[1];

            l = Common.CheckRange(l, 0, 15);
            r = Common.CheckRange(r, 0, 15);
            page.pan = (r << 4) | l;
            SetRf5c164Pan(mml, page, page.pan);
        }

        public override void CmdLoopExtProc(partPage page, MML mml)
        {
            if (page.chip is RF5C164 && parent.rf5c164[page.chipNumber].use)
            {
                //rf5c164の設定済み周波数値を初期化(ループ時に直前の周波数を引き継いでしまうケースがあるため)
                page.rf5c164AddressIncrement = -1;
                int n = page.instrument;
                page.pcmStartAddress = -1;
                page.pcmLoopAddress = -1;
                page.freq = -1;
                if (n != -1)
                {
                    SetRf5c164CurrentChannel(mml, page);
                    SetFNum(page, mml);
                    page.beforepcmStartAddress = -1;
                    page.pcmStartAddress = (int)parent.instPCM[n].stAdr;
                    SetRf5c164SampleStartAddress(mml, page);
                    SetRf5c164LoopAddress(
                        mml,
                        page
                        , (int)(parent.instPCM[n].loopAdr));
                }
            }
        }

        public override void CmdInstrument(partPage page, MML mml)
        {
            char type = (char)mml.args[0];
            int n = (int)mml.args[1];

            if (type == 'I')
            {
                msgBox.setErrMsg(msg.get("E13001"), mml.line.Lp);
                return;
            }

            if (type == 'T')
            {
                msgBox.setErrMsg(msg.get("E13002"), mml.line.Lp);
                return;
            }

            if (type == 'E')
            {
                n = SetEnvelopParamFromInstrument(page, n, mml);
                return;
            }

            n = Common.CheckRange(n, 0, 255);

            if (!parent.instPCM.ContainsKey(n))
            {
                msgBox.setErrMsg(string.Format(msg.get("E13003"), n), mml.line.Lp);
                return;
            }

            if (parent.instPCM[n].chip != enmChipType.RF5C164)
            {
                msgBox.setErrMsg(string.Format(msg.get("E13004"), n), mml.line.Lp);
                return;
            }

            page.instrument = n;
            page.beforepcmStartAddress = -1;
            page.pcmStartAddress = (int)parent.instPCM[n].stAdr;
            SetRf5c164SampleStartAddress(mml, page);
            SetRf5c164LoopAddress(mml, page, (int)(parent.instPCM[n].loopAdr + 2));

        }

        public override void CmdDetune(partPage page, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, -0xffff, 0xffff);
            page.detune = n;
            SetDummyData(page, mml);
        }



        public override void MultiChannelCommand(MML mml)
        {
        }


        public override string DispRegion(clsPcm pcm)
        {
            return string.Format("{0,-10} {1,-7} {2,-5:D3} {3,-4:D2} ${4,-7:X4} N/A      ${5,-7:X4} ${6,-7:X4}  NONE {7}\r\n"
                , Name //0
                , pcm.chipNumber != 0 ? "SEC" : "PRI" //1
                , pcm.num //2
                , pcm.stAdr >> 16 //3
                , pcm.stAdr & 0xffff //4
                , pcm.loopAdr & 0xffff //5
                , pcm.size //6
                , pcm.status.ToString() //7
                );
        }
    }
}