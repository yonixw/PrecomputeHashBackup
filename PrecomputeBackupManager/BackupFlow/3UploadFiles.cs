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

        private void copyAllFiles(string path) {
            if (!File.Exists(path)) return;
            System.IO.StreamReader file = new System.IO.StreamReader(path);

            string currentLine = null;
            while ((currentLine=file.ReadLine()) != null) {
                if (currentLine.StartsWith(addedPrefix)) {
                    // TODO: continue from here, nee to sort root folder issue with lists.
                }
            }
        }

        private void copyAllFolders(string path) { 

        }

        private void backworkUploadFiles_DoWork(object sender, DoWorkEventArgs e)
        {
            Log("Starting to upload files");
            UpdateProgress(Status: "Uploading files:", progress: 0);

            foreach (KeyValuePair<string, BackupDirectoryInfo> currentFolder in _FoldersToBackup)
            {
                if (currentFolder.Value.HasRecent)
                {
                    // Copy only delta
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
