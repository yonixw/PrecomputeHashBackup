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
