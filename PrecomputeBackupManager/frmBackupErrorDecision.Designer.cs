namespace PrecomputeBackupManager
{
    partial class frmBackupErrorDecision
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
            this.label1 = new System.Windows.Forms.Label();
            this.rtbDetails = new System.Windows.Forms.RichTextBox();
            this.btnTryAgain = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.backgroundWorkerPushBulletDecision = new System.ComponentModel.BackgroundWorker();
            this.cbSave = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 9);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(319, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "An error occured while trying to upload a file:";
            // 
            // rtbDetails
            // 
            this.rtbDetails.Location = new System.Drawing.Point(17, 32);
            this.rtbDetails.Name = "rtbDetails";
            this.rtbDetails.ReadOnly = true;
            this.rtbDetails.Size = new System.Drawing.Size(516, 142);
            this.rtbDetails.TabIndex = 1;
            this.rtbDetails.Text = "";
            // 
            // btnTryAgain
            // 
            this.btnTryAgain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnTryAgain.Location = new System.Drawing.Point(12, 212);
            this.btnTryAgain.Name = "btnTryAgain";
            this.btnTryAgain.Size = new System.Drawing.Size(99, 34);
            this.btnTryAgain.TabIndex = 2;
            this.btnTryAgain.Text = "Try Again";
            this.btnTryAgain.UseVisualStyleBackColor = false;
            this.btnTryAgain.Click += new System.EventHandler(this.btnTryAgain_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btnCancel.Location = new System.Drawing.Point(403, 212);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(130, 34);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Skip file";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // backgroundWorkerPushBulletDecision
            // 
            this.backgroundWorkerPushBulletDecision.WorkerReportsProgress = true;
            this.backgroundWorkerPushBulletDecision.WorkerSupportsCancellation = true;
            this.backgroundWorkerPushBulletDecision.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerPushBulletDecision_DoWork);
            this.backgroundWorkerPushBulletDecision.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerPushBulletDecision_RunWorkerCompleted);
            // 
            // cbSave
            // 
            this.cbSave.AutoSize = true;
            this.cbSave.Location = new System.Drawing.Point(17, 182);
            this.cbSave.Name = "cbSave";
            this.cbSave.Size = new System.Drawing.Size(349, 24);
            this.cbSave.TabIndex = 4;
            this.cbSave.Text = "Remember my choise for this backup process";
            this.cbSave.UseVisualStyleBackColor = true;
            // 
            // frmBackupErrorDecision
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(545, 258);
            this.ControlBox = false;
            this.Controls.Add(this.cbSave);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnTryAgain);
            this.Controls.Add(this.rtbDetails);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "frmBackupErrorDecision";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Please Decide";
            this.Load += new System.EventHandler(this.frmBackupErrorDecision_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox rtbDetails;
        private System.Windows.Forms.Button btnTryAgain;
        private System.Windows.Forms.Button btnCancel;
        private System.ComponentModel.BackgroundWorker backgroundWorkerPushBulletDecision;
        private System.Windows.Forms.CheckBox cbSave;
    }
}