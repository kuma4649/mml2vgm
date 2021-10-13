using musicDriverInterface;
using System.Collections.Generic;
using System.IO;

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

        private byte[] DefineTemp = new byte[]
        {
            0x44,0x65,0x66 //'Def'
            ,14 // Length 1byte
            ,0,0,0,0 // Chip Identify number	4byte
            ,0,0//Chip Command number	2byte	
            ,0,0,0,0//Clock					4byte
        };

        private byte[] TrackTemp = new byte[]
        {
            0x54,0x72,0x6b //'Trk'
            ,0,0,0,0 // Length 4byte
            ,255,255,255,255 //LoopAddress 4byte
        };

        private uint TotalSamples = 0;
        private uint GD3Offset;
        private List<DefineInfo> define;
        private List<TrackInfo> track;
        private ClsVgm mmlInfo;
        public static object[] tblChipIdentifyNumber = new object[]
        {
            //              ChipName           Ident                    Port  CmdLen  Option
            // 0x00000000 - 0x000000FF VGM Chips
              new object[]{ "SN76489"        , EnmZGMDevice.SN76489   , 2   , 1     , new byte[] { 0,0,0,0 } }
            , new object[]{ "YM2413"         , EnmZGMDevice.YM2413    , 1   , 2     , null }
            , new object[]{ "YM2612"         , EnmZGMDevice.YM2612    , 2   , 2     , null }
            , new object[]{ "YM2151"         , EnmZGMDevice.YM2151    , 1   , 2     , null }
            , new object[]{ "QSound"         , EnmZGMDevice.QSound    , 1   , 3     , null }
            , new object[]{ "RF5C164"        , EnmZGMDevice.RF5C164   , 1   , 3     , null }
            , new object[]{ "SEGAPCM"        , EnmZGMDevice.SegaPCM   , 1   , 3     , new byte[] { 0,0,0,0 } }
            , new object[]{ "YM2203"         , EnmZGMDevice.YM2203    , 1   , 2     , null }
            , new object[]{ "YM2608"         , EnmZGMDevice.YM2608    , 2   , 2     , null }
            , new object[]{ "YM2610B"        , EnmZGMDevice.YM2610    , 2   , 2     , null }
            , new object[]{ "Y8950"          , EnmZGMDevice.Y8950     , 1   , 2     , null }
            , new object[]{ "AY8910"         , EnmZGMDevice.AY8910    , 1   , 2     , null }
            , new object[]{ "DMG"            , EnmZGMDevice.GameBoyDMG, 1   , 2     , null }
            , new object[]{ "NES"            , EnmZGMDevice.NESAPU    , 1   , 2     , null }
            , new object[]{ "K051649"        , EnmZGMDevice.K051649   , 1   , 2     , null }
            , new object[]{ "K053260"        , EnmZGMDevice.K053260   , 1   , 3     , new byte[] { 0 } }
            , new object[]{ "HuC6280"        , EnmZGMDevice.HuC6280   , 1   , 2     , null }
            , new object[]{ "C140"           , EnmZGMDevice.C140      , 1   , 2     , new byte[] { 0 } }
            , new object[]{ "C352"           , EnmZGMDevice.C352      , 1   , 2     , null }
            // 0x00010000 - 0x0001FFFF 妄想Chips
            , new object[]{ "CONDUCTOR"      , EnmZGMDevice.Conductor , 1   , 2     , null }
            , new object[]{ "VRC6"           , EnmZGMDevice.VRC6      , 1   , 2     , null }
            // 0x00020000 - 0x0002FFFF 妄想Chips
            , new object[]{ "AY8910B"        , EnmZGMDevice.AY8910B   , 1   , 2     , null }
            , new object[]{ "YM2609"         , EnmZGMDevice.YM2609    , 4   , 2     , null }
            // 0x00030000 - 0x0003FFFF XG音源
            , new object[]{ "MU50"           , EnmZGMDevice.MU50      , 1   , -1    , null }
            // 0x00040000 - 0x0004FFFF LA/GS音源
            , new object[]{ "MT-32"          , EnmZGMDevice.MT32      , 1   , -1    , null }
            // 0x00050000 - 0x0005FFFF GM
            , new object[]{ "GeneralMIDI"    , EnmZGMDevice.MIDIGM    , 1   , -1    , null }
            // 0x00060000 - 0x0006FFFF VSTi
            , new object[]{ "VOPMex"         , EnmZGMDevice.VOPMex    , 1   , -1    , null }
            // 0x00070000 - 0x0007FFFF Wave
            , new object[]{ "Raw Wave"       , EnmZGMDevice.Wave      , 1   , -1    , null }
        };
        private Dictionary<string, object[]> dicChipIdentifyNumber;
        private int ChipCommandSize = 1;

        public bool useSkipPlayCommand { get; private set; } = false;

        public ZGMmaker()
        {
            dicChipIdentifyNumber = new Dictionary<string, object[]>();
            for (int i = 0; i < tblChipIdentifyNumber.Length; i++)
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
            makeAndOutExtraDiv();
            makeAndOutGD3Div();
            makeFooterDiv();

            return mmlInfo.dat.ToArray();
        }

        public void OutFile(outDatum[] desBuf, string fn)
        {
            log.Write("ZGMファイル出力");

            List<byte> buf = new List<byte>();
            foreach (outDatum dt in desBuf)
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
            foreach (byte b in HeaderTemp)
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
                    if (chip == null) continue;
                    if (!chip.use) continue;
                    if (!dicChipIdentifyNumber.ContainsKey(chip.Name)) continue;

                    log.Write(string.Format("Chip [{0}]", chip.Name));
                    DefineInfo di = new DefineInfo();
                    di.chip = chip;
                    di.chipIdentNo = (uint)(int)dicChipIdentifyNumber[chip.Name][1];
                    di.commandNo = cmdNo;
                    chip.ChipID = di.commandNo;
                    chip.port = new byte[(int)dicChipIdentifyNumber[chip.Name][2]][];
                    for (int i = 0; i < (int)dicChipIdentifyNumber[chip.Name][2]; i++)
                    {
                        chip.port[i] = new byte[] { (byte)cmdNo, (byte)(cmdNo >> 8) };
                        cmdNo++;
                    }

                    di.clock = chip.Frequency;
                    if (chip is YM2610B) di.clock = (int)(di.clock | 0x8000_0000);

                    if (dicChipIdentifyNumber[chip.Name][4] != null)
                    {
                        di.option = new byte[((byte[])dicChipIdentifyNumber[chip.Name][4]).Length];
                        for (int i = 0; i < ((byte[])dicChipIdentifyNumber[chip.Name][4]).Length; i++)
                        {
                            di.option[i] = ((byte[])dicChipIdentifyNumber[chip.Name][4])[i];
                        }
                    }
                    define.Add(di);


                    for (int i = 0; i < chip.lstPartWork.Count; i++)
                    {
                        chip.InitPart(chip.lstPartWork[i]);
                    }
                }
            }

            ChipCommandSize = cmdNo < 0x100 ? 1 : 2;
            mmlInfo.ChipCommandSize = ChipCommandSize;
            if (ChipCommandSize == 1)
            {
                //コマンドサイズが1byteの時はポートの配列を1byteに定義しなおす
                foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in mmlInfo.chips)
                {
                    foreach (ClsChip chip in kvp.Value)
                    {
                        if (chip == null) continue;

                        if (!chip.use) continue;
                        if (chip.port == null) continue;
                        for (int i = 0; i < chip.port.Length; i++)
                        {
                            chip.port[i] = new byte[] { chip.port[i][0] };
                        }
                    }
                }
            }

            foreach (DefineInfo di in define)
            {
                int pos = mmlInfo.dat.Count;
                di.offset = pos;
                foreach (byte b in DefineTemp)
                {
                    outDatum dt = new outDatum(enmMMLType.unknown, null, null, b);
                    mmlInfo.dat.Add(dt);
                }

                mmlInfo.dat[pos + 0x03].val = (byte)(di.length + (di.option != null ? di.option.Length : 0));
                Common.SetLE32(mmlInfo.dat, (uint)(pos + 0x04), (uint)(di.chipIdentNo));
                Common.SetLE16(mmlInfo.dat, (uint)(pos + 0x08), (ushort)(di.chip.port[0][0] + (di.chip.port[0].Length > 1 ? di.chip.port[0][1] : 0) * 256));
                Common.SetLE32(mmlInfo.dat, (uint)(pos + 0x0a), (uint)(di.clock));
                if (di.option != null)
                {
                    foreach (byte b in di.option)
                    {
                        outDatum dt = new outDatum(enmMMLType.unknown, null, null, b);
                        mmlInfo.dat.Add(dt);
                    }
                }
            }
        }

        private void makeTrackDiv()
        {
            log.Write("Track division");

            long waitCounter = 0;
            int endChannel = 0;
            int totalChannel = 0;

            TrackInfo ti = new TrackInfo();
            ti.offset = mmlInfo.dat.Count;
            track.Add(ti);

            foreach (byte b in TrackTemp)
            {
                outDatum dt = new outDatum(enmMMLType.unknown, null, null, b);
                mmlInfo.dat.Add(dt);
            }

            //PCM Data block
            foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in mmlInfo.chips)
            {
                foreach (ClsChip chip in kvp.Value)
                {
                    if (chip == null) continue;

                    chip.SetPCMDataBlock(null);
                }
            }

            //Set Initialize data
            foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in mmlInfo.chips)
            {
                foreach (ClsChip chip in kvp.Value)
                {
                    if (chip == null) continue;
                    chip.InitChip();
                    foreach (partWork pw in chip.lstPartWork)
                    {
                        foreach (partPage pg in pw.pg)
                        {
                            mmlInfo.OutData(pg.sendData);
                            pg.sendData.Clear();
                        }
                    }
                }
            }

            foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in mmlInfo.chips)
                foreach (ClsChip chip in kvp.Value)
                {
                    if (chip == null) continue;
                    totalChannel += chip.ChMax;
                }

            do
            {
                log.Write("全パートコマンド解析");
                mmlInfo.AnalyzeAllPartCommand();

                log.Write("全パートのうち次のコマンドまで一番近い値を求める");
                waitCounter = mmlInfo.ComputeAllPartDistance();

                //log.Write("全パートのwaitcounterを減らす");
                //mmlInfo.DecAllPartWaitCounter(waitCounter);

                log.Write("終了パートのカウント");
                endChannel = mmlInfo.CountUpEndPart();

                if (endChannel < totalChannel)
                {
                    log.Write("全パートのwaitcounterを減らす");
                    mmlInfo.DecAllPartWaitCounter(waitCounter);
                }

            } while (endChannel < totalChannel && !mmlInfo.compileEnd);

            mmlInfo.CompClockCounter();
            //残カット
            //if (mmlInfo.loopClock != -1 && waitCounter > 0 && waitCounter != long.MaxValue)
            //{
            //    mmlInfo.lClock -= waitCounter;
            //    mmlInfo.dSample -= (long)(mmlInfo.info.samplesPerClock * waitCounter);
            //}

            Common.SetLE32(mmlInfo.dat, (uint)(ti.offset + 0x03), (uint)(mmlInfo.dat.Count - ti.offset));
            Common.SetLE32(mmlInfo.dat, (uint)(ti.offset + 0x07), (uint)(mmlInfo.dummyCmdLoopOffset));
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
