using System;
using System.Collections.Generic;
using System.Text;

namespace Memoria.Assets
{
    public class FieldFormatter
    {
        public readonly ExportFieldTags Tags = new ExportFieldTags();
        public readonly ExportFieldCharacterNames Names = ExportFieldCharacterNames.GetCurrent();
        public readonly Dictionary<String, String> Cache = new Dictionary<String, String>();

        public TxtEntry[] Build(String name, String[] lines)
        {
            String postfix = '$' + name + '_';
            List<TxtEntry> entries = new List<TxtEntry>(lines.Length);
            for (Int32 i = 0; i < lines.Length; i++)
            {
                String key;
                String line = GetLine(lines, ref i);
                TxtEntry entry = new TxtEntry {Index = entries.Count, Prefix = postfix};
                if (line == String.Empty)
                {
                    entry.Value = String.Empty;
                }
                else if (!Cache.TryGetValue(line, out key))
                {
                    key = StringsFormatter.FormatKeyIndex(postfix, entries.Count);
                    Cache.Add(line, key);
                    entry.Value = FormatValue(name, line);
                }
                else
                {
                    entry.Value = '{' + key + '}';
                }
                entries.Add(entry);
            }
            return entries.ToArray();
        }

        private String GetLine(String[] lines, ref Int32 index)
        {
            if (Configuration.Export.TextFormat <= 1)
                return lines[index];

            // Not supported https://github.com/Albeoris/Memoria/wiki/Different-identifiers-in-different-languages
            String line = lines[index];
            if (line.LastIndexOf("[ENDN]", StringComparison.Ordinal) >= 0)
                return line;

            StringBuilder sb = new StringBuilder(line.Length * 4);
            sb.Append(line);

            while (index < lines.Length - 1)
            {
                line = lines[++index];
                sb.Append(line);
                if (line.LastIndexOf("[ENDN]", StringComparison.Ordinal) >= 0)
                    break;
            }

            return sb.ToString();
        }

        private String FormatValue(String name, String line)
        {
            line = Tags.Replace(line);
            line = Names.Replace(name, line);
            return RemoveInternalTags(line);
        }

        private static String RemoveInternalTags(String str)
        {
            StringBuilder sb = new StringBuilder(str.Length);
            Int32 counter = 0;
            foreach (Char ch in str)
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