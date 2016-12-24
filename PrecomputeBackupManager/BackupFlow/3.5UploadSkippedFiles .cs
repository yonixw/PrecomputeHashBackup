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
using PrecomputeBackupManager.DataSetHashDBTableAdapters;

namespace PrecomputeBackupManager
{
    public partial class frmMain : Form
    {
        #region Backup Step 3.5 - Upload Skipped
    
        private void backgroundUploadSkipped_DoWork(object sender, DoWorkEventArgs e)
        {
            if (!cbStep3.Checked) return; // Skip step - use the same as upload

            Log("Step 3.5/4: Starting to upload skipped files");
            UpdateProgress(Status: "Uploading files:", progress: 0);

            // MUST BE INVOKED, otherwise the tick callback wont be called.
            this.Invoke(new Action(() => { tmrUploadProgress.Enabled = true; }));

            totalUploadedSize = 0;
            long lastUploadedSize = 0;

            // Get folder for all lists:
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            DirectoryInfo listFolder = new DirectoryInfo(Path.Combine(folder, @"Precompute Backup Manager" + Path.DirectorySeparatorChar + _Foldername_DeltaLists));
            DirectoryInfo db3Folder = new DirectoryInfo(Path.Combine(folder, @"Precompute Backup Manager" + Path.DirectorySeparatorChar + _FolderName_db3));

            // First copy db3 and delta list which almost always smaller, so better rate for sucess.
            //      Secondly, copy the actual files.

            if (TryCancel()) return;

            if (listFolder.Exists)
            {
                // Copy list of added\rem\del  to server
                UpdateProgress(Status: "Step 3.1: Upload delta lists");
                copyFolderProgressRecursive(listFolder.FullName, varUploadfolder + listsUploadPath);

                Log("Uploading delta lists for all folders. \nSize: " + Utils.byte2hum(totalUploadedSize));
                lastUploadedSize = totalUploadedSize;
            }

            if (TryCancel()) return;

            if (db3Folder.Exists)
            {
                // Copy fresh db3  to server:
                UpdateProgress(Status: "Step 3.2: Upload db3's");
                copyFolderProgressRecursive(db3Folder.FullName, varUploadfolder + db3UploadPath);

                Log("Uploading db3 for all folders. \nSize: " + Utils.byte2hum(totalUploadedSize - lastUploadedSize));
                lastUploadedSize = totalUploadedSize;
            }

            // For each folder try to upload deltas.
            foreach (KeyValuePair<string, BackupDirectoryInfo> currentFolder in _FoldersToBackup)
            {
                if (!currentFolder.Value.isUploadNeeded()) continue;

                if (TryCancel()) return;

                string currentListFolder = Directory.CreateDirectory(Path.Combine(listFolder.FullName, currentFolder.Value.ServerName)).FullName;
                FileInfo logAddedFiles = new FileInfo(Path.Combine(currentListFolder, "new-files.txt"));
                FileInfo logAddedFolders = new FileInfo(Path.Combine(currentListFolder, "new-folders.txt"));

                DateTime startCopy = DateTime.Now;
                if (currentFolder.Value.HasRecent)
                {
                    Log("Upload delta files and folders for folder: " + currentFolder.Key);

                    // Copy only delta
                    UpdateProgress(Status: "Step 3.3.1 Case 1: Upload delta files for:" + currentFolder.Key);
                    copyAllFiles(logAddedFiles.FullName, currentFolder.Value);
                    UpdateProgress(Status: "Step 3.3.2 Case 1: Upload delta folders for:" + currentFolder.Key);
                    copyAllFolders(logAddedFolders.FullName, currentFolder.Value);
                }
                else 
                {
                    Log("Upload entire folder: " + currentFolder.Key);

                    // Copy the entire folder but with progress!
                    // Use local folder name to backup, name on list is only for easy handling!
                    DirectoryInfo di = new DirectoryInfo(currentFolder.Value.LocalPath);
                    UpdateProgress(Status: "Step 3.1 Case 2: Upload entire folder for:" + currentFolder.Key);
                    copyFolderProgressRecursive(di.FullName, varUploadfolder + filesUploadPath + @"\" + di.Name);
                }
                currentFolder.Value.CopyDuration = DateTime.Now - startCopy;

                currentFolder.Value.UploadMeasuredSize =  totalUploadedSize - lastUploadedSize;
                lastUploadedSize = totalUploadedSize;
            }

            
            
        }

        private void backgroundUploadSkipped_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbStatusProgress.Value = e.ProgressPercentage;
        }

        private void backgroundUploadSkipped_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Anyway stop the timer:
            tmrUploadProgress.Enabled = false;
           
            // Move Forward to step 4
            if (currentCancelled) // From TryCancel()
            {
                backupRunning = false;
                Log("Aborting uploading to server. (in step 3.5)");
                UpdateProgress(Status: "Step 3.5/4: Aborted all uploading", Desc: " ", progress: 100);
            }
            else
            {
                
                Log("Step 3.5/4: Finished uploading files.");
                // TODO: On cancel send "cancel to the server" (using user modal form);
                backworkLock.RunWorkerAsync();
            }
        }

        #endregion


        

    }
}
