using HLP.Parsing.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace HLP.Parsing.Log
{
    public class Logging
    {
        // naplófájl
        private static readonly string outFilePath = @".\logs.txt";

        public void Log(string message)
        {
            using (var writer = new StreamWriter(outFilePath, true))
            {
                writer.WriteLine($"{DateTime.Now}\t{message}");
            }
        }

        public void Log(SAResult result, long time)
        {
            using (var writer = new StreamWriter(outFilePath, true))
            {
                writer.WriteLine($"{DateTime.Now}\tElapsed time: {time} ms\tAnalyzed sentence: {result.OriginalSentence}");
            }
        }

        public void Log(MAResult result, long time)
        {
            using (var writer = new StreamWriter(outFilePath, true))
            {
                writer.WriteLine($"{DateTime.Now}\tElapsed time: {time} ms\tAnalyzed word: {result.OriginalWord} ({result.Variants.Count})");
            }
        }
    }
}
