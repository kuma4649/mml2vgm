using Corex64;
using System.Collections.Generic;

namespace mml2vgmIDEx64.Driver.ZGM.ZgmChip
{
    public abstract class ZgmChip : SoundManager.Chip
    {
        protected ChipRegister chipRegister;
        protected Setting setting;
        protected outDatum[] vgmBuf;

        public string name;
        public Corex64.DefineInfo defineInfo;

        public ZgmChip(int ch) : base(ch)
        {
        }

        public virtual void Setup(int chipIndex, ref uint dataPos, ref Dictionary<int, Driver.ZGM.zgm.RefAction<outDatum, uint>> cmdTable)
        {
            this.Index = chipIndex;
            defineInfo = new Corex64.DefineInfo();
            defineInfo.length = vgmBuf[dataPos + 0x03].val;
            defineInfo.chipIdentNo = Common.getLE32(vgmBuf, dataPos + 0x4);
            defineInfo.commandNo = (int)Common.getLE16(vgmBuf, dataPos + 0x8);
            defineInfo.clock = (int)Common.getLE32(vgmBuf, dataPos + 0xa);
            defineInfo.option = null;
            if (defineInfo.length > 14)
            {
                defineInfo.option = new byte[defineInfo.length - 14];
                for (int j = 0; j < defineInfo.length - 14; j++)
                {
                    defineInfo.option[j] = vgmBuf[dataPos + 0x0e + j].val;
                }
            }

            dataPos += defineInfo.length;
        }

    }
}
