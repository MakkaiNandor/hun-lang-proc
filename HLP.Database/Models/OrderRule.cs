using System;
using System.Collections.Generic;
using System.Text;

namespace HLP.Database.Models
{
    public class OrderRule
    {
        public string RootWordType { get; set; }
        public string RulesBeforeRoot { get; set; }
        public string RulesAfterRoot { get; set; }
    }
}
