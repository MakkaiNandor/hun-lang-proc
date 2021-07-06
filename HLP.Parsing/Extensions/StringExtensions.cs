using HLP.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HLP.Parsing.Extensions
{
    public static class StringExtensions
    {
        // string utolsó betűje
        public static string GetLastLetter(this string text)
        {
            var lastLetter = Alphabet.Letters.FirstOrDefault(l => text.EndsWith(l));
            return lastLetter ?? "";
        }

        // eltávolítás a végéről
        public static string RemoveFromEnd(this string text, string str)
        {
            return text.Substring(0, text.Length - str.Length);
        }

        // eltávolítás az elejéről
        public static string RemoveFromStart(this string text, string str)
        {
            return text.Substring(str.Length);
        }

        // A szó tartalmaz-e megadott mennyiségű magánhangzót
        public static bool HasVowel(this string text, int required = 1, int mode = 1)
        {
            /*
             * mode ->  -1: less than or equal to required
             *          0: equal to required
             *          1: greater than or equal to required
             */
            required = required < 0 ? 0 : required;
            var count = 0;
            foreach (var letter in text)
            {
                if (Alphabet.Vowels.Contains(letter.ToString()))
                {
                    ++count;
                }
            }
            return mode < 0 ? count <= required : mode > 0 ? count >= required : count == required;
        }

        // A szó magánhangzóval kezdődik-e
        public static bool StartsWithVowel(this string text)
        {
            var result = Alphabet.Vowels.Contains(text[0].ToString());
            return result;
        }

        // A szó előhangzóval végződik-e
        public static bool EndsWithPreVowel(this string text)
        {
            return Alphabet.PreVowels.Contains(text.Last().ToString());
        }

        // A szó hosszú (dupla) mássalhangzóval végződik-e
        public static bool EndsWithLongConsonant(this string text)
        {
            return Alphabet.LongConsonants.Any(c => text.EndsWith(c));
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
    }
}
