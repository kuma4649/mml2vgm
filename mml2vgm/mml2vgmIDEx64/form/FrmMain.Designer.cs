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
            tsmiNewMus = new ToolStripMenuItem();
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
            TsmiReferenceMuap = new ToolStripMenuItem();
            toolStripSeparator5 = new ToolStripSeparator();
            TsmiAbout = new ToolStripMenuItem();
            timer = new System.Windows.Forms.Timer(components);
            statusStrip1.SuspendLayout();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // dpMain
            // 
            resources.ApplyResources(dpMain, "dpMain");
            dpMain.AllowDrop = true;
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
            resources.ApplyResources(statusStrip1, "statusStrip1");
            statusStrip1.ImageScalingSize = new Size(24, 24);
            statusStrip1.Items.AddRange(new ToolStripItem[] { tsslCompileError, tsslCompileWarning, tsslCompileStatus, TsslLineCol, tsslJumpSoloMode, toolStripStatusLabel1, tsslSimpleKbdMode, tsslStatus, tssbOpen, tssbSave, tssbFind, tssbCompile, tssbPlay, tssbM98, tssbPause, tssbStop, tssbSlow, tssbFast, tssbExpAndMdp, tssbSien, tssbMIDIKbd });
            statusStrip1.Name = "statusStrip1";
            statusStrip1.ShowItemToolTips = true;
            // 
            // tsslCompileError
            // 
            resources.ApplyResources(tsslCompileError, "tsslCompileError");
            tsslCompileError.AutoToolTip = true;
            tsslCompileError.Image = Resources.Error;
            tsslCompileError.Name = "tsslCompileError";
            tsslCompileError.Click += TsslCompileError_Click;
            // 
            // tsslCompileWarning
            // 
            resources.ApplyResources(tsslCompileWarning, "tsslCompileWarning");
            tsslCompileWarning.AutoToolTip = true;
            tsslCompileWarning.Image = Resources.Warning;
            tsslCompileWarning.Name = "tsslCompileWarning";
            tsslCompileWarning.Click += TsslCompileWarning_Click;
            // 
            // tsslCompileStatus
            // 
            resources.ApplyResources(tsslCompileStatus, "tsslCompileStatus");
            tsslCompileStatus.Name = "tsslCompileStatus";
            // 
            // TsslLineCol
            // 
            resources.ApplyResources(TsslLineCol, "TsslLineCol");
            TsslLineCol.Name = "TsslLineCol";
            // 
            // tsslJumpSoloMode
            // 
            resources.ApplyResources(tsslJumpSoloMode, "tsslJumpSoloMode");
            tsslJumpSoloMode.Name = "tsslJumpSoloMode";
            // 
            // toolStripStatusLabel1
            // 
            resources.ApplyResources(toolStripStatusLabel1, "toolStripStatusLabel1");
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Click += toolStripStatusLabel1_Click;
            // 
            // tsslSimpleKbdMode
            // 
            resources.ApplyResources(tsslSimpleKbdMode, "tsslSimpleKbdMode");
            tsslSimpleKbdMode.Name = "tsslSimpleKbdMode";
            // 
            // tsslStatus
            // 
            resources.ApplyResources(tsslStatus, "tsslStatus");
            tsslStatus.Name = "tsslStatus";
            tsslStatus.Spring = true;
            // 
            // tssbOpen
            // 
            resources.ApplyResources(tssbOpen, "tssbOpen");
            tssbOpen.DropDownButtonWidth = 0;
            tssbOpen.Image = Resources.F01;
            tssbOpen.Name = "tssbOpen";
            tssbOpen.ButtonClick += TssbOpen_ButtonClick;
            // 
            // tssbSave
            // 
            resources.ApplyResources(tssbSave, "tssbSave");
            tssbSave.DropDownButtonWidth = 0;
            tssbSave.Image = Resources.F02;
            tssbSave.Name = "tssbSave";
            tssbSave.ButtonClick += TssbSave_ButtonClick;
            // 
            // tssbFind
            // 
            resources.ApplyResources(tssbFind, "tssbFind");
            tssbFind.DropDownButtonWidth = 0;
            tssbFind.Image = Resources.F03;
            tssbFind.Name = "tssbFind";
            tssbFind.ButtonClick += TssbFind_ButtonClick;
            // 
            // tssbCompile
            // 
            resources.ApplyResources(tssbCompile, "tssbCompile");
            tssbCompile.DropDownButtonWidth = 0;
            tssbCompile.Image = Resources.F04;
            tssbCompile.Name = "tssbCompile";
            tssbCompile.ButtonClick += tssbCompile_ButtonClick_1;
            // 
            // tssbPlay
            // 
            resources.ApplyResources(tssbPlay, "tssbPlay");
            tssbPlay.DropDownButtonWidth = 0;
            tssbPlay.Image = Resources.F05;
            tssbPlay.Name = "tssbPlay";
            tssbPlay.ButtonClick += TssbPlay_ButtonClick;
            // 
            // tssbM98
            // 
            resources.ApplyResources(tssbM98, "tssbM98");
            tssbM98.DropDownButtonWidth = 0;
            tssbM98.Image = Resources.F06;
            tssbM98.Name = "tssbM98";
            tssbM98.ButtonClick += tssbM98_ButtonClick;
            // 
            // tssbPause
            // 
            resources.ApplyResources(tssbPause, "tssbPause");
            tssbPause.DropDownButtonWidth = 0;
            tssbPause.Image = Resources.F08;
            tssbPause.Name = "tssbPause";
            tssbPause.ButtonClick += tssbPause_ButtonClick;
            // 
            // tssbStop
            // 
            resources.ApplyResources(tssbStop, "tssbStop");
            tssbStop.DropDownButtonWidth = 0;
            tssbStop.Image = Resources.F09;
            tssbStop.Name = "tssbStop";
            tssbStop.ButtonClick += TssbStop_ButtonClick;
            // 
            // tssbSlow
            // 
            resources.ApplyResources(tssbSlow, "tssbSlow");
            tssbSlow.DropDownButtonWidth = 0;
            tssbSlow.Image = Resources.F10;
            tssbSlow.Name = "tssbSlow";
            tssbSlow.ButtonClick += TssbSlow_ButtonClick;
            // 
            // tssbFast
            // 
            resources.ApplyResources(tssbFast, "tssbFast");
            tssbFast.DropDownButtonWidth = 0;
            tssbFast.Image = Resources.F11;
            tssbFast.Name = "tssbFast";
            tssbFast.ButtonClick += TssbFast_ButtonClick;
            // 
            // tssbExpAndMdp
            // 
            resources.ApplyResources(tssbExpAndMdp, "tssbExpAndMdp");
            tssbExpAndMdp.DropDownButtonWidth = 0;
            tssbExpAndMdp.Image = Resources.F12;
            tssbExpAndMdp.Name = "tssbExpAndMdp";
            tssbExpAndMdp.ButtonClick += tssbExpAndMdp_ButtonClick;
            // 
            // tssbSien
            // 
            resources.ApplyResources(tssbSien, "tssbSien");
            tssbSien.DropDownButtonWidth = 0;
            tssbSien.Image = Resources.F12;
            tssbSien.Name = "tssbSien";
            // 
            // tssbMIDIKbd
            // 
            resources.ApplyResources(tssbMIDIKbd, "tssbMIDIKbd");
            tssbMIDIKbd.DropDownButtonWidth = 0;
            tssbMIDIKbd.Image = Resources.F12;
            tssbMIDIKbd.Name = "tssbMIDIKbd";
            // 
            // menuStrip1
            // 
            resources.ApplyResources(menuStrip1, "menuStrip1");
            menuStrip1.ImageScalingSize = new Size(24, 24);
            menuStrip1.Items.AddRange(new ToolStripItem[] { TsmiFile, TsmiEdit, 表示VToolStripMenuItem, コンパイルCToolStripMenuItem, featureToolStripMenuItem, tsmiScript, ツールTToolStripMenuItem, TsmiHelp });
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Stretch = false;
            // 
            // TsmiFile
            // 
            resources.ApplyResources(TsmiFile, "TsmiFile");
            TsmiFile.DropDownItems.AddRange(new ToolStripItem[] { TsmiNew, TsmiOpenFile, TsmiOpenFolder, tsmiGwiFileHistory, TsmiSaveFile, TsmiSaveAs, toolStripSeparator2, tsmiImport, tsmiExport, toolStripSeparator1, TsmiExit });
            TsmiFile.Name = "TsmiFile";
            // 
            // TsmiNew
            // 
            resources.ApplyResources(TsmiNew, "TsmiNew");
            TsmiNew.DropDownItems.AddRange(new ToolStripItem[] { tsmiNewGwi, tsmiNewMuc, tsmiNewMml, tsmiNewMus });
            TsmiNew.Name = "TsmiNew";
            // 
            // tsmiNewGwi
            // 
            resources.ApplyResources(tsmiNewGwi, "tsmiNewGwi");
            tsmiNewGwi.Name = "tsmiNewGwi";
            tsmiNewGwi.Click += tsmiNewGwi_Click;
            // 
            // tsmiNewMuc
            // 
            resources.ApplyResources(tsmiNewMuc, "tsmiNewMuc");
            tsmiNewMuc.Name = "tsmiNewMuc";
            tsmiNewMuc.Click += tsmiNewMuc_Click;
            // 
            // tsmiNewMml
            // 
            resources.ApplyResources(tsmiNewMml, "tsmiNewMml");
            tsmiNewMml.Name = "tsmiNewMml";
            tsmiNewMml.Click += tsmiNewMml_Click;
            // 
            // tsmiNewMus
            // 
            resources.ApplyResources(tsmiNewMus, "tsmiNewMus");
            tsmiNewMus.Name = "tsmiNewMus";
            tsmiNewMus.Click += tsmiNewMus_Click;
            // 
            // TsmiOpenFile
            // 
            resources.ApplyResources(TsmiOpenFile, "TsmiOpenFile");
            TsmiOpenFile.Name = "TsmiOpenFile";
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
            resources.ApplyResources(tsmiGwiFileHistory, "tsmiGwiFileHistory");
            tsmiGwiFileHistory.Name = "tsmiGwiFileHistory";
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
            resources.ApplyResources(toolStripSeparator2, "toolStripSeparator2");
            toolStripSeparator2.Name = "toolStripSeparator2";
            // 
            // tsmiImport
            // 
            resources.ApplyResources(tsmiImport, "tsmiImport");
            tsmiImport.DropDownItems.AddRange(new ToolStripItem[] { tsmiImport_D88toMuc, tsmiImport_MIDtoGWI });
            tsmiImport.Name = "tsmiImport";
            tsmiImport.Click += TsmiImport_Click;
            // 
            // tsmiImport_D88toMuc
            // 
            resources.ApplyResources(tsmiImport_D88toMuc, "tsmiImport_D88toMuc");
            tsmiImport_D88toMuc.Name = "tsmiImport_D88toMuc";
            tsmiImport_D88toMuc.Click += tsmiImport_D88toMuc_Click;
            // 
            // tsmiImport_MIDtoGWI
            // 
            resources.ApplyResources(tsmiImport_MIDtoGWI, "tsmiImport_MIDtoGWI");
            tsmiImport_MIDtoGWI.Name = "tsmiImport_MIDtoGWI";
            tsmiImport_MIDtoGWI.Click += tsmiImport_MIDtoGWI_Click;
            // 
            // tsmiExport
            // 
            resources.ApplyResources(tsmiExport, "tsmiExport");
            tsmiExport.DropDownItems.AddRange(new ToolStripItem[] { tsmiExport_toDriverFormat, tsmiExport_MuctoD88, tsmiExport_MuctoVGM, tsmiExport_toWaveFile, tsmiExport_toWaveFileAllChSolo, tsmiExport_toMp3File, tsmiExport_toMidiFile, tsmiExport_toDriverFormatAndPlay });
            tsmiExport.Name = "tsmiExport";
            // 
            // tsmiExport_toDriverFormat
            // 
            resources.ApplyResources(tsmiExport_toDriverFormat, "tsmiExport_toDriverFormat");
            tsmiExport_toDriverFormat.Name = "tsmiExport_toDriverFormat";
            tsmiExport_toDriverFormat.Click += tsmiExport_toDriverFormat_Click;
            // 
            // tsmiExport_MuctoD88
            // 
            resources.ApplyResources(tsmiExport_MuctoD88, "tsmiExport_MuctoD88");
            tsmiExport_MuctoD88.Name = "tsmiExport_MuctoD88";
            tsmiExport_MuctoD88.Click += tsmiExport_MuctoD88_Click;
            // 
            // tsmiExport_MuctoVGM
            // 
            resources.ApplyResources(tsmiExport_MuctoVGM, "tsmiExport_MuctoVGM");
            tsmiExport_MuctoVGM.Name = "tsmiExport_MuctoVGM";
            tsmiExport_MuctoVGM.Click += tsmiExport_MuctoVGM_Click;
            // 
            // tsmiExport_toWaveFile
            // 
            resources.ApplyResources(tsmiExport_toWaveFile, "tsmiExport_toWaveFile");
            tsmiExport_toWaveFile.Name = "tsmiExport_toWaveFile";
            tsmiExport_toWaveFile.Click += tsmiExport_toWaveFile_Click;
            // 
            // tsmiExport_toWaveFileAllChSolo
            // 
            resources.ApplyResources(tsmiExport_toWaveFileAllChSolo, "tsmiExport_toWaveFileAllChSolo");
            tsmiExport_toWaveFileAllChSolo.Name = "tsmiExport_toWaveFileAllChSolo";
            tsmiExport_toWaveFileAllChSolo.Click += tsmiExport_toWaveFileAllChSolo_Click;
            // 
            // tsmiExport_toMp3File
            // 
            resources.ApplyResources(tsmiExport_toMp3File, "tsmiExport_toMp3File");
            tsmiExport_toMp3File.Name = "tsmiExport_toMp3File";
            tsmiExport_toMp3File.Click += tsmiExport_toWaveFile_Click;
            // 
            // tsmiExport_toMidiFile
            // 
            resources.ApplyResources(tsmiExport_toMidiFile, "tsmiExport_toMidiFile");
            tsmiExport_toMidiFile.Name = "tsmiExport_toMidiFile";
            tsmiExport_toMidiFile.Click += tsmiExport_toMidiFile_Click;
            // 
            // tsmiExport_toDriverFormatAndPlay
            // 
            resources.ApplyResources(tsmiExport_toDriverFormatAndPlay, "tsmiExport_toDriverFormatAndPlay");
            tsmiExport_toDriverFormatAndPlay.Name = "tsmiExport_toDriverFormatAndPlay";
            tsmiExport_toDriverFormatAndPlay.Click += tsmiExport_toDriverFormatAndPlay_Click;
            // 
            // toolStripSeparator1
            // 
            resources.ApplyResources(toolStripSeparator1, "toolStripSeparator1");
            toolStripSeparator1.Name = "toolStripSeparator1";
            // 
            // TsmiExit
            // 
            resources.ApplyResources(TsmiExit, "TsmiExit");
            TsmiExit.Name = "TsmiExit";
            TsmiExit.Click += TsmiExit_Click;
            // 
            // TsmiEdit
            // 
            resources.ApplyResources(TsmiEdit, "TsmiEdit");
            TsmiEdit.DropDownItems.AddRange(new ToolStripItem[] { TsmiUndo, TsmiRedo, toolStripSeparator4, TsmiFind, TsmiFindNext, TsmiFindPrevious, toolStripSeparator6, TsmiReplace });
            TsmiEdit.Name = "TsmiEdit";
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
            resources.ApplyResources(toolStripSeparator4, "toolStripSeparator4");
            toolStripSeparator4.Name = "toolStripSeparator4";
            // 
            // TsmiFind
            // 
            resources.ApplyResources(TsmiFind, "TsmiFind");
            TsmiFind.Name = "TsmiFind";
            TsmiFind.Click += TsmiFind_Click;
            // 
            // TsmiFindNext
            // 
            resources.ApplyResources(TsmiFindNext, "TsmiFindNext");
            TsmiFindNext.Name = "TsmiFindNext";
            TsmiFindNext.Click += TsmiFindNext_Click;
            // 
            // TsmiFindPrevious
            // 
            resources.ApplyResources(TsmiFindPrevious, "TsmiFindPrevious");
            TsmiFindPrevious.Name = "TsmiFindPrevious";
            TsmiFindPrevious.Click += TsmiFindPrevious_Click;
            // 
            // toolStripSeparator6
            // 
            resources.ApplyResources(toolStripSeparator6, "toolStripSeparator6");
            toolStripSeparator6.Name = "toolStripSeparator6";
            // 
            // TsmiReplace
            // 
            resources.ApplyResources(TsmiReplace, "TsmiReplace");
            TsmiReplace.Name = "TsmiReplace";
            TsmiReplace.Click += TsmiReplace_Click;
            // 
            // 表示VToolStripMenuItem
            // 
            resources.ApplyResources(表示VToolStripMenuItem, "表示VToolStripMenuItem");
            表示VToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { TsmiShowPartCounter, TsmiShowFolderTree, TsmiShowErrorList, TsmiShowLog, TsmiShowLyrics, TsmiShowSien, TsmiShowMixer, TsmiShowMIDIKbd, toolStripSeparator3, TsmiFunctionKey });
            表示VToolStripMenuItem.Name = "表示VToolStripMenuItem";
            // 
            // TsmiShowPartCounter
            // 
            resources.ApplyResources(TsmiShowPartCounter, "TsmiShowPartCounter");
            TsmiShowPartCounter.Name = "TsmiShowPartCounter";
            TsmiShowPartCounter.Click += TsmiShowPartCounter_Click;
            // 
            // TsmiShowFolderTree
            // 
            resources.ApplyResources(TsmiShowFolderTree, "TsmiShowFolderTree");
            TsmiShowFolderTree.Name = "TsmiShowFolderTree";
            TsmiShowFolderTree.Click += TsmiShowFolderTree_Click;
            // 
            // TsmiShowErrorList
            // 
            resources.ApplyResources(TsmiShowErrorList, "TsmiShowErrorList");
            TsmiShowErrorList.Name = "TsmiShowErrorList";
            TsmiShowErrorList.Click += TsmiShowErrorList_Click;
            // 
            // TsmiShowLog
            // 
            resources.ApplyResources(TsmiShowLog, "TsmiShowLog");
            TsmiShowLog.Name = "TsmiShowLog";
            TsmiShowLog.Click += TsmiShowLog_Click;
            // 
            // TsmiShowLyrics
            // 
            resources.ApplyResources(TsmiShowLyrics, "TsmiShowLyrics");
            TsmiShowLyrics.Name = "TsmiShowLyrics";
            TsmiShowLyrics.Click += TsmiShowLyrics_Click;
            // 
            // TsmiShowSien
            // 
            resources.ApplyResources(TsmiShowSien, "TsmiShowSien");
            TsmiShowSien.Name = "TsmiShowSien";
            TsmiShowSien.Click += TsmiShowSien_Click;
            // 
            // TsmiShowMixer
            // 
            resources.ApplyResources(TsmiShowMixer, "TsmiShowMixer");
            TsmiShowMixer.Name = "TsmiShowMixer";
            TsmiShowMixer.Click += TsmiShowMixer_Click;
            // 
            // TsmiShowMIDIKbd
            // 
            resources.ApplyResources(TsmiShowMIDIKbd, "TsmiShowMIDIKbd");
            TsmiShowMIDIKbd.Name = "TsmiShowMIDIKbd";
            TsmiShowMIDIKbd.Click += TsmiShowMIDIKbd_Click;
            // 
            // toolStripSeparator3
            // 
            resources.ApplyResources(toolStripSeparator3, "toolStripSeparator3");
            toolStripSeparator3.Name = "toolStripSeparator3";
            // 
            // TsmiFunctionKey
            // 
            resources.ApplyResources(TsmiFunctionKey, "TsmiFunctionKey");
            TsmiFunctionKey.DropDownItems.AddRange(new ToolStripItem[] { TsmiFncHide, TsmiFncButtonOnly, TsmiFncButtonAndText });
            TsmiFunctionKey.Name = "TsmiFunctionKey";
            // 
            // TsmiFncHide
            // 
            resources.ApplyResources(TsmiFncHide, "TsmiFncHide");
            TsmiFncHide.Name = "TsmiFncHide";
            TsmiFncHide.Click += TsmiFncHide_Click;
            // 
            // TsmiFncButtonOnly
            // 
            resources.ApplyResources(TsmiFncButtonOnly, "TsmiFncButtonOnly");
            TsmiFncButtonOnly.Name = "TsmiFncButtonOnly";
            TsmiFncButtonOnly.Click += TsmiFncButtonOnly_Click;
            // 
            // TsmiFncButtonAndText
            // 
            resources.ApplyResources(TsmiFncButtonAndText, "TsmiFncButtonAndText");
            TsmiFncButtonAndText.Name = "TsmiFncButtonAndText";
            TsmiFncButtonAndText.Click += TsmiFncButtonAndText_Click;
            // 
            // コンパイルCToolStripMenuItem
            // 
            resources.ApplyResources(コンパイルCToolStripMenuItem, "コンパイルCToolStripMenuItem");
            コンパイルCToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { TsmiCompileAndPlay, TsmiCompileAndTracePlay, TsmiCompileAndSkipPlay, TsmiAllCompile, TsmiCompile });
            コンパイルCToolStripMenuItem.Name = "コンパイルCToolStripMenuItem";
            // 
            // TsmiCompileAndPlay
            // 
            resources.ApplyResources(TsmiCompileAndPlay, "TsmiCompileAndPlay");
            TsmiCompileAndPlay.Name = "TsmiCompileAndPlay";
            TsmiCompileAndPlay.Click += TsmiCompileAndPlay_Click;
            // 
            // TsmiCompileAndTracePlay
            // 
            resources.ApplyResources(TsmiCompileAndTracePlay, "TsmiCompileAndTracePlay");
            TsmiCompileAndTracePlay.Name = "TsmiCompileAndTracePlay";
            TsmiCompileAndTracePlay.Click += TsmiCompileAndTracePlay_Click;
            // 
            // TsmiCompileAndSkipPlay
            // 
            resources.ApplyResources(TsmiCompileAndSkipPlay, "TsmiCompileAndSkipPlay");
            TsmiCompileAndSkipPlay.Name = "TsmiCompileAndSkipPlay";
            TsmiCompileAndSkipPlay.Click += TsmiCompileAndSkipPlay_Click;
            // 
            // TsmiAllCompile
            // 
            resources.ApplyResources(TsmiAllCompile, "TsmiAllCompile");
            TsmiAllCompile.Name = "TsmiAllCompile";
            // 
            // TsmiCompile
            // 
            resources.ApplyResources(TsmiCompile, "TsmiCompile");
            TsmiCompile.Name = "TsmiCompile";
            TsmiCompile.Click += TsmiCompile_Click;
            // 
            // featureToolStripMenuItem
            // 
            resources.ApplyResources(featureToolStripMenuItem, "featureToolStripMenuItem");
            featureToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { tsmiJumpSoloMode });
            featureToolStripMenuItem.Name = "featureToolStripMenuItem";
            // 
            // tsmiJumpSoloMode
            // 
            resources.ApplyResources(tsmiJumpSoloMode, "tsmiJumpSoloMode");
            tsmiJumpSoloMode.CheckOnClick = true;
            tsmiJumpSoloMode.Name = "tsmiJumpSoloMode";
            // 
            // tsmiScript
            // 
            resources.ApplyResources(tsmiScript, "tsmiScript");
            tsmiScript.Name = "tsmiScript";
            // 
            // ツールTToolStripMenuItem
            // 
            resources.ApplyResources(ツールTToolStripMenuItem, "ツールTToolStripMenuItem");
            ツールTToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { tsmiMakeCSM, tsmiOption });
            ツールTToolStripMenuItem.Name = "ツールTToolStripMenuItem";
            // 
            // tsmiMakeCSM
            // 
            resources.ApplyResources(tsmiMakeCSM, "tsmiMakeCSM");
            tsmiMakeCSM.Name = "tsmiMakeCSM";
            tsmiMakeCSM.Click += tsmiMakeCSM_Click;
            // 
            // tsmiOption
            // 
            resources.ApplyResources(tsmiOption, "tsmiOption");
            tsmiOption.Name = "tsmiOption";
            tsmiOption.Click += TsmiOption_Click;
            // 
            // TsmiHelp
            // 
            resources.ApplyResources(TsmiHelp, "TsmiHelp");
            TsmiHelp.DropDownItems.AddRange(new ToolStripItem[] { TsmiTutorial, TsmiReference, TsmiReferenceMucom, TsmiReferencePMD, TsmiReferenceM98, TsmiReferenceMuap, toolStripSeparator5, TsmiAbout });
            TsmiHelp.Name = "TsmiHelp";
            // 
            // TsmiTutorial
            // 
            resources.ApplyResources(TsmiTutorial, "TsmiTutorial");
            TsmiTutorial.Name = "TsmiTutorial";
            TsmiTutorial.Click += TsmiTutorial_Click;
            // 
            // TsmiReference
            // 
            resources.ApplyResources(TsmiReference, "TsmiReference");
            TsmiReference.Name = "TsmiReference";
            TsmiReference.Click += TsmiReference_Click;
            // 
            // TsmiReferenceMucom
            // 
            resources.ApplyResources(TsmiReferenceMucom, "TsmiReferenceMucom");
            TsmiReferenceMucom.Name = "TsmiReferenceMucom";
            TsmiReferenceMucom.Click += TsmiReferenceMucom_Click;
            // 
            // TsmiReferencePMD
            // 
            resources.ApplyResources(TsmiReferencePMD, "TsmiReferencePMD");
            TsmiReferencePMD.Name = "TsmiReferencePMD";
            TsmiReferencePMD.Click += TsmiReferencePMD_Click;
            // 
            // TsmiReferenceM98
            // 
            resources.ApplyResources(TsmiReferenceM98, "TsmiReferenceM98");
            TsmiReferenceM98.Name = "TsmiReferenceM98";
            TsmiReferenceM98.Click += TsmiReferenceM98_Click;
            // 
            // TsmiReferenceMuap
            // 
            resources.ApplyResources(TsmiReferenceMuap, "TsmiReferenceMuap");
            TsmiReferenceMuap.Name = "TsmiReferenceMuap";
            TsmiReferenceMuap.Click += TsmiReferenceMuap_Click;
            // 
            // toolStripSeparator5
            // 
            resources.ApplyResources(toolStripSeparator5, "toolStripSeparator5");
            toolStripSeparator5.Name = "toolStripSeparator5";
            // 
            // TsmiAbout
            // 
            resources.ApplyResources(TsmiAbout, "TsmiAbout");
            TsmiAbout.Name = "TsmiAbout";
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
        private ToolStripMenuItem tsmiNewMus;
        private ToolStripMenuItem TsmiReferenceMuap;
    }
}

