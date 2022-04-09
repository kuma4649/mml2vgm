using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mml2vgmIDE
{
    public static class muteManager
    {
        //
        // UpdateTrackInfoを使ってドキュメント毎のパートの情報を登録する
        // 登録するとそのパートに対応するpartKeyが取得できるので、
        // 以降はその番号を使用して、mute情報にアクセスできる
        // (スピード優先の場合は予めこのpartKeyを保持しておく。)
        //
        // ドキュメント単位で、mute soloをリセット/セットする機能
        // TBD:MuteSoloの情報にアクセスできるようにする
        // パート情報からもアクセスできるようにする
        //

        /// <summary>
        /// ドキュメント毎にパートのミュート状態を保持する
        ///   1 : Document
        ///   2 : partKey(ドキュメントのパート毎にユニークな番号)
        ///  ans: muteStatus(mute状態を保持している)
        /// </summary>
        private static Dictionary<Document, Dictionary<int, muteStatus>> lstMuteStatus = new Dictionary<Document, Dictionary<int, muteStatus>>();

        /// <summary>
        /// partKeyの辞書
        ///   1 : Document
        ///   2 : partNumber(int 
        ///   3 : chipIndex(int 
        ///   4 : chipNumber(int
        ///   5 : trackName(str 
        ///   6 : chipName(str
        ///  ans: キー番号(int
        /// </summary>
        private static Dictionary<Document, Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<string, Dictionary<string, int>>>>>> dicKey
            = new Dictionary<Document, Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<string, Dictionary<string, int>>>>>>();

        /// <summary>
        /// ドキュメント毎の
        /// ユニークキーナンバー（どんどんカウントアップする）
        /// </summary>
        private static Dictionary<Document, int> keyNum = new Dictionary<Document, int>();
        private static Dictionary<Document, bool> SoloMode = new Dictionary<Document, bool>();

        /// <summary>
        /// カレントドキュメント
        /// </summary>
        private static Document crntDoc = null;


        /// <summary>
        /// 
        ///   1 : Document
        ///   2 : chipName(str
        ///   3 : chipIndex(int 
        ///   4 : chipNumber(int
        ///  ans: チップ単位のmute配列
        /// </summary>
        private static Dictionary<Document, Dictionary<string, Dictionary<int, Dictionary<int, List<bool>>>>> chipMuteStatus
            = new Dictionary<Document, Dictionary<string, Dictionary<int, Dictionary<int, List<bool>>>>>();

        public static void SetChipMute(Document doc, string chipName, int chipIndex, int chipNumber, int partNumber, bool mute)
        {
            Dictionary<string, Dictionary<int, Dictionary<int, List<bool>>>> dic1;
            Dictionary<int, Dictionary<int, List<bool>>> dic2;
            Dictionary<int, List<bool>> dic3;
            List<bool> lst;

            if (!chipMuteStatus.ContainsKey(doc))
                chipMuteStatus.Add(doc, new Dictionary<string, Dictionary<int, Dictionary<int, List<bool>>>>());
            dic1 = chipMuteStatus[doc];

            if (!dic1.ContainsKey(chipName))
                dic1.Add(chipName, new Dictionary<int, Dictionary<int, List<bool>>>());
            dic2 = dic1[chipName];

            if (!dic2.ContainsKey(chipIndex))
                dic2.Add(chipIndex, new Dictionary<int, List<bool>>());
            dic3 = dic2[chipIndex];

            if (!dic3.ContainsKey(chipNumber))
                dic3.Add(chipNumber, new List<bool>());
            lst = dic3[chipNumber];

            while (partNumber > lst.Count)
            {
                lst.Add(false);
            }

            lst[partNumber - 1] = mute;
        }

        public static bool GetChipMute(Document doc, string chipName, int chipIndex, int chipNumber, int partNumber)
        {
            Dictionary<string, Dictionary<int, Dictionary<int, List<bool>>>> dic1;
            Dictionary<int, Dictionary<int, List<bool>>> dic2;
            Dictionary<int, List<bool>> dic3;
            List<bool> lst;

            if (!chipMuteStatus.ContainsKey(doc))
                chipMuteStatus.Add(doc, new Dictionary<string, Dictionary<int, Dictionary<int, List<bool>>>>());
            dic1 = chipMuteStatus[doc];

            if (!dic1.ContainsKey(chipName))
                dic1.Add(chipName, new Dictionary<int, Dictionary<int, List<bool>>>());
            dic2 = dic1[chipName];

            if (!dic2.ContainsKey(chipIndex))
                dic2.Add(chipIndex, new Dictionary<int, List<bool>>());
            dic3 = dic2[chipIndex];

            if (!dic3.ContainsKey(chipNumber))
                dic3.Add(chipNumber, new List<bool>());
            lst = dic3[chipNumber];

            while (partNumber > lst.Count)
            {
                lst.Add(false);
            }

            return lst[partNumber - 1];
        }

        public static List<bool> GetChipMutes(string chipName, int chipIndex, int chipNumber, Document doc = null)
        {
            if (doc == null) doc = crntDoc;
            if (doc == null) return null;

            Dictionary<string, Dictionary<int, Dictionary<int, List<bool>>>> dic1;
            Dictionary<int, Dictionary<int, List<bool>>> dic2;
            Dictionary<int, List<bool>> dic3;
            List<bool> lst;

            if (!chipMuteStatus.ContainsKey(doc))
                chipMuteStatus.Add(doc, new Dictionary<string, Dictionary<int, Dictionary<int, List<bool>>>>());
            dic1 = chipMuteStatus[doc];

            if (!dic1.ContainsKey(chipName))
                dic1.Add(chipName, new Dictionary<int, Dictionary<int, List<bool>>>());
            dic2 = dic1[chipName];

            if (!dic2.ContainsKey(chipIndex))
                dic2.Add(chipIndex, new Dictionary<int, List<bool>>());
            dic3 = dic2[chipIndex];

            if (!dic3.ContainsKey(chipNumber))
                dic3.Add(chipNumber, new List<bool>());
            lst = dic3[chipNumber];

            return lst;
        }


        public static void AllClear()
        {
            lstMuteStatus.Clear();
            keyNum.Clear();
            SoloMode.Clear();
        }

        /// <summary>
        /// ドキュメントのミュート情報を消去する
        /// </summary>
        public static void Clear(Document doc)
        {
            if (lstMuteStatus.ContainsKey(doc))
            {
                lstMuteStatus.Remove(doc);
                keyNum.Remove(doc);
                SoloMode.Remove(doc);
            }
        }

        public static void Add(Document doc)
        {
            if (!lstMuteStatus.ContainsKey(doc))
            {
                lstMuteStatus.Add(doc, null);
            }
        }

        public static int UpdateTrackInfo(Document doc, object[] cells, int key = -1)
        {
            crntDoc = doc;

            Add(doc);

            //if (trackNum < 0) return -1;
            Dictionary<int, muteStatus> dms = lstMuteStatus[doc];
            if (key == -1) key = GetKey(doc, cells);

            //自動で要素追加
            if (dms == null || !dms.ContainsKey(key))
            {
                if (dms == null)
                    dms = new Dictionary<int, muteStatus>();

                dms.Add(key, new muteStatus());
                lstMuteStatus[doc] = dms;
            }

            muteStatus ms = dms[key];
            ms.partNumber = (int)cells[0];
            ms.chipIndex = (int)cells[1];
            ms.chipNumber = (int)cells[2];
            ms.trackName = (string)cells[3];
            ms.chipName = (string)cells[4];

            //SetChipMute(doc, ms.chipName, ms.chipIndex, ms.chipNumber, ms.partNumber, false);

            return key;
        }


        public static void ClickAllSolo(Document doc = null)
        {
            if (doc == null) doc = crntDoc;
            if (doc == null) return;
            if (!lstMuteStatus.ContainsKey(doc)) return;

            if (!SoloMode[doc]) return;
            //Click SOLO Reset(mute復帰)) 
            foreach (muteStatus ms in lstMuteStatus[doc].Values)
            {
                ms.solo = false;
                ms.mute = ms.cache;
                SetChipMute(doc, ms.chipName, ms.chipIndex, ms.chipNumber, ms.partNumber, ms.mute);
            }
            SoloMode[doc] = false;
        }

        public static void ClickAllMute(Document doc = null)
        {
            if (doc == null) doc = crntDoc;
            if (doc == null) return;
            if (!lstMuteStatus.ContainsKey(doc)) return;

            if (SoloMode[doc]) return;
            //Click MUTE Reset 
            foreach (muteStatus ms in lstMuteStatus[doc].Values)
            {
                ms.mute = false;
                SetChipMute(doc, ms.chipName, ms.chipIndex, ms.chipNumber, ms.partNumber, ms.mute);
            }

        }


        public static void ClickMute(int partKey, Document doc = null)
        {
            if (doc == null) doc = crntDoc;
            if (doc == null) return;
            if (!lstMuteStatus.ContainsKey(doc)) return;
            if (!lstMuteStatus[doc].ContainsKey(partKey)) return;

            muteStatus mts = lstMuteStatus[doc][partKey];
            bool nowMute = mts.mute;
            mts.mute = !nowMute;
            if (SoloMode[doc]) mts.solo = nowMute;

            SetChipMute(doc, mts.chipName, mts.chipIndex, mts.chipNumber, mts.partNumber, mts.mute);

            if (SoloMode[doc] && !nowMute && !CheckSoloCh(doc))
            {
                SoloMode[doc] = false;

                //mute復帰
                foreach (muteStatus ms in lstMuteStatus[doc].Values)
                {
                    ms.mute = ms.cache;
                    SetChipMute(doc, ms.chipName, ms.chipIndex, ms.chipNumber, ms.partNumber, ms.mute);
                }
            }

            if (doc.srcFileFormat != EnmMmlFileFormat.MUC) return;
            string tn = mts.trackName;
            if (tn.Length < 2) return;
            char t = tn[0];
            foreach (muteStatus ms in lstMuteStatus[doc].Values)
            {
                if (ms.trackName[0] != t) continue;

                ms.mute = mts.mute;
                ms.solo = mts.solo;
                ms.cache = mts.cache;
                SetChipMute(doc, ms.chipName, ms.chipIndex, ms.chipNumber, ms.partNumber, ms.mute);
            }
        }

        public static void ClickMute(int partNumber,int chipIndex,int chipNumber,string trackName,string chipName,Document doc = null)
        {
            if (doc == null) doc = crntDoc;
            if (doc == null) return;

            int key = -1;
            if (dicKey.ContainsKey(doc))
                if (dicKey[doc].ContainsKey(partNumber))
                    if (dicKey[doc][partNumber].ContainsKey(chipIndex))
                        if (dicKey[doc][partNumber][chipIndex].ContainsKey(chipNumber))
                            if (dicKey[doc][partNumber][chipIndex][chipNumber].ContainsKey(trackName))
                                if (dicKey[doc][partNumber][chipIndex][chipNumber][trackName].ContainsKey(chipName))
                                    key = dicKey[doc][partNumber][chipIndex][chipNumber][trackName][chipName];

            if (key == -1) return;

            ClickMute(key, doc);
        }

        public static void ClickSolo(int partKey, Document doc = null)
        {
            if (doc == null) doc = crntDoc;
            if (doc == null) return;
            if (!lstMuteStatus.ContainsKey(doc)) return;
            if (!lstMuteStatus[doc].ContainsKey(partKey)) return;

            muteStatus mts = lstMuteStatus[doc][partKey];
            bool nowSolo = mts.solo;
            //SOLOモードではなく、SOLOではない場合はチェックを行う
            if (!SoloMode[doc] && !nowSolo && !CheckSoloCh(doc))
            {
                SoloMode[doc] = true;
                //mute退避
                foreach (muteStatus ms in lstMuteStatus[doc].Values)
                {
                    ms.cache = ms.mute;
                    ms.mute = true;
                    SetChipMute(doc, ms.chipName, ms.chipIndex, ms.chipNumber, ms.partNumber, ms.mute);
                    if(ms.partNumber==13 && doc.srcFileFormat == EnmMmlFileFormat.MUC)
                    {
                        SetChipMute(doc, ms.chipName, ms.chipIndex, ms.chipNumber, 13, ms.mute);//b
                        SetChipMute(doc, ms.chipName, ms.chipIndex, ms.chipNumber, 14, ms.mute);//s
                        SetChipMute(doc, ms.chipName, ms.chipIndex, ms.chipNumber, 15, ms.mute);//c
                        SetChipMute(doc, ms.chipName, ms.chipIndex, ms.chipNumber, 16, ms.mute);//h
                        SetChipMute(doc, ms.chipName, ms.chipIndex, ms.chipNumber, 17, ms.mute);//t
                        SetChipMute(doc, ms.chipName, ms.chipIndex, ms.chipNumber, 18, ms.mute);//r
                    }
                }
            }

            mts.solo = !nowSolo;
            if (SoloMode[doc]) mts.mute = nowSolo;
            SetChipMute(doc, mts.chipName, mts.chipIndex, mts.chipNumber, mts.partNumber, mts.mute);

            if (SoloMode[doc] && nowSolo && !CheckSoloCh(doc))
            {
                SoloMode[doc] = false;
                //mute復帰
                foreach (muteStatus ms in lstMuteStatus[doc].Values)
                {
                    ms.mute = ms.cache;
                    SetChipMute(doc, ms.chipName, ms.chipIndex, ms.chipNumber, ms.partNumber, ms.mute);
                }
            }

            if (doc.srcFileFormat != EnmMmlFileFormat.MUC) return;
            string tn = mts.trackName;
            if (tn.Length < 2) return;
            char t = tn[0];
            foreach (muteStatus ms in lstMuteStatus[doc].Values)
            {
                if (ms.trackName[0] != t) continue;

                ms.mute = mts.mute;
                ms.solo = mts.solo;
                ms.cache = mts.cache;
                SetChipMute(doc, ms.chipName, ms.chipIndex, ms.chipNumber, ms.partNumber, ms.mute);
                if (mts.partNumber >= 13 && mts.partNumber<=18)
                {
                    SetChipMute(doc, ms.chipName, ms.chipIndex, ms.chipNumber, 13, ms.mute);//b
                    SetChipMute(doc, ms.chipName, ms.chipIndex, ms.chipNumber, 14, ms.mute);//s
                    SetChipMute(doc, ms.chipName, ms.chipIndex, ms.chipNumber, 15, ms.mute);//c
                    SetChipMute(doc, ms.chipName, ms.chipIndex, ms.chipNumber, 16, ms.mute);//h
                    SetChipMute(doc, ms.chipName, ms.chipIndex, ms.chipNumber, 17, ms.mute);//t
                    SetChipMute(doc, ms.chipName, ms.chipIndex, ms.chipNumber, 18, ms.mute);//r
                }
            }
        }

        public static void ClickSolo(int partNumber, int chipIndex, int chipNumber, string trackName, string chipName, Document doc = null)
        {
            if (doc == null) doc = crntDoc;
            if (doc == null) return;

            int key = -1;
            if (dicKey.ContainsKey(doc))
                if (dicKey[doc].ContainsKey(partNumber))
                    if (dicKey[doc][partNumber].ContainsKey(chipIndex))
                        if (dicKey[doc][partNumber][chipIndex].ContainsKey(chipNumber))
                            if (dicKey[doc][partNumber][chipIndex][chipNumber].ContainsKey(trackName))
                                if (dicKey[doc][partNumber][chipIndex][chipNumber][trackName].ContainsKey(chipName))
                                    key = dicKey[doc][partNumber][chipIndex][chipNumber][trackName][chipName];

            if (key == -1) return;

            ClickSolo(key, doc);
        }


        public static muteStatus GetStatus(int partKey, Document doc = null)
        {
            if (doc == null) doc = crntDoc;
            if (doc == null) return null;
            if (!lstMuteStatus.ContainsKey(doc)) return null;
            if (!lstMuteStatus[doc].ContainsKey(partKey)) return null;
            return lstMuteStatus[doc][partKey];
        }

        public static bool? GetMuteStatus(int partKey, Document doc = null)
        {
            if (doc == null) doc = crntDoc;
            if (doc == null) return null;
            if (!lstMuteStatus.ContainsKey(doc)) return null;
            if (!lstMuteStatus[doc].ContainsKey(partKey)) return null;

            return lstMuteStatus[doc][partKey].mute;
        }

        public static bool? GetMuteStatus(int partNumber, int chipIndex, int chipNumber, string trackName, string chipName, Document doc = null)
        {
            if (doc == null) doc = crntDoc;
            if (doc == null) return null;
            if (!lstMuteStatus.ContainsKey(doc)) return null;

            int key = -1;
            if (dicKey.ContainsKey(doc))
                if (dicKey[doc].ContainsKey(partNumber))
                    if (dicKey[doc][partNumber].ContainsKey(chipIndex))
                        if (dicKey[doc][partNumber][chipIndex].ContainsKey(chipNumber))
                            if (dicKey[doc][partNumber][chipIndex][chipNumber].ContainsKey(trackName))
                                if (dicKey[doc][partNumber][chipIndex][chipNumber][trackName].ContainsKey(chipName))
                                    key = dicKey[doc][partNumber][chipIndex][chipNumber][trackName][chipName];

            if (key < 0)
                key = UpdateTrackInfo(doc, new object[] { partNumber, chipIndex, chipNumber, trackName, chipName });

            return lstMuteStatus[doc][key].mute;
        }

        public static bool? GetSoloStatus(int partKey, Document doc = null)
        {
            if (doc == null) doc = crntDoc;
            if (doc == null) return null;
            if (!lstMuteStatus.ContainsKey(doc)) return null;
            if (!lstMuteStatus[doc].ContainsKey(partKey)) return null;

            return lstMuteStatus[doc][partKey].solo;
        }

        public static bool? GetSoloStatus(int partNumber, int chipIndex, int chipNumber, string trackName, string chipName, Document doc = null)
        {
            if (doc == null) doc = crntDoc;
            if (doc == null) return null;
            if (!lstMuteStatus.ContainsKey(doc)) return null;

            int key = -1;
            if (dicKey.ContainsKey(doc))
                if (dicKey[doc].ContainsKey(partNumber))
                    if (dicKey[doc][partNumber].ContainsKey(chipIndex))
                        if (dicKey[doc][partNumber][chipIndex].ContainsKey(chipNumber))
                            if (dicKey[doc][partNumber][chipIndex][chipNumber].ContainsKey(trackName))
                                if (dicKey[doc][partNumber][chipIndex][chipNumber][trackName].ContainsKey(chipName))
                                    key = dicKey[doc][partNumber][chipIndex][chipNumber][trackName][chipName];

            if (key < 0)
                key = UpdateTrackInfo(doc, new object[] { partNumber, chipIndex, chipNumber, trackName, chipName });

            return lstMuteStatus[doc][key].solo;
        }


        private static bool CheckSoloCh(Document doc)
        {
            foreach (muteStatus ms in lstMuteStatus[doc].Values)
            {
                if (!ms.solo) continue;
                return true;
            }

            return false;
        }

        private static int GetKey(Document doc, object[] cells)
        {
            Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<string, Dictionary<string, int>>>>> dic1;
            Dictionary<int, Dictionary<int, Dictionary<string, Dictionary<string, int>>>> dic2;
            Dictionary<int, Dictionary<string, Dictionary<string, int>>> dic3;
            Dictionary<string, Dictionary<string, int>> dic4;
            Dictionary<string, int> dic5;

            int pN = (int)cells[0];
            int cIN = (int)cells[1];
            int cN = (int)cells[2];
            string tN = (string)cells[3];
            string cNm = (string)cells[4];

            if (!dicKey.ContainsKey(doc))
            {
                dicKey.Add(doc, new Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<string, Dictionary<string, int>>>>>());
            }
            dic1 = dicKey[doc];

            if (!dic1.ContainsKey(pN))
            {
                dic1.Add(pN, new Dictionary<int, Dictionary<int, Dictionary<string, Dictionary<string, int>>>>());
            }
            dic2 = dic1[pN];

            if (!dic2.ContainsKey(cIN))
            {
                dic2.Add(cIN, new Dictionary<int, Dictionary<string, Dictionary<string, int>>>());
            }
            dic3 = dic2[cIN];

            if (!dic3.ContainsKey(cN))
            {
                dic3.Add(cN, new Dictionary<string, Dictionary<string, int>>());
            }
            dic4 = dic3[cN];

            if (!dic4.ContainsKey(tN))
            {
                dic4.Add(tN, new Dictionary<string, int>());
            }
            dic5 = dic4[tN];

            if (!dic5.ContainsKey(cNm))
            {
                if (!keyNum.ContainsKey(doc)) keyNum.Add(doc, 0);
                int num = keyNum[doc]++;
                dic5.Add(cNm, num);
                if (!SoloMode.ContainsKey(doc)) SoloMode.Add(doc, false);
            }

            return dic5[cNm];
        }

        public static void SetMuteSolo(int partKey,muteStatus ms, Document doc = null)
        {
            if (doc == null) doc = crntDoc;
            if (doc == null) return;
            if (!lstMuteStatus.ContainsKey(doc)) return;
            if (!lstMuteStatus[doc].ContainsKey(partKey)) return;

            muteStatus mts = lstMuteStatus[doc][partKey];
            mts.mute = ms.mute;
            mts.solo = ms.solo;
            mts.cache = ms.cache;

            SetChipMute(doc, mts.chipName, mts.chipIndex, mts.chipNumber, mts.partNumber, mts.mute);
        }
    }
}