using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class ZGMmaker
    {
        private byte[] HeaderTemp = new byte[]
        {
            0x5a , 0x47 , 0x4d , 0x20 //'ZGM '
            ,0,0,0,0 //+0x04: EoF offset
            ,10,0,0,0 //+0x08: Version
            ,0,0,0,0 //+0x0c: Total # samples
            ,0,0,0,0 //+0x10: Loop # samples
            ,0,0,0,0 //+0x14: Loop offset
            ,0,0,0,0 //+0x18: GD3 offset
            ,0,0,0,0 //+0x1c: Define offset
            ,0,0,0,0 //+0x20: Track 1 offset
            ,0,0     //+0x24: Define Count
            ,0,0     //+0x26: Track  Count
            ,0,0,0,0 //+0x28: Extra Hdr offset
            ,0,0,0,0 //+0x2c: reserve
            ,0,0,0,0 //+0x30: reserve
            ,0,0,0,0 //+0x34: reserve
            ,0,0,0,0 //+0x38: reserve
            ,0,0,0,0 //+0x3c: reserve
        };

        private uint TotalSamples = 0;
        private uint GD3Offset;
        private List<DefineInfo> define;
        private List<TrackInfo> track;
        private ClsVgm mmlInfo;
        private object[] tblChipIdentifyNumber = new object[]
        {
              new object[]{ "SN76489" , 0x0000000c , 1 , 1 , new byte[] { 0,0,0,0 } }
            , new object[]{ "YM2612"  , 0x0000002c , 2 , 2 , null }
        };
        private Dictionary<string, object[]> dicChipIdentifyNumber;
        private int ChipCommandSize = 1;

        public ZGMmaker()
        {
            dicChipIdentifyNumber = new Dictionary<string, object[]>();
            for(int i = 0; i < tblChipIdentifyNumber.Length; i++)
            {
                string key = (string)(((object[])tblChipIdentifyNumber[i])[0]);
                object[] val = (object[])(tblChipIdentifyNumber[i]);
                dicChipIdentifyNumber.Add(key, val);
            }
        }

        public outDatum[] Build(ClsVgm mmlInfo)
        {
            this.mmlInfo = mmlInfo;
            mmlInfo.dat = new List<outDatum>();
            define = new List<DefineInfo>();
            track = new List<TrackInfo>();

            makeAndOutHeaderDiv();
            makeDefineDiv();
            makeTrackDiv();
            outDefineAndTrackDiv();
            makeAndOutExtraDiv();
            makeAndOutGD3Div();
            makeFooterDiv();

            return mmlInfo.dat.ToArray();
        }

        public void OutFile(outDatum[] desBuf,string fn)
        {
            log.Write("ZGMファイル出力");

            List<byte> buf = new List<byte>();
            foreach(outDatum dt in desBuf)
            {
                buf.Add(dt.val);
            }

            File.WriteAllBytes(
                Path.Combine(
                    Path.GetDirectoryName(fn)
                    , Path.GetFileNameWithoutExtension(fn) + ".zgm"
                    )
                , buf.ToArray());
            return;

        }




        private void makeAndOutHeaderDiv()
        {
            foreach(byte b in HeaderTemp)
            {
                outDatum dt = new outDatum(enmMMLType.unknown, null, null, b);
                mmlInfo.dat.Add(dt);
            }
        }

        /// <summary>
        /// DefineDiv
        /// </summary>
        private void makeDefineDiv()
        {
            log.Write("Define division");

            int cmdNo = 0x80;
            foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in mmlInfo.chips)
            {
                foreach (ClsChip chip in kvp.Value)
                {
                    if (!chip.use) continue;
                    if (!dicChipIdentifyNumber.ContainsKey(chip.Name)) continue;

                    log.Write(string.Format("Chip [{0}]", chip.Name));
                    DefineInfo di = new DefineInfo();
                    di.chip = chip;
                    di.chipIdentNo = (uint)(int)dicChipIdentifyNumber[chip.Name][1];
                    di.commandNo = cmdNo++;
                    di.clock = chip.Frequency;
                    define.Add(di);
                }
            }

            ChipCommandSize = cmdNo < 0x100 ? 1 : 2;

        }

        private void makeTrackDiv()
        {
            log.Write("Track division");

            long waitCounter = 0;
            int endChannel = 0;
            int totalChannel = 0;

            foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in mmlInfo.chips)
                foreach (ClsChip chip in kvp.Value)
                    totalChannel += chip.ChMax;

            do
            {
                log.Write("全パートコマンド解析");
                mmlInfo.AnalyzeAllPartCommand();

                log.Write("全パートのうち次のコマンドまで一番近い値を求める");
                waitCounter = mmlInfo.ComputeAllPartDistance();

                log.Write("全パートのwaitcounterを減らす");
                mmlInfo.DecAllPartWaitCounter(waitCounter);

                log.Write("終了パートのカウント");
                endChannel = mmlInfo.CountUpEndPart();
                
            } while (endChannel < totalChannel);

            //残カット
            if (mmlInfo.loopClock != -1 && waitCounter > 0 && waitCounter != long.MaxValue)
            {
                mmlInfo.lClock -= waitCounter;
                mmlInfo.dSample -= (long)(mmlInfo.info.samplesPerClock * waitCounter);
            }
        }

        /// <summary>
        /// Define divとTrackDivを出力する
        /// 
        /// 処理してみないと必要なDefの数とバイト長がわからない為、
        /// makeDefineDivとmakeTrackDivで予め処理したものを
        /// ここで確定値として出力する
        /// </summary>
        private void outDefineAndTrackDiv()
        {

        }

        private void makeAndOutExtraDiv()
        {
            //現時点では未使用の為、何もしない。
        }

        private void makeAndOutGD3Div()
        {
            GD3Offset = (uint)mmlInfo.dat.Count;
            GD3maker gd3 = new GD3maker();
            gd3.make(mmlInfo.dat, mmlInfo.info, mmlInfo.lyric);
        }

        private void makeFooterDiv()
        {
            //ヘッダーの更新
            Common.SetLE32(mmlInfo.dat, 0x04, (uint)(mmlInfo.dat.Count - 1)); //Eof offset
            Common.SetLE32(mmlInfo.dat, 0x0c, TotalSamples); //Total samples
            Common.SetLE32(mmlInfo.dat, 0x18, GD3Offset); //GD3 offset
            Common.SetLE32(mmlInfo.dat, 0x1c, (uint)(define.Count > 0 ? define[0].offset : 0)); //Define offset
            Common.SetLE32(mmlInfo.dat, 0x20, (uint)(track.Count > 0 ? track[0].offset : 0)); //Track 1 offset
            Common.SetLE16(mmlInfo.dat, 0x24, (ushort)define.Count); //Define count
            Common.SetLE16(mmlInfo.dat, 0x26, (ushort)track.Count); //Track count
        }
    }

    public class TrackInfo
    {
        public int offset = 0;
    }

    public class DefineInfo
    {
        public byte length = 14;
        public uint chipIdentNo = 0;
        public int commandNo = 0;
        public int clock = 0;
        public byte[] option = null;

        public ClsChip chip = null;
        public int offset = 0;
    }
}
