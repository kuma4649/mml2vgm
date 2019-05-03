using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace mml2vgmIDE
{
    public partial class FrmLog : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        public Action parentUpdate = null;

        public FrmLog(Setting setting)
        {
            InitializeComponent();
            this.textBox1.BackColor = Color.FromArgb(setting.ColorScheme.Log_BackColor);
            this.textBox1.ForeColor = Color.FromArgb(setting.ColorScheme.Log_ForeColor);
        }
        protected override string GetPersistString()
        {
            return this.Name;
        }

        private void FrmLog_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            parentUpdate?.Invoke();
        }
    }
}
