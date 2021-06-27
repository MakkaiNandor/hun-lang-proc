using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HLP.Parsing.Utils
{
    class Tokenizer
    {
        public List<string> GetWords(string text)
        {
            return Regex.Matches(text, @"([A-ZÁÉÍÓÖŐÚÜŰa-záéíóöőúüű]+)(\-[A-ZÁÉÍÓÖŐÚÜŰa-záéíóöőúüű]+)?").Cast<Match>().Select(m => m.Value).ToList();
        }
    }
}
