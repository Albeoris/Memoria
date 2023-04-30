using System;
using Memoria.Prime.CSV;

namespace Memoria.Data
{
    public sealed class CharacterCommandSet : ICsvEntry
    {
        public CharacterPresetId Id;
        public BattleCommandId Regular1;
        public BattleCommandId Regular2;
        public BattleCommandId Trance1;
        public BattleCommandId Trance2;

        public void ParseEntry(String[] raw, CsvMetaData metadata)
        {
            Int32 index = 0;

            if (metadata.HasOption($"Include{nameof(Id)}"))
                Id = (CharacterPresetId)CsvParser.Int32(raw[index++]);
            else
                Id = (CharacterPresetId)(-1);
            Regular1 = (BattleCommandId)CsvParser.Int32(raw[index++]);
            Regular2 = (BattleCommandId)CsvParser.Int32(raw[index++]);
            Trance1 = (BattleCommandId)CsvParser.Int32(raw[index++]);
            Trance2 = (BattleCommandId)CsvParser.Int32(raw[index++]);
        }

        public void WriteEntry(CsvWriter sw, CsvMetaData metadata)
        {
            if (metadata.HasOption($"Include{nameof(Id)}"))
                sw.Int32((Int32)Id);
            sw.Int32((Int32)Regular1);
            sw.Int32((Int32)Regular2);
            sw.Int32((Int32)Trance1);
            sw.Int32((Int32)Trance2);
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