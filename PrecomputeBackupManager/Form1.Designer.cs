﻿namespace PrecomputeBackupManager
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.btnAddBackupFolder = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label21 = new System.Windows.Forms.Label();
            this.pbStatusProgress = new System.Windows.Forms.ProgressBar();
            this.txtStatusDescription = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.txtCurrentStatus = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.btnStopBackup = new System.Windows.Forms.Button();
            this.btnStartBackup = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnCurrentBackupFolderChoose = new System.Windows.Forms.Button();
            this.txtCurrentBackupName = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.txtCurrentBackupFolderPath = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.btnRemoveCurrentFolder = new System.Windows.Forms.Button();
            this.btnSaveCurrentFolderChanges = new System.Windows.Forms.Button();
            this.lstvFoldersToBackup = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label5 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label22 = new System.Windows.Forms.Label();
            this.txtServerUploadPath = new System.Windows.Forms.TextBox();
            this.label23 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.lblNextBackupCountdown = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.numEveryMinutes = new System.Windows.Forms.NumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this.numEveryHours = new System.Windows.Forms.NumericUpDown();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.numEveryDays = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtLogFolderName = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.numBackupMaxSize = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.txtBackupApiURL = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSaveSettings = new System.Windows.Forms.Button();
            this.txtUsernameCode = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.lstvBackupHistory = new System.Windows.Forms.ListView();
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.prevPageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.txtHistoryCurrentBackup = new System.Windows.Forms.ToolStripTextBox();
            this.nextPageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.button1 = new System.Windows.Forms.Button();
            this.label16 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rtbCurrentLog = new System.Windows.Forms.RichTextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.lstLog = new System.Windows.Forms.ListBox();
            this.dlgChooseFolder = new System.Windows.Forms.OpenFileDialog();
            this.ntfIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.backworkHashFiles = new System.ComponentModel.BackgroundWorker();
            this.backworkUnlock = new System.ComponentModel.BackgroundWorker();
            this.backworkUploadFiles = new System.ComponentModel.BackgroundWorker();
            this.backworkLock = new System.ComponentModel.BackgroundWorker();
            this.logTimer = new System.Windows.Forms.Timer(this.components);
            this.tabMain.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numEveryMinutes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numEveryHours)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numEveryDays)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBackupMaxSize)).BeginInit();
            this.tabPage4.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabMain
            // 
            this.tabMain.Controls.Add(this.tabPage1);
            this.tabMain.Controls.Add(this.tabPage2);
            this.tabMain.Controls.Add(this.tabPage4);
            this.tabMain.Controls.Add(this.tabPage3);
            this.tabMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabMain.Location = new System.Drawing.Point(0, 0);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(1022, 772);
            this.tabMain.TabIndex = 2;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.btnAddBackupFolder);
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.lstvFoldersToBackup);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Location = new System.Drawing.Point(4, 29);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1014, 739);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Folders List";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // btnAddBackupFolder
            // 
            this.btnAddBackupFolder.Location = new System.Drawing.Point(895, 13);
            this.btnAddBackupFolder.Name = "btnAddBackupFolder";
            this.btnAddBackupFolder.Size = new System.Drawing.Size(94, 26);
            this.btnAddBackupFolder.TabIndex = 9;
            this.btnAddBackupFolder.Text = "Add Row";
            this.btnAddBackupFolder.UseVisualStyleBackColor = true;
            this.btnAddBackupFolder.Click += new System.EventHandler(this.btnAddBackupFolder_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label21);
            this.groupBox2.Controls.Add(this.pbStatusProgress);
            this.groupBox2.Controls.Add(this.txtStatusDescription);
            this.groupBox2.Controls.Add(this.label20);
            this.groupBox2.Controls.Add(this.txtCurrentStatus);
            this.groupBox2.Controls.Add(this.label19);
            this.groupBox2.Controls.Add(this.btnStopBackup);
            this.groupBox2.Controls.Add(this.btnStartBackup);
            this.groupBox2.Location = new System.Drawing.Point(21, 473);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(968, 258);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Current Status";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(25, 155);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(76, 20);
            this.label21.TabIndex = 13;
            this.label21.Text = "Progress:";
            // 
            // pbStatusProgress
            // 
            this.pbStatusProgress.Location = new System.Drawing.Point(172, 152);
            this.pbStatusProgress.Name = "pbStatusProgress";
            this.pbStatusProgress.Size = new System.Drawing.Size(600, 23);
            this.pbStatusProgress.TabIndex = 12;
            // 
            // txtStatusDescription
            // 
            this.txtStatusDescription.Location = new System.Drawing.Point(172, 104);
            this.txtStatusDescription.Name = "txtStatusDescription";
            this.txtStatusDescription.ReadOnly = true;
            this.txtStatusDescription.Size = new System.Drawing.Size(600, 26);
            this.txtStatusDescription.TabIndex = 10;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(25, 107);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(141, 20);
            this.label20.TabIndex = 11;
            this.label20.Text = "Status description:";
            // 
            // txtCurrentStatus
            // 
            this.txtCurrentStatus.Location = new System.Drawing.Point(148, 55);
            this.txtCurrentStatus.Name = "txtCurrentStatus";
            this.txtCurrentStatus.ReadOnly = true;
            this.txtCurrentStatus.Size = new System.Drawing.Size(624, 26);
            this.txtCurrentStatus.TabIndex = 9;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(25, 58);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(117, 20);
            this.label19.TabIndex = 9;
            this.label19.Text = "Current Status:";
            // 
            // btnStopBackup
            // 
            this.btnStopBackup.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.btnStopBackup.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.btnStopBackup.Location = new System.Drawing.Point(702, 202);
            this.btnStopBackup.Name = "btnStopBackup";
            this.btnStopBackup.Size = new System.Drawing.Size(123, 31);
            this.btnStopBackup.TabIndex = 1;
            this.btnStopBackup.Text = "Stop backup";
            this.btnStopBackup.UseVisualStyleBackColor = false;
            this.btnStopBackup.Click += new System.EventHandler(this.btnStopBackup_Click);
            // 
            // btnStartBackup
            // 
            this.btnStartBackup.Location = new System.Drawing.Point(831, 202);
            this.btnStartBackup.Name = "btnStartBackup";
            this.btnStartBackup.Size = new System.Drawing.Size(123, 31);
            this.btnStartBackup.TabIndex = 0;
            this.btnStartBackup.Text = "Start backup";
            this.btnStartBackup.UseVisualStyleBackColor = true;
            this.btnStartBackup.Click += new System.EventHandler(this.btnStartBackup_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnCurrentBackupFolderChoose);
            this.groupBox1.Controls.Add(this.txtCurrentBackupName);
            this.groupBox1.Controls.Add(this.label18);
            this.groupBox1.Controls.Add(this.txtCurrentBackupFolderPath);
            this.groupBox1.Controls.Add(this.label17);
            this.groupBox1.Controls.Add(this.btnRemoveCurrentFolder);
            this.groupBox1.Controls.Add(this.btnSaveCurrentFolderChanges);
            this.groupBox1.Location = new System.Drawing.Point(21, 320);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(968, 147);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Edit Folder";
            // 
            // btnCurrentBackupFolderChoose
            // 
            this.btnCurrentBackupFolderChoose.Location = new System.Drawing.Point(527, 36);
            this.btnCurrentBackupFolderChoose.Name = "btnCurrentBackupFolderChoose";
            this.btnCurrentBackupFolderChoose.Size = new System.Drawing.Size(94, 26);
            this.btnCurrentBackupFolderChoose.TabIndex = 8;
            this.btnCurrentBackupFolderChoose.Text = "Choose...";
            this.btnCurrentBackupFolderChoose.UseVisualStyleBackColor = true;
            this.btnCurrentBackupFolderChoose.Click += new System.EventHandler(this.btnCurrentBackupFolderChoose_Click);
            // 
            // txtCurrentBackupName
            // 
            this.txtCurrentBackupName.Location = new System.Drawing.Point(157, 68);
            this.txtCurrentBackupName.Name = "txtCurrentBackupName";
            this.txtCurrentBackupName.Size = new System.Drawing.Size(364, 26);
            this.txtCurrentBackupName.TabIndex = 7;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(25, 70);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(114, 20);
            this.label18.TabIndex = 6;
            this.label18.Text = "Friendly Name:";
            // 
            // txtCurrentBackupFolderPath
            // 
            this.txtCurrentBackupFolderPath.Location = new System.Drawing.Point(157, 36);
            this.txtCurrentBackupFolderPath.Name = "txtCurrentBackupFolderPath";
            this.txtCurrentBackupFolderPath.ReadOnly = true;
            this.txtCurrentBackupFolderPath.Size = new System.Drawing.Size(364, 26);
            this.txtCurrentBackupFolderPath.TabIndex = 5;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(25, 38);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(95, 20);
            this.label17.TabIndex = 4;
            this.label17.Text = "Folder Path:";
            // 
            // btnRemoveCurrentFolder
            // 
            this.btnRemoveCurrentFolder.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.btnRemoveCurrentFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.btnRemoveCurrentFolder.Location = new System.Drawing.Point(702, 107);
            this.btnRemoveCurrentFolder.Name = "btnRemoveCurrentFolder";
            this.btnRemoveCurrentFolder.Size = new System.Drawing.Size(123, 31);
            this.btnRemoveCurrentFolder.TabIndex = 3;
            this.btnRemoveCurrentFolder.Text = "Remove";
            this.btnRemoveCurrentFolder.UseVisualStyleBackColor = false;
            this.btnRemoveCurrentFolder.Click += new System.EventHandler(this.btnRemoveCurrentFolder_Click);
            // 
            // btnSaveCurrentFolderChanges
            // 
            this.btnSaveCurrentFolderChanges.Location = new System.Drawing.Point(831, 107);
            this.btnSaveCurrentFolderChanges.Name = "btnSaveCurrentFolderChanges";
            this.btnSaveCurrentFolderChanges.Size = new System.Drawing.Size(123, 31);
            this.btnSaveCurrentFolderChanges.TabIndex = 2;
            this.btnSaveCurrentFolderChanges.Text = "Apply changes";
            this.btnSaveCurrentFolderChanges.UseVisualStyleBackColor = true;
            this.btnSaveCurrentFolderChanges.Click += new System.EventHandler(this.btnSaveCurrentFolderChanges_Click);
            // 
            // lstvFoldersToBackup
            // 
            this.lstvFoldersToBackup.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.lstvFoldersToBackup.FullRowSelect = true;
            this.lstvFoldersToBackup.Location = new System.Drawing.Point(21, 43);
            this.lstvFoldersToBackup.Name = "lstvFoldersToBackup";
            this.lstvFoldersToBackup.Size = new System.Drawing.Size(968, 257);
            this.lstvFoldersToBackup.TabIndex = 3;
            this.lstvFoldersToBackup.UseCompatibleStateImageBehavior = false;
            this.lstvFoldersToBackup.View = System.Windows.Forms.View.Details;
            this.lstvFoldersToBackup.SelectedIndexChanged += new System.EventHandler(this.lstvFoldersToBackup_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Path";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Friendly Name";
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Last Backup end";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(17, 20);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(140, 20);
            this.label5.TabIndex = 2;
            this.label5.Text = "Folders to backup:";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label22);
            this.tabPage2.Controls.Add(this.txtServerUploadPath);
            this.tabPage2.Controls.Add(this.label23);
            this.tabPage2.Controls.Add(this.groupBox4);
            this.tabPage2.Controls.Add(this.label9);
            this.tabPage2.Controls.Add(this.label8);
            this.tabPage2.Controls.Add(this.label7);
            this.tabPage2.Controls.Add(this.txtLogFolderName);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.numBackupMaxSize);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.txtBackupApiURL);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.btnSaveSettings);
            this.tabPage2.Controls.Add(this.txtUsernameCode);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Location = new System.Drawing.Point(4, 29);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1014, 739);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Backup Setting";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.ForeColor = System.Drawing.Color.Red;
            this.label22.Location = new System.Drawing.Point(527, 52);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(362, 20);
            this.label22.TabIndex = 18;
            this.label22.Text = "Path of the designated upload folder on the server";
            // 
            // txtServerUploadPath
            // 
            this.txtServerUploadPath.Location = new System.Drawing.Point(147, 52);
            this.txtServerUploadPath.Name = "txtServerUploadPath";
            this.txtServerUploadPath.Size = new System.Drawing.Size(364, 26);
            this.txtServerUploadPath.TabIndex = 17;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(15, 55);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(100, 20);
            this.label23.TabIndex = 16;
            this.label23.Text = "Upload path:";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.lblNextBackupCountdown);
            this.groupBox4.Controls.Add(this.label15);
            this.groupBox4.Controls.Add(this.label14);
            this.groupBox4.Controls.Add(this.numEveryMinutes);
            this.groupBox4.Controls.Add(this.label13);
            this.groupBox4.Controls.Add(this.numEveryHours);
            this.groupBox4.Controls.Add(this.label12);
            this.groupBox4.Controls.Add(this.label11);
            this.groupBox4.Controls.Add(this.numEveryDays);
            this.groupBox4.Location = new System.Drawing.Point(19, 246);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(854, 125);
            this.groupBox4.TabIndex = 15;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Backup schedule";
            // 
            // lblNextBackupCountdown
            // 
            this.lblNextBackupCountdown.AutoSize = true;
            this.lblNextBackupCountdown.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.lblNextBackupCountdown.Location = new System.Drawing.Point(226, 83);
            this.lblNextBackupCountdown.Name = "lblNextBackupCountdown";
            this.lblNextBackupCountdown.Size = new System.Drawing.Size(196, 20);
            this.lblNextBackupCountdown.TabIndex = 23;
            this.lblNextBackupCountdown.Text = "<TIME\\UNDER BACKUP>";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(16, 83);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(195, 20);
            this.label15.TabIndex = 22;
            this.label15.Text = "Time left until next cackup:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.ForeColor = System.Drawing.Color.Red;
            this.label14.Location = new System.Drawing.Point(640, 37);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(69, 20);
            this.label14.TabIndex = 21;
            this.label14.Text = "Minutes.";
            // 
            // numEveryMinutes
            // 
            this.numEveryMinutes.Location = new System.Drawing.Point(512, 31);
            this.numEveryMinutes.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numEveryMinutes.Name = "numEveryMinutes";
            this.numEveryMinutes.Size = new System.Drawing.Size(122, 26);
            this.numEveryMinutes.TabIndex = 20;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.ForeColor = System.Drawing.Color.Red;
            this.label13.Location = new System.Drawing.Point(434, 37);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(56, 20);
            this.label13.TabIndex = 19;
            this.label13.Text = "Hours,";
            // 
            // numEveryHours
            // 
            this.numEveryHours.Location = new System.Drawing.Point(292, 31);
            this.numEveryHours.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numEveryHours.Name = "numEveryHours";
            this.numEveryHours.Size = new System.Drawing.Size(122, 26);
            this.numEveryHours.TabIndex = 18;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.ForeColor = System.Drawing.Color.Red;
            this.label12.Location = new System.Drawing.Point(226, 37);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(49, 20);
            this.label12.TabIndex = 17;
            this.label12.Text = "Days,";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(16, 33);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(52, 20);
            this.label11.TabIndex = 16;
            this.label11.Text = "Every:";
            // 
            // numEveryDays
            // 
            this.numEveryDays.Location = new System.Drawing.Point(93, 31);
            this.numEveryDays.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numEveryDays.Name = "numEveryDays";
            this.numEveryDays.Size = new System.Drawing.Size(122, 26);
            this.numEveryDays.TabIndex = 16;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.ForeColor = System.Drawing.Color.Red;
            this.label9.Location = new System.Drawing.Point(527, 197);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(346, 20);
            this.label9.TabIndex = 14;
            this.label9.Text = "Where to save logs of the manager in the server";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.Color.Red;
            this.label8.Location = new System.Drawing.Point(527, 96);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(185, 20);
            this.label8.TabIndex = 13;
            this.label8.Text = "path of API on the server";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.Color.Red;
            this.label7.Location = new System.Drawing.Point(527, 13);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(175, 20);
            this.label7.TabIndex = 12;
            this.label7.Text = "Secret code, ask admin";
            // 
            // txtLogFolderName
            // 
            this.txtLogFolderName.Location = new System.Drawing.Point(147, 193);
            this.txtLogFolderName.Name = "txtLogFolderName";
            this.txtLogFolderName.Size = new System.Drawing.Size(364, 26);
            this.txtLogFolderName.TabIndex = 11;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 197);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(128, 20);
            this.label6.TabIndex = 10;
            this.label6.Text = "Log folder name:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.Red;
            this.label4.Location = new System.Drawing.Point(527, 149);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(155, 20);
            this.label4.TabIndex = 9;
            this.label4.Text = "(KB) , 0 for unlimited.";
            // 
            // numBackupMaxSize
            // 
            this.numBackupMaxSize.Location = new System.Drawing.Point(147, 147);
            this.numBackupMaxSize.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numBackupMaxSize.Name = "numBackupMaxSize";
            this.numBackupMaxSize.Size = new System.Drawing.Size(364, 26);
            this.numBackupMaxSize.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 149);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(130, 20);
            this.label3.TabIndex = 7;
            this.label3.Text = "Max backup size:";
            // 
            // txtBackupApiURL
            // 
            this.txtBackupApiURL.Location = new System.Drawing.Point(147, 96);
            this.txtBackupApiURL.Name = "txtBackupApiURL";
            this.txtBackupApiURL.Size = new System.Drawing.Size(364, 26);
            this.txtBackupApiURL.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 99);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(118, 20);
            this.label2.TabIndex = 5;
            this.label2.Text = "Backup API url:";
            // 
            // btnSaveSettings
            // 
            this.btnSaveSettings.Location = new System.Drawing.Point(457, 400);
            this.btnSaveSettings.Name = "btnSaveSettings";
            this.btnSaveSettings.Size = new System.Drawing.Size(134, 36);
            this.btnSaveSettings.TabIndex = 4;
            this.btnSaveSettings.Text = "Save Changes";
            this.btnSaveSettings.UseVisualStyleBackColor = true;
            this.btnSaveSettings.Click += new System.EventHandler(this.btnSaveSettings_Click);
            // 
            // txtUsernameCode
            // 
            this.txtUsernameCode.Location = new System.Drawing.Point(147, 11);
            this.txtUsernameCode.Name = "txtUsernameCode";
            this.txtUsernameCode.PasswordChar = ' ';
            this.txtUsernameCode.Size = new System.Drawing.Size(364, 26);
            this.txtUsernameCode.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(126, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "Username code:";
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.lstvBackupHistory);
            this.tabPage4.Controls.Add(this.menuStrip1);
            this.tabPage4.Location = new System.Drawing.Point(4, 29);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(1014, 739);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Backup History";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // lstvBackupHistory
            // 
            this.lstvBackupHistory.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7,
            this.columnHeader8,
            this.columnHeader9});
            this.lstvBackupHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstvBackupHistory.FullRowSelect = true;
            this.lstvBackupHistory.Location = new System.Drawing.Point(3, 36);
            this.lstvBackupHistory.Name = "lstvBackupHistory";
            this.lstvBackupHistory.Size = new System.Drawing.Size(1008, 700);
            this.lstvBackupHistory.TabIndex = 4;
            this.lstvBackupHistory.UseCompatibleStateImageBehavior = false;
            this.lstvBackupHistory.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Folder Name";
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Start time";
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "End time";
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Backup Size";
            this.columnHeader7.Width = 104;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Status Code";
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "Status Description";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.prevPageToolStripMenuItem,
            this.txtHistoryCurrentBackup,
            this.nextPageToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(3, 3);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1008, 33);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // prevPageToolStripMenuItem
            // 
            this.prevPageToolStripMenuItem.Name = "prevPageToolStripMenuItem";
            this.prevPageToolStripMenuItem.Size = new System.Drawing.Size(123, 29);
            this.prevPageToolStripMenuItem.Text = "<--- Prev Page";
            this.prevPageToolStripMenuItem.Click += new System.EventHandler(this.prevPageToolStripMenuItem_Click);
            // 
            // txtHistoryCurrentBackup
            // 
            this.txtHistoryCurrentBackup.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtHistoryCurrentBackup.Name = "txtHistoryCurrentBackup";
            this.txtHistoryCurrentBackup.ReadOnly = true;
            this.txtHistoryCurrentBackup.Size = new System.Drawing.Size(100, 29);
            this.txtHistoryCurrentBackup.Text = "(0/0)";
            // 
            // nextPageToolStripMenuItem
            // 
            this.nextPageToolStripMenuItem.Name = "nextPageToolStripMenuItem";
            this.nextPageToolStripMenuItem.Size = new System.Drawing.Size(124, 29);
            this.nextPageToolStripMenuItem.Text = "Next Page --->";
            this.nextPageToolStripMenuItem.Click += new System.EventHandler(this.nextPageToolStripMenuItem_Click);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.button1);
            this.tabPage3.Controls.Add(this.label16);
            this.tabPage3.Controls.Add(this.groupBox3);
            this.tabPage3.Controls.Add(this.label10);
            this.tabPage3.Controls.Add(this.lstLog);
            this.tabPage3.Location = new System.Drawing.Point(4, 29);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(1014, 739);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Log";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(905, 10);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(98, 34);
            this.button1.TabIndex = 17;
            this.button1.Text = "Clear log";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(11, 17);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(276, 20);
            this.label16.TabIndex = 16;
            this.label16.Text = "Events since the last program started:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.rtbCurrentLog);
            this.groupBox3.Location = new System.Drawing.Point(12, 571);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(994, 149);
            this.groupBox3.TabIndex = 15;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Current Item";
            // 
            // rtbCurrentLog
            // 
            this.rtbCurrentLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbCurrentLog.Location = new System.Drawing.Point(3, 22);
            this.rtbCurrentLog.Name = "rtbCurrentLog";
            this.rtbCurrentLog.ReadOnly = true;
            this.rtbCurrentLog.Size = new System.Drawing.Size(988, 124);
            this.rtbCurrentLog.TabIndex = 0;
            this.rtbCurrentLog.Text = "";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.ForeColor = System.Drawing.Color.Red;
            this.label10.Location = new System.Drawing.Point(295, 17);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(205, 20);
            this.label10.TabIndex = 14;
            this.label10.Text = "Click on item to show bellow";
            // 
            // lstLog
            // 
            this.lstLog.FormattingEnabled = true;
            this.lstLog.ItemHeight = 20;
            this.lstLog.Location = new System.Drawing.Point(12, 60);
            this.lstLog.Name = "lstLog";
            this.lstLog.Size = new System.Drawing.Size(994, 504);
            this.lstLog.TabIndex = 0;
            this.lstLog.SelectedIndexChanged += new System.EventHandler(this.lstLog_SelectedIndexChanged);
            // 
            // dlgChooseFolder
            // 
            this.dlgChooseFolder.AddExtension = false;
            this.dlgChooseFolder.CheckFileExists = false;
            this.dlgChooseFolder.FileName = "Choose";
            this.dlgChooseFolder.ValidateNames = false;
            // 
            // ntfIcon
            // 
            this.ntfIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.ntfIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("ntfIcon.Icon")));
            this.ntfIcon.Text = "PreBackup Manager";
            this.ntfIcon.Visible = true;
            this.ntfIcon.DoubleClick += new System.EventHandler(this.ntfIcon_DoubleClick);
            // 
            // backworkHashFiles
            // 
            this.backworkHashFiles.WorkerReportsProgress = true;
            this.backworkHashFiles.WorkerSupportsCancellation = true;
            this.backworkHashFiles.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backworkHashFiles_DoWork);
            this.backworkHashFiles.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backworkHashFiles_ProgressChanged);
            this.backworkHashFiles.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backworkHashFiles_RunWorkerCompleted);
            // 
            // backworkUnlock
            // 
            this.backworkUnlock.WorkerReportsProgress = true;
            this.backworkUnlock.WorkerSupportsCancellation = true;
            this.backworkUnlock.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backworkUnlock_DoWork);
            this.backworkUnlock.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backworkUnlock_ProgressChanged);
            this.backworkUnlock.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backworkUnlock_RunWorkerCompleted);
            // 
            // backworkUploadFiles
            // 
            this.backworkUploadFiles.WorkerReportsProgress = true;
            this.backworkUploadFiles.WorkerSupportsCancellation = true;
            this.backworkUploadFiles.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backworkUploadFiles_DoWork);
            this.backworkUploadFiles.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backworkUploadFiles_ProgressChanged);
            this.backworkUploadFiles.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backworkUploadFiles_RunWorkerCompleted);
            // 
            // backworkLock
            // 
            this.backworkLock.WorkerReportsProgress = true;
            this.backworkLock.WorkerSupportsCancellation = true;
            this.backworkLock.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backworkLock_DoWork);
            this.backworkLock.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backworkLock_ProgressChanged);
            this.backworkLock.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backworkLock_RunWorkerCompleted);
            // 
            // logTimer
            // 
            this.logTimer.Enabled = true;
            this.logTimer.Interval = 750;
            this.logTimer.Tick += new System.EventHandler(this.logTimer_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1022, 772);
            this.Controls.Add(this.tabMain);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Backup Manager";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabMain.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numEveryMinutes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numEveryHours)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numEveryDays)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBackupMaxSize)).EndInit();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnStopBackup;
        private System.Windows.Forms.Button btnStartBackup;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListView lstvFoldersToBackup;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtLogFolderName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numBackupMaxSize;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtBackupApiURL;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnSaveSettings;
        private System.Windows.Forms.TextBox txtUsernameCode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RichTextBox rtbCurrentLog;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ListBox lstLog;
        private System.Windows.Forms.Button btnRemoveCurrentFolder;
        private System.Windows.Forms.Button btnSaveCurrentFolderChanges;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.NumericUpDown numEveryMinutes;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.NumericUpDown numEveryHours;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.NumericUpDown numEveryDays;
        private System.Windows.Forms.Label lblNextBackupCountdown;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox txtCurrentBackupFolderPath;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Button btnCurrentBackupFolderChoose;
        private System.Windows.Forms.TextBox txtCurrentBackupName;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.ProgressBar pbStatusProgress;
        private System.Windows.Forms.TextBox txtStatusDescription;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox txtCurrentStatus;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Button btnAddBackupFolder;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.ListView lstvBackupHistory;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem prevPageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nextPageToolStripMenuItem;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.OpenFileDialog dlgChooseFolder;
        private System.Windows.Forms.ToolStripTextBox txtHistoryCurrentBackup;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.TextBox txtServerUploadPath;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.NotifyIcon ntfIcon;
        private System.ComponentModel.BackgroundWorker backworkHashFiles;
        private System.ComponentModel.BackgroundWorker backworkUnlock;
        private System.ComponentModel.BackgroundWorker backworkUploadFiles;
        private System.ComponentModel.BackgroundWorker backworkLock;
        private System.Windows.Forms.Timer logTimer;
    }
}

