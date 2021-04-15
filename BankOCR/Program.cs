using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BankOCR
{
    // https://codingdojo.org/kata/BankOCR/
    
    class Program
    {
        public const string RESULTS_SUFFIX = "_Results";

        static void Main(string[] args)
        {
            try
            {
                string testFilesPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..", ".files"));
                var files = Directory.GetFiles(testFilesPath);

                foreach (var file in files)
                {
                    if (file.Contains(RESULTS_SUFFIX))
                    {
                        continue;
                    }

                    var lines = File.ReadAllLines(file).ToList();

                    var results = Decoder.DecodeFile(lines);

                    File.WriteAllLines(file.Replace(".txt", RESULTS_SUFFIX + ".txt"), results);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}
