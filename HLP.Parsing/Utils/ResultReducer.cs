using HLP.Database;
using HLP.Parsing.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HLP.Parsing.Utils
{
    class ResultReducer
    {
        private readonly string filePath = $"{DatabaseLoader.directory}/sorrend.txt";
        private readonly char[] separators = new[] { ';', '.', '|' };
        private Dictionary<string, List<List<string>>> orders = new Dictionary<string, List<List<string>>>();
        private DatabaseContext context;

        public ResultReducer()
        {
            context = DatabaseContext.GetInstance();
            LoadFile();
        }

        private void LoadFile()
        {
            using (var reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(separators[0]);
                    var key = values[0];
                    var value = values[1].Split(separators[1]).Select(c => c.Split(separators[2]).ToList()).ToList();
                    orders.Add(key, value);
                }
            }
        }

        public void ReduceResults(MAResult result)
        {
            var count = 0;
            foreach (var variant in new List<MAVariant>(result.Variants))
            {
                GetLemmaAndType(variant, out string lemma, out string type);

                if (!CheckRequirements(variant) ||
                    !CheckOrder(variant, lemma, type))
                {
                    result.Variants.Remove(variant);
                    ++count;
                }
            }

            Console.WriteLine($"Removed: {count}");
        }

        private bool CheckOrder(MAVariant variant, string lemma, string type)
        {
            type = context.GetCompatibleWordTypes("NSZ").Contains(type) ? "NSZ" : type;

            if (!orders.TryGetValue(type, out List<List<string>> order)) return false;

            var startPos = 0; 
            var endPos = order.Count();
            var lemmaPos = SearchCode(type, order, startPos, endPos);

            foreach (var prefix in variant.Prefixes.Where(p => p.Info.Type != "I"))
            {
                var newPos = SearchCode(prefix.Info.Code, order, startPos, lemmaPos);
                if (newPos == -1)
                {
                    return false;
                }
                startPos = newPos;
            }

            foreach (var suffix in variant.Suffixes.Where(s => s.Info.Type != "K").Reverse())
            {
                var newPos = SearchCode(suffix.Info.Code, order, lemmaPos + 1, endPos);
                if (newPos == -1)
                {
                    return false;
                }
                endPos = newPos;
            }

            return true;
        }

        private int SearchCode(string item, List<List<string>> list, int start, int end)
        {
            var pos = start;
            while (pos < end)
            {
                if (list[pos].Contains(item))
                {
                    return pos;
                }
                ++pos;
            }
            return -1;
        }

        private void GetLemmaAndType(MAVariant variant, out string lemma, out string type)
        {
            var currLemma = variant.OriginalText;
            var currType = variant.WordType;
            foreach(var suffix in variant.Suffixes)
            {
                if (suffix.Info.Type != "K") break;
                currLemma += suffix.OriginalText;
                currType = suffix.Info.WordTypeAfter;
            }
            lemma = currLemma;
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

            var persAffix = variant.Suffixes.Find(s => s.Info.Group == 7);
            if (persAffix != null &&
                !persAffix.Requirements.Any() &&
                variant.Suffixes.Exists(s => s.Info.Group == 3))
            {
                return false;
            }

            return true;
        }
    }
}
