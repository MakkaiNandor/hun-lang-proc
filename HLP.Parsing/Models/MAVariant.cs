using HLP.Database;
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
        }

        // copy konstruktor
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
        }

        // levágható prefixumok
        public List<Affix> PossiblePrefixes()
        {
            var prefixes = DatabaseContext.GetInstance().Affixes.Where(a =>
                    PossiblePrefixTypes.Contains(a.Info.Type) &&
                    !ContainedAffixGroups.Contains(a.Info.GroupNumber) &&
                    AreCompatibles(a.Info.WordTypeAfter, WordType) &&
                    CurrentText.StartsWith(a.Text) &&
                    CurrentText.RemoveFromStart(a.Text).HasVowel(1, 1)
            ).OrderByDescending(a => a.Text.Length).ToList();

            //Console.WriteLine($"Prefixes: {prefixes.Count}");

            return prefixes;
        }

        // levágható szuffixumok
        public List<Affix> PossibleSuffixes()
        {
            var suffixes = DatabaseContext.GetInstance().Affixes.Where(a =>
                    PossibleSuffixTypes.Contains(a.Info.Type) &&
                    !ContainedAffixGroups.Contains(a.Info.GroupNumber) &&
                    AreCompatibles(a.Info.WordTypeAfter, WordType) &&
                    (CurrentText.EndsWith(a.Text) ?
                    CurrentText.RemoveFromEnd(a.Text).HasVowel(1, 1) :
                    (a.Assimilation && CheckAssimilation(a.Text)))
            ).OrderByDescending(a => a.Text.Length).ToList();

            //Console.WriteLine($"Suffixes: {suffixes.Count}");

            return suffixes;
        }

        // prefixum levágása
        public void RemovePrefix(Affix prefix)
        {
            OriginalText = CurrentText = CurrentText.RemoveFromStart(prefix.OriginalText);
            Prefixes.Add(prefix);
            if (prefix.Info.GroupNumber != 0)
            {
                ContainedAffixGroups.Add(prefix.Info.GroupNumber);
            }
            PossiblePrefixTypes.Remove(prefix.Info.Type);
        }

        // szuffixum levágása
        public void RemoveSuffix(Affix suffix)
        {
            if (CurrentText.EndsWith(suffix.OriginalText))
            {
                OriginalText = CurrentText = CurrentText.RemoveFromEnd(suffix.OriginalText);
                Suffixes.Insert(0, suffix);
            }
            else
            {
                // Hasonulás ellenőrzése
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
            if (suffix.Info.GroupNumber != 0)
            {
                ContainedAffixGroups.Add(suffix.Info.GroupNumber);
            }
        }

        // hasonulás ellenőrzése
        private bool CheckAssimilation(string affix)
        {
            var remainder = affix.Remove(0, 1);
            if (!CurrentText.EndsWith(remainder)) return false;
            var word = CurrentText.RemoveFromEnd(remainder);
            return word.EndsWithLongConsonant() && word.HasVowel();
        }

        // két szófaj összeférhetőségének ellenőrzése
        private bool AreCompatibles(string type1, string type2)
        {
            if (type2.Length == 0) return true;

            var context = DatabaseContext.GetInstance();
            var types1 = context.GetCompatibleWordTypes(type1);
            var types2 = context.GetCompatibleWordTypes(type2);

            return types1.Intersect(types2).Any();
        }

        // összehasonlítás
        public bool Equals(MAVariant other)
        {
            if (WordType != other.WordType ||
                OriginalText != other.OriginalText ||
                CurrentText != other.CurrentText ||
                Prefixes.Count != other.Prefixes.Count ||
                Suffixes.Count != other.Suffixes.Count)
                return false;

            for (var i = 0; i < Prefixes.Count; ++i)
            {
                if (!Prefixes[i].Equals(other.Prefixes[i]))
                    return false;
            }

            for (var i = 0; i < Suffixes.Count; ++i)
            {
                if (!Suffixes[i].Equals(other.Suffixes[i]))
                    return false;
            }

            return true;
        }

        // morfológiai kód
        public string GetMorphCode()
        {
            var fromPrefixes = string.Join("", Prefixes.Select(p => p.Info.Code + "."));
            var fromSuffixes = string.Join("", Suffixes.Select(s => "." + s.Info.Code));
            return fromPrefixes + WordType + fromSuffixes;
        }
    }
}
