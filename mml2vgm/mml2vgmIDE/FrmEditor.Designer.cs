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
            this.azukiControl = new Sgry.Azuki.WinForms.AzukiControl();
            this.SuspendLayout();
            // 
            // azukiControl
            // 
            this.azukiControl.BackColor = System.Drawing.Color.White;
            this.azukiControl.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.azukiControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.azukiControl.DrawingOption = ((Sgry.Azuki.DrawingOption)((((((((Sgry.Azuki.DrawingOption.DrawsFullWidthSpace | Sgry.Azuki.DrawingOption.DrawsTab) 
            | Sgry.Azuki.DrawingOption.DrawsEol) 
            | Sgry.Azuki.DrawingOption.HighlightCurrentLine) 
            | Sgry.Azuki.DrawingOption.ShowsLineNumber) 
            | Sgry.Azuki.DrawingOption.ShowsHRuler) 
            | Sgry.Azuki.DrawingOption.ShowsDirtBar) 
            | Sgry.Azuki.DrawingOption.HighlightsMatchedBracket)));
            this.azukiControl.FirstVisibleLine = 0;
            this.azukiControl.Font = new System.Drawing.Font("Consolas", 9F);
            fontInfo1.Name = "Consolas";
            fontInfo1.Size = 9;
            fontInfo1.Style = System.Drawing.FontStyle.Regular;
            this.azukiControl.FontInfo = fontInfo1;
            this.azukiControl.ForeColor = System.Drawing.Color.Black;
            this.azukiControl.Location = new System.Drawing.Point(0, 0);
            this.azukiControl.Name = "azukiControl";
            this.azukiControl.ScrollPos = new System.Drawing.Point(0, 0);
            this.azukiControl.ShowsHRuler = true;
            this.azukiControl.Size = new System.Drawing.Size(284, 261);
            this.azukiControl.TabIndex = 0;
            this.azukiControl.Text = "azukiControl1";
            this.azukiControl.ViewWidth = 4136;
            this.azukiControl.VScroll += new System.EventHandler(this.AzukiControl_CancelSien);
            this.azukiControl.HScroll += new System.EventHandler(this.AzukiControl_CancelSien);
            this.azukiControl.TextChanged += new System.EventHandler(this.AzukiControl_TextChanged);
            this.azukiControl.Click += new System.EventHandler(this.AzukiControl_CancelSien);
            this.azukiControl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.AzukiControl_KeyDown);
            // 
            // FrmEditor
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.azukiControl);
            this.Name = "FrmEditor";
            this.Text = "新しいファイル.gwi";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmEditor_FormClosed);
            this.ResumeLayout(false);

        }

        #endregion

        public Sgry.Azuki.WinForms.AzukiControl azukiControl;
    }
}
