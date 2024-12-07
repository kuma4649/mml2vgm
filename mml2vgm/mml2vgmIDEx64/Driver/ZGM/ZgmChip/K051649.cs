using Corex64;
using System.Collections.Generic;

namespace mml2vgmIDEx64.Driver.ZGM.ZgmChip
{
    public class K051649 : ZgmChip
    {
        public K051649(ChipRegister chipRegister, Setting setting, outDatum[] vgmBuf) : base(5)
        {
            this.chipRegister = chipRegister;
            this.setting = setting;
            this.vgmBuf = vgmBuf;

            Use = true;
            Device = EnmZGMDevice.K051649;
            name = "K051649";
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
            int scc1_port = vgmBuf[vgmAdr + 1].val & 0x7f;
            byte scc1_offset = vgmBuf[vgmAdr + 2].val;
            byte rDat = vgmBuf[vgmAdr + 3].val;
            byte scc1_chipid = (byte)((vgmBuf[vgmAdr + 1].val & 0x80) != 0 ? 1 : 0);
            chipRegister.K051649SetRegister(od, Audio.DriverSeqCounter, scc1_chipid, (int)((scc1_port << 1) | 0x00), scc1_offset);
            chipRegister.K051649SetRegister(od, Audio.DriverSeqCounter, scc1_chipid, (int)((scc1_port << 1) | 0x01), rDat);

            //chipRegister.K051649SetRegister(od, Audio.DriverSeqCounter, Index, vgmBuf[vgmAdr + 1].val, vgmBuf[vgmAdr + 2].val);
            vgmAdr += 4;
        }

    }
}
