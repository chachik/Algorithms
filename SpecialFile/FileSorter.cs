using SortingAlgorithms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SpecialFile
{
    public class FileSorter
    {
        const string TempFileName = "{0}.txt";
        const int DefaultMemoryBufferLimit = 104857600; // 100 Mb
        const int DefaultLineLength = 20;

        private ISortingAlgorithm<FileLine> algorithm = new QuickSort<FileLine>();

        public int MemoryBufferLimit { get; set; }

        public FileSorter()
        {
            MemoryBufferLimit = DefaultMemoryBufferLimit;
        }

        public void Sort(string sourceFile, string destinationFile)
        {
            // Validate arguments
            if (string.IsNullOrWhiteSpace(sourceFile))
            {
                throw new ArgumentException("sourceFile cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(destinationFile))
            {
                throw new ArgumentException("destinationFile cannot be empty.");
            }

            long currentSize = 0;
            int fileCount = 0;

            // Read filest
            var list = new List<FileLine>(MemoryBufferLimit/DefaultLineLength);
            var queue = new Queue<Tuple<string, Task>>();

            using (var file = File.OpenText(sourceFile))
            {
                var str = String.Empty;

                while ((str = file.ReadLine()) != null)
                {
                    currentSize += str.Length;

                    list.Add(new FileLine(str));

                    if (currentSize >= MemoryBufferLimit)
                    {
                        var fileName = string.Format(TempFileName, ++fileCount);
                        var task = SortAndSaveAsync(fileName, list.ToArray());
                        queue.Enqueue(new Tuple<string, Task> (fileName, task));

                        currentSize = 0;
                        list = new List<FileLine>(MemoryBufferLimit / DefaultLineLength);

                        if (queue.Count > 1)
                        {
                            var fileName2 = string.Format(TempFileName, ++fileCount);
                            var item1 = queue.Dequeue();
                            var item2 = queue.Dequeue();
                            var task2 = MergeAsync(item1.Item1, item2.Item1, fileName2, item1.Item2, item2.Item2);
                            queue.Enqueue(new Tuple<string, Task>(fileName2, task2));
                        }
                    }
                }
            }

            if (list.Count > 0)
            {
                if (fileCount == 0)
                {
                    SortAndSave(destinationFile, list.ToArray());

                    return;
                }

                var fileName = string.Format(TempFileName, ++fileCount);
                var task = SortAndSaveAsync(fileName, list);
                queue.Enqueue(new Tuple<string, Task>(fileName, task));
            }

            while (queue.Count > 1)
            {
                var fileName = queue.Count == 2 ? destinationFile : string.Format(TempFileName, ++fileCount);
                var item1 = queue.Dequeue();
                var item2 = queue.Dequeue();
                var task = MergeAsync(item1.Item1, item2.Item1, fileName, item1.Item2, item2.Item2);
                if (queue.Count > 0)
                {
                    queue.Enqueue(new Tuple<string, Task>(fileName, task));
                }
                else
                {
                    task.Wait();
                }
            }
        }

        private async Task SortAndSaveAsync(string fileName, IList<FileLine> list)
        {
            await Task.Run(() => { SortAndSave(fileName, list); });
        }

        private void SortAndSave(string fileName, IList<FileLine> list)
        {
            var result = algorithm.Sort(list);
            using (var file = new StreamWriter(fileName, false, Encoding.Default, 65536))
            {
                foreach (var line in result)
                {
                    file.WriteLine(line);
                }
            }
        }

        private async Task MergeAsync(string file1, string file2, string destination, params Task[] tasks)
        {
            await Task.WhenAll(tasks);

            Merge(file1, file2, destination);
        }

        private void Merge(string file1, string file2, string destination)
        {
            using (var f1 = File.OpenText(file1))
            {
                FileLine l1 = f1.ReadFileLine();

                using (var f2 = File.OpenText(file2))
                {
                    FileLine l2 = f2.ReadFileLine();

                    using (var d = new StreamWriter(destination, false, Encoding.Default, 65536))
                    {
                        while (l1 != null || l2 != null)
                        {
                            if (l1 == null)
                            {
                                d.WriteLine(l2);
                                l2 = f2.ReadFileLine();
                                continue;
                            }

                            if (l2 == null)
                            {
                                d.WriteLine(l1);
                                l1 = f1.ReadFileLine();
                                continue;
                            }

                            if (l1.CompareTo(l2) <= 0)
                            {
                                d.WriteLine(l1);
                                l1 = f1.ReadFileLine();
                                continue;
                            }

                            d.WriteLine(l2);
                            l2 = f2.ReadFileLine();
                            continue;
                        }
                    }
                }
            }

            File.Delete(file1);
            File.Delete(file2);
        }
    }
}
