using System;
using FF9;

namespace Memoria
{
    public sealed class CharacterAbility : ICsvEntry
    {
        public Byte Id;
        public Byte Ap;

        public Byte SkillId => Id;
        public Byte AbilityId => checked((Byte)(Id - 192));
        public Boolean IsPassive => Id >= 192;

        public void ParseEntry(String[] raw)
        {
            Id = CsvParser.Byte(raw[0]);
            Ap = CsvParser.Byte(raw[1]);
        }

        public void WriteEntry(CsvWriter writer)
        {
            writer.Byte(Id);
            writer.Byte(Ap);
        }

        public void ToPaData(PA_DATA target)
        {
            target.id = Id;
            target.max_ap = Ap;
        }

        public static CharacterAbility FromPaData(PA_DATA entry)
        {
            return new CharacterAbility {Id = entry.id, Ap = entry.max_ap};
        }
    }
}