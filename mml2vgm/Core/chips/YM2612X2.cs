using musicDriverInterface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using static MDSound.ym2612;

namespace Core
{
    public class YM2612X2 : YM2612
    {
        private int[] pcmKeyOnCh = new int[] { 0, 0, 0, 0 };
        private int[] pcmKeyOnInstNum = new int[] { -1, -1, -1, -1 };

        public YM2612X2(ClsVgm parent, int chipID, string initialPartName, string stPath, int chipNumber) : base(parent, chipID, initialPartName, stPath, chipNumber)
        {
            _chipType = enmChipType.YM2612X2;
            _Name = "YM2612X2";
            _ShortName = "OPN2X2";
            _ChMax = 6 + 3 + 3 + 4 + 4 + 4;//fm:6ch + fmex:3ch + fmpcm:3ch + fmpcm(overlay):12ch = total:24ch
            _canUsePcm = true;
            _canUsePI = false;
            FNumTbl = _FNumTbl;
            ChipNumber = chipNumber;

            Frequency = 7670454;
            port = new byte[][]{
                new byte[] { 0 } //ベタ(port0)
                , new byte[] { 1 } //ベタ(port1)
                , new byte[] { 2 } //専用コマンド
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
            Ch[5].Type = enmChannelType.FMPCMex;
            Ch[6].Type = enmChannelType.FMOPNex;
            Ch[7].Type = enmChannelType.FMOPNex;
            Ch[8].Type = enmChannelType.FMOPNex;
            for (int i = 9; i < ChMax; i++)
            {
                Ch[i].Type = enmChannelType.FMPCMex;
            }

            pcmDataInfo = new clsPcmDataInfo[] { new clsPcmDataInfo() };
            pcmDataInfo[0].totalBuf = new byte[0];
            pcmDataInfo[0].totalBufPtr = 0L;
            pcmDataInfo[0].use = false;

            for (int i = 0; i < 4; i++)
            {
                pcmKeyOnCh[i] = 0;
                pcmKeyOnInstNum[i] = -1;
            }
        }

        public override void InitChip()
        {
            if (!use) return;

            OutOPNSetHardLfo(null, lstPartWork[0].cpg, false, 0);
            OutOPNSetCh3SpecialMode(null, lstPartWork[0].cpg, false);
            OutSetCh6PCMMode(null, lstPartWork[0].cpg, true);
            SOutData(lstPartWork[0].cpg, (MML)null, port[0], new byte[]{ 0x2a, 0x80});
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

        }

        public override void OutOPNSetHardLfo(MML mml, partPage page, bool sw, int lfoNum)
        {
            SOutData(page, mml, port[2]
                , 0xf9, (byte)((lfoNum & 7) + (sw ? 8 : 0))
                );
        }

        public override void OutOPNSetCh3SpecialMode(MML mml, partPage page, bool sw)
        {
            // ignore Timer ^^;
            SOutData(page, mml, port[2]
                , (byte)((sw ? 0xFA : 0xFB))
                );
        }

        public override void OutSetCh6PCMMode(MML mml, partPage page, bool sw)
        {
            SOutData(page, mml, port[2], (byte)(sw ? 0xFC : 0xFD));
        }

        public void OutYM2612X2PcmKeyOFF(MML mml, partPage page)
        {
            int id = 0;//0はキーオフ専用ID
            int ch = Math.Max(0, page.ch - 8);
            bool hiPriority = true;
            bool fullSpeed = true;

            if (pcmKeyOnCh[ch & 0x3] == page.ch)
            {
                pcmKeyOnCh[ch & 0x3] = 0;
                pcmKeyOnInstNum[ch & 0x3] = -1;
            }

            SOutData(page, mml,
                port[2]
                , (byte)(0x10 + (hiPriority ? 0x08 : 0x00) + (fullSpeed ? 0x00 : 0x04) + (ch & 0x3))
                , (byte)id
                );

            if (parent.info.vgmVsync == -1)
            {
                parent.info.samplesPerClock = parent.info.xgmSamplesPerSecond * 60.0 * 4.0 / (parent.info.tempo * parent.info.clockCount);
            }
            else
            {
                parent.info.samplesPerClock = parent.info.xgmSamplesPerSecond / parent.info.vgmVsync;
            }
        }

        public void OutYM2612X2PcmKeyON(MML mml, partPage page)
        {
            MML mmlDmy = mml != null ? mml : lastKeyOnMML;
            if (page.isPcmMap)
            {
                int n = Const.NOTE.IndexOf(page.noteCmd);
                int arpNote = page.arpFreqMode ? 0 : page.arpDelta;
                int f = page.octaveNow * 12 + n + page.shift + page.keyShift + arpNote;
                if (parent.instPCMMap.ContainsKey(page.pcmMapNo))
                {
                    if (parent.instPCMMap[page.pcmMapNo].ContainsKey(f))
                    {
                        page.instrument = parent.instPCMMap[page.pcmMapNo][f];
                    }
                    else
                    {
                        msgBox.setErrMsg(
                            string.Format(msg.get("E10025"), page.octaveNow, page.noteCmd, page.shift + page.keyShift + arpNote),
                            mmlDmy != null ? mmlDmy.line.Lp : null);
                        return;
                    }
                }
                else
                {
                    msgBox.setErrMsg(string.Format(msg.get("E10024"), page.pcmMapNo), mmlDmy != null ? mmlDmy.line.Lp : null);
                    return;
                }
            }

            if (!parent.instPCM.ContainsKey(page.isPcmMap ? (page.instrument & 0xff) : page.instrument))
            {
                msgBox.setErrMsg(string.Format(msg.get("E21000"), page.instrument), mmlDmy != null ? mmlDmy.line.Lp : null);
                return;
            }

            int id = parent.instPCM[page.instrument & 0xff].Item2.seqNum + 1;
            int ch = Math.Max(0, page.ch - 8);
            bool hiPriority = true;
            bool fullSpeed = page.noise == 0;
            if (page.isPcmMap)
            {
                fullSpeed = (page.instrument & 0x1_0000) == 0;
            }

            if (pcmKeyOnCh[ch & 0x3] == page.ch)
            {
                pcmKeyOnCh[ch & 0x3] = 0;
                pcmKeyOnInstNum[ch & 0x3] = -1;
            }

            SOutData(page, mml,
                port[2]
                , (byte)(0x10 + (hiPriority ? 0x08 : 0x00) + (fullSpeed ? 0x00 : 0x04) + (ch & 0x3))
                , (byte)id
                );

            if (parent.info.vgmVsync == -1)
            {
                parent.info.samplesPerClock = parent.info.xgmSamplesPerSecond * 60.0 * 4.0 / (parent.info.tempo * parent.info.clockCount);
            }
            else
            {
                parent.info.samplesPerClock = parent.info.xgmSamplesPerSecond / parent.info.vgmVsync;
            }
        }

        public override void OutFmKeyOff(partPage page, MML mml)
        {
            int n = 3;

            if ((page.ch > 8 || page.ch == 5) && page.pcm)
            {
                OutYM2612X2PcmKeyOFF(mml, page);
                return;
            }

            if (!page.pcm)//FM
            {
                if (page.chip.lstPartWork[2].cpg.Ch3SpecialMode && page.Type == enmChannelType.FMOPNex)
                {
                    page.Ch3SpecialModeKeyOn = false;

                    int slot = (page.chip.lstPartWork[2].apg.Ch3SpecialModeKeyOn ? page.chip.lstPartWork[2].apg.slots : 0x0)
                        | (page.chip.lstPartWork[n + 3].apg.Ch3SpecialModeKeyOn ? page.chip.lstPartWork[n + 3].apg.slots : 0x0)
                        | (page.chip.lstPartWork[n + 4].apg.Ch3SpecialModeKeyOn ? page.chip.lstPartWork[n + 4].apg.slots : 0x0)
                        | (page.chip.lstPartWork[n + 5].apg.Ch3SpecialModeKeyOn ? page.chip.lstPartWork[n + 5].apg.slots : 0x0);

                    SOutData(page, mml, page.port[2], 0xf8, (byte)((slot << 4) + 2));
                }
                else
                {
                    if (page.ch >= 0 && page.ch < n + 3)
                    {
                        byte vch = (byte)((page.ch > 2) ? page.ch + 1 : page.ch);
                        //key off
                        SOutData( page, mml, page.port[2], (byte)(0x40 + (vch & 7)) );//bit3: 0=OFF 1=ON
                        page.keyOnDelay.KeyOff();
                    }
                }
                return;
            }

            //PCM
            //ストリーム形式の発音は未サポート

            page.pcmWaitKeyOnCounter = -1;
        }

        public override void OutFmKeyOn(partPage page, MML mml)
        {
            SetDummyData(page, mml);

            int n = 3;

            if ((page.ch > 8 || page.ch == 5) && page.chip.lstPartWork[5].pg[0].pcm)
            {
                OutYM2612X2PcmKeyON(mml, page);
                return;
            }

            if (!page.pcm)//FM KeyON
            {
                OutFmKeyOnMain(page, mml, n);
                return;
            }

            //OutFmKeyOnPCMSide(page, mml);
        }

        private void OutFmKeyOnMain(partPage page, MML mml, int n)
        {
            if (page.chip.lstPartWork[2].apg.Ch3SpecialMode && page.Type == enmChannelType.FMOPNex)
            {
                page.Ch3SpecialModeKeyOn = true;

                int slot = (page.chip.lstPartWork[2].apg.Ch3SpecialModeKeyOn ? page.chip.lstPartWork[2].apg.slots : 0x0)
                    | (page.chip.lstPartWork[n + 3].apg.Ch3SpecialModeKeyOn ? page.chip.lstPartWork[n + 3].apg.slots : 0x0)
                    | (page.chip.lstPartWork[n + 4].apg.Ch3SpecialModeKeyOn ? page.chip.lstPartWork[n + 4].apg.slots : 0x0)
                    | (page.chip.lstPartWork[n + 5].apg.Ch3SpecialModeKeyOn ? page.chip.lstPartWork[n + 5].apg.slots : 0x0);

                outDatum od = new outDatum();
                od.val = (byte)((slot << 4) + 2);
                if (mml != null)
                {
                    od.type = mml.type;
                    if (mml.line != null && mml.line.Lp != null)
                    {
                        od.linePos = new LinePos(
                            mml.line.Lp.document,
                            mml.line.Lp.srcMMLID,
                            mml.line.Lp.row,
                            mml.line.Lp.col,
                            mml.line.Lp.length,
                            mml.line.Lp.part,
                            mml.line.Lp.chip,
                            mml.line.Lp.chipIndex,
                            mml.line.Lp.chipNumber,
                            mml.line.Lp.ch);
                    }
                }

                if (page.RR15sw)
                {
                    if (page.RR15 == 0) SetRR15(page, mml, slot);
                    ResetRR15(page, mml, slot);
                }

                SOutData(page, mml, port[0], 0x28, (byte)((slot << 4) + (2 & 7)));
            }
            else
            {
                if (page.ch >= 0 && page.ch < n + 3)
                {
                    GetPortVch(page, out byte[] p, out int vch);
                    outDatum od = new outDatum();
                    if (!page.keyOnDelay.sw)
                    {
                        od.val = (byte)((page.slots << 4) + (vch & 7));
                    }
                    else
                    {
                        od.val = (byte)((page.keyOnDelay.keyOn << 4) + (vch & 7));
                        page.keyOnDelay.beforekeyOn = 0xff;
                    }
                    if (mml != null)
                    {
                        od.type = mml.type;
                        if (mml.line != null && mml.line.Lp != null)
                        {
                            od.linePos = new LinePos(
                                mml.line.Lp.document,
                                mml.line.Lp.srcMMLID,
                                mml.line.Lp.row,
                                mml.line.Lp.col,
                                mml.line.Lp.length,
                                mml.line.Lp.part,
                                mml.line.Lp.chip,
                                mml.line.Lp.chipIndex,
                                mml.line.Lp.chipNumber,
                                mml.line.Lp.ch);
                        }
                    }

                    if (page.RR15sw)
                    {
                        if (page.RR15 == 0) SetRR15(page, mml, page.slots);
                        ResetRR15(page, mml, page.slots);
                    }
                    
                    SOutData(page, mml, port[2], (byte)(0x40 + (p[0] == 0 ? 0 : 4) + vch + 0x8));
                }
            }
        }

        public override void OutFmSetTl(MML mml, partPage page, int ope, int tl)
        {
            GetPortVch(page, out byte[] p, out int vch);
            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            tl &= 0x7f;

            if (page.beforeTL[ope] == tl) return;
            page.beforeTL[ope] = tl;

            SOutData(page, mml,
                port[2], (byte)(0x90 + (vch & 0x3) + (ope << 2)), (byte)((tl<<1) | (p[0] != 0 ? 0x01 : 0x00))
                );
        }

        public override void OutOPNSetPanAMSPMS(MML mml, partPage page, int pan, int ams, int pms)
        {
            //TODO: 効果音パートで指定されている場合の考慮不足
            int vch;
            byte[] portD;
            GetPortVch(page, out portD, out vch);
            if (page.chip is YM2612X && page.ch > 8 && page.ch < 12)
            {
                vch = 2;
            }

            pan &= 3;
            ams &= 3;
            pms &= 7;

            if(ams==0 && pms == 0)
            {
                SOutData(page, mml, port[2], (byte)(((vch & 0x4) != 0 ? 0x70 : 0x60) + (vch & 0x3) + (pan << 2)));
                return;
            }

            SOutData(page, mml, portD, (byte)(0xb4 + vch), (byte)((pan << 6) + (ams << 4) + pms));
        }

        public override void InitPart(partWork pw)
        {
            foreach (partPage page in pw.pg)
            {
                page.slots = (byte)((page.Type == enmChannelType.FMOPN || page.ch == 2 || page.ch == 5) ? 0xf : 0x0);
                page.volume = 127;
                page.MaxVolume = 127;
                page.port = port;
                page.pcm = page.ch > 9;
                page.noise = 0;
                page.noiseFreq = -1;
                page.spg.freq = -1;
            }
        }






        public void OutYM2612XPcmKeyON(MML mml, partPage page)
        {
            if (page.instrument >= 63) return;

            MML mmlDmy = mml != null ? mml : lastKeyOnMML;
            if (page.isPcmMap)
            {
                int n = Const.NOTE.IndexOf(page.noteCmd);
                int arpNote = page.arpFreqMode ? 0 : page.arpDelta;
                int f = page.octaveNow * 12 + n + page.shift + page.keyShift + arpNote;
                if (parent.instPCMMap.ContainsKey(page.pcmMapNo))
                {
                    if (parent.instPCMMap[page.pcmMapNo].ContainsKey(f))
                    {
                        page.instrument = parent.instPCMMap[page.pcmMapNo][f];
                    }
                    else
                    {
                        msgBox.setErrMsg(
                            string.Format(msg.get("E10025"), page.octaveNow, page.noteCmd, page.shift + page.keyShift + arpNote),
                            mmlDmy != null ? mmlDmy.line.Lp : null);
                        return;
                    }
                }
                else
                {
                    msgBox.setErrMsg(string.Format(msg.get("E10024"), page.pcmMapNo), mmlDmy != null ? mmlDmy.line.Lp : null);
                    return;
                }
            }

            if (!parent.instPCM.ContainsKey(page.instrument))
            {
                msgBox.setErrMsg(string.Format(msg.get("E21000"), page.instrument), mmlDmy != null ? mmlDmy.line.Lp : null);
                return;
            }

            int id = parent.instPCM[page.instrument].Item2.seqNum + 1;

            int ch = Math.Max(0, page.ch - 8);
            int priority = 0;
            int speed = page.noise == 0 ? 0 : 0x4;

            pcmKeyOnCh[ch & 0x3] = page.ch;
            pcmKeyOnInstNum[ch & 0x3] = id;

            byte[] cmd;
            if (parent.info.format == enmFormat.ZGM)
                cmd = new byte[] { 0x54, 0x00 };
            else
                cmd = new byte[] { 0x54 };

            SOutData(
                page,
                mmlDmy,
                cmd // original vgm command : YM2151
                  //b0b1:ch b2:speed b3:priority
                , (byte)(0x50 + (ch & 0x3) + speed + ((priority & 0x3) << 2))
                , (byte)id
                );

            if (parent.info.vgmVsync == -1)
            {
                parent.info.samplesPerClock = parent.info.xgmSamplesPerSecond * 60.0 * 4.0 / (parent.info.tempo * parent.info.clockCount);
            }
            else
            {
                parent.info.samplesPerClock = parent.info.xgmSamplesPerSecond / parent.info.vgmVsync;
            }
            //必要なサンプル数を算出し、保持しているサンプル数より大きい場合は更新
            double m = page.waitCounter * 60.0 * 4.0 / (parent.info.tempo * parent.info.clockCount) * 14000.0;//14000(Hz) = xgm sampling Rate
            parent.instPCM[page.instrument].Item2.xgmMaxSampleCount = Math.Max(parent.instPCM[page.instrument].Item2.xgmMaxSampleCount, m);

            if (parent.instPCM[page.instrument].Item2.status != enmPCMSTATUS.ERROR)
            {
                parent.instPCM[page.instrument].Item2.status = enmPCMSTATUS.USED;
            }

        }

        public void OutYM2612XPcmKeyOFF(MML mml, partPage page)
        {

            int id = 0;
            int ch = Math.Max(0, page.ch - 8);
            int priority = 0;

            byte[] cmd;
            if (parent.info.format == enmFormat.ZGM)
                cmd = new byte[] { 0x54, 0x00 };
            else
                cmd = new byte[] { 0x54 };

            if (pcmKeyOnCh[ch & 0x3] == page.ch)
            {
                pcmKeyOnCh[ch & 0x3] = 0;
                pcmKeyOnInstNum[ch & 0x3] = -1;
            }

            SOutData(
                page,
                mml,
                cmd // original vgm command : YM2151
                , (byte)(0x50 + ((priority & 0x3) << 2) + (ch & 0x3))
                , (byte)id
                );

            if (parent.info.vgmVsync == -1)
            {
                parent.info.samplesPerClock = parent.info.xgmSamplesPerSecond * 60.0 * 4.0 / (parent.info.tempo * parent.info.clockCount);
            }
            else
            {
                parent.info.samplesPerClock = parent.info.xgmSamplesPerSecond / parent.info.vgmVsync;
            }
        }

        public override void StorePcm(Dictionary<int,Tuple<string, clsPcm>> newDic, KeyValuePair<int, clsPcm> v, byte[] buf, bool is16bit, int samplerate, params object[] option)
        {
            clsPcmDataInfo pi = pcmDataInfo[0];

            try
            {
                byte[] newBuf;
                long size = buf.Length;

                for (int i = 0; i < size; i++)
                {
                    buf[i] -= 0x80;//符号化
                }

                //Padding
                if (size % 0x100 != 0)
                {
                    newBuf = Common.PcmPadding(ref buf, ref size, 0x00, 0x100);
                    buf = newBuf;
                }

                if (newDic.ContainsKey(v.Key))
                {
                    newDic.Remove(v.Key);
                }
                newDic.Add(v.Key, new Tuple<string, clsPcm>("", new clsPcm(
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
                    , samplerate)
                    ));
                pi.totalBufPtr += size;

                newBuf = new byte[pi.totalBuf.Length + buf.Length];
                Array.Copy(pi.totalBuf, newBuf, pi.totalBuf.Length);
                Array.Copy(buf, 0, newBuf, pi.totalBuf.Length, buf.Length);

                pi.totalBuf = newBuf;
                pi.use = true;
                pcmDataEasy = pi.use ? pi.totalBuf : null;
            }
            catch
            {
                pi.use = false;
            }
        }


        public override void CmdMode(partPage page, MML mml)
        {
            int n = (int)mml.args[0];
            if (page.Type == enmChannelType.FMPCMex)
            {
                n = Common.CheckRange(n, 0, 1);
                page.chip.lstPartWork[5].pg[0].pcm = (n == 1);
                for (int i = 9; i < ChMax; i++)
                {
                    page.chip.lstPartWork[i].pg[0].pcm = (n == 1);
                }
                page.freq = -1;//freqをリセット
                page.instrument = -1;
                OutSetCh6PCMMode(
                    mml,
                    page.chip.lstPartWork[5].pg[0]
                    , page.chip.lstPartWork[5].pg[0].pcm
                    );

                return;
            }

            base.CmdMode(page, mml);

        }

        public override void CmdPcmMapSw(partPage page, MML mml)
        {
            bool sw = (bool)mml.args[0];
            if (page.Type == enmChannelType.FMPCMex)
            {
                page.isPcmMap = sw;
            }
        }

        public override void CmdNoise(partPage page, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 0, 63);

            page.noise = n;
        }




        public override void CmdInstrument(partPage page, MML mml)
        {
            char type;
            bool re = false;
            int n;
            if (mml.args[0] is bool)
            {
                type = (char)mml.args[1];
                re = true;
                n = (int)mml.args[2];
            }
            else
            {
                type = (char)mml.args[0];
                n = (int)mml.args[1];
            }


            if (type == 'n' || type == 'N' || type == 'R' || type == 'A')
            {
                if (re) n = page.instrument + n;
                n = Common.CheckRange(n, 0, 255);

                if (page.Type == enmChannelType.FMOPNex)
                {
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
                        if (re) n = page.pcmMapNo + n;
                        n = Common.CheckRange(n, 0, 255);
                        page.pcmMapNo = n;
                        if (!parent.instPCMMap.ContainsKey(n))
                        {
                            msgBox.setErrMsg(string.Format(msg.get("E10024"), n), mml.line.Lp);
                        }
                        return;
                    }

                    if (re) n = page.instrument + n;
                    n = Common.CheckRange(n, 0, 255);
                    page.instrument = n;
                    SetDummyData(page, mml);
                    if (!parent.instPCM.ContainsKey(n))
                    {
                        msgBox.setErrMsg(string.Format(msg.get("E33000"), n), mml.line.Lp);
                    }
                    else
                    {
                        if (parent.instPCM[n].Item2.chip != enmChipType.YM2612X2)
                        {
                            msgBox.setErrMsg(string.Format(msg.get("E33001"), n), mml.line.Lp);
                        }
                    }
                    return;
                }

            }

            base.CmdInstrument(page, mml);
        }

        public override void OutFmSetFnum(partPage page, MML mml, int octave, int num)
        {
            int freq;
            freq = ((num & 0x700) >> 8) + (((octave - 1) & 0x7) << 3);
            freq = (freq << 8) + (num & 0xff);

            if (freq == page.spg.freq) return;

            int delta = 0;
            if (Math.Abs(freq - page.spg.freq) < 129)
            {
                delta = freq - page.spg.freq;
            }

            page.spg.freq = freq;

            partWork pw = page.chip.lstPartWork[2];

            if (pw.apg.Ch3SpecialMode && page.Type == enmChannelType.FMOPNex)
            {
                if ((page.slots & 8) != 0)
                {
                    int f;
                    if (pw.slotFixedFnum[3] == -1) f = freq + page.slotDetune[3];
                    else f = pw.slotFixedFnum[3] + page.slotDetune[3];

                    SOutData(page, mml, page.port[2]
                        , (byte)(0x30 + 0 + 3 + 8)//0:port 3:slot4 8:ch3specialMode
                        , (byte)((f & 0xff00) >> 8)
                        , (byte)(f & 0xff));
                }
                if ((page.slots & 4) != 0)
                {
                    int f;
                    if (pw.slotFixedFnum[2] == -1) f = freq + page.slotDetune[2];
                    else f = pw.slotFixedFnum[2] + page.slotDetune[2];

                    SOutData(page, mml, page.port[2]
                        , (byte)(0x30 + 0 + 2 + 8)//0:port 2:slot3 8:ch3specialMode
                        , (byte)((f & 0xff00) >> 8)
                        , (byte)(f & 0xff));
                }
                if ((page.slots & 1) != 0)
                {
                    int f;
                    if (pw.slotFixedFnum[0] == -1) f = freq + page.slotDetune[0];
                    else f = pw.slotFixedFnum[0] + page.slotDetune[0];

                    SOutData(page, mml, page.port[2]
                        , (byte)(0x30 + 0 + 1 + 8)//0:port 1:slot2 8:ch3specialMode
                        , (byte)((f & 0xff00) >> 8)
                        , (byte)(f & 0xff));
                }
                if ((page.slots & 2) != 0)
                {
                    int f;
                    if (pw.slotFixedFnum[1] == -1) f = freq + page.slotDetune[1];
                    else f = pw.slotFixedFnum[1] + page.slotDetune[1];

                    SOutData(page, mml, page.port[2]
                        , (byte)(0x30 + 0 + 0 + 8)//0:port 0:slot1 8:ch3specialMode
                        , (byte)((f & 0xff00) >> 8)
                        , (byte)(f & 0xff));
                }
            }
            else
            {
                int n=3;
                if (page.ch >= n + 3 && page.ch < n + 6) return;

                if (page.ch >= 9 && page.ch <= 11) return;

                if (page.ch < n + 3)
                {
                    if (page.pcm) return;

                    int vch;
                    byte[] port;
                    GetPortVch(page, out port, out vch);

                    if (delta == 0)
                    {
                        SOutData(page, mml, page.port[2]
                            , (byte)(0x30 + (port[0] == 0 ? 0 : 4) + vch + 0)
                            , (byte)((freq & 0xff00) >> 8)
                            , (byte)(freq & 0xff));
                    }
                    else
                    {
                        SOutData(page, mml, page.port[2]
                            , (byte)(0xa0 + (port[0] == 0 ? 0 : 4) + vch + 0)
                            , (byte)(((Math.Abs(delta)-1) << 1) + (delta > 0 ? 0 : 1))
                            );
                    }
                }
            }
        }

        public override void SetupPageData(partWork pw, partPage page)
        {
            if (page.Type == enmChannelType.FMOPN
                || page.Type == enmChannelType.FMOPNex //効果音モード対応チャンネル
                || (page.Type == enmChannelType.FMPCMex && !page.chip.lstPartWork[5].pg[0].pcm) //OPN2PCMチャンネル
                )
            {
                if (page.Type == enmChannelType.FMPCMex)
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
            else if (page.Type == enmChannelType.FMPCMex && page.chip.lstPartWork[5].pg[0].pcm)
            {
                if (page.Type == enmChannelType.FMPCMex)
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

        private MML lastKeyOnMML = null;

        public override void SetKeyOn(partPage page, MML mml)
        {
            SetDummyData(page, mml);
            page.keyOn = true;
            lastKeyOnMML = mml;
        }

        public override void MultiChannelCommand(MML mml)
        {
            if (!use) return;
            //int dat = 0;

            foreach (partWork pw in lstPartWork)
            {
                foreach (partPage page in pw.pg)
                {
                    if (page.Type == enmChannelType.FMOPN
                        || (!page.Ch3SpecialMode && page.Type == enmChannelType.FMOPNex)
                        || (!page.pcm && page.Type == enmChannelType.FMPCM))
                    {
                        if (page.keyOnDelay.sw && !page.keyOff)
                        {
                            if (page.keyOnDelay.beforekeyOn != page.keyOnDelay.keyOn)
                            {
                                page.keyOn = true;
                                page.keyOnDelay.beforekeyOn = page.keyOnDelay.keyOn;
                            }
                        }
                        //continue;
                    }

                    if (page.Type == enmChannelType.FMPCMex)
                    {
                        if (page.spg.pcm != page.chip.lstPartWork[5].pg[0].pcm)
                        {
                            OutSetCh6PCMMode(null, page, page.chip.lstPartWork[5].pg[0].pcm);
                            page.spg.pcm = page.chip.lstPartWork[5].pg[0].pcm;
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

        public void OutFmSetInstrumentBox(partPage page, MML mml,int n)
        {
            int vch;
            byte[] port;
            GetPortVch(page, out port, out vch);
            byte[] voi = new byte[30];

            int op1 = 0 * Const.INSTRUMENT_M_OPERATOR_SIZE;
            int op2 = 2 * Const.INSTRUMENT_M_OPERATOR_SIZE;
            int op3 = 1 * Const.INSTRUMENT_M_OPERATOR_SIZE;
            int op4 = 3 * Const.INSTRUMENT_M_OPERATOR_SIZE;

            //dt/ml
            voi[0] = (byte)(((parent.instFM[n].Item2[op1 + 9] & 7) << 4) + (parent.instFM[n].Item2[op1 + 8] & 15));
            voi[1] = (byte)(((parent.instFM[n].Item2[op2 + 9] & 7) << 4) + (parent.instFM[n].Item2[op2 + 8] & 15));
            voi[2] = (byte)(((parent.instFM[n].Item2[op3 + 9] & 7) << 4) + (parent.instFM[n].Item2[op3 + 8] & 15));
            voi[3] = (byte)(((parent.instFM[n].Item2[op4 + 9] & 7) << 4) + (parent.instFM[n].Item2[op4 + 8] & 15));
            //tl
            voi[4] = (byte)(parent.instFM[n].Item2[op1 + 6] & 0x7f);
            voi[5] = (byte)(parent.instFM[n].Item2[op2 + 6] & 0x7f);
            voi[6] = (byte)(parent.instFM[n].Item2[op3 + 6] & 0x7f);
            voi[7] = (byte)(parent.instFM[n].Item2[op4 + 6] & 0x7f);
            //ks/ar
            voi[8] = (byte)(((parent.instFM[n].Item2[op1 + 7] & 3) << 6) + (parent.instFM[n].Item2[op1 + 1] & 31));
            voi[9] = (byte)(((parent.instFM[n].Item2[op2 + 7] & 3) << 6) + (parent.instFM[n].Item2[op2 + 1] & 31));
            voi[10] = (byte)(((parent.instFM[n].Item2[op3 + 7] & 3) << 6) + (parent.instFM[n].Item2[op3 + 1] & 31));
            voi[11] = (byte)(((parent.instFM[n].Item2[op4 + 7] & 3) << 6) + (parent.instFM[n].Item2[op4 + 1] & 31));
            //DR/AM
            voi[12] = (byte)(((parent.instFM[n].Item2[op1 + 10] & 1) << 7) + (parent.instFM[n].Item2[op1 + 2] & 31));
            voi[13] = (byte)(((parent.instFM[n].Item2[op2 + 10] & 1) << 7) + (parent.instFM[n].Item2[op2 + 2] & 31));
            voi[14] = (byte)(((parent.instFM[n].Item2[op3 + 10] & 1) << 7) + (parent.instFM[n].Item2[op3 + 2] & 31));
            voi[15] = (byte)(((parent.instFM[n].Item2[op4 + 10] & 1) << 7) + (parent.instFM[n].Item2[op4 + 2] & 31));
            //SR
            voi[16] = (byte)(parent.instFM[n].Item2[op1 + 3] & 31);
            voi[17] = (byte)(parent.instFM[n].Item2[op2 + 3] & 31);
            voi[18] = (byte)(parent.instFM[n].Item2[op3 + 3] & 31);
            voi[19] = (byte)(parent.instFM[n].Item2[op4 + 3] & 31);
            //SL/RR
            voi[20] = (byte)(((parent.instFM[n].Item2[op1 + 5] & 15) << 4) + (parent.instFM[n].Item2[op1 + 4] & 15));
            voi[21] = (byte)(((parent.instFM[n].Item2[op2 + 5] & 15) << 4) + (parent.instFM[n].Item2[op2 + 4] & 15));
            voi[22] = (byte)(((parent.instFM[n].Item2[op3 + 5] & 15) << 4) + (parent.instFM[n].Item2[op3 + 4] & 15));
            voi[23] = (byte)(((parent.instFM[n].Item2[op4 + 5] & 15) << 4) + (parent.instFM[n].Item2[op4 + 4] & 15));
            //SSGEG
            voi[24] = (byte)(parent.instFM[n].Item2[op1 + 11] & 15);
            voi[25] = (byte)(parent.instFM[n].Item2[op2 + 11] & 15);
            voi[26] = (byte)(parent.instFM[n].Item2[op3 + 11] & 15);
            voi[27] = (byte)(parent.instFM[n].Item2[op4 + 11] & 15);
            //FB/ALG
            voi[28] = (byte)(((parent.instFM[n].Item2[46] & 7) << 3) + (parent.instFM[n].Item2[45] & 7));
            //pan/ams/pms
            voi[29] = 0xc0;

            SOutData(page, mml, page.port[2]
                , (byte)(0x20 + (port[0] == 0 ? 0 : 4) + vch)//0:port 3:slot4 8:ch3specialMode
                , voi[0], voi[1], voi[2], voi[3], voi[4], voi[5], voi[6], voi[7], voi[8], voi[9]
                , voi[10], voi[11], voi[12], voi[13], voi[14], voi[15], voi[16], voi[17], voi[18], voi[19]
                , voi[20], voi[21], voi[22], voi[23], voi[24], voi[25], voi[26], voi[27], voi[28], voi[29]
                );
        }
    }
}
