import hashlib
from precomputeBundle.client.log import *


def hashFileWithLength(path , startbyte, length):
    # type: (string, int, int) -> string
    """
    Read and hash a part of a file
    :param path: path of the file
    :param startbyte: index of first byte to read
    :param length: how much bytes to read
    :return: hash of the part of the file as lower case string, empty if error
    """
    bytes = 0
    with open(path, 'rb') as fileObj:
        fileObj.seek(startbyte)
        bytes = fileObj.read(length)

    actualReadBytes = len(bytes)
    if (actualReadBytes != length) :
        logCustomError("Cant read file chunk. read " + str(actualReadBytes) + " from " + length,"hashfile_length")
    else:

        log("Read hex: " + bytes.encode("hex"))


def hashFileWithEnd(path, startbyte, endbyte):
    # type: (string, int, int) -> string
    """
    Read and hash a part of a file
    :param path: path of the file
    :param startbyte: index of first byte to read
    :param endbyte: index of last byte to read (inclusive)
    :return: hash of the part of the file as lower case string, empty if error
    """
    calcLength = length=endbyte-startbyte+1
    return hashFileWithLength(path, startbyte, calcLength)



hashFileWithEnd("C:\Users\YoniWas\AppData\Roaming\Precompute Backup Manager\________log_12_11_2016.txt",0,5)
hashFileWithLength("C:\Users\YoniWas\AppData\Roaming\Precompute Backup Manager\________log_12_11_2016.txt",0,6)