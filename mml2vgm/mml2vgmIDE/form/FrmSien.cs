using musicDriverInterface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Security.Permissions;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace mml2vgmIDE
{
    public partial class FrmSien : DockContent
    {
        public Action parentUpdate = null;
        public FrmMain parent = null;
        public Setting setting = null;
        private SienSearch ss;
        public int selRow = -1;
        private Dictionary<string, string[]> instCache = new Dictionary<string, string[]>();
        private Dictionary<string, Tuple<InstrumentAtLocal.enmInstType, string, string, string>[]> instCacheFolder = new Dictionary<string, Tuple<InstrumentAtLocal.enmInstType, string, string, string>[]>();

        public FrmSien(FrmMain parent,Setting setting)
        {
            InitializeComponent();
            ss = new SienSearch(System.IO.Path.Combine(Common.GetApplicationFolder(), "mmlSien.json"));
            this.setting = setting;
            this.parent = parent;
            if (setting.sien.CacheInstrumentName != null)
            {
                instCache = ConvertCacheList(setting.sien);
            }

            this.BackColor = Color.FromArgb(setting.ColorScheme.Azuki_BackColor);
            treeView1.BackColor = Color.FromArgb(setting.ColorScheme.Azuki_BackColor);
            this.Opacity = setting.other.Opacity / 100.0;
        }
        protected override string GetPersistString()
        {
            return this.Name;
        }

        private Dictionary<string, string[]> ConvertCacheList(Setting.Sien sien)
        {
            Dictionary<string, string[]> ret = new Dictionary<string, string[]>();
            string[] names = sien.CacheInstrumentName;
            string[][] data = sien.CacheInstrumentData;

            if (names == null || data == null) return ret;
            if (names.Length != data.Length) return ret;
            if (setting.sien.cacheClear)
            {
                setting.sien.cacheClear = false;
                return ret;
            }

            for (int i = 0; i < names.Length; i++)
            {
                if (ret.ContainsKey(names[i]))
                {
                    ret.Remove(names[i]);
                }

                ret.Add(names[i], data[i]);
            }

            return ret;
        }

        private void ReConvertCacheList()
        {
            List<string> cNames = new List<string>();
            List<string[]> cData = new List<string[]>();

            foreach(var v in instCache)
            {
                cNames.Add(v.Key);
                cData.Add(v.Value);
            }

            setting.sien.CacheInstrumentName = cNames.ToArray();
            setting.sien.CacheInstrumentData = cData.ToArray();
        }

        protected override bool ShowWithoutActivation
        {
            get
            {
                //Console.WriteLine("*");
                return true;
            }
        }

        //protected override CreateParams CreateParams
        //{
        //    [SecurityPermission(SecurityAction.Demand,
        //        Flags = SecurityPermissionFlag.UnmanagedCode)]
        //    get
        //    {
        //        const int WS_EX_TOOLWINDOW = 0x80;
        //        const long WS_POPUP = 0x80000000L;
        //        const int WS_VISIBLE = 0x10000000;
        //        const int WS_SYSMENU = 0x80000;
        //        const int WS_MAXIMIZEBOX = 0x10000;

        //        CreateParams cp = base.CreateParams;
        //        cp.ExStyle = WS_EX_TOOLWINDOW;
        //        cp.Style = unchecked((int)WS_POPUP) |
        //            WS_VISIBLE | WS_SYSMENU | WS_MAXIMIZEBOX;

        //        if (this.Width != 0)
        //        {
        //            tw = this.Width;
        //            th = this.Height;
        //        }

        //        cp.Width = 0;
        //        cp.Height = 0;

        //        return cp;
        //    }
        //}

        public void update()
        {
        }

        public void Request(string line, Point ciP,int parentID,EnmMmlFileFormat tp,object option)
        {
            Location = new Point(ciP.X, ciP.Y);
            //dgvItem.Rows.Clear();
            ss.Request(
                line, 
                cbUpdateList, 
                parentID,
                tp== EnmMmlFileFormat.GWI ? ".gwi" : (
                tp == EnmMmlFileFormat.MUC ? ".muc" : (
                tp == EnmMmlFileFormat.MML ? ".mml" : 
                ""
                )),
                option
                );
        }

        private void cbUpdateList(List<SienItem> found)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<List<SienItem>>(cbUpdateList), found);
                return;
            }

            if (found == null || found.Count < 1)
            {
                return;
            }

            bool isFirst = true;
            bool isFirstFolder = true;

            TreeNode root = new TreeNode();
            treeView1.Nodes.Clear();
            foreach (SienItem si in found)
            {
                if (si == null) continue;
                if (si.tree == null) continue;

                string[] flds = si.tree.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                TreeNode tns = root;
                foreach (string fld in flds)
                {
                    TreeNode fnd = null;
                        foreach (TreeNode tn in tns.Nodes)
                        {
                            if (tn.Text == fld)
                            {
                                fnd = tn;
                                break;
                            }
                        }

                    if (fnd == null)
                    {
                        fnd = new TreeNode(fld);
                        tns.Nodes.Add(fnd);
                    }
                    tns = fnd;
                }

                if (si.sienType == 1)
                {
                    GetInstrument(si, tns, isFirst);
                    isFirst = false;
                }
                else if (si.sienType == 2)
                {
                    GetInstrumentFromFolder(si,tns, isFirstFolder);
                    isFirstFolder = false;
                }
                else
                {
                    TreeNode n = new TreeNode(si.title);
                    n.Tag = si;
                    tns.Nodes.Add(n);
                }
            }

            foreach(TreeNode tn in root.Nodes) treeView1.Nodes.Add(tn);

            if (isFirst) update();

            this.treeView1.Enabled = true;
        }


        private void GetInstrument(SienItem si, TreeNode tns, bool isFirst)
        {
            if (setting.OfflineMode) return;

            InstrumentAtValSound iavs = new InstrumentAtValSound();
            iavs.isFirst = isFirst;
            iavs.treenode = tns;
            string[] param = new string[6];
            string[] val = si.content.Split(',');
            for (int i = 0; i < param.Length; i++)
            {
                param[i] = val[i].Trim().Trim('\'');
            }

            if (!instCache.ContainsKey(si.content))
            {
                iavs.Start(
                    parent
                    , si
                    , param[0]
                    , param[1]
                    , Encoding.GetEncoding(param[2])
                    , param[3]
                    , param[4]
                    , param[5]
                    , GetInstrumentCompTN);
            }
            else
            {
                GetInstrumentCompTN(si, tns, instCache[si.content]);
                update();//キャッシュがある場合はここでupdateして大丈夫
            }
        }

        private void GetInstrumentCompTN(object sender,TreeNode tn, string[] obj)
        {
            if (!(sender is SienItem)) return;
            if (obj == null || obj.Length < 1) return;

            try
            {
                SienItem si = (SienItem)sender;

                foreach (string line in obj)
                {
                    SienItem ssi = new SienItem();
                    string lin = line;
                    if (lin.IndexOf("\r\n") >= 0)
                    {
                        lin = lin.Substring(0, line.IndexOf("\r\n")).Trim();
                    }
                    else
                    {
                        lin = lin.Substring(0, line.IndexOf("\n")).Trim();
                    }
                    ssi.title = string.Format(si.title, lin);
                    ssi.parentID = -1;// si.parentID;
                    ssi.content = line;
                    ssi.description = si.description;
                    ssi.foundCnt = si.foundCnt;
                    ssi.nextAnchor = si.nextAnchor;
                    ssi.nextCaret = si.nextCaret;
                    ssi.pattern = si.pattern;
                    ssi.patternType = si.patternType;
                    ssi.sienType = si.sienType + 1;

                    TreeNode n = new TreeNode(ssi.title);
                    n.Tag = ssi;
                    tn.Nodes.Add(n);
                }
                update();

                if (!instCache.ContainsKey(si.content))
                {
                    instCache.Add(si.content, obj);
                }
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
        }

        private void FrmSien_Load(object sender, EventArgs e)
        {
        }

        private void FrmSien_FormClosing(object sender, FormClosingEventArgs e)
        {
            ReConvertCacheList();

            e.Cancel = true;
            this.Hide();
            parentUpdate?.Invoke();
        }

        private void GetInstrumentFromFolder(SienItem si, TreeNode tn, bool isFirstFolder)
        {
            InstrumentAtLocal iavs = new InstrumentAtLocal();
            iavs.isFirst = isFirstFolder;
            iavs.treenode = tn;
            string path = Path.Combine(Common.GetApplicationFolder(), "Instruments");// "C:\\Users\\kuma\\Desktop\\FM-SoundLibrary-master";
            if (si.description != "")
            {
                path = si.description;
            }
            iavs.Start(
                parent, si, path, "", GetInstrumentFromFolderComp);
        }

        private void GetInstrumentFromFolderComp(object sender,TreeNode tn, List<Tuple<InstrumentAtLocal.enmInstType, string, string, string>> obj)
        {
            if (!(sender is SienItem)) return;
            if (obj == null || obj.Count < 1) return;

            try
            {
                SienItem si = (SienItem)sender;
                InstrumentAtLocal.enmInstType tp = InstrumentAtLocal.enmInstType.FM_L;
                foreach (InstrumentAtLocal.enmInstType e in Enum.GetValues(typeof(InstrumentAtLocal.enmInstType)))
                {
                    if (e.ToString() != si.content) continue;
                    tp = e;
                    break;
                }

                int depth = -1;
                foreach (Tuple<InstrumentAtLocal.enmInstType, string, string,string> line in obj)
                {
                    if (line.Item1 != InstrumentAtLocal.enmInstType.Dir) continue;

                    depth++;
                    SienItem ssi;
                    string name = Path.GetFileName(line.Item2);
                    string filePath = Path.GetDirectoryName(line.Item2);
                    string[] paths = filePath.Split(Path.DirectorySeparatorChar);
                    string lin = line.Item3;
                    string linMUC = line.Item4;
                    string treename = line.Item2.Replace(Path.Combine(Common.GetApplicationFolder(), "Instruments") + "\\", "");

                    ssi = new SienItem();
                    ssi.title = string.Format("{0}", name);
                    ssi.tree = treename;
                    ssi.ID = si.parentID;
                    ssi.parentID = si.ID;
                    ssi.haveChild = true;
                    ssi.content = si.content;
                    ssi.description = line.Item2;
                    ssi.foundCnt = si.foundCnt;
                    ssi.nextAnchor = si.nextAnchor;
                    ssi.nextCaret = si.nextCaret;
                    ssi.pattern = si.pattern;
                    ssi.patternType = si.patternType;
                    ssi.sienType = si.sienType;

                    string[] flds = ssi.tree.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                    TreeNode tns = tn;
                    foreach (string fld in flds)
                    {
                        TreeNode fnd = null;
                        foreach (TreeNode t in tns.Nodes)
                        {
                            if (t.Text == fld)
                            {
                                fnd = t;
                                break;
                            }
                        }

                        if (fnd == null)
                        {
                            fnd = new TreeNode(fld);
                            tns.Nodes.Add(fnd);
                        }
                        tns = fnd;
                    }

                }

                foreach (Tuple<InstrumentAtLocal.enmInstType, string, string, string> line in obj)
                {
                    if (line.Item1 == InstrumentAtLocal.enmInstType.Dir) continue;

                    depth++;
                    SienItem ssi;
                    string name = Path.GetFileName(line.Item2);
                    string filePath = Path.GetDirectoryName(line.Item2);
                    string[] paths = filePath.Split(Path.DirectorySeparatorChar);
                    string lin = line.Item3;
                    string linMUC = line.Item4;
                    string treename = line.Item2.Replace(Path.Combine(Common.GetApplicationFolder(), "Instruments") + "\\", "");

                    ssi = new SienItem();
                    ssi.title = string.Format("{0}", name);
                    ssi.tree = treename;
                    ssi.ID = si.parentID;
                    ssi.parentID = si.ID;
                    ssi.haveChild = true;
                    ssi.content = lin;// si.content;
                    ssi.content2 = linMUC;
                    ssi.description = line.Item2;
                    ssi.foundCnt = si.foundCnt;
                    ssi.nextAnchor = si.nextAnchor;
                    ssi.nextCaret = si.nextCaret;
                    ssi.pattern = si.pattern;
                    ssi.patternType = si.patternType;
                    ssi.sienType = si.sienType;

                    string[] flds = ssi.tree.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                    TreeNode tns = tn;
                    foreach (string fld in flds)
                    {
                        TreeNode fnd = null;
                        foreach (TreeNode t in tns.Nodes)
                        {
                            if (t.Text == fld)
                            {
                                fnd = t;
                                break;
                            }
                        }

                        if (fnd == null)
                        {
                            fnd = new TreeNode(fld);
                            fnd.Tag = ssi;
                            tns.Nodes.Add(fnd);
                        }
                        tns = fnd;
                    }

                }

                update();

                if (!instCacheFolder.ContainsKey(si.content))
                {
                    instCacheFolder.Add(si.content, obj.ToArray());
                }
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }

        }

        private void FrmSien_Shown(object sender, EventArgs e)
        {
            Request("'", Point.Empty, 0, EnmMmlFileFormat.GWI, null);
        }

        private void treeView1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 13)
            {
                return;
            }
            if (treeView1.SelectedNode == null) return;
            if (treeView1.SelectedNode.Tag == null) return;
            if (!(treeView1.SelectedNode.Tag is SienItem)) return;
            e.Handled = true;
            SienItem si = (SienItem)treeView1.SelectedNode.Tag;

            ReplaceContents(si);
        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node == null) return;
            if (e.Node.Tag == null) return;
            if (!(e.Node.Tag is SienItem)) return;
            SienItem si = (SienItem)e.Node.Tag;
            ReplaceContents(si);
        }

        private void ReplaceContents(SienItem si)
        {
            DockContent dc = (DockContent)parent.GetActiveDockContent();
            Document d;

            if (dc == null) return;
            if (!(dc.Tag is Document)) return;

            try
            {
                d = (Document)dc.Tag;
                int ci = d.editor.azukiControl.CaretIndex;
                d.editor.azukiControl.Document.Replace(
                    si.content,
                    ci,// - si.foundCnt,
                    ci);

                //d.editor.azukiControl.SetSelection(ci - si.foundCnt + si.nextAnchor, ci - si.foundCnt + si.nextCaret);
                d.editor.azukiControl.SetSelection(ci + si.nextAnchor, ci + si.nextCaret);
            }
            catch { }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            label2.Text = "";
            if (treeView1.SelectedNode == null) return;
            if (treeView1.SelectedNode.Tag == null) return;
            if (!(treeView1.SelectedNode.Tag is SienItem)) return;

            SienItem si = (SienItem)treeView1.SelectedNode.Tag;
            label2.Text = si.description;

        }
    }

}
