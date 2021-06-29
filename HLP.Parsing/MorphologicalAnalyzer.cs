using HLP.Database;
using HLP.Database.Models;
using HLP.Parsing.Extensions;
using HLP.Parsing.Models;
using HLP.Parsing.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            /*foreach (var prefix in variant.PossiblePrefixes())
            {
                var newVariant = new MAVariant(variant);
                newVariant.RemovePrefix(prefix);
                RemoveSuffixes(newVariant, result, 1);
            }*/

            Console.WriteLine("Before remove suffixes");

            // Szuffixumok levágása
            RemoveSuffixes(variant, result, 1);

            Console.WriteLine("After remove suffixes");

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

        private void RemoveSuffixes(MAVariant variant, MAResult result)
        {
            /*Console.WriteLine("Start remove suffixes");

            var variants = new List<MAVariant> { variant };

            while (true)
            {
                Console.WriteLine($"Variants:\n{string.Join("\n\t", variants)}");
                if (!variants.Any()) break;
                var variantList = new List<MAVariant>(variants);
                foreach (var currVariant in variantList)
                {
                    Console.WriteLine($"CurrVariant: {currVariant}");
                    // TODO: search in db
                    var alternativeVariants = new List<MAVariant>();
                    var commonTypes = context.SearchInDatabase(currVariant.CurrentText, currVariant.WordType);
                    Console.WriteLine($"Common types: {string.Join(", ", commonTypes)}");

                    // TODO: if in db, get common types and create new variants
                    if (commonTypes.Any())
                    {
                        Console.WriteLine($"In database!");
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
                        Console.WriteLine($"Not in database!");
                        if (currVariant.Suffixes.Any())
                        {
                            var stemVariants = StemChecker.CheckStems(currVariant.CurrentText, currVariant.WordType);

                            foreach (var stem in stemVariants)
                            {
                                var types = context.SearchInDatabase(stem, variant.WordType);

                                Console.WriteLine($"{stem}: {string.Join(", ", types)}");

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

                    Console.WriteLine($"AltVariants:\n{string.Join("\n\t", alternativeVariants)}");

                    // TODO: for every variant in altenative variants search possible suffixes
                    foreach (var v in alternativeVariants)
                    {
                        Console.WriteLine($"AltVariant: {v}");
                        var removableSuffixes = v.PossibleSuffixes();

                        if (!removableSuffixes.Any())
                        {
                            variants.Remove(v);
                        }

                        Console.WriteLine($"Removable suffixes: {string.Join(", ", removableSuffixes)}");

                        // TODO: for every suffix in possible suffixes:
                        foreach (var suffix in removableSuffixes)
                        {
                            // TODO: create new variant and remove suffix
                            var newVariant = new MAVariant(v);
                            newVariant.RemoveSuffix(suffix);
                            Console.WriteLine($"{suffix} removed: {newVariant}");
                            variants.Add(newVariant);

                            // TODO: create new variant and remove suffix with prevowel
                            if (suffix.Prevowel &&
                                newVariant.CurrentText.HasVowel(2) &&
                                newVariant.CurrentText.EndsWithPreVowel())
                            {
                                var preVowel = newVariant.CurrentText.GetLastLetter();
                                var preVowelVariant = new MAVariant(v);
                                preVowelVariant.RemoveSuffix(suffix.GetWithPreVowel(preVowel));
                                Console.WriteLine($"{suffix} removed with prevowel: {preVowelVariant}");
                                variants.Add(preVowelVariant);
                            }
                        }
                    }
                    variants.Remove(currVariant);
                }
            }

            Console.WriteLine("End remove suffixes");*/
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
                Info = /*context.AffixInfos.Find(i => i.Code == code)*/null,
                Requirements = new List<string>()
            };
        }
    }
}
