﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HLP.WordProcessing.Models
{
    public class AnalysisResult
    {
        public string OriginalWord { get; }
        public List<AnalysisVariant> Variants { get; }

        public AnalysisResult(string word)
        {
            OriginalWord = word;
            Variants = new List<AnalysisVariant>();
        }

        public override string ToString()
        {
            return $"{OriginalWord}:\n{string.Join("\n", Variants)}";
        }
    }
}
