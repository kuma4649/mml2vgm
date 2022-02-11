using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace mml2vgmIDE.D88N88
{
    public class N88Disk
    {
        private D88 d88 = null;
        private List<N88FileAtr> directory = new List<N88FileAtr>();
        private byte[] FAT = new byte[256];

        private string[] codeTable = new string[]
        {
            "",         "END",          "FOR",      "NEXT",     "DATA",     "INPUT",    "DIM",      "READ",
            "LET",      "GOTO",         "RUN",      "IF",       "RESTORE",  "GOSUB",    "RETURN",   "REM",
            "STOP",     "PRINT",        "CLEAR",    "LIST",     "NEW",      "ON",       "WAIT",     "DEF",
            "POKE",     "CONT",         "OUT",      "LPRINT",   "LLIST",    "CONSOLE",  "WIDTH",    "ELSE",
            "TRON",     "TROFF",        "SWAP",     "ERASE",    "EDIT",     "ERROR",    "RESUME",   "DELETE",
            "AUTO",     "RENUM",        "DEFSTR",   "DEFINT",   "DEFSNG",   "DEFDBL",   "LINE",     "WHILE",

            "WEND",     "CALL",         "",         "",         "",         "WRITE",    "COMMON",   "CHAIN",
            "OPTION",   "RANDOMIZE",    "DSKO$",    "OPEN",     "FIELD",    "GET",      "PUT",      "SET",
            "CLOSE",    "LOAD",         "MERGE",    "FILES",    "NAME",     "KILL",     "LSET",     "RSET",
            "SAVE",     "LFILES",       "MON",      "COLOR",    "CIRCLE",   "COPY",     "CLS",      "PSET",
            "PRESET",   "PAINT",        "TERM",     "SCREEN",   "BLOAD",    "BSAVE",    "LOCATE",   "BEEP",
            "ROLL",     "HELP",         "",         "KANJI",    "TO",       "THEN",     "TAB(",     "STEP",

            "USR",      "FN",           "SPC(",     "NOT",      "ERL",      "ERR",      "STRING$",  "USING",
            "INSTR",    "'",            "VARPTR",   "ATTR$",    "DSKI$",    "SRQ",      "OFF",      "INKEY$",
            ">",        "=",            "<",        "+",        "-",        "*",        "/",        "^",
            "AND",      "OR",           "XOR",      "EQV",      "IMP",      "MOD",      "\\",       "",
        };
        private string[] codeTableFF = new string[]
        {
            "",            "LEFT$",       "RIGHT$",      "MID$",        "SGN",         "INT",         "ABS",         "SQR",
            "RND",         "SIN",         "LOG",         "EXP",         "COS",         "TAN",         "ATN",         "FRE",
            "INP",         "POS",         "LEN",         "STR$",        "VAL",         "ASC",         "CHR$",        "PEEK",
            "SPACE$",      "OCT$",        "HEX$",        "LPOS",        "CINT",        "CSNG",        "CDBL",        "FIX",
            "CVI",         "CVS",         "CVD",         "EOF",         "LOC",         "LOF",         "FPOS",        "MKI$",
            "MKS$",        "MKD$",        "",            "",            "",            "",            "",            "",
            //0xb0 - 0xbf
            "",            "",            "",            "",            "",            "",            "",            "",
            "",            "",            "",            "",            "",            "",            "",            "",
            //0xc0 - 0xcf
            "",            "",            "",            "",            "",            "",            "",            "",
            "",            "",            "",            "",            "",            "",            "",            "",
            "DSKF",        "VIEW",        "WINDOW",      "POINT",       "CSRLIN",      "MAP",         "SEARCH",      "MOTOR",
            "PEN",         "DATE$",       "COM",         "KEY",         "TIME$",       "WBYTE",       "RBYTE",       "POLL",
            "ISET",        "IEEE",        "IRESET",      "STATUS",      "CMD",         "",            "",            "",
            "",            "",            "",            "",            "",            "",            "",            "",
            "",            "",            "",            "",            "",            "",            "",            "",
            "",            "",            "",            "",            "",            "",            "",            "",
        };

        private const int Directorytrack = 37;
        private const int DirectorySector = 0;
        private const int DirectoryInfo = 16;
        private const byte DirectoryUnuse = 0xff;
        private const int FATtrack = 37;
        private const int FATsector = 13;




        public N88Disk(D88 d88)
        {
            this.d88 = d88;
        }



        public List<N88FileAtr> GetDirectories()
        {
            directory.Clear();
            int trkPtr = Directorytrack;
            int secPtr = DirectorySector;
            int ptr = 0;

            while (true)
            {
                byte[] fn = new byte[DirectoryInfo];//1つのファイル当たり16byte
                Array.Copy(d88.tracks[trkPtr].sectors[secPtr].data, ptr, fn, 0, DirectoryInfo);
                if (fn[0] == DirectoryUnuse) break;//未使用の場合は取得完了
                directory.Add(new N88FileAtr(fn));
                ptr += DirectoryInfo;
                if (ptr >= d88.tracks[trkPtr].sectors[secPtr].data.Length)
                {
                    secPtr++;
                    ptr = 0;
                    if (secPtr >= d88.tracks[trkPtr].sectors.Length)
                    {
                        trkPtr++;
                        secPtr = 0;
                        if (trkPtr >= d88.tracks.Length)
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                    }
                }
            }

            return directory;
        }

        private N88FileAtr SetDirectories(byte[] atrb)
        {
            N88FileAtr atr = new N88FileAtr(atrb);
            directory.Add(atr);

            int trkPtr = Directorytrack;
            int secPtr = DirectorySector;
            int ptr = 0;

            foreach (N88FileAtr a in directory)
            {
                for (int rPtr = 0; rPtr < DirectoryInfo; rPtr++)
                {
                    d88.tracks[trkPtr].sectors[secPtr].data[ptr++] = a.raw[rPtr];
                    if (ptr < d88.tracks[trkPtr].sectors[secPtr].data.Length) continue;
                    secPtr++;
                    ptr = 0;
                    if (secPtr < d88.tracks[trkPtr].sectors.Length) continue;
                    trkPtr++;
                    secPtr = 0;
                    if (trkPtr < d88.tracks.Length) continue;
                    throw new ArgumentOutOfRangeException();
                }
            }

            return atr;
        }

        private N88FileAtr SearchDirectory(string fn)
        {
            N88FileAtr atr = null;
            string da;
            da = getFn(fn);
            foreach (N88FileAtr a in directory)
            {
                string sa = getFn(a.fn);
                if (sa == da)
                {
                    atr = a;
                    break;
                }
            }

            return atr;
        }



        public byte[] GetFAT()
        {
            Array.Copy(
                d88.tracks[FATtrack].sectors[FATsector].data, 0,
                FAT, 0,
                Math.Min(256, d88.tracks[FATtrack].sectors[FATsector].data.Length)
                );

            return FAT;
        }

        private void SetFAT()
        {
            int ptr;
            int trkPtr = FATtrack;
            int secPtr = FATsector;

            for (int j = 0; j < 3; j++)//同じもの３回書く
            {
                ptr = 0;
                for (int i = 0; i < FAT.Length; i++)
                {
                    d88.tracks[trkPtr].sectors[secPtr].data[ptr++] = FAT[i];
                    if (ptr < d88.tracks[trkPtr].sectors[secPtr].data.Length) continue;
                    secPtr++;
                    ptr = 0;
                    if (secPtr < d88.tracks[trkPtr].sectors.Length) continue;
                    trkPtr++;
                    secPtr = 0;
                    if (trkPtr < d88.tracks.Length) continue;
                    throw new ArgumentOutOfRangeException();
                }
            }

        }

        public int GetUseClusterFromFAT(byte cluster)
        {
            byte b = cluster;
            int sec = 0;
            int limit = 0xa0;
            while (b < 0xc1 || b > 0xc8)
            {
                b = FAT[b];
                sec += 8;

                //FATが壊れている時向け処理
                limit--;
                if (limit == 0)
                {
                    throw new FileNotFoundException("FAT error");
                }
            }

            return (sec + (b & 0x1f)) / 8;
        }



        public byte[] GetFile(string fn, int mode)
        {
            //ファイル名存在チェック
            N88FileAtr atr = SearchDirectory(fn);
            if (atr == null) throw new FileNotFoundException();
            //ファイルのデータをディスク(.d88)から読み込む
            List<byte> buf = GetFileRawData(atr);

            if ((atr.atr & 1) != 0)
            {
                //バイナリファイル
                //先頭の２byteを切り出すだけ
                int loadAddress = buf[0] + buf[1] * 0x100;
                buf.RemoveAt(0); buf.RemoveAt(0);
                //Console.WriteLine("load address = &H{0:X}", loadAddress);
                return buf.ToArray();
            }
            else if ((atr.atr & 0x80) != 0)
            {
                //非アスキーモード = 中間コードベーシックファイル
                //中間コードからBASICのプログラムソースを生成する
                while (buf[buf.Count - 1] == 0x00) buf.RemoveAt(buf.Count - 1);//尻を削る
                buf = ConvertPlaneBasic(buf);
                return buf.ToArray();
            }
            else
            {
                //アスキーモード ベーシックファイル
                while (buf[buf.Count - 1] == 0x00) buf.RemoveAt(buf.Count - 1);//尻を削る
                if (mode == 0)
                {
                    //一旦文字列に直して改行コードを変換し更にbyte配列に戻す...
                    string sbuf = Encoding.GetEncoding("shift_jis").GetString(buf.ToArray());
                    sbuf = sbuf.Replace("\r", "\r\n");
                    return Encoding.GetEncoding("shift_jis").GetBytes(sbuf);
                }
                return buf.ToArray();
            }
        }

        public byte[] SetFile(string fn, byte[] buf, int mode, int address)
        {

            //ファイルの存在チェック
            N88FileAtr atr = SearchDirectory(fn);
            //directoryにない時は新規ファイルとする
            if (atr == null) atr = SetFile_NewFile(fn, mode);

            // crlf -> lf 変換
            if (mode == 1) // 1: sa mode
            {
                //一度、stringに戻すorz
                string sbuf = Encoding.GetEncoding("shift_jis").GetString(buf);
                sbuf = sbuf.Replace("\r\n", "\r");
                buf = Encoding.GetEncoding("shift_jis").GetBytes(sbuf);
            }
            else if (mode == 3) // 3: sb mode
            {
                //一度、stringに戻すorz
                string sbuf = Encoding.GetEncoding("shift_jis").GetString(buf);
                string[] lines = sbuf.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                //中間ファイルに変換
                buf = ConvertObjectBasic(lines);
            }

            //出力用バッファ
            List<byte> lBuf = new List<byte>();

            //バイナリの場合は先頭にアドレスをくっつける
            if (mode == 0) // 0: s mode
            {
                lBuf.Add((byte)address);
                lBuf.Add((byte)(address >> 8));
            }

            //取得したバッファを追加
            lBuf.AddRange(buf);

            //パディング
            while (lBuf.Count % 256 != 0) lBuf.Add(0);

            //DATAの書き出し
            SetFileRawData(lBuf.ToArray(), atr);

            return d88.GetRaw();
        }


        private List<byte> GetFileRawData(N88FileAtr atr)
        {
            //ファイル属性に記される先頭のクラスター
            byte cls = atr.cluster;
            int saf = (cls % 4) / 2;//サーフェス
            int trk = cls / 4 * 2 + saf;//d88はsafが0/1をくりかえす
            int sec = (cls % 2) * 8;//d88はsectorを0から数える
            List<byte> buf = new List<byte>();
            //残りのセクター数(0の場合は次のトラックを読む必要がある)
            int zanSector = (FAT[cls] >= 0xc1 && FAT[cls] <= 0xc8) ? (FAT[cls] & 0xf) : 0;

            int limit = 0xa0;

            do
            {
                //セクターのデータ全て取得
                buf.AddRange(d88.tracks[trk].sectors[sec].data);
                sec++;
                zanSector--;
                if (zanSector == 0) break;
                if (zanSector < 0) zanSector = -1;//念のため

                //セクターが8の倍数値になった時は次のクラスターをFATから読み出す
                if (sec % 8 == 0)
                {
                    cls = (byte)(FAT[cls]);
                    saf = (cls % 4) / 2;//サーフェス
                    trk = cls / 4 * 2 + saf;//d88はsafが0/1をくりかえす
                    sec = (cls % 2) * 8;//d88はsectorを0から数える
                    zanSector = (FAT[cls] >= 0xc1 && FAT[cls] <= 0xc8) ? (FAT[cls] & 0xf) : 0;
                }

                //FATが壊れている時向け処理
                limit--;
                if (limit == 0)
                    throw new FileNotFoundException("FAT error");
            } while (true);
            return buf;
        }

        private void SetFileRawData(byte[] buf, N88FileAtr atr)
        {
            int bufPtr = 0;

            byte cls = atr.cluster;
            int saf = (cls % 4) / 2;//サーフェス
            int trkPtr = cls / 4 * 2 + saf;//d88はsafが0/1をくりかえす
            int secPtr = (cls % 2) * 8;//d88はsectorを0から数える
            int ptr = 0;

            do
            {

                d88.tracks[trkPtr].sectors[secPtr].data[ptr] = buf[bufPtr++];
                if (bufPtr == buf.Length) break;

                ptr++;
                if (ptr < d88.tracks[trkPtr].sectors[secPtr].data.Length) continue;
                secPtr++;
                ptr = 0;
                if ((secPtr % 8) == 0)
                {
                    byte emptyCluster = 0xff;
                    for (int i = 0; i < 0x9f; i++)
                    {
                        if (FAT[i] != 0xff) continue;
                        emptyCluster = (byte)i;
                        FAT[i] = 0xc1;
                        break;
                    }

                    if (emptyCluster == 0xff) throw new FileNotFoundException("file over");

                    FAT[cls] = emptyCluster;
                    cls = emptyCluster;

                    saf = (cls % 4) / 2;//サーフェス
                    trkPtr = cls / 4 * 2 + saf;//d88はsafが0/1をくりかえす
                    secPtr = (cls % 2) * 8;//d88はsectorを0から数える
                    ptr = 0;
                }
            } while (true);

            //FATに最後のセクター値をセット
            FAT[cls] = (byte)(0xc1 + secPtr);
            //FATの書き出し
            SetFAT();
        }


        private N88FileAtr SetFile_NewFile(string fn, int mode)
        {
            //ファイル名をbyte配列に変換
            if (fn.IndexOf(".") >= 0)
            {
                if (fn.IndexOf(".") < 6)
                {
                    fn = fn.Substring(0, fn.IndexOf(".")) + "     ".Substring(0, 6 - fn.IndexOf(".")) + "." + fn.Substring(fn.IndexOf(".") + 1);
                }
            }
            fn = fn.Replace(".", "");
            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            byte[] data = System.Text.Encoding.GetEncoding("shift_jis").GetBytes(fn);

            //アトリビュートの作成
            byte[] atrb = new byte[16];
            for (int i = 0; i < 9; i++)
            {
                if (i < data.Length) atrb[i] = data[i];
                else atrb[i] = 0x20;
            }
            for (int i = 0; i < 5; i++) atrb[i + 11] = 0xff;
            atrb[9] = (byte)(
                mode == 0 ? 0x1 :
                ((mode == 1 || mode == 2) ? 0x0 :
                (mode == 3 ? 0x80 :
                0x1)));//bit 7 非アスキー形式  6 リードアフターライト  5 Pオプションファイル  4 ライトプロテクション  0 機械語ファイル
            byte emptyCluster = 0;
            for (int i = 0; i < 0x9f; i++)
            {
                if (FAT[i] != 0xff) continue;
                emptyCluster = (byte)i;
                FAT[i] = 0xc1;
                break;
            }
            atrb[10] = emptyCluster;

            return SetDirectories(atrb);
        }

        private string getFn(string fn)
        {
            string da = fn.Replace("*", ".");
            if (da.IndexOf(".") < 0)
            {
                if (da.Length < 7)
                    da += ".";
                else
                    da = da.Substring(0, 6) + "." + da.Substring(6);
            }
            da = da.Substring(0, da.IndexOf(".")).TrimEnd() + "." + da.Substring(da.IndexOf(".") + 1).TrimEnd();
            return da;
        }



        /// <summary>
        /// 中間ファイル -> 生テキスト
        /// </summary>
        private List<byte> ConvertPlaneBasic(List<byte> buf)
        {
            int ptr = 0;
            List<byte> dBuf = new List<byte>();

            do
            {
                int linkPtr = buf[ptr] + buf[ptr + 1] * 0x100;
                //Console.WriteLine("linkPtr:{0}",linkPtr);
                if (linkPtr == 0) break;
                if (ptr >= buf.Count) break;
                ptr += 2;
                if (ptr >= buf.Count) break;
                int lineNum = buf[ptr] + buf[ptr + 1] * 0x100;
                ptr += 2;
                if (ptr >= buf.Count) break;

                //行番号を出力
                dBuf.AddRange(Encoding.GetEncoding("shift_jis").GetBytes(string.Format("{0} ", lineNum)));
                bool ssw = false;

                do
                {
                    if (ptr == buf.Count) break;
                    byte c = buf[ptr++];

                    if (c == 0)
                    {
                        dBuf.Add(0x0d); dBuf.Add(0x0a);
                        break;
                    }
                    else if (c >= 0x01 && c <= 0x10)
                        ptr = ConvertPlaneBasic_suuji(buf, ptr, dBuf, c);
                    else if (c >= 0x11 && c <= 0x1a)
                        dBuf.Add((byte)('0' + c - 0x11));
                    else if (c == 0x1b)
                    {
                        dBuf.Add((byte)'1');
                        dBuf.Add((byte)'0');
                    }
                    else if (c == 0x1c)
                    {
                        string s = string.Format("{0}", buf[ptr] + buf[ptr + 1] * 0x100);
                        ptr += 2;
                        dBuf.AddRange(Encoding.GetEncoding("shift_jis").GetBytes(s));
                    }
                    else if (c >= 0x1d && c <= 0x1f)
                    {
                        ;
                    }
                    else if (c >= 0x20 && c <= 0x7F)
                        ConvertPlaneBasic_koronKei(buf, ref ptr, dBuf, ref ssw, c);
                    else if (c >= 0x80 && c <= 0xfe)
                        ConvertPlaneBasic_objectCodeTable(dBuf, ssw, c);
                    else if (c == 0xff)
                        ConvertPlaneBasic_objectCodeTableFF(buf, ref ptr, dBuf, ssw, c);
                } while (true);

            } while (ptr < buf.Count);
            dBuf.Add(0x0d); dBuf.Add(0x0a);

            return dBuf;
        }

        private int ConvertPlaneBasic_suuji(List<byte> buf, int ptr, List<byte> dBuf, byte c)
        {
            if (c == 0xb)
            {
                //8進数
                string s = string.Format("&O{0:O}", buf[ptr] + buf[ptr + 1] * 0x100);
                ptr += 2;
                dBuf.AddRange(Encoding.GetEncoding("shift_jis").GetBytes(s));
            }
            else if (c == 0xc)
            {
                //16進数
                string s = string.Format("&H{0:X}", buf[ptr] + buf[ptr + 1] * 0x100);
                ptr += 2;
                dBuf.AddRange(Encoding.GetEncoding("shift_jis").GetBytes(s));
            }
            else if (c == 0xd || c == 0xe)
            {
                //2byte 数値
                string s = string.Format("{0}", buf[ptr] + buf[ptr + 1] * 0x100);
                ptr += 2;
                dBuf.AddRange(Encoding.GetEncoding("shift_jis").GetBytes(s));
            }
            else if (c == 0xf)
            {
                //1byte 数値
                string s = string.Format("{0}", buf[ptr]);
                ptr++;
                dBuf.AddRange(Encoding.GetEncoding("shift_jis").GetBytes(s));
            }
            else
            {
                //単精度、倍精度(TBD)
                ;
            }

            return ptr;
        }

        private void ConvertPlaneBasic_koronKei(List<byte> buf, ref int ptr, List<byte> dBuf, ref bool ssw, byte c)
        {
            if (c == 0x22)
            {
                dBuf.Add(c);
                ssw = !ssw;
            }
            else if (c == (byte)':')
            {
                if (buf[ptr] == 0x8f && buf[ptr + 1] == 0xe9)
                {
                    dBuf.Add((byte)'\'');
                    ptr += 2;
                }
                else
                {
                    dBuf.Add(c);
                }
            }
            else
            {
                dBuf.Add(c);
            }
        }

        private void ConvertPlaneBasic_objectCodeTable(List<byte> dBuf, bool ssw, byte c)
        {
            if (ssw) dBuf.Add(c);
            else
            {
                //中間コード
                string s = codeTable[c - 0x80];
                dBuf.AddRange(Encoding.GetEncoding("shift_jis").GetBytes(s));
            }
        }

        private void ConvertPlaneBasic_objectCodeTableFF(List<byte> buf, ref int ptr, List<byte> dBuf, bool ssw, byte c)
        {
            if (ssw) dBuf.Add(c);
            else
            {
                c = buf[ptr++];
                //中間コード2nd
                string s = codeTableFF[c - 0x80];
                dBuf.AddRange(Encoding.GetEncoding("shift_jis").GetBytes(s));
            }
        }



        /// <summary>
        /// 生テキスト -> 中間ファイル
        /// </summary>
        private byte[] ConvertObjectBasic(string[] lines)
        {
            List<byte> ret = new List<byte>();
            int linkPtr = 0;

            int n;

            for (int row = 0; row < lines.Length; row++)
            {
                byte[] ln = Encoding.GetEncoding("shift_jis").GetBytes(lines[row]);
                int col = 0;
                bool nonpress = false;
                bool nonpress2 = false;
                bool lineSw = false;//行番号フラグ


                ret.Add(0); ret.Add(0);//dummyのリンクポインタ

                //行番号
                List<byte> s = new List<byte>();
                while (ln[col] != ' ')
                {
                    s.Add(ln[col]);
                    col++;//タブは使いません
                }
                n = int.Parse(Encoding.GetEncoding("shift_jis").GetString(s.ToArray()));
                ret.Add((byte)n); ret.Add((byte)(n >> 8));


                //空白飛ばし
                while (ln[col] == ' ' && col < ln.Length) col++;
                //if (col >= ln.Length) break;

                do
                {
                    //命令探し
                    int size = 9;//最長のコマンド RANDOMIZE は9文字
                    bool fnd = false;
                    int cn = 0;

                    if (!nonpress && !nonpress2) fnd = SearchCommand(ln, col, ref size, ref cn);

                    //命令見つけた場合
                    if (fnd) lineSw = ConvertObjectBasic_foundCmd(ret, ln, ref col, ref nonpress, ref size, cn);
                    else ConvertObjectBasic_notFoundCmd(ret, ln, ref col, nonpress, ref nonpress2, ref lineSw);

                } while (col < ln.Length);

                ret.Add(0); //1行終わったマーク

                //リンクポインタ更新
                //if (row + 1 != lines.Length)
                {
                    n = ret.Count + 1;
                    ret[linkPtr] = (byte)n;
                    ret[linkPtr + 1] = (byte)(n >> 8);
                    linkPtr = ret.Count;
                }
            }

            return ret.ToArray();
        }

        private bool SearchCommand(byte[] ln, int col, ref int size, ref int cn)
        {
            do
            {
                int c = 0;
                List<byte> s = new List<byte>();
                while (c < size && col + c < ln.Length)
                {
                    s.Add(ln[col + c]);
                    c++;
                }

                if (s.Count < size)
                {
                    size--;
                    continue;
                }

                string ss = Encoding.GetEncoding("shift_jis").GetString(s.ToArray());
                //Console.WriteLine(ss);

                cn = 0;
                foreach (string code in codeTable)
                {
                    if (ss != code)
                    {
                        cn++;
                        continue;
                    }
                    return true;
                }

                cn = 1000;
                foreach (string code in codeTableFF)
                {
                    if (ss != code)
                    {
                        cn++;
                        continue;
                    }
                    return true;
                }

                size--;
            } while (size > 0);

            return false;
        }

        private bool ConvertObjectBasic_foundCmd(List<byte> ret, byte[] ln, ref int col, ref bool nonpress, ref int size, int cn)
        {
            bool lineSw = false;
            if (
                cn == 0x9 // goto
                || cn == 0xd // gosub
                || cn == 0x5d//then
                || cn == 0x1f//else
                || cn == 0xc//restore
                )
            {
                lineSw = true;
                ret.Add((byte)(cn + 0x80));
            }
            else if (cn == 0x75)
            {
                col++;
                ret.Add(0xf5);
                while ((ln[col] >= 'A' && ln[col] <= 'Z') || (ln[col] >= '0' && ln[col] <= '9'))
                {
                    ret.Add((byte)ln[col++]);
                    if (ln.Length == col) break;
                }
                size = 0;
            }
            else if (cn >= 1000)
            {
                ret.Add(0xff);
                ret.Add((byte)(cn - 1000 + 0x80));
            }
            else if (cn + 0x80 != 0xe9) ret.Add((byte)(cn + 0x80));
            else
            {
                ret.Add((byte)0x3a);
                ret.Add((byte)0x8f);
                ret.Add((byte)0xe9);
                nonpress = true;
            }

            col += size;
            if (cn == 0xf) nonpress = true;

            return lineSw;
        }

        private void ConvertObjectBasic_notFoundCmd(List<byte> ret, byte[] ln, ref int col, bool nonpress, ref bool nonpress2, ref bool lineSw)
        {
            if (nonpress)
            {
                ret.Add((byte)ln[col++]);
                return;
            }

            if (nonpress2)
            {
                if (ln[col] == '"') nonpress2 = !nonpress2;
                ret.Add((byte)ln[col++]);
                return;
            }

            if (ln[col] == '&' && col + 1 < ln.Length)
            {
                if (ln[col + 1] == 'O')
                {
                    //8進数？
                    col += 2;
                    if (!GetOct(ret, ln, ref col))
                    {
                        ret.Add((byte)'&');
                        ret.Add((byte)'O');
                    }
                    return;
                }

                if (ln[col + 1] == 'H')
                {
                    //16進数？
                    col += 2;
                    if (!GetHex(ret, ln, ref col))
                    {
                        ret.Add((byte)'&');
                        ret.Add((byte)'H');
                    }
                    return;
                }

                //只の&でした
                ret.Add((byte)'&');
                col++;
                return;
            }

            if (ln[col] >= '0' && ln[col] <= '9')
            {
                //数字の何か？
                if (!GetDec(ret, ln, ref col, lineSw)) ret.Add((byte)ln[col++]);
                lineSw = false;
                return;
            }

            if (ln[col] == '"') nonpress2 = !nonpress2;
            ret.Add((byte)ln[col++]);
        }



        private bool GetOct(List<byte> ret, byte[] ln, ref int col)
        {
            List<byte> s = new List<byte>();

            if (ln == null || col >= ln.Length) return false;
            while (ln[col] >= '0' && ln[col] <= '8')
            {
                s.Add(ln[col]);
                col++;
                if (col >= ln.Length) break;
            }
            if (s.Count == 0) return false;

            int n = Convert.ToInt32(Encoding.GetEncoding("shift_jis").GetString(s.ToArray()), 8);

            ret.Add((byte)0xb);
            ret.Add((byte)n); ret.Add((byte)(n >> 8));

            return true;
        }

        private bool GetHex(List<byte> ret, byte[] ln, ref int col)
        {
            List<byte> s = new List<byte>();

            if (ln == null || col >= ln.Length) return false;
            while ((ln[col] >= '0' && ln[col] <= '9') || (ln[col] >= 'A' && ln[col] <= 'F'))
            {
                s.Add(ln[col]);
                col++;
                if (col >= ln.Length) break;
            }
            if (s.Count == 0) return false;

            int n = Convert.ToInt32(Encoding.GetEncoding("shift_jis").GetString(s.ToArray()), 16);

            ret.Add((byte)0xc);
            ret.Add((byte)n); ret.Add((byte)(n >> 8));

            return true;
        }

        private bool GetDec(List<byte> ret, byte[] ln, ref int col, bool lineSw)
        {
            List<byte> s = new List<byte>();

            if (ln == null || col >= ln.Length) return false;
            while (ln[col] >= '0' && ln[col] <= '9')
            {
                s.Add(ln[col]);
                col++;
                if (col >= ln.Length) break;
            }
            if (s.Count == 0) return false;

            int n = Convert.ToInt32(Encoding.GetEncoding("shift_jis").GetString(s.ToArray()), 10);

            if (!lineSw)
            {
                if (n < 10)
                {
                    ret.Add((byte)(0x11 + n));
                }
                else if (n < 256)
                {
                    ret.Add((byte)0xf);
                    ret.Add((byte)n);
                }
                else
                {
                    ret.Add((byte)0x1c);
                    ret.Add((byte)n); ret.Add((byte)(n >> 8));
                }
            }
            else
            {
                ret.Add((byte)0x0e);
                ret.Add((byte)n); ret.Add((byte)(n >> 8));
            }

            return true;
        }
    }
}
