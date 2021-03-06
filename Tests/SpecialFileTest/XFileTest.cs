﻿using SpecialFile;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Collections.Generic;

namespace SpecialFileTest
{
    [TestClass]
    public class XFileTest
    {
        const long FileSize = 1048576; // 1Mb
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
            var options = new XFileGenerationOptions { FileName = FileName, Strings = Strings, MinNumber = MinNumber, MaxNumber = MaxNumber, FileSize = FileSize };
            var file = new XFile();

            // act
            file.Generate(options);

            // assert
            Assert.IsTrue(File.Exists(FileName));
        }

        [TestMethod]
        public void GenerateShouldCreateFileOfSpecifiedSize()
        {
            // arrange
            const string FileName = "TestFile2.txt";
            var options = new XFileGenerationOptions { FileName = FileName, Strings = Strings, MinNumber = MinNumber, MaxNumber = MaxNumber, FileSize = FileSize };
            var xFile = new XFile();

            // act
            xFile.Generate(options);

            // assert
            var file = File.Open(FileName, FileMode.Open, FileAccess.Read);
            Assert.IsTrue(file.Length >= FileSize - 50 && file.Length <= FileSize + 50);
        }

        [TestMethod]
        public void SortShouldCreateDestinationFile()
        {
            // arrange
            const string FileName = "TestFile3.txt";
            const string DestinationFile = "SortedFile3.txt";

            var options = new XFileGenerationOptions { FileName = FileName, Strings = Strings, MinNumber = MinNumber, MaxNumber = MaxNumber, FileSize = FileSize };
            var xFile = new XFile();
            xFile.Generate(options);

            // act
            xFile.Sort(FileName, DestinationFile);

            // assert
            Assert.IsTrue(File.Exists(DestinationFile));
        }

        [TestMethod]
        public void SortShouldCreateDestinationFileOfTheSameSize()
        {
            // arrange
            const string FileName = "TestFile4.txt";
            const string DestinationFile = "SortedFile4.txt";

            var options = new XFileGenerationOptions { FileName = FileName, Strings = Strings, MinNumber = MinNumber, MaxNumber = MaxNumber, FileSize = FileSize };
            var xFile = new XFile();
            xFile.Generate(options);

            // act
            xFile.Sort(FileName, DestinationFile);

            // assert
            using (var sourceFile = File.Open(FileName, FileMode.Open))
            {
                using (var destinationFile = File.Open(DestinationFile, FileMode.Open))
                {
                    Assert.AreEqual(sourceFile.Length, destinationFile.Length);
                }
            }
        }

        [TestMethod]
        public void SortShouldCreateSortedFile()
        {
            // arrange
            const string FileName = "TestFile4.txt";
            const string DestinationFile = "SortedFile4.txt";

            var options = new XFileGenerationOptions { FileName = FileName, Strings = Strings, MinNumber = MinNumber, MaxNumber = MaxNumber, FileSize = FileSize };
            var xFile = new XFile();
            xFile.Generate(options);

            // act
            xFile.Sort(FileName, DestinationFile);

            // assert
            using (var destinationFile = File.OpenText(DestinationFile))
            {
                var str = String.Empty;
                XRow cur = null;
                XRow prev = null;

                while ((str = destinationFile.ReadLine()) != null)
                {
                    cur = new XRow(str);

                    if (prev != null)
                    {
                        Assert.IsTrue(prev.CompareTo(cur) <= 0);
                    }

                    prev = cur;
                }
            }
        }
    }
}
