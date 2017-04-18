using System;
using System.IO;
using System.Text;

namespace SpecialFile
{
    /// <summary>
    /// The class generates a file of the following structure:
    /// ------------------------------------------------------
    /// 135.Apple
    /// 11.Something something something
    /// 23.Cherry is the best
    /// 58.Banana is yellow
    /// ------------------------------------------------------
    /// Where the first part is a random number of a specified 
    /// range and the second one is a random string from a provided list.
    /// </summary>
    public class FileGenerator
    {
        public FileGeneratorOptions Options { get; private set; }

        public FileGenerator(FileGeneratorOptions options)
        {
            Options = options;
        }

        public void Generate()
        {
            // Validate configuration options
            if (Options == null)
            {
                throw new ArgumentException("Options cannot be null.");
            }

            if (string.IsNullOrEmpty(Options.FileName))
            {
                throw new ArgumentException("FileName cannot be empty.");
            }

            if (Options.FileSize <= 0)
            {
                throw new ArgumentException("FileSize mast be greater than 0.");
            }

            if (Options.Strings == null || Options.Strings.Count == 0)
            {
                throw new ArgumentException("A list of Strings cannot be empty.");
            }

            if (Options.MinNumber >= Options.MaxNumber)
            {
                throw new ArgumentException("MinNumber must be less than MaxNumber.");
            }

            // Initialize generation parameters
            long fileSize = 0;
            var random = new Random();
            var maxStringsIndex = Options.Strings.Count;

            // Generate file
            using (var file = new StreamWriter(Options.FileName, false, Encoding.Default, 65536))
            {
                while (fileSize < Options.FileSize)
                {
                    var number = random.Next(Options.MinNumber, Options.MaxNumber);
                    var stringIndex = random.Next(0, maxStringsIndex);
                    var line = string.Format("{0}.{1}", number, Options.Strings[stringIndex]);

                    file.WriteLine(line);

                    fileSize += line.Length + 2;
                }
            }
        }
    }
}
