using System;
using System.Globalization;
using FF9;
using Memoria.Prime;
using Memoria.Prime.CSV;

namespace Memoria.Data
{
    public sealed class BattleStatusDataEntry : ICsvEntry
    {
        public const Int32 StatusCount = 32;

        public String Comment;
        public Int32 Id;

        public STAT_DATA Value;

        public void ParseEntry(String[] raw)
        {
            Comment = CsvParser.String(raw[0]);
            Id = CsvParser.Int32(raw[1]);

            Byte priority = CsvParser.Byte(raw[2]);
            Byte opr_cnt = CsvParser.Byte(raw[3]);
            UInt16 conti_cnt = CsvParser.UInt16(raw[4]);
            BattleStatus clear = 0;
            BattleStatus invalid = 0;
            String[] clearArray = raw[5].Split(',');
            for (Int32 i = 0; i < clearArray.Length; i++)
			{
                String clearToken = clearArray[i].Trim();
                if (String.IsNullOrEmpty(clearToken))
                    continue;

                Int32 clearNumber = (Int32)CsvParser.EnumValue<BattleStatusNumber>(clearToken);
                if (clearNumber == 0)
                    continue;

                clear |= (BattleStatus)(1 << checked(clearNumber - 1));
            }
            String[] invalidArray = raw[6].Split(',');
            for (Int32 i = 0; i < invalidArray.Length; i++)
            {
                String invalidToken = invalidArray[i].Trim();
                if (String.IsNullOrEmpty(invalidToken))
                    continue;

                Int32 invalidNumber = (Int32)CsvParser.EnumValue<BattleStatusNumber>(invalidToken);
                if (invalidNumber == 0)
                    continue;

                invalid |= (BattleStatus)(1 << checked(invalidNumber - 1));
            }
            Value = new STAT_DATA(priority, opr_cnt, conti_cnt, clear, invalid);
        }

        public void WriteEntry(CsvWriter sw)
        {
            sw.String(Comment);
            sw.Int32(Id);

            sw.Byte(Value.priority);
            sw.Byte(Value.opr_cnt);
            sw.UInt16(Value.conti_cnt);

            foreach (UInt32 flags in new[] { (UInt32)Value.clear, (UInt32)Value.invalid })
            {
                if (flags == 0)
                {
                    BattleStatusNumber number = 0;
                    sw.EnumValue(number);
                    continue;
                }
                String arrayStat = "";
                for (Int32 i = 0; i < 32; i++)
                {
                    if ((flags & (1 << i)) != 0)
                    {
                        BattleStatusNumber number = (BattleStatusNumber)(i + 1);
                        if (arrayStat.Length > 0)
                            arrayStat += ", ";
                        arrayStat += number;
                        arrayStat += "(";
                        arrayStat += EnumCache<BattleStatusNumber>.ToUInt64(number).ToString(CultureInfo.InvariantCulture);
                        arrayStat += ")";
                    }
                }
                sw.String(arrayStat);
            }
        }
    }
}