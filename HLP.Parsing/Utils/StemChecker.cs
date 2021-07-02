using HLP.Database;
using HLP.Parsing.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HLP.Parsing.Utils
{
    static class StemChecker
    {

        public static List<string> CheckStems(string word, string type)
        {
            var dbContext = DatabaseContext.GetInstance();

            var result = new List<string>();

            if (type == "" || type == "IGE")
            {
                result.AddRange(CheckVerbStem(word));
            }

            if (type == "" || dbContext.GetCompatibleWordTypes("NSZ").Contains(type))
            {
                result.AddRange(CheckNomenStem(word));
            }

            return result;
        }

        // igei tőváltozat szótári alakjának keresése
        private static List<string> CheckVerbStem(string stem)
        {
            var verbs = DatabaseContext.GetInstance().Words
                .Where(w => w.WordTypes.Contains("IGE"))
                .Select(w => w.Text).ToList();

            var result = new List<string>() { stem };

            var temp = stem;
            var lastLetter = stem.GetLastLetter();
            temp = temp.RemoveFromEnd(lastLetter);
            var preLastLetter = temp.GetLastLetter();

            //Console.WriteLine($"last: {lastLetter}, prelast: {preLastLetter}");

            // 6 ige van, amelyek szótári tőalakjuk magánhangzóban végződik: fő, lő, nő, nyű, ró, sző
            // ezeknek a tőváltozata 'v'-vel végződik és a magánhangzó rövidül
            if ((new List<string>() { "föv", "löv", "növ", "nyüv", "rov", "szöv" }).Contains(stem))
            {
                result.Add(temp.ReplaceVowel(temp.Length - 1));
                return result;
            }

            result.Add(stem + "szik");

            //'s'-ben végződik 
            if (lastLetter == "s")
            {
                // kap egy 't'-t
                result.Add(stem + "t");

                // 's' helyett 't' lesz
                result.Add(temp + "t");
            }

            // 'sz'-ben végződik
            if (lastLetter == "sz")
            {
                // kap egy 't'-t
                result.Add(stem + "t");

                // kap egy 'ik'-et
                result.Add(stem + "ik");
            }

            // utolsó két betű mássalhangzó (hangzóhiány), az utolsó betű 'd', 'g', 'l', 'm', 'r' vagy 'z'
            if ((new List<string> { "d", "g", "l", "m", "r", "z" }).Contains(lastLetter) &&
                Alphabet.Consonants.Contains(preLastLetter))
            {
                // beszúrunk egy magánhangzót a két mássalhangzó közé
                result.AddRange(Alphabet.Vowels.Select(v => temp + v + lastLetter));
            }

            // 'v'-vel végződik
            if (lastLetter == "v")
            {
                // speciális eset: 'hisz' -> 'hív'
                if (temp == "hí")
                {
                    // hosszú magánhangzó rövidül és a 'v' helyett 'sz' lesz
                    result.Add(temp.ReplaceVowel(temp.Length - 1) + "sz");
                }

                // 'v' helyett 'sz' lesz
                result.Add(temp + "sz");

                // 'v' helyett 'szik' lesz
                result.Add(temp + "szik");
            }

            // egy magánhangzót tartalmaz és abban is végződik
            if (stem.HasVowel(1, 0) && Alphabet.Vowels.Contains(lastLetter))
            {
                // ha hosszú a magánhangzó, akkor rövidítjük
                var newStem = Alphabet.LongVowels.Contains(lastLetter) ? stem.ReplaceVowel(stem.Length - 1) : stem;

                // hozzáadjuk az 'sz'-et
                result.Add(newStem + "sz");

                // hozzáadjuk a 'szik'-et
                result.Add(newStem + "szik");
            }

            // 'd'-vel vagy 'z'-vel végződik és azt egy magánhangzó előzi meg
            if ((lastLetter == "d" || lastLetter == "z") && Alphabet.Vowels.Contains(preLastLetter))
            {
                // kicseréljük az utolsó két betűt 'szik'-re
                result.Add(temp.Substring(0, temp.Length - preLastLetter.Length) + "szik");
            }

            //Console.WriteLine($"({preLastLetter}, {lastLetter}) {string.Join(", ", result)}");

            return result.Distinct().Intersect(verbs).ToList();
        }

        // névszói tőváltozat szótári alakjának keresése
        private static List<string> CheckNomenStem(string stem)
        {
             var context = DatabaseContext.GetInstance();
             var nomenTypes = context.GetCompatibleWordTypes("NSZ");
             var nomens = context.Words
                 .Where(w => w.WordTypes.Intersect(nomenTypes).Any())
                 .Select(w => w.Text).ToList();

             var result = new List<string>() { stem };

             var temp = stem;
             var lastLetter = stem.GetLastLetter();
             temp = temp.RemoveFromEnd(lastLetter);
             var preLastLetter = temp.GetLastLetter();

             // az utolsó és az azt megelőző betű is mássalhangzó
             if (Alphabet.Consonants.Contains(lastLetter) &&
                 Alphabet.Consonants.Contains(preLastLetter))
             {
                 // beszúrunk egy magánhangzót a két mássalhangzó közé
                 result.AddRange(Alphabet.Vowels.Select(v => temp + v + lastLetter));

                 // felcseréljük a mássalhangzókat és beszúrunk egy magánhangzót közéjük
                 result.AddRange(Alphabet.Vowels.Select(v => temp.Substring(0, temp.Length - preLastLetter.Length) + lastLetter + v + preLastLetter));
             }

             // a szó legfeljebb 2 magánhangzót tartalmaz, az utolsó betű mássalhangzó és az utolsó előtti meg magánhangzó ('a', 'e', 'i', 'u', 'ü')
             if (stem.HasVowel(2, -1) &&
                 (new List<string>() { "a", "e", "i", "u", "ü" }).Contains(preLastLetter) &&
                 Alphabet.Consonants.Contains(lastLetter))
             {
                 // meghosszabbítjuk az utolsó magánhangzót
                 result.Add(stem.ReplaceVowel(stem.Length - lastLetter.Length - 1));
             }

             // hosszú magánhangzóban végződik ('á', 'é')
             if ((new List<string>() { "á", "é" }).Contains(lastLetter))
             {
                 // rövidítjük a magánhangzót
                 result.Add(stem.ReplaceVowel(stem.Length - 1));
             }

             // mássalhangzóban végződik (hiányzik egy szóvégi magánhangzó)
             if (Alphabet.Consonants.Contains(lastLetter))
             {
                 // hosszáadunk egy magánhangzót ('ú', 'ű', 'a', 'e')
                 result.AddRange((new List<string>() { "ú", "ű", "a", "e" }).Select(v => stem + v));
             }

             // 'a'-ban vagy 'e'-ben végződik
             if (lastLetter == "a")
             {
                 // kicseréljük az 'a'-t 'ó'-ra
                 result.Add(temp + "ó");
             }
             else if (lastLetter == "e")
             {
                 // kicseréljük az 'e'-t 'ő'-re
                 result.Add(temp + "ő");
             }

             // 'v'-ben végződő szavak
             if (lastLetter == "v")
             {
                 // a 'magv', 'műv' és 'bőv' esetében
                 if ((new List<string>() { "magv", "műv", "bőv" }).Contains(stem))
                 {
                     // eltűnik a 'v'
                     result.Add(temp);
                 }

                 // egytagú 'o', 'ö', 'u', 'ü' vagy 'e' magánhagzós szavak
                 else if (stem.HasVowel(1, 0) &&
                     (new List<string>() { "o", "ö", "u", "ü", "e" }).Contains(preLastLetter))
                 {
                     // eltűnik a 'v' és hosszabbodik a magánhangzó
                     result.Add(temp.ReplaceVowel(temp.Length - 1));
                 }

                 // 'av'-val végződő szavak
                 else if (preLastLetter == "a")
                 {
                     // 'av' helyett 'ó' lesz
                     result.Add(temp.Substring(0, temp.Length - 1) + "ó");
                 }

                 // két mássalhangzóval végződik
                 else if (Alphabet.Consonants.Contains(preLastLetter))
                 {
                     // 'v' helyett 'u', 'ú' vagy 'ű' lesz
                     result.AddRange((new List<string>() { "u", "ú", "ű" }).Select(v => temp + v));
                 }
             }

             return result.Intersect(nomens).ToList();
        }
    }
}
