using System;
using System.Windows.Forms;

namespace mml2vgmIDEx64
{
    public partial class frmDebug : Form
    {
        public bool append = false;

        public frmDebug()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!append)
            {
                Action<string> act = dispMessage;
                log.dispMsg = act;
                append = true;
            }

            if (Audio.sm == null) return;

            long esc = Audio.EmuSeqCounter;
            long sc = Audio.sm.GetSeqCounter();
            long dlt = sc - esc;
            double escd = Audio.EmuSeqCounterDelta;
            double rscd = Audio.RealSeqCounterDelta;

            lblDriverSeqCounter.Text = Audio.DriverSeqCounter.ToString();
            lblEmuSeqCounter.Text = esc.ToString();
            lblEmuSeqCounterDelta.Text = escd.ToString("F2");
            lblRealSeqCounterDelta.Text = rscd.ToString("F2");
            lblEmuSampleCount.Text = Audio.EmuSampleCount.ToString();
            lblEmuProcTimePer1Frame.Text = Audio.ProcTimePer1Frame.ToString("F2");
            lblSeqCounter.Text = sc.ToString();
            lblRECounter.Text = dlt.ToString();
            lblDataSenderBufferCounter.Text = Audio.sm.GetDataSenderBufferCounter().ToString();
            lblDataSenderBufferSize.Text = Audio.sm.GetDataSenderBufferSize().ToString();
            lblEmuChipSenderBufferSize.Text = Audio.sm.GetEmuChipSenderBufferSize().ToString();
            lblRealChipSenderBufferSize.Text = Audio.sm.GetRealChipSenderBufferSize().ToString();
            lblMusicInterruptTimer.Text = Audio.sm.GetMusicInterruptTimer().ToString();
            lblProcessOverflowCounter.Text = Audio.sm.GetProcessOverFlowCounter().ToString();
            lblProcess1Lap.Text = Audio.sm.GetProcess1Lap().ToString();
            lblProcess2Lap.Text = Audio.sm.GetProcess2Lap().ToString();
            lblSkipFrame.Text = Audio.sm.GetSkipFrame().ToString();

            lblDataMakerIsRunning.Text = Audio.sm.IsRunningAtDataMaker() ? "Running" : "Stop";
            lblDataSenderIsRunning.Text = Audio.sm.IsRunningAtDataSender() ? "Running" : "Stop";
            lblEmuChipSenderIsRunning.Text = Audio.sm.IsRunningAtEmuChipSender() ? "Running" : "Stop";
            lblRealChipSenderIsRunning.Text = Audio.sm.IsRunningAtRealChipSender() ? "Running" : "Stop";

            lblInterrupt.Text = Audio.sm.GetInterrupt() ? "Enable" : "Disable";

            lblLoopCounter.Text = Audio.sm.GetLoopCounter().ToString();
            lblSeqSpeed.Text = Audio.sm.GetSpeed().ToString("F2");
            lblFadeOut.Text = Audio.sm.GetFadeOut() ? "Enable" : "Disable";
            lblFadeOutCounter.Text = Audio.fadeoutCounter.ToString("F2");
        }

        public void dispMessage(string msg)
        {
            try
            {
                Action<string> act = textBox1.AppendText;
                if (this.Created)
                {
                    this.BeginInvoke(act, msg + "\r\n");
                }
            }
            catch
            {
                ;//握りつぶす
            }
        }

        public bool force = false;
        private void frmDebug_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!force)
            {
                MessageBox.Show("Oops! Can't close !");
                e.Cancel = true;
            }
        }
    }
}
