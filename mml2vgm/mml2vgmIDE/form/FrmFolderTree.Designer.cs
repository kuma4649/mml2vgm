namespace mml2vgmIDE
{
    partial class FrmFolderTree
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmFolderTree));
            this.tvFolderTree = new System.Windows.Forms.TreeView();
            this.imgList = new System.Windows.Forms.ImageList(this.components);
            this.cmsMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.TsmiOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // tvFolderTree
            // 
            this.tvFolderTree.AllowDrop = true;
            this.tvFolderTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvFolderTree.ImageIndex = 0;
            this.tvFolderTree.ImageList = this.imgList;
            this.tvFolderTree.Location = new System.Drawing.Point(0, 0);
            this.tvFolderTree.Name = "tvFolderTree";
            this.tvFolderTree.SelectedImageIndex = 0;
            this.tvFolderTree.Size = new System.Drawing.Size(289, 380);
            this.tvFolderTree.TabIndex = 0;
            this.tvFolderTree.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.TreeView1_BeforeExpand);
            this.tvFolderTree.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TvFolderTree_NodeMouseClick);
            this.tvFolderTree.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TreeView1_NodeMouseDoubleClick);
            this.tvFolderTree.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TvFolderTree_KeyPress);
            // 
            // imgList
            // 
            this.imgList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgList.ImageStream")));
            this.imgList.TransparentColor = System.Drawing.Color.Transparent;
            this.imgList.Images.SetKeyName(0, "File.png");
            this.imgList.Images.SetKeyName(1, "Folder.png");
            // 
            // cmsMenu
            // 
            this.cmsMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TsmiOpen});
            this.cmsMenu.Name = "cmsMenu";
            this.cmsMenu.Size = new System.Drawing.Size(94, 26);
            // 
            // TsmiOpen
            // 
            this.TsmiOpen.Name = "TsmiOpen";
            this.TsmiOpen.Size = new System.Drawing.Size(180, 22);
            this.TsmiOpen.Text = "開く";
            this.TsmiOpen.Click += new System.EventHandler(this.TsmiOpen_Click);
            // 
            // FrmFolderTree
            // 
            this.ClientSize = new System.Drawing.Size(289, 380);
            this.Controls.Add(this.tvFolderTree);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmFolderTree";
            this.Text = "Folder";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmFolderTree_FormClosing);
            this.cmsMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.TreeView tvFolderTree;
        private System.Windows.Forms.ImageList imgList;
        private System.Windows.Forms.ContextMenuStrip cmsMenu;
        private System.Windows.Forms.ToolStripMenuItem TsmiOpen;
    }
}
