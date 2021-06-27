using HLP.Database.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLP.Database
{
    public class DatabaseLoader
    {
        public static readonly string directory = "./Data";
        public static readonly string wordTypesFilePath = $"{directory}/szofajok.txt";
        public static readonly string wordsFilePath = $"{directory}/szavak.txt";
        public static readonly string affixesFilePath = $"{directory}/toldalekok_uj.txt";
        public static readonly string infoFilePath = $"{directory}/kodok_uj.txt";

        private static readonly char[] separators = new[] { ';', '|', '.' };

        public async void LoadDataAsync(List<WordType> wordTypes, List<Word> words, List<Affix> affixes, List<AffixInfo> affixInfos)
        {
            await LoadWordTypesAsync(wordTypes);
            await LoadWordsAsync(words);
            await LoadAffixInfoAsync(affixInfos);
            await LoadAffixesAsync(affixes, affixInfos);
        }

        private async Task LoadWordTypesAsync(List<WordType> wordTypes)
        {
            Console.WriteLine("Loading word types!");
            using (var reader = new StreamReader(wordTypesFilePath))
            {
                await reader.ReadLineAsync(); // Skip the header
                while (!reader.EndOfStream)
                {
                    var values = (await reader.ReadLineAsync()).Split(separators[0]);
                    wordTypes.Add(new WordType
                    {
                        Code = values[0],
                        Name = values[1],
                        Includes = values[2].Split(separators[1]).ToList()
                    });
                }
            }
            Console.WriteLine($"Word types: {wordTypes.Count}");
        }

        private async Task LoadWordsAsync(List<Word> words)
        {
            Console.WriteLine("Loading words!");
            using (var reader = new StreamReader(wordsFilePath))
            {
                await reader.ReadLineAsync(); // Skip the header
                while (!reader.EndOfStream)
                {
                    var values = (await reader.ReadLineAsync()).Split(separators[0]);
                    words.Add(new Word
                    {
                        Text = values[0],
                        Types = values[1].Split(separators[1]).ToList()
                    });
                }
            }
            Console.WriteLine($"Words: {words.Count}");
        }

        private async Task LoadAffixesAsync(List<Affix> affixes, List<AffixInfo> affixInfos)
        {
            Console.WriteLine("Loading affixes!");
            using (var reader = new StreamReader(affixesFilePath))
            {
                await reader.ReadLineAsync(); // Skip the header
                while (!reader.EndOfStream)
                {
                    var values = (await reader.ReadLineAsync()).Split(separators[0]);
                    var codes = values[0].Split(separators[2]);
                    affixes.Add(new Affix
                    {
                        OriginalText = values[1],
                        Text = values[1],
                        Info = affixInfos.Find(c => c.Code == codes.Last()),
                        Requirements = new List<string>(codes.Take(codes.Length - 1)),
                        Prevowel = values[2] == "1",
                        Assimilation = values[3] == "1"
                    });
                }
            }
            Console.WriteLine($"Affixes: {affixes.Count}");
        }

        private async Task LoadAffixInfoAsync(List<AffixInfo> affixInfos)
        {
            Console.WriteLine("Loading affix info!");
            using (var reader = new StreamReader(infoFilePath))
            {
                await reader.ReadLineAsync(); // Skip the header
                while (!reader.EndOfStream)
                {
                    var values = (await reader.ReadLineAsync()).Split(separators[0]);
                    affixInfos.Add(new AffixInfo
                    {
                        Code = values[0],
                        Type = values[1],
                        Group = int.Parse(values[2]),
                        WordTypeBefore = values[3],
                        WordTypeAfter = values[4],
                        Description = values[5]
                    });
                }
            }
            Console.WriteLine($"Affix info: {affixInfos.Count}");
        }
    }
}
