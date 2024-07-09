using System;
using System.Collections.Generic;
using System.Text;
using musicDriverInterface;

namespace Core
{
    public class partWork
    {

        /// <summary>
        /// エイリアスデータ
        /// </summary>
        public Dictionary<string, Line> aData = null;

        /// <summary>
        /// パートページ
        /// </summary>
        public List<partPage> pg = null;

        /// <summary>
        /// 共有ページ
        /// </summary>
        public partPage spg = new partPage(null, null);

        /// <summary>
        /// 現在作業中のページ(アクティブページ)
        /// </summary>
        public partPage apg;

        /// <summary>
        /// 現在演奏権を持っているページ(カレントページ)
        /// </summary>
        public partPage cpg;

        public long clockCounter { get; internal set; }

        /// <summary>
        /// スロットごとの固定fnum値
        /// </summary>
        public int[] slotFixedFnum = new int[] { -1, -1, -1, -1 };


        /// <summary>
        /// エイリアス間の移動があったかを示すフラグをクリアする
        /// </summary>
        public bool doMoveAlies = false;
        public int doMoveAliesCnt = 0;

        /// <summary>
        /// パート情報をリセットする
        /// </summary>
        public void resetPos(partPage page)
        {
            page.pos = new clsPos();
            page.stackPos = new Stack<clsPos>();
        }

        /// <summary>
        /// 解析位置を取得する
        /// </summary>
        /// <returns></returns>
        public int getPos(partPage page)
        {
            return page.pos.tCol;
        }

        public Line getLine(partPage page)
        {
            if (page.pos.alies == null || page.pos.alies.Length < 1)
            {
                return page.pData[page.pos.row].Copy();
            }
            return aData[page.pos.alies[0].aliesNextName].Copy();
        }

        /// <summary>
        /// 解析位置に対するソースファイル上の行数を得る
        /// </summary>
        /// <returns></returns>
        public int getLineNumber(int page)
        {
            if (pg[page].pos.alies == null || pg[page].pos.alies.Length < 1)
            {
                return pg[page].pData[pg[page].pos.row].Lp.row;
            }
            return aData[pg[page].pos.alies[0].aliesNextName].Lp.row;
        }

        /// <summary>
        /// 解析位置に対するソースファイル名を得る
        /// </summary>
        /// <returns></returns>
        public string getSrcFn(int page)
        {
            if (pg[page].pos.alies == null || pg[page].pos.alies.Length < 1)
            {
                return pg[page].pData[pg[page].pos.row].Lp.srcMMLID;
            }
            return aData[pg[page].pos.alies[0].aliesNextName].Lp.srcMMLID;
        }

        /// <summary>
        /// 解析位置の文字を取得する
        /// </summary>
        /// <returns></returns>
        public char getChar(partPage page)
        {
            //if (dataEnd) return (char)0;

            char ch;
            if (page.pos.alies == null || page.pos.alies.Length<1)
            {
                if (page.pData[page.pos.row].Txt.Length <= page.pos.col + page.pData[page.pos.row].Lp.col)
                {
                    return (char)0;
                }
                ch = page.pData[page.pos.row].Txt[page.pos.col + page.pData[page.pos.row].Lp.col];
            }
            else
            {
                if (aData[page.pos.alies[0].aliesNextName].Txt.Length <= page.pos.col + aData[page.pos.alies[0].aliesNextName].Lp.col)
                {
                    return (char)0;
                }
                ch = aData[page.pos.alies[0].aliesNextName].Txt[page.pos.col + aData[page.pos.alies[0].aliesNextName].Lp.col];
            }
            //Console.Write(ch);
            return ch;
        }

        /// <summary>
        /// エイリアス間の移動があったかを示すフラグをクリアする
        /// </summary>
        public void ClearMoveAlies()
        {
            doMoveAlies = false;
            doMoveAliesCnt = 0;
        }

        /// <summary>
        /// 解析位置を一つ進める(重い！)
        /// </summary>
        public void incPos(partPage page)
        {
            setPos(page, page.pos.tCol + 1);
        }

        /// <summary>
        /// 解析位置を一つ戻す(重い！)
        /// </summary>
        public void decPos(partPage page)
        {
            setPos(page, page.pos.tCol - 1);
        }

        public void decPos(partPage page, int n)
        {
            setPos(page, page.pos.tCol - n);
        }

        /// <summary>
        /// 指定された文字数だけ読み出し、文字列を生成する
        /// </summary>
        /// <param name="len">文字数</param>
        /// <returns>文字列</returns>
        public string getString(partPage page, int len)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < len; i++)
            {
                sb.Append(getChar(page));
                incPos(page);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 解析位置を指定する
        /// </summary>
        /// <param name="tCol">解析位置</param>
        public void setPos(partPage page, int tCol)
        {
            if (page.pData == null)
            {
                return;
            }

            if (page.LstPos == null) MakeLstPos(page);

            int i = 0;
            while (i != page.LstPos.Count && tCol >= page.LstPos[i].tCol)
            {
                i++;
            }

            page.pos.tCol = tCol;
            if (page.pos.alies != page.LstPos[i - 1].alies)
                page.pw.doMoveAlies = true;
            else if (!page.pw.doMoveAlies) page.pw.doMoveAliesCnt++;
            page.pos.alies = page.LstPos[i - 1].alies;
            page.pos.col = page.LstPos[i - 1].col + tCol - page.LstPos[i - 1].tCol;
            page.pos.row = page.LstPos[i - 1].row;
            return;

        }

        //private List<clsPos> LstPos = null;

        //public int pcmMapNo { get; set; } = 0;
        //public int dutyCycle { get; set; } = 0;
        //public int oldDutyCycle { get; set; } = -1;
        //public int oldFreq { get; set; } = -1;

        //public bool directModeVib { get; set; }
        //public bool directModeTre { get; set; }
        //public int pitchBend { get; set; } = 0;
        //public int tieBend { get; set; } = 0;
        //public int portaBend { get; set; } = 0;
        //public int lfoBend { get; set; } = 0;
        //public int bendStartOctave { get; set; } = 0;
        //public char bendStartNote { get; set; } = 'c';
        //public int bendStartShift { get; set; } = 0;
        //public int velocity { get; set; } = 110;
        //public int effectDistortionSwitch { get; internal set; } = 0;
        //public int effectDistortionVolume { get; internal set; } = 32;
        //public bool isOp4Mode { get; internal set; } = false;
        //public int beforeBendNoteNum { get; internal set; } = -1;
        //public int panRL { get; internal set; }
        //public int panRR { get; internal set; }
        //public int flag { get; internal set; }
        //public bool changeFlag { get; internal set; }
        //public int C352flag { get; internal set; }

        public void MakeLstPos(partPage page)
        {
            if (page.pData == null)
            {
                return;
            }

            int tCol = 0;
            int row = 0;
            int col = 0;
            int pCol = 0;
            string aliesName = "";
            //int aliesDepth = 0;

            page.LstPos = new List<clsPos>();
            page.LstPos.Add(new clsPos());
            page.stackAliesPos = new Stack<LinePos>();
            resetPos(page);

            while (true)
            {
                string data;
                char ch;

                //読みだすデータの頭出し
                if (aliesName == "")
                {
                    if (page.pData.Count == row)
                    {
                        return;
                    }
                    data = page.pData[row].Txt;
                    pCol = page.pData[row].Lp.col;
                }
                else
                {
                    data = aData[aliesName].Txt;
                    pCol = aData[aliesName].Lp.col;
                }

                data = Common.CutComment(data);

                //解析行の解析位置が終端に達したときの処理
                while (data.Length == col + pCol)
                {
                    if (aliesName == "")
                    {
                        row++;
                        if (page.pData.Count == row)
                            goto Exit;
                        else
                        {
                            data = page.pData[row].Txt;
                            data = Common.CutComment(data);
                            pCol = page.pData[row].Lp.col;
                            col = 0;// pData[row].Lp.col;

                            clsPos p = new clsPos();
                            p.tCol = tCol;
                            p.alies = null;
                            p.col = 0;
                            p.row = row;
                            page.LstPos.Add(p);

                            continue;
                        }
                    }
                    else
                    {
                        clsPos p = page.stackPos.Pop();
                        aliesName = p.alies[0].aliesName;
                        page.stackAliesPos.Pop();
                        col = p.col;
                        row = p.row;
                        if (aliesName == "")
                        {
                            data = page.pData[row].Txt;
                            pCol = page.pData[row].Lp.col;
                        }
                        else
                        {
                            data = aData[aliesName].Txt;
                            pCol = aData[aliesName].Lp.col;
                        }

                        data = Common.CutComment(data);
                        p.tCol = tCol;
                        page.LstPos.Add(p);
                    }
                }

                ch = data[col + pCol];

                //解析位置でエイリアス指定されている場合
                while (ch == '%')
                {
                    string a = getAliesName(data, col + pCol);
                    if (a != "")
                    {
                        LinePos ali = new LinePos();
                        ali.aliesName = aliesName;//現在のエイリアス名
                        ali.aliesNextName = a;//飛び先となるエイリアス名
                        ali.aliesDepth = page.stackAliesPos.Count+1;
                        //ali.nextDepth = page.stackAliesPos.Count + 1;
                        if (aliesName == "") ali.row = page.pData[row].Lp.row;
                        else ali.row = aData[aliesName].Lp.row;
                        ali.col = col + pCol;
                        ali.length = a.Length + 1;

                        //マクロから復帰する際に使用する場所情報を作成
                        clsPos p = new clsPos();
                        p.col = col + a.Length + 1;
                        p.row = row;
                        p.alies = CopyAndToArrayStackAliesPos(page.stackAliesPos); //現時点のスタックの内容に戻す

                        page.stackPos.Push(p);
                        page.stackAliesPos.Push(ali);

                        data = aData[a].Txt;
                        data = Common.CutComment(data);
                        pCol = aData[a].Lp.col;
                        col = 0;
                        aliesName = a;
                        row = 0;

                        p = new clsPos();
                        p.tCol = tCol;
                        p.alies = CopyAndToArrayStackAliesPos(page.stackAliesPos);
                        p.col = 0;
                        p.row = 0;
                        page.LstPos.Add(p);
                    }
                    else
                    {
                        msgBox.setWrnMsg(msg.get("E06000")
                            , (aliesName == "") ? page.pData[row].Lp : aData[aliesName].Lp
                            );
                        col++;
                    }

                    ch = data[col + pCol];
                }

                tCol++;
                col++;
                //解析行の解析位置が終端に達したときの処理
                while (data.Length == col + pCol)// || data[col + pCol] == ';')
                {
                    if (aliesName == "")
                    {
                        row++;
                        if (page.pData.Count == row)
                            goto Exit;
                        else
                        {
                            data = page.pData[row].Txt;
                            data = Common.CutComment(data);
                            pCol = page.pData[row].Lp.col;
                            col = 0;

                            clsPos p = new clsPos();
                            p.tCol = tCol;
                            p.alies = null;
                            p.col = 0;
                            p.row = row;
                            page.LstPos.Add(p);

                            continue;
                        }
                    }
                    else
                    {
                        clsPos p = page.stackPos.Pop();
                        aliesName = p.alies.Length < 1 ? "" : p.alies[0].aliesNextName;
                        page.stackAliesPos.Pop();
                        col = p.col;
                        row = p.row;
                        if (aliesName == "")
                        {
                            data = page.pData[row].Txt;
                            data = Common.CutComment(data);
                            pCol = page.pData[row].Lp.col;
                        }
                        else
                        {
                            data = aData[aliesName].Txt;
                            data = Common.CutComment(data);
                            pCol = aData[aliesName].Lp.col;
                        }

                        p.tCol = tCol;
                        page.LstPos.Add(p);
                    }
                }

            }
        Exit:;

        }

        private LinePos[] CopyAndToArrayStackAliesPos(Stack<LinePos> stackAliesPos)
        {
            List<LinePos> ret = new List<LinePos>();

            LinePos[] saps = stackAliesPos.ToArray();
            foreach (LinePos sap in saps)
            {
                LinePos ap = LinePos.Copy(sap);
                ret.Add(ap);
            }

            return ret.ToArray();
        }

        /// <summary>
        /// タブと空白は読み飛ばす
        /// </summary>
        /// <returns>飛ばした文字数</returns>
        public int skipTabSpace(partPage page)
        {
            int cnt = 0;
            while (getChar(page) == ' ' || getChar(page) == '\t')
            {
                incPos(page);
                cnt++;
            }
            return cnt;
        }


        /// <summary>
        /// 解析位置から数値を取得する。
        /// タブと空白は読み飛ばす。
        /// +-符号を取得する(ない場合は正とする)
        /// 16進数($hh:2文字必ずいる)読み取り可。
        /// </summary>
        /// <param name="num">取得した数値が返却される</param>
        /// <returns>数値取得成功したかどうか</returns>
        public bool getNum(partPage page, out int num)
        {

            string n = "";
            int ret = -1;

            skipTabSpace(page);

            //+-符号を取得する(ない場合は正とする)
            if (getChar(page) == '-' || getChar(page) == '+')
            {
                n = getChar(page).ToString();
                incPos(page);
            }

            skipTabSpace(page);

            //１６進数指定されているか
            if (getChar(page) != '$')
            {
                //数字でなくなるまで取得
                while (true)
                {
                    if (getChar(page) >= '0' && getChar(page) <= '9')
                    {
                        try
                        {
                            n += getChar(page);
                            incPos(page);
                        }
                        catch
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                //数値に変換できたら成功
                if (!int.TryParse(n, out ret))
                {
                    num = -1;
                    return false;
                }

                num = ret;
            }
            else
            {
                //２文字取得
                incPos(page);
                n += getChar(page);
                incPos(page);
                n += getChar(page);
                incPos(page);
                //数値に変換できたら成功
                try
                {
                    num = Convert.ToInt32(n, 16);
                }
                catch
                {
                    num = -1;
                    return false;
                }
            }

            return true;
        }

        public bool getNum(partPage page, out int num, ref int col,ref int kcol)
        {

            string n = "";
            int ret = -1;

            col += skipTabSpace(page);

            //符号を取得する(ない場合は正とする)
            if (getChar(page) == '-' || getChar(page) == '+')
            {
                n = getChar(page).ToString();
                incPos(page);
                col++;
                kcol = col;
            }

            col += skipTabSpace(page);

            //１６進数指定されているか
            if (getChar(page) != '$')
            {
                //数字でなくなるまで取得
                while (true)
                {
                    if (getChar(page) >= '0' && getChar(page) <= '9')
                    {
                        try
                        {
                            n += getChar(page);
                            incPos(page);
                            col++;
                        }
                        catch
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                //数値に変換できたら成功
                if (!int.TryParse(n, out ret))
                {
                    num = -1;
                    return false;
                }

                kcol = col;
                num = ret;
            }
            else
            {
                //２文字取得
                incPos(page);
                col++;
                n += getChar(page);
                incPos(page);
                col++;
                n += getChar(page);
                incPos(page);
                col++;
                //数値に変換できたら成功
                try
                {
                    num = Convert.ToInt32(n, 16);
                    kcol = col;
                }
                catch
                {
                    num = -1;
                    return false;
                }
            }

            return true;
        }

        public bool getNumInt16(partPage page, out int num)
        {

            string n = "";
            int ret;// = -1;

            skipTabSpace(page);

            //+-符号を取得する(ない場合は正とする)
            if (getChar(page) == '-' || getChar(page) == '+')
            {
                n = getChar(page).ToString();
                incPos(page);
            }

            skipTabSpace(page);

            //１６進数指定されているか
            if (getChar(page) != '$')
            {
                //数字でなくなるまで取得
                while (true)
                {
                    if (getChar(page) >= '0' && getChar(page) <= '9')
                    {
                        try
                        {
                            n += getChar(page);
                            incPos(page);
                        }
                        catch
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                //数値に変換できたら成功
                if (!int.TryParse(n, out ret))
                {
                    num = -1;
                    return false;
                }

                num = ret;
            }
            else
            {
                //4文字取得
                incPos(page);
                n += getChar(page);
                incPos(page);
                n += getChar(page);
                incPos(page);
                n += getChar(page);
                incPos(page);
                n += getChar(page);
                incPos(page);
                //数値に変換できたら成功
                try
                {
                    num = Convert.ToInt32(n, 16);
                }
                catch
                {
                    num = -1;
                    return false;
                }
            }

            return true;
        }

        public bool getNumNoteLength(partPage page, out int num, out bool flg)
        {

            flg = false;

            skipTabSpace(page);

            //クロック直接指定
            if (getChar(page) == '#')
            {
                flg = true;
                incPos(page);
            }

            return getNum(page, out num);
        }

        public bool getNumNoteLength(partPage page, out int num, out bool flg, out int col,out int kcol)
        {

            flg = false;
            col = 0;
            kcol = 0;

            col += skipTabSpace(page);

            //クロック直接指定
            if (getChar(page) == '#')
            {
                flg = true;
                incPos(page);
                col++;
                kcol = col;
            }

            return getNum(page, out num, ref col,ref kcol);
        }

        /// <summary>
        /// エイリアス名を取得する
        /// </summary>
        /// <param name="data"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        private string getAliesName(string data, int col)
        {
            if (data.Length <= col + 1)
            {
                return "";
            }

            string wrd = data.Substring(col + 1);
            while (!aData.ContainsKey(wrd))
            {
                if (wrd.Length == 1)
                {
                    return "";
                }
                wrd = wrd.Substring(0, wrd.Length - 1);
            }
            return wrd;
        }

        public partWork()
        {
            spg.sharedPageInitializer();
        }
    }

    public class clsPos
    {

        /// <summary>
        /// すべてのデータ行を１行としたときの次に演奏されるデータの何桁目か
        /// </summary>
        public int tCol = 0;

        /// <summary>
        /// 次に演奏されるデータの何行目か
        /// </summary>
        public int row = 0;

        /// <summary>
        /// 次に演奏されるデータの何桁目か
        /// </summary>
        public int col = 0;

        public LinePos[] alies = null;
        ///// <summary>
        ///// 次に演奏されるデータのエイリアス名
        ///// </summary>
        //public string alies = "";

        //public int aliesDepth = 0;

        //public int aliesRow { get; internal set; }
        //public int aliesCol { get; internal set; }
        //public int aliesLen { get; internal set; }
    }

    //public class clsAliesPos
    //{
    //    public string name = "";
    //    public string nextName = "";
    //    public int depth = 0;
    //    public int row = -1;
    //    public int col = -1;
    //    public int len = -1;
    //}

    public class clsRepeat
    {
        /// <summary>
        /// 位置
        /// </summary>
        public int pos = 0;

        /// <summary>
        /// リピート向け回数
        /// </summary>
        public int repeatCount = 2;

    }

    public class clsRenpu
    {
        /// <summary>
        /// 位置
        /// </summary>
        public int pos = 0;

        /// <summary>
        /// リピートのスタック数
        /// </summary>
        public int repeatStackCount = 0;

        /// <summary>
        /// ノートの数
        /// </summary>
        public int noteCount = 0;

        public MML mml;

        public List<int> lstRenpuLength;
    }

    public class clsLfo
    {

        /// <summary>
        /// Lfoの種類
        /// </summary>
        public eLfoType type = eLfoType.unknown;

        /// <summary>
        /// Lfoの設定値
        /// </summary>
        public List<int> param = null;

        /// <summary>
        /// Lfoのスイッチ
        /// </summary>
        public bool sw = false;

        /// <summary>
        /// Lfoが完了したかどうか
        /// </summary>
        public bool isEnd = false;

        /// <summary>
        /// Lfoの待機カウンター
        /// </summary>
        public long waitCounter = 0;

        /// <summary>
        /// Lfoの変化値
        /// </summary>
        public int value = 0;

        /// <summary>
        /// Lfoの変化する方向
        /// </summary>
        public int direction = 0;

        public int depthWaitCounter { get; internal set; }
        public int depth { get; internal set; }
        public int depthV2 { get; internal set; }

        public int slot = 0;
        public int phase = 0;
    }

    public class clsPcm
    {
        public enmChipType chip = enmChipType.YM2612;
        public int chipNumber = 0;
        public int num = 0;
        public int seqNum = 0;
        public double xgmMaxSampleCount = 0;
        public string fileName = "";
        public int freq = 0;
        public int vol = 0;
        public long stAdr = 0;
        public long edAdr = 0;
        public long size = 0;
        public long loopAdr = -1;
        public bool is16bit = false;
        public int samplerate = 8000;
        public object[] option = null;
        public enmPCMSTATUS status = enmPCMSTATUS.NONE;

        public clsPcm(int num
            , int seqNum
            , enmChipType chip
            , int chipNumber
            , string fileName
            , int freq
            , int vol
            , long stAdr
            , long edAdr
            , long size
            , long loopAdr
            , bool is16bit
            , int samplerate
            , params object[] option)
        {
            this.num = num;
            this.seqNum = seqNum;
            this.chip = chip;
            this.chipNumber = chipNumber;
            this.fileName = fileName;
            this.freq = freq;
            this.vol = vol;
            this.stAdr = stAdr;
            this.edAdr = edAdr;
            this.size = size;
            this.loopAdr = loopAdr;
            this.is16bit = is16bit;
            this.samplerate = samplerate;
            this.option = option;
            this.status = enmPCMSTATUS.NONE;
        }
    }

    public class clsPcmDatSeq
    {
        public enmPcmDefineType type;
        public int No = -1;
        public string FileName = "";
        public int BaseFreq = 8000;
        public int Volume = 100;
        public enmChipType chip = enmChipType.YM2612;
        public int chipNumber = 0;
        public object[] Option = null;
        public int SrcStartAdr = 0;
        public int DesStartAdr = 0;
        public int SrcLength = 0;
        public int DatStartAdr = 0;
        public int DatEndAdr = 0;
        public int DatLoopAdr = 0;

        public clsPcmDatSeq(
            enmPcmDefineType type
            , int No
            , string FileName
            , int BaseFreq
            , int Volume
            , enmChipType chip
            , int chipNumber
            , int LoopAdr
            , params object[] option)
        {
            this.type = type;
            this.No = No;
            this.FileName = FileName;
            this.BaseFreq = BaseFreq;
            this.Volume = Volume;
            this.chip = chip;
            this.chipNumber = chipNumber;
            this.DatLoopAdr = LoopAdr;
            this.Option = option;
        }

        public clsPcmDatSeq(
            enmPcmDefineType type
            , string FileName
            , enmChipType chip
            , int chipNumber
            , int SrcStartAdr
            , int DesStartAdr
            , int Length
            , object[] Option)
        {
            this.type = type;
            this.FileName = FileName;
            this.chip = chip;
            this.chipNumber = chipNumber;
            this.SrcStartAdr = SrcStartAdr;
            this.DesStartAdr = DesStartAdr;
            this.SrcLength = Length;
            this.Option = Option;
            if (chip == enmChipType.YM2610B)
            {
                if (Option != null)
                {
                    this.DatLoopAdr = Option.ToString() == "0" ? 0 : 1;
                }
                else
                {
                    this.DatLoopAdr = 0;
                }
                ;
            }
        }

        public clsPcmDatSeq(
            enmPcmDefineType type
            , int No
            , enmChipType chip
            , int chipNumber
            , int BaseFreq
            , int DatStartAdr
            , int DatEndAdr
            , int DatLoopAdr
            , object[] Option)
        {
            this.type = type;
            this.No = No;
            this.chip = chip;
            this.chipNumber = chipNumber;
            this.BaseFreq = BaseFreq;
            this.DatStartAdr = DatStartAdr;
            this.DatEndAdr = DatEndAdr;
            this.DatLoopAdr = DatLoopAdr;
            this.Option = Option;
        }
    }

    public class clsToneDoubler
    {
        public int num = 0;
        public List<clsTD> lstTD = null;

        public clsToneDoubler(int num, List<clsTD> lstTD)
        {
            this.num = num;
            this.lstTD = lstTD;
        }
    }

    public class clsTD
    {
        public int OP1ML = 0;
        public int OP2ML = 0;
        public int OP3ML = 0;
        public int OP4ML = 0;
        public int OP1DT2 = 0;
        public int OP2DT2 = 0;
        public int OP3DT2 = 0;
        public int OP4DT2 = 0;
        public int KeyShift = 0;

        public clsTD(int op1ml, int op2ml, int op3ml, int op4ml, int op1dt2, int op2dt2, int op3dt2, int op4dt2, int keyshift)
        {
            OP1ML = op1ml;
            OP2ML = op2ml;
            OP3ML = op3ml;
            OP4ML = op4ml;
            OP1DT2 = op1dt2;
            OP2DT2 = op2dt2;
            OP3DT2 = op3dt2;
            OP4DT2 = op4dt2;
            KeyShift = keyshift;
        }
    }

    public class MIDINote
    {
        public partPage page { get; internal set; }
        public MML mml { get; internal set; }
        public byte noteNumber { get; internal set; }
        public byte velocity { get; internal set; }
        public long length { get; internal set; }
        public bool? beforeKeyon { get; internal set; } = null;
        public bool Keyon { get; internal set; } = false;
    }

}
