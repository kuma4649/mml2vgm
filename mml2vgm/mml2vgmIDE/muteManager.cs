using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mml2vgmIDE
{
    public class muteManager
    {
        private Dictionary<Document, Dictionary<int,muteStatus>> lstMuteStatus = new Dictionary<Document, Dictionary<int, muteStatus>>();
        private Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<string, Dictionary<string, int>>>>> dicKey
            = new Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<string, Dictionary<string, int>>>>>();
        private int keyNum = 0;

        public void Clear()
        {
            lstMuteStatus.Clear();
        }

        public void Clear(Document doc)
        {
            if (lstMuteStatus.ContainsKey(doc))
                lstMuteStatus[doc] = null;
        }

        public void Add(Document doc)
        {
            if (!lstMuteStatus.ContainsKey(doc))
            {
                lstMuteStatus.Add(doc, null);
            }
        }

        public int UpdateTrackInfo(Document doc, int trackNum, object[] cells, int key = -1)
        {
            Add(doc);

            if (trackNum < 0) return -1;
            Dictionary<int, muteStatus> dms = lstMuteStatus[doc];
            if (key == -1) key = GetKey(cells);

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

            return key;
        }

        private int GetKey(object[] cells)
        {
            Dictionary<int, Dictionary<int, Dictionary<string, Dictionary<string, int>>>> dic1;
            Dictionary<int, Dictionary<string, Dictionary<string, int>>> dic2;
            Dictionary<string, Dictionary<string, int>> dic3;
            Dictionary<string, int> dic4;

            int pN = (int)cells[0];
            int cIN = (int)cells[1];
            int cN = (int)cells[2];
            string tN = (string)cells[3];
            string cNm = (string)cells[4];

            if (!dicKey.ContainsKey(pN))
            {
                dicKey.Add(pN, new Dictionary<int, Dictionary<int, Dictionary<string, Dictionary<string, int>>>>());
            }
            dic1 = dicKey[pN];

            if (!dic1.ContainsKey(cIN))
            {
                dic1.Add(cIN, new Dictionary<int, Dictionary<string, Dictionary<string, int>>>());
            }
            dic2 = dic1[cIN];

            if (!dic2.ContainsKey(cN))
            {
                dic2.Add(cN, new Dictionary<string, Dictionary<string, int>>());
            }
            dic3 = dic2[cN];

            if (!dic3.ContainsKey(tN))
            {
                dic3.Add(tN, new Dictionary<string, int>());
            }
            dic4 = dic3[tN];

            if (!dic4.ContainsKey(cNm))
            {
                dic4.Add(cNm, keyNum++);
            }
            
            return dic4[cNm];
        }
    }
}
