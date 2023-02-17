using Core;
using System.Collections.Generic;

namespace mml2vgmIDE.Driver.ZGM.ZgmChip
{
    public class SN76489 : ZgmChip
    {
        public SN76489(ChipRegister chipRegister, Setting setting, outDatum[] vgmBuf) : base(4)
        {
            this.chipRegister = chipRegister;
            this.setting = setting;
            this.vgmBuf = vgmBuf;

            Use = true;
            Device = EnmZGMDevice.SN76489;
            name = "SN76489";
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
            chipRegister.SN76489SetRegister(od, Audio.DriverSeqCounter, Index, vgmBuf[vgmAdr + 1].val);
            vgmAdr += 2;
        }

        private void SendPort1(outDatum od, ref uint vgmAdr)
        {
            chipRegister.SN76489SetRegisterGGpanning(od, Audio.DriverSeqCounter, Index, vgmBuf[vgmAdr + 1].val);
            vgmAdr += 2;
        }

    }
}