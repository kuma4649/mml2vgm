using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corex64
{
    public class Gigatron : ClsChip
    {
        public Gigatron(ClsVgm parent, int chipID, string initialPartName, string stPath, int chipNumber) : base(parent, chipID, initialPartName, stPath, chipNumber)
        {
            _chipType = enmChipType.Gigatron;
            _Name = "Gigatron";
            _ShortName = "Gigatron";
            _ChMax = 4;//WAV:4ch
            _canUsePcm = false;
            _canUsePI = false;

            Frequency = (int)(521.0 * 59.98);//vsync(NTSC):59.98Hz scanline:521line
            port = new byte[][]{
                new byte[] { 0x00 }
            };

            if (string.IsNullOrEmpty(initialPartName)) return;

            Ch = new ClsChannel[ChMax];
            SetPartToCh(Ch, initialPartName);
            foreach(ClsChannel c in Ch) c.Type = enmChannelType.WaveForm;

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

        }

        public override void InitChip()
        {
            if (!use) return;

            //音源に送る初期化コマンド
            //parent.OutData((MML)null, port[0], new byte[] { 0x01, 0x02 });

        }

        public override void InitPart(partWork pw)
        {
            foreach (partPage page in pw.pg)
            {
                page.volume = 255;
                page.MaxVolume = 255;
                page.beforeVolume = -1;
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
                pl.phase =  (pl.param[0] == 0) ? pl.param[6] : 0;//ディレイ中は振幅補正は適用されない
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
                    vol = page.volume - (page.MaxVolume - page.envVolume);
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

            if (f >= FNumTbl[0].Length) f = FNumTbl[0].Length - 1;
            return FNumTbl[0][f] + pitchShift;
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

            f = Common.CheckRange(f, 0, 0xffff);
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

        private const byte adrWavA = 250;
        private const byte adrWavX = 251;
        private const byte adrFnumL = 252;
        private const byte adrFnumH = 253;

        public override void MultiChannelCommand(MML mml)
        {
            foreach (partWork pw in lstPartWork)
            {
                partPage page = pw.cpg;
                byte val;

                // キーオフの場合は一旦ボリュームを0にする
                if (page.keyOff)
                {
                    val = (byte)(
                        ((page.dutyCycle & 0x7) << 4)
                        | 0
                        );

                    SOutData(page, mml, port[0], adrFnumL, (byte)(page.ch + 1), 0);
                    SOutData(page, mml, port[0], adrFnumH, (byte)(page.ch + 1), 0);
                }

                // 周波数の更新(と位相)

                //キーオンした直後
                //または周波数が違う　場合は周波数再セット
                if (page.keyOn
                    || page.FNum != page.beforeFNum)
                {
                    val = (byte)page.FNum;
                    //SOutData(page, mml, port[0], adrWavA, (byte)(page.ch + 1), 0);
                    SOutData(page, mml, port[0], adrWavX, (byte)(page.ch + 1), 0x1);
                    SOutData(page, mml, port[0], adrFnumL, (byte)(page.ch + 1), (byte)(page.FNum & 0x7f));
                    SOutData(page, mml, port[0], adrFnumH, (byte)(page.ch + 1), (byte)(page.FNum >> 7));

                    val = (byte)(
                        (page.keyOn ? 0x80 : 0x00)
                        | ((page.FNum >> 8) & 0xf)
                        );
                    //SOutData(page, mml, port[0], (byte)(0x02 + page.ch * 4), val);
                }

                //キーオンした直後
                //またはボリュームが違う　場合はボリューム再セット
                if (page.keyOn || page.beforeVolume != page.latestVolume)
                {
                    val = (byte)(
                        ((page.dutyCycle & 0x7) << 4)
                        | (page.latestVolume & 0xf)
                        );
                    //SOutData(page, mml, port[0], (byte)(0x00 + page.ch * 4), val);
                    page.beforeVolume = page.latestVolume;
                }

                page.keyOff = false;
                page.keyOn = false;
                page.beforeFNum = page.FNum;
            }

        }

    }
}
