using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PrecomputeBackupManager.DataSet1TableAdapters;

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
                object result = Convert.ChangeType(dr[column] ?? fallback, fallback.GetType());
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

        /**************************
           STATIC ADAPTERS
       ***************************/
        static BackupFoldersTableAdapter adapFolders = new BackupFoldersTableAdapter();
        static BackupStatusTableAdapter adapStatus = new BackupStatusTableAdapter();
        static ConfigTableAdapter adapConfig = new ConfigTableAdapter();

        // Views:
        static BackupFolderExTableAdapter adapFoldersEx = new BackupFolderExTableAdapter();
        static BackupLogsExTableAdapter adapLogsEx = new BackupLogsExTableAdapter();


        #region >>>>>>>>>>>>>>>>>>>>>>>>> Log Tab [4]

        public void Log(string text)
        {
            lstLog.Items.Insert(0, "[" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss",
                                CultureInfo.InvariantCulture) + "] " + text);
        }

        public void Log(Exception ex)
        {
            lstLog.Items.Insert(0, "[" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss",
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

        
    }
}
