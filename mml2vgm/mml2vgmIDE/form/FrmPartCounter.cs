using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace mml2vgmIDE
{
    public partial class FrmPartCounter : DockContent,IForm
    {
        public Action parentUpdate = null;
        private MMLParams mmlParams = null;

        public FrmPartCounter(Setting setting)
        {
            InitializeComponent();

            dgvPartCounter.BackgroundColor = Color.FromArgb(setting.ColorScheme.PartCounter_BackColor);
            dgvPartCounter.DefaultCellStyle.BackColor = Color.FromArgb(setting.ColorScheme.PartCounter_BackColor);
            dgvPartCounter.ForeColor = Color.FromArgb(setting.ColorScheme.PartCounter_ForeColor);
            EnableDoubleBuffering(dgvPartCounter);
        }

        public void ClearCounter()
        {
            dgvPartCounter.Rows.Clear();
        }

        public void AddPartCounter(object[] cells)
        {
            DataGridViewRow r = new DataGridViewRow();
            r.CreateCells(dgvPartCounter);

            r.Cells[dgvPartCounter.Columns["ClmPartNumber"].Index].Value = cells[0];
            r.Cells[dgvPartCounter.Columns["ClmIsSecondary"].Index].Value = cells[1];
            r.Cells[dgvPartCounter.Columns["ClmPart"].Index].Value = cells[2];
            r.Cells[dgvPartCounter.Columns["ClmChip"].Index].Value = cells[3];
            r.Cells[dgvPartCounter.Columns["ClmCOunter"].Index].Value = cells[4];

            dgvPartCounter.Rows.Add(r);
        }

        public void Start(MMLParams mmlParams)
        {
            timer.Enabled = true;
            this.mmlParams = mmlParams;
        }

        public void Stop()
        {
            timer.Enabled = false;
            mmlParams = null;
        }


        /// <summary>
        /// ダブルバッファリングを有効にする(from DOBON)
        /// </summary>
        public static void EnableDoubleBuffering(Control control)
        {
            control.GetType().InvokeMember(
               "DoubleBuffered",
               BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
               null,
               control,
               new object[] { true });
        }

        private void FrmPartCounter_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
                parentUpdate?.Invoke();
                return;
            }
        }

        private void FrmPartCounter_FormClosed(object sender, FormClosedEventArgs e)
        {


        }

        protected override string GetPersistString()
        {
            return this.Name;
        }

        private void FrmPartCounter_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.None;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (mmlParams == null) return;

            //パラメータ取得

            //パラメータ描画
            dgvPartCounter.SuspendLayout();

            for (int p = 0; p < dgvPartCounter.Rows.Count; p++)
            {
                string chip = (string)dgvPartCounter.Rows[p].Cells["ClmChip"].Value;
                int r = (int)dgvPartCounter.Rows[p].Cells["ClmPartNumber"].Value-1;
                bool isSecondary=(bool)dgvPartCounter.Rows[p].Cells["ClmIsSecondary"].Value;

                MMLInst mmli = null;
                if (chip == "YM2612X") mmli = mmlParams.YM2612X;
                else if (chip == "YM2612") mmli = mmlParams.YM2612;
                else if (chip == "SN76489") mmli = mmlParams.SN76489;
                else if (chip == "RF5C164") mmli = mmlParams.RF5C164;
                if (mmli == null) continue;

                dgvPartCounter.Rows[p].Cells["ClmInstrument"].Value = mmli.inst[r] == null ? "-" : mmli.inst[r].ToString();
                dgvPartCounter.Rows[p].Cells["ClmEnvelope"].Value = mmli.envelope[r] == null ? "-" : mmli.envelope[r].ToString();
                dgvPartCounter.Rows[p].Cells["ClmVolume"].Value = mmli.vol[r] == null ? "-" : mmli.vol[r].ToString();
                dgvPartCounter.Rows[p].Cells["ClmPan"].Value = mmli.pan[r] == null ? "-" : mmli.pan[r];
                dgvPartCounter.Rows[p].Cells["ClmNote"].Value = mmli.notecmd[r] == null ? "-" : mmli.notecmd[r];
                dgvPartCounter.Rows[p].Cells["ClmLength"].Value = mmli.length[r] == null ? "-" : mmli.length[r];
            }

            dgvPartCounter.ResumeLayout();
        }
    }
}
