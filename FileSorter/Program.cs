using SpecialFile;
using System;
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
                Console.Write("Please provide a source file name: ");
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
                Console.Write("Please provide a target file name: ");
                destination = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(destination))
                {
                    Console.WriteLine("File cannot be empty.");
                    continue;
                }

            } while (string.IsNullOrWhiteSpace(destination));

            Console.WriteLine();
            Console.WriteLine("File is being sorted...");

            var xFile = new XFile();
            xFile.Sort(source, destination);

            Console.WriteLine();
            Console.WriteLine("Done!");

            Console.ReadKey();
        }
    }
}
