using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Sgry.Azuki;

namespace mml2vgmIDE
{
    public partial class FrmEditor : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        public FrmMain main = null;

        public FrmEditor()
        {
            InitializeComponent();

            azukiControl1.SetKeyBind(Keys.F5, compileAndPlay);
        }

        private void compileAndPlay(IUserInterface ui)
        {
            if (main == null) return;
            main.TsmiCompileAndPlay_Click(null, null);
        }
    }
}
