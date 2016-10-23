import os
import sys
import shutil # for rmtree
import time
import pwd,grp

def pathjoin(array):
    # SO? 14826888
    return os.path.join(*array)

def smartdir(target):
    if (not os.path.isdir(target)):
        os.makedirs(target)
    
# Using BDIR as root:    
def hardlinkTree(srcfolder, targfolder):
    #SO? 10781488 A:1997873
    smartdir(targfolder)
    for root, subdirs, files in os.walk(srcfolder):
        targroot = root.replace(srcfolder,targfolder)
        for dir in subdirs:
            # cant link folders!
            smartdir(pathjoin([targroot,dir]))
        for file in files:
            fromfile = pathjoin([root,file])
            targfile =  pathjoin([targroot,file])
            os.link(fromfile, targfile);
            
def removeFromList(fname,hidedir):
    baseRecent = pathjoin([BDIR, "recent", "all"])
    filescount = 0
    folderscount = 0
    
    print("Using list: " + fname[len(hidedir):])
    
    with open(fname) as file: # Will close because of `with`
        for line in file:
            if ( line.startswith("[DEL]:\\") ): # Only delete if del 
                # Actions:
                # 1) Replace windows sep with current sep
                # 2) Replace end lines
                # 3) remove [DEl]: prefix
                temppath = line.replace('\\',os.sep).replace('\r\n','')[len("[DEL]:\\"):]
                delpath = pathjoin([baseRecent, temppath])
                
                # Notes:
                # 1) `repr` to find whitepaces like \r or \n SO? 24964751
                # 2) isdir\isfile is Case Sensitive! ==> Loot != loot
                #print("Delete? " + repr(delpath)) 
                #print("Dir? " + str(os.path.isdir(delpath)))
                #print("File? " + str(os.path.isfile(delpath)))
                if (os.path.isdir(delpath)):
                    # delete only if exists and folder
                    shutil.rmtree(delpath, ignore_errors=True)
                    if (os.path.isdir(delpath)): # if still exists some error occured
                        print ("Error deleting folder:\n\t " + delpath)
                    else:
                        folderscount +=1
                if (os.path.isfile(delpath)):
                   # delete only if exists and file
                   os.remove(delpath)
                   if (os.path.isfile(delpath)): # if still exists some error occured
                        print ("Error deleting file:\n\t " + delpath)
                   else:
                        filescount +=1
    
    # Since we want to print dafe log, only print relatives and stats:
    print("Deleted %s folders and %s files" % (folderscount,filescount))
    
# B is for Backup and U is for upload
# Backup should have recent + dates of backups
# Upload have adde delta, db3 and list for deleting
BDIR="/media/HDD1PRT3/firstuser/current.unlocked"
UDIR="/media/HDD1PRT3/firstuser/current.unlocked/upload"

dateString = time.strftime("%Y-%m-%d %H-%M-%S")
print("Current time: " + dateString)

# Copy all files from recent to dated
print("* Copy -recent:all- folder to -current date:all-")
hardlinkTree(BDIR + "/recent/all", BDIR + "/" + dateString + "/all")

metadataDirs = ["db3", "delta-files", "delta-lists"]

# Move metadeta to dated folder 
print("* Move metadata from -recent- to -current date-:")
for metadir in metadataDirs:
    src = pathjoin([BDIR,"recent",metadir])
    trg = pathjoin([BDIR,dateString,metadir])
    shutil.move(src,trg)
    
# Move metadeta from upload to recent
print("* Move metadata from -upload- to -recent-:")
for metadir in metadataDirs:
    src = pathjoin([UDIR,metadir])
    trg = pathjoin([BDIR,"recent",metadir])
    shutil.move(src,trg)
    
# Re-create metadata folders with same permission as the upload folder:
# SO? 927866
print("* Recreate metadata dirs with upload permission")
stat_info = os.stat(UDIR)
uid = stat_info.st_uid
gid = stat_info.st_gid

for metadir in metadataDirs:
    trg = pathjoin([UDIR,metadir])
    smartdir(trg)
    fd = os.open( trg, os.O_RDONLY )
    os.fchown(fd,uid,gid)
    os.close( fd )

# Delete old files from text lists
print("* Deleting files using lists")
walk_dir = pathjoin([BDIR, "recent", "delta-lists"])
for root, subdirs, files in os.walk(walk_dir): # Recursive file enumeration SO? 2212643
    for filename in files:
        if (filename == 'old-folders.txt' or filename == 'old-files.txt'):
            removeFromList(pathjoin([root, filename]),walk_dir)
            
# Hardlink the deltafiles inside recent:
print("* Copy delta files to -all- in -recent-")
src = pathjoin([BDIR,"recent","delta-files"])
trg = pathjoin([BDIR,"recent","all"])
hardlinkTree(src,trg)