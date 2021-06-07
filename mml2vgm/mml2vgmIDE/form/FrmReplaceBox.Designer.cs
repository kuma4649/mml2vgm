namespace mml2vgmIDE.form
{
    partial class FrmReplaceBox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmReplaceBox));
            this.label1 = new System.Windows.Forms.Label();
            this.btnPrevious = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.cmbFrom = new System.Windows.Forms.ComboBox();
            this.cmbTo = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnReplace = new System.Windows.Forms.Button();
            this.btnAllReplace = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbTargetParts = new System.Windows.Forms.RadioButton();
            this.rbTargetMMLs = new System.Windows.Forms.RadioButton();
            this.rbTargetAll = new System.Windows.Forms.RadioButton();
            this.groupBox2.SuspendLayout();
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
            // cmbFrom
            // 
            resources.ApplyResources(this.cmbFrom, "cmbFrom");
            this.cmbFrom.FormattingEnabled = true;
            this.cmbFrom.Name = "cmbFrom";
            this.cmbFrom.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbFrom_KeyDown);
            // 
            // cmbTo
            // 
            resources.ApplyResources(this.cmbTo, "cmbTo");
            this.cmbTo.FormattingEnabled = true;
            this.cmbTo.Name = "cmbTo";
            this.cmbTo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbTo_KeyDown);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // btnReplace
            // 
            resources.ApplyResources(this.btnReplace, "btnReplace");
            this.btnReplace.Name = "btnReplace";
            this.btnReplace.UseVisualStyleBackColor = true;
            this.btnReplace.Click += new System.EventHandler(this.btnReplace_Click);
            // 
            // btnAllReplace
            // 
            resources.ApplyResources(this.btnAllReplace, "btnAllReplace");
            this.btnAllReplace.Name = "btnAllReplace";
            this.btnAllReplace.UseVisualStyleBackColor = true;
            this.btnAllReplace.Click += new System.EventHandler(this.btnAllReplace_Click);
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.rbTargetParts);
            this.groupBox2.Controls.Add(this.rbTargetMMLs);
            this.groupBox2.Controls.Add(this.rbTargetAll);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // rbTargetParts
            // 
            resources.ApplyResources(this.rbTargetParts, "rbTargetParts");
            this.rbTargetParts.Name = "rbTargetParts";
            this.rbTargetParts.UseVisualStyleBackColor = true;
            // 
            // rbTargetMMLs
            // 
            resources.ApplyResources(this.rbTargetMMLs, "rbTargetMMLs");
            this.rbTargetMMLs.Name = "rbTargetMMLs";
            this.rbTargetMMLs.UseVisualStyleBackColor = true;
            // 
            // rbTargetAll
            // 
            resources.ApplyResources(this.rbTargetAll, "rbTargetAll");
            this.rbTargetAll.Checked = true;
            this.rbTargetAll.Name = "rbTargetAll";
            this.rbTargetAll.TabStop = true;
            this.rbTargetAll.UseVisualStyleBackColor = true;
            // 
            // FrmReplaceBox
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.cmbTo);
            this.Controls.Add(this.cmbFrom);
            this.Controls.Add(this.btnAllReplace);
            this.Controls.Add(this.btnReplace);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.btnPrevious);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.KeyPreview = true;
            this.Name = "FrmReplaceBox";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmReplaceBox_FormClosed);
            this.Shown += new System.EventHandler(this.FrmReplaceBox_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmReplaceBox_KeyDown);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnPrevious;
        private System.Windows.Forms.Button btnNext;
        public System.Windows.Forms.ComboBox cmbFrom;
        public System.Windows.Forms.ComboBox cmbTo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnReplace;
        private System.Windows.Forms.Button btnAllReplace;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rbTargetAll;
        private System.Windows.Forms.RadioButton rbTargetParts;
        private System.Windows.Forms.RadioButton rbTargetMMLs;
    }
}