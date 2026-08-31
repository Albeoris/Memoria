using System;
using System.Collections.Generic;
using Memoria.Database;
using Memoria.Prime.CSV;

namespace Memoria.Data
{
    public sealed class CharacterCommandSet : ICsvEntry
    {
        public static List<BattleCommandMenu> SupportedMenus = [BattleCommandMenu.Attack, BattleCommandMenu.Defend, BattleCommandMenu.Ability1, BattleCommandMenu.Ability2, BattleCommandMenu.Item, BattleCommandMenu.Change];

        public CharacterPresetId Id;
        public CharacterCommandSet Data;

        // These should be sorted in the same order as BattleCommandMenu
        public BattleCommandId[] Regular = new BattleCommandId[6];
        public BattleCommandId[] Trance = new BattleCommandId[6];

        public static CharacterCommandSet GetExisting(CharacterPresetId id)
        {
            if (CharacterCommands.CommandSets.TryGetValue(id, out CharacterCommandSet result))
                return result;
            throw new NotSupportedException($"The option AppendMode must be used to patch existing entries but the entry {id} doesn't exist");
        }

        public void ParseDataEntry(String[] raw, CsvMetaData metadata, ref Int32 index)
        {
            if (metadata.HasOption($"IncludeFullSet") || metadata.IsAppendMode)
            {
                if (metadata.HasField("Attack")) Regular[0] = (BattleCommandId)CsvParser.Int32(raw[index++]);
                if (metadata.HasField("Defend")) Regular[1] = (BattleCommandId)CsvParser.Int32(raw[index++]);
                if (metadata.HasField("First")) Regular[2] = (BattleCommandId)CsvParser.Int32(raw[index++]);
                if (metadata.HasField("Second")) Regular[3] = (BattleCommandId)CsvParser.Int32(raw[index++]);
                if (metadata.HasField("Item")) Regular[4] = (BattleCommandId)CsvParser.Int32(raw[index++]);
                if (metadata.HasField("Change")) Regular[5] = (BattleCommandId)CsvParser.Int32(raw[index++]);
                if (metadata.HasField("TranceAttack")) Trance[0] = (BattleCommandId)CsvParser.Int32(raw[index++]);
                if (metadata.HasField("TranceDefend")) Trance[1] = (BattleCommandId)CsvParser.Int32(raw[index++]);
                if (metadata.HasField("TranceFirst")) Trance[2] = (BattleCommandId)CsvParser.Int32(raw[index++]);
                if (metadata.HasField("TranceSecond")) Trance[3] = (BattleCommandId)CsvParser.Int32(raw[index++]);
                if (metadata.HasField("TranceItem")) Trance[4] = (BattleCommandId)CsvParser.Int32(raw[index++]);
                if (metadata.HasField("TranceChange")) Trance[5] = (BattleCommandId)CsvParser.Int32(raw[index++]);
            }
            else
            {
                Regular[0] = Trance[0] = BattleCommandId.Attack;
                Regular[1] = Trance[1] = BattleCommandId.Defend;
                Regular[2] = (BattleCommandId)CsvParser.Int32(raw[index++]);
                Regular[3] = (BattleCommandId)CsvParser.Int32(raw[index++]);
                Trance[2] = (BattleCommandId)CsvParser.Int32(raw[index++]);
                Trance[3] = (BattleCommandId)CsvParser.Int32(raw[index++]);
                Regular[4] = Trance[4] = BattleCommandId.Item;
                Regular[5] = Trance[5] = BattleCommandId.Change;
            }
        }

        public void ParseEntry(String[] raw, CsvMetaData metadata)
        {
            Int32 index = 0;
            if (metadata.HasOption($"Include{nameof(Id)}") || metadata.IsAppendMode)
                Id = (CharacterPresetId)CsvParser.Int32(raw[index++]);
            else
                Id = (CharacterPresetId)(-1);
            Data = metadata.IsAppendMode ? GetExisting(Id) : this;
            Data.ParseDataEntry(raw, metadata, ref index);
        }

        public void WriteEntry(CsvWriter sw, CsvMetaData metadata)
        {
            if (metadata.HasOption($"Include{nameof(Id)}"))
                sw.Int32((Int32)Id);
            if (metadata.HasOption($"IncludeFullSet"))
            {
                for (Int32 i = 0; i < Regular.Length; i++)
                    sw.Int32((Int32)Regular[i]);
                for (Int32 i = 0; i < Trance.Length; i++)
                    sw.Int32((Int32)Trance[i]);
            }
            else
            {
                sw.Int32((Int32)Regular[2]);
                sw.Int32((Int32)Regular[3]);
                sw.Int32((Int32)Trance[2]);
                sw.Int32((Int32)Trance[3]);
            }
        }

        public BattleCommandId GetRegular(BattleCommandMenu menu)
        {
            Int32 number = (Int32)menu;
            if (number < 0 || number >= Regular.Length)
                throw new ArgumentOutOfRangeException(nameof(menu), menu, "Menu must be one of the 6 base menu types.");
            return Regular[number];
        }

        public BattleCommandId GetTrance(BattleCommandMenu menu)
        {
            Int32 number = (Int32)menu;
            if (number < 0 || number >= Trance.Length)
                throw new ArgumentOutOfRangeException(nameof(menu), menu, "Menu must be one of the 6 base menu types.");
            return Trance[number];
        }

        public BattleCommandId Get(Boolean isTrance, BattleCommandMenu menu)
        {
            return isTrance ? GetTrance(menu) : GetRegular(menu);
        }
    }
}
