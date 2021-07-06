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
        private MorphologicalAnalyzer MAnalyzer;
        private SyntacticAnalyzer SAnalyzer;
        private Logging log;

        public PerformanceTesting()
        {
            dbContext = DatabaseContext.GetInstance();
            MAnalyzer = new MorphologicalAnalyzer();
            SAnalyzer = new SyntacticAnalyzer();
            log = new Logging();
        }

        // morfológiai elemző tesztelése
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
                var analysisResult = MAnalyzer.AnalyzeWord(item.Word);
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

            log.Log($"MA performance testing stoped\tPerformance: {nrOfGoods} of {nrOfGoods+nrOfWrongs} ({((double)nrOfGoods/(nrOfGoods+nrOfWrongs))*100}%)\tElapsed time: {elapsedTime} ms");

            return elapsedTime;
        }

        // szintaktikai elemző tesztelése
        public long TestSyntAnalyzer(out int nrOfGoods, out int nrOfWrongs)
        {
            // Változók inicializálása
            long elapsedTime = 0;
            nrOfGoods = 0;
            nrOfWrongs = 0;
            var stopper = new Stopwatch();

            // Tesztelés indítása
            log.Log("SA performance testing started");
            foreach (var item in dbContext.SyntTests)
            {
                var sentence = string.Join(" ", item.Words);
                // Elemzés és időmérés
                stopper.Restart();
                var analysisResult = SAnalyzer.AnalyzeSentence(sentence);
                stopper.Stop();
                log.Log(analysisResult, stopper.ElapsedMilliseconds);
                elapsedTime += stopper.ElapsedMilliseconds;

                // Helyes elemzés-e
                if (Equals(item, analysisResult))
                    ++nrOfGoods;
                else
                    ++nrOfWrongs;
            }

            log.Log($"SA performance testing stoped\tPerformance: {nrOfGoods} of {nrOfGoods + nrOfWrongs} ({((double)nrOfGoods / (nrOfGoods + nrOfWrongs)) * 100}%)\tElapsed time: {elapsedTime} ms");

            return elapsedTime;
        }

        // morfológiai elemzés összehasonlítása az elvárt kimenettel
        private bool Equals(MorphTest test, MAVariant variant)
        {
            if (test.Stem.RealText != variant.CurrentText ||
                test.Stem.Text != variant.OriginalText ||
                test.Prefixes.Count != variant.Prefixes.Count ||
                test.Suffixes.Count != variant.Suffixes.Count ||
                test.MorphCode != variant.GetMorphCode())
            {
                return false;
            }

            for (var i = 0; i < test.Prefixes.Count; ++i)
            {
                if (test.Prefixes[i].Text != variant.Prefixes[i].OriginalText ||
                    test.Prefixes[i].RealText != variant.Prefixes[i].Text)
                {
                    return false;
                }
            }

            for (var i = 0; i < test.Suffixes.Count; ++i)
            {
                if (test.Suffixes[i].Text != variant.Suffixes[i].OriginalText ||
                    test.Suffixes[i].RealText != variant.Suffixes[i].Text)
                {
                    return false;
                }
            }

            return true;
        }

        // szintaktikai elemzés összehasonlítása az elvárt kimenettel
        private bool Equals(SyntTest test, SAResult result)
        {
            if (test.Words.Count != result.Result.Count)
                return false;
            for (var i = 0; i < test.Words.Count; ++i)
            {
                var tWord = test.Words[i];
                var tType = test.Types[i];
                var rWord = result.Result[i];
                if (tWord != rWord.Text)
                    return false;
                if (rWord.Type != (SParts)tType)
                    return false;
            }
            return true;
        }
    }
}
