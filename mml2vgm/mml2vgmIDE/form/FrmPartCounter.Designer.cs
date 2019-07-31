namespace mml2vgmIDE
{
    partial class FrmPartCounter
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmPartCounter));
            this.dgvPartCounter = new System.Windows.Forms.DataGridView();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.ClmPartNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ClmIsSecondary = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ClmPart = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ClmChip = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ClmCounter = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ClmInstrument = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ClmEnvelope = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ClmVolume = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ClmPan = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ClmNote = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ClmLength = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ClmSpacer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPartCounter)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvPartCounter
            // 
            this.dgvPartCounter.AllowUserToAddRows = false;
            this.dgvPartCounter.AllowUserToDeleteRows = false;
            this.dgvPartCounter.AllowUserToOrderColumns = true;
            this.dgvPartCounter.AllowUserToResizeRows = false;
            this.dgvPartCounter.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(160)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(180)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvPartCounter.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvPartCounter.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPartCounter.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ClmPartNumber,
            this.ClmIsSecondary,
            this.ClmPart,
            this.ClmChip,
            this.ClmCounter,
            this.ClmInstrument,
            this.ClmEnvelope,
            this.ClmVolume,
            this.ClmPan,
            this.ClmNote,
            this.ClmLength,
            this.ClmSpacer});
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle10.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle10.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(180)))));
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvPartCounter.DefaultCellStyle = dataGridViewCellStyle10;
            this.dgvPartCounter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvPartCounter.EnableHeadersVisualStyles = false;
            this.dgvPartCounter.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(180)))));
            this.dgvPartCounter.Location = new System.Drawing.Point(0, 0);
            this.dgvPartCounter.Name = "dgvPartCounter";
            this.dgvPartCounter.RowHeadersVisible = false;
            this.dgvPartCounter.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvPartCounter.RowTemplate.Height = 21;
            this.dgvPartCounter.Size = new System.Drawing.Size(1088, 259);
            this.dgvPartCounter.TabIndex = 0;
            // 
            // timer
            // 
            this.timer.Interval = 10;
            this.timer.Tick += new System.EventHandler(this.Timer_Tick);
            // 
            // ClmPartNumber
            // 
            this.ClmPartNumber.HeaderText = "PartNumber";
            this.ClmPartNumber.Name = "ClmPartNumber";
            this.ClmPartNumber.Visible = false;
            // 
            // ClmIsSecondary
            // 
            this.ClmIsSecondary.HeaderText = "IsSecondary";
            this.ClmIsSecondary.Name = "ClmIsSecondary";
            this.ClmIsSecondary.Visible = false;
            // 
            // ClmPart
            // 
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ClmPart.DefaultCellStyle = dataGridViewCellStyle2;
            this.ClmPart.HeaderText = "Part";
            this.ClmPart.Name = "ClmPart";
            this.ClmPart.ReadOnly = true;
            this.ClmPart.Width = 50;
            // 
            // ClmChip
            // 
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ClmChip.DefaultCellStyle = dataGridViewCellStyle3;
            this.ClmChip.HeaderText = "Chip";
            this.ClmChip.Name = "ClmChip";
            this.ClmChip.ReadOnly = true;
            // 
            // ClmCounter
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ClmCounter.DefaultCellStyle = dataGridViewCellStyle4;
            this.ClmCounter.HeaderText = "Counter";
            this.ClmCounter.Name = "ClmCounter";
            this.ClmCounter.ReadOnly = true;
            // 
            // ClmInstrument
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Consolas", 9F);
            this.ClmInstrument.DefaultCellStyle = dataGridViewCellStyle5;
            this.ClmInstrument.HeaderText = "Instrument";
            this.ClmInstrument.Name = "ClmInstrument";
            this.ClmInstrument.ReadOnly = true;
            // 
            // ClmEnvelope
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Consolas", 9F);
            this.ClmEnvelope.DefaultCellStyle = dataGridViewCellStyle6;
            this.ClmEnvelope.HeaderText = "Envelope";
            this.ClmEnvelope.Name = "ClmEnvelope";
            this.ClmEnvelope.ReadOnly = true;
            // 
            // ClmVolume
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Consolas", 9F);
            this.ClmVolume.DefaultCellStyle = dataGridViewCellStyle7;
            this.ClmVolume.HeaderText = "Volume";
            this.ClmVolume.Name = "ClmVolume";
            this.ClmVolume.ReadOnly = true;
            // 
            // ClmPan
            // 
            this.ClmPan.HeaderText = "Pan";
            this.ClmPan.Name = "ClmPan";
            this.ClmPan.ReadOnly = true;
            // 
            // ClmNote
            // 
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Consolas", 9F);
            this.ClmNote.DefaultCellStyle = dataGridViewCellStyle8;
            this.ClmNote.HeaderText = "Note";
            this.ClmNote.Name = "ClmNote";
            this.ClmNote.ReadOnly = true;
            // 
            // ClmLength
            // 
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Consolas", 9F);
            this.ClmLength.DefaultCellStyle = dataGridViewCellStyle9;
            this.ClmLength.HeaderText = "Length(#)";
            this.ClmLength.Name = "ClmLength";
            this.ClmLength.ReadOnly = true;
            // 
            // ClmSpacer
            // 
            this.ClmSpacer.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ClmSpacer.HeaderText = "";
            this.ClmSpacer.Name = "ClmSpacer";
            this.ClmSpacer.ReadOnly = true;
            this.ClmSpacer.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // FrmPartCounter
            // 
            this.AllowDrop = true;
            this.ClientSize = new System.Drawing.Size(1088, 259);
            this.Controls.Add(this.dgvPartCounter);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmPartCounter";
            this.Text = "Part counter";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmPartCounter_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmPartCounter_FormClosed);
            this.DragOver += new System.Windows.Forms.DragEventHandler(this.FrmPartCounter_DragOver);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPartCounter)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.DataGridView dgvPartCounter;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.DataGridViewTextBoxColumn ClmPartNumber;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ClmIsSecondary;
        private System.Windows.Forms.DataGridViewTextBoxColumn ClmPart;
        private System.Windows.Forms.DataGridViewTextBoxColumn ClmChip;
        private System.Windows.Forms.DataGridViewTextBoxColumn ClmCounter;
        private System.Windows.Forms.DataGridViewTextBoxColumn ClmInstrument;
        private System.Windows.Forms.DataGridViewTextBoxColumn ClmEnvelope;
        private System.Windows.Forms.DataGridViewTextBoxColumn ClmVolume;
        private System.Windows.Forms.DataGridViewTextBoxColumn ClmPan;
        private System.Windows.Forms.DataGridViewTextBoxColumn ClmNote;
        private System.Windows.Forms.DataGridViewTextBoxColumn ClmLength;
        private System.Windows.Forms.DataGridViewTextBoxColumn ClmSpacer;
    }
}
