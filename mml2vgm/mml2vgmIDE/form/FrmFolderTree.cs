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
using CustomControl;
using WeifenLuo.WinFormsUI.Docking;
using System.Collections;

namespace mml2vgmIDE
{
    public partial class FrmFolderTree : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        public Action parentUpdate = null;
        public Action<string[]> parentExecFile = null;
        public Action<string[]> parentDeleteFile = null;
        public string basePath = "";
        private bool useMouse;

        public FrmFolderTree(Setting setting,DockPanel dockPanel)
        {
            InitializeComponent();
            this.tvFolderTree.BackColor = Color.FromArgb(setting.ColorScheme.FolderTree_BackColor);
            this.tvFolderTree.ForeColor = Color.FromArgb(setting.ColorScheme.FolderTree_ForeColor);
            this.tvFolderTree.HotColor = Color.FromArgb(setting.ColorScheme.FolderTree_BackColor);
            this.tvFolderTree.HotColor = Color.FromArgb(
                (int)(tvFolderTree.HotColor.R * 1.3), 
                (int)(tvFolderTree.HotColor.G * 1.3), 
                (int)(tvFolderTree.HotColor.B * 1.3));

            tvFolderTree.TreeViewNodeSorter = new NodeSorter();

            dockPanel.Theme.ApplyTo(cmsMenu);
            dockPanel.Theme.ApplyTo(toolStrip1);

            Common.SetDoubleBuffered(tvFolderTree);
        }

        private void FrmFolderTree_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            parentUpdate?.Invoke();
        }

        private void TvFolderTree_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.ImageIndex == 1 && e.Node.Nodes.Count == 1 && e.Node.Nodes[0].Text == "!dmy")
            {
                TreeNode node = e.Node;
                TreeNode ts;

                // 展開するノードのフルパスを取得
                string path1 = Path.GetDirectoryName(basePath);
                path1 = string.IsNullOrEmpty(path1) ? basePath : path1;
                string fullpath = Path.Combine(path1, Path.GetFullPath(node.FullPath));
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

        private void TvFolderTree_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            List<string> lstFullPath = new List<string>();
            GetCheckTreeNodesFullPath(ref lstFullPath, tvFolderTree.Nodes);
            if (lstFullPath.Count < 1) return;
            parentExecFile?.Invoke(lstFullPath.ToArray());
        }

        private void TvFolderTree_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                List<string> lstFullPath = new List<string>();
                GetCheckTreeNodesFullPath(ref lstFullPath, tvFolderTree.Nodes);
                if (lstFullPath.Count < 1) return;
                parentExecFile?.Invoke(lstFullPath.ToArray());

                //TreeNode tn = tvFolderTree.SelectedNode;
                //if (tn == null) return;
                //if (tn.ImageIndex == 1) return;
                //string fullpath = System.IO.Path.Combine(Path.GetDirectoryName(basePath), tn.FullPath);
                //parentExecFile?.Invoke(fullpath);

                e.Handled = true;
            }
        }

        private void TvFolderTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            useMouse = true;

            if (e.Button == MouseButtons.Right)
            {
                if (!e.Node.Checked)
                {
                    if ((Control.ModifierKeys & (Keys.Control | Keys.Shift)) == 0)
                    {
                        CheckClear(tvFolderTree.Nodes);
                    }
                    tvFolderTree.SelectedNode = e.Node;
                }
                e.Node.Checked = true;
                tvFolderTree.SelectedNode = e.Node;
                TreeNode tn = e.Node;
                if (tn == null) return;
                if (tn.ImageIndex == 1) return;

                List<string> lstFullPathExt = new List<string>();
                GetCheckTreeNodesFullPathExt(lstFullPathExt, tvFolderTree.SelectedNode.Parent.Nodes);

                CheckExtension(cmsMenu.Items, lstFullPathExt.ToArray());

                cmsMenu.Show((Control)sender, e.X, e.Y);
                
            }
            else if (e.Button == MouseButtons.Left)
            {
                tvFolderTree.SelectedNode = e.Node;

                if ((Control.ModifierKeys & (Keys.Control | Keys.Shift)) == 0)
                {
                    CheckClear(tvFolderTree.Nodes);
                }

                e.Node.Checked = true;
            }
        }

        private void TvFolderTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (useMouse)
            {
                useMouse = false;
                return;
            }
            tvFolderTree.SelectedNode = e.Node;

            if ((Control.ModifierKeys & (Keys.Control | Keys.Shift)) == 0)
            {
                CheckClear(tvFolderTree.Nodes);
            }

            e.Node.Checked = true;

        }

        private void TvFolderTree_ItemDrag(object sender, ItemDragEventArgs e)
        {
            TreeView tv = (TreeView)sender;
            tv.SelectedNode = ((TreeNode)e.Item);
            ((TreeNode)e.Item).Checked = true;
            tv.Focus();

            List<string> lstFile = new List<string>();
            GetCheckTreeNodesFullPath(ref lstFile, tv.Nodes);

            DataObject dataObj = new DataObject(DataFormats.FileDrop, lstFile.ToArray());
            DragDropEffects dde = tv.DoDragDrop(dataObj, DragDropEffects.All);

            if ((dde & DragDropEffects.Move) == DragDropEffects.Move)
                refresh();
        }

        private void TvFolderTree_DragOver(object sender, DragEventArgs e)
        {
            //ドラッグされているデータがfileか調べる
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                //Ctrl:8
                //Shift:4

                if ((e.KeyState & 4) != 0 && (e.AllowedEffect & DragDropEffects.Move) != 0)
                    //Shiftキーが押されていればMove
                    e.Effect = DragDropEffects.Move;
                else if ((e.AllowedEffect & DragDropEffects.Copy) != 0)
                    //何も押されていなければCopy
                    e.Effect = DragDropEffects.Copy;
                else
                    e.Effect = DragDropEffects.None;
            }
            else
                e.Effect = DragDropEffects.None;

            if (e.Effect == DragDropEffects.None) return;

            //マウス下のNodeを選択する
            TreeView tv = (TreeView)sender;
            //マウスのあるNodeを取得する
            TreeNode target = tv.GetNodeAt(tv.PointToClient(new Point(e.X, e.Y)));

            string[] source = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (target == null) return;

            if (target.ImageIndex != 1)//fileはフォルダーの上にのみDropできるようにする
            {
                e.Effect = DragDropEffects.None;
                return;
            }

        }

        private void TvFolderTree_DragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            TreeView tv = (TreeView)sender;
            string[] source = (string[])e.Data.GetData(DataFormats.FileDrop);

            //ドロップ先のTreeNodeを取得する
            TreeNode target = tv.GetNodeAt(tv.PointToClient(new Point(e.X, e.Y)));

            if (target == null) return;

            if (target.ImageIndex != 1)//fileはフォルダーの上にのみDropできるようにする
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            if ((e.Effect & DragDropEffects.Copy) != 0)
            {
                //ファイルコピー
                CopyFile_DragDrop(source, target);
                return;
            }
            else if ((e.Effect & DragDropEffects.Move) != 0)
            {
                //ファイルムーブ
                CopyFile_DragDrop(source, target, true);
                return;
            }

            e.Effect = DragDropEffects.None;

        }

        private void TsmiOpen_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) return;

            List<string> lstFullPath = new List<string>();
            GetCheckTreeNodesFullPath(ref lstFullPath, tvFolderTree.Nodes);
            if (lstFullPath.Count < 1) return;
            parentExecFile?.Invoke(lstFullPath.ToArray());

        }

        private void TsmiDelete_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) return;

            List<string> lstFullPath = new List<string>();
            GetCheckTreeNodesFullPath(ref lstFullPath, tvFolderTree.Nodes);
            if (lstFullPath.Count < 1) return;

            DialogResult ret = DialogResult.Cancel;

            if (lstFullPath.Count == 1)
            {
                ret = MessageBox.Show(
                    string.Format("「{0}」を削除します。よろしいですか。", lstFullPath[0])
                    , "ファイル/フォルダーの削除", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            }
            else
            {
                string fns = "";
                for (int i = 0; i < Math.Min(lstFullPath.Count, 3); i++)
                {
                    fns += "    " + lstFullPath[i] + "\n";
                }
                if (lstFullPath.Count > 3)
                {
                    fns += string.Format("\n    他、{0} ファイル/フォルダー\n", lstFullPath.Count - 3);
                }

                ret = MessageBox.Show(
                    string.Format("以下のファイル/フォルダーを削除します。よろしいですか。\n\n{0}", fns)
                    , "ファイル/フォルダーの削除", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            }

            if (ret == DialogResult.Cancel) return;

            parentDeleteFile?.Invoke(lstFullPath.ToArray());

        }



        private void CopyFile_DragDrop(string[] source, TreeNode target, bool doDelete = false)
        {
            List<string> lstFail = new List<string>();

            foreach (string fileOrFolder in source)
            {
                try
                {
                    if ((File.GetAttributes(fileOrFolder) & FileAttributes.Directory) != 0)
                    {
                        // folder
                        string tmp =
                            (fileOrFolder[fileOrFolder.Length - 1] == Path.DirectorySeparatorChar)
                            ? fileOrFolder.Substring(0, fileOrFolder.Length - 1)
                            : fileOrFolder;
                        string folder = Path.GetFileName(tmp);
                        string path1 = Path.GetDirectoryName(basePath);
                        path1 = string.IsNullOrEmpty(path1) ? basePath : path1;
                        string targetPath = Path.Combine(path1, Path.GetFullPath(target.FullPath), folder);

                        CopyDirectory(fileOrFolder, targetPath);

                        if (doDelete) Directory.Delete(fileOrFolder, true);
                    }
                    else
                    {
                        // file
                        string file = Path.GetFileName(fileOrFolder);
                        string path1 = Path.GetDirectoryName(basePath);
                        path1 = string.IsNullOrEmpty(path1) ? basePath : path1;
                        string targetPath = Path.Combine(path1, Path.GetFullPath(target.FullPath), file);

                        if (File.Exists(targetPath))
                        {
                            DialogResult res = MessageBox.Show(
                                string.Format("次のファイルはコピー先に既に存在します。上書きしますか？\n\n{0}"
                                , file)
                                , "上書き確認"
                                , MessageBoxButtons.YesNoCancel
                                , MessageBoxIcon.Question);
                            if (res == DialogResult.Cancel) return;
                            if (res == DialogResult.Yes) File.Copy(fileOrFolder, targetPath, true);
                        }
                        else
                        {
                            File.Copy(fileOrFolder, targetPath);
                        }

                        if (doDelete) File.Delete(fileOrFolder);
                    }
                }
                catch (FileNotFoundException fe)
                {
                    log.ForcedWrite(fe);
                    lstFail.Add(string.Format("'{0}'", fileOrFolder));
                }
                catch (DirectoryNotFoundException de)
                {
                    log.ForcedWrite(de);
                    lstFail.Add(string.Format("'{0}'", fileOrFolder));
                }
            }

            refresh();
        }

        /// <summary>
        /// From dobon.net : https://dobon.net/vb/dotnet/file/copyfolder.html
        /// </summary>
        public static void CopyDirectory(string sourceDirName, string destDirName)
        {
            //コピー先のディレクトリがないときは作る
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
                //属性もコピー
                File.SetAttributes(destDirName, File.GetAttributes(sourceDirName));
            }

            //コピー先のディレクトリ名の末尾に"\"をつける
            if (destDirName[destDirName.Length - 1] != Path.DirectorySeparatorChar)
                destDirName = destDirName + Path.DirectorySeparatorChar;

            //コピー元のディレクトリにあるファイルをコピー
            string[] files = Directory.GetFiles(sourceDirName);
            foreach (string file in files)
            {
                string targetPath = destDirName + Path.GetFileName(file);

                if (File.Exists(targetPath))
                {
                    DialogResult res = MessageBox.Show(
                        string.Format("次のファイルはコピー先に既に存在します。上書きしますか？\n\n{0}"
                        , Path.GetFileName(file))
                        , "上書き確認"
                        , MessageBoxButtons.YesNoCancel
                        , MessageBoxIcon.Question);
                    if (res == DialogResult.Cancel)
                    {
                        return;
                    }
                    if (res == DialogResult.Yes)
                    {
                        File.Copy(file, targetPath, true);
                    }
                }
                else
                {
                    File.Copy(file, targetPath);
                }
            }

            //コピー元のディレクトリにあるディレクトリについて、再帰的に呼び出す
            string[] dirs = Directory.GetDirectories(sourceDirName);
            foreach (string dir in dirs)
                CopyDirectory(dir, destDirName + Path.GetFileName(dir));
        }

        public void refresh()
        {
            if (string.IsNullOrEmpty(basePath)) return;
            refreshRemoveCheck(tvFolderTree.Nodes);
            if (tvFolderTree.Nodes.Count > 0)
            {
                refreshAddCheck(tvFolderTree.Nodes[0]);
                refreshFilter(tvFolderTree.Nodes[0].Nodes);
            }
            tvFolderTree.Sort();
        }

        private void refreshRemoveCheck(TreeNodeCollection nodes)
        {
            int i = 0;
            while (i < nodes.Count)
            {
                try
                {
                    TreeNode tn = nodes[i++];
                    if (tn.Nodes.Count > 0 && tn.Nodes[0].Text != "!dmy")
                    {
                        refreshRemoveCheck(tn.Nodes);
                    }
                    string path1 = Path.GetDirectoryName(basePath);
                    path1 = string.IsNullOrEmpty(path1) ? basePath : path1;
                    string fullpath = Path.Combine(path1, Path.GetFullPath(tn.FullPath));
                    if (tn.ImageIndex == 1)
                    {
                        //folder
                        if (!Directory.Exists(fullpath))
                        {
                            nodes.Remove(tn);
                            i--;
                        }
                    }
                    else
                    {
                        //file
                        if (!File.Exists(fullpath))
                        {
                            nodes.Remove(tn);
                            i--;
                        }
                    }
                }
                catch
                {
                    ;
                }
            }
        }

        private void refreshAddCheck(TreeNode tn)
        {
            string path1 = Path.GetDirectoryName(basePath);
            path1 = string.IsNullOrEmpty(path1) ? basePath : path1;

            string fullpath = Path.Combine(path1, Path.GetFullPath(tn.FullPath));
            DirectoryInfo dm = new DirectoryInfo(fullpath);
            TreeNode ts;

            try
            {
                foreach (DirectoryInfo ds in dm.GetDirectories())
                {
                    int i = 0;
                    bool flg = false;
                    while (i < tn.Nodes.Count)
                    {
                        TreeNode ttn = tn.Nodes[i++];
                        if (ttn.Nodes.Count > 0 && ttn.Nodes[0].Text != "!dmy")
                        {
                            refreshAddCheck(ttn);
                        }
                        else
                        {
                            if (ttn.ImageIndex==1 && ttn.Nodes.Count == 0)
                                refreshAddCheck(ttn);

                        }

                        string fpath = Path.Combine(path1,Path.GetFullPath( ttn.FullPath));
                        if (ds.FullName == fpath)
                        {
                            flg = true;
                        }
                    }
                    if (!flg)
                    {
                        ts = new TreeNode(ds.Name, 1, 1);
                        ts.Nodes.Add("!dmy");
                        tn.Nodes.Add(ts);
                    }
                }
                foreach (FileInfo fi in dm.GetFiles())
                {
                    int i = 0;
                    bool flg = false;
                    while (i < tn.Nodes.Count)
                    {
                        TreeNode ttn = tn.Nodes[i++];
                        string fpath = Path.Combine(path1, Path.GetFullPath(ttn.FullPath));
                        if (fi.FullName.ToUpper() == fpath.ToUpper())
                        {
                            flg = true;
                            if (fi.FullName != fpath)
                            {
                                tn.Nodes.Remove(ttn);
                                flg = false;
                            }
                            break;
                        }
                    }
                    if (!flg)
                    {
                        ts = new TreeNode(fi.Name, 0, 0);
                        tn.Nodes.Add(ts);
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("アクセスが拒否されました。", "権限不足", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch { }

        }

        private void refreshFilter(TreeNodeCollection nodes)
        {
            if (tsmiFilterAll.Checked) return;

            int i = 0;
            while(i< nodes.Count)
            {
                TreeNode tn = nodes[i++];

                if (tn.ImageIndex == 1)
                {
                    if (!tsmiFilterFolder.Checked)
                    {
                        nodes.Remove(tn);
                        i--;
                    }
                    else
                    {
                        if (tn.Nodes.Count > 0)
                        {
                            refreshFilter(tn.Nodes);
                        }
                    }
                }
                else
                {
                    if (tsmiFilterFile.Checked)
                    {
                        continue;
                    }
                    else
                    {
                        if (Path.GetExtension(tn.Text).ToLower() == ".gwi")
                        {
                            if (tsmiFilterGwi.Checked)
                            {
                                continue;
                            }
                        }
                        else if (Path.GetExtension(tn.Text).ToLower() == ".wav")
                        {
                            if (tsmiFilterWav.Checked)
                            {
                                continue;
                            }
                        }
                        else if (tn.Text == "!dmy")
                        {
                            continue;
                        }
                        nodes.Remove(tn);
                        i--;
                    }
                }
            }

        }

        public ToolStripMenuItem extendItem
        {
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

        protected override string GetPersistString()
        {
            return this.Name;
        }

        private void CheckClear(TreeNodeCollection nodes)
        {
            foreach (TreeNode tn in nodes)
            {
                tn.Checked = false;
                if (tn.Nodes.Count > 0) CheckClear(tn.Nodes);
            }
        }

        public void GetCheckTreeNodesFullPath(ref List<string> lstFullPath, TreeNodeCollection nodes, bool force = false)
        {
            foreach (TreeNode tn in nodes)
            {
                if (tn.Nodes.Count > 0)
                {
                    GetCheckTreeNodesFullPath(ref lstFullPath, tn.Nodes);
                }
                if (!tn.Checked && !tn.IsSelected) continue;

                string fullpath = Core.Common.PathCombine(Core.Common.GetDirectoryName(basePath), Path.GetFullPath(tn.FullPath));
                lstFullPath.Add(fullpath);
            }
        }

        private void GetCheckTreeNodesFullPathExt(List<string> lstFullPathExt, TreeNodeCollection nodes)
        {
            foreach (TreeNode tn in nodes)
            {
                if (tn.Nodes.Count > 0)
                {
                    GetCheckTreeNodesFullPathExt(lstFullPathExt, tn.Nodes);
                    continue;
                }
                if (!tn.Checked) continue;
                lstFullPathExt.Add(Path.GetExtension(Path.GetFullPath(tn.FullPath)));
            }
        }

        private void GetCheckTreeNodes(List<TreeNode> lstNode, TreeNodeCollection nodes)
        {
            foreach (TreeNode tn in nodes)
            {
                if (tn.Nodes.Count > 0)
                {
                    GetCheckTreeNodes(lstNode, tn.Nodes);
                    continue;
                }
                if (!tn.Checked && !tn.IsSelected) continue;

                lstNode.Add(tn);
            }
        }

        private void CheckExtension(ToolStripItemCollection pItems, string[] exts)
        {
            foreach (ToolStripItem item in pItems)
            {
                if (item.Tag == null) continue;
                if (item is ToolStripSeparator) continue;

                ToolStripMenuItem tsmi = (ToolStripMenuItem)item;
                if (tsmi.DropDownItems.Count > 0)
                {
                    CheckExtension(tsmi.DropDownItems, exts);
                }

                Tuple<int, string, string[], string> tp = (Tuple<int, string, string[], string>)item.Tag;
                if (tp == null) continue;
                if (tp.Item1 < 0) continue;

                bool match = false;
                foreach (string ext in exts)
                {
                    match = false;
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

                    if (!match) break;
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

        public class NodeSorter : IComparer
        {
            // Compare the length of the strings, or the strings
            // themselves, if they are the same length.
            public int Compare(object x, object y)
            {
                TreeNode tx = x as TreeNode;
                TreeNode ty = y as TreeNode;

                if (tx.ImageIndex != ty.ImageIndex)
                    return tx.ImageIndex == 1 ? -1 : 1;

                // If they are the same length, call Compare.
                return string.Compare(tx.Text, ty.Text);
            }
        }

        private void TsbReload_Click(object sender, EventArgs e)
        {
            refresh();
        }

        private const string FILTER_ALL = "*.* (All)";
        private const string FILTER_FILE = "* (File)";
        private const string FILTER_FOLDER = "* (Folder)";

        private void TsmiFilterAll_Click(object sender, EventArgs e)
        {
            //All以外は全てチェックオフ
            foreach(ToolStripItem tsi in tsddbFilter.DropDownItems)
            {
                if (!(tsi is ToolStripMenuItem)) continue;
                ToolStripMenuItem item = (ToolStripMenuItem)tsi;
                if (item.Text == FILTER_ALL) continue;
                item.CheckState = CheckState.Unchecked;
            }

            refresh();
        }

        private void TsmiFilterFolder_Click(object sender, EventArgs e)
        {
            foreach (ToolStripItem tsi in tsddbFilter.DropDownItems)
            {
                if (!(tsi is ToolStripMenuItem)) continue;
                ToolStripMenuItem item = (ToolStripMenuItem)tsi;
                if (item.Text == FILTER_ALL)
                    item.CheckState = CheckState.Unchecked;
            }

            refresh();
        }

        private void TsmiFilterFile_Click(object sender, EventArgs e)
        {
            foreach (ToolStripItem tsi in tsddbFilter.DropDownItems)
            {
                if (!(tsi is ToolStripMenuItem)) continue;
                ToolStripMenuItem item = (ToolStripMenuItem)tsi;
                if (item.Text == FILTER_ALL)
                    item.CheckState = CheckState.Unchecked;
                else if (item.Text == FILTER_FILE) continue;
                else if (item.Text == FILTER_FOLDER) continue;
                else item.CheckState = CheckState.Unchecked;
            }

            refresh();
        }

        private void TsmiFilterGwi_Click(object sender, EventArgs e)
        {
            FilterItemCheckStateRefresh();

            refresh();
        }

        private void TsmiFilterWav_Click(object sender, EventArgs e)
        {
            FilterItemCheckStateRefresh();

            refresh();
        }

        private void FilterItemCheckStateRefresh()
        {
            foreach (ToolStripItem tsi in tsddbFilter.DropDownItems)
            {
                if (!(tsi is ToolStripMenuItem)) continue;
                ToolStripMenuItem item = (ToolStripMenuItem)tsi;
                if (item.Text != FILTER_ALL && item.Text != FILTER_FILE) continue;
                item.CheckState = CheckState.Unchecked;
            }
        }

        private void TvFolderTree_AfterExpand(object sender, TreeViewEventArgs e)
        {
            refreshFilter(e.Node.Nodes);
        }
    }
}
