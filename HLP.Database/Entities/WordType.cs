using System;
using System.Collections.Generic;
using System.Text;

namespace HLP.Database.Entities
{
    public class WordType
    {
        public int WordTypeId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public List<WordType> IncludedWordTypes { get; set; }
    }
}
