
using NAudio.Midi;
using System.Runtime.CompilerServices;
using static IronPython.Modules._ast;

namespace mml2vgmIDEx64
{
    internal class SimpleMIDIKbd
    {

        public bool previewMode = true;

        public SimpleMIDIKbd(FrmMain parent,Setting setting)
        {
            this.parent = parent;
            this.setting = setting;

            if(setting.midiKbd.simpleChangePreviewMode_Type!=-1)
                shortCut[setting.midiKbd.simpleChangePreviewMode_Type].Add(setting.midiKbd.simpleChangePreviewMode_Adr, ChangePreviewMode);
            if (setting.midiKbd.simpleUndo_Type != -1)
                shortCut[setting.midiKbd.simpleUndo_Type].Add(setting.midiKbd.simpleUndo_Adr, Undo);
            if (setting.midiKbd.simpleWriteSpace_Type != -1)
                shortCut[setting.midiKbd.simpleWriteSpace_Type].Add(setting.midiKbd.simpleWriteSpace_Adr, WriteSpace);
            if (setting.midiKbd.simpleWriteEnter_Type != -1)
                shortCut[setting.midiKbd.simpleWriteEnter_Type].Add(setting.midiKbd.simpleWriteEnter_Adr, WriteEnter);

            //shortCut[(int)enmTyp.ControlChange].Add(97, ChangePreviewMode);
            //shortCut[(int)enmTyp.ControlChange].Add(96, Undo);
            //shortCut[(int)enmTyp.Note].Add(48, ChangePreviewMode);
            //shortCut[(int)enmTyp.Note].Add(50, Undo);
        }

        public void StartMIDIInMonitoring()
        {
            if (setting.midiKbd.MidiInDeviceName == "")
                return;
            ReleaseMIDIIn();
            RegistMIDIIn();
        }

        public void StopMIDIInMonitoring()
        {
            if (midiin == null) return;
            ReleaseMIDIIn();
        }



        private Setting setting;
        private MidiIn midiin = null;
        private FrmMain parent = null;
        private const string notes = "c c+d d+e f f+g g+a a+b ";
        private int oldOct = -1;
        private enum enmTyp : int
        {
            ControlChange = 0,
            Note
        }

        private Dictionary<int, Action<int>>[] shortCut = [[], []];

        private void ReleaseMIDIIn()
        {
            if (midiin == null) return;

            try
            {
                midiin.Stop();
                midiin.Dispose();
                midiin.MessageReceived -= midiIn_MessageReceived;
                midiin.ErrorReceived -= midiIn_ErrorReceived;
                midiin = null;
            }
            catch
            {
                midiin = null;
            }
        }

        private void RegistMIDIIn()
        {
            if (!setting.midiKbd.UseMIDIKeyboard) return;
            if (midiin != null) return;

            for (int i = 0; i < MidiIn.NumberOfDevices; i++)
            {
                if (setting.midiKbd.MidiInDeviceName != MidiIn.DeviceInfo(i).ProductName)
                    continue;

                try
                {
                    midiin = new MidiIn(i);
                    midiin.MessageReceived += midiIn_MessageReceived;
                    midiin.ErrorReceived += midiIn_ErrorReceived;
                    midiin.Start();
                    break;
                }
                catch
                {
                    midiin = null;
                }
            }
        }

        private void midiIn_ErrorReceived(object sender, MidiInMessageEventArgs e)
        {
            log.ForcedWrite(String.Format("Error Time {0} Message 0x{1:X8} Event {2}",
                e.Timestamp, e.RawMessage, e.MidiEvent));
        }

        private void midiIn_MessageReceived(object sender, MidiInMessageEventArgs e)
        {
            if (!setting.midiKbd.UseMIDIKeyboard) return;

            NoteEvent ne;
            switch (e.MidiEvent.CommandCode)
            {
                case MidiCommandCode.NoteOn:
                    if (e.MidiEvent is NoteOnEvent)
                    {
                        NoteOnEvent noe = (NoteOnEvent)e.MidiEvent;
                        NoteOn(noe.NoteNumber);
                    }
                    else if (e.MidiEvent is NoteEvent)
                    {
                        ne = (NoteEvent)e.MidiEvent;
                        if (ne.Velocity == 0)
                        {
                            NoteOff(ne.NoteNumber);
                        }
                    }
                    break;
                case MidiCommandCode.NoteOff:
                    ne = (NoteEvent)e.MidiEvent;
                    NoteOff(ne.NoteNumber);
                    break;
                case MidiCommandCode.ControlChange:
                    // ---- 押したCCがショートカットに割り当てられているかチェック
                    int n = ((int)((ControlChangeEvent)e.MidiEvent).Controller);
                    log.Write(string.Format("CC:{0} {1}", n, ((ControlChangeEvent)e.MidiEvent).Controller));
                    if (shortCut[(int)enmTyp.ControlChange].ContainsKey(n))
                    {
                        shortCut[(int)enmTyp.ControlChange][n](
                            ((ControlChangeEvent)e.MidiEvent).ControllerValue
                            );
                        return;
                    }
                    break;
                case MidiCommandCode.TimingClock:
                    break;
                case MidiCommandCode.AutoSensing:
                    break;
                default:
                    log.Write(string.Format("MIDIevent:{0}", e.MidiEvent.CommandCode));
                    break;
            }
        }

        private void NoteOn(int n)
        {
            // ---- 押した音程がショートカットに割り当てられているかチェック
            if (shortCut[(int)enmTyp.Note].ContainsKey(n))
            {
                shortCut[(int)enmTyp.Note][n](0);
                return;
            }

            // ---- 押した音程をエディタに入力する

            string nt = "";
            // オクターブ変更チェック
            int oct = n / 12;
            if (oldOct == -1)
            {
                oldOct = oct;
            }
            else if (oldOct != oct)
            {
                while (oldOct < oct)
                {
                    nt += setting.midiKbd.simpleOctaveChange ? "<" : ">";
                    oldOct++;
                }
                while (oldOct > oct)
                {
                    nt += setting.midiKbd.simpleOctaveChange ? ">" : "<";
                    oldOct--;
                }
            }

            nt += notes.Substring((n % 12) * 2, 2).Trim();
            log.Write(string.Format("MIDIKbd:Note On {0}", nt));

            // プレビューモード(試し弾き)の場合は入力せずに終了
            if (previewMode) return;
            parent.WriteDocument(nt);
        }

        private void NoteOff(int n)
        {
            log.Write(string.Format("MIDIKbd:Note Off{0}", n));
        }

        private void ChangePreviewMode(int _)
        {
            previewMode = !previewMode;
            parent.SetSimpleMIDIKbdPreviewMode(previewMode);
        }

        private void Undo(int _)
        {
            if (previewMode) return;
            parent.UndoDocument();
        }

        private void WriteSpace(int _)
        {
            // プレビューモード(試し弾き)の場合は入力せずに終了
            if (previewMode) return;
            parent.WriteDocument(" ");
        }

        private void WriteEnter(int _)
        {
            // プレビューモード(試し弾き)の場合は入力せずに終了
            if (previewMode) return;
            parent.WriteEnter();
        }
    }
}