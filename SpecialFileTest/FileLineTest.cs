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
        public void FileLineShouldParseText()
        {
            // arrange
            var text = string.Format("{0}.{1}", Number, String);

            // act
            var line = new FileLine(text);

            // assert
            Assert.AreEqual(Number, line.Number);
            Assert.AreEqual(String, line.String);
        }

        [TestMethod]
        public void FileLineShouldIgnoreTextInWrongFormat()
        {
            // arrange
            var text1 = "WrongFormat";
            var text2 = (string)null;
            var text3 = string.Format("WrongNumber.{0}", String);

            // act
            var line1 = new FileLine(text1);
            var line2 = new FileLine(text2);
            var line3 = new FileLine(text3);

            // assert
            Assert.AreEqual(0, line1.Number);
            Assert.IsNull(line1.String);

            Assert.AreEqual(0, line2.Number);
            Assert.IsNull(line2.String);

            Assert.AreEqual(0, line3.Number);
            Assert.AreEqual(String, line3.String);
        }

        [TestMethod]
        public void FileLineShouldBeFormatedProperlyToString()
        {
            // arrange
            var text = string.Format("{0}.{1}", Number, String);

            // act
            var line = new FileLine(text);

            // assert
            Assert.AreEqual(text, line.ToString());
        }
    }
}
