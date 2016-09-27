using System;
using System.Collections.Generic;
using System.Reflection;
using Assets.Sources.Scripts.UI.Common;

namespace Memoria
{
    public static class FF9TextToolAccessor
    {
        private const BindingFlags SearchFlags = BindingFlags.Static | BindingFlags.NonPublic;

        private static readonly Type Type = typeof(FF9TextTool);

        public static readonly Action<String[]> SetFieldText = StaticArraySetter("fieldText");
        public static readonly Action<String[][]> SetTableText = StaticTableSetter("tableText");
        public static readonly Action<String[]> SetItemBattleDesc = StaticArraySetter("itemBattleDesc");
        public static readonly Action<String[]> SetItemHelpDesc = StaticArraySetter("itemHelpDesc");
        public static readonly Action<String[]> SetItemName = StaticArraySetter("itemName");
        public static readonly Action<String[]> SetImportantItemHelpDesc = StaticArraySetter("importantItemHelpDesc");
        public static readonly Action<String[]> SetImportantItemName = StaticArraySetter("importantItemName");
        public static readonly Action<String[]> SetImportantSkinDesc = StaticArraySetter("importantSkinDesc");
        public static readonly Action<String[]> SetSupportAbilityHelpDesc = StaticArraySetter("supportAbilityHelpDesc");
        public static readonly Action<String[]> SetSupportAbilityName = StaticArraySetter("supportAbilityName");
        public static readonly Action<String[]> SetActionAbilityHelpDesc = StaticArraySetter("actionAbilityHelpDesc");
        public static readonly Action<String[]> SetActionAbilityName = StaticArraySetter("actionAbilityName");
        public static readonly Action<String[]> SetCommandName = StaticArraySetter("commandName");
        public static readonly Action<String[]> SetCommandHelpDesc = StaticArraySetter("commandHelpDesc");
        public static readonly Action<String[]> SetBattleText = StaticArraySetter("battleText");
        public static readonly Action<String[]> SetCardName = StaticArraySetter("cardName");
        public static readonly Action<String[]> SetChocoUiText = StaticArraySetter("chocoUIText");
        public static readonly Action<String[]> SetCardLvName = StaticArraySetter("cardLvName");
        public static readonly Action<String[]> SetCmdTitleText = StaticArraySetter("cmdTitleText");
        public static readonly Action<String[]> SetFollowText = StaticArraySetter("followText");
        public static readonly Action<String[]> SetLibraText = StaticArraySetter("libraText");
        public static readonly Action<String[]> SetWorldLocationText = StaticArraySetter("worldLocationText");
        public static readonly Func<Int32> GetFieldZoneId = StaticInt32Getter("fieldZoneId");
        public static readonly Func<Int32> GetBattleZoneId = StaticInt32Getter("battleZoneId");
        public static readonly Func<Dictionary<Int32, String>> GetLocationName = Expressions.MakeStaticGetter<Dictionary<Int32, String>>(Type.GetField("locationName", SearchFlags));

        private static Action<String[]> StaticArraySetter(String fieldName)
        {
            return Expressions.MakeStaticSetter<String[]>(Type.GetField(fieldName, SearchFlags));
        }

        private static Action<String[][]> StaticTableSetter(String fieldName)
        {
            return Expressions.MakeStaticSetter<String[][]>(Type.GetField(fieldName, SearchFlags));
        }

        private static Func<Int32> StaticInt32Getter(String fieldName)
        {
            return Expressions.MakeStaticGetter<Int32>(Type.GetField(fieldName, SearchFlags));
        }

        public static String ActionAbilityName(Int32 abilityId)
        {
            return FF9TextTool.ActionAbilityName(abilityId);
        }

        public static String SupportAbilityName(Int32 abilityId)
        {
            return FF9TextTool.SupportAbilityName(abilityId);
        }

        public static String ItemName(Int32 itemId)
        {
            return FF9TextTool.ItemName(itemId);
        }
    }
}