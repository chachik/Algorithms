﻿using Microsoft.VisualBasic.Devices;
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
        const int DefaultMemoryBufferLimit = 52428800; // 50 Mb  // 104857600; // 100 Mb
        const int DefaultLineLength = 20;

        private ISortingAlgorithm<XLine> algorithm = new QuickSort<XLine>();

        public int MemoryBufferLimit { get; set; }

        public XFile()
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

            // Initialise parameters
            long currentSize = 0;
            int fileCounter = 0;
            int processorCount = Environment.ProcessorCount;

            MemoryBufferLimit = (int)(GetAvailableMemory() * 0.1 / processorCount);

            var lines = new List<XLine>(MemoryBufferLimit/DefaultLineLength);
            var chunks = new Queue<string>();
            var str = String.Empty;
            var fileName = string.Empty;
            var tasks = new Task[processorCount];

            // Read source file
            using (var file = File.OpenText(sourceFile))
            {
                while ((str = file.ReadLine()) != null)
                {
                    currentSize += str.Length;

                    lines.Add(new XLine(str));

                    // Split tha data on chunks
                    if (currentSize > MemoryBufferLimit)
                    {
                        // Sort and save every chunk to a temporary file
                        var i = (fileCounter < processorCount) ? fileCounter : Task.WaitAny(tasks);

                        fileName = string.Format(TempFileNameTemplate, ++fileCounter);
                        tasks[i] = SortAndSaveAsync(fileName,lines);
                        chunks.Enqueue(fileName);

                        currentSize = 0;
                        lines = new List<XLine>(MemoryBufferLimit / DefaultLineLength);
                    }
                }
            }

            // Sort and save the last chunk
            if (lines.Count > 0)
            {
                if (fileCounter == 0)
                {
                    Save(destinationFile, algorithm.Sort(lines));
                    return;
                }

                var i = (fileCounter < processorCount) ? fileCounter : Task.WaitAny(tasks);

                fileName = string.Format(TempFileNameTemplate, ++fileCounter);
                tasks[i] = SortAndSaveAsync(fileName, lines);
                chunks.Enqueue(fileName);
            }

            // Merge all chunks
            while (chunks.Count > 1)
            {
                var i = (fileCounter < processorCount) ? fileCounter : Task.WaitAny(tasks);

                fileName = chunks.Count == 2 ? destinationFile : string.Format(TempFileNameTemplate, ++fileCounter);
                tasks[i] = MergeAsync(chunks.Dequeue(), chunks.Dequeue(), fileName);
                if (chunks.Count > 0)
                {
                    chunks.Enqueue(fileName);
                }
            }

            Task.WaitAll(tasks);
        }

        private async Task SortAndSaveAsync(string fileName, IList<XLine> list)
        {
            await Task.Run(() => 
            {
                Save(fileName, algorithm.Sort(list));
            });
        }

        private void Save(string fileName, IList<XLine> list)
        {
            using (var file = new StreamWriter(fileName, false, Encoding.Default, 65536))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    file.WriteLine(list[i]);
                }
            }
        }

        private async Task MergeAsync(string source1, string source2, string destination, params Task[] tasks)
        {
            await Task.WhenAll(tasks);

            Merge(source1, source2, destination);
        }

        private void Merge(string source1, string source2, string destination)
        {
            using (var f1 = File.OpenText(source1))
            {
                XLine l1 = f1.ReadFileLine();

                using (var f2 = File.OpenText(source2))
                {
                    XLine l2 = f2.ReadFileLine();

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

            File.Delete(source1);
            File.Delete(source2);
        }

        private ulong GetAvailableMemory()
        {
            return new ComputerInfo().AvailablePhysicalMemory;
        }
    }
}
