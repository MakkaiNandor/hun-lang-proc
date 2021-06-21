using HLP.WordProcessing;
using System;
using HLP.WordProcessing.Extensions;
using System.Collections.Generic;

namespace HLP
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("[1] Szavak felbontása\n[2] Teljesítmény tesztelése");
            var option = Console.ReadLine();

            if (option == "1")
            {
                var analyzer = new MorphologicalAnalyzer();

                while (true)
                {
                    Console.WriteLine("\n~~~~~ Bemenet ~~~~~");
                    var input = Console.ReadLine();
                    analyzer.Analyze(input, true);
                }
            }
            else if (option == "2")
            {
                var tester = new PerformanceTesting();

                tester.TestData();
            }
        }
    }
}
