using HLP.Database;
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

        public static bool HasVowel(this string text)
        {
            return DatabaseContext.Vowels.Any(v => text.Contains(v));
        }

        public static bool HasTwoVowelsAtLeast(this string text)
        {
            var count = 0;
            foreach(var letter in text)
            {
                if (DatabaseContext.Vowels.Contains(letter.ToString()))
                {
                    ++count;
                }
            }
            //Console.WriteLine($"<Fn:HasTwoVowelsAtLeast ~ {text} has {count} vowels>");
            return count >= 2;
        }

        public static bool StartsWithVowel(this string text)
        {
            var result = DatabaseContext.Vowels.Contains(text[0].ToString());
            //Console.WriteLine($"<Fn:StartsWithVowel ~ {text} {result}>");
            return result;
        }

        public static bool EndsWithPreVowel(this string text)
        {
            var result = DatabaseContext.PreVowels.Contains(text.Last().ToString());
            //Console.WriteLine($"<Fn:EndsWithPreVowel ~ {text} {result}>");
            return result;
        }
    }
}
