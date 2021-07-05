using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HLP.Database.Models
{
    public class MyPair
    {
        public string Text { get; set; }

        public string RealText { get; set; }

        public MyPair(List<string> list)
        {
            if (list.Count != 2)
                return;
            Text = list[0];
            RealText = list[1];
        }

        public MyPair(string text, string realText)
        {
            Text = text;
            RealText = realText;
        }

        public MyPair(string text)
        {
            Text = RealText = text;
        }

        public override string ToString()
        {
            if (Text == RealText)
                return Text;
            return $"{Text}={RealText}";
        }
    }

    public class MorphTest
    {
        public string Word { get; set; }
        public MyPair Stem { get; set; }
        public string MorphCode { get; set; }
        public List<MyPair> Prefixes { get; set; }
        public List<MyPair> Suffixes { get; set; }
        public override string ToString()
        {
            return $"{Word}: {(Prefixes.Any() ? $"{string.Join("+", Prefixes)} + " : null)}{Stem}{(Suffixes.Any() ? $" + {string.Join("+", Suffixes)}" : null)} ({MorphCode})";
        }
    }
}
