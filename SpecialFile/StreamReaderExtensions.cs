using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SpecialFile
{
    public static class StreamReaderExtensions
    {
        private static string str = string.Empty;

        public static FileLine ReadFileLine(this StreamReader reader)
        {
            if ((str = reader.ReadLine()) != null)
            {
                return new FileLine(str);
            }

            return null;
        }
    }
}
