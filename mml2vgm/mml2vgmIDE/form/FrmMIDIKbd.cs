using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio;
using NAudio.Midi;

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

        public FrmMIDIKbd(FrmMain frm, int zoom, MDChipParams.MIDIKbd newParam)
        {
            parent = frm;
            this.zoom = zoom;
            this.setting = parent.setting;
            keyPress = new bool[kbdTbl.Length];
            cOct = 4;

            InitializeComponent();

            this.newParam = newParam;
            frameBuffer.Add(pbScreen, Properties.Resources.planeMIDIKB, null, zoom);
            DrawBuff.screenInitMixer(frameBuffer);
            update();
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

            StopMIDIInMonitoring();
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
        }

        public void screenDrawParams()
        {
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
                    NoteOn(noe.NoteNumber);
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
                        int n = Math.Min(Math.Max((i - 6) + cOct * 12, 0), 12 * 10);
                        NoteOn(n);
                    }
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
                    int n = Math.Min(Math.Max((i - 6) + cOct * 12, 0), 12 * 10);
                    NoteOff(n);
                }
                else
                {
                    cOct = Math.Min(Math.Max((i == 0) ? (cOct - 1) : (cOct + 1), 0), 9);
                }
            }
        }

        private void NoteOn(int n)
        {
            log.Write(string.Format("MIDIKbd:Note On{0}", n));
        }

        private void NoteOff(int n)
        {
            log.Write(string.Format("MIDIKbd:Note Off{0}", n));
        }
    }
}
