using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PrecomputeBackupManager.DataSet1TableAdapters;
using PrecomputeBackupManager.HashFileDatasetTableAdapters;

namespace PrecomputeBackupManager
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            reloadBackupFolders();
            LoadAllSettings();
            LoadBackupHistory();
        }

        public object safeDBNull(string column,DataRow dr, object fallback) {
            try {
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
            catch (Exception ex) {
                Log(ex);
                return fallback;
            }
        }

        public string safeDateDBnull(string column, DataRow dr) {
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


        #region >>>>>>>>>>>>>>>>>>>>>>>>> Log Tab [4]

        Queue<string> LogQue = new Queue<string>();

        private void logTimer_Tick(object sender, EventArgs e)
        {
            // For multi thread logging.
            while (LogQue.Count > 0) {
                lstLog.Items.Insert(0, LogQue.Dequeue());
            }
        }

        public void Log(string text)
        {
            LogQue.Enqueue("[" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss",
                                CultureInfo.InvariantCulture) + "] " + text);
        }

        public void Log(Exception ex)
        {
            LogQue.Enqueue( "[" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss",
                                CultureInfo.InvariantCulture) + "] " + ex.Message + "\n" + ex.StackTrace);
        }

        private void lstLog_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstLog.SelectedItems.Count > 0 ) {
                rtbCurrentLog.Text = (string)lstLog.SelectedItems[0];
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            lstLog.Items.Clear();
        }

        #endregion

        private Boolean TryCancel() {
            if (!currentWorker.CancellationPending) return false;

            Log("Backup was cancelled.");
            backupRunning = false;
            currentCancelled = true;
            return true;
        }

        private void UpdateProgress(string Status = null, string Desc = null, int progress = -1) {
            if (Status != null) {
                txtCurrentStatus.Invoke(new Action(() => {
                    txtCurrentStatus.Text = Status;
                }));
            }

            if (Desc != null)
            {
                txtStatusDescription.Invoke(new Action(() => {
                    txtStatusDescription.Text = Desc;
                }));
            }

            if (progress > -1)
            {
                pbStatusProgress.Invoke(new Action(() => {
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

        #endregion


        #region >>>>>>>>>>>>>>>>>>>>>>>>> Setting Tab [2]

        internal class KnownConfigKeys {
            public static string Usercode = "USER_CODE";
            public static string BackupAPIurl = "BACKUP_API_URL";
            public static string MaxBackupSize = "MAX_BACKUP_SIZE";
            public static string LogFolderServerName = "LOG_FOLDER_NAME";
            public static string ScheduleDays = "SCHEDULE_DAYS";
            public static string ScheduleHours = "SCHEDULE_HOURS";
            public static string ScheduleMinutes = "SCHEDULE_MINUTES";
            public static string ServerUploadPath = "SERVER_UPLOAD_PATH";
        }

        Dictionary<string, string> LoadSettingsFromDB() {
            DataSet1.ConfigDataTable dt = adapConfig.GetAllEntries();
            Dictionary<string, string> settings = new Dictionary<string, string>();

            foreach (DataSet1.ConfigRow row in dt) {
                settings.Add(row.Key, DBNull.Value.Equals(row.Value) ?   "0" : row.Value);
            }

            return settings;
        }

        void LoadAllSettings()
        {
            Dictionary<string, string> allSettings = LoadSettingsFromDB();

            txtServerUploadPath.Text = allSettings[KnownConfigKeys.ServerUploadPath];
            txtUsernameCode.Text = allSettings[KnownConfigKeys.Usercode];
            txtBackupApiURL.Text = allSettings[KnownConfigKeys.BackupAPIurl];
            numBackupMaxSize.Value = Int64.Parse(allSettings[KnownConfigKeys.MaxBackupSize]);
            txtLogFolderName.Text = allSettings[KnownConfigKeys.LogFolderServerName];
            numEveryDays.Value = Int64.Parse(allSettings[KnownConfigKeys.ScheduleDays]);
            numEveryHours.Value = Int64.Parse(allSettings[KnownConfigKeys.ScheduleHours]);
            numEveryMinutes.Value = Int64.Parse(allSettings[KnownConfigKeys.ScheduleMinutes]);

            Log("All settings were loaded from DB.");
        }


        void SaveAllSettings()
        {
            Dictionary<string, string> allSettings = LoadSettingsFromDB();

            allSettings[KnownConfigKeys.ServerUploadPath] = txtServerUploadPath.Text ?? "\\pi\\";
            allSettings[KnownConfigKeys.Usercode] = txtUsernameCode.Text ?? "0000";
            allSettings[KnownConfigKeys.BackupAPIurl] = txtBackupApiURL.Text ?? "HTTP";
            allSettings[KnownConfigKeys.MaxBackupSize] = numBackupMaxSize.Value.ToString() ;
            allSettings[KnownConfigKeys.LogFolderServerName] = txtLogFolderName.Text ?? "SERVER NAME";
            allSettings[KnownConfigKeys.ScheduleDays] = numEveryDays.Value.ToString();
            allSettings[KnownConfigKeys.ScheduleHours] = numEveryHours.Value.ToString();
            allSettings[KnownConfigKeys.ScheduleMinutes]= numEveryMinutes.Value.ToString();

            foreach (string key in allSettings.Keys)
            {
                if ((long)adapConfig.KeyExist(key) > 0) {
                    // Update
                    adapConfig.UpdateKey(allSettings[key], key);
                }
                else
                {
                    // Insert
                    adapConfig.NewKey(key, allSettings[key]);
                }
            }

            Log("All settings were saved to DB.");
        }

        private void btnSaveSettings_Click(object sender, EventArgs e)
        {
            SaveAllSettings();
        }

        #endregion


        #region >>>>>>>>>>>>>>>>>>>>>>>>> History Tab [3]

        int currentPage = 1;
        void LoadBackupHistory(int limit = 28) { // 28 came from brute trying.

            long historyCount = (long)adapStatus.GetCount();
            int pages = (int)Math.Ceiling(historyCount * 1.0f / limit);
            currentPage = Math.Max(Math.Min(currentPage, pages),1); // 1 <= page <= pages

            DataSet1.BackupLogsExDataTable dt = adapLogsEx.GetDataByPageOffset(limit, limit* (currentPage - 1));

            // Clear items from prev pages.
            lstvBackupHistory.Items.Clear();

            // Add history:
            foreach (DataSet1.BackupLogsExRow row in dt) {
                ListViewItem item = new ListViewItem(new[] {
                    (string)safeDBNull("FolderName",row,"???"),
                    safeDateDBnull("starttime", row),
                    safeDateDBnull("endtime", row),
                    (string)safeDBNull("size",row,"???"),
                    (string)safeDBNull("status",row,"???"),
                    (string)safeDBNull("statusdesc",row,"???"),
                });
                lstvBackupHistory.Items.Add(item);
            }

            AutoSizeLSTVColumn(lstvBackupHistory, -2);
            txtHistoryCurrentBackup.Text = "(" + currentPage + "/" + pages + ")";
            Log("Loaded all backup history.");

        }

        private void nextPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentPage++;
            LoadBackupHistory();
        }

        private void prevPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentPage--;
            LoadBackupHistory();
        }




        #endregion


        const string _FolderName_db3 = "db3";
        const string _FolderName_UploadedFiles = "delta-files";
        const string _Foldername_DeltaLists = "delta-lists";


        #region Backup Step 1 - Hash (offline\local)

        private DirectoryInfo HashSetup () {
            // SO? 16500080

            // Create Temp dir for db3 storage The folder for the roaming current user 
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string specificFolder = Path.Combine(folder, @"Precompute Backup Manager" + Path.DirectorySeparatorChar + _FolderName_db3);

            // Check if folder exists and if not, create it
            DirectoryInfo saveHashPath = new DirectoryInfo(specificFolder);
            if (!saveHashPath.Exists) {
                saveHashPath.Create();
                Log("Created temp folder for saving hash in:" + saveHashPath.FullName);
            }

            // Remove db3 files from last manager backup time if exists
            foreach(FileInfo fi in saveHashPath.GetFiles()) {
                if (fi.Extension == ".db3") {
                    fi.Delete();
                    Log("Deleted db3 file in temp:" + fi.FullName);
                }
            }

            return saveHashPath;
        }

        private DirectoryInfo BackupHashSetup() {
            // Create Temp dir for db3 storage The folder for the roaming current user 
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string specificFolder = Path.Combine(folder, @"Precompute Backup Manager" + Path.DirectorySeparatorChar + "recent-" + _FolderName_db3);

            // Check if folder exists and if not, create it
            DirectoryInfo saveHashPath = new DirectoryInfo(specificFolder);
            if (!saveHashPath.Exists)
            {
                saveHashPath.Create();
                Log("Created temp folder for copy last backup hash in:" + saveHashPath.FullName);
            }

            // Remove db3 files from last manager backup time if exists
            foreach (FileInfo fi in saveHashPath.GetFiles())
            {
                if (fi.Extension == ".db3")
                {
                    fi.Delete();
                    Log("Deleted db3 file in temp:" + fi.FullName);
                }
            }

            return saveHashPath;
        }

        private DirectoryInfo BackupListsSetup()
        {
            // Create Temp dir for db3 storage The folder for the roaming current user 
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string specificFolder = Path.Combine(folder, @"Precompute Backup Manager" + Path.DirectorySeparatorChar + _Foldername_DeltaLists);

            // Check if folder exists and if not, create it
            DirectoryInfo saveListPath = new DirectoryInfo(specificFolder);
            if (!saveListPath.Exists)
            {
                saveListPath.Create();
                Log("Created temp folder for copy last backup hash in:" + saveListPath.FullName);
            }

            // Remove list files in each sub directory
            foreach (DirectoryInfo di in saveListPath.GetDirectories())
            {
                di.Delete(true);
            }

            return saveListPath;
        }

        class BackupDirectoryInfo {
            // Report data
            public string ServerName;
            public string LocalPath;
            public TimeSpan HashDuration;
            public TimeSpan DiffDuration;
            public TimeSpan CopyDuration;
            public string LastError; // Even if success error 0.

            // Algo data:
            public bool HasRecent;
        }

        List<KeyValuePair<string, BackupDirectoryInfo>> _FoldersToBackup;
        private void backworkHashFiles_DoWork(object sender, DoWorkEventArgs e)
        {
            // Start worker:
            // =================================
            currentWorker = backworkHashFiles;
            Log("Started hashing files in the background");
            UpdateProgress(Status: "Step 1.1: Hashing folders");

            // Hash
            // =================================
            DirectoryInfo saveHashPath = HashSetup();

            // Save the list, to avoid changes:
            _FoldersToBackup = new List<KeyValuePair<string, BackupDirectoryInfo>>();

            lstvFoldersToBackup.Invoke(new Action(() => { 
                foreach (ListViewItem item in lstvFoldersToBackup.Items) {
                    // Server Name ---> Path locally
                    BackupDirectoryInfo bdi = new BackupDirectoryInfo()
                    {
                        ServerName = item.SubItems[1].Text,
                        LocalPath = item.SubItems[0].Text
                    };

                    _FoldersToBackup.Add(new KeyValuePair<string, BackupDirectoryInfo>(bdi.ServerName, bdi ));
                }
            }));

            string currentExePath = new FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location).DirectoryName;
            FileInfo templateDB3 = new FileInfo(Path.Combine(currentExePath,"template.db3"));

            // For each folder, Calculate db3 hash 
            foreach (KeyValuePair<string, BackupDirectoryInfo> currentFolder in _FoldersToBackup) {
                if (TryCancel()) return;

                string currentHashDB = Path.Combine(saveHashPath.FullName, currentFolder.Key + ".db3");

                // Copy template of hash db
                templateDB3.CopyTo(currentHashDB, true);

                // Set up adapters:
                PrecomputedHashDirDiff.PrecomputeHash hashClass = new PrecomputedHashDirDiff.PrecomputeHash();

                // Hash Folder
                UpdateProgress(Status: "Step 1.2: Hashing folder:" + currentFolder.Key);
                Log("Hashing Server Folder: " + currentFolder.Key);

                if (Directory.Exists(currentFolder.Value.LocalPath))
                {
                    hashClass.InitGeneric(currentFolder.Value.LocalPath, currentHashDB, currentWorker);
                }
                else
                {
                    _FoldersToBackup.Remove(currentFolder);
                    Log("Cannot find local folder :'" + currentFolder.Value.LocalPath + "'... removing from backup process (not from db)");
                }

                
            }

            if (TryCancel()) return;

            // COPY
            // =================================
            UpdateProgress(Status: "Step 1.3: Copy last backup db3");
            DirectoryInfo saveLastHashPath = BackupHashSetup();

            // Copy db3 folder from recent on the server
            DirectoryInfo recentBackup = new DirectoryInfo( Path.Combine(txtServerUploadPath.Text, "recent" + Path.DirectorySeparatorChar  + _FolderName_db3));
            if (!recentBackup.Exists)
            {
                // No comparison, upload all new.
                foreach (KeyValuePair<string, BackupDirectoryInfo> currentFolder in _FoldersToBackup)
                {
                    currentFolder.Value.HasRecent = false;
                }
            }
            else
            {
                foreach (KeyValuePair<string, BackupDirectoryInfo> currentFolder in _FoldersToBackup)
                {
                    string remoteDB3 = Path.Combine(recentBackup.FullName, currentFolder.Value.ServerName + ".db3");
                    string localDB3 = Path.Combine(saveLastHashPath.FullName, currentFolder.Value.ServerName + ".db3");

                    currentFolder.Value.HasRecent = File.Exists(remoteDB3);

                    if (currentFolder.Value.HasRecent) {
                        if (TryCancel()) return;

                        // Copy to our temp:
                        UpdateProgress(Desc: "Download db3 for server folder: " + currentFolder.Value.ServerName);
                        CopyFileWithProgress(remoteDB3, localDB3);
                    }
                }
            }

            // DIFF
            // =================================

            // Compute lists from hash diffs
            UpdateProgress(Status: "Step 1.4: Compute differences between dbs");

            DirectoryInfo listFolder =  BackupListsSetup();
            foreach (KeyValuePair<string, BackupDirectoryInfo> currentFolder in _FoldersToBackup)
            {
                if (currentFolder.Value.HasRecent) {
                    if (TryCancel()) return;

                    string lastDB3 = Path.Combine(saveLastHashPath.FullName, currentFolder.Value.ServerName + ".db3");
                    string freshDB3 = Path.Combine(saveHashPath.FullName, currentFolder.Value.ServerName + ".db3");

                    PrecomputedHashDirDiff.DiffDB diffObj = new PrecomputedHashDirDiff.DiffDB();

                    // Create folder for server name:
                    string currentListFolder = Directory.CreateDirectory(Path.Combine(listFolder.FullName, currentFolder.Value.ServerName)).FullName;
                    diffObj.logAddedFiles =         new FileInfo(Path.Combine(currentListFolder,"new-files.txt"));
                    diffObj.logAddedFolders =       new FileInfo(Path.Combine(currentListFolder,"new-folders.txt"));
                    diffObj.logDeletedFiles =       new FileInfo(Path.Combine(currentListFolder,"old-files.txt"));
                    diffObj.logDeletedFolders =     new FileInfo(Path.Combine(currentListFolder,"old-folders.txt"));
                    diffObj.useIOLog = true;

                    diffObj.Init(lastDB3, freshDB3);
                }
            }


            // For debug, always fail:
            backupRunning = false;
            currentCancelled = true;
        }

        private void backworkHashFiles_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState == null)
            {
                pbStatusProgress.Value = e.ProgressPercentage;
            }
            else
            {
                UpdateProgress(Desc: (string)e.UserState);
            }
        }

        private void backworkHashFiles_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (currentCancelled) {
                backupRunning = false;
                Log("Aborting hashing all folders.");
                UpdateProgress(Status: "Step 1/4: Aborted all hashing", Desc: " ", progress: 100);
            }
            else
            {
                Log("Step 1/4: Finished hashing all folders.");
                UpdateProgress(progress: 100);
                backworkUnlock.RunWorkerAsync();
            }
        }

        #endregion

        #region Backup Step 2 - Unlock

        private void backworkUnlock_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void backworkUnlock_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        private void backworkUnlock_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        #endregion

        #region Backup Step 3 - Upload

        private void backworkUploadFiles_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void backworkUploadFiles_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        private void backworkUploadFiles_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        #endregion

        #region Backup Step 4 - Lock

        private void backworkLock_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void backworkLock_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        private void backworkLock_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        #endregion


        // Current background thread, so we can cancel it if we want.
        bool backupRunning = false;
        bool currentCancelled = false;
        BackgroundWorker currentWorker = null;

        private void btnStartBackup_Click(object sender, EventArgs e)
        {
            if (backupRunning) {
                MessageBox.Show("A backup is already active. cancel it first.");
            }
            else
            {
                backupRunning = true;
                currentCancelled = false;

                // Start background work:
                backworkHashFiles.RunWorkerAsync();
            }
        }

        private void btnStopBackup_Click(object sender, EventArgs e)
        {
            if (currentWorker != null) {
                currentWorker.CancelAsync();
                // Dont say backupRunning = false, let the runner do that.
            }
        }


        #region Tools From OS

        //SO? 19755317 A:1997873

        //public delegate void IntDelegate(int Int);
        //public static event IntDelegate FileCopyProgress;

        public void CopyFileWithProgress(string source, string destination)
        {
            var webClient = new WebClient();
            webClient.DownloadProgressChanged += DownloadProgress;
            webClient.DownloadFileAsync(new Uri(source), destination);
        }

        private void DownloadProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            //if (FileCopyProgress != null)
            //    FileCopyProgress(e.ProgressPercentage);
            UpdateProgress(progress: e.ProgressPercentage);
        }
        #endregion
    }
}
