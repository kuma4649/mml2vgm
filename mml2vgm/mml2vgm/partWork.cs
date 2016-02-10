using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mml2vgm
{
    public class partWork
    {

        /// <summary>
        /// パートデータ
        /// </summary>
        public List<Tuple<int, string>> pData = null;

        /// <summary>
        /// エイリアスデータ
        /// </summary>
        public Dictionary<string, Tuple<int, string>> aData = null;

        /// <summary>
        /// データが最後まで演奏されたかどうかを示す(注意:trueでも演奏が終わったとは限らない)
        /// </summary>
        public bool dataEnd = false;

        /// <summary>
        /// 次に演奏されるデータの位置
        /// </summary>
        private clsPos pos=new clsPos();

        /// <summary>
        /// 位置情報のスタック
        /// </summary>
        private Stack<clsPos> stackPos = new Stack<clsPos>();

        /// <summary>
        /// リピート位置情報のスタック
        /// </summary>
        public Stack<clsRepeat> stackRepeat = new Stack<clsRepeat>();

        /// <summary>
        /// パートごとの音源の種類
        /// </summary>
        public ePartType type = ePartType.YM2612;

        /// <summary>
        /// 割り当てられた音源のチャンネル番号
        /// </summary>
        public int ch = 0;

        public string partName = "";

        public int freq = 0;

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
        public int octave = 4;

        /// <summary>
        /// vコマンドで設定されている音量
        /// </summary>
        public int volume = 127;

        /// <summary>
        /// pコマンドで設定されている音の定位(1:R 2:L 3:C)
        /// </summary>
        public int pan = 3;

        /// <summary>
        /// @コマンドで設定されている音色
        /// </summary>
        public int instrument = -1;

        public int envIndex = -1;

        public int envCounter = -1;

        public int envVolume = -1;

        public int[] envelope = new int[8] { 0,0,0,0,0,0,0,0 };

        public bool envelopeMode = false;

        /// <summary>
        /// Dコマンドで設定されているデチューン
        /// </summary>
        public int detune = 0;

        public char noteCmd = 'c';

        public int shift = 0;

        public int pcmNote = 0;

        /// <summary>
        /// mコマンドで設定されているpcmモード(true:PCM false:FM)
        /// </summary>
        public bool pcm = false;

        /// <summary>
        /// q/Qコマンドで設定されているゲートタイム(clock/%)
        /// </summary>
        public int gatetime = 0;

        /// <summary>
        /// q/Qコマンドで最後に指定されたのはQコマンドかどうか
        /// </summary>
        public bool gatetimePmode = false;

        /// <summary>
        /// 使用するスロット
        /// </summary>
        public byte slots = 0xf;

        public bool tie = false;

        public bool beforeTie = false;

        public int beforeVolume = -1;

        /// <summary>
        /// 効果音モード
        /// </summary>
        public bool Ch3SpecialMode = false;

        /// <summary>
        /// KeyOnフラグ
        /// </summary>
        public bool Ch3SpecialModeKeyOn = false;

        /// <summary>
        /// Lfo
        /// </summary>
        public clsLfo[] lfo = new clsLfo[4] { new clsLfo(), new clsLfo(), new clsLfo(), new clsLfo() };

        /// <summary>
        /// ベンド中のoコマンドで設定されているオクターブ数
        /// </summary>
        public int bendOctave = 4;

        public char bendNote = 'r';

        public long bendWaitCounter = -1;

        public Stack<Tuple<int, int>> bendList = new Stack<Tuple<int, int>>();

        public int bendFnum = 0;

        public int bendShift = 0;

        public int[] slotDetune = new int[] { 0, 0, 0, 0 };

        public int noise = 0;

        public int keyShift = 0;



        public void resetPos()
        {
            pos = new clsPos();
            stackPos = new Stack<clsPos>();
        }

        public int getPos()
        {
            return pos.tCol;
        }

        public int getLineNumber()
        {
            if (pos.alies == "")
            {
                return pData[pos.row].Item1;
            }
            return aData[pos.alies].Item1;
        }

        public char getChar()
        {
            char ch;
            if (pos.alies == "")
            {
                ch = pData[pos.row].Item2[pos.col];
            }
            else
            {
                ch = aData[pos.alies].Item2[pos.col];
            }
            //Console.Write(ch);
            return ch;
        }

        public void incPos()
        {
            setPos(pos.tCol + 1);
        }

        public void decPos()
        {
            setPos(pos.tCol - 1);
        }

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

        public void setPos(int tCol)
        {
            if (pData == null)
            {
                return;
            }

            dataEnd = false;

            int row = 0;
            int col = 0;
            int n = 0;
            string aliesName = "";
            resetPos();

            pos.tCol = tCol;

            while (true)
            {
                string data;
                char ch;

                //読みだすデータの頭出し
                if (aliesName == "")
                {
                    if (pData.Count == row)
                    {
                        dataEnd = true;
                        return;
                    }
                    data = pData[row].Item2;
                }
                else
                {
                    data = aData[aliesName].Item2;
                }

                while (data.Length == col)
                {
                    if (aliesName == "")
                    {
                        row++;
                        if (pData.Count == row)
                        {
                            dataEnd = true;
                            break;
                        }
                        else
                        {
                            data = pData[row].Item2;
                            col = 0;
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
                            data = pData[row].Item2;
                        }
                        else
                        {
                            data = aData[aliesName].Item2;
                        }
                    }
                }
                ch = data[col];

                while (ch == '%')
                {
                    string a = getAliesName(data, col);
                    if (a != "")
                    {
                        clsPos p = new clsPos();
                        p.alies = aliesName;
                        p.col = col + a.Length + 1;
                        p.row = row;
                        stackPos.Push(p);

                        data = aData[a].Item2;
                        col = 0;
                        aliesName = a;
                        row = 0;
                    }
                    else
                    {
                        msgBox.setWrnMsg("指定されたエイリアス名は定義されていません。"
                            , (aliesName == "") ? pData[row].Item1 : aData[aliesName].Item1
                            );
                        col++;
                    }
                    ch = data[col];
                }

                if (n == tCol)
                {
                    pos.row = row;
                    pos.col = col;
                    pos.alies = aliesName;
                    break;
                }

                n++;
                col++;
                while (data.Length == col)
                {
                    if (aliesName == "")
                    {
                        row++;
                        if (pData.Count == row)
                        {
                            dataEnd = true;
                            break;
                        }
                        else
                        {
                            data = pData[row].Item2;
                            col = 0;
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
                            data = pData[row].Item2;
                        }
                        else
                        {
                            data = aData[aliesName].Item2;
                        }
                    }
                }

            }

        }

        public bool getNum(out int num)
        {

            string n = "";
            int ret = -1;

            while (getChar() == ' ' || getChar() == '\t')
            {
                incPos();
            }

            if (getChar() == '-' || getChar() == '+')
            {
                n = getChar().ToString();
                incPos();
            }

            while (getChar() == ' ' || getChar() == '\t')
            {
                incPos();
            }

            if (getChar() != '$')
            {
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

                if (!int.TryParse(n, out ret))
                {
                    num = -1;
                    return false;
                }

                num = ret;
            }
            else
            {
                incPos();
                n += getChar();
                incPos();
                n += getChar();
                incPos();
                try {
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

    public enum ePartType
    {
        YM2612 , YM2612extend , SegaPSG
    }

    public enum eLfoType
    {
        Tremolo , Vibrato , Hardware
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

    public class clsLfo
    {

        public eLfoType type = eLfoType.Hardware;
        public List<int> param =null;
        public bool sw = false;
        public bool isEnd = false;
        public long waitCounter = 0;
        public int value = 0;
        public int direction = 0;

    }

}
