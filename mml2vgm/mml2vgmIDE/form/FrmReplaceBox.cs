using Sgry.Azuki;
using Sgry.Azuki.WinForms;
using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace mml2vgmIDE.form
{
    public partial class FrmReplaceBox : Form
    {
        private AzukiControl ac;
        private Regex searchRegex;
        private bool searchMatchCase = true;//大文字小文字区別する
        private int searchAnchorIndex = -1;
        private TextSegment select = new TextSegment();
        private TextSegment result = null;


        public FrmReplaceBox(Setting setting, AzukiControl azukicontrol)
        {
            InitializeComponent();
            DialogResult = DialogResult.Cancel;
            this.BackColor = Color.FromArgb(setting.ColorScheme.SearchBox_BackColor);
            this.ForeColor = Color.FromArgb(setting.ColorScheme.SearchBox_ForeColor);
            this.cmbFrom.BackColor = Color.FromArgb(setting.ColorScheme.SearchBox_BackColor);
            this.cmbFrom.ForeColor = Color.FromArgb(setting.ColorScheme.SearchBox_ForeColor);
            this.cmbTo.BackColor = Color.FromArgb(setting.ColorScheme.SearchBox_BackColor);
            this.cmbTo.ForeColor = Color.FromArgb(setting.ColorScheme.SearchBox_ForeColor);
            this.btnPrevious.BackColor = Color.FromArgb(setting.ColorScheme.SearchBox_BackColor);
            this.btnPrevious.ForeColor = Color.FromArgb(setting.ColorScheme.SearchBox_ForeColor);
            this.btnNext.BackColor = Color.FromArgb(setting.ColorScheme.SearchBox_BackColor);
            this.btnNext.ForeColor = Color.FromArgb(setting.ColorScheme.SearchBox_ForeColor);
            this.groupBox2.BackColor = Color.FromArgb(setting.ColorScheme.SearchBox_BackColor);
            this.groupBox2.ForeColor = Color.FromArgb(setting.ColorScheme.SearchBox_ForeColor);

            this.cmbFrom.Items.AddRange(setting.other.SearchWordHistory.ToArray());
            ac = azukicontrol;

            //TBD
            this.rbTargetMMLs.Enabled = false;
            this.rbTargetParts.Enabled = false;
        }

        private void BtnPrevious_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cmbFrom.Text)) return;
            bool res = SearchFindPrevious(cmbFrom.Text, false);
            ac.Refresh();
            if (!res) MessageBox.Show("not found");
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cmbFrom.Text)) return;
            bool res = SearchFindNext(cmbFrom.Text, false);
            ac.Refresh();
            if (!res) MessageBox.Show("not found");
        }

        private void btnReplace_Click(object sender, EventArgs e)
        {
            bool res = Replace();
            ac.Refresh();
            if (!res) MessageBox.Show("not found");
        }

        private void btnAllReplace_Click(object sender, EventArgs e)
        {
            ac.Document.BeginUndo();
            while (Replace()) ;
            ac.Document.EndUndo();
            ac.Refresh();
        }

        private void cmbFrom_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down && !cmbFrom.DroppedDown) cmbFrom.DroppedDown = true;
            if (e.KeyCode == Keys.Enter) BtnNext_Click(null, null);
            //if (e.KeyCode == Keys.Escape) this.Close();
        }

        private void cmbTo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down && !cmbFrom.DroppedDown) cmbTo.DroppedDown = true;
            if (e.KeyCode == Keys.Enter) BtnNext_Click(null, null);
            //if (e.KeyCode == Keys.Escape) this.Close();
        }

        private void FrmReplaceBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) this.Close();
        }

        private void FrmReplaceBox_FormClosed(object sender, FormClosedEventArgs e)
        {
            ac.Document.Unmark(0, ac.Document.Length - 1, 3);
            ac.Refresh();
        }

        private void FrmReplaceBox_Shown(object sender, EventArgs e)
        {
            ac.Document.GetSelection(out int st, out int ed);
            searchAnchorIndex = st;
            select.Begin = st;
            select.End = ed;

            if (ed != st)
                ac.Document.Mark(st, ed, 3);
            ac.SetSelection(st, st);
        }



        public bool SearchFindNext(string sTextPtn, bool searchUseRegex)
        {
            //AzukiのAnnの検索処理を利用

            Sgry.Azuki.Document azdoc = ac.Document;
            int startIndex;
            Regex regex;

            if (searchAnchorIndex == -1)
                searchAnchorIndex = select.Begin;
            else
            {
                ac.GetSelection(out int st, out int ed);
                searchAnchorIndex = ed;
            }

            // determine where to start text search
            if (0 <= searchAnchorIndex)
                startIndex = searchAnchorIndex;
            else
                startIndex = Math.Max(azdoc.CaretIndex, azdoc.AnchorIndex);

            // find
            if (searchUseRegex)
            {
                // Regular expression search.
                // get regex object from context
                regex = searchRegex;
                if (regex == null) return false;

                // ensure that "RightToLeft" option of the regex object is NOT set
                RegexOptions opt = regex.Options;
                if ((opt & RegexOptions.RightToLeft) != 0)
                {
                    opt &= ~(RegexOptions.RightToLeft);
                    regex = new Regex(regex.ToString(), opt);
                    searchRegex = regex;
                }
                result = azdoc.FindNext(regex, startIndex, azdoc.Length);
            }
            else
            {
                // normal text pattern matching.
                if (startIndex < azdoc.Length)
                    result = azdoc.FindNext(sTextPtn, startIndex, azdoc.Length, searchMatchCase);
                else
                    result = null;
            }

            // select the result
            if (result == null) return false;
            if (select.Begin != select.End && result.Begin > select.End)
                return false;

            ac.Document.SetSelection(result.Begin, result.End);
            ac.View.SetDesiredColumn();
            ac.ScrollToCaret();
            searchAnchorIndex = result.End;

            return true;
        }

        public bool SearchFindPrevious(string sTextPtn, bool searchUseRegex)
        {

            //AzukiのAnnの検索処理を利用

            Sgry.Azuki.Document azdoc = ac.Document;
            int startIndex;
            TextSegment result;
            Regex regex;

            if (searchAnchorIndex == -1)
                searchAnchorIndex = select.Begin;
            else
            {
                ac.GetSelection(out int st, out int ed);
                searchAnchorIndex = st - 1;
            }

            // determine where to start text search
            if (0 <= searchAnchorIndex)
                startIndex = searchAnchorIndex;
            else
                startIndex = Math.Min(azdoc.CaretIndex, azdoc.AnchorIndex);

            // find
            if (searchUseRegex)
            {
                // Regular expression search.
                // get regex object from context
                regex = searchRegex;
                if (regex == null) return false;

                // ensure that "RightToLeft" option of the regex object is set
                RegexOptions opt = searchRegex.Options;
                if ((opt & RegexOptions.RightToLeft) == 0)
                {
                    opt |= RegexOptions.RightToLeft;
                    searchRegex = new Regex(searchRegex.ToString(), opt);
                }
                result = azdoc.FindPrev(searchRegex, 0, startIndex);
            }
            else
            {
                // normal text pattern matching.
                result = azdoc.FindPrev(sTextPtn, 0, startIndex, searchMatchCase);
            }

            // select the result
            if (result == null) return false;
            if (select.Begin != select.End && result.End <= select.Begin)
                return false;

            ac.Document.SetSelection(result.Begin, result.End);
            ac.View.SetDesiredColumn();
            ac.ScrollToCaret();
            searchAnchorIndex = result.Begin;

            return true;
        }

        private bool Replace()
        {
            ac.Document.GetSelection(out int st, out int ed);

            bool res = true;
            if (st == ed) res = SearchFindNext(cmbFrom.Text, false);

            if (!res) return false;

            ac.Document.GetSelection(out st, out ed);
            ac.Document.Replace(cmbTo.Text, st, ed);

            int delta = cmbFrom.Text.Length - cmbTo.Text.Length;
            select.End -= delta;

            SearchFindNext(cmbFrom.Text, false);

            return true;
        }
    }
}
