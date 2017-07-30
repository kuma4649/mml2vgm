using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mml2vgm
{
    public class YM2608 : clsChip
    {
        protected int[][] _FNumTbl = new int[2][] {
            new int[] {
            // OPNA(FM) : TP = (144 * ftone * (2^20) / M) / (2^(B-1))       32:Divider 2:OPNA 
            //   c    c+     d    d+     e     f    f+     g    g+     a    a+     b    >c
             0x289,0x2af,0x2d8,0x303,0x331,0x362,0x395,0x3cc,0x405,0x443,0x484,0x4c8,0x289*2
            },
            new int[] {
            // OPNA(SSG) : TP = M / (ftone * 32 * 2)       32:Divider 2:OPNA 
            //   c    c+     d    d+     e     f    f+     g    g+     a    a+     b
             0xEE8,0xE12,0xD48,0xC89,0xBD5,0xB2B,0xA8A,0x9F3,0x964,0x8DD,0x85E,0x7E6
            ,0x774,0x709,0x6A4,0x645,0x5EA,0x595,0x545,0x4FA,0x4B2,0x46F,0x42F,0x3F3
            ,0x3BA,0x384,0x352,0x322,0x2F5,0x2CB,0x2A3,0x27D,0x259,0x237,0x217,0x1F9
            ,0x1DD,0x1C2,0x1A9,0x191,0x17B,0x165,0x151,0x13E,0x12D,0x11C,0x10C,0x0FD
            ,0x0EF,0x0E1,0x0D4,0x0C9,0x0BD,0x0B3,0x0A9,0x09F,0x096,0x08E,0x086,0x07E
            ,0x077,0x071,0x06A,0x064,0x05F,0x059,0x054,0x050,0x04B,0x047,0x043,0x03F
            ,0x03C,0x038,0x035,0x032,0x02F,0x02D,0x02A,0x028,0x026,0x023,0x021,0x020
            ,0x01E,0x01C,0x01B,0x019,0x018,0x016,0x015,0x014,0x013,0x012,0x011,0x010
            }
        };

        public byte SSGKeyOn = 0x3f;

        public byte[] pcmData = null;

        public int rhythm_TotalVolume = 63;
        public int rhythm_beforeTotalVolume = -1;
        public int rhythm_MAXTotalVolume = 63;
        public byte rhythm_KeyOn = 0;
        public byte rhythm_KeyOff = 0;

        public YM2608(int chipID, string initialPartName) : base(chipID, initialPartName)
        {

            _Name = "YM2608";
            _ShortName = "OPNA";
            _ChMax = 19;
            _canUsePcm = true;
            FNumTbl = _FNumTbl;

            Frequency = 7987200;
            makeFNumTbl();
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

            Ch[12].Type = enmChannelType.RHYTHM;
            Ch[13].Type = enmChannelType.RHYTHM;
            Ch[14].Type = enmChannelType.RHYTHM;
            Ch[15].Type = enmChannelType.RHYTHM;
            Ch[16].Type = enmChannelType.RHYTHM;
            Ch[17].Type = enmChannelType.RHYTHM;

            Ch[18].Type = enmChannelType.ADPCM;

        }
    }
}
