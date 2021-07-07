using musicDriverInterface;
using System.Collections.Generic;

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
                foreach (partPage pg in pw.pg)
                {
                    pg.port = port;
                    if (i-- > 0)
                    {
                        ResetPitchBend(pg, null);
                    }
                }
            }

        }

        public override void InitPart(partWork pw)
        {
            foreach (partPage pg in pw.pg)
            {
                pg.volume = 127;
                pg.expression = 127;
                pg.beforeExpression = -1;
                pg.MaxVolume = 127;
                pg.MaxExpression = 127;
                pg.tblNoteOn = new bool[128];
                pg.directModeVib = false;
                pg.directModeTre = false;
                pg.pitchBend = 0;
                pg.MIDIch = pg.ch % 16;
            }
        }

        public override void CmdMIDICh(partPage page, MML mml)
        {
            int ch = (int)mml.args[0];
            page.MIDIch = ch & 0xf;

            MML vmml = new MML();
            vmml.type = enmMMLType.MIDICh;
            vmml.args = new List<object>();
            vmml.args.Add(page.MIDIch);
            vmml.line = mml.line;
            SetDummyData(page, vmml);
        }

        public override void CmdMIDIControlChange(partPage page, MML mml)
        {
            int ctrl = (int)mml.args[0];
            int data = (int)mml.args[1];

            parent.OutData(mml, page.port[0], 3, (byte)(0xb0 + page.MIDIch), (byte)(ctrl & 0x7f), (byte)(data & 0x7f));
        }

        public override void CmdVelocity(partPage page, MML mml)
        {
            int vel = (int)mml.args[0];
            page.velocity = vel & 0x7f;

            MML vmml = new MML();
            vmml.type = enmMMLType.Velocity;
            vmml.args = new List<object>();
            vmml.args.Add(page.velocity);
            vmml.line = mml.line;
            SetDummyData(page, vmml);
        }

        public override void CmdPan(partPage page, MML mml)
        {
            int pan = (int)mml.args[0];
            page.panL = pan & 0x7f;
            OutMidiControlChange(page, mml, enmControlChange.Panpot, (byte)page.panL);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void CmdDirectMode(partPage page, MML mml)
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
                page.directModeVib = sw;
                ResetPitchBend(page, mml);
            }
            else
            {
                //tre
                page.directModeTre = sw;
            }
        }

        /// <summary>
        /// MIDI CC:Volume をわりあてている
        /// </summary>
        public override void CmdTotalVolume(partPage page, MML mml)
        {
            int n = (int)mml.args[0];
            page.volume = Common.CheckRange(n, 0, page.MaxVolume);

            MML vmml = new MML();
            vmml.type = enmMMLType.TotalVolume;
            vmml.args = new List<object>();
            vmml.args.Add(page.volume);
            vmml.line = mml.line;
            SetDummyData(page, vmml);

            if (page.beforeVolume == page.volume) return;
            page.beforeVolume = page.volume;
            OutMidiControlChange(page, mml, enmControlChange.Volume, (byte)page.volume);
        }

        /// <summary>
        /// Expression
        /// </summary>
        /// <param name="pw"></param>
        /// <param name="mml"></param>
        public override void CmdVolume(partPage page, MML mml)
        {
            int n;
            n = (mml.args != null && mml.args.Count > 0) ? (int)mml.args[0] : page.latestVolume;
            page.latestVolume = n;
            page.expression = Common.CheckRange(n, 0, page.MaxExpression);

            MML vmml = new MML();
            vmml.type = enmMMLType.Volume;
            vmml.args = new List<object>();
            vmml.args.Add(page.expression);
            vmml.line = mml.line;
            SetDummyData(page, vmml);

        }

        public override void CmdVolumeUp(partPage page, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 1, page.MaxExpression);
            page.expression += parent.info.volumeRev ? -n : n;
            page.expression = Common.CheckRange(page.expression, 0, page.MaxExpression);

            MML vmml = new MML();
            vmml.type = enmMMLType.Volume;
            vmml.args = new List<object>();
            vmml.args.Add(page.expression);
            vmml.line = mml.line;
            SetDummyData(page, vmml);
        }

        public override void CmdVolumeDown(partPage page, MML mml)
        {
            int n = (int)mml.args[0];
            n = Common.CheckRange(n, 1, page.MaxExpression);
            page.expression -= parent.info.volumeRev ? -n : n;
            page.expression = Common.CheckRange(page.expression, 0, page.MaxExpression);

            MML vmml = new MML();
            vmml.type = enmMMLType.Volume;
            vmml.args = new List<object>();
            vmml.args.Add(page.expression);
            vmml.line = mml.line;
            SetDummyData(page, vmml);
        }

        public override void CmdLoopExtProc(partPage page, MML mml)
        {
        }

        public override void CmdInstrument(partPage page, MML mml)
        {
            char type;
            bool re = false;
            int n;
            if (mml.args[0] is bool)
            {
                type = (char)mml.args[1];
                re = true;
                n = (int)mml.args[2];
            }
            else
            {
                type = (char)mml.args[0];
                n = (int)mml.args[1];
            }

            if (type == 'E')
            {
                n = SetEnvelopParamFromInstrument(page, n,re, mml);
                return;
            }
            else if (type == 'S')
            {
                if (re) n = page.instrument + n;
                SendSysEx(page, mml, n);
                return;
            }

            if (re) n = page.instrument + n;
            n = Common.CheckRange(n, 0, 127);
            page.instrument = n;

            MML vmml = new MML();
            vmml.type = enmMMLType.Instrument;
            vmml.args = new List<object>();
            vmml.args.Add(page.instrument);
            vmml.line = mml.line;
            SetDummyData(page, vmml);

            OutMidiProgramChange(page, mml, (byte)page.instrument);
        }

        public override void CmdY(partPage page, MML mml)
        {
            List<byte> dat = new List<byte>();
            foreach (object o in mml.args)
            {
                dat.Add((byte)(int)o);
                if (dat.Count == 255)
                {
                    dat.Insert(0, 255);
                    parent.OutData(mml, page.port[0], dat.ToArray());
                    dat.Clear();
                }
            }

            if (dat.Count == 0) return;

            dat.Insert(0, (byte)dat.Count);
            parent.OutData(mml, page.port[0], dat.ToArray());
        }

        public override void CmdDetune(partPage page, MML mml)
        {
            string cmd = (string)mml.args[0];
            int n = (int)mml.args[1];

            switch (cmd)
            {
                case "D":
                    page.detune = n;
                    break;
                case "D>":
                    page.detune += parent.info.octaveRev ? -n : n;
                    break;
                case "D<":
                    page.detune += parent.info.octaveRev ? n : -n;
                    break;
            }
            page.detune = Common.CheckRange(page.detune, -0xffff, 0xffff);

            SetDummyData(page, mml);
        }



        private void SendSysEx(partPage page, MML mml, int n)
        {
            if (!parent.midiSysEx.ContainsKey(n)) return;

            byte[] data = parent.midiSysEx[n].Item2;
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
                    parent.OutData(mml, page.port[0], dat.ToArray());
                    dat.Clear();
                }
            }

            if (dat.Count == 0) return;

            dat.Insert(0, (byte)dat.Count);
            parent.OutData(mml, page.port[0], dat.ToArray());
        }

        public override void SetFNum(partPage page, MML mml)
        {
            page.portaBend = 0;
            if (page.bendWaitCounter != -1)
            {
                page.portaBend = page.bendFnum;
            }
        }

        public override void SetVolume(partPage page, MML mml)
        {
            //if (pw.ppg[pw.cpgNum].beforeExpression == pw.ppg[pw.cpgNum].expression) return;
            //pw.ppg[pw.cpgNum].beforeExpression = pw.ppg[pw.cpgNum].expression;
            //OutMidiControlChange(pw, mml, enmControlChange.Expression, (byte)pw.ppg[pw.cpgNum].expression);

            int vol = page.expression;

            //if (pw.ppg[pw.cpgNum].keyOn)
            {
                if (page.envelopeMode)
                {
                    vol = 0;
                    if (page.envIndex != -1)
                    {
                        vol = page.envVolume - (127 - page.expression);
                    }
                }

                for (int lfo = 0; lfo < 4; lfo++)
                {
                    if (!page.lfo[lfo].sw) continue;
                    if (page.lfo[lfo].type != eLfoType.Tremolo) continue;

                    vol += page.lfo[lfo].value + page.lfo[lfo].param[6];
                }

                if (page.varpeggioMode)
                {
                    vol += page.varpDelta;
                }

            }
            //else
            {
                //vol = 0;
            }

            vol = Common.CheckRange(vol, 0, 127);

            if (page.beforeExpression != vol)
            {
                if (!page.directModeTre)
                {
                    OutMidiControlChange(page, mml, enmControlChange.Expression, (byte)vol);
                }
                page.beforeExpression = vol;
            }
        }

        public override void SetLfoAtKeyOn(partPage page, MML mml)
        {
            for (int lfo = 0; lfo < 4; lfo++)
            {
                clsLfo pl = page.lfo[lfo];
                if (!pl.sw)
                    continue;
                if (pl.type == eLfoType.Wah) continue;

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

        protected override void ResetTieBend(partPage page, MML mml)
        {
            page.tieBend = 0;
            page.bendStartOctave = page.octaveNew;
            page.bendStartNote = page.noteCmd;
            page.bendStartShift = page.shift;
        }

        protected override void SetTieBend(partPage page, MML mml)
        {
            if (page.bendWaitCounter == -1)
            {
                int n = (page.octaveNew * 12 + Const.NOTE.IndexOf(page.noteCmd) + page.shift) - (page.bendStartOctave * 12 + Const.NOTE.IndexOf(page.bendStartNote) + page.bendStartShift);
                page.tieBend = 8192 / 24 * n;
            }
        }

        public override void SetKeyOn(partPage page, MML mml)
        {
            Note n = (Note)mml.args[0];
            byte noteNum;
            int arpNote = page.arpFreqMode ? 0 : page.arpDelta;
            if (page.bendWaitCounter == -1)
            {
                noteNum = (byte)GetNoteNum(page.octaveNew, n.cmd, n.shift + page.keyShift + arpNote);
                if (!page.beforeTie)
                    page.beforeBendNoteNum = noteNum;
                //if (!page.beforeTie) 
                //    page.beforeBendNoteNum = -1;
            }
            else
            {
                noteNum = (byte)GetNoteNum(page.bendStartOctave, page.bendStartNote, page.bendStartShift + page.keyShift + arpNote);
                page.beforeBendNoteNum = noteNum;
            }
            page.tblNoteOn[noteNum] = true;

            MIDINote mn = new MIDINote();
            mn.page = page;
            mn.mml = mml;
            mn.noteNumber = noteNum;
            mn.velocity = (byte)(n.velocity == -1 ? page.velocity : n.velocity);
            mn.length = page.tie ? -1 : page.waitKeyOnCounter;
            mn.beforeKeyon = null;
            if (page.beforeTie)
            {
                mn.beforeKeyon = true;
                if (page.beforeBendNoteNum != -1)
                {
                    noteNum = (byte)page.beforeBendNoteNum;
                    mn.noteNumber = noteNum;
                }
            }
            mn.Keyon = true;
            page.noteOns[noteNum] = mn;
        }

        public override void SetKeyOff(partPage page, MML mml)
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

        public override int GetToneDoublerShift(partPage page, int octave, char noteCmd, int shift)
        {
            return 0;
        }

        public override void SetToneDoubler(partPage page, MML mml)
        {
        }

        public override int GetFNum(partPage page, MML mml, int octave, char cmd, int shift, int pitchShift)
        {
            int n = (octave * 12 + Const.NOTE.IndexOf(cmd) + shift) - (page.bendStartOctave * 12 + Const.NOTE.IndexOf(page.bendStartNote) + page.bendStartShift);
            return 8192 / 24 * n + pitchShift;
        }

        private void ResetPitchBend(partPage page, MML mml)
        {
            //ピッチベンドとセンシビリティを送信
            OutMidiPitchBend(page, mml, 8192 / 128, 8192 % 128);

            OutMidiControlChange(page, mml, enmControlChange.RPN_MSB, 0);//MSB先
            OutMidiControlChange(page, mml, enmControlChange.RPN_LSB, 0);
            OutMidiControlChange(page, mml, enmControlChange.DataEntryMSB, 24);//センシビリティを24(2オクターブ分)に設定
        }



        private void OutMidiNoteOff(partPage page, MML mml, byte noteNumber, byte velocity)
        {
            parent.OutData(mml, page.port[0], 3, (byte)(0x80 + page.MIDIch), (byte)(noteNumber & 0x7f), (byte)(velocity & 0x7f));
        }

        private void OutMidiNoteOn(partPage page, MML mml, byte noteNumber, byte velocity)
        {
            parent.OutData(mml, page.port[0], 3, (byte)(0x90 + page.MIDIch), (byte)(noteNumber & 0x7f), (byte)(velocity & 0x7f));
        }

        private void OutMidiPolyKeyPress(partPage page, MML mml, byte noteNumber, byte press)
        {
            parent.OutData(mml, page.port[0], 3, (byte)(0xa0 + page.MIDIch), (byte)(noteNumber & 0x7f), (byte)(press & 0x7f));
        }

        private void OutMidiControlChange(partPage page, MML mml, enmControlChange ctrl, byte data)
        {
            parent.OutData(mml, page.port[0], 3, (byte)(0xb0 + page.MIDIch), (byte)ctrl, (byte)(data & 0x7f));
        }

        private void OutMidiProgramChange(partPage page, MML mml, byte programNumber)
        {
            parent.OutData(mml, page.port[0], 2, (byte)(0xc0 + page.MIDIch), programNumber);//, (byte)0);
        }

        private void OutMidiChannelPress(partPage page, MML mml, byte press)
        {
            parent.OutData(mml, page.port[0], 3, (byte)(0xd0 + page.MIDIch), press, (byte)0);
        }

        private void OutMidiPitchBend(partPage page, MML mml, byte msb, byte lsb)
        {
            parent.OutData(mml, page.port[0], 3, (byte)(0xe0 + page.MIDIch), (byte)(lsb & 0x7f), (byte)(msb & 0x7f));
        }

        private void OutMidi(partPage page, MML mml)
        {
            //parent.OutData(mml, pw.ppg[pw.cpgNum].port[0]);
        }

        public override void MultiChannelCommand(MML mml)
        {
            if (!use) return;
            //コマンドを跨ぐデータ向け処理
            foreach (partWork pw in lstPartWork)
            {
                foreach (partPage page in pw.pg)
                {
                    page.lfoBend = 0;
                    for (int lfo = 0; lfo < 4; lfo++)
                    {
                        if (!page.lfo[lfo].sw)
                        {
                            continue;
                        }
                        if (page.lfo[lfo].type != eLfoType.Vibrato)
                        {
                            continue;
                        }
                        page.lfoBend += page.lfo[lfo].value + page.lfo[lfo].param[6];
                    }

                    if (page.pitchBend != (page.bendWaitCounter == -1 ? page.tieBend : 0) + page.portaBend + page.lfoBend + page.detune)
                    {
                        page.pitchBend = (page.bendWaitCounter == -1 ? page.tieBend : 0) + page.portaBend + page.lfoBend + page.detune;
                        if (!page.directModeVib)
                        {
                            int pb = (page.pitchBend + 8192) & 0x3fff;
                            OutMidiPitchBend(page, mml, (byte)(pb / 128), (byte)(pb % 128));
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
                    foreach (MIDINote n in page.noteOns)
                    {
                        if (n == null) continue;
                        if (n.beforeKeyon != n.Keyon)
                        {
                            if (n.Keyon)
                                OutMidiNoteOn(n.page, null, n.noteNumber, n.velocity);//mmlはわたさない
                            else
                                OutMidiNoteOff(n.page, null, n.noteNumber, n.velocity);//mmlはわたさない
                            n.beforeKeyon = n.Keyon;
                        }
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
            RPN_LSB = 100,
            RPN_MSB = 101
        }
    }
}
