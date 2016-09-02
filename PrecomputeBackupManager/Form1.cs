﻿using System;
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
        }


        #region >>>>>>>>>>>>>>>>>>>>>>>>> Log Tab [4]

        public void Log(string text)
        {
            lstLog.Items.Insert(0, "[" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss",
                                CultureInfo.InvariantCulture) + "] " + text);
        }

        private void lstLog_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstLog.SelectedItems.Count > 0 ) {
                rtbCurrentLog.Text = (string)lstLog.SelectedItems[0];
            }
        }

        #endregion


        #region >>>>>>>>>>>>>>>>>>>>>>>>> Log Tab [1]

        public static void AutoSizeLSTVColumn(ListView lstv, int width)
        {
            foreach (ColumnHeader col in lstv.Columns)
            {
                col.Width = width;
            }
        }



        /**************************
            STATIC ADAPTERS
        ***************************/
        static BackupFoldersTableAdapter adapFolders = new BackupFoldersTableAdapter();
        static BackupStatusTableAdapter adapStatus = new BackupStatusTableAdapter();
        static ConfigTableAdapter adapConfig = new ConfigTableAdapter();

        void reloadBackupFolders()
        {
            // Clear Items:
            lstvFoldersToBackup.Items.Clear();

            // Load backup folders:
            foreach (DataSet1.BackupFoldersRow row in adapFolders.GetBackupFolders())
            {
                ListViewItem item = new ListViewItem(new[] { row.FolderPath, row.FolderName, row.id.ToString() });
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


    }
}