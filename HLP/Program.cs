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
            var processor = new MorphologicalAnalyzer();

            while (true)
            {
                Console.WriteLine("\n~~~~~ Bemenet ~~~~~");
                var input = Console.ReadLine();
                processor.Analyze(input);

            /*foreach (var item in result)
            {
                Console.WriteLine(item.ToString());
            }*/
            }
        }
    }
}
