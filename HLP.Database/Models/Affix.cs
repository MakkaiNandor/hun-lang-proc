using System;
using System.Collections.Generic;
using System.Text;

namespace HLP.Database.Models
{
    public class Affix
    {
        public string Text { get; set; }
        public bool Prevowel { get; set; }
        public AffixCode Code { get; set; }
        public List<string> Requirements { get; set; }

        public Affix() { }

        public Affix(Affix other, string preVowel = null)
        {
            Text = preVowel != null ? other.Text.Insert(0, preVowel) : other.Text;
            Code = other.Code;
            Prevowel = other.Prevowel;
            Requirements = new List<string>(other.Requirements);
        }

        public override string ToString()
        {
            return $"{Text}[{Code.Type}]({Code.Code})";
        }
    }
}
