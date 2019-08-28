using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mml2vgmIDE
{
    public class RealTimeMML
    {
        public double samplesPerClock = Core.Information.DEFAULT_SAMPLES_PER_CLOCK;
        public long tempo = Core.Information.DEFAULT_TEMPO;
        public long clockCount = Core.Information.DEFAULT_CLOCK_COUNT;
        public double sampleCount = 0.0;
        public Core.ClsChip chip = null;
        public Core.ClsVgm vgm = null;

        public void OneFrameSeq()
        {
            if (chip == null) return;
            if (vgm == null) return;

            tempo = Audio.sm.CurrentTempo;
            clockCount = Audio.sm.CurrentClockCount;

            //割り込み回数が１クロック当たりのサンプル数を超えたかチェック
            samplesPerClock = Core.Information.VGM_SAMPLE_PER_SECOND * 60.0 * 4.0 / (tempo * clockCount);
            sampleCount++;
            if (sampleCount < samplesPerClock) return;
            sampleCount -= samplesPerClock;

            //超えた場合は選択している音源の演奏を行う
            vgm_getByteData();
            DecWaitCounter();
        }

        private void vgm_getByteData()
        {
            Core.partWork pw;
            for (int i = 0; i < chip.lstPartWork.Count; i++)
            {
                pw = chip.lstPartWork[
                    chip.ReversePartWork
                    ? (chip.lstPartWork.Count - 1 - i)
                    : i
                    ];
                vgm.partWorkByteData(pw);
            }
            if (chip.SupportReversePartWork) chip.ReversePartWork = !chip.ReversePartWork;

            //channelを跨ぐコマンド向け処理
            if (chip.use)
            {
                chip.MultiChannelCommand(null);
            }
        }

        private void DecWaitCounter()
        {
            foreach (Core.partWork pw in chip.lstPartWork)
            {
                if (pw.waitKeyOnCounter > 0) pw.waitKeyOnCounter--;
                if (pw.waitCounter > 0) pw.waitCounter--;
                if (pw.bendWaitCounter > 0) pw.bendWaitCounter--;
                if (pw.pcmWaitKeyOnCounter > 0) pw.pcmWaitKeyOnCounter--;
                if (pw.envelopeMode && pw.envIndex != -1) pw.envCounter--;
                for (int lfo = 0; lfo < 4; lfo++)
                {
                    if (!pw.lfo[lfo].sw) continue;
                    if (pw.lfo[lfo].waitCounter == -1) continue;
                    if (pw.lfo[lfo].waitCounter > 0)
                    {
                        pw.lfo[lfo].waitCounter--;
                        if (pw.lfo[lfo].waitCounter < 0) pw.lfo[lfo].waitCounter = 0;
                    }
                }
            }
        }
    }
}