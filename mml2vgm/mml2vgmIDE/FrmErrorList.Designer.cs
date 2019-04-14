namespace mml2vgmIDE
{
    partial class FrmErrorList
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.ClmEtype = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ClmFileName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ClmLine = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ClmMessage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ClmEtype,
            this.ClmFileName,
            this.ClmLine,
            this.ClmMessage});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.Height = 21;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(582, 155);
            this.dataGridView1.TabIndex = 0;
            // 
            // ClmEtype
            // 
            this.ClmEtype.HeaderText = "";
            this.ClmEtype.Name = "ClmEtype";
            this.ClmEtype.ReadOnly = true;
            this.ClmEtype.Width = 60;
            // 
            // ClmFileName
            // 
            this.ClmFileName.HeaderText = "File name";
            this.ClmFileName.Name = "ClmFileName";
            this.ClmFileName.ReadOnly = true;
            this.ClmFileName.Width = 90;
            // 
            // ClmLine
            // 
            this.ClmLine.HeaderText = "Line";
            this.ClmLine.Name = "ClmLine";
            this.ClmLine.ReadOnly = true;
            this.ClmLine.Width = 90;
            // 
            // ClmMessage
            // 
            this.ClmMessage.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ClmMessage.HeaderText = "Message";
            this.ClmMessage.Name = "ClmMessage";
            this.ClmMessage.ReadOnly = true;
            // 
            // FrmErrorList
            // 
            this.ClientSize = new System.Drawing.Size(582, 155);
            this.Controls.Add(this.dataGridView1);
            this.Name = "FrmErrorList";
            this.Text = "Error list";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmErrorList_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.DataGridViewTextBoxColumn ClmEtype;
        private System.Windows.Forms.DataGridViewTextBoxColumn ClmFileName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ClmLine;
        private System.Windows.Forms.DataGridViewTextBoxColumn ClmMessage;
        public System.Windows.Forms.DataGridView dataGridView1;
    }
}
