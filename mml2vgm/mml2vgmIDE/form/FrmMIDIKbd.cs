using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Core;
using NAudio;
using NAudio.Midi;
using Sgry.Azuki.WinForms;
using SoundManager;
using WeifenLuo.WinFormsUI.Docking;

namespace mml2vgmIDE
{
    public partial class FrmMIDIKbd : Form
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;
        public FrmMain parent = null;
        private int frameSizeW = 0;
        private int frameSizeH = 0;
        private int zoom = 1;
        private int chipn = -1;
        private Setting setting = null;
        private MidiIn midiin = null;
        //private int cQuantize = 8;
        private int recMode = 0;
        private AzukiControl recAC = null;

        private MDChipParams.MIDIKbd newParam = null;
        private MDChipParams.MIDIKbd oldParam = new MDChipParams.MIDIKbd();
        private FrameBuffer frameBuffer = new FrameBuffer();

        private Keys[] kbdTbl = new Keys[] {
            Keys.Q //  Q (Octave down)
            ,Keys.Oem4 //  [ (Octave up)
            ,Keys.A,Keys.Z,Keys.S,Keys.X //  A Z S X
            ,Keys.C,Keys.F,Keys.V,Keys.G,Keys.B //  C F V G B
            ,Keys.N,Keys.J,Keys.M,Keys.K,Keys.Oemcomma,Keys.L,Keys.OemPeriod //  N J M K , L .
            ,Keys.Oem2,Keys.Oem1 //  / :
        };
        private bool[] keyPress = null;
        private byte[] noteFlg = new byte[164];
        private int latestNoteNumberMONO = -1;
        private int[] shiftTbl = new int[] { 0, 1, 0, 1, 0, 0, 1, 0, 1, 0, 1, 0 };

        private Mml2vgm mv = null;
        private partWork pw;
        private ClsChip cChip = null;
        private Chip eChip = null;
        private RealTimeMML rtMML = null;

        public SoundManager.SoundManager SoundManager { get; internal set; }
        private object lockObject = new object();

        private int rec_oldNote = -1;
        private long rec_startCounter = 0;
        private string[] rec_noteTable = new string[] { "c", "c+", "d", "d+", "e", "f", "f+", "g", "g+", "a", "a+", "b" };
        private int rec_currentOctave = -1;
        private int mOct = -1;
        private int mNote = -1;



        public FrmMIDIKbd(FrmMain frm, int zoom, MDChipParams.MIDIKbd newParam)
        {
            parent = frm;
            this.zoom = zoom;
            this.setting = parent.setting;
            keyPress = new bool[kbdTbl.Length];
            if (setting.midiKbd.Octave == 0) setting.midiKbd.Octave = 4;
            SoundManager = Audio.sm;
            SoundManager.AddDataSeqFrqEvent(OnDataSeqFrq);
            SoundManager.CurrentChip = "YM2612";
            SoundManager.CurrentCh = 1;

            InitializeComponent();

            this.newParam = newParam;
            frameBuffer.Add(pbScreen, Properties.Resources.planeMIDIKB, null, zoom);
            DrawBuff.screenInitMixer(frameBuffer);
            update();
            Init();
        }

        protected override bool ShowWithoutActivation
        {
            get
            {
                return true;
            }
        }


        protected override void WndProc(ref Message m)
        {
            if (parent != null)
            {
                parent.windowsMessage(ref m);
            }

            try { base.WndProc(ref m); }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
        }

        private void FrmMIDIKbd_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                parent.setting.location.RMIDIKbd = new Rectangle(Location.X, Location.Y, 0, 0);
            }
            else
            {
                parent.setting.location.RMIDIKbd = new Rectangle(RestoreBounds.Location.X, RestoreBounds.Location.Y, 0, 0);
            }

            SoundManager.RemoveDataSeqFrqEvent(OnDataSeqFrq);
            StopMIDIInMonitoring();
            if ((SoundManager.Mode & SendMode.RealTime) == SendMode.RealTime)
            {
                if (SoundManager.Mode == SendMode.none)
                {
                    Audio.Stop(SendMode.Both);
                }
                else
                {
                    SoundManager.ResetMode(SendMode.RealTime);
                }
            }
            isClosed = true;

            if (frmMIDIKbd_prm != null) frmMIDIKbd_prm.Close();
        }

        private void FrmMIDIKbd_Load(object sender, EventArgs e)
        {
            this.Location = new Point(parent.setting.location.RMIDIKbd.X, parent.setting.location.RMIDIKbd.Y);
            this.Opacity = parent.setting.other.Opacity / 100.0;

            frameSizeW = this.Width - this.ClientSize.Width;
            frameSizeH = this.Height - this.ClientSize.Height;

            changeZoom();

            StartMIDIInMonitoring();
        }

        private void FrmMIDIKbd_Resize(object sender, EventArgs e)
        {

        }



        public void update()
        {
            frameBuffer.Refresh(null);
        }

        public void changeZoom()
        {
            this.MaximumSize = new Size(frameSizeW + Properties.Resources.planeMIDIKB.Width * zoom, frameSizeH + Properties.Resources.planeMIDIKB.Height * zoom);
            this.MinimumSize = new Size(frameSizeW + Properties.Resources.planeMIDIKB.Width * zoom, frameSizeH + Properties.Resources.planeMIDIKB.Height * zoom);
            this.Size = new Size(frameSizeW + Properties.Resources.planeMIDIKB.Width * zoom, frameSizeH + Properties.Resources.planeMIDIKB.Height * zoom);
            FrmMIDIKbd_Resize(null, null);

        }

        public void screenChangeParams()
        {
            for (int i = 0; i < newParam.note.Length; i++)
            {
                int n = i + (setting.midiKbd.Octave - 2) * 12 - 0;
                n = Math.Min(Math.Max(n, 0), noteFlg.Length - 1);
                newParam.note[i] = noteFlg[n];
            }

            newParam.cOctave = setting.midiKbd.Octave;
            newParam.kbOctave = setting.midiKbd.Octave - 1;
            newParam.kcOctave = setting.midiKbd.Octave;
            newParam.kaOctave = setting.midiKbd.Octave + 1;
            newParam.kaaOctave = setting.midiKbd.Octave + 2;
            newParam.cTempo = Audio.sm.CurrentTempo;
            newParam.cClockCnt = Audio.sm.CurrentClockCount;
            setting.midiKbd.Tempo = Audio.sm.CurrentTempo;
            setting.midiKbd.Clockcounter = Audio.sm.CurrentClockCount;
            //newParam.cNoteLength = Audio.sm.CurrentNoteLength < 1 ? 0 : (newParam.cClockCnt / Audio.sm.CurrentNoteLength);
            newParam.cNoteLength = setting.midiKbd.CurrentLength;
            newParam.cQuantize = setting.midiKbd.Quantize;
            //newParam.cQuantize = cQuantize;
            newParam.rec = recMode == 0 ? 0 : (((recMode - 1) / 17) + 1);
            recMode = recMode == 0 ? 0 : ((((++recMode) - 1) % 34) + 1);
            newParam.cChip = Audio.sm.CurrentChip;
            newParam.cCh = Audio.sm.CurrentCh;
            setting.midiKbd.Channel = Audio.sm.CurrentCh;

            if (recAC != null)
            {
                while (recBuf.Count > 0)
                {
                    recAC.Document.Replace(recBuf.Dequeue());
                }
            }

            if (pw != null)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (pw.lfo[i] == null) continue;

                    newParam.lfo[i].sw = pw.lfo[i].sw;
                    newParam.lfo[i].type = pw.lfo[i].type;
                    if (newParam.lfo[i].param == null) continue;
                    if (newParam.lfo[i].param.Count > 0) newParam.lfo[i].param[0] = pw.lfo[i].param[0];
                    if (newParam.lfo[i].param.Count > 1) newParam.lfo[i].param[1] = pw.lfo[i].param[1];
                    if (newParam.lfo[i].param.Count > 2) newParam.lfo[i].param[2] = pw.lfo[i].param[2];
                    if (newParam.lfo[i].param.Count > 3) newParam.lfo[i].param[3] = pw.lfo[i].param[3];
                    if (newParam.lfo[i].param.Count > 4) newParam.lfo[i].param[4] = pw.lfo[i].param[4];
                    if (newParam.lfo[i].param.Count > 5) newParam.lfo[i].param[5] = pw.lfo[i].param[5];
                    if (newParam.lfo[i].param.Count > 6) newParam.lfo[i].param[6] = pw.lfo[i].param[6];
                }
            }
        }

        private Queue<string> recBuf = new Queue<string>();

        public void screenDrawParams()
        {
            for (int n = 9; n < 49; n++)
            {
                DrawBuff.drawMIDILCD_Kbd(frameBuffer, -40 + 1
                    , 57, n, ref oldParam.note[n], newParam.note[n]);
            }

            DrawBuff.font4Hex4Bit(frameBuffer, 04 * 4 + 1, 07 * 8 + 1, 0, ref oldParam.kbOctave, newParam.kbOctave);
            DrawBuff.font4Hex4Bit(frameBuffer, 18 * 4 + 1, 07 * 8 + 1, 0, ref oldParam.kcOctave, newParam.kcOctave);
            DrawBuff.font4Hex4Bit(frameBuffer, 32 * 4 + 1, 07 * 8 + 1, 0, ref oldParam.kaOctave, newParam.kaOctave);
            DrawBuff.font4Hex4Bit(frameBuffer, 46 * 4 + 1, 07 * 8 + 1, 0, ref oldParam.kaaOctave, newParam.kaaOctave);

            DrawBuff.font4Int3(frameBuffer, 07 * 4 + 1, 01 * 8 + 1, 0, 3, ref oldParam.cCh, newParam.cCh);
            DrawBuff.font4Int2(frameBuffer, 15 * 4 + 1, 01 * 8 + 1, 0, 0, ref oldParam.cOctave, newParam.cOctave);
            DrawBuff.font4Int3(frameBuffer, 24 * 4 + 1, 01 * 8 + 1, 0, 3, ref oldParam.cTempo, newParam.cTempo);
            DrawBuff.font4Int3(frameBuffer, 32 * 4 + 1, 01 * 8 + 1, 0, 3, ref oldParam.cClockCnt, newParam.cClockCnt);
            DrawBuff.font4Int3(frameBuffer, 38 * 4 + 1, 01 * 8 + 1, 0, 3, ref oldParam.cNoteLength, newParam.cNoteLength);
            DrawBuff.font4Int3(frameBuffer, 44 * 4 + 1, 01 * 8 + 1, 0, 3, ref oldParam.cQuantize, newParam.cQuantize);

            DrawBuff.drawREC(frameBuffer, 04 * 4 + 1, 12 * 8, ref oldParam.rec, newParam.rec);

            for (int i = 0; i < 4; i++)
            {
                if (oldParam.lfo[i].sw != newParam.lfo[i].sw)
                {
                    DrawBuff.drawFont4(frameBuffer, 4 * 4 + 1, (3 + i) * 8 + 1, 0, newParam.lfo[i].sw ? "ON " : "OFF");
                    oldParam.lfo[i].sw = newParam.lfo[i].sw;
                }
                if (oldParam.lfo[i].type != newParam.lfo[i].type)
                {
                    DrawBuff.drawFont4(frameBuffer, 8 * 4 + 1, (3 + i) * 8 + 1, 0
                        , newParam.lfo[i].type == eLfoType.Tremolo ? "T" : (
                        newParam.lfo[i].type == eLfoType.Hardware ? "H" : (
                        newParam.lfo[i].type == eLfoType.Vibrato ? "V" : "?"
                    )));
                    oldParam.lfo[i].type = newParam.lfo[i].type;
                }
                if (newParam.lfo[i].param == null) continue;
                int on;

                on = oldParam.lfo[i].param[0];
                DrawBuff.font4Int3(frameBuffer, 11 * 4 + 1, (3 + i) * 8 + 1, 0, 3, ref on, newParam.lfo[i].param[0]);
                oldParam.lfo[i].param[0] = on;

                on = oldParam.lfo[i].param[1];
                DrawBuff.font4Int3(frameBuffer, 16 * 4 + 1, (3 + i) * 8 + 1, 0, 3, ref on, newParam.lfo[i].param[1]);
                oldParam.lfo[i].param[1] = on;

                on = oldParam.lfo[i].param[2];
                DrawBuff.font4Int5S(frameBuffer, 20 * 4 + 1, (3 + i) * 8 + 1, 0, ref on, newParam.lfo[i].param[2]);
                oldParam.lfo[i].param[2] = on;

                on = oldParam.lfo[i].param[3];
                DrawBuff.font4Int5S(frameBuffer, 27 * 4 + 1, (3 + i) * 8 + 1, 0, ref on, newParam.lfo[i].param[3]);
                oldParam.lfo[i].param[3] = on;

                if (oldParam.lfo[i].param[4] != newParam.lfo[i].param[4])
                {
                    /*
                    0:三角波
                    1:のこぎり波
                    2:矩形波
                    3:ワンショット
                    4:ランダム
                    */
                    DrawBuff.drawFont4(frameBuffer, 34 * 4 + 1, (3 + i) * 8 + 1, 0
                        , newParam.lfo[i].param[4] == 0 ? "TRI" : (
                        newParam.lfo[i].param[4] == 1 ? "SAW" : (
                        newParam.lfo[i].param[4] == 2 ? "SQR" : (
                        newParam.lfo[i].param[4] == 3 ? "ONE" : "RND"
                    ))));
                    oldParam.lfo[i].param[4] = newParam.lfo[i].param[4];
                }

                on = oldParam.lfo[i].param[5];
                DrawBuff.font4Int1(frameBuffer, 39 * 4 + 1, (3 + i) * 8 + 1, 0, ref on, newParam.lfo[i].param[5]);
                oldParam.lfo[i].param[5] = on;

                on = oldParam.lfo[i].param[6];
                DrawBuff.font4Int5S(frameBuffer, 41 * 4 + 1, (3 + i) * 8 + 1, 0, ref on, newParam.lfo[i].param[6]);
                oldParam.lfo[i].param[6] = on;
            }
        }

        public void screenInit()
        {
        }


        private void StartMIDIInMonitoring()
        {

            if (setting.midiKbd.MidiInDeviceName == "")
            {
                return;
            }

            if (midiin != null)
            {
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

            if (midiin == null)
            {
                for (int i = 0; i < MidiIn.NumberOfDevices; i++)
                {
                    if (setting.midiKbd.MidiInDeviceName == MidiIn.DeviceInfo(i).ProductName)
                    {
                        try
                        {
                            midiin = new MidiIn(i);
                            midiin.MessageReceived += midiIn_MessageReceived;
                            midiin.ErrorReceived += midiIn_ErrorReceived;
                            midiin.Start();
                        }
                        catch
                        {
                            midiin = null;
                        }
                    }
                }
            }

        }

        private void StopMIDIInMonitoring()
        {
            if (midiin != null)
            {
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
        }

        private void midiIn_ErrorReceived(object sender, MidiInMessageEventArgs e)
        {
            log.ForcedWrite(String.Format("Error Time {0} Message 0x{1:X8} Event {2}",
                e.Timestamp, e.RawMessage, e.MidiEvent));
        }

        private void midiIn_MessageReceived(object sender, MidiInMessageEventArgs e)
        {
            if (!setting.midiKbd.UseMIDIKeyboard) return;

            switch (e.MidiEvent.CommandCode)
            {
                case MidiCommandCode.NoteOn:
                    NoteOnEvent noe = (NoteOnEvent)e.MidiEvent;
                    NoteOn(noe.NoteNumber, 127);// noe.Velocity);
                    break;
                case MidiCommandCode.NoteOff:
                    NoteEvent ne = (NoteEvent)e.MidiEvent;
                    NoteOff(ne.NoteNumber);
                    break;
                case MidiCommandCode.ControlChange:
                    break;
            }
        }

        private void FrmMIDIKbd_KeyDown(object sender, KeyEventArgs e)
        {
            for (int i = 0; i < kbdTbl.Length; i++)
            {
                if (kbdTbl[i] == e.KeyCode && !keyPress[i])
                {
                    keyPress[i] = true;
                    if (i >= 2)
                    {
                        int n = Math.Min(Math.Max((i - 6) + (setting.midiKbd.Octave - 1 + 1) * 12, 0), 127);
                        NoteOn(n, 127);
                    }
                }
            }

            if (e.KeyCode == Keys.Space)
            {
                if (recMode == 0)
                {
                    StartRecording();
                }
                else
                {
                    recMode = 0;
                    recAC = null;
                }
            }
        }


        private void FrmMIDIKbd_KeyUp(object sender, KeyEventArgs e)
        {
            for (int i = 0; i < kbdTbl.Length; i++)
            {
                if (kbdTbl[i] != e.KeyCode) continue;

                keyPress[i] = false;
                if (i >= 2)
                {
                    int n = Math.Min(Math.Max((i - 6) + (setting.midiKbd.Octave - 1 + 1) * 12, 0), 127);
                    NoteOff(n);
                }
                else
                {
                    setting.midiKbd.Octave = Math.Min(Math.Max((i == 0) ? (setting.midiKbd.Octave - 1) : (setting.midiKbd.Octave + 1), 1), 11);
                }
            }
        }

        private void PbScreen_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            int px = e.Location.X / zoom - 1;
            int py = e.Location.Y / zoom - 1;
            px /= 4;
            py /= 8;

            if (py < 7 || py >= 12)
            {
                return;
            }

            //鍵盤チェック
            int oct;
            int note;
            PbScreen_MouseClick_KeybdArea(px, py, e, out oct, out note);

            int n = Math.Min(Math.Max(note + (oct + setting.midiKbd.Octave - 1) * 12, 0), 127);
            NoteOn(n, 127);
            mOct = oct;
            mNote = note;

        }

        private void PbScreen_MouseUp(object sender, MouseEventArgs e)
        {
            if (mOct == -1 && mNote == -1) return;

            int n = Math.Min(Math.Max(mNote + (mOct + setting.midiKbd.Octave - 1) * 12, 0), 127);
            NoteOff(n);
            mOct = -1;
            mNote = -1;
        }

        private void NoteOn(int n,int velocity)
        {
            noteFlg[n & 0x7f] = (byte)(velocity & 0x7f);
            log.Write(string.Format("MIDIKbd:Note On{0}", n));

            if (setting.midiKbd.IsMONO) NoteOnMONO(n, velocity);
            else NoteOnPOLY(n, velocity);
        }

        private void NoteOnPOLY(int n, int velocity)
        {
            throw new NotImplementedException();
        }

        private void NoteOnMONO(int n, int velocity)
        {
            if (cChip == null) return;
            eChip = Audio.GetChip(EnmChip.YM2612);
            if (eChip == null) return;
            if (n < 0 || n > 127) return;

            NoteOffMONO(latestNoteNumberMONO);
            MML mml = MakeMML_Octave(n);
            cChip.CmdOctave(pw, mml);
            mml = MakeMML_NoteOn(n);
            lock (lockObject)
            {
                cChip.CmdNote(pw, mml);
            }

            latestNoteNumberMONO = n;
        }

        private void NoteOff(int n)
        {
            noteFlg[n & 0x7f] = 0;
            log.Write(string.Format("MIDIKbd:Note Off{0}", n));

            if (setting.midiKbd.IsMONO) NoteOffMONO(n);
            else NoteOffPOLY(n);
        }

        private void NoteOffPOLY(int n)
        {
            throw new NotImplementedException();
        }

        private void NoteOffMONO(int n)
        {
            if (cChip == null) return;
            eChip = Audio.GetChip(EnmChip.YM2612);
            if (eChip == null) return;
            if (n < 0 || n > 127) return;

            if (latestNoteNumberMONO != n) return;

            MML mml = MakeMML_NoteOff(n);
            lock (lockObject)
            {
                cChip.SetKeyOff(pw, mml);
            }

        }

        private MML MakeMML_Octave(int n)
        {
            MML mml = new MML();
            mml.type = enmMMLType.Octave;
            mml.line = null;
            mml.column = -1;
            mml.args = new List<object>();
            mml.args.Add(n / 12);

            return mml;
        }

        private MML MakeMML_NoteOn(int n)
        {
            MML mml = new MML();
            mml.type = enmMMLType.Note;
            mml.line = null;
            mml.column = -1;
            mml.args = new List<object>();
            Note note = new Note();
            mml.args.Add(note);
            note.cmd = "ccddeffggaab"[n % 12];
            note.shift = shiftTbl[n % 12];
            note.length = 1;
            mml.args.Add(n);

            return mml;
        }

        private MML MakeMML_NoteOff(int n)
        {
            MML mml = new MML();
            mml.type = enmMMLType.Note;
            mml.line = null;
            mml.column = -1;
            mml.args = new List<object>();
            Note note = new Note();
            mml.args.Add(note);
            note.cmd = "ccddeffggaab"[n % 12];
            note.shift = shiftTbl[n % 12];
            note.length = 1;
            mml.args.Add(-n);

            return mml;
        }

        private void Init()
        {
            string txt = Properties.Resources.tmpMIDIKbd;
            txt = string.Format(
                txt
                , newParam.cClockCnt < 1 ? 192 : newParam.cClockCnt
                , newParam.cTempo < 1 ? 177 : newParam.cTempo
                );
            string[] text = txt.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            string stPath = System.Windows.Forms.Application.StartupPath;
            Action<string> dmy = dmyDisp;
            string wrkPath = "";

            mv = new Mml2vgm(text, "", stPath, dmy, wrkPath);
            mv.Start();
            cChip = mv.desVGM.ym2612[0];
            pw = cChip.lstPartWork[0];
            cChip.use = true;
            mv.desVGM.isRealTimeMode = true;
            for(int i = 0; i < 4; i++)
            {
                pw.lfo[i].param = new List<int>();
                pw.lfo[i].param.Add(0);
                pw.lfo[i].param.Add(0);
                pw.lfo[i].param.Add(0);
                pw.lfo[i].param.Add(0);
                pw.lfo[i].param.Add(0);
                pw.lfo[i].param.Add(0);
                pw.lfo[i].param.Add(0);
                newParam.lfo[i].param = new List<int>();
                newParam.lfo[i].param.Add(0);
                newParam.lfo[i].param.Add(0);
                newParam.lfo[i].param.Add(0);
                newParam.lfo[i].param.Add(0);
                newParam.lfo[i].param.Add(0);
                newParam.lfo[i].param.Add(0);
                newParam.lfo[i].param.Add(0);
                oldParam.lfo[i].param = new List<int>();
                oldParam.lfo[i].param.Add(0);
                oldParam.lfo[i].param.Add(0);
                oldParam.lfo[i].param.Add(0);
                oldParam.lfo[i].param.Add(0);
                oldParam.lfo[i].param.Add(0);
                oldParam.lfo[i].param.Add(0);
                oldParam.lfo[i].param.Add(0);
            }
            pw.lfo[0].sw = true;
            pw.lfo[0].direction = 1;
            pw.lfo[0].type = eLfoType.Vibrato;
            pw.lfo[0].param[0] = 100;//delay
            pw.lfo[0].param[1] = 1;
            pw.lfo[0].param[2] = 3;
            pw.lfo[0].param[3] = 15;//depth
            pw.lfo[0].param[4] = 0;//type 0:tri
            pw.lfo[0].param[5] = 1;
            pw.lfo[0].param[6] = 0;

            mv.desBuf = null;
            if (mv.desVGM.dat != null) mv.desVGM.dat.Clear();
            if (mv.desVGM.xdat != null) mv.desVGM.xdat.Clear();

            rtMML = new RealTimeMML();
            rtMML.chip = cChip;
            rtMML.vgm = mv.desVGM;

            this.TopMost = true;
            this.TopMost = setting.midiKbd.AlwaysTop;

            SoundManager.SetMode(SendMode.RealTime);
        }

        private void dmyDisp(string dmy)
        {
            log.Write(dmy);
        }


        private void OnDataSeqFrq(long SeqCounter)
        {
            //throw new NotImplementedException();
            if (mv == null) return;
            if (mv.desVGM == null) return;
            if (mv.desVGM.dat == null) return;

            if ((Audio.sm.Mode & SendMode.MML) != SendMode.MML)
            {
                if(rtMML!=null)
                rtMML.OneFrameSeq();
            }

            if (mv.desVGM.dat.Count == 0) return;
            eChip = Audio.GetChip(EnmChip.YM2612);
            if (eChip == null) return;


            Enq enq = SoundManager.GetDriverDataEnqueue();
            List<outDatum> dat = mv.desVGM.dat;

            lock (lockObject)
            {
                int badr = 0;
                while (badr < dat.Count)
                {
                    outDatum od = dat[badr];
                    if (od == null)
                    {
                        badr++;
                        continue;
                    }

                    byte val = od.val;
                    switch (val)
                    {
                        case 0x52:
                            byte adr = dat[badr + 1].val;
                            byte prm = dat[badr + 2].val;
                            enq(dat[badr], SeqCounter, eChip, EnmDataType.Normal, adr, prm, null);
                            //enq(dat[badr], 0, eChip, EnmDataType.Normal, adr, prm, null);
                            if (adr == 0x28 && dat[badr].type == enmMMLType.Note && dat[badr].args != null && dat[badr].args.Count > 1)
                            {
                                log.Write(string.Format("noteLog: note:{0} counter:{1}", (int)dat[badr].args[1], SeqCounter));
                                if (recMode != 0)
                                {
                                    if ((int)dat[badr].args[1] >= 0)
                                    {
                                        NoteON(SeqCounter, (int)dat[badr].args[1]);
                                    }
                                    else
                                    {
                                        NoteOFF(SeqCounter, (int)dat[badr].args[1]);
                                    }
                                }
                            }
                            badr += 2;
                            break;
                    }
                    badr++;
                }
                dat.Clear();
            }
        }

        private void StartRecording()
        {
            DockContent dc = null;
            Document d = null;
            if (parent.dpMain.ActiveDocument is DockContent)
            {
                dc = (DockContent)parent.dpMain.ActiveDocument;
                if (dc.Tag is Document)
                {
                    d = (Document)dc.Tag;
                }
            }
            if (d == null) return;


            recAC = d.editor.azukiControl;
            recMode = 1;
            rec_oldNote = -1;
            rec_startCounter = 0;
            rec_currentOctave = -1;
        }

        private void NoteON(long sqCnt, int note)
        {
            if (rec_oldNote == 255)//休符
            {
                long length = (long)((sqCnt - rec_startCounter) / rtMML.samplesPerClock);
                if (length != 0)
                    rec_dispNote(255, length);
            }
            else if (rec_oldNote > -1)
            {

            }

            rec_startCounter = sqCnt;
            rec_oldNote = note;
        }

        private void NoteOFF(long sqCnt, int note)
        {
            if (rec_oldNote != 255 && rec_oldNote > -1 && rec_oldNote == -note)
            {
                note = -note;
                rec_dispOctave(note);

                long length = (long)((sqCnt - rec_startCounter) / rtMML.samplesPerClock);
                if (length != 0)
                    rec_dispNote(note, length);
            }

            if (rec_oldNote != 255) rec_startCounter = sqCnt;
            rec_oldNote = 255;
        }

        private void rec_dispOctave(int note)
        {
            int octave = note / 12;
            if (rec_currentOctave < 0) recBuf.Enqueue(string.Format("o{0}", octave));
            else if (rec_currentOctave != octave)
            {
                int d = rec_currentOctave - octave;
                if (d > 0)
                    for (int i = 0; i < d; i++) recBuf.Enqueue("<");
                else
                    for (int i = 0; i < -d; i++) recBuf.Enqueue(">");
            }
            rec_currentOctave = octave;
        }

        private void rec_dispNote(int note,long length)
        {
            long c = rtMML.clockCount;
            long len = length;
            long n = 1;

            if (setting.midiKbd.Quantize != 0)
            {
                if ((len % (rtMML.clockCount / setting.midiKbd.Quantize)) < (rtMML.clockCount / setting.midiKbd.Quantize / 2))
                {
                    len -= (len % (rtMML.clockCount / setting.midiKbd.Quantize));
                }
                else
                {
                    len -= (len % (rtMML.clockCount / setting.midiKbd.Quantize));
                    len += (rtMML.clockCount / setting.midiKbd.Quantize);
                }
            }

            while (n <= 64 && len > 0)
            {
                while (len >= c)
                {
                    recBuf.Enqueue(string.Format("{0}{1}{2}"
                        , note == 255 ? "r" : rec_noteTable[note % 12]
                        , (Audio.sm.CurrentNoteLength != rtMML.clockCount / n) ? n.ToString() : ""
                        , note == 255 ? "" : ((len - c) > 0 ? "&" : "")
                        ));
                    len -= c;
                }
                c /= 2;
                n *= 2;
            }

            if (len > 0)
            {
                recBuf.Enqueue(string.Format("{0}#{1}"
                    , note == 255 ? "r" : rec_noteTable[note % 12]
                    , len
                    ));
            }
        }

        private void PbScreen_MouseClick(object sender, MouseEventArgs e)
        {
            int px = e.Location.X / zoom - 1;
            int py = e.Location.Y / zoom - 1;
            px /= 4;
            py /= 8;

            if (py < 1)
            {
                //チップ表示エリア
                PbScreen_MouseClick_ChipArea(px, py, e);
            }
            else if (py < 2)
            {
                //Ch Oct など
                PbScreen_MouseClick_ParamArea(px, py, e);
            }
            else if (py < 7)
            {
                //LFOエリア
                PbScreen_MouseClick_LFOArea(px, py, e);
            }
            else if (py < 12)
            {
                return;
                //鍵盤エリアClickイベントでは何もしない
                //PbScreen_MouseClick_KeybdArea(px, py, e);
            }
            else
            {
                //mono poly エリア
                PbScreen_MouseClick_MonopolyArea(px, py, e);
            }
        }

        private void PbScreen_MouseClick_ChipArea(int px, int py, MouseEventArgs e)
        {
            if (px < 10 || e.Button != MouseButtons.Left) return;

            //chip名表示欄を左クリックしている場合
        }


        public enum KbdParam
        {
            Unknown,
            CurrentChannel,
            Octave,
            Tempo,
            ClockCounter,
            NoteLength,
            Quantize,
            Lfo,
            LfoUse,
            LfoType,
            LfoDelay,
            LfoSpeed,
            LfoDelta,
            LfoDepth,
            LfoWaveType,
            LfoSw,
            LfoTrans
        }
        private FrmMIDIKbd_prm frmMIDIKbd_prm = null;

        private void PbScreen_MouseClick_ParamArea(int px, int py, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            KbdParam target = KbdParam.Unknown;

            if (px >= 7 && px <= 9) target = KbdParam.CurrentChannel;
            else if (px >= 15 && px <= 16) target = KbdParam.Octave;
            else if (px >= 24 && px <= 26) target = KbdParam.Tempo;
            else if (px >= 32 && px <= 34) target = KbdParam.ClockCounter;
            else if (px >= 38 && px <= 40) target = KbdParam.NoteLength;
            else if (px >= 44 && px <= 46) target = KbdParam.Quantize;

            if (frmMIDIKbd_prm == null || frmMIDIKbd_prm.IsDisposed)
            {
                frmMIDIKbd_prm = new FrmMIDIKbd_prm(setting);
                frmMIDIKbd_prm.Target = target;
                frmMIDIKbd_prm.TargetTab = 0;
                frmMIDIKbd_prm.Show();
            }
        }

        private void PbScreen_MouseClick_LFOArea(int px, int py, MouseEventArgs e)
        {
            if (py == 2 || e.Button != MouseButtons.Left) return;

            KbdParam target = KbdParam.Unknown;
            int n = py - 3;
            if (px >= 0 && px <= 2) target = KbdParam.Lfo;
            else if (px >= 4 && px <= 6) target = KbdParam.LfoUse;
            else if (px == 8) target = KbdParam.LfoType;
            else if (px >= 11 && px <= 13) target = KbdParam.LfoDelay;
            else if (px >= 16 && px <= 18) target = KbdParam.LfoSpeed;
            else if (px >= 20 && px <= 25) target = KbdParam.LfoDelta;
            else if (px >= 27 && px <= 32) target = KbdParam.LfoDepth;
            else if (px >= 34 && px <= 36) target = KbdParam.LfoWaveType;
            else if (px == 39) target = KbdParam.LfoSw;
            else if (px >= 41 && px <= 46) target = KbdParam.LfoTrans;

            if (frmMIDIKbd_prm == null || frmMIDIKbd_prm.IsDisposed)
            {
                frmMIDIKbd_prm = new FrmMIDIKbd_prm(setting);
                frmMIDIKbd_prm.Target = target;
                frmMIDIKbd_prm.TargetTab = n + 1;
                frmMIDIKbd_prm.Show();
            }
        }

        private void PbScreen_MouseClick_KeybdArea(int px, int py, MouseEventArgs e,out int oct,out int note)
        {
            px = e.Location.X / zoom - 1;
            py = e.Location.Y / zoom - 1 - 7 * 8;

            //オクターブ数
            oct = (px - 16) < 0 ? -1 : ((px - 16) / (7 * 8));
            //1オクターブ辺りのx座標
            int ox = (px - 16) % (7 * 8);
            ox = ox >= 0 ? ox : (7 * 8 + ox);

            note = -1;
            if (py < 17)
            {
                //黒鍵盤押下チェック
                if (ox >= 4 && ox <= 9) note = 1;//c#
                else if (ox >= 12 && ox <= 17) note = 3;//d#
                else if (ox >= 28 && ox <= 33) note = 6;//f#
                else if (ox >= 36 && ox <= 41) note= 8;//g#
                else if (ox >= 44 && ox <= 49) note = 10;//a#
            }
            if (note == -1)
            {
                //白鍵盤押下チェック
                note = ox / 8;
                if (note == 6) note = 11;//b
                else if (note == 5) note = 9;//a
                else if (note == 4) note = 7;//g
                else if (note == 3) note = 5;//f
                else if (note == 2) note = 4;//e
                else if (note == 1) note = 2;//d
            }

            //n:鍵盤 on:オクターブ
        }

        private void PbScreen_MouseClick_MonopolyArea(int px, int py, MouseEventArgs e)
        {
            //throw new NotImplementedException();
        }

    }

}
