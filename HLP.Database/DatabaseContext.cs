using HLP.Database.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HLP.Database
{
    public class DatabaseContext
    {
        private static readonly string WordTypesFilePath = "D:/Egyetem/Államvizsga/HunLangProc/HLP.Database/Data/szofajok.txt";
        private static readonly string WordsFilePath = "D:/Egyetem/Államvizsga/HunLangProc/HLP.Database/Data/szavak.txt";
        private static readonly string AffixesFilePath = "D:/Egyetem/Államvizsga/HunLangProc/HLP.Database/Data/toldalekok_uj.txt";
        private static readonly string CodesFilePath = "D:/Egyetem/Államvizsga/HunLangProc/HLP.Database/Data/kodok_uj.txt";

        private static readonly char MainSep = ';';
        private static readonly char SubSep = '|';
        private static readonly char SubSubSep = '.';

        private static DatabaseContext DBInstance = null;

        public static DatabaseContext GetDatabaseContext()
        {
            if(DBInstance is null)
            {
                DBInstance = new DatabaseContext();
            }
            return DBInstance;
        }

        public List<WordType> WordTypes { get; } = new List<WordType>();
        public List<Word> Words { get; } = new List<Word>();
        public List<Affix> Affixes { get; } = new List<Affix>();
        public List<AffixCode> Codes { get; } = new List<AffixCode>();

        private DatabaseContext()
        {
            // Szófajok beolvasása
            LoadWordTypesFromFile();
            // Toldalék kódok/típusok beolvasása
            LoadCodesFromFile();
            // Szavak beolvasása
            LoadWordsFromFile();
            // Toldalékok beolvasása
            LoadAffixesFromFile();
        }

        public List<string> GetCompatibleWordTypes(string typeCode)
        {
            var result = new List<string>();

            var type = WordTypes.Find(t => t.Code == typeCode);

            if (type == null) return result;

            result.Add(typeCode);

            type.Includes.ForEach(t => result.AddRange(GetCompatibleWordTypes(t)));

            result.AddRange(WordTypes.Where(t => t.Includes.Contains(typeCode)).Select(t => t.Code));

            return result;
        }

        private void LoadWordTypesFromFile()
        {
            using (var reader = new StreamReader(WordTypesFilePath))
            {
                reader.ReadLine(); // Skip the header
                while (!reader.EndOfStream)
                {
                    var values = reader.ReadLine().Split(MainSep);
                    WordTypes.Add(new WordType {
                        Code = values[0],
                        Name = values[1],
                        Includes = values[2].Split(SubSep).ToList()
                    });
                }
            }
        }

        private void LoadWordsFromFile()
        {
            using (var reader = new StreamReader(WordsFilePath))
            {
                reader.ReadLine(); // Skip the header
                while (!reader.EndOfStream)
                {
                    var values = reader.ReadLine().Split(MainSep);
                    Words.Add(new Word {
                        Text = values[0],
                        Types = values[1].Split(SubSep).ToList()
                    });
                }
            }
        }

        private void LoadAffixesFromFile()
        {
            using (var reader = new StreamReader(AffixesFilePath))
            {
                reader.ReadLine(); // Skip the header
                while (!reader.EndOfStream)
                { 
                    var values = reader.ReadLine().Split(MainSep);
                    var codes = values[0].Split(SubSubSep);
                    Affixes.Add(new Affix
                    {
                        Text = values[1],
                        Code = Codes.Find(c => c.Code == codes.Last()),
                        Requirements = new List<string>(codes.Take(codes.Length - 1)),
                        Prevowel = values[2] == "1"
                    });
                }
            }
        }

        private void LoadCodesFromFile()
        {
            using (var reader = new StreamReader(CodesFilePath))
            {
                reader.ReadLine(); // Skip the header
                while (!reader.EndOfStream)
                {
                    var values = reader.ReadLine().Split(MainSep);
                    Codes.Add(new AffixCode {
                        Code= values[0],
                        Type = values[1],
                        Group = int.Parse(values[2]),
                        WordTypeBefore = values[3],
                        WordTypeAfter = values[4],
                        Description = values[5]
                    });
                }
            }
        }
    }
}
