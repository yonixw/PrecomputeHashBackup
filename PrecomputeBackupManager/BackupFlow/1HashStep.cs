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
        #region Backup Step 1 - Hash (offline\local)

        private DirectoryInfo HashSetup()
        {
            // SO? 16500080

            // Create Temp dir for db3 storage The folder for the roaming current user 
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string specificFolder = Path.Combine(folder, @"Precompute Backup Manager" + Path.DirectorySeparatorChar + _FolderName_db3);

            // Check if folder exists and if not, create it
            DirectoryInfo saveHashPath = new DirectoryInfo(specificFolder);
            if (!saveHashPath.Exists)
            {
                saveHashPath.Create();
                Log("Created temp folder for saving hash in:" + saveHashPath.FullName);
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

        private DirectoryInfo BackupHashSetup()
        {
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

        class BackupDirectoryInfo
        {
            // Report data
            public string ServerName;
            public string LocalPath;
            public TimeSpan HashDuration;
            public TimeSpan DiffDuration;
            public TimeSpan CopyDuration;

            // Algo data:
            public bool HasRecent;

            // TODO: Provide Stats about all del\add\changed size and count.
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
                foreach (ListViewItem item in lstvFoldersToBackup.Items)
                {
                    // Server Name ---> Path locally
                    BackupDirectoryInfo bdi = new BackupDirectoryInfo()
                    {
                        ServerName = item.SubItems[1].Text,
                        LocalPath = item.SubItems[0].Text
                    };

                    _FoldersToBackup.Add(new KeyValuePair<string, BackupDirectoryInfo>(bdi.ServerName, bdi));
                }
            }));

            string currentExePath = new FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location).DirectoryName;
            FileInfo templateDB3 = new FileInfo(Path.Combine(currentExePath, "template.db3"));

            // For each folder, Calculate db3 hash 
            foreach (KeyValuePair<string, BackupDirectoryInfo> currentFolder in _FoldersToBackup)
            {
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
                    DateTime start = DateTime.Now;
                    hashClass.InitGeneric(currentFolder.Value.LocalPath, currentHashDB, currentWorker);
                    currentFolder.Value.HashDuration = DateTime.Now - start;
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
            DirectoryInfo recentBackup = new DirectoryInfo(Path.Combine(txtServerUploadPath.Text, "recent" + Path.DirectorySeparatorChar + _FolderName_db3));
            if (!recentBackup.Exists)
            {
                Log("Can't find recent folder in remote server, asssuming first backup.");
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

                    if (currentFolder.Value.HasRecent)
                    {
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

            DirectoryInfo listFolder = BackupListsSetup();
            foreach (KeyValuePair<string, BackupDirectoryInfo> currentFolder in _FoldersToBackup)
            {
                if (currentFolder.Value.HasRecent)
                {
                    if (TryCancel()) return;

                    string lastDB3 = Path.Combine(saveLastHashPath.FullName, currentFolder.Value.ServerName + ".db3");
                    string freshDB3 = Path.Combine(saveHashPath.FullName, currentFolder.Value.ServerName + ".db3");

                    PrecomputedHashDirDiff.DiffDB diffObj = new PrecomputedHashDirDiff.DiffDB();

                    // Create folder for server name:
                    string currentListFolder = Directory.CreateDirectory(Path.Combine(listFolder.FullName, currentFolder.Value.ServerName)).FullName;
                    diffObj.logAddedFiles = new FileInfo(Path.Combine(currentListFolder, "new-files.txt"));
                    diffObj.logAddedFolders = new FileInfo(Path.Combine(currentListFolder, "new-folders.txt"));
                    diffObj.logDeletedFiles = new FileInfo(Path.Combine(currentListFolder, "old-files.txt"));
                    diffObj.logDeletedFolders = new FileInfo(Path.Combine(currentListFolder, "old-folders.txt"));
                    diffObj.useIOLog = true;

                    Log("Starting diff for server folder: " + currentFolder.Value.ServerName);
                    diffObj.Init(lastDB3, freshDB3);
                    currentFolder.Value.DiffDuration = diffObj.duration;
                }
            }


            /* 
            After this step we have:
                1) db3 from old backup - Useless after this step, we leave it for debugging.
                2) db3 for current folder - We need to copy to upload folder
                3) list of files that should be added\deleted (delta) - we need to copy them, and them use the add-delta to copy to upload folder
            */

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
            if (currentCancelled)
            {
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
    }
}
