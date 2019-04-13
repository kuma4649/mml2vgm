namespace mml2vgmIDE
{
    partial class FrmEditor
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
            Sgry.Azuki.FontInfo fontInfo1 = new Sgry.Azuki.FontInfo();
            this.azukiControl1 = new Sgry.Azuki.WinForms.AzukiControl();
            this.SuspendLayout();
            // 
            // azukiControl1
            // 
            this.azukiControl1.BackColor = System.Drawing.Color.White;
            this.azukiControl1.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.azukiControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.azukiControl1.DrawingOption = ((Sgry.Azuki.DrawingOption)((((((((Sgry.Azuki.DrawingOption.DrawsFullWidthSpace | Sgry.Azuki.DrawingOption.DrawsTab) 
            | Sgry.Azuki.DrawingOption.DrawsEol) 
            | Sgry.Azuki.DrawingOption.HighlightCurrentLine) 
            | Sgry.Azuki.DrawingOption.ShowsLineNumber) 
            | Sgry.Azuki.DrawingOption.ShowsHRuler) 
            | Sgry.Azuki.DrawingOption.ShowsDirtBar) 
            | Sgry.Azuki.DrawingOption.HighlightsMatchedBracket)));
            this.azukiControl1.FirstVisibleLine = 0;
            this.azukiControl1.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            fontInfo1.Name = "Consolas";
            fontInfo1.Size = 9;
            fontInfo1.Style = System.Drawing.FontStyle.Regular;
            this.azukiControl1.FontInfo = fontInfo1;
            this.azukiControl1.ForeColor = System.Drawing.Color.Black;
            this.azukiControl1.Location = new System.Drawing.Point(0, 0);
            this.azukiControl1.Name = "azukiControl1";
            this.azukiControl1.ScrollPos = new System.Drawing.Point(0, 0);
            this.azukiControl1.ShowsHRuler = true;
            this.azukiControl1.Size = new System.Drawing.Size(284, 261);
            this.azukiControl1.TabIndex = 0;
            this.azukiControl1.Text = "azukiControl1";
            this.azukiControl1.ViewWidth = 4136;
            // 
            // FrmEditor
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.azukiControl1);
            this.Name = "FrmEditor";
            this.Text = "新しいファイル.gwi";
            this.ResumeLayout(false);

        }

        #endregion

        public Sgry.Azuki.WinForms.AzukiControl azukiControl1;
    }
}
