using System;
using System.Collections.Generic;
using System.ComponentModel;
using Memoria.Prime.CSV;

namespace Memoria.Data
{
    public sealed class CharacterCommand : ICsvEntry
    {
        public CharacterCommandType Type;
        public Byte Ability;
        public Byte[] Abilities;

        public IEnumerable<BattleAbilityId> EnumerateAbilities()
        {
            switch (Type)
            {
                case CharacterCommandType.Throw:
                case CharacterCommandType.Normal:
                    yield return (BattleAbilityId)Ability;
                    yield break;
                case CharacterCommandType.Ability:
                    foreach (Int32 id in Abilities)
                        yield return (BattleAbilityId)id;
                    yield break;
            }
        }

        public BattleAbilityId GetAbilityId(Int32 index = -1)
        {
            switch (Type)
            {
                case CharacterCommandType.Throw:
                case CharacterCommandType.Normal:
                    return (BattleAbilityId)Ability;
                case CharacterCommandType.Ability:
                    if (index < 0 || index >= Abilities.Length)
                        return BattleAbilityId.Void;
                    return (BattleAbilityId)Abilities[index];
                case CharacterCommandType.Item:
                    return BattleAbilityId.Void;
            }
            throw new InvalidEnumArgumentException($"[CharacterCommand] A command has an invalid type {Type}", (Int32)Type, typeof(CharacterCommandType));
        }

        public void ParseEntry(String[] raw)
        {
            Type = (CharacterCommandType)CsvParser.Byte(raw[0]);
            Ability = CsvParser.Byte(raw[1]);
            Abilities = CsvParser.ByteArray(raw[2]);
        }

        public void WriteEntry(CsvWriter sw)
        {
            sw.Byte((Byte)Type);
            sw.Byte(Ability);
            sw.ByteArray(Abilities);
        }
    }
}