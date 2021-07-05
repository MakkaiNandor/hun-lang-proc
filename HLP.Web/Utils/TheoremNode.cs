using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HLP.Web.Utils
{
    public class TheoremNode
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public List<TheoremNode> Items { get; set; }
    }
}
