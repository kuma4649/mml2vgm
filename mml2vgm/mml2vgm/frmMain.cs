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
        private mml2vgm mv = null;
        private string title = "";
        private FileSystemWatcher watcher = null;
        private long now = 0;


        public frmMain()
        {
            InitializeComponent();
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
            this.tsslMessage.Text = "Compile";
            dgvResult.Rows.Clear();

            Thread trdStartCompile = new Thread(new ThreadStart(startCompile));
            trdStartCompile.Start();

        }

        private void tsbOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Filter = ".gwiファイル(*.gwi)|*.gwi|すべてのファイル(*.*)|*.*";
            ofd.Title = ".gwiファイルを選択してください";
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
            partWork[] pw = mv.desVGM.pw;
            for (int i = 0; i < pw.Length; i++)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.Cells.Add(new DataGridViewTextBoxCell());
                row.Cells[0].Value = pw[i].partName;
                row.Cells.Add(new DataGridViewTextBoxCell());
                row.Cells[1].Value = pw[i].type.ToString().ToUpper();
                row.Cells.Add(new DataGridViewTextBoxCell());
                row.Cells[2].Value = pw[i].clockCounter;
                dgvResult.Rows.Add(row);
            }

            foreach (string mes in msgBox.getWrn())
            {
                textBox1.AppendText(string.Format("Warning : {0}\r\n", mes));
            }

            foreach (string mes in msgBox.getErr())
            {
                textBox1.AppendText(string.Format("Error : {0}\r\n", mes));
            }

            textBox1.AppendText(string.Format("Errors:{0} Warnings:{1}\r\n", msgBox.getErr().Length, msgBox.getWrn().Length));
            textBox1.AppendText(string.Format("Total Clocks  : {0}\r\n", mv.desVGM.lClock));
            textBox1.AppendText(string.Format("Total Samples : {0}({1}s)\r\n", mv.desVGM.lSample, mv.desVGM.lSample / 44100L));


            textBox1.AppendText("Finished.\r\n\r\n");
            this.toolStrip1.Enabled = true;
            this.tsslMessage.Text = "Done.";

            if (args.Length == 2 && tsbOnPlay.Checked) Process.Start(Path.ChangeExtension(args[1], Properties.Resources.ExtensionVGM));

        }

        private void startCompile()
        {
            Action dmy = updateTitle;

            for (int i = 1; i < args.Length; i++)
            {
                string arg = args[i];
                if (!File.Exists(arg))
                {
                    continue;
                }

                title = Path.GetFileName( arg);
                this.Invoke(dmy);

                msgBox.clear();

                string desfn = Path.ChangeExtension(arg, Properties.Resources.ExtensionVGM);
                if (tsbToVGZ.Checked)
                {
                    desfn = Path.ChangeExtension(arg, Properties.Resources.ExtensionVGZ);
                }
                mv = new mml2vgm(arg, desfn);
                mv.Start();
            }

            dmy = finishedCompile;
            this.Invoke(dmy);

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

    }

}
