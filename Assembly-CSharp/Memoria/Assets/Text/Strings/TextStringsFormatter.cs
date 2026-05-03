using Memoria.Prime;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Memoria.Assets
{
    public sealed class TextStringsFormatter : ITextFormatter
    {
        public static ITextFormatter Instance { get; } = new TextStringsFormatter();

        public ITextWriter GetWriter() => new Writer();
        public ITextReader GetReader() => new Reader();

        private sealed class Writer : ITextWriter
        {
            public void WriteAll(String outputPath, IList<TxtEntry> entries)
            {
                using (StreamWriter sw = File.CreateText(outputPath))
                {
                    sw.WriteLine("/*");
                    sw.WriteLine(outputPath);
                    sw.WriteLine(entries.Count.ToString("D4", CultureInfo.InvariantCulture));
                    sw.WriteLine("*/");

                    foreach (TxtEntry entry in entries)
                    {
                        String key = TextFormatterHelper.CreateKey(entry.Prefix, entry.Index);
                        String value = entry.Value
                            .Replace("\\", "\\\\")
                            .Replace("\"", "\\\"");

                        sw.WriteLine($"\"{key}\" = \"{value}\";");
                    }
                }
            }
        }

        private sealed class Reader : ITextReader
        {
            public TxtEntry[] ReadAll(String inputPath)
            {
                using (StreamReader sr = File.OpenText(inputPath))
                {
                    String countStr = null;

                    Boolean comment = false;
                    for (int i = 0; i < 2 || comment;)
                    {
                        String value = sr.ReadLine();
                        if (value.StartsWith("/*"))
                        {
                            comment = true;
                            value = value.Substring(2);
                        }

                        if (value.EndsWith("*/"))
                        {
                            comment = false;
                            value = value.Substring(0, value.Length - 2);
                        }

                        value = value.Trim();

                        if (!String.IsNullOrEmpty(value))
                        {
                            if (i == 0)
                                _ = value; // file path, not used
                            else
                                countStr = value;

                            i++;
                        }
                    }

                    Int32 count = Int32.Parse(countStr, CultureInfo.InvariantCulture);
                    TxtEntry[] result = new TxtEntry[count];

                    Int32 offset = 0;
                    for (Int32 i = 0; i < count && !sr.EndOfStream; i++)
                    {
                        TxtEntry entry;
                        try
                        {
                            entry = Read(sr);
                        }
                        catch (Exception ex)
                        {
                            if (i > 0)
                            {
                                TxtEntry previous = result[i - 1];
                                throw new Exception($"Cannot read {i} entry from {inputPath}.\r\nPrevious: [Index: {previous.Index}, Pefix: {previous.Prefix}, Value: {previous.Value}]", ex);
                            }
                            else
                            {
                                throw new Exception($"Cannot read {i} entry from {inputPath}.", ex);

                            }
                        }

                        if (entry == null)
                        {
                            offset++;
                            continue;
                        }

                        if (String.IsNullOrEmpty(entry.Prefix))
                        {
                            Log.Warning("Invalid record [Line: {0}, Value: {1}] in the file: {2}", i, entry, inputPath);
                            offset++;
                            continue;
                        }

                        result[i - offset] = entry;
                    }

                    return result;
                }
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
}
