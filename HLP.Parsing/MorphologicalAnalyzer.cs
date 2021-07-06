using HLP.Database;
using HLP.Database.Models;
using HLP.Parsing.Extensions;
using HLP.Parsing.Log;
using HLP.Parsing.Models;
using HLP.Parsing.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLP.Parsing
{
    public class MorphologicalAnalyzer
    {
        private readonly DatabaseContext dbContext;
        private readonly Tokenizer tokenizer;
        private readonly Logging log;

        public MorphologicalAnalyzer()
        {
            dbContext = DatabaseContext.GetInstance();
            tokenizer = new Tokenizer();
            log = new Logging();
        }

        // szöveg szavainak morfológiai elemzése
        public List<MAResult> AnalyzeText(string text)
        {
            var result = new List<MAResult>();

            if (text.Length == 0)
                return result;

            var words = tokenizer.GetWords(text);

            foreach (var word in words)
            {
                // Elemzés és időmérés
                MAResult res;
                var watch = new Stopwatch();
                watch.Start();
                res = AnalyzeWord(word);
                watch.Stop();
                log.Log(res, watch.ElapsedMilliseconds);
                result.Add(res);
            }

            return result;
        }

        // egy szó morfológiai elemzése
        public MAResult AnalyzeWord(string word)
        {
            var result = new MAResult(word);
            word = word.ToLower();

            var variant = new MAVariant(word);

            // Prefixumok levágása
            foreach (var prefix in variant.PossiblePrefixes())
            {
                var newVariant = new MAVariant(variant);
                newVariant.RemovePrefix(prefix);
                RemoveSuffixes(newVariant, result);
            }

            // Szuffixumok levágása
            if (!result.Variants.Any())
            {
                RemoveSuffixes(variant, result);
            }

            // Eredmények szűrése, redukálása
            var reducer = new ResultReducer();
            reducer.ReduceResults(result);

            return result;
        }

        // szuffixumok elkülönítése
        private void RemoveSuffixes(MAVariant variant, MAResult result)
        {
            var variants = new List<MAVariant> { variant };

            while (variants.Any())
            {
                var variantList = new List<MAVariant>(variants);
                foreach (var currVariant in variantList)
                {
                    // keresés az adatbázisban
                    var alternativeVariants = new List<MAVariant>();
                    var commonTypes = dbContext.SearchWordInDatabase(currVariant.CurrentText, currVariant.WordType);

                    // ha az adatbázisban megtalálható, a közös szófajok alapján új elemzésváltozatok készítése
                    if (commonTypes.Any())
                    {
                        alternativeVariants.AddRange(commonTypes.Select(t => new MAVariant(currVariant)
                        {
                            WordType = t
                        }));
                        result.Variants.AddRange(alternativeVariants);
                        variants.Remove(currVariant);
                    }
                    // ha nincs az adatbázisban, szótári alak keresése és új elemzésváltozatok készítése
                    else
                    {
                        if (currVariant.Suffixes.Any())
                        {
                            var stemVariants = StemChecker.CheckStems(currVariant.CurrentText, currVariant.WordType);

                            foreach (var stem in stemVariants)
                            {
                                var types = dbContext.SearchWordInDatabase(stem, variant.WordType);

                                if (types.Any())
                                {
                                    alternativeVariants.AddRange(types.Select(t => new MAVariant(currVariant)
                                    {
                                        CurrentText = stem,
                                        WordType = t
                                    }));
                                }
                            }
                        }

                        if (alternativeVariants.Any())
                        {
                            result.Variants.AddRange(alternativeVariants);
                            variants.Remove(currVariant);
                            continue;
                        }
                        else
                        {
                            alternativeVariants.Add(currVariant);
                        }
                    }

                    // levágható toldalékok keresése
                    foreach (var v in alternativeVariants)
                    {
                        var removableSuffixes = v.PossibleSuffixes();

                        if (!removableSuffixes.Any())
                        {
                            variants.Remove(v);
                        }

                        foreach (var suffix in removableSuffixes)
                        {
                            // toldalék levágása
                            var newVariant = new MAVariant(v);
                            newVariant.RemoveSuffix(suffix);
                            variants.Add(newVariant);

                            if (suffix.Prevowel &&
                                newVariant.CurrentText.HasVowel(2) &&
                                newVariant.CurrentText.EndsWithPreVowel())
                            {
                                // toldalék levágása előhangzóval
                                var preVowel = newVariant.CurrentText.GetLastLetter();
                                var preVowelVariant = new MAVariant(v);
                                preVowelVariant.RemoveSuffix(suffix.GetWithPreVowel(preVowel));
                                variants.Add(preVowelVariant);
                            }
                        }
                    }
                    variants.Remove(currVariant);
                }
            }
        }
    }
}
