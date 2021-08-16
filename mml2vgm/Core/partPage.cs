using musicDriverInterface;
using System;
using System.Collections.Generic;

namespace Core
{
    public class partPage
    {
        /// <summary>
        /// 共有ページ
        /// </summary>
        public partPage spg = null;

        /// <summary>
        /// レイヤーかどうか
        /// </summary>
        public bool isLayer = false;

        /// <summary>
        /// 割り込み許可フラグ
        /// (他のページからの割り込みを許可するかどうかのフラグ)
        /// </summary>
        public bool enableInterrupt = false;

        /// <summary>
        /// 割り込み要求フラグ
        /// </summary>
        public bool requestInterrupt = false;

        /// <summary>
        /// パートデータ
        /// </summary>
        public List<Line> pData = null;

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
        public Stack<clsPos> stackPos = new Stack<clsPos>();

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
        public int chipNumber = 0;

        /// <summary>
        /// 割り当てられた音源のチャンネル番号
        /// </summary>
        public int ch = 0;

        public int MIDIch = 0;

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

        public int latestOctave = 4;

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

        public int latestVolume = 127;

        /// <summary>
        /// expression
        /// </summary>
        public int expression = 127;

        /// <summary>
        /// pコマンドで設定されている音の定位(1:R 2:L 3:C)
        /// </summary>
        public int pan = 3;
        public int beforePan = -1;
        //public dint pan = new dint(3);
        //public dint beforePan = new dint(3);

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

        public int beforeRLVolume = -1;
        public int beforeRRVolume = -1;

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
        /// 使用中のアルペジオ定義番号
        /// </summary>
        public int arpInstrument = -1;
        public int beforeArpInstrument = 0;

        /// <summary>
        /// アルペジオの進捗位置
        /// </summary>
        public int arpIndex = -1;

        /// <summary>
        /// アルペジオの解析位置
        /// </summary>
        public int arpInstrumentPtr = 0;

        /// <summary>
        /// アルペジオの解析ループ位置
        /// </summary>
        public int arpLoopPtr = 0;

        /// <summary>
        /// アルペジオ中のKeyOnの長さ
        /// </summary>
        public int arpKeyOnLength = 0;

        /// <summary>
        /// アルペジオ中のKeyOffの長さ
        /// </summary>
        public int arpKeyOffLength = 0;

        /// <summary>
        /// アルペジオ中の変化量
        /// </summary>
        public int arpDelta = 0;

        /// <summary>
        /// アルペジオ中の長さ(KeyOn/Off含む)
        /// </summary>
        public int arpClock = 0;

        /// <summary>
        /// アルペジオ タイモード
        /// </summary>
        public bool arpTieMode = false;

        /// <summary>
        /// アルペジオ 周波数モード
        /// </summary>
        public bool arpFreqMode = false;

        /// <summary>
        /// アルペジオ向け汎用カウンター
        /// </summary>
        public int arpCounter = -1;

        /// <summary>
        /// アルペジオスイッチ
        /// </summary>
        public bool arpeggioMode = false;

        /// <summary>
        /// アルペジオ向けゲートタイムモード(当分false固定)
        /// </summary>
        public bool arpGatetimePmode = false;

        /// <summary>
        /// アルペジオ向けゲートタイム
        /// </summary>
        public int arpGatetime = 0;

        /// <summary>
        /// アルペジオ向け無限ループ対策
        /// </summary>
        public bool arpInfinite = false;


        /// <summary>
        /// volumeアルペジオスイッチ
        /// </summary>
        public bool varpeggioMode = false;

        /// <summary>
        /// 使用中のvolumeアルペジオ定義番号
        /// </summary>
        public int varpInstrument = -1;
        //public int beforeVArpInstrument = 0;

        /// <summary>
        /// volumeアルペジオ中の変化量
        /// </summary>
        public int varpDelta = 0;

        /// <summary>
        /// volumeアルペジオの進捗位置
        /// </summary>
        public int varpIndex = -1;

        /// <summary>
        /// volumeアルペジオの解析位置
        /// </summary>
        public int varpInstrumentPtr = 0;

        /// <summary>
        /// volumeアルペジオの解析ループ位置
        /// </summary>
        public int varpLoopPtr = 0;

        /// <summary>
        /// volumeアルペジオ中のKeyOnの長さ
        /// </summary>
        public int varpKeyOnLength = 0;

        ///// <summary>
        ///// volumeアルペジオ中のKeyOffの長さ
        ///// </summary>
        //public int varpKeyOffLength = 0;

        /// <summary>
        /// volumeアルペジオ中の長さ(KeyOn/Off含む)
        /// </summary>
        public int varpClock = 0;

        ///// <summary>
        ///// volumeアルペジオ タイモード
        ///// </summary>
        //public bool varpTieMode = false;

        /// <summary>
        /// volumeアルペジオ向け汎用カウンター
        /// </summary>
        public int varpCounter = -1;

        ///// <summary>
        ///// volumeアルペジオ向けゲートタイムモード(当分false固定)
        ///// </summary>
        //public bool varpGatetimePmode = false;

        ///// <summary>
        ///// volumeアルペジオ向けゲートタイム
        ///// </summary>
        //public int varpGatetime = 0;

        /// <summary>
        /// アルペジオ向け無限ループ対策
        /// </summary>
        public bool varpInfinite = false;



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
        /// 音程をずらす量(fnum)
        /// </summary>
        public int pitchShift = 0;

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
        /// q/Qコマンドのリバースモード
        /// </summary>
        public bool gatetimeReverse = false;

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
        public int tlDelta1 = 0;
        public int beforeTlDelta1 = -1;
        public int tlDelta2 = 0;
        public int beforeTlDelta2 = -1;
        public int tlDelta3 = 0;
        public int beforeTlDelta3 = -1;
        public int tlDelta4 = 0;
        public int beforeTlDelta4 = -1;
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
        /// ベンド中に音程をずらす量(fnum)
        /// </summary>
        public int bendPitchShift = 0;

        /// <summary>
        /// スロットごとのディチューン値
        /// </summary>
        public int[] slotDetune = new int[] { 0, 0, 0, 0 };

        /// <summary>
        /// ノイズモード値
        /// </summary>
        public int noise = 0;

        /// <summary>
        /// 固定fnumモード(-1:off 0以上:fnum値)
        /// </summary>
        public int forcedFnum = -1;

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

        public string PartName = "";

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

        public MIDINote[] noteOns = new MIDINote[128];

        public List<clsPos> LstPos = null;
        public bool phaseReset = false;

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
        public int velocity { get; set; } = 110;
        public int effectDistortionSwitch { get; internal set; } = 0;
        public int effectDistortionVolume { get; internal set; } = 32;
        public int effectChorusSwitch { get; internal set; } = 0;
        public int effectChorusMixLevel { get; internal set; } = 40;
        
        public int effectCompressorSwitch { get; internal set; } = 0;
        public int effectCompressorVolume { get; internal set; } = 40;
        public EffectParams effectLPF = new EffectParams();
        public EffectParams effectHPF = new EffectParams();
        public EffectParams effectSystemEffectEQLow = new EffectParams();
        public EffectParams effectSystemEffectEQMid = new EffectParams();
        public EffectParams effectSystemEffectEQHigh = new EffectParams();
        public bool isOp4Mode { get; internal set; } = false;
        public int beforeBendNoteNum { get; internal set; } = -1;
        public int panRL { get; internal set; }
        public int panRR { get; internal set; }
        public int flag { get; internal set; }
        public bool changeFlag { get; internal set; }
        public int C352flag { get; internal set; }
        public List<outDatum> sendData { get; internal set; } = new List<outDatum>();
        public int oldAliesCount { get; internal set; }
        public LinePos oldAlies { get; internal set; }
        public Stack<LinePos> stackAliesPos { get; internal set; }
        public Dictionary<int, CommandArpeggio> commandArpeggio = new Dictionary<int, CommandArpeggio>();

        public int beforeInstrument { get; set; } = -1;
        public bool DirectSend { get; internal set; } = false;
        public int sync { get; internal set; }
        public int beforeNoise { get; internal set; }
        public int beforeMixer { get; internal set; }
        public KeyOnDelay keyOnDelay { get; internal set; } = new KeyOnDelay();
        public HardEnvelopeSync hardEnvelopeSync { get; internal set; } = new HardEnvelopeSync();
        public int hsFnum { get; internal set; }
        public int masterVolume { get; internal set; }
        public int beforeMasterVolume { get; internal set; }
        public bool modulation { get; internal set; }
        public int modulationInst { get; internal set; }
        public int modulationFreq { get; internal set; }
        public int modulationGain { get; internal set; }
        public bool modulationGainFlg { get; internal set; }
        public bool modulationDirection { get; internal set; }
        public char beforeNote { get; internal set; } = 'c';
        public int beforeNoteShift { get; internal set; } = 0;

        public int MPortamentSwitch { get; internal set; } = 0;
        public int MPortamentDelta { get; internal set; } = 0;
        public int MPortamentLength { get; internal set; } = 2;
        public int MPortamentLastNote { get; internal set; } = -1;
        public int MPortamentOneshotSwitch { get; internal set; } = 0;


        public partPage(partPage sharedPg)
        {
            this.spg = sharedPg;
        }

        public void sharedPageInitializer()
        {

        }

        public class EffectParams
        {
            public int Sw = 0;
            public int Freq = 0;
            public int Gain = 0;
            public int Q = 0;
        }

        public class KeyOnDelay
        {
            public bool sw = false;
            public int[] delay = new int[] { 0, 0, 0, 0 };
            public int[] delayWrk = new int[] { -1, -1, -1, -1 };
            public byte keyOn = 0;
            public byte beforekeyOn = 0xff;
            public void KeyOff()
            {
                beforekeyOn = keyOn;
                delayWrk[0] = -1;
                delayWrk[1] = -1;
                delayWrk[2] = -1;
                delayWrk[3] = -1;
            }
        }

        public class HardEnvelopeSync
        {
            public bool sw = false;
            public int octave = 4;
            public int detune = 0;
            public bool tone = false;
        }
    }

}
