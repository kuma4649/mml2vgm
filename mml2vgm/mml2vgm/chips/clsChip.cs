using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mml2vgm
{
    abstract public class clsChip
    {
        public string Name
        {
            get
            {
                return _Name;
            }
        }
        protected string _Name="";

        public string ShortName
        {
            get
            {
                return _ShortName;
            }
        }
        protected string _ShortName = "";

        public int ChMax
        {
            get
            {
                return _ChMax;
            }
        }
        protected int _ChMax = 0;

        public int ChipID
        {
            get
            {
                return _ChipID;
            }
        }
        protected int _ChipID = -1;

        public clsChannel[] Ch;
        public int Frequency;
        public bool use;
        public List<partWork> lstPartWork;


        public clsChip(int chipID, string initialPartName)
        {
            this._ChipID = chipID;
        }

        public bool ChannelNameContains(string name)
        {
            foreach (clsChannel c in Ch)
            {
                if (c.Name == name) return true;
            }
            return false;
        }

        protected void setPartToCh(clsChannel[] Ch, string val)
        {
            if (val == null || (val.Length != 1 && val.Length != 2)) return;

            string f = val[0].ToString();
            string r = (val.Length == 2) ? val[1].ToString() : " ";

            for (int i = 0; i < Ch.Length; i++)
            {
                if (Ch[i] == null) Ch[i] = new clsChannel();
                Ch[i].Name = string.Format("{0}{1}{2:00}", f, r, i + 1);
            }

            //checkDuplication(fCh);
        }


    }

    public class clsChannel
    {
        public string Name;
        public enmChannelType Type;
        public bool isSecondary;
    }

    public enum enmChannelType
    {
        Multi,
        FMOPN,
        FMOPNex,
        FMOPM,
        DCSG,
        PCM,
        ADPCM,
        RHYTHM,
        FMPCM,
        DCSGNOISE
    }

    public enum enmChipType : int
    {
        None = -1,
        YM2151 = 0,
        YM2203 = 1,
        YM2608 = 2,
        YM2610B = 3,
        YM2612 = 4,
        SN76489 = 5,
        RF5C164 = 6
    }

}
