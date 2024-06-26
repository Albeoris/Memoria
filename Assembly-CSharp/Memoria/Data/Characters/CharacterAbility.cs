using System;
using FF9;
using Memoria.Prime.CSV;

namespace Memoria.Data
{
    public sealed class CharacterAbility : ICsvEntry
    {
        private const Boolean ExportWithType = true;

        public Int32 Id;
        public Byte Ap;

        public BattleAbilityId ActiveId => ff9abil.GetActiveAbilityFromAbilityId(Id);
        public SupportAbility PassiveId => ff9abil.GetSupportAbilityFromAbilityId(Id);
        public Boolean IsPassive => ff9abil.IsAbilitySupport(Id);

        public void ParseEntry(String[] raw, CsvMetaData metadata)
        {
            Int32 index = 0;

            Id = CsvParser.AnyAbility(raw[index++]);
            Ap = CsvParser.Byte(raw[index++]);
        }

        public void WriteEntry(CsvWriter writer, CsvMetaData metadata)
        {
            writer.AnyAbility(Id);
            writer.Byte(Ap);
        }
    }
}