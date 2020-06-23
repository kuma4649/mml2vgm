using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class YM3526 : ClsOPL
    {
        public YM3526(ClsVgm parent, int chipID, string initialPartName, string stPath, int chipNumber) : base(parent, chipID, initialPartName, stPath, chipNumber)
        {

            _Name = "YM3526";
            _ShortName = "OPL";
            _ChMax = 9 + 5;//FM 9ch Rhythm 5ch
            _canUsePcm = false;

            Frequency = 3579545;
            port = new byte[][] {
                new byte[] { (byte)(chipNumber != 0 ? 0xab : 0x5b) }
            };

            if (string.IsNullOrEmpty(initialPartName)) return;

            Dictionary<string, List<double>> dic = MakeFNumTbl();
            if (dic != null)
            {
                int c = 0;
                FNumTbl = new int[1][];
                FNumTbl[0] = new int[13];
                foreach (double v in dic["FNUM_00"])
                {
                    FNumTbl[0][c++] = (int)v;
                    if (c == FNumTbl[0].Length) break;
                }
                FNumTbl[0][FNumTbl[0].Length - 1] = FNumTbl[0][0] * 2;

            }

            Ch = new ClsChannel[ChMax];
            SetPartToCh(Ch, initialPartName);

        }

        protected override void SetInstAtOneOpeWithoutKslTl(partPage page, MML mml, int opeNum,
int ar, int dr, int sl, int rr,
int mt, int am, int vib, int eg,
int kr,
int ws
)
        {
            //portは18operator毎に切り替わる
            byte[] port = this.port[opeNum / 18];

            // % 18       ... port毎のoperator番号を得る --- (1)
            // / 6 ) * 8  ... (1) に対応するアドレスは6opeごとに8アドレス毎に分けられ、
            // % 6        ...                         0～5アドレスに割り当てられている
            int adr = ((opeNum % 18) / 6) * 8 + (opeNum % 6);

            ////slot1かslot2を求める
            //// % 6        ... slotは6opeの範囲で0か1を繰り返す
            //// / 3        ... slotは3ope毎に0か1を繰り返す
            //int slot = (opeNum % 6) / 3;

            SOutData(page,mml, port, (byte)(0x80 + adr), (byte)(((sl & 0xf) << 4) | (rr & 0xf)));
            SOutData(page,mml, port, (byte)(0x60 + adr), (byte)(((ar & 0xf) << 4) | (dr & 0xf)));
            SetInstAtOneOpeAmVibEgKsMl(page,mml, port, (byte)(0x20 + adr), mt, am, vib, eg, kr);
        }
    }
}
