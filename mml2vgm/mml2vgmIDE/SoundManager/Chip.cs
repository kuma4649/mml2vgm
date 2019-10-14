using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using mml2vgmIDE;

namespace SoundManager
{
    public class Chip
    {
        public bool Use;
        public long Delay;
        public EnmModel Model;
        public EnmDevice Device;
        public int Index;
        public int Number;
        public int Hosei;

        public void Move(Chip chip)
        {
            if (chip == null) return;

            this.Use = chip.Use;
            this.Delay = chip.Delay;
            this.Model = chip.Model;
            this.Device = chip.Device;
            this.Index = chip.Index;
            this.Number = chip.Number;
            this.Hosei = chip.Hosei;
        }
    }
}
