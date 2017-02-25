using System;
using FF9;
using Memoria.Prime.CSV;

namespace Memoria.Data
{
    public sealed class CharacterAbilityGems : ICsvEntry
    {
        public String Comment;
        public Int32 Id;

        public Byte GemsCount;

        public void ParseEntry(String[] raw)
        {
            Comment = CsvParser.String(raw[0]);
            Id = CsvParser.Int32(raw[1]);

            GemsCount = CsvParser.Byte(raw[2]);
        }

        public void WriteEntry(CsvWriter writer)
        {
            writer.String(Comment);
            writer.Int32(Id);

            writer.Byte(GemsCount);
        }
    }
}