using HLP.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HLP.WordProcessing.Extensions
{
    public static class StringExtensions
    {
        private static readonly char[] delimiters = new[] { '.', '!', '?', ',', ';', ':', '\'', '\"', '(', ')', '[', ']', '{', '}', '/' };

        // Szöveg felbontása szavakra
        public static List<string> SplitToWords(this string text)
        {
            var result = new HashSet<string>();
            foreach (var word in text.Split().Select(w => w.Trim(delimiters).ToLower()))
            {
                result.Add(word);
            }
            return result.ToList();
        }

        // A szó tartalmaz-e magánhangzót
        public static bool HasVowel(this string text)
        {
            return Alphabet.Vowels.Any(v => text.Contains(v));
        }

        // A szó tartalmaz-e rövid magánhangzót
        public static bool HasShortVowel(this string text)
        {
            return Alphabet.ShortVowels.Any(v => text.Contains(v));
        }

        // A szó tartalmaz-e legalább két magánhangzót
        public static int NumberOfVowels(this string text)
        {
            var count = 0;
            foreach(var letter in text)
            {
                if (Alphabet.Vowels.Contains(letter.ToString()))
                {
                    ++count;
                }
            }
            //Console.WriteLine($"<Fn:HasTwoVowelsAtLeast ~ {text} has {count} vowels>");
            return count;
        }

        // A szó magánhangzóval kezdődik-e
        public static bool StartsWithVowel(this string text)
        {
            var result = Alphabet.Vowels.Contains(text[0].ToString());
            //Console.WriteLine($"<Fn:StartsWithVowel ~ {text} {result}>");
            return result;
        }

        // A szó magánhangzóval végződik-e
        public static bool EndsWithVowel(this string text)
        {
            return Alphabet.Vowels.Contains(text.Last().ToString());
        }

        // A szó előhangzóval kezdődik-e
        public static bool EndsWithPreVowel(this string text)
        {
            var result = Alphabet.PreVowels.Contains(text.Last().ToString());
            //Console.WriteLine($"<Fn:EndsWithPreVowel ~ {text} {result}>");
            return result;
        }

        // A szó hosszú (dupla) mássalhangzóval végződik-e
        public static bool EndsWithLongConsonant(this string text)
        {
            return Alphabet.LongConsonants.Any(c => text.EndsWith(c));
        }

        // A szó mássalhangzóval végződik-e
        public static bool EndsWithConsonant(this string text)
        {
            return Alphabet.Consonants.Any(c => text.EndsWith(c));
        }

        // A szó két mássalhangzóval végződik-e
        public static bool EndsWithTwoConsonants(this string text)
        {
            if (text.EndsWithLongConsonant())
            {
                return false;
            }
            var last = Alphabet.Consonants.Where(c => text.EndsWith(c)).OrderByDescending(c => c.Length).FirstOrDefault();
            var preLast = last != null ? Alphabet.Consonants.Where(c => text.Substring(0, text.Length - last.Length).EndsWith(c)).OrderByDescending(c => c.Length).FirstOrDefault() : null;
            return last != null && preLast != null;
        }

        // A szó hosszú magánhangzóval végződik-e
        public static bool EndsWithLongVowel(this string text)
        {
            return Alphabet.LongVowels.Any(v => text.EndsWith(v));
        }

        // Magánhangzó rövidítése vagy hosszabbítása
        public static string ReplaceVowel(this string text, int index)
        {
            var vowel = text.ElementAt(index).ToString();
            var result = text;
            if (Alphabet.ShortVowels.Contains(vowel))
            {
                result = result.Remove(index, 1).Insert(index, Alphabet.LongVowels[Alphabet.ShortVowels.IndexOf(vowel)]);
            }
            else if (Alphabet.LongVowels.Contains(vowel))
            {
                result = result.Remove(index, 1).Insert(index, Alphabet.ShortVowels[Alphabet.LongVowels.IndexOf(vowel)]);
            }
            return result;
        }

        // Magánhangzók pozíciójai
        public static List<int> IndexOfShortVowels(this string text)
        {
            var result = new List<int>();

            foreach(var vowel in Alphabet.ShortVowels)
            {
                result.AddRange(text.Select((b, i) => b == vowel[0] ? i : -1).Where(i => i >= 0));
            }

            return result;
        }
    }
}
