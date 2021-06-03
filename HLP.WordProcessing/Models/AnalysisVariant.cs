using HLP.Database.Models;
using System.Collections.Generic;
using System.Linq;

namespace HLP.WordProcessing.Models
{
    public class AnalysisVariant
    {
        public string Text { get; set; }
        public string OriginalText { get; set; }
        public string Type { get; set; }
        public List<string> PossiblePrefixTypes { get; }
        public List<string> PossibleSuffixTypes { get; }
        public List<DBAffix> Prefixes { get; }
        public List<DBAffix> Suffixes { get; }

        public AnalysisVariant(string word, string type = "")
        {
            Text = word;
            OriginalText = word;
            Type = type;
            PossiblePrefixTypes = new List<string> { "P", "I" };
            PossibleSuffixTypes = new List<string> { "K", "J", "R" };
            Prefixes = new List<DBAffix>();
            Suffixes = new List<DBAffix>();
        }

        public AnalysisVariant(AnalysisVariant other)
        {
            Text = other.Text;
            OriginalText = other.OriginalText;
            Type = other.Type;
            PossiblePrefixTypes = new List<string>(other.PossiblePrefixTypes);
            PossibleSuffixTypes = new List<string>(other.PossibleSuffixTypes);
            Prefixes = new List<DBAffix>(other.Prefixes);
            Suffixes = new List<DBAffix>(other.Suffixes);
        }

        public void RemovePrefix(DBAffix prefix)
        {
            OriginalText = Text = Text.Substring(prefix.AffixText.Length);
            Prefixes.Add(prefix);
            if (prefix.AffixType == "I")
            {
                PossiblePrefixTypes.Remove("P");
            }
            PossiblePrefixTypes.Remove(prefix.AffixType);
        }

        public void RemoveSuffix(DBAffix suffix)
        {
            OriginalText = Text = Text.Substring(0, Text.Length - suffix.AffixText.Length);
            Type = suffix.WordTypeBefore;
            Suffixes.Insert(0, suffix);
            switch (suffix.AffixType)
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
        }

        public bool IsGood(DBAffix suffix)
        {
            if (Suffixes.Count() == 0)
            {
                return true;
            }
            return Suffixes.Last().AffixText != suffix.AffixText;
        }

        public override string ToString()
        {
            return $"\t{(Prefixes.Any() ? $"{string.Join(" + ", Prefixes)} + " : null)}{OriginalText}({Type}){(Text != OriginalText ? $"={Text}" : null)}{(Suffixes.Any() ? $" + {string.Join(" + ", Suffixes)}" : null)}";
        }
    }
}
