using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mml2vgmIDE.Driver.ZGM.ZgmChip
{
    public class ChipFactory
    {
        public ZgmChip Create(uint chipIdentNo, ChipRegister chipRegister, Setting setting, outDatum[] vgmBuf)
        {
            switch (chipIdentNo)
            {
                case 0x0000_000c:
                    return new YM2612(chipRegister, setting, vgmBuf);
                case 0x0000_002c:
                    return new YM2612(chipRegister, setting, vgmBuf);
                default:
                    throw new ArgumentException();
            }
        }
    }
}
