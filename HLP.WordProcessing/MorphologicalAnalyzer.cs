using HLP.Database;
using HLP.WordProcessing.Extensions;
using HLP.WordProcessing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HLP.WordProcessing
{
    public class MorphologicalAnalyzer
    {
        private readonly DatabaseContext DBContext;

        public MorphologicalAnalyzer()
        {
            DBContext = DatabaseContext.GetDatabaseContext();
        }

        public List<AnalysisResult> Analyze(string inputText)
        {
            var result = new List<AnalysisResult>();

            var words = inputText.SplitToWords();

            foreach (var word in words)
            {
                var subWords = word.Split('-').ToList();
            }

            // TODO: Összetett szavak ellenőrzése és felosztása
            // TODO: Igekötő és/vagy prefixum eltávolítása
            // TODO: Rag eltávolítása
            // TODO: Jelzők eltávolítása
            // TODO: Képők eltávolítása

            return result;
        }
    }
}
