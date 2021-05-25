using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HLP.WordProcessing.Extensions
{
    static class StringExtensions
    {
        private static readonly char[] delimiters = new[] { '.', '!', '?', ',', ';', ':', '\'', '\"', '(', ')', '[', ']', '{', '}', '/' };

        public static List<string> SplitToWords(this string text)
        {
            var result = new HashSet<string>();
            foreach (var word in text.Split().Select(w => w.Trim(delimiters).ToLower()))
            {
                result.Add(word);
            }
            return result.ToList();
        }
    }
}
