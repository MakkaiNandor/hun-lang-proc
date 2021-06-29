using System;
using System.Collections.Generic;
using System.Text;

namespace HLP.Database.Entities
{
    public class Affix
    {
        public int AffixId { get; set; }
        public string OriginalText { get; set; }
        public string Text { get; set; }
        public AffixInfo Info { get; set; }
        public List<AffixInfo> Requirements { get; set; }
        public bool Prevowel { get; set; }
        public bool Assimilation { get; set; }
    }
}
