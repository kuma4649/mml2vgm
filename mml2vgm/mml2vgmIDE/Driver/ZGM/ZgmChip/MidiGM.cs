using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mml2vgmIDE.Driver.ZGM.ZgmChip
{
    public class MidiGM:ZgmChip
    {
        public MidiGM(ChipRegister chipRegister, Setting setting, outDatum[] vgmBuf)
        {
            this.chipRegister = chipRegister;
            this.setting = setting;
            this.vgmBuf = vgmBuf;

            Use = true;
            Device = EnmDevice.MIDIGM;
            name = "GeneralMIDI";
            Model = EnmModel.None;
            Number = 0;
            Hosei = 0;
        }
    }
}
