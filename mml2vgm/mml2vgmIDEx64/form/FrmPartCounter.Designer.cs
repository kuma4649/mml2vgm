namespace mml2vgmIDEx64
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmPartCounter));
            this.dgvPartCounter = new System.Windows.Forms.DataGridView();
            this.cmsMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.ClmChipIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ClmMute = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ClmSolo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ClmPush = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPartCounter)).BeginInit();
            this.cmsMenu.SuspendLayout();
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
            this.dgvPartCounter.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.MenuHighlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvPartCounter.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvPartCounter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvPartCounter.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvPartCounter.EnableHeadersVisualStyles = false;
            this.dgvPartCounter.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(180)))));
            this.dgvPartCounter.Location = new System.Drawing.Point(0, 0);
            this.dgvPartCounter.MultiSelect = false;
            this.dgvPartCounter.Name = "dgvPartCounter";
            this.dgvPartCounter.ReadOnly = true;
            this.dgvPartCounter.RowHeadersVisible = false;
            this.dgvPartCounter.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvPartCounter.RowTemplate.Height = 21;
            this.dgvPartCounter.Size = new System.Drawing.Size(1088, 259);
            this.dgvPartCounter.TabIndex = 0;
            this.dgvPartCounter.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvPartCounter_CellClick);
            this.dgvPartCounter.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DgvPartCounter_CellMouseClick);
            // 
            // cmsMenu
            // 
            this.cmsMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.testToolStripMenuItem});
            this.cmsMenu.Name = "cmsMenu";
            this.cmsMenu.Size = new System.Drawing.Size(94, 26);
            this.cmsMenu.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.CmsMenu_PreviewKeyDown);
            // 
            // testToolStripMenuItem
            // 
            this.testToolStripMenuItem.Name = "testToolStripMenuItem";
            this.testToolStripMenuItem.Size = new System.Drawing.Size(93, 22);
            this.testToolStripMenuItem.Text = "test";
            // 
            // timer
            // 
            this.timer.Interval = 10;
            this.timer.Tick += new System.EventHandler(this.Timer_Tick);
            // 
            // ClmChipIndex
            // 
            this.ClmChipIndex.HeaderText = "ChipIndex";
            this.ClmChipIndex.Name = "ClmChipIndex";
            // 
            // ClmMute
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.ClmMute.DefaultCellStyle = dataGridViewCellStyle3;
            this.ClmMute.HeaderText = "M";
            this.ClmMute.Name = "ClmMute";
            this.ClmMute.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ClmSolo
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.ClmSolo.DefaultCellStyle = dataGridViewCellStyle4;
            this.ClmSolo.HeaderText = "S";
            this.ClmSolo.Name = "ClmSolo";
            this.ClmSolo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ClmPush
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.ClmPush.DefaultCellStyle = dataGridViewCellStyle5;
            this.ClmPush.HeaderText = "P";
            this.ClmPush.Name = "ClmPush";
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
            this.cmsMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.DataGridView dgvPartCounter;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.ContextMenuStrip cmsMenu;
        private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn ClmChipIndex;
        private System.Windows.Forms.DataGridViewTextBoxColumn ClmMute;
        private System.Windows.Forms.DataGridViewTextBoxColumn ClmSolo;
        private System.Windows.Forms.DataGridViewTextBoxColumn ClmPush;
    }
}
