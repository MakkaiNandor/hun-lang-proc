using HLP.Database;
using HLP.Database.Models;
using HLP.Parsing.Log;
using HLP.Parsing.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLP.Parsing.Testing
{
    public class PerformanceTesting
    {
        private DatabaseContext dbContext;
        private MorphologicalAnalyzer Analyzer;
        private Logging log;

        public PerformanceTesting()
        {
            dbContext = DatabaseContext.GetInstance();
            Analyzer = new MorphologicalAnalyzer();
            log = new Logging();
        }

        public long TestMorpAnalyzer(out int nrOfGoods, out int nrOfWrongs)
        {
            // Változók inicializálása
            long elapsedTime = 0;
            nrOfGoods = 0;
            nrOfWrongs = 0;
            var stopper = new Stopwatch();

            // Tesztelés indítása
            log.Log("MA performance testing started");
            foreach (var item in dbContext.MorphTests)
            {
                // Elemzés és időmérés
                stopper.Restart();
                var analysisResult = Analyzer.AnalyzeWord(item.Word);
                stopper.Stop();
                log.Log(analysisResult, stopper.ElapsedMilliseconds);
                elapsedTime += stopper.ElapsedMilliseconds;

                // Helyes elemzés keresése
                var match = analysisResult.Variants.Find(variant => Equals(item, variant));
                if (match == null)
                    ++nrOfWrongs;
                else
                    ++nrOfGoods;
            }

            log.Log($"MA performance testing stoped\tPerformance: {nrOfGoods} of {nrOfGoods+nrOfWrongs} ({(nrOfWrongs/(nrOfGoods+nrOfWrongs))*100}%)\tElapsed time: {elapsedTime} ms");

            return elapsedTime;
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
