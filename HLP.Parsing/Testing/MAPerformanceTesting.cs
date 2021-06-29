using HLP.Database;
using HLP.Parsing.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLP.Parsing.Testing
{
    public class MAPerformanceTesting
    {
        private class Test
        {
            public string Word { get; set; }
            public string Stem { get; set; }
            public List<string> Prefixes { get; set; }
            public List<string> Suffixes { get; set; }
            public string MorphCode { get; set; }
            public MAVariant Analysis { get; set; }
            public override string ToString()
            {
                return $"{Word}: {string.Join("+", Prefixes)}{(Prefixes.Any() ? "+" : null)}{Stem}{(Suffixes.Any() ? "+" : null)}{string.Join("+", Suffixes)} ({MorphCode})";
            }
        }

        private static readonly string SourceFilePath = $"{DatabaseLoader.directory}/morph_teszt.txt";

        private MorphologicalAnalyzer Analyzer;
        private List<Test> Tests = new List<Test>();

        public MAPerformanceTesting()
        {
            LoadTestingData();
            Analyzer = new MorphologicalAnalyzer();
            //Tests.ForEach(it => Console.WriteLine(it));
        }

        public void TestDataAsync()
        {
            var numberOfMatch = 0;
            foreach (var item in Tests)
            {
                Console.WriteLine($"Analyzing word '{item.Word}'");
                var analysisResult = Analyzer.AnalyzeWord(item.Word, false);
                Console.WriteLine($"nr = {analysisResult.Variants.Count}");
                var match = analysisResult.Variants.Find(variant => Equals(item, variant));
                item.Analysis = match;
                if (match == null)
                {
                    Console.WriteLine("~~~ NO MATCH ~~~");
                }
                else
                {
                    Console.WriteLine($"~~~  MATCH  ~~~");
                    ++numberOfMatch;
                }
            }
            Console.WriteLine($"{numberOfMatch} of {Tests.Count}");

            Console.WriteLine($"No match:\n{string.Join("\n", Tests.Where(t => t.Analysis == null).Select(t => t.Word))}");

            /*while (true)
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
            }*/
        }

        private bool Equals(Test test, MAVariant variant)
        {
            if (test.Stem != variant.CurrentText ||
                test.Prefixes.Count != variant.Prefixes.Count ||
                test.Suffixes.Count != variant.Suffixes.Count ||
                test.MorphCode != variant.GetMorphCode())
            {
                return false;
            }

            for (var i = 0; i < test.Prefixes.Count; ++i)
            {
                if (test.Prefixes[i] != variant.Prefixes[i].OriginalText)
                {
                    return false;
                }
            }

            for (var i = 0; i < test.Suffixes.Count; ++i)
            {
                if (test.Suffixes[i] != variant.Suffixes[i].OriginalText)
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
                    Tests.Add(new Test
                    {
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
