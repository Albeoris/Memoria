using System;
using Memoria.Prime.CSV;

namespace Memoria.Data
{
    public sealed class BattleStatusEntry : ICsvEntry
    {
        public const Int32 SetsCount = 128;

        public String Comment;
        public BattleStatusIndex Id;

        public BattleStatus Value;

        public void ParseEntry(String[] raw, CsvMetaData metadata)
        {
            Comment = CsvParser.String(raw[0]);
            Id = (BattleStatusIndex)CsvParser.Int32(raw[1]);

            BattleStatus result = 0;
            for (Int32 index = 2; index < raw.Length; index++)
            {
                String value = raw[index];
                if (String.IsNullOrEmpty(value))
                    continue;

                foreach (String fullToken in value.Split(','))
                {
                    String token = fullToken.Trim();
                    if (String.IsNullOrEmpty(token))
                        continue;

                    Int32 number = (Int32)CsvParser.EnumValue<BattleStatusNumber>(token);
                    if (number == 0)
                        continue;

                    result |= (BattleStatus)(1 << checked(number - 1));
                }
            }
            Value = result;
        }

        public void WriteEntry(CsvWriter sw, CsvMetaData metadata)
        {
            sw.String(Comment);
            sw.Int32((Int32)Id);

            UInt32 flags = (UInt32)Value;
            if (flags == 0)
            {
                BattleStatusNumber number = 0;
                sw.EnumValue(number);
                return;
            }

            for (Int32 i = 0; i < 32; i++)
            {
                if ((flags & (1 << i)) != 0)
                {
                    BattleStatusNumber number = (BattleStatusNumber)(i + 1);
                    sw.EnumValue(number);
                }
            }
        }
    }
}