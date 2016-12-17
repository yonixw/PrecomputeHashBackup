using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using PrecomputedHashDirDiff.DataSet1TableAdapters;
using System.Windows.Forms;
using System.ComponentModel;
using System.Web.Script.Serialization;

namespace PrecomputedHashDirDiff
{
    public class PrecomputeHash
    {

        public int FolderIdCounter = 0;
        public int FileIdCounter = 0;
        public long totalFilesSize = 0;

        MultiInsertSQLite multisql;
        
        public void Init(string DirectoryPath, BackgroundWorker bwCalcHash) {
            // To make sure each db ids start from 0.
            FolderIdCounter = 0;
            FileIdCounter = 0;
            totalFilesSize = 0;

            if (!Directory.Exists(DirectoryPath))
            {
                Console.WriteLine("Source folder not found.");
                return;
            }

            string dbPath = Environment.CurrentDirectory;
            File.Copy(dbPath + "\\template.db3", dbPath + "\\dirHash.db3", true);

            FilesTableAdapter _adptFiles = new FilesTableAdapter();
            _adptFiles.Connection.ConnectionString = "data source=\"" + dbPath + "\\dirHash.db3" + "\"";

            FoldersTableAdapter _adptFolders = new FoldersTableAdapter();
            _adptFolders.Connection.ConnectionString = "data source=\"" + dbPath + "\\dirHash.db3" + "\"";

            DirectoryInfo sourceDi = new DirectoryInfo(DirectoryPath);

            multisql = new MultiInsertSQLite(_adptFiles, _adptFolders, 499);

            // Start Recurtion:
            HashFiles(sourceDi, -1, _adptFiles, _adptFolders, bwCalcHash);

            // Flush any rows in cache:
            multisql.FlushAll();
        }

        public void InitGeneric(string DirectoryPath, string DBpath, BackgroundWorker bwCalcHash)
        {
            // To make sure each db ids start from 0.
            FolderIdCounter = 0;
            FileIdCounter = 0;
            totalFilesSize = 0;

            if (!Directory.Exists(DirectoryPath))
            {
                Console.WriteLine("Source folder not found.");
                return;
            }

            using (FilesTableAdapter _adptFiles = new FilesTableAdapter())
            {
                _adptFiles.Connection.ConnectionString = "data source=\"" + DBpath + "\"";

                using (FoldersTableAdapter _adptFolders = new FoldersTableAdapter())
                {
                    _adptFolders.Connection.ConnectionString = "data source=\"" + DBpath + "\"";

                    DirectoryInfo sourceDi = new DirectoryInfo(DirectoryPath);

                    multisql = new MultiInsertSQLite(_adptFiles, _adptFolders, 499);

                    // Start Recurtion:
                    HashFiles(sourceDi, -1, _adptFiles, _adptFolders, bwCalcHash);

                    // Flush any rows in cache:
                    multisql.FlushAll();

                    _adptFiles.Connection.Close();
                    _adptFolders.Connection.Close();

                    // Need these 2 to relese after use: (bug, read SO?8511901)
                    System.Data.SQLite.SQLiteConnection.ClearAllPools();
                    GC.Collect();

                    Console.WriteLine("Released db3.");
                }

            }

    



        }

        public class JSONFileAttribute {
            public class FileTimesInfo {
                public DateTime creation;
                public DateTime lastAccess;
                public DateTime lastWrite;
            }

            public FileTimesInfo times;
        }
        static JavaScriptSerializer json = new JavaScriptSerializer();


        const int progresscols = 30;

        public long HashFiles(DirectoryInfo di, int ParentFolderId, FilesTableAdapter aFiles, FoldersTableAdapter aFolders, BackgroundWorker bwCalcHash)
        {
            long myFolderSize = 0;
            int myFolderId = FolderIdCounter++;

            Console.WriteLine("[Directory] (" + myFolderId.ToString() + ") " + di.FullName);

            foreach (FileInfo fi in Utils.safeGet_Files(di))
            {
                Console.Write("\t[File] (" + FileIdCounter + ") " + fi.Name + "... [");
                bwCalcHash.ReportProgress(0, fi.Name);

                // Compute Hash
                SHA256 hash256 = SHA256.Create();
                byte[] buffer = new byte[1024];

                using (FileStream filestream = fi.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) // SO? 9759697
                {
                    long size = filestream.Length;
                    long bytesRead = 0;
                    long percent = size / progresscols;
                    if (percent == 0) percent = 1; // for files under 100B

                    int currentPercent = 0;

                    while (bytesRead < size)
                    {
                        if (bwCalcHash.CancellationPending) return 0;

                        int lastBytesRead = filestream.Read(buffer, 0, buffer.Length);
                        hash256.TransformBlock(buffer, 0, lastBytesRead, buffer, 0);
                        bytesRead += lastBytesRead;

                        if (bytesRead > percent * currentPercent && currentPercent < progresscols)
                        {
                            Console.Write('#');
                            currentPercent++;
                            bwCalcHash.ReportProgress((100 * currentPercent) / progresscols);
                        }
                    }

                    hash256.TransformFinalBlock(buffer, 0, 0);
                    string finalHash = BitConverter.ToString(hash256.Hash).Replace("-", "");
                    Console.WriteLine("] =>" + finalHash.Substring(0, 10));

                    totalFilesSize += size;
                    myFolderSize += size;


                    JSONFileAttribute attributes = new JSONFileAttribute();
                    attributes.times = new JSONFileAttribute.FileTimesInfo()
                    {
                        creation = fi.CreationTime,
                        lastAccess = fi.LastAccessTime,
                        lastWrite = fi.LastWriteTime
                    };



                    //aFiles.NewFile(fi.Name, finalHash, myFolderId, FileIdCounter++, size);
                    multisql.AddFileRow(fi.Name, finalHash, myFolderId, FileIdCounter++, size,json.Serialize(attributes));
                }


            }

            foreach (DirectoryInfo childdi in Utils.safeGet_Directories(di))
            {
               myFolderSize += HashFiles(childdi, myFolderId, aFiles, aFolders,bwCalcHash);
                if (bwCalcHash.CancellationPending) return 0;
            }

            if (bwCalcHash.CancellationPending) return 0;

            // After finding my size add it:
            //      Note: this method will make deeper folders have lower ids (the root having the highest id)
            multisql.AddFolderRow(di.Name, ParentFolderId, myFolderId,myFolderSize);

            return myFolderSize;
        }

       
    }
}
