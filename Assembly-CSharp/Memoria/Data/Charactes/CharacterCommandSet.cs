using System;
using FF9;
using Memoria.Prime.CSV;

namespace Memoria.Data
{
    public sealed class CharacterCommandSet : ICsvEntry
    {
        public command_tags Regular1;
        public command_tags Regular2;
        public command_tags Trance1;
        public command_tags Trance2;

        public void ParseEntry(String[] raw)
        {
            Regular1 = (command_tags)CsvParser.Byte(raw[0]);
            Regular2 = (command_tags)CsvParser.Byte(raw[1]);
            Trance1 = (command_tags)CsvParser.Byte(raw[2]);
            Trance2 = (command_tags)CsvParser.Byte(raw[3]);
        }

        public void WriteEntry(CsvWriter sw)
        {
            sw.Byte((Byte)Regular1);
            sw.Byte((Byte)Regular2);
            sw.Byte((Byte)Trance1);
            sw.Byte((Byte)Trance2);
        }

        public command_tags GetRegular(Int32 number)
        {
            if (number == 0)
                return Regular1;
            if (number == 1)
                return Regular2;
            throw new ArgumentOutOfRangeException(nameof(number), number, "Number must be 0 or 1.");
        }

        public command_tags GetTrance(Int32 number)
        {
            if (number == 0)
                return Trance1;
            if (number == 1)
                return Trance2;
            throw new ArgumentOutOfRangeException(nameof(number), number, "Number must be 0 or 1.");
        }

        public command_tags Get(Boolean isTrance, Int32 number)
        {
            return isTrance ? GetTrance(number) : GetRegular(number);
        }
    }
}