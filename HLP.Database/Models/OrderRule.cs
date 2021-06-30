using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HLP.Database.Models
{
    public class OrderRule
    {
        public string RootWordType { get; set; }
        public List<List<string>> RulesBeforeRoot { get; set; }
        public List<List<string>> RulesAfterRoot { get; set; }

        public override string ToString()
        {
            return $"{(RulesBeforeRoot.Any() ? $"{string.Join(" + ", RulesBeforeRoot.Select(it => string.Join("|", it)))} + " : null)}{RootWordType}{(RulesAfterRoot.Any() ? $" + {string.Join(" + ", RulesAfterRoot.Select(it => string.Join("|", it)))}" : null)}";
        }
    }
}
