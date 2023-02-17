using Core;
using System.Collections.Generic;

namespace mml2vgmIDE.Driver.ZGM.ZgmChip
{
    public class YM2608 : ZgmChip
    {
        public YM2608(ChipRegister chipRegister, Setting setting, outDatum[] vgmBuf) : base(6 + 3 + 3 + 6 + 1)
        {
            this.chipRegister = chipRegister;
            this.setting = setting;
            this.vgmBuf = vgmBuf;

            Use = true;
            Device = EnmZGMDevice.YM2608;
            name = "YM2608";
            Model = EnmVRModel.None;
            Number = 0;
            Hosei = 0;
        }

        public override void Setup(int chipIndex, ref uint dataPos, ref Dictionary<int, Driver.ZGM.zgm.RefAction<outDatum, uint>> cmdTable)
        {
            base.Setup(chipIndex, ref dataPos, ref cmdTable);

            if (cmdTable.ContainsKey(defineInfo.commandNo)) cmdTable.Remove(defineInfo.commandNo);
            cmdTable.Add(defineInfo.commandNo, SendPort0);

            if (cmdTable.ContainsKey(defineInfo.commandNo + 1)) cmdTable.Remove(defineInfo.commandNo + 1);
            cmdTable.Add(defineInfo.commandNo + 1, SendPort1);

        }

        private void SendPort0(outDatum od, ref uint vgmAdr)
        {
            chipRegister.YM2608SetRegister(od, Audio.DriverSeqCounter, Index, 0, vgmBuf[vgmAdr + 1].val, vgmBuf[vgmAdr + 2].val);
            vgmAdr += 3;
        }

        private void SendPort1(outDatum od, ref uint vgmAdr)
        {
            chipRegister.YM2608SetRegister(od, Audio.DriverSeqCounter, Index, 1, vgmBuf[vgmAdr + 1].val, vgmBuf[vgmAdr + 2].val);
            vgmAdr += 3;
        }

    }
}
