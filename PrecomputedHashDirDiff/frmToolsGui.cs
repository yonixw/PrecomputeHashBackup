using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SQLite;
using PrecomputedHashDirDiff.DataSet1TableAdapters;
using System.Runtime.InteropServices;
using System.IO;
using System.Security.Cryptography;

namespace PrecomputedHashDirDiff
{
    

    public partial class frmToolsGui : Form
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        public frmToolsGui()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Create console for easy cross-thread printing.
            AllocConsole();

            // Test functions:
            //
           
        }

        /*
        **************** STEP 1 : Precompute hash of changed directory
        */
        DateTime startHash;
        PrecomputeHash firstStep;
        private void btnCreateList_Click(object sender, EventArgs e)
        {
            pbHashStatus.Value = 0;
            startHash = DateTime.Now;
            bwCalcHash.RunWorkerAsync();
        }

        private void bwCalcHash_DoWork(object sender, DoWorkEventArgs e)
        {
            firstStep = new PrecomputeHash();
            firstStep.Init(txtHashSourceDir.Text, bwCalcHash);

        }

        private void bwCalcHash_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            pbHashStatus.Value = 100;
            Console.WriteLine("Compute Background worker: All done!");
            TimeSpan duration = DateTime.Now - startHash;
            Console.WriteLine("Duration: " + 
                String.Format("{0} Days, {1} Hours, {2} Minutes, {3} Seconds.",
                duration.Days, duration.Hours, duration.Minutes, duration.Seconds
                )
            );
            Console.WriteLine("Files: " + firstStep.FileIdCounter + ", Folders: " + firstStep.FolderIdCounter);
            Console.WriteLine("Total files size:" + Utils.byte2hum(firstStep.totalFilesSize));
            Console.WriteLine("\t(" + firstStep.totalFilesSize.ToString() + " Bytes)");
        }

        private void bwCalcHash_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbHashStatus.Value = e.ProgressPercentage;
        }





        /*
        **************** STEP 2 : Comapre db or io with db or io
        */

        DateTime startDiffDB;
        DiffDB secondStep;

        private void btnDiffDB_Click(object sender, EventArgs e)
        {
            pbDiffStatus.Value = 0;
            startDiffDB = DateTime.Now;
            bwDBdiff.RunWorkerAsync();
        }

        private void bwDBdiff_DoWork(object sender, DoWorkEventArgs e)
        {
            secondStep = new DiffDB();
            secondStep.Init(
                txtBackup.Text, 
                txtTartget.Text);
        }

        private void bwDBdiff_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbDiffStatus.Value  = e.ProgressPercentage;
        }

        private void bwDBdiff_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            pbDiffStatus.Value = 100;
            Console.WriteLine("Diff Background worker: All done!");
            TimeSpan duration = DateTime.Now - startDiffDB;

            Console.WriteLine("Duration: " +
                String.Format("{0} Days, {1} Hours, {2} Minutes, {3} Seconds.",
                duration.Days, duration.Hours, duration.Minutes, duration.Seconds
                )
            );

            Console.WriteLine(" * Files:");
            Console.WriteLine("           [COUNT]        [SIZE]");
            Console.WriteLine("  + Added:    "+ secondStep.AddedFilesCount + "       " + Utils.byte2hum(secondStep.AddedFileSize));
            Console.WriteLine("  + Changed:  " + secondStep.ChangedFilesCount + "       XXXX" );
            Console.WriteLine("  + Deleted:  " + secondStep.DeletedFilesCount + "       " + Utils.byte2hum(secondStep.DeletedFileSize));

            Console.WriteLine("* Folders:");
            Console.WriteLine("           [COUNT]");
            Console.WriteLine("  + Added:    " + secondStep.AddedFoldersCount);
            Console.WriteLine("  + Deleted:  " + secondStep.DeletedFoldersCount);
        }
    }
}
