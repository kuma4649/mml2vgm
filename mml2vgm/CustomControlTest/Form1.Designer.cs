namespace CustomControlTest
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("ノード7");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("ノード4", new System.Windows.Forms.TreeNode[] {
            treeNode1});
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("ノード8");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("ノード5", new System.Windows.Forms.TreeNode[] {
            treeNode3});
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("ノード6");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("ノード0", new System.Windows.Forms.TreeNode[] {
            treeNode2,
            treeNode4,
            treeNode5});
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("ノード1");
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("ノード3");
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("ノード2", new System.Windows.Forms.TreeNode[] {
            treeNode8});
            this.multiSelectTreeView1 = new CustomControl.MultiSelectTreeView();
            this.SuspendLayout();
            // 
            // multiSelectTreeView1
            // 
            this.multiSelectTreeView1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.multiSelectTreeView1.CheckedColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(160)))));
            this.multiSelectTreeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.multiSelectTreeView1.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawAll;
            this.multiSelectTreeView1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.multiSelectTreeView1.HotColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.multiSelectTreeView1.HotTracking = true;
            this.multiSelectTreeView1.Location = new System.Drawing.Point(0, 0);
            this.multiSelectTreeView1.Name = "multiSelectTreeView1";
            treeNode1.Name = "ノード7";
            treeNode1.Text = "ノード7";
            treeNode2.Name = "ノード4";
            treeNode2.Text = "ノード4";
            treeNode3.Name = "ノード8";
            treeNode3.Text = "ノード8";
            treeNode4.Name = "ノード5";
            treeNode4.Text = "ノード5";
            treeNode5.Name = "ノード6";
            treeNode5.Text = "ノード6";
            treeNode6.Name = "ノード0";
            treeNode6.Text = "ノード0";
            treeNode7.Name = "ノード1";
            treeNode7.Text = "ノード1";
            treeNode8.Name = "ノード3";
            treeNode8.Text = "ノード3";
            treeNode9.Name = "ノード2";
            treeNode9.Text = "ノード2";
            this.multiSelectTreeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode6,
            treeNode7,
            treeNode9});
            this.multiSelectTreeView1.SelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(180)))));
            this.multiSelectTreeView1.Size = new System.Drawing.Size(252, 372);
            this.multiSelectTreeView1.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(252, 372);
            this.Controls.Add(this.multiSelectTreeView1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private CustomControl.MultiSelectTreeView multiSelectTreeView1;
    }
}

