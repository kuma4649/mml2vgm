using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class C140 : ClsChip
    {
        public int Interface = 0;
        public bool isSystem2 = true;
        public List<long> memoryMap = null;

        public C140(ClsVgm parent, int chipID, string initialPartName, string stPath, bool isSecondary) : base(parent, chipID, initialPartName, stPath, isSecondary)
        {
            _chipType = enmChipType.C140;
            _Name = "C140";
            _ShortName = "C140";
            _ChMax = 24;
            _canUsePcm = true;
            IsSecondary = isSecondary;

            Frequency = 8000000;
            Interface = 0x0;//System2

            Ch = new ClsChannel[ChMax];
            SetPartToCh(Ch, initialPartName);
            foreach (ClsChannel ch in Ch)
            {
                ch.Type = enmChannelType.PCM;
                ch.isSecondary = chipID == 1;
                ch.MaxVolume = 255;
            }

            pcmDataInfo = new clsPcmDataInfo[] { new clsPcmDataInfo() };
            pcmDataInfo[0].totalBufPtr = 0L;
            pcmDataInfo[0].use = false;
            pcmDataInfo[0].totalBuf = new byte[15] {
                0x67, 0x66, 0x8D
                , 0x00, 0x00, 0x00, 0x00 //size of data
                , 0x00, 0x00, 0x00, 0x00 //size of the entire ROM
                , 0x00, 0x00, 0x00, 0x00 //start address of data
            };

            Envelope = new Function();
            Envelope.Max = 255;
            Envelope.Min = 0;

            makePcmTbl();
            memoryMap = new List<long>();
        }

        public override void InitPart(ref partWork pw)
        {
            pw.MaxVolume = 255;
            pw.volume = pw.MaxVolume;
            pw.port0 = 0xd4;
        }

        public override void InitChip()
        {
            if (!use) return;

            for (int ch = 0; ch < ChMax; ch++)
            {
                partWork pw = lstPartWork[ch];
                pw.MaxVolume = Ch[ch].MaxVolume;
                pw.panL = 255;
                pw.panR = 255;
                pw.volume = pw.MaxVolume;
            }

            if (IsSecondary) parent.dat[0xab] |= 0x40;
        }


        public override void SetPCMDataBlock()
        {
            if (use && pcmData != null && pcmData.Length > 0)
                parent.OutData(pcmData);
        }

        /// <summary>
        ///  8bit unsigned Wave ->  8bit signed LinnerPCM
        /// 16bit signed   Wave -> 13bit signed CompPCM
        /// </summary>
        /// <param name="newDic"></param>
        /// <param name="v"></param>
        /// <param name="buf"></param>
        /// <param name="option"></param>
        public override void StorePcm(Dictionary<int, clsPcm> newDic, KeyValuePair<int, clsPcm> v, byte[] buf,bool is16bit,int samplerate, params object[] option)
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
                while (freeAdr > pi.totalBuf.Length - 15)
                {
                    int fs = (pi.totalBuf.Length - 15) % 0x10000;
                    //if (size > 0x10000 - fs)
                    //{
                    List<byte> n = pi.totalBuf.ToList();
                    for (int i = 0; i < 0x10000 - fs; i++) n.Add(0x80);
                    pi.totalBuf = n.ToArray();
                    pi.totalBufPtr += 0x10000 - fs;
                    //}
                }

                newDic.Add(
                    v.Key
                    , new clsPcm(
                        v.Value.num
                        , v.Value.seqNum, v.Value.chip
                        , v.Value.isSecondary
                        , v.Value.fileName
                        , v.Value.freq
                        , v.Value.vol
                        , freeAdr
                        , freeAdr + size 
                        , size
                        , v.Value.loopAdr
                        , is16bit
                        , samplerate)
                    );

                if (newDic[v.Key].loopAdr != -1 && (newDic[v.Key].loopAdr < 0 || newDic[v.Key].loopAdr >= size))
                {
                    msgBox.setErrMsg(string.Format("PCMデータのサイズを超えたloopアドレス[{0}]を指定しています。0～{1}で指定してください。"
                        , newDic[v.Key].loopAdr
                        , size - 1));
                    newDic[v.Key].loopAdr = -1;
                    newDic[v.Key].status = enmPCMSTATUS.ERROR;
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
                    Array.Copy(buf, 0, pi.totalBuf, freeAdr, buf.Length);
                }

                Common.SetUInt32bit31(pi.totalBuf, 3, (UInt32)(pi.totalBuf.Length - 7), IsSecondary);
                Common.SetUInt32bit31(pi.totalBuf, 7, (UInt32)(pi.totalBuf.Length - 7));
                pi.use = true;
                pcmData = pi.use ? pi.totalBuf : null;
            }
            catch
            {
                pi.use = false;
                newDic[v.Key].status = enmPCMSTATUS.ERROR;
            }

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

        public int GetC140FNum(partWork pw, int octave, char noteCmd, int shift)
        {
            try
            {
                int o = octave - 1;
                int n = Const.NOTE.IndexOf(noteCmd) + shift;
                if (n >= 0)
                {
                    o += n / 12;
                    o = Common.CheckRange(o, 0, 7);
                    n %= 12;
                }
                else
                {
                    o += n / 12 - 1;
                    o = Common.CheckRange(o, 0, 7);
                    n %= 12;
                    if (n < 0) { n += 12; }
                }

                if (pw.instrument < 0 || !parent.instPCM.ContainsKey(pw.instrument))
                {
                    return 0;
                }

                if (parent.instPCM[pw.instrument].freq == -1)
                {
                    return ((int)(
                        1.531931
                        * 8000.0
                        * Const.pcmMTbl[n]
                        * Math.Pow(2, (o - 3))
                        * ((double)parent.instPCM[pw.instrument].samplerate/8000.0)
                        ));
                }
                else
                {
                    return ((int)(
                        1.531931
                        * 8000.0
                        * Const.pcmMTbl[n]
                        * Math.Pow(2, (o - 3))
                        * ((double)parent.instPCM[pw.instrument].freq/8000.0)
                        ));
                }

            }
            catch
            {
                return 0;
            }
        }

        public void OutC140Port(partWork pw, byte port, byte adr, byte data)
        {
            parent.OutData(
                pw.port0
                , (byte)(port | (IsSecondary ? 0x80 : 0))
                , adr
                , data
                );
        }

        public void OutC140KeyOff(partWork pw)
        {
            int adr = pw.ch * 16 + 0x05;
            byte data = 0x00;

            OutC140Port(pw
                , (byte)(adr >> 8)
                , (byte)adr
                , data);
        }

        public void OutC140KeyOn(partWork pw)
        {
            int adr = 0;
            byte data = 0;

            //KeyOff
            //OutC140KeyOff(pw);

            //Volume
            SetVolume(pw);

            if (pw.beforepcmStartAddress != pw.pcmStartAddress)
            {
                //StartAdr H
                adr = pw.ch * 16 + 0x06;
                data = (byte)((pw.pcmStartAddress & 0xff00) >> 8);
                OutC140Port(pw
                    , (byte)(adr >> 8)
                    , (byte)adr
                    , data);

                //StartAdr L
                adr = pw.ch * 16 + 0x07;
                data = (byte)((pw.pcmStartAddress & 0x00ff) >> 0);
                OutC140Port(pw
                    , (byte)(adr >> 8)
                    , (byte)adr
                    , data);

                pw.beforepcmStartAddress = pw.pcmStartAddress;
            }

            if (pw.beforepcmEndAddress != pw.pcmEndAddress)
            {
                //EndAdr H
                adr = pw.ch * 16 + 0x08;
                data = (byte)((pw.pcmEndAddress & 0xff00) >> 8);
                OutC140Port(pw
                    , (byte)(adr >> 8)
                    , (byte)adr
                    , data);
                //EndAdr L
                adr = pw.ch * 16 + 0x09;
                data = (byte)((pw.pcmEndAddress & 0x00ff) >> 0);
                OutC140Port(pw
                    , (byte)(adr >> 8)
                    , (byte)adr
                    , data);

                pw.beforepcmEndAddress = pw.pcmEndAddress;
            }

            if (pw.beforepcmLoopAddress != pw.pcmLoopAddress)
            {
                if (pw.pcmLoopAddress != -1)
                {
                    //LoopAdr H
                    adr = pw.ch * 16 + 0x0a;
                    data = (byte)((pw.pcmLoopAddress & 0xff00) >> 8);
                    OutC140Port(pw
                        , (byte)(adr >> 8)
                        , (byte)adr
                        , data);

                    //LoopAdr L
                    adr = pw.ch * 16 + 0x0b;
                    data = (byte)((pw.pcmLoopAddress & 0x00ff) >> 0);
                    OutC140Port(pw
                        , (byte)(adr >> 8)
                        , (byte)adr
                        , data);

                    pw.beforepcmLoopAddress = pw.pcmLoopAddress;
                }
            }

            if (pw.beforepcmBank != pw.pcmBank)
            {
                adr = pw.ch * 16 + 0x04;
                data = (byte)((pw.pcmBank & 7) | (isSystem2 ? ((pw.pcmBank & 0x8) << 2) : ((pw.pcmBank & 0x18) << 1)));
                OutC140Port(pw
                    , (byte)(adr >> 8)
                    , (byte)adr
                    , data);

                pw.beforepcmBank = pw.pcmBank;
            }

            adr = pw.ch * 16 + 0x05;

            bool is16bit = false;
            if (pw.instrument >= 0 && parent.instPCM.ContainsKey(pw.instrument))
            {
                is16bit = (bool)parent.instPCM[pw.instrument].is16bit;
            }

            if (parent.instPCM[pw.instrument].status != enmPCMSTATUS.ERROR)
            {
                parent.instPCM[pw.instrument].status = enmPCMSTATUS.USED;
            }

            data = (byte)(
                  0x80 //KeyOn
                | (is16bit ? 0x08 : 0x00) //CompPCM
                | ((pw.pcmLoopAddress != -1) ? 0x10 : 0x00) //Loop
                );
            OutC140Port(pw
                , (byte)(adr >> 8)
                , (byte)adr
                , data);
        }


        public override void SetFNum(partWork pw)
        {
            int f = GetC140FNum(pw, pw.octaveNow, pw.noteCmd, pw.shift + pw.keyShift);//
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
            if (pw.freq == f) return;

            pw.freq = f;


            //Delta
            int data = f & 0xffff;
            if (pw.beforeFNum != data)
            {
                int adr = pw.ch * 16 + 0x02;
                OutC140Port(pw
                    , (byte)(adr >> 8)
                    , (byte)adr
                    , (byte)(data >> 8));
                adr++;
                OutC140Port(pw
                    , (byte)(adr >> 8)
                    , (byte)adr
                    , (byte)data);
                pw.beforeFNum = data;
            }

        }

        public override int GetFNum(partWork pw, int octave, char cmd, int shift)
        {
            return GetC140FNum(pw, octave, cmd, shift);
        }

        public override void SetKeyOn(partWork pw)
        {
            OutC140KeyOn(pw);
        }

        public override void SetKeyOff(partWork pw)
        {
            OutC140KeyOff(pw);
        }

        public override void SetVolume(partWork pw)
        {
            int vol = pw.volume;

            if (pw.envelopeMode)
            {
                vol = 0;
                if (pw.envIndex != -1)
                {
                    vol = pw.envVolume - (pw.MaxVolume - pw.volume);
                }
            }
            //Console.WriteLine("{0} ", pw.envVolume);

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

            int vl = vol * pw.panL / pw.MaxVolume;
            int vr = vol * pw.panR / pw.MaxVolume;
            vl = Common.CheckRange(vl, 0, pw.MaxVolume);
            vr = Common.CheckRange(vr, 0, pw.MaxVolume);

            if (pw.beforeLVolume != vl)
            {
                //Volume(Left)
                int adr = pw.ch * 16 + 0x01;
                OutC140Port(pw
                    , (byte)(adr >> 8)
                    , (byte)adr
                    , (byte)vl);
                pw.beforeLVolume = vl;
            }

            if (pw.beforeRVolume != vr)
            {
                //Volume(Right)
                int adr = pw.ch * 16 + 0x00;
                OutC140Port(pw
                    , (byte)(adr >> 8)
                    , (byte)adr
                    , (byte)vr);
                pw.beforeRVolume = vr;
            }
        }

        public override void SetLfoAtKeyOn(partWork pw)
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
                    SetFNum(pw);
                }
                if (pl.type == eLfoType.Tremolo)
                {
                    pw.beforeVolume = -1;
                    SetVolume(pw);
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


        public override void CmdPan(partWork pw, MML mml)
        {
            int l = (int)mml.args[0];
            int r = (int)mml.args[1];

            l = Common.CheckRange(l, 0, 255);
            r = Common.CheckRange(r, 0, 255);
            pw.panL = l;
            pw.panR = r;
        }

        public override void CmdInstrument(partWork pw, MML mml)
        {
            char type = (char)mml.args[0];
            int n = (int)mml.args[1];

            if (type == 'I')
            {
                msgBox.setErrMsg("この音源はInstrumentを持っていません。"
                    , mml.line.Fn
                    , mml.line.Num);
                return;
            }

            if (type == 'T')
            {
                msgBox.setErrMsg("Tone DoublerはOPN,OPM音源以外では使用できません。"
                    , mml.line.Fn
                    , mml.line.Num);
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
                msgBox.setErrMsg(string.Format("PCM定義に指定された音色番号({0})が存在しません。", n)
                    , mml.line.Fn
                    , mml.line.Num);
                return;
            }

            if (parent.instPCM[n].chip != enmChipType.C140)
            {
                msgBox.setErrMsg(string.Format("指定された音色番号({0})はC140向けPCMデータではありません。", n)
                    , mml.line.Fn
                    , mml.line.Num);
                return;
            }

            pw.instrument = n;
            pw.pcmStartAddress = (int)parent.instPCM[n].stAdr;
            pw.pcmEndAddress = (int)parent.instPCM[n].edAdr;
            pw.pcmLoopAddress = (int)parent.instPCM[n].loopAdr;
            pw.pcmBank = (int)((parent.instPCM[n].stAdr >> 16));

        }

        public override void CmdY(partWork pw, MML mml)
        {
            if (mml.args[0] is string) return;

            int adr = (int)mml.args[0];
            byte dat = (byte)mml.args[1];

            OutC140Port(pw, (byte)(adr >> 8), (byte)adr, dat);
        }

        public override void CmdLoopExtProc(partWork pw, MML mml)
        {
        }

    }
}
