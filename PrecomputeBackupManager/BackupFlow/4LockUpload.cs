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
        #region Backup Step 4 - Lock

        private void backworkLock_DoWork(object sender, DoWorkEventArgs e)
        {
            //Lock using remote url
            Log("Step 4/4: Asking server to lock.");
        }

        private void backworkLock_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbStatusProgress.Value = e.ProgressPercentage;
        }

        private void backworkLock_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Check for cancel but for now just start lock:
            Log("Step 4/4: Folder was locked.");
            backupRunning = false;
            UpdateProgress(Desc: "Finished backup by locking folder", Status: "Backup finished sucessfully", progress: 100);
            
        }

        #endregion

    }
}
