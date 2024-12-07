using Corex64;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mml2vgmIDEx64.Driver.ZGM.ZgmChip
{
    public class Gigatron : ZgmChip
    {
        public Gigatron(ChipRegister chipRegister, Setting setting, outDatum[] vgmBuf) : base(4)
        {
            this.chipRegister = chipRegister;
            this.setting = setting;
            this.vgmBuf = vgmBuf;

            Use = true;
            Device = EnmZGMDevice.Gigatron;
            name = "Gigatron";
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
            chipRegister.GigatronSetRegister(od,
                Audio.DriverSeqCounter,
                Index,
                vgmBuf[vgmAdr + 1].val | (vgmBuf[vgmAdr + 2].val << 8),
                vgmBuf[vgmAdr + 3].val
                );
            vgmAdr += 4;
        }


    }
}
