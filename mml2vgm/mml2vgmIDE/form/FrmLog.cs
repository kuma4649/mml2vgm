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
    public partial class FrmLog : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        public Action parentUpdate = null;

        public FrmLog(Setting setting, ThemeBase theme)
        {
            InitializeComponent();
            this.tbLog.BackColor = Color.FromArgb(setting.ColorScheme.Log_BackColor);
            this.tbLog.ForeColor = Color.FromArgb(setting.ColorScheme.Log_ForeColor);
            theme.ApplyTo(toolStrip1);

            Common.SetDoubleBuffered(this);
            Common.SetDoubleBuffered(tbLog);
        }

        public void ClearLog()
        {
            tbLog.Clear();
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

        private void TbLog_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.None;
        }

        private void TsbClearLog_Click(object sender, EventArgs e)
        {
            ClearLog();
        }
    }
}
