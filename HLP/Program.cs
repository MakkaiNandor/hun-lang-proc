using HLP.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace HLP
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("~~~~~~~~");

            await DatabaseInitializer.InitializeAsync();

            Console.WriteLine("~~~~~~~~");

            /*
            Console.WriteLine("Start");

            using var db = new DatabaseContext();

            Console.WriteLine(db.GetInfo());*/

            /*Console.WriteLine("[1] Szavak felbontása\n[2] Teljesítmény tesztelése");
            var option = Console.ReadLine();*/

            /*if (option == "2")
            {
                var tester = new MAPerformanceTesting();

                tester.TestData();
            }

            var analyzer = new MorphologicalAnalyzer();

            while (true)
            {
                Console.Write(">>> ");
                var input = Console.ReadLine();
                analyzer.AnalyzeText(input);
            }*/
        }
    }
}
