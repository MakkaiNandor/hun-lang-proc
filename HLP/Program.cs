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
                Console.WriteLine("\n~~~~~ Bemenet ~~~~~");
                var input = Console.ReadLine();
                var result = processor.Analyze(input);

                /*foreach (var item in result)
                {
                    Console.WriteLine(item.ToString());
                }*/
            }
        }
    }
}
