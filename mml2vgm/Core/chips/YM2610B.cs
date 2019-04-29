using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class YM2610B : ClsOPN
    {
        protected int[][] _FNumTbl = new int[2][] {
            new int[13]
            ,new int[96]
            //new int[] {
            //// OPNB(FM) : TP = (144 * ftone * (2^20) / M) / (2^(B-1))       32:Divider 2:OPNB 
            ////   c    c+     d    d+     e     f    f+     g    g+     a    a+     b    >c
            // 0x269,0x28e,0x2b4,0x2de,0x309,0x337,0x368,0x39c,0x3d3,0x40e,0x44b,0x48d,0x4d2
            //},
            //new int[] {
            //// OPNB(SSG) : TP = M / (ftone * 32 * 2)       32:Divider 2:OPNB
            ////   c    c+     d    d+     e     f    f+     g    g+     a    a+     b
            // 0xEEE,0xE18,0xD4D,0xC8E,0xBDA,0xB30,0xA8F,0x9F7,0x968,0x8E1,0x861,0x7E9
            //,0x777,0x70C,0x6A7,0x647,0x5ED,0x598,0x547,0x4FC,0x4B4,0x470,0x431,0x3F4
            //,0x3BC,0x386,0x353,0x324,0x2F6,0x2CC,0x2A4,0x27E,0x25A,0x238,0x218,0x1FA
            //,0x1DE,0x1C3,0x1AA,0x192,0x17B,0x166,0x152,0x13F,0x12D,0x11C,0x10C,0x0FD
            //,0x0EF,0x0E1,0x0D5,0x0C9,0x0BE,0x0B3,0x0A9,0x09F,0x096,0x08E,0x086,0x07F
            //,0x077,0x071,0x06A,0x064,0x05F,0x059,0x054,0x050,0x04B,0x047,0x043,0x03F
            //,0x03C,0x038,0x035,0x032,0x02F,0x02D,0x02A,0x028,0x026,0x024,0x022,0x020
            //,0x01E,0x01C,0x01B,0x019,0x018,0x016,0x015,0x014,0x013,0x012,0x011,0x010
            //}
        };

        public byte[] pcmDataEasyA = null;
        public byte[] pcmDataEasyB = null;
        public List<byte[]> pcmDataDirectA = new List<byte[]>();
        public List<byte[]> pcmDataDirectB = new List<byte[]>();

        public int adpcmA_TotalVolume = 63;
        public int adpcmA_beforeTotalVolume = -1;
        public int adpcmA_MAXTotalVolume = 63;
        public byte adpcmA_KeyOn = 0;
        public byte adpcmA_KeyOff = 0;

        public YM2610B(ClsVgm parent, int chipID, string initialPartName, string stPath, bool isSecondary) : base(parent, chipID, initialPartName, stPath, isSecondary)
        {
            _chipType = enmChipType.YM2610B;
            _Name = "YM2610B";
            _ShortName = "OPNB";
            _ChMax = 19;
            _canUsePcm = true;
            _canUsePI = true;
            FNumTbl = _FNumTbl;
            IsSecondary = isSecondary;
            dataType = 0x82;
            Frequency = 8000000;

            Dictionary<string, List<double>> dic = MakeFNumTbl();
            if (dic != null)
            {
                int c = 0;
                foreach (double v in dic["FNUM_00"])
                {
                    FNumTbl[0][c++] = (int)v;
                    if (c == FNumTbl[0].Length) break;
                }
                FNumTbl[0][FNumTbl[0].Length - 1] = FNumTbl[0][0] * 2;

                c = 0;
                foreach (double v in dic["FNUM_01"])
                {
                    FNumTbl[1][c++] = (int)v;
                    if (c == FNumTbl[1].Length) break;
                }
            }

            Ch = new ClsChannel[ChMax];
            SetPartToCh(Ch, initialPartName);
            foreach (ClsChannel ch in Ch)
            {
                ch.Type = enmChannelType.FMOPN;
                ch.isSecondary = chipID == 1;
            }

            Ch[2].Type = enmChannelType.FMOPNex;
            Ch[6].Type = enmChannelType.FMOPNex;
            Ch[7].Type = enmChannelType.FMOPNex;
            Ch[8].Type = enmChannelType.FMOPNex;

            Ch[9].Type = enmChannelType.SSG;
            Ch[10].Type = enmChannelType.SSG;
            Ch[11].Type = enmChannelType.SSG;

            Ch[12].Type = enmChannelType.ADPCMA;
            Ch[13].Type = enmChannelType.ADPCMA;
            Ch[14].Type = enmChannelType.ADPCMA;
            Ch[15].Type = enmChannelType.ADPCMA;
            Ch[16].Type = enmChannelType.ADPCMA;
            Ch[17].Type = enmChannelType.ADPCMA;

            Ch[18].Type = enmChannelType.ADPCMB;

            pcmDataInfo = new clsPcmDataInfo[2] { new clsPcmDataInfo(), new clsPcmDataInfo() };
            pcmDataInfo[0].totalBufPtr = 0L;
            pcmDataInfo[0].use = false;
            pcmDataInfo[1].totalBufPtr = 0L;
            pcmDataInfo[1].use = false;
            if (!isSecondary)
            {
                pcmDataInfo[0].totalBuf = new byte[15] { 0x67, 0x66, 0x82, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                pcmDataInfo[1].totalBuf = new byte[15] { 0x67, 0x66, 0x83, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            }
            else
            {
                pcmDataInfo[0].totalBuf = new byte[15] { 0x67, 0x66, 0x82, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                pcmDataInfo[1].totalBuf = new byte[15] { 0x67, 0x66, 0x83, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            }

            Envelope = new Function();
            Envelope.Max = 255;
            Envelope.Min = 0;

        }

        public override void InitChip()
        {
            if (!use) return;

            //initialize shared param
            OutOPNSetHardLfo(null,lstPartWork[0], false, 0);
            OutOPNSetCh3SpecialMode(null,lstPartWork[0], false);

            //FM Off
            OutFmAllKeyOff();

            //SSG Off
            for (int ch = 9; ch < 12; ch++)
            {
                OutSsgKeyOff(null,lstPartWork[ch]);
                lstPartWork[ch].volume = 0;
                //setSsgVolume(ym2610b[i].lstPartWork[ch]);
                //ym2610b[i].lstPartWork[ch].volume = 15;
            }

            //ADPCM-A/B Reset
            parent.OutData(null,lstPartWork[0].port0, 0x1c, 0xbf);
            parent.OutData(null,lstPartWork[0].port0, 0x1c, 0x00);
            parent.OutData(null,lstPartWork[0].port0, 0x10, 0x00);
            parent.OutData(null,lstPartWork[0].port0, 0x11, 0xc0);

            foreach (partWork pw in lstPartWork)
            {
                if (pw.ch == 0)
                {
                    pw.hardLfoSw = false;
                    pw.hardLfoNum = 0;
                    OutOPNSetHardLfo(null,pw, pw.hardLfoSw, pw.hardLfoNum);
                }

                if (pw.ch < 6)
                {
                    pw.pan.val = 3;
                    pw.ams = 0;
                    pw.fms = 0;
                    if (!pw.dataEnd) OutOPNSetPanAMSPMS(null,pw, 3, 0, 0);
                }
            }

            parent.dat[0x4f] = new outDatum(enmMMLType.unknown, null, null, (byte)(parent.dat[0x4f].val | 0x80));//YM2610B
            if (IsSecondary)
            {
                parent.dat[0x4f] = new outDatum(enmMMLType.unknown, null, null, (byte)(parent.dat[0x4f].val | 0x40));//use Secondary
            }
        }

        public override void InitPart(ref partWork pw)
        {
            pw.slots = (byte)((pw.Type == enmChannelType.FMOPN || pw.ch == 2) ? 0xf : 0x0);
            pw.volume = 127;
            pw.MaxVolume = 127;
            if (pw.Type == enmChannelType.SSG)
            {
                //pw.volume = 32767;
                pw.MaxVolume = 15;
                pw.volume = pw.MaxVolume;
            }
            else if (pw.Type == enmChannelType.ADPCMA)
            {
                //pw.volume = 32767;
                pw.MaxVolume = 31;//5bit
                pw.volume = pw.MaxVolume;
            }
            else if (pw.Type == enmChannelType.ADPCMB)
            {
                //pw.volume = 32767;
                pw.MaxVolume = 255;
                pw.volume = pw.MaxVolume;
            }
            pw.port0 = (byte)(0x8 | (pw.isSecondary ? 0xa0 : 0x50));
            pw.port1 = (byte)(0x9 | (pw.isSecondary ? 0xa0 : 0x50));
        }


        public void SetADPCMAAddress(MML mml,partWork pw, int startAdr, int endAdr)
        {
            if (pw.pcmStartAddress != startAdr)
            {
                parent.OutData(mml,pw.port1, (byte)(0x10 + (pw.ch - 12)), (byte)((startAdr >> 8) & 0xff));
                parent.OutData(mml, pw.port1, (byte)(0x18 + (pw.ch - 12)), (byte)((startAdr >> 16) & 0xff));
                pw.pcmStartAddress = startAdr;
            }

            if (pw.pcmEndAddress != endAdr)
            {
                parent.OutData(mml, pw.port1, (byte)(0x20 + (pw.ch - 12)), (byte)(((endAdr - 0x100) >> 8) & 0xff));
                parent.OutData(mml, pw.port1, (byte)(0x28 + (pw.ch - 12)), (byte)(((endAdr - 0x100) >> 16) & 0xff));
                pw.pcmEndAddress = endAdr;
            }

        }

        public void SetADPCMBAddress(MML mml, partWork pw, int startAdr, int endAdr)
        {
            if (pw.pcmStartAddress != startAdr)
            {
                parent.OutData(mml, pw.port0, 0x12, (byte)((startAdr >> 8) & 0xff));
                parent.OutData(mml, pw.port0, 0x13, (byte)((startAdr >> 16) & 0xff));
                pw.pcmStartAddress = startAdr;
            }

            if (pw.pcmEndAddress != endAdr)
            {
                parent.OutData(mml, pw.port0, 0x14, (byte)(((endAdr - 0x100) >> 8) & 0xff));
                parent.OutData(mml, pw.port0, 0x15, (byte)(((endAdr - 0x100) >> 16) & 0xff));
                pw.pcmEndAddress = endAdr;
            }

            //outData(pw.port1, 0x01, 0x3f);
            //outData(pw.port1, 0x08, 0xdf);
            //outData(pw.port1, 0x00, 0x01);
        }

        public void SetAdpcmBFNum(MML mml,partWork pw)
        {
            int f = GetAdpcmBFNum(pw.octaveNow, pw.noteCmd, pw.shift + pw.keyShift);//
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

            byte data = 0;

            data = (byte)(f & 0xff);
            parent.OutData(mml, pw.port0, 0x19, data);

            data = (byte)((f & 0xff00) >> 8);
            parent.OutData(mml, pw.port0, 0x1a, data);
        }

        public void SetAdpcmBVolume(MML mml,partWork pw)
        {

            int vol = pw.volume;
            if (pw.envelopeMode)
            {
                vol = 0;
                if (pw.envIndex != -1)
                {
                    vol = pw.volume - (0xff - pw.envVolume);
                }
            }
            vol = Common.CheckRange(vol, 0, 0xff);

            if (pw.beforeVolume != vol)
            {
                parent.OutData(mml, pw.port0, 0x1b, (byte)vol);
                pw.beforeVolume = pw.volume;
            }
        }

        public void SetAdpcmBPan(MML mml,partWork pw, int pan)
        {
            if (pw.pan.val != pan)
            {
                parent.OutData(mml, pw.port0, 0x11, (byte)((pan & 0x3) << 6));
                pw.pan.val = pan;
            }
        }

        public int GetAdpcmBFNum(int octave, char noteCmd, int shift)
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

            return (int)(0x49ba * Const.pcmMTbl[n] * Math.Pow(2, (o - 4)));
        }

        public void OutAdpcmBKeyOn(MML mml,partWork pw)
        {

            SetAdpcmBVolume(mml, pw);
            parent.OutData(mml, pw.port0, 0x10, 0x80);

        }

        public void OutAdpcmBKeyOff(MML mml,partWork pw)
        {

            parent.OutData(mml, pw.port0, 0x10, 0x01);

        }


        public override void SetFNum(partWork pw, MML mml)
        {
            if (pw.ch < 9)
                SetFmFNum(pw,mml);
            else if (pw.Type == enmChannelType.SSG)
            {
                SetSsgFNum(pw,mml);
            }
            else if (pw.Type == enmChannelType.ADPCMA)
            {
            }
            else if (pw.Type == enmChannelType.ADPCMB)
            {
                SetAdpcmBFNum(mml,pw);
            }
        }

        public override void SetKeyOn(partWork pw, MML mml)
        {
            if(pw.ch<9)
            OutFmKeyOn(pw,mml);
            else if (pw.Type == enmChannelType.SSG)
            {
                OutSsgKeyOn(pw,mml);
            }
            else if (pw.Type == enmChannelType.ADPCMA)
            {
                pw.keyOn = true;
                pw.keyOff = false;
                if (parent.instPCM[pw.instrument].status != enmPCMSTATUS.ERROR)
                {
                    parent.instPCM[pw.instrument].status = enmPCMSTATUS.USED;
                }
            }
            else if (pw.Type == enmChannelType.ADPCMB)
            {
                OutAdpcmBKeyOn(mml, pw);
                if (parent.instPCM[pw.instrument].status != enmPCMSTATUS.ERROR)
                {
                    parent.instPCM[pw.instrument].status = enmPCMSTATUS.USED;
                }
            }
        }

        public override void SetKeyOff(partWork pw, MML mml)
        {
            if (pw.ch < 9)
                OutFmKeyOff(pw,mml);
            else if (pw.Type == enmChannelType.SSG)
            {
                OutSsgKeyOff(mml, pw);
            }
            else if (pw.Type == enmChannelType.ADPCMA)
            {
                pw.keyOn = false;
                pw.keyOff = true;
            }
            else if (pw.Type == enmChannelType.ADPCMB)
            {
                OutAdpcmBKeyOff(mml, pw);
            }
        }

        public override void SetVolume(partWork pw, MML mml)
        {
            base.SetVolume(pw,mml);

            if (pw.Type == enmChannelType.ADPCMA)
            {
            }
            else if (pw.Type == enmChannelType.ADPCMB)
            {
                SetAdpcmBVolume(mml, pw);
            }
        }

        public override void SetPCMDataBlock(MML mml)
        {
            //if (!CanUsePcm) return;
            if (!use) return;

            SetPCMDataBlock_AB(mml,pcmDataEasyA, pcmDataDirectA);
            SetPCMDataBlock_AB(mml,pcmDataEasyB, pcmDataDirectB);
        }

        private void SetPCMDataBlock_AB(MML mml,byte[] pcmDataEasy,List<byte[]> pcmDataDirect)
        {
            int maxSize = 0;
            if (pcmDataEasy != null && pcmDataEasy.Length > 0)
            {
                maxSize =
                    pcmDataEasy[7]
                    + (pcmDataEasy[8] << 8)
                    + (pcmDataEasy[9] << 16)
                    + (pcmDataEasy[10] << 24);
            }
            if (pcmDataDirect.Count > 0)
            {
                foreach (byte[] dat in pcmDataDirect)
                {
                    if (dat != null && dat.Length > 0)
                    {
                        int size =
                            dat[7]
                            + (dat[8] << 8)
                            + (dat[9] << 16)
                            + (dat[10] << 24);
                        if (maxSize < size) maxSize = size;
                    }
                }
            }
            if (pcmDataEasy != null && pcmDataEasy.Length > 0)
            {
                pcmDataEasy[7] = (byte)maxSize;
                pcmDataEasy[8] = (byte)(maxSize >> 8);
                pcmDataEasy[9] = (byte)(maxSize >> 16);
                pcmDataEasy[10] = (byte)(maxSize >> 24);
            }
            if (pcmDataDirect.Count > 0)
            {
                foreach (byte[] dat in pcmDataDirect)
                {
                    if (dat != null && dat.Length > 0)
                    {
                        dat[7] = (byte)maxSize;
                        dat[8] = (byte)(maxSize >> 8);
                        dat[9] = (byte)(maxSize >> 16);
                        dat[10] = (byte)(maxSize >> 24);
                    }
                }
            }

            if (pcmDataEasy != null && pcmDataEasy.Length > 15)
                parent.OutData(mml,pcmDataEasy);

            if (pcmDataDirect.Count < 1) return;

            foreach (byte[] dat in pcmDataDirect)
            {
                if (dat != null && dat.Length > 0)
                    parent.OutData(mml,dat);
            }
        }

        public override void SetLfoAtKeyOn(partWork pw, MML mml)
        {
            base.SetLfoAtKeyOn(pw,mml);

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
                    if (pw.Type == enmChannelType.ADPCMB)
                        SetAdpcmBFNum(mml, pw);

                }

                if (pl.type == eLfoType.Tremolo)
                {
                    pw.beforeVolume = -1;

                    //if (pw.Type == enmChannelType.ADPCMA)
                    //SetAdpcmAVolume(pw);
                    if (pw.Type == enmChannelType.ADPCMB)
                        SetAdpcmBVolume(mml, pw);

                }

            }
        }

        public override void StorePcm(Dictionary<int, clsPcm> newDic, KeyValuePair<int, clsPcm> v, byte[] buf, bool is16bit, int samplerate, params object[] option)
        {
            clsPcmDataInfo pi = pcmDataInfo[v.Value.loopAdr == 0 ? 0 : 1];

            try
            {
                EncAdpcmA ea = new EncAdpcmA();
                buf = v.Value.loopAdr == 0
                    ? ea.YM_ADPCM_A_Encode(buf, is16bit)
                    : ea.YM_ADPCM_B_Encode(buf, is16bit, true);
                long size = buf.Length;
                byte[] newBuf = new byte[size];
                Array.Copy(buf, newBuf, size);
                buf = newBuf;
                long tSize = size;
                size = buf.Length;

                newDic.Add(
                    v.Key
                    , new clsPcm(
                        v.Value.num
                        , v.Value.seqNum, v.Value.chip
                        , v.Value.isSecondary
                        , v.Value.fileName
                        , v.Value.freq
                        , v.Value.vol
                        , pi.totalBufPtr
                        , pi.totalBufPtr + size - 1
                        , size
                        , v.Value.loopAdr == 0 ? 0 : 1
                        , is16bit
                        , samplerate)
                    );

                pi.totalBufPtr += size;
                newBuf = new byte[pi.totalBuf.Length + buf.Length];
                Array.Copy(pi.totalBuf, newBuf, pi.totalBuf.Length);
                Array.Copy(buf, 0, newBuf, pi.totalBuf.Length, buf.Length);

                pi.totalBuf = newBuf;
                Common.SetUInt32bit31(pi.totalBuf, 3, (UInt32)(pi.totalBuf.Length - 7), IsSecondary);
                Common.SetUInt32bit31(pi.totalBuf, 7, (UInt32)(pi.totalBuf.Length - 0xf));
                pi.use = true;
                pcmDataEasyA = pi.use ? pcmDataInfo[0].totalBuf : null;
                pcmDataEasyB = pi.use ? pcmDataInfo[1].totalBuf : null;
            }
            catch
            {
                pi.use = false;
            }

        }

        public override void StorePcmRawData(clsPcmDatSeq pds, byte[] buf, bool isRaw, bool is16bit, int samplerate, params object[] option)
        {
            if (!isRaw)
            {
                //Rawファイルは何もしない
                //Wavファイルはエンコ
                EncAdpcmA ea = new EncAdpcmA();
                buf = pds.DatLoopAdr == 0
                    ? ea.YM_ADPCM_A_Encode(buf, is16bit)
                    : ea.YM_ADPCM_B_Encode(buf, is16bit, true);
            }

            if (pds.DatLoopAdr == 0)
            {
                pcmDataDirectA.Add(Common.MakePCMDataBlock(dataType, pds, buf));
            }
            else
            {
                pcmDataDirectB.Add(Common.MakePCMDataBlock((byte)(dataType + 1), pds, buf));
            }

        }


        public override void CmdY(partWork pw, MML mml)
        {
            base.CmdY(pw, mml);

            if (mml.args[0] is string) return;

            byte adr = (byte)mml.args[0];
            byte dat = (byte)mml.args[1];

            if (pw.Type == enmChannelType.FMOPN || pw.Type == enmChannelType.FMOPNex)
                parent.OutData(mml, (pw.ch > 2 && pw.ch < 6) ? pw.port1 : pw.port0, adr, dat);
            else if (pw.Type == enmChannelType.SSG)
                parent.OutData(mml, pw.port0, adr, dat);
            else if (pw.Type == enmChannelType.ADPCMA)
                parent.OutData(mml, pw.port1, adr, dat);
            else if (pw.Type == enmChannelType.ADPCMB)
                parent.OutData(mml, pw.port0, adr, dat);
        }

        public override void CmdMPMS(partWork pw, MML mml)
        {
            if (pw.Type != enmChannelType.FMOPN)
            {
                msgBox.setWrnMsg(msg.get("E19000"), mml.line.Lp);
                return;
            }

            int n = (int)mml.args[1];
            n = Common.CheckRange(n, 0, 3);
            pw.pms = n;
            ((ClsOPN)pw.chip).OutOPNSetPanAMSPMS(
                mml,
                pw
                , (int)pw.pan.val
                , pw.ams
                , pw.pms);
        }

        public override void CmdMAMS(partWork pw, MML mml)
        {
            if (pw.Type != enmChannelType.FMOPN)
            {
                msgBox.setWrnMsg(msg.get("E19001"), mml.line.Lp);
                return;
            }

            int n = (int)mml.args[1];
            n = Common.CheckRange(n, 0, 7);
            pw.ams = n;
            ((ClsOPN)pw.chip).OutOPNSetPanAMSPMS(
                mml,
                pw
                , (int)pw.pan.val
                , pw.ams
                , pw.pms);
        }

        public override void CmdLfo(partWork pw, MML mml)
        {
            base.CmdLfo(pw, mml);

            int c = (char)mml.args[0] - 'P';
            if (pw.lfo[c].type == eLfoType.Hardware)
            {
                if (pw.Type == enmChannelType.FMOPN)
                {
                    if (pw.lfo[c].param.Count < 4)
                    {
                        msgBox.setErrMsg(msg.get("E19002"), mml.line.Lp);
                        return;
                    }
                    if (pw.lfo[c].param.Count > 5)
                    {
                        msgBox.setErrMsg(msg.get("E19003"), mml.line.Lp);
                        return;
                    }

                    pw.lfo[c].param[0] = Common.CheckRange(pw.lfo[c].param[0], 0, (int)parent.info.clockCount);//Delay(無視)
                    pw.lfo[c].param[1] = Common.CheckRange(pw.lfo[c].param[1], 0, 7);//Freq
                    pw.lfo[c].param[2] = Common.CheckRange(pw.lfo[c].param[2], 0, 7);//PMS
                    pw.lfo[c].param[3] = Common.CheckRange(pw.lfo[c].param[3], 0, 3);//AMS
                    if (pw.lfo[c].param.Count == 5)
                    {
                        pw.lfo[c].param[4] = Common.CheckRange(pw.lfo[c].param[4], 0, 1); //Switch
                    }
                    else
                    {
                        pw.lfo[c].param.Add(1);
                    }
                }
            }
        }

        public override void CmdLfoSwitch(partWork pw, MML mml)
        {
            base.CmdLfoSwitch(pw, mml);

            int c = (char)mml.args[0] - 'P';
            int n = (int)mml.args[1];
            if (pw.lfo[c].type == eLfoType.Hardware)
            {
                if (pw.Type == enmChannelType.FMOPN)
                {
                    if (pw.lfo[c].param[4] == 0)
                    {
                        ((ClsOPN)pw.chip).OutOPNSetHardLfo(mml, pw, (n == 0) ? false : true, pw.lfo[c].param[1]);
                    }
                    else
                    {
                        ((ClsOPN)pw.chip).OutOPNSetHardLfo(mml, pw, false, pw.lfo[c].param[1]);
                    }
                }
            }
        }

        public override void CmdTotalVolume(partWork pw, MML mml)
        {
            int n = (int)mml.args[0];
            ((YM2610B)pw.chip).adpcmA_TotalVolume 
                = Common.CheckRange(n, 0, ((YM2610B)pw.chip).adpcmA_MAXTotalVolume);
        }

        public override void CmdPan(partWork pw, MML mml)
        {
            int n = (int)mml.args[0];
            if (pw.Type == enmChannelType.FMOPN || pw.Type == enmChannelType.FMOPNex)
            {
                n = Common.CheckRange(n, 0, 3);
                pw.pan.val = n;
                ((ClsOPN)pw.chip).OutOPNSetPanAMSPMS(mml, pw, n, pw.ams, pw.fms);
            }
            else if (pw.Type == enmChannelType.ADPCMA)
            {
                n = Common.CheckRange(n, 0, 3);
                pw.pan.val = n;
            }
            else if (pw.Type == enmChannelType.ADPCMB)
            {
                n = Common.CheckRange(n, 0, 3);
                ((YM2610B)pw.chip).SetAdpcmBPan(mml, pw, n);
            }
        }

        public override void CmdInstrument(partWork pw, MML mml)
        {
            char type = (char)mml.args[0];
            int n = (int)mml.args[1];

            if (type == 'n' || type == 'N' || type == 'R' || type == 'A')
            {
                if (pw.Type == enmChannelType.FMOPNex)
                {
                    pw.instrument = n;
                    lstPartWork[2].instrument = n;
                    lstPartWork[6].instrument = n;
                    lstPartWork[7].instrument = n;
                    lstPartWork[8].instrument = n;
                    OutFmSetInstrument(pw,mml, n, pw.volume, type);
                    return;
                }
            }

            if (type == 'n')
            {
                if (pw.Type == enmChannelType.ADPCMA)
                {
                    n = Common.CheckRange(n, 0, 255);
                    if (!parent.instPCM.ContainsKey(n))
                    {
                        msgBox.setErrMsg(string.Format(msg.get("E19004"), n), mml.line.Lp);
                    }
                    else
                    {
                        if (parent.instPCM[n].chip != enmChipType.YM2610B 
                            || parent.instPCM[n].loopAdr != 0)
                        {
                            msgBox.setErrMsg(string.Format(msg.get("E19005"), n), mml.line.Lp);
                        }
                        pw.instrument = n;
                        SetADPCMAAddress(
                            mml,
                            pw
                            , (int)parent.instPCM[n].stAdr
                            , (int)parent.instPCM[n].edAdr);

                    }
                    return;
                }

                if (pw.Type == enmChannelType.ADPCMB)
                {
                    n = Common.CheckRange(n, 0, 255);
                    if (!parent.instPCM.ContainsKey(n))
                    {
                        msgBox.setErrMsg(string.Format(msg.get("E19004"), n), mml.line.Lp);
                    }
                    else
                    {
                        if (parent.instPCM[n].chip != enmChipType.YM2610B 
                            || parent.instPCM[n].loopAdr != 1)
                        {
                            msgBox.setErrMsg(string.Format(msg.get("E19005"), n), mml.line.Lp);
                        }
                        pw.instrument = n;
                        SetADPCMBAddress(
                            mml,
                            pw
                            , (int)parent.instPCM[n].stAdr
                            , (int)parent.instPCM[n].edAdr);
                    }
                    return;
                }

                if(pw.Type== enmChannelType.SSG)
                {
                    SetEnvelopParamFromInstrument(pw, n,mml);
                    return;
                }
            }

            base.CmdInstrument(pw, mml);
        }

        public override void MultiChannelCommand(MML mml)
        {
            if (!use) return;
            //コマンドを跨ぐデータ向け処理
            foreach (partWork pw in lstPartWork)
            {
                if (pw.Type == enmChannelType.ADPCMA)
                {
                    //Adpcm-A TotalVolume処理
                    if (pw.beforeVolume != pw.volume || !pw.pan.eq())
                    {
                        parent.OutData(mml, pw.port1, (byte)(0x08 + (pw.ch - 12)), (byte)((byte)((pw.pan.val & 0x3) << 6) | (byte)(pw.volume & 0x1f)));
                        pw.beforeVolume = pw.volume;
                        pw.pan.rst();
                    }

                    adpcmA_KeyOn |= (byte)(pw.keyOn ? (1 << (pw.ch - 12)) : 0);
                    pw.keyOn = false;
                    adpcmA_KeyOff |= (byte)(pw.keyOff ? (1 << (pw.ch - 12)) : 0);
                    pw.keyOff = false;
                }
            }

            //Adpcm-A KeyOff処理
            if (0 != adpcmA_KeyOff)
            {
                byte data = (byte)(0x80 + adpcmA_KeyOff);
                parent.OutData(mml, lstPartWork[0].port1, 0x00, data);
                adpcmA_KeyOff = 0;
            }

            //Adpcm-A TotalVolume処理
            if (adpcmA_beforeTotalVolume != adpcmA_TotalVolume)
            {
                parent.OutData(mml, lstPartWork[0].port1, 0x01, (byte)(adpcmA_TotalVolume & 0x3f));
                adpcmA_beforeTotalVolume = adpcmA_TotalVolume;
            }

            //Adpcm-A KeyOn処理
            if (0 != adpcmA_KeyOn)
            {
                byte data = (byte)(0x00 + adpcmA_KeyOn);
                parent.OutData(mml, lstPartWork[0].port1, 0x00, data);
                adpcmA_KeyOn = 0;
            }
        }


        public override string DispRegion(clsPcm pcm)
        {
            return string.Format("{0,-10} {1,-7} {2,-5:D3} N/A  ${3,-7:X6} ${4,-7:X6} N/A      ${5,-7:X6}  NONE {6}\r\n"
                , Name + (pcm.loopAdr == 0 ? "_A" : "_B")
                , pcm.isSecondary ? "SEC" : "PRI"
                , pcm.num
                , pcm.stAdr & 0xffffff
                , pcm.edAdr & 0xffffff
                , pcm.size
                , pcm.status.ToString()
                );
        }

    }
}

