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
               127,123,119,115,111,107,104,101, 98,95,92,89,87,85,83,81,
                79, 77, 76, 75, 74, 73, 72, 71, 70,70,69,68,67,66,65,64,
                63, 62, 61, 60, 59, 58, 57, 56, 55,54,53,52,51,50,49,48,
                47, 46, 45, 44, 42, 41, 40, 39, 38,37,36,35,34,33,32,31,
                30, 29, 28, 27, 26, 25, 24, 23, 22,21,20,19,18,17,17,16,
                15, 15, 14, 14, 13, 13, 12, 12, 11,11,10,10, 9, 9, 8, 8,
                 7,  7,  7,  7,  6,  6,  6,  6,  5, 5, 5, 5, 4, 4, 4, 4,
                 3,  3,  3,  3,  2,  2,  2,  2,  1, 1, 1, 1, 0, 0, 0, 0,
            };

            foreach (Result d in dat)
            {
                float pow = d.freq > 440 ? (float)d.powSpec : (float)(d.powSpec * ((d.freq / 440.0 / 1.5) - 0.0));
                d.tl = TLtbl[Math.Min(Math.Max((int)(pow * tlMul / 220.0 * 127), 0), 127)];//要調整
                
                float freq = (float)d.freq;
                int oct;
                for (oct = 0; oct < 8; oct++) if (freq < (110 * Math.Pow(2, oct))) break;
                oct = Math.Min(Math.Max(oct, 0), 7);
                if (oct <= 4) freq = freq * (float)(Math.Pow(2 , (4 - oct)));
                else if (oct > 4) freq = freq / (float)(Math.Pow(2 , (oct - 4)));

                d.fnum = (int)(144.0 * freq * (2 << 19) / OPNMasterClock / (2 << oct));
                d.fnum = (d.fnum & 0x7ff) | (oct << 11);
                if (d.fnum > 0x3fff)
                    d.fnum = 0x3fff;
                else if (d.fnum < 0)
                    d.fnum = 0;
            }
        }


        public Complex[] GetTargetDat(float[] dat, int index)
        {
            var fftsample = new Complex[fftsampleRange];
            for(int i = 0; i < fftsampleRange; i++)
            {
                float d = i + index < dat.Length ? dat[i + index] : 0;
                fftsample[i].X = (float)(d * FastFourierTransform.HammingWindow(i, fftsampleRange));
                fftsample[i].Y = 0;
            }

            return fftsample;
        }

        public Complex[] FFTProcess2(Complex[] sdata, int index,out float[] res)
        {

            var fftsample = new Complex[fftsampleRange];
            res= new float[fftsampleRange];

            for (int i = 0; i < fftsampleRange; i++)
            {
                fftsample[i].X = sdata[i].X;
                fftsample[i].Y = sdata[i].Y;
            }

            FastFourierTransform.FFT(true, (int)Math.Log(fftsampleRange, 2), fftsample);

            for (int i = 0; i < fftsampleRange ; i++)
            {
                res[i] = (float)Math.Sqrt(fftsample[i].X * fftsample[i].X + fftsample[i].Y * fftsample[i].Y);//パワースペクトル
            }

            return fftsample;
        }


        public int GetPeek(float[] dat,out Result val)
        {
            List<Tuple<int,float>> ret = new List<Tuple<int, float>>();

            //自分が両隣と比較して頂点か比較する
            for (int i = 0; i < dat.Length / 2; i++)
            {
                float b = i > 0 ? dat[i - 1] : 0;
                float a = i < dat.Length / 2 - 1 ? dat[i + 1] : 0;

                if (b <= dat[i] && dat[i] >= a)
                {
                    ret.Add(new Tuple<int, float>(
                        i
                        , dat[i]
                        ));
                }
            }

            //sort
            for (int i = 0; i < ret.Count; i++)
            {
                for (int j = i + 1; j < ret.Count; j++)
                {
                    if (ret[i].Item2 < ret[j].Item2)
                    {
                        Tuple<int, float> d = ret[i];
                        ret[i] = ret[j];
                        ret[j] = d;
                    }
                }
            }

            val = new Result(
                (int)(ret[0].Item1 / (float)fftsampleRange * (float)sampleRate + 0.5)
                , (int)(ret[0].Item2 * 1000));

            return ret[0].Item1;
        }

        public Complex[] MakePeek(Complex[] dat, int pos)
        {
            Complex[] res = new Complex[dat.Length];
            for (int i = 0; i < dat.Length / 2; i++)
            {
                if (i == pos)
                {
                    res[i] = new Complex();
                    res[i].X = dat[i].X;
                    res[i].Y = dat[i].Y;
                    res[res.Length - 1 - i] = new Complex();
                    res[res.Length - 1 - i].X = dat[res.Length - 1 - i].X;
                    res[res.Length - 1 - i].Y = dat[res.Length - 1 - i].Y;
                    continue;
                }

                res[i] = new Complex();
                res[res.Length - 1 - i] = new Complex();
            }

            return res;
        }

        public List<Result> VoiceFilter(List<Result> lst)
        {
            List<Result> dst = new List<Result>();
            //先ずはコピー
            foreach(var v in lst)
                dst.Add(v);

            //対象外の周波数はふるい落とす
            int n = 0;
            while (n < dst.Count)
            {
                if (dst[n].freq > 10000)
                {
                    dst.RemoveAt(n);
                    continue;
                }
                if(dst[n].freq<10)
                {
                    dst.RemoveAt(n);
                    continue;
                }
                n++;
            }

            //ここで候補が5個未満ならばフィルタ完了
            if (dst.Count < 5)
            {
                while (dst.Count < 4) dst.Add(new Result(0, 0));
                return dst;
            }

            //自分と近い周波数はふるい落とす
            for (int i = 0; i < dst.Count; i++)
            {
                for (int j = i + 1; j < dst.Count; j++)
                {
                    float ml = 0;
                    if (dst[j].freq >= dst[i].freq) ml = (float)dst[j].freq / (float)dst[i].freq;
                    else ml = (float)dst[i].freq / (float)dst[j].freq;
                    if (ml < 1.5 && dst[i].freq < 2500)
                    {
                        dst.RemoveAt(j);
                        j = i;
                        continue;
                    }
                }
            }

            //ここで候補が5個未満ならばフィルタ完了
            if (dst.Count < 5)
            {
                while (dst.Count < 4) dst.Add(new Result(0, 0));
                return dst;
            }

            return dst;
        }

        public Complex[] RFFTProcess(Complex[] sdata)
        {
            var res = new float[fftsampleRange];

            FastFourierTransform.FFT(false, (int)Math.Log(fftsampleRange, 2), sdata);

            return sdata;
        }

        public void DecDat(Complex[] trg, Complex[] rres)
        {
            for (int i = 0; i < trg.Length; i++)
            {
                trg[i].X -= rres[i].X;
                trg[i].Y -= rres[i].Y;
            }
        }


    }
}
