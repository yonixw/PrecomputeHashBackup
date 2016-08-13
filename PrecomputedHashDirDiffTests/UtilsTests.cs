using Microsoft.VisualStudio.TestTools.UnitTesting;
using PrecomputedHashDirDiff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrecomputedHashDirDiff.Tests
{
    [TestClass()]
    public class UtilsTests
    {

        [TestMethod()]
        public void byte2humTest()
        {
            // Array to check:
            List<KeyValuePair<long, string>> _lst = new List<KeyValuePair<long, string>>();

            // Add values:

            _lst.Add(new KeyValuePair<long, string>(0, "0 TB, 0 GB, 0 MB, 0 KB, 0 Bytes."));
            _lst.Add(new KeyValuePair<long, string>(1, "0 TB, 0 GB, 0 MB, 0 KB, 1 Bytes."));
            _lst.Add(new KeyValuePair<long, string>(1*1024 + 1,
                 "0 TB, 0 GB, 0 MB, 1 KB, 1 Bytes."));

            // Chek all:

            foreach (KeyValuePair<long,string> pair in _lst) {
                if (pair.Value != Utils.byte2hum(pair.Key)) {
                    Assert.Fail();
                }
            }
        }
    }
}