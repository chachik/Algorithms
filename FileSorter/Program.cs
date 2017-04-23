using SpecialFile;
using System;
using System.Diagnostics;
using System.IO;

namespace FileSorter
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                SortFile();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unexpected Exception: {0}{1}", Environment.NewLine, ex);
                Console.ReadLine();
            }
        }

        static void SortFile()
        {
            Console.WriteLine("--------------File sorting tool----------------");

            var source = String.Empty;
            do
            {
                Console.WriteLine();
                Console.Write("Please provide a name of the source file: ");
                source = Console.ReadLine();
                if (!File.Exists(source))
                {
                    Console.WriteLine("File '{0}' doesn't exist.", source);
                    source = string.Empty;
                }
            } while (string.IsNullOrWhiteSpace(source));

            var destination = String.Empty;
            do
            {
                Console.WriteLine();
                Console.Write("Please provide a name of the target file: ");
                destination = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(destination))
                {
                    Console.WriteLine("File cannot be empty.");
                    continue;
                }

            } while (string.IsNullOrWhiteSpace(destination));

            Console.WriteLine();
            Console.WriteLine("File is being sorted...");

            var watch = Stopwatch.StartNew();

            var xFile = new XFile();
            xFile.Sort(source, destination);

            watch.Stop();

            Console.WriteLine();
            Console.WriteLine("Done! Execution time: {0}", watch.Elapsed);

            Console.ReadKey();
        }
    }
}
