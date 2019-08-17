using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class RF5C164 : ClsChip
    {
         
        public byte KeyOn = 0x0;
        public byte CurrentChannel = 0xff;

        public RF5C164(ClsVgm parent, int chipID, string initialPartName, string stPath, bool isSecondary) : base(parent, chipID, initialPartName, stPath, isSecondary)
        {
            _chipType = enmChipType.RF5C164;
            _Name = "RF5C164";
            _ShortName = "RF5C";
            _ChMax = 8;
            _canUsePcm = true;
            _canUsePI = true;
            IsSecondary = isSecondary;
            dataType = 0xc1;

            Frequency = 12500000;
            port =new byte[][] { new byte[] { 0xb1 } };

            Ch = new ClsChannel[ChMax];
            SetPartToCh(Ch, initialPartName);
            foreach (ClsChannel ch in Ch)
            {
                ch.Type = enmChannelType.PCM;
                ch.isSecondary = chipID == 1;
            }

            pcmDataInfo = new clsPcmDataInfo[] { new clsPcmDataInfo() };
            pcmDataInfo[0].totalBufPtr = 0L;
            pcmDataInfo[0].use = false;
            if (parent.info.format == enmFormat.ZGM)
            {
                if (parent.ChipCommandSize == 2)
                {
                    if (!isSecondary)
                        pcmDataInfo[0].totalBuf = new byte[] {
                            0xb1, 0x00, 0x07, 0x00//通常コマンド
                            , 0x07, 0x00, 0x66, 0xc1
                            , 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
                        };
                    else
                        pcmDataInfo[0].totalBuf = new byte[] {
                            0xb1,0x00, 0x87, 0x00//通常コマンド
                            , 0x07, 0x00, 0x66, 0xc1
                            , 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
                    };
                }
                else
                {
                    if (!isSecondary)
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
                if (!isSecondary)
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

                newDic.Add(
                    v.Key
                    , new clsPcm(
                        v.Value.num, v.Value.seqNum, v.Value.chip
                        , v.Value.isSecondary
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
                    , pi.totalHeadrSizeOfDataPtr + (parent.ChipCommandSize==2 ? 4 : 3)//通常コマンド分を他のチップと比べて余計に加算する
                    , (UInt32)(pi.totalBuf.Length - (pi.totalHeadrSizeOfDataPtr + 4 + (parent.ChipCommandSize == 2 ? 4 : 3)))//通常コマンド分を他のチップと比べて余計に加算する
                    , IsSecondary
                    );

                //RF5C164のPCMブロックの前に通常コマンドが存在するため書き換える
                if(parent.info.format== enmFormat.ZGM)
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

                SetRf5c164CurrentChannel(null,pw);
                pw.beforepcmStartAddress = -1;
                pw.pcmStartAddress = 0;
                SetRf5c164SampleStartAddress(null,pw);
                SetRf5c164LoopAddress(null,pw, 0);
                SetRf5c164AddressIncrement(null,pw, 0x400);
                SetRf5c164Pan(null,pw, 0xff);
                SetRf5c164Envelope(null,pw, 0xff);
            }

            if (IsSecondary)
            {
                parent.dat[0x6f] = new outDatum(enmMMLType.unknown, null, null, (byte)(parent.dat[0x6f].val | 0x40));
            }

            SupportReversePartWork = true;
        }

        public override void InitPart(partWork pw)
        {
            pw.MaxVolume = 255;
            pw.volume = pw.MaxVolume;
            pw.port = port;
        }


        public int GetRf5c164PcmNote(int octave, char noteCmd, int shift)
        {
            int o = octave;
            int n = Const.NOTE.IndexOf(noteCmd) + shift;
            if (n >= 0)
            {
                o += n / 12;
                o = Common.CheckRange(o, 1, 8);
                n %= 12;
            }
            else
            {
                o += n / 12 - 1;
                o = Common.CheckRange(o, 1, 8);
                n %= 12;
                if (n < 0) { n += 12; }
            }

            return (int)(0x0400 * Const.pcmMTbl[n] * Math.Pow(2, (o - 4)));
        }

        public void SetRf5c164Envelope(MML mml,partWork pw, int volume)
        {
            if (pw.rf5c164Envelope != volume)
            {
                SetRf5c164CurrentChannel(mml,pw);
                byte data = (byte)(volume & 0xff);
                OutRf5c164Port(mml,port[0],pw.isSecondary, 0x0, data);
                pw.rf5c164Envelope = volume;
            }
        }

        public void SetRf5c164Pan(MML mml,partWork pw, int pan)
        {
            if (pw.rf5c164Pan != pan)
            {
                SetRf5c164CurrentChannel(mml,pw);
                byte data = (byte)(pan & 0xff);
                OutRf5c164Port(mml, port[0], pw.isSecondary, 0x1, data);
                pw.rf5c164Pan = pan;
            }
        }

        public void SetRf5c164CurrentChannel(MML mml,partWork pw)
        {
            byte pch = (byte)pw.ch;
            bool isSecondary = pw.isSecondary;
            int chipID = pw.chip.ChipID;

            if (CurrentChannel != pch)
            {
                byte data = (byte)(0xc0 + pch);
                OutRf5c164Port(mml, port[0], isSecondary, 0x7, data);
                CurrentChannel = pch;
            }
        }

        public void SetRf5c164AddressIncrement(MML mml,partWork pw, int f)
        {
            if (pw.rf5c164AddressIncrement != f)
            {
                SetRf5c164CurrentChannel(mml,pw);

                byte data = (byte)(f & 0xff);
                OutRf5c164Port(mml, port[0], pw.isSecondary, 0x2, data);
                data = (byte)((f >> 8) & 0xff);
                OutRf5c164Port(mml, port[0], pw.isSecondary, 0x3, data);
                pw.rf5c164AddressIncrement = f;
            }
        }

        public void SetRf5c164SampleStartAddress(MML mml,partWork pw)
        {

            //Address shift
            int stAdr = pw.pcmStartAddress + pw.addressShift;
            //if (stAdr >= pw.pcmEndAddress) stAdr = pw.pcmEndAddress - 1;

            if (pw.beforepcmStartAddress != stAdr && stAdr>=0)
            {
                SetRf5c164CurrentChannel(mml,pw);
                byte data = (byte)(stAdr >> 8);
                OutRf5c164Port(mml, port[0], pw.isSecondary, 0x6, data);
                //pw.pcmStartAddress = stAdr;
            }
        }

        public void SetRf5c164LoopAddress(MML mml,partWork pw, int adr)
        {
            if (pw.pcmLoopAddress != adr)
            {
                SetRf5c164CurrentChannel(mml,pw);
                byte data = (byte)(adr >> 8);
                OutRf5c164Port(mml, port[0], pw.isSecondary, 0x5, data);
                data = (byte)adr;
                OutRf5c164Port(mml, port[0], pw.isSecondary, 0x4, data);
                pw.pcmLoopAddress = adr;
            }
        }

        public void OutRf5c164KeyOn(MML mml,partWork pw)
        {
            if (parent.instPCM.Count < 1) return;
            SetRf5c164SampleStartAddress(mml,pw);
            KeyOn |= (byte)(1 << pw.ch);
            byte data = (byte)(~KeyOn);
            OutRf5c164Port(mml, port[0], pw.isSecondary, 0x8, data);
            if (parent.instPCM[pw.instrument].status != enmPCMSTATUS.ERROR)
            {
                parent.instPCM[pw.instrument].status = enmPCMSTATUS.USED;
            }
        }

        public void OutRf5c164KeyOff(MML mml,partWork pw)
        {
            KeyOn &= (byte)(~(1 << pw.ch));
            byte data = (byte)(~KeyOn);
            OutRf5c164Port(mml, port[0], pw.isSecondary, 0x8, data);
        }

        public void OutRf5c164Port(MML mml,byte[] cmd,bool isSecondary, byte adr, byte data)
        {
            parent.OutData(
                mml,
                cmd
                , (byte)((adr & 0x7f) | (isSecondary ? 0x80 : 0x00))
                , data
                );
        }


        private byte[] Encode(byte[] buf,bool is16bit)
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
                Array.Copy(buf, newBuf, size-1);
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
                if (buf[i] != 0xff)
                {
                    if (buf[i] >= 0x80)
                    {
                        buf[i] = buf[i];
                    }
                    else
                    {
                        buf[i] = (byte)(0x80 - buf[i]);
                    }
                }

                if (buf[i] == 0xff)
                {
                    buf[i] = 0xfe;
                }
            }
            buf[tSize - 1] = 0xff;

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

            if (pcmDataEasy != null && pcmDataEasy.Length > 0)
                parent.OutData(mml,pcmDataEasy);

            if (pcmDataDirect.Count < 1) return;

            foreach (byte[] dat in pcmDataDirect)
            {
                if (dat != null && dat.Length > 0)
                    parent.OutData(mml,dat);
            }
        }




        public override void SetVolume(partWork pw, MML mml)
        {
            int vol = pw.volume;

            if (pw.envelopeMode)
            {
                vol = 0;
                if (pw.envIndex != -1)
                {
                    vol = pw.envVolume - (255 - pw.volume);
                }
            }

            for (int lfo = 0; lfo < 4; lfo++)
            {
                if (!pw.lfo[lfo].sw)
                {
                    continue;
                }
                if (pw.lfo[lfo].type != eLfoType.Tremolo)
                {
                    continue;
                }
                vol += pw.lfo[lfo].value + pw.lfo[lfo].param[6];
            }

            vol = Common.CheckRange(vol, 0, 255);

            if (pw.beforeVolume != vol)
            {
                SetRf5c164Envelope(mml,pw, vol);
                pw.beforeVolume = vol;
            }
        }

        public override int GetFNum(partWork pw, MML mml, int octave, char cmd, int shift)
        {
            return GetRf5c164PcmNote(octave, cmd, shift);
        }

        public override void SetFNum(partWork pw, MML mml)
        {
            int f = GetFNum(pw,mml, pw.octaveNow, pw.noteCmd, pw.keyShift + pw.shift);//

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

            f = Common.CheckRange(f, 0, 0xffff);
            pw.freq = f;

            //Address increment 再生スピードをセット
            SetRf5c164AddressIncrement(mml,pw, f);

        }

        public override void SetKeyOff(partWork pw, MML mml)
        {
            OutRf5c164KeyOff(mml,pw);
        }

        public override void SetKeyOn(partWork pw, MML mml)
        {
            OutRf5c164KeyOn(mml,pw);

            MML vmml = new MML();
            vmml.type = enmMMLType.Volume;
            vmml.args = new List<object>();
            vmml.args.Add(pw.volume);
            vmml.line = mml.line;
            SetDummyData(pw, vmml);

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
                    SetFNum(pw,mml);
                }
                if (pl.type == eLfoType.Tremolo)
                {
                    pw.beforeVolume = -1;
                    SetVolume(pw,mml);
                }
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

            OutRf5c164Port(mml, port[0], pw.isSecondary, adr, dat);
        }

        public override void CmdPan(partWork pw, MML mml)
        {
            int l = (int)mml.args[0];
            int r = (int)mml.args[1];

            l = Common.CheckRange(l, 0, 15);
            r = Common.CheckRange(r, 0, 15);
            pw.pan.val = (r << 4) | l;
            SetRf5c164Pan(mml,pw, (int)pw.pan.val);
        }

        public override void CmdLoopExtProc(partWork p, MML mml)
        {
            if (p.chip is RF5C164 && parent.rf5c164[p.isSecondary ? 1 : 0].use)
            {
                //rf5c164の設定済み周波数値を初期化(ループ時に直前の周波数を引き継いでしまうケースがあるため)
                p.rf5c164AddressIncrement = -1;
                int n = p.instrument;
                p.pcmStartAddress = -1;
                p.pcmLoopAddress = -1;
                p.freq = -1;
                if (n != -1)
                {
                    SetRf5c164CurrentChannel(mml,p);
                    SetFNum(p,mml);
                    p.beforepcmStartAddress = -1;
                    p.pcmStartAddress = (int)parent.instPCM[n].stAdr;
                    SetRf5c164SampleStartAddress(mml,p);
                    SetRf5c164LoopAddress(
                        mml,
                        p
                        , (int)(parent.instPCM[n].loopAdr));
                }
            }
        }

        public override void CmdInstrument(partWork pw, MML mml)
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
                n = SetEnvelopParamFromInstrument(pw, n,mml);
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

            pw.instrument = n;
            pw.beforepcmStartAddress = -1;
            pw.pcmStartAddress = (int)parent.instPCM[n].stAdr;
            SetRf5c164SampleStartAddress(mml,pw);
            SetRf5c164LoopAddress(mml,pw, (int)(parent.instPCM[n].loopAdr));

        }

        public override void MultiChannelCommand(MML mml)
        {
        }


        public override string DispRegion(clsPcm pcm)
        {
            return string.Format("{0,-10} {1,-7} {2,-5:D3} {3,-4:D2} ${4,-7:X4} N/A      ${5,-7:X4} ${6,-7:X4}  NONE {7}\r\n"
                , Name //0
                , pcm.isSecondary ? "SEC" : "PRI" //1
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
