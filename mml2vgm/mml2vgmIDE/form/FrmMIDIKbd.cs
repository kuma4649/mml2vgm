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
        private int cOct = 4;
        private int cQuantize = 8;
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
        private NoteLog[] noteLogs = new NoteLog[100];
        private int noteLogPtr = 0;

        private Mml2vgm mv = null;
        private partWork pw;
        private ClsChip cChip = null;
        private Chip eChip = null;
        private RealTimeMML rtMML = null;

        public SoundManager.SoundManager SoundManager { get; internal set; }
        private object lockObject = new object();



        public FrmMIDIKbd(FrmMain frm, int zoom, MDChipParams.MIDIKbd newParam)
        {
            parent = frm;
            this.zoom = zoom;
            this.setting = parent.setting;
            keyPress = new bool[kbdTbl.Length];
            cOct = 4;
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
            for (int i = 0; i < 37; i++) newParam.note[i] = noteFlg[i + (cOct - 1) * 12];

            newParam.cOctave = cOct;
            newParam.kbOctave = cOct - 1;
            newParam.kcOctave = cOct;
            newParam.kaOctave = cOct + 1;
            newParam.kaaOctave = cOct + 2;
            newParam.cTempo = Audio.sm.CurrentTempo;
            newParam.cClockCnt = Audio.sm.CurrentClockCount;
            newParam.cNoteLength = Audio.sm.CurrentNoteLength < 1 ? 0 : (newParam.cClockCnt / Audio.sm.CurrentNoteLength);
            newParam.cQuantize = cQuantize;
            newParam.rec = recMode == 0 ? 0 : (((recMode - 1) / 17) + 1);
            recMode = recMode == 0 ? 0 : ((((++recMode) - 1) % 34) + 1);
            newParam.cChip = Audio.sm.CurrentChip;
            newParam.cCh = Audio.sm.CurrentCh;

            if (recAC != null)
            {
                while (recBuf.Count > 0)
                {
                    recAC.Document.Replace(recBuf.Dequeue());
                }
            }
        }

        private Queue<string> recBuf = new Queue<string>();

        public void screenDrawParams()
        {
            for (int n = 0; n < 37; n++)
            {
                DrawBuff.drawMIDILCD_Kbd(frameBuffer, 1
                    , 56, n, ref oldParam.note[n], newParam.note[n]);

            }

            DrawBuff.font4Int2(frameBuffer, 41 * 4 + 1, 0, 0, 0, ref oldParam.cOctave, newParam.cOctave);
            DrawBuff.font4Hex4Bit(frameBuffer, 0 + 1, 56, 0, ref oldParam.kbOctave, newParam.kbOctave);
            DrawBuff.font4Hex4Bit(frameBuffer, 56 + 1, 56, 0, ref oldParam.kcOctave, newParam.kcOctave);
            DrawBuff.font4Hex4Bit(frameBuffer, 112 + 1, 56, 0, ref oldParam.kaOctave, newParam.kaOctave);
            DrawBuff.font4Hex4Bit(frameBuffer, 168 + 1, 56, 0, ref oldParam.kaaOctave, newParam.kaaOctave);

            DrawBuff.font4Int3(frameBuffer, 33 * 4 + 1, 0, 0, 3, ref oldParam.cCh, newParam.cCh);
            DrawBuff.font4Int3(frameBuffer, 34 * 4 + 1, 8, 0, 3, ref oldParam.cTempo, newParam.cTempo);
            DrawBuff.font4Int3(frameBuffer, 40 * 4 + 1, 8, 0, 3, ref oldParam.cClockCnt, newParam.cClockCnt);
            DrawBuff.font4Int3(frameBuffer, 21 * 4 + 1, 12 * 8, 0, 3, ref oldParam.cNoteLength, newParam.cNoteLength);
            DrawBuff.font4Int3(frameBuffer, 29 * 4 + 1, 13 * 8, 0, 3, ref oldParam.cQuantize, newParam.cQuantize);

            DrawBuff.drawREC(frameBuffer, 0 + 1, 13 * 8, ref oldParam.rec, newParam.rec);

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
                        int n = Math.Min(Math.Max((i - 6) + (cOct - 1 + 1) * 12, 0), 127);
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
                    int n = Math.Min(Math.Max((i - 6) + (cOct - 1 + 1) * 12, 0), 127);
                    NoteOff(n);
                }
                else
                {
                    cOct = Math.Min(Math.Max((i == 0) ? (cOct - 1) : (cOct + 1), 1), 11);
                }
            }
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
            pw.lfo[0].sw = true;
            pw.lfo[0].direction = 1;
            pw.lfo[0].type = eLfoType.Vibrato;
            pw.lfo[0].param = new List<int>();
            pw.lfo[0].param.Add(100);//delay
            pw.lfo[0].param.Add(1);
            pw.lfo[0].param.Add(3);
            pw.lfo[0].param.Add(15);//depth
            pw.lfo[0].param.Add(0);//type 0:tri
            pw.lfo[0].param.Add(1);
            pw.lfo[0].param.Add(0);

            mv.desBuf = null;
            if (mv.desVGM.dat != null) mv.desVGM.dat.Clear();
            if (mv.desVGM.xdat != null) mv.desVGM.xdat.Clear();

            for(int i = 0; i < noteLogs.Length; i++)
            {
                noteLogs[i] = new NoteLog();
            }
            noteLogPtr = 0;

            rtMML = new RealTimeMML();
            rtMML.chip = cChip;
            rtMML.vgm = mv.desVGM;

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
                                noteLogs[noteLogPtr].note = (int)dat[badr].args[1];
                                noteLogs[noteLogPtr].counter = SeqCounter;
                                log.Write(string.Format("noteLog: note:{0} counter:{1}", noteLogs[noteLogPtr].note, noteLogs[noteLogPtr].counter));
                                if (recMode != 0)
                                {
                                    if (noteLogs[noteLogPtr].note >= 0)
                                    {
                                        NoteON(SeqCounter, noteLogs[noteLogPtr].note);
                                    }
                                    else
                                    {
                                        NoteOFF(SeqCounter, noteLogs[noteLogPtr].note);
                                    }
                                }
                                noteLogPtr++;
                                if (noteLogPtr >= noteLogs.Length) noteLogPtr = 0;
                            }
                            badr += 2;
                            break;
                    }
                    badr++;
                }
                dat.Clear();
            }
        }

        private int rec_oldNote = -1;
        private long rec_startCounter = 0;
        private string[] rec_noteTable = new string[] { "c", "c+", "d", "d+", "e", "f", "f+", "g", "g+", "a", "a+", "b" };
        private int rec_currentOctave = -1;

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

            if((len % (rtMML.clockCount / cQuantize)) < (rtMML.clockCount / cQuantize / 2))
            {
                len -= (len % (rtMML.clockCount / cQuantize));
            }
            else
            {
                len -= (len % (rtMML.clockCount / cQuantize));
                len += (rtMML.clockCount / cQuantize);
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

    }

    public class NoteLog
    {
        public int note;
        public long counter;
    }
}
