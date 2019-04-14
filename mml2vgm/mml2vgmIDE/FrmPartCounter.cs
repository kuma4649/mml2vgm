using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace mml2vgmIDE
{
    public partial class FrmPartCounter : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        public Action parentUpdate = null;

        public FrmPartCounter()
        {
            InitializeComponent();
        }

        private void FrmPartCounter_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            parentUpdate?.Invoke();
        }
    }
}
