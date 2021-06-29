using System;
using System.Collections.Generic;
using System.Text;

namespace HLP.Database.Entities
{
    public class Word
    {
        public int WordId { get; set; }
        public string Text { get; set; }
        public ICollection<WordType> WordTypes { get; set; }
    }
}
