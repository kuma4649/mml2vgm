using Sgry.Azuki;
using Sgry.Azuki.WinForms;
using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace mml2vgmIDE
{
    public partial class FrmEditor : DockContent
    {
        private int searchAnchorIndex;
        //private bool searchUseRegex=false;
        private Regex searchRegex;
        private string anchorTextPattern = "//";
        private string searchTextPattern = "";
        private bool searchMatchCase = false;
        public Document document = null;
        public FrmSien frmSien = null;
        public EnmMmlFileFormat fmt = EnmMmlFileFormat.GWI;
        public int parentID= -1;

        public AzukiControl azukiControl;
        public bool forceClose = false;

        private FrmMain _main;
        private Setting setting;

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

        public FrmEditor(Setting setting, EnmMmlFileFormat fmt, FrmSien sien)
        {
            InitializeComponent();
            this.setting = setting;
            if (fmt == EnmMmlFileFormat.GWI) setHighlighterVGMZGMZGM();
            else if (fmt == EnmMmlFileFormat.MUC) setHighlighterMUC();
            else if (fmt == EnmMmlFileFormat.MML) setHighlighterMML();
            else setHighlighterVGMZGMZGM();

            frmSien = sien;

            //frmSien = new FrmSien(setting);
            //frmSien.parent = main;
            //frmSien.Show();

            this.fmt = fmt;
            Common.SetDoubleBuffered(this);
            Common.SetDoubleBuffered(azukiControl);
        }

        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, System.Windows.Forms.Keys keyData)
        {
            return main.SendProcessCmdKey(ref msg, keyData);
        }

        private void setHighlighterVGMZGMZGM()
        {
            Sgry.Azuki.Highlighter.KeywordHighlighter keywordHighlighter = new Sgry.Azuki.Highlighter.KeywordHighlighter();
            keywordHighlighter.AddRegex("^[^'].*", false, CharClass.DocComment);
            keywordHighlighter.AddRegex("^'[A-Za-z0-9\\-\\,\\+]+_*[ |\\t]", CharClass.Keyword);
            keywordHighlighter.AddRegex("^'[A-Za-z0-9\\-\\,\\+]+~*[ |\\t]", CharClass.Keyword);
            keywordHighlighter.AddRegex("^'@[ |\\t]", CharClass.Keyword);
            keywordHighlighter.AddRegex("^'%\\S+[ |\\t]", CharClass.Keyword);
            keywordHighlighter.AddRegex(";.*", CharClass.DocComment);
            keywordHighlighter.AddEnclosure("'{", "}", CharClass.Comment, true);
            azukiControl = new AzukiControl();
            azukiControl.Font = new Font(setting.other.TextFontName, setting.other.TextFontSize, setting.other.TextFontStyle);
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
            azukiControl.CaretMoved += AzukiControl_CaretMoved;
            azukiControl.AllowDrop = true;
            azukiControl.DragOver += AzukiControl_DragOver;
            azukiControl.DragDrop += AzukiControl_DragDrop;
            azukiControl.Silence = true;

            azukiControl.ColorScheme.ForeColor = Color.FromArgb(setting.ColorScheme.Azuki_ForeColor);
            azukiControl.ColorScheme.BackColor = Color.FromArgb(setting.ColorScheme.Azuki_BackColor);
            azukiControl.ColorScheme.IconBarBack = Color.FromArgb(setting.ColorScheme.Azuki_IconBarBack);
            azukiControl.ColorScheme.LineNumberBack = Color.FromArgb(setting.ColorScheme.Azuki_LineNumberBack_Normal);
            azukiControl.ColorScheme.LineNumberFore = Color.FromArgb(setting.ColorScheme.Azuki_LineNumberFore_Normal);
            azukiControl.ColorScheme.SelectionBack = Color.FromArgb(setting.ColorScheme.Azuki_SelectionBack_Normal);
            azukiControl.ColorScheme.SelectionFore = Color.FromArgb(setting.ColorScheme.Azuki_SelectionFore_Normal);
            azukiControl.ColorScheme.MatchedBracketBack = Color.FromArgb(setting.ColorScheme.Azuki_MatchedBracketBack_Normal);
            azukiControl.ColorScheme.MatchedBracketFore = Color.FromArgb(setting.ColorScheme.Azuki_MatchedBracketFore_Normal);
            azukiControl.ColorScheme.SetColor(CharClass.Keyword, Color.FromArgb(setting.ColorScheme.Azuki_Keyword), Color.Transparent);
            azukiControl.ColorScheme.SetColor(CharClass.Comment, Color.FromArgb(setting.ColorScheme.Azuki_Comment), Color.Transparent);
            azukiControl.ColorScheme.SetColor(CharClass.DocComment, Color.FromArgb(setting.ColorScheme.Azuki_DocComment), Color.Transparent);
            azukiControl.ColorScheme.SetColor(CharClass.Number, Color.FromArgb(setting.ColorScheme.Azuki_Number), Color.Transparent);
            azukiControl.ColorScheme.SetColor(CharClass.Delimiter, Color.FromArgb(setting.ColorScheme.Azuki_Number), Color.Transparent);

            MarkingInfo info = new MarkingInfo(1, "TraceInfo");
            Marking.Register(info);
            TextDecoration dec = new BgColorTextDecoration(Color.DarkGoldenrod);
            azukiControl.ColorScheme.SetMarkingDecoration(1, dec);
            
            info = new MarkingInfo(2, "TraceInfoAlies");
            Marking.Register(info);
            dec = new BgColorTextDecoration(Color.FromArgb(0xff,0xd4,0x68,0x10));
            azukiControl.ColorScheme.SetMarkingDecoration(2, dec);

            this.Controls.Add(azukiControl);

        }

        private void setHighlighterMUC()
        {
            Sgry.Azuki.Highlighter.KeywordHighlighter keywordHighlighter = new Sgry.Azuki.Highlighter.KeywordHighlighter();
            //keywordHighlighter.AddRegex("^.*", false, CharClass.DocComment);
            keywordHighlighter.AddRegex("^[A-Z]+[ |\\t]", CharClass.Keyword);
            keywordHighlighter.AddRegex("^#[A-Za-z0-9]*[ |\\t]", CharClass.Keyword);
            keywordHighlighter.AddRegex("^#\\S*\\*", CharClass.Keyword);
            keywordHighlighter.AddRegex("^'@", CharClass.Keyword);
            keywordHighlighter.AddRegex("^'%\\S+[ |\\t]", CharClass.Keyword);
            keywordHighlighter.AddRegex("^\\{ .*", CharClass.DocComment);
            keywordHighlighter.AddRegex("^\\}.*", CharClass.DocComment);
            keywordHighlighter.AddRegex("\\;.*", CharClass.Comment);
            //keywordHighlighter.AddEnclosure("{", "}", CharClass.Comment, true);
            azukiControl = new AzukiControl();
            azukiControl.Font = new Font(setting.other.TextFontName, setting.other.TextFontSize, setting.other.TextFontStyle);
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
            azukiControl.CaretMoved += AzukiControl_CaretMoved;
            azukiControl.AllowDrop = true;
            azukiControl.DragOver += AzukiControl_DragOver;
            azukiControl.DragDrop += AzukiControl_DragDrop;
            azukiControl.Silence = true;

            azukiControl.ColorScheme.ForeColor = Color.FromArgb(setting.ColorScheme.Azuki_ForeColor);
            azukiControl.ColorScheme.BackColor = Color.FromArgb(setting.ColorScheme.Azuki_BackColor);
            azukiControl.ColorScheme.IconBarBack = Color.FromArgb(setting.ColorScheme.Azuki_IconBarBack);
            azukiControl.ColorScheme.LineNumberBack = Color.FromArgb(setting.ColorScheme.Azuki_LineNumberBack_Normal);
            azukiControl.ColorScheme.LineNumberFore = Color.FromArgb(setting.ColorScheme.Azuki_LineNumberFore_Normal);
            azukiControl.ColorScheme.SelectionBack = Color.FromArgb(setting.ColorScheme.Azuki_SelectionBack_Normal);
            azukiControl.ColorScheme.SelectionFore = Color.FromArgb(setting.ColorScheme.Azuki_SelectionFore_Normal);
            azukiControl.ColorScheme.MatchedBracketBack = Color.FromArgb(setting.ColorScheme.Azuki_MatchedBracketBack_Normal);
            azukiControl.ColorScheme.MatchedBracketFore = Color.FromArgb(setting.ColorScheme.Azuki_MatchedBracketFore_Normal);
            azukiControl.ColorScheme.SetColor(CharClass.Keyword, Color.FromArgb(setting.ColorScheme.Azuki_Keyword), Color.Transparent);
            azukiControl.ColorScheme.SetColor(CharClass.Comment, Color.FromArgb(setting.ColorScheme.Azuki_Comment), Color.Transparent);
            azukiControl.ColorScheme.SetColor(CharClass.DocComment, Color.FromArgb(setting.ColorScheme.Azuki_DocComment), Color.Transparent);
            azukiControl.ColorScheme.SetColor(CharClass.Number, Color.FromArgb(setting.ColorScheme.Azuki_Number), Color.Transparent);
            azukiControl.ColorScheme.SetColor(CharClass.Delimiter, Color.FromArgb(setting.ColorScheme.Azuki_Number), Color.Transparent);

            MarkingInfo info = new MarkingInfo(1, "TraceInfo");
            Marking.Register(info);
            TextDecoration dec = new BgColorTextDecoration(Color.DarkGoldenrod);
            azukiControl.ColorScheme.SetMarkingDecoration(1, dec);

            this.Controls.Add(azukiControl);

        }

        private void setHighlighterMML()
        {
            Sgry.Azuki.Highlighter.KeywordHighlighter keywordHighlighter = new Sgry.Azuki.Highlighter.KeywordHighlighter();
            //keywordHighlighter.AddRegex("^.*", false, CharClass.DocComment);
            keywordHighlighter.AddRegex("^[A-Za-z]+[0-9]*[ |\\t]", CharClass.Keyword);
            keywordHighlighter.AddRegex("^[#|!][A-Za-z0-9]*[ |\\t]", CharClass.Keyword);
            keywordHighlighter.AddRegex("^#\\S*\\*", CharClass.Keyword);
            keywordHighlighter.AddRegex("^@", CharClass.Keyword);
            keywordHighlighter.AddRegex("^'%\\S+[ |\\t]", CharClass.Keyword);
            keywordHighlighter.AddRegex("^\\{ .*", CharClass.DocComment);
            keywordHighlighter.AddRegex("^\\}.*", CharClass.DocComment);
            keywordHighlighter.AddRegex("\\;.*", CharClass.Comment);
            //keywordHighlighter.AddEnclosure("{", "}", CharClass.Comment, true);
            azukiControl = new AzukiControl();
            azukiControl.Font = new Font(setting.other.TextFontName, setting.other.TextFontSize, setting.other.TextFontStyle);
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
            azukiControl.CaretMoved += AzukiControl_CaretMoved;
            azukiControl.AllowDrop = true;
            azukiControl.DragOver += AzukiControl_DragOver;
            azukiControl.DragDrop += AzukiControl_DragDrop;
            azukiControl.Silence = true;

            azukiControl.ColorScheme.ForeColor = Color.FromArgb(setting.ColorScheme.Azuki_ForeColor);
            azukiControl.ColorScheme.BackColor = Color.FromArgb(setting.ColorScheme.Azuki_BackColor);
            azukiControl.ColorScheme.IconBarBack = Color.FromArgb(setting.ColorScheme.Azuki_IconBarBack);
            azukiControl.ColorScheme.LineNumberBack = Color.FromArgb(setting.ColorScheme.Azuki_LineNumberBack_Normal);
            azukiControl.ColorScheme.LineNumberFore = Color.FromArgb(setting.ColorScheme.Azuki_LineNumberFore_Normal);
            azukiControl.ColorScheme.SelectionBack = Color.FromArgb(setting.ColorScheme.Azuki_SelectionBack_Normal);
            azukiControl.ColorScheme.SelectionFore = Color.FromArgb(setting.ColorScheme.Azuki_SelectionFore_Normal);
            azukiControl.ColorScheme.MatchedBracketBack = Color.FromArgb(setting.ColorScheme.Azuki_MatchedBracketBack_Normal);
            azukiControl.ColorScheme.MatchedBracketFore = Color.FromArgb(setting.ColorScheme.Azuki_MatchedBracketFore_Normal);
            azukiControl.ColorScheme.SetColor(CharClass.Keyword, Color.FromArgb(setting.ColorScheme.Azuki_Keyword), Color.Transparent);
            azukiControl.ColorScheme.SetColor(CharClass.Comment, Color.FromArgb(setting.ColorScheme.Azuki_Comment), Color.Transparent);
            azukiControl.ColorScheme.SetColor(CharClass.DocComment, Color.FromArgb(setting.ColorScheme.Azuki_DocComment), Color.Transparent);
            azukiControl.ColorScheme.SetColor(CharClass.Number, Color.FromArgb(setting.ColorScheme.Azuki_Number), Color.Transparent);
            azukiControl.ColorScheme.SetColor(CharClass.Delimiter, Color.FromArgb(setting.ColorScheme.Azuki_Number), Color.Transparent);

            MarkingInfo info = new MarkingInfo(1, "TraceInfo");
            Marking.Register(info);
            TextDecoration dec = new BgColorTextDecoration(Color.DarkGoldenrod);
            azukiControl.ColorScheme.SetMarkingDecoration(1, dec);

            this.Controls.Add(azukiControl);

        }

        private void AzukiControl_DragDrop(object sender, DragEventArgs e)
        {
            //ドラッグされているデータがfileか調べる
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            if (e.Effect == DragDropEffects.None) return;

            string[] source = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (e.Effect == DragDropEffects.Move)
            {
                main.ExecFile(source);
                return;
            }

            IncludeFile(source);
        }

        private void IncludeFile(string[] source)
        {
            foreach (string fn in source) IncludeFile(fn);
        }

        private void IncludeFile(string source)
        {
            string ext = System.IO.Path.GetExtension(source).ToLower();
            if (ext != ".gwi"
                && ext != ".wav"
                && ext != ".bin" //mucom PCM data?
                && ext != ".dat" //mucom voice data?
                && ext != ".tfi"
                )
            {
                log.dispMsg(string.Format("Can't include '{0}'.", source));
                return;
            }

            string path1 = System.IO.Path.GetDirectoryName(this.document.gwiFullPath);
            path1 = string.IsNullOrEmpty(path1) ? this.document.gwiFullPath : path1;
            string basePath = path1 + "\\";
            if (source.IndexOf(basePath) == 0)
            {
                source = source.Substring(basePath.Length);
            }

            string define = "";
            switch (ext)
            {
                case ".gwi":
                    define = "'+ \"{0}\"";
                    break;
                case ".wav":
                    define = "'@ P n , \"{0}\" , 8000 , 100";
                    break;
                case ".bin":
                    define = "'@ MUCOM88ADPCM n , \"{0}\"";
                    break;
                case ".dat":
                    define = "'@ MUCOM88 n , \"{0}\"";
                    break;
                case ".tfi":
                    define = "'@ TFI n , \"{0}\"";
                    break;
            }

            azukiControl.Document.Replace(string.Format("\r\n" + define, source));

        }

        private void AzukiControl_DragOver(object sender, DragEventArgs e)
        {
            //ドラッグされているデータがfileか調べる
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            string[] source = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (!CanMoveFiles(source))
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            DragDropEffects efc = DragDropEffects.Move;
            //shift または ctrl が押されている場合はmove
            if ((e.KeyState & (8 + 4)) != 0)
            {

                if (CanDropFilesToEditor(source))
                {
                    efc = DragDropEffects.Copy;
                    int index = azukiControl.GetIndexFromPosition(azukiControl.PointToClient(new Point(e.X, e.Y)));
                    //int lin;
                    //int col;
                    //azukiControl.GetLineColumnIndexFromCharIndex(index,out lin,out col);
                    azukiControl.SetSelection(index, index);
                    //Console.WriteLine("{0} Line:{1} Column:{2}", index,lin,col);
                }
                else
                {
                    efc = DragDropEffects.None;
                }
            }

            e.Effect = efc;
        }

        private bool CanDropFilesToEditor(string[] source)
        {
            foreach (string fn in source)
            {
                string ext = System.IO.Path.GetExtension(fn).ToLower();
                if (ext == ".gwi"
                    || ext == ".wav"
                    || ext == ".bin" //mucom PCM data?
                    || ext == ".dat" //mucom voice data?
                    )
                {
                    return true;
                }
            }

            return false;
        }

        private bool CanMoveFiles(string[] source)
        {
            //ひとつでも移動可能なファイルが含まれるならばtrue
            foreach (string fn in source)
            {
                string ext = System.IO.Path.GetExtension(fn).ToLower();
                if (ext == ".gwi"
                    || ext == ".mwi" //fmp7 mml data
                    || ext == ".muc" //mucom mml data
                    || ext == ".mml" //common mml data
                    || ext == ".doc"
                    || ext == ".txt"
                    || ext == ".hed"
                    || ext == ".wav"
                    || ext == ".bin" //mucom PCM data?
                    || ext == ".dat" //mucom voice data?
                    )
                {
                    return true;
                }
            }

            return false;
        }

        private void AzukiControl_CaretMoved(object sender, EventArgs e)
        {
            int ci = azukiControl.CaretIndex;
            int row, col;
            azukiControl.GetLineColumnIndexFromCharIndex(ci, out row, out col);
            if (main != null) main.TsslLineCol.Text = string.Format("Line:{0} Col:{1}", row + 1, col + 1);
        }

        public void ActionComment(IUserInterface ui)
        {
            if (main.isTrace) return;

            int b;
            int e;
            azukiControl.GetSelection(out b, out e);

            int st = azukiControl.GetLineHeadIndexFromCharIndex(b);
            b = azukiControl.GetLineHeadIndexFromCharIndex(e);
            int li = azukiControl.GetLineIndexFromCharIndex(e);
            int ed = b + azukiControl.GetLineLength(li);
            azukiControl.SetSelection(st, ed);
            string line = azukiControl.GetSelectedText();
            string[] lines = line.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            //チェック
            bool flg = false;
            foreach (string s in lines)
            {
                if (s.Length < 1 || s[0] != '\'')
                {
                    flg = true;
                    break;
                }
            }
            line = "";
            if (flg)
            {
                // 'をつける
                foreach (string s in lines)
                {
                    line += "'" + s + "\r\n";
                }
                line = line.Substring(0, line.Length - 2);
            }
            else
            {
                // 'をカットする
                foreach (string s in lines)
                {
                    line += s.Substring(1) + "\r\n";
                }
                line = line.Substring(0, line.Length - 2);
            }
            azukiControl.Document.Replace(line, st, ed);
            azukiControl.SetSelection(st, st + line.Length);
        }

        public void ActionShiftEnter(IUserInterface ui)
        {
            int ci = azukiControl.CaretIndex;
            int st = azukiControl.GetLineHeadIndexFromCharIndex(ci);
            int li = azukiControl.GetLineIndexFromCharIndex(ci);
            int ed = st + azukiControl.GetLineLength(li);
            string line = azukiControl.GetTextInRange(st, ci);

            if (line == null || line.Length < 1) return;

            if (fmt == EnmMmlFileFormat.GWI)
            {
                //先頭の文字が'ではないときは既存の動作
                if (line[0] != '\'') return;
            }
            else if (fmt == EnmMmlFileFormat.MUC)
            {
                //パート定義じゃないなら既存の動作
                if (!Regex.IsMatch(line, "^[A-Z]+[\\s\\t].*")) return;
            }
            else if (fmt == EnmMmlFileFormat.MML)
            {
                //パート定義じゃないなら既存の動作
                if (!Regex.IsMatch(line, "^[A-Za-z]+[0-9]*[\\s\\t].*")) return;
            }

            int a = -1;

            //1行を左からサーチし、初めに出現する空白又はタブの位置を取得する
            int s = line.IndexOf(' ');
            int t = line.IndexOf('\t');
            if (s < 0) a = t;
            if (t < 0) a = s;
            if (s >= 0 && t >= 0) a = Math.Min(s, t);


            //空白又はタブが見つからなかった場合は行末まで
            if (a < 0)
            {
                a = line.Length;
                a = (ci - st) < a ? (ci - st) : a;
            }
            else
            {
                if (a + st > ci) a = ci - st;

                while (a < line.Length && (line[a] == ' ' || line[a] == '\t'))
                {
                    a++;
                }
            }

            azukiControl.Document.Replace("\r\n" + line.Substring(0, a));
        }

        public void ActionHome(IUserInterface ui)
        {
            int ci = azukiControl.CaretIndex;
            int st = azukiControl.GetLineHeadIndexFromCharIndex(ci);
            int li = azukiControl.GetLineIndexFromCharIndex(ci);
            int ed = st + azukiControl.GetLineLength(li);
            string line = azukiControl.GetTextInRange(st, ed);
            int a = -1;

            if (line == null || line.Length < 1) return;
            //先頭の文字が'ではないときは既存の動作
            if (line[0] != '\'')
            {
                a = 0;
                while (a < line.Length && (line[a] == ' ' || line[a] == '\t'))
                {
                    a++;
                }

            }
            else
            {
                //1行を左からサーチし、初めに出現する空白又はタブの位置を取得する
                int s = line.IndexOf(' ');
                int t = line.IndexOf('\t');
                if (s < 0) a = t;
                if (t < 0) a = s;
                if (s >= 0 && t >= 0) a = Math.Min(s, t);

                //空白又はタブが見つからなかった場合は先頭へ移動
                if (a < 0)
                {
                    azukiControl.SetSelection(st, st);
                    return;
                }

                while (a < line.Length && (line[a] == ' ' || line[a] == '\t'))
                {
                    a++;
                }
            }

            //既に移動済みの場合は先頭へ移動
            if (st + a == ci)
            {
                azukiControl.SetSelection(st, st);
                return;
            }

            azukiControl.SetSelection(st + a, st + a);
        }

        public void ActionFind(IUserInterface ui)
        {
            form.FrmSearchBox fsb = new form.FrmSearchBox(setting);
            DialogResult res = fsb.ShowDialog();
            if (res == DialogResult.OK)
            {
                searchTextPattern = fsb.cmbPattern.Text;
                AddSearchWordHistory(searchTextPattern);
                ActionFindNext(ui);
            }
            else if (res == DialogResult.Yes)
            {
                searchTextPattern = fsb.cmbPattern.Text;
                AddSearchWordHistory(searchTextPattern);
                ActionFindPrevious(ui);
            }
        }

        private void AddSearchWordHistory(string searchTextPattern)
        {
            if (setting.other.SearchWordHistory.Contains(searchTextPattern))
            {
                setting.other.SearchWordHistory.Remove(searchTextPattern);
            }

            setting.other.SearchWordHistory.Insert(0, searchTextPattern);

            while (setting.other.SearchWordHistory.Count > 100)
            {
                setting.other.SearchWordHistory.RemoveAt(100);
            }

        }

        public void ActionJumpAnchorNext(IUserInterface ui)
        {
            if (anchorTextPattern == "")
            {
                return;
            }
            //searchRegex = new Regex(anchorTextPattern);
            string line = null;
            do
            {
                if (!SearchFindNext(anchorTextPattern, false)) break;
                int ci = azukiControl.CaretIndex;
                int st = azukiControl.GetLineHeadIndexFromCharIndex(ci);
                int li = azukiControl.GetLineIndexFromCharIndex(ci);
                line = azukiControl.GetTextInRange(st, ci);
                if (line == null || line.Length < 1) return;
            } while (line != null && line.Substring(0, anchorTextPattern.Length) != anchorTextPattern);
        }

        public void ActionJumpAnchorPrevious(IUserInterface ui)
        {
            if (anchorTextPattern == "")
            {
                return;
            }
            //searchRegex = new Regex(anchorTextPattern);
            string line = null;
            do
            {
                if (!SearchFindPrevious(anchorTextPattern, false)) break;
                int ci = azukiControl.CaretIndex;
                int st = azukiControl.GetLineHeadIndexFromCharIndex(ci);
                int li = azukiControl.GetLineIndexFromCharIndex(ci);
                line = azukiControl.GetTextInRange(st, ci);
                if (line == null || line.Length < 1) return;
            } while (line != null && line.Substring(0, anchorTextPattern.Length) != anchorTextPattern);
        }

        public void ActionFindNext(IUserInterface ui)
        {
            if (searchTextPattern == "")
            {
                ActionFind(ui);
                return;
            }

            searchRegex = null;
            SearchFindNext(searchTextPattern, false);
        }

        public void ActionFindPrevious(IUserInterface ui)
        {
            if (searchTextPattern == "")
            {
                ActionFind(ui);
                return;
            }
            searchRegex = null;
            SearchFindPrevious(searchTextPattern, false);
        }
        public void ActionDummy(IUserInterface ui)
        {
            ;
        }

        public bool SearchFindNext(string sTextPtn, bool searchUseRegex)
        {
            //AzukiのAnnの検索処理を利用

            Sgry.Azuki.Document azdoc = azukiControl.Document;
            int startIndex;
            TextSegment result;
            Regex regex;

            int s, e;
            azdoc.GetSelection(out s, out e);
            searchAnchorIndex = e + 1;

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
                if (regex == null)
                {
                    // current text pattern was invalid as a regular expression.
                    return false;
                }

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
            if (result != null)
            {
                azukiControl.Document.SetSelection(result.Begin, result.End);
                azukiControl.View.SetDesiredColumn();
                azukiControl.ScrollToCaret();
                searchAnchorIndex = result.End;
            }
            else
            {
                MessageBox.Show("見つかりません");
                return false;
            }

            return true;
        }

        public bool SearchFindPrevious(string sTextPtn, bool searchUseRegex)
        {

            //AzukiのAnnの検索処理を利用

            Sgry.Azuki.Document azdoc = azukiControl.Document;
            int startIndex;
            TextSegment result;
            Regex regex;

            int s, e;
            azdoc.GetSelection(out s, out e);
            searchAnchorIndex = s - 1;

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
                if (regex == null)
                {
                    // current text pattern was invalid as a regular expression.
                    return false;
                }

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
            if (result != null)
            {
                azukiControl.Document.SetSelection(result.End, result.Begin);
                azukiControl.View.SetDesiredColumn();
                azukiControl.ScrollToCaret();
            }
            else
            {
                MessageBox.Show("見つかりません");
                return false;
            }

            return true;
        }

        protected override string GetPersistString()
        {
            return this.Name;
        }

        private void AzukiControl_IconBarClicked(object sender, IconBarClickedEventArgs e)
        {
            AzukiControl_CancelSien(null, null);
            int n = azukiControl.Document.GetLineIconIndex(e.clickedIndex);
            n++;
            if (n == imglstIconBar.Images.Count) n = -1;
            int c = azukiControl.GetLineHeadIndex(e.clickedIndex);
            azukiControl.SetSelection(c, c);
            azukiControl.Document.SetLineIconIndex(e.clickedIndex, n);
        }

        private void acDown(IUserInterface ui)
        {
            if (!frmSien.GetOpacity())
            {
                return;
            }

            if (frmSien.dgvItem.Rows.Count - 1 > frmSien.selRow) frmSien.selRow++;
            frmSien.dgvItem.Rows[frmSien.selRow].Selected = true;
        }

        private void AzukiControl_TextChanged(object sender, EventArgs e)
        {
            if (document == null) return;

            if (!document.edit)
            {
                if (!document.isNew)
                {
                    if (azukiControl.CanUndo) this.Text += "*";
                }
                document.edit = true;
            }

            //main.UpdateControl();

            dispSienForm(-1);
        }

        private void dispSienForm(int row)
        {
            int ci = azukiControl.CaretIndex;
            int st = azukiControl.GetLineHeadIndexFromCharIndex(ci);
            Point ciP = azukiControl.GetPositionFromIndex(Math.Max(ci - 1, 0));
            ciP = azukiControl.PointToScreen(new Point(ciP.X, ciP.Y + azukiControl.LineHeight));
            string line = azukiControl.GetTextInRange(st, ci).TrimStart();
            if (row < 0)
            {
                if (line != "" && line[0] == '\'' && frmSien != null)
                {
                    frmSien.selRow = row;
                    frmSien.Request(line, ciP, parentID, fmt);
                }
            }
            else
            {
                frmSien.selRow = row;
                frmSien.Request(line, ciP, parentID, fmt);
            }
        }

        private void FrmEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                if (!forceClose && (document.isNew || document.edit))
                {
                    DialogResult res = MessageBox.Show("保存せずにファイルを閉じますか？"
                        , "ファイル保存確認"
                        , MessageBoxButtons.YesNo
                        , MessageBoxIcon.Question);
                    if (res != DialogResult.Yes)
                    {
                        e.Cancel = true;
                        return;
                    }
                }

                this.frmSien.Close();
                forceClose = false;
                main.RemoveForm(this);
                main.RemoveDocument(document);

            }
        }

        private void Hokan(string line, Point ciP)
        {
            if (line == "\'@" && setting.UseSien)
            {
                Point r = azukiControl.PointToScreen(new Point(ciP.X, ciP.Y + azukiControl.LineHeight));
                frmSien.Location = new Point(r.X, r.Y);
                frmSien.SetOpacity(true);
                frmSien.update();
                frmSien.dgvItem.Rows[0].Selected = false;
                frmSien.selRow = -1;
            }
            else
            {
                frmSien.SetOpacity(false);
            }
        }

        private void AzukiControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.DockState == DockState.Float)
            {
                main.FrmMain_KeyDown(sender, e);
            }

            if (frmSien == null)
            {
                e.SuppressKeyPress = false;
                return;
            }

            if (!frmSien.GetOpacity())
            {
                e.SuppressKeyPress = false;
                return;
            }

            //支援ウィンドウのキーによる制御
            e.SuppressKeyPress = true;
            
            //Console.WriteLine("{0}", e.KeyCode);

            try
            {
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
                            frmSien.SetOpacity(false);
                        else
                        {
                            frmSien.dgvItem.Rows[frmSien.selRow].Selected = true;
                            frmSien.dgvItem.FirstDisplayedScrollingRowIndex = Math.Max(frmSien.selRow, 0);
                        }
                        break;
                    case Keys.Enter:
                    case Keys.Tab:
                        frmSien.SetOpacity(false);
                        if (frmSien.dgvItem.SelectedRows.Count == 1)
                        {
                            int ci = azukiControl.CaretIndex;
                            SienItem si = (SienItem)frmSien.dgvItem.Rows[frmSien.dgvItem.SelectedRows[0].Index].Tag;

                            if (si.haveChild)
                            {
                                parentID = si.ID;
                                dispSienForm(0);
                                break;
                            }

                            if (si.sienType == 2)
                            {
                                string cnt = si.content;
                                if (cnt.IndexOf("\r\n") < 0 && cnt.IndexOf("\n")>=0)
                                {
                                    cnt=cnt.Replace("\n", "\r\n");
                                }

                                azukiControl.Document.Replace(
                                    (document.srcFileFormat == EnmMmlFileFormat.GWI) ? cnt : (
                                    (document.srcFileFormat == EnmMmlFileFormat.MUC) ? ConvertMucFromGwiOPN(cnt) : (
                                    (document.srcFileFormat == EnmMmlFileFormat.MML) ? ConvertMMLFromGwiOPN(cnt) : (
                                    cnt))),
                                    ci - si.foundCnt,
                                    ci);
                            }
                            else
                            {
                                azukiControl.Document.Replace(
                                    si.content,
                                    ci - si.foundCnt,
                                    ci);
                            }

                            azukiControl.SetSelection(ci - si.foundCnt + si.nextAnchor, ci - si.foundCnt + si.nextCaret);
                        }
                        break;
                    case Keys.Right:
                        if (frmSien.dgvItem.SelectedRows.Count == 1)
                        {
                            SienItem si = (SienItem)frmSien.dgvItem.Rows[frmSien.dgvItem.SelectedRows[0].Index].Tag;
                            if (!si.haveChild)
                            {
                                frmSien.SetOpacity(false);
                                parentID = -1;
                                break;
                            }

                            parentID = si.ID;
                            dispSienForm(0);
                        }
                        break;
                    case Keys.Left:
                        if (frmSien.dgvItem.SelectedRows.Count == 1)
                        {
                            SienItem si = (SienItem)frmSien.dgvItem.Rows[frmSien.dgvItem.SelectedRows[0].Index].Tag;
                            if (si.parentID < -1)
                            {
                                frmSien.SetOpacity(false);
                                parentID = -1;
                                break;
                            }

                            parentID = si.parentID;
                            dispSienForm(0);
                        }
                        break;
                    case Keys.Home:
                    case Keys.End:
                    case Keys.Escape:
                        frmSien.SetOpacity(false);
                        parentID = -1;
                        break;
                    case Keys.Back:
                        frmSien.SetOpacity(false);
                        parentID = -1;
                        e.SuppressKeyPress = false;
                        break;
                    default:
                        parentID = -1;
                        e.SuppressKeyPress = false;
                        break;
                }
            }
            catch
            {
                parentID = -1;
                e.SuppressKeyPress = false;
            }
        }

        private string ConvertMucFromGwiOPN(string content)
        {
            try
            {
                string[] ops;
                if (content.IndexOf("\r\n") >= 0)
                {
                    ops = content.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                }
                else
                {
                    ops = content.Split(new string[] { "\n" }, StringSplitOptions.None);
                }
                string ret = string.Format(
                    "  @001:{{\r\n  {5} {6}\r\n  {0}\r\n  {1}\r\n  {2}\r\n  {3} \"{4}\"}}\r\n"
                    , ops[3].Substring(3)
                    , ops[4].Substring(3)
                    , ops[5].Substring(3)
                    , ops[6].Substring(3)
                    , ops[0].Trim().Substring(0, 8)
                    , ops[8].Substring(3).Trim().Split(' ')[1]
                    , ops[8].Substring(3).Trim().Split(' ')[0]
                    );
                return ret;
            }
            catch
            {
                return "";
            }
        }

        private string ConvertMMLFromGwiOPN(string content)
        {
            try
            {
                string[] ops;
                if (content.IndexOf("\r\n") >= 0)
                {
                    ops = content.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                }
                else
                {
                    ops = content.Split(new string[] { "\n" }, StringSplitOptions.None);
                }

                string o1= ops[3].Trim().Substring(3);
                o1 = o1.Substring(0, o1.Length - 4);
                string o2= ops[4].Trim().Substring(3);
                o2 = o2.Substring(0, o2.Length - 4);
                string o3 = ops[5].Trim().Substring(3);
                o3 = o3.Substring(0, o3.Length - 4);
                string o4 = ops[6].Trim().Substring(3);
                o4 = o4.Substring(0, o4.Length - 4);

                string ret = string.Format(
                    "; nm alg fbl\r\n@xxx {0} ={5}\r\n; ar  dr  sr  rr  sl  tl  ks  ml  dt ams\r\n {1}\r\n {2}\r\n {3}\r\n {4}\r\n"
                    , ops[8].Substring(3)
                    , o1
                    , o2
                    , o3
                    , o4
                    ,ops[0].Trim()
                    );
                return ret;
            }
            catch
            {
                return "";
            }
        }

        private void AzukiControl_CancelSien(object sender, EventArgs e)
        {
            if (frmSien == null) return;
            if (frmSien.GetOpacity()) frmSien.SetOpacity(false);
        }

        private void FrmEditor_Shown(object sender, EventArgs e)
        {
            AzukiControl_CaretMoved(null, null);
        }

        private void FrmEditor_Activated(object sender, EventArgs e)
        {
            if (main != null) main.activeDocument = this;
        }

        private void FrmEditor_Deactivate(object sender, EventArgs e)
        {
            if (main != null && main.activeDocument == this) main.activeDocument = null;
        }
    }
}
