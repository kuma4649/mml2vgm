using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mml2vgmIDE
{
    public class muteManager
    {
        private Dictionary<Document, muteStatus[]> lstMuteStatus = new Dictionary<Document, muteStatus[]>();

        public void Clear()
        {
            lstMuteStatus.Clear();
        }

        public void Add(Document doc)
        {
            if (lstMuteStatus.ContainsKey(doc)) return;
            lstMuteStatus.Add(doc, null);
        }

        public void UpdateTrackInfo(Document doc,int trackNum, object[] cells)
        {
            Add(doc);

            if (trackNum < 0) return;
            muteStatus[] ms = lstMuteStatus[doc];

            //自動で要素追加
            if (ms == null || trackNum > ms.Length - 1)
            {
                if (ms == null)
                    ms = new muteStatus[0];

                List<muteStatus> lstMs = ms.ToList();
                while (lstMs.Count - 1 < trackNum)
                {
                    lstMs.Add(new muteStatus());
                }
                ms = lstMs.ToArray();
                lstMuteStatus[doc] = ms;
            }

            ms[trackNum].partNumber = (int)cells[0];
            ms[trackNum].chipIndex = (int)cells[1];
            ms[trackNum].chipNumber = (int)cells[2];
            ms[trackNum].trackName = (string)cells[3];
            ms[trackNum].chipName = (string)cells[4];
        }

    }
}
