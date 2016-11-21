import hash
import sqlite

import sys, os
from os.path import isfile, isdir, join
import shutil 

def write(text):
    sys.stdout.write(text)
    sys.stdout.flush()
    
def getFiles(dir):
    return [f for f in os.listdir(dir) if isfile(join(dir, f))]
    
def getFolders(dir):
    return [d for d in os.listdir(dir) if isdir(join(dir, d))]

FileIdCounter = 0
FolderIdCounter = 0    
    
def HashFiles(dir, dirname, parentId):
    global FileIdCounter
    global FolderIdCounter
    myFolderId = FolderIdCounter
    FolderIdCounter = FolderIdCounter +1
    
    print("[Directory] (" + str(myFolderId) + ") " + dirname)
    folderSize = 0
    
    # Loop through files
    for file in getFiles(dir):
        write("\t[FILE] (" + str(FileIdCounter) + ") " + file)
        
        # Hash Files 
        filehash = hash.SHA256File(join(dir,file))
        print("=> " + filehash[:10])
        
        # Add to sqlite
        fileId = FileIdCounter
        fileSize = os.stat(join(dir,file)).st_size
        folderSize = folderSize + fileSize
        sqlite.addFile(file, filehash, myFolderId, FileIdCounter, fileSize) # 0 size for debugging
        
        # And move on
        FileIdCounter = FileIdCounter+1
    
    # Recurse for each folder:
    for subdir in getFolders(dir):
        folderSize = folderSize + HashFiles(join(dir,subdir),subdir,myFolderId)
    
    sqlite.addFolder(dirname, parentId, myFolderId, 0)  
    return folderSize

def main():
    if len(sys.argv) < 3 : # Path and args
        print("Please pass folder path and name. Exiting...")
        return;
    
    # The root folder for hashing:
    rootFolder = sys.argv[1]
    rootName = sys.argv[2]
        
    print("Copy template db3");
    shutil.copy("template.db3", "dirHash.db3")
    
    sqlite.connect("dirHash.db3")
    
    HashFiles(rootFolder,rootName,-1)
    sqlite.flush()
    
    sqlite.close()
    
    print("Copy db3 to " + rootName + ".db3" );
    shutil.copy("dirHash.db3",rootName + ".db3" )
    
    print("All Hashing Done!")

# Start program:    
main()