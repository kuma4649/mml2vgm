using musicDriverInterface;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MDSound.fmgen.FileIO;

namespace Corex64.chips
{
    public class Pokey : ClsChip
    {

        private const byte AUDCTL = 0x08;
        private const byte AUDF1 = 0x00;
        private const byte AUDC1 = 0x01;
        private const byte AUDF2 = 0x02;
        private const byte AUDC2 = 0x03;
        private const byte AUDF3 = 0x04;
        private const byte AUDC3 = 0x05;
        private const byte AUDF4 = 0x06;
        private const byte AUDC4 = 0x07;

        public byte AUDCTLvalue = 0x00;
        public byte AUDCTLvalueBefore = 0xff;

        public Pokey(ClsVgm parent, int chipID, string initialPartName, string stPath, int chipNumber) : base(parent, chipID, initialPartName, stPath, chipNumber)
        {
            _chipType = enmChipType.POKEY;
            _Name = "POKEY";
            _ShortName = "POKEY";
            _ChMax = 4;
            _canUsePcm = true;
            _canUsePI = false;
            ChipNumber = chipNumber;

            Frequency = 1789772;
            port = new byte[][] { new byte[] { 0xbb } };
            DataBankID = 0x10;//TBD(固定値ではなく、恐らくデータごとに連番を振るのが良いと思われる。)

            if (string.IsNullOrEmpty(initialPartName)) return;

            Dictionary<string, List<double>> dic = MakeFNumTbl();
            if (dic != null)
            {
                int c = 0;
                FNumTbl = new int[1][] { new int[96] };
                if (dic.ContainsKey("FNUM_00"))
                {
                    foreach (double v in dic["FNUM_00"])
                    {
                        FNumTbl[0][c++] = (int)v;
                        if (c == FNumTbl[0].Length) break;
                    }
                }
                if (dic.ContainsKey("MASTERCLOCK"))
                {
                    Frequency = (int)dic["MASTERCLOCK"][0];
                }
            }

            Ch = new ClsChannel[ChMax];
            SetPartToCh(Ch, initialPartName);
            foreach (ClsChannel ch in Ch)
            {
                ch.Type = enmChannelType.SSG;
                ch.chipNumber = chipID == 1;
                ch.MaxVolume = 15;
            }

            Envelope = new Function();
            Envelope.Max = 15;
            Envelope.Min = 0;

            pcmDataInfo = new clsPcmDataInfo[] { new clsPcmDataInfo() };
            pcmDataInfo[0].totalBufPtr = 0L;
            pcmDataInfo[0].use = false;

            if (parent.info.format == enmFormat.ZGM)
            {
                if (parent.ChipCommandSize == 2)
                {
                    if (chipNumber == 0) pcmDataInfo[0].totalBuf = new byte[] { 0x07, 0x00, 0x66, DataBankID, 0x00, 0x00, 0x00, 0x00 };
                    else pcmDataInfo[0].totalBuf = new byte[] { 0x07, 0x00, 0x66, DataBankID, 0x00, 0x00, 0x00, 0x00 };
                }
                else
                {
                    if (chipNumber == 0) pcmDataInfo[0].totalBuf = new byte[] { 0x07, 0x66, DataBankID, 0x00, 0x00, 0x00, 0x00 };
                    else pcmDataInfo[0].totalBuf = new byte[] { 0x07, 0x66, DataBankID, 0x00, 0x00, 0x00, 0x00 };
                }
            }
            else
            {
                if (chipNumber == 0) pcmDataInfo[0].totalBuf = new byte[] { 0x67, 0x66, DataBankID, 0x00, 0x00, 0x00, 0x00 };
                else pcmDataInfo[0].totalBuf = new byte[] { 0x67, 0x66, DataBankID, 0x00, 0x00, 0x00, 0x00 };
            }
        }

        public override void InitPart(partWork pw)
        {
            foreach (partPage pg in pw.pg)
            {
                pg.volume = 15;
                pg.MaxVolume = 15;
                pg.port = port;
                pg.noise = 7;
            }

            AUDCTLvalue = 0x00;
            AUDCTLvalueBefore = 0xff;
        }

        public override void InitChip()
        {
            if (!use) return;

            for (int ch = 0; ch < ChMax; ch++)
            {
                //lstPartWork[ch].apg.volume = 0;
            }

            if (parent.info.format == enmFormat.VGM && ChipID != 0)
            {
                //parent.dat[0x77] = new outDatum(enmMMLType.unknown, null, null, (byte)(parent.dat[0x77].val | 0x40));//use Secondary
            }
        }

        public override void SetFNum(partPage page, MML mml)
        {
            SetSsgFNum(page, mml);
        }

        public override void SetVolume(partPage page, MML mml)
        {
            SetSsgVolume(page, mml);
        }

        public override void SetKeyOn(partPage page, MML mml)
        {
            page.keyOn = true;
        }

        public override void SetKeyOff(partPage page, MML mml)
        {
            page.keyOn = false;
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
                pl.value = 0;// (pl.param[0] == 0) ? pl.param[6] : 0;//ディレイ中は振幅補正は適用されない
                pl.phase = (pl.param[0] == 0) ? pl.param[6] : 0;//ディレイ中は振幅補正は適用されない
                pl.waitCounter = pl.param[0];
                pl.direction = pl.param[2] < 0 ? -1 : 1;
                pl.depthWaitCounter = pl.param[7];
                pl.depth = pl.param[3];
                pl.depthV2 = pl.param[2];

                if (pl.type == eLfoType.Vibrato)
                {
                    SetSsgFNum(page, mml);
                }

                if (pl.type == eLfoType.Tremolo)
                {
                    SetSsgVolume(page, mml);
                }

            }
        }

        public int GetSsgFNum(partPage page, MML mml, int octave, char noteCmd, int shift, int pitchShift)
        {
            int o = octave - 1;
            int n = Const.NOTE.IndexOf(noteCmd) + shift;
            o += n / 12;
            o = Common.CheckRange(o, 0, 7);
            n %= 12;

            int f = o * 12 + n;
            f += pitchShift;
            if (f < 0) f = 0;
            if (f >= FNumTbl[0].Length) f = FNumTbl[0].Length - 1;

            return FNumTbl[0][f];
        }

        private void SetSsgFNum(partPage page, MML mml)
        {
            int arpNote = page.arpFreqMode ? 0 : page.arpDelta;
            int arpFreq = page.arpFreqMode ? page.arpDelta : 0;

            int f = GetSsgFNum(page, mml, page.octaveNow, page.noteCmd, page.shift + page.keyShift + arpNote, page.pitchShift);//
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
                f += page.lfo[lfo].value + page.lfo[lfo].phase;
            }

            f = Common.CheckRange(f, 0, 0xfff);
            if (page.FNum == f) return;

            page.FNum = f;
        }

        private void SetSsgVolume(partPage page, MML mml)
        {
            int vol = page.volume;

            if (page.envelopeMode)
            {
                vol = 0;
                if (page.envIndex != -1)
                {
                    vol = page.envVolume - (15 - page.volume);
                }
                else
                {
                    page.keyOn = false;
                }
            }

            for (int lfo = 0; lfo < 4; lfo++)
            {
                if (!page.lfo[lfo].sw) continue;
                if (page.lfo[lfo].type != eLfoType.Tremolo) continue;

                vol += page.lfo[lfo].value + page.lfo[lfo].phase;
            }

            if (page.varpeggioMode)
            {
                vol += page.varpDelta;
            }

            page.beforeVolume = Common.CheckRange(vol, 0, 15);

        }

        public override void SetToneDoubler(partPage page, MML mml)
        {
        }

        public override int GetToneDoublerShift(partPage page, int octave, char noteCmd, int shift)
        {
            return 0;
        }

        public override int GetFNum(partPage page, MML mml, int octave, char cmd, int shift, int pitchShift)
        {
            return GetSsgFNum(page, mml, octave, cmd, shift, pitchShift);
        }

        public override void CmdNoise(partPage page, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 0, 7);
            page.noise = 7 - n;
        }

        public override void CmdHardEnvelope(partPage page, MML mml)
        {
            string cmd = (string)mml.args[0];
            //int n = 0;

            switch (cmd)
            {
                case "EH":
                    page.HardEnvelopeSpeed = (int)mml.args[1];
                    break;
                case "EHON":
                    page.HardEnvelopeSw = true;
                    break;
                case "EHOF":
                    page.HardEnvelopeSw = false;
                    break;
                case "EHT":
                    page.HardEnvelopeType = (int)mml.args[1];
                    break;
            }
        }

        public override void CmdLoopExtProc(partPage page, MML mml)
        {
            page.spg.freq = -1;
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

            if (type == 'I')
            {
                msgBox.setErrMsg(msg.get("E15001"), mml.line.Lp);
                return;
            }

            if (type == 'T')
            {
                msgBox.setErrMsg(msg.get("E15002"), mml.line.Lp);
                return;
            }

            if (!page.pcm)
            {
                SetEnvelopParamFromInstrument(page, n, re, mml);
                SetDummyData(page, mml);
                return;
            }

            if (re) n = page.instrument + n;
            n = Common.CheckRange(n, 0, 255);

            if (page.instrument == n) return;

            if (!parent.instPCM.ContainsKey(n))
            {
                msgBox.setErrMsg(string.Format(msg.get("E12005"), n), mml.line.Lp);
                return;
            }

            if (parent.instPCM[n].Item2.chip != enmChipType.SN76489)
            {
                msgBox.setErrMsg(string.Format(msg.get("E12006"), n), mml.line.Lp);
            }

            page.instrument = n;
            SetDummyData(page, mml);
        }

        public override void MultiChannelCommand(MML mml)
        {
            AUDCTLvalue = 0;

            foreach (partWork pw in lstPartWork)
            {
                byte c = 0;
                byte f = 0;

                partPage page = pw.cpg;
                if (page.keyOn)
                {
                    c |= (byte)page.beforeVolume;//.volume;
                }
                else
                {
                }

                c |= (byte)(page.noise << 5);

                if (page.beforeFNum != page.FNum)
                {
                    page.beforeFNum = page.FNum;
                    parent.OutData(mml, port[0], (byte)(AUDF1 + page.ch * 2), (byte)page.FNum);
                }
                if (page.beforeTL[0] != c)
                {
                    page.beforeTL[0] = c;
                    parent.OutData(mml, port[0], (byte)(AUDC1 + page.ch * 2), c);
                }

                if (page.HardEnvelopeSw)
                {
                    AUDCTLvalue |= (byte)((page.ch == 0 || page.ch == 2) ? 0x04 : ((page.ch == 1 || page.ch == 3) ? 0x02 : 0x00));
                    if (page.HardEnvelopeType != page.hardLfoAMD)
                    {
                        page.hardLfoAMD = page.HardEnvelopeType;
                        switch (page.ch)
                        {
                            case 0://ch1 のときはch3のfnumを変更
                                parent.OutData(mml, port[0], AUDF3, (byte)page.HardEnvelopeType);
                                break;
                            case 1://ch2 のときはch4のfnumを変更
                                parent.OutData(mml, port[0], AUDF4, (byte)page.HardEnvelopeType);
                                break;
                            case 2://ch3 のときはch1のfnumを変更
                                parent.OutData(mml, port[0], AUDF1, (byte)page.HardEnvelopeType);
                                break;
                            case 3://ch4 のときはch2のfnumを変更
                                parent.OutData(mml, port[0], AUDF2, (byte)page.HardEnvelopeType);
                                break;
                        }
                    }
                }
            }


            if (AUDCTLvalue != AUDCTLvalueBefore)
            {
                //Bit 7          Bit 6      Bit 5      Bit 4     Bit 3     Bit 2     Bit 1     Bit 0
                //17 vs 9 Poly   CH1 1.79   CH3 1.79   CH2 + 1   CH4 + 3   FI1 + 3   FI2 + 4   64 vs 15 kHz
                parent.OutData(mml, port[0], AUDCTL, AUDCTLvalue);
                AUDCTLvalueBefore = AUDCTLvalue;
            }
        }

    }
}
