using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Memoria.Prime.Exceptions;

namespace Memoria.Prime.CSV
{
    public static class CsvReader
    {
        public static T[] Read<T>(String filePath) where T : class, ICsvEntry, new()
        {
            using (Stream input = File.OpenRead(filePath))
                return Read<T>(input);
        }

        private static T[] Read<T>(Stream input) where T : class, ICsvEntry, new()
        {
            LinkedList<Exception> exceptions = new LinkedList<Exception>();
            LinkedList<T> entries = new LinkedList<T>();
            using (StreamReader sr = new StreamReader(input))
            {
                while (!sr.EndOfStream)
                {
                    String line = sr.ReadLine();
                    if (String.IsNullOrEmpty(line) || line[0] == '#')
                        continue;

                    try
                    {
                        String[] raw = line.Split(';');
                        for (Int32 i = 0; i < raw.Length; i++)
                        {
                            String col = raw[i];
                            if (col.Length > 0 && col[0] == '#')
                            {
                                Array.Resize(ref raw, i);
                                break;
                            }
                        }

                        T entry = new T();
                        entry.ParseEntry(raw);
                        entries.AddLast(entry);
                    }
                    catch (Exception ex)
                    {
                        exceptions.AddLast(new CsvParseException($"Failed to parse [{typeof(T).Name}] from line [{line}].", ex));
                        entries.AddLast((T)null);
                    }
                }
            }

            if (exceptions.Count == 0)
                return entries.ToArray();

            if (exceptions.Count == 1)
                throw exceptions.First.Value;

            throw new AggregateException(exceptions);
        }
    }
}