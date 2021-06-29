using HLP.Database.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLP.Database
{
    public class DatabaseInitializer
    {
        // Fájlok elérési útvonalai
        // Ha az adatbázis üres, ezekből a fájlokból töltjük fel adatokkal
        public static readonly string wordsFilePath = @"Data\szavak.txt";
        public static readonly string affixesFilePath = @"Data\toldalekok.txt";
        public static readonly string wordTypesFilePath = @"Data\szofajok.txt";
        public static readonly string infoFilePath = @"Data\kodok.txt";
        public static readonly string orderRulesFilePath = @"Data\sorrend.txt";
        public static readonly string morphTestDataFilePath = @"Data\morph_teszt.txt";

        private static readonly char[] separators = new[] { ';', '|', '.' };

        // Adatok beolvasásaa fájlokból
        public static async void InitializeAsync()
        {
            await using (var dbContext = new DatabaseContext())
            {
                Console.WriteLine("Start");
                await dbContext.Database.EnsureCreatedAsync();

                // Szavak beolvasása, ha üres
                if (!dbContext.Words.Any())
                {
                    await dbContext.Words.AddRangeAsync(await LoadWordsAsync());
                }
                // Toldalékok beolvasása, ha üres
                if (!dbContext.Affixes.Any())
                {
                    await dbContext.Affixes.AddRangeAsync(await LoadAffixesAsync());
                }
                // Szófajok beolvasása, ha üres
                if (!dbContext.WordTypes.Any())
                {
                    await dbContext.WordTypes.AddRangeAsync(await LoadWordTypesAsync());
                }
                // Toldalék kódok beolvasása, ha üres
                if (!dbContext.AffixInfos.Any())
                {
                    await dbContext.AffixInfos.AddRangeAsync(await LoadAffixInfosAsync());
                }
                // Sorrendi szabályok beolvasása, ha üres
                if (!dbContext.OrderRules.Any())
                {
                    await dbContext.OrderRules.AddRangeAsync(await LoadOrderRulesAsync());
                }
                // Teszt adatok beolvasása, ha üres
                if (!dbContext.MorphTests.Any())
                {
                    await dbContext.MorphTests.AddRangeAsync(await LoadMorphTestsAsync());
                }

                await dbContext.SaveChangesAsync();

                DatabaseContext.Instance = dbContext;

                Console.WriteLine("End");
            }
        }

        // A szavak beolvasása fájlból
        public static async Task<List<WordEntity>> LoadWordsAsync()
        {
            var words = new List<WordEntity>();

            Console.WriteLine("Loading words!");

            using (var reader = new StreamReader(wordsFilePath))
            {
                await reader.ReadLineAsync(); // Skip the header
                while (!reader.EndOfStream)
                {
                    var values = (await reader.ReadLineAsync()).Split(separators[0]);
                    words.Add(new WordEntity
                    {
                        Text = values[0],
                        WordTypes = values[1]
                    });
                }
            }

            Console.WriteLine($"Words: {words.Count}");

            return words;
        }

        // A toldalékok beolvasása fájlból
        public static async Task<List<AffixEntity>> LoadAffixesAsync()
        {
            var affixes = new List<AffixEntity>();

            Console.WriteLine("Loading affixes!");

            using (var reader = new StreamReader(affixesFilePath))
            {
                await reader.ReadLineAsync(); // Skip the header
                while (!reader.EndOfStream)
                {
                    try
                    {
                        var values = (await reader.ReadLineAsync()).Split(separators[0]);
                        var codes = values[0].Split(separators[2]);
                        affixes.Add(new AffixEntity
                        {
                            OriginalText = values[1],
                            Text = values[1],
                            InfoCode = codes.Last(),
                            Requirements = string.Join(separators[2], codes.Take(codes.Length - 1)),
                            Prevowel = values[2] == "1",
                            Assimilation = values[3] == "1"
                        });
                    }
                    catch(IndexOutOfRangeException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            Console.WriteLine($"Affixes: {affixes.Count}");

            return affixes;
        }

        // A szófajok beolvasása fájlból
        public static async Task<List<WordTypeEntity>> LoadWordTypesAsync()
        {
            Console.WriteLine("Loading word types!");

            var wordTypes = new List<WordTypeEntity>();

            using (var reader = new StreamReader(wordTypesFilePath))
            {
                await reader.ReadLineAsync(); // Skip the header
                while (!reader.EndOfStream)
                {
                    var values = (await reader.ReadLineAsync()).Split(separators[0]);
                    wordTypes.Add(new WordTypeEntity
                    {
                        Code = values[0],
                        Name = values[1],
                        IncludedWordTypes = values[2]
                    });
                }
            }

            Console.WriteLine($"Word types: {wordTypes.Count}");

            return wordTypes;
        }

        // A toldalékok információit tartalmazó kódok beolvasása fájlból
        public static async Task<List<AffixInfoEntity>> LoadAffixInfosAsync()
        {
            var affixInfos = new List<AffixInfoEntity>();

            Console.WriteLine("Loading affix info!");

            using (var reader = new StreamReader(infoFilePath))
            {
                await reader.ReadLineAsync(); // Skip the header
                while (!reader.EndOfStream)
                {
                    var values = (await reader.ReadLineAsync()).Split(separators[0]);
                    affixInfos.Add(new AffixInfoEntity
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
        public static async Task<List<OrderRuleEntity>> LoadOrderRulesAsync()
        {
            var orderRules = new List<OrderRuleEntity>();

            Console.WriteLine("Loading order rules!");

            using (var reader = new StreamReader(orderRulesFilePath))
            {
                await reader.ReadLineAsync(); // Skip the header
                while (!reader.EndOfStream)
                {
                    var values = (await reader.ReadLineAsync()).Split(separators[0]);
                    orderRules.Add(new OrderRuleEntity
                    {
                        RootWordType = values[0],
                        RulesBeforeRoot = values[1],
                        RulesAfterRoot = values[2]
                    });
                }
            }

            Console.WriteLine($"Order rules: {orderRules.Count}");

            return orderRules;
        }

        // A morfológiai elemző tesztelési adatainak beolvasása fájlból
        public static async Task<List<MorphTestEntity>> LoadMorphTestsAsync()
        {
            var morphTests = new List<MorphTestEntity>();

            Console.WriteLine("Loading morph tests!");

            using (var reader = new StreamReader(infoFilePath))
            {
                await reader.ReadLineAsync(); // Skip the header
                while (!reader.EndOfStream)
                {
                    var values = (await reader.ReadLineAsync()).Split(separators[0]);
                    morphTests.Add(new MorphTestEntity
                    {
                        Word = values[0],
                        Analysis = values[1],
                        MorphCode = values[2]
                    });
                }
            }

            Console.WriteLine($"Morph tests: {morphTests.Count}");

            return morphTests;
        }
    }
}
