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
        public static List<string> GetCommonTypes(this List<DBWord> words, AnalysisVariant variant)
        {
            var result = new List<string>();

            var word = words.Find(w => w.WordText == variant.Text);

            if (word != null)
            {
                switch (variant.Type)
                {
                    case "":
                        result.AddRange(word.WordTypes);
                        break;
                    case "NSZ":
                        result.AddRange(word.WordTypes.Intersect(DatabaseContext.Nomens));
                        break;
                    default:
                        if (word.WordTypes.Contains(variant.Type))
                        {
                            result.Add(variant.Type);
                        }
                        break;
                }
            }

            return result;
        }
    }
}
