using HLP.Parsing;
using HLP.Parsing.Testing;
using System;
using System.Collections.Generic;

namespace HLP
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("[1] Szavak felbontása\n[2] Teljesítmény tesztelése");
            var option = Console.ReadLine();

            if (option == "2")
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
            }
        }
    }
}
