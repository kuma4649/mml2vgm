using musicDriverInterface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Permissions;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace mml2vgmIDE
{
    public partial class FrmSien : Form
    {
        public FrmMain parent = null;
        public Setting setting = null;
        private SienSearch ss;
        public int selRow = -1;
        private Dictionary<string, string[]> instCache = new Dictionary<string, string[]>();
        private int tw;
        private int th;

        public FrmSien(Setting setting)
        {
            InitializeComponent();
            ss = new SienSearch(System.IO.Path.Combine(Common.GetApplicationFolder(), "mmlSien.json"));
            this.setting = setting;
            if (setting.sien.CacheInstrumentName != null)
            {
                instCache = ConvertCacheList(setting.sien);
            }
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

        protected override CreateParams CreateParams
        {
            [SecurityPermission(SecurityAction.Demand,
                Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                const int WS_EX_TOOLWINDOW = 0x80;
                const long WS_POPUP = 0x80000000L;
                const int WS_VISIBLE = 0x10000000;
                const int WS_SYSMENU = 0x80000;
                const int WS_MAXIMIZEBOX = 0x10000;

                CreateParams cp = base.CreateParams;
                cp.ExStyle = WS_EX_TOOLWINDOW;
                cp.Style = unchecked((int)WS_POPUP) |
                    WS_VISIBLE | WS_SYSMENU | WS_MAXIMIZEBOX;

                if (this.Width != 0)
                {
                    tw = this.Width;
                    th = this.Height;
                }

                cp.Width = 0;
                cp.Height = 0;

                return cp;
            }
        }

        private void dgvItem_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                dgvItem.Rows[e.RowIndex].Selected = true;
                DockContent dc = (DockContent)parent.GetActiveDockContent();
                Document d = null;
                if (dc != null)
                {
                    if (dc.Tag is Document)
                    {
                        d = (Document)dc.Tag;
                        int ci = d.editor.azukiControl.CaretIndex;
                        SienItem si = (SienItem)dgvItem.Rows[e.RowIndex].Tag;
                        d.editor.azukiControl.Document.Replace(
                            si.content,
                            ci - si.foundCnt,
                            ci);

                        d.editor.azukiControl.SetSelection(ci - si.foundCnt + si.nextAnchor, ci - si.foundCnt + si.nextCaret);
                    }
                }
            }

            this.SetOpacity(false);
            parent.Activate();
        }

        public void update()
        {
            this.Height = Math.Min(5 * (dgvItem.RowTemplate.Height + 1), dgvItem.RowCount * (dgvItem.RowTemplate.Height + 1));
        }

        public void Request(string line, Point ciP,int parentID,EnmMmlFileFormat tp)
        {
            Location = new Point(ciP.X, ciP.Y);
            dgvItem.Rows.Clear();
            ss.Request(
                line, 
                cbUpdateList, 
                parentID,
                tp== EnmMmlFileFormat.GWI ? ".gwi" : (
                tp == EnmMmlFileFormat.MUC ? ".muc" : (
                tp == EnmMmlFileFormat.MML ? ".mml" : 
                ""
                )));
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
                this.SetOpacity(false);
                selRow = -1;
                return;
            }

            this.SetOpacity(setting.UseSien);

            bool isFirst = true;
            foreach (SienItem si in found)
            {
                if (si.sienType == 1)
                {
                    if (!parent.setting.OfflineMode)
                    {
                        GetInstrument(si, isFirst);
                        isFirst = false;
                    }
                }
                else
                {
                    dgvItem.Rows.Add(
                        si.title,
                        si.description,
                        si.content);
                    dgvItem.Rows[dgvItem.Rows.Count - 1].Tag = si;
                }
            }
            if (selRow == -1 && dgvItem.SelectedRows.Count > 0)
            {
                dgvItem.SelectedRows[0].Selected = false;
            }

            if (isFirst) update();
        }

        /// <summary>
        /// 0.0 非表示
        /// 1.0 表示
        /// </summary>
        /// <param name="sw"></param>
        public void SetOpacity(bool sw)
        {
            if (sw)
            {
                this.Width = tw;
                this.Height = th;
                return;
            }

            //既に非表示の状態の場合は何もせずに戻る
            //(フォーカスが切り替わるときちらつきが発生してしまう為)
            if (this.Width == 0) return;

            this.Width = 0;
            this.Height = 0;
            this.TopMost = true;
            if (parent != null) parent.Activate();
        }

        public bool GetOpacity()
        {
            //Console.WriteLine("{0}", this.Width);
            return this.Width != 0;
            //return Opacity==1.0;
            //return Visible;
        }

        private void GetInstrument(SienItem si, bool isFirst)
        {
            InstrumentAtValSound iavs = new InstrumentAtValSound();
            iavs.isFirst = isFirst;
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
                    , GetInstrumentComp);
            }
            else
            {
                GetInstrumentComp(si, instCache[si.content]);
                update();//キャッシュがある場合はここでupdateして大丈夫
            }
        }


        private void GetInstrumentComp(object sender, string[] obj)
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

                    dgvItem.Rows.Add(
                        ssi.title,
                        ssi.description,
                        ssi.content);
                    dgvItem.Rows[dgvItem.Rows.Count - 1].Tag = ssi;
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
            dgvItem.BackColor = Color.FromArgb(setting.ColorScheme.FrmSien_BackColor);
            dgvItem.ForeColor = Color.FromArgb(setting.ColorScheme.FrmSien_ForeColor);
            dgvItem.DefaultCellStyle.BackColor = Color.FromArgb(setting.ColorScheme.FrmSien_BackColor);
            dgvItem.DefaultCellStyle.ForeColor = Color.FromArgb(setting.ColorScheme.FrmSien_ForeColor);
        }

        private void FrmSien_FormClosing(object sender, FormClosingEventArgs e)
        {
            ReConvertCacheList();
        }
    }

}
