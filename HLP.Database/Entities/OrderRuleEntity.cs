using System;
using System.Collections.Generic;
using System.Text;

namespace HLP.Database.Entities
{
    public class OrderRuleEntity : BaseEntity
    {
        public string RootWordType { get; set; }
        public string RulesBeforeRoot { get; set; }
        public string RulesAfterRoot { get; set; }
    }
}
