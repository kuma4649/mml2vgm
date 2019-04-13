using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using System.IO;
using Core;
using System.Diagnostics;

namespace mml2vgmIDE
{
    public partial class FrmMain : Form
    {

        private List<Form> FormBox = new List<Form>();
        private List<Document> DocumentBox = new List<Document>();
        private bool isSuccess = true;
        private string[] args;
        private Mml2vgm mv = null;
        private string title = "";
        private FrmLog frmConsole = null;
        private FrmPartCounter frmPartCounter = null;
        private FrmFolderTree frmFolderTree = null;
        private FrmErrorList frmErrorList = null;
        private bool doPlay = false;

        public FrmMain()
        {
            InitializeComponent();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            this.IsMdiContainer = true;

            frmPartCounter = new FrmPartCounter();
            frmPartCounter.Show(dpMain, WeifenLuo.WinFormsUI.Docking.DockState.DockLeft);
            FormBox.Add(frmPartCounter);

            frmConsole = new FrmLog();
            frmConsole.Show(dpMain, WeifenLuo.WinFormsUI.Docking.DockState.DockBottom);
            FormBox.Add(frmConsole);

            frmFolderTree = new FrmFolderTree();
            frmFolderTree.Show(dpMain, WeifenLuo.WinFormsUI.Docking.DockState.DockLeft);
            FormBox.Add(frmFolderTree);

            frmErrorList = new FrmErrorList();
            frmErrorList.Show(dpMain, WeifenLuo.WinFormsUI.Docking.DockState.DockBottom);
            FormBox.Add(frmErrorList);
        }

        private void AzukiControl1_Click(object sender, EventArgs e)
        {

        }

        private void DockPanel1_ActiveContentChanged(object sender, EventArgs e)
        {

        }

        private void TsmiExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {

            //DialogResult res= MessageBox.Show("終了しますか？"
            //    , "修了確認"
            //    , MessageBoxButtons.YesNoCancel
            //    , MessageBoxIcon.Question);
            //if(res!= DialogResult.Yes)
            //{
            //    e.Cancel = true;
            //}
        }

        private void TsmiNew_Click(object sender, EventArgs e)
        {
            OpenFile("");
        }

        private void TsmiFileOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = "gwiLファイル(*.gwi)|*.gwi|すべてのファイル(*.*)|*.*";
            ofd.Title = "ファイルを開く";
            ofd.RestoreDirectory = true;

            if (ofd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            OpenFile(ofd.FileName);
        }

        public void TsmiCompileAndPlay_Click(object sender, EventArgs e)
        {
            Compile(true);
        }

        private void TsmiCompile_Click(object sender, EventArgs e)
        {
            Compile(false);
        }



        private void OpenFile(string fileName)
        {
            Document dc = new Document();
            if (fileName != "") dc.InitOpen(fileName);
            dc.editor.Show(dpMain, DockState.Document);
            dc.editor.main = this;

            frmFolderTree.treeView1.Nodes.Clear();
            frmFolderTree.treeView1.Nodes.Add(dc.gwiTree);

            FormBox.Add(dc.editor);
            DocumentBox.Add(dc);
        }

        private void Compile(bool doPlay)
        {
            IDockContent dc = dpMain.ActiveDocument;
            if (dc == null) return;
            if (!(dc is FrmEditor)) return;

            string text = ((FrmEditor)dc).azukiControl1.Text;
            string tempPath = Path.Combine(Common.GetApplicationDataFolder(true), "temp.gwi");

            File.WriteAllText(tempPath, text);
            args = new string[2];
            args[1] = tempPath;

            isSuccess = true;
            this.doPlay = doPlay;
            frmPartCounter.dataGridView1.Rows.Clear();

            Thread trdStartCompile = new Thread(new ThreadStart(startCompile));
            trdStartCompile.Start();
        }

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
                //if (tsbToVGZ.Checked)
                //{
                    //desfn = Path.ChangeExtension(arg, Properties.Resources.ExtensionVGZ);
                //}

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

        private void updateTitle()
        {
            if (title == "")
            {
                this.Text = "mml2vgmIDE";
            }
            else
            {
                this.Text = string.Format("mml2vgmIDE - {0}", title);
            }
        }
        private void Disp(string msg)
        {
            Action<string> msgDisp = MsgDisp;
            this.Invoke(msgDisp, msg);
            Core.log.Write(msg);
        }

        private void MsgDisp(string msg)
        {
            if (frmConsole == null) return;
            if (frmConsole.IsDisposed) return;

            frmConsole.textBox1.AppendText(msg + "\r\n");
        }

        private void finishedCompile()
        {
            if (mv == null)
            {
                if (frmConsole != null && !frmConsole.IsDisposed) frmConsole.textBox1.AppendText(msg.get("I0105"));
                //this.toolStrip1.Enabled = true;
                //this.tsslMessage.Text = msg.get("I0106");
                return;
            }

            if (isSuccess)
            {
                foreach (KeyValuePair<enmChipType, ClsChip[]> kvp in mv.desVGM.chips)
                {
                    foreach (ClsChip chip in kvp.Value)
                    {
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
                            frmPartCounter.dataGridView1.Rows.Add(row);
                        }
                    }
                }
            }

            frmConsole.textBox1.AppendText(msg.get("I0107"));

            foreach (string mes in msgBox.getErr())
            {
                frmErrorList.dataGridView1.Rows.Add("Error", "-", "-", mes);
                //frmConsole.textBox1.AppendText(string.Format(msg.get("I0109"), mes));
            }

            foreach (string mes in msgBox.getWrn())
            {
                frmErrorList.dataGridView1.Rows.Add("Warning", "-", "-", mes);
                //frmConsole.textBox1.AppendText(string.Format(msg.get("I0108"), mes));
            }

            frmConsole.textBox1.AppendText("\r\n");
            frmConsole.textBox1.AppendText(string.Format(msg.get("I0110"), msgBox.getErr().Length, msgBox.getWrn().Length));

            if (mv.desVGM.loopSamples != -1)
            {
                frmConsole.textBox1.AppendText(string.Format(msg.get("I0111"), mv.desVGM.loopClock));
                if (mv.desVGM.info.format == enmFormat.VGM)
                    frmConsole.textBox1.AppendText(string.Format(msg.get("I0112")
                        , mv.desVGM.loopSamples
                        , mv.desVGM.loopSamples / 44100L));
                else
                    frmConsole.textBox1.AppendText(string.Format(msg.get("I0112")
                        , mv.desVGM.loopSamples
                        , mv.desVGM.loopSamples / (mv.desVGM.info.xgmSamplesPerSecond)));
            }

            frmConsole.textBox1.AppendText(string.Format(msg.get("I0113"), mv.desVGM.lClock));
            if (mv.desVGM.info.format == enmFormat.VGM)
                frmConsole.textBox1.AppendText(string.Format(msg.get("I0114")
                    , mv.desVGM.dSample
                    , mv.desVGM.dSample / 44100L));
            else
                frmConsole.textBox1.AppendText(string.Format(msg.get("I0114")
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


            frmConsole.textBox1.AppendText(msg.get("I0126"));
            //this.toolStrip1.Enabled = true;
            //this.tsslMessage.Text = msg.get("I0106");

            if (isSuccess)
            {
                if (args.Length == 2 && doPlay)
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
    }
}
