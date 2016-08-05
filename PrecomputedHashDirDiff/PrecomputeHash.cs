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

namespace PrecomputedHashDirDiff
{
    class PrecomputeHash
    {

        public int FolderIdCounter = 0;
        public int FileIdCounter = 0;
        public long totalFilesSize = 0;
        
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

            // Start Recurtion:
            HashFiles(sourceDi, -1, _adptFiles, _adptFolders, bwCalcHash);
        }


        public  void HashFiles(DirectoryInfo di, int ParentFolderId, FilesTableAdapter aFiles, FoldersTableAdapter aFolders, BackgroundWorker bwCalcHash)
        {
            int myFolderId = FolderIdCounter++;

            Console.WriteLine("[Directory] (" + myFolderId.ToString() + ") " + di.FullName);
            aFolders.NewFolder(di.Name, ParentFolderId, myFolderId);

            foreach (FileInfo fi in di.GetFiles())
            {
                Console.Write("\t[File] (" + FileIdCounter + ") " + fi.Name + "... ");

                // Compute Hash
                SHA256 hash256 = SHA256.Create();
                byte[] buffer = new byte[1024];

                using (FileStream filestream = fi.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) // SO? 9759697
                {
                    long size = filestream.Length;
                    long bytesRead = 0;
                    long percent = size / 100;
                    if (percent == 0) percent = 1; // for files under 100B

                    int currentPercent = 0;

                    while (bytesRead < size)
                    {
                        int lastBytesRead = filestream.Read(buffer, 0, buffer.Length);
                        hash256.TransformBlock(buffer, 0, lastBytesRead, buffer, 0);
                        bytesRead += lastBytesRead;

                        if (bytesRead > percent * currentPercent && currentPercent < 100)
                        {
                            currentPercent++;
                            bwCalcHash.ReportProgress(currentPercent);
                        }
                    }

                    hash256.TransformFinalBlock(buffer, 0, 0);
                    string finalHash = BitConverter.ToString(hash256.Hash).Replace("-", "");
                    Console.WriteLine(finalHash.Substring(0, 10));

                    totalFilesSize += size;
                    aFiles.NewFile(fi.Name, finalHash, myFolderId, FileIdCounter++, size);
                }

            }

            foreach (DirectoryInfo childdi in di.GetDirectories())
            {
                HashFiles(childdi, myFolderId, aFiles, aFolders,bwCalcHash);
            }
        }

        public static string byte2hum(long bytes)
        {
            string result = "";

            long KB = 1024;
            long MB = 1024 * KB;
            long GB = 1024 * MB;
            long TB = 1024 * GB;

            long units = 0;

            // TERA
            while (bytes > TB) {
                units++;
                bytes = bytes - TB;
            }

            result += units + " TB, ";
            units = 0;

            // GIGA
            while (bytes > GB)
            {
                units++;
                bytes = bytes - GB;
            }

            result += units + " GB, ";
            units = 0;

            // Mega
            while (bytes > MB)
            {
                units++;
                bytes = bytes - MB;
            }

            result += units + " MB, ";
            units = 0;

            // Kilo
            while (bytes > KB)
            {
                units++;
                bytes = bytes - KB;
            }

            result += units + " KB, ";
            units = 0;

            result += bytes + " Bytes.";
            return result;

            
        }
    }
}
