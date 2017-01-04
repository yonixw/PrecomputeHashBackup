from .log import log

def hashfile(path , startbyte, length):
    """
    Read and hash a part of a file
    :param path: path of the file
    :param startbyte: index of first byte to read
    :param length: how much bytes to read
    :return:
    """
    bytes = 0
    with open(path, 'rb') as fileObj:
        fileObj.seek(startbyte)
        bytes = fileObj.read(length)

    #if (len(bytes) != length) :


def hashfile(path, startbyte, endbyte):
    """
    Read and hash a part of a file
    :param path: path of the file
    :param startbyte: index of first byte to read
    :param endbyte: index of last byte to read (inclusive)
    :return: hash of the part of the file as lower case string
    """
    return hashfile(path, startbyte, length=(endbyte - startbyte +1));



hashfile("C:\Users\YoniWas\AppData\Roaming\Precompute Backup Manager\________log_12_11_2016.txt",0,endbyte=5);