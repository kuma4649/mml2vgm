using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            desVGM.useJumpCommand = 0;
            this.desVGM = desVGM;
        }

        public int Start()
        {
            foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in chips)
            {
                foreach (ClsChip chip in kvp.Value)
                {
                    if (!chip.use) continue;

                    foreach (partWork pw in chip.lstPartWork)
                    {
                        if (pw.pData == null) continue;

                        Step1(pw);//mml全体のフォーマット解析
                        Step2(pw);//toneDoubler,bend,tieコマンドの解析
                        Step3(pw);//リピート、連符コマンドの解析

                        pw.dataEnd = false;
                    }
                }
            }
            return 0;

        }



        #region step1

        private void Step1(partWork pw)
        {
            pw.resetPos();
            pw.dataEnd = false;
            pw.mmlData = new List<MML>();

            while (!pw.dataEnd)
            {
                char cmd = pw.getChar();
                if (cmd == 0) pw.dataEnd = true;
                else
                {
                    //lineNumber = pw.getLineNumber();
                    Commander(pw, cmd);
                }
            }
        }

        private bool swToneDoubler = false;

        public LinePos linePos { get; internal set; }

        private void Commander(partWork pw, char cmd)
        {
            MML mml = new MML();
            mml.line = pw.getLine();
            mml.column = pw.getPos();



            //コマンド解析

            switch (cmd)
            {
                case ' ':
                case '\t':
                    pw.incPos();
                    break;
                case '!': // CompileSkip
                    log.Write("CompileSkip");
                    pw.dataEnd = true;
                    mml.type = enmMMLType.CompileSkip;
                    mml.args = null;
                    break;
                case 'T': // tempo
                    log.Write(" tempo");
                    CmdTempo(pw, mml);
                    break;
                case '@': // instrument
                    log.Write("instrument");
                    CmdInstrument(pw, mml);
                    break;
                case 'v': // volume
                    log.Write("volume");
                    CmdVolume(pw, mml);
                    break;
                case 'V': // totalVolume(Adpcm-A / Rhythm)
                    log.Write("totalVolume(Adpcm-A / Rhythm)");
                    CmdTotalVolume(pw, mml);
                    break;
                case 'o': // octave
                    log.Write("octave");
                    CmdOctave(pw, mml);
                    break;
                case '>': // octave Up
                    log.Write("octave Up");
                    CmdOctaveUp(pw, mml);
                    break;
                case '<': // octave Down
                    log.Write("octave Down");
                    CmdOctaveDown(pw, mml);
                    break;
                case ')': // volume Up
                    log.Write(" volume Up");
                    CmdVolumeUp(pw, mml);
                    break;
                case '(': // volume Down
                    log.Write("volume Down");
                    CmdVolumeDown(pw, mml);
                    break;
                case 'l': // length
                    log.Write("length");
                    CmdLength(pw, mml);
                    break;
                case '#': // length(clock)
                    log.Write("length(clock)");
                    CmdClockLength(pw, mml);
                    break;
                case 'p': // pan
                    log.Write(" pan");
                    CmdPan(pw, mml);
                    break;
                case 'D': // Detune
                    log.Write("Detune");
                    CmdDetune(pw, mml);
                    break;
                case 'm': // pcm mode
                    log.Write("pcm mode");
                    CmdMode(pw, mml);
                    break;
                case 'q': // gatetime
                    log.Write(" gatetime q");
                    CmdGatetime(pw, mml);
                    break;
                case 'Q': // gatetime
                    log.Write("gatetime Q");
                    CmdGatetime2(pw, mml);
                    break;
                case 'E': // envelope / extendChannel
                    log.Write("envelope / extendChannel");
                    CmdE(pw, mml);
                    break;
                case 'L': // loop point
                    log.Write(" loop point");
                    CmdLoop(pw, mml);
                    break;
                case 'J': // Jump point
                    log.Write("Jump point");
                    CmdJump(pw, mml);
                    break;
                case '[': // repeat
                    log.Write("repeat [");
                    CmdRepeatStart(pw, mml);
                    break;
                case ']': // repeat
                    log.Write("repeat ]");
                    CmdRepeatEnd(pw, mml);
                    break;
                case '{': // renpu
                    log.Write("renpu {");
                    CmdRenpuStart(pw, mml);
                    break;
                case '}': // renpu
                    log.Write("renpu }");
                    CmdRenpuEnd(pw, mml);
                    break;
                case '/': // repeat
                    log.Write("repeat /");
                    CmdRepeatExit(pw, mml);
                    break;
                case 'M': // lfo
                    log.Write("lfo");
                    CmdLfo(pw, mml);
                    break;
                case 'S': // lfo switch
                    log.Write(" lfo switch");
                    CmdLfoSwitch(pw, mml);
                    break;
                case 'y': // y
                    log.Write(" y");
                    CmdY(pw, mml);
                    break;
                case 'w': // noise
                    log.Write("noise");
                    CmdNoise(pw, mml);
                    break;
                case 'P': // noise or tone mixer
                    log.Write("noise or tone mixer");
                    CmdMixer(pw, mml);
                    break;
                case 'K': // key shift
                    log.Write("key shift");
                    CmdKeyShift(pw, mml);
                    break;
                case 's': // sus ON/OFF
                    log.Write("sus ON/OFF");
                    CmdSusOnOff(pw, mml);
                    break;
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                case 'g':
                case 'a':
                case 'b':
                    log.Write(string.Format("note {0}", cmd));
                    CmdNote(pw, cmd, mml);
                    break;
                case 'r':
                    log.Write("rest");
                    CmdRest(pw, mml);
                    break;
                case 'R':
                    log.Write("restNoWork");
                    CmdRestNoWork(pw, mml);
                    break;
                case '"':
                    log.Write("lylic");
                    CmdLyric(pw, mml);
                    break;
                case '_':
                    log.Write("bend");
                    CmdBend(pw, mml);
                    break;
                case '&':
                    log.Write("tie");
                    CmdTie(pw, mml);
                    break;
                case '^':
                    log.Write("tie plus clock");
                    CmdTiePC(pw, mml);
                    break;
                case '~':
                    log.Write("tie minus clock");
                    CmdTieMC(pw, mml);
                    break;
                case 'A': // Address shift
                    log.Write("Address shift");
                    CmdAddressShift(pw, mml);
                    break;
                default:
                    msgBox.setErrMsg(string.Format(msg.get("E05000"), cmd), mml.line.Lp);
                    pw.incPos();
                    break;
            }



            //mmlコマンドの追加

            if (mml.type == enmMMLType.unknown) return;

            if (!mmlData.ContainsKey(pw.PartName))
            {
                mmlData.Add(pw.PartName, new List<MML>());
            }

            pw.mmlData.Add(mml);

            if (swToneDoubler)
            {
                mml = new MML();
                mml.type = enmMMLType.ToneDoubler;
                mml.line = pw.getLine();
                mml.column = pw.getPos();
                pw.mmlData.Add(mml);
            }
        }

        private void CmdTempo(partWork pw, MML mml)
        {
            pw.incPos();
            if (!pw.getNum(out int n))
            {
                msgBox.setErrMsg(msg.get("E05001"), mml.line.Lp);
                n = 120;
            }
            n = Common.CheckRange(n, 1, 1200);

            mml.type = enmMMLType.Tempo;
            mml.args = new List<object>();
            mml.args.Add(n);
        }

        private void CmdInstrument(partWork pw, MML mml)
        {
            int n;
            pw.incPos();

            mml.type = enmMMLType.Instrument;
            mml.args = new List<object>();

            //@Tn
            if (pw.getChar() == 'T')
            {
                //tone doubler
                mml.args.Add('T');
                pw.incPos();
            }
            //@En
            else if (pw.getChar() == 'E')
            {
                //Envelope
                mml.args.Add('E');
                pw.incPos();
            }
            //@In
            else if (pw.getChar() == 'I')
            {
                //Instrument(プリセット音色)
                mml.args.Add('I');
                pw.incPos();
            }
            //@Nn
            else if (pw.getChar() == 'N')
            {
                //None(音色設定直前に何もしない)
                mml.args.Add('N');
                pw.incPos();
            }
            //@Rn
            else if (pw.getChar() == 'R')
            {
                //RR(音色設定直前にRR=15をセットする)
                mml.args.Add('R');
                pw.incPos();
            }
            //@An
            else if (pw.getChar() == 'A')
            {
                //All send(音色設定直前に消音向け音色をセットする)
                mml.args.Add('A');
                pw.incPos();
            }
            else
            {
                //normal
                mml.args.Add('n');
            }

            //@n

            if (!pw.getNum(out n))
            {
                msgBox.setErrMsg(msg.get("E05002"), mml.line.Lp);
                n = 0;
            }
            n = Common.CheckRange(n, 0, 255);
            mml.args.Add(n);

        }

        private void CmdVolume(partWork pw, MML mml)
        {
            int n;
            pw.incPos();
            mml.type = enmMMLType.Volume;
            mml.args = new List<object>();

            if (!pw.getNum(out n))
            {
                msgBox.setErrMsg(msg.get("E05003"), mml.line.Lp);
                n = (int)(pw.MaxVolume * 0.9);
            }
            n = Common.CheckRange(n, 0, pw.MaxVolume);
            mml.args.Add(n);
        }

        private void CmdTotalVolume(partWork pw, MML mml)
        {
            int n;
            pw.incPos();
            mml.type = enmMMLType.TotalVolume;
            mml.args = new List<object>();

            if (!pw.getNum(out n))
            {
                msgBox.setErrMsg(msg.get("E05004"), mml.line.Lp);
                n = 0;
            }
            mml.args.Add(n);

            if (pw.getChar() == ',')
            {
                pw.incPos();
                if (!pw.getNum(out n))
                {
                    msgBox.setErrMsg(msg.get("E05004"), mml.line.Lp);
                    n = 0;
                }
                mml.args.Add(n);
            }

        }

        private void CmdOctave(partWork pw, MML mml)
        {
            pw.incPos();
            if (!pw.getNum(out int n))
            {
                msgBox.setErrMsg(msg.get("E05005"), mml.line.Lp);
                n = 110;
            }
            n = Common.CheckRange(n, 1, 8);

            mml.type = enmMMLType.Octave;
            mml.args = new List<object>();
            mml.args.Add(n);
        }

        private void CmdOctaveUp(partWork pw, MML mml)
        {
            pw.incPos();
            mml.type = enmMMLType.OctaveUp;
            mml.args = null;
        }

        private void CmdOctaveDown(partWork pw, MML mml)
        {
            pw.incPos();
            mml.type = enmMMLType.OctaveDown;
            mml.args = null;
        }

        private void CmdVolumeUp(partWork pw, MML mml)
        {
            int n;
            pw.incPos();
            if (!pw.getNum(out n))
            {
                msgBox.setErrMsg(msg.get("E05006"), mml.line.Lp);
                return;
            }
            mml.type = enmMMLType.VolumeUp;
            mml.args = new List<object>();
            mml.args.Add(n);

        }

        private void CmdVolumeDown(partWork pw, MML mml)
        {
            pw.incPos();
            if (!pw.getNum(out int n))
            {
                msgBox.setErrMsg(msg.get("E05007"), mml.line.Lp);
                n = 10;
            }
            n = Common.CheckRange(n, 1, pw.MaxVolume);
            mml.type = enmMMLType.VolumeDown;
            mml.args = new List<object>();
            mml.args.Add(n);
        }

        private void CmdLength(partWork pw, MML mml)
        {
            pw.incPos();
            //数値の解析
            if (pw.getNumNoteLength(out int n, out bool directFlg))
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
                while (pw.getChar() == '.')
                {
                    if (fn % 2 != 0)
                    {
                        msgBox.setWrnMsg(msg.get("E05036")
                            , mml.line.Lp);
                    }
                    fn = fn / 2;
                    futen += fn;
                    pw.incPos();
                }
                n += futen;

            }
            mml.type = enmMMLType.Length;
            mml.args = new List<object>();
            mml.args.Add(n);
            pw.length = n;
        }

        private void CmdClockLength(partWork pw, MML mml)
        {
            pw.incPos();
            if (!pw.getNum(out int n))
            {
                msgBox.setErrMsg(msg.get("E05009"), mml.line.Lp);
                n = 10;
            }
            n = Common.CheckRange(n, 1, 65535);
            mml.type = enmMMLType.LengthClock;
            mml.args = new List<object>();
            mml.args.Add(n);
        }

        private void CmdPan(partWork pw, MML mml)
        {
            int n;

            pw.incPos();
            if (!pw.getNum(out n))
            {
                msgBox.setErrMsg(msg.get("E05010"), mml.line.Lp);
            }
            mml.type = enmMMLType.Pan;
            mml.args = new List<object>();
            mml.args.Add(n);

            if (pw.getChar() == ',')
            {
                pw.incPos();
                if (!pw.getNum(out n))
                {
                    msgBox.setErrMsg(msg.get("E05010"), mml.line.Lp);
                }
                mml.args.Add(n);
            }
        }

        private void CmdDetune(partWork pw, MML mml)
        {
            int n;
            pw.incPos();
            if (!pw.getNum(out n))
            {
                msgBox.setErrMsg(msg.get("E05011"), mml.line.Lp);
                n = 0;
            }
            mml.type = enmMMLType.Detune;
            mml.args = new List<object>();
            mml.args.Add(n);
        }

        private void CmdMode(partWork pw, MML mml)
        {
            int n;
            pw.incPos();
            if (!pw.getNum(out n))
            {
                msgBox.setErrMsg(msg.get("E05012"), mml.line.Lp);
                n = 0;
            }
            mml.type = enmMMLType.PcmMode;
            mml.args = new List<object>();
            mml.args.Add(n);
        }

        private void CmdGatetime(partWork pw, MML mml)
        {
            int n;
            pw.incPos();
            if (!pw.getNum(out n))
            {
                msgBox.setErrMsg(msg.get("E05013"), mml.line.Lp);
                n = 0;
            }
            n = Common.CheckRange(n, 0, 255);
            mml.type = enmMMLType.Gatetime;
            mml.args = new List<object>();
            mml.args.Add(n);
        }

        private void CmdGatetime2(partWork pw, MML mml)
        {
            int n;
            pw.incPos();
            if (!pw.getNum(out n))
            {
                msgBox.setErrMsg(msg.get("E05014"), mml.line.Lp);
                n = 1;
            }
            n = Common.CheckRange(n, 1, 8);
            mml.type = enmMMLType.GatetimeDiv;
            mml.args = new List<object>();
            mml.args.Add(n);
        }

        private void CmdE(partWork pw,MML mml)
        {
            int n = -1;
            pw.incPos();
            switch (pw.getChar())
            {
                case 'O':
                    pw.incPos();
                    switch (pw.getChar())
                    {
                        case 'N':
                            pw.incPos();
                            mml.type = enmMMLType.Envelope;
                            mml.args = new List<object>();
                            mml.args.Add("EON");
                            break;
                        case 'F':
                            pw.incPos();
                            mml.type = enmMMLType.Envelope;
                            mml.args = new List<object>();
                            mml.args.Add("EOF");
                            break;
                        default:
                            msgBox.setErrMsg(string.Format(msg.get("E05015")
                                , pw.getChar())
                                , mml.line.Lp);
                            break;
                    }
                    break;
                case 'H':
                    pw.incPos();
                    n = -1;
                    if (pw.getChar() == 'O')
                    {
                        pw.incPos();
                        switch (pw.getChar())
                        {
                            case 'N':
                                pw.incPos();
                                mml.type = enmMMLType.HardEnvelope;
                                mml.args = new List<object>();
                                mml.args.Add("EHON");
                                break;
                            case 'F':
                                pw.incPos();
                                mml.type = enmMMLType.HardEnvelope;
                                mml.args = new List<object>();
                                mml.args.Add("EHOF");
                                break;
                            default:
                                msgBox.setErrMsg(string.Format(msg.get("E05016")
                                    , pw.getChar())
                                    , mml.line.Lp);
                                break;
                        }
                    }
                    else if (pw.getChar() == 'T')
                    {
                        pw.incPos();
                        mml.type = enmMMLType.HardEnvelope;
                        mml.args = new List<object>();
                        mml.args.Add("EHT");
                        if (!pw.getNum(out n))
                        {
                            msgBox.setErrMsg(msg.get("E05017")
                                , mml.line.Lp);
                            break;
                        }
                        mml.args.Add(n);
                        break;
                    }
                    else if (!pw.getNum(out n))
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
                    pw.incPos();
                    n = -1;
                    if (pw.getChar() == 'O')
                    {
                        pw.incPos();
                        switch (pw.getChar())
                        {
                            case 'N':
                                pw.incPos();
                                mml.type = enmMMLType.ExtendChannel;
                                mml.args = new List<object>();
                                mml.args.Add("EXON");
                                break;
                            case 'F':
                                pw.incPos();
                                mml.type = enmMMLType.ExtendChannel;
                                mml.args = new List<object>();
                                mml.args.Add("EXOF");
                                break;
                            default:
                                msgBox.setErrMsg(string.Format(msg.get("E05019"), pw.getChar()), mml.line.Lp);
                                break;
                        }
                    }
                    else if (pw.getChar() == 'D')
                    {
                        pw.incPos();
                        mml.type = enmMMLType.ExtendChannel;
                        mml.args = new List<object>();
                        mml.args.Add("EXD");
                        for (int i = 0; i < 4; i++)
                        {
                            if (!pw.getNum(out n))
                            {
                                msgBox.setErrMsg(msg.get("E05020"), mml.line.Lp);
                                break;
                            }
                            mml.args.Add(n);
                            if (i == 3) break;
                            pw.incPos();
                        }
                        break;
                    }
                    else if (!pw.getNum(out n))
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
                        if (pw.getNum(out n))
                        {
                            mml.args.Add(n);
                        }
                        else
                        {
                            msgBox.setErrMsg(msg.get("E05022"), mml.line.Lp);
                            break;
                        }
                        if (i == 3) break;
                        pw.incPos();
                    }
                    break;
            }

        }

        private void CmdLoop(partWork pw, MML mml)
        {
            pw.incPos();
            mml.type = enmMMLType.LoopPoint;
            mml.args = null;
        }

        private void CmdJump(partWork pw, MML mml)
        {
            pw.incPos();
            mml.type = enmMMLType.JumpPoint;
            mml.args = null;
            desVGM.useJumpCommand++;
        }

        private void CmdRepeatStart(partWork pw,MML mml)
        {
            pw.incPos();
            mml.type = enmMMLType.Repeat;
            mml.args = null;
        }

        private void CmdRepeatEnd(partWork pw,MML mml)
        {
            pw.incPos();
            if (!pw.getNum(out int n))
            {
                n = 2;
            }
            n = Common.CheckRange(n, 1, 255);
            mml.type = enmMMLType.RepeatEnd;
            mml.args = new List<object>();
            mml.args.Add(n);
        }

        private void CmdRenpuStart(partWork pw, MML mml)
        {
            pw.incPos();
            mml.type = enmMMLType.Renpu;
            mml.args = null;
        }

        private void CmdRenpuEnd(partWork pw,MML mml)
        {
            pw.incPos();
            mml.type = enmMMLType.RenpuEnd;
            mml.args = null;
            if (pw.getNumNoteLength(out int n, out bool directFlg))
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
        }

        private void CmdRepeatExit(partWork pw,MML mml)
        {
            pw.incPos();
            mml.type = enmMMLType.RepertExit;
            mml.args = null;
        }

        private void CmdLfo(partWork pw,MML mml)
        {
            int n;
            pw.incPos();
            char c = pw.getChar();

            if (c == 'A')
            {
                pw.incPos();
                char d = pw.getChar();
                if (d == 'M')
                {
                    pw.incPos();
                    d = pw.getChar();
                    if (d == 'S')
                    {
                        pw.incPos();
                        if (!pw.getNum(out n))
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

            pw.incPos();
            char t = pw.getChar();

            if (c == 'P' && t == 'M')
            {
                pw.incPos();
                char d = pw.getChar();
                if (d == 'S')
                {
                    pw.incPos();
                    if (!pw.getNum(out n))
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
                pw.incPos();
                if (pw.getNum(out n))
                {
                    mml.args.Add(n);
                }
                else
                {
                    msgBox.setErrMsg(msg.get("E05030"), mml.line.Lp);
                    return;
                }

                while (pw.getChar() == '\t' || pw.getChar() == ' ') { pw.incPos(); }

            } while (pw.getChar() == ',');

        }

        private void CmdLfoSwitch(partWork pw,MML mml)
        {

            pw.incPos();
            char c = pw.getChar();
            if (c < 'P' || c > 'S')
            {
                msgBox.setErrMsg(msg.get("E05031"), mml.line.Lp);
                pw.incPos();
                return;
            }

            int n = -1;
            pw.incPos();
            if (!pw.getNum(out n))
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

        private void CmdSusOnOff(partWork pw,MML mml)
        {
            pw.incPos();
            char c = pw.getChar();
            pw.incPos();
            if (c != 'o' && c != 'f')
            {
                msgBox.setErrMsg(msg.get("E05031"), mml.line.Lp);
                return;
            }

            mml.type = enmMMLType.SusOnOff;
            mml.args = new List<object>();
            mml.args.Add(c);
        }

        private void CmdY(partWork pw,MML mml)
        {
            int n = -1;
            byte adr = 0;
            byte dat = 0;
            byte op = 0;
            pw.incPos();

            char c = pw.getChar();
            if (c >= 'A' && c <= 'Z')
            {
                string toneparamName = "" + c;
                pw.incPos();
                toneparamName += pw.getChar();
                pw.incPos();
                if (toneparamName != "TL" && toneparamName != "SR")
                {
                    toneparamName += pw.getChar();
                    pw.incPos();
                    if (toneparamName != "SSG")
                    {
                        toneparamName += pw.getChar();
                        pw.incPos();
                    }
                }

                if (toneparamName == "DT1M" || toneparamName == "DT2S" || toneparamName == "PMSA")
                {
                    toneparamName += pw.getChar();
                    pw.incPos();
                    if (toneparamName == "PMSAM")
                    {
                        toneparamName += pw.getChar();
                        pw.incPos();
                    }
                }

                pw.incPos();

                if (toneparamName != "FBAL" && toneparamName != "PMSAMS")
                {
                    if (pw.getNum(out n))
                    {
                        op = (byte)(Common.CheckRange(n & 0xff, 1, 4) - 1);
                    }
                    pw.incPos();
                }

                if (pw.getNum(out n))
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

            if (pw.getNum(out n))
            {
                adr = (byte)(n & 0xff);
            }
            pw.incPos();
            if (pw.getNum(out n))
            {
                dat = (byte)(n & 0xff);
            }

            mml.type = enmMMLType.Y;
            mml.args = new List<object>();
            mml.args.Add(adr);
            mml.args.Add(dat);
        }

        private void CmdNoise(partWork pw, MML mml)
        {
            int n = -1;
            pw.incPos();

            if (!pw.getNum(out n))
            {
                msgBox.setErrMsg(msg.get("E05033"), mml.line.Lp);
                return;

            }
            mml.type = enmMMLType.Noise;
            mml.args = new List<object>();
            mml.args.Add(n);
        }

        private void CmdMixer(partWork pw,MML mml)
        {
            int n = -1;
            pw.incPos();

            if (!pw.getNum(out n))
            {
                msgBox.setErrMsg(msg.get("E05034"), mml.line.Lp);
                return;

            }
            mml.type = enmMMLType.NoiseToneMixer;
            mml.args = new List<object>();
            mml.args.Add(n);

        }

        private void CmdKeyShift(partWork pw, MML mml)
        {
            int n = -1;
            pw.incPos();

            if (!pw.getNum(out n))
            {
                msgBox.setErrMsg(msg.get("E05035"), mml.line.Lp);
                return;

            }
            mml.type = enmMMLType.KeyShift;
            mml.args = new List<object>();
            mml.args.Add(n);
        }

        private void CmdAddressShift(partWork pw, MML mml)
        {
            int n = -1;
            pw.incPos();
            int sign = 0;

            if (pw.getChar() == '+')
            {
                sign = 1;
                pw.incPos();
            }
            else if (pw.getChar() == '-')
            {
                sign = -1;
                pw.incPos();
            }

            if (!pw.getNum(out n))
            {
                msgBox.setErrMsg(msg.get("E05052"), mml.line.Lp);
                return;

            }
            mml.type = enmMMLType.AddressShift;
            mml.args = new List<object>();
            mml.args.Add(sign);
            mml.args.Add(n);
        }

        private void CmdNote(partWork pw, char cmd, MML mml)
        {
            pw.incPos();
            mml.type = enmMMLType.Note;
            mml.args = new List<object>();
            Note note = new Note();
            mml.args.Add(note);
            note.cmd = cmd;

            //+ -の解析
            int shift = 0;
            while (pw.getChar() == '+' || pw.getChar() == '-')
            {
                if (pw.getChar() == '+')
                    shift++;
                else
                    shift--;
                pw.incPos();
            }
            note.shift = shift;

            int n = -1;
            bool directFlg = false;

            //数値の解析
            if (pw.getNumNoteLength(out n, out directFlg))
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

                note.length = n;

                //ToneDoubler'0'指定の場合はここで解析終了
                if (n == 0)
                {
                    swToneDoubler = true;
                    return;
                }
            }
            else
            {
                note.length = (int)pw.length;

                //Tone Doubler','指定の場合はここで解析終了
                if (pw.getChar() == ',')
                {
                    pw.incPos();
                    swToneDoubler = true;
                    return;
                }
            }

            //.の解析
            int futen = 0;
            int fn = note.length;
            while (pw.getChar() == '.')
            {
                if (fn % 2 != 0)
                {
                    msgBox.setWrnMsg(msg.get("E05036")
                        , mml.line.Lp);
                }
                fn = fn / 2;
                futen += fn;
                pw.incPos();
            }
            note.length += futen;

        }

        private void CmdRest(partWork pw, MML mml)
        {
            pw.incPos();
            mml.type = enmMMLType.Rest;
            mml.args = new List<object>();
            Rest rest = new Rest();
            mml.args.Add(rest);

            rest.cmd = 'r';

            int n = -1;
            bool directFlg = false;

            //数値の解析
            if (pw.getNumNoteLength(out n, out directFlg))
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

            }
            else
            {
                rest.length = (int)pw.length;
            }

            //.の解析
            int futen = 0;
            int fn = rest.length;
            while (pw.getChar() == '.')
            {
                if (fn % 2 != 0)
                {
                    msgBox.setWrnMsg(msg.get("E05036")
                        , mml.line.Lp);
                }
                fn = fn / 2;
                futen += fn;
                pw.incPos();
            }
            rest.length += futen;

        }

        private void CmdRestNoWork(partWork pw, MML mml)
        {
            pw.incPos();
            mml.type = enmMMLType.Rest;
            mml.args = new List<object>();
            Rest rest = new Rest();
            mml.args.Add(rest);

            rest.cmd = 'R';

            int n = -1;
            bool directFlg = false;

            //数値の解析
            if (pw.getNumNoteLength(out n, out directFlg))
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

            }
            else
            {
                rest.length = 0;
            }

            //.の解析
            int futen = 0;
            int fn = rest.length;
            while (pw.getChar() == '.')
            {
                if (fn % 2 != 0)
                {
                    msgBox.setWrnMsg(msg.get("E05036")
                        , mml.line.Lp);
                }
                fn = fn / 2;
                futen += fn;
                pw.incPos();
            }
            rest.length += futen;

        }

        private void CmdLyric(partWork pw, MML mml)
        {
            pw.incPos();
            mml.type = enmMMLType.Lyric;
            mml.args = new List<object>();
            string str = "";
            while (true)
            {
                char ch = pw.getChar();
                if (ch == '"')
                {
                    pw.incPos();
                    break;
                }
                if (ch == '\\')
                {
                    pw.incPos();
                    if (ch != '"')
                    {
                        str += '\\';
                    }
                    ch = pw.getChar();
                }
                if (ch == '\0') break;

                str += ch;
                pw.incPos();
            }
            mml.args.Add(str);

            int n = -1;
            bool directFlg = false;
            int length = 0;

            //数値の解析
            if (pw.getNumNoteLength(out n, out directFlg))
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
                length = (int)pw.length;
            }

            //.の解析
            int futen = 0;
            int fn = length;
            while (pw.getChar() == '.')
            {
                if (fn % 2 != 0)
                {
                    msgBox.setWrnMsg(msg.get("E05036")
                        , mml.line.Lp);
                }
                fn = fn / 2;
                futen += fn;
                pw.incPos();
            }
            length += futen;
            mml.args.Add(length);
        }

        private void CmdBend(partWork pw, MML mml)
        {
            pw.incPos();
            mml.type = enmMMLType.Bend;
            mml.args = null;
        }

        private void CmdTie(partWork pw, MML mml)
        {
            pw.incPos();

            mml.type = enmMMLType.Tie;
            mml.args = null;

            int n;
            bool directFlg = false;
            if (!pw.getNumNoteLength(out n, out directFlg))
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
            while (pw.getChar() == '.')
            {
                if (fn % 2 != 0)
                {
                    msgBox.setWrnMsg(msg.get("E05036")
                        , mml.line.Lp);
                }
                fn = fn / 2;
                futen += fn;
                pw.incPos();
            }
            n += futen;

            mml.args = new List<object>();
            mml.args.Add(n);
        }

        private void CmdTiePC(partWork pw, MML mml)
        {
            int n;
            pw.incPos();
            mml.type = enmMMLType.TiePC;

            //数値の解析
            bool directFlg = false;
            if (pw.getNumNoteLength(out n, out directFlg))
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
                n = 0;
            }

            //.の解析
            int futen = 0;
            int fn = n;
            while (pw.getChar() == '.')
            {
                if (fn % 2 != 0)
                {
                    msgBox.setWrnMsg(msg.get("E05036")
                        , mml.line.Lp);
                }
                fn = fn / 2;
                futen += fn;
                pw.incPos();
            }
            n += futen;

            mml.args = new List<object>();
            mml.args.Add(n);
        }

        private void CmdTieMC(partWork pw, MML mml)
        {
            int n;
            pw.incPos();
            mml.type = enmMMLType.TieMC;

            //数値の解析
            bool directFlg = false;
            if (pw.getNumNoteLength(out n, out directFlg))
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
                n = 0;
            }

            //.の解析
            int futen = 0;
            int fn = n;
            while (pw.getChar() == '.')
            {
                if (fn % 2 != 0)
                {
                    msgBox.setWrnMsg(msg.get("E05036")
                        , mml.line.Lp);
                }
                fn = fn / 2;
                futen += fn;
                pw.incPos();
            }
            n += futen;

            mml.args = new List<object>();
            mml.args.Add(n);
        }

        #endregion

        #region step2

        private void Step2(partWork pw)
        {
            for (int i = 0; i < pw.mmlData.Count; i++)
            {
                if(pw.mmlData[i].type== enmMMLType.ToneDoubler)
                {
                    step2_CmdToneDoubler(pw, i);
                }
            }

            for (int i = 0; i < pw.mmlData.Count; i++)
            {
                if (pw.mmlData[i].type == enmMMLType.Bend)
                {
                    step2_CmdBend(pw, i);
                }
            }

            for (int i = 0; i < pw.mmlData.Count; i++)
            {
                if (pw.mmlData[i].type == enmMMLType.TiePC)
                {
                    step2_CmdTiePC(pw, i);
                    pw.mmlData.RemoveAt(i);
                    i--;
                }
                if (pw.mmlData[i].type == enmMMLType.TieMC)
                {
                    step2_CmdTieMC(pw, i);
                    pw.mmlData.RemoveAt(i);
                    i--;
                }
            }

            for (int i = 0; i < pw.mmlData.Count; i++)
            {
                if (pw.mmlData[i].type == enmMMLType.Tie)
                {
                    step2_CmdTie(pw, i);
                    pw.mmlData.RemoveAt(i);
                    i--;
                }
            }
        }

        private void step2_CmdToneDoubler(partWork pw, int pos)
        {
            if (pos < 1 || pw.mmlData[pos - 1].type != enmMMLType.Note)
            {
                msgBox.setErrMsg(msg.get("E05037")
                , pw.mmlData[pos].line.Lp);
                return;
            }

            Note note = (Note)pw.mmlData[pos - 1].args[0];

            //直前の音符コマンドへToneDoublerコマンドが続くことを知らせる
            note.tDblSw = true;

            //直後の音符コマンドまでサーチ
            Note toneDoublerNote = null;
            List<MML> toneDoublerMML = new List<MML>();
            for (int i = pos + 1; i < pw.mmlData.Count; i++)
            {
                switch (pw.mmlData[i].type)
                {
                    case enmMMLType.Note:
                        toneDoublerNote = (Note)pw.mmlData[i].args[0];
                        pw.mmlData.RemoveAt(i);
                        i--;
                        goto loop_exit;
                    case enmMMLType.Octave:
                    case enmMMLType.OctaveUp:
                    case enmMMLType.OctaveDown:
                        toneDoublerMML.Add(pw.mmlData[i]);
                        pw.mmlData.RemoveAt(i);
                        i--;
                        break;
                    default:
                        msgBox.setErrMsg(msg.get("E05038")
                        , pw.mmlData[i].line.Lp);
                        return;
                }
            }

            if (toneDoublerNote == null) return;

            loop_exit:

            note.tDblCmd = toneDoublerNote.cmd;
            note.tDblShift = toneDoublerNote.shift;
            note.length = toneDoublerNote.length;
            note.tDblOctave = toneDoublerMML;

            pw.mmlData[pos].args.Add(toneDoublerMML);
        }

        private void step2_CmdBend(partWork pw, int pos)
        {
            if (!(
                    (
                    pos > 0 
                    && pw.mmlData[pos - 1].type == enmMMLType.Note
                    )
                || 
                    (
                    pos > 1 
                    && pw.mmlData[pos - 1].type == enmMMLType.ToneDoubler 
                    && pw.mmlData[pos - 2].type == enmMMLType.Note
                    )
                ))
            {
                msgBox.setErrMsg(msg.get("E05039")
                , pw.mmlData[pos].line.Lp);
                return;
            }

            Note note = (Note)pw.mmlData[pos - (pw.mmlData[pos - 1].type == enmMMLType.Note ? 1 : 2)].args[0];

            //直前の音符コマンドへベンドコマンドが続くことを知らせる
            note.bendSw = true;

            //直後の音符コマンドまでサーチ
            Note bendNote = null;
            List<MML> bendMML = new List<MML>();
            for (int i = pos + 1; i < pw.mmlData.Count; i++)
            {
                switch (pw.mmlData[i].type)
                {
                    case enmMMLType.Note:
                        bendNote = (Note)pw.mmlData[i].args[0];
                        pw.mmlData.RemoveAt(i);
                        i--;
                        goto loop_exit;
                    case enmMMLType.Octave:
                    case enmMMLType.OctaveUp:
                    case enmMMLType.OctaveDown:
                        bendMML.Add(pw.mmlData[i]);
                        pw.mmlData.RemoveAt(i);
                        i--;
                        break;
                    default:
                        msgBox.setErrMsg(msg.get("E05040")
                        , pw.mmlData[i].line.Lp);
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
            pw.mmlData[pos].args = new List<object>();
            pw.mmlData[pos].args.Add(bendMML);
        }

        private void step2_CmdTiePC(partWork pw, int pos)
        {
            int nPos = 0;

            //遡ってnoteを探す
            for (int i = pos - 1; i > 0; i--)
            {
                switch (pw.mmlData[i].type)
                {
                    case enmMMLType.ToneDoubler:
                    case enmMMLType.Bend:
                        break;
                    case enmMMLType.Note:
                    case enmMMLType.Rest:
                    case enmMMLType.RestNoWork:
                        nPos = i;
                        goto loop_exit;
                    default:
                        msgBox.setErrMsg(msg.get("E05041")
                        , pw.mmlData[pos].line.Lp);
                        return;
                }
            }

            msgBox.setErrMsg(msg.get("E05042")
            , pw.mmlData[pos].line.Lp);
            return;
            loop_exit:

            Rest rest = (Rest)pw.mmlData[nPos].args[0];//NoteはRestを継承している
            rest.length += (int)pw.mmlData[pos].args[0];
        }

        private void step2_CmdTieMC(partWork pw, int pos)
        {
            int nPos = 0;

            //遡ってnoteを探す
            for (int i = pos - 1; i > 0; i--)
            {
                switch (pw.mmlData[i].type)
                {
                    case enmMMLType.ToneDoubler:
                    case enmMMLType.Bend:
                        break;
                    case enmMMLType.Note:
                    case enmMMLType.Rest:
                    case enmMMLType.RestNoWork:
                        nPos = i;
                        goto loop_exit;
                    default:
                        msgBox.setErrMsg(msg.get("E05043")
                        , pw.mmlData[pos].line.Lp);
                        return;
                }
            }

            msgBox.setErrMsg(msg.get("E05044")
            , pw.mmlData[pos].line.Lp);
            return;
            loop_exit:

            Rest rest = (Rest)pw.mmlData[nPos].args[0];//NoteはRestを継承している
            rest.length -= (int)pw.mmlData[pos].args[0];
        }

        private void step2_CmdTie(partWork pw, int pos)
        {
            int nPos = 0;

            //遡ってnoteを探す
            for (int i = pos - 1; i > 0; i--)
            {
                switch (pw.mmlData[i].type)
                {
                    case enmMMLType.ToneDoubler:
                    case enmMMLType.Bend:
                        break;
                    case enmMMLType.Note:
                        nPos = i;
                        goto loop_exit;
                    default:
                        msgBox.setErrMsg(msg.get("E05045")
                        , pw.mmlData[pos].line.Lp);
                        return;
                }
            }

            msgBox.setErrMsg(msg.get("E05046")
            , pw.mmlData[pos].line.Lp);
            return;
            loop_exit:

            //直前の音符コマンドへ&コマンドが続くことを知らせる
            Note note = (Note)pw.mmlData[nPos].args[0];
            note.tieSw = true;
        }

        #endregion

        #region step3

        private void Step3(partWork pw)
        {
            //リピート処理向けスタックのクリア
            pw.stackRepeat.Clear();
            pw.stackRenpu.Clear();

            for (int i = 0; i < pw.mmlData.Count; i++)
            {
                switch (pw.mmlData[i].type)
                {
                    case enmMMLType.Repeat:
                        step3_CmdRepeatStart(pw, i);
                        break;
                    case enmMMLType.RepertExit:
                        step3_CmdRepeatExit(pw, i);
                        break;
                    case enmMMLType.RepeatEnd:
                        step3_CmdRepeatEnd(pw, i);
                        break;
                    case enmMMLType.Renpu:
                        step3_CmdRenpuStart(pw, i);
                        break;
                    case enmMMLType.RenpuEnd:
                        step3_CmdRenpuEnd(pw, i);
                        break;
                    case enmMMLType.Note:
                    case enmMMLType.Rest:
                    case enmMMLType.RestNoWork:
                        step3_CmdNoteCount(pw, i);
                        break;
                }
            }
        }

        private void step3_CmdRepeatExit(partWork pw,int pos)
        {
            int nst = 0;

            for (int searchPos = pos; searchPos < pw.mmlData.Count; searchPos++)
            {
                if (pw.mmlData[searchPos].type == enmMMLType.Repeat)
                {
                    nst++;
                    continue;
                }
                if (pw.mmlData[searchPos].type != enmMMLType.RepeatEnd)
                {
                    continue;
                }
                if (nst == 0)
                {
                    pw.mmlData[pos].args = new List<object>();
                    pw.mmlData[pos].args.Add(searchPos);
                    return;
                }
                nst--;
            }

            msgBox.setWrnMsg(msg.get("E05047")
                , pw.mmlData[pos].line.Lp);

        }

        private void step3_CmdRepeatEnd(partWork pw,int pos)
        {
            try
            {
                clsRepeat re = pw.stackRepeat.Pop();
                pw.mmlData[pos].args.Add(re.pos);
            }
            catch
            {
                msgBox.setWrnMsg(msg.get("E05048")
                , pw.mmlData[pos].line.Lp);
            }
        }

        private void step3_CmdRepeatStart(partWork pw,int pos)
        {
            clsRepeat rs = new clsRepeat()
            {
                pos = pos,
                repeatCount = -1//初期値
            };
            pw.stackRepeat.Push(rs);
        }

        private void step3_CmdRenpuStart(partWork pw, int pos)
        {
            clsRenpu r = new clsRenpu();
            r.pos = pos;
            r.repeatStackCount = pw.stackRepeat.Count;
            r.noteCount = 0;
            r.mml = pw.mmlData[pos];
            pw.stackRenpu.Push(r);
        }

        private void step3_CmdRenpuEnd(partWork pw, int pos)
        {
            try
            {
                clsRenpu r = pw.stackRenpu.Pop();
                r.mml.args = new List<object>();
                r.mml.args.Add(r.noteCount);
                if (pw.mmlData[pos].args != null)
                {
                    r.mml.args.Add(pw.mmlData[pos].args[0]);//音長(クロック数)
                }

                if (r.repeatStackCount != pw.stackRepeat.Count)
                {
                    msgBox.setWrnMsg(msg.get("E05049")
                    , pw.mmlData[pos].line.Lp);
                }

                if (r.noteCount > 0)
                {
                    if (pw.stackRenpu.Count > 0)
                    {
                        pw.stackRenpu.First().noteCount++;
                    }
                }
            }
            catch
            {
                msgBox.setWrnMsg(msg.get("E05050")
                , pw.mmlData[pos].line.Lp);
            }
        }

        private void step3_CmdNoteCount(partWork pw, int pos)
        {
            if (pw.stackRenpu.Count < 1) return;

            pw.stackRenpu.First().noteCount++;
        }

        #endregion



    }
}
