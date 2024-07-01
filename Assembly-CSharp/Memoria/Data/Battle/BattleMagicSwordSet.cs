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

        public void ParseEntry(String[] raw, CsvMetaData metadata)
        {
            Int32 index = 0;
            Id = CsvParser.Int32(raw[index++]);
            Supporter = (CharacterId)CsvParser.Int32(raw[index++]);
            Beneficiary = (CharacterId)CsvParser.Int32(raw[index++]);
            BaseAbilities = CsvParser.AnyAbilityArray(raw[index++]);
            UnlockedAbilities = CsvParser.AnyAbilityArray(raw[index++]);
        }

        public void WriteEntry(CsvWriter sw, CsvMetaData metadata)
        {
            sw.Int32(Id);
            sw.Int32((Int32)Supporter);
            sw.Int32((Int32)Beneficiary);
            sw.AnyAbilityArray(BaseAbilities);
            sw.AnyAbilityArray(UnlockedAbilities);
        }
    }
}
