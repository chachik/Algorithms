using System;

namespace SpecialFile
{
    public class XRow : IComparable
    {
        public string String { get; private set; }

        public XRow(string text)
        {
            String = text;
        }

        public int CompareTo(object obj)
        {
            // Make a general validations
            var string2 = string.Empty;
            if (obj is XRow)
            {
                string2 = ((XRow)obj).String;
            }
            else if (obj is string)
            {
                string2 = (string)obj;
            }
            else
            {
                return -1;
            }

            var p1 = String.IndexOf('.');
            if (p1 < 0)
            {
                return 1;
            }

            var p2 = string2.IndexOf('.');
            if (p2 < 0)
            {
                return -1;
            }

            // Compaer String part
            var result = -1;
            var j = p2 + 1;
            for (int i = p1 + 1; i < String.Length; i++)
            {
                if (j >= string2.Length)
                {
                    return 1;
                }

                result = String[i].CompareTo(string2[j++]);
                if(result != 0)
                {
                    return result;
                }
            }
            if (j < string2.Length)
            {
                return -1;
            }

            // Compare Number part
            int n1 = 0;
            int.TryParse(String.Substring(0, p1), out n1);

            int n2 = 0;
            int.TryParse(string2.Substring(0, p2), out n2);

            return n1.CompareTo(n2);
        }

        public override string ToString()
        {
            return String;
        }
    }
}
