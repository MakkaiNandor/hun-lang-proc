using System;
using System.Collections.Generic;
using System.Text;

namespace HLP.Database.Models
{
    public class WordType
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public List<string> Includes { get; set; }
        public override string ToString()
        {
            return $"<WordType Code={Code} Name={Name} Includes={string.Join(", ", Includes)}>";
        }
    }
}
