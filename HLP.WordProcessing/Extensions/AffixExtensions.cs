using HLP.Database;
using HLP.Database.Models;
using HLP.WordProcessing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HLP.WordProcessing.Extensions
{
    static class AffixExtensions
    {
        // A lehetséges prefixumok, amelyek levághatók a szó elejéről
        public static List<Affix> GetPossiblePrefixes(this List<Affix> affixes, AnalysisVariant variant)
        {
            return affixes.Where(a =>
                variant.CurrentText.StartsWith(a.Text) &&
                variant.CurrentText.Substring(0, variant.CurrentText.Length - a.Text.Length).HasVowel() &&
                a.IsCompatibleWith(variant.WordType) &&
                (a.Info.Group == 0 || !variant.ContainedAffixGroups.Contains(a.Info.Group))
            ).ToList();
        }

        // A lehetséges szuffixumok, amelyek levághatók a szó végéről
        public static List<Affix> GetPossibleSuffixes(this List<Affix> affixes, AnalysisVariant variant)
        {
            return affixes.Where(a =>
                variant.PossibleSuffixTypes.Contains(a.Info.Type) &&
                variant.CurrentText.EndsWith(a.Text) &&
                variant.CurrentText.Substring(0, variant.CurrentText.Length - a.Text.Length).HasVowel() &&
                a.IsCompatibleWith(variant.WordType) &&
                (a.Info.Group == 0 || !variant.ContainedAffixGroups.Contains(a.Info.Group))
            ).ToList();
        }

        // Kompatibilis-e a szó szófaja a toldalékkal
        public static bool IsCompatibleWith(this Affix affix, string type)
        {
            var context = DatabaseContext.GetInstance();

            var types = context.GetCompatibleWordTypes(type);

            return types.Count == 0 || types.Contains(affix.Info.WordTypeAfter);
        }
    }
}
