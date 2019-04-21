using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Sgry.Azuki;
using Sgry.Azuki.WinForms;
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
                main.LocationChanged += AzukiControl_CancelSien;
                main.SizeChanged += AzukiControl_CancelSien;
            }
        }
        public Document parent = null;
        public FrmSien frmSien = null;
        //public int col = -1;
        public AzukiControl azukiControl;

        public FrmEditor()
        {
            InitializeComponent();

            Sgry.Azuki.Highlighter.KeywordHighlighter keywordHighlighter = new Sgry.Azuki.Highlighter.KeywordHighlighter();
            keywordHighlighter.AddRegex("^[^'].*", false, CharClass.DocComment);
            keywordHighlighter.AddRegex("^'[A-Za-z0-9\\-\\,]+ ", CharClass.Keyword);
            keywordHighlighter.AddRegex("^'@ ", CharClass.Keyword);
            keywordHighlighter.AddRegex("^'%\\S+ ", CharClass.Keyword);
            keywordHighlighter.AddEnclosure("'{", "}", CharClass.Normal, true);

            azukiControl = new AzukiControl();
            azukiControl.Font = new Font("Consolas", 12);
            azukiControl.Dock = DockStyle.Fill;
            azukiControl.ShowsIconBar = true;
            azukiControl.Highlighter = keywordHighlighter;
            azukiControl.IconBarImageList = imglstIconBar;
            azukiControl.IconBarClicked += AzukiControl_IconBarClicked;
            azukiControl.TextChanged += AzukiControl_TextChanged;
            azukiControl.KeyDown += AzukiControl_KeyDown;
            azukiControl.HScroll += AzukiControl_CancelSien;
            azukiControl.VScroll += AzukiControl_CancelSien;
            azukiControl.LocationChanged += AzukiControl_CancelSien;
            azukiControl.SizeChanged += AzukiControl_CancelSien;


            this.Controls.Add(azukiControl);

            frmSien = new FrmSien();
            frmSien.parent = main;
            frmSien.Show();
        }

        private void AzukiControl_IconBarClicked(object sender, IconBarClickedEventArgs e)
        {
            int n = azukiControl.Document.GetLineIconIndex(e.clickedIndex);
            n++;
            if (n == imglstIconBar.Images.Count) n = -1;
            azukiControl.Document.SetLineIconIndex(e.clickedIndex, n);
        }

        private void acDown(IUserInterface ui)
        {
            if (frmSien.Opacity == 0.0)
            {
                return;
            }

            if (frmSien.dgvItem.Rows.Count - 1 > frmSien.selRow) frmSien.selRow++;
            frmSien.dgvItem.Rows[frmSien.selRow].Selected = true;
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
            ciP = azukiControl.PointToScreen(new Point(ciP.X, ciP.Y + azukiControl.LineHeight));
            string line = azukiControl.GetTextInRange(st, ci).TrimStart();
            if (line != "" && line[0] == '\'')
            {
                frmSien.selRow = -1;
                frmSien.Request(line, ciP);
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
                frmSien.selRow = -1;
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
                    if (frmSien.selRow < 0) frmSien.selRow = -1;
                    if (frmSien.dgvItem.Rows.Count - 1 > frmSien.selRow) frmSien.selRow++;
                    frmSien.dgvItem.Rows[frmSien.selRow].Selected = true;
                    frmSien.dgvItem.FirstDisplayedScrollingRowIndex = Math.Max(frmSien.selRow, 0);
                    break;
                case Keys.Up:
                    if (frmSien.dgvItem.Rows.Count > -1) frmSien.selRow--;
                    if (frmSien.selRow < 0)
                        frmSien.Opacity = 0.0;
                    else
                    {
                        frmSien.dgvItem.Rows[frmSien.selRow].Selected = true;
                        frmSien.dgvItem.FirstDisplayedScrollingRowIndex = Math.Max(frmSien.selRow, 0);
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

//                        azukiControl.SetSelection(ci + si.nextAnchor, ci + si.nextCaret);
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
            if (frmSien == null) return;
            frmSien.Opacity = 0.0;
        }

        private void AzukiControl_Click(object sender, EventArgs e)
        {

        }
    }
}
