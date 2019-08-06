using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace mml2vgmIDE
{
    public partial class FrmPartCounter : DockContent, IForm
    {
        public Action parentUpdate = null;
        private MMLParameter.Manager mmlParams = null;
        private Setting setting = null;

        public FrmPartCounter(Setting setting)
        {
            InitializeComponent();
            this.setting = setting;

            dgvPartCounter.BackgroundColor = Color.FromArgb(setting.ColorScheme.PartCounter_BackColor);
            dgvPartCounter.DefaultCellStyle.BackColor = Color.FromArgb(setting.ColorScheme.PartCounter_BackColor);
            dgvPartCounter.ForeColor = Color.FromArgb(setting.ColorScheme.PartCounter_ForeColor);
            EnableDoubleBuffering(dgvPartCounter);
            SetDisplayIndex(setting.location.PartCounterClmInfo);
        }

        public void ClearCounter()
        {
            dgvPartCounter.Rows.Clear();
        }

        public void AddPartCounter(object[] cells)
        {
            DataGridViewRow r = new DataGridViewRow();
            r.CreateCells(dgvPartCounter);

            r.Cells[dgvPartCounter.Columns["ClmPartNumber"].Index].Value = cells[0];
            r.Cells[dgvPartCounter.Columns["ClmIsSecondary"].Index].Value = cells[1];
            r.Cells[dgvPartCounter.Columns["ClmPart"].Index].Value = cells[2];
            r.Cells[dgvPartCounter.Columns["ClmChip"].Index].Value = cells[3];
            r.Cells[dgvPartCounter.Columns["ClmCOunter"].Index].Value = cells[4];

            dgvPartCounter.Rows.Add(r);
        }

        public void Start(MMLParameter.Manager mmlParams)
        {
            timer.Enabled = true;
            this.mmlParams = mmlParams;
        }

        public void Stop()
        {
            timer.Enabled = false;
            mmlParams = null;
        }


        /// <summary>
        /// ダブルバッファリングを有効にする(from DOBON)
        /// </summary>
        public static void EnableDoubleBuffering(Control control)
        {
            control.GetType().InvokeMember(
               "DoubleBuffered",
               BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
               null,
               control,
               new object[] { true });
        }

        private void FrmPartCounter_FormClosing(object sender, FormClosingEventArgs e)
        {
            setting.location.PartCounterClmInfo = getDisplayIndex();

            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
                parentUpdate?.Invoke();
                return;
            }
        }


        private void FrmPartCounter_FormClosed(object sender, FormClosedEventArgs e)
        {


        }

        protected override string GetPersistString()
        {
            return this.Name;
        }

        private void FrmPartCounter_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.None;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (mmlParams == null) return;

            //パラメータ取得

            //パラメータ描画
            dgvPartCounter.SuspendLayout();

            for (int p = 0; p < dgvPartCounter.Rows.Count; p++)
            {
                string chip = (string)dgvPartCounter.Rows[p].Cells["ClmChip"].Value;
                int r = (int)dgvPartCounter.Rows[p].Cells["ClmPartNumber"].Value - 1;
                int isSecondary = ((bool)dgvPartCounter.Rows[p].Cells["ClmIsSecondary"].Value ? 1 : 0);

                if (mmlParams.Insts.ContainsKey(chip))
                {
                    MMLParameter.Instrument mmli = mmlParams.Insts[chip][isSecondary];

                    dgvPartCounter.Rows[p].Cells["ClmInstrument"].Value = mmli.inst[r] == null ? "-" : mmli.inst[r].ToString();
                    dgvPartCounter.Rows[p].Cells["ClmEnvelope"].Value = mmli.envelope[r] == null ? "-" : mmli.envelope[r].ToString();
                    dgvPartCounter.Rows[p].Cells["ClmVolume"].Value = mmli.vol[r] == null ? "-" : mmli.vol[r].ToString();
                    dgvPartCounter.Rows[p].Cells["ClmPan"].Value = mmli.pan[r] == null ? "-" : mmli.pan[r];
                    dgvPartCounter.Rows[p].Cells["ClmNote"].Value = mmli.notecmd[r] == null ? "-" : mmli.notecmd[r];
                    dgvPartCounter.Rows[p].Cells["ClmLength"].Value = mmli.length[r] == null ? "-" : mmli.length[r];
                }
            }

            dgvPartCounter.ResumeLayout();
        }

        private dgvColumnInfo[] getDisplayIndex()
        {
            List<dgvColumnInfo> ret = new List<dgvColumnInfo>();

            for (int i = 0; i < dgvPartCounter.Columns.Count; i++)
            {
                dgvColumnInfo info = (dgvColumnInfo)dgvPartCounter.Columns[i].Tag;
                info.columnName = dgvPartCounter.Columns[i].Name;
                info.displayIndex = dgvPartCounter.Columns[i].DisplayIndex;
                info.size = dgvPartCounter.Columns[i].Width;
                info.visible = dgvPartCounter.Columns[i].Visible;

                ret.Add(info);
            }

            return ret.ToArray();
        }

        private void SetDisplayIndex(dgvColumnInfo[] aryIndex)
        {
            if (aryIndex == null || aryIndex.Length < 1) return;

            for (int i = 0; i < aryIndex.Length; i++)
            {
                if (!dgvPartCounter.Columns.Contains(aryIndex[i].columnName)) continue;

                dgvPartCounter.Columns[aryIndex[i].columnName].DisplayIndex = aryIndex[i].displayIndex;
                dgvPartCounter.Columns[aryIndex[i].columnName].Width = Math.Max(aryIndex[i].size, 10);
                dgvPartCounter.Columns[aryIndex[i].columnName].Visible = aryIndex[i].visible;
                dgvPartCounter.Columns[aryIndex[i].columnName].Tag = aryIndex[i];
            }

            //spacerは常に最後にする
            dgvPartCounter.Columns["ClmSpacer"].DisplayIndex = dgvPartCounter.Columns.Count  - 1;
        }

        private void DgvPartCounter_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void DgvPartCounter_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void DgvPartCounter_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            if (e.RowIndex != -1) return;
            if (setting == null || setting.location == null || setting.location.PartCounterClmInfo == null) return;

            //メニューのアイテムを生成する
            //  hide / show all / セパレータの追加
            cmsMenu.Items.Clear();
            string txt = dgvPartCounter.Columns[e.ColumnIndex].HeaderText;
            if (!string.IsNullOrEmpty(txt))
            {
                cmsMenu.Items.Add(string.Format("Hide {0}", txt));
                cmsMenu.Items[0].Tag = dgvPartCounter.Columns[e.ColumnIndex].Tag;
                cmsMenu.Items[0].Click += MenuItem_Click;
            }
            cmsMenu.Items.Add("Show all");
            cmsMenu.Items[cmsMenu.Items.Count - 1].Click += MenuItem_Click;
            cmsMenu.Items.Add("-");

            //  その他の列を全て追加する
            foreach (DataGridViewColumn c in dgvPartCounter.Columns)
            {
                if (txt == c.HeaderText) continue;
                if (string.IsNullOrEmpty(c.HeaderText)) continue;
                if (c.Name == "ClmPartNumber") continue;
                if (c.Name == "ClmIsSecondary") continue;

                cmsMenu.Items.Add(c.HeaderText);
                cmsMenu.Items[cmsMenu.Items.Count - 1].Tag = c.Tag;
                cmsMenu.Items[cmsMenu.Items.Count - 1].Click += MenuItem_Click;
                ((ToolStripMenuItem)cmsMenu.Items[cmsMenu.Items.Count - 1]).Checked = c.Visible;
            }

            cmsMenu.Show(Cursor.Position);

        }

        private void CmsMenu_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyValue != 13) return;
            foreach (ToolStripItem i in cmsMenu.Items)
            {
                if (!i.Selected) continue;
                //MenuItem_Click(i, null);
                break;
            }
        }

        private void MenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem i = (ToolStripMenuItem)sender;
            foreach(DataGridViewColumn c in dgvPartCounter.Columns)
            {
                if (c.Tag != i.Tag) continue;
                c.Visible = !c.Visible;
                return;
            }

            //show all
            foreach (DataGridViewColumn c in dgvPartCounter.Columns)
            {
                if (c.Name == "ClmPartNumber") continue;
                if (c.Name == "ClmIsSecondary") continue;
                c.Visible = true;
                c.Width = Math.Max(c.Width, 10);
            }
        }

    }

    public class dgvColumnInfo
    {
        public string columnName = "";
        public int displayIndex = 0;
        public int size = 10;
        public bool visible = true;
    }
}