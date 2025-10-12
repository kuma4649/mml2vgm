using System.Collections.Generic;

namespace mml2vgmIDEx64
{
    public class SienItem
    {

        public int ID { get; set; }
        public string tree { get; set; }
        public int parentID { get; set; }
        public bool haveChild { get; set; }
        public string supportMMLFormat { get; set; }
        public int sienType { get; set; }
        public int nextAnchor { get; set; }
        public int nextCaret { get; set; }
        public string pattern { get; set; }
        public string patternType { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string content { get; set; }
        public string content2 { get; set; }
        public string content3 { get; set; }
        public int foundCnt { get; set; }
    }

    public class Sien
    {
        public List<SienItem> list { get; set; }
    }
}
