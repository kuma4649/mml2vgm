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
        private EnmMmlFileFormat fmt;
        private Regex searchRegex;
        private bool searchMatchCase = true;//大文字小文字区別する
        private int searchAnchorIndex = -1;
        private TextSegment select = new TextSegment();
        private TextSegment result = null;
        private Setting setting = null;

        public FrmReplaceBox(Setting setting, FrmEditor frmEditor)
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
            this.cmbTo.Items.AddRange(setting.other.ReplaceToWordHistory.ToArray());
            ac = frmEditor.azukiControl;
            fmt = frmEditor.fmt;
            this.setting = setting;
        }

        private void BtnPrevious_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cmbFrom.Text)) return;
            AddSearchWordHistory(cmbFrom.Text);
            bool res = SearchFindPrevious(cmbFrom.Text, false);
            ac.Refresh();
            if (!res) MessageBox.Show("not found");
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cmbFrom.Text)) return;
            AddSearchWordHistory(cmbFrom.Text);
            bool res = SearchFindNext(cmbFrom.Text, false);
            ac.Refresh();
            if (!res) MessageBox.Show("not found");
        }

        private void btnReplace_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cmbFrom.Text)) return;

            AddSearchWordHistory(cmbFrom.Text);
            AddReplaceToWordHistory(cmbTo.Text);

            bool res = Replace();
            ac.Refresh();
            if (!res) MessageBox.Show("not found");
        }

        private void btnAllReplace_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cmbFrom.Text)) return;

            AddSearchWordHistory(cmbFrom.Text);
            AddReplaceToWordHistory(cmbTo.Text);

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
            if (st == ed)
            {
                if (rbTargetAll.Checked)
                    res = SearchFindNext(cmbFrom.Text, false);
                else
                    res = SearchFindNextEx(cmbFrom.Text, false, rbTargetParts.Checked);
            }

            if (!res) return false;

            ac.Document.GetSelection(out st, out ed);
            ac.Document.Replace(cmbTo.Text, st, ed);

            int delta = cmbFrom.Text.Length - cmbTo.Text.Length;
            select.End -= delta;


            if (rbTargetAll.Checked)
                SearchFindNext(cmbFrom.Text, false);
            else
                SearchFindNextEx(cmbFrom.Text, false, rbTargetMMLs.Checked);

            return true;
        }

        public bool SearchFindNextEx(string sTextPtn, bool searchUseRegex,bool mmltarget)
        {
            //AzukiのAnnの検索処理を利用

            Sgry.Azuki.Document azdoc = ac.Document;
            int startIndex;
            int endIndex;
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
            
            endIndex = select.End;

            bool fnd = false;
            int opIndex = 0;
            int conIndex = 0;
            int conEndIndex = 0;

            while (startIndex < endIndex)
            {
                // 対象がmml行か調べる。
                if (fmt == EnmMmlFileFormat.MUC)
                {
                    //対象行の文字列を得る
                    int trgIndex = ac.GetLineIndexFromCharIndex(startIndex);
                    int trgHeadIndex = ac.GetLineHeadIndexFromCharIndex(startIndex);
                    int trgEndIndex = ac.Document.GetLineEndIndexFromCharIndex(startIndex);
                    string con = ac.Document.GetLineContent(trgIndex);
                    Regex rp = new Regex("^[A-Za-z0-9]+[ |\\t]");//パート
                    Match match1 = rp.Match(con, 0);
                    rp = new Regex("^#\\s*\\*[0-9]+");//マクロ
                    Match match2 = rp.Match(con, 0);
                    rp = new Regex("^[#|!][@_A-Za-z0-9]*[ |\\t]");//TAG
                    Match match3 = rp.Match(con, 0);

                    //対象行ではない場合は次の行
                    if (!match1.Success && !match2.Success && !match3.Success)
                    {
                        startIndex = trgEndIndex;
                        continue;
                    }

                    //対象行の場合は次の検索へ
                    opIndex = trgHeadIndex;
                    conIndex = match1.Success
                        ? (match1.Index + match1.Length)
                        : (
                            match2.Success
                            ? (match2.Index + match2.Length)
                            : (match3.Index + match3.Length)
                        )
                        + opIndex;
                    conEndIndex = trgEndIndex;
                }
                else if (fmt == EnmMmlFileFormat.MML)
                {
                    //対象行の文字列を得る
                    int trgIndex = ac.GetLineIndexFromCharIndex(startIndex);
                    int trgHeadIndex = ac.GetLineHeadIndexFromCharIndex(startIndex);
                    int trgEndIndex = ac.Document.GetLineEndIndexFromCharIndex(startIndex);
                    string con = ac.Document.GetLineContent(trgIndex);
                    Regex rp = new Regex("^[A-Za-z]+[0-9]*[ |\\t]");
                    Match match1 = rp.Match(con, 0);
                    rp = new Regex("^[#|!][A-Za-z0-9]*[ |\\t]");
                    Match match2 = rp.Match(con, 0);
                    rp = new Regex("^#\\S*\\*");
                    Match match3 = rp.Match(con, 0);

                    //対象行ではない場合は次の行
                    if (!match1.Success && !match2.Success && !match3.Success)
                    {
                        startIndex = trgEndIndex;
                        continue;
                    }

                    //対象行の場合は次の検索へ
                    opIndex = trgHeadIndex;
                    conIndex = match1.Success
                        ? (match1.Index + match1.Length)
                        : (
                            match2.Success
                            ? (match2.Index + match2.Length)
                            : (match3.Index + match3.Length)
                        )
                        + opIndex;
                    conEndIndex = trgEndIndex;
                }
                else if (fmt == EnmMmlFileFormat.MDL)
                {
                    //対象行の文字列を得る
                    int trgIndex = ac.GetLineIndexFromCharIndex(startIndex);
                    int trgHeadIndex = ac.GetLineHeadIndexFromCharIndex(startIndex);
                    int trgEndIndex = ac.Document.GetLineEndIndexFromCharIndex(startIndex);
                    string con = ac.Document.GetLineContent(trgIndex);
                    Regex rp = new Regex("^[A-Za-z]+[0-9]*[ |\\t]");
                    Match match1 = rp.Match(con, 0);
                    rp = new Regex("^[#|!][A-Za-z0-9]*[ |\\t]");
                    Match match2 = rp.Match(con, 0);
                    rp = new Regex("^'%\\S+[ |\\t]");
                    Match match3 = rp.Match(con, 0);

                    //対象行ではない場合は次の行
                    if (!match1.Success && !match2.Success && !match3.Success)
                    {
                        startIndex = trgEndIndex;
                        continue;
                    }

                    //対象行の場合は次の検索へ
                    opIndex = trgHeadIndex;
                    conIndex = match1.Success
                        ? (match1.Index + match1.Length)
                        : (
                            match2.Success
                            ? (match2.Index + match2.Length)
                            : (match3.Index + match3.Length)
                        )
                        + opIndex;
                    conEndIndex = trgEndIndex;
                }
                else
                {
                    //gwi/etc
                    //対象行の文字列を得る
                    int trgIndex = ac.GetLineIndexFromCharIndex(startIndex);
                    int trgHeadIndex = ac.GetLineHeadIndexFromCharIndex(startIndex);
                    int trgEndIndex = ac.Document.GetLineEndIndexFromCharIndex(startIndex);
                    string con = ac.Document.GetLineContent(trgIndex);
                    Regex rp = new Regex("^'[A-Za-z0-9\\-\\,\\+]+_*[ |\\t]");//パート1
                    Match match1 = rp.Match(con, 0);
                    rp = new Regex("^'[A-Za-z0-9\\-\\,\\+]+~*[ |\\t]");//パート2
                    Match match2 = rp.Match(con, 0);
                    rp = new Regex("^'%\\S+[ |\\t]");//エイリアス
                    Match match3 = rp.Match(con, 0);

                    //対象行ではない場合は次の行
                    if (!match1.Success && !match2.Success && !match3.Success)
                    {
                        startIndex = trgEndIndex;
                        continue;
                    }

                    //対象行の場合は次の検索へ
                    opIndex = trgHeadIndex;
                    conIndex = (
                            match1.Success
                            ? (match1.Index + match1.Length)
                            : (
                                match2.Success
                                ? (match2.Index + match2.Length)
                                : (match3.Index + match3.Length)
                            )
                        )
                        + opIndex;
                    conEndIndex = trgEndIndex;
                }

                //検索対象位置に置換対象の文字列があるか調べる
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

                    if(mmltarget)
                    result = azdoc.FindNext(regex, opIndex, conIndex);
                    else
                        result = azdoc.FindNext(regex, conIndex, conEndIndex);

                }
                else
                {
                    // normal text pattern matching.
                    if (startIndex < azdoc.Length)
                    {
                        if (mmltarget)
                            result = azdoc.FindNext(sTextPtn, opIndex, conIndex, searchMatchCase);
                        else
                            result = azdoc.FindNext(sTextPtn, conIndex, conEndIndex, searchMatchCase);
                    }
                    else
                        result = null;
                }

                if (result == null)
                {
                    startIndex = conEndIndex;
                    continue;
                }

                fnd = true;
                break;
            }

            if (!fnd) return false;

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

        private void AddSearchWordHistory(string searchTextPattern)
        {
            if (setting.other.SearchWordHistory.Contains(searchTextPattern))
                setting.other.SearchWordHistory.Remove(searchTextPattern);

            setting.other.SearchWordHistory.Insert(0, searchTextPattern);

            while (setting.other.SearchWordHistory.Count > 20)
                setting.other.SearchWordHistory.RemoveAt(20);

            cmbFrom.Items.Clear();
            cmbFrom.Items.AddRange(setting.other.SearchWordHistory.ToArray());
        }

        private void AddReplaceToWordHistory(string replaceTextPattern)
        {
            if (setting.other.ReplaceToWordHistory.Contains(replaceTextPattern))
                setting.other.ReplaceToWordHistory.Remove(replaceTextPattern);

            setting.other.ReplaceToWordHistory.Insert(0, replaceTextPattern);

            while (setting.other.ReplaceToWordHistory.Count > 20)
                setting.other.ReplaceToWordHistory.RemoveAt(20);

            cmbTo.Items.Clear();
            cmbTo.Items.AddRange(setting.other.ReplaceToWordHistory.ToArray());
        }



    }
}
