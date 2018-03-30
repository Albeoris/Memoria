using System;
using System.Globalization;
using System.IO;
using System.Text;
using Memoria.Prime.Exceptions;

namespace Memoria.Assets
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
            try
            {
                while (true)
                {
                    Int32 value = sr.Read();
                    if (value < 0)
                    {
                        if (sb.Length == 0)
                            return null;

                        throw new Exception("Unexpected end of stream.");
                    }

                    Char ch = (Char)value;
                    switch (ch)
                    {
                        case '\\':
                        {
                            if (!block)
                                continue;

                            if (escape)
                            {
                                AppendLines(ref line, sb);
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
                                AppendLines(ref line, sb);
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

                                        index = Int32.Parse(sb.ToString(sb.Length - 4, 4), CultureInfo.InvariantCulture);
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
                        {
                            if (!block)
                                continue;

                            break;
                        }
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

                            if (escape)
                            {
                                sb.Append('\\');
                                escape = false;
                            }

                            AppendLines(ref line, sb);
                            sb.Append(ch);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Cannot parse .strings entry.\r\n[Index: {result.Index}, Prefix: {result.Prefix}, Value: {result.Value}]\r\n[Buffer: {sb}, Line: {line} IsKey: {key}, IsBlock: {block}, IsEscape: {escape}]", ex);
            }
        }

        private static void AppendLines(ref Int32 line, StringBuilder sb)
        {
            if (line > 0)
            {
                for (Int32 i = 0; i < line; i++)
                    sb.Append('\n');
                line = 0;
            }
        }
    }
}