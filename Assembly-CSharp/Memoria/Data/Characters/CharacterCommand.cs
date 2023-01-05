using System;
using System.Collections.Generic;
using System.ComponentModel;
using Memoria.Prime.CSV;

namespace Memoria.Data
{
    public sealed class CharacterCommand : ICsvEntry
    {
        public BattleCommandId Id;
        public CharacterCommandType Type;
        public Int32 MainEntry;
        public Int32[] ListEntry;

        public IEnumerable<BattleAbilityId> EnumerateAbilities()
        {
            switch (Type)
            {
                case CharacterCommandType.Throw:
                case CharacterCommandType.Normal:
                    yield return(BattleAbilityId)MainEntry;
                    yield break;
                case CharacterCommandType.Ability:
                    foreach (Int32 id in ListEntry)
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
                    return (BattleAbilityId)MainEntry;
                case CharacterCommandType.Ability:
                    if (index < 0 || index >= ListEntry.Length)
                        return BattleAbilityId.Void;
                    return (BattleAbilityId)ListEntry[index];
                case CharacterCommandType.Item:
                    return BattleAbilityId.Void;
            }
            throw new InvalidEnumArgumentException($"[CharacterCommand] A command has an invalid type {Type}", (Int32)Type, typeof(CharacterCommandType));
        }

        public void ParseEntry(String[] raw, CsvMetaData metadata)
        {
            Int32 index = 0;

            if (metadata.HasOption($"Include{nameof(Id)}"))
                Id = (BattleCommandId)CsvParser.Int32(raw[index++]);
            else
                Id = (BattleCommandId)(-1);
            Type = (CharacterCommandType)CsvParser.Byte(raw[index++]);
            MainEntry = CsvParser.Int32(raw[index++]);
            ListEntry = CsvParser.Int32Array(raw[index++]);
        }

        public void WriteEntry(CsvWriter sw, CsvMetaData metadata)
        {
            if (metadata.HasOption($"Include{nameof(Id)}"))
                sw.Int32((Int32)Id);
            sw.Byte((Byte)Type);
            sw.Int32(MainEntry);
            sw.Int32Array(ListEntry);
        }
    }
}