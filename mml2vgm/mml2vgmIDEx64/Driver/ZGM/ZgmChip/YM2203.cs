﻿using Corex64;
using System.Collections.Generic;

namespace mml2vgmIDEx64.Driver.ZGM.ZgmChip
{
    public class YM2203 : ZgmChip
    {
        public YM2203(ChipRegister chipRegister, Setting setting, outDatum[] vgmBuf) : base(9)
        {
            this.chipRegister = chipRegister;
            this.setting = setting;
            this.vgmBuf = vgmBuf;

            Use = true;
            Device = EnmZGMDevice.YM2203;
            name = "YM2203";
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
            chipRegister.YM2203SetRegister(od, Audio.DriverSeqCounter, Index, vgmBuf[vgmAdr + 1].val, vgmBuf[vgmAdr + 2].val);
            vgmAdr += 3;
        }

    }
}
