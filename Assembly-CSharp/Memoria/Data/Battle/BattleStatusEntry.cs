using System;
using System.Collections.Generic;
using Memoria.Prime.CSV;

namespace Memoria.Data
{
    public sealed class BattleStatusEntry : ICsvEntry
    {
        public String Comment;
        public StatusSetId Id;

        public BattleStatus Value;

        public void ParseEntry(String[] raw, CsvMetaData metadata)
        {
            Comment = CsvParser.String(raw[0]);
            Id = (StatusSetId)CsvParser.Int32(raw[1]);

            Value = ParseBattleStatus(raw[2], metadata);
        }

        public void WriteEntry(CsvWriter sw, CsvMetaData metadata)
        {
            sw.String(Comment);
            sw.Int32((Int32)Id);

            WriteBattleStatus(sw, metadata, Value);
        }

        public static BattleStatus ParseBattleStatus(String raw, CsvMetaData metadata)
        {
            BattleStatus result = 0;
            String[] tokens = raw.Split(',');
            for (Int32 i = 0; i < tokens.Length; i++)
            {
                String tok = tokens[i].Trim();
                if (String.IsNullOrEmpty(tok))
                    continue;

                if (metadata.HasOption("UnshiftStatuses"))
                    result |= CsvParser.EnumValue<BattleStatusId>(tok).ToBattleStatus();
                else
                    result |= CsvParser.EnumValue<BattleStatusIdOldVersion>(tok).ToBattleStatus();
            }
            return result;
        }

        public static void WriteBattleStatus(CsvWriter sw, CsvMetaData metadata, BattleStatus status)
        {
            List<String> statusStr = new List<String>();
            if (metadata.HasOption("UnshiftStatuses"))
            {
                foreach (BattleStatusId statusId in status.ToStatusList())
                    statusStr.Add($"{statusId}({(Int32)statusId})");
            }
            else
            {
                if (status == 0)
                {
                    statusStr.Add($"{BattleStatusIdOldVersion.None}({(Int32)BattleStatusIdOldVersion.None})");
                }
                else
                {
                    foreach (BattleStatusId statusId in status.ToStatusList())
                    {
                        BattleStatusIdOldVersion oldId = (BattleStatusIdOldVersion)(statusId + 1);
                        statusStr.Add($"{oldId}({(Int32)oldId})");
                    }
                }
            }
            sw.String(String.Join(", ", statusStr.ToArray()));
        }
    }
}
