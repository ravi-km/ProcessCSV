using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;

namespace ProcessCSV
{
    public class Program
    {
        #region constants
        private const string delim = "delimiter";
        private const string file = "file";
        private const string outputName = "outputName";
        private const string outputAdr = "outputAdr";
        #endregion

        public static string fileVal
        {
            get
            {
                return ConfigurationManager.AppSettings[file].ToString();
            }
        }

        public static string delimVal
        {
            get
            {
                return ConfigurationManager.AppSettings[delim].ToString();
            }
        }

        public static string outputNameVal
        {
            get
            {
                return ConfigurationManager.AppSettings[outputName].ToString();
            }
        }

        public static string outputAdrVal
        {
            get
            {
                return ConfigurationManager.AppSettings[outputAdr].ToString();
            }
        }

        static void Main(string[] args)
        {
            try
            {
                bool firstLine = true; // skip header
                List<IDictionary<String, int>> result = ParseFile(firstLine, fileVal); // ParseFile

                // Sort names
                Dictionary<string, int> Namedict = result[0].OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
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

                //Write files
                WriteNameFile(sortedList);
                WriteAdrFile(result[1]);

                Console.WriteLine("Output Name File created at :" + outputNameVal);
                Console.WriteLine("Output Address File created at :" + outputAdrVal);
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
        }

        public static void WriteAdrFile(IDictionary<string, int> dictionary)
        {
            try
            {
                using (StreamWriter stream = new StreamWriter(outputAdrVal))
                {
                    foreach (var item in dictionary)
                    {
                        stream.WriteLine(item.Value + " " + item.Key);
                    }
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void WriteNameFile(List<SortedDictionary<string, int>> sortedList)
        {
            try
            {
                using (StreamWriter stream = new StreamWriter(outputNameVal))
                {
                    foreach (var sDict in sortedList)
                    {
                        foreach (var item in sDict)
                        {
                            stream.WriteLine(item.Key + ", " + item.Value);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        public static List<IDictionary<String, int>> ParseFile(bool firstLine, string path)
        {
            try
            {
                Dictionary<string, int> Namedict = new Dictionary<string, int>(); // list of names
                SortedDictionary<string, int> Adrdict = new SortedDictionary<string, int>(); // list of addresses

                List<IDictionary<String, int>> result = new List<IDictionary<string, int>>();

                using (TextFieldParser Fileparser = new TextFieldParser(path))
                {
                    Fileparser.TextFieldType = FieldType.Delimited;
                    Fileparser.HasFieldsEnclosedInQuotes = false;
                    Fileparser.SetDelimiters(delimVal);
                    Fileparser.TrimWhiteSpace = true;

                    while (!Fileparser.EndOfData)
                    {
                        string[] line = Fileparser.ReadFields();

                        if (firstLine)
                        {
                            firstLine = false;
                            continue;
                        }

                        /// parsing address
                        string[] temp = line[2].Split(' ');

                        int stNum = 0;
                        int.TryParse(temp[0], out stNum);

                        string streetName = temp[1] + " " + temp[2];

                        if (!Adrdict.ContainsKey(streetName)) // add to address dictionary
                        {
                            Adrdict.Add(streetName, stNum);
                        }

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
                }

                result.Add(Namedict);
                result.Add(Adrdict);

                return result;
            }
            catch (FileNotFoundException)
            {
                throw;
            }
            catch (Exception )
            {
                throw;
            }
        }
    }
}