using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mml2vgmIDE
{
    public class InstrumentAtLocal
    {
        private string add = "";

        public bool isFirst = false;
        private FrmMain parent = null;
        private object sender = null;
        private string baseDir = "";

        public enum enmInstType
        {
            Dir,
            FM_N,//OPN
            FM_L,//OPL
            FM_M,//OPM
            FM_NA,//OPNA2(FM)
            SSG_WF,//OPNA2(SSG)
        }

        public void Start(FrmMain parent, object sender, string baseDir,string add, Action<object, Tuple<enmInstType, string, string>[]> CompleteMethod)
        {
            this.parent = parent;
            this.sender = sender;
            this.baseDir = baseDir;
            this.add = add;

            //mmlファイルの一覧を得る
            List<FileSystemInfo> lstFile = GetFileList(baseDir);
            //音色データを解析する
            List<Tuple<enmInstType, string, string>> lstInst = new List<Tuple<enmInstType, string, string>>();
            foreach (FileSystemInfo fi in lstFile)
            {
                if (fi.Attributes == FileAttributes.Directory)
                {
                    Tuple<enmInstType, string, string> ins = new Tuple<enmInstType, string, string>(enmInstType.Dir, fi.FullName, "");
                    lstInst.Add(ins);
                }
                else
                {
                    List<Tuple<enmInstType, string, string>> ins = GetInsts(fi.FullName);
                    if (ins != null) lstInst.AddRange(ins);
                }
            }
            CompleteMethod?.Invoke(this.sender, lstInst.ToArray());
        }

        private List<FileSystemInfo> GetFileList(string baseDir)
        {
            List<FileSystemInfo> lstFile = new List<FileSystemInfo>();
            try
            {
                DirectoryInfo di = new DirectoryInfo(baseDir);
                
                IEnumerable<FileSystemInfo> files = di.EnumerateFileSystemInfos();
                foreach (FileSystemInfo f in files)
                {
                    if (f.Attributes == FileAttributes.Directory)
                    {
                        lstFile.Add(f);
                    }
                    else
                    {
                        string ext = Path.GetExtension(f.FullName).ToLower();
                        if (ext == ".gwi" || ext == ".muc" || ext == ".mml")
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

            return lstFile;
        }

        private List<Tuple<enmInstType, string, string>> GetInsts(string file)
        {
            List<Tuple<enmInstType, string, string>> lstInst = null;
            string ext = Path.GetExtension(file).ToLower();
            switch (ext)
            {
                case ".gwi":
                    lstInst = GetInstsAtGwi(file);
                    break;
                case ".muc":
                    lstInst = GetInstsAtMuc(file);
                    break;
            }

            return lstInst;
        }

        private List<Tuple<enmInstType, string, string>> GetInstsAtGwi(string srcFn)
        {
            List<Tuple<enmInstType, string, string>> ret = new List<Tuple<enmInstType, string, string>>();
            try
            {
                string stPath = System.Windows.Forms.Application.StartupPath;
                Core.Mml2vgm mv = new Core.Mml2vgm(srcFn, "", stPath, Disp);
                if (mv.Start_Analyze() != 0) return null;

                //inst FM
                Dictionary<int, Tuple<string, byte[]>> instFM = mv.desVGM.instFM;
                foreach (int num in instFM.Keys)
                {
                    if (instFM[num].Item2.Length == 47)
                    {
                        string name = instFM[num].Item1;
                        string str = GetInstrumentGwiFmString(num, name, instFM[num].Item2);
                        if (!string.IsNullOrEmpty(str))
                        {
                            ret.Add(new Tuple<enmInstType, string, string>(enmInstType.FM_N, Path.Combine(srcFn, name).Replace(baseDir, ""), str));
                        }
                    }
                }

                //inst WFS
                Dictionary<int, Tuple<string, byte[]>> instOPNA2WFS = mv.desVGM.instOPNA2WFS;
                foreach (int num in instOPNA2WFS.Keys)
                {
                    string name = instOPNA2WFS[num].Item1;
                    string str = GetInstrumentGwiWfsString(num, name, instOPNA2WFS[num].Item2);

                    if (!string.IsNullOrEmpty(str))
                    {
                        ret.Add(new Tuple<enmInstType, string, string>(enmInstType.SSG_WF, Path.Combine(srcFn, name).Replace(baseDir, ""), str));
                    }
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

        private string GetInstrumentGwiFmString(int num, string name, byte[] val)
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
@"'@ N {6} ""{0}""
   AR  DR  SR  RR  SL  TL  KS  ML  DT  AM  SSGEG
'@ {1}
'@ {2}
'@ {3}
'@ {4}
   AL  FB
'@ {5}
", name + add, line[0], line[1], line[2], line[3], line[4], num);

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



        private List<Tuple<enmInstType, string, string>> GetInstsAtMuc(string srcFn)
        {
            List<Tuple<enmInstType, string, string>> ret = new List<Tuple<enmInstType, string, string>>();

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
                    List<byte> dat = new List<byte>();
                    List<byte> dat25 = new List<byte>();
                    string name = "";

                    if (lin[i][srcCPtr] == '%')
                    {
                        srcCPtr++;
                        fvfg = true;
                    }

                    int voiceNum = REDATA(lin[i], ref srcCPtr,ref carry,ref errSign);

                    if (fvfg)
                    {
                        //%25
                        for (int row = 0; row < 6; row++)
                        {
                            i++;
                            srcCPtr = 1;
                            for (int col = 0; col < 4; col++)
                            {
                                byte v = (byte)REDATA(lin[i], ref srcCPtr,ref carry,ref errSign);
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
                        int fb = REDATA(lin[i], ref srcCPtr, ref carry, ref errSign);
                        if (carry || errSign)
                        {
                            //muc88.WriteWarning(msg.get("W0409"), i, srcCPtr);
                        }
                        srcCPtr++;
                        int alg = REDATA(lin[i], ref srcCPtr, ref carry, ref errSign);
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
                                dat.Add(v);
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

                        dat.Add((byte)fb);
                        dat.Add((byte)alg);
                    }


                    //音色定義文を作成

                    string[] line = new string[5];
                    for (int j = 0; j < 4; j++)
                    {
                        line[j] = "";
                        for (int k = 0; k < 9; k++)
                        {
                            line[j] += string.Format("{0:D3} ", dat[j * 9 + k + 0]);
                        }
                    }
                    line[4] = string.Format("{0:D3} {1:D3}", dat[37], dat[36]);

                    if (string.IsNullOrEmpty(name))
                    {
                        name = string.Format("{0}_{1}", Path.GetFileName(srcFn), voiceNum);
                    }

                    string sdat = string.Format(
        @"'@ N {6} ""{0}""
   AR  DR  SR  RR  SL  TL  KS  ML  DT  AM  SSGEG
'@ {1}000 000
'@ {2}000 000
'@ {3}000 000
'@ {4}000 000
   AL  FB
'@ {5}
", name + add, line[0], line[1], line[2], line[3], line[4], voiceNum);
                    //Console.WriteLine("{0}",sdat);


                    ret.Add(new Tuple<enmInstType, string, string>(enmInstType.FM_N, Path.Combine(srcFn, name).Replace(baseDir, ""), sdat));
                }
            }
            catch
            {
                return null;
            }

            return ret;
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
    }
}
