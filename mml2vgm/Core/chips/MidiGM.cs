using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class MidiGM : ClsChip
    {
        private List<MIDINote> noteOns = new List<MIDINote>();
        private List<MIDINote> noteOffs = new List<MIDINote>();

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
                pw.port = port;
                if (i-- > 0)
                {
                    ResetPitchBend(pw, null);
                }
            }

        }

        public override void InitPart(partWork pw)
        {
            pw.volume = 127;
            pw.expression = 127;
            pw.beforeExpression = -1;
            pw.MaxVolume = 127;
            pw.MaxExpression = 127;
            pw.tblNoteOn = new bool[128];
            pw.directModeVib = false;
            pw.directModeTre = false;
            pw.pitchBend = 0;
        }

        public override void CmdMIDICh(partWork pw, MML mml)
        {
            int ch = (int)mml.args[0];
            pw.ch = ch & 0xf;
        }

        public override void CmdMIDIControlChange(partWork pw,MML mml)
        {
            int ctrl = (int)mml.args[0];
            int data = (int)mml.args[1];

            parent.OutData(mml, pw.port[0], 3, (byte)(0xb0 + (pw.ch & 0xf)), (byte)(ctrl & 0x7f), (byte)(data & 0x7f));
        }

        public override void CmdVelocity(partWork pw, MML mml)
        {
            int vel = (int)mml.args[0];
            pw.velocity = vel & 0x7f;

            MML vmml = new MML();
            vmml.type = enmMMLType.Velocity;
            vmml.args = new List<object>();
            vmml.args.Add(pw.velocity);
            vmml.line = mml.line;
            SetDummyData(pw, vmml);
        }

        public override void CmdPan(partWork pw, MML mml)
        {
            int pan = (int)mml.args[0];
            pw.panL = pan & 0x7f;
            OutMidiControlChange(pw, mml, enmControlChange.Panpot, (byte)pw.panL);
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
                pw.directModeVib = sw;
                ResetPitchBend(pw, mml);
            }
            else
            {
                //tre
                pw.directModeTre = sw;
            }
        }

        /// <summary>
        /// MIDI CC:Volume をわりあてている
        /// </summary>
        public override void CmdTotalVolume(partWork pw, MML mml)
        {
            int n = (int)mml.args[0];
            pw.volume = Common.CheckRange(n, 0, pw.MaxVolume);

            MML vmml = new MML();
            vmml.type = enmMMLType.TotalVolume;
            vmml.args = new List<object>();
            vmml.args.Add(pw.volume);
            vmml.line = mml.line;
            SetDummyData(pw, vmml);

            if (pw.beforeVolume == pw.volume) return;
            pw.beforeVolume = pw.volume;
            OutMidiControlChange(pw, mml, enmControlChange.Volume, (byte)pw.volume);
        }

        /// <summary>
        /// Expression
        /// </summary>
        /// <param name="pw"></param>
        /// <param name="mml"></param>
        public override void CmdVolume(partWork pw, MML mml)
        {
            int n;
            n = (mml.args != null && mml.args.Count > 0) ? (int)mml.args[0] : pw.latestVolume;
            pw.latestVolume = n;
            pw.expression = Common.CheckRange(n, 0, pw.MaxExpression);

            MML vmml = new MML();
            vmml.type = enmMMLType.Volume;
            vmml.args = new List<object>();
            vmml.args.Add(pw.expression);
            vmml.line = mml.line;
            SetDummyData(pw, vmml);

        }

        public override void CmdVolumeUp(partWork pw, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 1, pw.MaxExpression);
            pw.expression += n;
            pw.expression = Common.CheckRange(pw.expression, 0, pw.MaxExpression);

            MML vmml = new MML();
            vmml.type = enmMMLType.Volume;
            vmml.args = new List<object>();
            vmml.args.Add(pw.expression);
            vmml.line = mml.line;
            SetDummyData(pw, vmml);
        }

        public override void CmdVolumeDown(partWork pw, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 1, pw.MaxExpression);
            pw.expression -= n;
            pw.expression = Common.CheckRange(pw.expression, 0, pw.MaxExpression);

            MML vmml = new MML();
            vmml.type = enmMMLType.Volume;
            vmml.args = new List<object>();
            vmml.args.Add(pw.expression);
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
            pw.instrument = n;

            MML vmml = new MML();
            vmml.type = enmMMLType.Instrument;
            vmml.args = new List<object>();
            vmml.args.Add(pw.instrument);
            vmml.line = mml.line;
            SetDummyData(pw, vmml);

            OutMidiProgramChange(pw, mml, (byte)pw.instrument);
        }

        public override void CmdY(partWork pw, MML mml)
        {
            List<byte> dat = new List<byte>();
            foreach (object o in mml.args)
            {
                dat.Add((byte)o);
                if (dat.Count == 255)
                {
                    dat.Insert(0, 255);
                    parent.OutData(mml, pw.port[0], dat.ToArray());
                    dat.Clear();
                }
            }

            if (dat.Count == 0) return;

            dat.Insert(0, (byte)dat.Count);
            parent.OutData(mml, pw.port[0], dat.ToArray());
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
                    parent.OutData(mml, pw.port[0], dat.ToArray());
                    dat.Clear();
                }
            }

            if (dat.Count == 0) return;

            dat.Insert(0, (byte)dat.Count);
            parent.OutData(mml, pw.port[0], dat.ToArray());
        }

        public override void SetFNum(partWork pw, MML mml)
        {
            pw.portaBend = 0;
            if (pw.bendWaitCounter != -1)
            {
                pw.portaBend = pw.bendFnum;
            }
        }

        public override void SetVolume(partWork pw, MML mml)
        {
            //if (pw.beforeExpression == pw.expression) return;
            //pw.beforeExpression = pw.expression;
            //OutMidiControlChange(pw, mml, enmControlChange.Expression, (byte)pw.expression);

            int vol = pw.expression;

            //if (pw.keyOn)
            {
                if (pw.envelopeMode)
                {
                    vol = 0;
                    if (pw.envIndex != -1)
                    {
                        vol = pw.envVolume - (127 - pw.expression);
                    }
                }

                for (int lfo = 0; lfo < 4; lfo++)
                {
                    if (!pw.lfo[lfo].sw) continue;
                    if (pw.lfo[lfo].type != eLfoType.Tremolo) continue;

                    vol += pw.lfo[lfo].value + pw.lfo[lfo].param[6];
                }
            }
            //else
            {
                //vol = 0;
            }

            vol = Common.CheckRange(vol, 0, 127);

            if (pw.beforeExpression != vol)
            {
                if (!pw.directModeTre)
                {
                    OutMidiControlChange(pw, mml, enmControlChange.Expression, (byte)vol);
                }
                pw.beforeExpression = vol;
            }
        }

        public override void SetLfoAtKeyOn(partWork pw, MML mml)
        {
            for (int lfo = 0; lfo < 4; lfo++)
            {
                clsLfo pl = pw.lfo[lfo];
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
            pw.tieBend = 0;
            pw.bendStartOctave = pw.octaveNew;
            pw.bendStartNote = pw.noteCmd;
            pw.bendStartShift = pw.shift;
        }

        protected override void SetTieBend(partWork pw, MML mml)
        {
            if (pw.bendWaitCounter == -1)
            {
                int n = (pw.octaveNew * 12 + Const.NOTE.IndexOf(pw.noteCmd) + pw.shift) - (pw.bendStartOctave * 12 + Const.NOTE.IndexOf(pw.bendStartNote) + pw.bendStartShift);
                pw.tieBend = 8192 / 24 * n;
            }
        }

        public override void SetKeyOn(partWork pw, MML mml)
        {
            Note n = (Note)mml.args[0];
            byte noteNum;
            if (pw.bendWaitCounter == -1)
                noteNum = (byte)GetNoteNum(pw.octaveNew, n.cmd, n.shift + pw.keyShift);
            else
                noteNum = (byte)GetNoteNum(pw.bendStartOctave, pw.bendStartNote, pw.bendStartShift + pw.keyShift);
            pw.tblNoteOn[noteNum] = true;

            MIDINote mn = new MIDINote();
            mn.pw = pw;
            mn.mml = mml;
            mn.noteNumber = noteNum;
            mn.velocity = (byte)(n.velocity==-1 ? pw.velocity : n.velocity);
            noteOns.Add(mn);
        }

        public override void SetKeyOff(partWork pw, MML mml)
        {
            for (byte i = 0; i < pw.tblNoteOn.Length; i++)
            {
                if (!pw.tblNoteOn[i]) continue;
                pw.tblNoteOn[i] = false;

                //MIDINote mn = new MIDINote();
                //mn.pw = pw;
                //mn.mml = mml;
                //mn.noteNumber = i;
                //mn.velocity = 0;
                //noteOffs.Add(mn);
                OutMidiNoteOn(pw, mml, i, 0);
            }
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
            int n = (octave * 12 + Const.NOTE.IndexOf(cmd) + shift) - (pw.bendStartOctave * 12 + Const.NOTE.IndexOf(pw.bendStartNote) + pw.bendStartShift);
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
            parent.OutData(mml, pw.port[0], 3, (byte)(0x80 + (pw.ch & 0xf)), (byte)(noteNumber & 0x7f), (byte)(velocity & 0x7f));
        }

        private void OutMidiNoteOn(partWork pw, MML mml, byte noteNumber, byte velocity)
        {
            parent.OutData(mml, pw.port[0], 3, (byte)(0x90 + (pw.ch & 0xf)), (byte)(noteNumber & 0x7f), (byte)(velocity & 0x7f));
        }

        private void OutMidiPolyKeyPress(partWork pw, MML mml, byte noteNumber, byte press)
        {
            parent.OutData(mml, pw.port[0], 3, (byte)(0xa0 + (pw.ch & 0xf)), (byte)(noteNumber & 0x7f), (byte)(press & 0x7f));
        }

        private void OutMidiControlChange(partWork pw, MML mml, enmControlChange ctrl, byte data)
        {
            parent.OutData(mml, pw.port[0], 3, (byte)(0xb0 + (pw.ch & 0xf)), (byte)ctrl, (byte)(data & 0x7f));
        }

        private void OutMidiProgramChange(partWork pw, MML mml, byte programNumber)
        {
            parent.OutData(mml, pw.port[0], 2, (byte)(0xc0 + (pw.ch & 0xf)), programNumber);//, (byte)0);
        }

        private void OutMidiChannelPress(partWork pw, MML mml, byte press)
        {
            parent.OutData(mml, pw.port[0], 3, (byte)(0xd0 + (pw.ch & 0xf)), press, (byte)0);
        }

        private void OutMidiPitchBend(partWork pw, MML mml, byte msb, byte lsb)
        {
            parent.OutData(mml, pw.port[0], 3, (byte)(0xe0 + (pw.ch & 0xf)), (byte)(lsb & 0x7f), (byte)(msb & 0x7f));
        }

        private void OutMidi(partWork pw, MML mml)
        {
            //parent.OutData(mml, pw.port[0]);
        }

        public override void MultiChannelCommand(MML mml)
        {
            if (!use) return;
            //コマンドを跨ぐデータ向け処理
            foreach (partWork pw in lstPartWork)
            {
                pw.lfoBend = 0;
                for (int lfo = 0; lfo < 4; lfo++)
                {
                    if (!pw.lfo[lfo].sw)
                    {
                        continue;
                    }
                    if (pw.lfo[lfo].type != eLfoType.Vibrato)
                    {
                        continue;
                    }
                    pw.lfoBend += pw.lfo[lfo].value + pw.lfo[lfo].param[6];
                }

                if (pw.pitchBend != (pw.bendWaitCounter==-1 ? pw.tieBend :0)+ pw.portaBend + pw.lfoBend + pw.detune)
                {
                    pw.pitchBend = (pw.bendWaitCounter == -1 ? pw.tieBend : 0) + pw.portaBend + pw.lfoBend + pw.detune;
                    if (!pw.directModeVib)
                    {
                        int pb = (pw.pitchBend + 8192) & 0x3fff;
                        OutMidiPitchBend(pw, mml, (byte)(pb / 128), (byte)(pb % 128));
                    }
                }

                //int exp = Common.CheckRange(pw.expression + pw.envExpression + pw.lfoExpression, 0, 127);
                //if (pw.beforeExpression != exp)
                //{
                //    pw.beforeExpression = exp;

                //    if (!pw.directModeTre)
                //    {
                //        OutMidiControlChange(pw, mml, enmControlChange.Expression, (byte)exp);
                //    }
                //}

            }

            log.Write("KeyOn情報をかき出し");
            foreach (MIDINote n in noteOns) OutMidiNoteOn(n.pw, n.mml, n.noteNumber, n.velocity);
            noteOns.Clear();
        }

        public class MIDINote
        {
            public partWork pw { get; internal set; }
            public MML mml { get; internal set; }
            public byte noteNumber { get; internal set; }
            public byte velocity { get; internal set; }
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
