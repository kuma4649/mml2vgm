
namespace mml2vgmIDE.form
{
    partial class FrmCSM
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
            this.tbWaveFileName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbSampleRate = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbTempo = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbReso = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbAnalyzeSize = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbTLlevelMul = new System.Windows.Forms.TextBox();
            this.btnRef = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.btnAccept = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tbWaveFileName
            // 
            this.tbWaveFileName.Location = new System.Drawing.Point(99, 21);
            this.tbWaveFileName.Name = "tbWaveFileName";
            this.tbWaveFileName.Size = new System.Drawing.Size(163, 19);
            this.tbWaveFileName.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Wave file";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "Sample rate";
            // 
            // tbSampleRate
            // 
            this.tbSampleRate.Location = new System.Drawing.Point(99, 46);
            this.tbSampleRate.Name = "tbSampleRate";
            this.tbSampleRate.Size = new System.Drawing.Size(62, 19);
            this.tbSampleRate.TabIndex = 4;
            this.tbSampleRate.Text = "44100";
            this.tbSampleRate.Leave += new System.EventHandler(this.tbSampleRate_Leave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "Tempo";
            // 
            // tbTempo
            // 
            this.tbTempo.Location = new System.Drawing.Point(99, 71);
            this.tbTempo.Name = "tbTempo";
            this.tbTempo.Size = new System.Drawing.Size(62, 19);
            this.tbTempo.TabIndex = 7;
            this.tbTempo.Text = "160";
            this.tbTempo.Leave += new System.EventHandler(this.tbTempo_Leave);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 99);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(31, 12);
            this.label4.TabIndex = 8;
            this.label4.Text = "Reso";
            // 
            // tbReso
            // 
            this.tbReso.Location = new System.Drawing.Point(99, 96);
            this.tbReso.Name = "tbReso";
            this.tbReso.Size = new System.Drawing.Size(62, 19);
            this.tbReso.TabIndex = 9;
            this.tbReso.Text = "64";
            this.tbReso.Leave += new System.EventHandler(this.tbReso_Leave);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 124);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(69, 12);
            this.label5.TabIndex = 11;
            this.label5.Text = "Analyze size";
            // 
            // tbAnalyzeSize
            // 
            this.tbAnalyzeSize.Location = new System.Drawing.Point(99, 121);
            this.tbAnalyzeSize.Name = "tbAnalyzeSize";
            this.tbAnalyzeSize.Size = new System.Drawing.Size(62, 19);
            this.tbAnalyzeSize.TabIndex = 12;
            this.tbAnalyzeSize.Text = "1024";
            this.tbAnalyzeSize.Leave += new System.EventHandler(this.tbAnalyzeSize_Leave);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 152);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(81, 12);
            this.label6.TabIndex = 14;
            this.label6.Text = "Total level Mul";
            // 
            // tbTLlevelMul
            // 
            this.tbTLlevelMul.Location = new System.Drawing.Point(99, 149);
            this.tbTLlevelMul.Name = "tbTLlevelMul";
            this.tbTLlevelMul.Size = new System.Drawing.Size(62, 19);
            this.tbTLlevelMul.TabIndex = 15;
            this.tbTLlevelMul.Text = "4.0";
            this.tbTLlevelMul.Leave += new System.EventHandler(this.tbTLlevelMul_Leave);
            // 
            // btnRef
            // 
            this.btnRef.Location = new System.Drawing.Point(268, 19);
            this.btnRef.Name = "btnRef";
            this.btnRef.Size = new System.Drawing.Size(26, 23);
            this.btnRef.TabIndex = 2;
            this.btnRef.Text = "...";
            this.btnRef.UseVisualStyleBackColor = true;
            this.btnRef.Click += new System.EventHandler(this.btnRef_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(167, 49);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(18, 12);
            this.label7.TabIndex = 5;
            this.label7.Text = "Hz";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(167, 99);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(32, 12);
            this.label8.TabIndex = 10;
            this.label8.Text = "clock";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(167, 124);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(47, 12);
            this.label9.TabIndex = 13;
            this.label9.Text = "samples";
            // 
            // btnAccept
            // 
            this.btnAccept.Location = new System.Drawing.Point(136, 194);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(75, 23);
            this.btnAccept.TabIndex = 16;
            this.btnAccept.Text = "Analysis";
            this.btnAccept.UseVisualStyleBackColor = true;
            this.btnAccept.Click += new System.EventHandler(this.btnAccept_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(217, 194);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 17;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // FrmCSM
            // 
            this.AcceptButton = this.btnAccept;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(304, 229);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnAccept);
            this.Controls.Add(this.btnRef);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbTLlevelMul);
            this.Controls.Add(this.tbAnalyzeSize);
            this.Controls.Add(this.tbReso);
            this.Controls.Add(this.tbTempo);
            this.Controls.Add(this.tbSampleRate);
            this.Controls.Add(this.tbWaveFileName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximumSize = new System.Drawing.Size(320, 268);
            this.MinimumSize = new System.Drawing.Size(320, 268);
            this.Name = "FrmCSM";
            this.Text = "Make CSM y Command";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmCSM_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbWaveFileName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbSampleRate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbTempo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbReso;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbAnalyzeSize;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbTLlevelMul;
        private System.Windows.Forms.Button btnRef;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnAccept;
        private System.Windows.Forms.Button btnCancel;
    }
}