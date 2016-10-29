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
    public partial class Form1 : Form
    {
        #region ServerStatus

        [DataContract]
        public class BackupProcessStatus
        {
            // Integers:

            [DataMember]
            public int id;
            
            [DataMember]
            public int addedbytes;

            [DataMember]
            public int statuscode;

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
            string pathToDownload = txtBackupApiURL + "/status.php?id=" + updateId.ToString();
            pathToDownload = pathToDownload.Replace(@"//", @"/");

            string result = (new WebClient()).DownloadString(pathToDownload);

            Log("Status downloaded from server:\n" + JsonPrettyPrinterPlus.PrettyPrinterExtensions.PrettyPrintJson(result));

            return FromJson<BackupProcessStatus>(result);
        }

        // Json for locking\unlocking 
        [DataContract]
        public class BackupActionResult
        {
            [DataMember]
            public int updateid;
            
            [DataMember]
            public string jobstatus;
            
        }

        private BackupActionResult serverunlock(int userid)
        {
            string pathToDownload = txtBackupApiURL + "/unlock.php?id=" + userid.ToString();
            pathToDownload = pathToDownload.Replace(@"//", @"/");

            string result = (new WebClient()).DownloadString(pathToDownload);

            Log("Unlock server result:\n" + JsonPrettyPrinterPlus.PrettyPrinterExtensions.PrettyPrintJson(result));

            return FromJson<BackupActionResult>(result);
        }

        private BackupActionResult serverlock(int userid, int updateId, int backupsize = 0) 
        {
            string pathToDownload = txtBackupApiURL + "/lock.php?userid=" + userid.ToString() + "&id=" + updateId + "&bytes=" + backupsize;
            pathToDownload = pathToDownload.Replace(@"//", @"/");

            string result = (new WebClient()).DownloadString(pathToDownload);

            Log("Lock server result:\n" + JsonPrettyPrinterPlus.PrettyPrinterExtensions.PrettyPrintJson(result));

            return FromJson<BackupActionResult>(result);
        }

        #endregion

        #region Backup Step 2 - Unlock




        private void backworkUnlock_DoWork(object sender, DoWorkEventArgs e)
        {
            //Unlock using remote url
            Log("Step 2/4: Asking server to unlock.");
            

            // Wait until we get a response that 
            // Of course, stop if we got a cancel and try to "cancel" (url should support 3 types: unlock, lock, cancel)

        }

        private void backworkUnlock_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbStatusProgress.Value = e.ProgressPercentage;
        }

        private void backworkUnlock_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Check for cancel but for now just start uplaod:
            Log("Step 2/4: Folder was unlocked.");
            backworkUploadFiles.RunWorkerAsync();
        }

        #endregion
    }
}
