using mml2vgmIDEx64.Properties;

namespace mml2vgmIDEx64
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            dpMain = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            visualStudioToolStripExtender1 = new WeifenLuo.WinFormsUI.Docking.VisualStudioToolStripExtender(components);
            vS2005Theme1 = new WeifenLuo.WinFormsUI.Docking.VS2015DarkTheme();
            statusStrip1 = new StatusStrip();
            tsslCompileError = new ToolStripStatusLabel();
            tsslCompileWarning = new ToolStripStatusLabel();
            tsslCompileStatus = new ToolStripStatusLabel();
            TsslLineCol = new ToolStripStatusLabel();
            tsslJumpSoloMode = new ToolStripStatusLabel();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            tsslSimpleKbdMode = new ToolStripStatusLabel();
            tsslStatus = new ToolStripStatusLabel();
            tssbOpen = new ToolStripSplitButton();
            tssbSave = new ToolStripSplitButton();
            tssbFind = new ToolStripSplitButton();
            tssbCompile = new ToolStripSplitButton();
            tssbPlay = new ToolStripSplitButton();
            tssbM98 = new ToolStripSplitButton();
            tssbPause = new ToolStripSplitButton();
            tssbStop = new ToolStripSplitButton();
            tssbSlow = new ToolStripSplitButton();
            tssbFast = new ToolStripSplitButton();
            tssbExpAndMdp = new ToolStripSplitButton();
            tssbSien = new ToolStripSplitButton();
            tssbMIDIKbd = new ToolStripSplitButton();
            menuStrip1 = new MenuStrip();
            TsmiFile = new ToolStripMenuItem();
            TsmiNew = new ToolStripMenuItem();
            tsmiNewGwi = new ToolStripMenuItem();
            tsmiNewMuc = new ToolStripMenuItem();
            tsmiNewMml = new ToolStripMenuItem();
            TsmiOpenFile = new ToolStripMenuItem();
            TsmiOpenFolder = new ToolStripMenuItem();
            tsmiGwiFileHistory = new ToolStripMenuItem();
            TsmiSaveFile = new ToolStripMenuItem();
            TsmiSaveAs = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            tsmiImport = new ToolStripMenuItem();
            tsmiImport_D88toMuc = new ToolStripMenuItem();
            tsmiImport_MIDtoGWI = new ToolStripMenuItem();
            tsmiExport = new ToolStripMenuItem();
            tsmiExport_toDriverFormat = new ToolStripMenuItem();
            tsmiExport_MuctoD88 = new ToolStripMenuItem();
            tsmiExport_MuctoVGM = new ToolStripMenuItem();
            tsmiExport_toWaveFile = new ToolStripMenuItem();
            tsmiExport_toWaveFileAllChSolo = new ToolStripMenuItem();
            tsmiExport_toMp3File = new ToolStripMenuItem();
            tsmiExport_toMidiFile = new ToolStripMenuItem();
            tsmiExport_toDriverFormatAndPlay = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            TsmiExit = new ToolStripMenuItem();
            TsmiEdit = new ToolStripMenuItem();
            TsmiUndo = new ToolStripMenuItem();
            TsmiRedo = new ToolStripMenuItem();
            toolStripSeparator4 = new ToolStripSeparator();
            TsmiFind = new ToolStripMenuItem();
            TsmiFindNext = new ToolStripMenuItem();
            TsmiFindPrevious = new ToolStripMenuItem();
            toolStripSeparator6 = new ToolStripSeparator();
            TsmiReplace = new ToolStripMenuItem();
            表示VToolStripMenuItem = new ToolStripMenuItem();
            TsmiShowPartCounter = new ToolStripMenuItem();
            TsmiShowFolderTree = new ToolStripMenuItem();
            TsmiShowErrorList = new ToolStripMenuItem();
            TsmiShowLog = new ToolStripMenuItem();
            TsmiShowLyrics = new ToolStripMenuItem();
            TsmiShowSien = new ToolStripMenuItem();
            TsmiShowMixer = new ToolStripMenuItem();
            TsmiShowMIDIKbd = new ToolStripMenuItem();
            toolStripSeparator3 = new ToolStripSeparator();
            TsmiFunctionKey = new ToolStripMenuItem();
            TsmiFncHide = new ToolStripMenuItem();
            TsmiFncButtonOnly = new ToolStripMenuItem();
            TsmiFncButtonAndText = new ToolStripMenuItem();
            コンパイルCToolStripMenuItem = new ToolStripMenuItem();
            TsmiCompileAndPlay = new ToolStripMenuItem();
            TsmiCompileAndTracePlay = new ToolStripMenuItem();
            TsmiCompileAndSkipPlay = new ToolStripMenuItem();
            TsmiAllCompile = new ToolStripMenuItem();
            TsmiCompile = new ToolStripMenuItem();
            featureToolStripMenuItem = new ToolStripMenuItem();
            tsmiJumpSoloMode = new ToolStripMenuItem();
            tsmiScript = new ToolStripMenuItem();
            ツールTToolStripMenuItem = new ToolStripMenuItem();
            tsmiMakeCSM = new ToolStripMenuItem();
            tsmiOption = new ToolStripMenuItem();
            TsmiHelp = new ToolStripMenuItem();
            TsmiTutorial = new ToolStripMenuItem();
            TsmiReference = new ToolStripMenuItem();
            TsmiReferenceMucom = new ToolStripMenuItem();
            TsmiReferencePMD = new ToolStripMenuItem();
            TsmiReferenceM98 = new ToolStripMenuItem();
            toolStripSeparator5 = new ToolStripSeparator();
            TsmiAbout = new ToolStripMenuItem();
            timer = new System.Windows.Forms.Timer(components);
            statusStrip1.SuspendLayout();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // dpMain
            // 
            dpMain.AllowDrop = true;
            resources.ApplyResources(dpMain, "dpMain");
            dpMain.Name = "dpMain";
            dpMain.ShowDocumentIcon = true;
            dpMain.ActiveDocumentChanged += DpMain_ActiveDocumentChanged;
            dpMain.DragDrop += DpMain_DragDrop;
            dpMain.DragOver += DpMain_DragOver;
            // 
            // visualStudioToolStripExtender1
            // 
            visualStudioToolStripExtender1.DefaultRenderer = null;
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new Size(24, 24);
            statusStrip1.Items.AddRange(new ToolStripItem[] { tsslCompileError, tsslCompileWarning, tsslCompileStatus, TsslLineCol, tsslJumpSoloMode, toolStripStatusLabel1, tsslSimpleKbdMode, tsslStatus, tssbOpen, tssbSave, tssbFind, tssbCompile, tssbPlay, tssbM98, tssbPause, tssbStop, tssbSlow, tssbFast, tssbExpAndMdp, tssbSien, tssbMIDIKbd });
            resources.ApplyResources(statusStrip1, "statusStrip1");
            statusStrip1.Name = "statusStrip1";
            statusStrip1.ShowItemToolTips = true;
            // 
            // tsslCompileError
            // 
            tsslCompileError.AutoToolTip = true;
            tsslCompileError.Image = Resources.Error;
            resources.ApplyResources(tsslCompileError, "tsslCompileError");
            tsslCompileError.Name = "tsslCompileError";
            tsslCompileError.Click += TsslCompileError_Click;
            // 
            // tsslCompileWarning
            // 
            tsslCompileWarning.AutoToolTip = true;
            tsslCompileWarning.Image = Resources.Warning;
            resources.ApplyResources(tsslCompileWarning, "tsslCompileWarning");
            tsslCompileWarning.Name = "tsslCompileWarning";
            tsslCompileWarning.Click += TsslCompileWarning_Click;
            // 
            // tsslCompileStatus
            // 
            tsslCompileStatus.Name = "tsslCompileStatus";
            resources.ApplyResources(tsslCompileStatus, "tsslCompileStatus");
            // 
            // TsslLineCol
            // 
            TsslLineCol.Name = "TsslLineCol";
            resources.ApplyResources(TsslLineCol, "TsslLineCol");
            // 
            // tsslJumpSoloMode
            // 
            tsslJumpSoloMode.Name = "tsslJumpSoloMode";
            resources.ApplyResources(tsslJumpSoloMode, "tsslJumpSoloMode");
            // 
            // toolStripStatusLabel1
            // 
            resources.ApplyResources(toolStripStatusLabel1, "toolStripStatusLabel1");
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Click += toolStripStatusLabel1_Click;
            // 
            // tsslSimpleKbdMode
            // 
            tsslSimpleKbdMode.Name = "tsslSimpleKbdMode";
            resources.ApplyResources(tsslSimpleKbdMode, "tsslSimpleKbdMode");
            // 
            // tsslStatus
            // 
            resources.ApplyResources(tsslStatus, "tsslStatus");
            tsslStatus.Name = "tsslStatus";
            tsslStatus.Spring = true;
            // 
            // tssbOpen
            // 
            tssbOpen.DropDownButtonWidth = 0;
            tssbOpen.Image = Resources.F01;
            resources.ApplyResources(tssbOpen, "tssbOpen");
            tssbOpen.Name = "tssbOpen";
            tssbOpen.ButtonClick += TssbOpen_ButtonClick;
            // 
            // tssbSave
            // 
            tssbSave.DropDownButtonWidth = 0;
            resources.ApplyResources(tssbSave, "tssbSave");
            tssbSave.Image = Resources.F02;
            tssbSave.Name = "tssbSave";
            tssbSave.ButtonClick += TssbSave_ButtonClick;
            // 
            // tssbFind
            // 
            tssbFind.DropDownButtonWidth = 0;
            tssbFind.Image = Resources.F03;
            resources.ApplyResources(tssbFind, "tssbFind");
            tssbFind.Name = "tssbFind";
            tssbFind.ButtonClick += TssbFind_ButtonClick;
            // 
            // tssbCompile
            // 
            tssbCompile.DropDownButtonWidth = 0;
            tssbCompile.Image = Resources.F04;
            resources.ApplyResources(tssbCompile, "tssbCompile");
            tssbCompile.Name = "tssbCompile";
            tssbCompile.ButtonClick += tssbCompile_ButtonClick_1;
            // 
            // tssbPlay
            // 
            tssbPlay.DropDownButtonWidth = 0;
            tssbPlay.Image = Resources.F05;
            resources.ApplyResources(tssbPlay, "tssbPlay");
            tssbPlay.Name = "tssbPlay";
            tssbPlay.ButtonClick += TssbPlay_ButtonClick;
            // 
            // tssbM98
            // 
            tssbM98.DropDownButtonWidth = 0;
            tssbM98.Image = Resources.F06;
            resources.ApplyResources(tssbM98, "tssbM98");
            tssbM98.Name = "tssbM98";
            tssbM98.ButtonClick += tssbM98_ButtonClick;
            // 
            // tssbPause
            // 
            tssbPause.DropDownButtonWidth = 0;
            tssbPause.Image = Resources.F08;
            resources.ApplyResources(tssbPause, "tssbPause");
            tssbPause.Name = "tssbPause";
            tssbPause.ButtonClick += tssbPause_ButtonClick;
            // 
            // tssbStop
            // 
            tssbStop.DropDownButtonWidth = 0;
            tssbStop.Image = Resources.F09;
            resources.ApplyResources(tssbStop, "tssbStop");
            tssbStop.Name = "tssbStop";
            tssbStop.ButtonClick += TssbStop_ButtonClick;
            // 
            // tssbSlow
            // 
            tssbSlow.DropDownButtonWidth = 0;
            tssbSlow.Image = Resources.F10;
            resources.ApplyResources(tssbSlow, "tssbSlow");
            tssbSlow.Name = "tssbSlow";
            tssbSlow.ButtonClick += TssbSlow_ButtonClick;
            // 
            // tssbFast
            // 
            tssbFast.DropDownButtonWidth = 0;
            tssbFast.Image = Resources.F11;
            resources.ApplyResources(tssbFast, "tssbFast");
            tssbFast.Name = "tssbFast";
            tssbFast.ButtonClick += TssbFast_ButtonClick;
            // 
            // tssbExpAndMdp
            // 
            tssbExpAndMdp.DropDownButtonWidth = 0;
            tssbExpAndMdp.Image = Resources.F12;
            resources.ApplyResources(tssbExpAndMdp, "tssbExpAndMdp");
            tssbExpAndMdp.Name = "tssbExpAndMdp";
            tssbExpAndMdp.ButtonClick += tssbExpAndMdp_ButtonClick;
            // 
            // tssbSien
            // 
            tssbSien.DropDownButtonWidth = 0;
            tssbSien.Image = Resources.F12;
            resources.ApplyResources(tssbSien, "tssbSien");
            tssbSien.Name = "tssbSien";
            // 
            // tssbMIDIKbd
            // 
            tssbMIDIKbd.DropDownButtonWidth = 0;
            tssbMIDIKbd.Image = Resources.F12;
            resources.ApplyResources(tssbMIDIKbd, "tssbMIDIKbd");
            tssbMIDIKbd.Name = "tssbMIDIKbd";
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(24, 24);
            menuStrip1.Items.AddRange(new ToolStripItem[] { TsmiFile, TsmiEdit, 表示VToolStripMenuItem, コンパイルCToolStripMenuItem, featureToolStripMenuItem, tsmiScript, ツールTToolStripMenuItem, TsmiHelp });
            resources.ApplyResources(menuStrip1, "menuStrip1");
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Stretch = false;
            // 
            // TsmiFile
            // 
            TsmiFile.DropDownItems.AddRange(new ToolStripItem[] { TsmiNew, TsmiOpenFile, TsmiOpenFolder, tsmiGwiFileHistory, TsmiSaveFile, TsmiSaveAs, toolStripSeparator2, tsmiImport, tsmiExport, toolStripSeparator1, TsmiExit });
            TsmiFile.Name = "TsmiFile";
            resources.ApplyResources(TsmiFile, "TsmiFile");
            // 
            // TsmiNew
            // 
            TsmiNew.DropDownItems.AddRange(new ToolStripItem[] { tsmiNewGwi, tsmiNewMuc, tsmiNewMml });
            TsmiNew.Name = "TsmiNew";
            resources.ApplyResources(TsmiNew, "TsmiNew");
            // 
            // tsmiNewGwi
            // 
            tsmiNewGwi.Name = "tsmiNewGwi";
            resources.ApplyResources(tsmiNewGwi, "tsmiNewGwi");
            tsmiNewGwi.Click += tsmiNewGwi_Click;
            // 
            // tsmiNewMuc
            // 
            tsmiNewMuc.Name = "tsmiNewMuc";
            resources.ApplyResources(tsmiNewMuc, "tsmiNewMuc");
            tsmiNewMuc.Click += tsmiNewMuc_Click;
            // 
            // tsmiNewMml
            // 
            tsmiNewMml.Name = "tsmiNewMml";
            resources.ApplyResources(tsmiNewMml, "tsmiNewMml");
            tsmiNewMml.Click += tsmiNewMml_Click;
            // 
            // TsmiOpenFile
            // 
            TsmiOpenFile.Name = "TsmiOpenFile";
            resources.ApplyResources(TsmiOpenFile, "TsmiOpenFile");
            TsmiOpenFile.Click += TsmiFileOpen_Click;
            // 
            // TsmiOpenFolder
            // 
            resources.ApplyResources(TsmiOpenFolder, "TsmiOpenFolder");
            TsmiOpenFolder.Name = "TsmiOpenFolder";
            TsmiOpenFolder.Click += TsmiOpenFolder_Click;
            // 
            // tsmiGwiFileHistory
            // 
            tsmiGwiFileHistory.Name = "tsmiGwiFileHistory";
            resources.ApplyResources(tsmiGwiFileHistory, "tsmiGwiFileHistory");
            // 
            // TsmiSaveFile
            // 
            resources.ApplyResources(TsmiSaveFile, "TsmiSaveFile");
            TsmiSaveFile.Name = "TsmiSaveFile";
            TsmiSaveFile.Click += TsmiSaveFile_Click;
            // 
            // TsmiSaveAs
            // 
            resources.ApplyResources(TsmiSaveAs, "TsmiSaveAs");
            TsmiSaveAs.Name = "TsmiSaveAs";
            TsmiSaveAs.Click += TsmiSaveAs_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(toolStripSeparator2, "toolStripSeparator2");
            // 
            // tsmiImport
            // 
            tsmiImport.DropDownItems.AddRange(new ToolStripItem[] { tsmiImport_D88toMuc, tsmiImport_MIDtoGWI });
            tsmiImport.Name = "tsmiImport";
            resources.ApplyResources(tsmiImport, "tsmiImport");
            tsmiImport.Click += TsmiImport_Click;
            // 
            // tsmiImport_D88toMuc
            // 
            tsmiImport_D88toMuc.Name = "tsmiImport_D88toMuc";
            resources.ApplyResources(tsmiImport_D88toMuc, "tsmiImport_D88toMuc");
            tsmiImport_D88toMuc.Click += tsmiImport_D88toMuc_Click;
            // 
            // tsmiImport_MIDtoGWI
            // 
            tsmiImport_MIDtoGWI.Name = "tsmiImport_MIDtoGWI";
            resources.ApplyResources(tsmiImport_MIDtoGWI, "tsmiImport_MIDtoGWI");
            tsmiImport_MIDtoGWI.Click += tsmiImport_MIDtoGWI_Click;
            // 
            // tsmiExport
            // 
            tsmiExport.DropDownItems.AddRange(new ToolStripItem[] { tsmiExport_toDriverFormat, tsmiExport_MuctoD88, tsmiExport_MuctoVGM, tsmiExport_toWaveFile, tsmiExport_toWaveFileAllChSolo, tsmiExport_toMp3File, tsmiExport_toMidiFile, tsmiExport_toDriverFormatAndPlay });
            tsmiExport.Name = "tsmiExport";
            resources.ApplyResources(tsmiExport, "tsmiExport");
            // 
            // tsmiExport_toDriverFormat
            // 
            tsmiExport_toDriverFormat.Name = "tsmiExport_toDriverFormat";
            resources.ApplyResources(tsmiExport_toDriverFormat, "tsmiExport_toDriverFormat");
            tsmiExport_toDriverFormat.Click += tsmiExport_toDriverFormat_Click;
            // 
            // tsmiExport_MuctoD88
            // 
            tsmiExport_MuctoD88.Name = "tsmiExport_MuctoD88";
            resources.ApplyResources(tsmiExport_MuctoD88, "tsmiExport_MuctoD88");
            tsmiExport_MuctoD88.Click += tsmiExport_MuctoD88_Click;
            // 
            // tsmiExport_MuctoVGM
            // 
            tsmiExport_MuctoVGM.Name = "tsmiExport_MuctoVGM";
            resources.ApplyResources(tsmiExport_MuctoVGM, "tsmiExport_MuctoVGM");
            tsmiExport_MuctoVGM.Click += tsmiExport_MuctoVGM_Click;
            // 
            // tsmiExport_toWaveFile
            // 
            tsmiExport_toWaveFile.Name = "tsmiExport_toWaveFile";
            resources.ApplyResources(tsmiExport_toWaveFile, "tsmiExport_toWaveFile");
            tsmiExport_toWaveFile.Click += tsmiExport_toWaveFile_Click;
            // 
            // tsmiExport_toWaveFileAllChSolo
            // 
            tsmiExport_toWaveFileAllChSolo.Name = "tsmiExport_toWaveFileAllChSolo";
            resources.ApplyResources(tsmiExport_toWaveFileAllChSolo, "tsmiExport_toWaveFileAllChSolo");
            tsmiExport_toWaveFileAllChSolo.Click += tsmiExport_toWaveFileAllChSolo_Click;
            // 
            // tsmiExport_toMp3File
            // 
            tsmiExport_toMp3File.Name = "tsmiExport_toMp3File";
            resources.ApplyResources(tsmiExport_toMp3File, "tsmiExport_toMp3File");
            tsmiExport_toMp3File.Click += tsmiExport_toWaveFile_Click;
            // 
            // tsmiExport_toMidiFile
            // 
            tsmiExport_toMidiFile.Name = "tsmiExport_toMidiFile";
            resources.ApplyResources(tsmiExport_toMidiFile, "tsmiExport_toMidiFile");
            tsmiExport_toMidiFile.Click += tsmiExport_toMidiFile_Click;
            // 
            // tsmiExport_toDriverFormatAndPlay
            // 
            tsmiExport_toDriverFormatAndPlay.Name = "tsmiExport_toDriverFormatAndPlay";
            resources.ApplyResources(tsmiExport_toDriverFormatAndPlay, "tsmiExport_toDriverFormatAndPlay");
            tsmiExport_toDriverFormatAndPlay.Click += tsmiExport_toDriverFormatAndPlay_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(toolStripSeparator1, "toolStripSeparator1");
            // 
            // TsmiExit
            // 
            TsmiExit.Name = "TsmiExit";
            resources.ApplyResources(TsmiExit, "TsmiExit");
            TsmiExit.Click += TsmiExit_Click;
            // 
            // TsmiEdit
            // 
            TsmiEdit.DropDownItems.AddRange(new ToolStripItem[] { TsmiUndo, TsmiRedo, toolStripSeparator4, TsmiFind, TsmiFindNext, TsmiFindPrevious, toolStripSeparator6, TsmiReplace });
            TsmiEdit.Name = "TsmiEdit";
            resources.ApplyResources(TsmiEdit, "TsmiEdit");
            // 
            // TsmiUndo
            // 
            resources.ApplyResources(TsmiUndo, "TsmiUndo");
            TsmiUndo.Name = "TsmiUndo";
            TsmiUndo.Click += TsmiUndo_Click;
            // 
            // TsmiRedo
            // 
            resources.ApplyResources(TsmiRedo, "TsmiRedo");
            TsmiRedo.Name = "TsmiRedo";
            TsmiRedo.Click += TsmiRedo_Click;
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(toolStripSeparator4, "toolStripSeparator4");
            // 
            // TsmiFind
            // 
            TsmiFind.Name = "TsmiFind";
            resources.ApplyResources(TsmiFind, "TsmiFind");
            TsmiFind.Click += TsmiFind_Click;
            // 
            // TsmiFindNext
            // 
            TsmiFindNext.Name = "TsmiFindNext";
            resources.ApplyResources(TsmiFindNext, "TsmiFindNext");
            TsmiFindNext.Click += TsmiFindNext_Click;
            // 
            // TsmiFindPrevious
            // 
            TsmiFindPrevious.Name = "TsmiFindPrevious";
            resources.ApplyResources(TsmiFindPrevious, "TsmiFindPrevious");
            TsmiFindPrevious.Click += TsmiFindPrevious_Click;
            // 
            // toolStripSeparator6
            // 
            toolStripSeparator6.Name = "toolStripSeparator6";
            resources.ApplyResources(toolStripSeparator6, "toolStripSeparator6");
            // 
            // TsmiReplace
            // 
            TsmiReplace.Name = "TsmiReplace";
            resources.ApplyResources(TsmiReplace, "TsmiReplace");
            TsmiReplace.Click += TsmiReplace_Click;
            // 
            // 表示VToolStripMenuItem
            // 
            表示VToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { TsmiShowPartCounter, TsmiShowFolderTree, TsmiShowErrorList, TsmiShowLog, TsmiShowLyrics, TsmiShowSien, TsmiShowMixer, TsmiShowMIDIKbd, toolStripSeparator3, TsmiFunctionKey });
            表示VToolStripMenuItem.Name = "表示VToolStripMenuItem";
            resources.ApplyResources(表示VToolStripMenuItem, "表示VToolStripMenuItem");
            // 
            // TsmiShowPartCounter
            // 
            TsmiShowPartCounter.Name = "TsmiShowPartCounter";
            resources.ApplyResources(TsmiShowPartCounter, "TsmiShowPartCounter");
            TsmiShowPartCounter.Click += TsmiShowPartCounter_Click;
            // 
            // TsmiShowFolderTree
            // 
            TsmiShowFolderTree.Name = "TsmiShowFolderTree";
            resources.ApplyResources(TsmiShowFolderTree, "TsmiShowFolderTree");
            TsmiShowFolderTree.Click += TsmiShowFolderTree_Click;
            // 
            // TsmiShowErrorList
            // 
            TsmiShowErrorList.Name = "TsmiShowErrorList";
            resources.ApplyResources(TsmiShowErrorList, "TsmiShowErrorList");
            TsmiShowErrorList.Click += TsmiShowErrorList_Click;
            // 
            // TsmiShowLog
            // 
            TsmiShowLog.Name = "TsmiShowLog";
            resources.ApplyResources(TsmiShowLog, "TsmiShowLog");
            TsmiShowLog.Click += TsmiShowLog_Click;
            // 
            // TsmiShowLyrics
            // 
            TsmiShowLyrics.Name = "TsmiShowLyrics";
            resources.ApplyResources(TsmiShowLyrics, "TsmiShowLyrics");
            TsmiShowLyrics.Click += TsmiShowLyrics_Click;
            // 
            // TsmiShowSien
            // 
            TsmiShowSien.Name = "TsmiShowSien";
            resources.ApplyResources(TsmiShowSien, "TsmiShowSien");
            TsmiShowSien.Click += TsmiShowSien_Click;
            // 
            // TsmiShowMixer
            // 
            TsmiShowMixer.Name = "TsmiShowMixer";
            resources.ApplyResources(TsmiShowMixer, "TsmiShowMixer");
            TsmiShowMixer.Click += TsmiShowMixer_Click;
            // 
            // TsmiShowMIDIKbd
            // 
            TsmiShowMIDIKbd.Name = "TsmiShowMIDIKbd";
            resources.ApplyResources(TsmiShowMIDIKbd, "TsmiShowMIDIKbd");
            TsmiShowMIDIKbd.Click += TsmiShowMIDIKbd_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(toolStripSeparator3, "toolStripSeparator3");
            // 
            // TsmiFunctionKey
            // 
            TsmiFunctionKey.DropDownItems.AddRange(new ToolStripItem[] { TsmiFncHide, TsmiFncButtonOnly, TsmiFncButtonAndText });
            TsmiFunctionKey.Name = "TsmiFunctionKey";
            resources.ApplyResources(TsmiFunctionKey, "TsmiFunctionKey");
            // 
            // TsmiFncHide
            // 
            TsmiFncHide.Name = "TsmiFncHide";
            resources.ApplyResources(TsmiFncHide, "TsmiFncHide");
            TsmiFncHide.Click += TsmiFncHide_Click;
            // 
            // TsmiFncButtonOnly
            // 
            TsmiFncButtonOnly.Name = "TsmiFncButtonOnly";
            resources.ApplyResources(TsmiFncButtonOnly, "TsmiFncButtonOnly");
            TsmiFncButtonOnly.Click += TsmiFncButtonOnly_Click;
            // 
            // TsmiFncButtonAndText
            // 
            TsmiFncButtonAndText.Name = "TsmiFncButtonAndText";
            resources.ApplyResources(TsmiFncButtonAndText, "TsmiFncButtonAndText");
            TsmiFncButtonAndText.Click += TsmiFncButtonAndText_Click;
            // 
            // コンパイルCToolStripMenuItem
            // 
            コンパイルCToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { TsmiCompileAndPlay, TsmiCompileAndTracePlay, TsmiCompileAndSkipPlay, TsmiAllCompile, TsmiCompile });
            コンパイルCToolStripMenuItem.Name = "コンパイルCToolStripMenuItem";
            resources.ApplyResources(コンパイルCToolStripMenuItem, "コンパイルCToolStripMenuItem");
            // 
            // TsmiCompileAndPlay
            // 
            TsmiCompileAndPlay.Name = "TsmiCompileAndPlay";
            resources.ApplyResources(TsmiCompileAndPlay, "TsmiCompileAndPlay");
            TsmiCompileAndPlay.Click += TsmiCompileAndPlay_Click;
            // 
            // TsmiCompileAndTracePlay
            // 
            TsmiCompileAndTracePlay.Name = "TsmiCompileAndTracePlay";
            resources.ApplyResources(TsmiCompileAndTracePlay, "TsmiCompileAndTracePlay");
            TsmiCompileAndTracePlay.Click += TsmiCompileAndTracePlay_Click;
            // 
            // TsmiCompileAndSkipPlay
            // 
            TsmiCompileAndSkipPlay.Name = "TsmiCompileAndSkipPlay";
            resources.ApplyResources(TsmiCompileAndSkipPlay, "TsmiCompileAndSkipPlay");
            TsmiCompileAndSkipPlay.Click += TsmiCompileAndSkipPlay_Click;
            // 
            // TsmiAllCompile
            // 
            resources.ApplyResources(TsmiAllCompile, "TsmiAllCompile");
            TsmiAllCompile.Name = "TsmiAllCompile";
            // 
            // TsmiCompile
            // 
            TsmiCompile.Name = "TsmiCompile";
            resources.ApplyResources(TsmiCompile, "TsmiCompile");
            TsmiCompile.Click += TsmiCompile_Click;
            // 
            // featureToolStripMenuItem
            // 
            featureToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { tsmiJumpSoloMode });
            featureToolStripMenuItem.Name = "featureToolStripMenuItem";
            resources.ApplyResources(featureToolStripMenuItem, "featureToolStripMenuItem");
            // 
            // tsmiJumpSoloMode
            // 
            tsmiJumpSoloMode.CheckOnClick = true;
            tsmiJumpSoloMode.Name = "tsmiJumpSoloMode";
            resources.ApplyResources(tsmiJumpSoloMode, "tsmiJumpSoloMode");
            // 
            // tsmiScript
            // 
            tsmiScript.Name = "tsmiScript";
            resources.ApplyResources(tsmiScript, "tsmiScript");
            // 
            // ツールTToolStripMenuItem
            // 
            ツールTToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { tsmiMakeCSM, tsmiOption });
            ツールTToolStripMenuItem.Name = "ツールTToolStripMenuItem";
            resources.ApplyResources(ツールTToolStripMenuItem, "ツールTToolStripMenuItem");
            // 
            // tsmiMakeCSM
            // 
            tsmiMakeCSM.Name = "tsmiMakeCSM";
            resources.ApplyResources(tsmiMakeCSM, "tsmiMakeCSM");
            tsmiMakeCSM.Click += tsmiMakeCSM_Click;
            // 
            // tsmiOption
            // 
            tsmiOption.Name = "tsmiOption";
            resources.ApplyResources(tsmiOption, "tsmiOption");
            tsmiOption.Click += TsmiOption_Click;
            // 
            // TsmiHelp
            // 
            TsmiHelp.DropDownItems.AddRange(new ToolStripItem[] { TsmiTutorial, TsmiReference, TsmiReferenceMucom, TsmiReferencePMD, TsmiReferenceM98, toolStripSeparator5, TsmiAbout });
            TsmiHelp.Name = "TsmiHelp";
            resources.ApplyResources(TsmiHelp, "TsmiHelp");
            // 
            // TsmiTutorial
            // 
            TsmiTutorial.Name = "TsmiTutorial";
            resources.ApplyResources(TsmiTutorial, "TsmiTutorial");
            TsmiTutorial.Click += TsmiTutorial_Click;
            // 
            // TsmiReference
            // 
            TsmiReference.Name = "TsmiReference";
            resources.ApplyResources(TsmiReference, "TsmiReference");
            TsmiReference.Click += TsmiReference_Click;
            // 
            // TsmiReferenceMucom
            // 
            TsmiReferenceMucom.Name = "TsmiReferenceMucom";
            resources.ApplyResources(TsmiReferenceMucom, "TsmiReferenceMucom");
            TsmiReferenceMucom.Click += TsmiReferenceMucom_Click;
            // 
            // TsmiReferencePMD
            // 
            TsmiReferencePMD.Name = "TsmiReferencePMD";
            resources.ApplyResources(TsmiReferencePMD, "TsmiReferencePMD");
            TsmiReferencePMD.Click += TsmiReferencePMD_Click;
            // 
            // TsmiReferenceM98
            // 
            TsmiReferenceM98.Name = "TsmiReferenceM98";
            resources.ApplyResources(TsmiReferenceM98, "TsmiReferenceM98");
            TsmiReferenceM98.Click += TsmiReferenceM98_Click;
            // 
            // toolStripSeparator5
            // 
            toolStripSeparator5.Name = "toolStripSeparator5";
            resources.ApplyResources(toolStripSeparator5, "toolStripSeparator5");
            // 
            // TsmiAbout
            // 
            TsmiAbout.Name = "TsmiAbout";
            resources.ApplyResources(TsmiAbout, "TsmiAbout");
            TsmiAbout.Click += TsmiAbout_Click;
            // 
            // timer
            // 
            timer.Enabled = true;
            timer.Interval = 16;
            timer.Tick += Timer_Tick;
            // 
            // FrmMain
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(statusStrip1);
            Controls.Add(menuStrip1);
            Controls.Add(dpMain);
            KeyPreview = true;
            MainMenuStrip = menuStrip1;
            Name = "FrmMain";
            FormClosing += FrmMain_FormClosing;
            Load += FrmMain_Load;
            Shown += Form1_Shown;
            KeyDown += FrmMain_KeyDown;
            KeyUp += FrmMain_KeyUp;
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private WeifenLuo.WinFormsUI.Docking.VisualStudioToolStripExtender visualStudioToolStripExtender1;
        private WeifenLuo.WinFormsUI.Docking.VS2015DarkTheme vS2005Theme1;
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
        private System.Windows.Forms.ToolStripMenuItem featureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsmiJumpSoloMode;
        private System.Windows.Forms.ToolStripStatusLabel tsslJumpSoloMode;
        private System.Windows.Forms.ToolStripSplitButton tssbPause;
        private System.Windows.Forms.ToolStripMenuItem tsmiNewGwi;
        private System.Windows.Forms.ToolStripMenuItem tsmiNewMuc;
        private System.Windows.Forms.ToolStripMenuItem tsmiNewMml;
        private System.Windows.Forms.ToolStripMenuItem TsmiReferenceMucom;
        private System.Windows.Forms.ToolStripMenuItem TsmiReferencePMD;
        private System.Windows.Forms.ToolStripMenuItem TsmiReferenceM98;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem tsmiImport_D88toMuc;
        private System.Windows.Forms.ToolStripMenuItem tsmiExport_toDriverFormat;
        private System.Windows.Forms.ToolStripMenuItem tsmiExport_MuctoD88;
        private System.Windows.Forms.ToolStripMenuItem tsmiExport_toWaveFile;
        private System.Windows.Forms.ToolStripMenuItem tsmiExport_toMidiFile;
        private System.Windows.Forms.ToolStripSplitButton tssbSien;
        private System.Windows.Forms.ToolStripMenuItem TsmiShowSien;
        private System.Windows.Forms.ToolStripMenuItem tsmiExport_toMp3File;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem TsmiReplace;
        private System.Windows.Forms.ToolStripMenuItem tsmiMakeCSM;
        private System.Windows.Forms.ToolStripMenuItem tsmiExport_toDriverFormatAndPlay;
        private System.Windows.Forms.ToolStripSplitButton tssbExpAndMdp;
        private System.Windows.Forms.ToolStripMenuItem tsmiImport_MIDtoGWI;
        private System.Windows.Forms.ToolStripMenuItem tsmiExport_MuctoVGM;
        private System.Windows.Forms.ToolStripMenuItem tsmiExport_toWaveFileAllChSolo;
        public System.Windows.Forms.ToolStripStatusLabel tsslStatus;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private ToolStripStatusLabel tsslSimpleKbdMode;
    }
}

