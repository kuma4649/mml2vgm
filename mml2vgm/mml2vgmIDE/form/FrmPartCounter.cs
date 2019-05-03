using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace mml2vgmIDE
{
    public partial class FrmPartCounter : DockContent,IForm
    {
        public Action parentUpdate = null;
        public WindowPosition windowPosition = null;

        public FrmPartCounter(Setting setting)
        {
            InitializeComponent();
            DockStateChanged += FrmPartCounter_DockStateChanged;
            this.dataGridView1.BackgroundColor = Color.FromArgb(setting.ColorScheme.PartCounter_BackColor);
            this.dataGridView1.DefaultCellStyle.BackColor = Color.FromArgb(setting.ColorScheme.PartCounter_BackColor);
            this.dataGridView1.ForeColor = Color.FromArgb(setting.ColorScheme.PartCounter_ForeColor);
        }

        private void FrmPartCounter_DockStateChanged(object sender, EventArgs e)
        {
            //Console.WriteLine(DockState);
            //if (DockState == DockState.Float)
            //{
            //    if (FloatPane != null)
            //    {
            //        FloatPane.FloatWindow.ClientSize = new Size(600, 600);
            //    }
            //}
            //DockWindow dw= this.DockPanel.DockWindows[DockState.DockLeft];
            
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
    }
}
