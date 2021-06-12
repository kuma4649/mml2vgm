using NAudio.Dsp;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mml2vgmIDE.Tool.CSM
{
    //参照元:
    //「NRTDRV」
    //OPMでCSM音声合成
    //
    //より
    //
    //URL:
    //http://nrtdrv.sakura.ne.jp/index.cgi?page=OPM%A4%C7CSM%B2%BB%C0%BC%B9%E7%C0%AE
    //
    //
    //「自宅で隠遁生活」
    //NAudio キャプチャしたデータをFFTして表示し、WaveOutで音も出してみる。
    //
    //より
    //
    //URL:
    //https://atliqu.com/archives/naudio-%e3%82%ad%e3%83%a3%e3%83%97%e3%83%81%e3%83%a3%e3%81%97%e3%81%9f%e3%83%87%e3%83%bc%e3%82%bf%e3%82%92fft%e3%81%97%e3%81%a6%e8%a1%a8%e7%a4%ba%e3%81%97%e3%80%81waveout%e3%81%a7%e9%9f%b3%e3%82%82/
    //

    public class FFT
    {
        private int fftsampleRange = 4096;
        private int sampleRate = 44100;


        public class Result
        {
            //周波数
            public int freq;
            //パワースペクトル * 1000
            public int powSpec;
            //F-number
            public int fnum;
            //total level
            public int tl;

            public Result(int freq, int powSpec)
            {
                this.freq = freq;
                this.powSpec = powSpec;
            }

        }


        public FFT(int pw = 11, int sampleRate = 44100)
        {
            //if (pw < 1) pw = 11;
            //fftsampleRange = 2 << pw;
            fftsampleRange = pw;
            this.sampleRate = sampleRate;
        }

        public float[] ConvertTo(short[] src)
        {
            float[] dest = new float[src.Length];
            for (int i = 0; i < src.Length; i++)
            {
                dest[i] = src[i] / 32768.0f;
            }
            return dest;
        }

        public float[] FFTProcess(float[] sdata, int index)
        {

            var fftsample = new Complex[fftsampleRange];
            var res = new float[fftsampleRange / 2];

            for (int i = 0; i < fftsampleRange; i++)
            {
                float d = i + index < sdata.Length ? sdata[i + index] : 0;
                fftsample[i].X = (float)(d * FastFourierTransform.HammingWindow(i, fftsampleRange));
                fftsample[i].Y = 0;
            }

            FastFourierTransform.FFT(true, (int)Math.Log(fftsampleRange, 2), fftsample);

            for (int i = 0; i < fftsampleRange / 2; i++)
            {
                //res[i] = 10 * (float)Math.Log(Math.Sqrt(fftsample[i].X * fftsample[i].X + fftsample[i].Y * fftsample[i].Y));//Dbに変換
                res[i] = (float)Math.Sqrt(fftsample[i].X * fftsample[i].X + fftsample[i].Y * fftsample[i].Y);//パワースペクトル
            }

            return res;

        }

        public float[] ReadWave(WaveFileReader reader)
        {
            int length = (int)reader.Length;// 4096*2;
            byte[] bytesBuffer = new byte[length];
            int read = reader.Read(bytesBuffer, 0, length);

            var floatSamples = new float[read / 2];
            for (int sampleIndex = 0; sampleIndex < read / 2; sampleIndex++)
            {
                var intSampleValue = BitConverter.ToInt16(bytesBuffer, sampleIndex * 2);
                floatSamples[sampleIndex] = intSampleValue / 32768.0f;
            }

            return floatSamples;
        }

        public List<Result> GetPeekList(float[] dat, int num)
        {
            List<Result> ret = new List<Result>();

            //自分が両隣と比較して頂点か比較する
            for (int i = 0; i < dat.Length; i++)
            {
                float b = i > 0 ? dat[i - 1] : 0;
                float a = i < dat.Length - 1 ? dat[i + 1] : 0;

                if (b <= dat[i] && dat[i] >= a)
                {
                    ret.Add(new Result(
                        (int)(i / (float)fftsampleRange * (float)sampleRate + 0.5)
                        , (int)(dat[i] * 1000)
                        ));
                }
            }

            //sort
            for (int i = 0; i < ret.Count; i++)
            {
                for (int j = i + 1; j < ret.Count; j++)
                {
                    if (ret[i].powSpec < ret[j].powSpec)
                    {
                        Result d = ret[i];
                        ret[i] = ret[j];
                        ret[j] = d;
                    }
                }
            }

            if (ret.Count > num)
            {
                //指定個数だけにする
                ret.RemoveRange(num, ret.Count - num);
            }

            return ret;
        }

        public void CalcFnumTl(List<Result> dat,float tlMul, int OPNMasterClock = 3993600, int prescale = 0)
        {
            float[] fmDivTbl = new float[] { 6, 3, 2 };
            float fmDiv = fmDivTbl[prescale];
            int[] TLtbl = new int[128]
            {
                85,83,82,80,79,77,76,75, 73,72,71,70,68,67,66,65,
                63,62,61,60,59,58,57,56, 55,54,53,52,51,50,49,48,
                47,46,45,44,43,42,41,40, 39,38,37,36,36,35,35,34,
                34,33,33,32,32,31,31,30, 30,29,29,28,28,27,27,26,
                26,25,25,24,24,23,23,22, 22,21,21,20,20,19,19,18,
                18,17,17,16,16,15,15,14, 14,13,13,12,12,11,11,11,
                10,10,10, 9, 9, 9, 8, 8,  8, 7, 7, 7, 6, 6, 6, 5,
                 5, 5, 4, 4, 4, 3, 3, 3,  2, 2, 2, 1, 1, 1, 0, 0,
            };

            foreach (Result d in dat)
            {
                d.tl = TLtbl[Math.Min(Math.Max((int)(d.powSpec * tlMul / 220.0 * 127), 0), 127)];//要調整
                
                float freq = (float)d.freq;
                int oct;
                for (oct = 0; oct < 8; oct++) if (freq < (110 * Math.Pow(2, oct))) break;
                oct = Math.Min(Math.Max(oct, 0), 7);
                if (oct < 4) freq = freq * (float)(Math.Pow(2 , (4 - oct)));
                if (oct > 4) freq = freq / (float)(Math.Pow(2 , (oct - 4)));

                d.fnum = (int)(144.0 * freq * (2 << 19) / OPNMasterClock / (2 << oct));
                d.fnum = (d.fnum & 0x7ff) | (oct << 11);
                if (d.fnum > 0x3fff)
                    d.fnum = 0x3fff;
                else if (d.fnum < 0)
                    d.fnum = 0;
            }
        }
    }
}
