using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HLP.Database.Models
{
    public class MorphTest
    {
        public string Word { get; set; }
        public string Stem { get; set; }
        public string MorphCode { get; set; }
        public List<string> Prefixes { get; set; }
        public List<string> Suffixes { get; set; }
        public override string ToString()
        {
            return $"{Word}: {string.Join("+", Prefixes)}{(Prefixes.Any() ? "+" : null)}{Stem}{(Suffixes.Any() ? "+" : null)}{string.Join("+", Suffixes)} ({MorphCode})";
        }
    }
}
