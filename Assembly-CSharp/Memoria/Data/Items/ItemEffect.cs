using FF9;
using Memoria.Prime.CSV;
using System;
using System.Collections.Generic;

namespace Memoria.Data
{
    public sealed class ItemEffect : ICsvEntry
    {
        public Int32 Id;
        public ITEM_DATA Data;

        public static ITEM_DATA GetExisting(Int32 id)
        {
            if (ff9item._FF9Item_Info.TryGetValue(id, out ITEM_DATA result))
                return result;
            throw new NotSupportedException($"The option AppendMode must be used to patch existing entries but the entry {id} doesn't exist");
        }

        public void ParseEntry(String[] raw, CsvMetaData metadata)
        {
            Int32 index = 0;

            if (metadata.HasOption($"IncludeId") || metadata.IsAppendMode)
                Id = CsvParser.Int32(raw[index++]);
            else
                Id = -1;

            Data = metadata.IsAppendMode ? GetExisting(Id) : new ITEM_DATA(new BattleCommandInfo(), new BTL_REF(), 0);

            if (metadata.HasField("Targets")) Data.info.Target = (TargetType)CsvParser.Byte(raw[index++]);
            if (metadata.HasField("DefaultAlly")) Data.info.DefaultAlly = CsvParser.Boolean(raw[index++]);
            if (metadata.HasField("Display")) Data.info.DisplayStats = (TargetDisplay)CsvParser.Byte(raw[index++]);
            if (metadata.HasField("AnimationId")) Data.info.VfxIndex = CsvParser.Int16(raw[index++]);
            if (metadata.HasField("ForDead")) Data.info.ForDead = CsvParser.Boolean(raw[index++]);
            if (metadata.HasField("DefaultDead")) Data.info.DefaultOnDead = CsvParser.Boolean(raw[index++]);

            if (metadata.HasField("ScriptId")) Data.Ref.ScriptId = CsvParser.Int32(raw[index++]);
            if (metadata.HasField("Power")) Data.Ref.Power = CsvParser.Int32(raw[index++]);
            if (metadata.HasField("Rate")) Data.Ref.Rate = CsvParser.Int32(raw[index++]);
            if (metadata.HasField("Elements")) Data.Ref.Elements = CsvParser.Byte(raw[index++]);

            if (metadata.HasField("Status"))
            {
                if (metadata.HasOption($"UseStatusList"))
                    Data.status = BattleStatusEntry.ParseBattleStatus(raw[index++], metadata, true);
                else
                    Data.status = (BattleStatus)CsvParser.UInt64(raw[index++]);
            }
        }

        public void WriteEntry(CsvWriter sw, CsvMetaData metadata)
        {
            if (metadata.HasOption($"Include{nameof(Id)}"))
                sw.Int32(Id);

            sw.Byte((Byte)Data.info.Target);
            sw.Boolean(Data.info.DefaultAlly);
            sw.Byte((Byte)Data.info.DisplayStats);
            sw.Int16(Data.info.VfxIndex);
            sw.Boolean(Data.info.ForDead);
            sw.Boolean(Data.info.DefaultOnDead);

            sw.Int32(Data.Ref.ScriptId);
            sw.Int32(Data.Ref.Power);
            sw.Int32(Data.Ref.Rate);
            sw.Byte(Data.Ref.Elements);

            if (metadata.HasOption($"UseStatusList"))
                BattleStatusEntry.WriteBattleStatus(sw, metadata, Data.status, true);
            else
                sw.UInt64((UInt64)Data.status);
        }
    }
}
