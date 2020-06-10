using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using musicDriverInterface;

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
            pw.pg[pw.cpg].MaxVolume = 65535;
            pw.pg[pw.cpg].volume = 3000;// pw.ppg[pw.cpgNum].MaxVolume;
            pw.pg[pw.cpg].panL = 16;
            pw.pg[pw.cpg].port = port;
            pw.pg[pw.cpg].beforepcmLoopAddress = -1;
        }

        public override void InitChip()
        {
            if (!use) return;

            for (int ch = 0; ch < ChMax; ch++)
            {
                partWork pw = lstPartWork[ch];
                pw.pg[pw.cpg].MaxVolume = Ch[ch].MaxVolume;
                pw.pg[pw.cpg].volume = 3000;// pw.ppg[pw.cpgNum].MaxVolume;
                pw.pg[pw.cpg].panL = 16;
            }

        }

        public override void StorePcm(Dictionary<int, clsPcm> newDic, KeyValuePair<int, clsPcm> v, byte[] buf, bool is16bit, int samplerate, params object[] option)
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

                //65536 バイトを超える場合はそれ以降をカット
                if (size > 0x10000)
                {
                    List<byte> n = newBuf.ToList();
                    n.RemoveRange(0x10000, (int)(size - 0x10000));
                    newBuf = n.ToArray();
                    size = 0x10000;
                }

                //空いているBankを探す
                int freeBank = 0;
                int freeAdr = 0;
                do
                {
                    if (memoryMap.Count < freeBank + 1)
                    {
                        memoryMap.Add(0);
                        freeAdr = 0 + freeBank * 0x10000;
                        memoryMap[freeBank] += size;
                        break;
                    }

                    if (size < 0x10000 - memoryMap[freeBank])
                    {
                        freeAdr = (int)(memoryMap[freeBank] + freeBank * 0x10000);
                        memoryMap[freeBank] += size;
                        break;
                    }

                    freeBank++;

                } while (true);

                //パディング(空きが足りない場合はバンクをひとつ進める(0x10000)為、空きを全て埋める)
                while (freeAdr > pi.totalBuf.Length - pi.totalHeaderLength)
                {
                    int fs = (pi.totalBuf.Length - pi.totalHeaderLength) % 0x10000;
                    List<byte> n = pi.totalBuf.ToList();
                    for (int i = 0; i < 0x10000 - fs; i++) n.Add(0x00);
                    pi.totalBuf = n.ToArray();
                    pi.totalBufPtr += 0x10000 - fs;
                }

                newDic.Add(
                    v.Key
                    , new clsPcm(
                        v.Value.num
                        , v.Value.seqNum, v.Value.chip
                        , v.Value.chipNumber
                        , v.Value.fileName
                        , v.Value.freq
                        , v.Value.vol
                        , freeAdr
                        , freeAdr + size - 1
                        , size
                        , (v.Value.loopAdr == -1 ? v.Value.loopAdr : (v.Value.loopAdr + freeAdr))
                        , is16bit
                        , samplerate)
                    );

                if (newDic[v.Key].loopAdr != -1 && (v.Value.loopAdr < 0 || v.Value.loopAdr >= size))
                {
                    msgBox.setErrMsg(string.Format(msg.get("E22000")
                        , newDic[v.Key].loopAdr
                        , size - 1), new LinePos("-"));
                    newDic[v.Key].loopAdr = -1;
                    newDic[v.Key].status = enmPCMSTATUS.ERROR;
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
                    Array.Copy(buf, 0, pi.totalBuf, freeAdr+ pi.totalHeaderLength, buf.Length);
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
                newDic[v.Key].status = enmPCMSTATUS.ERROR;
            }
        }

        public override void StorePcmRawData(clsPcmDatSeq pds, byte[] buf, bool isRaw, bool is16bit, int samplerate, params object[] option)
        {
            throw new NotImplementedException();
        }

        public override string DispRegion(clsPcm pcm)
        {
            return string.Format("{0,-10} {1,-7} {2,-5:D3} {3,-4:D2} ${4,-7:X4} ${5,-7:X4} {6} ${7,-7:X4}  {8,4} {9}\r\n"
                , Name //0
                , pcm.chipNumber!=0 ? "SEC" : "PRI" //1
                , pcm.num //2
                , pcm.stAdr >> 16 //3
                , pcm.stAdr & 0xffff //4
                , pcm.edAdr & 0xffff //5
                , pcm.loopAdr == -1 ? "N/A     " : string.Format("${0,-7:X4}", (pcm.loopAdr & 0xffff)) //6
                , pcm.size //7
                ,"NONE" //mode //8
                , pcm.status.ToString() //9
                );
        }


        private void OutQSoundPort(MML mml,byte[] cmd, partWork pw, byte adr, ushort data)
        {
            parent.OutData(
                mml,
                cmd
                , (byte)(data >> 8)
                , (byte)data
                , adr
                );
        }

        private void OutQSoundKeyOff(MML mml, partWork pw)
        {
            int bch = (pw.pg[pw.cpg].ch - 1) & 0xf;
            byte adr = (byte)((bch << 3) + 0x00);
            ushort data = (ushort)(pw.pg[pw.cpg].pcmBank);
            OutQSoundPort(mml, port[0], pw
                , adr
                , (ushort)(data & 0x7fff)
                );
            pw.pg[pw.cpg].beforepcmBank = -1;

            adr = (byte)((pw.pg[pw.cpg].ch << 3) + 0x03);
            data = 0;

            OutQSoundPort(mml,port[0], pw
                , adr
                , (ushort)data
                );
        }

        private void OutQSoundKeyOn(partWork pw, MML mml)
        {
            byte adr = 0;
            ushort data = 0;

            if (pw.pg[pw.cpg].instrument == -1)
            {
                LinePos lp = mml?.line?.Lp;
                if (lp == null) lp = new LinePos("-");
                msgBox.setErrMsg(msg.get("E22005"), lp);
                return;
            }

            //Volume
            SetVolume(pw, mml);

            //Address shift
            int stAdr = pw.pg[pw.cpg].pcmStartAddress + pw.pg[pw.cpg].addressShift;
            if (stAdr >= pw.pg[pw.cpg].pcmEndAddress) stAdr = pw.pg[pw.cpg].pcmEndAddress - 1;

            //if (pw.ppg[pw.cpgNum].beforepcmStartAddress != stAdr)
            {
                //StartAdr
                adr = (byte)((pw.pg[pw.cpg].ch << 3) + 0x01);
                data = (ushort)stAdr;
                OutQSoundPort(mml, port[0], pw
                    , adr
                    , (ushort)data
                    );

                pw.pg[pw.cpg].beforepcmStartAddress = stAdr;
            }

            if (pw.pg[pw.cpg].beforepcmEndAddress != pw.pg[pw.cpg].pcmEndAddress)
            {
                //EndAdr
                adr = (byte)((pw.pg[pw.cpg].ch << 3) + 0x05);
                data = (ushort)pw.pg[pw.cpg].pcmEndAddress;
                OutQSoundPort(mml, port[0], pw
                    , adr
                    , (ushort)data
                    );

                pw.pg[pw.cpg].beforepcmEndAddress = pw.pg[pw.cpg].pcmEndAddress;
            }

            if (pw.pg[pw.cpg].beforepcmLoopAddress != (pw.pg[pw.cpg].pcmEndAddress - pw.pg[pw.cpg].pcmLoopAddress) && pw.pg[pw.cpg].beforepcmLoopAddress!=4)
            {
                //LoopAdr
                adr = (byte)((pw.pg[pw.cpg].ch << 3) + 0x04);
                data = (ushort)(pw.pg[pw.cpg].pcmLoopAddress == -1
                    ? 4 //QSoundはループがデフォ。(thanks Ian(@SuperCTR)さん)
                    : (pw.pg[pw.cpg].pcmEndAddress- pw.pg[pw.cpg].pcmLoopAddress)
                    );
                OutQSoundPort(mml, port[0], pw
                    , adr
                    , (ushort)data
                    );

                pw.pg[pw.cpg].beforepcmLoopAddress = data;
            }

            if (pw.pg[pw.cpg].beforepcmBank != pw.pg[pw.cpg].pcmBank)
            {
                int bch = (pw.pg[pw.cpg].ch - 1) & 0xf;
                adr = (byte)((bch << 3) + 0x00);
                data = (ushort)(pw.pg[pw.cpg].pcmBank);
                OutQSoundPort(mml, port[0], pw
                    , adr
                    , (ushort)(data | 0x8000)
                    );

                pw.pg[pw.cpg].beforepcmBank = pw.pg[pw.cpg].pcmBank;
            }

            adr = (byte)((pw.pg[pw.cpg].ch << 3) + 0x03);
            data = (ushort)0x8000;

            if (parent.instPCM[pw.pg[pw.cpg].instrument].status != enmPCMSTATUS.ERROR)
            {
                parent.instPCM[pw.pg[pw.cpg].instrument].status = enmPCMSTATUS.USED;
            }

            OutQSoundPort(mml, port[0], pw
                , adr
                , (ushort)data
                );
        }

        public override void SetFNum(partWork pw, MML mml)
        {
            int f = GetQSoundFNum(mml, pw, pw.pg[pw.cpg].octaveNow, pw.pg[pw.cpg].noteCmd, pw.pg[pw.cpg].shift + pw.pg[pw.cpg].keyShift);//
            if (pw.pg[pw.cpg].bendWaitCounter != -1)
            {
                f = pw.pg[pw.cpg].bendFnum;
            }
            f = f + pw.pg[pw.cpg].detune;
            for (int lfo = 0; lfo < 4; lfo++)
            {
                if (!pw.pg[pw.cpg].lfo[lfo].sw)
                {
                    continue;
                }
                if (pw.pg[pw.cpg].lfo[lfo].type != eLfoType.Vibrato)
                {
                    continue;
                }
                f += pw.pg[pw.cpg].lfo[lfo].value + pw.pg[pw.cpg].lfo[lfo].param[6];
            }

            f = Common.CheckRange(f, 0, 0xffff);
            if (pw.pg[pw.cpg].freq == f) return;

            pw.pg[pw.cpg].freq = f;

            //Delta
            int data = f & 0xffff;
            if (pw.pg[pw.cpg].beforeFNum != data)
            {
                byte adr = (byte)((pw.pg[pw.cpg].ch << 3) + 0x02);
                OutQSoundPort(mml, port[0], pw
                    , adr
                    , (ushort)data
                    );
                pw.pg[pw.cpg].beforeFNum = data;
            }

        }

        public override int GetFNum(partWork pw, MML mml, int octave, char cmd, int shift)
        {
            return GetQSoundFNum(mml, pw, octave, cmd, shift);
        }

        public int GetQSoundFNum(MML mml, partWork pw, int octave, char noteCmd, int shift)
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

                if (pw.pg[pw.cpg].instrument < 0 || !parent.instPCM.ContainsKey(pw.pg[pw.cpg].instrument))
                {
                    return 0;
                }

                double freq = (double)parent.instPCM[pw.pg[pw.cpg].instrument].samplerate;
                if (parent.instPCM[pw.pg[pw.cpg].instrument].freq != -1)
                {
                    freq = (double)parent.instPCM[pw.pg[pw.cpg].instrument].freq;
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

        public override void SetKeyOn(partWork pw, MML mml)
        {
            OutQSoundKeyOn(pw, mml);
        }

        public override void SetKeyOff(partWork pw, MML mml)
        {
            OutQSoundKeyOff(mml, pw);
        }

        public override void SetVolume(partWork pw, MML mml)
        {
            int vol = pw.pg[pw.cpg].volume;

            if (pw.pg[pw.cpg].envelopeMode)
            {
                vol = 0;
                if (pw.pg[pw.cpg].envIndex != -1)
                {
                    vol = pw.pg[pw.cpg].envVolume - (pw.pg[pw.cpg].MaxVolume - pw.pg[pw.cpg].volume);
                }
            }
            //Console.WriteLine("{0} ", pw.ppg[pw.cpgNum].envVolume);

            for (int lfo = 0; lfo < 4; lfo++)
            {
                if (!pw.pg[pw.cpg].lfo[lfo].sw)
                {
                    continue;
                }
                if (pw.pg[pw.cpg].lfo[lfo].type != eLfoType.Tremolo)
                {
                    continue;
                }
                vol += pw.pg[pw.cpg].lfo[lfo].value + pw.pg[pw.cpg].lfo[lfo].param[6];
            }

            int data = Common.CheckRange(vol, 0, 65535);
            if (pw.pg[pw.cpg].beforeVolume != data)
            {
                byte adr = (byte)((pw.pg[pw.cpg].ch << 3) + 0x06);
                OutQSoundPort(mml,port[0], pw
                    , adr
                    , (ushort)data
                    );
                pw.pg[pw.cpg].beforeVolume = data;
            }

        }

        public override void SetLfoAtKeyOn(partWork pw, MML mml)
        {
            for (int lfo = 0; lfo < 4; lfo++)
            {
                clsLfo pl = pw.pg[pw.cpg].lfo[lfo];
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
                    SetFNum(pw, mml);
                }
                if (pl.type == eLfoType.Tremolo)
                {
                    pw.pg[pw.cpg].beforeVolume = -1;
                    SetVolume(pw, mml);
                }
            }
        }

        public override void SetToneDoubler(partWork pw, MML mml)
        {
            //実装不要
        }

        public override int GetToneDoublerShift(partWork pw, int octave, char noteCmd, int shift)
        {
            return 0;
        }

        public override void CmdPan(partWork pw, MML mml)
        {
            int p = (int)mml.args[0];
            p = Common.CheckRange(p, 0, 32);
            pw.pg[pw.cpg].panL = (32 - p);

            byte adr = (byte)(pw.pg[pw.cpg].ch + 0x80);
            OutQSoundPort(mml,port[0], pw
                , adr
                , (ushort)(pw.pg[pw.cpg].panL + 0x110)
                );

            SetDummyData(pw, mml);
        }

        public override void CmdInstrument(partWork pw, MML mml)
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
                n = SetEnvelopParamFromInstrument(pw, n, mml);
                return;
            }

            n = Common.CheckRange(n, 0, 255);

            if (!parent.instPCM.ContainsKey(n))
            {
                msgBox.setErrMsg(string.Format(msg.get("E22003"), n)
                    , mml.line.Lp);
                return;
            }

            if (parent.instPCM[n].chip != enmChipType.QSound)
            {
                msgBox.setErrMsg(string.Format(msg.get("E22004"), n)
                    , mml.line.Lp);
                return;
            }

            pw.pg[pw.cpg].instrument = n;
            pw.pg[pw.cpg].pcmStartAddress = (int)parent.instPCM[n].stAdr;
            pw.pg[pw.cpg].pcmEndAddress = (int)parent.instPCM[n].edAdr;
            pw.pg[pw.cpg].pcmLoopAddress = (int)parent.instPCM[n].loopAdr;
            pw.pg[pw.cpg].beforepcmLoopAddress = -1;
            pw.pg[pw.cpg].pcmBank = (int)((parent.instPCM[n].stAdr >> 16));
            SetDummyData(pw, mml);

        }

        public override void CmdY(partWork pw, MML mml)
        {
            if (mml.args[0] is string) return;

            byte adr = (byte)(int)mml.args[0];
            ushort dat = (ushort)(int)mml.args[1];

            OutQSoundPort(mml, port[0], pw, adr, dat);
        }

        public override void CmdLoopExtProc(partWork pw, MML mml)
        {
        }
    }
}
