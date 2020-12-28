using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class Y8950 : ClsOPL
    {
        public List<long> memoryMap = null;

        public Y8950(ClsVgm parent, int chipID, string initialPartName, string stPath, int chipNumber) : base(parent, chipID, initialPartName, stPath, chipNumber)
        {
            _chipType = enmChipType.Y8950;
            _Name = "Y8950";
            _ShortName = "Y89";
            _ChMax = 9 + 5 + 1;//FM 9ch Rhythm 5ch ADPCM 1ch
            _canUsePcm = true;

            Frequency = 3579545;
            port = new byte[][] {
                new byte[] { (byte)(chipNumber != 0 ? 0xac : 0x5c) }
            };

            if (string.IsNullOrEmpty(initialPartName)) return;

            Dictionary<string, List<double>> dic = MakeFNumTbl();
            if (dic != null)
            {
                int c = 0;
                FNumTbl = new int[1][];
                FNumTbl[0] = new int[13];
                foreach (double v in dic["FNUM_00"])
                {
                    FNumTbl[0][c++] = (int)v;
                    if (c == FNumTbl[0].Length) break;
                }
                FNumTbl[0][FNumTbl[0].Length - 1] = FNumTbl[0][0] * 2;

            }

            Ch = new ClsChannel[ChMax];
            SetPartToCh(Ch, initialPartName);

            pcmDataInfo = new clsPcmDataInfo[] { new clsPcmDataInfo() };
            pcmDataInfo[0].totalBufPtr = 0L;
            pcmDataInfo[0].use = false;
            if (parent.info.format == enmFormat.ZGM)
            {
                if (parent.ChipCommandSize == 2)
                {
                    if (chipNumber == 0)
                        pcmDataInfo[0].totalBuf = new byte[] { 0x07, 0x00, 0x66, 0x88, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                    else
                        pcmDataInfo[0].totalBuf = new byte[] { 0x07, 0x00, 0x66, 0x88, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                }
                else
                {
                    if (chipNumber == 0)
                        pcmDataInfo[0].totalBuf = new byte[] { 0x07, 0x66, 0x88, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                    else
                        pcmDataInfo[0].totalBuf = new byte[] { 0x07, 0x66, 0x88, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                }
            }
            else
            {
                if (chipNumber == 0)
                    pcmDataInfo[0].totalBuf = new byte[] { 0x67, 0x66, 0x88, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                else
                    pcmDataInfo[0].totalBuf = new byte[] { 0x67, 0x66, 0x88, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            }

            pcmDataInfo[0].totalHeaderLength = pcmDataInfo[0].totalBuf.Length;
            pcmDataInfo[0].totalHeadrSizeOfDataPtr = (parent.ChipCommandSize == 2) ? 4 : 3;

            memoryMap = new List<long>();
        }

        public override void InitChip()
        {

            if (!use) return;

            //FM Off
            outAllKeyOff(null, lstPartWork[0]);
            rhythmStatus = 0x00;
            beforeRhythmStatus = 0xff;
            connectionSel = 0;
            beforeConnectionSel = -1;

            //ADPCM Reset
            parent.OutData((MML)null, port[0], 0x08, 0x01);//bit0:  0 RAM  1 ROM

        }

        public override void InitPart(partWork pw)
        {
            foreach (partPage page in pw.pg)
            {
                page.beforeVolume = -1;
                page.volume = 60;
                page.MaxVolume = 63;
                page.beforeEnvInstrument = 0;
                page.envInstrument = 0;
                page.port = port;
                page.mixer = 0;
                page.noise = 0;
                page.pan = 3;
                page.Type = enmChannelType.FMOPL;
                page.isOp4Mode = false;
                if (page.ch > 8) page.Type = enmChannelType.RHYTHM;
                if (page.ch == 14)
                {
                    page.volume = 220;
                    page.MaxVolume = 255;
                    page.Type = enmChannelType.ADPCM;
                }
            }
        }



        protected override void SetInstAtOneOpeWithoutKslTl(partPage page, MML mml, int opeNum,
int ar, int dr, int sl, int rr,
int mt, int am, int vib, int eg,
int kr,
int ws
)
        {

            // % 18       ... port毎のoperator番号を得る --- (1)
            // / 6 ) * 8  ... (1) に対応するアドレスは6opeごとに8アドレス毎に分けられ、
            // % 6        ...                         0～5アドレスに割り当てられている
            int adr = ((opeNum % 18) / 6) * 8 + (opeNum % 6);

            ////slot1かslot2を求める
            //// % 6        ... slotは6opeの範囲で0か1を繰り返す
            //// / 3        ... slotは3ope毎に0か1を繰り返す
            //int slot = (opeNum % 6) / 3;

            SOutData(page, mml, port[0], (byte)(0x80 + adr), (byte)(((sl & 0xf) << 4) | (rr & 0xf)));
            SOutData(page, mml, port[0], (byte)(0x60 + adr), (byte)(((ar & 0xf) << 4) | (dr & 0xf)));
            SetInstAtOneOpeAmVibEgKsMl(page, mml, port[0], (byte)(0x20 + adr), mt, am, vib, eg, kr);
            //SOutData(page, mml, port, (byte)(0xe0 + adr), (byte)(ws & 0x3)); Y8950はOPL(YM3526)なのでwsなし
        }

        public override void SetVolume(partPage page, MML mml)
        {
            if (page.Type == enmChannelType.ADPCM)
            {
                SetAdpcmVolume(mml, page);
                return;
            }

            base.SetFmVolume(page, mml);
        }

        public override void SetFNum(partPage page, MML mml)
        {
            if (page.Type == enmChannelType.ADPCM)
            {
                SetAdpcmFNum(mml, page);
                return;
            }

            base.SetFmFNum(page, mml);
        }



        public override void StorePcm(Dictionary<int, Tuple<string, clsPcm>> newDic, KeyValuePair<int, clsPcm> v, byte[] buf, bool is16bit, int samplerate, params object[] option)
        {
            clsPcmDataInfo pi = pcmDataInfo[0];

            try
            {
                EncAdpcmA ea = new EncAdpcmA();
                buf = ea.YM_ADPCM_B_Encode(buf, is16bit, false, true);
                long size = buf.Length;

                byte[] newBuf;
                newBuf = new byte[size];
                Array.Copy(buf, newBuf, size);
                buf = newBuf;
                int m256kbyte = 0b100_0000_0000_0000_0000;//CASAdr 9bit  RASAdr 9bit = 18bit(2+4+4+4+4)

                //262,144 バイトを超える場合はそれ以降をカット
                if (size > m256kbyte)
                {
                    List<byte> n = newBuf.ToList();
                    n.RemoveRange(m256kbyte, (int)(size - m256kbyte));
                    newBuf = n.ToArray();
                    size = m256kbyte;
                }

                //空いているBankを探す
                int freeBank = 0;
                int freeAdr = 0;
                do
                {
                    if (memoryMap.Count < freeBank + 1)
                    {
                        memoryMap.Add(0);
                        freeAdr = 0 + freeBank * m256kbyte;
                        memoryMap[freeBank] += size;
                        break;
                    }

                    if (size < m256kbyte - memoryMap[freeBank])
                    {
                        freeAdr = (int)(memoryMap[freeBank] + freeBank * m256kbyte);
                        memoryMap[freeBank] += size;
                        break;
                    }

                    freeBank++;

                } while (true);

                //パディング(空きが足りない場合はバンクをひとつ進める(0b100_0000_0000_0000_0000)為、空きを全て埋める)
                while (freeAdr > pi.totalBuf.Length - pi.totalHeaderLength)
                {
                    int fs = (pi.totalBuf.Length - pi.totalHeaderLength) % m256kbyte;

                    List<byte> n = pi.totalBuf.ToList();
                    for (int i = 0; i < m256kbyte - fs; i++) n.Add(0x00);
                    pi.totalBuf = n.ToArray();
                    pi.totalBufPtr += m256kbyte - fs;
                }

                //long tSize = size;
                //size = buf.Length;

                newDic.Add(
                    v.Key
                    , new Tuple<string, clsPcm>("", new clsPcm(
                        v.Value.num
                        , v.Value.seqNum, v.Value.chip
                        , v.Value.chipNumber
                        , v.Value.fileName
                        , v.Value.freq
                        , v.Value.vol
                        , freeAdr
                        , freeAdr + size - 1
                        , size
                        , -1
                        , is16bit
                        , samplerate
                        , (v.Value.loopAdr & 3)
                        )
                    ));

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

        public void SetADPCMAddress(MML mml, partPage page, int startAdr, int endAdr)
        {

            if (page.spg.pcmStartAddress != startAdr)
            {
                SOutData(page, mml, port[0], 0x09, (byte)((startAdr >> 5) & 0xff));
                SOutData(page, mml, port[0], 0x0a, (byte)((startAdr >> 13) & 0xff));
                page.spg.pcmStartAddress = startAdr;
            }

            if (page.spg.pcmEndAddress != endAdr)
            {
                SOutData(page, mml, port[0], 0x0b, (byte)(((endAdr - 0x40) >> 5) & 0xff));
                SOutData(page, mml, port[0], 0x0c, (byte)(((endAdr - 0x40) >> 13) & 0xff));
                page.spg.pcmEndAddress = endAdr;
            }
        }

        public void SetAdpcmFNum(MML mml, partPage page)
        {
            int arpNote = page.arpFreqMode ? 0 : page.arpDelta;
            int arpFreq = page.arpFreqMode ? page.arpDelta : 0;

            int f = GetAdpcmFNum(page, page.octaveNow, page.noteCmd, page.shift + page.keyShift + arpNote);//
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

            page.freq = f;
            page.spg.freq = f;

            byte data = 0;

            data = (byte)(f & 0xff);
            SOutData(page, mml, port[0], 0x10, data);//DELTA-N(L)

            data = (byte)((f & 0xff00) >> 8);
            SOutData(page, mml, port[0], 0x11, data);//DELTA-N(H)
        }

        public int GetAdpcmFNum(partPage page, int octave, char noteCmd, int shift)
        {
            int o = octave - 1;
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

            double freq = 8000.0;
            if (page.instrument != -1)
            {
                freq = (double)parent.instPCM[page.instrument].Item2.samplerate;
                if (parent.instPCM[page.instrument].Item2.freq != -1)
                {
                    freq = (double)parent.instPCM[page.instrument].Item2.freq;
                }
            }

            return (int)(
                //(1 << 16) * 8000.0 / 50000.0 * 2.0 * Const.pcmMTbl[n] * Math.Pow(2, (o - 4)) * (freq / 8000.0)
                (1 << 16) * Const.pcmMTbl[n] * Math.Pow(2, (o - 3)) * freq / 50000.0
                );
        }

        public void SetAdpcmVolume(MML mml, partPage page)
        {

            int vol = page.volume;
            if (page.envelopeMode)
            {
                vol = 0;
                if (page.envIndex != -1)
                {
                    vol = page.volume - (0xff - page.envVolume);
                }
            }
            vol = Common.CheckRange(vol, 0, 0xff);//256段階

            if (page.spg.beforeVolume != vol)
            {
                SOutData(page, mml, port[0], 0x12, (byte)vol);
                page.spg.beforeVolume = vol;
            }
        }


        public override void CmdInstrument(partPage page, MML mml)
        {
            if(page.Type== enmChannelType.ADPCM)
            {
                int n = (int)mml.args[1];
                n = Common.CheckRange(n, 0, 255);
                if (!parent.instPCM.ContainsKey(n))
                {
                    msgBox.setErrMsg(string.Format(msg.get("E28004"), n), mml.line.Lp);
                    return;
                }
                if (parent.instPCM[n].Item2.chip != enmChipType.Y8950)
                {
                    msgBox.setErrMsg(string.Format(msg.get("E28005"), n), mml.line.Lp);
                    return;
                }

                page.instrument = n;
                SetADPCMAddress(
                    mml,
                    page
                    , (int)parent.instPCM[n].Item2.stAdr
                    , (int)parent.instPCM[n].Item2.edAdr);

                page.pcmLoopAddress = (int)(long)parent.instPCM[n].Item2.option[0];

                parent.instPCM[n].Item2.status = enmPCMSTATUS.USED;

                return;
            }

            base.CmdInstrument(page, mml);
        }


        public override byte[] getPortFromCh(int ch)
        {
            return port[0];
        }

        public override string DispRegion(Tuple<string, clsPcm> pcm)
        {
            return string.Format("{0,-10} {1,-7} {2,-5:D3} N/A  ${3,-7:X6} ${4,-7:X6} N/A      ${5,-7:X6}  NONE {6}\r\n"
                , Name
                , pcm.Item2.chipNumber != 0 ? "SEC" : "PRI"
                , pcm.Item2.num
                , pcm.Item2.stAdr & 0xff_ff_ff
                , pcm.Item2.edAdr & 0xff_ff_ff
                , pcm.Item2.size
                , pcm.Item2.status.ToString()
                );
        }
    }
}
