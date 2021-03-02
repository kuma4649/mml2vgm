using musicDriverInterface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public class C140 : ClsChip
    {
        public int Interface = 0;
        public bool isSystem2 = true;
        public List<long> memoryMap = null;

        public C140(ClsVgm parent, int chipID, string initialPartName, string stPath, int chipNumber) : base(parent, chipID, initialPartName, stPath, chipNumber)
        {
            _chipType = enmChipType.C140;
            _Name = "C140";
            _ShortName = "C140";
            _ChMax = 24;
            _canUsePcm = true;
            _canUsePI = true;
            ChipNumber = chipNumber;

            Frequency = 0x538e;// 8000000;
            port = new byte[][] { new byte[] { 0xd4 } };
            Interface = 0x0;//System2

            dataType = 0x8d;

            if (string.IsNullOrEmpty(initialPartName)) return;

            Ch = new ClsChannel[ChMax];
            SetPartToCh(Ch, initialPartName);
            foreach (ClsChannel ch in Ch)
            {
                ch.Type = enmChannelType.PCM;
                ch.chipNumber = chipID == 1;
                ch.MaxVolume = 255;
            }

            pcmDataInfo = new clsPcmDataInfo[] { new clsPcmDataInfo() };
            pcmDataInfo[0].totalBufPtr = 0L;
            pcmDataInfo[0].use = false;
            if (parent.info.format == enmFormat.ZGM)
            {
                if (parent.ChipCommandSize == 2)
                    pcmDataInfo[0].totalBuf = new byte[] { 0x07, 0x00, 0x66, 0x8D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                else
                    pcmDataInfo[0].totalBuf = new byte[] { 0x07, 0x66, 0x8D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            }
            else
                pcmDataInfo[0].totalBuf = new byte[] { 0x67, 0x66, 0x8D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            pcmDataInfo[0].totalHeaderLength = pcmDataInfo[0].totalBuf.Length;
            pcmDataInfo[0].totalHeadrSizeOfDataPtr = (parent.ChipCommandSize == 2) ? 4 : 3;

            Envelope = new Function();
            Envelope.Max = 255;
            Envelope.Min = 0;

            makePcmTbl();
            memoryMap = new List<long>();
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

        public override void InitChip()
        {
            if (!use) return;

            for (int ch = 0; ch < ChMax; ch++)
            {
                foreach (partPage pg in lstPartWork[ch].pg)
                {
                    pg.MaxVolume = Ch[ch].MaxVolume;
                    pg.panL = 255;
                    pg.panR = 255;
                    pg.volume = pg.MaxVolume;
                }
            }

            if (ChipNumber != 0)
            {
                parent.dat[0xab] = new outDatum(enmMMLType.unknown, null, null, (byte)(parent.dat[0xab].val | 0x40));
            }
        }


        /// <summary>
        ///  8bit unsigned Wave ->  8bit signed LinnerPCM
        /// 16bit signed   Wave -> 13bit signed CompPCM
        /// </summary>
        /// <param name="newDic"></param>
        /// <param name="v"></param>
        /// <param name="buf"></param>
        /// <param name="option"></param>
        public override void StorePcm(Dictionary<int,Tuple<string, clsPcm>> newDic, KeyValuePair<int, clsPcm> v, byte[] buf, bool is16bit, int samplerate, params object[] option)
        {
            clsPcmDataInfo pi = pcmDataInfo[0];

            try
            {

                List<byte> dBuf = new List<byte>();
                if (is16bit)
                {
                    //16bit signed Only
                    EncC140CompressedPCM(dBuf, buf, true, true);
                }
                else
                {
                    //8bit unsigned Only
                    EncC140LinerSigned8bitPCM(dBuf, buf, false, false);
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
                        , (v.Value.loopAdr == -1 ? v.Value.loopAdr : (v.Value.loopAdr + freeAdr))
                        , is16bit
                        , samplerate)
                    ));

                if (newDic[v.Key].Item2.loopAdr != -1 && (v.Value.loopAdr < 0 || v.Value.loopAdr >= size))
                {
                    msgBox.setErrMsg(string.Format(msg.get("E09000")
                        , newDic[v.Key].Item2.loopAdr
                        , size - 1), new LinePos(null, "-"));
                    newDic[v.Key].Item2.loopAdr = -1;
                    newDic[v.Key].Item2.status = enmPCMSTATUS.ERROR;
                }

                if (freeAdr == pi.totalBufPtr)
                {
                    pi.totalBufPtr += size;

                    if (pi.totalBufPtr > 0xf_ffff) isSystem2 = false;

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
                    , ChipNumber != 0
                    );//size of data ( totalHeadrSizeOfDataPtr:サイズ値を設定する位置 4:サイズ値の大きさ(4byte32bit) )
                Common.SetUInt32bit31(
                    pi.totalBuf
                    , pi.totalHeadrSizeOfDataPtr + 4
                    , (UInt32)(pi.totalBuf.Length - (pi.totalHeadrSizeOfDataPtr + 4 + 4 + 4))
                    );//size of the entire ROM
                pi.use = true;
                pcmDataEasy = pi.use ? pi.totalBuf : null;
            }
            catch
            {
                pi.use = false;
                newDic[v.Key].Item2.status = enmPCMSTATUS.ERROR;
            }

            //for(int i = 0; i < pi.totalBuf.Length; i++)
            //{
            //    if (i % 16 == 0)
            //    {
            //        Console.Write("\r\n{0:x06}::", i);
            //    }
            //    Console.Write("{0:x02} ", pi.totalBuf[i]);
            //}
        }

        public override void StorePcmRawData(clsPcmDatSeq pds, byte[] buf, bool isRaw, bool is16bit, int samplerate, params object[] option)
        {
            bool isCPCM = false;

            //Optionをチェック が0以外の場合、bufに対し13bitCompPCM向け圧縮処理を行う
            if (pds.Option != null && pds.Option.Length > 0)
            {
                isCPCM = pds.Option[0].ToString() != "0";
            }

            List<byte> dBuf = new List<byte>();
            if (isRaw)
            {
                //Rawファイル

                //圧縮指定がある時　圧縮処理を実施
                //それ以外は そのまま
                if (isCPCM) EncC140CompressedPCM(dBuf, buf, true, true);
                else dBuf.AddRange(buf);
            }
            else
            {
                //Wavファイル

                //16bitWavの時　圧縮処理を実施
                //それ以外は8bitLiner処理を実施
                if (is16bit) EncC140CompressedPCM(dBuf, buf, true, true);
                else EncC140LinerSigned8bitPCM(dBuf, buf, false, false);
            }
            buf = dBuf.ToArray();

            pcmDataDirect.Add(Common.MakePCMDataBlock(dataType, pds, buf));
        }




        //Encoder code ここから
        //vgmplay/mame c140 code ここから
        private int[] pcmtbl;
        private void makePcmTbl()
        {
            pcmtbl = new int[8];
            int segbase = 0;
            for (int i = 0; i < 8; i++)
            {
                pcmtbl[i] = segbase;  //segment base value
                segbase += 16 << i;
            }
        }
        //vgmplay/mame c140 code ここまで

        /// <summary>
        /// CompressPCMの作成
        /// </summary>
        /// <param name="dBuf">出力先</param>
        /// <param name="sBuf">入力元(PCMデータ 8/16bit unsigned/signed)</param>
        /// <param name="is16BitWav">入力元のデータは16bitWavか</param>
        /// <param name="isSigned">入力元のデータはSignedか</param>
        /// <param name="bruteforce">総当りルーチン使います？</param>
        private void EncC140CompressedPCM(List<byte> dBuf, byte[] sBuf, bool is16BitWav, bool isSigned, bool bruteforce = false)
        {

            int sBufLength = sBuf.Length;
            if (is16BitWav) sBufLength /= 2;

            for (int n = 0; n < sBufLength; n++)
            {
                ushort uDat;
                if (is16BitWav)
                {
                    uDat = (ushort)(sBuf[n * 2] + sBuf[n * 2 + 1] * 0x100);
                    if (!isSigned) uDat -= 0x8000;
                }
                else
                {
                    uDat = sBuf[n];
                    if (!isSigned) uDat -= 0x80;
                }

                short sDat = (short)uDat;

                //8bit -> 13bitにする
                double m = sDat / (double)sbyte.MaxValue;
                if (is16BitWav)
                {
                    //16bit->13bit
                    m = sDat / (double)short.MaxValue;
                }
                short bit13data = (short)(4095 * m);

                int shiftBit = 0;
                int upper4bit = 0;
                int mask = 0;
                byte dDat = 0;
                short destBit13Data = 0;

                if (!bruteforce)
                {
                    //シフトがいくつ必要かチェック
                    if (bit13data >= 0)
                    {
                        //符号なし
                        for (int i = 0; i < 8; i++)
                        {
                            if ((bit13data & (0b1000_0000_0000 >> i)) != 0)
                            {
                                shiftBit = 7 - i;
                                break;
                            }
                        }
                    }
                    else
                    {
                        //符号有り
                        for (int i = 0; i < 8; i++)
                        {
                            if ((bit13data & (0b1000_0000_0000 >> i)) == 0)
                            {
                                shiftBit = 7 - i;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    //総当りチェック
                    int distance = int.MaxValue;
                    int ansShiftBit = -1;
                    for (shiftBit = 0; shiftBit < 8; shiftBit++)
                    {
                        mask = unchecked((short)(0b1111_1111_0000_0000));
                        mask >>= (7 - shiftBit);
                        upper4bit = (bit13data & mask) >> (shiftBit + 1);
                        dDat = (byte)((upper4bit << 3) | (shiftBit & 7));

                        int sdt = (sbyte)dDat >> 3;
                        if (sdt < 0) sdt = (sdt << ((sbyte)dDat & 7)) - pcmtbl[(sbyte)dDat & 7];
                        else sdt = (sdt << ((sbyte)dDat & 7)) + pcmtbl[(sbyte)dDat & 7];
                        destBit13Data = (short)sdt;

                        //本来の音に近づいていれば答えのshiftBit更新
                        if (Math.Abs(bit13data - destBit13Data) < distance)
                        {
                            distance = Math.Abs(bit13data - destBit13Data);
                            ansShiftBit = shiftBit;
                        }
                    }
                    shiftBit = ansShiftBit;
                }


                mask = unchecked((short)(0b1111_1111_0000_0000));
                mask >>= (7 - shiftBit);
                upper4bit = (bit13data & mask) >> (shiftBit + 1);
                destBit13Data = (short)((upper4bit << shiftBit) + pcmtbl[shiftBit] * (bit13data >= 0 ? 1 : -1));
                dDat = (byte)((upper4bit << 3) | (shiftBit & 7));

                //#if DEBUG
                //                Console.WriteLine("src:{0:x2} 13bit:{1:x2}    dest:{2:x2} 13bit:{3:x2}   src-dest:{4}"
                //                    , sDat, bit13data
                //                    , dDat, destBit13Data
                //                    , bit13data - destBit13Data);
                //#endif

                dBuf.Add(dDat);
            }
        }

        /// <summary>
        /// LinerPCMの作成
        /// </summary>
        /// <param name="dBuf">出力先</param>
        /// <param name="sBuf">入力元(PCMデータ)</param>
        /// <param name="is16BitWav">入力元のデータは16bitWavか</param>
        /// <param name="isSigned">入力元のデータはSignedか</param>
        private void EncC140LinerSigned8bitPCM(List<byte> dBuf, byte[] sBuf, bool is16BitWav, bool isSigned)
        {
            int sBufLength = sBuf.Length;
            if (is16BitWav) sBufLength /= 2;

            for (int n = 0; n < sBufLength; n++)
            {
                ushort uDat;
                if (is16BitWav)
                {
                    uDat = (ushort)(sBuf[n * 2] + sBuf[n * 2 + 1] * 0x100);
                    if (!isSigned) uDat -= 0x8000;
                }
                else
                {
                    uDat = sBuf[n];
                    if (!isSigned) uDat -= 0x80;
                }

                short sDat = (short)uDat;

                //8bit はそのまま
                byte dat = (byte)sDat;
                if (is16BitWav)
                {
                    //16bit->8bit
                    double m = sDat / (double)short.MaxValue;
                    dat = (byte)(sbyte.MaxValue * m);
                }

                dBuf.Add(dat);
            }
        }
        //Encoder code ここまで

        public int GetC140FNum(MML mml, partPage page, int octave, char noteCmd, int shift)
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

                if (parent.instPCM[page.instrument].Item2.freq == -1)
                {
                    return ((int)(
                        65536.0 / 2.0 / Frequency //* 384
                        * 8000.0
                        * Const.pcmMTbl[n]
                        * Math.Pow(2, (o - 3))
                        * ((double)parent.instPCM[page.instrument].Item2.samplerate / 8000.0)
                        ));
                }
                else
                {
                    //return ((int)(
                    //    1.531931
                    //    * 8000.0
                    //    * Const.pcmMTbl[n]
                    //    * Math.Pow(2, (o - 3))
                    //    * ((double)parent.instPCM[pw.ppg[pw.cpgNum].instrument].freq / 8000.0)
                    //    ));
                    return ((int)(
                        65536.0 / 2.0 / Frequency //* 384
                        * 8000.0
                        * Const.pcmMTbl[n]
                        * Math.Pow(2, (o - 3))
                        * ((double)parent.instPCM[page.instrument].Item2.freq / 8000.0)
                        ));
                }

            }
            catch
            {
                return 0;
            }
        }

        public void OutC140Port(MML mml, partPage page, byte port, byte adr, byte data)
        {
            SOutData(
                page,
                mml,
                page.port[0]
                , (byte)(port | (ChipNumber != 0 ? 0x80 : 0))
                , adr
                , data
                );
        }

        public void OutC140KeyOff(MML mml, partPage page)
        {
            int adr = page.ch * 16 + 0x05;
            byte data = 0x00;

            OutC140Port(mml, page
                , (byte)(adr >> 8)
                , (byte)adr
                , data);
        }

        public void OutC140KeyOn(partPage page, MML mml)
        {
            int adr = 0;
            byte data = 0;

            if (page.instrument == -1)
            {
                LinePos lp = mml?.line?.Lp;
                if (lp == null) lp = new LinePos(null, "-");
                msgBox.setErrMsg(msg.get("E10030"), lp);
                return;
            }

            //Volume
            SetVolume(page, mml);

            //Address shift
            int stAdr = page.pcmStartAddress + page.addressShift;
            if (stAdr >= page.pcmEndAddress) stAdr = page.pcmEndAddress - 1;

            if (page.spg.beforepcmStartAddress != stAdr)
            {
                //StartAdr H
                adr = page.ch * 16 + 0x06;
                data = (byte)((stAdr & 0xff00) >> 8);
                OutC140Port(mml, page
                    , (byte)(adr >> 8)
                    , (byte)adr
                    , data);

                //StartAdr L
                adr = page.ch * 16 + 0x07;
                data = (byte)((stAdr & 0x00ff) >> 0);
                OutC140Port(mml, page
                    , (byte)(adr >> 8)
                    , (byte)adr
                    , data);

                page.spg.beforepcmStartAddress = stAdr;
            }

            if (page.spg.beforepcmEndAddress != page.pcmEndAddress)
            {
                int eAdr = page.pcmEndAddress;
                //EndAdr H
                adr = page.ch * 16 + 0x08;
                data = (byte)((eAdr & 0xff00) >> 8);
                OutC140Port(mml, page
                    , (byte)(adr >> 8)
                    , (byte)adr
                    , data);
                //EndAdr L
                adr = page.ch * 16 + 0x09;
                data = (byte)((eAdr & 0x00ff) >> 0);
                OutC140Port(mml, page
                    , (byte)(adr >> 8)
                    , (byte)adr
                    , data);

                page.spg.beforepcmEndAddress = page.pcmEndAddress;
            }

            if (page.spg.beforepcmLoopAddress != page.pcmLoopAddress)
            {
                if (page.pcmLoopAddress != -1)
                {
                    //LoopAdr H
                    adr = page.ch * 16 + 0x0a;
                    data = (byte)((page.pcmLoopAddress & 0xff00) >> 8);
                    OutC140Port(mml, page
                        , (byte)(adr >> 8)
                        , (byte)adr
                        , data);

                    //LoopAdr L
                    adr = page.ch * 16 + 0x0b;
                    data = (byte)((page.pcmLoopAddress & 0x00ff) >> 0);
                    OutC140Port(mml, page
                        , (byte)(adr >> 8)
                        , (byte)adr
                        , data);

                    page.spg.beforepcmLoopAddress = page.pcmLoopAddress;
                }
            }

            if (page.spg.beforepcmBank != page.pcmBank)
            {
                adr = page.ch * 16 + 0x04;
                data = (byte)((page.pcmBank & 7) | (isSystem2 ? ((page.pcmBank & 0x8) << 2) : ((page.pcmBank & 0x18) << 1)));
                OutC140Port(mml, page
                    , (byte)(adr >> 8)
                    , (byte)adr
                    , data);

                page.spg.beforepcmBank = page.pcmBank;
            }

            adr = page.ch * 16 + 0x05;

            bool is16bit = false;
            if (page.instrument >= 0 && parent.instPCM.ContainsKey(page.instrument))
            {
                is16bit = (bool)parent.instPCM[page.instrument].Item2.is16bit;
            }

            if (parent.instPCM[page.instrument].Item2.status != enmPCMSTATUS.ERROR)
            {
                parent.instPCM[page.instrument].Item2.status = enmPCMSTATUS.USED;
            }

            data = (byte)(
                  0x80 //KeyOn
                | (is16bit ? 0x08 : 0x00) //CompPCM
                | ((page.pcmLoopAddress != -1) ? 0x10 : 0x00) //Loop
                );
            OutC140Port(mml, page
                , (byte)(adr >> 8)
                , (byte)adr
                , data);
        }


        public override void SetFNum(partPage page, MML mml)
        {
            int arpNote = page.arpFreqMode ? 0 : page.arpDelta;
            int arpFreq = page.arpFreqMode ? page.arpDelta : 0;

            int f = GetC140FNum(mml, page, page.octaveNow, page.noteCmd, page.shift + page.keyShift + arpNote);//
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
            if (page.spg.freq == f) return;

            page.spg.freq = f;


            //Delta
            int data = f & 0xffff;
            if (page.spg.beforeFNum != data)
            {
                int adr = page.ch * 16 + 0x02;
                OutC140Port(mml, page
                    , (byte)(adr >> 8)
                    , (byte)adr
                    , (byte)(data >> 8));
                adr++;
                OutC140Port(mml, page
                    , (byte)(adr >> 8)
                    , (byte)adr
                    , (byte)data);
                page.beforeFNum = data;
            }

        }

        public override int GetFNum(partPage page, MML mml, int octave, char cmd, int shift)
        {
            return GetC140FNum(mml, page, octave, cmd, shift);
        }

        public override void SetKeyOn(partPage page, MML mml)
        {
            //OutC140KeyOn(page, mml);
            page.keyOn = true;
        }

        public override void SetKeyOff(partPage page, MML mml)
        {
            OutC140KeyOff(mml, page);
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

            int vl = vol * page.panL / page.MaxVolume;
            int vr = vol * page.panR / page.MaxVolume;
            vl = Common.CheckRange(vl, 0, page.MaxVolume);
            vr = Common.CheckRange(vr, 0, page.MaxVolume);

            if (page.spg.beforeLVolume != vl)
            {
                //Volume(Left)
                int adr = page.ch * 16 + 0x01;
                OutC140Port(mml, page
                    , (byte)(adr >> 8)
                    , (byte)adr
                    , (byte)vl);
                page.spg.beforeLVolume = vl;
            }

            if (page.spg.beforeRVolume != vr)
            {
                //Volume(Right)
                int adr = page.ch * 16 + 0x00;
                OutC140Port(mml, page
                    , (byte)(adr >> 8)
                    , (byte)adr
                    , (byte)vr);
                page.spg.beforeRVolume = vr;
            }
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
            int l = (int)mml.args[0];
            int r = (int)mml.args[1];

            l = Common.CheckRange(l, 0, 255);
            r = Common.CheckRange(r, 0, 255);
            page.panL = l;
            page.panR = r;

            SetDummyData(page, mml);
        }

        public override void CmdInstrument(partPage page, MML mml)
        {
            char type = (char)mml.args[0];
            int n = (int)mml.args[1];

            if (type == 'I')
            {
                msgBox.setErrMsg(msg.get("E09001")
                    , mml.line.Lp);
                return;
            }

            if (type == 'T')
            {
                msgBox.setErrMsg(msg.get("E09002")
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
                msgBox.setErrMsg(string.Format(msg.get("E09003"), n)
                    , mml.line.Lp);
                return;
            }

            if (parent.instPCM[n].Item2.chip != enmChipType.C140)
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

            int adr = (int)mml.args[0];
            byte dat = (byte)(int)mml.args[1];

            OutC140Port(mml, page, (byte)(adr >> 8), (byte)adr, dat);
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


        public override void SetupPageData(partWork pw, partPage page)
        {

            OutC140KeyOff(null, page);
            page.spg.instrument = -1;

            //周波数
            page.spg.freq = -1;
            SetFNum(page, null);

            //音量(パン兼用)
            page.spg.beforeLVolume = -1;
            page.spg.beforeRVolume = -1;
            SetVolume(page, null);

        }



        public override void MultiChannelCommand(MML mml)
        {
            if (!use) return;
            //int dat = 0;

            foreach (partWork pw in lstPartWork)
            {
                partPage page = pw.cpg;

                if (page.keyOn)
                {
                    page.keyOn = false;
                    OutC140KeyOn(page, mml);
                }

            }


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
                , pcm.Item2.is16bit ? 1 : 0 //8
                , pcm.Item2.status.ToString() //9
                );
        }

    }
}
