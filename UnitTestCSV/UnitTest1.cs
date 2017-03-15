using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Configuration;
using System;

namespace Program.Tests
{
    [TestClass()]
    public class UnitTest1
    {
        #region constants
        private const string delim = "delimiter";
        private const string file = "file";
        private const string outputName = "outputName";
        private const string outputAdr = "outputAdr";
        #endregion

        [TestMethod()]
        public void WriteNameFileTest()
        {
            SortedDictionary<string, int> sList = new SortedDictionary<string, int>() { { "Adam", 1 }, { "Brian", 2 } };
            List<SortedDictionary<string, int>> sortedList = new List<SortedDictionary<string, int>>();
            sortedList.Add(sList);

            ProcessCSV.Program.WriteNameFile(sortedList);

            Assert.AreEqual(true, File.Exists(ConfigurationManager.AppSettings[outputName].ToString()));
        }

        [TestMethod()]
        public void WriteAdrFileTest()
        {
            Dictionary<string, int> sList = new Dictionary<string, int>() { { "Long Lane", 1 }, { "Jan Smut", 2 } };
            ProcessCSV.Program.WriteAdrFile(sList);

            Assert.AreEqual(true, File.Exists(ConfigurationManager.AppSettings[outputAdr].ToString()));

        }

        [TestMethod()]
        public void ParseFileTest()
        {
            string path = ConfigurationManager.AppSettings[file].ToString();
            List<IDictionary<String, int>> result = ProcessCSV.Program.ParseFile(true,path); // ParseFile

            Assert.AreEqual(result[0].Count, 4);
            Assert.AreEqual(result[1].Count, 3);
        }
    }
}
