using System;
using System.Collections.Generic;
using System.Text;

namespace HLP.Database.Models
{
    public class DBWord
    {
        public string WordText { get; set; }
        public List<string> WordTypes { get; set; }

        public override string ToString()
        {
            return $"<DBWord Word='{WordText}' WordTypes='{string.Join(", ", WordTypes)}'>";
        }
    }
}
