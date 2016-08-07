using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using PrecomputedHashDirDiff.DataSet1TableAdapters;

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

        public static GenericFile FileObject(DataSet1.FilesRow fr, string ConnectionString)
        {
            // In addition to the row describing the file,
            //      to be aware of other files\folders "in the filesystem" 
            //      we need to know the database this file came from.
            return new SQLite3File(fr,ConnectionString);
        }

        public static GenericFolder FolderObject(DirectoryInfo di) {
            return new IOFolder(di);
        }

        public static GenericFolder FolderObject(DataSet1.FoldersRow fr, string ConnectionString) {
            // In addition to the row describing the folder,
            //      to be aware of other files\folders "in the filesystem" 
            //      we need to know the database this file came from.
            return new SQLite3Folder(fr, ConnectionString);
        }

        public static GenericFolder FolderObject(int FolderID, string ConnectionString) {
            FoldersTableAdapter adapt = new FoldersTableAdapter();
            adapt.Connection.ConnectionString = ConnectionString;

            DataSet1.FoldersDataTable dt = adapt.GetFolderById(FolderID);
            if (dt.Count > 0 ) {
                return GenericTools.FolderObject(dt[0], ConnectionString);
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
    }

    [DebuggerDisplay("{_file.FileName}")]
    public class SQLite3File : GenericFile
    {
        DataSet1.FilesRow _file;
        string _conn;
        public SQLite3File(DataSet1.FilesRow fr, string ConnectionString) {
            _file = fr;
            _conn = ConnectionString;
        }

        public GenericFolder Folder()
        {
            FoldersTableAdapter folders = new FoldersTableAdapter();
            folders.Connection.ConnectionString = _conn;

            DataSet1.FoldersDataTable dt = folders.GetFolderById(_file.FolderParentID);
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
            return _file.FileName;
        }

        public long Size()
        {
            return _file.FileSize;
        }
    }


    /*************************************
    *
    *       FOLDER OBJECT
    *
    **************************************/


    public interface GenericFolder {
        string Name();
        GenericFolder Parent();
        List<GenericFile> OrderedFiles();
        List<GenericFolder> OrderedFolders();
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

            var files = _folder.GetFiles().OrderBy(f => f.Name);
            foreach(FileInfo fi in files) {
                result.Add(GenericTools.FileObject(fi));
            }

            return result;
        }

        public List<GenericFolder> OrderedFolders()
        {
            List<GenericFolder> result = new List<GenericFolder>();

            var folders = _folder.GetDirectories().OrderBy(f => f.Name);
            foreach (DirectoryInfo di in folders)
            {
                result.Add(GenericTools.FolderObject(di));
            }

            return result;
        }

       
    }

    [DebuggerDisplay("{_folder.FolderName}")]
    public class SQLite3Folder : GenericFolder
    {
        DataSet1.FoldersRow _folder;
        string _conn;
        public SQLite3Folder(DataSet1.FoldersRow fr, string ConnectionString)
        {
            _folder = fr;
            _conn = ConnectionString;
        }

        public string Name()
        {
            return _folder.FolderName;
        }

        public GenericFolder Parent()
        {
            FoldersTableAdapter folders = new FoldersTableAdapter();
            folders.Connection.ConnectionString = _conn;

            DataSet1.FoldersDataTable dt = folders.GetFolderById(_folder.FolderParentID);
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

            var Rows = adap.GetFilesByFolderID(_folder.FolderId).AsEnumerable().OrderBy(f => f.FileName);

            foreach (DataSet1.FilesRow fr in Rows) 
            {
                result.Add(GenericTools.FileObject(fr, _conn));
            }

            return result;
        }

        public List<GenericFolder> OrderedFolders()
        {
            List<GenericFolder> result = new List<GenericFolder>();
            FoldersTableAdapter adap = new FoldersTableAdapter();
            adap.Connection.ConnectionString = _conn;

            var Rows = adap.GetFoldersByParentFolderId(_folder.FolderId).AsEnumerable().OrderBy(f => f.FolderName);

            foreach (DataSet1.FoldersRow fr in Rows)
            {
                result.Add(GenericTools.FolderObject(fr, _conn));
            }

            return result;
        }
    }
}
