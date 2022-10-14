using System;
using FF9;
using Memoria.Prime.CSV;

namespace Memoria.Data
{
    public sealed class CharacterCommandSet : ICsvEntry
    {
        public Int32 Id;
        public BattleCommandId Regular1;
        public BattleCommandId Regular2;
        public BattleCommandId Trance1;
        public BattleCommandId Trance2;

        public void ParseEntry(String[] raw)
        {
            Int32 index = 0;
            if (raw.Length <= 4)
                Id = -1;
            else
                Id = CsvParser.Int32(raw[index++]);
            Regular1 = (BattleCommandId)CsvParser.Byte(raw[index++]);
            Regular2 = (BattleCommandId)CsvParser.Byte(raw[index++]);
            Trance1 = (BattleCommandId)CsvParser.Byte(raw[index++]);
            Trance2 = (BattleCommandId)CsvParser.Byte(raw[index++]);
        }

        public void WriteEntry(CsvWriter sw)
        {
            sw.Int32(Id);
            sw.Byte((Byte)Regular1);
            sw.Byte((Byte)Regular2);
            sw.Byte((Byte)Trance1);
            sw.Byte((Byte)Trance2);
        }

        public BattleCommandId GetRegular(Int32 number)
        {
            if (number == 0)
                return Regular1;
            if (number == 1)
                return Regular2;
            throw new ArgumentOutOfRangeException(nameof(number), number, "Number must be 0 or 1.");
        }

        public BattleCommandId GetTrance(Int32 number)
        {
            if (number == 0)
                return Trance1;
            if (number == 1)
                return Trance2;
            throw new ArgumentOutOfRangeException(nameof(number), number, "Number must be 0 or 1.");
        }

        public BattleCommandId Get(Boolean isTrance, Int32 number)
        {
            return isTrance ? GetTrance(number) : GetRegular(number);
        }
    }
}