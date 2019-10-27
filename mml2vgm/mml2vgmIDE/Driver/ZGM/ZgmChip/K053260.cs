using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mml2vgmIDE.Driver.ZGM.ZgmChip
{
    public class K053260 : ZgmChip
    {
        public K053260(ChipRegister chipRegister, Setting setting, outDatum[] vgmBuf)
        {
            this.chipRegister = chipRegister;
            this.setting = setting;
            this.vgmBuf = vgmBuf;

            Use = true;
            Device = EnmZGMDevice.K053260;
            name = "K053260";
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
            chipRegister.K053260SetRegister(od, Audio.DriverSeqCounter, Index, vgmBuf[vgmAdr + 1].val, vgmBuf[vgmAdr + 2].val);
            vgmAdr += 3;
        }

    }
}
