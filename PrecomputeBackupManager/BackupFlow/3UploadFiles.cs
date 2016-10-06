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

        private void copyAllFiles(string filelistpath, BackupDirectoryInfo current) 
        {
            if (!File.Exists(filelistpath)) return;
            System.IO.StreamReader file = new System.IO.StreamReader(filelistpath);

            UpdateProgress(Desc: "Copying all files for:" + current.ServerName);

            string currentLine = null;
            while ((currentLine=file.ReadLine()) != null) 
            {
                if (currentLine.StartsWith(addedPrefix)) 
                {
                    // TODO: continue from here, nee to sort root folder issue with lists.
                    currentLine = currentLine.Substring(addedPrefix.Length + 1 + currentLine.Split('\\')[1].Length); // Remove root folder and add prefix

                    FileInfo fi = new FileInfo(current.LocalPath + currentLine);
                    if (fi.Exists) 
                    {
                        CopyFileWithProgress(fi.FullName, txtServerUploadPath.Text + currentLine);
                    }
                }
            }
        }

        private void copyAllFolders(string path) { 

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

                if (currentFolder.Value.HasRecent)
                {
                    // Copy only delta
                    copyAllFiles(logAddedFiles.FullName, currentFolder.Value);
                }
                else 
                {
                    // Copy the entire folder
                }
            }
        }

        private void backworkUploadFiles_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbStatusProgress.Value = e.ProgressPercentage;
        }

        private void backworkUploadFiles_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Log("Finshed uploading files.");

            // On cancel send "cancel to the server" (using user modal form);
            backworkLock.RunWorkerAsync();
        }

        #endregion

    }
}
