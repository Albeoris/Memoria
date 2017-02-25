using System;
using FF9;
using Memoria.Prime.CSV;

namespace Memoria.Data
{
    public sealed class CharacterAbility : ICsvEntry
    {
        public Byte Id;
        public Byte Ap;

        public Byte ActiveId => Id;
        public Byte PassiveId => checked((Byte)(Id - 192));
        public Boolean IsPassive => Id >= 192;

        public void ParseEntry(String[] raw)
        {
            Id = CsvParser.Byte(raw[0]);
            Ap = CsvParser.Byte(raw[1]);
        }

        public void WriteEntry(CsvWriter writer)
        {
            writer.Byte(Id);
            writer.Byte(Ap);
        }
    }
}