namespace mml2vgmIDE
{
    partial class FrmLyrics
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.rtbLyrics = new System.Windows.Forms.RichTextBox();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.lblDummy = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // rtbLyrics
            // 
            this.rtbLyrics.BackColor = System.Drawing.Color.Black;
            this.rtbLyrics.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbLyrics.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rtbLyrics.DetectUrls = false;
            this.rtbLyrics.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbLyrics.Font = new System.Drawing.Font("メイリオ", 80.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.rtbLyrics.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.rtbLyrics.Location = new System.Drawing.Point(0, 0);
            this.rtbLyrics.Multiline = false;
            this.rtbLyrics.Name = "rtbLyrics";
            this.rtbLyrics.ReadOnly = true;
            this.rtbLyrics.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.rtbLyrics.ShortcutsEnabled = false;
            this.rtbLyrics.Size = new System.Drawing.Size(874, 159);
            this.rtbLyrics.TabIndex = 0;
            this.rtbLyrics.TabStop = false;
            this.rtbLyrics.Text = "Lyrics";
            // 
            // timer
            // 
            this.timer.Interval = 10;
            this.timer.Tick += new System.EventHandler(this.Timer_Tick);
            // 
            // lblDummy
            // 
            this.lblDummy.AutoSize = true;
            this.lblDummy.Location = new System.Drawing.Point(5, 65);
            this.lblDummy.Name = "lblDummy";
            this.lblDummy.Size = new System.Drawing.Size(26, 12);
            this.lblDummy.TabIndex = 1;
            this.lblDummy.Text = "dmy";
            // 
            // FrmLyrics
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(874, 159);
            this.Controls.Add(this.rtbLyrics);
            this.Controls.Add(this.lblDummy);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
            this.Name = "FrmLyrics";
            this.Text = "Lyrics";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmLyrics_FormClosing);
            this.SizeChanged += new System.EventHandler(this.FrmLyrics_SizeChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbLyrics;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Label lblDummy;
    }
}