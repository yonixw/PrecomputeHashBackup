namespace PrecomputedHashDirDiff
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
            this.btnCreateList = new System.Windows.Forms.Button();
            this.txtHashSourceDir = new System.Windows.Forms.TextBox();
            this.bwCalcHash = new System.ComponentModel.BackgroundWorker();
            this.pbHashStatus = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
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
            this.label2.Location = new System.Drawing.Point(12, 155);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(144, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Compare premaid databases:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(601, 436);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pbHashStatus);
            this.Controls.Add(this.txtHashSourceDir);
            this.Controls.Add(this.btnCreateList);
            this.Name = "Form1";
            this.Text = "Form1";
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
    }
}

