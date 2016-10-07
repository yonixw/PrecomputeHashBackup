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
        const string db3UploadPath = @"\upload\db3";
        const string filesUploadPath = @"\upload\delta-files";
        const string listsUploadPath = @"\upload\delta-lists"; // For delete files

        private void copyAllFiles(string filelistpath, BackupDirectoryInfo current) 
        {
            if (!File.Exists(filelistpath)) return;
            using (System.IO.StreamReader file = new System.IO.StreamReader(filelistpath))
            {

                UpdateProgress(Desc: "Copying all added/changed files for:" + current.ServerName);

                string uploadDeltaFilesLocation = txtServerUploadPath.Text;

                string currentLine = null;
                while ((currentLine = file.ReadLine()) != null)
                {
                    if (currentLine.StartsWith(addedPrefix)) // might be delete if using same files for all.
                    {
                        // Get relative path from local folder
                        currentLine = currentLine.Substring(addedPrefix.Length + 1 + currentLine.Split('\\')[1].Length); // Remove root folder and add prefix

                        FileInfo fi = new FileInfo(current.LocalPath + currentLine);
                        if (fi.Exists) // if local file exist
                        {
                            CopyFileWithProgress(fi.FullName, uploadDeltaFilesLocation + filesUploadPath + currentLine);
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
            UpdateProgress(Desc: "Copying all added/changed files in folder for:" + sourceDir);

            DirectoryInfo sourceDi = new DirectoryInfo(sourceDir);
            DirectoryInfo targetDi = new DirectoryInfo(targetDir);
            if (!targetDi.Exists) targetDi.Create(); // In case of empty folder

            // Copy Files:
            foreach (FileInfo fi in sourceDi.GetFiles())
            {
                CopyFileWithProgress(fi.FullName, fi.FullName.Replace(sourceDir, targetDir));
            }

            // Recursive copy sub Folders:
            foreach (DirectoryInfo subdi in sourceDi.GetDirectories())
            {
                copyFolderProgressRecursive(sourceDi + @"\" + subdi.Name, targetDir + @"\" + subdi.Name);
            }
        }

        private void copyAllFolders(string folderlistpath, BackupDirectoryInfo current) {

            if (!File.Exists(folderlistpath)) return;
            using (System.IO.StreamReader file = new System.IO.StreamReader(folderlistpath))
            {

                string uploadDeltaFilesLocation = txtServerUploadPath.Text;

                string currentLine = null;
                while ((currentLine = file.ReadLine()) != null)
                {
                    if (currentLine.StartsWith(addedPrefix)) // might be delete if using same files for all.
                    {
                        // Get relative path from local folder
                        currentLine = currentLine.Substring(addedPrefix.Length + 1 + currentLine.Split('\\')[1].Length); // Remove root folder and add prefix

                        DirectoryInfo di = new DirectoryInfo(current.LocalPath + currentLine);
                        if (di.Exists) // if local file exist
                        {
                            copyFolderProgressRecursive(di.FullName, uploadDeltaFilesLocation + filesUploadPath + currentLine);
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
            Log("Starting to upload files");
            UpdateProgress(Status: "Uploading files:", progress: 0);

            // Get folder for all lists:
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            DirectoryInfo listFolder = new DirectoryInfo(Path.Combine(folder, @"Precompute Backup Manager" + Path.DirectorySeparatorChar + _Foldername_DeltaLists));

            // For each folder try to upload deltas.
            foreach (KeyValuePair<string, BackupDirectoryInfo> currentFolder in _FoldersToBackup)
            {
                string currentListFolder = Directory.CreateDirectory(Path.Combine(listFolder.FullName, currentFolder.Value.ServerName)).FullName;
                FileInfo logAddedFiles = new FileInfo(Path.Combine(currentListFolder, "new-files.txt"));
                FileInfo logAddedFolders = new FileInfo(Path.Combine(currentListFolder, "new-folders.txt"));

                DateTime startCopy = DateTime.Now;
                if (currentFolder.Value.HasRecent)
                {
                    // Copy only delta
                    copyAllFiles(logAddedFiles.FullName, currentFolder.Value);
                    copyAllFolders(logAddedFolders.FullName, currentFolder.Value);
                }
                else 
                {
                    // Copy the entire folder but with progress!
                    // Use local folder name to backup, name on list is only for easy handling!
                    DirectoryInfo di = new DirectoryInfo(currentFolder.Value.LocalPath);
                    copyFolderProgressRecursive(di.FullName, txtServerUploadPath.Text + filesUploadPath + @"\" + di.Name);
                }
                currentFolder.Value.CopyDuration = DateTime.Now - startCopy;
            }
        }

        private void backworkUploadFiles_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbStatusProgress.Value = e.ProgressPercentage;
        }

        private void backworkUploadFiles_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Log("Finshed uploading files.");

            foreach (KeyValuePair<string, BackupDirectoryInfo> currentFolder in _FoldersToBackup)
            {
                Log("Stat for folder: " + currentFolder.Key + "\n" + currentFolder.Value.ToString());
            }

            // On cancel send "cancel to the server" (using user modal form);
            backworkLock.RunWorkerAsync();
        }

        #endregion

    }
}
