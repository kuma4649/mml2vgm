using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class partWork
    {

        /// <summary>
        /// パートデータ
        /// </summary>
        public List<Line> pData = null;

        /// <summary>
        /// エイリアスデータ
        /// </summary>
        public Dictionary<string, Line> aData = null;

        /// <summary>
        /// mmlデータ
        /// </summary>
        public List<MML> mmlData = null;

        public int mmlPos = 0;

        /// <summary>
        /// データが最後まで演奏されたかどうかを示す(注意:trueでも演奏が終わったとは限らない)
        /// </summary>
        public bool dataEnd = false;

        /// <summary>
        /// 次に演奏されるデータの位置
        /// </summary>
        public clsPos pos = new clsPos();

        /// <summary>
        /// 位置情報のスタック
        /// </summary>
        private Stack<clsPos> stackPos = new Stack<clsPos>();

        /// <summary>
        /// リピート位置情報のスタック
        /// </summary>
        public Stack<clsRepeat> stackRepeat = new Stack<clsRepeat>();

        /// <summary>
        /// 連符位置情報のスタック
        /// </summary>
        public Stack<clsRenpu> stackRenpu = new Stack<clsRenpu>();

        /// <summary>
        /// パートごとの音源の種類
        /// </summary>
        public ClsChip chip = null;

        /// <summary>
        /// Secondary Chipか
        /// </summary>
        public int isSecondary = 0;
        
        /// <summary>
        /// 割り当てられた音源のチャンネル番号
        /// </summary>
        public int ch = 0;

        /// <summary>
        /// 未加工のf-num
        /// </summary>
        public int freq = 0;

        public int beforeFNum = -1;
        public int FNum = -1;


        /// <summary>
        /// いままで演奏した総クロック数
        /// </summary>
        public long clockCounter = 0L;

        /// <summary>
        /// あとどれだけ待機するかを示すカウンター(clock)
        /// </summary>
        public long waitCounter = 0L;

        /// <summary>
        /// キーオフコマンドを発行するまであとどれだけ待機するかを示すカウンター(clock)
        /// (waitCounterよりも大きい場合キーオフされない)
        /// </summary>
        public long waitKeyOnCounter = 0L;

        /// <summary>
        /// lコマンドで設定されている音符の長さ(clock)
        /// </summary>
        public long length = 24;

        /// <summary>
        /// oコマンドで設定されているオクターブ数
        /// </summary>
        public int octaveNow = 4;

        public int octaveNew = 4;

        public int TdA = -1;
        public int op1ml = -1;
        public int op2ml = -1;
        public int op3ml = -1;
        public int op4ml = -1;
        public int op1dt2 = -1;
        public int op2dt2 = -1;
        public int op3dt2 = -1;
        public int op4dt2 = -1;
        public int toneDoubler = 0;
        public int toneDoublerKeyShift = 0;

        /// <summary>
        /// vコマンドで設定されている音量
        /// </summary>
        public int volume = 127;

        /// <summary>
        /// expression
        /// </summary>
        public int expression = 127;

        /// <summary>
        /// pコマンドで設定されている音の定位(1:R 2:L 3:C)
        /// </summary>
        //public int pan = 3;
        //public int beforePan = -1;
        public dint pan = new dint(3);
        public dint beforePan = new dint(3);

        public bool[] tblNoteOn = null;

        /// <summary>
        /// 拡張パン(Left)
        /// </summary>
        /// <remarks>
        /// ボリュームが左右別管理の音源向け
        /// </remarks>
        public int panL = 0;
        public int beforePanL = -1;

        /// <summary>
        /// 拡張パン(Right)
        /// </summary>
        /// <remarks>
        /// ボリュームが左右別管理の音源向け
        /// </remarks>
        public int panR = 0;
        public int beforePanR = -1;

        /// <summary>
        /// 拡張ボリューム(Left)before
        /// </summary>
        /// <remarks>
        /// ボリュームが左右別管理の音源向け
        /// </remarks>
        public int beforeLVolume = -1;

        /// <summary>
        /// 拡張ボリューム(Right)before
        /// </summary>
        /// <remarks>
        /// ボリュームが左右別管理の音源向け
        /// </remarks>
        public int beforeRVolume = -1;

        /// <summary>
        /// @コマンドで設定されている音色
        /// </summary>
        public int instrument = -1;

        /// <summary>
        /// OPLLのサスティン
        /// </summary>
        public bool sus = false;

        /// <summary>
        /// 使用中のエンベロープ定義番号
        /// </summary>
        public int envInstrument = -1;
        public int beforeEnvInstrument = 0;

        /// <summary>
        /// エンベロープの進捗位置
        /// </summary>
        public int envIndex = -1;

        /// <summary>
        /// エンベロープ向け汎用カウンター
        /// </summary>
        public int envCounter = -1;

        /// <summary>
        /// エンベロープ音量
        /// </summary>
        public int envVolume = -1;

        /// <summary>
        /// 使用中のエンベロープの定義
        /// </summary>
        public int[] envelope = new int[9] { 0, 0, 0, 0, 0, 0, 0, 0, -1 };

        /// <summary>
        /// エンベロープスイッチ
        /// </summary>
        public bool envelopeMode = false;

        /// <summary>
        /// リズムモードスイッチ
        /// </summary>
        public bool rhythmMode = false;

        /// <summary>
        /// Dコマンドで設定されているデチューン
        /// </summary>
        public int detune = 0;

        /// <summary>
        /// 発音される音程
        /// </summary>
        public char noteCmd = (char)0;//'c';

        /// <summary>
        /// 音程をずらす量
        /// </summary>
        public int shift = 0;

        /// <summary>
        /// PCMの音程
        /// </summary>
        public int pcmNote = 0;
        public int pcmOctave = 0;

        /// <summary>
        /// mコマンドで設定されているpcmモード(true:PCM false:FM)
        /// </summary>
        public bool pcm = false;

        /// <summary>
        /// PCM マッピングモードスイッチ
        /// </summary>
        public bool isPcmMap = false;

        public float pcmBaseFreqPerFreq = 0.0f;
        public float pcmFreqCountBuffer = 0.0f;
        public long pcmWaitKeyOnCounter = 0L;
        public long pcmSizeCounter = 0L;

        public bool streamSetup = false;
        public int streamID = -1;
        public long streamFreq = 0;


        /// <summary>
        /// q/Qコマンドで設定されているゲートタイム(clock/%)
        /// </summary>
        public int gatetime = 0;

        /// <summary>
        /// q/Qコマンドで最後に指定されたのはQコマンドかどうか
        /// </summary>
        public bool gatetimePmode = false;

        /// <summary>
        /// 使用する現在のスロット
        /// </summary>
        public byte slots = 0xf;

        /// <summary>
        /// 4OP(通常)時に使用するスロット
        /// </summary>
        public byte slots4OP = 0xf;

        /// <summary>
        /// EX時に使用するスロット
        /// </summary>
        public byte slotsEX = 0x0;

        /// <summary>
        /// タイ
        /// </summary>
        public bool tie = false;

        /// <summary>
        /// 前回発音時にタイ指定があったかどうか
        /// </summary>
        public bool beforeTie = false;

        public bool keyOn = false;
        public bool keyOff = false;
        public int rhythmKeyOnData = -1;

        /// <summary>
        /// 前回発音時の音量
        /// </summary>
        public int beforeVolume = -1;

        public int beforeExpression = -1;

        /// <summary>
        /// 効果音モード
        /// </summary>
        public bool Ch3SpecialMode = false;

        /// <summary>
        /// KeyOnフラグ
        /// </summary>
        public bool Ch3SpecialModeKeyOn = false;

        public bool HardEnvelopeSw = false;
        public int HardEnvelopeType = -1;
        public int HardEnvelopeSpeed = -1;

        /// <summary>
        /// Lfo(4つ)
        /// </summary>
        public clsLfo[] lfo = new clsLfo[4] { new clsLfo(), new clsLfo(), new clsLfo(), new clsLfo() };

        /// <summary>
        /// ベンド中のoコマンドで設定されているオクターブ数
        /// </summary>
        public int bendOctave = 4;

        /// <summary>
        /// ベンド中の音程
        /// </summary>
        public char bendNote = 'r';

        /// <summary>
        /// ベンド中の待機カウンター
        /// </summary>
        public long bendWaitCounter = -1;

        /// <summary>
        /// ベンド中に参照される周波数スタックリスト
        /// </summary>
        public Stack<Tuple<int, int>> bendList = new Stack<Tuple<int, int>>();

        /// <summary>
        /// ベンド中の発音周波数
        /// </summary>
        public int bendFnum = 0;

        /// <summary>
        /// ベンド中に音程をずらす量
        /// </summary>
        public int bendShift = 0;

        /// <summary>
        /// スロットごとのディチューン値
        /// </summary>
        public int[] slotDetune = new int[] { 0, 0, 0, 0 };

        /// <summary>
        /// ノイズモード値
        /// </summary>
        public int noise = 0;

        /// <summary>
        /// SSG Noise or Tone mixer 0:Silent 1:Tone 2:Noise 3:Tone&Noise
        /// OPM Noise 0:Disable 1:Enable
        /// </summary>
        public int mixer = 1;

        /// <summary>
        /// キーシフト
        /// </summary>
        public int keyShift = 0;

        /// <summary>
        /// アドレスシフト値
        /// </summary>
        public int addressShift = 0;

        public string PartName="";

        public int rf5c164AddressIncrement = -1;
        public int rf5c164Envelope = -1;
        public int rf5c164Pan = -1;

        public int huc6280Envelope = -1;
        public int huc6280Pan = -1;

        public int pcmStartAddress = -1;
        public int beforepcmStartAddress = -1;
        public int pcmLoopAddress = -1;
        public int beforepcmLoopAddress = -1;
        public int pcmEndAddress = -1;
        public int beforepcmEndAddress = -1;
        public int pcmBank = 0;
        public int beforepcmBank = -1;

        public enmChannelType Type;
        public int MaxVolume = 0;
        public int MaxExpression = 0;
        public byte[][] port = null;

        public int feedBack = 0;
        public int beforeFeedBack = -1;
        public int algo = 0;
        public int beforeAlgo = -1;
        public int algConstSw = 0;
        public int beforeAlgConstSw = -1;
        public int ams = 0;
        public int beforeAms = -1;
        public int fms = 0;
        public int beforeFms = -1;
        public int pms = 0;
        public int beforePms = -1;

        public bool hardLfoSw = false;
        public int hardLfoNum = 0;
        public int hardLfoFreq = 0;
        public int hardLfoPMD = 0;
        public int hardLfoAMD = 0;

        public bool reqFreqReset = false;
        public bool reqKeyOffReset = false;

        public bool renpuFlg = false;
        public List<int> lstRenpuLength = null;

        /// <summary>
        /// パート情報をリセットする
        /// </summary>
        public void resetPos()
        {
            pos = new clsPos();
            stackPos = new Stack<clsPos>();
        }

        /// <summary>
        /// 解析位置を取得する
        /// </summary>
        /// <returns></returns>
        public int getPos()
        {
            return pos.tCol;
        }

        public Line getLine()
        {
            if (pos.alies == "")
            {
                return pData[pos.row].Copy();
            }
            return aData[pos.alies].Copy();
        }

        /// <summary>
        /// 解析位置に対するソースファイル上の行数を得る
        /// </summary>
        /// <returns></returns>
        public int getLineNumber()
        {
            if (pos.alies == "")
            {
                return pData[pos.row].Lp.row;
            }
            return aData[pos.alies].Lp.row;
        }

        /// <summary>
        /// 解析位置に対するソースファイル名を得る
        /// </summary>
        /// <returns></returns>
        public string getSrcFn()
        {
            if (pos.alies == "")
            {
                return pData[pos.row].Lp.filename;
            }
            return aData[pos.alies].Lp.filename;
        }

        /// <summary>
        /// 解析位置の文字を取得する
        /// </summary>
        /// <returns></returns>
        public char getChar()
        {
            //if (dataEnd) return (char)0;

            char ch;
            if (pos.alies == "")
            {
                if (pData[pos.row].Txt.Length <= pos.col+pData[pos.row].Lp.col) {
                    return (char)0;
                }
                ch = pData[pos.row].Txt[pos.col + pData[pos.row].Lp.col];
            }
            else
            {
                if (aData[pos.alies].Txt.Length <= pos.col + aData[pos.alies].Lp.col)
                {
                    return (char)0;
                }
                ch = aData[pos.alies].Txt[pos.col + aData[pos.alies].Lp.col];
            }
            //Console.Write(ch);
            return ch;
        }

        /// <summary>
        /// 解析位置を一つ進める(重い！)
        /// </summary>
        public void incPos()
        {
            setPos(pos.tCol + 1);
        }

        /// <summary>
        /// 解析位置を一つ戻す(重い！)
        /// </summary>
        public void decPos()
        {
            setPos(pos.tCol - 1);
        }

        /// <summary>
        /// 指定された文字数だけ読み出し、文字列を生成する
        /// </summary>
        /// <param name="len">文字数</param>
        /// <returns>文字列</returns>
        public string getString(int len)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < len; i++)
            {
                sb.Append(getChar());
                incPos();
            }
            return sb.ToString();
        }

        /// <summary>
        /// 解析位置を指定する
        /// </summary>
        /// <param name="tCol">解析位置</param>
        public void setPos(int tCol)
        {
            if (pData == null)
            {
                return;
            }

            if (LstPos == null) MakeLstPos();

            int i = 0;
            while (i != LstPos.Count && tCol >= LstPos[i].tCol)
            {
                i++;
            }

            pos.tCol = tCol;
            pos.alies = LstPos[i - 1].alies;
            pos.col = LstPos[i - 1].col + tCol - LstPos[i - 1].tCol;
            pos.row = LstPos[i - 1].row;
            return;

        }

        private List<clsPos> LstPos = null;

        public int pcmMapNo { get; set; } = 0;
        public int dutyCycle { get; set; } = 0;
        public int oldDutyCycle { get; set; } = -1;
        public int oldFreq { get; set; } = -1;

        public bool directModeVib { get; set; }
        public bool directModeTre { get; set; }
        public int pitchBend { get; set; } = 0;
        public int tieBend { get; set; } = 0;
        public int portaBend { get; set; } = 0;
        public int lfoBend { get; set; } = 0;
        public int bendStartOctave { get; set; } = 0;
        public char bendStartNote { get; set; } = 'c';
        public int bendStartShift { get; set; } = 0;

        public void MakeLstPos()
        {
            if (pData == null)
            {
                return;
            }

            int tCol = 0;
            int row = 0;
            int col = 0;
            int pCol = 0;
            string aliesName = "";

            LstPos = new List<clsPos>();
            LstPos.Add(new clsPos());
            resetPos();

            while (true)
            {
                string data;
                char ch;

                //読みだすデータの頭出し
                if (aliesName == "")
                {
                    if (pData.Count == row)
                    {
                        return;
                    }
                    data = pData[row].Txt;
                    pCol = pData[row].Lp.col;
                }
                else
                {
                    data = aData[aliesName].Txt;
                    pCol = aData[aliesName].Lp.col;
                }

                //解析行の解析位置が終端に達したときの処理
                while (data.Length == col+pCol)
                {
                    if (aliesName == "")
                    {
                        row++;
                        if (pData.Count == row)
                        {
                            break;
                        }
                        else
                        {
                            data = pData[row].Txt;
                            pCol = pData[row].Lp.col;
                            col = 0;// pData[row].Lp.col;

                            clsPos p = new clsPos();
                            p.tCol = tCol;
                            p.alies = "";
                            p.col = 0;
                            p.row = row;
                            LstPos.Add(p);

                            break;
                        }
                    }
                    else
                    {
                        clsPos p = stackPos.Pop();
                        aliesName = p.alies;
                        col = p.col;
                        row = p.row;
                        if (aliesName == "")
                        {
                            data = pData[row].Txt;
                            pCol = pData[row].Lp.col;
                        }
                        else
                        {
                            data = aData[aliesName].Txt;
                            pCol = aData[aliesName].Lp.col;
                        }

                        p.tCol = tCol;
                        LstPos.Add(p);
                    }
                }

                ch = data[col + pCol];

                //解析位置でエイリアス指定されている場合
                while (ch == '%')
                {
                    string a = getAliesName(data, col + pCol);
                    if (a != "")
                    {
                        clsPos p = new clsPos();
                        p.alies = aliesName;
                        p.col = col + a.Length + 1;
                        p.row = row;
                        stackPos.Push(p);

                        data = aData[a].Txt;
                        pCol = aData[a].Lp.col;
                        col = 0;
                        aliesName = a;
                        row = 0;

                        p = new clsPos();
                        p.tCol = tCol;
                        p.alies = a;
                        p.col = 0;
                        p.row = 0;
                        LstPos.Add(p);
                    }
                    else
                    {
                        msgBox.setWrnMsg(msg.get("E06000")
                            , (aliesName == "") ? pData[row].Lp : aData[aliesName].Lp
                            );
                        col++;
                    }

                    ch = data[col + pCol];
                }

                tCol++;
                col++;
                //解析行の解析位置が終端に達したときの処理
                while (data.Length == col+pCol)
                {
                    if (aliesName == "")
                    {
                        row++;
                        if (pData.Count == row)
                        {
                            break;
                        }
                        else
                        {
                            data = pData[row].Txt;
                            pCol = pData[row].Lp.col;
                            col = 0;

                            clsPos p = new clsPos();
                            p.tCol = tCol;
                            p.alies = "";
                            p.col = 0;
                            p.row = row;
                            LstPos.Add(p);

                            break;
                        }
                    }
                    else
                    {
                        clsPos p = stackPos.Pop();
                        aliesName = p.alies;
                        col = p.col;
                        row = p.row;
                        if (aliesName == "")
                        {
                            data = pData[row].Txt;
                            pCol = pData[row].Lp.col;
                        }
                        else
                        {
                            data = aData[aliesName].Txt;
                            pCol = aData[aliesName].Lp.col;
                        }

                        p.tCol = tCol;
                        LstPos.Add(p);
                    }
                }

            }

        }

        /// <summary>
        /// タブと空白は読み飛ばす
        /// </summary>
        /// <returns>飛ばした文字数</returns>
        public int skipTabSpace()
        {
            int cnt = 0;
            while (getChar() == ' ' || getChar() == '\t')
            {
                incPos();
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
        public bool getNum(out int num)
        {

            string n = "";
            int ret = -1;

            skipTabSpace();

            //+-符号を取得する(ない場合は正とする)
            if (getChar() == '-' || getChar() == '+')
            {
                n = getChar().ToString();
                incPos();
            }

            skipTabSpace();

            //１６進数指定されているか
            if (getChar() != '$')
            {
                //数字でなくなるまで取得
                while (true)
                {
                    if (getChar() >= '0' && getChar() <= '9')
                    {
                        try
                        {
                            n += getChar();
                            incPos();
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
                incPos();
                n += getChar();
                incPos();
                n += getChar();
                incPos();
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

        public bool getNum(out int num,ref int col)
        {

            string n = "";
            int ret = -1;

            col += skipTabSpace();

            //符号を取得する(ない場合は正とする)
            if (getChar() == '-' || getChar() == '+')
            {
                n = getChar().ToString();
                incPos();
                col++;
            }

            col += skipTabSpace();

            //１６進数指定されているか
            if (getChar() != '$')
            {
                //数字でなくなるまで取得
                while (true)
                {
                    if (getChar() >= '0' && getChar() <= '9')
                    {
                        try
                        {
                            n += getChar();
                            incPos();
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

                num = ret;
            }
            else
            {
                //２文字取得
                incPos();
                col++;
                n += getChar();
                incPos();
                col++;
                n += getChar();
                incPos();
                col++;
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

        public bool getNumNoteLength(out int num, out bool flg)
        {

            flg = false;

            skipTabSpace();

            //クロック直接指定
            if (getChar() == '#')
            {
                flg = true;
                incPos();
            }

            return getNum(out num);
        }

        public bool getNumNoteLength(out int num, out bool flg,out int col)
        {

            flg = false;
            col = 0;

            col += skipTabSpace();

            //クロック直接指定
            if (getChar() == '#')
            {
                flg = true;
                incPos();
                col++;
            }

            return getNum(out num,ref col);
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

        /// <summary>
        /// 次に演奏されるデータのエイリアス名
        /// </summary>
        public string alies = "";
    }

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

    }

    public class clsPcm
    {
        public enmChipType chip = enmChipType.YM2612;
        public int isSecondary = 0;
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
            ,int seqNum
            ,enmChipType chip
            ,int isSecondary
            ,string fileName
            ,int freq
            ,int vol 
            ,long stAdr
            ,long edAdr
            ,long size
            ,long loopAdr
            ,bool is16bit
            ,int samplerate
            ,params object[] option)
        {
            this.num = num;
            this.seqNum = seqNum;
            this.chip = chip;
            this.isSecondary = isSecondary;
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
        public int isSecondary = 0;
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
            , int isSecondary
            , int LoopAdr)
        {
            this.type = type;
            this.No = No;
            this.FileName = FileName;
            this.BaseFreq = BaseFreq;
            this.Volume = Volume;
            this.chip = chip;
            this.isSecondary = isSecondary;
            this.DatLoopAdr = LoopAdr;
        }

        public clsPcmDatSeq(
            enmPcmDefineType type
            , string FileName
            , enmChipType chip
            , int isSecondary
            , int SrcStartAdr
            , int DesStartAdr
            , int Length
            , object[] Option)
        {
            this.type = type;
            this.FileName = FileName;
            this.chip = chip;
            this.isSecondary = isSecondary;
            this.SrcStartAdr = SrcStartAdr;
            this.DesStartAdr = DesStartAdr;
            this.SrcLength = Length;
            this.Option = Option;
            if(chip== enmChipType.YM2610B)
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
            , int isSecondary
            , int BaseFreq
            , int DatStartAdr
            , int DatEndAdr
            , int DatLoopAdr
            , object[] Option)
        {
            this.type = type;
            this.No = No;
            this.chip = chip;
            this.isSecondary = isSecondary;
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

}
