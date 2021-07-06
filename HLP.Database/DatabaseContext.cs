using HLP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

namespace HLP.Database
{
    public class DatabaseContext
    {
        private static DatabaseContext Instance = null;
        public static int Users = 0;

        public static DatabaseContext GetInstance()
        {
            if (Instance == null)
            {
                Instance = new DatabaseContext();
            }
            return Instance;
        }

        public static void Dispose()
        {
            if (Instance != null && --Users == 0)
            {
                Instance.Words = null;
                Instance.Affixes = null;
                Instance.WordTypes = null;
                Instance.AffixInfos = null;
                Instance.OrderRules = null;
                Instance.MorphTests = null;
                Instance.SyntTests = null;
                Instance = null;
            }
        }

        private DatabaseContext() { }

        public List<Word> Words { get; set; } = new List<Word>();
        public List<Affix> Affixes { get; set; } = new List<Affix>();
        public List<WordType> WordTypes { get; set; } = new List<WordType>();
        public List<AffixInfo> AffixInfos { get; set; } = new List<AffixInfo>();
        public List<OrderRule> OrderRules { get; set; } = new List<OrderRule>();
        public List<MorphTest> MorphTests { get; set; } = new List<MorphTest>();
        public List<SyntTest> SyntTests { get; set; } = new List<SyntTest>();

        // szó keresése az adatbázisban, visszatéríti a közös szófajokat
        public List<string> SearchWordInDatabase(string word, string type)
        {
            var wordResult = Words.Find(w => w.Text == word);

            if (wordResult == null) return new List<string>();

            var types = GetCompatibleWordTypes(type);

            if (types.Count == 0) return new List<string>(wordResult.WordTypes);

            return new List<string>(wordResult.WordTypes.Intersect(types));
        }

        // szófajjal kompatibilis szófajok
        public List<string> GetCompatibleWordTypes(string typeCode)
        {
            var result = new List<string>();

            var type = WordTypes.Find(t => t.Code == typeCode);

            if (type == null) return result;

            result.Add(typeCode);

            type.IncludedWordTypes.ForEach(t => result.AddRange(GetCompatibleWordTypes(t)));

            return result.Distinct().ToList();
        }
    }
}
