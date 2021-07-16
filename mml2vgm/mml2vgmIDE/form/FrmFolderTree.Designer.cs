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
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbReload = new System.Windows.Forms.ToolStripButton();
            this.tsddbFilter = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsmiFilterAll = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiFilterFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiFilterFile = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiFilterGwi = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiFilterWav = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiFilterMuc = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiFilterMml = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsMenu.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
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
            this.cmsMenu.Size = new System.Drawing.Size(74, 54);
            // 
            // TsmiOpen
            // 
            this.TsmiOpen.Name = "TsmiOpen";
            this.TsmiOpen.Size = new System.Drawing.Size(73, 22);
            this.TsmiOpen.Text = "開く";
            this.TsmiOpen.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TsmiOpen_MouseUp);
            // 
            // TsmiDelete
            // 
            this.TsmiDelete.Name = "TsmiDelete";
            this.TsmiDelete.Size = new System.Drawing.Size(73, 22);
            this.TsmiDelete.Text = "削除";
            this.TsmiDelete.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TsmiDelete_MouseUp);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(70, 6);
            // 
            // tvFolderTree
            // 
            this.tvFolderTree.AllowDrop = true;
            this.tvFolderTree.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.tvFolderTree.BorderStyle = System.Windows.Forms.BorderStyle.None;
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
            this.tvFolderTree.Size = new System.Drawing.Size(289, 355);
            this.tvFolderTree.TabIndex = 0;
            this.tvFolderTree.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.TvFolderTree_BeforeExpand);
            this.tvFolderTree.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.TvFolderTree_AfterExpand);
            this.tvFolderTree.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.tvFolderTree_DrawNode);
            this.tvFolderTree.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.TvFolderTree_ItemDrag);
            this.tvFolderTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TvFolderTree_AfterSelect);
            this.tvFolderTree.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TvFolderTree_NodeMouseClick);
            this.tvFolderTree.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TvFolderTree_NodeMouseDoubleClick);
            this.tvFolderTree.DragDrop += new System.Windows.Forms.DragEventHandler(this.TvFolderTree_DragDrop);
            this.tvFolderTree.DragOver += new System.Windows.Forms.DragEventHandler(this.TvFolderTree_DragOver);
            this.tvFolderTree.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TvFolderTree_KeyPress);
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.tvFolderTree);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(289, 355);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(289, 380);
            this.toolStripContainer1.TabIndex = 1;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbReload,
            this.tsddbFilter});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(289, 25);
            this.toolStrip1.Stretch = true;
            this.toolStrip1.TabIndex = 0;
            // 
            // tsbReload
            // 
            this.tsbReload.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbReload.Image = global::mml2vgmIDE.Properties.Resources.Reload;
            this.tsbReload.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbReload.Name = "tsbReload";
            this.tsbReload.Size = new System.Drawing.Size(23, 22);
            this.tsbReload.Text = "Reload";
            this.tsbReload.Click += new System.EventHandler(this.TsbReload_Click);
            // 
            // tsddbFilter
            // 
            this.tsddbFilter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsddbFilter.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiFilterAll,
            this.toolStripSeparator2,
            this.tsmiFilterFolder,
            this.tsmiFilterFile,
            this.toolStripSeparator3,
            this.tsmiFilterGwi,
            this.tsmiFilterMuc,
            this.tsmiFilterMml,
            this.tsmiFilterWav});
            this.tsddbFilter.Image = global::mml2vgmIDE.Properties.Resources.Filter;
            this.tsddbFilter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsddbFilter.Name = "tsddbFilter";
            this.tsddbFilter.Size = new System.Drawing.Size(29, 22);
            this.tsddbFilter.Text = "Filter";
            // 
            // tsmiFilterAll
            // 
            this.tsmiFilterAll.Checked = true;
            this.tsmiFilterAll.CheckOnClick = true;
            this.tsmiFilterAll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsmiFilterAll.Name = "tsmiFilterAll";
            this.tsmiFilterAll.Size = new System.Drawing.Size(180, 22);
            this.tsmiFilterAll.Text = "*.* (All)";
            this.tsmiFilterAll.Click += new System.EventHandler(this.TsmiFilterAll_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(177, 6);
            // 
            // tsmiFilterFolder
            // 
            this.tsmiFilterFolder.CheckOnClick = true;
            this.tsmiFilterFolder.Name = "tsmiFilterFolder";
            this.tsmiFilterFolder.Size = new System.Drawing.Size(180, 22);
            this.tsmiFilterFolder.Text = "* (Folder)";
            this.tsmiFilterFolder.Click += new System.EventHandler(this.TsmiFilterFolder_Click);
            // 
            // tsmiFilterFile
            // 
            this.tsmiFilterFile.CheckOnClick = true;
            this.tsmiFilterFile.Name = "tsmiFilterFile";
            this.tsmiFilterFile.Size = new System.Drawing.Size(180, 22);
            this.tsmiFilterFile.Text = "* (File)";
            this.tsmiFilterFile.Click += new System.EventHandler(this.TsmiFilterFile_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(177, 6);
            // 
            // tsmiFilterGwi
            // 
            this.tsmiFilterGwi.CheckOnClick = true;
            this.tsmiFilterGwi.Name = "tsmiFilterGwi";
            this.tsmiFilterGwi.Size = new System.Drawing.Size(180, 22);
            this.tsmiFilterGwi.Text = ".gwi";
            this.tsmiFilterGwi.Click += new System.EventHandler(this.TsmiFilterGwi_Click);
            // 
            // tsmiFilterWav
            // 
            this.tsmiFilterWav.CheckOnClick = true;
            this.tsmiFilterWav.Name = "tsmiFilterWav";
            this.tsmiFilterWav.Size = new System.Drawing.Size(180, 22);
            this.tsmiFilterWav.Text = ".wav";
            this.tsmiFilterWav.Click += new System.EventHandler(this.TsmiFilterWav_Click);
            // 
            // tsmiFilterMuc
            // 
            this.tsmiFilterMuc.CheckOnClick = true;
            this.tsmiFilterMuc.Name = "tsmiFilterMuc";
            this.tsmiFilterMuc.Size = new System.Drawing.Size(180, 22);
            this.tsmiFilterMuc.Text = ".muc";
            this.tsmiFilterMuc.Click += new System.EventHandler(this.tsmiFilterMuc_Click);
            // 
            // tsmiFilterMml
            // 
            this.tsmiFilterMml.CheckOnClick = true;
            this.tsmiFilterMml.Name = "tsmiFilterMml";
            this.tsmiFilterMml.Size = new System.Drawing.Size(180, 22);
            this.tsmiFilterMml.Text = ".mml";
            this.tsmiFilterMml.Click += new System.EventHandler(this.tsmiFilterMml_Click);
            // 
            // FrmFolderTree
            // 
            this.ClientSize = new System.Drawing.Size(289, 380);
            this.Controls.Add(this.toolStripContainer1);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmFolderTree";
            this.Text = "Folder";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmFolderTree_FormClosing);
            this.cmsMenu.ResumeLayout(false);
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public CustomControl.MultiSelectTreeView tvFolderTree;
        private System.Windows.Forms.ImageList imgList;
        private System.Windows.Forms.ContextMenuStrip cmsMenu;
        private System.Windows.Forms.ToolStripMenuItem TsmiOpen;
        private System.Windows.Forms.ToolStripMenuItem TsmiDelete;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsbReload;
        private System.Windows.Forms.ToolStripDropDownButton tsddbFilter;
        private System.Windows.Forms.ToolStripMenuItem tsmiFilterAll;
        private System.Windows.Forms.ToolStripMenuItem tsmiFilterFolder;
        private System.Windows.Forms.ToolStripMenuItem tsmiFilterFile;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem tsmiFilterGwi;
        private System.Windows.Forms.ToolStripMenuItem tsmiFilterWav;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem tsmiFilterMuc;
        private System.Windows.Forms.ToolStripMenuItem tsmiFilterMml;
    }
}
