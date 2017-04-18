using System;

namespace FileGenerator
{
    class Program
    {
        static void Main(string[] args)
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

            Console.WriteLine();
            Console.WriteLine("Done!");

            Console.ReadKey();
        }
    }
}
