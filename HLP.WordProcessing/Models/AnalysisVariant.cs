﻿using HLP.Database;
using HLP.Database.Models;
using HLP.WordProcessing.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HLP.WordProcessing.Models
{
    public class AnalysisVariant
    {
        public string OriginalText { get; set; }
        public string CurrentText { get; set; }
        public string WordType { get; set; }
        public List<string> PossiblePrefixTypes { get; }
        public List<string> PossibleSuffixTypes { get; }
        public List<Affix> Prefixes { get; }
        public List<Affix> Suffixes { get; }
        public List<int> ContainedAffixGroups { get; }

        public AnalysisVariant(string word, string type = "")
        {
            OriginalText = word;
            CurrentText = word;
            WordType = type;
            PossiblePrefixTypes = new List<string> { "P" };
            PossibleSuffixTypes = new List<string> { "K", "J", "R" };
            Prefixes = new List<Affix>();
            Suffixes = new List<Affix>();
            ContainedAffixGroups = new List<int>();
        }

        public AnalysisVariant(AnalysisVariant other)
        {
            OriginalText = other.OriginalText;
            CurrentText = other.CurrentText;
            WordType = other.WordType;
            PossiblePrefixTypes = new List<string>(other.PossiblePrefixTypes);
            PossibleSuffixTypes = new List<string>(other.PossibleSuffixTypes);
            Prefixes = new List<Affix>(other.Prefixes);
            Suffixes = new List<Affix>(other.Suffixes);
            ContainedAffixGroups = new List<int>(other.ContainedAffixGroups);
        }

        public List<Affix> PossiblePrefixes()
        {
            return DatabaseContext.GetInstance().Affixes.Where(a => 
                PossiblePrefixTypes.Contains(a.Info.Type) &&
                CurrentText.StartsWith(a.Text) &&
                CurrentText.RemoveFromStart(a.Text).HasVowel() &&
                AreCompatibles(a.Info.WordTypeAfter, WordType) &&
                (a.Info.Group == 0 || !ContainedAffixGroups.Contains(a.Info.Group))
            ).ToList();
        }

        public List<Affix> PossibleSuffixes()
        {
            return DatabaseContext.GetInstance().Affixes.Where(a =>
                PossibleSuffixTypes.Contains(a.Info.Type) &&
                CurrentText.EndsWith(a.Text) &&
                CurrentText.RemoveFromEnd(a.Text).HasVowel() &&
                AreCompatibles(a.Info.WordTypeAfter, WordType) &&
                (a.Info.Group == 0 || !ContainedAffixGroups.Contains(a.Info.Group))
            ).ToList();
        }

        // Két szófaj kompatibilis-e egymással
        private bool AreCompatibles(string type1, string type2)
        {
            var types = DatabaseContext.GetInstance().GetCompatibleWordTypes(type1);

            return types.Count == 0 || types.Contains(type2);
        }

        public void RemovePrefix(Affix prefix)
        {
            OriginalText = CurrentText = CurrentText.RemoveFromStart(prefix.Text);
            Prefixes.Add(prefix);
            if (prefix.Info.Group != 0)
            {
                ContainedAffixGroups.Add(prefix.Info.Group);
            }
        }

        public void RemoveSuffix(Affix suffix)
        {
            OriginalText = CurrentText = CurrentText.RemoveFromEnd(suffix.Text);
            WordType = suffix.Info.WordTypeBefore;
            Suffixes.Insert(0, suffix);
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
            }
        }

        public string GetMorphCode()
        {
            var result = new List<string>();

            foreach (var prefix in Prefixes)
            {
                result.Add(prefix.Info.Code);
            }

            result.Add(WordType);

            foreach(var suffix in Suffixes)
            {
                result.Add(suffix.Info.Code);
            }

            return string.Join(".", result);
        }

        public override string ToString()
        {
            return $"\t{(Prefixes.Any() ? $"{string.Join(" + ", Prefixes)} + " : null)}{CurrentText}({WordType}){(CurrentText != OriginalText ? $"={OriginalText}" : null)}{(Suffixes.Any() ? $" + {string.Join(" + ", Suffixes)}" : null)}\n\t{GetMorphCode()}";
        }
    }
}
