using Corex64;
using mml2vgmIDEx64;
using System.Collections.Generic;

namespace SoundManager
{
    public class Chip
    {
        public bool Use;
        public long Delay;
        public EnmVRModel Model;
        public EnmZGMDevice Device;
        public int Index;
        public int Number;
        public int Hosei;

        private object lockobj = new object();
        private List<bool> _ChMasks = null;
        public List<bool> ChMasks
        {
            set
            {
                lock (lockobj) { _ChMasks = value; }
            }
            get
            {
                lock (lockobj) { return _ChMasks; }
            }
        }

        private int[] _ChMasksPG = null;
        public int[] ChMasksPG
        {
            set
            {
                lock (lockobj) { _ChMasksPG = value; }
            }
            get
            {
                lock (lockobj) { return _ChMasksPG; }
            }
        }

        public int currentCh { get; internal set; }

        public bool[] silentVoice { get; set; } = null;
        public int[] silentVoicePG { get; set; } = null;


        public Chip(int Ch)
        {
            ChMasks = new List<bool>(Ch);
            for (int i = 0; i < Ch; i++) ChMasks.Add(false);
            ChMasksPG = new int[Ch];
            silentVoice = new bool[Ch];
            silentVoicePG = new int[Ch];
        }

        public Chip(int Ch, string chipName, int chipIndex, int chipNumber)
        {
            List<bool> mutes = muteManager.GetChipMutes(chipName, chipIndex, chipNumber);
            ChMasks = mutes;
            if (mutes == null || mutes.Count<1)
            {
                ChMasks = new List<bool>(Ch);
                for (int i = 0; i < Ch; i++) ChMasks.Add(false);
            }
            ChMasksPG = new int[Ch];
            silentVoice = new bool[Ch];
            silentVoicePG = new int[Ch];
        }


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
            this.ChMasks = chip.ChMasks;
            this.silentVoice = chip.silentVoice;
        }
    }
}
