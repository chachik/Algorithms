using System;
using System.Collections.Generic;

namespace SortingAlgorithms
{
    public interface ISortingAlgorithm<T> where T : IComparable
    {
        IList<T> Sort(IList<T> list);
    }
}
