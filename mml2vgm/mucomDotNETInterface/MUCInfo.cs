using System;
using System.Collections.Generic;

namespace mucomDotNET.Interface
{
    public class MUCInfo
    {
        public string title { get; set; }
        public string composer { get; set; }
        public string author { get; set; }
        public string comment { get; set; }
        public string mucom88 { get; set; }
        public string date { get; set; }
        public string voice { get; set; }
        public string pcm { get; set; }
        public int lines { get; set; }
        /// <summary>
        /// mml中で定義した音色データ
        /// </summary>
        public byte[] mmlVoiceData { get; set; }
        /// <summary>
        /// ファイルから読み込んだプリセットの音色データ
        /// </summary>
        public byte[] voiceData { get; set; }
        public byte[] pcmData { get; set; }
        public List<Tuple<int, string>> basSrc { get; set; }
        public string fnSrc { get; set; }
        public string workPath { get; set; }
        public string fnDst { get; set; }
        public AutoExtendList<MubDat> bufDst { get; set; }
        public int srcLinPtr { get; set; }
        public int srcCPtr { get; set; }
        public Tuple<int, string> lin { get; set; }
        public bool Carry { get; set; }
        public bool ErrSign { get; set; }
        public AutoExtendList<int> bufMac { get; set; }
        public AutoExtendList<int> bufMacStack { get; set; }
        public AutoExtendList<byte> bufLoopStack { get; set; }

        /// <summary>
        /// mml全体で実際に使用した音色番号
        /// 関連項目:
        /// orig:DEFVOICE
        /// </summary>
        public AutoExtendList<int> bufDefVoice { get; set; }

        public int useOtoAdr { get; set; }
        public AutoExtendList<int> bufTitle { get; set; }
        public AutoExtendList<byte> mmlVoiceDataWork { get; set; }

        public int row { get; set; }
        public int col { get; set; }
        public int VM { get; set; }

        public void Clear()
        {
            title = "";
            composer = "";
            author = "";
            comment = "";
            mucom88 = "";
            date = "";
            voice = "";
            pcm = "";
            lines = 0;
            voiceData = null;
            pcmData = null;
            basSrc = new List<Tuple<int, string>>();
            fnSrc = "";
            workPath = "";
            fnDst = "";
            bufDst = new AutoExtendList<MubDat>();
            srcLinPtr = 0;
            srcCPtr = 0;
            bufMac = new AutoExtendList<int>();
            bufMacStack = new AutoExtendList<int>();
            bufLoopStack = new AutoExtendList<byte>();
            bufDefVoice = new AutoExtendList<int>();
            bufTitle = new AutoExtendList<int>();
            mmlVoiceDataWork = new AutoExtendList<byte>();
        }

    }
}