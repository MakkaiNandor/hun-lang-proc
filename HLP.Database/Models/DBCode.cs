using System;
using System.Collections.Generic;
using System.Text;

namespace HLP.Database.Models
{
    public class DBCode
    {
        public string CodeText { get; set; }
        public string AffixType { get; set; }
        public string WordType { get; set; }
        public List<string> Requirements { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return $"<DBCode Code='{CodeText}' AffixType='{AffixType}' WordType='{WordType}' Requirements='{string.Join(", ", Requirements)}' Description='{Description}'>";
        }
    }
}
