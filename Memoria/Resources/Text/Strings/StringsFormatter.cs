using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Memoria
{
    public sealed class StringsFormatter : ITxtFormatter
    {
        private static readonly StringsFormatter LazyInstance = new StringsFormatter();

        public static StringsFormatter Instance
        {
            get { return LazyInstance; }
        }

        public void Write(StreamWriter sw, TxtEntry entry)
        {
            sw.WriteLine("\"{0}\" = \"{1}\";",
                FormatKeyIndex(entry.Prefix, entry.Index),
                entry.Value.Replace("\\", "\\\\").Replace("\"", "\\\""));
        }

        public static String FormatKeyIndex(String prefix, Int32 index)
        {
            return prefix + index.ToString("D4", CultureInfo.InvariantCulture);
        }

        public TxtEntry Read(StreamReader sr)
        {
            Int32 index = -1;
            TxtEntry result = new TxtEntry();

            StringBuilder sb = new StringBuilder(512);
            Boolean key = true;
            Boolean block = false;
            Boolean escape = false;
            Int32 line = 0;
            while (true)
            {
                int value = sr.Read();
                if (value < 0)
                {
                    if (sb.Length == 0)
                        return null;

                    throw Exceptions.CreateException("Неожиданный конец потока.");
                }

                char ch = (char)value;
                switch (ch)
                {
                    case '\\':
                    {
                        if (!block)
                            continue;

                        if (escape)
                        {
                            sb.Append('\\');
                            escape = false;
                        }
                        else
                        {
                            escape = true;
                        }
                        break;
                    }
                    case '"':
                    {
                        if (escape)
                        {
                            sb.Append('"');
                            escape = false;
                        }
                        else
                        {
                            if (block)
                            {
                                if (key)
                                {
                                    if (sb.Length > 4)
                                        result.Prefix = sb.ToString(0, sb.Length - 4);

                                    index = int.Parse(sb.ToString(sb.Length - 4, 4), CultureInfo.InvariantCulture);
                                    key = false;
                                }
                                else
                                {
                                    result.Index = index;
                                    result.Value = sb.ToString();
                                    return result;
                                }
                                block = false;
                                sb.Length = 0;
                            }
                            else
                            {
                                block = true;
                            }
                        }
                        break;
                    }
                    case '\r':
                    case '\n':
                    {
                        if (!block)
                            continue;

                        line++;
                        break;
                    }
                    default:
                    {
                        if (!block)
                            continue;

                        if (line > 0)
                        {
                            for (int i = 0; i < (line + 1) / 2; i++)
                                sb.Append(Environment.NewLine);
                            line = 0;
                        }

                        sb.Append(ch);
                        break;
                    }
                }
            }
        }
    }
}