using musicDriverInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Xml.Linq;
using static MDSound.XGMFunction;

namespace Core
{
    public class XGM2maker
    {
        private byte[] HeaderTemp = new byte[]
        {
            0x58 , 0x47 , 0x4d , 0x32 //'XGM2'
            ,0x10 //+0x04: Version
            ,0 //+0x05: Flags( b0:isNTSC  b1:isMultiTrack  b2:existGD3  b3:packedData )
            ,0,0 // +0x06: sampleDataBlockSize / 256
            ,0,0 // +0x08: fmDataBlockSize / 256
            ,0,0 // +0x0a: psgDataBlockSize / 256
        };

        private ClsVgm mmlInfo;
        private bool isNTSC;
        private bool isMultiTrack;
        private bool existGD3;
        private bool packedData;
        private bool exportMode = false;
        private XGMSampleID[] sampleID;

        private uint[] fmID = new uint[128];
        private uint[] psgID = new uint[128];
        private uint[] gd3ID = new uint[128];

        private List<outDatum> headData = null;
        private List<outDatum> pcmData = null;
        private List<outDatum> fmData = null;
        private List<outDatum> psgData = null;
        private int fmFrameCounter = 0;
        private int psgFrameCounter = 0;
        private int fmFrameLength = 0;
        private int psgFrameLength = 0;
        private Action<string> disp;
        private Dictionary<int, int> fmFrameLengthDic;
        private Dictionary<int, int> psgFrameLengthDic;

        public XGM2maker(Action<string> disp=null)
        {
            this.disp = disp;
        }

        public outDatum[] Build(ClsVgm mmlInfo,bool outVgmFile,bool writeFileMode)
        {
            Disp("Start xgm2 build.");

            exportMode = outVgmFile && writeFileMode;
            this.mmlInfo = mmlInfo;
            mmlInfo.dat = new List<outDatum>();
            fmFrameCounter = 0;
            psgFrameCounter = 0;
            fmFrameLength = 0;
            fmFrameLengthDic=new Dictionary<int, int>();
            psgFrameLength = 0;
            psgFrameLengthDic = new Dictionary<int, int>();

            makeAndOutHeaderDiv();
            headData = mmlInfo.dat;
            mmlInfo.dat = new List<outDatum>();
            makePCMData();
            pcmData = mmlInfo.dat;
            mmlInfo.dat = new List<outDatum>();

            Dictionary<enmChipType, ClsChip[]> bd = mmlInfo.chips;
            Dictionary<enmChipType, ClsChip[]> nd;

            mmlInfo.dat.Clear();
            mmlInfo.chips = getChips(true, bd);
            fmData = makeFMtrack();
            fmData = ShapingFMDat(fmData);

            mmlInfo.dat.Clear();
            mmlInfo.chips = getChips(false, bd);
            psgData = makePSGtrack();
            psgData = ShapingPSGDat(psgData);

            PaddingFM();

            CheckFrameLength();

            mmlInfo.chips = bd;
            makeFooterDiv();

            Disp("FM  frame(s) : {0}", fmFrameCounter);
            Disp("PSG frame(s) : {0}", psgFrameCounter);
            Disp("Finish xgm2 build.");

            return mmlInfo.dat.ToArray();
        }

        private void Disp(string v,params object[] prm)
        {
            if (disp == null) return;
            disp(string.Format(v,prm));
        }

        public void OutFile(outDatum[] buf, string fn)
        {
            log.Write(msg.get("I04028"));// XGM2ファイル出力

            List<byte> b = new List<byte>();
            foreach (outDatum dt in buf)
            {
                if (dt.type == enmMMLType.IDE && dt.args.Count > 0 && dt.args[0] is outDatum)
                    continue;
                b.Add(dt.val);
            }

            File.WriteAllBytes(
                Path.Combine(
                    Path.GetDirectoryName(fn)
                    , Path.GetFileNameWithoutExtension(fn) + ".xgm"
                    )
                , b.ToArray());
            return;
        }


        private static Dictionary<enmChipType, ClsChip[]> getChips(bool isOPN2, Dictionary<enmChipType, ClsChip[]> bd)
        {
            Dictionary<enmChipType, ClsChip[]> nd = new Dictionary<enmChipType, ClsChip[]>();
            foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in bd)
            {
                if (kvp.Value == null) continue;
                if (kvp.Value.Length < 1) continue;
                if ((kvp.Key != (isOPN2 ? enmChipType.YM2612X2 : enmChipType.SN76489X2))
                    && (kvp.Key != enmChipType.CONDUCTOR)) continue;
                nd.Add(kvp.Key, kvp.Value);
            }

            return nd;
        }

        private void makeAndOutHeaderDiv()
        {
            foreach (byte b in HeaderTemp)
            {
                outDatum dt = new outDatum(enmMMLType.unknown, null, null, b);
                mmlInfo.dat.Add(dt);
            }

            isNTSC = mmlInfo.info.xgmSamplesPerSecond == 60;
            isMultiTrack = false;
            existGD3 = CheckInfo();
            packedData = false;
            mmlInfo.dat[5].val =
                (byte)(
                (isNTSC ? 0x0 : 0x1)  //bit0 0:NTSC 1:PAL
                | (isMultiTrack ? 0x2 : 0x0)//bit1 0:No 1:Yes 
                | (existGD3 ? 0x4 : 0x0) //bit2 0:No 1:Yes 
                | (packedData ? 0x8 : 0x0)//bit3 0:No 1:Yes 
                );

            sampleID = new XGMSampleID[((isMultiTrack ? 504 : 248) - 12) / 2];
            for (uint i = 0; i < sampleID.Length; i++)
            {
                sampleID[i] = new XGMSampleID();
                sampleID[i].addr = (uint)(i == 0 ? 0 : 0xffff00);//0xffff00 is empty
                sampleID[i].size = 0x000000;
                mmlInfo.dat.Add(new outDatum(enmMMLType.unknown, null, null, (byte)(i == 0 ? 0 : 0xff)));
                mmlInfo.dat.Add(new outDatum(enmMMLType.unknown, null, null, (byte)(i == 0 ? 0 : 0xff)));
            }
            for (uint i = 0; i < 3; i++)//PCM SFX
            {
                mmlInfo.dat.Add(new outDatum(enmMMLType.unknown, null, null, 0xff));
                mmlInfo.dat.Add(new outDatum(enmMMLType.unknown, null, null, 0xff));
                mmlInfo.dat.Add(new outDatum(enmMMLType.unknown, null, null, 0xff));
                mmlInfo.dat.Add(new outDatum(enmMMLType.unknown, null, null, 0xff));
            }

            if (isMultiTrack)
            {
                for (uint i = 0; i < 128; i++)
                {
                    fmID[i] = 0xffff00;//0xffff00 is empty
                    mmlInfo.dat.Add(new outDatum(enmMMLType.unknown, null, null, 0xff));
                    mmlInfo.dat.Add(new outDatum(enmMMLType.unknown, null, null, 0xff));
                }
                for (uint i = 0; i < 128; i++)
                {
                    psgID[i] = 0xffff00;//0xffff00 is empty
                    mmlInfo.dat.Add(new outDatum(enmMMLType.unknown, null, null, 0xff));
                    mmlInfo.dat.Add(new outDatum(enmMMLType.unknown, null, null, 0xff));
                }
                for (uint i = 0; i < 128; i++)
                {
                    gd3ID[i] = 0xffff00;//0xffff00 is empty
                }
            }

        }

        private void makeFooterDiv()
        {
            mmlInfo.dat.Clear();
            //headerDataBlock
            if (headData != null) foreach (outDatum dt in headData) mmlInfo.dat.Add(dt);
            //sampleDataBlock
            if (pcmData != null) foreach (outDatum dt in pcmData) mmlInfo.dat.Add(dt);
            //fmDataBlock
            if (fmData != null) foreach (outDatum dt in fmData) mmlInfo.dat.Add(dt);
            //psgDataBlock
            if (psgData != null) foreach (outDatum dt in psgData) mmlInfo.dat.Add(dt);

            if (isMultiTrack)
            {
                for (uint i = 0; i < 128; i++)
                {
                    outDatum dt;
                    dt = new outDatum(enmMMLType.unknown, null, null, (byte)(gd3ID[i] >> 8));
                    mmlInfo.dat.Add(dt);
                    dt = new outDatum(enmMMLType.unknown, null, null, (byte)(gd3ID[i] >> 16));
                    mmlInfo.dat.Add(dt);
                }
            }

            if (existGD3)
            {
                GD3maker gm = new GD3maker();
                List<outDatum> dat = new List<outDatum>();
                gm.make(dat, mmlInfo.info, null);
                foreach (outDatum dt in dat) mmlInfo.dat.Add(dt);
            }

            //ヘッダ情報の編集
            uint size;
            size = (uint)pcmData.Count;
            mmlInfo.dat[0x6].val = (byte)(size >> 8);
            mmlInfo.dat[0x7].val = (byte)(size >> 16);
            size = (uint)fmData.Count;
            mmlInfo.dat[0x8].val = (byte)(size >> 8);
            mmlInfo.dat[0x9].val = (byte)(size >> 16);
            size = (uint)psgData.Count;
            mmlInfo.dat[0xa].val = (byte)(size >> 8);
            mmlInfo.dat[0xb].val = (byte)(size >> 16);
            for (int i = 0; i < sampleID.Length; i++)
            {
                mmlInfo.dat[i * 2 + 0x0c].val = (byte)(sampleID[i].addr >> 8);
                mmlInfo.dat[i * 2 + 0x0d].val = (byte)(sampleID[i].addr >> 16);
            }

        }


        private bool CheckInfo()
        {
            if (mmlInfo.info.Composer != "") return true;
            if (mmlInfo.info.ComposerJ != "") return true;
            if (mmlInfo.info.Converted != "") return true;
            if (mmlInfo.info.GameName != "") return true;
            if (mmlInfo.info.GameNameJ != "") return true;
            if (mmlInfo.info.Notes != "") return true;
            if (mmlInfo.info.ReleaseDate != "") return true;
            if (mmlInfo.info.SystemName != "") return true;
            if (mmlInfo.info.SystemNameJ != "") return true;
            if (mmlInfo.info.TitleName != "") return true;
            if (mmlInfo.info.TitleNameJ != "") return true;

            return false;
        }

        private void makePCMData()
        {
            log.Write("PCMData division");

            //PCM Data block
            foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in mmlInfo.chips)
            {
                foreach (ClsChip chip in kvp.Value)
                {
                    if (chip == null) continue;
                    if (!(chip is YM2612X2)) continue;

                    chip.SetPCMDataBlock(null);

                }
            }

            int c = 0;
            uint ptr = 0;
            foreach(var p in mmlInfo.instPCM.Values)
            {
                sampleID[c].addr = ptr;
                sampleID[c].size = (uint)p.Item2.size;
                ptr+= (uint)p.Item2.size;
                c++;
            }

        }

        private List<outDatum> makeFMtrack()
        {
            log.Write("FM Data division");

            List<outDatum> dat = new List<outDatum>();

            //Set Initialize data
            foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in mmlInfo.chips)
            {
                foreach (ClsChip chip in kvp.Value)
                {
                    chip.InitChip();
                    foreach (partWork pw in chip.lstPartWork)
                    {
                        foreach (partPage pg in pw.pg)
                        {
                            string msg = "SDataFlashing";
                            if (pg.sendData == null || pg.sendData.Count == 0) continue;
                            foreach (outDatum od in pg.sendData)
                            {
                                dat.Add(od);
                                msg += string.Format("{0:x02} :", od.val);
                            }
                            log.ForcedWrite(msg);
                            pg.sendData.Clear();
                        }
                    }
                }
            }

            long waitCounter;
            int endChannel;
            int totalChannel = 0;

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

                log.Write("終了パートのカウント");
                endChannel = mmlInfo.CountUpEndPart();

                if (endChannel < totalChannel)
                {
                    log.Write("全パートのwaitcounterを減らす");
                    mmlInfo.DecAllPartWaitCounter(waitCounter);
                }

            } while (endChannel < totalChannel && !mmlInfo.compileEnd);

            mmlInfo.CompClockCounter();

            outDatum o;
            for (int i = 0; i < mmlInfo.dat.Count; i++)
            {
                o = mmlInfo.dat[i];
                dat.Add(o);
            }

            //end mark
            o = new outDatum();
            o.val = 0xff;
            dat.Add(o);
            dat.Add(o);
            dat.Add(o);
            dat.Add(o);

            return dat;
        }

        private List<outDatum> ShapingFMDat(List<outDatum> src)
        {
            log.Write("FM shaping division");

            List<outDatum> ret = new List<outDatum>();
            for (int i = 0; i < src.Count; i++)
            {
                outDatum od = src[i];
                if (od == null) continue;
                switch (od.val)
                {
                    case 0:
                    case 1:
                        List<outDatum> e0 = new List<outDatum>();
                        outDatum d = od.Copy();
                        d.val = 0xe0;
                        e0.Add(d);
                        fmFrameLength++;
                        int cnt = 0;
                        i--;
                        do
                        {
                            i++;
                            e0.Add(src[++i]);
                            e0.Add(src[++i]);
                            fmFrameLength += 2;
                            cnt++;
                        } while (src[i + 1].val == od.val && cnt < 8);
                        e0[0].val |= (byte)(od.val * 8 + (cnt - 1));
                        foreach (outDatum e in e0) ret.Add(e);
                        break;
                    case 2:
                        outDatum cmd = src[++i];
                        ret.Add(cmd);
                        switch (cmd.val & 0xf0)
                        {
                            case 0x00://size 1(0x0fの場合は2)
                            case 0x40:
                            case 0x50:
                            case 0x60:
                            case 0x70:
                                if (cmd.val == 0x0f) ret.Add(src[++i]);
                                fmFrameLength++;
                                break;
                            case 0x10://size 2
                                if (exportMode)
                                {
                                    //PCM発音すると雑音が酷いためカット
                                    ret.RemoveAt(ret.Count - 1);
                                    i++;
                                }
                                else
                                {
                                    ret.Add(src[++i]);
                                }
                                fmFrameLength += 2;
                                break;
                            case 0x90://size 2
                            case 0xa0:
                            case 0xb0:
                            case 0xc0:
                            case 0xd0:
                                ret.Add(src[++i]);
                                fmFrameLength += 2;
                                break;
                            case 0x20://size 31
                                for (int j = 0; j < 30; j++) ret.Add(src[++i]);
                                fmFrameLength += 31;
                                break;
                            case 0x30://size 3
                            case 0x80:
                                ret.Add(src[++i]);
                                ret.Add(src[++i]);
                                fmFrameLength += 3;
                                break;
                            case 0xe0://size 1+2(x+1)
                                fmFrameLength++;
                                for (int j = 0; j < (cmd.val & 0x7) + 1; j++)
                                {
                                    ret.Add(src[++i]);
                                    ret.Add(src[++i]);
                                    fmFrameLength += 2;
                                }
                                break;
                            case 0xf0://size1/2/4
                                switch (cmd.val & 0xf)
                                {
                                    case 0x8://size 2
                                    case 0x9:
                                        ret.Add(src[++i]);
                                        fmFrameLength += 2;
                                        break;
                                    case 0xf://size 4
                                        ret.Add(src[++i]);
                                        ret.Add(src[++i]);
                                        ret.Add(src[++i]);
                                        fmFrameLength += 4;
                                        break;
                                }
                                break;
                        }
                        break;
                    case 0x2f: //制御むけダミーデータ
                        if (!exportMode)
                        {
                            outDatum dmy = new outDatum();
                            dmy.type = enmMMLType.IDE;
                            dmy.args = new List<object>();
                            dmy.args.Add(src[i]);
                            dmy.args.Add(src[i + 1]);
                            dmy.args.Add(src[i + 2]);
                            ret.Add(dmy);
                        }
                        i += 2;
                        break;
                    case 0x61: //wait
                        if (fmFrameLengthDic.ContainsKey(fmFrameCounter))
                            fmFrameLengthDic[fmFrameCounter] += fmFrameLength;
                        else
                            fmFrameLengthDic.Add(fmFrameCounter, fmFrameLength);
                        //if (fmFrameLength > 255)
                        //{
                        //    Disp("fm frame Length : {0}(At frame {1})", fmFrameLength, fmFrameCounter);
                        //}
                        int w = src[i + 1].val + src[i + 2].val * 256;//ex) 21clock -> 16 + 5 -> [0x0f 0x05]
                        fmFrameCounter += w;
                        fmFrameLength = 0;
                        ret = AddWaitFM(ret, w);
                        i += 2;
                        break;
                    case 0xff://loop/end mark
                        ret.Add(src[i]);
                        ret.Add(src[++i]);
                        ret.Add(src[++i]);
                        ret.Add(src[++i]);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return ret;
        }

        private void PaddingFM()
        {
            //padding
            int pad = 256 - (fmData.Count % 256);
            outDatum p = new outDatum();
            p.val = 0x00;
            if (pad != 256) for (int i = 0; i < pad; i++) fmData.Add(p);
        }

        private List<outDatum> makePSGtrack()
        {
            log.Write("PSG Data division");
            List<outDatum> dat = new List<outDatum>();

            //Set Initialize data
            foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in mmlInfo.chips)
            {
                foreach (ClsChip chip in kvp.Value)
                {
                    chip.InitChip();
                    foreach (partWork pw in chip.lstPartWork)
                    {
                        foreach (partPage pg in pw.pg)
                        {
                            string msg = "SDataFlashing";
                            if (pg.sendData == null || pg.sendData.Count == 0) continue;
                            foreach (outDatum od in pg.sendData)
                            {
                                dat.Add(od);
                                msg += string.Format("{0:x02} :", od.val);
                            }
                            log.ForcedWrite(msg);
                            pg.sendData.Clear();
                        }
                    }
                }
            }

            long waitCounter;
            int endChannel;
            int totalChannel = 0;

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

                log.Write("終了パートのカウント");
                endChannel = mmlInfo.CountUpEndPart();

                if (endChannel < totalChannel)
                {
                    log.Write("全パートのwaitcounterを減らす");
                    mmlInfo.DecAllPartWaitCounter(waitCounter);
                }

            } while (endChannel < totalChannel && !mmlInfo.compileEnd);

            mmlInfo.CompClockCounter();

            outDatum o;
            for (int i = 0; i < mmlInfo.dat.Count; i++)
            {
                o = mmlInfo.dat[i];
                dat.Add(o);
            }

            //end mark
            o = new outDatum();
            o.val = 0x0f;
            dat.Add(o);
            o = new outDatum();
            o.val = 0xff;
            dat.Add(o);
            dat.Add(o);
            dat.Add(o);

            return dat;
        }

        private List<outDatum> ShapingPSGDat(List<outDatum> src)
        {
            log.Write("PSG shaping division");

            List<outDatum> ret = new List<outDatum>();
            for (int i = 0; i < src.Count; i++)
            {
                outDatum od = src[i];
                if (od == null) continue;
                switch (od.val)
                {
                    case 0x2f: //制御むけダミーデータ
                        if (!exportMode)
                        {
                            outDatum dmy = new outDatum();
                            dmy.type = enmMMLType.IDE;
                            dmy.args = new List<object>();
                            dmy.args.Add(src[i]);
                            dmy.args.Add(src[i + 1]);
                            dmy.args.Add(src[i + 2]);
                            ret.Add(dmy);
                        }
                        i += 2;
                        break;
                    case 0x50:
                        outDatum cmd = src[++i];
                        ret.Add(cmd);
                        switch (cmd.val & 0xf0)
                        {
                            case 0x00://size 1(0x0eの場合は2 0x0fの場合は4)
                                psgFrameLength++;
                                if (cmd.val == 0x0e)
                                {
                                    ret.Add(src[++i]);
                                    psgFrameLength++;
                                }
                                else if (cmd.val == 0x0f)
                                {
                                    ret.Add(src[++i]);
                                    ret.Add(src[++i]);
                                    ret.Add(src[++i]);
                                    psgFrameLength += 3;
                                }
                                break;
                            case 0x10://size 2
                            case 0x20:
                            case 0x30:
                                ret.Add(src[++i]);
                                psgFrameLength += 2;
                                break;
                            case 0x40://size 1
                            case 0x50:
                            case 0x60:
                            case 0x70:
                            case 0x80:
                            case 0x90:
                            case 0xa0:
                            case 0xb0:
                            case 0xc0:
                            case 0xd0:
                            case 0xe0:
                            case 0xf0:
                                psgFrameLength++;
                                break;
                        }
                        break;
                    case 0x61: //wait
                        if (psgFrameLengthDic.ContainsKey(psgFrameCounter))
                            psgFrameLengthDic[psgFrameCounter] += psgFrameLength;
                        else
                            psgFrameLengthDic.Add(psgFrameCounter, psgFrameLength);
                        int w = src[i + 1].val + src[i + 2].val * 256;//ex) 21clock -> 15 + 6 -> [0x0e 0x06]
                        psgFrameCounter += w;
                        psgFrameLength = 0;
                        ret = AddWaitPSG(ret, w);
                        i += 2;
                        break;
                    case 0x0f://loop/end mark
                        if (psgFrameCounter < fmFrameCounter)
                        {
                            ret = AddWaitPSG(ret, fmFrameCounter - psgFrameCounter);
                        }
                        else if (fmFrameCounter < psgFrameCounter)
                        {
                            outDatum[] loopEndMark= new outDatum[4];
                            loopEndMark[0] = fmData[fmData.Count - 4];
                            loopEndMark[1] = fmData[fmData.Count - 3];
                            loopEndMark[2] = fmData[fmData.Count - 2];
                            loopEndMark[3] = fmData[fmData.Count - 1];
                            fmData.RemoveAt(fmData.Count - 1);
                            fmData.RemoveAt(fmData.Count - 1);
                            fmData.RemoveAt(fmData.Count - 1);
                            fmData.RemoveAt(fmData.Count - 1);
                            fmData = AddWaitFM(fmData, psgFrameCounter - fmFrameCounter);
                            fmData.Add(loopEndMark[0]);
                            fmData.Add(loopEndMark[1]);
                            fmData.Add(loopEndMark[2]);
                            fmData.Add(loopEndMark[3]);
                        }
                        ret.Add(src[i]);
                        ret.Add(src[++i]);
                        ret.Add(src[++i]);
                        ret.Add(src[++i]);
                        break;
                }
            }

            //padding
            int pad = 256 - (ret.Count % 256);
            outDatum p = new outDatum();
            p.val = 0x00;
            if (pad != 256) for (int i = 0; i < pad; i++) ret.Add(p);

            return ret;
        }

        private List<outDatum> AddWaitFM(List<outDatum> ret, int w)
        {
            while (w > 0)
            {
                if (w < 16)
                {
                    outDatum wa = new outDatum();
                    wa.val = (byte)(0x00 + w - 1);
                    ret.Add(wa);
                    w = 0;
                }
                else
                {
                    w -= 16;
                    outDatum wa = new outDatum();
                    wa.val = 0x0f;
                    ret.Add(wa);
                    if (w < 256)
                    {
                        wa = new outDatum();
                        wa.val = (byte)w;
                        ret.Add(wa);
                        w = 0;
                    }
                    else
                    {
                        wa = new outDatum();
                        wa.val = (byte)0xff;
                        ret.Add(wa);
                        w -= 255;
                    }
                }
            }
            return ret;
        }

        private List<outDatum> AddWaitPSG(List<outDatum> ret,int w)
        {
            while (w > 0)
            {
                if (w < 15)
                {
                    outDatum wa = new outDatum();
                    wa.val = (byte)(0x00 + w - 1);
                    ret.Add(wa);
                    w = 0;
                }
                else
                {
                    w -= 15;
                    outDatum wa = new outDatum();
                    wa.val = 0x0e;
                    ret.Add(wa);
                    if (w < 256)
                    {
                        wa = new outDatum();
                        wa.val = (byte)w;
                        ret.Add(wa);
                        w = 0;
                    }
                    else
                    {
                        wa = new outDatum();
                        wa.val = (byte)0xff;
                        ret.Add(wa);
                        w -= 255;
                    }
                }
            }
            return ret;
        }

        private void CheckFrameLength()
        {
            for(int i = 0;i<Math.Max(fmFrameCounter,psgFrameCounter);i++)
            {
                if (!fmFrameLengthDic.ContainsKey(i) && !psgFrameLengthDic.ContainsKey(i)) continue;
                int len = 0;
                if (fmFrameLengthDic.ContainsKey(i)) len += fmFrameLengthDic[i];
                if (psgFrameLengthDic.ContainsKey(i)) len += psgFrameLengthDic[i];
                if (len < 256) continue;
                msgBox.setWrnMsg(string.Format("{0} frame is too long.(length : {1})", i, len), null);
            }
        }




    }
}