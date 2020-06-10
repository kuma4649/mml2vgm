using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using musicDriverInterface;

namespace Core
{
    public class MidiGM : ClsChip
    {

        public MidiGM(ClsVgm parent, int chipID, string initialPartName, string stPath, int chipNumber) : base(parent, chipID, initialPartName, stPath, chipNumber)
        {
            _chipType = enmChipType.MIDI_GM;
            _Name = "GeneralMIDI";
            _ShortName = "GM";
            _ChMax = 99;
            _canUsePcm = false;
            _canUsePI = false;
            ChipNumber = chipNumber;

            Frequency = 0;
            port = new byte[][] { new byte[] { 0x00 } };
            dataType = 0;

            if (string.IsNullOrEmpty(initialPartName)) return;

            Ch = new ClsChannel[ChMax];
            SetPartToCh(Ch, initialPartName);
            foreach (ClsChannel ch in Ch)
            {
                ch.Type = enmChannelType.MIDI;
                ch.chipNumber = chipID == 1;
                ch.MaxVolume = 127;
            }

            pcmDataInfo = null;

        }

        public override void InitChip()
        {
            if (!use) return;

            //initialize shared param

            //All Key Off
            int i = 16;
            foreach (partWork pw in lstPartWork)
            {
                pw.pg[pw.cpg].port = port;
                if (i-- > 0)
                {
                    ResetPitchBend(pw, null);
                }
            }

        }

        public override void InitPart(partWork pw)
        {
            pw.pg[pw.cpg].volume = 127;
            pw.pg[pw.cpg].expression = 127;
            pw.pg[pw.cpg].beforeExpression = -1;
            pw.pg[pw.cpg].MaxVolume = 127;
            pw.pg[pw.cpg].MaxExpression = 127;
            pw.pg[pw.cpg].tblNoteOn = new bool[128];
            pw.pg[pw.cpg].directModeVib = false;
            pw.pg[pw.cpg].directModeTre = false;
            pw.pg[pw.cpg].pitchBend = 0;
            pw.pg[pw.cpg].MIDIch = pw.pg[pw.cpg].ch % 16;
        }

        public override void CmdMIDICh(partWork pw, MML mml)
        {
            int ch = (int)mml.args[0];
            pw.pg[pw.cpg].MIDIch = ch & 0xf;

            MML vmml = new MML();
            vmml.type = enmMMLType.MIDICh;
            vmml.args = new List<object>();
            vmml.args.Add(pw.pg[pw.cpg].MIDIch);
            vmml.line = mml.line;
            SetDummyData(pw, vmml);
        }

        public override void CmdMIDIControlChange(partWork pw,MML mml)
        {
            int ctrl = (int)mml.args[0];
            int data = (int)mml.args[1];

            parent.OutData(mml, pw.pg[pw.cpg].port[0], 3, (byte)(0xb0 + pw.pg[pw.cpg].MIDIch), (byte)(ctrl & 0x7f), (byte)(data & 0x7f));
        }

        public override void CmdVelocity(partWork pw, MML mml)
        {
            int vel = (int)mml.args[0];
            pw.pg[pw.cpg].velocity = vel & 0x7f;

            MML vmml = new MML();
            vmml.type = enmMMLType.Velocity;
            vmml.args = new List<object>();
            vmml.args.Add(pw.pg[pw.cpg].velocity);
            vmml.line = mml.line;
            SetDummyData(pw, vmml);
        }

        public override void CmdPan(partWork pw, MML mml)
        {
            int pan = (int)mml.args[0];
            pw.pg[pw.cpg].panL = pan & 0x7f;
            OutMidiControlChange(pw, mml, enmControlChange.Panpot, (byte)pw.pg[pw.cpg].panL);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void CmdDirectMode(partWork pw, MML mml)
        {
            bool sw = (bool)mml.args[0];
            int vt = 0;
            if (mml.args.Count != 2 || ((char)mml.args[1] != 'V' && (char)mml.args[1] != 'T'))
            {
                msgBox.setWrnMsg(msg.get("W25000"), mml.line.Lp);
                return;
            }
            vt = ((char)mml.args[1] == 'V') ? 0 : 1;

            if (vt == 0)
            {
                //vib
                pw.pg[pw.cpg].directModeVib = sw;
                ResetPitchBend(pw, mml);
            }
            else
            {
                //tre
                pw.pg[pw.cpg].directModeTre = sw;
            }
        }

        /// <summary>
        /// MIDI CC:Volume をわりあてている
        /// </summary>
        public override void CmdTotalVolume(partWork pw, MML mml)
        {
            int n = (int)mml.args[0];
            pw.pg[pw.cpg].volume = Common.CheckRange(n, 0, pw.pg[pw.cpg].MaxVolume);

            MML vmml = new MML();
            vmml.type = enmMMLType.TotalVolume;
            vmml.args = new List<object>();
            vmml.args.Add(pw.pg[pw.cpg].volume);
            vmml.line = mml.line;
            SetDummyData(pw, vmml);

            if (pw.pg[pw.cpg].beforeVolume == pw.pg[pw.cpg].volume) return;
            pw.pg[pw.cpg].beforeVolume = pw.pg[pw.cpg].volume;
            OutMidiControlChange(pw, mml, enmControlChange.Volume, (byte)pw.pg[pw.cpg].volume);
        }

        /// <summary>
        /// Expression
        /// </summary>
        /// <param name="pw"></param>
        /// <param name="mml"></param>
        public override void CmdVolume(partWork pw, MML mml)
        {
            int n;
            n = (mml.args != null && mml.args.Count > 0) ? (int)mml.args[0] : pw.pg[pw.cpg].latestVolume;
            pw.pg[pw.cpg].latestVolume = n;
            pw.pg[pw.cpg].expression = Common.CheckRange(n, 0, pw.pg[pw.cpg].MaxExpression);

            MML vmml = new MML();
            vmml.type = enmMMLType.Volume;
            vmml.args = new List<object>();
            vmml.args.Add(pw.pg[pw.cpg].expression);
            vmml.line = mml.line;
            SetDummyData(pw, vmml);

        }

        public override void CmdVolumeUp(partWork pw, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 1, pw.pg[pw.cpg].MaxExpression);
            pw.pg[pw.cpg].expression += parent.info.volumeRev ? -n : n;
            pw.pg[pw.cpg].expression = Common.CheckRange(pw.pg[pw.cpg].expression, 0, pw.pg[pw.cpg].MaxExpression);

            MML vmml = new MML();
            vmml.type = enmMMLType.Volume;
            vmml.args = new List<object>();
            vmml.args.Add(pw.pg[pw.cpg].expression);
            vmml.line = mml.line;
            SetDummyData(pw, vmml);
        }

        public override void CmdVolumeDown(partWork pw, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 1, pw.pg[pw.cpg].MaxExpression);
            pw.pg[pw.cpg].expression -= parent.info.volumeRev ? -n : n;
            pw.pg[pw.cpg].expression = Common.CheckRange(pw.pg[pw.cpg].expression, 0, pw.pg[pw.cpg].MaxExpression);

            MML vmml = new MML();
            vmml.type = enmMMLType.Volume;
            vmml.args = new List<object>();
            vmml.args.Add(pw.pg[pw.cpg].expression);
            vmml.line = mml.line;
            SetDummyData(pw, vmml);
        }

        public override void CmdLoopExtProc(partWork pw, MML mml)
        {
        }

        public override void CmdInstrument(partWork pw, MML mml)
        {
            char type = (char)mml.args[0];
            int n = (int)mml.args[1];

            if (type == 'E')
            {
                n = SetEnvelopParamFromInstrument(pw, n, mml);
                return;
            }
            else if (type == 'S')
            {
                SendSysEx(pw, mml, n);
                return;
            }

            n = Common.CheckRange(n, 0, 127);
            pw.pg[pw.cpg].instrument = n;

            MML vmml = new MML();
            vmml.type = enmMMLType.Instrument;
            vmml.args = new List<object>();
            vmml.args.Add(pw.pg[pw.cpg].instrument);
            vmml.line = mml.line;
            SetDummyData(pw, vmml);

            OutMidiProgramChange(pw, mml, (byte)pw.pg[pw.cpg].instrument);
        }

        public override void CmdY(partWork pw, MML mml)
        {
            List<byte> dat = new List<byte>();
            foreach (object o in mml.args)
            {
                dat.Add((byte)(int)o);
                if (dat.Count == 255)
                {
                    dat.Insert(0, 255);
                    parent.OutData(mml, pw.pg[pw.cpg].port[0], dat.ToArray());
                    dat.Clear();
                }
            }

            if (dat.Count == 0) return;

            dat.Insert(0, (byte)dat.Count);
            parent.OutData(mml, pw.pg[pw.cpg].port[0], dat.ToArray());
        }


        private void SendSysEx(partWork pw, MML mml, int n)
        {
            if (!parent.midiSysEx.ContainsKey(n)) return;

            byte[] data = parent.midiSysEx[n];
            if (data == null || data.Length < 1) return;

            List<byte> dat = new List<byte>();
            int c = 0;
            foreach (object o in data)
            {
                c++;
                if (c == 1) continue;

                dat.Add((byte)o);
                if (dat.Count == 255)
                {
                    dat.Insert(0, 255);
                    parent.OutData(mml, pw.pg[pw.cpg].port[0], dat.ToArray());
                    dat.Clear();
                }
            }

            if (dat.Count == 0) return;

            dat.Insert(0, (byte)dat.Count);
            parent.OutData(mml, pw.pg[pw.cpg].port[0], dat.ToArray());
        }

        public override void SetFNum(partWork pw, MML mml)
        {
            pw.pg[pw.cpg].portaBend = 0;
            if (pw.pg[pw.cpg].bendWaitCounter != -1)
            {
                pw.pg[pw.cpg].portaBend = pw.pg[pw.cpg].bendFnum;
            }
        }

        public override void SetVolume(partWork pw, MML mml)
        {
            //if (pw.ppg[pw.cpgNum].beforeExpression == pw.ppg[pw.cpgNum].expression) return;
            //pw.ppg[pw.cpgNum].beforeExpression = pw.ppg[pw.cpgNum].expression;
            //OutMidiControlChange(pw, mml, enmControlChange.Expression, (byte)pw.ppg[pw.cpgNum].expression);

            int vol = pw.pg[pw.cpg].expression;

            //if (pw.ppg[pw.cpgNum].keyOn)
            {
                if (pw.pg[pw.cpg].envelopeMode)
                {
                    vol = 0;
                    if (pw.pg[pw.cpg].envIndex != -1)
                    {
                        vol = pw.pg[pw.cpg].envVolume - (127 - pw.pg[pw.cpg].expression);
                    }
                }

                for (int lfo = 0; lfo < 4; lfo++)
                {
                    if (!pw.pg[pw.cpg].lfo[lfo].sw) continue;
                    if (pw.pg[pw.cpg].lfo[lfo].type != eLfoType.Tremolo) continue;

                    vol += pw.pg[pw.cpg].lfo[lfo].value + pw.pg[pw.cpg].lfo[lfo].param[6];
                }
            }
            //else
            {
                //vol = 0;
            }

            vol = Common.CheckRange(vol, 0, 127);

            if (pw.pg[pw.cpg].beforeExpression != vol)
            {
                if (!pw.pg[pw.cpg].directModeTre)
                {
                    OutMidiControlChange(pw, mml, enmControlChange.Expression, (byte)vol);
                }
                pw.pg[pw.cpg].beforeExpression = vol;
            }
        }

        public override void SetLfoAtKeyOn(partWork pw, MML mml)
        {
            for (int lfo = 0; lfo < 4; lfo++)
            {
                clsLfo pl = pw.pg[pw.cpg].lfo[lfo];
                if (!pl.sw)
                    continue;

                if (pl.param[5] != 1)
                    continue;

                pl.isEnd = false;
                pl.value = (pl.param[0] == 0) ? pl.param[6] : 0;//ディレイ中は振幅補正は適用されない
                pl.waitCounter = pl.param[0];
                pl.direction = pl.param[2] < 0 ? -1 : 1;
                pl.depthWaitCounter = pl.param[7];
                pl.depth = pl.param[3];
                pl.depthV2 = pl.param[2];

            }
        }

        public int GetNoteNum(int octave, char noteCmd, int shift)
        {
            int o = octave;
            int n = Const.NOTE.IndexOf(noteCmd) + shift;

            o += n / 12;
            n %= 12;
            if (n < 0)
            {
                n += 12;
                o = Common.CheckRange(--o, 1, 8);
            }

            return o * 12 + n;
        }

        protected override void ResetTieBend(partWork pw, MML mml)
        {
            pw.pg[pw.cpg].tieBend = 0;
            pw.pg[pw.cpg].bendStartOctave = pw.pg[pw.cpg].octaveNew;
            pw.pg[pw.cpg].bendStartNote = pw.pg[pw.cpg].noteCmd;
            pw.pg[pw.cpg].bendStartShift = pw.pg[pw.cpg].shift;
        }

        protected override void SetTieBend(partWork pw, MML mml)
        {
            if (pw.pg[pw.cpg].bendWaitCounter == -1)
            {
                int n = (pw.pg[pw.cpg].octaveNew * 12 + Const.NOTE.IndexOf(pw.pg[pw.cpg].noteCmd) + pw.pg[pw.cpg].shift) - (pw.pg[pw.cpg].bendStartOctave * 12 + Const.NOTE.IndexOf(pw.pg[pw.cpg].bendStartNote) + pw.pg[pw.cpg].bendStartShift);
                pw.pg[pw.cpg].tieBend = 8192 / 24 * n;
            }
        }

        public override void SetKeyOn(partWork pw, MML mml)
        {
            Note n = (Note)mml.args[0];
            byte noteNum;
            if (pw.pg[pw.cpg].bendWaitCounter == -1)
            {
                noteNum = (byte)GetNoteNum(pw.pg[pw.cpg].octaveNew, n.cmd, n.shift + pw.pg[pw.cpg].keyShift);
                if (!pw.pg[pw.cpg].beforeTie) pw.pg[pw.cpg].beforeBendNoteNum = -1;
            }
            else
            {
                noteNum = (byte)GetNoteNum(pw.pg[pw.cpg].bendStartOctave, pw.pg[pw.cpg].bendStartNote, pw.pg[pw.cpg].bendStartShift + pw.pg[pw.cpg].keyShift);
                pw.pg[pw.cpg].beforeBendNoteNum = noteNum;
            }
            pw.pg[pw.cpg].tblNoteOn[noteNum] = true;

            MIDINote mn = new MIDINote();
            mn.pw = pw;
            mn.mml = mml;
            mn.noteNumber = noteNum;
            mn.velocity = (byte)(n.velocity==-1 ? pw.pg[pw.cpg].velocity : n.velocity);
            mn.length = pw.pg[pw.cpg].tie ? -1 : pw.pg[pw.cpg].waitKeyOnCounter;
            mn.beforeKeyon = null;
            if (pw.pg[pw.cpg].beforeTie)
            {
                mn.beforeKeyon = true;
                if (pw.pg[pw.cpg].beforeBendNoteNum != -1)
                {
                    noteNum = (byte)pw.pg[pw.cpg].beforeBendNoteNum;
                    mn.noteNumber = noteNum;
                }
            }
            mn.Keyon = true;
            pw.pg[pw.cpg].noteOns[noteNum] = mn;
        }

        public override void SetKeyOff(partWork pw, MML mml)
        {
            return;

            //for (byte i = 0; i < pw.ppg[pw.cpgNum].tblNoteOn.Length; i++)
            //{
            //    if (!pw.ppg[pw.cpgNum].tblNoteOn[i]) continue;
            //    pw.ppg[pw.cpgNum].tblNoteOn[i] = false;

            //    pw.ppg[pw.cpgNum].noteOns[i].Keyon = false;
            //    pw.ppg[pw.cpgNum].noteOns[i].length = 0;

            //    //MIDINote mn = new MIDINote();
            //    //mn.pw = pw;
            //    //mn.mml = mml;
            //    //mn.noteNumber = i;
            //    //mn.velocity = 0;
            //    //noteOffs.Add(mn);
            //    //OutMidiNoteOn(pw, mml, i, 0);
            //}
        }

        public override int GetToneDoublerShift(partWork pw, int octave, char noteCmd, int shift)
        {
            return 0;
        }

        public override void SetToneDoubler(partWork pw, MML mml)
        {
        }

        public override int GetFNum(partWork pw, MML mml, int octave, char cmd, int shift)
        {
            int n = (octave * 12 + Const.NOTE.IndexOf(cmd) + shift) - (pw.pg[pw.cpg].bendStartOctave * 12 + Const.NOTE.IndexOf(pw.pg[pw.cpg].bendStartNote) + pw.pg[pw.cpg].bendStartShift);
            return 8192 / 24 * n;
        }

        private void ResetPitchBend(partWork pw, MML mml)
        {
            //ピッチベンドとセンシビリティを送信
            OutMidiPitchBend(pw, mml, 8192 / 128, 8192 % 128);

            OutMidiControlChange(pw, mml, enmControlChange.RPN_MSB, 0);//MSB先
            OutMidiControlChange(pw, mml, enmControlChange.RPN_LSB, 0);
            OutMidiControlChange(pw, mml, enmControlChange.DataEntryMSB, 24);//センシビリティを24(2オクターブ分)に設定
        }



        private void OutMidiNoteOff(partWork pw, MML mml, byte noteNumber, byte velocity)
        {
            parent.OutData(mml, pw.pg[pw.cpg].port[0], 3, (byte)(0x80 + pw.pg[pw.cpg].MIDIch), (byte)(noteNumber & 0x7f), (byte)(velocity & 0x7f));
        }

        private void OutMidiNoteOn(partWork pw, MML mml, byte noteNumber, byte velocity)
        {
            parent.OutData(mml, pw.pg[pw.cpg].port[0], 3, (byte)(0x90 + pw.pg[pw.cpg].MIDIch), (byte)(noteNumber & 0x7f), (byte)(velocity & 0x7f));
        }

        private void OutMidiPolyKeyPress(partWork pw, MML mml, byte noteNumber, byte press)
        {
            parent.OutData(mml, pw.pg[pw.cpg].port[0], 3, (byte)(0xa0 + pw.pg[pw.cpg].MIDIch), (byte)(noteNumber & 0x7f), (byte)(press & 0x7f));
        }

        private void OutMidiControlChange(partWork pw, MML mml, enmControlChange ctrl, byte data)
        {
            parent.OutData(mml, pw.pg[pw.cpg].port[0], 3, (byte)(0xb0 + pw.pg[pw.cpg].MIDIch), (byte)ctrl, (byte)(data & 0x7f));
        }

        private void OutMidiProgramChange(partWork pw, MML mml, byte programNumber)
        {
            parent.OutData(mml, pw.pg[pw.cpg].port[0], 2, (byte)(0xc0 + pw.pg[pw.cpg].MIDIch), programNumber);//, (byte)0);
        }

        private void OutMidiChannelPress(partWork pw, MML mml, byte press)
        {
            parent.OutData(mml, pw.pg[pw.cpg].port[0], 3, (byte)(0xd0 + pw.pg[pw.cpg].MIDIch), press, (byte)0);
        }

        private void OutMidiPitchBend(partWork pw, MML mml, byte msb, byte lsb)
        {
            parent.OutData(mml, pw.pg[pw.cpg].port[0], 3, (byte)(0xe0 + pw.pg[pw.cpg].MIDIch), (byte)(lsb & 0x7f), (byte)(msb & 0x7f));
        }

        private void OutMidi(partWork pw, MML mml)
        {
            //parent.OutData(mml, pw.ppg[pw.cpgNum].port[0]);
        }

        public override void MultiChannelCommand(MML mml)
        {
            if (!use) return;
            //コマンドを跨ぐデータ向け処理
            foreach (partWork pw in lstPartWork)
            {
                pw.pg[pw.cpg].lfoBend = 0;
                for (int lfo = 0; lfo < 4; lfo++)
                {
                    if (!pw.pg[pw.cpg].lfo[lfo].sw)
                    {
                        continue;
                    }
                    if (pw.pg[pw.cpg].lfo[lfo].type != eLfoType.Vibrato)
                    {
                        continue;
                    }
                    pw.pg[pw.cpg].lfoBend += pw.pg[pw.cpg].lfo[lfo].value + pw.pg[pw.cpg].lfo[lfo].param[6];
                }

                if (pw.pg[pw.cpg].pitchBend != (pw.pg[pw.cpg].bendWaitCounter==-1 ? pw.pg[pw.cpg].tieBend :0)+ pw.pg[pw.cpg].portaBend + pw.pg[pw.cpg].lfoBend + pw.pg[pw.cpg].detune)
                {
                    pw.pg[pw.cpg].pitchBend = (pw.pg[pw.cpg].bendWaitCounter == -1 ? pw.pg[pw.cpg].tieBend : 0) + pw.pg[pw.cpg].portaBend + pw.pg[pw.cpg].lfoBend + pw.pg[pw.cpg].detune;
                    if (!pw.pg[pw.cpg].directModeVib)
                    {
                        int pb = (pw.pg[pw.cpg].pitchBend + 8192) & 0x3fff;
                        OutMidiPitchBend(pw, mml, (byte)(pb / 128), (byte)(pb % 128));
                    }
                }

                //int exp = Common.CheckRange(pw.ppg[pw.cpgNum].expression + pw.ppg[pw.cpgNum].envExpression + pw.ppg[pw.cpgNum].lfoExpression, 0, 127);
                //if (pw.ppg[pw.cpgNum].beforeExpression != exp)
                //{
                //    pw.ppg[pw.cpgNum].beforeExpression = exp;

                //    if (!pw.ppg[pw.cpgNum].directModeTre)
                //    {
                //        OutMidiControlChange(pw, mml, enmControlChange.Expression, (byte)exp);
                //    }
                //}

                log.Write("KeyOn情報をかき出し");
                foreach (MIDINote n in pw.pg[pw.cpg].noteOns)
                {
                    if (n == null) continue;
                    if (n.beforeKeyon != n.Keyon)
                    {
                        if (n.Keyon)
                            OutMidiNoteOn(n.pw, null , n.noteNumber, n.velocity);//mmlはわたさない
                        else
                            OutMidiNoteOff(n.pw, null , n.noteNumber, n.velocity);//mmlはわたさない
                        n.beforeKeyon = n.Keyon;
                    }
                }
            }

        }

        public enum enmControlChange : byte
        {
            BankSelectMSB = 0,
            Modulation = 1,
            Bless = 2,
            Foot = 4,
            PortaTime = 5,
            DataEntryMSB = 6,
            Volume = 7,
            Balance = 8,
            Panpot = 10,
            Expression = 11,
            RPN_LSB=100,
            RPN_MSB=101
        }
    }
}
