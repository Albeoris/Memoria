using Memoria.Prime.CSV;
using System;

namespace Memoria.Data
{
    public class CharacterBaseStats : ICsvEntry
    {
        public String Comment;
        public CharacterId Id;
        public CharacterBaseStats Data;

        public Byte Dexterity;
        public Byte Strength;
        public Byte Magic;
        public Byte Will;
        public UInt32 Gems;

        public static CharacterBaseStats GetExisting(CharacterId id)
        {
            if (ff9level.CharacterBaseStats.TryGetValue(id, out CharacterBaseStats result))
                return result;
            throw new NotSupportedException($"The option AppendMode must be used to patch existing entries but the entry {id} doesn't exist");
        }

        public void ParseDataEntry(String[] raw, CsvMetaData metadata, ref Int32 index)
        {
            if (metadata.HasField("Speed")) Dexterity = CsvParser.Byte(raw[index++]);
            if (metadata.HasField("Strength")) Strength = CsvParser.Byte(raw[index++]);
            if (metadata.HasField("Magic")) Magic = CsvParser.Byte(raw[index++]);
            if (metadata.HasField("Spirit")) Will = CsvParser.Byte(raw[index++]);
            if (metadata.HasField("Gems")) Gems = CsvParser.UInt32(raw[index++]);
        }

        public void ParseEntry(String[] raw, CsvMetaData metadata)
        {
            Int32 index = 0;
            Comment = CsvParser.String(raw[index++]);
            Id = (CharacterId)CsvParser.Byte(raw[index++]);
            Data = metadata.IsAppendMode ? GetExisting(Id) : this;
            Data.ParseDataEntry(raw, metadata, ref index);
        }

        public void WriteEntry(CsvWriter sw, CsvMetaData metadata)
        {
            sw.String(Comment);
            sw.Int32((Int32)Id);

            sw.Byte(Dexterity);
            sw.Byte(Strength);
            sw.Byte(Magic);
            sw.Byte(Will);
            sw.UInt32(Gems);
        }
    }
}
