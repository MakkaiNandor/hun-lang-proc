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
        public MAResult MorphResult { get; set; }

        // mondatrész kód -> mondatrész neve
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

        // tartalmazott szófajok
        public List<string> GetTypes()
        {
            return MorphResult.Variants.Select(it => it.WordType).Distinct().ToList();
        }

        // elemzések utolsó toldalékainak csoportja
        public List<int> LastGroups()
        {
            return MorphResult.Variants.Where(it => it.Suffixes.Any()).Select(it => it.Suffixes.Last().Info.GroupNumber).ToList();
        }

        // elemzések toldalékainak csoportja
        public List<int> AllGroups()
        {
            var result = new List<int>();
            MorphResult.Variants.Select(it => it.Suffixes.Select(s => s.Info.GroupNumber).ToList()).ToList().ForEach(it => result.AddRange(it));
            return result;
        }

        // elemzések utolsó toldalékainak kódja
        public List<string> LastCodes()
        {
            return MorphResult.Variants.Where(it => it.Suffixes.Any()).Select(it => it.Suffixes.Last().Info.Code).ToList();
        }


        // az elemzések tartalmazzák-e a megadott szófajt
        public bool MorphResultContains(string wordType)
        {
            return MorphResult.Variants.Select(v => v.WordType).Contains(wordType);
        }

        // csak egy szófaja van-e
        public bool HasOnlyOne()
        {
            return MorphResult.Variants.Select(v => v.WordType).Distinct().Count() == 1;
        }
    }
}
