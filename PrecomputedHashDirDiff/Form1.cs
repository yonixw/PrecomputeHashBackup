﻿using System;
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
    

    public partial class Form1 : Form
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Create console for easy cross-thread printing.
            AllocConsole();
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
            Console.WriteLine("Background worker: All done!");
            TimeSpan duration = DateTime.Now - startHash;
            Console.WriteLine("Duration: " + 
                String.Format("{0} Days, {1} Hours, {2} Minutes, {3} Seconds.",
                duration.Days, duration.Hours, duration.Minutes, duration.Seconds
                )
            );
            Console.WriteLine("Files: " + firstStep.FileIdCounter + ", Folders: " + firstStep.FolderIdCounter);
            Console.WriteLine("Total files size:" + PrecomputeHash.byte2hum(firstStep.totalFilesSize));
            Console.WriteLine("\t(" + firstStep.totalFilesSize.ToString() + " Bytes)");
        }

        private void bwCalcHash_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbHashStatus.Value = e.ProgressPercentage;
        }
    
        
    }
}
