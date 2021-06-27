using System;
using System.Collections.Generic;
using System.Text;

namespace HLP.Parsing.Models
{
    public class MAResult
    {
        public string OriginalWord { get; }
        public List<MAVariant> Variants { get; }

        public MAResult(string word)
        {
            OriginalWord = word;
            Variants = new List<MAVariant>();
        }

        public override string ToString()
        {
            return $"{OriginalWord}:\n{string.Join("\n", Variants)}";
        }
    }
}
