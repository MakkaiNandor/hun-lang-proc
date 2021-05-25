using HLP.WordProcessing;
using System;

namespace HLP
{
    class Program
    {
        static void Main(string[] args)
        {
            var processor = new MorphologicalAnalyzer();

            while (true)
            {
                var input = Console.ReadLine();
                processor.Analyze(input);
            }
        }
    }
}
