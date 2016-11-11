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
        #region Backup Step 3 - Upload

        const string addedPrefix = "[ADD]:";
        const string db3UploadPath = @"\db3";
        const string filesUploadPath = @"\delta-files";
        const string listsUploadPath = @"\delta-lists"; // For delete files

        long totalUploadedSize = 0;

        // Only used in delta copying:
        private void copyAllFiles(string filelistpath, BackupDirectoryInfo current) 
        {
            if (TryCancel()) return;

            if (!File.Exists(filelistpath)) return;
            using (System.IO.StreamReader file = new System.IO.StreamReader(filelistpath))
            {

                UpdateProgress(Desc: "Copying all added/changed files for:" + current.ServerName);

                

                string currentLine = null;
                while ((currentLine = file.ReadLine()) != null)
                {
                    if (TryCancel()) return;

                    if (currentLine.StartsWith(addedPrefix)) // might be delete if using same files for all.
                    {
                        // Get relative path from local folder
                        // current.Localpath include current folder name so we do some extra work
                        string currentFolderName = currentLine.Split('\\')[1];
                        currentLine = currentLine.Substring(addedPrefix.Length + (1 /*dash*/ + currentFolderName.Length)); // Remove root folder and add prefix

                        FileInfo fi = new FileInfo(current.LocalPath + currentLine); 
                        if (fi.Exists) // if local file exist
                        {
                            if (TryCancel()) return;

                            totalUploadedSize += fi.Length;
                            
                            CopyFileWithProgress(fi.FullName, varUploadfolder + filesUploadPath + "\\" + currentFolderName + currentLine);
                        }
                        else
                        {
                            Log("Couldn't find local file to upload: '" + fi.FullName + "'");
                        }
                    }
                }
            }
        }

        private void copyFolderProgressRecursive(string sourceDir, string targetDir)
        {
            if (TryCancel()) return;

            UpdateProgress(Desc: "Copying all added/changed files in folder for:" + sourceDir);

            DirectoryInfo sourceDi = new DirectoryInfo(sourceDir);
            DirectoryInfo targetDi = new DirectoryInfo(targetDir);
            if (!targetDi.Exists) targetDi.Create(); // In case of empty folder

            // Copy Files:
            foreach (FileInfo fi in sourceDi.GetFiles())
            {
                if (TryCancel()) return;

                totalUploadedSize += fi.Length;
                CopyFileWithProgress(fi.FullName, fi.FullName.Replace(sourceDir, targetDir));
            }

            // Recursive copy sub Folders:
            foreach (DirectoryInfo subdi in sourceDi.GetDirectories())
            {
                if (TryCancel()) return;

                copyFolderProgressRecursive(sourceDi + @"\" + subdi.Name, targetDir + @"\" + subdi.Name);
            }
        }

        private void copyAllFolders(string folderlistpath, BackupDirectoryInfo current) 
        {
            if (TryCancel()) return;

            if (!File.Exists(folderlistpath)) return;
            using (System.IO.StreamReader file = new System.IO.StreamReader(folderlistpath))
            {
            

                string currentLine = null;
                while ((currentLine = file.ReadLine()) != null)
                {
                    if (TryCancel()) return;

                    if (currentLine.StartsWith(addedPrefix)) // might be delete if using same files for all.
                    {
                        // Get relative path from local folder
                        // current.Localpath include current folder name so we do some extra work
                        string currentFolderName = currentLine.Split('\\')[1];
                        currentLine = currentLine.Substring(addedPrefix.Length + 1 + currentLine.Split('\\')[1].Length); // Remove root folder and add prefix

                        DirectoryInfo di = new DirectoryInfo(current.LocalPath + currentLine);
                        if (di.Exists) // if local file exist
                        {
                            if (TryCancel()) return;

                            copyFolderProgressRecursive(di.FullName, varUploadfolder + filesUploadPath + "\\" + currentFolderName + currentLine);
                        }
                        else
                        {
                            Log("Couldn't find local folder to upload: '" + di.FullName + "'");
                        }
                    }
                }
            }

        }

        private void backworkUploadFiles_DoWork(object sender, DoWorkEventArgs e)
        {
            if (!cbStep3.Checked) return; // Skip step
            foundSkipped = !cbSkipUpload.Checked; // If not checked, no need to find.

            Log("Step 3/4: Starting to upload files");
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
                    // Copy only delta
                    UpdateProgress(Status: "Step 3.3.1 Case 1: Upload delta files for:" + currentFolder.Key);
                    copyAllFiles(logAddedFiles.FullName, currentFolder.Value);
                    UpdateProgress(Status: "Step 3.3.2 Case 1: Upload delta folders for:" + currentFolder.Key);
                    copyAllFolders(logAddedFolders.FullName, currentFolder.Value);
                }
                else 
                {
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

        private void backworkUploadFiles_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbStatusProgress.Value = e.ProgressPercentage;
        }

        private void backworkUploadFiles_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Anyway stop the timer:
            tmrUploadProgress.Enabled = false;
            if (!foundSkipped) {
                Log("Didn't find the file to upload: " + txtUploadSkip.Text);
            }

            if (currentCancelled) // From TryCancel()
            {
                backupRunning = false;
                Log("Aborting uploading to server. (in step 3)");
                UpdateProgress(Status: "Step 3/4: Aborted all uploading", Desc: " ", progress: 100);
            }
            else
            {
                // Log each dir stats:
                foreach (KeyValuePair<string, BackupDirectoryInfo> currentFolder in _FoldersToBackup)
                {
                    if (currentFolder.Value.isUploadNeeded())
                    {
                        Log("Stat for folder: " + currentFolder.Key + "\n" + currentFolder.Value.ToString());
                    }
                    else 
                    {
                        Log("Folder: " + currentFolder.Key + " not uploaded because haven't changed.");
                    }
                }

                Log("Step 3/4: Finished uploading files.");
                // TODO: On cancel send "cancel to the server" (using user modal form);
                backworkLock.RunWorkerAsync();
            }
        }

        #endregion

        // SO? 6044629

        // For progress check:
        string currentFile = "";
        long sentBytes = 0, sentBytesSinceLast = 0, totalSizeBytes = 1;
        bool foundSkipped = false;

        public void CopyFileWithProgress(string SourceFilePath, string DestFilePath)
        {
            // Reset
            sentBytes = 0;
            sentBytesSinceLast = 0;
            totalSizeBytes = 1;
            currentFile = SourceFilePath;

            if (!foundSkipped && cbSkipUpload.Checked && txtUploadSkip.Text != SourceFilePath) {
                return;
            }
            else
            {
                foundSkipped = true;
            }

            try
            {
                byte[] buffer = new byte[1024 * 10]; // 10 KB buffer
                bool cancelFlag = false;

                using (FileStream source = new FileStream(SourceFilePath, FileMode.Open, FileAccess.Read))
                {
                    totalSizeBytes = source.Length;
                    using (FileStream dest = new FileStream(DestFilePath, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        sentBytes = 0;
                        int currentBlockSize = 0;

                        while ((currentBlockSize = source.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            sentBytes += currentBlockSize;
                            sentBytesSinceLast += currentBlockSize;

                            dest.Write(buffer, 0, currentBlockSize);

                            if (TryCancel()) return;

                            if (cancelFlag == true)
                            {
                                // Delete dest file here
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log("Error with uploading file: " + currentFile);
                Log(ex);
            }
        }

        const long _1kb = 1024;
        const long _1mb = 1048576;
        const long _1gb = 1073741824;
        const long _1tb = 1099511627776;

        string speedString (long speed) {
            if (speed < _1kb) {
                return (speed + " B");
            }
            if (speed < _1mb) 
            { 
                return (speed / _1kb + " KB");
            }
            if (speed < _1gb)
            { 
                return (speed / _1mb + " MB");
            }
            if (speed < _1tb)
            {
                return (speed / _1gb + " GB");
            }

            // Else just say in TB (unlikly)
            return (speed / _1tb + " TB");
        }

        private void tmrUploadProgress_Tick(object sender, EventArgs e)
        {
            long speed = (sentBytesSinceLast * 1000) / tmrUploadProgress.Interval;
            sentBytesSinceLast = 0;

            int percent = (int)((100 * sentBytes) / totalSizeBytes);
            string onlyfile = currentFile.Split('\\').Last();
            string trimSource = "";

            if (currentFile.Length > 20)
            {
                if (onlyfile.Length > 20)
                {
                    trimSource = onlyfile;
                }
                else
                {
                    trimSource = "..." + currentFile.Substring(currentFile.Length - 20, 20);
                }
            }
            else
            {
                trimSource = currentFile;
            }



            UpdateProgress(Desc: "[" + speedString(speed) + "] " + trimSource, progress: percent);
        }



    }
}
