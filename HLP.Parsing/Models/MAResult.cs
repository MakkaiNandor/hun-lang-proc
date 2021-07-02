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

        public void DeleteDuplicates()
        {
            for (var i = Variants.Count - 1; i > 0; --i)
            {
                for (var j = i - 1; j >= 0; --j)
                {
                    Console.WriteLine($"check {i}. and {j}. items");
                    if (Variants[i].Equals(Variants[j]))
                    {
                        Console.WriteLine($"we should delete: {Variants[i]} at {i}");
                        Variants.RemoveAt(i);
                        j = -1;
                    }
                }
            }

            /*var variants = new List<MAVariant>();

            foreach (var v1 in Variants)
            {
                var unique = true;
                foreach (var v2 in variants)
                {
                    if (v1 == v2)
                        unique = false;
                }
                if (unique)
                    variants.Add(v1);
            }

            Variants.Clear();
            Variants.AddRange(variants);*/
        }

        public override string ToString()
        {
            return $"{OriginalWord}:\n{string.Join("\n", Variants)}";
        }
    }
}
