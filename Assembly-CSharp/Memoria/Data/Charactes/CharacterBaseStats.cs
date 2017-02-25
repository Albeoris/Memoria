using System;
using Memoria.Prime.CSV;

namespace Memoria.Data
{
    public class CharacterBaseStats : ICsvEntry
    {
        public String Comment;
        public Int32 Id;

        public Byte Dexterity;
        public Byte Strength;
        public Byte Magic;
        public Byte Will;
        public Byte Gems;

        public void ParseEntry(String[] raw)
        {
            Comment = CsvParser.String(raw[0]);
            Id = CsvParser.Byte(raw[1]);

            Dexterity = CsvParser.Byte(raw[2]);
            Strength = CsvParser.Byte(raw[3]);
            Magic = CsvParser.Byte(raw[4]);
            Will = CsvParser.Byte(raw[5]);
            Gems = CsvParser.Byte(raw[6]);
        }

        public void WriteEntry(CsvWriter sw)
        {
            sw.String(Comment);
            sw.Int32(Id);

            sw.Byte(Dexterity);
            sw.Byte(Strength);
            sw.Byte(Magic);
            sw.Byte(Will);
            sw.Byte(Gems);
        }
    }
}