using HLP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HLP.WordProcessing.Models
{
    public class AnalysisVariant
    {
        public string Stem { get; private set; }
        public string Type { get; private set; }
        public List<DBAffix> Prefixes { get; }
        public List<DBAffix> Suffixes { get; }

        public AnalysisVariant(string word, string type = "")
        {
            Stem = word;
            Type = type;
            Prefixes = new List<DBAffix>();
            Suffixes = new List<DBAffix>();
        }

        public AnalysisVariant(AnalysisVariant other)
        {
            Stem = other.Stem;
            Type = other.Type;
            Prefixes = new List<DBAffix>(other.Prefixes);
            Suffixes = new List<DBAffix>(other.Suffixes);
        }

        public List<string> GetPossiblePrefixTypes()
        {
            var result = new List<string>();
            var prefixTypes = Prefixes.Select(a => a.AffixType).ToList();
            var suffixTypes = Suffixes.Select(a => a.AffixType).ToList();
            if (!prefixTypes.Contains("P"))
            {
                result.Add("P");
            }
            if (!prefixTypes.Contains("I"))
            {
                result.Add("I");
            }
            return result;
        }

        public List<string> GetPossibleSuffixTypes()
        {
            var result = new List<string>();
            var prefixTypes = Prefixes.Select(a => a.AffixType).ToList();
            var suffixTypes = Suffixes.Select(a => a.AffixType).ToList();
            if (!suffixTypes.Contains("R"))
            {
                result.Add("R");
            }
            if (!suffixTypes.Contains("R") && !(Type == "IGE" && suffixTypes.Contains("J")))
            {
                result.Add("J");
            }
            if (!suffixTypes.Contains("J") && !suffixTypes.Contains("R"))
            {
                result.Add("K");
            }
            return result;
        }

        public void RemovePrefix(DBAffix prefix)
        {
            Stem = Stem.Substring(prefix.AffixText.Length);
            Type = prefix.WordTypeBefore;
            Prefixes.Add(prefix);
        }

        public void RemoveSuffix(DBAffix suffix)
        {
            Stem = Stem.Substring(0, Stem.Length - suffix.AffixText.Length);
            Type = suffix.WordTypeBefore;
            Suffixes.Insert(0, suffix);
        }

        public override string ToString()
        {
            return $"\t{string.Join(" + ", Prefixes)} {(Prefixes.Count() > 0 ? "+" : null)} {Stem} [{Type}] {(Suffixes.Count() > 0 ? "+" : null)} {string.Join(" + ", Suffixes)}";
        }
    }
}
