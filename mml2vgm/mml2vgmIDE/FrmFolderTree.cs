using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace mml2vgmIDE
{
    public partial class FrmFolderTree : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        public Action parentUpdate = null;

        public FrmFolderTree()
        {
            InitializeComponent();
        }

        private void FrmFolderTree_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            parentUpdate?.Invoke();
        }
    }
}
