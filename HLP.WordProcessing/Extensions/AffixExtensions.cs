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
        public static List<Affix> GetPossiblePrefixes(this List<Affix> affixes, AnalysisVariant variant)
        {
            return affixes.Where(a =>
                variant.PossiblePrefixTypes.Contains(a.Code.Type) &&
                variant.Text.StartsWith(a.Text) &&
                variant.Text.Substring(0, variant.Text.Length - a.Text.Length).HasVowel() &&
                a.IsCompatibleWith(variant.Type)
            ).ToList();
        }

        public static List<Affix> GetPossibleSuffixes(this List<Affix> affixes, AnalysisVariant variant)
        {
            return affixes.Where(a =>
                variant.PossibleSuffixTypes.Contains(a.Code.Type) &&
                variant.Text.EndsWith(a.Text) &&
                variant.Text.Substring(0, variant.Text.Length - a.Text.Length).HasVowel() &&
                a.IsCompatibleWith(variant.Type) &&
                variant.IsGood(a)
            ).ToList();
        }

        public static bool IsCompatibleWith(this Affix affix, string type)
        {
            var DB = DatabaseContext.GetDatabaseContext();

            var types = DB.GetCompatibleWordTypes(type);

            return types.Count == 0 || types.Contains(affix.Code.WordTypeAfter);
        }
    }
}
