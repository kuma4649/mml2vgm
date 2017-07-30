using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mml2vgm
{
    public class YM2610B : clsChip
    {
        protected int[][] _FNumTbl = new int[2][] {
            new int[] {
            // OPNB(FM) : TP = (144 * ftone * (2^20) / M) / (2^(B-1))       32:Divider 2:OPNB 
            //   c    c+     d    d+     e     f    f+     g    g+     a    a+     b    >c
             0x269,0x28e,0x2b4,0x2de,0x309,0x337,0x368,0x39c,0x3d3,0x40e,0x44b,0x48d,0x4d2
            },
            new int[] {
            // OPNB(SSG) : TP = M / (ftone * 32 * 2)       32:Divider 2:OPNB
            //   c    c+     d    d+     e     f    f+     g    g+     a    a+     b
             0xEEE,0xE18,0xD4D,0xC8E,0xBDA,0xB30,0xA8F,0x9F7,0x968,0x8E1,0x861,0x7E9
            ,0x777,0x70C,0x6A7,0x647,0x5ED,0x598,0x547,0x4FC,0x4B4,0x470,0x431,0x3F4
            ,0x3BC,0x386,0x353,0x324,0x2F6,0x2CC,0x2A4,0x27E,0x25A,0x238,0x218,0x1FA
            ,0x1DE,0x1C3,0x1AA,0x192,0x17B,0x166,0x152,0x13F,0x12D,0x11C,0x10C,0x0FD
            ,0x0EF,0x0E1,0x0D5,0x0C9,0x0BE,0x0B3,0x0A9,0x09F,0x096,0x08E,0x086,0x07F
            ,0x077,0x071,0x06A,0x064,0x05F,0x059,0x054,0x050,0x04B,0x047,0x043,0x03F
            ,0x03C,0x038,0x035,0x032,0x02F,0x02D,0x02A,0x028,0x026,0x024,0x022,0x020
            ,0x01E,0x01C,0x01B,0x019,0x018,0x016,0x015,0x014,0x013,0x012,0x011,0x010
            }
        };

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
            _canUsePcm = true;
            FNumTbl = _FNumTbl;

            Frequency = 8000000;
            //makeFNumTbl();
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
