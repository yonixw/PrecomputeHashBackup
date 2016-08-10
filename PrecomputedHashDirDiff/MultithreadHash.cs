using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PrecomputedHashDirDiff
{

    /*
    General Parallel Problems:
        how to split reads and mid hash? need to replicate it ....
        If you split by known parts (like 5 mb) the last one need to wait for all the others to complete.... ===> no pipeline implementation.
        We split by size and cores.... 2 parameters that change final hash
    */



    class MultithreadHash
    {
        FileInfo _file;
        long _bytesPerThread;
        int _hashcores;
        Type _algo;
        bool _info;

        public MultithreadHash(FileInfo fi, long bytesPerThread, int HashCores, Type HashMethod, bool verbose = false) {
            //Set privates:
            _file = fi;
            _algo = HashMethod;
            _bytesPerThread = bytesPerThread; // Total speed = BperThread * HashCores 
            _hashcores = HashCores;
            _info = verbose;

            // Make sure we won't create unused hash cores:
            _hashcores = Math.Min(_hashcores, (int)Math.Ceiling((float)_file.Length / bytesPerThread));
            Console.WriteLine("Using {0} Hash cores.", _hashcores);

            // Hash cores is important for comparing two results,
            //      Results from 4 hashcores will be different from 3 hashcores.
            //      Use benchmark to find the best for any *disk* (not pc)

        }


        public string StartSync() {
            return Calculate();
        }

            Dictionary<int, string> HashResult = new Dictionary<int, string>();
        string  Calculate() {
            Task[] allHashCores = new Task[_hashcores];

            // Run N hash cores in parallel and tell each one it's id.
            for(int i=0;i<allHashCores.Length; i++) {
                allHashCores[i] = new Task((object coreId) =>
                {
                    Hash((int)coreId);
                },i);

                allHashCores[i].Start();
            }

            // Wait here for all task to End:
            Task.WaitAll(allHashCores);

            // Get Hash from all hashes:
            HashAlgorithm hashSum = SHA256.Create(); //(HashAlgorithm)(Activator.CreateInstance(_algo));
            byte[] buffer = new byte[1024];

            for (int i=0;i<allHashCores.Length; i++) {
                byte[] partialHashBytes = Encoding.UTF8.GetBytes(HashResult[i]);

                // Process each partial hash:
                hashSum.TransformBlock(partialHashBytes, 0, partialHashBytes.Length, buffer, 0);
            }

            // Get hash of hashes (sum) :
            hashSum.TransformFinalBlock(buffer, 0, 0);
            string finalHash = BitConverter.ToString(hashSum.Hash).Replace("-", "");
            Console.WriteLine("Final hash: {0}", finalHash);

            return finalHash;
        }

        public void Hash(int CoreId)
        {
            string finalHash = "";

            // Compute Hash
            HashAlgorithm hash = SHA256.Create(); //(HashAlgorithm)(Activator.CreateInstance(_algo));
            byte[] buffer = new byte[this._bytesPerThread];

            using (FileStream filestream = _file.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) // SO? 9759697
            {
                long size = filestream.Length;
                filestream.Position = CoreId * this._bytesPerThread;

                while (filestream.Position < size)
                {
                    int lastBytesRead = filestream.Read(buffer, 0, buffer.Length);
                    hash.TransformBlock(buffer, 0, lastBytesRead, buffer, 0);
                    filestream.Position += this._hashcores * this._bytesPerThread;

                }

                hash.TransformFinalBlock(buffer, 0, 0);

                finalHash = BitConverter.ToString(hash.Hash).Replace("-", "");
                if(_info) Console.WriteLine("Core: {0}, Hash: {1}",CoreId, finalHash.Substring(0, 10));

            }

            // Write safetly for the result dictionary
            lock(HashResult) {
                HashResult.Add(CoreId, finalHash);
            }
        }
    }
}
