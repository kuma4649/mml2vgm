using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
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
        protected string _Name = "";

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

        public bool CanUsePcm
        {
            get
            {
                return _canUsePcm;
            }

            set
            {
                _canUsePcm = value;
            }
        }
        protected bool _canUsePcm = false;

        protected int _ChipID = -1;

        public clsChannel[] Ch;
        public int Frequency = 7670454;
        public bool use;
        public List<partWork> lstPartWork;

        public double[] noteTbl = new double[] {
            //   c       c+        d       d+        e        f       f+        g       g+        a       a+        b
            261.62 , 277.18 , 293.66 , 311.12 , 329.62 , 349.22 , 369.99 , 391.99 , 415.30 , 440.00 , 466.16 , 493.88
        };

        public int[][] FNumTbl;

        //= new int[1][] {
        //    new int[13] {
        //        //   c    c+     d    d+     e     f    f+     g    g+     a    a+     b    >c
        //         0x289,0x2af,0x2d8,0x303,0x331,0x362,0x395,0x3cc,0x405,0x443,0x484,0x4c8,0x289*2
        //    }
        //};


        public clsChip(int chipID, string initialPartName)
        {
            this._ChipID = chipID;
            makeFNumTbl();
        }

        protected Dictionary<string, List<double>> makeFNumTbl()
        {
            //for (int i = 0; i < noteTbl.Length; i++)
            //{
            //    FNumTbl[0][i] = (int)(Math.Round(((144.0 * noteTbl[i] * Math.Pow(2.0, 20) / Frequency) / Math.Pow(2.0, (4 - 1))), MidpointRounding.AwayFromZero));
            //}
            //FNumTbl[0][12] = FNumTbl[0][0] * 2;

            string fn = string.Format("FNUM_{0}.txt",Name);
            Stream stream = null;
            Dictionary<string, List<double>> dic = new Dictionary<string, List<double>>();

            if (File.Exists(Path.Combine(System.Windows.Forms.Application.StartupPath, fn)))
            {
                stream = new FileStream(fn, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            else
            {
                System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
                string[] resources = asm.GetManifestResourceNames();
                foreach (string resource in resources)
                {
                    if (resource.IndexOf(fn) >= 0)
                    {
                        fn = resource;
                    }
                }
                stream = asm.GetManifestResourceStream(fn);
            }

            try
            {
                if (stream != null)
                {
                    using (System.IO.StreamReader sr = new System.IO.StreamReader(stream, Encoding.Unicode))
                    {
                        while (!sr.EndOfStream)
                        {
                            //内容を読み込む
                            string[] s = sr.ReadLine().Split(new string[] { "=" }, StringSplitOptions.None);
                            if (s == null || s.Length != 2) continue;
                            if (s[0].Trim() == "" || s[0].Trim().Length < 1 || s[0].Trim()[0] == '\'') continue;
                            string[] val = s[1].Split(new string[] { "," }, StringSplitOptions.None);
                            s[0] = s[0].ToUpper().Trim();

                            if (!dic.ContainsKey(s[0]))
                            {
                                List<double> value = new List<double>();
                                dic.Add(s[0], value);
                            }

                            foreach (string v in val)
                            {
                                string vv = v.Trim();

                                if (vv[0] == '$' && vv.Length > 1)
                                {
                                    int num16 = Convert.ToInt32(vv.Substring(1), 16);
                                    dic[s[0]].Add(num16);
                                }
                                else
                                {
                                    double o;
                                    if (double.TryParse(vv, out o))
                                    {
                                        dic[s[0]].Add(o);
                                    }
                                }
                            }

                        }
                    }
                }
            }
            catch
            {
                if(stream!=null) stream.Dispose();
                dic = null;
            }

            return dic;
        }

        public bool ChannelNameContains(string name)
        {
            foreach (clsChannel c in Ch)
            {
                if (c.Name == name) return true;
            }
            return false;
        }

        public void setPartToCh(clsChannel[] Ch, string val)
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
        public int MaxVolume;
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
        DCSGNOISE,
        SSG,
        ADPCMA,
        ADPCMB,
        WaveForm,
        FMPCMex
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
        RF5C164 = 6,
        SEGAPCM = 7,
        HuC6280 = 8,
        YM2612X=9
    }

}
