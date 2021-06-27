using HLP.Database;
using HLP.Database.Models;
using HLP.Parsing.Extensions;
using HLP.Parsing.Models;
using HLP.Parsing.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HLP.Parsing
{
    public class MorphologicalAnalyzer
    {
        private readonly DatabaseContext context;
        private readonly Tokenizer tokenizer;

        public MorphologicalAnalyzer()
        {
            context = DatabaseContext.GetInstance();
            tokenizer = new Tokenizer();
        }

        public List<MAResult> AnalyzeText(string text, bool print = true)
        {
            var result = new List<MAResult>();

            if (text.Length == 0)
            {
                return result;
            }

            var words = tokenizer.GetWords(text);

            if (print)
            {
                Console.WriteLine("~~~~~~~~~~ Words ~~~~~~~~~~");
                Console.WriteLine(string.Join("\n", words));
            }

            foreach (var word in words)
            {
                result.Add(AnalyzeWord(word, print));
            }

            return result;
        }

        public MAResult AnalyzeWord(string word, bool print = true)
        {
            var result = new MAResult(word);
            word = word.ToLower();

            if (print)
            {
                Console.WriteLine("~~~~~~~~~~ Start ~~~~~~~~~~");
                Console.WriteLine($"Analyzing word '{word}'!");
            }

            var variant = new MAVariant(word);

            // Prefixumok levágása
            foreach (var prefix in variant.PossiblePrefixes())
            {
                var newVariant = new MAVariant(variant);
                newVariant.RemovePrefix(prefix);
                RemoveSuffixes(newVariant, result, 1);
            }

            // Szuffixumok levágása
            RemoveSuffixes(variant, result, 1);

            // Eredmények szűrése, redukálása
            var reducer = new ResultReducer();
            reducer.ReduceResults(result);

            if (print)
            {
                Console.WriteLine($"Results:\n\t{string.Join("\n\t", result.Variants)}");
                Console.WriteLine("~~~~~~~~~~  End  ~~~~~~~~~~");
            }

            return result;
        }

        private void RemoveSuffixes(MAVariant variant, MAResult result, int level)
        {
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
                    int nrRes = result.Variants.Count;
                    //Console.WriteLine($"Suffix: {suffix}");
                    // Levágjuk a toldalékot és meghívjuk újból a függvényt
                    var newVariant = new MAVariant(currVariant);

                    newVariant.RemoveSuffix(suffix);

                    RemoveSuffixes(newVariant, result, level+1);

                    // Előhangzó vizsgálata
                    if (suffix.Prevowel &&
                        result.Variants.Count == nrRes &&
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
            }
        }

        private Affix ConvertWordToAffix(Word word, string code)
        {
            return new Affix()
            {
                OriginalText = word.Text,
                Text = word.Text,
                Prevowel = false,
                Assimilation = false,
                Info = context.AffixInfos.Find(i => i.Code == code),
                Requirements = new List<string>()
            };
        }
    }
}
