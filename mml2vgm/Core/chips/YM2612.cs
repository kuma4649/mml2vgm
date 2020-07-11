using musicDriverInterface;
using System;
using System.Collections.Generic;

namespace Core
{
    public class YM2612 : ClsOPN
    {
        protected int[][] _FNumTbl = new int[1][] {
            new int[13]
            //{
            ////   c    c+     d    d+     e     f    f+     g    g+     a    a+     b    >c
            // 0x289,0x2af,0x2d8,0x303,0x331,0x362,0x395,0x3cc,0x405,0x443,0x484,0x4c8,0x289*2
            //}
        };
        public const string FMF_NUM = "FMF-NUM";


        public YM2612(ClsVgm parent, int chipID, string initialPartName, string stPath, int chipNumber) : base(parent, chipID, initialPartName, stPath, chipNumber)
        {
            _chipType = enmChipType.YM2612;
            _Name = "YM2612";
            _ShortName = "OPN2";
            _ChMax = 9;
            _canUsePcm = true;
            _canUsePI = false;
            FNumTbl = _FNumTbl;
            ChipNumber = chipNumber;

            Frequency = 7670454;
            port = new byte[][]{
                new byte[] { (byte)(0x2 | (chipNumber!=0 ? 0xa0 : 0x50)) }
                , new byte[] { (byte)(0x3 | (chipNumber!=0 ? 0xa0 : 0x50)) }
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
            }

            Ch = new ClsChannel[ChMax];
            SetPartToCh(Ch, initialPartName);
            foreach (ClsChannel ch in Ch)
            {
                ch.Type = enmChannelType.FMOPN;
                ch.chipNumber = chipID == 1;
            }

            Ch[2].Type = enmChannelType.FMOPNex;
            Ch[5].Type = enmChannelType.FMPCM;
            Ch[6].Type = enmChannelType.FMOPNex;
            Ch[7].Type = enmChannelType.FMOPNex;
            Ch[8].Type = enmChannelType.FMOPNex;

            pcmDataInfo = new clsPcmDataInfo[] { new clsPcmDataInfo() };
            pcmDataInfo[0].totalBufPtr = 0L;
            pcmDataInfo[0].use = false;
            if (parent.info.format == enmFormat.ZGM)
            {
                if (parent.ChipCommandSize == 2)
                {
                    pcmDataInfo[0].totalBuf = new byte[] {
                        0x07 , 0x00 ,          // data block
                        0x00 , 0x00 ,          // Chip Command Number
                        0x00 ,                 // data type
                        0x00, 0x00, 0x00, 0x00 // size of data
                    };
                }
                else
                {
                    pcmDataInfo[0].totalBuf = new byte[] {
                        0x07 ,                 // data block
                        0x00 ,                 // Chip Command Number
                        0x00 ,                 // data type
                        0x00, 0x00, 0x00, 0x00 // size of data
                    };
                }
            }
            else
            {
                pcmDataInfo[0].totalBuf = new byte[] { 0x67, 0x66, 0x00, 0x00, 0x00, 0x00, 0x00 };
            }

            pcmDataInfo[0].totalHeaderLength = pcmDataInfo[0].totalBuf.Length;
            pcmDataInfo[0].totalHeadrSizeOfDataPtr = (parent.ChipCommandSize == 2) ? 5 : 3;

        }

        public override void InitPart(partWork pw)
        {
            foreach (partPage page in pw.pg)
            {
                page.slots = (byte)(((page.Type == enmChannelType.FMOPN || page.Type == enmChannelType.FMPCM) || page.ch == 2) ? 0xf : 0x0);
                page.volume = 127;
                page.MaxVolume = 127;
                page.port = port;
            }
        }

        public override void InitChip()
        {
            if (!use) return;

            OutOPNSetHardLfo(null, lstPartWork[0].cpg, false, 0);
            OutOPNSetCh3SpecialMode(null, lstPartWork[0].cpg, false);
            OutSetCh6PCMMode(null, lstPartWork[0].cpg, false);

            OutFmAllKeyOff();

            foreach (partWork pw in lstPartWork)
            {
                foreach (partPage page in pw.pg)
                {
                    if (page.ch == 0)
                    {
                        page.hardLfoSw = false;
                        page.hardLfoNum = 0;
                    }

                    if (page.ch < 6)
                    {
                        page.pan = 3;
                        page.ams = 0;
                        page.fms = 0;
                        if (!page.dataEnd) OutOPNSetPanAMSPMS(null, page, 3, 0, 0);
                    }
                }
            }

            if (ChipID != 0 && parent.info.format != enmFormat.ZGM)
            {
                parent.dat[0x2f] = new outDatum(enmMMLType.unknown, null, null, (byte)(parent.dat[0x2f].val | 0x40));
            }

        }


        public void OutSetCh6PCMMode(MML mml, partPage page, bool sw)
        {
            SOutData(
                page,
                mml,
                port[0]
                , 0x2b
                , (byte)((sw ? 0x80 : 0))
                );
        }

        public void GetPcmNote(partPage page)
        {
            int shift = page.shift + page.keyShift + page.arpDelta;
            int o = page.octaveNow;//
            int n = Const.NOTE.IndexOf(page.noteCmd) + shift;
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

            page.pcmOctave = o;
            page.pcmNote = n;
        }


        public override void SetFNum(partPage page, MML mml)
        {
            SetFmFNum(page, mml);
        }

        public override void SetKeyOn(partPage page, MML mml)
        {
            SetDummyData(page, mml);
            page.keyOn = true;
        }

        public override void SetKeyOff(partPage page, MML mml)
        {
            OutFmKeyOff(page, mml);
        }

        public override void StorePcm(Dictionary<int, clsPcm> newDic, KeyValuePair<int, clsPcm> v, byte[] buf, bool is16bit, int samplerate, params object[] option)
        {
            clsPcmDataInfo pi = pcmDataInfo[0];

            try
            {
                long size = buf.Length;
                if (newDic.ContainsKey(v.Key))
                {
                    newDic.Remove(v.Key);
                }
                newDic.Add(
                    v.Key
                    , new clsPcm(
                        v.Value.num
                        , v.Value.seqNum
                        , v.Value.chip
                        , 0
                        , v.Value.fileName
                        , v.Value.freq != -1 ? v.Value.freq : samplerate
                        , v.Value.vol
                        , pi.totalBufPtr
                        , pi.totalBufPtr + size - 1
                        , size
                        , -1
                        , is16bit
                        , samplerate));
                pi.totalBufPtr += size;

                byte[] newBuf = new byte[pi.totalBuf.Length + buf.Length];
                Array.Copy(pi.totalBuf, newBuf, pi.totalBuf.Length);
                Array.Copy(buf, 0, newBuf, pi.totalBuf.Length, buf.Length);

                pi.totalBuf = newBuf;
                Common.SetUInt32bit31(
                    pi.totalBuf
                    , pi.totalHeadrSizeOfDataPtr
                    , (UInt32)(pi.totalBuf.Length - (pi.totalHeadrSizeOfDataPtr + 4))
                    , ChipNumber != 0);
                pi.use = true;
                pcmDataEasy = pi.use ? pi.totalBuf : null;
            }
            catch
            {
                pi.use = false;
            }

        }

        public override void StorePcmRawData(clsPcmDatSeq pds, byte[] buf, bool isRaw, bool is16bit, int samplerate, params object[] option)
        {
            msgBox.setWrnMsg(msg.get("E20004"), new LinePos("-"));
        }


        public override void CmdY(partPage page, MML mml)
        {
            base.CmdY(page, mml);

            if (mml.args[0] is string) return;

            byte adr = (byte)(int)mml.args[0];
            byte dat = (byte)(int)mml.args[1];
            SOutData(page, mml, (page.ch > 2 && page.ch < 6) ? port[1] : port[0], adr, dat);
        }

        public override void CmdMPMS(partPage page, MML mml)
        {

            int n = (int)mml.args[1];
            n = Common.CheckRange(n, 0, 7);
            page.pms = n;
            ((ClsOPN)page.chip).OutOPNSetPanAMSPMS(
                mml,
                page
                , page.pan
                , page.ams
                , page.pms);
        }

        public override void CmdMAMS(partPage page, MML mml)
        {

            int n = (int)mml.args[1];
            n = Common.CheckRange(n, 0, 3);
            page.ams = n;
            ((ClsOPN)page.chip).OutOPNSetPanAMSPMS(
                mml,
                page
                , page.pan
                , page.ams
                , page.pms);
        }

        public override void CmdLfo(partPage page, MML mml)
        {
            base.CmdLfo(page, mml);

            if (mml.args[0] is string)
            {
                return;
            }

            int c = (char)mml.args[0] - 'P';
            if (page.lfo[c].type == eLfoType.Hardware)
            {
                if (page.lfo[c].param.Count < 4)
                {
                    msgBox.setErrMsg(msg.get("E20000"), mml.line.Lp);
                    return;
                }
                if (page.lfo[c].param.Count > 5)
                {
                    msgBox.setErrMsg(msg.get("E20001"), mml.line.Lp);
                    return;
                }

                page.lfo[c].param[0] = Common.CheckRange(page.lfo[c].param[0], 0, (int)parent.info.clockCount);//Delay(無視)
                page.lfo[c].param[1] = Common.CheckRange(page.lfo[c].param[1], 0, 7);//Freq
                page.lfo[c].param[2] = Common.CheckRange(page.lfo[c].param[2], 0, 7);//PMS
                page.lfo[c].param[3] = Common.CheckRange(page.lfo[c].param[3], 0, 3);//AMS
                if (page.lfo[c].param.Count == 5)
                {
                    page.lfo[c].param[4] = Common.CheckRange(page.lfo[c].param[4], 0, 1); //Switch
                }
                else
                {
                    page.lfo[c].param.Add(1);
                }
            }
        }

        public override void CmdLfoSwitch(partPage page, MML mml)
        {
            base.CmdLfoSwitch(page, mml);

            int c = (char)mml.args[0] - 'P';
            int n = (int)mml.args[1];
            if (page.lfo[c].type == eLfoType.Hardware)
            {
                if (page.lfo[c].param[4] == 0)
                {
                    page.fms = (n == 0) ? 0 : page.lfo[c].param[2];
                    page.ams = (n == 0) ? 0 : page.lfo[c].param[3];
                    ((ClsOPN)page.chip).OutOPNSetPanAMSPMS(mml, page, page.pan, page.ams, page.fms);
                    page.chip.lstPartWork[0].cpg.hardLfoSw = (n != 0);
                    page.chip.lstPartWork[0].cpg.hardLfoNum = page.lfo[c].param[1];
                    ((ClsOPN)page.chip).OutOPNSetHardLfo(null, page, page.chip.lstPartWork[0].cpg.hardLfoSw, page.chip.lstPartWork[0].cpg.hardLfoNum);
                }
                else
                {
                    ((ClsOPN)page.chip).OutOPNSetHardLfo(mml, page, false, page.lfo[c].param[1]);
                }
            }
        }

        public override void CmdPan(partPage page, MML mml)
        {
            int n = (int)mml.args[0];

            //強制的にモノラルにする
            if (parent.info.monoPart != null
                && parent.info.monoPart.Contains(Ch[5].Name))
            {
                n = 3;
            }

            n = Common.CheckRange(n, 0, 3);
            page.pan = n;
            ((ClsOPN)page.chip).OutOPNSetPanAMSPMS(mml, page, n, page.ams, page.fms);
        }

        public override void CmdMode(partPage page, MML mml)
        {
            int n = (int)mml.args[0];
            if (page.Type == enmChannelType.FMPCM)
            {
                n = Common.CheckRange(n, 0, 1);
                page.pcm = (n == 1);
                page.freq = -1;//freqをリセット
                page.instrument = -1;
                //((YM2612)(page.chip)).OutSetCh6PCMMode(mml, page, page.pcm);

                return;
            }

            base.CmdMode(page, mml);

        }

        public override void CmdPcmMapSw(partPage page, MML mml)
        {
            bool sw = (bool)mml.args[0];
            if (page.Type == enmChannelType.FMPCM)
            {
                page.isPcmMap = sw;
            }
        }

        public override void CmdInstrument(partPage page, MML mml)
        {
            char type = (char)mml.args[0];
            int n = (int)mml.args[1];

            if (type == 'n' || type == 'N' || type == 'R' || type == 'A')
            {
                if (page.Type == enmChannelType.FMOPNex)
                {
                    page.instrument = n;
                    lstPartWork[2].cpg.instrument = n;
                    lstPartWork[6].cpg.instrument = n;
                    lstPartWork[7].cpg.instrument = n;
                    lstPartWork[8].cpg.instrument = n;
                    OutFmSetInstrument(page, mml, n, page.volume, type);
                    return;
                }
            }

            if (type == 'n')
            {
                if (page.pcm)
                {
                    if (page.isPcmMap)
                    {
                        page.pcmMapNo = n;
                        if (!parent.instPCMMap.ContainsKey(n))
                        {
                            msgBox.setErrMsg(string.Format(msg.get("E10024"), n), mml.line.Lp);
                        }
                        return;
                    }

                    page.instrument = n;
                    if (!parent.instPCM.ContainsKey(n))
                    {
                        msgBox.setErrMsg(string.Format(msg.get("E20002"), n), mml.line.Lp);
                    }
                    else
                    {
                        if (parent.instPCM[n].chip != enmChipType.YM2612)
                        {
                            msgBox.setErrMsg(string.Format(msg.get("E20003"), n), mml.line.Lp);
                        }
                    }
                    return;
                }

            }

            base.CmdInstrument(page, mml);
        }

        public override void SetLfoAtKeyOn(partPage page, MML mml)
        {
            for (int lfo = 0; lfo < 4; lfo++)
            {
                clsLfo pl = page.lfo[lfo];
                if (!pl.sw)
                    continue;
                if (pl.type == eLfoType.Hardware)
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
                    SetFmFNum(page, mml);

                }

                if (pl.type == eLfoType.Tremolo)
                {
                    page.beforeVolume = -1;
                    SetFmVolume(page, mml);

                }

            }
        }


        public override string DispRegion(clsPcm pcm)
        {
            return string.Format("{0,-10} {1,-7} {2,-5:D3} N/A  ${3,-7:X6} ${4,-7:X6} N/A      ${5,-7:X6}  NONE {6}\r\n"
                , Name
                , pcm.chipNumber != 0 ? "SEC" : "PRI"
                , pcm.num
                , pcm.stAdr & 0xffffff
                , pcm.edAdr & 0xffffff
                , pcm.size
                , pcm.status.ToString()
                );
        }


        public override void SetupPageData(partWork pw, partPage page)
        {
            if (page.Type == enmChannelType.FMOPN
                || page.Type == enmChannelType.FMOPNex //効果音モード対応チャンネル
                || (page.Type == enmChannelType.FMPCM && !page.pcm) //OPN2PCMチャンネル
                )
            {
                if (page.Type == enmChannelType.FMPCM)
                {
                    if (page.spg.pcm)
                    {
                        OutSetCh6PCMMode(null, page, false);
                        page.spg.pcm = false;
                    }
                }

                OutFmKeyOff(page, null);
                //音色
                page.spg.instrument = -1;
                OutFmSetInstrument(page, null, page.instrument, page.volume, 'n');

                //周波数
                page.spg.freq = -1;
                SetFNum(page, null);

                //音量
                page.spg.beforeVolume = -1;
                SetVolume(page, null);

                //パン
                ((ClsOPN)page.chip).OutOPNSetPanAMSPMS(null, page, page.pan, page.ams, page.fms);

            }
            else if (page.Type == enmChannelType.FMPCM && page.pcm)
            {
                if (page.Type == enmChannelType.FMPCM)
                {
                    if (page.spg.pcm)
                    {
                        OutSetCh6PCMMode(null, page, true);
                        page.spg.pcm = true;
                    }
                }

                OutFmKeyOff(page, null);
                //音色 
                page.spg.instrument = -1;
                //周波数
                //音量
                //パン
                ((ClsOPN)page.chip).OutOPNSetPanAMSPMS(null, page, page.pan, page.ams, page.fms);
            }
        }

        public override void MultiChannelCommand(MML mml)
        {
            if (!use) return;
            //int dat = 0;

            foreach (partWork pw in lstPartWork)
            {
                foreach (partPage page in pw.pg)
                {
                    if (page.Type == enmChannelType.FMPCM)
                    {
                        if (page.spg.pcm != page.pcm)
                        {
                            OutSetCh6PCMMode(null, page, page.pcm);
                            page.spg.pcm = page.pcm;
                        }
                    }

                    if (page.keyOn)
                    {
                        page.keyOn = false;
                        OutFmKeyOn(page, mml);
                    }
                }
            }


        }


    }
}
