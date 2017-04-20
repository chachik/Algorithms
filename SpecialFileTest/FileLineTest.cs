using SpecialFile;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileFactoryTest
{
    [TestClass]
    public class LineTest
    {
        const long Number = 95;
        const string String = "Apple";

        [TestMethod]
        public void FileLineShouldCompareProperlyCase1()
        {
            // arrange
            var text1 = "1.Banana";
            var text2 = "1.Apple";


            // act
            var result = new FileLine(text1).CompareTo(text2);

            // assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void FileLineShouldCompareProperlyCase2()
        {
            // arrange
            var text1 = "1.Apple"; 
            var text2 = "1.Banana";


            // act
            var result = new FileLine(text1).CompareTo(text2);

            // assert
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void FileLineShouldCompareProperlyCase3()
        {
            // arrange
            var text1 = "1.Apple";
            var text2 = "1.Apple";


            // act
            var result = new FileLine(text1).CompareTo(text2);

            // assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void FileLineShouldCompareProperlyCase4()
        {
            // arrange
            var text1 = "2.Apple";
            var text2 = "1.Apple";


            // act
            var result = new FileLine(text1).CompareTo(text2);

            // assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void FileLineShouldCompareProperlyCase5()
        {
            // arrange
            var text1 = "1.Apple";
            var text2 = "2.Apple";


            // act
            var result = new FileLine(text1).CompareTo(text2);

            // assert
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void FileLineShouldCompareProperlyCase6()
        {
            // arrange
            var text1 = "1.AppleA";
            var text2 = "1.Apple";


            // act
            var result = new FileLine(text1).CompareTo(text2);

            // assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void FileLineShouldCompareProperlyCase7()
        {
            // arrange
            var text1 = "1.Apple";
            var text2 = "1.AppleA";


            // act
            var result = new FileLine(text1).CompareTo(text2);

            // assert
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void FileLineShouldCompareProperlyCase8()
        {
            // arrange
            var text1 = "11.Apple";
            var text2 = "1.Apple";


            // act
            var result = new FileLine(text1).CompareTo(text2);

            // assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void FileLineShouldCompareProperlyCase9()
        {
            // arrange
            var text1 = "1.Apple";
            var text2 = "11.AppleA";


            // act
            var result = new FileLine(text1).CompareTo(text2);

            // assert
            Assert.AreEqual(-1, result);
        }
    }
}
