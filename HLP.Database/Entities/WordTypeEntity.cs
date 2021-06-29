using System;
using System.Collections.Generic;
using System.Text;

namespace HLP.Database.Entities
{
    public class WordTypeEntity : BaseEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string IncludedWordTypes { get; set; }
    }
}
