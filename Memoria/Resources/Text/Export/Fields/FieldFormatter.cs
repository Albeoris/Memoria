using System;
using System.Collections.Generic;
using System.Text;

namespace Memoria
{
    public class FieldFormatter
    {
        public readonly FieldTags Tags = new FieldTags();
        public readonly FieldCharacterNames Names = FieldCharacterNames.GetCurrent();
        public readonly Dictionary<String, String> Cache = new Dictionary<String, String>();

        public TxtEntry[] Build(String name, String[] lines)
        {
            String postfix = '$' + name + '_';
            TxtEntry[] entries = new TxtEntry[lines.Length];
            for (int i = 0; i < entries.Length; i++)
            {
                String key;
                String line = lines[i];
                TxtEntry entry = new TxtEntry {Index = i, Prefix = postfix};
                if (line == String.Empty)
                {
                    entry.Value = String.Empty;
                }
                else if (!Cache.TryGetValue(line, out key))
                {
                    key = StringsFormatter.FormatKeyIndex(postfix, i);
                    Cache.Add(line, key);
                    entry.Value = FormatValue(name, line);
                }
                else
                {
                    entry.Value = '{' + key + '}';
                }
                entries[i] = entry;
            }
            return entries;
        }

        private string FormatValue(String name, String line)
        {
            line = Tags.Replace(line);
            line = Names.Replace(name, line);
            return RemoveInternalTags(line);
        }

        private static string RemoveInternalTags(String str)
        {
            StringBuilder sb = new StringBuilder(str.Length);
            int counter = 0;
            foreach (char ch in str)
            {
                if (ch == '{')
                {
                    if (counter++ == 0)
                        sb.Append(ch);
                }
                else if (ch == '}')
                {
                    if (--counter == 0)
                        sb.Append(ch);
                }
                else
                {
                    sb.Append(ch);
                }
            }
            return sb.ToString();
        }
    }
}