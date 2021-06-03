using HLP.Database;
using HLP.Database.Models;
using HLP.WordProcessing.Extensions;
using HLP.WordProcessing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HLP.WordProcessing
{
    public class MorphologicalAnalyzer
    {
        private readonly DatabaseContext DBContext;

        public MorphologicalAnalyzer()
        {
            DBContext = DatabaseContext.GetDatabaseContext();
        }

        public List<AnalysisResult> Analyze(string inputText)
        {
            var result = new List<AnalysisResult>();

            if (inputText.Length == 0)
            {
                return result;
            }

            Console.WriteLine("\n~~~~~ Kimenet ~~~~~");

            foreach (var word in inputText.SplitToWords())
            {
                //Console.WriteLine($"{++count}. {word}");

                var analysisResult = new AnalysisResult(word);

                var variant = new AnalysisVariant(word);

                AnalyzeOneWord(variant, analysisResult, 1);

                result.Add(analysisResult);

                Console.WriteLine(analysisResult);

                //this.SplitCompoundWord(word, count);
            }

            Console.WriteLine("\n~~~~~ Vége ~~~~~");

            //Console.WriteLine(string.Join("\n", result));

            return result;
        }

        private bool AnalyzeOneWord(AnalysisVariant variant, AnalysisResult result, int level)
        {
            var commonTypes = DBContext.Words.GetCommonTypes(variant);
            var variants = new List<AnalysisVariant>();
            var inDB = false;

            if (commonTypes.Any())
            {
                inDB = true;
                variants.AddRange(commonTypes.Select(t => new AnalysisVariant(variant) { 
                    Type = t
                }));
                result.Variants.AddRange(variants);
                foreach (var v in variants)
                {
                    Console.WriteLine($"{level}. {v}");
                }
            }
            else
            {
                variants.AddRange(SearchCorrectStem(variant));
                if (variants.Any())
                {
                    result.Variants.AddRange(variants);
                    return true;
                }
                else
                {
                    variants.Add(variant);
                }
            }

            foreach (var v in variants)
            {
                var prefixes = DBContext.Affixes.GetPossiblePrefixes(v);

                foreach (var prefix in prefixes)
                {
                    var newVariant = new AnalysisVariant(v);
                    newVariant.RemovePrefix(prefix);
                    AnalyzeOneWord(newVariant, result, level + 1);
                }

                var suffixes = DBContext.Affixes.GetPossibleSuffixes(v);

                Console.WriteLine($"{level}. {string.Join(", ", suffixes)}");

                foreach (var suffix in suffixes)
                {
                    var newVariant = new AnalysisVariant(v);
                    newVariant.RemoveSuffix(suffix);
                    //Console.WriteLine("new" + newVariant);

                    if (!AnalyzeOneWord(newVariant, result, level + 1) &&
                        !suffix.AffixText.StartsWithVowel() &&
                        newVariant.Text.EndsWithPreVowel() &&
                        newVariant.Text.HasTwoVowelsAtLeast())
                    {
                        var preVowelVariant = new AnalysisVariant(v);
                        preVowelVariant.RemoveSuffix(new DBAffix(suffix, newVariant.Text.Last().ToString()));
                        AnalyzeOneWord(preVowelVariant, result, level + 1);
                        //Console.WriteLine("pre" + preVowelVariant);
                    }
                }
            }
            return inDB;
        }

        private List<AnalysisVariant> SearchCorrectStem(AnalysisVariant variant)
        {
            var result = new List<AnalysisVariant>();

            // Szóvégi magánhangzó hosszabbodik
            if (variant.Text.EndsWithLongVowel())
            {
                result.Add(new AnalysisVariant(variant)
                {
                    Text = variant.Text.ReplaceVowel(variant.Text.Length - 1)
                });
            }

            // A magánhangzók rövidülnek
            else if (variant.Text.HasShortVowel())
            {
                var permutations = Permute(variant.Text.IndexOfShortVowels());

                foreach (var perm in permutations)
                {
                    var newText = variant.Text;

                    foreach (var index in perm)
                    {
                        newText = newText.ReplaceVowel(index);
                    }

                    result.Add(new AnalysisVariant(variant)
                    {
                        Text = newText
                    });
                }
            }

            // Hiányzik az utolsó magánhangzó (két mássalhangzóban végződik)
            if (variant.Text.EndsWithTwoConsonants())
            {
                var temp = new List<AnalysisVariant>();
                foreach (var v in result)
                {
                    temp.AddRange(Alphabet.Vowels.Select(l => new AnalysisVariant(v)
                    {
                        Text = v.Text.Insert(v.Text.Length - 1, l)
                    }));
                }
                result.AddRange(temp);
                result.AddRange(Alphabet.Vowels.Select(v => new AnalysisVariant(variant)
                {
                    Text = variant.Text.Insert(variant.Text.Length - 1, v)
                }));
            }

            // Hiányzik a szóvégi magánhangzó
            else if(variant.Text.EndsWithConsonant() || variant.Text.EndsWithLongConsonant())
            {

            }

            return result.Where(v => DBContext.Words.GetCommonTypes(v).Any()).ToList();
        }

        private List<string> SplitCompoundWord(string word, int index)
        {
            var result = new List<string>();
            var components = word.Split('-').ToList();

            if (components.Count == 0)
            {
                components.Add(word);
            }

            var count = 0;

            foreach(var component in components)
            {
                var containedWords = DBContext.Words.Where(w => 
                    w.WordText.Length > 1 &&
                    component.Contains(w.WordText)
                ).OrderByDescending(w => w.WordText.Length).ToList();

                Console.WriteLine($"\t{index}.{++count}. {component} ({string.Join(", ", containedWords.Select(w => w.WordText))})");

                this.SearchForComponents(component, containedWords, new List<string>());
            }

            return result;
        }

        private void SearchForComponents(string word, List<DBWord> containedWords, List<string> components)
        {
            if(word.Length == 0)
            {
                Console.WriteLine($"\t\t{string.Join(", ", components)}");
                return;
            }
            foreach (var startWord in containedWords.Where(w => word.StartsWith(w.WordText)))
            {
                var extendedComponents = new List<string>(components);
                var newWord = word.Substring(startWord.WordText.Length);
                extendedComponents.Add(startWord.WordText);
                this.SearchForComponents(newWord, containedWords, extendedComponents);
            }
        }

        private List<List<int>> Permute(List<int> ls)
        {
            var result = new List<List<int>>();
            int k = 0;

            while (k < ls.Count)
            {
                for (var i = k; i < ls.Count; ++i)
                {
                    result.Add(new List<int>(ls.GetRange(0, k))
                    {
                        ls[i]
                    });
                }
                ++k;
            }

            return result;
        }
    }
}
