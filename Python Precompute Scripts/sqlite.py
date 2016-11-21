import sqlite3
import base64

# Maximum rows per insert command:
multirowscount = 499

connection = None
files = []
folders = []

def connect(path):
    global connection
    connection = sqlite3.connect(path);
    
def close():
    global connection
    connection.close();    

def addFile(name, hash, folderid, fileid, size):
    global files,multirowscount
    name = base64.b64encode(name)
    files.append((name, hash, folderid, fileid, size))
    if len(files) > multirowscount - 1:
        flush()
    
def addFolder(name, parentid, folderid, size):
    global folders,multirowscount
    name = base64.b64encode(name)
    folders.append((name, parentid, folderid, size))
    if len(folders) > multirowscount - 1:
        flush()

def flush():
    global connection, files, folders

    foldersInsert = '''
    INSERT INTO Folders
                         (FolderName, FolderParentID, FolderId, FolderSize)
    VALUES        (?,?,?,?)'''
    
    filesInsert = '''
    INSERT INTO Files
                         (FileName, FileHash, FolderParentID, FileId, FileSize)
    VALUES        (?,?,?,?,?)
    '''
    # Insert many:
    if len(files) > 0:
        connection.executemany(filesInsert, files)
    
    if len(folders) > 0:
        connection.executemany(foldersInsert, folders)
    
    connection.commit()
        
    # Clean lists:
    files = []
    folders = []
    