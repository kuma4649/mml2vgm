using mml2vgmIDE.MMLParameter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace mml2vgmIDE
{
    public partial class FrmPartCounter : DockContent, IForm
    {
        public Action parentUpdate = null;
        private MMLParameter.Manager mmlParams = null;
        private muteManager muteManager = null;
        private Setting setting = null;
        private Brush[][] meterBrush = new Brush[9][];
        private bool SoloMode = false;
        //private List<Tuple<string, int, int, int, bool, bool, bool>> lstCacheMuteSolo = new List<Tuple<string, int, int, int, bool, bool, bool>>();

        private const string clmKBDAssign = "ClmKBDAssign";
        private const string Assign = "Assign";


        public FrmPartCounter(Setting setting,muteManager muteManager)
        {
            InitializeComponent();
            this.setting = setting;
            this.muteManager = muteManager;

            dgvPartCounter.BackgroundColor = Color.FromArgb(setting.ColorScheme.PartCounter_BackColor);
            dgvPartCounter.DefaultCellStyle.BackColor = Color.FromArgb(setting.ColorScheme.PartCounter_BackColor);
            dgvPartCounter.ForeColor = Color.FromArgb(setting.ColorScheme.PartCounter_ForeColor);
            EnableDoubleBuffering(dgvPartCounter);
            SetDisplayIndex(setting.location.PartCounterClmInfo);
            foreach (DataGridViewColumn c in dgvPartCounter.Columns)
            {
                //c.Visible = true;
                //continue;
                if (
                    c.Name == "ClmChipIndex"
                    || c.Name == "ClmChipNumber"
                    || c.Name == "ClmPartNumber"
                    || c.Name == "ClmchipNumber"
                    || c.Name == "ClmPush"
                    ) c.Visible = false;
            }

            double r = 0;
            double g = 0;
            double b = 0;
            double sr = 0;
            double sg = 0;
            double sb = 0;
            double tr = 0;
            double tg = 0;
            double tb = 0;
            double div = 0;
            int cnt = 0;

            double[][] bar = new double[9][]
            {
                //black 
                new double[]{    140 , 120 , 120   , 80 ,  80 ,  80   , 70 ,  70 ,  70   , 40 ,  40 ,  40   }
                //blue 
                , new double[]{  140 , 120 , 215   , 80 ,  80 , 160   , 70 ,  70 , 120   , 40 ,  40 ,  80   }
                //red 
                , new double[]{  235 , 120 , 140   ,160 ,  80 ,  80   ,120 ,  70 ,  70   , 80 ,  40 ,  40   }
                //purple
                , new double[]{  235 , 120 , 215   ,160 ,  80 , 160   ,120 ,  70 , 120   , 80 ,  40 ,  80   }
                //green
                , new double[]{  140 , 215 , 120   , 80 , 160 ,  80   , 70 , 120 ,  70   , 40 ,  80 ,  40   }
                //aqua
                , new double[]{  140 , 215 , 215   , 80 , 160 , 160   , 70 , 120 , 120   , 40 ,  80 ,  80   }
                //yellow
                , new double[]{  235 , 215 , 120   ,160 , 160 ,  80   ,120 , 120 ,  70   , 80 ,  80 ,  40   }
                //white
                , new double[]{  235 , 215 , 215   ,160 , 160 , 160   ,120 , 120 , 120   , 80 ,  80 ,  80   }
                //muap
                , new double[]{  255 ,   0 ,   0   ,255 , 255 ,   0   ,  0 , 255 ,   0   ,  0 ,   0 , 255   }
            };

            for (int j = 0; j < meterBrush.Length; j++)
            {
                meterBrush[j] = new Brush[256];
                for (int i = 0; i < 256; i++)
                {
                    if (i == 0)
                    {
                        r = bar[j][0];
                        g = bar[j][1];
                        b = bar[j][2];

                        sr = r;
                        sg = g;
                        sb = b;
                        tr = bar[j][3];
                        tg = bar[j][4];
                        tb = bar[j][5];

                        div = 20.0;
                        cnt = 20;
                    }
                    if (i == 80)
                    {
                        sr = r;
                        sg = g;
                        sb = b;
                        tr = bar[j][6];
                        tg = bar[j][7];
                        tb = bar[j][8];

                        div = 60.0;
                        cnt = 60;
                    }
                    if (i == 210)
                    {
                        sr = r;
                        sg = g;
                        sb = b;
                        tr = bar[j][9];
                        tg = bar[j][10];
                        tb = bar[j][11];

                        div = 40.0;
                        cnt = 40;
                    }

                    if (cnt > 0)
                    {
                        r += (tr - sr) / div;
                        g += (tg - sg) / div;
                        b += (tb - sb) / div;
                        cnt--;
                    }

                    Color c = Color.FromArgb(255
                        , Math.Max(Math.Min((int)r, 255), 0)
                        , Math.Max(Math.Min((int)g, 255), 0)
                        , Math.Max(Math.Min((int)b, 255), 0)
                        );
                    meterBrush[j][255 - i] = new SolidBrush(c);
                }
            }
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
            for (int j = 0; j < meterBrush.Length; j++)
                for (int i = 0; i < meterBrush[j].Length; i++)
                {
                    meterBrush[j][i].Dispose();
                }

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
                int r = (int)dgvPartCounter.Rows[p].Cells["ClmPartNumber"].Value -1;
                if (r < 0)
                    continue;
                int chipIndex = (int)dgvPartCounter.Rows[p].Cells["ClmChipIndex"].Value;
                int chipNumber = (int)dgvPartCounter.Rows[p].Cells["ClmChipNumber"].Value;

                if (!mmlParams.Insts.ContainsKey(chip)) 
                    continue;
                if (!mmlParams.Insts[chip].ContainsKey(chipIndex) || !mmlParams.Insts[chip][chipIndex].ContainsKey(chipNumber)) 
                    continue;

                MMLParameter.Instrument mmli = mmlParams.Insts[chip][chipIndex][chipNumber];

                if(mmli is YM2608_mucom)
                {
                    string cp = (string)dgvPartCounter.Rows[p].Cells["ClmPart"].Value;
                    int ch = cp[0]- (chipNumber == 0 ? 'A' : 'L');
                    int pg = cp.Length < 2 ? 0 : (cp[1] - '0');
                    r = ch * 10 + pg;
                }
                else if (mmli is YM2610B_mucom)
                {
                    string cp = (string)dgvPartCounter.Rows[p].Cells["ClmPart"].Value;
                    int ch = cp[0] - (chipNumber == 0 ? 'a' : 'l');
                    int pg = cp.Length < 2 ? 0 : (cp[1] - '0');
                    r = ch * 10 + pg;
                }
                else if (mmli is YM2151_mucom)
                {
                    string cp = (string)dgvPartCounter.Rows[p].Cells["ClmPart"].Value;
                    int ch = (cp[0] < 'w') ? (cp[0] - 'W') : (cp[0] - 'w' + 4);
                    int pg = cp.Length < 2 ? 0 : (cp[1] - '0');
                    r = ch * 10 + pg;
                }

                if (r >= mmli.inst.Length) continue;

                dgvPartCounter.Rows[p].Cells["ClmInstrument"].Value = mmli.inst[r] == null ? "-" : mmli.inst[r].ToString();
                dgvPartCounter.Rows[p].Cells["ClmEnvelope"].Value = mmli.envelope[r] == null ? "-" : mmli.envelope[r].ToString();
                dgvPartCounter.Rows[p].Cells["ClmVolume"].Value = mmli.vol[r] == null ? "-" : mmli.vol[r].ToString();
                dgvPartCounter.Rows[p].Cells["ClmExpression"].Value = mmli.expression[r] == null ? "-" : mmli.expression[r].ToString();
                dgvPartCounter.Rows[p].Cells["ClmVelocity"].Value = mmli.velocity[r] == null ? "-" : mmli.velocity[r].ToString();
                dgvPartCounter.Rows[p].Cells["ClmPan"].Value = mmli.pan[r] == null ? "-" : mmli.pan[r];
                dgvPartCounter.Rows[p].Cells["ClmGateTime"].Value = mmli.gatetime[r] == null ? "-" : mmli.gatetime[r];
                dgvPartCounter.Rows[p].Cells["ClmNote"].Value = mmli.notecmd[r] == null ? "-" : mmli.notecmd[r];
                dgvPartCounter.Rows[p].Cells["ClmLength"].Value = mmli.length[r] == null ? "-" : mmli.length[r];
                dgvPartCounter.Rows[p].Cells["ClmEnvSw"].Value = mmli.envSw[r] == null ? "-" : mmli.envSw[r];
                dgvPartCounter.Rows[p].Cells["ClmLfoSw"].Value = mmli.lfoSw[r] == null ? "-" : mmli.lfoSw[r];
                dgvPartCounter.Rows[p].Cells["ClmLfo"].Value = mmli.lfo[r] == null ? "-" : mmli.lfo[r];
                dgvPartCounter.Rows[p].Cells["ClmDetune"].Value = mmli.detune[r] == null ? "-" : mmli.detune[r].ToString();
                dgvPartCounter.Rows[p].Cells["ClmKeyShift"].Value = mmli.keyShift[r] == null ? "-" : mmli.keyShift[r].ToString();
                dgvPartCounter.Rows[p].Cells["ClmMIDIch"].Value = mmli.MIDIch[r] == null ? "-" : mmli.MIDIch[r].ToString();
                DrawMeter(dgvPartCounter.Rows[p].Cells["ClmMeter"], mmli, r);
            }

            dgvPartCounter.ResumeLayout();
        }

        private void DgvPartCounter_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            string cellName = dgvPartCounter.Columns[e.ColumnIndex].Name;

            if (cellName != "ClmMute" && cellName != "ClmSolo" && cellName != clmKBDAssign) return;
            switch (cellName)
            {
                case "ClmMute":
                    ClickMUTE(e.RowIndex);
                    break;
                case "ClmSolo":
                    ClickSOLO(e.RowIndex);
                    break;
                case clmKBDAssign:
                    ClickKBDAssign(e.RowIndex);
                    break;
            }
        }

        private void DgvPartCounter_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            if (e.RowIndex != -1) return;
            if (setting == null || setting.location == null) return;
            if (setting.location.PartCounterClmInfo == null)
            {
                setting.location.PartCounterClmInfo = getDisplayIndex();
            }

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
                if (c.Name == "ClmChipIndex") continue;
                if (c.Name == "ClmChipNumber") continue;
                if (c.Name == "ClmPartNumber") continue;
                if (c.Name == "ClmchipNumber") continue;
                if (c.Name == "ClmMuteMngKey") continue;

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
            foreach (DataGridViewColumn c in dgvPartCounter.Columns)
            {
                if (c.Tag != i.Tag) continue;
                c.Visible = !c.Visible;
                return;
            }

            //show all
            foreach (DataGridViewColumn c in dgvPartCounter.Columns)
            {
                if (c.Name == "ClmChipIndex") continue;
                if (c.Name == "ClmChipNumber") continue;
                if (c.Name == "ClmPartNumber") continue;
                if (c.Name == "ClmchipNumber") continue;
                if (c.Name == "ClmMuteMngKey") continue;

                c.Visible = true;
                c.Width = Math.Max(c.Width, 10);
            }
        }



        public void ClearCounter()
        {
            CacheMuteSolo();

            dgvPartCounter.Rows.Clear();
            //SoloMode = false;
        }

        private void CacheMuteSolo()
        {
            //lstCacheMuteSolo.Clear();
            foreach (DataGridViewRow r in dgvPartCounter.Rows)
            {
                Tuple<string, int, int, int, bool, bool, bool> t = new Tuple<string, int, int, int, bool, bool, bool>(
                    (string)r.Cells[dgvPartCounter.Columns["ClmChip"].Index].Value
                    , (int)r.Cells[dgvPartCounter.Columns["ClmChipIndex"].Index].Value
                    , (int)r.Cells[dgvPartCounter.Columns["ClmChipNumber"].Index].Value
                    , (int)r.Cells[dgvPartCounter.Columns["ClmPartNumber"].Index].Value
                    , (bool)(r.Cells[dgvPartCounter.Columns["ClmMute"].Index].Value == null ? false : ((string)r.Cells[dgvPartCounter.Columns["ClmMute"].Index].Value == "M"))
                    , (bool)(r.Cells[dgvPartCounter.Columns["ClmPush"].Index].Value == null ? false : ((string)r.Cells[dgvPartCounter.Columns["ClmPush"].Index].Value == "M"))
                    , (bool)(r.Cells[dgvPartCounter.Columns["ClmSolo"].Index].Value == null ? false : ((string)r.Cells[dgvPartCounter.Columns["ClmSolo"].Index].Value == "S"))
                    );
                //lstCacheMuteSolo.Add(t);
            }
        }

        public void AddPartCounter(object[] cells)
        {
            DataGridViewRow r = new DataGridViewRow();
            r.CreateCells(dgvPartCounter);

            r.Cells[dgvPartCounter.Columns["ClmPartNumber"].Index].Value = cells[0];
            r.Cells[dgvPartCounter.Columns["ClmChipIndex"].Index].Value = cells[1];
            r.Cells[dgvPartCounter.Columns["ClmChipNumber"].Index].Value = cells[2];
            r.Cells[dgvPartCounter.Columns["ClmPart"].Index].Value = cells[3];
            r.Cells[dgvPartCounter.Columns["ClmChip"].Index].Value = cells[4];
            r.Cells[dgvPartCounter.Columns["ClmCOunter"].Index].Value = cells[5];
            r.Cells[dgvPartCounter.Columns["ClmLoopCounter"].Index].Value = cells[6];
            r.Cells[dgvPartCounter.Columns["ClmMuteMngKey"].Index].Value = cells[7];

            muteStatus ms = muteManager.GetStatus((int)cells[7]);
            if (ms != null)
            {
                r.Cells[dgvPartCounter.Columns["ClmMute"].Index].Value = ms.mute ? "M" : "";
                r.Cells[dgvPartCounter.Columns["ClmPush"].Index].Value = ms.cache ? "M" : "";
                r.Cells[dgvPartCounter.Columns["ClmSolo"].Index].Value = ms.solo ? "S" : "";
            }
            else
            {
                r.Cells[dgvPartCounter.Columns["ClmMute"].Index].Value = "";
                r.Cells[dgvPartCounter.Columns["ClmPush"].Index].Value = "";
                r.Cells[dgvPartCounter.Columns["ClmSolo"].Index].Value = "";
            }


            ////mute状態を復帰する
            //bool fnd = false;
            //foreach (Tuple<string, int, int, int, bool, bool, bool> l in lstCacheMuteSolo)
            //{
            //    if (l.Item1 != (string)cells[4]
            //        || l.Item2 != (int)cells[1]
            //        || l.Item3 != (int)cells[2]
            //        || l.Item4 != (int)cells[0])
            //        continue;

            //    r.Cells[dgvPartCounter.Columns["ClmMute"].Index].Value = (bool)l.Item5 ? "M" : "";
            //    r.Cells[dgvPartCounter.Columns["ClmPush"].Index].Value = (bool)l.Item6 ? "M" : "";
            //    r.Cells[dgvPartCounter.Columns["ClmSolo"].Index].Value = (bool)l.Item7 ? "S" : "";
            //    //if (SoloMode) r.Cells[dgvPartCounter.Columns["ClmSolo"].Index].Value = (!(bool)l.Item5) ? "S" : "";
            //    //else r.Cells[dgvPartCounter.Columns["ClmSolo"].Index].Value = "";
            //    fnd = true;
            //    break;
            //}
            ////見つからない場合は初期値をセット
            //if (!fnd)
            //{
            //    SoloMode = false;
            //    r.Cells[dgvPartCounter.Columns["ClmMute"].Index].Value = "";
            //    r.Cells[dgvPartCounter.Columns["ClmSolo"].Index].Value = "";// SoloMode ? "S" : "";
            //    r.Cells[dgvPartCounter.Columns["ClmPush"].Index].Value = "";
            //}


            dgvPartCounter.Rows.Add(r);
            SetMute(r);
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

        private void DrawMeter(DataGridViewCell dataGridViewCell, Instrument mmli, int pn)
        {
            int col = mmli.partColor[pn];
            DataGridViewImageCell cell = (DataGridViewImageCell)dataGridViewCell;
            int cw = cell.Size.Width;
            int ch = cell.Size.Height;
            int x = 2;
            int y = (int)((ch - 4) / 6.0) + 2;
            int p = mmli.keyOnMeter[pn] == null ? 0 : (int)mmli.keyOnMeter[pn];
            int w = (int)((cw - 6) / 256.0 * p);
            int h = (int)((ch - 4) / 6.0 * 4.0);
            p = Common.Range(p, 0, meterBrush[col].Length - 1);

            Bitmap canvas = new Bitmap(cw, ch);
            using (Graphics g = Graphics.FromImage(canvas))
            {
                g.Clear(Color.Transparent);
                g.FillRectangle(meterBrush[col][p], x, y, w, h);
            }

            //PictureBox1に表示する
            cell.Value = canvas;

            if (mmli.keyOnMeter[pn] != null)
            {
                mmli.keyOnMeter[pn] -= 4;
                mmli.keyOnMeter[pn] = Math.Max((int)mmli.keyOnMeter[pn], 0);
            }
        }

        protected override string GetPersistString()
        {
            return this.Name;
        }

        private dgvColumnInfo[] getDisplayIndex()
        {
            List<dgvColumnInfo> ret = new List<dgvColumnInfo>();

            for (int i = 0; i < dgvPartCounter.Columns.Count; i++)
            {
                dgvColumnInfo info = (dgvColumnInfo)dgvPartCounter.Columns[i].Tag;
                if (info == null)
                {
                    info = new dgvColumnInfo();
                }

                info.columnName = dgvPartCounter.Columns[i].Name;
                info.displayIndex = dgvPartCounter.Columns[i].DisplayIndex;
                info.size = dgvPartCounter.Columns[i].Width;
                info.visible = dgvPartCounter.Columns[i].Visible;

                ret.Add(info);
            }

            return ret.ToArray();
        }

        private Tuple<string,string,string,bool,DataGridViewContentAlignment>[] colName = new Tuple<string, string,string,bool, DataGridViewContentAlignment>[] {
            new Tuple<string,string,string,bool,DataGridViewContentAlignment>("text","ClmMuteMngKey" ,"mmKey"     ,false,DataGridViewContentAlignment.MiddleCenter),
            new Tuple<string,string,string,bool,DataGridViewContentAlignment>("text","ClmMute"       ,"M"          ,true,DataGridViewContentAlignment.MiddleCenter),
            new Tuple<string,string,string,bool,DataGridViewContentAlignment>("text","ClmSolo"       ,"S"          ,true,DataGridViewContentAlignment.MiddleCenter),
            new Tuple<string,string,string,bool,DataGridViewContentAlignment>("text","ClmPush"       ,"P"         ,false,DataGridViewContentAlignment.MiddleCenter),
            new Tuple<string,string,string,bool,DataGridViewContentAlignment>("text","ClmKBDAssign"  ,"KBD"        ,true,DataGridViewContentAlignment.MiddleCenter),
            new Tuple<string,string,string,bool,DataGridViewContentAlignment>("text","ClmChipIndex"  ,"ChipIndex" ,false,DataGridViewContentAlignment.MiddleCenter),
            new Tuple<string,string,string,bool,DataGridViewContentAlignment>("text","ClmChipNumber" ,"ChipNumber",false,DataGridViewContentAlignment.MiddleCenter),
            new Tuple<string,string,string,bool,DataGridViewContentAlignment>("text","ClmPartNumber" ,"PartNumber",false,DataGridViewContentAlignment.MiddleCenter),
            new Tuple<string,string,string,bool,DataGridViewContentAlignment>("text","ClmPart"       ,"Part"       ,true,DataGridViewContentAlignment.NotSet),
            new Tuple<string,string,string,bool,DataGridViewContentAlignment>("text","ClmMIDICh"     ,"MIDI Ch"    ,true,DataGridViewContentAlignment.NotSet),
            new Tuple<string,string,string,bool,DataGridViewContentAlignment>("text","ClmChip"       ,"Chip"       ,true,DataGridViewContentAlignment.NotSet),
            new Tuple<string,string,string,bool,DataGridViewContentAlignment>("text","ClmCounter"    ,"Counter"    ,true,DataGridViewContentAlignment.NotSet),
            new Tuple<string,string,string,bool,DataGridViewContentAlignment>("text","ClmLoopCounter","LoopCounter",true,DataGridViewContentAlignment.NotSet),
            new Tuple<string,string,string,bool,DataGridViewContentAlignment>("text","ClmInstrument" ,"Instrument" ,true,DataGridViewContentAlignment.NotSet),
            new Tuple<string,string,string,bool,DataGridViewContentAlignment>("text","ClmEnvelope"   ,"Envelope"   ,true,DataGridViewContentAlignment.NotSet),
            new Tuple<string,string,string,bool,DataGridViewContentAlignment>("text","ClmVolume"     ,"Volume"     ,true,DataGridViewContentAlignment.NotSet),
            new Tuple<string,string,string,bool,DataGridViewContentAlignment>("text","ClmExpression" ,"Expression" ,true,DataGridViewContentAlignment.NotSet),
            new Tuple<string,string,string,bool,DataGridViewContentAlignment>("text","ClmVelocity"   ,"Velocity"   ,true,DataGridViewContentAlignment.NotSet),
            new Tuple<string,string,string,bool,DataGridViewContentAlignment>("text","ClmPan"        ,"Pan"        ,true,DataGridViewContentAlignment.NotSet),
            new Tuple<string,string,string,bool,DataGridViewContentAlignment>("text","ClmNote"       ,"Note"       ,true,DataGridViewContentAlignment.NotSet),  
            new Tuple<string,string,string,bool,DataGridViewContentAlignment>("text","ClmGateTime"   ,"GateTime"   ,true,DataGridViewContentAlignment.NotSet),
            new Tuple<string,string,string,bool,DataGridViewContentAlignment>("text","ClmLength"     ,"Length(#)"  ,true,DataGridViewContentAlignment.NotSet),
            new Tuple<string,string,string,bool,DataGridViewContentAlignment>("text","ClmEnvSw"      ,"Env.Sw."    ,true,DataGridViewContentAlignment.NotSet),
            new Tuple<string,string,string,bool,DataGridViewContentAlignment>("text","ClmLfoSw"      ,"LFO Sw."    ,true,DataGridViewContentAlignment.NotSet),
            new Tuple<string,string,string,bool,DataGridViewContentAlignment>("text","ClmLfo"        ,"LFO"        ,true,DataGridViewContentAlignment.NotSet),
            new Tuple<string,string,string,bool,DataGridViewContentAlignment>("text","ClmDetune"     ,"Detune"     ,true,DataGridViewContentAlignment.NotSet),
            new Tuple<string,string,string,bool,DataGridViewContentAlignment>("text","ClmKeyShift"   ,"Key shift"  ,true,DataGridViewContentAlignment.NotSet),
            new Tuple<string,string,string,bool,DataGridViewContentAlignment>("image","ClmMeter"     ,"KeyOn"      ,true,DataGridViewContentAlignment.NotSet),
            new Tuple<string,string,string,bool,DataGridViewContentAlignment>("text","ClmSpacer"     ,""           ,true,DataGridViewContentAlignment.NotSet)
        };                                                                                                                                           

        private void SetDisplayIndex(dgvColumnInfo[] aryIndex)
        {
            if (aryIndex == null || aryIndex.Length < 1)
            {
                setting.location.PartCounterClmInfo = getDisplayIndex();
                aryIndex = setting.location.PartCounterClmInfo;
            }

            for (int i = 0; i < colName.Length; i++)
            {
                if (!dgvPartCounter.Columns.Contains(colName[i].Item2))
                {
                    DataGridViewColumn col;
                    if (colName[i].Item1 == "text")
                        col = new DataGridViewTextBoxColumn();
                    else
                        col = new DataGridViewImageColumn();
                    col.Name = colName[i].Item2;
                    col.HeaderText = colName[i].Item3;
                    col.Visible = colName[i].Item4;
                    col.DefaultCellStyle.Alignment = colName[i].Item5;
                    col.SortMode = DataGridViewColumnSortMode.NotSortable;
                    if (colName[i].Item2 == "ClmSpacer")
                    {
                        col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    }
                    dgvPartCounter.Columns.Add(col);
                }
            }

            for (int i = 0; i < aryIndex.Length; i++)
            {
                if (aryIndex[i] == null) continue;

                dgvPartCounter.Columns[aryIndex[i].columnName].DisplayIndex = aryIndex[i].displayIndex;
                dgvPartCounter.Columns[aryIndex[i].columnName].Width = Math.Max(aryIndex[i].size, 10);
                dgvPartCounter.Columns[aryIndex[i].columnName].Visible = aryIndex[i].visible;
                dgvPartCounter.Columns[aryIndex[i].columnName].Tag = aryIndex[i];
            }

            //spacerは常に最後にする
            dgvPartCounter.Columns["ClmSpacer"].DisplayIndex = dgvPartCounter.Columns.Count - 1;
        }



        public void ClickMUTE(int rowIndex)
        {
            if (rowIndex < 0)
            {
                muteManager.ClickAllMute();
            }
            else
            {
                DataGridViewRow r = dgvPartCounter.Rows[rowIndex];
                muteManager.ClickMute((int)r.Cells[dgvPartCounter.Columns["ClmMuteMngKey"].Index].Value);
            }

            refreshMuteSolo();
        }

        public void ClickSOLO(int rowIndex)
        {

            if (rowIndex < 0)
            {
                muteManager.ClickAllSolo();
            }
            else
            {
                DataGridViewRow r = dgvPartCounter.Rows[rowIndex];
                muteManager.ClickSolo((int)r.Cells[dgvPartCounter.Columns["ClmMuteMngKey"].Index].Value);
            }

            refreshMuteSolo();
        }

        private void refreshMuteSolo()
        {
            int partKey;
            muteStatus ms;

            foreach (DataGridViewRow row in dgvPartCounter.Rows)
            {
                partKey = (int)row.Cells[dgvPartCounter.Columns["ClmMuteMngKey"].Index].Value;
                ms = muteManager.GetStatus(partKey);
                row.Cells[dgvPartCounter.Columns["ClmSolo"].Index].Value = ms.solo ? "S" : "";
                row.Cells[dgvPartCounter.Columns["ClmMute"].Index].Value = ms.mute ? "M" : "";
                SetMute(row);
            }
        }

        public void ClickKBDAssign(int rowIndex)
        {

            //アサインを全解除
            foreach (DataGridViewRow r in dgvPartCounter.Rows)
            {
                r.Cells[clmKBDAssign].Value = "";
                SetAssign(r);
            }

            //列名をクリック時
            if (rowIndex < 0) return;

            if (dgvPartCounter.Rows.Count < rowIndex + 1) return;

            //パートをクリック時はそのパートをアサイン
            DataGridViewRow row = dgvPartCounter.Rows[rowIndex];
            object obj = row.Cells[clmKBDAssign].Value;
            row.Cells[clmKBDAssign].Value = (obj == null || string.IsNullOrEmpty(obj.ToString())) ? Assign : "";
            SetAssign(row);
        }


        public void ClickSOLO_mucom(int rowIndex)
        {

            if (rowIndex < 0)
            {
                if (!SoloMode) return;
                //Click SOLO Reset(mute復帰)) 
                List<char> usePt = new List<char>();
                object obj1 = null;
                object obj2 = null;
                foreach (DataGridViewRow r in dgvPartCounter.Rows)
                {
                    string sp = (string)r.Cells["ClmPart"].Value;
                    if (usePt.Contains(sp[0]))
                    {
                        r.Cells["ClmSolo"].Value = obj1;
                        r.Cells["ClmMute"].Value = obj2;
                        continue;
                    }
                    usePt.Add(sp[0]);

                    obj1 = "";
                    obj2 = r.Cells["ClmPush"].Value;
                    r.Cells["ClmSolo"].Value = obj1;
                    r.Cells["ClmMute"].Value = obj2;
                    SetMute(r);
                }
                SoloMode = false;
                return;
            }

            if (dgvPartCounter.Rows.Count < rowIndex + 1) return;

            bool nowSolo = (string)dgvPartCounter.Rows[rowIndex].Cells["ClmSolo"].Value == "S";
            //SOLOモードではなく、SOLOではない場合はチェックを行う
            if (!SoloMode && !nowSolo && !CheckSoloCh())
            {
                SoloMode = true;
                //mute退避
                List<char> usePt = new List<char>();
                object obj1 = null;
                object obj2 = null;
                foreach (DataGridViewRow r in dgvPartCounter.Rows)
                {
                    string sp = (string)r.Cells["ClmPart"].Value;
                    if (usePt.Contains(sp[0]))
                    {
                        r.Cells["ClmPush"].Value = obj1;
                        r.Cells["ClmMute"].Value = obj2;
                        continue;
                    }
                    usePt.Add(sp[0]);

                    obj1 = r.Cells["ClmMute"].Value;
                    obj2 = "M";
                    r.Cells["ClmPush"].Value = obj1;
                    r.Cells["ClmMute"].Value = obj2;
                    SetMute(r);
                }
            }

            //
            char cp = ((string)dgvPartCounter.Rows[rowIndex].Cells["ClmPart"].Value)[0];
            foreach (DataGridViewRow r in dgvPartCounter.Rows)
            {
                string p = (string)r.Cells["ClmPart"].Value;
                if (p.IndexOf(cp) != 0) continue;

                r.Cells["ClmSolo"].Value = nowSolo ? "" : "S";
                if (SoloMode) r.Cells["ClmMute"].Value = !nowSolo ? "" : "M";
                SetMute(r);
            }

            //dgvPartCounter.Rows[rowIndex].Cells["ClmSolo"].Value = nowSolo ? "" : "S";
            //if (SoloMode) dgvPartCounter.Rows[rowIndex].Cells["ClmMute"].Value = !nowSolo ? "" : "M";
            //SetMute(dgvPartCounter.Rows[rowIndex]);

            if (SoloMode && nowSolo && !CheckSoloCh())
            {
                SoloMode = false;
                //mute復帰
                List<char> usePt = new List<char>();
                object obj=null;
                foreach (DataGridViewRow r in dgvPartCounter.Rows)
                {
                    string sp = (string)r.Cells["ClmPart"].Value;
                    if (usePt.Contains(sp[0]))
                    {
                        r.Cells["ClmMute"].Value = obj;
                        continue;
                    }
                    usePt.Add(sp[0]);

                    obj = r.Cells["ClmPush"].Value;
                    r.Cells["ClmMute"].Value = obj;
                    SetMute(r);
                }
            }
        }

        public void ClickMUTE_mucom(int rowIndex)
        {
            List<char> usePt;

            if (rowIndex < 0)
            {
                if (SoloMode) return;
                //Click MUTE Reset 
                usePt = new List<char>();
                object obj = null;
                foreach (DataGridViewRow r in dgvPartCounter.Rows)
                {
                    string sp = (string)r.Cells["ClmPart"].Value;
                    if (usePt.Contains(sp[0]))
                    {
                        r.Cells["ClmMute"].Value = obj;
                        continue;
                    }
                    usePt.Add(sp[0]);

                    obj = "";
                    r.Cells["ClmMute"].Value = obj;
                    SetMute(r);
                }
                return;
            }

            if (dgvPartCounter.Rows.Count < rowIndex + 1) return;

            bool nowMute = (string)dgvPartCounter.Rows[rowIndex].Cells["ClmMute"].Value == "M";
            char tp = ((string)dgvPartCounter.Rows[rowIndex].Cells["ClmPart"].Value)[0];
            usePt = new List<char>();
            object obj1 = null;
            object obj2 = null;
            foreach (DataGridViewRow r in dgvPartCounter.Rows)
            {
                string sp = (string)r.Cells["ClmPart"].Value;
                if (tp != sp[0]) continue;

                if (usePt.Contains(sp[0]))
                {
                    r.Cells["ClmMute"].Value = obj1;
                    if (SoloMode)
                        r.Cells["ClmSolo"].Value = obj2;
                    continue;
                }
                usePt.Add(sp[0]);

                obj1 = nowMute ? "" : "M";
                r.Cells["ClmMute"].Value = obj1;
                if (SoloMode)
                {
                    obj2 = !nowMute ? "" : "S";
                    r.Cells["ClmSolo"].Value = obj2;
                }
                SetMute(r);
            }

            if (SoloMode && !nowMute && !CheckSoloCh())
            {
                SoloMode = false;
                //mute復帰
                usePt = new List<char>();
                object obj = null;
                foreach (DataGridViewRow r in dgvPartCounter.Rows)
                {
                    string sp = (string)r.Cells["ClmPart"].Value;
                    if (usePt.Contains(sp[0]))
                    {
                        r.Cells["ClmMute"].Value = obj;
                        continue;
                    }
                    usePt.Add(sp[0]);

                    obj = r.Cells["ClmPush"].Value;
                    r.Cells["ClmMute"].Value = obj;
                    SetMute(r);
                }
            }
        }


        private bool CheckSoloCh()
        {
            foreach (DataGridViewRow r in dgvPartCounter.Rows)
            {
                if ((string)r.Cells["ClmSolo"].Value != "S") continue;
                return true;
            }

            return false;
        }


        private void SetMute(DataGridViewRow r)
        {
            string chip = (string)r.Cells["ClmChip"].Value;
            int chipIndex = (int)r.Cells["ClmChipIndex"].Value;
            int chipNumber = (int)r.Cells["ClmChipNumber"].Value;

            if (mmlParams == null) return;
            if (!mmlParams.Insts.ContainsKey(chip)) return;
            if (!mmlParams.Insts[chip].ContainsKey(chipIndex) || !mmlParams.Insts[chip][chipIndex].ContainsKey(chipNumber)) return;

            MMLParameter.Instrument mmli = mmlParams.Insts[chip][chipIndex][chipNumber];
            int pn = (int)r.Cells["ClmPartNumber"].Value - 1;
            bool mute = (string)r.Cells["ClmMute"].Value == "M";

            if (!(mmli is YM2608_mucom) && !(mmli is YM2610B_mucom) && !(mmli is YM2151_mucom))
            {
                mmli.SetMute(pn, mute);
                return;
            }


            string cp = (string)r.Cells["ClmPart"].Value;
            int isOPNA = "ABCDEFGHIJKLMNOPQRSTUV".IndexOf(cp[0]) < 0 ? ("abcdefghijklmnopqrstuv".IndexOf(cp[0]) < 0 ? 2 : 1) : 0;
            int ch;
            int cn = 0;
            if (isOPNA < 2)
            {
                ch = (cp[0] < 'a') ? (cp[0] - 'A') : (cp[0] - 'a');
                cn = ch < 11 ? 0 : 1;
                ch = ch < 11 ? ch : (ch - 11);
            }
            else
            {
                ch = (cp[0] <= 'w' ? (cp[0] - 'W') : (cp[0] - 'w' + 4));
            }
            int pg;// = cp.Length < 2 ? 0 : (cp[1] - '0');
            string mm = (string)r.Cells["ClmMute"].Value;
            string sm = (string)r.Cells["ClmSolo"].Value;
            //int partKey = (int)r.Cells["ClmMuteMngKey"].Value;
            //muteStatus ms = muteManager.GetStatus(partKey);

            foreach (DataGridViewRow rw in dgvPartCounter.Rows)
            {
                string p = (string)rw.Cells["ClmPart"].Value;
                int isOPNAc = "ABCDEFGHIJKLMNOPQRSTUV".IndexOf(p[0]) < 0 ? ("abcdefghijklmnopqrstuv".IndexOf(p[0]) < 0 ? 2 : 1) : 0;
                if (isOPNA != isOPNAc) continue;
                int c;
                int n = 0;
                if (isOPNAc < 2)
                {
                    c = (p[0] < 'a') ? (p[0] - 'A') : (p[0] - 'a');
                    n = c < 11 ? 0 : 1;
                    if (n != cn) continue;
                    c = c < 11 ? c : (c - 11);
                }
                else
                {
                    if (n != cn) continue;
                    c = (p[0] <= 'w' ? (p[0] - 'W') : (p[0] - 'w' + 4));
                }
                if (c != ch) continue;

                //partKey = (int)rw.Cells["ClmMuteMngKey"].Value;
                //muteManager.SetMuteSolo(partKey, ms);

                rw.Cells["ClmMute"].Value = mm;
                rw.Cells["ClmSolo"].Value = sm;
                pn = (int)rw.Cells["ClmPartNumber"].Value - 1;
                pg = p.Length < 2 ? 0 : (p[1] - '0');

                if (isOPNA==0)
                {
                    ((YM2608_mucom)mmli).SetMute(pn, ch, pg, mute);
                }
                else if (isOPNA == 1)
                {
                    ((YM2610B_mucom)mmli).SetMute(pn, ch, pg, mute);
                }
                else
                {
                    ((YM2151_mucom)mmli).SetMute(pn, ch, pg, mute);
                }
            }

        }

        private bool CheckMucomParameter()
        {
            if (dgvPartCounter.Rows.Count < 1) return false;

            DataGridViewRow r = dgvPartCounter.Rows[0];
            string chip = (string)r.Cells["ClmChip"].Value;
            int chipIndex = (int)r.Cells["ClmChipIndex"].Value;
            int chipNumber = (int)r.Cells["ClmChipNumber"].Value;
            if (mmlParams == null) return false;
            if (!mmlParams.Insts.ContainsKey(chip)) return false;
            if (!mmlParams.Insts[chip].ContainsKey(chipIndex) 
                || !mmlParams.Insts[chip][chipIndex].ContainsKey(chipNumber)) return false;
            MMLParameter.Instrument mmli = mmlParams.Insts[chip][chipIndex][chipNumber];

            if (mmli is YM2608_mucom) return true;
            if (mmli is YM2610B_mucom) return true;
            if (mmli is YM2151_mucom) return true;

            return false;
        }

        private void SetAssign(DataGridViewRow r)
        {
            string chip = (string)r.Cells["ClmChip"].Value;
            int chipIndex = (int)r.Cells["ClmChipIndex"].Value;
            int chipNumber = (int)r.Cells["ClmChipNumber"].Value;

            if (mmlParams == null) return;
            if (!mmlParams.Insts.ContainsKey(chip)) return;
            if (!mmlParams.Insts[chip].ContainsKey(chipIndex) || !mmlParams.Insts[chip][chipIndex].ContainsKey(chipNumber)) return;

            MMLParameter.Instrument mmli = mmlParams.Insts[chip][chipIndex][chipNumber];
            int pn = (int)r.Cells["ClmPartNumber"].Value - 1;
            bool assign = (string)r.Cells[clmKBDAssign].Value == Assign;

            //mucom以外は加工無し
            if (!(mmli is YM2608_mucom) && !(mmli is YM2610B_mucom) && !(mmli is YM2151_mucom))
            {
                mmli.SetAssign(pn, assign);
                return;
            }

            //mucomの場合は同系パートも同時にAssignする

            string cp = (string)r.Cells["ClmPart"].Value;
            int isOPNA = "ABCDEFGHIJKLMNOPQRSTUV".IndexOf(cp[0]) < 0 ? ("abcdefghijklmnopqrstuv".IndexOf(cp[0]) < 0 ? 2 : 1) : 0;
            int ch;
            int cn = 0;
            if (isOPNA < 2)
            {
                ch = (cp[0] < 'a') ? (cp[0] - 'A') : (cp[0] - 'a');
                cn = ch < 11 ? 0 : 1;
                ch = ch < 11 ? ch : (ch - 11);
            }
            else
            {
                ch = (cp[0] <= 'W' ? (cp[0] - 'W') : (cp[0] - 'w' + 4));
            }
            int pg;// = cp.Length < 2 ? 0 : (cp[1] - '0');
            string am = (string)r.Cells[clmKBDAssign].Value;

            foreach (DataGridViewRow rw in dgvPartCounter.Rows)
            {
                string p = (string)rw.Cells["ClmPart"].Value;
                int isOPNAc = "ABCDEFGHIJKLMNOPQRSTUV".IndexOf(p[0]) < 0 ? ("abcdefghijklmnopqrstuv".IndexOf(p[0]) < 0 ? 2 : 1) : 0;
                if (isOPNA != isOPNAc) continue;
                int c;
                int n = 0;
                if (isOPNAc < 2)
                {
                    c = (p[0] < 'a') ? (p[0] - 'A') : (p[0] - 'a');
                    n = c < 11 ? 0 : 1;
                    if (n != cn) continue;
                    c = c < 11 ? c : (c - 11);
                }
                else
                {
                    if (n != cn) continue;
                    c = (p[0] <= 'W' ? (p[0] - 'W') : (p[0] - 'w' + 4));
                }
                if (c != ch) continue;

                rw.Cells[clmKBDAssign].Value = am;
                pn = (int)rw.Cells["ClmPartNumber"].Value - 1;
                pg = p.Length < 2 ? 0 : (p[1] - '0');

                if (isOPNA == 0)
                {
                    ((YM2608_mucom)mmli).SetAssign(pn, ch, pg, assign);
                }
                else if (isOPNA == 1)
                {
                    ((YM2610B_mucom)mmli).SetAssign(pn, ch, pg, assign);
                }
                else
                {
                    ((YM2151_mucom)mmli).SetAssign(pn, ch, pg, assign);
                }
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