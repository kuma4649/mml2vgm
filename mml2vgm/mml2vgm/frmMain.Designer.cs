namespace mml2vgm
{
    partial class frmMain
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
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle6 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle5 = new DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            splitContainer1 = new SplitContainer();
            dgvResult = new DataGridView();
            clmPartName = new DataGridViewTextBoxColumn();
            clmChip = new DataGridViewTextBoxColumn();
            clmCounter = new DataGridViewTextBoxColumn();
            clmSpacer = new DataGridViewTextBoxColumn();
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
            splitContainer1.Margin = new Padding(4, 4, 4, 4);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(dgvResult);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(textBox1);
            splitContainer1.Size = new Size(840, 602);
            splitContainer1.SplitterDistance = 233;
            splitContainer1.SplitterWidth = 5;
            splitContainer1.TabIndex = 0;
            // 
            // dgvResult
            // 
            dgvResult.AllowUserToAddRows = false;
            dgvResult.AllowUserToDeleteRows = false;
            dgvResult.AllowUserToResizeRows = false;
            dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = SystemColors.Control;
            dataGridViewCellStyle4.Font = new Font("Consolas", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            dataGridViewCellStyle4.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = DataGridViewTriState.True;
            dgvResult.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            dgvResult.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvResult.Columns.AddRange(new DataGridViewColumn[] { clmPartName, clmChip, clmCounter, clmSpacer });
            dataGridViewCellStyle6.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = SystemColors.Window;
            dataGridViewCellStyle6.Font = new Font("Consolas", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            dataGridViewCellStyle6.ForeColor = Color.Navy;
            dataGridViewCellStyle6.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = DataGridViewTriState.False;
            dgvResult.DefaultCellStyle = dataGridViewCellStyle6;
            dgvResult.Dock = DockStyle.Fill;
            dgvResult.Location = new Point(0, 0);
            dgvResult.Margin = new Padding(4, 4, 4, 4);
            dgvResult.Name = "dgvResult";
            dgvResult.RowHeadersVisible = false;
            dgvResult.RowTemplate.Height = 21;
            dgvResult.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvResult.Size = new Size(233, 602);
            dgvResult.TabIndex = 0;
            // 
            // clmPartName
            // 
            clmPartName.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            clmPartName.HeaderText = "Part";
            clmPartName.Name = "clmPartName";
            clmPartName.ReadOnly = true;
            clmPartName.Width = 56;
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
            dataGridViewCellStyle5.Alignment = DataGridViewContentAlignment.MiddleRight;
            clmCounter.DefaultCellStyle = dataGridViewCellStyle5;
            clmCounter.HeaderText = "Counter";
            clmCounter.Name = "clmCounter";
            clmCounter.ReadOnly = true;
            clmCounter.Width = 74;
            // 
            // clmSpacer
            // 
            clmSpacer.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            clmSpacer.HeaderText = "";
            clmSpacer.Name = "clmSpacer";
            clmSpacer.ReadOnly = true;
            // 
            // textBox1
            // 
            textBox1.Dock = DockStyle.Fill;
            textBox1.Font = new Font("Consolas", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBox1.Location = new Point(0, 0);
            textBox1.Margin = new Padding(4, 4, 4, 4);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.ScrollBars = ScrollBars.Both;
            textBox1.Size = new Size(602, 602);
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
            toolStripContainer1.ContentPanel.Margin = new Padding(4, 4, 4, 4);
            toolStripContainer1.ContentPanel.Size = new Size(840, 602);
            toolStripContainer1.Dock = DockStyle.Fill;
            toolStripContainer1.Location = new Point(0, 0);
            toolStripContainer1.Margin = new Padding(4, 4, 4, 4);
            toolStripContainer1.Name = "toolStripContainer1";
            toolStripContainer1.Size = new Size(840, 649);
            toolStripContainer1.TabIndex = 1;
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
            statusStrip1.Size = new Size(840, 22);
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
            toolStrip1.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip1.Items.AddRange(new ToolStripItem[] { tsbOpen, tsbCompile, toolStripSeparator1, tsbOnPlay, tsbWatcher, tsbToVGZ, tsbUseCache });
            toolStrip1.Location = new Point(3, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(147, 25);
            toolStrip1.TabIndex = 0;
            // 
            // tsbOpen
            // 
            tsbOpen.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbOpen.Image = Properties.Resources.icon1;
            tsbOpen.ImageTransparentColor = Color.Magenta;
            tsbOpen.Name = "tsbOpen";
            tsbOpen.Size = new Size(23, 22);
            tsbOpen.Text = "Open and Compile";
            tsbOpen.Click += tsbOpen_Click;
            // 
            // tsbCompile
            // 
            tsbCompile.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbCompile.Image = Properties.Resources.icon2;
            tsbCompile.ImageTransparentColor = Color.Magenta;
            tsbCompile.Name = "tsbCompile";
            tsbCompile.Size = new Size(23, 22);
            tsbCompile.Text = "Compile";
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
            tsbOnPlay.Image = Properties.Resources.icon3;
            tsbOnPlay.ImageTransparentColor = Color.Magenta;
            tsbOnPlay.Name = "tsbOnPlay";
            tsbOnPlay.Size = new Size(23, 22);
            tsbOnPlay.Text = "Play After Compile";
            // 
            // tsbWatcher
            // 
            tsbWatcher.CheckOnClick = true;
            tsbWatcher.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbWatcher.Image = Properties.Resources.icon4;
            tsbWatcher.ImageTransparentColor = Color.Magenta;
            tsbWatcher.Name = "tsbWatcher";
            tsbWatcher.Size = new Size(23, 22);
            tsbWatcher.Text = "Watch to gwi file";
            tsbWatcher.CheckedChanged += tsbWatcher_CheckedChanged;
            // 
            // tsbToVGZ
            // 
            tsbToVGZ.CheckOnClick = true;
            tsbToVGZ.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbToVGZ.Image = Properties.Resources.icon5;
            tsbToVGZ.ImageTransparentColor = Color.Magenta;
            tsbToVGZ.Name = "tsbToVGZ";
            tsbToVGZ.Size = new Size(23, 22);
            tsbToVGZ.Text = "Compress to VGZ";
            // 
            // tsbUseCache
            // 
            tsbUseCache.CheckOnClick = true;
            tsbUseCache.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbUseCache.Image = Properties.Resources.icon6;
            tsbUseCache.ImageTransparentColor = Color.Magenta;
            tsbUseCache.Name = "tsbUseCache";
            tsbUseCache.Size = new Size(23, 22);
            tsbUseCache.Text = "Use PCM Cache";
            tsbUseCache.ToolTipText = "Use PCM Cache";
            // 
            // frmMain
            // 
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(840, 649);
            Controls.Add(toolStripContainer1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 4, 4, 4);
            MinimumSize = new Size(371, 290);
            Name = "frmMain";
            Text = "mml2vgm";
            Shown += frmMain_Shown;
            DragDrop += FrmMain_DragDrop;
            DragEnter += FrmMain_DragEnter;
            KeyDown += FrmMain_KeyDown;
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

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView dgvResult;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsbCompile;
        private System.Windows.Forms.ToolStripButton tsbOpen;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tsslMessage;
        private System.Windows.Forms.ToolStripProgressBar tspbProgress;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tsbOnPlay;
        private System.Windows.Forms.ToolStripButton tsbWatcher;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmPartName;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmChip;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmCounter;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmSpacer;
        private System.Windows.Forms.ToolStripButton tsbToVGZ;
        private System.Windows.Forms.ToolStripButton tsbUseCache;
    }
}