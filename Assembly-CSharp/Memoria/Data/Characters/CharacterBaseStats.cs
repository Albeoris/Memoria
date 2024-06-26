using Memoria.Prime.CSV;
using System;

namespace Memoria.Data
{
    public class CharacterBaseStats : ICsvEntry
    {
        public String Comment;
        public CharacterId Id;

        public Byte Dexterity;
        public Byte Strength;
        public Byte Magic;
        public Byte Will;
        public Byte Gems;

        public void ParseEntry(String[] raw, CsvMetaData metadata)
        {
            Comment = CsvParser.String(raw[0]);
            Id = (CharacterId)CsvParser.Byte(raw[1]);

            Dexterity = CsvParser.Byte(raw[2]);
            Strength = CsvParser.Byte(raw[3]);
            Magic = CsvParser.Byte(raw[4]);
            Will = CsvParser.Byte(raw[5]);
            Gems = CsvParser.Byte(raw[6]);
        }

        public void WriteEntry(CsvWriter sw, CsvMetaData metadata)
        {
            sw.String(Comment);
            sw.Int32((Int32)Id);

            sw.Byte(Dexterity);
            sw.Byte(Strength);
            sw.Byte(Magic);
            sw.Byte(Will);
            sw.Byte(Gems);
        }
    }
}
