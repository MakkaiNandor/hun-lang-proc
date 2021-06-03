using HLP.Database;
using HLP.Database.Models;
using HLP.WordProcessing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HLP.WordProcessing.Extensions
{
    static class DBAffixExtensions
    {
        public static List<DBAffix> GetPossiblePrefixes(this List<DBAffix> affixes, AnalysisVariant variant)
        {
            return affixes.Where(a =>
                variant.PossiblePrefixTypes.Contains(a.AffixType) &&
                variant.Text.StartsWith(a.AffixText) &&
                variant.Text.Substring(0, variant.Text.Length - a.AffixText.Length).HasVowel() &&
                a.IsCompatibleWith(variant.Type)
            ).ToList();
        }

        public static List<DBAffix> GetPossibleSuffixes(this List<DBAffix> affixes, AnalysisVariant variant)
        {
            return affixes.Where(a =>
                variant.PossibleSuffixTypes.Contains(a.AffixType) &&
                variant.Text.EndsWith(a.AffixText) &&
                variant.Text.Substring(0, variant.Text.Length - a.AffixText.Length).HasVowel() &&
                a.IsCompatibleWith(variant.Type) &&
                variant.IsGood(a)
            ).ToList();
        }

        public static bool IsCompatibleWith(this DBAffix affix, string type)
        {
            bool isCompatible;
            switch (type)
            {
                case "NSZ":
                    isCompatible = affix.WordTypeAfter == "NSZ" || DatabaseContext.Nomens.Contains(affix.WordTypeAfter);
                    break;
                case "":
                    isCompatible = true;
                    break;
                default:
                    isCompatible = affix.WordTypeAfter == type;
                    break;
            }
            return isCompatible;
        }
    }
}
