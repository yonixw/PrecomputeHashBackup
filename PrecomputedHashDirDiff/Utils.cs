using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrecomputedHashDirDiff
{
    public class Utils
    {
        public static string byte2hum(long bytes)
        {
            string result = "";

            long KB = 1024;
            long MB = 1024 * KB;
            long GB = 1024 * MB;
            long TB = 1024 * GB;

            long units = 0;

            // TERA
            while (bytes > TB)
            {
                units++;
                bytes = bytes - TB;
            }

            result += units + " TB, ";
            units = 0;

            // GIGA
            while (bytes > GB)
            {
                units++;
                bytes = bytes - GB;
            }

            result += units + " GB, ";
            units = 0;

            // Mega
            while (bytes > MB)
            {
                units++;
                bytes = bytes - MB;
            }

            result += units + " MB, ";
            units = 0;

            // Kilo
            while (bytes > KB)
            {
                units++;
                bytes = bytes - KB;
            }

            result += units + " KB, ";
            units = 0;

            result += bytes + " Bytes.";
            return result;


        }
    }
}
