using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class YM2612X : YM2612
    {
        private int[] pcmKeyOnCh = new int[] { 0, 0, 0, 0 };
        private int[] pcmKeyOnInstNum = new int[] { -1, -1, -1, -1 };

        public YM2612X(ClsVgm parent, int chipID, string initialPartName, string stPath, int chipNumber) : base(parent, chipID, initialPartName, stPath, chipNumber)
        {
            _chipType = enmChipType.YM2612X;
            _Name = "YM2612X";
            _ShortName = "OPN2X";
            _ChMax = 6 + 3 + 3 + 4 + 4 + 4;//fm:6ch + fmex:3ch + fmpcm:3ch + fmpcm(overlay):12ch = total:24ch
            _canUsePcm = true;
            _canUsePI = false;
            FNumTbl = _FNumTbl;
            ChipNumber = chipNumber;

            Frequency = 7670454;
            port = new byte[][]{
                new byte[] { (byte)(chipNumber!=0 ? 0xa2 : 0x52) }
                , new byte[] { (byte)(chipNumber!=0 ? 0xa3 : 0x53) }
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

    public override void InitPart(partWork pw)
        {
            pw.pg[pw.cpg].slots = (byte)((pw.pg[pw.cpg].Type == enmChannelType.FMOPN || pw.pg[pw.cpg].ch == 2 || pw.pg[pw.cpg].ch == 5) ? 0xf : 0x0);
            pw.pg[pw.cpg].volume = 127;
            pw.pg[pw.cpg].MaxVolume = 127;
            pw.pg[pw.cpg].port = port;
            pw.pg[pw.cpg].pcm = pw.pg[pw.cpg].ch > 9;
        }


        public void OutYM2612XPcmKeyON(MML mml,partWork pw)
        {
            if (pw.pg[pw.cpg].instrument >= 63) return;

            if (pw.pg[pw.cpg].isPcmMap)
            {
                int n = Const.NOTE.IndexOf(pw.pg[pw.cpg].noteCmd);
                int f = pw.pg[pw.cpg].octaveNow * 12 + n + pw.pg[pw.cpg].shift + pw.pg[pw.cpg].keyShift;
                if (parent.instPCMMap.ContainsKey(pw.pg[pw.cpg].pcmMapNo))
                {
                    if (parent.instPCMMap[pw.pg[pw.cpg].pcmMapNo].ContainsKey(f))
                    {
                        pw.pg[pw.cpg].instrument = parent.instPCMMap[pw.pg[pw.cpg].pcmMapNo][f];
                    }
                    else
                    {
                        msgBox.setErrMsg(string.Format(msg.get("E10025"), pw.pg[pw.cpg].octaveNow, pw.pg[pw.cpg].noteCmd, pw.pg[pw.cpg].shift + pw.pg[pw.cpg].keyShift), mml.line.Lp);
                        return;
                    }
                }
                else
                {
                    msgBox.setErrMsg(string.Format(msg.get("E10024"), pw.pg[pw.cpg].pcmMapNo), mml.line.Lp);
                    return;
                }
            }

            if (!parent.instPCM.ContainsKey(pw.pg[pw.cpg].instrument))
            {
                msgBox.setErrMsg(string.Format(msg.get("E21000"), pw.pg[pw.cpg].instrument), mml.line.Lp);
                return;
            }

            int id = parent.instPCM[pw.pg[pw.cpg].instrument].seqNum + 1;

            int ch = Math.Max(0, pw.pg[pw.cpg].ch - 8);
            int priority = 0;

            pcmKeyOnCh[ch & 0x3] = pw.pg[pw.cpg].ch;
            pcmKeyOnInstNum[ch & 0x3] = id;

            byte[] cmd;
            if (parent.info.format == enmFormat.ZGM)
                cmd = new byte[] { 0x54, 0x00 };
            else
                cmd = new byte[] { 0x54 };

            parent.OutData(
                mml,
                cmd // original vgm command : YM2151
                , (byte)(0x50 + ((priority & 0x3) << 2) + (ch & 0x3))
                , (byte)id
                );

            parent.info.samplesPerClock = parent.info.xgmSamplesPerSecond * 60.0 * 4.0 / (parent.info.tempo * parent.info.clockCount);

            //必要なサンプル数を算出し、保持しているサンプル数より大きい場合は更新
            double m = pw.pg[pw.cpg].waitCounter * 60.0 * 4.0 / (parent.info.tempo * parent.info.clockCount) * 14000.0;//14000(Hz) = xgm sampling Rate
            parent.instPCM[pw.pg[pw.cpg].instrument].xgmMaxSampleCount = Math.Max(parent.instPCM[pw.pg[pw.cpg].instrument].xgmMaxSampleCount, m);

            if (parent.instPCM[pw.pg[pw.cpg].instrument].status != enmPCMSTATUS.ERROR)
            {
                parent.instPCM[pw.pg[pw.cpg].instrument].status = enmPCMSTATUS.USED;
            }

        }

        public void OutYM2612XPcmKeyOFF(MML mml, partWork pw)
        {

            int id = 0;
            int ch = Math.Max(0, pw.pg[pw.cpg].ch - 8);
            int priority = 0;

            byte[] cmd;
            if (parent.info.format == enmFormat.ZGM)
                cmd = new byte[] { 0x54, 0x00 };
            else
                cmd = new byte[] { 0x54 };

            if (pcmKeyOnCh[ch & 0x3] == pw.pg[pw.cpg].ch)
            {
                pcmKeyOnCh[ch & 0x3] = 0;
                pcmKeyOnInstNum[ch & 0x3] = -1;
            }

            parent.OutData(
                mml,
                cmd // original vgm command : YM2151
                , (byte)(0x50 + ((priority & 0x3) << 2) + (ch & 0x3))
                , (byte)id
                );

            parent.info.samplesPerClock = parent.info.xgmSamplesPerSecond * 60.0 * 4.0 / (parent.info.tempo * parent.info.clockCount);

        }

        public override void StorePcm(Dictionary<int, clsPcm> newDic, KeyValuePair<int, clsPcm> v, byte[] buf, bool is16bit, int samplerate, params object[] option)
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
                newDic.Add(v.Key, new clsPcm(
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


        public override void CmdMode(partWork pw, MML mml)
        {
            int n = (int)mml.args[0];
            if (pw.pg[pw.cpg].Type == enmChannelType.FMPCMex)
            {
                n = Common.CheckRange(n, 0, 1);
                pw.pg[pw.cpg].chip.lstPartWork[5].pg[lstPartWork[5].cpg].pcm = (n == 1);
                for (int i = 9; i < ChMax; i++)
                {
                    pw.pg[pw.cpg].chip.lstPartWork[i].pg[lstPartWork[i].cpg].pcm = (n == 1);
                }
                pw.pg[pw.cpg].freq = -1;//freqをリセット
                pw.pg[pw.cpg].instrument = -1;
                OutSetCh6PCMMode(
                    mml,
                    pw.pg[pw.cpg].chip.lstPartWork[5]
                    , pw.pg[pw.cpg].chip.lstPartWork[5].pg[lstPartWork[5].cpg].pcm
                    );

                return;
            }

            base.CmdMode(pw, mml);

        }

        public override void CmdPcmMapSw(partWork pw, MML mml)
        {
            bool sw = (bool)mml.args[0];
            if(pw.pg[pw.cpg].Type== enmChannelType.FMPCMex)
            {
                pw.pg[pw.cpg].isPcmMap = sw;
            }
        }


        public override void CmdInstrument(partWork pw, MML mml)
        {
            char type = (char)mml.args[0];
            int n = (int)mml.args[1];

            if (type == 'n' || type == 'N' || type == 'R' || type == 'A')
            {
                if (pw.pg[pw.cpg].Type == enmChannelType.FMOPNex)
                {
                    lstPartWork[2].pg[lstPartWork[2].cpg].instrument = n;
                    lstPartWork[6].pg[lstPartWork[6].cpg].instrument = n;
                    lstPartWork[7].pg[lstPartWork[7].cpg].instrument = n;
                    lstPartWork[8].pg[lstPartWork[8].cpg].instrument = n;
                    OutFmSetInstrument(pw, mml, n, pw.pg[pw.cpg].volume, type);
                    return;
                }
            }

            if (type == 'n')
            {
                if (pw.pg[pw.cpg].pcm)
                {

                    if (pw.pg[pw.cpg].isPcmMap)
                    {
                        pw.pg[pw.cpg].pcmMapNo = n;
                        if (!parent.instPCMMap.ContainsKey(n))
                        {
                            msgBox.setErrMsg(string.Format(msg.get("E10024"), n), mml.line.Lp);
                        }
                        return;
                    }

                    pw.pg[pw.cpg].instrument = n;
                    SetDummyData(pw, mml);
                    if (!parent.instPCM.ContainsKey(n))
                    {
                        msgBox.setErrMsg(string.Format(msg.get("E21000"), n), mml.line.Lp);
                    }
                    else
                    {
                        if (parent.instPCM[n].chip != enmChipType.YM2612X)
                        {
                            msgBox.setErrMsg(string.Format(msg.get("E21001"), n), mml.line.Lp);
                        }
                    }
                    return;
                }

            }

            base.CmdInstrument(pw, mml);
        }


    }
}
