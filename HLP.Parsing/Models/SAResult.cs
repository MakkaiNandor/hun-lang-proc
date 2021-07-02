using System;
using System.Collections.Generic;
using System.Text;

namespace HLP.Parsing.Models
{
    public class SAResult
    {
        public string OriginalSentence { get; set; }
        public List<MAResult> WordResults { get; set; }
        public List<SAVariant> Variants { get; set; }

        public SAResult(string sentence)
        {
            OriginalSentence = sentence;
            Variants = new List<SAVariant>();
        }
    }
}
