using HLP.Database;
using HLP.Database.Extensions;
using HLP.Database.Models;
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
        private MorphologicalAnalyzer Analyzer;
        private DatabaseContext DbContext;

        public MAPerformanceTesting()
        {
            DbContext = DatabaseContext.GetInstance();
            Analyzer = new MorphologicalAnalyzer();
            //Tests.ForEach(it => Console.WriteLine(it));
        }

        public void TestDataAsync()
        {
            var numberOfMatch = 0;
            foreach (var item in DbContext.MorphTests.Select(t => t.ToModel()))
            {
                Console.WriteLine($"Analyzing word '{item.Word}'");
                var analysisResult = Analyzer.AnalyzeWord(item.Word, false);
                Console.WriteLine($"nr = {analysisResult.Variants.Count}");
                var match = analysisResult.Variants.Find(variant => Equals(item, variant));
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
            Console.WriteLine($"{numberOfMatch} of {DbContext.MorphTests.Count()}");

            //Console.WriteLine($"No match:\n{string.Join("\n", DbContext.MorphTests.Where(t => t.Analysis == null).Select(t => t.Word))}");

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

        private bool Equals(MorphTest test, MAVariant variant)
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
    }
}
