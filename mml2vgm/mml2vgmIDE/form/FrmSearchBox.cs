﻿using System;
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
            this.cmbPattern.BackColor = Color.FromArgb(setting.ColorScheme.SearchBox_BackColor);
            this.cmbPattern.ForeColor = Color.FromArgb(setting.ColorScheme.SearchBox_ForeColor);
            this.btnPrevious.BackColor = Color.FromArgb(setting.ColorScheme.SearchBox_BackColor);
            this.btnPrevious.ForeColor = Color.FromArgb(setting.ColorScheme.SearchBox_ForeColor);
            this.btnNext.BackColor = Color.FromArgb(setting.ColorScheme.SearchBox_BackColor);
            this.btnNext.ForeColor = Color.FromArgb(setting.ColorScheme.SearchBox_ForeColor);

            this.cmbPattern.Items.AddRange(setting.other.SearchWordHistory.ToArray());
        }

        private void BtnPrevious_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void cmbPattern_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down && !cmbPattern.DroppedDown) cmbPattern.DroppedDown = true;
            if (e.KeyCode == Keys.Enter) BtnNext_Click(null, null);
            if (e.KeyCode == Keys.Escape) this.Close();
        }
    }
}
