using musicDriverInterface;
using System.Collections.Generic;

namespace Core
{
    public class CommandArpeggio
    {
        public bool Sw = false;//アルペジヲ動作スイッチ

        public int Num = -1;//アルペジヲ定義番号
        public enmMMLType DefCmd;//デフォルトで使用するコマンドの種類
        public List<object> DefCmdArg;//デフォルトで使用するコマンドの引数
        public int Sync = 0;//同期モード

        public int Ptr = 0;//現在の位置
        public int LoopPtr = -1;//ループポイント
        public int WaitCounter = 0;//次の行動までの待ち時間
        public int WaitClock = 1;//待ち時間
        public bool Infinite = false;//無限ループ対策
    }
}
