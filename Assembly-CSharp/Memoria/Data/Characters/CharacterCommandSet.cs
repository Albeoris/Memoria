using System;
using System.Collections.Generic;
using Memoria.Prime.CSV;

namespace Memoria.Data
{
    public sealed class CharacterCommandSet : ICsvEntry
    {
        public static List<BattleCommandMenu> SupportedMenus = [BattleCommandMenu.Attack, BattleCommandMenu.Defend, BattleCommandMenu.Ability1, BattleCommandMenu.Ability2, BattleCommandMenu.Item, BattleCommandMenu.Change];

        public CharacterPresetId Id;

        // These should be sorted in the same order as BattleCommandMenu
        public BattleCommandId[] Regular = new BattleCommandId[6];
        public BattleCommandId[] Trance = new BattleCommandId[6];

        public void ParseEntry(String[] raw, CsvMetaData metadata)
        {
            Int32 index = 0;

            if (metadata.HasOption($"Include{nameof(Id)}"))
                Id = (CharacterPresetId)CsvParser.Int32(raw[index++]);
            else
                Id = (CharacterPresetId)(-1);
            if (metadata.HasOption($"IncludeFullSet"))
            {
                for (Int32 i = 0; i < Regular.Length; i++)
                    Regular[i] = (BattleCommandId)CsvParser.Int32(raw[index++]);
                for (Int32 i = 0; i < Trance.Length; i++)
                    Trance[i] = (BattleCommandId)CsvParser.Int32(raw[index++]);
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
