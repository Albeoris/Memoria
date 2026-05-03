using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Memoria.Assets
{
    public sealed class TextResjsonFormatter : ITextFormatter
    {
        public static ITextFormatter Instance { get; } = new TextResjsonFormatter();

        public ITextWriter GetWriter() => new Writer();
        public ITextReader GetReader() => new Reader();

        private sealed class Writer : ITextWriter
        {
            public void WriteAll(String outputPath, IList<TxtEntry> entries)
            {
                using (StreamWriter sw = File.CreateText(outputPath))
                using (JsonTextWriter writer = new(sw))
                {
                    writer.Formatting = Formatting.Indented;
                    writer.WriteStartObject();
                    {
                        foreach (TxtEntry entry in entries)
                        {
                            writer.WritePropertyName(entry.CreateKey());
                            writer.WriteValue(entry.Value);
                        }
                    }
                    writer.WriteEndObject();
                }
            }
        }

        private sealed class Reader : ITextReader
        {
            public TxtEntry[] ReadAll(String inputPath)
            {
                List<TxtEntry> entries = new(capacity: 128);

                using (StreamReader sr = File.OpenText(inputPath))
                using (JsonTextReader reader = new(sr))
                {
                    if (!reader.Read())
                        return entries.ToArray(); // File is empty

                    if (reader.TokenType != JsonToken.StartObject)
                        throw new FormatException("if (json.TokenType != JsonToken.StartObject)");

                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonToken.PropertyName)
                        {
                            String key = (String)reader.Value;
                            if (!reader.Read() || reader.TokenType != JsonToken.String)
                                throw new FormatException("if (!reader.Read() || reader.TokenType != JsonToken.String)");

                            String value = (String)reader.Value;
                            TxtEntry entry = new();
                            entry.SetKey(key);
                            entry.Value = value;
                            entries.Add(entry);
                        }
                    }
                }

                return entries.ToArray();
            }
        }
    }
}
