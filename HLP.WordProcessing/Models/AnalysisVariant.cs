using System;
using System.Collections.Generic;
using System.Text;

namespace HLP.WordProcessing.Models
{
    public class AnalysisVariant
    {
        public string Stem { get; }
        public List<Affix> Prefixes { get; } = new List<Affix>();
        public List<Affix> Suffixes { get; } = new List<Affix>();
    }
}
