using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class VRC6 : ClsChip
    {
        public int[][] Saw_FNumTbl;

        public VRC6(ClsVgm parent, int chipID, string initialPartName, string stPath, int chipNumber) : base(parent, chipID, initialPartName, stPath, chipNumber)
        {

            _chipType = enmChipType.VRC6;
            _Name = "VRC6";
            _ShortName = "VRC6";
            _ChMax = 3;//SQR:2ch SAW:1ch
            _canUsePcm = true;
            _canUsePI = true;

            Frequency = 1789772;//NTSC (PAL=1662607)
            port = new byte[][]{
                new byte[] { 0x00 }
            };

            if (string.IsNullOrEmpty(initialPartName)) return;

            Ch = new ClsChannel[ChMax];
            SetPartToCh(Ch, initialPartName);
            Ch[0].Type = enmChannelType.Square;
            Ch[1].Type = enmChannelType.Square;
            Ch[2].Type = enmChannelType.Saw;

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

                if (dic.ContainsKey("MASTERCLOCK"))
                {
                    Frequency = (int)dic["MASTERCLOCK"][0];
                }

                c = 0;
                Saw_FNumTbl = new int[1][] { new int[96] };
                foreach (double v in dic["SAW_FNUM_00"])
                {
                    Saw_FNumTbl[0][c++] = (int)v;
                    if (c == Saw_FNumTbl[0].Length) break;
                }
            }
        }

        public override void InitChip()
        {
            if (!use) return;

            //Square Off
            parent.OutData((MML)null, port[0], new byte[] { 0x01, 0x02 });
            //Saw Off

        }

        public override void InitPart(partWork pw)
        {
            foreach (partPage page in pw.pg)
            {
                page.volume = page.Type == enmChannelType.Square ? 15 : 63;
                page.MaxVolume = page.Type == enmChannelType.Square ? 15 : 63;
                page.beforeVolume = -1;
                page.dutyCycle = 7;
                page.port = port;
            }
        }

        public override void SetLfoAtKeyOn(partPage page, MML mml)
        {
            for (int lfo = 0; lfo < 4; lfo++)
            {
                clsLfo pl = page.lfo[lfo];
                if (!pl.sw) continue;
                if (pl.type == eLfoType.Hardware) continue;
                if (pl.type == eLfoType.Wah) continue;
                if (pl.param[5] != 1) continue;

                pl.isEnd = false;
                pl.value = 0;// (pl.param[0] == 0) ? pl.param[6] : 0;//ディレイ中は振幅補正は適用されない
                pl.phase = (pl.param[0] == 0) ? pl.param[6] : 0;//ディレイ中は振幅補正は適用されない
                pl.waitCounter = pl.param[0];
                pl.direction = pl.param[2] < 0 ? -1 : 1;
                pl.depthWaitCounter = pl.param[7];
                pl.depth = pl.param[3];
                pl.depthV2 = pl.param[2];

                if (pl.type == eLfoType.Vibrato) SetFNum(page, mml);
                if (pl.type == eLfoType.Tremolo) SetVolume(page, mml);
            }
        }

        public override void SetVolume(partPage page, MML mml)
        {
            int vol = page.volume;
            if (page.envelopeMode)
            {
                vol = 0;
                if (page.envIndex != -1)
                {
                    vol = page.volume - (15 - page.envVolume);
                }
            }

            for (int lfo = 0; lfo < 4; lfo++)
            {
                if (!page.lfo[lfo].sw) continue;
                if (page.lfo[lfo].type != eLfoType.Tremolo) continue;

                vol += page.lfo[lfo].value + page.lfo[lfo].phase;
            }

            if (page.varpeggioMode)
            {
                vol += page.varpDelta;
            }

            page.latestVolume = vol;
        }

        public int GetSsgFNum(partPage page, MML mml, int octave, char noteCmd, int shift, int pitchShift)
        {
            int o = octave - 1;
            int n = Const.NOTE.IndexOf(noteCmd) + shift;
            o += n / 12;
            o = Common.CheckRange(o, 0, 7);
            n %= 12;
            int f = o * 12 + n;
            if (f < 0) f = 0;
            
            if (page.Type == enmChannelType.Square)
            {
                if (f >= FNumTbl[0].Length) f = FNumTbl[0].Length - 1;
                return FNumTbl[0][f] + pitchShift;
            }

            if (f >= Saw_FNumTbl[0].Length) f = Saw_FNumTbl[0].Length - 1;
            return Saw_FNumTbl[0][f] + pitchShift;
        }

        public override void SetFNum(partPage page, MML mml)
        {
            int arpNote = page.arpFreqMode ? 0 : page.arpDelta;
            int arpFreq = page.arpFreqMode ? page.arpDelta : 0;

            int f = GetSsgFNum(page, mml
                , page.octaveNow, page.noteCmd, page.shift + page.keyShift + arpNote, page.pitchShift);//
            if (page.bendWaitCounter != -1)
            {
                f = page.bendFnum;
            }

            f += page.detune;
            f += arpFreq;

            for (int lfo = 0; lfo < 4; lfo++)
            {
                if (!page.lfo[lfo].sw)
                {
                    continue;
                }
                if (page.lfo[lfo].type != eLfoType.Vibrato)
                {
                    continue;
                }
                f += page.lfo[lfo].value + page.lfo[lfo].phase;
            }

            f = Common.CheckRange(f, 0, 0xfff);
            page.FNum = f;
        }

        public override void SetKeyOn(partPage page, MML mml)
        {
            page.keyOn = true;
        }

        public override void SetKeyOff(partPage page, MML mml)
        {
            page.keyOff = true;
        }

        public override void MultiChannelCommand(MML mml)
        {
            foreach (partWork pw in lstPartWork)
            {
                partPage page = pw.cpg;
                byte val;

                switch (page.Type)
                {
                    case enmChannelType.Square:
                        {
                            // キーオフの場合は一旦ボリュームを0にする
                            if (page.keyOff)
                            {
                                val = (byte)(
                                    ((page.dutyCycle & 0x7) << 4)
                                    | 0
                                    );
                                SOutData(page, mml, port[0], (byte)(0x00 + page.ch * 4), val);
                            }

                            // 周波数の更新(と位相)
                            
                            //キーオンした直後
                            //または周波数が違う　場合は周波数再セット
                            if (page.keyOn
                                || page.FNum != page.beforeFNum)
                            {
                                val= (byte)page.FNum;
                                SOutData(page, mml, port[0], (byte)(0x01 + page.ch * 4), val);

                                val = (byte)(
                                    (page.keyOn ? 0x80 : 0x00) 
                                    | ((page.FNum >> 8) & 0xf)
                                    );
                                SOutData(page, mml, port[0], (byte)(0x02 + page.ch * 4), val);
                            }

                            // ボリュームの更新

                            if (page.keyOn || page.beforeVolume != page.latestVolume)
                            {
                                val = (byte)(
                                    ((page.dutyCycle & 0x7) << 4)
                                    | (page.latestVolume & 0xf)
                                    );
                                SOutData(page, mml, port[0], (byte)(0x00 + page.ch * 4), val);
                                page.beforeVolume = page.latestVolume;
                            }
                        }
                        break;
                    case enmChannelType.Saw:
                        {
                            // キーオフの場合は一旦ボリュームを0にする
                            if (page.keyOff)
                            {
                                val = 0;
                                SOutData(page, mml, port[0], (byte)(0x00 + page.ch * 4), val);
                            }

                            // 周波数の更新(と位相)

                            //キーオンした直後
                            //または周波数が違う　場合は周波数再セット
                            if (page.keyOn
                                || page.FNum != page.beforeFNum)
                            {
                                val = (byte)page.FNum;
                                SOutData(page, mml, port[0], (byte)(0x01 + page.ch * 4), val);

                                val = (byte)(
                                    (page.keyOn ? 0x80 : 0x00)
                                    | ((page.FNum >> 8) & 0xf)
                                    );
                                SOutData(page, mml, port[0], (byte)(0x02 + page.ch * 4), val);
                            }

                            // ボリュームの更新

                            if (page.keyOn || page.beforeVolume != page.latestVolume)
                            {
                                val = (byte)(page.latestVolume & 0x3f);
                                SOutData(page, mml, port[0], (byte)(0x00 + page.ch * 4), val);
                                page.beforeVolume = page.latestVolume;
                            }

                        }
                        break;
                }

                page.keyOff = false;
                page.keyOn = false;
                page.beforeFNum = page.FNum;

            }

        }


        public override void StorePcmRawData(clsPcmDatSeq pds, byte[] buf, bool isRaw, bool is16bit, int samplerate, params object[] option)
        {
        }

        public override bool StorePcmCheck()
        {
            return false;
        }

        public override int GetToneDoublerShift(partPage page, int octave, char noteCmd, int shift)
        {
            return 0;
        }

        public override void SetToneDoubler(partPage page, MML mml)
        {
        }

        public override void CmdY(partPage page, MML mml)
        {
        }

        public override void CmdLoopExtProc(partPage page, MML mml)
        {
        }

        public override void CmdInstrument(partPage page, MML mml)
        {
            char type;
            bool re = false;
            int n;
            if (mml.args[0] is bool)
            {
                type = (char)mml.args[1];
                re = true;
                n = (int)mml.args[2];
            }
            else
            {
                type = (char)mml.args[0];
                n = (int)mml.args[1];
            }

            //Duty比切り替え
            if (type == 'I')
            {
                if (page.Type == enmChannelType.Square)
                {
                    if (re) n = page.instrument + n;
                    n = Common.CheckRange(n, 0, 7);
                    page.dutyCycle = n;
                    SetDummyData(page, mml);
                    return;
                }

                msgBox.setErrMsg(msg.get("E32000"), mml.line.Lp);
                return;
            }

            //E指定　ソフトエンベロープ切り替え
            if (type == 'E')
            {
                SetEnvelopParamFromInstrument(page, n, re, mml);
                return;
            }

            //無指定でdpcm以外の場合は ソフトエンベロープ切り替え
            if (page.Type != enmChannelType.DPCM)
            {
                SetEnvelopParamFromInstrument(page, n, re, mml);
                return;
            }

        }


    }
}
