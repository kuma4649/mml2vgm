using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class YM2608 : ClsOPN
    {
        protected int[][] _FNumTbl = new int[2][] {
            new int[13]
            ,new int[96]
            //new int[] {
            //// OPNA(FM) : TP = (144 * ftone * (2^20) / M) / (2^(B-1))       32:Divider 2:OPNA 
            ////   c    c+     d    d+     e     f    f+     g    g+     a    a+     b    >c
            // 0x289,0x2af,0x2d8,0x303,0x331,0x362,0x395,0x3cc,0x405,0x443,0x484,0x4c8,0x289*2
            //},
            //new int[] {
            //// OPNA(SSG) : TP = M / (ftone * 32 * 2)       32:Divider 2:OPNA 
            ////   c    c+     d    d+     e     f    f+     g    g+     a    a+     b
            // 0xEE8,0xE12,0xD48,0xC89,0xBD5,0xB2B,0xA8A,0x9F3,0x964,0x8DD,0x85E,0x7E6
            //,0x774,0x709,0x6A4,0x645,0x5EA,0x595,0x545,0x4FA,0x4B2,0x46F,0x42F,0x3F3
            //,0x3BA,0x384,0x352,0x322,0x2F5,0x2CB,0x2A3,0x27D,0x259,0x237,0x217,0x1F9
            //,0x1DD,0x1C2,0x1A9,0x191,0x17B,0x165,0x151,0x13E,0x12D,0x11C,0x10C,0x0FD
            //,0x0EF,0x0E1,0x0D4,0x0C9,0x0BD,0x0B3,0x0A9,0x09F,0x096,0x08E,0x086,0x07E
            //,0x077,0x071,0x06A,0x064,0x05F,0x059,0x054,0x050,0x04B,0x047,0x043,0x03F
            //,0x03C,0x038,0x035,0x032,0x02F,0x02D,0x02A,0x028,0x026,0x023,0x021,0x020
            //,0x01E,0x01C,0x01B,0x019,0x018,0x016,0x015,0x014,0x013,0x012,0x011,0x010
            //}
        };

        public int rhythm_TotalVolume = 63;
        public int rhythm_beforeTotalVolume = -1;
        public int rhythm_MAXTotalVolume = 63;
        public byte rhythm_KeyOn = 0;
        public byte rhythm_KeyOff = 0;

        public YM2608(ClsVgm parent, int chipID, string initialPartName, string stPath, int isSecondary) : base(parent, chipID, initialPartName, stPath, isSecondary)
        {
            _chipType = enmChipType.YM2608;
            _Name = "YM2608";
            _ShortName = "OPNA";
            _ChMax = 19;
            _canUsePcm = true;
            _canUsePI = true;
            FNumTbl = _FNumTbl;
            IsSecondary = isSecondary;
            dataType = 0x81;
            Frequency = 7987200;
            port = new byte[][]{
                new byte[] { (byte)(isSecondary!=0 ? 0xa6 : 0x56) }
                , new byte[] { (byte)(isSecondary!=0 ? 0xa7 : 0x57) }
            };

            if (string.IsNullOrEmpty(initialPartName)) return;

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

            Ch[12].Type = enmChannelType.RHYTHM;
            Ch[13].Type = enmChannelType.RHYTHM;
            Ch[14].Type = enmChannelType.RHYTHM;
            Ch[15].Type = enmChannelType.RHYTHM;
            Ch[16].Type = enmChannelType.RHYTHM;
            Ch[17].Type = enmChannelType.RHYTHM;

            Ch[18].Type = enmChannelType.ADPCM;

            pcmDataInfo = new clsPcmDataInfo[] { new clsPcmDataInfo()};
            pcmDataInfo[0].totalBufPtr = 0L;
            pcmDataInfo[0].use = false;
            if (parent.info.format == enmFormat.ZGM)
            {
                if (parent.ChipCommandSize == 2)
                {
                    if (isSecondary==0)
                        pcmDataInfo[0].totalBuf = new byte[] { 0x07,0x00, 0x66, 0x81, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                    else
                        pcmDataInfo[0].totalBuf = new byte[] { 0x07,0x00, 0x66, 0x81, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                }
                else
                {
                    if (isSecondary==0)
                        pcmDataInfo[0].totalBuf = new byte[] { 0x07, 0x66, 0x81, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                    else
                        pcmDataInfo[0].totalBuf = new byte[] { 0x07, 0x66, 0x81, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                }
            }
            else
            {
                if (isSecondary==0)
                    pcmDataInfo[0].totalBuf = new byte[] { 0x67, 0x66, 0x81, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                else
                    pcmDataInfo[0].totalBuf = new byte[] { 0x67, 0x66, 0x81, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            }

            pcmDataInfo[0].totalHeaderLength = pcmDataInfo[0].totalBuf.Length;
            pcmDataInfo[0].totalHeadrSizeOfDataPtr = (parent.ChipCommandSize == 2) ? 4 : 3;

            Envelope = new Function();
            Envelope.Max = 255;
            Envelope.Min = 0;

        }

        public override void InitChip()
        {
            if (!use) return;

            //initialize shared param
            OutOPNSetHardLfo(null, lstPartWork[0], false, 0);
            OutOPNSetCh3SpecialMode(null, lstPartWork[0], false);

            //FM Off
            OutFmAllKeyOff();

            //SSG Off
            for (int ch = 9; ch < 12; ch++)
            {
                OutSsgKeyOff(null, lstPartWork[ch]);
                lstPartWork[ch].volume = 0;
            }

            //Use OPNA mode
            parent.OutData((MML)null, port[0], 0x29, 0x82);
            parent.OutData((MML)null, port[1], 0x29, 0x82);
            //ADPCM Reset
            parent.OutData((MML)null, port[1], 0x10, 0x17);
            parent.OutData((MML)null, port[1], 0x10, 0x80);
            parent.OutData((MML)null, port[1], 0x00, 0x80);
            parent.OutData((MML)null, port[1], 0x01, 0xc0);
            parent.OutData((MML)null, port[1], 0x00, 0x01);

            foreach (partWork pw in lstPartWork)
            {
                if (pw.ch == 0)
                {
                    pw.hardLfoSw = false;
                    pw.hardLfoNum = 0;
                    OutOPNSetHardLfo(null, pw, pw.hardLfoSw, pw.hardLfoNum);
                }

                if (pw.ch < 6)
                {
                    pw.pan.val = 3;
                    pw.ams = 0;
                    pw.fms = 0;
                    if (!pw.dataEnd) OutOPNSetPanAMSPMS(null, pw, 3, 0, 0);
                }
            }

            if (ChipID != 0 && parent.info.format != enmFormat.ZGM)
            {
                parent.dat[0x4b] = new outDatum(enmMMLType.unknown, null, null, (byte)(parent.dat[0x4b].val | 0x40));//use Secondary
            }
        }

        public override void InitPart(partWork pw)
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
            else if (pw.Type == enmChannelType.RHYTHM)
            {
                //pw.volume = 32767;
                pw.MaxVolume = 31;//5bit
                pw.volume = pw.MaxVolume;
            }
            else if (pw.Type == enmChannelType.ADPCM)
            {
                //pw.volume = 32767;
                pw.MaxVolume = 255;
                pw.volume = pw.MaxVolume;
            }

            pw.port = port;
        }


        public void SetADPCMAddress(MML mml, partWork pw, int startAdr, int endAdr)
        {
            if (pw.pcmStartAddress != startAdr)
            {
                parent.OutData(mml, port[1], 0x02, (byte)((startAdr >> 2) & 0xff));
                parent.OutData(mml, port[1], 0x03, (byte)((startAdr >> 10) & 0xff));
                pw.pcmStartAddress = startAdr;
            }

            if (pw.pcmEndAddress != endAdr)
            {
                parent.OutData(mml, port[1], 0x04, (byte)(((endAdr - 0x04) >> 2) & 0xff));
                parent.OutData(mml, port[1], 0x05, (byte)(((endAdr - 0x04) >> 10) & 0xff));
                pw.pcmEndAddress = endAdr;
            }

        }

        public void SetAdpcmFNum(MML mml,partWork pw)
        {
            int f = GetAdpcmFNum(pw.octaveNow, pw.noteCmd, pw.shift + pw.keyShift);//
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
            parent.OutData(mml,port[1], 0x09, data);

            data = (byte)((f & 0xff00) >> 8);
            parent.OutData(mml,port[1], 0x0a, data);
        }

        public void SetAdpcmVolume(MML mml,partWork pw)
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
                parent.OutData(mml,port[1], 0x0b, (byte)vol);
                pw.beforeVolume = pw.volume;
            }
        }

        public void SetAdpcmPan(MML mml,partWork pw, int pan)
        {
            if (pw.pan.val != pan)
            {
                parent.OutData(mml,port[1], 0x01, (byte)((pan & 0x3) << 6));
                pw.pan.val = pan;
            }
        }

        public int GetAdpcmFNum(int octave, char noteCmd, int shift)
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

        public void OutAdpcmKeyOn(MML mml,partWork pw)
        {

            SetAdpcmVolume(mml,pw);
            parent.OutData(mml,port[1], 0x00, 0xa0);

        }

        public void OutAdpcmKeyOff(MML mml,partWork pw)
        {

            parent.OutData(mml,port[1], 0x00, 0x01);

        }


        public override void SetFNum(partWork pw, MML mml)
        {
            if (pw.ch < 9)
                SetFmFNum(pw,mml);
            else if (pw.Type == enmChannelType.SSG)
            {
                SetSsgFNum(pw,mml);
            }
            else if (pw.Type == enmChannelType.RHYTHM)
            {
            }
            else if (pw.Type == enmChannelType.ADPCM)
            {
                SetAdpcmFNum(mml,pw);
            }
        }

        public override void SetKeyOn(partWork pw, MML mml)
        {
            if (pw.ch < 9)
                OutFmKeyOn(pw,mml);
            else if (pw.Type == enmChannelType.SSG)
            {
                OutSsgKeyOn(pw,mml);
            }
            else if (pw.Type == enmChannelType.RHYTHM)
            {
                pw.keyOn = true;
                pw.keyOff = false;
                SetDummyData(pw, mml);
            }
            else if (pw.Type == enmChannelType.ADPCM)
            {
                OutAdpcmKeyOn(mml,pw);
            }
        }

        public override void SetKeyOff(partWork pw, MML mml)
        {
            if (pw.ch < 9)
                OutFmKeyOff(pw,mml);
            else if (pw.Type == enmChannelType.SSG)
            {
                OutSsgKeyOff(mml,pw);
            }
            else if (pw.Type == enmChannelType.RHYTHM)
            {
                pw.keyOn = false;
                pw.keyOff = true;
            }
            else if (pw.Type == enmChannelType.ADPCM)
            {
                OutAdpcmKeyOff(mml,pw);
            }
        }

        public override void SetVolume(partWork pw, MML mml)
        {
            base.SetVolume(pw,mml);

            if (pw.Type == enmChannelType.RHYTHM)
            {
            }
            else if (pw.Type == enmChannelType.ADPCM)
            {
                SetAdpcmVolume(mml,pw);
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
                    if (pw.Type == enmChannelType.ADPCM)
                        SetAdpcmFNum(mml,pw);

                }

                if (pl.type == eLfoType.Tremolo)
                {
                    pw.beforeVolume = -1;

                    //if (pw.Type == enmChannelType.RHYTHM)
                    //SetRhythmVolume(pw);
                    if (pw.Type == enmChannelType.ADPCM)
                        SetAdpcmVolume(mml,pw);

                }

            }
        }

        public override void StorePcm(Dictionary<int, clsPcm> newDic, KeyValuePair<int, clsPcm> v, byte[] buf, bool is16bit, int samplerate, params object[] option)
        {
            clsPcmDataInfo pi = pcmDataInfo[0];

            try
            {
                EncAdpcmA ea = new EncAdpcmA();
                buf = ea.YM_ADPCM_B_Encode(buf, is16bit, false);
                long size = buf.Length;

                byte[] newBuf;
                newBuf = new byte[size];
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
                        , -1
                        , is16bit
                        , samplerate)
                    );

                pi.totalBufPtr += size;
                newBuf = new byte[pi.totalBuf.Length + buf.Length];
                Array.Copy(pi.totalBuf, newBuf, pi.totalBuf.Length);
                Array.Copy(buf, 0, newBuf, pi.totalBuf.Length, buf.Length);

                pi.totalBuf = newBuf;
                pi.use = true;
                Common.SetUInt32bit31(
                    pi.totalBuf
                    , pi.totalHeadrSizeOfDataPtr
                    , (UInt32)(pi.totalBuf.Length - (pi.totalHeadrSizeOfDataPtr + 4))
                    , IsSecondary!=0
                    );
                Common.SetUInt32bit31(
                    pi.totalBuf
                    , pi.totalHeadrSizeOfDataPtr + 4
                    , (UInt32)(pi.totalBuf.Length - (pi.totalHeadrSizeOfDataPtr + 4 + 4 + 4))
                    );
                pcmDataEasy = pi.use ? pi.totalBuf : null;

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
                buf = ea.YM_ADPCM_B_Encode(buf, is16bit, true);
            }

            pcmDataDirect.Add(Common.MakePCMDataBlock((byte)dataType, pds, buf));

        }


        public override void CmdY(partWork pw, MML mml)
        {
            base.CmdY(pw, mml);

            if (mml.args[0] is string) return;

            byte adr = (byte)mml.args[0];
            byte dat = (byte)mml.args[1];

            if (pw.Type == enmChannelType.FMOPN || pw.Type == enmChannelType.FMOPNex)
                parent.OutData(mml, (pw.ch > 2 && pw.ch < 6) ? port[1] : port[0], adr, dat);
            else if (pw.Type == enmChannelType.SSG)
                parent.OutData(mml, port[0], adr, dat);
            else if (pw.Type == enmChannelType.RHYTHM)
                parent.OutData(mml, port[0], adr, dat);
            else if (pw.Type == enmChannelType.ADPCM)
                parent.OutData(mml, port[1], adr, dat);
        }

        public override void CmdMPMS(partWork pw, MML mml)
        {
            if (pw.Type != enmChannelType.FMOPN)
            {
                msgBox.setWrnMsg(msg.get("E18000"), mml.line.Lp);
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
                msgBox.setWrnMsg(msg.get("E18001"), mml.line.Lp);
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
                        msgBox.setErrMsg(msg.get("E18002"), mml.line.Lp);
                        return;
                    }
                    if (pw.lfo[c].param.Count > 5)
                    {
                        msgBox.setErrMsg(msg.get("E18003"), mml.line.Lp);
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
            ((YM2608)pw.chip).rhythm_TotalVolume 
                = Common.CheckRange(n, 0, ((YM2608)pw.chip).rhythm_MAXTotalVolume);
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
            else if (pw.Type == enmChannelType.RHYTHM)
            {
                n = Common.CheckRange(n, 0, 3);
                pw.pan.val = n;
            }
            else if (pw.Type == enmChannelType.ADPCM)
            {
                n = Common.CheckRange(n, 0, 3);
                ((YM2608)pw.chip).SetAdpcmPan(mml, pw, n);
            }
            SetDummyData(pw, mml);
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
                if (pw.Type == enmChannelType.ADPCM)
                {
                    n = Common.CheckRange(n, 0, 255);
                    if (!parent.instPCM.ContainsKey(n))
                    {
                        msgBox.setErrMsg(string.Format(msg.get("E18004"), n), mml.line.Lp);
                    }
                    else
                    {
                        if (parent.instPCM[n].chip != enmChipType.YM2608)
                        {
                            msgBox.setErrMsg(string.Format(msg.get("E18005"), n), mml.line.Lp);
                        }
                        pw.instrument = n;
                        SetADPCMAddress(
                            mml,
                            pw
                            , (int)parent.instPCM[n].stAdr
                            , (int)parent.instPCM[n].edAdr);
                    }
                    return;
                }

                if (pw.Type == enmChannelType.SSG)
                {
                    SetEnvelopParamFromInstrument(pw, n, mml);
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
                if (pw.Type == enmChannelType.RHYTHM)
                {
                    //Rhythm Volume処理
                    if (pw.beforeVolume != pw.volume || !pw.pan.eq())
                    {
                        parent.OutData(mml,port[0], (byte)(0x18 + (pw.ch - 12)), (byte)((byte)((pw.pan.val & 0x3) << 6) | (byte)(pw.volume & 0x1f)));
                        pw.beforeVolume = pw.volume;
                        pw.pan.rst();
                    }

                    rhythm_KeyOn |= (byte)(pw.keyOn ? (1 << (pw.ch - 12)) : 0);
                    pw.keyOn = false;
                    rhythm_KeyOff |= (byte)(pw.keyOff ? (1 << (pw.ch - 12)) : 0);
                    pw.keyOff = false;
                }
            }

            //Rhythm KeyOff処理
            if (0 != rhythm_KeyOff)
            {
                byte data = (byte)(0x80 + rhythm_KeyOff);
                parent.OutData(mml,port[0], 0x10, data);
                rhythm_KeyOff = 0;
            }

            //Rhythm TotalVolume処理
            if (rhythm_beforeTotalVolume != rhythm_TotalVolume)
            {
                parent.OutData(mml,port[0], 0x11, (byte)(rhythm_TotalVolume & 0x3f));
                rhythm_beforeTotalVolume = rhythm_TotalVolume;
            }

            //Rhythm KeyOn処理
            if (0 != rhythm_KeyOn)
            {
                byte data = (byte)(0x00 + rhythm_KeyOn);
                parent.OutData(mml,port[0], 0x10, data);
                rhythm_KeyOn = 0;
            }
        }

        public override string DispRegion(clsPcm pcm)
        {
            return string.Format("{0,-10} {1,-7} {2,-5:D3} N/A  ${3,-7:X6} ${4,-7:X6} N/A      ${5,-7:X6}  NONE {6}\r\n"
                , Name
                , pcm.isSecondary!=0 ? "SEC" : "PRI"
                , pcm.num
                , pcm.stAdr & 0xffffff
                , pcm.edAdr & 0xffffff
                , pcm.size
                , pcm.status.ToString()
                );
        }
    }
}
