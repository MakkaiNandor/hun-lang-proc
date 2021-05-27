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

            Console.WriteLine("\n~~~~~ Kimenet ~~~~~");

            foreach (var word in inputText.SplitToWords())
            {
                //Console.WriteLine($"{++count}. {word}");

                var analysisResult = new AnalysisResult(word);

                var variant = new AnalysisVariant(word);

                AnalyzeOneWord(variant, analysisResult);

                result.Add(analysisResult);

                Console.WriteLine(analysisResult);

                //this.SplitCompoundWord(word, count);
            }

            Console.WriteLine("\n~~~~~ Vége ~~~~~");

            //Console.WriteLine(string.Join("\n", result));

            return result;
        }

        private void AnalyzeOneWord(AnalysisVariant variant, AnalysisResult result)
        {
            if (DBContext.Words.Any(w => 
                w.WordText == variant.Stem && 
                (variant.Type == "NSZ" ? 
                w.WordTypes.Intersect(DatabaseContext.Nomens).Count() > 0 : 
                (variant.Type == "" || w.WordTypes.Contains(variant.Type)))
            ))
            {
                //Console.WriteLine($"\tEredmény: {variant}");
                result.Variants.Add(variant);
            }

            //var prefixes = DBContext.Affixes.GetPossiblePrefixes(variant);
            var suffixes = DBContext.Affixes.GetPossibleSuffixes(variant);

            //Console.WriteLine($"{variant} ~~ ({string.Join(", ", variant.PossiblePrefixTypes)}) ~~ {string.Join(", ", prefixes)}");
            //Console.WriteLine($"{variant} ~~ ({string.Join(", ", variant.PossibleSuffixTypes)}) ~~ {string.Join(", ", suffixes)}");

            /*foreach (var prefix in prefixes)
            {
                var newVariant = new AnalysisVariant(variant);
                newVariant.RemovePrefix(prefix);
                AnalyzeOneWord(newVariant, result);
            }*/

            foreach (var suffix in suffixes)
            {
                var newVariant = new AnalysisVariant(variant);
                newVariant.RemoveSuffix(suffix);
                AnalyzeOneWord(newVariant, result);
            }

            // TODO: Igekötő és/vagy prefixum eltávolítása
            // TODO: Rag eltávolítása
            // TODO: Jelzők eltávolítása
            // TODO: Képzők eltávolítása
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
    }
}
