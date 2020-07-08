using System.Collections.Generic;

namespace mml2vgmIDE
{
    public class SienItem
    {
        public int sienType { get; set; }
        public int nextAnchor { get; set; }
        public int nextCaret { get; set; }
        public string pattern { get; set; }
        public string patternType { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string content { get; set; }
        public int foundCnt { get; set; }
    }

    public class Sien
    {
        public List<SienItem> list { get; set; }
    }
}
