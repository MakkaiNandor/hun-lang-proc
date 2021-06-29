using System;
using System.Collections.Generic;
using System.Text;

namespace HLP.Database.Models
{
    public class Word
    {
        public string Text { get; set; }
        public List<string> WordTypes { get; set; }

        public override string ToString()
        {
            return $"<Word Text='{Text}' Types='{string.Join(", ", WordTypes)}'>";
        }
    }
}
