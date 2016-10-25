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
    
# B is for Backup and U is for upload
# Backup should have recent + dates of backups
# Upload have adde delta, db3 and list for deleting
BDIR="/media/HDD1PRT3/firstuser/current.unlocked"
UDIR="/media/HDD1PRT3/firstuser/current.unlocked/upload"

print("INFO: please run under the same user as the uploader.")

metadataDirs = ["db3", "delta-files", "delta-lists"]
    
# Re-create metadata folders with same permission as the upload folder:
# SO? 927866
print("* Create subfolders with upload permission")
stat_info = os.stat(UDIR)
uid = stat_info.st_uid
gid = stat_info.st_gid

for metadir in metadataDirs:
    trg = pathjoin([UDIR,metadir])
    smartdir(trg)
    fd = os.open( trg, os.O_RDONLY )
    os.fchown(fd,uid,gid)
    os.close( fd )

print("* Create recent folder")    
recent = pathjoin([BDIR,"recent"])
smartdir(recent)

for metadir in metadataDirs:
    trg = pathjoin([recent,metadir])
    smartdir(trg)

# Recent has also all dir
trg = pathjoin([recent,"all"])
smartdir(trg)    