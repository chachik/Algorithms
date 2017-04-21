using System;

namespace SpecialFile
{
    public class XLine : IComparable
    {
        public long Number { get; private set; }
        public string String { get; private set; }

        public XLine(string text)
        {
            if (text == null)
            {
                return;
            }

            var subStrings = text.Split('.');

            if (subStrings.Length == 2)
            {
                var number = 0;
                if (int.TryParse(subStrings[0], out number))
                {
                    Number = number;
                }
                
                String = subStrings[1];
            }
        }

        public int CompareTo(object obj)
        {
            var line = obj as XLine;

            if (line == null)
            {
                return -1;
            }

            var result = this.String.CompareTo(line.String);
            if (result == 0)
            {
                result = this.Number.CompareTo(line.Number);
            }

            return result;
        }

        public override string ToString()
        {
            return string.Format("{0}.{1}", Number, String);
        }
    }
}
