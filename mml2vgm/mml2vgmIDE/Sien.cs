using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    }

    public class Sien
    {
        public List<SienItem> list { get; set; }
    }
}
