using System;
using System.Collections.Generic;
using System.Text;

namespace HLP.Database.Entities
{
    public class AffixEntity : BaseEntity
    {
        public string OriginalText { get; set; }
        public string Text { get; set; }
        public string InfoCode { get; set; }
        public string Requirements { get; set; }
        public bool Prevowel { get; set; }
        public bool Assimilation { get; set; }
    }
}
