using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HLP.Parsing.Models
{
    public class SAItem
    {
        public SParts Type { get; set; }
        public string Text { get; set; }
        public string WordType { get; set; }
        public MAResult MorphResult { get; set; }

        public string TypeToString()
        {
            string result;
            switch (Type)
            {
                case SParts.PREDICATE:
                    result = "állítmány";
                    break;
                case SParts.SUBJECT:
                    result = "alany";
                    break;
                case SParts.OBJECT:
                    result = "tárgy";
                    break;
                case SParts.ADVERB:
                    result = "határozó";
                    break;
                case SParts.ATTRIBUTE:
                    result = "jelző";
                    break;
                default:
                    result = "ismeretlen";
                    break;
            }
            return result;
        }

        public List<string> GetTypes()
        {
            return MorphResult.Variants.Select(it => it.WordType).Distinct().ToList();
        }

        public List<int> LastGroups()
        {
            return MorphResult.Variants.Where(it => it.Suffixes.Any()).Select(it => it.Suffixes.Last().Info.GroupNumber).ToList();
        }

        public List<string> LastCodes()
        {
            return MorphResult.Variants.Where(it => it.Suffixes.Any()).Select(it => it.Suffixes.Last().Info.Code).ToList();
        }

        public bool MorphResultContains(string wordType)
        {
            return MorphResult.Variants.Select(v => v.WordType).Contains(wordType);
        }

        public bool HasOnlyOne()
        {
            return MorphResult.Variants.Select(v => v.WordType).Distinct().Count() == 1;
        }

        public override string ToString()
        {
            return $"{Text}[{TypeToString()}]";
        }
    }
}
