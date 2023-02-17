namespace mml2vgmIDE.form
{
    partial class FrmSearchBox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmSearchBox));
            this.label1 = new System.Windows.Forms.Label();
            this.btnPrevious = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.cmbPattern = new System.Windows.Forms.ComboBox();
            this.cbCaseSenstivity = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // btnPrevious
            // 
            resources.ApplyResources(this.btnPrevious, "btnPrevious");
            this.btnPrevious.Name = "btnPrevious";
            this.btnPrevious.UseVisualStyleBackColor = true;
            this.btnPrevious.Click += new System.EventHandler(this.BtnPrevious_Click);
            // 
            // btnNext
            // 
            resources.ApplyResources(this.btnNext, "btnNext");
            this.btnNext.Name = "btnNext";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.BtnNext_Click);
            // 
            // cmbPattern
            // 
            resources.ApplyResources(this.cmbPattern, "cmbPattern");
            this.cmbPattern.FormattingEnabled = true;
            this.cmbPattern.Name = "cmbPattern";
            this.cmbPattern.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbPattern_KeyDown);
            // 
            // cbCaseSenstivity
            // 
            resources.ApplyResources(this.cbCaseSenstivity, "cbCaseSenstivity");
            this.cbCaseSenstivity.Name = "cbCaseSenstivity";
            this.cbCaseSenstivity.UseVisualStyleBackColor = true;
            // 
            // FrmSearchBox
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbCaseSenstivity);
            this.Controls.Add(this.cmbPattern);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.btnPrevious);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "FrmSearchBox";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmSearchBox_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnPrevious;
        private System.Windows.Forms.Button btnNext;
        public System.Windows.Forms.ComboBox cmbPattern;
        public System.Windows.Forms.CheckBox cbCaseSenstivity;
    }
}