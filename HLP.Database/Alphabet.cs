using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HLP.Database
{
    public static class Alphabet
    {
        public static List<string> Vowels { get; } = new List<string>() { "a", "á", "e", "é", "i", "í", "o", "ó", "ö", "ő", "u", "ú", "ü", "ű" };

        public static List<string> ShortVowels { get { return Vowels.Where((v, i) => i % 2 == 0).ToList(); } }

        public static List<string> LongVowels { get { return Vowels.Where((v, i) => i % 2 == 1).ToList(); } }

        public static List<string> PreVowels { get; } = new List<string>() { "a", "á", "e", "é", "o", "ó", "ö", "ő" };

        public static List<string> Consonants { get; } = new List<string>() { "b", "c", "cs", "d", "dz", "dzs", "f", "g", "gy", "h", "j", "k", "l", "ly", "m", "n", "ny", "p", "q", "r", "s", "sz", "t", "ty", "v", "x", "y", "z", "zs" };

        public static List<string> LongConsonants { get { return Consonants.Select(c => c[0] + c).ToList(); } }
    }
}
