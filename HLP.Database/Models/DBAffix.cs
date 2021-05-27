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
            return $"{AffixText} [{AffixType}] ({WordTypeBefore} -> {WordTypeAfter})";
        }
    }
}
