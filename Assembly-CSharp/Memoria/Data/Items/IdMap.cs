using System;
using Memoria.Prime.CSV;

namespace Memoria.Data
{
    public sealed class IdMap : ICsvEntry
    {
        public String Comment;
        public Int32 Id;

        public Int32 MappedId;

        public void ParseEntry(String[] raw)
        {
            Comment = CsvParser.String(raw[0]);
            Id = CsvParser.Int32(raw[1]);

            MappedId = CsvParser.Int32(raw[2]);
        }

        public void WriteEntry(CsvWriter sw)
        {
            sw.String(Comment);
            sw.Int32(Id);

            sw.Int32(MappedId);
        }
    }
}