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
        private FrmMain _main;
        public FrmMain main
        {
            get
            {
                return _main;
            }
            set
            {
                frmSien.parent = value;
                _main = value;
            }
        }
        public Document parent = null;
        public FrmSien frmSien = null;
        public int col = -1;

        public FrmEditor()
        {
            InitializeComponent();
            frmSien = new FrmSien();
            frmSien.parent = main;
            frmSien.Show();
        }

        private void acDown(IUserInterface ui)
        {
            if (frmSien.Opacity == 0.0)
            {
                return;
            }

            if (frmSien.dgvItem.Rows.Count - 1 > col) col++;
            frmSien.dgvItem.Rows[col].Selected = true;
        }

        private void AzukiControl_TextChanged(object sender, EventArgs e)
        {
            if (parent == null) return;

            if (parent.edit)
            {
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

            int ci = azukiControl.CaretIndex;
            int st = azukiControl.GetLineHeadIndexFromCharIndex(ci);
            Point ciP = azukiControl.GetPositionFromIndex(Math.Max(ci - 1, 0));
            string line = azukiControl.GetTextInRange(st, ci).TrimStart();
            if (line!="" && line[0] == '\'')
            {
                Hokan(line,ciP);
            }
        }

        private void FrmEditor_FormClosed(object sender, FormClosedEventArgs e)
        {
            main.RemoveForm(this);
            main.RemoveDocument(parent);
        }

        private void Hokan(string line,Point ciP)
        {
            if (line == "\'@")
            {
                Point r = azukiControl.PointToScreen(new Point(ciP.X, ciP.Y + azukiControl.LineHeight));
                frmSien.Location = new Point(r.X, r.Y);
                frmSien.Opacity = 1.0;
                frmSien.update();
                frmSien.dgvItem.Rows[0].Selected = false;
                col = -1;
            }
            else
            {
                frmSien.Opacity = 0.0;
            }
        }

        private void AzukiControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (frmSien.Opacity == 0.0)
            {
                e.SuppressKeyPress = false;
                return;
            }
            e.SuppressKeyPress = true;
            
            switch (e.KeyCode)
            {
                case Keys.Down:
                    if (col < 0) col = -1;
                    if (frmSien.dgvItem.Rows.Count - 1 > col) col++;
                    frmSien.dgvItem.Rows[col].Selected = true;
                    frmSien.dgvItem.FirstDisplayedScrollingRowIndex = Math.Max(col, 0);
                    break;
                case Keys.Up:
                    if (frmSien.dgvItem.Rows.Count > -1) col--;
                    if (col < 0)
                        frmSien.Opacity = 0.0;
                    else
                    {
                        frmSien.dgvItem.Rows[col].Selected = true;
                        frmSien.dgvItem.FirstDisplayedScrollingRowIndex = Math.Max(col, 0);
                    }
                    break;
                case Keys.Enter:
                case Keys.Tab:
                    frmSien.Opacity = 0.0;
                    if (frmSien.dgvItem.SelectedRows.Count == 1)
                    {
                        int ci = azukiControl.CaretIndex;
                        SienItem si = (SienItem)frmSien.dgvItem.Rows[frmSien.dgvItem.SelectedRows[0].Index].Tag;
                        azukiControl.Document.Replace(
                            si.content,
                            ci - 2,
                            ci);
                        azukiControl.SetSelection(ci + si.nextAnchor, ci + si.nextCaret);
                    }
                    break;
                case Keys.Right:
                case Keys.Left:
                case Keys.Home:
                case Keys.End:
                case Keys.Escape:
                    frmSien.Opacity = 0.0;
                    break;
                default:
                    e.SuppressKeyPress = false;
                    break;
            }
        }

        private void AzukiControl_CancelSien(object sender, EventArgs e)
        {
            frmSien.Opacity = 0.0;
        }

    }
}
