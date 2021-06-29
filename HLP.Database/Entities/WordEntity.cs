using System;
using System.Collections.Generic;
using System.Text;

namespace HLP.Database.Entities
{
    public class WordEntity : BaseEntity
    {
        public string Text { get; set; }
        public string WordTypes { get; set; }
    }
}
