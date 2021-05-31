﻿using HLP.Database.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HLP.Database
{
    public class DatabaseContext
    {
        private static readonly string WordsFilePath = "./Data/teszt.txt";
        private static readonly string AffixesFilePath = "./Data/toldalekok.txt";
        private static readonly string CodesFilePath = "./Data/kodok.txt";

        private static DatabaseContext DBInstance = null;

        public static readonly List<string> Nomens = new List<string>() { "MN", "FN", "NM" };
        public static readonly List<string> Vowels = new List<string>() { "a", "á", "e", "é", "i", "í", "o", "ó", "ö", "ő", "u", "ú", "ü", "ű" };
        public static readonly List<string> PreVowels = new List<string>() { "a", "á", "e", "é", "o", "ó", "ö", "ő" };

        public static DatabaseContext GetDatabaseContext()
        {
            if(DBInstance is null)
            {
                DBInstance = new DatabaseContext();
            }
            return DBInstance;
        }

        public List<DBWord> Words { get; } = new List<DBWord>();
        public List<DBAffix> Affixes { get; } = new List<DBAffix>();
        public List<DBCode> Codes { get; } = new List<DBCode>();

        private DatabaseContext()
        {
            LoadWordsFromFile();
            LoadAffixesFromFile();
            LoadCodesFromFile();
        }

        private void LoadWordsFromFile()
        {
            using (var reader = new StreamReader(WordsFilePath))
            {
                reader.ReadLine(); // Skip the header
                while (!reader.EndOfStream)
                {
                    var values = reader.ReadLine().Split(',');
                    Words.Add(new DBWord
                    {
                        WordText = values[0],
                        WordTypes = values[1].Split('|').ToList()
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
                    var values = reader.ReadLine().Split(',');
                    Affixes.Add(new DBAffix
                    {
                        AffixText = values[0],
                        AffixType = values[1],
                        WordTypeBefore = values[2],
                        WordTypeAfter = values[3],
                        Code = values[4]
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
                    var values = reader.ReadLine().Split(',');
                    Codes.Add(new DBCode
                    {
                        CodeText = values[0],
                        AffixType = values[1],
                        WordType = values[2],
                        Requirements = values[3].Split('|').ToList(),
                        Description = values[4]
                    });
                }
            }
        }
    }
}
