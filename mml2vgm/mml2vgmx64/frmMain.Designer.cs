namespace mml2vgmx64
{
    partial class frmMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            splitContainer1 = new SplitContainer();
            dgvResult = new DataGridView();
            clmPartName = new DataGridViewTextBoxColumn();
            clmChip = new DataGridViewTextBoxColumn();
            clmCounter = new DataGridViewTextBoxColumn();
            Column4 = new DataGridViewTextBoxColumn();
            textBox1 = new TextBox();
            toolStripContainer1 = new ToolStripContainer();
            statusStrip1 = new StatusStrip();
            tsslMessage = new ToolStripStatusLabel();
            tspbProgress = new ToolStripProgressBar();
            toolStrip1 = new ToolStrip();
            tsbOpen = new ToolStripButton();
            tsbCompile = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            tsbOnPlay = new ToolStripButton();
            tsbWatcher = new ToolStripButton();
            tsbToVGZ = new ToolStripButton();
            tsbUseCache = new ToolStripButton();
            tsbXGM2ForcePCMMode = new ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvResult).BeginInit();
            toolStripContainer1.BottomToolStripPanel.SuspendLayout();
            toolStripContainer1.ContentPanel.SuspendLayout();
            toolStripContainer1.TopToolStripPanel.SuspendLayout();
            toolStripContainer1.SuspendLayout();
            statusStrip1.SuspendLayout();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(dgvResult);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(textBox1);
            splitContainer1.Size = new Size(720, 472);
            splitContainer1.SplitterDistance = 200;
            splitContainer1.TabIndex = 0;
            // 
            // dgvResult
            // 
            dgvResult.AllowUserToAddRows = false;
            dgvResult.AllowUserToDeleteRows = false;
            dgvResult.AllowUserToResizeRows = false;
            dgvResult.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvResult.Columns.AddRange(new DataGridViewColumn[] { clmPartName, clmChip, clmCounter, Column4 });
            dgvResult.Dock = DockStyle.Fill;
            dgvResult.Location = new Point(0, 0);
            dgvResult.Name = "dgvResult";
            dgvResult.RowHeadersVisible = false;
            dgvResult.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvResult.Size = new Size(200, 472);
            dgvResult.TabIndex = 0;
            // 
            // clmPartName
            // 
            clmPartName.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            clmPartName.HeaderText = "Part";
            clmPartName.Name = "clmPartName";
            clmPartName.ReadOnly = true;
            clmPartName.Width = 53;
            // 
            // clmChip
            // 
            clmChip.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            clmChip.HeaderText = "Chip";
            clmChip.Name = "clmChip";
            clmChip.ReadOnly = true;
            clmChip.Width = 56;
            // 
            // clmCounter
            // 
            clmCounter.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            clmCounter.HeaderText = "Counter";
            clmCounter.Name = "clmCounter";
            clmCounter.ReadOnly = true;
            clmCounter.Width = 74;
            // 
            // Column4
            // 
            Column4.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Column4.HeaderText = "";
            Column4.Name = "Column4";
            Column4.ReadOnly = true;
            // 
            // textBox1
            // 
            textBox1.Dock = DockStyle.Fill;
            textBox1.Location = new Point(0, 0);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.ScrollBars = ScrollBars.Both;
            textBox1.Size = new Size(516, 472);
            textBox1.TabIndex = 0;
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.BottomToolStripPanel
            // 
            toolStripContainer1.BottomToolStripPanel.Controls.Add(statusStrip1);
            // 
            // toolStripContainer1.ContentPanel
            // 
            toolStripContainer1.ContentPanel.Controls.Add(splitContainer1);
            toolStripContainer1.ContentPanel.Size = new Size(720, 472);
            toolStripContainer1.Dock = DockStyle.Fill;
            toolStripContainer1.Location = new Point(0, 0);
            toolStripContainer1.Name = "toolStripContainer1";
            toolStripContainer1.Size = new Size(720, 519);
            toolStripContainer1.TabIndex = 0;
            toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            toolStripContainer1.TopToolStripPanel.Controls.Add(toolStrip1);
            // 
            // statusStrip1
            // 
            statusStrip1.Dock = DockStyle.None;
            statusStrip1.Items.AddRange(new ToolStripItem[] { tsslMessage, tspbProgress });
            statusStrip1.Location = new Point(0, 0);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(720, 22);
            statusStrip1.TabIndex = 0;
            // 
            // tsslMessage
            // 
            tsslMessage.Name = "tsslMessage";
            tsslMessage.Size = new Size(0, 17);
            // 
            // tspbProgress
            // 
            tspbProgress.Name = "tspbProgress";
            tspbProgress.Size = new Size(100, 16);
            tspbProgress.Visible = false;
            // 
            // toolStrip1
            // 
            toolStrip1.Dock = DockStyle.None;
            toolStrip1.Items.AddRange(new ToolStripItem[] { tsbOpen, tsbCompile, toolStripSeparator1, tsbOnPlay, tsbWatcher, tsbToVGZ, tsbUseCache, tsbXGM2ForcePCMMode });
            toolStrip1.Location = new Point(3, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(210, 25);
            toolStrip1.TabIndex = 0;
            // 
            // tsbOpen
            // 
            tsbOpen.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbOpen.Image = (Image)resources.GetObject("tsbOpen.Image");
            tsbOpen.ImageTransparentColor = Color.Magenta;
            tsbOpen.Name = "tsbOpen";
            tsbOpen.Size = new Size(23, 22);
            tsbOpen.Text = "toolStripButton1";
            tsbOpen.ToolTipText = "Open and Compile";
            tsbOpen.Click += tsbOpen_Click;
            // 
            // tsbCompile
            // 
            tsbCompile.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbCompile.Image = (Image)resources.GetObject("tsbCompile.Image");
            tsbCompile.ImageTransparentColor = Color.Magenta;
            tsbCompile.Name = "tsbCompile";
            tsbCompile.Size = new Size(23, 22);
            tsbCompile.Text = "toolStripButton2";
            tsbCompile.ToolTipText = "Compile";
            tsbCompile.Click += tsbCompile_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 25);
            // 
            // tsbOnPlay
            // 
            tsbOnPlay.CheckOnClick = true;
            tsbOnPlay.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbOnPlay.Image = (Image)resources.GetObject("tsbOnPlay.Image");
            tsbOnPlay.ImageTransparentColor = Color.Magenta;
            tsbOnPlay.Name = "tsbOnPlay";
            tsbOnPlay.Size = new Size(23, 22);
            tsbOnPlay.Text = "toolStripButton3";
            tsbOnPlay.ToolTipText = "Play After Compile";
            // 
            // tsbWatcher
            // 
            tsbWatcher.CheckOnClick = true;
            tsbWatcher.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbWatcher.Image = (Image)resources.GetObject("tsbWatcher.Image");
            tsbWatcher.ImageTransparentColor = Color.Magenta;
            tsbWatcher.Name = "tsbWatcher";
            tsbWatcher.Size = new Size(23, 22);
            tsbWatcher.Text = "toolStripButton4";
            tsbWatcher.ToolTipText = "Watch to gwi file";
            tsbWatcher.CheckedChanged += tsbWatcher_CheckedChanged;
            // 
            // tsbToVGZ
            // 
            tsbToVGZ.CheckOnClick = true;
            tsbToVGZ.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbToVGZ.Image = (Image)resources.GetObject("tsbToVGZ.Image");
            tsbToVGZ.ImageTransparentColor = Color.Magenta;
            tsbToVGZ.Name = "tsbToVGZ";
            tsbToVGZ.Size = new Size(23, 22);
            tsbToVGZ.Text = "toolStripButton5";
            tsbToVGZ.ToolTipText = "Compress to VGZ";
            // 
            // tsbUseCache
            // 
            tsbUseCache.CheckOnClick = true;
            tsbUseCache.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbUseCache.Image = (Image)resources.GetObject("tsbUseCache.Image");
            tsbUseCache.ImageTransparentColor = Color.Magenta;
            tsbUseCache.Name = "tsbUseCache";
            tsbUseCache.Size = new Size(23, 22);
            tsbUseCache.Text = "toolStripButton6";
            tsbUseCache.ToolTipText = "Use PCM Cache";
            // 
            // tsbXGM2ForcePCMMode
            // 
            tsbXGM2ForcePCMMode.CheckOnClick = true;
            tsbXGM2ForcePCMMode.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbXGM2ForcePCMMode.Image = (Image)resources.GetObject("tsbXGM2ForcePCMMode.Image");
            tsbXGM2ForcePCMMode.ImageTransparentColor = Color.Magenta;
            tsbXGM2ForcePCMMode.Name = "tsbXGM2ForcePCMMode";
            tsbXGM2ForcePCMMode.Size = new Size(23, 22);
            tsbXGM2ForcePCMMode.Text = "toolStripButton1";
            tsbXGM2ForcePCMMode.ToolTipText = "XGM2 Force PCM mode";
            // 
            // frmMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(720, 519);
            Controls.Add(toolStripContainer1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(320, 240);
            Name = "frmMain";
            Text = "mml2vgm(x64)";
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvResult).EndInit();
            toolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
            toolStripContainer1.BottomToolStripPanel.PerformLayout();
            toolStripContainer1.ContentPanel.ResumeLayout(false);
            toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            toolStripContainer1.TopToolStripPanel.PerformLayout();
            toolStripContainer1.ResumeLayout(false);
            toolStripContainer1.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainer1;
        private ToolStripContainer toolStripContainer1;
        private StatusStrip statusStrip1;
        private ToolStrip toolStrip1;
        private DataGridView dgvResult;
        private TextBox textBox1;
        private ToolStripButton tsbOpen;
        private ToolStripButton tsbCompile;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton tsbOnPlay;
        private ToolStripButton tsbWatcher;
        private ToolStripButton tsbToVGZ;
        private ToolStripButton tsbUseCache;
        private ToolStripProgressBar tspbProgress;
        private DataGridViewTextBoxColumn clmPartName;
        private DataGridViewTextBoxColumn clmChip;
        private DataGridViewTextBoxColumn clmCounter;
        private DataGridViewTextBoxColumn Column4;
        private ToolStripStatusLabel tsslMessage;
        private ToolStripButton tsbXGM2ForcePCMMode;
    }
}
