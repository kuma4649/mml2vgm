using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public static class mucomADPCM2PCM
    {
        public static List<mucomPCMInfo> lstMucomPCMInfo;
        private static uint SamplingRate = 8000;
        private static MDSound.MDSound mds = null;

        public static void initial(byte[] mucompcmbin)
        {
            uint samplingBuffer = 1024;
            MDSound.MDSound.Chip[] chips = new MDSound.MDSound.Chip[1];
            MDSound.MDSound.Chip chip = null;

            chip = new MDSound.MDSound.Chip();
            chip.type = MDSound.MDSound.enmInstrumentType.YM2608;
            chip.ID = 0;
            MDSound.ym2608 ym2608 = new MDSound.ym2608();
            chip.Instrument = ym2608;
            chip.Update = ym2608.Update;
            chip.Start = ym2608.Start;
            chip.Stop = ym2608.Stop;
            chip.Reset = ym2608.Reset;
            chip.SamplingRate = SamplingRate;
            chip.Clock = 7987200;
            chip.Volume = 0;
            chip.Option = null;
            chips[0] = chip;

            mds = new MDSound.MDSound(SamplingRate, samplingBuffer, chips);

            mds.WriteYM2608(0, 0, 0x2d, 0x00);
            mds.WriteYM2608(0, 0, 0x29, 0x82);
            mds.WriteYM2608(0, 0, 0x07, 0x38);

            byte[] adpcmBuffer = mds.GetADPCMBufferYM2608(0);
            lstMucomPCMInfo = setPCMData(mucompcmbin, adpcmBuffer);
        }

        public static byte[] GetPcmData(mucomPCMInfo info)
        {
            int sadr = info.stAdr;
            int eadr = info.edAdr;
            int deltaN = 0x49BA + 200;// 0x243e *2;//8kHz
            byte vol = 255;

            mds.WriteYM2608(0, 1, 0x10, 0x08);
            mds.WriteYM2608(0, 1, 0x10, 0x80);
            mds.WriteYM2608(0, 1, 0x02, (byte)sadr);
            mds.WriteYM2608(0, 1, 0x03, (byte)(sadr >> 8));
            mds.WriteYM2608(0, 1, 0x04, (byte)eadr);
            mds.WriteYM2608(0, 1, 0x05, (byte)(eadr >> 8));
            mds.WriteYM2608(0, 1, 0x09, (byte)deltaN);
            mds.WriteYM2608(0, 1, 0x0a, (byte)(deltaN >> 8));
            mds.WriteYM2608(0, 1, 0x0b, vol);
            mds.WriteYM2608(0, 1, 0x01, 0xc0);
            mds.WriteYM2608(0, 1, 0x00, 0xa0);

            List<byte> pcm = new List<byte>();
            do
            {
                short[] buf = new short[2];
                mds.Update(buf, 0, 1, null);
                //Console.Write("[{0:d8}]/[{1:d8}]\r\n", buf[0]>>8, buf[1]);
                pcm.Add((byte)((buf[0] / (double)short.MaxValue) * sbyte.MaxValue + 0x80));
            } while ((mds.ReadStatusExYM2608(0) & 0x20) != 0);

            //dumpData(pcm.ToArray(), (int)SamplingRate, info.name);
            return pcm.ToArray();
        }

        private static List<mucomPCMInfo> setPCMData(byte[] pcmdata, byte[] adpcmBuffer)
        {
            if (pcmdata == null) return null;

            int infosize;
            int i;
            int pcmtable;
            int inftable;
            int adr, whl, eadr;
            byte[] pcmname = new byte[17];
            int maxpcm = 32;
            List<mucomPCMInfo> ret = null;

            infosize = 0x400;
            inftable = 0x0000;

            pcmtable = 0xe300;
            for (i = 0; i < maxpcm; i++)
            {
                adr = pcmdata[inftable + 28] | (pcmdata[inftable + 29] * 0x100);
                whl = pcmdata[inftable + 30] | (pcmdata[inftable + 31] * 0x100);
                eadr = adr + (whl >> 2);
                if (pcmdata[i * 32] != 0)
                {
                    if (ret == null) ret = new List<mucomPCMInfo>();
                    mucomPCMInfo info = new mucomPCMInfo();
                    info.no = i + 1;
                    info.stAdr = adr;
                    info.edAdr = eadr;
                    Array.Copy(pcmdata, i * 32, pcmname, 0, 16);
                    pcmname[16] = 32;
                    info.name = Encoding.GetEncoding("shift_jis").GetString(pcmname).Trim();
                    ret.Add(info);
                    //Console.WriteLine(string.Format("#PCM{0} ${1:x04} ${2:x04} {3}", info.no, info.stAdr, info.edAdr, info.name));
                }
                pcmtable += 8;
                inftable += 32;
            }

            // データ転送
            Array.Copy(pcmdata, infosize, adpcmBuffer, 0, pcmdata.Length - infosize);

            return ret;
        }

        private static void dumpData(byte[] buf, int freq, string fn)
        {

            try
            {
                string dFn = string.Format("{0}.wav", fn);
                List<byte> des = new List<byte>();

                // 'RIFF'
                des.Add((byte)'R'); des.Add((byte)'I'); des.Add((byte)'F'); des.Add((byte)'F');
                // サイズ
                //int fsize = src.Length + 36;
                int fsize = (int)buf.Length + 36;
                des.Add((byte)(fsize >> 0));
                des.Add((byte)(fsize >> 8));
                des.Add((byte)(fsize >> 16));
                des.Add((byte)(fsize >> 24));
                // 'WAVE'
                des.Add((byte)'W'); des.Add((byte)'A'); des.Add((byte)'V'); des.Add((byte)'E');
                // 'fmt '
                des.Add((byte)'f'); des.Add((byte)'m'); des.Add((byte)'t'); des.Add((byte)' ');
                // サイズ(16)
                des.Add(0x10); des.Add(0); des.Add(0); des.Add(0);
                // フォーマット(1)
                des.Add(0x01); des.Add(0x00);
                // チャンネル数(mono)
                des.Add(0x01); des.Add(0x00);
                //サンプリング周波数(16KHz)
                des.Add((byte)(freq >> 0));
                des.Add((byte)(freq >> 8));
                des.Add((byte)(freq >> 16));
                des.Add((byte)(freq >> 24));
                //平均データ割合(16K)
                des.Add((byte)(freq >> 0));
                des.Add((byte)(freq >> 8));
                des.Add((byte)(freq >> 16));
                des.Add((byte)(freq >> 24));
                //ブロックサイズ(1)
                des.Add(0x01); des.Add(0x00);
                //ビット数(8bit)
                des.Add(0x08); des.Add(0x00);

                // 'data'
                des.Add((byte)'d'); des.Add((byte)'a'); des.Add((byte)'t'); des.Add((byte)'a');
                // サイズ(データサイズ)
                des.Add((byte)((buf.Length & 0xff) >> 0));
                des.Add((byte)((buf.Length & 0xff00) >> 8));
                des.Add((byte)((buf.Length & 0xff0000) >> 16));
                des.Add((byte)((buf.Length & 0xff000000) >> 24));

                for (int i = 0; i < buf.Length; i++)
                {
                    des.Add(buf[i]);
                }

                //出力
                System.IO.File.WriteAllBytes(dFn, des.ToArray());

            }
            catch
            {
            }
        }

        public class mucomPCMInfo
        {
            public int no = 0;
            public int stAdr = 0;
            public int edAdr = 0;
            public string name = "";
        }

    }
}
