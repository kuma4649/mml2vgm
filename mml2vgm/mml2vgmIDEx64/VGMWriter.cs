using SoundManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mml2vgmIDEx64
{
    public class VGMWriter
    {
        private Setting setting = null;
        private FileStream dest = null;
        //private int len = 0;
        private long waitCounter = 0;
        //header
        readonly public static byte[] hDat = new byte[] {
            //00 'Vgm '          Eof offset           Version number
            0x56,0x67,0x6d,0x20, 0x00,0x00,0x00,0x00, 0x71,0x01,0x00,0x00, 0x00,0x00,0x00,0x00,
            //10                 GD3 offset(no use)   Total # samples
            0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,
            //20                 Rate(NTSC 60Hz)
            0x00,0x00,0x00,0x00, 0x3c,0x00,0x00,0x00, 0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,
            //30                 VGMdataofs(0x100~)
            0x00,0x00,0x00,0x00, 0xcc,0x00,0x00,0x00, 0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,
            //40                                      
            0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00, 
            //YM2608 clock(7987200 0x0079_e000)
            0x00,0xe0,0x79,0x40, 
            //YM2610 clock(8000000 0x007a_1200)
            0x00,0x00,0x00,0x00,
            //50
            0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,
            //60
            0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,
            //70
            0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,
            //80
            0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,
            //90
            0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,
            //A0
            0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,
            //B0
            0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,
            //C0
            0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,
            //D0
            0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,
            //E0
            0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,
            //F0
            0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00
        };
        private int[] useChips;
        public long totalSample { get; private set; }
        public List<Tuple<string, string>> tags;
        private static readonly int SamplingRate = 44100;//vgm format freq
        private static uint opmMasterClock = 3579545;
        private static readonly uint opnaMasterClock = 7987200;
        private static readonly uint opnbMasterClock = 8000000;

        public VGMWriter(Setting setting)
        {
            this.setting = setting;
        }

        public void Open(string filename)
        {

            if (dest != null) Close();
            dest = new FileStream(filename, FileMode.Create, FileAccess.Write);

            List<byte> des = new List<byte>();

            //ヘッダの出力
            dest.Write(hDat, 0, hDat.Length);

        }

        public void Close()
        {
            if (dest == null) return;


            //ヘッダ、フッタの調整

            //end of data
            dest.WriteByte(0x66);

            //Total # samples
            dest.Position = 0x18;
            dest.WriteByte((byte)totalSample);
            dest.WriteByte((byte)(totalSample >> 8));
            dest.WriteByte((byte)(totalSample >> 16));
            dest.WriteByte((byte)(totalSample >> 24));

            //tag
            if (tags != null)
            {
                GD3 gd3 = new GD3();
                foreach (Tuple<string, string> tag in tags)
                {
                    switch (tag.Item1)
                    {
                        case "title":
                            gd3.TrackName = tag.Item2;
                            gd3.TrackNameJ = tag.Item2;
                            break;
                        case "composer":
                            gd3.Composer = tag.Item2;
                            gd3.ComposerJ = tag.Item2;
                            break;
                        case "author":
                            gd3.VGMBy = tag.Item2;
                            break;
                        case "comment":
                            if (string.IsNullOrEmpty(gd3.Notes)) gd3.Notes = tag.Item2;
                            else
                            {
                                gd3.Notes += "\r\n" + tag.Item2;
                            }
                            break;
                        case "mucom88":
                            gd3.Version = tag.Item2;
                            gd3.Notes = tag.Item2;
                            break;
                        case "date":
                            gd3.Converted = tag.Item2;
                            break;
                    }
                }

                byte[] tagary = gd3.make();
                dest.Seek(0, SeekOrigin.End);
                long gd3ofs = dest.Length - 0x14;
                foreach (byte b in tagary) dest.WriteByte(b);

                //Tag offset
                if (tagary != null && tagary.Length > 0)
                {
                    dest.Position = 0x14;
                    dest.WriteByte((byte)gd3ofs);
                    dest.WriteByte((byte)(gd3ofs >> 8));
                    dest.WriteByte((byte)(gd3ofs >> 16));
                    dest.WriteByte((byte)(gd3ofs >> 24));
                }
            }

            //EOF offset
            dest.Position = 0x4;
            dest.WriteByte((byte)(dest.Length - 4));
            dest.WriteByte((byte)((dest.Length - 4) >> 8));
            dest.WriteByte((byte)((dest.Length - 4) >> 16));
            dest.WriteByte((byte)((dest.Length - 4) >> 24));

            //YM2608 offset
            dest.Position = 0x48;
            dest.WriteByte(0);
            dest.WriteByte(0);
            dest.WriteByte(0);
            dest.WriteByte(0);

            //YM2610 offset
            dest.Position = 0x4c;
            dest.WriteByte(0);
            dest.WriteByte(0);
            dest.WriteByte(0);
            dest.WriteByte(0);

            //YM2151 offset
            dest.Position = 0x30;
            dest.WriteByte(0);
            dest.WriteByte(0);
            dest.WriteByte(0);
            dest.WriteByte(0);

            for (int i = 0; i < 5; i++)
            {
                if (useChips[i] == 0 || useChips[i] > 5) continue;
                switch (useChips[i])
                {
                    case 1:
                    case 2:
                        dest.Position = 0x48;
                        dest.WriteByte((byte)(opnaMasterClock >> 0));
                        dest.WriteByte((byte)(opnaMasterClock >> 8));
                        dest.WriteByte((byte)(opnaMasterClock >> 16));
                        if (useChips[i] == 1) dest.WriteByte(0);
                        else dest.WriteByte(0x40);
                        break;
                    case 3:
                    case 4:
                        dest.Position = 0x4c;
                        dest.WriteByte((byte)(opnbMasterClock >> 0));
                        dest.WriteByte((byte)(opnbMasterClock >> 8));
                        dest.WriteByte((byte)(opnbMasterClock >> 16));
                        if (useChips[i] == 3) dest.WriteByte(0x80 + 0x00);//bit31:OPNB2  bit30:dualchip
                        else dest.WriteByte(0x80 + 0x40);
                        break;
                    case 5:
                        dest.Position = 0x30;
                        dest.WriteByte((byte)(opmMasterClock >> 0));
                        dest.WriteByte((byte)(opmMasterClock >> 8));
                        dest.WriteByte((byte)(opmMasterClock >> 16));
                        if (useChips[i] == 5) dest.WriteByte(0);
                        else dest.WriteByte(0x40);
                        break;
                }
            }

            dest.Close();
            dest = null;
        }

        public void IncrementWaitCOunter()
        {
            waitCounter++;
        }

        public void Write(short[] buffer, int offset, int sampleCount)
        {
            if (dest == null) return;

            //波形データは不要の為何も行わない

        }

        public void SendChipData(long counter, Chip chip, EnmDataType type, int address, int data, object exData)
        {
            //Console.WriteLine("counter:{0} chip:{1} type:{2} address:{3} data:{4} exData:{5}", counter,  chip, type, address, data, exData);

            if (chip.Model != EnmVRModel.VirtualModel) return;
            if (chip.Index != 0) return;

            if (chip.Device == Corex64.EnmZGMDevice.YM2608)
            {
                if (address == -1 && data == -1)
                {
                    //Block data?
                    if (exData == null) return;
                    if (!(exData is PackData[])) return;
                    foreach (PackData pd in (PackData[])exData)
                    {
                        WriteYM2608(chip.Number, (byte)(pd.Address >> 8), (byte)pd.Address, (byte)pd.Data);
                    }
                }
                else
                {
                    WriteYM2608(chip.Number, (byte)(address >> 8), (byte)address, (byte)data);
                }
            }
            else if (chip.Device == Corex64.EnmZGMDevice.YM2610B)
            {
                if (type == EnmDataType.Block)
                {
                    //Block data?
                    if (exData == null) return;
                    if (exData is PackData[])
                    {
                        foreach (PackData pd in (PackData[])exData)
                        {
                            WriteYM2610(chip.Number, (byte)(pd.Address >> 8), (byte)pd.Address, (byte)pd.Data);
                        }
                    }
                    else
                    {
                        ;
                        if (data == -1)
                        {
                            WriteYM2610_SetAdpcmA((byte)chip.Number, (byte[])exData);
                        }
                        else
                        {
                            WriteYM2610_SetAdpcmB((byte)chip.Number, (byte[])exData);
                        }

                    }
                }
                else
                {
                    WriteYM2610(chip.Number, (byte)(address >> 8), (byte)address, (byte)data);
                }
            }
            else if (chip.Device == Corex64.EnmZGMDevice.YM2151)
            {
                if (address == -1 && data == -1)
                {
                    //Block data?
                    if (exData == null) return;
                    if (!(exData is PackData[])) return;
                    foreach (PackData pd in (PackData[])exData)
                    {
                        WriteYM2151(chip.Number, (byte)pd.Address, (byte)pd.Data);
                    }
                }
                else
                {
                    WriteYM2151(chip.Number, (byte)address, (byte)data);
                }
            }

        }

        public void WriteYM2608(int v, byte port, byte address, byte data)
        {
            if (dest == null) return;
            if (useChips[0 + v] == 0) return;

            if (waitCounter != 0)
            {
                totalSample += waitCounter;

                ////waitコマンド出力
                //Log.WriteLine(LogLevel.TRACE
                //    , string.Format("wait:{0}", waitCounter)
                //    );

                AddWaitCounterCommand();
                waitCounter = 0;
            }

            //Log.WriteLine(LogLevel.TRACE
            //    , string.Format("p:{0} a:{1} d:{2}", port, address, data)
            //    );

            dest.WriteByte((byte)((v == 0 ? 0x56 : 0xa6) + (port & 1)));
            dest.WriteByte(address);
            dest.WriteByte(data);

        }

        public void WriteYM2610(int v, byte port, byte address, byte data)
        {
            if (dest == null) return;

            if (useChips[2 + v] == 0) return;

            if (waitCounter != 0)
            {
                totalSample += waitCounter;

                //waitコマンド出力
                //Log.WriteLine(LogLevel.TRACE
                //    , string.Format("wait:{0}", waitCounter)
                //    );

                AddWaitCounterCommand();
                waitCounter = 0;
            }

            //Log.WriteLine(LogLevel.TRACE
            //    , string.Format("p:{0} a:{1} d:{2}", port, address, data)
            //    );

            dest.WriteByte((byte)((v == 0 ? 0x58 : 0xa8) + (port & 1)));
            dest.WriteByte(address);
            dest.WriteByte(data);

        }

        private void AddWaitCounterCommand()
        {
            if (waitCounter <= 882 * 3)
            {
                while (waitCounter > 882)
                {
                    dest.WriteByte(0x63);
                    waitCounter -= 882;
                }
                while (waitCounter > 735)
                {
                    dest.WriteByte(0x62);
                    waitCounter -= 735;
                }
            }

            while (waitCounter > 0)
            {
                while (waitCounter > 0xffff)
                {
                    dest.WriteByte(0x61);
                    dest.WriteByte((byte)0xff);
                    dest.WriteByte((byte)0xff);
                    waitCounter -= 0xffff;
                }

                dest.WriteByte(0x61);
                dest.WriteByte((byte)waitCounter);
                dest.WriteByte((byte)(waitCounter >> 8));
                waitCounter -= (waitCounter & 0xffff);
            }
        }

        public void WriteYM2610_SetAdpcmA(byte chipId, byte[] pcmData)
        {
            dest.WriteByte(0x67);
            dest.WriteByte(0x66);
            dest.WriteByte(0x82);

            WritePCMData(chipId, pcmData);
        }

        public void WriteYM2610_SetAdpcmB(byte chipId, byte[] pcmData)
        {

            dest.WriteByte(0x67);
            dest.WriteByte(0x66);
            dest.WriteByte(0x83);

            WritePCMData(chipId, pcmData);
        }

        private void WritePCMData(byte chipId, byte[] pcmData)
        {
            int size = pcmData.Length;

            long sizeOfData = size + 8 + chipId * 0x8000_0000;
            dest.WriteByte((byte)(sizeOfData >> 0));
            dest.WriteByte((byte)(sizeOfData >> 8));
            dest.WriteByte((byte)(sizeOfData >> 16));
            dest.WriteByte((byte)(sizeOfData >> 24));

            dest.WriteByte((byte)(size >> 0));
            dest.WriteByte((byte)(size >> 8));
            dest.WriteByte((byte)(size >> 16));
            dest.WriteByte((byte)(size >> 24));

            int startAddress = 0;
            dest.WriteByte((byte)(startAddress >> 0));
            dest.WriteByte((byte)(startAddress >> 8));
            dest.WriteByte((byte)(startAddress >> 16));
            dest.WriteByte((byte)(startAddress >> 24));

            for (int i = 0x0; i < pcmData.Length; i++)
            {
                dest.WriteByte(pcmData[i]);
            }
        }

        public void WriteYM2151(int v, byte address, byte data)
        {
            if (dest == null) return;
            if (useChips[4 + v] == 0) return;

            if (waitCounter != 0)
            {
                totalSample += waitCounter;

                ////waitコマンド出力
                //Log.WriteLine(LogLevel.TRACE
                //    , string.Format("wait:{0}", waitCounter)
                //    );

                AddWaitCounterCommand();
                waitCounter = 0;
            }

            //Log.WriteLine(LogLevel.TRACE
            //    , string.Format("a:{0} d:{1}", address, data)
            //    );

            dest.WriteByte((byte)(v == 0 ? 0x54 : 0xa4));
            dest.WriteByte(address);
            dest.WriteByte(data);

        }

        public void useChipsFromMub(byte[] buf)
        {
            List<int> ret = new List<int>();
            ret.Add(1);//1:OPNA
            ret.Add(0);//0:unuse
            ret.Add(0);
            ret.Add(0);
            ret.Add(0);
            useChips = ret.ToArray();

            //dest.WriteByte(0x56); dest.WriteByte(0x29); dest.WriteByte(0x82);
            //WriteAdpcm(0, new byte[65536]);

            //標準的なmubファイル
            if (buf[0] == 0x4d
                && buf[1] == 0x55
                && buf[2] == 0x43
                && buf[3] == 0x38)
            {
                return;
            }
            //標準的なmubファイル
            if (buf[0] == 0x4d
                && buf[1] == 0x55
                && buf[2] == 0x42
                && buf[3] == 0x38)
            {
                return;
            }
            //拡張mubファイル？
            if (buf[0] != 'm'
                || buf[1] != 'u'
                || buf[2] != 'P'
                || buf[3] != 'b')
            {
                //見知らぬファイル
                return;
            }

            uint chipsCount = buf[0x0009];
            int ptr = 0x0022;
            uint[] partCount = new uint[chipsCount];
            uint[][] pageCount = new uint[chipsCount][];
            uint[][][] pageLength = new uint[chipsCount][][];
            for (int i = 0; i < chipsCount; i++)
            {
                partCount[i] = buf[ptr + 0x16];
                int instCount = buf[ptr + 0x17];
                ptr += 2 * instCount + 0x18;
                int pcmCount = buf[ptr];
                ptr += 2 * pcmCount + 1;
            }

            for (int i = 0; i < chipsCount; i++)
            {
                pageCount[i] = new uint[partCount[i]];
                pageLength[i] = new uint[partCount[i]][];
                for (int j = 0; j < partCount[i]; j++)
                {
                    pageCount[i][j] = buf[ptr++];
                }
            }

            for (int i = 0; i < chipsCount; i++)
            {
                for (int j = 0; j < partCount[i]; j++)
                {
                    pageLength[i][j] = new uint[pageCount[i][j]];
                    for (int k = 0; k < pageCount[i][j]; k++)
                    {
                        pageLength[i][j][k] = (uint)(
                            buf[ptr]
                            + buf[ptr + 1] * 0x100
                            + buf[ptr + 2] * 0x10000
                            + buf[ptr + 3] * 0x1000000);
                        ptr += 8;
                    }
                }
            }

            ret.Clear();
            ret.Add(0);
            ret.Add(0);
            ret.Add(0);
            ret.Add(0);
            ret.Add(0);

            if (chipsCount > 0)
            {
                if (partCount[0] > 0)
                {
                    int n = 0;
                    for (int i = 0; i < partCount[0]; i++)
                    {
                        for (int j = 0; j < pageLength[0][i].Length; j++)
                        {
                            n += pageLength[0][i][j] > 1 ? 1 : 0;
                        }
                    }
                    if (n > 0) ret[0] = 1;
                }
            }

            if (chipsCount > 1)
            {
                if (partCount[1] > 0)
                {
                    int n = 0;
                    for (int i = 0; i < partCount[1]; i++)
                    {
                        for (int j = 0; j < pageLength[1][i].Length; j++)
                        {
                            n += pageLength[1][i][j] > 1 ? 1 : 0;
                        }
                    }
                    if (n > 0)
                    {
                        ret[1] = 2;
                        dest.WriteByte(0xa6); dest.WriteByte(0x29); dest.WriteByte(0x82);
                    }
                }
            }

            if (chipsCount > 2)
            {
                if (partCount[2] > 0)
                {
                    int n = 0;
                    for (int i = 0; i < partCount[2]; i++)
                    {
                        for (int j = 0; j < pageLength[2][i].Length; j++)
                        {
                            n += pageLength[2][i][j] > 1 ? 1 : 0;
                        }
                    }
                    if (n > 0) ret[2] = 3;
                }
            }

            if (chipsCount > 3)
            {
                if (partCount[3] > 0)
                {
                    int n = 0;
                    for (int i = 0; i < partCount[3]; i++)
                    {
                        for (int j = 0; j < pageLength[3][i].Length; j++)
                        {
                            n += pageLength[3][i][j] > 1 ? 1 : 0;
                        }
                    }
                    if (n > 0) ret[3] = 4;
                }
            }

            if (chipsCount > 4)
            {
                if (partCount[4] > 0)
                {
                    int n = 0;
                    for (int i = 0; i < partCount[4]; i++)
                    {
                        for (int j = 0; j < pageLength[4][i].Length; j++)
                        {
                            n += pageLength[4][i][j] > 1 ? 1 : 0;
                        }
                    }
                    if (n > 0) ret[4] = 5;
                }
            }

            useChips = ret.ToArray();
            return;
        }

        internal void SetTagInfo(Tuple<string, string>[] tag)
        {
            this.tags = tag.ToList();
        }
    }
}