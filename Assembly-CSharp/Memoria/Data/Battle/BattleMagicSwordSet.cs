using Memoria.Prime.CSV;
using System;

namespace Memoria.Data
{
    public class BattleMagicSwordSet : ICsvEntry
    {
        public Int32 Id;
        public BattleMagicSwordSet Data;

        public CharacterId Supporter;
        public CharacterId Beneficiary;
        public Int32[] BaseAbilities;
        public Int32[] UnlockedAbilities;
        public BattleStatus SupporterBlockingStatus;
        public BattleStatus BeneficiaryBlockingStatus;

        public static BattleMagicSwordSet GetExisting(Int32 id)
        {
            if (FF9BattleDB.MagicSwordData.TryGetValue(id, out BattleMagicSwordSet result))
                return result;
            throw new NotSupportedException($"The option AppendMode must be used to patch existing entries but the entry {id} doesn't exist");
        }

        public void ParseDataEntry(String[] raw, CsvMetaData metadata, ref Int32 index)
        {
            if (metadata.HasField("Supporter")) Supporter = (CharacterId)CsvParser.Int32(raw[index++]);
            if (metadata.HasField("Beneficiary")) Beneficiary = (CharacterId)CsvParser.Int32(raw[index++]);
            if (metadata.HasField("BaseAbilities")) BaseAbilities = CsvParser.AnyAbilityArray(raw[index++]);
            if (metadata.HasField("UnlockedAbilities")) UnlockedAbilities = CsvParser.AnyAbilityArray(raw[index++]);

            if (metadata.HasOption($"IncludeStatusBlockers"))
            {
                if (metadata.HasField("SupporterBlockingStatus")) SupporterBlockingStatus = BattleStatusEntry.ParseBattleStatus(raw[index++], metadata, true);
                if (metadata.HasField("BeneficiaryBlockingStatus")) BeneficiaryBlockingStatus = BattleStatusEntry.ParseBattleStatus(raw[index++], metadata, true);
            }
            else if (!metadata.IsAppendMode)
            {
                SupporterBlockingStatus = BattleStatus.Silence | BattleStatus.Confuse | BattleStatus.Berserk | BattleStatus.Sleep | BattleStatus.Heat | BattleStatus.Mini;
                BeneficiaryBlockingStatus = BattleStatus.Sleep | BattleStatus.Mini;
            }
        }

        public void ParseEntry(String[] raw, CsvMetaData metadata)
        {
            Int32 index = 0;
            Id = CsvParser.Int32(raw[index++]);
            Data = metadata.IsAppendMode ? GetExisting(Id) : this;
            Data.ParseDataEntry(raw, metadata, ref index);
        }

        public void WriteEntry(CsvWriter sw, CsvMetaData metadata)
        {
            sw.Int32(Id);
            sw.Int32((Int32)Supporter);
            sw.Int32((Int32)Beneficiary);
            sw.AnyAbilityArray(BaseAbilities);
            sw.AnyAbilityArray(UnlockedAbilities);
            if (metadata.HasOption($"IncludeStatusBlockers"))
            {
                BattleStatusEntry.WriteBattleStatus(sw, metadata, SupporterBlockingStatus, true);
                BattleStatusEntry.WriteBattleStatus(sw, metadata, BeneficiaryBlockingStatus, true);
            }
        }
    }
}
