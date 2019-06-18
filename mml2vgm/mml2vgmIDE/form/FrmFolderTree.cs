using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace mml2vgmIDE
{
    public partial class FrmFolderTree : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        public Action parentUpdate = null;
        public Action<string> parentExecFile = null;
        public string basePath = "";

        public FrmFolderTree(Setting setting)
        {
            InitializeComponent();
            this.tvFolderTree.BackColor = Color.FromArgb(setting.ColorScheme.FolderTree_BackColor);
            this.tvFolderTree.ForeColor = Color.FromArgb(setting.ColorScheme.FolderTree_ForeColor);
        }
        protected override string GetPersistString()
        {
            return this.Name;
        }

        private void FrmFolderTree_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            parentUpdate?.Invoke();
        }

        private void TreeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.ImageIndex == 1 && e.Node.Nodes.Count == 1 && e.Node.Nodes[0].Text == "!dmy")
            {
                TreeNode node = e.Node;
                TreeNode ts;

                // 展開するノードのフルパスを取得
                string fullpath = System.IO.Path.Combine(Path.GetDirectoryName(basePath), node.FullPath);
                node.Nodes.Clear();

                DirectoryInfo dm = new DirectoryInfo(fullpath);

                try
                {
                    foreach (DirectoryInfo ds in dm.GetDirectories())
                    {
                        ts = new TreeNode(ds.Name, 1, 1);
                        ts.Nodes.Add("!dmy");
                        node.Nodes.Add(ts);
                    }
                    foreach (FileInfo fi in dm.GetFiles())
                    {
                        ts = new TreeNode(fi.Name, 0, 0);
                        node.Nodes.Add(ts);
                    }
                }
                catch(UnauthorizedAccessException)
                {
                    MessageBox.Show("アクセスが拒否されました。", "権限不足", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch { }
            }
        }

        private void TreeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.ImageIndex == 1) return;
            string fullpath = System.IO.Path.Combine(Path.GetDirectoryName(basePath), e.Node.FullPath);
            parentExecFile?.Invoke(fullpath);
        }

        private void TvFolderTree_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                TreeNode tn = tvFolderTree.SelectedNode;
                if (tn == null) return;
                if (tn.ImageIndex == 1) return;
                string fullpath = System.IO.Path.Combine(Path.GetDirectoryName(basePath), tn.FullPath);
                parentExecFile?.Invoke(fullpath);
                e.Handled = true;
            }
        }

        private void TvFolderTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                tvFolderTree.SelectedNode = e.Node;
                TreeNode tn = e.Node;
                if (tn == null) return;
                if (tn.ImageIndex == 1) return;
                cmsMenu.Show((Control)sender, e.X, e.Y);
            }
        }

        private void TsmiOpen_Click(object sender, EventArgs e)
        {
            TreeNode tn = tvFolderTree.SelectedNode;
            if (tn == null) return;
            if (tn.ImageIndex == 1) return;
            string fullpath = System.IO.Path.Combine(Path.GetDirectoryName(basePath), tn.FullPath);
            parentExecFile?.Invoke(fullpath);
        }
    }
}
