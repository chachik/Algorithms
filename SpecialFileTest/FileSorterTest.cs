using SpecialFile;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace FileFactoryTest
{
    [TestClass]
    public class FileSorterTest
    {
        const string SourceFile = "TestFile1.txt";
        const string DestinationFile = "SortedTestFile1.txt"; 

        [TestMethod]
        public void SortShouldCreateDestinationFile()
        {
            // arrange
            var fileSorter = new FileSorter();

            // act
            fileSorter.Sort(SourceFile, DestinationFile);

            // assert
            Assert.IsTrue(File.Exists(DestinationFile));
        }

        [TestMethod]
        public void SortShouldCreateDestinationFileOfTheSameSize()
        {
            // arrange
            var fileSorter = new FileSorter();

            // act
            fileSorter.Sort(SourceFile, DestinationFile);

            // assert
            using (var sourceFile = File.Open(SourceFile, FileMode.Open))
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
            var fileSorter = new FileSorter();

            // act
            fileSorter.Sort(SourceFile, DestinationFile);

            // assert
            using (var destinationFile = File.OpenText(DestinationFile))
            {
                var str = String.Empty;
                FileLine cur = null;
                FileLine prev = null;

                while ((str = destinationFile.ReadLine()) != null)
                {
                    cur = new FileLine(str);

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
