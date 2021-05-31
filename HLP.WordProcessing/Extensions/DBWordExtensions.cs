using HLP.Database;
using HLP.Database.Models;
using HLP.WordProcessing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HLP.WordProcessing.Extensions
{
    static class DBWordExtensions
    {
        public static bool ContainsWord(this List<DBWord> words, AnalysisVariant variant)
        {
            return words.Any(w =>
                w.WordText == variant.Stem &&
                (variant.Type == "NSZ" ?
                w.WordTypes.Intersect(DatabaseContext.Nomens).Any() :
                (variant.Type == "" || w.WordTypes.Contains(variant.Type)))
            );
        }
    }
}
