using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class K051649 : ClsChip
    {
        private byte keyOnStatus = 0;
        private byte keyOnStatusOld = 0;

        public K051649(ClsVgm parent, int chipID, string initialPartName, string stPath, int chipNumber) : base(parent, chipID, initialPartName, stPath, chipNumber)
        {
            _chipType = enmChipType.K051649;
            _Name = "K051649";
            _ShortName = "SCC";
            _ChMax = 5;
            _canUsePcm = false;
            _canUsePI = false;
            ChipNumber = chipNumber;

            Frequency = 1789772;
            port =new byte[][] { new byte[] { 0xd2 } };

            if (string.IsNullOrEmpty(initialPartName)) return;

            Dictionary<string, List<double>> dic = MakeFNumTbl();
            if (dic != null)
            {
                int c = 0;
                FNumTbl = new int[1][] { new int[96] };
                foreach (double v in dic["FNUM_00"])
                {
                    FNumTbl[0][c++] = (int)v;
                    if (c == FNumTbl[0].Length) break;
                }
            }

            Ch = new ClsChannel[ChMax];
            SetPartToCh(Ch, initialPartName);
            foreach (ClsChannel ch in Ch)
            {
                ch.Type = enmChannelType.WaveForm;
                ch.chipNumber = chipID == 1;
                ch.MaxVolume = 15;
            }

            Envelope = new Function();
            Envelope.Max = 15;
            Envelope.Min = 0;

        }

        public override void InitChip()
        {
            if (!use) return;

            //SCC1 mode
            //isK052539 = false;

            //keyOnOff : 0
            OutK051649Port(null,port[0],ChipNumber, 3, 0, 0);
            keyOnStatus = 0;
            keyOnStatusOld = 0;

            for (int i = 0; i < _ChMax; i++)
            {
                //freq : 0
                OutK051649Port(null, port[0], ChipNumber, 1, (byte)(i * 2 + 0), 0);
                OutK051649Port(null, port[0], ChipNumber, 1, (byte)(i * 2 + 1), 0);

                //volume : 0
                OutK051649Port(null, port[0], ChipNumber, 2, (byte)i, 0);

                //WaveForm : all 0
                if (parent.info.isK052539 || i < 4) //K051の場合は4Ch分の初期化を行う
                {
                    for (int j = 0; j < 32; j++)
                    {
                        OutK051649Port(null, port[0], ChipNumber, (byte)(parent.info.isK052539 ? 4 : 0), (byte)(i * 32 + j), 0);
                    }
                }
            }

            if (ChipID != 0 && parent.info.format != enmFormat.ZGM)
            {
                parent.dat[0x9f] = new outDatum(enmMMLType.unknown, null, null, (byte)(parent.dat[0x9f].val | 0x40));//use Secondary
            }
        }

        public override void InitPart(partWork pw)
        {
            pw.FNum = 0;
            pw.beforeFNum = -1;
            pw.MaxVolume = 15;
            pw.volume = pw.MaxVolume;
            pw.beforeLVolume = -1;
            pw.beforeEnvInstrument = -1;
            pw.keyOn = false;
            pw.port = port;
        }

        public void OutK051649Port(MML mml, byte[] cmd, int chipNumber, byte port, byte adr, byte data)
        {
            parent.OutData(
                mml,
                cmd
                , (byte)((chipNumber!=0 ? 0x80 : 0x00) + port)
                , adr
                , data);
        }

        public void OutK051649SetInstrument(partWork pw, MML mml, int n)
        {

            if (!parent.instWF.ContainsKey(n))
            {
                msgBox.setWrnMsg(string.Format(msg.get("E10021"), n), mml.line.Lp);
                return;
            }

            for (int i = 1; i < parent.instWF[n].Length; i++) // 添え字0 は音色番号が入っている為1からスタート
            {
                int ch = pw.ch;
                if (!parent.info.isK052539 && ch == 4) ch = 3;

                OutK051649Port(
                    mml,port[0],
                    pw.chipNumber
                    , (byte)(parent.info.isK052539 ? 4 : 0)
                    , (byte)(ch * 32 + i - 1)
                    , parent.instWF[n][i]);
            }

        }

        public override void StorePcm(Dictionary<int, clsPcm> newDic, KeyValuePair<int, clsPcm> v, byte[] buf, bool is16bit, int samplerate, params object[] option)
        {
        }

        public override void StorePcmRawData(clsPcmDatSeq pds, byte[] buf, bool isRaw, bool is16bit, int samplerate, params object[] option)
        {
        }

        public override void MultiChannelCommand(MML mml)
        {
            if (!use) return;

            for (int i = 0; i < ChMax; i++)
            {
                partWork pw = lstPartWork[i];

                //fnum
                if (pw.beforeFNum != pw.FNum)
                {
                    byte data = (byte)pw.FNum;
                    OutK051649Port(mml, port[0], ChipNumber, 1, (byte)(0 + pw.ch * 2), data);

                    data = (byte)((pw.FNum & 0xf00) >> 8);
                    OutK051649Port(mml, port[0], ChipNumber, 1, (byte)(1 + pw.ch * 2), data);
                    pw.beforeFNum = pw.FNum;
                }

                //volume
                SetSsgVolume(mml,pw);

                //keyonoff
                if (pw.keyOn)
                {
                    keyOnStatus |= (byte)(1 << pw.ch);
                }
                else
                {
                    keyOnStatus &= (byte)~(1 << pw.ch);
                }
            }

            //keyonoff
            if (keyOnStatus != keyOnStatusOld)
            {
                OutK051649Port(mml, port[0], ChipNumber, 3, 0, keyOnStatus);
                keyOnStatusOld = keyOnStatus;
            }

        }

        public void OutSsgKeyOn(MML mml,partWork pw)
        {
            SetSsgVolume(mml,pw);
            pw.keyOn = true;
        }

        public void OutSsgKeyOff(MML mml,partWork pw)
        {
            SetSsgVolume(mml,pw);
            pw.keyOn = false;
        }

        public void SetSsgVolume(MML mml,partWork pw)
        {
            byte pch = (byte)pw.ch;

            int vol = pw.volume;
            if (pw.envelopeMode)
            {
                vol = 0;
                if (pw.envIndex != -1)
                {
                    vol = pw.volume - (15 - pw.envVolume);
                }
            }

            for (int lfo = 0; lfo < 4; lfo++)
            {
                if (!pw.lfo[lfo].sw) continue;
                if (pw.lfo[lfo].type != eLfoType.Tremolo) continue;

                vol += pw.lfo[lfo].value + pw.lfo[lfo].param[6];
            }

            vol = Common.CheckRange(vol, 0, 15);

            if (pw.beforeVolume != vol)
            {
                OutK051649Port(mml, port[0], ChipNumber, 2, (byte)pw.ch, (byte)vol);
                pw.beforeVolume = vol;
            }
        }

        public void SetSsgFNum(partWork pw)
        {
            int f = GetSsgFNum(pw, pw.octaveNow, pw.noteCmd, pw.shift + pw.keyShift);//
            if (pw.bendWaitCounter != -1)
            {
                f = pw.bendFnum;
            }
            f = f + pw.detune;
            for (int lfo = 0; lfo < 4; lfo++)
            {
                if (!pw.lfo[lfo].sw)
                {
                    continue;
                }
                if (pw.lfo[lfo].type != eLfoType.Vibrato)
                {
                    continue;
                }
                f += pw.lfo[lfo].value + pw.lfo[lfo].param[6];
            }

            f = Common.CheckRange(f, 0, 0xfff);
            pw.FNum = f;
        }

        public int GetSsgFNum(partWork pw, int octave, char noteCmd, int shift)
        {
            int o = octave - 1;
            int n = Const.NOTE.IndexOf(noteCmd) + shift;
            o += n / 12;
            o = Common.CheckRange(o, 0, 7);
            n %= 12;

            int f = o * 12 + n;
            if (f < 0) f = 0;
            if (f >= FNumTbl[0].Length) f = FNumTbl[0].Length - 1;

            return FNumTbl[0][f];
        }

        public override int GetFNum(partWork pw, MML mml, int octave, char cmd, int shift)
        {
            return GetSsgFNum(pw, octave, cmd, shift);
        }


        public override void SetFNum(partWork pw, MML mml)
        {
            SetSsgFNum(pw);
        }

        public override void SetKeyOn(partWork pw, MML mml)
        {
            OutSsgKeyOn(mml,pw);
            SetDummyData(pw, mml);
        }

        public override void SetKeyOff(partWork pw, MML mml)
        {
            OutSsgKeyOff(mml,pw);
        }

        public override void SetVolume(partWork pw, MML mml)
        {
            SetSsgVolume(mml,pw);
        }

        public override void SetLfoAtKeyOn(partWork pw, MML mml)
        {
            for (int lfo = 0; lfo < 4; lfo++)
            {
                clsLfo pl = pw.lfo[lfo];
                if (!pl.sw)
                    continue;

                if (pl.param[5] != 1)
                    continue;

                pl.isEnd = false;
                pl.value = (pl.param[0] == 0) ? pl.param[6] : 0;//ディレイ中は振幅補正は適用されない
                pl.waitCounter = pl.param[0];
                pl.direction = pl.param[2] < 0 ? -1 : 1;

            }
        }

        public override int GetToneDoublerShift(partWork pw, int octave, char noteCmd, int shift)
        {
            //実装不要
            return 0;
        }

        public override void SetToneDoubler(partWork pw, MML mml)
        {
            //実装不要
        }

        public override void CmdY(partWork pw, MML mml)
        {
            if (mml.args[0] is string) return;

            byte port = (byte)((int)mml.args[0] >> 8);
            byte adr = (byte)mml.args[0];
            byte dat = (byte)mml.args[1];

            OutK051649Port(mml, this.port[0], pw.chipNumber, port, adr, dat);
        }

        public override void CmdLoopExtProc(partWork p, MML mml)
        {
            if (p.chip is K051649 && p.chip.use)
            {
                p.beforeFNum = -1;
            }
        }

        public override void CmdInstrument(partWork pw, MML mml)
        {
            char type = (char)mml.args[0];
            int n = (int)mml.args[1];

            if (type == 'I')
            {
                msgBox.setErrMsg(msg.get("E10019"), mml.line.Lp);
                return;
            }

            if (type == 'T')
            {
                msgBox.setErrMsg(msg.get("E10020"), mml.line.Lp);
                return;
            }

            if (type == 'E')
            {
                n = SetEnvelopParamFromInstrument(pw, n, mml);
                return;
            }

            n = Common.CheckRange(n, 0, 255);
            if (pw.instrument != n)
            {
                pw.instrument = n;
                ((K051649)pw.chip).OutK051649SetInstrument(pw,mml, n);
            }

        }

    }
}
