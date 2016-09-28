﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrecomputedHashDirDiff.DataSet1TableAdapters;

namespace PrecomputedHashDirDiff
{
    public class DiffDB
    {
        // Comparing two files based loosly on logic from http://www.codeproject.com/Articles/312484/

        // Stats:
        public long AddedFilesSize = 0; 
        public long AddedFilesCount = 0;
        public long AddedFoldersSize = 0;
        public long AddedFoldersCount = 0;

        public long ChangedFilesSize = 0;
        public long ChangedFilesCount = 0;
        

        public long DeletedFilesSize = 0;
        public long DeletedFilesCount = 0;
        public long DeletedFoldersSize = 0;
        public long DeletedFoldersCount = 0;

        public bool useIOLog = false;
        public FileInfo logAddedFiles;
        public FileInfo logAddedFolders;
        public FileInfo logDeletedFiles;
        public FileInfo logDeletedFolders;

        public TimeSpan duration;

        enum KnownDiffKeys : int
        {
            SecondDuration ,

            AddedFilesCount,
            AddedFilesSize,
            ChangedFilesCount,
            ChangedFilesSize,
            DeletedFilesCount,
            DeletedFilesSize,

            AddedFoldersCount,
            AddedFoldersSize,
           
            DeletedFoldersCount,
            DeletedFoldersSize,

        }

        Dictionary<int, long> lastDiffStat = new Dictionary<int, long>();
        DateTime startDiff;

        void calcDictyStat() {
            lastDiffStat.Add((int)KnownDiffKeys.SecondDuration,         (long)duration.TotalSeconds);
            lastDiffStat.Add((int)KnownDiffKeys.AddedFilesCount,        (long)AddedFilesCount);
            lastDiffStat.Add((int)KnownDiffKeys.AddedFilesSize,         (long)AddedFilesSize);
            lastDiffStat.Add((int)KnownDiffKeys.ChangedFilesCount,      (long)ChangedFilesCount);
            lastDiffStat.Add((int)KnownDiffKeys.ChangedFilesSize,       (long)ChangedFilesSize);
            lastDiffStat.Add((int)KnownDiffKeys.DeletedFilesCount,      (long)DeletedFilesCount);
            lastDiffStat.Add((int)KnownDiffKeys.DeletedFilesSize,       (long)DeletedFilesSize);
            lastDiffStat.Add((int)KnownDiffKeys.AddedFoldersCount,      (long)AddedFoldersCount);
            lastDiffStat.Add((int)KnownDiffKeys.AddedFoldersSize,       (long)AddedFoldersSize);
            lastDiffStat.Add((int)KnownDiffKeys.DeletedFoldersCount,    (long)DeletedFoldersCount);
            lastDiffStat.Add((int)KnownDiffKeys.DeletedFoldersSize,     (long)DeletedFoldersSize);
        }

        void IOLog(string text,FileInfo fi) {
            if (!useIOLog) return;
            if (fi != null) {
                try
                {
                   File.AppendAllText(fi.FullName,text + Environment.NewLine);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[ERROR] with IO log file: " + fi.FullName + "\n" + ex.Message);
                }
            }
        }

        public bool Init(string backupDB, string targetDB) {
            // Make generic objects based on path

            GenericFolder backupRootDir = null;
            GenericFolder targetRootDir = null;

            if (backupDB.EndsWith(".db3") && File.Exists(backupDB))
            {
                backupRootDir = GenericTools.FolderObject(0, "data source=\"" + backupDB + "\"");
            }

            if (targetDB.EndsWith(".db3") && File.Exists(targetDB))
            {
                targetRootDir = GenericTools.FolderObject(0, "data source=\"" + targetDB + "\"");
            }
            
            if (backupRootDir == null && Directory.Exists(backupDB)) {
                backupRootDir = GenericTools.FolderObject(new DirectoryInfo(backupDB));
            }

            if (targetRootDir == null && Directory.Exists(targetDB))
            {
                targetRootDir = GenericTools.FolderObject(new DirectoryInfo(targetDB));
            }

            if (backupRootDir == null) {
                Console.WriteLine("Cant find db3 or folder for backup:\n\t" + backupDB);
            }

            if (targetRootDir == null)
            {
                Console.WriteLine("Cant find db3 or folder for target:\n\t" + targetDB);
            }

            if (backupRootDir != null && targetRootDir != null)
            {
                // Start recursion:
                lastDiffStat = new Dictionary<int, long>();

                startDiff = DateTime.Now;
                compareDirs(backupRootDir, targetRootDir);
                duration = DateTime.Now - startDiff;

                calcDictyStat();

            }

            // Assume error:
            return false;
        }


        internal class QueuedCompare {
            public GenericFolder backup;
            public GenericFolder target;
        }

        void compareDirs(GenericFolder backupDir, GenericFolder targetDir) {
            // Note: bahind the scene, list is ordered by name, this is important and we can 
            //      compare them in a linear time as explained here: "Merging two lists" https://en.wikipedia.org/wiki/Merge_algorithm

            // Print to screen what folders are being compared:
            Console.WriteLine("Comparing [" + backupDir.Name() + "] And [" + targetDir.Name() + "] :");

            // Comaparing Files:
            // -----------------------------------------------------------------------------

            Console.WriteLine("Comparing files:");

            // Getting ordered file list.
            List<GenericFile> backupFiles = backupDir.OrderedFiles();
            List<GenericFile> targetFiles = targetDir.OrderedFiles();

            int backupIndx = 0;
            int targetIndx = 0;

            while (backupIndx < backupFiles.Count && targetIndx < targetFiles.Count ) {
                int comp = backupFiles[backupIndx].Name().CompareTo(targetFiles[targetIndx].Name());

                if (comp < 0 ) {
                    // backup is smaller than target
                    Console.WriteLine("\t1. Deleted file: [" + backupFiles[backupIndx].Name() + "]");

                    // Stats:
                    DeletedFilesCount++;
                    DeletedFilesSize += backupFiles[backupIndx].Size();

                    //Log
                    IOLog("[DEL]:" + backupFiles[backupIndx].FullRelativeName(), logDeletedFiles);

                    backupIndx++;
                }

                if (comp > 0)
                {
                    // backup is bigger than target
                    Console.WriteLine("\t2. Added file: [" + targetFiles[targetIndx].Name() + "]");

                    // Stats:
                    AddedFilesCount++;
                    AddedFilesSize += targetFiles[targetIndx].Size();

                    //Log
                    IOLog("[ADD]:" + targetFiles[targetIndx].FullRelativeName(), logAddedFiles);

                    targetIndx++;
                }

                if (comp == 0 ) {
                    // Same file, check checksum:
                    if (backupFiles[backupIndx].Hash() !=  targetFiles[targetIndx].Hash() ) {
                        Console.WriteLine("\t3. Changed file: [" + backupFiles[backupIndx].Name() + "]");

                        // Stats ( Changed, delete old and add new )
                        ChangedFilesCount++;

                        DeletedFilesCount++;
                        DeletedFilesSize += backupFiles[backupIndx].Size();
                        ChangedFilesSize -= backupFiles[backupIndx].Size();

                        AddedFilesCount++;
                        AddedFilesSize += targetFiles[targetIndx].Size();
                        ChangedFilesSize += targetFiles[targetIndx].Size();

                        //Log
                        IOLog("[DEL]:" + backupFiles[backupIndx].FullRelativeName(), logDeletedFiles);
                        IOLog("[ADD]:" + targetFiles[targetIndx].FullRelativeName(), logAddedFiles);
                    }
                    targetIndx++;
                    backupIndx++;
                }
            }

            if (backupIndx < backupFiles.Count  && targetIndx == targetFiles.Count )
            {
                // Files that only in backup is deleted:
                while(backupIndx < backupFiles.Count)
                {
                    Console.WriteLine("\t4. Deleted file: [" + backupFiles[backupIndx].Name() + "]");

                    // Stats:
                    DeletedFilesCount++;
                    DeletedFilesSize += backupFiles[backupIndx].Size();

                    //Log
                    IOLog("[DEL]:" + backupFiles[backupIndx].FullRelativeName(), logDeletedFiles);

                    backupIndx++;
                }
            }

            if (backupIndx == backupFiles.Count  && targetIndx < targetFiles.Count ) {
                // Files that only in target is added:
                while (targetIndx < targetFiles.Count) 
                { 
                    Console.WriteLine("\t5. Added file: [" + targetFiles[targetIndx].Name() + "]");

                    // Stats:
                    AddedFilesCount++;
                    AddedFilesSize += targetFiles[targetIndx].Size();

                    //Log
                    IOLog("[ADD]:" + targetFiles[targetIndx].FullRelativeName(), logAddedFiles);

                    targetIndx++;
                }
                
            }

            // Free memory:
            backupFiles.Clear();
            targetFiles.Clear();

            // Comaparing Folders:
            // -----------------------------------------------------------------------------

            Console.WriteLine("Comparing Folders:");

            // Getting ordered file list.
            List<GenericFolder> backupFolders = backupDir.OrderedFolders();
            List<GenericFolder> targetFolders = targetDir.OrderedFolders();

            backupIndx = 0;
            targetIndx = 0;

            Queue<QueuedCompare> folderQ = new Queue<QueuedCompare>();

            while (backupIndx < backupFolders.Count && targetIndx < targetFolders.Count)
            {
                int comp = backupFolders[backupIndx].Name().CompareTo(targetFolders[targetIndx].Name());

                if (comp < 0)
                {
                    // backup is smaller than target
                    Console.WriteLine("\t1. Deleted Folder: [" + backupFolders[backupIndx].Name() + "]");

                    // Stats:
                    DeletedFoldersCount++;
                    DeletedFoldersSize += backupFolders[backupIndx].Size();

                    //Log
                    IOLog("[DEL]:" + backupFolders[backupIndx].FullRelativeName(), logDeletedFolders);

                    backupIndx++;
                }

                if (comp > 0)
                {
                    // backup is bigger than target
                    Console.WriteLine("\t2. Added Folder: [" + targetFolders[targetIndx].Name() + "]");

                    // Stats:
                    AddedFoldersCount++;
                    AddedFoldersSize += targetFolders[targetIndx].Size();

                    //Log
                    IOLog("[ADD]:" + targetFolders[targetIndx].FullRelativeName(), logAddedFolders);

                    targetIndx++;
                }

                if (comp == 0)
                {
                    // Add folder for later comparison....   
                    folderQ.Enqueue(new QueuedCompare() { 
                        backup = backupFolders[backupIndx], target = targetFolders[targetIndx] 
                    });

                    targetIndx++;
                    backupIndx++;
                }
            }

            if (backupIndx < backupFolders.Count && targetIndx == targetFolders.Count)
            {
                // Folders that only in backup is deleted:
                while (backupIndx < backupFolders.Count)
                {
                    Console.WriteLine("\t3. Deleted Folder: [" + backupFolders[backupIndx].Name() + "]");

                    // Stats:
                    DeletedFoldersCount++;
                    DeletedFoldersSize += backupFolders[backupIndx].Size();

                    //Log
                    IOLog("[DEL]:" + backupFolders[backupIndx].FullRelativeName(), logDeletedFolders);

                    backupIndx++;
                }
            }

            if (backupIndx == backupFolders.Count  && targetIndx < targetFolders.Count )
            {
                // Folders that only in target is added:
                while (targetIndx < targetFolders.Count)
                {
                    Console.WriteLine("\t4. Added Folder: [" + targetFolders[targetIndx].Name() + "]");

                    // Stats:
                    AddedFoldersCount++;
                    AddedFoldersSize += targetFolders[targetIndx].Size();

                    //Log
                    IOLog("[ADD]:" + targetFolders[targetIndx].FullRelativeName(), logAddedFolders);

                    targetIndx++;
                }
            }

            // Free memory:
            backupFolders.Clear();
            targetFolders.Clear();

            // Comaparing Folders deeper:
            // -----------------------------------------------------------------------------

            while (folderQ.Count > 0 ) {
                QueuedCompare q = folderQ.Dequeue();
                compareDirs(q.backup, q.target);
            }
        }

    }
}
