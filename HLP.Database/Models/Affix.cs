using System;
using System.Collections.Generic;
using System.Text;

namespace HLP.Database.Models
{
    public class Affix
    {
        public string OriginalText { get; set; }
        public string Text { get; set; }
        public AffixInfo Info { get; set; }
        public List<string> Requirements { get; set; }
        public bool Prevowel { get; set; }
        public bool Assimilation { get; set; }

        public Affix() { }

        // copy konstruktor
        public Affix(Affix other)
        {
            OriginalText = other.OriginalText;
            Text = other.Text;
            Prevowel = other.Prevowel;
            Assimilation = other.Assimilation;
            Info = other.Info;
            Requirements = new List<string>(other.Requirements);
        }

        // toldalék másolása a megadott előhangzóval
        public Affix GetWithPreVowel(string preVowel)
        {
            return new Affix()
            {
                OriginalText = OriginalText.Insert(0, preVowel),
                Text = Text,
                Info = Info,
                Prevowel = false,
                Assimilation = Assimilation,
                Requirements = Requirements
            };
        }

        // összehasonlítás
        public bool Equals(Affix other)
        {
            return !(Info.Code != other.Info.Code ||
                OriginalText != other.OriginalText ||
                Text != other.Text);
        }
    }
}
