namespace mml2vgmIDE
{
    partial class FrmMIDIKbd
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMIDIKbd));
            this.pbScreen = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbScreen)).BeginInit();
            this.SuspendLayout();
            // 
            // pbScreen
            // 
            this.pbScreen.Image = global::mml2vgmIDE.Properties.Resources.planeMIDIKB;
            this.pbScreen.Location = new System.Drawing.Point(0, 0);
            this.pbScreen.Name = "pbScreen";
            this.pbScreen.Size = new System.Drawing.Size(177, 112);
            this.pbScreen.TabIndex = 0;
            this.pbScreen.TabStop = false;
            // 
            // FrmMIDIKbd
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(177, 112);
            this.Controls.Add(this.pbScreen);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "FrmMIDIKbd";
            this.Text = "Keyboard";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmMIDIKbd_FormClosed);
            this.Load += new System.EventHandler(this.FrmMIDIKbd_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmMIDIKbd_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FrmMIDIKbd_KeyUp);
            this.Resize += new System.EventHandler(this.FrmMIDIKbd_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.pbScreen)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.PictureBox pbScreen;
    }
}