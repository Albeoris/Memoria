using Memoria.Prime.CSV;
using System;

namespace Memoria.Data
{
    public class BattleMagicSwordSet : ICsvEntry
    {
        public Int32 Id;
        public CharacterId Supporter;
        public CharacterId Beneficiary;
        public Int32[] BaseAbilities;
        public Int32[] UnlockedAbilities;
        public BattleStatus SupporterBlockingStatus;
        public BattleStatus BeneficiaryBlockingStatus;

        public void ParseEntry(String[] raw, CsvMetaData metadata)
        {
            Int32 index = 0;
            Id = CsvParser.Int32(raw[index++]);
            Supporter = (CharacterId)CsvParser.Int32(raw[index++]);
            Beneficiary = (CharacterId)CsvParser.Int32(raw[index++]);
            BaseAbilities = CsvParser.AnyAbilityArray(raw[index++]);
            UnlockedAbilities = CsvParser.AnyAbilityArray(raw[index++]);

            if (metadata.HasOption($"IncludeStatusBlockers"))
            {
                metadata.AddOption("UnshiftStatuses");
                SupporterBlockingStatus = BattleStatusEntry.ParseBattleStatus(raw[index++], metadata);
                BeneficiaryBlockingStatus = BattleStatusEntry.ParseBattleStatus(raw[index++], metadata);
            }
            else
            {
                SupporterBlockingStatus = BattleStatus.Silence | BattleStatus.Confuse | BattleStatus.Berserk | BattleStatus.Sleep | BattleStatus.Heat | BattleStatus.Mini;
                BeneficiaryBlockingStatus = BattleStatus.Sleep | BattleStatus.Mini;
            }
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
                metadata.AddOption("UnshiftStatuses");
                BattleStatusEntry.WriteBattleStatus(sw, metadata, SupporterBlockingStatus);
                BattleStatusEntry.WriteBattleStatus(sw, metadata, BeneficiaryBlockingStatus);
            }
        }
    }
}
