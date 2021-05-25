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

        public override string ToString()
        {
            return $"<DBAffix Affix='{AffixText}' AffixType='{AffixType}' WordTypeBefore='{WordTypeBefore}' WordTypeAfter='{WordTypeAfter}' Code='{Code}'>";
        }
    }
}
