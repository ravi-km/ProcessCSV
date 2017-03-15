using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;

namespace ProcessCSV
{
    class Program
    {
        #region constants
        private const string delim = "delimiter";
        private const string file = "file";
        private const string outputName = "outputName";
        private const string outputAdr = "outputAdr";
        #endregion

        static void Main(string[] args)
        {
            List<string> adr = new List<string>(); // list of addresses
            bool firstLine = true; // skip header

            //read from file and create two lists for name and address
            using (TextFieldParser Fileparser = new TextFieldParser(ConfigurationManager.AppSettings[file].ToString()))
            {
                Fileparser.TextFieldType = FieldType.Delimited;
                Fileparser.HasFieldsEnclosedInQuotes = false;
                Fileparser.SetDelimiters(ConfigurationManager.AppSettings[delim].ToString());
                Fileparser.TrimWhiteSpace = true;

                Dictionary<string, int> Namedict = new Dictionary<string, int>();
                while (!Fileparser.EndOfData)
                {
                    string[] line = Fileparser.ReadFields();

                    if (firstLine)
                    {
                        firstLine = false;
                        continue;
                    }

                    adr.Add(line[2]); // add to adrress list

                    for (int i = 0; i < 2; i++) // add to name dictionary
                    {
                        if (Namedict.ContainsKey(line[i]))
                        {
                            Namedict[line[i]]++;
                        }
                        else
                        {
                            Namedict.Add(line[i], 1);
                        }
                    }
                }

                Namedict = Namedict.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                var Namegroups = Namedict.GroupBy(x => x.Value);

                List<SortedDictionary<string, int>> sortedList = new List<SortedDictionary<string, int>>();
                SortedDictionary<string, int> tmp = new SortedDictionary<string, int>();

                foreach (var group in Namegroups)
                {
                    tmp = new SortedDictionary<string, int>();
                    foreach (var key in group)
                    {
                        tmp.Add(key.Key, key.Value);
                    }
                    sortedList.Add(tmp);
                }

                using (StreamWriter stream = new StreamWriter(ConfigurationManager.AppSettings[outputName].ToString()))
                {
                    foreach (var sDict in sortedList)
                    {
                        foreach (var item in sDict)
                        {
                            stream.WriteLine(item.Key + ", " + item.Value);
                        }
                    }
                }

                tmp.Clear();

                foreach (var item in adr)
                {
                    string[] temp = item.Split(' ');

                    int i = 0;
                    int.TryParse(temp[0], out i);

                    string streetName = temp[1] + " " + temp[2];

                    if (!tmp.ContainsKey(streetName))
                    {
                        tmp.Add(streetName, i);
                    }
                }
                using (StreamWriter stream1 = new StreamWriter(ConfigurationManager.AppSettings[outputAdr].ToString()))
                {
                    foreach (var item in tmp)
                    {
                        stream1.WriteLine(item.Value + " " + item.Key);
                    }
                }
            }

        }
    }
}