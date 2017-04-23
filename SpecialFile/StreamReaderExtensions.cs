using System.IO;

namespace SpecialFile
{
    public static class StreamReaderExtensions
    {
        public static XRow ReadXRow(this StreamReader reader)
        {
            var str = string.Empty;

            if ((str = reader.ReadLine()) != null)
            {
                return new XRow(str);
            }

            return null;
        }
    }
}
