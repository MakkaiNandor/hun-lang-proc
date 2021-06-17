using System;
using System.Collections.Generic;
using System.Text;

namespace HLP.Database.Models
{
    public class AffixCode
    {
        public string Code { get; set; }
        public string Type { get; set; }
        public string WordTypeBefore { get; set; }
        public string WordTypeAfter { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return $"<AffixCode Code='{Code}' AffixType='{Type}' WordTypeBefore='{WordTypeBefore}' WordTypeAfter='{WordTypeAfter}' Description='{Description}'>";
        }
    }
}
