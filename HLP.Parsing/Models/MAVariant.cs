﻿using HLP.Database;
using HLP.Database.Models;
using HLP.Parsing.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HLP.Parsing.Models
{
    public class MAVariant
    {
        public string OriginalText { get; set; }
        public string CurrentText { get; set; }
        public string WordType { get; set; }
        public List<string> PossiblePrefixTypes { get; }
        public List<string> PossibleSuffixTypes { get; }
        public List<Affix> Prefixes { get; }
        public List<Affix> Suffixes { get; }
        public List<int> ContainedAffixGroups { get; }
        public bool IgekotoChecked { get; set; }

        public MAVariant(string word, string type = "")
        {
            OriginalText = word;
            CurrentText = word;
            WordType = type;
            PossiblePrefixTypes = new List<string> { "P" };
            PossibleSuffixTypes = new List<string> { "K", "J", "R" };
            Prefixes = new List<Affix>();
            Suffixes = new List<Affix>();
            ContainedAffixGroups = new List<int>();
            IgekotoChecked = false;
        }

        public MAVariant(MAVariant other)
        {
            OriginalText = other.OriginalText;
            CurrentText = other.CurrentText;
            WordType = other.WordType;
            PossiblePrefixTypes = new List<string>(other.PossiblePrefixTypes);
            PossibleSuffixTypes = new List<string>(other.PossibleSuffixTypes);
            Prefixes = new List<Affix>(other.Prefixes);
            Suffixes = new List<Affix>(other.Suffixes);
            ContainedAffixGroups = new List<int>(other.ContainedAffixGroups);
            IgekotoChecked = other.IgekotoChecked;
        }

        public List<Affix> PossiblePrefixes()
        {
            /*var context = DatabaseContext.GetInstance().Result;

            var prefixes = context.Affixes.Where(a =>
                PossiblePrefixTypes.Contains(a.Info.Type) &&
                !ContainedAffixGroups.Contains(a.Info.Group) &&
                AreCompatibles(a.Info.WordTypeAfter, WordType) &&
                CurrentText.StartsWith(a.Text) &&
                CurrentText.RemoveFromStart(a.Text).HasVowel() 
            ).OrderByDescending(a => a.Text.Length).ToList();

            //Console.WriteLine($"Prefixes: {prefixes.Count}");

            return prefixes;*/
            return null;
        }

        public List<Affix> PossibleSuffixes()
        {
            /*var context = DatabaseContext.GetInstance();

            var suffixes = context.Affixes.Where(a =>
                PossibleSuffixTypes.Contains(a.Info.Type) &&
                !ContainedAffixGroups.Contains(a.Info.Group) &&
                AreCompatibles(a.Info.WordTypeAfter, WordType) &&
                (CurrentText.EndsWith(a.Text) ?
                CurrentText.RemoveFromEnd(a.Text).HasVowel() :
                (a.Assimilation && CheckAssimilation(a.Text)))
            ).OrderByDescending(a => a.Text.Length).ToList();

            //Console.WriteLine($"Suffixes: {suffixes.Count}");

            return suffixes;*/
            return null;
        }

        private bool CheckAssimilation(string affix)
        {
            var remainder = affix.Remove(0, 1);
            if (!CurrentText.EndsWith(remainder)) return false;
            var word = CurrentText.RemoveFromEnd(remainder);
            return word.EndsWithLongConsonant() && word.HasVowel();
        }

        // Két szófaj kompatibilis-e egymással
        private bool AreCompatibles(string type1, string type2)
        {
            /*if (type2.Length == 0) return true;

            var context = DatabaseContext.GetInstance();
            var types1 = context.GetCompatibleWordTypes(type1);
            var types2 = context.GetCompatibleWordTypes(type2);
            
            return types1.Intersect(types2).Any();*/
            return false;
        }

        public void RemovePrefix(Affix prefix)
        {
            /*OriginalText = CurrentText = CurrentText.RemoveFromStart(prefix.OriginalText);
            Prefixes.Add(prefix);
            if (prefix.Info.Group != 0)
            {
                ContainedAffixGroups.Add(prefix.Info.Group);
            }*/
        }

        public void RemoveSuffix(Affix suffix)
        {
            /*if (CurrentText.EndsWith(suffix.OriginalText))
            {
                OriginalText = CurrentText = CurrentText.RemoveFromEnd(suffix.OriginalText);
                Suffixes.Insert(0, suffix);
            }
            else
            {
                // Hasonulás
                var remainder = suffix.Text.Remove(0, 1);
                var word = CurrentText.RemoveFromEnd(remainder);
                if (!word.EndsWithLongConsonant() || !word.HasVowel()) return;
                var longConsonant = word.GetLastLetter();
                var shortConsonant = longConsonant.Remove(0, 1);
                OriginalText = CurrentText = word.RemoveFromEnd(longConsonant) + shortConsonant;
                Suffixes.Insert(0, new Affix(suffix)
                {
                    OriginalText = shortConsonant + remainder
                });
            }
            WordType = suffix.Info.WordTypeBefore;
            switch (suffix.Info.Type)
            {
                case "R":
                    // Rag előtt több rag nem lehet, csak jel vagy képző
                    PossibleSuffixTypes.Remove("R");
                    break;
                case "J":
                    // Jel előtt állhat jel vagy képző, ige esetén csak képző
                    PossibleSuffixTypes.Remove("R");
                    if (WordType == "IGE")
                    {
                        PossibleSuffixTypes.Remove("J");
                    }
                    break;
                case "K":
                    // Képző előtt csak képző állhat
                    PossibleSuffixTypes.Remove("R");
                    PossibleSuffixTypes.Remove("J");
                    break;
                default: break;
            }
            if (suffix.Info.Group != 0)
            {
                ContainedAffixGroups.Add(suffix.Info.Group);
            }*/
        }

        public string GetMorphCode()
        {
            var fromPrefixes = string.Join("", Prefixes.Select(p => p.Info.Code + "."));
            var fromSuffixes = string.Join("", Suffixes.Select(s => "." + s.Info.Code));
            return fromPrefixes + WordType + fromSuffixes;
        }

        public override string ToString()
        {
            return $"{(Prefixes.Any() ? $"{string.Join(" + ", Prefixes)} + " : null)}{OriginalText}({WordType}){(CurrentText != OriginalText ? $"={CurrentText}" : null)}{(Suffixes.Any() ? $" + {string.Join(" + ", Suffixes)}" : null)}\n\t{GetMorphCode()}";
        }
    }
}
