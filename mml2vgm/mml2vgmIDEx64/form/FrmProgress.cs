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
    public partial class FrmProgress : Form
    {
        public FrmProgress()
        {
            InitializeComponent();
        }

        private void btnAbort_Click(object sender, EventArgs e)
        {
            Audio.waveModeAbort = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!Audio.waveMode) this.Close();
            lblCOunter.Text = string.Format("Seq:{0}\r\nLoop:{1}", Audio.EmuSeqCounter, Audio.GetVgmCurLoopCounter());
            this.Size = new Size(320, 160);
        }

        private void FrmProgress_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Audio.waveMode) 
                Audio.waveModeAbort = true;
        }
    }
}
