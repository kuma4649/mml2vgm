namespace mml2vgmIDEx64
{
    public class RealTimeMML
    {
        public double samplesPerClock = Corex64.Information.DEFAULT_SAMPLES_PER_CLOCK;
        public long tempo = Corex64.Information.DEFAULT_TEMPO;
        public long clockCount = Corex64.Information.DEFAULT_CLOCK_COUNT;
        public double sampleCount = 0.0;
        public Corex64.ClsChip chip = null;
        public Corex64.ClsVgm vgm = null;

        public void OneFrameSeq()
        {
            if (chip == null) return;
            if (vgm == null) return;

            tempo = Audio.sm.CurrentTempo;
            clockCount = Audio.sm.CurrentClockCount;

            //割り込み回数が１クロック当たりのサンプル数を超えたかチェック
            samplesPerClock = Corex64.Information.VGM_SAMPLE_PER_SECOND * 60.0 * 4.0 / (tempo * clockCount);
            sampleCount++;
            if (sampleCount < samplesPerClock) return;
            sampleCount -= samplesPerClock;

            //超えた場合は選択している音源の演奏を行う
            vgm_getByteData();
            DecWaitCounter();
        }

        private void vgm_getByteData()
        {
            Corex64.partWork pw;
            for (int i = 0; i < chip.lstPartWork.Count; i++)
            {
                pw = chip.lstPartWork[
                    chip.ReversePartWork
                    ? (chip.lstPartWork.Count - 1 - i)
                    : i
                    ];
                vgm.partWorkByteData(pw, 0);
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
            foreach (Corex64.partWork pw in chip.lstPartWork)
            {

                if (pw.pg[0].waitKeyOnCounter > 0) pw.pg[0].waitKeyOnCounter--;
                if (pw.pg[0].waitCounter > 0) pw.pg[0].waitCounter--;
                if (pw.pg[0].bendWaitCounter > 0) pw.pg[0].bendWaitCounter--;
                if (pw.pg[0].pcmWaitKeyOnCounter > 0) pw.pg[0].pcmWaitKeyOnCounter--;
                if (pw.pg[0].envelopeMode && pw.pg[0].envIndex != -1) pw.pg[0].envCounter--;
                for (int lfo = 0; lfo < 4; lfo++)
                {
                    if (!pw.pg[0].lfo[lfo].sw) continue;
                    if (pw.pg[0].lfo[lfo].waitCounter == -1) continue;
                    if (pw.pg[0].lfo[lfo].waitCounter > 0)
                    {
                        pw.pg[0].lfo[lfo].waitCounter--;
                        if (pw.pg[0].lfo[lfo].waitCounter < 0) pw.pg[0].lfo[lfo].waitCounter = 0;
                    }
                }
                if (pw.pg[0].keyOnDelay.sw)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (pw.pg[0].keyOnDelay.delayWrk[i]<=0) continue;
                        pw.pg[0].keyOnDelay.delayWrk[i]--;
                    }
                }

            }
        }
    }
}