using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mml2vgmIDE.Driver.ZGM.ZgmChip
{
    public class VRC7 : ZgmChip
    {
        public VRC7(ChipRegister chipRegister, Setting setting, outDatum[] vgmBuf) : base(6)
        {
            this.chipRegister = chipRegister;
            this.setting = setting;
            this.vgmBuf = vgmBuf;

            Use = true;
            Device = EnmZGMDevice.VRC7;
            name = "VRC7";
            Model = EnmVRModel.None;
            Number = 0;
            Hosei = 0;
        }
    }
}
