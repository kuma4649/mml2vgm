using Core;
using System.Collections.Generic;

namespace mml2vgmIDE.Driver.ZGM.ZgmChip
{
    public class SEGAPCM : ZgmChip
    {
        public SEGAPCM(ChipRegister chipRegister, Setting setting, outDatum[] vgmBuf) : base(16)
        {
            this.chipRegister = chipRegister;
            this.setting = setting;
            this.vgmBuf = vgmBuf;

            Use = true;
            Device = EnmZGMDevice.SegaPCM;
            name = "SEGAPCM";
            Model = EnmVRModel.None;
            Number = 0;
            Hosei = 0;
        }

        public override void Setup(int chipIndex, ref uint dataPos, ref Dictionary<int, Driver.ZGM.zgm.RefAction<outDatum, uint>> cmdTable)
        {
            base.Setup(chipIndex, ref dataPos, ref cmdTable);

            if (cmdTable.ContainsKey(defineInfo.commandNo)) cmdTable.Remove(defineInfo.commandNo);
            cmdTable.Add(defineInfo.commandNo, SendPort0);

        }

        private void SendPort0(outDatum od, ref uint vgmAdr)
        {
            chipRegister.SEGAPCMSetRegister(od, Audio.DriverSeqCounter, Index, (int)((vgmBuf[vgmAdr + 0x01].val & 0xFF) | ((vgmBuf[vgmAdr + 0x02].val & 0xFF) << 8)), vgmBuf[vgmAdr + 0x03].val);
            vgmAdr += 4;
        }

    }
}
