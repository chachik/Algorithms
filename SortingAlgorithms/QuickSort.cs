using System;
using System.Collections.Generic;

namespace SortingAlgorithms
{
    public class QuickSort<T> : ISortingAlgorithm<T> where T : IComparable
    {
        public IList<T> Sort(IList<T> list)
        {
            quickSort(list, 0, list.Count - 1);

            return list;
        }

        private void quickSort(IList<T> l, int lo, int hi)
        {
            if (lo < hi)
            {
                var p = partition(l, lo, hi);
                quickSort(l, lo, p);
                quickSort(l, p + 1, hi);
            }
        }

        private int partition(IList<T> l, int lo, int hi)
        {
            var pivot = l[lo];
            var i = lo - 1;
            var j = hi + 1;
            var t = l[0];

            do
            {
                do
                {
                    i = i + 1;
                }
                while (l[i].CompareTo(pivot) < 0);

                do
                {
                    j = j - 1;
                }
                while (l[j].CompareTo(pivot) > 0);

                if (i < j)
                {
                    // swap
                    t = l[i];
                    l[i] = l[j];
                    l[j] = t;
                }
            }
            while (i < j);

            return j;
        }
    }
}
