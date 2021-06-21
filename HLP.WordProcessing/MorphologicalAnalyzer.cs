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

        public void AnalyzeText(string text)
        {
            if (text.Length == 0)
            {
                return;
            }
        }

        public List<AnalysisResult> Analyze(string inputText, bool print = false)
        {
            var result = new List<AnalysisResult>();

            if (inputText.Length == 0)
            {
                return result;
            }

            if (print) Console.WriteLine("\n~~~~~ Kimenet ~~~~~");

            foreach (var word in inputText.SplitToWords())
            {
                //Console.WriteLine($"{++count}. {word}");

                var analysisResult = new AnalysisResult(word);

                var variant = new AnalysisVariant(word);

                AnalyzeOneWord(variant, analysisResult, 1);

                result.Add(analysisResult);

                //Console.WriteLine(analysisResult);

                //this.SplitCompoundWord(word, count);
            }

            if (print)
            {
                Console.WriteLine("\n~~~~~ Vége ~~~~~");
                Console.WriteLine(string.Join("\n", result));
            }

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
                /*foreach (var v in variants)
                {
                    Console.WriteLine($"{level}. {v}");
                }*/
            }
            /*else
            {
                variants.Add(variant);
            }*/
            else
            {
                SearchCorrectStem(variant).ForEach(v =>
                {
                    variants.AddRange(DBContext.Words.GetCommonTypes(v).Select(t => new AnalysisVariant(v)
                    {
                        Type = t
                    }));
                });
                if (variants.Any())
                {
                    result.Variants.AddRange(variants);
                    //return true;
                }
                else
                {
                    variants.Add(variant);
                }
            }

            var tmp = false;

            foreach (var v in variants)
            {
                /*var prefixes = DBContext.Affixes.GetPossiblePrefixes(v);
                var prefixIsGood = false;
                Console.WriteLine($"{level}p. {string.Join(", ", prefixes)}");
                foreach (var prefix in prefixes)
                {
                    var newVariant = new AnalysisVariant(v);
                    newVariant.RemovePrefix(prefix);
                    prefixIsGood = AnalyzeOneWord(newVariant, result, level + 1);
                }

                if (prefixIsGood)
                {
                    inDB = true;
                    continue;
                }*/

                var suffixes = DBContext.Affixes.GetPossibleSuffixes(v);

                //Console.WriteLine($"{level}. {string.Join(", ", suffixes)}");

                foreach (var suffix in suffixes)
                {
                    var newVariant = new AnalysisVariant(v);
                    newVariant.RemoveSuffix(suffix);
                    //Console.WriteLine("new" + newVariant);

                    var tmp1 = AnalyzeOneWord(newVariant, result, level + 1);
                    tmp = tmp1 || tmp;

                    if (!tmp1 &&
                        suffix.Prevowel &&
                        newVariant.Text.EndsWithPreVowel() &&
                        newVariant.Text.NumberOfVowels() >= 2)
                    {
                        var preVowelVariant = new AnalysisVariant(v);
                        preVowelVariant.RemoveSuffix(new Affix(suffix, newVariant.Text.Last().ToString()));
                        tmp = AnalyzeOneWord(preVowelVariant, result, level + 1) || tmp;
                        //Console.WriteLine("pre" + preVowelVariant);
                    }
                }
            }

            /*if (!inDB && !tmp)
            {
                var list = SearchCorrectStem(variant);
                if (list.Any())
                {
                    result.Variants.AddRange(list);
                    inDB = true;
                }
            }*/

            return inDB;
        }

        private List<AnalysisVariant> SearchCorrectStem(AnalysisVariant variant)
        {
            var result = new List<AnalysisVariant>();

            if (variant.Type == "IGE")
            {
                var res = StemChecker.CheckVerbStem(variant.Text).Where(s => !s.EndsWith(variant.Suffixes[0].Text)).ToList();
                //Console.WriteLine($"{variant.Text}: {string.Join(", ", res)}");
                res.ForEach(s =>
                {
                    result.Add(new AnalysisVariant(variant)
                    {
                        Text = s
                    });
                });
            }
            else
            {
                var res = StemChecker.CheckNomenStem(variant.Text).Where(s => !s.EndsWith(variant.Suffixes[0].Text)).ToList();
                //Console.WriteLine($"{variant.Text}: {string.Join(", ", res)}");
                res.ForEach(s =>
                {
                    result.Add(new AnalysisVariant(variant)
                    {
                        Text = s
                    });
                });
            }
            /*else
            {
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
                if (variant.Text.EndsWithConsonant() || variant.Text.EndsWithLongConsonant())
                {
                    result.AddRange(Alphabet.Vowels.Select(v => new AnalysisVariant(variant)
                    {
                        Text = variant.Text + v
                    }));
                }
            }*/

            return result;//.Where(v => DBContext.Words.GetCommonTypes(v).Any()).ToList();
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
                    w.Text.Length > 1 &&
                    component.Contains(w.Text)
                ).OrderByDescending(w => w.Text.Length).ToList();

                Console.WriteLine($"\t{index}.{++count}. {component} ({string.Join(", ", containedWords.Select(w => w.Text))})");

                this.SearchForComponents(component, containedWords, new List<string>());
            }

            return result;
        }

        private void SearchForComponents(string word, List<Word> containedWords, List<string> components)
        {
            if(word.Length == 0)
            {
                Console.WriteLine($"\t\t{string.Join(", ", components)}");
                return;
            }
            foreach (var startWord in containedWords.Where(w => word.StartsWith(w.Text)))
            {
                var extendedComponents = new List<string>(components);
                var newWord = word.Substring(startWord.Text.Length);
                extendedComponents.Add(startWord.Text);
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
