using SpecialFile;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileFactoryTest
{
    [TestClass]
    public class XRowTest
    {
        const long Number = 95;
        const string String = "Apple";

        [TestMethod]
        public void XRowShouldCompareProperlyCase1()
        {
            // arrange
            var text1 = "1.Banana";
            var text2 = "1.Apple";


            // act
            var result = new XRow(text1).CompareTo(text2);

            // assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void XRowShouldCompareProperlyCase2()
        {
            // arrange
            var text1 = "1.Apple"; 
            var text2 = "1.Banana";


            // act
            var result = new XRow(text1).CompareTo(text2);

            // assert
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void XRowShouldCompareProperlyCase3()
        {
            // arrange
            var text1 = "1.Apple";
            var text2 = "1.Apple";


            // act
            var result = new XRow(text1).CompareTo(text2);

            // assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void XRowShouldCompareProperlyCase4()
        {
            // arrange
            var text1 = "2.Apple";
            var text2 = "1.Apple";


            // act
            var result = new XRow(text1).CompareTo(text2);

            // assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void XRowShouldCompareProperlyCase5()
        {
            // arrange
            var text1 = "1.Apple";
            var text2 = "2.Apple";


            // act
            var result = new XRow(text1).CompareTo(text2);

            // assert
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void XRowShouldCompareProperlyCase6()
        {
            // arrange
            var text1 = "1.AppleA";
            var text2 = "1.Apple";


            // act
            var result = new XRow(text1).CompareTo(text2);

            // assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void XRowShouldCompareProperlyCase7()
        {
            // arrange
            var text1 = "1.Apple";
            var text2 = "1.AppleA";


            // act
            var result = new XRow(text1).CompareTo(text2);

            // assert
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void XRowShouldCompareProperlyCase8()
        {
            // arrange
            var text1 = "11.Apple";
            var text2 = "1.Apple";


            // act
            var result = new XRow(text1).CompareTo(text2);

            // assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void XRowShouldCompareProperlyCase9()
        {
            // arrange
            var text1 = "1.Apple";
            var text2 = "11.AppleA";


            // act
            var result = new XRow(text1).CompareTo(text2);

            // assert
            Assert.AreEqual(-1, result);
        }
    }
}
