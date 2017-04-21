using SpecialFile;
using System;
using System.Collections.Generic;

namespace FileGenerator
{
    class Program
    {
        const int MinNumber = 1;
        const int MaxNumber = 100000;
        static List<string> Strings =
            new List<string>
                {
                    "Apple",
                    "Something something something",
                    "Cherry is the best",
                    "Banana is yellow"
                };

        static void Main(string[] args)
        {
            try
            {
                GenerateFile();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unexpected Exception: {0}{1}", Environment.NewLine, ex);
                Console.ReadLine();
            }
        }

        static void GenerateFile()
        {
            Console.WriteLine("--------------File generation tool----------------");

            long size = 0;
            do
            {
                Console.WriteLine();
                Console.Write("Please provide a target file size in bytes: ");
                var line = Console.ReadLine();
                if (!long.TryParse(line, out size))
                {
                    Console.WriteLine("Unsuitable size: {0}", line);
                }
            } while (size <= 0);

            var file = String.Empty;
            do
            {
                Console.WriteLine();
                Console.Write("Please provide a target file name: ");
                file = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(file))
                {
                    Console.WriteLine("File cannot be empty.");
                }

            } while (string.IsNullOrWhiteSpace(file));

            Console.WriteLine();
            Console.WriteLine("File is being generated...");

            var xFile = new XFile();
            var options = new XFileGenerationOptions { FileName = file, Strings = Strings, MinNumber = MinNumber, MaxNumber = MaxNumber, FileSize = size };
            xFile.Generate(options);

            Console.WriteLine();
            Console.WriteLine("Done!");

            Console.ReadKey();
        }
    }
}
