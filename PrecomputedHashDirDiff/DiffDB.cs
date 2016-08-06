using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrecomputedHashDirDiff.DataSet1TableAdapters;

namespace PrecomputedHashDirDiff
{
    class DiffDB
    {
        // Comparing two files based on logic from http://www.codeproject.com/Articles/312484/

        /******************************************************************

        * Start with the left folder. Get a list of all files and folders in the root directory
        * Loop through the items one by one
        
        [FILE]
        * If it's a file, call the method to process it. This method goes like
           + For the current file see if a corresponding file exists in the other side
           + If a file exits, compare the 2 files, create an entry and add it to a list that tracks comparisons (discussion   about    the compare operation follows)
           + If there is no match, create an entry to indicate this and add it to a list that tracks comparisons

        [FOLDER]
        * If it's a folder, call a method which is a recusrive method to get comparison information about more files /    folders    within the current folder
           + For files within the folder, it's processed as decribed above
           + For folders, the recursive process described here is followed
           + If any folder is an empty folder, an entry is added even for this case. This is indicated by folder icon to the      extreme  left (show in the image in the next section, more discussion about this follows)
        
        * Once the folder comparison with respect to the right folder starts I just have to deal with files/folders that  exists   only on the right

        *******************************************************************/

        FilesTableAdapter backup_adptFiles = new FilesTableAdapter();
        FoldersTableAdapter backup_adptFolders = new FoldersTableAdapter();

        FilesTableAdapter   target_adptFiles = new FilesTableAdapter();
        FoldersTableAdapter target_adptFolders = new FoldersTableAdapter();


        // Stats:
        long AddedFileSize = 0; 
        long AddedFilesCount = 0;
        long AddedFoldersCount = 0;

        long ChangedFileSize = 0;
        long ChangedFilesCount = 0;
        long ChangedFoldersCount = 0;

        long DeletedFileSize = 0;
        long DeletedFilesCount = 0;
        long DeletedFoldersCount = 0;

        public void Init(string backupDB, string targetDB) {
            // Make adapters for both dbs

            if (!File.Exists(backupDB))
            {
                Console.WriteLine("Backup db not found.");
                return;
            }

            if (!File.Exists(targetDB))
            {
                Console.WriteLine("Target db not found.");
                return;
            }

            backup_adptFiles.Connection.ConnectionString = "data source=\"" + backupDB + "\"";         
            backup_adptFolders.Connection.ConnectionString = "data source=\"" + backupDB + "\"";

            target_adptFiles.Connection.ConnectionString = "data source=\"" +   targetDB + "\"";
            target_adptFolders.Connection.ConnectionString = "data source=\"" + targetDB + "\"";

            // Start recursion:
            compareDirs(0, 0);
        }


        string getFolderName(int id, FoldersTableAdapter adp) {
            string name = "(Not Found)";
            DataSet1.FoldersDataTable dt = adp.GetFolderById(id);
            if (dt.Count > 0) {
                name = dt[0].FolderName;
            }
            return name;
        }

        void compareDirs(int backupID, int targetID) {
            // Note: bahind the scene, the sql is ordering by name, this is important and we can 
            //      compare them in a linear time as explained here: "Merging two lists" https://en.wikipedia.org/wiki/Merge_algorithm

            // Print to screen what folders are being compared:

            // Get backup dir and files:
            DataSet1.FoldersDataTable backupFolders = backup_adptFolders.GetFoldersByParentFolderId(backupID);
            DataSet1.FilesDataTable backupFiles = backup_adptFiles.GetFilesByFolderID(backupID);

            // Get target dir and files:
            DataSet1.FoldersDataTable targetFolders = target_adptFolders.GetFoldersByParentFolderId(targetID);
            DataSet1.FilesDataTable targetFiles = target_adptFiles.GetFilesByFolderID(targetID);

            
        }

    }
}
