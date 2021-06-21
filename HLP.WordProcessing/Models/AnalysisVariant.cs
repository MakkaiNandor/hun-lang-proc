using HLP.Database.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HLP.WordProcessing.Models
{
    public class AnalysisVariant
    {
        public string Text { get; set; }
        public string OriginalText { get; set; }
        public string Type { get; set; }
        public List<string> PossiblePrefixTypes { get; }
        public List<string> PossibleSuffixTypes { get; }
        public List<Affix> Prefixes { get; }
        public List<Affix> Suffixes { get; }
        public List<int> AffixGroups { get; }

        public AnalysisVariant(string word, string type = "")
        {
            Text = word;
            OriginalText = word;
            Type = type;
            PossiblePrefixTypes = new List<string> { "P", "I" };
            PossibleSuffixTypes = new List<string> { "K", "J", "R" };
            Prefixes = new List<Affix>();
            Suffixes = new List<Affix>();
            AffixGroups = new List<int>();
        }

        public AnalysisVariant(AnalysisVariant other)
        {
            Text = other.Text;
            OriginalText = other.OriginalText;
            Type = other.Type;
            PossiblePrefixTypes = new List<string>(other.PossiblePrefixTypes);
            PossibleSuffixTypes = new List<string>(other.PossibleSuffixTypes);
            Prefixes = new List<Affix>(other.Prefixes);
            Suffixes = new List<Affix>(other.Suffixes);
            AffixGroups = new List<int>(other.AffixGroups);
        }

        public void RemovePrefix(Affix prefix)
        {
            OriginalText = Text = Text.Substring(prefix.Text.Length);
            Prefixes.Add(prefix);
            if (prefix.Code.Type == "I")
            {
                PossiblePrefixTypes.Remove("P");
            }
            PossiblePrefixTypes.Remove(prefix.Code.Type);
            if (prefix.Code.Group != 0 && !AffixGroups.Contains(prefix.Code.Group))
            {
                AffixGroups.Add(prefix.Code.Group);
            }
        }

        public void RemoveSuffix(Affix suffix)
        {
            OriginalText = Text = Text.Substring(0, Text.Length - suffix.Text.Length);
            Type = suffix.Code.WordTypeBefore;
            Suffixes.Insert(0, suffix);
            switch (suffix.Code.Type)
            {
                case "R":
                    PossibleSuffixTypes.Remove("R");
                    break;
                case "J":
                    PossibleSuffixTypes.Remove("R");
                    if (Type == "IGE")
                    {
                        PossibleSuffixTypes.Remove("J");
                    }
                    break;
                case "K":
                    PossibleSuffixTypes.Remove("R");
                    PossibleSuffixTypes.Remove("J");
                    break;
                default: break;
            }
            if (suffix.Code.Group != 0 && !AffixGroups.Contains(suffix.Code.Group))
            {
                AffixGroups.Add(suffix.Code.Group);
            }
        }

        public bool IsGood(Affix suffix)
        {
            if (Suffixes.Count() == 0)
            {
                return true;
            }
            return Suffixes.Last().Text != suffix.Text;
        }

        public string GetMorphCode()
        {
            var result = new List<string>();

            foreach (var prefix in Prefixes)
            {
                result.Add(prefix.Code.Code);
            }

            result.Add(Type);

            foreach(var suffix in Suffixes)
            {
                result.Add(suffix.Code.Code);
            }

            return string.Join(".", result);
        }

        public override string ToString()
        {
            return $"\t{(Prefixes.Any() ? $"{string.Join(" + ", Prefixes)} + " : null)}{OriginalText}({Type}){(Text != OriginalText ? $"={Text}" : null)}{(Suffixes.Any() ? $" + {string.Join(" + ", Suffixes)}" : null)}\n\t{GetMorphCode()}";
        }
    }
}
