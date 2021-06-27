﻿using HLP.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HLP.WordProcessing.Extensions
{
    public static class StringExtensions
    {
        public static string GetLastLetter(this string text)
        {
            return Alphabet.Letters.FirstOrDefault(l => text.EndsWith(l));
        }

        public static string RemoveFromEnd(this string text, string str)
        {
            return text.Substring(0, text.Length - str.Length);
        }

        public static string RemoveFromStart(this string text, string str)
        {
            return text.Substring(str.Length);
        }

        // A szó tartalmaz-e megadott mennyiségű magánhangzót
        public static bool HasVowel(this string text, int required = 1)
        {
            var count = 0;
            foreach (var letter in text)
            {
                if (Alphabet.Vowels.Contains(letter.ToString()))
                {
                    ++count;
                }
            }
            return count >= required;
        }

        // A szó tartalmaz-e rövid magánhangzót
        public static bool HasShortVowel(this string text)
        {
            return Alphabet.ShortVowels.Any(v => text.Contains(v));
        }

        // A szó magánhangzóval kezdődik-e
        public static bool StartsWithVowel(this string text)
        {
            var result = Alphabet.Vowels.Contains(text[0].ToString());
            return result;
        }

        // A szó magánhangzóval végződik-e
        public static bool EndsWithVowel(this string text)
        {
            return Alphabet.Vowels.Contains(text.Last().ToString());
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
