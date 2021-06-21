using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HLP.WordProcessing.Models;

namespace HLP.WordProcessing
{
    public class PerformanceTesting
    {
        private class Test
        {
            public string Word { get; set; }
            public string Stem { get; set; }
            public List<string> Prefixes { get; set; }
            public List<string> Suffixes { get; set; }
            public string MorphCode { get; set; }
            public AnalysisVariant analysis { get; set; }
            public override string ToString()
            {
                return $"{Word}: {string.Join("+", Prefixes)}{(Prefixes.Any() ? "+" : null)}{Stem}{(Suffixes.Any() ? "+" : null)}{string.Join("+", Suffixes)} ({MorphCode})";
            }
        }

        private static readonly string SourceFilePath = "D:\\Egyetem\\Államvizsga\\HunLangProc\\HLP.Database\\Data\\morph_teszt.txt";

        private MorphologicalAnalyzer Analyzer;
        private List<Test> Tests = new List<Test>();

        public PerformanceTesting()
        {
            LoadTestingData();
            Analyzer = new MorphologicalAnalyzer();
            //Tests.ForEach(it => Console.WriteLine(it));
        }

        public void TestData()
        {
            var numberOfMatch = 0;
            foreach (var item in Tests)
            {
                Console.WriteLine($"Testing word '{item.Word}'");
                var analysisResult = Analyzer.Analyze(item.Word).First();
                //Console.WriteLine(result);
                var match = analysisResult.Variants.Find(variant => Equals(item, variant));
                item.analysis = match;
                if (match == null)
                {
                    Console.WriteLine("NO MATCH!");
                }
                else
                {
                    Console.WriteLine($"MATCH!");
                    ++numberOfMatch;
                }
            }
            Console.WriteLine($"{numberOfMatch} of {Tests.Count}");

            while (true)
            {
                var input = Console.ReadLine();
                if (input == "exit")
                {
                    break;
                }
                var test = Tests.Find(t => t.Word == input);
                if (test != null)
                {
                    Console.WriteLine(test.analysis);
                }
            }
        }

        private bool Equals(Test test, AnalysisVariant variant)
        {
            if (test.Stem != variant.Text || 
                test.Prefixes.Count != variant.Prefixes.Count || 
                test.Suffixes.Count != variant.Suffixes.Count ||
                test.MorphCode != variant.GetMorphCode())
            {
                return false;
            }

            for (var i = 0; i < test.Prefixes.Count; ++i)
            {
                if (test.Prefixes[i] != variant.Prefixes[i].Text)
                {
                    return false;
                }
            }

            for (var i = 0; i < test.Suffixes.Count; ++i)
            {
                if (test.Suffixes[i] != variant.Suffixes[i].Text)
                {
                    return false;
                }
            }

            return true;
        }

        private void LoadTestingData()
        {
            using (var reader = new StreamReader(SourceFilePath))
            {
                reader.ReadLine(); // Skip the header
                while (!reader.EndOfStream)
                {
                    var values = reader.ReadLine().Split(';');
                    var prefixes = new List<string>();
                    var suffixes = new List<string>();
                    var stem = "";
                    foreach (var item in values[1].Split('+'))
                    {
                        if (item.StartsWith("!"))
                        {
                            stem = item.Trim('!');
                        }
                        else if (stem.Length > 0)
                        {
                            suffixes.Add(item);
                        }
                        else
                        {
                            prefixes.Add(item);
                        }
                    }
                    Tests.Add(new Test {
                        Word = values[0],
                        Stem = stem,
                        Prefixes = prefixes,
                        Suffixes = suffixes,
                        MorphCode = values[2]
                    });
                }
            }
        }
    }
}
