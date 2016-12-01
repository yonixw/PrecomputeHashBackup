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
    public partial class frmMain : Form
    {
        #region Backup Step 4 - Lock

        DateTime startLockWaiting;
        private void backworkLock_DoWork(object sender, DoWorkEventArgs e)
        {
            if (!cbStep4.Checked) return; // Skip step

            int tryCount = 0;

            //Lock using remote url
            Log("Step 4/4: Asking server to lock.");
            UpdateProgress(Status: "Step 4.1: Waiting for lock", Desc: "Sending request to server", progress: 0);
            startLockWaiting = DateTime.Now;

            if (TryCancel()) return;

            // Wait until we get a response that 
            // Of course, stop if we got a cancel and try to "cancel" (url should support 3 types: unlock, lock, cancel)
            BackupActionResult actionResult = serverlock(int.Parse(txtUsernameCode.Text),CurrentBackupUpdateID, totalUploadedSize); 
            if (actionResult == null || actionResult.updateid < 0)
            {
                currentCancelled = true;
                Log("Error while locking. is result Null? " + (actionResult == null).ToString());
                return;
            }

            if (TryCancel()) return;

            // Now check satus until ready (4)
            // Status code 3 is still locking, status code 4 is locked and ready to go!

            BackupProcessStatus status = getBackupProcessUpdate(CurrentBackupUpdateID);
            while (status != null && status.statuscode < 4)
            {
                tryCount++;
                UpdateProgress(Desc: "Try number: " + tryCount.ToString());
                CancableSleep(TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(1));

                if (TryCancel()) return;

                status = getBackupProcessUpdate(CurrentBackupUpdateID);
            }

            // Now ready to upload !
        }

        private void backworkLock_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbStatusProgress.Value = e.ProgressPercentage;
        }

        private void backworkLock_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (currentCancelled) // From TryCancel()
            {
                backupRunning = false;
                Log("Aborting unlocking to server. (in step 2)");
                UpdateProgress(Status: "Step 2/4: Aborted unlocking", Desc: " ", progress: 100);
            }
            else
            {
                // Check for cancel but for now just start lock:
                Log("Step 4/4: Folder was locked.\n Time to unlock: " + Utils.DurationToString(DateTime.Now - startUnlockWaiting));
                backupRunning = false;
                UpdateProgress(Desc: "Finished backup by locking folder", Status: "Backup finished sucessfully", progress: 100);
            }
               
            
        }

        #endregion

    }
}
