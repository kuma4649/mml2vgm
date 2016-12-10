using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mml2vgm
{
    public class clsChip
    {

        public string Name;
        public string ShortName;
        public enmChipType Type;
        public clsChannel[][] Ch;
        public int ChMax;
        public int Frequency;

        public int[] partStartCh = new int[2];
        public bool[] use = new bool[2];

        public bool ChannelNameContains(int chipNum,string name)
        {
            foreach (clsChannel c in Ch[chipNum])
            {
                if (c.Name == name) return true;
            }
            return false;
        }

    }

    public class clsChannel
    {
        public string Name;
        public enmChannelType Type;
        public bool isSecondary;
    }

    public enum enmChannelType
    {
        Multi,
        FMOPN,
        FMOPNex,
        FMOPM,
        DCSG,
        PCM,
        ADPCM,
        RHYTHM,
        FMPCM,
        DCSGNOISE
    }

    public enum enmChipType
    {
        YM2151,
        YM2203,
        YM2608,
        YM2610B,
        YM2612,
        SN76489,
        RF5C164
    }
}
