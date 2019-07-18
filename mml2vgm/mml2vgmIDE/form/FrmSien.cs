using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace mml2vgmIDE
{
    public partial class FrmSien : Form
    {
        public FrmMain parent = null;
        public Setting setting = null;
        private SienSearch ss;
        public int selRow = -1;
        private Dictionary<string, string[]> instCache = new Dictionary<string, string[]>();

        public FrmSien(Setting setting)
        {
            InitializeComponent();
            ss = new SienSearch(System.IO.Path.Combine(Common.GetApplicationFolder(), "mmlSien.json"));
            this.setting = setting;
        }

        private void dgvItem_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            this.Opacity = 0.0;
            parent.Activate();
        }

        public void update()
        {
            this.Height = Math.Min(5 * (dgvItem.RowTemplate.Height + 1), dgvItem.RowCount * (dgvItem.RowTemplate.Height + 1));
        }

        internal void Request(string line, Point ciP)
        {
            Location = new Point(ciP.X, ciP.Y);
            dgvItem.Rows.Clear();
            ss.Request(line, cbUpdateList);
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
                Opacity = 0.0;
                selRow = -1;
                return;
            }

            Opacity = 1.0;

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
            if (selRow==-1 && dgvItem.SelectedRows.Count > 0)
            {
                dgvItem.SelectedRows[0].Selected = false;
            }
            update();
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
                    ssi.title = string.Format(si.title, line.Substring(0, line.IndexOf("\r\n")).Trim());
                    ssi.content = line;
                    ssi.description = si.description;
                    ssi.foundCnt = si.foundCnt;
                    ssi.nextAnchor = si.nextAnchor;
                    ssi.nextCaret = si.nextCaret;
                    ssi.pattern = si.pattern;
                    ssi.patternType = si.patternType;
                    ssi.sienType = ssi.sienType;

                    dgvItem.Rows.Add(
                        ssi.title,
                        ssi.description,
                        ssi.content);
                    dgvItem.Rows[dgvItem.Rows.Count - 1].Tag = ssi;
                }

                if (!instCache.ContainsKey(si.content))
                {
                    instCache.Add(si.content, obj);
                }
            }
            catch(Exception ex)
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
    }

}
