using Microsoft.VisualStudio.TestTools.UnitTesting;
using SortingAlgorithms;

namespace SortingAlgorithmsTest
{
    [TestClass]
    public class QuickSortTest
    {
        static int[] Array = new[] { 5, 3, 1, 8, 7, 1, 4, 9 };

        [TestMethod]
        public void SortShouldReturnArrayOfTheSameSize()
        {
            // arrange
            var algorithm = new QuickSort<int>();

            // act
            var result = algorithm.Sort(Array);

            // assert
            Assert.AreEqual(Array.Length, result.Count);
        }

        [TestMethod]
        public void SortShouldReturnSortedArray()
        {
            // arrange
            var algorithm = new QuickSort<int>();

            // act
            var result = algorithm.Sort(Array);

            // assert
            for (int i = 0; i < result.Count - 1; i++)
            {
                Assert.IsTrue(result[i] <= result[i+1]);
            }
        }
    }
}
