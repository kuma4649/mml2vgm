using Corex64;
using System.Collections.Generic;

namespace mml2vgmIDEx64.Driver.ZGM.ZgmChip
{
    public class C352 : ZgmChip
    {
        public C352(ChipRegister chipRegister, Setting setting, outDatum[] vgmBuf) : base(32)
        {
            this.chipRegister = chipRegister;
            this.setting = setting;
            this.vgmBuf = vgmBuf;

            Use = true;
            Device = EnmZGMDevice.C352;
            name = "C352";
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
            byte id = (byte)((vgmBuf[vgmAdr + 1].val & 0x80) != 0 ? 1 : 0);
            int adr = (int)((vgmBuf[vgmAdr + 1].val & 0x7f) * 0x100 + (vgmBuf[vgmAdr + 2].val & 0xff));
            int data = (int)((vgmBuf[vgmAdr + 3].val & 0xff) * 0x100 + (vgmBuf[vgmAdr + 4].val & 0xff));
            chipRegister.C352SetRegister(od, Audio.DriverSeqCounter, id, adr, data);
            vgmAdr += 5;

        }

    }
}
