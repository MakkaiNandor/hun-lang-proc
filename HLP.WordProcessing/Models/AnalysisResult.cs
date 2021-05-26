using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HLP.WordProcessing.Models
{
    public class AnalysisResult
    {
        public string OriginalWord { get; }
        public bool IsCompound { get; }
        public List<AnalysisVariant> Variants { get; }
        public List<AnalysisResult> ResultsOfComponents { get; }

        public AnalysisResult(string word, bool isCompound = false)
        {
            OriginalWord = word;
            IsCompound = isCompound;
            Variants = new List<AnalysisVariant>();
            ResultsOfComponents = new List<AnalysisResult>();
        }

        public override string ToString()
        {
            return $"{OriginalWord}:\n{string.Join("\n", Variants)}";
        }
    }
}
