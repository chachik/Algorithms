using Microsoft.VisualBasic.Devices;
using SortingAlgorithms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SpecialFile
{
    public class XFile
    {
        const string TempFileNameTemplate = "{0}.txt";
        const int DefaultXRowLength = 20;

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

            // Initialise paramet
            long currentSize = 0;
            var worker = new XFileWorker(destinationFile);

            var chunkMemoryLimit = (int)(getAvailableMemory() * 0.2 / Environment.ProcessorCount);

            var capacity = chunkMemoryLimit / DefaultXRowLength;
            XRow row = null;
            var rows = new List<XRow>(capacity);

            // Read source file
            using (var file = File.OpenText(sourceFile))
            {
                while ((row = file.ReadXRow()) != null)
                {
                    currentSize += row.String.Length;
                    rows.Add(row);

                    // Split tha data on chunks
                    if (currentSize > chunkMemoryLimit)
                    {
                        // Sort and save every chunk to a temporary file
                        worker.SortAndSave(rows);

                        // Clean lines buffer
                        currentSize = 0;
                        rows = new List<XRow>(capacity);

                        // Start merging the chanks to load CPU and I/O in parallel
                        worker.Merge();
                    }
                }
            }

            // Sort and save the last chunk
            if (rows.Count > 0)
            {
                worker.SortAndSave(rows, true);
            }

            // Merge all chunks
            worker.Merge(true);
        }

        private ulong getAvailableMemory()
        {
            return new ComputerInfo().AvailablePhysicalMemory;
        }

        private class XFileWorker
        {
            private Queue<Tuple<string, Task>> chunks = new Queue<Tuple<string, Task>>();
            private string fileName = string.Empty;
            private Task[] tasks = new Task[Environment.ProcessorCount];
            private int taskIndex = 0;
            private string destination = string.Empty;
            private int fileCounter = 0;
            private ISortingAlgorithm<XRow> algorithm = new QuickSort<XRow>();

            public XFileWorker(string destinationFile)
            {
                destination = destinationFile;
            }

            internal void SortAndSave(IList<XRow> rows, bool isLastChunk = false)
            {
                if (isLastChunk && fileCounter == 0)
                {
                    save(destination, algorithm.Sort(rows));
                    return;
                }

                taskIndex = (fileCounter < Environment.ProcessorCount) ? fileCounter : Task.WaitAny(tasks);
                fileName = string.Format(TempFileNameTemplate, ++fileCounter);
                tasks[taskIndex] = sortAndSaveAsync(fileName, rows);
                chunks.Enqueue(new Tuple<string, Task>(fileName, tasks[taskIndex]));
            }

            internal void Merge(bool lastMerge = false)
            {
                if (chunks.Count > 1)
                {
                    taskIndex = (fileCounter < Environment.ProcessorCount) ? fileCounter : Task.WaitAny(tasks);
                    fileName = (lastMerge && chunks.Count == 2) ? destination : string.Format(TempFileNameTemplate, ++fileCounter);
                    var chunk1 = chunks.Dequeue();
                    var chunk2 = chunks.Dequeue();
                    tasks[taskIndex] = mergeAsync(chunk1.Item1, chunk2.Item1, fileName, chunk1.Item2, chunk2.Item2);
                    if (!lastMerge || chunks.Count > 0)
                    {
                        chunks.Enqueue(new Tuple<string, Task>(fileName, tasks[taskIndex]));
                    }
                }

                if (lastMerge)
                {
                    Task.WaitAll(tasks);
                }
            }

            private async Task sortAndSaveAsync(string fileName, IList<XRow> list)
            {
                await Task.Run(() =>
                {
                    save(fileName, algorithm.Sort(list));
                });
            }

            private void save(string fileName, IList<XRow> list)
            {
                using (var file = new StreamWriter(fileName, false, Encoding.Default, 65536))
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        file.WriteLine(list[i]);
                    }
                }
            }

            private async Task mergeAsync(string source1, string source2, string destination, params Task[] tasks)
            {
                await Task.WhenAll(tasks);

                merge(source1, source2, destination);
            }

            private void merge(string source1, string source2, string destination)
            {
                using (var f1 = File.OpenText(source1))
                {
                    XRow l1 = f1.ReadXRow();

                    using (var f2 = File.OpenText(source2))
                    {
                        XRow l2 = f2.ReadXRow();

                        using (var d = new StreamWriter(destination, false, Encoding.Default, 65536))
                        {
                            while (l1 != null || l2 != null)
                            {
                                if (l1 == null)
                                {
                                    d.WriteLine(l2);
                                    l2 = f2.ReadXRow();
                                    continue;
                                }

                                if (l2 == null)
                                {
                                    d.WriteLine(l1);
                                    l1 = f1.ReadXRow();
                                    continue;
                                }

                                if (l1.CompareTo(l2) <= 0)
                                {
                                    d.WriteLine(l1);
                                    l1 = f1.ReadXRow();
                                    continue;
                                }

                                d.WriteLine(l2);
                                l2 = f2.ReadXRow();
                                continue;
                            }
                        }
                    }
                }

                File.Delete(source1);
                File.Delete(source2);
            }
        }
    }
}
