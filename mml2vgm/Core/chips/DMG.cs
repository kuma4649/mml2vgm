using musicDriverInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class DMG : ClsChip
    {
        private const byte NR10 = 0x00;
        private const byte NR11 = 0x01;
        private const byte NR12 = 0x02;
        private const byte NR13 = 0x03;
        private const byte NR14 = 0x04;
        // 0x05       
        private const byte NR21 = 0x06;
        private const byte NR22 = 0x07;
        private const byte NR23 = 0x08;
        private const byte NR24 = 0x09;
        private const byte NR30 = 0x0A;
        private const byte NR31 = 0x0B;
        private const byte NR32 = 0x0C;
        private const byte NR33 = 0x0D;
        private const byte NR34 = 0x0E;
        // 0x0F       
        private const byte NR41 = 0x10;
        private const byte NR42 = 0x11;
        private const byte NR43 = 0x12;
        private const byte NR44 = 0x13;
        private const byte NR50 = 0x14;
        private const byte NR51 = 0x15;
        private const byte NR52 = 0x16;
        // 0x17 - 0x1F
        private const byte AUD3W0 = 0x20;
        private const byte AUD3W1 = 0x21;
        private const byte AUD3W2 = 0x22;
        private const byte AUD3W3 = 0x23;
        private const byte AUD3W4 = 0x24;
        private const byte AUD3W5 = 0x25;
        private const byte AUD3W6 = 0x26;
        private const byte AUD3W7 = 0x27;
        private const byte AUD3W8 = 0x28;
        private const byte AUD3W9 = 0x29;
        private const byte AUD3WA = 0x2A;
        private const byte AUD3WB = 0x2B;
        private const byte AUD3WC = 0x2C;
        private const byte AUD3WD = 0x2D;
        private const byte AUD3WE = 0x2E;
        private const byte AUD3WF = 0x2F;

        public DMG(ClsVgm parent, int chipID, string initialPartName, string stPath, int chipNumber) : base(parent, chipID, initialPartName, stPath, chipNumber)
        {
            _chipType = enmChipType.DMG;
            _Name = "DMG";
            _ShortName = "DMG";
            _ChMax = 4;
            _canUsePcm = true;
            _canUsePI = false;
            ChipNumber = chipNumber;

            Frequency = 4194304;
            port = new byte[][] { new byte[] { 0xb3 } };
            vgmHeaderPos = 0x80;
            DataBankID = 0xb;
            int c = 0;

            if (string.IsNullOrEmpty(initialPartName)) return;

            Dictionary<string, List<double>> dic = MakeFNumTbl();
            if (dic != null)
            {
                c = 0;
                FNumTbl = new int[1][] { new int[96] };
                foreach (double v in dic["FNUM_00"])
                {
                    FNumTbl[0][c++] = (int)v;
                    if (c == FNumTbl[0].Length) break;
                }
                if (dic.ContainsKey("MASTERCLOCK"))
                {
                    Frequency = (int)dic["MASTERCLOCK"][0];
                }
            }

            Ch = new ClsChannel[ChMax];
            SetPartToCh(Ch, initialPartName);
            c = 0;
            foreach (ClsChannel ch in Ch)
            {
                ch.Type = c < 2 ? enmChannelType.Pulse : (c == 2 ? enmChannelType.WaveForm : enmChannelType.Noise);
                ch.chipNumber = chipID == 1;
                ch.MaxVolume = 15;
                c++;
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

            pcmDataInfo[0].totalHeaderLength = pcmDataInfo[0].totalBuf.Length;
            pcmDataInfo[0].totalHeadrSizeOfDataPtr = (parent.ChipCommandSize == 2) ? 4 : 3;
        }

        public override void InitPart(partWork pw)
        {
            foreach (partPage pg in pw.pg)
            {
                pg.volume = 15;
                pg.MaxVolume = 15;
                pg.port = port;
            }

        }

        public override void InitChip()
        {
            if (!use) return;

            for (int ch = 0; ch < ChMax; ch++)
            {
                OutKeyOff(null, lstPartWork[ch].cpg);
                lstPartWork[ch].apg.volume = 0;
            }

            //set volume 0
            parent.OutData((MML)null, port[0], new byte[] { NR12, 0x00 });//pls1
            parent.OutData((MML)null, port[0], new byte[] { NR22, 0x00 });//pls2
            parent.OutData((MML)null, port[0], new byte[] { NR32, 0x00 });//wf
            parent.OutData((MML)null, port[0], new byte[] { NR42, 0x00 });//noise
            parent.OutData((MML)null, port[0], new byte[] { NR50, 0x00 });//total
            parent.OutData((MML)null, port[0], new byte[] { NR51, 0x00 });//pan
            parent.OutData((MML)null, port[0], new byte[] { NR52, 0x80 });//Sound ON

            //ヘッダの調整
            if (ChipID != 0)
            {
                parent.dat[vgmHeaderPos + 3] = new outDatum(enmMMLType.unknown, null, null, (byte)(parent.dat[vgmHeaderPos + 3].val | 0x40));//use Secondary
            }
        }

        public void OutKeyOn(MML mml, partPage page)
        {
            if (!page.pcm)
            {
                page.keyOn = true;

                SetVolume(page, mml);

                return;
            }

        }

        public void OutKeyOff(MML mml, partPage page)
        {
            page.keyOn = false;
        }
    }
}
