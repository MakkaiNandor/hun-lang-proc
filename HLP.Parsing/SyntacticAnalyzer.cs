using HLP.Database;
using HLP.Database.Models;
using HLP.Parsing.Log;
using HLP.Parsing.Models;
using HLP.Parsing.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HLP.Parsing
{
    public class SyntacticAnalyzer
    {
        private readonly MorphologicalAnalyzer analyzer;
        private readonly Logging log;

        public SyntacticAnalyzer()
        {
            analyzer = new MorphologicalAnalyzer();
            log = new Logging();
        }

        public SAResult AnalyzeSentence(string sentence)
        {
            var result = new SAResult(sentence);

            result.WordResults = analyzer.AnalyzeText(sentence);

            foreach (var wordResult in result.WordResults)
            {
                wordResult.Lemmatize();
                wordResult.DeleteDuplicates();
            }

            return result;
        }

    }
}
