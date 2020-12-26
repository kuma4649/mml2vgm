using SoundManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mml2vgmIDE
{
    public class MidWriter
    {
        private Setting setting = null;
        private FileStream dest = null;
        private midiChip midi = new midiChip();
        private int len = 0;

        public MidWriter(Setting setting)
        {
            this.setting = setting;
        }

        public void Open(string filename)
        {

            if (dest != null) Close();
            dest = new FileStream(filename, FileMode.Create, FileAccess.Write);

            List<byte> cData = new List<byte>();
            cData.Add(0x4d); //チャンクタイプ'MThd'
            cData.Add(0x54);
            cData.Add(0x68);
            cData.Add(0x64);

            cData.Add(0x00); //データ長
            cData.Add(0x00);
            cData.Add(0x00);
            cData.Add(0x06);

            cData.Add(0x00); //フォーマット
            cData.Add(0x01);

            cData.Add(0x00); //トラック数
            cData.Add(0x11);

            cData.Add(0x01); //分解能
            cData.Add(0xe0);

            cData.Add(0x4d); //チャンクタイプ'MTrk'
            cData.Add(0x54);
            cData.Add(0x72);
            cData.Add(0x6b);

            cData.Add(0x00); //データ長 0x17
            cData.Add(0x00);
            cData.Add(0x00);
            cData.Add(0x17);

            cData.Add(0x00); //Delta 0
            cData.Add(0xff); //メタイベント
            cData.Add(0x03);
            cData.Add(0x00);

            cData.Add(0x00); //Delta 0
            cData.Add(0xff); //メタイベント　拍子 4/4(固定)
            cData.Add(0x58);
            cData.Add(0x04);
            cData.Add(0x04);
            cData.Add(0x02);
            cData.Add(0x18);
            cData.Add(0x08);

            cData.Add(0x00); //Delta 0
            cData.Add(0xff); //メタイベント　テンポ設定 BPM = 120(固定)
            cData.Add(0x51);
            cData.Add(0x03);
            cData.Add(0x07);
            cData.Add(0xa1);
            cData.Add(0x20);

            cData.Add(0x00); //Delta 0
            cData.Add(0xff); //メタイベント　終端
            cData.Add(0x2f);
            cData.Add(0x00);

            //出力
            dest.Write(cData.ToArray(), 0, cData.Count);
            InitMIDIDevice();
        }

        public void Close()
        {
            if (dest == null) return;

            //dest.Seek(4, SeekOrigin.Begin);
            //int fsize = len + 36;
            //dest.WriteByte((byte)((fsize & 0xff) >> 0));
            //dest.WriteByte((byte)((fsize & 0xff00) >> 8));
            //dest.WriteByte((byte)((fsize & 0xff0000) >> 16));
            //dest.WriteByte((byte)((fsize & 0xff000000) >> 24));

            //dest.Seek(40, SeekOrigin.Begin);
            //dest.WriteByte((byte)((len & 0xff) >> 0));
            //dest.WriteByte((byte)((len & 0xff00) >> 8));
            //dest.WriteByte((byte)((len & 0xff0000) >> 16));
            //dest.WriteByte((byte)((len & 0xff000000) >> 24));

            int MTrkLengthAdr = 0x04;
            for (int ch = 0; ch < midi.data.Length; ch++)
            {
                midi.data[ch].Add(0x00);
                midi.data[ch].Add(0xff); //メタイベント
                midi.data[ch].Add(0x2f);
                midi.data[ch].Add(0x00);

                midi.data[ch][MTrkLengthAdr + 0] = (byte)(((midi.data[ch].Count - (MTrkLengthAdr + 4)) & 0xff000000) >> 24);
                midi.data[ch][MTrkLengthAdr + 1] = (byte)(((midi.data[ch].Count - (MTrkLengthAdr + 4)) & 0x00ff0000) >> 16);
                midi.data[ch][MTrkLengthAdr + 2] = (byte)(((midi.data[ch].Count - (MTrkLengthAdr + 4)) & 0x0000ff00) >> 8);
                midi.data[ch][MTrkLengthAdr + 3] = (byte)(((midi.data[ch].Count - (MTrkLengthAdr + 4)) & 0x000000ff) >> 0);
            }
            foreach (List<byte> dat in midi.data) foreach (byte d in dat) dest.WriteByte(d);

            dest.Close();
            dest = null;
        }

        public void Write(short[] buffer, int offset, int sampleCount)
        {
            if (dest == null) return;

            //for (int i = 0; i < sampleCount; i++)
            //{
            //    dest.WriteByte((byte)(buffer[offset + i] & 0xff));
            //    dest.WriteByte((byte)((buffer[offset + i] & 0xff00) >> 8));
            //}
            //len += sampleCount * 2;
        }

        public void SendChipData(long counter, Chip chip, EnmDataType type, int address, int data, object exData)
        {
            Console.WriteLine("counter:{0} chip:{1} type:{2} address:{3} data:{4} exData:{5}", counter,  chip, type, address, data, exData);

            int ch = 0;
            byte[] bs = (byte[])exData;
            if ((bs[0] & 0x80) != 0)
            {
                ch = bs[0] & 0xf;
            }

            SetDelta(ch, midi, counter);

            if (bs[0] == 0xf0)
            {
                midi.data[ch].Add(0xf0);
                midi.data[ch].Add((byte)bs.Length);
            }

            foreach (byte b in bs)
            {
                midi.data[ch].Add(b);
            }
        }

        private void InitMIDIDevice()
        {
            midi = new midiChip();
            midi.maxTrk = 16;
            midi.data = new List<byte>[midi.maxTrk];
            midi.oldFrameCounter = new long[midi.maxTrk];

            for (int i = 0; i < midi.maxTrk; i++)
            {
                midi.data[i] = new List<byte>();
                midi.oldFrameCounter[i] = -1L;
            }

            for (int i = 0; i < midi.maxTrk; i++)
            {
                midi.data[i].Add(0x4d); //チャンクタイプ'MTrk'
                midi.data[i].Add(0x54);
                midi.data[i].Add(0x72);
                midi.data[i].Add(0x6b);

                midi.data[i].Add(0x00); //データ長 この時点では不明のためとりあえず0
                midi.data[i].Add(0x00);
                midi.data[i].Add(0x00);
                midi.data[i].Add(0x00);

                midi.data[i].Add(0x00); //delta0
                midi.data[i].Add(0xff); // メタイベントポート指定
                midi.data[i].Add(0x21);
                midi.data[i].Add(0x01);
                midi.data[i].Add(0x00); //Port1

                midi.data[i].Add(0x00); //delta0
                midi.data[i].Add(0xff); // メタイベント　トラック名
                midi.data[i].Add(0x03);
                midi.data[i].Add(0x00);

            }

        }


        private void SetDelta(int ch, midiChip chip, long NewFrameCounter)
        {
            if (ch >= chip.oldFrameCounter.Length) return;

            long sub = NewFrameCounter - chip.oldFrameCounter[ch];
            long step = (long)(sub / (double)Common.SampleRate * 960.0);
            chip.oldFrameCounter[ch] += (long)(step * (double)Common.SampleRate / 960.0);

            bool flg = true;
            for (int i = 0; i < 4; i++)
            {
                byte d = (byte)((step & (0x0fe00000 >> (7 * i))) >> (21 - 7 * i));
                if (flg && d == 0 && i < 3) continue;
                flg = false;
                d |= (byte)((i != 3) ? 0x80 : 0x00);
                chip.data[ch].Add(d);
            }

        }

        public class midiChip
        {
            public List<byte>[] data = null;
            public long[] oldFrameCounter = null;
            public int maxTrk = 0;
        }

    }
}
