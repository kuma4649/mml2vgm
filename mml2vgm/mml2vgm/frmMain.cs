using Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace mml2vgm
{
    public partial class frmMain : Form
    {
        private string[] args;
        private Mml2vgm mv = null;
        private string title = "";
        private FileSystemWatcher watcher = null;
        private long now = 0;


        public frmMain()
        {
            InitializeComponent();
#if DEBUG
            Core.log.debug = true;
#endif
        }

        private void frmMain_Shown(object sender, EventArgs e)
        {
            string[] args = Environment.GetCommandLineArgs();
            if (args != null && args.Length > 1)
            {
                this.args = args;
                tsbCompile_Click(null, null);
            }
        }

        private void tsbCompile_Click(object sender, EventArgs e)
        {
            if (args == null || args.Length < 2)
            {
                return;
            }

            this.toolStrip1.Enabled = false;
            this.tsslMessage.Text = msg.get("I0100");
            dgvResult.Rows.Clear();

            textBox1.AppendText(msg.get("I0101"));
            textBox1.AppendText(msg.get("I0102"));

            isSuccess = true;
            Thread trdStartCompile = new Thread(new ThreadStart(startCompile));
            trdStartCompile.Start();

        }

        private void tsbOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Filter = msg.get("I0103");
            ofd.Title = msg.get("I0104");
            if (ofd.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            args = null;
            List<string> a = new List<string>();
            a.Add("dummy");
            foreach (string fn in ofd.FileNames)
            {
                a.Add(fn);
            }
            args = a.ToArray();
            if (tsbWatcher.Checked)
            {
                stopWatch();
            }

            tsbCompile_Click(null, null);

            if (tsbWatcher.Checked)
            {
                startWatch();
            }
        }

        private void updateTitle()
        {
            if (title == "")
            {
                this.Text = "mml2vgm";
            }
            else
            {
                this.Text = string.Format("mml2vgm - {0}", title);
            }
        }

        private void finishedCompile()
        {
            if (mv == null)
            {
                textBox1.AppendText(msg.get("I0105"));
                this.toolStrip1.Enabled = true;
                this.tsslMessage.Text = msg.get("I0106");
                return;
            }

            if (isSuccess)
            {
                foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in mv.desVGM.chips)
                {
                    foreach (ClsChip chip in kvp.Value)
                    {
                        if (chip == null) continue;
                        List<partWork> pw = chip.lstPartWork;
                        for (int i = 0; i < pw.Count; i++)
                        {
                            if (pw[i].clockCounter == 0) continue;

                            DataGridViewRow row = new DataGridViewRow();
                            row.Cells.Add(new DataGridViewTextBoxCell());
                            row.Cells[0].Value = pw[i].PartName.Substring(0, 2).Replace(" ", "") + int.Parse(pw[i].PartName.Substring(2, 2)).ToString();
                            row.Cells.Add(new DataGridViewTextBoxCell());
                            row.Cells[1].Value = pw[i].chip.Name.ToUpper();
                            row.Cells.Add(new DataGridViewTextBoxCell());
                            row.Cells[2].Value = pw[i].clockCounter;
                            dgvResult.Rows.Add(row);
                        }
                    }
                }
            }

            textBox1.AppendText(msg.get("I0107"));

            foreach (msgInfo mes in msgBox.getWrn())
            {
                textBox1.AppendText(string.Format(msg.get("I0108"), mes.filename, mes.line == -1 ? "-" : (mes.line + 1).ToString(), mes.body));
            }

            foreach (msgInfo mes in msgBox.getErr())
            {
                textBox1.AppendText(string.Format(msg.get("I0109"), mes.filename, mes.line == -1 ? "-" : (mes.line + 1).ToString(), mes.body));
            }

            textBox1.AppendText("\r\n");
            textBox1.AppendText(string.Format(msg.get("I0110"), msgBox.getErr().Length, msgBox.getWrn().Length));

            if (mv.desVGM.loopSamples != -1)
            {
                textBox1.AppendText(string.Format(msg.get("I0111"), mv.desVGM.loopClock));
                if (mv.desVGM.info.format == enmFormat.VGM)
                    textBox1.AppendText(string.Format(msg.get("I0112")
                        , mv.desVGM.loopSamples
                        , mv.desVGM.loopSamples / 44100L));
                else
                    textBox1.AppendText(string.Format(msg.get("I0112")
                        , mv.desVGM.loopSamples
                        , mv.desVGM.loopSamples / (mv.desVGM.info.xgmSamplesPerSecond)));
            }

            textBox1.AppendText(string.Format(msg.get("I0113"), mv.desVGM.lClock));
            if (mv.desVGM.info.format == enmFormat.VGM)
                textBox1.AppendText(string.Format(msg.get("I0114")
                    , mv.desVGM.dSample
                    , mv.desVGM.dSample / 44100L));
            else
                textBox1.AppendText(string.Format(msg.get("I0114")
                    , mv.desVGM.dSample
                    , mv.desVGM.dSample / (mv.desVGM.info.xgmSamplesPerSecond)));

            //if (mv.desVGM.ym2608[0].pcmDataEasy != null) textBox1.AppendText(string.Format(msg.get("I0115"), mv.desVGM.ym2608[0].pcmDataEasy.Length - 15));
            //if (mv.desVGM.ym2608[1].pcmDataEasy != null) textBox1.AppendText(string.Format(msg.get("I0116"), mv.desVGM.ym2608[1].pcmDataEasy.Length - 15));
            //if (mv.desVGM.ym2610b[0].pcmDataEasyA != null) textBox1.AppendText(string.Format(msg.get("I0117"), mv.desVGM.ym2610b[0].pcmDataEasyA.Length-15));
            //if (mv.desVGM.ym2610b[0].pcmDataEasyB != null) textBox1.AppendText(string.Format(msg.get("I0118"), mv.desVGM.ym2610b[0].pcmDataEasyB.Length-15));
            //if (mv.desVGM.ym2610b[1].pcmDataEasyA != null) textBox1.AppendText(string.Format(msg.get("I0119"), mv.desVGM.ym2610b[1].pcmDataEasyA.Length - 15));
            //if (mv.desVGM.ym2610b[1].pcmDataEasyB != null) textBox1.AppendText(string.Format(msg.get("I0120"), mv.desVGM.ym2610b[1].pcmDataEasyB.Length - 15));
            //if (mv.desVGM.segapcm[0].pcmData != null) textBox1.AppendText(string.Format(" PCM Data size(SEGAPCM)  : {0} byte\r\n", mv.desVGM.segapcm[0].pcmData.Length - 15));
            //if (mv.desVGM.segapcm[1].pcmData != null) textBox1.AppendText(string.Format(" PCM Data size(SEGAPCMSecondary)  : {0} byte\r\n", mv.desVGM.segapcm[1].pcmData.Length - 15));
            //if (mv.desVGM.ym2612[0].pcmDataEasy != null) textBox1.AppendText(string.Format(msg.get("I0121"), mv.desVGM.ym2612[0].pcmDataEasy.Length));
            //if (mv.desVGM.rf5c164[0].pcmDataEasy != null) textBox1.AppendText(string.Format(msg.get("I0122"), mv.desVGM.rf5c164[0].pcmDataEasy.Length-12));
            //if (mv.desVGM.rf5c164[1].pcmDataEasy != null) textBox1.AppendText(string.Format(msg.get("I0123"), mv.desVGM.rf5c164[1].pcmDataEasy.Length-12));
            //if (mv.desVGM.huc6280[0].pcmDataEasy != null) textBox1.AppendText(string.Format(msg.get("I0124"), mv.desVGM.huc6280[0].pcmDataEasy.Length));
            //if (mv.desVGM.huc6280[1].pcmDataEasy != null) textBox1.AppendText(string.Format(msg.get("I0125"), mv.desVGM.huc6280[1].pcmDataEasy.Length));


            textBox1.AppendText(msg.get("I0126"));
            this.toolStrip1.Enabled = true;
            this.tsslMessage.Text = msg.get("I0106");

            if (isSuccess)
            {
                if (args.Length == 2 && tsbOnPlay.Checked)
                {
                    try
                    {
                        Process.Start(Path.ChangeExtension(args[1], (mv.desVGM.info.format == enmFormat.VGM) ? Properties.Resources.ExtensionVGM : Properties.Resources.ExtensionXGM));
                    }
                    catch (Exception)
                    {
                        MessageBox.Show(msg.get("E0100"), "mml2vgm", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        bool isSuccess = true;

        private void startCompile()
        {
            Core.log.Open();
            Core.log.Write("start compile thread");

            Action dmy = updateTitle;
            string stPath = System.Windows.Forms.Application.StartupPath;

            for (int i = 1; i < args.Length; i++)
            {
                string arg = args[i];
                if (!File.Exists(arg))
                {
                    continue;
                }


                title = Path.GetFileName(arg);
                this.Invoke(dmy);

                Core.log.Write(string.Format("  compile at [{0}]", args[i]));

                msgBox.clear();

                string desfn = Path.ChangeExtension(arg, Properties.Resources.ExtensionVGM);
                if (tsbToVGZ.Checked)
                {
                    desfn = Path.ChangeExtension(arg, Properties.Resources.ExtensionVGZ);
                }

                Core.log.Write("Call mml2vgm core");

                mv = new Mml2vgm(arg, desfn, stPath, Disp);
                if (mv.Start() != 0)
                {
                    isSuccess = false;
                    break;
                }

                Core.log.Write("Return mml2vgm core");
            }

            Core.log.Write("Disp Result");

            dmy = finishedCompile;
            this.Invoke(dmy);

            Core.log.Write("end compile thread");
            Core.log.Close();
        }

        private void Disp(string msg)
        {
            Action<string> msgDisp = MsgDisp;
            this.Invoke(msgDisp, msg);
            Core.log.Write(msg);
        }

        private void MsgDisp(string msg)
        {
            textBox1.AppendText(msg + "\r\n");
        }

        private void startWatch()
        {
            if (watcher != null) return;

            watcher = new System.IO.FileSystemWatcher();
            watcher.Path = Path.GetDirectoryName(args[1]);
            watcher.NotifyFilter =
                (
                System.IO.NotifyFilters.LastAccess
                | System.IO.NotifyFilters.LastWrite
                | System.IO.NotifyFilters.FileName
                | System.IO.NotifyFilters.DirectoryName
                );
            watcher.Filter = Path.GetFileName(args[1]);
            watcher.SynchronizingObject = this;

            watcher.Changed += new System.IO.FileSystemEventHandler(watcher_Changed);
            watcher.Created += new System.IO.FileSystemEventHandler(watcher_Changed);

            watcher.EnableRaisingEvents = true;
        }

        private void stopWatch()
        {
            watcher.EnableRaisingEvents = false;
            watcher.Dispose();
            watcher = null;
        }

        private void watcher_Changed(System.Object source, System.IO.FileSystemEventArgs e)
        {
            switch (e.ChangeType)
            {
                case System.IO.WatcherChangeTypes.Changed:
                case System.IO.WatcherChangeTypes.Created:

                    long n = DateTime.Now.Ticks / 10000000L;
                    if (now == n) return;
                    now = n;
                    tsbCompile_Click(null, null);
                    break;
            }
        }

        private void tsbWatcher_CheckedChanged(object sender, EventArgs e)
        {
            if (args == null || args.Length < 2)
            {
                tsbWatcher.Checked = false;
                return;
            }

            if (tsbWatcher.Checked)
            {
                startWatch();
                return;
            }

            stopWatch();
        }

        private void FrmMain_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            string[] fileNames = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            foreach (string fn in fileNames)
            {
                string ext = Path.GetExtension(fn).ToLower();
                if (ext == ".gwi") continue;
                e.Effect = DragDropEffects.None;

                return;
            }

            e.Effect = DragDropEffects.Copy;
        }

        private void FrmMain_DragDrop(object sender, DragEventArgs e)
        {
            string[] fileNames = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            foreach (string fn in fileNames)
            {
                string ext = Path.GetExtension(fn).ToLower();
                if (ext == ".gwi") continue;

                if (fileNames.Length < 1)
                    MessageBox.Show(msg.get("E0101"));
                else
                    MessageBox.Show(msg.get("E0102"));

                return;
            }

            List<string> fs = new List<string>();
            fs.Add("");//dummy;
            fs.AddRange(fileNames);
            args = fs.ToArray();

            tsbCompile_Click(null, null);
        }

        private void FrmMain_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F1:
                    tsbOpen_Click(null, null);
                    break;
                //case Keys.F2:
                //break;
                //case Keys.S:
                //break;
                case Keys.F5:
                    tsbCompile_Click(null, null);
                    break;
                default:
                    //↓KeyData確認用
                    //log.Write(string.Format("動作未定義のキー：{0}",e.KeyData));
                    break;
            }
        }

    }

}
