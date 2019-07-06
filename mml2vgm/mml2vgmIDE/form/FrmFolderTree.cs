using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace mml2vgmIDE
{
    public partial class FrmFolderTree : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        public Action parentUpdate = null;
        public Action<string> parentExecFile = null;
        public Action<string> parentDeleteFile = null;
        public string basePath = "";

        public ToolStripMenuItem extendItem {
            set
            {
                SetContextMenuItem(value);
            }
        }

        private void SetContextMenuItem(ToolStripMenuItem value)
        {
            if (value == null || value.DropDownItems.Count == 0) return;
            while (0 < value.DropDownItems.Count)
            {
                cmsMenu.Items.Add(value.DropDownItems[0]);
            }
        }

        public FrmFolderTree(Setting setting)
        {
            InitializeComponent();
            this.tvFolderTree.BackColor = Color.FromArgb(setting.ColorScheme.FolderTree_BackColor);
            this.tvFolderTree.ForeColor = Color.FromArgb(setting.ColorScheme.FolderTree_ForeColor);
        }

        private void FrmFolderTree_Load(object sender, EventArgs e)
        {
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

                string ext = Path.GetExtension(tn.FullPath);
                CheckExtension(cmsMenu.Items, ext);

                cmsMenu.Show((Control)sender, e.X, e.Y);

            }
        }

        private void CheckExtension(ToolStripItemCollection pItems, string ext)
        {
            foreach(ToolStripItem item in pItems)
            {
                if (item.Tag == null) continue;
                if (item is ToolStripSeparator) continue;

                ToolStripMenuItem tsmi = (ToolStripMenuItem)item;
                if (tsmi.DropDownItems.Count > 0)
                {
                    CheckExtension(tsmi.DropDownItems, ext);
                }

                Tuple<int, string, string[], string> tp = (Tuple<int, string, string[], string>)item.Tag;
                if (tp == null) continue;
                if (tp.Item1 < 0) continue;

                bool match = false;
                foreach (string s in tp.Item3)
                {
                    if (s == ".*")
                    {
                        match = true;
                        break;
                    }

                    string regexPattern = TransW2Reg(s);
                    if (new Regex(regexPattern).IsMatch(ext))
                    {
                        match = true;
                        break;
                    }
                }

                item.Enabled = match;
            }
        }

        private string TransW2Reg(string wildcardPtn)
        {
            //参考：
            //  ワイルドカードを使用した文字列マッチ
            //    大谷 和広さま(@kazuhirox)
            //https://qiita.com/kazuhirox/items/5e314d5e7732041a3fe7

            var regexPattern = System.Text.RegularExpressions.Regex.Replace(
              wildcardPtn,
              ".",
              m =>
              {
                  string s = m.Value;
                  if (s.Equals("?"))
                  {
                      //?は任意の1文字を示す正規表現(.)に変換
                      return ".";
                  }
                  else if (s.Equals("*"))
                  {
                      //*は0文字以上の任意の文字列を示す正規表現(.*)に変換
                      return ".*";
                  }
                  else
                  {
                      //上記以外はエスケープする
                      return System.Text.RegularExpressions.Regex.Escape(s);
                  }
              }
              );

            return regexPattern;
        }

        private void TsmiOpen_Click(object sender, EventArgs e)
        {
            TreeNode tn = tvFolderTree.SelectedNode;
            if (tn == null) return;
            if (tn.ImageIndex == 1) return;
            string fullpath = System.IO.Path.Combine(Path.GetDirectoryName(basePath), tn.FullPath);
            parentExecFile?.Invoke(fullpath);
        }

        private void TsmiDelete_Click(object sender, EventArgs e)
        {
            TreeNode tn = tvFolderTree.SelectedNode;
            if (tn == null) return;
            if (tn.ImageIndex == 1) return;
            string fullpath = System.IO.Path.Combine(Path.GetDirectoryName(basePath), tn.FullPath);
            DialogResult ret = MessageBox.Show(
                string.Format("「{0}」を削除します。よろしいですか。",tn.Text)
                ,"ファイル削除", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (ret == DialogResult.Cancel) return;

            parentDeleteFile?.Invoke(fullpath);

        }
    }
}
