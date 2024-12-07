using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace mml2vgmIDEx64
{
    public class InstrumentAtLocal
    {
        private string add = "";

        public bool isFirst = false;
        private FrmMain parent = null;
        private object sender = null;
        private string baseDir = "";

        public TreeNode treenode { get; internal set; }

        public enum enmInstType
        {
            Dir,
            FM_N,//OPN
            FM_L,//OPL
            FM_M,//OPM
            FM_NA,//OPNA2(FM)
            SSG_WF,//OPNA2(SSG)
        }

        public void Start(FrmMain parent, object sender, string baseDir,string add, Action<object,TreeNode , List<Tuple<enmInstType, string, string, string>>> CompleteMethod)
        {
            this.parent = parent;
            this.sender = sender;
            this.baseDir = baseDir;
            this.add = add;

            //mmlファイルの一覧を得る
            List<FileSystemInfo> lstFile = new List<FileSystemInfo>();
            GetFileList(baseDir,ref lstFile);
            //音色データを解析する
            List<Tuple<enmInstType, string, string, string>> lstInst = new List<Tuple<enmInstType, string, string, string>>();
            foreach (FileSystemInfo fi in lstFile)
            {
                if (fi.Attributes == FileAttributes.Directory)
                {
                    Tuple<enmInstType, string, string, string> ins = new Tuple<enmInstType, string, string, string>(enmInstType.Dir, fi.FullName, "", "");
                    lstInst.Add(ins);
                }
                else
                {
                    List<Tuple<enmInstType, string, string, string>> ins = GetInsts(fi.FullName);
                    if (ins != null)
                    {
                        lstInst.AddRange(ins);
                    }
                }
            }
            CompleteMethod?.Invoke(this.sender,treenode, lstInst);
        }

        private void GetFileList(string baseDir, ref List<FileSystemInfo> lstFile)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(baseDir);
                
                IEnumerable<FileSystemInfo> files = di.EnumerateFileSystemInfos();
                foreach (FileSystemInfo f in files)
                {
                    if (f.Attributes == FileAttributes.Directory)
                    {
                        lstFile.Add(f);
                        GetFileList(f.FullName, ref lstFile);
                    }
                    else
                    {
                        string ext = Path.GetExtension(f.FullName).ToLower();
                        if (ext == ".gwi" 
                            || ext == ".muc" 
                            || ext == ".mml"
                            || ext == ".rym2612"
                            || ext == ".btb"
                            )
                        {
                            lstFile.Add(f);
                        }
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("アクセスが拒否されました。", "権限不足", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch { }

        }

        private List<Tuple<enmInstType, string, string, string>> GetInsts(string file)
        {
            List<Tuple<enmInstType, string, string, string>> lstInst =
                    new List<Tuple<enmInstType, string, string, string>>();
            string ext = Path.GetExtension(file).ToLower();
            switch (ext)
            {
                case ".gwi":
                    lstInst = GetInstsAtGwi(file);
                    break;
                case ".muc":
                    lstInst = GetInstsAtMuc(file);
                    break;
                case ".rym2612":
                    lstInst = GetInstsAtRym2612(file);
                    break;
                case ".btb":
                    lstInst = GetInstsAtBambooTracker(file);
                    break;
            }

            return lstInst;
        }

        private List<Tuple<enmInstType, string, string, string>> GetInstsAtGwi(string srcFn)
        {
            List<Tuple<enmInstType, string, string, string>> ret = new List<Tuple<enmInstType, string, string, string>>();
            try
            {
                string stPath = System.Windows.Forms.Application.StartupPath;
                Corex64.Mml2vgm mv = new Corex64.Mml2vgm(null, srcFn, "", stPath, Disp);
                mv.isIDE = true;
                if (mv.Start_Analyze() != 0) return null;

                int renban = 0;
                //inst FM
                Dictionary<int, Tuple<string, byte[]>> instFM = mv.desVGM.instFM;
                foreach (int num in instFM.Keys)
                {
                    if (instFM[num].Item2.Length == 47)
                    {
                        string name = instFM[num].Item1;
                        if (string.IsNullOrEmpty(name))
                        {
                            name = string.Format("OPN instrument noname {0}", renban++);
                        }

                        int[][] prms = new int[][] { new int[11], new int[11], new int[11], new int[11] };
                        int alg = 0, fb = 0;
                        for (int i = 0; i < 4; i++)
                        {
                            for (int j = 0; j < 11; j++)
                            {
                                prms[i][j] = (byte)instFM[num].Item2[i * 11 + j + 1];
                            }
                        }
                        alg = (int)instFM[num].Item2[45];
                        fb = (int)instFM[num].Item2[46];
                        string sdatV, sdatM;
                        sdatV = MakeVoiceDefineForGWI_OPN(name + add, num, prms, alg, fb);
                        sdatM = MakeVoiceDefineForMUC_OPN(name + add, num, prms, alg, fb);
                        ret.Add(new Tuple<enmInstType, string, string, string>(enmInstType.FM_N, Path.Combine(srcFn, name).Replace(baseDir, ""), sdatV, sdatM));
                    }
                    else
                    {
                        ;
                    }
                }

                //inst OPM
                Dictionary<int, Tuple<string, byte[]>> instOPM = mv.desVGM.instOPM;
                foreach (int num in instOPM.Keys)
                {
                    if (instOPM[num].Item2.Length == 47)
                    {
                        string name = instOPM[num].Item1;
                        if (string.IsNullOrEmpty(name))
                        {
                            name = string.Format("OPM instrument noname {0}", renban++);
                        }
                        string str = GetInstrumentGwiFmOPMString(num, name, instOPM[num].Item2);
                        if (!string.IsNullOrEmpty(str))
                        {
                            ret.Add(new Tuple<enmInstType, string, string, string>(enmInstType.FM_N, Path.Combine(srcFn, name).Replace(baseDir, ""), str,""));
                        }
                    }
                    else
                    {
                        ;
                    }
                }

                //inst OPL
                Dictionary<int, Tuple<string, byte[]>> instOPL = mv.desVGM.instOPL;
                foreach (int num in instOPL.Keys)
                {
                    if (instOPL[num].Item2.Length == Corex64.Const.OPLL_INSTRUMENT_SIZE)
                    {
                        string name = instOPL[num].Item1;
                        if (string.IsNullOrEmpty(name))
                        {
                            name = string.Format("OPLL instrument noname {0}", renban++);
                        }
                        string str = GetInstrumentGwiFmOPLLString(num, name, instOPL[num].Item2);
                        if (!string.IsNullOrEmpty(str))
                        {
                            ret.Add(new Tuple<enmInstType, string, string, string>(enmInstType.FM_L, Path.Combine(srcFn, name).Replace(baseDir, ""), str, ""));
                        }
                    }
                    else if (instOPL[num].Item2.Length == Corex64.Const.OPL3_INSTRUMENT_SIZE)
                    {
                        string name = instOPL[num].Item1;
                        if (string.IsNullOrEmpty(name))
                        {
                            name = string.Format("OPL instrument noname {0}", renban++);
                        }
                        string str = GetInstrumentGwiFmOPLString(num, name, instOPL[num].Item2);
                        if (!string.IsNullOrEmpty(str))
                        {
                            ret.Add(new Tuple<enmInstType, string, string, string>(enmInstType.FM_L, Path.Combine(srcFn, name).Replace(baseDir, ""), str,""));
                        }
                    }
                    else if (instOPL[num].Item2.Length == Corex64.Const.OPL_OP4_INSTRUMENT_SIZE)
                    {
                        string name = instOPL[num].Item1;
                        if (string.IsNullOrEmpty(name))
                        {
                            name = string.Format("OPL4 4OP instrument noname {0}", renban++);
                        }
                        string str = GetInstrumentGwiFmOPL4String(num, name, instOPL[num].Item2);
                        if (!string.IsNullOrEmpty(str))
                        {
                            ret.Add(new Tuple<enmInstType, string, string, string>(enmInstType.FM_L, Path.Combine(srcFn, name).Replace(baseDir, ""), str,""));
                        }
                    }
                    else
                    {
                        ;
                    }
                }

                //inst WF
                Dictionary<int, Tuple<string, byte[]>> instWF = mv.desVGM.instWF;
                foreach (int num in instWF.Keys)
                {
                    if (instWF[num].Item2.Length == 33)
                    {
                        string name = instWF[num].Item1;
                        if (string.IsNullOrEmpty(name))
                        {
                            name = string.Format("HuC instrument noname {0}", renban++);
                        }
                        string str = GetInstrumentGwiWFString(num, name, instWF[num].Item2);
                        if (!string.IsNullOrEmpty(str))
                        {
                            ret.Add(new Tuple<enmInstType, string, string, string>(enmInstType.FM_M, Path.Combine(srcFn, name).Replace(baseDir, ""), str,""));
                        }
                    }
                    else
                    {
                        ;
                    }
                }

                //inst WFS
                Dictionary<int, Tuple<string, byte[]>> instOPNA2WFS = mv.desVGM.instOPNA2WFS;
                foreach (int num in instOPNA2WFS.Keys)
                {
                    string name = instOPNA2WFS[num].Item1;
                    if (string.IsNullOrEmpty(name))
                    {
                        name = string.Format("OPNA2 SSG WF instrument noname {0}", renban++);
                    }
                    string str = GetInstrumentGwiWfsString(num, name, instOPNA2WFS[num].Item2);

                    if (!string.IsNullOrEmpty(str))
                    {
                        ret.Add(new Tuple<enmInstType, string, string, string>(enmInstType.SSG_WF, Path.Combine(srcFn, name).Replace(baseDir, ""), str,""));
                    }
                }
            }
            catch
            {
                return null;
            }

            return ret;
        }

        private List<Tuple<enmInstType, string, string, string>> GetInstsAtMuc(string srcFn)
        {
            List<Tuple<enmInstType, string, string, string>> ret = new List<Tuple<enmInstType, string, string, string>>();
            int renban = 0;
            try
            {
                string[] lin = File.ReadAllText(srcFn, Encoding.GetEncoding(932)).Split(new string[] { "\r\n" }, StringSplitOptions.None);
                bool fvfg = false;
                for (int i = 0; i < lin.Length; i++)
                {
                    if (string.IsNullOrEmpty(lin[i])) continue;
                    if (lin[i].Length < 4) continue;
                    if (lin[i][0] != ' ') continue;
                    if (lin[i][2] != '@') continue;

                    int srcCPtr = 3;
                    bool carry = false;
                    bool errSign = false;
                    int[][] dat = new int[4][] { new int[11], new int[11], new int[11], new int[11] };
                    List<byte> dat25 = new List<byte>();
                    string name = "";
                    int fb = 0, alg = 0;

                    if (lin[i][srcCPtr] == '%')
                    {
                        srcCPtr++;
                        fvfg = true;
                    }

                    int voiceNum = REDATA(lin[i], ref srcCPtr, ref carry, ref errSign);

                    if (fvfg)
                    {
                        //%25
                        for (int row = 0; row < 6; row++)
                        {
                            i++;
                            srcCPtr = 1;
                            for (int col = 0; col < 4; col++)
                            {
                                byte v = (byte)REDATA(lin[i], ref srcCPtr, ref carry, ref errSign);
                                if (carry || errSign)
                                {
                                    //muc88.WriteWarning(msg.get("W0409"), i, srcCPtr);
                                }
                                srcCPtr++;// SKIP','
                                dat25.Add(v);
                            }
                        }

                        i++;
                        srcCPtr = 2;
                        dat25.Add((byte)REDATA(lin[i], ref srcCPtr, ref carry, ref errSign));
                    }
                    else
                    {
                        //38
                        i++;
                        srcCPtr = 2;
                        fb = REDATA(lin[i], ref srcCPtr, ref carry, ref errSign);
                        if (carry || errSign)
                        {
                            //muc88.WriteWarning(msg.get("W0409"), i, srcCPtr);
                        }
                        srcCPtr++;
                        alg = REDATA(lin[i], ref srcCPtr, ref carry, ref errSign);
                        if (carry || errSign)
                        {
                            //muc88.WriteWarning(msg.get("W0409"), i, srcCPtr);
                        }
                        srcCPtr++;

                        for (int row = 0; row < 4; row++)
                        {
                            i++;
                            srcCPtr = 1;
                            for (int col = 0; col < 9; col++)
                            {
                                byte v = (byte)REDATA(lin[i], ref srcCPtr, ref carry, ref errSign);
                                if (carry || errSign)
                                {
                                    //muc88.WriteWarning(msg.get("W0409"), i, srcCPtr);
                                }
                                srcCPtr++;// SKIP','
                                dat[row][col] = v;
                            }

                            if (row == 3)
                            {
                                for (; srcCPtr < lin[i].Length; srcCPtr++)
                                {
                                    if (lin[i][srcCPtr] != '"') continue;
                                    srcCPtr++;
                                    for (; srcCPtr < lin[i].Length; srcCPtr++)
                                    {
                                        if (lin[i][srcCPtr] == '"') break;
                                        name += lin[i][srcCPtr];
                                    }
                                }
                            }
                        }

                    }


                    //音色定義文を作成

                    if (string.IsNullOrEmpty(name))
                    {
                        name = string.Format("{0}_{1}", Path.GetFileName(srcFn), renban++);
                    }

                    string sdatV, sdatM;
                    sdatV = MakeVoiceDefineForGWI_OPN(name + add, voiceNum, dat, alg, fb);
                    sdatM = MakeVoiceDefineForMUC_OPN(name + add, voiceNum, dat, alg, fb);

                    ret.Add(new Tuple<enmInstType, string, string, string>(enmInstType.FM_N, Path.Combine(srcFn, name).Replace(baseDir, ""), sdatV, sdatM));
                }
            }
            catch
            {
                return null;
            }

            return ret;
        }

        private List<Tuple<enmInstType, string, string, string>> GetInstsAtRym2612(string srcFn)
        {
            List<Tuple<enmInstType, string, string, string>> ret = new List<Tuple<enmInstType, string, string, string>>();
            try
            {
                XElement xml = XElement.Load(srcFn);
                if (xml == null) return null;
                if (xml.Name.ToString().ToUpper().Trim() != "RYM2612PARAMS") return null;

                string patchName = "";
                string category = "";
                var xatr = xml.FirstAttribute;
                while (xatr != null)
                {
                    if (xatr.Name.ToString().ToUpper().Trim() == "PATCHNAME")
                        patchName = xatr.Value;
                    else if (xatr.Name.ToString().ToUpper().Trim() == "CATEGORY")
                        category = xatr.Value;
                    xatr = xatr.NextAttribute;
                }

                Dictionary<string, decimal> dicPatch = new Dictionary<string, decimal>();

                foreach (var eleParam in xml.Elements())
                {
                    if (eleParam.Name.ToString().ToUpper().Trim() != "PARAM") continue;
                    string id = null;
                    string val = null;
                    foreach (var atr in eleParam.Attributes())
                    {
                        XName name = atr.Name;
                        if (name == null) continue;
                        string sval = atr.Value;
                        if (string.IsNullOrEmpty(sval)) continue;
                        string sname = name.ToString().ToUpper().Trim();
                        sval = sval.ToUpper().Trim();

                        if (sname == "ID") id = sval;
                        else if (sname == "VALUE") val = sval;
                    }
                    if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(val)) continue;

                    if (dicPatch.ContainsKey(id)) dicPatch.Remove(id);
                    decimal q;
                    if (decimal.TryParse(val, out q))
                    {
                        dicPatch.Add(id, q);
                    }
                }

                int[][] iop = new int[4][] { new int[11], new int[11], new int[11], new int[11] };
                int[] muls = new int[] { 0, 1054, 1581, 2635, 3689, 4743, 5797, 6851, 7905, 8959, 10013, 10540, 11594, 12648, 14229, 15000 };
                for (int op = 0; op < 4; op++)
                {
                    string sop = $"OP{op + 1}";
                    string key;
                    key = sop + "AR"; if (dicPatch.ContainsKey(key)) iop[op][0] = (int)dicPatch[key];
                    key = sop + "D1R"; if (dicPatch.ContainsKey(key)) iop[op][1] = (int)dicPatch[key];
                    key = sop + "D2R"; if (dicPatch.ContainsKey(key)) iop[op][2] = (int)dicPatch[key];
                    key = sop + "RR"; if (dicPatch.ContainsKey(key)) iop[op][3] = (int)dicPatch[key];
                    key = sop + "D2L"; if (dicPatch.ContainsKey(key)) iop[op][4] = 15 - (int)dicPatch[key];
                    key = sop + "TL"; if (dicPatch.ContainsKey(key)) iop[op][5] = 127 - (int)dicPatch[key];
                    key = sop + "RS"; if (dicPatch.ContainsKey(key)) iop[op][6] = (int)dicPatch[key];
                    key = sop + "MUL"; if (dicPatch.ContainsKey(key))
                    {
                        decimal ml = dicPatch[key];
                        decimal dis = decimal.MaxValue;
                        int dml = 0;
                        for (int m = 0; m < muls.Length; m++)
                        {
                            if (dis <= Math.Abs(muls[m] - ml))
                                continue;

                            dis = Math.Abs(muls[m] - ml);
                            dml = m;
                        }
                        iop[op][7] = dml;
                    }
                    key = sop + "DT"; if (dicPatch.ContainsKey(key)) iop[op][8] = (int)dicPatch[key] >= 0 ? (int)dicPatch[key] : (4 - (int)dicPatch[key]);
                    key = sop + "AM"; if (dicPatch.ContainsKey(key)) iop[op][9] = (int)dicPatch[key];
                    key = sop + "SSGEG"; if (dicPatch.ContainsKey(key)) iop[op][10] = (int)dicPatch[key] == 0 ? 0 : ((int)dicPatch[key] + 7);
                }

                int alg = 0, fb = 0;
                if (dicPatch.ContainsKey("ALGORITHM")) alg = (int)dicPatch["ALGORITHM"] - 1; else alg = 0;
                if (dicPatch.ContainsKey("FEEDBACK")) fb = (int)dicPatch["FEEDBACK"]; else fb = 0;

                //音色定義文を作成
                string vname = "";
                int renban = 0;
                int voiceNum = 0;

                if (string.IsNullOrEmpty(vname))
                {
                    vname = string.Format("{0}_{1}", Path.GetFileName(srcFn), renban++);
                }

                string sdatV,sdatM;
                    sdatV = MakeVoiceDefineForGWI_OPN(patchName == "" ? (vname + add) : patchName, voiceNum, iop, alg, fb);
                    sdatM = MakeVoiceDefineForMUC_OPN(patchName == "" ? (vname + add) : patchName, voiceNum, iop, alg, fb);

                ret.Add(new Tuple<enmInstType, string, string, string>(enmInstType.FM_N, srcFn.Replace(baseDir, ""), sdatV,sdatM));
            }
            catch
            {
                return null;
            }

            return ret;
        }

        private List<Tuple<enmInstType,string,string,string>> GetInstsAtBambooTracker(string srcFn)
        {
            List<Tuple<enmInstType, string, string, string>> ret = new List<Tuple<enmInstType, string, string, string>>();

            try
            {
                byte[] bin = File.ReadAllBytes(srcFn);//, Encoding.GetEncoding(932)).Split(new string[] { "\r\n" }, StringSplitOptions.None);
                if (bin == null || bin.Length < 24) return null;
                string identifier = System.Text.Encoding.ASCII.GetString(bin, 0, 16);
                if (identifier != "BambooTrackerBnk") return null;
                int eofOffset = BitConverter.ToInt32(bin, 16);
                int fileVersion = BitConverter.ToInt32(bin, 20);
                if (fileVersion < 0x00010000) return null;

                identifier = System.Text.Encoding.ASCII.GetString(bin, 24, 8);
                if (identifier != "INSTRMNT") return null;
                int instEndOffset = 32 + BitConverter.ToInt32(bin, 32);
                int instCount = bin[36];
                if (instCount < 1) return null;
                int ptr = 37;

                List<Tuple<int, string>> lstInst = new List<Tuple<int, string>>();
                for (int ic = 0; ic < instCount; ic++)
                {
                    int envNum = bin[ptr++];
                    int instDetailOffset = BitConverter.ToInt32(bin, ptr);
                    int instDetailAdr = ptr + instDetailOffset;
                    ptr += 4;
                    int instNameLength = BitConverter.ToInt32(bin, ptr);
                    ptr += 4;
                    string instName = System.Text.Encoding.UTF8.GetString(bin, ptr, instNameLength);
                    ptr += instNameLength;
                    int instType = bin[ptr++];
                    if (instType == 0)
                    {
                        lstInst.Add(new Tuple<int, string>(envNum, instName));
                    }

                    ptr = instDetailAdr;
                }

                ptr = instEndOffset;

                identifier = System.Text.Encoding.ASCII.GetString(bin, ptr, 8);
                if (identifier != "INSTPROP") return null;
                ptr += 8;
                int instPropOffset = ptr + BitConverter.ToInt32(bin, ptr);
                ptr += 4;

                int subsectionIdentifier = bin[ptr++];
                if (subsectionIdentifier != 0) return null;
                int blockCount = bin[ptr++];

                List<Tuple<int, byte[]>> lstProp = new List<Tuple<int, byte[]>>();
                for (int ic = 0; ic < blockCount; ic++)
                {
                    int envIndex = bin[ptr++];
                    int envOffset = ptr + bin[ptr++];
                    List<byte> prm = new List<byte>();
                    prm.Add(bin[ptr++]);//AL/FB
                    for(int op = 0; op < 4; op++)
                    {
                        prm.Add(bin[ptr++]);//Enable/AR
                        prm.Add(bin[ptr++]);//KS/DR
                        prm.Add(bin[ptr++]);//DT/SR
                        prm.Add(bin[ptr++]);//SL/RR
                        prm.Add(bin[ptr++]);//TL
                        prm.Add(bin[ptr++]);//SSGEG/ML
                    }
                    ptr = envOffset;
                    lstProp.Add(new Tuple<int, byte[]>(envIndex, prm.ToArray()));
                }

                for (int ic = 0; ic < blockCount; ic++)
                {
                    int envIndex = lstProp[ic].Item1;
                    string name = "";
                    //for(int inum = 0; inum < lstInst.Count; inum++)
                    //{
                    //    if (envIndex != lstInst[inum].Item1) continue;
                    //    name = lstInst[inum].Item2;
                    //}
                    name = lstInst[ic].Item2;
                    byte[] dat = lstProp[ic].Item2;

                    int alg = (byte)(dat[0] >> 4);
                    int fb = (byte)(dat[0] & 0xf);

                    int[][] prms = new int[][] { new int[11], new int[11], new int[11], new int[11] };
                    for (int op = 0; op < 4; op++)
                    {
                        prms[op][0] = dat[1 + op * 6] & 0x1f;//AR
                        prms[op][1] = dat[2 + op * 6] & 0x1f;//DR
                        prms[op][2] = dat[3 + op * 6] & 0x1f;//SR
                        prms[op][3] = dat[4 + op * 6] & 0x0f;//RR
                        prms[op][4] = dat[4 + op * 6] >> 4;//SL
                        prms[op][5] = dat[5 + op * 6] & 0x7f;//TL
                        prms[op][6] = (dat[2 + op * 6] >> 5) & 0x3;//KS
                        prms[op][7] = dat[6 + op * 6] & 0x0f;//ML
                        prms[op][8] = (dat[3 + op * 6] >> 5) & 0x7;//DT
                        prms[op][9] = 0;
                        prms[op][10] = dat[6 + op * 6] >> 4;//SSGEG;
                        if (prms[op][10] == 0x8) prms[op][10] = 0;
                    }

                    //音色定義文を作成
                    string sdatV, sdatM;
                    sdatV = MakeVoiceDefineForGWI_OPN(name + add, envIndex, prms, alg, fb);
                    sdatM = MakeVoiceDefineForMUC_OPN(name + add, envIndex, prms, alg, fb);

                    ret.Add(new Tuple<enmInstType, string, string, string>(
                        enmInstType.FM_N, Path.Combine(srcFn, name).Replace(baseDir, ""), sdatV, sdatM));
                }

            }
            catch
            {
                return null;
            }

            return ret;
        }

        private void Disp(string msg)
        {

        }

        private string GetInstrumentGwiFmOPMString(int num, string name, byte[] val)
        {

            string[] line = new string[5];
            for (int i = 0; i < 4; i++)
            {
                line[i] = "";
                for (int j = 0; j < 11; j++)
                {
                    line[i] += string.Format("{0:D3} ", val[i * 11 + j + 1]);
                }
            }
            line[4] = string.Format("{0:D3} {1:D3}", val[45], val[46]);

            return string.Format(
@"'@ M {6} ""{0}""
   AR  DR  SR  RR  SL  TL  KS  ML  DT1 DT2 AME
'@ {1}
'@ {2}
'@ {3}
'@ {4}
   AL  FB
'@ {5}
", name + add, line[0], line[1], line[2], line[3], line[4], num);

        }

        private string GetInstrumentGwiFmOPLLString(int num, string name, byte[] val)
        {

            string[] line = new string[3];
            for (int i = 0; i < 2; i++)
            {
                line[i] = "";
                for (int j = 0; j < 11; j++)
                {
                    line[i] += string.Format("{0:D3} ", val[i * 11 + j + 1]);
                }
            }
            line[2] = string.Format("{0:D3} {1:D3}", val[23], val[24]);

            return string.Format(
@"'@ LL {4} ""{0}""
   AR  DR  SL  RR  KL  MT  AM  VB  EG  KR  DT
'@ {1}
'@ {2}
   TL  FB
'@ {3}
", name + add, line[0], line[1], line[2], num);

        }

        private string GetInstrumentGwiFmOPLString(int num, string name, byte[] val)
        {

            string[] line = new string[3];
            for (int i = 0; i < 2; i++)
            {
                line[i] = "";
                for (int j = 0; j < 12; j++)
                {
                    line[i] += string.Format("{0:D3} ", val[i * 12 + j + 1]);
                }
            }
            line[2] = string.Format("{0:D3} {1:D3}", val[25], val[26]);

            return string.Format(
@"'@ L {4} ""{0}""
   AR  DR  SL  RR  KSL TL  MT  AM  VIB EGT KSR WS
'@ {1}
'@ {2}
   AL  FB
'@ {3}
", name + add, line[0], line[1], line[2], num);

        }

        private string GetInstrumentGwiFmOPL4String(int num, string name, byte[] val)
        {

            string[] line = new string[5];
            for (int i = 0; i < 4; i++)
            {
                line[i] = "";
                for (int j = 0; j < 12; j++)
                {
                    line[i] += string.Format("{0:D3} ", val[i * 12 + j + 1]);
                }
            }
            line[4] = string.Format("{0:D3} {1:D3} {2:D3}", val[49], val[50], val[51]);

            return string.Format(
@"'@ L4 {6} ""{0}""
   AR  DR  SL  RR  KSL TL  MT  AM  VIB EGT KSR WS
'@ {1}
'@ {2}
'@ {3}
'@ {4}
   CNT1 CNT2 FB
'@ {5}
", name + add, line[0], line[1], line[2], line[3], line[4], num);

        }

        private string GetInstrumentGwiWFString(int num, string name, byte[] val)
        {

            string[] line = new string[5];
            for (int i = 0; i < 4; i++)
            {
                line[i] = "";
                for (int j = 0; j < 8; j++)
                {
                    line[i] += string.Format("{0:D2} ", val[i * 8 + j + 1]);
                }
            }
            line[4] = string.Format("{0:D3}", val[32]);

            return string.Format(
@"'@ H {5} ""{0}""
   +0 +1 +2 +3 +4 +5 +6 +7
'@ {1}
'@ {2}
'@ {3}
'@ {4}
", name + add, line[0], line[1], line[2], line[3], line[4]);

        }

        private string GetInstrumentGwiWfsString(int num, string name, byte[] val)
        {

            string[] line = new string[4];
            for (int i = 0; i < 4; i++)
            {
                line[i] = "";
                for (int j = 0; j < 16; j++)
                {
                    line[i] += string.Format("${0:X02} ", val[i * 16 + j + 1]);
                }
            }

            return string.Format(
@"'@ WS {0} ""{1}""
'@ {2}
'@ {3}
'@ {4}
'@ {5}
",num , name + add, line[0], line[1], line[2], line[3]);

        }

        private int REDATA(string lin, ref int srcCPtr,ref bool carry,ref bool ErrSign)
        {
            int[] SCORE = new int[6];
            carry = false;

            for (int i = 0; i < SCORE.Length; i++) SCORE[i] = 0;
            int degit = 5;   // 5ｹﾀ ﾏﾃﾞ

            int HEXFG = 0;
            int MINUSF = 0;
            

            //READ0:			// FIRST CHECK
            char ch;

            do
            {
                if (lin.Length == srcCPtr)
                {
                    srcCPtr++;
                    carry = true; // NON DATA
                    return 0;
                }
                ch = lin.Length > srcCPtr ? lin[srcCPtr] : (char)0;
                srcCPtr++;
            } while (ch == ' ');

            if (ch == '$')//0x24
            {
                HEXFG = 1;
                srcCPtr++;
                goto READ7;
            }

            if (ch == '-')//0x2d
            {
                ch = lin.Length > srcCPtr ? lin[srcCPtr] : (char)0;
                srcCPtr++;
                if (ch < '0' || ch > '9')//0x30 0x39
                {
                    goto READE;//0ｲｼﾞｮｳ ﾉ ｷｬﾗｸﾀﾅﾗ ﾂｷﾞ
                }
                MINUSF = 1;   // SET MINUS FLAG
                goto READ7;
            }

            if (ch < '0' || ch > '9')//0x30 0x39
            {
                goto READE;//0ｲｼﾞｮｳ ﾉ ｷｬﾗｸﾀﾅﾗ ﾂｷﾞ
            }

        READ7:
            srcCPtr--;
            do
            {
                ch = lin.Length > srcCPtr ? lin[srcCPtr] : (char)0;
                if (HEXFG == 0)
                {
                    goto READC;
                }

                if (ch >= 'a' && ch <= 'f')
                {
                    ch -= (char)32;
                }
                //READG:
                if (ch >= 'A' && ch <= 'F')
                {
                    ch -= (char)7;
                    goto READF;
                }
            READC:
                if (ch < '0' || ch > '9')
                {
                    goto READ1;//9ｲｶﾅﾗ ﾂｷﾞ
                }
            READF:

                SCORE[0] = SCORE[1];
                SCORE[1] = SCORE[2];
                SCORE[2] = SCORE[3];
                SCORE[3] = SCORE[4];
                SCORE[4] = SCORE[5];

                ch -= (char)0x30;// A= 0 - 9
                SCORE[4] = (byte)ch;
                srcCPtr++; // NEXT TEXT
                degit--;

                if (lin.Length == srcCPtr) goto READ1;

            } while (degit > 0);

            ch = lin.Length > srcCPtr ? lin[srcCPtr] : (char)0; // THIRD CHECK
            if (ch < '0' || ch > '9')
            {
                goto READ1;//9ｲｶﾅﾗ ﾂｷﾞ
            }
            //READ8:
            carry = false;
            ErrSign = true;// ERROR SIGN
            return 0;//RET	; 7ｹﾀｲｼﾞｮｳ ﾊ ｴﾗｰ

        READ1:
            int a = 0;
            if (HEXFG == 1)
            {
                for (int i = 1; i < 5; i++)
                {
                    a *= 16;
                    a += SCORE[i];
                }
                goto READA;
            }
            //READD:
            for (int i = 0; i < 5; i++)
            {
                a *= 10;
                a += SCORE[i];
            }

            if (MINUSF != 0)
            {// CHECK MINUS FLAG
                a = -a;
            }
        READA:
            carry = false;
            return a;//    RET
        READE:
            //SECCOM = (byte)ch;
            carry = true; // NON DATA
            return 0;
        }

        private string MakeVoiceDefineForGWI_OPN(string patchName,int voiceNo,int[][] prms,int alg,int fb)
        {
            string[] line = new string[5];
            for (int j = 0; j < 4; j++)
            {
                line[j] = "";
                for (int k = 0; k < 11; k++)
                {
                    line[j] += string.Format("{0:D3} ", (int)prms[j][k]);
                }
            }
            line[4] = string.Format("{0:D3} {1:D3}", alg, fb);

            string sdat = string.Format(
@"'@ N {6} ""{0}""
   AR  DR  SR  RR  SL  TL  KS  ML  DT  AM  SSGEG
'@ {1}
'@ {2}
'@ {3}
'@ {4}
   AL  FB
'@ {5}
", patchName, line[0], line[1], line[2], line[3], line[4], voiceNo);

            return sdat;
        }

        private string MakeVoiceDefineForMUC_OPN(string patchName, int voiceNo, int[][] prms, int alg, int fb)
        {
            string[] line = new string[5];
            for (int j = 0; j < 4; j++)
            {
                line[j] = "";
                for (int k = 0; k < 9; k++)
                {
                    line[j] += string.Format("{0:D3} ", (int)prms[j][k]);
                }
            }
            line[4] = string.Format("{0:D3} {1:D3}", fb, alg);

            string sdat = string.Format(
@"  @{6}:{{ ""{0}""
  {5}
  {1}
  {2}
  {3}
  {4}
  }}
", patchName, line[0], line[1], line[2], line[3], line[4], voiceNo);

            return sdat;
        }
    }
}
