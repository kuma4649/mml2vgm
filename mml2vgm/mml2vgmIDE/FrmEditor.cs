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
