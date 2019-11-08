using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mml2vgmIDE.Driver.ZGM.ZgmChip
{
    public class C140 : ZgmChip
    {
        public C140(ChipRegister chipRegister, Setting setting, outDatum[] vgmBuf) : base(24)
        {
            this.chipRegister = chipRegister;
            this.setting = setting;
            this.vgmBuf = vgmBuf;

            Use = true;
            Device = EnmZGMDevice.C140;
            name = "C140";
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
            int adr = (vgmBuf[vgmAdr + 1].val & 0x7f) * 0x100 + (vgmBuf[vgmAdr + 2].val & 0xff);
            byte data = vgmBuf[vgmAdr + 3].val;
            chipRegister.C140SetRegister(od, Audio.DriverSeqCounter, id, adr, data);
            vgmAdr += 4;

        }

    }
}
