import hashlib
import binascii
import os, sys

def write(text):
    sys.stdout.write(text)
    sys.stdout.flush()
    
def SHA256File(file):
    buffer_size = 1024 * 1024 # MB
    hash256 = hashlib.sha256()
    
    f = open(file)
    sizepercent = os.stat(file).st_size / 10
    if sizepercent < 10:
        sizepercent = 10 # Just to avoid 0
        
    bytesread = 0;
    current10percent = 0
    
    write(" [")
    
    chunk = f.read(buffer_size)
    while not not chunk:
        hash256.update(chunk)
        chunk = f.read(buffer_size)
        bytesread = bytesread + len(chunk)
        if bytesread > (current10percent * sizepercent):
            write(str(current10percent)) # Shows percent 0...1.......2....3 until 9
            current10percent = current10percent + 1
    f.close()
    
    write("] ")
    return binascii.hexlify(hash256.digest()).upper()
    
def test(file):
    print(SHA256File(file)[:10])
    
#test("/media/HDD1PRT1/LockMe/template.db3")
#test("/media/HDD1PRT1/backup1-root/upload.unlocked/delta-files/SEM8/TwitchVOD/twitch_save9132493.ts")



