﻿using System;
using System.Windows.Forms;

namespace mml2vgmIDEx64
{
    public partial class ucSettingInstruments : UserControl
    {
        public ucSettingInstruments()
        {
            InitializeComponent();
        }

        private void cbSendWait_CheckedChanged(object sender, EventArgs e)
        {
            cbTwice.Enabled = cbSendWait.Checked;
        }
    }
}
