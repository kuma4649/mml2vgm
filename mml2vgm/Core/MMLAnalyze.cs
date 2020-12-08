using musicDriverInterface;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;

namespace Core
{
    public class MMLAnalyze
    {

        public Dictionary<string, List<MML>> mmlData = new Dictionary<string, List<MML>>();

        private Dictionary<enmChipType, ClsChip[]> chips;
        private Information info;
        private ClsVgm desVGM;

        public MMLAnalyze(ClsVgm desVGM)
        {
            desVGM.PartInit();
            this.chips = desVGM.chips;
            this.info = desVGM.info;
            //desVGM.jumpCommandCounter = 0;
            this.desVGM = desVGM;
        }

        public int Start()
        {
            foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in chips)
            {
                foreach (ClsChip chip in kvp.Value)
                {
                    if (chip == null) continue;

                    if (!chip.use) continue;

                    foreach (partWork pw in chip.lstPartWork)
                    {
                        foreach (partPage page in pw.pg)
                        {
                            if (page.pData == null)
                            {
                                page.dataEnd = true;
                                page.enableInterrupt = true;
                                continue;
                            }

                            page.chip = chip;
                            page.chipNumber = ((info.format != enmFormat.ZGM && chip.ChipID == 1) ? 1 : 0);
                            page.ch = pw.pg[0].ch;
                            pw.setPos(page, 0);
                            page.PartName = pw.pg[0].PartName;
                            page.waitKeyOnCounter = -1;
                            page.waitCounter = 0;
                            page.freq = -1;

                            Step1(pw, page);//mml全体のフォーマット解析
                            Step2(page);//toneDoubler,bend,tieコマンドの解析
                            Step3(page);//リピート、連符コマンドの解析

                            page.dataEnd = false;

                        }
                    }
                }
            }

            Step4();

            return 0;

        }



        #region step1

        private void Step1(partWork pw, partPage page)
        {
            pw.resetPos(page);
            page.dataEnd = false;
            page.mmlData = new List<MML>();
            SkipFlg = false;

            while (!page.dataEnd)
            {
                char cmd = pw.getChar(page);
                if (cmd == 0) page.dataEnd = true;
                else
                {
                    //lineNumber = pw.getLineNumber();
                    Commander(pw, page, cmd);
                }
            }
        }

        private bool swToneDoubler = false;
        public bool doSkip;
        public Point caretPoint;
        private bool SkipFlg = false;
        private List<partWork> caretPointChannels = new List<partWork>();
        private List<Tuple<int, int>> lstSynchronousMark = new List<Tuple<int, int>>();

        public LinePos linePos { get; internal set; }

        private void Commander(partWork pw, partPage page, char cmd)
        {
            MML mml = new MML();
            mml.line = pw.getLine(page);
            mml.column = page.pos.col + 1;// pw.getPos();


            //エイリアスのスタックが変化したかチェック,更新する

            int aliesCnt = page.pos.alies == null ? 0 : page.pos.alies.Length;
            if (aliesCnt != page.oldAliesCount
                || (
                        aliesCnt > 0
                        // 同じ位置のマクロに入りなおした場合、検知できないが動作には問題ないはず...
                        // ( [%BD]2 こんな感じにしてもループ脱出コマンドが間にはいる )
                        && page.pos.alies[0] != page.oldAlies
                    )
                )
            {

                //初っ端の場合は直前値に初期値をセット
                if (page.oldAlies == null)
                {
                    page.oldAlies = new LinePos();
                    page.oldAlies.aliesDepth = 0;
                }

#if DEBUG

                //参考 : マクロに入ったか、抜けたか、入りなおしたか判定する。
                int n = 0;
                if (page.pos.alies != null)
                {
                    if ((page.pos.alies.Length < 1 ? 0 : page.pos.alies[0].aliesDepth) > page.oldAlies.aliesDepth)
                    {
                        n = page.pos.alies[0].aliesDepth - page.oldAlies.aliesDepth;
                        Console.WriteLine("Push {0} times", n);
                    }
                    else if ((page.pos.alies.Length < 1 ? 0 : page.pos.alies[0].aliesDepth) == page.oldAlies.aliesDepth)
                    {
                        Console.WriteLine("Pop and Push");
                    }
                    else
                    {
                        n = page.oldAlies.aliesDepth - (page.pos.alies.Length < 1 ? 0 : page.pos.alies[0].aliesDepth);
                        Console.WriteLine("Pop {0} times", n);
                    }
                }
#endif

                //トレース情報の更新コマンドを発行
                MML mm = new MML();
                mm.type = enmMMLType.TraceUpdateStack;
                mm.line = pw.getLine(page);
                mm.column = page.pos.col + 1;
                mm.args = new List<object>();
                mm.args.Add(page.pos.alies);
                page.mmlData.Add(mm);

                //直前値を更新
                page.oldAliesCount = aliesCnt;
                if (page.pos.alies != null && page.pos.alies.Length > 0)
                {
                    LinePos.Move(page.pos.alies[0], page.oldAlies);
                }
                else
                {
                    LinePos.Clear(page.oldAlies);
                }
            }


            if (caretPoint.Y == mml.line.Lp.row)
            {
                if (!caretPointChannels.Contains(pw)) caretPointChannels.Add(pw);
            }

            if (!SkipFlg && (
                (caretPoint.Y == mml.line.Lp.row && caretPoint.X <= mml.line.Lp.col + mml.column - 1)
                || (caretPoint.Y < mml.line.Lp.row && caretPointChannels.Contains(pw))
                ))
            {

                MML m = new MML();
                m.type = enmMMLType.SkipPlay;
                m.line = pw.getLine(page);
                m.column = pw.getPos(page);
                page.mmlData.Add(m);
                SkipFlg = true;

            }

            //コマンド解析

            switch (cmd)
            {
                case ' ':
                case '\t':
                    pw.incPos(page);
                    break;
                case '!': // CompileSkip
                    log.Write("CompileSkip");
                    page.dataEnd = true;
                    mml.type = enmMMLType.CompileSkip;
                    mml.args = null;
                    break;
                case '@': // instrument
                    log.Write("instrument");
                    CmdInstrument(pw, page, mml);
                    break;
                case '>': // octave Up
                    log.Write("octave Up");
                    CmdOctaveUp(pw, page, mml);
                    break;
                case '<': // octave Down
                    log.Write("octave Down");
                    CmdOctaveDown(pw, page, mml);
                    break;
                case ')': // volume Up
                    log.Write(" volume Up");
                    CmdVolumeUp(pw, page, mml);
                    break;
                case '(': // volume Down
                    log.Write("volume Down");
                    CmdVolumeDown(pw, page, mml);
                    break;
                case '#': // length(clock)
                    log.Write("length(clock)");
                    CmdClockLength(pw, page, mml);
                    break;
                case '[': // repeat
                    log.Write("repeat [");
                    CmdRepeatStart(pw, page, mml);
                    break;
                case ']': // repeat
                    log.Write("repeat ]");
                    CmdRepeatEnd(pw, page, mml);
                    break;
                case '{': // renpu
                    log.Write("renpu {");
                    CmdRenpuStart(pw, page, mml);
                    break;
                case '}': // renpu
                    log.Write("renpu }");
                    CmdRenpuEnd(pw, page, mml);
                    break;
                case '/': // repeat
                    log.Write("repeat /");
                    CmdRepeatExit(pw, page, mml);
                    break;
                case '"':
                    log.Write("lylic");
                    CmdLyric(pw, page, mml);
                    break;
                case '_':
                    log.Write("bend");
                    CmdBend(pw, page, mml);
                    break;
                case '&':
                    log.Write("tie");
                    CmdTie(pw, page, mml);
                    break;
                case '^':
                    log.Write("tie plus clock");
                    CmdTiePC(pw, page, mml);
                    break;
                case '~':
                    log.Write("tie minus clock");
                    CmdTieMC(pw, page, mml);
                    break;


                case 'A': // Address shift
                    log.Write("Address shift / Arpeggio");
                    CmdAddressShiftArpeggio(pw, page, mml);
                    break;
                case 'C': //MIDI Ch / Command Arpeggio
                    log.Write("MIDI Ch / Command Arpeggio");
                    CmdMIDIChCommandArpeggio(pw, page, mml);
                    break;
                case 'D': // Detune / ダイレクトモード
                    log.Write("Detune / DirectMode");
                    CmdDetuneDirectMode(pw, page, mml);
                    break;
                case 'E': // envelope / extendChannel
                    log.Write("envelope / extendChannel");
                    CmdE(pw, page, mml);
                    break;
                case 'J': // Jump point
                    log.Write("Jump point");
                    CmdJump(pw, page, mml);
                    break;
                case 'K': // key shift
                    log.Write("key shift");
                    CmdKeyShift(pw, page, mml);
                    break;
                case 'l': // length
                    log.Write("length");
                    CmdLength(pw, page, mml);
                    break;
                case 'L': // loop point
                    log.Write(" loop point");
                    CmdLoop(pw, page, mml);
                    break;
                case 'm': // pcm mode / pcm mapMode Sw
                    log.Write("pcm mode / pcm mapMode Sw");
                    CmdMode(pw, page, mml);
                    break;
                case 'M': // lfo
                    log.Write("lfo");
                    CmdLfo(pw, page, mml);
                    break;
                case 'o': // octave
                    log.Write("octave");
                    CmdOctave(pw, page, mml);
                    break;
                case 'p': // pan
                    log.Write(" pan");
                    CmdPan(pw, page, mml);
                    break;
                case 'P': // noise or tone mixer
                    log.Write("noise or tone mixer or phase reset");
                    CmdMixer(pw, page, mml);
                    break;
                case 'q': // gatetime
                    log.Write(" gatetime q");
                    CmdGatetime(pw, page, mml);
                    break;
                case 'Q': // gatetime
                    log.Write("gatetime Q");
                    CmdGatetime2(pw, page, mml);
                    break;
                case 's': // sus ON/OFF
                    log.Write("sus ON/OFF");
                    CmdSusOnOff(pw, page, mml);
                    break;
                case 'S': // lfo switch
                    log.Write(" lfo switch");
                    CmdLfoSwitch(pw, page, mml);
                    break;
                case 'T': // tempo
                    log.Write(" tempo");
                    CmdTempo(pw, page, mml);
                    break;
                case 'U': // velocity
                    log.Write("velocity");
                    CmdVelocity(pw, page, mml);
                    break;
                case 'v': // volume
                    log.Write("volume");
                    CmdVolume(pw, page, mml);
                    break;
                case 'V': // totalVolume(Adpcm-A / Rhythm)
                    log.Write("totalVolume(Adpcm-A / Rhythm)");
                    CmdTotalVolume(pw, page, mml);
                    break;
                case 'w': // noise
                    log.Write("noise");
                    CmdNoise(pw, page, mml);
                    break;
                case 'y': // y
                    log.Write(" y");
                    CmdY(pw, page, mml);
                    break;
                case 'X': // Effect
                    log.Write(" Effect");
                    CmdEffect(pw, page, mml);
                    break;
                case 'F'://Forced Fnum
                    log.Write("Forced Fnum");
                    CmdForcedFnum(pw, page, mml);
                    break;


                case 'c':
                case 'd':
                case 'e':
                case 'f':
                case 'g':
                case 'a':
                case 'b':
                    log.Write(string.Format("note {0}", cmd));
                    CmdNote(pw, page, cmd, mml);
                    break;
                case 'r':
                    log.Write("rest");
                    CmdRest(pw, page, mml);
                    break;
                case 'R':
                    log.Write("restNoWork");
                    CmdRestNoWork(pw, page, mml);
                    break;
                case ';':
                    log.Write("comment out");
                    CmdCommentout(pw, page, mml);
                    break;
                case '*':
                    log.Write("synchronous");
                    CmdSynchronous(pw, page, mml);
                    break;

                default:
                    msgBox.setErrMsg(string.Format(msg.get("E05000"), cmd), mml.line.Lp);
                    pw.incPos(page);
                    break;
            }

            //mmlコマンドの追加

            if (mml.type == enmMMLType.unknown) return;

            if (!mmlData.ContainsKey(page.PartName))//この処理無意味っぽい
            {//この処理無意味っぽい
                mmlData.Add(page.PartName, new List<MML>());//この処理無意味っぽい(mmlData使ってない)
            }//この処理無意味っぽい


            page.mmlData.Add(mml);


            if (swToneDoubler)
            {
                mml = new MML();
                mml.type = enmMMLType.ToneDoubler;
                mml.line = pw.getLine(page);
                mml.column = pw.getPos(page);
                page.mmlData.Add(mml);
                swToneDoubler = false;
            }
        }

        private void CmdTempo(partWork pw, partPage page, MML mml)
        {
            pw.incPos(page);
            if (!pw.getNum(page, out int n))
            {
                msgBox.setErrMsg(msg.get("E05001"), mml.line.Lp);
                n = 120;
            }
            n = Common.CheckRange(n, 1, 1200);

            mml.type = enmMMLType.Tempo;
            mml.args = new List<object>();
            mml.args.Add(n);
            mml.args.Add((int)desVGM.info.clockCount);
        }

        private void CmdVelocity(partWork pw, partPage page, MML mml)
        {
            pw.incPos(page);
            if (!pw.getNum(page, out int n))
            {
                msgBox.setErrMsg(msg.get("E05056"), mml.line.Lp);
                return;
            }
            n = Common.CheckRange(n, 0, 127);

            mml.type = enmMMLType.Velocity;
            mml.args = new List<object>();
            mml.args.Add(n);
        }

        private void CmdInstrument(partWork pw, partPage page, MML mml)
        {
            int n;
            pw.incPos(page);

            mml.type = enmMMLType.Instrument;
            mml.args = new List<object>();
            char a = pw.getChar(page);

            switch (a)
            {
                case 'T':                    //@Tn
                    //tone doubler
                    mml.args.Add('T');
                    pw.incPos(page);
                    break;
                case 'E':                    //@En
                    //Envelope
                    mml.args.Add('E');
                    pw.incPos(page);
                    break;
                case 'I':                    //@In
                    //Instrument(プリセット音色)
                    mml.args.Add('I');
                    pw.incPos(page);
                    break;
                case 'N':                    //@Nn
                    //None(音色設定直前に何もしない)
                    mml.args.Add('N');
                    pw.incPos(page);
                    break;
                case 'R':                    //@Rn
                    //RR(音色設定直前にRR=15をセットする)
                    mml.args.Add('R');
                    pw.incPos(page);
                    break;
                case 'A':                    //@An
                    //All send(音色設定直前に消音向け音色をセットする)
                    mml.args.Add('A');
                    pw.incPos(page);
                    break;
                case 'W':                    //@Wn,n,n,n
                    //波形データ(OPNA2専用)
                    mml.args.Add('W');
                    pw.incPos(page);
                    break;
                case 'S':                    //@Sn
                    //SysEx(MIDI系専用)
                    mml.args.Add('S');
                    pw.incPos(page);
                    break;
                case 'v':                    //@vn
                    mml.type = enmMMLType.RelativeVolumeSetting;
                    pw.incPos(page);
                    break;
                default:                     //normal
                    mml.args.Add('n');
                    break;
            }

            //数値取得
            if (a != 'W')
            {
                if (!pw.getNum(page, out n))
                {
                    if (mml.type == enmMMLType.Instrument) msgBox.setErrMsg(msg.get("E05002"), mml.line.Lp);
                    else msgBox.setErrMsg(msg.get("E05003"), mml.line.Lp);
                    n = 0;
                }
                n = Common.CheckRange(n, 0, 0xffff);
                mml.args.Add(n);

                if (
                        (
                        pw.cpg.Type == enmChannelType.FMOPN
                        || pw.cpg.Type == enmChannelType.FMOPL
                        || pw.cpg.Type == enmChannelType.FMOPM
                        || pw.cpg.Type == enmChannelType.FMOPNex
                        )
                    && (
                        a == 'N' 
                        || a == 'R'
                        || a == 'A'
                        || (mml.args[0] is char && (char)mml.args[0] == 'n')
                        )
                    )
                    mml.args.Add(desVGM.instFM[n].Item1);

                return;
            }

            //数値複数取得
            while (true)
            {
                if (pw.getChar(page) == 'r')
                {
                    pw.incPos(page);
                    mml.args.Add("reset");
                }
                else if (pw.getNum(page, out n))
                {
                    n = Common.CheckRange(n, 0, 4);
                    mml.args.Add(n);
                }
                else
                {
                    mml.args.Add(null);
                }

                pw.skipTabSpace(page);

                if (pw.getChar(page) != ',') break;
                pw.incPos(page);
            }
        }

        private void CmdVolume(partWork pw, partPage page, MML mml)
        {
            pw.incPos(page);
            mml.type = enmMMLType.Volume;
            if (pw.getNum(page, out int n))
            {
                n = Common.CheckRange(n, 0, page.MaxVolume);
                mml.args = new List<object>();
                mml.args.Add(n);
            }
            else
            {
                mml.args = null;
            }
        }

        private void CmdTotalVolume(partWork pw, partPage page, MML mml)
        {
            int n;
            pw.incPos(page);
            mml.type = enmMMLType.TotalVolume;
            mml.args = new List<object>();

            if (!pw.getNum(page, out n))
            {
                msgBox.setErrMsg(msg.get("E05004"), mml.line.Lp);
                n = 0;
            }
            mml.args.Add(n);

            pw.skipTabSpace(page);

            if (pw.getChar(page) == ',')
            {
                pw.incPos(page);
                if (!pw.getNum(page, out n))
                {
                    msgBox.setErrMsg(msg.get("E05004"), mml.line.Lp);
                    n = 0;
                }
                mml.args.Add(n);
            }

        }

        private void CmdOctave(partWork pw, partPage page, MML mml)
        {
            pw.incPos(page);
            mml.type = enmMMLType.Octave;
            if (pw.getNum(page, out int n))
            {
                n = Common.CheckRange(n, 1, 8);
                mml.args = new List<object>();
                mml.args.Add(n);
            }
            else
            {
                mml.args = null;
            }
        }

        private void CmdOctaveUp(partWork pw, partPage page, MML mml)
        {
            pw.incPos(page);
            mml.type = enmMMLType.OctaveUp;
            mml.args = null;
        }

        private void CmdOctaveDown(partWork pw, partPage page, MML mml)
        {
            pw.incPos(page);
            mml.type = enmMMLType.OctaveDown;
            mml.args = null;
        }

        private void CmdVolumeUp(partWork pw, partPage page, MML mml)
        {
            int n;
            pw.incPos(page);
            if (!pw.getNum(page, out n))
            {
                mml.type = enmMMLType.VolumeUp;
                mml.args = new List<object>();
                mml.args.Add(null);
                //msgBox.setErrMsg(msg.get("E05006"), mml.line.Lp);
                return;
            }
            n = Common.CheckRange(n, 1, page.MaxVolume);
            mml.type = enmMMLType.VolumeUp;
            mml.args = new List<object>();
            mml.args.Add(n);

        }

        private void CmdVolumeDown(partWork pw, partPage page, MML mml)
        {
            pw.incPos(page);
            if (!pw.getNum(page, out int n))
            {
                mml.type = enmMMLType.VolumeDown;
                mml.args = new List<object>();
                mml.args.Add(null);
                return;
                //msgBox.setErrMsg(msg.get("E05007"), mml.line.Lp);
                //n = 10;
            }
            n = Common.CheckRange(n, 1, page.MaxVolume);
            mml.type = enmMMLType.VolumeDown;
            mml.args = new List<object>();
            mml.args.Add(n);
        }

        private void CmdLength(partWork pw, partPage page, MML mml)
        {
            pw.incPos(page);
            //数値の解析
            if (pw.getNumNoteLength(page, out int n, out bool directFlg))
            {
                if (!directFlg)
                {
                    if ((int)info.clockCount % n != 0)
                    {
                        msgBox.setWrnMsg(string.Format(msg.get("E05008"), n), mml.line.Lp);
                    }
                    n = (int)info.clockCount / n;
                }
                else
                {
                    n = Common.CheckRange(n, 1, 65535);
                }

                //.の解析
                int futen = 0;
                int fn = n;
                while (pw.getChar(page) == '.')
                {
                    if (fn % 2 != 0)
                    {
                        msgBox.setWrnMsg(msg.get("E05036")
                            , mml.line.Lp);
                    }
                    fn = fn / 2;
                    futen += fn;
                    pw.incPos(page);
                }
                n += futen;

            }
            mml.type = enmMMLType.Length;
            mml.args = new List<object>();
            mml.args.Add(n);
            page.length = n;
        }

        private void CmdClockLength(partWork pw, partPage page, MML mml)
        {
            pw.incPos(page);
            if (!pw.getNum(page, out int n))
            {
                msgBox.setErrMsg(msg.get("E05009"), mml.line.Lp);
                n = 10;
            }
            n = Common.CheckRange(n, 1, 65535);
            mml.type = enmMMLType.LengthClock;
            mml.args = new List<object>();
            mml.args.Add(n);
        }

        private void CmdPan(partWork pw, partPage page, MML mml)
        {
            int n;

            pw.incPos(page);
            if (!pw.getNum(page, out n))
            {
                msgBox.setErrMsg(msg.get("E05010"), mml.line.Lp);
            }
            mml.type = enmMMLType.Pan;
            mml.args = new List<object>();
            mml.args.Add(n);

            for (int i = 0; i < 3; i++)
            {
                pw.skipTabSpace(page);
                if (pw.getChar(page) != ',') break;

                pw.incPos(page);
                if (!pw.getNum(page, out n))
                {
                    msgBox.setErrMsg(msg.get("E05010"), mml.line.Lp);
                }
                mml.args.Add(n);
            }
        }

        private void CmdDetuneDirectMode(partWork pw, partPage page, MML mml)
        {
            int n;
            pw.incPos(page);

            if (pw.getChar(page) == 'O')
            {
                //Direct Mode ?

                pw.incPos(page);
                if (pw.getChar(page) == 'N')
                {
                    //directMode ON
                    mml.type = enmMMLType.DirectMode;
                    mml.args = new List<object>();
                    mml.args.Add(true);
                    pw.incPos(page);
                    char vt = pw.getChar(page);
                    if (vt == 'V' || vt == 'T')
                    {
                        mml.args.Add(vt);
                        pw.incPos(page);
                    }
                }
                else if (pw.getChar(page) == 'F')
                {
                    //directMode OFF
                    mml.type = enmMMLType.DirectMode;
                    mml.args = new List<object>();
                    mml.args.Add(false);
                    pw.incPos(page);
                    char vt = pw.getChar(page);
                    if (vt == 'V' || vt == 'T')
                    {
                        mml.args.Add(vt);
                        pw.incPos(page);
                    }
                }
                else
                {
                    msgBox.setErrMsg(msg.get("E05054"), mml.line.Lp);
                }
            }
            else
            {
                //Detune 

                if (!pw.getNum(page, out n))
                {
                    msgBox.setErrMsg(msg.get("E05011"), mml.line.Lp);
                    n = 0;
                }

                mml.type = enmMMLType.Detune;
                mml.args = new List<object>();
                mml.args.Add(n);
            }
        }

        private void CmdMode(partWork pw, partPage page, MML mml)
        {
            int n;
            pw.incPos(page);

            if (pw.getChar(page) == 'o')
            {
                //pcm mapMode ?
                pw.incPos(page);
                if (pw.getChar(page) == 'n')
                {
                    mml.type = enmMMLType.PcmMap;
                    mml.args = new List<object>();
                    mml.args.Add(true);
                    pw.incPos(page);
                }
                else if (pw.getChar(page) == 'f')
                {
                    mml.type = enmMMLType.PcmMap;
                    mml.args = new List<object>();
                    mml.args.Add(false);
                    pw.incPos(page);
                }
                else
                {
                    msgBox.setErrMsg(msg.get("E05053"), mml.line.Lp);
                }
            }
            else
            {
                if (!pw.getNum(page, out n))
                {
                    msgBox.setErrMsg(msg.get("E05012"), mml.line.Lp);
                    n = 0;
                }
                mml.type = enmMMLType.PcmMode;
                mml.args = new List<object>();
                mml.args.Add(n);
            }
        }

        private void CmdGatetime(partWork pw, partPage page, MML mml)
        {
            int n;
            pw.incPos(page);
            if (!pw.getNum(page, out n))
            {
                msgBox.setErrMsg(msg.get("E05013"), mml.line.Lp);
                n = 0;
            }
            n = Common.CheckRange(n, 0, 255);
            mml.type = enmMMLType.Gatetime;
            mml.args = new List<object>();
            mml.args.Add(n);
        }

        private void CmdGatetime2(partWork pw, partPage page, MML mml)
        {
            int n;
            pw.incPos(page);
            if (!pw.getNum(page, out n))
            {
                msgBox.setErrMsg(msg.get("E05014"), mml.line.Lp);
                n = 1;
            }
            n = Common.CheckRange(n, 1, 8);
            mml.type = enmMMLType.GatetimeDiv;
            mml.args = new List<object>();
            mml.args.Add(n);
        }

        private void CmdE(partWork pw, partPage page, MML mml)
        {
            int n = -1;
            pw.incPos(page);
            switch (pw.getChar(page))
            {
                case 'O':
                    pw.incPos(page);
                    switch (pw.getChar(page))
                    {
                        case 'N':
                            pw.incPos(page);
                            mml.type = enmMMLType.Envelope;
                            mml.args = new List<object>();
                            mml.args.Add("EON");
                            break;
                        case 'F':
                            pw.incPos(page);
                            mml.type = enmMMLType.Envelope;
                            mml.args = new List<object>();
                            mml.args.Add("EOF");
                            break;
                        default:
                            msgBox.setErrMsg(string.Format(msg.get("E05015")
                                , pw.getChar(page))
                                , mml.line.Lp);
                            break;
                    }
                    break;
                case 'H':
                    pw.incPos(page);
                    n = -1;
                    if (pw.getChar(page) == 'O')
                    {
                        pw.incPos(page);
                        switch (pw.getChar(page))
                        {
                            case 'N':
                                pw.incPos(page);
                                mml.type = enmMMLType.HardEnvelope;
                                mml.args = new List<object>();
                                mml.args.Add("EHON");
                                break;
                            case 'F':
                                pw.incPos(page);
                                mml.type = enmMMLType.HardEnvelope;
                                mml.args = new List<object>();
                                mml.args.Add("EHOF");
                                break;
                            default:
                                msgBox.setErrMsg(string.Format(msg.get("E05016")
                                    , pw.getChar(page))
                                    , mml.line.Lp);
                                break;
                        }
                    }
                    else if (pw.getChar(page) == 'T')
                    {
                        pw.incPos(page);
                        mml.type = enmMMLType.HardEnvelope;
                        mml.args = new List<object>();
                        mml.args.Add("EHT");
                        if (!pw.getNum(page, out n))
                        {
                            msgBox.setErrMsg(msg.get("E05017")
                                , mml.line.Lp);
                            break;
                        }
                        mml.args.Add(n);
                        break;
                    }
                    else if (!pw.getNum(page, out n))
                    {
                        msgBox.setErrMsg(msg.get("E05018")
                                , mml.line.Lp);
                        n = 0;
                    }
                    if (n != -1)
                    {
                        mml.type = enmMMLType.HardEnvelope;
                        mml.args = new List<object>();
                        mml.args.Add("EH");
                        mml.args.Add(n);
                    }
                    break;
                case 'X':
                    pw.incPos(page);
                    n = -1;
                    if (pw.getChar(page) == 'O')
                    {
                        pw.incPos(page);
                        switch (pw.getChar(page))
                        {
                            case 'N':
                                pw.incPos(page);
                                mml.type = enmMMLType.ExtendChannel;
                                mml.args = new List<object>();
                                mml.args.Add("EXON");
                                break;
                            case 'F':
                                pw.incPos(page);
                                mml.type = enmMMLType.ExtendChannel;
                                mml.args = new List<object>();
                                mml.args.Add("EXOF");
                                break;
                            default:
                                msgBox.setErrMsg(string.Format(msg.get("E05019"), pw.getChar(page)), mml.line.Lp);
                                break;
                        }
                    }
                    else if (pw.getChar(page) == 'D')
                    {
                        pw.incPos(page);
                        mml.type = enmMMLType.ExtendChannel;
                        mml.args = new List<object>();
                        mml.args.Add("EXD");
                        for (int i = 0; i < 4; i++)
                        {
                            if (!pw.getNum(page, out n))
                            {
                                msgBox.setErrMsg(msg.get("E05020"), mml.line.Lp);
                                break;
                            }
                            mml.args.Add(n);
                            if (i == 3) break;
                            pw.incPos(page);
                        }
                        break;
                    }
                    else if (!pw.getNum(page, out n))
                    {
                        msgBox.setErrMsg(msg.get("E05021"), mml.line.Lp);
                        n = 0;
                    }
                    if (n != -1)
                    {
                        mml.type = enmMMLType.ExtendChannel;
                        mml.args = new List<object>();
                        mml.args.Add("EX");
                        mml.args.Add(n);
                    }
                    break;
                default:
                    mml.type = enmMMLType.Envelope;
                    mml.args = new List<object>();
                    for (int i = 0; i < 4; i++)
                    {
                        if (pw.getNum(page, out n))
                        {
                            mml.args.Add(n);
                        }
                        else
                        {
                            msgBox.setErrMsg(msg.get("E05022"), mml.line.Lp);
                            break;
                        }
                        if (i == 3) break;
                        pw.incPos(page);
                    }
                    break;
            }

        }

        private void CmdLoop(partWork pw, partPage page, MML mml)
        {
            pw.incPos(page);
            mml.type = enmMMLType.LoopPoint;
            mml.args = null;
        }

        private void CmdJump(partWork pw, partPage page, MML mml)
        {
            pw.incPos(page);
            mml.type = enmMMLType.JumpPoint;
            mml.args = null;
            //desVGM.jumpCommandCounter++;
            //desVGM.useJumpCommand = true;
        }

        private void CmdRepeatStart(partWork pw, partPage page, MML mml)
        {
            pw.incPos(page);
            mml.type = enmMMLType.Repeat;
            mml.args = null;
        }

        private void CmdRepeatEnd(partWork pw, partPage page, MML mml)
        {
            pw.incPos(page);
            if (!pw.getNum(page, out int n))
            {
                n = 2;
            }
            n = Common.CheckRange(n, 1, 255);
            mml.type = enmMMLType.RepeatEnd;
            mml.args = new List<object>();
            mml.args.Add(n);
        }

        private void CmdRenpuStart(partWork pw, partPage page, MML mml)
        {
            pw.incPos(page);
            mml.type = enmMMLType.Renpu;
            mml.args = null;
        }

        private void CmdRenpuEnd(partWork pw, partPage page, MML mml)
        {
            pw.incPos(page);
            mml.type = enmMMLType.RenpuEnd;
            mml.args = null;
            if (pw.getNumNoteLength(page, out int n, out bool directFlg))
            {
                if (!directFlg)
                {
                    if ((int)info.clockCount % n != 0)
                    {
                        msgBox.setWrnMsg(string.Format(msg.get("E05023"), n), mml.line.Lp);
                    }
                    n = (int)info.clockCount / n;
                }
                else
                {
                    n = Common.CheckRange(n, 1, 65535);
                }

                mml.args = new List<object>();
                mml.args.Add(n);
            }
            else
            {
                mml.args = new List<object>();
                mml.args.Add((int)page.length);
            }
        }

        private void CmdRepeatExit(partWork pw, partPage page, MML mml)
        {
            pw.incPos(page);
            mml.type = enmMMLType.RepertExit;
            mml.args = null;
        }

        private void CmdLfo(partWork pw, partPage page, MML mml)
        {
            int n;
            pw.incPos(page);
            char c = pw.getChar(page);

            if (c == 'A')
            {
                pw.incPos(page);
                char d = pw.getChar(page);
                if (d == 'M')
                {
                    pw.incPos(page);
                    d = pw.getChar(page);
                    if (d == 'S')
                    {
                        pw.incPos(page);
                        if (!pw.getNum(page, out n))
                        {
                            msgBox.setErrMsg(msg.get("E05024"), mml.line.Lp);
                            return;
                        }
                        mml.type = enmMMLType.Lfo;
                        mml.args = new List<object>();
                        mml.args.Add("MAMS");
                        mml.args.Add(n);
                        return;
                    }
                }
                msgBox.setErrMsg(msg.get("E05025"), mml.line.Lp);
                return;
            }

            if (c < 'P' && c > 'S')
            {
                msgBox.setErrMsg(msg.get("E05026"), mml.line.Lp);
                return;
            }

            pw.incPos(page);
            char t = pw.getChar(page);

            if (c == 'P' && t == 'M')
            {
                pw.incPos(page);
                char d = pw.getChar(page);
                if (d == 'S')
                {
                    pw.incPos(page);
                    if (!pw.getNum(page, out n))
                    {
                        msgBox.setErrMsg(msg.get("E05027"), mml.line.Lp);
                        return;
                    }
                    mml.type = enmMMLType.Lfo;
                    mml.args = new List<object>();
                    mml.args.Add("MPMS");
                    mml.args.Add(n);
                    return;
                }
                msgBox.setErrMsg(msg.get("E05028"), mml.line.Lp);
                return;
            }

            if (t != 'T' && t != 'V' && t != 'H')
            {
                msgBox.setErrMsg(msg.get("E05029"), mml.line.Lp);
                return;
            }

            mml.type = enmMMLType.Lfo;
            mml.args = new List<object>();
            mml.args.Add(c);
            mml.args.Add(t);

            n = -1;
            do
            {
                pw.incPos(page);
                if (pw.getNum(page, out n))
                {
                    mml.args.Add(n);
                }
                else
                {
                    msgBox.setErrMsg(msg.get("E05030"), mml.line.Lp);
                    return;
                }

                pw.skipTabSpace(page);

            } while (pw.getChar(page) == ',');

        }

        private void CmdLfoSwitch(partWork pw, partPage page, MML mml)
        {

            pw.incPos(page);
            char c = pw.getChar(page);
            if (c < 'P' || c > 'S')
            {
                msgBox.setErrMsg(msg.get("E05031"), mml.line.Lp);
                pw.incPos(page);
                return;
            }

            int n = -1;
            pw.incPos(page);
            if (!pw.getNum(page, out n))
            {
                msgBox.setErrMsg(msg.get("E05032"), mml.line.Lp);
                return;
            }
            n = Common.CheckRange(n, 0, 2);

            mml.type = enmMMLType.LfoSwitch;
            mml.args = new List<object>();
            mml.args.Add(c);
            mml.args.Add(n);
        }

        private void CmdSusOnOff(partWork pw, partPage page, MML mml)
        {
            pw.incPos(page);
            char c = pw.getChar(page);
            pw.incPos(page);
            if (c != 'o' && c != 'f')
            {
                msgBox.setErrMsg(msg.get("E05031"), mml.line.Lp);
                return;
            }

            mml.type = enmMMLType.SusOnOff;
            mml.args = new List<object>();
            mml.args.Add(c);
        }

        private void CmdForcedFnum(partWork pw, partPage page, MML mml)
        {
            pw.incPos(page);
            if (!pw.getNum(page, out int num))
            {
                msgBox.setErrMsg(msg.get("E05065"), mml.line.Lp);
                return;
            }

            //Fコマンドは数値でオンオフする...
            if (num != 0 && num != 1)
            {
                msgBox.setErrMsg(msg.get("E05065"), mml.line.Lp);
                return;
            }

            mml.type = enmMMLType.ForcedFnum;
            mml.args = new List<object>();
            mml.args.Add(num);

            //1(ON)の場合は第２引数(固定するfnum値)を参照する
            if (num == 0) return;

            pw.incPos(page);//デリミタは何でもいいけど','を推奨
            pw.skipTabSpace(page);
            if (!pw.getNumInt16(page, out num))
            {
                msgBox.setErrMsg(msg.get("E05065"), mml.line.Lp);
                return;
            }
            num = Common.CheckRange((int)(UInt16)num, 0, 0xffff);
            mml.args.Add(num);

        }

        private void CmdEffect(partWork pw, partPage page, MML mml)
        {
            pw.incPos(page);
            pw.skipTabSpace(page);

            char c = pw.getChar(page);
            if (c == 'R')
            {
                CmdEffectReverb(pw, page, mml);
                return;
            }
            else if (c == 'D')
            {
                CmdEffectDistortion(pw, page, mml);
                return;
            }
            else if (c == 'C')
            {
                CmdEffectChorus(pw, page, mml);
                return;
            }

            msgBox.setErrMsg(msg.get("E05059"), mml.line.Lp);
        }

        private void CmdEffectReverb(partWork pw, partPage page, MML mml)
        {
            pw.incPos(page);
            char c = pw.getChar(page);
            if (c != 'v')
            {
                msgBox.setErrMsg(msg.get("E05059"), mml.line.Lp);
                return;
            }

            mml.type = enmMMLType.Effect;
            mml.args = new List<object>();
            mml.args.Add("Rv");

            pw.incPos(page);
            pw.skipTabSpace(page);

            c = pw.getChar(page);
            if (c != 'D' && c != 'S')
            {
                msgBox.setErrMsg(msg.get("E05059"), mml.line.Lp);
                return;
            }

            mml.args.Add(c);

            int n = -1;
            pw.incPos(page);
            pw.skipTabSpace(page);
            if (!pw.getNum(page, out n))
            {
                msgBox.setErrMsg(msg.get("E05059"), mml.line.Lp);
                return;
            }

            if (c == 'D')
            {
                n = Common.CheckRange(n, 0, 127);
            }
            else
            {
                n = Common.CheckRange(n, 0, 15);
            }
            mml.args.Add(n);

        }

        private void CmdEffectDistortion(partWork pw, partPage page, MML mml)
        {
            pw.incPos(page);
            char c = pw.getChar(page);
            if (c != 's')
            {
                msgBox.setErrMsg(msg.get("E05059"), mml.line.Lp);
                return;
            }

            mml.type = enmMMLType.Effect;
            mml.args = new List<object>();
            mml.args.Add("Ds");

            pw.incPos(page);
            pw.skipTabSpace(page);

            c = pw.getChar(page);
            if (c != 'C' && c != 'G' && c != 'V' && c != 'S')
            {
                msgBox.setErrMsg(msg.get("E05059"), mml.line.Lp);
                return;
            }

            mml.args.Add(c);

            int n = -1;
            pw.incPos(page);
            pw.skipTabSpace(page);
            if (!pw.getNum(page, out n))
            {
                msgBox.setErrMsg(msg.get("E05059"), mml.line.Lp);
                return;
            }

            if (c == 'S')//switch
            {
                n = Common.CheckRange(n, 0, 1);
            }
            else if (c == 'V')//volume
            {
                n = Common.CheckRange(n, 0, 127);
            }
            else if (c == 'G')//gain
            {
                n = Common.CheckRange(n, 0, 127);
            }
            else if (c == 'C')//cutoff
            {
                n = Common.CheckRange(n, 0, 127);
            }
            mml.args.Add(n);

        }

        private void CmdEffectChorus(partWork pw, partPage page, MML mml)
        {
            pw.incPos(page);
            char c = pw.getChar(page);
            if (c != 'h')
            {
                msgBox.setErrMsg(msg.get("E05059"), mml.line.Lp);
                return;
            }

            mml.type = enmMMLType.Effect;
            mml.args = new List<object>();
            mml.args.Add("Ch");

            pw.incPos(page);
            pw.skipTabSpace(page);

            c = pw.getChar(page);
            if (c != 'S' && c != 'M' && c != 'R' && c != 'D' && c != 'F')
            {
                msgBox.setErrMsg(msg.get("E05059"), mml.line.Lp);
                return;
            }

            mml.args.Add(c);

            int n = -1;
            pw.incPos(page);
            pw.skipTabSpace(page);
            if (!pw.getNum(page, out n))
            {
                msgBox.setErrMsg(msg.get("E05059"), mml.line.Lp);
                return;
            }

            if (c == 'S')//switch
            {
                n = Common.CheckRange(n, 0, 1);
            }
            else if (c == 'M')//mix level
            {
                n = Common.CheckRange(n, 0, 127);
            }
            else if (c == 'R')//rate
            {
                n = Common.CheckRange(n, 0, 127);
            }
            else if (c == 'D')//depth
            {
                n = Common.CheckRange(n, 0, 127);
            }
            else if (c == 'F')//feedback
            {
                n = Common.CheckRange(n, 0, 127);
            }
            mml.args.Add(n);

        }

        private void CmdY(partWork pw, partPage page, MML mml)
        {
            int n = -1;
            int dat = 0;
            int op = 0;
            pw.incPos(page);

            char c = pw.getChar(page);
            if (c >= 'A' && c <= 'Z')
            {
                string toneparamName = "" + c;
                pw.incPos(page);
                toneparamName += pw.getChar(page);
                pw.incPos(page);
                if (toneparamName != "TL" && toneparamName != "SR")
                {
                    toneparamName += pw.getChar(page);
                    pw.incPos(page);
                    if (toneparamName != "SSG")
                    {
                        toneparamName += pw.getChar(page);
                        pw.incPos(page);
                    }
                }

                if (toneparamName == "DT1M" || toneparamName == "DT2S" || toneparamName == "PMSA")
                {
                    toneparamName += pw.getChar(page);
                    pw.incPos(page);
                    if (toneparamName == "PMSAM")
                    {
                        toneparamName += pw.getChar(page);
                        pw.incPos(page);
                    }
                }

                pw.incPos(page);

                if (toneparamName != "FBAL" && toneparamName != "PMSAMS")
                {
                    if (pw.getNum(page, out n))
                    {
                        op = (byte)(Common.CheckRange(n & 0xff, 1, 4) - 1);
                    }
                    pw.incPos(page);
                }

                if (pw.getNum(page, out n))
                {
                    dat = (byte)(n & 0xff);
                }

                mml.type = enmMMLType.Y;
                mml.args = new List<object>();
                mml.args.Add(toneparamName);
                mml.args.Add(op);
                mml.args.Add(dat);
                return;
            }

            mml.type = enmMMLType.Y;
            mml.args = new List<object>();
            do
            {
                if (!pw.getNum(page, out n)) break;

                //dat = (byte)(n & 0xff);
                mml.args.Add(n);
                pw.skipTabSpace(page);

                if (pw.getChar(page) != ',') break;

                pw.incPos(page);
            } while (true);

        }

        private void CmdNoise(partWork pw, partPage page, MML mml)
        {
            int n = -1;
            pw.incPos(page);

            char c = pw.getChar(page);
            if (c == 'f')
            {
                pw.incPos(page);

                if (!pw.getNum(page, out n))
                {
                    msgBox.setErrMsg(msg.get("E05058"), mml.line.Lp);
                    return;

                }

                mml.type = enmMMLType.DCSGCh3Freq;
                mml.args = new List<object>();
                mml.args.Add(n);
                return;
            }

            if (!pw.getNum(page, out n))
            {
                msgBox.setErrMsg(msg.get("E05033"), mml.line.Lp);
                return;

            }
            mml.type = enmMMLType.Noise;
            mml.args = new List<object>();
            mml.args.Add(n);
        }

        private void CmdMixer(partWork pw, partPage page, MML mml)
        {
            int n = -1;
            pw.incPos(page);
            if (pw.getChar(page) == 'R') //PR
            {
                pw.incPos(page);
                if (pw.getChar(page) == 'O') //PRO
                {
                    pw.incPos(page);
                    if (pw.getChar(page) == 'N') //PRON
                    {
                        pw.incPos(page);
                        mml.type = enmMMLType.PhaseReset;
                        mml.args = new List<object>();
                        mml.args.Add("PRON");
                    }
                    else if (pw.getChar(page) == 'F') //PROF
                    {
                        pw.incPos(page);
                        mml.type = enmMMLType.PhaseReset;
                        mml.args = new List<object>();
                        mml.args.Add("PROF");
                    }
                }
                else
                {
                    msgBox.setErrMsg(msg.get("E05064"), mml.line.Lp);
                }
                return;
            }

            if (!pw.getNum(page, out n))
            {
                msgBox.setErrMsg(msg.get("E05034"), mml.line.Lp);
                return;

            }
            mml.type = enmMMLType.NoiseToneMixer;
            mml.args = new List<object>();
            mml.args.Add(n);

        }

        private void CmdKeyShift(partWork pw, partPage page, MML mml)
        {
            int n = -1;
            pw.incPos(page);

            if (!pw.getNum(page, out n))
            {
                msgBox.setErrMsg(msg.get("E05035"), mml.line.Lp);
                return;

            }
            mml.type = enmMMLType.KeyShift;
            mml.args = new List<object>();
            mml.args.Add(n);
        }

        private void CmdAddressShiftArpeggio(partWork pw, partPage page, MML mml)
        {
            pw.incPos(page);
            if (pw.getChar(page) != 'P') //aP
            {
                CmdAddressShift(pw, page, mml);
                return;
            }

            CmdArpeggio(pw, page, mml);
        }

        private void CmdAddressShift(partWork pw, partPage page, MML mml)
        {
            int n;
            int sign = 0;

            if (pw.getChar(page) == '+')
            {
                sign = 1;
                pw.incPos(page);
            }
            else if (pw.getChar(page) == '-')
            {
                sign = -1;
                pw.incPos(page);
            }

            if (!pw.getNum(page, out n))
            {
                msgBox.setErrMsg(msg.get("E05052"), mml.line.Lp);
                return;

            }
            mml.type = enmMMLType.AddressShift;
            mml.args = new List<object>();
            mml.args.Add(sign);
            mml.args.Add(n);
        }

        private void CmdArpeggio(partWork pw, partPage page, MML mml)
        {
            pw.incPos(page);
            if (pw.getChar(page) == 'O') //apO
            {
                pw.incPos(page);
                if (pw.getChar(page) == 'N') //apoN
                {
                    pw.incPos(page);
                    mml.type = enmMMLType.Arpeggio;
                    mml.args = new List<object>();
                    mml.args.Add("APON");
                }
                else if (pw.getChar(page) == 'F') //apoN
                {
                    pw.incPos(page);
                    mml.type = enmMMLType.Arpeggio;
                    mml.args = new List<object>();
                    mml.args.Add("APOF");
                }
                else
                {
                    msgBox.setErrMsg(msg.get("E05062"), mml.line.Lp);
                }
            }
            else
            {
                //APn
                if (!pw.getNum(page, out int n))
                {
                    msgBox.setErrMsg(msg.get("E05063"), mml.line.Lp);
                }
                else
                {
                    mml.type = enmMMLType.Arpeggio;
                    mml.args = new List<object>();
                    mml.args.Add(n);
                }
            }
        }

        private void CmdMIDIChCommandArpeggio(partWork pw, partPage page, MML mml)
        {
            pw.incPos(page);
            if (pw.getChar(page) != 'A') //cA
            {
                CmdMIDICh(pw, page, mml);
                return;
            }

            CmdCommandArpeggio(pw, page, mml);
        }

        /// <summary>
        /// MIDI CH
        /// </summary>
        /// <remarks>
        /// コマンド
        ///   CHn
        /// パラメーター
        ///   n  :  1～16
        /// 内容
        ///   パートに割り当てるMIDI Channelを設定する
        /// </remarks>
        private void CmdMIDICh(partWork pw, partPage page, MML mml)
        {
            int n = -1;

            //pw.incPos(page);

            if (pw.getChar(page) == 'H')
            {
                pw.incPos(page);

                if (!pw.getNum(page, out n))
                {
                    msgBox.setErrMsg(msg.get("E05055"), mml.line.Lp);
                    return;
                }
                n = Common.CheckRange(n, 1, 16) - 1;//0-15に変換

                mml.type = enmMMLType.MIDICh;
                mml.args = new List<object>();
                mml.args.Add(n);
            }
            else if (pw.getChar(page) == 'C')
            {
                mml.type = enmMMLType.MIDIControlChange;
                mml.args = new List<object>();

                pw.incPos(page);
                pw.skipTabSpace(page);

                if (!pw.getNum(page, out n))
                {
                    mml.type = enmMMLType.unknown;
                    msgBox.setErrMsg(msg.get("E05057"), mml.line.Lp);
                    return;
                }
                mml.args.Add(n);

                pw.skipTabSpace(page);
                if (pw.getChar(page) != ',')
                {
                    mml.type = enmMMLType.unknown;
                    msgBox.setErrMsg(msg.get("E05057"), mml.line.Lp);
                    return;
                }
                pw.incPos(page);
                pw.skipTabSpace(page);

                if (!pw.getNum(page, out n))
                {
                    mml.type = enmMMLType.unknown;
                    msgBox.setErrMsg(msg.get("E05057"), mml.line.Lp);
                    return;
                }
                mml.args.Add(n);

            }
            else
            {
                msgBox.setErrMsg(msg.get("E05055"), mml.line.Lp);
                return;
            }
        }

        private void CmdCommandArpeggio(partWork pw, partPage page, MML mml)
        {
            pw.incPos(page);
            if (pw.getChar(page) == 'O') //caO
            {
                pw.incPos(page);
                if (pw.getChar(page) == 'N') //caoN
                {
                    pw.incPos(page);
                    mml.type = enmMMLType.Arpeggio;
                    mml.args = new List<object>();
                    mml.args.Add("CAON");
                    //CAONn
                    if (!pw.getNum(page, out int n)) msgBox.setErrMsg(msg.get("E05067"), mml.line.Lp);
                    else mml.args.Add(n);
                }
                else if (pw.getChar(page) == 'F') //caoF
                {
                    pw.incPos(page);
                    mml.type = enmMMLType.Arpeggio;
                    mml.args = new List<object>();
                    mml.args.Add("CAOF");
                    //CAOFn
                    if (!pw.getNum(page, out int n)) msgBox.setErrMsg(msg.get("E05067"), mml.line.Lp);
                    else mml.args.Add(n);
                }
                else
                {
                    msgBox.setErrMsg(msg.get("E05066"), mml.line.Lp);
                }
            }
            else
            {
                //CAn1,n2
                mml.type = enmMMLType.Arpeggio;
                mml.args = new List<object>();
                mml.args.Add("CA");

                int n;
                do
                {
                    if (!pw.getNum(page, out n)) break;
                    mml.args.Add(n);
                    pw.skipTabSpace(page);
                    if (pw.getChar(page) != ',') break;
                    pw.incPos(page);
                } while (true);
            }
        }

        //private void CmdNote(partWork pw, char cmd, MML mml,int page)
        //{
        //    pw.incPos(page);
        //    mml.line.Lp.length = 1;
        //    mml.type = enmMMLType.Note;
        //    mml.args = new List<object>();
        //    Note note = new Note();
        //    mml.args.Add(note);
        //    note.cmd = cmd;

        //    //+ -の解析
        //    int shift = 0;
        //    while (pw.getChar(page) == '+' || pw.getChar(page) == '-')
        //    {
        //        if (pw.getChar(page) == '+')
        //            shift++;
        //        else
        //            shift--;
        //        pw.incPos(page);
        //        mml.line.Lp.length++;
        //    }
        //    note.shift = shift;

        //    int n = -1;
        //    bool directFlg = false;
        //    int col = 0;
        //    bool veloCheckSkip = false;

        //    //数値の解析
        //    if (pw.getNumNoteLength(out n, out directFlg,out col))
        //    {
        //        if (!directFlg)
        //        {
        //            if (n != 0)
        //            {
        //                if ((int)info.clockCount % n != 0)
        //                {
        //                    msgBox.setWrnMsg(string.Format(msg.get("E05023"), n), mml.line.Lp);
        //                }
        //                n = (int)info.clockCount / n;
        //            }
        //        }
        //        else
        //        {
        //            n = Common.CheckRange(n, 1, 65535);
        //        }

        //        note.length = n;
        //        mml.line.Lp.length += col;

        //        //ToneDoubler'0'指定の場合はここで解析終了
        //        if (n == 0)
        //        {
        //            swToneDoubler = true;
        //            return;
        //        }
        //    }
        //    else
        //    {
        //        note.length = (int)pw.ppg[pw.cpgNum].length;
        //        pw.skipTabSpace(page);
        //        if (pw.getChar(page) == ',')
        //        {
        //            pw.incPos(page);
        //            pw.skipTabSpace(page);
        //            if (!pw.getNum(page,out n))
        //            {
        //                //Tone Doubler','指定の場合はここで解析終了
        //                //ToneDoublerは数値ではないはず
        //                mml.line.Lp.length++;
        //                swToneDoubler = true;
        //                return;
        //            }
        //            else
        //            {
        //                note.velocity = Common.CheckRange(n, 0, 127);
        //                veloCheckSkip = true;
        //            }
        //        }
        //    }

        //    //.の解析
        //    int futen = 0;
        //    int fn = note.length;
        //    while (pw.getChar(page) == '.')
        //    {
        //        if (fn % 2 != 0)
        //        {
        //            msgBox.setWrnMsg(msg.get("E05036")
        //                , mml.line.Lp);
        //        }
        //        fn = fn / 2;
        //        futen += fn;
        //        pw.incPos(page);
        //        mml.line.Lp.length++;
        //    }
        //    note.length += futen;

        //    pw.skipTabSpace(page);

        //    //ベロシティ解析
        //    if (!veloCheckSkip && pw.getChar(page) == ',')
        //    {
        //        pw.incPos(page);
        //        pw.skipTabSpace(page);
        //        if (pw.getNum(page,out n))
        //        {
        //            note.velocity = Common.CheckRange(n, 0, 127);
        //        }
        //    }

        //    pw.skipTabSpace(page);

        //    //和音装飾解析
        //    if (pw.getChar(page) == ':')
        //    {
        //        pw.incPos(page);
        //        pw.skipTabSpace(page);

        //        note.chordSw = true;
        //    }
        //}

        //
        //  c + 0 99 .. ^99 &99 ~99 ,99 :
        //  1 2 3  4  5   6   7   8 9   10
        //
        // 1... c d e f g a b 音符
        // 2... + -           半音上げる/下げる(複数可能) 
        // 3... 0             ToneDoubler
        // 4... n             音長($は16進 #はクロック表記 #$は16進のクロック表記)
        // 5... .             符点(複数可能)
        // 6... ^n..          音長を加算($は16進 #はクロック表記 #$は16進のクロック表記)符点も可能
        // 7... &n..          音長を加算($は16進 #はクロック表記 #$は16進のクロック表記)符点も可能
        // 8... ~n..          音長を減算($は16進 #はクロック表記 #$は16進のクロック表記)符点も可能
        //  6,7,8 は複数繰り返し指定可能。順番も自由
        // 9... ,c            コンマの後が音符ならばToneDoubler
        //                    数値の場合はベロシティ
        //10... :             和音指定(ウエイトキャンセル)
        private void CmdNote(partWork pw, partPage page, char cmd, MML mml)
        {
            pw.incPos(page);
            mml.line.Lp.length = 1;
            mml.type = enmMMLType.Note;
            mml.args = new List<object>();
            Note note = new Note();
            mml.args.Add(note);
            note.cmd = cmd;

            pw.skipTabSpace(page);

            //+ -の解析
            int shift = 0;
            while (pw.getChar(page) == '+' || pw.getChar(page) == '-')
            {
                if (pw.getChar(page) == '+')
                    shift++;
                else
                    shift--;
                pw.incPos(page);
                mml.line.Lp.length++;
            }
            note.shift = shift;

            int n;// = -1;
            bool directFlg;// = false;
            int col;// = 0;

            //数値の解析
            if (pw.getNumNoteLength(page, out n, out directFlg, out col))
            {
                if (!directFlg)
                {
                    if (n != 0)
                    {
                        if ((int)info.clockCount % n != 0)
                        {
                            msgBox.setWrnMsg(string.Format(msg.get("E05023"), n), mml.line.Lp);
                        }
                        n = (int)info.clockCount / n;
                    }
                }
                else
                {
                    n = Common.CheckRange(n, 1, 65535);
                }

                note.length = n;
                mml.line.Lp.length += col;

                //ToneDoubler'0'指定の場合はここで解析終了
                if (n == 0)
                {
                    swToneDoubler = true;
                    return;
                }
            }
            else
            {

                //数値未指定の場合はlコマンドでの設定値を使用する
                note.length = (int)page.length;

                pw.skipTabSpace(page);

            }

            //.の解析
            note.length += CountFuten(pw, page, mml, note.length);

            pw.skipTabSpace(page);

            //& ^ ~ コマンドの解析
            int len = 0;
            while (pw.getChar(page) == '&' || pw.getChar(page) == '^' || pw.getChar(page) == '~')
            {
                char ch = pw.getChar(page);
                int oldPos = pw.getPos(page);
                pw.incPos(page);
                if (ch == '&')
                {
                    if (pw.getNumNoteLength(page, out n, out directFlg))
                    {
                        if (!directFlg)
                        {
                            if ((int)info.clockCount % n != 0)
                            {
                                msgBox.setWrnMsg(string.Format(msg.get("E05023")
                                    , n), mml.line.Lp);
                            }
                            n = (int)info.clockCount / n;
                        }
                        else
                        {
                            n = Common.CheckRange(n, 1, 65535);
                        }
                    }
                    else
                    {
                        //数値ではなかった
                        int nowPos = pw.getPos(page);
                        pw.decPos(page, nowPos - oldPos);
                        break;
                    }
                    n += CountFuten(pw, page, mml, n);

                }
                else if (ch == '^')
                {
                    GetLength(pw, page, mml, out n);
                }
                else if (ch == '~')
                {
                    GetLength(pw, page, mml, out n);
                    n = -n;
                }

                len += n;
            }

            note.length += len;
            note.addLength = len;
            pw.skipTabSpace(page);
            //

            //,はToneDoublerかベロシティのどちらか。
            if (pw.getChar(page) == ',')
            {
                pw.incPos(page);
                pw.skipTabSpace(page);
                if (!pw.getNum(page, out n))
                {
                    //Tone Doubler','指定の場合はここで解析終了
                    //ToneDoublerは数値ではないはず
                    mml.line.Lp.length++;
                    swToneDoubler = true;
                    return;
                }
                else
                {
                    note.velocity = Common.CheckRange(n, 0, 127);
                }
            }

            pw.skipTabSpace(page);

            //和音装飾解析
            if (pw.getChar(page) == ':')
            {
                pw.incPos(page);
                pw.skipTabSpace(page);

                note.chordSw = true;
            }

        }

        private void GetLength(partWork pw, partPage page, MML mml, out int n)
        {
            if (pw.getNumNoteLength(page, out n, out bool directFlg))
            {
                if (!directFlg)
                {
                    if ((int)info.clockCount % n != 0)
                    {
                        msgBox.setWrnMsg(string.Format(msg.get("E05023")
                            , n), mml.line.Lp);
                    }
                    n = (int)info.clockCount / n;
                }
                else
                {
                    n = Common.CheckRange(n, 1, 65535);
                }
            }
            else
            {
                //数値未指定の場合はlコマンドでの設定値を使用する
                n = (int)page.length;
            }
            n += CountFuten(pw, page, mml, n);
        }

        private int CountFuten(partWork pw, partPage page, MML mml, int noteLength)
        {
            int futen = 0;
            int fn = noteLength;
            while (pw.getChar(page) == '.')
            {
                if (fn % 2 != 0)
                {
                    msgBox.setWrnMsg(msg.get("E05036")
                        , mml.line.Lp);
                }
                fn = fn / 2;
                futen += fn;
                pw.incPos(page);
                mml.line.Lp.length++;
            }

            return futen;
        }

        private void CmdCommentout(partWork pw, partPage page, MML mml)
        {

            int row = page.pos.row;
            char ch;
            string alies = (page.pos.alies == null || page.pos.alies.Length<1) ? "" : page.pos.alies[0].aliesNextName;
            do
            {
                pw.incPos(page);
                ch = pw.getChar(page);
            } while (
                (row == page.pos.row && 
                    (
                        (
                            alies == "" && 
                            (page.pos.alies == null || page.pos.alies.Length < 1)
                        ) 
                        || alies == page.pos.alies[0].aliesNextName
                    )
                ) 
                && ch != (char)0
            );

        }

        private void CmdSynchronous(partWork pw, partPage page, MML mml)
        {
            pw.incPos(page);
            char c = pw.getChar(page);
            int m = 0;
            int n;

            if (c == 'M')
            {
                m = 1;
            }
            else if (c == 'S')
            {
                m = 2;
            }
            else
            {
                msgBox.setErrMsg(msg.get("E05060"), mml.line.Lp);
                return;
            }

            pw.incPos(page);
            pw.skipTabSpace(page);

            if (!pw.getNum(page, out n))
            {
                n = -1;
            }

            mml.type = enmMMLType.Synchronous;
            mml.args = new List<object>();
            mml.args.Add(m);
            mml.args.Add(n);

        }

        private void CmdRest(partWork pw, partPage page, MML mml)
        {
            pw.incPos(page);
            mml.line.Lp.length = 1;
            mml.type = enmMMLType.Rest;
            mml.args = new List<object>();
            Rest rest = new Rest();
            mml.args.Add(rest);

            rest.cmd = 'r';

            int n = -1;
            bool directFlg = false;
            int col = 0;

            //数値の解析
            if (pw.getNumNoteLength(page, out n, out directFlg, out col))
            {
                if (!directFlg)
                {
                    if ((int)info.clockCount % n != 0)
                    {
                        msgBox.setWrnMsg(string.Format(msg.get("E05023"), n), mml.line.Lp);
                    }
                    n = (int)info.clockCount / n;
                }
                else
                {
                    n = Common.CheckRange(n, 1, 65535);
                }

                rest.length = n;
                mml.line.Lp.length += col;

            }
            else
            {
                rest.length = (int)page.length;
            }

            //.の解析
            int futen = 0;
            int fn = rest.length;
            while (pw.getChar(page) == '.')
            {
                if (fn % 2 != 0)
                {
                    msgBox.setWrnMsg(msg.get("E05036")
                        , mml.line.Lp);
                }
                fn = fn / 2;
                futen += fn;
                pw.incPos(page);
                mml.line.Lp.length++;
            }
            rest.length += futen;

        }

        private void CmdRestNoWork(partWork pw, partPage page, MML mml)
        {
            pw.incPos(page);
            mml.line.Lp.length = 1;
            mml.type = enmMMLType.Rest;
            mml.args = new List<object>();
            Rest rest = new Rest();
            mml.args.Add(rest);

            rest.cmd = 'R';

            int n = -1;
            bool directFlg = false;
            int col = 0;

            //数値の解析
            if (pw.getNumNoteLength(page, out n, out directFlg, out col))
            {
                if (!directFlg)
                {
                    if ((int)info.clockCount % n != 0)
                    {
                        msgBox.setWrnMsg(string.Format(msg.get("E05023")
                            , n)
                            , mml.line.Lp);
                    }
                    n = (int)info.clockCount / n;
                }
                else
                {
                    n = Common.CheckRange(n, 1, 65535);
                }

                rest.length = n;
                mml.line.Lp.length += col;

            }
            else
            {
                rest.length = 0;
            }

            //.の解析
            int futen = 0;
            int fn = rest.length;
            while (pw.getChar(page) == '.')
            {
                if (fn % 2 != 0)
                {
                    msgBox.setWrnMsg(msg.get("E05036")
                        , mml.line.Lp);
                }
                fn = fn / 2;
                futen += fn;
                pw.incPos(page);
                mml.line.Lp.length++;
            }
            rest.length += futen;

        }

        private void CmdLyric(partWork pw, partPage page, MML mml)
        {
            pw.incPos(page);
            mml.type = enmMMLType.Lyric;
            mml.args = new List<object>();
            string str = "";
            while (true)
            {
                char ch = pw.getChar(page);
                if (ch == '"')
                {
                    pw.incPos(page);
                    break;
                }
                if (ch == '\\')
                {
                    pw.incPos(page);
                    if (ch != '"')
                    {
                        str += '\\';
                    }
                    ch = pw.getChar(page);
                }
                if (ch == '\0') break;

                str += ch;
                pw.incPos(page);
            }
            mml.args.Add(str);

            int n = -1;
            bool directFlg = false;
            int length = 0;

            //数値の解析
            if (pw.getNumNoteLength(page, out n, out directFlg))
            {
                if (!directFlg)
                {
                    if ((int)info.clockCount % n != 0)
                    {
                        msgBox.setWrnMsg(string.Format(msg.get("E05023"), n), mml.line.Lp);
                    }
                    n = (int)info.clockCount / n;
                }
                else
                {
                    n = Common.CheckRange(n, 1, 65535);
                }

                length = n;

            }
            else
            {
                length = (int)page.length;
            }

            //.の解析
            int futen = 0;
            int fn = length;
            while (pw.getChar(page) == '.')
            {
                if (fn % 2 != 0)
                {
                    msgBox.setWrnMsg(msg.get("E05036")
                        , mml.line.Lp);
                }
                fn = fn / 2;
                futen += fn;
                pw.incPos(page);
            }
            length += futen;
            mml.args.Add(length);
        }

        private void CmdBend(partWork pw, partPage page, MML mml)
        {
            pw.incPos(page);
            mml.type = enmMMLType.Bend;
            mml.args = null;
        }

        private void CmdTie(partWork pw, partPage page, MML mml)
        {
            pw.incPos(page);

            mml.type = enmMMLType.Tie;
            mml.args = null;

            int n;
            bool directFlg = false;
            if (!pw.getNumNoteLength(page, out n, out directFlg))
            {
                return;
            }

            mml.type = enmMMLType.TiePC;
            if (!directFlg)
            {
                if ((int)info.clockCount % n != 0)
                {
                    msgBox.setWrnMsg(string.Format(
                        msg.get("E05023")
                        , n), mml.line.Lp);
                }
                n = (int)info.clockCount / n;
            }
            else
            {
                n = Common.CheckRange(n, 1, 65535);
            }

            //.の解析
            int futen = 0;
            int fn = n;
            while (pw.getChar(page) == '.')
            {
                if (fn % 2 != 0)
                {
                    msgBox.setWrnMsg(msg.get("E05036")
                        , mml.line.Lp);
                }
                fn = fn / 2;
                futen += fn;
                pw.incPos(page);
            }
            n += futen;

            mml.args = new List<object>();
            mml.args.Add(n);
        }

        private void CmdTiePC(partWork pw, partPage page, MML mml)
        {
            int n;
            pw.incPos(page);
            mml.type = enmMMLType.TiePC;

            GetLength(pw, page, mml, out n);

            mml.args = new List<object>();
            mml.args.Add(n);
        }

        private void CmdTieMC(partWork pw, partPage page, MML mml)
        {
            int n;
            pw.incPos(page);
            mml.type = enmMMLType.TieMC;

            GetLength(pw, page, mml, out n);

            mml.args = new List<object>();
            mml.args.Add(n);
        }

        #endregion

        #region step2

        private void Step2(partPage page)
        {
            for (int i = 0; i < page.mmlData.Count; i++)
            {
                if (page.mmlData[i].type == enmMMLType.ToneDoubler)
                {
                    step2_CmdToneDoubler(page, i);
                }
            }

            for (int i = 0; i < page.mmlData.Count; i++)
            {
                if (page.mmlData[i].type == enmMMLType.Bend)
                {
                    step2_CmdBend(page, i);
                }
            }

            for (int i = 0; i < page.mmlData.Count; i++)
            {
                if (page.mmlData[i].type == enmMMLType.TiePC)
                {
                    step2_CmdTiePC(page, i);
                    page.mmlData.RemoveAt(i);
                    i--;
                }
                if (page.mmlData[i].type == enmMMLType.TieMC)
                {
                    step2_CmdTieMC(page, i);
                    page.mmlData.RemoveAt(i);
                    i--;
                }
            }

            for (int i = 0; i < page.mmlData.Count; i++)
            {
                if (page.mmlData[i].type == enmMMLType.Tie)
                {
                    step2_CmdTie(page, i);
                    page.mmlData.RemoveAt(i);
                    i--;
                }
            }
        }

        private void step2_CmdToneDoubler(partPage page, int pos)
        {
            if (pos < 1 || page.mmlData[pos - 1].type != enmMMLType.Note)
            {
                msgBox.setErrMsg(msg.get("E05037")
                , page.mmlData[pos].line.Lp);
                return;
            }

            Note note = (Note)page.mmlData[pos - 1].args[0];

            //直前の音符コマンドへToneDoublerコマンドが続くことを知らせる
            note.tDblSw = true;

            //直後の音符コマンドまでサーチ
            Note toneDoublerNote = null;
            List<MML> toneDoublerMML = new List<MML>();
            for (int i = pos + 1; i < page.mmlData.Count; i++)
            {
                switch (page.mmlData[i].type)
                {
                    case enmMMLType.Note:
                        toneDoublerNote = (Note)page.mmlData[i].args[0];
                        page.mmlData.RemoveAt(i);
                        i--;
                        goto loop_exit;
                    case enmMMLType.Octave:
                    case enmMMLType.OctaveUp:
                    case enmMMLType.OctaveDown:
                        toneDoublerMML.Add(page.mmlData[i]);
                        page.mmlData.RemoveAt(i);
                        i--;
                        break;
                    default:
                        msgBox.setErrMsg(msg.get("E05038")
                        , page.mmlData[i].line.Lp);
                        return;
                }
            }

            if (toneDoublerNote == null) return;

            loop_exit:

            note.tDblCmd = toneDoublerNote.cmd;
            note.tDblShift = toneDoublerNote.shift;
            note.length = toneDoublerNote.length;
            note.tDblOctave = toneDoublerMML;

            if (page.mmlData[pos].args == null) page.mmlData[pos].args = new List<object>();
            page.mmlData[pos].args.Add(toneDoublerMML);
        }

        private void step2_CmdBend(partPage page, int pos)
        {
            if (!(
                    (
                    pos > 0
                    && page.mmlData[pos - 1].type == enmMMLType.Note
                    )
                ||
                    (
                    pos > 1
                    && page.mmlData[pos - 1].type == enmMMLType.ToneDoubler
                    && page.mmlData[pos - 2].type == enmMMLType.Note
                    )
                ))
            {
                msgBox.setErrMsg(msg.get("E05039")
                , page.mmlData[pos].line.Lp);
                return;
            }

            Note note = (Note)page.mmlData[pos - (page.mmlData[pos - 1].type == enmMMLType.Note ? 1 : 2)].args[0];

            //直前の音符コマンドへベンドコマンドが続くことを知らせる
            note.bendSw = true;

            //直後の音符コマンドまでサーチ
            Note bendNote = null;
            List<MML> bendMML = new List<MML>();
            for (int i = pos + 1; i < page.mmlData.Count; i++)
            {
                switch (page.mmlData[i].type)
                {
                    case enmMMLType.Note:
                        bendNote = (Note)page.mmlData[i].args[0];
                        if (bendNote.addLength != 0)
                        {
                            page.mmlData[i].type = enmMMLType.TiePC;
                            page.mmlData[i].args = new List<object>();
                            page.mmlData[i].args.Add(bendNote.addLength);
                        }
                        else
                        {
                            page.mmlData.RemoveAt(i);
                        }
                        goto loop_exit;
                    case enmMMLType.Octave:
                    case enmMMLType.OctaveUp:
                    case enmMMLType.OctaveDown:
                        bendMML.Add(page.mmlData[i]);
                        page.mmlData.RemoveAt(i);
                        i--;
                        break;
                    default:
                        msgBox.setErrMsg(msg.get("E05040")
                        , page.mmlData[i].line.Lp);
                        return;
                }
            }

            if (bendNote == null) return;

            loop_exit:

            note.bendCmd = bendNote.cmd;
            note.bendShift = bendNote.shift;
            //note.length = bendNote.length;
            //note.futen = bendNote.futen;
            note.bendOctave = bendMML;
            page.mmlData[pos].args = new List<object>();
            page.mmlData[pos].args.Add(bendMML);
        }

        private void step2_CmdTiePC(partPage page, int pos)
        {
            int nPos = 0;

            //遡ってnoteを探す
            for (int i = pos - 1; i > 0; i--)
            {
                switch (page.mmlData[i].type)
                {
                    case enmMMLType.ToneDoubler:
                    case enmMMLType.Bend:
                    case enmMMLType.TraceUpdateStack:
                        break;
                    case enmMMLType.Note:
                    case enmMMLType.Rest:
                    case enmMMLType.RestNoWork:
                        nPos = i;
                        goto loop_exit;
                    default:
                        msgBox.setErrMsg(msg.get("E05041")
                        , page.mmlData[pos].line.Lp);
                        return;
                }
            }

            msgBox.setErrMsg(msg.get("E05042")
            , page.mmlData[pos].line.Lp);
            return;
        loop_exit:

            Rest rest = (Rest)page.mmlData[nPos].args[0];//NoteはRestを継承している
            rest.length += (int)page.mmlData[pos].args[0];
        }

        private void step2_CmdTieMC(partPage page, int pos)
        {
            int nPos = 0;

            //遡ってnoteを探す
            for (int i = pos - 1; i > 0; i--)
            {
                switch (page.mmlData[i].type)
                {
                    case enmMMLType.ToneDoubler:
                    case enmMMLType.Bend:
                    case enmMMLType.TraceUpdateStack:
                        break;
                    case enmMMLType.Note:
                    case enmMMLType.Rest:
                    case enmMMLType.RestNoWork:
                        nPos = i;
                        goto loop_exit;
                    default:
                        msgBox.setErrMsg(msg.get("E05043")
                        , page.mmlData[pos].line.Lp);
                        return;
                }
            }

            msgBox.setErrMsg(msg.get("E05044")
            , page.mmlData[pos].line.Lp);
            return;
        loop_exit:

            Rest rest = (Rest)page.mmlData[nPos].args[0];//NoteはRestを継承している
            rest.length -= (int)page.mmlData[pos].args[0];
        }

        private void step2_CmdTie(partPage page, int pos)
        {
            int nPos = 0;

            //遡ってnoteを探す
            for (int i = pos - 1; i > 0; i--)
            {
                switch (page.mmlData[i].type)
                {
                    case enmMMLType.ToneDoubler:
                    case enmMMLType.Bend:
                        break;
                    case enmMMLType.Note:
                        nPos = i;
                        goto loop_exit;
                    default:
                        msgBox.setErrMsg(msg.get("E05045")
                        , page.mmlData[pos].line.Lp);
                        return;
                }
            }

            msgBox.setErrMsg(msg.get("E05046")
            , page.mmlData[pos].line.Lp);
            return;
        loop_exit:

            //直前の音符コマンドへ&コマンドが続くことを知らせる
            Note note = (Note)page.mmlData[nPos].args[0];
            note.tieSw = true;
        }

        #endregion

        #region step3

        private void Step3(partPage page)
        {
            //リピート処理向けスタックのクリア
            page.stackRepeat.Clear();
            page.stackRenpu.Clear();

            for (int i = 0; i < page.mmlData.Count; i++)
            {
                switch (page.mmlData[i].type)
                {
                    case enmMMLType.Repeat:
                        step3_CmdRepeatStart(page, i);
                        break;
                    case enmMMLType.RepertExit:
                        step3_CmdRepeatExit(page, i);
                        break;
                    case enmMMLType.RepeatEnd:
                        step3_CmdRepeatEnd(page, i);
                        break;
                    case enmMMLType.Renpu:
                        step3_CmdRenpuStart(page, i);
                        break;
                    case enmMMLType.RenpuEnd:
                        step3_CmdRenpuEnd(page, i);
                        break;
                    case enmMMLType.Note:
                    case enmMMLType.Rest:
                    case enmMMLType.RestNoWork:
                        step3_CmdNoteCount(page, i);
                        break;
                }
            }
        }

        private void step3_CmdRepeatExit(partPage page, int pos)
        {
            int nst = 0;

            for (int searchPos = pos; searchPos < page.mmlData.Count; searchPos++)
            {
                if (page.mmlData[searchPos].type == enmMMLType.Repeat)
                {
                    nst++;
                    continue;
                }
                if (page.mmlData[searchPos].type != enmMMLType.RepeatEnd)
                {
                    continue;
                }
                if (nst == 0)
                {
                    page.mmlData[pos].args = new List<object>();
                    page.mmlData[pos].args.Add(searchPos);
                    return;
                }
                nst--;
            }

            msgBox.setWrnMsg(msg.get("E05047")
                , page.mmlData[pos].line.Lp);

        }

        private void step3_CmdRepeatEnd(partPage page, int pos)
        {
            try
            {
                clsRepeat re = page.stackRepeat.Pop();
                page.mmlData[pos].args.Add(re.pos);
            }
            catch
            {
                msgBox.setWrnMsg(msg.get("E05048")
                , page.mmlData[pos].line.Lp);
            }
        }

        private void step3_CmdRepeatStart(partPage page, int pos)
        {
            clsRepeat rs = new clsRepeat()
            {
                pos = pos,
                repeatCount = -1//初期値
            };
            page.stackRepeat.Push(rs);
        }

        private void step3_CmdRenpuStart(partPage page, int pos)
        {
            clsRenpu r = new clsRenpu();
            r.pos = pos;
            r.repeatStackCount = page.stackRepeat.Count;
            r.noteCount = 0;
            r.mml = page.mmlData[pos];
            page.stackRenpu.Push(r);
        }

        private void step3_CmdRenpuEnd(partPage page, int pos)
        {
            try
            {
                clsRenpu r = page.stackRenpu.Pop();
                r.mml.args = new List<object>();
                r.mml.args.Add(r.noteCount);
                if (page.mmlData[pos].args != null)
                {
                    r.mml.args.Add(page.mmlData[pos].args[0]);//音長(クロック数)
                }

                if (r.repeatStackCount != page.stackRepeat.Count)
                {
                    msgBox.setWrnMsg(msg.get("E05049")
                    , page.mmlData[pos].line.Lp);
                }

                if (r.noteCount > 0)
                {
                    if (page.stackRenpu.Count > 0)
                    {
                        page.stackRenpu.First().noteCount++;
                    }
                }
            }
            catch
            {
                msgBox.setWrnMsg(msg.get("E05050")
                , page.mmlData[pos].line.Lp);
            }
        }

        private void step3_CmdNoteCount(partPage page, int pos)
        {
            if (page.stackRenpu.Count < 1) return;

            page.stackRenpu.First().noteCount++;
        }

        #endregion

        private void Step4()
        {
            //同期マーキングをリストアップする
            foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in chips)
            {
                foreach (ClsChip chip in kvp.Value)
                {
                    if (chip == null) continue;
                    if (!chip.use) continue;
                    foreach (partWork pw in chip.lstPartWork)
                    {
                        foreach (partPage page in pw.pg)
                        {
                            if (page.dataEnd) continue;
                            Step4_CheckSync(page, 0);
                        }
                    }
                }
            }

            //同期マーキングをカウンター順にソート
            for (int i = 0; i < lstSynchronousMark.Count; i++)
            {
                for (int j = i + 1; j < lstSynchronousMark.Count; j++)
                {
                    if (lstSynchronousMark[i].Item2 < lstSynchronousMark[j].Item2) continue;
                    Tuple<int, int> item = lstSynchronousMark[i];
                    lstSynchronousMark[i] = lstSynchronousMark[j];
                    lstSynchronousMark[j] = item;
                }
            }

            //同期の直後に休符を補填する
            foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in chips)
            {
                foreach (ClsChip chip in kvp.Value)
                {
                    if (chip == null) continue;
                    if (!chip.use) continue;
                    foreach (partWork pw in chip.lstPartWork)
                    {
                        foreach (partPage page in pw.pg)
                        {
                            if (page.dataEnd) continue;
                            Step4_CheckSync(page, 1);
                        }
                    }
                }
            }

        }

        private void Step4_CheckSync(partPage page, int mode)
        {
            int counter = 0;
            Stack<int> renpuStack = new Stack<int>();
            Stack<Tuple<int, int, int>> repeatStack = new Stack<Tuple<int, int, int>>();

            for (int i = 0; i < page.mmlData.Count; i++)
            {
                MML mml = page.mmlData[i];
                Tuple<int, int, int> re;
                int repPos;
                int repCnt;
                int repExitCnt;

                switch (mml.type)
                {
                    case enmMMLType.Rest:
                    case enmMMLType.Note:
                        Rest rest = (Rest)mml.args[0];
                        int len = rest.length;

                        //連符の中のノートの場合はカウントしない
                        if (renpuStack.Count > 0) len = 0;

                        counter += len;
                        break;
                    case enmMMLType.Repeat:
                        re = new Tuple<int, int, int>(i, -1, -1);
                        repeatStack.Push(re);
                        break;
                    case enmMMLType.RepertExit:
                        re = repeatStack.Pop();
                        repPos = re.Item1;
                        repCnt = re.Item2;
                        repExitCnt = re.Item3;

                        if (repCnt == 1)
                        {
                            i = (int)mml.args[0];
                        }
                        else
                        {
                            if (repExitCnt == -1)
                            {
                                repExitCnt = counter;
                            }
                            repeatStack.Push(new Tuple<int, int, int>(repPos, repCnt, repExitCnt));
                        }
                        break;
                    case enmMMLType.RepeatEnd:
                        try
                        {
                            re = repeatStack.Pop();
                            repPos = re.Item1;
                            repCnt = re.Item2;
                            repExitCnt = re.Item3;

                            if (repCnt == -1)
                            {
                                repCnt = (int)mml.args[0];
                            }
                            repCnt--;
                            if (repCnt != 0)
                            {
                                i = repPos;
                                repeatStack.Push(new Tuple<int, int, int>(repPos, repCnt, repExitCnt));
                            }
                            else
                            {
                                if ((int)mml.args[0] < 2 && repExitCnt != -1)
                                {
                                    counter = repExitCnt;
                                }
                            }
                        }
                        catch
                        {
                            ;//スタックがない場合は既にエラー検知されているのでここでは無視する
                        }
                        break;
                    case enmMMLType.Renpu:
                        renpuStack.Push((int)mml.args[1]);
                        break;
                    case enmMMLType.RenpuEnd:
                        try
                        {
                            int rlen = renpuStack.Pop();
                            if (renpuStack.Count == 0) counter += rlen;
                        }
                        catch
                        {
                            ;//スタックがない場合は既にエラー検知されているのでここでは無視する
                        }
                        break;
                    case enmMMLType.Synchronous:
                        if (mode == 0)
                        {
                            int m = (int)mml.args[0];
                            int n = (int)mml.args[1];
                            if (m == 1)
                            {
                                //Console.WriteLine("number:{0} counter:{1}", n, counter);
                                Tuple<int, int> item = new Tuple<int, int>(n, counter);
                                if (n != -1)
                                {
                                    for (int j = 0; j < lstSynchronousMark.Count; j++)
                                    {
                                        if (lstSynchronousMark[j].Item1 == n)
                                        {
                                            lstSynchronousMark.RemoveAt(j);
                                            j--;
                                        }
                                    }
                                }
                                lstSynchronousMark.Add(item);
                            }
                        }
                        else
                        {
                            int m = (int)mml.args[0];
                            int n = (int)mml.args[1];
                            if (m == 2)
                            {
                                int j;
                                for (j = 0; j < lstSynchronousMark.Count; j++)
                                {
                                    if (n == lstSynchronousMark[j].Item1)
                                    {
                                        break;
                                    }
                                }
                                int err = 1;
                                if (j != lstSynchronousMark.Count)
                                {
                                    int slen = lstSynchronousMark[j].Item2;
                                    slen = slen - counter;
                                    if (slen > 0)
                                    {
                                        //休符発行
                                        err = 0;
                                        Rest rests = new Rest();
                                        mml.args.Add(rests);
                                        rests.cmd = 'r';
                                        rests.length = slen;
                                        mml.args[0] = 3;
                                    }
                                }

                                if (err != 0)
                                {
                                    msgBox.setWrnMsg(msg.get("E05061")
                                    , page.mmlData[i].line.Lp);
                                }
                            }
                        }
                        break;
                    default:
                        //Console.WriteLine(mml.type.ToString());
                        break;
                }
            }
        }

    }
}