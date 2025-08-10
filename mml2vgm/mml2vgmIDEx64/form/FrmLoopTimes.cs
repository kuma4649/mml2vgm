using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mml2vgmIDEx64.form
{
    public partial class FrmLoopTimes : Form
    {
        public FrmLoopTimes()
        {
            InitializeComponent();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int count { get; internal set; }

        private void FrmLoopTimes_Load(object sender, EventArgs e)
        {
            tbLoopCount.Text = count.ToString();
        }

        private void FrmLoopTimes_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (int.TryParse(tbLoopCount.Text, out int i))
            {
                count = i;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
