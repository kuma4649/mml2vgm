using musicDriverInterface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public class C352 : ClsChip
    {
        private short[] mulaw_tab;
        public List<long> memoryMap = null;
        private double C352Divider;

        public bool executeKeyonoff { get; private set; }

        public C352(ClsVgm parent, int chipID, string initialPartName, string stPath, int chipNumber) : base(parent, chipID, initialPartName, stPath, chipNumber)
        {
            _chipType = enmChipType.C352;
            _Name = "C352";
            _ShortName = "C352";
            _ChMax = 32;
            _canUsePcm = true;
            _canUsePI = true;
            ChipNumber = chipNumber;

            Frequency = 24192000;
            C352Divider = 288.0;

            port = new byte[][] { new byte[] { 0xe1 } };

            dataType = 0x92;

            if (string.IsNullOrEmpty(initialPartName)) return;

            Ch = new ClsChannel[ChMax];
            SetPartToCh(Ch, initialPartName);
            foreach (ClsChannel ch in Ch)
            {
                ch.Type = enmChannelType.PCM;
                ch.chipNumber = chipID == 1;
                ch.MaxVolume = 256;
            }

            pcmDataInfo = new clsPcmDataInfo[] { new clsPcmDataInfo() };
            pcmDataInfo[0].totalBufPtr = 0L;
            pcmDataInfo[0].use = false;
            if (parent.info.format == enmFormat.ZGM)
            {
                if (parent.ChipCommandSize == 2)
                    pcmDataInfo[0].totalBuf = new byte[] { 0x07, 0x00, 0x66, 0x92, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                else
                    pcmDataInfo[0].totalBuf = new byte[] { 0x07, 0x66, 0x92, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            }
            else
                pcmDataInfo[0].totalBuf = new byte[] { 0x67, 0x66, 0x92, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            pcmDataInfo[0].totalHeaderLength = pcmDataInfo[0].totalBuf.Length;
            pcmDataInfo[0].totalHeadrSizeOfDataPtr = (parent.ChipCommandSize == 2) ? 4 : 3;

            Envelope = new Function();
            Envelope.Max = 256;
            Envelope.Min = 0;

            makeC352Table();
            memoryMap = new List<long>();
        }

        public override void InitPart(partWork pw)
        {
            foreach (partPage pg in pw.pg)
            {
                pg.MaxVolume = 256;
                pg.volume = pg.MaxVolume;
                pg.port = port;
                pg.C352flag = 0;
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
                    pg.panRL = 0;
                    pg.panRR = 0;
                    pg.volume = pg.MaxVolume;
                }
            }

            if (ChipNumber != 0)
            {
                //C352はDualChip非サポート?
                //parent.dat[0xef] = new outDatum(enmMMLType.unknown, null, null, (byte)(parent.dat[0xef].val | 0x40));
            }
        }

        /// <summary>
        ///  8bit unsigned Wave ->  8bit signed LinnerPCM
        /// 16bit signed   Wave ->  8bit signed mu-law
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

                if (is16bit)
                {
                    //16bit signed Only
                    buf = standbyPCM(buf, true, true);
                }
                else
                {
                    //8bit unsigned Only
                    buf = standbyPCM(buf, false, false);
                }

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
                if (isCPCM) buf = standbyPCM(buf, false, true);
                else dBuf.AddRange(buf);
            }
            else
            {
                //Wavファイル

                //16bitWavの時　圧縮処理を実施
                //それ以外は8bitLiner処理を実施
                if (is16bit) buf = standbyPCM(buf, true, false);
                else buf = standbyPCM(buf, false, false);
            }

            pcmDataDirect.Add(Common.MakePCMDataBlock(dataType, pds, buf));
        }

        private byte[] standbyPCM(byte[] pcmbuf, bool is16bitSrc, bool mulaw)
        {
            byte[] dest = new byte[pcmbuf.Length / (is16bitSrc ? 2 : 1)];

            // 8/16bitPCM  ->  8bitPCM/mu-law(C352)
            for (int i = 0; i < dest.Length; i++)
            {
                int m;
                if (is16bitSrc)
                {
                    short dat = (short)(pcmbuf[i * 2] + pcmbuf[i * 2 + 1] * 0x100);
                    if (mulaw) m = mulawConversion(dat);
                    else m = dat >> 8;

                    dest[i] = (byte)m;
                    continue;
                }

                if (mulaw)
                {
                    short dat = (short)(pcmbuf[i] << 8);
                    m = mulawConversion(dat);
                }
                else m = pcmbuf[i] ^ 0x80;
                dest[i] = (byte)m;
            }

            return dest;
        }


        /// <summary>
        /// C352向けmulawテーブルのインデックスに変換します
        /// Thank you! Ian Karlsson(@SuperCTR)!
        /// </summary>
        private int mulawConversion(short dat)
        {
            int m;
            dat += 0x10;
            int k;
            k = Math.Abs(dat);
            if (k > 0x7a00)
                m = 0x7f;
            else if (k > 0x4400)
                m = 0x64 + ((k - 0x4400) / 0x200);
            else if (k > 0x1000)
                m = 0x30 + ((k - 0x1000) / 0x100);
            else if (k > 0x400)
                m = 0x18 + ((k - 0x400) / 0x80);
            else if (k > 0x200)
                m = 0x10 + ((k - 0x200) / 0x40);
            else
                m = k / 0x20;

            m |= ((dat < 0) ? 0x80 : 0);
            //Console.WriteLine("org:{0} enc:{1} dis:{2}", dat, mulaw_tab[m], Math.Abs(dat - mulaw_tab[m]));
            return m;
        }

        /// <summary>
        /// C352向けmulawテーブルを生成します
        /// Thank you! musicalman!
        /// </summary>
        private void makeC352Table()
        {
            mulaw_tab = new short[256];

            int j = 0;

            for (int i = 0; i < 128; i++)
            {
                mulaw_tab[i] = (short)(j << 5);
                if (i < 16)
                    j += 1;
                else if (i < 24)
                    j += 2;
                else if (i < 48)
                    j += 4;
                else if (i < 100)
                    j += 8;
                else
                    j += 16;
            }
            for (int i = 128; i < 256; i++)
                mulaw_tab[i] = (short)((~mulaw_tab[i - 128]) & 0xffe0);
        }



        public int GetC352FNum(MML mml, partPage page, int octave, char noteCmd, int shift)
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
                        0x10000 / (Frequency / C352Divider)
                        * 8000.0
                        * Const.pcmMTbl[n]
                        * Math.Pow(2, (o - 3))
                        * ((double)parent.instPCM[page.instrument].Item2.samplerate / 8000.0)
                        ));
                }
                else
                {
                    return ((int)(
                        0x10000 / (Frequency / C352Divider)
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

        public void OutC352Port(MML mml, partPage page, int adr, int data)
        {
            SOutData(
                page,
                mml,
                page.port[0]
                , (byte)(adr >> 8)
                , (byte)(adr)
                , (byte)(data >> 8)
                , (byte)(data)
                );
        }



        public override void SetFNum(partPage page, MML mml)
        {
            int arpNote = page.arpFreqMode ? 0 : page.arpDelta;
            int arpFreq = page.arpFreqMode ? page.arpDelta : 0;

            int f = GetC352FNum(mml, page, page.octaveNow, page.noteCmd, page.shift + page.keyShift + arpNote);//
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
                int adr = page.ch * 8 + 0x02;//0x02:freq
                OutC352Port(mml, page
                    , adr
                    , data);
                page.spg.beforeFNum = data;
            }

        }

        public override int GetFNum(partPage page, MML mml, int octave, char cmd, int shift)
        {
            return GetC352FNum(mml, page, octave, cmd, shift);
        }

        public override void SetKeyOn(partPage page, MML mml)
        {
            page.keyOn = true;
            page.keyOff = false;
            page.changeFlag = true;
            executeKeyonoff = true;
        }

        private void OutC352KeyOn(partPage page, MML mml)
        {
            //Volume
            SetVolume(page, mml);

            //Address shift
            int stAdr = page.pcmStartAddress + page.addressShift;
            if (stAdr >= page.pcmEndAddress) stAdr = page.pcmEndAddress - 1;

            int adr, data;
            if (page.spg.beforepcmStartAddress != stAdr)
            {
                //StartAdr
                adr = page.ch * 8 + 0x05;
                data = (ushort)(stAdr & 0xffff);
                OutC352Port(mml, page, adr, data);

                page.spg.beforepcmStartAddress = stAdr;
            }

            if (page.spg.beforepcmEndAddress != page.pcmEndAddress)
            {
                int eAdr = page.pcmEndAddress;
                //EndAdr
                adr = page.ch * 8 + 0x06;
                data = (ushort)(eAdr & 0xffff);
                OutC352Port(mml, page, adr, data);

                page.spg.beforepcmEndAddress = page.pcmEndAddress;
            }

            if (page.spg.beforepcmLoopAddress != page.pcmLoopAddress)
            {
                if (page.pcmLoopAddress != -1)
                {
                    //LoopAdr H
                    adr = page.ch * 8 + 0x07;
                    data = (ushort)(page.pcmLoopAddress & 0xffff);
                    OutC352Port(mml, page, adr, data);

                    page.spg.beforepcmLoopAddress = page.pcmLoopAddress;
                }
            }

            if (page.spg.beforepcmBank != page.pcmBank)
            {
                adr = page.ch * 8 + 0x04;
                data = (ushort)(page.pcmBank & 0xffff);
                OutC352Port(mml, page, adr, data);

                page.spg.beforepcmBank = page.pcmBank;
            }

        }

        public override void SetKeyOff(partPage page, MML mml)
        {

            int flag =
    0x0000 |
    0x2000 |
    (page.noise != 0 ? 0x0010 : 0x0000) |
    (parent.instPCM[page.instrument].Item2.is16bit ? 0x0008 : 0x0000) |
    (parent.instPCM[page.instrument].Item2.loopAdr != -1 ? 0x0002 : 0x0000) |
    (page.C352flag & 0xffff)
    ;

            OutC352Port(mml, page, page.ch * 8 + 3, flag);

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

            vol = Common.CheckRange(vol, 0, 256);
            int vl = vol * page.panL / page.MaxVolume;
            int vr = vol * page.panR / page.MaxVolume;
            vl = Common.CheckRange(vl, 0, page.MaxVolume);
            vr = Common.CheckRange(vr, 0, page.MaxVolume);
            int rvl = vol * page.panRL / page.MaxVolume;
            int rvr = vol * page.panRR / page.MaxVolume;
            rvl = Common.CheckRange(rvl, 0, page.MaxVolume);
            rvr = Common.CheckRange(rvr, 0, page.MaxVolume);

            if (page.spg.beforeLVolume != vl || page.spg.beforeRVolume != vr)
            {
                //front Volume
                int adr = page.ch * 8 + 0x00;
                OutC352Port(mml, page
                    , adr
                    , ((byte)vl << 8) | (byte)vr
                    );
                page.spg.beforeLVolume = vl;
                page.spg.beforeRVolume = vr;
            }

            if (page.spg.beforeRLVolume != rvl || page.spg.beforeRRVolume != rvr)
            {
                //rear Volume
                int adr = page.ch * 8 + 0x01;
                OutC352Port(mml, page
                    , adr
                    , ((byte)rvl << 8) | (byte)rvr
                    );
                page.spg.beforeRLVolume = rvl;
                page.spg.beforeRRVolume = rvr;
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
            if (mml.args.Count == 2 || mml.args.Count == 4)
            {
                int l = (int)mml.args[0];
                int r = (int)mml.args[1];
                l = Common.CheckRange(l, 0, 255);
                r = Common.CheckRange(r, 0, 255);
                page.panL = l;
                page.panR = r;
            }

            if (mml.args.Count == 4)
            {
                int rl = (int)mml.args[2];
                int rr = (int)mml.args[3];
                rl = Common.CheckRange(rl, 0, 255);
                rr = Common.CheckRange(rr, 0, 255);
                page.panRL = rl;
                page.panRR = rr;
            }

            SetDummyData(page, mml);
        }

        public override void CmdInstrument(partPage page, MML mml)
        {
            char type = (char)mml.args[0];
            int n = (int)mml.args[1];

            if (type == 'I')
            {
                msgBox.setErrMsg(msg.get("E27001")
                    , mml.line.Lp);
                return;
            }

            if (type == 'T')
            {
                msgBox.setErrMsg(msg.get("E27002")
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
                msgBox.setErrMsg(string.Format(msg.get("E27003"), n)
                    , mml.line.Lp);
                return;
            }

            if (parent.instPCM[n].Item2.chip != enmChipType.C352)
            {
                msgBox.setErrMsg(string.Format(msg.get("E27004"), n)
                    , mml.line.Lp);
                return;
            }

            page.instrument = n;
            page.pcmStartAddress = (int)(parent.instPCM[n].Item2.stAdr & 0xffff);
            page.pcmEndAddress = (int)(parent.instPCM[n].Item2.edAdr & 0xffff);
            page.pcmLoopAddress = (int)(parent.instPCM[n].Item2.loopAdr & 0xffff);
            page.pcmBank = (int)((parent.instPCM[n].Item2.stAdr >> 16) & 0xffff);
            SetDummyData(page, mml);

        }

        public override void CmdY(partPage page, MML mml)
        {
            if (mml.args[0] is string) return;

            int adr = (int)mml.args[0];
            int dat = (int)mml.args[1];
            if (adr != 1000)
            {
                OutC352Port(mml, page, adr, dat);
            }
            else
            {
                page.C352flag = dat;
                page.changeFlag = true;
            }
        }

        public override void CmdDetune(partPage page, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, -0xffff, 0xffff);
            page.detune = n;
            SetDummyData(page, mml);
        }

        public override void CmdLoopExtProc(partPage page, MML mml)
        {
        }

        public override void CmdNoise(partPage page, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 0, 1);
            page.noise = n;
            page.changeFlag = true;
        }

        public override void CmdNoiseToneMixer(partPage page, MML mml)
        {
            CmdNoise(page, mml);
        }


        public override void SetupPageData(partWork pw, partPage page)
        {

            SetKeyOff(page, null);
            page.spg.instrument = -1;

            //周波数
            page.spg.freq = -1;
            page.spg.beforeFNum = -1;
            SetFNum(page, null);

            //音量(パン兼用)
            page.spg.beforeLVolume = -1;
            page.spg.beforeRVolume = -1;
            page.spg.beforeRLVolume = -1;
            page.spg.beforeRRVolume = -1;
            SetVolume(page, null);

        }

        public override void MultiChannelCommand(MML mml)
        {
            foreach (partWork pw in lstPartWork)
            {
                foreach (partPage page in pw.pg)
                {
                    if (page.instrument == -1) continue;
                    if (!page.changeFlag) continue;
                    page.changeFlag = false;

                    OutC352KeyOn(page, mml);

                    int flag =
                        (page.keyOn ? 0x4000 : 0x0000) |
                        (page.keyOff ? 0x2000 : 0x0000) |
                        (page.noise != 0 ? 0x0010 : 0x0000) |
                        (parent.instPCM[page.instrument].Item2.is16bit ? 0x0008 : 0x0000) |
                        (parent.instPCM[page.instrument].Item2.loopAdr != -1 ? 0x0002 : 0x0000) |
                        (page.C352flag & 0xffff)
                        ;

                    OutC352Port(mml, page, page.ch * 8 + 3, flag);
                }
            }

            if (executeKeyonoff)
            {
                SOutData(lstPartWork[lstPartWork.Count - 1].cpg, mml, port[0], 0x02, 0x02, 0x0, 0x0);//execute keyon/keyoff
                executeKeyonoff = false;
            }
        }



        public override string DispRegion(Tuple<string, clsPcm> pcm)
        {
            return string.Format("{0,-10} {1,-7} {2,-4:D3} ${3,-4:X4} ${4,-7:X4} ${5,-7:X4} {6} ${7,-7:X4}  {8,4} {9}\r\n"
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