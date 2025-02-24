using FF9;
using Memoria.Prime.CSV;
using System;

namespace Memoria.Data
{
    public sealed class CharacterAbility : ICsvEntry
    {
        private const Boolean ExportWithType = true;

        public Int32 Id;
        public Int32 Ap;

        public BattleAbilityId ActiveId => ff9abil.GetActiveAbilityFromAbilityId(Id);
        public SupportAbility PassiveId => ff9abil.GetSupportAbilityFromAbilityId(Id);
        public Boolean IsPassive => ff9abil.IsAbilitySupport(Id);

        public void ParseEntry(String[] raw, CsvMetaData metadata)
        {
            Int32 index = 0;

            Id = CsvParser.AnyAbility(raw[index++]);
            Ap = CsvParser.Int32(raw[index++]);
        }

        public void WriteEntry(CsvWriter writer, CsvMetaData metadata)
        {
            writer.AnyAbility(Id);
            writer.Int32(Ap);
        }
    }
}
