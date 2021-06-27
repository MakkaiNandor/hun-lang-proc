using HLP.Database.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HLP.Database
{
    public class DatabaseContext
    {
        private static DatabaseContext DBInstance = null;

        public static DatabaseContext GetInstance()
        {
            if(DBInstance is null)
            {
                DBInstance = new DatabaseContext();
            }
            return DBInstance;
        }

        public List<WordType> WordTypes { get; } = new List<WordType>();
        public List<Word> Words { get; } = new List<Word>();
        public List<Affix> Affixes { get; } = new List<Affix>();
        public List<AffixInfo> AffixInfos { get; } = new List<AffixInfo>();

        private DatabaseContext()
        {
            var loader = new DatabaseLoader();
            loader.LoadDataAsync(WordTypes, Words, Affixes, AffixInfos);
        }

        public List<string> SearchInDatabase(string word, string type)
        {
            var wordResult = Words.Find(w => w.Text == word);

            if (wordResult == null) return new List<string>();

            var types = GetCompatibleWordTypes(type);

            if (types.Count == 0) return new List<string>(wordResult.Types);

            return new List<string>(wordResult.Types.Intersect(types));
        }

        public List<string> GetCompatibleWordTypes(string typeCode)
        {
            var result = new List<string>();

            var type = WordTypes.Find(t => t.Code == typeCode);

            if (type == null) return result;

            result.Add(typeCode);

            type.Includes.ForEach(t => result.AddRange(GetCompatibleWordTypes(t)));

            result.AddRange(WordTypes.Where(t => t.Includes.Contains(typeCode)).Select(t => t.Code));

            return result.Distinct().ToList();
        }
    }
}
