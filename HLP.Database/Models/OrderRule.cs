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
    }
}
