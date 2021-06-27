using HLP.Database;
using HLP.Database.Models;
using HLP.WordProcessing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HLP.WordProcessing.Extensions
{
    static class WordExtensions
    {
        public static List<string> GetCommonTypes(this List<Word> words, AnalysisVariant variant)
        {
            var word = words.Find(w => w.Text == variant.CurrentText);

            if (word == null) return new List<string>();

            var DB = DatabaseContext.GetInstance();

            var types = DB.GetCompatibleWordTypes(variant.WordType);

            if (types.Count == 0) return new List<string>(word.Types);

            return new List<string>(word.Types.Intersect(types));
        }
    }
}
