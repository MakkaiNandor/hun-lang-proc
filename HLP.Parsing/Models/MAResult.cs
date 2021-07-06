using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HLP.Parsing.Models
{
    public class MAResult
    {
        public string OriginalWord { get; }
        public List<MAVariant> Variants { get; private set; }

        public MAResult(string word)
        {
            OriginalWord = word;
            Variants = new List<MAVariant>();
        }

        // képzők visszaillesztése a szótőre
        public void Lemmatize()
        {
            foreach (var variant in Variants)
            {
                foreach (var suffix in variant.Suffixes.Where(s => s.Info.Type == "K"))
                {
                    variant.OriginalText += suffix.OriginalText;
                    variant.CurrentText = variant.OriginalText;
                    variant.WordType = suffix.Info.WordTypeAfter;
                }
                variant.Suffixes.RemoveAll(s => s.Info.Type == "K");
            }
        }

        // ismétlődések törlése
        public void DeleteDuplicates()
        {
            for (var i = Variants.Count - 1; i > 0; --i)
            {
                for (var j = i - 1; j >= 0; --j)
                {
                    if (Variants[i].Equals(Variants[j]))
                    {
                        Variants.RemoveAt(i);
                        j = -1;
                    }
                }
            }
        }
    }
}
