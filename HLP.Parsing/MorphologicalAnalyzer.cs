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

        private void RemoveSuffixes(MAVariant variant, MAResult result)
        {
            var variants = new List<MAVariant> { variant };

            while (true)
            {
                if (!variants.Any()) break;
                var variantList = new List<MAVariant>(variants);
                foreach (var currVariant in variantList)
                {
                    // TODO: search in db
                    var alternativeVariants = new List<MAVariant>();
                    var commonTypes = dbContext.SearchWordInDatabase(currVariant.CurrentText, currVariant.WordType);

                    // TODO: if in db, get common types and create new variants
                    if (commonTypes.Any())
                    {
                        alternativeVariants.AddRange(commonTypes.Select(t => new MAVariant(currVariant)
                        {
                            WordType = t
                        }));
                        result.Variants.AddRange(alternativeVariants);
                        variants.Remove(currVariant);
                    }
                    // TODO: if not in db, search for stem variants and create new variants
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

                    // TODO: for every variant in altenative variants search possible suffixes
                    foreach (var v in alternativeVariants)
                    {
                        var removableSuffixes = v.PossibleSuffixes();

                        if (!removableSuffixes.Any())
                        {
                            variants.Remove(v);
                        }

                        // TODO: for every suffix in possible suffixes:
                        foreach (var suffix in removableSuffixes)
                        {
                            // TODO: create new variant and remove suffix
                            var newVariant = new MAVariant(v);
                            newVariant.RemoveSuffix(suffix);
                            variants.Add(newVariant);

                            // TODO: create new variant and remove suffix with prevowel
                            if (suffix.Prevowel &&
                                newVariant.CurrentText.HasVowel(2) &&
                                newVariant.CurrentText.EndsWithPreVowel())
                            {
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

        private void RemoveSuffixes(MAVariant variant, MAResult result, int level)
        {/*
            //Console.WriteLine($"Lvl {level}. {variant}");

            var commonTypes = context.SearchInDatabase(variant.CurrentText, variant.WordType);
            var alternativeVariants = new List<MAVariant>();

            //Console.WriteLine($"Common types: {string.Join(", ", commonTypes)}");

            // Megvan az adatbázisban
            if (commonTypes.Any())
            {
                if (commonTypes.Contains("IGE") && !variant.IgekotoChecked)
                {
                    // Igekötők ellenőrzése
                    variant.IgekotoChecked = true;
                    var igekotok = context.Words.Where(w => w.Types.Contains("IK") && variant.OriginalText.StartsWith(w.Text)).Select(w => ConvertWordToAffix(w, "IK")).OrderByDescending(a => a.OriginalText.Length);
                    foreach (var igekoto in igekotok)
                    {
                        var newVariant = new MAVariant(variant)
                        {
                            WordType = "IGE"
                        };
                        newVariant.RemovePrefix(igekoto);
                        alternativeVariants.Add(newVariant);
                    }
                }

                //Console.WriteLine($"In database!");

                // Hozzáadjuka megoldásokhoz az elemzéseket
                alternativeVariants.AddRange(commonTypes.Select(t => new MAVariant(variant)
                {
                    WordType = t
                }));
                result.Variants.AddRange(alternativeVariants);
            }
            // Nincs meg az adatbázisban
            else
            {
                if (variant.Suffixes.Any())
                {
                    // Megvizsgáljuk az esetleges tőváltozásokat
                    var stemVariants = StemChecker.CheckStems(variant.CurrentText, variant.WordType);

                    // Visszaállítjuk az eredeti tövet
                    foreach (var stem in stemVariants)
                    {
                        var types = context.SearchInDatabase(stem, variant.WordType);
                        //Console.WriteLine($"{stem}: {string.Join(", ", types)}");
                        if (types.Any())
                        {
                            alternativeVariants.AddRange(types.Select(t => new MAVariant(variant)
                            {
                                CurrentText = stem,
                                WordType = t
                            }));
                        }
                    }
                }

                //Console.WriteLine($"Alternative stems: {string.Join(", ", alternativeVariants.Select(v => v.CurrentText))}");

                if (alternativeVariants.Any())
                {
                    result.Variants.AddRange(alternativeVariants);
                    return;
                }
                else
                {
                    alternativeVariants.Add(variant);
                }
            }
            
            foreach (var currVariant in alternativeVariants)
            {
                //Console.WriteLine($"Variant: {currVariant}");

                foreach (var suffix in currVariant.PossibleSuffixes())
                {
                    //Console.WriteLine($"Suffix: {suffix}");
                    // Levágjuk a toldalékot és meghívjuk újból a függvényt
                    var newVariant = new MAVariant(currVariant);

                    newVariant.RemoveSuffix(suffix);

                    RemoveSuffixes(newVariant, result, level+1);

                    // Előhangzó vizsgálata
                    if (suffix.Prevowel &&
                        newVariant.CurrentText.HasVowel(2) &&
                        newVariant.CurrentText.EndsWithPreVowel())
                    {
                        var preVowel = newVariant.CurrentText.GetLastLetter();
                        //Console.WriteLine($"Prevowel: {preVowel}");
                        var preVowelVariant = new MAVariant(currVariant);

                        preVowelVariant.RemoveSuffix(suffix.GetWithPreVowel(preVowel));

                        RemoveSuffixes(preVowelVariant, result, level+1);
                    }
                }
            }*/
        }

        private Affix ConvertWordToAffix(Word word, string code)
        {
            return new Affix()
            {
                OriginalText = word.Text,
                Text = word.Text,
                Prevowel = false,
                Assimilation = false,
                Info = dbContext.AffixInfos.Find(i => i.Code == code),
                Requirements = new List<string>()
            };
        }
    }
}
