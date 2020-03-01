using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using System.IO;
using Core;
using System.Diagnostics;
using SoundManager;
using System.Runtime.InteropServices;
using System.Reflection;
using musicDriverInterface;

namespace mml2vgmIDE
{
    public partial class FrmMain : Form
    {
        private string appName = "mml2vgmIDE";
        private List<Form> FormBox = new List<Form>();
        private List<Document> DocumentBox = new List<Document>();
        public bool isSuccess = true;
        private string[] args;
        private Mml2vgm mv = null;
        private string title = "";
        public FrmLog frmLog = null;
        private FrmPartCounter frmPartCounter = null;
        private FrmFolderTree frmFolderTree = null;
        private FrmErrorList frmErrorList = null;
        private FrmLyrics frmLyrics = null;
        private frmDebug frmDebug = null;
        private FrmMixer frmMixer = null;
        private FrmMIDIKbd frmMIDIKbd = null;
        private bool doPlay = false;
        private bool isTrace = false;
        private bool doSkip = false;
        private bool doSkipStop = false;
        private bool doExport;
        private Point caretPoint = Point.Empty;
        public int Compiling = 0;
        private bool flgReinit = false;
        public const int WM_COPYDATA = 0x004A;
        public const int WM_PASTE = 0x0302;
        public MDChipParams oldParam = new MDChipParams();
        private MDChipParams newParam = new MDChipParams();
        private bool ctrl = false;
        private bool shift = false;
        private ChannelInfo defaultChannelInfo = null;
        private mucomManager mucom = null;
        private musicDriverInterface.MmlDatum[] mubData = null;

        private object traceInfoLockObj = new object();
        private bool traceInfoSw = false;
        private string wrkPath = "";
        private string[] activeMMLTextLines = null;
        private System.Media.SoundPlayer player = null;
        public Setting setting;
        private ToolStripMenuItem tsmiTreeView = null;
        public IDockContent activeDocument;




        //SendMessageで送る構造体（Unicode文字列送信に最適化したパターン）
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpData;
        }

        public FrmMain()
        {
            InitializeComponent();

            DrawBuff.Init();
            //Init();
        }

        public void windowsMessage(ref Message m)
        {
            if (m.Msg == WM_COPYDATA)
            {
                string sParam = ReceiveString(m);
                try
                {

                    //frmPlayList.Stop();

                    //PlayList pl = frmPlayList.getPlayList();
                    //if (pl.lstMusic.Count < 1 || pl.lstMusic[pl.lstMusic.Count - 1].fileName != sParam)
                    //{
                    //    frmPlayList.getPlayList().AddFile(sParam);
                    //    //frmPlayList.AddList(sParam);
                    //}

                    //if (!loadAndPlay(0, 0, sParam))
                    //{
                    //    frmPlayList.Stop();
                    //    Audio.Stop();
                    //    return;
                    //}

                    //frmPlayList.setStart(-1);
                    //oldParam = new MDChipParams();

                    //frmPlayList.Play();

                }
                catch (Exception ex)
                {
                    log.ForcedWrite(ex);
                    //メッセージによる読み込み失敗の場合は何も表示しない
                    //                    MessageBox.Show("ファイルの読み込みに失敗しました。");
                }
            }

        }

        //メッセージ処理
        protected override void WndProc(ref Message m)
        {
            windowsMessage(ref m);
            base.WndProc(ref m);
        }

        //SendString()で送信された文字列を取り出す
        string ReceiveString(Message m)
        {
            string str = null;
            try
            {
                COPYDATASTRUCT cds = (COPYDATASTRUCT)m.GetLParam(typeof(COPYDATASTRUCT));
                str = cds.lpData;
                str = str.Substring(0, cds.cbData / 2);
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
                str = null;
            }
            return str;
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            UpdateControl();
            Core.Common.CheckSoXVersion(System.Windows.Forms.Application.StartupPath, Disp);
            CheckAndLoadMucomDotNET(System.Windows.Forms.Application.StartupPath, Disp);
            OpenLatestFile();
        }

        private void CheckAndLoadMucomDotNET(string startupPath, Action<string> disp)
        {
            string mes = "";
            if (!File.Exists(Path.Combine(startupPath, "mucomDotNETCommon.dll"))) mes += "'mucomDotNETCommon.dll' not found.\r\n";
            if (!File.Exists(Path.Combine(startupPath, "mucomDotNETCompiler.dll"))) mes += "'mucomDotNETCompiler.dll' not found.\r\n";
            if (!File.Exists(Path.Combine(startupPath, "mucomDotNETDriver.dll"))) mes += "'mucomDotNETDriver.dll' not found.\r\n";
            if (!File.Exists(Path.Combine(startupPath, "lang\\mucomDotNETmessage.ja-JP.txt"))) mes += "'lang\\mucomDotNETmessage.ja-JP.txt' not found.\r\n";
            if (!File.Exists(Path.Combine(startupPath, "lang\\mucomDotNETmessage.txt"))) mes += "'lang\\mucomDotNETmessage.txt' not found.\r\n";
            if (mes != "")
            {
                disp(mes);
                return;
            }

            try
            {
                Assembly comp = Assembly.LoadFrom(Path.Combine(startupPath, "mucomDotNETCompiler.dll"));
                Assembly driv = Assembly.LoadFrom(Path.Combine(startupPath, "mucomDotNETDriver.dll"));
                mucom = new mucomManager(comp,driv,disp);
                Audio.mucomManager = mucom;

                disp("mucomDotNETを読み込みました");
                //mucom.compile("D:\\bootcamp\\FM音源\\data\\MUC\\mucom88win_yk2mml181225\\BARE2_MML\\bare03.muc");
            }
            catch
            {
                disp("mucomDotNETの読み込みに失敗しました");
                mucom = null;
                Audio.mucomManager = null;
            }
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            bool flg = false;
            foreach (Document d in DocumentBox)
            {
                if (d.isNew || d.edit)
                {
                    flg = true;
                    break;
                }
            }

            if (flg)
            {
                DialogResult res = MessageBox.Show("保存していないファイルがあります。終了しますか？"
                    , "終了確認"
                    , MessageBoxButtons.YesNo
                    , MessageBoxIcon.Question);
                if (res != DialogResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }
            }

            Finish();
        }

        public void RemoveForm(Form frm)
        {
            FormBox.Remove(frm);
        }

        public void RemoveDocument(Document parent)
        {
            DocumentBox.Remove(parent);
        }

        private void TsmiNew_Click(object sender, EventArgs e)
        {
            OpenFile("");
        }

        public void TsmiFileOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = "全てのサポートファイル(*.gwi;*.muc)|*.gwi;*.muc|gwiファイル(*.gwi)|*.gwi|mucファイル(*.muc)|*.muc|すべてのファイル(*.*)|*.*";
            ofd.Title = "ファイルを開く";
            ofd.RestoreDirectory = true;

            if (ofd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            OpenFile(ofd.FileName);
        }

        private void TsmiOpenFolder_Click(object sender, EventArgs e)
        {

        }

        public void TsmiSaveFile_Click(object sender, EventArgs e)
        {
            DockContent dc = (DockContent)GetActiveDockContent();
            Document d = null;
            if (dc != null)
            {
                if (dc.Tag is Document)
                {
                    d = (Document)dc.Tag;
                }
            }

            if (d == null) return;

            try
            {
                File.WriteAllText(d.gwiFullPath, d.editor.azukiControl.Text, Encoding.UTF8);
            }
            catch(System.IO.IOException ioe)
            {
                MessageBox.Show(string.Format("Occured exception.\r\nMessage:\r\n{0}",ioe.Message),"Saving failed.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            AddGwiFileHistory(d.gwiFullPath);
            UpdateGwiFileHistory();

            d.edit = false;
            d.editor.azukiControl.ClearHistory();
            if (d.editor.Text.Length > 0 && d.editor.Text[d.editor.Text.Length - 1] == '*')
            {
                d.editor.Text = d.editor.Text.Substring(0, d.editor.Text.Length - 1);
            }
            d.isNew = false;
            UpdateControl();
        }

        public void refreshFolderTreeView()
        {
            frmFolderTree.refresh();
            //UpdateFolderTree();
        }

        private void TsmiSaveAs_Click(object sender, EventArgs e)
        {
            DockContent dc = (DockContent)GetActiveDockContent();
            Document d = null;
            if (dc != null)
            {
                if (dc.Tag is Document)
                {
                    d = (Document)dc.Tag;
                }
            }

            if (d == null) return;

            SaveFileDialog sfd = new SaveFileDialog();
            string fn = d.gwiFullPath;
            if (fn.Length > 0 && fn[fn.Length - 1] == '*')
            {
                fn = fn.Substring(0, fn.Length - 1);
            }
            sfd.FileName = Path.GetFileName(fn);
            sfd.InitialDirectory = Path.GetDirectoryName(fn);
            sfd.Filter = "gwiファイル(*.gwi)|*.gwi|すべてのファイル(*.*)|*.*";
            sfd.Title = "名前を付けて保存";
            sfd.RestoreDirectory = true;
            if (sfd.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            fn = Path.Combine(Path.GetDirectoryName(fn), sfd.FileName);
            d.editor.Text = Path.GetFileName(sfd.FileName);
            d.gwiFullPath = fn;
            TsmiSaveFile_Click(null, null);
        }

        private void TsmiImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = "btmファイル(*.btm)|*.btm|すべてのファイル(*.*)|*.*";
            ofd.Title = "ファイルを開く";
            ofd.RestoreDirectory = true;

            if (ofd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            ImportFile(ofd.FileName);
        }

        private void TsmiExport_Click(object sender, EventArgs e)
        {
            try
            {
                DockContent dc = (DockContent)GetActiveDockContent();
                Document d = null;
                if (dc != null)
                {
                    if (dc.Tag is Document)
                    {
                        d = (Document)dc.Tag;
                    }
                }

                if (d == null) return;

                Compile(false, false, false, false, true);
                while (Compiling!=0) { Application.DoEvents(); }//待ち合わせ

                if (msgBox.getErr().Length > 0)
                {
                    MessageBox.Show("コンパイル時にエラーが発生しました。エクスポート処理を中断します。",
                        "エラー発生",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    return;
                }

                SaveFileDialog sfd = new SaveFileDialog();
                string fn = d.gwiFullPath;
                if (fn.Length > 0 && fn[fn.Length - 1] == '*')
                {
                    fn = fn.Substring(0, fn.Length - 1);
                }
                sfd.FileName = Path.GetFileNameWithoutExtension(fn) + (FileInformation.format == enmFormat.VGM ? ".vgm" : (FileInformation.format == enmFormat.XGM ? ".xgm" : ".zgm"));
                sfd.InitialDirectory = Path.GetDirectoryName(fn);
                sfd.Filter = "vgmファイル(*.vgm)|*.vgm|すべてのファイル(*.*)|*.*";
                if (FileInformation.format == enmFormat.XGM)
                {
                    sfd.Filter = "xgmファイル(*.xgm)|*.xgm|すべてのファイル(*.*)|*.*";
                }
                else if (FileInformation.format == enmFormat.ZGM)
                {
                    sfd.Filter = "zgmファイル(*.zgm)|*.zgm|すべてのファイル(*.*)|*.*";
                }
                sfd.Title = "エクスポート";
                sfd.RestoreDirectory = true;
                if (sfd.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                fn = sfd.FileName;
                if (Path.GetExtension(fn) == "")
                {
                    fn = Path.GetFileNameWithoutExtension(fn) + (FileInformation.format == enmFormat.VGM ? ".vgm" : (FileInformation.format == enmFormat.XGM ? ".xgm" : ".zgm"));
                }

                string sf = Path.Combine(
                    Common.GetApplicationDataFolder(true)
                    , "temp"
                    , Path.GetFileNameWithoutExtension(Path.GetFileName(d.gwiFullPath)) + (FileInformation.format == enmFormat.VGM ? ".vgm" : (FileInformation.format == enmFormat.XGM ? ".xgm" : ".zgm"))
                    );
                File.Copy(sf, fn, File.Exists(fn));
            }
            catch (Exception)
            {
                MessageBox.Show("エクスポート処理に失敗しました。", "エクスポート失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TsmiExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void TsmiCompileAndPlay_Click(object sender, EventArgs e)
        {
            Compile(true, false, false, false, false);
        }

        private void TsmiCompileAndTracePlay_Click(object sender, EventArgs e)
        {
            Compile(true, true, false, false, false);
        }

        private void TsmiCompileAndSkipPlay_Click(object sender, EventArgs e)
        {
            Compile(true, true, true, false, false);
        }

        private void TsmiCompile_Click(object sender, EventArgs e)
        {
            Compile(false, false, false, false, false);
        }

        private void TsmiUndo_Click(object sender, EventArgs e)
        {
            Document d = GetActiveDocument();
            if (d != null) d.editor.azukiControl.Undo();
            UpdateControl();
        }

        private void TsmiRedo_Click(object sender, EventArgs e)
        {
            Document d = GetActiveDocument();
            if (d != null) d.editor.azukiControl.Redo();
            UpdateControl();
        }

        private void TsmiFind_Click(object sender, EventArgs e)
        {
            Document d = GetActiveDocument();
            if (d != null) d.editor.ActionFind(null);
        }

        private void TsmiFindNext_Click(object sender, EventArgs e)
        {
            Document d = GetActiveDocument();
            if (d != null) d.editor.ActionFindNext(null);
        }

        private void TsmiFindPrevious_Click(object sender, EventArgs e)
        {
            Document d = GetActiveDocument();
            if (d != null) d.editor.ActionFindPrevious(null);
        }

        private void TsmiShowPartCounter_Click(object sender, EventArgs e)
        {
            if (frmPartCounter.IsHidden) frmPartCounter.Show();
            else frmPartCounter.Hide();

            TsmiShowPartCounter.Checked = !frmPartCounter.IsHidden;
        }

        private void TsmiShowFolderTree_Click(object sender, EventArgs e)
        {
            if (frmFolderTree.IsHidden) frmFolderTree.Show();
            else frmFolderTree.Hide();

            TsmiShowFolderTree.Checked = !frmFolderTree.IsHidden;
        }

        private void TsmiShowErrorList_Click(object sender, EventArgs e)
        {
            if (frmErrorList.IsHidden) frmErrorList.Show();
            else frmErrorList.Hide();

            TsmiShowErrorList.Checked = !frmErrorList.IsHidden;
        }

        private void TsmiShowLog_Click(object sender, EventArgs e)
        {
            if (frmLog.IsHidden) frmLog.Show();
            else frmLog.Hide();

            TsmiShowLog.Checked = !frmLog.IsHidden;
        }

        private void TsmiShowLyrics_Click(object sender, EventArgs e)
        {
            if (frmLyrics.IsHidden) frmLyrics.Show();
            else frmLyrics.Hide();

            TsmiShowLyrics.Checked = !frmLyrics.IsHidden;
        }

        private void TsmiShowMixer_Click(object sender, EventArgs e)
        {
            if (frmMixer == null)
            {
                frmMixer = new FrmMixer(this, 1, newParam.mixer);
                frmMixer.Location = new Point(setting.location.RMixer.X, setting.location.RMixer.Y);
                frmMixer.Show();
                setting.location.ShowMixer = true;
            }
            else
            {
                setting.location.RMixer = new Rectangle(frmMixer.Location.X, frmMixer.Location.Y, 0, 0);
                frmMixer.Close();
                frmMixer = null;
                setting.location.ShowMixer = false;
            }

            TsmiShowMixer.Checked = frmMixer != null;
        }

        private void TsmiShowMIDIKbd_Click(object sender, EventArgs e)
        {
            if (frmMIDIKbd == null)
            {
                if (!Audio.ReadyOK())
                {
                    firstPlay();
                }
                else
                {
                    if (Audio.sm.Mode == SendMode.none)
                    {
                        Audio.sm.RequestStart(SendMode.RealTime);
                    }
                    else
                    {
                        Audio.sm.SetMode(SendMode.RealTime);
                    }
                }

                frmMIDIKbd = new FrmMIDIKbd(this, 2, newParam.mIDIKbd);
                frmMIDIKbd.KeyDown += FrmMain_KeyDown;
                frmMIDIKbd.KeyUp += FrmMain_KeyUp;
                frmMIDIKbd.Show();
                ChannelInfo ci = GetCurrentChannelInfo();
            }
            else
            {
                frmMIDIKbd.Close();
                frmMIDIKbd = null;
            }

            TsmiShowMIDIKbd.Checked = frmMIDIKbd != null;
        }

        private void FrmMIDIKbd_KeyDown(object sender, KeyEventArgs e)
        {
            FrmMain_KeyDown(sender, e);
        }

        private void firstPlay()
        {
            string file = Path.Combine(System.Windows.Forms.Application.StartupPath, "Setup.gwi");
            file = File.ReadAllText(file);
            string[] text = file.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            Compile(true, false, false, false, false, text);
            while (Compiling!=0)
            {
                Thread.Sleep(0);
                Application.DoEvents();
            }
            Audio.sm.ResetMode(SendMode.MML);
        }

        private void TsmiOption_Click(object sender, EventArgs e)
        {
            FrmSetting frmSetting = new FrmSetting(setting);
            DialogResult res = frmSetting.ShowDialog();
            if (res != DialogResult.OK)
            {
                return;
            }

            stop();
            flgReinit = true;
            Reinit(frmSetting.setting);
        }

        private void TsmiTutorial_Click(object sender, EventArgs e)
        {
            Process.Start("Tutorial.txt");
        }

        private void TsmiReference_Click(object sender, EventArgs e)
        {
            Process.Start("CommandReference.txt");
        }

        private void TsmiAbout_Click(object sender, EventArgs e)
        {
            FrmAbout frm = new FrmAbout();
            frm.ShowDialog();
        }

        private void TssbOpen_ButtonClick(object sender, EventArgs e)
        {
            TsmiFileOpen_Click(null, null);
        }

        private void TssbSave_ButtonClick(object sender, EventArgs e)
        {
            TsmiSaveFile_Click(null, null);
        }

        private void TssbFind_ButtonClick(object sender, EventArgs e)
        {
            TsmiFindNext_Click(null, null);
        }

        private void TssbCompile_ButtonClick(object sender, EventArgs e)
        {
            Compile(true, ctrl, shift, false, false);
            //TsmiCompileAndPlay_Click(null, null);
        }

        private void TssbStop_ButtonClick(object sender, EventArgs e)
        {
            stop();
        }

        private void TssbFast_ButtonClick(object sender, EventArgs e)
        {
            ff();
        }

        private void TssbSlow_ButtonClick(object sender, EventArgs e)
        {
            slow();
        }

        private void TssbPlay_ButtonClick(object sender, EventArgs e)
        {
            TsmiCompileAndTracePlay_Click(null, null);
        }

        private void TssbSkipPlay_ButtonClick(object sender, EventArgs e)
        {
            TsmiCompileAndSkipPlay_Click(null, null);
        }

        private void TssbMIDIKbd_ButtonClick(object sender, EventArgs e)
        {
            TsmiShowMIDIKbd_Click(null, null);
        }

        public void FrmMain_KeyDown(object sender, KeyEventArgs e)
        {
            ctrl = (e.KeyData & Keys.Control) == Keys.Control;
            shift = (e.KeyData & Keys.Shift) == Keys.Shift;
            tssbCompile.Text = (ctrl ? "トレース+" : "") + (shift ? "スキップ+" : "") + "再生";

            switch (e.KeyCode)
            {
                case Keys.F1:
                    TsmiFileOpen_Click(null, null);
                    break;
                case Keys.O:
                    if ((e.Modifiers & Keys.Control) == Keys.Control)
                    {
                        TsmiFileOpen_Click(null, null);
                    }
                    break;
                case Keys.F2:
                    TsmiSaveFile_Click(null, null);
                    break;
                case Keys.S:
                    if ((e.Modifiers & Keys.Control) == Keys.Control)
                    {
                        TsmiSaveFile_Click(null, null);
                    }
                    break;
                case Keys.F5:
                    Compile(true, ctrl, shift, false,false);
                    break;
                case Keys.F9:
                    stop();
                    break;
                case Keys.F10:
                    slow();
                    break;
                case Keys.F11:
                    ff();
                    break;
                case Keys.F12:
                    TsmiShowMIDIKbd_Click(null, null);
                    break;
                default:
                    //↓KeyData確認用
                    //log.Write(string.Format("動作未定義のキー：{0}",e.KeyData));
                    break;
            }
        }

        private void FrmMain_KeyUp(object sender, KeyEventArgs e)
        {

            ctrl = (e.KeyData & Keys.Control) == Keys.Control;
            shift = (e.KeyData & Keys.Shift) == Keys.Shift;
            tssbCompile.Text = (ctrl ? "トレース+" : "") + (shift ? "スキップ+" : "") + "再生";

        }


        private void OpenFile(string fileName)
        {
            Document dc = new Document(setting, Path.GetExtension(fileName).ToLower() == ".muc");
            if (fileName != "") dc.InitOpen(fileName);
            dc.editor.Show(dpMain, DockState.Document);
            dc.editor.main = this;
            dc.editor.document = dc;

            frmFolderTree.tvFolderTree.Nodes.Clear();
            frmFolderTree.tvFolderTree.Nodes.Add(dc.gwiTree);
            frmFolderTree.basePath = Path.GetDirectoryName(dc.gwiFullPath);

            FormBox.Add(dc.editor);
            DocumentBox.Add(dc);
            AddGwiFileHistory(fileName);
            UpdateGwiFileHistory();
        }

        private void ImportFile(string fileName)
        {
            Document dc = new Document(setting, false);
            if (fileName != "") dc.InitOpen(fileName);
            dc.editor.Show(dpMain, DockState.Document);
            dc.editor.main = this;
            dc.editor.document = dc;

            frmFolderTree.tvFolderTree.Nodes.Clear();
            frmFolderTree.tvFolderTree.Nodes.Add(dc.gwiTree);
            frmFolderTree.basePath = Path.GetDirectoryName(dc.gwiFullPath);

            FormBox.Add(dc.editor);
            DocumentBox.Add(dc);
        }

        private void AddGwiFileHistory(string fileName)
        {
            List<string> lst = new List<string>();
            lst.Add(fileName);
            if (setting.other.GwiFileHistory != null)
            {
                foreach (string fn in setting.other.GwiFileHistory)
                {
                    bool flg = false;
                    for (int i = 0; i < lst.Count; i++)
                    {
                        if (lst[i] == fn)
                        {
                            flg = true;
                            break;
                        }
                    }

                    if (!flg && !string.IsNullOrEmpty(fn)) lst.Add(fn);
                    if (lst.Count == 10) break;
                }
            }

            setting.other.GwiFileHistory = lst.ToArray();
        }

        private void UpdateGwiFileHistory()
        {
            tsmiGwiFileHistory.DropDownItems.Clear();
            if (setting.other.GwiFileHistory == null) return;
            foreach (string fn in setting.other.GwiFileHistory)
            {
                if (string.IsNullOrEmpty(fn))
                {
                    continue;
                }
                ToolStripMenuItem tsmi = new ToolStripMenuItem(fn);
                tsmi.Click += Tsmi_Click;
                tsmiGwiFileHistory.DropDownItems.Add(tsmi);
            }
        }

        private void OpenLatestFile()
        {
            string lastFn = "";
            if (setting.other.GwiFileHistory == null) return;
            foreach (string fn in setting.other.GwiFileHistory)
            {
                if (string.IsNullOrEmpty(fn))
                    continue;
                lastFn = fn;
                if (File.Exists(fn))
                    break;
            }
            if (string.IsNullOrEmpty(lastFn)) return;

            try
            {
                OpenFile(lastFn);
            }
            catch { }
        }

        private void Tsmi_Click(object sender, EventArgs e)
        {
            string fn = ((ToolStripMenuItem)sender).Text;
            OpenFile(fn);
        }

        public IDockContent GetActiveDockContent()
        {
            IDockContent dc = null;

            foreach (object o in FormBox)
            {
                if (!(o is DockContent))
                {
                    continue;
                }

                DockContent d = (DockContent)o;
                if (d.DockState != DockState.Float)
                {
                    continue;
                }

                if (activeDocument == d)
                {
                    dc = d;
                    break;
                }
            }

            if (dc == null) dc = dpMain.ActiveDocument;

            return dc;
        }

        public void Compile(bool doPlay, bool isTrace, bool doSkip, bool doSkipStop,bool doExport,string[] text=null)
        {
            if (Compiling != 0) return;
            
            stop();

            IDockContent dc = GetActiveDockContent();

            if (text == null)
            {
                if (dc == null) return;
                if (!(dc is FrmEditor)) return;
                activeMMLTextLines = ((FrmEditor)dc).azukiControl.Text.Split(new string[] { "\r\n" }, StringSplitOptions.None);

                string tempPath = Path.Combine(Common.GetApplicationDataFolder(true), "temp", Path.GetFileName(((Document)((FrmEditor)dc).Tag).gwiFullPath));
                title = Path.GetFileName(Path.GetFileName(((Document)((FrmEditor)dc).Tag).gwiFullPath));

                if (Path.GetExtension(tempPath) == ".muc")
                {
                    File.WriteAllText(tempPath, ((FrmEditor)dc).azukiControl.Text, Encoding.GetEncoding(932));
                }

                args = new string[2];
                args[1] = tempPath;
                wrkPath = Path.GetDirectoryName(((Document)((FrmEditor)dc).Tag).gwiFullPath);
            }
            else
            {
                activeMMLTextLines = text;
                args = new string[2];
                //string file = Path.Combine(System.Windows.Forms.Application.StartupPath, "Setup.gwi");
                string tempPath = Path.Combine(Common.GetApplicationDataFolder(true), "temp", "Setup.gwi");
                args[1] = tempPath;
                wrkPath = System.Windows.Forms.Application.StartupPath;
            }


            traceInfoSw = false;
            Sgry.Azuki.WinForms.AzukiControl ac = null;
            if (dc != null)
            {
                ac = ((FrmEditor)dc).azukiControl;
                ac.ColorScheme.LineNumberBack = Color.FromArgb(setting.ColorScheme.Azuki_LineNumberBack_Normal);
                ac.ColorScheme.LineNumberFore = Color.FromArgb(setting.ColorScheme.Azuki_LineNumberFore_Normal);
                statusStrip1.BackColor = Color.FromArgb(setting.ColorScheme.StatusStripBack_Normal);
                ac.Document.Unmark(0, ac.Text.Length, 1);
                ac.IsReadOnly = false;
                ac.Refresh();
            }

            isSuccess = true;
            this.doPlay = doPlay;
            this.isTrace = isTrace;
            this.doSkip = doSkip;
            this.doSkipStop = doSkipStop;
            this.doExport = doExport;
            //スキップ再生の場合はカレットの位置を取得する
            if (doSkip)
            {
                if (ac != null)
                {
                    int ci = ac.CaretIndex;
                    int row, col;
                    ac.GetLineColumnIndexFromCharIndex(ci, out row, out col);
                    caretPoint = new Point(col, row);
                    int st = ac.GetLineHeadIndexFromCharIndex(ci);
                    int li = ac.GetLineIndexFromCharIndex(ci);
                    //int ed = st + ac.GetLineLength(li);
                    string line = ac.GetTextInRange(st, ci);
                    if (line == null || line.Length < 1) doSkip = false;
                    //先頭の文字が'ではないときは既存の動作
                    else if (line[0] != '\'') doSkip = false;
                }
            }
            frmPartCounter.ClearCounter();
            frmErrorList.dataGridView1.Rows.Clear();

            Thread trdStartCompile = new Thread(new ThreadStart(startCompile));
            trdStartCompile.Start();
            Compiling = 1;
        }

        private void startCompile()
        {
            Core.log.Open();
            Core.log.Write("start compile thread");

            Action dmy = updateTitle;

            //for (int i = 1; i < args.Length; i++)
            //{
            //string arg = args[i];
            //if (!File.Exists(arg))
            //{
            //continue;
            //}


            this.Invoke(dmy);

            Core.log.Write(string.Format("  compile at [{0}]", title));

            msgBox.clear();

            //string desfn = Path.ChangeExtension(arg, Properties.Resources.ExtensionVGM);
            //if (tsbToVGZ.Checked)
            //{
            //desfn = Path.ChangeExtension(arg, Properties.Resources.ExtensionVGZ);
            //}

            string ext = Path.GetExtension(args[1]);
            switch (ext)
            {
                case ".gwi":
                    Compiling |= 2;
                    startCompileGWI();
                    break;
                case ".muc":
                    if (mucom == null)
                    {
                        MessageBox.Show("Not found mucomDotNET.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    Compiling |= 4;
                    startCompileMUC();
                    break;
            }
        }

        private void startCompileGWI()
        { 
            Core.log.Write("Call mml2vgm core");

            string stPath = System.Windows.Forms.Application.StartupPath;
            if (!doExport)
            {
                mv = new Mml2vgm(activeMMLTextLines, args[1], null, stPath, Disp, wrkPath, false);
            }
            else
            {
                mv = new Mml2vgm(activeMMLTextLines, args[1], args[1], stPath, Disp, wrkPath, true);
            }
            mv.doSkip = doSkip;
            mv.doSkipStop = doSkipStop;
            mv.caretPoint = caretPoint;
            if (mv.Start() != 0)
            {
                isSuccess = false;
                //break;
            }

            Core.log.Write("Return mml2vgm core");
            //}

            Core.log.Write("Disp Result");

            Action dmy = finishedCompile;
            this.Invoke(dmy);

            Core.log.Write("end compile thread");
            Core.log.Close();
        }

        private void startCompileMUC()
        {
            Core.log.Write("Call mucomDotNET compiler");

            string stPath = System.Windows.Forms.Application.StartupPath;

            try
            {
                mubData = mucom.compile(args[1], wrkPath);
            }
            catch
            {
                isSuccess = false;
            }

            Core.log.Write("Return mucomDotNET compiler");
            //}

            Core.log.Write("Disp Result");

            Action dmy = finishedCompile;
            this.Invoke(dmy);

            Core.log.Write("end compile thread");
            Core.log.Close();
        }

        private void updateTitle()
        {
            if (title == "")
            {
                this.Text = appName;
            }
            else
            {
                this.Text = string.Format("{0} - {1}", appName, title);
            }
        }

        public void Disp(string msg)
        {
            Action<string> msgDisp = MsgDisp;
            this.Invoke(msgDisp, msg);
            Core.log.Write(msg);
        }

        public void MsgDisp(string msg)
        {
            if (frmLog == null) return;
            if (frmLog.IsDisposed) return;

            frmLog.tbLog.AppendText(msg + "\r\n");
        }

        public void ProcessMsgDisp(string msg)
        {
            Action<string> msgDisp = PMsgDisp;
            this.Invoke(msgDisp, msg);
        }

        private void PMsgDisp(string msg)
        {
            if (frmLog == null) return;
            if (frmLog.IsDisposed) return;
            if (string.IsNullOrEmpty(msg)) return;

            frmLog.tbLog.AppendText(msg + "\r\n");
        }

        public void MsgClear()
        {
            if (frmLog == null) return;
            if (frmLog.IsDisposed) return;

            frmLog.tbLog.Clear();
        }

        private void finishedCompile()
        {
            if ((Compiling & 2) != 0)
            {
                finishedCompileGWI();
            }
            else
            {
                finishedCompileMUC();
            }
        }

        private void finishedCompileGWI()
        { 

            if (mv == null)
            {
                if (frmLog != null && !frmLog.IsDisposed) frmLog.tbLog.AppendText(msg.get("I0105"));
                //this.toolStrip1.Enabled = true;
                //this.tsslMessage.Text = msg.get("I0106");
                return;
            }

            if (isSuccess)
            {
                Object[] cells = new object[6];

                foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in mv.desVGM.chips)
                {
                    foreach (ClsChip chip in kvp.Value)
                    {
                        if (chip == null) continue;
                        List<partWork> pw = chip.lstPartWork;
                        for (int i = 0; i < pw.Count; i++)
                        {
                            if (pw[i].clockCounter == 0) continue;

                            cells[0] = int.Parse(pw[i].PartName.Substring(2, 2));
                            cells[1] = chip.ChipID;//ChipIndex
                            cells[2] = pw[i].chipNumber;//ChipNumber
                            cells[3] = pw[i].PartName.Substring(0, 2).Replace(" ", "") + int.Parse(pw[i].PartName.Substring(2, 2)).ToString();
                            cells[4] = pw[i].chip.Name;//.ToUpper();
                            cells[5] = pw[i].clockCounter;
                            frmPartCounter.AddPartCounter(cells);
                        }

                    }
                }
            }

            frmLog.tbLog.AppendText(msg.get("I0107"));

                foreach (msgInfo mes in msgBox.getErr())
                {
                    frmErrorList.dataGridView1.Rows.Add("Error", mes.filename, mes.line == -1 ? "-" : (mes.line + 1).ToString(), mes.body);
                    //frmConsole.textBox1.AppendText(string.Format(msg.get("I0109"), mes));
                }

                foreach (msgInfo mes in msgBox.getWrn())
                {
                    frmErrorList.dataGridView1.Rows.Add("Warning", mes.filename, mes.line == -1 ? "-" : (mes.line + 1).ToString(), mes.body);
                    //frmConsole.textBox1.AppendText(string.Format(msg.get("I0108"), mes));
                }

                frmLog.tbLog.AppendText("\r\n");
                frmLog.tbLog.AppendText(string.Format(msg.get("I0110"), msgBox.getErr().Length, msgBox.getWrn().Length));

                if (mv.desVGM.loopSamples != -1)
                {
                    frmLog.tbLog.AppendText(string.Format(msg.get("I0111"), mv.desVGM.loopClock));
                    if (mv.desVGM.info.format == enmFormat.VGM)
                        frmLog.tbLog.AppendText(string.Format(msg.get("I0112")
                            , mv.desVGM.loopSamples
                            , mv.desVGM.loopSamples / 44100L));
                    else
                        frmLog.tbLog.AppendText(string.Format(msg.get("I0112")
                            , mv.desVGM.loopSamples
                            , mv.desVGM.loopSamples / (mv.desVGM.info.xgmSamplesPerSecond)));
                }

                frmLog.tbLog.AppendText(string.Format(msg.get("I0113"), mv.desVGM.lClock));
                if (mv.desVGM.info.format == enmFormat.VGM)
                    frmLog.tbLog.AppendText(string.Format(msg.get("I0114")
                        , mv.desVGM.dSample
                        , mv.desVGM.dSample / 44100L));
                else
                    frmLog.tbLog.AppendText(string.Format(msg.get("I0114")
                        , mv.desVGM.dSample
                        , mv.desVGM.dSample / (mv.desVGM.info.xgmSamplesPerSecond)));

            frmLog.tbLog.AppendText(msg.get("I0126"));

            if (isSuccess)
            {
                if (args.Length == 2 && doPlay && msgBox.getErr().Length < 1)
                {
                    try
                    {
                        //Process.Start(Path.ChangeExtension(args[1], (mv.desVGM.info.format == enmFormat.VGM) ? Properties.Resources.ExtensionVGM : Properties.Resources.ExtensionXGM));

                        //ヘッダー情報にダミーコマンド情報分の値を水増しした値をセットしなおす
                        if (mv.desVGM.info.format == enmFormat.VGM)
                        {
                            uint EOFOffset = Common.getLE32(mv.desBuf, 0x04) + (uint)mv.desVGM.dummyCmdCounter;
                            Common.SetLE32(mv.desBuf, 0x04, EOFOffset);

                            uint GD3Offset = Common.getLE32(mv.desBuf, 0x14) + (uint)mv.desVGM.dummyCmdCounter;
                            Common.SetLE32(mv.desBuf, 0x14, GD3Offset);

                            uint LoopOffset = (uint)mv.desVGM.dummyCmdLoopOffset - 0x1c;
                            Common.SetLE32(mv.desBuf, 0x1c, LoopOffset);

                            InitPlayer(EnmFileFormat.VGM, mv.desBuf);
                        }
                        else if (mv.desVGM.info.format == enmFormat.XGM)
                        {
                            //uint LoopOffserAddress = (uint)mv.desVGM.dummyCmdLoopOffsetAddress;
                            //uint LoopOffset = (uint)mv.desVGM.dummyCmdLoopOffset;
                            //Common.SetLE24(mv.desBuf, (uint)(mv.desVGM.dummyCmdLoopOffsetAddress + 1), LoopOffset);

                            InitPlayer(EnmFileFormat.XGM, mv.desBuf);
                        }
                        else
                        {
                            InitPlayer(EnmFileFormat.ZGM, mv.desBuf);
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show(msg.get("E0100"), "mml2vgm", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            Compiling = 0;
            UpdateControl();
        }

        private void finishedCompileMUC()
        {
            musicDriverInterface.CompilerInfo ci = mucom.GetCompilerInfo();
            if (isSuccess)
            {
                Object[] cells = new object[6];

                for (int i = 0; i < 11; i++)
                {
                    //if (pw[i].clockCounter == 0) continue;

                    cells[0] = 0;
                    cells[1] = 0;//ChipIndex
                    cells[2] = 0;//ChipNumber
                    cells[3] = ((char)('A' + i)).ToString();
                    cells[4] = "YM2608";//.ToUpper();
                    cells[5] = ci.totalCount[i];
                    frmPartCounter.AddPartCounter(cells);
                }
            }

            //frmLog.tbLog.AppendText(msg.get("I0107"));

            foreach (Tuple<int,int,string> mes in ci.errorList)
            {
                frmErrorList.dataGridView1.Rows.Add("Error", "-"
                    , mes.Item1 == -1 ? "-" : (mes.Item1 + 1).ToString()
                    , mes.Item3);
            }

            foreach (Tuple<int, int, string> mes in ci.warningList)
            {
                frmErrorList.dataGridView1.Rows.Add("Warning", "-"
                    , mes.Item1 == -1 ? "-" : (mes.Item1 + 1).ToString()
                    , mes.Item3);
            }

            if (isSuccess)
            {
                if (doPlay && ci.errorList.Count < 1)
                {
                    try
                    {
                        InitPlayer(EnmFileFormat.MUB, mubData);
                    }
                    catch 
                    {
                        MessageBox.Show(msg.get("E0100"), "mml2vgm", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            Compiling = 0;
            UpdateControl();
        }

        private void DpMain_ActiveDocumentChanged(object sender, EventArgs e)
        {
            UpdateControl();
        }

        public void UpdateControl()
        {
            DockContent dc = (DockContent)GetActiveDockContent();
            Document d = null;
            if (dc != null)
            {
                if (dc.Tag is Document)
                {
                    d = (Document)dc.Tag;
                }
            }

            if (d != null)
            {
                //メニューの有効無効を切り替え
                if (d.isNew)
                {
                    TsmiSaveFile.Enabled = false;
                    tssbSave.Enabled = false;
                    TsmiSaveAs.Enabled = true;
                }
                else
                {
                    TsmiSaveFile.Enabled = true;
                    tssbSave.Enabled = true;
                    TsmiSaveAs.Enabled = true;
                }

                TsmiUndo.Enabled = d.editor.azukiControl.CanUndo;
                TsmiRedo.Enabled = d.editor.azukiControl.CanRedo;

                if (frmFolderTree.tvFolderTree.Nodes.Count == 0 || frmFolderTree.tvFolderTree.Nodes[0] != d.gwiTree) 
                {
                    frmFolderTree.basePath = Path.GetDirectoryName(d.gwiFullPath);
                    frmFolderTree.tvFolderTree.Nodes.Clear();
                    frmFolderTree.tvFolderTree.Nodes.Add(d.gwiTree);
                    frmFolderTree.refresh();
                }

                this.Text = string.Format("{0} - {1}", appName, d.editor.Text);
            }
            else
            {
                TsmiSaveFile.Enabled = false;
                TsmiSaveAs.Enabled = false;
                TsmiUndo.Enabled = false;
                TsmiRedo.Enabled = false;

                this.Text = appName;
            }

            TsmiShowPartCounter.Checked = !frmPartCounter.IsHidden;
            TsmiShowFolderTree.Checked = !frmFolderTree.IsHidden;
            TsmiShowErrorList.Checked = !frmErrorList.IsHidden;
            TsmiShowLog.Checked = !frmLog.IsHidden;
            TsmiShowLyrics.Checked = !frmLyrics.IsHidden;

            tsslCompileError.Text = string.Format(
                "{0}",
                msgBox.getErr().Length
                );
            tsslCompileWarning.Text = string.Format(
                "{0}",
                msgBox.getWrn().Length
                );
            tsslCompileStatus.Text = string.Format(
                "TCnt:{0} LCnt:{1}",
                FileInformation.totalCounter,
                FileInformation.loopCounter == -1 ? "-" : FileInformation.loopCounter.ToString()
                );


        }

        public void UpdateFolderTree()
        {
            DockContent dc = (DockContent)GetActiveDockContent();
            Document d = null;
            if (dc != null)
            {
                if (dc.Tag is Document)
                {
                    d = (Document)dc.Tag;
                }
            }

            if (d == null) return;

            frmFolderTree.tvFolderTree.Nodes.Clear();
            frmFolderTree.tvFolderTree.Nodes.Add(d.gwiTree);
            frmFolderTree.tvFolderTree.Nodes[0].Collapse();
            frmFolderTree.tvFolderTree.Nodes[0].Nodes.Clear();
            frmFolderTree.tvFolderTree.Nodes[0].Nodes.Add("!dmy");
            frmFolderTree.tvFolderTree.Nodes[0].Expand();
            frmFolderTree.Focus();
        }

        public void ExecFile(string[] filenames)
        {

            foreach (string filename in filenames)
            {
                MsgDisp(string.Format("Open '{0}'", filename));

                try
                {
                    if (filename.Length < 1) continue;
                    if (filename[filename.Length - 1] == '\\')
                    {
                        continue;
                    }

                    if (Path.GetExtension(filename).ToLower() == ".gwi")
                    {
                        OpenFile(filename);
                        continue;
                    }

                    if (Path.GetExtension(filename).ToLower() == ".wav")
                    {
                        if (player != null)
                            StopSound();
                        player = new System.Media.SoundPlayer(filename);
                        player.Play();
                        //player.PlaySync();
                        continue;
                    }

                    Process.Start(filename);
                }
                catch
                {
                }
            }

        }

        private void DeleteFile(string[] filenames)
        {
            foreach (string filename in filenames)
            {
                try
                {
                    if (filename.Length < 1) continue;
                    if (filename[filename.Length - 1] == '\\')
                    {
                        Directory.Delete(filename, true);
                        continue;
                    }

                    File.Delete(filename);
                }
                catch
                {

                }
            }

            refreshFolderTreeView();
        }

        private void StopSound()
        {
            if (player != null)
            {
                player.Stop();
                player.Dispose();
                player = null;
            }
        }

        private void JumpDocument(string fn, long ln, bool wantFocus)
        {
            try
            {
                foreach (DockContent dc in dpMain.Documents)
                {
                    if (Path.GetFileName(((Document)dc.Tag).gwiFullPath) != fn)
                    {
                        continue;
                    }

                    Application.DoEvents();
                    Sgry.Azuki.Document d = ((Document)dc.Tag).editor.azukiControl.Document;
                    Sgry.Azuki.IView v = ((Document)dc.Tag).editor.azukiControl.View;
                    if (ln > 1)
                    {
                        int anc = d.GetLineHeadIndex((int)(ln - 1));
                        int caret = d.GetLineHeadIndex((int)ln) - 2;//改行前までを選択する
                        int ancM = d.GetLineHeadIndex((int)(ln - 2));
                        anc = Math.Max(anc, 0);
                        ancM = Math.Max(ancM, 0);
                        caret = Math.Max(anc, caret);
                        v.ScrollPos = v.GetVirPosFromIndex(ancM);//1行手前を画面の最上部になるようスクロールさせる。
                        v.Scroll(1);//scroll barの表示を更新させるため
                        v.Scroll(-1);//scroll barの表示を更新させるため

                        d.SetSelection(anc, caret);
                    }
                    else
                    {
                        v.ScrollPos = v.GetVirPosFromIndex(0);//1行手前を画面の最上部になるようスクロールさせる。
                        d.SetSelection(0, 0);
                    }

                    if (wantFocus) ((Document)dc.Tag).editor.azukiControl.Focus();
                }
            }
            catch (Exception e)
            {
                log.ForcedWrite(e);
            }
        }

        public void Init()
        {
            var theme = new VS2015DarkTheme();
            this.dpMain.Theme = theme;
            theme.ApplyTo(menuStrip1);
            theme.ApplyTo(statusStrip1);

            //setting = Setting.Load();

            this.KeyPreview = true;

            if (Directory.Exists(Path.Combine(Common.GetApplicationDataFolder(true), "temp")))
            {
                DirectoryInfo target = new DirectoryInfo(Path.Combine(Common.GetApplicationDataFolder(true), "temp"));
                foreach (FileInfo file in target.GetFiles())
                {
                    file.Delete();
                }
            }
            else
            {
                Directory.CreateDirectory(Path.Combine(Common.GetApplicationDataFolder(true), "temp"));
            }

            log.ForcedWrite("起動時のAudio初期化処理開始");
            //Audio.Init(setting);

            //Audio.SetMMLTraceInfo = SetMMLTraceInfo;

            log.ForcedWrite("デバッグウィンドウ起動");
            log.debug = setting.Debug_DispFrameCounter;
            if (frmDebug != null)
            {
                frmDebug.force = true;
                frmDebug.Close();
            }
            if (setting.Debug_DispFrameCounter)
            {
                frmDebug = new frmDebug();
                frmDebug.Show();
            }

            this.IsMdiContainer = true;

            FormBox.Add(this);

            frmPartCounter = new FrmPartCounter(setting);
            FormBox.Add(frmPartCounter);

            frmLog = new FrmLog(setting, theme);
            FormBox.Add(frmLog);

            frmFolderTree = new FrmFolderTree(setting,dpMain);
            FormBox.Add(frmFolderTree);

            frmErrorList = new FrmErrorList(setting);
            FormBox.Add(frmErrorList);

            frmLyrics = new FrmLyrics(setting, theme);
            FormBox.Add(frmLyrics);

            if (string.IsNullOrEmpty(setting.dockingState))
            {
                frmPartCounter.Show(dpMain, WeifenLuo.WinFormsUI.Docking.DockState.DockLeft);
                frmLog.Show(dpMain, WeifenLuo.WinFormsUI.Docking.DockState.DockBottom);
                frmFolderTree.Show(dpMain, WeifenLuo.WinFormsUI.Docking.DockState.DockLeft);
                frmErrorList.Show(dpMain, WeifenLuo.WinFormsUI.Docking.DockState.DockBottom);
                frmLyrics.Show(dpMain, WeifenLuo.WinFormsUI.Docking.DockState.DockTop);
            }
            else
            {
                try
                {
                    MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(setting.dockingState));
                    dpMain.LoadFromXml(stream, new DeserializeDockContent(GetDockContentFromPersistString));

                    if (frmPartCounter.ParentForm == null) frmPartCounter.Show(dpMain, DockState.DockLeft);
                    if (frmLog.ParentForm == null) frmLog.Show(dpMain, DockState.DockBottom);
                    if (frmFolderTree.ParentForm == null) frmFolderTree.Show(dpMain, DockState.DockLeft);
                    if (frmErrorList.ParentForm == null) frmErrorList.Show(dpMain, DockState.DockBottom);
                    if (frmLyrics.ParentForm == null) frmLyrics.Show(dpMain, DockState.DockTop);
                }
                catch (Exception ex)
                {
                    log.ForcedWrite(ex);

                    frmPartCounter.Show(dpMain, WeifenLuo.WinFormsUI.Docking.DockState.DockLeft);
                    frmLog.Show(dpMain, WeifenLuo.WinFormsUI.Docking.DockState.DockBottom);
                    frmFolderTree.Show(dpMain, WeifenLuo.WinFormsUI.Docking.DockState.DockLeft);
                    frmErrorList.Show(dpMain, WeifenLuo.WinFormsUI.Docking.DockState.DockBottom);
                    frmLyrics.Show(dpMain, WeifenLuo.WinFormsUI.Docking.DockState.DockTop);
                }
            }

            frmPartCounter.parentUpdate = UpdateControl;
            frmLog.parentUpdate = UpdateControl;
            frmFolderTree.parentUpdate = UpdateControl;
            frmFolderTree.parentExecFile = ExecFile;
            frmFolderTree.parentDeleteFile = DeleteFile;
            frmErrorList.parentUpdate = UpdateControl;
            frmErrorList.parentJumpDocument = JumpDocument;
            frmLyrics.parentUpdate = UpdateControl;

            statusStrip1.BackColor = Color.FromArgb(setting.ColorScheme.StatusStripBack_Normal);

        }

        private IDockContent GetDockContentFromPersistString(string persistString)
        {
            foreach (Form frm in FormBox)
            {
                if (!(frm is IDockContent)) continue;

                if (frm.Name == persistString)
                {
                    return (IDockContent)frm;
                }
            }

            return null;
        }

        private void Reinit(Setting setting)
        {
            if (!flgReinit) return;

            Audio.Stop(0);
            Audio.Close();

            foreach (var dc in dpMain.Documents)
            {
                ((FrmEditor)dc).azukiControl.Font = new Font(setting.other.TextFontName, setting.other.TextFontSize, setting.other.TextFontStyle);
            }


            this.setting = setting;
            this.setting.Save();

            log.ForcedWrite("設定が変更されたため、再度Audio初期化処理開始");

            Audio.Init(this.setting);

            log.ForcedWrite("Audio初期化処理完了");
            log.debug = setting.Debug_DispFrameCounter;
            if (frmDebug != null)
            {
                frmDebug.force = true;
                frmDebug.Close();
            }
            if (setting.Debug_DispFrameCounter)
            {
                frmDebug = new frmDebug();
                frmDebug.Show();
            }

            flgReinit = false;

            for (int i = 0; i < 5; i++)
            {
                System.Threading.Thread.Sleep(100);
                Application.DoEvents();
            }

        }

        public void Finish()
        {
            log.ForcedWrite("終了処理開始");
            log.ForcedWrite("frmMain_FormClosing:STEP 00");

            Audio.Close();
            Audio.RealChipClose();

            MemoryStream stream = new MemoryStream();
            dpMain.SaveAsXml(stream, Encoding.UTF8);
            setting.dockingState = Encoding.UTF8.GetString(stream.ToArray());

            if (WindowState == FormWindowState.Normal)
            {
                setting.location.RMain = new Rectangle(this.Location.X, this.Location.Y, this.Size.Width, this.Size.Height);
            }
            else
            {
                setting.location.RMain = new Rectangle(RestoreBounds.Location.X, RestoreBounds.Location.Y, RestoreBounds.Size.Width, RestoreBounds.Size.Height);
            }
            if (setting.location.ShowMixer)
            {
                setting.location.RMixer = new Rectangle(frmMixer.Location.X, frmMixer.Location.Y, 0,0);
            }

            frmPartCounter.Close();

            setting.Save();
        }

        public bool InitPlayer(EnmFileFormat format, outDatum[] srcBuf)
        {
            if (srcBuf == null) return false;

            try
            {
                IDockContent dc = GetActiveDockContent();
                Sgry.Azuki.WinForms.AzukiControl ac = null;
                if (dc != null && (dc is FrmEditor))
                {
                    ac = ((FrmEditor)dc).azukiControl;
                }


                if (Audio.flgReinit) flgReinit = true;
                if (setting.other.InitAlways) flgReinit = true;
                //Reinit(setting);


                //rowとcolをazuki向けlinePosに変換する
                if (ac != null)
                {
                    foreach (outDatum od in srcBuf)
                    {
                        if (od.linePos == null) continue;
                        //Console.WriteLine("{0} {1}", od.linePos.row, od.linePos.col);
                        od.linePos.col = ac.GetCharIndexFromLineColumnIndex(od.linePos.row, od.linePos.col);

                    }
                }


                if (Audio.isPaused)
                {
                    Audio.Pause();
                }

                Audio.SetVGMBuffer(format, srcBuf);

                for (int i = 0; i < 100; i++)
                {
                    Thread.Sleep(1);
                    Application.DoEvents();
                }

                if (srcBuf != null)
                {
                    playdata();
                    if (Audio.errMsg != "")
                    {
                        stop();
                        return false;
                    }
                }

                frmLyrics.update();
                frmPartCounter.Stop();
                Audio.mmlParams.Init(isTrace);
                frmPartCounter.Start(Audio.mmlParams);

                if (isTrace && ac != null)
                {
                    ac.ColorScheme.LineNumberBack = Color.FromArgb(setting.ColorScheme.Azuki_LineNumberBack_Trace);
                    ac.ColorScheme.LineNumberFore = Color.FromArgb(setting.ColorScheme.Azuki_LineNumberFore_Trace);
                    statusStrip1.BackColor = Color.FromArgb(setting.ColorScheme.StatusStripBack_Trace);
                    ac.Refresh();
                    traceInfoSw = true;
                    ac.IsReadOnly = true;
                }
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
                srcBuf = null;
                MessageBox.Show(
                    string.Format("ファイルの読み込みに失敗しました。\r\nメッセージ={0}", ex.Message),
                    "TinyMDPlayer",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        public bool InitPlayer(EnmFileFormat format, musicDriverInterface.MmlDatum[] mubdata)
        {
            if (mucom == null) return false;
            if (mubdata == null || mubdata.Length < 1) return false;

            try
            {
                IDockContent dc = GetActiveDockContent();
                Sgry.Azuki.WinForms.AzukiControl ac = null;
                if (dc != null && (dc is FrmEditor))
                {
                    ac = ((FrmEditor)dc).azukiControl;
                }


                if (Audio.flgReinit) flgReinit = true;
                if (setting.other.InitAlways) flgReinit = true;
                //Reinit(setting);


                //rowとcolをazuki向けlinePosに変換する
                if (ac != null)
                {
                    foreach (musicDriverInterface.MmlDatum od in mubData)
                    {
                        if (od == null) continue;
                        if (od.linePos == null) continue;
                        if (od.linePos.row == -1 || od.linePos.col == -1) continue;

                        od.linePos.col = ac.GetCharIndexFromLineColumnIndex(
                            (int)(od.linePos.row - 1)
                            , (int)(od.linePos.col - 1)
                            );
                    }
                }


                if (Audio.isPaused)
                {
                    Audio.Pause();
                }

                //List<byte> dmy = new List<byte>();
                //foreach(mucomDotNET.Interface.MubDat md in mubdata)
                //{
                //    dmy.Add(md.dat);
                //}
                //File.WriteAllBytes("c:\\temp\\test.mub",dmy.ToArray());

                Audio.SetVGMBuffer(format, mubData, wrkPath);

                for (int i = 0; i < 100; i++)
                {
                    Thread.Sleep(1);
                    Application.DoEvents();
                }

                if (mubData != null)
                {
                    playdata();
                    if (Audio.errMsg != "")
                    {
                        stop();
                        return false;
                    }
                }

                frmLyrics.update();
                frmPartCounter.Stop();
                Audio.mmlParams.Init(isTrace);
                frmPartCounter.Start(Audio.mmlParams);

                if (isTrace && ac != null)
                {
                    ac.ColorScheme.LineNumberBack = Color.FromArgb(setting.ColorScheme.Azuki_LineNumberBack_Trace);
                    ac.ColorScheme.LineNumberFore = Color.FromArgb(setting.ColorScheme.Azuki_LineNumberFore_Trace);
                    statusStrip1.BackColor = Color.FromArgb(setting.ColorScheme.StatusStripBack_Trace);
                    ac.Refresh();
                    traceInfoSw = true;
                    ac.IsReadOnly = true;
                }
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
                mubData = null;
                MessageBox.Show(
                    string.Format("ファイルの読み込みに失敗しました。\r\nメッセージ={0}", ex.Message),
                    "TinyMDPlayer",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void playdata()
        {
            try
            {

                if (Audio.isPaused)
                {
                    Audio.Pause();
                }
                Audio.Stop(0);
                ResumeNormalModeDisp();

                if (!Audio.Play(setting, doSkipStop))
                {
                    try
                    {
                        Audio.Stop(0);
                    }
                    catch (Exception ex)
                    {
                        log.ForcedWrite(ex);
                    }
                    if (Audio.errMsg == "") throw new Exception();
                    else
                    {
                        MessageBox.Show(Audio.errMsg, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

            }
            catch (Exception e)
            {
                Audio.errMsg = e.Message;
            }
        }

        public void stop()
        {
            if (Audio.isPaused)
            {
                Audio.Pause();
            }

            frmPartCounter.Stop();

            if (frmMIDIKbd == null)
            {
                //鍵盤が表示されていない場合は完全に停止する
                Audio.Stop(SendMode.Both);
            }
            else
            {
                //鍵盤が表示されている場合はmmlの演奏のみ停止し、リアルタイム入力は受け付けるままにする
                Audio.Stop(SendMode.MML);
            }
            ResumeNormalModeDisp();
        }

        public void ff()
        {
            if (Audio.isPaused)
            {
                Audio.Pause();
            }

            Audio.FF();
        }

        public void slow()
        {
            if (Audio.isPaused)
            {
                Audio.StepPlay(4000);
                Audio.Pause();
                return;
            }

            //if (Audio.isStopped)
            //{
            //    play();
            //}

            Audio.Slow();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateTraceInfo();
            UpdateScreenInfo();

            long w = Audio.EmuSeqCounter;
            double sec = (double)w / (double)Common.SampleRate;
            toolStripStatusLabel1.Text = string.Format("{0:d2}:{1:d2}.{2:d2}", (int)(sec / 60), (int)(sec%60), (int)(sec * 100 % 100));
        }

        private void UpdateScreenInfo()
        {
            screenChangeParams();
            screenDrawParams();
        }

        private void screenChangeParams()
        {
            if (frmMixer != null && !frmMixer.isClosed)
            {
                frmMixer.screenChangeParams();
            }
            else
            {
                frmMixer = null;
                TsmiShowMixer.Checked = (frmMixer != null);
            }

            if (frmMIDIKbd != null && !frmMIDIKbd.isClosed)
            {
                frmMIDIKbd.screenChangeParams();
            }
            else
            {
                frmMIDIKbd = null;
                TsmiShowMIDIKbd.Checked = (frmMIDIKbd != null);
            }
        }

        private void screenDrawParams()
        {
            if (frmMixer != null && !frmMixer.isClosed)
            {
                frmMixer.screenDrawParams();
                frmMixer.update();
            }
            else frmMixer = null;

            if (frmMIDIKbd != null && !frmMIDIKbd.isClosed)
            {
                frmMIDIKbd.screenDrawParams();
                frmMIDIKbd.update();
            }
            else frmMIDIKbd = null;
        }

        private void UpdateTraceInfo()
        {
            if (!traceInfoSw) return;


            if ((Audio.sm.Mode & SendMode.MML) != SendMode.MML)
            {
                traceInfoSw = false;

                ResumeNormalModeDisp();
                return;
            }


            IDockContent dcnt = GetActiveDockContent();
            if (dcnt == null) return;
            if (!(dcnt is FrmEditor)) return;
            FrmEditor fe = ((FrmEditor)dcnt);
            Sgry.Azuki.WinForms.AzukiControl ac = fe.azukiControl;
            bool refresh = false;

            try
            {
                foreach (string instName in Audio.mmlParams.Insts.Keys)
                {
                    foreach (int chipIndex in Audio.mmlParams.Insts[instName].Keys)
                    {
                        foreach (int chipNumber in Audio.mmlParams.Insts[instName][chipIndex].Keys)
                        {
                            MMLParameter.Instrument i = Audio.mmlParams.Insts[instName][chipIndex][chipNumber];
                            for (int ch = 0; ch < i.ChCount; ch++)
                            {
                                bool ret = MarkUpTraceInfo(i.TraceInfo, i.TraceInfoOld, ch, fe, ac);
                                if (ret) refresh = ret;
                            }
                        }
                    }
                }

                if (refresh)
                {
                    ac.Refresh();
                    //ac.View.ScrollToCaret();
                }
            }
            catch
            {
                ;//何もしない
            }
        }

        private void ResumeNormalModeDisp()
        {
            foreach (object o in FormBox)
            {
                if (!(o is DockContent))
                {
                    continue;
                }

                DockContent d = (DockContent)o;
                if (d == null) continue;
                if (!(d is FrmEditor)) continue;
                Sgry.Azuki.WinForms.AzukiControl a = ((FrmEditor)d).azukiControl;
                a.ColorScheme.LineNumberBack = Color.FromArgb(setting.ColorScheme.Azuki_LineNumberBack_Normal);
                a.ColorScheme.LineNumberFore = Color.FromArgb(setting.ColorScheme.Azuki_LineNumberFore_Normal);
                a.Document.Unmark(0, a.Text.Length, 1);
                a.IsReadOnly = false;
                a.Refresh();
            }

            this.statusStrip1.BackColor = Color.FromArgb(setting.ColorScheme.StatusStripBack_Normal);
        }

        private bool MarkUpTraceInfo(Queue<outDatum>[] ods, outDatum[] odos, int ch, FrmEditor fe, Sgry.Azuki.WinForms.AzukiControl ac)
        {
            bool flg = false;
            if (ods[ch] == null) return false;

            while (ods[ch].Count > 0)
            {
                outDatum od;
                lock (traceInfoLockObj)
                {
                    od = ods[ch].Dequeue();
                }
                outDatum odo = odos[ch];
                if (od != null
                    && od != odo
                    && (od.type == enmMMLType.Note || od.type == enmMMLType.Rest || od.type == enmMMLType.Lyric)
                    && (
                        (odo != null && od.linePos.col != odo.linePos.col)
                        || odo == null
                    )
                    && (fe.Text == od.linePos.filename || fe.Text == od.linePos.filename + "*")
                )
                {
                    int i, c;
                    ac.GetLineColumnIndexFromCharIndex(od.linePos.col, out i, out c);
                    //log.Write(string.Format("{0} {1}", i, c));
                    lock (traceInfoLockObj)
                    {
                        if (odo != null)
                        {
                            try
                            {
                                ac.Document.Unmark(odo.linePos.col, odo.linePos.col + Math.Max(odo.linePos.length, 1), 1);
                            }
                            catch
                            {
                                ;//何もしない
                            }
                        }
                        ac.Document.Mark(od.linePos.col, od.linePos.col + Math.Max(od.linePos.length,1), 1);
                        odos[ch] = od;
                    }
                    flg = true;
                    continue;
                }
                if (od != null && od.type == enmMMLType.Tempo)
                {
                    ;
                }
            }

            return flg;
        }

        private void TsmiFncHide_Click(object sender, EventArgs e)
        {
            SetFunctionKeyButtonState(false, ToolStripItemDisplayStyle.None);
            TsmiFncHide.Checked = true;
            TsmiFncButtonOnly.Checked = false;
            TsmiFncButtonAndText.Checked = false;
        }

        private void TsmiFncButtonOnly_Click(object sender, EventArgs e)
        {
            SetFunctionKeyButtonState(true, ToolStripItemDisplayStyle.Image);
            TsmiFncHide.Checked = false;
            TsmiFncButtonOnly.Checked = true;
            TsmiFncButtonAndText.Checked = false;

        }

        private void TsmiFncButtonAndText_Click(object sender, EventArgs e)
        {
            SetFunctionKeyButtonState(true, ToolStripItemDisplayStyle.ImageAndText);
            TsmiFncHide.Checked = false;
            TsmiFncButtonOnly.Checked = false;
            TsmiFncButtonAndText.Checked = true;

        }

        private void SetFunctionKeyButtonState(bool visible, ToolStripItemDisplayStyle style)
        {
            tssbOpen.Visible = visible;
            tssbSave.Visible = visible;
            tssbCompile.Visible = visible;
            //tssbTracePlay.Visible = visible;
            tssbStop.Visible = visible;
            tssbSlow.Visible = visible;
            tssbFast.Visible = visible;
            tssbFind.Visible = visible;
            tssbMIDIKbd.Visible = visible;

            tssbOpen.DisplayStyle = style;
            tssbSave.DisplayStyle = style;
            tssbCompile.DisplayStyle = style;
            //tssbTracePlay.DisplayStyle = style;
            tssbStop.DisplayStyle = style;
            tssbSlow.DisplayStyle = style;
            tssbFast.DisplayStyle = style;
            tssbFind.DisplayStyle = style;
            tssbMIDIKbd.DisplayStyle = style;
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {

            if (setting.location.RMain != System.Drawing.Rectangle.Empty)
            {
                this.Location = new Point(setting.location.RMain.X, setting.location.RMain.Y);
                this.Size = new Size(setting.location.RMain.Width, setting.location.RMain.Height);
            }
            this.Opacity = setting.other.Opacity / 100.0;

            if(setting.location.RMixer!=System.Drawing.Rectangle.Empty)
            {
                if (setting.location.ShowMixer)
                {
                    frmMixer = new FrmMixer(this, 1, newParam.mixer);
                    frmMixer.Location = new Point(setting.location.RMixer.X, setting.location.RMixer.Y);
                    frmMixer.Show();
                    frmMixer.isClosed = false;
                    setting.location.ShowMixer = true;
                    TsmiShowMixer.Checked = (frmMixer != null);
                }
            }

            UpdateGwiFileHistory();

            log.ForcedWrite("スクリプトの検索");
            tsmiScript.Enabled = false;
            if (!Directory.Exists(Path.Combine(Common.GetApplicationFolder(), "Script")))
            {
                Directory.CreateDirectory(Path.Combine(Common.GetApplicationFolder(), "Script"));
            }

            tsmiTreeView = new ToolStripMenuItem();
            GetScripts(tsmiScript, tsmiTreeView, Path.Combine(Common.GetApplicationFolder(), "Script"));

        }

        private void GetScripts(ToolStripMenuItem tsmiScript, ToolStripMenuItem tsmiTreeView, string path)
        {
            TreeNode tn = new TreeNode();
            SScript(tn, path);

            DivScripts(tsmiScript, tn , "FROMMENU");
            DivScripts(tsmiTreeView, tn , "FROMTREEVIEWCONTEXTMENU");
            frmFolderTree.extendItem = tsmiTreeView;
        }

        private void SScript(TreeNode parent, string path)
        {
            DirectoryInfo dm = new DirectoryInfo(path);

            try
            {
                foreach (DirectoryInfo ds in dm.GetDirectories())
                {
                    TreeNode tn = new TreeNode(ds.Name);
                    tn.Tag = new Tuple<int, string, string[], string>(-1, "", new string[] { "" }, ds.FullName);
                    SScript(tn, ds.FullName);
                    if (tn.Nodes.Count > 0) parent.Nodes.Add(tn);
                }
                foreach (FileInfo fi in dm.GetFiles())
                {
                    string[] scriptTitles = ScriptInterface.GetScriptTitles(fi.FullName);
                    string[] scriptTypes = ScriptInterface.GetScriptTypes(fi.FullName);
                    string[] scriptSupportFileExt= ScriptInterface.GetScriptSupportFileExt(fi.FullName);
                    for (int i = 0; i < scriptTitles.Length; i++)
                    {
                        TreeNode tn = new TreeNode(scriptTitles[i]);
                        tn.Tag = new Tuple<int, string, string[], string>(i, scriptTypes[i], scriptSupportFileExt[i].Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries), fi.FullName);
                        parent.Nodes.Add(tn);
                    }
                }
            }
            catch { }
        }

        private void DivScripts(ToolStripMenuItem tsmi,TreeNode tn,string target)
        {
            foreach (TreeNode ctn in tn.Nodes)
            {
                ToolStripMenuItem ctsmi = new ToolStripMenuItem(ctn.Text);
                Tuple<int, string, string[], string> tpl = (Tuple<int, string, string[], string>)ctn.Tag;
                if (tpl.Item1 != -1 && tpl.Item2.ToUpper() != target)
                {
                    continue;
                }
                ctsmi.Tag = ctn.Tag;
                tsmi.DropDownItems.Add(ctsmi);
                tsmi.Enabled = true;
                if (ctn.Nodes.Count > 0)
                {
                    ctsmi.MouseUp += tsmiScriptDirectoryItem_Clicked;
                    DivScripts(ctsmi, ctn, target);
                    if (ctsmi.DropDownItems.Count == 0)
                    {
                        tsmi.DropDownItems.Remove(ctsmi);
                    }
                }
                else
                {
                    ctsmi.MouseUp += tsmiScriptFileItem_Clicked;
                }
            }
        }

        private void tsmiScriptDirectoryItem_Clicked(object sender, EventArgs e)
        {
            MouseEventArgs mea = (MouseEventArgs)e;
            if (mea.Button == MouseButtons.Right) return;

            ToolStripMenuItem tsmi = (ToolStripMenuItem)sender;
            Tuple<int, string, string[], string> tpl = (Tuple<int, string, string[], string>)((ToolStripMenuItem)sender).Tag;
            string path = tpl.Item4;
            if (string.IsNullOrEmpty(path) || path[0] != '+') return;
            path = path.Substring(1);
            if (string.IsNullOrEmpty(path)) return;
            tsmi.Tag = path;

        }

        private void tsmiScriptFileItem_Clicked(object sender, EventArgs e)
        {
            MouseEventArgs mea = (MouseEventArgs)e;
            if (mea.Button == MouseButtons.Right) return;

            DockContent dc = (DockContent)GetActiveDockContent();
            Document d = null;
            if (dc != null)
            {
                if (dc.Tag is Document)
                {
                    d = (Document)dc.Tag;
                }
            }

            //if (d == null) return;

            Tuple<int,string, string[],string> tpl = (Tuple<int, string, string[], string>)((ToolStripMenuItem)sender).Tag;
            string fn = tpl.Item4;

            List<string> lstFullPath = new List<string>();
            frmFolderTree.GetCheckTreeNodesFullPath(ref lstFullPath, frmFolderTree.tvFolderTree.Nodes);

            Mml2vgmInfo info = new Mml2vgmInfo();
            info.parent = this;
            info.name = "";
            info.document = d;
            info.fileNamesFull = lstFullPath.ToArray();

            ScriptInterface.run(fn, info, tpl.Item1);
        }

        private ChannelInfo GetCurrentChannelInfo()
        {
            ChannelInfo chi = null;
            Document d = GetActiveDocument();

            if (d == null)
            {
                firstPlay();
                return defaultChannelInfo;
            }

            //int ci = d.editor.azukiControl.CaretIndex;
            //int st = d.editor.azukiControl.GetLineHeadIndexFromCharIndex(ci);
            //string line = d.editor.azukiControl.GetTextInRange(st, ci).TrimStart();
            //if (line == "" || line[0] != '\'')
            //{
            //    return defaultChannelInfo;
            //}

            //演奏中はコンパイルしない
            if (!Audio.sm.IsRunningAtDataMaker())
            {
                Compile(true, false, true, true, false);
                while (Compiling!=0)
                {
                    Thread.Sleep(0);
                    Application.DoEvents();
                }
                Audio.sm.ResetMode(SendMode.MML);
            }


            return chi;
        }

        private Document GetActiveDocument()
        {
            DockContent dc = (DockContent)GetActiveDockContent();
            Document d = null;
            if (dc != null)
            {
                if (dc.Tag is Document)
                {
                    d = (Document)dc.Tag;
                }
            }

            return d;
        }

        private void dmyDisp(string dmy)
        {
            log.Write(dmy);
        }

        private void DpMain_DragOver(object sender, DragEventArgs e)
        {
            //ドラッグされているデータがfileか調べる
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] source = (string[])e.Data.GetData(DataFormats.FileDrop);
                e.Effect = DragDropEffects.Move;
            }
            else
                e.Effect = DragDropEffects.None;

        }

        private void DpMain_DragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            string[] source = (string[])e.Data.GetData(DataFormats.FileDrop);
            ExecFile(source);

        }

        private void TsslCompileError_Click(object sender, EventArgs e)
        {
            frmErrorList.Focus();
            if(frmErrorList.DockState!= DockState.Float)
            {
                frmErrorList.Activate();
            }
        }

        private void TsslCompileWarning_Click(object sender, EventArgs e)
        {
            frmErrorList.Focus();
            if (frmErrorList.DockState != DockState.Float)
            {
                frmErrorList.Activate();
            }
        }

    }
}
