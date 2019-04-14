using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Sgry.Azuki;
using WeifenLuo.WinFormsUI.Docking;

namespace mml2vgmIDE
{
    public partial class FrmEditor : DockContent
    {
        public FrmMain main = null;
        public Document parent = null;

        public FrmEditor()
        {
            InitializeComponent();

            azukiControl.SetKeyBind(Keys.F1, Open);
            azukiControl.SetKeyBind(Keys.F2, Save);
            azukiControl.SetKeyBind(Keys.F5, CompileAndPlay);
            azukiControl.SetKeyBind(Keys.Control | Keys.O, Open);
            azukiControl.SetKeyBind(Keys.Control | Keys.S, Save);
        }

        private void Open(IUserInterface ui)
        {
            if (main == null) return;
            main.TsmiFileOpen_Click(null, null);
        }

        private void Save(IUserInterface ui)
        {
            if (main == null) return;
            main.TsmiSaveFile_Click(null, null);
        }

        private void CompileAndPlay(IUserInterface ui)
        {
            if (main == null) return;
            main.TsmiCompileAndPlay_Click(null, null);
        }

        private void AzukiControl_TextChanged(object sender, EventArgs e)
        {
            if (parent == null) return;

            if (parent.edit)
            {
                //if (!azukiControl.CanUndo && this.Text.Length > 0 && this.Text[this.Text.Length - 1] == '*')
                //{
                //    parent.edit = false;
                //    this.Text = this.Text.Substring(0, this.Text.Length - 1);
                //}
            }
            else
            {
                if (!parent.isNew)
                {
                    if (azukiControl.CanUndo) this.Text += "*";
                }
                parent.edit = true;
            }

            main.UpdateControl();
        }

        private void FrmEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (parent.edit)
            {
                DialogResult res= MessageBox.Show(
                    "閉じる前に、変更を保存しますか？",
                    "保存確認",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (res == DialogResult.Yes)
                {
                    //名前を付けて保存後閉じる
                }
                else if (res == DialogResult.No)
                {
                    //そのまま閉じる
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        private void FrmEditor_FormClosed(object sender, FormClosedEventArgs e)
        {
            main.RemoveForm(this);
            main.RemoveDocument(parent);
        }
    }
}
