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
    public partial class FrmSearchBox : Form
    {
        public FrmSearchBox(Setting setting)
        {
            InitializeComponent();
            DialogResult = DialogResult.Cancel;
            this.BackColor = Color.FromArgb(setting.ColorScheme.SearchBox_BackColor);
            this.ForeColor = Color.FromArgb(setting.ColorScheme.SearchBox_ForeColor);
            this.tbPattern.BackColor = Color.FromArgb(setting.ColorScheme.SearchBox_BackColor);
            this.tbPattern.ForeColor = Color.FromArgb(setting.ColorScheme.SearchBox_ForeColor);
            this.btnPrevious.BackColor = Color.FromArgb(setting.ColorScheme.SearchBox_BackColor);
            this.btnPrevious.ForeColor = Color.FromArgb(setting.ColorScheme.SearchBox_ForeColor);
            this.btnNext.BackColor = Color.FromArgb(setting.ColorScheme.SearchBox_BackColor);
            this.btnNext.ForeColor = Color.FromArgb(setting.ColorScheme.SearchBox_ForeColor);

        }

        private void BtnPrevious_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}
