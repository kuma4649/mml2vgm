﻿using Corex64;
using mml2vgmIDEx64;

namespace SoundManager
{
    public class PackData
    {
        /// <summary>
        /// Emuチップ / 実チップ
        /// </summary>
        public Chip Chip = new Chip(1);

        /// <summary>
        /// データの種類
        /// </summary>
        public EnmDataType Type;

        /// <summary>
        /// データのアドレス
        /// </summary>
        public int Address;

        /// <summary>
        /// データ
        /// </summary>
        public int Data;

        /// <summary>
        /// 複数データ
        /// </summary>
        public object ExData;

        public outDatum od;

        public PackData()
        {

        }

        public PackData(outDatum od, Chip Chip, EnmDataType Type, int Address, int Data, object ExData)
        {
            this.od = od;
            this.Chip.Move(Chip);
            this.Type = Type;
            this.Address = Address;
            this.Data = Data;
            this.ExData = ExData;
        }

        public void Copy(PackData pack)
        {
            Chip.Move(pack.Chip);
            Type = pack.Type;
            Address = pack.Address;
            Data = pack.Data;
            ExData = pack.ExData;
        }

        public void Copy(outDatum od, Chip Chip, EnmDataType Type, int Address, int Data, object ExData)
        {
            this.od = null;
            if (od != null)
            {
                this.od = new outDatum();
                this.od.args = od.args;
                this.od.linePos = od.linePos;
                this.od.type = od.type;
                this.od.val = od.val;
            }
            this.Chip.Move(Chip);
            this.Type = Type;
            this.Address = Address;
            this.Data = Data;
            this.ExData = ExData;
        }
    }

    public class CntPackData
    {
        public CntPackData prev;
        public CntPackData next;

        public long Counter;

        public PackData pack = new PackData();
    }

}
