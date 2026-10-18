using System;
using Memoria.Prime.CSV;
using FF9;

namespace Memoria.Data
{
    public class ItemDefence : ICsvEntry
    {
        public String Comment;
        public Int32 Id;
        public ItemDefence Data;

        public Int32 PhysicalDefence;
        public Int32 PhysicalEvade;
        public Int32 MagicalDefence;
        public Int32 MagicalEvade;

        public static ItemDefence GetExisting(Int32 id)
        {
            if (ff9armor.ArmorData.TryGetValue(id, out ItemDefence result))
                return result;
            throw new NotSupportedException($"The option AppendMode must be used to patch existing entries but the entry {id} doesn't exist");
        }

        public void ParseDataEntry(String[] raw, CsvMetaData metadata, ref Int32 index)
        {
            if (metadata.HasField("PhysicalDefence")) PhysicalDefence = CsvParser.Int32(raw[index++]);
            if (metadata.HasField("PhysicalEvade")) PhysicalEvade = CsvParser.Int32(raw[index++]);
            if (metadata.HasField("MagicalDefence")) MagicalDefence = CsvParser.Int32(raw[index++]);
            if (metadata.HasField("MagicalEvade")) MagicalEvade = CsvParser.Int32(raw[index++]);
        }

        public void ParseEntry(String[] raw, CsvMetaData metadata)
        {
            Int32 index = 0;
            Comment = CsvParser.String(raw[index++]);
            Id = CsvParser.Int32(raw[index++]);
            Data = metadata.IsAppendMode ? GetExisting(Id) : this;
            Data.ParseDataEntry(raw, metadata, ref index);
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
