using mml2vgmIDEx64.Properties;

namespace mml2vgmIDEx64
{
    partial class FrmSetting
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmSetting));
            btnOK = new Button();
            btnCancel = new Button();
            gbWaveOut = new GroupBox();
            cmbWaveOutDevice = new ComboBox();
            rbWaveOut = new RadioButton();
            rbAsioOut = new RadioButton();
            rbWasapiOut = new RadioButton();
            gbAsioOut = new GroupBox();
            btnASIOControlPanel = new Button();
            cmbAsioDevice = new ComboBox();
            rbDirectSoundOut = new RadioButton();
            gbWasapiOut = new GroupBox();
            rbExclusive = new RadioButton();
            rbShare = new RadioButton();
            cmbWasapiDevice = new ComboBox();
            gbDirectSound = new GroupBox();
            cmbDirectSoundDevice = new ComboBox();
            tcSetting = new TabControl();
            tpOutput = new TabPage();
            cbUseRealChip = new CheckBox();
            tabControl1 = new TabControl();
            tabPage11 = new TabPage();
            label55 = new Label();
            label50 = new Label();
            cmbSampleRate = new ComboBox();
            rbSPPCM = new RadioButton();
            rbNullDevice = new RadioButton();
            groupBox3 = new GroupBox();
            label13 = new Label();
            label12 = new Label();
            label11 = new Label();
            tbLatencyEmu = new TextBox();
            tbLatencySCCI = new TextBox();
            label10 = new Label();
            lblWaitTime = new Label();
            lblLatencyUnit = new Label();
            groupBox16 = new GroupBox();
            cmbSPPCMDevice = new ComboBox();
            label28 = new Label();
            lblLatency = new Label();
            cmbWaitTime = new ComboBox();
            cmbLatency = new ComboBox();
            tabPage12 = new TabPage();
            groupBox27 = new GroupBox();
            rbManualDetect = new RadioButton();
            rbAutoDetect = new RadioButton();
            groupBox28 = new GroupBox();
            rbSCCIDetect = new RadioButton();
            rbC86ctlDetect = new RadioButton();
            groupBox1 = new GroupBox();
            ucSI = new ucSettingInstruments();
            tabPage13 = new TabPage();
            splitContainer1 = new SplitContainer();
            label16 = new Label();
            dgvMIDIoutPallet = new DataGridView();
            clmID = new DataGridViewTextBoxColumn();
            clmDeviceName = new DataGridViewTextBoxColumn();
            clmManufacturer = new DataGridViewTextBoxColumn();
            clmSpacer = new DataGridViewTextBoxColumn();
            tbcMIDIoutList = new TabControl();
            tabPage1 = new TabPage();
            dgvMIDIoutListA = new DataGridView();
            dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
            clmIsVST = new DataGridViewCheckBoxColumn();
            clmFileName = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
            clmType = new DataGridViewComboBoxColumn();
            ClmBeforeSend = new DataGridViewComboBoxColumn();
            dataGridViewTextBoxColumn3 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn4 = new DataGridViewTextBoxColumn();
            btnUP_A = new Button();
            btnDOWN_A = new Button();
            tabPage2 = new TabPage();
            dgvMIDIoutListB = new DataGridView();
            btnUP_B = new Button();
            btnDOWN_B = new Button();
            tabPage3 = new TabPage();
            dgvMIDIoutListC = new DataGridView();
            btnUP_C = new Button();
            btnDOWN_C = new Button();
            tabPage4 = new TabPage();
            dgvMIDIoutListD = new DataGridView();
            btnUP_D = new Button();
            btnDOWN_D = new Button();
            btnAddVST = new Button();
            label18 = new Label();
            btnAddMIDIout = new Button();
            btnSubMIDIout = new Button();
            tpNuked = new TabPage();
            tabControl2 = new TabControl();
            tabPage14 = new TabPage();
            groupBox26 = new GroupBox();
            rbNukedOPN2OptionYM2612u = new RadioButton();
            rbNukedOPN2OptionYM2612 = new RadioButton();
            rbNukedOPN2OptionDiscrete = new RadioButton();
            rbNukedOPN2OptionASIClp = new RadioButton();
            rbNukedOPN2OptionASIC = new RadioButton();
            tabPage5 = new TabPage();
            groupBox34 = new GroupBox();
            cbGensSSGEG = new CheckBox();
            cbGensDACHPF = new CheckBox();
            tpNSF = new TabPage();
            tabControl3 = new TabControl();
            tabPage15 = new TabPage();
            groupBox9 = new GroupBox();
            cbNFSNes_DutySwap = new CheckBox();
            cbNFSNes_PhaseRefresh = new CheckBox();
            cbNFSNes_NonLinearMixer = new CheckBox();
            cbNFSNes_UnmuteOnReset = new CheckBox();
            groupBox10 = new GroupBox();
            cbNSFDmc_TriNull = new CheckBox();
            cbNSFDmc_TriMute = new CheckBox();
            cbNSFDmc_RandomizeNoise = new CheckBox();
            cbNSFDmc_DPCMAntiClick = new CheckBox();
            cbNSFDmc_EnablePNoise = new CheckBox();
            cbNSFDmc_Enable4011 = new CheckBox();
            cbNSFDmc_NonLinearMixer = new CheckBox();
            cbNSFDmc_UnmuteOnReset = new CheckBox();
            groupBox8 = new GroupBox();
            label21 = new Label();
            label20 = new Label();
            tbNSFFds_LPF = new TextBox();
            cbNFSFds_4085Reset = new CheckBox();
            cbNSFFDSWriteDisable8000 = new CheckBox();
            groupBox12 = new GroupBox();
            cbNSFN160_Serial = new CheckBox();
            groupBox11 = new GroupBox();
            cbNSFMmc5_PhaseRefresh = new CheckBox();
            cbNSFMmc5_NonLinearMixer = new CheckBox();
            tabPage16 = new TabPage();
            groupBox14 = new GroupBox();
            label27 = new Label();
            label26 = new Label();
            label25 = new Label();
            rdSIDQ1 = new RadioButton();
            rdSIDQ3 = new RadioButton();
            rdSIDQ2 = new RadioButton();
            rdSIDQ4 = new RadioButton();
            groupBox13 = new GroupBox();
            btnSIDBasic = new Button();
            btnSIDCharacter = new Button();
            btnSIDKernal = new Button();
            tbSIDCharacter = new TextBox();
            tbSIDBasic = new TextBox();
            tbSIDKernal = new TextBox();
            label24 = new Label();
            label23 = new Label();
            label22 = new Label();
            label49 = new Label();
            label51 = new Label();
            tbSIDOutputBufferSize = new TextBox();
            tabPage17 = new TabPage();
            groupBox15 = new GroupBox();
            btnBeforeSend_Default = new Button();
            tbBeforeSend_Custom = new TextBox();
            tbBeforeSend_XGReset = new TextBox();
            label35 = new Label();
            label34 = new Label();
            label32 = new Label();
            tbBeforeSend_GSReset = new TextBox();
            label33 = new Label();
            tbBeforeSend_GMReset = new TextBox();
            label31 = new Label();
            tpPMDDotNET = new TabPage();
            rbPMDManual = new RadioButton();
            rbPMDAuto = new RadioButton();
            btnPMDResetDriverArguments = new Button();
            label47 = new Label();
            btnPMDResetCompilerArhguments = new Button();
            tbPMDDriverArguments = new TextBox();
            label37 = new Label();
            tbPMDCompilerArguments = new TextBox();
            gbPMDManual = new GroupBox();
            cbPMDSetManualVolume = new CheckBox();
            cbPMDUsePPZ8 = new CheckBox();
            groupBox32 = new GroupBox();
            rbPMD86B = new RadioButton();
            rbPMDSpbB = new RadioButton();
            rbPMDNrmB = new RadioButton();
            cbPMDUsePPSDRV = new CheckBox();
            gbPPSDRV = new GroupBox();
            groupBox33 = new GroupBox();
            rbPMDUsePPSDRVManualFreq = new RadioButton();
            label38 = new Label();
            rbPMDUsePPSDRVFreqDefault = new RadioButton();
            btnPMDPPSDRVManualWait = new Button();
            label40 = new Label();
            tbPMDPPSDRVFreq = new TextBox();
            label39 = new Label();
            tbPMDPPSDRVManualWait = new TextBox();
            gbPMDSetManualVolume = new GroupBox();
            label41 = new Label();
            label46 = new Label();
            tbPMDVolumeAdpcm = new TextBox();
            label42 = new Label();
            tbPMDVolumeRhythm = new TextBox();
            label43 = new Label();
            tbPMDVolumeSSG = new TextBox();
            label44 = new Label();
            tbPMDVolumeGIMICSSG = new TextBox();
            label45 = new Label();
            tbPMDVolumeFM = new TextBox();
            tpExport = new TabPage();
            cbAlwaysAskForLoopCount = new CheckBox();
            cbFixedExportPlace = new CheckBox();
            gpbFixedExportPlace = new GroupBox();
            btnFixedExportPlace = new Button();
            label57 = new Label();
            tbFixedExportPlacePath = new TextBox();
            label48 = new Label();
            tbLoopTimes = new TextBox();
            lblLoopTimes = new Label();
            tpMIDIExp = new TabPage();
            cbUseMIDIExport = new CheckBox();
            gbMIDIExport = new GroupBox();
            cbMIDIKeyOnFnum = new CheckBox();
            cbMIDIUseVOPM = new CheckBox();
            groupBox6 = new GroupBox();
            cbMIDIYM2612 = new CheckBox();
            cbMIDISN76489Sec = new CheckBox();
            cbMIDIYM2612Sec = new CheckBox();
            cbMIDISN76489 = new CheckBox();
            cbMIDIYM2151 = new CheckBox();
            cbMIDIYM2610BSec = new CheckBox();
            cbMIDIYM2151Sec = new CheckBox();
            cbMIDIYM2610B = new CheckBox();
            cbMIDIYM2203 = new CheckBox();
            cbMIDIYM2608Sec = new CheckBox();
            cbMIDIYM2203Sec = new CheckBox();
            cbMIDIYM2608 = new CheckBox();
            cbMIDIPlayless = new CheckBox();
            btnMIDIOutputPath = new Button();
            lblOutputPath = new Label();
            tbMIDIOutputPath = new TextBox();
            tpMIDIKBD = new TabPage();
            cbMIDIKbdAlwaysTop = new CheckBox();
            cbUseMIDIKeyboard = new CheckBox();
            gbMIDIKeyboard = new GroupBox();
            pictureBox8 = new PictureBox();
            pictureBox7 = new PictureBox();
            pictureBox6 = new PictureBox();
            pictureBox5 = new PictureBox();
            pictureBox4 = new PictureBox();
            pictureBox3 = new PictureBox();
            pictureBox2 = new PictureBox();
            pictureBox1 = new PictureBox();
            tbCCFadeout = new TextBox();
            tbCCPause = new TextBox();
            tbCCSlow = new TextBox();
            tbCCPrevious = new TextBox();
            tbCCNext = new TextBox();
            tbCCFast = new TextBox();
            tbCCStop = new TextBox();
            tbCCPlay = new TextBox();
            tbCCCopyLog = new TextBox();
            label17 = new Label();
            tbCCDelLog = new TextBox();
            label15 = new Label();
            tbCCChCopy = new TextBox();
            label8 = new Label();
            label9 = new Label();
            gbUseChannel = new GroupBox();
            rbMONO = new RadioButton();
            rbPOLY = new RadioButton();
            groupBox7 = new GroupBox();
            rbFM6 = new RadioButton();
            rbFM3 = new RadioButton();
            rbFM5 = new RadioButton();
            rbFM2 = new RadioButton();
            rbFM4 = new RadioButton();
            rbFM1 = new RadioButton();
            groupBox2 = new GroupBox();
            cbFM1 = new CheckBox();
            cbFM6 = new CheckBox();
            cbFM2 = new CheckBox();
            cbFM5 = new CheckBox();
            cbFM3 = new CheckBox();
            cbFM4 = new CheckBox();
            cmbMIDIIN = new ComboBox();
            label5 = new Label();
            tpKeyBoard = new TabPage();
            btnInitializeShortCutKey = new Button();
            lblSKKey = new Label();
            dgvShortCutKey = new DataGridView();
            clmNumber = new DataGridViewTextBoxColumn();
            clmFunc = new DataGridViewTextBoxColumn();
            clmShift = new DataGridViewCheckBoxColumn();
            clmCtrl = new DataGridViewCheckBoxColumn();
            clmAlt = new DataGridViewCheckBoxColumn();
            clmKey = new DataGridViewTextBoxColumn();
            clmSet = new DataGridViewButtonColumn();
            clmClr = new DataGridViewButtonColumn();
            clmKBDSpacer = new DataGridViewTextBoxColumn();
            tpBalance = new TabPage();
            groupBox25 = new GroupBox();
            rbAutoBalanceNotSamePositionAsSongData = new RadioButton();
            rbAutoBalanceSamePositionAsSongData = new RadioButton();
            cbAutoBalanceUseThis = new CheckBox();
            groupBox18 = new GroupBox();
            groupBox24 = new GroupBox();
            groupBox21 = new GroupBox();
            rbAutoBalanceNotSaveSongBalance = new RadioButton();
            rbAutoBalanceSaveSongBalance = new RadioButton();
            groupBox22 = new GroupBox();
            label4 = new Label();
            groupBox23 = new GroupBox();
            groupBox19 = new GroupBox();
            rbAutoBalanceNotLoadSongBalance = new RadioButton();
            rbAutoBalanceLoadSongBalance = new RadioButton();
            groupBox20 = new GroupBox();
            rbAutoBalanceNotLoadDriverBalance = new RadioButton();
            rbAutoBalanceLoadDriverBalance = new RadioButton();
            tpMMLParameter = new TabPage();
            cbDispInstrumentName = new CheckBox();
            tpOther2 = new TabPage();
            tbTABWidth = new TextBox();
            cbDispWarningMessage = new CheckBox();
            tbUseHistoryBackUp = new TextBox();
            cbUseHistoryBackUp = new CheckBox();
            cbUseMoonDriverDotNET = new CheckBox();
            cbUsePMDDotNET = new CheckBox();
            cbUseMucomDotNET = new CheckBox();
            cbUseScript = new CheckBox();
            cbChangeEnterCode = new CheckBox();
            cbClearHistory = new CheckBox();
            tbOpacity = new TrackBar();
            label56 = new Label();
            label52 = new Label();
            groupBox29 = new GroupBox();
            label36 = new Label();
            btFont = new Button();
            label54 = new Label();
            lblFontName = new Label();
            label53 = new Label();
            lblFontSize = new Label();
            lblFontStyle = new Label();
            cbInfiniteOfflineMode = new CheckBox();
            cbUseSIen = new CheckBox();
            cbRequestCacheClear = new CheckBox();
            tpOther = new TabPage();
            cbWavSwitch = new CheckBox();
            groupBox17 = new GroupBox();
            tbImageExt = new TextBox();
            tbMMLExt = new TextBox();
            tbTextExt = new TextBox();
            label1 = new Label();
            label3 = new Label();
            label2 = new Label();
            cbUseGetInst = new CheckBox();
            groupBox4 = new GroupBox();
            cmbInstFormat = new ComboBox();
            lblInstFormat = new Label();
            cbDumpSwitch = new CheckBox();
            gbWav = new GroupBox();
            btnWavPath = new Button();
            label7 = new Label();
            tbWavPath = new TextBox();
            gbDump = new GroupBox();
            btnDumpPath = new Button();
            label6 = new Label();
            tbDumpPath = new TextBox();
            label30 = new Label();
            tbScreenFrameRate = new TextBox();
            label29 = new Label();
            btnDataPath = new Button();
            tbDataPath = new TextBox();
            label19 = new Label();
            btnResetPosition = new Button();
            btnOpenSettingFolder = new Button();
            cbEmptyPlayList = new CheckBox();
            cbInitAlways = new CheckBox();
            cbAutoOpen = new CheckBox();
            cbUseLoopTimes = new CheckBox();
            tpOmake = new TabPage();
            groupBox31 = new GroupBox();
            rbLoglevelINFO = new RadioButton();
            rbLoglevelDEBUG = new RadioButton();
            rbLoglevelTRACE = new RadioButton();
            groupBox30 = new GroupBox();
            rbQueryPerformanceCounter = new RadioButton();
            rbDateTime = new RadioButton();
            rbStopWatch = new RadioButton();
            label14 = new Label();
            btVST = new Button();
            tbVST = new TextBox();
            groupBox5 = new GroupBox();
            cbSinWave = new CheckBox();
            cbPlayDeviceCB = new CheckBox();
            cbLogWarning = new CheckBox();
            cbDispFrameCounter = new CheckBox();
            tpAbout = new TabPage();
            tableLayoutPanel = new TableLayoutPanel();
            logoPictureBox = new PictureBox();
            labelProductName = new Label();
            labelVersion = new Label();
            labelCopyright = new Label();
            labelCompanyName = new Label();
            textBoxDescription = new TextBox();
            llOpenGithub = new LinkLabel();
            cbHiyorimiMode = new CheckBox();
            cbHilightOn = new CheckBox();
            gbWaveOut.SuspendLayout();
            gbAsioOut.SuspendLayout();
            gbWasapiOut.SuspendLayout();
            gbDirectSound.SuspendLayout();
            tcSetting.SuspendLayout();
            tpOutput.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPage11.SuspendLayout();
            groupBox3.SuspendLayout();
            groupBox16.SuspendLayout();
            tabPage12.SuspendLayout();
            groupBox27.SuspendLayout();
            groupBox28.SuspendLayout();
            groupBox1.SuspendLayout();
            tabPage13.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvMIDIoutPallet).BeginInit();
            tbcMIDIoutList.SuspendLayout();
            tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvMIDIoutListA).BeginInit();
            tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvMIDIoutListB).BeginInit();
            tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvMIDIoutListC).BeginInit();
            tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvMIDIoutListD).BeginInit();
            tpNuked.SuspendLayout();
            tabControl2.SuspendLayout();
            tabPage14.SuspendLayout();
            groupBox26.SuspendLayout();
            tabPage5.SuspendLayout();
            groupBox34.SuspendLayout();
            tpNSF.SuspendLayout();
            tabControl3.SuspendLayout();
            tabPage15.SuspendLayout();
            groupBox9.SuspendLayout();
            groupBox10.SuspendLayout();
            groupBox8.SuspendLayout();
            groupBox12.SuspendLayout();
            groupBox11.SuspendLayout();
            tabPage16.SuspendLayout();
            groupBox14.SuspendLayout();
            groupBox13.SuspendLayout();
            tabPage17.SuspendLayout();
            groupBox15.SuspendLayout();
            tpPMDDotNET.SuspendLayout();
            gbPMDManual.SuspendLayout();
            groupBox32.SuspendLayout();
            gbPPSDRV.SuspendLayout();
            groupBox33.SuspendLayout();
            gbPMDSetManualVolume.SuspendLayout();
            tpExport.SuspendLayout();
            gpbFixedExportPlace.SuspendLayout();
            tpMIDIExp.SuspendLayout();
            gbMIDIExport.SuspendLayout();
            groupBox6.SuspendLayout();
            tpMIDIKBD.SuspendLayout();
            gbMIDIKeyboard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox8).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox7).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox6).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            gbUseChannel.SuspendLayout();
            groupBox7.SuspendLayout();
            groupBox2.SuspendLayout();
            tpKeyBoard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvShortCutKey).BeginInit();
            tpBalance.SuspendLayout();
            groupBox25.SuspendLayout();
            groupBox18.SuspendLayout();
            groupBox24.SuspendLayout();
            groupBox21.SuspendLayout();
            groupBox22.SuspendLayout();
            groupBox23.SuspendLayout();
            groupBox19.SuspendLayout();
            groupBox20.SuspendLayout();
            tpMMLParameter.SuspendLayout();
            tpOther2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)tbOpacity).BeginInit();
            groupBox29.SuspendLayout();
            tpOther.SuspendLayout();
            groupBox17.SuspendLayout();
            groupBox4.SuspendLayout();
            gbWav.SuspendLayout();
            gbDump.SuspendLayout();
            tpOmake.SuspendLayout();
            groupBox31.SuspendLayout();
            groupBox30.SuspendLayout();
            groupBox5.SuspendLayout();
            tpAbout.SuspendLayout();
            tableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)logoPictureBox).BeginInit();
            SuspendLayout();
            // 
            // btnOK
            // 
            btnOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnOK.Location = new Point(345, 569);
            btnOK.Margin = new Padding(4);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(88, 29);
            btnOK.TabIndex = 0;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(440, 569);
            btnCancel.Margin = new Padding(4);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(88, 29);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "キャンセル";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // gbWaveOut
            // 
            gbWaveOut.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            gbWaveOut.Controls.Add(cmbWaveOutDevice);
            gbWaveOut.Location = new Point(7, 12);
            gbWaveOut.Margin = new Padding(4);
            gbWaveOut.Name = "gbWaveOut";
            gbWaveOut.Padding = new Padding(4);
            gbWaveOut.Size = new Size(373, 60);
            gbWaveOut.TabIndex = 1;
            gbWaveOut.TabStop = false;
            // 
            // cmbWaveOutDevice
            // 
            cmbWaveOutDevice.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cmbWaveOutDevice.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbWaveOutDevice.FormattingEnabled = true;
            cmbWaveOutDevice.Location = new Point(7, 22);
            cmbWaveOutDevice.Margin = new Padding(4);
            cmbWaveOutDevice.Name = "cmbWaveOutDevice";
            cmbWaveOutDevice.Size = new Size(359, 23);
            cmbWaveOutDevice.TabIndex = 0;
            // 
            // rbWaveOut
            // 
            rbWaveOut.AutoSize = true;
            rbWaveOut.Checked = true;
            rbWaveOut.Location = new Point(14, 8);
            rbWaveOut.Margin = new Padding(4);
            rbWaveOut.Name = "rbWaveOut";
            rbWaveOut.Size = new Size(74, 19);
            rbWaveOut.TabIndex = 0;
            rbWaveOut.TabStop = true;
            rbWaveOut.Text = "WaveOut";
            rbWaveOut.UseVisualStyleBackColor = true;
            rbWaveOut.CheckedChanged += rbWaveOut_CheckedChanged;
            // 
            // rbAsioOut
            // 
            rbAsioOut.AutoSize = true;
            rbAsioOut.Location = new Point(14, 218);
            rbAsioOut.Margin = new Padding(4);
            rbAsioOut.Name = "rbAsioOut";
            rbAsioOut.Size = new Size(68, 19);
            rbAsioOut.TabIndex = 6;
            rbAsioOut.Text = "AsioOut";
            rbAsioOut.UseVisualStyleBackColor = true;
            rbAsioOut.CheckedChanged += rbAsioOut_CheckedChanged;
            // 
            // rbWasapiOut
            // 
            rbWasapiOut.AutoSize = true;
            rbWasapiOut.Location = new Point(14, 146);
            rbWasapiOut.Margin = new Padding(4);
            rbWasapiOut.Name = "rbWasapiOut";
            rbWasapiOut.Size = new Size(83, 19);
            rbWasapiOut.TabIndex = 4;
            rbWasapiOut.Text = "WasapiOut";
            rbWasapiOut.UseVisualStyleBackColor = true;
            rbWasapiOut.CheckedChanged += rbWasapiOut_CheckedChanged;
            // 
            // gbAsioOut
            // 
            gbAsioOut.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            gbAsioOut.Controls.Add(btnASIOControlPanel);
            gbAsioOut.Controls.Add(cmbAsioDevice);
            gbAsioOut.Location = new Point(7, 221);
            gbAsioOut.Margin = new Padding(4);
            gbAsioOut.Name = "gbAsioOut";
            gbAsioOut.Padding = new Padding(4);
            gbAsioOut.Size = new Size(483, 62);
            gbAsioOut.TabIndex = 7;
            gbAsioOut.TabStop = false;
            // 
            // btnASIOControlPanel
            // 
            btnASIOControlPanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnASIOControlPanel.Location = new Point(383, 10);
            btnASIOControlPanel.Margin = new Padding(4);
            btnASIOControlPanel.Name = "btnASIOControlPanel";
            btnASIOControlPanel.Size = new Size(94, 49);
            btnASIOControlPanel.TabIndex = 8;
            btnASIOControlPanel.Text = "ASIO Control Panel";
            btnASIOControlPanel.UseVisualStyleBackColor = true;
            btnASIOControlPanel.Click += btnASIOControlPanel_Click;
            // 
            // cmbAsioDevice
            // 
            cmbAsioDevice.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cmbAsioDevice.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbAsioDevice.FormattingEnabled = true;
            cmbAsioDevice.Location = new Point(7, 22);
            cmbAsioDevice.Margin = new Padding(4);
            cmbAsioDevice.Name = "cmbAsioDevice";
            cmbAsioDevice.Size = new Size(367, 23);
            cmbAsioDevice.TabIndex = 6;
            // 
            // rbDirectSoundOut
            // 
            rbDirectSoundOut.AutoSize = true;
            rbDirectSoundOut.Location = new Point(14, 78);
            rbDirectSoundOut.Margin = new Padding(4);
            rbDirectSoundOut.Name = "rbDirectSoundOut";
            rbDirectSoundOut.Size = new Size(90, 19);
            rbDirectSoundOut.TabIndex = 2;
            rbDirectSoundOut.Text = "DirectSound";
            rbDirectSoundOut.UseVisualStyleBackColor = true;
            rbDirectSoundOut.CheckedChanged += rbDirectSoundOut_CheckedChanged;
            // 
            // gbWasapiOut
            // 
            gbWasapiOut.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            gbWasapiOut.Controls.Add(rbExclusive);
            gbWasapiOut.Controls.Add(rbShare);
            gbWasapiOut.Controls.Add(cmbWasapiDevice);
            gbWasapiOut.Location = new Point(7, 151);
            gbWasapiOut.Margin = new Padding(4);
            gbWasapiOut.Name = "gbWasapiOut";
            gbWasapiOut.Padding = new Padding(4);
            gbWasapiOut.Size = new Size(483, 62);
            gbWasapiOut.TabIndex = 5;
            gbWasapiOut.TabStop = false;
            // 
            // rbExclusive
            // 
            rbExclusive.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            rbExclusive.AutoSize = true;
            rbExclusive.Location = new Point(388, 39);
            rbExclusive.Margin = new Padding(4);
            rbExclusive.Name = "rbExclusive";
            rbExclusive.Size = new Size(49, 19);
            rbExclusive.TabIndex = 7;
            rbExclusive.TabStop = true;
            rbExclusive.Text = "排他";
            rbExclusive.UseVisualStyleBackColor = true;
            // 
            // rbShare
            // 
            rbShare.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            rbShare.AutoSize = true;
            rbShare.Location = new Point(388, 11);
            rbShare.Margin = new Padding(4);
            rbShare.Name = "rbShare";
            rbShare.Size = new Size(49, 19);
            rbShare.TabIndex = 6;
            rbShare.TabStop = true;
            rbShare.Text = "共有";
            rbShare.UseVisualStyleBackColor = true;
            // 
            // cmbWasapiDevice
            // 
            cmbWasapiDevice.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cmbWasapiDevice.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbWasapiDevice.FormattingEnabled = true;
            cmbWasapiDevice.Location = new Point(7, 22);
            cmbWasapiDevice.Margin = new Padding(4);
            cmbWasapiDevice.Name = "cmbWasapiDevice";
            cmbWasapiDevice.Size = new Size(367, 23);
            cmbWasapiDevice.TabIndex = 4;
            // 
            // gbDirectSound
            // 
            gbDirectSound.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            gbDirectSound.Controls.Add(cmbDirectSoundDevice);
            gbDirectSound.Location = new Point(7, 82);
            gbDirectSound.Margin = new Padding(4);
            gbDirectSound.Name = "gbDirectSound";
            gbDirectSound.Padding = new Padding(4);
            gbDirectSound.Size = new Size(484, 60);
            gbDirectSound.TabIndex = 3;
            gbDirectSound.TabStop = false;
            // 
            // cmbDirectSoundDevice
            // 
            cmbDirectSoundDevice.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cmbDirectSoundDevice.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDirectSoundDevice.FormattingEnabled = true;
            cmbDirectSoundDevice.Location = new Point(7, 22);
            cmbDirectSoundDevice.Margin = new Padding(4);
            cmbDirectSoundDevice.Name = "cmbDirectSoundDevice";
            cmbDirectSoundDevice.Size = new Size(471, 23);
            cmbDirectSoundDevice.TabIndex = 2;
            // 
            // tcSetting
            // 
            tcSetting.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tcSetting.Controls.Add(tpOutput);
            tcSetting.Controls.Add(tpNuked);
            tcSetting.Controls.Add(tpNSF);
            tcSetting.Controls.Add(tpPMDDotNET);
            tcSetting.Controls.Add(tpExport);
            tcSetting.Controls.Add(tpMIDIExp);
            tcSetting.Controls.Add(tpMIDIKBD);
            tcSetting.Controls.Add(tpKeyBoard);
            tcSetting.Controls.Add(tpBalance);
            tcSetting.Controls.Add(tpMMLParameter);
            tcSetting.Controls.Add(tpOther2);
            tcSetting.Controls.Add(tpOther);
            tcSetting.Controls.Add(tpOmake);
            tcSetting.Controls.Add(tpAbout);
            tcSetting.HotTrack = true;
            tcSetting.Location = new Point(1, 4);
            tcSetting.Margin = new Padding(4);
            tcSetting.Multiline = true;
            tcSetting.Name = "tcSetting";
            tcSetting.SelectedIndex = 0;
            tcSetting.Size = new Size(526, 558);
            tcSetting.TabIndex = 2;
            // 
            // tpOutput
            // 
            tpOutput.BorderStyle = BorderStyle.FixedSingle;
            tpOutput.Controls.Add(cbUseRealChip);
            tpOutput.Controls.Add(tabControl1);
            tpOutput.Location = new Point(4, 44);
            tpOutput.Margin = new Padding(4);
            tpOutput.Name = "tpOutput";
            tpOutput.Padding = new Padding(4);
            tpOutput.Size = new Size(518, 510);
            tpOutput.TabIndex = 0;
            tpOutput.Text = "出力";
            tpOutput.UseVisualStyleBackColor = true;
            // 
            // cbUseRealChip
            // 
            cbUseRealChip.AutoSize = true;
            cbUseRealChip.Location = new Point(7, 8);
            cbUseRealChip.Margin = new Padding(4);
            cbUseRealChip.Name = "cbUseRealChip";
            cbUseRealChip.Size = new Size(245, 19);
            cbUseRealChip.TabIndex = 8;
            cbUseRealChip.Text = "C86ctl/SCCIを使用しない(再起動が必要です)";
            cbUseRealChip.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            tabControl1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControl1.Controls.Add(tabPage11);
            tabControl1.Controls.Add(tabPage12);
            tabControl1.Controls.Add(tabPage13);
            tabControl1.HotTrack = true;
            tabControl1.Location = new Point(4, 38);
            tabControl1.Margin = new Padding(4);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(507, 479);
            tabControl1.TabIndex = 7;
            // 
            // tabPage11
            // 
            tabPage11.Controls.Add(label55);
            tabPage11.Controls.Add(label50);
            tabPage11.Controls.Add(cmbSampleRate);
            tabPage11.Controls.Add(rbWasapiOut);
            tabPage11.Controls.Add(rbSPPCM);
            tabPage11.Controls.Add(rbDirectSoundOut);
            tabPage11.Controls.Add(rbAsioOut);
            tabPage11.Controls.Add(rbWaveOut);
            tabPage11.Controls.Add(rbNullDevice);
            tabPage11.Controls.Add(groupBox3);
            tabPage11.Controls.Add(gbWasapiOut);
            tabPage11.Controls.Add(gbDirectSound);
            tabPage11.Controls.Add(lblWaitTime);
            tabPage11.Controls.Add(gbAsioOut);
            tabPage11.Controls.Add(lblLatencyUnit);
            tabPage11.Controls.Add(groupBox16);
            tabPage11.Controls.Add(label28);
            tabPage11.Controls.Add(lblLatency);
            tabPage11.Controls.Add(cmbWaitTime);
            tabPage11.Controls.Add(cmbLatency);
            tabPage11.Controls.Add(gbWaveOut);
            tabPage11.Location = new Point(4, 24);
            tabPage11.Margin = new Padding(4);
            tabPage11.Name = "tabPage11";
            tabPage11.Padding = new Padding(4);
            tabPage11.Size = new Size(499, 451);
            tabPage11.TabIndex = 0;
            tabPage11.Text = "Emulation";
            tabPage11.UseVisualStyleBackColor = true;
            // 
            // label55
            // 
            label55.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label55.AutoSize = true;
            label55.Location = new Point(464, 62);
            label55.Margin = new Padding(4, 0, 4, 0);
            label55.Name = "label55";
            label55.Size = new Size(21, 15);
            label55.TabIndex = 12;
            label55.Text = "Hz";
            // 
            // label50
            // 
            label50.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label50.AutoSize = true;
            label50.Location = new Point(390, 12);
            label50.Margin = new Padding(4, 0, 4, 0);
            label50.Name = "label50";
            label50.Size = new Size(68, 15);
            label50.TabIndex = 11;
            label50.Text = "SampleRate";
            // 
            // cmbSampleRate
            // 
            cmbSampleRate.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cmbSampleRate.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSampleRate.FormattingEnabled = true;
            cmbSampleRate.Items.AddRange(new object[] { "44100", "48000", "96000", "192000" });
            cmbSampleRate.Location = new Point(387, 32);
            cmbSampleRate.Margin = new Padding(4);
            cmbSampleRate.Name = "cmbSampleRate";
            cmbSampleRate.Size = new Size(100, 23);
            cmbSampleRate.TabIndex = 10;
            // 
            // rbSPPCM
            // 
            rbSPPCM.AutoSize = true;
            rbSPPCM.Enabled = false;
            rbSPPCM.Location = new Point(14, 288);
            rbSPPCM.Margin = new Padding(4);
            rbSPPCM.Name = "rbSPPCM";
            rbSPPCM.Size = new Size(63, 19);
            rbSPPCM.TabIndex = 2;
            rbSPPCM.Text = "SPPCM";
            rbSPPCM.UseVisualStyleBackColor = true;
            rbSPPCM.CheckedChanged += rbDirectSoundOut_CheckedChanged;
            // 
            // rbNullDevice
            // 
            rbNullDevice.Location = new Point(262, 304);
            rbNullDevice.Margin = new Padding(4);
            rbNullDevice.Name = "rbNullDevice";
            rbNullDevice.Size = new Size(184, 36);
            rbNullDevice.TabIndex = 2;
            rbNullDevice.Text = "NULL(サウンドデバイスを使用しない)";
            rbNullDevice.UseVisualStyleBackColor = true;
            rbNullDevice.CheckedChanged += rbDirectSoundOut_CheckedChanged;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(label13);
            groupBox3.Controls.Add(label12);
            groupBox3.Controls.Add(label11);
            groupBox3.Controls.Add(tbLatencyEmu);
            groupBox3.Controls.Add(tbLatencySCCI);
            groupBox3.Controls.Add(label10);
            groupBox3.Location = new Point(301, 340);
            groupBox3.Margin = new Padding(4);
            groupBox3.Name = "groupBox3";
            groupBox3.Padding = new Padding(4);
            groupBox3.Size = new Size(184, 78);
            groupBox3.TabIndex = 6;
            groupBox3.TabStop = false;
            groupBox3.Text = "遅延演奏";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(7, 22);
            label13.Margin = new Padding(4, 0, 4, 0);
            label13.Name = "label13";
            label13.Size = new Size(32, 15);
            label13.TabIndex = 0;
            label13.Text = "EMU";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(7, 50);
            label12.Margin = new Padding(4, 0, 4, 0);
            label12.Name = "label12";
            label12.Size = new Size(29, 15);
            label12.TabIndex = 3;
            label12.Text = "Real";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(154, 50);
            label11.Margin = new Padding(4, 0, 4, 0);
            label11.Name = "label11";
            label11.Size = new Size(22, 15);
            label11.TabIndex = 5;
            label11.Text = "ms";
            // 
            // tbLatencyEmu
            // 
            tbLatencyEmu.Location = new Point(48, 19);
            tbLatencyEmu.Margin = new Padding(4);
            tbLatencyEmu.Name = "tbLatencyEmu";
            tbLatencyEmu.Size = new Size(98, 23);
            tbLatencyEmu.TabIndex = 1;
            // 
            // tbLatencySCCI
            // 
            tbLatencySCCI.Location = new Point(48, 46);
            tbLatencySCCI.Margin = new Padding(4);
            tbLatencySCCI.Name = "tbLatencySCCI";
            tbLatencySCCI.Size = new Size(98, 23);
            tbLatencySCCI.TabIndex = 4;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(154, 22);
            label10.Margin = new Padding(4, 0, 4, 0);
            label10.Name = "label10";
            label10.Size = new Size(22, 15);
            label10.TabIndex = 2;
            label10.Text = "ms";
            // 
            // lblWaitTime
            // 
            lblWaitTime.AutoSize = true;
            lblWaitTime.Location = new Point(273, 395);
            lblWaitTime.Margin = new Padding(4, 0, 4, 0);
            lblWaitTime.Name = "lblWaitTime";
            lblWaitTime.Size = new Size(22, 15);
            lblWaitTime.TabIndex = 9;
            lblWaitTime.Text = "ms";
            // 
            // lblLatencyUnit
            // 
            lblLatencyUnit.AutoSize = true;
            lblLatencyUnit.Location = new Point(273, 362);
            lblLatencyUnit.Margin = new Padding(4, 0, 4, 0);
            lblLatencyUnit.Name = "lblLatencyUnit";
            lblLatencyUnit.Size = new Size(22, 15);
            lblLatencyUnit.TabIndex = 9;
            lblLatencyUnit.Text = "ms";
            // 
            // groupBox16
            // 
            groupBox16.Controls.Add(cmbSPPCMDevice);
            groupBox16.Enabled = false;
            groupBox16.Location = new Point(7, 291);
            groupBox16.Margin = new Padding(4);
            groupBox16.Name = "groupBox16";
            groupBox16.Padding = new Padding(4);
            groupBox16.Size = new Size(248, 60);
            groupBox16.TabIndex = 3;
            groupBox16.TabStop = false;
            // 
            // cmbSPPCMDevice
            // 
            cmbSPPCMDevice.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cmbSPPCMDevice.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSPPCMDevice.FormattingEnabled = true;
            cmbSPPCMDevice.Location = new Point(7, 24);
            cmbSPPCMDevice.Margin = new Padding(4);
            cmbSPPCMDevice.Name = "cmbSPPCMDevice";
            cmbSPPCMDevice.Size = new Size(234, 23);
            cmbSPPCMDevice.TabIndex = 2;
            // 
            // label28
            // 
            label28.AutoSize = true;
            label28.Location = new Point(7, 395);
            label28.Margin = new Padding(4, 0, 4, 0);
            label28.Name = "label28";
            label28.Size = new Size(100, 15);
            label28.TabIndex = 9;
            label28.Text = "演奏開始待ち時間";
            // 
            // lblLatency
            // 
            lblLatency.AutoSize = true;
            lblLatency.Location = new Point(6, 354);
            lblLatency.Margin = new Padding(4, 0, 4, 0);
            lblLatency.Name = "lblLatency";
            lblLatency.Size = new Size(100, 30);
            lblLatency.TabIndex = 9;
            lblLatency.Text = "遅延時間\r\n(レンダリングバッファ)";
            // 
            // cmbWaitTime
            // 
            cmbWaitTime.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbWaitTime.FormattingEnabled = true;
            cmbWaitTime.Items.AddRange(new object[] { "0", "500", "1000", "1500", "2000", "2500", "3000", "3500", "4000", "4500", "5000" });
            cmbWaitTime.Location = new Point(125, 391);
            cmbWaitTime.Margin = new Padding(4);
            cmbWaitTime.Name = "cmbWaitTime";
            cmbWaitTime.Size = new Size(140, 23);
            cmbWaitTime.TabIndex = 8;
            // 
            // cmbLatency
            // 
            cmbLatency.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbLatency.FormattingEnabled = true;
            cmbLatency.Items.AddRange(new object[] { "25", "50", "100", "150", "200", "300", "400", "500" });
            cmbLatency.Location = new Point(125, 359);
            cmbLatency.Margin = new Padding(4);
            cmbLatency.Name = "cmbLatency";
            cmbLatency.Size = new Size(140, 23);
            cmbLatency.TabIndex = 8;
            // 
            // tabPage12
            // 
            tabPage12.Controls.Add(groupBox27);
            tabPage12.Location = new Point(4, 24);
            tabPage12.Margin = new Padding(4);
            tabPage12.Name = "tabPage12";
            tabPage12.Padding = new Padding(4);
            tabPage12.Size = new Size(499, 451);
            tabPage12.TabIndex = 1;
            tabPage12.Text = "音源割り当て";
            tabPage12.UseVisualStyleBackColor = true;
            // 
            // groupBox27
            // 
            groupBox27.Controls.Add(rbManualDetect);
            groupBox27.Controls.Add(rbAutoDetect);
            groupBox27.Controls.Add(groupBox28);
            groupBox27.Controls.Add(groupBox1);
            groupBox27.Dock = DockStyle.Fill;
            groupBox27.Location = new Point(4, 4);
            groupBox27.Margin = new Padding(4);
            groupBox27.Name = "groupBox27";
            groupBox27.Padding = new Padding(4);
            groupBox27.Size = new Size(491, 443);
            groupBox27.TabIndex = 9;
            groupBox27.TabStop = false;
            groupBox27.Text = "音源の割り当て";
            // 
            // rbManualDetect
            // 
            rbManualDetect.AutoSize = true;
            rbManualDetect.Location = new Point(14, 89);
            rbManualDetect.Margin = new Padding(4);
            rbManualDetect.Name = "rbManualDetect";
            rbManualDetect.Size = new Size(49, 19);
            rbManualDetect.TabIndex = 10;
            rbManualDetect.Text = "手動";
            rbManualDetect.UseVisualStyleBackColor = true;
            // 
            // rbAutoDetect
            // 
            rbAutoDetect.AutoSize = true;
            rbAutoDetect.Checked = true;
            rbAutoDetect.Location = new Point(14, 21);
            rbAutoDetect.Margin = new Padding(4);
            rbAutoDetect.Name = "rbAutoDetect";
            rbAutoDetect.Size = new Size(49, 19);
            rbAutoDetect.TabIndex = 10;
            rbAutoDetect.TabStop = true;
            rbAutoDetect.Text = "自動";
            rbAutoDetect.UseVisualStyleBackColor = true;
            // 
            // groupBox28
            // 
            groupBox28.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox28.Controls.Add(rbSCCIDetect);
            groupBox28.Controls.Add(rbC86ctlDetect);
            groupBox28.Location = new Point(7, 22);
            groupBox28.Margin = new Padding(4);
            groupBox28.Name = "groupBox28";
            groupBox28.Padding = new Padding(4);
            groupBox28.Size = new Size(477, 58);
            groupBox28.TabIndex = 9;
            groupBox28.TabStop = false;
            // 
            // rbSCCIDetect
            // 
            rbSCCIDetect.AutoSize = true;
            rbSCCIDetect.Location = new Point(247, 26);
            rbSCCIDetect.Margin = new Padding(4);
            rbSCCIDetect.Name = "rbSCCIDetect";
            rbSCCIDetect.Size = new Size(155, 19);
            rbSCCIDetect.TabIndex = 0;
            rbSCCIDetect.Text = "SCCIのモジュールを優先する";
            rbSCCIDetect.UseVisualStyleBackColor = true;
            // 
            // rbC86ctlDetect
            // 
            rbC86ctlDetect.AutoSize = true;
            rbC86ctlDetect.Checked = true;
            rbC86ctlDetect.Location = new Point(24, 26);
            rbC86ctlDetect.Margin = new Padding(4);
            rbC86ctlDetect.Name = "rbC86ctlDetect";
            rbC86ctlDetect.Size = new Size(164, 19);
            rbC86ctlDetect.TabIndex = 0;
            rbC86ctlDetect.TabStop = true;
            rbC86ctlDetect.Text = "C86ctlのモジュールを優先する";
            rbC86ctlDetect.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.Controls.Add(ucSI);
            groupBox1.Location = new Point(7, 89);
            groupBox1.Margin = new Padding(4);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(4);
            groupBox1.Size = new Size(477, 347);
            groupBox1.TabIndex = 8;
            groupBox1.TabStop = false;
            // 
            // ucSI
            // 
            ucSI.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            ucSI.AutoScroll = true;
            ucSI.Location = new Point(4, 22);
            ucSI.Margin = new Padding(5);
            ucSI.Name = "ucSI";
            ucSI.Size = new Size(470, 321);
            ucSI.TabIndex = 7;
            // 
            // tabPage13
            // 
            tabPage13.Controls.Add(splitContainer1);
            tabPage13.Location = new Point(4, 24);
            tabPage13.Margin = new Padding(4);
            tabPage13.Name = "tabPage13";
            tabPage13.Size = new Size(499, 451);
            tabPage13.TabIndex = 2;
            tabPage13.Text = "MIDI";
            tabPage13.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Margin = new Padding(4);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(label16);
            splitContainer1.Panel1.Controls.Add(dgvMIDIoutPallet);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(tbcMIDIoutList);
            splitContainer1.Panel2.Controls.Add(btnAddVST);
            splitContainer1.Panel2.Controls.Add(label18);
            splitContainer1.Panel2.Controls.Add(btnAddMIDIout);
            splitContainer1.Panel2.Controls.Add(btnSubMIDIout);
            splitContainer1.Size = new Size(499, 451);
            splitContainer1.SplitterDistance = 178;
            splitContainer1.SplitterWidth = 5;
            splitContainer1.TabIndex = 6;
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Location = new Point(4, 0);
            label16.Margin = new Padding(4, 0, 4, 0);
            label16.Name = "label16";
            label16.Size = new Size(130, 15);
            label16.TabIndex = 0;
            label16.Text = "MIDI Outデバイス パレット";
            // 
            // dgvMIDIoutPallet
            // 
            dgvMIDIoutPallet.AllowUserToAddRows = false;
            dgvMIDIoutPallet.AllowUserToDeleteRows = false;
            dgvMIDIoutPallet.AllowUserToResizeRows = false;
            dgvMIDIoutPallet.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvMIDIoutPallet.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvMIDIoutPallet.Columns.AddRange(new DataGridViewColumn[] { clmID, clmDeviceName, clmManufacturer, clmSpacer });
            dgvMIDIoutPallet.Location = new Point(2, 19);
            dgvMIDIoutPallet.Margin = new Padding(4);
            dgvMIDIoutPallet.MultiSelect = false;
            dgvMIDIoutPallet.Name = "dgvMIDIoutPallet";
            dgvMIDIoutPallet.RowHeadersVisible = false;
            dgvMIDIoutPallet.RowTemplate.Height = 21;
            dgvMIDIoutPallet.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMIDIoutPallet.Size = new Size(491, 155);
            dgvMIDIoutPallet.TabIndex = 1;
            // 
            // clmID
            // 
            clmID.Frozen = true;
            clmID.HeaderText = "ID";
            clmID.Name = "clmID";
            clmID.ReadOnly = true;
            clmID.Visible = false;
            clmID.Width = 40;
            // 
            // clmDeviceName
            // 
            clmDeviceName.Frozen = true;
            clmDeviceName.HeaderText = "Device Name";
            clmDeviceName.Name = "clmDeviceName";
            clmDeviceName.ReadOnly = true;
            clmDeviceName.SortMode = DataGridViewColumnSortMode.NotSortable;
            clmDeviceName.Width = 200;
            // 
            // clmManufacturer
            // 
            clmManufacturer.Frozen = true;
            clmManufacturer.HeaderText = "Manufacturer";
            clmManufacturer.Name = "clmManufacturer";
            clmManufacturer.ReadOnly = true;
            clmManufacturer.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // clmSpacer
            // 
            clmSpacer.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            clmSpacer.HeaderText = "";
            clmSpacer.Name = "clmSpacer";
            clmSpacer.ReadOnly = true;
            clmSpacer.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // tbcMIDIoutList
            // 
            tbcMIDIoutList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tbcMIDIoutList.Controls.Add(tabPage1);
            tbcMIDIoutList.Controls.Add(tabPage2);
            tbcMIDIoutList.Controls.Add(tabPage3);
            tbcMIDIoutList.Controls.Add(tabPage4);
            tbcMIDIoutList.Location = new Point(4, 39);
            tbcMIDIoutList.Margin = new Padding(4);
            tbcMIDIoutList.Name = "tbcMIDIoutList";
            tbcMIDIoutList.SelectedIndex = 0;
            tbcMIDIoutList.Size = new Size(491, 225);
            tbcMIDIoutList.TabIndex = 4;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(dgvMIDIoutListA);
            tabPage1.Controls.Add(btnUP_A);
            tabPage1.Controls.Add(btnDOWN_A);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Margin = new Padding(4);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(4);
            tabPage1.Size = new Size(483, 197);
            tabPage1.TabIndex = 0;
            tabPage1.Tag = "0";
            tabPage1.Text = "GM";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // dgvMIDIoutListA
            // 
            dgvMIDIoutListA.AllowUserToAddRows = false;
            dgvMIDIoutListA.AllowUserToDeleteRows = false;
            dgvMIDIoutListA.AllowUserToResizeRows = false;
            dgvMIDIoutListA.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvMIDIoutListA.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvMIDIoutListA.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn1, clmIsVST, clmFileName, dataGridViewTextBoxColumn2, clmType, ClmBeforeSend, dataGridViewTextBoxColumn3, dataGridViewTextBoxColumn4 });
            dgvMIDIoutListA.Location = new Point(0, 0);
            dgvMIDIoutListA.Margin = new Padding(4);
            dgvMIDIoutListA.MultiSelect = false;
            dgvMIDIoutListA.Name = "dgvMIDIoutListA";
            dgvMIDIoutListA.RowHeadersVisible = false;
            dgvMIDIoutListA.RowTemplate.Height = 21;
            dgvMIDIoutListA.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMIDIoutListA.Size = new Size(447, 183);
            dgvMIDIoutListA.TabIndex = 1;
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewTextBoxColumn1.Frozen = true;
            dataGridViewTextBoxColumn1.HeaderText = "ID";
            dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            dataGridViewTextBoxColumn1.ReadOnly = true;
            dataGridViewTextBoxColumn1.SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridViewTextBoxColumn1.Visible = false;
            dataGridViewTextBoxColumn1.Width = 40;
            // 
            // clmIsVST
            // 
            clmIsVST.HeaderText = "IsVST";
            clmIsVST.Name = "clmIsVST";
            clmIsVST.Visible = false;
            // 
            // clmFileName
            // 
            clmFileName.HeaderText = "fileName";
            clmFileName.Name = "clmFileName";
            clmFileName.Resizable = DataGridViewTriState.True;
            clmFileName.SortMode = DataGridViewColumnSortMode.NotSortable;
            clmFileName.Visible = false;
            // 
            // dataGridViewTextBoxColumn2
            // 
            dataGridViewTextBoxColumn2.HeaderText = "Device Name";
            dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            dataGridViewTextBoxColumn2.ReadOnly = true;
            dataGridViewTextBoxColumn2.SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridViewTextBoxColumn2.Width = 180;
            // 
            // clmType
            // 
            clmType.HeaderText = "Type";
            clmType.Items.AddRange(new object[] { "GM" });
            clmType.Name = "clmType";
            clmType.Resizable = DataGridViewTriState.True;
            clmType.Width = 70;
            // 
            // ClmBeforeSend
            // 
            ClmBeforeSend.HeaderText = "Before Send";
            ClmBeforeSend.Items.AddRange(new object[] { "None", "GM Reset", "XG Reset", "GS Reset", "Custom" });
            ClmBeforeSend.Name = "ClmBeforeSend";
            ClmBeforeSend.Resizable = DataGridViewTriState.True;
            ClmBeforeSend.SortMode = DataGridViewColumnSortMode.Automatic;
            // 
            // dataGridViewTextBoxColumn3
            // 
            dataGridViewTextBoxColumn3.HeaderText = "Manufacturer";
            dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            dataGridViewTextBoxColumn3.ReadOnly = true;
            dataGridViewTextBoxColumn3.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // dataGridViewTextBoxColumn4
            // 
            dataGridViewTextBoxColumn4.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewTextBoxColumn4.HeaderText = "";
            dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            dataGridViewTextBoxColumn4.ReadOnly = true;
            dataGridViewTextBoxColumn4.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // btnUP_A
            // 
            btnUP_A.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnUP_A.Location = new Point(454, 0);
            btnUP_A.Margin = new Padding(4);
            btnUP_A.Name = "btnUP_A";
            btnUP_A.Size = new Size(26, 72);
            btnUP_A.TabIndex = 3;
            btnUP_A.Text = "↑";
            btnUP_A.UseVisualStyleBackColor = true;
            btnUP_A.Click += btnUP_Click;
            // 
            // btnDOWN_A
            // 
            btnDOWN_A.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnDOWN_A.Location = new Point(454, 111);
            btnDOWN_A.Margin = new Padding(4);
            btnDOWN_A.Name = "btnDOWN_A";
            btnDOWN_A.Size = new Size(26, 72);
            btnDOWN_A.TabIndex = 3;
            btnDOWN_A.Text = "↓";
            btnDOWN_A.UseVisualStyleBackColor = true;
            btnDOWN_A.Click += btnDOWN_Click;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(dgvMIDIoutListB);
            tabPage2.Controls.Add(btnUP_B);
            tabPage2.Controls.Add(btnDOWN_B);
            tabPage2.Location = new Point(4, 24);
            tabPage2.Margin = new Padding(4);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(4);
            tabPage2.Size = new Size(483, 184);
            tabPage2.TabIndex = 1;
            tabPage2.Tag = "1";
            tabPage2.Text = "GS";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // dgvMIDIoutListB
            // 
            dgvMIDIoutListB.AllowUserToAddRows = false;
            dgvMIDIoutListB.AllowUserToDeleteRows = false;
            dgvMIDIoutListB.AllowUserToResizeRows = false;
            dgvMIDIoutListB.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvMIDIoutListB.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvMIDIoutListB.Location = new Point(0, 0);
            dgvMIDIoutListB.Margin = new Padding(4);
            dgvMIDIoutListB.MultiSelect = false;
            dgvMIDIoutListB.Name = "dgvMIDIoutListB";
            dgvMIDIoutListB.RowHeadersVisible = false;
            dgvMIDIoutListB.RowTemplate.Height = 21;
            dgvMIDIoutListB.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMIDIoutListB.Size = new Size(449, 192);
            dgvMIDIoutListB.TabIndex = 7;
            // 
            // btnUP_B
            // 
            btnUP_B.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnUP_B.Location = new Point(456, 0);
            btnUP_B.Margin = new Padding(4);
            btnUP_B.Name = "btnUP_B";
            btnUP_B.Size = new Size(26, 72);
            btnUP_B.TabIndex = 5;
            btnUP_B.Text = "↑";
            btnUP_B.UseVisualStyleBackColor = true;
            btnUP_B.Click += btnUP_Click;
            // 
            // btnDOWN_B
            // 
            btnDOWN_B.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnDOWN_B.Location = new Point(456, 119);
            btnDOWN_B.Margin = new Padding(4);
            btnDOWN_B.Name = "btnDOWN_B";
            btnDOWN_B.Size = new Size(26, 72);
            btnDOWN_B.TabIndex = 6;
            btnDOWN_B.Text = "↓";
            btnDOWN_B.UseVisualStyleBackColor = true;
            btnDOWN_B.Click += btnDOWN_Click;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(dgvMIDIoutListC);
            tabPage3.Controls.Add(btnUP_C);
            tabPage3.Controls.Add(btnDOWN_C);
            tabPage3.Location = new Point(4, 24);
            tabPage3.Margin = new Padding(4);
            tabPage3.Name = "tabPage3";
            tabPage3.Size = new Size(483, 184);
            tabPage3.TabIndex = 2;
            tabPage3.Tag = "2";
            tabPage3.Text = "XG";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // dgvMIDIoutListC
            // 
            dgvMIDIoutListC.AllowUserToAddRows = false;
            dgvMIDIoutListC.AllowUserToDeleteRows = false;
            dgvMIDIoutListC.AllowUserToResizeRows = false;
            dgvMIDIoutListC.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvMIDIoutListC.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvMIDIoutListC.Location = new Point(0, 0);
            dgvMIDIoutListC.Margin = new Padding(4);
            dgvMIDIoutListC.MultiSelect = false;
            dgvMIDIoutListC.Name = "dgvMIDIoutListC";
            dgvMIDIoutListC.RowHeadersVisible = false;
            dgvMIDIoutListC.RowTemplate.Height = 21;
            dgvMIDIoutListC.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMIDIoutListC.Size = new Size(449, 192);
            dgvMIDIoutListC.TabIndex = 7;
            // 
            // btnUP_C
            // 
            btnUP_C.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnUP_C.Location = new Point(456, 0);
            btnUP_C.Margin = new Padding(4);
            btnUP_C.Name = "btnUP_C";
            btnUP_C.Size = new Size(26, 72);
            btnUP_C.TabIndex = 5;
            btnUP_C.Text = "↑";
            btnUP_C.UseVisualStyleBackColor = true;
            btnUP_C.Click += btnUP_Click;
            // 
            // btnDOWN_C
            // 
            btnDOWN_C.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnDOWN_C.Location = new Point(456, 119);
            btnDOWN_C.Margin = new Padding(4);
            btnDOWN_C.Name = "btnDOWN_C";
            btnDOWN_C.Size = new Size(26, 72);
            btnDOWN_C.TabIndex = 6;
            btnDOWN_C.Text = "↓";
            btnDOWN_C.UseVisualStyleBackColor = true;
            btnDOWN_C.Click += btnDOWN_Click;
            // 
            // tabPage4
            // 
            tabPage4.Controls.Add(dgvMIDIoutListD);
            tabPage4.Controls.Add(btnUP_D);
            tabPage4.Controls.Add(btnDOWN_D);
            tabPage4.Location = new Point(4, 24);
            tabPage4.Margin = new Padding(4);
            tabPage4.Name = "tabPage4";
            tabPage4.Size = new Size(483, 184);
            tabPage4.TabIndex = 3;
            tabPage4.Tag = "3";
            tabPage4.Text = "VSTi";
            tabPage4.UseVisualStyleBackColor = true;
            // 
            // dgvMIDIoutListD
            // 
            dgvMIDIoutListD.AllowUserToAddRows = false;
            dgvMIDIoutListD.AllowUserToDeleteRows = false;
            dgvMIDIoutListD.AllowUserToResizeRows = false;
            dgvMIDIoutListD.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvMIDIoutListD.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvMIDIoutListD.Location = new Point(0, 0);
            dgvMIDIoutListD.Margin = new Padding(4);
            dgvMIDIoutListD.MultiSelect = false;
            dgvMIDIoutListD.Name = "dgvMIDIoutListD";
            dgvMIDIoutListD.RowHeadersVisible = false;
            dgvMIDIoutListD.RowTemplate.Height = 21;
            dgvMIDIoutListD.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMIDIoutListD.Size = new Size(449, 192);
            dgvMIDIoutListD.TabIndex = 7;
            // 
            // btnUP_D
            // 
            btnUP_D.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnUP_D.Location = new Point(456, 0);
            btnUP_D.Margin = new Padding(4);
            btnUP_D.Name = "btnUP_D";
            btnUP_D.Size = new Size(26, 72);
            btnUP_D.TabIndex = 5;
            btnUP_D.Text = "↑";
            btnUP_D.UseVisualStyleBackColor = true;
            btnUP_D.Click += btnUP_Click;
            // 
            // btnDOWN_D
            // 
            btnDOWN_D.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnDOWN_D.Location = new Point(456, 119);
            btnDOWN_D.Margin = new Padding(4);
            btnDOWN_D.Name = "btnDOWN_D";
            btnDOWN_D.Size = new Size(26, 72);
            btnDOWN_D.TabIndex = 6;
            btnDOWN_D.Text = "↓";
            btnDOWN_D.UseVisualStyleBackColor = true;
            btnDOWN_D.Click += btnDOWN_Click;
            // 
            // btnAddVST
            // 
            btnAddVST.Enabled = false;
            btnAddVST.Location = new Point(307, 5);
            btnAddVST.Margin = new Padding(4);
            btnAddVST.Name = "btnAddVST";
            btnAddVST.Size = new Size(88, 29);
            btnAddVST.TabIndex = 5;
            btnAddVST.Text = "Add VST";
            btnAddVST.UseVisualStyleBackColor = true;
            btnAddVST.Click += btnAddVST_Click;
            // 
            // label18
            // 
            label18.AutoSize = true;
            label18.Location = new Point(7, 20);
            label18.Margin = new Padding(4, 0, 4, 0);
            label18.Name = "label18";
            label18.Size = new Size(80, 15);
            label18.TabIndex = 2;
            label18.Text = "MIDI Outリスト";
            // 
            // btnAddMIDIout
            // 
            btnAddMIDIout.Location = new Point(138, 5);
            btnAddMIDIout.Margin = new Padding(4);
            btnAddMIDIout.Name = "btnAddMIDIout";
            btnAddMIDIout.Size = new Size(77, 30);
            btnAddMIDIout.TabIndex = 3;
            btnAddMIDIout.Text = "↓ +";
            btnAddMIDIout.UseVisualStyleBackColor = true;
            btnAddMIDIout.Click += btnAddMIDIout_Click;
            // 
            // btnSubMIDIout
            // 
            btnSubMIDIout.Location = new Point(222, 5);
            btnSubMIDIout.Margin = new Padding(4);
            btnSubMIDIout.Name = "btnSubMIDIout";
            btnSubMIDIout.Size = new Size(77, 30);
            btnSubMIDIout.TabIndex = 3;
            btnSubMIDIout.Text = "-";
            btnSubMIDIout.UseVisualStyleBackColor = true;
            btnSubMIDIout.Click += btnSubMIDIout_Click;
            // 
            // tpNuked
            // 
            tpNuked.BorderStyle = BorderStyle.FixedSingle;
            tpNuked.Controls.Add(tabControl2);
            tpNuked.Location = new Point(4, 44);
            tpNuked.Margin = new Padding(4);
            tpNuked.Name = "tpNuked";
            tpNuked.Size = new Size(518, 510);
            tpNuked.TabIndex = 14;
            tpNuked.Text = "チップ別";
            tpNuked.UseVisualStyleBackColor = true;
            // 
            // tabControl2
            // 
            tabControl2.Controls.Add(tabPage14);
            tabControl2.Controls.Add(tabPage5);
            tabControl2.Dock = DockStyle.Fill;
            tabControl2.HotTrack = true;
            tabControl2.Location = new Point(0, 0);
            tabControl2.Margin = new Padding(4);
            tabControl2.Name = "tabControl2";
            tabControl2.SelectedIndex = 0;
            tabControl2.Size = new Size(516, 508);
            tabControl2.TabIndex = 1;
            // 
            // tabPage14
            // 
            tabPage14.Controls.Add(groupBox26);
            tabPage14.Location = new Point(4, 24);
            tabPage14.Margin = new Padding(4);
            tabPage14.Name = "tabPage14";
            tabPage14.Padding = new Padding(4);
            tabPage14.Size = new Size(508, 480);
            tabPage14.TabIndex = 0;
            tabPage14.Text = "Nuked-OPN2";
            tabPage14.UseVisualStyleBackColor = true;
            // 
            // groupBox26
            // 
            groupBox26.Controls.Add(rbNukedOPN2OptionYM2612u);
            groupBox26.Controls.Add(rbNukedOPN2OptionYM2612);
            groupBox26.Controls.Add(rbNukedOPN2OptionDiscrete);
            groupBox26.Controls.Add(rbNukedOPN2OptionASIClp);
            groupBox26.Controls.Add(rbNukedOPN2OptionASIC);
            groupBox26.Location = new Point(7, 8);
            groupBox26.Margin = new Padding(4);
            groupBox26.Name = "groupBox26";
            groupBox26.Padding = new Padding(4);
            groupBox26.Size = new Size(371, 160);
            groupBox26.TabIndex = 0;
            groupBox26.TabStop = false;
            groupBox26.Text = "Emulation type";
            // 
            // rbNukedOPN2OptionYM2612u
            // 
            rbNukedOPN2OptionYM2612u.AutoSize = true;
            rbNukedOPN2OptionYM2612u.Location = new Point(7, 105);
            rbNukedOPN2OptionYM2612u.Margin = new Padding(4);
            rbNukedOPN2OptionYM2612u.Name = "rbNukedOPN2OptionYM2612u";
            rbNukedOPN2OptionYM2612u.Size = new Size(198, 19);
            rbNukedOPN2OptionYM2612u.TabIndex = 0;
            rbNukedOPN2OptionYM2612u.TabStop = true;
            rbNukedOPN2OptionYM2612u.Text = "YM2612(without filter emulation)";
            rbNukedOPN2OptionYM2612u.UseVisualStyleBackColor = true;
            // 
            // rbNukedOPN2OptionYM2612
            // 
            rbNukedOPN2OptionYM2612.AutoSize = true;
            rbNukedOPN2OptionYM2612.Location = new Point(7, 78);
            rbNukedOPN2OptionYM2612.Margin = new Padding(4);
            rbNukedOPN2OptionYM2612.Name = "rbNukedOPN2OptionYM2612";
            rbNukedOPN2OptionYM2612.Size = new Size(197, 19);
            rbNukedOPN2OptionYM2612.TabIndex = 0;
            rbNukedOPN2OptionYM2612.TabStop = true;
            rbNukedOPN2OptionYM2612.Text = "YM2612(MD1,MD2 VA2)(default)";
            rbNukedOPN2OptionYM2612.UseVisualStyleBackColor = true;
            // 
            // rbNukedOPN2OptionDiscrete
            // 
            rbNukedOPN2OptionDiscrete.AutoSize = true;
            rbNukedOPN2OptionDiscrete.Location = new Point(7, 22);
            rbNukedOPN2OptionDiscrete.Margin = new Padding(4);
            rbNukedOPN2OptionDiscrete.Name = "rbNukedOPN2OptionDiscrete";
            rbNukedOPN2OptionDiscrete.Size = new Size(122, 19);
            rbNukedOPN2OptionDiscrete.TabIndex = 0;
            rbNukedOPN2OptionDiscrete.TabStop = true;
            rbNukedOPN2OptionDiscrete.Text = "Discrete(Teradrive)";
            rbNukedOPN2OptionDiscrete.UseVisualStyleBackColor = true;
            // 
            // rbNukedOPN2OptionASIClp
            // 
            rbNukedOPN2OptionASIClp.AutoSize = true;
            rbNukedOPN2OptionASIClp.Location = new Point(7, 132);
            rbNukedOPN2OptionASIClp.Margin = new Padding(4);
            rbNukedOPN2OptionASIClp.Name = "rbNukedOPN2OptionASIClp";
            rbNukedOPN2OptionASIClp.Size = new Size(151, 19);
            rbNukedOPN2OptionASIClp.TabIndex = 0;
            rbNukedOPN2OptionASIClp.TabStop = true;
            rbNukedOPN2OptionASIClp.Text = "ASIC(with lowpass filter)";
            rbNukedOPN2OptionASIClp.UseVisualStyleBackColor = true;
            // 
            // rbNukedOPN2OptionASIC
            // 
            rbNukedOPN2OptionASIC.AutoSize = true;
            rbNukedOPN2OptionASIC.Location = new Point(7, 50);
            rbNukedOPN2OptionASIC.Margin = new Padding(4);
            rbNukedOPN2OptionASIC.Name = "rbNukedOPN2OptionASIC";
            rbNukedOPN2OptionASIC.Size = new Size(181, 19);
            rbNukedOPN2OptionASIC.TabIndex = 0;
            rbNukedOPN2OptionASIC.TabStop = true;
            rbNukedOPN2OptionASIC.Text = "ASIC(MD1 VA7,MD2,MD3,etc)";
            rbNukedOPN2OptionASIC.UseVisualStyleBackColor = true;
            // 
            // tabPage5
            // 
            tabPage5.Controls.Add(groupBox34);
            tabPage5.Location = new Point(4, 24);
            tabPage5.Margin = new Padding(4);
            tabPage5.Name = "tabPage5";
            tabPage5.Padding = new Padding(4);
            tabPage5.Size = new Size(508, 460);
            tabPage5.TabIndex = 1;
            tabPage5.Text = "Gens";
            tabPage5.UseVisualStyleBackColor = true;
            // 
            // groupBox34
            // 
            groupBox34.Controls.Add(cbGensSSGEG);
            groupBox34.Controls.Add(cbGensDACHPF);
            groupBox34.Location = new Point(7, 8);
            groupBox34.Margin = new Padding(4);
            groupBox34.Name = "groupBox34";
            groupBox34.Padding = new Padding(4);
            groupBox34.Size = new Size(371, 82);
            groupBox34.TabIndex = 1;
            groupBox34.TabStop = false;
            groupBox34.Text = "Gens Emulation option";
            // 
            // cbGensSSGEG
            // 
            cbGensSSGEG.AutoSize = true;
            cbGensSSGEG.Location = new Point(7, 50);
            cbGensSSGEG.Margin = new Padding(4);
            cbGensSSGEG.Name = "cbGensSSGEG";
            cbGensSSGEG.Size = new Size(103, 19);
            cbGensSSGEG.TabIndex = 1;
            cbGensSSGEG.Text = "SSG-EG Enable";
            cbGensSSGEG.UseVisualStyleBackColor = true;
            // 
            // cbGensDACHPF
            // 
            cbGensDACHPF.AutoSize = true;
            cbGensDACHPF.Location = new Point(7, 22);
            cbGensDACHPF.Margin = new Padding(4);
            cbGensDACHPF.Name = "cbGensDACHPF";
            cbGensDACHPF.Size = new Size(139, 19);
            cbGensDACHPF.TabIndex = 0;
            cbGensDACHPF.Text = "DAC Highpass Enable";
            cbGensDACHPF.UseVisualStyleBackColor = true;
            // 
            // tpNSF
            // 
            tpNSF.BorderStyle = BorderStyle.FixedSingle;
            tpNSF.Controls.Add(tabControl3);
            tpNSF.Location = new Point(4, 44);
            tpNSF.Margin = new Padding(4);
            tpNSF.Name = "tpNSF";
            tpNSF.Size = new Size(518, 510);
            tpNSF.TabIndex = 9;
            tpNSF.Text = "MIDI詳細";
            tpNSF.UseVisualStyleBackColor = true;
            // 
            // tabControl3
            // 
            tabControl3.Controls.Add(tabPage15);
            tabControl3.Controls.Add(tabPage16);
            tabControl3.Controls.Add(tabPage17);
            tabControl3.Dock = DockStyle.Fill;
            tabControl3.HotTrack = true;
            tabControl3.Location = new Point(0, 0);
            tabControl3.Margin = new Padding(4);
            tabControl3.Multiline = true;
            tabControl3.Name = "tabControl3";
            tabControl3.SelectedIndex = 0;
            tabControl3.Size = new Size(516, 508);
            tabControl3.TabIndex = 9;
            // 
            // tabPage15
            // 
            tabPage15.Controls.Add(groupBox9);
            tabPage15.Controls.Add(groupBox10);
            tabPage15.Controls.Add(groupBox8);
            tabPage15.Controls.Add(groupBox12);
            tabPage15.Controls.Add(groupBox11);
            tabPage15.Location = new Point(4, 24);
            tabPage15.Margin = new Padding(4);
            tabPage15.Name = "tabPage15";
            tabPage15.Padding = new Padding(4);
            tabPage15.Size = new Size(508, 480);
            tabPage15.TabIndex = 0;
            tabPage15.Text = ".NSF";
            tabPage15.UseVisualStyleBackColor = true;
            // 
            // groupBox9
            // 
            groupBox9.Controls.Add(cbNFSNes_DutySwap);
            groupBox9.Controls.Add(cbNFSNes_PhaseRefresh);
            groupBox9.Controls.Add(cbNFSNes_NonLinearMixer);
            groupBox9.Controls.Add(cbNFSNes_UnmuteOnReset);
            groupBox9.Location = new Point(4, 4);
            groupBox9.Margin = new Padding(4);
            groupBox9.Name = "groupBox9";
            groupBox9.Padding = new Padding(4);
            groupBox9.Size = new Size(245, 134);
            groupBox9.TabIndex = 8;
            groupBox9.TabStop = false;
            groupBox9.Text = "NES";
            // 
            // cbNFSNes_DutySwap
            // 
            cbNFSNes_DutySwap.AutoSize = true;
            cbNFSNes_DutySwap.Location = new Point(7, 105);
            cbNFSNes_DutySwap.Margin = new Padding(4);
            cbNFSNes_DutySwap.Name = "cbNFSNes_DutySwap";
            cbNFSNes_DutySwap.Size = new Size(81, 19);
            cbNFSNes_DutySwap.TabIndex = 7;
            cbNFSNes_DutySwap.Text = "Duty swap";
            cbNFSNes_DutySwap.UseVisualStyleBackColor = true;
            // 
            // cbNFSNes_PhaseRefresh
            // 
            cbNFSNes_PhaseRefresh.AutoSize = true;
            cbNFSNes_PhaseRefresh.Location = new Point(7, 78);
            cbNFSNes_PhaseRefresh.Margin = new Padding(4);
            cbNFSNes_PhaseRefresh.Name = "cbNFSNes_PhaseRefresh";
            cbNFSNes_PhaseRefresh.Size = new Size(96, 19);
            cbNFSNes_PhaseRefresh.TabIndex = 7;
            cbNFSNes_PhaseRefresh.Text = "Phase refresh";
            cbNFSNes_PhaseRefresh.UseVisualStyleBackColor = true;
            // 
            // cbNFSNes_NonLinearMixer
            // 
            cbNFSNes_NonLinearMixer.AutoSize = true;
            cbNFSNes_NonLinearMixer.Location = new Point(7, 50);
            cbNFSNes_NonLinearMixer.Margin = new Padding(4);
            cbNFSNes_NonLinearMixer.Name = "cbNFSNes_NonLinearMixer";
            cbNFSNes_NonLinearMixer.Size = new Size(115, 19);
            cbNFSNes_NonLinearMixer.TabIndex = 7;
            cbNFSNes_NonLinearMixer.Text = "Non-linear mixer";
            cbNFSNes_NonLinearMixer.UseVisualStyleBackColor = true;
            // 
            // cbNFSNes_UnmuteOnReset
            // 
            cbNFSNes_UnmuteOnReset.AutoSize = true;
            cbNFSNes_UnmuteOnReset.Location = new Point(7, 22);
            cbNFSNes_UnmuteOnReset.Margin = new Padding(4);
            cbNFSNes_UnmuteOnReset.Name = "cbNFSNes_UnmuteOnReset";
            cbNFSNes_UnmuteOnReset.Size = new Size(113, 19);
            cbNFSNes_UnmuteOnReset.TabIndex = 7;
            cbNFSNes_UnmuteOnReset.Text = "Unmute on reset";
            cbNFSNes_UnmuteOnReset.UseVisualStyleBackColor = true;
            // 
            // groupBox10
            // 
            groupBox10.Controls.Add(cbNSFDmc_TriNull);
            groupBox10.Controls.Add(cbNSFDmc_TriMute);
            groupBox10.Controls.Add(cbNSFDmc_RandomizeNoise);
            groupBox10.Controls.Add(cbNSFDmc_DPCMAntiClick);
            groupBox10.Controls.Add(cbNSFDmc_EnablePNoise);
            groupBox10.Controls.Add(cbNSFDmc_Enable4011);
            groupBox10.Controls.Add(cbNSFDmc_NonLinearMixer);
            groupBox10.Controls.Add(cbNSFDmc_UnmuteOnReset);
            groupBox10.Location = new Point(257, 4);
            groupBox10.Margin = new Padding(4);
            groupBox10.Name = "groupBox10";
            groupBox10.Padding = new Padding(4);
            groupBox10.Size = new Size(245, 250);
            groupBox10.TabIndex = 8;
            groupBox10.TabStop = false;
            groupBox10.Text = "DMC";
            // 
            // cbNSFDmc_TriNull
            // 
            cbNSFDmc_TriNull.AutoSize = true;
            cbNSFDmc_TriNull.Location = new Point(7, 214);
            cbNSFDmc_TriNull.Margin = new Padding(4);
            cbNSFDmc_TriNull.Name = "cbNSFDmc_TriNull";
            cbNSFDmc_TriNull.Size = new Size(65, 19);
            cbNSFDmc_TriNull.TabIndex = 7;
            cbNSFDmc_TriNull.Text = "TRI null";
            cbNSFDmc_TriNull.UseVisualStyleBackColor = true;
            // 
            // cbNSFDmc_TriMute
            // 
            cbNSFDmc_TriMute.AutoSize = true;
            cbNSFDmc_TriMute.Location = new Point(7, 186);
            cbNSFDmc_TriMute.Margin = new Padding(4);
            cbNSFDmc_TriMute.Name = "cbNSFDmc_TriMute";
            cbNSFDmc_TriMute.Size = new Size(72, 19);
            cbNSFDmc_TriMute.TabIndex = 7;
            cbNSFDmc_TriMute.Text = "TRI mute";
            cbNSFDmc_TriMute.UseVisualStyleBackColor = true;
            // 
            // cbNSFDmc_RandomizeNoise
            // 
            cbNSFDmc_RandomizeNoise.AutoSize = true;
            cbNSFDmc_RandomizeNoise.Location = new Point(7, 160);
            cbNSFDmc_RandomizeNoise.Margin = new Padding(4);
            cbNSFDmc_RandomizeNoise.Name = "cbNSFDmc_RandomizeNoise";
            cbNSFDmc_RandomizeNoise.Size = new Size(115, 19);
            cbNSFDmc_RandomizeNoise.TabIndex = 7;
            cbNSFDmc_RandomizeNoise.Text = "Randomize noise";
            cbNSFDmc_RandomizeNoise.UseVisualStyleBackColor = true;
            // 
            // cbNSFDmc_DPCMAntiClick
            // 
            cbNSFDmc_DPCMAntiClick.AutoSize = true;
            cbNSFDmc_DPCMAntiClick.Location = new Point(7, 132);
            cbNSFDmc_DPCMAntiClick.Margin = new Padding(4);
            cbNSFDmc_DPCMAntiClick.Name = "cbNSFDmc_DPCMAntiClick";
            cbNSFDmc_DPCMAntiClick.Size = new Size(109, 19);
            cbNSFDmc_DPCMAntiClick.TabIndex = 7;
            cbNSFDmc_DPCMAntiClick.Text = "DPCM anti click";
            cbNSFDmc_DPCMAntiClick.UseVisualStyleBackColor = true;
            // 
            // cbNSFDmc_EnablePNoise
            // 
            cbNSFDmc_EnablePNoise.AutoSize = true;
            cbNSFDmc_EnablePNoise.Location = new Point(7, 105);
            cbNSFDmc_EnablePNoise.Margin = new Padding(4);
            cbNSFDmc_EnablePNoise.Name = "cbNSFDmc_EnablePNoise";
            cbNSFDmc_EnablePNoise.Size = new Size(99, 19);
            cbNSFDmc_EnablePNoise.TabIndex = 7;
            cbNSFDmc_EnablePNoise.Text = "Enable Pnoise";
            cbNSFDmc_EnablePNoise.UseVisualStyleBackColor = true;
            // 
            // cbNSFDmc_Enable4011
            // 
            cbNSFDmc_Enable4011.AutoSize = true;
            cbNSFDmc_Enable4011.Location = new Point(7, 78);
            cbNSFDmc_Enable4011.Margin = new Padding(4);
            cbNSFDmc_Enable4011.Name = "cbNSFDmc_Enable4011";
            cbNSFDmc_Enable4011.Size = new Size(94, 19);
            cbNSFDmc_Enable4011.TabIndex = 7;
            cbNSFDmc_Enable4011.Text = "Enable $4011";
            cbNSFDmc_Enable4011.UseVisualStyleBackColor = true;
            // 
            // cbNSFDmc_NonLinearMixer
            // 
            cbNSFDmc_NonLinearMixer.AutoSize = true;
            cbNSFDmc_NonLinearMixer.Location = new Point(7, 50);
            cbNSFDmc_NonLinearMixer.Margin = new Padding(4);
            cbNSFDmc_NonLinearMixer.Name = "cbNSFDmc_NonLinearMixer";
            cbNSFDmc_NonLinearMixer.Size = new Size(115, 19);
            cbNSFDmc_NonLinearMixer.TabIndex = 7;
            cbNSFDmc_NonLinearMixer.Text = "Non-linear mixer";
            cbNSFDmc_NonLinearMixer.UseVisualStyleBackColor = true;
            // 
            // cbNSFDmc_UnmuteOnReset
            // 
            cbNSFDmc_UnmuteOnReset.AutoSize = true;
            cbNSFDmc_UnmuteOnReset.Location = new Point(7, 22);
            cbNSFDmc_UnmuteOnReset.Margin = new Padding(4);
            cbNSFDmc_UnmuteOnReset.Name = "cbNSFDmc_UnmuteOnReset";
            cbNSFDmc_UnmuteOnReset.Size = new Size(113, 19);
            cbNSFDmc_UnmuteOnReset.TabIndex = 7;
            cbNSFDmc_UnmuteOnReset.Text = "Unmute on reset";
            cbNSFDmc_UnmuteOnReset.UseVisualStyleBackColor = true;
            // 
            // groupBox8
            // 
            groupBox8.Controls.Add(label21);
            groupBox8.Controls.Add(label20);
            groupBox8.Controls.Add(tbNSFFds_LPF);
            groupBox8.Controls.Add(cbNFSFds_4085Reset);
            groupBox8.Controls.Add(cbNSFFDSWriteDisable8000);
            groupBox8.Location = new Point(4, 145);
            groupBox8.Margin = new Padding(4);
            groupBox8.Name = "groupBox8";
            groupBox8.Padding = new Padding(4);
            groupBox8.Size = new Size(245, 109);
            groupBox8.TabIndex = 8;
            groupBox8.TabStop = false;
            groupBox8.Text = "FDS";
            // 
            // label21
            // 
            label21.AutoSize = true;
            label21.Location = new Point(167, 20);
            label21.Margin = new Padding(4, 0, 4, 0);
            label21.Name = "label21";
            label21.Size = new Size(21, 15);
            label21.TabIndex = 10;
            label21.Text = "Hz";
            // 
            // label20
            // 
            label20.AutoSize = true;
            label20.Location = new Point(7, 20);
            label20.Margin = new Padding(4, 0, 4, 0);
            label20.Name = "label20";
            label20.Size = new Size(26, 15);
            label20.TabIndex = 10;
            label20.Text = "LPF";
            // 
            // tbNSFFds_LPF
            // 
            tbNSFFds_LPF.Location = new Point(43, 16);
            tbNSFFds_LPF.Margin = new Padding(4);
            tbNSFFds_LPF.Name = "tbNSFFds_LPF";
            tbNSFFds_LPF.Size = new Size(116, 23);
            tbNSFFds_LPF.TabIndex = 9;
            // 
            // cbNFSFds_4085Reset
            // 
            cbNFSFds_4085Reset.AutoSize = true;
            cbNFSFds_4085Reset.Location = new Point(7, 48);
            cbNFSFds_4085Reset.Margin = new Padding(4);
            cbNFSFds_4085Reset.Name = "cbNFSFds_4085Reset";
            cbNFSFds_4085Reset.Size = new Size(84, 19);
            cbNFSFds_4085Reset.TabIndex = 7;
            cbNFSFds_4085Reset.Text = "$4085 reset";
            cbNFSFds_4085Reset.UseVisualStyleBackColor = true;
            // 
            // cbNSFFDSWriteDisable8000
            // 
            cbNSFFDSWriteDisable8000.AutoSize = true;
            cbNSFFDSWriteDisable8000.Location = new Point(7, 75);
            cbNSFFDSWriteDisable8000.Margin = new Padding(4);
            cbNSFFDSWriteDisable8000.Name = "cbNSFFDSWriteDisable8000";
            cbNSFFDSWriteDisable8000.Size = new Size(175, 19);
            cbNSFFDSWriteDisable8000.TabIndex = 7;
            cbNSFFDSWriteDisable8000.Text = "Write disable($8000 - $DFFF)";
            cbNSFFDSWriteDisable8000.UseVisualStyleBackColor = true;
            // 
            // groupBox12
            // 
            groupBox12.Controls.Add(cbNSFN160_Serial);
            groupBox12.Location = new Point(257, 261);
            groupBox12.Margin = new Padding(4);
            groupBox12.Name = "groupBox12";
            groupBox12.Padding = new Padding(4);
            groupBox12.Size = new Size(245, 78);
            groupBox12.TabIndex = 8;
            groupBox12.TabStop = false;
            groupBox12.Text = "N160";
            // 
            // cbNSFN160_Serial
            // 
            cbNSFN160_Serial.AutoSize = true;
            cbNSFN160_Serial.Location = new Point(7, 22);
            cbNSFN160_Serial.Margin = new Padding(4);
            cbNSFN160_Serial.Name = "cbNSFN160_Serial";
            cbNSFN160_Serial.Size = new Size(54, 19);
            cbNSFN160_Serial.TabIndex = 7;
            cbNSFN160_Serial.Text = "Serial";
            cbNSFN160_Serial.UseVisualStyleBackColor = true;
            // 
            // groupBox11
            // 
            groupBox11.Controls.Add(cbNSFMmc5_PhaseRefresh);
            groupBox11.Controls.Add(cbNSFMmc5_NonLinearMixer);
            groupBox11.Location = new Point(4, 261);
            groupBox11.Margin = new Padding(4);
            groupBox11.Name = "groupBox11";
            groupBox11.Padding = new Padding(4);
            groupBox11.Size = new Size(245, 78);
            groupBox11.TabIndex = 8;
            groupBox11.TabStop = false;
            groupBox11.Text = "MMC5";
            // 
            // cbNSFMmc5_PhaseRefresh
            // 
            cbNSFMmc5_PhaseRefresh.AutoSize = true;
            cbNSFMmc5_PhaseRefresh.Location = new Point(7, 50);
            cbNSFMmc5_PhaseRefresh.Margin = new Padding(4);
            cbNSFMmc5_PhaseRefresh.Name = "cbNSFMmc5_PhaseRefresh";
            cbNSFMmc5_PhaseRefresh.Size = new Size(96, 19);
            cbNSFMmc5_PhaseRefresh.TabIndex = 7;
            cbNSFMmc5_PhaseRefresh.Text = "Phase refresh";
            cbNSFMmc5_PhaseRefresh.UseVisualStyleBackColor = true;
            // 
            // cbNSFMmc5_NonLinearMixer
            // 
            cbNSFMmc5_NonLinearMixer.AutoSize = true;
            cbNSFMmc5_NonLinearMixer.Location = new Point(7, 22);
            cbNSFMmc5_NonLinearMixer.Margin = new Padding(4);
            cbNSFMmc5_NonLinearMixer.Name = "cbNSFMmc5_NonLinearMixer";
            cbNSFMmc5_NonLinearMixer.Size = new Size(116, 19);
            cbNSFMmc5_NonLinearMixer.TabIndex = 7;
            cbNSFMmc5_NonLinearMixer.Text = "Non-linear Mixer";
            cbNSFMmc5_NonLinearMixer.UseVisualStyleBackColor = true;
            // 
            // tabPage16
            // 
            tabPage16.Controls.Add(groupBox14);
            tabPage16.Controls.Add(groupBox13);
            tabPage16.Controls.Add(label49);
            tabPage16.Controls.Add(label51);
            tabPage16.Controls.Add(tbSIDOutputBufferSize);
            tabPage16.Location = new Point(4, 24);
            tabPage16.Margin = new Padding(4);
            tabPage16.Name = "tabPage16";
            tabPage16.Padding = new Padding(4);
            tabPage16.Size = new Size(508, 460);
            tabPage16.TabIndex = 1;
            tabPage16.Text = ".SID";
            tabPage16.UseVisualStyleBackColor = true;
            // 
            // groupBox14
            // 
            groupBox14.Controls.Add(label27);
            groupBox14.Controls.Add(label26);
            groupBox14.Controls.Add(label25);
            groupBox14.Controls.Add(rdSIDQ1);
            groupBox14.Controls.Add(rdSIDQ3);
            groupBox14.Controls.Add(rdSIDQ2);
            groupBox14.Controls.Add(rdSIDQ4);
            groupBox14.Location = new Point(4, 136);
            groupBox14.Margin = new Padding(4);
            groupBox14.Name = "groupBox14";
            groupBox14.Padding = new Padding(4);
            groupBox14.Size = new Size(327, 139);
            groupBox14.TabIndex = 2;
            groupBox14.TabStop = false;
            groupBox14.Text = "Quality";
            // 
            // label27
            // 
            label27.AutoSize = true;
            label27.Location = new Point(189, 108);
            label27.Margin = new Padding(4, 0, 4, 0);
            label27.Name = "label27";
            label27.Size = new Size(64, 15);
            label27.TabIndex = 2;
            label27.Text = "Low(Light)";
            // 
            // label26
            // 
            label26.AutoSize = true;
            label26.Location = new Point(189, 68);
            label26.Margin = new Padding(4, 0, 4, 0);
            label26.Name = "label26";
            label26.Size = new Size(44, 15);
            label26.TabIndex = 2;
            label26.Text = "Middle";
            // 
            // label25
            // 
            label25.AutoSize = true;
            label25.Location = new Point(189, 25);
            label25.Margin = new Padding(4, 0, 4, 0);
            label25.Name = "label25";
            label25.Size = new Size(74, 15);
            label25.TabIndex = 2;
            label25.Text = "High(Heavy)";
            // 
            // rdSIDQ1
            // 
            rdSIDQ1.AutoSize = true;
            rdSIDQ1.Checked = true;
            rdSIDQ1.Location = new Point(7, 105);
            rdSIDQ1.Margin = new Padding(4);
            rdSIDQ1.Name = "rdSIDQ1";
            rdSIDQ1.Size = new Size(112, 19);
            rdSIDQ1.TabIndex = 1;
            rdSIDQ1.TabStop = true;
            rdSIDQ1.Text = "Interpolate - fast";
            rdSIDQ1.UseVisualStyleBackColor = true;
            // 
            // rdSIDQ3
            // 
            rdSIDQ3.AutoSize = true;
            rdSIDQ3.Location = new Point(7, 50);
            rdSIDQ3.Margin = new Padding(4);
            rdSIDQ3.Name = "rdSIDQ3";
            rdSIDQ3.Size = new Size(105, 19);
            rdSIDQ3.TabIndex = 1;
            rdSIDQ3.Text = "Resample - fast";
            rdSIDQ3.UseVisualStyleBackColor = true;
            // 
            // rdSIDQ2
            // 
            rdSIDQ2.AutoSize = true;
            rdSIDQ2.Location = new Point(7, 78);
            rdSIDQ2.Margin = new Padding(4);
            rdSIDQ2.Name = "rdSIDQ2";
            rdSIDQ2.Size = new Size(82, 19);
            rdSIDQ2.TabIndex = 1;
            rdSIDQ2.Text = "Interpolate";
            rdSIDQ2.UseVisualStyleBackColor = true;
            // 
            // rdSIDQ4
            // 
            rdSIDQ4.AutoSize = true;
            rdSIDQ4.Location = new Point(7, 22);
            rdSIDQ4.Margin = new Padding(4);
            rdSIDQ4.Name = "rdSIDQ4";
            rdSIDQ4.Size = new Size(75, 19);
            rdSIDQ4.TabIndex = 1;
            rdSIDQ4.Text = "Resample";
            rdSIDQ4.UseVisualStyleBackColor = true;
            // 
            // groupBox13
            // 
            groupBox13.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox13.Controls.Add(btnSIDBasic);
            groupBox13.Controls.Add(btnSIDCharacter);
            groupBox13.Controls.Add(btnSIDKernal);
            groupBox13.Controls.Add(tbSIDCharacter);
            groupBox13.Controls.Add(tbSIDBasic);
            groupBox13.Controls.Add(tbSIDKernal);
            groupBox13.Controls.Add(label24);
            groupBox13.Controls.Add(label23);
            groupBox13.Controls.Add(label22);
            groupBox13.Location = new Point(4, 4);
            groupBox13.Margin = new Padding(4);
            groupBox13.Name = "groupBox13";
            groupBox13.Padding = new Padding(4);
            groupBox13.Size = new Size(499, 125);
            groupBox13.TabIndex = 0;
            groupBox13.TabStop = false;
            groupBox13.Text = "ROM Image";
            // 
            // btnSIDBasic
            // 
            btnSIDBasic.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSIDBasic.Location = new Point(466, 55);
            btnSIDBasic.Margin = new Padding(4);
            btnSIDBasic.Name = "btnSIDBasic";
            btnSIDBasic.Size = new Size(27, 29);
            btnSIDBasic.TabIndex = 2;
            btnSIDBasic.Text = "...";
            btnSIDBasic.UseVisualStyleBackColor = true;
            btnSIDBasic.Click += btnSIDBasic_Click;
            // 
            // btnSIDCharacter
            // 
            btnSIDCharacter.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSIDCharacter.Location = new Point(466, 86);
            btnSIDCharacter.Margin = new Padding(4);
            btnSIDCharacter.Name = "btnSIDCharacter";
            btnSIDCharacter.Size = new Size(27, 29);
            btnSIDCharacter.TabIndex = 2;
            btnSIDCharacter.Text = "...";
            btnSIDCharacter.UseVisualStyleBackColor = true;
            btnSIDCharacter.Click += btnSIDCharacter_Click;
            // 
            // btnSIDKernal
            // 
            btnSIDKernal.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSIDKernal.Location = new Point(466, 24);
            btnSIDKernal.Margin = new Padding(4);
            btnSIDKernal.Name = "btnSIDKernal";
            btnSIDKernal.Size = new Size(27, 29);
            btnSIDKernal.TabIndex = 2;
            btnSIDKernal.Text = "...";
            btnSIDKernal.UseVisualStyleBackColor = true;
            btnSIDKernal.Click += btnSIDKernal_Click;
            // 
            // tbSIDCharacter
            // 
            tbSIDCharacter.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tbSIDCharacter.Location = new Point(78, 89);
            tbSIDCharacter.Margin = new Padding(4);
            tbSIDCharacter.Name = "tbSIDCharacter";
            tbSIDCharacter.Size = new Size(380, 23);
            tbSIDCharacter.TabIndex = 1;
            // 
            // tbSIDBasic
            // 
            tbSIDBasic.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tbSIDBasic.Location = new Point(78, 58);
            tbSIDBasic.Margin = new Padding(4);
            tbSIDBasic.Name = "tbSIDBasic";
            tbSIDBasic.Size = new Size(380, 23);
            tbSIDBasic.TabIndex = 1;
            // 
            // tbSIDKernal
            // 
            tbSIDKernal.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tbSIDKernal.Location = new Point(78, 26);
            tbSIDKernal.Margin = new Padding(4);
            tbSIDKernal.Name = "tbSIDKernal";
            tbSIDKernal.Size = new Size(380, 23);
            tbSIDKernal.TabIndex = 1;
            // 
            // label24
            // 
            label24.AutoSize = true;
            label24.Location = new Point(7, 92);
            label24.Margin = new Padding(4, 0, 4, 0);
            label24.Name = "label24";
            label24.Size = new Size(57, 15);
            label24.TabIndex = 0;
            label24.Text = "Character";
            // 
            // label23
            // 
            label23.AutoSize = true;
            label23.Location = new Point(7, 61);
            label23.Margin = new Padding(4, 0, 4, 0);
            label23.Name = "label23";
            label23.Size = new Size(34, 15);
            label23.TabIndex = 0;
            label23.Text = "Basic";
            // 
            // label22
            // 
            label22.AutoSize = true;
            label22.Location = new Point(7, 30);
            label22.Margin = new Padding(4, 0, 4, 0);
            label22.Name = "label22";
            label22.Size = new Size(40, 15);
            label22.TabIndex = 0;
            label22.Text = "Kernal";
            // 
            // label49
            // 
            label49.AutoSize = true;
            label49.Location = new Point(10, 286);
            label49.Margin = new Padding(4, 0, 4, 0);
            label49.Name = "label49";
            label49.Size = new Size(98, 15);
            label49.TabIndex = 0;
            label49.Text = "OutputBuffer size";
            // 
            // label51
            // 
            label51.Location = new Point(244, 280);
            label51.Margin = new Padding(4, 0, 4, 0);
            label51.Name = "label51";
            label51.Size = new Size(260, 48);
            label51.TabIndex = 0;
            label51.Text = "テンポが速かったり、音が途切れる場合に調整すると改善することがあります。通常は5000。";
            // 
            // tbSIDOutputBufferSize
            // 
            tbSIDOutputBufferSize.Location = new Point(128, 282);
            tbSIDOutputBufferSize.Margin = new Padding(4);
            tbSIDOutputBufferSize.MaxLength = 10;
            tbSIDOutputBufferSize.Name = "tbSIDOutputBufferSize";
            tbSIDOutputBufferSize.Size = new Size(108, 23);
            tbSIDOutputBufferSize.TabIndex = 1;
            // 
            // tabPage17
            // 
            tabPage17.Controls.Add(groupBox15);
            tabPage17.Location = new Point(4, 24);
            tabPage17.Margin = new Padding(4);
            tabPage17.Name = "tabPage17";
            tabPage17.Size = new Size(508, 460);
            tabPage17.TabIndex = 2;
            tabPage17.Text = "Resetコマンド";
            tabPage17.UseVisualStyleBackColor = true;
            // 
            // groupBox15
            // 
            groupBox15.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox15.Controls.Add(btnBeforeSend_Default);
            groupBox15.Controls.Add(tbBeforeSend_Custom);
            groupBox15.Controls.Add(tbBeforeSend_XGReset);
            groupBox15.Controls.Add(label35);
            groupBox15.Controls.Add(label34);
            groupBox15.Controls.Add(label32);
            groupBox15.Controls.Add(tbBeforeSend_GSReset);
            groupBox15.Controls.Add(label33);
            groupBox15.Controls.Add(tbBeforeSend_GMReset);
            groupBox15.Controls.Add(label31);
            groupBox15.Location = new Point(4, 4);
            groupBox15.Margin = new Padding(4);
            groupBox15.Name = "groupBox15";
            groupBox15.Padding = new Padding(4);
            groupBox15.Size = new Size(499, 249);
            groupBox15.TabIndex = 0;
            groupBox15.TabStop = false;
            groupBox15.Text = "Before Send";
            // 
            // btnBeforeSend_Default
            // 
            btnBeforeSend_Default.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBeforeSend_Default.Location = new Point(406, 154);
            btnBeforeSend_Default.Margin = new Padding(4);
            btnBeforeSend_Default.Name = "btnBeforeSend_Default";
            btnBeforeSend_Default.Size = new Size(88, 29);
            btnBeforeSend_Default.TabIndex = 2;
            btnBeforeSend_Default.Text = "元に戻す";
            btnBeforeSend_Default.UseVisualStyleBackColor = true;
            btnBeforeSend_Default.Click += btnBeforeSend_Default_Click;
            // 
            // tbBeforeSend_Custom
            // 
            tbBeforeSend_Custom.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tbBeforeSend_Custom.Location = new Point(105, 122);
            tbBeforeSend_Custom.Margin = new Padding(4);
            tbBeforeSend_Custom.Name = "tbBeforeSend_Custom";
            tbBeforeSend_Custom.Size = new Size(388, 23);
            tbBeforeSend_Custom.TabIndex = 1;
            // 
            // tbBeforeSend_XGReset
            // 
            tbBeforeSend_XGReset.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tbBeforeSend_XGReset.Location = new Point(105, 60);
            tbBeforeSend_XGReset.Margin = new Padding(4);
            tbBeforeSend_XGReset.Name = "tbBeforeSend_XGReset";
            tbBeforeSend_XGReset.Size = new Size(388, 23);
            tbBeforeSend_XGReset.TabIndex = 1;
            // 
            // label35
            // 
            label35.AutoSize = true;
            label35.Font = new Font("Consolas", 9F);
            label35.Location = new Point(7, 160);
            label35.Margin = new Padding(4, 0, 4, 0);
            label35.Name = "label35";
            label35.Size = new Size(329, 28);
            label35.TabIndex = 0;
            label35.Text = "Format:\r\n  (delayTime(dec)):(command data(hex)),...;...\r\n";
            // 
            // label34
            // 
            label34.AutoSize = true;
            label34.Location = new Point(7, 126);
            label34.Margin = new Padding(4, 0, 4, 0);
            label34.Name = "label34";
            label34.Size = new Size(47, 15);
            label34.TabIndex = 0;
            label34.Text = "Custom";
            // 
            // label32
            // 
            label32.AutoSize = true;
            label32.Location = new Point(7, 64);
            label32.Margin = new Padding(4, 0, 4, 0);
            label32.Name = "label32";
            label32.Size = new Size(78, 15);
            label32.TabIndex = 0;
            label32.Text = "XG SystemOn";
            // 
            // tbBeforeSend_GSReset
            // 
            tbBeforeSend_GSReset.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tbBeforeSend_GSReset.Location = new Point(105, 91);
            tbBeforeSend_GSReset.Margin = new Padding(4);
            tbBeforeSend_GSReset.Name = "tbBeforeSend_GSReset";
            tbBeforeSend_GSReset.Size = new Size(388, 23);
            tbBeforeSend_GSReset.TabIndex = 1;
            // 
            // label33
            // 
            label33.AutoSize = true;
            label33.Location = new Point(7, 95);
            label33.Margin = new Padding(4, 0, 4, 0);
            label33.Name = "label33";
            label33.Size = new Size(52, 15);
            label33.TabIndex = 0;
            label33.Text = "GS Reset";
            // 
            // tbBeforeSend_GMReset
            // 
            tbBeforeSend_GMReset.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tbBeforeSend_GMReset.Location = new Point(105, 29);
            tbBeforeSend_GMReset.Margin = new Padding(4);
            tbBeforeSend_GMReset.Name = "tbBeforeSend_GMReset";
            tbBeforeSend_GMReset.Size = new Size(388, 23);
            tbBeforeSend_GMReset.TabIndex = 1;
            // 
            // label31
            // 
            label31.AutoSize = true;
            label31.Location = new Point(7, 32);
            label31.Margin = new Padding(4, 0, 4, 0);
            label31.Name = "label31";
            label31.Size = new Size(82, 15);
            label31.TabIndex = 0;
            label31.Text = "GM SystemOn";
            // 
            // tpPMDDotNET
            // 
            tpPMDDotNET.BorderStyle = BorderStyle.FixedSingle;
            tpPMDDotNET.Controls.Add(rbPMDManual);
            tpPMDDotNET.Controls.Add(rbPMDAuto);
            tpPMDDotNET.Controls.Add(btnPMDResetDriverArguments);
            tpPMDDotNET.Controls.Add(label47);
            tpPMDDotNET.Controls.Add(btnPMDResetCompilerArhguments);
            tpPMDDotNET.Controls.Add(tbPMDDriverArguments);
            tpPMDDotNET.Controls.Add(label37);
            tpPMDDotNET.Controls.Add(tbPMDCompilerArguments);
            tpPMDDotNET.Controls.Add(gbPMDManual);
            tpPMDDotNET.Location = new Point(4, 44);
            tpPMDDotNET.Margin = new Padding(4);
            tpPMDDotNET.Name = "tpPMDDotNET";
            tpPMDDotNET.Size = new Size(518, 510);
            tpPMDDotNET.TabIndex = 15;
            tpPMDDotNET.Text = "PMDDotNET";
            tpPMDDotNET.UseVisualStyleBackColor = true;
            // 
            // rbPMDManual
            // 
            rbPMDManual.AutoSize = true;
            rbPMDManual.Location = new Point(10, 72);
            rbPMDManual.Margin = new Padding(4);
            rbPMDManual.Name = "rbPMDManual";
            rbPMDManual.Size = new Size(65, 19);
            rbPMDManual.TabIndex = 3;
            rbPMDManual.TabStop = true;
            rbPMDManual.Text = "Manual";
            rbPMDManual.UseVisualStyleBackColor = true;
            rbPMDManual.CheckedChanged += rbPMDManual_CheckedChanged;
            // 
            // rbPMDAuto
            // 
            rbPMDAuto.AutoSize = true;
            rbPMDAuto.Location = new Point(10, 44);
            rbPMDAuto.Margin = new Padding(4);
            rbPMDAuto.Name = "rbPMDAuto";
            rbPMDAuto.Size = new Size(51, 19);
            rbPMDAuto.TabIndex = 3;
            rbPMDAuto.TabStop = true;
            rbPMDAuto.Text = "Auto";
            rbPMDAuto.UseVisualStyleBackColor = true;
            // 
            // btnPMDResetDriverArguments
            // 
            btnPMDResetDriverArguments.Location = new Point(439, 464);
            btnPMDResetDriverArguments.Margin = new Padding(4);
            btnPMDResetDriverArguments.Name = "btnPMDResetDriverArguments";
            btnPMDResetDriverArguments.Size = new Size(75, 29);
            btnPMDResetDriverArguments.TabIndex = 2;
            btnPMDResetDriverArguments.Text = "clear";
            btnPMDResetDriverArguments.UseVisualStyleBackColor = true;
            btnPMDResetDriverArguments.Click += btnPMDResetDriverArguments_Click;
            // 
            // label47
            // 
            label47.AutoSize = true;
            label47.Location = new Point(8, 470);
            label47.Margin = new Padding(4, 0, 4, 0);
            label47.Name = "label47";
            label47.Size = new Size(97, 15);
            label47.TabIndex = 1;
            label47.Text = "Driver arguments";
            // 
            // btnPMDResetCompilerArhguments
            // 
            btnPMDResetCompilerArhguments.Location = new Point(439, 10);
            btnPMDResetCompilerArhguments.Margin = new Padding(4);
            btnPMDResetCompilerArhguments.Name = "btnPMDResetCompilerArhguments";
            btnPMDResetCompilerArhguments.Size = new Size(75, 29);
            btnPMDResetCompilerArhguments.TabIndex = 2;
            btnPMDResetCompilerArhguments.Text = "reset";
            btnPMDResetCompilerArhguments.UseVisualStyleBackColor = true;
            btnPMDResetCompilerArhguments.Click += btnPMDResetCompilerArhguments_Click;
            // 
            // tbPMDDriverArguments
            // 
            tbPMDDriverArguments.Location = new Point(140, 466);
            tbPMDDriverArguments.Margin = new Padding(4);
            tbPMDDriverArguments.Name = "tbPMDDriverArguments";
            tbPMDDriverArguments.Size = new Size(291, 23);
            tbPMDDriverArguments.TabIndex = 0;
            // 
            // label37
            // 
            label37.AutoSize = true;
            label37.Location = new Point(8, 16);
            label37.Margin = new Padding(4, 0, 4, 0);
            label37.Name = "label37";
            label37.Size = new Size(113, 15);
            label37.TabIndex = 1;
            label37.Text = "Compiler arguments";
            // 
            // tbPMDCompilerArguments
            // 
            tbPMDCompilerArguments.Location = new Point(140, 12);
            tbPMDCompilerArguments.Margin = new Padding(4);
            tbPMDCompilerArguments.Name = "tbPMDCompilerArguments";
            tbPMDCompilerArguments.Size = new Size(291, 23);
            tbPMDCompilerArguments.TabIndex = 0;
            // 
            // gbPMDManual
            // 
            gbPMDManual.Controls.Add(cbPMDSetManualVolume);
            gbPMDManual.Controls.Add(cbPMDUsePPZ8);
            gbPMDManual.Controls.Add(groupBox32);
            gbPMDManual.Controls.Add(cbPMDUsePPSDRV);
            gbPMDManual.Controls.Add(gbPPSDRV);
            gbPMDManual.Controls.Add(gbPMDSetManualVolume);
            gbPMDManual.Location = new Point(19, 75);
            gbPMDManual.Margin = new Padding(4);
            gbPMDManual.Name = "gbPMDManual";
            gbPMDManual.Padding = new Padding(4);
            gbPMDManual.Size = new Size(495, 379);
            gbPMDManual.TabIndex = 9;
            gbPMDManual.TabStop = false;
            // 
            // cbPMDSetManualVolume
            // 
            cbPMDSetManualVolume.AutoSize = true;
            cbPMDSetManualVolume.Location = new Point(9, 69);
            cbPMDSetManualVolume.Margin = new Padding(4);
            cbPMDSetManualVolume.Name = "cbPMDSetManualVolume";
            cbPMDSetManualVolume.Size = new Size(131, 19);
            cbPMDSetManualVolume.TabIndex = 10;
            cbPMDSetManualVolume.Text = "Set volume(manual)";
            cbPMDSetManualVolume.UseVisualStyleBackColor = true;
            cbPMDSetManualVolume.CheckedChanged += cbPMDSetManualVolume_CheckedChanged;
            // 
            // cbPMDUsePPZ8
            // 
            cbPMDUsePPZ8.AutoSize = true;
            cbPMDUsePPZ8.Location = new Point(7, 351);
            cbPMDUsePPZ8.Margin = new Padding(4);
            cbPMDUsePPZ8.Name = "cbPMDUsePPZ8";
            cbPMDUsePPZ8.Size = new Size(75, 19);
            cbPMDUsePPZ8.TabIndex = 6;
            cbPMDUsePPZ8.Text = "Use PPZ8";
            cbPMDUsePPZ8.UseVisualStyleBackColor = true;
            // 
            // groupBox32
            // 
            groupBox32.Controls.Add(rbPMD86B);
            groupBox32.Controls.Add(rbPMDSpbB);
            groupBox32.Controls.Add(rbPMDNrmB);
            groupBox32.Location = new Point(7, 22);
            groupBox32.Margin = new Padding(4);
            groupBox32.Name = "groupBox32";
            groupBox32.Padding = new Padding(4);
            groupBox32.Size = new Size(332, 45);
            groupBox32.TabIndex = 4;
            groupBox32.TabStop = false;
            groupBox32.Text = "Select board";
            // 
            // rbPMD86B
            // 
            rbPMD86B.AutoSize = true;
            rbPMD86B.Enabled = false;
            rbPMD86B.Location = new Point(227, 18);
            rbPMD86B.Margin = new Padding(4);
            rbPMD86B.Name = "rbPMD86B";
            rbPMD86B.Size = new Size(71, 19);
            rbPMD86B.TabIndex = 0;
            rbPMD86B.TabStop = true;
            rbPMD86B.Text = "86 board";
            rbPMD86B.UseVisualStyleBackColor = true;
            // 
            // rbPMDSpbB
            // 
            rbPMDSpbB.AutoSize = true;
            rbPMDSpbB.Location = new Point(120, 18);
            rbPMDSpbB.Margin = new Padding(4);
            rbPMDSpbB.Name = "rbPMDSpbB";
            rbPMDSpbB.Size = new Size(90, 19);
            rbPMDSpbB.TabIndex = 0;
            rbPMDSpbB.TabStop = true;
            rbPMDSpbB.Text = "Speak board";
            rbPMDSpbB.UseVisualStyleBackColor = true;
            // 
            // rbPMDNrmB
            // 
            rbPMDNrmB.AutoSize = true;
            rbPMDNrmB.Location = new Point(7, 18);
            rbPMDNrmB.Margin = new Padding(4);
            rbPMDNrmB.Name = "rbPMDNrmB";
            rbPMDNrmB.Size = new Size(98, 19);
            rbPMDNrmB.TabIndex = 0;
            rbPMDNrmB.TabStop = true;
            rbPMDNrmB.Text = "Normal board";
            rbPMDNrmB.UseVisualStyleBackColor = true;
            // 
            // cbPMDUsePPSDRV
            // 
            cbPMDUsePPSDRV.AutoSize = true;
            cbPMDUsePPSDRV.Location = new Point(7, 188);
            cbPMDUsePPSDRV.Margin = new Padding(4);
            cbPMDUsePPSDRV.Name = "cbPMDUsePPSDRV";
            cbPMDUsePPSDRV.Size = new Size(90, 19);
            cbPMDUsePPSDRV.TabIndex = 5;
            cbPMDUsePPSDRV.Text = "Use PPSDRV";
            cbPMDUsePPSDRV.UseVisualStyleBackColor = true;
            cbPMDUsePPSDRV.CheckedChanged += cbPMDUsePPSDRV_CheckedChanged;
            // 
            // gbPPSDRV
            // 
            gbPPSDRV.Controls.Add(groupBox33);
            gbPPSDRV.Location = new Point(14, 188);
            gbPPSDRV.Margin = new Padding(4);
            gbPPSDRV.Name = "gbPPSDRV";
            gbPPSDRV.Padding = new Padding(4);
            gbPPSDRV.Size = new Size(474, 156);
            gbPPSDRV.TabIndex = 8;
            gbPPSDRV.TabStop = false;
            // 
            // groupBox33
            // 
            groupBox33.Controls.Add(rbPMDUsePPSDRVManualFreq);
            groupBox33.Controls.Add(label38);
            groupBox33.Controls.Add(rbPMDUsePPSDRVFreqDefault);
            groupBox33.Controls.Add(btnPMDPPSDRVManualWait);
            groupBox33.Controls.Add(label40);
            groupBox33.Controls.Add(tbPMDPPSDRVFreq);
            groupBox33.Controls.Add(label39);
            groupBox33.Controls.Add(tbPMDPPSDRVManualWait);
            groupBox33.Enabled = false;
            groupBox33.Location = new Point(7, 22);
            groupBox33.Margin = new Padding(4);
            groupBox33.Name = "groupBox33";
            groupBox33.Padding = new Padding(4);
            groupBox33.Size = new Size(460, 125);
            groupBox33.TabIndex = 12;
            groupBox33.TabStop = false;
            groupBox33.Text = "TBD";
            // 
            // rbPMDUsePPSDRVManualFreq
            // 
            rbPMDUsePPSDRVManualFreq.AutoSize = true;
            rbPMDUsePPSDRVManualFreq.Location = new Point(258, 55);
            rbPMDUsePPSDRVManualFreq.Margin = new Padding(4);
            rbPMDUsePPSDRVManualFreq.Name = "rbPMDUsePPSDRVManualFreq";
            rbPMDUsePPSDRVManualFreq.Size = new Size(14, 13);
            rbPMDUsePPSDRVManualFreq.TabIndex = 8;
            rbPMDUsePPSDRVManualFreq.TabStop = true;
            rbPMDUsePPSDRVManualFreq.UseVisualStyleBackColor = true;
            rbPMDUsePPSDRVManualFreq.CheckedChanged += rbPMDUsePPSDRVManualFreq_CheckedChanged;
            // 
            // label38
            // 
            label38.AutoSize = true;
            label38.Location = new Point(14, 24);
            label38.Margin = new Padding(4, 0, 4, 0);
            label38.Name = "label38";
            label38.Size = new Size(178, 15);
            label38.TabIndex = 1;
            label38.Text = "(Real Chip) Rendering Frequency";
            // 
            // rbPMDUsePPSDRVFreqDefault
            // 
            rbPMDUsePPSDRVFreqDefault.AutoSize = true;
            rbPMDUsePPSDRVFreqDefault.Location = new Point(258, 21);
            rbPMDUsePPSDRVFreqDefault.Margin = new Padding(4);
            rbPMDUsePPSDRVFreqDefault.Name = "rbPMDUsePPSDRVFreqDefault";
            rbPMDUsePPSDRVFreqDefault.Size = new Size(133, 19);
            rbPMDUsePPSDRVFreqDefault.TabIndex = 8;
            rbPMDUsePPSDRVFreqDefault.TabStop = true;
            rbPMDUsePPSDRVFreqDefault.Text = "Use interface default";
            rbPMDUsePPSDRVFreqDefault.UseVisualStyleBackColor = true;
            // 
            // btnPMDPPSDRVManualWait
            // 
            btnPMDPPSDRVManualWait.Location = new Point(348, 89);
            btnPMDPPSDRVManualWait.Margin = new Padding(4);
            btnPMDPPSDRVManualWait.Name = "btnPMDPPSDRVManualWait";
            btnPMDPPSDRVManualWait.Size = new Size(75, 29);
            btnPMDPPSDRVManualWait.TabIndex = 2;
            btnPMDPPSDRVManualWait.Text = "reset";
            btnPMDPPSDRVManualWait.UseVisualStyleBackColor = true;
            btnPMDPPSDRVManualWait.Click += btnPMDPPSDRVManualWait_Click;
            // 
            // label40
            // 
            label40.AutoSize = true;
            label40.Location = new Point(405, 55);
            label40.Margin = new Padding(4, 0, 4, 0);
            label40.Name = "label40";
            label40.Size = new Size(21, 15);
            label40.TabIndex = 1;
            label40.Text = "Hz";
            // 
            // tbPMDPPSDRVFreq
            // 
            tbPMDPPSDRVFreq.Location = new Point(281, 51);
            tbPMDPPSDRVFreq.Margin = new Padding(4);
            tbPMDPPSDRVFreq.Name = "tbPMDPPSDRVFreq";
            tbPMDPPSDRVFreq.Size = new Size(116, 23);
            tbPMDPPSDRVFreq.TabIndex = 7;
            tbPMDPPSDRVFreq.Click += tbPMDPPSDRVFreq_Click;
            tbPMDPPSDRVFreq.MouseClick += tbPMDPPSDRVFreq_MouseClick;
            // 
            // label39
            // 
            label39.AutoSize = true;
            label39.Location = new Point(14, 95);
            label39.Margin = new Padding(4, 0, 4, 0);
            label39.Name = "label39";
            label39.Size = new Size(182, 15);
            label39.TabIndex = 1;
            label39.Text = "(SCCI) Send syncronize wait value";
            // 
            // tbPMDPPSDRVManualWait
            // 
            tbPMDPPSDRVManualWait.Location = new Point(281, 91);
            tbPMDPPSDRVManualWait.Margin = new Padding(4);
            tbPMDPPSDRVManualWait.Name = "tbPMDPPSDRVManualWait";
            tbPMDPPSDRVManualWait.Size = new Size(59, 23);
            tbPMDPPSDRVManualWait.TabIndex = 7;
            // 
            // gbPMDSetManualVolume
            // 
            gbPMDSetManualVolume.Controls.Add(label41);
            gbPMDSetManualVolume.Controls.Add(label46);
            gbPMDSetManualVolume.Controls.Add(tbPMDVolumeAdpcm);
            gbPMDSetManualVolume.Controls.Add(label42);
            gbPMDSetManualVolume.Controls.Add(tbPMDVolumeRhythm);
            gbPMDSetManualVolume.Controls.Add(label43);
            gbPMDSetManualVolume.Controls.Add(tbPMDVolumeSSG);
            gbPMDSetManualVolume.Controls.Add(label44);
            gbPMDSetManualVolume.Controls.Add(tbPMDVolumeGIMICSSG);
            gbPMDSetManualVolume.Controls.Add(label45);
            gbPMDSetManualVolume.Controls.Add(tbPMDVolumeFM);
            gbPMDSetManualVolume.Location = new Point(15, 69);
            gbPMDSetManualVolume.Margin = new Padding(4);
            gbPMDSetManualVolume.Name = "gbPMDSetManualVolume";
            gbPMDSetManualVolume.Padding = new Padding(4);
            gbPMDSetManualVolume.Size = new Size(472, 116);
            gbPMDSetManualVolume.TabIndex = 11;
            gbPMDSetManualVolume.TabStop = false;
            // 
            // label41
            // 
            label41.AutoSize = true;
            label41.Location = new Point(6, 22);
            label41.Margin = new Padding(4, 0, 4, 0);
            label41.Name = "label41";
            label41.Size = new Size(231, 15);
            label41.TabIndex = 1;
            label41.Text = "OPNA emulator volume(Min:-191  Max:20)";
            // 
            // label46
            // 
            label46.AutoSize = true;
            label46.Location = new Point(7, 80);
            label46.Margin = new Padding(4, 0, 4, 0);
            label46.Name = "label46";
            label46.Size = new Size(197, 15);
            label46.TabIndex = 1;
            label46.Text = "(GIMIC) SSG volume(Min:0 Max:127)";
            // 
            // tbPMDVolumeAdpcm
            // 
            tbPMDVolumeAdpcm.Location = new Point(399, 41);
            tbPMDVolumeAdpcm.Margin = new Padding(4);
            tbPMDVolumeAdpcm.Name = "tbPMDVolumeAdpcm";
            tbPMDVolumeAdpcm.Size = new Size(59, 23);
            tbPMDVolumeAdpcm.TabIndex = 9;
            // 
            // label42
            // 
            label42.AutoSize = true;
            label42.Location = new Point(12, 45);
            label42.Margin = new Padding(4, 0, 4, 0);
            label42.Name = "label42";
            label42.Size = new Size(24, 15);
            label42.TabIndex = 1;
            label42.Text = "FM";
            // 
            // tbPMDVolumeRhythm
            // 
            tbPMDVolumeRhythm.Location = new Point(273, 41);
            tbPMDVolumeRhythm.Margin = new Padding(4);
            tbPMDVolumeRhythm.Name = "tbPMDVolumeRhythm";
            tbPMDVolumeRhythm.Size = new Size(59, 23);
            tbPMDVolumeRhythm.TabIndex = 9;
            // 
            // label43
            // 
            label43.AutoSize = true;
            label43.Location = new Point(110, 45);
            label43.Margin = new Padding(4, 0, 4, 0);
            label43.Name = "label43";
            label43.Size = new Size(27, 15);
            label43.TabIndex = 1;
            label43.Text = "SSG";
            // 
            // tbPMDVolumeSSG
            // 
            tbPMDVolumeSSG.Location = new Point(148, 41);
            tbPMDVolumeSSG.Margin = new Padding(4);
            tbPMDVolumeSSG.Name = "tbPMDVolumeSSG";
            tbPMDVolumeSSG.Size = new Size(59, 23);
            tbPMDVolumeSSG.TabIndex = 9;
            // 
            // label44
            // 
            label44.AutoSize = true;
            label44.Location = new Point(215, 45);
            label44.Margin = new Padding(4, 0, 4, 0);
            label44.Name = "label44";
            label44.Size = new Size(48, 15);
            label44.TabIndex = 1;
            label44.Text = "Rhythm";
            // 
            // tbPMDVolumeGIMICSSG
            // 
            tbPMDVolumeGIMICSSG.Location = new Point(234, 76);
            tbPMDVolumeGIMICSSG.Margin = new Padding(4);
            tbPMDVolumeGIMICSSG.Name = "tbPMDVolumeGIMICSSG";
            tbPMDVolumeGIMICSSG.Size = new Size(59, 23);
            tbPMDVolumeGIMICSSG.TabIndex = 9;
            // 
            // label45
            // 
            label45.AutoSize = true;
            label45.Location = new Point(340, 45);
            label45.Margin = new Padding(4, 0, 4, 0);
            label45.Name = "label45";
            label45.Size = new Size(48, 15);
            label45.TabIndex = 1;
            label45.Text = "ADPCM";
            // 
            // tbPMDVolumeFM
            // 
            tbPMDVolumeFM.Location = new Point(43, 41);
            tbPMDVolumeFM.Margin = new Padding(4);
            tbPMDVolumeFM.Name = "tbPMDVolumeFM";
            tbPMDVolumeFM.Size = new Size(59, 23);
            tbPMDVolumeFM.TabIndex = 9;
            // 
            // tpExport
            // 
            tpExport.BorderStyle = BorderStyle.FixedSingle;
            tpExport.Controls.Add(cbAlwaysAskForLoopCount);
            tpExport.Controls.Add(cbFixedExportPlace);
            tpExport.Controls.Add(gpbFixedExportPlace);
            tpExport.Controls.Add(label48);
            tpExport.Controls.Add(tbLoopTimes);
            tpExport.Controls.Add(lblLoopTimes);
            tpExport.Location = new Point(4, 44);
            tpExport.Margin = new Padding(4);
            tpExport.Name = "tpExport";
            tpExport.Size = new Size(518, 510);
            tpExport.TabIndex = 17;
            tpExport.Text = "Export";
            tpExport.UseVisualStyleBackColor = true;
            // 
            // cbAlwaysAskForLoopCount
            // 
            cbAlwaysAskForLoopCount.AutoSize = true;
            cbAlwaysAskForLoopCount.Location = new Point(16, 111);
            cbAlwaysAskForLoopCount.Margin = new Padding(4);
            cbAlwaysAskForLoopCount.Name = "cbAlwaysAskForLoopCount";
            cbAlwaysAskForLoopCount.Size = new Size(223, 19);
            cbAlwaysAskForLoopCount.TabIndex = 28;
            cbAlwaysAskForLoopCount.Text = "エクスポート時にループ回数を問い合わせる";
            cbAlwaysAskForLoopCount.UseVisualStyleBackColor = true;
            cbAlwaysAskForLoopCount.CheckedChanged += cbFixedExportPlace_CheckedChanged;
            // 
            // cbFixedExportPlace
            // 
            cbFixedExportPlace.AutoSize = true;
            cbFixedExportPlace.Location = new Point(16, 44);
            cbFixedExportPlace.Margin = new Padding(4);
            cbFixedExportPlace.Name = "cbFixedExportPlace";
            cbFixedExportPlace.Size = new Size(155, 19);
            cbFixedExportPlace.TabIndex = 28;
            cbFixedExportPlace.Text = "エクスポート場所を固定する";
            cbFixedExportPlace.UseVisualStyleBackColor = true;
            cbFixedExportPlace.CheckedChanged += cbFixedExportPlace_CheckedChanged;
            // 
            // gpbFixedExportPlace
            // 
            gpbFixedExportPlace.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            gpbFixedExportPlace.Controls.Add(btnFixedExportPlace);
            gpbFixedExportPlace.Controls.Add(label57);
            gpbFixedExportPlace.Controls.Add(tbFixedExportPlacePath);
            gpbFixedExportPlace.Location = new Point(8, 48);
            gpbFixedExportPlace.Margin = new Padding(4);
            gpbFixedExportPlace.Name = "gpbFixedExportPlace";
            gpbFixedExportPlace.Padding = new Padding(4);
            gpbFixedExportPlace.Size = new Size(503, 56);
            gpbFixedExportPlace.TabIndex = 29;
            gpbFixedExportPlace.TabStop = false;
            // 
            // btnFixedExportPlace
            // 
            btnFixedExportPlace.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnFixedExportPlace.Location = new Point(469, 20);
            btnFixedExportPlace.Margin = new Padding(4);
            btnFixedExportPlace.Name = "btnFixedExportPlace";
            btnFixedExportPlace.Size = new Size(27, 29);
            btnFixedExportPlace.TabIndex = 16;
            btnFixedExportPlace.Text = "...";
            btnFixedExportPlace.UseVisualStyleBackColor = true;
            btnFixedExportPlace.Click += btnFixedExportPlace_Click;
            // 
            // label57
            // 
            label57.AutoSize = true;
            label57.Location = new Point(7, 26);
            label57.Margin = new Padding(4, 0, 4, 0);
            label57.Name = "label57";
            label57.Size = new Size(55, 15);
            label57.TabIndex = 14;
            label57.Text = "出力Path";
            // 
            // tbFixedExportPlacePath
            // 
            tbFixedExportPlacePath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tbFixedExportPlacePath.Location = new Point(85, 22);
            tbFixedExportPlacePath.Margin = new Padding(4);
            tbFixedExportPlacePath.Name = "tbFixedExportPlacePath";
            tbFixedExportPlacePath.Size = new Size(376, 23);
            tbFixedExportPlacePath.TabIndex = 15;
            // 
            // label48
            // 
            label48.Location = new Point(8, 16);
            label48.Margin = new Padding(4, 0, 4, 0);
            label48.Name = "label48";
            label48.Size = new Size(234, 24);
            label48.TabIndex = 27;
            label48.Text = "ファイルエクスポート時のループ回数";
            label48.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // tbLoopTimes
            // 
            tbLoopTimes.Location = new Point(250, 16);
            tbLoopTimes.Margin = new Padding(4);
            tbLoopTimes.Name = "tbLoopTimes";
            tbLoopTimes.Size = new Size(60, 23);
            tbLoopTimes.TabIndex = 0;
            // 
            // lblLoopTimes
            // 
            lblLoopTimes.AutoSize = true;
            lblLoopTimes.Location = new Point(317, 20);
            lblLoopTimes.Margin = new Padding(4, 0, 4, 0);
            lblLoopTimes.Name = "lblLoopTimes";
            lblLoopTimes.Size = new Size(19, 15);
            lblLoopTimes.TabIndex = 1;
            lblLoopTimes.Text = "回";
            // 
            // tpMIDIExp
            // 
            tpMIDIExp.BorderStyle = BorderStyle.FixedSingle;
            tpMIDIExp.Controls.Add(cbUseMIDIExport);
            tpMIDIExp.Controls.Add(gbMIDIExport);
            tpMIDIExp.Location = new Point(4, 44);
            tpMIDIExp.Margin = new Padding(4);
            tpMIDIExp.Name = "tpMIDIExp";
            tpMIDIExp.Size = new Size(518, 510);
            tpMIDIExp.TabIndex = 6;
            tpMIDIExp.Text = "MIDIExport";
            tpMIDIExp.UseVisualStyleBackColor = true;
            // 
            // cbUseMIDIExport
            // 
            cbUseMIDIExport.AutoSize = true;
            cbUseMIDIExport.Location = new Point(18, 4);
            cbUseMIDIExport.Margin = new Padding(4);
            cbUseMIDIExport.Name = "cbUseMIDIExport";
            cbUseMIDIExport.Size = new Size(183, 19);
            cbUseMIDIExport.TabIndex = 1;
            cbUseMIDIExport.Text = "演奏時MIDIファイルをexportする";
            cbUseMIDIExport.UseVisualStyleBackColor = true;
            cbUseMIDIExport.CheckedChanged += cbUseMIDIExport_CheckedChanged;
            // 
            // gbMIDIExport
            // 
            gbMIDIExport.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            gbMIDIExport.Controls.Add(cbMIDIKeyOnFnum);
            gbMIDIExport.Controls.Add(cbMIDIUseVOPM);
            gbMIDIExport.Controls.Add(groupBox6);
            gbMIDIExport.Controls.Add(cbMIDIPlayless);
            gbMIDIExport.Controls.Add(btnMIDIOutputPath);
            gbMIDIExport.Controls.Add(lblOutputPath);
            gbMIDIExport.Controls.Add(tbMIDIOutputPath);
            gbMIDIExport.Location = new Point(8, 4);
            gbMIDIExport.Margin = new Padding(4);
            gbMIDIExport.Name = "gbMIDIExport";
            gbMIDIExport.Padding = new Padding(4);
            gbMIDIExport.Size = new Size(503, 582);
            gbMIDIExport.TabIndex = 0;
            gbMIDIExport.TabStop = false;
            // 
            // cbMIDIKeyOnFnum
            // 
            cbMIDIKeyOnFnum.AutoSize = true;
            cbMIDIKeyOnFnum.Location = new Point(24, 82);
            cbMIDIKeyOnFnum.Margin = new Padding(4);
            cbMIDIKeyOnFnum.Name = "cbMIDIKeyOnFnum";
            cbMIDIKeyOnFnum.Size = new Size(176, 19);
            cbMIDIKeyOnFnum.TabIndex = 23;
            cbMIDIKeyOnFnum.Text = "KeyON時のみfnumを評価する";
            cbMIDIKeyOnFnum.UseVisualStyleBackColor = true;
            // 
            // cbMIDIUseVOPM
            // 
            cbMIDIUseVOPM.AutoSize = true;
            cbMIDIUseVOPM.Location = new Point(24, 55);
            cbMIDIUseVOPM.Margin = new Padding(4);
            cbMIDIUseVOPM.Name = "cbMIDIUseVOPM";
            cbMIDIUseVOPM.Size = new Size(197, 19);
            cbMIDIUseVOPM.TabIndex = 23;
            cbMIDIUseVOPM.Text = "VOPMex向けコントロールを出力する";
            cbMIDIUseVOPM.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            groupBox6.Controls.Add(cbMIDIYM2612);
            groupBox6.Controls.Add(cbMIDISN76489Sec);
            groupBox6.Controls.Add(cbMIDIYM2612Sec);
            groupBox6.Controls.Add(cbMIDISN76489);
            groupBox6.Controls.Add(cbMIDIYM2151);
            groupBox6.Controls.Add(cbMIDIYM2610BSec);
            groupBox6.Controls.Add(cbMIDIYM2151Sec);
            groupBox6.Controls.Add(cbMIDIYM2610B);
            groupBox6.Controls.Add(cbMIDIYM2203);
            groupBox6.Controls.Add(cbMIDIYM2608Sec);
            groupBox6.Controls.Add(cbMIDIYM2203Sec);
            groupBox6.Controls.Add(cbMIDIYM2608);
            groupBox6.Location = new Point(23, 141);
            groupBox6.Margin = new Padding(4);
            groupBox6.Name = "groupBox6";
            groupBox6.Padding = new Padding(4);
            groupBox6.Size = new Size(219, 190);
            groupBox6.TabIndex = 22;
            groupBox6.TabStop = false;
            groupBox6.Text = "出力対象音源";
            // 
            // cbMIDIYM2612
            // 
            cbMIDIYM2612.AutoSize = true;
            cbMIDIYM2612.Checked = true;
            cbMIDIYM2612.CheckState = CheckState.Checked;
            cbMIDIYM2612.Location = new Point(7, 22);
            cbMIDIYM2612.Margin = new Padding(4);
            cbMIDIYM2612.Name = "cbMIDIYM2612";
            cbMIDIYM2612.Size = new Size(68, 19);
            cbMIDIYM2612.TabIndex = 21;
            cbMIDIYM2612.Text = "YM2612";
            cbMIDIYM2612.UseVisualStyleBackColor = true;
            // 
            // cbMIDISN76489Sec
            // 
            cbMIDISN76489Sec.AutoSize = true;
            cbMIDISN76489Sec.Enabled = false;
            cbMIDISN76489Sec.Location = new Point(98, 160);
            cbMIDISN76489Sec.Margin = new Padding(4);
            cbMIDISN76489Sec.Name = "cbMIDISN76489Sec";
            cbMIDISN76489Sec.Size = new Size(97, 19);
            cbMIDISN76489Sec.TabIndex = 21;
            cbMIDISN76489Sec.Text = "SN76489(Sec)";
            cbMIDISN76489Sec.UseVisualStyleBackColor = true;
            // 
            // cbMIDIYM2612Sec
            // 
            cbMIDIYM2612Sec.AutoSize = true;
            cbMIDIYM2612Sec.Enabled = false;
            cbMIDIYM2612Sec.Location = new Point(98, 22);
            cbMIDIYM2612Sec.Margin = new Padding(4);
            cbMIDIYM2612Sec.Name = "cbMIDIYM2612Sec";
            cbMIDIYM2612Sec.Size = new Size(94, 19);
            cbMIDIYM2612Sec.TabIndex = 21;
            cbMIDIYM2612Sec.Text = "YM2612(Sec)";
            cbMIDIYM2612Sec.UseVisualStyleBackColor = true;
            // 
            // cbMIDISN76489
            // 
            cbMIDISN76489.AutoSize = true;
            cbMIDISN76489.Enabled = false;
            cbMIDISN76489.Location = new Point(7, 160);
            cbMIDISN76489.Margin = new Padding(4);
            cbMIDISN76489.Name = "cbMIDISN76489";
            cbMIDISN76489.Size = new Size(71, 19);
            cbMIDISN76489.TabIndex = 21;
            cbMIDISN76489.Text = "SN76489";
            cbMIDISN76489.UseVisualStyleBackColor = true;
            // 
            // cbMIDIYM2151
            // 
            cbMIDIYM2151.AutoSize = true;
            cbMIDIYM2151.Location = new Point(7, 50);
            cbMIDIYM2151.Margin = new Padding(4);
            cbMIDIYM2151.Name = "cbMIDIYM2151";
            cbMIDIYM2151.Size = new Size(68, 19);
            cbMIDIYM2151.TabIndex = 21;
            cbMIDIYM2151.Text = "YM2151";
            cbMIDIYM2151.UseVisualStyleBackColor = true;
            // 
            // cbMIDIYM2610BSec
            // 
            cbMIDIYM2610BSec.AutoSize = true;
            cbMIDIYM2610BSec.Enabled = false;
            cbMIDIYM2610BSec.Location = new Point(98, 132);
            cbMIDIYM2610BSec.Margin = new Padding(4);
            cbMIDIYM2610BSec.Name = "cbMIDIYM2610BSec";
            cbMIDIYM2610BSec.Size = new Size(101, 19);
            cbMIDIYM2610BSec.TabIndex = 21;
            cbMIDIYM2610BSec.Text = "YM2610B(Sec)";
            cbMIDIYM2610BSec.UseVisualStyleBackColor = true;
            // 
            // cbMIDIYM2151Sec
            // 
            cbMIDIYM2151Sec.AutoSize = true;
            cbMIDIYM2151Sec.Enabled = false;
            cbMIDIYM2151Sec.Location = new Point(98, 50);
            cbMIDIYM2151Sec.Margin = new Padding(4);
            cbMIDIYM2151Sec.Name = "cbMIDIYM2151Sec";
            cbMIDIYM2151Sec.Size = new Size(94, 19);
            cbMIDIYM2151Sec.TabIndex = 21;
            cbMIDIYM2151Sec.Text = "YM2151(Sec)";
            cbMIDIYM2151Sec.UseVisualStyleBackColor = true;
            // 
            // cbMIDIYM2610B
            // 
            cbMIDIYM2610B.AutoSize = true;
            cbMIDIYM2610B.Enabled = false;
            cbMIDIYM2610B.Location = new Point(7, 132);
            cbMIDIYM2610B.Margin = new Padding(4);
            cbMIDIYM2610B.Name = "cbMIDIYM2610B";
            cbMIDIYM2610B.Size = new Size(75, 19);
            cbMIDIYM2610B.TabIndex = 21;
            cbMIDIYM2610B.Text = "YM2610B";
            cbMIDIYM2610B.UseVisualStyleBackColor = true;
            // 
            // cbMIDIYM2203
            // 
            cbMIDIYM2203.AutoSize = true;
            cbMIDIYM2203.Enabled = false;
            cbMIDIYM2203.Location = new Point(7, 78);
            cbMIDIYM2203.Margin = new Padding(4);
            cbMIDIYM2203.Name = "cbMIDIYM2203";
            cbMIDIYM2203.Size = new Size(68, 19);
            cbMIDIYM2203.TabIndex = 21;
            cbMIDIYM2203.Text = "YM2203";
            cbMIDIYM2203.UseVisualStyleBackColor = true;
            // 
            // cbMIDIYM2608Sec
            // 
            cbMIDIYM2608Sec.AutoSize = true;
            cbMIDIYM2608Sec.Enabled = false;
            cbMIDIYM2608Sec.Location = new Point(98, 105);
            cbMIDIYM2608Sec.Margin = new Padding(4);
            cbMIDIYM2608Sec.Name = "cbMIDIYM2608Sec";
            cbMIDIYM2608Sec.Size = new Size(94, 19);
            cbMIDIYM2608Sec.TabIndex = 21;
            cbMIDIYM2608Sec.Text = "YM2608(Sec)";
            cbMIDIYM2608Sec.UseVisualStyleBackColor = true;
            // 
            // cbMIDIYM2203Sec
            // 
            cbMIDIYM2203Sec.AutoSize = true;
            cbMIDIYM2203Sec.Enabled = false;
            cbMIDIYM2203Sec.Location = new Point(98, 78);
            cbMIDIYM2203Sec.Margin = new Padding(4);
            cbMIDIYM2203Sec.Name = "cbMIDIYM2203Sec";
            cbMIDIYM2203Sec.Size = new Size(94, 19);
            cbMIDIYM2203Sec.TabIndex = 21;
            cbMIDIYM2203Sec.Text = "YM2203(Sec)";
            cbMIDIYM2203Sec.UseVisualStyleBackColor = true;
            // 
            // cbMIDIYM2608
            // 
            cbMIDIYM2608.AutoSize = true;
            cbMIDIYM2608.Enabled = false;
            cbMIDIYM2608.Location = new Point(7, 105);
            cbMIDIYM2608.Margin = new Padding(4);
            cbMIDIYM2608.Name = "cbMIDIYM2608";
            cbMIDIYM2608.Size = new Size(68, 19);
            cbMIDIYM2608.TabIndex = 21;
            cbMIDIYM2608.Text = "YM2608";
            cbMIDIYM2608.UseVisualStyleBackColor = true;
            // 
            // cbMIDIPlayless
            // 
            cbMIDIPlayless.AutoSize = true;
            cbMIDIPlayless.Enabled = false;
            cbMIDIPlayless.Location = new Point(24, 28);
            cbMIDIPlayless.Margin = new Padding(4);
            cbMIDIPlayless.Name = "cbMIDIPlayless";
            cbMIDIPlayless.Size = new Size(142, 19);
            cbMIDIPlayless.TabIndex = 20;
            cbMIDIPlayless.Text = "演奏を行わずに出力する";
            cbMIDIPlayless.UseVisualStyleBackColor = true;
            // 
            // btnMIDIOutputPath
            // 
            btnMIDIOutputPath.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnMIDIOutputPath.Location = new Point(468, 108);
            btnMIDIOutputPath.Margin = new Padding(4);
            btnMIDIOutputPath.Name = "btnMIDIOutputPath";
            btnMIDIOutputPath.Size = new Size(27, 29);
            btnMIDIOutputPath.TabIndex = 19;
            btnMIDIOutputPath.Text = "...";
            btnMIDIOutputPath.UseVisualStyleBackColor = true;
            btnMIDIOutputPath.Click += btnMIDIOutputPath_Click;
            // 
            // lblOutputPath
            // 
            lblOutputPath.AutoSize = true;
            lblOutputPath.Location = new Point(22, 114);
            lblOutputPath.Margin = new Padding(4, 0, 4, 0);
            lblOutputPath.Name = "lblOutputPath";
            lblOutputPath.Size = new Size(55, 15);
            lblOutputPath.TabIndex = 17;
            lblOutputPath.Text = "出力Path";
            // 
            // tbMIDIOutputPath
            // 
            tbMIDIOutputPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tbMIDIOutputPath.Location = new Point(90, 110);
            tbMIDIOutputPath.Margin = new Padding(4);
            tbMIDIOutputPath.Name = "tbMIDIOutputPath";
            tbMIDIOutputPath.Size = new Size(370, 23);
            tbMIDIOutputPath.TabIndex = 18;
            // 
            // tpMIDIKBD
            // 
            tpMIDIKBD.BorderStyle = BorderStyle.FixedSingle;
            tpMIDIKBD.Controls.Add(cbMIDIKbdAlwaysTop);
            tpMIDIKBD.Controls.Add(cbUseMIDIKeyboard);
            tpMIDIKBD.Controls.Add(gbMIDIKeyboard);
            tpMIDIKBD.Location = new Point(4, 44);
            tpMIDIKBD.Margin = new Padding(4);
            tpMIDIKBD.Name = "tpMIDIKBD";
            tpMIDIKBD.Size = new Size(518, 510);
            tpMIDIKBD.TabIndex = 5;
            tpMIDIKBD.Text = "MIDI鍵盤";
            tpMIDIKBD.UseVisualStyleBackColor = true;
            // 
            // cbMIDIKbdAlwaysTop
            // 
            cbMIDIKbdAlwaysTop.AutoSize = true;
            cbMIDIKbdAlwaysTop.Location = new Point(14, 80);
            cbMIDIKbdAlwaysTop.Margin = new Padding(4);
            cbMIDIKbdAlwaysTop.Name = "cbMIDIKbdAlwaysTop";
            cbMIDIKbdAlwaysTop.Size = new Size(156, 19);
            cbMIDIKbdAlwaysTop.TabIndex = 2;
            cbMIDIKbdAlwaysTop.Text = "鍵盤を常に手前に表示する";
            cbMIDIKbdAlwaysTop.UseVisualStyleBackColor = true;
            // 
            // cbUseMIDIKeyboard
            // 
            cbUseMIDIKeyboard.AutoSize = true;
            cbUseMIDIKeyboard.Location = new Point(13, 5);
            cbUseMIDIKeyboard.Margin = new Padding(4);
            cbUseMIDIKeyboard.Name = "cbUseMIDIKeyboard";
            cbUseMIDIKeyboard.Size = new Size(123, 19);
            cbUseMIDIKeyboard.TabIndex = 1;
            cbUseMIDIKeyboard.Text = "MIDIキーボードを使う";
            cbUseMIDIKeyboard.UseVisualStyleBackColor = true;
            cbUseMIDIKeyboard.CheckedChanged += cbUseMIDIKeyboard_CheckedChanged;
            // 
            // gbMIDIKeyboard
            // 
            gbMIDIKeyboard.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            gbMIDIKeyboard.Controls.Add(pictureBox8);
            gbMIDIKeyboard.Controls.Add(pictureBox7);
            gbMIDIKeyboard.Controls.Add(pictureBox6);
            gbMIDIKeyboard.Controls.Add(pictureBox5);
            gbMIDIKeyboard.Controls.Add(pictureBox4);
            gbMIDIKeyboard.Controls.Add(pictureBox3);
            gbMIDIKeyboard.Controls.Add(pictureBox2);
            gbMIDIKeyboard.Controls.Add(pictureBox1);
            gbMIDIKeyboard.Controls.Add(tbCCFadeout);
            gbMIDIKeyboard.Controls.Add(tbCCPause);
            gbMIDIKeyboard.Controls.Add(tbCCSlow);
            gbMIDIKeyboard.Controls.Add(tbCCPrevious);
            gbMIDIKeyboard.Controls.Add(tbCCNext);
            gbMIDIKeyboard.Controls.Add(tbCCFast);
            gbMIDIKeyboard.Controls.Add(tbCCStop);
            gbMIDIKeyboard.Controls.Add(tbCCPlay);
            gbMIDIKeyboard.Controls.Add(tbCCCopyLog);
            gbMIDIKeyboard.Controls.Add(label17);
            gbMIDIKeyboard.Controls.Add(tbCCDelLog);
            gbMIDIKeyboard.Controls.Add(label15);
            gbMIDIKeyboard.Controls.Add(tbCCChCopy);
            gbMIDIKeyboard.Controls.Add(label8);
            gbMIDIKeyboard.Controls.Add(label9);
            gbMIDIKeyboard.Controls.Add(gbUseChannel);
            gbMIDIKeyboard.Controls.Add(cmbMIDIIN);
            gbMIDIKeyboard.Controls.Add(label5);
            gbMIDIKeyboard.Enabled = false;
            gbMIDIKeyboard.Location = new Point(4, 8);
            gbMIDIKeyboard.Margin = new Padding(4);
            gbMIDIKeyboard.Name = "gbMIDIKeyboard";
            gbMIDIKeyboard.Padding = new Padding(4);
            gbMIDIKeyboard.Size = new Size(507, 185);
            gbMIDIKeyboard.TabIndex = 0;
            gbMIDIKeyboard.TabStop = false;
            // 
            // pictureBox8
            // 
            pictureBox8.Image = Resources.ccNext;
            pictureBox8.Location = new Point(433, 321);
            pictureBox8.Margin = new Padding(4);
            pictureBox8.Name = "pictureBox8";
            pictureBox8.Size = new Size(19, 20);
            pictureBox8.TabIndex = 4;
            pictureBox8.TabStop = false;
            pictureBox8.Visible = false;
            // 
            // pictureBox7
            // 
            pictureBox7.Image = Resources.ccFast;
            pictureBox7.Location = new Point(304, 321);
            pictureBox7.Margin = new Padding(4);
            pictureBox7.Name = "pictureBox7";
            pictureBox7.Size = new Size(19, 20);
            pictureBox7.TabIndex = 4;
            pictureBox7.TabStop = false;
            pictureBox7.Visible = false;
            // 
            // pictureBox6
            // 
            pictureBox6.Image = Resources.ccPlay;
            pictureBox6.Location = new Point(177, 322);
            pictureBox6.Margin = new Padding(4);
            pictureBox6.Name = "pictureBox6";
            pictureBox6.Size = new Size(19, 20);
            pictureBox6.TabIndex = 4;
            pictureBox6.TabStop = false;
            pictureBox6.Visible = false;
            // 
            // pictureBox5
            // 
            pictureBox5.Image = Resources.ccSlow;
            pictureBox5.Location = new Point(49, 322);
            pictureBox5.Margin = new Padding(4);
            pictureBox5.Name = "pictureBox5";
            pictureBox5.Size = new Size(19, 20);
            pictureBox5.TabIndex = 4;
            pictureBox5.TabStop = false;
            pictureBox5.Visible = false;
            // 
            // pictureBox4
            // 
            pictureBox4.Image = Resources.ccStop;
            pictureBox4.Location = new Point(49, 292);
            pictureBox4.Margin = new Padding(4);
            pictureBox4.Name = "pictureBox4";
            pictureBox4.Size = new Size(19, 20);
            pictureBox4.TabIndex = 4;
            pictureBox4.TabStop = false;
            pictureBox4.Visible = false;
            // 
            // pictureBox3
            // 
            pictureBox3.Image = Resources.ccPause;
            pictureBox3.Location = new Point(177, 292);
            pictureBox3.Margin = new Padding(4);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(19, 20);
            pictureBox3.TabIndex = 4;
            pictureBox3.TabStop = false;
            pictureBox3.Visible = false;
            // 
            // pictureBox2
            // 
            pictureBox2.Image = Resources.ccPrevious;
            pictureBox2.Location = new Point(433, 292);
            pictureBox2.Margin = new Padding(4);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(19, 20);
            pictureBox2.TabIndex = 4;
            pictureBox2.TabStop = false;
            pictureBox2.Visible = false;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Resources.ccFadeout;
            pictureBox1.Location = new Point(304, 292);
            pictureBox1.Margin = new Padding(4);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(19, 20);
            pictureBox1.TabIndex = 4;
            pictureBox1.TabStop = false;
            pictureBox1.Visible = false;
            // 
            // tbCCFadeout
            // 
            tbCCFadeout.Location = new Point(262, 290);
            tbCCFadeout.Margin = new Padding(4);
            tbCCFadeout.MaxLength = 3;
            tbCCFadeout.Name = "tbCCFadeout";
            tbCCFadeout.Size = new Size(34, 23);
            tbCCFadeout.TabIndex = 12;
            tbCCFadeout.Visible = false;
            // 
            // tbCCPause
            // 
            tbCCPause.Location = new Point(135, 290);
            tbCCPause.Margin = new Padding(4);
            tbCCPause.MaxLength = 3;
            tbCCPause.Name = "tbCCPause";
            tbCCPause.Size = new Size(34, 23);
            tbCCPause.TabIndex = 11;
            tbCCPause.Visible = false;
            // 
            // tbCCSlow
            // 
            tbCCSlow.Location = new Point(7, 321);
            tbCCSlow.Margin = new Padding(4);
            tbCCSlow.MaxLength = 3;
            tbCCSlow.Name = "tbCCSlow";
            tbCCSlow.Size = new Size(34, 23);
            tbCCSlow.TabIndex = 14;
            tbCCSlow.Visible = false;
            // 
            // tbCCPrevious
            // 
            tbCCPrevious.Location = new Point(391, 290);
            tbCCPrevious.Margin = new Padding(4);
            tbCCPrevious.MaxLength = 3;
            tbCCPrevious.Name = "tbCCPrevious";
            tbCCPrevious.Size = new Size(34, 23);
            tbCCPrevious.TabIndex = 13;
            tbCCPrevious.Visible = false;
            // 
            // tbCCNext
            // 
            tbCCNext.Location = new Point(391, 321);
            tbCCNext.Margin = new Padding(4);
            tbCCNext.MaxLength = 3;
            tbCCNext.Name = "tbCCNext";
            tbCCNext.Size = new Size(34, 23);
            tbCCNext.TabIndex = 17;
            tbCCNext.Visible = false;
            // 
            // tbCCFast
            // 
            tbCCFast.Location = new Point(262, 321);
            tbCCFast.Margin = new Padding(4);
            tbCCFast.MaxLength = 3;
            tbCCFast.Name = "tbCCFast";
            tbCCFast.Size = new Size(34, 23);
            tbCCFast.TabIndex = 16;
            tbCCFast.Visible = false;
            // 
            // tbCCStop
            // 
            tbCCStop.Location = new Point(7, 290);
            tbCCStop.Margin = new Padding(4);
            tbCCStop.MaxLength = 3;
            tbCCStop.Name = "tbCCStop";
            tbCCStop.Size = new Size(34, 23);
            tbCCStop.TabIndex = 10;
            tbCCStop.Visible = false;
            // 
            // tbCCPlay
            // 
            tbCCPlay.Location = new Point(135, 321);
            tbCCPlay.Margin = new Padding(4);
            tbCCPlay.MaxLength = 3;
            tbCCPlay.Name = "tbCCPlay";
            tbCCPlay.Size = new Size(34, 23);
            tbCCPlay.TabIndex = 15;
            tbCCPlay.Visible = false;
            // 
            // tbCCCopyLog
            // 
            tbCCCopyLog.Location = new Point(7, 259);
            tbCCCopyLog.Margin = new Padding(4);
            tbCCCopyLog.MaxLength = 3;
            tbCCCopyLog.Name = "tbCCCopyLog";
            tbCCCopyLog.Size = new Size(34, 23);
            tbCCCopyLog.TabIndex = 8;
            tbCCCopyLog.Visible = false;
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Location = new Point(49, 262);
            label17.Margin = new Padding(4, 0, 4, 0);
            label17.Name = "label17";
            label17.Size = new Size(233, 15);
            label17.TabIndex = 9;
            label17.Text = "MONOモード時、選択ログをクリップボードに設定";
            label17.Visible = false;
            // 
            // tbCCDelLog
            // 
            tbCCDelLog.Location = new Point(7, 228);
            tbCCDelLog.Margin = new Padding(4);
            tbCCDelLog.MaxLength = 3;
            tbCCDelLog.Name = "tbCCDelLog";
            tbCCDelLog.Size = new Size(34, 23);
            tbCCDelLog.TabIndex = 6;
            tbCCDelLog.Visible = false;
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Location = new Point(49, 231);
            label15.Margin = new Padding(4, 0, 4, 0);
            label15.Name = "label15";
            label15.Size = new Size(120, 15);
            label15.TabIndex = 7;
            label15.Text = "直近のログをひとつ削除";
            label15.Visible = false;
            // 
            // tbCCChCopy
            // 
            tbCCChCopy.Location = new Point(7, 196);
            tbCCChCopy.Margin = new Padding(4);
            tbCCChCopy.MaxLength = 3;
            tbCCChCopy.Name = "tbCCChCopy";
            tbCCChCopy.Size = new Size(34, 23);
            tbCCChCopy.TabIndex = 4;
            tbCCChCopy.Visible = false;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(7, 178);
            label8.Margin = new Padding(4, 0, 4, 0);
            label8.Name = "label8";
            label8.Size = new Size(162, 15);
            label8.TabIndex = 3;
            label8.Text = "CC(Control Change)による操作";
            label8.Visible = false;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(49, 200);
            label9.Margin = new Padding(4, 0, 4, 0);
            label9.Name = "label9";
            label9.Size = new Size(261, 15);
            label9.TabIndex = 5;
            label9.Text = "1Chの音色を全てのチャンネルにコピー(選択状況無視)";
            label9.Visible = false;
            // 
            // gbUseChannel
            // 
            gbUseChannel.Controls.Add(rbMONO);
            gbUseChannel.Controls.Add(rbPOLY);
            gbUseChannel.Controls.Add(groupBox7);
            gbUseChannel.Controls.Add(groupBox2);
            gbUseChannel.Location = new Point(7, 55);
            gbUseChannel.Margin = new Padding(4);
            gbUseChannel.Name = "gbUseChannel";
            gbUseChannel.Padding = new Padding(4);
            gbUseChannel.Size = new Size(496, 108);
            gbUseChannel.TabIndex = 2;
            gbUseChannel.TabStop = false;
            gbUseChannel.Text = "use channel";
            gbUseChannel.Visible = false;
            // 
            // rbMONO
            // 
            rbMONO.AutoSize = true;
            rbMONO.Checked = true;
            rbMONO.Location = new Point(14, 21);
            rbMONO.Margin = new Padding(4);
            rbMONO.Name = "rbMONO";
            rbMONO.Size = new Size(63, 19);
            rbMONO.TabIndex = 1;
            rbMONO.TabStop = true;
            rbMONO.Text = "MONO";
            rbMONO.UseVisualStyleBackColor = true;
            // 
            // rbPOLY
            // 
            rbPOLY.AutoSize = true;
            rbPOLY.Location = new Point(251, 21);
            rbPOLY.Margin = new Padding(4);
            rbPOLY.Name = "rbPOLY";
            rbPOLY.Size = new Size(54, 19);
            rbPOLY.TabIndex = 3;
            rbPOLY.Text = "POLY";
            rbPOLY.UseVisualStyleBackColor = true;
            // 
            // groupBox7
            // 
            groupBox7.Controls.Add(rbFM6);
            groupBox7.Controls.Add(rbFM3);
            groupBox7.Controls.Add(rbFM5);
            groupBox7.Controls.Add(rbFM2);
            groupBox7.Controls.Add(rbFM4);
            groupBox7.Controls.Add(rbFM1);
            groupBox7.Location = new Point(7, 22);
            groupBox7.Margin = new Padding(4);
            groupBox7.Name = "groupBox7";
            groupBox7.Padding = new Padding(4);
            groupBox7.Size = new Size(230, 78);
            groupBox7.TabIndex = 0;
            groupBox7.TabStop = false;
            // 
            // rbFM6
            // 
            rbFM6.AutoSize = true;
            rbFM6.Location = new Point(126, 50);
            rbFM6.Margin = new Padding(4);
            rbFM6.Name = "rbFM6";
            rbFM6.Size = new Size(48, 19);
            rbFM6.TabIndex = 5;
            rbFM6.Text = "FM6";
            rbFM6.UseVisualStyleBackColor = true;
            // 
            // rbFM3
            // 
            rbFM3.AutoSize = true;
            rbFM3.Location = new Point(126, 22);
            rbFM3.Margin = new Padding(4);
            rbFM3.Name = "rbFM3";
            rbFM3.Size = new Size(48, 19);
            rbFM3.TabIndex = 2;
            rbFM3.Text = "FM3";
            rbFM3.UseVisualStyleBackColor = true;
            // 
            // rbFM5
            // 
            rbFM5.AutoSize = true;
            rbFM5.Location = new Point(66, 50);
            rbFM5.Margin = new Padding(4);
            rbFM5.Name = "rbFM5";
            rbFM5.Size = new Size(48, 19);
            rbFM5.TabIndex = 4;
            rbFM5.Text = "FM5";
            rbFM5.UseVisualStyleBackColor = true;
            // 
            // rbFM2
            // 
            rbFM2.AutoSize = true;
            rbFM2.Location = new Point(66, 22);
            rbFM2.Margin = new Padding(4);
            rbFM2.Name = "rbFM2";
            rbFM2.Size = new Size(48, 19);
            rbFM2.TabIndex = 1;
            rbFM2.Text = "FM2";
            rbFM2.UseVisualStyleBackColor = true;
            // 
            // rbFM4
            // 
            rbFM4.AutoSize = true;
            rbFM4.Location = new Point(7, 50);
            rbFM4.Margin = new Padding(4);
            rbFM4.Name = "rbFM4";
            rbFM4.Size = new Size(48, 19);
            rbFM4.TabIndex = 3;
            rbFM4.Text = "FM4";
            rbFM4.UseVisualStyleBackColor = true;
            // 
            // rbFM1
            // 
            rbFM1.AutoSize = true;
            rbFM1.Checked = true;
            rbFM1.Location = new Point(7, 22);
            rbFM1.Margin = new Padding(4);
            rbFM1.Name = "rbFM1";
            rbFM1.Size = new Size(48, 19);
            rbFM1.TabIndex = 0;
            rbFM1.TabStop = true;
            rbFM1.Text = "FM1";
            rbFM1.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(cbFM1);
            groupBox2.Controls.Add(cbFM6);
            groupBox2.Controls.Add(cbFM2);
            groupBox2.Controls.Add(cbFM5);
            groupBox2.Controls.Add(cbFM3);
            groupBox2.Controls.Add(cbFM4);
            groupBox2.Location = new Point(244, 22);
            groupBox2.Margin = new Padding(4);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new Padding(4);
            groupBox2.Size = new Size(245, 78);
            groupBox2.TabIndex = 2;
            groupBox2.TabStop = false;
            // 
            // cbFM1
            // 
            cbFM1.AutoSize = true;
            cbFM1.Checked = true;
            cbFM1.CheckState = CheckState.Checked;
            cbFM1.Location = new Point(7, 22);
            cbFM1.Margin = new Padding(4);
            cbFM1.Name = "cbFM1";
            cbFM1.Size = new Size(49, 19);
            cbFM1.TabIndex = 0;
            cbFM1.Text = "FM1";
            cbFM1.UseVisualStyleBackColor = true;
            // 
            // cbFM6
            // 
            cbFM6.AutoSize = true;
            cbFM6.Checked = true;
            cbFM6.CheckState = CheckState.Checked;
            cbFM6.Location = new Point(128, 50);
            cbFM6.Margin = new Padding(4);
            cbFM6.Name = "cbFM6";
            cbFM6.Size = new Size(49, 19);
            cbFM6.TabIndex = 5;
            cbFM6.Text = "FM6";
            cbFM6.UseVisualStyleBackColor = true;
            // 
            // cbFM2
            // 
            cbFM2.AutoSize = true;
            cbFM2.Checked = true;
            cbFM2.CheckState = CheckState.Checked;
            cbFM2.Location = new Point(68, 22);
            cbFM2.Margin = new Padding(4);
            cbFM2.Name = "cbFM2";
            cbFM2.Size = new Size(49, 19);
            cbFM2.TabIndex = 1;
            cbFM2.Text = "FM2";
            cbFM2.UseVisualStyleBackColor = true;
            // 
            // cbFM5
            // 
            cbFM5.AutoSize = true;
            cbFM5.Checked = true;
            cbFM5.CheckState = CheckState.Checked;
            cbFM5.Location = new Point(68, 50);
            cbFM5.Margin = new Padding(4);
            cbFM5.Name = "cbFM5";
            cbFM5.Size = new Size(49, 19);
            cbFM5.TabIndex = 4;
            cbFM5.Text = "FM5";
            cbFM5.UseVisualStyleBackColor = true;
            // 
            // cbFM3
            // 
            cbFM3.AutoSize = true;
            cbFM3.Checked = true;
            cbFM3.CheckState = CheckState.Checked;
            cbFM3.Location = new Point(128, 22);
            cbFM3.Margin = new Padding(4);
            cbFM3.Name = "cbFM3";
            cbFM3.Size = new Size(49, 19);
            cbFM3.TabIndex = 2;
            cbFM3.Text = "FM3";
            cbFM3.UseVisualStyleBackColor = true;
            // 
            // cbFM4
            // 
            cbFM4.AutoSize = true;
            cbFM4.Checked = true;
            cbFM4.CheckState = CheckState.Checked;
            cbFM4.Location = new Point(7, 50);
            cbFM4.Margin = new Padding(4);
            cbFM4.Name = "cbFM4";
            cbFM4.Size = new Size(49, 19);
            cbFM4.TabIndex = 3;
            cbFM4.Text = "FM4";
            cbFM4.UseVisualStyleBackColor = true;
            // 
            // cmbMIDIIN
            // 
            cmbMIDIIN.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cmbMIDIIN.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbMIDIIN.FormattingEnabled = true;
            cmbMIDIIN.Location = new Point(84, 22);
            cmbMIDIIN.Margin = new Padding(4);
            cmbMIDIIN.Name = "cmbMIDIIN";
            cmbMIDIIN.Size = new Size(416, 23);
            cmbMIDIIN.TabIndex = 1;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(7, 26);
            label5.Margin = new Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new Size(47, 15);
            label5.TabIndex = 0;
            label5.Text = "MIDI IN";
            // 
            // tpKeyBoard
            // 
            tpKeyBoard.BorderStyle = BorderStyle.FixedSingle;
            tpKeyBoard.Controls.Add(btnInitializeShortCutKey);
            tpKeyBoard.Controls.Add(lblSKKey);
            tpKeyBoard.Controls.Add(dgvShortCutKey);
            tpKeyBoard.Location = new Point(4, 44);
            tpKeyBoard.Margin = new Padding(4);
            tpKeyBoard.Name = "tpKeyBoard";
            tpKeyBoard.Size = new Size(518, 510);
            tpKeyBoard.TabIndex = 13;
            tpKeyBoard.Text = "ショートカットキー";
            tpKeyBoard.UseVisualStyleBackColor = true;
            // 
            // btnInitializeShortCutKey
            // 
            btnInitializeShortCutKey.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnInitializeShortCutKey.Location = new Point(390, 461);
            btnInitializeShortCutKey.Margin = new Padding(4);
            btnInitializeShortCutKey.Name = "btnInitializeShortCutKey";
            btnInitializeShortCutKey.Size = new Size(121, 29);
            btnInitializeShortCutKey.TabIndex = 31;
            btnInitializeShortCutKey.Text = "初期状態に戻す";
            btnInitializeShortCutKey.UseVisualStyleBackColor = true;
            btnInitializeShortCutKey.Click += btnInitializeShortCutKey_Click;
            // 
            // lblSKKey
            // 
            lblSKKey.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblSKKey.AutoSize = true;
            lblSKKey.Location = new Point(49, 468);
            lblSKKey.Margin = new Padding(4, 0, 4, 0);
            lblSKKey.Name = "lblSKKey";
            lblSKKey.Size = new Size(0, 15);
            lblSKKey.TabIndex = 30;
            // 
            // dgvShortCutKey
            // 
            dgvShortCutKey.AllowUserToAddRows = false;
            dgvShortCutKey.AllowUserToDeleteRows = false;
            dgvShortCutKey.AllowUserToOrderColumns = true;
            dgvShortCutKey.AllowUserToResizeRows = false;
            dgvShortCutKey.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvShortCutKey.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvShortCutKey.Columns.AddRange(new DataGridViewColumn[] { clmNumber, clmFunc, clmShift, clmCtrl, clmAlt, clmKey, clmSet, clmClr, clmKBDSpacer });
            dgvShortCutKey.Location = new Point(4, 4);
            dgvShortCutKey.Margin = new Padding(4);
            dgvShortCutKey.MultiSelect = false;
            dgvShortCutKey.Name = "dgvShortCutKey";
            dgvShortCutKey.RowHeadersVisible = false;
            dgvShortCutKey.RowTemplate.Height = 21;
            dgvShortCutKey.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvShortCutKey.Size = new Size(507, 441);
            dgvShortCutKey.TabIndex = 29;
            dgvShortCutKey.CellContentClick += dgvShortCutKey_CellContentClick;
            // 
            // clmNumber
            // 
            clmNumber.HeaderText = "Number";
            clmNumber.Name = "clmNumber";
            clmNumber.Visible = false;
            // 
            // clmFunc
            // 
            clmFunc.HeaderText = "機能";
            clmFunc.Name = "clmFunc";
            clmFunc.ReadOnly = true;
            clmFunc.Width = 150;
            // 
            // clmShift
            // 
            clmShift.HeaderText = "Shift";
            clmShift.Name = "clmShift";
            clmShift.Resizable = DataGridViewTriState.False;
            clmShift.Width = 40;
            // 
            // clmCtrl
            // 
            clmCtrl.HeaderText = "Ctrl";
            clmCtrl.Name = "clmCtrl";
            clmCtrl.Resizable = DataGridViewTriState.False;
            clmCtrl.Width = 40;
            // 
            // clmAlt
            // 
            clmAlt.HeaderText = "Alt";
            clmAlt.Name = "clmAlt";
            clmAlt.Resizable = DataGridViewTriState.False;
            clmAlt.Width = 40;
            // 
            // clmKey
            // 
            clmKey.HeaderText = "Key";
            clmKey.Name = "clmKey";
            clmKey.ReadOnly = true;
            clmKey.Width = 50;
            // 
            // clmSet
            // 
            clmSet.HeaderText = "Set";
            clmSet.Name = "clmSet";
            clmSet.Resizable = DataGridViewTriState.False;
            clmSet.Text = "Set";
            clmSet.Width = 40;
            // 
            // clmClr
            // 
            clmClr.HeaderText = "Clr";
            clmClr.Name = "clmClr";
            clmClr.Resizable = DataGridViewTriState.False;
            clmClr.Text = "Clr";
            clmClr.Width = 40;
            // 
            // clmKBDSpacer
            // 
            clmKBDSpacer.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            clmKBDSpacer.HeaderText = "";
            clmKBDSpacer.Name = "clmKBDSpacer";
            clmKBDSpacer.ReadOnly = true;
            clmKBDSpacer.Resizable = DataGridViewTriState.True;
            clmKBDSpacer.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // tpBalance
            // 
            tpBalance.BorderStyle = BorderStyle.FixedSingle;
            tpBalance.Controls.Add(groupBox25);
            tpBalance.Controls.Add(cbAutoBalanceUseThis);
            tpBalance.Controls.Add(groupBox18);
            tpBalance.Location = new Point(4, 44);
            tpBalance.Margin = new Padding(4);
            tpBalance.Name = "tpBalance";
            tpBalance.Size = new Size(518, 510);
            tpBalance.TabIndex = 12;
            tpBalance.Text = "ミキサーバランス";
            tpBalance.UseVisualStyleBackColor = true;
            // 
            // groupBox25
            // 
            groupBox25.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox25.Controls.Add(rbAutoBalanceNotSamePositionAsSongData);
            groupBox25.Controls.Add(rbAutoBalanceSamePositionAsSongData);
            groupBox25.Location = new Point(8, 409);
            groupBox25.Margin = new Padding(4);
            groupBox25.Name = "groupBox25";
            groupBox25.Padding = new Padding(4);
            groupBox25.Size = new Size(503, 51);
            groupBox25.TabIndex = 1;
            groupBox25.TabStop = false;
            groupBox25.Text = "ソングミキサーバランス参照フォルダー";
            // 
            // rbAutoBalanceNotSamePositionAsSongData
            // 
            rbAutoBalanceNotSamePositionAsSongData.AutoSize = true;
            rbAutoBalanceNotSamePositionAsSongData.Checked = true;
            rbAutoBalanceNotSamePositionAsSongData.Location = new Point(7, 22);
            rbAutoBalanceNotSamePositionAsSongData.Margin = new Padding(4);
            rbAutoBalanceNotSamePositionAsSongData.Name = "rbAutoBalanceNotSamePositionAsSongData";
            rbAutoBalanceNotSamePositionAsSongData.Size = new Size(113, 19);
            rbAutoBalanceNotSamePositionAsSongData.TabIndex = 0;
            rbAutoBalanceNotSamePositionAsSongData.TabStop = true;
            rbAutoBalanceNotSamePositionAsSongData.Text = "設定ファイルと同じ";
            rbAutoBalanceNotSamePositionAsSongData.UseVisualStyleBackColor = true;
            // 
            // rbAutoBalanceSamePositionAsSongData
            // 
            rbAutoBalanceSamePositionAsSongData.AutoSize = true;
            rbAutoBalanceSamePositionAsSongData.Location = new Point(142, 22);
            rbAutoBalanceSamePositionAsSongData.Margin = new Padding(4);
            rbAutoBalanceSamePositionAsSongData.Name = "rbAutoBalanceSamePositionAsSongData";
            rbAutoBalanceSamePositionAsSongData.Size = new Size(93, 19);
            rbAutoBalanceSamePositionAsSongData.TabIndex = 0;
            rbAutoBalanceSamePositionAsSongData.Text = "曲データと同じ";
            rbAutoBalanceSamePositionAsSongData.UseVisualStyleBackColor = true;
            // 
            // cbAutoBalanceUseThis
            // 
            cbAutoBalanceUseThis.AutoSize = true;
            cbAutoBalanceUseThis.Checked = true;
            cbAutoBalanceUseThis.CheckState = CheckState.Checked;
            cbAutoBalanceUseThis.Location = new Point(15, 4);
            cbAutoBalanceUseThis.Margin = new Padding(4);
            cbAutoBalanceUseThis.Name = "cbAutoBalanceUseThis";
            cbAutoBalanceUseThis.Size = new Size(220, 19);
            cbAutoBalanceUseThis.TabIndex = 1;
            cbAutoBalanceUseThis.Text = "ミキサーバランス自動設定機能を使用する";
            cbAutoBalanceUseThis.UseVisualStyleBackColor = true;
            // 
            // groupBox18
            // 
            groupBox18.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox18.Controls.Add(groupBox24);
            groupBox18.Controls.Add(groupBox23);
            groupBox18.Location = new Point(8, 4);
            groupBox18.Margin = new Padding(4);
            groupBox18.Name = "groupBox18";
            groupBox18.Padding = new Padding(4);
            groupBox18.Size = new Size(503, 398);
            groupBox18.TabIndex = 0;
            groupBox18.TabStop = false;
            // 
            // groupBox24
            // 
            groupBox24.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox24.Controls.Add(groupBox21);
            groupBox24.Controls.Add(groupBox22);
            groupBox24.Location = new Point(7, 231);
            groupBox24.Margin = new Padding(4);
            groupBox24.Name = "groupBox24";
            groupBox24.Padding = new Padding(4);
            groupBox24.Size = new Size(489, 159);
            groupBox24.TabIndex = 1;
            groupBox24.TabStop = false;
            groupBox24.Text = "保存";
            // 
            // groupBox21
            // 
            groupBox21.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox21.Controls.Add(rbAutoBalanceNotSaveSongBalance);
            groupBox21.Controls.Add(rbAutoBalanceSaveSongBalance);
            groupBox21.Location = new Point(7, 22);
            groupBox21.Margin = new Padding(4);
            groupBox21.Name = "groupBox21";
            groupBox21.Padding = new Padding(4);
            groupBox21.Size = new Size(475, 78);
            groupBox21.TabIndex = 0;
            groupBox21.TabStop = false;
            groupBox21.Text = "ソングミキサーバランス(曲データ毎)";
            // 
            // rbAutoBalanceNotSaveSongBalance
            // 
            rbAutoBalanceNotSaveSongBalance.AutoSize = true;
            rbAutoBalanceNotSaveSongBalance.Checked = true;
            rbAutoBalanceNotSaveSongBalance.Location = new Point(7, 50);
            rbAutoBalanceNotSaveSongBalance.Margin = new Padding(4);
            rbAutoBalanceNotSaveSongBalance.Name = "rbAutoBalanceNotSaveSongBalance";
            rbAutoBalanceNotSaveSongBalance.Size = new Size(154, 19);
            rbAutoBalanceNotSaveSongBalance.TabIndex = 0;
            rbAutoBalanceNotSaveSongBalance.TabStop = true;
            rbAutoBalanceNotSaveSongBalance.Text = "保存しない(手動保存のみ)";
            rbAutoBalanceNotSaveSongBalance.UseVisualStyleBackColor = true;
            // 
            // rbAutoBalanceSaveSongBalance
            // 
            rbAutoBalanceSaveSongBalance.AutoSize = true;
            rbAutoBalanceSaveSongBalance.Location = new Point(7, 22);
            rbAutoBalanceSaveSongBalance.Margin = new Padding(4);
            rbAutoBalanceSaveSongBalance.Name = "rbAutoBalanceSaveSongBalance";
            rbAutoBalanceSaveSongBalance.Size = new Size(152, 19);
            rbAutoBalanceSaveSongBalance.TabIndex = 0;
            rbAutoBalanceSaveSongBalance.Text = "演奏停止時に自動で保存";
            rbAutoBalanceSaveSongBalance.UseVisualStyleBackColor = true;
            // 
            // groupBox22
            // 
            groupBox22.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox22.Controls.Add(label4);
            groupBox22.Location = new Point(7, 108);
            groupBox22.Margin = new Padding(4);
            groupBox22.Name = "groupBox22";
            groupBox22.Padding = new Padding(4);
            groupBox22.Size = new Size(475, 44);
            groupBox22.TabIndex = 0;
            groupBox22.TabStop = false;
            groupBox22.Text = "ドライバーミキサーバランス(ドライバ毎)";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(7, 19);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(76, 15);
            label4.TabIndex = 1;
            label4.Text = "手動保存のみ";
            // 
            // groupBox23
            // 
            groupBox23.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox23.Controls.Add(groupBox19);
            groupBox23.Controls.Add(groupBox20);
            groupBox23.Location = new Point(7, 28);
            groupBox23.Margin = new Padding(4);
            groupBox23.Name = "groupBox23";
            groupBox23.Padding = new Padding(4);
            groupBox23.Size = new Size(489, 196);
            groupBox23.TabIndex = 1;
            groupBox23.TabStop = false;
            groupBox23.Text = "読み込み";
            // 
            // groupBox19
            // 
            groupBox19.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox19.Controls.Add(rbAutoBalanceNotLoadSongBalance);
            groupBox19.Controls.Add(rbAutoBalanceLoadSongBalance);
            groupBox19.Location = new Point(7, 22);
            groupBox19.Margin = new Padding(4);
            groupBox19.Name = "groupBox19";
            groupBox19.Padding = new Padding(4);
            groupBox19.Size = new Size(475, 79);
            groupBox19.TabIndex = 0;
            groupBox19.TabStop = false;
            groupBox19.Text = "ソングミキサーバランス(曲データ毎)";
            // 
            // rbAutoBalanceNotLoadSongBalance
            // 
            rbAutoBalanceNotLoadSongBalance.AutoSize = true;
            rbAutoBalanceNotLoadSongBalance.Checked = true;
            rbAutoBalanceNotLoadSongBalance.Location = new Point(7, 50);
            rbAutoBalanceNotLoadSongBalance.Margin = new Padding(4);
            rbAutoBalanceNotLoadSongBalance.Name = "rbAutoBalanceNotLoadSongBalance";
            rbAutoBalanceNotLoadSongBalance.Size = new Size(104, 19);
            rbAutoBalanceNotLoadSongBalance.TabIndex = 0;
            rbAutoBalanceNotLoadSongBalance.TabStop = true;
            rbAutoBalanceNotLoadSongBalance.Text = "手動で読み込む";
            rbAutoBalanceNotLoadSongBalance.UseVisualStyleBackColor = true;
            // 
            // rbAutoBalanceLoadSongBalance
            // 
            rbAutoBalanceLoadSongBalance.AutoSize = true;
            rbAutoBalanceLoadSongBalance.Location = new Point(7, 22);
            rbAutoBalanceLoadSongBalance.Margin = new Padding(4);
            rbAutoBalanceLoadSongBalance.Name = "rbAutoBalanceLoadSongBalance";
            rbAutoBalanceLoadSongBalance.Size = new Size(149, 19);
            rbAutoBalanceLoadSongBalance.TabIndex = 0;
            rbAutoBalanceLoadSongBalance.Text = "再生時に自動で読み込む";
            rbAutoBalanceLoadSongBalance.UseVisualStyleBackColor = true;
            // 
            // groupBox20
            // 
            groupBox20.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox20.Controls.Add(rbAutoBalanceNotLoadDriverBalance);
            groupBox20.Controls.Add(rbAutoBalanceLoadDriverBalance);
            groupBox20.Location = new Point(7, 109);
            groupBox20.Margin = new Padding(4);
            groupBox20.Name = "groupBox20";
            groupBox20.Padding = new Padding(4);
            groupBox20.Size = new Size(475, 79);
            groupBox20.TabIndex = 0;
            groupBox20.TabStop = false;
            groupBox20.Text = "ドライバーミキサーバランス(ドライバ毎)";
            // 
            // rbAutoBalanceNotLoadDriverBalance
            // 
            rbAutoBalanceNotLoadDriverBalance.AutoSize = true;
            rbAutoBalanceNotLoadDriverBalance.Location = new Point(7, 50);
            rbAutoBalanceNotLoadDriverBalance.Margin = new Padding(4);
            rbAutoBalanceNotLoadDriverBalance.Name = "rbAutoBalanceNotLoadDriverBalance";
            rbAutoBalanceNotLoadDriverBalance.Size = new Size(104, 19);
            rbAutoBalanceNotLoadDriverBalance.TabIndex = 0;
            rbAutoBalanceNotLoadDriverBalance.Text = "手動で読み込む";
            rbAutoBalanceNotLoadDriverBalance.UseVisualStyleBackColor = true;
            // 
            // rbAutoBalanceLoadDriverBalance
            // 
            rbAutoBalanceLoadDriverBalance.AutoSize = true;
            rbAutoBalanceLoadDriverBalance.Checked = true;
            rbAutoBalanceLoadDriverBalance.Location = new Point(7, 22);
            rbAutoBalanceLoadDriverBalance.Margin = new Padding(4);
            rbAutoBalanceLoadDriverBalance.Name = "rbAutoBalanceLoadDriverBalance";
            rbAutoBalanceLoadDriverBalance.Size = new Size(343, 19);
            rbAutoBalanceLoadDriverBalance.TabIndex = 0;
            rbAutoBalanceLoadDriverBalance.TabStop = true;
            rbAutoBalanceLoadDriverBalance.Text = "再生時に自動で読み込む(曲データ毎のバランスファイルが無い場合)";
            rbAutoBalanceLoadDriverBalance.UseVisualStyleBackColor = true;
            // 
            // tpMMLParameter
            // 
            tpMMLParameter.BorderStyle = BorderStyle.FixedSingle;
            tpMMLParameter.Controls.Add(cbDispInstrumentName);
            tpMMLParameter.Location = new Point(4, 44);
            tpMMLParameter.Margin = new Padding(4);
            tpMMLParameter.Name = "tpMMLParameter";
            tpMMLParameter.Size = new Size(518, 510);
            tpMMLParameter.TabIndex = 16;
            tpMMLParameter.Text = "MMLParameter";
            tpMMLParameter.UseVisualStyleBackColor = true;
            // 
            // cbDispInstrumentName
            // 
            cbDispInstrumentName.AutoSize = true;
            cbDispInstrumentName.Location = new Point(27, 24);
            cbDispInstrumentName.Margin = new Padding(4);
            cbDispInstrumentName.Name = "cbDispInstrumentName";
            cbDispInstrumentName.Size = new Size(140, 19);
            cbDispInstrumentName.TabIndex = 0;
            cbDispInstrumentName.Text = "disp Instrument name";
            cbDispInstrumentName.UseVisualStyleBackColor = true;
            // 
            // tpOther2
            // 
            tpOther2.Controls.Add(cbHilightOn);
            tpOther2.Controls.Add(tbTABWidth);
            tpOther2.Controls.Add(cbDispWarningMessage);
            tpOther2.Controls.Add(tbUseHistoryBackUp);
            tpOther2.Controls.Add(cbUseHistoryBackUp);
            tpOther2.Controls.Add(cbUseMoonDriverDotNET);
            tpOther2.Controls.Add(cbUsePMDDotNET);
            tpOther2.Controls.Add(cbUseMucomDotNET);
            tpOther2.Controls.Add(cbUseScript);
            tpOther2.Controls.Add(cbChangeEnterCode);
            tpOther2.Controls.Add(cbClearHistory);
            tpOther2.Controls.Add(tbOpacity);
            tpOther2.Controls.Add(label56);
            tpOther2.Controls.Add(label52);
            tpOther2.Controls.Add(groupBox29);
            tpOther2.Controls.Add(cbInfiniteOfflineMode);
            tpOther2.Controls.Add(cbUseSIen);
            tpOther2.Controls.Add(cbRequestCacheClear);
            tpOther2.Location = new Point(4, 44);
            tpOther2.Margin = new Padding(4);
            tpOther2.Name = "tpOther2";
            tpOther2.Size = new Size(518, 510);
            tpOther2.TabIndex = 18;
            tpOther2.Text = "Other";
            tpOther2.UseVisualStyleBackColor = true;
            // 
            // tbTABWidth
            // 
            tbTABWidth.Location = new Point(125, 472);
            tbTABWidth.Margin = new Padding(4);
            tbTABWidth.Name = "tbTABWidth";
            tbTABWidth.Size = new Size(61, 23);
            tbTABWidth.TabIndex = 31;
            // 
            // cbDispWarningMessage
            // 
            cbDispWarningMessage.AutoSize = true;
            cbDispWarningMessage.Location = new Point(8, 279);
            cbDispWarningMessage.Margin = new Padding(4);
            cbDispWarningMessage.Name = "cbDispWarningMessage";
            cbDispWarningMessage.Size = new Size(283, 19);
            cbDispWarningMessage.TabIndex = 30;
            cbDispWarningMessage.Text = "コンパイル時の警告メッセージをログウィンドウに表示する";
            cbDispWarningMessage.UseVisualStyleBackColor = true;
            // 
            // tbUseHistoryBackUp
            // 
            tbUseHistoryBackUp.Location = new Point(402, 249);
            tbUseHistoryBackUp.Margin = new Padding(4);
            tbUseHistoryBackUp.Name = "tbUseHistoryBackUp";
            tbUseHistoryBackUp.Size = new Size(61, 23);
            tbUseHistoryBackUp.TabIndex = 29;
            // 
            // cbUseHistoryBackUp
            // 
            cbUseHistoryBackUp.AutoSize = true;
            cbUseHistoryBackUp.Location = new Point(8, 251);
            cbUseHistoryBackUp.Margin = new Padding(4);
            cbUseHistoryBackUp.Name = "cbUseHistoryBackUp";
            cbUseHistoryBackUp.Size = new Size(346, 19);
            cbUseHistoryBackUp.TabIndex = 28;
            cbUseHistoryBackUp.Text = "保存時に既にファイルがある場合はバックアップする。バックアップ数：";
            cbUseHistoryBackUp.UseVisualStyleBackColor = true;
            // 
            // cbUseMoonDriverDotNET
            // 
            cbUseMoonDriverDotNET.AutoSize = true;
            cbUseMoonDriverDotNET.Location = new Point(8, 224);
            cbUseMoonDriverDotNET.Margin = new Padding(4);
            cbUseMoonDriverDotNET.Name = "cbUseMoonDriverDotNET";
            cbUseMoonDriverDotNET.Size = new Size(237, 19);
            cbUseMoonDriverDotNET.TabIndex = 27;
            cbUseMoonDriverDotNET.Text = "MoonDriverDotNETを使用する(要再起動)";
            cbUseMoonDriverDotNET.UseVisualStyleBackColor = true;
            // 
            // cbUsePMDDotNET
            // 
            cbUsePMDDotNET.AutoSize = true;
            cbUsePMDDotNET.Location = new Point(8, 196);
            cbUsePMDDotNET.Margin = new Padding(4);
            cbUsePMDDotNET.Name = "cbUsePMDDotNET";
            cbUsePMDDotNET.Size = new Size(200, 19);
            cbUsePMDDotNET.TabIndex = 27;
            cbUsePMDDotNET.Text = "PMDDotNETを使用する(要再起動)";
            cbUsePMDDotNET.UseVisualStyleBackColor = true;
            // 
            // cbUseMucomDotNET
            // 
            cbUseMucomDotNET.AutoSize = true;
            cbUseMucomDotNET.Location = new Point(8, 169);
            cbUseMucomDotNET.Margin = new Padding(4);
            cbUseMucomDotNET.Name = "cbUseMucomDotNET";
            cbUseMucomDotNET.Size = new Size(214, 19);
            cbUseMucomDotNET.TabIndex = 27;
            cbUseMucomDotNET.Text = "mucomDotNETを使用する(要再起動)";
            cbUseMucomDotNET.UseVisualStyleBackColor = true;
            // 
            // cbUseScript
            // 
            cbUseScript.AutoSize = true;
            cbUseScript.Location = new Point(8, 141);
            cbUseScript.Margin = new Padding(4);
            cbUseScript.Name = "cbUseScript";
            cbUseScript.Size = new Size(177, 19);
            cbUseScript.TabIndex = 27;
            cbUseScript.Text = "スクリプトを使用する(要再起動)";
            cbUseScript.UseVisualStyleBackColor = true;
            // 
            // cbChangeEnterCode
            // 
            cbChangeEnterCode.AutoSize = true;
            cbChangeEnterCode.Location = new Point(8, 31);
            cbChangeEnterCode.Margin = new Padding(4);
            cbChangeEnterCode.Name = "cbChangeEnterCode";
            cbChangeEnterCode.Size = new Size(256, 19);
            cbChangeEnterCode.TabIndex = 24;
            cbChangeEnterCode.Text = "テキストファイルを開いた時、改行コード変換を行う";
            cbChangeEnterCode.UseVisualStyleBackColor = true;
            // 
            // cbClearHistory
            // 
            cbClearHistory.AutoSize = true;
            cbClearHistory.Location = new Point(8, 4);
            cbClearHistory.Margin = new Padding(4);
            cbClearHistory.Name = "cbClearHistory";
            cbClearHistory.Size = new Size(232, 19);
            cbClearHistory.TabIndex = 26;
            cbClearHistory.Text = "ファイル保存時、ヒストリ(アンドゥ)をクリアする";
            cbClearHistory.UseVisualStyleBackColor = true;
            // 
            // tbOpacity
            // 
            tbOpacity.Location = new Point(125, 416);
            tbOpacity.Margin = new Padding(4);
            tbOpacity.Maximum = 100;
            tbOpacity.Minimum = 1;
            tbOpacity.Name = "tbOpacity";
            tbOpacity.Size = new Size(167, 45);
            tbOpacity.TabIndex = 20;
            tbOpacity.TickFrequency = 10;
            tbOpacity.TickStyle = TickStyle.Both;
            tbOpacity.Value = 1;
            tbOpacity.Scroll += tbOpacity_Scroll;
            // 
            // label56
            // 
            label56.AutoSize = true;
            label56.Location = new Point(8, 475);
            label56.Margin = new Padding(4, 0, 4, 0);
            label56.Name = "label56";
            label56.Size = new Size(37, 15);
            label56.TabIndex = 19;
            label56.Text = "タブ幅";
            // 
            // label52
            // 
            label52.AutoSize = true;
            label52.Location = new Point(6, 436);
            label52.Margin = new Padding(4, 0, 4, 0);
            label52.Name = "label52";
            label52.Size = new Size(97, 15);
            label52.TabIndex = 19;
            label52.Text = "ウィンドウ不透明度";
            // 
            // groupBox29
            // 
            groupBox29.Controls.Add(label36);
            groupBox29.Controls.Add(btFont);
            groupBox29.Controls.Add(label54);
            groupBox29.Controls.Add(lblFontName);
            groupBox29.Controls.Add(label53);
            groupBox29.Controls.Add(lblFontSize);
            groupBox29.Controls.Add(lblFontStyle);
            groupBox29.Location = new Point(299, 392);
            groupBox29.Margin = new Padding(4);
            groupBox29.Name = "groupBox29";
            groupBox29.Padding = new Padding(4);
            groupBox29.Size = new Size(215, 106);
            groupBox29.TabIndex = 21;
            groupBox29.TabStop = false;
            groupBox29.Text = "フォント";
            // 
            // label36
            // 
            label36.AutoSize = true;
            label36.Location = new Point(6, 19);
            label36.Margin = new Padding(4, 0, 4, 0);
            label36.Name = "label36";
            label36.Size = new Size(38, 15);
            label36.TabIndex = 19;
            label36.Text = "Name";
            // 
            // btFont
            // 
            btFont.Location = new Point(181, 69);
            btFont.Margin = new Padding(4);
            btFont.Name = "btFont";
            btFont.Size = new Size(27, 29);
            btFont.TabIndex = 18;
            btFont.Text = "...";
            btFont.UseVisualStyleBackColor = true;
            btFont.Click += BtFont_Click;
            // 
            // label54
            // 
            label54.AutoSize = true;
            label54.Location = new Point(6, 75);
            label54.Margin = new Padding(4, 0, 4, 0);
            label54.Name = "label54";
            label54.Size = new Size(32, 15);
            label54.TabIndex = 19;
            label54.Text = "Style";
            // 
            // lblFontName
            // 
            lblFontName.AutoSize = true;
            lblFontName.Location = new Point(64, 19);
            lblFontName.Margin = new Padding(4, 0, 4, 0);
            lblFontName.Name = "lblFontName";
            lblFontName.Size = new Size(54, 15);
            lblFontName.TabIndex = 19;
            lblFontName.Text = "Consolas";
            // 
            // label53
            // 
            label53.AutoSize = true;
            label53.Location = new Point(6, 48);
            label53.Margin = new Padding(4, 0, 4, 0);
            label53.Name = "label53";
            label53.Size = new Size(27, 15);
            label53.TabIndex = 19;
            label53.Text = "Size";
            // 
            // lblFontSize
            // 
            lblFontSize.AutoSize = true;
            lblFontSize.Location = new Point(64, 48);
            lblFontSize.Margin = new Padding(4, 0, 4, 0);
            lblFontSize.Name = "lblFontSize";
            lblFontSize.Size = new Size(19, 15);
            lblFontSize.TabIndex = 19;
            lblFontSize.Text = "12";
            // 
            // lblFontStyle
            // 
            lblFontStyle.AutoSize = true;
            lblFontStyle.Location = new Point(64, 75);
            lblFontStyle.Margin = new Padding(4, 0, 4, 0);
            lblFontStyle.Name = "lblFontStyle";
            lblFontStyle.Size = new Size(47, 15);
            lblFontStyle.TabIndex = 19;
            lblFontStyle.Text = "Regular";
            // 
            // cbInfiniteOfflineMode
            // 
            cbInfiniteOfflineMode.AutoSize = true;
            cbInfiniteOfflineMode.Location = new Point(8, 114);
            cbInfiniteOfflineMode.Margin = new Padding(4);
            cbInfiniteOfflineMode.Name = "cbInfiniteOfflineMode";
            cbInfiniteOfflineMode.Size = new Size(139, 19);
            cbInfiniteOfflineMode.TabIndex = 23;
            cbInfiniteOfflineMode.Text = "永続的にオフラインモード";
            cbInfiniteOfflineMode.UseVisualStyleBackColor = true;
            // 
            // cbUseSIen
            // 
            cbUseSIen.AutoSize = true;
            cbUseSIen.Location = new Point(8, 86);
            cbUseSIen.Margin = new Padding(4);
            cbUseSIen.Name = "cbUseSIen";
            cbUseSIen.Size = new Size(150, 19);
            cbUseSIen.TabIndex = 23;
            cbUseSIen.Text = "入力支援機能を使用する";
            cbUseSIen.UseVisualStyleBackColor = true;
            // 
            // cbRequestCacheClear
            // 
            cbRequestCacheClear.AutoSize = true;
            cbRequestCacheClear.Location = new Point(8, 59);
            cbRequestCacheClear.Margin = new Padding(4);
            cbRequestCacheClear.Name = "cbRequestCacheClear";
            cbRequestCacheClear.Size = new Size(213, 19);
            cbRequestCacheClear.TabIndex = 23;
            cbRequestCacheClear.Text = "音色のキャッシュをクリアする(要再起動)";
            cbRequestCacheClear.UseVisualStyleBackColor = true;
            // 
            // tpOther
            // 
            tpOther.BorderStyle = BorderStyle.FixedSingle;
            tpOther.Controls.Add(cbWavSwitch);
            tpOther.Controls.Add(groupBox17);
            tpOther.Controls.Add(cbUseGetInst);
            tpOther.Controls.Add(groupBox4);
            tpOther.Controls.Add(cbDumpSwitch);
            tpOther.Controls.Add(gbWav);
            tpOther.Controls.Add(gbDump);
            tpOther.Controls.Add(label30);
            tpOther.Controls.Add(tbScreenFrameRate);
            tpOther.Controls.Add(label29);
            tpOther.Controls.Add(btnDataPath);
            tpOther.Controls.Add(tbDataPath);
            tpOther.Controls.Add(label19);
            tpOther.Controls.Add(btnResetPosition);
            tpOther.Controls.Add(btnOpenSettingFolder);
            tpOther.Controls.Add(cbEmptyPlayList);
            tpOther.Controls.Add(cbInitAlways);
            tpOther.Controls.Add(cbAutoOpen);
            tpOther.Controls.Add(cbUseLoopTimes);
            tpOther.Location = new Point(4, 44);
            tpOther.Margin = new Padding(4);
            tpOther.Name = "tpOther";
            tpOther.Size = new Size(518, 510);
            tpOther.TabIndex = 2;
            tpOther.Text = "Other";
            tpOther.UseVisualStyleBackColor = true;
            // 
            // cbWavSwitch
            // 
            cbWavSwitch.AutoSize = true;
            cbWavSwitch.Location = new Point(16, 252);
            cbWavSwitch.Margin = new Padding(4);
            cbWavSwitch.Name = "cbWavSwitch";
            cbWavSwitch.Size = new Size(181, 19);
            cbWavSwitch.TabIndex = 0;
            cbWavSwitch.Text = "演奏時に.wavファイルを出力する";
            cbWavSwitch.UseVisualStyleBackColor = true;
            cbWavSwitch.CheckedChanged += cbWavSwitch_CheckedChanged;
            // 
            // groupBox17
            // 
            groupBox17.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox17.Controls.Add(tbImageExt);
            groupBox17.Controls.Add(tbMMLExt);
            groupBox17.Controls.Add(tbTextExt);
            groupBox17.Controls.Add(label1);
            groupBox17.Controls.Add(label3);
            groupBox17.Controls.Add(label2);
            groupBox17.Location = new Point(8, 320);
            groupBox17.Margin = new Padding(4);
            groupBox17.Name = "groupBox17";
            groupBox17.Padding = new Padding(4);
            groupBox17.Size = new Size(262, 104);
            groupBox17.TabIndex = 1;
            groupBox17.TabStop = false;
            groupBox17.Text = "File Extension";
            // 
            // tbImageExt
            // 
            tbImageExt.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tbImageExt.Location = new Point(61, 72);
            tbImageExt.Margin = new Padding(4);
            tbImageExt.Name = "tbImageExt";
            tbImageExt.Size = new Size(188, 23);
            tbImageExt.TabIndex = 1;
            // 
            // tbMMLExt
            // 
            tbMMLExt.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tbMMLExt.Location = new Point(61, 44);
            tbMMLExt.Margin = new Padding(4);
            tbMMLExt.Name = "tbMMLExt";
            tbMMLExt.Size = new Size(188, 23);
            tbMMLExt.TabIndex = 1;
            // 
            // tbTextExt
            // 
            tbTextExt.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tbTextExt.Location = new Point(61, 15);
            tbTextExt.Margin = new Padding(4);
            tbTextExt.Name = "tbTextExt";
            tbTextExt.Size = new Size(188, 23);
            tbTextExt.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(7, 19);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(28, 15);
            label1.TabIndex = 0;
            label1.Text = "Text";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(7, 76);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(39, 15);
            label3.TabIndex = 0;
            label3.Text = "Image";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(7, 48);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(35, 15);
            label2.TabIndex = 0;
            label2.Text = "MML";
            // 
            // cbUseGetInst
            // 
            cbUseGetInst.AutoSize = true;
            cbUseGetInst.Location = new Point(16, 39);
            cbUseGetInst.Margin = new Padding(4);
            cbUseGetInst.Name = "cbUseGetInst";
            cbUseGetInst.Size = new Size(289, 19);
            cbUseGetInst.TabIndex = 12;
            cbUseGetInst.Text = "音色欄をクリック時、その音色をクリップボードにコピーする";
            cbUseGetInst.UseVisualStyleBackColor = true;
            cbUseGetInst.CheckedChanged += cbUseGetInst_CheckedChanged;
            // 
            // groupBox4
            // 
            groupBox4.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox4.Controls.Add(cmbInstFormat);
            groupBox4.Controls.Add(lblInstFormat);
            groupBox4.Location = new Point(8, 39);
            groupBox4.Margin = new Padding(4);
            groupBox4.Name = "groupBox4";
            groupBox4.Padding = new Padding(4);
            groupBox4.Size = new Size(503, 56);
            groupBox4.TabIndex = 23;
            groupBox4.TabStop = false;
            // 
            // cmbInstFormat
            // 
            cmbInstFormat.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cmbInstFormat.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbInstFormat.FormattingEnabled = true;
            cmbInstFormat.Items.AddRange(new object[] { "FMP7", "MDX", ".TFI(ファイル出力)", "MUSIC LALF #1", "MUSIC LALF #2", "MML2VGM", "NRTDRV", "HuSIC" });
            cmbInstFormat.Location = new Point(348, 24);
            cmbInstFormat.Margin = new Padding(4);
            cmbInstFormat.Name = "cmbInstFormat";
            cmbInstFormat.Size = new Size(148, 23);
            cmbInstFormat.TabIndex = 18;
            // 
            // lblInstFormat
            // 
            lblInstFormat.AutoSize = true;
            lblInstFormat.Location = new Point(276, 28);
            lblInstFormat.Margin = new Padding(4, 0, 4, 0);
            lblInstFormat.Name = "lblInstFormat";
            lblInstFormat.Size = new Size(56, 15);
            lblInstFormat.TabIndex = 17;
            lblInstFormat.Text = "フォーマット";
            // 
            // cbDumpSwitch
            // 
            cbDumpSwitch.AutoSize = true;
            cbDumpSwitch.Location = new Point(16, 190);
            cbDumpSwitch.Margin = new Padding(4);
            cbDumpSwitch.Name = "cbDumpSwitch";
            cbDumpSwitch.Size = new Size(222, 19);
            cbDumpSwitch.TabIndex = 0;
            cbDumpSwitch.Text = "DataBlock処理時にその内容をダンプする";
            cbDumpSwitch.UseVisualStyleBackColor = true;
            cbDumpSwitch.CheckedChanged += cbDumpSwitch_CheckedChanged;
            // 
            // gbWav
            // 
            gbWav.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            gbWav.Controls.Add(btnWavPath);
            gbWav.Controls.Add(label7);
            gbWav.Controls.Add(tbWavPath);
            gbWav.Location = new Point(8, 256);
            gbWav.Margin = new Padding(4);
            gbWav.Name = "gbWav";
            gbWav.Padding = new Padding(4);
            gbWav.Size = new Size(503, 56);
            gbWav.TabIndex = 22;
            gbWav.TabStop = false;
            // 
            // btnWavPath
            // 
            btnWavPath.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnWavPath.Location = new Point(469, 20);
            btnWavPath.Margin = new Padding(4);
            btnWavPath.Name = "btnWavPath";
            btnWavPath.Size = new Size(27, 29);
            btnWavPath.TabIndex = 16;
            btnWavPath.Text = "...";
            btnWavPath.UseVisualStyleBackColor = true;
            btnWavPath.Click += btnWavPath_Click;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(7, 26);
            label7.Margin = new Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new Size(55, 15);
            label7.TabIndex = 14;
            label7.Text = "出力Path";
            // 
            // tbWavPath
            // 
            tbWavPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tbWavPath.Location = new Point(85, 22);
            tbWavPath.Margin = new Padding(4);
            tbWavPath.Name = "tbWavPath";
            tbWavPath.Size = new Size(376, 23);
            tbWavPath.TabIndex = 15;
            // 
            // gbDump
            // 
            gbDump.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            gbDump.Controls.Add(btnDumpPath);
            gbDump.Controls.Add(label6);
            gbDump.Controls.Add(tbDumpPath);
            gbDump.Location = new Point(8, 192);
            gbDump.Margin = new Padding(4);
            gbDump.Name = "gbDump";
            gbDump.Padding = new Padding(4);
            gbDump.Size = new Size(503, 56);
            gbDump.TabIndex = 22;
            gbDump.TabStop = false;
            // 
            // btnDumpPath
            // 
            btnDumpPath.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnDumpPath.Location = new Point(469, 20);
            btnDumpPath.Margin = new Padding(4);
            btnDumpPath.Name = "btnDumpPath";
            btnDumpPath.Size = new Size(27, 29);
            btnDumpPath.TabIndex = 16;
            btnDumpPath.Text = "...";
            btnDumpPath.UseVisualStyleBackColor = true;
            btnDumpPath.Click += btnDumpPath_Click;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(7, 26);
            label6.Margin = new Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new Size(55, 15);
            label6.TabIndex = 14;
            label6.Text = "出力Path";
            // 
            // tbDumpPath
            // 
            tbDumpPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tbDumpPath.Location = new Point(85, 22);
            tbDumpPath.Margin = new Padding(4);
            tbDumpPath.Name = "tbDumpPath";
            tbDumpPath.Size = new Size(376, 23);
            tbDumpPath.TabIndex = 15;
            // 
            // label30
            // 
            label30.AutoSize = true;
            label30.Location = new Point(159, 165);
            label30.Margin = new Padding(4, 0, 4, 0);
            label30.Name = "label30";
            label30.Size = new Size(31, 15);
            label30.TabIndex = 21;
            label30.Text = "Hz/s";
            // 
            // tbScreenFrameRate
            // 
            tbScreenFrameRate.Location = new Point(93, 161);
            tbScreenFrameRate.Margin = new Padding(4);
            tbScreenFrameRate.Name = "tbScreenFrameRate";
            tbScreenFrameRate.Size = new Size(58, 23);
            tbScreenFrameRate.TabIndex = 20;
            // 
            // label29
            // 
            label29.AutoSize = true;
            label29.Location = new Point(6, 165);
            label29.Margin = new Padding(4, 0, 4, 0);
            label29.Name = "label29";
            label29.Size = new Size(66, 15);
            label29.TabIndex = 19;
            label29.Text = "フレームレート";
            // 
            // btnDataPath
            // 
            btnDataPath.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnDataPath.Location = new Point(477, 128);
            btnDataPath.Margin = new Padding(4);
            btnDataPath.Name = "btnDataPath";
            btnDataPath.Size = new Size(27, 29);
            btnDataPath.TabIndex = 16;
            btnDataPath.Text = "...";
            btnDataPath.UseVisualStyleBackColor = true;
            btnDataPath.Click += btnDataPath_Click;
            // 
            // tbDataPath
            // 
            tbDataPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tbDataPath.Location = new Point(93, 130);
            tbDataPath.Margin = new Padding(4);
            tbDataPath.Name = "tbDataPath";
            tbDataPath.Size = new Size(376, 23);
            tbDataPath.TabIndex = 15;
            // 
            // label19
            // 
            label19.AutoSize = true;
            label19.Location = new Point(6, 134);
            label19.Margin = new Padding(4, 0, 4, 0);
            label19.Name = "label19";
            label19.Size = new Size(57, 15);
            label19.TabIndex = 14;
            label19.Text = "データPath";
            // 
            // btnResetPosition
            // 
            btnResetPosition.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnResetPosition.Location = new Point(192, 468);
            btnResetPosition.Margin = new Padding(4);
            btnResetPosition.Name = "btnResetPosition";
            btnResetPosition.Size = new Size(166, 29);
            btnResetPosition.TabIndex = 13;
            btnResetPosition.Text = "ウィンドウ位置をリセット";
            btnResetPosition.UseVisualStyleBackColor = true;
            btnResetPosition.Click += btnResetPosition_Click;
            // 
            // btnOpenSettingFolder
            // 
            btnOpenSettingFolder.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnOpenSettingFolder.Location = new Point(365, 468);
            btnOpenSettingFolder.Margin = new Padding(4);
            btnOpenSettingFolder.Name = "btnOpenSettingFolder";
            btnOpenSettingFolder.Size = new Size(146, 29);
            btnOpenSettingFolder.TabIndex = 13;
            btnOpenSettingFolder.Text = "設定フォルダーを開く";
            btnOpenSettingFolder.UseVisualStyleBackColor = true;
            btnOpenSettingFolder.Click += btnOpenSettingFolder_Click;
            // 
            // cbEmptyPlayList
            // 
            cbEmptyPlayList.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cbEmptyPlayList.AutoSize = true;
            cbEmptyPlayList.Location = new Point(304, 385);
            cbEmptyPlayList.Margin = new Padding(4);
            cbEmptyPlayList.Name = "cbEmptyPlayList";
            cbEmptyPlayList.Size = new Size(180, 19);
            cbEmptyPlayList.TabIndex = 0;
            cbEmptyPlayList.Text = "起動時にプレイリストを空にする。";
            cbEmptyPlayList.UseVisualStyleBackColor = true;
            cbEmptyPlayList.CheckedChanged += cbUseLoopTimes_CheckedChanged;
            // 
            // cbInitAlways
            // 
            cbInitAlways.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cbInitAlways.Location = new Point(278, 329);
            cbInitAlways.Margin = new Padding(4);
            cbInitAlways.Name = "cbInitAlways";
            cbInitAlways.Size = new Size(226, 49);
            cbInitAlways.TabIndex = 0;
            cbInitAlways.Text = "再生開始時に必ずデバイスを初期化する。";
            cbInitAlways.UseVisualStyleBackColor = true;
            cbInitAlways.CheckedChanged += cbUseLoopTimes_CheckedChanged;
            // 
            // cbAutoOpen
            // 
            cbAutoOpen.AutoSize = true;
            cbAutoOpen.Location = new Point(8, 102);
            cbAutoOpen.Margin = new Padding(4);
            cbAutoOpen.Name = "cbAutoOpen";
            cbAutoOpen.Size = new Size(170, 19);
            cbAutoOpen.TabIndex = 0;
            cbAutoOpen.Text = "使用音源の画面を自動で開く";
            cbAutoOpen.UseVisualStyleBackColor = true;
            cbAutoOpen.CheckedChanged += cbUseLoopTimes_CheckedChanged;
            // 
            // cbUseLoopTimes
            // 
            cbUseLoopTimes.AutoSize = true;
            cbUseLoopTimes.Location = new Point(8, 9);
            cbUseLoopTimes.Margin = new Padding(4);
            cbUseLoopTimes.Name = "cbUseLoopTimes";
            cbUseLoopTimes.Size = new Size(215, 19);
            cbUseLoopTimes.TabIndex = 0;
            cbUseLoopTimes.Text = "無限ループ時、指定の回数だけ繰り返す";
            cbUseLoopTimes.UseVisualStyleBackColor = true;
            cbUseLoopTimes.CheckedChanged += cbUseLoopTimes_CheckedChanged;
            // 
            // tpOmake
            // 
            tpOmake.BorderStyle = BorderStyle.FixedSingle;
            tpOmake.Controls.Add(groupBox31);
            tpOmake.Controls.Add(groupBox30);
            tpOmake.Controls.Add(label14);
            tpOmake.Controls.Add(btVST);
            tpOmake.Controls.Add(tbVST);
            tpOmake.Controls.Add(groupBox5);
            tpOmake.Location = new Point(4, 44);
            tpOmake.Margin = new Padding(4);
            tpOmake.Name = "tpOmake";
            tpOmake.Size = new Size(518, 510);
            tpOmake.TabIndex = 7;
            tpOmake.Text = "おまけ";
            tpOmake.UseVisualStyleBackColor = true;
            // 
            // groupBox31
            // 
            groupBox31.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox31.Controls.Add(rbLoglevelINFO);
            groupBox31.Controls.Add(rbLoglevelDEBUG);
            groupBox31.Controls.Add(rbLoglevelTRACE);
            groupBox31.Location = new Point(8, 151);
            groupBox31.Margin = new Padding(4);
            groupBox31.Name = "groupBox31";
            groupBox31.Padding = new Padding(4);
            groupBox31.Size = new Size(503, 54);
            groupBox31.TabIndex = 25;
            groupBox31.TabStop = false;
            groupBox31.Text = "Log level";
            // 
            // rbLoglevelINFO
            // 
            rbLoglevelINFO.AutoSize = true;
            rbLoglevelINFO.Checked = true;
            rbLoglevelINFO.Location = new Point(13, 20);
            rbLoglevelINFO.Margin = new Padding(4);
            rbLoglevelINFO.Name = "rbLoglevelINFO";
            rbLoglevelINFO.Size = new Size(52, 19);
            rbLoglevelINFO.TabIndex = 3;
            rbLoglevelINFO.TabStop = true;
            rbLoglevelINFO.Text = "INFO";
            rbLoglevelINFO.UseVisualStyleBackColor = true;
            // 
            // rbLoglevelDEBUG
            // 
            rbLoglevelDEBUG.AutoSize = true;
            rbLoglevelDEBUG.Location = new Point(77, 20);
            rbLoglevelDEBUG.Margin = new Padding(4);
            rbLoglevelDEBUG.Name = "rbLoglevelDEBUG";
            rbLoglevelDEBUG.Size = new Size(62, 19);
            rbLoglevelDEBUG.TabIndex = 3;
            rbLoglevelDEBUG.Text = "DEBUG";
            rbLoglevelDEBUG.UseVisualStyleBackColor = true;
            // 
            // rbLoglevelTRACE
            // 
            rbLoglevelTRACE.AutoSize = true;
            rbLoglevelTRACE.Location = new Point(156, 20);
            rbLoglevelTRACE.Margin = new Padding(4);
            rbLoglevelTRACE.Name = "rbLoglevelTRACE";
            rbLoglevelTRACE.Size = new Size(59, 19);
            rbLoglevelTRACE.TabIndex = 3;
            rbLoglevelTRACE.Text = "TRACE";
            rbLoglevelTRACE.UseVisualStyleBackColor = true;
            // 
            // groupBox30
            // 
            groupBox30.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox30.Controls.Add(rbQueryPerformanceCounter);
            groupBox30.Controls.Add(rbDateTime);
            groupBox30.Controls.Add(rbStopWatch);
            groupBox30.Location = new Point(8, 212);
            groupBox30.Margin = new Padding(4);
            groupBox30.Name = "groupBox30";
            groupBox30.Padding = new Padding(4);
            groupBox30.Size = new Size(503, 106);
            groupBox30.TabIndex = 24;
            groupBox30.TabStop = false;
            groupBox30.Text = "Music Interrupt Timer";
            // 
            // rbQueryPerformanceCounter
            // 
            rbQueryPerformanceCounter.AutoSize = true;
            rbQueryPerformanceCounter.Location = new Point(7, 78);
            rbQueryPerformanceCounter.Margin = new Padding(4);
            rbQueryPerformanceCounter.Name = "rbQueryPerformanceCounter";
            rbQueryPerformanceCounter.Size = new Size(225, 19);
            rbQueryPerformanceCounter.TabIndex = 2;
            rbQueryPerformanceCounter.Text = "QueryPerformanceCounter(Win32API)";
            rbQueryPerformanceCounter.UseVisualStyleBackColor = true;
            // 
            // rbDateTime
            // 
            rbDateTime.AutoSize = true;
            rbDateTime.Location = new Point(7, 50);
            rbDateTime.Margin = new Padding(4);
            rbDateTime.Name = "rbDateTime";
            rbDateTime.Size = new Size(96, 19);
            rbDateTime.TabIndex = 1;
            rbDateTime.Text = "DateTime(C#)";
            rbDateTime.UseVisualStyleBackColor = true;
            // 
            // rbStopWatch
            // 
            rbStopWatch.AutoSize = true;
            rbStopWatch.Checked = true;
            rbStopWatch.Location = new Point(7, 22);
            rbStopWatch.Margin = new Padding(4);
            rbStopWatch.Name = "rbStopWatch";
            rbStopWatch.Size = new Size(143, 19);
            rbStopWatch.TabIndex = 0;
            rbStopWatch.TabStop = true;
            rbStopWatch.Text = "Stopwatch(C#) default";
            rbStopWatch.UseVisualStyleBackColor = true;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new Point(8, 474);
            label14.Margin = new Padding(4, 0, 4, 0);
            label14.Name = "label14";
            label14.Size = new Size(58, 15);
            label14.TabIndex = 19;
            label14.Text = "VST effect";
            label14.Visible = false;
            // 
            // btVST
            // 
            btVST.Location = new Point(486, 468);
            btVST.Margin = new Padding(4);
            btVST.Name = "btVST";
            btVST.Size = new Size(27, 29);
            btVST.TabIndex = 18;
            btVST.Text = "...";
            btVST.UseVisualStyleBackColor = true;
            btVST.Visible = false;
            btVST.Click += btVST_Click;
            // 
            // tbVST
            // 
            tbVST.Location = new Point(103, 470);
            tbVST.Margin = new Padding(4);
            tbVST.Name = "tbVST";
            tbVST.Size = new Size(376, 23);
            tbVST.TabIndex = 17;
            tbVST.Visible = false;
            // 
            // groupBox5
            // 
            groupBox5.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox5.Controls.Add(cbSinWave);
            groupBox5.Controls.Add(cbPlayDeviceCB);
            groupBox5.Controls.Add(cbLogWarning);
            groupBox5.Controls.Add(cbDispFrameCounter);
            groupBox5.Location = new Point(8, 4);
            groupBox5.Margin = new Padding(4);
            groupBox5.Name = "groupBox5";
            groupBox5.Padding = new Padding(4);
            groupBox5.Size = new Size(503, 140);
            groupBox5.TabIndex = 3;
            groupBox5.TabStop = false;
            groupBox5.Text = "Debug Mode";
            // 
            // cbSinWave
            // 
            cbSinWave.AutoSize = true;
            cbSinWave.Location = new Point(8, 106);
            cbSinWave.Margin = new Padding(4);
            cbSinWave.Name = "cbSinWave";
            cbSinWave.Size = new Size(94, 19);
            cbSinWave.TabIndex = 2;
            cbSinWave.Text = "440Hz正弦波";
            cbSinWave.UseVisualStyleBackColor = true;
            // 
            // cbPlayDeviceCB
            // 
            cbPlayDeviceCB.AutoSize = true;
            cbPlayDeviceCB.Location = new Point(7, 79);
            cbPlayDeviceCB.Margin = new Padding(4);
            cbPlayDeviceCB.Name = "cbPlayDeviceCB";
            cbPlayDeviceCB.Size = new Size(160, 19);
            cbPlayDeviceCB.TabIndex = 2;
            cbPlayDeviceCB.Text = "デバイスのコールバックで演奏";
            cbPlayDeviceCB.UseVisualStyleBackColor = true;
            // 
            // cbLogWarning
            // 
            cbLogWarning.AutoSize = true;
            cbLogWarning.Location = new Point(7, 49);
            cbLogWarning.Margin = new Padding(4);
            cbLogWarning.Name = "cbLogWarning";
            cbLogWarning.Size = new Size(150, 19);
            cbLogWarning.TabIndex = 2;
            cbLogWarning.Text = "ログにWarningを出力する";
            cbLogWarning.UseVisualStyleBackColor = true;
            // 
            // cbDispFrameCounter
            // 
            cbDispFrameCounter.AutoSize = true;
            cbDispFrameCounter.Location = new Point(7, 21);
            cbDispFrameCounter.Margin = new Padding(4);
            cbDispFrameCounter.Name = "cbDispFrameCounter";
            cbDispFrameCounter.Size = new Size(127, 19);
            cbDispFrameCounter.TabIndex = 2;
            cbDispFrameCounter.Text = "Debugウィンドウ表示";
            cbDispFrameCounter.UseVisualStyleBackColor = true;
            // 
            // tpAbout
            // 
            tpAbout.BorderStyle = BorderStyle.FixedSingle;
            tpAbout.Controls.Add(tableLayoutPanel);
            tpAbout.Location = new Point(4, 44);
            tpAbout.Margin = new Padding(4);
            tpAbout.Name = "tpAbout";
            tpAbout.Padding = new Padding(4);
            tpAbout.Size = new Size(518, 510);
            tpAbout.TabIndex = 1;
            tpAbout.Text = "About";
            tpAbout.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.ColumnCount = 2;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 67F));
            tableLayoutPanel.Controls.Add(logoPictureBox, 0, 0);
            tableLayoutPanel.Controls.Add(labelProductName, 1, 0);
            tableLayoutPanel.Controls.Add(labelVersion, 1, 1);
            tableLayoutPanel.Controls.Add(labelCopyright, 1, 2);
            tableLayoutPanel.Controls.Add(labelCompanyName, 1, 3);
            tableLayoutPanel.Controls.Add(textBoxDescription, 1, 4);
            tableLayoutPanel.Controls.Add(llOpenGithub, 1, 5);
            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.Location = new Point(4, 4);
            tableLayoutPanel.Margin = new Padding(4);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 6;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 8.070175F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 53.33333F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 8.421053F));
            tableLayoutPanel.Size = new Size(508, 500);
            tableLayoutPanel.TabIndex = 1;
            // 
            // logoPictureBox
            // 
            logoPictureBox.Dock = DockStyle.Fill;
            logoPictureBox.Image = Resources.ccPlay;
            logoPictureBox.Location = new Point(4, 4);
            logoPictureBox.Margin = new Padding(4);
            logoPictureBox.Name = "logoPictureBox";
            tableLayoutPanel.SetRowSpan(logoPictureBox, 6);
            logoPictureBox.Size = new Size(159, 492);
            logoPictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
            logoPictureBox.TabIndex = 12;
            logoPictureBox.TabStop = false;
            // 
            // labelProductName
            // 
            labelProductName.Dock = DockStyle.Fill;
            labelProductName.Location = new Point(174, 0);
            labelProductName.Margin = new Padding(7, 0, 4, 0);
            labelProductName.MaximumSize = new Size(0, 20);
            labelProductName.Name = "labelProductName";
            labelProductName.Size = new Size(330, 20);
            labelProductName.TabIndex = 19;
            labelProductName.Text = "製品名";
            labelProductName.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // labelVersion
            // 
            labelVersion.Dock = DockStyle.Fill;
            labelVersion.Location = new Point(174, 50);
            labelVersion.Margin = new Padding(7, 0, 4, 0);
            labelVersion.MaximumSize = new Size(0, 20);
            labelVersion.Name = "labelVersion";
            labelVersion.Size = new Size(330, 20);
            labelVersion.TabIndex = 0;
            labelVersion.Text = "バージョン";
            labelVersion.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // labelCopyright
            // 
            labelCopyright.Dock = DockStyle.Fill;
            labelCopyright.Location = new Point(174, 100);
            labelCopyright.Margin = new Padding(7, 0, 4, 0);
            labelCopyright.MaximumSize = new Size(0, 20);
            labelCopyright.Name = "labelCopyright";
            labelCopyright.Size = new Size(330, 20);
            labelCopyright.TabIndex = 21;
            labelCopyright.Text = "著作権";
            labelCopyright.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // labelCompanyName
            // 
            labelCompanyName.Dock = DockStyle.Fill;
            labelCompanyName.Location = new Point(174, 150);
            labelCompanyName.Margin = new Padding(7, 0, 4, 0);
            labelCompanyName.MaximumSize = new Size(0, 20);
            labelCompanyName.Name = "labelCompanyName";
            labelCompanyName.Size = new Size(330, 20);
            labelCompanyName.TabIndex = 22;
            labelCompanyName.Text = "会社名";
            labelCompanyName.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // textBoxDescription
            // 
            textBoxDescription.Dock = DockStyle.Fill;
            textBoxDescription.Location = new Point(174, 194);
            textBoxDescription.Margin = new Padding(7, 4, 4, 4);
            textBoxDescription.Multiline = true;
            textBoxDescription.Name = "textBoxDescription";
            textBoxDescription.ReadOnly = true;
            textBoxDescription.ScrollBars = ScrollBars.Both;
            textBoxDescription.Size = new Size(330, 259);
            textBoxDescription.TabIndex = 23;
            textBoxDescription.TabStop = false;
            textBoxDescription.Text = "説明";
            // 
            // llOpenGithub
            // 
            llOpenGithub.AutoSize = true;
            llOpenGithub.Dock = DockStyle.Fill;
            llOpenGithub.Location = new Point(171, 457);
            llOpenGithub.Margin = new Padding(4, 0, 4, 0);
            llOpenGithub.Name = "llOpenGithub";
            llOpenGithub.Size = new Size(333, 43);
            llOpenGithub.TabIndex = 24;
            llOpenGithub.TabStop = true;
            llOpenGithub.Text = "Open latest version page of Github.";
            llOpenGithub.TextAlign = ContentAlignment.MiddleCenter;
            llOpenGithub.LinkClicked += llOpenGithub_LinkClicked;
            // 
            // cbHiyorimiMode
            // 
            cbHiyorimiMode.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            cbHiyorimiMode.AutoSize = true;
            cbHiyorimiMode.Location = new Point(13, 570);
            cbHiyorimiMode.Margin = new Padding(4);
            cbHiyorimiMode.Name = "cbHiyorimiMode";
            cbHiyorimiMode.Size = new Size(222, 34);
            cbHiyorimiMode.TabIndex = 6;
            cbHiyorimiMode.Text = "日和見モード(出力タブ：\r\n遅延時間100ms以下の時、使用を推奨)";
            cbHiyorimiMode.UseVisualStyleBackColor = true;
            cbHiyorimiMode.Visible = false;
            // 
            // cbHilightOn
            // 
            cbHilightOn.AutoSize = true;
            cbHilightOn.Location = new Point(8, 305);
            cbHilightOn.Name = "cbHilightOn";
            cbHilightOn.Size = new Size(121, 19);
            cbHilightOn.TabIndex = 32;
            cbHilightOn.Text = "ハイライトを使用する";
            cbHilightOn.UseVisualStyleBackColor = true;
            // 
            // FrmSetting
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new Size(541, 612);
            Controls.Add(cbHiyorimiMode);
            Controls.Add(tcSetting);
            Controls.Add(btnCancel);
            Controls.Add(btnOK);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4);
            MinimumSize = new Size(557, 651);
            Name = "FrmSetting";
            StartPosition = FormStartPosition.CenterParent;
            Text = "オプション";
            FormClosed += frmSetting_FormClosed;
            Load += frmSetting_Load;
            gbWaveOut.ResumeLayout(false);
            gbAsioOut.ResumeLayout(false);
            gbWasapiOut.ResumeLayout(false);
            gbWasapiOut.PerformLayout();
            gbDirectSound.ResumeLayout(false);
            tcSetting.ResumeLayout(false);
            tpOutput.ResumeLayout(false);
            tpOutput.PerformLayout();
            tabControl1.ResumeLayout(false);
            tabPage11.ResumeLayout(false);
            tabPage11.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            groupBox16.ResumeLayout(false);
            tabPage12.ResumeLayout(false);
            groupBox27.ResumeLayout(false);
            groupBox27.PerformLayout();
            groupBox28.ResumeLayout(false);
            groupBox28.PerformLayout();
            groupBox1.ResumeLayout(false);
            tabPage13.ResumeLayout(false);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvMIDIoutPallet).EndInit();
            tbcMIDIoutList.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvMIDIoutListA).EndInit();
            tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvMIDIoutListB).EndInit();
            tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvMIDIoutListC).EndInit();
            tabPage4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvMIDIoutListD).EndInit();
            tpNuked.ResumeLayout(false);
            tabControl2.ResumeLayout(false);
            tabPage14.ResumeLayout(false);
            groupBox26.ResumeLayout(false);
            groupBox26.PerformLayout();
            tabPage5.ResumeLayout(false);
            groupBox34.ResumeLayout(false);
            groupBox34.PerformLayout();
            tpNSF.ResumeLayout(false);
            tabControl3.ResumeLayout(false);
            tabPage15.ResumeLayout(false);
            groupBox9.ResumeLayout(false);
            groupBox9.PerformLayout();
            groupBox10.ResumeLayout(false);
            groupBox10.PerformLayout();
            groupBox8.ResumeLayout(false);
            groupBox8.PerformLayout();
            groupBox12.ResumeLayout(false);
            groupBox12.PerformLayout();
            groupBox11.ResumeLayout(false);
            groupBox11.PerformLayout();
            tabPage16.ResumeLayout(false);
            tabPage16.PerformLayout();
            groupBox14.ResumeLayout(false);
            groupBox14.PerformLayout();
            groupBox13.ResumeLayout(false);
            groupBox13.PerformLayout();
            tabPage17.ResumeLayout(false);
            groupBox15.ResumeLayout(false);
            groupBox15.PerformLayout();
            tpPMDDotNET.ResumeLayout(false);
            tpPMDDotNET.PerformLayout();
            gbPMDManual.ResumeLayout(false);
            gbPMDManual.PerformLayout();
            groupBox32.ResumeLayout(false);
            groupBox32.PerformLayout();
            gbPPSDRV.ResumeLayout(false);
            groupBox33.ResumeLayout(false);
            groupBox33.PerformLayout();
            gbPMDSetManualVolume.ResumeLayout(false);
            gbPMDSetManualVolume.PerformLayout();
            tpExport.ResumeLayout(false);
            tpExport.PerformLayout();
            gpbFixedExportPlace.ResumeLayout(false);
            gpbFixedExportPlace.PerformLayout();
            tpMIDIExp.ResumeLayout(false);
            tpMIDIExp.PerformLayout();
            gbMIDIExport.ResumeLayout(false);
            gbMIDIExport.PerformLayout();
            groupBox6.ResumeLayout(false);
            groupBox6.PerformLayout();
            tpMIDIKBD.ResumeLayout(false);
            tpMIDIKBD.PerformLayout();
            gbMIDIKeyboard.ResumeLayout(false);
            gbMIDIKeyboard.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox8).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox7).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox6).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            gbUseChannel.ResumeLayout(false);
            gbUseChannel.PerformLayout();
            groupBox7.ResumeLayout(false);
            groupBox7.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            tpKeyBoard.ResumeLayout(false);
            tpKeyBoard.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvShortCutKey).EndInit();
            tpBalance.ResumeLayout(false);
            tpBalance.PerformLayout();
            groupBox25.ResumeLayout(false);
            groupBox25.PerformLayout();
            groupBox18.ResumeLayout(false);
            groupBox24.ResumeLayout(false);
            groupBox21.ResumeLayout(false);
            groupBox21.PerformLayout();
            groupBox22.ResumeLayout(false);
            groupBox22.PerformLayout();
            groupBox23.ResumeLayout(false);
            groupBox19.ResumeLayout(false);
            groupBox19.PerformLayout();
            groupBox20.ResumeLayout(false);
            groupBox20.PerformLayout();
            tpMMLParameter.ResumeLayout(false);
            tpMMLParameter.PerformLayout();
            tpOther2.ResumeLayout(false);
            tpOther2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)tbOpacity).EndInit();
            groupBox29.ResumeLayout(false);
            groupBox29.PerformLayout();
            tpOther.ResumeLayout(false);
            tpOther.PerformLayout();
            groupBox17.ResumeLayout(false);
            groupBox17.PerformLayout();
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            gbWav.ResumeLayout(false);
            gbWav.PerformLayout();
            gbDump.ResumeLayout(false);
            gbDump.PerformLayout();
            tpOmake.ResumeLayout(false);
            tpOmake.PerformLayout();
            groupBox31.ResumeLayout(false);
            groupBox31.PerformLayout();
            groupBox30.ResumeLayout(false);
            groupBox30.PerformLayout();
            groupBox5.ResumeLayout(false);
            groupBox5.PerformLayout();
            tpAbout.ResumeLayout(false);
            tableLayoutPanel.ResumeLayout(false);
            tableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)logoPictureBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnOK;
        private Button btnCancel;
        private GroupBox gbWaveOut;
        private RadioButton rbWaveOut;
        private RadioButton rbAsioOut;
        private RadioButton rbWasapiOut;
        private GroupBox gbAsioOut;
        private RadioButton rbDirectSoundOut;
        private GroupBox gbWasapiOut;
        private GroupBox gbDirectSound;
        private ComboBox cmbWaveOutDevice;
        private Button btnASIOControlPanel;
        private ComboBox cmbAsioDevice;
        private ComboBox cmbWasapiDevice;
        private ComboBox cmbDirectSoundDevice;
        private TabControl tcSetting;
        private TabPage tpOutput;
        private TabPage tpAbout;
        private TableLayoutPanel tableLayoutPanel;
        private PictureBox logoPictureBox;
        private Label labelProductName;
        private Label labelVersion;
        private Label labelCopyright;
        private Label labelCompanyName;
        private TextBox textBoxDescription;
        private TabPage tpOther;
        private GroupBox gbMIDIKeyboard;
        private GroupBox gbUseChannel;
        private CheckBox cbFM1;
        private CheckBox cbFM2;
        private CheckBox cbFM3;
        private CheckBox cbUseMIDIKeyboard;
        private CheckBox cbFM4;
        private CheckBox cbFM5;
        private CheckBox cbFM6;
        private ComboBox cmbMIDIIN;
        private Label label5;
        private RadioButton rbExclusive;
        private RadioButton rbShare;
        private Label lblLatencyUnit;
        private Label lblLatency;
        private ComboBox cmbLatency;
        private GroupBox groupBox3;
        private Label label13;
        private Label label12;
        private Label label11;
        private TextBox tbLatencyEmu;
        private TextBox tbLatencySCCI;
        private Label label10;
        private GroupBox groupBox5;
        private CheckBox cbDispFrameCounter;
        private CheckBox cbHiyorimiMode;
        private CheckBox cbUseLoopTimes;
        private Label lblLoopTimes;
        private TextBox tbLoopTimes;
        private Button btnOpenSettingFolder;
        private CheckBox cbUseGetInst;
        private Button btnDataPath;
        private TextBox tbDataPath;
        private Label label19;
        private TabPage tpMIDIKBD;
        private ComboBox cmbInstFormat;
        private Label lblInstFormat;
        private Label label30;
        private TextBox tbScreenFrameRate;
        private Label label29;
        private CheckBox cbAutoOpen;
        private ucSettingInstruments ucSI;
        private GroupBox groupBox1;
        private GroupBox groupBox4;
        private CheckBox cbDumpSwitch;
        private GroupBox gbDump;
        private Button btnDumpPath;
        private Label label6;
        private TextBox tbDumpPath;
        private Button btnResetPosition;
        private TabPage tpMIDIExp;
        private CheckBox cbUseMIDIExport;
        private GroupBox gbMIDIExport;
        private CheckBox cbMIDIUseVOPM;
        private GroupBox groupBox6;
        private CheckBox cbMIDIYM2612;
        private CheckBox cbMIDISN76489Sec;
        private CheckBox cbMIDIYM2612Sec;
        private CheckBox cbMIDISN76489;
        private CheckBox cbMIDIYM2151;
        private CheckBox cbMIDIYM2610BSec;
        private CheckBox cbMIDIYM2151Sec;
        private CheckBox cbMIDIYM2610B;
        private CheckBox cbMIDIYM2203;
        private CheckBox cbMIDIYM2608Sec;
        private CheckBox cbMIDIYM2203Sec;
        private CheckBox cbMIDIYM2608;
        private CheckBox cbMIDIPlayless;
        private Button btnMIDIOutputPath;
        private Label lblOutputPath;
        private TextBox tbMIDIOutputPath;
        private CheckBox cbWavSwitch;
        private GroupBox gbWav;
        private Button btnWavPath;
        private Label label7;
        private TextBox tbWavPath;
        private RadioButton rbMONO;
        private RadioButton rbPOLY;
        private GroupBox groupBox7;
        private RadioButton rbFM6;
        private RadioButton rbFM3;
        private RadioButton rbFM5;
        private RadioButton rbFM2;
        private RadioButton rbFM4;
        private RadioButton rbFM1;
        private GroupBox groupBox2;
        private TabPage tpOmake;
        private TextBox tbCCFadeout;
        private TextBox tbCCPause;
        private TextBox tbCCSlow;
        private TextBox tbCCPrevious;
        private TextBox tbCCNext;
        private TextBox tbCCFast;
        private TextBox tbCCStop;
        private TextBox tbCCPlay;
        private TextBox tbCCCopyLog;
        private Label label17;
        private TextBox tbCCDelLog;
        private Label label15;
        private TextBox tbCCChCopy;
        private Label label9;
        private Label label8;
        private PictureBox pictureBox1;
        private PictureBox pictureBox4;
        private PictureBox pictureBox3;
        private PictureBox pictureBox2;
        private PictureBox pictureBox8;
        private PictureBox pictureBox7;
        private PictureBox pictureBox6;
        private PictureBox pictureBox5;
        private Label label14;
        private Button btVST;
        private TextBox tbVST;
        private Button btnUP_A;
        private Button btnSubMIDIout;
        private Button btnDOWN_A;
        private Button btnAddMIDIout;
        private Label label18;
        private DataGridView dgvMIDIoutListA;
        private DataGridView dgvMIDIoutPallet;
        private Label label16;
        private DataGridViewTextBoxColumn clmID;
        private DataGridViewTextBoxColumn clmDeviceName;
        private DataGridViewTextBoxColumn clmManufacturer;
        private DataGridViewTextBoxColumn clmSpacer;
        private TabControl tbcMIDIoutList;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TabPage tabPage3;
        private TabPage tabPage4;
        private Button btnUP_B;
        private Button btnDOWN_B;
        private Button btnUP_C;
        private Button btnDOWN_C;
        private Button btnUP_D;
        private Button btnDOWN_D;
        private Button btnAddVST;
        private DataGridView dgvMIDIoutListB;
        private DataGridView dgvMIDIoutListC;
        private DataGridView dgvMIDIoutListD;
        private TabPage tpNSF;
        private GroupBox groupBox8;
        private CheckBox cbNSFFDSWriteDisable8000;
        private GroupBox groupBox10;
        private CheckBox cbNSFDmc_TriNull;
        private CheckBox cbNSFDmc_TriMute;
        private CheckBox cbNSFDmc_RandomizeNoise;
        private CheckBox cbNSFDmc_DPCMAntiClick;
        private CheckBox cbNSFDmc_EnablePNoise;
        private CheckBox cbNSFDmc_Enable4011;
        private CheckBox cbNSFDmc_NonLinearMixer;
        private CheckBox cbNSFDmc_UnmuteOnReset;
        private GroupBox groupBox12;
        private CheckBox cbNSFN160_Serial;
        private GroupBox groupBox11;
        private CheckBox cbNSFMmc5_PhaseRefresh;
        private CheckBox cbNSFMmc5_NonLinearMixer;
        private GroupBox groupBox9;
        private CheckBox cbNFSNes_DutySwap;
        private CheckBox cbNFSNes_PhaseRefresh;
        private CheckBox cbNFSNes_NonLinearMixer;
        private CheckBox cbNFSNes_UnmuteOnReset;
        private Label label21;
        private Label label20;
        private TextBox tbNSFFds_LPF;
        private CheckBox cbNFSFds_4085Reset;
        private GroupBox groupBox13;
        private Label label22;
        private Button btnSIDCharacter;
        private Button btnSIDBasic;
        private Button btnSIDKernal;
        private TextBox tbSIDCharacter;
        private TextBox tbSIDBasic;
        private TextBox tbSIDKernal;
        private Label label24;
        private Label label23;
        private GroupBox groupBox14;
        private Label label27;
        private Label label26;
        private Label label25;
        private RadioButton rdSIDQ1;
        private RadioButton rdSIDQ3;
        private RadioButton rdSIDQ2;
        private RadioButton rdSIDQ4;
        private Label lblWaitTime;
        private Label label28;
        private ComboBox cmbWaitTime;
        private GroupBox groupBox15;
        private Button btnBeforeSend_Default;
        private TextBox tbBeforeSend_Custom;
        private TextBox tbBeforeSend_XGReset;
        private Label label34;
        private Label label32;
        private TextBox tbBeforeSend_GSReset;
        private Label label33;
        private TextBox tbBeforeSend_GMReset;
        private Label label31;
        private Label label35;
        private RadioButton rbSPPCM;
        private GroupBox groupBox16;
        private ComboBox cmbSPPCMDevice;
        private GroupBox groupBox17;
        private TextBox tbImageExt;
        private TextBox tbMMLExt;
        private TextBox tbTextExt;
        private Label label1;
        private Label label3;
        private Label label2;
        private CheckBox cbInitAlways;
        private TabPage tpBalance;
        private CheckBox cbAutoBalanceUseThis;
        private GroupBox groupBox18;
        private GroupBox groupBox24;
        private GroupBox groupBox21;
        private RadioButton rbAutoBalanceNotSaveSongBalance;
        private RadioButton rbAutoBalanceSamePositionAsSongData;
        private RadioButton rbAutoBalanceSaveSongBalance;
        private GroupBox groupBox22;
        private Label label4;
        private GroupBox groupBox23;
        private GroupBox groupBox19;
        private RadioButton rbAutoBalanceNotLoadSongBalance;
        private RadioButton rbAutoBalanceLoadSongBalance;
        private GroupBox groupBox20;
        private RadioButton rbAutoBalanceNotLoadDriverBalance;
        private RadioButton rbAutoBalanceLoadDriverBalance;
        private GroupBox groupBox25;
        private RadioButton rbAutoBalanceNotSamePositionAsSongData;
        private TabPage tpKeyBoard;
        private RadioButton rbNullDevice;
        private TextBox tbSIDOutputBufferSize;
        private Label label49;
        private Label label51;
        private TabPage tpNuked;
        private GroupBox groupBox26;
        private RadioButton rbNukedOPN2OptionYM2612u;
        private RadioButton rbNukedOPN2OptionYM2612;
        private RadioButton rbNukedOPN2OptionDiscrete;
        private RadioButton rbNukedOPN2OptionASIC;
        private RadioButton rbNukedOPN2OptionASIClp;
        private CheckBox cbEmptyPlayList;
        private CheckBox cbMIDIKeyOnFnum;
        private TrackBar tbOpacity;
        private Label label52;
        private SplitContainer splitContainer1;
        private GroupBox groupBox27;
        private RadioButton rbManualDetect;
        private RadioButton rbAutoDetect;
        private GroupBox groupBox28;
        private RadioButton rbSCCIDetect;
        private RadioButton rbC86ctlDetect;
        private TabControl tabControl1;
        private TabPage tabPage11;
        private TabPage tabPage12;
        private TabPage tabPage13;
        private TabControl tabControl2;
        private TabPage tabPage14;
        private TabControl tabControl3;
        private TabPage tabPage15;
        private TabPage tabPage16;
        private TabPage tabPage17;
        private Label label36;
        private Button btFont;
        private Label label54;
        private Label label53;
        private Label lblFontStyle;
        private Label lblFontSize;
        private Label lblFontName;
        private GroupBox groupBox29;
        private CheckBox cbInfiniteOfflineMode;
        private CheckBox cbMIDIKbdAlwaysTop;
        private CheckBox cbUseRealChip;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewCheckBoxColumn clmIsVST;
        private DataGridViewTextBoxColumn clmFileName;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private DataGridViewComboBoxColumn clmType;
        private DataGridViewComboBoxColumn ClmBeforeSend;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private GroupBox groupBox30;
        private RadioButton rbQueryPerformanceCounter;
        private RadioButton rbDateTime;
        private RadioButton rbStopWatch;
        private GroupBox groupBox31;
        private RadioButton rbLoglevelINFO;
        private RadioButton rbLoglevelDEBUG;
        private RadioButton rbLoglevelTRACE;
        private CheckBox cbLogWarning;
        private DataGridView dgvShortCutKey;
        private DataGridViewTextBoxColumn clmNumber;
        private DataGridViewTextBoxColumn clmFunc;
        private DataGridViewCheckBoxColumn clmShift;
        private DataGridViewCheckBoxColumn clmCtrl;
        private DataGridViewCheckBoxColumn clmAlt;
        private DataGridViewTextBoxColumn clmKey;
        private DataGridViewButtonColumn clmSet;
        private DataGridViewButtonColumn clmClr;
        private DataGridViewTextBoxColumn clmKBDSpacer;
        private Label lblSKKey;
        private Button btnInitializeShortCutKey;
        private CheckBox cbUseSIen;
        private LinkLabel llOpenGithub;
        private CheckBox cbPlayDeviceCB;
        private TabPage tpPMDDotNET;
        private RadioButton rbPMDManual;
        private RadioButton rbPMDAuto;
        private Button btnPMDResetDriverArguments;
        private Label label47;
        private Button btnPMDResetCompilerArhguments;
        private TextBox tbPMDDriverArguments;
        private Label label37;
        private TextBox tbPMDCompilerArguments;
        private GroupBox gbPMDManual;
        private CheckBox cbPMDSetManualVolume;
        private CheckBox cbPMDUsePPZ8;
        private GroupBox groupBox32;
        private RadioButton rbPMD86B;
        private RadioButton rbPMDSpbB;
        private RadioButton rbPMDNrmB;
        private CheckBox cbPMDUsePPSDRV;
        private GroupBox gbPPSDRV;
        private RadioButton rbPMDUsePPSDRVManualFreq;
        private RadioButton rbPMDUsePPSDRVFreqDefault;
        private Label label40;
        private Label label38;
        private TextBox tbPMDPPSDRVManualWait;
        private Label label39;
        private TextBox tbPMDPPSDRVFreq;
        private Button btnPMDPPSDRVManualWait;
        private GroupBox gbPMDSetManualVolume;
        private Label label41;
        private Label label46;
        private TextBox tbPMDVolumeAdpcm;
        private Label label42;
        private TextBox tbPMDVolumeRhythm;
        private Label label43;
        private TextBox tbPMDVolumeSSG;
        private Label label44;
        private TextBox tbPMDVolumeGIMICSSG;
        private Label label45;
        private TextBox tbPMDVolumeFM;
        private GroupBox groupBox33;
        private CheckBox cbRequestCacheClear;
        private TabPage tabPage5;
        private GroupBox groupBox34;
        private CheckBox cbGensSSGEG;
        private CheckBox cbGensDACHPF;
        private CheckBox cbSinWave;
        private CheckBox cbClearHistory;
        private TabPage tpMMLParameter;
        private CheckBox cbDispInstrumentName;
        private Label label48;
        private Label label55;
        private Label label50;
        private ComboBox cmbSampleRate;
        private CheckBox cbChangeEnterCode;
        private TabPage tpExport;
        private CheckBox cbFixedExportPlace;
        private GroupBox gpbFixedExportPlace;
        private Button btnFixedExportPlace;
        private Label label57;
        private TextBox tbFixedExportPlacePath;
        private CheckBox cbAlwaysAskForLoopCount;
        private TabPage tpOther2;
        private CheckBox cbUseScript;
        private CheckBox cbUseMoonDriverDotNET;
        private CheckBox cbUsePMDDotNET;
        private CheckBox cbUseMucomDotNET;
        private TextBox tbUseHistoryBackUp;
        private CheckBox cbUseHistoryBackUp;
        private CheckBox cbDispWarningMessage;
        private TextBox tbTABWidth;
        private Label label56;
        private CheckBox cbHilightOn;
    }
}