using HLP.Database;
using HLP.Parsing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HLP.Parsing.Utils
{
    public class ResultReducer
    {
        private DatabaseContext dbContext;

        public ResultReducer()
        {
            dbContext = DatabaseContext.GetInstance();
        }

        public void ReduceResults(MAResult result)
        {
            var count = 0;
            foreach (var variant in new List<MAVariant>(result.Variants))
            {
                GetLemmaAndType(variant, out string type);

                if (!CheckRequirements(variant) || 
                    !CheckOrder(variant, type))
                {
                    result.Variants.Remove(variant);
                    ++count;
                }
            }
        }

        private bool CheckOrder(MAVariant variant, string type)
        {
            type = dbContext.GetCompatibleWordTypes("NSZ").Contains(type) ? "NSZ" : type;

            var order = dbContext.OrderRules.Find(rule => rule.RootWordType == type);

            if (order == null) return true;

            var offset = 0;

            foreach (var prefix in variant.Prefixes)
            {
                var newPos = SearchCode(prefix.Info.Code, order.RulesBeforeRoot, offset);
                if (newPos == -1)
                {
                    return false;
                }
                offset = newPos;
            }

            offset = 0;
            foreach (var suffix in variant.Suffixes.Where(s => s.Info.Type != "K"))
            {
                var newPos = SearchCode(suffix.Info.Code, order.RulesAfterRoot, offset);
                if (newPos == -1)
                {
                    return false;
                }
                offset = newPos;
            }

            return true;
        }

        private int SearchCode(string item, List<List<string>> list, int offset)
        {
            var pos = offset;
            while (pos < list.Count())
            {
                if (list[pos].Contains(item))
                {
                    return pos;
                }
                ++pos;
            }
            return -1;
        }

        private void GetLemmaAndType(MAVariant variant, out string type)
        {
            var currType = variant.WordType;
            foreach(var suffix in variant.Suffixes)
            {
                if (suffix.Info.Type != "K") break;
                currType = suffix.Info.WordTypeAfter;
            }
            type = currType;
        }

        private bool CheckRequirements(MAVariant variant)
        {
            var prevAffixCode = "";
            foreach (var suffix in variant.Suffixes)
            {
                var req = suffix.Requirements;
                if (req.Any() && prevAffixCode != req[0])
                {
                    return false;
                }
                prevAffixCode = suffix.Info.Code;
            }

            var persAffix = variant.Suffixes.Find(s => s.Info.GroupNumber == 7);
            if (persAffix != null &&
                !persAffix.Requirements.Any() &&
                variant.Suffixes.Exists(s => s.Info.GroupNumber == 3))
            {
                return false;
            }

            return true;
        }
    }
}
