using musicDriverInterface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
    //
    // QSound memo
    //
    // Pannning reso. 33 (0-32) 32段階ではないので注意
    // Volume 16bit!!(0-65535) 65535までうけつけるけど実際のところは不明
    // PCM 普通のLinerPCM 符号付き8bit
    // PCM Bank 0x7f(0-127)
    // PCM Address 16bit ひとつのPCM当たり最大65536byteまで再生可能(Bank跨ぎはできない)
    // loop(16bit)指定可能(0でループ無し)



    public class QSound : ClsChip
    {
        public List<long> memoryMap = null;

        public QSound(ClsVgm parent, int chipID, string initialPartName, string stPath, int chipNumber) : base(parent, chipID, initialPartName, stPath, chipNumber)
        {
            _chipType = enmChipType.QSound;
            _Name = "QSound";
            _ShortName = "QSnd";
            _ChMax = 16;
            _canUsePcm = true;
            _canUsePI = false;
            ChipNumber = chipNumber;//QSound はDualChip非対応

            Frequency = 4_000_000;//4MHz
            port = new byte[][] { new byte[] { 0xc4 } };

            if (string.IsNullOrEmpty(initialPartName)) return;

            Ch = new ClsChannel[ChMax];
            SetPartToCh(Ch, initialPartName);
            foreach (ClsChannel ch in Ch)
            {
                ch.Type = enmChannelType.PCM;
                ch.chipNumber = chipID == 1;
                ch.MaxVolume = 65535;
            }

            pcmDataInfo = new clsPcmDataInfo[] { new clsPcmDataInfo() };
            pcmDataInfo[0].totalBufPtr = 0L;
            pcmDataInfo[0].use = false;
            if (parent.info.format == enmFormat.ZGM)
            {
                if (parent.ChipCommandSize == 2)
                {
                    pcmDataInfo[0].totalBuf = new byte[] {
                        0x07,0x00, 0x66, 0x8f
                        , 0x00, 0x00, 0x00, 0x00 //size of data
                        , 0x00, 0x00, 0x00, 0x00 //size of the entire ROM
                        , 0x00, 0x00, 0x00, 0x00 //start address of data
                    };
                }
                else
                {
                    pcmDataInfo[0].totalBuf = new byte[] {
                        0x07, 0x66, 0x8f
                        , 0x00, 0x00, 0x00, 0x00 //size of data
                        , 0x00, 0x00, 0x00, 0x00 //size of the entire ROM
                        , 0x00, 0x00, 0x00, 0x00 //start address of data
                    };
                }
            }
            else
            {
                pcmDataInfo[0].totalBuf = new byte[] {
                    0x67, 0x66, 0x8f
                    , 0x00, 0x00, 0x00, 0x00 //size of data
                    , 0x00, 0x00, 0x00, 0x00 //size of the entire ROM
                    , 0x00, 0x00, 0x00, 0x00 //start address of data
                };
            }

            pcmDataInfo[0].totalHeaderLength = pcmDataInfo[0].totalBuf.Length;
            pcmDataInfo[0].totalHeadrSizeOfDataPtr = (parent.ChipCommandSize == 2) ? 4 : 3;

            memoryMap = new List<long>();
        }

        public override void InitPart(partWork pw)
        {
            foreach (partPage pg in pw.pg)
            {
                pg.MaxVolume = 65535;
                pg.volume = 3000;// pw.ppg[pw.cpgNum].MaxVolume;
                pg.panL = 16;
                pg.port = port;
                pg.beforepcmLoopAddress = -1;
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
                    pg.MaxVolume = Ch[ch].MaxVolume;
                    pg.volume = 3000;// pw.ppg[pw.cpgNum].MaxVolume;
                    pg.panL = 16;
                }
            }

        }

        public override void StorePcm(Dictionary<int,Tuple<string, clsPcm>> newDic, KeyValuePair<int, clsPcm> v, byte[] buf, bool is16bit, int samplerate, params object[] option)
        {
            clsPcmDataInfo pi = pcmDataInfo[0];

            try
            {
                List<byte> dBuf = new List<byte>();
                if (is16bit)
                {
                    throw new NotImplementedException();
                }
                else
                {
                    //8bit unsigned -> 8bit signed
                    for (int i = 0; i < buf.Length; i++)
                    {
                        dBuf.Add((byte)(buf[i] - 0x80));//符号化
                    }
                }
                buf = dBuf.ToArray();

                long size = buf.Length;
                byte[] newBuf = new byte[size];
                Array.Copy(buf, newBuf, size);
                buf = newBuf;

                //65536/2 バイトを超える場合はそれ以降をカット
                int bankSize = 0x10000 / 2;

                if (size > bankSize)
                {
                    List<byte> n = newBuf.ToList();
                    n.RemoveRange(bankSize, (int)(size - bankSize));
                    newBuf = n.ToArray();
                    size = bankSize;
                }

                //空いているBankを探す
                int freeBank = 0;
                int freeAdr = 0;
                do
                {
                    if (memoryMap.Count < freeBank + 1)
                    {
                        memoryMap.Add(0);
                        freeAdr = 0 + freeBank * bankSize;
                        memoryMap[freeBank] += size;
                        break;
                    }

                    if (size < bankSize - memoryMap[freeBank])
                    {
                        freeAdr = (int)(memoryMap[freeBank] + freeBank * bankSize);
                        memoryMap[freeBank] += size;
                        break;
                    }

                    freeBank++;

                } while (true);

                //パディング(空きが足りない場合はバンクをひとつ進める(0x10000)為、空きを全て埋める)
                while (freeAdr > pi.totalBuf.Length - pi.totalHeaderLength)
                {
                    int fs = (pi.totalBuf.Length - pi.totalHeaderLength) % bankSize;
                    List<byte> n = pi.totalBuf.ToList();
                    for (int i = 0; i < bankSize - fs; i++) n.Add(0x00);
                    pi.totalBuf = n.ToArray();
                    pi.totalBufPtr += bankSize - fs;
                }

                newDic.Add(
                    v.Key
                    ,new Tuple<string, clsPcm>("", new clsPcm(
                        v.Value.num
                        , v.Value.seqNum, v.Value.chip
                        , v.Value.chipNumber
                        , v.Value.fileName
                        , v.Value.freq
                        , v.Value.vol
                        , freeAdr
                        , freeAdr + size - 1
                        , size
                        , (v.Value.loopAdr == -1 ? freeAdr : (v.Value.loopAdr + freeAdr))
                        , is16bit
                        , samplerate)
                    ));

                if (newDic[v.Key].Item2.loopAdr != -1 
                    && (newDic[v.Key].Item2.loopAdr < freeAdr || v.Value.loopAdr >= size))
                {
                    msgBox.setErrMsg(string.Format(msg.get("E22000")
                        , newDic[v.Key].Item2.loopAdr
                        , size - 1), new LinePos("-"));
                    newDic[v.Key].Item2.loopAdr = -1;
                    newDic[v.Key].Item2.status = enmPCMSTATUS.ERROR;
                }

                if (freeAdr == pi.totalBufPtr)
                {
                    pi.totalBufPtr += size;

                    newBuf = new byte[pi.totalBuf.Length + buf.Length];
                    Array.Copy(pi.totalBuf, newBuf, pi.totalBuf.Length);
                    Array.Copy(buf, 0, newBuf, pi.totalBuf.Length, buf.Length);
                    pi.totalBuf = newBuf;
                }
                else
                {
                    Array.Copy(buf, 0, pi.totalBuf, freeAdr + pi.totalHeaderLength, buf.Length);
                }

                Common.SetUInt32bit31(
                    pi.totalBuf
                    , pi.totalHeadrSizeOfDataPtr
                    , (UInt32)(pi.totalBuf.Length - (pi.totalHeadrSizeOfDataPtr + 4))
                    , false
                    );
                Common.SetUInt32bit31(
                    pi.totalBuf
                    , pi.totalHeadrSizeOfDataPtr + 4
                    , (UInt32)(pi.totalBuf.Length - (pi.totalHeadrSizeOfDataPtr + 4 + 4 + 4))
                    );
                pi.use = true;
                pcmDataEasy = pi.use ? pi.totalBuf : null;
            }
            catch
            {
                pi.use = false;
                newDic[v.Key].Item2.status = enmPCMSTATUS.ERROR;
            }
        }

        public override void StorePcmRawData(clsPcmDatSeq pds, byte[] buf, bool isRaw, bool is16bit, int samplerate, params object[] option)
        {
            throw new NotImplementedException();
        }

        public override string DispRegion(Tuple<string, clsPcm> pcm)
        {
            return string.Format("{0,-10} {1,-7} {2,-5:D3} {3,-4:D2} ${4,-7:X4} ${5,-7:X4} {6} ${7,-7:X4}  {8,4} {9}\r\n"
                , Name //0
                , pcm.Item2.chipNumber != 0 ? "SEC" : "PRI" //1
                , pcm.Item2.num //2
                , pcm.Item2.stAdr >> 16 //3
                , pcm.Item2.stAdr & 0xffff //4
                , pcm.Item2.edAdr & 0xffff //5
                , pcm.Item2.loopAdr == -1 ? "N/A     " : string.Format("${0,-7:X4}", (pcm.Item2.loopAdr & 0xffff)) //6
                , pcm.Item2.size //7
                , "NONE" //mode //8
                , pcm.Item2.status.ToString() //9
                );
        }


        private void OutQSoundPort(MML mml, byte[] cmd, partPage page, byte adr, ushort data)
        {
            SOutData(
                page,
                mml,
                cmd
                , (byte)(data >> 8)
                , (byte)data
                , adr
                );
        }

        private void OutQSoundKeyOff(MML mml, partPage page)
        {
            int bch = (page.ch - 1) & 0xf;
            byte adr = (byte)((bch << 3) + 0x00);
            ushort data = (ushort)(page.pcmBank);
            OutQSoundPort(mml, port[0], page
                , adr
                , (ushort)(data & 0x7fff)
                );
            page.beforepcmBank = -1;

            adr = (byte)((page.ch << 3) + 0x03);
            data = 0;

            OutQSoundPort(mml, port[0], page
                , adr
                , (ushort)data
                );
        }

        private void OutQSoundKeyOn(partPage page, MML mml)
        {
            byte adr = 0;
            ushort data = 0;

            if (page.instrument == -1)
            {
                LinePos lp = mml?.line?.Lp;
                if (lp == null) lp = new LinePos("-");
                msgBox.setErrMsg(msg.get("E22005"), lp);
                return;
            }

            //Volume
            SetVolume(page, mml);

            //Address shift
            int stAdr = page.pcmStartAddress + page.addressShift;
            if (stAdr >= page.pcmEndAddress) stAdr = page.pcmEndAddress - 1;

            //if (pw.ppg[pw.cpgNum].beforepcmStartAddress != stAdr)
            {
                //StartAdr
                adr = (byte)((page.ch << 3) + 0x01);
                data = (ushort)stAdr;
                OutQSoundPort(mml, port[0], page
                    , adr
                    , (ushort)data
                    );

                page.beforepcmStartAddress = stAdr;
            }

            if (page.beforepcmEndAddress != page.pcmEndAddress)
            {
                //EndAdr
                adr = (byte)((page.ch << 3) + 0x05);
                data = (ushort)page.pcmEndAddress;
                OutQSoundPort(mml, port[0], page
                    , adr
                    , (ushort)data
                    );

                page.beforepcmEndAddress = page.pcmEndAddress;
            }

            if (page.beforepcmLoopAddress != (page.pcmEndAddress - page.pcmLoopAddress) && page.beforepcmLoopAddress != 4)
            {
                //LoopAdr
                adr = (byte)((page.ch << 3) + 0x04);
                data = (ushort)(page.pcmLoopAddress == -1
                    ? 4 //QSoundはループがデフォ。(thanks Ian(@SuperCTR)さん)
                    : (page.pcmEndAddress - page.pcmLoopAddress)
                    );
                OutQSoundPort(mml, port[0], page
                    , adr
                    , (ushort)data
                    );

                page.beforepcmLoopAddress = data;
            }

            if (page.beforepcmBank != page.pcmBank)
            {
                int bch = (page.ch - 1) & 0xf;
                adr = (byte)((bch << 3) + 0x00);
                data = (ushort)(page.pcmBank);
                OutQSoundPort(mml, port[0], page
                    , adr
                    , (ushort)(data | 0x8000)
                    );

                page.beforepcmBank = page.pcmBank;
            }

            adr = (byte)((page.ch << 3) + 0x03);
            data = (ushort)0x8000;

            if (parent.instPCM[page.instrument].Item2.status != enmPCMSTATUS.ERROR)
            {
                parent.instPCM[page.instrument].Item2.status = enmPCMSTATUS.USED;
            }

            OutQSoundPort(mml, port[0], page
                , adr
                , (ushort)data
                );
        }

        public override void SetFNum(partPage page, MML mml)
        {
            int arpNote = page.arpFreqMode ? 0 : page.arpDelta;
            int arpFreq = page.arpFreqMode ? page.arpDelta : 0;

            int f = GetQSoundFNum(mml, page, page.octaveNow, page.noteCmd, page.shift + page.keyShift + arpNote);//
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

            f = Common.CheckRange(f, 0, 0xffff);
            if (page.freq == f) return;

            page.freq = f;

            //Delta
            int data = f & 0xffff;
            if (page.beforeFNum != data)
            {
                byte adr = (byte)((page.ch << 3) + 0x02);
                OutQSoundPort(mml, port[0], page
                    , adr
                    , (ushort)data
                    );
                page.beforeFNum = data;
            }

        }

        public override int GetFNum(partPage page, MML mml, int octave, char cmd, int shift)
        {
            return GetQSoundFNum(mml, page, octave, cmd, shift);
        }

        public int GetQSoundFNum(MML mml, partPage page, int octave, char noteCmd, int shift)
        {
            try
            {
                int o = octave - 1;
                int n = Const.NOTE.IndexOf(noteCmd) + shift;

                o += n / 12;
                n %= 12;
                if (n < 0)
                {
                    n += 12;
                    o = Common.CheckRange(--o, 0, 7);
                }

                if (page.instrument < 0 || !parent.instPCM.ContainsKey(page.instrument))
                {
                    return 0;
                }

                double freq = (double)parent.instPCM[page.instrument].Item2.samplerate;
                if (parent.instPCM[page.instrument].Item2.freq != -1)
                {
                    freq = (double)parent.instPCM[page.instrument].Item2.freq;
                }

                return (int)(
                    4000000.0
                    * Const.pcmMTbl[n]
                    * Math.Pow(2, o)
                    * (freq / 8000.0)
                    / Frequency
                    * 166.0 //div
                    );

            }
            catch
            {
                return 0;
            }
        }

        public override void SetKeyOn(partPage page, MML mml)
        {
            OutQSoundKeyOn(page, mml);
        }

        public override void SetKeyOff(partPage page, MML mml)
        {
            OutQSoundKeyOff(mml, page);
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
            //Console.WriteLine("{0} ", pw.ppg[pw.cpgNum].envVolume);

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

            if (page.varpeggioMode)
            {
                vol += page.varpDelta;
            }

            int data = Common.CheckRange(vol, 0, 65535);
            if (page.beforeVolume != data)
            {
                byte adr = (byte)((page.ch << 3) + 0x06);
                OutQSoundPort(mml, port[0], page
                    , adr
                    , (ushort)data
                    );
                page.beforeVolume = data;
            }

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

        public override void CmdPan(partPage page, MML mml)
        {
            int p = (int)mml.args[0];
            p = Common.CheckRange(p, 0, 32);
            page.panL = (32 - p);

            byte adr = (byte)(page.ch + 0x80);
            OutQSoundPort(mml, port[0], page
                , adr
                , (ushort)(page.panL + 0x110)
                );

            SetDummyData(page, mml);
        }

        public override void CmdInstrument(partPage page, MML mml)
        {
            char type = (char)mml.args[0];
            int n = (int)mml.args[1];

            if (type == 'I')
            {
                msgBox.setErrMsg(msg.get("E22001")
                    , mml.line.Lp);
                return;
            }

            if (type == 'T')
            {
                msgBox.setErrMsg(msg.get("E22002")
                    , mml.line.Lp);
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
                msgBox.setErrMsg(string.Format(msg.get("E22003"), n)
                    , mml.line.Lp);
                return;
            }

            if (parent.instPCM[n].Item2.chip != enmChipType.QSound)
            {
                msgBox.setErrMsg(string.Format(msg.get("E22004"), n)
                    , mml.line.Lp);
                return;
            }

            page.instrument = n;
            page.pcmStartAddress = (int)parent.instPCM[n].Item2.stAdr;
            page.pcmEndAddress = (int)parent.instPCM[n].Item2.edAdr;
            page.pcmLoopAddress = (int)parent.instPCM[n].Item2.loopAdr;
            page.beforepcmLoopAddress = -1;
            page.pcmBank = (int)((parent.instPCM[n].Item2.stAdr >> 16));
            SetDummyData(page, mml);

        }

        public override void CmdY(partPage page, MML mml)
        {
            if (mml.args[0] is string) return;

            byte adr = (byte)(int)mml.args[0];
            ushort dat = (ushort)(int)mml.args[1];

            OutQSoundPort(mml, port[0], page, adr, dat);
        }

        public override void CmdLoopExtProc(partPage page, MML mml)
        {
        }

        public override void CmdDetune(partPage page, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, -0xffff, 0xffff);
            page.detune = n;
            SetDummyData(page, mml);
        }
    }
}
