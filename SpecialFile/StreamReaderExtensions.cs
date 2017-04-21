using System.IO;

namespace SpecialFile
{
    public static class StreamReaderExtensions
    {
        private static string str = string.Empty;

        public static XRow ReadXRow(this StreamReader reader)
        {
            if ((str = reader.ReadLine()) != null)
            {
                return new XRow(str);
            }

            return null;
        }
    }
}
