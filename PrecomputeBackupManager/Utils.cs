using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PrecomputeBackupManager
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

        public static string StatToString<T>(Dictionary<string, T> array) {
            string result = "";

            if (array != null) // null if no diff (no recent of folder)
            {
                foreach (string key in array.Keys)
                {
                    result += "\t" + key + ": " + array[key].ToString() + "\n ";
                }
            }

            return result;
        }

        public static string DurationToString(TimeSpan duration)
        {
            if (duration == null) return "[Not Set]";
            return String.Format("{0} Days, {1} Hours, {2} Minutes, {3} Seconds.", duration.Days, duration.Hours, duration.Minutes, duration.Seconds);
        }

        
    }
}
