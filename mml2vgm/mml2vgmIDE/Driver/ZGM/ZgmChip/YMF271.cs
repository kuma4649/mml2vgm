using Core;
using System.Collections.Generic;

namespace mml2vgmIDE.Driver.ZGM.ZgmChip
{
    public class YMF271 : ZgmChip
    {
        public YMF271(ChipRegister chipRegister, Setting setting, outDatum[] vgmBuf) : base(18+5+24)
        {
            this.chipRegister = chipRegister;
            this.setting = setting;
            this.vgmBuf = vgmBuf;

            Use = true;
            Device = EnmZGMDevice.YMF271;
            name = "YMF271";
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
            chipRegister.YMF271SetRegister(
                od
                ,Audio.DriverSeqCounter
                , Index
                , (vgmBuf[vgmAdr].val & 0x80) == 0 ? 0 : 1
                , vgmBuf[vgmAdr + 1].val
                , vgmBuf[vgmAdr + 2].val);
            vgmAdr += 3;
        }

    }
}
