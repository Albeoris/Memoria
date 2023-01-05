using System;
using Memoria.Prime.CSV;

namespace Memoria.Data
{
    public class ItemDefence : ICsvEntry
    {
        public String Comment;
        public Int32 Id;

        public Byte PhisicalDefence;
        public Byte PhisicalEvade;
        public Byte MagicalDefence;
        public Byte MagicalEvade;

        public void ParseEntry(String[] raw, CsvMetaData metadata)
        {
            Comment = CsvParser.String(raw[0]);
            Id = CsvParser.Int32(raw[1]);

            PhisicalDefence = CsvParser.ByteOrMinusOne(raw[2]);
            PhisicalEvade = CsvParser.ByteOrMinusOne(raw[3]);
            MagicalDefence = CsvParser.ByteOrMinusOne(raw[4]);
            MagicalEvade = CsvParser.ByteOrMinusOne(raw[5]);
        }

        public void WriteEntry(CsvWriter writer, CsvMetaData metadata)
        {
            writer.String(Comment);
            writer.Int32(Id);

            writer.ByteOrMinusOne(PhisicalDefence);
            writer.ByteOrMinusOne(PhisicalEvade);
            writer.ByteOrMinusOne(MagicalDefence);
            writer.ByteOrMinusOne(MagicalEvade);
        }
    }
}
