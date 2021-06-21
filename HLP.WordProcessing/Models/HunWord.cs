using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HLP.WordProcessing.Models
{
    public class HunWord
    {
        public string Word { get; set; }
        public string Stem { get; set; }
        public List<string> Prefixes { get; set; }
        public List<string> Suffixes { get; set; }

        public HunWord() {}

        public HunWord(string word)
        {
            Word = word;
            Stem = word;
            Prefixes = new List<string>();
            Suffixes = new List<string>();
        }

        public HunWord Clone()
        {
            return new HunWord
            {
                Word = Word,
                Stem = Stem,
                Prefixes = new List<string>(Prefixes),
                Suffixes = new List<string>(Suffixes)
            };
        }

        public void RemovePrefix(string prefix)
        {
            var newWord = Word.Substring(prefix.Length);
        }

        public override string ToString()
        {
            return $"{Word}: {(Prefixes.Any() ? $"{string.Join("+", Prefixes)}+" : null)}{Stem}{(Suffixes.Any() ? $"+{string.Join("+", Suffixes)}" : null)}";
        }
    }
}
