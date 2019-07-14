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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmEditor));
            this.imglstIconBar = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // imglstIconBar
            // 
            this.imglstIconBar.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imglstIconBar.ImageStream")));
            this.imglstIconBar.TransparentColor = System.Drawing.Color.Transparent;
            this.imglstIconBar.Images.SetKeyName(0, "Error.png");
            this.imglstIconBar.Images.SetKeyName(1, "Folder.png");
            this.imglstIconBar.Images.SetKeyName(2, "Warning.png");
            this.imglstIconBar.Images.SetKeyName(3, "BreakPoint.png");
            this.imglstIconBar.Images.SetKeyName(4, "BreakPointMute.png");
            this.imglstIconBar.Images.SetKeyName(5, "LineCursor.png");
            // 
            // FrmEditor
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.Document)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmEditor";
            this.Text = "新しいファイル.gwi";
            this.Activated += new System.EventHandler(this.FrmEditor_Activated);
            this.Deactivate += new System.EventHandler(this.FrmEditor_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmEditor_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmEditor_FormClosed);
            this.Shown += new System.EventHandler(this.FrmEditor_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmEditor_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ImageList imglstIconBar;
    }
}
