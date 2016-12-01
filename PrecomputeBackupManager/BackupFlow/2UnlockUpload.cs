using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PrecomputeBackupManager.DataSet1TableAdapters;
using PrecomputeBackupManager.HashFileDatasetTableAdapters;

namespace PrecomputeBackupManager
{
    public partial class frmMain : Form
    {
        #region ServerStatus

        [DataContract]
        public class BackupProcessStatus
        {
            // Integers:

            [DataMember]
            public long? id;

            [DataMember]
            public long? addedbytes;

            [DataMember]
            public int? statuscode;

            // Strings:

            [DataMember]
            public string username;

            [DataMember]
            public string start;

            [DataMember]
            public string end;

            [DataMember]
            public string statusdescription;
        }

        private static string ToJson<T>(T data)
        {
            // SO ? 1178255

            DataContractJsonSerializer serializer
                        = new DataContractJsonSerializer(typeof(T));

            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, data);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        private static T FromJson<T>(string json)
        {
            // https://blog.udemy.com/json-serializer-c-sharp/

            DataContractJsonSerializer serializer
                        = new DataContractJsonSerializer(typeof(T));

            using (MemoryStream ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json)))
            {
                return (T)serializer.ReadObject(ms);
            }
        }

        private BackupProcessStatus getBackupProcessUpdate(int updateId)
        {
            try
            {
                string pathToDownload = txtBackupApiURL.Text + "/status.php?id=" + updateId.ToString();
                pathToDownload = pathToDownload.Replace(@"//status.php", @"/status.php");

                string result = (new WebClient()).DownloadString(pathToDownload);

                Log("Status downloaded from server:\n " + JsonPrettyPrinterPlus.PrettyPrinterExtensions.PrettyPrintJson(result));

                return FromJson<BackupProcessStatus>(result);
            }
            catch (Exception ex)
            {
                Log(ex);
                return null;
            }
        }

        // Json for locking\unlocking 
        [DataContract]
        public class BackupActionResult
        {
            [DataMember]
            public int? updateid;

            [DataMember]
            public string jobstatus;

        }

        private BackupActionResult serverunlock(int userid)
        {
            try
            {
                string pathToDownload = txtBackupApiURL.Text + "/unlock.php?id=" + userid.ToString();
                pathToDownload = pathToDownload.Replace(@"//unlock.php", @"/unlock.php");

                string result = (new WebClient()).DownloadString(pathToDownload);

                Log("Unlock server result:\n " + JsonPrettyPrinterPlus.PrettyPrinterExtensions.PrettyPrintJson(result));

                return FromJson<BackupActionResult>(result);
            }
            catch (Exception ex)
            {
                Log(ex);
                return null;
            }
        }

        private BackupActionResult serverlock(int userid, int updateId, long backupsize = 0)
        {
            try
            {
                string pathToDownload = txtBackupApiURL.Text + "/lock.php?userid=" + userid.ToString() + "&id=" + updateId + "&bytes=" + backupsize;
                pathToDownload = pathToDownload.Replace(@"//lock.php", @"/lock.php");

                string result = (new WebClient()).DownloadString(pathToDownload);

                Log("Lock server result:\n " + JsonPrettyPrinterPlus.PrettyPrinterExtensions.PrettyPrintJson(result));

                return FromJson<BackupActionResult>(result);
            }
            catch (Exception ex)
            {
                Log(ex);
                return null;
            }
        }

        #endregion

        #region Backup Step 2 - Unlock


        DateTime startUnlockWaiting;
        int CurrentBackupUpdateID = -1;

        private void backworkUnlock_DoWork(object sender, DoWorkEventArgs e)
        {
            if (!cbStep2.Checked) return; // Skip step

            int tryCount = 0;

            //Unlock using remote url
            Log("Step 2/4: Asking server to unlock.");
            UpdateProgress(Status: "Step 2.1: Waiting for unlock", Desc: "Sending request to server",  progress: 0);

            // If no folder need to be uploaded just abort backup:
            bool needUpload = false;
            foreach (KeyValuePair<string, BackupDirectoryInfo> currentFolder in _FoldersToBackup)
            {
                if (currentFolder.Value.isUploadNeeded()) {
                    needUpload = true;
                    break;
                }
            }
            if (!needUpload) {
                currentCancelled = true;
                Log("Backup aborted because no changes were found");
                return;
            }


            startUnlockWaiting = DateTime.Now;

            if (TryCancel()) return;

            // Wait until we get a response that 
            // Of course, stop if we got a cancel and try to "cancel" (url should support 3 types: unlock, lock, cancel)
            BackupActionResult actionResult = serverunlock(int.Parse(txtUsernameCode.Text));
            if (actionResult == null || actionResult.updateid < 0) {
                currentCancelled = true;
                Log("Error while unlocking. is result Null? " + (actionResult == null).ToString());
                return;
            }

            // Set the current id of update for the rest of the process:
            CurrentBackupUpdateID = actionResult.updateid ?? -1;

            if (TryCancel()) return;

            // Now check satus until ready (2)
            // Status code 1 is still unlocking, status code 2 is unlocked and ready to go!

            BackupProcessStatus status = getBackupProcessUpdate(CurrentBackupUpdateID);
            while(status != null && status.statuscode < 2) {
                tryCount++;
                UpdateProgress(Desc: "Try number: " + tryCount.ToString());
                CancableSleep(TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(1));

                if (TryCancel()) return;

                status = getBackupProcessUpdate(CurrentBackupUpdateID);
            }
            
            // Now ready to upload !
        }

        private void CancableSleep(TimeSpan Total, TimeSpan Steps) {
            // Sleep `total` time but check every `step` if cancelled
            double total = Total.TotalMilliseconds;
            double step = Steps.TotalMilliseconds;

            while (total > step) {
                System.Threading.Thread.Sleep((int)step);
                if (TryCancel()) return;
                total -= step;
            }

        }

        private void backworkUnlock_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbStatusProgress.Value = e.ProgressPercentage;
        }

        private void backworkUnlock_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (currentCancelled) // From TryCancel()
            {
                backupRunning = false;
                Log("Aborting unlocking to server. (in step 2)");
                UpdateProgress(Status: "Step 2/4: Aborted unlocking", Desc: " ", progress: 100);
            }
            else
            {
                Log("Step 2/4: Folder was unlocked.\n Time to unlock: " + Utils.DurationToString(DateTime.Now - startUnlockWaiting));
                backworkUploadFiles.RunWorkerAsync();
            }
        }
        #endregion
    }
}
