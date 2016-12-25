using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using PrecomputeBackupManager.DataSet1TableAdapters;
using PrecomputeBackupManager.DataSetHashDBTableAdapters;
using System.Collections.Concurrent;
using System.Security;

namespace PrecomputeBackupManager
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            reloadBackupFolders();
            LoadAllSettings();
        }

        public object safeDBNull(string column, DataRow dr, object fallback)
        {
            try
            {
                object result = null;
                if (!(dr[column] is DBNull))
                {
                    result = Convert.ChangeType(dr[column] ?? fallback, fallback.GetType());
                }
                else
                {
                    result = fallback;
                }
                return result;
            }
            catch (Exception ex)
            {
                Log(ex);
                return fallback;
            }
        }

        public string safeDateDBnull(string column, DataRow dr)
        {
            return ((DateTime)safeDBNull(column, dr, DateTime.MinValue)).ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
        }

        private void ntfIcon_DoubleClick(object sender, EventArgs e)
        {
            this.Visible = !this.Visible;
        }

        /**************************
           STATIC ADAPTERS
       ***************************/
        // Tables:
        static BackupFoldersTableAdapter adapFolders = new BackupFoldersTableAdapter();
        static BackupStatusTableAdapter adapStatus = new BackupStatusTableAdapter();
        static ConfigTableAdapter adapConfig = new ConfigTableAdapter();

        // Views:
        static BackupFolderExTableAdapter adapFoldersEx = new BackupFolderExTableAdapter();
        static BackupLogsExTableAdapter adapLogsEx = new BackupLogsExTableAdapter();


        #region >>>>>>>>>>>>>>>>>>>>>>>>> Log Tab [3]

        Queue<string> LogQue = new Queue<string>();
        Queue<string> SkippedLogQueue = new Queue<string>(); // With error description


        struct SkipInfo {
            public string source;
            public string target;
        }
        Queue<SkipInfo> SkippedListQueue = new Queue<SkipInfo>(); // Pure copy info

        string logFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            @"Precompute Backup Manager" + Path.DirectorySeparatorChar + "log_" +
            DateTime.Now.ToString("dd_MM_yyyy", CultureInfo.InvariantCulture)
            + ".txt");

        string skippedLogFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            @"Precompute Backup Manager" + Path.DirectorySeparatorChar + "skipped_info_" +
            DateTime.Now.ToString("dd_MM_yyyy", CultureInfo.InvariantCulture)
            + ".txt");

        string skippedNamesFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            @"Precompute Backup Manager" + Path.DirectorySeparatorChar + "skipped.txt");

        bool isLogtimerBusy = false;
        private void logTimer_Tick(object sender, EventArgs e)
        {
            if (isLogtimerBusy) return;
            isLogtimerBusy = true;

            // For multi thread logging.
            while (LogQue.Count > 0)
            {
                string addition = LogQue.Dequeue();

                lstLog.Items.Insert(0, addition);
                File.AppendAllText(logFilePath, "\n" + addition);
            }


            // Dump skipped file:

            while (SkippedLogQueue.Count > 0)
            {
                string addition = SkippedLogQueue.Dequeue();

                lstSkippedFiles.Items.Insert(0, addition);
                File.AppendAllText(skippedLogFilePath, "\n" + addition);
            }

            // Dump skipped file copy info:

            while (SkippedListQueue.Count > 0)
            {
                SkipInfo addition = SkippedListQueue.Dequeue();

                // 2 Rows: Source, Target
                File.AppendAllText(skippedNamesFilePath, addition.source +  "\n" + addition.target + '\n');
            }

            isLogtimerBusy = false;

        }

        public void Log(string text)
        {
            LogQue.Enqueue("[" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss",
                                CultureInfo.InvariantCulture) + "]\n " + text);
        }

        public void Log(Exception ex)
        {
            LogQue.Enqueue("[" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss",
                                CultureInfo.InvariantCulture) + "]\n " + ex.Message + "\n " + ex.StackTrace);
        }


        public void LogSkipped(string skippedFile, string targetCopy, string description)
        {
            SkippedLogQueue.Enqueue(
                "[ Logged: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss",
                                    CultureInfo.InvariantCulture) + "]\n"
                +  skippedFile + "\n" + description
            );

            SkippedListQueue.Enqueue(new SkipInfo() { source = skippedFile, target = targetCopy });
        }

        private void lstLog_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstLog.SelectedItems.Count > 0)
            {
                rtbCurrentLog.Text = (string)lstLog.SelectedItems[0];
            }
        }

        private void lstSkippedFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstSkippedFiles.SelectedItems.Count > 0)
            {
                rtbCurrentLog.Text = (string)lstSkippedFiles.SelectedItems[0];
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            lstLog.Items.Clear();
            lstSkippedFiles.Items.Clear();
        }

        private void btnLocalLogFolder_Click(object sender, EventArgs e)
        {
            Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Precompute Backup Manager");
        }

        #endregion

        public bool isBackupCancelled()
        {
            // Only checks if canceled.
            return (currentCancelled || (currentWorker != null && currentWorker.CancellationPending));
        }

        private Boolean TryCancel()
        {
            if (!isBackupCancelled()) return false;

            Log("Backup was cancelled.");
            backupRunning = false;
            currentCancelled = true;
            return true;
        }

        private void UpdateProgress(string Status = null, string Desc = null, int progress = -1)
        {
            if (Status != null)
            {
                txtCurrentStatus.Invoke(new Action(() =>
                {
                    txtCurrentStatus.Text = Status;
                }));
                AddPushBulletNoteToQueue("Backup BOT Update", Status);
            }

            if (Desc != null)
            {
                txtStatusDescription.Invoke(new Action(() =>
                {
                    txtStatusDescription.Text = Desc;
                }));
            }

            if (progress > -1)
            {
                pbStatusProgress.Invoke(new Action(() =>
                {
                    pbStatusProgress.Value = Math.Min(100, progress);
                }));
            }
        }

        #region >>>>>>>>>>>>>>>>>>>>>>>>> Backup Folder Tab [1]

        public static void AutoSizeLSTVColumn(ListView lstv, int width)
        {
            foreach (ColumnHeader col in lstv.Columns)
            {
                col.Width = width;
            }
        }

        void reloadBackupFolders()
        {
            // Clear Items:
            lstvFoldersToBackup.Items.Clear();

            // Load backup folders:
            foreach (DataSet1.BackupFolderExRow row in adapFoldersEx.GetData())
            {
                ListViewItem item = new ListViewItem(new[] { row.FolderPath, row.FolderName,
                    ((DateTime)safeDBNull("endtime",row,DateTime.MinValue)).ToString("dd/MM/yyyy HH:mm:ss",CultureInfo.InvariantCulture)
                });
                item.Tag = row.id;
                lstvFoldersToBackup.Items.Add(item);
            }

            AutoSizeLSTVColumn(lstvFoldersToBackup, -2);
            Log("Reloaded all backup folders.");
        }

        private void btnAddBackupFolder_Click(object sender, EventArgs e)
        {
            adapFolders.NewFolder("PATH HERE", "NAME HERE");
            clearCurrentBackupFolder();
            reloadBackupFolders();
        }

        /**************************
           Current Items
        ***************************/

        Int64 currentFolderID = -1;

        private void clearCurrentBackupFolder()
        {
            currentFolderID = -1;
            txtCurrentBackupFolderPath.Text = "";
            txtCurrentBackupName.Text = "";
        }

        private void lstvFoldersToBackup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstvFoldersToBackup.SelectedItems.Count > 0)
            {
                ListViewItem item = lstvFoldersToBackup.SelectedItems[0];
                currentFolderID = (Int64)item.Tag;
                txtCurrentBackupFolderPath.Text = item.Text;
                txtCurrentBackupName.Text = item.SubItems[1].Text; // 0 is the main text
            }
        }

        private void btnCurrentBackupFolderChoose_Click(object sender, EventArgs e)
        {
            if (dlgChooseFolder.ShowDialog() == DialogResult.OK)
            {
                txtCurrentBackupFolderPath.Text = new FileInfo(dlgChooseFolder.FileName).DirectoryName;
            }
        }

        private void btnSaveCurrentFolderChanges_Click(object sender, EventArgs e)
        {
            if (currentFolderID > -1)
            {
                if ((long)adapFolders.CheckIfExists(txtCurrentBackupName.Text, txtCurrentBackupFolderPath.Text, currentFolderID) > 0)
                {
                    // Duplicate exists:
                    MessageBox.Show("An item with similar data exists.", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    if ((long)adapFolders.CheckIfSubset(txtCurrentBackupFolderPath.Text, currentFolderID) > 0)
                    // '%' is a wild card in sql like statement
                    // /* SO? 2621586 */
                    {
                        // Path subset exists:
                        MessageBox.Show("An item with path prefix exists.", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    else
                    {
                        // Update:
                        adapFolders.UpdateFolder(txtCurrentBackupFolderPath.Text, txtCurrentBackupName.Text, (int?)currentFolderID);
                        reloadBackupFolders();
                        Log("Updated folder with id=" + currentFolderID.ToString());
                    }
                }
            }
            else
            {
                MessageBox.Show("No item is selected", "Info", MessageBoxButtons.OK);
            }
        }

        private void btnRemoveCurrentFolder_Click(object sender, EventArgs e)
        {
            if (currentFolderID > -1)
            {
                adapFolders.DeleteByID((int?)currentFolderID);
                Log("Removed backup folder with id=" + currentFolderID);
                reloadBackupFolders();
                clearCurrentBackupFolder();
            }
        }

        private void cbSkipUpload_CheckedChanged(object sender, EventArgs e)
        {
            txtUploadSkip.Enabled = cbSkipUpload.Checked;
        }

        #endregion


        #region >>>>>>>>>>>>>>>>>>>>>>>>> Setting Tab [2]

        public static string pbAuth = "";

        void LoadAllSettings()
        {
            AutoSettings _st = new AutoSettings();

            DataSet1.ConfigDataTable dt = adapConfig.GetAllEntries();

            foreach (DataSet1.ConfigRow row in dt)
            {
                _st.allSettings.Add(row.Key, DBNull.Value.Equals(row.Value) ? "0" : row.Value);
            }

            _st.WriteDataFromDictionary(tabBackupSettings);

            // Pushbullet need to be all across
            if (txtPushBulletAuthCode.Text.Length > 0)
            {
                pbAuth = txtPushBulletAuthCode.Text;
            }

            Log("All settings were loaded from DB.");
        }


        void SaveAllSettings()
        {
            AutoSettings _st = new AutoSettings();
            _st.ReadDataFromControl(tabBackupSettings);

            foreach (string key in _st.allSettings.Keys)
            {
                if ((long)adapConfig.KeyExist(key) > 0)
                {
                    // Update
                    adapConfig.UpdateKey(_st.allSettings[key], key);
                }
                else
                {
                    // Insert
                    adapConfig.NewKey(key, _st.allSettings[key]);
                }
            }

            Log("All settings were saved to DB.");
        }

        private void btnSaveSettings_Click(object sender, EventArgs e)
        {
            SaveAllSettings();
        }

        #endregion


        const string _FolderName_db3 = "db3";
        const string _FolderName_UploadedFiles = "delta-files";
        const string _Foldername_DeltaLists = "delta-lists";

        // Current background thread, so we can cancel it if we want.
        bool _backupRunning = false;
        bool backupRunning
        {
            get
            {
                return _backupRunning;
            }
            set
            {
                _backupRunning = value;
                if (value)
                {
                    SetWorkingState();
                }
                else
                {
                    SetIdleState();
                }
            }
        }

        bool currentCancelled = false; // Cause from code side, currentWorker.Cancel is from user side
        BackgroundWorker currentWorker = null;

        private bool checkRemote()
        {
            bool result = false;

            result = Directory.Exists(txtServerUploadPath.Text + "-backup");

            return result;
        }

        private void btnStartBackup_Click(object sender, EventArgs e)
        {
            if (backupRunning)
            {
                MessageBox.Show("A backup is already active. cancel it first.");
            }
            else
            {
                if (checkRemote())
                {
                    //TODO: 
                    // If db3,lists,delta-files folder doesnt exist, create them!

                    backupRunning = true;
                    currentCancelled = false;

                    // Start background work:
                    backworkHashFiles.RunWorkerAsync();
                }
                else
                {
                    MessageBox.Show("Cant find remote folder at: " + txtServerUploadPath.Text + "-backup");
                }
            }
        }

        private void btnStopBackup_Click(object sender, EventArgs e)
        {
            if (currentWorker != null)
            {
                currentWorker.CancelAsync();
                // Dont say backupRunning = false, let the runner do that.
            }
        }

        // TODO: Class to hold all setting before backup to avoid Invoke and Data change with backup.

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                if (MessageBox.Show("Really close the program?", "Alert", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }


        #region Tools
        uint fPreviousExecutionState = 0;

        public void SetWorkingState()
        {
            fPreviousExecutionState = NativeMethods.SetThreadExecutionState(
                NativeMethods.ES_CONTINUOUS | NativeMethods.ES_SYSTEM_REQUIRED);
            if (fPreviousExecutionState == 0)
            {
                Log("Cant set thread state to busy.");
            }
        }

        public void SetIdleState()
        {
            if (NativeMethods.SetThreadExecutionState(fPreviousExecutionState) == 0)
            {
                Log("Cant set thread state to idle.");
            }
        }

        internal static class NativeMethods
        {
            // Import SetThreadExecutionState Win32 API and necessary flags
            [DllImport("kernel32.dll")]
            public static extern uint SetThreadExecutionState(uint esFlags);
            public const uint ES_CONTINUOUS = 0x80000000;
            public const uint ES_SYSTEM_REQUIRED = 0x00000001;
        }

        // Need safe function for cases like junctions and symlinks.

        public FileInfo[] safeGet_Files(DirectoryInfo di)
        {
            FileInfo[] result = new FileInfo[] { };
            try
            {
                result = di.GetFiles();
            }
            catch (Exception ex)
            {
                Log(ex);
            }
            return result;
        }

        public DirectoryInfo[] safeGet_Directories(DirectoryInfo di)
        {
            DirectoryInfo[] result = new DirectoryInfo[] { };
            try
            {
                result = di.GetDirectories();
            }
            catch (Exception ex)
            {
                Log(ex);
            }
            return result;
        }
        #endregion

        #region Pushbullet queue

        class NoteExInfo
        {
            public string _title;
            public string _body;
            public Exception _stack; // For stack info
        }

        DateTime lastSentPushUpdate = DateTime.Now.AddHours(-2);

        // list of {title,body}
        // read about this thread-safe Queue here: https://msdn.microsoft.com/en-us/library/dd267265.aspx
        private ConcurrentQueue<NoteExInfo> _lowPriorityNotes = new ConcurrentQueue<NoteExInfo>();

        public void AddPushBulletNoteToQueue(string title, string body)
        {
            if (frmMain.pbAuth == "")
                return;

            _lowPriorityNotes.Enqueue(
                new NoteExInfo
                {
                    _title = title ?? "Backup BOT",
                    _body = body ?? "<No message body>",
                    _stack = new Exception("Push note origin stack")
                }
                );

            // Debug write it:
            Console.WriteLine("Sending pushbullet. Body:" + body);
        }

        private void tmrBackupPushUpdates_Tick(object sender, EventArgs e)
        {
            // Stop if no PushBullet code
            if (frmMain.pbAuth == "")
            {
                tmrBackupPushUpdates.Enabled = false;
                return;
            }

            // Add updates about the bakup process every hour.
            if (backupRunning)
            {
                if (DateTime.Now - lastSentPushUpdate > TimeSpan.FromHours(1))
                {
                    lastSentPushUpdate = DateTime.Now;
                    string message = "Backup process is still running."
                                               + "\n\n Current update status:\n"
                                               + txtCurrentStatus.Text

                                                + "\n\n Current update description:\n"
                                               + txtStatusDescription.Text

                                                + "\n\n Current update percentage: "
                                               + pbStatusProgress.Value + "%"
                                           ;

                    AddPushBulletNoteToQueue("Backup BOT Update", message);
                }
            }

            // Every interval (5 seconds) try and push to user some value from the list:
            if (_lowPriorityNotes.Count > 0)
            {
                NoteExInfo noteInfo;
                PushBulletAPI.PushNoteObject newNoteObject;

                if (_lowPriorityNotes.TryPeek(out noteInfo))
                {
                    if (null != (newNoteObject = PushBulletAPI.Pushes.createPushNote(noteInfo._title, noteInfo._body)))
                    {
                        // Sucess on sending. Remove from list.
                        if (!_lowPriorityNotes.TryDequeue(out noteInfo))
                        {
                            Log(new Exception("Cant dequeue after sending a note with peeking!"));
                        }

                        // Anyway tell me where the note came from:
                        Log("Push created with id: " + newNoteObject.iden
                            + "\nStack of creation:\n"
                            + noteInfo._stack.StackTrace);

                    }
                    else
                    {
                        // failed so dont do antything cause we used peek.
                        // but tell me:
                        Log("Couldn't send push note. Queue length:" + _lowPriorityNotes.Count);
                    }
                }
            }
        }



        #endregion

       
    }
}
