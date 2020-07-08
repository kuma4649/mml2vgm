using Core;
using System.Collections.Generic;

namespace mml2vgmIDE.Driver.ZGM.ZgmChip
{
    public class Conductor : ZgmChip
    {

        public Conductor(ChipRegister chipRegister, Setting setting, outDatum[] vgmBuf) : base(2)
        {
            this.chipRegister = chipRegister;
            this.setting = setting;
            this.vgmBuf = vgmBuf;

            Use = true;
            Device = EnmZGMDevice.Conductor;
            name = "CONDUCTOR";
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
            vgmAdr += 3;
        }

    }
}
