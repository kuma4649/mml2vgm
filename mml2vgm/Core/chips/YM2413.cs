using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class YM2413 : ClsChip
    {
        protected int[][] _FNumTbl = new int[1][] {
            new int[13]
            //new int[] {
            //// OPLL(FM) : Fnum = 9 * 2^(22-B) * ftone / M       ftone:Hz M:MasterClock B:Block
            ////   c    c+     d    d+     e     f    f+     g    g+     a    a+     b    >c
            // 0x289,0x2af,0x2d8,0x303,0x331,0x362,0x395,0x3cc,0x405,0x443,0x484,0x4c8,0x289*2
            //}
        };

        public YM2413(ClsVgm parent, int chipID, string initialPartName, string stPath, bool isSecondary) : base(parent, chipID, initialPartName, stPath, isSecondary)
        {

            _Name = "YM2413";
            _ShortName = "OPLL";
            _ChMax = 14; // FM 9ch + Rhythm 5ch
            _canUsePcm = false;

            Frequency = 3579545;

            MakeFNumTbl();
            Ch = new ClsChannel[ChMax];
            SetPartToCh(Ch, initialPartName);
            foreach (ClsChannel ch in Ch)
            {
                ch.Type = enmChannelType.FMOPL;
                ch.isSecondary = chipID == 1;
            }

        }

        public override void InitPart(ref partWork pw)
        {
            pw.slots = 0x3;
            pw.volume = 127;
            pw.MaxVolume = 127;
            pw.port0 = (byte)(0x1 | (pw.isSecondary ? 0xa0 : 0x50));
            pw.port1 = 0xff;
            pw.mixer = 0;
            pw.noise = 0;
        }

        public override void InitChip()
        {

            if (!use) return;

            //FM Off
            outYM2413AllKeyOff(lstPartWork[0]);

            if (ChipID != 0) parent.dat[0x13] |= 0x40;//use Secondary(YM2413 OPLL)

        }

        public void outYM2413SetAdr00_01(partWork pw,byte adr, bool AM,bool VIB,bool EG,bool KS, int mul)
        {
            parent.OutData(
                pw.port0
                , (byte)(adr & 1)
                , (byte)((AM ? 0x80 : 0) + (VIB ? 0x40 : 0) + (EG ? 0x20 : 0) + (KS ? 0x10 : 0) + (mul & 0xf))
                );
        }

        public void outYM2413AllKeyOff(partWork pw)
        {
            //Rhythm Off
            parent.OutData(pw.port0, 0x0e, 0);
            for (byte adr = 0; adr < 9; adr++)
            {
                //Ch Off
                parent.OutData(pw.port0, (byte)(0x20 + adr), 0);
                parent.OutData(pw.port0, (byte)(0x30 + adr), 0);
            }
        }

        public void outYM2413SetInstrument(partWork pw, int n)
        {
            pw.instrument = n;

            if (!parent.instFM.ContainsKey(n))
            {
                msgBox.setWrnMsg(string.Format("未定義の音色(@{0})を指定しています。", n), pw.getSrcFn(), pw.getLineNumber());
                return;
            }

            for (byte ope = 0; ope < 2; ope++)
            {
                outYM2413SetAdr00_01(pw, ope
                    , parent.instFM[n][ope * 11 + 7] != 0 //AM
                    , parent.instFM[n][ope * 11 + 8] != 0 //VIB
                    , parent.instFM[n][ope * 11 + 9] != 0 //EG
                    , parent.instFM[n][ope * 11 + 10] != 0 //KS
                    , parent.instFM[n][ope * 11 + 6] & 0xf //MT
                    );
                parent.OutData(pw.port0, (byte)(0x4 + ope), (byte)((
                    (parent.instFM[n][ope * 11 + 1] & 0xf) << 4) //AR
                    | (parent.instFM[n][ope * 11 + 2] & 0xf) // DR
                    ));
                parent.OutData(pw.port0, (byte)(0x6 + ope), (byte)((
                    (parent.instFM[n][ope * 11 + 3] & 0xf) << 4) //SL
                    | (parent.instFM[n][ope * 11 + 4] & 0xf) // RR
                    ));
            }
            parent.OutData(pw.port0, (byte)(0x2), (byte)((
                (parent.instFM[n][0 * 11 + 5] & 0x3) << 6)  //KL(M)
                | (parent.instFM[n][23] & 0x3f) //TL
                ));
            parent.OutData(pw.port0, (byte)(0x3), (byte)((
                (parent.instFM[n][1 * 11 + 5] & 0x3) << 6) //KL(C)
                | (parent.instFM[n][0 * 11 + 11] != 0 ? 0x08 : 0) // DT(M)
                | (parent.instFM[n][1 * 11 + 11] != 0 ? 0x10 : 0) // DT(C)
                | (parent.instFM[n][24] & 0x07) //FB
                ));

            pw.op1ml = parent.instFM[n][0 * 11 + 5];
            pw.op2ml = parent.instFM[n][1 * 11 + 5];
            pw.op1dt2 = 0;
            pw.op2dt2 = 0;

        }

        public void outYM2413SetInstVol(partWork pw,int inst,int vol)
        {
            pw.envInstrument = inst & 0xf;
            pw.volume = vol & 0xf;

            parent.OutData(pw.port0
                , (byte)(0x30 + pw.ch)
                , (byte)((pw.envInstrument << 4) | (15 - pw.volume))
                );
        }



        public override void CmdInstrument(partWork pw, MML mml)
        {
            char type = (char)mml.args[0];
            int n = (int)mml.args[1];

            if (type == 'T')
            {
                msgBox.setErrMsg("Tone DoublerはOPN,OPM音源以外では使用できません。", pw.getSrcFn(), pw.getLineNumber());
                return;
            }

            if (pw.getChar() == 'E')
            {
                n = SetEnvelopParamFromInstrument(pw, n,mml);
                return;
            }

            if (pw.getChar() == 'I')
            {
                n = Common.CheckRange(n, 0, 15);
                if (pw.envInstrument != n)
                {
                    ((YM2413)pw.chip).outYM2413SetInstVol(pw, n, pw.volume); //INSTをnにセット
                }
                return;
            }

            n = Common.CheckRange(n, 0, 255);
            if (pw.instrument == n) return;

            ((YM2413)pw.chip).outYM2413SetInstrument(pw, n); //音色のセット
            ((YM2413)pw.chip).outYM2413SetInstVol(pw, 0, pw.volume); //INSTを0にセット
            parent.OutData(pw.port0, 0x20, 0x19);

        }

        public override void SetPCMDataBlock()
        {
            //実装不要
        }

        public override void SetToneDoubler(partWork pw)
        {
            //実装不要
        }

    }
}
