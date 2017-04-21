using Microsoft.VisualBasic.Devices;
using SortingAlgorithms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecialFile
{
    /// <summary>
    /// The class represents a text File of the following structure:
    /// ------------------------------------------------------
    /// 135.Apple
    /// 11.Something something something
    /// 23.Cherry is the best
    /// 58.Banana is yellow
    /// ------------------------------------------------------
    /// </summary>
    public class XFile
    {
        const string TempFileNameTemplate = "{0}.txt";
        const int DefaultXRowLength = 20;

        /// <summary>
        /// Generates the file, where the first part of every row is a random number of a specified 
        /// range and the second part is a random string from a provided list.
        /// </summary>
        public void Generate(XFileGenerationOptions options)
        {
            // Validate configuration options
            if (options == null)
            {
                throw new ArgumentException("Options cannot be null.");
            }

            if (string.IsNullOrEmpty(options.FileName))
            {
                throw new ArgumentException("FileName cannot be empty.");
            }

            if (options.FileSize <= 0)
            {
                throw new ArgumentException("FileSize mast be greater than 0.");
            }

            if (options.Strings == null || options.Strings.Count == 0)
            {
                throw new ArgumentException("A list of Strings cannot be empty.");
            }

            if (options.MinNumber >= options.MaxNumber)
            {
                throw new ArgumentException("MinNumber must be less than MaxNumber.");
            }

            // Initialize generation parameters
            long fileSize = 0;
            var random = new Random();
            var maxStringsIndex = options.Strings.Count;

            // Generate file
            using (var file = new StreamWriter(options.FileName, false, Encoding.Default, 65536))
            {
                while (fileSize < options.FileSize)
                {
                    var number = random.Next(options.MinNumber, options.MaxNumber);
                    var stringIndex = random.Next(0, maxStringsIndex);
                    var line = string.Format("{0}.{1}", number, options.Strings[stringIndex]);

                    file.WriteLine(line);

                    fileSize += line.Length + 2;
                }
            }
        }

        /// <summary>
        /// The method sorts the source file and saves the result to the destination file.
        /// </summary>
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

            // Initialise paramets
            var fileWorker = new XFileWorker(destinationFile);
            var blockLimit = (int)(getAvailableMemory() * 0.2 / Environment.ProcessorCount);
            var blockCapacity = blockLimit / DefaultXRowLength;
            long currentblockSize = 0;
            XRow row = null;
            var rows = new List<XRow>(blockCapacity);

            // Read source file
            using (var file = File.OpenText(sourceFile))
            {
                while ((row = file.ReadXRow()) != null)
                {
                    currentblockSize += row.String.Length;
                    rows.Add(row);

                    // Split tha data on blocks
                    if (currentblockSize > blockLimit)
                    {
                        // Sort block and save it to a temporary file
                        fileWorker.SortAndSave(rows);

                        // Clean lines buffer
                        currentblockSize = 0;
                        rows = new List<XRow>(blockCapacity);

                        // Start merging the chanks to load CPU and I/O in parallel
                        //fileWorker.Merge();
                    }
                }
            }

            // Sort and save the last block
            if (rows.Count > 0)
            {
                fileWorker.SortAndSave(rows, true);
            }

            // Merge all blocks
            fileWorker.Merge(true);
        }

        private ulong getAvailableMemory()
        {
            return new ComputerInfo().AvailablePhysicalMemory;
        }

        private class XFileWorker
        {
            private Queue<Tuple<string, Task>> blocks = new Queue<Tuple<string, Task>>();
            private string fileName = string.Empty;
            private Task[] tasks = new Task[Environment.ProcessorCount];
            private int taskIndex = 0;
            private string finalDestination = string.Empty;
            private int fileCounter = 0;
            private ISortingAlgorithm<XRow> algorithm = new QuickSort<XRow>();

            public XFileWorker(string destinationFile)
            {
                finalDestination = destinationFile;
            }

            /// <summary>
            /// Sorts the block of rows and saves it to a temporary/destination file asynchronously.
            /// </summary>
            /// <param name="rows">A list of rows to proceed.</param>
            /// <param name="isLastBlock">True if it's a last block, othervise False.</param>
            internal void SortAndSave(IList<XRow> rows, bool isLastBlock = false)
            {
                if (isLastBlock && fileCounter == 0)
                {
                    save(finalDestination, algorithm.Sort(rows));
                    return;
                }

                taskIndex = (fileCounter < Environment.ProcessorCount) ? fileCounter : Task.WaitAny(tasks);
                fileName = string.Format(TempFileNameTemplate, ++fileCounter);
                tasks[taskIndex] = sortAndSaveAsync(fileName, rows);
                blocks.Enqueue(new Tuple<string, Task>(fileName, tasks[taskIndex]));
            }

            /// <summary>
            /// Merges the temporary files if they are available asynchronously.
            /// </summary>
            /// <param name="lastMerge">True if it is the last Merge call, othervise Fasle. 
            /// If it's the last merge the function merges all available files and waits for tasks completion.</param>
            internal void Merge(bool lastMerge = false)
            {
                Task.WaitAll(tasks.Where(t => t != null).ToArray());
                while (blocks.Count > 1)
                {
                    taskIndex = (fileCounter < Environment.ProcessorCount) ? fileCounter : Task.WaitAny(tasks);
                    fileName = (lastMerge && blocks.Count == 2) ? finalDestination : string.Format(TempFileNameTemplate, ++fileCounter);
                    var block1 = blocks.Dequeue();
                    var block2 = blocks.Dequeue();
                    tasks[taskIndex] = mergeAsync(block1.Item1, block2.Item1, fileName, block1.Item2, block2.Item2);

                    if (!lastMerge || blocks.Count > 0)
                    {
                        blocks.Enqueue(new Tuple<string, Task>(fileName, tasks[taskIndex]));
                    }

                    if (!lastMerge)
                    {
                        break;
                    }
                }

                if (lastMerge)
                {
                    Task.WaitAll(tasks.Where(t => t != null).ToArray());
                }
            }

            private async Task sortAndSaveAsync(string fileName, IList<XRow> list)
            {
                await Task.Run(() => { save(fileName, algorithm.Sort(list)); });
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

            private async Task mergeAsync(string source1, string source2, string destination, params Task[] tasksToWait)
            {
                await Task.WhenAll(tasksToWait);

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
