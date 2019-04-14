using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace mml2vgmIDE
{
    public partial class FrmErrorList : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        public Action parentUpdate = null;

        public FrmErrorList()
        {
            InitializeComponent();
        }

        private void FrmErrorList_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            parentUpdate?.Invoke();

        }
    }
}
