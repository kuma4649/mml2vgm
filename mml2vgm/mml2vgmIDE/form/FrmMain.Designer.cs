namespace mml2vgmIDE
{
    partial class FrmMain
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.dpMain = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.visualStudioToolStripExtender1 = new WeifenLuo.WinFormsUI.Docking.VisualStudioToolStripExtender(this.components);
            this.vS2005Theme1 = new WeifenLuo.WinFormsUI.Docking.VS2005Theme();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tsslCompileError = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslCompileWarning = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslCompileStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.TsslLineCol = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssbOpen = new System.Windows.Forms.ToolStripSplitButton();
            this.tssbSave = new System.Windows.Forms.ToolStripSplitButton();
            this.tssbFind = new System.Windows.Forms.ToolStripSplitButton();
            this.tssbCompile = new System.Windows.Forms.ToolStripSplitButton();
            this.tssbPlay = new System.Windows.Forms.ToolStripSplitButton();
            this.tssbM98 = new System.Windows.Forms.ToolStripSplitButton();
            this.tssbStop = new System.Windows.Forms.ToolStripSplitButton();
            this.tssbSlow = new System.Windows.Forms.ToolStripSplitButton();
            this.tssbFast = new System.Windows.Forms.ToolStripSplitButton();
            this.tssbMIDIKbd = new System.Windows.Forms.ToolStripSplitButton();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.TsmiFile = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiNew = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiOpenFile = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiOpenFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiGwiFileHistory = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiSaveFile = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiImport = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiExport = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.TsmiExit = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiUndo = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiRedo = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.TsmiFind = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiFindNext = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiFindPrevious = new System.Windows.Forms.ToolStripMenuItem();
            this.表示VToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiShowPartCounter = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiShowFolderTree = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiShowErrorList = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiShowLog = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiShowLyrics = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiShowMixer = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiShowMIDIKbd = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.TsmiFunctionKey = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiFncHide = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiFncButtonOnly = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiFncButtonAndText = new System.Windows.Forms.ToolStripMenuItem();
            this.コンパイルCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiCompileAndPlay = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiCompileAndTracePlay = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiCompileAndSkipPlay = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiAllCompile = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiCompile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiScript = new System.Windows.Forms.ToolStripMenuItem();
            this.ツールTToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiOption = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiTutorial = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiReference = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dpMain
            // 
            this.dpMain.AllowDrop = true;
            resources.ApplyResources(this.dpMain, "dpMain");
            this.dpMain.Name = "dpMain";
            this.dpMain.ShowDocumentIcon = true;
            this.dpMain.ActiveDocumentChanged += new System.EventHandler(this.DpMain_ActiveDocumentChanged);
            this.dpMain.DragDrop += new System.Windows.Forms.DragEventHandler(this.DpMain_DragDrop);
            this.dpMain.DragOver += new System.Windows.Forms.DragEventHandler(this.DpMain_DragOver);
            // 
            // visualStudioToolStripExtender1
            // 
            this.visualStudioToolStripExtender1.DefaultRenderer = null;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslCompileError,
            this.tsslCompileWarning,
            this.tsslCompileStatus,
            this.TsslLineCol,
            this.toolStripStatusLabel1,
            this.tssbOpen,
            this.tssbSave,
            this.tssbFind,
            this.tssbCompile,
            this.tssbPlay,
            this.tssbM98,
            this.tssbStop,
            this.tssbSlow,
            this.tssbFast,
            this.tssbMIDIKbd});
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.ShowItemToolTips = true;
            // 
            // tsslCompileError
            // 
            this.tsslCompileError.AutoToolTip = true;
            this.tsslCompileError.Image = global::mml2vgmIDE.Properties.Resources.Error;
            resources.ApplyResources(this.tsslCompileError, "tsslCompileError");
            this.tsslCompileError.Name = "tsslCompileError";
            this.tsslCompileError.Click += new System.EventHandler(this.TsslCompileError_Click);
            // 
            // tsslCompileWarning
            // 
            this.tsslCompileWarning.AutoToolTip = true;
            this.tsslCompileWarning.Image = global::mml2vgmIDE.Properties.Resources.Warning;
            resources.ApplyResources(this.tsslCompileWarning, "tsslCompileWarning");
            this.tsslCompileWarning.Name = "tsslCompileWarning";
            this.tsslCompileWarning.Click += new System.EventHandler(this.TsslCompileWarning_Click);
            // 
            // tsslCompileStatus
            // 
            this.tsslCompileStatus.Name = "tsslCompileStatus";
            resources.ApplyResources(this.tsslCompileStatus, "tsslCompileStatus");
            // 
            // TsslLineCol
            // 
            this.TsslLineCol.Name = "TsslLineCol";
            resources.ApplyResources(this.TsslLineCol, "TsslLineCol");
            // 
            // toolStripStatusLabel1
            // 
            resources.ApplyResources(this.toolStripStatusLabel1, "toolStripStatusLabel1");
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Spring = true;
            // 
            // tssbOpen
            // 
            this.tssbOpen.DropDownButtonWidth = 0;
            this.tssbOpen.Image = global::mml2vgmIDE.Properties.Resources.F01;
            resources.ApplyResources(this.tssbOpen, "tssbOpen");
            this.tssbOpen.Name = "tssbOpen";
            this.tssbOpen.ButtonClick += new System.EventHandler(this.TssbOpen_ButtonClick);
            // 
            // tssbSave
            // 
            this.tssbSave.DropDownButtonWidth = 0;
            resources.ApplyResources(this.tssbSave, "tssbSave");
            this.tssbSave.Image = global::mml2vgmIDE.Properties.Resources.F02;
            this.tssbSave.Name = "tssbSave";
            this.tssbSave.ButtonClick += new System.EventHandler(this.TssbSave_ButtonClick);
            // 
            // tssbFind
            // 
            this.tssbFind.DropDownButtonWidth = 0;
            this.tssbFind.Image = global::mml2vgmIDE.Properties.Resources.F03;
            resources.ApplyResources(this.tssbFind, "tssbFind");
            this.tssbFind.Name = "tssbFind";
            this.tssbFind.ButtonClick += new System.EventHandler(this.TssbFind_ButtonClick);
            // 
            // tssbCompile
            // 
            this.tssbCompile.DropDownButtonWidth = 0;
            this.tssbCompile.Image = global::mml2vgmIDE.Properties.Resources.F04;
            resources.ApplyResources(this.tssbCompile, "tssbCompile");
            this.tssbCompile.Name = "tssbCompile";
            this.tssbCompile.ButtonClick += new System.EventHandler(this.tssbCompile_ButtonClick_1);
            // 
            // tssbPlay
            // 
            this.tssbPlay.DropDownButtonWidth = 0;
            this.tssbPlay.Image = global::mml2vgmIDE.Properties.Resources.F05;
            resources.ApplyResources(this.tssbPlay, "tssbPlay");
            this.tssbPlay.Name = "tssbPlay";
            this.tssbPlay.ButtonClick += new System.EventHandler(this.TssbPlay_ButtonClick);
            // 
            // tssbM98
            // 
            this.tssbM98.DropDownButtonWidth = 0;
            this.tssbM98.Image = global::mml2vgmIDE.Properties.Resources.F06;
            resources.ApplyResources(this.tssbM98, "tssbM98");
            this.tssbM98.Name = "tssbM98";
            this.tssbM98.ButtonClick += new System.EventHandler(this.tssbM98_ButtonClick);
            // 
            // tssbStop
            // 
            this.tssbStop.DropDownButtonWidth = 0;
            this.tssbStop.Image = global::mml2vgmIDE.Properties.Resources.F09;
            resources.ApplyResources(this.tssbStop, "tssbStop");
            this.tssbStop.Name = "tssbStop";
            this.tssbStop.ButtonClick += new System.EventHandler(this.TssbStop_ButtonClick);
            // 
            // tssbSlow
            // 
            this.tssbSlow.DropDownButtonWidth = 0;
            this.tssbSlow.Image = global::mml2vgmIDE.Properties.Resources.F10;
            resources.ApplyResources(this.tssbSlow, "tssbSlow");
            this.tssbSlow.Name = "tssbSlow";
            this.tssbSlow.ButtonClick += new System.EventHandler(this.TssbSlow_ButtonClick);
            // 
            // tssbFast
            // 
            this.tssbFast.DropDownButtonWidth = 0;
            this.tssbFast.Image = global::mml2vgmIDE.Properties.Resources.F11;
            resources.ApplyResources(this.tssbFast, "tssbFast");
            this.tssbFast.Name = "tssbFast";
            this.tssbFast.ButtonClick += new System.EventHandler(this.TssbFast_ButtonClick);
            // 
            // tssbMIDIKbd
            // 
            this.tssbMIDIKbd.DropDownButtonWidth = 0;
            this.tssbMIDIKbd.Image = global::mml2vgmIDE.Properties.Resources.F12;
            resources.ApplyResources(this.tssbMIDIKbd, "tssbMIDIKbd");
            this.tssbMIDIKbd.Name = "tssbMIDIKbd";
            this.tssbMIDIKbd.ButtonClick += new System.EventHandler(this.TssbMIDIKbd_ButtonClick);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TsmiFile,
            this.TsmiEdit,
            this.表示VToolStripMenuItem,
            this.コンパイルCToolStripMenuItem,
            this.tsmiScript,
            this.ツールTToolStripMenuItem,
            this.TsmiHelp});
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Stretch = false;
            // 
            // TsmiFile
            // 
            this.TsmiFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TsmiNew,
            this.TsmiOpenFile,
            this.TsmiOpenFolder,
            this.tsmiGwiFileHistory,
            this.TsmiSaveFile,
            this.TsmiSaveAs,
            this.toolStripSeparator2,
            this.tsmiImport,
            this.tsmiExport,
            this.toolStripSeparator1,
            this.TsmiExit});
            this.TsmiFile.Name = "TsmiFile";
            resources.ApplyResources(this.TsmiFile, "TsmiFile");
            // 
            // TsmiNew
            // 
            this.TsmiNew.Name = "TsmiNew";
            resources.ApplyResources(this.TsmiNew, "TsmiNew");
            this.TsmiNew.Click += new System.EventHandler(this.TsmiNew_Click);
            // 
            // TsmiOpenFile
            // 
            this.TsmiOpenFile.Name = "TsmiOpenFile";
            resources.ApplyResources(this.TsmiOpenFile, "TsmiOpenFile");
            this.TsmiOpenFile.Click += new System.EventHandler(this.TsmiFileOpen_Click);
            // 
            // TsmiOpenFolder
            // 
            resources.ApplyResources(this.TsmiOpenFolder, "TsmiOpenFolder");
            this.TsmiOpenFolder.Name = "TsmiOpenFolder";
            this.TsmiOpenFolder.Click += new System.EventHandler(this.TsmiOpenFolder_Click);
            // 
            // tsmiGwiFileHistory
            // 
            this.tsmiGwiFileHistory.Name = "tsmiGwiFileHistory";
            resources.ApplyResources(this.tsmiGwiFileHistory, "tsmiGwiFileHistory");
            // 
            // TsmiSaveFile
            // 
            resources.ApplyResources(this.TsmiSaveFile, "TsmiSaveFile");
            this.TsmiSaveFile.Name = "TsmiSaveFile";
            this.TsmiSaveFile.Click += new System.EventHandler(this.TsmiSaveFile_Click);
            // 
            // TsmiSaveAs
            // 
            resources.ApplyResources(this.TsmiSaveAs, "TsmiSaveAs");
            this.TsmiSaveAs.Name = "TsmiSaveAs";
            this.TsmiSaveAs.Click += new System.EventHandler(this.TsmiSaveAs_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // tsmiImport
            // 
            this.tsmiImport.Name = "tsmiImport";
            resources.ApplyResources(this.tsmiImport, "tsmiImport");
            this.tsmiImport.Click += new System.EventHandler(this.TsmiImport_Click);
            // 
            // tsmiExport
            // 
            this.tsmiExport.Name = "tsmiExport";
            resources.ApplyResources(this.tsmiExport, "tsmiExport");
            this.tsmiExport.Click += new System.EventHandler(this.TsmiExport_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // TsmiExit
            // 
            this.TsmiExit.Name = "TsmiExit";
            resources.ApplyResources(this.TsmiExit, "TsmiExit");
            this.TsmiExit.Click += new System.EventHandler(this.TsmiExit_Click);
            // 
            // TsmiEdit
            // 
            this.TsmiEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TsmiUndo,
            this.TsmiRedo,
            this.toolStripSeparator4,
            this.TsmiFind,
            this.TsmiFindNext,
            this.TsmiFindPrevious});
            this.TsmiEdit.Name = "TsmiEdit";
            resources.ApplyResources(this.TsmiEdit, "TsmiEdit");
            // 
            // TsmiUndo
            // 
            resources.ApplyResources(this.TsmiUndo, "TsmiUndo");
            this.TsmiUndo.Name = "TsmiUndo";
            this.TsmiUndo.Click += new System.EventHandler(this.TsmiUndo_Click);
            // 
            // TsmiRedo
            // 
            resources.ApplyResources(this.TsmiRedo, "TsmiRedo");
            this.TsmiRedo.Name = "TsmiRedo";
            this.TsmiRedo.Click += new System.EventHandler(this.TsmiRedo_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            // 
            // TsmiFind
            // 
            this.TsmiFind.Name = "TsmiFind";
            resources.ApplyResources(this.TsmiFind, "TsmiFind");
            this.TsmiFind.Click += new System.EventHandler(this.TsmiFind_Click);
            // 
            // TsmiFindNext
            // 
            this.TsmiFindNext.Name = "TsmiFindNext";
            resources.ApplyResources(this.TsmiFindNext, "TsmiFindNext");
            this.TsmiFindNext.Click += new System.EventHandler(this.TsmiFindNext_Click);
            // 
            // TsmiFindPrevious
            // 
            this.TsmiFindPrevious.Name = "TsmiFindPrevious";
            resources.ApplyResources(this.TsmiFindPrevious, "TsmiFindPrevious");
            this.TsmiFindPrevious.Click += new System.EventHandler(this.TsmiFindPrevious_Click);
            // 
            // 表示VToolStripMenuItem
            // 
            this.表示VToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TsmiShowPartCounter,
            this.TsmiShowFolderTree,
            this.TsmiShowErrorList,
            this.TsmiShowLog,
            this.TsmiShowLyrics,
            this.TsmiShowMixer,
            this.TsmiShowMIDIKbd,
            this.toolStripSeparator3,
            this.TsmiFunctionKey});
            this.表示VToolStripMenuItem.Name = "表示VToolStripMenuItem";
            resources.ApplyResources(this.表示VToolStripMenuItem, "表示VToolStripMenuItem");
            // 
            // TsmiShowPartCounter
            // 
            this.TsmiShowPartCounter.Name = "TsmiShowPartCounter";
            resources.ApplyResources(this.TsmiShowPartCounter, "TsmiShowPartCounter");
            this.TsmiShowPartCounter.Click += new System.EventHandler(this.TsmiShowPartCounter_Click);
            // 
            // TsmiShowFolderTree
            // 
            this.TsmiShowFolderTree.Name = "TsmiShowFolderTree";
            resources.ApplyResources(this.TsmiShowFolderTree, "TsmiShowFolderTree");
            this.TsmiShowFolderTree.Click += new System.EventHandler(this.TsmiShowFolderTree_Click);
            // 
            // TsmiShowErrorList
            // 
            this.TsmiShowErrorList.Name = "TsmiShowErrorList";
            resources.ApplyResources(this.TsmiShowErrorList, "TsmiShowErrorList");
            this.TsmiShowErrorList.Click += new System.EventHandler(this.TsmiShowErrorList_Click);
            // 
            // TsmiShowLog
            // 
            this.TsmiShowLog.Name = "TsmiShowLog";
            resources.ApplyResources(this.TsmiShowLog, "TsmiShowLog");
            this.TsmiShowLog.Click += new System.EventHandler(this.TsmiShowLog_Click);
            // 
            // TsmiShowLyrics
            // 
            this.TsmiShowLyrics.Name = "TsmiShowLyrics";
            resources.ApplyResources(this.TsmiShowLyrics, "TsmiShowLyrics");
            this.TsmiShowLyrics.Click += new System.EventHandler(this.TsmiShowLyrics_Click);
            // 
            // TsmiShowMixer
            // 
            this.TsmiShowMixer.Name = "TsmiShowMixer";
            resources.ApplyResources(this.TsmiShowMixer, "TsmiShowMixer");
            this.TsmiShowMixer.Click += new System.EventHandler(this.TsmiShowMixer_Click);
            // 
            // TsmiShowMIDIKbd
            // 
            this.TsmiShowMIDIKbd.Name = "TsmiShowMIDIKbd";
            resources.ApplyResources(this.TsmiShowMIDIKbd, "TsmiShowMIDIKbd");
            this.TsmiShowMIDIKbd.Click += new System.EventHandler(this.TsmiShowMIDIKbd_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            // 
            // TsmiFunctionKey
            // 
            this.TsmiFunctionKey.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TsmiFncHide,
            this.TsmiFncButtonOnly,
            this.TsmiFncButtonAndText});
            this.TsmiFunctionKey.Name = "TsmiFunctionKey";
            resources.ApplyResources(this.TsmiFunctionKey, "TsmiFunctionKey");
            // 
            // TsmiFncHide
            // 
            this.TsmiFncHide.Name = "TsmiFncHide";
            resources.ApplyResources(this.TsmiFncHide, "TsmiFncHide");
            this.TsmiFncHide.Click += new System.EventHandler(this.TsmiFncHide_Click);
            // 
            // TsmiFncButtonOnly
            // 
            this.TsmiFncButtonOnly.Name = "TsmiFncButtonOnly";
            resources.ApplyResources(this.TsmiFncButtonOnly, "TsmiFncButtonOnly");
            this.TsmiFncButtonOnly.Click += new System.EventHandler(this.TsmiFncButtonOnly_Click);
            // 
            // TsmiFncButtonAndText
            // 
            this.TsmiFncButtonAndText.Name = "TsmiFncButtonAndText";
            resources.ApplyResources(this.TsmiFncButtonAndText, "TsmiFncButtonAndText");
            this.TsmiFncButtonAndText.Click += new System.EventHandler(this.TsmiFncButtonAndText_Click);
            // 
            // コンパイルCToolStripMenuItem
            // 
            this.コンパイルCToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TsmiCompileAndPlay,
            this.TsmiCompileAndTracePlay,
            this.TsmiCompileAndSkipPlay,
            this.TsmiAllCompile,
            this.TsmiCompile});
            this.コンパイルCToolStripMenuItem.Name = "コンパイルCToolStripMenuItem";
            resources.ApplyResources(this.コンパイルCToolStripMenuItem, "コンパイルCToolStripMenuItem");
            // 
            // TsmiCompileAndPlay
            // 
            this.TsmiCompileAndPlay.Name = "TsmiCompileAndPlay";
            resources.ApplyResources(this.TsmiCompileAndPlay, "TsmiCompileAndPlay");
            this.TsmiCompileAndPlay.Click += new System.EventHandler(this.TsmiCompileAndPlay_Click);
            // 
            // TsmiCompileAndTracePlay
            // 
            this.TsmiCompileAndTracePlay.Name = "TsmiCompileAndTracePlay";
            resources.ApplyResources(this.TsmiCompileAndTracePlay, "TsmiCompileAndTracePlay");
            this.TsmiCompileAndTracePlay.Click += new System.EventHandler(this.TsmiCompileAndTracePlay_Click);
            // 
            // TsmiCompileAndSkipPlay
            // 
            this.TsmiCompileAndSkipPlay.Name = "TsmiCompileAndSkipPlay";
            resources.ApplyResources(this.TsmiCompileAndSkipPlay, "TsmiCompileAndSkipPlay");
            this.TsmiCompileAndSkipPlay.Click += new System.EventHandler(this.TsmiCompileAndSkipPlay_Click);
            // 
            // TsmiAllCompile
            // 
            resources.ApplyResources(this.TsmiAllCompile, "TsmiAllCompile");
            this.TsmiAllCompile.Name = "TsmiAllCompile";
            // 
            // TsmiCompile
            // 
            this.TsmiCompile.Name = "TsmiCompile";
            resources.ApplyResources(this.TsmiCompile, "TsmiCompile");
            this.TsmiCompile.Click += new System.EventHandler(this.TsmiCompile_Click);
            // 
            // tsmiScript
            // 
            this.tsmiScript.Name = "tsmiScript";
            resources.ApplyResources(this.tsmiScript, "tsmiScript");
            // 
            // ツールTToolStripMenuItem
            // 
            this.ツールTToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiOption});
            this.ツールTToolStripMenuItem.Name = "ツールTToolStripMenuItem";
            resources.ApplyResources(this.ツールTToolStripMenuItem, "ツールTToolStripMenuItem");
            // 
            // tsmiOption
            // 
            this.tsmiOption.Name = "tsmiOption";
            resources.ApplyResources(this.tsmiOption, "tsmiOption");
            this.tsmiOption.Click += new System.EventHandler(this.TsmiOption_Click);
            // 
            // TsmiHelp
            // 
            this.TsmiHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TsmiTutorial,
            this.TsmiReference,
            this.TsmiAbout});
            this.TsmiHelp.Name = "TsmiHelp";
            resources.ApplyResources(this.TsmiHelp, "TsmiHelp");
            // 
            // TsmiTutorial
            // 
            this.TsmiTutorial.Name = "TsmiTutorial";
            resources.ApplyResources(this.TsmiTutorial, "TsmiTutorial");
            this.TsmiTutorial.Click += new System.EventHandler(this.TsmiTutorial_Click);
            // 
            // TsmiReference
            // 
            this.TsmiReference.Name = "TsmiReference";
            resources.ApplyResources(this.TsmiReference, "TsmiReference");
            this.TsmiReference.Click += new System.EventHandler(this.TsmiReference_Click);
            // 
            // TsmiAbout
            // 
            this.TsmiAbout.Name = "TsmiAbout";
            resources.ApplyResources(this.TsmiAbout, "TsmiAbout");
            this.TsmiAbout.Click += new System.EventHandler(this.TsmiAbout_Click);
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Interval = 16;
            this.timer.Tick += new System.EventHandler(this.Timer_Tick);
            // 
            // FrmMain
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.dpMain);
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FrmMain";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmMain_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FrmMain_KeyUp);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private WeifenLuo.WinFormsUI.Docking.VisualStudioToolStripExtender visualStudioToolStripExtender1;
        private WeifenLuo.WinFormsUI.Docking.VS2005Theme vS2005Theme1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem TsmiFile;
        private System.Windows.Forms.ToolStripMenuItem TsmiNew;
        private System.Windows.Forms.ToolStripMenuItem TsmiOpenFile;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem TsmiExit;
        private System.Windows.Forms.ToolStripMenuItem TsmiEdit;
        private System.Windows.Forms.ToolStripMenuItem TsmiHelp;
        private System.Windows.Forms.ToolStripMenuItem コンパイルCToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem TsmiCompileAndPlay;
        private System.Windows.Forms.ToolStripMenuItem TsmiAllCompile;
        private System.Windows.Forms.ToolStripMenuItem TsmiCompile;
        private System.Windows.Forms.ToolStripMenuItem TsmiOpenFolder;
        private System.Windows.Forms.ToolStripMenuItem TsmiAbout;
        private System.Windows.Forms.ToolStripMenuItem 表示VToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem TsmiShowPartCounter;
        private System.Windows.Forms.ToolStripMenuItem TsmiShowFolderTree;
        private System.Windows.Forms.ToolStripMenuItem TsmiShowErrorList;
        private System.Windows.Forms.ToolStripMenuItem TsmiShowLog;
        private System.Windows.Forms.ToolStripMenuItem ツールTToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsmiOption;
        private System.Windows.Forms.ToolStripMenuItem TsmiTutorial;
        private System.Windows.Forms.ToolStripMenuItem TsmiReference;
        private System.Windows.Forms.ToolStripSplitButton tssbOpen;
        private System.Windows.Forms.ToolStripSplitButton tssbSave;
        private System.Windows.Forms.ToolStripSplitButton tssbPlay;
        private System.Windows.Forms.ToolStripStatusLabel tsslCompileError;
        private System.Windows.Forms.ToolStripSplitButton tssbSlow;
        private System.Windows.Forms.ToolStripSplitButton tssbFast;
        private System.Windows.Forms.ToolStripSplitButton tssbStop;
        private System.Windows.Forms.ToolStripMenuItem TsmiSaveFile;
        private System.Windows.Forms.ToolStripMenuItem TsmiSaveAs;
        private System.Windows.Forms.ToolStripMenuItem TsmiUndo;
        private System.Windows.Forms.ToolStripMenuItem TsmiRedo;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem tsmiExport;
        private System.Windows.Forms.ToolStripStatusLabel tsslCompileWarning;
        private System.Windows.Forms.ToolStripStatusLabel tsslCompileStatus;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.ToolStripMenuItem TsmiCompileAndTracePlay;
        public WeifenLuo.WinFormsUI.Docking.DockPanel dpMain;
        private System.Windows.Forms.ToolStripMenuItem TsmiFunctionKey;
        private System.Windows.Forms.ToolStripMenuItem TsmiFncHide;
        private System.Windows.Forms.ToolStripMenuItem TsmiFncButtonOnly;
        private System.Windows.Forms.ToolStripMenuItem TsmiFncButtonAndText;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripMenuItem tsmiGwiFileHistory;
        private System.Windows.Forms.ToolStripMenuItem tsmiImport;
        private System.Windows.Forms.ToolStripMenuItem tsmiScript;
        private System.Windows.Forms.ToolStripMenuItem TsmiShowMixer;
        private System.Windows.Forms.ToolStripMenuItem TsmiShowMIDIKbd;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem TsmiCompileAndSkipPlay;
        public System.Windows.Forms.ToolStripStatusLabel TsslLineCol;
        private System.Windows.Forms.ToolStripSplitButton tssbMIDIKbd;
        private System.Windows.Forms.ToolStripMenuItem TsmiShowLyrics;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem TsmiFind;
        private System.Windows.Forms.ToolStripMenuItem TsmiFindNext;
        private System.Windows.Forms.ToolStripMenuItem TsmiFindPrevious;
        private System.Windows.Forms.ToolStripSplitButton tssbFind;
        private System.Windows.Forms.ToolStripSplitButton tssbM98;
        private System.Windows.Forms.ToolStripSplitButton tssbCompile;
    }
}

