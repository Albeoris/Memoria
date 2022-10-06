using System;
using Memoria.Prime.CSV;

namespace Memoria.Data
{
    public sealed class CharacterCommand : ICsvEntry
    {
        public CharacterCommandType Type;
        public Byte Ability;
        public Byte[] Abilities;

        public void ParseEntry(String[] raw)
        {
            Type = (CharacterCommandType)CsvParser.Byte(raw[0]);
            Ability = CsvParser.Byte(raw[1]);
            Abilities = CsvParser.ByteArray(raw[2]);
        }

        public void WriteEntry(CsvWriter sw)
        {
            sw.Byte((Byte)Type);
            sw.Byte(Ability);
            sw.ByteArray(Abilities);
        }
    }
}