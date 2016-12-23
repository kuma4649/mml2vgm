using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mml2vgm
{
    public class YM2610B : clsChip
    {
        public byte SSGKeyOn = 0x3f;

        public byte[] pcmDataA = null;
        public byte[] pcmDataB = null;

        public int adpcmA_TotalVolume = 63;
        public int adpcmA_beforeTotalVolume = -1;
        public int adpcmA_MAXTotalVolume = 63;
        public byte adpcmA_KeyOn = 0;
        public byte adpcmA_KeyOff = 0;

        public YM2610B(int chipID, string initialPartName) : base(chipID, initialPartName)
        {

            _Name = "YM2610B";
            _ShortName = "OPNB";
            _ChMax = 19;

            Frequency = 8000000;
            Ch = new clsChannel[ChMax];
            setPartToCh(Ch, initialPartName);
            foreach (clsChannel ch in Ch)
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

        }
    }
}
