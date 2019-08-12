using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class K053260 : ClsChip
    {
        public K053260(ClsVgm parent, int chipID, string initialPartName, string stPath, bool isSecondary) : base(parent, chipID, initialPartName, stPath, isSecondary)
        {
            _chipType = enmChipType.K053260;
            _Name = "K053260";
            _ShortName = "K53";
            _ChMax = 4;
            _canUsePcm = true;
            _canUsePI = false;
            IsSecondary = isSecondary;
            Frequency = 3579545;

            Ch = new ClsChannel[ChMax];
            SetPartToCh(Ch, initialPartName);
            foreach (ClsChannel ch in Ch)
            {
                ch.Type = enmChannelType.PCM;
                ch.isSecondary = chipID == 1;
                ch.MaxVolume = 255;
            }
        }

        public override void InitPart(ref partWork pw)
        {
            pw.MaxVolume = 255;
            pw.volume = pw.MaxVolume;
            pw.port0 = 0xba;
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

            if (IsSecondary)
            {
                parent.dat[0xaf] = new outDatum(enmMMLType.unknown, null, null, (byte)(parent.dat[0xaf].val | 0x40));
            }
        }

        public override void StorePcm(Dictionary<int, clsPcm> newDic, KeyValuePair<int, clsPcm> v, byte[] buf, bool is16bit, int samplerate, params object[] option)
        {
        }

        public override void StorePcmRawData(clsPcmDatSeq pds, byte[] buf, bool isRaw, bool is16bit, int samplerate, params object[] option)
        {
        }

    }
}
