using System;
using System.Collections.Generic;
using System.Text;

namespace HLP.Parsing.Models
{
    public class SAResult
    {
        public string OriginalSentence { get; set; }
        public List<SAItem> Result { get; set; }

        public SAResult(string sentence)
        {
            OriginalSentence = sentence;
            Result = new List<SAItem>();
        }
    }
}
