using HLP.Database;
using HLP.Database.Models;
using HLP.Parsing.Log;
using HLP.Parsing.Models;
using HLP.Parsing.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HLP.Parsing.Extensions;

namespace HLP.Parsing
{
    // mondatrészek
    public enum SPart
    {
        UNDEFINED = 0,
        PREDICATE = 1,
        SUBJECT = 2,
        OBJECT = 3,
        ADVERB = 4,
        ATTRIBUTE = 5
    }

    public class SyntacticAnalyzer
    {
        private readonly MorphologicalAnalyzer analyzer;
        private readonly Logging log;
        private readonly DatabaseContext context;

        public SyntacticAnalyzer()
        {
            analyzer = new MorphologicalAnalyzer();
            log = new Logging();
            context = DatabaseContext.GetInstance();
        }

        // egy mondat szintaktikai elemzése
        public SAResult AnalyzeSentence(string sentence)
        {
            var result = new SAResult(sentence);
            var wordResults = analyzer.AnalyzeText(sentence);

            foreach (var wordResult in wordResults)
            {
                wordResult.ExpandWord();
                wordResult.DeleteDuplicates();
                RemoveWrongResults(wordResult);
            }

            result.Result = wordResults.Select(it => new SAItem
            {
                Text = it.OriginalWord,
                Type = SPart.UNDEFINED,
                MorphResult = it
            }).ToList();

            DefinePredicate(result);

            DefineObjects(result);

            DefineAttributes(result);

            DefineAdverbs(result);

            DefineSubjects(result);

            return result;
        }

        // állítmány keresése
        private void DefinePredicate(SAResult result)
        {
            var resultsWithVerb = result.Result.Where(it => it.MorphResultContains("IGE")).ToList();
            if (!resultsWithVerb.Any())
                return;
            if (resultsWithVerb.Count == 1)
            {
                resultsWithVerb[0].Type = SPart.PREDICATE;
                CheckNegate(resultsWithVerb[0], result, SPart.PREDICATE);
                return;
            }
            var resultsWithOnlyVerb = resultsWithVerb.Where(it => it.HasOnlyOne()).ToList();
            if (!resultsWithOnlyVerb.Any())
            {
                var item = resultsWithVerb.Select(it => new { Item = it, Value = it.MorphResult.Variants.Where(v => v.WordType != "IGE").Count() }).OrderByDescending(it => it.Value).First().Item;
                item.Type = SPart.PREDICATE;
                CheckNegate(item, result, SPart.PREDICATE);
                return;
            }
            resultsWithOnlyVerb[0].Type = SPart.PREDICATE;
            CheckNegate(resultsWithOnlyVerb[0], result, SPart.PREDICATE);
        }

        // tárgyak keresése
        private void DefineObjects(SAResult result)
        {
            foreach (var item in result.Result.Where(it => it.Type == SPart.UNDEFINED))
            {
                var types = item.GetTypes();
                var codes = item.LastCodes();
                if ((types.Contains("FN") || types.Contains("NM")) && codes.Contains("ACC"))
                {
                    item.Type = SPart.OBJECT;
                    CheckArticle(item, result, SPart.OBJECT);
                }
                else if (types.Contains("NM") || types.Contains("FIN"))
                {
                    item.Type = SPart.OBJECT;
                }
            }
        }

        // határozók keresése
        private void DefineAdverbs(SAResult result)
        {
            foreach (var item in result.Result.Where(it => it.Type == SPart.UNDEFINED))
            {
                var types = item.GetTypes();
                if (types.Contains("HA"))
                {
                    item.Type = SPart.ADVERB;
                    CheckArticle(item, result, SPart.ADVERB);
                    continue;
                }
                var lastGroups = item.LastGroups();
                if ((types.Contains("FN") || types.Contains("MN") || types.Contains("NM") || types.Contains("SZN")) && lastGroups.Contains(5))
                {
                    item.Type = SPart.ADVERB;
                    CheckArticle(item, result, SPart.ADVERB);
                    continue;
                }
                var index = result.Result.IndexOf(item);
                if (index == result.Result.Count - 1 || !types.Contains("FN"))
                    continue;
                var nextItemTypes = result.Result[index + 1].GetTypes();
                if (nextItemTypes.Contains("NU"))
                {
                    item.Type = SPart.ADVERB;
                    result.Result[index + 1].Type = SPart.ADVERB;
                    CheckArticle(item, result, SPart.ADVERB);
                }
            }
        }

        // alanyok keresése
        private void DefineSubjects(SAResult result)
        {
            foreach (var item in result.Result.Where(it => it.Type == SPart.UNDEFINED))
            {
                var typesWithoutRags = item.MorphResult.Variants.Where(it => !it.Suffixes.Any(s => s.Info.Type == "R")).Select(it => it.WordType).ToList();
                if (typesWithoutRags.Contains("FN"))
                {
                    item.Type = SPart.SUBJECT;
                    CheckArticle(item, result, SPart.SUBJECT);
                    continue;
                }
                var types = item.GetTypes();
                if (types.Contains("FIN") || types.Contains("NM"))
                {
                    item.Type = SPart.SUBJECT;
                }
            }
        }

        // jelzők keresése
        private void DefineAttributes(SAResult result)
        {
            foreach (var item in result.Result)
            {
                var groups = item.AllGroups();
                if (groups.Contains(4) || groups.Contains(7))
                {
                    var index = result.Result.IndexOf(item);
                    if (index > 0)
                    {
                        var prevItem = result.Result[index - 1];
                        var prevTypes = prevItem.GetTypes();
                        if (prevTypes.Contains("FN"))
                        {
                            prevItem.Type = SPart.ATTRIBUTE;
                            CheckArticle(prevItem, result, SPart.ATTRIBUTE);
                            continue;
                        }
                    }
                }
                if (item.Type != SPart.UNDEFINED) continue;
                var types = item.GetTypes();
                var codes = item.LastCodes();
                if ((types.Contains("NM") || types.Contains("FN")) && codes.Contains("DAT"))
                {
                    item.Type = SPart.ATTRIBUTE;
                    CheckArticle(item, result, SPart.ATTRIBUTE);
                    continue;
                }
                groups = item.LastGroups();
                if (types.Contains("MN") && !groups.Contains(5))
                {
                    var index = result.Result.IndexOf(item);
                    if (index == result.Result.Count - 1) continue;
                    var nextTypes = result.Result[index + 1].GetTypes();
                    if (nextTypes.Contains("FN"))
                    {
                        item.Type = SPart.ATTRIBUTE;
                        CheckArticle(item, result, SPart.ATTRIBUTE);
                    }
                }
            }
        }

        // tagadószó ellenőrzése
        private void CheckNegate(SAItem item, SAResult result, SPart type)
        {
            var index = result.Result.IndexOf(item);
            if (index == 0) return;
            var prevItem = result.Result[index - 1];
            if (prevItem.Text.ToLower() == "nem" || prevItem.Text.ToLower() == "ne")
                prevItem.Type = type;
        }

        // névelő ellenőrzése
        private void CheckArticle(SAItem item, SAResult result, SPart type)
        {
            var index = result.Result.IndexOf(item);
            if (index == 0) return;
            var prevItem = result.Result[index - 1];
            if (item.Text.ToLower().StartsWithVowel() && prevItem.Text.ToLower() == "az")
                prevItem.Type = type;
            else if (!item.Text.ToLower().StartsWithVowel() && prevItem.Text.ToLower() == "a")
                prevItem.Type = type;
        }

        // helytelen morfológiai elemzések kiszűrése
        private void RemoveWrongResults(MAResult result)
        {
            var wordsNotInDB = result.Variants.Where(it => !context.SearchWordInDatabase(it.OriginalText, it.WordType).Any()).ToList();
            if (wordsNotInDB.Count != result.Variants.Count)
            {
                wordsNotInDB.ForEach(it => result.Variants.Remove(it));
            }
            else
            {
                wordsNotInDB = result.Variants.Where(it => !context.SearchWordInDatabase(it.CurrentText, it.WordType).Any()).ToList();
                if (wordsNotInDB.Count != result.Variants.Count)
                {
                    wordsNotInDB.ForEach(it => result.Variants.Remove(it));
                }
            }

            var checkedWords = new List<string>();
            var varsWithSuffixes = result.Variants.Where(it => it.Suffixes.Any()).ToList();
            foreach (var variant in varsWithSuffixes)
            {
                if (!checkedWords.Contains(variant.OriginalText))
                {
                    var varsWithThisWord = varsWithSuffixes.Where(it => it.OriginalText == variant.OriginalText);
                    var minSuffixes = varsWithThisWord.Min(it => it.Suffixes.Count);
                    varsWithThisWord.Where(it => it.Suffixes.Count > minSuffixes).ToList().ForEach(it => result.Variants.Remove(it));
                    checkedWords.Add(variant.OriginalText);
                }
            }

            checkedWords = new List<string>();
            var varsWithStemVar = result.Variants.Where(it => it.CurrentText != it.OriginalText).ToList();
            foreach (var variant in varsWithStemVar)
            {
                if (!checkedWords.Contains(variant.CurrentText))
                {
                    var varsWithThisWord = varsWithStemVar.Where(it => it.CurrentText == variant.CurrentText);
                    var maxLength = varsWithThisWord.Max(it => it.OriginalText.Length);
                    varsWithThisWord.Where(it => it.OriginalText.Length < maxLength).ToList().ForEach(it => result.Variants.Remove(it));
                    checkedWords.Add(variant.CurrentText);
                }
            }
        }
    }
}
