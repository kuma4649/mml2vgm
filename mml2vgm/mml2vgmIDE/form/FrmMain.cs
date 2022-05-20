using Core;
using mml2vgmIDE.D88N88;
using mml2vgmIDE.form;
using musicDriverInterface;
using Sgry.Azuki.WinForms;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace mml2vgmIDE
{
    public partial class FrmMain : Form
    {
        private string appName = "mml2vgmIDE";
        private List<Form> FormBox = new List<Form>();
        private List<Document> DocumentBox = new List<Document>();
        public bool isSuccess = true;
        private string[] args;
        private bool exportWav;
        private bool exportMid;
        private bool exportVGM;
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
        private FrmSien frmSien = null;
        private bool doPlay = false;
        public bool isTrace = false;
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
        private bool alt = false;
        private MIDIKbd midikbd;

        public bool SendProcessCmdKey(ref Message msg, Keys keyData)
        {
            return ProcessCmdKey(ref msg, keyData);
        }

        //private bool beforeAlt = false;
        private ChannelInfo defaultChannelInfo = null;
        private mucomManager mucom = null;
        private PMDManager pmdmng = null;
        private MoonDriverManager mdmng = null;
        private musicDriverInterface.MmlDatum[] mubData = null;
        private musicDriverInterface.MmlDatum[] mData = null;
        private string m98ResultMucString = null;
        private CompileManager compileManager = null;

        private object traceInfoLockObj = new object();
        private bool traceInfoSw = false;
        private string wrkPath = "";
        private string[] activeMMLTextLines = null;
        private object compileTargetDocument = null;
        private System.Media.SoundPlayer player = null;
        public Setting setting;
        private ToolStripMenuItem tsmiTreeView = null;
        public IDockContent activeDocument;
        private int m98Count = 0;
        private musicDriverInterface.CompilerInfo mubCompilerInfo = null;
        private bool jumpSoloModeSw = false;




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
            CheckAndLoadPMDDotNET(System.Windows.Forms.Application.StartupPath, Disp);
            CheckAndLoadMoonDriverDotNET(System.Windows.Forms.Application.StartupPath, Disp);

            Audio.OPNARhythmSample = LoadRhythmSample(System.Windows.Forms.Application.StartupPath, Disp);

            compileManager = new CompileManager(Disp, mucom, pmdmng, mdmng);

            string[] args = Environment.GetCommandLineArgs();
            //foreach (string ag in args) MessageBox.Show(ag);
            string startFn = "";
            if (args.Length > 1
                && File.Exists(args[1])
                && (Path.GetExtension(args[1]).ToLower() == ".gwi" || Path.GetExtension(args[1]).ToLower() == ".muc"))
            {
                startFn = args[1];
            }

            OpenLatestFile(startFn);

            midikbd = new MIDIKbd(this.setting, newParam.mIDIKbd);
            midikbd.StartMIDIInMonitoring();
        }

        private List<byte[]> LoadRhythmSample(string startupPath, Action<string> disp)
        {
            string[] rhythmname = new string[6]
            {
                "BD", "SD", "TOP", "HH", "TOM", "RIM",
            };

            int i;
            List<byte[]> dat = new List<byte[]>();

            for (i = 0; i < 6; i++)
            {
                dat.Add(null);

                try
                {
                    string fn = Path.Combine(startupPath, string.Format("2608_{0}.WAV", rhythmname[i]));
                    string rymFn = Path.Combine(startupPath, "2608_RYM.WAV");
                    byte[] buf = null;

                    if (File.Exists(fn))
                    {
                        buf = File.ReadAllBytes(fn);
                    }

                    //リムショットのファイル名は2パターンあるので更に読み込みに挑戦する
                    if (buf == null)
                    {
                        if (i == 5)
                        {
                            if (File.Exists(rymFn))
                            {
                                buf = File.ReadAllBytes(rymFn);
                            }
                        }
                    }

                    //読み込みができなかった場合は次のファイルの読み込みに挑戦する
                    if (buf == null)
                    {
                        //errMsg += string.Format(
                        //    "Failed to load 2608_{0}.wav ... \r\n",
                        //    rhythmname[i]);
                        continue;
                    }

                    dat[i] = buf;
                }
                catch (Exception e)
                {
                    //errMsg += string.Format(
                    //    "ExceptionMessage:{0} stacktrace:{1} \r\n",
                    //    e.Message, e.StackTrace);
                }
            }

            return dat;
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
                mucom = new mucomManager(
                    Path.Combine(startupPath, "mucomDotNETCompiler.dll"),
                    Path.Combine(startupPath, "mucomDotNETDriver.dll"),
                    Path.Combine(startupPath, "M98DotNETcore.dll"),
                    disp, setting);
                Audio.mucomManager = mucom;

                disp("mucomDotNETを読み込みました");
            }
            catch (Exception ex)
            {
                disp("mucomDotNETの読み込みに失敗しました");
                disp(string.Format("message:\r\n{0}\r\nstacktrace:\r\n{1}\r\n", ex.Message, ex.StackTrace));
                disp(string.Format("startuppath:\r\n{0}\r\n", startupPath));
                mucom = null;
                Audio.mucomManager = null;
            }
        }

        private void CheckAndLoadPMDDotNET(string startupPath, Action<string> disp)
        {
            string mes = "";
            if (!File.Exists(Path.Combine(startupPath, "PMDDotNETCommon.dll"))) mes += "'PMDDotNETCommon.dll' not found.\r\n";
            if (!File.Exists(Path.Combine(startupPath, "PMDDotNETCompiler.dll"))) mes += "'PMDDotNETCompiler.dll' not found.\r\n";
            if (!File.Exists(Path.Combine(startupPath, "PMDDotNETDriver.dll"))) mes += "'PMDDotNETDriver.dll' not found.\r\n";
            if (!File.Exists(Path.Combine(startupPath, "lang\\PMDDotNETmessage.ja-JP.txt"))) mes += "'lang\\PMDDotNETmessage.ja-JP.txt' not found.\r\n";
            if (!File.Exists(Path.Combine(startupPath, "lang\\PMDDotNETmessage.txt"))) mes += "'lang\\PMDDotNETmessage.txt' not found.\r\n";
            if (mes != "")
            {
                disp(mes);
                return;
            }

            try
            {
                pmdmng = new PMDManager(
                    Path.Combine(startupPath, "PMDDotNETCompiler.dll"),
                    Path.Combine(startupPath, "PMDDotNETDriver.dll"),
                    disp, setting);
                Audio.PMDManager = pmdmng;

                disp("PMDDotNETを読み込みました");
            }
            catch (Exception ex)
            {
                disp("PMDDotNETの読み込みに失敗しました");
                disp(string.Format("message:\r\n{0}\r\nstacktrace:\r\n{1}\r\n", ex.Message, ex.StackTrace));
                disp(string.Format("startuppath:\r\n{0}\r\n", startupPath));
                pmdmng = null;
                Audio.PMDManager = null;
            }
        }

        private void CheckAndLoadMoonDriverDotNET(string startupPath, Action<string> disp)
        {
            string mes = "";
            if (!File.Exists(Path.Combine(startupPath, "MoonDriverDotNETCommon.dll"))) mes += "'MoonDriverDotNETCommon.dll' not found.\r\n";
            if (!File.Exists(Path.Combine(startupPath, "MoonDriverDotNETCompiler.dll"))) mes += "'MoonDriverDotNETCompiler.dll' not found.\r\n";
            if (!File.Exists(Path.Combine(startupPath, "MoonDriverDotNETDriver.dll"))) mes += "'MoonDriverDotNETDriver.dll' not found.\r\n";
            if (!File.Exists(Path.Combine(startupPath, "lang\\MoonDriverDotNETmessage.ja-JP.txt"))) mes += "'lang\\MoonDriverDotNETmessage.ja-JP.txt' not found.\r\n";
            if (!File.Exists(Path.Combine(startupPath, "lang\\MoonDriverDotNETmessage.txt"))) mes += "'lang\\MoonDriverDotNETmessage.txt' not found.\r\n";
            if (mes != "")
            {
                disp(mes);
                return;
            }

            try
            {
                mdmng = new MoonDriverManager(
                    Path.Combine(startupPath, "MoonDriverDotNETCompiler.dll"),
                    Path.Combine(startupPath, "MoonDriverDotNETDriver.dll"),
                    disp, setting);
                Audio.MoonDriverManager = mdmng;

                disp("MoonDriverDotNETを読み込みました");
            }
            catch (Exception ex)
            {
                disp("MoonDriverDotNETの読み込みに失敗しました");
                disp(string.Format("message:\r\n{0}\r\nstacktrace:\r\n{1}\r\n", ex.Message, ex.StackTrace));
                disp(string.Format("startuppath:\r\n{0}\r\n", startupPath));
                mdmng = null;
                Audio.MoonDriverManager = null;
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
                DialogResult res = MessageBox.Show("未保存のファイルがあります。このまま終了しますか？"
                    , "終了確認"
                    , MessageBoxButtons.YesNo
                    , MessageBoxIcon.Question
                    , MessageBoxDefaultButton.Button2);
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

        private void tsmiNewGwi_Click(object sender, EventArgs e)
        {
            OpenFile("Template.gwi");

        }

        private void tsmiNewMuc_Click(object sender, EventArgs e)
        {
            OpenFile("Template.muc");

        }

        private void tsmiNewMml_Click(object sender, EventArgs e)
        {
            OpenFile("Template.mml");

        }

        public void TsmiFileOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = "全てのサポートファイル(*.gwi;*.muc;*.mml;*.mdl)|*.gwi;*.muc;*.mml;*.mdl|gwiファイル(*.gwi)|*.gwi|mucファイル(*.muc)|*.muc|mmlファイル(*.mml)|*.mml|mdlファイル(*.mdl)|*.mdl|すべてのファイル(*.*)|*.*";
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
                if (d.srcFileFormat == EnmMmlFileFormat.GWI)
                    File.WriteAllText(d.gwiFullPath, d.editor.azukiControl.Text, Encoding.UTF8);
                else if (d.srcFileFormat == EnmMmlFileFormat.MUC)
                    File.WriteAllText(d.gwiFullPath, d.editor.azukiControl.Text, Encoding.GetEncoding(932));
                else if (d.srcFileFormat == EnmMmlFileFormat.MML)
                    File.WriteAllText(d.gwiFullPath, d.editor.azukiControl.Text, Encoding.GetEncoding(932));
                else if (d.srcFileFormat == EnmMmlFileFormat.MDL)
                    File.WriteAllText(d.gwiFullPath, d.editor.azukiControl.Text, Encoding.GetEncoding(932));

                d.parentFullPath = "";
                AddGwiFileHistory(d.gwiFullPath);
                UpdateGwiFileHistory();

                d.edit = false;

                if (setting.other.ClearHistory)
                    d.editor.azukiControl.ClearHistory();

                if (d.editor.Text.Length > 0 && d.editor.Text[d.editor.Text.Length - 1] == '*')
                {
                    d.editor.Text = d.editor.Text.Substring(0, d.editor.Text.Length - 1);
                }
                d.isNew = false;
                UpdateControl();
            }
            catch (System.IO.IOException ioe)
            {
                MessageBox.Show(string.Format("Occured exception.\r\nMessage:\r\n{0}", ioe.Message), "Saving failed.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

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
            string path1 = System.IO.Path.GetDirectoryName(fn);
            path1 = string.IsNullOrEmpty(path1) ? fn : path1;
            sfd.InitialDirectory = path1;
            sfd.Filter = "gwiファイル(*.gwi)|*.gwi|mucファイル(*.muc)|*.muc|mmlファイル(*.mml)|*.mml|mdlファイル(*.mdl)|*.mdl|すべてのファイル(*.*)|*.*";
            sfd.Title = "名前を付けて保存";
            int n = (int)Common.GetEnmMmlFileFormat(Path.GetExtension(sfd.FileName));
            if (n < 1 || n > 4) n = 5;
            sfd.FilterIndex = n;
            sfd.RestoreDirectory = true;
            if (sfd.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            fn = Path.Combine(path1, sfd.FileName);
            d.editor.Text = Path.GetFileName(sfd.FileName);
            d.gwiFullPath = fn;
            d.parentFullPath = "";
            TsmiSaveFile_Click(null, null);
        }

        private void TsmiImport_Click(object sender, EventArgs e)
        {
            //OpenFileDialog ofd = new OpenFileDialog();

            //ofd.Filter = "btmファイル(*.btm)|*.btm|すべてのファイル(*.*)|*.*";
            //ofd.Title = "ファイルを開く";
            //ofd.RestoreDirectory = true;

            //if (ofd.ShowDialog() != DialogResult.OK)
            //{
            //    return;
            //}

            //ImportFile(ofd.FileName);
        }

        private bool ExportVgmXgmZgm(Document d, ref string fn)
        {
            Compile(false, false, false, false, true);
            while (Compiling != 0) { Application.DoEvents(); }//待ち合わせ

            if (msgBox.getErr().Length > 0)
            {
                MessageBox.Show("コンパイル時にエラーが発生しました。エクスポート処理を中断します。",
                    "エラー発生",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return false;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            fn = d.gwiFullPath;
            if (fn.Length > 0 && fn[fn.Length - 1] == '*')
            {
                fn = fn.Substring(0, fn.Length - 1);
            }
            sfd.FileName = Path.GetFileNameWithoutExtension(fn) + (FileInformation.format == enmFormat.VGM ? ".vgm" : (FileInformation.format == enmFormat.XGM ? ".xgm" : ".zgm"));
            string path1 = System.IO.Path.GetDirectoryName(fn);
            path1 = string.IsNullOrEmpty(path1) ? fn : path1;
            sfd.InitialDirectory = path1;
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
                return false;
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

            return true;
        }

        private bool ExportM(Document d, ref string fn)
        {
            Compile(false, false, false, false, true);
            while (Compiling != 0) { Application.DoEvents(); }//待ち合わせ

            if (msgBox.getErr().Length > 0)
            {
                MessageBox.Show("コンパイル時にエラーが発生しました。エクスポート処理を中断します。",
                    "エラー発生",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return false;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            fn = d.gwiFullPath;
            if (fn.Length > 0 && fn[fn.Length - 1] == '*')
            {
                fn = fn.Substring(0, fn.Length - 1);
            }

            GD3Tag gd3 = pmdmng.GetGD3Tag();
            string gd3FileName = "";
            if (gd3.dicItem.ContainsKey(enmTag.SongObjFilename))
            {
                gd3FileName = gd3.dicItem[enmTag.SongObjFilename][0];
                if (Path.GetFileNameWithoutExtension(gd3FileName) == "")
                {
                    gd3FileName = Path.GetFileNameWithoutExtension(fn) + gd3FileName;
                }
            }
            if (gd3FileName == "")
            {
                gd3FileName = Path.GetFileNameWithoutExtension(fn) + ".m";
            }

            sfd.FileName = gd3FileName;
            string path1 = System.IO.Path.GetDirectoryName(fn);
            path1 = string.IsNullOrEmpty(path1) ? fn : path1;
            sfd.InitialDirectory = path1;
            sfd.Filter = "mファイル(*.m;*.m2;*.mz)|*.m;*.m2;*.mz|すべてのファイル(*.*)|*.*";
            sfd.Title = "エクスポート";
            sfd.RestoreDirectory = true;
            if (sfd.ShowDialog() != DialogResult.OK)
            {
                return false;
            }
            fn = sfd.FileName;

            if (Path.GetExtension(fn) == "")
            {
                fn = Path.GetFileNameWithoutExtension(fn) + ".m";
            }

            List<byte> buf = new List<byte>();
            foreach (MmlDatum md in mData) buf.Add((byte)md.dat);

            File.WriteAllBytes(fn, buf.ToArray());
            return true;
        }

        private bool ExportMub(Document d,ref string fn)
        {
            Compile(false, false, false, false, true);
            while (Compiling != 0) { Application.DoEvents(); }//待ち合わせ

            if (msgBox.getErr().Length > 0)
            {
                MessageBox.Show("コンパイル時にエラーが発生しました。エクスポート処理を中断します。",
                    "エラー発生",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return false;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            fn = d.gwiFullPath;
            if (fn.Length > 0 && fn[fn.Length - 1] == '*')
            {
                fn = fn.Substring(0, fn.Length - 1);
            }
            fn = Path.Combine(Path.GetDirectoryName(fn), Path.GetFileNameWithoutExtension(fn) + ".mub");

            sfd.FileName = fn;
            string path1 = System.IO.Path.GetDirectoryName(fn);
            path1 = string.IsNullOrEmpty(path1) ? fn : path1;
            sfd.InitialDirectory = path1;
            sfd.Filter = "mubファイル(*.mub)|*.mub|すべてのファイル(*.*)|*.*";
            sfd.Title = "エクスポート";
            sfd.RestoreDirectory = true;
            if (sfd.ShowDialog() != DialogResult.OK)
            {
                return false;
            }
            fn = sfd.FileName;

            if (Path.GetExtension(fn) == "")
            {
                fn = Path.GetFileNameWithoutExtension(fn) + ".mub";
            }

            List<byte> buf = new List<byte>();
            foreach (MmlDatum md in mubData) buf.Add(md != null ? (byte)md.dat : (byte)0);

            File.WriteAllBytes(fn, buf.ToArray());

            return true;
        }

        private bool ExportMdr(Document d, ref string fn)
        {
            Compile(false, false, false, false, true);
            while (Compiling != 0) { Application.DoEvents(); }//待ち合わせ

            if (msgBox.getErr().Length > 0)
            {
                MessageBox.Show("コンパイル時にエラーが発生しました。エクスポート処理を中断します。",
                    "エラー発生",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return false;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            fn = d.gwiFullPath;
            if (fn.Length > 0 && fn[fn.Length - 1] == '*')
            {
                fn = fn.Substring(0, fn.Length - 1);
            }
            fn = Path.Combine(Path.GetDirectoryName(fn), Path.GetFileNameWithoutExtension(fn) + ".mdr");

            sfd.FileName = fn;
            string path1 = System.IO.Path.GetDirectoryName(fn);
            path1 = string.IsNullOrEmpty(path1) ? fn : path1;
            sfd.InitialDirectory = path1;
            sfd.Filter = "mdrファイル(*.mdr)|*.mdr|すべてのファイル(*.*)|*.*";
            sfd.Title = "エクスポート";
            sfd.RestoreDirectory = true;
            if (sfd.ShowDialog() != DialogResult.OK)
            {
                return false;
            }
            fn = sfd.FileName;

            if (Path.GetExtension(fn) == "")
            {
                fn = Path.GetFileNameWithoutExtension(fn) + ".mdr";
            }

            List<byte> buf = new List<byte>();
            foreach (MmlDatum md in mData) buf.Add(md != null ? (byte)md.dat : (byte)0);

            File.WriteAllBytes(fn, buf.ToArray());

            return true;
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

        private void TsmiReplace_Click(object sender, EventArgs e)
        {
            Document d = GetActiveDocument();
            if (d != null) d.editor.ActionReplace(null);
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

        private void TsmiShowSien_Click(object sender, EventArgs e)
        {
            if (frmSien.IsHidden) frmSien.Show();
            else frmSien.Hide();

            TsmiShowSien.Checked = !frmSien.IsHidden;
        }

        private void TsmiShowMixer_Click(object sender, EventArgs e)
        {
            if (frmMixer == null)
            {
                frmMixer = new FrmMixer(this, setting.other.Zoom, newParam.mixer);
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
            //if (frmMIDIKbd == null)
            //{
            //if (!Audio.ReadyOK())
            //{
            //    firstPlay();
            //}
            //else
            //{
            //    if (Audio.sm.Mode == SendMode.none)
            //    {
            //        Audio.sm.RequestStart(SendMode.RealTime, true, true);
            //    }
            //    else
            //    {
            //        Audio.sm.SetMode(SendMode.RealTime);
            //    }
            //}

            //frmMIDIKbd = new FrmMIDIKbd(this, 2, newParam.mIDIKbd);
            //frmMIDIKbd.KeyDown += FrmMain_KeyDown;
            //frmMIDIKbd.KeyUp += FrmMain_KeyUp;
            //frmMIDIKbd.Show();
            //ChannelInfo ci = GetCurrentChannelInfo();

            //}
            //else
            //{
            //frmMIDIKbd.Close();
            //frmMIDIKbd = null;
            //}

            //TsmiShowMIDIKbd.Checked = frmMIDIKbd != null;
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
            while (Compiling != 0)
            {
                Thread.Sleep(0);
                Application.DoEvents();
            }
            Audio.sm.ResetMode(SendMode.MML);
        }

        private void TsmiOption_Click(object sender, EventArgs e)
        {
            stop();
            for (int i = 0; i < 10; i++) { Application.DoEvents(); Thread.Sleep(1); }
            Audio.Close();

            FrmSetting frmSetting = new FrmSetting(setting);
            DialogResult res = frmSetting.ShowDialog();
            //if (res != DialogResult.OK)
            //{
            //    return;
            //}

            //stop();
            flgReinit = true;
            Reinit(frmSetting.setting);
        }

        private void TsmiTutorial_Click(object sender, EventArgs e)
        {
            try
            {
                while (!File.Exists(setting.other.Tutorial) || (Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                {
                    OpenFileDialog ofd = new OpenFileDialog();
                    ofd.Filter = "テキストファイル(*.txt;*.doc;*.man;*.pdf)|*.txt;*.doc;*.man;*.pdf|すべてのファイル(*.*)|*.*";
                    ofd.Title = "MMLのリファレンスマニュアルを選択";
                    ofd.RestoreDirectory = true;

                    if (ofd.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }

                    setting.other.Tutorial = ofd.FileName;
                }

                Process.Start(setting.other.Tutorial);
            }
            catch
            {
                MessageBox.Show("Failed to open the file.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TsmiReference_Click(object sender, EventArgs e)
        {
            try
            {
                while (!File.Exists(setting.other.CommandManual) || (Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                {
                    OpenFileDialog ofd = new OpenFileDialog();
                    ofd.Filter = "テキストファイル(*.txt;*.doc;*.man;*.pdf)|*.txt;*.doc;*.man;*.pdf|すべてのファイル(*.*)|*.*";
                    ofd.Title = "MMLのリファレンスマニュアルを選択";
                    ofd.RestoreDirectory = true;

                    if (ofd.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }

                    setting.other.CommandManual = ofd.FileName;
                }

                Process.Start(setting.other.CommandManual);
            }
            catch
            {
                MessageBox.Show("Failed to open the file.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TsmiReferenceMucom_Click(object sender, EventArgs e)
        {
            try
            {
                while (!File.Exists(setting.other.CommandManualMucom) || (Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                {
                    OpenFileDialog ofd = new OpenFileDialog();
                    ofd.Filter = "テキストファイル(*.txt;*.doc;*.man;*.pdf)|*.txt;*.doc;*.man;*.pdf|すべてのファイル(*.*)|*.*";
                    ofd.Title = "MML(Mucom向け)のリファレンスマニュアルを選択";
                    ofd.RestoreDirectory = true;

                    if (ofd.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }

                    setting.other.CommandManualMucom = ofd.FileName;
                }

                Process.Start(setting.other.CommandManualMucom);
            }
            catch
            {
                MessageBox.Show("Failed to open the file.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TsmiReferencePMD_Click(object sender, EventArgs e)
        {
            try
            {
                while (!File.Exists(setting.other.CommandManualPMD) || (Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                {
                    OpenFileDialog ofd = new OpenFileDialog();
                    ofd.Filter = "テキストファイル(*.txt;*.doc;*.man;*.pdf)|*.txt;*.doc;*.man;*.pdf|すべてのファイル(*.*)|*.*";
                    ofd.Title = "MML(PMD向け)のリファレンスマニュアルを選択";
                    ofd.RestoreDirectory = true;

                    if (ofd.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }

                    setting.other.CommandManualPMD = ofd.FileName;
                }

                Process.Start(setting.other.CommandManualPMD);
            }
            catch
            {
                MessageBox.Show("Failed to open the file.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TsmiReferenceM98_Click(object sender, EventArgs e)
        {
            try
            {
                while (!File.Exists(setting.other.CommandManualM98) || (Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                {
                    OpenFileDialog ofd = new OpenFileDialog();
                    ofd.Filter = "テキストファイル(*.txt;*.doc;*.man;*.pdf)|*.txt;*.doc;*.man;*.pdf|すべてのファイル(*.*)|*.*";
                    ofd.Title = "MML(Mucom向け)のリファレンスマニュアルを選択";
                    ofd.RestoreDirectory = true;

                    if (ofd.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }

                    setting.other.CommandManualM98 = ofd.FileName;
                }

                Process.Start(setting.other.CommandManualM98);
            }
            catch
            {
                MessageBox.Show("Failed to open the file.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

        private void TssbPlay_ButtonClick(object sender, EventArgs e)
        {
            Compile(true, ctrl, shift, false, false);
            //TsmiCompileAndPlay_Click(null, null);
        }

        private void tssbM98_ButtonClick(object sender, EventArgs e)
        {
            M98Preprocess();
        }

        private void tssbPause_ButtonClick(object sender, EventArgs e)
        {
            pause();
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

        private void tssbExpAndMdp_ButtonClick(object sender, EventArgs e)
        {
            tsmiExport_toDriverFormatAndPlay_Click(null, null);
        }

        //private void TssbPlay_ButtonClick(object sender, EventArgs e)
        //{
        //TsmiCompileAndTracePlay_Click(null, null);
        //}

        private void TssbSkipPlay_ButtonClick(object sender, EventArgs e)
        {
            TsmiCompileAndSkipPlay_Click(null, null);
        }

        private void TssbMIDIKbd_ButtonClick(object sender, EventArgs e)
        {
            TsmiShowMIDIKbd_Click(null, null);
        }

        private void tssbSien_ButtonClick(object sender, EventArgs e)
        {
            //sien();
        }

        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, System.Windows.Forms.Keys keyData)
        {
            //log.Write(string.Format("2動作未定義のキー：{0}", keyData));

            ctrl = (keyData & Keys.Control) == Keys.Control;
            shift = (keyData & Keys.Shift) == Keys.Shift;
            alt = (keyData & Keys.Alt) == Keys.Alt;

            if (keyData == (System.Windows.Forms.Keys.OemMinus | System.Windows.Forms.Keys.Alt))
            {
                frmPartCounter.ClickMUTE(10);
                return true;
            }

            int pass = 0;
            for (int i = 0; i < scriptShortCutKey.Count; i++)
            {
                Tuple<Setting.ShortCutKey.ShortCutKeyInfo, Tuple<int, string, string[], string, string>> ssck = scriptShortCutKey[i];
                Setting.ShortCutKey.ShortCutKeyInfo info = ssck.Item1;

                Keys k = keyData;
                if (info.step == 0)
                {
                    if (info.shift != ((k & Keys.Shift) == Keys.Shift)
                    || info.ctrl != ((k & Keys.Control) == Keys.Control)
                    || info.alt != ((k & Keys.Alt) == Keys.Alt))
                        continue;
                }
                else
                {
                    if (info.shift2 != ((k & Keys.Shift) == Keys.Shift)
                    || info.ctrl2 != ((k & Keys.Control) == Keys.Control)
                    || info.alt2 != ((k & Keys.Alt) == Keys.Alt))
                    {
                        info.step = 0;
                        continue;
                    }
                }

                k = k & ~Keys.Shift;
                k = k & ~Keys.Control;
                k = k & ~Keys.Alt;

                if (k == Keys.ControlKey) return base.ProcessCmdKey(ref msg, keyData);

                if (info.step == 0)
                {
                    if (info.key == k.ToString())
                    {
                        info.step++;
                        pass++;
                    }
                }
                else
                {
                    if (info.key2 == k.ToString())
                    {
                        //ショートカット確定!

                        //全てのショートカットのステップをリセットする
                        for (int j = 0; j < scriptShortCutKey.Count; j++)
                        {
                            Setting.ShortCutKey.ShortCutKeyInfo inf = ((Tuple<Setting.ShortCutKey.ShortCutKeyInfo, Tuple<int, string, string[], string, string>>)scriptShortCutKey[j]).Item1;
                            inf.step = 0;
                        }

                        Mml2vgmInfo minfo = new Mml2vgmInfo();
                        minfo.parent = this;
                        minfo.name = "";
                        minfo.document = GetActiveDocument();
                        minfo.fileNamesFull = null;

                        string cd = Directory.GetCurrentDirectory();
                        ScriptInterface.run(ssck.Item2.Item4, minfo, ssck.Item2.Item1);
                        Directory.SetCurrentDirectory(cd);

                        return true;
                    }
                    info.step = 0;
                }
            }
            if (pass != 0) return true;

            for (int i = 0; i < setting.shortCutKey.Info.Length; i++)
            {
                if (setting.shortCutKey.Info[i].shift != ((keyData & Keys.Shift) == Keys.Shift)) continue;
                if (setting.shortCutKey.Info[i].ctrl != ((keyData & Keys.Control) == Keys.Control)) continue;
                if (setting.shortCutKey.Info[i].alt != ((keyData & Keys.Alt) == Keys.Alt)) continue;
                Keys k = keyData & ~Keys.Shift;
                k = k & ~Keys.Control;
                k = k & ~Keys.Alt;
                if (setting.shortCutKey.Info[i].key != k.ToString()) continue;

                Document doc;

                switch (setting.shortCutKey.Info[i].number - (setting.shortCutKey.Info[i].number % 10))
                {
                    case (int)Setting.ShortCutKey.enmContent.FileOpen:
                        TsmiFileOpen_Click(null, null);
                        return true;
                    case (int)Setting.ShortCutKey.enmContent.FileSave:
                        TsmiSaveFile_Click(null, null);
                        return true;
                    case (int)Setting.ShortCutKey.enmContent.Search:
                        doc = GetActiveDocument();
                        if (doc != null) doc.editor.ActionFind(null);
                        return true;
                    case (int)Setting.ShortCutKey.enmContent.SearchNext:
                        doc = GetActiveDocument();
                        if (doc != null) doc.editor.ActionFindNext(null);
                        return true;
                    case (int)Setting.ShortCutKey.enmContent.SearchPrev:
                        doc = GetActiveDocument();
                        if (doc != null) doc.editor.ActionFindPrevious(null);
                        return true;
                    case (int)Setting.ShortCutKey.enmContent.JumpAnchorNext:
                        doc = GetActiveDocument();
                        if (doc != null) doc.editor.ActionJumpAnchorNext(null);
                        return true;
                    case (int)Setting.ShortCutKey.enmContent.JumpAnchorPrev:
                        doc = GetActiveDocument();
                        if (doc != null) doc.editor.ActionJumpAnchorPrevious(null);
                        return true;
                    case (int)Setting.ShortCutKey.enmContent.Compile:
                        Comp();
                        return true;
                    case (int)Setting.ShortCutKey.enmContent.M98:
                        M98Preprocess();
                        return true;
                    case (int)Setting.ShortCutKey.enmContent.Pause:
                        pause();
                        return true;
                    case (int)Setting.ShortCutKey.enmContent.FadeOut:
                        fadeOut();
                        return true;
                    case (int)Setting.ShortCutKey.enmContent.Stop:
                        stop();
                        return true;
                    case (int)Setting.ShortCutKey.enmContent.Slow:
                        slow();
                        return true;
                    case (int)Setting.ShortCutKey.enmContent.Fastx4:
                        ff();
                        return true;
                    case (int)Setting.ShortCutKey.enmContent.Kbd:
                        //TsmiShowMIDIKbd_Click(null, null);
                        return true;
                    case (int)Setting.ShortCutKey.enmContent.Sien:
                        sien();
                        return true;
                    case (int)Setting.ShortCutKey.enmContent.MDPlay:
                        tsmiExport_toDriverFormatAndPlay_Click(null,null);
                        return true;
                    case (int)Setting.ShortCutKey.enmContent.CloseTab:
                        IDockContent dc1 = GetActiveDockContent();
                        if (dc1 == null) break;
                        if (!(dc1 is FrmEditor)) break;
                        ((FrmEditor)dc1).Close();
                        return true;
                    case (int)Setting.ShortCutKey.enmContent.CloseTabForce:
                        IDockContent dc2 = GetActiveDockContent();
                        if (dc2 == null) break;
                        if (!(dc2 is FrmEditor)) break;
                        ((FrmEditor)dc2).forceClose = true;
                        ((FrmEditor)dc2).Close();
                        return true;

                    case (int)Setting.ShortCutKey.enmContent.CommentOnOff:
                        doc = GetActiveDocument();
                        if (doc != null) doc.editor.ActionComment(null);
                        return true;
                    case (int)Setting.ShortCutKey.enmContent.PartEnter:
                        doc = GetActiveDocument();
                        if (doc != null) doc.editor.ActionShiftEnter(null);
                        return true;
                    case (int)Setting.ShortCutKey.enmContent.Home:
                        doc = GetActiveDocument();
                        if (doc != null) doc.editor.ActionHome(null);
                        return true;

                    case (int)Setting.ShortCutKey.enmContent.Ch01Solo:
                        frmPartCounter.ClickSOLO(0);
                        return true;
                    case (int)Setting.ShortCutKey.enmContent.Ch02Solo:
                        frmPartCounter.ClickSOLO(1);
                        return true;
                    case (int)Setting.ShortCutKey.enmContent.Ch03Solo:
                        frmPartCounter.ClickSOLO(2);
                        return true;
                    case (int)Setting.ShortCutKey.enmContent.Ch04Solo:
                        frmPartCounter.ClickSOLO(3);
                        return true;
                    case (int)Setting.ShortCutKey.enmContent.Ch05Solo:
                        frmPartCounter.ClickSOLO(4);
                        return true;
                    case (int)Setting.ShortCutKey.enmContent.Ch06Solo:
                        frmPartCounter.ClickSOLO(5);
                        return true;
                    case (int)Setting.ShortCutKey.enmContent.Ch07Solo:
                        frmPartCounter.ClickSOLO(6);
                        return true;
                    case (int)Setting.ShortCutKey.enmContent.Ch08Solo:
                        frmPartCounter.ClickSOLO(7);
                        return true;
                    case (int)Setting.ShortCutKey.enmContent.Ch09Solo:
                        frmPartCounter.ClickSOLO(8);
                        return true;
                    case (int)Setting.ShortCutKey.enmContent.Ch10Solo:
                        frmPartCounter.ClickSOLO(9);
                        return true;
                    case (int)Setting.ShortCutKey.enmContent.Ch11Solo:
                        frmPartCounter.ClickSOLO(10);
                        return true;
                    case (int)Setting.ShortCutKey.enmContent.ResetSolo:
                        frmPartCounter.ClickSOLO(-1);
                        return true;
                    case (int)Setting.ShortCutKey.enmContent.Ch01Mute:
                        frmPartCounter.ClickMUTE(0);
                        return true;
                    case (int)Setting.ShortCutKey.enmContent.Ch02Mute:
                        frmPartCounter.ClickMUTE(1);
                        return true;
                    case (int)Setting.ShortCutKey.enmContent.Ch03Mute:
                        frmPartCounter.ClickMUTE(2);
                        return true;
                    case (int)Setting.ShortCutKey.enmContent.Ch04Mute:
                        frmPartCounter.ClickMUTE(3);
                        return true;
                    case (int)Setting.ShortCutKey.enmContent.Ch05Mute:
                        frmPartCounter.ClickMUTE(4);
                        return true;
                    case (int)Setting.ShortCutKey.enmContent.Ch06Mute:
                        frmPartCounter.ClickMUTE(5);
                        return true;
                    case (int)Setting.ShortCutKey.enmContent.Ch07Mute:
                        frmPartCounter.ClickMUTE(6);
                        return true;
                    case (int)Setting.ShortCutKey.enmContent.Ch08Mute:
                        frmPartCounter.ClickMUTE(7);
                        return true;
                    case (int)Setting.ShortCutKey.enmContent.Ch09Mute:
                        frmPartCounter.ClickMUTE(8);
                        return true;
                    case (int)Setting.ShortCutKey.enmContent.Ch10Mute:
                        frmPartCounter.ClickMUTE(9);
                        return true;
                    case (int)Setting.ShortCutKey.enmContent.Ch11Mute:
                        frmPartCounter.ClickMUTE(10);
                        return true;
                    case (int)Setting.ShortCutKey.enmContent.ResetMute:
                        frmPartCounter.ClickMUTE(-1);
                        return true;

                    case (int)Setting.ShortCutKey.enmContent.Play:
                        jumpSoloModeSw = false;
                        //      doPlay isTrace doSkip
                        Compile(true, false, false, false, false);
                        return true;
                    case (int)Setting.ShortCutKey.enmContent.JsoloPlay:
                        jumpSoloModeSw = true;
                        //      doPlay isTrace doSkip
                        //Compile(true, false, false, false, false);
                        Compile(true, false, true, false, false);
                        return true;
                    case (int)Setting.ShortCutKey.enmContent.SkipJsoloPlay:
                        jumpSoloModeSw = true;
                        //      doPlay isTrace doSkip
                        Compile(true, false, true, false, false);
                        return true;
                    case (int)Setting.ShortCutKey.enmContent.SkipPlay:
                        jumpSoloModeSw = false;
                        //      doPlay isTrace doSkip
                        Compile(true, false, true, false, false);
                        return true;
                    case (int)Setting.ShortCutKey.enmContent.SkipTraceJsoloPlay:
                        jumpSoloModeSw = true;
                        //      doPlay isTrace doSkip
                        Compile(true, true, true, false, false);
                        return true;
                    case (int)Setting.ShortCutKey.enmContent.SkipTracePlay:
                        jumpSoloModeSw = false;
                        //      doPlay isTrace doSkip
                        Compile(true, true, true, false, false);
                        return true;
                    case (int)Setting.ShortCutKey.enmContent.TraceJsoloPlay:
                        jumpSoloModeSw = true;
                        //      doPlay isTrace doSkip
                        //Compile(true, true, false, false, false);
                        Compile(true, true, true, false, false);
                        return true;
                    case (int)Setting.ShortCutKey.enmContent.TracePlay:
                        jumpSoloModeSw = false;
                        //      doPlay isTrace doSkip
                        Compile(true, true, false, false, false);
                        return true;

                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        public void FrmMain_KeyDown(object sender, KeyEventArgs e)
        {
            ctrl = (e.KeyData & Keys.Control) == Keys.Control;
            shift = (e.KeyData & Keys.Shift) == Keys.Shift;
            alt = (e.KeyData & Keys.Alt) == Keys.Alt;
            tssbPlay.Text = (ctrl ? Properties.Resources.lblF5_Trace : "")
                + (shift ? Properties.Resources.lblF5_Skip : "")
                + (alt ? Properties.Resources.lblF5_JSolo : "")
                + Properties.Resources.lblF5_Play;

            tssbStop.Text = (shift ? Properties.Resources.lblF9_FadeOut : Properties.Resources.lblF9_Stop);
            //switch (e.KeyCode)
            //{
            //    //case Keys.F5:
            //    //    jumpSoloModeSw = alt;//nVidia Geforce ExperienceがインストールされているとAlt+F5が検知できない
            //    //    Compile(true, ctrl, shift, false, false);
            //    //    break;
            //    default:
            //        //↓KeyData確認用
            //        //log.Write(string.Format("動作未定義のキー：{0}",e.KeyData));
            //        break;
            //}
        }

        private void FrmMain_KeyUp(object sender, KeyEventArgs e)
        {

            ctrl = (e.KeyData & Keys.Control) == Keys.Control;
            shift = (e.KeyData & Keys.Shift) == Keys.Shift;
            alt = (e.KeyData & Keys.Alt) == Keys.Alt;
            tssbPlay.Text = (ctrl ? Properties.Resources.lblF5_Trace : "")
                + (shift ? Properties.Resources.lblF5_Skip : "")
                + (alt ? Properties.Resources.lblF5_JSolo : "")
                + Properties.Resources.lblF5_Play;

            tssbStop.Text = (shift ? Properties.Resources.lblF9_FadeOut : Properties.Resources.lblF9_Stop);
        }


        private void OpenFile(string fileName)
        {
            Document dc = new Document(setting, Common.GetEnmMmlFileFormat(Path.GetExtension(fileName)));//,frmSien);
            if (fileName != "") dc.InitOpen(fileName);
            dc.editor.Show(dpMain, DockState.Document);
            dc.editor.main = this;
            dc.editor.document = dc;

            frmFolderTree.tvFolderTree.Nodes.Clear();
            frmFolderTree.tvFolderTree.Nodes.Add(dc.gwiTree);
            string path1 = Path.GetDirectoryName(dc.gwiFullPath);
            path1 = string.IsNullOrEmpty(path1) ? dc.gwiFullPath : path1;
            frmFolderTree.basePath = path1;

            FormBox.Add(dc.editor);
            DocumentBox.Add(dc);
            AddGwiFileHistory(fileName);
            UpdateGwiFileHistory();
        }

        private void ImportFile(string fileName)
        {
            Document dc = new Document(setting, EnmMmlFileFormat.GWI);//, frmSien);
            if (fileName != "") dc.InitOpen(fileName);

            dc.editor.Show(dpMain, DockState.Document);
            dc.editor.main = this;
            dc.editor.document = dc;

            frmFolderTree.tvFolderTree.Nodes.Clear();
            frmFolderTree.tvFolderTree.Nodes.Add(dc.gwiTree);
            string path1 = System.IO.Path.GetDirectoryName(dc.gwiFullPath);
            path1 = string.IsNullOrEmpty(path1) ? dc.gwiFullPath : path1;
            frmFolderTree.basePath = path1;

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

        private void OpenLatestFile(string startFn)
        {
            if (!string.IsNullOrEmpty(startFn))
            {
                OpenFile(startFn);
                return;
            }

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

        public void M98Preprocess()
        {
            if (Compiling != 0) return;

            frmLog.ClearLog();
            //stop();

            IDockContent dc = GetActiveDockContent();
            if (dc == null) return;
            if (!(dc is FrmEditor)) return;
            activeMMLTextLines = new string[1] { ((FrmEditor)dc).azukiControl.Text };
            string tempPath = Path.Combine(Common.GetApplicationDataFolder(true), "temp", Path.GetFileName(((Document)((FrmEditor)dc).Tag).gwiFullPath));
            if (Path.GetExtension(tempPath) != ".muc")
            {
                MessageBox.Show(".mucファイルのみです!");
                return;
            }
            if (mucom == null)
            {
                MessageBox.Show("Not found M98DotNET.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            title = Path.GetFileName(Path.GetFileName(((Document)((FrmEditor)dc).Tag).gwiFullPath));

            args = new string[2];
            args[1] = tempPath;
            string path1 = System.IO.Path.GetDirectoryName(((Document)((FrmEditor)dc).Tag).gwiFullPath);
            path1 = string.IsNullOrEmpty(path1) ? ((Document)((FrmEditor)dc).Tag).gwiFullPath : path1;
            wrkPath = path1;

            traceInfoSw = false;
            Sgry.Azuki.WinForms.AzukiControl ac = null;
            ac = ((FrmEditor)dc).azukiControl;
            ac.ColorScheme.LineNumberBack = Color.FromArgb(setting.ColorScheme.Azuki_LineNumberBack_Normal);
            ac.ColorScheme.LineNumberFore = Color.FromArgb(setting.ColorScheme.Azuki_LineNumberFore_Normal);
            statusStrip1.BackColor = Color.FromArgb(setting.ColorScheme.StatusStripBack_Normal);
            ac.Document.Unmark(0, ac.Text.Length, 1);
            ac.Document.Unmark(0, ac.Text.Length, 2);
            ac.IsReadOnly = false;
            ac.Refresh();

            isSuccess = true;
            //frmPartCounter.ClearCounter();
            frmErrorList.dataGridView1.Rows.Clear();

            Thread trdStartPreprocess = new Thread(new ThreadStart(startPreprocess));
            trdStartPreprocess.Start();
            Compiling = 1;//フラグはコンパイルと共有
        }

        public void Compile(bool doPlay, bool isTrace, bool doSkip, bool doSkipStop, bool doExport, string[] text = null)
        {
            if (Compiling != 0) return;

            frmLog.ClearLog();
            stop();

            IDockContent dc = GetActiveDockContent();
            EnmMmlFileFormat isMuc = EnmMmlFileFormat.GWI;

            if (text == null)
            {
                if (dc == null) return;
                if (!(dc is FrmEditor)) return;
                compileTargetDocument = (FrmEditor)dc;
                activeMMLTextLines = ((FrmEditor)dc).azukiControl.Text.Split(new string[] { "\r\n" }, StringSplitOptions.None);

                string tempPath = Path.Combine(Common.GetApplicationDataFolder(true), "temp", Path.GetFileName(((Document)((FrmEditor)dc).Tag).gwiFullPath));
                title = Path.GetFileName(Path.GetFileName(((Document)((FrmEditor)dc).Tag).gwiFullPath));

                if (Path.GetExtension(tempPath).ToLower() == ".muc")
                {
                    if (mucom == null)
                    {
                        MessageBox.Show("Not found mucomDotNET.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    activeMMLTextLines = new string[] { ((FrmEditor)dc).azukiControl.Text };
                    isMuc = EnmMmlFileFormat.MUC;
                }
                else if (Path.GetExtension(tempPath).ToLower() == ".mml")
                {
                    if (pmdmng == null)
                    {
                        MessageBox.Show("Not found PMDDotNET.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    activeMMLTextLines = new string[] { ((FrmEditor)dc).azukiControl.Text };
                    isMuc = EnmMmlFileFormat.MML;
                }
                else if (Path.GetExtension(tempPath).ToLower() == ".mdl")
                {
                    if (mdmng == null)
                    {
                        MessageBox.Show("Not found MoonDriverDotNET.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    activeMMLTextLines = new string[] { ((FrmEditor)dc).azukiControl.Text };
                    isMuc = EnmMmlFileFormat.MDL;
                }

                args = new string[2];
                args[1] = tempPath;
                string path1 = System.IO.Path.GetDirectoryName(((Document)((FrmEditor)dc).Tag).gwiFullPath);
                path1 = string.IsNullOrEmpty(path1) ? ((Document)((FrmEditor)dc).Tag).gwiFullPath : path1;
                wrkPath = path1;
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
                ac.Document.Unmark(0, ac.Text.Length, 2);
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
            caretPoint = Point.Empty;
            if (doSkip)
            {
                if (ac != null)
                {
                    int ci = ac.CaretIndex;
                    int row, col;
                    ac.GetLineColumnIndexFromCharIndex(ci, out row, out col);
                    caretPoint = new Point(col, row);
                    switch (isMuc)
                    {
                        case EnmMmlFileFormat.MUC:
                            caretPoint.Y++;
                            break;
                    }

                    int st = ac.GetLineHeadIndexFromCharIndex(ci);
                    int li = ac.GetLineIndexFromCharIndex(ci);
                    //int ed = st + ac.GetLineLength(li);
                    string line = ac.GetTextInRange(st, ci);
                    if (line == null || line.Length < 1) doSkip = false;
                    //先頭の文字が'ではないときは既存の動作
                    else
                    {
                        switch (isMuc)
                        {
                            case EnmMmlFileFormat.MUC:
                                if (line[0] < 'A' || line[0] > 'K') doSkip = false;
                                break;
                            case EnmMmlFileFormat.MML:
                                break;
                            default:
                                if (line[0] != '\'') doSkip = false;
                                break;
                        }
                    }
                }
            }

            //frmPartCounter.ClearCounter();//ここではパート表示をクリアしない。コンパイル終了時(成功時)にクリアするため

            frmErrorList.dataGridView1.Rows.Clear();

            Thread trdStartCompile = new Thread(new ThreadStart(startCompile));
            trdStartCompile.Start();
            Compiling = 1;
        }

        private void startPreprocess()
        {
            Core.log.Open();
            Core.log.Write("start preprocess thread");
            Action dmy = updateTitle;
            this.Invoke(dmy);
            Core.log.Write(string.Format("  preprocess at [{0}]", title));
            msgBox.clear();

            Compiling |= 8;

            Core.log.Write("Call M98DotNET preprocessor");

            string stPath = System.Windows.Forms.Application.StartupPath;

            try
            {
                m98ResultMucString = mucom.preprocess(activeMMLTextLines[0]);
            }
            catch (ApplicationException)
            {
                isSuccess = false;
            }
            catch (Exception e)
            {
                Disp(string.Format("Message:\r\n{0}\r\nStackTrace:\r\n{1}\r\n", e.Message, e.StackTrace));
                isSuccess = false;
            }

            if (string.IsNullOrEmpty(m98ResultMucString))
            {
                isSuccess = false;
            }

            Core.log.Write("Return M98DotNET preprocessor");
            Core.log.Write("Disp Result");

            dmy = finishedPreprocess;
            this.Invoke(dmy);

            Core.log.Write("end preprocess thread");
            Core.log.Close();

        }

        private void finishedPreprocess()
        {
            if (isSuccess)
            {
                IDockContent idc = GetActiveDockContent();
                if (idc == null) return;
                if (!(idc is FrmEditor)) return;

                string fileName;
                string parentFullPath;

                if (((Document)((FrmEditor)idc).Tag).parentFullPath == "")
                {
                    fileName = ((Document)((FrmEditor)idc).Tag).gwiFullPath;
                    parentFullPath = fileName;
                }
                else
                {
                    fileName = ((Document)((FrmEditor)idc).Tag).parentFullPath;
                    parentFullPath = ((Document)((FrmEditor)idc).Tag).parentFullPath;
                }

                string path1 = System.IO.Path.GetDirectoryName(fileName);
                path1 = string.IsNullOrEmpty(path1) ? fileName : path1;
                fileName = Path.Combine(
                    path1
                    , Path.GetFileNameWithoutExtension(fileName)
                    + string.Format("_{0}", m98Count++) + ".muc"
                    );
                title = Path.GetFileName(Path.GetFileName(fileName));
                Document dc = new Document(setting, EnmMmlFileFormat.MUC);//, frmSien);
                dc.InitOpen(fileName, m98ResultMucString);
                dc.editor.Show(dpMain, DockState.Document);
                dc.editor.main = this;
                dc.editor.document = dc;
                dc.parentFullPath = parentFullPath;

                frmFolderTree.tvFolderTree.Nodes.Clear();
                frmFolderTree.tvFolderTree.Nodes.Add(dc.gwiTree);
                path1 = System.IO.Path.GetDirectoryName(dc.gwiFullPath);
                path1 = string.IsNullOrEmpty(path1) ? dc.gwiFullPath : path1;
                frmFolderTree.basePath = path1;

                FormBox.Add(dc.editor);
                DocumentBox.Add(dc);
                //AddGwiFileHistory(fileName);
                //UpdateGwiFileHistory();
            }

            Compiling = 0;
            UpdateControl();
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

            string ext = Path.GetExtension(args[1]).ToLower();
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
                case ".mml":
                    if (pmdmng == null)
                    {
                        MessageBox.Show("Not found PMDDotNET.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    Compiling |= 8;
                    startCompileMML();
                    break;
                case ".mdl":
                    if (mdmng == null)
                    {
                        MessageBox.Show("Not found MoonDriverDotNET.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    Compiling |= 16;
                    startCompileMDL();
                    break;
            }
        }

        private void startCompileGWI()
        {
            Core.log.Write("Call mml2vgm core");

            string stPath = System.Windows.Forms.Application.StartupPath;
            if (!doExport)
            {
                mv = new Mml2vgm(compileTargetDocument, activeMMLTextLines, args[1], null, stPath, Disp, wrkPath, false);
            }
            else
            {
                mv = new Mml2vgm(compileTargetDocument, activeMMLTextLines, args[1], args[1], stPath, Disp, wrkPath, true);
            }
            mv.isIDE = true;
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
                mubData = mucom.compileFromSrcText(activeMMLTextLines[0], wrkPath, doSkip ? caretPoint : Point.Empty);
            }
            catch
            {
                isSuccess = false;
            }

            if (mubData == null)
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

        private void startCompileMML()
        {
            Core.log.Write("Call PMDDotNET compiler");

            string stPath = System.Windows.Forms.Application.StartupPath;

            try
            {
                mData = pmdmng.compileFromSrcText(activeMMLTextLines[0], wrkPath, args[1], doSkip ? caretPoint : Point.Empty, !doExport);
            }
            catch
            {
                isSuccess = false;
            }

            if (mData == null)
            {
                isSuccess = false;
            }

            Core.log.Write("Return PMDDotNET compiler");
            //}

            Core.log.Write("Disp Result");

            Action dmy = finishedCompile;
            this.Invoke(dmy);

            Core.log.Write("end compile thread");
            Core.log.Close();
        }

        private void startCompileMDL()
        {
            Core.log.Write("Call MoonDriverDotNET compiler");

            string stPath = System.Windows.Forms.Application.StartupPath;

            try
            {
                mData = mdmng.compileFromSrcText(activeMMLTextLines[0], wrkPath, args[1], doSkip ? caretPoint : Point.Empty, !doExport);
            }
            catch
            {
                isSuccess = false;
            }

            if (mData == null)
            {
                isSuccess = false;
            }

            Core.log.Write("Return MoonDriverDotNET compiler");
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
            if (isSuccess) Audio.silent = false;

            //

            if ((Compiling & 2) != 0)
            {
                finishedCompileGWI();
            }
            else if ((Compiling & 4) != 0)
            {
                finishedCompileMUC();
            }
            else if ((Compiling & 8) != 0)
            {
                finishedCompileMML();
            }
            else if ((Compiling & 16) != 0)
            {
                finishedCompileMDL();
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

            Document doc = null;
            if (compileTargetDocument is FrmEditor) doc = (Document)((FrmEditor)compileTargetDocument).Tag;
            else doc = (Document)compileTargetDocument;
            muteManager.Add(doc);

            try
            {
                //パートカウンターのリスト表示を初期化
                if (isSuccess)
                {
                    frmPartCounter.ClearCounter();
                    Object[] cells = new object[8];

                    //int trackNum = 0;
                    foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in mv.desVGM.chips)
                    {
                        foreach (ClsChip chip in kvp.Value)
                        {
                            if (chip == null) continue;
                            List<partWork> pw = chip.lstPartWork;
                            for (int i = 0; i < pw.Count; i++)
                            {
                                if (pw[i].clockCounter == 0) continue;

                                cells[0] = int.Parse(pw[i].pg[0].PartName.Substring(2, 2));
                                cells[1] = chip.ChipID;//ChipIndex
                                cells[2] = pw[i].pg[0].chipNumber;//ChipNumber
                                cells[3] = pw[i].pg[0].PartName.Substring(0, 2).Replace(" ", "") + int.Parse(pw[i].pg[0].PartName.Substring(2, 2)).ToString();
                                cells[4] = pw[i].pg[0].chip.Name;//.ToUpper();
                                cells[5] = pw[i].clockCounter;
                                cells[6] = "-";
                                cells[7] = muteManager.UpdateTrackInfo(doc, cells);
                                frmPartCounter.AddPartCounter(cells);
                            }

                        }
                    }
                    frmPartCounter.refreshMuteSolo();
                }
            }
            catch
            {
                isSuccess = false;
                Compiling = 0;
                UpdateControl();
                return;
            }

            frmLog.tbLog.AppendText(msg.get("I0107"));

            //エラーリストウィンドウにエラーメッセージリストを追加
            foreach (msgInfo mes in msgBox.getErr())
            {
                if (mes.document != null)
                {
                    frmErrorList.dataGridView1.Rows.Add(
                        "Error",
                        ((FrmEditor)mes.document).document.gwiFullPath,
                        mes.line == -1 ? "-" : (mes.line + 1).ToString(),
                        mes.body);
                }
                else
                {
                    frmErrorList.dataGridView1.Rows.Add(
                        "Error",
                        mes.filename,
                        mes.line == -1 ? "-" : (mes.line + 1).ToString(),
                        mes.body);
                }
                //frmConsole.textBox1.AppendText(string.Format(msg.get("I0109"), mes));
            }

            //エラーリストウィンドウにワーニングメッセージリストを追加
            foreach (msgInfo mes in msgBox.getWrn())
            {
                if (mes.document != null)
                {
                    frmErrorList.dataGridView1.Rows.Add(
                        "Warning",
                        ((FrmEditor)mes.document).document.gwiFullPath,
                        mes.line == -1 ? "-" : (mes.line + 1).ToString(),
                        mes.body);
                }
                else
                {
                    frmErrorList.dataGridView1.Rows.Add(
                    "Warning",
                    mes.filename,
                    mes.line == -1 ? "-" : (mes.line + 1).ToString(),
                    mes.body);
                }
                //frmConsole.textBox1.AppendText(string.Format(msg.get("I0108"), mes));
            }

            //ログウィンドウに各種メッセージを表示
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



            if (!isSuccess)
            {
                Compiling = 0;
                UpdateControl();
                return;
            }

            if (args.Length != 2 || !doPlay || msgBox.getErr().Length >= 1)
            {
                Compiling = 0;
                UpdateControl();
                return;
            }

            try
            {
                //ヘッダー情報にダミーコマンド情報分の値を水増しした値をセットしなおす
                if (mv.desVGM.info.format == enmFormat.VGM)
                {
                    if (!exportWav)
                    {
                        uint EOFOffset = Common.getLE32(mv.desBuf, 0x04) + (uint)mv.desVGM.dummyCmdCounter;
                        Common.SetLE32(mv.desBuf, 0x04, EOFOffset);

                        uint GD3Offset = Common.getLE32(mv.desBuf, 0x14) + (uint)mv.desVGM.dummyCmdCounter;
                        Common.SetLE32(mv.desBuf, 0x14, GD3Offset);

                        uint LoopOffset = (uint)mv.desVGM.dummyCmdLoopOffset - 0x1c;
                        Common.SetLE32(mv.desBuf, 0x1c, LoopOffset);
                    }

                    InitPlayer(EnmFileFormat.VGM, mv.desBuf, mv.desVGM.jumpPointClock);
                }
                else if (mv.desVGM.info.format == enmFormat.XGM)
                {
                    //XGM
                    InitPlayer(EnmFileFormat.XGM, mv.desBuf, mv.desVGM.jumpPointClock);
                }
                else
                {
                    //ZGM
                    InitPlayer(EnmFileFormat.ZGM, mv.desBuf, mv.desVGM.jumpPointClock);
                }
            }
            catch (Exception)
            {
                MessageBox.Show(msg.get("E0100"), "mml2vgm", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Compiling = 0;
            UpdateControl();
        }

        private string mucPartName = "ABCDEFGHIJKLMNOPQRSTUVabcdefghijklmnopqrstuvWXYZwxyz";

        private void finishedCompileMUC()
        {
            musicDriverInterface.CompilerInfo ci = mucom.GetCompilerInfo();

            Document doc = null;
            if (compileTargetDocument is FrmEditor) doc = (Document)((FrmEditor)compileTargetDocument).Tag;
            else doc = (Document)compileTargetDocument;
            muteManager.Add(doc);

            if (compileTargetDocument != null && compileTargetDocument is FrmEditor)
            {
                FrmEditor fe = (FrmEditor)compileTargetDocument;
                AzukiControl ac = fe.azukiControl;
                for (int i = 0; i < ac.Document.LineCount; i++)
                    ac.Document.SetLineIconIndex(i, -1);
                if (ci != null && ci.jumpRow > 0)
                {
                    ac.Document.SetLineIconIndex(ci.jumpRow - 1, 5);//●
                }
            }

            try
            {
                if (isSuccess)
                {
                    frmPartCounter.ClearCounter();
                    Object[] cells = new object[8];
                    int[] pn = new int[] { 1, 2, 3, 10, 11, 12, 13, 4, 5, 6, 19 };
                    if (ci.formatType == "mub")
                    {
                        //int trackNum = 0;
                        pn = new int[] { 1, 2, 3, 10, 11, 12, 13, 4, 5, 6, 19 };
                        for (int i = 0; i < 11; i++)
                        {
                            //if (pw[i].clockCounter == 0) continue;

                            cells[0] = pn[i];//PartNumber
                            cells[1] = 0;//ChipIndex
                            cells[2] = 0;//ChipNumber
                            cells[3] = ((char)('A' + i)).ToString();
                            cells[4] = "YM2608";//.ToUpper();
                            cells[5] = ci.totalCount[i];
                            cells[6] = ci.loopCount[i];
                            cells[7] = muteManager.UpdateTrackInfo(doc, cells);
                            frmPartCounter.AddPartCounter(cells);
                        }
                    }
                    else
                    {
                        //int trackNum = 0;
                        pn = new int[] { 1, 2, 3, 10, 11, 12, 13, 4, 5, 6, 19 };
                        for (int i = 0; i < 52; i++)
                        {
                            for (int j = 0; j < 10; j++)
                            {
                                //if (pw[i].clockCounter == 0) continue;

                                cells[0] = (i < 44 ? pn[i % 11] : (i - 43));//PartNumber
                                cells[1] = 0;//ChipIndex
                                cells[2] = ((i / 11) & 1);//ChipNumber
                                cells[3] = (mucPartName[i]).ToString() + j.ToString();
                                cells[4] = i < 22 ? "YM2608" : (i < 44 ? "YM2610B" : "YM2151");//.ToUpper();
                                cells[5] = ci.totalCount[i * 10 + j];
                                cells[6] = ci.loopCount[i * 10 + j];
                                if (ci.bufferCount[i * 10 + j] > 3)
                                {
                                    cells[7] = muteManager.UpdateTrackInfo(doc, cells);
                                    frmPartCounter.AddPartCounter(cells);
                                }
                            }
                        }
                    }

                    frmPartCounter.refreshMuteSolo();
                }
            }
            catch
            {
                isSuccess = false;
                Compiling = 0;
                UpdateControl();
                return;
            }

            //frmLog.tbLog.AppendText(msg.get("I0107"));

            if (ci != null)
                foreach (Tuple<int, int, string> mes in ci.errorList)
                {
                    frmErrorList.dataGridView1.Rows.Add("Error", "-"
                        , mes.Item1 == -1 ? "-" : (mes.Item1 + 1).ToString()
                        , mes.Item3);
                    msgInfo mi = new msgInfo(null, "", mes.Item1, mes.Item2, -1, mes.Item3);
                    msgBox.setErrMsg(mi);
                }

            if (ci != null)
                foreach (Tuple<int, int, string> mes in ci.warningList)
                {
                    frmErrorList.dataGridView1.Rows.Add("Warning", "-"
                        , mes.Item1 == -1 ? "-" : (mes.Item1 + 1).ToString()
                        , mes.Item3);
                    msgInfo mi = new msgInfo(null, "", mes.Item1, mes.Item2, -1, mes.Item3);
                    msgBox.setWrnMsg(mi);
                }

            if (isSuccess)
            {
                mubCompilerInfo = ci;

                if (ci.jumpChannel != null && jumpSoloModeSw)
                {
                    for (int i = 0; i < 52; i++)
                    {
                        if (i < 44)
                        {
                            int p = i % 11;
                            bool solo = false;
                            if (p == ci.jumpChannel[0]) solo = true;

                            int ch = p;
                            //if (p < 2)//FM 1-2
                            //{
                            //    Audio.chipRegister.YM2608[0].ChMasks[ch] = !solo;
                            //}
                            //else if (p == 2)//FM 3
                            //{
                            //    Audio.chipRegister.YM2608[0].ChMasks[2] = !solo;
                            //    Audio.chipRegister.YM2608[0].ChMasks[6] = !solo;
                            //    Audio.chipRegister.YM2608[0].ChMasks[7] = !solo;
                            //    Audio.chipRegister.YM2608[0].ChMasks[8] = !solo;
                            //}
                            //else if (p < 6)//SSG
                            //{
                            //    ch = p + 6;
                            //    Audio.chipRegister.YM2608[0].ChMasks[ch] = !solo;
                            //}
                            //else if (p == 6)//Rhythm
                            //{
                            //    Audio.chipRegister.YM2608[0].ChMasks[12] = !solo;
                            //    Audio.chipRegister.YM2608[0].ChMasks[13] = !solo;
                            //    Audio.chipRegister.YM2608[0].ChMasks[14] = !solo;
                            //    Audio.chipRegister.YM2608[0].ChMasks[15] = !solo;
                            //    Audio.chipRegister.YM2608[0].ChMasks[16] = !solo;
                            //    Audio.chipRegister.YM2608[0].ChMasks[17] = !solo;
                            //}
                            //else if (p < 10)
                            //{
                            //    ch = p - 4;
                            //    Audio.chipRegister.YM2608[0].ChMasks[ch] = !solo;
                            //}
                            //else
                            //{
                            //    Audio.chipRegister.YM2608[0].ChMasks[18] = !solo;
                            //}
                        }
                        else
                        {
                            int ch = i - 44;
                            bool solo = false;
                            if (ch == ci.jumpChannel[0]) solo = true;
                            Audio.chipRegister.YM2151[0].ChMasks[ch] = !solo;
                        }
                    }
                }

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

        private void finishedCompileMML()
        {
            musicDriverInterface.CompilerInfo ci = pmdmng.GetCompilerInfo();
            Document doc = null;
            if (compileTargetDocument is FrmEditor) doc = (Document)((FrmEditor)compileTargetDocument).Tag;
            else doc = (Document)compileTargetDocument;
            muteManager.Add(doc);

            try
            {
                if (isSuccess)
                {
                    frmPartCounter.ClearCounter();
                    Object[] cells = new object[8];
                    //int trackNum = 0;

                    try
                    {
                        for (int i = 0; i < ci.totalCount.Count; i++)
                        {
                            cells[0] = -1;

                            if (ci.partType == null) continue;

                            if (ci.partType[i] == "FMOPN" || ci.partType[i] == "PPZ8")
                            {
                                cells[0] = ci.partNumber[i] + 1;
                            }

                            //Fm3exは　D/E/F又は任意
                            if (ci.partType[i] == "FMOPNex")
                            {
                                cells[0] = 6 + 1 + (ci.partNumber[i] % 3);//FM3ex1～3 part(6-8)に割り当てる
                            }

                            //SSG はG/H/I固定
                            if (ci.partName[i][0] == 'G') cells[0] = 9 + 1;//SSG1 part(9)に割り当てる
                            if (ci.partName[i][0] == 'H') cells[0] = 10 + 1;//SSG2 part(10)に割り当てる
                            if (ci.partName[i][0] == 'I') cells[0] = 11 + 1;//SSG3 part(11)に割り当てる

                            //ADPCM はJ固定
                            if (ci.partName[i][0] == 'J') cells[0] = 18 + 1;//ADPCM part(18)に割り当てる

                            //K/R はK固定
                            if (ci.partName[i][0] == 'K') cells[0] = 12 + 1;//BD part(12)に割り当てる

                            cells[1] = 0;//ChipIndex
                            cells[2] = 0;//ChipNumber
                            cells[3] = ci.partName[i];
                            cells[4] = ci.partType[i] != "PPZ8" ? "YM2608" : ci.partType[i];
                            cells[5] = ci.totalCount[i];
                            cells[6] = ci.loopCount[i];
                            cells[7] = muteManager.UpdateTrackInfo(doc,  cells);
                            frmPartCounter.AddPartCounter(cells);
                        }
                    }
                    catch (Exception e)
                    {
                        Log.WriteLine(LogLevel.ERROR, string.Format("Exception:\r\nMessage\r\n{0}\r\nStackTrace\r\n{1}\r\n", e.Message, e.StackTrace));
                    }
                    frmPartCounter.refreshMuteSolo();

                }
            }
            catch
            {
                isSuccess = false;
                Compiling = 0;
                UpdateControl();
                return;
            }

            foreach (Tuple<int, int, string> mes in ci.errorList)
            {
                frmErrorList.dataGridView1.Rows.Add("Error", "-"
                    , mes.Item1 == -1 ? "-" : (mes.Item1 + 1).ToString()
                    , mes.Item3);
                msgInfo mi = new msgInfo(null, "", mes.Item1, mes.Item2, -1, mes.Item3);
                msgBox.setErrMsg(mi);
            }

            foreach (Tuple<int, int, string> mes in ci.warningList)
            {
                frmErrorList.dataGridView1.Rows.Add("Warning", "-"
                    , mes.Item1 == -1 ? "-" : (mes.Item1 + 1).ToString()
                    , mes.Item3);
                msgInfo mi = new msgInfo(null, "", mes.Item1, mes.Item2, -1, mes.Item3);
                msgBox.setWrnMsg(mi);
            }

            if (isSuccess)
            {

                if (doPlay && ci.errorList.Count < 1)
                {
                    try
                    {
                        //WriteMmlDatumn("test.m", mData);
                        InitPlayer(EnmFileFormat.M, mData);
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

        private void finishedCompileMDL()
        {
            musicDriverInterface.CompilerInfo ci = mdmng.GetCompilerInfo();
            Document doc = null;
            if (compileTargetDocument is FrmEditor) doc = (Document)((FrmEditor)compileTargetDocument).Tag;
            else doc = (Document)compileTargetDocument;

            muteManager.Add(doc);

            if (ci == null)
            {
                Compiling = 0;
                UpdateControl();
                return;
            }
            try
            {
                if (isSuccess)
                {
                    frmPartCounter.ClearCounter();
                    Object[] cells = new object[8];
                    //int trackNum = 0;
                    try
                    {
                        if (ci != null && ci.totalCount != null && ci.totalCount.Count > 0)
                        {
                            for (int i = 0; i < ci.totalCount.Count; i++)
                            {
                                //if (pw[i].clockCounter == 0) continue;
                                int ch = ci.partNumber[i];
                                ch += (ci.partName[i][0] >= 'G' && ci.partName[i][0] <= 'I') ? 3 : 0;
                                ch++;
                                cells[0] = ch;
                                cells[1] = 0;//ChipIndex
                                cells[2] = 0;//ChipNumber
                                cells[3] = ci.partName[i];
                                cells[4] = ci.partType[i];
                                cells[5] = ci.totalCount[i];
                                cells[6] = ci.loopCount[i];
                                cells[7] = muteManager.UpdateTrackInfo(doc, cells);
                                frmPartCounter.AddPartCounter(cells);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Log.WriteLine(LogLevel.ERROR, string.Format("Exception:\r\nMessage\r\n{0}\r\nStackTrace\r\n{1}\r\n", e.Message, e.StackTrace));
                    }
                    frmPartCounter.refreshMuteSolo();
                }
            }
            catch
            {
                isSuccess = false;
                Compiling = 0;
                UpdateControl();
                return;
            }

            foreach (Tuple<int, int, string> mes in ci.errorList)
            {
                frmErrorList.dataGridView1.Rows.Add("Error", "-"
                    , mes.Item1 == -1 ? "-" : (mes.Item1 + 1).ToString()
                    , mes.Item3);
                msgInfo mi = new msgInfo(null, "", mes.Item1, mes.Item2, -1, mes.Item3);
                msgBox.setErrMsg(mi);
            }

            foreach (Tuple<int, int, string> mes in ci.warningList)
            {
                frmErrorList.dataGridView1.Rows.Add("Warning", "-"
                    , mes.Item1 == -1 ? "-" : (mes.Item1 + 1).ToString()
                    , mes.Item3);
                msgInfo mi = new msgInfo(null, "", mes.Item1, mes.Item2, -1, mes.Item3);
                msgBox.setWrnMsg(mi);
            }

            if (isSuccess)
            {
                if (doPlay && ci.errorList.Count < 1)
                {
                    try
                    {
                        //WriteMmlDatumn("test.m", mData);
                        InitPlayer(EnmFileFormat.MDR, mData);
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

        private void WriteMmlDatumn(string fn, MmlDatum[] mmlData)
        {
            List<byte> data = new List<byte>();
            foreach (MmlDatum d in mmlData) data.Add((byte)d.dat);
            File.WriteAllBytes(fn, data.ToArray());
        }

        private delegate void SafeCallDelegate();
        private void jumpSoloMode()
        {
            if (this.InvokeRequired)
            {
                var d = new SafeCallDelegate(jumpSoloMode);
                this.Invoke(d);
            }

            if (mubCompilerInfo == null) return;
            if (!jumpSoloModeSw) return;
            if (mubCompilerInfo.jumpChannel == null) return;

            frmPartCounter.ClickSOLO(-1);
            frmPartCounter.ClickMUTE(-1);
            frmPartCounter.ClickSOLO(mubCompilerInfo.jumpChannel[0]);
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
                    string path1 = System.IO.Path.GetDirectoryName(d.gwiFullPath);
                    path1 = string.IsNullOrEmpty(path1) ? d.gwiFullPath : path1;
                    frmFolderTree.basePath = path1;
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
            TsmiShowSien.Checked = !frmSien.IsHidden;

            tsslCompileError.Text = string.Format(
                "{0}",
                msgBox.getErr().Length
                );
            tsslCompileWarning.Text = string.Format(
                "{0}",
                msgBox.getWrn().Length
                );
            tsslCompileStatus.Text = string.Format(
                "TC:{0} LC:{1}",
                FileInformation.totalCounter,
                FileInformation.loopCounter == -1 ? "-" : FileInformation.loopCounter.ToString()
                );

            //tsslJumpSoloMode.Visible = tsmiJumpSoloMode.Checked;
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
                    if (string.IsNullOrEmpty(filename)) continue;
                    if (filename[filename.Length - 1] == '\\')
                    {
                        continue;
                    }

                    if (Path.GetExtension(filename).ToLower() == ".gwi")
                    {
                        OpenFile(filename);
                        continue;
                    }

                    if (Path.GetExtension(filename).ToLower() == ".muc")
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
                    //if (Path.GetFileName(((Document)dc.Tag).gwiFullPath) != fn)
                    if (((Document)dc.Tag).gwiFullPath != fn)
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

            frmFolderTree = new FrmFolderTree(setting, dpMain);
            FormBox.Add(frmFolderTree);

            frmErrorList = new FrmErrorList(setting);
            FormBox.Add(frmErrorList);

            frmLyrics = new FrmLyrics(setting, theme);
            FormBox.Add(frmLyrics);

            frmSien = new FrmSien(this, setting);
            FormBox.Add(frmSien);

            if (string.IsNullOrEmpty(setting.dockingState))
            {
                frmPartCounter.Show(dpMain, DockState.DockLeft);
                frmLog.Show(dpMain, DockState.DockBottom);
                frmFolderTree.Show(dpMain, DockState.DockLeft);
                frmErrorList.Show(dpMain, DockState.DockBottom);
                frmLyrics.Show(dpMain, DockState.DockTop);
                frmSien.Show(dpMain, DockState.DockRight);
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
                    if (frmSien.ParentForm == null) frmSien.Show(dpMain, DockState.DockRight);
                }
                catch (Exception ex)
                {
                    log.ForcedWrite(ex);

                    frmPartCounter.Show(dpMain, WeifenLuo.WinFormsUI.Docking.DockState.DockLeft);
                    frmLog.Show(dpMain, WeifenLuo.WinFormsUI.Docking.DockState.DockBottom);
                    frmFolderTree.Show(dpMain, WeifenLuo.WinFormsUI.Docking.DockState.DockLeft);
                    frmErrorList.Show(dpMain, WeifenLuo.WinFormsUI.Docking.DockState.DockBottom);
                    frmLyrics.Show(dpMain, WeifenLuo.WinFormsUI.Docking.DockState.DockTop);
                    frmSien.Show(dpMain, WeifenLuo.WinFormsUI.Docking.DockState.DockRight);
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
            frmSien.parentUpdate = UpdateControl;

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

            for (int i = 0; i < 500; i++)
            {
                System.Threading.Thread.Sleep(1);
                Application.DoEvents();
            }

        }

        public void Finish()
        {
            try
            {
                this.Visible = false;
                log.ForcedWrite("終了処理開始");
                log.ForcedWrite("frmMain_FormClosing:STEP 00");

                timer.Enabled = false;

                midikbd.StopMIDIInMonitoring();

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
                    if (frmMixer != null)
                    {
                        setting.location.RMixer = new Rectangle(frmMixer.Location.X, frmMixer.Location.Y, 0, 0);
                    }
                }

                frmPartCounter.Close();

                if (frmSien != null) frmSien.Close();

                setting.Save();
            }
            catch(Exception ex)
            {
                log.ForcedWrite(ex);
            }
        }

        public bool InitPlayer(EnmFileFormat format, outDatum[] srcBuf, long jumpPointClock)
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

                DockContent ddc = (DockContent)dc;
                Document d = null;
                if (ddc != null)
                {
                    if (ddc.Tag is Document)
                    {
                        d = (Document)ddc.Tag;
                    }
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
                        if (d != null && d.editor == od.linePos.document)//d.gwiFullPath == od.linePos.srcMMLID)
                            od.linePos.col = ac.GetCharIndexFromLineColumnIndex(od.linePos.row, od.linePos.col);
                        else
                            od.linePos = null;

                    }
                }


                if (Audio.isPaused)
                {
                    Audio.Pause();
                }

                Audio.SetVGMBuffer(format, srcBuf, jumpPointClock);

                //for (int i = 0; i < 100; i++)
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
                Audio.mmlParams.Init(isTrace
                    , format == EnmFileFormat.VGM
                    ? EnmMmlFileFormat.GWI
                    : (
                        format == EnmFileFormat.XGM
                        ? EnmMmlFileFormat.GWI
                        : (
                            format == EnmFileFormat.ZGM
                            ? EnmMmlFileFormat.GWI
                            : (
                                format == EnmFileFormat.MUB
                                ? EnmMmlFileFormat.MUC
                                : (
                                    format == EnmFileFormat.MUC
                                    ? EnmMmlFileFormat.MUC
                                    : (
                                        format == EnmFileFormat.M
                                        ? EnmMmlFileFormat.MML
                                        : EnmMmlFileFormat.unknown
                                        )
                                    )
                                )
                            )
                        )
                    , midikbd
                    );
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

        public bool InitPlayer(EnmFileFormat format, musicDriverInterface.MmlDatum[] md)
        {
            if (format == EnmFileFormat.MUB)
            {
                if (mucom == null) return false;
            }
            else if (format == EnmFileFormat.M)
            {
                if (pmdmng == null) return false;
            }
            else if (format == EnmFileFormat.MDR)
            {
                if (mdmng == null) return false;
            }

            if (md == null || md.Length < 1) return false;

            try
            {
                IDockContent dc = GetActiveDockContent();
                Sgry.Azuki.WinForms.AzukiControl ac = null;
                if (dc != null && (dc is FrmEditor))
                {
                    ac = ((FrmEditor)dc).azukiControl;
                }
                string fn = ((FrmEditor)dc).document.gwiFullPath;

                if (Audio.flgReinit) flgReinit = true;
                if (setting.other.InitAlways) flgReinit = true;
                //Reinit(setting);


                //rowとcolをazuki向けlinePosに変換する
                if (ac != null)
                {
                    foreach (musicDriverInterface.MmlDatum od in md)
                    {
                        if (od == null) continue;
                        if (od.linePos == null) continue;
                        if (od.linePos.row == -1 || od.linePos.col == -1) continue;
                        string src = od.linePos.srcMMLID;
                        string dst = fn;
                        if (src != null && src.Length > 0 && src[0] == '"' && src.Length > 1 && src[src.Length - 1] == '"')
                        {
                            src = src.Substring(1, src.Length - 2);
                            src = Path.GetFileName(src);
                        }
                        if (dst != null && dst.Length > 0) dst = Path.GetFileName(dst);

                        if (!string.IsNullOrEmpty(src) && src != dst) continue;

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

                Audio.SetVGMBuffer(format, md, wrkPath, fn);

                //for (int i = 0; i < 100; i++)
                {
                    Thread.Sleep(1);
                    Application.DoEvents();
                }

                if (md != null)
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
                EnmMmlFileFormat mff = EnmMmlFileFormat.unknown;
                switch (format)
                {
                    case EnmFileFormat.VGM:
                    case EnmFileFormat.XGM:
                    case EnmFileFormat.ZGM:
                        mff = EnmMmlFileFormat.GWI;
                        break;
                    case EnmFileFormat.MUB:
                        mff = EnmMmlFileFormat.MUC;
                        break;
                    case EnmFileFormat.M:
                        mff = EnmMmlFileFormat.MML;
                        break;
                    case EnmFileFormat.MDR:
                        mff = EnmMmlFileFormat.MDL;
                        break;
                }
                Audio.mmlParams.Init(isTrace, mff, midikbd);
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

                //TODO: 無意味なコール?
                Audio.Stop(0);

                ResumeNormalModeDisp();

                if (Audio.waveMode)
                {
                    return;
                }

                if (!Audio.Play(setting, doSkipStop, jumpSoloMode))
                {
                    try
                    {
                        //TODO: 無意味なコール?
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

        public void pause()
        {
            Audio.Pause();
        }

        public void fadeOut()
        {
            if (Audio.isPaused)
            {
                Audio.Pause();
            }

            Audio.Fadeout();
        }

        public void stop()
        {
            if (Compiling != 0)
            {
                return;
            }

            mmfControl mmf = new mmfControl(true, "MDPlayer", 1024 * 4);
            try
            {
                mmf.SendMessage(string.Join(" ", "STOP"));
            }
            catch (ArgumentOutOfRangeException)
            {
                ;
            }
            catch (FileNotFoundException)
            {
                ;
            }

            if (Audio.isStopped)
            {
                //もし既に停止しているならば
                return;
            }

            if (shift)
            {
                fadeOut();
                return;
            }

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
            if (!Audio.silent) Audio.silent = true;

            ResumeNormalModeDisp();

            while (Audio.sm.IsRunningAtDataSender())
            {
                Thread.Sleep(1);
            }
            Audio.sm.ClearData();
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
                Audio.StepPlay(Common.SampleRate / 10);
                //Audio.Pause();
                return;
            }

            //if (Audio.isStopped)
            //{
            //    play();
            //}

            Audio.Slow();
        }

        public void sien()
        {
            if (frmSien != null && !frmSien.IsDisposed)
            {
                frmSien.Close();
                frmSien = null;
            }
            else
            {
                if (frmSien != null)
                {
                    frmSien.Close();
                }

                frmSien = new FrmSien(this, setting);
                frmSien.Show(dpMain, DockState.DockRight);
            }
        }



        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateTraceInfo();
            UpdateScreenInfo();

            long w = Audio.EmuSeqCounter;
            double sec = (double)w / (double)Common.DataSequenceSampleRate;//.SampleRate;
            toolStripStatusLabel1.Text = string.Format("{0:d2}:{1:d2}.{2:d2}", (int)(sec / 60), (int)(sec % 60), (int)(sec * 100 % 100));

            CheckRemoteMemory();
        }

        private void CheckRemoteMemory()
        {
            String msg= mmfFMVoicePool.GetMessage();
            if (string.IsNullOrEmpty(msg)) return;

            int n = msg.IndexOf(':');
            string cmd = msg;
            if (n > 0)
            {
                cmd = msg.Substring(0, n);
            }
            if (cmd == "SendVoice")
            {
                WriteVoiceToDocument(msg.Substring(msg.IndexOf(":") + 1));
            }
            else
            {
                //MessageBox.Show("Received unknown command.");
            }
        }

        private void WriteVoiceToDocument(string v)
        {
            //現在のアクティブなドキュメントを取得する
            Document d = GetActiveDocument();
            if (d == null) return;

            switch (d.srcFileFormat)
            {
                case EnmMmlFileFormat.MUC:
                    v = Common.ConvertVoiceDataGWIToMucom(v);
                    d.editor.azukiControl.Document.Replace(v);
                    break;
                default:
                case EnmMmlFileFormat.GWI:
                    v += "\r\n";
                    d.editor.azukiControl.Document.Replace(v);
                    break;
            }

            d.editor.azukiControl.ScrollToCaret();
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
                if (frmMixer != null)
                {
                    if(frmMixer.DialogResult== DialogResult.Abort)
                    {
                        frmMixer = null;
                        TsmiShowMixer.Checked = (frmMixer != null);
                        TsmiShowMixer_Click(null, null);
                        return;
                    }
                }

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
                if (o == null) continue;
                if (!(o is DockContent)) continue;
                DockContent d = (DockContent)o;
                if (!(d is FrmEditor)) continue;
                Sgry.Azuki.WinForms.AzukiControl a = ((FrmEditor)d).azukiControl;
                a.ColorScheme.LineNumberBack = Color.FromArgb(setting.ColorScheme.Azuki_LineNumberBack_Normal);
                a.ColorScheme.LineNumberFore = Color.FromArgb(setting.ColorScheme.Azuki_LineNumberFore_Normal);
                a.Document.Unmark(0, a.Text.Length, 1);
                a.Document.Unmark(0, a.Text.Length, 2);
                a.IsReadOnly = false;
                a.Refresh();
            }

            this.statusStrip1.BackColor = Color.FromArgb(setting.ColorScheme.StatusStripBack_Normal);
        }

        private bool MarkUpTraceInfo(ConcurrentQueue<outDatum>[] ods, outDatum[] odos, int ch, FrmEditor fe, Sgry.Azuki.WinForms.AzukiControl ac)
        {
            bool flg = false;
            if (ods[ch] == null) return false;

            while (ods[ch].Count > 0)
            {
                outDatum od;
                lock (traceInfoLockObj) ods[ch].TryDequeue(out od);
                if (od == null) continue;
                string odfn = Path.GetFileName(od.linePos.srcMMLID);
                if (fe.Text != odfn && fe.Text != odfn + "*") continue;

                switch (od.type)
                {
                    case enmMMLType.Note:
                    case enmMMLType.Rest:
                    case enmMMLType.Lyric:
                        flg = MarkUpTraceInfo_Update(odos, ch, ac, od);
                        break;
                    case enmMMLType.TraceUpdateStack:
                        flg = MarkUpTraceInfo_TraceUpdateStack(ch, ac, flg, od);
                        break;
                    case enmMMLType.TraceLocate:
                        flg = MarkUpTraceInfo_TraceLocate(ac, flg, od);
                        break;
                }
            }

            return flg;
        }

        private bool MarkUpTraceInfo_Update(outDatum[] odos, int ch, AzukiControl ac, outDatum od)
        {
            outDatum odo = odos[ch];
            if (od == odo) return true;
            if (odo != null && od.linePos.col == odo.linePos.col) return true;

            bool flg;
            ac.GetLineColumnIndexFromCharIndex(od.linePos.col, out int i, out int c);
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
                ac.Document.Mark(od.linePos.col, od.linePos.col + Math.Max(od.linePos.length, 1), 1);
                odos[ch] = od;
            }
            flg = true;
            return flg;
        }

        private bool MarkUpTraceInfo_TraceUpdateStack(int ch, AzukiControl ac, bool flg, outDatum od)
        {
            while (lstOldAliesPos.Count < ch + 1)
                lstOldAliesPos.Add(new List<LinePos>());

            lock (traceInfoLockObj)
            {
                for (int i = 0; i < lstOldAliesPos[ch].Count; i++)
                {
                    if (lstOldAliesPos[ch][i].col == -1) continue;
                    try
                    {
                        ac.Document.Unmark(
                            lstOldAliesPos[ch][i].col
                            , lstOldAliesPos[ch][i].col + Math.Max(lstOldAliesPos[ch][i].length, 1)
                            , 2);
                    }
                    catch
                    {
                        ;//何もしない
                    }
                    lstOldAliesPos[ch][i].col = -1;
                    flg = true;
                }
            }

            LinePos[] lp = (LinePos[])od.args[0];
            if (lp == null) return flg;

            for (int i = 0; i < lp.Length; i++)
            {
                while (lstOldAliesPos[ch].Count < i + 1)
                    lstOldAliesPos[ch].Add(new LinePos());

                int ci;
                try
                {
                    ci = ac.GetCharIndexFromLineColumnIndex(lp[i].row, lp[i].col);
                    //if (lp.Length > 0) Console.WriteLine("* {0}", ci);
                }
                catch
                {
                    continue;
                }

                lstOldAliesPos[ch][i].col = ci;
                lstOldAliesPos[ch][i].length = lp[i].length;

                //マーク
                lock (traceInfoLockObj)
                    ac.Document.Mark(
                        ci
                        , ci + Math.Max(lp[i].length, 1)
                        , 2);
                flg = true;
            }

            return flg;
        }

        private bool MarkUpTraceInfo_TraceLocate(AzukiControl ac, bool flg, outDatum od)
        {
            //
            int sw = (int)od.args[1];
            if (sw != 1) return flg;

            int ci;
            if (oldTraceLoc != null)
            {
                ci = oldTraceLoc.col;
                //アンマーク
                lock (traceInfoLockObj) ac.Document.Unmark(
                    ci
                    , ci + Math.Max(oldTraceLoc.length, 1)
                    , 2);
                oldTraceLoc = null;
            }

            LinePos lp = ((MmlDatum)od.args[2]).linePos;
            ci = lp.col;
            //マーク
            lock (traceInfoLockObj) ac.Document.Mark(
                ci
                , ci + Math.Max(lp.length, 1)
                , 2);
            oldTraceLoc = lp;
            flg = true;

            return flg;
        }



        private List<List<LinePos>> lstOldAliesPos = new List<List<LinePos>>();
        private LinePos oldTraceLoc = null;
        private List<Tuple<Setting.ShortCutKey.ShortCutKeyInfo, Tuple<int, string, string[], string, string>>> scriptShortCutKey
            = new List<Tuple<Setting.ShortCutKey.ShortCutKeyInfo, Tuple<int, string, string[], string, string>>>();

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
            tssbFind.Visible = visible;
            tssbCompile.Visible = visible;
            tssbPlay.Visible = visible;
            //tssbTracePlay.Visible = visible;
            tssbM98.Visible = visible;
            tssbPause.Visible = visible;
            tssbStop.Visible = visible;
            tssbSlow.Visible = visible;
            tssbFast.Visible = visible;
            tssbMIDIKbd.Visible = visible;
            tssbExpAndMdp.Visible = visible;

            tssbOpen.DisplayStyle = style;
            tssbSave.DisplayStyle = style;
            tssbFind.DisplayStyle = style;
            tssbCompile.DisplayStyle = style;
            tssbPlay.DisplayStyle = style;
            //tssbTracePlay.DisplayStyle = style;
            tssbM98.DisplayStyle = style;
            tssbPause.DisplayStyle = style;
            tssbStop.DisplayStyle = style;
            tssbSlow.DisplayStyle = style;
            tssbFast.DisplayStyle = style;
            tssbMIDIKbd.DisplayStyle = style;
            tssbExpAndMdp.DisplayStyle = style;
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {

            //if (setting.location.RMain != System.Drawing.Rectangle.Empty)
            {
                if (SearchScreen(setting.location.RMain.X, setting.location.RMain.Y))
                {
                    this.Location = new Point(setting.location.RMain.X, setting.location.RMain.Y);
                    this.Size = new Size(setting.location.RMain.Width, setting.location.RMain.Height);
                }
                else
                {
                    this.Location = new Point(0, 0);
                    this.Size = new Size(this.MinimumSize.Width, this.MinimumSize.Height);
                }
            }


            this.Opacity = setting.other.Opacity / 100.0;

            //if (setting.location.RMixer != System.Drawing.Rectangle.Empty)
            {
                if (setting.location.ShowMixer)
                {
                    frmMixer = new FrmMixer(this, setting.other.Zoom, newParam.mixer);
                    frmMixer.Show();
                    if (SearchScreen(setting.location.RMixer.X, setting.location.RMixer.Y))
                    {
                        frmMixer.Location = new Point(setting.location.RMixer.X, setting.location.RMixer.Y);
                    }
                    else
                    {
                        frmMixer.Location = new Point(0, 0);
                    }
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

            //frmSien = new FrmSien(setting);
            //frmSien.parent = this;
            //frmSien.Show();

            mmfFMVoicePool = new mmfControl(false, "mml2vgmFMVoicePool", 1024 * 4);
        }

        private bool SearchScreen(int x, int y)
        {
            Screen[] scrn = Screen.AllScreens;
            foreach (Screen scr in scrn)
            {
                if (x >= scr.WorkingArea.Left && x <= scr.WorkingArea.Right)
                {
                    if (y >= scr.WorkingArea.Top && y <= scr.WorkingArea.Bottom)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void GetScripts(ToolStripMenuItem tsmiScript, ToolStripMenuItem tsmiTreeView, string path)
        {
            TreeNode tn = new TreeNode();
            SScript(tn, path);

            DivScripts(tsmiScript, tn, "FROMMENU");
            DivScripts(tsmiTreeView, tn, "FROMTREEVIEWCONTEXTMENU");
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
                    tn.Tag = new Tuple<int, string, string[], string, string>(-1, "", new string[] { "" }, ds.FullName, "");
                    SScript(tn, ds.FullName);
                    if (tn.Nodes.Count > 0) parent.Nodes.Add(tn);
                }
                foreach (FileInfo fi in dm.GetFiles())
                {
                    string[] scriptTitles = ScriptInterface.GetScriptTitles(fi.FullName);
                    string[] scriptTypes = ScriptInterface.GetScriptTypes(fi.FullName);
                    string[] scriptSupportFileExt = ScriptInterface.GetScriptSupportFileExt(fi.FullName);
                    string[] scriptShortCutKey = ScriptInterface.GetDefaultShortCutKey(fi.FullName);
                    for (int i = 0; i < scriptTitles.Length; i++)
                    {
                        TreeNode tn = new TreeNode(scriptTitles[i]);
                        tn.Tag = new Tuple<int, string, string[], string, string>(
                            i,
                            scriptTypes[i],
                            scriptSupportFileExt[i].Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries),
                            fi.FullName,
                            i < scriptShortCutKey.Length ? scriptShortCutKey[i] : null
                            );
                        parent.Nodes.Add(tn);
                    }
                }
            }
            catch { }
        }

        private void DivScripts(ToolStripMenuItem tsmi, TreeNode tn, string target)
        {
            foreach (TreeNode ctn in tn.Nodes)
            {
                ToolStripMenuItem ctsmi = new ToolStripMenuItem(ctn.Text);
                Tuple<int, string, string[], string, string> tpl = (Tuple<int, string, string[], string, string>)ctn.Tag;
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

                if (!string.IsNullOrEmpty(tpl.Item5))
                {
                    //
                    bool[] shift = new bool[2] { false, false };
                    bool[] ctrl = new bool[2] { false, false };
                    bool[] alt = new bool[2] { false, false };
                    string[] key = new string[2] { "", "" };

                    string[] strks = tpl.Item5.ToUpper().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < 2; i++)
                    {
                        if (i >= strks.Length) continue;
                        string strk = strks[i];
                        string[] keys = strk.Trim().Split(new string[] { "+" }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string k in keys)
                        {
                            string kt = k.Trim();
                            if (kt == "SHIFT") shift[i] = true;
                            else if (kt == "CTRL") ctrl[i] = true;
                            else if (kt == "ALT") alt[i] = true;
                            else key[i] = kt;
                        }
                    }

                    Setting.ShortCutKey.ShortCutKeyInfo info = new Setting.ShortCutKey.ShortCutKeyInfo(
                        tpl.Item1//number
                        , tpl.Item2//func
                        , shift[0]
                        , ctrl[0]
                        , alt[0]
                        , key[0]
                        , shift[1]
                        , ctrl[1]
                        , alt[1]
                        , key[1]
                        );

                    scriptShortCutKey.Add(
                        new Tuple<Setting.ShortCutKey.ShortCutKeyInfo, Tuple<int, string, string[], string, string>>(
                            info
                            , tpl
                            )
                        );
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

            Tuple<int, string, string[], string, string> tpl
                = (Tuple<int, string, string[], string, string>)((ToolStripMenuItem)sender).Tag;
            string fn = tpl.Item4;

            List<string> lstFullPath = new List<string>();
            frmFolderTree.GetCheckTreeNodesFullPath(ref lstFullPath, frmFolderTree.tvFolderTree.Nodes);

            Mml2vgmInfo info = new Mml2vgmInfo();
            info.parent = this;
            info.name = "";
            info.document = d;
            info.fileNamesFull = lstFullPath.ToArray();

            string cd = Directory.GetCurrentDirectory();
            ScriptInterface.run(fn, info, tpl.Item1);
            Directory.SetCurrentDirectory(cd);
        }

        private ChannelInfo GetCurrentChannelInfo()
        {
            ChannelInfo chi = null;
            //Document d = GetActiveDocument();

            //if (d == null)
            //{
            //    firstPlay();
            //    return defaultChannelInfo;
            //}

            ////int ci = d.editor.azukiControl.CaretIndex;
            ////int st = d.editor.azukiControl.GetLineHeadIndexFromCharIndex(ci);
            ////string line = d.editor.azukiControl.GetTextInRange(st, ci).TrimStart();
            ////if (line == "" || line[0] != '\'')
            ////{
            ////    return defaultChannelInfo;
            ////}

            ////演奏中はコンパイルしない
            //if (!Audio.sm.IsRunningAtDataMaker())
            //{
            //    Compile(true, false, true, true, false);
            //    while (Compiling != 0)
            //    {
            //        Thread.Sleep(0);
            //        Application.DoEvents();
            //    }
            //    Audio.sm.ResetMode(SendMode.MML);
            //}


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
            if (frmErrorList.DockState != DockState.Float)
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

        private void tssbCompile_ButtonClick_1(object sender, EventArgs e)
        {
            Comp();
        }

        private void Comp()
        {
            IDockContent dc = GetActiveDockContent();
            if (dc == null) return;
            if (!(dc is FrmEditor)) return;
            compileManager.RequestCompile(((FrmEditor)dc).document, ((FrmEditor)dc).azukiControl.Text + "");
            compileManager.RequestPlayBack(((FrmEditor)dc).document, cbPlayBack);
        }

        private void cbPlayBack(CompileManager.queItem qi)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<CompileManager.queItem>(cbPlayBack), new object[] { qi });
                return;
            }

            if (qi.doc.dstFileFormat == EnmFileFormat.MUB)
            {
                mubData = (musicDriverInterface.MmlDatum[])qi.doc.compiledData;
                doPlay = true;
                finishedCompileMUC();
            }
            else
            {
                mv = (Mml2vgm)qi.doc.compiledData;
                doPlay = true;
                args = new string[2] { qi.doc.gwiFullPath, "" };
                finishedCompileGWI();
            }
        }

        private void tsmiImport_D88toMuc_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = "D88ファイル(*.d88)|*.d88|すべてのファイル(*.*)|*.*";
            ofd.Title = "D88ファイルを選択してください";
            ofd.RestoreDirectory = true;

            if (ofd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            List<N88FileAtr> directory;
            N88Disk n88;
            try
            {
                byte[] d88b = File.ReadAllBytes(ofd.FileName);
                D88 d88 = new D88(d88b);
                n88 = new N88Disk(d88);
                directory = n88.GetDirectories();
                n88.GetFAT();
            }
            catch
            {
                MessageBox.Show("D88ファイルのオープンに失敗しました。", "インポート失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            FormN88Files fnf = new FormN88Files();

            for (int row = 0; row < Math.Ceiling(directory.Count / 5.0); row++)
            {
                object[] celObj = new object[5];
                for (int col = 0; col < 5; col++)
                {
                    if (row * 5 + col < directory.Count)
                    {
                        try
                        {
                            N88FileAtr atr = directory[row * 5 + col];
                            string f = atr.fn.Replace(".", ((atr.atr & 1) != 0 ? "*" : ((atr.atr & 0x80) == 0 ? " " : ".")));
                            celObj[col] = string.Format(
                                " {0} {1,3:D} "
                                , f
                                , n88.GetUseClusterFromFAT(atr.cluster));
                        }
                        catch { }
                    }
                }
                fnf.dgvFiles.Rows.Add(celObj);
            }

            DialogResult res = fnf.ShowDialog();
            if (res == DialogResult.Cancel) return;
            if (fnf.n == -1) return;
            if (fnf.n >= directory.Count) return;

            string[] sbuf;
            string fileName = directory[fnf.n].fn;
            try
            {
                string ss = Encoding.GetEncoding("shift_jis").GetString(n88.GetFile(Path.GetFileName(fileName), 0));
                sbuf = ss.Split(new string[] { "\r\n" }, StringSplitOptions.None);

                for (int i = 0; i < sbuf.Length; i++)
                {
                    if (sbuf[i].IndexOf("'") != -1) sbuf[i] = sbuf[i].Substring(sbuf[i].IndexOf("'") + 1);
                }
            }
            catch (Exception ex)
            {
                log.Write(ex.Message);
                MessageBox.Show("D88内のbasファイルのオープンに失敗しました。", "インポート失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Document dc = new Document(setting, EnmMmlFileFormat.MUC);//, frmSien);
            fileName = fileName.Trim();
            if (fileName.Length > 1 && fileName[fileName.Length - 1] == '.') fileName = fileName.Substring(0, fileName.Length - 1);
            fileName += ".muc";
            dc.InitOpen(fileName, sbuf, EnmMmlFileFormat.MUC);
            dc.editor.Show(dpMain, DockState.Document);
            dc.editor.main = this;
            dc.editor.document = dc;

            frmFolderTree.tvFolderTree.Nodes.Clear();
            frmFolderTree.tvFolderTree.Nodes.Add(dc.gwiTree);
            string path1 = Path.GetDirectoryName(dc.gwiFullPath);
            path1 = string.IsNullOrEmpty(path1) ? dc.gwiFullPath : path1;
            frmFolderTree.basePath = path1;

            FormBox.Add(dc.editor);
            DocumentBox.Add(dc);
            AddGwiFileHistory(fileName);
            UpdateGwiFileHistory();

        }

        private void tsmiImport_MIDtoGWI_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = "Standard MIDファイル(*.mid)|*.mid|すべてのファイル(*.*)|*.*";
            ofd.Title = "MIDファイルを選択してください";
            ofd.RestoreDirectory = true;

            if (ofd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            try
            {
                byte[] mid = File.ReadAllBytes(ofd.FileName);
                MIDIImporter mi = new MIDIImporter();
                string[] mml = mi.Import(mid);
                string fileName = ofd.FileName;

                Document dc = new Document(setting, EnmMmlFileFormat.GWI);//, frmSien);
                fileName = fileName.Trim();
                if (fileName.Length > 1 && fileName[fileName.Length - 1] == '.') fileName = fileName.Substring(0, fileName.Length - 1);
                if (fileName.IndexOf(".") >= 0)
                {
                    fileName = fileName.Substring(0, fileName.IndexOf("."));
                }
                fileName += ".gwi";
                dc.InitOpen(fileName, mml, EnmMmlFileFormat.GWI);
                dc.editor.Show(dpMain, DockState.Document);
                dc.editor.main = this;
                dc.editor.document = dc;

                frmFolderTree.tvFolderTree.Nodes.Clear();
                frmFolderTree.tvFolderTree.Nodes.Add(dc.gwiTree);
                string path1 = Path.GetDirectoryName(dc.gwiFullPath);
                path1 = string.IsNullOrEmpty(path1) ? dc.gwiFullPath : path1;
                frmFolderTree.basePath = path1;

                FormBox.Add(dc.editor);
                DocumentBox.Add(dc);
                AddGwiFileHistory(fileName);
                UpdateGwiFileHistory();
            }
            catch
            {
                MessageBox.Show("D88ファイルのオープンに失敗しました。", "インポート失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }


        private void tsmiExport_MuctoD88_Click(object sender, EventArgs e)
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
            if (Path.GetExtension(d.gwiFullPath).ToLower() != ".muc")
            {
                MessageBox.Show("この機能は.mucファイル専用です。", "Export Muc to D88", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            string fnD88 = d.gwiFullPath;
            if (fnD88.Length > 0 && fnD88[fnD88.Length - 1] == '*')
            {
                fnD88 = fnD88.Substring(0, fnD88.Length - 1);
            }
            fnD88 = Path.Combine(Path.GetDirectoryName(fnD88), Path.GetFileNameWithoutExtension(fnD88) + ".d88");

            sfd.FileName = fnD88;
            string path1 = System.IO.Path.GetDirectoryName(fnD88);
            path1 = string.IsNullOrEmpty(path1) ? fnD88 : path1;
            sfd.InitialDirectory = path1;
            sfd.Filter = "D88ファイル(*.d88)|*.d88|すべてのファイル(*.*)|*.*";
            sfd.Title = "エクスポート";
            sfd.RestoreDirectory = true;
            sfd.OverwritePrompt = false;
            if (sfd.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            fnD88 = sfd.FileName;
            if (Path.GetExtension(fnD88) == "")
            {
                fnD88 = Path.GetFileNameWithoutExtension(fnD88) + ".d88";
            }
            if (!File.Exists(fnD88))
            {
                MessageBox.Show("指定したD88ファイルが見つかりませんでした。", "Export Muc to D88", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            String backFn = fnD88 + ".bak";
            String backFnb = backFn;
            int i = 1;
            while (File.Exists(backFnb))
            {
                backFnb = backFn + string.Format("({0})", i++);
            }
            backFn = backFnb;
            File.Copy(fnD88, backFn);

            List<N88FileAtr> directory;
            N88Disk n88;
            try
            {
                byte[] d88b = File.ReadAllBytes(fnD88);
                D88 d88 = new D88(d88b);
                n88 = new N88Disk(d88);
                directory = n88.GetDirectories();
                n88.GetFAT();
            }
            catch
            {
                MessageBox.Show("D88ファイルのオープンに失敗しました。", "インポート失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string[] text = d.editor.azukiControl.Text.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            for (i = 0; i < Math.Min(65535, text.Length); i++)
            {
                text[i] = string.Format("{0} '{1}", i + 1, text[i]);
            }
            string txt = string.Join("\r\n", text);

            byte[] buf = n88.SetFile(Path.GetFileName(d.gwiFullPath), Encoding.GetEncoding("shift_jis").GetBytes(txt), 3, 0);
            File.WriteAllBytes(fnD88, buf);

        }

        private void tsmiExport_toDriverFormat_Click(object sender, EventArgs e)
        {
            try
            {
                DockContent dc = (DockContent)GetActiveDockContent();
                Document d = null;
                string outFn = "";
                if (dc != null)
                {
                    if (dc.Tag is Document)
                    {
                        d = (Document)dc.Tag;
                    }
                }

                if (d == null) return;

                if (Path.GetExtension(d.gwiFullPath).ToLower() == ".muc")
                {
                    ExportMub(d, ref outFn);
                    return;
                }
                if (Path.GetExtension(d.gwiFullPath).ToLower() == ".mml")
                {
                    ExportM(d, ref outFn);
                    return;
                }

                if (Path.GetExtension(d.gwiFullPath).ToLower() == ".gwi")
                {
                    ExportVgmXgmZgm(d, ref outFn);
                    return;
                }

                if (Path.GetExtension(d.gwiFullPath).ToLower() == ".mdl")
                {
                    ExportMdr(d, ref outFn);
                    return;
                }

            }
            catch (Exception)
            {
                MessageBox.Show("エクスポート処理に失敗しました。", "エクスポート失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tsmiExport_toDriverFormatAndPlay_Click(object sender, EventArgs e)
        {
            Document d = null;
            string outFn = "";
            List<byte> buf = new List<byte>();

            try
            {
                DockContent dc = (DockContent)GetActiveDockContent();
                if (dc != null)
                {
                    if (dc.Tag is Document)
                    {
                        d = (Document)dc.Tag;
                    }
                }

                if (d == null) return;

                //コンパイル実施
                Compile(false, false, false, false, true);
                while (Compiling != 0) { Application.DoEvents(); }//待ち合わせ

                if (Path.GetExtension(d.gwiFullPath).ToLower() == ".muc")
                {
                    foreach (MmlDatum md in mubData) buf.Add(md != null ? (byte)md.dat : (byte)0);
                    outFn = Path.ChangeExtension(d.gwiFullPath, ".mub");
                }
                else if (Path.GetExtension(d.gwiFullPath).ToLower() == ".mml")
                {
                    foreach (MmlDatum md in mData) buf.Add((byte)md.dat);
                    outFn = Path.ChangeExtension(d.gwiFullPath, ".m");
                }
                else if (Path.GetExtension(d.gwiFullPath).ToLower() == ".gwi")
                {
                    string sf = Path.Combine(
                        Common.GetApplicationDataFolder(true)
                        , "temp"
                        , Path.GetFileNameWithoutExtension(Path.GetFileName(d.gwiFullPath))
                            + (FileInformation.format == enmFormat.VGM ? ".vgm"
                                : (FileInformation.format == enmFormat.XGM ? ".xgm" : ".zgm"))
                        );
                    buf.AddRange(File.ReadAllBytes(sf));
                    outFn = Path.ChangeExtension(d.gwiFullPath,
                        (FileInformation.format == enmFormat.VGM ? ".vgm"
                                : (FileInformation.format == enmFormat.XGM ? ".xgm" : ".zgm")));
                }

            }
            catch (Exception)
            {
                MessageBox.Show("エクスポート処理に失敗しました。", "エクスポート失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {

                if (mml2vgmMmf != null) mml2vgmMmf.Close();
                mml2vgmMmf = new mmfControl(false, "mml2vgmIDEmmf", buf.Count);
                mml2vgmMmf.SetBytes(buf.ToArray());

                if (mmf != null) mmf.Close();
                mmf = new mmfControl(true, "MDPlayer", 1024 * 4);
                mmf.SendMessage(string.Join(" ", "SPLAY", "mml2vgmIDEmmf", buf.Count.ToString(), outFn));

            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("ファイル名が長すぎます。", "演奏開始失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("MDPlayerが見つかりませんでした。", "演奏開始失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private mmfControl mmf = null;
        private mmfControl mml2vgmMmf = null;
        private mmfControl mmfFMVoicePool = null;

        private void tsmiExport_MuctoVGM_Click(object sender, EventArgs e)
        {
            IDockContent dc = GetActiveDockContent();
            if (dc == null) return;
            if (!(dc is FrmEditor)) return;
            if (((FrmEditor)dc).document.srcFileFormat != EnmMmlFileFormat.MUC) return;

            string tempPath = Path.Combine(Common.GetApplicationDataFolder(true), "temp", Path.GetFileName(((Document)((FrmEditor)dc).Tag).gwiFullPath));

            compileManager.RequestCompile(((FrmEditor)dc).document, ((FrmEditor)dc).azukiControl.Text + "", tempPath);
            compileManager.RequestPlayBack(((FrmEditor)dc).document, playToVGMCB);
        }

        private void tsmiExport_toWaveFile_Click(object sender, EventArgs e)
        {
            IDockContent dc = GetActiveDockContent();
            if (dc == null) return;
            if (!(dc is FrmEditor)) return;

            string tempPath = Path.Combine(Common.GetApplicationDataFolder(true), "temp", Path.GetFileName(((Document)((FrmEditor)dc).Tag).gwiFullPath));

            compileManager.RequestCompile(((FrmEditor)dc).document, ((FrmEditor)dc).azukiControl.Text + "", tempPath);
            if (sender == tsmiExport_toMp3File) ((FrmEditor)dc).document.isMp3 = true;
            compileManager.RequestPlayBack(((FrmEditor)dc).document, playToWavCB);
        }

        private void tsmiExport_toWaveFileAllChSolo_Click(object sender, EventArgs e)
        {
            IDockContent dc = GetActiveDockContent();
            if (dc == null) return;
            if (!(dc is FrmEditor)) return;

            string tempPath = Path.Combine(Common.GetApplicationDataFolder(true), "temp", Path.GetFileName(((Document)((FrmEditor)dc).Tag).gwiFullPath));
            if (Path.GetExtension(tempPath) != ".muc")
            {
                MessageBox.Show(".mucファイルのみです!");
                return;
            }

            compileManager.RequestCompile(((FrmEditor)dc).document, ((FrmEditor)dc).azukiControl.Text + "", tempPath);

            compileManager.RequestPlayBack(((FrmEditor)dc).document, playToWavCBAllChSolo);

        }

        private void tsmiExport_toMidiFile_Click(object sender, EventArgs e)
        {
            IDockContent dc = GetActiveDockContent();
            if (dc == null) return;
            if (!(dc is FrmEditor)) return;

            FrmEditor fe = (FrmEditor)dc;
            Document doc = (Document)(fe.Tag);
            if (doc.srcFileFormat != EnmMmlFileFormat.GWI)
            {
                MessageBox.Show("This mml file is not support.");
                return;
            }

            string tempPath = Path.Combine(Common.GetApplicationDataFolder(true), "temp", Path.GetFileName(((Document)((FrmEditor)dc).Tag).gwiFullPath));

            compileManager.RequestCompile(((FrmEditor)dc).document, ((FrmEditor)dc).azukiControl.Text + "", tempPath);
            compileManager.RequestPlayBack(((FrmEditor)dc).document, playToMIDCB);
        }

        private void playToWavCB(CompileManager.queItem qi)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<CompileManager.queItem>(playToWavCB), new object[] { qi });
                return;
            }

            //コンパイルエラーが発生する場合はエクスポート処理中止
            if (qi.doc.errBox != null && qi.doc.errBox.Length > 0)
            {
                Audio.Stop(SendMode.Both);
                MessageBox.Show("コンパイル時にエラーが発生したため、エクスポート処理を中止しました。", "エクスポート失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            //出力ファイル名の問い合わせ
            SaveFileDialog sfd = new SaveFileDialog();
            string fnWav = qi.doc.gwiFullPath;
            if (fnWav.Length > 0 && fnWav[fnWav.Length - 1] == '*')
            {
                fnWav = fnWav.Substring(0, fnWav.Length - 1);
            }
            if (!qi.doc.isMp3)
            {
                fnWav = Path.Combine(Path.GetDirectoryName(fnWav), Path.GetFileNameWithoutExtension(fnWav) + ".wav");
                sfd.FileName = fnWav;
                sfd.Filter = "WAVファイル(*.wav)|*.wav|すべてのファイル(*.*)|*.*";
            }
            else
            {
                fnWav = Path.Combine(Path.GetDirectoryName(fnWav), Path.GetFileNameWithoutExtension(fnWav) + ".mp3");
                sfd.FileName = fnWav;
                sfd.Filter = "MP3ファイル(*.mp3)|*.mp3|すべてのファイル(*.*)|*.*";
            }
            string path1 = System.IO.Path.GetDirectoryName(fnWav);
            path1 = string.IsNullOrEmpty(path1) ? fnWav : path1;
            sfd.InitialDirectory = path1;
            sfd.Title = "エクスポート";
            sfd.RestoreDirectory = true;
            sfd.OverwritePrompt = false;

            if (sfd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            exportWav = true;
            int Latency = setting.outputDevice.Latency;
            int WaitTime = setting.outputDevice.WaitTime;
            int LatencyEmu = setting.LatencyEmulation;
            int LatencySCCI = setting.LatencySCCI;
            setting.outputDevice.Latency = 0;
            setting.outputDevice.WaitTime = 0;
            setting.LatencyEmulation = 0;
            setting.LatencySCCI = 0;

            try
            {
                if (qi.doc.dstFileFormat == EnmFileFormat.MUB)
                {
                    mubData = (MmlDatum[])qi.doc.compiledData;
                    doPlay = true;
                    compileTargetDocument = qi.doc;
                    finishedCompileMUC();
                }
                else if (qi.doc.dstFileFormat == EnmFileFormat.M)
                {
                    mData = (MmlDatum[])qi.doc.compiledData;
                    doPlay = true;
                    finishedCompileMML();
                }
                else if (qi.doc.dstFileFormat == EnmFileFormat.MDR)
                {
                    mData = (MmlDatum[])qi.doc.compiledData;
                    doPlay = true;
                    compileTargetDocument = qi.doc;
                    finishedCompileMDL();
                }
                else
                {
                    mv = (Mml2vgm)qi.doc.compiledData;
                    doPlay = true;
                    args = new string[2] { qi.doc.gwiFullPath, "" };
                    finishedCompileGWI();
                }

                Audio.waveMode = true;
                Audio.waveModeAbort = false;
                Audio.Stop(SendMode.Both);
                exportWav = false;

                bool res = Audio.PlayToWav(setting, sfd.FileName);
                if (!res)
                {
                    MessageBox.Show("失敗");
                    return;
                }

                FrmProgress fp = new FrmProgress();
                fp.ShowDialog();

                MessageBox.Show("完了");
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("失敗\r\n{0}\r\n{1}\r\n", ex.Message, ex.StackTrace));
            }
            finally
            {
                setting.outputDevice.Latency = Latency;
                setting.outputDevice.WaitTime = WaitTime;
                setting.LatencyEmulation = LatencyEmu;
                setting.LatencySCCI = LatencySCCI;
            }

        }

        private void playToWavCBAllChSolo(CompileManager.queItem qi)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<CompileManager.queItem>(playToWavCBAllChSolo), new object[] { qi });
                return;
            }

            //コンパイルエラーが発生する場合はエクスポート処理中止
            if (qi.doc.errBox != null && qi.doc.errBox.Length > 0)
            {
                Audio.Stop(SendMode.Both);
                MessageBox.Show("コンパイル時にエラーが発生したため、エクスポート処理を中止しました。", "エクスポート失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            //出力ファイル名の問い合わせ
            SaveFileDialog sfd = new SaveFileDialog();
            string fnWav = qi.doc.gwiFullPath;
            if (fnWav.Length > 0 && fnWav[fnWav.Length - 1] == '*')
            {
                fnWav = fnWav.Substring(0, fnWav.Length - 1);
            }
            if (!qi.doc.isMp3)
            {
                fnWav = Path.Combine(Path.GetDirectoryName(fnWav), Path.GetFileNameWithoutExtension(fnWav) + ".wav");
                sfd.FileName = fnWav;
                sfd.Filter = "WAVファイル(*.wav)|*.wav|すべてのファイル(*.*)|*.*";
            }
            else
            {
                fnWav = Path.Combine(Path.GetDirectoryName(fnWav), Path.GetFileNameWithoutExtension(fnWav) + ".mp3");
                sfd.FileName = fnWav;
                sfd.Filter = "MP3ファイル(*.mp3)|*.mp3|すべてのファイル(*.*)|*.*";
            }
            string path1 = System.IO.Path.GetDirectoryName(fnWav);
            path1 = string.IsNullOrEmpty(path1) ? fnWav : path1;
            sfd.InitialDirectory = path1;
            sfd.Title = "エクスポート";
            sfd.RestoreDirectory = true;
            sfd.OverwritePrompt = false;

            if (sfd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            exportWav = true;
            int Latency = setting.outputDevice.Latency;
            int WaitTime = setting.outputDevice.WaitTime;
            int LatencyEmu = setting.LatencyEmulation;
            int LatencySCCI = setting.LatencySCCI;
            setting.outputDevice.Latency = 0;
            setting.outputDevice.WaitTime = 0;
            setting.LatencyEmulation = 0;
            setting.LatencySCCI = 0;

            try
            {
                if (qi.doc.dstFileFormat == EnmFileFormat.MUB)
                {
                    mubData = (MmlDatum[])qi.doc.compiledData;
                    doPlay = true;
                    compileTargetDocument = qi.doc;
                    finishedCompileMUC();
                }
                else if (qi.doc.dstFileFormat == EnmFileFormat.M)
                {
                    mData = (MmlDatum[])qi.doc.compiledData;
                    doPlay = true;
                    finishedCompileMML();
                }
                else if (qi.doc.dstFileFormat == EnmFileFormat.MDR)
                {
                    mData = (MmlDatum[])qi.doc.compiledData;
                    doPlay = true;
                    compileTargetDocument = qi.doc;
                    finishedCompileMDL();
                }
                else
                {
                    mv = (Mml2vgm)qi.doc.compiledData;
                    doPlay = true;
                    args = new string[2] { qi.doc.gwiFullPath, "" };
                    finishedCompileGWI();
                }


                musicDriverInterface.CompilerInfo ci = mucom.GetCompilerInfo();
                List<Tuple<int, int, int>> useCh = new List<Tuple<int, int, int>>();
                for(int n = 0; n < ci.totalCount.Count; n++)
                {
                    if (ci.totalCount[n] == 0) continue;
                    if (ci.formatType == "mub")
                    {
                        useCh.Add(new Tuple<int, int, int>(-1, -1, n));
                    }
                    else
                    {
                        useCh.Add(new Tuple<int, int, int>(n / (11 * 10), (n % (11 * 10)) / 10, n % 10));
                    }
                }

                foreach (Tuple<int, int, int> ch in useCh)
                {
                    Audio.waveMode = true;
                    Audio.waveModeAbort = false;
                    Audio.Stop(SendMode.Both);
                    exportWav = false;

                    string fn = GetSoloChFileName(sfd.FileName, ch);
                    bool res = Audio.PlayToWavSolo(setting, fn, ch);
                    if (!res)
                    {
                        MessageBox.Show("失敗");
                        return;
                    }

                    FrmProgress fp = new FrmProgress();
                    fp.ShowDialog();
                }

                MessageBox.Show("完了");
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("失敗\r\n{0}\r\n{1}\r\n", ex.Message, ex.StackTrace));
            }
            finally
            {
                setting.outputDevice.Latency = Latency;
                setting.outputDevice.WaitTime = WaitTime;
                setting.LatencyEmulation = LatencyEmu;
                setting.LatencySCCI = LatencySCCI;
            }

        }

        private string GetSoloChFileName(string fileName, Tuple<int, int, int> ch)
        {
            string[] chipCh = new string[]
            {
                "ABCDEFGHIJK",
                "LMNOPQRSTUV",
                "abcdefghijk",
                "lmnopqrstuv",
                "WXYZwxyz"
            };

            string ret;
            if (ch.Item1 < 0)
            {
                ret = string.Format("{0}_{1}.{2}"
                    , Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName))
                    , chipCh[0][ch.Item3]
                    , Path.GetExtension(fileName)
                    );
            }
            else
            {
                ret = string.Format("{0}_{1}{2}{3}.{4}"
                    , Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName))
                    , ch.Item1 == 0 ? "OPNA1_" :
                    (ch.Item1 == 1 ? "OPNA2_" :
                    (ch.Item1 == 2 ? "OPNB1_" :
                    (ch.Item1 == 3 ? "OPNB2_" :
                    string.Format("OPM_{0}_",ch.Item2)
                    )))
                    , chipCh[ch.Item1][ch.Item2]
                    , ch.Item3
                    , Path.GetExtension(fileName)
                    );
            }

            return ret;
        }

        private void playToMIDCB(CompileManager.queItem qi)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<CompileManager.queItem>(playToMIDCB), new object[] { qi });
                return;
            }

            //コンパイルエラーが発生する場合はエクスポート処理中止
            if (qi.doc.errBox != null && qi.doc.errBox.Length > 0)
            {
                Audio.Stop(SendMode.Both);
                MessageBox.Show("コンパイル時にエラーが発生したため、エクスポート処理を中止しました。", "エクスポート失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (qi.doc.dstFileFormat != EnmFileFormat.ZGM)
            {
                Audio.Stop(SendMode.Both);
                MessageBox.Show("MIDエクスポートはZGM形式のみです。", "エクスポート失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //出力ファイル名の問い合わせ
            SaveFileDialog sfd = new SaveFileDialog();
            string fnMid = qi.doc.gwiFullPath;
            if (fnMid.Length > 0 && fnMid[fnMid.Length - 1] == '*')
            {
                fnMid = fnMid.Substring(0, fnMid.Length - 1);
            }
            fnMid = Path.Combine(Path.GetDirectoryName(fnMid), Path.GetFileNameWithoutExtension(fnMid) + ".mid");
            sfd.FileName = fnMid;
            string path1 = System.IO.Path.GetDirectoryName(fnMid);
            path1 = string.IsNullOrEmpty(path1) ? fnMid : path1;
            sfd.InitialDirectory = path1;
            sfd.Filter = "Standard MIDファイル(*.mid)|*.mid|すべてのファイル(*.*)|*.*";
            sfd.Title = "エクスポート";
            sfd.RestoreDirectory = true;
            sfd.OverwritePrompt = false;
            if (sfd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            exportMid = true;
            int Latency = setting.outputDevice.Latency;
            int WaitTime = setting.outputDevice.WaitTime;
            int LatencyEmu = setting.LatencyEmulation;
            int LatencySCCI = setting.LatencySCCI;
            setting.outputDevice.Latency = 0;
            setting.outputDevice.WaitTime = 0;
            setting.LatencyEmulation = 0;
            setting.LatencySCCI = 0;

            try
            {
                mv = (Mml2vgm)qi.doc.compiledData;
                doPlay = true;
                args = new string[2] { qi.doc.gwiFullPath, "" };
                finishedCompileGWI();

                Audio.waveMode = true;
                Audio.waveModeAbort = false;
                Audio.Stop(SendMode.Both);
                exportMid = false;

                bool res = Audio.PlayToMid(setting, sfd.FileName);
                if (!res)
                {
                    MessageBox.Show("失敗");
                    return;
                }

                FrmProgress fp = new FrmProgress();
                fp.ShowDialog();

                MessageBox.Show("完了");
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("失敗\r\n{0}\r\n{1}\r\n", ex.Message, ex.StackTrace));
            }
            finally
            {
                setting.outputDevice.Latency = Latency;
                setting.outputDevice.WaitTime = WaitTime;
                setting.LatencyEmulation = LatencyEmu;
                setting.LatencySCCI = LatencySCCI;
            }

        }

        private void playToVGMCB(CompileManager.queItem qi)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<CompileManager.queItem>(playToVGMCB), new object[] { qi });
                return;
            }

            //コンパイルエラーが発生する場合はエクスポート処理中止
            if (qi.doc.errBox != null && qi.doc.errBox.Length > 0)
            {
                Audio.Stop(SendMode.Both);
                MessageBox.Show("コンパイル時にエラーが発生したため、エクスポート処理を中止しました。", "エクスポート失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (qi.doc.dstFileFormat != EnmFileFormat.MUB)
            {
                Audio.Stop(SendMode.Both);
                MessageBox.Show("VGMエクスポートはMUB形式のみです。", "エクスポート失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //出力ファイル名の問い合わせ
            SaveFileDialog sfd = new SaveFileDialog();
            string fnVgm = qi.doc.gwiFullPath;
            if (fnVgm.Length > 0 && fnVgm[fnVgm.Length - 1] == '*')
            {
                fnVgm = fnVgm.Substring(0, fnVgm.Length - 1);
            }
            fnVgm = Path.Combine(Path.GetDirectoryName(fnVgm), Path.GetFileNameWithoutExtension(fnVgm) + ".vgm");
            sfd.FileName = fnVgm;
            string path1 = System.IO.Path.GetDirectoryName(fnVgm);
            path1 = string.IsNullOrEmpty(path1) ? fnVgm : path1;
            sfd.InitialDirectory = path1;
            sfd.Filter = "VGMファイル(*.vgm)|*.vgm|すべてのファイル(*.*)|*.*";
            sfd.Title = "エクスポート";
            sfd.RestoreDirectory = true;
            sfd.OverwritePrompt = false;
            if (sfd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            exportVGM = true;
            int Latency = setting.outputDevice.Latency;
            int WaitTime = setting.outputDevice.WaitTime;
            int LatencyEmu = setting.LatencyEmulation;
            int LatencySCCI = setting.LatencySCCI;
            setting.outputDevice.Latency = 0;
            setting.outputDevice.WaitTime = 0;
            setting.LatencyEmulation = 0;
            setting.LatencySCCI = 0;

            try
            {
                mubData = (MmlDatum[])qi.doc.compiledData;
                Document doc = qi.doc;
                doPlay = true;
                args = new string[2] { qi.doc.gwiFullPath, "" };
                compileTargetDocument = qi.doc;
                finishedCompileMUC();

                Audio.waveMode = true;
                Audio.waveModeAbort = false;
                Audio.Stop(SendMode.Both);
                exportVGM = false;

                bool res = Audio.PlayToVGM(setting, sfd.FileName, false, null, doc);
                if (!res)
                {
                    MessageBox.Show("失敗");
                    return;
                }

                FrmProgress fp = new FrmProgress();
                fp.ShowDialog();

                MessageBox.Show("完了");
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("失敗\r\n{0}\r\n{1}\r\n", ex.Message, ex.StackTrace));
            }
            finally
            {
                setting.outputDevice.Latency = Latency;
                setting.outputDevice.WaitTime = WaitTime;
                setting.LatencyEmulation = LatencyEmu;
                setting.LatencySCCI = LatencySCCI;
            }

        }

        private void tsmiMakeCSM_Click(object sender, EventArgs e)
        {
            IDockContent dc = GetActiveDockContent();
            if (dc == null) return;

            FrmCSM fm = new FrmCSM(RemoveForm, (FrmEditor)dc);
            FormBox.Add(fm);
            fm.Show();
        }

    }
}