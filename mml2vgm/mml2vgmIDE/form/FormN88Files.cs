using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mml2vgmIDE.form
{
    public partial class FormN88Files : Form
    {
        public int n = -1;

        public FormN88Files()
        {
            InitializeComponent();
        }

        private void dgvFiles_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            n= e.RowIndex * 5 + e.ColumnIndex;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
