using System;
using System.Collections.Generic;
using System.Text;

namespace HLP.Database.Models
{
    public class DBAffix
    {
        public string AffixText { get; set; }
        public string AffixType { get; set; }
        public string WordTypeBefore { get; set; }
        public string WordTypeAfter { get; set; }
        public string Code { get; set; }

        public DBAffix() { }

        public DBAffix(DBAffix other, string preVowel = null)
        {
            AffixText = preVowel != null ? other.AffixText.Insert(0, preVowel) : other.AffixText;
            AffixType = other.AffixType;
            WordTypeBefore = other.WordTypeBefore;
            WordTypeAfter = other.WordTypeAfter;
            Code = other.Code;
        }

        public bool IsPrefix()
        {
            return AffixType == "P" || AffixType == "I";
        }

        public bool IsSuffix()
        {
            return !IsPrefix();
        }

        public override string ToString()
        {
            return $"{AffixText}[{AffixType}]({WordTypeBefore}->{WordTypeAfter})";
        }
    }
}
