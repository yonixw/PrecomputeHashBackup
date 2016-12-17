using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using PrecomputedHashDirDiff.DataSetHashDBTableAdapters;

namespace PrecomputedHashDirDiff
{
    // This file contains classes that can be fed from sqlite or IO.
    // This way we give the user a full coverage over any scenerio like:
    //      * Compare source IO dir with sql3 dir.
    //      * Compare sql3 dir with sql3 dir 
    //      etc... All using the same objects!


    /*************************************
    *
    *       Create generic object
    *
    **************************************/

    public class GenericTools {
        public static GenericFile FileObject(FileInfo fi) {
            return new IOFile(fi);
        }

        public static GenericFile FileObject(DataSetHashDB.FilesRow fr, string ConnectionString, string DirectoryName = "")
        {
            // In addition to the row describing the file,
            //      to be aware of other files\folders "in the filesystem" 
            //      we need to know the database this file came from.
            return new SQLite3File(fr,ConnectionString, DirectoryName);
        }

        public static GenericFolder FolderObject(DirectoryInfo di) {
            return new IOFolder(di);
        }

        public static GenericFolder FolderObject(DataSetHashDB.FoldersRow fr, string ConnectionString, string ParentDirectoryName = "") {
            // In addition to the row describing the folder,
            //      to be aware of other files\folders "in the filesystem" 
            //      we need to know the database this file came from.
            return new SQLite3Folder(fr, ConnectionString, ParentDirectoryName);
        }

        public static GenericFolder FolderObject(int FolderID, string ConnectionString, out long FilesCount, out long FoldersCount) {
            FoldersTableAdapter adaptFolders = new FoldersTableAdapter();
            adaptFolders.Connection.ConnectionString = ConnectionString;
            FoldersCount = System.Convert.ToInt64(adaptFolders.GetFoldersCount());

            FilesTableAdapter adaptFiles = new FilesTableAdapter();
            adaptFiles.Connection.ConnectionString = ConnectionString;
            FilesCount = System.Convert.ToInt64(adaptFiles.GetFilesCount());

            DataSetHashDB.FoldersDataTable dt = adaptFolders.GetFolderById(FolderID);
            if (dt.Count > 0 ) {
                return GenericTools.FolderObject(dt[0], ConnectionString); // No parent for specific id
            }
            else
            {
                return null;
            }
        }
    }


    /*************************************
    *
    *      FILE OBJECT 
    *
    **************************************/


    public interface GenericFile {
        string Name();
        string Hash();
        GenericFolder Folder();
        long Size();
        bool Skipped();

        string FullRelativeName();
    }

    public class IOFile : GenericFile
    {
        FileInfo _file;
        public IOFile(FileInfo fi) {
            _file = fi;
        }

        public GenericFolder Folder()
        {
            return GenericTools.FolderObject(_file.Directory);
        }

        string finalHash = "";
        public string Hash()
        {
            // Check if was already calculated
            if (finalHash != "")
                return finalHash;

            // O.W. Compute Hash
            SHA256 hash256 = SHA256.Create();
            byte[] buffer = new byte[1024];
            

            Console.Write("\n[FILE-HASH] " + _file.Name + "\n Progress: [");

            using (FileStream filestream = _file.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) // SO? 9759697
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
                        Console.Write("#");
                    }
                }

                hash256.TransformFinalBlock(buffer, 0, 0);
                Console.Write("]."); // End Progress

                finalHash = BitConverter.ToString(hash256.Hash).Replace("-", "");
                Console.WriteLine(finalHash.Substring(0, 10));
                
            }

            return finalHash;
        }

        public string Name()
        {
            return _file.Name;
        }

        public long Size()
        {
            return _file.Length;
        }

        public string FullRelativeName()
        {
            return _file.Directory.FullName;
        }

        public bool Skipped()
        {
            return false;
        }
    }

    [DebuggerDisplay("{_file.FileName}")]
    public class SQLite3File : GenericFile
    {
        DataSetHashDB.FilesRow _file;
        string _conn;
        string _DirectoryName;

        public SQLite3File(DataSetHashDB.FilesRow fr, string ConnectionString, string DirectoryName) {
            _file = fr;
            _conn = ConnectionString;
            _DirectoryName = DirectoryName;
        }

        public GenericFolder Folder()
        {
            FoldersTableAdapter folders = new FoldersTableAdapter();
            folders.Connection.ConnectionString = _conn;

            DataSetHashDB.FoldersDataTable dt = folders.GetFolderById(_file.FolderParentID);
            if (dt.Count > 0) {
                return GenericTools.FolderObject(dt[0],_conn);
            }
            else
            {
                Console.Write("Cant find parent folder for file: (" + _file.FileId + ") " + _file.FileName);
                return null;
            }
        }

        public string Hash()
        {
            return _file.FileHash;
        }

        public string Name()
        {
            return Utils.strFrom64(_file.FileName);
        }

        public long Size()
        {
            return _file.FileSize;
        }

        public string FullRelativeName()
        {
            return _DirectoryName + '\\' +  Name();
        }

        public bool Skipped()
        {
            return _file.Skipped;
        }
    }


    /*************************************
    *
    *       FOLDER OBJECT
    *
    **************************************/


    public interface GenericFolder {
        string Name();
        long Size();
        GenericFolder Parent();
        List<GenericFile> OrderedFiles();
        List<GenericFolder> OrderedFolders();

        string FullRelativeName();
    }

    public class IOFolder : GenericFolder
    {
        DirectoryInfo _folder;
        public IOFolder(DirectoryInfo di) {
            _folder = di;
        }
        public string Name()
        {
            return _folder.Name;
        }

        public GenericFolder Parent()
        {
            return GenericTools.FolderObject(_folder.Parent);
        }

        public List<GenericFile> OrderedFiles()
        {
            List<GenericFile> result = new List<GenericFile>();

            var files = Utils.safeGet_Files(_folder).OrderBy(f => f.Name);
            foreach(FileInfo fi in files) {
                result.Add(GenericTools.FileObject(fi));
            }

            return result;
        }

        public List<GenericFolder> OrderedFolders()
        {
            List<GenericFolder> result = new List<GenericFolder>();

            var folders = Utils.safeGet_Directories(_folder).OrderBy(f => f.Name);
            foreach (DirectoryInfo di in folders)
            {
                result.Add(GenericTools.FolderObject(di));
            }

            return result;
        }

        public long Size()
        {
            return 0;
        }

        public string FullRelativeName()
        {
            return _folder.FullName;
        }
    }

    [DebuggerDisplay("{_folder.FolderName}")]
    public class SQLite3Folder : GenericFolder
    {
        DataSetHashDB.FoldersRow _folder;
        string _conn;
        string _ParentDirectoryName;
        public SQLite3Folder(DataSetHashDB.FoldersRow fr, string ConnectionString,string ParentDirectoryName = "")
        {
            _folder = fr;
            _conn = ConnectionString;
            _ParentDirectoryName = ParentDirectoryName;
        }

        public string Name()
        {
            return Utils.strFrom64(_folder.FolderName);
        }

        public GenericFolder Parent()
        {
            FoldersTableAdapter folders = new FoldersTableAdapter();
            folders.Connection.ConnectionString = _conn;

            DataSetHashDB.FoldersDataTable dt = folders.GetFolderById(_folder.FolderParentID);
            if (dt.Count > 0)
            {
                return GenericTools.FolderObject(dt[0], _conn);
            }
            else
            {
                Console.Write("Cant find parent folder for folder: (" + _folder.FolderId + ") " + _folder.FolderName);
                return null;
            }
        }

        public List<GenericFile> OrderedFiles()
        {
            List<GenericFile> result = new List<GenericFile>();
            FilesTableAdapter adap = new FilesTableAdapter();
            adap.Connection.ConnectionString = _conn;

            var Rows = adap.GetFilesByFolderID(_folder.FolderId).AsEnumerable().OrderBy(f => Utils.strFrom64(f.FileName));

            foreach (DataSetHashDB.FilesRow fr in Rows) 
            {
                result.Add(GenericTools.FileObject(fr, _conn, _ParentDirectoryName + "\\" + Utils.strFrom64(_folder.FolderName) ));
            }

            return result;
        }

        public List<GenericFolder> OrderedFolders()
        {
            List<GenericFolder> result = new List<GenericFolder>();
            FoldersTableAdapter adap = new FoldersTableAdapter();
            adap.Connection.ConnectionString = _conn;

            var Rows = adap.GetFoldersByParentFolderId(_folder.FolderId).AsEnumerable().OrderBy(f => Utils.strFrom64(f.FolderName));

            foreach (DataSetHashDB.FoldersRow fr in Rows)
            {
                result.Add(GenericTools.FolderObject(fr, _conn, _ParentDirectoryName + "\\" + Utils.strFrom64(_folder.FolderName) ));
            }

            return result;
        }

        public long Size()
        {
            return _folder.FolderSize;
        }

        public string FullRelativeName()
        {
            return _ParentDirectoryName + '\\' + Name();
        }
    }
}
