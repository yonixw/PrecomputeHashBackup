namespace PrecomputedHashDirDiff
{
    partial class frmToolsGui
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
            this.btnCreateList = new System.Windows.Forms.Button();
            this.txtHashSourceDir = new System.Windows.Forms.TextBox();
            this.bwCalcHash = new System.ComponentModel.BackgroundWorker();
            this.pbHashStatus = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtBackup = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtTartget = new System.Windows.Forms.TextBox();
            this.btnDiffDB = new System.Windows.Forms.Button();
            this.bwDBdiff = new System.ComponentModel.BackgroundWorker();
            this.pbDiffStatus = new System.Windows.Forms.ProgressBar();
            this.label5 = new System.Windows.Forms.Label();
            this.txtDiffLog = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnCreateList
            // 
            this.btnCreateList.Location = new System.Drawing.Point(478, 67);
            this.btnCreateList.Name = "btnCreateList";
            this.btnCreateList.Size = new System.Drawing.Size(101, 23);
            this.btnCreateList.TabIndex = 0;
            this.btnCreateList.Text = "Create hash list";
            this.btnCreateList.UseVisualStyleBackColor = true;
            this.btnCreateList.Click += new System.EventHandler(this.btnCreateList_Click);
            // 
            // txtHashSourceDir
            // 
            this.txtHashSourceDir.Location = new System.Drawing.Point(12, 33);
            this.txtHashSourceDir.Name = "txtHashSourceDir";
            this.txtHashSourceDir.Size = new System.Drawing.Size(567, 20);
            this.txtHashSourceDir.TabIndex = 1;
            // 
            // bwCalcHash
            // 
            this.bwCalcHash.WorkerReportsProgress = true;
            this.bwCalcHash.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwCalcHash_DoWork);
            this.bwCalcHash.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bwCalcHash_ProgressChanged);
            this.bwCalcHash.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwCalcHash_RunWorkerCompleted);
            // 
            // pbHashStatus
            // 
            this.pbHashStatus.Location = new System.Drawing.Point(13, 66);
            this.pbHashStatus.Name = "pbHashStatus";
            this.pbHashStatus.Size = new System.Drawing.Size(459, 23);
            this.pbHashStatus.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(154, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Pre compute hash for directory:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 127);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(146, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Compare (IO\\DB) vs (IO\\DB):";
            // 
            // txtBackup
            // 
            this.txtBackup.Location = new System.Drawing.Point(15, 164);
            this.txtBackup.Name = "txtBackup";
            this.txtBackup.Size = new System.Drawing.Size(567, 20);
            this.txtBackup.TabIndex = 5;
            this.txtBackup.Text = "C:\\Users\\YoniWas\\Documents\\Visual Studio 2015\\Projects\\PrecomputedHashDirDiff\\Tes" +
    "t Cases v2\\original-backup.db3";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 148);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(105, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Backup db (Source):";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(20, 191);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(117, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Live Folder db (Target):";
            // 
            // txtTartget
            // 
            this.txtTartget.Location = new System.Drawing.Point(13, 207);
            this.txtTartget.Name = "txtTartget";
            this.txtTartget.Size = new System.Drawing.Size(567, 20);
            this.txtTartget.TabIndex = 7;
            this.txtTartget.Text = "C:\\Users\\YoniWas\\Documents\\Visual Studio 2015\\Projects\\PrecomputedHashDirDiff\\Tes" +
    "t Cases v2\\.db3";
            // 
            // btnDiffDB
            // 
            this.btnDiffDB.Location = new System.Drawing.Point(481, 309);
            this.btnDiffDB.Name = "btnDiffDB";
            this.btnDiffDB.Size = new System.Drawing.Size(101, 23);
            this.btnDiffDB.TabIndex = 9;
            this.btnDiffDB.Text = "Compare hash list";
            this.btnDiffDB.UseVisualStyleBackColor = true;
            this.btnDiffDB.Click += new System.EventHandler(this.btnDiffDB_Click);
            // 
            // bwDBdiff
            // 
            this.bwDBdiff.WorkerReportsProgress = true;
            this.bwDBdiff.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwDBdiff_DoWork);
            this.bwDBdiff.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bwDBdiff_ProgressChanged);
            this.bwDBdiff.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwDBdiff_RunWorkerCompleted);
            // 
            // pbDiffStatus
            // 
            this.pbDiffStatus.Location = new System.Drawing.Point(15, 309);
            this.pbDiffStatus.Name = "pbDiffStatus";
            this.pbDiffStatus.Size = new System.Drawing.Size(459, 23);
            this.pbDiffStatus.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(19, 235);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(128, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Log file (empty for no log):";
            // 
            // txtDiffLog
            // 
            this.txtDiffLog.Location = new System.Drawing.Point(12, 251);
            this.txtDiffLog.Name = "txtDiffLog";
            this.txtDiffLog.Size = new System.Drawing.Size(567, 20);
            this.txtDiffLog.TabIndex = 11;
            this.txtDiffLog.Text = "C:\\Users\\YoniWas\\Documents\\Visual Studio 2015\\Projects\\PrecomputedHashDirDiff\\Tes" +
    "t Cases v2\\1.log";
            // 
            // frmToolsGui
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(601, 359);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtDiffLog);
            this.Controls.Add(this.pbDiffStatus);
            this.Controls.Add(this.btnDiffDB);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtTartget);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtBackup);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pbHashStatus);
            this.Controls.Add(this.txtHashSourceDir);
            this.Controls.Add(this.btnCreateList);
            this.Name = "frmToolsGui";
            this.Text = "Tools Gui";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCreateList;
        private System.Windows.Forms.TextBox txtHashSourceDir;
        private System.ComponentModel.BackgroundWorker bwCalcHash;
        private System.Windows.Forms.ProgressBar pbHashStatus;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtBackup;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtTartget;
        private System.Windows.Forms.Button btnDiffDB;
        private System.ComponentModel.BackgroundWorker bwDBdiff;
        private System.Windows.Forms.ProgressBar pbDiffStatus;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtDiffLog;
    }
}

