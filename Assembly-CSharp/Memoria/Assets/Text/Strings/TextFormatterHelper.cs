using System;
using System.Globalization;
using System.Text;

namespace Memoria.Assets
{
    public static class TextFormatterHelper
    {
        public static String CreateKey(this TxtEntry entry)
        {
            return CreateKey(entry.Prefix, entry.Index);
        }

        public static void SetKey(this TxtEntry entry, String key)
        {
            ParseKey(key, out entry.Prefix, out entry.Index);
        }

        public static String CreateKey(String prefix, Int32 index)
        {
            return prefix + index.ToString("D4", CultureInfo.InvariantCulture);
        }

        public static void ParseKey(String key, out String prefix, out Int32 index)
        {
            prefix = key.Length > 4
                ? key.Substring(startIndex: 0, length: key.Length - 4)
                : String.Empty;

            index = Int32.Parse(key.Substring(startIndex: key.Length - 4, length: 4), CultureInfo.InvariantCulture);
        }

        public static void ParseKey(StringBuilder key, out String prefix, out Int32 index)
        {
            prefix = key.Length > 4
                ? key.ToString(startIndex: 0, length: key.Length - 4)
                : String.Empty;

            index = Int32.Parse(key.ToString(startIndex: key.Length - 4, length: 4), CultureInfo.InvariantCulture);
        }
    }
}
