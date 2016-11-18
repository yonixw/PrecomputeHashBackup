using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrecomputedHashDirDiff
{
    public class Utils
    {
        public static string byte2hum(long bytes)
        {
            string result = bytes  > -1 ? "(+) " : "(-) "; // Negative can happen in changed after only deletion

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

        public static FileInfo[] safeGet_Files(DirectoryInfo di)
        {
            FileInfo[] result = new FileInfo[] { };
            try
            {
                result = di.GetFiles();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return result;
        }

        public static DirectoryInfo[] safeGet_Directories(DirectoryInfo di)
        {
            DirectoryInfo[] result = new DirectoryInfo[] { };
            try
            {
                result = di.GetDirectories();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return result;
        }

        public static string strTo64(string input)
        {
            // SO? 11743160
            return System.Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
        }

        public static string strFrom64(string input)
        {
            // SO? 11743160
            return Encoding.UTF8.GetString(System.Convert.FromBase64String(input));
        }

    }
}
