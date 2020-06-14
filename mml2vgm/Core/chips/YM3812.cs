using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class YM3812 : ClsOPL
    {
        public YM3812(ClsVgm parent, int chipID, string initialPartName, string stPath, int chipNumber) : base(parent, chipID, initialPartName, stPath, chipNumber)
        {

            _Name = "YM3812";
            _ShortName = "OPL2";
            _ChMax = 9 + 5;//FM 9ch Rhythm 5ch
            _canUsePcm = false;

            Frequency = 3579545;
            port = new byte[][] { 
                new byte[] { (byte)(chipNumber != 0 ? 0xaa : 0x5a) }
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

        public override void InitChip()
        {

            if (!use) return;

            //OPL2 mode enable
            parent.OutData((MML)null, port[0], 0x01, 0x20);

            //FM Off
            outAllKeyOff(null, lstPartWork[0]);
            rhythmStatus = 0x00;
            beforeRhythmStatus = 0xff;
            connectionSel = 0;
            beforeConnectionSel = -1;

        }

        public override void InitPart(partWork pw)
        {
            foreach (partPage page in pw.pg)
            {
                page.beforeVolume = -1;
                page.volume = 60;
                page.MaxVolume = 63;
                page.beforeEnvInstrument = 0;
                page.envInstrument = 0;
                page.port = port;
                page.mixer = 0;
                page.noise = 0;
                page.pan.val = 3;
                page.Type = enmChannelType.FMOPL;
                page.isOp4Mode = false;
                if (page.ch > 8) page.Type = enmChannelType.RHYTHM;
            }
        }

        protected override void SetInstAtOneOpeWithoutKslTl(MML mml, int opeNum,
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

            parent.OutData(mml, port, (byte)(0x80 + adr), (byte)(((sl & 0xf) << 4) | (rr & 0xf)));
            parent.OutData(mml, port, (byte)(0x60 + adr), (byte)(((ar & 0xf) << 4) | (dr & 0xf)));
            SetInstAtOneOpeAmVibEgKsMl(mml, port, (byte)(0x20 + adr), mt, am, vib, eg, kr);
            parent.OutData(mml, port, (byte)(0xe0 + adr), (byte)(ws & 0x3));
        }

    }
}
