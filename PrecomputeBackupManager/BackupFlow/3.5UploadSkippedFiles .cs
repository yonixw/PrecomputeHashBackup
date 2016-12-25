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

        string skippedLogFilePathAfterCopy = Path.Combine(
               Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
               @"Precompute Backup Manager" + Path.DirectorySeparatorChar + "skipped_" +
               DateTime.Now.ToString("dd_MM_yyyy", CultureInfo.InvariantCulture)
               + ".txt");

        private bool copySkippedFiles() {
            if (!File.Exists(skippedNamesFilePath)) return false;

            // Copy file
            File.Copy(skippedNamesFilePath, skippedLogFilePathAfterCopy);

            return true;
        }
    
        private void backgroundUploadSkipped_DoWork(object sender, DoWorkEventArgs e)
        {
            if (!cbStep3.Checked) return; // Skip step - use the same as upload

            // Copy skipedd list to other file to enable skip file logging again.
            if (!copySkippedFiles())
            {
                Log("Step 3.5/4: Did not found skipped list. going to next step.");
                return;
            }

            Log("Step 3.5/4: Starting to upload skipped files");
            UpdateProgress(Status: "Uploading skipped files:", progress: 0);

            // MUST BE INVOKED, otherwise the tick callback wont be called.
            this.Invoke(new Action(() => { tmrUploadProgress.Enabled = true; }));
            if (TryCancel()) return;

            string skip_source, skip_dest;
            using (StreamReader txtR = File.OpenText(skippedLogFilePathAfterCopy)) {
                // Read line return null when file ends.
                skip_source = txtR.ReadLine();
                skip_dest = txtR.ReadLine();

                while (skip_source != null && skip_dest != null) {
                    if (TryCancel()) return;

                    // Try to copy skipped file:
                    CopyFileWithProgress(skip_source, skip_dest);


                    skip_source = txtR.ReadLine();
                    skip_dest = txtR.ReadLine();
                }
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
                
                Log("Step 3.5/4: Finished uploading skipped files.");
                // TODO: On cancel send "cancel to the server" (using user modal form);
                backworkLock.RunWorkerAsync();
            }
        }

        #endregion


        

    }
}
