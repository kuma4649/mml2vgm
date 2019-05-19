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

namespace mml2vgmIDE
{
    public partial class FrmMain : Form
    {
        private string appName = "mml2vgmIDE";
        private List<Form> FormBox = new List<Form>();
        private List<Document> DocumentBox = new List<Document>();
        private bool isSuccess = true;
        private string[] args;
        private Mml2vgm mv = null;
        private string title = "";
        private FrmLog frmLog = null;
        private FrmPartCounter frmPartCounter = null;
        private FrmFolderTree frmFolderTree = null;
        private FrmErrorList frmErrorList = null;
        private frmDebug frmDebug = null;
        private bool doPlay = false;
        private bool isTrace = false;
        private bool Compiling = false;
        private bool flgReinit = false;

        public FrmMain()
        {
            InitializeComponent();

            //Init();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            UpdateControl();
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            bool flg = false;
            foreach(Document d in DocumentBox)
            {
                if(d.isNew || d.edit)
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

            ofd.Filter = "gwiファイル(*.gwi)|*.gwi|すべてのファイル(*.*)|*.*";
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
            DockContent dc = null;
            Document d = null;
            if (dpMain.ActiveDocument is DockContent)
            {
                dc = (DockContent)dpMain.ActiveDocument;
                if (dc.Tag is Document)
                {
                    d = (Document)dc.Tag;
                }
            }
            if (d == null) return;

            File.WriteAllText(d.gwiFullPath, d.editor.azukiControl.Text, Encoding.UTF8);
            AddGwiFileHistory(d.gwiFullPath);
            UpdateGwiFileHistory();

            d.edit = false;
            d.editor.azukiControl.ClearHistory();
            if (d.editor.Text.Length>0 && d.editor.Text[d.editor.Text.Length - 1] == '*')
            {
                d.editor.Text = d.editor.Text.Substring(0, d.editor.Text.Length - 1);
            }
            d.isNew = false;
            UpdateControl();
        }

        private void TsmiSaveAs_Click(object sender, EventArgs e)
        {
            DockContent dc = null;
            Document d = null;
            if (dpMain.ActiveDocument is DockContent)
            {
                dc = (DockContent)dpMain.ActiveDocument;
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
                DockContent dc = null;
                Document d = null;
                if (dpMain.ActiveDocument is DockContent)
                {
                    dc = (DockContent)dpMain.ActiveDocument;
                    if (dc.Tag is Document)
                    {
                        d = (Document)dc.Tag;
                    }
                }
                if (d == null) return;

                Compile(false, false);
                while (Compiling) { Application.DoEvents(); }//待ち合わせ

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
                sfd.FileName = Path.GetFileNameWithoutExtension(fn) + (FileInformation.format == enmFormat.VGM ? ".vgm" : ".xgm");
                sfd.InitialDirectory = Path.GetDirectoryName(fn);
                sfd.Filter = "vgmファイル(*.vgm)|*.vgm|すべてのファイル(*.*)|*.*";
                if (FileInformation.format == enmFormat.XGM)
                {
                    sfd.Filter = "xgmファイル(*.xgm)|*.xgm|すべてのファイル(*.*)|*.*";
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
                    fn = Path.GetFileNameWithoutExtension(fn) + (FileInformation.format == enmFormat.VGM ? ".vgm" : ".xgm");
                }

                File.Copy(Path.Combine(Common.GetApplicationDataFolder(true), "temp", Path.GetFileName(sfd.FileName)), fn);
            }
            catch(Exception )
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
            Compile(true,false);
        }

        private void TsmiCompileAndTracePlay_Click(object sender, EventArgs e)
        {
            Compile(true, true);
        }

        private void TsmiCompile_Click(object sender, EventArgs e)
        {
            Compile(false,false);
        }

        private void TsmiUndo_Click(object sender, EventArgs e)
        {
            DockContent dc = null;
            Document d = null;
            if (dpMain.ActiveDocument is DockContent)
            {
                dc = (DockContent)dpMain.ActiveDocument;
                if (dc.Tag is Document)
                {
                    d = (Document)dc.Tag;
                }
            }

            if (d != null) d.editor.azukiControl.Undo();
            UpdateControl();
        }

        private void TsmiRedo_Click(object sender, EventArgs e)
        {
            DockContent dc = null;
            Document d = null;
            if (dpMain.ActiveDocument is DockContent)
            {
                dc = (DockContent)dpMain.ActiveDocument;
                if (dc.Tag is Document)
                {
                    d = (Document)dc.Tag;
                }
            }

            if (d != null) d.editor.azukiControl.Redo();
            UpdateControl();
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

        private void TsmiOption_Click(object sender, EventArgs e)
        {
            FrmSetting frmSetting = new FrmSetting(setting);
            DialogResult res= frmSetting.ShowDialog();
            if(res!= DialogResult.OK)
            {
                return;
            }

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

        private void TssbCompile_ButtonClick(object sender, EventArgs e)
        {
            TsmiCompileAndPlay_Click(null, null);
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

        private void FrmMain_KeyDown(object sender, KeyEventArgs e)
        {
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
                    TsmiCompileAndPlay_Click(null, null);
                    break;
                case Keys.F6:
                    TsmiCompileAndTracePlay_Click(null, null);
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
            }
        }

        private void OpenFile(string fileName)
        {
            Document dc = new Document(setting);
            if (fileName != "") dc.InitOpen(fileName);
            dc.editor.Show(dpMain, DockState.Document);
            dc.editor.main = this;
            dc.editor.parent = dc;

            frmFolderTree.treeView1.Nodes.Clear();
            frmFolderTree.treeView1.Nodes.Add(dc.gwiTree);
            frmFolderTree.basePath = Path.GetDirectoryName(dc.gwiFullPath);

            FormBox.Add(dc.editor);
            DocumentBox.Add(dc);
            AddGwiFileHistory(fileName);
            UpdateGwiFileHistory();
        }

        private void ImportFile(string fileName)
        {
            Document dc = new Document(setting);
            if (fileName != "") dc.InitOpen(fileName);
            dc.editor.Show(dpMain, DockState.Document);
            dc.editor.main = this;
            dc.editor.parent = dc;

            frmFolderTree.treeView1.Nodes.Clear();
            frmFolderTree.treeView1.Nodes.Add(dc.gwiTree);
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
            foreach(string fn in setting.other.GwiFileHistory)
            {
                ToolStripMenuItem tsmi = new ToolStripMenuItem(fn);
                tsmi.Click += Tsmi_Click;
                tsmiGwiFileHistory.DropDownItems.Add(tsmi);
            }
        }

        private void Tsmi_Click(object sender, EventArgs e)
        {
            string fn = ((ToolStripMenuItem)sender).Text;
            OpenFile(fn); 
        }

        string wrkPath = "";

        private void Compile(bool doPlay,bool isTrace)
        {
            IDockContent dc = dpMain.ActiveDocument;
            if (dc == null) return;
            if (!(dc is FrmEditor)) return;

            string text = ((FrmEditor)dc).azukiControl.Text;
            if (!Directory.Exists(Path.Combine(Common.GetApplicationDataFolder(true), "temp")))
            {
                Directory.CreateDirectory(Path.Combine(Common.GetApplicationDataFolder(true), "temp"));
            }
            string tempPath = Path.Combine(Common.GetApplicationDataFolder(true), "temp", Path.GetFileName(((Document)((FrmEditor)dc).Tag).gwiFullPath));
            File.WriteAllText(tempPath, text);
            args = new string[2];
            args[1] = tempPath;
            wrkPath = Path.GetDirectoryName(((Document)((FrmEditor)dc).Tag).gwiFullPath);

            traceInfoSw = false;
            Sgry.Azuki.WinForms.AzukiControl ac = ((FrmEditor)dc).azukiControl;
            ac.ColorScheme.LineNumberBack = Color.FromArgb(setting.ColorScheme.Azuki_LineNumberBack_Normal);
            ac.ColorScheme.LineNumberFore = Color.FromArgb(setting.ColorScheme.Azuki_LineNumberFore_Normal);
            ac.Document.Unmark(0, ac.Text.Length, 1);
            ac.IsReadOnly = false;
            ac.Refresh();

            isSuccess = true;
            this.doPlay = doPlay;
            this.isTrace = isTrace;
            frmPartCounter.dataGridView1.Rows.Clear();
            frmErrorList.dataGridView1.Rows.Clear();

            Thread trdStartCompile = new Thread(new ThreadStart(startCompile));
            trdStartCompile.Start();
            Compiling = true;
        }

        private void startCompile()
        {
            Core.log.Open();
            Core.log.Write("start compile thread");

            Action dmy = updateTitle;
            string stPath = System.Windows.Forms.Application.StartupPath;

            for (int i = 1; i < args.Length; i++)
            {
                string arg = args[i];
                if (!File.Exists(arg))
                {
                    continue;
                }


                title = Path.GetFileName(arg);
                this.Invoke(dmy);

                Core.log.Write(string.Format("  compile at [{0}]", args[i]));

                msgBox.clear();

                string desfn = Path.ChangeExtension(arg, Properties.Resources.ExtensionVGM);
                //if (tsbToVGZ.Checked)
                //{
                    //desfn = Path.ChangeExtension(arg, Properties.Resources.ExtensionVGZ);
                //}

                Core.log.Write("Call mml2vgm core");

                mv = new Mml2vgm(arg, desfn, stPath, Disp, wrkPath);
                if (mv.Start() != 0)
                {
                    isSuccess = false;
                    break;
                }

                Core.log.Write("Return mml2vgm core");
            }

            Core.log.Write("Disp Result");

            dmy = finishedCompile;
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

        private void Disp(string msg)
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

        public void MsgClear()
        {
            if (frmLog == null) return;
            if (frmLog.IsDisposed) return;

            frmLog.tbLog.Clear();
        }

        private void finishedCompile()
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
                foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in mv.desVGM.chips)
                {
                    foreach (ClsChip chip in kvp.Value)
                    {
                        List<partWork> pw = chip.lstPartWork;
                        for (int i = 0; i < pw.Count; i++)
                        {
                            if (pw[i].clockCounter == 0) continue;

                            DataGridViewRow row = new DataGridViewRow();
                            row.Cells.Add(new DataGridViewTextBoxCell());
                            row.Cells[0].Value = pw[i].PartName.Substring(0, 2).Replace(" ", "") + int.Parse(pw[i].PartName.Substring(2, 2)).ToString();
                            row.Cells.Add(new DataGridViewTextBoxCell());
                            row.Cells[1].Value = pw[i].chip.Name.ToUpper();
                            row.Cells.Add(new DataGridViewTextBoxCell());
                            row.Cells[2].Value = pw[i].clockCounter;
                            frmPartCounter.dataGridView1.Rows.Add(row);
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

            //if (mv.desVGM.ym2608[0].pcmDataEasy != null) textBox1.AppendText(string.Format(msg.get("I0115"), mv.desVGM.ym2608[0].pcmDataEasy.Length - 15));
            //if (mv.desVGM.ym2608[1].pcmDataEasy != null) textBox1.AppendText(string.Format(msg.get("I0116"), mv.desVGM.ym2608[1].pcmDataEasy.Length - 15));
            //if (mv.desVGM.ym2610b[0].pcmDataEasyA != null) textBox1.AppendText(string.Format(msg.get("I0117"), mv.desVGM.ym2610b[0].pcmDataEasyA.Length-15));
            //if (mv.desVGM.ym2610b[0].pcmDataEasyB != null) textBox1.AppendText(string.Format(msg.get("I0118"), mv.desVGM.ym2610b[0].pcmDataEasyB.Length-15));
            //if (mv.desVGM.ym2610b[1].pcmDataEasyA != null) textBox1.AppendText(string.Format(msg.get("I0119"), mv.desVGM.ym2610b[1].pcmDataEasyA.Length - 15));
            //if (mv.desVGM.ym2610b[1].pcmDataEasyB != null) textBox1.AppendText(string.Format(msg.get("I0120"), mv.desVGM.ym2610b[1].pcmDataEasyB.Length - 15));
            //if (mv.desVGM.segapcm[0].pcmData != null) textBox1.AppendText(string.Format(" PCM Data size(SEGAPCM)  : {0} byte\r\n", mv.desVGM.segapcm[0].pcmData.Length - 15));
            //if (mv.desVGM.segapcm[1].pcmData != null) textBox1.AppendText(string.Format(" PCM Data size(SEGAPCMSecondary)  : {0} byte\r\n", mv.desVGM.segapcm[1].pcmData.Length - 15));
            //if (mv.desVGM.ym2612[0].pcmDataEasy != null) textBox1.AppendText(string.Format(msg.get("I0121"), mv.desVGM.ym2612[0].pcmDataEasy.Length));
            //if (mv.desVGM.rf5c164[0].pcmDataEasy != null) textBox1.AppendText(string.Format(msg.get("I0122"), mv.desVGM.rf5c164[0].pcmDataEasy.Length-12));
            //if (mv.desVGM.rf5c164[1].pcmDataEasy != null) textBox1.AppendText(string.Format(msg.get("I0123"), mv.desVGM.rf5c164[1].pcmDataEasy.Length-12));
            //if (mv.desVGM.huc6280[0].pcmDataEasy != null) textBox1.AppendText(string.Format(msg.get("I0124"), mv.desVGM.huc6280[0].pcmDataEasy.Length));
            //if (mv.desVGM.huc6280[1].pcmDataEasy != null) textBox1.AppendText(string.Format(msg.get("I0125"), mv.desVGM.huc6280[1].pcmDataEasy.Length));


            frmLog.tbLog.AppendText(msg.get("I0126"));
            //this.toolStrip1.Enabled = true;
            //this.tsslMessage.Text = msg.get("I0106");

            if (isSuccess)
            {
                if (args.Length == 2 && doPlay && msgBox.getErr().Length < 1)
                {
                    try
                    {
                        //Process.Start(Path.ChangeExtension(args[1], (mv.desVGM.info.format == enmFormat.VGM) ? Properties.Resources.ExtensionVGM : Properties.Resources.ExtensionXGM));
                        
                        //ヘッダー情報にダミーコマンド情報分の値を水増しした値をセットしなおす
                        if(mv.desVGM.info.format== enmFormat.VGM)
                        {
                            uint EOFOffset = Common.getLE32(mv.desBuf, 0x04) + (uint)mv.desVGM.dummyCmdCounter;
                            Common.SetLE32(mv.desBuf, 0x04, EOFOffset);

                            uint GD3Offset = Common.getLE32(mv.desBuf, 0x14) + (uint)mv.desVGM.dummyCmdCounter;
                            Common.SetLE32(mv.desBuf, 0x14, GD3Offset);

                            uint LoopOffset = (uint)mv.desVGM.dummyCmdLoopOffset - 0x1c;
                            Common.SetLE32(mv.desBuf, 0x1c, LoopOffset);
                        }
                        else
                        {
                            uint LoopOffserAddress = (uint)mv.desVGM.dummyCmdLoopOffset;
                            uint LoopOffset = (uint)mv.desVGM.dummyCmdLoopOffset;
                            Common.SetLE24(mv.desBuf, (uint)(mv.desVGM.dummyCmdLoopOffsetAddress + 1), LoopOffset);
                        }
                        InitPlayer(
                            mv.desVGM.info.format == enmFormat.VGM ? EnmFileFormat.VGM : EnmFileFormat.XGM,
                            mv.desBuf);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show(msg.get("E0100"), "mml2vgm", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            Compiling = false;
            UpdateControl();
        }

        private void DpMain_ActiveDocumentChanged(object sender, EventArgs e)
        {
            UpdateControl();
        }

        public void UpdateControl()
        {
            DockContent dc = null;
            Document d = null;
            if (dpMain.ActiveDocument is DockContent)
            {
                dc = (DockContent)dpMain.ActiveDocument;
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

                if (frmFolderTree.treeView1.Nodes.Count==0 || frmFolderTree.treeView1.Nodes[0] != d.gwiTree)
                {
                    frmFolderTree.treeView1.Nodes.Clear();
                    frmFolderTree.treeView1.Nodes.Add(d.gwiTree);
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

        private System.Media.SoundPlayer player = null;
        public Setting setting;

        private void ExecFile(string filename)
        {
            if (Path.GetExtension(filename).ToUpper() == ".WAV")
            {
                if (player != null)
                    StopSound();
                player = new System.Media.SoundPlayer(filename);
                player.Play();
                return;
            }
            try
            {
                Process.Start(filename);
            }
            catch
            {
            }
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

        private void JumpDocument(string fn, long ln,bool wantFocus)
        {
            foreach(DockContent dc in dpMain.Documents)
            {
                if (Path.GetFileName(((Document)dc.Tag).gwiFullPath) != fn)
                {
                    continue;
                }
                
                Application.DoEvents();
                Sgry.Azuki.Document d = ((Document)dc.Tag).editor.azukiControl.Document;
                Sgry.Azuki.IView v = ((Document)dc.Tag).editor.azukiControl.View;
                int anc = d.GetLineHeadIndex((int)(ln - 1));
                int caret = d.GetLineHeadIndex((int)ln) - 2;//改行前までを選択する
                int ancM = d.GetLineHeadIndex((int)(ln - 2));
                anc = Math.Max(anc, 0);
                ancM = Math.Max(ancM, 0);
                caret = Math.Max(anc, caret);
                v.ScrollPos= v.GetVirPosFromIndex(ancM);//1行手前を画面の最上部になるようスクロールさせる。

                v.Scroll(1);//scroll barの表示を更新させるため
                v.Scroll(-1);//scroll barの表示を更新させるため

                d.SetSelection(anc, caret);
                if (wantFocus) ((Document)dc.Tag).editor.azukiControl.Focus();
            }
        }

        public void Init()
        {
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

            Audio.SetMMLTraceInfo = SetMMLTraceInfo;

            log.ForcedWrite("デバッグウィンドウ起動");
            log.debug = setting.Debug_DispFrameCounter;
            if (setting.Debug_DispFrameCounter)
            {
                if (frmDebug != null)
                {
                    frmDebug.Close();
                }
                frmDebug = new frmDebug();
                frmDebug.Show();
            }

            this.IsMdiContainer = true;

            FormBox.Add(this);

            frmPartCounter = new FrmPartCounter(setting);
            FormBox.Add(frmPartCounter);

            frmLog = new FrmLog(setting);
            FormBox.Add(frmLog);

            frmFolderTree = new FrmFolderTree(setting);
            FormBox.Add(frmFolderTree);

            frmErrorList = new FrmErrorList(setting);
            FormBox.Add(frmErrorList);

            if (string.IsNullOrEmpty(setting.dockingState))
            {
                frmPartCounter.Show(dpMain, WeifenLuo.WinFormsUI.Docking.DockState.DockLeft);
                frmLog.Show(dpMain, WeifenLuo.WinFormsUI.Docking.DockState.DockBottom);
                frmFolderTree.Show(dpMain, WeifenLuo.WinFormsUI.Docking.DockState.DockLeft);
                frmErrorList.Show(dpMain, WeifenLuo.WinFormsUI.Docking.DockState.DockBottom);
            }
            else
            {
                MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(setting.dockingState));
                dpMain.LoadFromXml(stream, new DeserializeDockContent(GetDockContentFromPersistString));
            }

            frmPartCounter.parentUpdate = UpdateControl;
            frmLog.parentUpdate = UpdateControl;
            frmFolderTree.parentUpdate = UpdateControl;
            frmFolderTree.parentExecFile = ExecFile;
            frmErrorList.parentUpdate = UpdateControl;
            frmErrorList.parentJumpDocument = JumpDocument;

        }

        private IDockContent GetDockContentFromPersistString(string persistString)
        {
            foreach(Form frm in FormBox)
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

            Audio.Stop();
            Audio.Close();

            foreach(var dc in dpMain.Documents)
            {
                ((FrmEditor)dc).azukiControl.Font = new Font(setting.other.TextFontName, setting.other.TextFontSize, setting.other.TextFontStyle);
            }


            this.setting = setting;
            this.setting.Save();

            log.ForcedWrite("設定が変更されたため、再度Audio初期化処理開始");

            Audio.Init(this.setting);

            log.ForcedWrite("Audio初期化処理完了");
            log.debug = setting.Debug_DispFrameCounter;
            if (setting.Debug_DispFrameCounter)
            {
                if (frmDebug != null)
                {
                    frmDebug.Close();
                }
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

            MemoryStream stream=new MemoryStream();
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

            setting.Save();
        }

        public bool InitPlayer(EnmFileFormat format, outDatum[] srcBuf)
        {
            try
            {
                IDockContent dc = dpMain.ActiveDocument;
                if (dc == null) return false;
                if (!(dc is FrmEditor)) return false;
                Sgry.Azuki.WinForms.AzukiControl ac = ((FrmEditor)dc).azukiControl;

                if (isTrace)
                {
                    ac.IsReadOnly = true;
                }

                if (Audio.flgReinit) flgReinit = true;
                if (setting.other.InitAlways) flgReinit = true;
                //Reinit(setting);


                //rowとcolをazuki向けlinePosに変換する
                foreach (outDatum od in srcBuf)
                {
                    if (od.linePos == null) continue;
                    //Console.WriteLine("{0} {1}", od.linePos.row, od.linePos.col);
                    od.linePos.col = ac.GetCharIndexFromLineColumnIndex(od.linePos.row,od.linePos.col);
                    
                }


                if (Audio.isPaused)
                {
                    Audio.Pause();
                }

                Audio.SetVGMBuffer(format, srcBuf);

                for(int i = 0; i < 100; i++)
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

                if (isTrace)
                {
                    ac.ColorScheme.LineNumberBack = Color.FromArgb(setting.ColorScheme.Azuki_LineNumberBack_Trace);
                    ac.ColorScheme.LineNumberFore = Color.FromArgb(setting.ColorScheme.Azuki_LineNumberFore_Trace);
                    ac.Refresh();
                    traceInfoSw = true;
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

        private void playdata()
        {
            try
            {

                if (Audio.isPaused)
                {
                    Audio.Pause();
                }
                Audio.Stop();

                if (!Audio.Play(setting))
                {
                    try
                    {
                        Audio.Stop();
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
            Audio.Stop();
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

        private void SetMMLTraceInfo(PackData pd)
        {
            if (pd == null) return;
            if (pd.od == null) return;
            if (pd.od.linePos == null) return;

            outDatum od = pd.od;

            switch (pd.od.linePos.chip)//.Chip.Device)
            {
                case "YM2151":
                    lock (traceInfoLockObj)
                    {
                        TraceInfo_YM2151[od.linePos.ch + od.linePos.isSecondary * 8] = od;
                    }
                    break;
                case "YM2203":
                    lock (traceInfoLockObj)
                    {
                        TraceInfo_YM2203[od.linePos.ch + od.linePos.isSecondary * 9] = od;
                    }
                    break;
                case "YM2608":
                    lock (traceInfoLockObj)
                    {
                        TraceInfo_YM2608[od.linePos.ch + od.linePos.isSecondary * 19] = od;
                    }
                    break;
                case "YM2610B":
                    lock (traceInfoLockObj)
                    {
                        TraceInfo_YM2610B[od.linePos.ch + od.linePos.isSecondary * 19] = od;
                    }
                    break;
                //case EnmDevice.YM2612:
                case "YM2612":
                case "YM2612X":
                    lock (traceInfoLockObj)
                    {
                        TraceInfo_YM2612[od.linePos.ch + od.linePos.isSecondary * 12] = od;
                    }
                    break;
                //case EnmDevice.SN76489:
                case "SN76489":
                    lock (traceInfoLockObj)
                    {
                        TraceInfo_SN76489[od.linePos.ch + od.linePos.isSecondary * 4] = od;
                    }
                    break;
                case "HuC6280":
                    lock (traceInfoLockObj)
                    {
                        TraceInfo_HuC6280[od.linePos.ch + od.linePos.isSecondary * 6] = od;
                    }
                    break;
                case "RF5C164":
                    lock (traceInfoLockObj)
                    {
                        TraceInfo_RF5C164[od.linePos.ch + od.linePos.isSecondary * 8] = od;
                    }
                    break;
                default:
                    if(pd.od.linePos.chip!="")
                    Console.WriteLine(pd.od.linePos.chip);
                    break;
            }
            //int i, c;
            //ac.GetLineColumnIndexFromCharIndex(od.linePos.col,out i,out c);
            //Console.WriteLine("{0} {1}", i, c);
            //ac.Document.Mark(od.linePos.col, od.linePos.col + od.linePos.length, 1);
        }

        private outDatum[] TraceInfo_YM2151 = new outDatum[16];
        private outDatum[] TraceInfo_YM2151old = new outDatum[16];
        private outDatum[] TraceInfo_YM2203 = new outDatum[18];
        private outDatum[] TraceInfo_YM2203old = new outDatum[18];
        private outDatum[] TraceInfo_YM2608 = new outDatum[38];
        private outDatum[] TraceInfo_YM2608old = new outDatum[38];
        private outDatum[] TraceInfo_YM2610B = new outDatum[38];
        private outDatum[] TraceInfo_YM2610Bold = new outDatum[38];
        private outDatum[] TraceInfo_YM2612 = new outDatum[24];
        private outDatum[] TraceInfo_YM2612old = new outDatum[24];
        private outDatum[] TraceInfo_SN76489 = new outDatum[8];
        private outDatum[] TraceInfo_SN76489old = new outDatum[8];
        private outDatum[] TraceInfo_HuC6280 = new outDatum[6];
        private outDatum[] TraceInfo_HuC6280old = new outDatum[6];
        private outDatum[] TraceInfo_RF5C164 = new outDatum[16];
        private outDatum[] TraceInfo_RF5C164old = new outDatum[16];
        private object traceInfoLockObj = new object();
        private bool traceInfoSw = false;

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!traceInfoSw) return;

            IDockContent dcnt = dpMain.ActiveDocument;
            if (dcnt == null) return;
            if (!(dcnt is FrmEditor)) return;
            FrmEditor fe = ((FrmEditor)dcnt);
            Sgry.Azuki.WinForms.AzukiControl ac = fe.azukiControl;
            bool refresh = false;

            if (!Audio.sm.IsRunningAtDataSender())
            {
                traceInfoSw = false;
                ac.ColorScheme.LineNumberBack = Color.FromArgb(setting.ColorScheme.Azuki_LineNumberBack_Normal);
                ac.ColorScheme.LineNumberFore = Color.FromArgb(setting.ColorScheme.Azuki_LineNumberFore_Normal);
                ac.Document.Unmark(0, ac.Text.Length, 1);
                ac.IsReadOnly = false;
                ac.Refresh();
            }

            try
            {
                for (int ch = 0; ch < 16; ch++)
                {
                    bool ret = MarkUpTraceInfo(TraceInfo_YM2151, TraceInfo_YM2151old, ch, fe, ac);
                    if (ret) refresh = ret;
                }

                for (int ch = 0; ch < 18; ch++)
                {
                    bool ret = MarkUpTraceInfo(TraceInfo_YM2203, TraceInfo_YM2203old, ch, fe, ac);
                    if (ret) refresh = ret;
                }

                for (int ch = 0; ch < 38; ch++)
                {
                    bool ret = MarkUpTraceInfo(TraceInfo_YM2608, TraceInfo_YM2608old, ch, fe, ac);
                    if (ret) refresh = ret;
                }

                for (int ch = 0; ch < 38; ch++)
                {
                    bool ret = MarkUpTraceInfo(TraceInfo_YM2610B, TraceInfo_YM2610Bold, ch, fe, ac);
                    if (ret) refresh = ret;
                }

                for (int ch = 0; ch < 24; ch++)
                {
                    bool ret = MarkUpTraceInfo(TraceInfo_YM2612, TraceInfo_YM2612old, ch, fe, ac);
                    if (ret) refresh = ret;
                }

                for (int ch = 0; ch < 8; ch++)
                {
                    bool ret = MarkUpTraceInfo(TraceInfo_SN76489, TraceInfo_SN76489old, ch, fe, ac);
                    if (ret) refresh = ret;
                }

                for (int ch = 0; ch < 6; ch++)
                {
                    bool ret = MarkUpTraceInfo(TraceInfo_HuC6280, TraceInfo_HuC6280old, ch, fe, ac);
                    if (ret) refresh = ret;
                }

                for (int ch = 0; ch < 16; ch++)
                {
                    bool ret = MarkUpTraceInfo(TraceInfo_RF5C164, TraceInfo_RF5C164old, ch, fe, ac);
                    if (ret) refresh = ret;
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

        private bool MarkUpTraceInfo(outDatum[] ods,outDatum[] odos,int ch,FrmEditor fe, Sgry.Azuki.WinForms.AzukiControl ac)
        {
            outDatum od = ods[ch];
            outDatum odo = odos[ch];
            if (od != null
                && od != odo
                && (od.type == enmMMLType.Note || od.type == enmMMLType.Rest)
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
                            ac.Document.Unmark(odo.linePos.col, odo.linePos.col + odo.linePos.length, 1);
                        }
                        catch
                        {
                            ;//何もしない
                        }
                    }
                    ac.Document.Mark(od.linePos.col, od.linePos.col + od.linePos.length, 1);
                    odos[ch] = ods[ch];
                }
                return true;
            }
            return false;
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

        private void SetFunctionKeyButtonState(bool visible,ToolStripItemDisplayStyle style)
        {
            tssbOpen.Visible = visible;
            tssbSave.Visible = visible;
            tssbCompile.Visible = visible;
            tssbTracePlay.Visible = visible;
            tssbStop.Visible = visible;
            tssbSlow.Visible = visible;
            tssbFast.Visible = visible;

            tssbOpen.DisplayStyle = style;
            tssbSave.DisplayStyle = style;
            tssbCompile.DisplayStyle = style;
            tssbTracePlay.DisplayStyle = style;
            tssbStop.DisplayStyle = style;
            tssbSlow.DisplayStyle = style;
            tssbFast.DisplayStyle = style;
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {

            if (setting.location.RMain != System.Drawing.Rectangle.Empty)
            {
                this.Location = new Point(setting.location.RMain.X, setting.location.RMain.Y);
                this.Size = new Size(setting.location.RMain.Width, setting.location.RMain.Height);
            }

            UpdateGwiFileHistory();

            log.ForcedWrite("スクリプトの検索");
            tsmiScript.Enabled = false;
            if (!Directory.Exists(Path.Combine(Common.GetApplicationFolder(), "Script")))
            {
                Directory.CreateDirectory(Path.Combine(Common.GetApplicationFolder(), "Script"));
            }
            SearchScript(tsmiScript, Path.Combine(Common.GetApplicationFolder(), "Script"));

        }

        private void SearchScript(ToolStripMenuItem parent, string path)
        {
            DirectoryInfo dm = new DirectoryInfo(path);

            try
            {
                foreach (DirectoryInfo ds in dm.GetDirectories())
                {
                    ToolStripMenuItem tsmi = new ToolStripMenuItem(ds.Name);
                    tsmi.Tag = "+" + ds.FullName;
                    parent.DropDownItems.Add(tsmi);
                    tsmi.Click += tsmiScriptDirectoryItem_Clicked;
                    parent.Enabled = true;
                }
                foreach (FileInfo fi in dm.GetFiles())
                {
                    string scriptTitle = ScriptInterface.GetScriptTitle(fi.FullName);
                    ToolStripMenuItem tsmi = new ToolStripMenuItem(scriptTitle);
                    tsmi.Tag = fi.FullName;
                    parent.DropDownItems.Add(tsmi);
                    tsmi.Click += tsmiScriptFileItem_Clicked;
                    parent.Enabled = true;
                }
            }
            catch { }
        }

        private void tsmiScriptDirectoryItem_Clicked(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmi = (ToolStripMenuItem)sender;
            string path = (string)tsmi.Tag;
            if (string.IsNullOrEmpty(path) || path[0] != '+') return;
            path = path.Substring(1);
            if (string.IsNullOrEmpty(path)) return;
            tsmi.Tag = path;

            SearchScript(tsmi, path);
        }

        private void tsmiScriptFileItem_Clicked(object sender, EventArgs e)
        {
            DockContent dc = null;
            Document d = null;
            if (dpMain.ActiveDocument is DockContent)
            {
                dc = (DockContent)dpMain.ActiveDocument;
                if (dc.Tag is Document)
                {
                    d = (Document)dc.Tag;
                }
            }
            //if (d == null) return;

            string fn = (string)((ToolStripMenuItem)sender).Tag;

            Mml2vgmInfo info = new Mml2vgmInfo();
            info.parent = this;
            info.name = "";
            info.document = d;
            ScriptInterface.run(fn, info);
        }

    }
}
