using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class LinePos
    {
        public string fullPath = "";
        public int row = -1;
        public int col = -1;
        public int length = 1;
        public string part = "";
        public string chip = "";
        public int chipIndex = 0;
        public int chipNumber = 0;
        public int ch = -1;

        public string path
        {
            get
            {
                return System.IO.Path.GetDirectoryName(fullPath);
            }
        }

        public string filename
        {
            get
            {
                return System.IO.Path.GetFileName(fullPath);
            }
        }

        public LinePos(string fullPath, int row = -1, int col = -1, int length = -1, string part = "", string chip = "", int chipIndex = 0, int chipNumber = 0, int ch = -1)
        {
            this.fullPath = fullPath;
            this.row = row;
            this.col = col;
            this.length = length;
            this.part = part;
            this.chip = chip;
            this.chipIndex = chipIndex;
            this.chipNumber = chipNumber;
            this.ch = ch;
        }
    }
}