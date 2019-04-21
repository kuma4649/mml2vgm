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
        private SienSearch ss;
        public int selRow = -1;

        public FrmSien()
        {
            InitializeComponent();
            ss = new SienSearch("mmlSien.json");

            //Sien sien = JsonConvert.DeserializeObject<Sien>(System.IO.File.ReadAllText("mmlSien.json"));
            //foreach(var item in sien.list)
            //{
            //    dgvItem.Rows.Add(item.title);
            //    dgvItem.Rows[dgvItem.Rows.Count - 1].Tag = item;
            //}
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

            foreach (SienItem si in found)
            {
                dgvItem.Rows.Add(
                    si.title,
                    si.description,
                    si.content);
                dgvItem.Rows[dgvItem.Rows.Count - 1].Tag = si;
            }
            if (selRow==-1 && dgvItem.SelectedRows.Count > 0)
            {
                dgvItem.SelectedRows[0].Selected = false;
            }
            update();
        }
    }

}
