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
            this.tssbOpen = new System.Windows.Forms.ToolStripSplitButton();
            this.tssbSave = new System.Windows.Forms.ToolStripSplitButton();
            this.tssbCompile = new System.Windows.Forms.ToolStripSplitButton();
            this.tssbTracePlay = new System.Windows.Forms.ToolStripSplitButton();
            this.tssbSlow = new System.Windows.Forms.ToolStripSplitButton();
            this.tssbFast = new System.Windows.Forms.ToolStripSplitButton();
            this.tssbStop = new System.Windows.Forms.ToolStripSplitButton();
            this.tsslCompileError = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslCompileWarning = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslCompileStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.TsmiFile = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiNew = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiOpenFile = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiOpenFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiSaveFile = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiExport = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.TsmiExit = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiUndo = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiRedo = new System.Windows.Forms.ToolStripMenuItem();
            this.表示VToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiShowPartCounter = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiShowFolderTree = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiShowErrorList = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiShowLog = new System.Windows.Forms.ToolStripMenuItem();
            this.コンパイルCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiCompileAndPlay = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiCompileAndTracePlay = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiAllCompile = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiCompile = new System.Windows.Forms.ToolStripMenuItem();
            this.ツールTToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiOption = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiTutorial = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiReference = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.tssbSoloPlay = new System.Windows.Forms.ToolStripSplitButton();
            this.TsmiFunctionKey = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiFncHide = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiFncButtonOnly = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiFncButtonAndText = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.TsslLineCol = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dpMain
            // 
            this.dpMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dpMain.Location = new System.Drawing.Point(0, 23);
            this.dpMain.Name = "dpMain";
            this.dpMain.Size = new System.Drawing.Size(933, 406);
            this.dpMain.TabIndex = 1;
            this.dpMain.ActiveDocumentChanged += new System.EventHandler(this.DpMain_ActiveDocumentChanged);
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
            this.tssbCompile,
            this.tssbTracePlay,
            this.tssbSoloPlay,
            this.tssbStop,
            this.tssbSlow,
            this.tssbFast});
            this.statusStrip1.Location = new System.Drawing.Point(0, 428);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            this.statusStrip1.ShowItemToolTips = true;
            this.statusStrip1.Size = new System.Drawing.Size(933, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tssbOpen
            // 
            this.tssbOpen.DropDownButtonWidth = 0;
            this.tssbOpen.Image = global::mml2vgmIDE.Properties.Resources.F01;
            this.tssbOpen.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tssbOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tssbOpen.Name = "tssbOpen";
            this.tssbOpen.Size = new System.Drawing.Size(63, 20);
            this.tssbOpen.Text = "開く";
            this.tssbOpen.ButtonClick += new System.EventHandler(this.TssbOpen_ButtonClick);
            // 
            // tssbSave
            // 
            this.tssbSave.DropDownButtonWidth = 0;
            this.tssbSave.Enabled = false;
            this.tssbSave.Image = global::mml2vgmIDE.Properties.Resources.F02;
            this.tssbSave.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tssbSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tssbSave.Name = "tssbSave";
            this.tssbSave.Size = new System.Drawing.Size(68, 20);
            this.tssbSave.Text = "保存";
            this.tssbSave.ButtonClick += new System.EventHandler(this.TssbSave_ButtonClick);
            // 
            // tssbCompile
            // 
            this.tssbCompile.DropDownButtonWidth = 0;
            this.tssbCompile.Image = global::mml2vgmIDE.Properties.Resources.F05;
            this.tssbCompile.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tssbCompile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tssbCompile.Name = "tssbCompile";
            this.tssbCompile.Size = new System.Drawing.Size(68, 20);
            this.tssbCompile.Text = "再生";
            this.tssbCompile.ButtonClick += new System.EventHandler(this.TssbCompile_ButtonClick);
            // 
            // tssbTracePlay
            // 
            this.tssbTracePlay.DropDownButtonWidth = 0;
            this.tssbTracePlay.Image = global::mml2vgmIDE.Properties.Resources.F06;
            this.tssbTracePlay.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tssbTracePlay.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tssbTracePlay.Name = "tssbTracePlay";
            this.tssbTracePlay.Size = new System.Drawing.Size(102, 20);
            this.tssbTracePlay.Text = "トレース再生";
            this.tssbTracePlay.ButtonClick += new System.EventHandler(this.TssbPlay_ButtonClick);
            // 
            // tssbSlow
            // 
            this.tssbSlow.DropDownButtonWidth = 0;
            this.tssbSlow.Image = global::mml2vgmIDE.Properties.Resources.F10;
            this.tssbSlow.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tssbSlow.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tssbSlow.Name = "tssbSlow";
            this.tssbSlow.Size = new System.Drawing.Size(70, 20);
            this.tssbSlow.Text = "スロー";
            this.tssbSlow.ButtonClick += new System.EventHandler(this.TssbSlow_ButtonClick);
            // 
            // tssbFast
            // 
            this.tssbFast.DropDownButtonWidth = 0;
            this.tssbFast.Image = global::mml2vgmIDE.Properties.Resources.F11;
            this.tssbFast.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tssbFast.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tssbFast.Name = "tssbFast";
            this.tssbFast.Size = new System.Drawing.Size(74, 20);
            this.tssbFast.Text = "4倍速";
            this.tssbFast.ButtonClick += new System.EventHandler(this.TssbFast_ButtonClick);
            // 
            // tssbStop
            // 
            this.tssbStop.DropDownButtonWidth = 0;
            this.tssbStop.Image = global::mml2vgmIDE.Properties.Resources.F09;
            this.tssbStop.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tssbStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tssbStop.Name = "tssbStop";
            this.tssbStop.Size = new System.Drawing.Size(68, 20);
            this.tssbStop.Text = "停止";
            this.tssbStop.ButtonClick += new System.EventHandler(this.TssbStop_ButtonClick);
            // 
            // tsslCompileError
            // 
            this.tsslCompileError.AutoToolTip = true;
            this.tsslCompileError.Image = global::mml2vgmIDE.Properties.Resources.Error;
            this.tsslCompileError.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.tsslCompileError.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsslCompileError.Name = "tsslCompileError";
            this.tsslCompileError.Size = new System.Drawing.Size(41, 17);
            this.tsslCompileError.Text = "123";
            this.tsslCompileError.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.tsslCompileError.ToolTipText = "エラー数";
            // 
            // tsslCompileWarning
            // 
            this.tsslCompileWarning.AutoToolTip = true;
            this.tsslCompileWarning.Image = global::mml2vgmIDE.Properties.Resources.Warning;
            this.tsslCompileWarning.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsslCompileWarning.Name = "tsslCompileWarning";
            this.tsslCompileWarning.Size = new System.Drawing.Size(41, 17);
            this.tsslCompileWarning.Text = "123";
            this.tsslCompileWarning.ToolTipText = "警告数";
            // 
            // tsslCompileStatus
            // 
            this.tsslCompileStatus.Name = "tsslCompileStatus";
            this.tsslCompileStatus.Size = new System.Drawing.Size(84, 17);
            this.tsslCompileStatus.Text = "C:9999 LC:9999";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TsmiFile,
            this.TsmiEdit,
            this.表示VToolStripMenuItem,
            this.コンパイルCToolStripMenuItem,
            this.ツールTToolStripMenuItem,
            this.TsmiHelp});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(933, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // TsmiFile
            // 
            this.TsmiFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TsmiNew,
            this.TsmiOpenFile,
            this.TsmiOpenFolder,
            this.TsmiSaveFile,
            this.TsmiSaveAs,
            this.toolStripSeparator2,
            this.tsmiExport,
            this.toolStripSeparator1,
            this.TsmiExit});
            this.TsmiFile.Name = "TsmiFile";
            this.TsmiFile.Size = new System.Drawing.Size(67, 20);
            this.TsmiFile.Text = "ファイル(&F)";
            // 
            // TsmiNew
            // 
            this.TsmiNew.Name = "TsmiNew";
            this.TsmiNew.Size = new System.Drawing.Size(177, 22);
            this.TsmiNew.Text = "新規作成(&N)";
            this.TsmiNew.Click += new System.EventHandler(this.TsmiNew_Click);
            // 
            // TsmiOpenFile
            // 
            this.TsmiOpenFile.Name = "TsmiOpenFile";
            this.TsmiOpenFile.Size = new System.Drawing.Size(177, 22);
            this.TsmiOpenFile.Text = "ファイルを開く(&O)";
            this.TsmiOpenFile.Click += new System.EventHandler(this.TsmiFileOpen_Click);
            // 
            // TsmiOpenFolder
            // 
            this.TsmiOpenFolder.Enabled = false;
            this.TsmiOpenFolder.Name = "TsmiOpenFolder";
            this.TsmiOpenFolder.Size = new System.Drawing.Size(177, 22);
            this.TsmiOpenFolder.Text = "フォルダーを開く(&F)";
            this.TsmiOpenFolder.Click += new System.EventHandler(this.TsmiOpenFolder_Click);
            // 
            // TsmiSaveFile
            // 
            this.TsmiSaveFile.Enabled = false;
            this.TsmiSaveFile.Name = "TsmiSaveFile";
            this.TsmiSaveFile.Size = new System.Drawing.Size(177, 22);
            this.TsmiSaveFile.Text = "上書き保存(&S)";
            this.TsmiSaveFile.Click += new System.EventHandler(this.TsmiSaveFile_Click);
            // 
            // TsmiSaveAs
            // 
            this.TsmiSaveAs.Enabled = false;
            this.TsmiSaveAs.Name = "TsmiSaveAs";
            this.TsmiSaveAs.Size = new System.Drawing.Size(177, 22);
            this.TsmiSaveAs.Text = "名前を付けて保存(&A)";
            this.TsmiSaveAs.Click += new System.EventHandler(this.TsmiSaveAs_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(174, 6);
            // 
            // tsmiExport
            // 
            this.tsmiExport.Name = "tsmiExport";
            this.tsmiExport.Size = new System.Drawing.Size(177, 22);
            this.tsmiExport.Text = "エクスポート";
            this.tsmiExport.Click += new System.EventHandler(this.TsmiExport_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(174, 6);
            // 
            // TsmiExit
            // 
            this.TsmiExit.Name = "TsmiExit";
            this.TsmiExit.Size = new System.Drawing.Size(177, 22);
            this.TsmiExit.Text = "終了(&E)";
            this.TsmiExit.Click += new System.EventHandler(this.TsmiExit_Click);
            // 
            // TsmiEdit
            // 
            this.TsmiEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TsmiUndo,
            this.TsmiRedo});
            this.TsmiEdit.Name = "TsmiEdit";
            this.TsmiEdit.Size = new System.Drawing.Size(57, 20);
            this.TsmiEdit.Text = "編集(&E)";
            // 
            // TsmiUndo
            // 
            this.TsmiUndo.Enabled = false;
            this.TsmiUndo.Name = "TsmiUndo";
            this.TsmiUndo.Size = new System.Drawing.Size(117, 22);
            this.TsmiUndo.Text = "元に戻す";
            this.TsmiUndo.Click += new System.EventHandler(this.TsmiUndo_Click);
            // 
            // TsmiRedo
            // 
            this.TsmiRedo.Enabled = false;
            this.TsmiRedo.Name = "TsmiRedo";
            this.TsmiRedo.Size = new System.Drawing.Size(117, 22);
            this.TsmiRedo.Text = "やり直し";
            this.TsmiRedo.Click += new System.EventHandler(this.TsmiRedo_Click);
            // 
            // 表示VToolStripMenuItem
            // 
            this.表示VToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TsmiShowPartCounter,
            this.TsmiShowFolderTree,
            this.TsmiShowErrorList,
            this.TsmiShowLog,
            this.TsmiFunctionKey});
            this.表示VToolStripMenuItem.Name = "表示VToolStripMenuItem";
            this.表示VToolStripMenuItem.Size = new System.Drawing.Size(58, 20);
            this.表示VToolStripMenuItem.Text = "表示(&V)";
            // 
            // TsmiShowPartCounter
            // 
            this.TsmiShowPartCounter.Name = "TsmiShowPartCounter";
            this.TsmiShowPartCounter.Size = new System.Drawing.Size(180, 22);
            this.TsmiShowPartCounter.Text = "パートカウンター";
            this.TsmiShowPartCounter.Click += new System.EventHandler(this.TsmiShowPartCounter_Click);
            // 
            // TsmiShowFolderTree
            // 
            this.TsmiShowFolderTree.Name = "TsmiShowFolderTree";
            this.TsmiShowFolderTree.Size = new System.Drawing.Size(180, 22);
            this.TsmiShowFolderTree.Text = "フォルダーツリー";
            this.TsmiShowFolderTree.Click += new System.EventHandler(this.TsmiShowFolderTree_Click);
            // 
            // TsmiShowErrorList
            // 
            this.TsmiShowErrorList.Name = "TsmiShowErrorList";
            this.TsmiShowErrorList.Size = new System.Drawing.Size(180, 22);
            this.TsmiShowErrorList.Text = "エラーリスト";
            this.TsmiShowErrorList.Click += new System.EventHandler(this.TsmiShowErrorList_Click);
            // 
            // TsmiShowLog
            // 
            this.TsmiShowLog.Name = "TsmiShowLog";
            this.TsmiShowLog.Size = new System.Drawing.Size(180, 22);
            this.TsmiShowLog.Text = "ログ";
            this.TsmiShowLog.Click += new System.EventHandler(this.TsmiShowLog_Click);
            // 
            // コンパイルCToolStripMenuItem
            // 
            this.コンパイルCToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TsmiCompileAndPlay,
            this.TsmiCompileAndTracePlay,
            this.TsmiAllCompile,
            this.TsmiCompile});
            this.コンパイルCToolStripMenuItem.Name = "コンパイルCToolStripMenuItem";
            this.コンパイルCToolStripMenuItem.Size = new System.Drawing.Size(80, 20);
            this.コンパイルCToolStripMenuItem.Text = "コンパイル(&C)";
            // 
            // TsmiCompileAndPlay
            // 
            this.TsmiCompileAndPlay.Name = "TsmiCompileAndPlay";
            this.TsmiCompileAndPlay.Size = new System.Drawing.Size(225, 22);
            this.TsmiCompileAndPlay.Text = "コンパイルと再生";
            this.TsmiCompileAndPlay.Click += new System.EventHandler(this.TsmiCompileAndPlay_Click);
            // 
            // TsmiCompileAndTracePlay
            // 
            this.TsmiCompileAndTracePlay.Name = "TsmiCompileAndTracePlay";
            this.TsmiCompileAndTracePlay.Size = new System.Drawing.Size(225, 22);
            this.TsmiCompileAndTracePlay.Text = "コンパイルとトレース再生";
            this.TsmiCompileAndTracePlay.Click += new System.EventHandler(this.TsmiCompileAndTracePlay_Click);
            // 
            // TsmiAllCompile
            // 
            this.TsmiAllCompile.Enabled = false;
            this.TsmiAllCompile.Name = "TsmiAllCompile";
            this.TsmiAllCompile.Size = new System.Drawing.Size(225, 22);
            this.TsmiAllCompile.Text = "開いているファイル全てコンパイル";
            // 
            // TsmiCompile
            // 
            this.TsmiCompile.Name = "TsmiCompile";
            this.TsmiCompile.Size = new System.Drawing.Size(225, 22);
            this.TsmiCompile.Text = "コンパイルのみ";
            this.TsmiCompile.Click += new System.EventHandler(this.TsmiCompile_Click);
            // 
            // ツールTToolStripMenuItem
            // 
            this.ツールTToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiOption});
            this.ツールTToolStripMenuItem.Name = "ツールTToolStripMenuItem";
            this.ツールTToolStripMenuItem.Size = new System.Drawing.Size(60, 20);
            this.ツールTToolStripMenuItem.Text = "ツール(&T)";
            // 
            // tsmiOption
            // 
            this.tsmiOption.Name = "tsmiOption";
            this.tsmiOption.Size = new System.Drawing.Size(135, 22);
            this.tsmiOption.Text = "オプション(&O)";
            this.tsmiOption.Click += new System.EventHandler(this.TsmiOption_Click);
            // 
            // TsmiHelp
            // 
            this.TsmiHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TsmiTutorial,
            this.TsmiReference,
            this.TsmiAbout});
            this.TsmiHelp.Name = "TsmiHelp";
            this.TsmiHelp.Size = new System.Drawing.Size(65, 20);
            this.TsmiHelp.Text = "ヘルプ(&H)";
            // 
            // TsmiTutorial
            // 
            this.TsmiTutorial.Name = "TsmiTutorial";
            this.TsmiTutorial.Size = new System.Drawing.Size(181, 22);
            this.TsmiTutorial.Text = "データ作成手順";
            this.TsmiTutorial.Click += new System.EventHandler(this.TsmiTutorial_Click);
            // 
            // TsmiReference
            // 
            this.TsmiReference.Name = "TsmiReference";
            this.TsmiReference.Size = new System.Drawing.Size(181, 22);
            this.TsmiReference.Text = "mmlコマンドリファレンス";
            this.TsmiReference.Click += new System.EventHandler(this.TsmiReference_Click);
            // 
            // TsmiAbout
            // 
            this.TsmiAbout.Name = "TsmiAbout";
            this.TsmiAbout.Size = new System.Drawing.Size(181, 22);
            this.TsmiAbout.Text = "About";
            this.TsmiAbout.Click += new System.EventHandler(this.TsmiAbout_Click);
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Interval = 16;
            this.timer.Tick += new System.EventHandler(this.Timer_Tick);
            // 
            // tssbSoloPlay
            // 
            this.tssbSoloPlay.DropDownButtonWidth = 0;
            this.tssbSoloPlay.Image = global::mml2vgmIDE.Properties.Resources.F07;
            this.tssbSoloPlay.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tssbSoloPlay.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tssbSoloPlay.Name = "tssbSoloPlay";
            this.tssbSoloPlay.Size = new System.Drawing.Size(86, 20);
            this.tssbSoloPlay.Text = "ソロ再生";
            // 
            // TsmiFunctionKey
            // 
            this.TsmiFunctionKey.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TsmiFncHide,
            this.TsmiFncButtonOnly,
            this.TsmiFncButtonAndText});
            this.TsmiFunctionKey.Name = "TsmiFunctionKey";
            this.TsmiFunctionKey.Size = new System.Drawing.Size(180, 22);
            this.TsmiFunctionKey.Text = "ファンクションキー";
            // 
            // TsmiFncHide
            // 
            this.TsmiFncHide.Name = "TsmiFncHide";
            this.TsmiFncHide.Size = new System.Drawing.Size(180, 22);
            this.TsmiFncHide.Text = "非表示";
            this.TsmiFncHide.Click += new System.EventHandler(this.TsmiFncHide_Click);
            // 
            // TsmiFncButtonOnly
            // 
            this.TsmiFncButtonOnly.Name = "TsmiFncButtonOnly";
            this.TsmiFncButtonOnly.Size = new System.Drawing.Size(180, 22);
            this.TsmiFncButtonOnly.Text = "ボタンのみ";
            this.TsmiFncButtonOnly.Click += new System.EventHandler(this.TsmiFncButtonOnly_Click);
            // 
            // TsmiFncButtonAndText
            // 
            this.TsmiFncButtonAndText.Name = "TsmiFncButtonAndText";
            this.TsmiFncButtonAndText.Size = new System.Drawing.Size(180, 22);
            this.TsmiFncButtonAndText.Text = "ボタンとテキスト";
            this.TsmiFncButtonAndText.Click += new System.EventHandler(this.TsmiFncButtonAndText_Click);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(17, 17);
            this.toolStripStatusLabel1.Spring = true;
            this.toolStripStatusLabel1.Text = " ";
            // 
            // TsslLineCol
            // 
            this.TsslLineCol.Name = "TsslLineCol";
            this.TsslLineCol.Size = new System.Drawing.Size(103, 17);
            this.TsslLineCol.Text = "Line:9999 Col:9999";
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(933, 450);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.dpMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FrmMain";
            this.Text = "mml2vgmIDE";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmMain_KeyDown);
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
        private System.Windows.Forms.ToolStripSplitButton tssbCompile;
        private System.Windows.Forms.ToolStripStatusLabel tsslCompileError;
        private System.Windows.Forms.ToolStripSplitButton tssbTracePlay;
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
        private System.Windows.Forms.ToolStripSplitButton tssbSoloPlay;
        private System.Windows.Forms.ToolStripMenuItem TsmiFunctionKey;
        private System.Windows.Forms.ToolStripMenuItem TsmiFncHide;
        private System.Windows.Forms.ToolStripMenuItem TsmiFncButtonOnly;
        private System.Windows.Forms.ToolStripMenuItem TsmiFncButtonAndText;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel TsslLineCol;
    }
}

