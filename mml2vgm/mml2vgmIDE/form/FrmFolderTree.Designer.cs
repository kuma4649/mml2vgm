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
            this.imgList = new System.Windows.Forms.ImageList(this.components);
            this.cmsMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.TsmiOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tvFolderTree = new CustomControl.MultiSelectTreeView();
            this.cmsMenu.SuspendLayout();
            this.SuspendLayout();
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
            this.TsmiOpen,
            this.TsmiDelete,
            this.toolStripSeparator1});
            this.cmsMenu.Name = "cmsMenu";
            this.cmsMenu.ShowImageMargin = false;
            this.cmsMenu.Size = new System.Drawing.Size(156, 76);
            // 
            // TsmiOpen
            // 
            this.TsmiOpen.Name = "TsmiOpen";
            this.TsmiOpen.Size = new System.Drawing.Size(155, 22);
            this.TsmiOpen.Text = "開く";
            this.TsmiOpen.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TsmiOpen_MouseUp);
            // 
            // TsmiDelete
            // 
            this.TsmiDelete.Name = "TsmiDelete";
            this.TsmiDelete.Size = new System.Drawing.Size(155, 22);
            this.TsmiDelete.Text = "削除";
            this.TsmiDelete.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TsmiDelete_MouseUp);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(152, 6);
            // 
            // tvFolderTree
            // 
            this.tvFolderTree.AllowDrop = true;
            this.tvFolderTree.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.tvFolderTree.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tvFolderTree.CheckBoxes = true;
            this.tvFolderTree.CheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(160)))));
            this.tvFolderTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvFolderTree.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawAll;
            this.tvFolderTree.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.tvFolderTree.HotColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(90)))));
            this.tvFolderTree.ImageIndex = 0;
            this.tvFolderTree.ImageList = this.imgList;
            this.tvFolderTree.Location = new System.Drawing.Point(0, 0);
            this.tvFolderTree.Name = "tvFolderTree";
            this.tvFolderTree.SelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(180)))));
            this.tvFolderTree.SelectedImageIndex = 0;
            this.tvFolderTree.Size = new System.Drawing.Size(289, 380);
            this.tvFolderTree.TabIndex = 0;
            this.tvFolderTree.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.TreeView1_BeforeExpand);
            this.tvFolderTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TvFolderTree_AfterSelect);
            this.tvFolderTree.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TvFolderTree_NodeMouseClick);
            this.tvFolderTree.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TreeView1_NodeMouseDoubleClick);
            this.tvFolderTree.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TvFolderTree_KeyPress);
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
            this.Load += new System.EventHandler(this.FrmFolderTree_Load);
            this.cmsMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public CustomControl.MultiSelectTreeView tvFolderTree;
        private System.Windows.Forms.ImageList imgList;
        private System.Windows.Forms.ContextMenuStrip cmsMenu;
        private System.Windows.Forms.ToolStripMenuItem TsmiOpen;
        private System.Windows.Forms.ToolStripMenuItem TsmiDelete;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    }
}
