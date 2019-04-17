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

        public FrmSien()
        {
            InitializeComponent();

            Sien sien = JsonConvert.DeserializeObject<Sien>(System.IO.File.ReadAllText("mmlSien.json"));
            foreach(var item in sien.list)
            {
                dgvItem.Rows.Add(item.title);
                dgvItem.Rows[dgvItem.Rows.Count - 1].Tag = item;
            }
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
    }

}
