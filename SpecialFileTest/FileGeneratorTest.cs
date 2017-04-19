using SpecialFile;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;

namespace FileFactoryTest
{
    [TestClass]
    public class FileGeneratorTest
    {
        const long FileSize = 1048576000; // 1 Gb  // 10737418240; // 10Gb
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

        [TestMethod]
        public void GenerateShouldCreateFile()
        {
            // arrange
            const string FileName = "TestFile1.txt";
            var options = new FileGeneratorOptions { FileName = FileName, Strings = Strings, MinNumber = MinNumber, MaxNumber = MaxNumber, FileSize = FileSize };
            var fileGenerator = new FileGenerator(options);

            // act
            fileGenerator.Generate();

            // assert
            Assert.IsTrue(File.Exists(FileName));
        }

        [TestMethod]
        public void GenerateShouldCreateFileOfSpecifiedSize()
        {
            // arrange
            const string FileName = "TestFile2.txt";
            var options = new FileGeneratorOptions { FileName = FileName, Strings = Strings, MinNumber = MinNumber, MaxNumber = MaxNumber, FileSize = FileSize };
            var fileGenerator = new FileGenerator(options);

            // act
            fileGenerator.Generate();

            // assert
            var file = File.Open(FileName, FileMode.Open, FileAccess.Read);
            Assert.IsTrue(file.Length >= FileSize - 50 && file.Length <= FileSize + 50);
        }
    }
}
