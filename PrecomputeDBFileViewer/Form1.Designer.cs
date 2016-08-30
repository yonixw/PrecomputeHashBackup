namespace PrecomputeDBFileViewer
{
    partial class Form1
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
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("Select Folder....");
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openSql3DbToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showDuplicatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.treeFolders = new System.Windows.Forms.TreeView();
            this.lsvFiles = new System.Windows.Forms.ListView();
            this.dlgOpen = new System.Windows.Forms.OpenFileDialog();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.menuStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.dBToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(658, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openSql3DbToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openSql3DbToolStripMenuItem
            // 
            this.openSql3DbToolStripMenuItem.Name = "openSql3DbToolStripMenuItem";
            this.openSql3DbToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.openSql3DbToolStripMenuItem.Text = "Open sql3 db...";
            this.openSql3DbToolStripMenuItem.Click += new System.EventHandler(this.openSql3DbToolStripMenuItem_Click);
            // 
            // dBToolStripMenuItem
            // 
            this.dBToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statsToolStripMenuItem,
            this.showDuplicatesToolStripMenuItem});
            this.dBToolStripMenuItem.Name = "dBToolStripMenuItem";
            this.dBToolStripMenuItem.Size = new System.Drawing.Size(102, 20);
            this.dBToolStripMenuItem.Text = "Precompute DB";
            // 
            // statsToolStripMenuItem
            // 
            this.statsToolStripMenuItem.Name = "statsToolStripMenuItem";
            this.statsToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.statsToolStripMenuItem.Text = "Stats";
            // 
            // showDuplicatesToolStripMenuItem
            // 
            this.showDuplicatesToolStripMenuItem.Name = "showDuplicatesToolStripMenuItem";
            this.showDuplicatesToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.showDuplicatesToolStripMenuItem.Text = "Show Duplicates";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.treeFolders, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lsvFiles, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 24);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(658, 465);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // treeFolders
            // 
            this.treeFolders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeFolders.Location = new System.Drawing.Point(3, 3);
            this.treeFolders.Name = "treeFolders";
            this.treeFolders.Size = new System.Drawing.Size(323, 459);
            this.treeFolders.TabIndex = 0;
            this.treeFolders.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.treeFolders_AfterExpand);
            this.treeFolders.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeFolders_AfterSelect);
            // 
            // lsvFiles
            // 
            this.lsvFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.lsvFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lsvFiles.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1});
            this.lsvFiles.Location = new System.Drawing.Point(332, 3);
            this.lsvFiles.Name = "lsvFiles";
            this.lsvFiles.Size = new System.Drawing.Size(323, 459);
            this.lsvFiles.TabIndex = 1;
            this.lsvFiles.UseCompatibleStateImageBehavior = false;
            this.lsvFiles.View = System.Windows.Forms.View.Details;
            // 
            // dlgOpen
            // 
            this.dlgOpen.Filter = "sqlite3 db file (*.db3)|*.db3|All files|*.*";
            this.dlgOpen.Title = "Open DB file";
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 84;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Hash";
            this.columnHeader2.Width = 37;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Size";
            this.columnHeader3.Width = 198;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(658, 489);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "DB Viewer for precompute sql3";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openSql3DbToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dBToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem statsToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TreeView treeFolders;
        private System.Windows.Forms.ListView lsvFiles;
        private System.Windows.Forms.ToolStripMenuItem showDuplicatesToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog dlgOpen;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
    }
}

