using System.Collections.Generic;

namespace SpecialFile
{
    public class XFileGenerationOptions
    {
        /// <summary>
        /// Gets or sets an inclusive lower bound for random number generation.
        /// </summary>
        public int MinNumber { get; set; }

        /// <summary>
        /// Gets or sets an exclusive upper bound for random number generation.
        /// </summary>
        public int MaxNumber { get; set; }

        /// <summary>
        /// Gets of sets a list of strings for random lines generation.
        /// </summary>
        public List<string> Strings { get; set; }

        /// <summary>
        /// Gets or sets a file name.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets a file size in bytes.
        /// </summary>
        public long FileSize { get; set; }
    }
}
