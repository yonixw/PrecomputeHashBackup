using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrecomputedHashDirDiff.DataSet1TableAdapters;

namespace PrecomputedHashDirDiff
{
    class MultiInsertSQLite
    {
        public int _cutoff;
        public FilesTableAdapter adapFiles;
        public FoldersTableAdapter adapFolders;

        public MultiInsertSQLite(
            FilesTableAdapter FilesAdapter,
            FoldersTableAdapter FolderAdapter,
            int cutoff
            ) 
        {
            _cutoff = Math.Min(499, cutoff); //http://www.sqlite.org/limits.html
            adapFiles = FilesAdapter;
            adapFolders = FolderAdapter;
        }

        DataSet1.FilesDataTable localFiles = new DataSet1.FilesDataTable();
        DataSet1.FoldersDataTable localFolders = new DataSet1.FoldersDataTable();


        string strTo64(string input) {
            // SO? 11743160
            return System.Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
        }

        public void AddFileRow(DataSet1.FilesRow fileRow) {
            fileRow.FileName = strTo64(fileRow.FileName);

            localFiles.AddFilesRow(fileRow);

            if (localFiles.Rows.Count > this._cutoff)
                FlushFileData();
        }

        public void AddFileRow(string name, string hash, int folderid, int fileid, long size)
        {
            name = strTo64(name);

            localFiles.AddFilesRow(fileid,name,hash,folderid,size);

            if (localFiles.Rows.Count > this._cutoff)
                FlushFileData();
        }

        

        string FileRowToString(DataSet1.FilesRow fileRow) {
            // No base64 encoding here, done before.

            return
               string.Format("\n('{0}', '{1}', {2}, {3}, {4})",
               fileRow.FileName, fileRow.FileHash, fileRow.FolderParentID, fileRow.FileId, fileRow.FileSize
               )
            ;
        }
     

        public void FlushFileData() {
            Console.WriteLine("\t\tSave [Files] to DB...");

            /*
            SO? 1609637

            insert into myTable (col1,col2) 
                 select aValue as col1,anotherValue as col2 
                 union all select moreValue,evenMoreValue 
                 union all ...
            */
            //https://msdn.microsoft.com/en-us/library/ms233812.aspx;


            string InsertCommand = @"INSERT INTO 'Files'
                         ('FileName', 'FileHash', 'FolderParentID', 'FileId', 'FileSize') VALUES";
                            
            if (localFiles.Rows.Count == 0) {
                return;
            }

            for(int i=0; i< localFiles.Rows.Count -1;i++) {// all except last row
                InsertCommand += FileRowToString(localFiles[i]) + "," ;
            }

            // Last row must be because we checked for not empty....:
            InsertCommand += FileRowToString(localFiles[localFiles.Rows.Count - 1]);

            System.Data.SQLite.SQLiteConnection objConn = adapFiles.Connection;
            System.Data.SQLite.SQLiteCommand cmdInsert = new System.Data.SQLite.SQLiteCommand();
            cmdInsert.CommandType = System.Data.CommandType.Text;
            cmdInsert.CommandText = InsertCommand;
            cmdInsert.Connection = objConn;

            objConn.Open();
            cmdInsert.ExecuteNonQuery(); // Insert is none query.
            objConn.Close();


            // Clear table
            localFiles.Rows.Clear();
        }

        public void AddFolderRow(DataSet1.FoldersRow folderRow)
        {
            folderRow.FolderName = strTo64(folderRow.FolderName);

            localFolders.AddFoldersRow(folderRow);

            if (localFolders.Rows.Count > this._cutoff)
                FlushFolderData();
        }

        public void AddFolderRow(string name, int folderparent, int folderid, long foldersize)
        {
            name = strTo64(name);
            localFolders.AddFoldersRow(folderid, name, folderparent,foldersize);

            if (localFolders.Rows.Count > this._cutoff)
                FlushFolderData();
        }

        string FolderRowToString(DataSet1.FoldersRow folderRow)
        {
            // No base64 encoding here, done before.

            return
               string.Format("\n('{0}', {1}, {2}, {3})",
               folderRow.FolderName, folderRow.FolderParentID, folderRow.FolderId, folderRow.FolderSize
               )
            ;
        }

        public void FlushFolderData()
        {
            Console.WriteLine("\t\tSave [Folders] to DB...");

            string InsertCommand = @"INSERT INTO Folders
                         (FolderName, FolderParentID, FolderId, FolderSize) VALUES";

            if (localFolders.Rows.Count == 0)
            {
                return;
            }

            for (int i = 0; i < localFolders.Rows.Count - 1; i++)
            {// all except last row
                InsertCommand += FolderRowToString(localFolders[i]) + ",";
            }

            // Last row must be because we checked for not empty....:
            InsertCommand += FolderRowToString(localFolders[localFolders.Rows.Count - 1]);

            System.Data.SQLite.SQLiteConnection objConn = adapFiles.Connection;
            System.Data.SQLite.SQLiteCommand cmdInsert = new System.Data.SQLite.SQLiteCommand();
            cmdInsert.CommandType = System.Data.CommandType.Text;
            cmdInsert.CommandText = InsertCommand;
            cmdInsert.Connection = objConn;

            objConn.Open();
            cmdInsert.ExecuteNonQuery(); // Insert is none query.
            objConn.Close();


            //Clear table
            localFolders.Rows.Clear();
        }

        public void FlushAll() {
           
            FlushFileData();
            FlushFolderData();
        }
    }
}
