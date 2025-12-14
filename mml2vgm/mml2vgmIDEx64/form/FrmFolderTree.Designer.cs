using mml2vgmIDEx64.Properties;

namespace mml2vgmIDEx64
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmFolderTree));
            TreeNode treeNode1 = new TreeNode("ノード2");
            TreeNode treeNode2 = new TreeNode("ノード0", new TreeNode[] { treeNode1 });
            TreeNode treeNode3 = new TreeNode("ノード1");
            imgList = new ImageList(components);
            cmsMenu = new ContextMenuStrip(components);
            TsmiOpen = new ToolStripMenuItem();
            TsmiDelete = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            tvFolderTree = new CustomControl.MultiSelectTreeView();
            toolStripContainer1 = new ToolStripContainer();
            toolStrip1 = new ToolStrip();
            tsbReload = new ToolStripButton();
            tsddbFilter = new ToolStripDropDownButton();
            tsmiFilterAll = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            tsmiFilterFolder = new ToolStripMenuItem();
            tsmiFilterFile = new ToolStripMenuItem();
            toolStripSeparator3 = new ToolStripSeparator();
            tsmiFilterGwi = new ToolStripMenuItem();
            tsmiFilterMuc = new ToolStripMenuItem();
            tsmiFilterMml = new ToolStripMenuItem();
            tsmiFilterWav = new ToolStripMenuItem();
            cmsMenu.SuspendLayout();
            toolStripContainer1.ContentPanel.SuspendLayout();
            toolStripContainer1.TopToolStripPanel.SuspendLayout();
            toolStripContainer1.SuspendLayout();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // imgList
            // 
            imgList.ColorDepth = ColorDepth.Depth32Bit;
            imgList.ImageStream = (ImageListStreamer)resources.GetObject("imgList.ImageStream");
            imgList.TransparentColor = Color.Transparent;
            imgList.Images.SetKeyName(0, "File.png");
            imgList.Images.SetKeyName(1, "Folder.png");
            // 
            // cmsMenu
            // 
            cmsMenu.Items.AddRange(new ToolStripItem[] { TsmiOpen, TsmiDelete, toolStripSeparator1 });
            cmsMenu.Name = "cmsMenu";
            cmsMenu.ShowImageMargin = false;
            cmsMenu.Size = new Size(74, 54);
            // 
            // TsmiOpen
            // 
            TsmiOpen.Name = "TsmiOpen";
            TsmiOpen.Size = new Size(73, 22);
            TsmiOpen.Text = "開く";
            TsmiOpen.MouseUp += TsmiOpen_MouseUp;
            // 
            // TsmiDelete
            // 
            TsmiDelete.Name = "TsmiDelete";
            TsmiDelete.Size = new Size(73, 22);
            TsmiDelete.Text = "削除";
            TsmiDelete.MouseUp += TsmiDelete_MouseUp;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(70, 6);
            // 
            // tvFolderTree
            // 
            tvFolderTree.AllowDrop = true;
            tvFolderTree.BackColor = Color.Gray;
            tvFolderTree.BorderStyle = BorderStyle.None;
            tvFolderTree.CheckedColor = Color.FromArgb(80, 80, 160);
            tvFolderTree.Dock = DockStyle.Fill;
            tvFolderTree.DrawMode = TreeViewDrawMode.OwnerDrawAll;
            tvFolderTree.ForeColor = Color.FromArgb(224, 224, 224);
            tvFolderTree.HotColor = Color.FromArgb(64, 64, 90);
            tvFolderTree.ImageIndex = 0;
            tvFolderTree.ImageList = imgList;
            tvFolderTree.LineColor = Color.IndianRed;
            tvFolderTree.Location = new Point(0, 0);
            tvFolderTree.Name = "tvFolderTree";
            treeNode1.ImageKey = "File.png";
            treeNode1.Name = "ノード2";
            treeNode1.SelectedImageKey = "File.png";
            treeNode1.Text = "ノード2";
            treeNode2.ImageIndex = 1;
            treeNode2.Name = "ノード0";
            treeNode2.Text = "ノード0";
            treeNode3.Name = "ノード1";
            treeNode3.Text = "ノード1";
            tvFolderTree.Nodes.AddRange(new TreeNode[] { treeNode2, treeNode3 });
            tvFolderTree.SelectedColor = Color.FromArgb(100, 100, 180);
            tvFolderTree.SelectedImageIndex = 0;
            tvFolderTree.Size = new Size(289, 355);
            tvFolderTree.TabIndex = 0;
            tvFolderTree.BeforeExpand += TvFolderTree_BeforeExpand;
            tvFolderTree.AfterExpand += TvFolderTree_AfterExpand;
            tvFolderTree.DrawNode += tvFolderTree_DrawNode;
            tvFolderTree.ItemDrag += TvFolderTree_ItemDrag;
            tvFolderTree.AfterSelect += TvFolderTree_AfterSelect;
            tvFolderTree.NodeMouseClick += TvFolderTree_NodeMouseClick;
            tvFolderTree.NodeMouseDoubleClick += TvFolderTree_NodeMouseDoubleClick;
            tvFolderTree.DragDrop += TvFolderTree_DragDrop;
            tvFolderTree.DragOver += TvFolderTree_DragOver;
            tvFolderTree.KeyPress += TvFolderTree_KeyPress;
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            toolStripContainer1.ContentPanel.Controls.Add(tvFolderTree);
            toolStripContainer1.ContentPanel.Size = new Size(289, 355);
            toolStripContainer1.Dock = DockStyle.Fill;
            toolStripContainer1.Location = new Point(0, 0);
            toolStripContainer1.Name = "toolStripContainer1";
            toolStripContainer1.Size = new Size(289, 380);
            toolStripContainer1.TabIndex = 1;
            toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            toolStripContainer1.TopToolStripPanel.Controls.Add(toolStrip1);
            // 
            // toolStrip1
            // 
            toolStrip1.Dock = DockStyle.None;
            toolStrip1.Items.AddRange(new ToolStripItem[] { tsbReload, tsddbFilter });
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(289, 25);
            toolStrip1.Stretch = true;
            toolStrip1.TabIndex = 0;
            // 
            // tsbReload
            // 
            tsbReload.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbReload.Image = Resources.Reload;
            tsbReload.ImageTransparentColor = Color.Magenta;
            tsbReload.Name = "tsbReload";
            tsbReload.Size = new Size(23, 22);
            tsbReload.Text = "Reload";
            tsbReload.Click += TsbReload_Click;
            // 
            // tsddbFilter
            // 
            tsddbFilter.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsddbFilter.DropDownItems.AddRange(new ToolStripItem[] { tsmiFilterAll, toolStripSeparator2, tsmiFilterFolder, tsmiFilterFile, toolStripSeparator3, tsmiFilterGwi, tsmiFilterMuc, tsmiFilterMml, tsmiFilterWav });
            tsddbFilter.Image = Resources.Filter;
            tsddbFilter.ImageTransparentColor = Color.Magenta;
            tsddbFilter.Name = "tsddbFilter";
            tsddbFilter.Size = new Size(29, 22);
            tsddbFilter.Text = "Filter";
            // 
            // tsmiFilterAll
            // 
            tsmiFilterAll.Checked = true;
            tsmiFilterAll.CheckOnClick = true;
            tsmiFilterAll.CheckState = CheckState.Checked;
            tsmiFilterAll.Name = "tsmiFilterAll";
            tsmiFilterAll.Size = new Size(123, 22);
            tsmiFilterAll.Text = "*.* (All)";
            tsmiFilterAll.Click += TsmiFilterAll_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(120, 6);
            // 
            // tsmiFilterFolder
            // 
            tsmiFilterFolder.CheckOnClick = true;
            tsmiFilterFolder.Name = "tsmiFilterFolder";
            tsmiFilterFolder.Size = new Size(123, 22);
            tsmiFilterFolder.Text = "* (Folder)";
            tsmiFilterFolder.Click += TsmiFilterFolder_Click;
            // 
            // tsmiFilterFile
            // 
            tsmiFilterFile.CheckOnClick = true;
            tsmiFilterFile.Name = "tsmiFilterFile";
            tsmiFilterFile.Size = new Size(123, 22);
            tsmiFilterFile.Text = "* (File)";
            tsmiFilterFile.Click += TsmiFilterFile_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(120, 6);
            // 
            // tsmiFilterGwi
            // 
            tsmiFilterGwi.CheckOnClick = true;
            tsmiFilterGwi.Name = "tsmiFilterGwi";
            tsmiFilterGwi.Size = new Size(123, 22);
            tsmiFilterGwi.Text = ".gwi";
            tsmiFilterGwi.Click += TsmiFilterGwi_Click;
            // 
            // tsmiFilterMuc
            // 
            tsmiFilterMuc.CheckOnClick = true;
            tsmiFilterMuc.Name = "tsmiFilterMuc";
            tsmiFilterMuc.Size = new Size(123, 22);
            tsmiFilterMuc.Text = ".muc";
            tsmiFilterMuc.Click += tsmiFilterMuc_Click;
            // 
            // tsmiFilterMml
            // 
            tsmiFilterMml.CheckOnClick = true;
            tsmiFilterMml.Name = "tsmiFilterMml";
            tsmiFilterMml.Size = new Size(123, 22);
            tsmiFilterMml.Text = ".mml";
            tsmiFilterMml.Click += tsmiFilterMml_Click;
            // 
            // tsmiFilterWav
            // 
            tsmiFilterWav.CheckOnClick = true;
            tsmiFilterWav.Name = "tsmiFilterWav";
            tsmiFilterWav.Size = new Size(123, 22);
            tsmiFilterWav.Text = ".wav";
            tsmiFilterWav.Click += TsmiFilterWav_Click;
            // 
            // FrmFolderTree
            // 
            ClientSize = new Size(289, 380);
            Controls.Add(toolStripContainer1);
            DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "FrmFolderTree";
            Text = "Folder";
            FormClosing += FrmFolderTree_FormClosing;
            cmsMenu.ResumeLayout(false);
            toolStripContainer1.ContentPanel.ResumeLayout(false);
            toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            toolStripContainer1.TopToolStripPanel.PerformLayout();
            toolStripContainer1.ResumeLayout(false);
            toolStripContainer1.PerformLayout();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);

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
