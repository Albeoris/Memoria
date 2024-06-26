using Memoria.Prime.CSV;
using System;

namespace Memoria.Data
{
    public class ItemDefence : ICsvEntry
    {
        public String Comment;
        public Int32 Id;

        public Int32 PhysicalDefence;
        public Int32 PhysicalEvade;
        public Int32 MagicalDefence;
        public Int32 MagicalEvade;

        public void ParseEntry(String[] raw, CsvMetaData metadata)
        {
            Comment = CsvParser.String(raw[0]);
            Id = CsvParser.Int32(raw[1]);

            PhysicalDefence = CsvParser.Int32(raw[2]);
            PhysicalEvade = CsvParser.Int32(raw[3]);
            MagicalDefence = CsvParser.Int32(raw[4]);
            MagicalEvade = CsvParser.Int32(raw[5]);
        }

        public void WriteEntry(CsvWriter writer, CsvMetaData metadata)
        {
            writer.String(Comment);
            writer.Int32(Id);

            writer.Int32(PhysicalDefence);
            writer.Int32(PhysicalEvade);
            writer.Int32(MagicalDefence);
            writer.Int32(MagicalEvade);
        }
    }
}
