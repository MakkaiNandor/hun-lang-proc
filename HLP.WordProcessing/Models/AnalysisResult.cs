using System;
using System.Collections.Generic;
using System.Text;

namespace HLP.WordProcessing.Models
{
    public class AnalysisResult
    {
        public string OriginalWord { get; }
        public bool IsCompound { get; }
        public List<AnalysisVariant> Variants { get; }
        public List<AnalysisResult> ResultsOfComponents { get; }
    }
}
