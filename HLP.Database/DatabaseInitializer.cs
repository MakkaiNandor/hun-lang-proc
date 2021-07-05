using HLP.Database.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLP.Database
{
    public static class DatabaseInitializer
    {
        // Fájlok elérési útvonalai
        // Ha az adatbázis üres, ezekből a fájlokból töltjük fel adatokkal
        private static readonly string wordsFilePath = @"Data\szavak.txt";
        private static readonly string affixesFilePath = @"Data\toldalekok.txt";
        private static readonly string wordTypesFilePath = @"Data\szofajok.txt";
        private static readonly string infoFilePath = @"Data\kodok.txt";
        private static readonly string orderRulesFilePath = @"Data\sorrend.txt";
        private static readonly string morphTestDataFilePath = @"Data\morph_teszt.txt";

        private static readonly char[] separators = new[] { ';', '|', '.', '+' };

        // Adatok beolvasása a fájlokból
        public static async Task InitializeAsync()
        {
            var dbContext = DatabaseContext.GetInstance();

            ++DatabaseContext.Users;

            if (DatabaseContext.Users != 1)
                return;
                //await Task.Delay(500);

            Console.WriteLine("Start");

            // Szavak beolvasása, ha üres
            if (dbContext.Words == null || !dbContext.Words.Any())
            {
                dbContext.Words = await LoadWordsAsync();
            }
            // Toldalék kódok beolvasása, ha üres
            if (dbContext.AffixInfos == null || !dbContext.AffixInfos.Any())
            {
                dbContext.AffixInfos = await LoadAffixInfosAsync();
            }
            // Szófajok beolvasása, ha üres
            if (dbContext.WordTypes == null || !dbContext.WordTypes.Any())
            {
                dbContext.WordTypes = await LoadWordTypesAsync();
            }
            // Toldalékok beolvasása, ha üres
            if (dbContext.Affixes == null || !dbContext.Affixes.Any())
            {
                dbContext.Affixes = await LoadAffixesAsync();
            }
            // Sorrendi szabályok beolvasása, ha üres
            if (dbContext.OrderRules == null || !dbContext.OrderRules.Any())
            {
                dbContext.OrderRules = await LoadOrderRulesAsync();
            }
            // Teszt adatok beolvasása, ha üres
            if (dbContext.MorphTests == null || !dbContext.MorphTests.Any())
            {
                dbContext.MorphTests = await LoadMorphTestsAsync();
            }

            Console.WriteLine("End");
        }

        // A szavak beolvasása fájlból
        private static async Task<List<Word>> LoadWordsAsync()
        {
            var words = new List<Word>();

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
                        WordTypes = values[1].Split(separators[1]).ToList()
                    });
                }
            }

            Console.WriteLine($"Words: {words.Count}");

            return words;
        }

        // A toldalékok beolvasása fájlból
        private static async Task<List<Affix>> LoadAffixesAsync()
        {
            var affixes = new List<Affix>();

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
                        Info = DatabaseContext.GetInstance().AffixInfos.Find(it => it.Code == codes.Last()),
                        Requirements = codes.Take(codes.Length - 1).ToList(),
                        Prevowel = values[2] == "1",
                        Assimilation = values[3] == "1"
                    });
                }
            }

            Console.WriteLine($"Affixes: {affixes.Count}");

            return affixes;
        }

        // A szófajok beolvasása fájlból
        private static async Task<List<WordType>> LoadWordTypesAsync()
        {
            Console.WriteLine("Loading word types!");

            var wordTypes = new List<WordType>();

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
                        IncludedWordTypes = values[2].Split(separators[1]).ToList()
                    });
                }
            }

            Console.WriteLine($"Word types: {wordTypes.Count}");

            return wordTypes;
        }

        // A toldalékok információit tartalmazó kódok beolvasása fájlból
        private static async Task<List<AffixInfo>> LoadAffixInfosAsync()
        {
            var affixInfos = new List<AffixInfo>();

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
                        GroupNumber = int.Parse(values[2]),
                        WordTypeBefore = values[3],
                        WordTypeAfter = values[4],
                        Description = values[5]
                    });
                }
            }

            Console.WriteLine($"Affix info: {affixInfos.Count}");

            return affixInfos;
        }

        // Toldalékolási sorrendre vonatkozó szabályok beolvasása fájlból
        private static async Task<List<OrderRule>> LoadOrderRulesAsync()
        {
            var orderRules = new List<OrderRule>();

            Console.WriteLine("Loading order rules!");

            using (var reader = new StreamReader(orderRulesFilePath))
            {
                await reader.ReadLineAsync(); // Skip the header
                while (!reader.EndOfStream)
                {
                    var values = (await reader.ReadLineAsync()).Split(separators[0]);
                    orderRules.Add(new OrderRule
                    {
                        RootWordType = values[0],
                        RulesBeforeRoot = values[1].Split(separators[2]).Select(it => it.Split(separators[1]).ToList()).ToList(),
                        RulesAfterRoot = values[2].Split(separators[2]).Select(it => it.Split(separators[1]).ToList()).ToList()
                    });
                }
            }

            Console.WriteLine($"Order rules: {orderRules.Count}");

            return orderRules;
        }

        // A morfológiai elemző tesztelési adatainak beolvasása fájlból
        private static async Task<List<MorphTest>> LoadMorphTestsAsync()
        {
            var morphTests = new List<MorphTest>();

            Console.WriteLine("Loading morph tests!");

            using (var reader = new StreamReader(morphTestDataFilePath))
            {
                await reader.ReadLineAsync(); // Skip the header
                while (!reader.EndOfStream)
                {
                    var values = (await reader.ReadLineAsync()).Split(separators[0]);
                    var items = values[1].Split(separators[3]).ToList();
                    var index = items.IndexOf(items.Find(it => it.StartsWith("!")));

                    morphTests.Add(new MorphTest
                    {
                        Word = values[0],
                        Stem = new MyPair(items[index].Remove(0, 1).Split("=").ToList()),
                        MorphCode = values[2],
                        Prefixes = items.Take(index).Select(it => new MyPair(it.Split("=").ToList())).ToList(),
                        Suffixes = items.TakeLast(items.Count() - index - 1).Select(it => new MyPair(it.Split("=").ToList())).ToList()
                    });
                }
            }

            Console.WriteLine($"Morph tests: {morphTests.Count}");

            return morphTests;
        }
    }
}
