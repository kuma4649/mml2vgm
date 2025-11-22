namespace mml2vgmIDEx64.form
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
            label1 = new Label();
            btnPrevious = new Button();
            btnNext = new Button();
            cmbFrom = new ComboBox();
            cmbTo = new ComboBox();
            label2 = new Label();
            btnReplace = new Button();
            btnAllReplace = new Button();
            groupBox2 = new GroupBox();
            rbTargetParts = new RadioButton();
            rbTargetMMLs = new RadioButton();
            rbTargetAll = new RadioButton();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            // 
            // btnPrevious
            // 
            resources.ApplyResources(btnPrevious, "btnPrevious");
            btnPrevious.Name = "btnPrevious";
            btnPrevious.UseVisualStyleBackColor = true;
            btnPrevious.Click += BtnPrevious_Click;
            // 
            // btnNext
            // 
            resources.ApplyResources(btnNext, "btnNext");
            btnNext.Name = "btnNext";
            btnNext.UseVisualStyleBackColor = true;
            btnNext.Click += BtnNext_Click;
            // 
            // cmbFrom
            // 
            resources.ApplyResources(cmbFrom, "cmbFrom");
            cmbFrom.FormattingEnabled = true;
            cmbFrom.Name = "cmbFrom";
            cmbFrom.KeyDown += cmbFrom_KeyDown;
            // 
            // cmbTo
            // 
            resources.ApplyResources(cmbTo, "cmbTo");
            cmbTo.FormattingEnabled = true;
            cmbTo.Name = "cmbTo";
            cmbTo.KeyDown += cmbTo_KeyDown;
            // 
            // label2
            // 
            resources.ApplyResources(label2, "label2");
            label2.Name = "label2";
            // 
            // btnReplace
            // 
            resources.ApplyResources(btnReplace, "btnReplace");
            btnReplace.Name = "btnReplace";
            btnReplace.UseVisualStyleBackColor = true;
            btnReplace.Click += btnReplace_Click;
            // 
            // btnAllReplace
            // 
            resources.ApplyResources(btnAllReplace, "btnAllReplace");
            btnAllReplace.Name = "btnAllReplace";
            btnAllReplace.UseVisualStyleBackColor = true;
            btnAllReplace.Click += btnAllReplace_Click;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(rbTargetParts);
            groupBox2.Controls.Add(rbTargetMMLs);
            groupBox2.Controls.Add(rbTargetAll);
            resources.ApplyResources(groupBox2, "groupBox2");
            groupBox2.Name = "groupBox2";
            groupBox2.TabStop = false;
            // 
            // rbTargetParts
            // 
            resources.ApplyResources(rbTargetParts, "rbTargetParts");
            rbTargetParts.Name = "rbTargetParts";
            rbTargetParts.UseVisualStyleBackColor = true;
            // 
            // rbTargetMMLs
            // 
            resources.ApplyResources(rbTargetMMLs, "rbTargetMMLs");
            rbTargetMMLs.Name = "rbTargetMMLs";
            rbTargetMMLs.UseVisualStyleBackColor = true;
            // 
            // rbTargetAll
            // 
            resources.ApplyResources(rbTargetAll, "rbTargetAll");
            rbTargetAll.Checked = true;
            rbTargetAll.Name = "rbTargetAll";
            rbTargetAll.TabStop = true;
            rbTargetAll.UseVisualStyleBackColor = true;
            // 
            // FrmReplaceBox
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(groupBox2);
            Controls.Add(cmbTo);
            Controls.Add(cmbFrom);
            Controls.Add(btnAllReplace);
            Controls.Add(btnReplace);
            Controls.Add(btnNext);
            Controls.Add(btnPrevious);
            Controls.Add(label2);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            KeyPreview = true;
            Name = "FrmReplaceBox";
            FormClosed += FrmReplaceBox_FormClosed;
            Shown += FrmReplaceBox_Shown;
            KeyDown += FrmReplaceBox_KeyDown;
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

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