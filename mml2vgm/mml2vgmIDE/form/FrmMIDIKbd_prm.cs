using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mml2vgmIDE
{
    public partial class FrmMIDIKbd_prm : Form
    {
        private Setting setting = null;

        public FrmMIDIKbd.KbdParam Target { get; internal set; }
        public int TargetTab { get; internal set; }

        public FrmMIDIKbd_prm(Setting setting)
        {
            InitializeComponent();
            this.setting = setting;

            Common.SetBFColor(this, setting);
            this.Opacity = setting.other.Opacity / 100.0;
        }

        private void FrmMIDIKbd_prm_Load(object sender, EventArgs e)
        {
            Setting.MidiKbd mk = setting.midiKbd;
            txtCH.Text = mk.Channel.ToString();
            txtOCT.Text = mk.Octave.ToString();
            lblTEMPO.Text = mk.Tempo.ToString();
            lblCLK.Text = mk.Clockcounter.ToString();
            txtL.Text = mk.CurrentLength.ToString();
            rbQ_OFF.Checked = !mk.UseQuantize;
            rbQ_ON.Checked = mk.UseQuantize;
            txtQ.Enabled = mk.UseQuantize;
            txtQ.Text = mk.Quantize.ToString();

            if (mk.LfoParams[0] == null) mk.LfoParams[0] = new Setting.LfoParam();
            Setting.LfoParam lp = mk.LfoParams[0];
            rbP_USE_OFF.Checked = !lp.Use;
            rbP_USE_ON.Checked = lp.Use;
            rbP_Type_Vibrato.Checked = lp.Type == 0 || lp.Type == 1;
            rbP_Type_Tremolo.Checked = lp.Type == 2;
            rbP_Type_Hardware.Checked = lp.Type == 3;
            txtP_Delay.Text = lp.Delay.ToString();
            txtP_Speed.Text = lp.Speed.ToString();
            txtP_Delta.Text = lp.Delta.ToString();
            txtP_Depth.Text = lp.Depth.ToString();
            rbP_WType_Triangle.Checked = lp.WaveType == 0 || lp.WaveType == 1;
            rbP_WType_Saw.Checked = lp.WaveType == 2;
            rbP_WType_Square.Checked = lp.WaveType == 3;
            rbP_WType_OneShot.Checked = lp.WaveType == 4;
            rbP_WType_Random.Checked = lp.WaveType == 5;
            rbP_Switch_OFF.Checked = !lp.Switch;
            rbP_Switch_ON.Checked = lp.Switch;
            txtP_Trans.Text = lp.Trans.ToString();

            if (mk.LfoParams[1] == null) mk.LfoParams[1] = new Setting.LfoParam();
            lp = mk.LfoParams[1];
            rbQ_USE_OFF.Checked = !lp.Use;
            rbQ_USE_ON.Checked = lp.Use;
            rbQ_Type_Vibrato.Checked = lp.Type == 0 || lp.Type == 1;
            rbQ_Type_Tremolo.Checked = lp.Type == 2;
            rbQ_Type_Hardware.Checked = lp.Type == 3;
            txtQ_Delay.Text = lp.Delay.ToString();
            txtQ_Speed.Text = lp.Speed.ToString();
            txtQ_Delta.Text = lp.Delta.ToString();
            txtQ_Depth.Text = lp.Depth.ToString();
            rbQ_WType_Triangle.Checked = lp.WaveType == 0 || lp.WaveType == 1;
            rbQ_WType_Saw.Checked = lp.WaveType == 2;
            rbQ_WType_Square.Checked = lp.WaveType == 3;
            rbQ_WType_OneShot.Checked = lp.WaveType == 4;
            rbQ_WType_Random.Checked = lp.WaveType == 5;
            rbQ_Switch_OFF.Checked = !lp.Switch;
            rbQ_Switch_ON.Checked = lp.Switch;
            txtQ_Trans.Text = lp.Trans.ToString();

            if (mk.LfoParams[2] == null) mk.LfoParams[2] = new Setting.LfoParam();
            lp = mk.LfoParams[2];
            rbR_USE_OFF.Checked = !lp.Use;
            rbR_USE_ON.Checked = lp.Use;
            rbR_Type_Vibrato.Checked = lp.Type == 0 || lp.Type == 1;
            rbR_Type_Tremolo.Checked = lp.Type == 2;
            rbR_Type_Hardware.Checked = lp.Type == 3;
            txtR_Delay.Text = lp.Delay.ToString();
            txtR_Speed.Text = lp.Speed.ToString();
            txtR_Delta.Text = lp.Delta.ToString();
            txtR_Depth.Text = lp.Depth.ToString();
            rbR_WType_Triangle.Checked = lp.WaveType == 0 || lp.WaveType == 1;
            rbR_WType_Saw.Checked = lp.WaveType == 2;
            rbR_WType_Square.Checked = lp.WaveType == 3;
            rbR_WType_OneShot.Checked = lp.WaveType == 4;
            rbR_WType_Random.Checked = lp.WaveType == 5;
            rbR_Switch_OFF.Checked = !lp.Switch;
            rbR_Switch_ON.Checked = lp.Switch;
            txtR_Trans.Text = lp.Trans.ToString();

            if (mk.LfoParams[3] == null) mk.LfoParams[3] = new Setting.LfoParam();
            lp = mk.LfoParams[3];
            rbS_USE_OFF.Checked = !lp.Use;
            rbS_USE_ON.Checked = lp.Use;
            rbS_Type_Vibrato.Checked = lp.Type == 0 || lp.Type == 1;
            rbS_Type_Tremolo.Checked = lp.Type == 2;
            rbS_Type_Hardware.Checked = lp.Type == 3;
            txtS_Delay.Text = lp.Delay.ToString();
            txtS_Speed.Text = lp.Speed.ToString();
            txtS_Delta.Text = lp.Delta.ToString();
            txtS_Depth.Text = lp.Depth.ToString();
            rbS_WType_Triangle.Checked = lp.WaveType == 0 || lp.WaveType == 1;
            rbS_WType_Saw.Checked = lp.WaveType == 2;
            rbS_WType_Square.Checked = lp.WaveType == 3;
            rbS_WType_OneShot.Checked = lp.WaveType == 4;
            rbS_WType_Random.Checked = lp.WaveType == 5;
            rbS_Switch_OFF.Checked = !lp.Switch;
            rbS_Switch_ON.Checked = lp.Switch;
            txtS_Trans.Text = lp.Trans.ToString();
        }

        private void TxtOCT_Leave(object sender, EventArgs e)
        {
            int n;
            if (int.TryParse(txtOCT.Text, out n))
            {
                setting.midiKbd.Octave = Math.Max(Math.Min(n, 8), 0);
            }
            txtOCT.Text = setting.midiKbd.Octave.ToString();
        }

        private void TxtOCT_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter 
                || e.KeyCode == Keys.Escape 
                || e.KeyCode == Keys.PageUp 
                || e.KeyCode == Keys.PageDown)
            {
                TxtOCT_Leave(null, null);
            }
        }

        private void TxtL_Leave(object sender, EventArgs e)
        {
            int n;
            if (int.TryParse(txtL.Text, out n))
            {
                setting.midiKbd.CurrentLength = Math.Max(Math.Min(n, 64), 0);
            }
            txtL.Text = setting.midiKbd.CurrentLength.ToString();
        }

        private void TxtL_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter
                || e.KeyCode == Keys.Escape
                || e.KeyCode == Keys.PageUp
                || e.KeyCode == Keys.PageDown)
            {
                TxtL_Leave(null, null);
            }
        }

        private void RbQ_OFF_Click(object sender, EventArgs e)
        {
            setting.midiKbd.Quantize = 0;
            txtQ.Enabled = false;
        }

        private void RbQ_ON_Click(object sender, EventArgs e)
        {
            txtQ.Enabled = true;
            TxtQ_Leave(null, null);
        }

        private void TxtQ_Leave(object sender, EventArgs e)
        {
            int n;
            if (rbQ_ON.Checked && int.TryParse(txtQ.Text, out n))
            {
                setting.midiKbd.Quantize = Math.Max(Math.Min(n, 64), 0);
            }
            txtQ.Text = setting.midiKbd.Quantize.ToString();
        }

        private void TxtQ_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter
                || e.KeyCode == Keys.Escape
                || e.KeyCode == Keys.PageUp
                || e.KeyCode == Keys.PageDown)
            {
                TxtQ_Leave(null, null);
            }
        }

        private void FrmMIDIKbd_prm_Shown(object sender, EventArgs e)
        {
            Control[] tbl = null;
            tabControl1.SelectedIndex = TargetTab;
            Setting.LfoParam[] lp = setting.midiKbd.LfoParams;

            switch (Target)
            {
                case FrmMIDIKbd.KbdParam.CurrentChannel:
                    txtCH.Focus();
                    break;
                case FrmMIDIKbd.KbdParam.Octave:
                    txtOCT.Focus();
                    break;
                case FrmMIDIKbd.KbdParam.Tempo:
                case FrmMIDIKbd.KbdParam.ClockCounter:
                    break;
                case FrmMIDIKbd.KbdParam.NoteLength:
                    txtL.Focus();
                    break;
                case FrmMIDIKbd.KbdParam.Quantize:
                    rbQ_OFF.Focus();
                    break;
                case FrmMIDIKbd.KbdParam.Lfo:
                    break;
                case FrmMIDIKbd.KbdParam.LfoUse:
                    tbl = new Control[] {
                        lp[0].Use ? rbP_USE_ON : rbP_USE_OFF,
                        lp[1].Use ? rbQ_USE_ON : rbQ_USE_OFF,
                        lp[2].Use ? rbR_USE_ON : rbR_USE_OFF,
                        lp[3].Use ? rbS_USE_ON : rbS_USE_OFF
                    };
                    break;
                case FrmMIDIKbd.KbdParam.LfoType:
                    tbl = new Control[] {
                        lp[0].Type==0 ? rbP_Type_Vibrato : (lp[0].Type==1 ? rbP_Type_Tremolo : rbP_Type_Hardware),
                        lp[1].Type==0 ? rbQ_Type_Vibrato : (lp[1].Type==1 ? rbQ_Type_Tremolo : rbQ_Type_Hardware),
                        lp[2].Type==0 ? rbR_Type_Vibrato : (lp[2].Type==1 ? rbR_Type_Tremolo : rbR_Type_Hardware),
                        lp[3].Type==0 ? rbS_Type_Vibrato : (lp[3].Type==1 ? rbS_Type_Tremolo : rbS_Type_Hardware)
                    };
                    break;
                case FrmMIDIKbd.KbdParam.LfoDelay:
                    tbl = new Control[] {
                        txtP_Delay,
                        txtQ_Delay,
                        txtR_Delay,
                        txtS_Delay
                    };
                    break;
                case FrmMIDIKbd.KbdParam.LfoSpeed:
                    tbl = new Control[] {
                        txtP_Speed,
                        txtQ_Speed,
                        txtR_Speed,
                        txtS_Speed
                    };
                    break;
                case FrmMIDIKbd.KbdParam.LfoDelta:
                    tbl = new Control[] {
                        txtP_Delta,
                        txtQ_Delta,
                        txtR_Delta,
                        txtS_Delta
                    };
                    break;
                case FrmMIDIKbd.KbdParam.LfoDepth:
                    tbl = new Control[] {
                        txtP_Depth,
                        txtQ_Depth,
                        txtR_Depth,
                        txtS_Depth
                    };
                    break;
                case FrmMIDIKbd.KbdParam.LfoWaveType:
                    tbl = new Control[] {
                        lp[0].WaveType==0 ? rbP_WType_Triangle : (lp[0].WaveType==1 ? rbP_WType_Saw 
                            : (lp[0].WaveType==2 ? rbP_WType_Square : (lp[0].WaveType==3 ? rbP_WType_OneShot : rbP_WType_Random))),
                        lp[1].WaveType==0 ? rbQ_WType_Triangle : (lp[1].WaveType==1 ? rbQ_WType_Saw 
                            : (lp[1].WaveType==2 ? rbQ_WType_Square : (lp[1].WaveType==3 ? rbQ_WType_OneShot : rbQ_WType_Random))),
                        lp[2].WaveType==0 ? rbR_WType_Triangle : (lp[2].WaveType==1 ? rbR_WType_Saw 
                            : (lp[2].WaveType==2 ? rbR_WType_Square : (lp[2].WaveType==3 ? rbR_WType_OneShot : rbR_WType_Random))),
                        lp[3].WaveType==0 ? rbS_WType_Triangle : (lp[3].WaveType==1 ? rbS_WType_Saw 
                            : (lp[3].WaveType==2 ? rbS_WType_Square : (lp[3].WaveType==3 ? rbS_WType_OneShot : rbS_WType_Random)))
                    };
                    break;
                case FrmMIDIKbd.KbdParam.LfoSw:
                    tbl = new Control[] {
                        lp[0].Switch ? rbP_Switch_ON : rbP_Switch_OFF,
                        lp[1].Switch ? rbQ_Switch_ON : rbQ_Switch_OFF,
                        lp[2].Switch ? rbR_Switch_ON : rbR_Switch_OFF,
                        lp[3].Switch ? rbS_Switch_ON : rbS_Switch_OFF
                    };
                    break;
                case FrmMIDIKbd.KbdParam.LfoTrans:
                    tbl = new Control[] {
                        txtP_Trans,
                        txtQ_Trans,
                        txtR_Trans,
                        txtS_Trans
                    };
                    break;
            }

            if (tbl != null) tbl[TargetTab - 1].Focus();

        }

        private void CancelEnterBeep(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                e.Handled = true;
            }
        }

    }
}
