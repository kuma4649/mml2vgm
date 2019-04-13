namespace mml2vgmIDE
{
    partial class FrmMain
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.dpMain = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.visualStudioToolStripExtender1 = new WeifenLuo.WinFormsUI.Docking.VisualStudioToolStripExtender(this.components);
            this.vS2005Theme1 = new WeifenLuo.WinFormsUI.Docking.VS2005Theme();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.TsmiFile = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiNew = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiFileOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.フォルダーを開くFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.TsmiExit = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.元に戻すToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.やり直すToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.表示VToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.パートカウンターToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.フォルダーツリーToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.エラーリストToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ログToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.コンパイルCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiCompileAndPlay = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiAllCompile = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiCompile = new System.Windows.Forms.ToolStripMenuItem();
            this.ツールTToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.オプションOToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.データ作成手順ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mmlコマンドリファレンスToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripSplitButton1 = new System.Windows.Forms.ToolStripSplitButton();
            this.toolStripSplitButton2 = new System.Windows.Forms.ToolStripSplitButton();
            this.toolStripSplitButton3 = new System.Windows.Forms.ToolStripSplitButton();
            this.toolStripSplitButton4 = new System.Windows.Forms.ToolStripSplitButton();
            this.toolStripSplitButton5 = new System.Windows.Forms.ToolStripSplitButton();
            this.toolStripSplitButton6 = new System.Windows.Forms.ToolStripSplitButton();
            this.toolStripSplitButton7 = new System.Windows.Forms.ToolStripSplitButton();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dpMain
            // 
            this.dpMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dpMain.Location = new System.Drawing.Point(0, 24);
            this.dpMain.Name = "dpMain";
            this.dpMain.Size = new System.Drawing.Size(800, 405);
            this.dpMain.TabIndex = 0;
            this.dpMain.ActiveContentChanged += new System.EventHandler(this.DockPanel1_ActiveContentChanged);
            // 
            // visualStudioToolStripExtender1
            // 
            this.visualStudioToolStripExtender1.DefaultRenderer = null;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSplitButton2,
            this.toolStripSplitButton3,
            this.toolStripSplitButton1,
            this.toolStripSplitButton4,
            this.toolStripSplitButton6,
            this.toolStripSplitButton7,
            this.toolStripSplitButton5,
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 428);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(800, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TsmiFile,
            this.TsmiEdit,
            this.表示VToolStripMenuItem,
            this.コンパイルCToolStripMenuItem,
            this.ツールTToolStripMenuItem,
            this.TsmiHelp});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // TsmiFile
            // 
            this.TsmiFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TsmiNew,
            this.TsmiFileOpen,
            this.フォルダーを開くFToolStripMenuItem,
            this.toolStripSeparator1,
            this.TsmiExit});
            this.TsmiFile.Name = "TsmiFile";
            this.TsmiFile.Size = new System.Drawing.Size(67, 20);
            this.TsmiFile.Text = "ファイル(&F)";
            // 
            // TsmiNew
            // 
            this.TsmiNew.Name = "TsmiNew";
            this.TsmiNew.Size = new System.Drawing.Size(159, 22);
            this.TsmiNew.Text = "新規作成(&N)";
            this.TsmiNew.Click += new System.EventHandler(this.TsmiNew_Click);
            // 
            // TsmiFileOpen
            // 
            this.TsmiFileOpen.Name = "TsmiFileOpen";
            this.TsmiFileOpen.Size = new System.Drawing.Size(159, 22);
            this.TsmiFileOpen.Text = "ファイルを開く(&O)";
            this.TsmiFileOpen.Click += new System.EventHandler(this.TsmiFileOpen_Click);
            // 
            // フォルダーを開くFToolStripMenuItem
            // 
            this.フォルダーを開くFToolStripMenuItem.Name = "フォルダーを開くFToolStripMenuItem";
            this.フォルダーを開くFToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.フォルダーを開くFToolStripMenuItem.Text = "フォルダーを開く(&F)";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(156, 6);
            // 
            // TsmiExit
            // 
            this.TsmiExit.Name = "TsmiExit";
            this.TsmiExit.Size = new System.Drawing.Size(159, 22);
            this.TsmiExit.Text = "終了(&E)";
            this.TsmiExit.Click += new System.EventHandler(this.TsmiExit_Click);
            // 
            // TsmiEdit
            // 
            this.TsmiEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.元に戻すToolStripMenuItem,
            this.やり直すToolStripMenuItem});
            this.TsmiEdit.Name = "TsmiEdit";
            this.TsmiEdit.Size = new System.Drawing.Size(57, 20);
            this.TsmiEdit.Text = "編集(&E)";
            // 
            // 元に戻すToolStripMenuItem
            // 
            this.元に戻すToolStripMenuItem.Name = "元に戻すToolStripMenuItem";
            this.元に戻すToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.元に戻すToolStripMenuItem.Text = "元に戻す";
            // 
            // やり直すToolStripMenuItem
            // 
            this.やり直すToolStripMenuItem.Name = "やり直すToolStripMenuItem";
            this.やり直すToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.やり直すToolStripMenuItem.Text = "やり直し";
            // 
            // 表示VToolStripMenuItem
            // 
            this.表示VToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.パートカウンターToolStripMenuItem,
            this.フォルダーツリーToolStripMenuItem,
            this.エラーリストToolStripMenuItem,
            this.ログToolStripMenuItem});
            this.表示VToolStripMenuItem.Name = "表示VToolStripMenuItem";
            this.表示VToolStripMenuItem.Size = new System.Drawing.Size(58, 20);
            this.表示VToolStripMenuItem.Text = "表示(&V)";
            // 
            // パートカウンターToolStripMenuItem
            // 
            this.パートカウンターToolStripMenuItem.Name = "パートカウンターToolStripMenuItem";
            this.パートカウンターToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.パートカウンターToolStripMenuItem.Text = "パートカウンター";
            // 
            // フォルダーツリーToolStripMenuItem
            // 
            this.フォルダーツリーToolStripMenuItem.Name = "フォルダーツリーToolStripMenuItem";
            this.フォルダーツリーToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.フォルダーツリーToolStripMenuItem.Text = "フォルダーツリー";
            // 
            // エラーリストToolStripMenuItem
            // 
            this.エラーリストToolStripMenuItem.Name = "エラーリストToolStripMenuItem";
            this.エラーリストToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.エラーリストToolStripMenuItem.Text = "エラーリスト";
            // 
            // ログToolStripMenuItem
            // 
            this.ログToolStripMenuItem.Name = "ログToolStripMenuItem";
            this.ログToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.ログToolStripMenuItem.Text = "ログ";
            // 
            // コンパイルCToolStripMenuItem
            // 
            this.コンパイルCToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TsmiCompileAndPlay,
            this.TsmiAllCompile,
            this.TsmiCompile});
            this.コンパイルCToolStripMenuItem.Name = "コンパイルCToolStripMenuItem";
            this.コンパイルCToolStripMenuItem.Size = new System.Drawing.Size(80, 20);
            this.コンパイルCToolStripMenuItem.Text = "コンパイル(&C)";
            // 
            // TsmiCompileAndPlay
            // 
            this.TsmiCompileAndPlay.Name = "TsmiCompileAndPlay";
            this.TsmiCompileAndPlay.Size = new System.Drawing.Size(225, 22);
            this.TsmiCompileAndPlay.Text = "コンパイルと再生";
            this.TsmiCompileAndPlay.Click += new System.EventHandler(this.TsmiCompileAndPlay_Click);
            // 
            // TsmiAllCompile
            // 
            this.TsmiAllCompile.Enabled = false;
            this.TsmiAllCompile.Name = "TsmiAllCompile";
            this.TsmiAllCompile.Size = new System.Drawing.Size(225, 22);
            this.TsmiAllCompile.Text = "開いているファイル全てコンパイル";
            // 
            // TsmiCompile
            // 
            this.TsmiCompile.Name = "TsmiCompile";
            this.TsmiCompile.Size = new System.Drawing.Size(225, 22);
            this.TsmiCompile.Text = "コンパイルのみ";
            this.TsmiCompile.Click += new System.EventHandler(this.TsmiCompile_Click);
            // 
            // ツールTToolStripMenuItem
            // 
            this.ツールTToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.オプションOToolStripMenuItem});
            this.ツールTToolStripMenuItem.Name = "ツールTToolStripMenuItem";
            this.ツールTToolStripMenuItem.Size = new System.Drawing.Size(60, 20);
            this.ツールTToolStripMenuItem.Text = "ツール(&T)";
            // 
            // オプションOToolStripMenuItem
            // 
            this.オプションOToolStripMenuItem.Name = "オプションOToolStripMenuItem";
            this.オプションOToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.オプションOToolStripMenuItem.Text = "オプション(&O)";
            // 
            // TsmiHelp
            // 
            this.TsmiHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.データ作成手順ToolStripMenuItem,
            this.mmlコマンドリファレンスToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.TsmiHelp.Name = "TsmiHelp";
            this.TsmiHelp.Size = new System.Drawing.Size(65, 20);
            this.TsmiHelp.Text = "ヘルプ(&H)";
            // 
            // データ作成手順ToolStripMenuItem
            // 
            this.データ作成手順ToolStripMenuItem.Name = "データ作成手順ToolStripMenuItem";
            this.データ作成手順ToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.データ作成手順ToolStripMenuItem.Text = "データ作成手順";
            // 
            // mmlコマンドリファレンスToolStripMenuItem
            // 
            this.mmlコマンドリファレンスToolStripMenuItem.Name = "mmlコマンドリファレンスToolStripMenuItem";
            this.mmlコマンドリファレンスToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.mmlコマンドリファレンスToolStripMenuItem.Text = "mmlコマンドリファレンス";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.aboutToolStripMenuItem.Text = "About";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(149, 17);
            this.toolStripStatusLabel1.Text = "E:123 W:123 C:9999 LC:9999";
            // 
            // toolStripSplitButton1
            // 
            this.toolStripSplitButton1.DropDownButtonWidth = 0;
            this.toolStripSplitButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripSplitButton1.Image")));
            this.toolStripSplitButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSplitButton1.Name = "toolStripSplitButton1";
            this.toolStripSplitButton1.Size = new System.Drawing.Size(113, 20);
            this.toolStripSplitButton1.Text = "F5:コンパイル&再生";
            // 
            // toolStripSplitButton2
            // 
            this.toolStripSplitButton2.DropDownButtonWidth = 0;
            this.toolStripSplitButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripSplitButton2.Image")));
            this.toolStripSplitButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSplitButton2.Name = "toolStripSplitButton2";
            this.toolStripSplitButton2.Size = new System.Drawing.Size(62, 20);
            this.toolStripSplitButton2.Text = "F1:開く";
            // 
            // toolStripSplitButton3
            // 
            this.toolStripSplitButton3.DropDownButtonWidth = 0;
            this.toolStripSplitButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripSplitButton3.Image")));
            this.toolStripSplitButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSplitButton3.Name = "toolStripSplitButton3";
            this.toolStripSplitButton3.Size = new System.Drawing.Size(67, 20);
            this.toolStripSplitButton3.Text = "F2:保存";
            // 
            // toolStripSplitButton4
            // 
            this.toolStripSplitButton4.DropDownButtonWidth = 0;
            this.toolStripSplitButton4.Image = ((System.Drawing.Image)(resources.GetObject("toolStripSplitButton4.Image")));
            this.toolStripSplitButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSplitButton4.Name = "toolStripSplitButton4";
            this.toolStripSplitButton4.Size = new System.Drawing.Size(67, 20);
            this.toolStripSplitButton4.Text = "F6:再生";
            // 
            // toolStripSplitButton5
            // 
            this.toolStripSplitButton5.DropDownButtonWidth = 0;
            this.toolStripSplitButton5.Image = ((System.Drawing.Image)(resources.GetObject("toolStripSplitButton5.Image")));
            this.toolStripSplitButton5.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSplitButton5.Name = "toolStripSplitButton5";
            this.toolStripSplitButton5.Size = new System.Drawing.Size(67, 20);
            this.toolStripSplitButton5.Text = "F9:停止";
            // 
            // toolStripSplitButton6
            // 
            this.toolStripSplitButton6.DropDownButtonWidth = 0;
            this.toolStripSplitButton6.Image = ((System.Drawing.Image)(resources.GetObject("toolStripSplitButton6.Image")));
            this.toolStripSplitButton6.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSplitButton6.Name = "toolStripSplitButton6";
            this.toolStripSplitButton6.Size = new System.Drawing.Size(69, 20);
            this.toolStripSplitButton6.Text = "F7:スロー";
            // 
            // toolStripSplitButton7
            // 
            this.toolStripSplitButton7.DropDownButtonWidth = 0;
            this.toolStripSplitButton7.Image = ((System.Drawing.Image)(resources.GetObject("toolStripSplitButton7.Image")));
            this.toolStripSplitButton7.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSplitButton7.Name = "toolStripSplitButton7";
            this.toolStripSplitButton7.Size = new System.Drawing.Size(73, 20);
            this.toolStripSplitButton7.Text = "F8:4倍速";
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.dpMain);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FrmMain";
            this.Text = "mml2vgmIDE";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private WeifenLuo.WinFormsUI.Docking.DockPanel dpMain;
        private WeifenLuo.WinFormsUI.Docking.VisualStudioToolStripExtender visualStudioToolStripExtender1;
        private WeifenLuo.WinFormsUI.Docking.VS2005Theme vS2005Theme1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem TsmiFile;
        private System.Windows.Forms.ToolStripMenuItem TsmiNew;
        private System.Windows.Forms.ToolStripMenuItem TsmiFileOpen;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem TsmiExit;
        private System.Windows.Forms.ToolStripMenuItem TsmiEdit;
        private System.Windows.Forms.ToolStripMenuItem TsmiHelp;
        private System.Windows.Forms.ToolStripMenuItem コンパイルCToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem TsmiCompileAndPlay;
        private System.Windows.Forms.ToolStripMenuItem TsmiAllCompile;
        private System.Windows.Forms.ToolStripMenuItem TsmiCompile;
        private System.Windows.Forms.ToolStripMenuItem フォルダーを開くFToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 元に戻すToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem やり直すToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 表示VToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem パートカウンターToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem フォルダーツリーToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem エラーリストToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ログToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ツールTToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem オプションOToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem データ作成手順ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mmlコマンドリファレンスToolStripMenuItem;
        private System.Windows.Forms.ToolStripSplitButton toolStripSplitButton2;
        private System.Windows.Forms.ToolStripSplitButton toolStripSplitButton3;
        private System.Windows.Forms.ToolStripSplitButton toolStripSplitButton1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripSplitButton toolStripSplitButton4;
        private System.Windows.Forms.ToolStripSplitButton toolStripSplitButton6;
        private System.Windows.Forms.ToolStripSplitButton toolStripSplitButton7;
        private System.Windows.Forms.ToolStripSplitButton toolStripSplitButton5;
    }
}

