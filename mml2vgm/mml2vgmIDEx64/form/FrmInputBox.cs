using System;
using System.Drawing;
using System.Windows.Forms;

namespace mml2vgmIDE
{
    public partial class FrmInputBox : Form
    {
        public string title = "";
        public string resultText = "";
        private Setting setting = null;

        public FrmInputBox(Setting setting)
        {
            InitializeComponent();
            this.setting = setting;

            Common.SetBFColor(this, setting);
            this.Opacity = setting.other.Opacity / 100.0;
        }

        private void FrmInputBox_Load(object sender, EventArgs e)
        {
            this.Location = new Point(setting.location.RInputBox.X, setting.location.RInputBox.Y);
            this.Text = title;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.resultText = txtText.Text;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            this.resultText = txtText.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void FrmInputBox_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                setting.location.RInputBox = new Rectangle(Location.X, Location.Y, 0, 0);
            }
            else
            {
                setting.location.RInputBox = new Rectangle(RestoreBounds.Location.X, RestoreBounds.Location.Y, 0, 0);
            }

        }
    }
}
